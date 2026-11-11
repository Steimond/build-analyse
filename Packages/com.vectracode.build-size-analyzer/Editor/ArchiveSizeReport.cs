using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace VectraCode.BuildSizeAnalyzer
{
    [Serializable]
    public sealed class ArchiveSizeEntry
    {
        public string path;
        public long compressedBytes;
        public long uncompressedBytes;
    }

    [Serializable]
    public sealed class ArchiveSizeReport
    {
        public string archivePath;
        public long archiveBytes;
        public ArchiveSizeEntry[] entries;
        public long CompressedBytes => entries.Sum(e => e.compressedBytes);
        public long UncompressedBytes => entries.Sum(e => e.uncompressedBytes);

        public static ArchiveSizeReport Analyze(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Choose an APK, AAB, or IPA archive.");
            if (!new[] { ".apk", ".aab", ".ipa" }.Contains(Path.GetExtension(path).ToLowerInvariant()))
                throw new NotSupportedException("Supported formats: APK, AAB, IPA.");
            var rows = new List<ArchiveSizeEntry>();
            using (var stream = File.OpenRead(path))
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                foreach (var entry in zip.Entries)
                    if (!string.IsNullOrEmpty(entry.Name))
                        rows.Add(new ArchiveSizeEntry { path = entry.FullName.Replace('\\', '/'), compressedBytes = entry.CompressedLength, uncompressedBytes = entry.Length });
            return new ArchiveSizeReport { archivePath = Path.GetFullPath(path), archiveBytes = new FileInfo(path).Length,
                entries = rows.OrderByDescending(e => e.compressedBytes).ThenBy(e => e.path, StringComparer.Ordinal).ToArray() };
        }

        public string ToCsv()
        {
            var csv = new StringBuilder("Path,Compressed bytes,Uncompressed bytes\r\n");
            foreach (var entry in entries)
            {
                // Prevent spreadsheet applications from interpreting archive paths as formulas.
                var path = entry.path;
                if (path.Length > 0 && "=+-@\t\r\n".IndexOf(path[0]) >= 0) path = "'" + path;
                csv.Append('"').Append(path.Replace("\"", "\"\"")).Append("\",")
                    .Append(entry.compressedBytes).Append(',').Append(entry.uncompressedBytes).Append("\r\n");
            }
            return csv.ToString();
        }
    }
}
