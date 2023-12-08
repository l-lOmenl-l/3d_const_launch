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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        static System.Windows.Controls.Label ui_TB_Message;

        public MainWindow()
        {
            InitializeComponent();
            ui_TB_Message = TB_Message;
            TB_Message.Content = "Start";
            config.init();
            
        }

        public static void const_actual()
        {
            ui_TB_Message.Content = "Actual";
        }

        public static void const_update()
        {
            ui_TB_Message.Content = "update";
        }

        public static void const_download()
        {
            ui_TB_Message.Content = "download";
        }

        static public void ChangeMessage(string msg)
        {
            ui_TB_Message.Content = msg;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton==MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
