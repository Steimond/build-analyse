using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using VectraCode.BuildSizeAnalyzer;
using VectraCode.ReleasePreflight;
using UnityEngine;

namespace VectraCode.Tests
{
    public class ArchiveSizeTests
    {
        [TestCase(".apk")]
        [TestCase(".aab")]
        [TestCase(".ipa")]
        public void ReadsEntriesWithoutExtractingAndKeepsDuplicateNames(string extension)
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);
            try
            {
                using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
                {
                    zip.CreateEntry("folder/");
                    using (var writer = new StreamWriter(zip.CreateEntry("folder/data").Open())) writer.Write("payload");
                    using (var writer = new StreamWriter(zip.CreateEntry("folder/data").Open())) writer.Write("more payload");
                }
                var report = ArchiveSizeReport.Analyze(path);
                Assert.AreEqual(2, report.entries.Length);
                Assert.AreEqual(19, report.UncompressedBytes);
                Assert.Greater(report.archiveBytes, report.CompressedBytes);
                Assert.GreaterOrEqual(report.entries[0].compressedBytes, report.entries[1].compressedBytes);
            }
            finally { File.Delete(path); }
        }

        [Test]
        public void RejectsUnsupportedAndInvalidArchives()
        {
            Assert.Throws<NotSupportedException>(() => ArchiveSizeReport.Analyze("example.zip"));
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".apk");
            try
            {
                File.WriteAllText(path, "not a ZIP archive");
                Assert.Throws<InvalidDataException>(() => ArchiveSizeReport.Analyze(path));
            }
            finally { File.Delete(path); }
        }

        [Test]
        public void CsvEscapesQuotesAndNeutralizesFormulaPaths()
        {
            var report = new ArchiveSizeReport { entries = new[] { new ArchiveSizeEntry { path = "=SUM(1,2)\"", compressedBytes = 3, uncompressedBytes = 4 } } };
            StringAssert.Contains("\"'=SUM(1,2)\"\"\",3,4", report.ToCsv());
        }

        [Test]
        public void PreflightErrorsControlCiFailureAndSurviveJsonExport()
        {
            var report = new PreflightReport { findings = new[] { new PreflightFinding { id = "test", severity = "Warning" } } };
            Assert.IsFalse(report.HasErrors);
            report.findings[0].severity = "Error";
            Assert.IsTrue(report.HasErrors);
            Assert.IsTrue(JsonUtility.FromJson<PreflightReport>(JsonUtility.ToJson(report)).HasErrors);
        }

        [Test]
        public void PreflightScanReportsActiveTargetAndActionableFindings()
        {
            var report = PreflightChecks.Scan();
            Assert.IsNotEmpty(report.target);
            Assert.IsNotEmpty(report.generatedUtc);
            Assert.IsTrue(report.findings.All(f => !string.IsNullOrWhiteSpace(f.id) && !string.IsNullOrWhiteSpace(f.action)));
        }
    }
}
