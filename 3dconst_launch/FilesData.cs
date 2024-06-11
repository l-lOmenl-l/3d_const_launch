using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace _3dconst_launch
{
    internal class FilesData
    {
        public static Dictionary<string, string> GetLocalFilesData()
        {
            if (Directory.Exists(config.getPath()))
            {
                var result = new Dictionary<string, string>();

                string[] path = Directory.GetFiles(config.getPath(), "*", SearchOption.AllDirectories);
                var md5 = MD5.Create();
                foreach (string files in path)
                {
                    result.Add(files.Replace("\\", "/"), BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(files))).Replace("-", "").ToLower());
                }
                return result;
            }
            else
            {
                return new Dictionary<string, string>();
            }
            
        }

   
        public static Dictionary<string, string> GetServerFilesData()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(config.getIP() + "/MD5");
            req.Method = "GET";
            req.Headers.Add("Authorization", config.getAuth());
            req.Proxy = null;
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            var myjson = JsonSerializer.Deserialize<Dictionary<string, string>>(res.GetResponseStream());
            return myjson;
        }


        public static List<string> GetServerPath()
        {
            List<string> path = new List<string>();
            foreach (var obj in GetServerFilesData())
            {
                path.Add(obj.Key);
            }
            return path;
        }


        public static List<string> checkDiffFiles()
        {
            Dictionary<string, string> local = GetLocalFilesData();
            Dictionary<string, string> server = GetServerFilesData();
            List<string> upload = new List<string>();
            List<string> delete = new List<string>();
            foreach (var file in server)
            {
                string temp = config.getPath() + file.Key;
                if (!local.ContainsKey(temp))
                {
                    upload.Add(file.Key);
                }
                else
                {
                    local.TryGetValue(temp, out var temp2);
                    if (temp2 != file.Value)
                    {
                        upload.Add(file.Key.Replace("\\", "/"));
                    }
                }
            }

            foreach (var file in local)
            {
                string temp = file.Key.Replace(config.getPath(), "");
                if (!server.ContainsKey(temp))
                {
                    delete.Add(file.Key);
                }
            }

            deletefiles(delete);

            return upload;
        }

        private static void deletefiles(List<string> files)
        {
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }


        public static bool checkFiles()
        {
            Dictionary<string, string> local = GetLocalFilesData();
            Dictionary<string, string> server = GetServerFilesData();


            foreach (var file in server)
            {
                string temp = config.getPath() + file.Key;
                if (!local.ContainsKey(temp))
                {
                    return false;
                }
                else
                {
                    local.TryGetValue(temp, out var temp2);
                    if (temp2 != file.Value)
                    {
                        return false;
                    }
                }
            }

            foreach (var file in local)
            {
                string temp = file.Key.Replace(config.getPath(), "");
                if (!server.ContainsKey(temp))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
