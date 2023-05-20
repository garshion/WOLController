using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Bass.Util.WOL
{
    public class WOLSender : IDisposable
    {
        public const int MAC_ADDRESS_BYTE_ARRAY_SIZE = 6;
        public const int MAGIC_PACKET_LENGTH = 102;
        public const int MAC_ADDRESS_REPEAT_COUNT = 16;

        public const int DEFAULT_WOL_PORT = 9;

        private UdpClient mSocket = new UdpClient();

        private string mLastError = string.Empty;



        public void Dispose()
        {
            mSocket.Close();
            mSocket.Dispose();

        }

        public bool Wake(string macAddress, int port = DEFAULT_WOL_PORT)
        {
            if (port < 0
                || port > ushort.MaxValue)
                return false;   // invalid port range

            if (null == mSocket)
                return false;   // Socket is null

            var packetData = _MakePacket(macAddress);
#if NETFRAMEWORK
            if (null == packetData)
                return false;
#else
            if(packetData.Length == 0)
                return false;
#endif

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), port);
            try
            {
                mSocket.Send(packetData, packetData.Length, endPoint);
            }
            catch (Exception e)
            {
                mLastError = e.ToString();
                return false;
            }

            return true;
        }

        public int Wake(IEnumerable<string> macAddressList, int port = DEFAULT_WOL_PORT)
        {
            int nSuccessCount = 0;
            if (port < 0
                || port > ushort.MaxValue)
                return nSuccessCount;   // invalid port range

            if (null == mSocket)
                return nSuccessCount;   // Socket is null

            mLastError = "*** Failed List ***" + Environment.NewLine;

            foreach (string macAddress in macAddressList)
            {
                if (Wake(macAddress, port))
                {
                    ++nSuccessCount;
                }
                else
                {
                    mLastError += macAddress + Environment.NewLine;
                }
            }

            return nSuccessCount;
        }




        public string GetLastError() => mLastError;

        public static bool IsValidMacAddress(string macAddress)
        {
            if (string.IsNullOrWhiteSpace(macAddress))
                return false;

            // 00-11-22-33-44-55 -> 001122334455
            string mac = macAddress.Replace("-", "")
                                .Replace(":", "")
                                .Replace(" ", "")
                                .ToUpper();

            if (mac.Length != 12)
                return false;

            foreach (char ch in mac)
            {
                switch (ch)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                        continue;

                    default:
                        return false;
                }
            }

            return true;
        }

        private byte[] _ConvertMacAddressToByteArray(string macAddress)
        {
            if (!IsValidMacAddress(macAddress))
#if NETFRAMEWORK
                return null;
#else
                return new byte[0];
#endif
            string mac = macAddress.Replace("-", "")
                    .Replace(":", "")
                    .Replace(" ", "")
                    .ToUpper();

            byte[] retData = new byte[MAC_ADDRESS_BYTE_ARRAY_SIZE];

            for (int i = 0; i < MAC_ADDRESS_BYTE_ARRAY_SIZE; ++i)
                retData[i] = Convert.ToByte(mac.Substring(i * 2, 2), 16);

            return retData;
        }

        private byte[] _MakePacket(string macAddress)
        {
            var macData = _ConvertMacAddressToByteArray(macAddress);

#if NETFRAMEWORK
            if (null == macData)
                return null;
#else
            if(macData.Length == 0)
                return new byte[0];
#endif

            byte[] packetData = new byte[MAGIC_PACKET_LENGTH];

            // fill 0xFF first 6 bytes
            packetData[0] = packetData[1] = packetData[2] = packetData[3] = packetData[4] = packetData[5] = 0xFF;

            for (int i = 1; i <= MAC_ADDRESS_REPEAT_COUNT; ++i)
                Buffer.BlockCopy(macData, 0, packetData, i * MAC_ADDRESS_BYTE_ARRAY_SIZE, MAC_ADDRESS_BYTE_ARRAY_SIZE);

            return packetData;
        }

    }
}
