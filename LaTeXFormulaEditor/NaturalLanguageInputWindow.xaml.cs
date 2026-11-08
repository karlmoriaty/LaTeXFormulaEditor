
using System.Windows;

namespace LaTeXFormulaEditor
{
    public partial class NaturalLanguageInputWindow : Window
    {
        public string? InputText { get; private set; }
        
        public NaturalLanguageInputWindow()
        {
            InitializeComponent();
        }
        
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = inputTextBox.Text;
            DialogResult = true;
            Close();
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
