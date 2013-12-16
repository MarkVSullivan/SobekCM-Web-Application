#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Library.Results
{
    /// <summary> Class contains the list of all private items within an item aggregation, for display from the internal header </summary>
    public class Private_Items_List
    {
        /// <summary> Constructor for a new instance of the Private_Items_List class </summary>
        public Private_Items_List()
        {
            Title_Results = new List<Private_Items_List_Title>();
        }

        /// <summary> Total number of items matching the search parameters </summary>
        public int Total_Items {  get; set; }

        /// <summary> Total number of titles matching the search parameters </summary>
        public int Total_Titles { get; set; }

        /// <summary> Single  page of results, which is of private items list titles </summary>
        public List<Private_Items_List_Title> Title_Results { get; set; }
    }
}
