# Asset Store listing draft — Release Preflight

## Short description
Review common release configuration risks in the Unity Editor and export a JSON checklist for CI.

## Description
Scan the active build target and current build scene list before creating a release. Review development-build, script-debugging and profiler flags, application identity and version fields, enabled/missing scenes, and Android version-code and custom-keystore settings. Each finding explains the issue and suggests a manual action.

The tool is read-only. Open Player Settings or Build Profiles to make changes yourself, then scan again. Export a JSON snapshot or run the documented batch entry point to fail CI on error findings. Warnings do not fail CI.

## Technical details
Editor-only. Unity 6 required; list exact verified editor versions after release testing. Render-pipeline independent. No external packages, services, or runtime code.

Checks configuration, not build success or store approval. Does not inspect scene contents, verify signing credentials, compare published version numbers, test devices, or certify platform compliance. Findings describe a snapshot of the current active target/profile; rerun after changes. A result with no findings means only that these specific checks found no issues.

## Privacy
No telemetry or network requests. Keystore contents and passwords are never read. Reports can include scene paths.

## AI disclosure draft
AI assistance was used in developing source code and documentation. Product screenshots must show the actual tool. Review this statement against the final delivered package before submission.

## Publisher fields still required
Verified publisher identity, support email and website, price/free decision, real screenshots, final compatibility results, and package upload.
