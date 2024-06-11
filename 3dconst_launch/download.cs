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
    internal class download
    {

        public static void DownloadConstruct(MainWindow mainWindow, List<string> files)
        {
            if (!Directory.Exists(config.getPath())) 
            {
                Directory.CreateDirectory(config.getPath());
            }

            Directory.CreateDirectory(config.getPath() + "/temp");
            //List<string> serverData = new List<string>();

            mainWindow.ChangeProgressBarVisibility(System.Windows.Visibility.Visible);

            //serverData.RemoveRange(10, 37);

            foreach (var (file, index) in files.Select((value, i) => (value, i)))
            {
                int myindex = index + 1;
                mainWindow.changeMessageAsync("Скачивается файл " + (index + 1) + " из " + files.Count());
                mainWindow.ChangeProgressBatAsync(((float)index + (float)1) * (100 / (float)files.Count()));

                string str = file;
                str = str.Remove(str.LastIndexOf("/"));
                if (str != "")
                {
                    if (!Directory.Exists(str))
                    {
                        Directory.CreateDirectory(config.getPath() + "/temp/" + str);
                        Directory.CreateDirectory(config.getPath() + str);
                    }
                }

                using (var wc = new WebClient())
                {
                    wc.Proxy = null;
                    wc.Headers[HttpRequestHeader.Authorization] = config.getAuth();
                    string mybase = Convert.ToBase64String(Encoding.UTF8.GetBytes(file));
                    wc.Headers.Add("file", mybase);

                    Uri uri = new Uri(config.getIP() + "/update_build");
                    wc.DownloadFile(uri, config.getPath() + "/temp/" + file);

                }
            }

            mainWindow.ChangeProgressBarVisibility(System.Windows.Visibility.Hidden);
            mainWindow.changeMessageAsync("Проверка файлов");
            foreach (var item in Directory.GetFiles(config.getPath() + "/temp/", "*", SearchOption.AllDirectories))
            {
                string newfile = (config.getPath() + item.Replace(config.getPath() + "/temp", ""));
                System.IO.File.Copy(item, newfile, true);
                System.IO.File.Delete(item);
            }

            System.IO.Directory.Delete(config.getPath() + "/temp/", true);
            mainWindow.changeMessageAsync("Файлы успешно прошли проверку");
            mainWindow.btnChange("Запустить");

        }

        public static void DownloadKKT() 
        {
            string pathexe = System.Reflection.Assembly.GetEntryAssembly().Location;
            pathexe = pathexe.Remove(pathexe.LastIndexOf("\\"));
            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers[HttpRequestHeader.Authorization] = config.getAuth();
                wc.DownloadFile(config.getIP() + "/update_kkt", pathexe + "\\fptr10.dll.temp");
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

