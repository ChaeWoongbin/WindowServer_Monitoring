using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using static M_Client.Client_Socket;

namespace M_Client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // 반복
        private DispatcherTimer Timer;
        ClientInfo Client = new ClientInfo();
        Client_Socket s_Client = new Client_Socket();

        private PerformanceCounter cpuCounter;
        public MainWindow()
        {
            InitializeComponent();
            SetProgram(); // 프로그램 실행
        }

        // 프로그램 실행 값
        private void SetProgram()
        {
            // 트레이
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            try
            {
                ni.Icon = new System.Drawing.Icon("monitor.ico");
                ni.Visible = true;
                ni.DoubleClick += delegate (object senders, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    this.ShowInTaskbar = true;
                };
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            // 클라이언트 정보
            try { cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"); Client.GetData(); Client.GetCPU(); }
            catch { MessageBox.Show("(couCounter Error) starting restore Regi..."); CMD("lodctr /r"); } // 성능 카운터 레지스트리 초기화

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 1, 0); // 10초당 타이머 실행
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }


        public void CMD(string command) // 명령프롬프트 실행
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.Verb = "runas";
            cmd.Start();
            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit(1);
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }      

        private void Timer_Tick(object sender, EventArgs e)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                try { Client.GetData(); }
                catch { MessageBox.Show("(couCounter Error) starting restore Regi..."); CMD("lodctr /r"); } // 성능 카운터 레지스트리 초기화

                cpu_value.Text = (int)cpuCounter.NextValue() + "";
                lbl_CPU_value.Content = cpu_value.Text + " %";
                lbl_Memory_value.Content = Client.Memory.ToString() + " / " + Client.Memory_Max + " GB" + " ( " + Client.Memory_percent + " ) ";
                lbl_drive_value.Content = Client.drive.ToString() + " GB";
                //lbl_Server_value.Content = Client.connect.ToString();
                                
                s_Client.CPU_use = cpu_value.Text;


                //send_packet("393500" + value.Text);
                AsyncObject ao = new AsyncObject(1);
                ao.Buffer = stringtobyte("3935" + s_Client.CPU_use.ToString().PadLeft(4, '0'));
                ao.WorkingSocket = s_Client.m_ClientSocket;
                try
                {
                    s_Client.m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, s_Client.m_fnSenderHandler, ao);
                }
                catch (Exception ex)
                {
                }
            });
        }

        //윈도우 트레이
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Tray_Event();
            e.Cancel = true;
            return;
        }

        // esc 입력시 트레이 | q 입력시 종료
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape: Tray_Event(); break;
                case Key.Q: Application.Current.Shutdown(); break;
            }
        }

        // 트레이 이동
        void Tray_Event()
        {
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            s_Client.m_ClientSocket.Close();
            s_Client = new Client_Socket();
        }
    }
}
