# Build Diff Tool

Build Diff Tool compares two APK, AAB, or IPA archives in the Unity Editor. It shows every file that was added, removed, modified, or left unchanged, then sorts the result by the largest compressed-size change.

Open it from **Tools > Build Diff Tool**.

## Current behavior

- Uses SHA-256 hashes to distinguish unchanged files from files whose contents changed.
- Uses compressed archive entry size to calculate each file's impact on archive size.
- Shows size changes in red for growth, green for reduction, white for unchanged files, and amber when content changed without changing archive size.
- The **Select** button selects a matching `Assets/...` path in the Project window when the archive path is also a project asset path.

This first version compares packaged files. Mapping Android and iOS build output back to Unity source assets is a later feature because archive paths usually do not directly match Unity asset paths.
