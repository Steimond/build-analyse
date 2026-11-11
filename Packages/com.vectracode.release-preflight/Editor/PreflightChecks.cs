using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace VectraCode.ReleasePreflight
{
    [Serializable]
    public sealed class PreflightFinding
    {
        public string id;
        public string severity;
        public string message;
        public string action;
    }

    [Serializable]
    public sealed class PreflightReport
    {
        public string unityVersion;
        public string target;
        public string generatedUtc;
        public PreflightFinding[] findings;
        public bool HasErrors => findings.Any(f => f.severity == "Error");
    }

    public static class PreflightChecks
    {
        public static PreflightReport Scan()
        {
            var findings = new List<PreflightFinding>();
            Action<string, string, string, string> add = (id, severity, message, action) =>
                findings.Add(new PreflightFinding { id = id, severity = severity, message = message, action = action });
            var target = EditorUserBuildSettings.activeBuildTarget;
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(target));
            if (EditorApplication.isCompiling) add("editor.compiling", "Error", "Scripts are still compiling; the scan is incomplete.", "Wait for compilation, resolve Console errors, and scan again.");
            if (EditorUserBuildSettings.development) add("build.development", "Warning", "Development Build is enabled.", "Review Development Build in Build Profiles before a store release.");
            if (EditorUserBuildSettings.allowDebugging) add("build.debugging", "Warning", "Script debugging is enabled.", "Disable Script Debugging for production builds.");
            if (EditorUserBuildSettings.connectProfiler) add("build.profiler", "Warning", "Autoconnect Profiler is enabled.", "Disable Autoconnect Profiler for production builds.");
            if (string.IsNullOrWhiteSpace(PlayerSettings.companyName) || PlayerSettings.companyName == "DefaultCompany")
                add("player.company", "Warning", "Company name is empty or uses the default.", "Set Company Name in Player Settings.");
            if (string.IsNullOrWhiteSpace(PlayerSettings.productName)) add("player.product", "Error", "Product name is empty.", "Set Product Name in Player Settings.");
            if (string.IsNullOrWhiteSpace(PlayerSettings.bundleVersion)) add("player.version", "Error", "Application version is empty.", "Set Version in Player Settings.");
            var identifier = PlayerSettings.GetApplicationIdentifier(namedTarget);
            if (string.IsNullOrWhiteSpace(identifier) || identifier.StartsWith("com.DefaultCompany.", StringComparison.OrdinalIgnoreCase))
                add("player.identifier", "Warning", "Application identifier is empty or uses DefaultCompany.", "Review the application identifier for the active target in Player Settings.");
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).ToArray();
            if (scenes.Length == 0) add("scenes.empty", "Error", "No enabled scenes in the active build scene list.", "Add and enable the intended scenes in Build Profiles.");
            foreach (var scene in scenes)
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path) == null)
                    add("scenes.missing", "Error", "Enabled scene cannot be loaded: " + scene.path, "Restore the scene or remove its build-list entry.");
            if (target == BuildTarget.Android)
            {
                if (PlayerSettings.Android.bundleVersionCode <= 0) add("android.versionCode", "Error", "Android version code must be positive.", "Set Bundle Version Code in Android Player Settings.");
                if (!PlayerSettings.Android.useCustomKeystore) add("android.signing", "Warning", "Custom Android keystore is disabled.", "Configure your release signing key before uploading. Keystore passwords are never read by this tool.");
            }
            return new PreflightReport { unityVersion = Application.unityVersion, target = target.ToString(),
                generatedUtc = DateTime.UtcNow.ToString("O"), findings = findings.ToArray() };
        }

        // unity -batchmode -projectPath <project> -executeMethod VectraCode.ReleasePreflight.PreflightChecks.RunCi -quit
        public static void RunCi()
        {
            var report = Scan();
            var destination = Path.GetFullPath(Path.Combine(Application.dataPath, "../Artifacts/release-preflight.json"));
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.WriteAllText(destination, JsonUtility.ToJson(report, true));
            Debug.Log("Release Preflight report: " + destination);
            if (report.HasErrors) throw new BuildFailedException("Release Preflight found blocking configuration errors. See " + destination);
        }
    }
}
