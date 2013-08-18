#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Library.Navigation
{
	/// <summary> The legacy query string analyzer is used to continue to support the last
    /// URL query string configuration. </summary>
    /// <remarks> This implements the <see cref="iSobekCM_QueryString_Analyzer"/> interface. <br /> <br />
    /// QueryString_Analyzer is a class which analyzes the query string
    /// passed to the web server along with the URL.  This determines which portion
    /// of the web application to display first, and allows users to cut and paste
    /// a particular search or map. </remarks>
	public class SobekCM_QueryString_Analyzer_Legacy : iSobekCM_QueryString_Analyzer
    {
	    #region Private members of this class

	    private string aggregation;
	    private string bibid;
	    private string coord;
	    private string currSkin;
	    private string error_message;

	    private string fields;

	    private string folderid;
	    private string item;
	    private string mode;
	    private string return_url;
	    private string search;

	    private string sort;

	    private string subaggregation;

	    private string text_search;

	    private string vid;

	    private string viewport_options, viewport_point;

	    #endregion

	    #region Constructor

	    /// <summary> Constructor for a new instance of the SobekCM_QueryString_Analyzer class </summary>
	    public SobekCM_QueryString_Analyzer_Legacy()
	    {
	        // Set all the strings to empty initially
	        mode = String.Empty;
	        aggregation = String.Empty;
	        subaggregation = String.Empty;
	        item = String.Empty;
	        bibid = String.Empty;
	        vid = String.Empty;
	        search = String.Empty;
	        fields = String.Empty;
	        sort = String.Empty;
	        text_search = String.Empty;
	        currSkin = String.Empty;
	        viewport_options = String.Empty;
	        viewport_point = String.Empty;
	        error_message = String.Empty;
	        coord = String.Empty;
	        folderid = String.Empty;
	        return_url = String.Empty;
	    }

	    #endregion

	    #region iSobekCM_QueryString_Analyzer Members

	    /// <summary> Parse the query and set the internal variables </summary>
        /// <param name="QueryString"> QueryString collection passed from the main page </param>
        /// <param name="navigator"> Navigation object to hold the mode information </param>
        /// <param name="Base_URL">Requested base URL (without query string, etc..)</param>
        /// <param name="User_Languages"> Languages preferred by user, per their browser settings </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections </param>
        /// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations</param>
        /// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library</param>
        /// <param name="URL_Portals"> List of all web portals into this system </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Parse_Query(NameValueCollection QueryString,
            SobekCM_Navigation_Object navigator,
            string Base_URL,
            string[] User_Languages,
            Aggregation_Code_Manager Code_Manager,
            Dictionary<string, string> Aggregation_Aliases,
            ref Item_Lookup_Object All_Items_Lookup,
            Portal_List URL_Portals,
            Custom_Tracer Tracer )
        {
            // This is the legacy URL reader, so this is not optimized as well as the new one.
            // Just make sure the item list is loaded here
            SobekCM_Database.Verify_Item_Lookup_Object(true, ref All_Items_Lookup, Tracer);

			// Set default mode to error
			navigator.Mode = Display_Mode_Enum.Error;

			// If this has 'verb' then this is an OAI-PMH request
			if ( QueryString["verb"] != null )
			{
                navigator.Writer_Type = Writer_Type_Enum.OAI;
				return;
			}

            // Is there a TOC state set?
            if (QueryString["toc"] != null)
            {
                if (QueryString["toc"] == "y")
                {
                    navigator.TOC_Display = TOC_Display_Type_Enum.Show;
                }
                else if ( QueryString["toc"] == "n" )
                {
                    navigator.TOC_Display = TOC_Display_Type_Enum.Hide;
                }
            }

			// Is there a language defined?  If so, load right into the navigator
			if ( QueryString["l"] != null )
			{
				if (( QueryString["l"].ToUpper() == "ES" ) || ( QueryString["l"].ToUpper() == "SP" ))
				{
					navigator.Language = Web_Language_Enum.Spanish;
				}
				if ( QueryString["l"].ToUpper() == "FR" )
				{
					navigator.Language = Web_Language_Enum.French;
				}
			}

            // If there is flag indicating to show the trace route, save it
            if (QueryString["trace"] != null)
            {
                navigator.Trace_Flag = QueryString["trace"].ToUpper() == "NO" ? Trace_Flag_Type_Enum.No : Trace_Flag_Type_Enum.Explicit;
            }
            else
            {
                navigator.Trace_Flag = Trace_Flag_Type_Enum.Unspecified;
            }
			
			// Collect all the values into the strings
			Collect_Strings( QueryString );

            // Get the valid URL Portal
            navigator.Default_Aggregation = "all";
            Portal urlPortal = URL_Portals.Get_Valid_Portal(Base_URL);
            navigator.SobekCM_Instance_Abbreviation = urlPortal.Abbreviation;
            if (urlPortal.Default_Aggregation.Length > 0)
            {
                navigator.Default_Aggregation = urlPortal.Default_Aggregation;
                navigator.Aggregation = urlPortal.Default_Aggregation;
            }
            if (urlPortal.Default_Web_Skin.Length > 0)
            {
                navigator.Default_Skin = urlPortal.Default_Web_Skin;
                navigator.Skin = urlPortal.Default_Web_Skin;
                navigator.Skin_in_URL = false;
            }

            // Save the interface
            if (currSkin.Length > 0)
            {
                navigator.Skin = currSkin.ToLower();
                navigator.Skin_in_URL = true;
            }

            // Get the return url if there was one
            if (return_url.Length > 0)
            {
                navigator.Return_URL = HttpUtility.UrlDecode(return_url);
            }

			// If there are no values in the query string, the
			// current mode should be the default
			if (( QueryString.Keys.Count == 0 ) && ( aggregation.Length == 0 ))
			{
                navigator.Mode = Display_Mode_Enum.Aggregation_Home;
                navigator.Home_Type = Home_Type_Enum.List;
				return;
			}

            char default_html = 'h';
            if (HttpContext.Current != null)
            {
                if ((HttpContext.Current.Session["user"] != null) || (HttpContext.Current.Request.Cookies["UfdcUser"] != null))
                    default_html = 'l';
            }

			// If there is no mode listed, just set it to 'h' for home
			if (string.IsNullOrEmpty(mode))
			{
				if ( search.Length > 0 )
				{
					mode = default_html + "sb";
				}
				else
				{
					if (( item.Length > 0 ) || ( bibid.Length > 0 ))
					{
                        mode = default_html + "d";
					}
					else
					{
                        mode = default_html + "hl";
					}
				}
			}

			// If the length is insufficient, try adding 'h' in front
			if ( mode.Length == 1 )
                mode = default_html + mode;

			// Check for writer mode
			switch( mode[0] )
			{
				case 'x':
                    navigator.Writer_Type = Writer_Type_Enum.XML;
					break;

				case 'd':
                    navigator.Writer_Type = Writer_Type_Enum.DataSet;
					break;

				case 't':
                    navigator.Writer_Type = Writer_Type_Enum.Text;
					break;

                case 'j':
                    navigator.Writer_Type = Writer_Type_Enum.JSON;
                    break;

                case 'l':
                    navigator.Writer_Type = Writer_Type_Enum.HTML_LoggedIn;
                    break;

                default:
                    navigator.Writer_Type = Writer_Type_Enum.HTML;
                    break;
			}

            // Save the aggregation information
            navigator.Aggregation = aggregation;
            navigator.SubAggregation = subaggregation;

			// Perform switch, based on the mode
			switch ( mode[1] )
			{
                case 'a':
                    Admin_Block(navigator);
                    return;

				case 'b':
					navigator.Mode = Display_Mode_Enum.Aggregation_Browse_Info;
					Browse_Info_Block( navigator );
					return;

                case 'c':
                    if ((mode == "hcs") || ( mode == "lcs" ))
                    {
                        navigator.Mode = Display_Mode_Enum.Contact_Sent;
                    }
                    else
                    {
                        navigator.Mode = Display_Mode_Enum.Contact;
                        navigator.Error_Message = error_message;
                    }
                    navigator.Aggregation = aggregation;
                    return;

                case 'd':
                    Item_Display_Block(navigator);
                    return;

                case 'e':
                    Text_Display(navigator);
                    return;

                case 'f':
                    Item_Print_Block(navigator);
                    return;

                case 'g':
                    Public_Folder_Block(navigator);
                    break;

				case 'h':
					Home_Block( navigator );
					return;

                case 'i':
                    navigator.Mode = Display_Mode_Enum.Aggregation_Browse_Info;
                    Browse_Info_Block(navigator);
                    return;

                case 'm':
                    My_Sobek_Block(navigator);
                    return;

                case 'p':
                    navigator.Mode = Display_Mode_Enum.Preferences;
                    return;

                case 'r':
                    Results_Block(navigator);
                    return;

				case 's':    // Performing a basic search
					Search_Block( navigator );
					return;

                case 't':
                    Statistics_Block(navigator);
                    return;

                case 'z':
                    navigator.Mode = Display_Mode_Enum.Reset;
                    return;

				default:
					navigator.Mode = Display_Mode_Enum.Error;
                    navigator.Error_Message = "Unknown mode '" + mode + "'";
					return;
			}
		}

	    #endregion

	    /// <summary> Collects the strings from the query string and populates the temporary
        /// string values in this class, prior to transferring to the navigation object </summary>
        /// <param name="QueryString"> Collection from the URL query string </param>
		private void Collect_Strings( NameValueCollection QueryString )
		{
			// Collect the mode string
			if ( QueryString["m"] != null )
				mode = QueryString["m"].ToLower();

            // Collect any aggregation information
            if (QueryString["a"] != null)
            {
                aggregation = HttpUtility.HtmlEncode(QueryString["a"].ToLower().Replace("'", ""));
                if (QueryString["s"] != null)
                    subaggregation = HttpUtility.HtmlEncode(QueryString["s"].ToLower().Replace("'", ""));
            }
            else
            {
                // CHECK FOR LEGACY VALUES 

                // Collect any institution listed
                if (QueryString["h"] != null)
                {
                    aggregation = "i" + HttpUtility.HtmlEncode(QueryString["h"].ToLower().Replace("'", ""));
                }
                else
                {
                    string group = String.Empty;
                    string collection = String.Empty;
                    string subcollection = String.Empty;

                    // Collect any group listed
                    if (QueryString["g"] != null)
                    {
                        group = HttpUtility.HtmlEncode(QueryString["g"].ToLower().Replace("'", ""));
                    }
                    
                    // Collect any collection listed
                    if (QueryString["c"] != null)
                    {
                        collection = HttpUtility.HtmlEncode(QueryString["c"].ToLower().Replace("'", ""));
                    }
                    
                    // Collect any subcollection listed
                    if (QueryString["s"] != null)
                    {
                        subcollection = HttpUtility.HtmlEncode(QueryString["s"].ToLower().Replace("'", ""));
                    }

                    if (( subcollection.Length > 0 ) && ( subcollection[0] != '.'))
                    {
                        aggregation = subcollection;
                    }
                    else
                    {
                        if (( collection.Length > 0 ) && ( collection[0] != '.' ))
                        {
                            aggregation = collection;
                            if (( subcollection.Length > 0 ) && ( subcollection[0] == '.' ))
                                subaggregation = subcollection;
                        }
                        else
                        {
                            if (( group.Length > 0 ) && ( group[0] != '.' ))
                            {
                                aggregation = group;
                                if (( collection.Length > 0 ) && ( collection[0] == '.' ))
                                    subaggregation = collection;
                            }
                        }
                    }
                }
            }

            //  If MAP was selected as aggregation, make it MAPS
            if (aggregation.ToLower() == "map")
                aggregation = "maps";

			// Collect any item information
			if ( QueryString["i"] != null )
                item = HttpUtility.HtmlEncode(QueryString["i"].Replace("'", ""));
			else if ( QueryString["item"] != null )
                item = HttpUtility.HtmlEncode(QueryString["item"].Replace("'", ""));

			// Collect any bib id information
			if ( QueryString["b"] != null )
                bibid = HttpUtility.HtmlEncode(QueryString["b"].ToUpper().Replace("'", ""));
			else if ( QueryString["bib"] != null )
                bibid = HttpUtility.HtmlEncode(QueryString["bib"].ToUpper().Replace("'", ""));

			// Collect any vid information
			if ( QueryString["v"] != null )
                vid = HttpUtility.HtmlEncode(QueryString["v"].Replace("'", ""));
			else if ( QueryString["vid"] != null )
                vid = HttpUtility.HtmlEncode(QueryString["vid"].Replace("'", ""));
            if ((vid.Length > 0) && (vid.Length < 5))
            {
                vid = vid.PadLeft(5, '0');
            }

			// Collect any search value
			if ( QueryString["t"] != null )
				search = QueryString["t"];

			// Collect any fields value
			if ( QueryString["f"] != null )
				fields = QueryString["f"];

			// Collect any sort value
			if ( QueryString["o"] != null )
				sort = QueryString["o"];

			// Collect the text search string
			if ( QueryString["search"] != null )
                text_search = QueryString["search"];

			// Collect the interface string
			if ( QueryString["n"] != null )
                currSkin = HttpUtility.HtmlEncode(QueryString["n"].ToLower().Replace("'", ""));
			else if ( QueryString["int"] != null )
                currSkin = HttpUtility.HtmlEncode(QueryString["int"].ToLower().Replace("'", ""));

			// Collect the viewport options
			if ( QueryString["vo"] != null )
				viewport_options = QueryString["vo"];

			// Collect the viewport_point
			if ( QueryString["vp"] != null )
				viewport_point = QueryString["vp"];

            // Collects any error message
            if (QueryString["em"] != null)
                error_message = QueryString["em"];

            // Collect the coordinate information from the URL
			if ( QueryString["coord"] != null )
				coord = QueryString["coord"];

            // Collects the folder id information
            if (QueryString["fid"] != null)
                folderid = QueryString["fid"];

            // Collect the mode string
            if (QueryString["return"] != null)
                return_url = QueryString["return"];
		}

	    /// <summary> Method checks to see if this string contains only numbers </summary>
        /// <param name="test_string"> string to check for all numerals </param>
        /// <returns> TRUE if the string is made of all numerals </returns>
        /// <remarks> This just steps through each character in the string and tests with the Char.IsNumber method</remarks>
        private static bool is_String_Number(string test_string)
	    {
	        // Step through each character and return false if not a number
	        return test_string.All(Char.IsNumber);
	    }

	    #region Private methods for handling each different mode type

	    private void Public_Folder_Block(SobekCM_Navigation_Object navigator)
	    {
	        // Set the mode first
	        navigator.Mode = Display_Mode_Enum.Public_Folder;
	        try
	        {
	            navigator.FolderID = Convert.ToInt32(folderid);
	        }
	        catch
	        {
	            navigator.FolderID = -1;
	        }
	        navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;

	        if (mode.Length > 2)
	        {
	            switch (mode[2])
	            {
	                case 't':
	                    navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
	                    break;

	                case 'b':
	                    navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
	                    break;

	                case 'h':
	                    navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
	                    break;

	                case 'm':
	                    navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
	                    break;

	                case 'f':
	                    navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
	                    break;

	                case 'g':
	                    navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
	                    break;

	                case 'i':
	                    navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
	                    break;
	            }
	        }

	        // Look for the first number
	        navigator.Page = 1;
	        if (mode.Length > 3)
	        {
	            try
	            {
	                navigator.Page = Convert.ToUInt16(mode.Substring(3));
	            }
	            catch
	            {

	            }
	        }
	    }

	    private void My_Sobek_Block(SobekCM_Navigation_Object navigator)
	    {
	        // Set the mode first
	        navigator.Mode = Display_Mode_Enum.My_Sobek;

	        if (mode.Length < 3)
	        {
	            navigator.My_Sobek_Type = My_Sobek_Type_Enum.Home;
	        }
	        else
	        {
	            if (mode.Length > 3)
	            {
	                navigator.My_Sobek_SubMode = mode.Substring(3);
	            }

	            switch (mode[2])
	            {
                   
	                case 'l':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
	                    break;

	                case 'h':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.Home;
	                    break;

	                case 'n':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.New_Item;
	                    break;

	                case 'e':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;

	                    // If the item is -1, make sure there is at least a bib
	                    if (bibid.Length == 0)
	                    {
	                        navigator.Mode = Display_Mode_Enum.Error;
	                        navigator.Error_Message = "Missing item information (item id or bibid is required)";
	                        return;
	                    }

	                    // Save the item, bib, and vid to the navigator as well
	                    navigator.BibID = bibid;
	                    navigator.VID = vid;
	                    break;

	                case 'f':
	                    navigator.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
	                    if (mode.Length > 3)
	                    {
	                        // Get the display type
	                        switch (mode[3])
	                        {
	                            case 'l':
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
	                                break;

	                            case 't':
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
	                                break;

	                            case 'b':
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
	                                break;

	                            case 'h':
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
	                                break;

	                            case 'm':
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
	                                break;

	                            case 'f':
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
	                                break;

	                            case 'g':
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
	                                break;

	                            case 'i':
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
	                                break;

	                            default:
	                                navigator.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
	                                break;
	                        }
	                    }

	                    if (mode.Length > 4)
	                    {
	                        navigator.My_Sobek_SubMode = mode.Substring(4);

	                        // Try to get a page number
	                        navigator.Page = 0;
	                        try
	                        {
	                            navigator.Page = Convert.ToUInt16(viewport_point);
	                        }
	                        catch
	                        {
	                            navigator.Page = 0;
	                        }
	                        if (navigator.Page == 0)
	                            navigator.Page = 1;
	                    }
	                    else
	                    {
	                        navigator.My_Sobek_SubMode = String.Empty;
	                    }
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
	                    break;

	                case 'p':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
	                    break;

	                case 'o':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.Log_Out;
	                    break;

	                case 'g':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.Shibboleth_Landing;
	                    break;

	                case 's':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
	                    break;

	                case 't':
	                    navigator.My_Sobek_Type = My_Sobek_Type_Enum.User_Tags;
	                    break;
	            }
	        }
	    }

	    private void Admin_Block( SobekCM_Navigation_Object navigator )
	    {
	        // Set the mode first
	        navigator.Mode = Display_Mode_Enum.Internal;

	        if ( mode.Length < 3 )
	        {
	            navigator.Internal_Type = Internal_Type_Enum.Aggregations;
	        }
	        else
	        {
	            if (mode.Length > 3)
	            {
	                navigator.Info_Browse_Mode = mode.Substring(3);
	            }

	            switch ( mode[2] )
	            {
	                case 'c':
	                    navigator.Internal_Type = Internal_Type_Enum.Cache;
	                    break;
	                case 'b':
	                    navigator.Internal_Type = Internal_Type_Enum.Aggregations;
	                    break;
	                case 'n':
	                    navigator.Internal_Type = Internal_Type_Enum.New_Items;
	                    break;
	                case 'g':
	                    navigator.Internal_Type = Internal_Type_Enum.Aggregations;
	                    break;
	                case 's':
	                    navigator.Internal_Type = Internal_Type_Enum.Aggregations;
	                    break;
	                case 'l':
	                    navigator.Internal_Type = Internal_Type_Enum.Build_Failures;
	                    break;
	                case 'w':
	                    navigator.Internal_Type = Internal_Type_Enum.Wordmarks;
	                    break;
	                default:
	                    navigator.Internal_Type = Internal_Type_Enum.Aggregations;
	                    break;
	            }
	        }
	    }

	    private void Statistics_Block( SobekCM_Navigation_Object navigator )
	    {
	        // Set the mode first
	        navigator.Mode = Display_Mode_Enum.Statistics;

	        if ( mode.Length < 3 )
	        {
	            navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
	        }
	        else
	        {
	            if (mode.Length > 3)
	            {
	                navigator.Info_Browse_Mode = mode.Substring(3);
	            }

	            switch ( mode[2] )
	            {
	                case 'i':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
	                    break;
	                case 'r':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Recent_Searches;
	                    break;
	                case 'g':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Growth_View;
	                    break;
	                case 'u':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Usage_Overall;
	                    break;
	                case 'h':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collection_History;
	                    break;
	                case 'c':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collections_By_Date;
	                    break;
	                case 'd':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Usage_Definitions;
	                    break;
	                case 'j':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Usage_Item_Views_By_Date;
	                    break;
	                case 'w':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Usage_Items_By_Collection;
	                    break;
	                case 'x':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Usage_By_Date_Text;
	                    break;
	                case 'y':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Text;
	                    break;
	                case 'z':
	                    navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collection_History_Text;
	                    break;
	                default:
	                    navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
	                    break;
	            }
	        }
	    }

	    private void Text_Display(SobekCM_Navigation_Object navigator)
	    {
	        // Set the mode first
	        navigator.Mode = Display_Mode_Enum.Simple_HTML_CMS;

	        // There must be a submode for this feature
	        if (mode.Length <= 2)
	        {
	            navigator.Mode = Display_Mode_Enum.Error;
	            navigator.Error_Message = "Invalid browse/info mode '" + mode + "'";
	            return;
	        }

	        // Clear all collection hierarchy information
	        navigator.Aggregation = String.Empty;
	        navigator.SubAggregation = String.Empty;

	        string base_source = SobekCM_Library_Settings.Base_Directory + "design\\webcontent";
	        string source = base_source;
	        string possible_info_mode = mode.Substring(2).Replace("_", "/");
	        string filename = possible_info_mode;

	        if ((possible_info_mode.IndexOf("\\") > 0) || (possible_info_mode.IndexOf("/") > 0))
	        {
	            source = source + "\\" + possible_info_mode.Replace("/", "\\");
	            string[] split = source.Split("\\".ToCharArray());
	            filename = split[split.Length - 1];
	            source = source.Substring(0, source.Length - filename.Length);
	        }

	        string[] matching_file = Directory.GetFiles(source, filename + ".htm*");
	        if (matching_file.Length > 0)
	        {
	            navigator.Info_Browse_Mode = possible_info_mode;
	            navigator.Page_By_FileName = matching_file[0];
	        }
	        else
	        {
	            // This may point to the default html in the parent directory
	            if ((Directory.Exists(source + "\\" + filename)) && (File.Exists(source + "\\" + filename + "\\default.html")))
	            {
	                navigator.Info_Browse_Mode = possible_info_mode;
	                navigator.Page_By_FileName = source + "\\" + filename + "\\default.html";
	            }
	            else
	            {
	                if (navigator.Default_Aggregation == "all")
	                {
	                    navigator.Info_Browse_Mode = "default";
	                    navigator.Page_By_FileName = base_source + "\\default.html";
	                }
	            }
	        }         
	    }


	    private void Browse_Info_Block( SobekCM_Navigation_Object navigator )
	    {
	        // DO NOT SET THE MODE HERE FIRST, SINCE THIS COULD BE EITHER A BROWSE
	        // OR AN INFO REQUEST

	        // There must be a submode for this feature
	        if ( mode.Length <= 3 )
	        {
	            if (( mode == "xb" ) || ( mode == "tb" ) || ( mode == "db" ))
	                mode = mode + "tall";
	            else
	            {
	                navigator.Mode = Display_Mode_Enum.Error;
	                navigator.Error_Message = "Invalid browse/info mode '" + mode + "'";
	                return;
	            }
	        }

	        // Get the submode from the remainder of the mode
	        navigator.Info_Browse_Mode = mode.Substring(3);

	        // Look for the first number
	        int numberStart = -1;
	        int count = 0;
	        foreach( char thisChar in navigator.Info_Browse_Mode )
	        {
	            if ( Char.IsNumber( thisChar ))
	            {
	                numberStart = count;
	                break;
	            }
	            count++;
	        }

	        // Get the page
	        if ( numberStart > 0 )
	        {
	            navigator.Page = Convert.ToUInt16( navigator.Info_Browse_Mode.Substring( numberStart ));
	            navigator.Info_Browse_Mode = navigator.Info_Browse_Mode.Substring( 0, numberStart );
	        }
			

	        // Get the display type
	        navigator.Result_Display_Type = Result_Display_Type_Enum.Default;
	        if ( mode[2] == 't' )
	            navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
	        if ( mode[2] == 'b' )
	            navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
	        if ( mode[2] == 'h' )
	            navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
	        if (mode[2] == 'm')
	            navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
	        if (mode[2] == 'f')
	            navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
	        if (mode[2] == 'g')
	            navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
	        if (mode[2] == 'i')
	            navigator.Result_Display_Type = Result_Display_Type_Enum.Export;

	        // Save the sort value
	        navigator.Sort = 0;
	        if ((HttpContext.Current != null) && (HttpContext.Current.Session != null) && (HttpContext.Current.Session["User_Default_Sort"] != null))
	            navigator.Sort = Convert.ToInt16(HttpContext.Current.Session["User_Default_Sort"]);
	        if (( sort.Length > 0 ) && ( is_String_Number( sort )))
	        {
	            try
	            {
	                navigator.Sort = Convert.ToInt16( sort );
	            }
	            catch
	            {

	            }
	        }

	        // There must be an aggregation listed here, unless this is XML
	        if (( aggregation.Length == 0 ) && ( navigator.Info_Browse_Mode != "new" ))
	        {
	            if ((navigator.Mode == Display_Mode_Enum.Aggregation_Browse_Info) || (navigator.Writer_Type == Writer_Type_Enum.XML) || (navigator.Writer_Type == Writer_Type_Enum.DataSet) || (navigator.Writer_Type == Writer_Type_Enum.Text))
	            {
	                // This is okay since this will just pull EVERY ITEM from the database
	            }
	            else
	            {
	                navigator.Mode = Display_Mode_Enum.Error;
	                navigator.Error_Message = "You can not browse every item in this digital library.";
	                return;
	            }
	        }
	    }

	    private void Item_Print_Block(SobekCM_Navigation_Object navigator)
	    {
	        // Set this to display initially
	        navigator.Mode = Display_Mode_Enum.Item_Print;

	        // If the item has a value, check that it is a number

	        // If the item is -1, make sure there is at least a bib
	        if (bibid.Length == 0)
	        {
	            navigator.Mode = Display_Mode_Enum.Error;
	            navigator.Error_Message = "Missing item information (item id or bibid is required)";
	            return;
	        }

	        // Save the item, bib, and vid to the navigator as well
	        navigator.BibID = bibid;
	        navigator.VID = vid;

	        // Get the view code
	        if (mode.Length > 2)
	        {
	            // Get the viewer code
	            navigator.ViewerCode = mode.Substring(2).ToUpper();

	            // Get all the viewport options
	            if (viewport_options.Length > 0)
	            {
	                navigator.Viewport_Size = Convert.ToUInt16("0" + viewport_options[0]);

	                if (viewport_options.Length > 1)
	                {
	                    navigator.Viewport_Zoom = (ushort)(Convert.ToUInt16("0" + viewport_options[1]) + 1);
	                    if (navigator.Viewport_Zoom > 5)
	                        navigator.Viewport_Zoom = 5;
	                    if (navigator.Viewport_Zoom < 1)
	                        navigator.Viewport_Zoom = 1;

	                    if (viewport_options.Length > 2)
	                    {
	                        navigator.Viewport_Rotation = Convert.ToUInt16("0" + viewport_options[2]);
	                    }
	                }
	            }

	            // Get the viewport point
	            if ((viewport_point.Length > 0) && (viewport_point.IndexOf(",") > 0))
	            {
	                string[] split = viewport_point.Split(",".ToCharArray());
	                navigator.Viewport_Point_X = Convert.ToInt32(split[0]);
	                navigator.Viewport_Point_Y = Convert.ToInt32(split[1]);
	            }

	            // Sequence is set to 1
	            navigator.Page = 1;
	        }
	        else
	        {
	            // Sequence is set to 1
	            navigator.ViewerCode = "JJ1";
	            navigator.Page = 1;
	        }
	    }

	    private void Item_Display_Block( SobekCM_Navigation_Object navigator )
	    {
	        // Set this to display initially
	        navigator.Mode = Display_Mode_Enum.Item_Display;

	        // If the item is -1, make sure there is at least a bib
	        if (( bibid.Length == 0 ) && ( item.Length == 0 ))
	        {
	            navigator.Mode = Display_Mode_Enum.Error;
	            navigator.Error_Message = "Missing item information (item id or bibid is required)";
	            return;
	        }

	        // Save the item, bib, and vid to the navigator as well
	        navigator.BibID = bibid;
	        navigator.VID = vid;
	        if (item.Length > 0)
	        {
	            try
	            {
	                navigator.ItemID_DEPRECATED = Convert.ToInt32(item);
	            }
	            catch
	            {

	            }
	        }

	        // If coordinates were here, save them
	        if (coord.Length > 0)
	        {
	            navigator.Coordinates = coord;
	        }

	        // Save the text search and display values, if one exists
	        navigator.Text_Search = text_search;
		
	        // Trim off any subpage first
	        if (mode.IndexOf(".") > 0)
	        {
	            string[] splitter = mode.Split(".".ToCharArray());
	            mode = splitter[0];
	            try
	            {
	                if (splitter[1].Length > 0)
	                    navigator.SubPage = Convert.ToUInt16(splitter[1]);
	            }
	            catch { }
	        }

	        // Get the view code
	        if ( mode.Length > 2 )
	        {
	            // Get the viewer code
	            navigator.ViewerCode = mode.Substring( 2 );

	            // Get all the viewport options
	            if ( viewport_options.Length > 0 )
	            {
	                navigator.Viewport_Size = Convert.ToUInt16("0" + viewport_options[0]);
	
	                if ( viewport_options.Length > 1 )
	                {
	                    navigator.Viewport_Zoom = (ushort)(Convert.ToUInt16("0" + viewport_options[1]) + 1);
	                    if ( navigator.Viewport_Zoom > 5 )
	                        navigator.Viewport_Zoom = 5;
	                    if ( navigator.Viewport_Zoom < 1 )
	                        navigator.Viewport_Zoom = 1;

	                    if ( viewport_options.Length > 2 )
	                    {
	                        navigator.Viewport_Rotation = Convert.ToUInt16("0" + viewport_options[2]);
	                    }
	                }
	            }

	            // Get the viewport point
	            if (( viewport_point.Length > 0 ) && ( viewport_point.IndexOf(",") > 0 ))
	            {
	                string[] split = viewport_point.Split(",".ToCharArray());
	                navigator.Viewport_Point_X = Convert.ToInt32( split[0] );
	                navigator.Viewport_Point_Y = Convert.ToInt32( split[1] );
	            }

	            // Now, get the page
	            if (( navigator.ViewerCode.Length > 0 ) && ( Char.IsNumber( navigator.ViewerCode[0] )))
	            {
	                // Look for the first number
	                int numberEnd = navigator.ViewerCode.Length;
	                int count = 0;
	                foreach( char thisChar in navigator.ViewerCode )
	                {
	                    if ( !Char.IsNumber( thisChar ))
	                    {
	                        numberEnd = count;
	                        break;
	                    }
	                    count++;
	                }

	                // Get the page
	                navigator.Page = Convert.ToUInt16(navigator.ViewerCode.Substring(0, numberEnd));
	            }

	        }
	        else
	        {
	            // Sequence is set to 1
	            navigator.Page = 1;
	        }
	    }


	    private void Home_Block( SobekCM_Navigation_Object navigator )
	    {
	        // Save the group and collection values
	        navigator.Mode = Display_Mode_Enum.Aggregation_Home;
	        navigator.Home_Type = Home_Type_Enum.List;

	        if (mode.Length > 2)
	        {
	            switch (mode[2])
	            {
	                case 'h':
	                    navigator.Home_Type = Home_Type_Enum.List;
	                    break;

	                case 's':
	                    navigator.Home_Type = Home_Type_Enum.List;
	                    navigator.Show_Selection_Panel = true;
	                    break;

	                case 'd':
	                    navigator.Home_Type = Home_Type_Enum.Descriptions;
	                    break;

	                case 't':
	                    navigator.Home_Type = Home_Type_Enum.Tree_Collapsed;
	                    break;

	                case 'e':
	                    navigator.Home_Type = Home_Type_Enum.Tree_Expanded;
	                    break;

	                case 'i':
	                    navigator.Home_Type = Home_Type_Enum.Partners_List;
	                    break;

	                case 'j':
	                    navigator.Home_Type = Home_Type_Enum.Partners_Thumbnails;
	                    break;

	                case 'p':
	                    navigator.Home_Type = Home_Type_Enum.Personalized;
	                    break;
	            }
	        }

	        // Save the search information as well
	        if ( search.Length > 0 )
	            navigator.Search_String = search;
	    }

	    private void Search_Block( SobekCM_Navigation_Object navigator )
	    {
	        // Set the mode
	        navigator.Mode = Display_Mode_Enum.Search;

	        // Save the search information as well
	        if ( search.Length > 0 )
	            navigator.Search_String = search;

	        // Check the submode
	        navigator.Search_Type = Search_Type_Enum.Basic;
	        navigator.Search_Fields = "ZZ";

	        // Check to see if this is the advanced search
	        if ( mode.Length > 2 ) 
	        {
	            switch (mode[2])
	            {
	                case 'a':
	                    navigator.Search_Type = Search_Type_Enum.Advanced;
	                    navigator.Search_Fields = fields;
	                    break;

	                case 'b':
	                    navigator.Search_Type = Search_Type_Enum.Basic;
	                    navigator.Search_Fields = "ZZ";
	                    break;

	                case 'd':
	                    navigator.Search_Type = Search_Type_Enum.dLOC_Full_Text;
	                    navigator.Search_Fields = fields;
	                    break;

	                case 'f':
	                    navigator.Search_Type = Search_Type_Enum.Full_Text;
	                    navigator.Search_Fields = fields;
	                    break;

	                case 'm':
	                    navigator.Search_Type = Search_Type_Enum.Map;
	                    if (mode.Length > 3)
	                        navigator.Info_Browse_Mode = mode.Substring(3);
	                    navigator.Coordinates = search;
	                    break;

	                case 'n':
	                    navigator.Search_Type = Search_Type_Enum.Newspaper;
	                    break;

	            }
	        }

	        if ((mode.Length > 3) && (mode[3] == 's'))
	            navigator.Show_Selection_Panel = true;
               
	    }

	    private void Results_Block( SobekCM_Navigation_Object navigator )
	    {
	        // Set the mode
	        navigator.Mode = Display_Mode_Enum.Results;

	        // Save the search information as well
	        if ( search.Length > 0 )
	            navigator.Search_String = search;
	        if ( fields.Length > 0 )
	            navigator.Search_Fields = fields;

	        // Save the sort value
	        navigator.Sort = 0;
	        if ((HttpContext.Current != null) && (HttpContext.Current.Session != null) && (HttpContext.Current.Session["User_Default_Sort"] != null))
	            navigator.Sort = Convert.ToInt16(HttpContext.Current.Session["User_Default_Sort"]);

	        if (( sort.Length > 0 ) && ( is_String_Number( sort )))
	        {
	            try
	            {
	                navigator.Sort = Convert.ToInt16( sort );
	            }
	            catch
	            {

	            }
	        }

	        // Check the submode
	        navigator.Search_Type = Search_Type_Enum.Basic;
	        navigator.Search_Fields = "ZZ";

	        // Look for the first number
	        string modeTemp = mode;
	        int numberStart = -1;
	        int count = 0;
	        foreach( char thisChar in modeTemp )
	        {
	            if ( Char.IsNumber( thisChar ))
	            {
	                numberStart = count;
	                break;
	            }
	            count++;
	        }

	        // Get the page
	        if ( numberStart > 0 )
	        {
	            navigator.Page = Convert.ToUInt16( modeTemp.Substring( numberStart ));
	            mode = modeTemp.Substring( 0, numberStart );
	        }

	        // Check to see if this is the advanced search
	        if ( mode.Length > 2 )
	        {
	            switch (mode[2])
	            {
	                case 'a':
	                    navigator.Search_Type = Search_Type_Enum.Advanced;
	                    navigator.Search_Fields = fields;
	                    break;

	                case 'b':
	                    navigator.Search_Type = Search_Type_Enum.Basic;
	                    navigator.Search_Fields = "ZZ";
	                    break;

	                case 'f':
	                    navigator.Search_Type = Search_Type_Enum.Full_Text;
	                    navigator.Search_Fields = fields;
	                    break;

	                case 'm':
	                    navigator.Search_Type = Search_Type_Enum.Map;
	                    navigator.Search_Fields = fields;
	                    navigator.Coordinates = search;
	                    break;

	                case 'n':
	                    navigator.Search_Type = Search_Type_Enum.Newspaper;
	                    navigator.Search_Fields = fields;
	                    break;
	            }
	        }

	        // Get the display type
	        navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
	        if ( mode.Length > 3 )
	        {
	            if (mode[3] == 't')
	                navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
	            if ( mode[3] == 'b' )
	                navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
	            if ( mode[3] == 'h' )
	                navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
	            if (mode[3] == 'm')
	                navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
	            if (mode[3] == 'f')
	                navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
	            if (mode[3] == 'g')
	                navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
	            if (mode[3] == 'i')
	                navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
	        }
	    }

	    #endregion
    }
}
