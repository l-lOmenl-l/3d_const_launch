using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;


namespace _3dconst_launch
{
    



    internal abstract class Config
    {
        public static string ip;

        public static string GetAuth()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("3dConst" + ":" + "UdyT0epH"));   
        }

        public static string GetIp()
        {
            return ip;
        }

        public static string GetPath()
        {
            return Alias.GetPathConst(ip);
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


        public static void Init(MainWindow mainWindow)
        {

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

        }
    }
}
