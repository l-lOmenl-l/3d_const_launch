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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для Circular.xaml
    /// </summary>
    public partial class Circular : UserControl
    {
        public Circular(string msg)
        {
            InitializeComponent();
            StartLoadingAnimation(msg);
        }

        public void StartLoadingAnimation(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                tb_message.Visibility = Visibility.Visible;
                tb_message.Text = message;
            }

            sp_main.Visibility = Visibility.Visible;

            Storyboard loadingAnimation = (Storyboard)FindResource("LoadingAnimation");
            loadingAnimation.Begin();
        }

        public void ChangeMessage(string message)
        {
            tb_message.Visibility = Visibility.Visible;
            tb_message.Text = message;
        }

    }
}
