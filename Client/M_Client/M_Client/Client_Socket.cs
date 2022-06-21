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


namespace M_Client
{
    class Client_Socket
    {
        public AsyncCallback m_fnReceiveHandler;
        public AsyncCallback m_fnSenderHandler;
        public Socket m_ClientSocket = null;

        bool socket_state = false;

        int time = 0;

        //private static System.Timers.Timer aTimer;

        public string CPU_use;

        public Client_Socket()
        {
            try
            {
                ConnectToServer("127.0.0.1", "2020");
            }
            catch
            {
                Console.WriteLine("연결 실패");
            }
            // 10초의 interval을 둔 timer 만들기

            //aTimer = new System.Timers.Timer(10000);



            // 쓰레드 추가

            //aTimer.Elapsed += realtime;

            //aTimer.Enabled = true;
        }


        //private void realtime(object sender, ElapsedEventArgs e)
        //{            
        //    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
        //    {
        //        //send_packet("393500" + value.Text);
        //        AsyncObject ao = new AsyncObject(1);
        //        ao.Buffer = stringtobyte("3935" + CPU_use.PadLeft(4, '0'));
        //        ao.WorkingSocket = m_ClientSocket;
        //        try
        //        {
        //            m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnSenderHandler, ao);
        //        }
        //        catch (Exception ex)
        //        {
        //        }

        //        time++;
        //    });
        //}


        public void ConnectToServer(string ip, string port)
        {
            string IP = ip;
            int Port = Convert.ToInt16(port);

            m_fnReceiveHandler = new AsyncCallback(handleDataReceive);
            try
            {
                m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);

                IAsyncResult result = m_ClientSocket.BeginConnect(IP, Port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(5, true);

                if (success)
                {
                    if (m_ClientSocket.Connected)
                    {
                        socket_state = true;
                    }
                    else
                    {
                        socket_state = false;
                    }
                }
                else
                {
                    socket_state = false;
                }
            }
            catch
            {
                m_ClientSocket.Close();
            }

            if (m_ClientSocket.Connected)
            {
                AsyncObject ao = new AsyncObject(1024);
                ao.WorkingSocket = m_ClientSocket;

                m_ClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
            }
        }


        private void handleDataReceive(IAsyncResult ar)
        {

        }

        public class AsyncObject
        {
            public Byte[] Buffer;
            public Socket WorkingSocket;
            public AsyncObject(Int32 bufferSize)
            {
                this.Buffer = new Byte[bufferSize];
            }
        }


        private void send_packet(string msg) // 패킷으로 전송하기
        {
            AsyncObject ao = new AsyncObject(1);
            ao.Buffer = stringtobyte(msg);
            ao.WorkingSocket = m_ClientSocket;
            try
            {
                m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnSenderHandler, ao);
            }
            catch (Exception ex)
            {

            }
        }
        public static byte[] stringtobyte(string covertString)
        {
            byte[] convertArr = new byte[covertString.Length / 2];

            for (int i = 0; i < convertArr.Length; i++)
            {
                convertArr[i] = Convert.ToByte(covertString.Substring(i * 2, 2), 16);
            }
            return convertArr;
        }
    }
}
