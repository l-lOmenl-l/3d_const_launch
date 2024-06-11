using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3dconst_launch
{
    public static class Alias
    {
        public static Dictionary<string, string> alias_IP = new Dictionary<string, string>()
        {
            ["https://3d.e-1.ru:8000/sync"] = "Рабочий сервер",
            ["https://3d-test.e-1.ru:8000/sync"] = "Тестовый сервер",
            ["http://192.168.40.135:8000/sync"] = "dev",
            ["http://127.0.0.1:8000/sync"] = "dev local"
        };

        public static Dictionary<string, string> alias_Path_Const = new Dictionary<string, string>()
        {
            ["https://3d.e-1.ru:8000/sync"] = "C:\\Soft\\3dconst\\",
            ["https://3d-test.e-1.ru:8000/sync"] = "C:\\Soft\\3dconstTestOnline\\",
            ["http://192.168.40.135:8000/sync"] = "C:\\Soft\\3dconstTest\\",
            ["http://127.0.0.1:8000/sync"] = "C:\\Soft\\3dconstLocal"
        };

        public static string GetPathConst(string ip)
        {
            foreach (KeyValuePair<string, string> items in Alias.alias_Path_Const)
            {
                if (ip == items.Key)
                {
                    return items.Value;
                }
            }

            return null;
        }

        public static string[] listSettingsParams = new string[]
                {
                "ViewDistanceQuality",
                "AntiAliasingQuality",
                "ShadowQuality",
                "PostProcessQuality",
                "TextureQuality",
                "EffectsQuality",
                "ShadingQuality",
                };

        public static Dictionary<int, string> alias_Settings_Param = new Dictionary<int, string>()
        {
            [0] = "Низко",
            [1] = "Средне",
            [2] = "Высоко",
            [3] = "Эпично",
            [4] = "Синематик",
        };

        public static Dictionary<string, string> alias_label = new Dictionary<string, string>()
        {
            ["ViewDistanceQuality"] = "Дальность",
            ["AntiAliasingQuality"] = "Сглаживание",
            ["ShadowQuality"] = "Качество теней",
            ["PostProcessQuality"] = "Качество постпроцесса",
            ["TextureQuality"] = "Качество текстур",
            ["EffectsQuality"] = "Качество эффектов",
            ["ShadingQuality"] = "Качество шейдеров",

        };

    }
}
