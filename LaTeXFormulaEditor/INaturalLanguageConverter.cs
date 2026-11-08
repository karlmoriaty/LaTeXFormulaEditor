namespace LaTeXFormulaEditor;

public interface INaturalLanguageConverter
{
    Task<string> ConvertToLatexAsync(string naturalLanguageDescription);
}
