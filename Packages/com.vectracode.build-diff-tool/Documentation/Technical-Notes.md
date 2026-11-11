# Technical notes and limitations

## Supported files

Build Diff Tool supports completed, valid ZIP-based mobile build archives with the following extensions:

- `.apk`
- `.aab`
- `.ipa`

The tool does not include sample APK, AAB, or IPA files. Use your own completed build artifacts; they remain on your computer and are never uploaded.

## How comparison works

The tool reads each archive entry, records its path, compressed size, uncompressed size, and SHA-256 hash. It compares entries by normalized archive path.

The reported per-file delta is based on compressed entry size. ZIP container overhead, signing blocks, and some platform-specific metadata can make the archive-level delta differ slightly from the sum of file-level deltas.

## Current limitations

- It compares packaged files, not Unity `BuildReport` data.
- It does not yet map generated Android/iOS output to their original Unity assets except when the archive path already equals an `Assets/...` path.
- It does not inspect nested archives or native binary symbol contents.
- Large archives take longer because every file is hashed for a correct unchanged/modified result.

## Privacy

No network connection is used. Build contents, paths, hashes, and analysis results remain local to the Unity Editor session.
