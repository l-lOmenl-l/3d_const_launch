using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace _3dconst_launch
{
    
    internal class config
    {
        public static void checkConf(MainWindow mainWindow)
        {
            if (!File.Exists(GetPathConfig(true)))
            {
                createConf("");
            }
        }

        public static string getAuth()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("3dConst" + ":" + "UdyT0epH"));   
        }

        public static string getIP()
        {
            FileStream readStream = File.OpenRead(GetPathConfig(true));
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(readStream);
            var temp = result["ip_server"].ToString();
            if (temp == "")
            {
                SetIP setIP = new SetIP();
                setIP.ShowDialog();
            }
            return temp;
        }

        public static string getPath()
        {
            FileStream readStream = File.OpenRead(GetPathConfig(true));
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(readStream);
            var temp = result["Path_const"].ToString();
            return temp;
        }

        static public string GetPathConfig(bool addfile)
        {
            if (addfile)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Launcher_E1/conf.json";
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Launcher_E1/";
        }


        public static void createConf(string ip)
        {
            //await Task.Run(() => mainWindow.changeMessageAsync("Создание конфигурации"));
            FileStream createStream = File.Create(GetPathConfig(true));

            if (ip == "")
            {
                ip = "https://3d.e-1.ru:8000/sync";
            }

            var configs = new { Path_const = Alias.GetPathConst(ip), ip_server = ip };
                

            JsonSerializer.Serialize(createStream, configs);
            createStream.Dispose();
        }

        /*
        static public void readConf()
        {
            FileStream readStream = File.OpenRead(GetPathConfig(true));
            conf = JsonSerializer.Deserialize<Conf>(readStream);
        }
        */

        public static async void init(MainWindow mainWindow)
        {
            getIP();

            if (!Directory.Exists(GetPathConfig(false)))
            {
                Directory.CreateDirectory(GetPathConfig(false));
            }

            if (!File.Exists(GetPathConfig(true)))
            {
                createConf("");
            }

            if (Directory.Exists(getPath() + "/temp"))
            {
                Directory.Delete(getPath() + "/temp", true);
            }
            mainWindow.changeMessageAsync("Проверка конфигурации");


            if (FilesData.GetLocalFilesData().Count > 0)
            {
                if (!FilesData.checkFiles())
                {
                    mainWindow.changeMessageAsync("Файлы не прошли проверку, готов к обновлению");
                    mainWindow.btnChange("Обновление");
                }
                else
                {
                    mainWindow.changeMessageAsync("Готов к запуску");
                    mainWindow.btnChange("Запустить");
                }
            }
            else
            {
                mainWindow.changeMessageAsync("Конструктор не установлен, готов к загрузке");
                mainWindow.btnChange("Загрузить");
            }




            /*
            if (conf.GetLocalVersion() == "none")
            {
                mainWindow.changeMessageAsync("Конструктор не установлен, готов к загрузке");
                mainWindow.const_download();
            } 
            */


        }




    }
}
