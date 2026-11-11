# Build Size Analyzer 1.0.0

Open **Tools > Build Size Analyzer > Open**, choose a completed APK, AAB, or IPA, and inspect the largest compressed entries. Filter paths, expand the breakdown by file extension, or export every entry to CSV/JSON.

The tool works locally without the original Unity project, a BuildReport, external packages, or online services. It reads the ZIP directory without extracting or executing archive content. No archive content is decompressed. Duplicate entry names remain separate rows. The window shows up to 500 matching entries; exports include all entries. Extension summaries show the largest 20 groups.

## Interpreting sizes

Archive bytes include ZIP headers and other overhead; summed entry bytes do not. Uncompressed bytes are the sizes recorded in the archive directory. These are not installed size, device-specific download estimates, runtime memory, Unity source-asset sizes, or verified content integrity. APK/AAB and IPA archives must be valid ZIP containers. Encrypted or damaged archives are unsupported. Serialized Unity data is not decoded or mapped to project assets.

Analysis currently runs synchronously in the Editor. Very large archive directories may pause the window. CSV escapes formula-like paths for spreadsheet use. JSON exports absolute local archive paths: review reports before sharing.

## Compatibility and support

Editor-only, render-pipeline independent. Requires Unity 6; release testing is pending. No runtime code or third-party dependencies. Publisher support details will be provided in the Asset Store listing. See LICENSE.md and CHANGELOG.md.
