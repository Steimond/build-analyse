# Prefab Missing Script Scanner 1.0.0

Find missing MonoBehaviour slots in prefab assets, including inactive children.

## Getting started

Wait for compilation/import to finish and clear compilation errors. Select Tools > Prefab Missing Script Scanner > Open, choose an optional Assets folder, and click Scan. Use Review findings only to hide clean prefab rows.

## Features

Scans .prefab assets in a selected Assets folder. Includes inactive children, nested prefab contents, and variants as resolved by Unity. Reports prefab path, indexed GameObject hierarchy path, and missing slot count. Includes clean prefab rows. Select assets and export CSV/JSON.

## Scope and limitations

Does not scan scenes, model prefabs, ScriptableObjects, or general missing object references. Fix Console compilation errors first, since unresolved script types may otherwise produce misleading findings. Nested issues can appear in source prefabs and parent/variant prefabs. Does not remove components, rewrite GUIDs, or repair scripts. Select opens the prefab asset in the Project window; the hierarchy path identifies the object for manual inspection.

Missing MonoBehaviour slot count on each reported GameObject; clean prefab rows use zero.

Scans are synchronous and provide a cancellable progress dialog. Cancelling discards the partial report. Read failures are exported and set scanCompleted=false; an incomplete scan is never a clean result. The window shows the first 500 matching rows and up to 50 errors. CSV/JSON exports include the complete snapshot regardless of UI filters. Results are snapshots: rescan after changing files or settings. There are no automatic project changes. Selecting a result changes only the Editor selection.

## Reports and privacy

JSON includes tool version, UTC timestamp, Unity version, scope, completion status, rows and errors. CSV includes completion status and scan errors. Review paths and error messages before sharing; errors may include absolute local paths. No telemetry, accounts, network requests, external services, or runtime components. Files are read locally; exports are saved only when you choose a destination. Formula-like text is escaped in CSV.

## Compatibility

Requires Unity 6; the release candidate is being validated with Unity 6000.6.6f1 on Windows. Editor-only and independent of render pipeline. No third-party dependencies. This product is standalone; it does not require any other VectraCode tool. Support details are provided through the publisher profile on its Asset Store listing.

See LICENSE.md, CHANGELOG.md, and Third-Party Notices.txt.
