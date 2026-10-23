using System.IO;
using UnityEditor;
using UnityEngine;

namespace VectraCode.Release
{
    public static class PortfolioPackageExporter
    {
        [MenuItem("Tools/Build Diagnostics/Export Standalone Packages")]
        public static void ExportAll()
        {
            Directory.CreateDirectory("Artifacts");
            foreach (var product in new[] { "Build Diff Tool", "Build Size Analyzer", "Release Preflight" })
            {
                var destination = Path.GetFullPath(Path.Combine("Artifacts", product.Replace(' ', '-') + "-1.0.0.unitypackage"));
                AssetDatabase.ExportPackage("Assets/" + product, destination, ExportPackageOptions.Recurse);
                Debug.Log("Exported standalone package: " + destination);
            }
        }
    }
}
