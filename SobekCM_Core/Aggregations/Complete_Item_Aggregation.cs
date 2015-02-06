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
    public class Complete_Item_Aggregation
    {
        #region Private variables

        private string defaultBrowseBy;
        private readonly Dictionary<string, Complete_Item_Aggregation_Child_Page> childPagesHash;
        private readonly string baseDesignLocation;
        private readonly Web_Language_Enum defaultUiLanguage;


        #endregion

        #region Constructor

        /// <summary> Constructor for a new instance of the Item_Aggregation_Complete class  </summary>
        /// <param name="Default_UI_Language"> Default user interface language for this interface </param>
        /// <param name="Base_Design_Location"> Base design location for this instance </param>
        public Complete_Item_Aggregation( Web_Language_Enum Default_UI_Language, string Base_Design_Location )
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
            childPagesHash = new Dictionary<string, Complete_Item_Aggregation_Child_Page>();
            Search_Fields = new List<Complete_Item_Aggregation_Metadata_Type>();
            Browseable_Fields = new List<Complete_Item_Aggregation_Metadata_Type>();
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

        /// <summary> Constructor for a new instance of the Item_Aggregation_Complete class </summary>
        /// <param name="Default_UI_Language"> Default user interface language for this interface </param>
        /// <param name="Base_Design_Location"> Base design location for this instance </param>
        /// <param name = "Code"> Unique code for this item aggregation object </param>
        /// <param name = "Type"> Type of aggregation object (i.e., collection, institution, exhibit, etc..)</param>
        /// <param name = "ID"> ID for this aggregation object from the database </param>
        /// <param name = "Display_Options"> Display options used to determine the views and searches for this item</param>
        /// <param name = "Last_Item_Added"> Date the last item was added ( or 1/1/2000 by default )</param>
        public Complete_Item_Aggregation(Web_Language_Enum Default_UI_Language, string Base_Design_Location, string Code, string Type, int ID, string Display_Options, DateTime Last_Item_Added)
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
            childPagesHash = new Dictionary<string, Complete_Item_Aggregation_Child_Page>();
            Search_Fields = new List<Complete_Item_Aggregation_Metadata_Type>();
            Browseable_Fields = new List<Complete_Item_Aggregation_Metadata_Type>();
            Facets = new List<short> {3, 5, 7, 10, 8};

            // Add the searches and views
            Views_And_Searches = new List<Item_Aggregation_Views_Searches_Enum>();
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
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.No_Home_Search);
						home_search_found = true;
						break;

					case 'A':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Advanced_Search);
						break;

                    case 'B':
                    case 'D':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Basic_Search);
                        home_search_found = true;
                        break;

					case 'C':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.DLOC_FullText_Search);
						home_search_found = true;
						break;

                    case 'F':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.FullText_Search);
                        home_search_found = true;
                        break;

					case 'G':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Map_Browse);
						break;

                    case 'g':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Map_Browse_Beta);
                        break;

					case 'I':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.All_New_Items);
						break;

					case 'M':
						Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Map_Search);
						break;

                    case 'Q':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Map_Search_Beta);
                        break;

                    case 'N':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Newspaper_Search);
                        home_search_found = true;
                        break;

                    case 'W':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Basic_Search_MimeType);
                        home_search_found = true;
                        break;

					case 'X':
						Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Advanced_Search_MimeType);
						break;

                    case 'Y':
                        Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Basic_Search_YearRange);
                        home_search_found = true;
                        break;

					case 'Z':
						Views_And_Searches.Add(Item_Aggregation_Views_Searches_Enum.Advanced_Search_YearRange);
						break;
						
                }
            }

            // If no home search found, add the no home search view
            if (!home_search_found)
            {
                Views_And_Searches.Insert(0, Item_Aggregation_Views_Searches_Enum.No_Home_Search);
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

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus should appear in the advanced search drop downs  </summary>
        [DataMember(Name = "searchFields")]
        [ProtoMember(5)]
        public List<Complete_Item_Aggregation_Metadata_Type> Search_Fields { get; private set; }

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus could appear in the metadata browse </summary>
        [DataMember(Name = "browseableFields")]
        [ProtoMember(6)]
        public List<Complete_Item_Aggregation_Metadata_Type> Browseable_Fields { get; private set; }

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

        /// <summary> Statistical information about this aggregation ( i.e., item, title, and page count ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "statistics")]
        [ProtoMember(12)]
        public Item_Aggregation_Statistics Statistics { get; set; }

        /// <summary> Gets the list of highlights associated with this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "highlights")]
        [ProtoMember(13)]
        public List<Complete_Item_Aggregation_Highlights> Highlights { get; set; }

        /// <summary> Value indicates if the highlights should be rotating through a number of 
        ///   highlights, or be fixed on a single highlight </summary>
        [DataMember(EmitDefaultValue = false, Name = "rotatingHighlights")]
        [ProtoMember(14)]
        public bool? Rotating_Highlights { get; set; }

        /// <summary> Key to the thematic heading under which this aggregation should appear on the main home page </summary>
        [DataMember(EmitDefaultValue = false, Name = "thematicHeading")]
        [ProtoMember(15)]
        public Thematic_Heading Thematic_Heading { get; set; }

        /// <summary> Email address to receive notifications when items load under this aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "loadEmail")]
        [ProtoMember(16)]
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
        [ProtoMember(17)]
        public string Name { get; set; }

        /// <summary> Short name of this item aggregation </summary>
        /// <remarks> This is an alternate name used for breadcrumbs, etc.. </remarks>
        [DataMember(Name = "shortName")]
        [ProtoMember(18)]
        public string ShortName { get; set; }

        /// <summary> Description of this item aggregation (in the default language ) </summary>
        /// <remarks> This field is displayed on the main home pages if this is part of a thematic collection </remarks>
        [DataMember(Name = "description")]
        [ProtoMember(19)]
        public string Description { get; set; }

        /// <summary> Flag indicating this item aggregation is active </summary>
        [DataMember(Name = "isActive")]
        [ProtoMember(20)]
        public bool Active { get; set; }

        /// <summary> Flag indicating this item aggregation is hidden from most displays </summary>
        /// <remarks> If this item aggregation is active, public users can still be directed to this item aggreagtion, but it will
        ///   not appear in the lists of subaggregations anywhere. </remarks>
        [DataMember(Name = "isHidden")]
        [ProtoMember(21)]
        public bool Hidden { get; set; }

        /// <summary> Display options string for this item aggregation </summary>
        /// <remarks> This defines which views and browses are available for this item aggregation </remarks>
        [DataMember(Name = "displayOptions")]
        [ProtoMember(22)]
        public string Display_Options { get; set; }

        /// <summary> Flag that tells what type of map searching to allow for this item aggregation </summary>
        [DataMember(Name = "mapSearch")]
        [ProtoMember(23)]
        public ushort Map_Search { get; set; }

        /// <summary> Flag that tells what type of map searching to allow for this item aggregation </summary>
        [IgnoreDataMember]
        public ushort Map_Search_Beta { get; set; }

        /// <summary> Flag that tells what type of map display to show for this item aggregation </summary>
        [DataMember(Name = "mapDisplay")]
        [ProtoMember(24)]
        public ushort Map_Display { get; set; }

        /// <summary> Flag that tells what type of map display to show for this item aggregation </summary>
        [IgnoreDataMember]
        public ushort Map_Display_Beta { get; set; }

        /// <summary> Flag indicates whether this item aggregation should be made available via OAI-PMH </summary>
        [DataMember(Name = "oaiEnabled")]
        [ProtoMember(25)]
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

        /// <summary> Read-only list of collection views and searches for this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "Views_And_Searches")]
        [ProtoMember(36)]
        public List<Item_Aggregation_Views_Searches_Enum> Views_And_Searches { get; private set; }

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
        public List<Complete_Item_Aggregation_Child_Page> Child_Pages { get; set; }

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

        /// <summary> Get the list of different language variants available </summary>
        [DataMember(EmitDefaultValue = false, Name = "languageVariants"), ProtoMember(44)]
        public List<Web_Language_Enum> Language_Variants { get; private set; }

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
                return childPagesHash.Values.Any(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By);
            }
        }

        /// <summary> Read-only list of all the info objects attached to this item aggregation </summary>
        /// <remarks> These are returned in alphabetical order of the SUBMODE CODE portion of each info </remarks>
        [IgnoreDataMember]
        public ReadOnlyCollection<Complete_Item_Aggregation_Child_Page> Info_Pages
        {
            get
            {
                SortedList<string, Complete_Item_Aggregation_Child_Page> otherInfos = new SortedList<string, Complete_Item_Aggregation_Child_Page>();
                foreach (Complete_Item_Aggregation_Child_Page thisInfo in childPagesHash.Values.Where(ThisInfo => ThisInfo.Browse_Type == Item_Aggregation_Child_Visibility_Enum.None))
                {
                    otherInfos[thisInfo.Code] = thisInfo;
                }
                return new ReadOnlyCollection<Complete_Item_Aggregation_Child_Page>(otherInfos.Values);
            }
        }


        /// <summary> Read-only list of searches types for this item aggregation </summary>
        [IgnoreDataMember]
        public ReadOnlyCollection<Search_Type_Enum> Search_Types
        {
            get
            {
                List<Search_Type_Enum> returnValue = new List<Search_Type_Enum>();
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Basic_Search))
                    returnValue.Add(Search_Type_Enum.Basic);
				if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Basic_Search_YearRange))
					returnValue.Add(Search_Type_Enum.Basic);
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Basic_Search_MimeType))
                    returnValue.Add(Search_Type_Enum.Basic);
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Newspaper_Search))
                    returnValue.Add(Search_Type_Enum.Newspaper);
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Map_Search))
                    returnValue.Add(Search_Type_Enum.Map);
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Map_Search_Beta))
                    returnValue.Add(Search_Type_Enum.Map_Beta);
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.FullText_Search))
                    returnValue.Add(Search_Type_Enum.Full_Text);
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.DLOC_FullText_Search))
                    returnValue.Add(Search_Type_Enum.dLOC_Full_Text);
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Advanced_Search))
                    returnValue.Add(Search_Type_Enum.Advanced);
				if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Advanced_Search_YearRange))
					returnValue.Add(Search_Type_Enum.Advanced);
                if (Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Advanced_Search_MimeType))
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
        public Complete_Item_Aggregation_Child_Page Child_Page_By_Code(string ChildCode)
		{
			return childPagesHash.ContainsKey(ChildCode.ToUpper()) ? childPagesHash[ChildCode.ToUpper()] : null;
		}

	    /// <summary> Add a child page to this item aggregatiion </summary>
		/// <param name="ChildPage"> New child page to add </param>
        public void Add_Child_Page(Complete_Item_Aggregation_Child_Page ChildPage)
	    {
            if (Child_Pages == null)
                Child_Pages = new List<Complete_Item_Aggregation_Child_Page>();

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
        public Complete_Item_Aggregation_Child_Page Add_Child_Page(Item_Aggregation_Child_Visibility_Enum Browse_Type, string Browse_Code, string StaticHtmlSource, string Text)
		{
			// Create the new Browse_Info object
            Complete_Item_Aggregation_Child_Page childPage = new Complete_Item_Aggregation_Child_Page(Browse_Type, Item_Aggregation_Child_Source_Data_Enum.Database_Table, Browse_Code, StaticHtmlSource, Text);

		    if (Child_Pages == null)
		        Child_Pages = new List<Complete_Item_Aggregation_Child_Page>();

			// Add this to the Hash table
			Child_Pages.Add(childPage);
			childPagesHash[Browse_Code.ToUpper()] = childPage;

			return childPage;
		}

        /// <summary> Read-only list of all the browse objects to appear on the home page attached to this item aggregation </summary>
        /// <param name = "Current_Language"> Current language used to sort the browses by the label </param>
        /// <remarks> These are returned in alphabetical order of the LABEL portion of each browse, according to the provided language </remarks>
        public ReadOnlyCollection<Complete_Item_Aggregation_Child_Page> Browse_Home_Pages(Web_Language_Enum Current_Language)
        {
            SortedList<string, Complete_Item_Aggregation_Child_Page> otherBrowses = new SortedList<string, Complete_Item_Aggregation_Child_Page>();
            foreach (Complete_Item_Aggregation_Child_Page thisBrowse in childPagesHash.Values.Where(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Main_Menu))
            {
                otherBrowses[thisBrowse.Get_Label(Current_Language)] = thisBrowse;
            }
            return new ReadOnlyCollection<Complete_Item_Aggregation_Child_Page>(otherBrowses.Values);
        }

        /// <summary> Read-only list of all the browse objects to appear under the BROWSE BY attached to this item aggregation </summary>
        /// <param name = "Current_Language"> Current language used to sort the browses by the label </param>
        /// <remarks> These are returned in alphabetical order of the CODE portion of each browse, according to the provided language </remarks>
        public ReadOnlyCollection<Complete_Item_Aggregation_Child_Page> Browse_By_Pages(Web_Language_Enum Current_Language)
        {
            SortedList<string, Complete_Item_Aggregation_Child_Page> otherBrowses = new SortedList<string, Complete_Item_Aggregation_Child_Page>();
            foreach (Complete_Item_Aggregation_Child_Page thisBrowse in childPagesHash.Values.Where(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By))
            {
                otherBrowses[thisBrowse.Code] = thisBrowse;
            }
            return new ReadOnlyCollection<Complete_Item_Aggregation_Child_Page>(otherBrowses.Values);
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

   

        #endregion

        #region Methods to support BROWSE and INFO objects

        /// <summary> Remove an existing browse or info object from this item aggregation </summary>
        /// <param name="Browse_Page"> Child page information to remove </param>
        public void Remove_Child_Page( Complete_Item_Aggregation_Child_Page Browse_Page )
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
				Complete_Item_Aggregation_Child_Page childPage = childPagesHash[Browse_Page_Code.ToUpper()];

				childPagesHash.Remove(Browse_Page_Code.ToUpper());

				Child_Pages.Remove(childPage);
			}
		}

        /// <summary> Gets the browse or info object from this hierarchy </summary>
        /// <param name = "SubMode">Submode for the browse being requested</param>
        /// <returns>All the information about how to retrieve the browse data</returns>
        public Complete_Item_Aggregation_Child_Page Get_Browse_Info_Object(string SubMode)
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
				if (!String.IsNullOrEmpty(Custom_Home_Page_Source_File))
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
                if (( Facets != null ) && ( Facets.Count > 0))
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
                            case Item_Aggregation_Front_Banner_Type_Enum.Full:
                                writer.Write(" side=\"FULL\"");
                                break;

                            case Item_Aggregation_Front_Banner_Type_Enum.Left:
                                writer.Write(" side=\"LEFT\"");
                                break;

                            case Item_Aggregation_Front_Banner_Type_Enum.Right:
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
                if (Result_Views != null)
                {
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
                }
                if (Default_Result_View != null)
                {
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
                    foreach (Complete_Item_Aggregation_Highlights thisHighlight in Highlights)
                    {
                        thisHighlight.Write_In_Configuration_XML_File(writer);
                    }
                    writer.WriteLine("  </hi:highlights>");
                    writer.WriteLine();
                }

                // Add all the child pages
	            if (( childPagesHash != null ) && ( childPagesHash.Count > 0))
	            {
		            foreach (Complete_Item_Aggregation_Child_Page browseObject in Child_Pages)
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
            catch (Exception ee)
            {
                return ee.Message.Length > 0;
                return false;
            }
        }

        #endregion



    }
}