# Asset Store listing draft

## Title
Texture Budget Auditor

## Summary
Review imported 2D textures against dimension and RGBA8 reference budgets.

## Description
Shows imported dimensions, imported format, mipmap and streaming flags, and Read/Write findings. Configurable maximum dimension and per-texture RGBA8 reference MiB budget. Sorts largest reference budgets first. Select assets and export CSV/JSON.

RGBA8 reference is width x height x 4 plus the full mip chain when enabled. It is not measured GPU memory, platform-compressed size, VRAM residency, device performance, or build size. The current imported target and its overrides affect dimensions and format. Only TextureImporter-backed Texture2D assets are included. Cubemaps, arrays, RenderTextures, generated textures, and atlas packing are excluded. Read/Write and budget findings require review and do not imply a texture is broken.

Runs locally, with no account or network service. The scanner reads project assets without deleting, merging, or repairing them. Cancellable progress and CSV/JSON exports help you review results with your team.

## Technical details
Editor-only, Unity 6 required. Release validation target: Unity 6000.6.6f1 on Windows. Render-pipeline independent, no external dependencies. Estimated RGBA8 reference bytes for each imported texture, including a full mip chain when enabled. Reports include scan scope and errors; cancelled scans are discarded, incomplete scans are labelled. Window displays up to 500 matching rows; exports include all rows. Setup and limitations are documented in the included README.md.

## Keywords
editor, diagnostics, assets

## Price (draft for review)
4.99

## AI disclosure
AI assistance was used to develop C# source code, tests, and documentation. No generated runtime assets are included.

## Remaining before submission
Real product screenshots and required media, upload validated package through Unity Uploader, verify maintained support details and final pricing, inspect listing Preview. No submission or store acceptance claim.
