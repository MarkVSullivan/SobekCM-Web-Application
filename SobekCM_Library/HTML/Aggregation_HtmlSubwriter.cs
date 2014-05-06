#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.AggregationViewer;
using SobekCM.Library.AggregationViewer.Viewers;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.Email;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.Settings;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;
using SobekCM.Library.WebContent;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Aggregation html subwriter renders all views of item aggregations, including home pages, searches, and browses </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Aggregation_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly Aggregation_Code_Manager codeManager;
        private readonly abstractAggregationViewer collectionViewer;
        private readonly User_Object currentUser;
        private readonly Item_Lookup_Object itemList;
        private readonly List<iSearch_Title_Result> pagedResults;
        private readonly Search_Results_Statistics resultsStatistics;
        private List<Thematic_Heading> thematicHeadings;
        private readonly Item_Aggregation_Child_Page thisBrowseObject;
        private readonly HTML_Based_Content thisStaticBrowseObject;
        private readonly Language_Support_Info translator;

        /// <summary> Constructor creates a new instance of the Aggregation_HtmlSubwriter class </summary>
		/// <param name="Current_Aggregation"> Current item aggregation object to display </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
        /// <param name="Paged_Results"> Paged results to display within a browse or search result </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Thematic_Headings"> Headings under which all the highlighted collections on the home page are organized </param>
        /// <param name="Current_User"> Currently logged on user (or object representing the unlogged on user's preferences) </param>
        /// <param name="Static_Web_Content"> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public Aggregation_HtmlSubwriter(Item_Aggregation Current_Aggregation, 
            SobekCM_Navigation_Object Current_Mode, SobekCM_Skin_Object HTML_Skin, 
            Language_Support_Info Translator, 
            Item_Aggregation_Child_Page Browse_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Aggregation_Code_Manager Code_Manager, Item_Lookup_Object All_Items_Lookup,
            List<Thematic_Heading> Thematic_Headings, User_Object Current_User,
            HTML_Based_Content Static_Web_Content,
            Custom_Tracer Tracer )
        {
            currentUser = Current_User;
			base.Current_Aggregation = Current_Aggregation;
            currentMode = Current_Mode;
            Skin = HTML_Skin;
            translator = Translator;
            thisBrowseObject = Browse_Object;
            thisStaticBrowseObject = Static_Web_Content;
            codeManager = Code_Manager;
            itemList = All_Items_Lookup;
            thematicHeadings = Thematic_Headings;
            resultsStatistics = Results_Statistics;
            pagedResults = Paged_Results;

			// Check to see if the user should be able to edit the home page
			if ((currentMode.Mode == Display_Mode_Enum.Aggregation) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit))
			{
				if ( currentUser == null )
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
				else
				{
					if ((!currentUser.Is_System_Admin) && (!currentUser.Is_Portal_Admin) && (!currentUser.Is_Aggregation_Admin(Current_Aggregation.Code)))
					{
						currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
					}
				}
			}
			else if ( currentMode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit )
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;

            NameValueCollection form = HttpContext.Current.Request.Form;
            if ( form["item_action"] != null)
            {
                string action = form["item_action"].ToLower().Trim();

                if ((action == "add_aggregation") && ( currentUser != null ))
                {
                    SobekCM_Database.User_Set_Aggregation_Home_Page_Flag(currentUser.UserID, base.Current_Aggregation.Aggregation_ID, true, Tracer);
                    currentUser.Set_Aggregation_Home_Page_Flag(base.Current_Aggregation.Code, base.Current_Aggregation.Name, true);
                    HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "Added aggregation to your home page");
                }

                if (( action == "remove_aggregation") && ( currentUser != null ))
                {
                    int removeAggregationID = base.Current_Aggregation.Aggregation_ID;
                    string remove_code = base.Current_Aggregation.Code;
                    string remove_name = base.Current_Aggregation.Name;

                    if ((form["aggregation"] != null) && (form["aggregation"].Length > 0))
                    {
                        Item_Aggregation_Related_Aggregations aggrInfo = codeManager[form["aggregation"]];
                        if (aggrInfo != null)
                        {
                            remove_code = aggrInfo.Code;
                            removeAggregationID = aggrInfo.ID;
                        }
                    }

                    SobekCM_Database.User_Set_Aggregation_Home_Page_Flag(currentUser.UserID, removeAggregationID, false, Tracer);
                    currentUser.Set_Aggregation_Home_Page_Flag(remove_code, remove_name, false);

                    if (currentMode.Home_Type != Home_Type_Enum.Personalized)
                    {
                        HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "Removed aggregation from your home page");
                    }
                }

                if ((action == "private_folder") && ( currentUser != null ))
                {
                    User_Folder thisFolder = currentUser.Get_Folder(form["aggregation"]);
                    if (SobekCM_Database.Edit_User_Folder(thisFolder.Folder_ID, currentUser.UserID, -1, thisFolder.Folder_Name, false, String.Empty, Tracer) >= 0)
                        thisFolder.isPublic = false;
                }


                if ((action == "email") && ( currentUser != null ))
                {
                    string address = form["email_address"].Replace(";", ",").Trim();
                    string comments = form["email_comments"].Trim();
                    string format = form["email_format"].Trim().ToUpper();

                    if (address.Length > 0)
                    {
                        // Determine the email format
                        bool is_html_format = format != "TEXT";

                        // CC: the user, unless they are already on the list
                        string cc_list = currentUser.Email;
                        if (address.ToUpper().IndexOf(currentUser.Email.ToUpper()) >= 0)
                            cc_list = String.Empty;

                        // Send the email
                        string any_error = URL_Email_Helper.Send_Email(address, cc_list, comments, currentUser.Full_Name, currentMode.SobekCM_Instance_Abbreviation, is_html_format, HttpContext.Current.Items["Original_URL"].ToString(), base.Current_Aggregation.Name, "Collection", currentUser.UserID);
                        HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", any_error.Length > 0 ? any_error : "Your email has been sent");

                        currentMode.isPostBack = true;

                        // Do this to force a return trip (cirumnavigate cacheing)
                        string original_url = HttpContext.Current.Items["Original_URL"].ToString();
                        if (original_url.IndexOf("?") < 0)
                            HttpContext.Current.Response.Redirect(original_url + "?p=" + DateTime.Now.Millisecond, false);
                        else
                            HttpContext.Current.Response.Redirect(original_url + "&p=" + DateTime.Now.Millisecond, false);

                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        Current_Mode.Request_Completed = true;
                        return;
                    }
                }
            }

			if (( currentMode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit ) && ( form["sbkAghsw_HomeTextEdit"] != null))
			{
				string aggregation_folder = SobekCM_Library_Settings.Base_Design_Location + "aggregations\\" + Current_Aggregation.Code + "\\";
				string file = aggregation_folder + Current_Aggregation.Home_Page_File(currentMode.Language);

				// Make a backup from today, if none made yet
				if (File.Exists(file))
				{
					DateTime lastWrite = (new FileInfo(file)).LastWriteTime;
					string new_file = file.ToLower().Replace(".txt", "").Replace(".html", "").Replace(".htm", "") + lastWrite.Year + lastWrite.Month.ToString().PadLeft(2, '0') + lastWrite.Day.ToString() .PadLeft(2, '0')+ ".bak";
					if (File.Exists(new_file))
						File.Delete(new_file);
					File.Move(file, new_file);
				}

				// Write to the file now
				StreamWriter homeWriter = new StreamWriter(file, false);
				homeWriter.WriteLine(form["sbkAghsw_HomeTextEdit"]);
				homeWriter.Flush();
				homeWriter.Close();

				// Also save this change
				SobekCM_Database.Save_Item_Aggregation_Milestone(Current_Aggregation.Code, "Home page edited (" + Web_Language_Enum_Converter.Enum_To_Name(currentMode.Language) + ")", currentUser.Full_Name);

				// Clear this aggreation from the cache
				Cached_Data_Manager.Remove_Item_Aggregation(Current_Aggregation.Code, Tracer);

				// If this is all, save the new text as well.
				if (String.Compare("all", Current_Aggregation.Code, StringComparison.OrdinalIgnoreCase) == 0)
				{
					HttpContext.Current.Application["SobekCM_Home"] = form["sbkAghsw_HomeTextEdit"];
				}

				// Forward along
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
				string redirect_url = currentMode.Redirect_URL();
				if (redirect_url.IndexOf("?") > 0)
					redirect_url = redirect_url + "&refresh=always";
				else
					redirect_url = redirect_url + "?refresh=always";
				currentMode.Request_Completed = true;
				HttpContext.Current.Response.Redirect(redirect_url, false);
				HttpContext.Current.ApplicationInstance.CompleteRequest();

				return;
			}
            
            // If this is a search, verify it is a valid search type
            if (currentMode.Mode == Display_Mode_Enum.Search)
            {
                // Not every collection has every search type...
                ReadOnlyCollection<Search_Type_Enum> possibleSearches = base.Current_Aggregation.Search_Types;
                if (!possibleSearches.Contains(currentMode.Search_Type))
                {
                    bool found_valid = false;

                    if ((currentMode.Search_Type == Search_Type_Enum.Full_Text) && (possibleSearches.Contains(Search_Type_Enum.dLOC_Full_Text)))
                    {
                        found_valid = true;
                        currentMode.Search_Type = Search_Type_Enum.dLOC_Full_Text;
                    }

                    if ((!found_valid) && (currentMode.Search_Type == Search_Type_Enum.Basic) && (possibleSearches.Contains(Search_Type_Enum.Newspaper)))
                    {
                        found_valid = true;
                        currentMode.Search_Type = Search_Type_Enum.Newspaper;
                    }

                    if (( !found_valid ) && ( possibleSearches.Count > 0 ))
                    {
                        found_valid = true;
                        currentMode.Search_Type = possibleSearches[0];
                    }

                    if ( !found_valid )
                    {
						currentMode.Mode = Display_Mode_Enum.Aggregation;
						currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    }
                }
            }

            if (currentMode.Mode == Display_Mode_Enum.Search)
            {
                collectionViewer = AggregationViewer_Factory.Get_Viewer(currentMode.Search_Type, base.Current_Aggregation, currentMode, currentUser);
            }

			if (currentMode.Mode == Display_Mode_Enum.Aggregation)
			{
				switch (currentMode.Aggregation_Type)
				{
					case Aggregation_Type_Enum.Home:
					case Aggregation_Type_Enum.Home_Edit:
						collectionViewer = AggregationViewer_Factory.Get_Viewer(base.Current_Aggregation.Views_And_Searches[0], base.Current_Aggregation, currentMode);
						break;

					case Aggregation_Type_Enum.Browse_Info:
						if (resultsStatistics == null)
						{
							collectionViewer = new Static_Browse_Info_AggregationViewer(thisBrowseObject, thisStaticBrowseObject, Current_Aggregation, currentMode, Current_User);
						}
						else
						{
							collectionViewer = new DataSet_Browse_Info_AggregationViewer(thisBrowseObject, resultsStatistics, pagedResults, codeManager, itemList, currentUser);
						}
						break;

					case Aggregation_Type_Enum.Child_Page_Edit:
						collectionViewer = new Static_Browse_Info_AggregationViewer(thisBrowseObject, thisStaticBrowseObject, Current_Aggregation, currentMode, Current_User);
						break;

					case Aggregation_Type_Enum.Browse_By:
						collectionViewer = new Metadata_Browse_AggregationViewer(Current_Mode, Current_Aggregation, Tracer);
						break;

					case Aggregation_Type_Enum.Browse_Map:
						collectionViewer = new Map_Browse_AggregationViewer(Current_Mode, Current_Aggregation, Tracer);
						break;

                    case Aggregation_Type_Enum.Browse_Map_Beta:
                        collectionViewer = new Map_Browse_AggregationViewer_Beta(Current_Mode, Current_Aggregation, Tracer);
                        break;

					case Aggregation_Type_Enum.Item_Count:
						collectionViewer = new Item_Count_AggregationViewer(Current_Mode, Current_Aggregation);
						break;

					case Aggregation_Type_Enum.Usage_Statistics:
						collectionViewer = new Usage_Statistics_AggregationViewer(Current_Mode, Current_Aggregation);
						break;

					case Aggregation_Type_Enum.Private_Items:
						collectionViewer = new Private_Items_AggregationViewer(Current_Mode, Current_Aggregation, Tracer);
						break;
				}
			}


            // If execution should end, do it now
            if (currentMode.Request_Completed)
                return;

            if (collectionViewer != null)
            {
                collectionViewer.Translator = translator;
                collectionViewer.HTML_Skin = HTML_Skin;
                collectionViewer.CurrentMode = Current_Mode;
				collectionViewer.CurrentObject = Current_Aggregation;
                collectionViewer.Current_User = Current_User;

                // Pull the standard values
                switch (collectionViewer.Selection_Panel_Display)
                {
                    case Selection_Panel_Display_Enum.Selectable:
                        if (form["show_subaggrs"] != null)
                        {
                            string show_subaggrs = form["show_subaggrs"].ToUpper();
                            if (show_subaggrs == "TRUE")
                                currentMode.Show_Selection_Panel = true;
                        }
                        break;

                    case Selection_Panel_Display_Enum.Always:
                        currentMode.Show_Selection_Panel = true;
                        break;
                }
            }
        }

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get
            {
                // When editing the aggregation details, the banner should be included here
                if (collectionViewer.Type == Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search)
                {
                    return new List<HtmlSubwriter_Behaviors_Enum>
                        {
                            HtmlSubwriter_Behaviors_Enum.Suppress_Banner
                        };
                }
                return emptybehaviors;
            }
        }

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual html subwriters </remarks>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            // Based on display mode, add ROBOT instructions
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Search:
                    Output.WriteLine("  <meta name=\"robots\" content=\"index, follow\" />");
                    break;

				case Display_Mode_Enum.Aggregation:
					if (( currentMode.Aggregation_Type == Aggregation_Type_Enum.Home ) || ( currentMode.Aggregation_Type == Aggregation_Type_Enum.Browse_Info ))
						Output.WriteLine("  <meta name=\"robots\" content=\"index, follow\" />");
					else
						Output.WriteLine("  <meta name=\"robots\" content=\"noindex, nofollow\" />");
		            break;

                default:
                    Output.WriteLine("  <meta name=\"robots\" content=\"noindex, nofollow\" />");
                    break;
            }

			// If this was to display the static page, include that info in the header as well
			if (thisStaticBrowseObject != null)
			{
				if (thisStaticBrowseObject.Description.Length > 0)
				{
					Output.WriteLine("  <meta name=\"description\" content=\"" + thisStaticBrowseObject.Description.Replace("\"","'") + "\" />");
				}
				if (thisStaticBrowseObject.Keywords.Length > 0)
				{
					Output.WriteLine("  <meta name=\"keywords\" content=\"" + thisStaticBrowseObject.Keywords.Replace("\"", "'") + "\" />");
				}
				if (thisStaticBrowseObject.Author.Length > 0)
				{
					Output.WriteLine("  <meta name=\"author\" content=\"" + thisStaticBrowseObject.Author.Replace("\"", "'") + "\" />");
				}
				if (thisStaticBrowseObject.Date.Length > 0)
				{
					Output.WriteLine("  <meta name=\"date\" content=\"" + thisStaticBrowseObject.Date.Replace("\"", "'") + "\" />");
				}

				if (thisStaticBrowseObject.Extra_Head_Info.Length > 0)
				{
					Output.WriteLine("  " + thisStaticBrowseObject.Extra_Head_Info.Trim());
				}
			}

            // In the home mode, add the open search XML file to allow users to add SobekCM as a default search in browsers
            if (( currentMode.Mode == Display_Mode_Enum.Aggregation ) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Home))
            {
                Output.WriteLine("  <link rel=\"search\" href=\"" + currentMode.Base_URL + "default/opensearch.xml\" type=\"application/opensearchdescription+xml\"  title=\"Add " + currentMode.SobekCM_Instance_Abbreviation + " Search\" />");
            }

			// If this is to edit the home page, add the html editor
	        if ((currentMode.Mode == Display_Mode_Enum.Aggregation) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit))
	        {
		        Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/scripts/htmleditor/jquery.cleditor.css\" />");
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/htmleditor/jquery.cleditor.min.js\"></script>");
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/htmleditor/jquery.cleditor.advancedtable.min.js\"></script>");
                Output.WriteLine("  <script type=\"text/javascript\">");
				Output.WriteLine("    $(document).ready(function () { $(\"#sbkAghsw_HomeTextEdit\").cleditor({height:400}); });");
		        Output.WriteLine("  </script>");
	        }

			// If this is to edit the home page, add the html editor
			if ((currentMode.Mode == Display_Mode_Enum.Aggregation) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Child_Page_Edit))
			{
				Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/scripts/htmleditor/jquery.cleditor.css\" />");
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/htmleditor/jquery.cleditor.min.js\"></script>");
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/htmleditor/jquery.cleditor.advancedtable.min.js\"></script>");
				Output.WriteLine("  <script type=\"text/javascript\">");
				Output.WriteLine("    $(document).ready(function () { $(\"#sbkSbia_ChildTextEdit\").cleditor({height:400}); });");
				Output.WriteLine("  </script>");
			}
        }


        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get
            {
	            if (currentMode.Mode == Display_Mode_Enum.Search)
	            {
		            return (Current_Aggregation != null) ? "{0} Search - " + Current_Aggregation.Name : "{0} Search";
	            }
	            
				if (currentMode.Mode == Display_Mode_Enum.Aggregation)
	            {
		            switch (currentMode.Aggregation_Type)
		            {
			            case Aggregation_Type_Enum.Home:
				            if (Current_Aggregation != null)
				            {
					            return (Current_Aggregation.Code == "ALL") ? "{0} Home" : "{0} Home - " + Current_Aggregation.Name;
				            }
				            return "{0} Home";

			            case Aggregation_Type_Enum.Browse_Info:
							if (thisStaticBrowseObject != null)
							{
								if (thisStaticBrowseObject.Title.Length > 0)
									return "{0} - " + thisStaticBrowseObject.Title + " - " + Current_Aggregation.Name;
								return "{0} - " + currentMode.Info_Browse_Mode + " - " + Current_Aggregation.Name;
							}
							
							if (Current_Aggregation != null)
							{
								return "{0} - " + Current_Aggregation.Name;
							}

				            break;

						case Aggregation_Type_Enum.Child_Page_Edit:
							if (Current_Aggregation != null)
							{
								return "{0} - Edit " + Current_Aggregation.Name;
							}
							break;

						case Aggregation_Type_Enum.Browse_By:
						case Aggregation_Type_Enum.Browse_Map:
                        case Aggregation_Type_Enum.Browse_Map_Beta:
						case Aggregation_Type_Enum.Private_Items:
						case Aggregation_Type_Enum.Item_Count:
						case Aggregation_Type_Enum.Usage_Statistics:
				            return "{0} - " + Current_Aggregation.Name;
		            }
	            }

	            // default
                return (Current_Aggregation != null) ? "{0} - " + Current_Aggregation.Name : "{0}";
            }
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        public override List<Tuple<string, string>> Body_Attributes
        {
            get
            {
                List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>();
	            if (currentMode.Mode == Display_Mode_Enum.Search)
	            {
		            if (currentMode.Search_Type == Search_Type_Enum.Map)
		            {
			            returnValue.Add(new Tuple<string, string>("onload", "load();"));
		            }
                    if (currentMode.Search_Type == Search_Type_Enum.Map_Beta)
                    {
                        returnValue.Add(new Tuple<string, string>("onload", "load();"));
                    }
				}
				else if (currentMode.Mode == Display_Mode_Enum.Aggregation)
				{
					switch (currentMode.Aggregation_Type)
					{
						case Aggregation_Type_Enum.Browse_Info:
							if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map)
							{
								returnValue.Add(new Tuple<string, string>("onload", "load();"));
							}
							break;

						case Aggregation_Type_Enum.Browse_Map:
							returnValue.Add(new Tuple<string, string>("onload", "load();"));
							break;

                        case Aggregation_Type_Enum.Browse_Map_Beta:
                            returnValue.Add(new Tuple<string, string>("onload", "load();"));
                            break;
					}
				}

                return returnValue;
            }
        }

        #region Public method to write the internal header

        /// <summary> Adds the internal header HTML for this specific HTML writer </summary>
        /// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
        /// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
        public override void Write_Internal_Header_HTML(TextWriter Output, User_Object Current_User)
        {
	        if ((Current_User != null) && ( currentMode.Aggregation.Length > 0 ) && ( currentMode.Aggregation.ToUpper() != "ALL" ) && ((Current_User.Is_Aggregation_Curator(currentMode.Aggregation)) || (Current_User.Is_Internal_User) || ( Current_User.Can_Edit_All_Items( currentMode.Aggregation ))))
            {
				Output.WriteLine("  <table id=\"sbk_InternalHeader\">");
                Output.WriteLine("    <tr style=\"height:45px;\">");
                Output.WriteLine("      <td style=\"text-align:left; width:100px;\">");
                Output.WriteLine("          <button title=\"Hide Internal Header\" class=\"intheader_button_aggr hide_intheader_button_aggr\" onclick=\"return hide_internal_header();\" alt=\"Hide Internal Header\"></button>");
                Output.WriteLine("      </td>");

                Output.WriteLine("      <td style=\"text-align:center; vertical-align:middle\">");

                // Add button to view private items
                Display_Mode_Enum displayMode = currentMode.Mode;
	            Aggregation_Type_Enum aggrType = currentMode.Aggregation_Type;
                string submode = currentMode.Info_Browse_Mode;

				currentMode.Mode = Display_Mode_Enum.Aggregation;
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Private_Items;
                currentMode.Info_Browse_Mode = String.Empty;
                Output.WriteLine("          <button title=\"View Private Items\" class=\"intheader_button_aggr view_private_items\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\" ></button>");

                // Add button to view item count information
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Item_Count;
                Output.WriteLine("          <button title=\"View Item Count\" class=\"intheader_button_aggr show_item_count\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                // Add button to view usage statistics information
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Usage_Statistics;
                Output.WriteLine("          <button title=\"View Usage Statistics\" class=\"intheader_button_aggr show_usage_statistics\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                // Add admin view is system administrator
                if ((Current_User.Is_System_Admin) || (Current_User.Is_Aggregation_Curator(Current_Aggregation.Code)))
                {
                    currentMode.Mode = Display_Mode_Enum.Administrative;
                    currentMode.Admin_Type = Admin_Type_Enum.Aggregation_Single;
                    Output.WriteLine("          <button title=\"Edit Administrative Information\" class=\"intheader_button_aggr admin_view_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\" ></button>");
                }

                Output.WriteLine("      </td>");

                currentMode.Info_Browse_Mode = submode;
                currentMode.Mode = displayMode;
	            currentMode.Aggregation_Type = aggrType;

                // Add the HELP icon next
                Output.WriteLine("      <td style=\"text-align:left; width:30px;\">");
				Output.WriteLine("        <span id=\"sbk_InternalHeader_Help\"><a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "help/aggrheader\" title=\"Help regarding this header\" ><img src=\"" + currentMode.Base_URL + "default/images/help_button_darkgray.jpg\" alt=\"?\" title=\"Help regarding this header\" /></a></span>");
                Output.WriteLine("      </td>");

                Write_Internal_Header_Search_Box(Output);

                Output.WriteLine("    </tr>");
                
                Output.WriteLine("  </table>");
            }
            else
            {
                base.Write_Internal_Header_HTML(Output, Current_User);
            }
        }
		
        #endregion

        #region Public method to write HTML to the output stream

        /// <summary> Writes the HTML generated by this aggregation html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aggregation_HtmlSubwriter.Write_HTML", "Rendering HTML");
			
			// Draw the banner and add links to the other views first
	        if (collectionViewer.Type != Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search)
	        {
		        // If this skin has top-level navigation suppressed, skip the top tabs
		        if (htmlSkin.Suppress_Top_Navigation)
		        {
			        Output.WriteLine("<br />");
		        }
				else if (collectionViewer.Type != Item_Aggregation.CollectionViewsAndSearchesEnum.DataSet_Browse)
		        {
			        // Add the main aggrgeation menu here
			        MainMenus_Helper_HtmlSubWriter.Add_Aggregation_Main_Menu(Output, currentMode, currentUser, Current_Aggregation, translator, codeManager);

					// Start the page container
					Output.WriteLine("<div id=\"pagecontainer\">");
					Output.WriteLine("<br />");
		        }
				else
				{
					// Add the main aggrgeation menu here
					MainMenus_Helper_HtmlSubWriter.Add_Aggregation_Search_Results_Menu(Output, currentMode, currentUser, Current_Aggregation, translator, codeManager, false);

					// Start the (optional) page container
					Output.WriteLine("<div id=\"sbkAhs_ResultsPageContainer\">");
				}
	        }



            // If this is the map browse, end the page container here
            if (( currentMode.Mode == Display_Mode_Enum.Aggregation ) && ( currentMode.Aggregation_Type == Aggregation_Type_Enum.Browse_Map))
                Output.WriteLine("</div>");

            // If this is the map browse beta, end the page container here
            if ((currentMode.Mode == Display_Mode_Enum.Aggregation) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Browse_Map_Beta))
                Output.WriteLine("</div>");

            // Is there a script to be included?
            if (collectionViewer.Search_Script_Reference.Length > 0)
                Output.WriteLine(collectionViewer.Search_Script_Reference);

            // Write the search box
            if (collectionViewer.Type != Item_Aggregation.CollectionViewsAndSearchesEnum.DataSet_Browse)
            {
                string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());

                if (collectionViewer.Search_Script_Action.Length > 0)
                {
                    Output.WriteLine("<form name=\"search_form\" method=\"post\" action=\"" + post_url + "\" id=\"addedForm\" >");
                }
                else
                {
                    Output.WriteLine("<form name=\"search_form\" method=\"post\" action=\"" + post_url + "\" id=\"addedForm\" >");
                }

                const string FORM_NAME = "search_form";
                if (( currentMode.Mode == Display_Mode_Enum.Aggregation ) && (( currentMode.Aggregation_Type == Aggregation_Type_Enum.Home ) || ( currentMode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit )))
                {
                    // Determine the number of columns for text areas, depending on browser
                    int actual_cols = 50;
                    if (currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                        actual_cols = 45;

                    // Add the hidden field
                    Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                    Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
                    Output.WriteLine("<input type=\"hidden\" id=\"aggregation\" name=\"aggregation\" value=\"\" />");
                    Output.WriteLine("<input type=\"hidden\" id=\"show_subaggrs\" name=\"show_subaggrs\" value=\"" + currentMode.Show_Selection_Panel.ToString() + "\" />");

                    #region Email form

                    if (currentUser != null)
                    {
                        Output.WriteLine("<!-- Email form -->");
						Output.WriteLine("<div class=\"form_email sbk_PopupForm\" id=\"form_email\" style=\"display:none;\">");
						Output.WriteLine("  <div class=\"sbk_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left\">Send this Collection to a Friend</td><td style=\"text-align:right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"email_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                        Output.WriteLine("  <br />");
                        Output.WriteLine("  <fieldset><legend>Enter the email information below &nbsp; </legend>");
                        Output.WriteLine("    <br />");
						Output.WriteLine("    <table class=\"sbk_PopupTable\">");


                        // Add email address line
                        Output.Write("      <tr><td style=\"width:80px\"><label for=\"email_address\">To:</label></td>");
						Output.WriteLine("<td><input class=\"email_input sbk_Focusable\" name=\"email_address\" id=\"email_address\" type=\"text\" value=\"" + currentUser.Email + "\" /></td></tr>");

                        // Add comments area
                        Output.Write("      <tr style=\"vertical-align:top\"><td><br /><label for=\"email_comments\">Comments:</label></td>");
						Output.WriteLine("<td><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"email_comments\" id=\"email_comments\" class=\"email_textarea sbk_Focusable\" ></textarea></td></tr>");

                        // Add format area
						Output.Write("      <tr style=\"vertical-align:top\"><td>Format:</td>");
                        Output.Write("<td><input type=\"radio\" name=\"email_format\" id=\"email_format_html\" value=\"html\" checked=\"checked\" /> <label for=\"email_format_html\">HTML</label> &nbsp; &nbsp; ");
                        Output.WriteLine("<input type=\"radio\" name=\"email_format\" id=\"email_format_text\" value=\"text\" /> <label for=\"email_format_text\">Plain Text</label></td></tr>");


                        Output.WriteLine("    </table>");
                        Output.WriteLine("    <br />");
						Output.WriteLine("  </fieldset><br />");
						Output.WriteLine("  <div style=\"text-align:center; font-size:1.3em;\">");
						Output.WriteLine("    <button title=\"Send\" class=\"roundbutton\" onclick=\"return email_form_close();\"> CANCEL </button> &nbsp; &nbsp; ");
						Output.WriteLine("    <button title=\"Send\" class=\"roundbutton\" type=\"submit\"> SEND </button>");
						Output.WriteLine("  </div><br />");
						Output.WriteLine("</div>");
                        Output.WriteLine();

                    }

                    #endregion

                    #region Share form

                    Output.WriteLine("<!-- Share form (empty, filled by javascript)-->");
                    Output.WriteLine("<div class=\"share_popup_div\" id=\"share_form\" style=\"display:none;\"></div>");
                    Output.WriteLine();

                    #endregion

                    if (collectionViewer.Type != Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search)
                    {
                        Output.WriteLine("<div class=\"SobekSearchPanel\">");
                        Add_Sharing_Buttons(Output, FORM_NAME, "SobekResultsSort");
                    }
                }
                else
                {
                    if (( currentMode.Mode != Display_Mode_Enum.Aggregation ) || ( currentMode.Aggregation_Type != Aggregation_Type_Enum.Browse_Map ))
                        Output.WriteLine("<div class=\"SobekSearchPanel\">");

                    if ((currentMode.Mode != Display_Mode_Enum.Aggregation) || (currentMode.Aggregation_Type != Aggregation_Type_Enum.Browse_Map_Beta))
                        Output.WriteLine("<div class=\"SobekSearchPanel\">");
                }

                if (collectionViewer.Type == Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search)
                {
                    StringBuilder builder = new StringBuilder(2000);
                    StringWriter writer = new StringWriter(builder);
                    Add_Sharing_Buttons(writer, FORM_NAME, "SobekHomeBannerButton");
                    ((Rotating_Highlight_Search_AggregationViewer)collectionViewer).Sharing_Buttons_HTML = builder.ToString();

                    collectionViewer.Add_Search_Box_HTML(Output, Tracer);

	                MainMenus_Helper_HtmlSubWriter.Add_Aggregation_Main_Menu(Output, currentMode, currentUser, Current_Aggregation, translator, codeManager);

					// Start the page container
					Output.WriteLine("<div id=\"pagecontainer\">");
                }
                else
                {
                    collectionViewer.Add_Search_Box_HTML(Output, Tracer);
                    Output.WriteLine((( currentMode.Mode != Display_Mode_Enum.Aggregation ) || ( currentMode.Aggregation_Type != Aggregation_Type_Enum.Browse_Map )) ? "</div>" : "<div id=\"pagecontainer_resumed\">");

                    Output.WriteLine(((currentMode.Mode != Display_Mode_Enum.Aggregation) || (currentMode.Aggregation_Type != Aggregation_Type_Enum.Browse_Map_Beta)) ? "</div>" : "<div id=\"pagecontainer_resumed\">");
                }

                Output.WriteLine();
            }
            else
            {
                collectionViewer.Add_Search_Box_HTML(Output, Tracer);
            }

			#region Old code to show the collection selection panel, now deprecated for the main menu

			//// Prepare to add the collection selector information, but first, check to see if this the main home page
			//bool sobekcm_main_home_page = (currentMode.Mode == Display_Mode_Enum.Aggregation) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Home) && (Current_Aggregation.Code == "all");

			//// Add the collection selector, if it ever appears here
			//if ((!sobekcm_main_home_page) && (collectionViewer.Selection_Panel_Display != Selection_Panel_Display_Enum.Never) && (Current_Aggregation.Children_Count > 0))
			//{
			//	// Get the collection of children
			//	ReadOnlyCollection<Item_Aggregation_Related_Aggregations> child_aggregations = Current_Aggregation.Children;

			//	// Set the strings for the tab here
			//	string show_collect_groups = "SHOW COLLECTION GROUPS";
			//	string show_collect = "SHOW COLLECTIONS";
			//	string show_subcollect = "SHOW SUBCOLLECTIONS";
			//	string hide_collect_groups = "HIDE COLLECTION GROUPS";
			//	string hide_collect = "HIDE COLLECTIONS";
			//	string hide_subcollect = "HIDE SUBCOLLECTIONS";
			//	string select_collect_groups = "Select collection groups to include in search:";
			//	string select_collect = "Select collections to include in search:";
			//	string select_subcollect = "Select subcollections to include in search:";

			//	// Change text if this is Spanish
			//	if (currentMode.Language == Web_Language_Enum.Spanish)
			//	{
			//		show_collect_groups = "SELECCIONE GRUPOS DE COLECCIONES";
			//		show_collect = "SELECCIONE COLECCIONES";
			//		show_subcollect = "SELECCIONE SUBCOLECCIONES";
			//		hide_collect_groups = "ESCONDA GRUPOS DE COLECCIONES";
			//		hide_collect = "ESCONDA COLECCIONES";
			//		hide_subcollect = "ESCONDA SUBCOLECCIONES";
			//		select_collect_groups = "Seleccione grupos de colecciones para incluir en la búsqueda:";
			//		select_collect = "Seleccione colecciones para incluir en la búsqueda:";
			//		select_subcollect = "Seleccione subcolecciones para incluir en la búsqueda:";

			//	}

			//	// Change the text if this is french
			//	if (currentMode.Language == Web_Language_Enum.French)
			//	{
			//		show_collect_groups = "VOIR LE GROUPE DE COLLECTION";
			//		show_collect = "VOIR LES COLLECTIONS";
			//		show_subcollect = "VOIR LES SOUSCOLLECTIONS";
			//		hide_collect_groups = "SUPPRIMER LE GROUPE DE COLLECTION";
			//		hide_collect = "SUPPRIMER LES COLLECTIONS";
			//		hide_subcollect = "SUPPRIMER LES SOUSCOLLECTIONS";
			//		select_collect_groups = "Choisir les group de collection pour inclure dans votre recherche:";
			//		select_collect = "Choisir les collections pour inclure dans votre recherche:";
			//		select_subcollect = "Choisir les souscollections pour inclure dans votre recherche:";
			//	}

			//	// Determine the sub text to use
			//	string select_text = select_subcollect;
			//	string show_text = show_subcollect;
			//	string hide_text = hide_subcollect;
			//	if (Current_Aggregation.Code == "all")
			//	{
			//		select_text = select_collect_groups;
			//		show_text = show_collect_groups;
			//		hide_text = hide_collect_groups;
			//	}
			//	else
			//	{
			//		if (child_aggregations[0].Type.ToUpper() == "COLLECTION")
			//		{
			//			select_text = select_collect;
			//			show_text = show_collect;
			//			hide_text = hide_collect;
			//		}
			//	}

			//	if ((collectionViewer.Selection_Panel_Display == Selection_Panel_Display_Enum.Selectable) && (!currentMode.Show_Selection_Panel))
			//	{
			//		Output.WriteLine("<div class=\"ShowSelectRow\">");
			//		//currentMode.Show_Selection_Panel = true;
			//		Output.WriteLine("  <a href=\"\" onclick=\"return set_subaggr_display('true');\">" + Down_Tab_Start + show_text + Down_Tab_End + "</a>");
			//		//currentMode.Show_Selection_Panel = false;
			//		Output.WriteLine("</div>");
			//		Output.WriteLine();
			//	}
			//	else
			//	{
			//		if (collectionViewer.Selection_Panel_Display == Selection_Panel_Display_Enum.Selectable)
			//		{
			//			Output.WriteLine("<div class=\"HideSelectRow\">");
			//			//currentMode.Show_Selection_Panel = false;
			//			Output.WriteLine("  <a href=\"\" onclick=\"return set_subaggr_display('false');\">" + Unselected_Tab_Start + hide_text + Unselected_Tab_End + "</a>");
			//			//currentMode.Show_Selection_Panel = true;
			//			Output.WriteLine("</div>");
			//			Output.WriteLine();
			//		}
			//		else
			//		{
			//			Output.WriteLine("<br />");
			//		}

			//		Output.WriteLine("<div class=\"SobekSelectPanel\"><b>" + select_text + "</b>");
			//		Output.WriteLine("  <br />");

			//		Display_Mode_Enum lastDisplayMode = currentMode.Mode;
			//		Aggregation_Type_Enum lastAggrType = currentMode.Aggregation_Type;
			//		string thisAggr = currentMode.Aggregation;
			//		string thisAlias = currentMode.Aggregation_Alias;
			//		currentMode.Aggregation_Alias = String.Empty;
			//		currentMode.Mode = Display_Mode_Enum.Aggregation;
			//		currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
			//		foreach (Item_Aggregation_Related_Aggregations t in child_aggregations)
			//		{
			//			if ((t.Active) && (!t.Hidden))
			//			{
			//				Output.WriteLine("  <span class=\"SobekSelectCheckBox\">");
			//				Output.Write("    <input type=\"checkbox\" value=\"" + t.Code + "\" name=\"checkgroup\"");
			//				Output.WriteLine("< checked=\"checked\" />");
			//			//    Output.WriteLine(currentMode.SubAggregation.IndexOf(t.Code) < 0 ? " />" : " checked />");
			//				currentMode.Aggregation = t.Code;
			//				Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\">" + t.Name + "</a>");
			//				Output.WriteLine("  </span>");
			//				Output.WriteLine("  <br />");
			//			}
			//		}
			//		currentMode.Aggregation = thisAggr;
			//		currentMode.Aggregation_Alias = thisAlias;
			//		currentMode.Mode = lastDisplayMode;
			//		currentMode.Aggregation_Type = lastAggrType;
			//		Output.WriteLine("</div>");
			//	}
			//}

			#endregion

			Output.WriteLine("</form>");

            // Add the secondary HTML ot the home page
            bool finish_page = true;
            if (((currentMode.Mode == Display_Mode_Enum.Aggregation) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Home )) || (collectionViewer.Always_Display_Home_Text))
            {
                finish_page = add_home_html(Output, Tracer);
            }
            else
            {
                collectionViewer.Add_Secondary_HTML(Output, Tracer);
            }

            if (( currentMode.Mode == Display_Mode_Enum.Aggregation ) && ( currentMode.Aggregation_Type == Aggregation_Type_Enum.Browse_Info ))
            {
                if (resultsStatistics != null)
                    finish_page = false;
            }
            return finish_page;
        }

        private void Add_Sharing_Buttons( TextWriter Output, string FormName, string Style )
        {
            #region Add the buttons for sharing, emailing, etc..

            Output.Write("  <span class=\"" + Style + "\">");
            Output.Write("<a href=\"\" onmouseover=\"document." + FormName + ".print_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button_h.gif'\" onmouseout=\"document." + FormName + ".print_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button.gif'\" onclick=\"window.print(); return false;\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"print_button\" id=\"print_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button.gif\" title=\"Print this page\" alt=\"PRINT\" /></a>");

            if (currentUser != null)
            {
                if ((currentMode.Home_Type == Home_Type_Enum.Personalized) && (currentMode.Aggregation.Length == 0))
                {
                    Output.Write("<img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"send_button\" id=\"send_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif\" title=\"Send this to someone\" alt=\"SEND\" />");
                }
                else
                {
					Output.Write("<a href=\"\" onmouseover=\"document." + FormName + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button_h.gif'\" onmouseout=\"document." + FormName + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif'\" onclick=\"return email_form_open2();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"send_button\" id=\"send_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif\" title=\"Send this to someone\" alt=\"SEND\" /></a>");

                }
                if (Current_Aggregation.Aggregation_ID > 0)
                {
                    if (currentUser.Is_On_Home_Page(currentMode.Aggregation))
                    {
                        Output.Write("<a href=\"\" onmouseover=\"document." + FormName + ".remove_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/remove_rect_button_h.gif'\" onmouseout=\"document." + FormName + ".remove_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/remove_rect_button.gif'\" onclick=\"return remove_aggregation();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"remove_button\" id=\"remove_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/remove_rect_button.gif\" title=\"Remove this from my collections home page\" alt=\"REMOVE\" /></a>");
                    }
                    else
                    {
                        Output.Write("<a href=\"\" onmouseover=\"document." + FormName + ".add_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button_h.gif'\" onmouseout=\"document." + FormName + ".add_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button.gif'\" onclick=\"return add_aggregation();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"add_button\" id=\"add_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button.gif\" title=\"Add this to my collections home page\" alt=\"ADD\" /></a>");
                    }
                }
            }
            else
            {
                Output.Write("<a href=\"?m=hmh\" onmouseover=\"document." + FormName + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button_h.gif'\" onmouseout=\"document." + FormName + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif'\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"send_button\" id=\"send_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif\" title=\"Send this to someone\" alt=\"SEND\" /></a>");
                Output.Write("<a href=\"?m=hmh\" onmouseover=\"document." + FormName + ".add_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button_h.gif'\" onmouseout=\"document." + FormName + ".add_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button.gif'\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"add_button\" id=\"add_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button.gif\" title=\"Save this to my collections home page\" alt=\"ADD\" /></a>");
            }

            if ((currentMode.Home_Type == Home_Type_Enum.Personalized) && (currentMode.Aggregation.Length == 0))
            {
                Output.Write("<img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"share_button\" id=\"share_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button.gif\" title=\"Share this collection\" />");
            }
            else
            {
				// Calculate the title and url
				string title = HttpUtility.HtmlEncode(Current_Aggregation.Name.Replace("'",""));
				string share_url = HttpContext.Current.Items["Original_URL"].ToString().Replace("&", "%26").Replace("?", "%3F").Replace("http://", "").Replace("=", "%3D").Replace("\"", "&quot;");


                Output.Write("<a href=\"\" onmouseover=\"document." + FormName + ".share_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button_h.gif'\" onmouseout=\"document." + FormName + ".share_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button.gif'\" onclick=\"return toggle_share_form2('" + title.Replace("'","") + "','" + share_url + "','" + currentMode.Base_URL + "');\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"share_button\" id=\"share_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button.gif\" title=\"Share this collection\" alt=\"SHARE\" /></a>");
            }
            Output.WriteLine("</span>");

            #endregion
        }

        #endregion

        #region Public method to add controls to the place holder 

        /// <summary> Adds the tree view control to the provided place holder if this is the tree view main home page </summary>
        /// <param name="MainPlaceHolder"> Place holder into which to place the built tree control </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns>Flag indicates if secondary text contains controls </returns>
        public bool Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (((currentMode.Home_Type == Home_Type_Enum.Tree_Collapsed) || (currentMode.Home_Type == Home_Type_Enum.Tree_Expanded)) && (Current_Aggregation.Code == "all"))
            {
                Tracer.Add_Trace("Aggregation_HtmlSubwriter.Add_Controls", "Adding tree view of collection hierarchy");

                // Make sure the ALL aggregations has the collection hierarchies
                if (Current_Aggregation.Children_Count == -1)
                {
                    // Get the collection hierarchy information
                    SobekCM_Database.Add_Children_To_Main_Agg(Current_Aggregation, Tracer);
                }

                Home_Type_Enum currentType = currentMode.Home_Type;
                currentMode.Home_Type = Home_Type_Enum.Tree_Expanded;
                string expand_url = currentMode.Redirect_URL();
                currentMode.Home_Type = Home_Type_Enum.Tree_Collapsed;
                string collapsed_url = currentMode.Redirect_URL();
                currentMode.Home_Type = currentType;

				Literal literal1 = new Literal { Text = string.Format("<div class=\"SobekText\">" + Environment.NewLine + "<h2 style=\"margin-top:0;\">All Collections</h2>" + Environment.NewLine + "<blockquote>" + Environment.NewLine + "<div style=\"text-align:right;\"><a href=\"{0}\">Collapse All</a> | <a href=\"{1}\">Expand All</a></div>" + Environment.NewLine, collapsed_url, expand_url) };
                MainPlaceHolder.Controls.Add(literal1);

                // Create the treeview
                TreeView treeView1 = new TreeView
                                         {CssClass = "SobekCollectionTreeView", ExpandDepth = 0, NodeIndent = 15};

                // load the table of contents in the tree
                Create_TreeView_From_Collections(treeView1);

                // Add the tree view to the placeholder
                MainPlaceHolder.Controls.Add(treeView1);

                Literal literal2 = new Literal {Text = @"</blockquote></div>"};
                MainPlaceHolder.Controls.Add(literal2);

                return true;
            }

            if (collectionViewer.Secondary_Text_Requires_Controls)
            {
                collectionViewer.Add_Secondary_Controls(MainPlaceHolder, Tracer);
                return true;
            }

            return false;
        }

        #endregion

        #region Methods to add home page text

        /// <summary> Adds the home page text to the output for this collection view </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE unless this is tree view mode, in which case the tree control needs to be added before the page can be finished </returns>
        protected internal bool add_home_html(TextWriter Output, Custom_Tracer Tracer)
        {
			Output.WriteLine();
			Output.WriteLine("<div class=\"SobekText\">");

            // If this is a normal aggregation type ( i.e., not the library home ) just display the home text normally
            if ((currentMode.Aggregation.Length != 0) && (Current_Aggregation.Aggregation_ID > 0))
            {
                string url_options = currentMode.URL_Options();
                string urlOptions1 = String.Empty;
                string urlOptions2 = String.Empty;
                if (url_options.Length > 0)
                {
                    urlOptions1 = "?" + url_options;
                    urlOptions2 = "&" + url_options;
                }

				// Get the raw home hteml text
				string home_html = Current_Aggregation.Get_Home_HTML(currentMode.Language, Tracer);

	            bool isAdmin = (currentUser != null ) && ( currentUser.Is_Aggregation_Admin(Current_Aggregation.Code));

	            if (( isAdmin ) && ( currentMode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit))
	            {
					string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());
					Output.WriteLine("<form name=\"home_edit_form\" method=\"post\" action=\"" + post_url + "\" id=\"addedForm\" >");
					Output.WriteLine("  <textarea id=\"sbkAghsw_HomeTextEdit\" name=\"sbkAghsw_HomeTextEdit\" >");
					Output.WriteLine(home_html);
					Output.WriteLine("  </textarea>");
					Output.WriteLine();

					Output.WriteLine("<div id=\"sbkAghsw_HomeEditButtons\">");
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
					Output.WriteLine("  <button title=\"Do not apply changes\" class=\"roundbutton\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"roundbutton_img_left\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
					Output.WriteLine("  <button title=\"Save changes to this aggregation home page text\" class=\"roundbutton\" type=\"submit\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
					Output.WriteLine("</div>");
					Output.WriteLine("</form>");
					Output.WriteLine("<br /><br /><br />");
					Output.WriteLine();

	            }
	            else
	            {

		            // Add the highlights
		            if ((Current_Aggregation.Highlights.Count > 0) && (collectionViewer.Type != Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search))
		            {
			            Output.WriteLine(Current_Aggregation.Highlights[0].ToHTML(currentMode.Language, currentMode.Base_Design_URL + Current_Aggregation.ObjDirectory).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2));
		            }

		            // Determine the different counts as strings and replace if they exist
		            if ((home_html.Contains("<%PAGES%>")) || (home_html.Contains("<%TITLES%>")) || (home_html.Contains("<%ITEMS%>")))
		            {
			            if ((Current_Aggregation.Page_Count < 0) && (Current_Aggregation.Item_Count < 0) && (Current_Aggregation.Title_Count < 0))
			            {
				            if ((!currentMode.Is_Robot) && (SobekCM_Database.Get_Item_Aggregation_Counts(Current_Aggregation, Tracer)))
				            {
					            Cached_Data_Manager.Store_Item_Aggregation(Current_Aggregation.Code, currentMode.Language_Code, Current_Aggregation, Tracer);

					            string page_count = Int_To_Comma_String(Current_Aggregation.Page_Count);
					            string item_count = Int_To_Comma_String(Current_Aggregation.Item_Count);
					            string title_count = Int_To_Comma_String(Current_Aggregation.Title_Count);

					            home_html = home_html.Replace("<%PAGES%>", page_count).Replace("<%ITEMS%>", item_count).Replace("<%TITLES%>", title_count);
				            }
				            else
				            {
					            home_html = home_html.Replace("<%PAGES%>", String.Empty).Replace("<%ITEMS%>", String.Empty).Replace("<%TITLES%>", String.Empty);
				            }
			            }
			            else
			            {
				            string page_count = Int_To_Comma_String(Current_Aggregation.Page_Count);
				            string item_count = Int_To_Comma_String(Current_Aggregation.Item_Count);
				            string title_count = Int_To_Comma_String(Current_Aggregation.Title_Count);

				            home_html = home_html.Replace("<%PAGES%>", page_count).Replace("<%ITEMS%>", item_count).Replace("<%TITLES%>", title_count);
			            }
		            }

		            // Replace any item aggregation specific custom directives
		            string original_home = home_html;
		            home_html = Current_Aggregation.Custom_Directives.Keys.Where(original_home.Contains).Aggregate(home_html, (Current, ThisKey) => Current.Replace(ThisKey, Current_Aggregation.Custom_Directives[ThisKey].Replacement_HTML));

		            // Replace any standard directives last
		            home_html = home_html.Replace("<%BASEURL%>", currentMode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2);

		            // Output the adjusted home html
		            if (isAdmin)
		            {
						Output.WriteLine("<div id=\"sbkAghsw_EditableHome\">");
			            Output.WriteLine(home_html);
						currentMode.Aggregation_Type = Aggregation_Type_Enum.Home_Edit;
						Output.WriteLine("<div id=\"sbkAghsw_EditableHomeLink\"><a href=\"" + currentMode.Redirect_URL() + "\" title=\"Edit this home text\"><img src=\"" + currentMode.Base_URL + "default/images/edit.gif\" alt=\"\" />edit content</a></div>");
						currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
						Output.WriteLine("</div>");
						Output.WriteLine();

						Output.WriteLine("<script>");
						Output.WriteLine("  $(\"#sbkAghsw_EditableHome\").mouseover(function() { $(\"#sbkAghsw_EditableHomeLink\").css(\"display\",\"inline-block\"); });");
						Output.WriteLine("  $(\"#sbkAghsw_EditableHome\").mouseout(function() { $(\"#sbkAghsw_EditableHomeLink\").css(\"display\",\"none\"); });");
						Output.WriteLine("</script>");
						Output.WriteLine();
		            }
		            else
		            {
						Output.WriteLine("<div id=\"sbkAghsw_Home\">");
						Output.WriteLine(home_html);
						Output.WriteLine("</div>");
		            }
	            }

	            // If there are sub aggregations here, show them
	            if (Current_Aggregation.Children_Count > 0)
	            {
		            // Verify some of the children are active and not hidden
		            // Keep the last aggregation alias
		            string lastAlias = currentMode.Aggregation_Alias;
		            currentMode.Aggregation_Alias = String.Empty;

		            // Collect the html to write (this alphabetizes the children)
		            List<string> html_list = new List<string>();
	                int aggreCount = -1;
		            foreach (Item_Aggregation_Related_Aggregations childAggr in Current_Aggregation.Children)
		            {
		                aggreCount++;
			            Item_Aggregation_Related_Aggregations latest = codeManager[childAggr.Code];
						if ((latest != null ) && (!latest.Hidden) && (latest.Active))
			            {
				            string name = childAggr.Name;
			                string thisDescription = childAggr.Description;

				            if (name.ToUpper() == "ADDED AUTOMATICALLY")
					            name = childAggr.Code + " ( Added Automatically )";

				            currentMode.Aggregation = childAggr.Code.ToLower();
				            string image_url = currentMode.Base_URL + "design/aggregations/" + childAggr.Code + "/images/buttons/coll.gif";
				            if ((name.IndexOf("The ") == 0) && (name.Length > 4))
				            {
							//	html_list.Add("    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(name, currentMode.Language) + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(name, currentMode.Language) + "</a></span>" + Environment.NewLine + "    </td>");
                                html_list.Add("    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\" class=\"sbkAghsw_CollectionButtonImg\"><span class=\"spanHoverText\" style=\"display:none\">" + thisDescription + "</span><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "  </td>");
				            }
				            else
				            {
								//html_list.Add("    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(name, currentMode.Language) + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(name, currentMode.Language) + "</a></span>" + Environment.NewLine + "    </td>");
                                html_list.Add("    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\" class=\"sbkAghsw_CollectionButtonImg\"><span class=\"spanHoverText\" style=\"display:none\">" + thisDescription + "</span><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "  </td>");
				            }
			            }
		            }

		            if (html_list.Count > 0)
		            {
			            string childTypes = Current_Aggregation.Child_Types.Trim();
			            if (childTypes.IndexOf(" ") > 0)
			            {
				            // Write the name of the sub aggregations
				            StringBuilder aggregationTypeBuilder = new StringBuilder(30);
				            string[] splitter = childTypes.Trim().Split(" ".ToCharArray());
				            foreach (string thisSplit in splitter.Where(ThisSplit => ThisSplit.Length > 0))
				            {
					            if (thisSplit.Length == 1)
					            {
						            aggregationTypeBuilder.Append(thisSplit + " ");
					            }
					            else
					            {
						            aggregationTypeBuilder.Append(thisSplit[0] + thisSplit.Substring(1).ToLower() + " ");
					            }
				            }

				            Output.WriteLine("<h2 id=\"subcolls\">" + translator.Get_Translation(aggregationTypeBuilder.ToString().Trim(), currentMode.Language) + "</h2>");
			            }
			            else
			            {
							Output.WriteLine("<h2 id=\"subcolls\">" + translator.Get_Translation(childTypes, currentMode.Language) + "</h2>");
			            }

						Output.WriteLine("<table id=\"sbkAghsw_CollectionButtonTbl\">");
			            int column_spot = 0;
			            Output.WriteLine("  <tr>");

			            foreach (string thisHtml in html_list)
			            {
				            if (column_spot == 3)
				            {
					            Output.WriteLine("  </tr>");
					            Output.WriteLine("  <tr>");
					            column_spot = 0;
				            }

				            Output.WriteLine(thisHtml);
				            column_spot++;
			            }

			            if (column_spot == 2)
			            {
							Output.WriteLine("    <td>&nbsp;</td>");
			            }
			            if (column_spot == 1)
			            {
							Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			            }
			            Output.WriteLine("  </tr>");
			            Output.WriteLine("</table>");

			            // Restore the old alias
			            currentMode.Aggregation_Alias = lastAlias;
		            }
	            }
                currentMode.Aggregation = Current_Aggregation.Code;
            }
            else
            {
                if ((currentMode.Home_Type != Home_Type_Enum.Personalized) && (currentMode.Home_Type != Home_Type_Enum.Partners_List) && (currentMode.Home_Type != Home_Type_Enum.Partners_Thumbnails))
                {
					// SHould this person be able to edit this page?
	                bool isAdmin = (currentUser != null) && ((currentUser.Is_System_Admin) || (currentUser.Is_Portal_Admin));
					if ((isAdmin) && (SobekCM_Library_Settings.Additional_Settings.ContainsKey("Portal Admins Can Edit Home Page")))
					{
						if (SobekCM_Library_Settings.Additional_Settings["Portal Admins Can Edit Home Page"].ToUpper().Trim() == "FALSE")
						{
							isAdmin = currentUser.Is_System_Admin;
						}
					}

					// This is the main home page, so call one of the special functions to draw the home
                    // page types ( i.e., icon view, brief view, or tree view )
                    string sobekcm_home_page_text;
                    object sobekcm_home_page_obj = HttpContext.Current.Application["SobekCM_Home"];
                    if (sobekcm_home_page_obj == null)
                    {
                        if (Tracer != null)
                        {
                            Tracer.Add_Trace("Aggregation_HtmlSubwriter.add_home_html", "Reading main library home text source file");
                        }

                        sobekcm_home_page_text = Current_Aggregation.Get_Home_HTML(currentMode.Language, Tracer);

                        HttpContext.Current.Application["SobekCM_Home"] = sobekcm_home_page_text;
                    }
                    else
                    {
                        sobekcm_home_page_text = (string)sobekcm_home_page_obj;
                    }

	                if ((isAdmin) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit))
	                {
		                string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());
		                Output.WriteLine("<form name=\"home_edit_form\" method=\"post\" action=\"" + post_url + "\" id=\"addedForm\" >");
		                Output.WriteLine("  <textarea id=\"sbkAghsw_HomeTextEdit\" name=\"sbkAghsw_HomeTextEdit\" >");
		                Output.WriteLine(sobekcm_home_page_text);
		                Output.WriteLine("  </textarea>");
		                Output.WriteLine();

		                Output.WriteLine("<div id=\"sbkAghsw_HomeEditButtons\">");
		                currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
		                Output.WriteLine("  <button title=\"Do not apply changes\" class=\"roundbutton\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"roundbutton_img_left\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
		                Output.WriteLine("  <button title=\"Save changes to this aggregation home page text\" class=\"roundbutton\" type=\"submit\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
		                Output.WriteLine("</div>");
		                Output.WriteLine("</form>");
						Output.WriteLine("<br /><br /><br />");
		                Output.WriteLine();

	                }
	                else
	                {

		                int index = sobekcm_home_page_text.IndexOf("<%END%>");

		                // Determine the different counts as strings
		                string page_count = Int_To_Comma_String(Current_Aggregation.Page_Count);
		                string item_count = Int_To_Comma_String(Current_Aggregation.Item_Count);
		                string title_count = Int_To_Comma_String(Current_Aggregation.Title_Count);

		                string url_options = currentMode.URL_Options();
		                string urlOptions1 = String.Empty;
		                string urlOptions2 = String.Empty;
		                if (url_options.Length > 0)
		                {
			                urlOptions1 = "?" + url_options;
			                urlOptions2 = "&" + url_options;
		                }

						string adjusted_home = index > 0 ? sobekcm_home_page_text.Substring(0, index).Replace("<%BASEURL%>", currentMode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%INTERFACE%>", currentMode.Base_Skin).Replace("<%WEBSKIN%>", currentMode.Base_Skin).Replace("<%PAGES%>", page_count).Replace("<%ITEMS%>", item_count).Replace("<%TITLES%>", title_count)
			                                 : sobekcm_home_page_text.Replace("<%BASEURL%>", currentMode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%INTERFACE%>", currentMode.Base_Skin).Replace("<%WEBSKIN%>", currentMode.Base_Skin).Replace("<%PAGES%>", page_count).Replace("<%ITEMS%>", item_count).Replace("<%TITLES%>", title_count);


						// Output the adjusted home html
						if (isAdmin)
						{
							Output.WriteLine("<div id=\"sbkAghsw_EditableHome\">");
							Output.WriteLine(adjusted_home);
							currentMode.Aggregation_Type = Aggregation_Type_Enum.Home_Edit;
							Output.WriteLine("  <div id=\"sbkAghsw_EditableHomeLink\"><a href=\"" + currentMode.Redirect_URL() + "\" title=\"Edit this home text\"><img src=\"" + currentMode.Base_URL + "default/images/edit.gif\" alt=\"\" />edit content</a></div>");
							currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
							Output.WriteLine("</div>");
							Output.WriteLine();

							Output.WriteLine("<script>");
							Output.WriteLine("  $(\"#sbkAghsw_EditableHome\").mouseover(function() { $(\"#sbkAghsw_EditableHomeLink\").css(\"display\",\"inline-block\"); });");
							Output.WriteLine("  $(\"#sbkAghsw_EditableHome\").mouseout(function() { $(\"#sbkAghsw_EditableHomeLink\").css(\"display\",\"none\"); });");
							Output.WriteLine("</script>");
							Output.WriteLine();
						}
						else
						{
							Output.WriteLine("<div id=\"sbkAghsw_Home\">");
							Output.WriteLine(adjusted_home);
							Output.WriteLine("</div>");
						}
	                }
                }
 

                if ((currentMode.Home_Type == Home_Type_Enum.Partners_List) || (currentMode.Home_Type == Home_Type_Enum.Partners_Thumbnails))
                {
					Output.WriteLine("<br />");
                    Output.WriteLine("<p>Partners collaborating and contributing to digital collections and libraries include:</p>");
                }

                if (currentMode.Home_Type != Home_Type_Enum.Personalized)
                {
                    // See if there are actually aggregations linked to the  thematic headings
                    bool aggrsLinkedToThemes = false;
                    if ((!SobekCM_Library_Settings.Include_TreeView_On_System_Home) && ( thematicHeadings.Count > 0 ))
                    {
                        if (thematicHeadings.Any(ThisTheme => codeManager.Aggregations_By_ThemeID(ThisTheme.ThematicHeadingID).Count > 0))
                        {
                            aggrsLinkedToThemes = true;
                        } 
                    }

                    // If aggregations are linked to themes, or if the tree view should always be displayed on home
                    if ((aggrsLinkedToThemes) || (SobekCM_Library_Settings.Include_TreeView_On_System_Home))
                    {
                        string listText = "LIST VIEW";
                        string descriptionText = "BRIEF VIEW";
                        string treeText = "TREE VIEW";
                        const string THUMBNAIL_TEXT = "THUMBNAIL VIEW";

                        if (currentMode.Language == Web_Language_Enum.Spanish)
                        {
                            listText = "LISTADO";
                            descriptionText = "VISTA BREVE";
                            treeText = "JERARQUIA";
                        }

						Output.WriteLine("<ul class=\"sbk_FauxUpwardTabsList\" id=\"sbkAghsw_HomeTypeLinks\">");

                        Home_Type_Enum startHomeType = currentMode.Home_Type;

                        if ((startHomeType != Home_Type_Enum.Partners_List) && (startHomeType != Home_Type_Enum.Partners_Thumbnails))
                        {
	                        if (thematicHeadings.Count > 0)
	                        {
		                        if (startHomeType == Home_Type_Enum.List)
		                        {
									Output.WriteLine("  <li class=\"current\">" + listText + "</li>");
		                        }
		                        else
		                        {
			                        currentMode.Home_Type = Home_Type_Enum.List;
			                        Output.WriteLine("  <li><a href=\"" + currentMode.Redirect_URL() + "\">" + listText + "</a></li>");
		                        }

		                        if (startHomeType == Home_Type_Enum.Descriptions)
		                        {
									Output.WriteLine("  <li class=\"current\">" + descriptionText + "</li>");
		                        }
		                        else
		                        {
			                        currentMode.Home_Type = Home_Type_Enum.Descriptions;
			                        Output.WriteLine("  <li><a href=\"" + currentMode.Redirect_URL() + "\">" + descriptionText + "</a></li>");
		                        }


		                        if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
		                        {
			                        if ((startHomeType == Home_Type_Enum.Tree_Collapsed) || (startHomeType == Home_Type_Enum.Tree_Expanded))
			                        {
										Output.WriteLine("  <li class=\"current\">" + treeText + "</li>");
			                        }
			                        else
			                        {
				                        currentMode.Home_Type = Home_Type_Enum.Tree_Collapsed;
				                        Output.WriteLine("  <li><a href=\"" + currentMode.Redirect_URL() + "\">" + treeText + "</a></li>");
			                        }
		                        }
	                        }
                        }
                        else
                        {
                            if (startHomeType == Home_Type_Enum.Partners_List)
                            {
								Output.WriteLine("  <li class=\"current\">" + listText + "</li>");
                            }
                            else
                            {
                                currentMode.Home_Type = Home_Type_Enum.Partners_List;
                                Output.WriteLine("  <li><a href=\"" + currentMode.Redirect_URL() + "\">" + listText + "</a></li>" );
                            }

                            if (startHomeType == Home_Type_Enum.Partners_Thumbnails)
                            {
								Output.WriteLine("  <li class=\"current\">" + THUMBNAIL_TEXT + "</li>");
                            }
                            else
                            {
                                currentMode.Home_Type = Home_Type_Enum.Partners_Thumbnails;
                                Output.WriteLine("  <li><a href=\"" + currentMode.Redirect_URL() + "\">" + THUMBNAIL_TEXT + "</a></li>" );
                            }

							if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
							{
								if ((startHomeType == Home_Type_Enum.Tree_Collapsed) || (startHomeType == Home_Type_Enum.Tree_Expanded))
								{
									Output.WriteLine("  <li class=\"current\">" + treeText + "</li>");
								}
								else
								{
									currentMode.Home_Type = Home_Type_Enum.Tree_Collapsed;
									Output.WriteLine("  <li><a href=\"" + currentMode.Redirect_URL() + "\">" + treeText + "</a></li>");
								}
							}
                        }
                        currentMode.Home_Type = startHomeType;

                        Output.WriteLine("</ul>");
                        Output.WriteLine();
                    }
                }

                switch (currentMode.Home_Type)
                {
                    case Home_Type_Enum.List:
                        write_list_home(Output, Tracer);
                        break;

                    case Home_Type_Enum.Descriptions:
                        write_description_home(Output, Tracer);
                        break;

                    case Home_Type_Enum.Personalized:
                        write_personalized_home(Output, Tracer);
                        break;

                    case Home_Type_Enum.Partners_List:
                        write_institution_list(Output, Tracer);
                        break;

                    case Home_Type_Enum.Partners_Thumbnails:
                        write_institution_icons(Output, Tracer);
                        break;

                    case Home_Type_Enum.Tree_Expanded:
                    case Home_Type_Enum.Tree_Collapsed:
						Output.WriteLine("</div>");
                        return false;
                }
            }

			Output.WriteLine("</div>");
            return true;
        }

        private string Int_To_Comma_String(int Value)
        {
            if (Value < 1000)
                return Value.ToString();

            string value_string = Value.ToString();
            if ((Value >= 1000) && (Value < 1000000))
            {
                return value_string.Substring(0, value_string.Length - 3) + "," + value_string.Substring(value_string.Length - 3);
            }

            return value_string.Substring(0, value_string.Length - 6) + "," + value_string.Substring(value_string.Length - 6, 3) + "," + value_string.Substring(value_string.Length - 3);
        }

        #region Main Home Page Methods

        #region Method to create the tree view

        private void Create_TreeView_From_Collections(TreeView TreeView1)
        {
            // Save the current home type
            TreeNode rootNode = new TreeNode("Collection Hierarchy") {SelectAction = TreeNodeSelectAction.None};
            TreeView1.Nodes.Add(rootNode);

	        string currentSkin = currentMode.Skin;

            // Step through each node under this
            SortedList<string, TreeNode> sorted_node_list = new SortedList<string, TreeNode>();
            foreach (Item_Aggregation_Related_Aggregations childAggr in Current_Aggregation.Children)
            {
                if ((!childAggr.Hidden) && ( childAggr.Active ))
                {
                    // Set the aggregation value, for the redirect URL
                    currentMode.Aggregation = childAggr.Code.ToLower();

                    // Set some default interfaces
                    if (currentMode.Aggregation == "dloc1")
                        currentMode.Skin = "dloc";
                    if (currentMode.Aggregation == "edlg")
                        currentMode.Skin = "edl";

                    // Create this tree node
                    TreeNode childNode = new TreeNode("<a href=\"" + currentMode.Redirect_URL() + "\"><abbr title=\"" + childAggr.Description + "\">" + childAggr.Name + "</abbr></a>");
                    if (currentMode.Internal_User)
                    {
                        childNode.Text = string.Format("<a href=\"{0}\"><abbr title=\"{1}\">{2} ( {3} )</abbr></a>", currentMode.Redirect_URL(), childAggr.Description, childAggr.Name, childAggr.Code);
                    }
                    childNode.SelectAction = TreeNodeSelectAction.None;
                    childNode.NavigateUrl = currentMode.Redirect_URL();

                    // Add to the sorted list
                    if ((childAggr.Name.Length > 4) && (childAggr.Name.IndexOf("The ") == 0 ))
                        sorted_node_list.Add(childAggr.Name.Substring(4).ToUpper(), childNode);
                    else
                        sorted_node_list.Add(childAggr.Name.ToUpper(), childNode);

                    // Check the children nodes recursively
                    add_children_to_tree(childAggr, childNode);

                    currentMode.Skin = String.Empty;
                }
            }

            // Now add the sorted nodes to the tree
            foreach( TreeNode childNode in sorted_node_list.Values )
            {
                rootNode.ChildNodes.Add(childNode);
            }

            currentMode.Aggregation = String.Empty;

            if ((currentMode.Home_Type == Home_Type_Enum.Tree_Expanded) || ( currentMode.Is_Robot ))
            {
                TreeView1.ExpandAll();
            }
            else
            {
                TreeView1.CollapseAll();
                rootNode.Expand();
            }

	        currentMode.Skin = currentSkin;
        }

        private void add_children_to_tree(Item_Aggregation_Related_Aggregations Aggr, TreeNode Node)
        {
            // Step through each node under this
            SortedList<string, TreeNode> sorted_node_list = new SortedList<string, TreeNode>();
            foreach (Item_Aggregation_Related_Aggregations childAggr in Aggr.Children)
            {
                if ((!childAggr.Hidden) && ( childAggr.Active ))
                {
                    // Set the aggregation value, for the redirect URL
                    currentMode.Aggregation = childAggr.Code.ToLower();

                    // Create this tree node
                    TreeNode childNode = new TreeNode("<a href=\"" + currentMode.Redirect_URL() + "\"><abbr title=\"" + childAggr.Description + "\">" + childAggr.Name + "</abbr></a>");
                    if (currentMode.Internal_User)
                    {
                        childNode.Text = string.Format("<a href=\"{0}\"><abbr title=\"{1}\">{2} ( {3} )</abbr></a>", currentMode.Redirect_URL(), childAggr.Description, childAggr.Name, childAggr.Code);
                    }
                    childNode.SelectAction = TreeNodeSelectAction.None;
                    childNode.NavigateUrl = currentMode.Redirect_URL();

                    // Add to the sorted list
                    if ((childAggr.Name.Length > 4) && (childAggr.Name.IndexOf("The ") == 0))
                        sorted_node_list.Add(childAggr.Name.Substring(4).ToUpper(), childNode);
                    else
                        sorted_node_list.Add(childAggr.Name.ToUpper(), childNode);

                    // Check the children nodes recursively
                    add_children_to_tree(childAggr, childNode); 
                }
            }

            // Now add the sorted nodes to the tree
            foreach (TreeNode childNode in sorted_node_list.Values)
            {
                Node.ChildNodes.Add(childNode);
            }
        }

        #endregion

        #region Method to create the descriptive home page

        /// <summary> Adds the main library home page with short descriptions about each highlighted item aggregation</summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        protected internal void write_description_home(TextWriter Output, Custom_Tracer Tracer)
        {
            // If the thematic headings were null, get it now
            if (thematicHeadings == null)
            {
                thematicHeadings = new List<Thematic_Heading>();
                SobekCM_Database.Populate_Thematic_Headings(thematicHeadings, Tracer);
            }

            // Step through each thematic heading and add all the needed aggreagtions
	        bool first = true;
            foreach (Thematic_Heading thisTheme in thematicHeadings)
            {
                // Build the list of html to display, first adding collections and subcollections
                SortedList<string, string> html_list = new SortedList<string, string>();
                ReadOnlyCollection<Item_Aggregation_Related_Aggregations> thisThemesAggrs = codeManager.Aggregations_By_ThemeID(thisTheme.ThematicHeadingID);
                foreach (Item_Aggregation_Related_Aggregations thisAggr in thisThemesAggrs)
                {
                    string image_url = currentMode.Base_URL + "design/aggregations/" + thisAggr.Code + "/images/buttons/coll.gif";
                    currentMode.Aggregation = thisAggr.Code.ToLower();

                    if (currentMode.Aggregation == "dloc1")
                        currentMode.Skin = "dloc";
                    if (currentMode.Aggregation == "edlg")
                        currentMode.Skin = "edl";

                    if (thisAggr.Name.IndexOf("The ") == 0)
                    {
						html_list[thisAggr.Name.Substring(4)] = "    <td class=\"sbkAghsw_CollectionDescription\">" + Environment.NewLine + "      <br /><span class=\"sbkAghsw_CollectionDesciptionImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>" + Environment.NewLine + "      <p>" + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</p></span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";
                    }
                    else
                    {
						html_list[thisAggr.Name] = "   <td class=\"sbkAghsw_CollectionDescription\"" + Environment.NewLine + "      <br /><span class=\"sbkAghsw_CollectionDesciptionImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>" + Environment.NewLine + "      <p>" + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</p></span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";

                        if (thisAggr.Code == "EPC")
                        {
							html_list[thisAggr.Name] = "    <td class=\"sbkAghsw_CollectionDescription\"" + Environment.NewLine + "      <br /><span class=\"sbkAghsw_CollectionDesciptionImg\"><a href=\"http://www.uflib.ufl.edu/epc/\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>" + Environment.NewLine + "      <p>" + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</p></span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";
                        }
                        if (thisAggr.Code == "UFHERB")
                        {
							html_list[thisAggr.Name] = "    <td class=\"sbkAghsw_CollectionDescription\"" + Environment.NewLine + "      <br /><span class=\"sbkAghsw_CollectionDesciptionImg\"><a href=\"http://www.flmnh.ufl.edu/natsci/herbarium/cat/\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>" + Environment.NewLine + "      <p>" + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</p></span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";
                        }
                        if (thisAggr.Code == "EXHIBITMATERIALS")
                        {
                            currentMode.Aggregation = "exhibits";
							html_list[thisAggr.Name] = "    <td class=\"sbkAghsw_CollectionDescription\"" + Environment.NewLine + "      <br /><span class=\"sbkAghsw_CollectionDesciptionImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>" + Environment.NewLine + "      <p>" + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</p></span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";
                        }
                    }

                    currentMode.Skin = String.Empty;
                }

                if (html_list.Count > 0)
                {
                    // Write this theme
					if (first)
					{
						Output.WriteLine("<h2 style=\"margin-top:0;margin-bottom:0;\">" + thisTheme.ThemeName + "</h2>");
						first = false;
					}
					else
						Output.WriteLine("<h2 style=\"margin-bottom:0;\">" + thisTheme.ThemeName + "</h2>");

					Output.WriteLine("<table id=\"sbkAghsw_CollectionDescriptionTbl\">");
                    int column_spot = 0;
                    Output.WriteLine("  <tr>");

                    foreach (string thisHtml in html_list.Values)
                    {
                        if (column_spot == 2)
                        {
                            Output.WriteLine("  </tr>");
                            Output.WriteLine("  <tr>");
                            column_spot = 0;
                        }

                        if (column_spot == 1)
                        {
                            Output.WriteLine("    <td style=\"width:15px;\"></td>");
                            Output.WriteLine("    <td style=\"width:1px;background-color:#cccccc;\"></td>");
							Output.WriteLine("    <td style=\"width:15px;\"></td>");
                        }


                        Output.WriteLine(thisHtml);
                        column_spot++;
                    }

                    if (column_spot == 1)
                    {
						Output.WriteLine("    <td style=\"width:15px;\"></td>");
						Output.WriteLine("    <td style=\"width:1px;background-color:#cccccc;\"></td>");
						Output.WriteLine("    <td style=\"width:15px;\"></td>");
						Output.WriteLine("    <td class=\"sbkAghsw_CollectionDescription\">&nbsp;</td>");
                    }

                    Output.WriteLine("  </tr>");
                }
                Output.WriteLine("</table>");
                Output.WriteLine("<br />");
            }

            currentMode.Aggregation = String.Empty;
        }

        #endregion

        #region Method to show the icon list

        /// <summary> Adds the main library home page with icons and names about each highlighted item aggregation</summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        protected internal void write_list_home(TextWriter Output, Custom_Tracer Tracer)
        {
            // If the thematic headings were null, get it now
            if (thematicHeadings == null)
            {
                thematicHeadings = new List<Thematic_Heading>();
                SobekCM_Database.Populate_Thematic_Headings(thematicHeadings, Tracer);
            }

            // Step through each thematic heading and add all the needed aggreagtions
	        bool first = true;
            foreach (Thematic_Heading thisTheme in thematicHeadings)
            {
                // Build the list of html to display
                SortedList<string, string> html_list = new SortedList<string, string>();
                ReadOnlyCollection<Item_Aggregation_Related_Aggregations> thisThemesAggrs = codeManager.Aggregations_By_ThemeID(thisTheme.ThematicHeadingID);
                int aggreCount = -1;
                foreach (Item_Aggregation_Related_Aggregations thisAggr in thisThemesAggrs)
                {
                    aggreCount++;
                    string image_url = currentMode.Base_URL + "design/aggregations/" + thisAggr.Code + "/images/buttons/coll.gif";
                    string thisDescription = thisAggr.Description;
                    currentMode.Aggregation = thisAggr.Code.ToLower();

                    if (currentMode.Aggregation == "dloc1")
                        currentMode.Skin = "dloc";
                    if (currentMode.Aggregation == "edlg")
                        currentMode.Skin = "edl";

                    string hoverHiddenDivTitle = "<div id=\"hoverDivTitle\" style=\"display:none\" class=\"tooltipTitle\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</div>";
                    string hoverHiddenDiv = "<div id=\"hoverDiv\" style=\"display:none\" class=\"tooltipText\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" />"+thisDescription+"</div>";

                    if (thisAggr.Name.IndexOf("The ") == 0)
                    {
                        html_list[thisAggr.Name.Substring(4)] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\" class=\"sbkAghsw_CollectionButtonImg\"><span class=\"spanHoverText\" style=\"display:none\">"+thisDescription+"</span><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>" + Environment.NewLine + "   </span> " + hoverHiddenDivTitle + hoverHiddenDiv + "</td>";
                        //html_list[thisAggr.Name.Substring(4)] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\" ><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span><span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a>"+thisDescription+"</span>" + Environment.NewLine + "  </td>";
                    }
                    else
                    {
                        html_list[thisAggr.Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\" class=\"sbkAghsw_CollectionButtonImg\"><span class=\"spanHoverText\" style=\"display:none\">" + thisDescription + "</span><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "  </td>";

                        if (thisAggr.Code == "EPC")
                        {
                            html_list[thisAggr.Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\" class=\"sbkAghsw_CollectionButtonImg\"><span class=\"spanHoverText\" style=\"display:none\">" + thisDescription + "</span><a href=\"http://www.uflib.ufl.edu/epc/\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                        if (thisAggr.Code == "UFHERB")
                        {
                            html_list[thisAggr.Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\" class=\"sbkAghsw_CollectionButtonImg\"><span class=\"spanHoverText\" style=\"display:none\">" + thisDescription + "</span><a href=\"http://www.flmnh.ufl.edu/natsci/herbarium/cat/\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                        if (thisAggr.Code == "EXHIBITMATERIALS")
                        {
                            currentMode.Aggregation = "exhibits";
                            html_list[thisAggr.Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\" class=\"sbkAghsw_CollectionButtonImg\"><span class=\"spanHoverText\" style=\"display:none\">" + thisDescription + "</span><a href=\"" + currentMode.Base_URL + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                    }

                    currentMode.Skin = String.Empty;
                }

                if (html_list.Count > 0)
                {
                    // Write this theme
					if (first)
					{
						Output.WriteLine("<h2 style=\"margin-top:0;\">" + translator.Get_Translation(thisTheme.ThemeName, currentMode.Language) + "</h2>");
						first = false;
					}
	                else
						Output.WriteLine("<h2>" + translator.Get_Translation(thisTheme.ThemeName, currentMode.Language) + "</h2>");

					Output.WriteLine("<table id=\"sbkAghsw_CollectionButtonTbl\">");
                    int column_spot = 0;
                    Output.WriteLine("  <tr>");

                    foreach (string thisHtml in html_list.Values)
                    {
                        if (column_spot == 3)
                        {
                            Output.WriteLine("  </tr>");
                            Output.WriteLine("  <tr>");
                            column_spot = 0;
                        }

                        Output.WriteLine(thisHtml);
                        column_spot++;
                    }

                    if (column_spot == 2)
                    {
						Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
                    }
                    if (column_spot == 1)
                    {
						Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
						Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
                    }
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("</table>");
                }

                Output.WriteLine("<br />");
            }

            currentMode.Aggregation = String.Empty;
        }

        #endregion

        #region Method to show the personalized home page

        /// <summary> Adds the personalized main library home page for logged on users</summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        protected internal void write_personalized_home(TextWriter Output, Custom_Tracer Tracer)
        {
            // Build the list of html to display, first adding collections and subcollections
            SortedList<string, string> html_list = new SortedList<string, string>();

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.10.2.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.qtip.min.js\"></script>");
            //NOTE: The jquery.hovercard.min.js file included below has been modified for SobekCM, and also includes bug fixes. DO NOT REPLACE with another version
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.hovercard.min.js\"></script>");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.qtip.min.css\" /> ");
            

            foreach (User_Editable_Aggregation thisAggregation in currentUser.Aggregations.Where(ThisAggregation => ThisAggregation.OnHomePage))
            {
                currentMode.Aggregation = thisAggregation.Code.ToLower();
                string image_url = currentMode.Base_URL + "design/aggregations/" + thisAggregation.Code + "/images/buttons/coll.gif";

                if ((thisAggregation.Name.IndexOf("The ") == 0) && (thisAggregation.Name.Length > 4))
                {
					html_list[thisAggregation.Name.Substring(4)] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('" + thisAggregation.Code + "');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";
                }
                else
                {
					html_list[thisAggregation.Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('" + thisAggregation.Code + "');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";

                    if (thisAggregation.Code == "EPC")
                    {
						html_list[thisAggregation.Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"http://www.uflib.ufl.edu/epc/\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('" + thisAggregation.Code + "');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";
                    }
                    if (thisAggregation.Code == "UFHERB")
                    {
						html_list[thisAggregation.Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"http://www.flmnh.ufl.edu/natsci/herbarium/cat/\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('" + thisAggregation.Code + "');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";
                    }
                    if (thisAggregation.Code == "EXHIBITMATERIALS")
                    {
                        currentMode.Aggregation = "exhibits";
						html_list[thisAggregation.Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('EXHIBITMATERIALS');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";
                    }
                }
            }

            // Write this theme
			Output.WriteLine("<br />");
            Output.WriteLine("<p>Welcome to your personalized " + currentMode.SobekCM_Instance_Abbreviation + " home page.  This page displays any collections you have added, as well as any of your bookshelves you have made public.</p>");
			Output.WriteLine("<h2>My Collections</h2>");

            // If there were any saves collections, show them here
            if (html_list.Count > 0)
            {
				Output.WriteLine("<table id=\"sbkAghsw_CollectionButtonTbl\">");
                int column_spot = 0;
                Output.WriteLine("  <tr>");

                foreach (string thisHtml in html_list.Values)
                {
                    if (column_spot == 3)
                    {
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr>");
                        column_spot = 0;
                    }

                    Output.WriteLine(thisHtml);
                    column_spot++;
                }

				if (column_spot == 2)
				{
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
				}
				if (column_spot == 1)
				{
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
				}
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");
                Output.WriteLine("<br />");
            }
            else
            {
                Output.WriteLine("<p>You do not have any collections added to your home page.<p>");
                Output.WriteLine("<p>To add a collection, use the <img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/add_rect_button.gif\" alt=\"ADD\" /> button from that collection's home page.</p>");
            }

            // Were there any public folders
            SortedList<string, string> public_folder_list = new SortedList<string, string>();
            currentMode.Mode = Display_Mode_Enum.Public_Folder;
            currentMode.Result_Display_Type = Result_Display_Type_Enum.Brief;
            currentMode.Aggregation = String.Empty;
            foreach (User_Folder thisFolder in currentUser.All_Folders.Where(ThisFolder => ThisFolder.isPublic))
            {
                currentMode.FolderID = thisFolder.Folder_ID;
                if ((thisFolder.Folder_Name.IndexOf("The ") == 0) && (thisFolder.Folder_Name.Length > 4))
                {
					public_folder_list[thisFolder.Folder_Name.Substring(4)] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/closed_folder_public_big.jpg\" alt=\"" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return make_folder_private('" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "');\">make private</a> )</span></span>" + Environment.NewLine + "    </td>";
                }
                else
                {
					public_folder_list[thisFolder.Folder_Name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/closed_folder_public_big.jpg\" alt=\"" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return make_folder_private('" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "');\">make private</a> )</span></span>" + Environment.NewLine + "    </td>";
                }
            }
			currentMode.Mode = Display_Mode_Enum.Aggregation;
			currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;

            // if there were any public folders
            if (public_folder_list.Count > 0)
            {
                // Write this theme
				Output.WriteLine("<h2>My Public Bookshelves</h2>");

				Output.WriteLine("<table id=\"sbkAghsw_PublicBookshelvesTbl\">");
                int column_spot2 = 0;
                Output.WriteLine("  <tr>");

                foreach (string thisHtml in public_folder_list.Values)
                {
                    if (column_spot2 == 3)
                    {
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr>");
                        column_spot2 = 0;
                    }

                    Output.WriteLine(thisHtml);
                    column_spot2++;
                }

				if (column_spot2 == 2)
				{
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
				}
				if (column_spot2 == 1)
				{
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
				}
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");
                Output.WriteLine("<br />");
            }

            // Add some of the static links
			Output.WriteLine("<h2>My Links</h2>");
			Output.WriteLine("<table id=\"sbkAghsw_MyLinksTbl\">");
            Output.WriteLine("  <tr>");

            currentMode.Aggregation = String.Empty;
            currentMode.Mode = Display_Mode_Enum.My_Sobek;
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
			Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/home_button.gif\" alt=\"Go to my" + currentMode.SobekCM_Instance_Abbreviation + " home\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">my" + currentMode.SobekCM_Instance_Abbreviation + " Home</a></span>" + Environment.NewLine + "    </td>");

            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
			Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/big_bookshelf.gif\" alt=\"Go to my bookshelf\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">My Library</a></span>" + Environment.NewLine + "    </td>");

            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
			Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/saved_searches_big.gif\" alt=\"Go to my saved searches\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">My Saved Searches</a></span>" + Environment.NewLine + "    </td>");

			currentMode.Mode = Display_Mode_Enum.Aggregation;
			currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;

            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br />");

            currentMode.Aggregation = String.Empty;
        }

        #endregion

        #region Methods to show the institution home page

        /// <summary> Adds the partner institution page from the main library home page as small icons and html names </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        protected internal void write_institution_list(TextWriter Output, Custom_Tracer Tracer)
        {
            // Build the list of html to display, first adding collections and subcollections
            SortedList<string, string> html_list = new SortedList<string, string>();

            // Get the institutions
            ReadOnlyCollection<Item_Aggregation_Related_Aggregations> institutions = codeManager.Aggregations_By_Type("Institution");
            int aggreCount = -1;
            foreach (Item_Aggregation_Related_Aggregations thisAggr in institutions)
            {
                if ( thisAggr.Active )
                {
                    aggreCount++;
                    string name = thisAggr.ShortName.Replace("&", "&amp;").Replace("\"", "&quot;");
                    string description = thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;");
                    if (name.ToUpper() == "ADDED AUTOMATICALLY")
                        name = thisAggr.Code + " ( Added Automatically )";

                    if ((thisAggr.Code.Length > 2) && (thisAggr.Code[0] == 'I'))
                    {
                        currentMode.Aggregation = thisAggr.Code.ToLower();
                        string image_url = currentMode.Base_URL + "design/aggregations/" + thisAggr.Code + "/images/buttons/coll.gif";
                        if ((name.IndexOf("The ") == 0) && (name.Length > 4))
                        {
                            html_list[name.Substring(4)] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span id=\"sbkAghsw_CollectionButtonImg" + aggreCount + "\" class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + name + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + name + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                        else
                        {
							html_list[name] = "    <td class=\"sbkAghsw_CollectionButton\">" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonImg\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + name + "\" /></a></span>" + Environment.NewLine + "      <span class=\"sbkAghsw_CollectionButtonTxt\"><a href=\"" + currentMode.Redirect_URL() + "\">" + name + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                    }
                }
            }
            currentMode.Aggregation = String.Empty;

            if (html_list.Count > 0)
            {
                // Write this theme
				Output.WriteLine("<h2 style=\"margin-top:0;\">Partners and Contributing Institutions</h2>");

				Output.WriteLine("<table id=\"sbkAghsw_CollectionButtonTbl\">");
                int column_spot = 0;
                Output.WriteLine("  <tr>");

                foreach (string thisHtml in html_list.Values)
                {
                    if (column_spot == 3)
                    {
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr>");
                        column_spot = 0;
                    }

                    Output.WriteLine(thisHtml);
                    column_spot++;
                }

				if (column_spot == 2)
				{
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
				}
				if (column_spot == 1)
				{
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
				}
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");
            }

            Output.WriteLine("<br />");
        }

        /// <summary> Adds the partner institution page from the main library home page as large icons </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        protected internal void write_institution_icons(TextWriter Output, Custom_Tracer Tracer)
        {
            // Build the list of html to display, first adding collections and subcollections
            SortedList<string, string> html_list = new SortedList<string, string>();

            // Get the institutions
            ReadOnlyCollection<Item_Aggregation_Related_Aggregations> institutions = codeManager.Aggregations_By_Type("Institution");
            foreach (Item_Aggregation_Related_Aggregations thisAggr in institutions)
            {
                if (thisAggr.Active)
                {
                    string name = thisAggr.ShortName.Replace("&", "&amp;").Replace("\"", "&quot;");
                    if (name.ToUpper() == "ADDED AUTOMATICALLY")
                        name = thisAggr.Code + " ( Added Automatically )";

                    if ((thisAggr.Code.Length > 2) && (thisAggr.Code[0] == 'I'))
                    {
                        currentMode.Aggregation = thisAggr.Code.ToLower();
                        string image_url = currentMode.Base_URL + "design/aggregations/" + thisAggr.Code + "/images/buttons/" + thisAggr.Code.Substring(1) + ".gif";
                        html_list[name] = "    <td>" + Environment.NewLine + "      <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + "\"><img src=\"" + image_url + "\" alt=\"" + name + "\" /></a></td>";
                    }
                }
            }
            currentMode.Aggregation = String.Empty;

            if (html_list.Count > 0)
            {
                // Write this theme
				Output.WriteLine("<h2 style=\"margin-top:0;\">Partners and Contributing Institutions</h2>");

				Output.WriteLine("<table id=\"sbkAghsw_CollectionButtonTbl\">");
                int column_spot = 0;
                Output.WriteLine("  <tr style=\"text-align:center;vertical-align:middle;\">");

                foreach (string thisHtml in html_list.Values)
                {
                    if (column_spot == 4)
                    {
                        Output.WriteLine("  </tr>");
						Output.WriteLine("  <tr style=\"text-align:center;vertical-align:middle;\">");
                        column_spot = 0;
                    }

                    Output.WriteLine(thisHtml);
                    column_spot++;
                }

                if (column_spot == 3)
                {
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
                }
				if (column_spot == 2)
				{
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");

				}
				if (column_spot == 1)
				{
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
					Output.WriteLine("    <td class=\"sbkAghsw_CollectionButton\">&nbsp;</td>");
				}

                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");
            }

            Output.WriteLine("<br />");
        }

        #endregion

        #endregion

        #endregion

        /// <summary> Writes final HTML after all the forms </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
		        Output.WriteLine("<!-- Close the pagecontainer div -->");
		        Output.WriteLine("</div>");
		        Output.WriteLine();

	        // Add the scripts needed
#if DEBUG
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.draggable.js\"></script>");
#else
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.draggable.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.10.2.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.qtip.min.js\"></script>");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.qtip.min.css\" /> ");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.hovercard.min.js\"></script>");
            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_aggre.js\"></script>");
#endif
            Output.WriteLine();
        }

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		public override string Container_CssClass
		{
			get
			{
				switch (currentMode.Aggregation_Type)
				{
					case Aggregation_Type_Enum.Browse_By:
						return "container-facets";

					case Aggregation_Type_Enum.Browse_Map:
						return "container-inner1000";

                    case Aggregation_Type_Enum.Browse_Map_Beta:
                        return "container-inner1000";

					case Aggregation_Type_Enum.Private_Items:
						return "container-inner1215";

					case Aggregation_Type_Enum.Browse_Info:
						if (pagedResults != null)
						{
							return "container-facets";
						}
						break;
				}

				return base.Container_CssClass;
			}
		}
    }
}
