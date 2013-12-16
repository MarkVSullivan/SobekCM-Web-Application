#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Library.Results
{
    /// <summary> A single title which has private items, used for display of all private items within 
    /// an item aggreation, from the internal header </summary>
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
        public ReadOnlyCollection<Private_Items_List_Item> Items
        {
            get
            {
                return new ReadOnlyCollection<Private_Items_List_Item>(items);
            }
        }

        /// <summary> Row number from within the page of results </summary>
        public int RowNumber { get; set; }

        /// <summary> Bibliographic identifier (BibID) for this title result </summary>
        public string BibID { get; set; }

        /// <summary> Group title for this title result </summary>
        public string Group_Title { get; set; }

        /// <summary> Material type for this title result </summary>
        public string Type { get; set; }

        /// <summary> ALEPH cataloging number for this title result </summary>
        public long ALEPH_Number { get; set; }

        /// <summary> OCLC cataloging number for this title result </summary>
        public long OCLC_Number { get; set; }

        /// <summary> Date of the last activity to an item in this title </summary>
        public DateTime Last_Activity_Date { get; set; }

        /// <summary> Date of the last milestone to an item in this title </summary>
        public DateTime Last_Milestone_Date { get; set; }

        /// <summary> Complete number of items which are related to this title </summary>
        public int Complete_Item_Count { get; set; }

        /// <summary> Primary alternate identifier type related to this title ( i.e., 'Accession Numner', 'LLMC#' )</summary>
        public string Primary_Identifier_Type { get; set; }

        /// <summary> Primary alternate identifier related to this title </summary>
        public string Primary_Identifier { get; set; }

        /// <summary> Adds a single item result to this private item title </summary>
        /// <param name="New_Item"> Item to add to the collection for this title </param>
        public void Add_Item_Result( Private_Items_List_Item New_Item )
        {
            items.Add(New_Item);
        }
    }
}
