using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using IWshRuntimeLibrary;

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
            Loading loading = new Loading();
            loading.Show();

            InitializeComponent();

            config.checkConf(this);

            if (config.getIP() == "https://3d.e-1.ru:8000/sync")
            {
                LabelTest.Visibility = Visibility.Collapsed;
            } 
            else 
            { 
                LabelTest.Visibility = Visibility.Visible;
            }


            config.init(this);
            checkConnect();
            appShortcutToDesktop();
            checklauncher();

            init();
            loading.Close();
        }


        private void appShortcutToDesktop()
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (!System.IO.File.Exists(deskDir + "3dconst_launch"))
            {
                string name;
                if (config.getIP() == "https://3d.e-1.ru:8000/sync")
                {
                   name = "3dconst_launch";
                }
                else
                {
                   name = "3dconst_launch_test";
                }

                using (StreamWriter writer = new StreamWriter(deskDir + "\\" + name + ".url"))
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



        private static void checkConnect()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(config.getIP() + "/status");
            req.Method = "GET";
            req.Headers.Add("Authorization", config.getAuth());
            req.Proxy = null;
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
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


        public void checklauncher()
        {
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;
            MD5 md5 = MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(System.IO.File.ReadAllBytes(path))).Replace("-", "").ToLower();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(config.getIP() + "/check_launcher");  
            req.Method = "GET";
            req.Headers.Add("Authorization", config.getAuth());
            req.Proxy = null;


            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            var myjson = JsonSerializer.Deserialize<string>(res.GetResponseStream());
            if (hash != myjson)
            {/*
                Process proc = new Process();
                proc.StartInfo.FileName = path.Remove(path.LastIndexOf("\\"))+"/update_launcher.exe";
                proc.Start();
                Application.Current.Shutdown();
                */
            }
        }


        public async void init()
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

        public async void changeMessageAsync(string msg)
        {
            await Task.Run(() => Dispatcher.Invoke(() => label_message.Content = msg));
        }


        public async void btnChange(string value)
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
                    await Task.Run(() => download.DownloadConstruct(this, FilesData.GetServerPath()));
                    break;

                case "Обновление":
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => download.DownloadConstruct(this, FilesData.checkDiffFiles()));
                    break;

                case "Запустить":
                    Process proc = new Process();
                    proc.StartInfo.FileName = config.getPath() + "/3dconst.exe";
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
            Settings SettingWindow = new Settings();
            SettingWindow.Show();
        }
    }
}
