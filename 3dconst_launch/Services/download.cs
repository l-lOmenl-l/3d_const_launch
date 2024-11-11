using IWshRuntimeLibrary;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace _3dconst_launch
{
    internal abstract class Download
    {
        private static float calc_AllSize(Dictionary<string, float> files)
        {
            float sum = 0;
            foreach (var temp in files) { sum += temp.Value; }
            return sum;
        }
        private static async Task CalculationDonwloadSize(MainWindow mainWindow, Dictionary<string, float> files, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int downloadSize = (int)FilesData.getDownloadSizes();
                int allSize = (int)calc_AllSize(files) / 1000;
                float percent = (float)((float)downloadSize / (float)allSize * 100.0);
                mainWindow.ChangeMsgLoadingCircularAsync("Скачано: " + downloadSize + " МБ из " + allSize + " МБ", percent);
                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

        }

        public static void DownloadConstruct(MainWindow mainWindow, Dictionary<string, float> files)
        {
            if (!Directory.Exists(Config.GetPath())) 
            {
                Directory.CreateDirectory(Config.GetPath());
            }
            
            Directory.CreateDirectory(Config.GetPath() + "/temp");

            CancellationTokenSource cts = new CancellationTokenSource();
            Task countingTask = CalculationDonwloadSize(mainWindow, files, cts.Token);

            foreach (var fileWithIndex in files.Select((value, i) => new { File = value, Index = i }))
            {
                var file = fileWithIndex.File;
                var index = fileWithIndex.Index;

                string str = file.Key;
                str = str.Remove(str.LastIndexOf("/", StringComparison.Ordinal));
                if (str != "")
                {
                    if (!Directory.Exists(str))
                    {
                        Directory.CreateDirectory(Config.GetPath() + "/temp/" + str);
                        Directory.CreateDirectory(Config.GetPath() + str);
                    }
                }

                using (var wc = new WebClient())
                {
                    wc.Proxy = null;
                    wc.Headers[HttpRequestHeader.Authorization] = Config.GetAuth();
                    string mybase = Convert.ToBase64String(Encoding.UTF8.GetBytes(file.Key));
                    wc.Headers.Add("file", mybase);

                    var uri = new Uri(Config.GetIp() + "/sync/update_build");
                    string temp = Config.GetPath() + "/temp" + file.Key;
                    wc.DownloadFile(uri, temp);
                }
            }

            cts.Cancel();

            mainWindow.ChangeMessageAsync("Проверка файлов");
            foreach (var item in Directory.GetFiles(Config.GetPath() + "/temp/", "*", SearchOption.AllDirectories))
            {
                string newfile = (Config.GetPath() + item.Replace(Config.GetPath() + "/temp", ""));
                System.IO.File.Copy(item, newfile, true);
                System.IO.File.Delete(item);
            }

            System.IO.Directory.Delete(Config.GetPath() + "/temp/", true);

            //mainWindow.ChangeMessageAsync("Файлы успешно прошли проверку");
            //mainWindow.BtnChange("Запустить");
            mainWindow.ActualConst();

        }

        public static void DownloadKkt() 
        {
            string pathexe = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            pathexe = pathexe?.Remove(pathexe.LastIndexOf("\\", StringComparison.Ordinal));

            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers[HttpRequestHeader.Authorization] = Config.GetAuth();
                wc.DownloadFile(Config.GetIp() + "/sync/update_kkt", pathexe + "\\fptr10.dll.temp");
            }

            if (System.IO.File.Exists(pathexe + "\\fptr10.dll.temp"))
            {
                if(System.IO.File.Exists(pathexe + "\\fptr10.dll"))
                {
                    System.IO.File.Delete(pathexe + "\\fptr10.dll");
                }

                if (System.IO.File.Exists("C:\\Windows\\fptr10.dll"))
                {
                    System.IO.File.Delete("C:\\Windows\\fptr10.dll");
                }

                System.IO.File.Move(pathexe + "\\fptr10.dll.temp", pathexe + "\\fptr10.dll");
                System.IO.File.Copy(pathexe + "\\fptr10.dll", "C:\\Windows\\fptr10.dll");
            }


        }


        private static async void test(MainWindow mainWindow)
        {
            mainWindow.ChangeMsgLoadingCircular("Через 3 секунды лаунчер будет перезапущен.", 0);
            System.IO.File.Copy(".\\temp\\3dconst_launch.exe", ".\\3dconst_launch.exe.temp");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.Arguments = $"/c TIMEOUT /T 1 /NOBREAK && del 3dconst_launch.exe && move 3dconst_launch.exe.temp 3dconst_launch.exe && start 3dconst_launch.exe";
            await Task.Run(() => mainWindow.Dispatcher.Invoke(() => Process.Start(psi)));
            mainWindow.AppClose();
        }


        public static bool DownloadLauncher(MainWindow mainWindow)
        {
            if (Directory.Exists("temp"))
            {
                Directory.Delete("temp", true);
                Directory.CreateDirectory("temp");
            }
            else
            {
                Directory.CreateDirectory("temp");
            }



            Uri uri = new Uri(Config.GetIp() + "/sync/update_launcher");
            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers[HttpRequestHeader.Authorization] = Config.GetAuth();
                string pathexe = System.Reflection.Assembly.GetEntryAssembly().Location;
                pathexe = pathexe.Remove(pathexe.LastIndexOf("\\"));
                wc.DownloadFileCompleted += (sender, e) => test(mainWindow);
                wc.DownloadFileAsync(uri, pathexe + "\\temp\\3dconst_launch.exe");
            }

            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            path = path.Remove(path.LastIndexOf("\\"));
            return true;
        }

    }

}

