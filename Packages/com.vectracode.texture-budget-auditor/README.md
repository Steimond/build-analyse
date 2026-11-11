# Texture Budget Auditor 1.0.0

Review imported 2D textures against dimension and RGBA8 reference budgets.

## Getting started

Select Tools > Texture Budget Auditor > Open. Choose an optional Assets folder, set positive budgets (defaults: 2048 maximum dimension, 16 MiB RGBA8 reference), and click Scan. Change importer settings yourself if appropriate and rescan.

## Features

Shows imported dimensions, imported format, mipmap and streaming flags, and Read/Write findings. Configurable maximum dimension and per-texture RGBA8 reference MiB budget. Sorts largest reference budgets first. Select assets and export CSV/JSON.

## Scope and limitations

RGBA8 reference is width x height x 4 plus the full mip chain when enabled. It is not measured GPU memory, platform-compressed size, VRAM residency, device performance, or build size. The current imported target and its overrides affect dimensions and format. Only TextureImporter-backed Texture2D assets are included. Cubemaps, arrays, RenderTextures, generated textures, and atlas packing are excluded. Read/Write and budget findings require review and do not imply a texture is broken.

Estimated RGBA8 reference bytes for each imported texture, including a full mip chain when enabled.

Scans are synchronous and provide a cancellable progress dialog. Cancelling discards the partial report. Read failures are exported and set scanCompleted=false; an incomplete scan is never a clean result. The window shows the first 500 matching rows and up to 50 errors. CSV/JSON exports include the complete snapshot regardless of UI filters. Results are snapshots: rescan after changing files or settings. There are no automatic project changes. Selecting a result changes only the Editor selection.

## Reports and privacy

JSON includes tool version, UTC timestamp, Unity version, scope, completion status, rows and errors. CSV includes completion status and scan errors. Review paths and error messages before sharing; errors may include absolute local paths. No telemetry, accounts, network requests, external services, or runtime components. Files are read locally; exports are saved only when you choose a destination. Formula-like text is escaped in CSV.

## Compatibility

Requires Unity 6; the release candidate is being validated with Unity 6000.6.6f1 on Windows. Editor-only and independent of render pipeline. No third-party dependencies. This product is standalone; it does not require any other VectraCode tool. Support details are provided through the publisher profile on its Asset Store listing.

See LICENSE.md, CHANGELOG.md, and Third-Party Notices.txt.
