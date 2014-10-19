#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using SobekCM.Core.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Resource_Object;
using SobekCM.Tools;

#endregion

namespace SobekCM.EngineLibrary.ApplicationState
{
    /// <summary> Allows individual items to be retrieved by various methods as <see cref="Single_Item"/> objects. </summary>
    public class Item_Lookup_Object 
    {
        private readonly Object thisLock = new Object();
        private readonly Dictionary<string, Multiple_Volume_Item> titleLookupByBib;


        /// <summary> Constructor for a new instance of the Item_Lookup_Object class </summary>
        public Item_Lookup_Object()
        {
            titleLookupByBib = new Dictionary<string, Multiple_Volume_Item>();
            Last_Updated = new DateTime(2000, 1, 1);
        }

        /// <summary> Returns the time the item list was last updated </summary>
        public DateTime Last_Updated { get; set; }

        /// <summary> Clears the dictionary of titles by bibid </summary>
        /// <remarks> This is generally called before repopulating the dictionary from the database </remarks>
        public void Clear()
        {
            titleLookupByBib.Clear();
        }

        /// <summary> Gets a <see cref="Single_Item"/> object from the collection, by Bib ID and VID </summary>
        /// <param name="BibID_VID"> Object Id for this item ( BibID + '_' + VID ) </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Basic information about this item as a <see cref="Single_Item"/> object. </returns>
        public Single_Item Item_By_Bib_VID(string BibID_VID, Custom_Tracer Tracer)
        {
            // Try to look this up in the database
            lock (thisLock)
            {
                // Try to pull this from the database
                string[] splitter = BibID_VID.Split("_".ToCharArray());
                if (splitter.Length == 2)
                {
                    string bibid = splitter[0];
                    string vid = splitter[1];

                    return Item_By_Bib_VID(bibid, vid, Tracer);
                }
                return null;
            }
        }

        /// <summary> Gets a <see cref="Single_Item"/> object from the collection, by Bib ID and VID </summary>
        /// <param name="BibID"> Bibliographic identifier for the title / item group </param>
        /// <param name="VID"> Volume identifier for the individual volume within the title </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Basic information about this item as a <see cref="Single_Item"/> object. </returns>
        public Single_Item Item_By_Bib_VID(string BibID, string VID, Custom_Tracer Tracer)
        {
            // Try to look this up in the database
            lock (thisLock)
            {
                if (titleLookupByBib.ContainsKey(BibID))
                {
                    if (titleLookupByBib[BibID].Contains_VID(VID))
                        return titleLookupByBib[BibID][VID];
                }

                // Try to pull this from the database
                DataRow itemRow = Engine_Database.Get_Item_Information(BibID, VID, Tracer);

                if (itemRow != null)
                {
                    // Get a reference to the item table first
                    DataTable itemTable = itemRow.Table;

                    // Get references to the datacolumn next
                    DataColumn vidColumn = itemTable.Columns["VID"];
                    DataColumn restrictionColumn = itemTable.Columns["IP_Restriction_Mask"];
                    DataColumn titleColumn = itemTable.Columns["Title"];

                    // Create this item object
                    Single_Item newItem = new Single_Item(itemRow[vidColumn].ToString(), Convert.ToInt16(itemRow[restrictionColumn]), itemRow[titleColumn].ToString());

                    // Add this to the existing title, or add a new one
                    if (titleLookupByBib.ContainsKey(BibID))
                    {
                        titleLookupByBib[BibID].Add_Item(newItem);
                    }
                    else
                    {
                        Multiple_Volume_Item newTitle = new Multiple_Volume_Item(BibID);
                        newTitle.Add_Item(newItem);
                        Add_Title(newTitle);
                    }

                    // Return the newly built item as well
                    return newItem;
                }

                return null;
            }
        }

        /// <summary> Gets a <see cref="Single_Item"/> object from the collection, by Bib ID and VID </summary>
        /// <param name="BibID"> Bibliographic identifier for the title / item group </param>
        /// <returns> Basic information about this item as a <see cref="Single_Item"/> object. </returns>
        /// <remarks> If this title / item group has more than one volume, NULL is returned </remarks>
        public Single_Item Item_By_Bib_Only(string BibID)
        {
            // Try to look this up in the database
            lock (thisLock)
            {
                if (titleLookupByBib.ContainsKey(BibID))
                {
                    if (titleLookupByBib[BibID].Item_Count == 1)
                    {
                        return titleLookupByBib[BibID].First_Item;
                    }
                }
                return null;
            }
        }

        /// <summary> Gets any <see cref="Single_Item"/> object from the collection, by Bib ID </summary>
        /// <param name="BibID"> Bibliographic identifier for the title / item group </param>
        /// <returns> Basic information about this item as a <see cref="Single_Item"/> object. </returns>
        /// <remarks> Even if there are more than one volumes in this title, the first is returned </remarks>
        public Single_Item Any_Item_By_Bib(string BibID)
        {
            // Try to look this up in the database
            lock (thisLock)
            {
                if (titleLookupByBib.ContainsKey(BibID))
                {
                    return titleLookupByBib[BibID].First_Item;
                }
                return null;
            }
        }

        /// <summary> Gets any <see cref="Multiple_Volume_Item"/> object from the collection, by Bib ID </summary>
        /// <param name="BibID"> Bibliographic identifier for the title / item group </param>
        /// <returns> Basic information about this item group as a <see cref="Multiple_Volume_Item"/> object. </returns>
        public Multiple_Volume_Item Title_By_Bib(string BibID)
        {
            // Try to look this up in the database
            lock (thisLock)
            {
                return titleLookupByBib.ContainsKey(BibID) ? titleLookupByBib[BibID] : null;
            }
        }

        /// <summary> Removes an existing item from the lookup tables </summary>
        /// <param name="BibID"> Bibliographic identifier for the title / item group </param>
        /// <param name="VID"> Volume identifier for the individual volume within the title </param>
        /// <remarks> Returns TRUE if successul, otherwise FALSE </remarks>
        public void Remove_Item(string BibID, string VID)
        {
            if (titleLookupByBib.ContainsKey(BibID))
                titleLookupByBib[BibID].Remove_Item(VID);
        }
        
        /// <summary> Adds a new title to the lookup tables </summary>
        /// <param name="newTitle"> New title object </param>
        public void Add_Title(Multiple_Volume_Item newTitle)
        {
            titleLookupByBib[newTitle.BibID] = newTitle;
        }

        /// <summary> Adds an single item ( as a <see cref="SobekCM.Resource_Object.SobekCM_Item"/> object) to the collections of items </summary>
        /// <param name="Item"> Single digital resource to add to the collection of items </param>
        /// <remarks> This does perform a multiple-check to see if volumes already exist for this title / item group </remarks>
        public void Add_SobekCM_Item(SobekCM_Item Item)
        {
            Add_SobekCM_Item(Item, true);
        }

        /// <summary> Adds an single item ( as a <see cref="SobekCM.Resource_Object.SobekCM_Item"/> object) to the collections of items </summary>
        /// <param name="Item"> Single digital resource to add to the collection of items </param>
        /// <param name="check_for_multiples"> Flag indicates whether to perform a multiple-check to see if volumes already exist for this title / item group </param>
        public void Add_SobekCM_Item(SobekCM_Item Item, bool check_for_multiples)
        {
            // Create this item
            Single_Item newItem = new Single_Item(Item.VID, Item.Behaviors.IP_Restriction_Membership, Item.Bib_Info.Main_Title.ToString());

            // Add this to the existing title, or add a new one
            string bibId = Item.BibID;
            if (titleLookupByBib.ContainsKey(bibId))
            {
                titleLookupByBib[bibId].Add_Item(newItem);
            }
            else
            {
                Multiple_Volume_Item newTitle = new Multiple_Volume_Item(bibId);
                newTitle.Add_Item(newItem);
                Add_Title(newTitle);
            }
        }

        /// <summary> Checks to see if a given bibliographic identifier exists </summary>
        /// <param name="BibID"> Bibliographic Identifier to check for existence </param>
        /// <returns> TRUE if a title matches the bibliographic identifier, otherwise FALSE </returns>
        public bool Contains_BibID(string BibID)
        {
            return titleLookupByBib.ContainsKey(BibID);
        }

        /// <summary> Checks to see if a given digital resource exists </summary>
        /// <param name="BibID"> Bibliographic Identifier to check for existence </param>
        /// <param name="VID"> Volume identifier </param>
        /// <returns> TRUE if a item matches the bibliographic and volume identifiers, otherwise FALSE </returns>
        public bool Contains_BibID_VID(string BibID, string VID )
        {
            return titleLookupByBib.ContainsKey(BibID) && titleLookupByBib[BibID].Contains_VID(VID);
        }
        
        /// <summary> Adds an single item ( as a <see cref="Single_Item"/> object) to the collections of items </summary>
        /// <param name="Item"> Single digital resource to add to the collection of items </param>
        /// <param name="BibID"> Bibliographic identifier for the title this volume belongs to </param>
        public void Add_Item(Single_Item Item, string BibID )
        {
            if (titleLookupByBib.ContainsKey(BibID))
            {
                titleLookupByBib[BibID].Add_Item(Item);
            }
            else
            {
                Multiple_Volume_Item newTitle = new Multiple_Volume_Item(BibID);
                newTitle.Add_Item(Item);
                Add_Title(newTitle);
            } 
        }
    }
}
