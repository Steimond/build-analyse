# Asset Store listing draft — Build Size Analyzer

## Short description
Inspect the largest packaged files in completed APK, AAB, and IPA archives, without the original Unity project.

## Description
Choose a mobile build archive in the Unity Editor to inspect compressed and uncompressed entry sizes. Find the largest files, filter paths, inspect size by extension, and export a complete CSV or JSON report for your team.

Works offline without a BuildReport or access to the original project. Reads archive metadata without extracting or executing files. Window rows are limited to the largest 500 matching entries; exports include all entries.

## Technical details
Editor-only. Unity 6 required; list exact verified editor versions after release testing. Render-pipeline independent. No third-party dependencies or runtime components. Inputs: ZIP-based APK/AAB/IPA. Outputs: CSV/JSON.

Sizes are archive-entry sizes, not installed size, store-download estimates, or runtime memory. Does not decode Unity serialized data, map packaged entries back to source assets, verify signatures, or test archive contents. Large archive directories are processed synchronously.

## Privacy
No telemetry or network requests. JSON reports include local archive paths; users choose where to save reports.

## AI disclosure draft
AI assistance was used in developing source code and documentation. Product screenshots must show the actual tool. Review this statement against the final delivered package before submission.

## Publisher fields still required
Verified publisher identity, support email and website, price/free decision, real screenshots, final compatibility results, and package upload.
