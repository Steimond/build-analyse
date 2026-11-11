using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BuildDiffTool.Editor
{
    internal sealed class BuildDiffWindow : EditorWindow
    {
        private enum SortColumn
        {
            Status,
            Path,
            BaselineSize,
            CurrentSize,
            SizeDelta
        }

        private const float StatusWidth = 86f;
        private const float SizeWidth = 92f;
        private const float ActionWidth = 58f;
        private const float MinimumPathWidth = 240f;
        private const float TableSidePadding = 44f;

        private string baselinePath;
        private string currentPath;
        private string errorMessage;
        private string searchTerm;
        private bool showOnlyChanged;
        private SortColumn sortColumn = SortColumn.SizeDelta;
        private bool sortAscending;
        private Vector2 scrollPosition;
        private BuildDiffResult result;

        [MenuItem("Tools/Build Diff Tool")]
        private static void Open()
        {
            GetWindow<BuildDiffWindow>("Build Diff Tool");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Build Diff Tool", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                $"Version {BuildDiffToolInfo.Version} · Compare APK, AAB, or IPA archives by the files they contain.",
                EditorStyles.miniLabel);
            EditorGUILayout.Space(8f);

            DrawBuildPicker("Baseline build", ref baselinePath);
            DrawBuildPicker("New build", ref currentPath);

            using (new EditorGUI.DisabledScope(!CanCompare()))
            {
                if (GUILayout.Button("Analyze and compare", GUILayout.Height(28f)))
                {
                    AnalyzeBuilds();
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
            }

            if (result == null)
            {
                DrawEmptyState();
                return;
            }

            DrawSummary();
            DrawFilters();
            DrawTable();
        }

        private void DrawBuildPicker(string label, ref string path)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(path) ? "No archive selected" : path, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button("Choose…", GUILayout.Width(75f)))
            {
                var selected = EditorUtility.OpenFilePanelWithFilters(
                    "Choose " + label,
                    GetInitialDirectory(path),
                    new[] { "Mobile build archive", "apk,aab,ipa" });

                if (!string.IsNullOrEmpty(selected))
                {
                    path = selected;
                    result = null;
                    errorMessage = null;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEmptyState()
        {
            EditorGUILayout.Space(20f);
            EditorGUILayout.HelpBox(
                "Choose a baseline and a newer APK, AAB, or IPA. Build Diff Tool reads each archive, hashes its files, and shows the size impact of every change.",
                MessageType.Info);
        }

        private void DrawSummary()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Archive summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Baseline", FormatBytes(result.Baseline.ArchiveSize));
            EditorGUILayout.LabelField("New build", FormatBytes(result.Current.ArchiveSize));
            DrawColoredLabel("Archive change", FormatDelta(result.ArchiveSizeDelta), GetDeltaColor(result.ArchiveSizeDelta, false));
            EditorGUILayout.LabelField("Files compared", result.Entries.Count.ToString());
            EditorGUILayout.EndVertical();
        }

        private void DrawFilters()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            searchTerm = GUILayout.TextField(
                searchTerm ?? string.Empty,
                GUI.skin.FindStyle("ToolbarSearchTextField"),
                GUILayout.MinWidth(260f),
                GUILayout.ExpandWidth(true));
            if (GUILayout.Button(string.Empty, GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                searchTerm = string.Empty;
                GUI.FocusControl(null);
            }

            GUILayout.FlexibleSpace();
            showOnlyChanged = GUILayout.Toggle(
                showOnlyChanged,
                "Show only changed",
                EditorStyles.toolbarButton,
                GUILayout.MinWidth(125f));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTable()
        {
            var rows = SortRows(result.Entries.Where(MatchesFilter)).ToList();
            var tableWidth = GetTableWidth();
            var pathWidth = tableWidth - StatusWidth - (SizeWidth * 3f) - ActionWidth;

            EditorGUILayout.Space(4f);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true);
            EditorGUILayout.BeginVertical(GUILayout.Width(tableWidth));
            DrawTableHeader(pathWidth);
            for (var index = 0; index < rows.Count; index++)
            {
                DrawRow(rows[index], index, pathWidth);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawTableHeader(float pathWidth)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawSortHeader("Status", SortColumn.Status, GUILayout.Width(StatusWidth));
            DrawSortHeader("Path", SortColumn.Path, GUILayout.Width(pathWidth));
            DrawSortHeader("Before", SortColumn.BaselineSize, GUILayout.Width(SizeWidth));
            DrawSortHeader("After", SortColumn.CurrentSize, GUILayout.Width(SizeWidth));
            DrawSortHeader("Change", SortColumn.SizeDelta, GUILayout.Width(SizeWidth));
            GUILayout.Space(ActionWidth);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRow(BuildDiffEntry row, int index, float pathWidth)
        {
            var rowRect = EditorGUILayout.BeginHorizontal();
            if ((index & 1) == 1)
            {
                EditorGUI.DrawRect(rowRect, GetAlternateRowColor());
            }

            GUILayout.Label(GetStatusLabel(row.Kind), EditorStyles.miniLabel, GUILayout.Width(StatusWidth));
            GUILayout.Label(row.Path, EditorStyles.label, GUILayout.Width(pathWidth));
            GUILayout.Label(FormatBytes(row.BaselineSize), EditorStyles.label, GUILayout.Width(SizeWidth));
            GUILayout.Label(FormatBytes(row.CurrentSize), EditorStyles.label, GUILayout.Width(SizeWidth));
            DrawColoredLabel(string.Empty, FormatDelta(row.SizeDelta), GetDeltaColor(row.SizeDelta, row.Kind == BuildDiffKind.Unchanged), GUILayout.Width(SizeWidth));

            var asset = AssetDatabase.LoadMainAssetAtPath(row.Path);
            if (asset != null)
            {
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(ActionWidth)))
                {
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                }
            }
            else
            {
                GUILayout.Space(ActionWidth);
            }

            EditorGUILayout.EndHorizontal();
        }

        private float GetTableWidth()
        {
            var minimumTableWidth = StatusWidth + MinimumPathWidth + (SizeWidth * 3f) + ActionWidth;
            return Mathf.Max(minimumTableWidth, position.width - TableSidePadding);
        }

        private static Color GetAlternateRowColor()
        {
            return EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.045f)
                : new Color(1f, 1f, 1f, 0.5f);
        }

        private bool MatchesFilter(BuildDiffEntry entry)
        {
            if (showOnlyChanged && entry.Kind == BuildDiffKind.Unchanged)
            {
                return false;
            }

            return string.IsNullOrEmpty(searchTerm)
                   || entry.Path.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void DrawSortHeader(string label, SortColumn column, params GUILayoutOption[] options)
        {
            var suffix = sortColumn == column ? (sortAscending ? " ▲" : " ▼") : string.Empty;
            if (GUILayout.Button(label + suffix, EditorStyles.toolbarButton, options))
            {
                if (sortColumn == column)
                {
                    sortAscending = !sortAscending;
                    return;
                }

                sortColumn = column;
                sortAscending = column == SortColumn.Status || column == SortColumn.Path;
            }
        }

        private System.Collections.Generic.IEnumerable<BuildDiffEntry> SortRows(
            System.Collections.Generic.IEnumerable<BuildDiffEntry> entries)
        {
            switch (sortColumn)
            {
                case SortColumn.Status:
                    return sortAscending
                        ? entries.OrderBy(entry => entry.Kind).ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase)
                        : entries.OrderByDescending(entry => entry.Kind).ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase);
                case SortColumn.Path:
                    return sortAscending
                        ? entries.OrderBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase)
                        : entries.OrderByDescending(entry => entry.Path, StringComparer.OrdinalIgnoreCase);
                case SortColumn.BaselineSize:
                    return sortAscending
                        ? entries.OrderBy(entry => entry.BaselineSize).ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase)
                        : entries.OrderByDescending(entry => entry.BaselineSize).ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase);
                case SortColumn.CurrentSize:
                    return sortAscending
                        ? entries.OrderBy(entry => entry.CurrentSize).ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase)
                        : entries.OrderByDescending(entry => entry.CurrentSize).ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase);
                default:
                    return sortAscending
                        ? entries.OrderBy(entry => entry.SizeDelta).ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase)
                        : entries.OrderByDescending(entry => entry.SizeDelta).ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase);
            }
        }

        private bool CanCompare()
        {
            return !string.IsNullOrEmpty(baselinePath)
                   && !string.IsNullOrEmpty(currentPath)
                   && !string.Equals(baselinePath, currentPath, StringComparison.OrdinalIgnoreCase);
        }

        private void AnalyzeBuilds()
        {
            try
            {
                errorMessage = null;
                result = BuildDiffService.Compare(
                    BuildArchiveAnalyzer.Analyze(baselinePath),
                    BuildArchiveAnalyzer.Analyze(currentPath));
            }
            catch (Exception exception)
            {
                result = null;
                errorMessage = exception.Message;

                if (!(exception is ArgumentException)
                    && !(exception is FileNotFoundException)
                    && !(exception is InvalidDataException)
                    && !(exception is NotSupportedException))
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static string GetInitialDirectory(string path)
        {
            return string.IsNullOrEmpty(path) ? Application.dataPath : Path.GetDirectoryName(path);
        }

        private static string GetStatusLabel(BuildDiffKind kind)
        {
            switch (kind)
            {
                case BuildDiffKind.Added:
                    return "Added";
                case BuildDiffKind.Removed:
                    return "Removed";
                case BuildDiffKind.Modified:
                    return "Modified";
                default:
                    return "Unchanged";
            }
        }

        private static void DrawColoredLabel(string label, string value, Color color, params GUILayoutOption[] options)
        {
            var previousColor = GUI.contentColor;
            GUI.contentColor = color;
            if (string.IsNullOrEmpty(label))
            {
                GUILayout.Label(value, EditorStyles.label, options);
            }
            else
            {
                EditorGUILayout.LabelField(label, value, options);
            }

            GUI.contentColor = previousColor;
        }

        private static Color GetDeltaColor(long delta, bool unchanged)
        {
            if (unchanged)
            {
                return Color.white;
            }

            if (delta > 0)
            {
                return new Color(1f, 0.35f, 0.35f);
            }

            if (delta < 0)
            {
                return new Color(0.35f, 0.85f, 0.4f);
            }

            return new Color(1f, 0.75f, 0.25f);
        }

        private static string FormatDelta(long bytes)
        {
            return bytes == 0 ? "0 B" : (bytes > 0 ? "+" : "-") + FormatBytes(Math.Abs(bytes));
        }

        private static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            var size = (double)bytes;
            var suffixIndex = 0;
            while (size >= 1024d && suffixIndex < suffixes.Length - 1)
            {
                size /= 1024d;
                suffixIndex++;
            }

            return suffixIndex == 0 ? $"{size:0} {suffixes[suffixIndex]}" : $"{size:0.##} {suffixes[suffixIndex]}";
        }
    }
}
