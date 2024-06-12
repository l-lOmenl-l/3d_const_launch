using System;
using System.Diagnostics;
using System.IO;
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
            var loading = new Loading();
            loading.Show();

            InitializeComponent();

            Config.CheckConf(this);

            LabelTest.Visibility = Config.GetIp() == "https://3d.e-1.ru:8000/sync" ? Visibility.Collapsed : Visibility.Visible;


            Config.Init(this);
            CheckConnect();
            AppShortcutToDesktop();
            Checklauncher();

            Init();
            loading.Close();
        }


        private static void AppShortcutToDesktop()
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (!System.IO.File.Exists(deskDir + "3dconst_launch"))
            {
                var name = Config.GetIp() == "https://3d.e-1.ru:8000/sync" ? "3dconst_launch" : "3dconst_launch_test";

                using (var writer = new StreamWriter(deskDir + "\\" + name + ".url"))
                {
                    string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=file:///" + app);
                    writer.WriteLine("IconIndex=0");
                    string icon = app.Replace('\\', '/');
                    writer.WriteLine("IconFile=" + icon);
                }
            }
        }



        private static void CheckConnect()
        {
            var req = (HttpWebRequest)WebRequest.Create(Config.GetIp() + "/status");
            req.Method = "GET";
            req.Headers.Add("Authorization", Config.GetAuth());
            req.Proxy = null;
            try
            {
                var res = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                var result = MessageBox.Show("Ошибка соединения с сервером, попробуйте запустить еще раз, через некоторое время.",
                    "Ошибка соединения", MessageBoxButton.OK, MessageBoxImage.Error);

                if (result == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
            }
        }


        private static void Checklauncher()
        {
            var path = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            var md5 = MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(System.IO.File.ReadAllBytes(path ?? string.Empty))).Replace("-", "").ToLower();

            var req = (HttpWebRequest)WebRequest.Create(Config.GetIp() + "/check_launcher");  
            req.Method = "GET";
            req.Headers.Add("Authorization", Config.GetAuth());
            req.Proxy = null;


            var res = (HttpWebResponse)req.GetResponse();
            var json = JsonSerializer.Deserialize<string>(res.GetResponseStream() ?? throw new InvalidOperationException());
            
            if (hash == json) return;
            
            /*var proc = new Process();
            proc.StartInfo.FileName = path?.Remove(path.LastIndexOf("\\", StringComparison.Ordinal))+"/update_launcher.exe";
            proc.Start();
            Application.Current.Shutdown();*/
        }


        private static async void Init()
        {
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

        public async void ChangeMessageAsync(string msg)
        {
            await Task.Run(() => Dispatcher.Invoke(() => label_message.Content = msg));
        }


        public async void BtnChange(string value)
        {
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.IsEnabled = true));
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.Content = value));
        }


        public void AppClose()
        {
            Dispatcher.Invoke(() => Application.Current.Shutdown());
        }

        private async void Btn_download_Click(object sender, RoutedEventArgs e)
        {
            switch (Btn_download.Content)
            {
                case "Загрузить":
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => Download.DownloadConstruct(this, FilesData.GetServerPath()));
                    break;

                case "Обновление":
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => Download.DownloadConstruct(this, FilesData.CheckDiffFiles()));
                    break;

                case "Запустить":
                    var proc = new Process();
                    proc.StartInfo.FileName = Config.GetPath() + "/3dconst.exe";
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

        public void ChangeMessage(string msg) => label_message.Content = msg;


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void btnSettings_Click(object sender, RoutedEventArgs e) 
        {
            var settingWindow = new Settings();
            settingWindow.Show();
        }
    }
}
