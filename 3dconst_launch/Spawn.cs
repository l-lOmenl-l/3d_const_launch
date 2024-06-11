using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace _3dconst_launch
{
    internal class Spawn
    {
        public static Label labelAdd(string name)
        {
            Label label = new Label();
            label.Foreground = new SolidColorBrush(Colors.White);
            string temp = "";
            Alias.alias_label.TryGetValue(name, out temp);
            label.Content = temp + ":";
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.SetValue(Grid.ColumnProperty, 0);
            return label;
        }

        public static ComboBox ComboBoxAdd(int param, string name)
        {
            ComboBox combobox = new ComboBox();
            combobox.Name = name;
            combobox.Width = 140;
            combobox.Height = 20;
            combobox.HorizontalAlignment = HorizontalAlignment.Center;
            combobox.VerticalAlignment = VerticalAlignment.Center;
            combobox.SetValue(Grid.ColumnProperty, 1);
            foreach (KeyValuePair<int, string> item in Alias.alias_Settings_Param)
            {
                combobox.Items.Add(item.Value);
            }
            string temp = "";
            Alias.alias_Settings_Param.TryGetValue(param, out temp);
            combobox.SelectedItem = temp;
            return combobox;
        }
    }
}
