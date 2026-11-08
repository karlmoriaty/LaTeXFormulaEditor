using System.Windows;

namespace LaTeXFormulaEditor;

public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();
        Loaded += HelpWindow_Loaded;
        LocalizationManager.LanguageChanged += OnLanguageChanged;
    }

    private void HelpWindow_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateUI();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        Title = LocalizationManager.GetString("HelpTitle");
        helpTitle.Text = LocalizationManager.GetString("HelpTitle");
        section1Title.Text = LocalizationManager.GetString("QuickStart");
        section1Content.Text = LocalizationManager.GetString("QuickStartContent");
        section2Title.Text = LocalizationManager.GetString("ToolbarGuide");
        section2Content.Text = LocalizationManager.GetString("ToolbarGuideContent");
        section3Title.Text = LocalizationManager.GetString("StyleSettings");
        section3Content.Text = LocalizationManager.GetString("StyleSettingsContent");
        section4Title.Text = LocalizationManager.GetString("ImageRecognition");
        section4Content.Text = LocalizationManager.GetString("ImageRecognitionContent");
        section5Title.Text = LocalizationManager.GetString("Shortcuts");
        section5Content.Text = LocalizationManager.GetString("ShortcutsContent");
        section6Title.Text = LocalizationManager.GetString("History");
        section6Content.Text = LocalizationManager.GetString("HistoryContent");
        closeButton.Content = LocalizationManager.GetString("Close");
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
