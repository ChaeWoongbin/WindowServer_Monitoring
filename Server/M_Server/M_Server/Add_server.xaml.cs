﻿using System;
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

namespace M_Server
{
    /// <summary>
    /// Add_server.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Add_server : Window
    {
        public Add_server()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            target_ip_info info = new target_ip_info() { ip = txtServerIP.Text, name = txtServerName.Text } ;

            Data.target_list.Add(info);
            this.DialogResult = true;
        }
    }
}
