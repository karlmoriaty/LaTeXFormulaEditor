using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Windows.Media;

namespace LaTeXFormulaEditor;

public class LatexColorizer : DocumentColorizingTransformer
{
    private static readonly Brush CommandBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));
    private static readonly Brush MathBrush = new SolidColorBrush(Color.FromRgb(163, 21, 21));
    private static readonly Brush CommentBrush = new SolidColorBrush(Color.FromRgb(0, 128, 0));
    private static readonly Brush BracketBrush = new SolidColorBrush(Color.FromRgb(128, 0, 128));
    private static readonly Brush SubSupBrush = new SolidColorBrush(Color.FromRgb(204, 102, 0));

    static LatexColorizer()
    {
        CommandBrush.Freeze();
        MathBrush.Freeze();
        CommentBrush.Freeze();
        BracketBrush.Freeze();
        SubSupBrush.Freeze();
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        var text = CurrentContext.Document.GetText(line);
        var startOffset = line.Offset;
        
        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '%')
            {
                ChangeLinePart(startOffset + i, startOffset + text.Length, element => element.TextRunProperties.SetForegroundBrush(CommentBrush));
                break;
            }
            else if (text[i] == '\\')
            {
                int j = i + 1;
                while (j < text.Length && char.IsLetter(text[j]))
                    j++;
                
                if (j > i + 1)
                {
                    ChangeLinePart(startOffset + i, startOffset + j, element => element.TextRunProperties.SetForegroundBrush(CommandBrush));
                }
                i = j;
            }
            else if (text[i] == '{' || text[i] == '}' || text[i] == '(' || text[i] == ')' || text[i] == '[' || text[i] == ']')
            {
                ChangeLinePart(startOffset + i, startOffset + i + 1, element => element.TextRunProperties.SetForegroundBrush(BracketBrush));
                i++;
            }
            else if (text[i] == '_' || text[i] == '^')
            {
                ChangeLinePart(startOffset + i, startOffset + i + 1, element => element.TextRunProperties.SetForegroundBrush(SubSupBrush));
                i++;
            }
            else if (char.IsDigit(text[i]))
            {
                int j = i + 1;
                while (j < text.Length && char.IsDigit(text[j]))
                    j++;
                ChangeLinePart(startOffset + i, startOffset + j, element => element.TextRunProperties.SetForegroundBrush(MathBrush));
                i = j;
            }
            else
            {
                i++;
            }
        }
    }
}
