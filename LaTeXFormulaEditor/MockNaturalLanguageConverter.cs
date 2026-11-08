namespace LaTeXFormulaEditor;

public class MockNaturalLanguageConverter : INaturalLanguageConverter
{
    public async Task<string> ConvertToLatexAsync(string naturalLanguageDescription)
    {
        await Task.Delay(500);
        
        var description = naturalLanguageDescription.ToLower();
        
        if (description.Contains("积分"))
        {
            return "\\int_{a}^{b} f(x) dx";
        }
        if (description.Contains("求和") || description.Contains("sum"))
        {
            return "\\sum_{n=1}^{\\infty} a_n";
        }
        if (description.Contains("平方") || description.Contains("power"))
        {
            return "x^2 + y^2 = z^2";
        }
        if (description.Contains("分数") || description.Contains("fraction"))
        {
            return "\\frac{a}{b}";
        }
        if (description.Contains("导数") || description.Contains("derivative"))
        {
            return "\\frac{d}{dx} f(x)";
        }
        if (description.Contains("极限") || description.Contains("limit"))
        {
            return "\\lim_{x \\to \\infty} f(x)";
        }
        
        return "E = mc^2";
    }
}
