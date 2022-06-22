using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace M_Server
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Server Server = new Server("127.0.0.1");
        private static System.Timers.Timer aTimer;

        public MainWindow()
        {
            InitializeComponent();
            Server.OpenServer();

            // 1초의 interval을 둔 timer 만들기

            aTimer = new System.Timers.Timer(500);



            // 쓰레드 추가

            aTimer.Elapsed += realtime;

            aTimer.Enabled = true;
        }


        private void realtime(object sender, ElapsedEventArgs e) // 실시간 값입력 타이머
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                double result = 0;
                foreach (string list in Server.m_ClientSocket_value.Values)
                {
                    result += Convert.ToDouble(list);
                }
                lbl.Content = result.ToString("F2"); // 소수점 2자리
            }
           );
        }
    }
}
