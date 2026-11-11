# VectraCode diagnostics tools

Standalone, offline Unity Editor tools. Each product owns its Editor assembly, documentation, and export root. None requires another product in this repository.

| Tool | Workflow |
| --- | --- |
| Build Diff Tool | Compare APK/AAB/IPA archive contents and size changes. |
| Build Size Analyzer | Inspect a completed mobile archive and export file-size reports. |
| Release Preflight | Review release settings and export a CI checklist. |
| Asset Dependency Explorer | Inspect direct/transitive dependencies and scoped direct referrers. |
| Duplicate Asset Finder | Group source files by identical SHA-256 content without cleanup actions. |
| Texture Budget Auditor | Review imported 2D textures against explicit reference budgets. |
| Prefab Missing Script Scanner | Find missing MonoBehaviour slots in prefab assets. |

Open each tool through `Tools > <Product> > Open`. Build Diff uses its existing Tools menu.

Each product now lives in an embedded UPM package under `Packages/com.vectracode.*`. Run `./tools/Export-Upm.ps1` to build seven `.tgz` archives. Development helpers and tests remain under `Assets` and are excluded. See [UPM publishing and screenshot capture](docs/UPM-Publishing.md).

See [release candidates](docs/Portfolio-Release-Candidates.md) and [the four-tool review handoff](docs/Diagnostics-Batch-Review.md) for validation evidence and Publisher Portal drafts. Exported packages and saved drafts do not mean a product has been uploaded, submitted, or approved.
