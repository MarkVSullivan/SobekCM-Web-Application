#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ProtoBuf;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Skins;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary>
    ///   Contains all of the data about a single aggregation of items, such as a collection or
    ///   institutional items.  This class indicates to SobekCM where to look for searches, browses,
    ///   and information.  It also contains all the information for rendering the home pages of each of
    ///   these levels.
    /// </summary>
    [Serializable, DataContract, ProtoContract]
    public class Item_Aggregation
    {

        #region CollectionViewsAndSearchesEnum enum

        /// <summary>
        ///   Enumeration holds all the various types of collection level views
        /// </summary>
        /// <remarks>
        ///   These roughly correspond to the collection viewer class used by the html collection subwriter
        /// </remarks>
        [Serializable]
        public enum CollectionViewsAndSearchesEnum : byte
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

            /// <summary> Views the list of all private items which are in this aggregation from within the collection viewer wrapper </summary>
            View_Private_Items
        }

        #endregion

        #region Private variables

        private string defaultBrowseBy;
        private readonly List<CollectionViewsAndSearchesEnum> viewsAndSearches;
		private readonly Dictionary<string, Item_Aggregation_Child_Page> childPagesHash;
        private readonly Web_Language_Enum defaultUiLanguage;
        private readonly string baseDesignLocation;

        #endregion

        #region Public readonly variables

        /// <summary> ID for this item aggregation object </summary>
        /// <remarks> The AggregationID for the ALL aggregation is set to -1 by the stored procedure </remarks>
        [DataMember(Name = "id"), ProtoMember(1)]
        public int ID { get; private set; }

        /// <summary> Type of item aggregation object </summary>
        [DataMember(Name = "type"), ProtoMember(2)]
        public string Type { get; private set; }

        /// <summary> Code for this item aggregation object </summary>
        [DataMember(Name = "code"), ProtoMember(3)]
        public string Code { get; private set; }

        /// <summary> Date the last item was added to this collection </summary>
        /// <remarks> If there is no record of this, the date of 1/1/2000 is returned </remarks>
        [DataMember(Name = "lastItemAdded"), ProtoMember(4)]
        public DateTime Last_Item_Added { get; set; }

        /// <summary> Flag indicates if this aggregation can potentially include the ALL ITEMS and NEW ITEMS tabs </summary>
        [IgnoreDataMember]
        public bool Can_Browse_Items { get { return (Display_Options.IndexOf("I") >= 0); } }

        #endregion

        #region Constructor

        /// <summary>
        ///   Constructor for a new instance of the Item_Aggregation class
        /// </summary>
        /// <param name="Default_UI_Language"> Default user interface language for this interface </param>
        /// <param name="Base_Design_Location"> Base design location for this instance </param>
        public Item_Aggregation( Web_Language_Enum Default_UI_Language, string Base_Design_Location )
        {
            defaultUiLanguage = Default_UI_Language;
            baseDesignLocation = Base_Design_Location;

            // Set some defaults
            Name = String.Empty;
            ShortName = String.Empty;
            Active = true;
            Hidden = false;
            Map_Search = 0;
            Map_Search_Beta = 0;
            Map_Display = 0;
            Map_Display_Beta = 0;
            OAI_Enabled = false;
            Has_New_Items = false;
            childPagesHash = new Dictionary<string, Item_Aggregation_Child_Page>();
            Advanced_Search_Fields = new List<short>();
            Browseable_Fields = new List<short>();
            Facets = new List<short> {3, 5, 7, 10, 8};

            // Add the default result views
            Result_Views = new List<Result_Display_Type_Enum>
                              {
                                  Result_Display_Type_Enum.Brief,
                                  Result_Display_Type_Enum.Table,
                                  Result_Display_Type_Enum.Thumbnails,
                                  Result_Display_Type_Enum.Full_Citation
                              };
            Default_Result_View = Result_Display_Type_Enum.Brief;
        }

        /// <summary>
        ///   Constructor for a new instance of the Item_Aggregation class
        /// </summary>
        /// <param name="Default_UI_Language"> Default user interface language for this interface </param>
        /// <param name="Base_Design_Location"> Base design location for this instance </param>
        /// <param name = "Code"> Unique code for this item aggregation object </param>
        /// <param name = "Type"> Type of aggregation object (i.e., collection, institution, exhibit, etc..)</param>
        /// <param name = "ID"> ID for this aggregation object from the database </param>
        /// <param name = "Display_Options"> Display options used to determine the views and searches for this item</param>
        /// <param name = "Last_Item_Added"> Date the last item was added ( or 1/1/2000 by default )</param>
        public Item_Aggregation(Web_Language_Enum Default_UI_Language, string Base_Design_Location, string Code, string Type, int ID, string Display_Options, DateTime Last_Item_Added)
        {
            // Save these parameters
            this.Code = Code;
            this.Type = Type;
            this.ID = ID;
            this.Last_Item_Added = Last_Item_Added;
            this.Display_Options = Display_Options;
            defaultUiLanguage = Default_UI_Language;
            baseDesignLocation = Base_Design_Location;

            // Set some defaults
            Name = String.Empty;
            ShortName = String.Empty;
            Active = true;
            Hidden = false;
            Map_Search = 0;
            Map_Search_Beta = 0;
            Map_Display = 0;
            Map_Display_Beta = 0;
            OAI_Enabled = false;
            Has_New_Items = false;
            childPagesHash = new Dictionary<string, Item_Aggregation_Child_Page>();
            Advanced_Search_Fields = new List<short>();
            Browseable_Fields = new List<short>();
            Facets = new List<short> {3, 5, 7, 10, 8};

            // Add the searches and views
            viewsAndSearches = new List<CollectionViewsAndSearchesEnum>();
            string options = Display_Options;
            if (Display_Options.Length == 0)
                options = "BAFI";
            this.Display_Options = options;
            bool home_search_found = false;
            foreach (char thisViewSearch in options)
            {
                switch (thisViewSearch)
                {
					case '0':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.No_Home_Search);
						home_search_found = true;
						break;

					case 'A':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Advanced_Search);
						break;

                    case 'B':
                    case 'D':
                        viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Basic_Search);
                        home_search_found = true;
                        break;

					case 'C':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.DLOC_FullText_Search);
						home_search_found = true;
						break;

                    case 'F':
                        viewsAndSearches.Add(CollectionViewsAndSearchesEnum.FullText_Search);
                        home_search_found = true;
                        break;

					case 'G':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Map_Browse);
						break;

                    case 'g':
                        viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Map_Browse_Beta);
                        break;

					case 'I':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.All_New_Items);
						break;

					case 'M':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Map_Search);
						break;

                    case 'Q':
                        viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Map_Search_Beta);
                        break;

                    case 'N':
                        viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Newspaper_Search);
                        home_search_found = true;
                        break;

                    case 'W':
                        viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Basic_Search_MimeType);
                        home_search_found = true;
                        break;

					case 'X':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Advanced_Search_MimeType);
						break;

                    case 'Y':
                        viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Basic_Search_YearRange);
                        home_search_found = true;
                        break;

					case 'Z':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Advanced_Search_YearRange);
						break;
						
                }
            }

            // If no home search found, add the no home search view
            if (!home_search_found)
            {
                viewsAndSearches.Insert(0, CollectionViewsAndSearchesEnum.No_Home_Search);
            }

            // Add the default result views
            Result_Views = new List<Result_Display_Type_Enum>
                              {
                                  Result_Display_Type_Enum.Brief,
                                  Result_Display_Type_Enum.Table,
                                  Result_Display_Type_Enum.Thumbnails,
                                  Result_Display_Type_Enum.Full_Citation
                              };
            Default_Result_View = Result_Display_Type_Enum.Brief;
        }

        #endregion

        #region Public Properties

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus should appear in the advanced search drop downs  </summary>
        [DataMember(Name = "advancedSearchFields")]
        [ProtoMember(5)]
        public List<short> Advanced_Search_Fields { get; private set; }

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus could appear in the metadata browse </summary>
        [DataMember(Name = "browseableFields")]
        [ProtoMember(6)]
        public List<short> Browseable_Fields { get; private set; }

        /// <summary> Returns the list of all facets to display during searches and browses within this aggregation </summary>
        /// <remarks> This can hold up to eight facets, by primary key for the metadata type.  By default this holds 3,5,7,10, and 8. </remarks>
        [DataMember(Name = "facets")]
        [ProtoMember(7)]
        public List<short> Facets { get; private set; }

        /// <summary> Gets the list of web skins this aggregation can appear under </summary>
        /// <remarks> If no web skins are indicated, this is not restricted to any set of web skins, and can appear under any skin </remarks>
        [DataMember(EmitDefaultValue = false,Name="webSkins")]
        [ProtoMember(8)]
        public List<string> Web_Skins { get; set; }

        /// <summary> Gets the list of custom directives (which function like Server-Side Includes) 
        ///   for this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "customDirectives")]
        [ProtoMember(8)]
        public Dictionary<string, Item_Aggregation_Custom_Directive> Custom_Directives { get; set; }

        /// <summary> Default browse by code, if no code is provided in the request </summary>
        [DataMember(EmitDefaultValue = false, Name = "defaultBrowseBy")]
        [ProtoMember(9)]
        public string Default_BrowseBy
        {
            get { return defaultBrowseBy; }
            set { defaultBrowseBy = value; }
        }

        /// <summary> Gets the default results view mode for this item aggregation </summary>
        [DataMember(Name = "defaultResultView")]
        [ProtoMember(10)]
        public Result_Display_Type_Enum Default_Result_View { get; set; }

        /// <summary> Gets the list of all result views present in this item aggregation </summary>
        [DataMember(Name = "resultsViews")]
        [ProtoMember(11)]
        public List<Result_Display_Type_Enum> Result_Views { get; private set; }

        /// <summary> Number of pages for all the items within this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageCount")]
        [ProtoMember(12)]
        public int? Page_Count { get; set; }

        /// <summary> Number of titles within this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "titleCount")]
        [ProtoMember(13)]
        public int? Title_Count { get; set; }

        /// <summary> Number of items within this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "itemCount")]
        [ProtoMember(14)]
        public int? Item_Count { get; set; }

        /// <summary> Gets the list of highlights associated with this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "highlights")]
        [ProtoMember(15)]
        public List<Item_Aggregation_Highlights> Highlights { get; set; }

        /// <summary> Value indicates if the highlights should be rotating through a number of 
        ///   highlights, or be fixed on a single highlight </summary>
        [DataMember(EmitDefaultValue = false, Name = "rotatingHighlights")]
        [ProtoMember(16)]
        public bool? Rotating_Highlights { get; set; }

        /// <summary> Key to the thematic heading under which this aggregation should appear on the main home page </summary>
        [DataMember(EmitDefaultValue = false, Name = "thematicHeadingId")]
        [ProtoMember(17)]
        public int? Thematic_Heading_ID { get; set; }

        /// <summary> Email address to receive notifications when items load under this aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "loadEmail")]
        [ProtoMember(18)]
        public string Load_Email { get; set; }

        /// <summary> Directory where all design information for this object is found </summary>
        /// <remarks> This always returns the string 'aggregationPermissions/[Code]/' with the code for this item aggregation </remarks>
        [IgnoreDataMember]
        public string ObjDirectory
        {
            get { return "aggregations/" + Code + "/"; }
        }

        /// <summary> Full name of this item aggregation </summary>
        [DataMember(Name = "name")]
        [ProtoMember(19)]
        public string Name { get; set; }

        /// <summary> Short name of this item aggregation </summary>
        /// <remarks> This is an alternate name used for breadcrumbs, etc.. </remarks>
        [DataMember(Name = "shortName")]
        [ProtoMember(20)]
        public string ShortName { get; set; }

        /// <summary> Description of this item aggregation (in the default language ) </summary>
        /// <remarks> This field is displayed on the main home pages if this is part of a thematic collection </remarks>
        [DataMember(Name = "description")]
        [ProtoMember(21)]
        public string Description { get; set; }

        /// <summary> Flag indicating this item aggregation is active </summary>
        [DataMember(Name = "isActive")]
        [ProtoMember(22)]
        public bool Active { get; set; }

        /// <summary> Flag indicating this item aggregation is hidden from most displays </summary>
        /// <remarks> If this item aggregation is active, public users can still be directed to this item aggreagtion, but it will
        ///   not appear in the lists of subaggregations anywhere. </remarks>
        [DataMember(Name = "isHidden")]
        [ProtoMember(23)]
        public bool Hidden { get; set; }

        /// <summary> Display options string for this item aggregation </summary>
        /// <remarks> This defines which views and browses are available for this item aggregation </remarks>
        [DataMember(Name = "displayOptions")]
        [ProtoMember(24)]
        public string Display_Options { get; set; }

        /// <summary> Flag that tells what type of map searching to allow for this item aggregation </summary>
        [DataMember(Name = "mapSearch")]
        [ProtoMember(25)]
        public ushort Map_Search { get; set; }

        /// <summary> Flag that tells what type of map searching to allow for this item aggregation </summary>
        [IgnoreDataMember]
        public ushort Map_Search_Beta { get; set; }

        /// <summary> Flag that tells what type of map display to show for this item aggregation </summary>
        [DataMember(Name = "mapDisplay")]
        [ProtoMember(26)]
        public ushort Map_Display { get; set; }

        /// <summary> Flag that tells what type of map display to show for this item aggregation </summary>
        [IgnoreDataMember]
        public ushort Map_Display_Beta { get; set; }

        /// <summary> Flag indicates whether this item aggregation should be made available via OAI-PMH </summary>
        [DataMember(Name = "oaiEnabled")]
        [ProtoMember(27)]
        public bool OAI_Enabled { get; set; }

        /// <summary> Additional metadata for this item aggregation to be included when providing the list of OAI-PMH sets </summary>
        [DataMember(EmitDefaultValue = false, Name = "oaiMetadata")]
        [ProtoMember(28)]
        public string OAI_Metadata { get; set; }

        /// <summary> Contact email for any correspondance regarding this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "contactEmail")]
        [ProtoMember(29)]
        public string Contact_Email { get; set; }

        /// <summary> Default interface to be used for this item aggregation </summary>
        /// <remarks> This is primarily used for institutional aggregations, but can be utilized anywhere </remarks>
        [DataMember(EmitDefaultValue = false, Name = "defaultSkin")]
        [ProtoMember(30)]
        public string Default_Skin { get; set; }

		/// <summary> Aggregation-level CSS file, if one exists </summary>
        [DataMember(EmitDefaultValue = false, Name = "cssFile")]
        [ProtoMember(31)]
		public string CSS_File { get; set; }

		/// <summary> Custom home page source file, if one exists </summary>
		/// <remarks> This overrides many of the other parts of the item aggregation if in affect </remarks>
        [DataMember(EmitDefaultValue = false, Name = "customHomeFile")]
        [ProtoMember(32)]
		public string Custom_Home_Page_Source_File { get; set; }

        /// <summary> External link associated with this item aggregation  </summary>
        /// <remarks> This shows up in the citation view when an item is linked to this item aggregation </remarks>
        [DataMember(EmitDefaultValue = false, Name = "externalLink")]
        [ProtoMember(33)]
        public string External_Link { get; set;  }

        /// <summary> Flag indicates whether items linked to this item can be described by logged in users  </summary>
        [DataMember(EmitDefaultValue = false, Name = "canItemsBeDescribed")]
        [ProtoMember(34)]
        public short Items_Can_Be_Described { get; set; }

		/// <summary> The common type of all child collections, or the default </summary>
        [DataMember(EmitDefaultValue = false, Name = "childTypes")]
        [ProtoMember(35)]
		public string Child_Types { get; set; }

        /// <summary> Gets the number of browses and info pages attached to this item aggregation </summary>
        [IgnoreDataMember]
        public int Browse_Info_Count
        {
            get { return childPagesHash.Count; }
        }

        /// <summary> Flag indicates if this item aggregation has at least one BROWSE BY page to display </summary>
        [IgnoreDataMember]
        public bool Has_Browse_By_Pages
        {
            get
            {
                return childPagesHash.Values.Any( ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY);
            }
        }

        /// <summary> Read-only list of all the info objects attached to this item aggregation </summary>
        /// <remarks> These are returned in alphabetical order of the SUBMODE CODE portion of each info </remarks>
        [IgnoreDataMember]
        public ReadOnlyCollection<Item_Aggregation_Child_Page> Info_Pages
        {
            get
            {
                SortedList<string, Item_Aggregation_Child_Page> otherInfos =
                    new SortedList<string, Item_Aggregation_Child_Page>();
                foreach (Item_Aggregation_Child_Page thisInfo in childPagesHash.Values.Where(ThisInfo => ThisInfo.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.NONE))
                {
                    otherInfos[thisInfo.Code] = thisInfo;
                }
                return new ReadOnlyCollection<Item_Aggregation_Child_Page>(otherInfos.Values);
            }
        }

        /// <summary> Read-only list of collection views and searches for this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "viewsAndSearches")]
        [ProtoMember(36)]
        public ReadOnlyCollection<CollectionViewsAndSearchesEnum> Views_And_Searches
        {
            get { return new ReadOnlyCollection<CollectionViewsAndSearchesEnum>(viewsAndSearches); }
        }

        /// <summary> Read-only list of searches types for this item aggregation </summary>
        [IgnoreDataMember]
        public ReadOnlyCollection<Search_Type_Enum> Search_Types
        {
            get
            {
                List<Search_Type_Enum> returnValue = new List<Search_Type_Enum>();
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Basic_Search))
                    returnValue.Add(Search_Type_Enum.Basic);
				if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Basic_Search_YearRange))
					returnValue.Add(Search_Type_Enum.Basic);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Basic_Search_MimeType))
                    returnValue.Add(Search_Type_Enum.Basic);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Newspaper_Search))
                    returnValue.Add(Search_Type_Enum.Newspaper);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Map_Search))
                    returnValue.Add(Search_Type_Enum.Map);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Map_Search_Beta))
                    returnValue.Add(Search_Type_Enum.Map_Beta);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.FullText_Search))
                    returnValue.Add(Search_Type_Enum.Full_Text);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.DLOC_FullText_Search))
                    returnValue.Add(Search_Type_Enum.dLOC_Full_Text);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Advanced_Search))
                    returnValue.Add(Search_Type_Enum.Advanced);
				if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Advanced_Search_YearRange))
					returnValue.Add(Search_Type_Enum.Advanced);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Advanced_Search_MimeType))
                    returnValue.Add(Search_Type_Enum.Advanced);

                return new ReadOnlyCollection<Search_Type_Enum>(returnValue);
            }
        }

        /// <summary> Flag indicates if this aggregation has had any changes over the last two weeks </summary>
        /// <remarks> This, in part, controls whether the NEW ITEMS tab will appear for this item aggregation </remarks>
        [IgnoreDataMember]
        public bool Show_New_Item_Browse
        {
            get
            {
                // See if the last item added date was within the last two weeks
                TimeSpan sinceLastItem = DateTime.Now.Subtract(Last_Item_Added);
                return (sinceLastItem.TotalDays <= 14);
            }
        }

        /// <summary> Flag indicates if new items have recently been added to this collection which requires additional collection-level work </summary>
        /// <remarks> This flag is used by the builder to determine if the static collection level pages should be recreated and if the search index for this aggregation should be rebuilt. </remarks>
        [IgnoreDataMember]
        public bool Has_New_Items { get; set; }

        /// <summary> Gets the number of child item aggregationPermissions present </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref = "Children" /> property.  Even if 
        ///   there are no children, the Children property creates a readonly collection to pass back out. </remarks>
        [IgnoreDataMember]
        public int Children_Count
        {
            get
            {
                if (Children == null) return -1;
                return Children.Count(ThisChild => (ThisChild.Active) && (!ThisChild.Hidden));
            }
        }

        /// <summary> Gets the read-only collection of children item aggregation objects </summary>
        /// <remarks> You should check the count of children first using the <see cref = "Children_Count" /> before using this property.
        ///   Even if there are no children, this property creates a readonly collection to pass back out. </remarks>
        [DataMember(EmitDefaultValue = false, Name = "children"), ProtoMember(37)]
        public List<Item_Aggregation_Related_Aggregations> Children { get; set; }

        /// <summary> Gets the read-only collection of parent item aggregation objects </summary>
        /// <remarks> You should check the count of parents first using the <see cref = "Parent_Count" /> before using this property.
        ///   Even if there are no parents, this property creates a readonly collection to pass back out. </remarks>
        [DataMember(EmitDefaultValue = false, Name = "parents"), ProtoMember(38)]
        public List<Item_Aggregation_Related_Aggregations> Parents { get; set; }

        /// <summary> Collection of all child pages </summary>
        [DataMember(EmitDefaultValue = false, Name = "childPages"), ProtoMember(39)]
        public List<Item_Aggregation_Child_Page> Child_Pages { get; set; }

        /// <summary> Any aggregation-specific contact form configuration, otherwise NULL </summary>
        [DataMember(EmitDefaultValue = false, Name = "contactForm"), ProtoMember(40)]
        public ContactForm_Configuration ContactForm { get; set; }

        /// <summary> Gets the raw home page source file </summary>
        [DataMember(EmitDefaultValue = false, Name = "homePageFiles"), ProtoMember(41)]
        public Dictionary<Web_Language_Enum, string> Home_Page_File_Dictionary { get; private set; }

        /// <summary> Get the standard banner dictionary, by language </summary>
        [DataMember(EmitDefaultValue = false, Name = "banners"), ProtoMember(42)]
        public Dictionary<Web_Language_Enum, string> Banner_Dictionary { get; private set; }

        /// <summary> Get the front banner dictionary, by language </summary>
        [DataMember(EmitDefaultValue = false, Name = "frontBanners"), ProtoMember(43)]
        public Dictionary<Web_Language_Enum, Item_Aggregation_Front_Banner> Front_Banner_Dictionary { get; private set; }

        /// <summary> Removes a child from this collection, by aggregation code </summary>
		/// <param name="AggregationCode"> Code of the child to remove </param>
		public void Remove_Child(string AggregationCode)
		{
			if (Children == null) return;

			// Get list of matches
			List<Item_Aggregation_Related_Aggregations> removes = Children.Where(ThisChild => ThisChild.Code == AggregationCode).ToList();

			// Remove all matches
			foreach (Item_Aggregation_Related_Aggregations toRemove in removes)
			{
				Children.Remove(toRemove);
			}
		}

        /// <summary> Gets the number of parent item aggregationPermissions present </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref = "Parents" /> property.  Even if 
        ///   there are no parents, the Parent property creates a readonly collection to pass back out. </remarks>
        [IgnoreDataMember]
        public int Parent_Count
        {
            get
            {
	            return Parents == null ? 0 : Parents.Count;
            }
        }


        /// <summary> Returns the list of all parent codes to this aggregation, seperate by a semi-colon  </summary>
        [IgnoreDataMember]
        public string Parent_Codes
        {
            get
            {
                if ((Parents == null) || (Parents.Count == 0))
                    return String.Empty;

                if (Parents.Count == 1)
                    return Parents[0].Code;

                StringBuilder builder = new StringBuilder();
                foreach (Item_Aggregation_Related_Aggregations thisParent in Parents)
                {
                    if (builder.Length == 0)
                        builder.Append(thisParent.Code);
                    else
                        builder.Append(" ; " + thisParent.Code);                   
                }
                return builder.ToString();
            }
        }

        /// <summary> Clears all the facets in this item aggregation </summary>
        public void Clear_Facets()
        {
            Facets.Clear();
        }

        /// <summary> Adds a single facet type, by primary key, to this item aggregation's browse and search result pages </summary>
        /// <param name = "New_Facet_ID"> Primary key for the metadata type to include as a facet </param>
        public void Add_Facet(short New_Facet_ID)
        {
            Facets.Add(New_Facet_ID);
        }

        /// <summary> Add a web skin which this aggregation can appear under </summary>
        /// <param name = "Web_Skin"> Web skin this can appear under </param>
        public void Add_Web_Skin(string Web_Skin)
        {
            if (Web_Skins == null) Web_Skins = new List<string>();

            Web_Skins.Add(Web_Skin);
            if ( String.IsNullOrEmpty(Default_Skin))
                Default_Skin = Web_Skin;
        }

        /// <summary> Get a child page by code </summary>
		/// <param name="ChildCode"> Code for this child page </param>
		/// <returns> Either the matching page, or NULL </returns>
		public Item_Aggregation_Child_Page Child_Page_By_Code(string ChildCode)
		{
			return childPagesHash.ContainsKey(ChildCode.ToUpper()) ? childPagesHash[ChildCode.ToUpper()] : null;
		}

	    /// <summary> Add a child page to this item aggregatiion </summary>
		/// <param name="ChildPage"> New child page to add </param>
		public void Add_Child_Page(Item_Aggregation_Child_Page ChildPage)
	    {
            if (Child_Pages == null)
                Child_Pages = new List<Item_Aggregation_Child_Page>();

		    string upper_code = ChildPage.Code.ToUpper();
			if (childPagesHash.ContainsKey(upper_code))
			{
				childPagesHash.Remove(upper_code);
				Child_Pages.RemoveAll(CurrentPage => CurrentPage.Code.ToUpper() == upper_code);
			}

			Child_Pages.Add(ChildPage);
			childPagesHash[upper_code] = ChildPage;
		}

		/// <summary> Add a new browse or info object to this hierarchical object </summary>
		/// <param name = "Browse_Type">Flag indicates if this is a BROWSE or INFO object</param>
		/// <param name = "Browse_Code">SubMode indicator for this object</param>
		/// <param name = "StaticHtmlSource">Any static HTML source to be used for display</param>
		/// <param name = "Text">Text to display for this browse</param>
		/// <returns>The built data object</returns>
		public Item_Aggregation_Child_Page Add_Child_Page(Item_Aggregation_Child_Page.Visibility_Type Browse_Type, string Browse_Code, string StaticHtmlSource, string Text)
		{
			// Create the new Browse_Info object
			Item_Aggregation_Child_Page childPage = new Item_Aggregation_Child_Page(Browse_Type, Item_Aggregation_Child_Page.Source_Type.Database, Browse_Code, StaticHtmlSource, Text);

		    if (Child_Pages == null)
		        Child_Pages = new List<Item_Aggregation_Child_Page>();

			// Add this to the Hash table
			Child_Pages.Add(childPage);
			childPagesHash[Browse_Code.ToUpper()] = childPage;

			return childPage;
		}

        /// <summary> Read-only list of all the browse objects to appear on the home page attached to this item aggregation </summary>
        /// <param name = "Current_Language"> Current language used to sort the browses by the label </param>
        /// <remarks> These are returned in alphabetical order of the LABEL portion of each browse, according to the provided language </remarks>
        public ReadOnlyCollection<Item_Aggregation_Child_Page> Browse_Home_Pages(Web_Language_Enum Current_Language)
        {
            SortedList<string, Item_Aggregation_Child_Page> otherBrowses = new SortedList<string, Item_Aggregation_Child_Page>();
            foreach (Item_Aggregation_Child_Page thisBrowse in childPagesHash.Values.Where(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU))
            {
                otherBrowses[thisBrowse.Get_Label(Current_Language)] = thisBrowse;
            }
            return new ReadOnlyCollection<Item_Aggregation_Child_Page>(otherBrowses.Values);
        }

        /// <summary> Read-only list of all the browse objects to appear under the BROWSE BY attached to this item aggregation </summary>
        /// <param name = "Current_Language"> Current language used to sort the browses by the label </param>
        /// <remarks> These are returned in alphabetical order of the CODE portion of each browse, according to the provided language </remarks>
        public ReadOnlyCollection<Item_Aggregation_Child_Page> Browse_By_Pages(Web_Language_Enum Current_Language)
        {
            SortedList<string, Item_Aggregation_Child_Page> otherBrowses = new SortedList<string, Item_Aggregation_Child_Page>();
            foreach (Item_Aggregation_Child_Page thisBrowse in childPagesHash.Values.Where(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY))
            {
                otherBrowses[thisBrowse.Code] = thisBrowse;
            }
            return new ReadOnlyCollection<Item_Aggregation_Child_Page>(otherBrowses.Values);
        }

        #endregion


        #region Internal methods used to build this object

        /// <summary> Method adds another aggregation as a child of this </summary>
        /// <param name = "Child_Aggregation">New child aggregation</param>
        public void Add_Child_Aggregation(Item_Aggregation_Related_Aggregations Child_Aggregation)
        {
            // If the list is currently null, create it
            if (Children == null)
            {
                Children = new List<Item_Aggregation_Related_Aggregations> {Child_Aggregation};
            }
            else
            {
                // If this does not exist, add it
                if (!Children.Contains(Child_Aggregation))
                {
                    Children.Add(Child_Aggregation);
                }
            }
        }

        /// <summary> Method adds another aggregation as a parent to this </summary>
        /// <param name = "Parent_Aggregation">New parent aggregation</param>
        public void Add_Parent_Aggregation(Item_Aggregation_Related_Aggregations Parent_Aggregation)
        {
            // If the list is currently null, create it
            if (Parents == null)
            {
                Parents = new List<Item_Aggregation_Related_Aggregations> {Parent_Aggregation};
            }
            else
            {
                // If this does not exist, add it
                if (!Parents.Contains(Parent_Aggregation))
                {
                    Parents.Add(Parent_Aggregation);
                }
            }
        }

        /// <summary> Add the home page source file information, by language </summary>
        /// <param name = "Home_Page_File"> Home page text source file </param>
        /// <param name = "Language"> Language code </param>
        public void Add_Home_Page_File(string Home_Page_File, Web_Language_Enum Language)
        {
            if (Home_Page_File_Dictionary == null)
                Home_Page_File_Dictionary = new Dictionary<Web_Language_Enum, string>();

            // If no language code, then always use this as the default
            if (Language == Web_Language_Enum.DEFAULT)
            {
                Home_Page_File_Dictionary[defaultUiLanguage] = Home_Page_File;
            }
            else
            {
                // Save this under the normalized language code
                Home_Page_File_Dictionary[Language] = Home_Page_File;
            }
        }

        /// <summary> Add the main banner image for this aggregation, by language </summary>
        /// <param name = "Banner_Image"> Main banner image source file for this aggregation </param>
        /// <param name = "Language"> Language code </param>
        public void Add_Banner_Image(string Banner_Image, Web_Language_Enum Language)
        {
            if (Banner_Dictionary == null)
                Banner_Dictionary = new Dictionary<Web_Language_Enum, string>();

            // If no language code, then always use this as the default
            if (Language == Web_Language_Enum.DEFAULT)
            {
                Banner_Dictionary[defaultUiLanguage] = Banner_Image;
            }
            else
            {
                // Save this under the normalized language code
                Banner_Dictionary[Language] = Banner_Image;
            }
        }

        /// <summary>
        ///   Add the special front banner image that displays on the home page only for this aggregation, by language
        /// </summary>
        /// <param name = "Banner_Image"> special front banner image source file for this aggregation </param>
        /// <param name = "Language"> Language code </param>
        /// <returns> Build front banner image information object </returns>
        public Item_Aggregation_Front_Banner Add_Front_Banner_Image(string Banner_Image, Web_Language_Enum Language)
        {
			Item_Aggregation_Front_Banner banner = new Item_Aggregation_Front_Banner(Banner_Image);

            if (Front_Banner_Dictionary == null)
                Front_Banner_Dictionary = new Dictionary<Web_Language_Enum, Item_Aggregation_Front_Banner>();

            // If no language code, then always use this as the default
            if (Language == Web_Language_Enum.DEFAULT)
            {
                Front_Banner_Dictionary[defaultUiLanguage] = banner;
            }
            else
            {
                // Save this under the normalized language code
				Front_Banner_Dictionary[Language] = banner;
            }
	        return banner;
        }

        /// <summary>
        ///   Add the special front banner image that displays on the home page only for this aggregation, by language
        /// </summary>
        /// <param name = "Banner"> special front banner image source file for this aggregation </param>
        /// <param name = "Language"> Language code </param>
        /// <returns> Build front banner image information object </returns>
        public Item_Aggregation_Front_Banner Add_Front_Banner_Image(Item_Aggregation_Front_Banner Banner, Web_Language_Enum Language)
        {
            if (Front_Banner_Dictionary == null)
                Front_Banner_Dictionary = new Dictionary<Web_Language_Enum, Item_Aggregation_Front_Banner>();

            // If no language code, then always use this as the default
            if (Language == Web_Language_Enum.DEFAULT)
            {
                Front_Banner_Dictionary[defaultUiLanguage] = Banner;
            }
            else
            {
                // Save this under the normalized language code
                Front_Banner_Dictionary[Language] = Banner;
            }
            return Banner;
        }

        #endregion

        #region Methods to return the appropriate banner HTML

        /// <summary> Clear the list of banners, by language </summary>
        public void Clear_Banners()
        {
            Front_Banner_Dictionary = null;
            Banner_Dictionary = null;
        }

        /// <summary>
        ///   Gets the banner image for this aggregation, by language
        /// </summary>
        /// <param name = "Language"> Language code </param>
        /// <param name = "ThisWebSkin"> Web skin object which may override the banner</param>
        /// <returns> Either the language-specific banner image, or else the default banner image</returns>
        /// <remarks>
        ///   If NO banner images were included in the aggregation XML, then this could be the empty string.<br /><br />
        ///   If the provided web skin overrides the banner, then use that web skin's banner.
        /// </remarks>
        public string Banner_Image(Web_Language_Enum Language, SobekCM_Skin_Object ThisWebSkin)
        {
            // Does the web skin exist and override the banner?  For non-institutional agggregations
            // use the web skin banner HTML instead of the aggregation's banner
            if ((ThisWebSkin != null) && (ThisWebSkin.Override_Banner) && (Type.ToLower().IndexOf("institution") < 0))
            {
                return ThisWebSkin.Banner_HTML;
            }

            if (Banner_Dictionary == null)
                return String.Empty;


            // Does this language exist in the banner image lookup dictionary?
            if (Banner_Dictionary.ContainsKey(Language))
            {
                return "design/" + ObjDirectory + Banner_Dictionary[Language];
            }

            // Default to the system language then
            if (Banner_Dictionary.ContainsKey(defaultUiLanguage))
            {
                return "design/" + ObjDirectory + Banner_Dictionary[defaultUiLanguage];
            }

            // Just return the first, assuming one exists
            return Banner_Dictionary.Count > 0 ? Banner_Dictionary.ElementAt(0).Value : String.Empty;
        }



        /// <summary> Gets the special front banner image for this aggregation's home page, by language </summary>
        /// <param name = "Language"> Language code </param>
        /// <returns> Either the language-specific special front banner image, or else the default front banner image</returns>
        /// <remarks>
        ///   If NO special front banner images were included in the aggregation XML, then this could be NULL. <br /><br />
        ///   This is a special front banner image used for aggregationPermissions that show the highlighted
        ///   item and the search box in the main banner at the top on the front page
        /// </remarks>
		public Item_Aggregation_Front_Banner Front_Banner_Image(Web_Language_Enum Language)
        {
            if (Front_Banner_Dictionary == null)
                return null;

            // Does this language exist in the banner image lookup dictionary?
            if (Front_Banner_Dictionary.ContainsKey(Language))
            {
                return Front_Banner_Dictionary[Language];
            }

            // Default to the system language then
            if (Front_Banner_Dictionary.ContainsKey(defaultUiLanguage))
            {
                return Front_Banner_Dictionary[defaultUiLanguage];
            }

            // Just return the first, assuming one exists
            return Front_Banner_Dictionary.Count > 0 ? Front_Banner_Dictionary.ElementAt(0).Value : null;
        }

        #endregion

        #region Method to return appropriate home page source file, or the home page HTML

        /// <summary> Removes a single home page from the collection of home pages </summary>
		/// <param name="Language"> Language of the home page to remove </param>
		public void Delete_Home_Page(Web_Language_Enum Language)
		{
            if ( Home_Page_File_Dictionary != null )
    			Home_Page_File_Dictionary.Remove(Language);
		}

        /// <summary>
        ///   Gets the home page source file for this aggregation, by language
        /// </summary>
        /// <param name = "Language"> Language code </param>
        /// <returns> Either the language-specific home source file, or else the default home file</returns>
        /// <remarks>
        ///   If NO home page files were included in the aggregation XML, then this could be the empty string.
        /// </remarks>
        public string Home_Page_File(Web_Language_Enum Language)
        {
            if (Home_Page_File_Dictionary == null)
                return String.Empty;

            // Does this language exist in the home page file lookup dictionary?
            if (Home_Page_File_Dictionary.ContainsKey(Language))
            {
                return Home_Page_File_Dictionary[Language];
            }

            // Default to the system language then
            if (Home_Page_File_Dictionary.ContainsKey(defaultUiLanguage))
            {
                return Home_Page_File_Dictionary[defaultUiLanguage];
            }

            // Just return the first, assuming one exists
            return Home_Page_File_Dictionary.Count > 0 ? Home_Page_File_Dictionary.ElementAt(0).Value : String.Empty;
        }

        /// <summary>
        ///   Method gets the HOME PAGE html for the appropriate UI settings
        /// </summary>
        /// <param name = "Language"> Current language of the user interface </param>
        /// <param name = "Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns>Home page HTML</returns>
        public string Get_Home_HTML(Web_Language_Enum Language, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation.Get_Home_HTML", "Reading home text source file");
            }

            // Get the home file source
            string homeFileSource = baseDesignLocation + ObjDirectory + Home_Page_File(Language);

            // If no home file source even found, return a message to that affect
            if (homeFileSource.Length == 0)
            {
                return "<div class=\"error_div\">NO HOME PAGE SOURCE FILE FOUND</div>";
            }

            // Do the rest in a try/catch
            try
            {
                // Does the file exist?
                if (!File.Exists(homeFileSource))
                {
                    return "<div class=\"error_div\">HOME PAGE SOURCE FILE '" + homeFileSource +
                           "' DOES NOT EXIST.</div>";
                }

                // Get the text by language
                StreamReader reader = new StreamReader(homeFileSource);
                string tempHomeHtml = reader.ReadToEnd();
                reader.Close();

                // Ensure that any HTML header and end body tags are removed
                if (tempHomeHtml.IndexOf("<body>") > 0)
                    tempHomeHtml =tempHomeHtml.Substring(tempHomeHtml.IndexOf("<body>") + 6).Replace("</body>", "").Replace("</html>", "");

                // Does the home page have a place for te highlights?
                if (tempHomeHtml.IndexOf("<%HIGHLIGHT%>") >= 0)
                {
                    string highlightHtml = String.Empty;
                    if (( Highlights != null ) && (Highlights.Count > 0))
                    {
                        if (Highlights.Count == 1)
                            highlightHtml = Highlights[0].ToHTML(Language, "<%BASEURL%>" + ObjDirectory);
                        else
                        {
                            int dayInteger = DateTime.Now.DayOfYear + 1;
                            int highlightToUse = dayInteger%Highlights.Count;
                            highlightHtml = Highlights[highlightToUse].ToHTML(Language, "<%BASEURL%>" + ObjDirectory);
                        }
                    }

                    // Return the home page for this
                    return tempHomeHtml.Replace("<%HIGHLIGHT%>", highlightHtml);
                }
                
                return tempHomeHtml;
            }
            catch (Exception ee)
            {
                return "<div class=\"error_div\">EXCEPTION CAUGHT WHILE TRYING TO READ THE HOME PAGE SOURCE FILE '" + homeFileSource + "'.<br /><br />ERROR: " + ee.Message + "</div>";
            }
        }

        #endregion

        #region Methods to support BROWSE and INFO objects

        /// <summary> Remove an existing browse or info object from this item aggregation </summary>
        /// <param name="Browse_Page"> Child page information to remove </param>
        public void Remove_Child_Page( Item_Aggregation_Child_Page Browse_Page )
        {
	        if (childPagesHash.ContainsKey(Browse_Page.Code.ToUpper()))
	        {
		        childPagesHash.Remove(Browse_Page.Code.ToUpper());
	        }
	        Child_Pages.Remove(Browse_Page);
        }

		/// <summary> Remove an existing browse or info object from this item aggregation </summary>
		/// <param name="Browse_Page_Code"> Child page information to remove </param>
		public void Remove_Child_Page(string Browse_Page_Code)
		{
			if (childPagesHash.ContainsKey(Browse_Page_Code.ToUpper()))
			{
				Item_Aggregation_Child_Page childPage = childPagesHash[Browse_Page_Code.ToUpper()];

				childPagesHash.Remove(Browse_Page_Code.ToUpper());

				Child_Pages.Remove(childPage);
			}
		}

        /// <summary> Gets the browse or info object from this hierarchy </summary>
        /// <param name = "SubMode">Submode for the browse being requested</param>
        /// <returns>All the information about how to retrieve the browse data</returns>
        public Item_Aggregation_Child_Page Get_Browse_Info_Object(string SubMode)
        {
            return childPagesHash.ContainsKey(SubMode.ToUpper()) ? childPagesHash[SubMode.ToUpper()] : null;
        }

        /// <summary> Checks to see if a particular browse code exists in the list of browses or infos for this item aggregation </summary>
        /// <param name = "Browse_Code"> Code for the browse or info to check for existence </param>
        /// <returns> TRUE if this browse exists, otherwise FALSE </returns>
        public bool Contains_Browse_Info(string Browse_Code)
        {
            return childPagesHash.ContainsKey(Browse_Code.ToUpper());
        }



        #endregion

        #region Method to write the Aggregation XML Configuration File

        /// <summary> Write the XML configuration file for this item aggregation </summary>
        /// <param name = "Directory"> Directory within which to write this XML configuration file </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Configuration_File(string Directory)
        {
            try
            {
                string filename = Code.ToLower() + ".xml";

                // Create the writer object
                StreamWriter writer = new StreamWriter(Directory + "\\" + filename, false);

                // Write the header for the XML file
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
                writer.WriteLine();
                writer.WriteLine("<!-- ITEM AGGREGATION CONFIGURATION FILE FOR '" + Code.ToUpper() + "'                                      -->");
                writer.WriteLine("<!--                                                                                    -->");
                writer.WriteLine("<!-- This required file contains most of the configuration information needed for       -->");
                writer.WriteLine("<!-- item aggregations within a SobekCM system.                                         -->");
                writer.WriteLine("<!--                                                                                    -->");
                writer.WriteLine("<!-- For more information, see http://ufdc.ufl.edu/admin/aggrfiles                      -->");
                writer.WriteLine("<!--                                                                                    -->");
                writer.WriteLine("<!-- This file contains the following possible pieces of data:                          -->");
                writer.WriteLine("<!--          1) a number of settings which apply directly to the aggreagtion, such     -->");
                writer.WriteLine("<!--             as required web skin, possible facets, etc..                           -->");
                writer.WriteLine("<!--          2) home page source files, by language if desired                         -->");
                writer.WriteLine("<!--          3) banner source files, by language if desired                            -->");
                writer.WriteLine("<!--          4) result and browse view information, including types and default        -->");
                writer.WriteLine("<!--          5) 'directives' which act as server-side includes for an aggregation      -->");
                writer.WriteLine("<!--          6) highlight information to be displayed in banner or home text           -->");
                writer.WriteLine("<!--          7) browses, with title and source by language                             -->");
                writer.WriteLine();
				writer.WriteLine("<hi:aggregationInfo xmlns:hi=\"http://sobekrepository.org/schemas/sobekcm_aggregation/\" ");
                writer.WriteLine("				  xmlns:xlink=\"http://www.w3.org/1999/xlink\" ");
                writer.WriteLine("				  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
				writer.WriteLine("				  xsi:schemaLocation=\"http://sobekrepository.org/schemas/sobekcm_aggregation/ ");
				writer.WriteLine("									 http://sobekrepository.org/schemas/sobekcm_aggregation.xsd\" >");
                writer.WriteLine();

                // Write the settings portion
                writer.WriteLine("<!-- Simple aggregation-wide settings, including a specified web skin, facets, etc.. -->");
                writer.WriteLine("<hi:settings>");
                writer.WriteLine("    <!-- Webskins here LIMIT the skins that this aggregation can appear under    -->");
                writer.WriteLine("    <!-- and if none appear here, then the aggregation can appear under any      -->");
                writer.WriteLine("    <!-- web skin.  Multiple web skins should appear with a comma between them.  -->");
                if (( Web_Skins != null ) && ( Web_Skins.Count > 0))
                {
                    if (Web_Skins.Count == 1)
                        writer.WriteLine("    <hi:webskins>" + Web_Skins[0] + "</hi:webskins>");
                    else
                    {
                        writer.Write("    <hi:webskins>" + Web_Skins[0]);
                        for (int i = 1; i < Web_Skins.Count; i++)
                            writer.Write("," + Web_Skins[i]);
                        writer.WriteLine("</hi:webskins>");
                    }
                }
                else
                {
                    writer.WriteLine("    <hi:webskins></hi:webskins>");
                }
                writer.WriteLine();

				// Add the aggregation-level CSS file
				if ( !String.IsNullOrEmpty(CSS_File))
				{
					writer.WriteLine("    <!-- Aggregation-level CSS file  -->");
					writer.WriteLine("    <hi:css>" + CSS_File + "</hi:css>");
					writer.WriteLine();
				}

				// Add the custom home page source file
				if (Custom_Home_Page_Source_File.Length > 0)
				{
					writer.WriteLine("    <!-- Custom home page source file  -->");
					writer.WriteLine("    <hi:customhome>" + Custom_Home_Page_Source_File + "</hi:customhome>");
					writer.WriteLine();
				}
				

                writer.WriteLine("    <!-- Facets here indicate which metadata elements should appear as facets    -->");
                writer.WriteLine("    <!-- on the left when viewing browse or search results within this           -->");
                writer.WriteLine("    <!-- aggregation.  These are indicated by the primary key for the metadata   -->");
                writer.WriteLine("    <!-- type in the SobekCM_Metadata_Types table.  Multiple facets should       -->");
                writer.WriteLine("    <!-- appear with a comma between them.                                       -->");
                if (Facets.Count > 0)
                {
                    if (Facets.Count == 1)
                        writer.WriteLine("    <hi:facets>" + Facets[0] + "</hi:facets>");
                    else
                    {
                        writer.Write("    <hi:facets>" + Facets[0]);
                        for (int i = 1; i < Facets.Count; i++)
                            writer.Write("," + Facets[i]);
                        writer.WriteLine("</hi:facets>");
                    }
                }
                else
                {
                    writer.WriteLine("    <hi:facets></hi:facets>");
                }
                writer.WriteLine("</hi:settings>");
                writer.WriteLine();

                // Add the home page information
                writer.WriteLine("<!-- Source files for the home page, by language -->");
                writer.WriteLine("<hi:home>");
                if (Home_Page_File_Dictionary != null)
                {
                    foreach (KeyValuePair<Web_Language_Enum, string> homePair in Home_Page_File_Dictionary)
                    {
                        writer.WriteLine("    <hi:body lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(homePair.Key) + "\">" + homePair.Value + "</hi:body>");
                    }
                }
                writer.WriteLine("</hi:home>");
                writer.WriteLine();

                // Add the banner information
                writer.WriteLine("<!-- Banner source images by language.  Source can either be jpg, gif, or png.     -->");
                writer.WriteLine("<!-- Including a TYPE=HIGHLIGHT indicates a special banner to use for the          -->");
                writer.WriteLine("<!-- aggregation home page which includes a spot for a rotating highlighted item.  -->");
                writer.WriteLine("<!-- If a highlight banner is indicated, the width, height, and side for the       -->");
                writer.WriteLine("<!-- highlights must be included in the hi:highlights section.                     -->");
                writer.WriteLine("<hi:banner>");
                if (Banner_Dictionary != null)
                {
                    foreach (KeyValuePair<Web_Language_Enum, string> homePair in Banner_Dictionary)
                    {
                        writer.WriteLine("    <hi:source lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(homePair.Key) + "\">" + homePair.Value + "</hi:source>");
                    }
                }
                if (Front_Banner_Dictionary != null)
                {
                    foreach (KeyValuePair<Web_Language_Enum, Item_Aggregation_Front_Banner> homePair in Front_Banner_Dictionary)
                    {
                        writer.Write("    <hi:source type=\"HIGHLIGHT\" lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(homePair.Key) + "\"");
                        writer.Write(" height=\"" + homePair.Value.Height + "\" width=\"" + homePair.Value.Width + "\"");
                        switch (homePair.Value.Type)
                        {
                            case Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.FULL:
                                writer.Write(" side=\"FULL\"");
                                break;

                            case Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.LEFT:
                                writer.Write(" side=\"LEFT\"");
                                break;

                            case Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.RIGHT:
                                writer.Write(" side=\"RIGHT\"");
                                break;
                        }

                        writer.WriteLine(">" + homePair.Value.File + "</hi:source>");
                    }
                }
                writer.WriteLine("</hi:banner>");
                writer.WriteLine();

                // Add any changes to the standard views during browses or search results
                writer.WriteLine("<!-- Changes to the standard views included as options when viewing  -->");
                writer.WriteLine("<!-- browse or search results.  Standard views are 'BRIEF', 'TABLE', -->");
                writer.WriteLine("<!-- 'THUMBNAIL', and 'FULL', with 'BRIEF' as default.               -->");
                writer.WriteLine("<!-- Changes are represented here as either ADDS or DELETES from     -->");
                writer.WriteLine("<!-- that standard list of views.                                    -->");
                List<string> adds = new List<string>();
                List<string> deletes = new List<string>
                                           {
                                               "<hi:remove type=\"BRIEF\" />",
                                               "<hi:remove type=\"FULL\" />",
                                               "<hi:remove type=\"TABLE\" />",
                                               "<hi:remove type=\"THUMBNAIL\" />"
                                           };
                foreach (Result_Display_Type_Enum thisType in Result_Views)
                {
                    switch (thisType)
                    {
                        case Result_Display_Type_Enum.Brief:
                            deletes.Remove("<hi:remove type=\"BRIEF\" />");
                            break;

                        case Result_Display_Type_Enum.Full_Citation:
                        case Result_Display_Type_Enum.Full_Image:
                            deletes.Remove("<hi:remove type=\"FULL\" />");
                            break;

                        case Result_Display_Type_Enum.Map:
                            adds.Add("<hi:remove type=\"MAP\" />");
                            break;

                        case Result_Display_Type_Enum.Map_Beta:
                            adds.Add("<hi:remove type=\"MAP_BETA\" />");
                            break;

                        case Result_Display_Type_Enum.Table:
                            deletes.Remove("<hi:remove type=\"TABLE\" />");
                            break;

                        case Result_Display_Type_Enum.Thumbnails:
                            deletes.Remove("<hi:remove type=\"THUMBNAIL\" />");
                            break;
                    }
                }
                switch (Default_Result_View)
                {
                    case Result_Display_Type_Enum.Brief:
                        adds.Add("<hi:add type=\"BRIEF\" default=\"DEFAULT\" />");
                        break;

                    case Result_Display_Type_Enum.Full_Citation:
                    case Result_Display_Type_Enum.Full_Image:
                        adds.Add("<hi:add type=\"FULL\" default=\"DEFAULT\" />");
                        break;

                    case Result_Display_Type_Enum.Map:
                        adds.Remove("<hi:add type=\"MAP\" />");
                        adds.Add("<hi:add type=\"MAP\" default=\"DEFAULT\" />");
                        break;

                    case Result_Display_Type_Enum.Map_Beta:
                        adds.Remove("<hi:add type=\"MAP_BETA\" />");
                        adds.Add("<hi:add type=\"MAP_BETA\" default=\"DEFAULT\" />");
                        break;

                    case Result_Display_Type_Enum.Table:
                        adds.Add("<hi:add type=\"TABLE\" default=\"DEFAULT\" />");
                        break;

                    case Result_Display_Type_Enum.Thumbnails:
                        adds.Add("<hi:add type=\"THUMBNAIL\" default=\"DEFAULT\" />");
                        break;
                }
                if ((adds.Count > 0) || (deletes.Count > 0))
                {
                    writer.WriteLine("  <hi:results>");
                    writer.WriteLine("    <hi:views>");
                    foreach (string thisDelete in deletes)
                        writer.WriteLine("      " + thisDelete);
                    foreach (string thisAdd in adds)
                        writer.WriteLine("      " + thisAdd);
                    writer.WriteLine("    </hi:views>");
                    writer.WriteLine("  </hi:results>");
                }
                else
                {
                    writer.WriteLine("  <hi:results></hi:results>");
                }
                writer.WriteLine();

                // Add the custom derivative information here, if there are some
                if (( Custom_Directives != null ) && ( Custom_Directives.Count > 0))
                {
                    writer.WriteLine(
                        "  <!-- Custom directive allows the user to specify that a certain code, embedded -->");
                    writer.WriteLine(
                        "  <!-- as a directive like <%CODE%> should be replaced with some replacement     -->");
                    writer.WriteLine(
                        "  <!-- html read from a source file.                                             -->");
                    writer.WriteLine("  <hi:directives>");
                    foreach (KeyValuePair<string, Item_Aggregation_Custom_Directive> thisDirective in Custom_Directives)
                    {
                        writer.WriteLine("    <hi:directive>");
                        writer.WriteLine("      <hi:code>" + thisDirective.Value.Code + "</hi:code>");
                        writer.WriteLine("      <hi:source>" + thisDirective.Value.Source_File + "</hi:source>");
                        writer.WriteLine("    </hi:directive>");
                    }
                    writer.WriteLine("  </hi:directives>");
                    writer.WriteLine();
                }

                // Add the highlights, if there are some
                if (( Highlights != null ) && ( Highlights.Count > 0))
                {
                    writer.WriteLine("  <!-- Highlighted items.  Inclusion of more than one will cause the daily -->");
                    writer.WriteLine("  <!-- displayed highlighted item to slowly change through the items.      -->");
                    writer.WriteLine("  <!-- Including TYPE=ROTATING will cause up to eight highlighted items    -->");
                    writer.WriteLine("  <!-- to rotate on the home page or in the special highlight banner.      -->");

                    // Is there a special front banner with highlighted item within it?
                    if (( Front_Banner_Dictionary != null ) && ( Front_Banner_Dictionary.Count > 0))
                    {
                        writer.Write("  <hi:highlights ");
                        if (( Rotating_Highlights.HasValue ) && ( Rotating_Highlights.Value ))
                            writer.WriteLine( "type=\"ROTATING\">");
                        else
                            writer.WriteLine( "type=\"STANDARD\">");
                    }
                    else
                    {
                        writer.WriteLine("  <hi:highlights>");
                    }

                    // Step through each highlight
                    foreach (Item_Aggregation_Highlights thisHighlight in Highlights)
                    {
                        thisHighlight.Write_In_Configuration_XML_File(writer);
                    }
                    writer.WriteLine("  </hi:highlights>");
                    writer.WriteLine();
                }

                // Add all the child pages
	            if (childPagesHash.Count > 0)
	            {
		            foreach (Item_Aggregation_Child_Page browseObject in Child_Pages)
		            {
						// Don't add ALL or NEW here
						if ((String.Compare(browseObject.Code, "all", StringComparison.InvariantCultureIgnoreCase) != 0) && (String.Compare(browseObject.Code, "new", StringComparison.InvariantCultureIgnoreCase) != 0))
			            {
				            browseObject.Write_In_Configuration_XML_File(writer, Default_BrowseBy);
			            }
		            }
		            writer.WriteLine();
	            }

	            // Close the main tag
				writer.WriteLine("</hi:aggregationInfo>");

                // Flush and close the writer
                writer.Flush();
                writer.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


    }
}