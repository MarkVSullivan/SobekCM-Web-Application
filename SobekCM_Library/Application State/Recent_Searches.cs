#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SobekCM.Library.Navigation;

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

			// If this exceeds 50 remove the first
			if ( searches.Count > 50 )
			{
				searches.RemoveAt( 0 );
			}
		}

		/// <summary> Returns the list of recent searches as a HTML table for display online </summary>
		/// <param name="language"> Current language of the user's interface </param>
		/// <returns> HTML for a table of the recent searches ( collection, type, terms, time )</returns>
		public string ToHTML( Language_Enum language)
		{
			if (searches.Count == 0)
			{
				return "<br /><br /><br /><b>NO SEARCHES SINCE LAST SYSTEM RESTART</b><br /><br /><br />";
			}

			// Build this in a string builder
			StringBuilder builder = new StringBuilder(5000);

			// Add the header information
			builder.Append("<table width=\"100%\" border=\"0px\" cellspacing=\"0px\" >" + Environment.NewLine );
			builder.Append("  <tr align=\"left\" bgcolor=\"#0022a7\" >" + Environment.NewLine );

			switch ( language )
			{
				case Language_Enum.French:
					builder.Append("    <th><span style=\"color: White\">COLLECTION</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">TYPE</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">TERMES</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">HEURE</span></th>" + Environment.NewLine );
					break;

				case Language_Enum.Spanish:
					builder.Append("    <th><span style=\"color: White\">COLECCION</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">TIPO</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">TERMINOS</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">FECHA</span></th>" + Environment.NewLine );
					break;

				default:
					builder.Append("    <th><span style=\"color: White\">COLLECTION</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">TYPE</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">TERMS</span></th>" + Environment.NewLine );
					builder.Append("    <th><span style=\"color: White\">TIME</span></th>" + Environment.NewLine );
					break;
			}
			builder.Append("  </tr>" + Environment.NewLine );

			// Now, add each row
			foreach( Search thisSearch in searches )
			{
				builder.Append("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>" + Environment.NewLine );
				builder.Append("  <tr align=\"left\">" + Environment.NewLine );
				builder.Append("    <td>" + thisSearch.Aggregation.Replace("&","&amp;").Replace("\"","&quot;") + "</td>" + Environment.NewLine );
				builder.Append("    <td>" + thisSearch.Search_Type + "</td>" + Environment.NewLine );
				builder.Append("    <td>" + thisSearch.Search_Terms.Replace("&", "&amp;").Replace("\"", "&quot;") + "</td>" + Environment.NewLine );
				builder.Append("    <td>" + thisSearch.Time + "</td>" + Environment.NewLine );
				builder.Append("  </tr>" + Environment.NewLine );
			}
			builder.Append("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>" + Environment.NewLine );
			builder.Append("</table>" + Environment.NewLine );
			return builder.ToString();
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
				currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
				this.Aggregation = "<a href=\"" +  currentMode.Redirect_URL() + "\">" + Aggregation + "</a>";

				// Save the search terms as a link to the search
				currentMode.Mode = lastMode;
				this.Search_Terms = "<a href=\"" + currentMode.Redirect_URL() + "\">" + Search_Terms + "</a>";
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
