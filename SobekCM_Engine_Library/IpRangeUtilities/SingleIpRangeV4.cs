#region Using directives

using System;

#endregion

namespace SobekCM.Engine_Library.IpRangeUtilities
{
    /// <summary> Single IP address range used in the solution for quick IP range checks to
    /// support IP restrictions on the SobekCM engine </summary>
    public class SingleIpRangeV4 
    {
        /// <summary> Constructor for a new instance of the SingleIpRangeV4 class </summary>
        /// <param name="SingleIpAddress"> IP address, as an unsigned long </param>
        public SingleIpRangeV4(ulong SingleIpAddress)
        {
            StartIpAddress = SingleIpAddress;
        }

        /// <summary> Constructor for a new instance of the SingleIpRangeV4 class </summary>
        /// <param name="StartIpAddress"> First IP address in the range (as an unsigned long)</param>
        /// <param name="EndIpAddress">  Last IP address in the range (as an unsigned long)</param>
        public SingleIpRangeV4(ulong StartIpAddress, ulong EndIpAddress)
        {
            this.StartIpAddress = StartIpAddress;
            this.EndIpAddress = EndIpAddress;

            Prefix = Convert.ToByte(this.StartIpAddress / 16777216);
        }

        /// <summary> Constructor for a new instance of the SingleIpRangeV4 class </summary>
        /// <param name="SingleIpAddress"> IP address, as a string </param>
        public SingleIpRangeV4(string SingleIpAddress)
        {
            if (SingleIpAddress.Contains("/"))
            {
                string[] parse = SingleIpAddress.Split("/".ToCharArray());
                StartIpAddress = ComparableIpAddress.ToUlong(parse[0]);

                int cidr = Int32.Parse(parse[1]);
                int add_number = (int) Math.Pow(2, (32 - cidr)) - 1;
                EndIpAddress = StartIpAddress + (ulong) add_number;
            }
            else
            {
                StartIpAddress = ComparableIpAddress.ToUlong(SingleIpAddress);
            }

            Prefix = Convert.ToByte(StartIpAddress / 16777216);
            
        }

        /// <summary> Constructor for a new instance of the SingleIpRangeV4 class </summary>
        /// <param name="StartIpAddress"> First IP address in the range (as a string)</param>
        /// <param name="EndIpAddress">  Last IP address in the range (as a string)</param>
        public SingleIpRangeV4(string StartIpAddress, string EndIpAddress)
        {
            this.StartIpAddress = ComparableIpAddress.ToUlong(StartIpAddress);
            this.EndIpAddress = ComparableIpAddress.ToUlong(EndIpAddress);

            Prefix = Convert.ToByte(this.StartIpAddress / 16777216);
        }

        /// <summary> First IP address in the range (as an unsigned long) </summary>
        public ulong StartIpAddress { get; private set; }

        /// <summary> Last IP address in the range (as an unsigned long) </summary>
        public ulong? EndIpAddress { get; private set; }

        /// <summary> First byte of the IP address, for quick comparisons </summary>
        public byte Prefix { get; private set; }

        /// <summary> Compares a provided IP address against this range of IPs </summary>
        /// <param name="IpAddress"> IP address to compare to this range </param>
        /// <returns> 0 if this is in the range, 1 if this range is greater than 
        /// the IP, or -1 if the range is less than the comparison IP address </returns>
        public int CompareTo(ComparableIpAddress IpAddress)
        {
            if (!EndIpAddress.HasValue)
            {
                return StartIpAddress.CompareTo(IpAddress.Value);
            }

            if (StartIpAddress > IpAddress.Value)
                return 1;
            if (EndIpAddress.Value < IpAddress.Value)
                return -1;
            return 0;
        }
    }
}
