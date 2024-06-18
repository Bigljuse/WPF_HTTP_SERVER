using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;
using WPF_HTTP_SERVER.HTTP;
using SaveableSettings = WPF_HTTP_SERVER.Settings.Settings;

namespace WPF_HTTP_SERVER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Button_StopServer_Click(sender, new RoutedEventArgs());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_PythondllPath.Text = SaveableSettings.Default.PythondllPath;
            TextBox_ServerIp.Text = SaveableSettings.Default.Ip;
            TextBox_ServerPort.Text = SaveableSettings.Default.Port;
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (Server.Status == Status.Listening)
                return;

            if (TextBox_ServerIp.Text == string.Empty ||
                TextBox_ServerPort.Text == string.Empty)
                return;

            string ip = TextBox_ServerIp.Text;
            string port = TextBox_ServerPort.Text;
            SaveableSettings.Default.Ip = TextBox_ServerIp.Text;
            SaveableSettings.Default.Port = TextBox_ServerPort.Text;
            SaveableSettings.Default.Save();
            Server.SetServer(ip, port);
            Server.Start();

            SetRectangleStatusFill(Server.Status);
            Button_Start.IsEnabled = false;
            Button_Restart.IsEnabled = true;
        }

        private async void Button_Restart_Click(object sender, RoutedEventArgs e)
        {
            Button_StopServer_Click(sender, e);
            Button_Start.IsEnabled = false;
            await Task.Delay(1000);
            Button_Start_Click(sender, e);
        }

        private void Button_StopServer_Click(object sender, RoutedEventArgs e)
        {
            Server.Stop();
            SetRectangleStatusFill(Server.Status);
            Button_Start.IsEnabled = true;
            Button_Restart.IsEnabled = false;
        }

        private void Button_OpenDialogWindow_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() is true)
            {
                var filePath = openFileDialog.FileName;
                TextBox_PythondllPath.Text = filePath;
                SaveableSettings.Default.PythondllPath = filePath;
                SaveableSettings.Default.Save();
            }
        }
    }
    public partial class MainWindow
    {
        private void SetRectangleStatusFill(Status status)
        {
            if (status == Status.Listening)
                Rectangle_Status.Fill = Brushes.Green;
            if (status == Status.Error)
                Rectangle_Status.Fill = Brushes.Red;
            if (status == Status.Off)
                Rectangle_Status.Fill = Brushes.Gray;
        }
    }
}