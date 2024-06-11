using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Interop;
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

        List<ComboBox> ref_CB = new List<ComboBox>();
        Dictionary<string, int> UploadSettings = new Dictionary<string, int>();

        public string getPathConf()
        {
            return "C:\\Users\\User\\AppData\\Local\\E1\\Saved\\Config\\WindowsNoEditor";
        }

        private void Init()
        {
            var ip = config.getIP();
            foreach (var items in Alias.alias_IP)
            {
                CB_IP.Items.Add(items.Value);
                if (ip == items.Key)
                {

                    CB_IP.SelectionChanged -= CB_IP_SelectionChanged;
                    CB_IP.SelectedItem = items.Value;
                    CB_IP.SelectionChanged += CB_IP_SelectionChanged;

                }
            }



            var lines = File.ReadLines(getPathConf() + "\\GameUserSettings.ini");
            foreach (KeyValuePair<string, int> entry in initSettings())
            {
                string result = string.Join("\n",
                lines.Where(s => s.IndexOf(entry.Key, StringComparison.InvariantCultureIgnoreCase) >= 0));
                string[] words = result.Split(new char[] { '=' });
                UploadSettings.Add(entry.Key, Convert.ToInt32(words[1]));
            }
            Console.WriteLine("");

            foreach (var x in UploadSettings.Select((Entry, Index) => new { Entry, Index }))
            {


                Grid grd = new Grid();
                grd.SetValue(Grid.RowProperty, x.Index + 3);
                ColumnDefinition c1 = new ColumnDefinition();
                c1.Width = new GridLength(130, GridUnitType.Star);
                ColumnDefinition c2 = new ColumnDefinition();
                c2.Width = new GridLength(150, GridUnitType.Star);
                grd.ColumnDefinitions.Add(c1);
                grd.ColumnDefinitions.Add(c2);

                grd.Children.Add((Spawn.labelAdd(x.Entry.Key)));
                ref_CB.Add(Spawn.ComboBoxAdd(x.Entry.Value, x.Entry.Key));
                grd.Children.Add(ref_CB.Last());
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

        public Dictionary<string, int> initSettings()
        {
            Dictionary<string, int> settings = new Dictionary<string, int>();
            foreach (var name in Alias.listSettingsParams)
            {
                settings.Add(name, -1);
            }
            return settings;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newSettings = new Dictionary<string, int>();
            foreach (var name in ref_CB)
            {
                newSettings.Add(name.Name, name.SelectedIndex);
            }

            var lines = File.ReadLines(getPathConf() + "\\GameUserSettings.ini").ToList();
            foreach (KeyValuePair<string, int> newline in newSettings)
            {
                for (int i = 0; i < lines.Count(); i++)
                {
                    string temp = lines[i].Split(new char[] { '=' })[0];
                    temp = temp.Replace("sg.", "");
                    if (newline.Key == temp)
                    {
                        Console.WriteLine(newline.Key + "==" + temp);
                        lines[i] = "sg." + newline.Key + "=" + newline.Value;
                        break;
                    }
                }
            }
            File.WriteAllLines(getPathConf() + "\\GameUserSettings.tmp", lines);
            File.Delete(getPathConf() + "\\GameUserSettings.ini");
            File.Move(getPathConf() + "\\GameUserSettings.tmp", getPathConf() + "\\GameUserSettings.ini");

        }

        private void CB_IP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var Result = MessageBox.Show("sure?", "Change IP?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Result == MessageBoxResult.Yes)
            {
                string new_ip = "";
                foreach(KeyValuePair<string, string> item in Alias.alias_IP)
                {
                    if (item.Value == CB_IP.SelectedItem.ToString())
                    {
                        new_ip = item.Key;
                    }
                }    
                File.Delete(config.GetPathConfig(true));
                config.createConf(new_ip);
                var path = System.Reflection.Assembly.GetEntryAssembly().Location;
                Process proc = new Process();
                proc.StartInfo.FileName = path.Remove(path.LastIndexOf("\\")) + "/3dconst_launch.exe";
                proc.Start();
                System.Windows.Application.Current.Shutdown();
            }
            else if (Result == MessageBoxResult.No)
            {

            }
        }

        private void DownloadKKT_Click(object sender, RoutedEventArgs e)
        {
            download.DownloadKKT();
        }
    }
}
