using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {
        public MainWindow(string ip)
        {
            InitializeComponent();
            Config.ip = ip;
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, EventArgs e)
        {
           await Init();
        }

        public Circular CreateLoadingCircular(string msg)
        {
            SecondGrid.Effect = new BlurEffect { Radius = 10 };
            Circular circular = Spawn.CircularAdd(msg);
            MainGrid.RegisterName(circular.Name, circular);
            MainGrid.Children.Add(circular);
            MainGrid.IsEnabled = false;
            return circular;
        }

        public void DestroyLoadingCircular()
        {
            Circular circular = (Circular)MainGrid.FindName("dynamicCircular");
            MainGrid.UnregisterName(circular.Name);
            MainGrid.Children.Remove(circular);
            MainGrid.IsEnabled = true;
            SecondGrid.Effect = new BlurEffect { Radius = 0 };
        }

        public void ChangeMsgLoadingCircular(string msg)
        {
            Circular circular = (Circular)MainGrid.FindName("dynamicCircular");
            circular.ChangeMessage(msg);
        }

        public async void ChangeMsgLoadingCircularAsync(string msg)
        {
            await Task.Run(() => Dispatcher.Invoke(() => ChangeMsgLoadingCircular(msg)));
        }



        private Task Init()
        {
            CreateLoadingCircular("Инициализация");
           
            Alias.AliasIp.TryGetValue(Config.ip, out var outTemp);
            LabelTest.Content = outTemp;

            AppShortcutToDesktop();
            Config.Init(this);

            ChangeMsgLoadingCircular("Проверка обновлений лаунчера");
            Checklauncher();

            /*
            newcircular.ChangeMessage("Проверка обновлений конструктора");
            FilesData.CheckDiffFiles();
            */
            DestroyLoadingCircular();
            return Task.CompletedTask;
        }

        private static void AppShortcutToDesktop()
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (!System.IO.File.Exists(deskDir + "3dconst_launch"))
            {
                using (var writer = new StreamWriter(deskDir + "\\3dconst_launch.url"))
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

        

        private void Checklauncher()
        {
            var path = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            var md5 = MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(System.IO.File.ReadAllBytes(path ?? string.Empty))).Replace("-", "").ToLower();

            var req = (HttpWebRequest)WebRequest.Create(Config.GetIp() + "/sync" + "/check_launcher");  
            req.Method = "GET";
            req.Headers.Add("Authorization", Config.GetAuth());
            req.Proxy = null;


            var res = (HttpWebResponse)req.GetResponse();
            var json = JsonSerializer.Deserialize<string>(res.GetResponseStream() ?? throw new InvalidOperationException());
            
            if (hash == json) return;

            SP_Center.Children.Add(Spawn.InfoLabelAdd("Доступно обновление лаунчера"));
            var btn_param = new Spawn.parametersButton
            {
                msg = "Обновить лаунчер",
                heigth = 35,
                width = 135,
                FontSize = 14,
                horizontalAligment = HorizontalAlignment.Center,
                verticalAlignment = VerticalAlignment.Center,
                RoutedEvent = DownloadLauncher_Click
            };
            Button btn = Spawn.ButtonAdd(btn_param);
            SP_Center.Children.Add(btn);
            btn.Style = (Style)FindResource("ButtonSettings");
            btn.Template = (ControlTemplate)FindResource("CornerButton");
            btn.DataContext = "9";
            /*
            var proc = new Process();
            proc.StartInfo.FileName = path?.Remove(path.LastIndexOf("\\", StringComparison.Ordinal))+"/update_launcher.exe";
            proc.Start();
            Application.Current.Shutdown();
            */
        }

        private async void DownloadLauncher_Click(object sender, RoutedEventArgs e)
        {
            CreateLoadingCircular("Загрузка лаунчера");
            await Download.DownloadLauncher();
            ChangeMsgLoadingCircular("Через 3 секунды лаунчер будет перезапущен.");
            File.Copy(".\\temp\\3dconst_launch.exe", ".\\3dconst_launch.exe.temp");
            Thread.Sleep(3000);
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.Arguments = $"/c TIMEOUT /T 1 /NOBREAK && del 3dconst_launch.exe && move 3dconst_launch.exe.temp 3dconst_launch.exe && start 3dconst_launch.exe";
            await Task.Run(() => Dispatcher.Invoke(() => Process.Start(psi)));
            AppClose();
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
            await Task.Run(() => Dispatcher.Invoke(() => label_message.Text = msg));
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
                    CreateLoadingCircular("Скачивание");
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => Download.DownloadConstruct(this, FilesData.GetServerPath()));
                    DestroyLoadingCircular();
                    break;

                case "Обновление":
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => Download.DownloadConstruct(this, FilesData.CheckDiffFiles()));
                    break;

                case "Запустить":
                    string parameters = "-url="+Config.ip;

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        //FileName = Config.GetPath() + "/3dconst.exe",
                        
                        FileName = "C:\\Soft\\3dconstLocal\\3dconst.exe",
                        Arguments = parameters,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    try
                    {
                        using (Process process = Process.Start(startInfo))
                        {
                            // Чтение стандартного вывода и ошибок
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();

                            process.WaitForExit();

                            // Вывод результатов
                            Console.WriteLine("Output:");
                            Console.WriteLine(output);
                            Console.WriteLine("Error:");
                            Console.WriteLine(error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                    }
                    break;
            }



        }

        public void const_actual()
        {
            label_message.Text = "Actual";
        }

        public void const_update()
        {
            label_message.Text = "update";
        }

        public void const_download()
        {
            label_message.Text = "Готов к загрузке";
            Btn_download.Content = "Загрузить";
        }

        public void ChangeMessage(string msg) => label_message.Text = msg;


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
