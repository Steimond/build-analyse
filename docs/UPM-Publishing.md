# UPM publication preparation

Seven embedded packages live under `Packages/com.vectracode.*`. Existing scripts and .meta GUIDs are preserved. Development helpers and tests remain in Assets and are excluded from the products.

Run `./tools/Export-Upm.ps1` to build seven `.tgz` archives in `Artifacts/UPM`. Each contains the required `package/` root. Package IDs use the provisional `com.vectracode` namespace: confirm the namespace assigned by Unity enrollment before upload. Existing classic portal drafts have not been converted or submitted.

## Screenshots

In an interactive Unity Editor choose `Tools > Build Diagnostics > Capture Screenshot Windows`. The development-only helper creates sample assets under `Assets/VectraCodeScreenshotSamples`, runs the actual analyzers and saves seven window captures in `Artifacts/Screenshots`. Keep Unity visible and windows unobstructed. Inspect captures before upload, especially scaling and window edges on high DPI monitors.

The sample APKs are synthetic ZIP fixtures, not runnable builds: label screenshots as sample data. The prefab example is clean, so its screenshot does not demonstrate missing-script detection. Preflight shows the actual project settings; inspect for sensitive values before publishing. No results are hard-coded.

`Prepare Screenshot Windows` prepares the same data for manual composition. Sample assets and screenshot helpers are development-only and excluded from packages.

## Release gates

Fresh compilation, EditMode tests, independent clean-project installs, store-specific validation, publisher verification, assigned namespace, media review and upload are required. Historical validation reports predate the migration. Local archives are not publication approval.

Verified on 2026-11-11 in Unity 6000.6.8f1: compilation succeeded; 22/22 EditMode tests passed (Artifacts/upm-editmode.xml). All 144 migrated original files retain their contents. Seven real window PNGs were captured and visually inspected. Independent clean-project tarball installs and Publisher Portal checks remain outstanding.
