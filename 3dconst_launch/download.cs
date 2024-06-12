using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace _3dconst_launch
{
    internal abstract class Download
    {

        public static void DownloadConstruct(MainWindow mainWindow, List<string> files)
        {
            if (!Directory.Exists(Config.GetPath())) 
            {
                Directory.CreateDirectory(Config.GetPath());
            }

            Directory.CreateDirectory(Config.GetPath() + "/temp");
            //List<string> serverData = new List<string>();

            mainWindow.ChangeProgressBarVisibility(System.Windows.Visibility.Visible);

            //serverData.RemoveRange(10, 37);

            foreach (var (file, index) in files.Select((value, i) => (value, i)))
            {
                mainWindow.ChangeMessageAsync("Скачивается файл " + (index + 1) + " из " + files.Count());
                mainWindow.ChangeProgressBatAsync(((float)index + (float)1) * (100 / (float)files.Count()));

                string str = file;
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
                    string mybase = Convert.ToBase64String(Encoding.UTF8.GetBytes(file));
                    wc.Headers.Add("file", mybase);

                    var uri = new Uri(Config.GetIp() + "/update_build");
                    wc.DownloadFile(uri, Config.GetPath() + "/temp/" + file);

                }
            }

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

            if (File.Exists(pathexe + "\\fptr10.dll.temp"))
            {
                if(File.Exists(pathexe + "\\fptr10.dll"))
                {
                    File.Delete(pathexe + "\\fptr10.dll");
                }

                if (File.Exists("C:\\Windows\\fptr10.dll"))
                {
                    File.Delete("C:\\Windows\\fptr10.dll");
                }

                File.Move(pathexe + "\\fptr10.dll.temp", pathexe + "\\fptr10.dll");
                File.Copy(pathexe + "\\fptr10.dll", "C:\\Windows\\fptr10.dll");
            }


        } 
    }

}

