// HTML5 - 10/14

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
using SobekCM.Library.Settings;
using SobekCM.Core.Users;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

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
		private readonly string actionMessage;

		/// <summary> Constructor for a new instance of the IP_Restrictions_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
		/// <param name="CurrentMode"> Mode / navigation information for the current request</param>
        /// <param name="IP_Restrictions"> List of all IP restrictions ranges used in this digital library to restrict access to certain digital resources </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling an edit or new item aggregation alias is handled here in the constructor </remarks>
        public IP_Restrictions_AdminViewer( User_Object User, SobekCM_Navigation_Object CurrentMode, IP_Restriction_Ranges IP_Restrictions, Custom_Tracer Tracer ) : base(User)
        {
            Tracer.Add_Trace("IP_Restrictions_AdminViewer.Constructor", String.Empty);

            ipRestrictionInfo = IP_Restrictions;
			currentMode = CurrentMode;

            // Ensure the user is the system admin
            if ((User == null) || ((!User.Is_System_Admin) && ( !User.Is_Portal_Admin )))
            {
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
                return;
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

            if ((currentMode.isPostBack) && ( user.Is_System_Admin ))
            {
				// Get a reference to this form
				NameValueCollection form = HttpContext.Current.Request.Form;

				string action = form["action"].Trim();

				if (action == "new")
				{
					// Pull the main values
					string title = form["new_admin_title"].Trim();
					string notes = form["new_admin_notes"].Trim();
					string message = form["new_admin_message"].Trim();

					if ((title.Length == 0) || (message.Length == 0))
					{
						actionMessage = "Both title and message are required fields";
					}
					else
					{
						if ( SobekCM_Database.Edit_IP_Range(-1, title, notes, message, Tracer))
							actionMessage = "Saved new IP range '" + title + "'";
						else
							actionMessage = "Error saving new IP range '" + title + "'";
					}
				}
				else if (( details != null ) && ( thisRange != null ))
				{
					try
					{
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
								if ((ip_index.IndexOf("new") < 0) && (single_ip_index < details.Tables[1].Rows.Count))
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
											int edit_point_count = thisIpStart.Count(ThisChar => ThisChar == '.');

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
										int add_point_count = thisIpStart.Count(ThisChar => ThisChar == '.');

										if (add_point_count == 3)
										{
											SobekCM_Database.Edit_Single_IP(-1, thisRange.RangeID, thisIpStart, thisIpEnd, thisIpNote, Tracer);
										}
									}
								}
							}
						}
					}
					catch (Exception)
					{
						actionMessage = "Error saving IP range";
					}
				}
 

                // Repopulate the restriction table
                DataTable ipRestrictionTbl = SobekCM_Database.Get_IP_Restriction_Ranges(Tracer);
                if (ipRestrictionTbl != null)
                {
                    IP_Restrictions.Populate_IP_Ranges(ipRestrictionTbl);
                }

                // Forward back to the main form
	            if (String.IsNullOrEmpty(actionMessage))
	            {
		            currentMode.My_Sobek_SubMode = String.Empty;
		            currentMode.Redirect();
	            }
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
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
			Output.WriteLine("<!-- IP_Restrictions_AdminViewer.Write_ItemNavForm_Closing -->");

			// Add the stylesheet(s)and javascript  needed
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" ></script>");
			Output.WriteLine();

			// Add the hidden field
			Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
			if ( thisRange != null )
				Output.WriteLine("<input type=\"hidden\" id=\"rangeid\" name=\"rangeid\" value=\"" + thisRange.RangeID + "\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");
			Output.WriteLine();

			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (!String.IsNullOrEmpty(actionMessage))
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\">" + actionMessage + "</div>");
			}
			
            if ((details != null) && (thisRange != null) && (details.Tables[0].Rows.Count > 0))
            {
                Tracer.Add_Trace("IP_Restrictions_AdminViewer.Write_ItemNavForm_Closing", "Display details regarding one IP restrictive range");

                // Assign some of the values from the details to the range
                thisRange.Title = details.Tables[0].Rows[0]["Title"].ToString();
                thisRange.Notes = details.Tables[0].Rows[0]["Notes"].ToString();

                // Add the save and cancel button and link to help
                currentMode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("  <br />");
	            Output.WriteLine("  <table style=\"width:750px;\">");
	            Output.WriteLine("    <tr>");
	            Output.WriteLine("      <td> &nbsp; &nbsp; &nbsp; For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/restrictions\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</td>");
	            Output.WriteLine("      <td style=\"text-align:right\">");
	            if (user.Is_System_Admin)
	            {
					Output.WriteLine("        <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"parent.location='" + currentMode.Redirect_URL() + "';return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
					Output.WriteLine("        <button title=\"Save changes to this IP restriction range\" class=\"sbkAdm_RoundButton\" type=\"submit\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
	            }
	            else
	            {
					Output.WriteLine("        <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"parent.location='" + currentMode.Redirect_URL() + "';return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> BACK</button> &nbsp; &nbsp; ");
	            }
	            Output.WriteLine("      </td>");
	            Output.WriteLine("    </tr>");
				Output.WriteLine("  </table>");
				Output.WriteLine();

				// Add portal admin message
				string readonly_tag = String.Empty;
				int columns = 4;
				if (!user.Is_System_Admin)
				{
					Output.WriteLine("<p>Portal Admins have rights to see these settings. System Admins can change these settings.</p>");
					readonly_tag = " readonly=\"readonly\"";
					columns = 3;
				}

                // Add all the basic information
                Output.WriteLine("  <h2>Basic Information</h2>");
				Output.WriteLine("  <div class=\"sbkIpav_NewDiv\">");
				Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");

                // Add line for range title
                Output.WriteLine("      <tr>");
                Output.WriteLine("        <td style=\"width:120px;\"><label for=\"admin_title\">Title:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkIpav_large_input sbkAdmin_Focusable\" name=\"admin_title\" id=\"admin_title\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisRange.Title) + "\" " + readonly_tag + " /></td>");
                Output.WriteLine("      </tr>");

				// Add the notes text area box
				Output.WriteLine("      <tr style=\"vertical-align:top\"><td><label for=\"admin_notes\">Notes:</label></td><td colspan=\"2\"><textarea rows=\"5\" name=\"admin_notes\" id=\"admin_notes\" class=\"sbkIpav_input sbkAdmin_Focusable\"" + readonly_tag + ">" + HttpUtility.HtmlEncode(thisRange.Notes) + "</textarea></td></tr>");

				// Add the message text area box
				Output.WriteLine("      <tr style=\"vertical-align:top\"><td><label for=\"admin_message\">Message:</label></td><td colspan=\"2\"><textarea rows=\"10\" name=\"admin_message\" id=\"admin_message\" class=\"sbkIpav_input sbkAdmin_Focusable\"" + readonly_tag + " >" + HttpUtility.HtmlEncode(thisRange.Item_Restricted_Statement) + "</textarea></td></tr>");

                Output.WriteLine("    </table>");
                Output.WriteLine("  </div>");
				Output.WriteLine();

                Output.WriteLine("  <h2>IP Addresses</h2>");

				Output.WriteLine("  <table class=\"sbkIpav_Table sbkAdm_Table\">");
                Output.WriteLine("    <tr>");
				if ( user.Is_System_Admin )
					Output.WriteLine("      <th class=\"sbkIpav_TableHeader1\">ACTIONS</th>");
				Output.WriteLine("      <th class=\"sbkIpav_TableHeader2\">START IP</th>");
				Output.WriteLine("      <th class=\"sbkIpav_TableHeader3\">END IP</th>");
				Output.WriteLine("      <th class=\"sbkIpav_TableHeader4\">LABEL</th>");
                Output.WriteLine("    </tr>");

                foreach (DataRow thisRow in details.Tables[1].Rows)
                {
                    // Get the primary key for this IP address
                    string ip_primary = thisRow["IP_SingleID"].ToString();

                    // Build the action links
                    Output.WriteLine("    <tr>");

					if ( user.Is_System_Admin )
						Output.WriteLine("      <td class=\"sbkAdm_ActionLink\" >( <a title=\"Click to clear this ip address\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return clear_ip_address('" + ip_primary + "');\">clear</a> )</td>");

                    // Add the rest of the row with data
					Output.WriteLine("      <td><input class=\"sbkIpav_small_input sbkAdmin_Focusable\" name=\"admin_ipstart_" + ip_primary + "\" id=\"admin_ipstart_" + ip_primary + "\" type=\"text\" value=\"" + thisRow["StartIP"].ToString().Trim() + "\" /></td>");
					Output.WriteLine("      <td><input class=\"sbkIpav_small_input sbkAdmin_Focusable\" name=\"admin_ipend_" + ip_primary + "\" id=\"admin_ipend_" + ip_primary + "\" type=\"text\" value=\"" + thisRow["EndIP"].ToString().Trim() + "\" /></td>");
					Output.WriteLine("      <td><input class=\"sbkIpav_medium_input sbkAdmin_Focusable\" name=\"admin_iplabel_" + ip_primary + "\" id=\"admin_iplabel_" + ip_primary + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisRow["Notes"].ToString().Trim()) + "\" /></td>");
                    Output.WriteLine("     </tr>");
					Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"" + columns  + "\"></td></tr>");
                }


                // Now, always add ten empty IP rows here, for system administrators
	            if (user.Is_System_Admin)
	            {
		            for (int i = 1; i < 10; i++)
		            {
			            Output.WriteLine("    <tr>");
			            Output.WriteLine("      <td class=\"sbkAdm_ActionLink\" >( <a title=\"Click to clear this ip address\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return clear_ip_address('new" + i + "');\">clear</a> )</td>");

			            // Add the rest of the row with data
			            Output.WriteLine("      <td><input class=\"sbkIpav_small_input sbkAdmin_Focusable\" name=\"admin_ipstart_new" + i + "\" id=\"admin_ipstart_new" + i + "\" type=\"text\" value=\"\" /></td>");
			            Output.WriteLine("      <td><input class=\"sbkIpav_small_input sbkAdmin_Focusable\" name=\"admin_ipend_new" + i + "\" id=\"admin_ipend_new" + i + "\" type=\"text\" value=\"\" /></td>");
			            Output.WriteLine("      <td><input class=\"sbkIpav_medium_input sbkAdmin_Focusable\" name=\"admin_iplabel_new" + i + "\" id=\"admin_iplabel_new" + i + "\" type=\"text\" value=\"\" /></td>");
			            Output.WriteLine("    </tr>");
			            Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"4\"></td></tr>");
		            }
	            }

	            Output.WriteLine("  </table>");
                Output.WriteLine("</div>");

                return;
            }

            Tracer.Add_Trace("IP_Restrictions_AdminViewer.Write_ItemNavForm_Closing", "Display main IP restrictive range admin form");


            Output.WriteLine("  <p>Restrictive ranges of IP addresses may be used to restrict access to digital resources.  This form allows system administrators to edit the individual IP addresses and contiguous IP addresses associated with an existing restrictive range.</p>");
            Output.WriteLine("  <p>For more information about IP restriction ranges and this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/restrictions\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");
	        Output.WriteLine();

	        if (user.Is_System_Admin)
	        {
		        // Add all the basic information
		        Output.WriteLine("  <h2>New IP Restrictive Range</h2>");
		        Output.WriteLine("  <div class=\"sbkIpav_NewDiv\">");
		        Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");

		        // Add line for range title
		        Output.WriteLine("      <tr>");
		        Output.WriteLine("        <td style=\"width:120px;\"><label for=\"admin_title\">Title:</label></td>");
		        Output.WriteLine("        <td><input class=\"sbkIpav_large_input sbkAdmin_Focusable\" name=\"new_admin_title\" id=\"new_admin_title\" type=\"text\" value=\"\" /></td>");
		        Output.WriteLine("      </tr>");

		        // Add the notes text area box
		        Output.WriteLine("      <tr style=\"vertical-align:top\"><td><label for=\"admin_notes\">Notes:</label></td><td colspan=\"2\"><textarea rows=\"5\" name=\"new_admin_notes\" id=\"new_admin_notes\" class=\"sbkIpav_input sbkAdmin_Focusable\" ></textarea></td></tr>");

		        // Add the message text area box
		        Output.WriteLine("      <tr style=\"vertical-align:top\"><td><label for=\"admin_message\">Message:</label></td><td colspan=\"2\"><textarea rows=\"10\" name=\"new_admin_message\" id=\"new_admin_message\" class=\"sbkIpav_input sbkAdmin_Focusable\" ></textarea></td></tr>");
		        // Add the SAVE button
				Output.WriteLine("      <tr style=\"height:30px; text-align: center;\"><td></td><td><button title=\"Save new IP restrictive range\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_ip_range();\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td></tr>");
		        Output.WriteLine("    </table>");
		        Output.WriteLine("  </div>");
		        Output.WriteLine();
	        }
	        else
	        {
				// Add portal admin message
				Output.WriteLine("<p>Portal Admins have rights to see these settings. System Admins can change these settings.</p>");
	        }

	        Output.WriteLine("  <h2>Existing Ranges</h2>");
	        if (ipRestrictionInfo.Count == 0)
	        {
		        Output.WriteLine("  <p>No existing IP restrictive ranges exist.</p>");
				if ( user.Is_System_Admin )
					Output.WriteLine("<p>To add one, enter the information above and press SAVE.</p>");
	        }
	        else
	        {
		        Output.WriteLine("  <p>Select an IP restrictive range below to view or edit:</p>");
		        Output.WriteLine("  <ul id=\"sbkIpav_RangeList\">");

		        for (int i = 0; i < ipRestrictionInfo.Count; i++)
		        {
			        currentMode.My_Sobek_SubMode = ipRestrictionInfo[i].RangeID.ToString();
			        Output.WriteLine("<li><a href=\"" + currentMode.Redirect_URL() + "\">" + ipRestrictionInfo[i].Title + "</a></li>");
		        }

		        Output.WriteLine("  </ul>");
	        }

			Output.WriteLine("</div>");
        }
    }
}
