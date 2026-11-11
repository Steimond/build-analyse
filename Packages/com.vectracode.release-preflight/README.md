# Release Preflight 1.0.0

Open **Tools > Release Preflight > Open** and select **Scan release settings**. Review each finding and its suggested action. Open Player Settings or Build Profiles to make changes yourself, then rescan. Export JSON to share or archive the snapshot.

## Scope

Checks the active build target and current EditorBuildSettings scene list, development/debug/profiler flags, company/product/version and application identifier. For Android, also checks a positive version code and custom-keystore toggle. Does not open scenes or alter project settings, scene contents, or signing configuration. Signing passwords and keystore contents are never read.

A scan checks configuration only. It does not prove a successful compilation/build, detect missing references inside scenes, enforce increasing store version numbers, verify certificates, test runtime behavior, or certify store acceptance. Warnings require review and do not fail CI. Scan again after changing settings or the active Build Profile. CI must use the intended target/profile; the tool does not switch them.

## CI

Run Unity in batch mode with your project and intended target:

```
Unity -batchmode -nographics -projectPath <project> -executeMethod VectraCode.ReleasePreflight.PreflightChecks.RunCi -quit
```

Writes `Artifacts/release-preflight.json` relative to the project working directory. Error findings throw BuildFailedException so Unity batch execution fails. Warnings alone do not fail. Report generation errors also fail the invocation. No build is created. Reports contain scene paths; review before sharing.

## Compatibility and support

Editor-only, render-pipeline independent. Requires Unity 6; release testing is pending. Uses only Unity and .NET APIs, with no external dependencies, telemetry, or online services. Publisher support details will be provided in the Asset Store listing. See LICENSE.md and CHANGELOG.md.
