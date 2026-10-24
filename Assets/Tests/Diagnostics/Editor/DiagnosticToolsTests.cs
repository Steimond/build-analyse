using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Dep = VectraCode.AssetDependencyExplorer;
using Dup = VectraCode.DuplicateAssetFinder;
using Tex = VectraCode.TextureBudgetAuditor;
using Pref = VectraCode.PrefabMissingScriptScanner;

namespace VectraCode.Diagnostics.Tests
{
    public sealed class DiagnosticToolsTests
    {
        private string folder;
        [SetUp]
        public void SetUp()
        {
            folder = "Assets/DiagnosticsTest-" + Guid.NewGuid().ToString("N");
            AssetDatabase.CreateFolder("Assets", Path.GetFileName(folder));
        }
        [TearDown]
        public void TearDown() => AssetDatabase.DeleteAsset(folder);

        [Test]
        public void DependenciesDistinguishDirectTransitiveAndReverseEdges()
        {
            var leaf = ScriptableObject.CreateInstance<ReferenceFixture>();
            var middle = ScriptableObject.CreateInstance<ReferenceFixture>();
            var root = ScriptableObject.CreateInstance<ReferenceFixture>();
            middle.reference = leaf;
            root.reference = middle;
            AssetDatabase.CreateAsset(leaf, folder + "/leaf.asset");
            AssetDatabase.CreateAsset(middle, folder + "/middle.asset");
            AssetDatabase.CreateAsset(root, folder + "/root.asset");
            AssetDatabase.SaveAssets();
            var direct = Dep.DependencyScanner.Scan(folder + "/root.asset", folder, Dep.DependencyMode.DirectDependencies);
            Assert.IsTrue(direct.scanCompleted);
            Assert.IsTrue(direct.rows.Any(r => r.path == folder + "/middle.asset"));
            Assert.IsFalse(direct.rows.Any(r => r.path == folder + "/leaf.asset" || r.path == folder + "/root.asset"));
            var recursive = Dep.DependencyScanner.Scan(folder + "/root.asset", folder, Dep.DependencyMode.RecursiveDependencies);
            Assert.AreEqual("Transitive dependency", recursive.rows.Single(r => r.path == folder + "/leaf.asset").group);
            var reverse = Dep.DependencyScanner.Scan(folder + "/leaf.asset", folder, Dep.DependencyMode.DirectReferrers);
            CollectionAssert.AreEqual(new[] { folder + "/middle.asset" }, reverse.rows.Select(r => r.path));
        }

        [Test]
        public void DuplicateScanHashesContentsInsteadOfOnlyComparingLengths()
        {
            File.WriteAllBytes(folder + "/a.bytes", new byte[] { 1, 2, 3, 4 });
            File.WriteAllBytes(folder + "/b.bytes", new byte[] { 1, 2, 3, 4 });
            File.WriteAllBytes(folder + "/c.bytes", new byte[] { 4, 3, 2, 1 });
            File.WriteAllBytes(folder + "/empty.bytes", Array.Empty<byte>());
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            var report = Dup.DuplicateScanner.Scan(folder);
            Assert.IsTrue(report.scanCompleted);
            CollectionAssert.AreEquivalent(new[] { folder + "/a.bytes", folder + "/b.bytes" }, report.rows.Select(r => r.path));
            Assert.AreEqual(report.rows[0].group, report.rows[1].group);
            Assert.AreEqual(64, report.rows[0].group.Length);
            Assert.AreEqual(4, report.rows[0].bytes);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, File.ReadAllBytes(folder + "/a.bytes"));
        }

        [Test]
        public void DuplicateScanCanCancelAndRejectPathsOutsideAssets()
        {
            File.WriteAllBytes(folder + "/a.bytes", new byte[] { 1 });
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Assert.Throws<OperationCanceledException>(() => Dup.DuplicateScanner.Scan(folder, (path, fraction) => true));
            Assert.Throws<IOException>(() => Dup.DuplicateScanner.SafePath(Directory.GetCurrentDirectory(), "Assets/../../outside.bytes"));
        }

        [TestCase(4, 2, false, 32)]
        [TestCase(4, 2, true, 44)]
        [TestCase(3, 5, true, 72)]
        [TestCase(1, 1, true, 4)]
        public void TextureReferenceComputesExactMipDimensions(int width, int height, bool mipmaps, long expected)
            => Assert.AreEqual(expected, Tex.TextureScanner.RgbaReferenceBytes(width, height, mipmaps));

        [Test]
        public void TextureScanUsesImportedDimensionsAndDoesNotModifyImporter()
        {
            var path = folder + "/texture.png";
            var texture = new Texture2D(8, 4, TextureFormat.RGBA32, false);
            try { File.WriteAllBytes(path, texture.EncodeToPNG()); }
            finally { UnityEngine.Object.DestroyImmediate(texture); }
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.isReadable = true;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            var before = File.ReadAllBytes(path + ".meta");
            var report = Tex.TextureScanner.Scan(folder, 4, 16);
            Assert.IsTrue(report.scanCompleted);
            var row = report.rows.Single();
            Assert.AreEqual("Review", row.severity);
            Assert.AreEqual(128, row.bytes);
            StringAssert.Contains("Read/Write enabled", row.detail);
            CollectionAssert.AreEqual(before, File.ReadAllBytes(path + ".meta"));
        }

        [Test]
        public void PrefabScanDetectsMissingScriptOnInactiveChildWithoutSavingChanges()
        {
            var root = new GameObject("root");
            var child = new GameObject("child");
            child.transform.SetParent(root.transform);
            child.SetActive(false);
            child.AddComponent<PrefabProbe>();
            var path = folder + "/broken.prefab";
            string guid;
            try
            {
                guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(child.GetComponent<PrefabProbe>())));
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
            Assert.IsNotEmpty(guid);
            // Corrupt only the known script GUID in a prefab generated by Unity, to simulate a deleted script.
            var yaml = File.ReadAllText(path);
            StringAssert.Contains(guid, yaml);
            File.WriteAllText(path, yaml.Replace(guid, "00000000000000000000000000001234"));
            var previousIgnore = LogAssert.ignoreFailingMessages;
            try
            {
                LogAssert.ignoreFailingMessages = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                var before = File.ReadAllBytes(path);
                var report = Pref.PrefabScriptScanner.Scan(folder);
                Assert.IsTrue(report.scanCompleted);
                var row = report.rows.Single(r => r.severity == "Review");
                Assert.AreEqual(1, row.bytes);
                StringAssert.Contains("child[0]", row.group);
                CollectionAssert.AreEqual(before, File.ReadAllBytes(path));
            }
            finally { LogAssert.ignoreFailingMessages = previousIgnore; }
        }

        [Test]
        public void FolderBoundaryDoesNotIncludeSimilarlyNamedSiblings()
        {
            Assert.IsTrue(Dep.ScanUtility.IsInFolder("Assets/Art/a.png", "Assets/Art"));
            Assert.IsFalse(Dep.ScanUtility.IsInFolder("Assets/Artwork/a.png", "Assets/Art"));
            Assert.Throws<ArgumentException>(() => Tex.TextureScanner.Scan("Packages", 2048, 16));
        }

        [Test]
        public void CsvContainsIncompleteStatusErrorsAndEscapedFormulaPaths()
        {
            var report = Dup.DiagnosticReport.Create("Duplicate Asset Finder", folder,
                new System.Collections.Generic.List<Dup.DiagnosticRow> { new Dup.DiagnosticRow { path = "  =SUM(1,2)\"", group = "hash", severity = "Review" } },
                new System.Collections.Generic.List<string> { "Unreadable asset" });
            Assert.IsFalse(report.scanCompleted);
            StringAssert.Contains("\"'  =SUM(1,2)\"\"\"", report.ToCsv());
            StringAssert.Contains(",false,,,Error,0,", report.ToCsv());
            var decoded = JsonUtility.FromJson<Dup.DiagnosticReport>(JsonUtility.ToJson(report));
            Assert.IsFalse(decoded.scanCompleted);
            Assert.AreEqual("Unreadable asset", decoded.errors[0]);
        }
    }
}
