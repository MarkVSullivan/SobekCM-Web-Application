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
        private readonly IP_Restriction_Ranges ipRestrictions;
        private readonly Item_Lookup_Object itemList;
        private readonly List<iSearch_Title_Result> pagedResults;
        private readonly Search_Results_Statistics resultsStatistics;
        private List<Thematic_Heading> thematicHeadings;
        private readonly Item_Aggregation_Browse_Info thisBrowseObject;
        private readonly HTML_Based_Content thisStaticBrowseObject;
        private readonly Language_Support_Info translator;

        /// <summary> Constructor creates a new instance of the Aggregation_HtmlSubwriter class </summary>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
        /// <param name="Paged_Results"> Paged results to display within a browse or search result </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Thematic_Headings"> Headings under which all the highlighted collections on the home page are organized </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="IP_Restrictions"> IP restrictions, used to determine if a user has access to a particular item </param>
        /// <param name="Static_Web_Content"> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Aggregation_HtmlSubwriter(Item_Aggregation Hierarchy_Object, 
            SobekCM_Navigation_Object Current_Mode, SobekCM_Skin_Object HTML_Skin, 
            Language_Support_Info Translator, 
            Item_Aggregation_Browse_Info Browse_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Aggregation_Code_Manager Code_Manager, Item_Lookup_Object All_Items_Lookup,
            List<Thematic_Heading> Thematic_Headings, User_Object Current_User,
            IP_Restriction_Ranges IP_Restrictions,
            HTML_Based_Content Static_Web_Content,
            Custom_Tracer Tracer )
        {
            currentUser = Current_User;
            base.Hierarchy_Object = Hierarchy_Object;
            currentMode = Current_Mode;
            Skin = HTML_Skin;
            translator = Translator;
            thisBrowseObject = Browse_Object;
            thisStaticBrowseObject = Static_Web_Content;
            codeManager = Code_Manager;
            itemList = All_Items_Lookup;
            thematicHeadings = Thematic_Headings;
            ipRestrictions = IP_Restrictions;
            resultsStatistics = Results_Statistics;
            pagedResults = Paged_Results;

            NameValueCollection form = HttpContext.Current.Request.Form;
            if (form["item_action"] != null)
            {
                string action = form["item_action"].ToLower().Trim();

                if (action == "add_aggregation")
                {
                    SobekCM_Database.User_Set_Aggregation_Home_Page_Flag(currentUser.UserID, base.Hierarchy_Object.Aggregation_ID, true, Tracer);
                    currentUser.Set_Aggregation_Home_Page_Flag(base.Hierarchy_Object.Code, base.Hierarchy_Object.Name, true);
                    HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "Added aggregation to your home page");
                }

                if (action == "remove_aggregation")
                {
                    int removeAggregationID = base.Hierarchy_Object.Aggregation_ID;
                    string remove_code = base.Hierarchy_Object.Code;
                    string remove_name = base.Hierarchy_Object.Name;

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

                if (action == "private_folder")
                {
                    User_Folder thisFolder = currentUser.Get_Folder(form["aggregation"]);
                    if (SobekCM_Database.Edit_User_Folder(thisFolder.Folder_ID, currentUser.UserID, -1, thisFolder.Folder_Name, false, String.Empty, Tracer) >= 0)
                        thisFolder.isPublic = false;
                }


                if (action == "email")
                {
                    string address = form["email_address"].Replace(";", ",").Trim();
                    string comments = form["email_comments"].Trim();
                    string format = form["email_format"].Trim().ToUpper();

                    if (address.Length > 0)
                    {
                        // Determine the email format
                        bool is_html_format = true;
                        if (format == "TEXT")
                            is_html_format = false;

                        // CC: the user, unless they are already on the list
                        string cc_list = currentUser.Email;
                        if (address.ToUpper().IndexOf(currentUser.Email.ToUpper()) >= 0)
                            cc_list = String.Empty;

                        // Send the email
                        string any_error = URL_Email_Helper.Send_Email(address, cc_list, comments, currentUser.Full_Name, currentMode.SobekCM_Instance_Abbreviation, is_html_format, HttpContext.Current.Items["Original_URL"].ToString(), base.Hierarchy_Object.Name, "home");
                        HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", any_error.Length > 0 ? any_error : "Your email has been sent");

                        currentMode.isPostBack = true;

                        // Do this to force a return trip (cirumnavigate cacheing)
                        string original_url = HttpContext.Current.Items["Original_URL"].ToString();
                        if (original_url.IndexOf("?") < 0)
                            HttpContext.Current.Response.Redirect(original_url + "?p=" + DateTime.Now.Millisecond, false);
                        else
                            HttpContext.Current.Response.Redirect(original_url + "&p=" + DateTime.Now.Millisecond, false);
                    }
                }
            }
            
            // If this is a search, verify it is a valid search type
            if (currentMode.Mode == Display_Mode_Enum.Search)
            {
                // Not every collection has every search type...
                ReadOnlyCollection<Search_Type_Enum> possibleSearches = base.Hierarchy_Object.Search_Types;
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
                        currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                    }
                }
            }

            if (currentMode.Mode == Display_Mode_Enum.Search)
            {
                collectionViewer = AggregationViewer_Factory.Get_Viewer(currentMode.Search_Type, base.Hierarchy_Object, currentMode, currentUser);
            }


            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Home)
            {
                collectionViewer = AggregationViewer_Factory.Get_Viewer(base.Hierarchy_Object.Views_And_Searches[0], base.Hierarchy_Object, currentMode);
            }

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Info)
            {
                if ( resultsStatistics == null )
                {
                    collectionViewer = new Static_Browse_Info_AggregationViewer(thisBrowseObject, thisStaticBrowseObject);
                }
                else
                {
                    collectionViewer = new DataSet_Browse_Info_AggregationViewer(thisBrowseObject, resultsStatistics, pagedResults, codeManager, itemList, currentUser );
                }
            }

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_By)
            {
                collectionViewer = new Metadata_Browse_AggregationViewer( Current_Mode, Hierarchy_Object, Tracer );
            }

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Map)
            {
                collectionViewer = new Map_Browse_AggregationViewer(Current_Mode, Hierarchy_Object, Tracer);
            }

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Item_Count)
            {
                collectionViewer = new Item_Count_AggregationViewer(Current_Mode, Hierarchy_Object);
            }

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Usage_Statistics)
            {
                collectionViewer = new Usage_Statistics_AggregationViewer(Current_Mode, Hierarchy_Object);
            }

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Private_Items)
            {
                collectionViewer = new Private_Items_AggregationViewer(Current_Mode, Hierarchy_Object, Tracer);
            }

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Admin_View)
            {
                collectionViewer = new Admin_AggregationViewer(Hierarchy_Object);
            }


            if (collectionViewer != null)
            {
                collectionViewer.Translator = translator;
                collectionViewer.HTML_Skin = HTML_Skin;
                collectionViewer.CurrentMode = Current_Mode;
                collectionViewer.CurrentObject = Hierarchy_Object;
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

        #region Public method to write the internal header

        /// <summary> Adds the internal header HTML for this specific HTML writer </summary>
        /// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
        /// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
        public override void Add_Internal_Header_HTML(TextWriter Output, User_Object Current_User)
        {
            if ((Current_User != null) && ( currentMode.Aggregation.Length > 0 ) && ( currentMode.Aggregation.ToUpper() != "ALL" ) && ((Current_User.Is_Aggregation_Curator(currentMode.Aggregation)) || (Current_User.Is_Internal_User) || ( Current_User.Can_Edit_All_Items( currentMode.Aggregation ))))
            {
                Output.WriteLine("  <table cellspacing=\"0\" id=\"internalheader_aggr\">");
                Output.WriteLine("    <tr height=\"45px\">");
                Output.WriteLine("      <td align=\"left\" width=\"100px\">");
                Output.WriteLine("          <button title=\"Hide Internal Header\" class=\"intheader_button_aggr hide_intheader_button_aggr\" onclick=\"return hide_internal_header();\" alt=\"Hide Internal Header\"></button>");
                Output.WriteLine("      </td>");

                Output.WriteLine("      <td align=\"center\" valign=\"middle\">");

                // Add button to view private items
                Display_Mode_Enum displayMode = currentMode.Mode;
                string submode = currentMode.Info_Browse_Mode;

                currentMode.Mode = Display_Mode_Enum.Aggregation_Private_Items;
                currentMode.Info_Browse_Mode = String.Empty;
                Output.WriteLine("          <button title=\"View Private Items\" class=\"intheader_button_aggr view_private_items\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\" ></button>");

                // Add button to view item count information
                currentMode.Mode = Display_Mode_Enum.Aggregation_Item_Count;
                Output.WriteLine("          <button title=\"View Item Count\" class=\"intheader_button_aggr show_item_count\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                // Add button to view usage statistics information
                currentMode.Mode = Display_Mode_Enum.Aggregation_Usage_Statistics;
                Output.WriteLine("          <button title=\"View Usage Statistics\" class=\"intheader_button_aggr show_usage_statistics\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                // Add admin view is system administrator
                if ((Current_User.Is_System_Admin) || (Current_User.Is_Aggregation_Curator(Hierarchy_Object.Code)))
                {
                    currentMode.Mode = Display_Mode_Enum.Administrative;
                    currentMode.Admin_Type = Admin_Type_Enum.Aggregation_Single;
                    Output.WriteLine("          <button title=\"Edit Administrative Information\" class=\"intheader_button_aggr admin_view_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\" ></button>");
                }
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation_Admin_View;
                    Output.WriteLine("          <button title=\"View Administrative Information\" class=\"intheader_button_aggr admin_view_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\" ></button>");
                }
                Output.WriteLine("      </td>");

                currentMode.Info_Browse_Mode = submode;
                currentMode.Mode = displayMode;

                // Add the HELP icon next
                Output.WriteLine("      <td align=\"left\" width=\"30px\">");
                Output.WriteLine("        <span class=\"intheader_help\"><a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "help/aggrheader\" title=\"Help regarding this header\" ><img src=\"" + currentMode.Base_URL + "default/images/help_button_darkgray.jpg\" alt=\"?\" title=\"Help regarding this header\" /></a></span>");
                Output.WriteLine("      </td>");

                Add_Internal_Header_Search_Box(Output);

                Output.WriteLine("    </tr>");
                
                Output.WriteLine("  </table>");
            }
            else
            {
                base.Add_Internal_Header_HTML(Output, Current_User);
            }
        }

        #endregion

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
                return base.emptybehaviors;
            }
        }

        #region Public method to write HTML to the output stream

        /// <summary> Writes the HTML generated by this administrative html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aggregation_HtmlSubwriter.Write_HTML", "Rendering HTML");

            // Draw the banner and add links to the other views first
            if (collectionViewer.Type != Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search)
            {
                // Add all the other view tabs as well
                Add_Other_View_Tabs(Output, false, "ViewsBrowsesRow");
            }

            // If this is the map browse, end the page container here
            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Map)
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

                const string formName = "search_form";
                if (currentMode.Mode == Display_Mode_Enum.Aggregation_Home)
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

                    // Add the scripts needed
                    Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.6.2.min.js\"></script>");
                    Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.8.16.custom.min.js\"></script>");
                    Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");
                    Output.WriteLine();


                    #region Email form

                    if (currentUser != null)
                    {
                        Output.WriteLine("<!-- Email form -->");
                        Output.WriteLine("<div class=\"email_popup_div\" id=\"form_email\" style=\"display:none;\">");
                        Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">S<span class=\"smaller\">END THIS</span> C<span class=\"smaller\">OLLECTION TO A</span> F<span class=\"smaller\">RIEND</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"email_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                        Output.WriteLine("  <br />");
                        Output.WriteLine("  <fieldset><legend>Enter the email information below &nbsp; </legend>");
                        Output.WriteLine("    <br />");
                        Output.WriteLine("    <table class=\"popup_table\">");


                        // Add email address line
                        Output.Write("      <tr align=\"left\"><td width=\"80px\"><label for=\"email_address\">To:</label></td>");
                        Output.WriteLine("<td><input class=\"email_input\" name=\"email_address\" id=\"email_address\" type=\"text\" value=\"" + currentUser.Email + "\" onfocus=\"javascript:textbox_enter('email_address', 'email_input_focused')\" onblur=\"javascript:textbox_leave('email_address', 'email_input')\" /></td></tr>");

                        // Add comments area
                        Output.Write("      <tr align=\"left\" valign=\"top\"><td><br /><label for=\"email_comments\">Comments:</label></td>");
                        Output.WriteLine("<td><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"email_comments\" id=\"email_comments\" class=\"email_textarea\" onfocus=\"javascript:textbox_enter('email_comments','email_textarea_focused')\" onblur=\"javascript:textbox_leave('email_comments','email_textarea')\"></textarea></td></tr>");

                        // Add format area
                        Output.Write("      <tr align=\"left\" valign=\"top\"><td>Format:</td>");
                        Output.Write("<td><input type=\"radio\" name=\"email_format\" id=\"email_format_html\" value=\"html\" checked=\"checked\" /> <label for=\"email_format_html\">HTML</label> &nbsp; &nbsp; ");
                        Output.WriteLine("<input type=\"radio\" name=\"email_format\" id=\"email_format_text\" value=\"text\" /> <label for=\"email_format_text\">Plain Text</label></td></tr>");


                        Output.WriteLine("    </table>");
                        Output.WriteLine("    <br />");
                        Output.WriteLine("  </fieldset><br />");
                        Output.WriteLine("  <center><a href=\"\" onclick=\"return email_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/send_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center><br />");
                        Output.WriteLine("</div>");
                        Output.WriteLine();
                    }

                    #endregion

                    #region Share form

                    // Calculate the title and url
                    string title = HttpUtility.HtmlEncode(Hierarchy_Object.Name);
                    string share_url = HttpContext.Current.Items["Original_URL"].ToString().Replace("&", "%26").Replace("?", "%3F").Replace("http://", "").Replace("=", "%3D").Replace("\"", "&quot;");


                    Output.WriteLine("<!-- Share form -->");
                    Output.WriteLine("<div class=\"share_popup_div\" id=\"share_form\" style=\"display:none;\">");

                    Output.WriteLine("<a href=\"http://www.facebook.com/share.php?t=" + title + "&amp;u=" + share_url + "\" target=\"FACEBOOK_WINDOW\" onmouseover=\"facebook_share.src='" + currentMode.Base_URL + "default/images/facebook_share_h.gif'\" onmouseout=\"facebook_share.src='" + currentMode.Base_URL + "default/images/facebook_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"facebook_share\" name=\"facebook_share\" src=\"" + currentMode.Base_URL + "default/images/facebook_share.gif\" alt=\"FACEBOOK\" /></a>");
                    Output.WriteLine("<a href=\"http://buzz.yahoo.com/buzz?targetUrl=" + share_url + "&amp;headline=" + title + "\" target=\"YAHOOBUZZ_WINDOW\" onmouseover=\"yahoobuzz_share.src='" + currentMode.Base_URL + "default/images/yahoobuzz_share_h.gif'\" onmouseout=\"yahoobuzz_share.src='" + currentMode.Base_URL + "default/images/yahoobuzz_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoobuzz_share\" name=\"yahoobuzz_share\" src=\"" + currentMode.Base_URL + "default/images/yahoobuzz_share.gif\" alt=\"YAHOO BUZZ\" /></a>");
                    Output.WriteLine("<br />");

                    Output.WriteLine("<a href=\"http://twitter.com/home?status=Currently reading " + share_url + "\" target=\"TWITTER_WINDOW\" onmouseover=\"twitter_share.src='" + currentMode.Base_URL + "default/images/twitter_share_h.gif'\" onmouseout=\"twitter_share.src='" + currentMode.Base_URL + "default/images/twitter_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"twitter_share\" name=\"twitter_share\" src=\"" + currentMode.Base_URL + "default/images/twitter_share.gif\" alt=\"TWITTER\" /></a>");
                    Output.WriteLine("<a href=\"http://www.google.com/bookmarks/mark?op=add&amp;bkmk=" + share_url + "&amp;title=" + title + "\" target=\"GOOGLE_WINDOW\" onmouseover=\"google_share.src='" + currentMode.Base_URL + "default/images/google_share_h.gif'\" onmouseout=\"google_share.src='" + currentMode.Base_URL + "default/images/google_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"google_share\" name=\"google_share\" src=\"" + currentMode.Base_URL + "default/images/google_share.gif\" alt=\"GOOGLE SHARE\" /></a>");
                    Output.WriteLine("<br />");

                    Output.WriteLine("<a href=\"http://www.stumbleupon.com/submit?url=" + share_url + "&amp;title=" + title + "\" target=\"STUMBLEUPON_WINDOW\" onmouseover=\"stumbleupon_share.src='" + currentMode.Base_URL + "default/images/stumbleupon_share_h.gif'\" onmouseout=\"stumbleupon_share.src='" + currentMode.Base_URL + "default/images/stumbleupon_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"stumbleupon_share\" name=\"stumbleupon_share\" src=\"" + currentMode.Base_URL + "default/images/stumbleupon_share.gif\" alt=\"STUMBLEUPON\" /></a>");
                    Output.WriteLine("<a href=\"http://myweb.yahoo.com/myresults/bookmarklet?t=" + title + "&amp;u=" + share_url + "\" target=\"YAHOO_WINDOW\" onmouseover=\"yahoo_share.src='" + currentMode.Base_URL + "default/images/yahoo_share_h.gif'\" onmouseout=\"yahoo_share.src='" + currentMode.Base_URL + "default/images/yahoo_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoo_share\" name=\"yahoo_share\" src=\"" + currentMode.Base_URL + "default/images/yahoo_share.gif\" alt=\"YAHOO SHARE\" /></a>");
                    Output.WriteLine("<br />");

                    Output.WriteLine("<a href=\"http://digg.com/submit?phase=2&amp;url=" + share_url + "&amp;title=" + title + "\" target=\"DIGG_WINDOW\" onmouseover=\"digg_share.src='" + currentMode.Base_URL + "default/images/digg_share_h.gif'\" onmouseout=\"digg_share.src='" + currentMode.Base_URL + "default/images/digg_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"digg_share\" name=\"digg_share\" src=\"" + currentMode.Base_URL + "default/images/digg_share.gif\" alt=\"DIGG\" /></a>");
                    Output.WriteLine("<a onmouseover=\"favorites_share.src='" + currentMode.Base_URL + "default/images/favorites_share_h.gif'\" onmouseout=\"favorites_share.src='" + currentMode.Base_URL + "default/images/favorites_share.gif'\" onclick=\"javascript:add_to_favorites();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"favorites_share\" name=\"favorites_share\" src=\"" + currentMode.Base_URL + "default/images/favorites_share.gif\" alt=\"MY FAVORITES\" /></a>");
                    Output.WriteLine("<br />");

                    Output.WriteLine("</div>");
                    Output.WriteLine();

                    #endregion

                    if (collectionViewer.Type != Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search)
                    {
                        Output.WriteLine("<div class=\"SobekSearchPanel\">");
                        Add_Sharing_Buttons(Output, formName, "SobekResultsSort");
                    }
                }
                else
                {
                    if (currentMode.Mode != Display_Mode_Enum.Aggregation_Browse_Map)
                        Output.WriteLine("<div class=\"SobekSearchPanel\">");
                }

                if (collectionViewer.Type == Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search)
                {
                    StringBuilder builder = new StringBuilder(2000);
                    StringWriter writer = new StringWriter(builder);
                    Add_Sharing_Buttons(writer, formName, "SobekHomeBannerButton");
                    ((Rotating_Highlight_Search_AggregationViewer)collectionViewer).Sharing_Buttons_HTML = builder.ToString();

                    collectionViewer.Add_Search_Box_HTML(Output, Tracer);

                    Add_Other_View_Tabs(Output, true, "SobekHomeBannerShowSelectRow");
                }
                else
                {
                    collectionViewer.Add_Search_Box_HTML(Output, Tracer);
                    Output.WriteLine(currentMode.Mode != Display_Mode_Enum.Aggregation_Browse_Map ? "</div>" : "<div id=\"pagecontainer_resumed\">");
                }

                Output.WriteLine();
            }
            else
            {
                collectionViewer.Add_Search_Box_HTML(Output, Tracer);
            }

            // Prepare to add the collection selector information, but first, check to see if this the main home page
            bool sobekcm_main_home_page = false;
            if ((currentMode.Mode == Display_Mode_Enum.Aggregation_Home) && (Hierarchy_Object.Code == "all"))
            {
                sobekcm_main_home_page = true;
            }

            // Add the collection selector, if it ever appears here
            if ((!sobekcm_main_home_page) && (collectionViewer.Selection_Panel_Display != Selection_Panel_Display_Enum.Never) && (Hierarchy_Object.Children_Count > 0))
            {
                // Get the collection of children
                ReadOnlyCollection<Item_Aggregation_Related_Aggregations> child_aggregations = Hierarchy_Object.Children;

                // Set the strings for the tab here
                string show_collect_groups = "SHOW COLLECTION GROUPS";
                string show_collect = "SHOW COLLECTIONS";
                string show_subcollect = "SHOW SUBCOLLECTIONS";
                string hide_collect_groups = "HIDE COLLECTION GROUPS";
                string hide_collect = "HIDE COLLECTIONS";
                string hide_subcollect = "HIDE SUBCOLLECTIONS";
                string select_collect_groups = "Select collection groups to include in search:";
                string select_collect = "Select collections to include in search:";
                string select_subcollect = "Select subcollections to include in search:";

                // Change text if this is Spanish
                if (currentMode.Language == Web_Language_Enum.Spanish)
                {
                    show_collect_groups = "SELECCIONE GRUPOS DE COLECCIONES";
                    show_collect = "SELECCIONE COLECCIONES";
                    show_subcollect = "SELECCIONE SUBCOLECCIONES";
                    hide_collect_groups = "ESCONDA GRUPOS DE COLECCIONES";
                    hide_collect = "ESCONDA COLECCIONES";
                    hide_subcollect = "ESCONDA SUBCOLECCIONES";
                    select_collect_groups = "Seleccione grupos de colecciones para incluir en la búsqueda:";
                    select_collect = "Seleccione colecciones para incluir en la búsqueda:";
                    select_subcollect = "Seleccione subcolecciones para incluir en la búsqueda:";

                }

                // Change the text if this is french
                if (currentMode.Language == Web_Language_Enum.French)
                {
                    show_collect_groups = "VOIR LE GROUPE DE COLLECTION";
                    show_collect = "VOIR LES COLLECTIONS";
                    show_subcollect = "VOIR LES SOUSCOLLECTIONS";
                    hide_collect_groups = "SUPPRIMER LE GROUPE DE COLLECTION";
                    hide_collect = "SUPPRIMER LES COLLECTIONS";
                    hide_subcollect = "SUPPRIMER LES SOUSCOLLECTIONS";
                    select_collect_groups = "Choisir les group de collection pour inclure dans votre recherche:";
                    select_collect = "Choisir les collections pour inclure dans votre recherche:";
                    select_subcollect = "Choisir les souscollections pour inclure dans votre recherche:";
                }

                // Determine the sub text to use
                string select_text = select_subcollect;
                string show_text = show_subcollect;
                string hide_text = hide_subcollect;
                if (Hierarchy_Object.Code == "all")
                {
                    select_text = select_collect_groups;
                    show_text = show_collect_groups;
                    hide_text = hide_collect_groups;
                }
                else
                {
                    if (child_aggregations[0].Type.ToUpper() == "COLLECTION")
                    {
                        select_text = select_collect;
                        show_text = show_collect;
                        hide_text = hide_collect;
                    }
                }

                if ((collectionViewer.Selection_Panel_Display == Selection_Panel_Display_Enum.Selectable) && (!currentMode.Show_Selection_Panel))
                {
                    Output.WriteLine("<div class=\"ShowSelectRow\">");
                    //currentMode.Show_Selection_Panel = true;
                    Output.WriteLine("  <a href=\"\" onclick=\"return set_subaggr_display('true');\">" + Down_Tab_Start + show_text + Down_Tab_End + "</a>");
                    //currentMode.Show_Selection_Panel = false;
                    Output.WriteLine("</div>");
                    Output.WriteLine();
                }
                else
                {
                    if (collectionViewer.Selection_Panel_Display == Selection_Panel_Display_Enum.Selectable)
                    {
                        Output.WriteLine("<div class=\"HideSelectRow\">");
                        //currentMode.Show_Selection_Panel = false;
                        Output.WriteLine("  <a href=\"\" onclick=\"return set_subaggr_display('false');\">" + Unselected_Tab_Start + hide_text + Unselected_Tab_End + "</a>");
                        //currentMode.Show_Selection_Panel = true;
                        Output.WriteLine("</div>");
                        Output.WriteLine();
                    }
                    else
                    {
                        Output.WriteLine("<br />");
                    }

                    Output.WriteLine("<div class=\"SobekSelectPanel\"><b>" + select_text + "</b>");
                    Output.WriteLine("  <br />");

                    Display_Mode_Enum lastDisplayMode = currentMode.Mode;
                    string thisAggr = currentMode.Aggregation;
                    string thisAlias = currentMode.Aggregation_Alias;
                    currentMode.Aggregation_Alias = String.Empty;
                    currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                    foreach (Item_Aggregation_Related_Aggregations t in child_aggregations)
                    {
                        if ((t.Active) && (!t.Hidden))
                        {
                            Output.WriteLine("  <span class=\"SobekSelectCheckBox\">");
                            Output.Write("    <input type=\"checkbox\" value=\"" + t.Code + "\" name=\"checkgroup\"");
                            Output.WriteLine("< checked=\"checked\" />");
                        //    Output.WriteLine(currentMode.SubAggregation.IndexOf(t.Code) < 0 ? " />" : " checked />");
                            currentMode.Aggregation = t.Code;
                            Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\">" + t.Name + "</a>");
                            Output.WriteLine("  </span>");
                            Output.WriteLine("  <br />");
                        }
                    }
                    currentMode.Aggregation = thisAggr;
                    currentMode.Aggregation_Alias = thisAlias;
                    currentMode.Mode = lastDisplayMode;
                    Output.WriteLine("</div>");
                }
            }

            
            Output.WriteLine("</form>");
            

            // Add the secondary HTML ot the home page
            bool finish_page = true;
            if ((currentMode.Mode == Display_Mode_Enum.Aggregation_Home) || (collectionViewer.Always_Display_Home_Text))
            {
                finish_page = add_home_html(Output, Tracer);
            }
            else
            {
                collectionViewer.Add_Secondary_HTML(Output, Tracer);
            }

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Info)
            {
                if (resultsStatistics != null)
                    finish_page = false;
            }
            return finish_page;
        }

        private void Add_Other_View_Tabs(TextWriter Output, bool downward_tabs, string style )
        {
            // If this skin has top-level navigation suppressed, skip the top tabs
            if (htmlSkin.Suppress_Top_Navigation)
            {
                Output.WriteLine("<br />");
                return;
            }

            // Get ready to draw the tabs
            string home = "HOME";
            string libraryHome = currentMode.SobekCM_Instance_Abbreviation.ToUpper() + " HOME";
            string allItems = "ALL ITEMS";
            string newItems = "NEW ITEMS";
            string myCollections = "MY COLLECTIONS";
            string partners = "BROWSE PARTNERS";
            string browseBy = "BROWSE BY";
            const string browseMap = "MAP BROWSE";

            if (currentMode.Language == Web_Language_Enum.Spanish)
            {
                home = "INICIO";
                libraryHome = "INICIO " + currentMode.SobekCM_Instance_Abbreviation.ToUpper();
                allItems = "TODOS LOS ARTÍCULOS";
                newItems = "NUEVOS ARTÍCULOS";
                browseBy = "BÚSQUEDA POR";
                partners = "AFILIADOS";
                myCollections = "MIS COLECCIONES";
            }

            if (currentMode.Language == Web_Language_Enum.French)
            {
                home = "PAGE D'ACCUEIL";
                libraryHome = "PAGE D'ACCUEIL";
                allItems = "TOUS LES ARTICLES";
                newItems = "NOUVEAUX ARTICLES";
                browseBy = "PARCOURIR PAR";

            }

            // Save the current mode and browse
            Display_Mode_Enum thisMode = currentMode.Mode;
            Search_Type_Enum thisSearch = currentMode.Search_Type;
            Home_Type_Enum thisHomeType = currentMode.Home_Type;
            string browse_code = currentMode.Info_Browse_Mode;
            if (thisMode == Display_Mode_Enum.Aggregation_Browse_Info)
            {
                browse_code = currentMode.Info_Browse_Mode;
            }

            Output.WriteLine("<!-- Add the different applicable collection views -->");
            Output.WriteLine("<div class=\"" + style + "\">");
            Output.WriteLine("");


            string unselected_start = Unselected_Tab_Start;
            string unselected_end = Unselected_Tab_End;
            string selected_start = Selected_Tab_Start;
            string selected_end = Selected_Tab_End;
            if (downward_tabs)
            {
                unselected_start = Down_Tab_Start;
                unselected_end = Down_Tab_End;
                selected_start = Down_Selected_Tab_Start;
                selected_end = Down_Selected_Tab_End;
            }

            // Get the home search type (just to do a matching in case it was explicitly requested)
            Item_Aggregation.CollectionViewsAndSearchesEnum homeView = Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search;
            if (Hierarchy_Object.Views_And_Searches.Count > 0)
            {
                homeView = Hierarchy_Object.Views_And_Searches[0];
            }

            // Remove any search string
            string current_search = currentMode.Search_String;
            currentMode.Search_String = String.Empty;

            // Add the HOME tab
            if (Hierarchy_Object.Code == "all")
            {
                if (((currentMode.Mode == Display_Mode_Enum.Aggregation_Home) && (currentMode.Home_Type != Home_Type_Enum.Personalized) && (currentMode.Home_Type != Home_Type_Enum.Partners_List) && (currentMode.Home_Type != Home_Type_Enum.Partners_Thumbnails)) ||
                    ((currentMode.Mode == Display_Mode_Enum.Search) &&
                     (Aggregation_Nav_Bar_HTML_Factory.Do_Search_Types_Match(homeView, currentMode.Search_Type))))
                {
                    Output.WriteLine("  " + selected_start + libraryHome + selected_end);
                }
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                    currentMode.Home_Type = Home_Type_Enum.List;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + unselected_start + libraryHome + unselected_end + "</a>");
                }
            }
            else
            {
                if ((currentMode.Mode == Display_Mode_Enum.Aggregation_Home) ||
                    ((currentMode.Mode == Display_Mode_Enum.Search) &&
                     (Aggregation_Nav_Bar_HTML_Factory.Do_Search_Types_Match(homeView, currentMode.Search_Type))))
                {
                    Output.WriteLine("  " + selected_start + home + selected_end);
                }
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                    currentMode.Home_Type = Home_Type_Enum.List;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + unselected_start + home + unselected_end + "</a>");
                }
            }

            // Add any additional search types
            currentMode.Mode = thisMode;
            for (int i = 1; i < Hierarchy_Object.Views_And_Searches.Count; i++)
            {
                Output.Write(Aggregation_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(Hierarchy_Object.Views_And_Searches[i], currentMode, translator, downward_tabs));
            }

            // Replace any search string
            currentMode.Search_String = current_search;

            // Check for the existence of any BROWSE BY pages
            if (Hierarchy_Object.Has_Browse_By_Pages)
            {
                if ((thisMode == Display_Mode_Enum.Aggregation_Browse_By) || ( currentMode.Is_Robot ))
                {
                    Output.WriteLine("  " + unselected_start + browseBy + unselected_end);
                }
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation_Browse_By;
                    currentMode.Info_Browse_Mode = String.Empty;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + unselected_start + browseBy + unselected_end + "</a>");
                }
            }

            // Check for the existence of any MAP BROWSE pages
            if (Hierarchy_Object.Views_And_Searches.Contains( Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Browse ))
            {
                if (thisMode == Display_Mode_Enum.Aggregation_Browse_Map)
                {
                    Output.WriteLine("  " + selected_start + browseMap + selected_end);
                }
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation_Browse_Map;
                    currentMode.Info_Browse_Mode = String.Empty;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + unselected_start + browseMap + unselected_end + "</a>");
                }
            }

            // Add all the BROWSES
            currentMode.Mode = Display_Mode_Enum.Aggregation_Browse_Info;

            // Find the URL for all these browses
            currentMode.Info_Browse_Mode = "XYXYXYXYXY";
            string redirect_url = currentMode.Redirect_URL();

            // Only show ALL and NEW if they are in the collection list of searches and views
            int included_browses = 0;
            if (Hierarchy_Object.Views_And_Searches.Contains(Item_Aggregation.CollectionViewsAndSearchesEnum.All_New_Items))
            {
                // First, look for 'ALL'
                if (Hierarchy_Object.Contains_Browse_Info("all"))
                {
                    if (browse_code == "all")
                    {
                        Output.WriteLine("  " + selected_start + allItems + selected_end);
                    }
                    else
                    {
                        Output.WriteLine("  <a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/","/") + "\">" + unselected_start + allItems + unselected_end + "</a>");
                    }
                    included_browses++;
                }

                // Next, look for 'NEW'
                if ((Hierarchy_Object.Contains_Browse_Info("new")) && (!currentMode.Is_Robot))
                {
                    if (browse_code == "new")
                    {
                        Output.WriteLine("  " + selected_start + newItems + selected_end);
                    }
                    else
                    {
                        Output.WriteLine("  <a href=\"" + redirect_url.Replace("XYXYXYXYXY", "new").Replace("/info/", "/") + "\">" + unselected_start + newItems + unselected_end + "</a>");
                    }
                    included_browses++;
                }
            }

            // Are there any additional browses to include?
            ReadOnlyCollection<Item_Aggregation_Browse_Info> otherBrowses = Hierarchy_Object.Browse_Home_Pages(currentMode.Language);
            if (otherBrowses.Count > included_browses)
            {
                // Now, step through the sorted list
                foreach (Item_Aggregation_Browse_Info thisBrowseObj in otherBrowses)
                {
                    if ((thisBrowseObj.Code != "all") && (thisBrowseObj.Code != "new"))
                    {
                        currentMode.Info_Browse_Mode = thisBrowseObj.Code;
                        if (browse_code == thisBrowseObj.Code)
                        {
                            Output.WriteLine("  " + selected_start + thisBrowseObj.Get_Label(currentMode.Language).ToUpper() + selected_end);
                        }
                        else
                        {
                            Output.WriteLine("  <a href=\"" + redirect_url.Replace("XYXYXYXYXY", thisBrowseObj.Code) + "\">" + unselected_start + thisBrowseObj.Get_Label(currentMode.Language).ToUpper() + unselected_end + "</a>");
                        }
                    }
                }
            }

            // If there is a user and this is the main home page, show MY COLLECTIONS
            if ((currentUser != null))
            {
                if (Hierarchy_Object.Code == "all")
                {
                    // Show personalized
                    if (thisHomeType == Home_Type_Enum.Personalized)
                    {
                        Output.WriteLine("  " + selected_start + myCollections + selected_end);
                    }
                    else
                    {
                        currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                        currentMode.Home_Type = Home_Type_Enum.Personalized;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + unselected_start + myCollections + unselected_end + "</a>");
                    }
                }
                else
                {
                    if (currentUser.Is_Aggregation_Curator(Hierarchy_Object.Code))
                    {
                        // Return the code and mode back
                        currentMode.Info_Browse_Mode = String.Empty;
                        currentMode.Search_Type = thisSearch;
                        currentMode.Mode = thisMode;
                        currentMode.Home_Type = thisHomeType;

                        Output.Write(Aggregation_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(Item_Aggregation.CollectionViewsAndSearchesEnum.Admin_View, currentMode, translator, downward_tabs));
                    }

                }
            }

            // Show institution
            if (Hierarchy_Object.Code == "all")
            {
                if (((thisHomeType == Home_Type_Enum.Partners_List) || (thisHomeType == Home_Type_Enum.Partners_Thumbnails)))
                {
                    Output.WriteLine("  " + selected_start + partners + selected_end);
                }
                else
                {
                    // Is this library set to show the partners tab?
                    if ( SobekCM_Library_Settings.Include_Partners_On_System_Home )
                    {
                        currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                        currentMode.Home_Type = Home_Type_Enum.Partners_List;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + unselected_start + partners + unselected_end + "</a>");
                    }
                }
            }

            Output.WriteLine("");
            Output.WriteLine("</div>");
            Output.WriteLine();

            // Return the code and mode back
            currentMode.Info_Browse_Mode = browse_code;
            currentMode.Search_Type = thisSearch;
            currentMode.Mode = thisMode;
            currentMode.Home_Type = thisHomeType;
        }

        private void Add_Sharing_Buttons( TextWriter Output, string form_name, string style )
        {
            #region Add the buttons for sharing, emailing, etc..

            Output.WriteLine("  <span class=\"" + style + "\">");
            Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".print_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".print_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button.gif'\" onclick=\"window.print(); return false;\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"print_button\" id=\"print_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button.gif\" title=\"Print this page\" alt=\"PRINT\" /></a>");

            if (currentUser != null)
            {
                if ((currentMode.Home_Type == Home_Type_Enum.Personalized) && (currentMode.Aggregation.Length == 0))
                {
                    Output.Write("<img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"send_button\" id=\"send_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif\" title=\"Send this to someone\" alt=\"SEND\" />");
                }
                else
                {
                    Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif'\" onclick=\"return email_form_open2('send_button','');\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"send_button\" id=\"send_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif\" title=\"Send this to someone\" alt=\"SEND\" /></a>");

                }
                if (Hierarchy_Object.Aggregation_ID > 0)
                {
                    if (currentUser.Is_On_Home_Page(currentMode.Aggregation))
                    {
                        Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".remove_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/remove_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".remove_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/remove_rect_button.gif'\" onclick=\"return remove_aggregation();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"remove_button\" id=\"remove_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/remove_rect_button.gif\" title=\"Remove this from my collections home page\" alt=\"REMOVE\" /></a>");
                    }
                    else
                    {
                        Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".add_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".add_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button.gif'\" onclick=\"return add_aggregation();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"add_button\" id=\"add_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button.gif\" title=\"Add this to my collections home page\" alt=\"ADD\" /></a>");
                    }
                }
            }
            else
            {
                Output.Write("<a href=\"?m=hmh\" onmouseover=\"document." + form_name + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif'\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"send_button\" id=\"send_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif\" title=\"Send this to someone\" alt=\"SEND\" /></a>");
                Output.Write("<a href=\"?m=hmh\" onmouseover=\"document." + form_name + ".add_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".add_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button.gif'\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"add_button\" id=\"add_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/add_rect_button.gif\" title=\"Save this to my collections home page\" alt=\"ADD\" /></a>");
            }

            if ((currentMode.Home_Type == Home_Type_Enum.Personalized) && (currentMode.Aggregation.Length == 0))
            {
                Output.Write("<img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"share_button\" id=\"share_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button.gif\" title=\"Share this collection\" />");
            }
            else
            {
                Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".share_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".share_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button.gif'\" onclick=\"return toggle_share_form2('share_button');\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"share_button\" id=\"share_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button.gif\" title=\"Share this collection\" alt=\"SHARE\" /></a>");
            }
            Output.WriteLine("</span>");

            #endregion
        }

        #endregion

        #region Public method to add controls to the place holder 

        /// <summary> Adds the tree view control to the provided place holder if this is the tree view main home page </summary>
        /// <param name="placeHolder"> Place holder into which to place the built tree control </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns>Flag indicates if secondary text contains controls </returns>
        public bool Add_Controls(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (((currentMode.Home_Type == Home_Type_Enum.Tree_Collapsed) || (currentMode.Home_Type == Home_Type_Enum.Tree_Expanded)) && (Hierarchy_Object.Code == "all"))
            {
                Tracer.Add_Trace("Aggregation_HtmlSubwriter.Add_Controls", "Adding tree view of collection hierarchy");

                // Make sure the ALL aggregations has the collection hierarchies
                if (Hierarchy_Object.Children_Count == -1)
                {
                    // Get the collection hierarchy information
                    SobekCM_Database.Add_Children_To_Main_Agg(Hierarchy_Object, Tracer);
                }

                Home_Type_Enum currentType = currentMode.Home_Type;
                currentMode.Home_Type = Home_Type_Enum.Tree_Expanded;
                string expand_url = currentMode.Redirect_URL();
                currentMode.Home_Type = Home_Type_Enum.Tree_Collapsed;
                string collapsed_url = currentMode.Redirect_URL();
                currentMode.Home_Type = currentType;

                Literal literal1 = new Literal
                                       { Text = string.Format("<div class=\"thematicHeading\">All Collections</div>" + Environment.NewLine + "<div class=\"SobekText\">" + Environment.NewLine + "<blockquote>" + Environment.NewLine + "<div align=\"right\"><a href=\"{0}\">Collapse All</a> | <a href=\"{1}\">Expand All</a></div>" + Environment.NewLine , collapsed_url, expand_url) };
                placeHolder.Controls.Add(literal1);

                // Create the treeview
                TreeView treeView1 = new TreeView
                                         {CssClass = "SobekCollectionTreeView", ExpandDepth = 0, NodeIndent = 15};

                // load the table of contents in the tree
                Create_TreeView_From_Collections(treeView1);

                // Add the tree view to the placeholder
                placeHolder.Controls.Add(treeView1);

                Literal literal2 = new Literal {Text = @"</blockquote></div>"};
                placeHolder.Controls.Add(literal2);

                return true;
            }

            if (collectionViewer.Secondary_Text_Requires_Controls)
            {
                collectionViewer.Add_Secondary_Controls(placeHolder, Tracer);
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
            // If this is a normal aggregation type ( i.e., not the library home ) just display the home text normally
            if ((currentMode.Aggregation.Length != 0) && (Hierarchy_Object.Aggregation_ID > 0))
            {
                Output.WriteLine("<br />");
                Output.WriteLine();
                Output.WriteLine("<div class=\"SobekText\">");

                string url_options = currentMode.URL_Options();
                string urlOptions1 = String.Empty;
                string urlOptions2 = String.Empty;
                if (url_options.Length > 0)
                {
                    urlOptions1 = "?" + url_options;
                    urlOptions2 = "&" + url_options;
                }

                // Add the highlights
                if ((Hierarchy_Object.Highlights.Count > 0) && (collectionViewer.Type != Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search))
                {
                    Output.WriteLine(Hierarchy_Object.Highlights[0].ToHTML(currentMode.Language, currentMode.Base_Design_URL + Hierarchy_Object.objDirectory).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2));
                }

                // Get the raw home hteml text
                string home_html = Hierarchy_Object.Get_Home_HTML(currentMode.Language, Tracer);

                // Determine the different counts as strings and replace if they exist
                if ((home_html.Contains("<%PAGES%>")) || (home_html.Contains("<%TITLES%>")) || (home_html.Contains("<%ITEMS%>")))
                {
                    if ((Hierarchy_Object.Page_Count < 0) && (Hierarchy_Object.Item_Count < 0) && (Hierarchy_Object.Title_Count < 0))
                    {
                        if ((!currentMode.Is_Robot) && (SobekCM_Database.Get_Item_Aggregation_Counts(Hierarchy_Object, Tracer)))
                        {
                            Cached_Data_Manager.Store_Item_Aggregation(Hierarchy_Object.Code, currentMode.Language_Code, Hierarchy_Object, Tracer);

                            string page_count = Int_To_Comma_String(Hierarchy_Object.Page_Count);
                            string item_count = Int_To_Comma_String(Hierarchy_Object.Item_Count);
                            string title_count = Int_To_Comma_String(Hierarchy_Object.Title_Count);

                            home_html = home_html.Replace("<%PAGES%>", page_count).Replace("<%ITEMS%>", item_count).Replace("<%TITLES%>", title_count);
                        }
                        else
                        {
                            home_html = home_html.Replace("<%PAGES%>", String.Empty).Replace("<%ITEMS%>", String.Empty).Replace("<%TITLES%>", String.Empty);
                        }
                    }
                    else
                    {
                        string page_count = Int_To_Comma_String(Hierarchy_Object.Page_Count);
                        string item_count = Int_To_Comma_String(Hierarchy_Object.Item_Count);
                        string title_count = Int_To_Comma_String(Hierarchy_Object.Title_Count);

                        home_html = home_html.Replace("<%PAGES%>", page_count).Replace("<%ITEMS%>", item_count).Replace("<%TITLES%>", title_count);
                    }
                }

                // Replace any item aggregation specific custom directives
                string original_home = home_html;
                home_html = Hierarchy_Object.Custom_Directives.Keys.Where(original_home.Contains).Aggregate(home_html, (current, thisKey) => current.Replace(thisKey, Hierarchy_Object.Custom_Directives[thisKey].Replacement_HTML));

                // Replace any standard directives last
                home_html = home_html.Replace("<%BASEURL%>", currentMode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2);

                // Output the adjusted home html
                Output.WriteLine(home_html);

                // If there are sub aggregations here, show them
                if (Hierarchy_Object.Children_Count > 0)
                {
                    // Get the list of all aggregation types
                    SortedList<string, string> aggregationTypes = new SortedList<string, string>();
                    foreach (Item_Aggregation_Related_Aggregations childAggr in Hierarchy_Object.Children.Where(childAggr => (!childAggr.Hidden) && (childAggr.Active)).Where(childAggr => !aggregationTypes.ContainsKey(childAggr.Type.ToUpper() + "S")))
                    {
                        aggregationTypes.Add(childAggr.Type.ToUpper() + "S", childAggr.Type.ToUpper() + "S");
                    }

                    // If all children were hidden, no need to continnue
                    if (aggregationTypes.Count > 0)
                    {
                        Output.WriteLine("<br />");

                        // Add the gray bar first
                        Output.Write("<h2>");
                        if (aggregationTypes.Count == 1)
                        {
                            // Write the name of the sub aggregations
                            StringBuilder aggregationTypeBuilder = new StringBuilder(30);
                            string[] splitter = aggregationTypes.Keys[0].Trim().Split(" ".ToCharArray());
                            foreach (string thisSplit in splitter.Where(thisSplit => thisSplit.Length > 0))
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

                            Output.Write(translator.Get_Translation(aggregationTypeBuilder.ToString().Trim(), currentMode.Language));

                        }
                        else
                        {
                            Output.Write(translator.Get_Translation("Child Collections", currentMode.Language));
                        }
                        Output.WriteLine("</h2>");

                        // Keep the last aggregation alias
                        string lastAlias = currentMode.Aggregation_Alias;
                        currentMode.Aggregation_Alias = String.Empty;

                        // Collect the html to write (this alphabetizes the children)
                        SortedList<string, string> html_list = new SortedList<string, string>();
                        foreach (Item_Aggregation_Related_Aggregations childAggr in Hierarchy_Object.Children)
                        {
                            if ((!childAggr.Hidden) && (childAggr.Active))
                            {
                                string name = childAggr.Name;
                                if (name.ToUpper() == "ADDED AUTOMATICALLY")
                                    name = childAggr.Code + " ( Added Automatically )";

                                currentMode.Aggregation = childAggr.Code.ToLower();
                                string image_url = currentMode.Base_URL + "design/aggregations/" + childAggr.Code + "/images/buttons/coll.gif";
                                if ((name.IndexOf("The ") == 0) && (name.Length > 4))
                                {
                                    html_list[name.Substring(4)] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(name, currentMode.Language) + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(name, currentMode.Language) + "</a></span>" + Environment.NewLine + "    </td>";
                                }
                                else
                                {
                                    html_list[name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(name, currentMode.Language) + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(name, currentMode.Language) + "</a></span>" + Environment.NewLine + "    </td>";
                                }
                            }
                        }

                        Output.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"5\" cellspacing=\"5\">");
                        int column_spot = 0;
                        Output.WriteLine("  <tr>");
                        Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");

                        foreach (string thisHtml in html_list.Values)
                        {
                            if (column_spot == 3)
                            {
                                Output.WriteLine("  </tr>");
                                Output.WriteLine("  <tr>");
                                Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");
                                column_spot = 0;
                            }

                            Output.WriteLine(thisHtml);
                            column_spot++;
                        }

                        if (column_spot == 2)
                        {
                            Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
                        }
                        if (column_spot == 1)
                        {
                            Output.WriteLine("    <td>&nbsp;</td>");
                        }
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("</table>");

                        // Restore the old alias
                        currentMode.Aggregation_Alias = lastAlias;
                    }
                }

                Output.WriteLine("</div>");
                currentMode.Aggregation = Hierarchy_Object.Code;
            }
            else
            {
                Output.WriteLine();
                Output.WriteLine("<div class=\"SobekText\">");
                Output.WriteLine("<br />");

                if ((currentMode.Home_Type != Home_Type_Enum.Personalized) && (currentMode.Home_Type != Home_Type_Enum.Partners_List) && (currentMode.Home_Type != Home_Type_Enum.Partners_Thumbnails))
                {
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

                        sobekcm_home_page_text = Hierarchy_Object.Get_Home_HTML(currentMode.Language, Tracer);

                        HttpContext.Current.Application["SobekCM_Home"] = sobekcm_home_page_text;
                    }
                    else
                    {
                        sobekcm_home_page_text = (string)sobekcm_home_page_obj;
                    }


                    int index = sobekcm_home_page_text.IndexOf("<%END%>");

                    string tabstart = "<img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\">";
                    string tabend = "</span><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";
                    string select_tabstart = "<img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\">";
                    string select_tabend = "</span><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";

                    // Determine the different counts as strings
                    string page_count = Int_To_Comma_String(Hierarchy_Object.Page_Count);
                    string item_count = Int_To_Comma_String(Hierarchy_Object.Item_Count);
                    string title_count = Int_To_Comma_String(Hierarchy_Object.Title_Count);

                    string url_options = currentMode.URL_Options();
                    string urlOptions1 = String.Empty;
                    string urlOptions2 = String.Empty;
                    if (url_options.Length > 0)
                    {
                        urlOptions1 = "?" + url_options;
                        urlOptions2 = "&" + url_options;
                    }

                    Output.WriteLine(index > 0 ? sobekcm_home_page_text.Substring(0, index).Replace("<%BASEURL%>", currentMode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%INTERFACE%>",currentMode.Base_Skin).Replace("<%WEBSKIN%>", currentMode.Base_Skin).Replace("<%TABSTART%>",tabstart).Replace("<%TABEND%>",tabend).Replace("<%SELECTED_TABSTART%>", select_tabstart).Replace("<%SELECTED_TABEND%>", select_tabend).Replace("<%PAGES%>", page_count).Replace("<%ITEMS%>", item_count).Replace("<%TITLES%>", title_count)
                        : sobekcm_home_page_text.Replace("<%BASEURL%>", currentMode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%INTERFACE%>",currentMode.Base_Skin).Replace("<%WEBSKIN%>",currentMode.Base_Skin).Replace("<%TABSTART%>", tabstart).Replace("<%TABEND%>", tabend).Replace("<%SELECTED_TABSTART%>", select_tabstart).Replace("<%SELECTED_TABEND%>", select_tabend).Replace("<%PAGES%>",page_count).Replace("<%ITEMS%>", item_count).Replace("<%TITLES%>", title_count));
                }
                Output.WriteLine("</div>");

                if ((currentMode.Home_Type == Home_Type_Enum.Partners_List) || (currentMode.Home_Type == Home_Type_Enum.Partners_Thumbnails))
                {
                    Output.WriteLine("<div class=\"SobekText\">");
                    Output.WriteLine("<br />");
                    Output.WriteLine("<p>Partners collaborating and contributing to digital collections and libraries include:</p>");
                }

                if (currentMode.Home_Type != Home_Type_Enum.Personalized)
                {
                    // See if there are actually aggregations linked to the  thematic headings
                    bool aggrsLinkedToThemes = false;
                    if ((!SobekCM_Library_Settings.Include_TreeView_On_System_Home) && ( thematicHeadings.Count > 0 ))
                    {
                        foreach (Thematic_Heading thisTheme in thematicHeadings)
                        {
                            if (codeManager.Aggregations_By_ThemeID(thisTheme.ThematicHeadingID).Count > 0)
                            {
                                aggrsLinkedToThemes = true;
                                break;
                            }
                        } 
                    }

                    // If aggregations are linked to themes, or if the tree view should always be displayed on home
                    if ((aggrsLinkedToThemes) || (SobekCM_Library_Settings.Include_TreeView_On_System_Home))
                    {
                        string listText = "LIST VIEW";
                        string descriptionText = "BRIEF VIEW";
                        string treeText = "TREE VIEW";
                        const string thumbnailText = "THUMBNAIL VIEW";

                        if (currentMode.Language == Web_Language_Enum.Spanish)
                        {
                            listText = "LISTADO";
                            descriptionText = "VISTA BREVE";
                            treeText = "JERARQUIA";
                        }

                        Output.WriteLine("<div class=\"ShowSelectRow\">");

                        Home_Type_Enum startHomeType = currentMode.Home_Type;

                        if ((startHomeType != Home_Type_Enum.Partners_List) && (startHomeType != Home_Type_Enum.Partners_Thumbnails))
                        {
                            if (thematicHeadings.Count > 0)
                            {
                                if (startHomeType == Home_Type_Enum.List)
                                {
                                    Output.Write("  " + Selected_Tab_Start + listText + Selected_Tab_End  + Environment.NewLine );
                                }
                                else
                                {
                                    currentMode.Home_Type = Home_Type_Enum.List;
                                    Output.Write("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + listText + Unselected_Tab_End + "</a>" + Environment.NewLine );
                                }

                                if (startHomeType == Home_Type_Enum.Descriptions)
                                {
                                    Output.Write("  " + Selected_Tab_Start + descriptionText + Selected_Tab_End  + Environment.NewLine );
                                }
                                else
                                {
                                    currentMode.Home_Type = Home_Type_Enum.Descriptions;
                                    Output.Write("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + descriptionText + Unselected_Tab_End + "</a>" + Environment.NewLine );
                                }
                            }

                            if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
                            {
                                if ((startHomeType == Home_Type_Enum.Tree_Collapsed) || (startHomeType == Home_Type_Enum.Tree_Expanded))
                                {
                                    Output.Write("  " + Selected_Tab_Start + treeText + Selected_Tab_End  + Environment.NewLine );
                                }
                                else
                                {
                                    currentMode.Home_Type = Home_Type_Enum.Tree_Collapsed;
                                    Output.Write("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + treeText + Unselected_Tab_End + "</a>" + Environment.NewLine );
                                }
                            }
                        }
                        else
                        {
                            if (startHomeType == Home_Type_Enum.Partners_List)
                            {
                                Output.Write("  " + Selected_Tab_Start + listText + Selected_Tab_End  + Environment.NewLine );
                            }
                            else
                            {
                                currentMode.Home_Type = Home_Type_Enum.Partners_List;
                                Output.Write("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + listText + Unselected_Tab_End + "</a>" + Environment.NewLine );
                            }

                            if (startHomeType == Home_Type_Enum.Partners_Thumbnails)
                            {
                                Output.Write("  " + Selected_Tab_Start + thumbnailText + Selected_Tab_End  + Environment.NewLine );
                            }
                            else
                            {
                                currentMode.Home_Type = Home_Type_Enum.Partners_Thumbnails;
                                Output.Write("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + thumbnailText + Unselected_Tab_End + "</a>" + Environment.NewLine );
                            }
                        }
                        currentMode.Home_Type = startHomeType;

                        Output.WriteLine("</div>");
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
                        return false;
                }
            }

            return true;
        }

        private string Int_To_Comma_String(int value)
        {
            if (value < 1000)
                return value.ToString();

            string value_string = value.ToString();
            if ((value >= 1000) && (value < 1000000))
            {
                return value_string.Substring(0, value_string.Length - 3) + "," + value_string.Substring(value_string.Length - 3);
            }

            return value_string.Substring(0, value_string.Length - 6) + "," + value_string.Substring(value_string.Length - 6, 3) + "," + value_string.Substring(value_string.Length - 3);
        }

        #region Main Home Page Methods

        #region Method to create the tree view

        private void Create_TreeView_From_Collections(TreeView treeView1)
        {
            // Save the current home type
            TreeNode rootNode = new TreeNode("Collection Hierarchy") {SelectAction = TreeNodeSelectAction.None};
            treeView1.Nodes.Add(rootNode);

            // Step through each node under this
            SortedList<string, TreeNode> sorted_node_list = new SortedList<string, TreeNode>();
            foreach (Item_Aggregation_Related_Aggregations childAggr in Hierarchy_Object.Children)
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
                treeView1.ExpandAll();
            }
            else
            {
                treeView1.CollapseAll();
                rootNode.Expand();
            }
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
                        html_list[thisAggr.Name.Substring(4)] = "    <td align=\"left\" width=\"330px\">" + Environment.NewLine + "      <br /><span class=\"homePageCollectionButtonLeft\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"homePageCollectionBlock\"><a href=\"" + currentMode.Redirect_URL() + "\"><b>" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</b></a><br />" + Environment.NewLine + "      " + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";
                    }
                    else
                    {
                        html_list[thisAggr.Name] = "    <td align=\"left\" width=\"330px\">" + Environment.NewLine + "      <br /><span class=\"homePageCollectionButtonLeft\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"homePageCollectionBlock\"><a href=\"" + currentMode.Redirect_URL() + "\"><b>" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</b></a></span><br />" + Environment.NewLine + "      <div class=\"homePageCollectionBlock\">" + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</div>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";

                        if (thisAggr.Code == "EPC")
                        {
                            html_list[thisAggr.Name] = "    <td align=\"left\" width=\"330px\">" + Environment.NewLine + "      <br /><span class=\"homePageCollectionButtonLeft\"><a href=\"http://www.uflib.ufl.edu/epc/\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"homePageCollectionBlock\"><a href=\"" + currentMode.Redirect_URL() + "\"><b>" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</b></a><br />" + Environment.NewLine + "      " + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";
                        }
                        if (thisAggr.Code == "UFHERB")
                        {
                            html_list[thisAggr.Name] = "    <td align=\"left\" width=\"330px\">" + Environment.NewLine + "      <br /><span class=\"homePageCollectionButtonLeft\"><a href=\"http://www.flmnh.ufl.edu/natsci/herbarium/cat/\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"homePageCollectionBlock\"><a href=\"" + currentMode.Redirect_URL() + "\"><b>" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</b></a><br />" + Environment.NewLine + "      " + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";
                        }
                        if (thisAggr.Code == "EXHIBITMATERIALS")
                        {
                            currentMode.Aggregation = "exhibits";
                            html_list[thisAggr.Name] = "    <td align=\"left\" width=\"330px\">" + Environment.NewLine + "      <br /><span class=\"homePageCollectionButtonLeft\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"homePageCollectionBlock\"><a href=\"" + currentMode.Redirect_URL() + "\"><b>" + thisAggr.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</b></a><br />" + Environment.NewLine + "      " + thisAggr.Description.Replace("&", "&amp;").Replace("\"", "&quot;") + "</span>" + Environment.NewLine + "    <br />" + Environment.NewLine + "    </td>";
                        }
                    }

                    currentMode.Skin = String.Empty;
                }

                if (html_list.Count > 0)
                {
                    // Write this theme
                    Output.WriteLine("<div class=\"thematicHeading\">" + thisTheme.ThemeName + "</div>");

                    Output.WriteLine("<table width=\"700\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">");
                    int column_spot = 0;
                    Output.WriteLine("  <tr valign=\"top\">");

                    foreach (string thisHtml in html_list.Values)
                    {
                        if (column_spot == 2)
                        {
                            Output.WriteLine("  </tr>");
                            Output.WriteLine("  <tr><td bgcolor=\"#cccccc\" colspan=\"5\"></td></tr>");
                            Output.WriteLine("  <tr valign=\"top\">");
                            column_spot = 0;
                        }

                        if (column_spot == 1)
                        {
                            Output.WriteLine("    <td width=\"20px\"></td>");
                            Output.WriteLine("    <td width=\"1px\" bgcolor=\"#cccccc\"></td>");
                            Output.WriteLine("    <td width=\"20px\"></td>");
                        }


                        Output.WriteLine(thisHtml);
                        column_spot++;
                    }

                    if (column_spot == 1)
                    {
                        Output.WriteLine("    <td width=\"20px\"></td>");
                        Output.WriteLine("    <td width=\"1px\" bgcolor=\"#cccccc\"></td>");
                        Output.WriteLine("    <td colspan=\"2\" width=\"350px\">&nbsp;</td>");
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
            foreach (Thematic_Heading thisTheme in thematicHeadings)
            {
                // Build the list of html to display
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
                        html_list[thisAggr.Name.Substring(4)] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "    </td>";
                    }
                    else
                    {
                        html_list[thisAggr.Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "    </td>";

                        if (thisAggr.Code == "EPC")
                        {
                            html_list[thisAggr.Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"http://www.uflib.ufl.edu/epc/\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                        if (thisAggr.Code == "UFHERB")
                        {
                            html_list[thisAggr.Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"http://www.flmnh.ufl.edu/natsci/herbarium/cat/\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                        if (thisAggr.Code == "EXHIBITMATERIALS")
                        {
                            currentMode.Aggregation = "exhibits";
                            html_list[thisAggr.Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Base_URL + "\"><img src=\"" + image_url + "\" alt=\"" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + translator.Get_Translation(thisAggr.Name, currentMode.Language).Replace("&", "&amp;").Replace("\"", "&quot;") + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                    }

                    currentMode.Skin = String.Empty;
                }

                if (html_list.Count > 0)
                {
                    // Write this theme
                    Output.WriteLine("<div class=\"thematicHeading\">" + translator.Get_Translation(thisTheme.ThemeName, currentMode.Language) + "</div>");

                    Output.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"5\" cellspacing=\"5\">");
                    int column_spot = 0;
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");

                    foreach (string thisHtml in html_list.Values)
                    {
                        if (column_spot == 3)
                        {
                            Output.WriteLine("  </tr>");
                            Output.WriteLine("  <tr>");
                            Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");
                            column_spot = 0;
                        }

                        Output.WriteLine(thisHtml);
                        column_spot++;
                    }

                    if (column_spot == 2)
                    {
                        Output.WriteLine("    <td colspan=\"1\" width=\"208px\">&nbsp;</td>");
                    }
                    if (column_spot == 1)
                    {
                        Output.WriteLine("    <td colspan=\"2\" width=\"416px\">&nbsp;</td>");
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

            foreach (User_Editable_Aggregation thisAggregation in currentUser.Aggregations.Where(thisAggregation => thisAggregation.OnHomePage))
            {
                currentMode.Aggregation = thisAggregation.Code.ToLower();
                string image_url = currentMode.Base_URL + "design/aggregations/" + thisAggregation.Code + "/images/buttons/coll.gif";

                if ((thisAggregation.Name.IndexOf("The ") == 0) && (thisAggregation.Name.Length > 4))
                {
                    html_list[thisAggregation.Name.Substring(4)] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('" + thisAggregation.Code + "');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";
                }
                else
                {
                    html_list[thisAggregation.Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('" + thisAggregation.Code + "');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";

                    if (thisAggregation.Code == "EPC")
                    {
                        html_list[thisAggregation.Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"http://www.uflib.ufl.edu/epc/\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('" + thisAggregation.Code + "');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";
                    }
                    if (thisAggregation.Code == "UFHERB")
                    {
                        html_list[thisAggregation.Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"http://www.flmnh.ufl.edu/natsci/herbarium/cat/\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('" + thisAggregation.Code + "');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";
                    }
                    if (thisAggregation.Code == "EXHIBITMATERIALS")
                    {
                        currentMode.Aggregation = "exhibits";
                        html_list[thisAggregation.Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisAggregation.Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return remove_aggr_from_myhome('EXHIBITMATERIALS');\">remove</a> )</span></span>" + Environment.NewLine + "    </td>";
                    }
                }
            }

            // Write this theme
            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<br />");
            Output.WriteLine("<p>Welcome to your personalized " + currentMode.SobekCM_Instance_Abbreviation + " home page.  This page displays any collections you have added, as well as any of your bookshelves you have made public.</p>");
            Output.WriteLine("<br />");
            Output.WriteLine("<div class=\"thematicHeading\">My Collections</div>");

            // If there were any saves collections, show them here
            if (html_list.Count > 0)
            {
                Output.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"5\" cellspacing=\"5\">");
                int column_spot = 0;
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");

                foreach (string thisHtml in html_list.Values)
                {
                    if (column_spot == 3)
                    {
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr>");
                        Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");
                        column_spot = 0;
                    }

                    Output.WriteLine(thisHtml);
                    column_spot++;
                }

                if (column_spot == 2)
                {
                    Output.WriteLine("    <td colspan=\"1\" width=\"208px\">&nbsp;</td>");
                }
                if (column_spot == 1)
                {
                    Output.WriteLine("    <td colspan=\"2\" width=\"416px\">&nbsp;</td>");
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
            foreach (User_Folder thisFolder in currentUser.All_Folders.Where(thisFolder => thisFolder.isPublic))
            {
                currentMode.FolderID = thisFolder.Folder_ID;
                if ((thisFolder.Folder_Name.IndexOf("The ") == 0) && (thisFolder.Folder_Name.Length > 4))
                {
                    public_folder_list[thisFolder.Folder_Name.Substring(4)] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/closed_folder_public_big.jpg\" alt=\"" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return make_folder_private('" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "');\">make private</a> )</span></span>" + Environment.NewLine + "    </td>";
                }
                else
                {
                    public_folder_list[thisFolder.Folder_Name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/closed_folder_public_big.jpg\" alt=\"" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a><br /><span class=\"MyHomeActionLink\" >( <a href=\"\" onclick=\"return make_folder_private('" + thisFolder.Folder_Name.Replace("&", "&amp;").Replace("\"", "&quot;") + "');\">make private</a> )</span></span>" + Environment.NewLine + "    </td>";
                }
            }
            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;

            // if there were any public folders
            if (public_folder_list.Count > 0)
            {
                // Write this theme
                Output.WriteLine("<div class=\"thematicHeading\">My Public Bookshelves</div>");

                Output.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"5\" cellspacing=\"5\">");
                int column_spot2 = 0;
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");

                foreach (string thisHtml in public_folder_list.Values)
                {
                    if (column_spot2 == 3)
                    {
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr>");
                        Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");
                        column_spot2 = 0;
                    }

                    Output.WriteLine(thisHtml);
                    column_spot2++;
                }

                if (column_spot2 == 2)
                {
                    Output.WriteLine("    <td colspan=\"1\" width=\"208px\">&nbsp;</td>");
                }
                if (column_spot2 == 1)
                {
                    Output.WriteLine("    <td colspan=\"2\" width=\"416px\">&nbsp;</td>");
                }
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");
                Output.WriteLine("<br />");
            }

            // Add some of the static links
            Output.WriteLine("<div class=\"thematicHeading\">My Links</div>");
            Output.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"5\" cellspacing=\"5\">");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");

            currentMode.Aggregation = String.Empty;
            currentMode.Mode = Display_Mode_Enum.My_Sobek;
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
            Output.WriteLine("    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/home_button.gif\" alt=\"Go to my" + currentMode.SobekCM_Instance_Abbreviation + " home\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">my" + currentMode.SobekCM_Instance_Abbreviation + " Home</a></span>" + Environment.NewLine + "    </td>");

            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
            Output.WriteLine("    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/big_bookshelf.gif\" alt=\"Go to my bookshelf\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">My Library</a></span>" + Environment.NewLine + "    </td>");

            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
            Output.WriteLine("    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "default/images/saved_searches_big.gif\" alt=\"Go to my saved searches\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">My Saved Searches</a></span>" + Environment.NewLine + "    </td>");

            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;

            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
            Output.WriteLine("</div>");

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
            foreach (Item_Aggregation_Related_Aggregations thisAggr in institutions)
            {
                if ( thisAggr.Active )
                {
                    string name = thisAggr.ShortName.Replace("&", "&amp;").Replace("\"", "&quot;");
                    if (name.ToUpper() == "ADDED AUTOMATICALLY")
                        name = thisAggr.Code + " ( Added Automatically )";

                    if ((thisAggr.Code.Length > 2) && (thisAggr.Code[0] == 'I'))
                    {
                        currentMode.Aggregation = thisAggr.Code.ToLower();
                        string image_url = currentMode.Base_URL + "design/aggregations/" + thisAggr.Code + "/images/buttons/coll.gif";
                        if ((name.IndexOf("The ") == 0) && (name.Length > 4))
                        {
                            html_list[name.Substring(4)] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + name + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + name + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                        else
                        {
                            html_list[name] = "    <td align=\"left\" width=\"203px\">" + Environment.NewLine + "      <span class=\"homePageCollectionButtonLeft2\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + image_url + "\" alt=\"" + name + "\" width=\"50\" height=\"50\" /></a></span>" + Environment.NewLine + "      <span class=\"SobekMainButtons\"><a href=\"" + currentMode.Redirect_URL() + "\">" + name + "</a></span>" + Environment.NewLine + "    </td>";
                        }
                    }
                }
            }
            currentMode.Aggregation = String.Empty;

            if (html_list.Count > 0)
            {
                // Write this theme
                Output.WriteLine("<div class=\"thematicHeading\">");
                Output.WriteLine("  <span class=\"groupnamecaps\">P</span>ARTNERS AND <span class=\"groupnamecaps\">C</span>ONTRIBUTING <span class=\"groupnamecaps\">I</span>NSTITUTIONS");
                Output.WriteLine("</div>");

                Output.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"5\" cellspacing=\"5\">");
                int column_spot = 0;
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");

                foreach (string thisHtml in html_list.Values)
                {
                    if (column_spot == 3)
                    {
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr>");
                        Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");
                        column_spot = 0;
                    }

                    Output.WriteLine(thisHtml);
                    column_spot++;
                }

                if (column_spot == 2)
                {
                    Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
                }
                if (column_spot == 1)
                {
                    Output.WriteLine("    <td>&nbsp;</td>");
                }
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");
            }

            Output.WriteLine("<br />");
            Output.WriteLine("</div>");
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
                Output.WriteLine("<div class=\"thematicHeading\">");
                Output.WriteLine("  <span class=\"groupnamecaps\">P</span>ARTNERS AND <span class=\"groupnamecaps\">C</span>ONTRIBUTING <span class=\"groupnamecaps\">I</span>NSTITUTIONS");
                Output.WriteLine("</div>");

                Output.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"5\" cellspacing=\"5\">");
                int column_spot = 0;
                Output.WriteLine("  <tr align=\"center\">");
                Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");

                foreach (string thisHtml in html_list.Values)
                {
                    if (column_spot == 4)
                    {
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr align=\"center\">");
                        Output.WriteLine("    <td width=\"5px\">&nbsp;</td>");
                        column_spot = 0;
                    }

                    Output.WriteLine(thisHtml);
                    column_spot++;
                }

                if (column_spot == 3)
                {
                    Output.WriteLine("    <td colspan=\"3\" width=\"208px\">&nbsp;</td>");
                }
                if (column_spot == 2)
                {
                    Output.WriteLine("    <td colspan=\"2\" width=\"208px\">&nbsp;</td>");
                }
                if (column_spot == 1)
                {
                    Output.WriteLine("    <td>&nbsp;</td>");
                }
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");
            }

            Output.WriteLine("<br />");
            Output.WriteLine("</div>");
        }

        #endregion

        #endregion

        #endregion

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        public override List<Tuple<string, string>> Body_Attributes
        {
            get
            {
                List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>();

                switch (currentMode.Mode)
                {
                    case Display_Mode_Enum.Aggregation_Browse_Info:
                        if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map)
                        {
                            returnValue.Add(new Tuple<string, string>("onload", "load();"));
                        }
                        break;

                    case Display_Mode_Enum.Search:
                        if (currentMode.Search_Type == Search_Type_Enum.Map)
                        {
                            returnValue.Add(new Tuple<string, string>("onload", "load();"));

                        }
                        break;

                    case Display_Mode_Enum.Aggregation_Browse_Map:
                        returnValue.Add(new Tuple<string, string>("onload", "load();"));
                        break;
                }

                return returnValue;
            }
        }

        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get 
            {
                switch (currentMode.Mode)
                {
                    case Display_Mode_Enum.Aggregation_Home:
                        if (Hierarchy_Object != null)
                        {
                            if (Hierarchy_Object.Code == "ALL")
                            {
                                return "{0} Home";
                            }
                            else
                            {
                                return "{0} Home - " + Hierarchy_Object.Name;
                            }
                        }
                        else
                        {
                            return "{0} Home";
                        }
 
                    case Display_Mode_Enum.Search:
                        if (Hierarchy_Object != null)
                        {
                            return "{0} Search - " + Hierarchy_Object.Name;
                        }
                        else
                        {
                            return "{0} Search";
                        }

                    case Display_Mode_Enum.Aggregation_Browse_Info:
                        if (Hierarchy_Object != null)
                        {
                            return "{0} - " + Hierarchy_Object.Name;
                        }
                        break;

                    case Display_Mode_Enum.Aggregation_Browse_By:
                    case Display_Mode_Enum.Aggregation_Browse_Map:
                    case Display_Mode_Enum.Aggregation_Private_Items:
                    case Display_Mode_Enum.Aggregation_Item_Count:
                    case Display_Mode_Enum.Aggregation_Usage_Statistics:
                    case Display_Mode_Enum.Aggregation_Admin_View:
                        return "{0} - " + Hierarchy_Object.Name;
                }

                // default
                if ( Hierarchy_Object != null )
                    return "{0} - " + Hierarchy_Object.Name;
                else
                    return "{0}";                
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
                case Display_Mode_Enum.Aggregation_Home:
                case Display_Mode_Enum.Aggregation_Browse_Info:
                case Display_Mode_Enum.Search:
                    Output.WriteLine("  <meta name=\"robots\" content=\"index, follow\" />");
                    break;

                default:
                    Output.WriteLine("  <meta name=\"robots\" content=\"noindex, nofollow\" />");
                    break;
            }

            // In the home mode, add the open search XML file to allow users to add SobekCM/UFDC as a default search in browsers
            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Home)
            {
                Output.WriteLine("  <link rel=\"search\" href=\"" + currentMode.Base_URL + "default/opensearch.xml\" type=\"application/opensearchdescription+xml\"  title=\"Add " + currentMode.SobekCM_Instance_Abbreviation + " Search\" />");
            }
        }
    }
}
