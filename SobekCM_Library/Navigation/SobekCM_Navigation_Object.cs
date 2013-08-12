#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;

#endregion

namespace SobekCM.Library.Navigation
{
	/// <summary> This object stores the current and next mode information for a single HTTP request. <br /> <br /> </summary>
	/// <remarks> This implements the <see cref="iSobekCM_Navigation_Object"/> interface.<br /> <br />
	/// Object written by Mark V Sullivan for the University of Florida. </remarks>
	public class SobekCM_Navigation_Object : iSobekCM_Navigation_Object
	{
		#region Private members of this object

		private string aggregation;
		private string aggregationAlias;
		private string baseSkin;
		private string baseUrl;
		private string bibid;
		private string browser;
		private string coordinates;
		private string currSkin;
		private string defaultAggregation;
		private string defaultSkin;
		private string errorMessage;
		private string infoBrowseMode;
		private string mysobekSubmode;
		private string pageByFilename;
		private string portalAbbreviation;
		private string portalName;
		private string returnUrl;
		private string searchFields;
		private string searchString;
		private string subaggregation;
		private string textSearch;
		private string vid;
		private string viewerCode;

		#endregion

		#region Constructors

		/// <summary> Constructor for a new instance of the SobekCM_Navigation_Object which stores 
		/// all of the information about an individual request. </summary>
		public SobekCM_Navigation_Object()
		{
			// Do general item construction
			Constructor_Helper();
		}

		/// <summary> Constructor for a new instance of the SobekCM_Navigation_Object which stores 
		/// all of the information about an individual request. </summary>
		/// <param name="QueryString">Query string information for analysis</param>
		/// <param name="Base_URL"> Requested base URL (without query string, etc..)</param>
		/// <param name="User_Languages"> Languages preferred by user, per their browser settings </param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections </param>
		/// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations</param>
		/// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library</param>
		/// <param name="URL_Portals"> List of all web portals into this system </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public SobekCM_Navigation_Object(NameValueCollection QueryString, string Base_URL, string[] User_Languages, Aggregation_Code_Manager Code_Manager, Dictionary<string, string> Aggregation_Aliases, ref Item_Lookup_Object All_Items_Lookup, Portal_List URL_Portals, Custom_Tracer Tracer )
		{
			// Do general item construction
			Constructor_Helper();

			// If there is a mode value or an aggregation value, use the legacy query string analyzer
			if ((QueryString["m"] != null) || (QueryString["a"] != null) || (QueryString["c"] != null) || (QueryString["s"] != null) || (QueryString["g"] != null) || (QueryString["h"] != null) || (QueryString["i"] != null))
			{
				// Use the legacy query string analyzer
				SobekCM_QueryString_Analyzer_Legacy analyzerLegacy = new SobekCM_QueryString_Analyzer_Legacy();
				analyzerLegacy.Parse_Query(QueryString, this, Base_URL, User_Languages, Code_Manager, Aggregation_Aliases, ref All_Items_Lookup, URL_Portals, Tracer);
			}
			else
			{
				// Analyze the query string with the default analyzer
				SobekCM_QueryString_Analyzer analyzer = new SobekCM_QueryString_Analyzer();
				analyzer.Parse_Query(QueryString, this, Base_URL, User_Languages, Code_Manager, Aggregation_Aliases, ref All_Items_Lookup, URL_Portals, Tracer);
			}
		}

		/// <summary> Constructor for a new instance of the SobekCM_Navigation_Object which stores 
		/// all of the information about an individual request. </summary>
		/// <param name="QueryString">Query string information for analysis</param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections </param>
		/// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations</param>
		/// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library</param>
		/// <param name="URL_Portals"> List of all web portals into this system </param>
		public SobekCM_Navigation_Object(NameValueCollection QueryString, Aggregation_Code_Manager Code_Manager, Dictionary<string, string> Aggregation_Aliases, ref Item_Lookup_Object All_Items_Lookup, Portal_List URL_Portals)
		{
			// Do general item construction
			Constructor_Helper();

			// Analyze the query string with the default analyzer
			SobekCM_QueryString_Analyzer analyzer = new SobekCM_QueryString_Analyzer();
			analyzer.Parse_Query(QueryString, this, String.Empty, null, Code_Manager, Aggregation_Aliases, ref All_Items_Lookup, URL_Portals, null);
		}

		/// <summary> Constructor for a new instance of the SobekCM_Navigation_Object which stores 
		/// all of the information about an individual request. </summary>
		/// <param name="QueryString">Query string information for analysis</param>
		/// <param name="Base_URL"> Requested base URL (without query string, etc..)</param>
		/// <param name="User_Languages"> Languages preferred by user, per their browser settings </param>
		/// <param name="Analyzer">Object to user for the URL analysis</param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections </param>
		/// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations</param>
		/// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library</param>
		/// <param name="URL_Portals"> List of all web portals into this system </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public SobekCM_Navigation_Object(NameValueCollection QueryString, string Base_URL, string[] User_Languages, iSobekCM_QueryString_Analyzer Analyzer, Aggregation_Code_Manager Code_Manager, Dictionary<string, string> Aggregation_Aliases, ref Item_Lookup_Object All_Items_Lookup, Portal_List URL_Portals, Custom_Tracer Tracer ) 
		{
			// Do general item construction
			Constructor_Helper();

			// Analyze the query string with the provided analyzer
			Analyzer.Parse_Query(QueryString, this, Base_URL, User_Languages, Code_Manager, Aggregation_Aliases, ref All_Items_Lookup, URL_Portals, Tracer);		
		}

		private void Constructor_Helper()
		{
			// Declare some defaults
			Mode = Display_Mode_Enum.Error;
			Search_Type = Search_Type_Enum.Basic;
			Result_Display_Type = Result_Display_Type_Enum.Default;
			Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
			Internal_Type = Internal_Type_Enum.Aggregations_List;
			My_Sobek_Type = My_Sobek_Type_Enum.Home;
			Language = Web_Language_Enum.English;
			Default_Language = Web_Language_Enum.English;
			Writer_Type = Writer_Type_Enum.HTML;
			TOC_Display = TOC_Display_Type_Enum.Undetermined;
			Trace_Flag = Trace_Flag_Type_Enum.Unspecified;
			Search_Precision = Search_Precision_Type_Enum.Contains;

			currSkin = "sobek";
			defaultSkin = "sobek";

			Page = 1;
			SubPage = 0;
			Viewport_Size = 1;
			Viewport_Zoom = 1;
			Viewport_Point_X = 0;
			Viewport_Point_Y = 0;
			Viewport_Rotation = 0;
			FolderID = 0;
			Sort = 0;
			ItemID_DEPRECATED = -1;
		    Thumbnails_Per_Page = -100;
		    Size_Of_Thumbnails = -1;

			Invalid_Item = false;
			Skin_in_URL = false;
			isPostBack = false;
			Show_Selection_Panel = false;
			Is_Robot = false;
			Internal_User = false;
			Logon_Required = false;
		    Request_Completed = false;
		}

		#endregion

		#region Code to set the robot flag from request variables

		/// <summary> Algorithm tests the user agent and IP address against known robots 
		/// to determine if this request is from a search engine indexer or web site crawler bot.  
		/// This returns the value and also sets an internal robot flag. </summary>
		/// <param name="UserAgent">User Agent string from the HTTP request</param>
		/// <param name="IP">IP address from the HTTP request</param>
		/// <returns>TRUE if the request appears to be a robot, otherwise FALSE</returns>
		public bool Set_Robot_Flag(string UserAgent, string IP)
        {
            if (UserAgent != null)
            {
                string useragent_upper = UserAgent.ToUpper();

                if (useragent_upper.IndexOf("BOT") >= 0)
                {
                    if ((useragent_upper.IndexOf("MSNBOT") >= 0) || (useragent_upper.IndexOf("GIGABOT") >= 0) || (useragent_upper.IndexOf("GOOGLEBOT") >= 0) || (useragent_upper.IndexOf("AISEARCHBOT") >= 0) || (useragent_upper.IndexOf("CCBOT") >= 0) || (useragent_upper.IndexOf("PLONEBOT") >= 0) || (useragent_upper.IndexOf("CAZOODLEBOT") >= 0) || (useragent_upper.IndexOf("DISCOBOT") >= 0) || (useragent_upper.IndexOf("BINGBOT") >= 0) || (useragent_upper.IndexOf("YANDEXBOT") >= 0) || (useragent_upper.IndexOf("ATRAXBOT") >= 0) || (useragent_upper.IndexOf("MJ12BOT") >= 0) || (useragent_upper.IndexOf("SITEBOT") >= 0) || (useragent_upper.IndexOf("LINGUEE+BOT") >= 0) || (useragent_upper.IndexOf("MLBOT") >= 0) || (useragent_upper.IndexOf("NEXTGENSEARCHBOT") >= 0) || (useragent_upper.IndexOf("BENDERTHEWEBROBOT") >= 0) || (useragent_upper.IndexOf("EZOOMS.BOT") >= 0) || (useragent_upper.IndexOf("LSSBOT") == 0) || (useragent_upper.IndexOf("DISCOVERYBOT") >= 0))
                    {
                        Is_Robot = true;
                        return true;
                    }
                }

                if ((useragent_upper.IndexOf("CRAWLER") >= 0) || (useragent_upper.IndexOf("SLURP") >= 0) || (useragent_upper.IndexOf("WEBVAC") >= 0) || (useragent_upper.IndexOf("ABOUT.ASK.COM") >= 0) || (useragent_upper.IndexOf("SCOUTJET") >= 0) || (useragent_upper.IndexOf("SITESUCKER") >= 0) || (useragent_upper.IndexOf("SEARCHME.COM") >= 0) || (useragent_upper.IndexOf("PICSEARCH.COM") >= 0) || (useragent_upper.IndexOf("XENU+LINK+SLEUTH") >= 0) || (useragent_upper.IndexOf("YANDEX") >= 0) || (useragent_upper.IndexOf("JAVA/") == 0) || (useragent_upper.IndexOf("SOGOU+WEB+SPIDER") >= 0) || (useragent_upper.IndexOf("CAMONTSPIDER") >= 0))
                {
                    Is_Robot = true;
                    return true;
                }

                if ((useragent_upper.IndexOf("BAIDUSPIDER") >= 0) || (useragent_upper.IndexOf("ICOPYRIGHT+CONDUCTOR") >= 0) || (useragent_upper.IndexOf("HTTP://AHREFS.COM/ROBOT") >= 0) || (useragent_upper.IndexOf("BENDERTHEROBOT.TUMBLR.COM") >= 0) || (useragent_upper.IndexOf("SHOULU.JIKE.COM/SPIDER") >= 0))
                {
                    Is_Robot = true;
                    return true;
                }

                if ((useragent_upper.IndexOf("WWW.PROFOUND.NET") >= 0) || (useragent_upper.IndexOf("URLAPPENDBOT") >= 0) || (useragent_upper.IndexOf("SEARCHMETRICBOT") >= 0) || (useragent_upper.IndexOf("HAVIJ") >= 0) || (useragent_upper.IndexOf("SYNAPSE") >= 0) || (useragent_upper.IndexOf("BEWSLEBOT") >= 0) || (useragent_upper.IndexOf("SOSOSPIDER") >= 0) || (useragent_upper.IndexOf("YYSPIDER") >= 0) || (useragent_upper.IndexOf("WBSEARCHBOT") >= 0))
                {
                    Is_Robot = true;
                    return true;
                }
            }

            // First IP address is for test purposes only
            if ((IP == "128.227.223.160") || (IP == "216.118.117.45") || (IP.IndexOf("65.55.230.") == 0) || (IP == "92.82.225.56") || (IP.IndexOf("220.181.51.") == 0) || (IP == "193.105.210.170") || (IP == "192.162.19.21"))
            {
                Is_Robot = true;
                return true;
            }

            return false;
        }

		#endregion

		#region Public Properties

		/// <summary> Gets the PURL associated with this portal which should be used for building permanent links for items </summary>
		public string Portal_PURL { get; set; }

		/// <summary> Filename indicated in the URL to allow direct link by page file name, rather than sequence </summary>
		public string Page_By_FileName
		{
			get { return pageByFilename ?? String.Empty; }
			set {   pageByFilename = value;    }
		}

		/// <summary> (DEPRECATED) ItemID which formerly was used for indicating items in the URL </summary>
		public int ItemID_DEPRECATED { get; set; }

		/// <summary> Precision to be used while performing a metadata search in the database </summary>
		public Search_Precision_Type_Enum Search_Precision { get; set; }

		/// <summary> Returns the name of this instance ( i.e., 'UFDC', 'dLOC', etc... ) </summary>
		public string SobekCM_Instance_Abbreviation
		{
			get 
			{
				return portalAbbreviation ?? "SOBEK";
			}
			set
			{
				portalAbbreviation = value;
			}
		}

		/// <summary> Returns the name of this instance ( i.e., 'UFDC', 'dLOC', etc... ) </summary>
		public string SobekCM_Instance_Name
		{
			get
			{
				return portalName ?? "Default SobekCM Library";
			}
			set
			{
				portalName = value;
			}
		}

		/// <summary> Flag indicates that logon is required to access the requested mode </summary>
		public bool Logon_Required { get; set; }

		/// <summary> Flag indicating this is an internal user </summary>
		public bool Internal_User { get; set; }

		/// <summary> Flag indicating if the current request is from a search engine 
		/// indexer or web site crawler bot.</summary>
		/// <remarks>This value is set in by calling the <see cref="Set_Robot_Flag"/> procedure. </remarks>
		public bool Is_Robot { get; set; }

		/// <summary> Return url value from the url string </summary>
		/// <remarks>This is primarily used by the mySobek feature, to return a user to their previously
		/// requested site, once they log on.</remarks>
		public string Return_URL
		{
			get { return returnUrl ?? String.Empty; }
			set { returnUrl = value; }
		}

		/// <summary> mySobek type of display for the current request.</summary>
		public My_Sobek_Type_Enum My_Sobek_Type { get; set; }

        /// <summary> Admin type of display for the current request.</summary>
        public Admin_Type_Enum Admin_Type { get; set; }

		/// <summary> mySobek submode for the current request.</summary>
		/// <remarks>The value returned is always lower case, and is also used for the admin pages</remarks>
		public string My_Sobek_SubMode
		{
			get { return mysobekSubmode ?? String.Empty; }
			set { mysobekSubmode = value; }
		}

		/// <summary> Base URL requested by the user </summary>
		/// <remarks> This is the URL post-rewriting from any URL path rewrite routine (such as SobekCM_URL_Rewriter) </remarks>
		public string Base_URL
		{
			get { return baseUrl ?? String.Empty; }
			set { baseUrl = value; }
		}

		/// <summary> Flag indicating this is a post back </summary>
		public bool isPostBack { get; set; }

		/// <summary> Default interface (based on original URL) </summary>
		/// <remarks>The value returned is always lower case</remarks>
		public string Default_Skin
		{
			get { return defaultSkin ?? String.Empty; }
			set { defaultSkin = value; }
		}

		/// <summary> Default aggregation (based on original URL) </summary>
		/// <remarks>The value returned is always lower case</remarks>
		public string Default_Aggregation
		{
			get { return defaultAggregation ?? String.Empty; }
			set { defaultAggregation = value.ToLower(); }
		}

		/// <summary> Flag which indicates the interface is indicated in the URL and query string </summary>
		public bool Skin_in_URL { get; set; }

		/// <summary> Simple error message generated during execution </summary>
		public string Error_Message
		{
			get { return errorMessage ?? String.Empty; }
			set { errorMessage = value; }
		}

		/// <summary> Exception generated during execution </summary>
		public Exception Caught_Exception { get; set; }

		/// <summary> TOC Display flag, which indicates whether to display the table of contents in the item viewer </summary>
		public TOC_Display_Type_Enum TOC_Display { get; set; }

		/// <summary> Flag that is set to indicate item requested is invalid </summary>
		public bool Invalid_Item { get; set; }

		/// <summary> String to use for a single-item text search </summary>
		public string Text_Search
		{
			get { return textSearch ?? String.Empty;		}
			set	{	textSearch = value;	}
		}
		
		/// <summary> Viewer code which indicates which viewer to use when displaying 
		/// a single item in the item writer.  </summary>
		/// <remarks>The value returned is always lower case</remarks>
		public string ViewerCode
		{
			get { return viewerCode ?? String.Empty;		}
			set	{	viewerCode = value;	}
		}

		/// <summary> Mode determined by parsing the query string </summary>
		public Display_Mode_Enum Mode { get; set; }

		/// <summary> Submode for searching </summary>
		public Search_Type_Enum Search_Type { get; set; }

		/// <summary> Submode for the internal pages </summary>
		public Internal_Type_Enum Internal_Type { get; set; }

		/// <summary> Submode for the statistics pages </summary>
		public Statistics_Type_Enum Statistics_Type { get; set; }

		/// <summary> Submode for the results display </summary>
		public Result_Display_Type_Enum Result_Display_Type { get; set; }

		/// <summary>Submode for the main library home page </summary>
		public Home_Type_Enum Home_Type { get; set; }

		/// <summary> Language for the interface </summary>
        public Web_Language_Enum Language { get; set; }

		/// <summary> Default language for this user, from their browser settings </summary>
        public Web_Language_Enum Default_Language { get; set; }

		/// <summary> Language code for the current skin language  </summary>
		public string Language_Code
		{
			get { return Web_Language_Enum_Converter.Enum_To_Code(Language); }
		}

		/// <summary> Browse or info mode to display </summary>
		/// <remarks>The value returned is always lower case</remarks>
		public string Info_Browse_Mode
		{
			get { return infoBrowseMode ?? String.Empty; }
			set	{	infoBrowseMode = value;	}
		}

		/// <summary> Current aggregation code </summary>
		/// <remarks>The value returned is always lower case</remarks>
		public string Aggregation
		{
			get { return aggregation ?? String.Empty;		}
			set {   aggregation = value;    }
		}

		/// <summary> Current aggregation alias code, if there is one </summary>
		/// <remarks>The value returned is always lower case</remarks>
		public string Aggregation_Alias
		{
			get { return aggregationAlias ?? String.Empty;		}
			set {   aggregationAlias = value;    }
		}

		/// <summary> Information about which sub aggregations to include or exclude
		/// during a collection group search</summary>
		public string SubAggregation
		{
			get { return subaggregation ?? String.Empty;		}
			set { subaggregation = value; }
		}

		/// <summary> Bib id to display </summary>
		/// <remarks>The value returned is always upper case </remarks>
		public string BibID
		{
			get { return bibid ?? String.Empty;		}
			set	{	bibid = value;		}
		}

		/// <summary> Volume id to display </summary>
		public string VID
		{
			get { return vid ?? String.Empty;			}
			set	{	vid = value;		}
		}

		/// <summary> Primary key for the folder to display </summary>
		public int FolderID { get; set; }

		/// <summary> Page number to be displayed (either for an item or for results )</summary>
		public ushort Page { get; set; }

		/// <summary> Sub page number to be displayed (either for an item or for results )</summary>
		public ushort SubPage { get; set; }

		/// <summary> Trace flag which indicates whether to display the trace route </summary>
		public Trace_Flag_Type_Enum Trace_Flag { get; set; }

		/// <summary> Simplified flag for displaying the trace route </summary>
		public bool Trace_Flag_Simple
		{
			get {
				return (Trace_Flag == Trace_Flag_Type_Enum.Explicit) || (Trace_Flag == Trace_Flag_Type_Enum.Implied);
			}
		}

		/// <summary> Coordinate search string for a geographic search </summary>
		public string Coordinates
		{
			get { return coordinates ?? String.Empty; }
			set { coordinates = value; }
		}

		/// <summary> Search string </summary>
		public string Search_String
		{
			get { return searchString ?? String.Empty; }
			set { searchString = value; }
		}

		/// <summary> Search fields </summary>
		public string Search_Fields
		{
			get { return searchFields ?? String.Empty;	}
			set	{	searchFields = value;	}
		}

		/// <summary> Sort type employed for displaying result sets </summary>
		public short Sort { get; set; }

		/// <summary> Current skin for display purposes </summary>
		/// <remarks>The value returned is always lower case</remarks>
		public string Skin		
		{
			get { return currSkin ?? String.Empty; 	}
			set	{	currSkin = value;	}
		}

		/// <summary> Base infterface interface for display purposes </summary>
		/// <remarks> The value returned is always lower case</remarks>
		public string Base_Skin
		{
			get
			{
				if (!string.IsNullOrEmpty(baseSkin))
					return baseSkin;
				return currSkin ?? String.Empty;
			}
			set { baseSkin = value; }
		}

		/// <summary>Viewport size for the zoomable image display </summary>
		public ushort Viewport_Size { get; set; }

		/// <summary>Viewport zoom for the zoomable image display </summary>
		public ushort Viewport_Zoom { get; set; }

		/// <summary> Viewport horizontal location for the zoomable image display </summary>
		public int Viewport_Point_X { get; set; }

		/// <summary> Viewport vertical location for the zoomable image display </summary>
		public int Viewport_Point_Y { get; set; }

		/// <summary> Viewport rotation for the zoomable image display </summary>
		public ushort Viewport_Rotation { get; set; }

        /// <summary> Thumbnails per page to appear in the related images item viewer  </summary>
        public short Thumbnails_Per_Page { get; set;  }

        /// <summary> Size of Thumbnails to appear in the related items viewer </summary>
        public short Size_Of_Thumbnails { get; set; }

	    /// <summary> Fragment utilized when only a portion of a page needs to be rendered </summary>
        public string Fragment { get; set; }

	    /// <summary> Writer type to be employed for rendering </summary>
		public Writer_Type_Enum Writer_Type { get; set; }

		/// <summary> Browser type </summary>
		public string Browser_Type
		{
			get { return browser ?? String.Empty; }
			set { browser = value; }
		}

		/// <summary> Flag which determines if the selection panel is shown
		/// for an aggregation search </summary>
		public bool Show_Selection_Panel { get; set; }

        /// <summary> Flag indicates if the request was completed, so no further
        /// operations should occur </summary>
        public bool Request_Completed { get; set; }

		#endregion

		#region iSobekCM_Navigation_Object Members

		/// <summary> Returns the URL to redirect the user's browser, based on the current
		/// mode and specifics for this mode. </summary>
		/// <returns> String to be attached to the end of the main application name to redirect
		/// the current user's browser.  </returns>
		public string Redirect_URL()
		{
			return Redirect_URL( ViewerCode, true );
		}

		/// <summary> Returns the URL to redirect the user's browser, based on the current
		/// mode and specifics for this mode. </summary>
		/// <param name="Item_View_Code">Item view code to display</param>
		/// <param name="Include_URL_Opts"> Flag indicates whether to include URL opts or not </param>
		/// <returns> String to be attached to the end of the main application name to redirect
		/// the current user's browser.  </returns>
		public string Redirect_URL(string Item_View_Code, bool Include_URL_Opts )
		{
			string this_base_url = baseUrl;

			// Determine the aggregation code to use
			string adjusted_aggregation = aggregation;
			if (!String.IsNullOrEmpty(aggregationAlias))
				adjusted_aggregation = aggregationAlias;


			// Add the writer type if it is not HTML 
			switch (Writer_Type)
			{
				case Writer_Type_Enum.DataSet:
					this_base_url = this_base_url + "dataset/";
					break;

				case Writer_Type_Enum.XML:
					this_base_url = this_base_url + "xml/";
					break;

				case Writer_Type_Enum.HTML_LoggedIn:
					if (Mode != Display_Mode_Enum.My_Sobek)
					{
						this_base_url = this_base_url + "l/";
					}
					break;

				case Writer_Type_Enum.Text:
					this_base_url = this_base_url + "textonly/";
					break;

				case Writer_Type_Enum.JSON:
					this_base_url = this_base_url + "json/";
					break;
			}

			string url_options = URL_Options();
			string urlOptions1 = String.Empty;
			string urlOptions2 = String.Empty;
			if ((url_options.Length > 0) && ( Include_URL_Opts ))
			{
				urlOptions1 = "?" + url_options;
				urlOptions2 = "&" + url_options;
			}

			switch (Mode)
			{
				case Display_Mode_Enum.Error:
					return SobekCM_Library_Settings.System_Error_URL;

				case Display_Mode_Enum.Internal:
					switch (Internal_Type)
					{
						case Internal_Type_Enum.Aggregations_List:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "internal/aggregations/list/" + infoBrowseMode + urlOptions1;
							return this_base_url + "internal/aggregations/list" + urlOptions1;

						case Internal_Type_Enum.Cache:
							return this_base_url + "internal/cache" + urlOptions1;

						case Internal_Type_Enum.Aggregations:
							if ( !String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "internal/aggregations/" + infoBrowseMode.Replace(" ", "_") + urlOptions1;
							return this_base_url + "internal/aggregations" + urlOptions1;

						case Internal_Type_Enum.New_Items:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "internal/new/" + infoBrowseMode + urlOptions1;
							return this_base_url + "internal/new" + urlOptions1;

						case Internal_Type_Enum.Build_Failures:
							return this_base_url + "internal/failures" + urlOptions1;

						case Internal_Type_Enum.Wordmarks:
							return this_base_url + "internal/wordmarks" + urlOptions1;
					}
					break;

				case Display_Mode_Enum.Contact:
					if (!String.IsNullOrEmpty(aggregation))
					{
						if (!String.IsNullOrEmpty(errorMessage))
							return this_base_url + "contact/" + aggregation + "?em=" + HttpUtility.HtmlEncode(errorMessage) + urlOptions2;
						return this_base_url + "contact/" + aggregation + urlOptions1;
					}
					if (!String.IsNullOrEmpty(errorMessage))
						return this_base_url + "contact?em=" + HttpUtility.HtmlEncode(errorMessage) + urlOptions2;
					return this_base_url + "contact" + urlOptions1;

				case Display_Mode_Enum.Contact_Sent:
					return this_base_url + "contact/sent" + urlOptions1;

				case Display_Mode_Enum.Public_Folder:
					if (FolderID > 0)
					{
						StringBuilder folderBuilder = new StringBuilder(this_base_url + "folder/" + FolderID );
						switch (Result_Display_Type)
						{
							case Result_Display_Type_Enum.Brief:
								folderBuilder.Append("/brief");
								break;
							case Result_Display_Type_Enum.Export:
								folderBuilder.Append("/export");
								break;
							case Result_Display_Type_Enum.Full_Citation:
								folderBuilder.Append("/citation");
								break;
							case Result_Display_Type_Enum.Full_Image:
								folderBuilder.Append("/image");
								break;
							case Result_Display_Type_Enum.Map:
								folderBuilder.Append("/map");
								break;
							case Result_Display_Type_Enum.Table:
								folderBuilder.Append("/table");
								break;
							case Result_Display_Type_Enum.Thumbnails:
								folderBuilder.Append("/thumbs");
								break;
							default:
								folderBuilder.Append("/brief");
								break;
						}
						if (Page > 1)
						{
							folderBuilder.Append("/" + Page.ToString() );
						}
						return folderBuilder + urlOptions1;
					}
					return this_base_url + "folder" + urlOptions1;

				case Display_Mode_Enum.Simple_HTML_CMS:
					return this_base_url + infoBrowseMode + urlOptions1;

				case Display_Mode_Enum.My_Sobek:
					switch (My_Sobek_Type)
					{
						case My_Sobek_Type_Enum.Logon:
							if (!String.IsNullOrEmpty(returnUrl))
								return this_base_url + "my/logon?return=" + HttpUtility.UrlEncode(returnUrl).Replace("%2c", ",")  + urlOptions2;
							return this_base_url + "my/logon" + urlOptions1;

						case My_Sobek_Type_Enum.Home:
							if (!String.IsNullOrEmpty(returnUrl))
								return this_base_url + "my/home?return=" + HttpUtility.UrlEncode(returnUrl).Replace("%2c", ",") + urlOptions2;
							return this_base_url + "my/home" + urlOptions1;

						case My_Sobek_Type_Enum.Delete_Item:
							return this_base_url + "my/delete/" + BibID + "/" + VID + urlOptions1;

						case My_Sobek_Type_Enum.New_Item:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/submit/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/submit" + urlOptions1;

						case My_Sobek_Type_Enum.Edit_Item_Behaviors:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/behaviors/" + BibID + "/" + VID + "/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/behaviors/" + BibID + "/" + VID + urlOptions1;

						case My_Sobek_Type_Enum.Edit_Item_Metadata:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/edit/" + BibID + "/" + VID + "/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/edit/" + BibID + "/" + VID + urlOptions1;

						case My_Sobek_Type_Enum.File_Management:
							return this_base_url + "my/files/" + BibID + "/" + VID + urlOptions1;

                        case My_Sobek_Type_Enum.Page_Images_Management:
                            return this_base_url + "my/images/" + BibID + "/" + VID + urlOptions1;

						case My_Sobek_Type_Enum.Edit_Group_Behaviors:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/groupbehaviors/" + BibID + "/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/groupbehaviors/" + BibID + urlOptions1;

						case My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/serialhierarchy/" + BibID + "/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/serialhierarchy/" + BibID + urlOptions1;

						case My_Sobek_Type_Enum.Group_Add_Volume:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/addvolume/" + BibID + "/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/addvolume/" + BibID + urlOptions1;

						case My_Sobek_Type_Enum.Group_AutoFill_Volumes:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/autofill/" + BibID + "/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/autofill/" + BibID + urlOptions1;

						case My_Sobek_Type_Enum.Group_Mass_Update_Items:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/massupdate/" + BibID + "/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/massupdate/" + BibID + urlOptions1;

						case My_Sobek_Type_Enum.Folder_Management:
							switch( Result_Display_Type )
							{
								case Result_Display_Type_Enum.Brief:
									if (!String.IsNullOrEmpty(mysobekSubmode))
									{
										if (Page > 1)
										{
											return this_base_url + "my/bookshelf/brief/" + mysobekSubmode + "/" + Page + urlOptions1;
										}
										return this_base_url + "my/bookshelf/brief/" + mysobekSubmode + urlOptions1;
									}
									return this_base_url + "my/bookshelf" + urlOptions1;

								case Result_Display_Type_Enum.Export:
									if (!String.IsNullOrEmpty(mysobekSubmode))
									{
										if (Page > 1)
										{
											return this_base_url + "my/bookshelf/export/" + mysobekSubmode + "/" + Page + urlOptions1;
										}
										return this_base_url + "my/bookshelf/export/" + mysobekSubmode + urlOptions1;
									}
									return this_base_url + "my/bookshelf" + urlOptions1;

								case Result_Display_Type_Enum.Full_Citation:
									if (!String.IsNullOrEmpty(mysobekSubmode))
									{
										if (Page > 1)
										{
											return this_base_url + "my/bookshelf/citation/" + mysobekSubmode + "/" + Page + urlOptions1;
										}
										return this_base_url + "my/bookshelf/citation/" + mysobekSubmode + urlOptions1;
									}
									return this_base_url + "my/bookshelf" + urlOptions1;

								case Result_Display_Type_Enum.Full_Image:
									if (!String.IsNullOrEmpty(mysobekSubmode))
									{
										if (Page > 1)
										{
											return this_base_url + "my/bookshelf/image/" + mysobekSubmode + "/" + Page + urlOptions1;
										}
										return this_base_url + "my/bookshelf/image/" + mysobekSubmode + urlOptions1;
									}
									return this_base_url + "my/bookshelf" + urlOptions1;

								case Result_Display_Type_Enum.Table:
									if (!String.IsNullOrEmpty(mysobekSubmode))
									{
										if (Page > 1)
										{
											return this_base_url + "my/bookshelf/table/" + mysobekSubmode + "/" + Page + urlOptions1;
										}
										return this_base_url + "my/bookshelf/table/" + mysobekSubmode + urlOptions1;
									}
									return this_base_url + "my/bookshelf" + urlOptions1;

								case Result_Display_Type_Enum.Thumbnails:
									if (!String.IsNullOrEmpty(mysobekSubmode))
									{
										if (Page > 1)
										{
											return this_base_url + "my/bookshelf/thumbs/" + mysobekSubmode + "/" + Page + urlOptions1;
										}
										return this_base_url + "my/bookshelf/thumbs/" + mysobekSubmode + urlOptions1;
									}
									return this_base_url + "my/bookshelf" + urlOptions1;

								default:
									if (!String.IsNullOrEmpty(mysobekSubmode))
									{
										if (Page > 1)
										{
											return this_base_url + "my/bookshelf/" + mysobekSubmode + "/" + Page + urlOptions1;
										}
										return this_base_url + "my/bookshelf/" + mysobekSubmode + urlOptions1;
									}
									return this_base_url + "my/bookshelf" + urlOptions1;
							}
							

						case My_Sobek_Type_Enum.Preferences:
							return this_base_url + "my/preferences" + urlOptions1;

						case My_Sobek_Type_Enum.Log_Out:
							if (!String.IsNullOrEmpty(returnUrl))
							{
								string modified_return_url = returnUrl;
								if ((modified_return_url.IndexOf("my/") == 0) || (modified_return_url == "my"))
									modified_return_url = string.Empty;
								if (modified_return_url.IndexOf("l/") == 0)
								{
									modified_return_url = modified_return_url.Length == 2 ? String.Empty : returnUrl.Substring(2);
								}
							   
								if ( modified_return_url.Length > 0 )
									return this_base_url + "my/logout?return=" + HttpUtility.UrlEncode(returnUrl).Replace("%2c", ",") + urlOptions2;
								return this_base_url + "my/logout" + urlOptions1;
							}
							return this_base_url + "my/logout" + urlOptions1;

						case My_Sobek_Type_Enum.Shibboleth_Landing:
							if (!String.IsNullOrEmpty(returnUrl))
								return this_base_url + "my/gatorlink?return=" + HttpUtility.UrlEncode(returnUrl).Replace("%2c", ",") + urlOptions2;
							return this_base_url + "my/gatorlink" + urlOptions1;

						case My_Sobek_Type_Enum.Saved_Searches:
							return this_base_url + "my/searches" + urlOptions1;

						case My_Sobek_Type_Enum.User_Tags:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/tags/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/tags" + urlOptions1;

						case My_Sobek_Type_Enum.User_Usage_Stats:
							if (!String.IsNullOrEmpty(mysobekSubmode))
								return this_base_url + "my/stats/" + mysobekSubmode + urlOptions1;
							return this_base_url + "my/stats" + urlOptions1;
					}
					break;

                case Display_Mode_Enum.Administrative:
                    switch (Admin_Type)
                    {
                        case Admin_Type_Enum.Home:
                            return this_base_url + "admin" + urlOptions1;

                        case Admin_Type_Enum.Aggregation_Single:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/editaggr/" + aggregation + "/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/editaggr/" + aggregation + urlOptions1;

                        case Admin_Type_Enum.Aggregations_Mgmt:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/aggregations/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/aggregations" + urlOptions1;

                        case Admin_Type_Enum.Builder_Status:
                            return this_base_url + "admin/builder" + urlOptions1;

                        case Admin_Type_Enum.Forwarding:
                            return this_base_url + "admin/aliases" + urlOptions1;

                        case Admin_Type_Enum.Interfaces:
                            return this_base_url + "admin/webskins" + urlOptions1;

                        case Admin_Type_Enum.Projects:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/projects/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/projects" + urlOptions1;

                        case Admin_Type_Enum.IP_Restrictions:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/restrictions/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/restrictions" + urlOptions1;

                        case Admin_Type_Enum.URL_Portals:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/portals/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/portals" + urlOptions1;

                        case Admin_Type_Enum.Users:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/users/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/users" + urlOptions1;

                        case Admin_Type_Enum.User_Groups:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/groups/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/groups" + urlOptions1;

                        case Admin_Type_Enum.Wordmarks:
                            return this_base_url + "admin/wordmarks" + urlOptions1;

                        case Admin_Type_Enum.Reset:
                            return this_base_url + "admin/reset" + urlOptions1;

                        case Admin_Type_Enum.Thematic_Headings:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/headings/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/headings" + urlOptions1;

                        case Admin_Type_Enum.Settings:
                            if (!String.IsNullOrEmpty(mysobekSubmode))
                                return this_base_url + "admin/settings/" + mysobekSubmode + urlOptions1;
                            return this_base_url + "admin/settings" + urlOptions1;

                        default:
                            return this_base_url + "admin" + urlOptions1;
                    }
                    break;

				case Display_Mode_Enum.Preferences:
					return this_base_url + "preferences" + urlOptions1;

				case Display_Mode_Enum.Statistics:
					switch (Statistics_Type)
					{
						case Statistics_Type_Enum.Item_Count_Standard_View:
							return this_base_url + "stats/itemcount" + urlOptions1;

						case Statistics_Type_Enum.Item_Count_Growth_View:
							return this_base_url + "stats/itemcount/growth" + urlOptions1;

						case Statistics_Type_Enum.Item_Count_Arbitrary_View:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "stats/itemcount/arbitrary/" + infoBrowseMode + urlOptions1;
							return this_base_url + "stats/itemcount/arbitrary" + urlOptions1;

						case Statistics_Type_Enum.Item_Count_Text:
							return this_base_url + "stats/itemcount/text" + urlOptions1;

						case Statistics_Type_Enum.Recent_Searches:
							return this_base_url + "stats/searches" + urlOptions1;

						case Statistics_Type_Enum.Usage_Overall:
							return this_base_url + "stats/usage" + urlOptions1;

						case Statistics_Type_Enum.Usage_Collection_History:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "stats/usage/history/" + infoBrowseMode + urlOptions1;
							return this_base_url + "stats/usage/history" + urlOptions1;

						case Statistics_Type_Enum.Usage_Collection_History_Text:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "stats/usage/history/text/" + infoBrowseMode + urlOptions1;
							return this_base_url + "stats/usage/history/text" + urlOptions1;

						case Statistics_Type_Enum.Usage_Collections_By_Date:
							if ( !String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "stats/usage/collections/" + infoBrowseMode + urlOptions1;
							return this_base_url + "stats/usage/collections" + urlOptions1;

						case Statistics_Type_Enum.Usage_Definitions:
							return this_base_url + "stats/usage/definitions" + urlOptions1;

						case Statistics_Type_Enum.Usage_Item_Views_By_Date:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "stats/usage/items/" + infoBrowseMode + urlOptions1;
							return this_base_url + "stats/usage/items" + urlOptions1;

						case Statistics_Type_Enum.Usage_Items_By_Collection:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "stats/usage/items/top/" + infoBrowseMode + urlOptions1;
							return this_base_url + "stats/usage/items/top" + urlOptions1;

						case Statistics_Type_Enum.Usage_Titles_By_Collection:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "stats/usage/titles/top/" + infoBrowseMode + urlOptions1;
							return this_base_url + "stats/usage/titles/top" + urlOptions1;

						case Statistics_Type_Enum.Usage_By_Date_Text:
							if (!String.IsNullOrEmpty(infoBrowseMode))
								return this_base_url + "stats/usage/items/text/" + infoBrowseMode + urlOptions1;
							return this_base_url + "stats/usage/items/text" + urlOptions1;
					}
					break;

				case Display_Mode_Enum.Item_Display:
				case Display_Mode_Enum.Item_Print:
                    if (!String.IsNullOrEmpty(bibid))
                    {
                        // Build the url for this item
                        StringBuilder itemDisplayBuilder = new StringBuilder(this_base_url + bibid.ToUpper(), 100);
                        if (!String.IsNullOrEmpty(vid))
                            itemDisplayBuilder.Append("/" + vid);
                        if (Mode == Display_Mode_Enum.Item_Print)
                            itemDisplayBuilder.Append("/print");
                        if (!String.IsNullOrEmpty(Item_View_Code))
                            itemDisplayBuilder.Append("/" + Item_View_Code);
                        if (SubPage > 1)
                            itemDisplayBuilder.Append("/" + SubPage.ToString());

                        bool query_string_started = false;

                        // Check for any query string to be included 
                        if (((String.IsNullOrEmpty(Item_View_Code)) && (!String.IsNullOrEmpty(pageByFilename))) || (!String.IsNullOrEmpty(textSearch)) || (!String.IsNullOrEmpty(coordinates)) ||
                            ((ViewerCode.IndexOf("x") >= 0) && ((Viewport_Point_X > 0) || (Viewport_Point_Y > 0) || (Viewport_Size != 1) || ((Viewport_Zoom - 1) > 0) || (Viewport_Rotation > 0))))
                        {
                            // Add either the text search or text display, if they exist
                            if (!String.IsNullOrEmpty(textSearch))
                            {
                                itemDisplayBuilder.Append("?search=" + HttpUtility.UrlEncode(textSearch));
                                query_string_started = true;
                            }

                            // Add the coordinates if they exist
                            if (!String.IsNullOrEmpty(coordinates))
                            {
                                if (!query_string_started)
                                {
                                    itemDisplayBuilder.Append("?coord=" + coordinates);
                                    query_string_started = true;
                                }
                                else
                                {
                                    itemDisplayBuilder.Append("&coord=" + coordinates);
                                }
                            }


                            // Add any viewport option information if this is a ZOOMABLE view
                            if (ViewerCode.IndexOf("x") >= 0)
                            {
                                int adjustedZoom = Viewport_Zoom - 1;
                                if ((Viewport_Size != 1) || (adjustedZoom > 0) || (Viewport_Rotation > 0))
                                {
                                    if (Viewport_Rotation > 0)
                                    {
                                        if (!query_string_started)
                                        {
                                            itemDisplayBuilder.Append("?vo=" + Viewport_Size + adjustedZoom + Viewport_Rotation);
                                            query_string_started = true;
                                        }
                                        else
                                        {
                                            itemDisplayBuilder.Append("&vo=" + Viewport_Size + adjustedZoom + Viewport_Rotation);
                                        }
                                    }
                                    else
                                    {
                                        if (adjustedZoom > 0)
                                        {
                                            if (!query_string_started)
                                            {
                                                itemDisplayBuilder.Append("?vo=" + Viewport_Size + adjustedZoom);
                                                query_string_started = true;
                                            }
                                            else
                                            {
                                                itemDisplayBuilder.Append("&vo=" + Viewport_Size + adjustedZoom);
                                            }
                                        }
                                        else
                                        {
                                            if (!query_string_started)
                                            {
                                                itemDisplayBuilder.Append("?vo=" + Viewport_Size);
                                                query_string_started = true;
                                            }
                                            else
                                            {
                                                itemDisplayBuilder.Append("&vo=" + Viewport_Size);
                                            }
                                        }
                                    }
                                }

                                // Only add the point if it is not 0,0
                                if ((Viewport_Point_X > 0) || (Viewport_Point_Y > 0))
                                {
                                    if (!query_string_started)
                                    {
                                        itemDisplayBuilder.Append("?vp=" + Viewport_Point_X + "," + Viewport_Point_Y);
                                        query_string_started = true;
                                    }
                                    else
                                    {
                                        itemDisplayBuilder.Append("&vp=" + Viewport_Point_X + "," + Viewport_Point_Y);
                                    }
                                }
                            }
                        }

                        //Add the number and size of thumbnails if this is the THUMBNAILS (Related Images) View
                        if (( ViewerCode.IndexOf("thumbs") >= 0) && ( Thumbnails_Per_Page >= -1 ))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?nt=" + Thumbnails_Per_Page);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&nt=" + Thumbnails_Per_Page);
                            }
                        }

                        if ((ViewerCode.IndexOf("thumbs") >= 0) && (Size_Of_Thumbnails > 0))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?ts=" + Size_Of_Thumbnails);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&ts=" + Size_Of_Thumbnails);
                            }
                        }

                        //Add the number, size of thumbnails and autonumbering mode if this is the QUALITY CONTROL (QC) View
                        if ((ViewerCode.IndexOf("qc") >= 0) && (Thumbnails_Per_Page >= -1))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?nt=" + Thumbnails_Per_Page);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&nt=" + Thumbnails_Per_Page);
                            }
                        }

                        if ((ViewerCode.IndexOf("qc") >= 0) && (Size_Of_Thumbnails > 0))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?ts=" + Size_Of_Thumbnails);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&ts=" + Size_Of_Thumbnails);
                            }
                        }

                        // Add the page by file information, if there is no viewer code
                        if ((String.IsNullOrEmpty(Item_View_Code)) && (!String.IsNullOrEmpty(pageByFilename)))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?file=" + pageByFilename);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&file=" + pageByFilename);
                            }
                        }


                        string returnValue = itemDisplayBuilder.ToString();
                        return (returnValue.IndexOf("?") > 0) ? returnValue + urlOptions2 : returnValue + urlOptions1;
                    }
			        break;

				case Display_Mode_Enum.Search:
					if ((!String.IsNullOrEmpty(adjusted_aggregation)) && ( adjusted_aggregation != defaultAggregation ))
					{
						switch (Search_Type)
						{
							case Search_Type_Enum.Advanced:
								return this_base_url + adjusted_aggregation + "/advanced" + urlOptions1;
							case Search_Type_Enum.Map:
								if( infoBrowseMode == "1" )
									return this_base_url + adjusted_aggregation + "/map/1" + urlOptions1;
								return this_base_url + adjusted_aggregation + "/map" + urlOptions1;
							case Search_Type_Enum.Full_Text:
							case Search_Type_Enum.dLOC_Full_Text:
								return this_base_url + adjusted_aggregation + "/text" + urlOptions1;
							default:
								return this_base_url + adjusted_aggregation;
						}
					}
					switch (Search_Type)
					{
						case Search_Type_Enum.Advanced:
							return this_base_url + "advanced" + urlOptions1; 
						case Search_Type_Enum.Map:
							return this_base_url + "map" + urlOptions1;
						case Search_Type_Enum.Full_Text:
						case Search_Type_Enum.dLOC_Full_Text:
							return this_base_url + "text" + urlOptions1;
						default:
							return this_base_url + urlOptions1;
					}


				case Display_Mode_Enum.Results:
					StringBuilder results_url_builder = new StringBuilder(this_base_url);
					if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != defaultAggregation))
					{
						results_url_builder.Append(adjusted_aggregation + "/");
					}
					switch( Search_Precision )
					{
						case Search_Precision_Type_Enum.Contains:
							results_url_builder.Append("contains/");
							break;

						case Search_Precision_Type_Enum.Synonmic_Form:
							results_url_builder.Append("resultslike/");
							break;

						case Search_Precision_Type_Enum.Inflectional_Form:
							results_url_builder.Append("results/");
							break;

						case Search_Precision_Type_Enum.Exact_Match:
							results_url_builder.Append("exact/");
							break;
					}

					// Add the results display type into the search results URL
					switch (Result_Display_Type)
					{
						case Result_Display_Type_Enum.Brief:
							results_url_builder.Append("brief/");
							break;
						case Result_Display_Type_Enum.Export:
							results_url_builder.Append("export/");
							break;
						case Result_Display_Type_Enum.Full_Citation:
							results_url_builder.Append("citation/");
							break;
						case Result_Display_Type_Enum.Full_Image:
							results_url_builder.Append("image/");
							break;
						case Result_Display_Type_Enum.Map:
							results_url_builder.Append("map/");
							break;
						case Result_Display_Type_Enum.Table:
							results_url_builder.Append("table/");
							break;
						case Result_Display_Type_Enum.Thumbnails:
							results_url_builder.Append("thumbs/");
							break;
					}
					// Add the page into the search results URL
					if (Page > 1)
					{
						results_url_builder.Append(Page.ToString() + "/");
					}
					// Add the search terms onto the search results URL
					if ((Search_String.Length > 0) || !string.IsNullOrEmpty(searchFields))
					{
						if ((Search_Type == Search_Type_Enum.Basic) && (Search_String.Length > 0))
						{
							results_url_builder.Append("?t=" + HttpUtility.UrlEncode(Search_String).Replace("%2c", ","));
						}

						if (Search_Type == Search_Type_Enum.Advanced)
						{
							if (Search_String.Length > 0)
							{
								results_url_builder.Append("?t=" + HttpUtility.UrlEncode(Search_String).Replace("%2c", ",") + "&f=" + Search_Fields);
							}
							else
							{
								results_url_builder.Append("?f=" + searchFields);
							}
						}

						if (Search_Type == Search_Type_Enum.Full_Text)
						{
							results_url_builder.Append("?text=" + HttpUtility.UrlEncode(Search_String).Replace("%2c", ","));
						}

						if (Search_Type == Search_Type_Enum.Map)
						{
							results_url_builder.Append("?coord=" + HttpUtility.UrlEncode(Coordinates).Replace("%2c", ","));
						}

						// Add the sort order
						if (Sort > 0)
						{
							results_url_builder.Append("&o=" + Sort);
						}
					}

					string returnValue2 = results_url_builder.ToString();
					if (returnValue2.IndexOf("?") > 0)
						return returnValue2 + urlOptions2;
					return returnValue2 + urlOptions1;


				case Display_Mode_Enum.Aggregation_Home:
					if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != defaultAggregation))
						return this_base_url + adjusted_aggregation + urlOptions1;
					switch (Home_Type)
					{
						case Home_Type_Enum.Descriptions:
							return this_base_url + "brief" + urlOptions1;

						case Home_Type_Enum.Tree_Collapsed:
							return this_base_url + "tree" + urlOptions1;

						case Home_Type_Enum.Tree_Expanded:
							return this_base_url + "tree/expanded" + urlOptions1;

						case Home_Type_Enum.Partners_List:
							return this_base_url + "partners" + urlOptions1;

						case Home_Type_Enum.Partners_Thumbnails:
							return this_base_url + "partners/thumbs" + urlOptions1;

						case Home_Type_Enum.Personalized:
							return this_base_url + "personalized" + urlOptions1;

						default:
							return this_base_url + urlOptions1;
					}


				case Display_Mode_Enum.Aggregation_Browse_By:
					string browse_by_mode = String.Empty;
					if (!String.IsNullOrEmpty(infoBrowseMode))
						browse_by_mode = infoBrowseMode.Replace(" ", "_");
					if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != defaultAggregation))
					{
						if ((Page > 1) && (browse_by_mode.Length > 0))
						{
							return this_base_url + adjusted_aggregation + "/browseby/" + browse_by_mode + "/" + Page + urlOptions1;
						}
						return this_base_url + adjusted_aggregation + "/browseby/" + browse_by_mode + urlOptions1;
					}
					if ((Page > 1) && (browse_by_mode.Length > 0))
					{
						return this_base_url + "browseby/" + browse_by_mode + "/" + Page + urlOptions1;
					}
					return this_base_url + "browseby/" + browse_by_mode + urlOptions1;

				case Display_Mode_Enum.Aggregation_Browse_Map:
					if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != defaultAggregation))
					{
						return this_base_url + adjusted_aggregation + "/geography/" + infoBrowseMode + urlOptions1;
					}
					return this_base_url + "geography/" + infoBrowseMode + urlOptions1;

				case Display_Mode_Enum.Aggregation_Item_Count:
					if (!String.IsNullOrEmpty(adjusted_aggregation))
					{
						return this_base_url + adjusted_aggregation + "/itemcount/" + infoBrowseMode + urlOptions1;
					}
					return this_base_url + "itemcount/" + infoBrowseMode + urlOptions1;

				case Display_Mode_Enum.Aggregation_Usage_Statistics:
					if (!String.IsNullOrEmpty(adjusted_aggregation))
					{
						return this_base_url + adjusted_aggregation + "/usage/" + infoBrowseMode + urlOptions1;
					}
					return this_base_url + "usage/" + infoBrowseMode + urlOptions1;

				case Display_Mode_Enum.Aggregation_Private_Items:
					if (Page > 1)
					{
						if (!String.IsNullOrEmpty(adjusted_aggregation))
						{
							if (Sort == 0)
								return this_base_url + adjusted_aggregation + "/inprocess/" + Page + urlOptions1;
							return this_base_url + adjusted_aggregation + "/inprocess/" + Page + "?o=" + Sort + urlOptions2;
						}
						if (Sort == 0)
							return this_base_url + "inprocess/" + Page + urlOptions1;
						return this_base_url + "inprocess/" + Page + "?o=" + Sort + urlOptions2;
					}
					if (!String.IsNullOrEmpty(adjusted_aggregation))
					{
						if (Sort == 0)
							return this_base_url + adjusted_aggregation + "/inprocess" + urlOptions1;
						return this_base_url + adjusted_aggregation + "/inprocess?o=" + Sort + urlOptions2;
					}
					if (Sort == 0)
						return this_base_url + "inprocess" + urlOptions1;
					return this_base_url + "inprocess?o=" + Sort + urlOptions2;

				case Display_Mode_Enum.Aggregation_Admin_View:
					if (!String.IsNullOrEmpty(adjusted_aggregation))
					{
						return this_base_url + adjusted_aggregation + "/admin" + urlOptions1;
					}
					return this_base_url + "admin" +  urlOptions1;

				case Display_Mode_Enum.Aggregation_Browse_Info:
					if ( Sort > 0 )
					{
						if (url_options.Length > 0)
						{
							urlOptions1 = "?" + url_options + "&o=" + Sort;
						}
						else
						{
							urlOptions1 = "?o=" + Sort;
						}
					}

					if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != defaultAggregation))
					{
						if (Page > 1)
						{
							switch (Result_Display_Type)
							{
								case Result_Display_Type_Enum.Brief:
									return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/brief/" + Page + urlOptions1;
								case Result_Display_Type_Enum.Export:
									return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/export/" + Page + urlOptions1;
								case Result_Display_Type_Enum.Full_Citation:
									return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/citation/" + Page + urlOptions1;
								case Result_Display_Type_Enum.Full_Image:
									return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/image/" + Page + urlOptions1;
								case Result_Display_Type_Enum.Map:
									return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/map/" + Page + urlOptions1;
								case Result_Display_Type_Enum.Table:
									return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/table/" + Page + urlOptions1;
								case Result_Display_Type_Enum.Thumbnails:
									return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/thumbs/" + Page + urlOptions1;
								default:
									return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/" + Page + urlOptions1;
							}
						}
						switch (Result_Display_Type)
						{
							case Result_Display_Type_Enum.Brief:
								return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/brief" + urlOptions1;
							case Result_Display_Type_Enum.Export:
								return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/export" + urlOptions1;
							case Result_Display_Type_Enum.Full_Citation:
								return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/citation" + urlOptions1;
							case Result_Display_Type_Enum.Full_Image:
								return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/image" + urlOptions1;
							case Result_Display_Type_Enum.Map:
								return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/map" + urlOptions1;
							case Result_Display_Type_Enum.Table:
								return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/table" + urlOptions1;
							case Result_Display_Type_Enum.Thumbnails:
								return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + "/thumbs" + urlOptions1;
							default:
								return this_base_url + adjusted_aggregation + "/" + infoBrowseMode + urlOptions1;
						}
					}
					// See if you need to include 'info' here
					string pre_mode_string = "info/";
					if ((infoBrowseMode == "all") || (infoBrowseMode == "new"))
						pre_mode_string = String.Empty;
					if (Page > 1)
					{
						switch (Result_Display_Type)
						{
							case Result_Display_Type_Enum.Brief:
								return this_base_url + pre_mode_string + infoBrowseMode + "/brief/" + Page + urlOptions1;
							case Result_Display_Type_Enum.Export:
								return this_base_url + pre_mode_string + infoBrowseMode + "/export/" + Page + urlOptions1;
							case Result_Display_Type_Enum.Full_Citation:
								return this_base_url + pre_mode_string + infoBrowseMode + "/citation/" + Page + urlOptions1;
							case Result_Display_Type_Enum.Full_Image:
								return this_base_url + pre_mode_string + infoBrowseMode + "/image/" + Page + urlOptions1;
							case Result_Display_Type_Enum.Map:
								return this_base_url + pre_mode_string + infoBrowseMode + "/map/" + Page + urlOptions1;
							case Result_Display_Type_Enum.Table:
								return this_base_url + pre_mode_string + infoBrowseMode + "/table/" + Page + urlOptions1;
							case Result_Display_Type_Enum.Thumbnails:
								return this_base_url + pre_mode_string + infoBrowseMode + "/thumbs/" + Page + urlOptions1;
							default:
								return this_base_url + pre_mode_string + infoBrowseMode + "/" + Page + urlOptions1;
						}
					}
					switch (Result_Display_Type)
					{
						case Result_Display_Type_Enum.Brief:
							return this_base_url + pre_mode_string + infoBrowseMode + "/brief" + urlOptions1;
						case Result_Display_Type_Enum.Export:
							return this_base_url + pre_mode_string + infoBrowseMode + "/export" + urlOptions1;
						case Result_Display_Type_Enum.Full_Citation:
							return this_base_url + pre_mode_string + infoBrowseMode + "/citation" + urlOptions1;
						case Result_Display_Type_Enum.Full_Image:
							return this_base_url + pre_mode_string + infoBrowseMode + "/image" + urlOptions1;
						case Result_Display_Type_Enum.Map:
							return this_base_url + pre_mode_string + infoBrowseMode + "/map" + urlOptions1;
						case Result_Display_Type_Enum.Table:
							return this_base_url + pre_mode_string + infoBrowseMode + "/table" + urlOptions1;
						case Result_Display_Type_Enum.Thumbnails:
							return this_base_url + pre_mode_string + infoBrowseMode + "/thumbs" + urlOptions1;
						default:
							return this_base_url + pre_mode_string + infoBrowseMode + urlOptions1;
					}
			}

			return this_base_url + "unknown";
		}

		/// <summary> Returns the URL options the user has currently set </summary>
		/// <returns>URL options</returns>
		public string URL_Options()
		{
			// If this is a robot, we do not allow them to set anything through the query string
			if (Is_Robot)
				return String.Empty;

			// Define the StringBuilder
			StringBuilder redirect = new StringBuilder( );

			if (Trace_Flag == Trace_Flag_Type_Enum.Explicit)
				redirect.Append("trace=yes");

			// Was there an interface?
			if (!string.IsNullOrEmpty(currSkin) && ( String.Compare( currSkin,  defaultSkin, true  ) != 0 ))
			{
				if (redirect.Length > 0)
					redirect.Append("&");
				redirect.Append( "n=" + currSkin.ToLower() );
			}

			// Add language if it is not the browser default
			if (Language != Default_Language)
			{
				switch (Language)
				{
					case Web_Language_Enum.English:
						if (redirect.Length > 0)
							redirect.Append("&");
						redirect.Append("l=en");
						break;

					case Web_Language_Enum.French:
						if (redirect.Length > 0)
							redirect.Append("&");
						redirect.Append("l=fr");
						break;

					case Web_Language_Enum.Spanish:
						if (redirect.Length > 0)
							redirect.Append("&");
						redirect.Append("l=sp");
						break;

					case Web_Language_Enum.TEMPLATE:
						if (redirect.Length > 0)
							redirect.Append("&");
						redirect.Append("l=XXXXX");
						break;
				}
			}

			// Return the built string
			return redirect.ToString();
		}

		#endregion

		#region Methods which return the base directory or base url with a constant ending to indicate the SobekCM standard subfolders

		/// <summary> URL for the general image folder containing images used throughout UFDC, and not aggregation or item specific </summary>
		/// <value> [Base_URL] + 'default/images/' </value>
		public string Default_Images_URL
		{
			get { return baseUrl + "default/images/"; }
		}

		/// <summary> URL to this application's design folder </summary>
		/// <value> [Base_URL] + 'design/' </value>
		public string Base_Design_URL
		{
			get { return baseUrl + "design/"; }
		}

		/// <summary> URL for the watermarks/icons under the design folder </summary>
		/// <value> [Base_URL] + 'design/wordmarks/' </value>
		public string Watermarks_URL
		{
			get { return baseUrl + "design/wordmarks/"; }
		}

		#endregion

		/// <summary> Returns the URL to redirect the user's browser, based on the current
		/// mode and specifics for this mode. </summary>
		/// <param name="Include_URL_Opts"> Flag indicates whether to include URL opts or not </param>
		/// <returns> String to be attached to the end of the main application name to redirect
		/// the current user's browser.  </returns>
		public string Redirect_URL(bool Include_URL_Opts)
		{
			return Redirect_URL(ViewerCode, Include_URL_Opts);
		}

		/// <summary> Returns the URL to redirect the user's browser, based on the current
		/// mode and specifics for this mode. </summary>
		/// <param name="Item_View_Code">Item view code to display</param>
		/// <returns> String to be attached to the end of the main application name to redirect
		/// the current user's browser.  </returns>
		public string Redirect_URL(string Item_View_Code)
		{
			return Redirect_URL(Item_View_Code, true);
		}

        /// <summary> Redirect the user to the current mode's URL </summary>
        /// <remarks> This does not stop execution immediately (which would raise a ThreadAbortedException
        /// and be costly in terms of performance) but it does set the 
        /// Request_Completed flag, which should be checked and will effectively stop any 
        /// further actions. </remarks>
        public void Redirect()
        {
            HttpContext.Current.Response.Redirect(Redirect_URL(), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Request_Completed = true;
        }

        /// <summary> Redirect the user to the current mode's URL </summary>
        /// <param name="Flush_Response">Flag indicates if the response should be flushed</param>
        /// <remarks> This does not stop execution immediately (which would raise a ThreadAbortedException
        /// and be costly in terms of performance) but it does set the 
        /// Request_Completed flag, which should be checked and will effectively stop any 
        /// further actions. </remarks>
        public void Redirect( bool Flush_Response )
        {
            if (Flush_Response)
                HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Redirect(Redirect_URL(), false);
            Request_Completed = true;
        }
	}
}
