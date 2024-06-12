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
        public static Label LabelAdd(string name)
        {
            var label = new Label
            {
                Foreground = new SolidColorBrush(Colors.White)
            };
            Alias.AliasLabel.TryGetValue(name, out var temp);
            label.Content = temp + ":";
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.SetValue(Grid.ColumnProperty, 0);
            return label;
        }

        public static ComboBox ComboBoxAdd(int param, string name)
        {
            var combobox = new ComboBox
            {
                Name = name,
                Width = 140,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            combobox.SetValue(Grid.ColumnProperty, 1);
            foreach (var item in Alias.AliasSettingsParam)
            {
                combobox.Items.Add(item.Value);
            }
            Alias.AliasSettingsParam.TryGetValue(param, out var temp);
            combobox.SelectedItem = temp;
            return combobox;
        }
    }
}
