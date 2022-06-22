using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Server
{
    public static class ConvertPacket
    {
        public static string ByteArrayToString(byte[] ba, int cnt)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (int i = 0; i < cnt; i++)
                hex.Append(ba[i].ToString("X2"));
            return hex.ToString();
        }

        public static byte[] stringtobyte(string covertString) // 패킷으로 보내기 ( 03 없이 <-> Encoding.UTF8.GetBytes(string) )
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
