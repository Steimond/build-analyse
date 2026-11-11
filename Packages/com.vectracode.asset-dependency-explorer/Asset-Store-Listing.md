# Asset Store listing draft

## Title
Asset Dependency Explorer

## Summary
Inspect asset dependencies and direct referrers without changing your project.

## Description
Choose an asset file and inspect direct or recursive dependencies. Find direct referrers within a selected Assets folder. Select an asset from any result. Export complete CSV/JSON snapshots.

Uses AssetDatabase.GetDependencies. Runtime loading paths, Resources strings, Addressables keys, reflection, and custom content-loading code are not inferred. No unused-asset or safe-to-delete claim. Forward dependencies can include Packages. Folder scope applies to referrer scans only. Asset-level results do not identify individual serialized properties or render a graphical node view.

Runs locally, with no account or network service. The scanner reads project assets without deleting, merging, or repairing them. Cancellable progress and CSV/JSON exports help you review results with your team.

## Technical details
Editor-only, Unity 6 required. Release validation target: Unity 6000.6.6f1 on Windows. Render-pipeline independent, no external dependencies. No size metric. Rows are direct or transitive dependency paths, or direct referrer paths. Reports include scan scope and errors; cancelled scans are discarded, incomplete scans are labelled. Window displays up to 500 matching rows; exports include all rows. Setup and limitations are documented in the included README.md.

## Keywords
editor, diagnostics, assets

## Price (draft for review)
4.99

## AI disclosure
AI assistance was used to develop C# source code, tests, and documentation. No generated runtime assets are included.

## Remaining before submission
Real product screenshots and required media, upload validated package through Unity Uploader, verify maintained support details and final pricing, inspect listing Preview. No submission or store acceptance claim.
