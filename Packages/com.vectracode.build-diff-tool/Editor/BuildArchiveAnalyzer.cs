using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace BuildDiffTool.Editor
{
    internal static class BuildArchiveAnalyzer
    {
        private static readonly HashSet<string> SupportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".apk",
            ".aab",
            ".ipa"
        };

        public static BuildArchiveAnalysis Analyze(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Choose a build archive first.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The selected build archive was not found.", filePath);
            }

            var extension = Path.GetExtension(filePath);
            if (!SupportedExtensions.Contains(extension))
            {
                throw new NotSupportedException("Build Diff Tool supports APK, AAB, and IPA archives.");
            }

            try
            {
                var entries = new Dictionary<string, BuildArchiveEntry>(StringComparer.Ordinal);
                using (var stream = File.OpenRead(filePath))
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, false))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            continue;
                        }

                        var path = NormalizePath(entry.FullName);
                        var contentHash = CalculateHash(entry);

                        if (entries.TryGetValue(path, out var previous))
                        {
                            entries[path] = new BuildArchiveEntry(
                                path,
                                previous.CompressedSize + entry.CompressedLength,
                                previous.UncompressedSize + entry.Length,
                                previous.ContentHash + ":" + contentHash);
                            continue;
                        }

                        entries.Add(path, new BuildArchiveEntry(path, entry.CompressedLength, entry.Length, contentHash));
                    }
                }

                return new BuildArchiveAnalysis(filePath, new FileInfo(filePath).Length, entries);
            }
            catch (InvalidDataException exception)
            {
                throw new InvalidDataException(
                    $"'{Path.GetFileName(filePath)}' is not a valid {extension.TrimStart('.').ToUpperInvariant()} archive. Choose the completed build output, not a partial, renamed, or extracted file.",
                    exception);
            }
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/').TrimStart('/');
        }

        private static string CalculateHash(ZipArchiveEntry entry)
        {
            using (var hash = SHA256.Create())
            using (var entryStream = entry.Open())
            {
                return BitConverter.ToString(hash.ComputeHash(entryStream)).Replace("-", string.Empty);
            }
        }
    }
}
