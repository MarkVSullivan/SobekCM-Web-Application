#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration;

#endregion

namespace SobekCM.Core.Navigation
{
	/// <summary> This object stores the current and next mode information for a single HTTP request. <br /> <br /> </summary>
	/// <remarks>  Object written by Mark V Sullivan for the University of Florida. </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("navigationObject")]
	public class Navigation_Object 
	{
		#region Private members of this object

		private string searchFields;
		private string searchString;

		#endregion

		#region Constructors

		/// <summary> Constructor for a new instance of the SobekCM_Navigation_Object which stores 
		/// all of the information about an individual request. </summary>
		public Navigation_Object()
		{
			// Do general item construction
			Constructor_Helper();
		}

		private void Constructor_Helper()
		{
			// Declare some defaults
		    Admin_Type = Admin_Type_Enum.NONE;
			Mode = Display_Mode_Enum.Error;
            Search_Type = Search_Type_Enum.NONE;
            Result_Display_Type = Result_Display_Type_Enum.NONE;
            Statistics_Type = Statistics_Type_Enum.NONE;
            Internal_Type = Internal_Type_Enum.NONE;
            My_Sobek_Type = My_Sobek_Type_Enum.NONE;
			Language = Web_Language_Enum.English;
			Default_Language = Web_Language_Enum.English;
			Writer_Type = Writer_Type_Enum.HTML;
			TOC_Display = TOC_Display_Type_Enum.Undetermined;
			Trace_Flag = Trace_Flag_Type_Enum.Unspecified;
			Search_Precision = Search_Precision_Type_Enum.Contains;
            WebContent_Type = WebContent_Type_Enum.NONE;


			Skin = "sobek";
			Default_Skin = "sobek";
		    Instance_Abbreviation = "SOBEK";
            Instance_Name = "Default SobekCM Library";

			Skin_In_URL = false;
			isPostBack = false;
			Is_Robot = false;
			Logon_Required = false;
		    Request_Completed = false;
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


        /// <summary> Admin type of display for the current request.</summary>
        [DataMember(EmitDefaultValue = false, Name = "adminType")]
        [XmlElement("adminType")]
        [ProtoMember(1)]
        public Admin_Type_Enum Admin_Type { get; set; }

        /// <summary> Current aggregation code </summary>
        /// <remarks>The value returned is always lower case</remarks>
        [DataMember(EmitDefaultValue = false, Name = "aggregation")]
        [XmlElement("aggregation")]
        [ProtoMember(2)]
        public string Aggregation { get; set; }

        /// <summary> Current aggregation alias code, if there is one </summary>
        /// <remarks>The value returned is always lower case</remarks>
        [DataMember(EmitDefaultValue = false, Name = "aggrAlias")]
        [XmlElement("aggrAlias")]
        [ProtoMember(3)]
        public string Aggregation_Alias { get; set; }

        /// <summary> Submode for agrgegation views </summary>
        [DataMember(EmitDefaultValue = false, Name = "aggrType")]
        [XmlElement("aggrType")]
        [ProtoMember(4)]
        public Aggregation_Type_Enum Aggregation_Type { get; set; }

        /// <summary> Base infterface interface for display purposes </summary>
        /// <remarks> The value returned is always lower case</remarks>
        [DataMember(EmitDefaultValue = false, Name = "baseSkin")]
        [XmlElement("baseSkin")]
        [ProtoMember(5)]
        public string Base_Skin { get; set; }

        /// <summary> Base infterface interface for display purposes </summary>
        /// <remarks> The value returned is always lower case</remarks>
        [XmlIgnore]
        [IgnoreDataMember]
        public string Base_Skin_Or_Skin
        {
            get
            {
                if (!string.IsNullOrEmpty(Base_Skin))
                    return Base_Skin;
                return Skin ?? String.Empty;
            }
        }

        /// <summary> Base URL requested by the user </summary>
        /// <remarks> This is the URL post-rewriting from any URL path rewrite routine (such as SobekCM_URL_Rewriter) </remarks>
        [DataMember(EmitDefaultValue = false, Name = "baseUrl")]
        [XmlAttribute("baseUrl")]
        [ProtoMember(6)]
        public string Base_URL { get; set; }

        /// <summary> Bib id to display </summary>
        /// <remarks>The value returned is always upper case </remarks>
        [DataMember(EmitDefaultValue = false, Name = "bibid")]
        [XmlElement("bibid")]
        [ProtoMember(7)]
        public string BibID { get; set; }

        /// <summary> Browser type </summary>
        [DataMember(EmitDefaultValue = false, Name = "browser")]
        [XmlElement("browser")]
        [ProtoMember(8)]
        public string Browser_Type { get; set; }

        /// <summary> Exception generated during execution </summary>
        [DataMember(EmitDefaultValue = false, Name = "exception")]
        [XmlIgnore]
        [ProtoMember(9)]
        public Exception Caught_Exception { get; set; }

        /// <summary> Coordinate search string for a geographic search </summary>
        [DataMember(EmitDefaultValue = false, Name = "coordinates")]
        [XmlElement("coordinates")]
        [ProtoMember(10)]
        public string Coordinates { get; set; }

        /// <summary> Beginning of a date range, if the search includes
        /// a date range between two arbitrary dates </summary>
        [DataMember(EmitDefaultValue = false, Name = "dateRangeDate1")]
        [XmlElement("dateRangeDate1")]
        [ProtoMember(11)]
        public long? DateRange_Date1 { get; set; }

        /// <summary> End of a date range, if the search includes
        /// a date range between two arbitrary dates </summary>
        [DataMember(EmitDefaultValue = false, Name = "dateRangeDate2")]
        [XmlElement("dateRangeDate2")]
        [ProtoMember(12)]
        public long? DateRange_Date2 { get; set; }

        /// <summary> Beginning of the year range, if the search includes
        /// a date range between two years </summary>
        [DataMember(EmitDefaultValue = false, Name = "dateRangeYear1")]
        [XmlElement("dateRangeYear1")]
        [ProtoMember(13)]
        public short? DateRange_Year1 { get; set; }

        /// <summary> End of the year range, if the search includes
        /// a date range between two years </summary>
        [DataMember(EmitDefaultValue = false, Name = "dateRangeYear2")]
        [XmlElement("dateRangeYear2")]
        [ProtoMember(14)]
        public short? DateRange_Year2 { get; set; }

        /// <summary> Default aggregation (based on original URL) </summary>
        /// <remarks>The value returned is always lower case</remarks>
        [DataMember(EmitDefaultValue = false, Name = "defaultAggregation")]
        [XmlElement("defaultAggregation")]
        [ProtoMember(15)]
        public string Default_Aggregation { get; set; }

        /// <summary> Default language for this user, from their browser settings </summary>
        [DataMember(EmitDefaultValue = false, Name = "defaultLanguage")]
        [XmlElement("defaultLanguage")]
        [ProtoMember(16)]
        public Web_Language_Enum Default_Language { get; set; }

        /// <summary> Default interface (based on original URL) </summary>
        /// <remarks>The value returned is always lower case</remarks>
        [DataMember(EmitDefaultValue = false, Name = "defaultSkin")]
        [XmlElement("defaultSkin")]
        [ProtoMember(17)]
        public string Default_Skin { get; set; }
        
        /// <summary> Simple error message generated during execution </summary>
        [DataMember(EmitDefaultValue = false, Name = "error")]
        [XmlElement("error")]
        [ProtoMember(18)]
        public string Error_Message { get; set; }

        /// <summary> Primary key for the folder to display </summary>
        [DataMember(EmitDefaultValue = false, Name = "folder")]
        [XmlElement("folder")]
        [ProtoMember(19)]
        public int? FolderID { get; set; }

        /// <summary> Fragment utilized when only a portion of a page needs to be rendered </summary>
        [DataMember(EmitDefaultValue = false, Name = "fragment")]
        [XmlElement("fragment")]
        [ProtoMember(20)]
        public string Fragment { get; set; }

        /// <summary>Submode for the main library home page </summary>
        [DataMember(EmitDefaultValue = false, Name = "homeType")]
        [XmlElement("homeType")]
        [ProtoMember(21)]
        public Home_Type_Enum Home_Type { get; set; }
        
        /// <summary> Browse or info mode to display </summary>
        /// <remarks>The value returned is always lower case</remarks>
        [DataMember(EmitDefaultValue = false, Name = "infoBrowseMode")]
        [XmlElement("infoBrowseMode")]
        [ProtoMember(22)]
        public string Info_Browse_Mode { get; set; }

        /// <summary> Returns the name of this instance ( i.e., 'UDC', 'dLOC', etc... ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "instanceAbbreviation")]
        [XmlElement("instanceAbbreviation")]
        [ProtoMember(23)]
        public string Instance_Abbreviation { get; set; }

        /// <summary> Returns the name of this instance ( i.e., 'UDC', 'dLOC', etc... ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "instanceName")]
        [XmlElement("instanceName")]
        [ProtoMember(24)]
        public string Instance_Name { get; set; }

        /// <summary> Submode for the internal pages </summary>
        [DataMember(EmitDefaultValue = false, Name = "internalType")]
        [XmlElement("internalType")]
        [ProtoMember(25)]
        public Internal_Type_Enum Internal_Type { get; set; }

        /// <summary> Flag that is set to indicate item requested is invalid </summary>
        [DataMember(EmitDefaultValue = false, Name = "invalidItem")]
        [XmlElement("invalidItem")]
        [ProtoMember(27)]
        public bool? Invalid_Item { get; set; }

        /// <summary> Flag indicating this is a post back </summary>
        [DataMember(Name = "isPostBack")]
        [XmlAttribute("isPostBack")]
        [ProtoMember(28)]
        public bool isPostBack { get; set; }

        /// <summary> Flag indicating if the current request is from a search engine 
        /// indexer or web site crawler bot.</summary>
        /// <remarks>This value is set in by calling the <see cref="Set_Robot_Flag"/> procedure. </remarks>
        [DataMember(Name = "isRobot")]
        [XmlAttribute("isRobot")]
        [ProtoMember(29)]
        public bool Is_Robot { get; set; }

        /// <summary> (DEPRECATED) ItemID which formerly was used for indicating items in the URL </summary>
        [DataMember(EmitDefaultValue = false, Name = "itemId")]
        [XmlElement("itemId")]
        [ProtoMember(30)]
        public int? ItemID_DEPRECATED { get; set; }

        /// <summary> Language for the interface </summary>
        [DataMember(EmitDefaultValue = false, Name = "language")]
        [XmlElement("language")]
        [ProtoMember(31)]
        public Web_Language_Enum Language { get; set; }

        /// <summary> Language code for the current skin language  </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public string Language_Code
        {
            get { return Web_Language_Enum_Converter.Enum_To_Code(Language); }
        }

        /// <summary> Flag indicates that logon is required to access the requested mode </summary>
        [DataMember(EmitDefaultValue = false, Name = "logonRequired")]
        [XmlElement("logonRequired")]
        [ProtoMember(32)]
        public bool Logon_Required { get; set; }

        /// <summary> Flag indicates if the requested web content page (or item) is not present (possibly a bad URL) </summary>
        [DataMember(EmitDefaultValue = false, Name = "missing")]
        [XmlElement("missing")]
        [ProtoMember(75)]
        public bool? Missing { get; set; }

        /// <summary> Method suppresses XML Serialization of the Missing flag property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeMissing()
        {
            return  Missing.HasValue;
        }

        /// <summary> Mode determined by parsing the query string </summary>
        [DataMember(EmitDefaultValue = false, Name = "mode")]
        [XmlAttribute("mode")]
        [ProtoMember(33)]
        public Display_Mode_Enum Mode { get; set; }

        /// <summary> mySobek submode for the current request.</summary>
        /// <remarks>The value returned is always lower case, and is also used for the admin pages</remarks>
        [DataMember(EmitDefaultValue = false, Name = "mySobekSubmode")]
        [XmlElement("mySobekSubmode")]
        [ProtoMember(34)]
        public string My_Sobek_SubMode { get; set; }

        /// <summary> mySobek type of display for the current request.</summary>
        [DataMember(EmitDefaultValue = false, Name = "mySobekType")]
        [XmlElement("mySobekType")]
        [ProtoMember(35)]
        public My_Sobek_Type_Enum My_Sobek_Type { get; set; }

        /// <summary> Page number to be displayed (either for an item or for results )</summary>
        [DataMember(EmitDefaultValue = false, Name = "page")]
        [XmlElement("page")]
        [ProtoMember(36)]
        public ushort? Page { get; set; }

        /// <summary> Filename indicated in the URL to allow direct link by page file name, rather than sequence </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageByFilename")]
        [XmlElement("pageByFilename")]
        [ProtoMember(37)]
        public string Page_By_FileName { get; set; }

        /// <summary> Gets the PURL associated with this portal which should be used for building permanent links for items </summary>
        [DataMember(EmitDefaultValue = false, Name = "portalPurl")]
        [XmlElement("portalPurl")]
        [ProtoMember(38)]
        public string Portal_PURL { get; set; }

        /// <summary> If this found a web content redirect in the system, the URL that this should be redirected to </summary>
        [DataMember(EmitDefaultValue = false, Name = "redirect")]
        [XmlElement("redirect")]
        [ProtoMember(63)]
        public string Redirect { get; set; }

		/// <summary> Name of the report requested from the reporting module </summary>
        [DataMember(EmitDefaultValue = false, Name = "reportName")]
        [XmlElement("reportName")]
        [ProtoMember(39)]
		public string Report_Name { get; set; }

        /// <summary> Submode for the results display </summary>
        [DataMember(EmitDefaultValue = false, Name = "resultType")]
        [XmlElement("resultType")]
        [ProtoMember(40)]
        public Result_Display_Type_Enum Result_Display_Type { get; set; }

        /// <summary> Return url value from the url string </summary>
        /// <remarks>This is primarily used by the mySobek feature, to return a user to their previously
        /// requested site, once they log on.</remarks>
        [DataMember(EmitDefaultValue = false, Name = "returnUri")]
        [XmlElement("returnUri")]
        [ProtoMember(41)]
        public string Return_URL { get; set; }

        /// <summary> Flag indicates if the request was completed, so no further
        /// operations should occur </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public bool Request_Completed { get; set; }

        /// <summary> Search fields </summary>
        [DataMember(Name = "searchFields")]
        [XmlElement("searchFields")]
        [ProtoMember(43)]
        public string Search_Fields
        {
            get { return searchFields ?? String.Empty; }
            set { searchFields = value; }
        }

        /// <summary> Precision to be used while performing a metadata search in the database </summary>
        [DataMember(EmitDefaultValue = false, Name = "searchPrecision")]
        [XmlElement("searchPrecision")]
        [ProtoMember(44)]
        public Search_Precision_Type_Enum Search_Precision { get; set; }

        /// <summary> Search string </summary>
        [DataMember(EmitDefaultValue = false, Name = "searchString")]
        [XmlElement("searchString")]
        [ProtoMember(45)]
        public string Search_String
        {
            get { return searchString ?? String.Empty; }
            set { searchString = value; }
        }

        /// <summary> Submode for searching </summary>
        [DataMember(EmitDefaultValue = false, Name = "searchType")]
        [XmlElement("searchType")]
        [ProtoMember(46)]
        public Search_Type_Enum Search_Type { get; set; }

        /// <summary> Flag which determines if the selection panel is shown
        /// for an aggregation search </summary>
        [DataMember(EmitDefaultValue = false, Name = "showSelectionPanel")]
        [XmlElement("showSelectionPanel")]
        [ProtoMember(47)]
        public bool? Show_Selection_Panel { get; set; }

        /// <summary> Size of Thumbnails to appear in the related items viewer </summary>
        [DataMember(EmitDefaultValue = false, Name = "thumbnailSize")]
        [XmlElement("thumbnailSize")]
        [ProtoMember(48)]
        public short? Size_Of_Thumbnails { get; set; }

        /// <summary> Current skin for display purposes </summary>
        /// <remarks>The value returned is always lower case</remarks>
        [DataMember(EmitDefaultValue = false, Name = "skin")]
        [XmlElement("skin")]
        [ProtoMember(49)]
        public string Skin { get; set; }

        /// <summary> Flag which indicates the interface is indicated in the URL and query string </summary>
        [DataMember(EmitDefaultValue = false, Name = "skinInUrl")]
        [XmlElement("skinInUrl")]
        [ProtoMember(50)]
        public bool Skin_In_URL { get; set; }

        /// <summary> Sort type employed for displaying result sets </summary>
        [DataMember(EmitDefaultValue = false, Name = "sort")]
        [XmlElement("sort")]
        [ProtoMember(51)]
        public short? Sort { get; set; }

        /// <summary> Submode for the statistics pages </summary>
        [DataMember(EmitDefaultValue = false, Name = "statsType")]
        [XmlElement("statsType")]
        [ProtoMember(52)]
        public Statistics_Type_Enum Statistics_Type { get; set; }

        /// <summary> Information about which sub aggregationPermissions to include or exclude
        /// during a collection group search</summary>
        [DataMember(EmitDefaultValue = false, Name = "subAggregation")]
        [XmlElement("subAggregation")]
        [ProtoMember(53)]
        public string SubAggregation { get; set; }

        /// <summary> Sub page number to be displayed (either for an item or for results )</summary>
        [DataMember(EmitDefaultValue = false, Name = "subPage")]
        [XmlElement("subPage")]
        [ProtoMember(54)]
        public ushort? SubPage { get; set; }

        /// <summary> String to use for a single-item text search </summary>
        [DataMember(EmitDefaultValue = false, Name = "textSearch")]
        [XmlElement("textSearch")]
        [ProtoMember(55)]
        public string Text_Search { get; set; }

        /// <summary> Thumbnails per page to appear in the related images item viewer  </summary>
        [DataMember(EmitDefaultValue = false, Name = "thumbnailsPerPage")]
        [XmlElement("thumbnailsPerPage")]
        [ProtoMember(56)]
        public short? Thumbnails_Per_Page { get; set; }

        /// <summary> TOC Display flag, which indicates whether to display the table of contents in the item viewer </summary>
        [DataMember(EmitDefaultValue = false, Name = "tocDisplay")]
        [XmlElement("tocDisplay")]
        [ProtoMember(57)]
        public TOC_Display_Type_Enum TOC_Display { get; set; }


        /// <summary> Trace flag which indicates whether to display the trace route </summary>
        [DataMember(EmitDefaultValue = false, Name = "traceFlag")]
        [XmlElement("traceFlag")]
        [ProtoMember(58)]
        public Trace_Flag_Type_Enum Trace_Flag { get; set; }

        /// <summary> Simplified flag for displaying the trace route </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public bool Trace_Flag_Simple
        {
            get
            {
                return (Trace_Flag == Trace_Flag_Type_Enum.Explicit) || (Trace_Flag == Trace_Flag_Type_Enum.Implied);
            }
        }

	    /// <summary> Volume id to display </summary>
        [DataMember(EmitDefaultValue = false, Name = "vid")]
        [XmlElement("vid")]
        [ProtoMember(59)]
	    public string VID { get; set; }

	    /// <summary> Viewer code which indicates which viewer to use when displaying 
        /// a single item in the item writer.  </summary>
        /// <remarks>The value returned is always lower case</remarks>
        [DataMember(EmitDefaultValue = false, Name = "viewerCode")]
        [XmlElement("viewerCode")]
        [ProtoMember(60)]
        public string ViewerCode { get; set; }

        /// <summary> Primary key to the web content object selected </summary>
        [DataMember(EmitDefaultValue = false, Name = "webContentId")]
        [XmlElement("webContentId")]
        [ProtoMember(64)]
        public int? WebContentID { get; set; }

        /// <summary> Webcontent type of display for the current request </summary>
        [DataMember(EmitDefaultValue = false, Name = "webContentType")]
        [XmlElement("webContentType")]
        [ProtoMember(61)]
        public WebContent_Type_Enum WebContent_Type { get; set; }

        /// <summary> Writer type to be employed for rendering </summary>
        [DataMember(EmitDefaultValue = false, Name = "writerType")]
        [XmlElement("writerType")]
        [ProtoMember(62)]
        public Writer_Type_Enum Writer_Type { get; set; }

		#endregion

        #region Methods for XML serialization

        /// <summary> Method suppresses XML Serialization of the DateRange_Date1 property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeDateRange_Date1()
        {
            return DateRange_Date1 != null;
        }

        /// <summary> Method suppresses XML Serialization of the DateRange_Date2 property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeDateRange_Date2()
        {
            return DateRange_Date2 != null;
        }

        /// <summary> Method suppresses XML Serialization of the DateRange_Year1 property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeDateRange_Year1()
        {
            return DateRange_Year1 != null;
        }

        /// <summary> Method suppresses XML Serialization of the DateRange_Year2 property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeDateRange_Year2()
        {
            return DateRange_Year2 != null;
        }

        /// <summary> Method suppresses XML Serialization of the FolderID property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeFolderID()
        {
            return FolderID != null;
        }

        /// <summary> Method suppresses XML Serialization of the Invalid_Item property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeInvalid_Item()
        {
            return Invalid_Item != null;
        }

        /// <summary> Method suppresses XML Serialization of the ItemID_DEPRECATED property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeItemID_DEPRECATED()
        {
            return ItemID_DEPRECATED != null;
        }

        /// <summary> Method suppresses XML Serialization of the Page property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializePage()
        {
            return Page != null;
        }

        /// <summary> Method suppresses XML Serialization of the Show_Selection_Panel property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeShow_Selection_Panel()
        {
            return Show_Selection_Panel != null;
        }

        /// <summary> Method suppresses XML Serialization of the Size_Of_Thumbnails property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeSize_Of_Thumbnails()
        {
            return Size_Of_Thumbnails != null;
        }

        /// <summary> Method suppresses XML Serialization of the Sort property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeSort()
        {
            return Sort != null;
        }

        /// <summary> Method suppresses XML Serialization of the SubPage property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeSubPage()
        {
            return SubPage != null;
        }

        /// <summary> Method suppresses XML Serialization of the Thumbnails_Per_Page property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeThumbnails_Per_Page()
        {
            return Thumbnails_Per_Page != null;
        }

        /// <summary> Method suppresses XML Serialization of the WebContentID property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeWebContentID()
        {
            return WebContentID != null;
        }

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
		[IgnoreDataMember]
		public string Base_Design_URL
		{
			get { return Base_URL + "design/"; }
		}

		/// <summary> URL for the watermarks/icons under the design folder </summary>
		/// <value> [Base_URL] + 'design/wordmarks/' </value>
        [IgnoreDataMember]
		public string Watermarks_URL
		{
            get { return Base_URL + "design/wordmarks/"; }
		}

		#endregion

	}
}
