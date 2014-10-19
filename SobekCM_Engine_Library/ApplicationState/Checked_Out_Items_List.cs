#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SobekCM.Engine_Library.ApplicationState
{
    /// <summary> Stores the list of all items which are currently checked out for single
    /// fair use and the IP address currently viewing the item </summary>
    public class Checked_Out_Items_List
    {
        private readonly SortedList<DateTime, Checked_Out_Item> itemsByCheckoutTime;
        private readonly Dictionary<int, Checked_Out_Item> itemsByItemid;

        private readonly Object thisLock = new Object();

        /// <summary> Constructor for a new instance of the Checked_Out_Items_List </summary>
        public Checked_Out_Items_List()
        {
            itemsByCheckoutTime = new SortedList<DateTime, Checked_Out_Item>();
            itemsByItemid = new Dictionary<int, Checked_Out_Item>();
        }

        /// <summary> Attempts to check out an item (if it was marked for checking out and single use) 
        /// to a particular user.  If the user already has that item, it is renewed.  If the item is 
        /// checked out to a different user, then the user fails to check it out </summary>
        /// <param name="ItemID"> Primary key to the digital resource which to check out</param>
        /// <param name="IP_Address"> IP Address to which to check this item </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Check_Out(int ItemID, string IP_Address)
        {
            // Perform this in a lock
            lock (thisLock)
            {
                // First, always update the checkout list
                Update_Check_Out_List();

                if (itemsByItemid.ContainsKey(ItemID))
                {
                    Checked_Out_Item newItem = itemsByItemid[ItemID];

                    if (newItem.IP_Address == IP_Address)
                    {
                        if (itemsByCheckoutTime.ContainsKey(newItem.Check_Out_Time))
                            itemsByCheckoutTime.Remove(newItem.Check_Out_Time);
                        newItem.Renew();
                        bool reserved = false;
                        while (!reserved)
                        {
                            try
                            {
                                itemsByCheckoutTime[newItem.Check_Out_Time] = newItem;
                                reserved = true;
                            }
                            catch
                            {
                                newItem.Renew();
                            }
                        }
                        return true;
                    }

                    return false;
                }

                add_new_checked_out_status(ItemID, IP_Address);
                return true;
            }
        }

        /// <summary> Checks the list of items currently checked out and checks back in any
        /// items which have not been accessed for five minutes </summary>
        public void Update_Check_Out_List()
        {
            lock (thisLock)
            {
                List<Checked_Out_Item> deleteItems = null;
                foreach (KeyValuePair<DateTime, Checked_Out_Item> checkedInItem in itemsByCheckoutTime)
                {
                    TimeSpan sinceCheckOut = DateTime.Now.Subtract(checkedInItem.Key);
                    if (sinceCheckOut.TotalSeconds > (60*15))
                    {
                        if (deleteItems == null)
                            deleteItems = new List<Checked_Out_Item>();
                        deleteItems.Add(checkedInItem.Value);
                    }
                    else
                    {
                        break;
                    }
                }
                if (deleteItems != null)
                {
                    foreach (Checked_Out_Item deleteItem in deleteItems)
                    {
                        remove_item(deleteItem);
                    }
                }
            }
        }

        private void remove_item(Checked_Out_Item DeleteItem)
        {
            lock (thisLock)
            {
                itemsByCheckoutTime.Remove(DeleteItem.Check_Out_Time);
                itemsByItemid.Remove(DeleteItem.ItemID);
            }
        }

        private void add_new_checked_out_status(int ItemID, string IP_Address)
        {
            lock (thisLock)
            {
                Checked_Out_Item newItem = new Checked_Out_Item(ItemID, IP_Address);
                bool reserved = false;
                while (!reserved)
                {
                    try
                    {
                        itemsByCheckoutTime[newItem.Check_Out_Time] = newItem;
                        reserved = true;
                    }
                    catch
                    {
                        newItem.Renew();
                    }
                }
                itemsByItemid[ItemID] = newItem;
            }
        }
    }
}
