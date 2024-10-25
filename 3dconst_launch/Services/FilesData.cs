using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace _3dconst_launch
{
    internal class FilesData
    {
        public static float getDownloadSizes()
        {
            string[] path = Directory.GetFiles(Config.GetPath()+ "/temp", "*", SearchOption.AllDirectories);
            float sum = 0;
            foreach (string temp in path)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(temp);
                sum += file.Length;
            }
            
            return sum * (float)Math.Pow(10, -3) / 1000;
        }

        public static float getDownloadFileCount()
        {
            string[] path = Directory.GetFiles(Config.GetPath()+"/temp", "*", SearchOption.AllDirectories);
            float sum = 0;
            foreach (string temp in path)
            {
                sum += 1;
            }

            return sum;
        }

        public static Dictionary<string, string> GetLocalFilesData()
        {
            if (Directory.Exists(Config.GetPath()))
            {
                string[] path = Directory.GetFiles(Config.GetPath(), "*", SearchOption.AllDirectories);
                var md5 = MD5.Create();
                return path.ToDictionary(files => files.Replace("\\", "/"), files => BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(files))).Replace("-", "").ToLower());
            }
            else
            {
                return new Dictionary<string, string>();
            }
            
        }

        public class FileInfo
        {
            public float size { get; set; }
            public string hash { get; set; }
        }

        private static Dictionary<string, FileInfo> GetServerFilesData()
        {
            var req = (HttpWebRequest)WebRequest.Create(Config.GetIp() + "/sync" + "/MD5");
            req.Method = "GET";
            req.Headers.Add("Authorization", Config.GetAuth());
            req.Proxy = null;
            var res = (HttpWebResponse)req.GetResponse();
            var json = JsonSerializer.Deserialize<Dictionary<string, FileInfo>>(res.GetResponseStream() ?? throw new InvalidOperationException());
            return json;
        }


        public static Dictionary<string, float> GetServerPath()
        {
            var path = new Dictionary<string, float> ();
            foreach (var obj in GetServerFilesData())
            {
                path[obj.Key] = obj.Value.size;
            }
            return path;
        }


        public static Dictionary<string, float> CheckDiffFiles()
        {
            Dictionary<string, string> local = GetLocalFilesData();
            Dictionary<string, FileInfo> server = GetServerFilesData();
            var upload = new Dictionary<string, float>();
            foreach (var file in server)
            {
                string temp = Config.GetPath() + file.Key;
                if (!local.ContainsKey(temp))
                {
                    upload[temp.Replace(Config.GetPath(),"")] = file.Value.size;
                }
                else
                {
                    local.TryGetValue(temp, out var temp2);
                    if (temp2 != file.Value.hash)
                    {
                        upload[file.Key.Replace("\\", "/").Replace(Config.GetPath(), "")] = file.Value.size;
                    }
                }
            }

            List<string> delete = (from file in local let temp = file.Key.Replace(Config.GetPath(), "") where !server.ContainsKey(temp) select file.Key).ToList();

            Deletefiles(delete);

            return upload;
        }

        private static void Deletefiles(List<string> files)
        {
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }


        public static bool CheckFiles()
        {
            Dictionary<string, string> local = GetLocalFilesData();
            Dictionary<string, FileInfo> server = GetServerFilesData();


            foreach (var file in server)
            {
                string temp = (Config.GetPath() + file.Key);
                if (!local.ContainsKey(temp))
                {
                    return false;
                }
                else
                {
                    local.TryGetValue(temp, out var temp2);
                    if (temp2 != file.Value.hash)
                    {
                        return false;
                    }
                }
            }
            return local.Select(file => file.Key.Replace(Config.GetPath(), "")).All(temp => server.ContainsKey(temp));
        }
    }
}
