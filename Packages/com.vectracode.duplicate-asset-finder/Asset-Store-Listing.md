# Asset Store listing draft

## Title
Duplicate Asset Finder

## Summary
Find identical source-file content in your Assets folder without deleting or merging assets.

## Description
Groups non-empty asset source files by length and SHA-256 hash. Reads candidate files in chunks. Reports full content hashes, source paths and source bytes. Select results and export CSV/JSON.

GUIDs and .meta import settings are excluded, so identical source files can import differently. Matching content does not prove safe removal, equivalent behavior, or build-space savings. No automatic fixes, deletion, merging, or reference rewriting. Empty files and symlinks/junctions are excluded; a linked or unreadable candidate marks the scan incomplete. Files changing during a scan require a rescan.

Runs locally, with no account or network service. The scanner reads project assets without deleting, merging, or repairing them. Cancellable progress and CSV/JSON exports help you review results with your team.

## Technical details
Editor-only, Unity 6 required. Release validation target: Unity 6000.6.6f1 on Windows. Render-pipeline independent, no external dependencies. Source bytes per row; the sum of listed bytes includes every copy and is not the amount safely reclaimable. Reports include scan scope and errors; cancelled scans are discarded, incomplete scans are labelled. Window displays up to 500 matching rows; exports include all rows. Setup and limitations are documented in the included README.md.

## Keywords
editor, diagnostics, assets

## Price (draft for review)
4.99

## AI disclosure
AI assistance was used to develop C# source code, tests, and documentation. No generated runtime assets are included.

## Remaining before submission
Real product screenshots and required media, upload validated package through Unity Uploader, verify maintained support details and final pricing, inspect listing Preview. No submission or store acceptance claim.
