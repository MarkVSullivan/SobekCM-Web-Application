#region Using directives

using System;
using SobekCM.Core.Configuration;

#endregion

namespace SobekCM.Core.Navigation
{
	/// <summary> This object stores the current and next mode information for a single HTTP request. <br /> <br /> </summary>
	/// <remarks>  Object written by Mark V Sullivan for the University of Florida. </remarks>
	public class SobekCM_Navigation_Object 
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
			DateRange_Year1 = -1;
			DateRange_Year2 = -1;
			DateRange_Date1 = -1;
			DateRange_Date2 = -1;


			Invalid_Item = false;
			Skin_In_URL = false;
			isPostBack = false;
			Show_Selection_Panel = false;
			Is_Robot = false;
			Internal_User = false;
			Logon_Required = false;
		    Request_Completed = false;

            //beta
		    Use_Beta = false;

		}

		#endregion

		#region Code to set the robot flag from request variables

		/// <summary> Algorithm tests the user agent and IP address against known robots 
		/// to determine if this request is from a search engine indexer or web site crawler bot. </summary>
		/// <param name="UserAgent">User Agent string from the HTTP request</param>
		/// <param name="IP">IP address from the HTTP request</param>
		/// <returns>TRUE if the request appears to be a robot, otherwise FALSE</returns>
		public static bool Is_UserAgent_IP_Robot(string UserAgent, string IP)
		{
			if (UserAgent != null)
			{
				string useragent_upper = UserAgent.ToUpper();

				if (useragent_upper.IndexOf("BOT") >= 0)
				{
					if ((useragent_upper.IndexOf("MSNBOT") >= 0) || (useragent_upper.IndexOf("GIGABOT") >= 0) || (useragent_upper.IndexOf("GOOGLEBOT") >= 0) || (useragent_upper.IndexOf("AISEARCHBOT") >= 0) || (useragent_upper.IndexOf("CCBOT") >= 0) || (useragent_upper.IndexOf("PLONEBOT") >= 0) || (useragent_upper.IndexOf("CAZOODLEBOT") >= 0) || (useragent_upper.IndexOf("DISCOBOT") >= 0) || (useragent_upper.IndexOf("BINGBOT") >= 0) || (useragent_upper.IndexOf("YANDEXBOT") >= 0) || (useragent_upper.IndexOf("ATRAXBOT") >= 0) || (useragent_upper.IndexOf("MJ12BOT") >= 0) || (useragent_upper.IndexOf("SITEBOT") >= 0) || (useragent_upper.IndexOf("LINGUEE+BOT") >= 0) || (useragent_upper.IndexOf("MLBOT") >= 0) || (useragent_upper.IndexOf("NEXTGENSEARCHBOT") >= 0) || (useragent_upper.IndexOf("BENDERTHEWEBROBOT") >= 0) || (useragent_upper.IndexOf("EZOOMS.BOT") >= 0) || (useragent_upper.IndexOf("LSSBOT") == 0) || (useragent_upper.IndexOf("DISCOVERYBOT") >= 0))
					{
						return true;
					}
				}

				if ((useragent_upper.IndexOf("CRAWLER") >= 0) || (useragent_upper.IndexOf("SLURP") >= 0) || (useragent_upper.IndexOf("WEBVAC") >= 0) || (useragent_upper.IndexOf("ABOUT.ASK.COM") >= 0) || (useragent_upper.IndexOf("SCOUTJET") >= 0) || (useragent_upper.IndexOf("SITESUCKER") >= 0) || (useragent_upper.IndexOf("SEARCHME.COM") >= 0) || (useragent_upper.IndexOf("PICSEARCH.COM") >= 0) || (useragent_upper.IndexOf("XENU+LINK+SLEUTH") >= 0) || (useragent_upper.IndexOf("YANDEX") >= 0) || (useragent_upper.IndexOf("JAVA/") == 0) || (useragent_upper.IndexOf("SOGOU+WEB+SPIDER") >= 0) || (useragent_upper.IndexOf("CAMONTSPIDER") >= 0))
				{
					return true;
				}

				if ((useragent_upper.IndexOf("BAIDUSPIDER") >= 0) || (useragent_upper.IndexOf("ICOPYRIGHT+CONDUCTOR") >= 0) || (useragent_upper.IndexOf("HTTP://AHREFS.COM/ROBOT") >= 0) || (useragent_upper.IndexOf("BENDERTHEROBOT.TUMBLR.COM") >= 0) || (useragent_upper.IndexOf("SHOULU.JIKE.COM/SPIDER") >= 0))
				{
					return true;
				}

				if ((useragent_upper.IndexOf("WWW.PROFOUND.NET") >= 0) || (useragent_upper.IndexOf("URLAPPENDBOT") >= 0) || (useragent_upper.IndexOf("SEARCHMETRICBOT") >= 0) || (useragent_upper.IndexOf("HAVIJ") >= 0) || (useragent_upper.IndexOf("SYNAPSE") >= 0) || (useragent_upper.IndexOf("BEWSLEBOT") >= 0) || (useragent_upper.IndexOf("SOSOSPIDER") >= 0) || (useragent_upper.IndexOf("YYSPIDER") >= 0) || (useragent_upper.IndexOf("WBSEARCHBOT") >= 0))
				{
					return true;
				}
			}

			// First IP address is for test purposes only
			if ((IP == "128.227.223.160") || (IP == "216.118.117.45") || (IP.IndexOf("65.55.230.") == 0) || (IP == "92.82.225.56") || (IP.IndexOf("220.181.51.") == 0) || (IP == "193.105.210.170") || (IP == "192.162.19.21"))
			{
				return true;
			}

			return false;
		}

		/// <summary> Algorithm tests the user agent and IP address against known robots 
		/// to determine if this request is from a search engine indexer or web site crawler bot.  
		/// This returns the value and also sets an internal robot flag. </summary>
		/// <param name="UserAgent">User Agent string from the HTTP request</param>
		/// <param name="IP">IP address from the HTTP request</param>
		/// <returns>TRUE if the request appears to be a robot, otherwise FALSE</returns>
		public bool Set_Robot_Flag(string UserAgent, string IP)
		{
			Is_Robot = Is_Robot || Is_UserAgent_IP_Robot(UserAgent, IP);
			return Is_Robot;
        }

		#endregion

		#region Public Properties

		/// <summary> Name of the report requested from the reporting module </summary>
		public string Report_Name { get; set; }

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

		/// <summary> Returns the name of this instance ( i.e., 'UDC', 'dLOC', etc... ) </summary>
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

		/// <summary> Returns the name of this instance ( i.e., 'UDC', 'dLOC', etc... ) </summary>
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
		public bool Skin_In_URL { get; set; }

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

		/// <summary> Submode for agrgegation views </summary>
		public Aggregation_Type_Enum Aggregation_Type { get; set;  }

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

		/// <summary> Information about which sub aggregationPermissions to include or exclude
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

		/// <summary> Beginning of the year range, if the search includes
		/// a date range between two years </summary>
		/// <value>-1 if no year range</value>
		public short DateRange_Year1 { get; set; }

		/// <summary> End of the year range, if the search includes
		/// a date range between two years </summary>
		/// <value>-1 if no year range</value>
		public short DateRange_Year2 { get; set; }

		/// <summary> Beginning of a date range, if the search includes
		/// a date range between two arbitrary dates </summary>
		/// <value>-1 if no year range</value>
		public long DateRange_Date1 { get; set; }

		/// <summary> End of a date range, if the search includes
		/// a date range between two arbitrary dates </summary>
		/// <value>-1 if no year range</value>
		public long DateRange_Date2 { get; set; }


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

        /// <summary> Flag indicates if the we are using beta</summary>
        public bool Use_Beta { get; set; }

		#endregion


		#region Methods which return the base directory or base url with a constant ending to indicate the SobekCM standard subfolders

        ///// <summary> URL for the general image folder containing images used throughout the system, and not aggregation or item specific </summary>
        ///// <value> [Base_URL] + 'default/images/' </value>
        //public string Default_Images_URL
        //{
        //    get { return baseUrl + "default/images/"; }
        //}

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


	}
}
