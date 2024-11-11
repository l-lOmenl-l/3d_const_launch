using IWshRuntimeLibrary;
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
        bool firstUpload = true;

        public MainWindow(string ip)
        {
            InitializeComponent();
            Config.ip = ip;
            if (ip != "https://3d.e-1.ru:8000")
            {
                SP_Warning.Visibility = Visibility.Visible;
            }

            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, EventArgs e)
        {
            //await Task.Run(() => Dispatcher.Invoke(() => Init()));
            await Init();
        }

        public Circular CreateLoadingCircular(string msg)
        {
            SecondGrid.Effect = new BlurEffect { Radius = 20 };
            
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

        public void ChangeMsgLoadingCircular(string msg, double procent)
        {
            Circular circular = (Circular)MainGrid.FindName("dynamicCircular");
            circular.ChangeMessage(msg, procent);
        }

        public async void ChangeMsgLoadingCircularAsync(string msg, double procent)
        {
            await Task.Run(() => Dispatcher.Invoke(() => ChangeMsgLoadingCircular(msg, procent)));
        }


        private Task CheckWebSocket()
        {
            if (!System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\E1_WebSocketLKK.lnk"))
            {
                WshShell shell = new WshShell();

                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\E1_WebSocketLKK.lnk");
                shortcut.TargetPath = Config.GetPath() + "\\E1_WebSocketLKK" + "\\E1_WebSocketLKK.exe";
                shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(Config.GetPath() + "\\E1_WebSocketLKK" + "\\E1_WebSocketLKK.exe");
                shortcut.Description = "E1WebSocketLKK";
                shortcut.IconLocation = Config.GetPath() + "\\E1_WebSocketLKK" + "\\E1_WebSocketLKK.exe" + ",0"; // Использование иконки из целевого приложения

                // Сохранение ярлыка
                shortcut.Save();


                // Вывод результата на консоль
                if (!IsProcessRunning("E1_WebSocketLKK"))
                {
                    Process.Start(Config.GetPath() + "\\E1_WebSocketLKK" + "\\E1_WebSocketLKK.exe");
                }


            }

            return Task.CompletedTask;
        }

        static bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }



        private async Task<Task> Init()
        {
            CreateLoadingCircular("Инициализация");

            //Проверка ВебСокета
            await CheckWebSocket();
            Alias.AliasIp.TryGetValue(Config.ip, out var outTemp);

            //AppShortcutToDesktop();
            await Config.Init(this);

            ChangeMsgLoadingCircular("Проверка обновлений лаунчера", 0);
            await Checklauncher();

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
            catch
            {
                var result = MessageBox.Show("Ошибка соединения с сервером, попробуйте запустить еще раз, через некоторое время.",
                    "Ошибка соединения", MessageBoxButton.OK, MessageBoxImage.Error);

                if (result == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
            }
        }



        private Task Checklauncher()
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

            if (hash == json) return Task.CompletedTask;
            btn_upload_launch.Visibility = Visibility.Visible; 
            return Task.CompletedTask;
        }

        private void DownloadLauncher_Click()
        {
            CreateLoadingCircular("Загрузка лаунчера");
            ChangeMsgLoadingCircular("Через 3 секунды лаунчер будет перезапущен.", 0);

            Download.DownloadLauncher(this);
   

        }


        public async void ChangeMessageAsync(string msg)
        {
            await Task.Run(() => Dispatcher.Invoke(() => label_message.Text = msg));
        }

        public async void ActualConst()
        {
            firstUpload = false;
            await Task.Run(() => Dispatcher.Invoke(() => Title.Text = "Можно запускать"));
            await Task.Run(() => Dispatcher.Invoke(() => IconTitle.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline));
            await Task.Run(() => Dispatcher.Invoke(() => Desctiption.Text = "Конструктор загружен, файлы успешно прошли проверку, удачного дня и помните, при возникновении ошибок, вы всегда можете оставить свое обращение в сервисе обратной связи."));

            await Task.Run(() => Dispatcher.Invoke(() => label_message.Text = "Файлы успешно прошли проверку"));
            await Task.Run(() => Dispatcher.Invoke(() => icon_labelMessage.Kind = MaterialDesignThemes.Wpf.PackIconKind.Check));

            await Task.Run(() => Dispatcher.Invoke(() => Btn_launch.IsEnabled = true));
            await Task.Run(() => Dispatcher.Invoke(() => btn_upload.Visibility = Visibility.Collapsed));
        }

        public async void FirstUploadConst()
        {
            firstUpload = true;
            await Task.Run(() => Dispatcher.Invoke(() => Title.Text = "Требуется загрузка"));
            await Task.Run(() => Dispatcher.Invoke(() => IconTitle.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloseOutline));
            await Task.Run(() => Dispatcher.Invoke(() => Desctiption.Text = "Конструктор не установлен. Требуется загрузка."));

            await Task.Run(() => Dispatcher.Invoke(() => label_message.Text = "Конструктор не установлен"));
            await Task.Run(() => Dispatcher.Invoke(() => icon_labelMessage.Kind = MaterialDesignThemes.Wpf.PackIconKind.EmoticonCryOutline));

            await Task.Run(() => Dispatcher.Invoke(() => Btn_launch.IsEnabled = false));
            await Task.Run(() => Dispatcher.Invoke(() => btn_upload.Visibility = Visibility.Visible));
            await Task.Run(() => Dispatcher.Invoke(() => text_btn_upload.Text = "Скачать"));

        }

        public async void UploadConst()
        {
            firstUpload = false;
            await Task.Run(() => Dispatcher.Invoke(() => Title.Text = "Требуется обновление"));
            await Task.Run(() => Dispatcher.Invoke(() => IconTitle.Kind = MaterialDesignThemes.Wpf.PackIconKind.Update));
            await Task.Run(() => Dispatcher.Invoke(() => Desctiption.Text = "Файлы не прошли проверку, требуется обновление конструктора"));

            await Task.Run(() => Dispatcher.Invoke(() => label_message.Text = "Файлы не прошли проверку"));
            await Task.Run(() => Dispatcher.Invoke(() => icon_labelMessage.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloseOutline));

            await Task.Run(() => Dispatcher.Invoke(() => Btn_launch.IsEnabled = false));
            await Task.Run(() => Dispatcher.Invoke(() => btn_upload.Visibility = Visibility.Visible));
            await Task.Run(() => Dispatcher.Invoke(() => text_btn_upload.Text = "Имеется обновление - 'Скачать'"));
        }


        public async void BtnChange(string value)
        {
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.IsEnabled = true));
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.Text = value));
        }


        public void AppClose()
        {
            Dispatcher.Invoke(() => Application.Current.Shutdown());
        }

        private void Btn_launch_Click(object sender, RoutedEventArgs e)
        {

            string parameters = "-url=" + Config.ip;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Config.GetPath() + "/3dconst.exe",
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
                    WindowState = WindowState.Minimized;
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    
                    process.WaitForExit();
                    WindowState = WindowState.Normal;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

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
            var settingWindow = new Settings(firstUpload);
            settingWindow.Show();
        }

        private async void btn_upload_Click(object sender, RoutedEventArgs e)
        {
            if (firstUpload)
            {
                CreateLoadingCircular("Скачивание");
                Btn_download.IsEnabled = false;
                await Task.Run(() => Download.DownloadConstruct(this, FilesData.GetServerPath()));
                DestroyLoadingCircular();

            }
            else
            {
                CreateLoadingCircular("Скачивание");
                Btn_download.IsEnabled = false;
                await Task.Run(() => Download.DownloadConstruct(this, FilesData.CheckDiffFiles()));
                DestroyLoadingCircular();

            }
        }

        private void btn_upload_launch_Click(object sender, RoutedEventArgs e)
        {
             DownloadLauncher_Click();
        }
    }
}
