#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> Class contains the list of all private items within an item aggregation, for display from the internal header </summary>
    [DataContract]
    public class Private_Items_List
    {
        /// <summary> Constructor for a new instance of the Private_Items_List class </summary>
        public Private_Items_List()
        {
            TitleResults = new List<Private_Items_List_Title>();
        }

        /// <summary> Total number of items matching the search parameters </summary>
        [DataMember]
        public int TotalItems {  get; set; }

        /// <summary> Total number of titles matching the search parameters </summary>
        [DataMember]
        public int TotalTitles { get; set; }

        /// <summary> Single  page of results, which is of private items list titles </summary>
        [DataMember]
        public List<Private_Items_List_Title> TitleResults { get; set; }

    }
}
