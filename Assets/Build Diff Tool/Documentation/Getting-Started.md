# Getting started

## Requirements

- Unity 6.6.2f1 or later.
- A completed APK, AAB, or IPA from two builds of the same product.

Build Diff Tool is Editor-only. It adds no runtime component, has no render-pipeline dependency, and works with Built-in Render Pipeline, URP, and HDRP projects.

## Compare two builds

1. Open **Tools > Build Diff Tool**.
2. Choose the older archive as **Baseline build**.
3. Choose the newer archive as **New build**.
4. Select **Analyze and compare**.

The table is initially ordered by the largest size increase. Select a column header to sort it; select the same header again to reverse its direction.

## Read the table

| Status | Meaning |
|---|---|
| Added | The file is in the new archive only. |
| Removed | The file is in the baseline archive only. |
| Modified | The path exists in both archives, but the contents differ. |
| Unchanged | The path and SHA-256 content hash match. |

The **Change** value uses compressed archive-entry size, which best reflects the contribution of each file to the downloadable archive. Red means the archive entry grew; green means it shrank. Amber means the contents changed but the compressed size did not.

## Select a project asset

The **Select** button appears only when the archive entry directly matches a project path such as `Assets/Textures/Icon.png`. Most APK, AAB, and IPA paths are generated build-output paths, so they cannot yet be mapped reliably to a Unity source asset.
