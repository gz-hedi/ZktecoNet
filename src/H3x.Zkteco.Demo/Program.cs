using System;
using H3x.Zkteco;

namespace H3x.Zkteco.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var fpReader = ZktecoUsbFingerprintReader.CreateAndOpenDevice();
        }
    }
}
