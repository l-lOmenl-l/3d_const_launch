using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        const string appName = "3dconst_launch";
        public Welcome()

        {   Mutex _mutex;
            
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // Если приложение уже запущено, выходим
                MessageBox.Show("Приложение уже запущено.");
                this.Close();
            }

            InitializeComponent();
            CB_Type.Items.Add(Alias.AliasIp["https://3d.e-1.ru:8000"]);
            //CB_Type.Items.Add(Alias.AliasIp["https://3d-test.e-1.ru:8000"]);
            CB_Type.SelectedItem = Alias.AliasIp["https://3d.e-1.ru:8000"];
        }

        private async void Btn_Apply_ClickAsync(object sender, RoutedEventArgs e)
        {   
            foreach (var item in Alias.AliasIp)
            {
                if (CB_Type.SelectedItem.ToString() == item.Value)
                {
                    var Main = new MainWindow(item.Key);

                    this.Close();
                    break;
                }
            }
            return;
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
