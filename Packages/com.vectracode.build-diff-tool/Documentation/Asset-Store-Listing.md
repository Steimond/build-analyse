# Asset Store listing draft

Use this text as the basis for the Publisher Portal listing. Review and edit it in the publisher's own voice before submission.

## Title

Build Diff Tool

## Short description

Find out exactly which files changed the size of your APK, AAB, or IPA build.

## Description

Build Diff Tool compares two mobile build archives directly in the Unity Editor. Select a baseline APK, AAB, or IPA and a newer build to see which packaged files were added, removed, modified, or left unchanged.

The sortable table highlights size increases in red and reductions in green, making it quick to investigate unexpected build growth. Content hashes distinguish unchanged files from files that happen to have the same size.

### Features

- Compare APK, AAB, and IPA archives locally.
- See added, removed, modified, and unchanged packaged files.
- Sort by status, path, previous size, new size, or size delta.
- Search paths and filter to changed files only.
- Use SHA-256 hashes for reliable content comparison.
- Select matching `Assets/...` paths in the Project window when available.

### Requirements and compatibility

- Validated with Unity 6.6.2f1.
- Editor-only; no runtime code is added to builds.
- Compatible with Built-in Render Pipeline, URP, and HDRP.
- No external packages, services, account, API key, or internet connection required.

### Important limitations

- Input files must be complete and valid APK, AAB, or IPA archives.
- Version 1.0.0 compares packaged files and does not generally map generated mobile build paths back to Unity source assets.
- File-level deltas use compressed archive-entry sizes; signing and container metadata can cause a small difference from the total archive-size delta.

### Privacy

Build files and analysis results stay on the local machine. The tool does not collect telemetry or transmit project data.

### Third-party notices

This package does not redistribute third-party content. See `Third-Party Notices.txt` in the package.

### AI description field

OpenAI Codex assisted with drafting C# source code and documentation. The publisher reviewed, tested, integrated, and refined the package before submission.

## Suggested tags

Build, Build Size, APK, AAB, IPA, Android, iOS, Mobile, Analysis, Optimization, Editor Tool
