#region Using directives

using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
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
    /// <summary> Class allows an authenticated system admin to view and edit IP restriction ranges used to limit access to certain digital resources </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to manage the IP restriction ranges in this digital library</li>
    /// </ul></remarks>
    public class IP_Restrictions_AdminViewer : abstract_AdminViewer
    {
        private readonly DataSet details;
        private readonly int index;
        private readonly IP_Restriction_Ranges ipRestrictionInfo;
        private readonly IP_Restriction_Range thisRange;

        /// <summary> Constructor for a new instance of the Aliases_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="currentMode"> Mode / navigation information for the current request</param>
        /// <param name="IP_Restrictions"> List of all IP restrictions ranges used in this digital library to restrict access to certain digital resources </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling an edit or new item aggregation alias is handled here in the constructor </remarks>
        public IP_Restrictions_AdminViewer( User_Object User, SobekCM_Navigation_Object currentMode, IP_Restriction_Ranges IP_Restrictions, Custom_Tracer Tracer ) : base(User)
        {
            Tracer.Add_Trace("IP_Restrictions_AdminViewer.Constructor", String.Empty);

            ipRestrictionInfo = IP_Restrictions;
            this.currentMode = currentMode;

            // Ensure the user is the system admin
            if ((User == null) || (!User.Is_System_Admin))
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL(), false);
            }

            // Determine if there is an specific IP address range for editing
            index = -1;
            if (currentMode.My_Sobek_SubMode.Length > 0)
            {
                if ( !Int32.TryParse(currentMode.My_Sobek_SubMode, out index ))
                    index = -1;
            }

            // If there was an index included, try to pull the information about it
            thisRange = null;
            details = null;
            if ((index >= 1) && (index <= ipRestrictionInfo.Count))
            {
                thisRange = ipRestrictionInfo[index - 1];
                if (thisRange != null)
                {
                    details = SobekCM_Database.Get_IP_Restriction_Range_Details(thisRange.RangeID, Tracer);
                }
            }

            if ((currentMode.isPostBack) && ( details != null ) && ( thisRange != null ))
            {
                try
                {
                    // Get a reference to this form
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    // Pull the main values
                    string title = form["admin_title"].Trim();
                    string notes = form["admin_notes"].Trim();
                    string message = form["admin_message"].Trim();

                    if (title.Length == 0)
                    {
                        title = thisRange.Title;
                    }

                    // Edit the main values in the database
                    SobekCM_Database.Edit_IP_Range(thisRange.RangeID, title, notes, message, Tracer);
                    thisRange.Title = title;
                    thisRange.Notes = notes;
                    thisRange.Item_Restricted_Statement = message;

                    // Now check each individual IP address range
                    string[] getKeys = form.AllKeys;
                    int single_ip_index = 0;
                    foreach (string thisKey in getKeys)
                    {
                        // Is this for a new ip address?
                        if (thisKey.IndexOf("admin_ipstart_") == 0)
                        {
                            // Get the basic information for this single ip address
                            string ip_index = thisKey.Replace("admin_ipstart_", "");
                            string thisIpStart = form["admin_ipstart_" + ip_index].Trim();
                            string thisIpEnd = form["admin_ipend_" + ip_index].Trim();
                            string thisIpNote = form["admin_iplabel_" + ip_index].Trim();

                            // Does this match an existing IP range?
                            if ((ip_index.IndexOf("new") < 0) && ( single_ip_index < details.Tables[1].Rows.Count ))
                            {
                                // Get the pre-existing IP row
                                DataRow ipRow = details.Tables[1].Rows[single_ip_index];
                                int singleIpId = Convert.ToInt32(ipRow[0]);
                                if (thisIpStart.Length == 0)
                                {
                                    SobekCM_Database.Delete_Single_IP(singleIpId, Tracer);
                                }
                                else
                                {
                                    // Is this the same?
                                    if ((thisIpStart != ipRow[1].ToString().Trim()) || (thisIpEnd != ipRow[2].ToString().Trim()) || (thisIpNote != ipRow[3].ToString().Trim()))
                                    {
                                        int edit_point_count = thisIpStart.Count(thisChar => thisChar == '.');

                                        if (edit_point_count == 3)
                                        {
                                            SobekCM_Database.Edit_Single_IP(singleIpId, thisRange.RangeID, thisIpStart, thisIpEnd, thisIpNote, Tracer);
                                        }
                                    }
                                }

                                // Be ready to look at the next pre-existing IP range
                                single_ip_index++;
                            }
                            else
                            {
                                // Just add this as a new single ip address
                                if (thisIpStart.Length > 0)
                                {
                                    int add_point_count = thisIpStart.Count(thisChar => thisChar == '.');

                                    if (add_point_count == 3)
                                    {
                                        SobekCM_Database.Edit_Single_IP(-1, thisRange.RangeID, thisIpStart, thisIpEnd, thisIpNote, Tracer);
                                    }
                                }
                            }
                        }
                    }
                }
                catch ( Exception)
                {
                    // Some error caught while handling postback
                }

                // Repopulate the restriction table
                DataTable ipRestrictionTbl = SobekCM_Database.Get_IP_Restriction_Ranges(Tracer);
                if (ipRestrictionTbl != null)
                {
                    IP_Restrictions.Populate_IP_Ranges(ipRestrictionTbl);
                }

                // Forward back to the main form
                currentMode.My_Sobek_SubMode = String.Empty;
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'IP Restriction Ranges' </value>
        public override string Web_Title
        {
            get { return "IP Restriction Ranges"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the alias list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("IP_Restrictions_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            if ((details != null) && (details.Tables[0].Rows.Count > 0))
            {
                Tracer.Add_Trace("IP_Restrictions_AdminViewer.Add_HTML_In_Main_Form", "Display details regarding one IP restrictive range");

                // Assign some of the values from the details to the range
                thisRange.Title = details.Tables[0].Rows[0]["Title"].ToString();
                thisRange.Notes = details.Tables[0].Rows[0]["Notes"].ToString();

                // Add the stylesheet(s)and javascript  needed
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" ></script>");

                // Add the hidden field
                Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                Output.WriteLine("<input type=\"hidden\" id=\"rangeid\" name=\"rangeid\" value=\"" + thisRange.RangeID + "\" />");
                Output.WriteLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");
                Output.WriteLine();

                // Start the HTML rendering
                Output.WriteLine("<div class=\"SobekHomeText\">");

                // Add the save and cancel button and link to help
                currentMode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("  <br />");
                Output.WriteLine("  <table width=\"750px\"><tr><td align=\"left\"> &nbsp; &nbsp; &nbsp; For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "admin/restrictions\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</td><td align=\"right\"><a href=\"" + currentMode.Redirect_URL() + "\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_22.gif\" alt=\"CLOSE\" /></a> &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\"></td></tr></table>");

                // Add all the basic information
                Output.WriteLine("  <span class=\"SobekAdminTitle\">Basic Information</span>");
                Output.WriteLine("  <blockquote>");
                Output.WriteLine("    <div class=\"admin_aggr_new_div\">");
                Output.WriteLine("      <table class=\"popup_table\">");

                // Add line for range title
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td width=\"120px\"><label for=\"admin_title\">Title:</label></td>");
                Output.WriteLine("          <td><input class=\"admin_ip_large_input\" name=\"admin_title\" id=\"admin_title\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisRange.Title) + "\"  onfocus=\"javascript:textbox_enter('admin_title', 'admin_ip_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_title', 'admin_ip_large_input')\" /></td>");
                Output.WriteLine("        </tr>");

                // Compute the size of the text boxes
                int actual_cols = 75;
                if (currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                    actual_cols = 70;

                // Add the notes text area box
                Output.WriteLine("        <tr valign=\"top\"><td valign=\"top\"><label for=\"admin_notes\">Notes:</label></td><td colspan=\"2\"><textarea rows=\"5\" cols=\"" + actual_cols + "\" name=\"admin_notes\" id=\"admin_notes\" class=\"admin_ip_input\" onfocus=\"javascript:textbox_enter('admin_notes','admin_ip_focused')\" onblur=\"javascript:textbox_leave('admin_notes','admin_ip_input')\">" + HttpUtility.HtmlEncode(thisRange.Notes) + "</textarea></td></tr>");

                // Add the message text area box
                Output.WriteLine("        <tr valign=\"top\"><td valign=\"top\"><label for=\"admin_message\">Message:</label></td><td colspan=\"2\"><textarea rows=\"10\" cols=\"" + actual_cols + "\" name=\"admin_message\" id=\"admin_message\" class=\"admin_ip_input\" onfocus=\"javascript:textbox_enter('admin_message','admin_ip_focused')\" onblur=\"javascript:textbox_leave('admin_message','admin_ip_input')\">" + HttpUtility.HtmlEncode(thisRange.Item_Restricted_Statement) + "</textarea></td></tr>");

                Output.WriteLine("      </table>");
                Output.WriteLine("    </div>");
                Output.WriteLine("  </blockquote>");
                Output.WriteLine("  <br />");



                Output.WriteLine("  <span class=\"SobekAdminTitle\">IP Addresses</span>");
                Output.WriteLine("  <br /><br />");
                Output.WriteLine("    <blockquote>");

                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                Output.WriteLine("    <th width=\"90px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
                Output.WriteLine("    <th width=\"135px\" align=\"left\"><span style=\"color: White\">START IP</span></th>");
                Output.WriteLine("    <th width=\"135px\" align=\"left\"><span style=\"color: White\">END IP</span></th>");
                Output.WriteLine("    <th width=\"250px\" align=\"left\"><span style=\"color: White\">LABEL</span></th>");
                Output.WriteLine("  </tr>");

                foreach (DataRow thisRow in details.Tables[1].Rows)
                {
                    // Get the primary key for this IP address
                    string ip_primary = thisRow["IP_SingleID"].ToString();

                    // Build the action links
                    Output.WriteLine("  <tr align=\"left\" >");
                    Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                    Output.WriteLine("<a title=\"Click to clear this ip address\" id=\"CLEAR_" + ip_primary + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return clear_ip_address('" + ip_primary + "');\">clear</a> )</td>");

                    // Add the rest of the row with data
                    Output.WriteLine("    <td><input class=\"admin_ip_small_input\" name=\"admin_ipstart_" + ip_primary + "\" id=\"admin_ipstart_" + ip_primary + "\" type=\"text\" value=\"" + thisRow["StartIP"].ToString().Trim() + "\"  onfocus=\"javascript:textbox_enter('admin_ipstart_" + ip_primary + "', 'admin_ip_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_ipstart_" + ip_primary + "', 'admin_ip_small_input')\" /></td>");
                    Output.WriteLine("    <td><input class=\"admin_ip_small_input\" name=\"admin_ipend_" + ip_primary + "\" id=\"admin_ipend_" + ip_primary + "\" type=\"text\" value=\"" + thisRow["EndIP"].ToString().Trim() + "\"  onfocus=\"javascript:textbox_enter('admin_ipend_" + ip_primary + "', 'admin_ip_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_ipend_" + ip_primary + "', 'admin_ip_small_input')\" /></td>");
                    Output.WriteLine("    <td><input class=\"admin_ip_medium_input\" name=\"admin_iplabel_" + ip_primary + "\" id=\"admin_iplabel_" + ip_primary + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisRow["Notes"].ToString().Trim()) + "\"  onfocus=\"javascript:textbox_enter('admin_iplabel_" + ip_primary + "', 'admin_ip_medium_input_focused')\" onblur=\"javascript:textbox_leave('admin_iplabel_" + ip_primary + "', 'admin_ip_medium_input')\" /></td>");
                    Output.WriteLine("   </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
                }

                // Now, always add ten empty IP rows here
                for (int i = 1; i < 10; i++)
                {
                    Output.WriteLine("  <tr align=\"left\" >");
                    Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                    Output.WriteLine("<a title=\"Click to clear this ip address\" id=\"CLEAR_new" + i + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return clear_ip_address('new" + i + "');\">clear</a> )</td>");

                    // Add the rest of the row with data
                    Output.WriteLine("    <td><input class=\"admin_ip_small_input\" name=\"admin_ipstart_new" + i + "\" id=\"admin_ipstart_new" + i + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('admin_ipstart_new" + i + "', 'admin_ip_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_ipstart_new" + i + "', 'admin_ip_small_input')\" /></td>");
                    Output.WriteLine("    <td><input class=\"admin_ip_small_input\" name=\"admin_ipend_new" + i + "\" id=\"admin_ipend_new" + i + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('admin_ipend_new" + i + "', 'admin_ip_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_ipend_new" + i + "', 'admin_ip_small_input')\" /></td>");
                    Output.WriteLine("    <td><input class=\"admin_ip_medium_input\" name=\"admin_iplabel_new" + i + "\" id=\"admin_iplabel_new" + i + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('admin_iplabel_new" + i + "', 'admin_ip_medium_input_focused')\" onblur=\"javascript:textbox_leave('admin_iplabel_new" + i + "', 'admin_ip_medium_input')\" /></td>");
                    Output.WriteLine("   </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");

                }

                Output.WriteLine("</table>");
                Output.WriteLine("    </blockquote>");

                Output.WriteLine("</div>");

                return;
            }

            Tracer.Add_Trace("IP_Restrictions_AdminViewer.Add_HTML_In_Main_Form", "Display main IP restrictive range admin form");
            Output.WriteLine("<div class=\"SobekHomeText\">");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    Restrictive ranges of IP addresses may be used to restrict access to digital resources.  This form allows system administrators to edit the individual IP addresses and contiguous IP addresses associated with an existing restrictive range.<br /><br />");
            Output.WriteLine("    For more information about IP restriction ranges and this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "admin/restrictions\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.");
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Ranges</span>");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    Select an IP restrictive range below to view or edit:<br />");
            Output.WriteLine("    <blockquote>");

            for (int i = 0; i < ipRestrictionInfo.Count; i++)
            {
                currentMode.My_Sobek_SubMode = ipRestrictionInfo[i].RangeID.ToString();
                Output.WriteLine("<a href=\"" + currentMode.Redirect_URL() + "\">" + ipRestrictionInfo[i].Title + "</a><br /><br />");
            }

            Output.WriteLine("    </blockquote>");
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <br />");
            Output.WriteLine("</div>");
        }
    }
}
