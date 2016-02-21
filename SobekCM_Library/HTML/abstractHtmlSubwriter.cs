#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Core.Skins;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
	/// <summary> Abstract class which all HTML subwriters must extend.  This class contains some of the
	/// basic HTML-writing helper values and contains some of the values used by many of the subclasses.
	/// HTML subwriters are the top level writing classes employed by the <see cref="Html_MainWriter"/>. </summary>
	public abstract class abstractHtmlSubwriter
	{
        /// <summary> Protected field contains the information specific to the current request </summary>
        protected RequestCache RequestSpecificValues;

        /// <summary> Empty list of behaviors, returned by default </summary>
        /// <remarks> This just prevents an empty set from having to be created over and over </remarks>
        protected static List<HtmlSubwriter_Behaviors_Enum> emptybehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

        /// <summary> Base constructor </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
	    protected abstractHtmlSubwriter(RequestCache RequestSpecificValues)
	    {
	        this.RequestSpecificValues = RequestSpecificValues;
	    }

	    /// <summary> Adds the banner to the response stream from either the html web skin
	    /// or from the current item aggreagtion object, depending on flags in the web skin object </summary>
	    /// <param name="Output"> Stream to which to write the HTML for the banner </param>
	    /// <param name="Banner_Division_Name"> Name for the wrapper division around the banner </param>
	    /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
	    /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
	    /// <param name="Web_Page_Title"> Web page title to add behind the banner image </param>
	    /// <param name="CurrentMode"> Mode / navigation information for the current request</param>
	    /// <remarks> This is called by several html subwriters that otherwise tell this class to suppress writing the banner </remarks>
	    public static void Add_Banner(TextWriter Output, string Banner_Division_Name, string Web_Page_Title, Navigation_Object CurrentMode, Web_Skin_Object HTML_Skin, Item_Aggregation Hierarchy_Object)
		{
			Output.WriteLine("<!-- Write the main collection, interface, or institution banner -->");
            if ((HTML_Skin != null) && (HTML_Skin.Override_Banner.HasValue) && (HTML_Skin.Override_Banner.Value))
			{
                if ( !String.IsNullOrEmpty(HTML_Skin.Banner_HTML))
                    Output.WriteLine(HTML_Skin.Banner_HTML);
			}
			else
			{
				string url_options = UrlWriterHelper.URL_Options(CurrentMode);
				if (url_options.Length > 0)
					url_options = "?" + url_options;

				if ((Hierarchy_Object != null) && (Hierarchy_Object.Code != "all"))
				{
				    Output.WriteLine("<section id=\"sbkAhs_BannerDiv\" role=\"banner\" title=\"" + Hierarchy_Object.ShortName + "\">");
				    Output.WriteLine("  <h1 class=\"hidden-element\">" + Web_Page_Title + "</h1>");
				    Output.WriteLine("  <a alt=\"" + Hierarchy_Object.ShortName + "\" href=\"" + CurrentMode.Base_URL + Hierarchy_Object.Code + url_options + "\"><img id=\"mainBanner\" src=\"" + CurrentMode.Base_URL + Hierarchy_Object.Get_Banner_Image(HTML_Skin) + "\"  alt=\"" + Hierarchy_Object.ShortName + "\" /></a>");
                    Output.WriteLine("</section>");
				}
				else
				{
                    if ((Hierarchy_Object != null) && (Hierarchy_Object.Get_Banner_Image(HTML_Skin).Length > 0))
                    {
                        Output.WriteLine("<section id=\"sbkAhs_BannerDiv\" role=\"banner\" title=\"" + Hierarchy_Object.ShortName + "\">");
                        Output.WriteLine("  <h1 class=\"hidden-element\">" + Web_Page_Title + "</h1>");
                        Output.WriteLine("  <a alt=\"" + Hierarchy_Object.ShortName + "\" href=\"" + CurrentMode.Base_URL + url_options + "\"><img id=\"mainBanner\" src=\"" + CurrentMode.Base_URL + Hierarchy_Object.Get_Banner_Image(HTML_Skin) + "\"  alt=\"" + Hierarchy_Object.ShortName + "\" /></a>");
                        Output.WriteLine("</section>");
					}
					else
					{
						string skin_url = CurrentMode.Base_Design_URL + "skins/" + CurrentMode.Skin + "/";
                        Output.WriteLine("<section id=\"sbkAhs_BannerDiv\" role=\"banner\"><h1 class=\"hidden-element\">" + Web_Page_Title + "</h1><a href=\"" + CurrentMode.Base_URL + url_options + "\"><img id=\"mainBanner\" src=\"" + skin_url + "default.jpg\" alt=\"\" /></a></section>");
					}
				}
			}
			Output.WriteLine();
		}
        
	    /// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
		/// for the current request, or if it can be hidden. </summary>
		/// <value> This value can be override by child classes, but by default this returns FALSE </value>
		public virtual bool Upload_File_Possible
		{
			get { return false; }
		}

		/// <summary> Write any additional values within the HTML Head of the
		/// final served page </summary>
		/// <param name="Output"> Output stream currently within the HTML head tags </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <remarks> By default this does nothing, but can be overwritten by all the individual html subwriters </remarks>
		public virtual void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

        /// <summary> Chance for a final, final CSS which can override anything else, including the web skin </summary>
	    public virtual string Final_CSS
	    {
	        get { return String.Empty;  }
	    }

        /// <summary> Add the header to the output </summary>
        /// <param name="Output"> Stream to which to write the HTML for this header </param>
	    public virtual void Add_Header(TextWriter Output)
	    {
            HeaderFooter_Helper_HtmlSubWriter.Add_Header(Output, RequestSpecificValues, Container_CssClass, WebPage_Title, Subwriter_Behaviors, null, null);
	    }



        /// <summary> Flag indicates if the internal header should included </summary>
        /// <remarks> By default this return TRUE if the user is internal, or a portal/system admin, but can be 
        /// overwritten by all the individual html subwriters </remarks>
	    public virtual bool Include_Internal_Header
	    {
            get
            {
                // If no user, do not show
                if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
                    return false;

                return ((RequestSpecificValues.Current_User.Is_Internal_User) || (RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin));
            }
	    }

		/// <summary> Adds the internal header HTML for this specific HTML writer </summary>
		/// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
		/// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
		public virtual void Write_Internal_Header_HTML(TextWriter Output, User_Object Current_User)
		{
			Output.WriteLine("  <table id=\"sbk_InternalHeader\">");
			Output.WriteLine("    <tr>");
			Output.WriteLine("      <td style=\"text-align:left;\">");
			Output.WriteLine("          <button title=\"Hide Internal Header\" class=\"intheader_button_aggr hide_intheader_button_aggr\" onclick=\"return hide_internal_header();\"></button>");
			Output.WriteLine("      </td>");
			Write_Internal_Header_Search_Box(Output);
			Output.WriteLine("    </tr>");
			Output.WriteLine("  </table>");
		}

		/// <summary> Adds the internal header search box to the current output stream  </summary>
		/// <param name="Output"> Output stream to write the html for the internal header search box to </param>
		protected void Write_Internal_Header_Search_Box(TextWriter Output)
		{
			Output.WriteLine("      <td style=\"text-align:right; vertical-align:middle; width:340px;\">");
			Output.WriteLine("        <table>");
			Output.WriteLine("          <tr style=\"vertical-align:top; height: 16px;\">");
			Output.WriteLine("            <td valign=\"top\">");
			Output.Write("              <input name=\"internalSearchTextBox\" type=\"text\" id=\"internalSearchTextBox\" class=\"SobekInternalSearchBox\" value=\"\" onfocus=\"javascript:textbox_enter('internalSearchTextBox', 'SobekInternalSearchBox_focused')\" onblur=\"javascript:textbox_leave('internalSearchTextBox', 'SobekInternalSearchBox')\"");
            if (( !String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Browser_Type)) && ( RequestSpecificValues.Current_Mode.Browser_Type.IndexOf("IE") >= 0))
                Output.WriteLine(" onkeydown=\"internalTrapKD(event, '" + RequestSpecificValues.Current_Mode.Base_URL + "contains');\" />");
			else
                Output.WriteLine(" onkeydown=\"return internalTrapKD(event, '" + RequestSpecificValues.Current_Mode.Base_URL + "contains');\" />");
			Output.WriteLine("              <select name=\"internalDropDownList\" id=\"internalDropDownList\" class=\"SobekInternalSelectBox\" >");
			Output.WriteLine("                <option value=\"BI\" selected=\"selected\">BibID</option>");
			Output.WriteLine("                <option value=\"OC\">OCLC Number</option>");
			Output.WriteLine("                <option value=\"AL\">ALEPH Number</option>");
			Output.WriteLine("                <option value=\"ZZ\">Anywhere</option>");
			Output.WriteLine("                <option value=\"TI\">Title</option>");
			Output.WriteLine("                <option value=\"AU\">Author</option>");
			Output.WriteLine("                <option value=\"SU\">Subject Keywords</option>");
			Output.WriteLine("                <option value=\"CO\">Country</option>");
			Output.WriteLine("                <option value=\"ST\">State</option>");
			Output.WriteLine("                <option value=\"CT\">County</option>");
			Output.WriteLine("                <option value=\"CI\">City</option>");
			Output.WriteLine("                <option value=\"PP\">Place of Publication</option>");
			Output.WriteLine("                <option value=\"SP\">Spatial Coverage</option>");
			Output.WriteLine("                <option value=\"TY\">Type</option>");
			Output.WriteLine("                <option value=\"LA\">Language</option>");
			Output.WriteLine("                <option value=\"PU\">Publisher</option>");
			Output.WriteLine("                <option value=\"GE\">Genre</option>");
			Output.WriteLine("                <option value=\"TA\">Target Audience</option>");
			Output.WriteLine("                <option value=\"DO\">Donor</option>");
			Output.WriteLine("                <option value=\"AT\">Attribution</option>");
			Output.WriteLine("                <option value=\"TL\">Tickler</option>");
			Output.WriteLine("                <option value=\"NO\">Notes</option>");
			Output.WriteLine("                <option value=\"ID\">Identifier</option>");
			Output.WriteLine("                <option value=\"FR\">Frequency</option>");
			Output.WriteLine("                <option value=\"TB\">Tracking Box</option>");
			Output.WriteLine("              </select>");
			Output.WriteLine("            </td>");
			Output.WriteLine("            <td>");
            Output.WriteLine("              <a onclick=\"internal_search('" + RequestSpecificValues.Current_Mode.Base_URL + "contains')\"><img src=\"" + Static_Resources.Go_Gray_Gif + "\" title=\"Perform search\" alt=\"Perform search\" style=\"margin-top: 1px\" /></a>");
			Output.WriteLine("              &nbsp;");
			Output.WriteLine("            </td>");
			Output.WriteLine("          </tr>");
			Output.WriteLine("        </table>");
			Output.WriteLine("      </td> ");
		}


		/// <summary> Writes the HTML generated by this abstract html subwriter directly to the response stream </summary>
		/// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
		public abstract bool Write_HTML(TextWriter Output, Custom_Tracer Tracer);

		/// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public virtual void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

		/// <summary> Writes additional HTML to the output stream just before the main place holder but after the TocPlaceHolder in the itemNavForm form.  </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public virtual void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

		/// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
		/// <param name="Output"> Stream to which to write the text for this main writer </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public virtual void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

		/// <summary> Writes final HTML after all the forms </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public virtual void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

        /// <summary> Add the footer to the output </summary>
        /// <param name="Output"> Stream to which to write the HTML for this footer </param>
        public virtual void Add_Footer(TextWriter Output)
        {
            HeaderFooter_Helper_HtmlSubWriter.Add_Footer(Output, RequestSpecificValues, Subwriter_Behaviors, null, null);
        }

		/// <summary> Gets the collection of special behaviors which this subwriter
		/// requests from the main HTML writer. </summary>
		/// <remarks> By default, this returns an empty list </remarks>
		public virtual List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
		{
			get { return emptybehaviors; }
		}

		/// <summary> Gets the collection of body attributes to be included 
		/// within the HTML body tag (usually to add events to the body) </summary>
		public virtual List<Tuple<string, string>> Body_Attributes
		{
			get { return null; }
		}

		/// <summary> Title for this web page </summary>
		/// <value> This value is set by each of the sub classes </value>
		public virtual string WebPage_Title
		{
			get { return "{0}"; }
		}

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		/// <value> By default, returns 'container-inner' </value>
		public virtual string Container_CssClass
		{
			get { return "container-inner"; }
		}

        #region Helper methods for getting collections and itemss

        /// <summary> Gets the item aggregation and search fields for the current item aggregation </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Aggregation_Object"> [OUT] Fully-built object for the current aggregation object </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This attempts to pull the objects from the cache.  If unsuccessful, it builds the objects from the
        /// database and hands off to the <see cref="CachedDataManager" /> to store in the cache. </remarks>
        protected static bool Get_Collection(Navigation_Object Current_Mode, Custom_Tracer Tracer, out Item_Aggregation Aggregation_Object)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstractHtmlSubwriter.Get_Collection", String.Empty);
            }

            string languageCode = Web_Language_Enum_Converter.Enum_To_Code(Current_Mode.Language);

            // If there is an aggregation listed, try to get that now
            if ((Current_Mode.Aggregation.Length > 0) && (Current_Mode.Aggregation != "all"))
            {
                // Try to pull the aggregation information
                Aggregation_Object = CachedDataManager.Aggregations.Retrieve_Item_Aggregation(Current_Mode.Aggregation, Web_Language_Enum_Converter.Code_To_Enum(languageCode), Tracer);
                if (Aggregation_Object != null)
                    return true;

                // Get the item aggregation from the Sobek Engine Client
                Aggregation_Object = SobekEngineClient.Aggregations.Get_Aggregation(Current_Mode.Aggregation, Web_Language_Enum_Converter.Code_To_Enum(languageCode), UI_ApplicationCache_Gateway.Settings.System.Default_UI_Language, Tracer);

                // Return if this was valid
                if (Aggregation_Object != null)
                {
                    if ((Current_Mode.Skin_In_URL != true) && (!String.IsNullOrEmpty(Aggregation_Object.Default_Skin)))
                    {
                        Current_Mode.Skin = Aggregation_Object.Default_Skin.ToLower();
                    }
                    return true;
                }

                Current_Mode.Error_Message = "Invalid item aggregation '" + Current_Mode.Aggregation + "' referenced.";
                return false;
            }

            return Get_Top_Level_Collection(Current_Mode, Tracer, out Aggregation_Object);
        }

        /// <summary> Gets the item aggregation and search fields for the current item aggregation </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Aggregation_Object"> [OUT] Fully-built object for the current aggregation object </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This attempts to pull the objects from the cache.  If unsuccessful, it builds the objects from the
        /// database and hands off to the <see cref="CachedDataManager" /> to store in the cache. </remarks>
        protected static bool Get_Top_Level_Collection(Navigation_Object Current_Mode, Custom_Tracer Tracer, out Item_Aggregation Aggregation_Object)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstractHtmlSubwriter.Get_Top_Level_Collection", String.Empty);
            }

            string languageCode = Web_Language_Enum_Converter.Enum_To_Code(Current_Mode.Language);

            // Get the ALL collection group
            try
            {
                // Try to pull this from the cache
                Aggregation_Object = CachedDataManager.Aggregations.Retrieve_Item_Aggregation("all", Web_Language_Enum_Converter.Code_To_Enum(languageCode), Tracer);
                if (Aggregation_Object != null)
                    return true;

                // Get the item aggregation from the Sobek Engine Client
                Aggregation_Object = SobekEngineClient.Aggregations.Get_Aggregation("all", Web_Language_Enum_Converter.Code_To_Enum(languageCode), UI_ApplicationCache_Gateway.Settings.System.Default_UI_Language, Tracer);
            }
            catch (Exception ee)
            {
                Aggregation_Object = null;
                Current_Mode.Error_Message = "Error pulling the item aggregation corresponding to all collection groups : " + ee.Message;
                return false;
            }

            // If this is null, just stop
            if (Aggregation_Object == null)
            {
                Current_Mode.Error_Message = "Unable to pull the item aggregation corresponding to all collection groups";
                return false;
            }

            return true;
        }

        /// <summary> Gets the browse or info object and any other needed data for display ( text to display) </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Aggregation_Object"> Item Aggregation object</param>
        /// <param name="Base_Directory"> Base directory location under which the the CMS/info source file will be found</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Browse_Object"> [OUT] Stores all the information about this browse or info </param>
        /// <param name="Complete_Result_Set_Info"> [OUT] Information about the entire set of results </param>
        /// <param name="Paged_Results"> [OUT] List of search results for the requested page of results </param>
        /// <param name="Browse_Info_Display_Text"> [OUT] Static HTML-based content to be displayed if this is browing a staticly created html source file </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This attempts to pull the objects from the cache.  If unsuccessful, it builds the objects from the
        /// database and hands off to the <see cref="CachedDataManager" /> to store in the cache </remarks>
        protected static bool Get_Browse_Info(Navigation_Object Current_Mode,
                                    Item_Aggregation Aggregation_Object,
                                    string Base_Directory,
                                    Custom_Tracer Tracer,
                                    out Item_Aggregation_Child_Page Browse_Object,
                                    out Search_Results_Statistics Complete_Result_Set_Info,
                                    out List<iSearch_Title_Result> Paged_Results,
                                    out HTML_Based_Content Browse_Info_Display_Text)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstractHtmlSubwriter.Get_Browse_Info", String.Empty);
            }

            // Set output initially to null
            Paged_Results = null;
            Complete_Result_Set_Info = null;
            Browse_Info_Display_Text = null;

            // First, make sure the browse submode is valid
            Browse_Object = Aggregation_Object.Child_Page_By_Code(Current_Mode.Info_Browse_Mode);

            if (Browse_Object == null)
            {
                Current_Mode.Error_Message = "Unable to retrieve browse/info item '" + Current_Mode.Info_Browse_Mode + "'";
                return false;
            }

            // Is this a table result, or a string?
            switch (Browse_Object.Source_Data_Type)
            {
                case Item_Aggregation_Child_Source_Data_Enum.Database_Table:

                    // Set the current sort to ZERO, if currently set to ONE and this is an ALL BROWSE.
                    // Those two sorts are the same in this case
                    int sort = Current_Mode.Sort.HasValue ? Math.Max(Current_Mode.Sort.Value, ((ushort)1)) : 1;
                    if ((sort == 0) && (Browse_Object.Code == "all"))
                        sort = 1;

                    // Special code if this is a JSON browse
                    string browse_code = Current_Mode.Info_Browse_Mode;
                    if (Current_Mode.Writer_Type == Writer_Type_Enum.JSON)
                    {
                        browse_code = browse_code + "_JSON";
                        sort = 12;
                    }

                    // Get the page count in the results
                    int current_page_index = Current_Mode.Page.HasValue ? Math.Max(Current_Mode.Page.Value, ((ushort)1)) : 1;

                    // Determine if this is a special search type which returns more rows and is not cached.
                    // This is used to return the results as XML and DATASET
                    bool special_search_type = false;
                    int results_per_page = 20;
                    if ((Current_Mode.Writer_Type == Writer_Type_Enum.XML) || (Current_Mode.Writer_Type == Writer_Type_Enum.DataSet))
                    {
                        results_per_page = 1000000;
                        special_search_type = true;
                        sort = 2; // Sort by BibID always for these
                    }

                    // Set the flags for how much data is needed.  (i.e., do we need to pull ANYTHING?  or
                    // perhaps just the next page of results ( as opposed to pulling facets again).
                    bool need_browse_statistics = true;
                    bool need_paged_results = true;
                    if (!special_search_type)
                    {
                        // Look to see if the browse statistics are available on any cache for this browse
                        Complete_Result_Set_Info = CachedDataManager.Retrieve_Browse_Result_Statistics(Aggregation_Object.Code, browse_code, Tracer);
                        if (Complete_Result_Set_Info != null)
                            need_browse_statistics = false;

                        // Look to see if the paged results are available on any cache..
                        Paged_Results = CachedDataManager.Retrieve_Browse_Results(Aggregation_Object.Code, browse_code, current_page_index, sort, Tracer);
                        if (Paged_Results != null)
                            need_paged_results = false;
                    }

                    // Was a copy found in the cache?
                    if ((!need_browse_statistics) && (!need_paged_results))
                    {
                        if (Tracer != null)
                        {
                            Tracer.Add_Trace("SobekCM_Assistant.Get_Browse_Info", "Browse statistics and paged results retrieved from cache");
                        }
                    }
                    else
                    {
                        if (Tracer != null)
                        {
                            Tracer.Add_Trace("SobekCM_Assistant.Get_Browse_Info", "Building results information");
                        }

                        // Try to pull more than one page, so we can cache the next page or so
                        List<List<iSearch_Title_Result>> pagesOfResults;

                        // Get from the hierarchy object
                            Multiple_Paged_Results_Args returnArgs = Item_Aggregation_Utilities.Get_Browse_Results(Aggregation_Object, Browse_Object, current_page_index, sort, results_per_page, !special_search_type, need_browse_statistics, Tracer);
                            if (need_browse_statistics)
                            {
                                Complete_Result_Set_Info = returnArgs.Statistics;
                            }
                            pagesOfResults = returnArgs.Paged_Results;
                            if ((pagesOfResults != null) && (pagesOfResults.Count > 0))
                                Paged_Results = pagesOfResults[0];

                        // Save the overall result set statistics to the cache if something was pulled
                        if (!special_search_type)
                        {
                            if ((need_browse_statistics) && (Complete_Result_Set_Info != null))
                            {
                                CachedDataManager.Store_Browse_Result_Statistics(Aggregation_Object.Code, browse_code, Complete_Result_Set_Info, Tracer);
                            }

                            // Save the overall result set statistics to the cache if something was pulled
                            if ((need_paged_results) && (Paged_Results != null))
                            {
                                CachedDataManager.Store_Browse_Results(Aggregation_Object.Code, browse_code, current_page_index, sort, pagesOfResults, Tracer);
                            }
                        }
                    }
                    break;

                case Item_Aggregation_Child_Source_Data_Enum.Static_HTML:
                    Browse_Info_Display_Text = SobekEngineClient.Aggregations.Get_Aggregation_HTML_Child_Page(Aggregation_Object.Code, Aggregation_Object.Language, UI_ApplicationCache_Gateway.Settings.System.Default_UI_Language, Browse_Object.Code, Tracer);
                    break;
            }
            return true;
        }

        #endregion

    }
}
