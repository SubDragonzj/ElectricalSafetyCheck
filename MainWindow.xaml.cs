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
            var examWindow = new ExamWindow("语文");
            examWindow.Show();
            this.Close();
        }

        private void StartMathExam(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow("数学");
            examWindow.Show();
            this.Close();
        }

        private void StartEnglishExam(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow("英语");
            examWindow.Show();
            this.Close();
        }
    }
}