using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LaTeXFormulaEditor;

public partial class HistoryWindow : Window
{
    public string? SelectedFormula { get; private set; }
    private List<string> _history;
    
    public HistoryWindow()
    {
        InitializeComponent();
        _history = new List<string>();
        Loaded += HistoryWindow_Loaded;
        LocalizationManager.LanguageChanged += OnLanguageChanged;
    }
    
    public HistoryWindow(List<string> history) : this()
    {
        _history = history ?? new List<string>();
    }

    private void HistoryWindow_Loaded(object sender, RoutedEventArgs e)
    {
        LoadHistory(_history);
        UpdateUI();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        Title = "History";
    }

    public void LoadHistory(List<string> history)
    {
        historyListBox.Items.Clear();
        foreach (var item in history)
        {
            historyListBox.Items.Add(item);
        }
    }

    private void HistoryListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (historyListBox.SelectedItem is string selectedItem)
        {
            SelectedFormula = selectedItem;
            DialogResult = true;
            Close();
        }
    }

    private void InsertButton_Click(object sender, RoutedEventArgs e)
    {
        if (historyListBox.SelectedItem is string selectedItem)
        {
            SelectedFormula = selectedItem;
            DialogResult = true;
            Close();
        }
    }

    private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        historyListBox.Items.Clear();
        _history.Clear();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
