using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VectraCode.ReleasePreflight
{
    public sealed class PreflightWindow : EditorWindow
    {
        private PreflightReport report;
        private Vector2 scroll;
        private string error;
        [MenuItem("Tools/Release Preflight/Open")]
        public static void Open() => GetWindow<PreflightWindow>("Release Preflight");
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Release Preflight", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Read-only checks of the active target and build scene list. Reports are snapshots: scan again after changing settings. This does not validate store acceptance, device behavior, signing credentials, or compilation success.", MessageType.Info);
            if (GUILayout.Button("Scan release settings"))
            {
                report = null;
                error = null;
                try { report = PreflightChecks.Scan(); }
                catch (Exception ex) { error = ex.Message; }
            }
            if (error != null) EditorGUILayout.HelpBox(error, MessageType.Error);
            if (report == null) return;
            EditorGUILayout.LabelField(report.target + " • " + report.generatedUtc);
            if (GUILayout.Button("Export JSON…"))
            {
                var path = EditorUtility.SaveFilePanel("Save preflight report", "", "release-preflight", "json");
                if (!string.IsNullOrEmpty(path))
                    try { File.WriteAllText(path, JsonUtility.ToJson(report, true)); }
                    catch (Exception ex) { error = ex.Message; }
            }
            if (report.findings.Length == 0) EditorGUILayout.HelpBox("No findings in the checked settings.", MessageType.Info);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var finding in report.findings)
                EditorGUILayout.HelpBox(finding.id + "\n" + finding.message + "\n" + finding.action,
                    finding.severity == "Error" ? MessageType.Error : MessageType.Warning);
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Open Player Settings")) SettingsService.OpenProjectSettings("Project/Player");
            if (GUILayout.Button("Open Build Profiles")) EditorApplication.ExecuteMenuItem("File/Build Profiles");
        }
    }
}
