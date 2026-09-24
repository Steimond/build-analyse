# Troubleshooting

## “Not a valid APK/AAB/IPA archive”

The selected file has a supported extension but is not a complete ZIP-based archive. Select the finished build artifact, not a partially copied file, a renamed file, or an extracted folder.

## Select is not shown

The archive entry path does not correspond directly to an asset in the current project. This is expected for most Android and iOS build output.

## The archive-size total does not equal the sum of file deltas

Archive containers include metadata, alignment, signing data, and ZIP overhead. Build Diff Tool reports both the archive-level change and individual compressed-entry changes so that this difference is visible.

## Analysis takes time

The tool hashes every included file to avoid marking different files with equal byte sizes as unchanged. Larger archives therefore require more time to analyze.
