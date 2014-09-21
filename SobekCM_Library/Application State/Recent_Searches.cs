#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SobekCM.Library.Configuration;
using SobekCM.Library.Navigation;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.Application_State
{
	/// <summary> Class keeps track of all the recent searches performed against this digital library </summary>
	public class Recent_Searches
	{
		private readonly List<Search> searches;

		/// <summary> Constructor for a new instance of the Recent_Searches class </summary>
		public Recent_Searches()
		{
			searches = new List<Search>();
		}

		/// <summary> Read-only collection of recent searches against this digital library </summary>
		public ReadOnlyCollection<Search> Searches
		{
			get { return new ReadOnlyCollection<Search>(searches); }
		}

		/// <summary> Adds a new search to the collection of recent searches </summary>
		/// <param name="currentMode"> Current navigation object to save the modal information ( interface, etc..) </param>
		/// <param name="SessionIP"> IP address which performed this search </param>
		/// <param name="Search_Type"> Type of search </param>
		/// <param name="Aggregation"> Aggregation against which this search was performed </param>
		/// <param name="Search_Terms"> Terms used in the search </param>
		public void Add_New_Search(SobekCM_Navigation_Object currentMode, string SessionIP, Search_Type_Enum Search_Type, string Aggregation, string Search_Terms)
		{
			// Add this to the end of the ArrayList
			Search newSearch = new Search(currentMode, SessionIP, Search_Type, Aggregation, Search_Terms);

			// If this was not found, add it
			if (searches.Contains(newSearch)) return;

			searches.Add( newSearch );

			// If this exceeds 100 remove the first
			if ( searches.Count > 100 )
			{
				searches.RemoveAt( 0 );
			}
		}

		#region Nested type: Search

		/// <summary> Stores the pertinent information about a recent search against this digital library </summary>
		public class Search : IEquatable<Search>
		{
			/// <summary> Aggregation against which this search was performed </summary>
			public readonly string Aggregation;

			/// <summary> Key is composed of session ip and all the search information, to avoid duplication in search list </summary>
			public readonly string Key;

			/// <summary> Terms used in the search </summary>
			public readonly string Search_Terms;

			/// <summary> String version of the type of search ( Advanced, Basic, Map, etc.. ) </summary>
			public readonly string Search_Type;

			/// <summary> IP address which performed this search </summary>
			public readonly string SessionIP;

			/// <summary> Time the search was first performed by this user </summary>
			public readonly string Time;

			/// <summary> Constructor for a new instance of the Search object </summary>
			/// <param name="currentMode"> Current navigation object to save the modal information ( interface, etc..)</param>
			/// <param name="SessionIP"> IP address which performed this search</param>
			/// <param name="Search_Type"> Type of search </param>
			/// <param name="Aggregation"> Aggregation against which this search was performed</param>
			/// <param name="Search_Terms"> Terms used in the search </param>
			public Search(SobekCM_Navigation_Object currentMode, string SessionIP, Search_Type_Enum Search_Type, string Aggregation, string Search_Terms)
			{
				Time = DateTime.Now.ToShortDateString().Replace("/","-") + " " + DateTime.Now.ToShortTimeString().Replace(" ","");

				// Save some of the values
				this.SessionIP = SessionIP;
				switch (Search_Type)
				{
					case Search_Type_Enum.Advanced:
						this.Search_Type = "Advanced";
						break;

					case Search_Type_Enum.Basic:
						this.Search_Type = "Basic";
						break;

					case Search_Type_Enum.Newspaper:
						this.Search_Type = "Newspaper";
						break;

					case Search_Type_Enum.Map:
						this.Search_Type = "Map";
						break;

					default:
						this.Search_Type = "Unknown";
						break;               
				}

				// Create the key
				Key = SessionIP + "_" + this.Search_Type + "_" + Aggregation + "_" + Search_Terms.Replace(" ", "_");

				// Save the collection as a link
				Display_Mode_Enum lastMode = currentMode.Mode;
				currentMode.Mode = Display_Mode_Enum.Aggregation;
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                this.Aggregation = "<a href=\"" + currentMode.Redirect_URL() + "\">" + Aggregation.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>";

				// Save the search terms as a link to the search
				currentMode.Mode = lastMode;
                this.Search_Terms = "<a href=\"" + currentMode.Redirect_URL() + "\">" + Search_Terms.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>";
			}

			#region IEquatable<Search> Members

			/// <summary> Checks to see if two searches are equal </summary>
			/// <param name="other"> Search to compare to this search </param>
			/// <returns> TRUE if equal, otherwise FALSE </returns>
			/// <remarks> Two searches are considered equal if the keys are identical </remarks>
			public bool Equals(Search other)
			{
				return other.Key == Key;
			}

			#endregion
		}

		#endregion
	}
}
