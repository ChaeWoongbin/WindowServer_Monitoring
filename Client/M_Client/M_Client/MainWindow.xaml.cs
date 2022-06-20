using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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
using System.Windows.Threading;

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


        public MainWindow()
        {
            InitializeComponent();
            SetProgram(); // 프로그램 실행
        }

        private void SetProgram()
        {
            try { Client.GetData(); }
            catch { MessageBox.Show("(couCounter Error) starting restore Regi..."); CMD("lodctr /r"); } // 성능 카운터 레지스트리 초기화

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 10, 0); // 10초당 타이머 실행
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
            try { Client.GetData(); }
            catch { MessageBox.Show("(couCounter Error) starting restore Regi..."); CMD("lodctr /r"); } // 성능 카운터 레지스트리 초기화
            test.Text = Client.Memory + "%";
        }
    }
}
