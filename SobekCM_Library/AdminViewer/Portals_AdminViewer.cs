// HTML5 10/14/2013

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
using SobekCM.Library.Settings;
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
		                        bool isDefault = portals.Default_Portal.ID == portalid;


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

			Output.WriteLine("<!-- Portals_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_portal_action\" name=\"admin_portal_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_portal_tosave\" name=\"admin_portal_tosave\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- URL Portal Edit Form -->");
			Output.WriteLine("<div class=\"sbkPoav_PopupDiv\" id=\"form_portal\" style=\"display:none;\">");
			Output.WriteLine("  <div class=\"sbkAdm_PopupTitle\"><table style=\"width:100%;\"><tr style=\"height:20px;\"><td style=\"text-align:left;\">EDIT URL PORTAL</td><td style=\"text-align:right;\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"portal_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
			Output.WriteLine("  <table class=\"sbkAdm_PopupTable\">");

            // Add the line for the url portal name
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:145px;\"><label for=\"form_portal_name\">Portal Name:</label></td>");
			Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_name\" id=\"form_portal_name\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the url portal abbreviation
			Output.WriteLine("    <tr>");
            Output.WriteLine("      <td><label for=\"form_portal_abbr\">System Abbreviation:</label></td>");
			Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_abbr\" id=\"form_portal_abbr\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the default web skin
			Output.WriteLine("    <tr>");
            Output.WriteLine("      <td><label for=\"form_portal_skin\">Default Web Skin:</label></td>");
			Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_skin\" id=\"form_portal_skin\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the default aggregation
			Output.WriteLine("    <tr>");
            Output.WriteLine("      <td><label for=\"form_portal_aggregation\">Default Aggregation:</label></td>");
			Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_aggregation\" id=\"form_portal_aggregation\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the base url segment
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td><label for=\"form_portal_url\">URL Segment:</label></td>");
			Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_url\" id=\"form_portal_url\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("    </tr>");

            // Add the line for the base purl 
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td><label for=\"form_portal_purl\">Base PURL:</label></td>");
			Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_purl\" id=\"form_portal_purl\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("    </tr>");

			// Add the buttons and close out the pop-up table
			Output.WriteLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			Output.WriteLine("      <td colspan=\"3\">");
			Output.WriteLine("        <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return portal_form_close();\">CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button title=\"Save changes to this existing portal\" class=\"sbkAdm_RoundButton\" type=\"submit\">SAVE</button>");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");
			Output.WriteLine("  </table>");
			Output.WriteLine("</div>");
			Output.WriteLine();

            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (actionMessage.Length > 0)
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\">" + actionMessage + "</div>");
			}

            Output.WriteLine("  <p>URL portals allow the same SobekCM library to have a very different look and feel and encompass different item aggregations.  Each portal is defined by the incoming URL and the URL for each incoming request is analyzed to ensure it is handled correctly.</p>");
            Output.WriteLine("  <p>For more information about URL portals, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/portals\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");

            Output.WriteLine("  <h2>New URL Portal</h2>");
            Output.WriteLine("  <p>To add a new URL portal to this system, enter the information below and press SAVE.</p>");
			Output.WriteLine("  <div class=\"sbkPoav_NewDiv\">");
			Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");

            Portal newPortal = new Portal(-1, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);

            // Add the line for the url portal name
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td style=\"width:145px;\"><label for=\"admin_portal_name\">Portal Name:</label></td>");
			Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_name\" id=\"admin_portal_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Name) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("      </tr>");

            // Add the line for the url portal abbreviation
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td><label for=\"admin_portal_abbr\">System Abbreviation:</label></td>");
			Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_abbr\" id=\"admin_portal_abbr\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Abbreviation) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("      </tr>");

            // Add the line for the default web skin
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td><label for=\"admin_portal_skin\">Default Web Skin:</label></td>");
			Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_skin\" id=\"admin_portal_skin\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Default_Web_Skin) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("      </tr>");

            // Add the line for the default aggregation
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td><label for=\"admin_portal_aggregation\">Default Aggregation:</label></td>");
			Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_aggregation\" id=\"admin_portal_aggregation\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Default_Aggregation) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("      </tr>");

            // Add the line for the base url segment
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td><label for=\"admin_portal_url\">URL Segment:</label></td>");
			Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_url\" id=\"admin_portal_url\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.URL_Segment) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("      </tr>");

            // Add the line for the base purl 
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td><label for=\"admin_portal_purl\">Base PURL:</label></td>");
			Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_purl\" id=\"admin_portal_purl\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Base_PURL) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
            Output.WriteLine("      </tr>");

			// Add the SAVE button
			Output.WriteLine("      <tr style=\"height:30px; text-align: center;\"><td colspan=\"3\"><button title=\"Save new portal\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_portal();\">SAVE</button></td></tr>");
			Output.WriteLine("    </table>");
			Output.WriteLine("  </div>");
			Output.WriteLine("  <br />");
			Output.WriteLine();

            Output.WriteLine("  <h2>Existing URL Portals</h2>");
            Output.WriteLine("  <p>The following URL portals are currently cached in this web application.</p>");

            Output.WriteLine("  <table class=\"sbkSav_Table sbkAdm_Table\">");
            Output.WriteLine("    <tr>");
			Output.WriteLine("      <th class=\"sbkPoav_TableHeader1\">ACTIONS</th>");
			Output.WriteLine("      <th class=\"sbkPoav_TableHeader2\">URL<br />SEGMENT</th>");
			Output.WriteLine("      <th class=\"sbkPoav_TableHeader3\">SYSTEM<br />ABBREVIATION</th>");
			Output.WriteLine("      <th class=\"sbkPoav_TableHeader4\">DEFAULT<br />WEB SKIN</th>");
			Output.WriteLine("      <th class=\"sbkPoav_TableHeader5\">DEFAULT<br />AGGREGATION</th>");
			Output.WriteLine("      <th class=\"sbkPoav_TableHeader6\">BASE<br />PURL</th>");
            Output.WriteLine("    </tr>");
			Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"6\"></td></tr>");

            // Write the default portal first
            Output.WriteLine("    <tr>");
            if (portals.Default_Portal.ID > 0)
            {
				Output.Write("      <td class=\"sbkAdm_ActionLink\" >( ");
                Portal thisPortal = portals.Default_Portal;
                Output.WriteLine("      <a title=\"Edit this portal\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return portal_form_popup( '" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'", "")) + "','" + thisPortal.Abbreviation + "','" + thisPortal.Default_Web_Skin + "','" + thisPortal.Default_Aggregation + "','" + thisPortal.URL_Segment + "','" + thisPortal.Base_PURL + "');\">edit</a> ) </td>");
            }
            else
            {
                Output.WriteLine("    <td>&nbsp;</td>");
            }
            Output.WriteLine("      <td id=\"sbkPoav_DefaultCell\">default</td>");
            Output.WriteLine("      <td>" + portals.Default_Portal.Abbreviation + "</td>");
            Output.WriteLine("      <td>" + portals.Default_Portal.Default_Web_Skin + "</td>");
            Output.WriteLine("      <td>" + portals.Default_Portal.Default_Aggregation + "</td>");
            Output.WriteLine("      <td>" + portals.Default_Portal.Base_PURL + "</td>");
            Output.WriteLine("    </tr>");
			Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"6\"></td></tr>");

            // Write the data for each portal
            foreach (Portal thisPortal in portals.All_Portals)
            {
                if (thisPortal != portals.Default_Portal)
                {
                    Output.WriteLine("    <tr>");
					Output.Write("      <td class=\"sbkAdm_ActionLink\" >( ");
                    Output.Write("<a title=\"Edit this portal\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return portal_form_popup( '" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'","")) + "','" + thisPortal.Abbreviation + "','" + thisPortal.Default_Web_Skin + "','" + thisPortal.Default_Aggregation + "','" + thisPortal.URL_Segment + "','" + thisPortal.Base_PURL + "');\">edit</a> | ");
                    Output.WriteLine("<a title=\"Delete this portal\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_portal('" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'", "")) + "');\">delete</a> )</td>");
                    Output.WriteLine("      <td>" + thisPortal.URL_Segment + "</td>");
                    Output.WriteLine("      <td>" + thisPortal.Abbreviation + "</td>");
                    Output.WriteLine("      <td>" + thisPortal.Default_Web_Skin + "</td>");
                    Output.WriteLine("      <td>" + thisPortal.Default_Aggregation + "</td>");
                    Output.WriteLine("      <td>" + thisPortal.Base_PURL + "</td>");
                    Output.WriteLine("    </tr>");
					Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"6\"></td></tr>");
                }
            }
            Output.WriteLine("  </table>");
            Output.WriteLine("</div>");

        }
    }
}
