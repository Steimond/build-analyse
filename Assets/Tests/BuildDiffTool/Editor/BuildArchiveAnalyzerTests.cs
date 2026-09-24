using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;

namespace BuildDiffTool.Editor.Tests
{
    internal sealed class BuildArchiveAnalyzerTests
    {
        private string temporaryDirectory;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectory = Path.Combine(Path.GetTempPath(), "BuildDiffToolTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temporaryDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(temporaryDirectory, true);
            }
        }

        [Test]
        public void Compare_ReportsAddedRemovedModifiedAndUnchangedEntries()
        {
            var baselinePath = CreateArchive("baseline.apk", new[]
            {
                ("same.txt", "same"),
                ("changed.txt", "old content"),
                ("removed.txt", "remove me")
            });
            var currentPath = CreateArchive("current.apk", new[]
            {
                ("same.txt", "same"),
                ("changed.txt", "new content with more bytes"),
                ("added.txt", "add me")
            });

            var result = BuildDiffService.Compare(
                BuildArchiveAnalyzer.Analyze(baselinePath),
                BuildArchiveAnalyzer.Analyze(currentPath));

            Assert.That(result.Entries.First(entry => entry.Path == "same.txt").Kind, Is.EqualTo(BuildDiffKind.Unchanged));
            Assert.That(result.Entries.First(entry => entry.Path == "changed.txt").Kind, Is.EqualTo(BuildDiffKind.Modified));
            Assert.That(result.Entries.First(entry => entry.Path == "added.txt").Kind, Is.EqualTo(BuildDiffKind.Added));
            Assert.That(result.Entries.First(entry => entry.Path == "removed.txt").Kind, Is.EqualTo(BuildDiffKind.Removed));
        }

        [Test]
        public void Analyze_RejectsAnUnsupportedFileExtension()
        {
            var unsupportedPath = Path.Combine(temporaryDirectory, "report.txt");
            File.WriteAllText(unsupportedPath, "not an archive");

            Assert.Throws<NotSupportedException>(() => BuildArchiveAnalyzer.Analyze(unsupportedPath));
        }

        [Test]
        public void Analyze_ExplainsWhenAnArchiveIsInvalid()
        {
            var invalidArchivePath = Path.Combine(temporaryDirectory, "incomplete.apk");
            File.WriteAllText(invalidArchivePath, "not a zip archive");

            var exception = Assert.Throws<InvalidDataException>(() => BuildArchiveAnalyzer.Analyze(invalidArchivePath));

            Assert.That(exception.Message, Does.Contain("not a valid APK archive"));
        }

        private string CreateArchive(string fileName, (string path, string content)[] files)
        {
            var archivePath = Path.Combine(temporaryDirectory, fileName);
            using (var stream = File.Create(archivePath))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, false))
            {
                foreach (var file in files)
                {
                    var entry = archive.CreateEntry(file.path);
                    using (var writer = new StreamWriter(entry.Open()))
                    {
                        writer.Write(file.content);
                    }
                }
            }

            return archivePath;
        }
    }
}
