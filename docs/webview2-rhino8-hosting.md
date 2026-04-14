# WebView2 Hosting in a Rhino 8 Grasshopper Plugin

## Problem

When hosting WebView2 inside a WinForms `UserControl` using
`Microsoft.Web.WebView2.WinForms.WebView2` (the standard managed wrapper control),
calling `EnsureCoreWebView2Async` throws:

```
System.MissingMethodException: Method not found:
'Void Microsoft.Web.WebView2.Core.CoreWebView2Profile.add_Deleted(System.EventHandler`1<System.Object>)'.
   at Microsoft.Web.WebView2.WinForms.WebView2.InitCoreWebView2Async(...)
```

This happens **regardless of which NuGet SDK version is used**.

## Root Cause

Rhino 8 ships a patched `Microsoft.Web.WebView2.Core.dll` from
`C:\Program Files\Rhino 8\System\` that is missing `CoreWebView2Profile.add_Deleted`.
Rhino loads this DLL into the process at startup. The `Microsoft.Web.WebView2.WinForms.WebView2`
managed control calls `add_Deleted` internally during initialisation and hits the
missing method — the NuGet package version makes no difference because Rhino's Core DLL
is always the one in-process.

## Fix

Do **not** use `Microsoft.Web.WebView2.WinForms.WebView2` as a hosted control.
Use `CoreWebView2Environment` + `CoreWebView2Controller` with the WinForms control's
raw HWND instead. This bypasses the WinForms wrapper entirely and only touches Core
APIs that exist in Rhino's DLL.

The `Microsoft.Web.WebView2` NuGet package can remain at any version (including
prerelease) — it is only needed for compile-time types.

### gh.csproj

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Web.WebView2" Version="1.0.3965-prerelease" />
</ItemGroup>
```

No special `Private` or `ExcludeAssets` flags needed.

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

        // Pass the WinForms HWND directly.
        // This avoids the WinForms WebView2 wrapper control, which internally calls
        // CoreWebView2Profile.add_Deleted — a method missing from Rhino's patched Core DLL.
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
fundamental APIs present in WebView2 since its earliest releases — Rhino's patched
Core DLL has them. By passing the WinForms control's `Handle` (Win32 HWND) directly,
WebView2 creates a native child window inside the control, which is exactly what the
managed wrapper does internally — just without the newer API calls that Rhino's DLL
is missing.
