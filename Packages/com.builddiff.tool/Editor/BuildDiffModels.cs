using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildDiffTool.Editor
{
    internal sealed class BuildArchiveEntry
    {
        public BuildArchiveEntry(string path, long compressedSize, long uncompressedSize, string contentHash)
        {
            Path = path;
            CompressedSize = compressedSize;
            UncompressedSize = uncompressedSize;
            ContentHash = contentHash;
        }

        public string Path { get; }
        public long CompressedSize { get; }
        public long UncompressedSize { get; }
        public string ContentHash { get; }
    }

    internal sealed class BuildArchiveAnalysis
    {
        public BuildArchiveAnalysis(string filePath, long archiveSize, IReadOnlyDictionary<string, BuildArchiveEntry> entries)
        {
            FilePath = filePath;
            ArchiveSize = archiveSize;
            Entries = entries;
        }

        public string FilePath { get; }
        public long ArchiveSize { get; }
        public IReadOnlyDictionary<string, BuildArchiveEntry> Entries { get; }
        public long ContentSize => Entries.Values.Sum(entry => entry.CompressedSize);
    }

    internal enum BuildDiffKind
    {
        Unchanged,
        Added,
        Removed,
        Modified
    }

    internal sealed class BuildDiffEntry
    {
        public BuildDiffEntry(string path, BuildArchiveEntry baseline, BuildArchiveEntry current)
        {
            Path = path;
            Baseline = baseline;
            Current = current;
        }

        public string Path { get; }
        public BuildArchiveEntry Baseline { get; }
        public BuildArchiveEntry Current { get; }
        public long BaselineSize => Baseline?.CompressedSize ?? 0;
        public long CurrentSize => Current?.CompressedSize ?? 0;
        public long SizeDelta => CurrentSize - BaselineSize;

        public BuildDiffKind Kind
        {
            get
            {
                if (Baseline == null)
                {
                    return BuildDiffKind.Added;
                }

                if (Current == null)
                {
                    return BuildDiffKind.Removed;
                }

                return string.Equals(Baseline.ContentHash, Current.ContentHash, StringComparison.Ordinal)
                    ? BuildDiffKind.Unchanged
                    : BuildDiffKind.Modified;
            }
        }
    }

    internal sealed class BuildDiffResult
    {
        public BuildDiffResult(BuildArchiveAnalysis baseline, BuildArchiveAnalysis current, IReadOnlyList<BuildDiffEntry> entries)
        {
            Baseline = baseline;
            Current = current;
            Entries = entries;
        }

        public BuildArchiveAnalysis Baseline { get; }
        public BuildArchiveAnalysis Current { get; }
        public IReadOnlyList<BuildDiffEntry> Entries { get; }
        public long ArchiveSizeDelta => Current.ArchiveSize - Baseline.ArchiveSize;
    }
}
