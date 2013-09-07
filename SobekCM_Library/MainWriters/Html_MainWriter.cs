#region Using directives

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.Items;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.SiteMap;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;
using SobekCM.Library.WebContent;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes the HTML response to a user's request </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Html_MainWriter : abstractMainWriter
    {
        // HTML specific objects
        private readonly Dictionary<string, string> aggregationAliases;
        private readonly Checked_Out_Items_List checkedItems;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly User_Object currentUser;
        private bool finishPageInAddFinalHtmlMethod;
        private readonly SobekCM_Skin_Object htmlSkin;
        private readonly Dictionary<string, Wordmark_Icon> iconList;
        private readonly IP_Restriction_Ranges ipRestrictionInfo;
        private readonly Item_Lookup_Object itemList;
        private readonly SobekCM_Items_In_Title itemsInTitle;
        private readonly Public_User_Folder publicFolder;
        private readonly Recent_Searches searchHistory;
        private readonly SobekCM_SiteMap siteMap;
        private readonly Statistics_Dates statsDateRange;
        private readonly List<Thematic_Heading> thematicHeadings;
        private readonly Language_Support_Info translator;
        private readonly Portal_List urlPortals;
        private readonly SobekCM_Skin_Collection webSkins;

        // Special HTML sub-writers that need to have some persistance between methods
        private abstractHtmlSubwriter subwriter;

        /// <summary> Constructor for a new instance of the Text_MainWriter class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
        /// <param name="Current_Item"> Current item to display </param>
        /// <param name="Current_Page"> Current page within the item</param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Item_List"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Stats_Date_Range"> Object contains the start and end dates for the statistical data in the database </param>
        /// <param name="Search_History"> List of recent searches performed against this digital library </param>
        /// <param name="Icon_Dictionary"> Dictionary of information about every wordmark/icon in this digital library, used to build the wordmarks subpage </param>
        /// <param name="Thematic_Headings"> Headings under which all the highlighted collections on the main home page are organized </param>
        /// <param name="Public_Folder"> Object contains the information about the public folder to display </param>
        /// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations </param>
        /// <param name="Web_Skin_Collection"> Collection of all the web skins </param>
        /// <param name="Checked_Items"> List of all items which are currently checked out for single fair use and the IP address currently viewing the item</param>
        /// <param name="IP_Restrictions"> Any possible restriction on item access by IP ranges </param>
        /// <param name="URL_Portals"> List of all web portals into this system </param>
        /// <param name="Site_Map"> Optional site map object used to render a navigational tree-view on left side of static web content pages </param>
        /// <param name="Items_In_Title"> List of items within the current title ( used for the Item Group display )</param>
        /// <param name="Static_Web_Content"> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained </param>
        public Html_MainWriter(SobekCM_Navigation_Object Current_Mode,
            Item_Aggregation Hierarchy_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Item_Aggregation_Browse_Info Browse_Object,
            SobekCM_Item Current_Item,
            Page_TreeNode Current_Page,
            SobekCM_Skin_Object HTML_Skin,
            User_Object Current_User,
            Language_Support_Info Translator,
            Aggregation_Code_Manager Code_Manager,
            Item_Lookup_Object Item_List,
            Statistics_Dates Stats_Date_Range,
            Recent_Searches Search_History,
            Dictionary<string, Wordmark_Icon> Icon_Dictionary,
            List<Thematic_Heading> Thematic_Headings,
            Public_User_Folder Public_Folder,
            Dictionary<string, string> Aggregation_Aliases,
            SobekCM_Skin_Collection Web_Skin_Collection,
            Checked_Out_Items_List Checked_Items,
            IP_Restriction_Ranges IP_Restrictions,
            Portal_List URL_Portals,
            SobekCM_SiteMap Site_Map,
            SobekCM_Items_In_Title Items_In_Title,
            HTML_Based_Content Static_Web_Content,
            Custom_Tracer Tracer )
            : base(Current_Mode, Hierarchy_Object, Results_Statistics, Paged_Results, Browse_Object,  Current_Item, Current_Page, Static_Web_Content)
        {
            // Save parameters
            htmlSkin = HTML_Skin;
            translator = Translator;
            codeManager = Code_Manager;
            itemList = Item_List;
            statsDateRange = Stats_Date_Range;
            searchHistory = Search_History;
            currentUser = Current_User;
            iconList = Icon_Dictionary;
            thematicHeadings = Thematic_Headings;
            publicFolder = Public_Folder;
            aggregationAliases = Aggregation_Aliases;
            webSkins = Web_Skin_Collection;
            checkedItems = Checked_Items;
            ipRestrictionInfo = IP_Restrictions;
            urlPortals = URL_Portals;
            siteMap = Site_Map;
            itemsInTitle = Items_In_Title;

            // Set some defaults
            finishPageInAddFinalHtmlMethod = false;

            // Handle basic events which may be fired by the internal header

            if (HttpContext.Current.Request.Form["internal_header_action"] != null)
            {
                // Pull the action value
                string internalHeaderAction = HttpContext.Current.Request.Form["internal_header_action"].Trim();

                // Was this to hide or show the header?
                if ((internalHeaderAction == "hide") || (internalHeaderAction == "show"))
                {
                    // Pull the current visibility from the session
                    bool shown = !((HttpContext.Current.Session["internal_header"] != null) && (HttpContext.Current.Session["internal_header"].ToString() == "hidden"));
                    if ((internalHeaderAction == "hide") && (shown))
                    {
                        HttpContext.Current.Session["internal_header"] = "hidden";
                        currentMode.Redirect();
                        return;
                    }
                    if ((internalHeaderAction == "show") && (!shown))
                    {
                        HttpContext.Current.Session["internal_header"] = "shown";
                        currentMode.Redirect();
                        return;
                    }
                }
            }

            // Create the html sub writer now
            switch (Current_Mode.Mode)
            {
                case Display_Mode_Enum.Internal:
                    subwriter = new Internal_HtmlSubwriter(iconList, currentUser, codeManager);
                    break;

                case Display_Mode_Enum.Statistics:
                    subwriter = new Statistics_HtmlSubwriter(searchHistory, codeManager, statsDateRange);
                    break;

                case Display_Mode_Enum.Preferences:
                    subwriter = new Preferences_HtmlSubwriter(currentMode);
                    break;

                case Display_Mode_Enum.Error:
                    subwriter = new Error_HtmlSubwriter(false);
                    // Send the email now
                    if (currentMode.Caught_Exception != null)
                    {
                        if (currentMode.Error_Message.Length == 0)
                            currentMode.Error_Message = "Unknown exception caught";
                        Email_Information(currentMode.Error_Message, currentMode.Caught_Exception, Tracer, false);
                    }
                    break;

                case Display_Mode_Enum.Legacy_URL:
                    subwriter = new LegacyUrl_HtmlSubwriter();
                    break;

                case Display_Mode_Enum.Item_Print:
                    subwriter = new Print_Item_HtmlSubwriter(currentItem, codeManager, translator, currentMode);
                    break;

                case Display_Mode_Enum.Contact:


                    StringBuilder builder = new StringBuilder();
                    builder.Append("\n\nSUBMISSION INFORMATION\n");
                    builder.Append("\tDate:\t\t\t\t" + DateTime.Now.ToString() + "\n");
                    string lastMode = String.Empty;
                    try
                    {
                        if (HttpContext.Current.Session["Last_Mode"] != null)
                            lastMode = HttpContext.Current.Session["Last_Mode"].ToString();
                        builder.Append("\tIP Address:\t\t\t" + HttpContext.Current.Request.UserHostAddress + "\n");
                        builder.Append("\tHost Name:\t\t\t" + HttpContext.Current.Request.UserHostName + "\n");
                        builder.Append("\tBrowser:\t\t\t" + HttpContext.Current.Request.Browser.Browser + "\n");
                        builder.Append("\tBrowser Platform:\t\t" + HttpContext.Current.Request.Browser.Platform + "\n");
                        builder.Append("\tBrowser Version:\t\t" + HttpContext.Current.Request.Browser.Version + "\n");
                        builder.Append("\tBrowser Language:\t\t");
                        bool first = true;
                        string[] languages = HttpContext.Current.Request.UserLanguages;
                        if (languages != null)
                            foreach (string thisLanguage in languages)
                            {
                                if (first)
                                {
                                    builder.Append(thisLanguage);
                                    first = false;
                                }
                                else
                                {
                                    builder.Append(", " + thisLanguage);
                                }
                            }

                        builder.Append("\n\nUFDC HISTORY\n");
                        if (HttpContext.Current.Session["LastSearch"] != null)
                            builder.Append("\tLast Search:\t\t" + HttpContext.Current.Session["LastSearch"] + "\n");
                        if (HttpContext.Current.Session["LastResults"] != null)
                            builder.Append("\tLast Results:\t\t" + HttpContext.Current.Session["LastResults"] + "\n");
                        if (HttpContext.Current.Session["Last_Mode"] != null)
                            builder.Append("\tLast Mode:\t\t\t" + HttpContext.Current.Session["Last_Mode"] + "\n");
                        builder.Append("\tURL:\t\t\t\t" + HttpContext.Current.Items["Original_URL"]);
                    }
                    catch (Exception ee)
                    {

                    }
                    subwriter = new Contact_HtmlSubwriter(lastMode, builder.ToString(), currentMode, hierarchyObject);
                    break;


                case Display_Mode_Enum.Contact_Sent:
                    subwriter = new Contact_HtmlSubwriter(String.Empty, String.Empty, currentMode, hierarchyObject);
                    break;

                case Display_Mode_Enum.Simple_HTML_CMS:
                    subwriter = new Web_Content_HtmlSubwriter(hierarchyObject, currentMode, htmlSkin, htmlBasedContent, siteMap);
                    break;

                case Display_Mode_Enum.My_Sobek:
                    subwriter = new MySobek_HtmlSubwriter(results_statistics, paged_results, codeManager, itemList, hierarchyObject, htmlSkin, translator, currentMode, currentItem, aggregationAliases, webSkins, currentUser, ipRestrictionInfo, iconList, urlPortals, statsDateRange, thematicHeadings, Tracer);
                    break;

                case Display_Mode_Enum.Administrative:
                    subwriter = new Admin_HtmlSubwriter(results_statistics, paged_results, codeManager, itemList, hierarchyObject, htmlSkin, translator, currentMode, currentItem, aggregationAliases, webSkins, currentUser, ipRestrictionInfo, iconList, urlPortals, statsDateRange, thematicHeadings, Tracer);
                    break;

                case Display_Mode_Enum.Results:
                    subwriter = new Search_Results_HtmlSubwriter(results_statistics, paged_results, codeManager, translator, itemList, currentUser);
                    break;

                case Display_Mode_Enum.Public_Folder:
                    subwriter = new Public_Folder_HtmlSubwriter(results_statistics, paged_results, codeManager, translator, itemList, currentUser, publicFolder);
                    break;

                case Display_Mode_Enum.Search:
                case Display_Mode_Enum.Aggregation_Home:
                case Display_Mode_Enum.Aggregation_Browse_Info:
                case Display_Mode_Enum.Aggregation_Browse_By:
                case Display_Mode_Enum.Aggregation_Browse_Map:
                case Display_Mode_Enum.Aggregation_Private_Items:
                case Display_Mode_Enum.Aggregation_Item_Count:
                case Display_Mode_Enum.Aggregation_Usage_Statistics:
                case Display_Mode_Enum.Aggregation_Admin_View:
                    subwriter = new Aggregation_HtmlSubwriter(hierarchyObject, currentMode, htmlSkin, translator, thisBrowseObject, results_statistics, paged_results, codeManager, itemList, thematicHeadings, currentUser, htmlBasedContent, Tracer);
                    break;

                case Display_Mode_Enum.Item_Display:
                    if ((!currentMode.Invalid_Item) && ( currentItem != null ))
                    {
                        bool show_toc = false;
                        if (HttpContext.Current.Session["Show TOC"] != null)
                        {
                            Boolean.TryParse(HttpContext.Current.Session["Show TOC"].ToString(), out show_toc);
                        }

                        // Check that this item is not checked out by another user
                        bool itemCheckedOutByOtherUser = false;
                        if (currentItem.Behaviors.CheckOut_Required)
                        {
                            if ( !checkedItems.Check_Out(currentItem.Web.ItemID, HttpContext.Current.Request.UserHostAddress))
                            {
                                itemCheckedOutByOtherUser = true;
                            }
                        }

                        // Check to see if this is IP restricted
                        string restriction_message = String.Empty;
                        if (currentItem.Behaviors.IP_Restriction_Membership > 0)
                        {
                            if (HttpContext.Current != null)
                            {
                                int user_mask = (int) HttpContext.Current.Session["IP_Range_Membership"];
                                int comparison = currentItem.Behaviors.IP_Restriction_Membership & user_mask;
                                if (comparison == 0)
                                {
                                    int restriction = currentItem.Behaviors.IP_Restriction_Membership;
                                    int restriction_counter = 0;
                                    while (restriction%2 != 1)
                                    {
                                        restriction = restriction >> 1;
                                        restriction_counter++;
                                    }
                                    restriction_message = ipRestrictionInfo[restriction_counter].Item_Restricted_Statement;
                                }
                            }
                        }

                        // Create the item viewer writer
                        subwriter = new Item_HtmlSubwriter(currentItem, currentPage, currentUser, codeManager, translator, show_toc, (SobekCM_Library_Settings.JP2_Server.Length > 0), currentMode, hierarchyObject, restriction_message, itemsInTitle, Tracer); 
                        ((Item_HtmlSubwriter) subwriter).Item_Checked_Out_By_Other_User = itemCheckedOutByOtherUser;
                        break;
                    }
                    else
                    {
                        // Create the invalid item html subwrite and write the HTML
                        subwriter = new Error_HtmlSubwriter(true);
                    }
                    break;

            }


            if (subwriter != null)
            {
                subwriter.Mode = currentMode;
                subwriter.Skin = htmlSkin;
                subwriter.Hierarchy_Object = hierarchyObject;
            }
        }


        /// <summary> Returns a flag indicating if the current request requires the navigation form in the main ASPX
        /// application page, or whether all the html is served directly to the output stream, without the need of this form
        /// or any controls added to it </summary>
        /// <value> The return value of this varies according to the current request </value>
        public override bool Include_Navigation_Form
        {
            get
            {
                switch (currentMode.Mode)
                {
                    case Display_Mode_Enum.Item_Print:
                    case Display_Mode_Enum.Internal:
                    case Display_Mode_Enum.Statistics:
                    case Display_Mode_Enum.Preferences:
                    case Display_Mode_Enum.Search:
                    case Display_Mode_Enum.Contact_Sent:
                    case Display_Mode_Enum.Error:
                    case Display_Mode_Enum.Legacy_URL:
                        return false;

                    case Display_Mode_Enum.Simple_HTML_CMS:
                        return siteMap != null;

                    case Display_Mode_Enum.Aggregation_Home:
                        if ((currentMode.Aggregation.Length != 0) && (currentMode.Aggregation.ToUpper() != "ALL"))
                        {
                            return false;
                        }
                        return (currentMode.Home_Type == Home_Type_Enum.Tree_Collapsed) || (currentMode.Home_Type == Home_Type_Enum.Tree_Expanded);

                    default: 
                        return true;
                }
            }
        }


        /// <summary> Returns a flag indicating whether the additional table of contents place holder ( &quot;tocPlaceHolder&quot; ) in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden. </summary>
        /// <value> This property always returns TRUE for the Html_MainWriter </value>
        public override bool Include_TOC_Place_Holder
        {
            get
            {
                return true;
            }
        }

        /// <summary> Returns a flag indicating whether the additional place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden. </summary>
        /// <value> This property always returns TRUE for the Html_MainWriter </value>
        public override bool Include_Main_Place_Holder
        {
            get
            {
                return true;
            }
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.Library.Navigation.Writer_Type_Enum.HTML"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.HTML; } }

        /// <summary> Perform all the work of adding to the response stream back to the web user </summary>
        /// <param name="TOC_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="Main_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="myUfdcUploadPlaceHolder"> Place holder is used to add more complex server-side objects during execution </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Since this class writes all the output directly to the response stream, this method simply returns, without doing anything</remarks>
        public override void Add_Controls( PlaceHolder TOC_Place_Holder, PlaceHolder Main_Place_Holder, PlaceHolder myUfdcUploadPlaceHolder, Custom_Tracer Tracer)
        {
            // If execution should end, do it now
            if (currentMode.Request_Completed)
                return;

            Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Adding any necessary controls to the placeholders on the page");

            // Render HTML and add controls depending on the current mode
            switch (currentMode.Mode)
            {
                #region Start adding HTML and controls for SIMPLE WEB CONTENT TEXT mode

                case Display_Mode_Enum.Simple_HTML_CMS:
                    // Add any necessary controls
                    if (siteMap != null)
                    {
                        ((Web_Content_HtmlSubwriter)subwriter).Add_Controls(Main_Place_Holder, Tracer);
                    }

                    break;

                #endregion

                #region Start adding HTML and controls for MY SOBEK mode

                case Display_Mode_Enum.My_Sobek:

                    if ((subwriter != null) && (subwriter is MySobek_HtmlSubwriter))
                    {
                        MySobek_HtmlSubwriter mySobekWriter = (MySobek_HtmlSubwriter) subwriter;

                        // If the my sobek writer contains pop up forms, add the header here first
                        if (mySobekWriter.Contains_Popup_Forms)
                        {
                            StringBuilder header_builder = new StringBuilder();
                            StringWriter header_writer = new StringWriter(header_builder);
                            Display_Header(header_writer, Tracer);
                            LiteralControl header_literal = new LiteralControl(header_builder.ToString());
                            Main_Place_Holder.Controls.Add(header_literal);
                        }

                        // Add any necessary controls
                        mySobekWriter.Add_Controls(Main_Place_Holder, myUfdcUploadPlaceHolder, Tracer);

                        // Finally, add the footer
                        if (mySobekWriter.Contains_Popup_Forms)
                        {
                            StringBuilder footer_builder = new StringBuilder();
                            StringWriter footer_writer = new StringWriter(footer_builder);
                            Display_Footer(footer_writer, Tracer);
                            LiteralControl footer_literal = new LiteralControl(footer_builder.ToString());
                            Main_Place_Holder.Controls.Add(footer_literal);
                        }
                    }
                    break;

                #endregion

                #region Start adding HTML and controls for ADMINISTRATIVE mode

                case Display_Mode_Enum.Administrative:

                    if ((subwriter != null) && (subwriter is Admin_HtmlSubwriter))
                    {
                        // Build the my sobek subwriter
                        Admin_HtmlSubwriter adminWriter = (Admin_HtmlSubwriter) subwriter;

                        // If the my sobek writer contains pop up forms, add the header here first
                        if (adminWriter.Contains_Popup_Forms)
                        {
                            StringBuilder header_builder = new StringBuilder();
                            StringWriter header_writer = new StringWriter(header_builder);
                            Display_Header(header_writer, Tracer);
                            LiteralControl header_literal = new LiteralControl(header_builder.ToString());
                            Main_Place_Holder.Controls.Add(header_literal);
                        }

                        // Add any necessary controls
                        adminWriter.Add_Controls(Main_Place_Holder, myUfdcUploadPlaceHolder, Tracer);

                        // Finally, add the footer
                        if (adminWriter.Contains_Popup_Forms)
                        {
                            StringBuilder footer_builder = new StringBuilder();
                            StringWriter footer_writer = new StringWriter(footer_builder);
                            Display_Footer(footer_writer, Tracer);
                            LiteralControl footer_literal = new LiteralControl(footer_builder.ToString());
                            Main_Place_Holder.Controls.Add(footer_literal);
                        }
                    }

                    break;

                #endregion

                #region Start adding HTML and add controls for RESULTS mode

                case Display_Mode_Enum.Results:
                    if ((subwriter != null) && (subwriter is Search_Results_HtmlSubwriter))
                    {
                        // Make sure the corresponding 'search' is the latest
                        currentMode.Mode = Display_Mode_Enum.Search;
                        HttpContext.Current.Session["LastSearch"] = currentMode.Redirect_URL();
                        currentMode.Mode = Display_Mode_Enum.Results;

                        // Add the controls 
                        ((Search_Results_HtmlSubwriter)subwriter).Add_Controls(Main_Place_Holder, Tracer, null);
                    }

                    break;

                #endregion

                #region Add HTML and controls for PUBLIC FOLDER mode

                case Display_Mode_Enum.Public_Folder:
                    if ((subwriter != null) && (subwriter is Public_Folder_HtmlSubwriter))
                    {
                        // Also try to add any controls
                        ((Public_Folder_HtmlSubwriter) subwriter).Add_Controls(Main_Place_Holder, Tracer, null);
                    }
                    break;

                #endregion

                #region Add HTML and controls for COLLECTION VIEWS

                case Display_Mode_Enum.Search:
                case Display_Mode_Enum.Aggregation_Home:
                case Display_Mode_Enum.Aggregation_Browse_Info:
                case Display_Mode_Enum.Aggregation_Browse_By:
                case Display_Mode_Enum.Aggregation_Browse_Map:
                case Display_Mode_Enum.Aggregation_Private_Items:
                case Display_Mode_Enum.Aggregation_Item_Count:
                case Display_Mode_Enum.Aggregation_Usage_Statistics:
                case Display_Mode_Enum.Aggregation_Admin_View:
                    if ((subwriter != null) && (subwriter is Aggregation_HtmlSubwriter))
                    {
                        // Also try to add any controls
                        ((Aggregation_HtmlSubwriter) subwriter).Add_Controls(Main_Place_Holder, Tracer);
                    }
                    break;

                #endregion

                #region Start adding HTML and add controls for ITEM DISPLAY mode

                case Display_Mode_Enum.Item_Display:
                    if ((subwriter != null) && (subwriter is Item_HtmlSubwriter))
                    {
                        Item_HtmlSubwriter itemWriter = (Item_HtmlSubwriter) subwriter;

                        // Add the TOC section
                        Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Allowing item viewer to add table of contents to <i>tocPlaceHolder</i>");
                        itemWriter.Add_Standard_TOC(TOC_Place_Holder, Tracer);

                        // Add the main viewer section
                        itemWriter.Add_Main_Viewer_Section(Main_Place_Holder, Tracer);
                    }
                    break;

                #endregion

                default:
                    Tracer.Add_Trace("Html_MainWriter.Add_Html_And_Controls", "No controls or html added to page");
                    break;
            }
        }

        
        /// <summary> Gets the title to use for this web page, based on the current request mode </summary>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Title to use in the HTML result document </returns>
        public string Get_Page_Title(Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Get_Page_Title", "Getting page title");

            string thisTitle = null;
            if (subwriter != null)
                thisTitle = subwriter.WebPage_Title;
            if ( String.IsNullOrEmpty(thisTitle))
                thisTitle = "{0}";

            return String.Format(thisTitle, currentMode.SobekCM_Instance_Abbreviation);
        }

        /// <summary> Writes the style references and other data to the HEAD portion of the web page </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Add_Style_References", "Adding style references and apple touch icon to HTML");

            // A couple extraordinary cases
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Reset:
                case Display_Mode_Enum.Item_Cache_Reload:
                case Display_Mode_Enum.None:
                    Output.WriteLine("  <meta name=\"robots\" content=\"noindex, nofollow\" />");
                    break;
            }

            // Write the style sheet to use 
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");

            // Always add jQuery library (changed as of 7/8/2013)
            if ((currentMode.Mode != Display_Mode_Enum.Item_Display) || (currentMode.ViewerCode != "pageturner"))
            {
#if DEBUG
                Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.10.2.js\"></script>");
#else
                Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.10.2.min.js\"></script>");
#endif
            }


            // Add the special code for the html subwriter
            if (subwriter != null)
                subwriter.Write_Within_HTML_Head(Output, Tracer);

            // Special CSS when printing an item from the selection menu
            if (currentMode.Mode == Display_Mode_Enum.Item_Print)
            {
                return;
            }

            // Include the interface's style sheet if it has one
            if ((htmlSkin != null) && (htmlSkin.CSS_Style.Length > 0))
            {
                Output.WriteLine("  <style type=\"text/css\" media=\"screen\">");
                Output.WriteLine("    @import url( " + currentMode.Base_URL + htmlSkin.CSS_Style + " );");
                Output.WriteLine("  </style>");
            }

            // Add a printer friendly CSS
            Output.WriteLine("  <link rel=\"stylesheet\" href=\"" + currentMode.Base_URL + "default/print.css\" type=\"text/css\" media=\"print\" /> ");

            // Add the apple touch icon
            Output.WriteLine("  <link rel=\"apple-touch-icon\" href=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Skin + "/iphone-icon.png\" />");
        }


        /// <summary> Gets the body attributes to include within the BODY tag of the main HTML response document </summary>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Body attributes to include in the BODY tag </returns>
        public string Get_Body_Attributes(Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Get_Body_Attributes", "Adding body attributes to HTML");

            // Get the attributes which should be included by the html sub writer
            List<Tuple<string, string>> bodyAttributes = subwriter.Body_Attributes;

            // Handles special case where a message should be displayed to the user
            // from a previous action
            if (!currentMode.isPostBack)
            {
                if ((HttpContext.Current.Session["ON_LOAD_MESSAGE"] != null) || (HttpContext.Current.Session["ON_LOAD_WINDOW"] != null))
                {
                    // ENsure the body attributes list is not null
                    if (bodyAttributes == null)
                        bodyAttributes = new List<Tuple<string, string>>();

                    // Handle the previously saved actions
                    if (HttpContext.Current.Session["ON_LOAD_MESSAGE"] != null)
                    {
                        string on_load_message = HttpContext.Current.Session["ON_LOAD_MESSAGE"].ToString();
                        if (on_load_message.Length > 0)
                            bodyAttributes.Add(new Tuple<string, string>("onload", "alert('" + on_load_message + "');"));
                        HttpContext.Current.Session.Remove("ON_LOAD_MESSAGE");
                    }
                    if (HttpContext.Current.Session["ON_LOAD_WINDOW"] != null)
                    {
                        string on_load_window = HttpContext.Current.Session["ON_LOAD_WINDOW"].ToString();
                        if (on_load_window.Length > 0)
                            bodyAttributes.Add(new Tuple<string, string>("onload", "window.open('" + on_load_window + "', 'new_" + DateTime.Now.Millisecond + "');"));
                        HttpContext.Current.Session.Remove("ON_LOAD_WINDOW");
                    }
                }
            }

            // If there is nothing to add, return now
            if ((bodyAttributes == null) || (bodyAttributes.Count == 0))
                return String.Empty;

            // Create the string for the body attributes
            Dictionary<string, string> collapsedAttributes = new Dictionary<string, string>();
            foreach (Tuple<string, string> thisAttr in bodyAttributes)
            {
                if (collapsedAttributes.ContainsKey(thisAttr.Item1))
                    collapsedAttributes[thisAttr.Item1] = collapsedAttributes[thisAttr.Item1] + thisAttr.Item2;
                else
                    collapsedAttributes.Add(thisAttr.Item1, thisAttr.Item2);
            }

            // Now, build and return the string
            StringBuilder builder = new StringBuilder(" ");
            foreach (string thisKey in collapsedAttributes.Keys)
            {
                builder.Append(thisKey + "=\"" + collapsedAttributes[thisKey] + "\" ");
            }

            return builder.ToString();
        }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Write_Html", String.Empty);

            // If the subwriter is null, this is an ERROR, but do nothing for now
            if (subwriter == null) return;

            // Always add the link to the main, small SobekCM.js
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm.js\" type=\"text/javascript\"></script>");

            // Start with the basic html at the beginning of the page
            if (!subwriter.Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Header))
            {
                Display_Header(Output, Tracer);
            }

            try
            {
                subwriter.Write_HTML(Output, Tracer);
            }
            catch (Exception ee)
            {
                Email_Information("Error caught in Html_MainWriter", ee, Tracer, true);
                throw new SobekCM_Traced_Exception("Error caught in Html_MainWriter.Write_Html", ee, Tracer);
            }
        }

        /// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            if (subwriter == null) return;
            Tracer.Add_Trace("Html_MainWriter.Write_Additional_HTML", "Allowing html subwriter to write to the page");

            subwriter.Write_ItemNavForm_Opening(Output, Tracer);
        }

        /// <summary> Writes additional HTML to the output stream just before the main place holder but after the TocPlaceHolder in the itemNavForm form.  </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (subwriter == null) return;
            Tracer.Add_Trace("Html_MainWriter.Write_Additional_HTML", "Allowing html subwriter to write to the page");
 
            subwriter.Write_Additional_HTML(Output, Tracer);
        }

        /// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            if (subwriter == null) return;
            Tracer.Add_Trace("Html_MainWriter.Write_Additional_HTML", "Allowing html subwriter to write to the page");

            subwriter.Write_ItemNavForm_Closing(Output, Tracer);
        }

        /// <summary> Writes any final HTML needed after the main place holder directly to the output stream</summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (( currentMode.isPostBack) && ( currentMode.Mode != Display_Mode_Enum.My_Sobek )) return;
            if (subwriter == null) return;

            Tracer.Add_Trace("Html_MainWriter.Write_Final_HTML", String.Empty);

            // Allow the html subwriter to write some final HTML
            subwriter.Write_Final_HTML(Output, Tracer);

            // Add the footer if necessary
            if (!subwriter.Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Footer))
            {
                Display_Footer(Output, Tracer);
            }
        }

        #region Protected internal methods to write the header and footer to the stream

        /// <summary> Writes the header directly to the output stream writer </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        protected internal void Display_Header(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Display_Header", "Adding header to HTML");

            // In GnuBooks page turner, this is a full-screen view.. so no footer
            if ((currentMode.Mode == Display_Mode_Enum.Item_Display) && (currentMode.ViewerCode == "pageturner"))
            {
                return;
            }

            // Should the internal header be added?
            if ((subwriter != null) && (currentMode.Mode != Display_Mode_Enum.My_Sobek) && (currentMode.Mode != Display_Mode_Enum.Administrative) && (currentUser != null))
            {

                bool displayHeader = false;
                if (( currentUser.Is_Internal_User ) || ( currentUser.Is_System_Admin ))
                {
                    displayHeader = true;
                }
                else
                {
                    switch ( currentMode.Mode )
                    {
                        case Display_Mode_Enum.Item_Display:
                            if ( currentUser.Can_Edit_This_Item( currentItem ))
                                displayHeader = true;
                            break;

                        case Display_Mode_Enum.Aggregation_Admin_View:
                        case Display_Mode_Enum.Aggregation_Browse_By:
                        case Display_Mode_Enum.Aggregation_Browse_Info:
                        case Display_Mode_Enum.Aggregation_Browse_Map:
                        case Display_Mode_Enum.Aggregation_Home:
                        case Display_Mode_Enum.Aggregation_Item_Count:
                        case Display_Mode_Enum.Aggregation_Private_Items:
                        case Display_Mode_Enum.Aggregation_Usage_Statistics:
                        case Display_Mode_Enum.Results:
                        case Display_Mode_Enum.Search:
                            if (( currentUser.Is_Aggregation_Curator( currentMode.Aggregation )) || ( currentUser.Can_Edit_All_Items( currentMode.Aggregation )))
                            {
                                displayHeader = true;
                            }
                            break;
                    }
                }
                
                if (( displayHeader ) && ( !subwriter.Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header)))
                {
                    string return_url = currentMode.Redirect_URL();
                    if ((HttpContext.Current != null) && (HttpContext.Current.Session["Original_URL"] != null))
                        return_url = HttpContext.Current.Session["Original_URL"].ToString();

                    Output.WriteLine("<!-- Start the internal header -->");
                    Output.WriteLine("<form name=\"internalHeaderForm\" method=\"post\" action=\"" + return_url + "\" id=\"internalHeaderForm\"> ");
                    Output.WriteLine();
                    Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
                    Output.WriteLine("<input type=\"hidden\" id=\"internal_header_action\" name=\"internal_header_action\" value=\"\" />");
                    Output.WriteLine();

                    // Is the header currently hidden?
                    if (HttpContext.Current != null && ((HttpContext.Current.Session["internal_header"] != null) && (HttpContext.Current.Session["internal_header"].ToString() == "hidden")))
                    {
                        Output.WriteLine("  <table cellspacing=\"0\" id=\"internalheader\">");
                        Output.WriteLine("    <tr>");
                        Output.WriteLine("      <td align=\"left\">");
                        Output.WriteLine("        <button title=\"Show Internal Header\" class=\"intheader_button_aggr show_intheader_button_aggr\" onclick=\"return show_internal_header();\"></button>");
                        Output.WriteLine("      </td>");
                        Output.WriteLine("    </tr>");
                        Output.WriteLine("  </table>"); 
                    }
                    else
                    {
                        subwriter.Write_Internal_Header_HTML(Output, currentUser);
                    }

                    Output.WriteLine("</form>");
                    Output.WriteLine("<!-- End the internal header -->");
                    Output.WriteLine();
                }
            }

            // Get the url options
            string url_options = currentMode.URL_Options();
            string modified_url_options = String.Empty;
            if (url_options.Length > 0)
                modified_url_options = "?" + url_options;

            // Create the breadcrumbs text
            string breadcrumbs = "&nbsp; &nbsp; ";
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Error:
                    breadcrumbs = "<a href=\"" + currentMode.Base_URL + modified_url_options + "\">" + currentMode.SobekCM_Instance_Abbreviation + " Home</a>";
                    break;

                case Display_Mode_Enum.Item_Display:
                    StringBuilder breadcrumb_builder = new StringBuilder("<a href=\"" + currentMode.Base_URL + modified_url_options + "\">" + currentMode.SobekCM_Instance_Abbreviation + " Home</a>");

                    int codes_added = 0;
                    if ((currentMode.Aggregation.Length > 0) && (currentMode.Aggregation != "all"))
                    {
                        breadcrumb_builder.Append(" &nbsp;|&nbsp; <a href=\"" + currentMode.Base_URL + currentMode.Aggregation + modified_url_options + "\">" + codeManager.Get_Collection_Short_Name(currentMode.Aggregation) + "</a>");
                        codes_added++;
                    }

                    if (currentItem != null)
                    {
                        if (currentItem.Behaviors.Aggregation_Count > 0)
                        {
                            foreach (Aggregation_Info aggregation in currentItem.Behaviors.Aggregations)
                            {
                                string aggrCode = aggregation.Code;
                                if (aggrCode.ToLower() != currentMode.Aggregation)
                                {
                                    if ((aggrCode.ToUpper() != "I" + currentItem.Bib_Info.Source.Code.ToUpper()) &&
                                        (aggrCode.ToUpper() != "I" + currentItem.Bib_Info.Location.Holding_Code.ToUpper()))
                                    {
	                                    Item_Aggregation_Related_Aggregations thisAggr = codeManager[aggrCode];
	                                    if ((thisAggr != null) && (!thisAggr.Hidden) && (thisAggr.Active))
	                                    {
		                                    breadcrumb_builder.Append(" &nbsp;|&nbsp; <a href=\"" + currentMode.Base_URL +
		                                                              aggrCode.ToLower() + modified_url_options + "\">" +
		                                                              thisAggr.ShortName +
		                                                              "</a>");
		                                    codes_added++;
	                                    }
                                    }
                                }
                                if (codes_added == 5)
                                    break;
                            }
                        }

                        if (codes_added < 5)
                        {
                            if ((currentItem.Bib_Info.Source.Code.Length > 0) &&
                                (currentItem.Bib_Info.Source.Code != "UF") &&
                                (currentItem.Bib_Info.Source.Code.ToUpper() != "IUF"))
                            {
                                // Add source code
                                string source_code = currentItem.Bib_Info.Source.Code;
                                if ((source_code[0] != 'i') && (source_code[0] != 'I'))
                                    source_code = "I" + source_code;
								Item_Aggregation_Related_Aggregations thisSourceAggr = codeManager[source_code];
								if ((thisSourceAggr != null) && (!thisSourceAggr.Hidden) && (thisSourceAggr.Active))
	                            {
									string source_name = thisSourceAggr.ShortName;
		                            if (source_name.ToUpper() != "ADDED AUTOMATICALLY")
		                            {
			                            breadcrumb_builder.Append(" &nbsp;|&nbsp; <a href=\"" + currentMode.Base_URL +
			                                                      source_code.ToLower() + modified_url_options + "\">" +
			                                                      source_name + "</a>");
		                            }
	                            }

								// Add the holding code
	                            if ((currentItem.Bib_Info.Location.Holding_Code.Length > 0) &&
                                    (currentItem.Bib_Info.Location.Holding_Code != currentItem.Bib_Info.Source.Code) &&
                                    (currentItem.Bib_Info.Location.Holding_Code != "UF") &&
                                    (currentItem.Bib_Info.Location.Holding_Code.ToUpper() != "IUF"))
                                {
                                    // Add holding code
                                    string holding_code = currentItem.Bib_Info.Location.Holding_Code;
                                    if ((holding_code[0] != 'i') && (holding_code[0] != 'I'))
                                        holding_code = "I" + holding_code;

									Item_Aggregation_Related_Aggregations thisAggr = codeManager[holding_code];
	                                if ((thisAggr != null) && (!thisAggr.Hidden) && (thisAggr.Active))
	                                {
		                                string holding_name = thisAggr.ShortName;

		                                if (holding_name.ToUpper() != "ADDED AUTOMATICALLY")
		                                {
			                                breadcrumb_builder.Append(" &nbsp;|&nbsp; <a href=\"" + currentMode.Base_URL +
			                                                          holding_code.ToLower() + modified_url_options + "\">" +
			                                                          holding_name + "</a>");
		                                }
	                                }
                                }
                            }
                            else
                            {
                                if ((currentItem.Bib_Info.Location.Holding_Code.Length > 0) &&
                                    (currentItem.Bib_Info.Location.Holding_Code != "UF") &&
                                    (currentItem.Bib_Info.Location.Holding_Code.ToUpper() != "IUF"))
                                {
                                    // Add holding code
                                    string holding_code = currentItem.Bib_Info.Location.Holding_Code;
                                    if ((holding_code[0] != 'i') && (holding_code[0] != 'I'))
                                        holding_code = "I" + holding_code;
                                    string holding_name = codeManager.Get_Collection_Short_Name(holding_code);
                                    if (holding_name.ToUpper() != "ADDED AUTOMATICALLY")
                                    {
                                        breadcrumb_builder.Append(" &nbsp;|&nbsp; <a href=\"" + currentMode.Base_URL +
                                                                  holding_code.ToLower() + modified_url_options + "\">" +
                                                                  holding_name + "</a>");
                                    }
                                }
                            }
                        }
                    }
                    breadcrumbs = breadcrumb_builder.ToString();
                    break;

                case Display_Mode_Enum.Aggregation_Home:
                    if ((currentMode.Aggregation.Length > 0) && (currentMode.Aggregation != "all"))
                    {
                        breadcrumbs = "<a href=\"" + currentMode.Base_URL + modified_url_options + "\">" + currentMode.SobekCM_Instance_Abbreviation + " Home</a>";
                    }
                    break;

                default:
                    breadcrumbs = "<a href=\"" + currentMode.Base_URL + modified_url_options + "\">" + currentMode.SobekCM_Instance_Abbreviation + " Home</a>";
                    if ((currentMode.Aggregation.Length > 0) && (currentMode.Aggregation != "all"))
                    {
                        breadcrumbs = breadcrumbs + " &nbsp;|&nbsp; <a href=\"" + currentMode.Base_URL + currentMode.Aggregation + modified_url_options + "\">" + codeManager.Get_Collection_Short_Name(currentMode.Aggregation) + "</a>";
                    }
                    break;

            }
            
            // Create they myUFDC text
            string mySobekLinks = String.Empty;
            if (!currentMode.Is_Robot)
            {
                string mySobekText = "my" + currentMode.SobekCM_Instance_Abbreviation;
                string mySobekOptions = url_options;
                string mySobekLogoutOptions = url_options;
                string return_url = String.Empty;
                if (( HttpContext.Current != null ) && ( HttpContext.Current.Items["Original_URL"] != null ))
                    return_url = HttpContext.Current.Items["Original_URL"].ToString().ToLower().Replace(currentMode.Base_URL.ToLower(), "");
                if (return_url.IndexOf("?") > 0)
                    return_url = return_url.Substring(0, return_url.IndexOf("?"));
                if (return_url.IndexOf("my/") == 0)
                    return_url = String.Empty;
                string logout_return_url = return_url;
                if (logout_return_url.IndexOf("l/") == 0)
                    logout_return_url = logout_return_url.Substring(2);
                
                return_url = HttpUtility.UrlEncode(return_url);
                logout_return_url = HttpUtility.UrlEncode(logout_return_url);

                if ((url_options.Length > 0) || ( return_url.Length > 0 ))
                {
                    if ((url_options.Length > 0) && (return_url.Length > 0))
                    {
                        mySobekOptions = "?" + mySobekOptions + "&return=" + return_url;
                    }
                    else
                    {
                        if (url_options.Length > 0)
                            mySobekOptions = "?" + mySobekOptions;
                        else
                            mySobekOptions = "?return=" + return_url;
                    }
                }

                if ((url_options.Length > 0) || (logout_return_url.Length > 0))
                {
                    if ((url_options.Length > 0) && (logout_return_url.Length > 0))
                    {
                        mySobekLogoutOptions = "?" + mySobekOptions + "&return=" + logout_return_url;
                    }
                    else
                    {
                        if (url_options.Length > 0)
                            mySobekLogoutOptions = "?" + mySobekOptions;
                        else
                            mySobekLogoutOptions = "?return=" + logout_return_url;
                    }
                }

                if (( HttpContext.Current != null ) && (HttpContext.Current.Session["user"] == null))
                {
                    mySobekLinks = "<a href=\"" + currentMode.Base_URL + "my/logon" + mySobekOptions + "\">" + mySobekText + " Home</a>";
                }
                else
                {
                    User_Object tempObject = ((User_Object)HttpContext.Current.Session["user"]);
                    if (tempObject.Nickname.Length > 0)
                    {
                        mySobekLinks = "<a href=\"" + currentMode.Base_URL + "my" + mySobekOptions + "\">" + tempObject.Nickname + "'s " + mySobekText + "</a>&nbsp; | &nbsp; <a href=\"" + currentMode.Base_URL + "my/logout" + mySobekLogoutOptions + "\">Log Out</a>";
                    }
                    else
                    {
                        mySobekLinks = "<a href=\"" + currentMode.Base_URL + "my" + mySobekOptions + "\">" + tempObject.Given_Name + "'s " + mySobekText + "</a>&nbsp; | &nbsp; <a href=\"" + currentMode.Base_URL + "my/logout" + mySobekLogoutOptions + "\">Log Out</a>";
                    }
                }
            }


            // Get the language selections
            Web_Language_Enum language = currentMode.Language;
            currentMode.Language = Web_Language_Enum.TEMPLATE;
            string template_language = currentMode.Redirect_URL();
            string english = template_language.Replace("l=XXXXX", "l=en");
            string french = template_language.Replace("l=XXXXX", "l=fr");
            string spanish = template_language.Replace("l=XXXXX", "l=es");
            currentMode.Language = language;

            if (currentMode.Is_Robot)
            {
                english = String.Empty;
                french = String.Empty;
                spanish = String.Empty;
            }

            // Determine which container to use, depending on the current mode
            string container_inner = "container-inner";
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.My_Sobek :
                    if ((currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Metadata) && ( currentMode.My_Sobek_SubMode.IndexOf("0.2") == 0 ))
                    {
                        container_inner = "container-inner1000";
                    }
                    break;

                case Display_Mode_Enum.Statistics:
                    switch (currentMode.Statistics_Type)
                    {
                        case Statistics_Type_Enum.Item_Count_Growth_View:
                        case Statistics_Type_Enum.Item_Count_Arbitrary_View:
                            container_inner = "container-inner1000";
                            break;

                        case Statistics_Type_Enum.Usage_Collections_By_Date:
                            container_inner = "container-inner1040";
                            break;

                        case Statistics_Type_Enum.Usage_Item_Views_By_Date:
                            container_inner = "container-inner1215";
                            break;

                    }
                    break;

                case Display_Mode_Enum.Internal:
                    if (currentMode.Internal_Type == Internal_Type_Enum.Wordmarks)
                    {
                        container_inner = "container-inner1000";
                    }
                    if ( currentMode.Internal_Type == Internal_Type_Enum.Aggregations )
                        container_inner = "container-inner1215";
                    break;

				case Display_Mode_Enum.Aggregation_Browse_By:
					container_inner = "container-facets";
		            break;

                case Display_Mode_Enum.Aggregation_Browse_Map:
                    container_inner = "container-inner1000";
                    break;

                case Display_Mode_Enum.Aggregation_Private_Items:
                    container_inner = "container-inner1215";
                    break;

				case Display_Mode_Enum.Results:
				case Display_Mode_Enum.Aggregation_Browse_Info:
		            if (paged_results != null)
		            {
			            container_inner = "container-facets";
		            }
		            break;

				case Display_Mode_Enum.Item_Display:
				case Display_Mode_Enum.Item_Print:
		            container_inner = String.Empty;
		            break;

				case Display_Mode_Enum.Simple_HTML_CMS:
					if ( siteMap != null )
						container_inner = String.Empty;
					break;


            }

            // Get the skin url
            string skin_url = currentMode.Base_Design_URL + "skins/" + currentMode.Skin + "/";

            // Determine the URL options for replacement
            string urlOptions1 = String.Empty;
            string urlOptions2 = String.Empty;
            if (url_options.Length > 0)
            {
                urlOptions1 = "?" + url_options;
                urlOptions2 = "&" + url_options;
            }

            // Determine the possible banner to display
            string banner = String.Empty;
            if ((subwriter != null) && ( !subwriter.Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Banner)))
            {
                if ((htmlSkin != null) && (htmlSkin.Override_Banner))
                {
                    banner = htmlSkin.Banner_HTML;
                }
                else
                {
                    if (hierarchyObject != null)
                    {
                        string banner_image = hierarchyObject.Banner_Image(currentMode.Language, htmlSkin);
                        if  (hierarchyObject.Code != "all")
                        {                            
                            if (banner_image.Length > 0)
                                banner = "<div id=\"sbkHmw_BannerDiv\"><a alt=\"" + hierarchyObject.ShortName + "\" href=\"" + currentMode.Base_URL + hierarchyObject.Code + urlOptions1 + "\"><img id=\"mainBanner\" src=\"" + currentMode.Base_URL + banner_image + "\" alt=\"\" /></a></div>";
                        }
                        else
                        {
                            if (banner_image.Length > 0)
                            {
                                banner = "<div id=\"sbkHmw_BannerDiv\"><a href=\"" + currentMode.Base_URL + urlOptions1 + "\"><img id=\"mainBanner\" src=\"" + currentMode.Base_URL + banner_image + "\" alt=\"\" /></a></div>";
                            }
                            else
                            {
                                banner = "<div id=\"sbkHmw_BannerDiv\"><a href=\"" + currentMode.Base_URL + urlOptions1 + "\"><img id=\"mainBanner\" src=\"" + currentMode.Base_URL + "default/images/sobek.jpg\" alt=\"\" /></a></div>";
                            }
                        }
                    }
                }
            }

            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Item_Display:
				case Display_Mode_Enum.Item_Print:
                    Output.WriteLine(htmlSkin.Header_Item_HTML.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%BREADCRUMBS%>", breadcrumbs).Replace("<%MYSOBEK%>", mySobekLinks).Replace("<%ENGLISH%>", english).Replace("<%FRENCH%>", french).Replace("<%SPANISH%>", spanish).Replace("<%BASEURL%>", currentMode.Base_URL).Replace("\"container-inner\"", "\"" + container_inner + "\"").Replace("<%BANNER%>", banner).Replace("<%SKINURL%>", skin_url));
                    break;

                default:
					if ( container_inner.Length == 0 )
						Output.WriteLine(htmlSkin.Header_HTML.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%BREADCRUMBS%>", breadcrumbs).Replace("<%MYSOBEK%>", mySobekLinks).Replace("<%ENGLISH%>", english).Replace("<%FRENCH%>", french).Replace("<%SPANISH%>", spanish).Replace("<%BASEURL%>", currentMode.Base_URL).Replace("\"container-inner\"", "\"" + container_inner + "\"").Replace("<%BANNER%>", banner).Replace("<%SKINURL%>", skin_url));
					else
						Output.WriteLine("<div id=\"" + container_inner + "\">" + Environment.NewLine + htmlSkin.Header_HTML.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%BREADCRUMBS%>", breadcrumbs).Replace("<%MYSOBEK%>", mySobekLinks).Replace("<%ENGLISH%>", english).Replace("<%FRENCH%>", french).Replace("<%SPANISH%>", spanish).Replace("<%BASEURL%>", currentMode.Base_URL).Replace("\"container-inner\"", "\"" + container_inner + "\"").Replace("<%BANNER%>", banner).Replace("<%SKINURL%>", skin_url));
                    break;
            }

            Output.WriteLine(String.Empty);
        }

        /// <summary> Writes the footer directly to the output stream writer provided </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        protected internal void Display_Footer(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Display_Footer", "Adding footer to HTML");

            // Some subwriters (for example item viewer with Gnu page turning view) suppress the footer
            if (subwriter.Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Footer))
            {
                return;
            }

            // Get the current contact URL
            Display_Mode_Enum thisMode = currentMode.Mode;
            currentMode.Mode = Display_Mode_Enum.Contact;
            string contact = currentMode.Redirect_URL();

            // Restore the old mode
            currentMode.Mode = thisMode;

            // Get the URL options
            string url_options = currentMode.URL_Options();
            string urlOptions1 = String.Empty;
            string urlOptions2 = String.Empty;
            if (url_options.Length > 0)
            {
                urlOptions1 = "?" + url_options;
                urlOptions2 = "&" + url_options;
            }

            // Get the base url
            string base_url = currentMode.Base_URL;
            if (currentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = base_url + "l/";

            // Get the skin url
            string skin_url = currentMode.Base_Design_URL + "skins/" + htmlSkin.Skin_Code + "/";

	        bool end_div = !(( currentMode.Mode == Display_Mode_Enum.Simple_HTML_CMS ) && ( siteMap != null ));

	        const string version = SobekCM_Library_Settings.CURRENT_WEB_VERSION;
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Item_Display:
                    Output.WriteLine(htmlSkin.Footer_Item_HTML.Replace("<%CONTACT%>", contact).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%VERSION%>", version).Replace("<%BASEURL%>", base_url).Replace("<%SKINURL%>", skin_url ) .Trim());
                    break;

                default:
					if ( !end_div )
	                    Output.WriteLine(htmlSkin.Footer_HTML.Replace("<%CONTACT%>", contact).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%VERSION%>", version).Replace("<%BASEURL%>", base_url).Replace("<%SKINURL%>", skin_url).Trim());
					else
						Output.WriteLine(htmlSkin.Footer_HTML.Replace("<%CONTACT%>", contact).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%VERSION%>", version).Replace("<%BASEURL%>", base_url).Replace("<%SKINURL%>", skin_url).Trim() + Environment.NewLine + "</div>");
                    break;
            }


            // Add the time and trace at the end
            if (( currentMode.Trace_Flag_Simple ) || (( currentUser != null ) && ( currentUser.Is_System_Admin )))
            {
                Output.WriteLine("<style type=\"text/css\">");
                Output.WriteLine("table.Traceroute { border-width: 2px; border-style: solid; border-color: gray; border-collapse: collapse; background-color: white; font-size: small; }");
                Output.WriteLine("table.Traceroute th { border-width: 2px; padding: 3px; border-style: solid; border-color: gray; background-color: gray; color: white; }");
                Output.WriteLine("table.Traceroute td { border-width: 2px; padding: 3px; border-style: solid; border-color: gray;	background-color: white; }");
                Output.WriteLine("</style>");
				Output.WriteLine("<a href=\"\" onclick=\"return show_trace_route()\" id=\"sbkHmw_TraceRouterShowLink\">show trace route (sys admin)</a>");
				Output.WriteLine("<div id=\"sbkHmw_TraceRouter\" style=\"display:none;\">");

                Output.WriteLine("<br /><br /><b>URL REWRITE</b>");
                if (HttpContext.Current.Items["Original_URL"] == null)
                    Output.WriteLine("<br /><br />Original URL: <i>None found</i><br />");
                else
                    Output.WriteLine("<br /><br />Original URL: " + HttpContext.Current.Items["Original_URL"] + "<br />");

                Output.WriteLine("Current URL: " + HttpContext.Current.Request.Url + "<br />");


                Output.WriteLine("<br /><br /><b>TRACE ROUTE</b>");
                Output.WriteLine("<br /><br />Total Execution Time: " + Tracer.Milliseconds + " Milliseconds<br /><br />");
                Output.WriteLine(Tracer.Complete_Trace + "<br />");
				Output.WriteLine("</div>");
            }
        }

        #endregion

        #region Method to email information during an error

        private static void Email_Information(string email_title, Exception objErr, Custom_Tracer Tracer, bool Redirect )
        {
            // Is ther an error email address in the configuration?
            if (SobekCM_Library_Settings.System_Error_Email.Length > 0)
            {
                try
                {
                    // Build the error message
                    string err;
                    if (objErr != null)
                    {
                        if (objErr.InnerException != null)
                        {
                            err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />" +
                                  "Error in!!: " + HttpContext.Current.Items["Original_URL"] + "<br /><br />" +
                                  "Error Message: " + objErr.Message + "<br /><br />" +
                                  "Inner Exception: " + objErr.InnerException.Message + "<br /><br />" +
                                  "Stack Trace: " + objErr.InnerException.StackTrace + "<br /><br />";
                        }
                        else
                        {
                            err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />" +
                                  "Error in!!: " + HttpContext.Current.Items["Original_URL"] + "<br /><br />" +
                                  "Error Message: " + objErr.Message + "<br /><br />" +
                                  "Stack Trace: " + objErr.StackTrace + "<br /><br />";

                        }

                        if (objErr.Message.IndexOf("Timeout expired") >= 0)
                            email_title = "Database Timeout Expired";
                    }
                    else
                    {
                        err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />";
                    }

                    // Email the error message
                    if (Tracer != null)
                    {
                        SobekCM_Database.Send_Database_Email(SobekCM_Library_Settings.System_Error_Email, email_title, err + "<br /><br />" + Tracer.Text_Trace, true, false, -1, -1);
                    }
                    else
                    {
                        SobekCM_Database.Send_Database_Email(SobekCM_Library_Settings.System_Error_Email, email_title, err, true, false, -1, -1);
                    }
                }
                catch (Exception)
                {
                    // Failed to send the email.. but not much else to do here really
                }
            }

            // Forward to our error message
            if (Redirect)
            {
                HttpContext.Current.Response.Redirect(SobekCM_Library_Settings.System_Error_URL, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        #endregion
    }
}
 