namespace LaTeXFormulaEditor;

public interface ILatexToNaturalLanguageConverter
{
    Task<string> ConvertToNaturalLanguageAsync(string latexCode);
}
