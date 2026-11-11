# Changelog

All notable changes to Build Diff Tool are documented in this file.

## 1.0.0 — 2026-09-24

### Added

- Comparison of APK, AAB, and IPA archives.
- SHA-256 content hashing for archive entries.
- File status detection for added, removed, modified, and unchanged entries.
- Sortable comparison table with search and a changed-files filter.
- Archive-size summary and colour-coded per-file size deltas.
- Asset selection for archive paths that match project asset paths.
- Offline documentation and Asset Store submission material.

### Fixed

- Invalid or incomplete mobile archives now show a clear in-window message without logging an exception to the Unity Console.
- Table rows and headers stay aligned in narrow windows.
