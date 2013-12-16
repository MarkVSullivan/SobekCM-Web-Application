namespace SobekCM.Library.Results
{
    /// <summary> Interface which any class that contains the required display and identification 
    /// information for a single item (within a title) within a set of results. </summary>
    public interface iSearch_Item_Result
    {
        /// <summary> Volume identifier (VID) for this item within a title within a collection of results </summary>
        string VID { get; }

        /// <summary> Title for this item within a title within a collection of results </summary>
        string Title { get; }

        /// <summary> IP restriction mask for this item within a title within a collection of results </summary>
        short IP_Restriction_Mask { get; }

        /// <summary> Thumbnail image for this item within a title within a collection of results </summary>
        string MainThumbnail { get; }

        /// <summary> Index of the first serial hierarchy level for this item within a title within a collection of results </summary>
        short Level1_Index { get; }

        /// <summary> Text of the first serial hierarchy level for this item within a title within a collection of results </summary>
        string Level1_Text { get; }

        /// <summary> Index of the second serial hierarchy level for this item within a title within a collection of results </summary>
        short Level2_Index { get; }

        /// <summary> Text of the second serial hierarchy level for this item within a title within a collection of results </summary>
        string Level2_Text { get; }

        /// <summary> Index of the third serial hierarchy level for this item within a title within a collection of results </summary>
        short Level3_Index { get; }

        /// <summary> Text of the third serial hierarchy level for this item within a title within a collection of results </summary>
        string Level3_Text { get; }

        /// <summary> Publication date for this item within a title within a collection of results </summary>
        string PubDate { get; }

        /// <summary> Number of pages within this item within a title within a collection of results </summary>
        int PageCount { get; }

        /// <summary> External URL for this item within a title within a collection of results </summary>
        string Link { get; }

		/// <summary> Spatial coverage as KML for this item within a title result for map display </summary>
		string Spatial_KML { get; }

		/// <summary> COinS OpenURL format of citation for citation sharing </summary>
		string COinS_OpenURL { get; }
    }
}
