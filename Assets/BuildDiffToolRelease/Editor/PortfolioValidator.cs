using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace VectraCode.Release
{
    // Development-only bridge to Publishing Tools 0.3.2's internal validator.
    // This folder is excluded from every exported product.
    public static class PortfolioValidator
    {
        public static void ValidateAll()
        {
            var settingsType = RequiredType("AssetStore.Validator.Data.UnityPackageValidationSettings");
            var providerType = RequiredType("AssetStore.Validator.UnityPackageTestProvider");
            var validatorType = RequiredType("AssetStore.Validator.UnityPackageValidator");
            Directory.CreateDirectory("Artifacts");
            foreach (var product in new[] { "Build Diff Tool", "Build Size Analyzer", "Release Preflight", "Asset Dependency Explorer", "Duplicate Asset Finder", "Texture Budget Auditor", "Prefab Missing Script Scanner" })
            {
                var settings = Activator.CreateInstance(settingsType);
                settingsType.GetField("ValidationPaths").SetValue(settings, new List<string> { "Assets/" + product });
                settingsType.GetField("Category").SetValue(settings, "Tools/Utilities");
                var validator = Activator.CreateInstance(validatorType, new[] { Activator.CreateInstance(providerType) });
                var result = validatorType.GetMethod("Validate").Invoke(validator, new[] { settings });
                var destination = "Artifacts/" + product.Replace(' ', '-') + "-validation.json";
                File.WriteAllText(destination, JsonConvert.SerializeObject(result, Formatting.Indented));
                Debug.Log("Asset Store Validator report saved: " + destination);
            }
        }

        private static Type RequiredType(string name) => Type.GetType(name + ", Unity.PublishingTools.Editor", true);
    }
}
