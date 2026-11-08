using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LaTeXFormulaEditor;

public static class LocalizationManager
{
    private static Dictionary<string, string> _currentResources = new Dictionary<string, string>();
    private static string _currentLanguage = "zh-CN";

    public static event EventHandler? LanguageChanged;

    public static string CurrentLanguage => _currentLanguage;

    public static void Initialize()
    {
        LoadLanguage(_currentLanguage);
    }

    public static void LoadLanguage(string languageCode)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var resourcePath = Path.Combine(basePath, "Resources", $"{languageCode}.json");
        
        if (!File.Exists(resourcePath))
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDir = Path.GetDirectoryName(assemblyLocation);
            if (!string.IsNullOrEmpty(assemblyDir))
            {
                resourcePath = Path.Combine(assemblyDir, "Resources", $"{languageCode}.json");
            }
        }

        if (!File.Exists(resourcePath))
        {
            if (languageCode != "zh-CN")
            {
                LoadLanguage("zh-CN");
            }
            return;
        }
        
        try
        {
            var json = File.ReadAllText(resourcePath);
            _currentResources = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            _currentLanguage = languageCode;
            OnLanguageChanged();
        }
        catch
        {
            if (languageCode != "zh-CN")
            {
                LoadLanguage("zh-CN");
            }
        }
    }

    public static string GetString(string key)
    {
        return _currentResources.TryGetValue(key, out var value) ? value : key;
    }

    private static void OnLanguageChanged()
    {
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
