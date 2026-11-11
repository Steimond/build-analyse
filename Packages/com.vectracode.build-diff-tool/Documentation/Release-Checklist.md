# Release checklist — 1.0.0

## Completed in this repository

- [x] Product version set to 1.0.0.
- [x] Separate Editor assembly and test assembly.
- [x] Offline setup, technical, and troubleshooting documentation.
- [x] Changelog, license statement, and Third-Party Notices file.
- [x] Asset Store listing draft, limitations, privacy statement, and AI description text.
- [x] No external dependencies, services, telemetry, sample executables, or runtime code.
- [x] Editor compilation and EditMode tests pass in Unity 6.6.2f1.
- [x] Asset Store Validator passes with one accepted warning: no demo scene. The product is an Editor-only archive analyzer; screenshots of the comparison window are the relevant demonstration media.

## Required before Publisher Portal submission

- [ ] Set the exact Asset Store publisher name, website, and support email in the Publisher Portal product details.
- [ ] Set the approved initial price of **$7.99** in the Publisher Portal draft. Category: Tools/Utilities.
- [ ] Create real screenshots of the working comparison window. Do not use sample APK/AAB/IPA files in the submitted package; the Asset Store does not accept embedded executables.
- [ ] Run Asset Store Publishing Tools Validator on `Assets/Build Diff Tool` and resolve every validator issue.
- [ ] Export `Assets/Build Diff Tool` as `Build Diff Tool 1.0.0.unitypackage` through Asset Store Publishing Tools.
- [ ] Create the Publisher Portal draft, paste the reviewed listing text, add media, and select the AI description disclosure.
- [ ] Confirm the publisher profile has a maintained website and active support email.
- [ ] Run the final smoke test in a clean Unity 6.6.2f1 project before upload.
