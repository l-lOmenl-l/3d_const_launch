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


        private static Dictionary<string, string> GetServerFilesData()
        {
            var req = (HttpWebRequest)WebRequest.Create(Config.GetIp() + "/MD5");
            req.Method = "GET";
            req.Headers.Add("Authorization", Config.GetAuth());
            req.Proxy = null;
            var res = (HttpWebResponse)req.GetResponse();
            var json = JsonSerializer.Deserialize<Dictionary<string, string>>(res.GetResponseStream() ?? throw new InvalidOperationException());
            return json;
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


        public static List<string> CheckDiffFiles()
        {
            Dictionary<string, string> local = GetLocalFilesData();
            Dictionary<string, string> server = GetServerFilesData();
            List<string> upload = new List<string>();
            foreach (var file in server)
            {
                string temp = Config.GetPath() + file.Key;
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
            Dictionary<string, string> server = GetServerFilesData();


            foreach (var file in server)
            {
                string temp = Config.GetPath() + file.Key;
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

            return local.Select(file => file.Key.Replace(Config.GetPath(), "")).All(temp => server.ContainsKey(temp));
        }
    }
}
