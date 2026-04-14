# WebView2 Hosting in a Rhino 8 Grasshopper Plugin

## Problem

When hosting WebView2 inside a WinForms `UserControl` (used as a Grasshopper panel),
calling `EnsureCoreWebView2Async` throws:

```
System.MissingMethodException: Method not found:
'Void Microsoft.Web.WebView2.Core.CoreWebView2Profile.add_Deleted(System.EventHandler`1<System.Object>)'.
   at Microsoft.Web.WebView2.WinForms.WebView2.InitCoreWebView2Async(...)
```

## Root Cause

Rhino 8 ships its own `Microsoft.Web.WebView2.Core.dll` (v1.0.1938.49) from
`C:\Program Files\Rhino 8\System\`. It also ships `Microsoft.Web.WebView2.Wpf.dll`
but **not** `Microsoft.Web.WebView2.WinForms.dll`.

Rhino's Core DLL is a **patched build** missing some APIs that exist in the official
NuGet release at the same version — including `CoreWebView2Profile.add_Deleted`
(introduced in SDK ~1.0.2210). Because Rhino loads its Core DLL into the process
before any plugin, the NuGet WinForms wrapper always resolves against Rhino's patched
DLL and fails, regardless of which NuGet SDK version you specify.

## Fix

Do **not** use the `Microsoft.Web.WebView2` NuGet package. Instead:

1. Reference Rhino's Core DLL directly at compile time (not copied to output).
2. Use `CoreWebView2Environment` + `CoreWebView2Controller` with the WinForms
   control's raw HWND — no WinForms wrapper needed.

### gh.csproj

```xml
<ItemGroup>
  <!-- Compile-time reference to Rhino's own Core DLL.
       Private=false → not copied to output; Rhino loads it at runtime. -->
  <Reference Include="Microsoft.Web.WebView2.Core">
    <HintPath>C:\Program Files\Rhino 8\System\Microsoft.Web.WebView2.Core.dll</HintPath>
    <Private>false</Private>
  </Reference>
</ItemGroup>
```

### WatchPanel.cs

```csharp
using Microsoft.Web.WebView2.Core;
using System.Windows.Forms;

public class WatchPanel : UserControl
{
    private CoreWebView2Controller _controller;
    private CoreWebView2 _webView;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        _ = InitWebViewAsync();
    }

    private async Task InitWebViewAsync()
    {
        var userDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "gh-watch-webview");

        var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);

        // Pass the WinForms HWND directly — no WinForms wrapper DLL required.
        _controller = await env.CreateCoreWebView2ControllerAsync(Handle);
        _controller.Bounds = ClientRectangle;

        _webView = _controller.CoreWebView2;
        _webView.Navigate("http://localhost:5173");

        Resize += (s, _) =>
        {
            if (_controller != null)
                _controller.Bounds = ClientRectangle;
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _controller?.Close(); _controller = null; }
        base.Dispose(disposing);
    }
}
```

## Why This Works

`CoreWebView2Environment.CreateAsync` and `CreateCoreWebView2ControllerAsync` are
fundamental APIs present since WebView2's earliest releases. By passing the WinForms
control's `Handle` (Win32 HWND) directly, WebView2 creates a native child window
inside the control — identical to what the managed WinForms wrapper does internally,
but without calling any of the newer APIs that Rhino's patched Core DLL is missing.
