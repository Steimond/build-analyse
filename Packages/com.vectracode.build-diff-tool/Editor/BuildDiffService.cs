using System.Collections.Generic;
using System.Linq;

namespace BuildDiffTool.Editor
{
    internal static class BuildDiffService
    {
        public static BuildDiffResult Compare(BuildArchiveAnalysis baseline, BuildArchiveAnalysis current)
        {
            var paths = new HashSet<string>(baseline.Entries.Keys);
            paths.UnionWith(current.Entries.Keys);

            var entries = paths
                .Select(path => new BuildDiffEntry(
                    path,
                    baseline.Entries.TryGetValue(path, out var baselineEntry) ? baselineEntry : null,
                    current.Entries.TryGetValue(path, out var currentEntry) ? currentEntry : null))
                .OrderByDescending(entry => System.Math.Abs(entry.SizeDelta))
                .ThenBy(entry => entry.Path, System.StringComparer.Ordinal)
                .ToList();

            return new BuildDiffResult(baseline, current, entries);
        }
    }
}
