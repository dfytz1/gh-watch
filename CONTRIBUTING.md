# Contributing

## Prerequisites

- Rhino 8 (Windows)
- Visual Studio 2022 with .NET 8 workload
- Node.js 18+

## Dev Setup

**Frontend**

```bash
cd watch-frontend
npm install
npm run dev        # http://localhost:5173
```

**Plugin**

Open `Gh.Watch/GrasshopperWatch.sln` in Visual Studio 2022.  
Build → copy `Gh.Watch.gha` to `%APPDATA%\Grasshopper\Libraries\`.

For a fast iteration cycle, keep the Vite dev server running, rebuild the `.gha`, and use Grasshopper's *Unload/Reload* to pick up changes without restarting Rhino.

## Branches

| Pattern | Purpose |
|---|---|
| `feature/<issue>-short-description` | New functionality |
| `fix/<issue>-short-description` | Bug fixes |

Branch off `main`, open a PR back to `main`.

## Commits

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add point cloud support
fix: wire grip offset on high-DPI canvas
docs: update WebView2 hosting notes
```

## Pull Requests

- Keep PRs focused — one concern per PR
- Include a short description of what changed and why
- Reference the issue number if applicable

## Code Style

**C#** — standard .NET conventions: PascalCase for types and members, 4-space indent.  
**TypeScript / React** — follow the existing ESLint config (`npm run lint`).

## Project Notes

- The frontend URL is hardcoded in `WatchPanel.cs`. If you need to test against a production build, update `_webView.Navigate(...)` locally — do not commit that change.
- `rhino3dm` is aliased to its browser ESM build in `vite.config.ts` — do not change this or the production build will fail.
- Each Watch component instance shares the WebView2 user data folder at `%APPDATA%\gh-watch-webview`, so the WASM cache is shared across instances.
