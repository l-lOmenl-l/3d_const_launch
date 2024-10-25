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

        public static Label InfoLabelAdd(string message)
        {
            var label = new Label
            {
                Foreground = new SolidColorBrush(Colors.White)
            };

            label.Content = message;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            return label;
        }

        public struct parametersButton{
            public string msg {  get; set; }
            public int width { get; set; }
            public int heigth { get; set; }

            public int FontSize { get; set; }

            public HorizontalAlignment horizontalAligment { get; set; }
            public VerticalAlignment verticalAlignment { get; set;}

            public RoutedEventHandler RoutedEvent { get; set; }

        }

        public static Button ButtonAdd(parametersButton param)
        {
            var button = new Button
            {
                Name = "DynamicButton",
                Content = param.msg,
                FontSize = param.FontSize,
                Width = param.width,
                Height = param.heigth,
                HorizontalAlignment = param.horizontalAligment,
                VerticalAlignment = param.verticalAlignment 
            };
            button.Click += param.RoutedEvent;
            
            return button;
        }


        public static ComboBox ComboBoxAdd(int param, string name)
        {
            var combobox = new ComboBox
            {
                Name = name,
                Width = 140,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };


            Style customStyle = (Style)Application.Current.FindResource("ComboBoxStyle1");

            // Применение стиля к ComboBox
            combobox.Style = customStyle;
            combobox.HorizontalContentAlignment = HorizontalAlignment.Center;
            combobox.VerticalContentAlignment = VerticalAlignment.Center;
            combobox.SetValue(Grid.ColumnProperty, 1);
            foreach (var item in Alias.AliasSettingsParam)
            {
                combobox.Items.Add(item.Value);
            }
            Alias.AliasSettingsParam.TryGetValue(param, out var temp);
            combobox.SelectedItem = temp;
            return combobox;
        }

        public static Circular CircularAdd(string msg)
        {
            Circular circular = new Circular(msg);
            circular.Name = "dynamicCircular";
            circular.HorizontalAlignment = HorizontalAlignment.Stretch;
            circular.VerticalAlignment = VerticalAlignment.Stretch;
            circular.SetValue(Grid.RowSpanProperty, 2);
            return circular;
        }
    }
}
