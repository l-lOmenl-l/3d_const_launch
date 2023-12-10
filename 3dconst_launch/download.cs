using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace _3dconst_launch
{
    internal class download
    {

        public static string auth()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("3dConst" + ":" + "UdyT0epH"));
        }

        public static void DownloadConstruct(MainWindow mainWindow, List<string> files)
        {
            Directory.CreateDirectory(config.conf.Path_const + "/temp");
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
                        Directory.CreateDirectory(config.conf.Path_const + "/temp/" + str);
                        Directory.CreateDirectory(config.conf.Path_const + str);
                    }
                }

                using (var wc = new WebClient())
                {
                    wc.Proxy = null;
                    wc.Headers[HttpRequestHeader.Authorization] = auth();
                    string mybase = Convert.ToBase64String(Encoding.UTF8.GetBytes(file));
                    wc.Headers.Add("file", mybase);

                    Uri uri = new Uri(config.conf.ip_server + "/update_build");
                    wc.DownloadFile(uri, config.conf.Path_const + "/temp" + file);

                }
            }

            mainWindow.ChangeProgressBarVisibility(System.Windows.Visibility.Hidden);
            mainWindow.changeMessageAsync("Проверка файлов");
            foreach (var item in Directory.GetFiles(config.conf.Path_const + "/temp/", "*", SearchOption.AllDirectories))
            {
                string newfile = (config.conf.Path_const + item.Replace(config.conf.Path_const + "/temp", ""));
                System.IO.File.Copy(item, newfile, true);
                System.IO.File.Delete(item);
            }

            System.IO.Directory.Delete(config.conf.Path_const + "/temp/", true);
            mainWindow.changeMessageAsync("Файлы успешно прошли проверку");
            mainWindow.btnChange("Запустить");

        }
    }

}

