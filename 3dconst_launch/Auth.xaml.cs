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
                circular.ChangeMessage("Отправляю запрос");
                var result = await client.GetAsync("http://127.0.0.1:8000/login_launcher");
                

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:

                        string responseBody = await result.Content.ReadAsStringAsync();

                        var json = JsonConvert.DeserializeObject<user>(responseBody);
                        if (json.superuser)
                        {
                            refWelcome.LabelWelcome.Content = "Добро пожаловать " + tb_login.Text + "!";
                            refWelcome.CB_Type.Items.Clear();
                            foreach (var item in Alias.AliasIp)
                            {
                                refWelcome.CB_Type.Items.Add(item.Value);
                            }
                            this.Close();
                            break;
                        }
                        else
                        {
                            MainGrid.Children.Remove(circular);
                            MainGrid.IsEnabled = true;
                            FirstGrid.Effect = new BlurEffect { Radius = 0 };
                            TB_Error.Text = "Вы не являетесь администратором!";
                            break;
                        }


                    case HttpStatusCode.Unauthorized:
                        MainGrid.Children.Remove(circular);
                        FirstGrid.Effect = new BlurEffect { Radius = 0 };
                        MainGrid.IsEnabled = true;
                        TB_Error.Text = "Логин или пароль указан неверно!";
                        break;

                    default:
                        MainGrid.Children.Remove(circular);
                        FirstGrid.Effect = new BlurEffect { Radius = 0 };
                        MainGrid.IsEnabled = true;
                        TB_Error.Text = "Неизвестная ошибка!";
                        break;
                }
            }
            else
            {
                MainGrid.Children.Remove(circular);
                FirstGrid.Effect = new BlurEffect { Radius = 0 };
                MainGrid.IsEnabled = true;
                TB_Error.Text = "Логин или пароль не введен!";
            }
        }
       

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
