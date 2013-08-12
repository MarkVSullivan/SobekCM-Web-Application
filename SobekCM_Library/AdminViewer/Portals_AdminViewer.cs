#region Using directives

using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit the URL portals active in this library </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to show the URL portals active in this digital library</li>
    /// </ul></remarks>
    public class Portals_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly Portal_List portals;

        /// <summary> Constructor for a new instance of the Portals_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="URL_Portals"> List of all web portals into this system </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Portals_AdminViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Portal_List URL_Portals, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Portals_AdminViewer.Constructor", String.Empty);

            portals = URL_Portals;

            // Save the mode 
            currentMode = Current_Mode;

            // Set action message to nothing to start
            actionMessage = String.Empty;

            // If the user cannot edit this, go back
            if (!user.Is_System_Admin)
            {
                Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
                return;
            }

            // Handle any post backs
            if (Current_Mode.isPostBack)
            {
                try
                {
                    // Pull the standard values from the form
                    NameValueCollection form = HttpContext.Current.Request.Form;
                    string save_value = form["admin_portal_tosave"];
                    string action_value = form["admin_portal_action"];

                    // Switch, depending on the request
                    if (action_value != null)
                    {
                        switch (action_value.Trim().ToLower())
                        {
                            case "edit":
                                // Get the values from the form for this new portal
                                string edit_name = form["form_portal_name"].Trim();
                                string edit_abbr = form["form_portal_abbr"].Trim();
                                string edit_skin = form["form_portal_skin"].Trim();
                                string edit_aggr = form["form_portal_aggregation"].Trim();
                                string edit_url = form["form_portal_url"].Trim();
                                string edit_purl = form["form_portal_purl"].Trim();
                                int portalid = Convert.ToInt32(save_value);

                                // Look for this to see if this was the pre-existing default
                                bool isDefault = false;
                                if ( portals.Default_Portal.ID == portalid )
                                {
                                    isDefault = true;
                                }
                                

                                // Don't edit if the URL segment is empty and this is NOT default
                                if ((!isDefault) && (edit_url.Trim().Length == 0))
                                {
                                    actionMessage = "ERROR: Non default portals MUST have a url segment associated.";
                                }
                                else
                                {
                                    // Now, save this portal information
                                    int edit_id = SobekCM_Database.Edit_URL_Portal(portalid, edit_url, true, isDefault, edit_abbr, edit_name, edit_aggr, edit_skin, edit_purl, Tracer);
                                    if (edit_id > 0)
                                        actionMessage = "Edited existing URL portal '" + edit_name + "'";
                                    else
                                        actionMessage = "Error editing URL portal.";
                                }
                                break;

                            case "delete":
                                actionMessage = SobekCM_Database.Delete_URL_Portal(Convert.ToInt32(save_value), Tracer) ? "URL portal deleted" : "Error deleting the URL portal";
                                break;

                            case "new":
                                // Get the values from the form for this new portal
                                string new_name = form["admin_portal_name"];
                                string new_abbr = form["admin_portal_abbr"];
                                string new_skin = form["admin_portal_skin"];
                                string new_aggr = form["admin_portal_aggregation"];
                                string new_url = form["admin_portal_url"];
                                string new_purl = form["admin_portal_purl"];

                                // Save this to the database
                                int new_id = SobekCM_Database.Edit_URL_Portal(-1, new_url, true, false, new_abbr, new_name, new_aggr, new_skin, new_purl, Tracer);
                                if (new_id > 0)
                                    actionMessage = "Saved new URL portal '" + new_name + "'";
                                else
                                    actionMessage = "Error saving URL portal.";
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    actionMessage = "Exception caught while handling request";
                }

                // Reload all the URL portals
                SobekCM_Database.Populate_URL_Portals(portals, Tracer);
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'URL Portals' </value>
        public override string Web_Title
        {
            get { return "URL Portals"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Portals_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            // Set the help for these subelements
            const string PORTAL_NAME_HELP = "Name for the system when accessed through this url portal.  This additionally identifies this url portal to administrators.\n\nFor example, Digital Library of the Caribbean, or University of Florida Digital Collections.";
            const string ABBREVIATION_HELP = "Abbreviation for the system, when accessed through this url portal.\n\nFor example, 'dLOC', or UFDC.";
            const string WEB_SKIN_HELP = "Default web skin under which this url portal should be displayed.  If none is probided, it will default to the system web skin.";
            const string AGGREGATION_HELP = "Default aggregation which should be displayed under this url portal.  If none is provided, this url portal will display the main system home page.";
            const string URL_SEGMENT_HELP = "URL segment used for matching purposes to determine which url portal a user is accessing this system from.\n\nA blank URL portal will make this the default portal.";
            const string BASE_PURL_HELP = "Base permanent link URL to be used when constructing permanent URLs for items which do not have itt explicitly entered.\n\nA blank value here will result in the current URL being used as the base for the purl.";

            Tracer.Add_Trace("Portals_AdminViewer.Add_HTML_In_Main_Form", "Adds the portal information to the main form");

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_portal_action\" name=\"admin_portal_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_portal_tosave\" name=\"admin_portal_tosave\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- URL Portal Edit Form -->");
            Output.WriteLine("<div class=\"admin_portal_popup_div\" id=\"form_portal\" style=\"display:none;\">");
            Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT URL PORTAL</td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"portal_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"popup_table\">");

            // Add the line for the url portal name
            Output.WriteLine("    <tr height=\"30px\">");
            Output.WriteLine("          <td width=\"145px\"><label for=\"form_portal_name\">Portal Name:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"form_portal_name\" id=\"form_portal_name\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('form_portal_name', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('form_portal_name', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the url portal abbreviation
            Output.WriteLine("    <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"form_portal_abbr\">System Abbreviation:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"form_portal_abbr\" id=\"form_portal_abbr\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('form_portal_abbr', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('form_portal_abbr', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the default web skin
            Output.WriteLine("    <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"form_portal_skin\">Default Web Skin:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"form_portal_skin\" id=\"form_portal_skin\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('form_portal_skin', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('form_portal_skin', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the default aggregation
            Output.WriteLine("    <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"form_portal_aggregation\">Default Aggregation:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"form_portal_aggregation\" id=\"form_portal_aggregation\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('form_portal_aggregation', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('form_portal_aggregation', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the base url segment
            Output.WriteLine("    <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"form_portal_url\">URL Segment:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"form_portal_url\" id=\"form_portal_url\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('form_portal_url', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('form_portal_url', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the base purl 
            Output.WriteLine("    <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"form_portal_purl\">Base PURL:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"form_portal_purl\" id=\"form_portal_purl\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('form_portal_purl', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('form_portal_purl', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <center><a href=\"\" onclick=\"return portal_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center>");
            Output.WriteLine("</div>");

            Output.WriteLine("<!-- Portals_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");

            if ( !String.IsNullOrEmpty(actionMessage))
                Output.WriteLine("  <strong>" + actionMessage + "</strong>");

            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    URL portals allow the same SobekCM library to have a very different look and feel and encompass different item aggregations.  Each portal is defined by the incoming URL and the URL for each incoming request is analyzed to ensure it is handled correctly.<br /><br />");
            Output.WriteLine("    For more information about URL portals, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/portals\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.");
            Output.WriteLine("  </blockquote>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">New URL Portal</span>");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("  To add a new URL portal to this system, enter the information below and press SAVE.<br /><br />");
            Output.WriteLine("    <div class=\"admin_portal_new_div\">");
            Output.WriteLine("      <table class=\"popup_table\">");

            Portal newPortal = new Portal(-1, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);



            // Add the line for the url portal name
            Output.WriteLine("        <tr height=\"30px\">");
            Output.WriteLine("          <td width=\"145px\"><label for=\"admin_portal_name\">Portal Name:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"admin_portal_name\" id=\"admin_portal_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Name) + "\"  onfocus=\"javascript:textbox_enter('admin_portal_name', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('admin_portal_name', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("        </tr>");

            // Add the line for the url portal abbreviation
            Output.WriteLine("        <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"admin_portal_abbr\">System Abbreviation:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"admin_portal_abbr\" id=\"admin_portal_abbr\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Abbreviation) + "\"  onfocus=\"javascript:textbox_enter('admin_portal_abbr', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('admin_portal_abbr', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("        </tr>");

            // Add the line for the default web skin
            Output.WriteLine("        <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"admin_portal_skin\">Default Web Skin:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"admin_portal_skin\" id=\"admin_portal_skin\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Default_Web_Skin) + "\"  onfocus=\"javascript:textbox_enter('admin_portal_skin', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('admin_portal_skin', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("        </tr>");

            // Add the line for the default aggregation
            Output.WriteLine("        <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"admin_portal_aggregation\">Default Aggregation:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"admin_portal_aggregation\" id=\"admin_portal_aggregation\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Default_Aggregation) + "\"  onfocus=\"javascript:textbox_enter('admin_portal_aggregation', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('admin_portal_aggregation', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("        </tr>");

            // Add the line for the base url segment
            Output.WriteLine("        <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"admin_portal_url\">URL Segment:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"admin_portal_url\" id=\"admin_portal_url\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.URL_Segment) + "\"  onfocus=\"javascript:textbox_enter('admin_portal_url', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('admin_portal_url', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("        </tr>");

            // Add the line for the base purl 
            Output.WriteLine("        <tr height=\"30px\">");
            Output.WriteLine("          <td><label for=\"admin_portal_purl\">Base PURL:</label></td>");
            Output.WriteLine("          <td><input class=\"admin_portal_input\" name=\"admin_portal_purl\" id=\"admin_portal_purl\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Base_PURL) + "\"  onfocus=\"javascript:textbox_enter('admin_portal_purl', 'admin_portal_input_focused')\" onblur=\"javascript:textbox_leave('admin_portal_purl', 'admin_portal_input')\" /></td>");
            Output.WriteLine("          <td><img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" /></td>");
            Output.WriteLine("        </tr>");

            Output.WriteLine("        </table>");

            Output.WriteLine("        <center><input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\" onclick=\"return save_new_portal();\"/></center>");

            Output.WriteLine("      </div>");
            Output.WriteLine("    </blockquote>");


            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing URL Portals</span>");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    The following URL portals are currently cached in this web application.<br /><br />");
            Output.WriteLine("  </blockquote>");

            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"85px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
            Output.WriteLine("    <th width=\"130px\" align=\"left\"><span style=\"color: White\">URL<br />SEGMENT</span></th>");
            Output.WriteLine("    <th width=\"110px\" align=\"left\"><span style=\"color: White\">SYSTEM<br />ABBREVIATION</span></th>");
            Output.WriteLine("    <th width=\"85px\" align=\"left\"><span style=\"color: White\">DEFAULT<br />WEB SKIN</span></th>");
            Output.WriteLine("    <th width=\"105px\" align=\"left\"><span style=\"color: White\">DEFAULT<br />AGGREGATION</span></th>");
            Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\">BASE<br />PURL</span></th>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"6\"></td></tr>");

            // Write the default portal first
            Output.WriteLine("  <tr align=\"left\" >");
            if (portals.Default_Portal.ID > 0)
            {
                Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                Portal thisPortal = portals.Default_Portal;
                Output.WriteLine("      <a title=\"Edit this portal\" id=\"VIEW_0\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return portal_form_popup('VIEW_0','" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'", "")) + "','" + thisPortal.Abbreviation + "','" + thisPortal.Default_Web_Skin + "','" + thisPortal.Default_Aggregation + "','" + thisPortal.URL_Segment + "','" + thisPortal.Base_PURL + "');\">edit</a> ) </td>");
            }
            else
            {
                Output.WriteLine("    <td>&nbsp;</td>");
            }
            Output.WriteLine("    <td><i><b>default</b></i></span></td>");
            Output.WriteLine("    <td>" + portals.Default_Portal.Abbreviation + "</span></td>");
            Output.WriteLine("    <td>" + portals.Default_Portal.Default_Web_Skin + "</span></td>");
            Output.WriteLine("    <td>" + portals.Default_Portal.Default_Aggregation + "</span></td>");
            Output.WriteLine("    <td>" + portals.Default_Portal.Base_PURL + "</span></td>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"6\"></td></tr>");

            // Write the data for each portal
            foreach (Portal thisPortal in portals.All_Portals)
            {
                if (thisPortal != portals.Default_Portal)
                {
                    Output.WriteLine("  <tr align=\"left\" >");
                    Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                    Output.Write("<a title=\"Edit this portal\" id=\"VIEW_" + thisPortal.ID + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return portal_form_popup('VIEW_" + thisPortal.ID + "','" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'","")) + "','" + thisPortal.Abbreviation + "','" + thisPortal.Default_Web_Skin + "','" + thisPortal.Default_Aggregation + "','" + thisPortal.URL_Segment + "','" + thisPortal.Base_PURL + "');\">edit</a> | ");
                    Output.WriteLine("<a title=\"Delete this portal\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_portal('" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'", "")) + "');\">delete</a> )</td>");
                    Output.WriteLine("    <td>" + thisPortal.URL_Segment + "</span></td>");
                    Output.WriteLine("    <td>" + thisPortal.Abbreviation + "</span></td>");
                    Output.WriteLine("    <td>" + thisPortal.Default_Web_Skin + "</span></td>");
                    Output.WriteLine("    <td>" + thisPortal.Default_Aggregation + "</span></td>");
                    Output.WriteLine("    <td>" + thisPortal.Base_PURL + "</span></td>");
                    Output.WriteLine("   </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"6\"></td></tr>");
                }
            }
            Output.WriteLine("  </table>");
            Output.WriteLine("  <br /><br />");
            Output.WriteLine("</div>");

        }
    }
}
