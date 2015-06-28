#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.ApplicationState
{
	/// <summary> Stores information about a title which has multiple volumes or is always represented as multiple volumes ( i.e. Newspapers and Serials) </summary>
    [DataContract]
	public class Multiple_Volume_Item : iSerializationEvents
	{
	    /// <summary> Bibliographic identifier (BibID) for this title within the digital library </summary>
        [DataMember]
	    public readonly string BibID;

        /// <summary> Collection of all the single items within this BibID </summary>
	    [DataMember] 
        public List<Single_Item> Items;

	    private readonly Dictionary<string, Single_Item> itemDictionary;

	    /// <summary> Constructor for a new instance of the Multiple_Volume_Item class </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for this title within the digital library</param>
        public Multiple_Volume_Item( string BibID )
		{
            this.BibID = BibID;
	        Items = new List<Single_Item>();
            itemDictionary = new Dictionary<string, Single_Item>();
		}

	    /// <summary> Number of child items contained within this title </summary>
        public int Item_Count
        {
            get
            {
                return Items.Count;
            }
        }

        /// <summary> Gets the first item within this title's collection of child items </summary>
        /// <remarks> This is used to pull any arbitraty item within this title </remarks>
        public Single_Item First_Item
        {
            get
            {
                return (Items.Count > 0) ? Items[0] : null;
            }
        }

	    /// <summary> Returns a single item from this title's collection of child items, by Volume ID (VID) </summary>
	    /// <param name="VID"> Volume ID for the item to retrieve from this title </param>
	    /// <returns> Object containing necessary information about the item, or NULL if there is no matching item within this title's collection of child items </returns>
	    public Single_Item this[ string VID ]
	    {
	        get 
            {
                return itemDictionary.ContainsKey(VID) ? itemDictionary[VID] : null;
	        }
	    }

	    /// <summary> Adds a single item to this title's collection of child items </summary>
	    /// <param name="NewItem"> New single item information to add to this title </param>
	    public void Add_Item(Single_Item NewItem)
	    {
	        Items.Add(NewItem);
            itemDictionary[NewItem.VID] = NewItem;
	    }

	    /// <summary> Removes a child item from this title's collection of items </summary>
        /// <param name="VID"> Volume identifier for the item to remove </param>
        /// <remarks> This currently does nothing, but should probably check single_vid though </remarks>
        public void Remove_Item( string VID )
        {
            // Do nothing for now
            // Should probably check single_vid though
        }

        /// <summary> Flag indicates if this title's collection of child items includes a particular volume, by Volume ID (VID) </summary>
        /// <param name="VID"> Volume ID for the item to check for existence </param>
        /// <returns> TRUE if the item exists within this title, otherwise FALSE </returns>
        public bool Contains_VID(string VID)
        {
            return itemDictionary.ContainsKey(VID);
        }

        /// <summary> Method is called by the serializer after this item is unserialized </summary>
	    public void PostUnSerialization()
	    {
	        itemDictionary.Clear();
	        foreach (Single_Item thisItem in Items)
	        {
                itemDictionary[thisItem.VID] = thisItem;
	        }
	    }
	}
}
