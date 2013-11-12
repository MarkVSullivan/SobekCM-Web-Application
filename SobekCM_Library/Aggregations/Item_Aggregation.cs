#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SobekCM.Library.Configuration;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Skins;

#endregion

namespace SobekCM.Library.Aggregations
{
    /// <summary>
    ///   Contains all of the data about a single aggregation of items, such as a collection or
    ///   institutional items.  This class indicates to SobekCM where to look for searches, browses,
    ///   and information.  It also contains all the information for rendering the home pages of each of
    ///   these levels.
    /// </summary>
    [Serializable]
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
            /// <remarks> This enum value is needed for serialization of some item aggregations </remarks>
            NONE = 0,

            /// <summary> Admin view gives access to aggregation administrative features  </summary>
            Admin_View = 1,

            /// <summary> Advanced search type allows boolean searching with four different search fields </summary>
            Advanced_Search,

			/// <summary> Advanced search type allows boolean searching with four different search fields 
			/// and allows a range of years to be included in the search </summary>
			Advanced_Search_YearRange,

            /// <summary> Browse the list of all items or new items </summary>
            All_New_Items,

            /// <summary> Basic search type allows metadata searching with one search field </summary>
            Basic_Search,

			/// <summary> Basic search type allows metadata searching with one search field and allows a
			/// range of years to be included in the search </summary>
			Basic_Search_YearRange,

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

            /// <summary> Map searching employs a map to allow the user to select a rectangle of interest </summary>
            Map_Search,

            /// <summary> Browse by metadata feature allows a user to see every piece of data in a particular metadata field </summary>
            Metadata_Browse,

            /// <summary> Newspaper search type allows searching with one search field and suggests several metadata fields to search (i.e., newspaper title, full text, location, etc..) </summary>
            Newspaper_Search,

            /// <summary> Home page which has no searching enabled </summary>
            No_Home_Search,

            /// <summary> Home page search which includes the rotating highlight to the left of a special banner </summary>
            Rotating_Highlight_Search,

            /// <summary> Static browse or info view with simply displays static html within the collection wrapper </summary>
            Static_Browse_Info,

            /// <summary> View the usage statistics for this single aggregation from within the collection viewer wrapper </summary>
            Usage_Statistics,

            /// <summary> Views the list of all private items which are in this aggregation from within the collection viewer wrapper </summary>
            View_Private_Items
        }

        #endregion

        #region Private variables

        private readonly Dictionary<Web_Language_Enum, string> bannerImagesByLanguage;
        private List<Item_Aggregation_Related_Aggregations> children;
        private string defaultBrowseBy;
		private readonly Dictionary<Web_Language_Enum, Item_Aggregation_Front_Banner> frontBannerImageByLanguage;
        private readonly Dictionary<Web_Language_Enum, string> homeFilesByLanguage;
        private List<Item_Aggregation_Related_Aggregations> parents;
        private readonly List<CollectionViewsAndSearchesEnum> viewsAndSearches;

		private readonly Dictionary<string, Item_Aggregation_Child_Page> childPagesHash;
	    private readonly List<Item_Aggregation_Child_Page> childPages;

        #endregion

        #region Public readonly variables

        /// <summary>
        ///   ID for this item aggregation object
        /// </summary>
        /// <remarks>
        ///   The AggregationID for the ALL aggregation is set to -1 by the stored procedure
        /// </remarks>
        public readonly int Aggregation_ID;

        /// <summary>
        ///   Type of item aggregation object
        /// </summary>
        public readonly string Aggregation_Type;

        /// <summary>
        ///   Code for this item aggregation object
        /// </summary>
        public readonly string Code;

        /// <summary>
        ///   Date the last item was added to this collection
        /// </summary>
        /// <remarks>
        ///   If there is no record of this, the date of 1/1/2000 is returned
        /// </remarks>
        public readonly DateTime Last_Item_Added;

        /// <summary>
        ///   Flag indicates if this aggregation can potentially include the ALL ITEMS and NEW ITEMS tabs
        /// </summary>
        public bool Can_Browse_Items
        {
            get
            {
                return (Display_Options.IndexOf("I") >= 0);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        ///   Constructor for a new instance of the Item_Aggregation class
        /// </summary>
        public Item_Aggregation()
        {
            // Set some defaults
            Metadata_Code = Code;
            Name = String.Empty;
            ShortName = String.Empty;
            Description = String.Empty;
            Is_Active = true;
            Hidden = false;
            Map_Search = 0;
            Map_Display = 0;
            OAI_Flag = false;
            OAI_Metadata = String.Empty;
            Contact_Email = String.Empty;
            Has_New_Items = false;
            Default_Skin = String.Empty;
            Show_New_Item_Browse = false;
            Load_Email = String.Empty;
            childPagesHash = new Dictionary<string, Item_Aggregation_Child_Page>();
            Highlights = new List<Item_Aggregation_Highlights>();
            Rotating_Highlights = false;
            homeFilesByLanguage = new Dictionary<Web_Language_Enum, string>();
            bannerImagesByLanguage = new Dictionary<Web_Language_Enum, string>();
			frontBannerImageByLanguage = new Dictionary<Web_Language_Enum, Item_Aggregation_Front_Banner>();
			childPages = new List<Item_Aggregation_Child_Page>();
            Custom_Directives = new Dictionary<string, Item_Aggregation_Custom_Directive>();
            Title_Count = -1;
            Page_Count = -1;
            Item_Count = -1;
            Web_Skins = new List<string>();
            Advanced_Search_Fields = new List<short>();
            Browseable_Fields = new List<short>();
            Facets = new List<short> {3, 5, 7, 10, 8};
            Thematic_Heading_ID = -1;
	        CSS_File = String.Empty;
	        Custom_Home_Page_Source_File = String.Empty;
	        Child_Types = "SubCollections";

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
        /// <param name = "Code"> Unique code for this item aggregation object </param>
        /// <param name = "Aggregation_Type"> Type of aggregation object (i.e., collection, institution, exhibit, etc..)</param>
        /// <param name = "Aggregation_ID"> ID for this aggregation object from the database </param>
        /// <param name = "Display_Options"> Display options used to determine the views and searches for this item</param>
        /// <param name = "Last_Item_Added"> Date the last item was added ( or 1/1/2000 by default )</param>
        public Item_Aggregation(string Code, string Aggregation_Type, int Aggregation_ID, string Display_Options, DateTime Last_Item_Added)
        {
            // Save these parameters
            this.Code = Code;
            this.Aggregation_Type = Aggregation_Type;
            this.Aggregation_ID = Aggregation_ID;
            this.Last_Item_Added = Last_Item_Added;
            this.Display_Options = Display_Options;

            // Set some defaults
            Metadata_Code = Code;
            Name = String.Empty;
            ShortName = String.Empty;
            Description = String.Empty;
            Is_Active = true;
            Hidden = false;
            Map_Search = 0;
            Map_Display = 0;
            OAI_Flag = false;
            OAI_Metadata = String.Empty;
            Contact_Email = String.Empty;
            Has_New_Items = false;
            Default_Skin = String.Empty;
            Show_New_Item_Browse = false;
            Load_Email = String.Empty;
            childPagesHash = new Dictionary<string, Item_Aggregation_Child_Page>();
            Highlights = new List<Item_Aggregation_Highlights>();
            Rotating_Highlights = false;
            homeFilesByLanguage = new Dictionary<Web_Language_Enum, string>();
            bannerImagesByLanguage = new Dictionary<Web_Language_Enum, string>();
			frontBannerImageByLanguage = new Dictionary<Web_Language_Enum, Item_Aggregation_Front_Banner>();
			childPages = new List<Item_Aggregation_Child_Page>();
            Custom_Directives = new Dictionary<string, Item_Aggregation_Custom_Directive>();
            Title_Count = -1;
            Page_Count = -1;
            Item_Count = -1;
            Web_Skins = new List<string>();
            Advanced_Search_Fields = new List<short>();
            Browseable_Fields = new List<short>();
            Facets = new List<short> {3, 5, 7, 10, 8};
            Thematic_Heading_ID = -1;
			CSS_File = String.Empty;
	        Custom_Home_Page_Source_File = String.Empty;
			Child_Types = "SubCollections";

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

					case 'I':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.All_New_Items);
						break;

					case 'M':
						viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Map_Search);
						break;

                    case 'N':
                        viewsAndSearches.Add(CollectionViewsAndSearchesEnum.Newspaper_Search);
                        home_search_found = true;
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

            // See if the last item added date was within the last two weeks
            TimeSpan sinceLastItem = DateTime.Now.Subtract(Last_Item_Added);
            if (sinceLastItem.TotalDays <= 14)
            {
                Show_New_Item_Browse = true;
            }
        }

        #endregion

        #region Public Properties

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus should appear in the advanced search drop downs  </summary>
        public List<short> Advanced_Search_Fields { get; private set; }

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus could appear in the metadata browse </summary>
        public List<short> Browseable_Fields { get; private set; }

        /// <summary> Returns the list of all facets to display during searches and browses within this aggregation </summary>
        /// <remarks> This can hold up to eight facets, by primary key for the metadata type.  By default this holds 3,5,7,10, and 8. </remarks>
        public List<short> Facets { get; private set; }

        /// <summary> Gets the list of web skins this aggregation can appear under </summary>
        /// <remarks> If no web skins are indicated, this is not restricted to any set of web skins, and can appear under any skin </remarks>
        public List<string> Web_Skins { get; private set; }

        /// <summary> Gets the list of custom directives (which function like Server-Side Includes) 
        ///   for this item aggregation </summary>
        public Dictionary<string, Item_Aggregation_Custom_Directive> Custom_Directives { get; private set; }

        /// <summary> Default browse by code, if no code is provided in the request </summary>
        public string Default_BrowseBy
        {
            get { return defaultBrowseBy ?? String.Empty; }
            set { defaultBrowseBy = value; }
        }

        /// <summary> Gets the default results view mode for this item aggregation </summary>
        public Result_Display_Type_Enum Default_Result_View { get; set; }

        /// <summary> Gets the list of all result views present in this item aggregation </summary>
        public List<Result_Display_Type_Enum> Result_Views { get; private set; }

        /// <summary> Number of pages for all the items within this item aggregation </summary>
        public int Page_Count { get; set; }

        /// <summary> Number of titles within this item aggregation </summary>
        public int Title_Count { get; set; }

        /// <summary> Number of items within this item aggregation </summary>
        public int Item_Count { get; set; }

        /// <summary> Gets the list of highlights associated with this item </summary>
        public List<Item_Aggregation_Highlights> Highlights { get; private set; }

        /// <summary> Value indicates if the highlights should be rotating through a number of 
        ///   highlights, or be fixed on a single highlight </summary>
        public bool Rotating_Highlights { get; set; }

        /// <summary> Key to the thematic heading under which this aggregation should appear on the main home page </summary>
        public int Thematic_Heading_ID { get; set; }

        /// <summary> Email address to receive notifications when items load under this aggregation </summary>
        public string Load_Email { get; set; }

        /// <summary> Directory where all design information for this object is found </summary>
        /// <remarks> This always returns the string 'aggregations/[Code]/' with the code for this item aggregation </remarks>
        public string ObjDirectory
        {
            get { return "aggregations/" + Code + "/"; }
        }

        /// <summary> Metadata aggregation code used to search in the corresponding greenstone collection(s) </summary>
        /// <remarks> If this item aggregation corresponds directly to a greenstone collect, this field will be unnecessary, and therefore blank </remarks>
        public string Metadata_Code { get; set; }

        /// <summary> Full name of this item aggregation </summary>
        public string Name { get; set; }

        /// <summary> Short name of this item aggregation </summary>
        /// <remarks> This is an alternate name used for breadcrumbs, etc.. </remarks>
        public string ShortName { get; set; }

        /// <summary> Description of this item aggregation (in the default language ) </summary>
        /// <remarks> This field is displayed on the main home pages if this is part of a thematic collection </remarks>
        public string Description { get; set; }

        /// <summary> Flag indicating this item aggregation is active </summary>
        public bool Is_Active { get; set; }

        /// <summary> Flag indicating this item aggregation is hidden from most displays </summary>
        /// <remarks> If this item aggregation is active, public users can still be directed to this item aggreagtion, but it will
        ///   not appear in the lists of subaggregations anywhere. </remarks>
        public bool Hidden { get; set; }

        /// <summary> Display options string for this item aggregation </summary>
        /// <remarks> This defines which views and browses are available for this item aggregation </remarks>
        public string Display_Options { get; set; }

        /// <summary> Flag that tells what type of map searching to allow for this item aggregation </summary>
        public ushort Map_Search { get; set; }

        /// <summary> Flag that tells what type of map display to show for this item aggregation </summary>
        public ushort Map_Display { get; set; }

        /// <summary> Flag indicates whether this item aggregation should be made available via OAI-PMH </summary>
        public bool OAI_Flag { get; set; }

        /// <summary> Additional metadata for this item aggregation to be included when providing the list of OAI-PMH sets </summary>
        public string OAI_Metadata { get; set; }

        /// <summary> Contact email for any correspondance regarding this item aggregation </summary>
        public string Contact_Email { get; set; }

        /// <summary> Default interface to be used for this item aggregation </summary>
        /// <remarks> This is primarily used for institutional aggregations, but can be utilized anywhere </remarks>
        public string Default_Skin { get; set; }

		/// <summary> Aggregation-level CSS file, if one exists </summary>
		public string CSS_File { get; set; }

		/// <summary> Custom home page source file, if one exists </summary>
		/// <remarks> This overrides many of the other parts of the item aggregation if in affect </remarks>
		public string Custom_Home_Page_Source_File { get; set; }

        /// <summary> External link associated with this item aggregation  </summary>
        /// <remarks> This shows up in the citation view when an item is linked to this item aggregation </remarks>
        public string External_Link { get; set;  }

        /// <summary> Flag indicates whether items linked to this item can be described by logged in users  </summary>
        public short Items_Can_Be_Described { get; set; }

		/// <summary> The common type of all child collections, or the default </summary>
		public string Child_Types { get; set; }

        /// <summary> Gets the number of browses and info pages attached to this item aggregation </summary>
        public int Browse_Info_Count
        {
            get { return childPagesHash.Count; }
        }

        /// <summary> Flag indicates if this item aggregation has at least one BROWSE BY page to display </summary>
        public bool Has_Browse_By_Pages
        {
            get
            {
                return childPagesHash.Values.Any( ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY);
            }
        }

        /// <summary> Read-only list of all the info objects attached to this item aggregation </summary>
        /// <remarks> These are returned in alphabetical order of the SUBMODE CODE portion of each info </remarks>
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
        public ReadOnlyCollection<CollectionViewsAndSearchesEnum> Views_And_Searches
        {
            get { return new ReadOnlyCollection<CollectionViewsAndSearchesEnum>(viewsAndSearches); }
        }

        /// <summary> Read-only list of searches types for this item aggregation </summary>
        public ReadOnlyCollection<Search_Type_Enum> Search_Types
        {
            get
            {
                List<Search_Type_Enum> returnValue = new List<Search_Type_Enum>();
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Basic_Search))
                    returnValue.Add(Search_Type_Enum.Basic);
				if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Basic_Search_YearRange))
					returnValue.Add(Search_Type_Enum.Basic);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Newspaper_Search))
                    returnValue.Add(Search_Type_Enum.Newspaper);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Map_Search))
                    returnValue.Add(Search_Type_Enum.Map);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.FullText_Search))
                    returnValue.Add(Search_Type_Enum.Full_Text);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.DLOC_FullText_Search))
                    returnValue.Add(Search_Type_Enum.dLOC_Full_Text);
                if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Advanced_Search))
                    returnValue.Add(Search_Type_Enum.Advanced);
				if (viewsAndSearches.Contains(CollectionViewsAndSearchesEnum.Advanced_Search_YearRange))
					returnValue.Add(Search_Type_Enum.Advanced);

                return new ReadOnlyCollection<Search_Type_Enum>(returnValue);
            }
        }

        /// <summary> Flag indicates if this aggregation has had any changes over the last two weeks </summary>
        /// <remarks> This, in part, controls whether the NEW ITEMS tab will appear for this item aggregation </remarks>
        public bool Show_New_Item_Browse { get; set; }

        /// <summary> Flag indicates if new items have recently been added to this collection which requires additional collection-level work </summary>
        /// <remarks> This flag is used by the builder to determine if the static collection level pages should be recreated and if the search index for this aggregation should be rebuilt. </remarks>
        public bool Has_New_Items { get; set; }

        /// <summary> Gets the number of child item aggregations present </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref = "Children" /> property.  Even if 
        ///   there are no children, the Children property creates a readonly collection to pass back out. </remarks>
        public int Children_Count
        {
            get
            {
                if (children == null) return -1;
                return children.Count(ThisChild => (ThisChild.Active) && (!ThisChild.Hidden));
            }
        }

        /// <summary> Gets the read-only collection of children item aggregation objects </summary>
        /// <remarks> You should check the count of children first using the <see cref = "Children_Count" /> before using this property.
        ///   Even if there are no children, this property creates a readonly collection to pass back out. </remarks>
        public ReadOnlyCollection<Item_Aggregation_Related_Aggregations> Children
        {
            get
            {
				return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>(children ?? new List<Item_Aggregation_Related_Aggregations>());
            }
        }

		/// <summary> Removes a child from this collection, by aggregation code </summary>
		/// <param name="AggregationCode"> Code of the child to remove </param>
		public void Remove_Child(string AggregationCode)
		{
			if (children == null) return;

			// Get list of matches
			List<Item_Aggregation_Related_Aggregations> removes = children.Where(ThisChild => ThisChild.Code == AggregationCode).ToList();

			// Remove all matches
			foreach (Item_Aggregation_Related_Aggregations toRemove in removes)
			{
				children.Remove(toRemove);
			}
		}

        /// <summary> Gets the number of parent item aggregations present </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref = "Parents" /> property.  Even if 
        ///   there are no parents, the Parent property creates a readonly collection to pass back out. </remarks>
        public int Parent_Count
        {
            get
            {
	            return parents == null ? 0 : parents.Count;
            }
        }

        /// <summary> Gets the read-only collection of parent item aggregation objects </summary>
        /// <remarks> You should check the count of parents first using the <see cref = "Parent_Count" /> before using this property.
        ///   Even if there are no parents, this property creates a readonly collection to pass back out. </remarks>
        public ReadOnlyCollection<Item_Aggregation_Related_Aggregations> Parents
        {
            get
            {
				return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>( parents ?? new List<Item_Aggregation_Related_Aggregations>());
            }
        }

        /// <summary> Returns the list of all parent codes to this aggregation, seperate by a semi-colon  </summary>
        public string Parent_Codes
        {
            get
            {
                if ((parents == null) || (parents.Count == 0))
                    return String.Empty;

                if (parents.Count == 1)
                    return parents[0].Code;

                StringBuilder builder = new StringBuilder();
                foreach (Item_Aggregation_Related_Aggregations thisParent in parents)
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
        internal void Clear_Facets()
        {
            Facets.Clear();
        }

        /// <summary> Adds a single facet type, by primary key, to this item aggregation's browse and search result pages </summary>
        /// <param name = "New_Facet_ID"> Primary key for the metadata type to include as a facet </param>
        internal void Add_Facet(short New_Facet_ID)
        {
            Facets.Add(New_Facet_ID);
        }

        /// <summary> Add a web skin which this aggregation can appear under </summary>
        /// <param name = "Web_Skin"> Web skin this can appear under </param>
        internal void Add_Web_Skin(string Web_Skin)
        {
            Web_Skins.Add(Web_Skin);
            if ( Default_Skin.Length == 0 )
                Default_Skin = Web_Skin;
        }

		/// <summary> Collection of all child pages </summary>
	    public ReadOnlyCollection<Item_Aggregation_Child_Page> Child_Pages
	    {
		    get
		    {
			    return new ReadOnlyCollection<Item_Aggregation_Child_Page>(childPages);
		    }
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
		    string upper_code = ChildPage.Code.ToUpper();
			if (childPagesHash.ContainsKey(upper_code))
			{
				childPagesHash.Remove(upper_code);
				childPages.RemoveAll(CurrentPage => CurrentPage.Code.ToUpper() == upper_code);
			}

			childPages.Add(ChildPage);
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

			// Add this to the Hash table
			childPages.Add(childPage);
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
        internal void Add_Child_Aggregation(Item_Aggregation_Related_Aggregations Child_Aggregation)
        {
            // If the list is currently null, create it
            if (children == null)
            {
                children = new List<Item_Aggregation_Related_Aggregations> {Child_Aggregation};
            }
            else
            {
                // If this does not exist, add it
                if (!children.Contains(Child_Aggregation))
                {
                    children.Add(Child_Aggregation);
                }
            }
        }

        /// <summary> Method adds another aggregation as a parent to this </summary>
        /// <param name = "Parent_Aggregation">New parent aggregation</param>
        internal void Add_Parent_Aggregation(Item_Aggregation_Related_Aggregations Parent_Aggregation)
        {
            // If the list is currently null, create it
            if (parents == null)
            {
                parents = new List<Item_Aggregation_Related_Aggregations> {Parent_Aggregation};
            }
            else
            {
                // If this does not exist, add it
                if (!parents.Contains(Parent_Aggregation))
                {
                    parents.Add(Parent_Aggregation);
                }
            }
        }

        /// <summary> Add the home page source file information, by language </summary>
        /// <param name = "Home_Page_File"> Home page text source file </param>
        /// <param name = "Language"> Language code </param>
        internal void Add_Home_Page_File(string Home_Page_File, Web_Language_Enum Language)
        {
            // If no language code, then always use this as the default
            if (Language == Web_Language_Enum.DEFAULT)
            {
                homeFilesByLanguage[SobekCM_Library_Settings.Default_UI_Language] = Home_Page_File;
            }
            else
            {
                // Save this under the normalized language code
                homeFilesByLanguage[Language] = Home_Page_File;
            }
        }

        /// <summary> Add the main banner image for this aggregation, by language </summary>
        /// <param name = "Banner_Image"> Main banner image source file for this aggregation </param>
        /// <param name = "Language"> Language code </param>
        internal void Add_Banner_Image(string Banner_Image, Web_Language_Enum Language)
        {
            // If no language code, then always use this as the default
            if (Language == Web_Language_Enum.DEFAULT)
            {
                bannerImagesByLanguage[SobekCM_Library_Settings.Default_UI_Language] = Banner_Image;
            }
            else
            {
                // Save this under the normalized language code
                bannerImagesByLanguage[Language] = Banner_Image;
            }
        }

        /// <summary>
        ///   Add the special front banner image that displays on the home page only for this aggregation, by language
        /// </summary>
        /// <param name = "Banner_Image"> special front banner image source file for this aggregation </param>
        /// <param name = "Language"> Language code </param>
        /// <returns> Build front banner image information object </returns>
		internal Item_Aggregation_Front_Banner Add_Front_Banner_Image(string Banner_Image, Web_Language_Enum Language)
        {
			Item_Aggregation_Front_Banner banner = new Item_Aggregation_Front_Banner(Banner_Image);

            // If no language code, then always use this as the default
            if (Language == Web_Language_Enum.DEFAULT)
            {
				frontBannerImageByLanguage[SobekCM_Library_Settings.Default_UI_Language] = banner;
            }
            else
            {
                // Save this under the normalized language code
				frontBannerImageByLanguage[Language] = banner;
            }
	        return banner;
        }

        #endregion

        #region Methods to return the appropriate banner HTML

        /// <summary> Clear the list of banners, by language </summary>
        public void Clear_Banners()
        {
            frontBannerImageByLanguage.Clear();
            bannerImagesByLanguage.Clear();
        }



        /// <summary> Get the standard banner dictionary, by language </summary>
        public Dictionary<Web_Language_Enum, string> Banner_Dictionary
        {
            get
            {
                return bannerImagesByLanguage;
            }
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
            if ((ThisWebSkin != null) && (ThisWebSkin.Override_Banner) && (Aggregation_Type.ToLower().IndexOf("institution") < 0))
            {
                return ThisWebSkin.Banner_HTML;
            }

            // Does this language exist in the banner image lookup dictionary?
            if (bannerImagesByLanguage.ContainsKey(Language))
            {
                return "design/" + ObjDirectory + bannerImagesByLanguage[Language];
            }

            // Default to the system language then
            if (bannerImagesByLanguage.ContainsKey(SobekCM_Library_Settings.Default_UI_Language))
            {
                return "design/" + ObjDirectory + bannerImagesByLanguage[SobekCM_Library_Settings.Default_UI_Language];
            }

            // Just return the first, assuming one exists
            return bannerImagesByLanguage.Count > 0 ? bannerImagesByLanguage.ElementAt(0).Value : String.Empty;
        }

        /// <summary> Get the front banner dictionary, by language </summary>
		public Dictionary<Web_Language_Enum, Item_Aggregation_Front_Banner> Front_Banner_Dictionary
        {
            get
            {
                return frontBannerImageByLanguage;
            }
        }

        /// <summary> Gets the special front banner image for this aggregation's home page, by language </summary>
        /// <param name = "Language"> Language code </param>
        /// <returns> Either the language-specific special front banner image, or else the default front banner image</returns>
        /// <remarks>
        ///   If NO special front banner images were included in the aggregation XML, then this could be NULL. <br /><br />
        ///   This is a special front banner image used for aggregations that show the highlighted
        ///   item and the search box in the main banner at the top on the front page
        /// </remarks>
		public Item_Aggregation_Front_Banner Front_Banner_Image(Web_Language_Enum Language)
        {
            // Does this language exist in the banner image lookup dictionary?
            if (frontBannerImageByLanguage.ContainsKey(Language))
            {
                return frontBannerImageByLanguage[Language];
            }

            // Default to the system language then
            if (frontBannerImageByLanguage.ContainsKey(SobekCM_Library_Settings.Default_UI_Language))
            {
                return frontBannerImageByLanguage[SobekCM_Library_Settings.Default_UI_Language];
            }

            // Just return the first, assuming one exists
            return frontBannerImageByLanguage.Count > 0 ? frontBannerImageByLanguage.ElementAt(0).Value : null;
        }

        #endregion

        #region Method to return appropriate home page source file, or the home page HTML

        /// <summary> Gets the raw home page source file </summary>
        public Dictionary<Web_Language_Enum, string> Home_Page_File_Dictionary
        {
            get
            {
                return homeFilesByLanguage;
            }
        }

		/// <summary> Removes a single home page from the collection of home pages </summary>
		/// <param name="Language"> Language of the home page to remove </param>
		public void Delete_Home_Page(Web_Language_Enum Language)
		{
			homeFilesByLanguage.Remove(Language);
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
            // Does this language exist in the home page file lookup dictionary?
            if (homeFilesByLanguage.ContainsKey(Language))
            {
                return homeFilesByLanguage[Language];
            }

            // Default to the system language then
            if (homeFilesByLanguage.ContainsKey(SobekCM_Library_Settings.Default_UI_Language))
            {
                return homeFilesByLanguage[SobekCM_Library_Settings.Default_UI_Language];
            }

            // Just return the first, assuming one exists
            return homeFilesByLanguage.Count > 0 ? homeFilesByLanguage.ElementAt(0).Value : String.Empty;
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
            string homeFileSource = SobekCM_Library_Settings.Base_Design_Location + ObjDirectory +
                                    Home_Page_File(Language);

            // If no home file source even found, return a message to that affect
            if (homeFileSource.Length == 0)
            {
                return "<div class=\"error_div\">NO HOME PAGE SOURCE FILE FOUND</div>";
            }

            // Do the rest in a try/catch
            try
            {
                // Does the file exist?
                if (!System.IO.File.Exists(homeFileSource))
                {
                    return "<div class=\"error_div\">HOME PAGE SOURCE FILE '" + homeFileSource +
                           "' DOES NOT EXIST.</div>";
                }

                // Get the text by language
                System.IO.StreamReader reader = new System.IO.StreamReader(homeFileSource);
                string tempHomeHtml = reader.ReadToEnd();
                reader.Close();

                // Ensure that any HTML header and end body tags are removed
                if (tempHomeHtml.IndexOf("<body>") > 0)
                    tempHomeHtml =tempHomeHtml.Substring(tempHomeHtml.IndexOf("<body>") + 6).Replace("</body>", "").Replace("</html>", "");

                // Does the home page have a place for te highlights?
                if (tempHomeHtml.IndexOf("<%HIGHLIGHT%>") >= 0)
                {
                    string highlightHtml = String.Empty;
                    if (Highlights.Count > 0)
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
	        childPages.Remove(Browse_Page);
        }

		/// <summary> Remove an existing browse or info object from this item aggregation </summary>
		/// <param name="Browse_Page_Code"> Child page information to remove </param>
		public void Remove_Child_Page(string Browse_Page_Code)
		{
			if (childPagesHash.ContainsKey(Browse_Page_Code.ToUpper()))
			{
				Item_Aggregation_Child_Page childPage = childPagesHash[Browse_Page_Code.ToUpper()];

				childPagesHash.Remove(Browse_Page_Code.ToUpper());

				childPages.Remove(childPage);
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

	    /// <summary> Method returns the table of results for the browse indicated </summary>
	    /// <param name = "ChildPageObject">Object with all the information about the browse</param>
	    /// <param name = "Page"> Page of results requested for the indicated browse </param>
	    /// <param name = "Sort"> Sort applied to the results before being returned </param>
	    /// <param name="Potentially_Include_Facets"> Flag indicates if facets could be included in this browse results </param>
	    /// <param name = "Need_Browse_Statistics"> Flag indicates if the browse statistics (facets and total counts) are required for this browse as well </param>
	    /// <param name = "Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
	    /// <param name="Results_Per_Page"> Number of results to retrieve per page</param>
	    /// <returns> Resutls for the browse or info in table form </returns>
	    public Results.Multiple_Paged_Results_Args Get_Browse_Results(Item_Aggregation_Child_Page ChildPageObject,
                                                                      int Page, int Sort, int Results_Per_Page, bool Potentially_Include_Facets, bool Need_Browse_Statistics,
                                                                      Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation.Get_Browse_Table", String.Empty);
            }

            // Get the list of facets first
            List<short> facetsList = Facets;
            if (!Potentially_Include_Facets)
                facetsList = null;

            // Pull data from the database if necessary
            if ((ChildPageObject.Code == "all") || (ChildPageObject.Code == "new"))
            {
                // Get this browse from the database
                if ((Aggregation_ID < 0) || (Code.ToUpper() == "ALL"))
                {
                    if (ChildPageObject.Code == "new")
                        return Database.SobekCM_Database.Get_All_Browse_Paged(true, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
                    return Database.SobekCM_Database.Get_All_Browse_Paged(false, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
                }
                
                if (ChildPageObject.Code == "new")
                {
                    return Database.SobekCM_Database.Get_Item_Aggregation_Browse_Paged(Code, true, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
                }
                return Database.SobekCM_Database.Get_Item_Aggregation_Browse_Paged(Code, false, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
            }

            // Default return NULL
            return null;
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
                System.IO.StreamWriter writer = new System.IO.StreamWriter(Directory + "\\" + filename, false);

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
                writer.WriteLine("<hi:hierarchyInfo xmlns:hi=\"http://digital.uflib.ufl.edu/metadata/hierarchyInfo/\" ");
                writer.WriteLine("				  xmlns:xlink=\"http://www.w3.org/1999/xlink\" ");
                writer.WriteLine("				  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
                writer.WriteLine("				  xsi:schemaLocation=\"http://digital.uflib.ufl.edu/metadata/hierarchyInfo/ ");
                writer.WriteLine("									 http://digital.uflib.ufl.edu/metadata/hierarchyInfo/hierarchyInfo.xsd\" >");
                writer.WriteLine();

                // Write the settings portion
                writer.WriteLine("<!-- Simple aggregation-wide settings, including a specified web skin, facets, etc.. -->");
                writer.WriteLine("<hi:settings>");
                writer.WriteLine("    <!-- Webskins here LIMIT the skins that this aggregation can appear under    -->");
                writer.WriteLine("    <!-- and if none appear here, then the aggregation can appear under any      -->");
                writer.WriteLine("    <!-- web skin.  Multiple web skins should appear with a comma between them.  -->");
                if (Web_Skins.Count > 0)
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
				if (CSS_File.Length > 0 )
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
                foreach (KeyValuePair<Web_Language_Enum, string> homePair in homeFilesByLanguage)
                {
                    writer.WriteLine("    <hi:body lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(homePair.Key) + "\">" +homePair.Value + "</hi:body>");
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
                foreach (KeyValuePair<Web_Language_Enum, string> homePair in bannerImagesByLanguage)
                {
                    writer.WriteLine("    <hi:source lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(homePair.Key) + "\">" +homePair.Value + "</hi:source>");
                }
				foreach (KeyValuePair<Web_Language_Enum, Item_Aggregation_Front_Banner> homePair in frontBannerImageByLanguage)
				{
					writer.Write("    <hi:source type=\"HIGHLIGHT\" lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(homePair.Key) + "\"");
					writer.Write(" height=\"" + homePair.Value.Height + "\" width=\"" + homePair.Value.Width + "\"");
					switch (homePair.Value.Banner_Type)
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

					writer.WriteLine(">" +homePair.Value.Image_File + "</hi:source>");
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
                if (Custom_Directives.Count > 0)
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
                if (Highlights.Count > 0)
                {
                    writer.WriteLine("  <!-- Highlighted items.  Inclusion of more than one will cause the daily -->");
                    writer.WriteLine("  <!-- displayed highlighted item to slowly change through the items.      -->");
                    writer.WriteLine("  <!-- Including TYPE=ROTATING will cause up to eight highlighted items    -->");
                    writer.WriteLine("  <!-- to rotate on the home page or in the special highlight banner.      -->");

                    // Is there a special front banner with highlighted item within it?
                    if (frontBannerImageByLanguage.Count > 0)
                    {
                        writer.Write("  <hi:highlights ");
                        writer.WriteLine(Rotating_Highlights ? "type=\"ROTATING\">" : "type=\"STANDARD\">");
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
		            foreach (Item_Aggregation_Child_Page browseObject in childPages)
		            {
			            browseObject.Write_In_Configuration_XML_File(writer, Default_BrowseBy);
		            }
		            writer.WriteLine();
	            }

	            // Close the main tag
                writer.WriteLine("</hi:hierarchyInfo>");

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

        #region Method to save this item aggregation to the database

	    /// <summary> Saves the information about this item aggregation to the database </summary>
	    /// <param name="Username"> Name of the user performing this save, for the item aggregation milestones</param>
	    /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
	    /// <returns>TRUE if successful, otherwise FALSE </returns>
	    public bool Save_To_Database( string Username, Custom_Tracer Tracer )
        {
            return Database.SobekCM_Database.Save_Item_Aggregation(Aggregation_ID, Code, Name, ShortName,
                Description, Thematic_Heading_ID, Aggregation_Type, Is_Active, Hidden, Display_Options, Map_Search, Map_Display, OAI_Flag,
                OAI_Metadata, Contact_Email,  String.Empty, External_Link, -1, Username, Tracer );
        }

        #endregion
    }
}