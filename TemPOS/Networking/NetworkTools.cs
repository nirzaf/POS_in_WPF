using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

namespace TemPOS.Networking
{
    public static class NetworkTools
    {
        #region Licensed Access Only
        static NetworkTools()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(NetworkTools).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
        }
        #endregion

        public static int? GetLastLanByte()
        {
            // Lookup from DNS
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress[] IpA = hostEntry.AddressList;
            foreach (IPAddress t in IpA)
            {
                if (!IsLanAddress(t)) continue;
                byte[] address = t.GetAddressBytes();
                return address[3];
            }

            // Lookup from network interfaces
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nics)
            {
                foreach (UnicastIPAddressInformation x in ni.GetIPProperties().UnicastAddresses)
                {
                    if (x.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (IsLanAddress(x.Address))
                        {
                            byte[] address = x.Address.GetAddressBytes();
                            return address[3];
                        }
                    }
                }
            }
            return null;
        }

        public static bool IsLanAddress(IPAddress ipAddress)
        {
            // 10.0.0.0 – 10.255.255.255
            // 172.16.0.0 – 172.31.255.255
            // 192.168.0.0 – 192.168.255.255
            byte[] address = ipAddress.GetAddressBytes();
            if (((int)address[0] != 10) &&
                ((int)address[0] != 172) &&
                ((int)address[0] != 192))
                return false;
            if (((int)address[0] == 172) &&
                (((int)address[1] < 16) || ((int)address[1] > 31)))
                return false;
            if (((int)address[0] == 192) &&
                ((int)address[1] != 168))
                return false;
            return true;
        }

    }
}
