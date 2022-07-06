using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Server
{
    public class Data
    {
        public static List<target_ip_info> target_list = new List<target_ip_info>();

        public static List<string> Client_list = new List<string>();
    }

    public class target_ip_info{
        public string ip { get; set; }
        public string name { get; set; }
    }
}
