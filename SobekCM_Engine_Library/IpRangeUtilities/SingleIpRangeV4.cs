using System;

namespace SobekCM.Engine_Library.IpRangeUtilities
{
    public class SingleIpRangeV4 
    {
        public SingleIpRangeV4(ulong SingleIpAddress)
        {
            StartIpAddress = SingleIpAddress;
        }

        public SingleIpRangeV4(ulong StartIpAddress, ulong EndIpAddress)
        {
            this.StartIpAddress = StartIpAddress;
            this.EndIpAddress = EndIpAddress;

            Prefix = Convert.ToByte(this.StartIpAddress / 16777216);
        }

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

        public SingleIpRangeV4(string StartIpAddress, string EndIpAddress)
        {
            this.StartIpAddress = ComparableIpAddress.ToUlong(StartIpAddress);
            this.EndIpAddress = ComparableIpAddress.ToUlong(EndIpAddress);

            Prefix = Convert.ToByte(this.StartIpAddress / 16777216);
        }

        public ulong StartIpAddress { get; private set; }

        public ulong? EndIpAddress { get; private set; }

        public byte Prefix { get; private set; }

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
