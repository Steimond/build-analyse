using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace VectraCode.AssetDependencyExplorer
{
    public enum DependencyMode { DirectDependencies, RecursiveDependencies, DirectReferrers }

    public static class DependencyScanner
    {
        public static DiagnosticReport Scan(string targetPath, string folder, DependencyMode mode, Func<string, float, bool> cancel = null)
        {
            ScanUtility.ValidateFolder(folder);
            if (string.IsNullOrEmpty(targetPath) || AssetDatabase.IsValidFolder(targetPath) || AssetDatabase.LoadMainAssetAtPath(targetPath) == null)
                throw new ArgumentException("Choose an asset file in the Project window, not a scene object or folder.");
            var rows = new List<DiagnosticRow>();
            var errors = new List<string>();
            if (mode == DependencyMode.DirectReferrers)
            {
                var paths = AssetDatabase.GetAllAssetPaths().Where(p => ScanUtility.IsInFolder(p, folder) && !AssetDatabase.IsValidFolder(p)).OrderBy(p => p, StringComparer.Ordinal).ToArray();
                for (var i = 0; i < paths.Length; i++)
                {
                    ScanUtility.CheckCancellation(cancel, paths[i], i, paths.Length);
                    if (paths[i] == targetPath) continue;
                    try
                    {
                        if (AssetDatabase.GetDependencies(paths[i], false).Contains(targetPath, StringComparer.Ordinal))
                            rows.Add(new DiagnosticRow { path = paths[i], detail = "Directly references " + targetPath, group = "Direct referrer", severity = "Info" });
                    }
                    catch (Exception ex) { errors.Add(paths[i] + ": " + ex.Message); }
                }
            }
            else
            {
                ScanUtility.CheckCancellation(cancel, targetPath, 0, 1);
                var recursive = mode == DependencyMode.RecursiveDependencies;
                var direct = new HashSet<string>(AssetDatabase.GetDependencies(targetPath, false), StringComparer.Ordinal);
                foreach (var path in AssetDatabase.GetDependencies(targetPath, recursive).Distinct(StringComparer.Ordinal).OrderBy(p => p, StringComparer.Ordinal))
                    if (path != targetPath)
                        rows.Add(new DiagnosticRow { path = path, detail = "Referenced by " + targetPath + (recursive ? " (direct or transitive)" : " (direct)"),
                            group = direct.Contains(path) ? "Direct dependency" : "Transitive dependency", severity = "Info" });
            }
            return DiagnosticReport.Create("Asset Dependency Explorer", targetPath + " | " + mode + (mode == DependencyMode.DirectReferrers ? " | " + folder : ""), rows, errors);
        }
    }
}
