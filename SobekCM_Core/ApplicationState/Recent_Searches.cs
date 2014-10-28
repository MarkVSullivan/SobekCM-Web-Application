#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.ApplicationState
{
	/// <summary> Class keeps track of all the recent searches performed against this digital library </summary>
	[DataContract]
	public class Recent_Searches
	{

		/// <summary> Constructor for a new instance of the Recent_Searches class </summary>
		public Recent_Searches()
		{
            Searches = new List<Search>();
		}

		/// <summary> Collection of recent searches against this digital library </summary>
		[DataMember]
        public List<Search> Searches { get; private set;  }

		/// <summary> Adds a new search to the collection of recent searches </summary>
        /// <param name="NewSearch"> New search information to add </param>
		public void Add_New_Search(Search NewSearch )
		{
			// If this was not found, add it
            if (Searches.Contains(NewSearch)) return;

            Searches.Add(NewSearch);

			// If this exceeds 100 remove the first
            if (Searches.Count > 100)
			{
                Searches.RemoveAt(0);
			}
		}

		#region Nested type: Search

		/// <summary> Stores the pertinent information about a recent search against this digital library </summary>
        [DataContract]
		public class Search : IEquatable<Search>
		{
			/// <summary> Aggregation against which this search was performed </summary>
			[DataMember]
            public string Aggregation { get; set;  }

			/// <summary> Key is composed of session ip and all the search information, to avoid duplication in search list </summary>
            public string Key 
            {
			    get { return SessionIP + "_" + Search_Type + "_" + Aggregation + "_" + Search_Terms; }
            }

			/// <summary> Terms used in the search </summary>
            [DataMember]
            public string Search_Terms { get; set; }

			/// <summary> String version of the type of search ( Advanced, Basic, Map, etc.. ) </summary>
            [DataMember]
            public string Search_Type { get; set; }

			/// <summary> IP address which performed this search </summary>
            [DataMember]
            public string SessionIP { get; set; }

			/// <summary> Time the search was first performed by this user </summary>
            [DataMember]
            public string Time { get; set; }

			/// <summary> Constructor for a new instance of the Search object </summary>
			public Search()
			{
                // Do nothing here
	        }

			#region IEquatable<Search> Members

			/// <summary> Checks to see if two searches are equal </summary>
			/// <param name="Other"> Search to compare to this search </param>
			/// <returns> TRUE if equal, otherwise FALSE </returns>
			/// <remarks> Two searches are considered equal if the keys are identical </remarks>
			public bool Equals(Search Other)
			{
				return Other.Key == Key;
			}

			#endregion
		}

		#endregion
	}
}
