// HTML5 10/14/2013

#region Using directives

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit the URL UI_ApplicationCache_Gateway.URL_Portals active in this library </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to show the URL UI_ApplicationCache_Gateway.URL_Portals active in this digital library</li>
    /// </ul></remarks>
    public class Portals_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;

        private string entered_portal_name;
        private string entered_sys_abbrev;
        private string entered_web_skin;
        private string entered_aggregation;
        private string entered_url_segment;
        private string entered_base_purl;


        /// <summary> Constructor for a new instance of the Portals_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Portals_AdminViewer(RequestCache RequestSpecificValues) :  base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Portals_AdminViewer.Constructor", String.Empty);

            // Set action message to nothing to start and some defaults
            actionMessage = String.Empty;
            entered_portal_name = String.Empty;
            entered_sys_abbrev = String.Empty;
            entered_web_skin = String.Empty;
            entered_aggregation = String.Empty;
            entered_url_segment = String.Empty;
            entered_base_purl = String.Empty;

            // If the RequestSpecificValues.Current_User cannot edit this, go back
            if (( RequestSpecificValues.Current_User == null ) || ((!RequestSpecificValues.Current_User.Is_System_Admin) && ( !RequestSpecificValues.Current_User.Is_Portal_Admin )))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Handle any post backs
            if ((RequestSpecificValues.Current_Mode.isPostBack) && ( RequestSpecificValues.Current_User.Is_System_Admin ))
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
                                bool isDefault = UI_ApplicationCache_Gateway.URL_Portals.Default_Portal.ID == portalid;


                                // Don't edit if the URL segment is empty and this is NOT default
                                if ((!isDefault) && (edit_url.Trim().Length == 0))
                                {
                                    actionMessage = "ERROR: Non default portals MUST have a url segment associated.";
                                }
                                else if (edit_name.Length == 0)
                                {
                                    actionMessage = "ERROR: Portal name is a REQUIRED field.";
                                }
                                else if (edit_abbr.Length == 0)
                                {
                                    actionMessage = "ERROR: System abbreviation is a REQUIRED field";
                                }
                                else
                                {
                                    // Look for matching portal or URL segment names
                                    bool portal_name_match = false;
                                    bool url_segment_match = false;
                                    foreach (Portal thisPortal in UI_ApplicationCache_Gateway.URL_Portals.All_Portals)
                                    {
                                        if (thisPortal.ID != portalid)
                                        {
                                            if (String.Compare(thisPortal.Name, entered_portal_name, true) == 0)
                                            {
                                                portal_name_match = true;
                                                break;
                                            }

                                            if (String.Compare(thisPortal.URL_Segment, entered_url_segment, true) == 0)
                                            {
                                                url_segment_match = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (portal_name_match)
                                    {
                                        actionMessage = "ERROR: Portal name must be unique, and that name is already in use";
                                    }
                                    else if (url_segment_match)
                                    {
                                        actionMessage = "ERROR: URL segment must be unique, and that URL segment already exists";
                                    }
                                    else
                                    {
                                        bool result = edit_abbr.All(C => Char.IsLetterOrDigit(C) || C == '_');
                                        if (!result)
                                        {
                                            actionMessage = "ERROR: System abbreviation must include only letters and numbers";
                                            edit_abbr = edit_abbr.Replace("\"", "");
                                        }
                                        else
                                        {
                                            // Now, save this portal information
                                            int edit_id = SobekCM_Database.Edit_URL_Portal(portalid, edit_url, true, isDefault, edit_abbr, edit_name, edit_aggr, edit_skin, edit_purl, RequestSpecificValues.Tracer);
                                            if (edit_id > 0)
                                                actionMessage = "Edited existing URL portal '" + edit_name + "'";
                                            else
                                                actionMessage = "Error editing URL portal.";
                                        }
                                    }
                                }
                                break;

                            case "delete":
                                actionMessage = SobekCM_Database.Delete_URL_Portal(Convert.ToInt32(save_value), RequestSpecificValues.Tracer) ? "URL portal deleted" : "Error deleting the URL portal";
                                break;

                            case "new":
                                // Get the values from the form for this new portal
                                entered_portal_name = form["admin_portal_name"];
                                entered_sys_abbrev = form["admin_portal_abbr"];
                                entered_web_skin = form["admin_portal_skin"].ToLower();
                                entered_aggregation = form["admin_portal_aggregation"].ToLower();
                                entered_url_segment = form["admin_portal_url"];
                                entered_base_purl = form["admin_portal_purl"];



                                if (entered_portal_name.Length == 0)
                                {
                                    actionMessage = "ERROR: Portal name is a REQUIRED field.";
                                }
                                else if (entered_sys_abbrev.Length == 0)
                                {
                                    actionMessage = "ERROR: System abbreviation is a REQUIRED field.";
                                }
                                else if (entered_web_skin.Length == 0)
                                {
                                    actionMessage = "ERROR: Default web skin is a REQUIRED field.";
                                }
                                else if (entered_url_segment.Length == 0)
                                {
                                    actionMessage = "ERROR: URL segment is a REQUIRED field.";
                                }
                                else
                                {
                                    // Look for matching portal or URL segment names
                                    bool portal_name_match = false;
                                    bool url_segment_match = false;
                                    foreach (Portal thisPortal in UI_ApplicationCache_Gateway.URL_Portals.All_Portals)
                                    {
                                        if (String.Compare(thisPortal.Name, entered_portal_name, true) == 0)
                                        {
                                            portal_name_match = true;
                                            break;
                                        }

                                        if (String.Compare(thisPortal.URL_Segment, entered_url_segment, true) == 0)
                                        {
                                            url_segment_match = true;
                                            break;
                                        }
                                    }

                                    if (portal_name_match)
                                    {
                                        actionMessage = "ERROR: Portal name must be unique, and that name is already in use";
                                    }
                                    else if (url_segment_match)
                                    {
                                        actionMessage = "ERROR: URL segment must be unique, and that URL segment already exists";
                                    }
                                    else
                                    {
                                        bool result = entered_sys_abbrev.All(C => Char.IsLetterOrDigit(C) || C == '_');
                                        if (!result)
                                        {
                                            actionMessage = "ERROR: System abbreviation must include only letters and numbers";
                                            entered_sys_abbrev = entered_sys_abbrev.Replace("\"", "");
                                        }
                                        else
                                        {
                                            // Save this to the database
                                            int new_id = SobekCM_Database.Edit_URL_Portal(-1, entered_url_segment, true, false, entered_sys_abbrev, entered_portal_name, entered_aggregation, entered_web_skin, entered_base_purl, RequestSpecificValues.Tracer);
                                            if (new_id > 0)
                                            {
                                                actionMessage = "Saved new URL portal '" + entered_portal_name + "'";

                                                entered_portal_name = String.Empty;
                                                entered_sys_abbrev = String.Empty;
                                                entered_web_skin = String.Empty;
                                                entered_aggregation = String.Empty;
                                                entered_url_segment = String.Empty;
                                                entered_base_purl = String.Empty;
                                            }
                                            else
                                                actionMessage = "Error saving URL portal.";
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    actionMessage = "Exception caught while handling request";
                }

                // Reload all the URL UI_ApplicationCache_Gateway.URL_Portals
                Engine_Database.Populate_URL_Portals(UI_ApplicationCache_Gateway.URL_Portals, RequestSpecificValues.Tracer);
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
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            // Set the help for these subelements
            const string PORTAL_NAME_HELP = "Name for the system when accessed through this url portal.  This additionally identifies this url portal to administrators.\n\nFor example, Digital Library of the Caribbean, or University of Florida Digital Collections.";
            const string ABBREVIATION_HELP = "Abbreviation for the system, when accessed through this url portal.\n\nFor example, 'dLOC', or UFDC.";
            const string WEB_SKIN_HELP = "Default web skin under which this url portal should be displayed.  If none is probided, it will default to the system web skin.";
            const string AGGREGATION_HELP = "Default aggregation which should be displayed under this url portal.  If none is provided, this url portal will display the main system home page.";
            const string URL_SEGMENT_HELP = "URL segment used for matching purposes to determine which url portal a RequestSpecificValues.Current_User is accessing this system from.\n\nA blank URL portal will make this the default portal.";
            const string BASE_PURL_HELP = "Base permanent link URL to be used when constructing permanent URLs for items which do not have itt explicitly entered.\n\nA blank value here will result in the current URL being used as the base for the purl.";

            Tracer.Add_Trace("Portals_AdminViewer.Write_ItemNavForm_Closing", "Adds the portal information to the main form");

			Output.WriteLine("<!-- Portals_AdminViewer.Write_ItemNavForm_Closing -->");
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");
			Output.WriteLine();

            // Add the hidden field
            Output.WriteLine("<!-- Hidden fields are used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_portal_action\" name=\"admin_portal_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_portal_tosave\" name=\"admin_portal_tosave\" value=\"\" />");
            Output.WriteLine();

			// Only system admins can edit the URL UI_ApplicationCache_Gateway.URL_Portals
	        if (RequestSpecificValues.Current_User.Is_System_Admin)
	        {
		        Output.WriteLine("<!-- URL Portal Edit Form -->");
		        Output.WriteLine("<div class=\"sbkPoav_PopupDiv\" id=\"form_portal\" style=\"display:none;\">");
		        Output.WriteLine("  <div class=\"sbkAdm_PopupTitle\"><table style=\"width:100%;\"><tr style=\"height:20px;\"><td style=\"text-align:left;\">EDIT URL PORTAL</td><td style=\"text-align:right;\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"portal_form_close()\">X</a> &nbsp; </td></tr></table></div>");
		        Output.WriteLine("  <br />");
		        Output.WriteLine("  <table class=\"sbkAdm_PopupTable\">");

		        // Add the line for the url portal name
		        Output.WriteLine("    <tr>");
		        Output.WriteLine("      <td style=\"width:145px;\"><label for=\"form_portal_name\">Portal Name:</label></td>");
		        Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_name\" id=\"form_portal_name\" type=\"text\" value=\"\" /></td>");
		        Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("    </tr>");

		        // Add the line for the url portal abbreviation
		        Output.WriteLine("    <tr>");
		        Output.WriteLine("      <td><label for=\"form_portal_abbr\">System Abbreviation:</label></td>");
		        Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_abbr\" id=\"form_portal_abbr\" type=\"text\" value=\"\" /></td>");
		        Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("    </tr>");

		        // Add the line for the default web skin
		        Output.WriteLine("    <tr>");
		        Output.WriteLine("      <td><label for=\"form_portal_skin\">Default Web Skin:</label></td>");
		        Output.WriteLine("      <td>");

				//Output.WriteLine("        <input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_skin\" id=\"form_portal_skin\" type=\"text\" value=\"\" /></td>");

				Output.WriteLine("        <select class=\"sbkPoav_select\" name=\"form_portal_skin\" id=\"form_portal_skin\">");
				foreach (string thisSkin in UI_ApplicationCache_Gateway.Web_Skin_Collection.Ordered_Skin_Codes)
				{
					Output.WriteLine("          <option value=\"" + thisSkin.ToLower() + "\">" + thisSkin.ToLower() + "</option>");
				}
				Output.WriteLine("        </select>");

		        Output.WriteLine("      </td>");
		        Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("    </tr>");

		        // Add the line for the default aggregation
		        Output.WriteLine("    <tr>");
		        Output.WriteLine("      <td><label for=\"form_portal_aggregation\">Default Aggregation:</label></td>");
                Output.WriteLine("        <td>");
                Output.WriteLine("            <select class=\"sbkPoav_select sbkAdmin_Focusable\" name=\"form_portal_aggregation\" id=\"form_portal_aggregation\">");
                Output.WriteLine("              <option value=\"\"></option>");
                foreach (Core.Aggregations.Item_Aggregation_Related_Aggregations thisAggr in UI_ApplicationCache_Gateway.Aggregations.All_Aggregations)
                {
                    if (thisAggr.Code != "ALL")
                    {
                        string display = thisAggr.Code.ToLower() + " - " + thisAggr.ShortName;
                        if (display.Length > 40)
                            display = display.Substring(0, 40) + "...";

                        Output.WriteLine("              <option value=\"" + thisAggr.Code.ToUpper() + "\">" + HttpUtility.HtmlEncode(display) + "</option>");
                    }
                }
                Output.WriteLine("            </select>");
                Output.WriteLine("        </td>");
		        Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("    </tr>");

		        // Add the line for the base url segment
		        Output.WriteLine("    <tr>");
		        Output.WriteLine("      <td><label for=\"form_portal_url\">URL Segment:</label></td>");
		        Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_url\" id=\"form_portal_url\" type=\"text\" value=\"\" /></td>");
		        Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("    </tr>");

		        // Add the line for the base purl 
		        Output.WriteLine("    <tr>");
		        Output.WriteLine("      <td><label for=\"form_portal_purl\">Base PURL:</label></td>");
		        Output.WriteLine("      <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"form_portal_purl\" id=\"form_portal_purl\" type=\"text\" value=\"\" /></td>");
		        Output.WriteLine("      <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("    </tr>");

		        // Add the buttons and close out the pop-up table
		        Output.WriteLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
		        Output.WriteLine("      <td colspan=\"3\">");
				Output.WriteLine("        <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return portal_form_close();\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
				Output.WriteLine("        <button title=\"Save changes to this existing portal\" class=\"sbkAdm_RoundButton\" type=\"submit\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
		        Output.WriteLine("      </td>");
		        Output.WriteLine("    </tr>");
		        Output.WriteLine("  </table>");
		        Output.WriteLine("</div>");
		        Output.WriteLine();
	        }

	        Output.WriteLine("<script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (actionMessage.Length > 0)
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\">" + actionMessage + "</div>");
			}

            Output.WriteLine("  <p>URL UI_ApplicationCache_Gateway.URL_Portals allow the same SobekCM library to have a very different look and feel and encompass different item aggregations.  Each portal is defined by the incoming URL and the URL for each incoming request is analyzed to ensure it is handled correctly.</p>");
            Output.WriteLine("  <p>For more information about URL UI_ApplicationCache_Gateway.URL_Portals, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/UI_ApplicationCache_Gateway.URL_Portals\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");

			// Add portal admin message
			int columns = 6;
	        if (!RequestSpecificValues.Current_User.Is_System_Admin)
	        {
		        Output.WriteLine("<p>Portal Admins have rights to see these settings. System Admins can change these settings.</p>");
		        columns = 5;
	        }
	        else
	        {
		        Output.WriteLine("  <h2>New URL Portal</h2>");
		        Output.WriteLine("  <p>To add a new URL portal to this system, enter the information below and press SAVE.</p>");
		        Output.WriteLine("  <div class=\"sbkPoav_NewDiv\">");
		        Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");

                Portal newPortal = new Portal(-1, entered_portal_name, entered_sys_abbrev, entered_aggregation, entered_web_skin, entered_url_segment, entered_base_purl);


		        // Add the line for the url portal name
		        Output.WriteLine("      <tr>");
		        Output.WriteLine("        <td style=\"width:145px;\"><label for=\"admin_portal_name\">Portal Name:</label></td>");
		        Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_name\" id=\"admin_portal_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Name) + "\" /></td>");
		        Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + PORTAL_NAME_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("      </tr>");

		        // Add the line for the url portal abbreviation
		        Output.WriteLine("      <tr>");
		        Output.WriteLine("        <td><label for=\"admin_portal_abbr\">System Abbreviation:</label></td>");
		        Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_abbr\" id=\"admin_portal_abbr\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Abbreviation) + "\" /></td>");
		        Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + ABBREVIATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("      </tr>");

		        // Add the line for the default web skin
		        Output.WriteLine("      <tr>");
		        Output.WriteLine("        <td><label for=\"admin_portal_skin\">Default Web Skin:</label></td>");

				Output.WriteLine("        <td>");

				//Output.WriteLine("        <input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_skin\" id=\"admin_portal_skin\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Default_Web_Skin) + "\" />");

				Output.WriteLine("          <select class=\"sbkPoav_select\" name=\"admin_portal_skin\" id=\"admin_portal_skin\">");

				if ( newPortal.Default_Web_Skin.Trim().Length == 0 )
					Output.WriteLine("            <option value=\"\" selected=\"selected\"></option>");
				else
					Output.WriteLine("            <option value=\"\"></option>");

				foreach (string thisSkin in UI_ApplicationCache_Gateway.Web_Skin_Collection.Ordered_Skin_Codes)
				{
					if ( String.Compare(thisSkin, newPortal.Default_Web_Skin, StringComparison.OrdinalIgnoreCase) == 0 )
						Output.WriteLine("            <option value=\"" + thisSkin.ToLower() + "\" selected=\"selected\">" + thisSkin.ToLower() + "</option>");
					else
						Output.WriteLine("            <option value=\"" + thisSkin.ToLower() + "\">" + thisSkin.ToLower() + "</option>");
				}
				Output.WriteLine("          </select>");

				Output.WriteLine("        </td>");
		        Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + WEB_SKIN_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("      </tr>");

		        // Add the line for the default aggregation
		        Output.WriteLine("      <tr>");
		        Output.WriteLine("        <td><label for=\"admin_portal_aggregation\">Default Aggregation:</label></td>");
	            Output.WriteLine("        <td>");
                Output.WriteLine("            <select class=\"sbkPoav_select sbkAdmin_Focusable\" name=\"admin_portal_aggregation\" id=\"admin_portal_aggregation\">");
	            if (entered_aggregation.Length == 0)
	            {
	                Output.WriteLine("              <option value=\"\" selected=\"selected\"></option>");
	            }
	            else
	            {
                    Output.WriteLine("              <option value=\"\"></option>");
	            }

	            foreach (Core.Aggregations.Item_Aggregation_Related_Aggregations thisAggr in UI_ApplicationCache_Gateway.Aggregations.All_Aggregations)
                {
                    if (thisAggr.Code != "ALL")
                    {
                        string display = thisAggr.Code.ToLower() + " - " + thisAggr.ShortName;
                        if (display.Length > 40)
                            display = display.Substring(0, 40) + "...";

                        if (String.Compare(thisAggr.Code, newPortal.Default_Aggregation, StringComparison.OrdinalIgnoreCase) == 0)
                            Output.WriteLine("              <option value=\"" + thisAggr.Code + "\" selected=\"selected\">" + HttpUtility.HtmlEncode(display) + "</option>");
                        else
                            Output.WriteLine("              <option value=\"" + thisAggr.Code + "\">" + HttpUtility.HtmlEncode(display) + "</option>");
                    }
                }
                Output.WriteLine("            </select>");
                Output.WriteLine("        </td>");
		        Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + AGGREGATION_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("      </tr>");

		        // Add the line for the base url segment
		        Output.WriteLine("      <tr>");
		        Output.WriteLine("        <td><label for=\"admin_portal_url\">URL Segment:</label></td>");
		        Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_url\" id=\"admin_portal_url\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.URL_Segment) + "\" /></td>");
		        Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + URL_SEGMENT_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("      </tr>");

		        // Add the line for the base purl 
		        Output.WriteLine("      <tr>");
		        Output.WriteLine("        <td><label for=\"admin_portal_purl\">Base PURL:</label></td>");
		        Output.WriteLine("        <td><input class=\"sbkPoav_input sbkAdmin_Focusable\" name=\"admin_portal_purl\" id=\"admin_portal_purl\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newPortal.Base_PURL) + "\" /></td>");
		        Output.WriteLine("        <td><img class=\"sbkPoav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" title=\"" + BASE_PURL_HELP.Replace("'", "").Replace("\\", "\\\\").Replace("\n", " ") + "\" /></td>");
		        Output.WriteLine("      </tr>");

		        // Add the SAVE button
				Output.WriteLine("      <tr style=\"height:30px; text-align: center;\"><td colspan=\"3\"><button title=\"Save new portal\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_portal();\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td></tr>");
		        Output.WriteLine("    </table>");
		        Output.WriteLine("  </div>");
		        Output.WriteLine("  <br />");
		        Output.WriteLine();
	        }

	        Output.WriteLine("  <h2>Existing URL Portals</h2>");
            Output.WriteLine("  <p>The following URL portals are active:</p>");

            Output.WriteLine("  <table class=\"sbkPoav_Table sbkAdm_Table\">");
            Output.WriteLine("    <tr>");
			if ( RequestSpecificValues.Current_User.Is_System_Admin )
				Output.WriteLine("      <th class=\"sbkPoav_TableHeader1\">ACTIONS</th>");
            Output.WriteLine("      <th class=\"sbkPoav_TableHeader2\">PORTAL<br />NAME</th>");
            Output.WriteLine("      <th class=\"sbkPoav_TableHeader3\">SYSTEM<br />ABBREVIATION</th>");
            Output.WriteLine("      <th class=\"sbkPoav_TableHeader4\">DEFAULT<br />WEB SKIN</th>");
            Output.WriteLine("      <th class=\"sbkPoav_TableHeader5\">DEFAULT<br />AGGREGATION</th>");
			Output.WriteLine("      <th class=\"sbkPoav_TableHeader6\">URL<br />SEGMENT</th>");
            Output.WriteLine("    </tr>");

            // Write the default portal first
            Output.WriteLine("    <tr>");
	        if (RequestSpecificValues.Current_User.Is_System_Admin)
	        {
		        if (UI_ApplicationCache_Gateway.URL_Portals.Default_Portal.ID > 0)
		        {
			        Output.Write("      <td class=\"sbkAdm_ActionLink\" >( ");
			        Portal thisPortal = UI_ApplicationCache_Gateway.URL_Portals.Default_Portal;
			        Output.WriteLine("      <a title=\"Edit this portal\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return portal_form_popup( '" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'", "")) + "','" + thisPortal.Abbreviation + "','" + thisPortal.Default_Web_Skin + "','" + thisPortal.Default_Aggregation.ToUpper() + "','" + thisPortal.URL_Segment + "','" + thisPortal.Base_PURL + "');\">edit</a> ) </td>");
		        }
		        else
		        {
			        Output.WriteLine("    <td>&nbsp;</td>");
		        }
	        }
            Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(UI_ApplicationCache_Gateway.URL_Portals.Default_Portal.Name) + "</td>");
            Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(UI_ApplicationCache_Gateway.URL_Portals.Default_Portal.Abbreviation) + "</td>");
            Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(UI_ApplicationCache_Gateway.URL_Portals.Default_Portal.Default_Web_Skin) + "</td>");
            Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(UI_ApplicationCache_Gateway.URL_Portals.Default_Portal.Default_Aggregation) + "</td>");
	        Output.WriteLine("      <td id=\"sbkPoav_DefaultCell\">default</td>");
            
            Output.WriteLine("    </tr>");
			Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"" + columns + "\"></td></tr>");

            // Write the data for each portal
            foreach (Portal thisPortal in UI_ApplicationCache_Gateway.URL_Portals.All_Portals)
            {
                if (thisPortal != UI_ApplicationCache_Gateway.URL_Portals.Default_Portal)
                {
                    Output.WriteLine("    <tr>");
	                if (RequestSpecificValues.Current_User.Is_System_Admin)
	                {
		                Output.Write("      <td class=\"sbkAdm_ActionLink\" >( ");
		                Output.Write("<a title=\"Edit this portal\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return portal_form_popup( '" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'", "")) + "','" + thisPortal.Abbreviation + "','" + thisPortal.Default_Web_Skin + "','" + thisPortal.Default_Aggregation.ToUpper() + "','" + thisPortal.URL_Segment + "','" + thisPortal.Base_PURL + "');\">edit</a> | ");
		                Output.WriteLine("<a title=\"Delete this portal\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_portal('" + thisPortal.ID + "','" + HttpUtility.HtmlEncode(thisPortal.Name.Replace("'", "")) + "');\">delete</a> )</td>");
	                }
                    Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(thisPortal.Name) + "</td>");
                    Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(thisPortal.Abbreviation) + "</td>");
                    Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(thisPortal.Default_Web_Skin) + "</td>");
                    Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(thisPortal.Default_Aggregation) + "</td>");
                    Output.WriteLine("      <td>" + HttpUtility.HtmlEncode(thisPortal.URL_Segment) + "</td>");
                    Output.WriteLine("    </tr>");
					Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"" + columns + "\"></td></tr>");
                }
            }
            Output.WriteLine("  </table>");
            Output.WriteLine("</div>");

        }
    }
}
