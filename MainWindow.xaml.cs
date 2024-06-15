using System.Windows;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartFCPU(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow("安全PLC")
            {
                Left = this.Left,
                Top = this.Top
            };
            examWindow.Show();
            this.Close();
        }

        private void StartProgRealy(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow("可编程安全继电器")
            {
                Left = this.Left,
                Top = this.Top
            };
            examWindow.Show();
            this.Close();
        }

        private void StartRealy(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow("安全继电器")
            {
                Left = this.Left,
                Top = this.Top
            };
            examWindow.Show();
            this.Close();
        }
    }
}