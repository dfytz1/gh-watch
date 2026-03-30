# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**gh-watch** is a Grasshopper 3D (Rhino) plugin that embeds a WebView2 browser panel in the Grasshopper canvas and streams real-time geometry visualizations from Rhino to a React/Three.js frontend.

The repo has two sub-projects:
- `gh/` — C# .NET Grasshopper plugin (backend)
- `gh-watch-frontend/` — React/TypeScript/Three.js web app (frontend)

## Commands

### Frontend (`gh-watch-frontend/`)
```bash
npm run dev          # Vite dev server on http://localhost:5173
npm run build        # tsc -b && vite build
npm run lint         # ESLint check
npm run copy-wasm    # Copy rhino3dm.wasm to public/ (run once after install)
```

### Backend (`gh/`)
```bash
dotnet build gh.csproj   # Build the C# plugin
```
The plugin is also built via Visual Studio 2022 (solution: `gh/gh.sln`).

## Architecture

### Data Flow

```
Grasshopper solve → ghComponent.cs
  → SerializationHelper (IGH_Goo → JSON DTOs)
  → WatchPanel (WebView2 PostWebMessageAsJson)
  → [WebView2 message channel]
  → wv.ts (registerWebViewMessageHandlers)
  → App.tsx → MeshView (Three.js BufferGeometry)
```

### C# Layer

- **ghComponent.cs** — Grasshopper component; receives generic geometry input, calls serialization, posts to the panel
- **WatchAttributes.cs** — Custom `GH_ResizableAttributes` subclass; manages the resizable canvas panel and synchronizes the Win32 HWND (WebView2 window) with canvas zoom/pan transforms
- **WatchPanel.cs** — WinForms `UserControl` hosting the WebView2 instance; calls `PostWebMessageAsJson` to send data to the frontend
- **Serialization/SerializationHelper.cs** — Pattern-matches on `IGH_Goo` types (GH_Brep, GH_Surface, GH_Box, GH_Mesh, GH_Curve, GH_Line, GH_Point), meshes surfaces using `MeshingParameters.FastRenderMesh`, and produces flat typed arrays for performance
- **Dtos/** — `SendDataDto` (wrapper with `eventType`/`payload`), `MeshPayloadDto`, `LinePayloadDto`, `PointPayloadDto`, `CurvePayloadDto`
- **Constants/** — `GeometryType` string constants; `SendToWebvViewCommand` ("geometry")

### React/TypeScript Layer

- **webview-communication/wv.ts** — Declares `window.chrome.webview` global interface; `registerWebViewMessageHandlers()` wires up C#→JS messages
- **geometry-views/mesh-view.tsx** — Builds `BufferGeometry` from flat typed arrays (zero-copy), renders shaded mesh + wireframe, explicitly disposes old geometry on update
- **props/payload-props/** — TypeScript interfaces mirroring the C# DTOs: `IMeshPayload`, `IPointPayload`, `ICurvePayload`
- **App.tsx** — React Three Fiber canvas with ambient light, Grid helper, OrbitControls, axes, and MeshView
- **main.tsx** — Sets `THREE.Object3D.DEFAULT_UP = (0,0,1)` for Rhino's Z-up convention

### Message Protocol (C# → Frontend)

```json
{
  "eventType": "geometry",
  "payload": [
    { "type": "mesh", "vertices": [...], "normals": [...], "faces": [...] },
    { "type": "point", "x": 1.0, "y": 2.0, "z": 3.0 },
    { "type": "curve", "buffer": [...] }
  ]
}
```

## Key Technical Details

- **Dual-target framework**: `gh.csproj` targets both `.NET 8.0-windows` (Rhino 8) and `.NET Framework 4.8` (Rhino 7); WebView2 and Newtonsoft.Json are conditional on framework version
- **WebView2 user data folder**: Stored at `%APPDATA%\gh-watch-webview` by WatchPanel
- **Geometry meshing**: Uses `FastRenderMesh` (not high-quality) for real-time performance; Breps/Surfaces are joined into a single mesh before serialization
- **Curves**: Sampled into 64-segment polylines; the flat point buffer matches `ICurvePayload.buffer`
- **Canvas positioning**: `WatchAttributes` maps Grasshopper canvas coordinates to screen pixels every frame, repositioning the underlying Win32 window accordingly
