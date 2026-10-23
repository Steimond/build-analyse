# Portfolio release candidates — 2026-10-23

## Selected next slice

Two standalone tools after Build Diff Tool: **Build Size Analyzer** (free) and **Release Preflight** ($7.99). User approved these prices on 2026-10-23; both are saved in Publisher Portal. Build Diff Tool remains $7.99.

Market review: Unity 6.6 already provides Build Analysis and build history, so Build Size Analyzer focuses on completed APK/AAB/IPA ZIP contents without a source project, instead of recreating Unity BuildReport views. Release Preflight focuses on explicit configuration checks and a CI JSON snapshot. Competing release validators exist; no claim of uniqueness is made.

Sources reviewed:
- https://discussions.unity.com/t/unity-6-6-is-now-available/1735357
- https://github.com/Unity-Technologies/BuildReportInspector
- https://assetstore.unity.com/packages/tools/utilities/release-check-pro-game-asset-validator-401728
- https://docs.unity.com/en-us/asset-store/publishing/asset-packages/upload

## Release gates

- [x] Compile and run EditMode tests for all three tools (Unity 6000.6.6f1, 11/11 passed).
- [ ] Smoke-test UI and CSV/JSON export in Unity.
- [x] Run Asset Store Validator for each product root (no errors, one demo-scene warning per Editor-only tool).
- [x] Export three independent unitypackages (Tools > Build Diagnostics > Export Standalone Packages).
- [x] Clean-project import and compilation test for each package, Unity 6000.6.6f1.
- [ ] Real screenshots of each tool, plus product media.
- [ ] Confirm publisher identity, support website/email, product pricing, and AI disclosure.
- [x] Create two new Publisher Portal drafts, save descriptions, technical details, AI disclosures, keywords, and version 1.0.0. Existing Build Diff draft version updated to 1.0.0.
- [ ] Complete media/support fields and preview all listings.
- [ ] Upload validated packages, then submit when all gates are complete.

CI smoke test: Release Preflight in a clean project without scenes wrote JSON with scenes.empty and returned exit code 1 as expected.

Publisher Portal drafts: Build Diff Tool 1491784; Build Size Analyzer 1529970; Release Preflight 1529972. All three show 0 Bytes: packages must be uploaded through Tools > Asset Store > Uploader inside Unity. Browser upload is unavailable. Unity GUI opened, but this project's Pipeline server cannot be reached and native UI control is unavailable in this session. No product has been uploaded, submitted, or approved. Source implementation is not publication readiness.

Local packages: Artifacts/Build-Diff-Tool-1.0.0.unitypackage, Artifacts/Build-Size-Analyzer-1.0.0.unitypackage, Artifacts/Release-Preflight-1.0.0.unitypackage.
Evidence: Artifacts/portfolio-editmode.xml, Artifacts/*-validation.json, Logs/smoke-*.log, Artifacts/publisher-drafts.png.
