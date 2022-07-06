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
        Server Server = new Server("192.168.1.137");
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

                set_clients(); // 연결된 클라이언트 수
            }
           );
        }

        List<string> old_clients = new List<string>();
        List<string> new_clients = new List<string>();
        private void set_clients() // 클라이언트 확인후 리스트 재 작성
        {
            foreach (string list in Server.m_ClientSocket.Keys)
            {
                new_clients.Add(list);
            }

            if (old_clients.SequenceEqual(new_clients))
            {

            }
            else // Client 변경 감지 
            {
                Data.Client_list.Clear();
                foreach (string list in new_clients)
                {
                    Data.Client_list.Add(list);
                }

                Clients_list.Items.Clear();
                foreach (string list in Data.Client_list)
                {
                    Clients_list.Items.Add(list);
                }
            }

            old_clients = new List<string>(new_clients);
            new_clients.Clear();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Add_server new_server = new Add_server();
            new_server.ShowDialog();

            string result = "";
            foreach(target_ip_info a in Data.target_list){
                result += a.ip;
            }

            MessageBox.Show(result);
        }
    }
}
