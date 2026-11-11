# Asset Store listing draft

## Title
Prefab Missing Script Scanner

## Summary
Find missing MonoBehaviour slots in prefab assets, including inactive children.

## Description
Scans .prefab assets in a selected Assets folder. Includes inactive children, nested prefab contents, and variants as resolved by Unity. Reports prefab path, indexed GameObject hierarchy path, and missing slot count. Includes clean prefab rows. Select assets and export CSV/JSON.

Does not scan scenes, model prefabs, ScriptableObjects, or general missing object references. Fix Console compilation errors first, since unresolved script types may otherwise produce misleading findings. Nested issues can appear in source prefabs and parent/variant prefabs. Does not remove components, rewrite GUIDs, or repair scripts. Select opens the prefab asset in the Project window; the hierarchy path identifies the object for manual inspection.

Runs locally, with no account or network service. The scanner reads project assets without deleting, merging, or repairing them. Cancellable progress and CSV/JSON exports help you review results with your team.

## Technical details
Editor-only, Unity 6 required. Release validation target: Unity 6000.6.6f1 on Windows. Render-pipeline independent, no external dependencies. Missing MonoBehaviour slot count on each reported GameObject; clean prefab rows use zero. Reports include scan scope and errors; cancelled scans are discarded, incomplete scans are labelled. Window displays up to 500 matching rows; exports include all rows. Setup and limitations are documented in the included README.md.

## Keywords
editor, diagnostics, assets

## Price (draft for review)
Free

## AI disclosure
AI assistance was used to develop C# source code, tests, and documentation. No generated runtime assets are included.

## Remaining before submission
Real product screenshots and required media, upload validated package through Unity Uploader, verify maintained support details and final pricing, inspect listing Preview. No submission or store acceptance claim.
