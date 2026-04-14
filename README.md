# gh-watch

A Grasshopper plugin that embeds a live 3D viewer directly inside a component on the canvas. Connect any geometry to the **Watch** component and inspect it in an interactive Three.js viewport — without leaving Grasshopper.

## Features

- Real-time geometry streaming from Rhino/Grasshopper into an embedded WebView2 panel
- Supports Breps, Surfaces, Meshes, Curves, Lines, and Points
- Shaded mesh with wireframe and edge overlays
- Orbit, pan, and zoom controls inside the viewer
- Resizable panel that stays anchored as you pan and zoom the canvas

## Requirements

| | Version |
|---|---|
| Rhino | 8 (Windows) |
| Visual Studio | 2022 |
| .NET SDK | 8.0 |
| Node.js | 18+ |

## Setup

### 1 — Frontend

The plugin loads the viewer from a running frontend server. Start it before opening Grasshopper:

```bash
cd watch-frontend
npm install
npm run dev        # starts on http://localhost:5173
```

The URL is configured in `Gh.Watch/WatchPanel.cs`:

```csharp
_webView.Navigate("http://localhost:5173");
```

Change this to point at a production build URL or a local file path as needed.

### 2 — Plugin

Open `Gh.Watch/GrasshopperWatch.sln` in Visual Studio 2022 / 2026 and build. Copy the output `Gh.Watch.gha` to your Grasshopper libraries folder:

```
%APPDATA%\Grasshopper\Libraries\
```

Restart Rhino, open Grasshopper, and find the **Watch** component under `Display > Preview`.

## Project Structure

```
gh-watch/
├── Gh.Watch/           # C# Grasshopper plugin
│   ├── Watch_Component.cs
│   ├── WatchAttributes.cs
│   ├── WatchPanel.cs
│   ├── Serialization/
│   ├── Dtos/
│   └── Extensions/
├── watch-frontend/     # React / Three.js viewer
│   ├── src/
│   └── public/
└── docs/
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

[MIT](LICENSE) — © 2026 Omkar Bhagwat
