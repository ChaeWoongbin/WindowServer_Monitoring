using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace M_Server
{
    public class Server
    {
        public Socket m_ServerSocket;
        public Dictionary<string, Socket> m_ClientSocket;
        public Dictionary<string, string> m_ClientSocket_value;
        private byte[] szData;

        public string ip_info = "";
        
        public Server(string ip)
        {
            ip_info = ip;
        }

        public void OpenServer()
        {
            m_ClientSocket = new Dictionary<string, Socket>();
            m_ClientSocket_value = new Dictionary<string, string>();
            // N개 소켓 변수
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // InterNetWork ( IPv4 ), Stream( 양방향 연결 기반의 바이트 스트림 ), TCP( TCP 프로토콜 )

            //IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 2020);
            // 모든 IP의 2020포트 연결 ( EndPoint - 네트워크 목적지 (device))
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip_info), 2020);
            // IP의 2020포트 연결 ( EndPoint - 네트워크 목적지 (device))

            m_ServerSocket.Bind(ipep);
            // 엔드포인트 서버소켓 설정
            m_ServerSocket.Listen(20);
            // 수신, 연결 큐 대기연결 수 20

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            //비동기 소켓
            args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            //비동기작업 완료 이벤트
            m_ServerSocket.AcceptAsync(args);
            //비동기작업에 사용할 개체 (args) 설정
        }
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = e.AcceptSocket;

            string ip_ = ((IPEndPoint)ClientSocket.RemoteEndPoint).Address.ToString();
            string port_ = ((IPEndPoint)ClientSocket.RemoteEndPoint).Port.ToString();

            //string[] array = a.Split(new char[] { ':' });
            /*
            if(array[0] != "192.168.0.45")
            {
                SetText(array[0] + " : 허용되지 않은 IP");
                return;
            }
            */

            m_ClientSocket.Add(ip_ + ":" + port_, ClientSocket);
            m_ClientSocket_value.Add(ip_ + ":" + port_, "0");
            // 클라이언트 소켓 Dictionary 추가    소켓번호, 소켓

            if (m_ClientSocket != null)
            {
                try
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    szData = new byte[1024]; // 패킷 크기 1024
                    int bytesRec = ClientSocket.Receive(szData); // 수신 바이트 1024
                    args.SetBuffer(szData, 0, szData.Length); // 버퍼설정

                    ClientSocket.ReceiveAsync(args);

                    args.UserToken = m_ClientSocket;
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
                }
                catch
                {
                    m_ClientSocket[ip_ + ":" + port_].Disconnect(true);
                }
            }
            e.AcceptSocket = null;
            m_ServerSocket.AcceptAsync(e);
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = (Socket)sender;
            if (ClientSocket.Connected && e.BytesTransferred > 0)
            {
                byte[] szData = e.Buffer;    // 데이터 수신

                //string sData = Encoding.UTF8.GetString(szData);
                //sData = sData.Replace("\0", "").Trim();
                //byte[] sDataz = Encoding.UTF8.GetBytes(sData);
                string sData = Encoding.UTF8.GetString(szData, 0, e.BytesTransferred);
                sData += "\n" + ConvertPacket.ByteArrayToString(szData, e.BytesTransferred);

                string sData_point = m_ClientSocket.FirstOrDefault(x => x.Value == ClientSocket).Key.ToString() + " : " + sData;

                if (ConvertPacket.ByteArrayToString(szData, e.BytesTransferred).Substring(0, 4) == "3935")
                {
                    string ip_ = ((IPEndPoint)ClientSocket.RemoteEndPoint).Address.ToString();
                    string port_ = ((IPEndPoint)ClientSocket.RemoteEndPoint).Port.ToString();
                    string str = ConvertPacket.ByteArrayToString(szData, e.BytesTransferred);
                    try
                    {
                        m_ClientSocket_value[ip_ + ":" + port_] = str.Substring(4, 4);
                    }
                    catch { }
                }

                try
                {
                    byte[] sDataz = ConvertPacket.stringtobyte(sData_point);
                    ClientSocket.Send(sDataz, sDataz.Length, SocketFlags.None);
                }
                catch
                {
                    byte[] sDataz = Encoding.UTF8.GetBytes(sData);
                    ClientSocket.Send(sDataz, sDataz.Length, SocketFlags.None);
                }
                for (int i = 0; i < szData.Length; i++)
                {
                    szData[i] = 0;
                }
                e.SetBuffer(szData, 0, e.Buffer.Length);
                ClientSocket.ReceiveAsync(e);
            }
            else
            {
                try
                {
                    ClientSocket.Disconnect(false);
                    ClientSocket.Dispose();
                    m_ClientSocket_value.Remove(m_ClientSocket.FirstOrDefault(x => x.Value == ClientSocket).Key);
                    m_ClientSocket.Remove(m_ClientSocket.FirstOrDefault(x => x.Value == ClientSocket).Key);
                }
                catch
                {

                }
            }
        }

        public bool SendMessage(string key, string msg)
        {
            try
            {
                string test_packet = msg;
                byte[] sDataz;
                sDataz = ConvertPacket.stringtobyte(test_packet);

                try
                {
                    m_ClientSocket[key].Send(sDataz, sDataz.Length, SocketFlags.None);
                }
                catch
                {
                    return false;
                }
                Console.WriteLine(key + " : " + test_packet);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return false; }
            return true;
        }
    }
}
