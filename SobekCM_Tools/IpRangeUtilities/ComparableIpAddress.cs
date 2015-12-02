#region Using directives

using System;
using System.Net;

#endregion

namespace SobekCM.Tools.IpRangeUtilities
{
    /// <summary> Single IP address that is wrapped in such as way to quickly compare to a set of restricted IP addresses </summary>
    public class ComparableIpAddress
    {
        /// <summary> Constructor for a new instance of the ComparableIpAddress class </summary>
        /// <param name="Value"> IP Address as an unsigned long </param>
        public ComparableIpAddress(ulong Value)
        {
            this.Value = Value;

            Prefix = Convert.ToByte(Value /16777216);
        }

        /// <summary> Constructor for a new instance of the ComparableIpAddress class </summary>
        /// <param name="Value"> IP Address as a string </param>
        public ComparableIpAddress(string Value)
        {
            
            IPAddress thisIpAddr = IPAddress.Parse(Value);
            byte[] bytes = thisIpAddr.GetAddressBytes();
            ulong ipAddrLong = bytes[3] + (Convert.ToUInt64(bytes[2])*256) + (Convert.ToUInt64(bytes[1])*65536) + (Convert.ToUInt64(bytes[0])*16777216); //bytes.Aggregate<byte, ulong>(0, (current, t) => (current*256) + t);
            this.Value = ipAddrLong;

            Prefix = bytes[0];
        }

        /// <summary> Value of the IP address (as an unsigned long) </summary>
        public ulong Value { get; private set; }

        /// <summary> First byte of the IP address, for quick comparison lookup </summary>
        public byte Prefix { get; private set; }

        /// <summary> Static method is used to convert from a string IP address to an unsigned long IP adress </summary>
        /// <param name="IpAddress"> IP address as a string </param>
        /// <returns> IP address as an unsigned long </returns>
        public static ulong ToUlong(string IpAddress)
        {
            IPAddress thisIpAddr = IPAddress.Parse(IpAddress);
            byte[] bytes = thisIpAddr.GetAddressBytes();
            ulong ipAddrLong = bytes[3] + (Convert.ToUInt64(bytes[2]) * 256) + (Convert.ToUInt64(bytes[1]) * 65536) + (Convert.ToUInt64(bytes[0]) * 16777216);
            return ipAddrLong;
        }
    }
}
