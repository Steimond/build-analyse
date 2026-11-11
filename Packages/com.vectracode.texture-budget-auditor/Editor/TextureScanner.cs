using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VectraCode.TextureBudgetAuditor
{
    public static class TextureScanner
    {
        public static long RgbaReferenceBytes(int width, int height, bool mipmaps)
        {
            if (width < 1 || height < 1) throw new ArgumentOutOfRangeException("Texture dimensions must be positive.");
            long total = 0;
            checked
            {
                while (true)
                {
                    total += (long)width * height * 4;
                    if (!mipmaps || (width == 1 && height == 1)) return total;
                    width = Math.Max(1, width / 2);
                    height = Math.Max(1, height / 2);
                }
            }
        }

        public static DiagnosticReport Scan(string folder, int dimensionBudget, int referenceMiBBudget, Func<string, float, bool> cancel = null)
        {
            ScanUtility.ValidateFolder(folder);
            if (dimensionBudget < 1 || referenceMiBBudget < 1) throw new ArgumentException("Budgets must be positive.");
            var paths = AssetDatabase.FindAssets("t:Texture2D", new[] { folder }).Select(AssetDatabase.GUIDToAssetPath).Distinct().OrderBy(p => p, StringComparer.Ordinal).ToArray();
            var rows = new List<DiagnosticRow>();
            var errors = new List<string>();
            for (var i = 0; i < paths.Length; i++)
            {
                ScanUtility.CheckCancellation(cancel, paths[i], i, paths.Length);
                try
                {
                    var importer = AssetImporter.GetAtPath(paths[i]) as TextureImporter;
                    if (importer == null || importer.textureShape != TextureImporterShape.Texture2D) continue;
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(paths[i]);
                    if (texture == null) { errors.Add(paths[i] + ": Texture2D could not be loaded."); continue; }
                    var estimate = RgbaReferenceBytes(texture.width, texture.height, importer.mipmapEnabled);
                    var issues = new List<string>();
                    if (Math.Max(texture.width, texture.height) > dimensionBudget) issues.Add("Imported dimensions exceed " + dimensionBudget);
                    if (estimate > (long)referenceMiBBudget * 1024 * 1024) issues.Add("RGBA8 reference exceeds " + referenceMiBBudget + " MiB");
                    if (importer.isReadable) issues.Add("Read/Write enabled: review whether a CPU copy is required");
                    rows.Add(new DiagnosticRow { path = paths[i], bytes = estimate, severity = issues.Count == 0 ? "Info" : "Review", group = texture.format.ToString(),
                        detail = texture.width + "x" + texture.height + "; mipmaps=" + importer.mipmapEnabled + "; streaming=" + importer.streamingMipmaps + "; " + (issues.Count == 0 ? "Within selected budgets" : string.Join("; ", issues)) });
                }
                catch (Exception ex) { errors.Add(paths[i] + ": " + ex.Message); }
            }
            return DiagnosticReport.Create("Texture Budget Auditor", folder + " | max dimension=" + dimensionBudget + " | RGBA8 reference MiB=" + referenceMiBBudget, rows.OrderByDescending(r => r.bytes).ThenBy(r => r.path, StringComparer.Ordinal).ToList(), errors);
        }
    }
}
