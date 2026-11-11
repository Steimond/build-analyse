using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace VectraCode.DuplicateAssetFinder
{
    public static class DuplicateScanner
    {
        public static DiagnosticReport Scan(string folder, Func<string, float, bool> cancel = null)
        {
            ScanUtility.ValidateFolder(folder);
            var paths = AssetDatabase.GetAllAssetPaths().Where(p => ScanUtility.IsInFolder(p, folder) && !AssetDatabase.IsValidFolder(p) && !p.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)).OrderBy(p => p, StringComparer.Ordinal).ToArray();
            var candidates = new Dictionary<long, List<string>>();
            var errors = new List<string>();
            var project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            foreach (var path in paths)
            {
                ScanUtility.CheckCancellation(cancel, path, 0, paths.Length);
                try
                {
                    var full = SafePath(project, path);
                    var length = new FileInfo(full).Length;
                    if (length == 0) continue;
                    if (!candidates.TryGetValue(length, out var list)) candidates.Add(length, list = new List<string>());
                    list.Add(path);
                }
                catch (Exception ex) { errors.Add(path + ": " + ex.Message); }
            }
            var groups = new Dictionary<string, List<DiagnosticRow>>(StringComparer.Ordinal);
            var eligible = candidates.Where(pair => pair.Value.Count > 1).SelectMany(pair => pair.Value.Select(path => new { path, length = pair.Key })).ToArray();
            for (var i = 0; i < eligible.Length; i++)
            {
                var entry = eligible[i];
                ScanUtility.CheckCancellation(cancel, entry.path, i, eligible.Length);
                try
                {
                    var hash = HashFile(SafePath(project, entry.path), () => cancel != null && cancel(entry.path, eligible.Length == 0 ? 0 : (float)i / eligible.Length));
                    var key = entry.length + ":" + hash;
                    if (!groups.TryGetValue(key, out var list)) groups.Add(key, list = new List<DiagnosticRow>());
                    list.Add(new DiagnosticRow { path = entry.path, bytes = entry.length, group = hash, severity = "Review", detail = "Identical source-file SHA-256; importer settings and GUIDs may differ. Review references before any manual changes." });
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex) { errors.Add(entry.path + ": " + ex.Message); }
            }
            var rows = groups.Values.Where(g => g.Count > 1).OrderByDescending(g => g[0].bytes * (g.Count - 1)).ThenBy(g => g[0].path, StringComparer.Ordinal).SelectMany(g => g).ToList();
            return DiagnosticReport.Create("Duplicate Asset Finder", folder, rows, errors);
        }

        public static string HashFile(string path, Func<bool> cancel = null)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sha = SHA256.Create())
            {
                var buffer = new byte[1024 * 1024];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (cancel != null && cancel()) throw new OperationCanceledException();
                    sha.TransformBlock(buffer, 0, read, buffer, 0);
                }
                sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                return BitConverter.ToString(sha.Hash).Replace("-", "");
            }
        }

        public static string SafePath(string projectRoot, string assetPath)
        {
            var assets = Path.GetFullPath(Path.Combine(projectRoot, "Assets"));
            var full = Path.GetFullPath(Path.Combine(projectRoot, assetPath));
            if (!full.StartsWith(assets + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new IOException("Path is outside Assets.");
            for (var current = full; !string.Equals(current, assets, StringComparison.OrdinalIgnoreCase); current = Path.GetDirectoryName(current))
                if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new IOException("Linked files and folders are excluded from hashing.");
            if ((File.GetAttributes(assets) & FileAttributes.ReparsePoint) != 0) throw new IOException("Linked Assets roots are excluded from hashing.");
            return full;
        }
    }
}
