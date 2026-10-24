# Duplicate Asset Finder 1.0.0

Find identical source-file content in your Assets folder without deleting or merging assets.

## Getting started

Select Tools > Duplicate Asset Finder > Open. Choose an optional Assets folder and click Scan. Each hash identifies a content group. Review references and importer settings before any manual cleanup.

## Features

Groups non-empty asset source files by length and SHA-256 hash. Reads candidate files in chunks. Reports full content hashes, source paths and source bytes. Select results and export CSV/JSON.

## Scope and limitations

GUIDs and .meta import settings are excluded, so identical source files can import differently. Matching content does not prove safe removal, equivalent behavior, or build-space savings. No automatic fixes, deletion, merging, or reference rewriting. Empty files and symlinks/junctions are excluded; a linked or unreadable candidate marks the scan incomplete. Files changing during a scan require a rescan.

Source bytes per row; the sum of listed bytes includes every copy and is not the amount safely reclaimable.

Scans are synchronous and provide a cancellable progress dialog. Cancelling discards the partial report. Read failures are exported and set scanCompleted=false; an incomplete scan is never a clean result. The window shows the first 500 matching rows and up to 50 errors. CSV/JSON exports include the complete snapshot regardless of UI filters. Results are snapshots: rescan after changing files or settings. There are no automatic project changes. Selecting a result changes only the Editor selection.

## Reports and privacy

JSON includes tool version, UTC timestamp, Unity version, scope, completion status, rows and errors. CSV includes completion status and scan errors. Review paths and error messages before sharing; errors may include absolute local paths. No telemetry, accounts, network requests, external services, or runtime components. Files are read locally; exports are saved only when you choose a destination. Formula-like text is escaped in CSV.

## Compatibility

Requires Unity 6; the release candidate is being validated with Unity 6000.6.6f1 on Windows. Editor-only and independent of render pipeline. No third-party dependencies. This product is standalone; it does not require any other VectraCode tool. Support details are provided through the publisher profile on its Asset Store listing.

See LICENSE.md, CHANGELOG.md, and Third-Party Notices.txt.
