# LaTeX Formula Editor

A Windows desktop editor for writing LaTeX formulas and seeing them rendered as you
type. A WPF shell hosts a KaTeX page inside WebView2, with a symbol palette, live
preview, history and export to image or PDF.

## Features

- **Live preview** — the preview updates from the editor through a WebView2 ↔ C#
  bridge; render errors are shown inline instead of failing silently.
- **Two render engines** — KaTeX and MathJax, switchable at runtime.
- **Symbol palette** — tabs for fractions/roots, Greek letters, brackets, sums and
  integrals, matrices, arrows, and sets/logic.
- **Styling** — formula color, background color, font and font size, plus preview
  zoom.
- **Export** — save the rendered formula as PNG or PDF (SVG for some targets).
- **History** — recently used formulas, reopenable from a separate window.
- **Localized UI** — English, Chinese, German and Russian resources in
  `Resources/*.json`.
- **Help window** with an overview of the syntax and controls.

## How it works

```
WPF window
    └── WebView2 (Chromium)
            └── LaTeXRenderer.html — KaTeX / MathJax render the input
```

The C# side calls small JS functions (`renderLatex`, `setRenderEngine`,
`setFormulaColor`, …) via `ExecuteScriptAsync`, and reads results back for export.

## Requirements

- Windows 10/11
- .NET 8 SDK
- WebView2 Runtime (preinstalled on Windows 11; install the Evergreen runtime on
  Windows 10)
- Network access for the KaTeX CDN assets

## Build and run

```bash
cd LaTeXFormulaEditor
dotnet build
dotnet run
```

## Status

Real: rendering, palette, styling, export, history, localization.

Stubbed: image-to-LaTeX recognition and natural-language ↔ LaTeX conversion are
implemented against interfaces (`IFormulaRecognizer`,
`INaturalLanguageConverter`, `ILatexToNaturalLanguageConverter`) but currently use
mock implementations that return canned formulas. Dropping in a real backend means
writing one class per interface.

## Stack

C# 12, WPF on .NET 8, Microsoft.Web.WebView2 1.0.3912.50, KaTeX 0.16.9 (MathJax 3 for
the alternative engine), AvalonEdit for the editor, SkiaSharp + Svg.Skia for PNG/PDF
export.

## License

MIT
