using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace _3dconst_launch
{
    internal class config
    {

        public class Conf
        {
            public string Local_version { get; set; }
            public bool Setup { get; set; }

            public string Path_const { get; set; }

            public string ip_server { get; set; }

            public Conf(string local_version, bool setup, string path_const, string ip)
            {
                Local_version = local_version;
                Setup = setup;
                Path_const = path_const;
                ip_server = ip;
            }

            public bool GetSetup()
            {
                return Setup;
            }

            public string GetLocalVersion()
            {
                return Local_version;
            }

        }

        static public Conf conf = new Conf("none", false, "D:/TestDirectory", "http://192.168.0.105:8000");


        static public string GetPathConfig(bool addfile)
        {
            if (addfile)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Launcher_E1/conf.json";
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Launcher_E1/";
        }

        public static async Task createConf(MainWindow mainWindow)
        {
            await Task.Run(() => mainWindow.changeMessageAsync("Создание конфигурации"));
            conf = new Conf("none", false, "D:/TestDirectory", "http://192.168.0.105:8000");
            FileStream createStream = File.Create(GetPathConfig(true));
            JsonSerializer.Serialize(createStream, conf);
            createStream.Dispose();
        }

        static public void readConf()
        {
            while (!File.Exists(GetPathConfig(true)))
            {

            }
            FileStream readStream = File.OpenRead(GetPathConfig(true));
            conf = JsonSerializer.Deserialize<Conf>(readStream);
        }



        public static async void init(MainWindow mainWindow)
        {
            if (!Directory.Exists(GetPathConfig(false)))
            {
                Directory.CreateDirectory(GetPathConfig(false));
            }
            if (!File.Exists(GetPathConfig(true)))
            {
                await createConf(mainWindow);
            }
            if (Directory.Exists(conf.Path_const + "/temp"))
            {
                Directory.Delete(conf.Path_const + "/temp", true);
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
