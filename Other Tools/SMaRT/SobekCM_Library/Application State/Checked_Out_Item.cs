#region Using directives

using System;

#endregion

namespace SobekCM.Library.Application_State
{
    /// <summary> Contains information about a single checked out digital resource </summary>
    /// <remarks> In general, the only digital items that are checked out are items that contain
    /// copyright material and are reserved as single (fair) use.<br /><br />
    /// This class is utilized by the <see cref="Checked_Out_Items_List"/> class. </remarks>
    public class Checked_Out_Item
    {
        /// <summary> IP Address to which the digital resource is currently checked out </summary>
        public readonly string IP_Address;

        /// <summary> Primary key to the digital resource which is checked out </summary>
        public readonly int ItemID;

        private DateTime checkOutTime;

        /// <summary> Constructor for a new instance of the Checked_Out_Item class, representing a 
        /// single digital resource which has been checked out </summary>
        /// <param name="ItemID"> Primary key to the digital resource which is checked out </param>
        /// <param name="IP_Address"> IP Address to which the digital resource is currently checked out </param>
        /// <remarks> This also sets the checked out time to the current time </remarks>
        public Checked_Out_Item(int ItemID, string IP_Address)
        {
            this.ItemID = ItemID;
            this.IP_Address = IP_Address;
            checkOutTime = DateTime.Now;
        }

        /// <summary> Time the digital resource was checked out to the IP address </summary>
        public DateTime Check_Out_Time
        {
            get { return checkOutTime; }
        }

        /// <summary> Renew this item, by resetting the check out time to the current time </summary>
        public void Renew()
        {
            checkOutTime = DateTime.Now;
        }
    }
}
