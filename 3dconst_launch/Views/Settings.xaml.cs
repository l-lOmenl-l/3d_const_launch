using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            Init();
        }

        private readonly List<ComboBox> _refCb = new List<ComboBox>();
        private readonly Dictionary<string, int> _uploadSettings = new Dictionary<string, int>();

        private static string GetPathConf()
        {
            return "C:\\Users\\User\\AppData\\Local\\E1\\Saved\\Config\\WindowsNoEditor";
        }

        private void Init()
        {
            var ip = Config.GetIp();

            var lines = File.ReadLines(GetPathConf() + "\\GameUserSettings.ini");
            foreach (KeyValuePair<string, int> entry in InitSettings())
            {
                string result = string.Join("\n",
                lines.Where(s => s.IndexOf(entry.Key, StringComparison.InvariantCultureIgnoreCase) >= 0));
                string[] words = result.Split(new char[] { '=' });
                _uploadSettings.Add(entry.Key, Convert.ToInt32(words[1]));
            }
            Console.WriteLine("");

            foreach (var x in _uploadSettings.Select((entry, index) => new { Entry = entry, Index = index }))
            {
                var grd = new Grid();
                grd.SetValue(Grid.RowProperty, x.Index + 3);
                
                var c1 = new ColumnDefinition
                {
                    Width = new GridLength(130, GridUnitType.Star)
                };
                
                var c2 = new ColumnDefinition
                {
                    Width = new GridLength(150, GridUnitType.Star)
                };

                grd.ColumnDefinitions.Add(c1);
                grd.ColumnDefinitions.Add(c2);

                grd.Children.Add((Spawn.LabelAdd(x.Entry.Key)));
                _refCb.Add(Spawn.ComboBoxAdd(x.Entry.Value, x.Entry.Key));
                grd.Children.Add(_refCb.Last());
                StackSettings.Children.Add(grd);
            }

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
            var newSettings = _refCb.ToDictionary(name => name.Name, name => name.SelectedIndex);

            var lines = File.ReadLines(GetPathConf() + "\\GameUserSettings.ini").ToList();
            foreach (KeyValuePair<string, int> newline in newSettings)
            {
                for (var i = 0; i < lines.Count(); i++)
                {
                    string temp = lines[i].Split(new char[] { '=' })[0];
                    temp = temp.Replace("sg.", "");
                    if (newline.Key != temp) continue;
                    Console.WriteLine(newline.Key + "==" + temp);
                    lines[i] = "sg." + newline.Key + "=" + newline.Value;
                    break;
                }
            }
            File.WriteAllLines(GetPathConf() + "\\GameUserSettings.tmp", lines);
            File.Delete(GetPathConf() + "\\GameUserSettings.ini");
            File.Move(GetPathConf() + "\\GameUserSettings.tmp", GetPathConf() + "\\GameUserSettings.ini");

        }
        /*
        private void CB_IP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены?", "Изменить IP?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                {
                    string newIp = "";
                    foreach (var item in Alias.AliasIp.Where(item => item.Value == CB_IP.SelectedItem.ToString()))
                    {
                        newIp = item.Key;
                    }    
                    File.Delete(Config.GetPathConfig(true));
                        
                    //Config.CreateConf(newIp);
                    var path = System.Reflection.Assembly.GetEntryAssembly()?.Location;
                    var proc = new Process();
                    proc.StartInfo.FileName = path?.Remove(path.LastIndexOf("\\", StringComparison.Ordinal)) + "/3dconst_launch.exe";
                    proc.Start();
                    System.Windows.Application.Current.Shutdown();
                    break;
                }
                case MessageBoxResult.No:
                    break;
            }
        }
        */
        private void DownloadKKT_Click(object sender, RoutedEventArgs e)
        {
            Download.DownloadKkt();
        }
    }
}
