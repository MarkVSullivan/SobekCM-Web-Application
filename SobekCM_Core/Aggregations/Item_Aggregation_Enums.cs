#region Using directives

using System;

#endregion

namespace SobekCM.Core.Aggregations
{

    /// <summary> Enumeration holds all the various types of collection level views </summary>
    /// <remarks> These roughly correspond to the collection viewer class used by the html collection subwriter </remarks>
    [Serializable]
    public enum Item_Aggregation_Views_Searches_Enum : byte
    {
        /// <summary> No collcetion view or search </summary>
        /// <remarks> This enum value is needed for serialization of some item aggregationPermissions </remarks>
        NONE = 0,

        /// <summary> Admin view gives access to aggregation administrative features  </summary>
        Admin_View = 1,

        /// <summary> Advanced search type allows boolean searching with four different search fields </summary>
        Advanced_Search,

        /// <summary> Advanced search type allows boolean searching with four different search fields but also
        ///  ability to select items that have SOME mimetype ( i.e., some digital resources ) </summary>
        Advanced_Search_MimeType,

        /// <summary> Advanced search type allows boolean searching with four different search fields 
        /// and allows a range of years to be included in the search </summary>
        Advanced_Search_YearRange,

        /// <summary> Browse the list of all items or new items </summary>
        All_New_Items,

        /// <summary> Basic search type allows metadata searching with one search field </summary>
        Basic_Search,

        /// <summary> Basic search type that includes an option for the user to include full text search in their results </summary>
        Basic_Search_FullTextOption,

        /// <summary> Basic search type allows metadata searching with one search field but also ability to 
        /// select items that have SOME mimetype ( i.e., some digital resources )</summary>
        Basic_Search_MimeType,

        /// <summary> Basic search type allows metadata searching with one search field and allows a
        /// range of years to be included in the search </summary>
        Basic_Search_YearRange,

        /// <summary> Custom home page overrides most of the normal home page writing mechanism and just displays
        /// a static HTML file </summary>
        Custom_Home_Page,

        /// <summary> Browse from a dataset which is pulled in some manner </summary>
        DataSet_Browse,

        /// <summary> dLOC search is a basic search which also includes a check box to exclude or include newspapers </summary>
        DLOC_FullText_Search,

        /// <summary> Full text search allows the full text of the documents to be searched </summary>
        FullText_Search,

        /// <summary> View the item count information for this single aggregation from within the collection viewer wrapper </summary>
        Item_Count,

        /// <summary> Menu for aggregation curators and admins gives access to all the
        /// special aggregation views they can use </summary>
        Manage_Menu,

        /// <summary> View all of the coordinates points present for an item aggregation </summary>
        Map_Browse,

        /// <summary> View all of the coordinates points present for an item aggregation </summary>
        Map_Browse_Beta,

        /// <summary> Map searching employs a map to allow the user to select a rectangle of interest </summary>
        Map_Search,

        /// <summary> Map searching employs a map to allow the user to select a rectangle of interest </summary>
        Map_Search_Beta,

        /// <summary> Browse by metadata feature allows a user to see every piece of data in a particular metadata field </summary>
        Metadata_Browse,

        /// <summary> Newspaper search type allows searching with one search field and suggests several metadata fields to search (i.e., newspaper title, full text, location, etc..) </summary>
        Newspaper_Search,

        /// <summary> Home page which has no searching enabled </summary>
        No_Home_Search,

        /// <summary> Home page search which includes the rotating highlight to the left of a special banner </summary>
        Rotating_Highlight_Search,

        /// <summary> Home page search which includes the rotating highlight to the left of a special banner with one 
        /// search field but also ability to select items that have SOME mimetype ( i.e., some digital resources )</summary>
        Rotating_Highlight_MimeType_Search,

        /// <summary> Static browse or info view with simply displays static html within the collection wrapper </summary>
        Static_Browse_Info,

        /// <summary> View the usage statistics for this single aggregation from within the collection viewer wrapper </summary>
        Usage_Statistics,

        /// <summary> Displays all the users with their permissions, either individually assigned 
        /// or assigned through a group, that have special permissions assigned for this aggregation </summary>
        User_Permissions,

        /// <summary> Views the list of all private items which are in this aggregation from within the collection viewer wrapper </summary>
        View_Private_Items,

        /// <summary> List of all changes against the aggregation itself, such as design or
        /// other administrative changes </summary>
        Work_History,
    }


    /// <summary> Tells whether this is a browse to appear on the home page, a browse on the Browse By.. portion,
    /// or an information page with not explicit links </summary>
    public enum Item_Aggregation_Child_Visibility_Enum : byte
    {
        /// <summary> This is a child page which appears on the metadata Browse By.. screen </summary>
        Metadata_Browse_By = 1,

        /// <summary> This is a child page which can appear on the item aggregation main menu</summary>
        Main_Menu,

        /// <summary> This is an child page page which does not have its link automatically advertised </summary>
        None
    }

    /// <summary> Specifies the source of this browse or info page and the type of  </summary>
    public enum Item_Aggregation_Child_Source_Data_Enum : byte
    {
        /// <summary> This browse or info page is pulled from the database </summary>
        Database_Table = 1,

        /// <summary> This browse or info page is pulled from a static (usually HTML) file </summary>
        Static_HTML
    };

    /// <summary> Type of item aggregation front banner -- either FULL, LEFT, or RIGHT </summary>
    public enum Item_Aggregation_Front_Banner_Type_Enum : byte
    {
        /// <summary> This is a full-width banner, and does not include
        /// the rotating highlights feature </summary>
        Full,

        /// <summary> The banner sits to the left and the higlights sit 
        /// to the right </summary>
        Left,

        /// <summary> The banner sits to the right and the highlights sit
        /// to the left </summary>
        Right
    }
}
