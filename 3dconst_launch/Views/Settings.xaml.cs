using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Windows.Media.Effects;
using System.Threading.Tasks;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private class setting
        {
            public int ViewDistanceQuality { get; set; }
            public int AntiAliasingQuality { get; set; }
            public int ShadowQuality { get; set; }
            public int PostProcessQuality { get; set; }
            public int TextureQuality { get; set; }
            public int EffectsQuality { get; set; }
            public int ShadingQuality { get; set; }

            public void LoadData()
            {
                this.ViewDistanceQuality = ReadFileSettings("ViewDistanceQuality");
                this.AntiAliasingQuality = ReadFileSettings("AntiAliasingQuality");
                this.ShadowQuality = ReadFileSettings("ShadowQuality");
                this.PostProcessQuality = ReadFileSettings("PostProcessQuality");
                this.TextureQuality = ReadFileSettings("TextureQuality"); ;
                this.EffectsQuality = ReadFileSettings("EffectsQuality");
                this.ShadingQuality = ReadFileSettings("ShadingQuality");
            }


        }

        public string TranslateNumberToName(int num)
        {
            switch (num)
            {
                case 0:
                    return "Низко";
                case 1:
                    return "Средне";
                case 2:
                    return "Высоко";
                case 3:
                    return "Эпично";
                case 4:
                    return "Синематик";
            }

            return null;
        }

        public int TransNameToNumber(string name)
        {
            switch (name)
            {
                case "Низко":
                    return 0;
                case "Средне":
                    return 1;
                case "Высоко":
                    return 2;
                case "Эпично":
                    return 3;
                case "Синематик":
                    return 4;
            }

            return -1;
        }

        private static int ReadFileSettings(string key)
        {
           var lines = File.ReadLines(GetPathConf() + "\\GameUserSettings.ini");
           string result = string.Join("\n",lines.Where(s => s.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) >= 0));
           string[] words = result.Split(new char[] { '=' });
           return Convert.ToInt32(words[1]);
        }


        List<ComboBox> cb = new List<ComboBox>();
        public Settings(bool firstUpload)
        {
            InitializeComponent();
            Init();
        }

        private readonly List<ComboBox> _refCb = new List<ComboBox>();
        private readonly Dictionary<string, int> _uploadSettings = new Dictionary<string, int>();

        private static string GetPathConf()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\..\\" + "\\Local\\E1\\Saved\\Config\\WindowsNoEditor";
        }

        private void cb_init()
        {
            List<string> names = new List<string> ();
            for (int i = 0; i < 5; i++) 
            {
                names.Add(TranslateNumberToName(i));
            }
            cb.Add(CB_ViewDistanceQuality);
            cb.Add(CB_AntiAliasingQuality);
            cb.Add(CB_ShadowQuality);
            cb.Add(CB_EffectsQuality);
            cb.Add(CB_ShadingQuality);
            cb.Add(CB_TextureQuality);
            cb.Add(CB_PostProcessQuality);

            foreach (ComboBox item in cb)
            {
                foreach (string name in names)
                {
                    item.Items.Add(name);
                }
            }

        }


        private void Init()
        {
            var settings = new setting();
            settings.LoadData();
            cb_init();
            CB_ViewDistanceQuality.SelectedItem = TranslateNumberToName(settings.ViewDistanceQuality);
            CB_AntiAliasingQuality.SelectedItem = TranslateNumberToName(settings.AntiAliasingQuality);
            CB_ShadowQuality.SelectedItem = TranslateNumberToName(settings.ShadowQuality);
            CB_PostProcessQuality.SelectedItem = TranslateNumberToName(settings.PostProcessQuality);
            CB_TextureQuality.SelectedItem = TranslateNumberToName(settings.TextureQuality);
            CB_EffectsQuality.SelectedItem = TranslateNumberToName(settings.EffectsQuality);
            CB_ShadingQuality.SelectedItem = TranslateNumberToName(settings.ShadingQuality);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        private static Dictionary<string, int> InitSettings()
        {
            return Alias.ListSettingsParams.ToDictionary(name => name, name => -1);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var lines = File.ReadLines(GetPathConf() + "\\GameUserSettings.ini").ToList();

            foreach (ComboBox cb_ in cb)
            {
                for (var i = 0; i < lines.Count(); i++)
                {
                    string temp = lines[i].Split(new char[] { '=' })[0];
                    temp = temp.Replace("sg.", "");
                    if (cb_.Name.Replace("CB_", "") != temp) continue;
                    lines[i] = "sg." + cb_.Name.Replace("CB_", "") + "=" + TransNameToNumber(cb_.SelectedItem.ToString());
                    break;
                }
            }

            File.WriteAllLines(GetPathConf() + "\\GameUserSettings.tmp", lines);
            File.Delete(GetPathConf() + "\\GameUserSettings.ini");
            File.Move(GetPathConf() + "\\GameUserSettings.tmp", GetPathConf() + "\\GameUserSettings.ini");

        }

        public Circular CreateLoadingCircular(string msg)
        {
            Circular circular = Spawn.CircularAdd(msg);
            MainGrid.RegisterName(circular.Name, circular);
            MainGrid.Children.Add(circular);
            MainGrid.IsEnabled = false;
            return circular;
        }

        public void DestroyLoadingCircular()
        {
            Circular circular = (Circular)MainGrid.FindName("dynamicCircular");
            MainGrid.UnregisterName(circular.Name);
            MainGrid.Children.Remove(circular);
            MainGrid.IsEnabled = true;
            SecondGrid.Effect = new BlurEffect { Radius = 0 };
        }

        private async void DownloadKKT_Click(object sender, RoutedEventArgs e)
        {
            var circular = CreateLoadingCircular("Скачивание");
            circular.ChangeMessage("Загрузка", 0);
            await Task.Run(() => Download.DownloadKkt());
            DestroyLoadingCircular();
            
        }

        private void btn_TypeWork_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
