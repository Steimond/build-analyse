using System.IO;
using UnityEditor;
using UnityEngine;

namespace BuildDiffTool.Release
{
    public static class BuildDiffToolPackageExporter
    {
        private const string ProductRoot = "Assets/Build Diff Tool";
        private const string DefaultFileName = "Build-Diff-Tool-1.0.0.unitypackage";

        [MenuItem("Tools/Build Diff Tool/Export Asset Store Package...")]
        private static void ExportWithSaveDialog()
        {
            var destination = EditorUtility.SaveFilePanel(
                "Export Build Diff Tool",
                Directory.GetCurrentDirectory(),
                DefaultFileName,
                "unitypackage");

            if (string.IsNullOrWhiteSpace(destination))
            {
                return;
            }

            ExportTo(destination);
        }

        public static void ExportToDefaultArtifactsDirectory()
        {
            var destination = Path.GetFullPath(Path.Combine("Artifacts", DefaultFileName));
            ExportTo(destination);
        }

        private static void ExportTo(string destination)
        {
            var directory = Path.GetDirectoryName(destination);
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new IOException("The export destination must have a parent folder.");
            }

            Directory.CreateDirectory(directory);
            AssetDatabase.ExportPackage(
                new[] { ProductRoot },
                destination,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            Debug.Log($"Build Diff Tool Asset Store package exported to: {destination}");
        }
    }
}
