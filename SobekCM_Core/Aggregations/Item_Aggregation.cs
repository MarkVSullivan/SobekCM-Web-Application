#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Skins;
using SobekCM.Core.WebContent;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary> Language-specific item aggregation data </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("aggregation")]
    public class Item_Aggregation
    {
        /// <summary> List of custom directives used within this aggregation to replicate a large chunk of HTML on 
        /// multiple static browse pages </summary>
        /// <remarks> This property is not serialized </remarks>
        [XmlIgnore]
        [IgnoreDataMember]
        [NonSerialized]
        public Dictionary<string, Item_Aggregation_Custom_Directive> Custom_Directives;

        #region Private variables

        #endregion

        #region Constructor

        /// <summary> Constructor for a new instance of the language-specific Item_Aggregation class  </summary> 
        public Item_Aggregation()
        {
            Search_Fields = new List<Item_Aggregation_Metadata_Type>();
            Browseable_Fields = new List<Item_Aggregation_Metadata_Type>();
            Facets = new List<short>();
            Views_And_Searches = new List<Item_Aggregation_Views_Searches_Enum>();
            Result_Views = new List<Result_Display_Type_Enum>();

            Custom_Home_Page = false;
        }

        /// <summary> Constructor for a new instance of the language-specific Item_Aggregation class  </summary>
        /// <param name="Language"> Language for this language-specific version of an item aggregation </param>
        /// <param name="Code"> Aggregation code for this item aggregation </param>
        /// <param name="ID"> Primary key for this item aggregation, from the database </param>
        public Item_Aggregation( Web_Language_Enum Language, int ID, string Code  )
        {
            this.Language = Language;
            this.ID = ID;
            this.Code = Code;
            Search_Fields = new List<Item_Aggregation_Metadata_Type>();
            Browseable_Fields = new List<Item_Aggregation_Metadata_Type>();
            Facets = new List<short>();
            Views_And_Searches = new List<Item_Aggregation_Views_Searches_Enum>();
            Result_Views = new List<Result_Display_Type_Enum>();

            Custom_Home_Page = false;
        }

        #endregion

        #region Public Properties

        /// <summary> Language this item aggregation represents  </summary>
        [DataMember(Name = "language")]
        [XmlAttribute("language")]
        [ProtoMember(33)]
        public Web_Language_Enum Language { get; set; }

        /// <summary> ID for this item aggregation object </summary>
        /// <remarks> The AggregationID for the ALL aggregation is set to -1 by the stored procedure </remarks>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary> Type of item aggregation object </summary>
        [DataMember(Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(2)]
        public string Type { get; set; }

        /// <summary> Code for this item aggregation object </summary>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(3)]
        public string Code { get; set; }

        /// <summary> Date the last item was added to this collection </summary>
        /// <remarks> If there is no record of this, the date of 1/1/2000 is returned </remarks>
        [DataMember(Name = "lastItemAdded")]
        [XmlElement("lastItemAdded")]
        [ProtoMember(4)]
        public DateTime Last_Item_Added { get; set; }

        /// <summary> Flag indicates if this aggregation can potentially include the ALL ITEMS and NEW ITEMS tabs </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public bool Can_Browse_Items { get { return (Display_Options.IndexOf("I") >= 0); } }

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus should appear in the advanced search drop downs  </summary>
        [DataMember(Name = "searchFields")]
        [XmlArray("searchFields")]
        [XmlArrayItem("metadataType", typeof(Item_Aggregation_Metadata_Type))]
        [ProtoMember(5)]
        public List<Item_Aggregation_Metadata_Type> Search_Fields { get; set; }

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus could appear in the metadata browse </summary>
        [DataMember(Name = "browseableFields")]
        [XmlArray("browseableFields")]
        [XmlArrayItem("metadataType", typeof(Item_Aggregation_Metadata_Type))]
        [ProtoMember(6)]
        public List<Item_Aggregation_Metadata_Type> Browseable_Fields { get; set; }

        /// <summary> Returns the list of all facets to display during searches and browses within this aggregation </summary>
        /// <remarks> This can hold up to eight facets, by primary key for the metadata type.  By default this holds 3,5,7,10, and 8. </remarks>
        [DataMember(Name = "facets")]
        [XmlArray("facets")]
        [XmlArrayItem("metadataTypeId")]
        [ProtoMember(7)]
        public List<short> Facets { get; set; }

        /// <summary> Gets the list of web skins this aggregation can appear under </summary>
        /// <remarks> If no web skins are indicated, this is not restricted to any set of web skins, and can appear under any skin </remarks>
        [DataMember(EmitDefaultValue = false,Name="webSkins")]
        [XmlArray("webSkins")]
        [XmlArrayItem("skinCode")]
        [ProtoMember(8)]
        public List<string> Web_Skins { get; set; }

        /// <summary> Default browse by code, if no code is provided in the request </summary>
        [DataMember(EmitDefaultValue = false, Name = "defaultBrowseBy")]
        [XmlElement("defaultBrowseBy")]
        [ProtoMember(9)]
        public string Default_BrowseBy { get; set; }

        /// <summary> Gets the default results view mode for this item aggregation </summary>
        [DataMember(Name = "defaultResultView")]
        [XmlElement("defaultResultView")]
        [ProtoMember(10)]
        public Result_Display_Type_Enum Default_Result_View { get; set; }

        /// <summary> Gets the list of all result views present in this item aggregation </summary>
        [DataMember(Name = "resultsViews")]
        [XmlArray("resultsViews")]
        [XmlArrayItem("resultView", typeof(Result_Display_Type_Enum))]
        [ProtoMember(11)]
        public List<Result_Display_Type_Enum> Result_Views { get; set; }

        /// <summary> Statistical information about this aggregation ( i.e., item, title, and page count ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "statistics")]
        [XmlElement("statistics")]
        [ProtoMember(12)]
        public Item_Aggregation_Statistics Statistics { get; set; }

        /// <summary> Gets the list of highlights associated with this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "highlights")]
        [XmlArray("highlights")]
        [XmlArrayItem("highlight", typeof(Item_Aggregation_Highlights))]
        [ProtoMember(13)]
        public List<Item_Aggregation_Highlights> Highlights { get; set; }

        /// <summary> Value indicates if the highlights should be rotating through a number of 
        ///   highlights, or be fixed on a single highlight </summary>
        [DataMember(EmitDefaultValue = false, Name = "rotatingHighlights")]
        [XmlIgnore]
        [ProtoMember(14)]
        public bool? Rotating_Highlights { get; set; }

        /// <summary> Value indicates if the highlights should be rotating through a number of 
        ///   highlights, or be fixed on a single highlight </summary>
        /// <remarks> This is for the XML serialization portions </remarks>
        [IgnoreDataMember]
        [XmlElement("rotatingHighlights")]
        public string Rotating_Highlights_AsString
        {
            get {
                return Rotating_Highlights.HasValue ? Rotating_Highlights.ToString() : null;
            }
            set
            {
                bool temp;
                if (Boolean.TryParse(value, out temp))
                    Rotating_Highlights = temp;
            }
        }

        /// <summary> Directory where all design information for this object is found </summary>
        /// <remarks> This always returns the string 'aggregationPermissions/[Code]/' with the code for this item aggregation </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public string ObjDirectory
        {
            get { return "aggregations/" + Code + "/"; }
        }

        /// <summary> Full name of this item aggregation </summary>
        [DataMember(Name = "name")]
        [XmlElement("name")]
        [ProtoMember(15)]
        public string Name { get; set; }

        /// <summary> Short name of this item aggregation </summary>
        /// <remarks> This is an alternate name used for breadcrumbs, etc.. </remarks>
        [DataMember(Name = "shortName")]
        [XmlElement("shortName")]
        [ProtoMember(16)]
        public string ShortName { get; set; }

        /// <summary> Description of this item aggregation (in the default language ) </summary>
        /// <remarks> This field is displayed on the main home pages if this is part of a thematic collection </remarks>
        [DataMember(Name = "description")]
        [XmlElement("description")]
        [ProtoMember(17)]
        public string Description { get; set; }

        /// <summary> Flag indicating this item aggregation is active </summary>
        [DataMember(Name = "isActive")]
        [XmlElement("isActive")]
        [ProtoMember(18)]
        public bool Active { get; set; }

        /// <summary> Flag indicating this item aggregation is hidden from most displays </summary>
        /// <remarks> If this item aggregation is active, public users can still be directed to this item aggreagtion, but it will
        ///   not appear in the lists of subaggregations anywhere. </remarks>
        [DataMember(Name = "isHidden")]
        [XmlElement("isHidden")]
        [ProtoMember(19)]
        public bool Hidden { get; set; }

        /// <summary> Display options string for this item aggregation </summary>
        /// <remarks> This defines which views and browses are available for this item aggregation </remarks>
        [DataMember(Name = "displayOptions")]
        [XmlElement("displayOptions")]
        [ProtoMember(20)]
        public string Display_Options { get; set; }

        /// <summary> Flag that tells what type of map searching to allow for this item aggregation </summary>
        [DataMember(Name = "mapSearch")]
        [XmlElement("mapSearch")]
        [ProtoMember(21)]
        public ushort Map_Search { get; set; }

        /// <summary> Flag that tells what type of map searching to allow for this item aggregation </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public ushort Map_Search_Beta { get; set; }

        /// <summary> Flag that tells what type of map display to show for this item aggregation </summary>
        [DataMember(Name = "mapDisplay")]
        [XmlElement("mapDisplay")]
        [ProtoMember(22)]
        public ushort Map_Display { get; set; }

        /// <summary> Flag that tells what type of map display to show for this item aggregation </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public ushort Map_Display_Beta { get; set; }

        /// <summary> Contact email for any correspondance regarding this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "contactEmail")]
        [XmlElement("contactEmail")]
        [ProtoMember(23)]
        public string Contact_Email { get; set; }

        /// <summary> Default interface to be used for this item aggregation </summary>
        /// <remarks> This is primarily used for institutional aggregations, but can be utilized anywhere </remarks>
        [DataMember(EmitDefaultValue = false, Name = "defaultSkin")]
        [XmlElement("defaultSkin")]
        [ProtoMember(24)]
        public string Default_Skin { get; set; }

		/// <summary> Aggregation-level CSS file, if one exists </summary>
        [DataMember(EmitDefaultValue = false, Name = "cssFile")]
        [XmlElement("cssFile")]
        [ProtoMember(25)]
		public string CSS_File { get; set; }

		/// <summary> Custom home page source file, if one exists </summary>
		/// <remarks> This overrides many of the other parts of the item aggregation if in affect </remarks>
        [DataMember(EmitDefaultValue = false, Name = "isCustomHome")]
        [XmlElement("isCustomHome")]
        [ProtoMember(26)]
		public bool Custom_Home_Page { get; set; }

		/// <summary> The common type of all child collections, or the default </summary>
        [DataMember(EmitDefaultValue = false, Name = "childTypes")]
        [XmlElement("childTypes")]
        [ProtoMember(27)]
		public string Child_Types { get; set; }

        /// <summary> Read-only list of collection views and searches for this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "viewsAndSearches")]
        [XmlArray("viewsAndSearches")]
        [XmlArrayItem("view")]
        [ProtoMember(28)]
        public List<Item_Aggregation_Views_Searches_Enum> Views_And_Searches { get; set; }

        /// <summary> Gets the read-only collection of children item aggregation objects </summary>
        /// <remarks> You should check the count of children first using the <see cref = "Children_Count" /> before using this property.
        ///   Even if there are no children, this property creates a readonly collection to pass back out. </remarks>
        [DataMember(EmitDefaultValue = false, Name = "children")]
        [XmlArray("children")]
        [XmlArrayItem("relatedAggr", typeof(Item_Aggregation_Related_Aggregations))]
        [ProtoMember(29)]
        public List<Item_Aggregation_Related_Aggregations> Children { get; set; }

        /// <summary> Gets the read-only collection of parent item aggregation objects </summary>
        /// <remarks> You should check the count of parents first using the <see cref = "Parent_Count" /> before using this property.
        ///   Even if there are no parents, this property creates a readonly collection to pass back out. </remarks>
        [DataMember(EmitDefaultValue = false, Name = "parents")]
        [XmlArray("parents")]
        [XmlArrayItem("relatedAggr", typeof(Item_Aggregation_Related_Aggregations))]
        [ProtoMember(30)]
        public List<Item_Aggregation_Related_Aggregations> Parents { get; set; }

        /// <summary> Collection of all child pages </summary>
        [DataMember(EmitDefaultValue = false, Name = "childPages")]
        [XmlArray("childPages")]
        [XmlArrayItem("childPage", typeof(Item_Aggregation_Child_Page))]
        [ProtoMember(31)]
        public List<Item_Aggregation_Child_Page> Child_Pages { get; set; }

        /// <summary> Any aggregation-specific contact form configuration, otherwise NULL </summary>
        [DataMember(EmitDefaultValue = false, Name = "contactForm")]
        [XmlElement("contactForm")]
        [ProtoMember(32)]
        public ContactForm_Configuration ContactForm { get; set; }

        /// <summary> Front banner information, used for the banner on the main home page of this aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "frontBanner")]
        [XmlElement("frontBanner")]
        [ProtoMember(34)]
        public Item_Aggregation_Front_Banner FrontBannerObj { get; set; }

        /// <summary> Filename for the banner used on most of the aggregation pages, and perhaps the front page as well </summary>
        [DataMember(EmitDefaultValue = false, Name = "bannerImg")]
        [XmlElement("bannerImg")]
        [ProtoMember(35)]
        public string BannerImage { get; set; }

        /// <summary> Information pulled from the home page source file, including the full home page text </summary>
        [DataMember(EmitDefaultValue = false, Name = "homeText")]
        [XmlElement("homeText")]
        [ProtoMember(36)]
        public HTML_Based_Content HomePageHtml { get; set; }

        /// <summary> Source file for the home page text </summary>
        /// <remarks> This property is not serialized </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public string HomePageSource { get; set; }

        /// <summary> Gets the number of browses and info pages attached to this item aggregation </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public int Browse_Info_Count
        {
            get {
                return Child_Pages != null ? Child_Pages.Count : 0;
            }
        }

        /// <summary> Flag indicates if this item aggregation has at least one BROWSE BY page to display </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public bool Has_Browse_By_Pages
        {
            get
            {
                return Child_Pages != null && Child_Pages.Any(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By);
            }
        }

        /// <summary> Read-only list of all the info objects attached to this item aggregation </summary>
        /// <remarks> These are returned in alphabetical order of the SUBMODE CODE portion of each info </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public ReadOnlyCollection<Item_Aggregation_Child_Page> Info_Pages
        {
            get
            {
                SortedList<string, Item_Aggregation_Child_Page> otherInfos = new SortedList<string, Item_Aggregation_Child_Page>();
                if (Child_Pages != null)
                {
                    foreach (Item_Aggregation_Child_Page thisInfo in Child_Pages.Where(ThisInfo => ThisInfo.Browse_Type == Item_Aggregation_Child_Visibility_Enum.None))
                    {
                        otherInfos[thisInfo.Code] = thisInfo;
                    }
                }
                return new ReadOnlyCollection<Item_Aggregation_Child_Page>(otherInfos.Values);
            }
        }



        /// <summary> Read-only list of searches types for this item aggregation </summary>
        [IgnoreDataMember]
        [XmlIgnore]
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
        [XmlIgnore]
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
        [XmlIgnore]
        public bool Has_New_Items { get; set; }

        /// <summary> Gets the number of child item aggregationPermissions present </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref = "Children" /> property.  Even if 
        ///   there are no children, the Children property creates a readonly collection to pass back out. </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public int Children_Count
        {
            get
            {
                if (Children == null) return 0;
                return Children.Count(ThisChild => (ThisChild.Active) && (!ThisChild.Hidden));
            }
        }



        /// <summary> Gets the number of parent item aggregationPermissions present </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref = "Parents" /> property.  Even if 
        ///   there are no parents, the Parent property creates a readonly collection to pass back out. </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public int Parent_Count
        {
            get
            {
	            return Parents == null ? 0 : Parents.Count;
            }
        }


        /// <summary> Returns the list of all parent codes to this aggregation, seperate by a semi-colon  </summary>
        [IgnoreDataMember]
        [XmlIgnore]
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
            if (Child_Pages == null)
                return null;

            return Child_Pages.FirstOrDefault(ChildPage => String.Compare(ChildCode, ChildPage.Code, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

	    /// <summary> Add a child page to this item aggregatiion </summary>
		/// <param name="ChildPage"> New child page to add </param>
		public void Add_Child_Page(Item_Aggregation_Child_Page ChildPage)
	    {
	        if (Child_Pages == null)
	            Child_Pages = new List<Item_Aggregation_Child_Page>();
	        else
	        {
	            Item_Aggregation_Child_Page existingPage = Child_Page_By_Code(ChildPage.Code);
	            if (existingPage != null)
	                Child_Pages.Remove(existingPage);
	        }

	        Child_Pages.Add(ChildPage);
	    }

		/// <summary> Add a new browse or info object to this hierarchical object </summary>
		/// <param name = "Browse_Type">Flag indicates if this is a BROWSE or INFO object</param>
		/// <param name = "Browse_Code">SubMode indicator for this object</param>
		/// <param name = "StaticHtmlSource">Any static HTML source to be used for display</param>
		/// <param name = "Text">Text to display for this browse</param>
		/// <returns>The built data object</returns>
        public Item_Aggregation_Child_Page Add_Child_Page(Item_Aggregation_Child_Visibility_Enum Browse_Type, string Browse_Code, string StaticHtmlSource, string Text)
		{
			// Create the new Browse_Info object
            Item_Aggregation_Child_Page childPage = new Item_Aggregation_Child_Page(Browse_Type, Item_Aggregation_Child_Source_Data_Enum.Database_Table, Browse_Code, StaticHtmlSource, Text);

		    Add_Child_Page(childPage);

			return childPage;
		}

        /// <summary> Read-only list of all the browse objects to appear on the home page attached to this item aggregation </summary>
        /// <remarks> These are returned in alphabetical order of the LABEL portion of each browse, according to the provided language </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public ReadOnlyCollection<Item_Aggregation_Child_Page> Browse_Home_Pages
        {
            get
            {
                SortedList<string, Item_Aggregation_Child_Page> otherBrowses = new SortedList<string, Item_Aggregation_Child_Page>();
                if (Child_Pages != null)
                {
                    foreach (Item_Aggregation_Child_Page thisBrowse in Child_Pages.Where(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Main_Menu))
                    {
                        otherBrowses[thisBrowse.Label] = thisBrowse;
                    }
                }
                return new ReadOnlyCollection<Item_Aggregation_Child_Page>(otherBrowses.Values);
            }
        }

        /// <summary> Read-only list of all the browse objects to appear under the BROWSE BY attached to this item aggregation </summary>
        /// <remarks> These are returned in alphabetical order of the CODE portion of each browse, according to the provided language </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public ReadOnlyCollection<Item_Aggregation_Child_Page> Browse_By_Pages
        {
            get
            {
                SortedList<string, Item_Aggregation_Child_Page> otherBrowses = new SortedList<string, Item_Aggregation_Child_Page>();
                if (Child_Pages != null)
                {
                    foreach (Item_Aggregation_Child_Page thisBrowse in Child_Pages.Where(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By))
                    {
                        otherBrowses[thisBrowse.Code] = thisBrowse;
                    }
                }
                return new ReadOnlyCollection<Item_Aggregation_Child_Page>(otherBrowses.Values);
            }
        }

        #endregion



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

        /// <summary> Get the banner image for this aggregation, possibly returning the web skin banner if it overrides
        /// the aggregation banner  </summary>
        /// <param name="ThisWebSkin"> Web skin object, which may override the banner from this aggregation </param>
        /// <returns> String for the banner to use for this aggregation </returns>
        public string Get_Banner_Image(Web_Skin_Object ThisWebSkin)
        {
            // Does the web skin exist and override the banner?  For non-institutional agggregations
            // use the web skin banner HTML instead of the aggregation's banner
            if ((ThisWebSkin != null) && (ThisWebSkin.Override_Banner.HasValue) && ( ThisWebSkin.Override_Banner.Value ) && (Type.ToLower().IndexOf("institution") < 0))
            {
                return !String.IsNullOrEmpty(ThisWebSkin.Banner_HTML) ? ThisWebSkin.Banner_HTML : String.Empty;
            }

            return BannerImage;
        }

        #region Methods to support BROWSE and INFO objects

        /// <summary> Checks to see if a particular browse code exists in the list of browses or infos for this item aggregation </summary>
        /// <param name = "Browse_Code"> Code for the browse or info to check for existence </param>
        /// <returns> TRUE if this browse exists, otherwise FALSE </returns>
        public bool Contains_Browse_Info(string Browse_Code)
        {
            return Child_Page_By_Code(Browse_Code) != null;
        }

        #endregion

    }
}
