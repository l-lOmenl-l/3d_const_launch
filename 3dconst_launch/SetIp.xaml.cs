using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using static System.Reflection.Assembly;


namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class SetIp : Window
    {
        public SetIp()
        {
            InitializeComponent();

            foreach (KeyValuePair<string, string> items in Alias.AliasIp) 
            {
                IpComboBox.Items.Add(items.Value);
            }
            
        }

        private void Apply_Click(object sender, RoutedEventArgs e) 
        {
            string newIp = "";
            foreach (var item in Alias.AliasIp.Where(item => item.Value == IpComboBox.SelectedItem.ToString()))
            {
                newIp = item.Key;
            }    
            File.Delete(Config.GetPathConfig(true));
            Config.CreateConf(newIp);
            var path = GetEntryAssembly()?.Location;
            Process proc = new Process();
            proc.StartInfo.FileName = path?.Remove(path.LastIndexOf("\\", StringComparison.Ordinal)) + "/3dconst_launch.exe";
            proc.Start();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
