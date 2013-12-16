#region Using directives

using System;
using SobekCM.Library.Results;

#endregion

namespace SobekCM.Library.Database
{
	/// <summary> Class contains all the display and identification information about a single item 
	/// within a title in a search result set from a database search  </summary>
	[Serializable]
	public class Database_Item_Result : iSearch_Item_Result
	{
		/// <summary> Gets the primary key for this item, from the database </summary>
		public int ItemID { get; internal set; }

		#region Basic properties the implement the iSearch_Item_Result interface

		/// <summary> Volume identifier (VID) for this item within a title within a collection of results </summary>
		public string VID 
		{ 
			get; internal set; 
		}

		/// <summary> Title for this item within a title within a collection of results </summary>
		public string Title { get; internal set; }

		/// <summary> IP restriction mask for this item within a title within a collection of results </summary>
		public short IP_Restriction_Mask { get; internal set; }

		/// <summary> Thumbnail image for this item within a title within a collection of results </summary>
		public string MainThumbnail { get; internal set; }

		/// <summary> Index of the first serial hierarchy level for this item within a title within a collection of results </summary>
		public short Level1_Index { get; internal set; }

		/// <summary> Text of the first serial hierarchy level for this item within a title within a collection of results </summary>
		public string Level1_Text { get; internal set; }

		/// <summary> Index of the second serial hierarchy level for this item within a title within a collection of results </summary>
		public short Level2_Index { get; internal set; }

		/// <summary> Text of the second serial hierarchy level for this item within a title within a collection of results </summary>
		public string Level2_Text { get; internal set; }

		/// <summary> Index of the third serial hierarchy level for this item within a title within a collection of results </summary>
		public short Level3_Index { get; internal set; }

		/// <summary> Text of the third serial hierarchy level for this item within a title within a collection of results </summary>
		public string Level3_Text { get; internal set; }

		/// <summary> Publication date for this item within a title within a collection of results </summary>
		public string PubDate { get; internal set; }

		/// <summary> Number of pages within this item within a title within a collection of results </summary>
		public int PageCount { get; internal set; }

		/// <summary> External URL for this item within a title within a collection of results </summary>
		public string Link { get; internal set; }

		/// <summary> Spatial coverage as KML for this item within a title result for map display </summary>
		public string Spatial_KML { get; internal set; }

		/// <summary> COinS OpenURL format of citation for citation sharing </summary>
		public string COinS_OpenURL { get; internal set; }

		#endregion
	}
}
