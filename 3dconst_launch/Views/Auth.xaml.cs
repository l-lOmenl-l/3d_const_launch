using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        Welcome refWelcome;

        public Auth(Welcome ref_)
        {
            InitializeComponent();
            refWelcome = ref_;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public struct auth
        {
            public string login { get; set; }
            public string password { get; set; }

            public auth(string login, string password) {
                this.login = login;
                this.password = password;
            }
        }



        private struct user
        {
            public bool superuser {get; set;}
        }


        private void ChangeWarning(string message, MaterialDesignThemes.Wpf.PackIconKind icon, SolidColorBrush color) 
        {
            sp_warning.Visibility = Visibility.Visible;
            tb_warning.Text = message;
            icon_warning.Kind = icon;
            icon_warning.Foreground = color;
        }


        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.IsEnabled = false;
            FirstGrid.Effect = new BlurEffect { Radius = 10 };

            Circular circular = Spawn.CircularAdd(null);
            circular.SetValue(Grid.RowSpanProperty, 2);
            MainGrid.Children.Add(circular);

            var check_login = tb_login.Text.Length > 0 ? true : false;
            var check_pass = tb_pass.Password.Length > 0 ? true : false;

            if (check_login & check_pass)
            {
                using var client = new HttpClient();
                var authToken = Encoding.ASCII.GetBytes($"{tb_login.Text}:{tb_pass.Password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                circular.ChangeMessage("Отправляю запрос", 0);
                // TODO: Изменить адрес аутентификации при деплое в прод
                var result = await client.GetAsync("https://3d.e-1.ru:8000/login_launcher");
                //var result = await client.GetAsync("http://127.0.0.1:8000/login_launcher");

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:

                        string responseBody = await result.Content.ReadAsStringAsync();

                        var json = JsonConvert.DeserializeObject<user>(responseBody);
                        if (json.superuser)
                        {
                            refWelcome.CB_Type.Items.Clear();
                            foreach (var item in Alias.AliasIp)
                            {
                                refWelcome.CB_Type.Items.Add(item.Value);
                            }
                            refWelcome.CB_Type.SelectedIndex = 0;
                            this.Close();
                            break;
                        }
                        else
                        {
                            MainGrid.Children.Remove(circular);
                            MainGrid.IsEnabled = true;
                            FirstGrid.Effect = new BlurEffect { Radius = 0 };
                            ChangeWarning("Вы не являетесь администратором!", MaterialDesignThemes.Wpf.PackIconKind.Alert, new SolidColorBrush(Colors.Yellow));
                            break;
                        }


                    case HttpStatusCode.Unauthorized:
                        MainGrid.Children.Remove(circular);
                        FirstGrid.Effect = new BlurEffect { Radius = 0 };
                        MainGrid.IsEnabled = true;
                        ChangeWarning("Логин или пароль указан неверно!", MaterialDesignThemes.Wpf.PackIconKind.MinusCircleOutline, new SolidColorBrush(Colors.Yellow));
                        break;

                    default:
                        MainGrid.Children.Remove(circular);
                        FirstGrid.Effect = new BlurEffect { Radius = 0 };
                        MainGrid.IsEnabled = true;
                        ChangeWarning("Неизвестная ошибка!", MaterialDesignThemes.Wpf.PackIconKind.Alien, new SolidColorBrush(Colors.LightGreen));
                        break;
                }
            }
            else
            {
                MainGrid.Children.Remove(circular);
                FirstGrid.Effect = new BlurEffect { Radius = 0 };
                MainGrid.IsEnabled = true;
                ChangeWarning("Логин или пароль не введен!", MaterialDesignThemes.Wpf.PackIconKind.Alert, new SolidColorBrush(Colors.Red));
            }
        }
       

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
