#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

#endregion

namespace SobekCM.Library.Application_State
{
    /// <summary> Contains all of the IP ranges which may be used to restrict access
    /// to digital resources within this library </summary>
    public class IP_Restriction_Ranges
    {
        /// <summary> Constructor for a new instance of the IP_Restriction_Ranges class </summary>
        public IP_Restriction_Ranges(  )
        {
            IpRanges = new List<IP_Restriction_Range>();
        }

        public List<IP_Restriction_Range> IpRanges { get; set; }

        /// <summary> Gets an IP restriction range by index from this collection </summary>
        /// <param name="index"> Index of the restriction range to get </param>
        /// <returns> Requested IP restriction range </returns>
        public IP_Restriction_Range this[int index]
        {
            get
            {
                return index < IpRanges.Count ? IpRanges[index] : null;
            }
        }

        /// <summary> Number of IP restriction ranges included in this collection </summary>
        public int Count
        {
            get
            {
                return IpRanges.Count;
            }
        }

        /// <summary> Populates this IP_Restriction range information with all of the IP Restriction ranges for this library </summary>
        /// <param name="All_Ranges"> DataTable containing all of the IP ranges </param>
        public void Populate_IP_Ranges(DataTable All_Ranges)
        {
            DataColumn startColumn = All_Ranges.Columns["StartIP"];
            DataColumn endColumn = All_Ranges.Columns["EndIP"];

            IpRanges.Clear();
            IP_Restriction_Range currentRange = null;
            foreach (DataRow thisRow in All_Ranges.Rows)
            {
                // Check if this is the same range, or add a new one
                int ipRangeId = Convert.ToInt32(thisRow[1]);
                if ((currentRange == null) || (currentRange.RangeID != ipRangeId))
                {
                    currentRange = new IP_Restriction_Range(ipRangeId, thisRow[0].ToString(), thisRow[2].ToString());
                    IpRanges.Add(currentRange);
                }

                // Add all the IP addresses to this
                string start = thisRow[startColumn].ToString().Trim();
                if (start.Length > 0)
                {
                    currentRange.Add_IP_Address(start, thisRow[endColumn].ToString().Trim());
                }
            }
            
        }

        /// <summary> Gets the bitwise mask of IP restrictive ranges to which this IP address belongs </summary>
        /// <param name="IP_Address"> IP Address to verify against all IP restrictive ranges </param>
        /// <returns>Restrictive ranges to which this IP address belongs, as a bitwise mask </returns>
        public int Restrictive_Range_Membership(string IP_Address)
        {
            int returnMask = 0;

            // Split the IP address
            string[] ipSplitString = IP_Address.Split(new[] { '.' });

            // Currently only support IP 4
            if (ipSplitString.Length == 4)
            {
                // Get the IP address as an unsigned integer
                uint ipAsNumber = (Convert.ToUInt32(ipSplitString[0]) * 169476096) + (Convert.ToUInt32(ipSplitString[1]) * 65536) + (Convert.ToUInt32(ipSplitString[2]) * 256) + Convert.ToUInt32(ipSplitString[3]);
                byte firstIpByte = Convert.ToByte(ipSplitString[0]);

                // Step through each IP Restrictive range
                returnMask += IpRanges.Where(thisRange => thisRange.Contains(ipAsNumber, firstIpByte)).Sum(thisRange => (int) Math.Pow(2, thisRange.RangeID - 1));
            }

            return returnMask;
        }
    }

    /// <summary> Contains all the information about an IP restriction range, including notes, 
    /// text to use for users that do not have access, and the collection of individual IP addresses </summary>
    public class IP_Restriction_Range
    {
        private readonly Dictionary<byte, List<IP_Restriction_Address>> ipAddresses;
        private string notes;

        /// <summary> Constructor for a new instance of the IP_Restriction_Range class </summary>
        /// <param name="RangeID"> Key for this IP Restriction Range, to which items are actually restricted </param>
        /// <param name="Title"> Title for this IP Restriction Range </param>
        /// <param name="Item_Restricted_Statement"> Statement used when a user directly requests an item for which they do not the pre-requisite access </param>
        /// <param name="Notes"> Notes about this IP Restriction Range (for system admins)</param>
        public IP_Restriction_Range(int RangeID, string Title, string Item_Restricted_Statement, string Notes )
        {
            this.RangeID = RangeID;
            this.Title = Title;
            this.Item_Restricted_Statement = Item_Restricted_Statement;
            notes = Notes;
            ipAddresses = new Dictionary<byte, List<IP_Restriction_Address>>();
        }

        /// <summary> Constructor for a new instance of the IP_Restriction_Range class </summary>
        /// <param name="RangeID"> Key for this IP Restriction Range, to which items are actually restricted </param>
        /// <param name="Title"> Title for this IP Restriction Range </param>
        /// <param name="Item_Restricted_Statement"> Statement used when a user does not have access to certain results </param>
        public IP_Restriction_Range(int RangeID, string Title, string Item_Restricted_Statement )
        {
            this.RangeID = RangeID;
            this.Title = Title;
            this.Item_Restricted_Statement = Item_Restricted_Statement;
            ipAddresses = new Dictionary<byte, List<IP_Restriction_Address>>();
        }

        /// <summary> Title for this IP Restriction Range </summary>
        public string Title { get; set; }

        /// <summary>  Statement used when a user directly requests an item for which they do not the pre-requisite access  </summary>
        public string Item_Restricted_Statement { get; set; }

        /// <summary> Notes about this IP Restriction Range (for system admins) </summary>
        public string Notes
        {
            get { return notes ?? String.Empty; }
            set { notes = value; }
        }

        /// <summary> Key for this IP Restriction Range, to which items are actually restricted </summary>
        public int RangeID { get; set; }

        /// <summary> Add a new IP address or contiguous IP addresses to this IP restrictive range </summary>
        /// <param name="StartIP"> IP address, or beginning of the contiguous IP addresses </param>
        /// <param name="EndIP"> End IP address, if this is a contiguous group of IP addresses </param>
        public void Add_IP_Address(string StartIP, string EndIP)
        {
            IP_Restriction_Address thisAddress = new IP_Restriction_Address(StartIP, EndIP);
            byte start = thisAddress.Start_Byte;
            byte end = thisAddress.End_Byte;
            for (byte x = start; x <= end; x++)
            {
                if (ipAddresses.ContainsKey(x))
                    ipAddresses[x].Add(thisAddress);
                else
                {
                    List<IP_Restriction_Address> newList = new List<IP_Restriction_Address> {thisAddress};
                    ipAddresses.Add(x, newList); 
                }
            }
        }

        /// <summary> Checks to see if the provided IP address is part of this IP restrictive range </summary>
        /// <param name="IP_Address_As_Number"> IP Address expressed as an unsigned int </param>
        /// <param name="First_IP_Part"> First byte of the IP address </param>
        /// <returns> TRUE if this IP address is part of this IP restrictive range </returns>
        public bool Contains( uint IP_Address_As_Number, byte First_IP_Part )
        {
            // Check to see if the first part of the IP address exists at all
            if (ipAddresses.ContainsKey(First_IP_Part))
            {
                List<IP_Restriction_Address> matches = ipAddresses[First_IP_Part];
                return matches.Any(thisMatch => thisMatch.Contains(IP_Address_As_Number));
            }

            return false;
        }            
    }

    /// <summary> Class contains the data about a single IP Address or contiguous IP addresses </summary>
    public class IP_Restriction_Address
    {
        private readonly byte endByte;
        private readonly uint endIpInt;
        private readonly byte startByte;
        private readonly uint startIpInt;

        private static readonly char[] DotArray = new[] {'.'};

        /// <summary> Constructor for a new instance of the IP_Restriction_Address class </summary>
        /// <param name="Start_IP"> Beginning of the IP , or the complete IP address </param>
        /// <param name="End_IP"> End of the IP range, if this was a true range </param>
        public IP_Restriction_Address(string Start_IP, string End_IP)
        {
            startIpInt = 0;
            endIpInt = 0;
            startByte = 0;
            endByte = 0;


            string trueStartIp = Start_IP;
            string trueEndIp = End_IP;
            if (String.IsNullOrEmpty(End_IP))
                trueEndIp = Start_IP;
            if (Start_IP.IndexOf("*") > 0)
            {
                trueStartIp = Start_IP.Replace("*", "0");
                trueEndIp = Start_IP.Replace("*", "255");
            }

            string[] startIpSplitString = trueStartIp.Split(DotArray);
            string[] endIpSplitString = trueEndIp.Split(DotArray);


            startByte = Convert.ToByte(startIpSplitString[0]);
            endByte = Convert.ToByte(endIpSplitString[0]);
            startIpInt = (Convert.ToUInt32(startIpSplitString[0])*169476096) + (Convert.ToUInt32(startIpSplitString[1])*65536) + (Convert.ToUInt32(startIpSplitString[2])*256) + Convert.ToUInt32(startIpSplitString[3]);
            endIpInt = (Convert.ToUInt32(endIpSplitString[0])*169476096) + (Convert.ToUInt32(endIpSplitString[1])*65536) + (Convert.ToUInt32(endIpSplitString[2])*256) + Convert.ToUInt32(endIpSplitString[3]); 
        }

        /// <summary> Gets the first byte of the starting IP address </summary>
        public byte Start_Byte
        {
            get { return startByte; }
        }

        /// <summary> Gets the first byte of the end IP address </summary>
        public byte End_Byte
        {
            get { return endByte; }
        }

        /// <summary> Check to see if the provided IP address is a match for this IP Address or contiguous IP addresses  </summary>
        /// <param name="IP_Address_As_Number"> IP address as a single unsigned integer </param>
        /// <returns> TRUE if this IP address is part of this IP address </returns>
        public bool Contains( uint IP_Address_As_Number )
        {
            if ((IP_Address_As_Number >= startIpInt) && (IP_Address_As_Number <= endIpInt))
                return true;
            
            return false;
        }
    }
}
