#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Serialization;

#endregion


namespace SobekCM.Core.ApplicationState
{
    /// <summary>
    ///   Code manager maintains a list of all the valid aggregation codes and
    ///   also provides a lookup from aggregation code to greenstone full text collection code
    ///   to allow for full text searching of single items from the item viewer.
    /// </summary>
    [DataContract]
    public class Aggregation_Code_Manager : iSerializationEvents
    {
        private readonly Dictionary<string, Item_Aggregation_Related_Aggregations> aggregationsByCode;
        private readonly Dictionary<string, List<Item_Aggregation_Related_Aggregations>> aggregationsByType;

        private readonly List<string> allTypes;

        /// <summary>
        ///   Constructor for a new instance of the Aggregation_Code_Manager class
        /// </summary>
        public Aggregation_Code_Manager()
        {
            // Declare the collections
            Aggregations_By_Thematic_Heading = new Dictionary<int, List<Item_Aggregation_Related_Aggregations>>();
            aggregationsByType = new Dictionary<string, List<Item_Aggregation_Related_Aggregations>>();
            aggregationsByCode = new Dictionary<string, Item_Aggregation_Related_Aggregations>();
            allTypes = new List<string>();
            All_Aggregations = new List<Item_Aggregation_Related_Aggregations>();
        }

        /// <summary> Collection of basic information about every aggregation in this instance, sorted by code </summary>
        [DataMember]
        public List<Item_Aggregation_Related_Aggregations> All_Aggregations { get; private set; }

        /// <summary> Dictionary which maps aggregations to thematic headings </summary>
        [DataMember]
        public Dictionary<int, List<Item_Aggregation_Related_Aggregations>> Aggregations_By_Thematic_Heading { get; private set; }

        /// <summary> Read-only collection of all the aggregation information sorted by full name </summary>
		public ReadOnlyCollection<Item_Aggregation_Related_Aggregations> All_Aggregations_Name_Sorted
		{
			get 
			{ 
				SortedDictionary<string, Item_Aggregation_Related_Aggregations> sorter = new SortedDictionary<string, Item_Aggregation_Related_Aggregations>();
				foreach (Item_Aggregation_Related_Aggregations thisAggr in All_Aggregations)
				{
					sorter[thisAggr.Name.ToUpper()] = thisAggr;
				}
				List<Item_Aggregation_Related_Aggregations> returnVal = sorter.Select(DictEntry => DictEntry.Value).ToList();
			    return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>(returnVal);
			}
		}

		/// <summary> Read-only collection of all the aggregation information sorted by short name  </summary>
		public ReadOnlyCollection<Item_Aggregation_Related_Aggregations> All_Aggregations_ShortName_Sorted
		{
			get
			{
				SortedDictionary<string, Item_Aggregation_Related_Aggregations> sorter = new SortedDictionary<string, Item_Aggregation_Related_Aggregations>();
				foreach (Item_Aggregation_Related_Aggregations thisAggr in All_Aggregations)
				{
					sorter[thisAggr.ShortName.ToUpper()] = thisAggr;
				}
				List<Item_Aggregation_Related_Aggregations> returnVal = sorter.Select(DictEntry => DictEntry.Value).ToList();
			    return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>(returnVal);
			}
		}

        /// <summary> Read-only collection of all the aggregation types </summary>
        public ReadOnlyCollection<string> All_Types
        {
            get { return new ReadOnlyCollection<string>(allTypes); }
        }

        /// <summary> Gets the number of different aggregation types present </summary>
        public int Types_Count
        {
            get { return allTypes.Count; }
        }

        /// <summary>
        ///   Gets the aggregation information by aggregation code
        /// </summary>
        /// <param name = "Aggregation_Code"> Code for the aggregation of interest</param>
        /// <returns> Aggregation information, or NULL if not present </returns>
        public Item_Aggregation_Related_Aggregations this[string Aggregation_Code]
        {
            get {
                return aggregationsByCode.ContainsKey(Aggregation_Code.ToUpper()) ? aggregationsByCode[Aggregation_Code.ToUpper()] : null;
            }
        }

        /// <summary>
        ///   Clears the internal data for this code manager
        /// </summary>
        public void Clear()
        {
            Aggregations_By_Thematic_Heading.Clear();
            aggregationsByType.Clear();
            aggregationsByCode.Clear();
            allTypes.Clear();
            All_Aggregations.Clear();
        }

        public void Add_Collection(Item_Aggregation_Related_Aggregations New_Aggregation, int Theme)
        {
			// Insert this into the proper spot in the item aggregation list
	        int index = 0;
			while ((index < All_Aggregations.Count) && ( string.CompareOrdinal(New_Aggregation.Code, All_Aggregations[index].Code) > 0 ))
			{
				index++;
			}
			All_Aggregations.Insert(index, New_Aggregation);

            // Add this to the various dictionaries
            aggregationsByCode[New_Aggregation.Code] = New_Aggregation;
            if (!allTypes.Contains(New_Aggregation.Type))
            {
                allTypes.Add(New_Aggregation.Type);
            }
            if (aggregationsByType.ContainsKey(New_Aggregation.Type))
            {
                aggregationsByType[New_Aggregation.Type].Add(New_Aggregation);
            }
            else
            {
                aggregationsByType[New_Aggregation.Type] = new List<Item_Aggregation_Related_Aggregations> {New_Aggregation};
            }
            if (Theme > 0)
            {
                if (Aggregations_By_Thematic_Heading.ContainsKey(Theme))
                {
                    Aggregations_By_Thematic_Heading[Theme].Add(New_Aggregation);
                }
                else
                {
                    Aggregations_By_Thematic_Heading[Theme] = new List<Item_Aggregation_Related_Aggregations> {New_Aggregation};
                }
            }
        }

        /// <summary>
        ///   Read-only collection of item aggregationPermissions matching a particular aggregation type
        /// </summary>
        /// <param name = "AggregationType"> Type of aggregationPermissions to return </param>
        /// <returns> Read-only collection of item aggregation relational objects </returns>
        public ReadOnlyCollection<Item_Aggregation_Related_Aggregations> Aggregations_By_Type(string AggregationType)
        {
            if (aggregationsByType.ContainsKey(AggregationType))
            {
                return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>(aggregationsByType[AggregationType]);
            }
            
            return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>( new List<Item_Aggregation_Related_Aggregations>());
        }

        /// <summary>
        ///   Read-only collection of item aggregationPermissions matching a particular thematic heading id
        /// </summary>
        /// <param name = "ThemeID"> Primary key for the thematic heading to pull </param>
        /// <returns> Read-only collection of item aggregation relational objects </returns>
        public ReadOnlyCollection<Item_Aggregation_Related_Aggregations> Aggregations_By_ThemeID(int ThemeID)
        {
            if (Aggregations_By_Thematic_Heading.ContainsKey(ThemeID))
            {
                return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>(Aggregations_By_Thematic_Heading[ThemeID]);
            }
            
            return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>(new List<Item_Aggregation_Related_Aggregations>());
        }

        /// <summary>
        ///   Gets the short name associated with a provided aggregation code
        /// </summary>
        /// <param name = "Aggregation_Code"> Code for the aggregation of interest</param>
        /// <returns> Short name of valid aggregation, otehrwise the aggregation code is returned </returns>
        public string Get_Collection_Short_Name(string Aggregation_Code)
        {
            if (aggregationsByCode.ContainsKey(Aggregation_Code.ToUpper()))
                return aggregationsByCode[Aggregation_Code.ToUpper()].ShortName;
            
            return Aggregation_Code;
        }

        /// <summary>
        ///   Checks to see if an aggregation code exists
        /// </summary>
        /// <param name = "Aggregation_Code"> Code for the aggregation of interest </param>
        /// <returns> TRUE if the aggregation exists, otherwise FALSE </returns>
        public bool isValidCode(string Aggregation_Code)
        {
            return aggregationsByCode.ContainsKey(Aggregation_Code.ToUpper());
        }

		/// <summary> Set an aggregation to be a part of an existing thematic heading id </summary>
		/// <param name="Code"></param>
		/// <param name="ThematicHeadingID"></param>
	    public void Set_Aggregation_Thematic_Heading(string Code, int? ThematicHeadingID)
		{
            Item_Aggregation_Related_Aggregations thisAggr = aggregationsByCode[Code.ToUpper()];

            // If this is NULL, just return
		    if (!ThematicHeadingID.HasValue)
		    {
                foreach (KeyValuePair<int, List<Item_Aggregation_Related_Aggregations>> theme in Aggregations_By_Thematic_Heading)
                {
                    if (theme.Value.Contains(thisAggr))
                        theme.Value.Remove(thisAggr);
                }
                return;
		    }

			// If the thematic heading ID does not exit, just return
			if (!Aggregations_By_Thematic_Heading.ContainsKey(ThematicHeadingID.Value))
				return;

			// If this aggregation does not exist, just return
			if (!aggregationsByCode.ContainsKey(Code.ToUpper()))
				return;

			// Get this aggregation and list for this thematic heading
            List<Item_Aggregation_Related_Aggregations> thematicHeadingList = Aggregations_By_Thematic_Heading[ThematicHeadingID.Value];

			// If this is already a part of the thematic heading, just return
			if (thematicHeadingList.Contains(thisAggr))
				return;

			// Ensure this aggregation is not a part of any other thematic headings
			foreach (KeyValuePair<int, List<Item_Aggregation_Related_Aggregations>> theme in Aggregations_By_Thematic_Heading)
			{
				if (theme.Value.Contains(thisAggr))
					theme.Value.Remove(thisAggr);
			}

			// Now, add this to the list for this thematic heading
			int index = 0;
			while ((index < thematicHeadingList.Count) && (string.CompareOrdinal(thisAggr.Code, thematicHeadingList[index].Code) > 0))
			{
				index++;
			}
			thematicHeadingList.Insert(index, thisAggr);

		}

		/// <summary> Adds a new blank thematic heading when a user adds one through
		/// the administrative tools </summary>
		/// <param name="NewThematicHeadingID">ID for the new thematic heading</param>
		public void Add_Blank_Thematic_Heading(int NewThematicHeadingID)
	    {
			if (!Aggregations_By_Thematic_Heading.ContainsKey(NewThematicHeadingID))
			{
				Aggregations_By_Thematic_Heading[NewThematicHeadingID] = new List<Item_Aggregation_Related_Aggregations>();
			}
	    }

        public void PostUnSerialization()
        {
            Aggregations_By_Thematic_Heading.Clear();
            aggregationsByType.Clear();
            aggregationsByCode.Clear();
            allTypes.Clear();

            foreach (Item_Aggregation_Related_Aggregations thisAggr in All_Aggregations)
            {

                // Add this to the various dictionaries
                aggregationsByCode[thisAggr.Code] = thisAggr;
                if (!allTypes.Contains(thisAggr.Type))
                {
                    allTypes.Add(thisAggr.Type);
                }
                if (aggregationsByType.ContainsKey(thisAggr.Type))
                {
                    aggregationsByType[thisAggr.Type].Add(thisAggr);
                }
                else
                {
                    aggregationsByType[thisAggr.Type] = new List<Item_Aggregation_Related_Aggregations> { thisAggr };
                }
            }
        }
    }
}