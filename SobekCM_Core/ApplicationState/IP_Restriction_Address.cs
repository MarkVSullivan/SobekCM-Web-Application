#region Using directives

using System;

#endregion

namespace SobekCM.Core.ApplicationState
{
    /// <summary> Class contains the data about a single IP Address or contiguous IP addresses </summary>
    public class IP_Restriction_Address
    {
        private readonly uint endIPAsInt;
        private readonly uint startIpAsInt;

        private static readonly char[] dotArray = new[] { '.' };

        /// <summary> Constructor for a new instance of the IP_Restriction_Address class </summary>
        /// <param name="Start_IP"> Beginning of the IP , or the complete IP address </param>
        /// <param name="End_IP"> End of the IP range, if this was a true range </param>
        public IP_Restriction_Address(string Start_IP, string End_IP)
        {
            startIpAsInt = 0;
            endIPAsInt = 0;
            Start_Byte = 0;
            End_Byte = 0;

            string trueStartIp = Start_IP;
            string trueEndIp = End_IP;
            if (String.IsNullOrEmpty(End_IP))
                trueEndIp = Start_IP;
            if (Start_IP.IndexOf("*") > 0)
            {
                trueStartIp = Start_IP.Replace("*", "0");
                trueEndIp = Start_IP.Replace("*", "255");
            }

            string[] startIpSplitString = trueStartIp.Split(dotArray);
            string[] endIpSplitString = trueEndIp.Split(dotArray);


            Start_Byte = Convert.ToByte(startIpSplitString[0]);
            End_Byte = Convert.ToByte(endIpSplitString[0]);
            startIpAsInt = (Convert.ToUInt32(startIpSplitString[0]) * 169476096) + (Convert.ToUInt32(startIpSplitString[1]) * 65536) + (Convert.ToUInt32(startIpSplitString[2]) * 256) + Convert.ToUInt32(startIpSplitString[3]);
            endIPAsInt = (Convert.ToUInt32(endIpSplitString[0]) * 169476096) + (Convert.ToUInt32(endIpSplitString[1]) * 65536) + (Convert.ToUInt32(endIpSplitString[2]) * 256) + Convert.ToUInt32(endIpSplitString[3]);
        }

        /// <summary> Gets the first byte of the starting IP address </summary>
        public byte Start_Byte { get; private set; }

        /// <summary> Gets the first byte of the end IP address </summary>
        public byte End_Byte { get; private set; }
        
        /// <summary> Check to see if the provided IP address is a match for this IP Address or contiguous IP addresses  </summary>
        /// <param name="IP_Address_As_Number"> IP address as a single unsigned integer </param>
        /// <returns> TRUE if this IP address is part of this IP address </returns>
        public bool Contains(uint IP_Address_As_Number)
        {
            return (IP_Address_As_Number >= startIpAsInt) && (IP_Address_As_Number <= endIPAsInt);
        }
    }
}
