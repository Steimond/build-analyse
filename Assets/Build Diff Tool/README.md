# Build Diff Tool

Build Diff Tool compares two APK, AAB, or IPA archives in the Unity Editor. It answers a simple release question: which files made the new mobile build larger or smaller?

**Version:** 1.0.0
**Validated editor:** Unity 6.6.2f1
**Menu:** `Tools > Build Diff Tool`

## Features

- Compares valid APK, AAB, and IPA archives locally.
- Shows added, removed, modified, and unchanged files.
- Uses SHA-256 hashes to distinguish identical files from files with equal sizes but different contents.
- Sorts by status, path, previous size, new size, or delta.
- Marks growth in red, reduction in green, unchanged files in white, and changed files with a zero size delta in amber.
- Selects an asset in the Project window when an archive entry path directly matches an `Assets/...` path.

## Documentation

- [Getting started](Documentation/Getting-Started.md)
- [Technical notes and limitations](Documentation/Technical-Notes.md)
- [Troubleshooting](Documentation/Troubleshooting.md)
- [Changelog](CHANGELOG.md)

## Privacy and dependencies

Build Diff Tool runs entirely in the Unity Editor. It does not send build files, file paths, telemetry, or project data over the network. It has no external packages or paid services.
