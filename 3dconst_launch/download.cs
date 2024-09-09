using IWshRuntimeLibrary;
using System;
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
                mainWindow.ChangeMsgLoadingCircularAsync("Скачивается файл " + FilesData.getDownloadFileCount() + " из " + files.Count() + "\r\n" + "Общий размер файлов: " + (int)calc_AllSize(files) / 1000 + " МБ, Скачано: " + (int)FilesData.getDownloadSizes() + " МБ");
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
            
            foreach (var (file, index) in files.Select((value, i) => (value, i)))
            {
                
                /*
                mainWindow.ChangeMessageAsync("Скачивается файл " + (index + 1) + " из " + files.Count() + "\r\n" + "Общий размер файлов: " + (int)allSize + " МБ, Скачано: " + (int)downloadSize + " МБ");
                //mainWindow.ChangeProgressBatAsync(((float)index + (float)1) * (100 / (float)files.Count()));
                mainWindow.ChangeProgressBatAsync(downloadSize / allSize * 100);
                */
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

            mainWindow.ChangeProgressBarVisibility(System.Windows.Visibility.Hidden);
            mainWindow.ChangeMessageAsync("Проверка файлов");
            foreach (var item in Directory.GetFiles(Config.GetPath() + "/temp/", "*", SearchOption.AllDirectories))
            {
                string newfile = (Config.GetPath() + item.Replace(Config.GetPath() + "/temp", ""));
                System.IO.File.Copy(item, newfile, true);
                System.IO.File.Delete(item);
            }

            System.IO.Directory.Delete(Config.GetPath() + "/temp/", true);

            mainWindow.ChangeMessageAsync("Файлы успешно прошли проверку");
            mainWindow.BtnChange("Запустить");
            

        }

        public static void DownloadKkt() 
        {
            string pathexe = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            pathexe = pathexe?.Remove(pathexe.LastIndexOf("\\", StringComparison.Ordinal));
            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers[HttpRequestHeader.Authorization] = Config.GetAuth();
                wc.DownloadFile(Config.GetIp() + "/update_kkt", pathexe + "\\fptr10.dll.temp");
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

        public static Task DownloadLauncher()
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

            Uri uri = new Uri(Config.GetIp() + "/sync");
            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers[HttpRequestHeader.Authorization] = Config.GetAuth();
                string pathexe = System.Reflection.Assembly.GetEntryAssembly().Location;
                pathexe = pathexe.Remove(pathexe.LastIndexOf("\\"));
                
                wc.DownloadFile(uri + "/update_launcher", pathexe + "\\temp" + "\\3dconst_launch.exe");
            }

            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            path = path.Remove(path.LastIndexOf("\\"));
            return Task.CompletedTask;
        }

    }

}

