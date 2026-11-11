# Asset Dependency Explorer 1.0.0

Inspect asset dependencies and direct referrers without changing your project.

## Getting started

Select Tools > Asset Dependency Explorer > Open. Pick a target asset, choose the direction, and optionally choose the Assets folder to search for referrers. Click Scan. Rescan after changing assets.

## Features

Choose an asset file and inspect direct or recursive dependencies. Find direct referrers within a selected Assets folder. Select an asset from any result. Export complete CSV/JSON snapshots.

## Scope and limitations

Uses AssetDatabase.GetDependencies. Runtime loading paths, Resources strings, Addressables keys, reflection, and custom content-loading code are not inferred. No unused-asset or safe-to-delete claim. Forward dependencies can include Packages. Folder scope applies to referrer scans only. Asset-level results do not identify individual serialized properties or render a graphical node view.

No size metric. Rows are direct or transitive dependency paths, or direct referrer paths.

Scans are synchronous and provide a cancellable progress dialog. Cancelling discards the partial report. Read failures are exported and set scanCompleted=false; an incomplete scan is never a clean result. The window shows the first 500 matching rows and up to 50 errors. CSV/JSON exports include the complete snapshot regardless of UI filters. Results are snapshots: rescan after changing files or settings. There are no automatic project changes. Selecting a result changes only the Editor selection.

## Reports and privacy

JSON includes tool version, UTC timestamp, Unity version, scope, completion status, rows and errors. CSV includes completion status and scan errors. Review paths and error messages before sharing; errors may include absolute local paths. No telemetry, accounts, network requests, external services, or runtime components. Files are read locally; exports are saved only when you choose a destination. Formula-like text is escaped in CSV.

## Compatibility

Requires Unity 6; the release candidate is being validated with Unity 6000.6.6f1 on Windows. Editor-only and independent of render pipeline. No third-party dependencies. This product is standalone; it does not require any other VectraCode tool. Support details are provided through the publisher profile on its Asset Store listing.

See LICENSE.md, CHANGELOG.md, and Third-Party Notices.txt.
