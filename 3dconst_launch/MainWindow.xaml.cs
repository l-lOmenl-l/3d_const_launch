using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            checklauncher();


            init();
        }

        public void checklauncher()
        {
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;
            MD5 md5 = MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(System.IO.File.ReadAllBytes(path))).Replace("-", "").ToLower();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(config.conf.ip_server + "/check_launcher");
            req.Method = "GET";
            req.Headers.Add("Authorization", download.auth());
            req.Proxy = null;

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            var myjson = JsonSerializer.Deserialize<string>(res.GetResponseStream());
            if (hash != myjson)
            {
                
                Process proc = new Process();
                proc.StartInfo.FileName = path.Remove(path.LastIndexOf("\\"))+"/update_launcher.exe";
                proc.Start();
                System.Windows.Application.Current.Shutdown();
            }

        }


        public async void init()
        {
            await Task.Run(() => config.init(this));

            //CheckDiffPath();
        }

        public async void ChangeProgressBatAsync(float value)
        {
            await Task.Run(() => Dispatcher.Invoke(() => ProgressUpload.Value = value));
        }

        public async void ChangeProgressBarVisibility(Visibility value)
        {
            await Task.Run(() => Dispatcher.Invoke(() => ProgressUpload.Visibility = value));
        }

        public async void changeMessageAsync(string msg)
        {
            await Task.Run(() => Dispatcher.Invoke(() => label_message.Content = msg));
        }


        public async void btnChange(string value)
        {
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.IsEnabled = true));
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.Content = value));
        }


        

        private async void Btn_download_Click(object sender, RoutedEventArgs e)
        {
            switch (Btn_download.Content)
            {
                case "Загрузить":
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => download.DownloadConstruct(this, FilesData.GetServerPath()));
                    break;

                case "Обновление":
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => download.DownloadConstruct(this, FilesData.checkDiffFiles()));
                    break;

                case "Запустить":
                    Process proc = new Process();
                    proc.StartInfo.FileName = config.conf.Path_const + "/3dconst.exe";
                    proc.Start();
                    break;
            }



        }

        public void const_actual()
        {
            label_message.Content = "Actual";
        }

        public void const_update()
        {
            label_message.Content = "update";
        }

        public void const_download()
        {
            label_message.Content = "Готов к загрузке";
            Btn_download.Content = "Загрузить";
        }

        public void ChangeMessage(string msg)
        {
            label_message.Content = msg;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }



    }
}
