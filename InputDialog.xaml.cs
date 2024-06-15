using System.Windows;

namespace YourNamespace
{
    public partial class InputDialog : Window
    {
        public string ResponseText { get; private set; }

        public InputDialog(string prompt)
        {
            InitializeComponent();
            this.Title = prompt;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResponseText = InputTextBox.Text;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}