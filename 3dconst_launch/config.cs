using System;
using System.IO;
using System.Text.Json;

namespace _3dconst_launch
{
    internal class config
    {
        public class Conf
        {
            public string Local_version {  get; }
            public bool Setup { get; }

            public Conf(string local_version, bool setup)
            {
                Local_version = local_version;
                Setup = setup;
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

        static public Conf conf = new Conf("", false);


        static public string GetPathConfig()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Launcher_E1/conf.json"; ;
        }

        static public void createConf()
        {
            conf = new Conf("none", false);
            FileStream createStream = File.Create(GetPathConfig());
            JsonSerializer.SerializeAsync(createStream, conf);
            createStream.Dispose();
        }

        static public void readConf()
        {
            FileStream readStream = File.OpenRead(GetPathConfig());
            conf = JsonSerializer.Deserialize<Conf>(readStream);
        }

       static public void init()
        {
            if (!File.Exists(GetPathConfig())){
                createConf();
            }

            readConf();

            if (conf.GetLocalVersion() == "none")
            {
                MainWindow.const_download();
            } 



        }



    }
}
