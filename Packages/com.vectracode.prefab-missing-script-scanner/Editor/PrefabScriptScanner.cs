using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VectraCode.PrefabMissingScriptScanner
{
    public static class PrefabScriptScanner
    {
        public static DiagnosticReport Scan(string folder, Func<string, float, bool> cancel = null)
        {
            ScanUtility.ValidateFolder(folder);
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) throw new InvalidOperationException("Wait until compilation and asset importing finish before scanning.");
            var paths = AssetDatabase.FindAssets("t:Prefab", new[] { folder }).Select(AssetDatabase.GUIDToAssetPath).Where(p => p.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)).Distinct().OrderBy(p => p, StringComparer.Ordinal).ToArray();
            var rows = new List<DiagnosticRow>();
            var errors = new List<string>();
            for (var i = 0; i < paths.Length; i++)
            {
                ScanUtility.CheckCancellation(cancel, paths[i], i, paths.Length);
                try
                {
                    var root = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
                    if (root == null) { errors.Add(paths[i] + ": Could not load prefab asset."); continue; }
                    var count = 0;
                    foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                    {
                        var missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
                        if (missing == 0) continue;
                        count += missing;
                        rows.Add(new DiagnosticRow { path = paths[i], group = HierarchyPath(transform), severity = "Review", bytes = missing,
                            detail = missing + " missing MonoBehaviour slot(s). Restore the original script/GUID or inspect this prefab manually. No components were removed." });
                    }
                    if (count == 0) rows.Add(new DiagnosticRow { path = paths[i], group = "Prefab", severity = "Info", detail = "No missing MonoBehaviour slots detected, including inactive children." });
                }
                catch (Exception ex) { errors.Add(paths[i] + ": " + ex.Message); }
            }
            return DiagnosticReport.Create("Prefab Missing Script Scanner", folder, rows, errors);
        }

        public static string HierarchyPath(Transform transform)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            var segments = new List<string>();
            while (transform != null)
            {
                // Sibling indices disambiguate children with identical names.
                segments.Add(transform.name + "[" + transform.GetSiblingIndex() + "]");
                transform = transform.parent;
            }
            segments.Reverse();
            return string.Join("/", segments);
        }
    }
}
