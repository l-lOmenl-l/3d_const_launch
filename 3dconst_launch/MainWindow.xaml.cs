using DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.RightsManagement;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static _3dconst_launch.config;
using static System.Net.WebRequestMethods;

namespace _3dconst_launch
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;
            MD5 md5 = MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(System.IO.File.ReadAllBytes(path))).Replace("-", "").ToLower();
            init();
        }


        public async void init()
        {
            await Task.Run(() => config.init(this));

            //CheckDiffPath();
        }

        public async void ChangeProgressBatAsync(float value)
        {
            await Task.Run(() => Dispatcher.Invoke(() => ProgressUpload.Value = value));
        }

        public async void ChangeProgressBarVisibility(Visibility value)
        {
            await Task.Run(() => Dispatcher.Invoke(() => ProgressUpload.Visibility = value));
        }

        public async void changeMessageAsync(string msg)
        {
            await Task.Run(() => Dispatcher.Invoke(() => label_message.Content = msg));
        }

        public async void btnPlay()
        {
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.IsEnabled = true));
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.Content = "Запустить"));
        }

        public async void btnUpdate()
        {
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.IsEnabled = true));
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.Content = "Обновление"));
        }

        public async void btnDownload()
        {
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.IsEnabled = true));
            await Task.Run(() => Dispatcher.Invoke(() => Btn_download.Content = "Загрузить"));
        }

        private async void Btn_download_Click(object sender, RoutedEventArgs e)
        { 
            switch (Btn_download.Content)
            {
                case "Загрузить":
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => download.DownloadConstruct(this, FilesData.GetServerPath()));
                    break;

                case "Обновление":
                    Btn_download.IsEnabled = false;
                    await Task.Run(() => download.DownloadConstruct(this, FilesData.checkDiffFiles()));
                    break;

                case "Запустить":
                    Process proc = new Process();
                    proc.StartInfo.FileName = config.conf.Path_const + "/3dconst.exe";
                    proc.Start();
                    break;
            }



        }

        public void const_actual()
        {
            label_message.Content = "Actual";
        }

        public void const_update()
        {
            label_message.Content = "update";
        }

        public void const_download()
        {
            label_message.Content = "Готов к загрузке";
            Btn_download.Content = "Загрузить";
        }

        public void ChangeMessage(string msg)
        {
            label_message.Content = msg;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
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
            System.Windows.Application.Current.Shutdown();
        }



    }
}
