using System.Windows;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartChineseExam(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow("安全PLC");
            examWindow.Show();
            this.Close();
        }

        private void StartMathExam(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow("可编程安全继电器");
            examWindow.Show();
            this.Close();
        }

        private void StartEnglishExam(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow("安全继电器");
            examWindow.Show();
            this.Close();
        }
    }
}