using Newtonsoft.Json.Linq;
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
    /// Логика взаимодействия для Progress_Download.xaml
    /// </summary>
    public partial class Progress_Download : Window
    {
        public Progress_Download()
        {
            InitializeComponent();
        }

        public async void SetProgress(float value)
        {
            await Task.Run(() => Dispatcher.Invoke(() => progress.Value = value));
        }
    }
}
