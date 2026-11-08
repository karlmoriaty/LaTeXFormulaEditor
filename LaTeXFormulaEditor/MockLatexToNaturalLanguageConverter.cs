namespace LaTeXFormulaEditor;

public class MockLatexToNaturalLanguageConverter : ILatexToNaturalLanguageConverter
{
    public async Task<string> ConvertToNaturalLanguageAsync(string latexCode)
    {
        await Task.Delay(500);
        
        var code = latexCode.ToLower();
        
        if (code.Contains("\\int"))
        {
            return "这是一个积分公式，表示函数f(x)在区间[a, b]上的定积分";
        }
        if (code.Contains("\\sum"))
        {
            return "这是一个求和公式，表示从n=1到无穷大的a_n的累加和";
        }
        if (code.Contains("\\frac"))
        {
            return "这是一个分数公式，表示a除以b";
        }
        if (code.Contains("\\lim"))
        {
            return "这是一个极限公式，表示当x趋近于无穷大时f(x)的极限";
        }
        if (code.Contains("\\sqrt"))
        {
            return "这是一个平方根公式";
        }
        
        return "这是一个数学公式";
    }
}
