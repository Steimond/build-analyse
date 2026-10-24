using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VectraCode.DuplicateAssetFinder
{
    public sealed class DiagnosticWindow : EditorWindow
    {
        private DefaultAsset folderAsset;
        private DiagnosticReport report;
        private string filter = "";
        private string status;
        private bool reviewOnly;
        private Vector2 scroll;
        

        [MenuItem("Tools/Duplicate Asset Finder/Open")]
        public static void Open() => GetWindow<DiagnosticWindow>("Duplicate Asset Finder");

        private bool Progress(string path, float fraction) => EditorUtility.DisplayCancelableProgressBar("Duplicate Asset Finder", path, fraction);

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Duplicate Asset Finder", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Find non-empty source files with identical SHA-256 content in a chosen Assets folder, without deleting or merging assets.", MessageType.Info);
            folderAsset = (DefaultAsset)EditorGUILayout.ObjectField("Assets folder (optional)", folderAsset, typeof(DefaultAsset), false);
            
            using (new EditorGUI.DisabledScope(EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode))
                if (GUILayout.Button("Scan (read-only)"))
                {
                    report = null;
                    status = null;
                    var folder = folderAsset == null ? "Assets" : AssetDatabase.GetAssetPath(folderAsset);
                    try { report = DuplicateScanner.Scan(folder, Progress); }
                    catch (OperationCanceledException) { status = "Scan cancelled. No partial report is retained. Run again to obtain a complete snapshot."; }
                    catch (Exception ex) { status = ex.Message; }
                    finally { EditorUtility.ClearProgressBar(); }
                }
            EditorGUILayout.HelpBox("Groups describe source-file bytes, not equivalent Unity imports or guaranteed build savings. GUIDs and .meta settings are excluded; different import settings may produce different assets. Empty files and linked paths are excluded. Hashing large projects can take time. Review references before manual cleanup.", MessageType.None);
            if (!string.IsNullOrEmpty(status)) EditorGUILayout.HelpBox(status, MessageType.Warning);
            if (report == null) return;
            if (!report.scanCompleted) EditorGUILayout.HelpBox("Incomplete scan: some assets could not be inspected. Review the errors; this is not a clean result.", MessageType.Warning);
            EditorGUILayout.LabelField(report.rows.Length + " rows | " + report.errors.Length + " errors | " + report.generatedUtc);
            EditorGUILayout.LabelField("Snapshot: " + report.scope, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(report.metric, EditorStyles.wordWrappedMiniLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Export CSV…")) Export("csv");
                if (GUILayout.Button("Export JSON…")) Export("json");
            }
            filter = EditorGUILayout.TextField("Filter path / details", filter);
            reviewOnly = EditorGUILayout.Toggle("Review findings only", reviewOnly);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var error in report.errors.Take(50)) EditorGUILayout.HelpBox(error, MessageType.Warning);
            if (report.errors.Length > 50) EditorGUILayout.LabelField("More errors are included in the export.");
            var shown = 0;
            foreach (var row in report.rows)
            {
                if (reviewOnly && row.severity == "Info") continue;
                if ((row.path + " " + row.detail + " " + row.group).IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (++shown > 500) { EditorGUILayout.LabelField("Showing the first 500 matches. Refine the filter or export all rows."); break; }
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.SelectableLabel(row.path, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        if (GUILayout.Button("Select", GUILayout.Width(55)))
                        {
                            var asset = AssetDatabase.LoadMainAssetAtPath(row.path);
                            if (asset != null) { Selection.activeObject = asset; EditorGUIUtility.PingObject(asset); }
                        }
                    }
                    EditorGUILayout.LabelField(row.severity + " | " + row.group + " | " + row.bytes, EditorStyles.wordWrappedMiniLabel);
                    EditorGUILayout.LabelField(row.detail, EditorStyles.wordWrappedLabel);
                }
            }
            if (shown == 0) EditorGUILayout.LabelField("No matching rows in this snapshot. Check scan scope and errors.");
            EditorGUILayout.EndScrollView();
        }

        private void Export(string extension)
        {
            var path = EditorUtility.SaveFilePanel("Export Duplicate Asset Finder report", "", "DuplicateAssetFinder-report", extension);
            if (string.IsNullOrEmpty(path)) return;
            try { File.WriteAllText(path, extension == "csv" ? report.ToCsv() : JsonUtility.ToJson(report, true)); }
            catch (Exception ex) { status = ex.Message; }
        }
    }
}
