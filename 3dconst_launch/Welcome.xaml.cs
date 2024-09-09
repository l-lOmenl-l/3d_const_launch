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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();

            CB_Type.Items.Add(Alias.AliasIp["https://3d.e-1.ru:8000"]);
            CB_Type.Items.Add(Alias.AliasIp["https://3d-test.e-1.ru:8000"]);
            CB_Type.Items.Add(Alias.AliasIp["http://127.0.0.1:8000"]);
            CB_Type.SelectedItem = Alias.AliasIp["http://127.0.0.1:8000"];
        }

        private void Btn_Apply(object sender, RoutedEventArgs e)
        {            
            foreach (var item in Alias.AliasIp)
            {
                if (CB_Type.SelectedItem.ToString() == item.Value) {
                    var Main = new MainWindow(item.Key);
                    Main.Show();
                    this.Close();
                    break;
                }
            }
            
        }


        private void Btn_LoginAdmin(object sender, RoutedEventArgs e)
        {
            var login = new Auth(this);
            login.ShowDialog();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();


    }
}
