# Four-tool batch — review handoff

Prepared at the user's request on 2026-10-23 (Europe/Budapest). The scope is four new products and filled Publisher Portal drafts. The user will upload package files and screenshots later; no product should be submitted for review during this batch.

## Products

| Product | Draft price for review | Workflow | Portal draft |
| --- | --- | --- | --- |
| Asset Dependency Explorer | $4.99 | Direct/transitive dependency lists and scoped direct referrers. | 1530258 |
| Duplicate Asset Finder | $4.99 | Group identical non-empty source files by size and SHA-256. | 1530268 |
| Texture Budget Auditor | $4.99 | Imported Texture2D dimensions, Read/Write flags, and RGBA8 reference budgets. | 1530272 |
| Prefab Missing Script Scanner | Free | Find missing MonoBehaviour slots in .prefab files, including inactive children. | 1530274 |

Prices are provisional choices for the user's morning review, not a request to launch at these prices. All products are version 1.0.0 release candidates, Editor-only, local, read-only, with no cross-product dependency.

## Completed validation

- Unity 6000.6.6f1 on Windows: all 21 EditMode tests passed across the portfolio. Includes actual asset dependency fixtures, different same-length file content, texture importer preservation, and a deliberately broken script GUID in a Unity-generated prefab.
- Asset Store Publishing Tools 0.3.2: no failures or compilation errors for all four roots. One warning per root: no demo scene, consistent with these Editor-only utilities. Actual UI screenshots still need to demonstrate them.
- Each exported .unitypackage imported into its own empty Unity project with no external dependencies; a separate batch run compiled each project successfully.
- Cancelled scans discard partial reports. Read failures are retained as errors and set scanCompleted=false. CSV exports escape formula-like text.

Evidence: `Artifacts/diagnostics-editmode.xml`, `Artifacts/<Product>-validation.json`, `Logs/smoke-<Product>.log` and exported packages under `Artifacts`.

All four Publisher Portal cards have summary, full description, technical details, AI disclosure, category Tools/Utilities, version 1.0.0, release notes, and at least three relevant keywords. Descriptions, AI disclosures and release notes were checked again after reopening the pages. The refreshed package list confirms exactly seven drafts, with the four new prices shown above. Screenshots and package uploads are deliberately left to the user. No Submit action was taken.

Saved portal evidence: `Artifacts/publisher-seven-drafts.png` and `Artifacts/publisher-seven-drafts.txt`. `Artifacts/diagnostics-release-manifest.json` records package filenames, byte sizes, SHA-256 checksums, and draft links.

Validation used `Artifacts/DiagnosticsValidation` so the already-open user project did not need to be closed. Source .meta GUIDs were preserved in the exports. Test fixtures and publisher tools are excluded from standalone packages.

## Before submission

- Review each tool's scope and limitations in its README; inspect the actual Editor windows. This batch verifies services, compilation and packaging, not visual UI behavior.
- Capture real product screenshots, prepare required media, and upload packages through Tools > Asset Store > Uploader.
- Review draft descriptions, AI disclosure, keywords, version, provisional price, and publisher support details; open listing Preview.
- Submit only when all release gates are complete.

## Market check

These workflows have existing alternatives; no uniqueness claim is made. The narrow scope and read-only reports are deliberate. Unity Search can already find missing scripts in a hierarchy, while this scanner produces scoped prefab inventory reports. Unity's Asset Auditor and newer Project Auditor overlap texture diagnostics; this tool uses an explicit, documented RGBA8 reference metric rather than claiming measured runtime memory.

- https://assetstore-fallback.unity.com/packages/tools/utilities/duplicate-cleaner-316234
- https://github.com/Unity-Technologies/asset-auditing-tools/blob/master/Documentation/AssetAuditor.md
- https://docs.unity.com/en-us/engine/6000.0/manual/unity-editor/editor-navigating-managing/editor-searching/search-overview/search-by-provider/search-project-scenes/search-scene
- https://assetstore.unity.com/packages/tools/asset-hunter-suite-9722953
