// HTML5 - 10/13/2013

#region Using directives

using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
	/// <summary> Class allows an authenticated system admin to view and edit existing default metadata files, and add new default metadata files </summary>
	/// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
	/// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
	/// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
	/// During a valid html request, the following steps occur:
	/// <ul>
	/// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
	/// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
	/// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
	/// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
	/// <li>The mySobek subwriter creates an instance of this viewer to manage the default metadata in this digital library</li>
	/// </ul></remarks>
	public class Default_Metadata_AdminViewer : abstract_AdminViewer
	{
		private readonly string actionMessage;

		#region Constructor

		/// <summary> Constructor for a new instance of the Default_Metadata_AdminViewer class </summary>
		/// <param name="User"> Authenticated user information </param>
		/// <param name="CurrentMode"> Mode / navigation information for the current request</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> Postback from handling a new project is handled here in the constructor </remarks>
		public Default_Metadata_AdminViewer(User_Object User, SobekCM_Navigation_Object CurrentMode, Custom_Tracer Tracer)
			: base(User)
		{
			Tracer.Add_Trace("Default_Metadata_AdminViewer.Constructor", String.Empty);

			// Save the mode and settings  here
			currentMode = CurrentMode;

			// Set action message to nothing to start
			actionMessage = String.Empty;

			// If the user cannot edit this, go back
			if ((!user.Is_System_Admin ) && ( !user.Is_Portal_Admin ))
			{
				currentMode.Mode = Display_Mode_Enum.My_Sobek;
				currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
				currentMode.Redirect();
				return;
			}

			// If this is a postback, handle any events first
			if (currentMode.isPostBack)
			{
				try
				{
					// Pull the standard values
					NameValueCollection form = HttpContext.Current.Request.Form;

					string save_value = form["admin_project_tosave"].ToUpper().Trim();
					string delete_value = form["admin_project_delete"].ToUpper().Trim();
					string code_value = form["admin_project_code"].ToUpper().Trim();

					// Was this a delete request?
					if (( user.Is_System_Admin ) && ( delete_value.Length > 0))
					{
						Tracer.Add_Trace("Default_Metadata_AdminViewer.Constructor", "Delete default metadata '" + delete_value + "'");
					
						// Try to delete in the database
						if (SobekCM_Database.Delete_Project(delete_value, Tracer))
						{
							// Set the message
							actionMessage = "Deletes default metadata '" + delete_value + "'";

							// Look for the file to delete as well
							string pmets_file = SobekCM_Library_Settings.Base_MySobek_Directory + "projects\\" + delete_value + ".pmets";
							if (File.Exists(pmets_file))
							{
								try
								{
									File.Delete(pmets_file);
								}
								catch 
								{
									actionMessage = "Deleted default metadata '" + delete_value + "' but failed to delete associated pmets file";
								}
							}
						}
						else
						{
							actionMessage = "Error encountered deleting default metadata '" + delete_value + "' from the database";
						}
					}
					else if (save_value.Length > 0) // Or.. was this a save request
					{
						Tracer.Add_Trace("Default_Metadata_AdminViewer.Constructor", "Save default metadata '" + save_value + "'");

						// Was this to save a new project (from the main page) or rename an existing (from the popup form)?
						if (save_value == code_value)
						{
							string new_base_code = form["admin_project_base"].ToUpper().Trim();
							string new_name = form["admin_project_name"].Trim();

							// Save this new interface
							if (SobekCM_Database.Save_Default_Metadata(save_value.ToUpper(), new_name, Tracer))
							{
								actionMessage = "Saved new default metadata <i>" + save_value + "</i>";
							}
							else
							{
								actionMessage = "Unable to save new default metadata <i>" + save_value + "</i>";
							}

							// Try to creating the PMETS file if there was a base PMETS code provided
							try
							{
								if (new_base_code.Length > 0)
								{
									string pmets_file = SobekCM_Library_Settings.Base_MySobek_Directory + "projects\\" + code_value + ".pmets";
									string base_pmets_file = SobekCM_Library_Settings.Base_MySobek_Directory + "projects\\" + new_base_code + ".pmets";

									if (File.Exists(base_pmets_file))
										File.Copy(base_pmets_file, pmets_file, true);
								}
							}
							catch ( Exception )
							{
								actionMessage = "Error copying new default metadata METS to the project folder";
							}
						}
						else
						{
							string edit_name = form["form_project_name"].Trim();

							// Save this existing interface
							if (SobekCM_Database.Save_Default_Metadata(save_value.ToUpper(), edit_name, Tracer))
							{
								actionMessage = "Renamed existing default metadata <i>" + save_value + "</i>";
							}
							else
							{
								actionMessage = "Unable to rename existing default metadata <i>" + save_value + "</i>";
							}
						}
					}
				}
				catch (Exception )
				{
					actionMessage = "Unknown exception occurred while processing your request";
				}
			}
		}

		#endregion

		/// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
		/// <value> This always returns the value 'Default Metadata' </value>
		public override string Web_Title
		{
			get { return "Default Metadata"; }
		}

		/// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
		/// <param name="Output"> Textwriter to write the HTML for this viewer</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
		public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("Default_Metadata_AdminViewer.Write_HTML", "Do nothing");
		}

				/// <summary> This is an opportunity to write HTML directly into the main form, without
		/// using the pop-up html form architecture </summary>
		/// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("Default_Metadata_AdminViewer.Write_ItemNavForm_Closing", "Add any popup divisions for form elements");

			Output.WriteLine("<!-- Default_Metadata_AdminViewer.Write_ItemNavForm_Closing -->");

			// Add the scripts needed
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");

			// Add the hidden field
			Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save -->");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_project_tosave\" name=\"admin_project_tosave\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_project_delete\" name=\"admin_project_delete\" value=\"\" />");
			Output.WriteLine();

			Output.WriteLine("<!-- Default_Metadata Rename Form -->");
			Output.WriteLine("<div class=\"sbkPav_PopupDiv\" id=\"form_project\" style=\"display:none;\">");
			Output.WriteLine("  <div class=\"sbkAdm_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left;\">RENAME DEFAULT METADATA</td><td style=\"text-align:right;\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"project_form_close()\">X</a> &nbsp; </td></tr></table></div>");
			Output.WriteLine("  <br />");
			Output.WriteLine("  <table class=\"sbkAdm_PopupTable\">");

			// Add line for code
			Output.WriteLine("    <tr>");
			Output.WriteLine("      <td style=\"width:120px;\"><label for=\"form_project_code\">Metadata Code:</label></td>");
			Output.WriteLine("      <td><span class=\"form_linkline admin_existing_code_line\" id=\"form_project_code\"></span></td>");
			Output.WriteLine("    </tr>");

			// Add line for name
			Output.WriteLine("    <tr>");
			Output.WriteLine("      <td><label for=\"form_project_name\">Metadata Name:</label></td>");
			Output.WriteLine("      <td><input class=\"sbkPav_large_input sbkAdmin_Focusable\" name=\"form_project_name\" id=\"form_project_name\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("    </tr>");

			// Add the buttons and close the table
			Output.WriteLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			Output.WriteLine("      <td colspan=\"2\"> &nbsp; &nbsp; ");
			Output.WriteLine("        <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return project_form_close();\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button title=\"Save changes to this default metadata\" class=\"sbkAdm_RoundButton\" type=\"submit\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");
			Output.WriteLine("  </table>");
			Output.WriteLine("</div>");
			Output.WriteLine();

			Tracer.Add_Trace("Default_Metadata_AdminViewer.Write_ItemNavForm_Closing", "Write the rest of the form html");

			// Get the list of all projects
            DataSet projectsSet = SobekCM_Database.Get_All_Template_DefaultMetadatas(Tracer);

			Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (actionMessage.Length > 0)
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\">" + actionMessage + "</div>");
			}

			Output.WriteLine("  <p>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/projects\" target=\"PROJECTS_INTERFACE_HELP\" >click here to view the help page</a>.</p>");

			Output.WriteLine("  <h2>New Default Metadata</h2>");
			Output.WriteLine("  <div class=\"sbkPav_NewDiv\">");
			Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");

			// Add line for code and base code
			Output.WriteLine("      <tr>");
			Output.WriteLine("        <td style=\"width:120px;\"><label for=\"admin_project_code\">Metadata Code:</label></td>");
			Output.WriteLine("        <td><input class=\"sbkPav_small_input sbkAdmin_Focusable\" name=\"admin_project_code\" id=\"admin_project_code\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("        <td style=\"width:285px;\">");
			Output.WriteLine("          <label for=\"admin_project_base\">Base Metadata Code:</label> &nbsp; ");
			Output.WriteLine("          <select class=\"sbkPav_select\" name=\"admin_project_base\" id=\"admin_project_base\">");
			Output.WriteLine("            <option value=\"(none)\" selected=\"selected\">(none)</option>");
			foreach (DataRow thisRow in projectsSet.Tables[0].Rows)
			{
				Output.Write("            <option value=\"" + thisRow["MetadataCode"] + "\" >" + thisRow["MetadataCode"] + "</option>");
			}
			Output.WriteLine("          </select>");
			Output.WriteLine("        </td>");
			Output.WriteLine("      </tr>");

			// Add line for name
			Output.WriteLine("      <tr>");
			Output.WriteLine("        <td><label for=\"admin_project_name\">Metadata Name:</label></td>");
			Output.WriteLine("        <td colspan=\"2\"><input class=\"sbkPav_large_input sbkAdmin_Focusable\" name=\"admin_project_name\" id=\"admin_project_name\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("      <tr>");

			// Add the SAVE button
			Output.WriteLine("      <tr style=\"height:30px; text-align: center;\"><td colspan=\"3\"><button title=\"Save new default metadata\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_project();\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td></tr>");
			Output.WriteLine("    </table>");
			Output.WriteLine("  </div>");
			Output.WriteLine("  <br />");
			Output.WriteLine();

			// Add all the existing proejcts
			Output.WriteLine("  <h2>Existing Default Metadata</h2>");
			Output.WriteLine("  <table class=\"sbkPav_Table sbkAdm_Table\">");
			Output.WriteLine("    <tr>");
			Output.WriteLine("      <th class=\"sbkPav_TableHeader1\">ACTIONS</th>");
			Output.WriteLine("      <th class=\"sbkPav_TableHeader2\">CODE</th>");
			Output.WriteLine("      <th class=\"sbkPav_TableHeader3\">NAME</th>");
			Output.WriteLine("    </tr>");
			Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"3\"></td></tr>");

			currentMode.My_Sobek_SubMode = "XXXXXXX";
			string redirect = currentMode.Redirect_URL();

			// Write the data for each interface
			foreach (DataRow thisRow in projectsSet.Tables[0].Rows)
			{
				// Pull all these values
				string code = thisRow["MetadataCode"].ToString();
				string name = thisRow["MetadataName"].ToString();

				// Build the action links
				Output.WriteLine("    <tr>");
				Output.Write("      <td class=\"sbkAdm_ActionLink\" >( ");
				Output.Write("<a title=\"Click to edit this default metadata\" href=\"" + redirect.Replace("XXXXXXX", "1" + code) + "\" >edit</a> | ");
				Output.Write("<a title=\"Click to change the name of this default metadata\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return project_form_popup('" + code + "','" + name + "');\">rename</a> ");
				if (String.Compare(code, "none", StringComparison.OrdinalIgnoreCase) != 0)
				{
					if ( user.Is_System_Admin )
						Output.WriteLine("| <a title=\"Click to delete this default metadata\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\"  onclick=\"return delete_project('" + code + "');\">delete</a> )</td>");
					else
						Output.WriteLine("| <a title=\"Only SYSTEM administrators can delete default metadata\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\"  onclick=\"alert('Only SYSTEM administrators can delete default metadata'); return false;\">delete</a> )</td>");
				}
				else
					Output.WriteLine(")</td>");


				// Add the rest of the row with data
				Output.WriteLine("      <td>" + code + "</td>");
				Output.WriteLine("      <td>" + name + "</td>");
				Output.WriteLine("    </tr>");
				Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"3\"></td></tr>");
			}

			Output.WriteLine("  </table>");
			Output.WriteLine("  <br />");
			Output.WriteLine("</div>");
			Output.WriteLine();
		}
	}
}



