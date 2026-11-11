using System.IO;
using UnityEditor;
using UnityEngine;

namespace VectraCode.Release
{
    public static class PortfolioPackageExporter
    {
        public static void ValidateAndExportAll()
        {
            PortfolioValidator.ValidateAll();
            ExportAll();
        }

        [MenuItem("Tools/Build Diagnostics/Export Standalone Packages")]
        public static void ExportAll()
        {
            Directory.CreateDirectory("Artifacts");
            foreach (var product in new[] { "Build Diff Tool", "Build Size Analyzer", "Release Preflight", "Asset Dependency Explorer", "Duplicate Asset Finder", "Texture Budget Auditor", "Prefab Missing Script Scanner" })
            {
                var destination = Path.GetFullPath(Path.Combine("Artifacts", product.Replace(' ', '-') + "-1.0.0.unitypackage"));
                AssetDatabase.ExportPackage("Packages/com.vectracode." + product.ToLowerInvariant().Replace(' ', '-'), destination, ExportPackageOptions.Recurse);
                Debug.Log("Exported standalone package: " + destination);
            }
        }
    }
}
