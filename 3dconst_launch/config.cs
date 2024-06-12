using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;


namespace _3dconst_launch
{
    internal abstract class Config
    {
        public static void CheckConf(MainWindow mainWindow)
        {
            if (!File.Exists(GetPathConfig(true)))
            {
                CreateConf("");
            }
        }

        public static string GetAuth()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("3dConst" + ":" + "UdyT0epH"));   
        }

        public static string GetIp()
        {
            var readStream = File.OpenRead(GetPathConfig(true));
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(readStream);
            var temp = result["ip_server"].ToString();

            if (temp != "") return temp;
            
            var setIp = new SetIp();
            setIp.ShowDialog();
            
            return temp;
        }

        public static string GetPath()
        {
            var readStream = File.OpenRead(GetPathConfig(true));
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(readStream);
            var temp = result["Path_const"].ToString();
            return temp;
        }

        public static string GetPathConfig(bool addFile)
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Launcher_E1/"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
                                          "/Launcher_E1/");
            }
            
            if (addFile)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Launcher_E1/conf.json";
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Launcher_E1/";
        }


        public static void CreateConf(string ip)
        {
            //await Task.Run(() => mainWindow.changeMessageAsync("Создание конфигурации"));
            var createStream = File.Create(GetPathConfig(true));

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

        public static async void Init(MainWindow mainWindow)
        {
            GetIp();

            if (!Directory.Exists(GetPathConfig(false)))
            {
                Directory.CreateDirectory(GetPathConfig(false));
            }

            if (!File.Exists(GetPathConfig(true)))
            {
                CreateConf("");
            }

            if (Directory.Exists(GetPath() + "/temp"))
            {
                Directory.Delete(GetPath() + "/temp", true);
            }
            mainWindow.ChangeMessageAsync("Проверка конфигурации");


            if (FilesData.GetLocalFilesData().Count > 0)
            {
                if (!FilesData.CheckFiles())
                {
                    mainWindow.ChangeMessageAsync("Файлы не прошли проверку, готов к обновлению");
                    mainWindow.BtnChange("Обновление");
                }
                else
                {
                    mainWindow.ChangeMessageAsync("Готов к запуску");
                    mainWindow.BtnChange("Запустить");
                }
            }
            else
            {
                mainWindow.ChangeMessageAsync("Конструктор не установлен, готов к загрузке");
                mainWindow.BtnChange("Загрузить");
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
