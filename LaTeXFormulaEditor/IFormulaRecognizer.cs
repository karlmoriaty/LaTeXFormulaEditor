using System.IO;
using System.Threading.Tasks;

namespace LaTeXFormulaEditor;

public interface IFormulaRecognizer
{
    Task<string> RecognizeFromImageAsync(string imagePath);
    Task<string> RecognizeFromImageStreamAsync(Stream imageStream);
    Task<string> RecognizeFromBitmapAsync(System.Windows.Media.Imaging.BitmapSource bitmap);
}
