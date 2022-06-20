using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace M_Client
{
    /// <summary>
    /// CPU : CPU 사용량
    /// Memory : 메모리 사용량
    /// drive : 여유용량
    /// connect : 연결확인
    /// </summary>
    class ClientInfo
    {
        // 성능 기본정보
        private PerformanceCounter cpuCounter;
        private PerformanceCounter memCounter;
        public DriveInfo storageCheck;


        public string CPU = "";
        public string Memory = "";
        public string Memory_percent = "";
        public string drive = "";
        public bool connect = false;

        public void GetData()
        {
            GetCPU();
            GetMemory();
            Getdrive();
            Getconnect();
        }


        private void GetCPU()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            CPU = cpuCounter.NextValue().ToString();
        }
        private void GetMemory()
        {
            memCounter = new PerformanceCounter("Memory", "Available MBytes");
            Memory = ((Convert.ToDouble(memCounter.NextValue())) / 1024).ToString("N2");
            Memory_percent = ram_percentage(RAM_Check()).ToString("N2") + "%";
        }
        private void Getdrive()
        {
            storageCheck = new DriveInfo(System.IO.Directory.GetCurrentDirectory().ToString());
            drive = ((double)storageCheck.AvailableFreeSpace / (1024 * 1024 * 1024)).ToString("N2");
        }
        private void Getconnect()
        {
            connect = false;
        }



        private double ram_percentage(double ram_check) // 램 계산
        {
            double percent = 0;

            double now = Convert.ToDouble(((RAM_Check() / 1024 / 1024) - ((Convert.ToDouble(memCounter.NextValue())) / 1024)).ToString());
            double max = Double.Parse((ram_check / 1024 / 1024).ToString("N2"));

            percent = (now / max) * 100;


            return percent;
        }


        private double RAM_Check() // 전체램 확인용
        {
            double test = 0;
            try
            {
                ManagementClass cls = new ManagementClass("Win32_OperatingSystem");
                ManagementObjectCollection instances = cls.GetInstances();

                foreach (ManagementObject info in instances)
                {
                    test = double.Parse(info["TotalVisibleMemorySize"].ToString());
                }

                return test;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
    }
}
