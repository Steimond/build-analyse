using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VectraCode.BuildSizeAnalyzer
{
    public sealed class ArchiveSizeWindow : EditorWindow
    {
        private ArchiveSizeReport report;
        private string filter = "";
        private string error;
        private Vector2 scroll;
        private bool expanded;

        [MenuItem("Tools/Build Size Analyzer/Open")]
        public static void Open() => GetWindow<ArchiveSizeWindow>("Build Size Analyzer");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Build Size Analyzer", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Inspect a completed APK, AAB, or IPA without the source project. Sizes describe ZIP entries, not download or installed size. Archives are read locally and never extracted.", MessageType.Info);
            if (GUILayout.Button("Choose archive and analyze…"))
            {
                var path = EditorUtility.OpenFilePanelWithFilters("Choose build archive", "", new[] { "Mobile archives", "apk,aab,ipa" });
                if (!string.IsNullOrEmpty(path))
                {
                    report = null;
                    error = null;
                    try { report = ArchiveSizeReport.Analyze(path); }
                    catch (Exception ex) { error = ex.Message; }
                }
            }
            if (!string.IsNullOrEmpty(error)) EditorGUILayout.HelpBox(error, MessageType.Error);
            if (report == null) return;
            EditorGUILayout.LabelField(Path.GetFileName(report.archivePath), EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Archive: {EditorUtility.FormatBytes(report.archiveBytes)} • Entries: {report.entries.Length}");
            EditorGUILayout.LabelField($"Compressed content: {EditorUtility.FormatBytes(report.CompressedBytes)} • Uncompressed: {EditorUtility.FormatBytes(report.UncompressedBytes)}");
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Export CSV…")) Export("csv");
                if (GUILayout.Button("Export JSON…")) Export("json");
            }
            expanded = EditorGUILayout.Foldout(expanded, "Compressed size by extension");
            if (expanded)
                foreach (var group in report.entries.GroupBy(e => Path.GetExtension(e.path).ToLowerInvariant()).OrderByDescending(g => g.Sum(e => e.compressedBytes)).Take(20))
                    EditorGUILayout.LabelField(string.IsNullOrEmpty(group.Key) ? "(no extension)" : group.Key, EditorUtility.FormatBytes(group.Sum(e => e.compressedBytes)));
            filter = EditorGUILayout.TextField("Filter path", filter);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            var shown = 0;
            foreach (var entry in report.entries)
            {
                if (entry.path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (++shown > 500) { EditorGUILayout.LabelField("Showing the largest 500 matching entries. Refine the filter or export all entries."); break; }
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.SelectableLabel(entry.path, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    GUILayout.Label(EditorUtility.FormatBytes(entry.compressedBytes), GUILayout.Width(90));
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void Export(string extension)
        {
            var path = EditorUtility.SaveFilePanel("Export archive report", "", "archive-size-report", extension);
            if (string.IsNullOrEmpty(path)) return;
            try { File.WriteAllText(path, extension == "csv" ? report.ToCsv() : JsonUtility.ToJson(report, true)); }
            catch (Exception ex) { error = ex.Message; }
        }
    }
}
