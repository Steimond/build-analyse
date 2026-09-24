# Build diagnostics portfolio — Asset Store plan

## Goal

Build a coherent line of small Unity Editor tools for investigating mobile-build size and contents:

- Three paid standalone tools.
- Two free standalone tools.
- One paid **Build Diagnostics Suite** containing every tool, priced below buying the three paid standalone tools separately.

Each individual product should solve one clear workflow. The suite is a convenience purchase, not a different codebase.

## Delivery model

The development source stays modular: each tool owns its own assembly definition, tests, documentation, and semantic version. Every Store product is delivered as a traditional `.unitypackage`.

| Product type | Asset Store delivery | Reason |
|---|---|---|
| Free standalone tool | Traditional `.unitypackage` | One delivery and validation workflow for the entire line. |
| Paid standalone tool | Traditional `.unitypackage` | One delivery and validation workflow for the entire line. |
| Build Diagnostics Suite | Traditional `.unitypackage` | It can include the shared delivery roots for every included tool. |

Export every product from a unique top-level path such as `Assets/<PublisherName>/BuildDiffTool`. Keep a separate `.asmdef` for every tool. The suite should use one top-level root, for example `Assets/<PublisherName>/BuildDiagnosticsSuite`, so it does not overwrite an already imported standalone product.

## Product draft workflow

1. Create a Publisher Portal draft for every planned standalone product and one more for the suite. If the line has five standalone tools, this means six drafts.
2. Fill title, category, description, tags, support email, website, privacy text, AI disclosure, price, and media in each draft.
3. Save all changes and use **Preview** to inspect each listing.
4. Do **not** select **Submit**. Drafts are private and do not appear to customers.
5. Upload an asset package only after its release candidate passes validation.
6. Submit individual products only when their listing, support channel, and release archive are ready. Use manual publishing if a submitted product should wait for a coordinated launch.

## Pricing rule for the suite

Set the suite price lower than the combined price of the three paid standalone tools. Review price changes together whenever a new paid tool joins the suite so the bundle continues to be the clear value purchase.

## Initial price decision

Build Diff Tool launches as a paid product at **$7.99**. This is the low-friction price point for the first focused tool from a new publisher. Reassess the price after customer feedback and after larger features such as source-asset mapping or exportable reports are available.

## Before creating the drafts

- Choose the exact publisher display name and verify its support email and website.
- Decide which two tools are free and which three are paid.
- Decide the public names and one-sentence customer promise for each planned tool.
- Create real screenshots using the same visual style across the line.
- Keep `Build Diff Tool` at 1.0.0 until its first uploaded release candidate is ready.
