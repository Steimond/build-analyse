using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VectraCode.Release
{
    // Development-only: runs real analyzers against explicitly labelled sample data.
    public static class ScreenshotStudio
    {
        const string Root = "Assets/VectraCodeScreenshotSamples";
        static EditorWindow[] windows;
        static int index;
        static double captureAt;

        [MenuItem("Tools/Build Diagnostics/Prepare Screenshot Windows")]
        public static void Prepare()
        {
            Directory.CreateDirectory(Root);
            Directory.CreateDirectory("Artifacts/Screenshots");
            foreach (var item in new[] { "MenuAtlas", "MenuAtlasCopy", "EnvironmentAtlas" })
            {
                var texture = new Texture2D(512, 512);
                var pixels = Enumerable.Repeat(item == "EnvironmentAtlas" ? Color.gray : Color.cyan, 512 * 512).ToArray();
                texture.SetPixels(pixels); texture.Apply();
                File.WriteAllBytes(Root + "/" + item + ".png", texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
            }
            AssetDatabase.Refresh();
            var material = new Material(Shader.Find("Standard") ?? Shader.Find("Hidden/InternalErrorShader"));
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/MenuAtlas.png");
            var materialPath = Root + "/SampleMaterial.mat";
            if (AssetDatabase.LoadAssetAtPath<Material>(materialPath) == null) AssetDatabase.CreateAsset(material, materialPath);
            else UnityEngine.Object.DestroyImmediate(material);
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "SampleEnvironment";
            obj.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            PrefabUtility.SaveAsPrefabAsset(obj, Root + "/SampleEnvironment.prefab");
            UnityEngine.Object.DestroyImmediate(obj);
            AssetDatabase.SaveAssets();
            var baseline = Path.GetFullPath("Artifacts/Screenshots/sample-baseline.apk");
            var current = Path.GetFullPath("Artifacts/Screenshots/sample-current.apk");
            Archive(baseline, false); Archive(current, true);
            windows = new[] {
                Window("BuildDiffTool.Editor.BuildDiffWindow", "Build Diff Tool"),
                Window(typeof(VectraCode.BuildSizeAnalyzer.ArchiveSizeWindow)),
                Window(typeof(VectraCode.ReleasePreflight.PreflightWindow)),
                Window(typeof(VectraCode.AssetDependencyExplorer.DiagnosticWindow)),
                Window(typeof(VectraCode.DuplicateAssetFinder.DiagnosticWindow)),
                Window(typeof(VectraCode.TextureBudgetAuditor.DiagnosticWindow)),
                Window(typeof(VectraCode.PrefabMissingScriptScanner.DiagnosticWindow)) };
            Set(windows[0], "baselinePath", baseline); Set(windows[0], "currentPath", current);
            windows[0].GetType().GetMethod("AnalyzeBuilds", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(windows[0], null);
            Set(windows[1], "report", VectraCode.BuildSizeAnalyzer.ArchiveSizeReport.Analyze(current));
            Set(windows[1], "expanded", true);
            Set(windows[2], "report", VectraCode.ReleasePreflight.PreflightChecks.Scan());
            Set(windows[3], "target", AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/SampleEnvironment.prefab"));
            Set(windows[3], "mode", VectraCode.AssetDependencyExplorer.DependencyMode.RecursiveDependencies);
            Set(windows[3], "report", VectraCode.AssetDependencyExplorer.DependencyScanner.Scan(Root + "/SampleEnvironment.prefab", Root, VectraCode.AssetDependencyExplorer.DependencyMode.RecursiveDependencies));
            Set(windows[4], "report", VectraCode.DuplicateAssetFinder.DuplicateScanner.Scan(Root));
            Set(windows[5], "dimensionBudget", 256);
            Set(windows[5], "report", VectraCode.TextureBudgetAuditor.TextureScanner.Scan(Root, 256, 16));
            Set(windows[6], "report", VectraCode.PrefabMissingScriptScanner.PrefabScriptScanner.Scan(Root));
            foreach (var window in windows.Skip(3)) Set(window, "folderAsset", AssetDatabase.LoadAssetAtPath<DefaultAsset>(Root));
            index = 0;
            Debug.Log("Screenshot windows ready. Sample archives are synthetic ZIP fixtures, not runnable builds. Prefab scan intentionally shows a clean prefab. Use Capture Screenshot Windows in an interactive Editor.");
        }

        [MenuItem("Tools/Build Diagnostics/Capture Screenshot Windows")]
        public static void Capture()
        {
            if (Application.isBatchMode) throw new InvalidOperationException("Screenshots require an interactive Editor.");
            Prepare();
            ShowNext();
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        static void ShowNext()
        {
            windows[index].position = new Rect(60, 60, 1200, 760);
            windows[index].ShowUtility(); windows[index].Focus(); windows[index].Repaint();
            captureAt = EditorApplication.timeSinceStartup + 2;
        }
        static void Tick()
        {
            if (EditorApplication.timeSinceStartup < captureAt) return;
            try
            {
                var rect = windows[index].position;
                var scale = EditorGUIUtility.pixelsPerPoint;
                var width = Mathf.RoundToInt(rect.width * scale);
                var height = Mathf.RoundToInt(rect.height * scale);
                var pixels = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel(rect.position, width, height);
                var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                texture.SetPixels(pixels); texture.Apply();
                File.WriteAllBytes("Artifacts/Screenshots/" + windows[index].titleContent.text.Replace(' ', '-') + ".png", texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
                windows[index].Close();
                if (++index < windows.Length) ShowNext();
                else { EditorApplication.update -= Tick; Debug.Log("Captured seven real tool windows in Artifacts/Screenshots. Inspect before store upload."); }
            }
            catch (Exception exception) { EditorApplication.update -= Tick; Debug.LogException(exception); }
        }
        static EditorWindow Window(Type type) => Window(type.FullName, type.Namespace.Split('.').Last());
        static EditorWindow Window(string name, string title)
        {
            var type = UnityEngine.Assemblies.CurrentAssemblies.GetLoadedAssemblies().Select(a => a.GetType(name)).First(t => t != null);
            var window = EditorWindow.GetWindow(type);
            // Preserve the product's menu title for store media.
            var titles = new[] { "Build Diff Tool", "Build Size Analyzer", "Release Preflight", "Asset Dependency Explorer", "Duplicate Asset Finder", "Texture Budget Auditor", "Prefab Missing Script Scanner" };
            var names = new[] { "BuildDiffTool", "BuildSizeAnalyzer", "ReleasePreflight", "AssetDependencyExplorer", "DuplicateAssetFinder", "TextureBudgetAuditor", "PrefabMissingScriptScanner" };
            window.titleContent = new GUIContent(titles[Array.FindIndex(names, n => type.Namespace.Contains(n))]);
            return window;
        }
        static void Set(object target, string field, object value) => target.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, value);
        static void Archive(string path, bool newer)
        {
            using (var zip = new ZipArchive(File.Create(path), ZipArchiveMode.Create))
                foreach (var item in new[] { "assets/bin/Data/sharedassets0.assets", "lib/arm64-v8a/libil2cpp.so", "assets/bin/Data/resources.assets", newer ? "assets/bin/Data/level2" : "assets/bin/Data/level1" })
                    using (var stream = zip.CreateEntry(item).Open())
                    {
                        var data = new byte[(item.Contains("sharedassets") && newer ? 3 : 1) * 1024 * 1024];
                        new System.Random(item.Length).NextBytes(data); stream.Write(data, 0, data.Length);
                    }
        }
    }
}

