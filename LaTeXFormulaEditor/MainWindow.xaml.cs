using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Microsoft.Web.WebView2.Core;
using ICSharpCode.AvalonEdit;
using SkiaSharp;

namespace LaTeXFormulaEditor;

public partial class MainWindow : Window
{
    private bool _webViewReady = false;
    private readonly string _defaultLatex = @"\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}";
    private readonly IFormulaRecognizer _formulaRecognizer;
    private readonly INaturalLanguageConverter _naturalLanguageConverter;
    private readonly ILatexToNaturalLanguageConverter _latexToNaturalLanguageConverter;
    private bool _isRecognizing = false;
    private bool _isConverting = false;
    private readonly List<string> _history = new List<string>();
    private string _lastSavedFormula = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        KeyDown += MainWindow_KeyDown;
        latexEditor.Text = _defaultLatex;
        LocalizationManager.Initialize();
        LocalizationManager.LanguageChanged += OnLanguageChanged;
        _formulaRecognizer = new MockFormulaRecognizer();
        _naturalLanguageConverter = new MockNaturalLanguageConverter();
        _latexToNaturalLanguageConverter = new MockLatexToNaturalLanguageConverter();
        InitializeComboBoxes();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        InitializeWebView();
        InitializeEditor();
        InitializeLanguageComboBox();
        UpdateUI();
    }

    private void InitializeComboBoxes()
    {
        formulaColorComboBox.Items.Clear();
        var colors = new[] { "#000000", "#1E40AF", "#166534", "#7C3AED", "#DC2626" };
        foreach (var color in colors)
        {
            var item = new ComboBoxItem { Content = color, Tag = color };
            formulaColorComboBox.Items.Add(item);
        }
        formulaColorComboBox.SelectedIndex = 0;

        backgroundColorComboBox.Items.Clear();
        var bgColors = new[] { "#FFFFFF", "#F9FAFB", "#FEF3C7", "#DBEAFE" };
        foreach (var color in bgColors)
        {
            var item = new ComboBoxItem { Content = color, Tag = color };
            backgroundColorComboBox.Items.Add(item);
        }
        backgroundColorComboBox.SelectedIndex = 0;

        fontComboBox.Items.Clear();
        var fonts = new[] { "KaTeX", "Times New Roman", "Arial", "Courier New" };
        foreach (var font in fonts)
        {
            var item = new ComboBoxItem { Content = font, Tag = font };
            fontComboBox.Items.Add(item);
        }
        fontComboBox.SelectedIndex = 0;
        
        InitializeRenderEngineComboBox();
    }
    
    private void InitializeRenderEngineComboBox()
    {
        renderEngineComboBox.Items.Clear();
        var engines = new[]
        {
            ("KaTeX", "katex"),
            ("MathJax", "mathjax")
        };
        
        foreach (var (name, code) in engines)
        {
            var item = new ComboBoxItem { Content = name, Tag = code };
            renderEngineComboBox.Items.Add(item);
        }
        renderEngineComboBox.SelectedIndex = 0;
    }

    private void InitializeLanguageComboBox()
    {
        languageComboBox.Items.Clear();
        var languages = new[]
        {
            ("中文", "zh-CN"),
            ("English", "en-US"),
            ("Deutsch", "de-DE"),
            ("Русский", "ru-RU")
        };
        
        foreach (var (name, code) in languages)
        {
            var item = new ComboBoxItem { Content = name, Tag = code };
            languageComboBox.Items.Add(item);
        }
        languageComboBox.SelectedIndex = 0;
    }

    private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (languageComboBox.SelectedItem is ComboBoxItem item && item.Tag is string langCode)
        {
            LocalizationManager.LoadLanguage(langCode);
        }
    }
    
    private async void RenderEngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_webViewReady && renderEngineComboBox.SelectedItem is ComboBoxItem item && item.Tag is string engineCode)
        {
            try
            {
                await webView.CoreWebView2.ExecuteScriptAsync($"setRenderEngine('{engineCode}')");
                if (!string.IsNullOrEmpty(latexEditor.Text))
                {
                    await RenderLatex(latexEditor.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"切换渲染引擎失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        mainWindow.Title = LocalizationManager.GetString("AppTitle");
        
        // 更新按钮文本
        renderButtonText.Text = LocalizationManager.GetString("RenderButton");
        clearButtonText.Text = LocalizationManager.GetString("ClearButton");
        uploadImageButtonText.Text = LocalizationManager.GetString("UploadImageButton");
        pasteImageButtonText.Text = LocalizationManager.GetString("PasteImageButton");
        naturalLanguageButtonText.Text = LocalizationManager.GetString("NaturalLanguage");
        toNaturalLanguageButtonText.Text = LocalizationManager.GetString("ToNaturalLanguage");
        exportButtonText.Text = LocalizationManager.GetString("Export");
        historyButtonText.Text = LocalizationManager.GetString("HistoryButton");
        helpButtonText.Text = LocalizationManager.GetString("HelpButton");
        
        // 更新标签文本
        languageLabel.Text = LocalizationManager.GetString("Language") + ":";
        renderEngineLabel.Text = LocalizationManager.GetString("RenderEngine") + ":";
        formulaColorLabel.Text = LocalizationManager.GetString("FormulaColor") + ":";
        backgroundColorLabel.Text = LocalizationManager.GetString("BgColor") + ":";
        fontLabel.Text = LocalizationManager.GetString("Font") + ":";
        fontSizeLabel.Text = LocalizationManager.GetString("Size") + ":";
        
        // 更新Expander标题
        basicSymbolsExpander.Header = LocalizationManager.GetString("BasicSymbols");
        fractionsRootsExpander.Header = LocalizationManager.GetString("FractionsAndRoots");
        sumsIntegralsExpander.Header = LocalizationManager.GetString("SumsAndIntegrals");
        greekLettersExpander.Header = LocalizationManager.GetString("GreekLetters");
        bracketsExpander.Header = LocalizationManager.GetString("Brackets");
        setLogicExpander.Header = LocalizationManager.GetString("SetAndLogic");
        matricesExpander.Header = LocalizationManager.GetString("Matrices");
        arrowsExpander.Header = LocalizationManager.GetString("Arrows");
        otherExpander.Header = LocalizationManager.GetString("Other");
        
        // 更新区域标题
        editorTitleTextBlock.Text = LocalizationManager.GetString("LatexCode");
        previewTitleTextBlock.Text = LocalizationManager.GetString("FormulaPreview");
    }

    private void InitializeEditor()
    {
        latexEditor.TextArea.TextView.LineTransformers.Add(new LatexColorizer());
        latexEditor.TextChanged += LatexEditor_TextChanged;
        latexEditor.LostFocus += LatexEditor_LostFocus;
    }

    private void LatexEditor_LostFocus(object sender, RoutedEventArgs e)
    {
        SaveToHistory();
    }

    private void SaveToHistory()
    {
        var currentText = latexEditor.Text.Trim();
        if (!string.IsNullOrEmpty(currentText) && currentText != _lastSavedFormula)
        {
            _history.Insert(0, currentText);
            _lastSavedFormula = currentText;
            if (_history.Count > 50)
            {
                _history.RemoveAt(_history.Count - 1);
            }
        }
    }

    private async void InitializeWebView()
    {
        await webView.EnsureCoreWebView2Async();
        var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LaTeXRenderer.html");
        webView.CoreWebView2.Navigate(htmlPath);
        webView.NavigationCompleted += WebView_NavigationCompleted;
    }

    private async void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        _webViewReady = true;
        await RenderLatex(latexEditor.Text);
        await SetZoom(zoomSlider.Value / 100);
        await ApplyAllStyles();
    }

    private async System.Threading.Tasks.Task ApplyAllStyles()
    {
        if (_webViewReady)
        {
            await SetFormulaColor(GetSelectedColor(formulaColorComboBox));
            await SetBackgroundColor(GetSelectedColor(backgroundColorComboBox));
            await SetFont(GetSelectedFont());
            await SetFontSize(fontSizeSlider.Value);
        }
    }

    private string GetSelectedColor(ComboBox comboBox)
    {
        if (comboBox.SelectedItem is ComboBoxItem item && item.Tag is string color)
        {
            return color;
        }
        return "#000000";
    }

    private string GetSelectedFont()
    {
        if (fontComboBox.SelectedItem is ComboBoxItem item && item.Content is string font)
        {
            return font;
        }
        return "KaTeX";
    }

    private async System.Threading.Tasks.Task SetFormulaColor(string color)
    {
        if (_webViewReady)
        {
            await webView.CoreWebView2.ExecuteScriptAsync($"setFormulaColor('{color}')");
        }
    }

    private async System.Threading.Tasks.Task SetBackgroundColor(string color)
    {
        if (_webViewReady)
        {
            await webView.CoreWebView2.ExecuteScriptAsync($"setBackgroundColor('{color}')");
        }
    }

    private async System.Threading.Tasks.Task SetFont(string font)
    {
        if (_webViewReady)
        {
            await webView.CoreWebView2.ExecuteScriptAsync($"setFont('{font}')");
        }
    }

    private async System.Threading.Tasks.Task SetFontSize(double size)
    {
        if (_webViewReady)
        {
            await webView.CoreWebView2.ExecuteScriptAsync($"setFontSize({size})");
            fontSizeDisplay.Text = $"{(int)size}";
        }
    }

    private async System.Threading.Tasks.Task RenderLatex(string latex)
    {
        if (_webViewReady)
        {
            var escapedLatex = latex.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
            await webView.CoreWebView2.ExecuteScriptAsync($"renderLatex(\"{escapedLatex}\")");
        }
    }

    private async System.Threading.Tasks.Task SetZoom(double scale)
    {
        if (_webViewReady)
        {
            await webView.CoreWebView2.ExecuteScriptAsync($"setScale({scale})");
            zoomDisplay.Text = $"{(int)(scale * 100)}%";
        }
    }

    private async void RenderButton_Click(object sender, RoutedEventArgs e)
    {
        await RenderLatex(latexEditor.Text);
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        latexEditor.Text = string.Empty;
    }

    private async void LatexEditor_TextChanged(object? sender, EventArgs e)
    {
        await RenderLatex(latexEditor.Text);
    }

    private void InsertTemplate_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string template)
        {
            string decodedTemplate = System.Net.WebUtility.HtmlDecode(template);
            InsertText(decodedTemplate);
        }
    }

    private void InsertText(string text)
    {
        var document = latexEditor.Document;
        var caretOffset = latexEditor.CaretOffset;
        
        document.Insert(caretOffset, text);
        latexEditor.CaretOffset = caretOffset + text.Length;
        
        latexEditor.Focus();
    }

    private void ZoomInButton_Click(object sender, RoutedEventArgs e)
    {
        if (zoomSlider.Value < zoomSlider.Maximum)
        {
            zoomSlider.Value += 10;
        }
    }

    private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
    {
        if (zoomSlider.Value > zoomSlider.Minimum)
        {
            zoomSlider.Value -= 10;
        }
    }

    private async void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        await SetZoom(zoomSlider.Value / 100);
    }

    private async void FormulaColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await SetFormulaColor(GetSelectedColor(formulaColorComboBox));
    }

    private async void BackgroundColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await SetBackgroundColor(GetSelectedColor(backgroundColorComboBox));
    }

    private async void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await SetFont(GetSelectedFont());
    }

    private async void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        await SetFontSize(fontSizeSlider.Value);
    }

    private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if ((e.Key == System.Windows.Input.Key.V) && (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control))
        {
            ProcessPastedImage();
        }
    }

    private async void UploadImageButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files (*.*)|*.*",
            Title = "Select an image containing a formula"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            await RecognizeFromImageAsync(openFileDialog.FileName);
        }
    }

    private void PasteImageButton_Click(object sender, RoutedEventArgs e)
    {
        ProcessPastedImage();
    }

    private async void ProcessPastedImage()
    {
        var dataObject = Clipboard.GetDataObject();
        
        if (dataObject != null && dataObject.GetDataPresent(System.Windows.DataFormats.Bitmap))
        {
            if (dataObject.GetData(System.Windows.DataFormats.Bitmap) is BitmapSource bitmap)
            {
                await RecognizeFromBitmapAsync(bitmap);
            }
        }
        else if (dataObject != null && dataObject.GetDataPresent(System.Windows.DataFormats.FileDrop))
        {
            if (dataObject.GetData(System.Windows.DataFormats.FileDrop) is string[] filePaths && filePaths.Length > 0)
            {
                await RecognizeFromImageAsync(filePaths[0]);
            }
        }
    }

    private async System.Threading.Tasks.Task RecognizeFromImageAsync(string imagePath)
    {
        if (_isRecognizing) return;
        
        _isRecognizing = true;
        
        try
        {
            var result = await _formulaRecognizer.RecognizeFromImageAsync(imagePath);
            if (!string.IsNullOrEmpty(result))
            {
                latexEditor.Text = result;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isRecognizing = false;
        }
    }

    private async System.Threading.Tasks.Task RecognizeFromBitmapAsync(BitmapSource bitmap)
    {
        if (_isRecognizing) return;
        
        _isRecognizing = true;
        
        try
        {
            var result = await _formulaRecognizer.RecognizeFromBitmapAsync(bitmap);
            if (!string.IsNullOrEmpty(result))
            {
                latexEditor.Text = result;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isRecognizing = false;
        }
    }

    private async void NaturalLanguageButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NaturalLanguageInputWindow();
        if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.InputText))
        {
            await ConvertNaturalLanguageAsync(dialog.InputText);
        }
    }

    private async System.Threading.Tasks.Task ConvertNaturalLanguageAsync(string description)
    {
        if (_isConverting) return;
        
        _isConverting = true;
        
        try
        {
            var result = await _naturalLanguageConverter.ConvertToLatexAsync(description);
            if (!string.IsNullOrEmpty(result))
            {
                latexEditor.Text = result;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isConverting = false;
        }
    }

    private async void ToNaturalLanguageButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isConverting) return;
        
        _isConverting = true;
        
        try
        {
            var result = await _latexToNaturalLanguageConverter.ConvertToNaturalLanguageAsync(latexEditor.Text);
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Formula Description", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isConverting = false;
        }
    }

    private void HistoryButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new HistoryWindow(_history);
        if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.SelectedFormula))
        {
            latexEditor.Text = dialog.SelectedFormula;
        }
    }

    private void HelpButton_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow();
        helpWindow.ShowDialog();
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|SVG Image (*.svg)|*.svg|PDF Document (*.pdf)|*.pdf|EPS File (*.eps)|*.eps",
            Title = "Export Formula"
        };

        if (dialog.ShowDialog() == true)
        {
            ExportFormula(dialog.FileName);
        }
    }

    private async void ExportFormula(string filePath)
    {
        try
        {
            var extension = Path.GetExtension(filePath).ToLower();
            if (_webViewReady)
            {
                // First, set the page to export mode (remove padding, set white background)
                await webView.CoreWebView2.ExecuteScriptAsync(@"
                    document.body.style.margin = '0';
                    document.body.style.padding = '20px';
                    document.body.style.backgroundColor = 'white';
                    const output = document.getElementById('latex-output');
                    if (output) {
                        output.style.transform = '';
                        output.style.display = 'inline-block';
                    }
                ");
                
                // Wait a bit for styles to apply
                await Task.Delay(200);
                
                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    // Capture preview
                    using var stream = new MemoryStream();
                    await webView.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, stream);
                    stream.Position = 0;
                    
                    using var bitmap = SKBitmap.Decode(stream);
                    
                    if (extension == ".jpg" || extension == ".jpeg")
                    {
                        // Convert to JPEG
                        using var jpegStream = new FileStream(filePath, FileMode.Create);
                        bitmap.Encode(jpegStream, SKEncodedImageFormat.Jpeg, 95);
                    }
                    else
                    {
                        // Save as PNG
                        using var pngStream = new FileStream(filePath, FileMode.Create);
                        bitmap.Encode(pngStream, SKEncodedImageFormat.Png, 100);
                    }
                    
                    MessageBox.Show("Exported successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (extension == ".pdf")
                {
                    // Capture preview and convert to PDF
                    using var stream = new MemoryStream();
                    await webView.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, stream);
                    stream.Position = 0;
                    
                    using var bitmap = SKBitmap.Decode(stream);
                    
                    using var pdfStream = new FileStream(filePath, FileMode.Create);
                    using var document = SKDocument.CreatePdf(pdfStream, new SKDocumentPdfMetadata());
                    using var pdfCanvas = document.BeginPage(bitmap.Width, bitmap.Height);
                    
                    pdfCanvas.DrawBitmap(bitmap, 0, 0);
                    document.EndPage();
                    
                    MessageBox.Show("Exported successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (extension == ".svg")
                {
                    // For SVG, we'll create a simple SVG with foreignObject
                    var htmlResult = await webView.CoreWebView2.ExecuteScriptAsync(@"
                        (function() {
                            const output = document.getElementById('latex-output');
                            if (!output) return JSON.stringify({ success: false });
                            const width = output.offsetWidth + 40;
                            const height = output.offsetHeight + 40;
                            const html = output.outerHTML;
                            return JSON.stringify({ 
                                success: true, 
                                width: width, 
                                height: height, 
                                html: html 
                            });
                        })();
                    ");
                    
                    htmlResult = htmlResult.Trim('"').Replace("\\\"", "\"").Replace("\\\\", "\\");
                    var jsonDoc = JsonDocument.Parse(htmlResult);
                    if (jsonDoc.RootElement.TryGetProperty("success", out var success) && success.GetBoolean())
                    {
                        var width = jsonDoc.RootElement.GetProperty("width").GetInt32();
                        var height = jsonDoc.RootElement.GetProperty("height").GetInt32();
                        var html = jsonDoc.RootElement.GetProperty("html").GetString();
                        
                        var svg = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" width=""{width}"" height=""{height}"" viewBox=""0 0 {width} {height}"">
    <rect width=""100%"" height=""100%"" fill=""white""/>
    <foreignObject x=""20"" y=""20"" width=""{width-40}"" height=""{height-40}"">
        {html}
    </foreignObject>
</svg>";
                        
                        File.WriteAllText(filePath, svg);
                        MessageBox.Show("Exported successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("SVG export failed, falling back to PNG", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        // Fallback to PNG
                        using var stream = new MemoryStream();
                        await webView.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, stream);
                        stream.Position = 0;
                        File.WriteAllBytes(Path.ChangeExtension(filePath, ".png"), stream.ToArray());
                    }
                }
                else if (extension == ".eps")
                {
                    MessageBox.Show("EPS export is not supported. Please export as PNG, JPG, SVG, or PDF instead.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                // Restore original styles
                await webView.CoreWebView2.ExecuteScriptAsync(@"
                    document.body.style.padding = '20px';
                    document.body.style.backgroundColor = 'white';
                    const output = document.getElementById('latex-output');
                    if (output) {
                        output.style.display = 'block';
                    }
                ");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting: {ex.Message}\nStack Trace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
