using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace VectraCode.AssetDependencyExplorer
{
    [Serializable]
    public sealed class DiagnosticRow
    {
        public string path;
        public string group;
        public string severity;
        public long bytes;
        public string detail;
    }

    [Serializable]
    public sealed class DiagnosticReport
    {
        public string tool;
        public string version = "1.0.0";
        public string generatedUtc;
        public string unityVersion;
        public string scope;
        public bool scanCompleted;
        public string metric = "Rows have no size metric.";
        public DiagnosticRow[] rows;
        public string[] errors;

        public static DiagnosticReport Create(string tool, string scope, List<DiagnosticRow> rows, List<string> errors)
        {
            return new DiagnosticReport { tool = tool, scope = scope, generatedUtc = DateTime.UtcNow.ToString("O"),
                unityVersion = UnityEngine.Application.unityVersion, scanCompleted = errors.Count == 0, rows = rows.ToArray(), errors = errors.ToArray() };
        }

        public string ToCsv()
        {
            var csv = new StringBuilder("Tool,Scope,Scan completed,Path,Group,Severity,Metric value,Detail\r\n");
            foreach (var row in rows)
                csv.Append(Cell(tool)).Append(',').Append(Cell(scope)).Append(',').Append(scanCompleted ? "true" : "false").Append(',')
                    .Append(Cell(row.path)).Append(',').Append(Cell(row.group)).Append(',').Append(Cell(row.severity)).Append(',')
                    .Append(row.bytes.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(',').Append(Cell(row.detail)).Append("\r\n");
            foreach (var error in errors)
                csv.Append(Cell(tool)).Append(',').Append(Cell(scope)).Append(",false,,,Error,0,").Append(Cell(error)).Append("\r\n");
            return csv.ToString();
        }

        private static string Cell(string value)
        {
            value = value ?? "";
            var trimmed = value.TrimStart();
            if (trimmed.Length > 0 && "=+-@".IndexOf(trimmed[0]) >= 0) value = "'" + value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }

    public static class ScanUtility
    {
        public static bool IsInFolder(string path, string folder) => path != null && folder != null && path.StartsWith(folder.TrimEnd('/') + "/", StringComparison.Ordinal);
        public static void ValidateFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder) || !(folder == "Assets" || folder.StartsWith("Assets/", StringComparison.Ordinal)) || !AssetDatabase.IsValidFolder(folder))
                throw new ArgumentException("Choose an existing folder under Assets.");
        }
        public static void CheckCancellation(Func<string, float, bool> cancel, string path, int index, int count)
        {
            if (cancel != null && cancel(path, count == 0 ? 0 : (float)index / count)) throw new OperationCanceledException();
        }
    }
}
