using System;
using System.Net;

namespace SobekCM.Engine_Library.IpRangeUtilities
{
    public class ComparableIpAddress
    {
        public ComparableIpAddress(ulong Value)
        {
            this.Value = Value;

            Prefix = Convert.ToByte(Value /16777216);
        }

        public ComparableIpAddress(string Value)
        {
            
            IPAddress thisIpAddr = IPAddress.Parse(Value);
            byte[] bytes = thisIpAddr.GetAddressBytes();
            ulong ipAddrLong = bytes[3] + (Convert.ToUInt64(bytes[2])*256) + (Convert.ToUInt64(bytes[1])*65536) + (Convert.ToUInt64(bytes[0])*16777216); //bytes.Aggregate<byte, ulong>(0, (current, t) => (current*256) + t);
            this.Value = ipAddrLong;

            Prefix = bytes[0];
        }

        public ulong Value { get; private set; }

        public byte Prefix { get; private set; }

        public static ulong ToUlong(string IpAddress)
        {
            IPAddress thisIpAddr = IPAddress.Parse(IpAddress);
            byte[] bytes = thisIpAddr.GetAddressBytes();
            ulong ipAddrLong = bytes[3] + (Convert.ToUInt64(bytes[2]) * 256) + (Convert.ToUInt64(bytes[1]) * 65536) + (Convert.ToUInt64(bytes[0]) * 16777216);
            return ipAddrLong;
        }
    }
}
