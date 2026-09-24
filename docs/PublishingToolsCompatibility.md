# Asset Store Publishing Tools and Unity 6.6

## Current workaround

`com.unity.upm-publishing-tools` 0.3.2 uses APIs that Unity 6.6.2f1 flags as compilation errors:

- `UAC0005` for `AppDomain.CurrentDomain.GetAssemblies()`.
- `UAC0007` for `Assembly.Location`.

The local PackageCache copy is patched after installation at:

`Library/PackageCache/com.unity.upm-publishing-tools@<hash>/Editor/Validator/Scripts/Services/Validation/ScriptUtilityService.cs`

1. Add `using UnityEngine;`.
2. Replace both calls to `AppDomain.CurrentDomain.GetAssemblies()` with `UnityEngine.Assemblies.CurrentAssemblies.GetLoadedAssemblies()`.
3. Replace `x.Location` with `x.GetLoadedAssemblyPath()`.

These edits are only for the Publisher Tools installed in this development project. They do not change or ship with `com.builddiff.tool`.

## When updating the publisher tool

Package Manager may replace the patched cache on an update or after deleting `Library`. Re-run this compatibility check before using the validator. Remove this workaround as soon as Unity publishes a version that compiles in Unity 6.6 without it.
