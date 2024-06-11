using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class SetIP : Window
    {
        public SetIP()
        {
            InitializeComponent();

            foreach (KeyValuePair<string, string> items in Alias.alias_IP) 
            {
                IP_ComboBox.Items.Add(items.Value);
            }
            
        }

        private void Apply_Click(object sender, RoutedEventArgs e) 
        {
            
            this.Close();
        }
    }
}
