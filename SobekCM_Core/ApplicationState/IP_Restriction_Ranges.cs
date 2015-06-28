#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.ApplicationState
{
    /// <summary> Contains all of the IP ranges which may be used to restrict access
    /// to digital resources within this library </summary>
    [DataContract]
    public class IP_Restriction_Ranges : iSerializationEvents
    {
        private readonly Object thisLock = new Object();
        private Dictionary<int, IP_Restriction_Range> rangeDictionary;

        /// <summary> Constructor for a new instance of the IP_Restriction_Ranges class </summary>
        public IP_Restriction_Ranges(  )
        {
            IpRanges = new List<IP_Restriction_Range>();
            rangeDictionary = new Dictionary<int, IP_Restriction_Range>();
        }

        /// <summary> Collection of all the IP restriction ranges </summary>
        [DataMember]
        public List<IP_Restriction_Range> IpRanges { get; set; }

        /// <summary> Gets an IP restriction range by rangeID from this collection </summary>
        /// <param name="RangeID"> RangeID of the restriction range to get </param>
        /// <returns> Requested IP restriction range </returns>
        public IP_Restriction_Range this[int RangeID]
        {
            get {
                return rangeDictionary.ContainsKey(RangeID) ? rangeDictionary[RangeID] : null;
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
            lock (thisLock)
            {
                DataColumn startColumn = All_Ranges.Columns["StartIP"];
                DataColumn endColumn = All_Ranges.Columns["EndIP"];

                rangeDictionary.Clear();
                IpRanges.Clear();
                IP_Restriction_Range currentRange = null;
                foreach (DataRow thisRow in All_Ranges.Rows)
                {
                    // Check if this is the same range, or add a new one
                    int ipRangeId = Convert.ToInt32(thisRow[1]);
                    if ((currentRange == null) || (currentRange.RangeID != ipRangeId))
                    {
                        currentRange = new IP_Restriction_Range(ipRangeId, thisRow[0].ToString(), thisRow[2].ToString(), thisRow[5].ToString());
                        IpRanges.Add(currentRange);

                        rangeDictionary[currentRange.RangeID] = currentRange;
                    }

                    // Add all the IP addresses to this
                    string start = thisRow[startColumn].ToString().Trim();
                    if (start.Length > 0)
                    {
                        currentRange.Add_IP_Address(start, thisRow[endColumn].ToString().Trim());
                    }
                }
            }
        }

        /// <summary> Gets the bitwise mask of IP restrictive ranges to which this IP address belongs </summary>
        /// <param name="IP_Address"> IP Address to verify against all IP restrictive ranges </param>
        /// <returns>Restrictive ranges to which this IP address belongs, as a bitwise mask </returns>
        public int Restrictive_Range_Membership(string IP_Address)
        {
            lock (thisLock)
            {

                int returnMask = 0;

                // Split the IP address
                string[] ipSplitString = IP_Address.Split(new[] {'.'});

                // Currently only support IP 4
                if (ipSplitString.Length == 4)
                {
                    // Get the IP address as an unsigned integer
                    uint ipAsNumber = (Convert.ToUInt32(ipSplitString[0])*169476096) + (Convert.ToUInt32(ipSplitString[1])*65536) + (Convert.ToUInt32(ipSplitString[2])*256) + Convert.ToUInt32(ipSplitString[3]);
                    byte firstIpByte = Convert.ToByte(ipSplitString[0]);

                    // Step through each IP Restrictive range
                    returnMask += IpRanges.Where(ThisRange => ThisRange.Contains(ipAsNumber, firstIpByte)).Sum(ThisRange => (int) Math.Pow(2, ThisRange.RangeID - 1));
                }

                return returnMask;
            }
        }

        /// <summary> Method is called by the serializer after this item is unserialized </summary>
        public void PostUnSerialization()
        {
            if (rangeDictionary == null)
                rangeDictionary = new Dictionary<int, IP_Restriction_Range>();
            else
                rangeDictionary.Clear();

            foreach (IP_Restriction_Range thisRange in IpRanges)
            {
                rangeDictionary[thisRange.RangeID] = thisRange;
            }

        }
    }

 

 
}
