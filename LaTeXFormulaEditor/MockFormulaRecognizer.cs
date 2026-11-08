using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LaTeXFormulaEditor;

public class MockFormulaRecognizer : IFormulaRecognizer
{
    private static readonly Random _random = new Random();
    
    private static readonly string[] _mockFormulas = new string[]
    {
        @"x = \frac{-b \pm \sqrt{b^2 - 4ac}}{2a}",
        @"\int_{- \infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}",
        @"e^{i \pi} + 1 = 0",
        @"F = G \frac{m_1 m_2}{r^2}",
        @"\sum_{n=1}^{\infty} \frac{1}{n^2} = \frac{\pi^2}{6}",
        @"\lim_{x \to \infty} \left(1 + \frac{1}{x}\right)^x = e",
        @"\frac{d}{dx} \left(x^n\right) = n x^{n-1}",
        @"A = \begin{pmatrix} a & b \\ c & d \end{pmatrix}",
        @"\nabla \times \mathbf{E} = - \frac{\partial \mathbf{B}}{\partial t}",
        @"\int_a^b f(x) dx = F(b) - F(a)"
    };

    public async Task<string> RecognizeFromImageAsync(string imagePath)
    {
        return await SimulateRecognitionAsync();
    }

    public async Task<string> RecognizeFromImageStreamAsync(Stream imageStream)
    {
        return await SimulateRecognitionAsync();
    }

    public async Task<string> RecognizeFromBitmapAsync(BitmapSource bitmap)
    {
        return await SimulateRecognitionAsync();
    }

    private async Task<string> SimulateRecognitionAsync()
    {
        var delay = _random.Next(500, 2000);
        await Task.Delay(delay);
        
        var index = _random.Next(_mockFormulas.Length);
        return _mockFormulas[index];
    }
}
