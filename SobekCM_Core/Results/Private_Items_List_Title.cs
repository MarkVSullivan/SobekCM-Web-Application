#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> A single title which has private items, used for display of all private items within 
    /// an item aggreation, from the internal header </summary>
    [DataContract]
    public class Private_Items_List_Title
    {
        private readonly List<Private_Items_List_Item> items;

        /// <summary> Constructor for a new instance of the Private_Items_List_Title class </summary>
        public Private_Items_List_Title()
        {
            items = new List<Private_Items_List_Item>();
        }

        /// <summary> Number of private items within this private title </summary>
        public int Item_Count
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary> Collection of private items for this private title  </summary>
        [DataMember]
        public ReadOnlyCollection<Private_Items_List_Item> Items
        {
            get
            {
                return new ReadOnlyCollection<Private_Items_List_Item>(items);
            }
        }

        /// <summary> Row number from within the page of results </summary>
        [DataMember]
        public int RowNumber { get; set; }

        /// <summary> Bibliographic identifier (BibID) for this title result </summary>
        [DataMember]
        public string BibID { get; set; }

        /// <summary> Group title for this title result </summary>
        [DataMember]
        public string Group_Title { get; set; }

        /// <summary> Material type for this title result </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary> Date of the last activity to an item in this title </summary>
        [DataMember]
        public DateTime LastActivityDate { get; set; }

        /// <summary> Date of the last milestone to an item in this title </summary>
        [DataMember]
        public DateTime LastMilestoneDate { get; set; }

        /// <summary> Complete number of items which are related to this title </summary>
        [DataMember]
        public int CompleteItemCount { get; set; }

        /// <summary> Primary alternate identifier type related to this title ( i.e., 'Accession Numner', 'LLMC#' )</summary>
        [DataMember]
        public string PrimaryIdentifierType { get; set; }

        /// <summary> Primary alternate identifier related to this title </summary>
        [DataMember]
        public string PrimaryIdentifier { get; set; }

        /// <summary> Adds a single item result to this private item title </summary>
        /// <param name="New_Item"> Item to add to the collection for this title </param>
        public void Add_Item_Result( Private_Items_List_Item New_Item )
        {
            items.Add(New_Item);
        }
    }
}
