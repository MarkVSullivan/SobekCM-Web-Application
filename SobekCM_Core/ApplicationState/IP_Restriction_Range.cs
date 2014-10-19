#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.ApplicationState
{
    /// <summary> Contains all the information about an IP restriction range, including notes, 
    /// text to use for users that do not have access, and the collection of individual IP addresses </summary>
    [DataContract]
    public class IP_Restriction_Range
    {
        private readonly Dictionary<byte, List<IP_Restriction_Address>> ipAddresses;
        private string notes;

        private readonly Object thisLock = new Object();

        /// <summary> Constructor for a new instance of the IP_Restriction_Range class </summary>
        /// <param name="RangeID"> Key for this IP Restriction Range, to which items are actually restricted </param>
        /// <param name="Title"> Title for this IP Restriction Range </param>
        /// <param name="Item_Restricted_Statement"> Statement used when a user directly requests an item for which they do not the pre-requisite access </param>
        /// <param name="Notes"> Notes about this IP Restriction Range (for system admins)</param>
        public IP_Restriction_Range(int RangeID, string Title, string Item_Restricted_Statement, string Notes)
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
        public IP_Restriction_Range(int RangeID, string Title, string Item_Restricted_Statement)
        {
            this.RangeID = RangeID;
            this.Title = Title;
            this.Item_Restricted_Statement = Item_Restricted_Statement;
            ipAddresses = new Dictionary<byte, List<IP_Restriction_Address>>();
        }

        /// <summary> Title for this IP Restriction Range </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>  Statement used when a user directly requests an item for which they do not the pre-requisite access  </summary>
        [DataMember]
        public string Item_Restricted_Statement { get; set; }

        /// <summary> Notes about this IP Restriction Range (for system admins) </summary>
        [DataMember]
        public string Notes
        {
            get { return notes ?? String.Empty; }
            set { notes = value; }
        }

        /// <summary> Key for this IP Restriction Range, to which items are actually restricted </summary>
        [DataMember]
        public int RangeID { get; set; }

        /// <summary> Add a new IP address or contiguous IP addresses to this IP restrictive range </summary>
        /// <param name="StartIP"> IP address, or beginning of the contiguous IP addresses </param>
        /// <param name="EndIP"> End IP address, if this is a contiguous group of IP addresses </param>
        public void Add_IP_Address(string StartIP, string EndIP)
        {
            lock (thisLock)
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
        }

        /// <summary> Checks to see if the provided IP address is part of this IP restrictive range </summary>
        /// <param name="IP_Address_As_Number"> IP Address expressed as an unsigned int </param>
        /// <param name="First_IP_Part"> First byte of the IP address </param>
        /// <returns> TRUE if this IP address is part of this IP restrictive range </returns>
        public bool Contains(uint IP_Address_As_Number, byte First_IP_Part)
        {
            lock (thisLock)
            {
                // Check to see if the first part of the IP address exists at all
                if (ipAddresses.ContainsKey(First_IP_Part))
                {
                    List<IP_Restriction_Address> matches = ipAddresses[First_IP_Part];
                    return matches.Any(ThisMatch => ThisMatch.Contains(IP_Address_As_Number));
                }

                return false;
            }
        }
    }
}
