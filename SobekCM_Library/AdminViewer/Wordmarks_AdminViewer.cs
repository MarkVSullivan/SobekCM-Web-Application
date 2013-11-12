// HTML5 = 10/12/2013 MVS

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;
using SobekCM.Tools;
using darrenjohnstone.net.FileUpload;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit existing interfaces, and add new interfaces </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to manage the HTML interfaces in this digital library</li>
    /// </ul></remarks>
    public class Wordmarks_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
		private DJAccessibleProgressBar djAccessibleProgrssBar1;
		private DJFileUpload djFileUpload1;
		private DJUploadController djUploadController1;
	    private readonly string wordmarkDirectory;
	    private readonly List<string> loweredFiles;
	    private readonly Dictionary<string, Wordmark_Icon> wordmarks;

        #region Constructor

        /// <summary> Constructor for a new instance of the Wordmarks_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from editing an existing wordmark, deleting a wordmark, or creating a new wordmark is handled here in the constructor </remarks>
        public Wordmarks_AdminViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Custom_Tracer Tracer) : base(User)
        {
            Tracer.Add_Trace("Wordmarks_AdminViewer.Constructor", String.Empty);

			// Save the mode and settings  here
            currentMode = Current_Mode;

            // Set action message to nothing to start
            actionMessage = String.Empty;

            // If the user cannot edit this, go back
            if ((user == null ) || ((!user.Is_System_Admin) && ( !user.Is_Portal_Admin )))
            {
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
                return;
            }

			// Get the wordmark directory and ensure it exists
			wordmarkDirectory = HttpContext.Current.Server.MapPath("design/wordmarks");
	        if (!Directory.Exists(wordmarkDirectory))
		        Directory.CreateDirectory(wordmarkDirectory);

			// Get the list of all wordmarks
			wordmarks = new Dictionary<string, Wordmark_Icon>();
			SobekCM_Database.Populate_Icon_List(wordmarks, Tracer);

            // If this is a postback, handle any events first
           // if (currentMode.isPostBack)
           // {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;
					if (form["admin_wordmark_code_delete"] != null)
					{
		                string delete_value = form["admin_wordmark_code_delete"].ToUpper().Trim();
		                string save_value = form["admin_wordmark_code_tosave"].ToUpper().Trim();
		                string new_wordmark_code = form["admin_wordmark_code"].ToUpper().Trim();

		                // Was this a reset request?
		                if (delete_value.Length > 0)
		                {
							// If the value to delete does not have a period, then it has no extension, 
							// so this is to delete a USED wordmark which is both a file AND in the database
			                if (delete_value.IndexOf(".") < 0)
			                {
				                Tracer.Add_Trace("Wordmarks_AdminViewer.Constructor", "Delete wordmark '" + delete_value + "' from the database");

								// Get the wordmark, so we can also delete the file
				                Wordmark_Icon deleteIcon = wordmarks[delete_value];

								// Delete from the database
				                if (SobekCM_Database.Delete_Icon(delete_value, Tracer))
				                {
									// Set the deleted wordmark message
					                actionMessage = "Deleted wordmark <i>" + delete_value + "</i>";

									// Try to delete the file related to this wordmark now
									if ((deleteIcon != null) && (File.Exists(wordmarkDirectory + "\\" + deleteIcon.Image_FileName)))
									{
										try
										{
											File.Delete(wordmarkDirectory + "\\" + deleteIcon.Image_FileName);
										}
										catch (Exception)
										{
											actionMessage = "Deleted wordmark <i>" + delete_value + "</i> but unable to delete the file <i>" + deleteIcon.Image_FileName + "</i>";
										}
									}

									// Repull the wordmark list now
									wordmarks = new Dictionary<string, Wordmark_Icon>();
									SobekCM_Database.Populate_Icon_List(wordmarks, Tracer);
				                }
				                else
				                {
									// Report the error
					                if (SobekCM_Database.Last_Exception == null)
					                {
						                actionMessage = "Unable to delete wordmark <i>" + delete_value + "</i> since it is in use";
					                }
					                else
					                {
						                actionMessage = "Unknown error while deleting wordmark <i>" + delete_value + "</i>";
					                }
				                }
			                }
			                else
			                {
				                // This is to delete just a file, which presumably is unused by the system
								// and does not appear in the database
								// Try to delete the file related to this wordmark now
								if (File.Exists(wordmarkDirectory + "\\" + delete_value))
								{
									try
									{
										File.Delete(wordmarkDirectory + "\\" + delete_value);
										actionMessage = "Deleted unused image file <i>" + delete_value + "</i>";
									}
									catch (Exception)
									{
										actionMessage = "Unable to delete unused image <i>" + delete_value + "</i>";
									}
								}
			                }
		                }
		                else
		                {
			                // Or.. was this a save request
			                if (save_value.Length > 0)
			                {
				                Tracer.Add_Trace("Wordmarks_AdminViewer.Constructor", "Save wordmark '" + save_value + "'");

				                // Was this to save a new interface (from the main page) or edit an existing (from the popup form)?
				                if (save_value == new_wordmark_code)
				                {
					                string new_file = form["admin_wordmark_file"].Trim();
					                string new_link = form["admin_wordmark_link"].Trim();
					                string new_title = form["admin_wordmark_title"].Trim();

					                // Save this new wordmark
					                if (SobekCM_Database.Save_Icon(new_wordmark_code, new_file, new_link, new_title, Tracer) > 0)
					                {
						                actionMessage = "Saved new wordmark <i>" + save_value + "</i>";
					                }
					                else
					                {
						                actionMessage = "Unable to save new wordmark <i>" + save_value + "</i>";
					                }
				                }
				                else
				                {
					                string edit_file = form["form_wordmark_file"].Trim();
					                string edit_link = form["form_wordmark_link"].Trim();
					                string edit_title = form["form_wordmark_title"].Trim();

					                // Save this existing wordmark
					                if (SobekCM_Database.Save_Icon(save_value, edit_file, edit_link, edit_title, Tracer) > 0)
					                {
						                actionMessage = "Edited existing wordmark <i>" + save_value + "</i>";
					                }
					                else
					                {
						                actionMessage = "Unable to edit existing wordmark <i>" + save_value + "</i>";
					                }
				                }

								// Repull the wordmark list now
								wordmarks = new Dictionary<string, Wordmark_Icon>();
								SobekCM_Database.Populate_Icon_List(wordmarks, Tracer);
			                }
		                }
	                }
                }
                catch ( Exception )
                {
                    actionMessage = "Unknown error caught while handing request.";
                }
            //}

			// Get the list of wordmarks in the directory
			string[] allFiles = SobekCM_File_Utilities.GetFiles(wordmarkDirectory, "*.jpg|*.jpeg|*.png|*.gif|*.bmp");
			loweredFiles = allFiles.Select(ThisFileName => new FileInfo(ThisFileName)).Select(ThisFileInfo => ThisFileInfo.Name.ToLower()).ToList();
			loweredFiles.Sort();

			// Get the list of all assigned wordmark files
			foreach (Wordmark_Icon thisWordmark in wordmarks.Values)
			{
				if (loweredFiles.Contains(thisWordmark.Image_FileName.ToLower()))
					loweredFiles.Remove(thisWordmark.Image_FileName.ToLower());
			}
        }

        #endregion


        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Skins' </value>
        public override string Web_Title
        {
            get { return "Wordmarks / Icons"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the wordmarks list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Wordmarks_AdminViewer.Write_HTML", "Do nothing");

			Output.WriteLine("<!-- Wordmarks_AdminViewer.Write_HTML -->");

			Tracer.Add_Trace("Wordmarks_AdminViewer.Write_ItemNavForm_Closing", "Add any popup divisions for form elements");

			Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");

			// Start this added form
			string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());
			Output.WriteLine("<form name=\"sbkAdm_AddedForm\" method=\"post\" action=\"" + post_url + "\" id=\"sbkAdm_AddedForm\" >");

			// Add the hidden field
			Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_wordmark_code_tosave\" name=\"admin_wordmark_code_tosave\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_wordmark_code_delete\" name=\"admin_wordmark_code_delete\" value=\"\" />");
			Output.WriteLine();

			Output.WriteLine("<!-- Wordmarks Edit Form -->");
			Output.WriteLine("<div class=\"sbkWav_PopupDiv\" id=\"form_wordmark\" name=\"form_wordmark\" style=\"display:none;\">");
			Output.WriteLine("  <div class=\"sbkAdm_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left;\">EDIT WORDMARK / ICON <td><td style=\"text-align:right;\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"wordmark_form_close()\">X</a> &nbsp; </td></tr></table></div>");
			Output.WriteLine("  <br />");
			Output.WriteLine("  <table class=\"sbkAdm_PopupTable\">");

			// Add line for interface code and base interface code
	        Output.WriteLine("    <tr style=\"text-align:left;\">");
			Output.WriteLine("      <td><label for=\"form_wordmark_code\">Wordmark Code:</label></td>");
			Output.WriteLine("      <td><span class=\"form_linkline admin_existing_code_line\" id=\"form_wordmark_code\"></span></td>");
			Output.WriteLine("    </tr>");

			// Add line for filename
			Output.WriteLine("    <tr><td><label for=\"form_wordmark_file\">Image File:</label></td><td><input readonly=\"readonly\" class=\"sbkWav_medium_input sbkAdmin_Focusable\" name=\"form_wordmark_file\" id=\"form_wordmark_file\" type=\"text\" value=\"\" /></td></tr>");

			// Add line for title
			Output.WriteLine("    <tr><td><label for=\"form_wordmark_title\">Title:</label></td><td><input class=\"sbkWav_large_input sbkAdmin_Focusable\" name=\"form_wordmark_title\" id=\"form_wordmark_title\" type=\"text\" value=\"\" /></td></tr>");

			// Add line for banner link
			Output.WriteLine("    <tr><td><label for=\"form_wordmark_link\">Link:</label></td><td><input class=\"sbkWav_large_input sbkAdmin_Focusable\" name=\"form_wordmark_link\" id=\"form_wordmark_link\" type=\"text\" value=\"\" /></td></tr>");

			// Add the buttons
			Output.WriteLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			Output.WriteLine("      <td colspan=\"2\">");
			Output.WriteLine("        <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return wordmark_form_close();\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button title=\"Save changes to this existing wordmark\" class=\"sbkAdm_RoundButton\" type=\"submit\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button> &nbsp; &nbsp; ");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");
			Output.WriteLine("  </table>");
			Output.WriteLine("</div>");

			Tracer.Add_Trace("Wordmarks_AdminViewer.Write_HTML", "Write the HTML for the rest of the form");

			Output.WriteLine();
			Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (actionMessage.Length > 0)
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\">" + actionMessage + "</div>");
			}

			Output.WriteLine("  <p>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/wordmarks\" target=\"ADMIN_WORDMARK_HELP\" >click here to view the help page</a>.</p>");



		        Output.WriteLine("  <h2>New Wordmark / Icon</h2>");
				Output.WriteLine("  <p>Add a new wordmark to this repository.  If you have not yet uploaded the image to use, do this first.</p>");

	        if (loweredFiles.Count > 0)
	        {

		        Output.WriteLine("  <div style=\"width:210px; float:right;\"><img id=\"sbkWav_SelectedImage\" name=\"sbkWav_SelectedImage\" src=\"" + currentMode.Base_URL + "design/wordmarks/" + loweredFiles[0] + "\" alt=\"Missing\" Title=\"Selected image file\" /></div>");

		        Output.WriteLine("    <div class=\"sbkWav_NewDiv\">");
				Output.WriteLine("      <table class=\"sbkAdm_PopupTable\">");

		        // Add line for wordmark code
		        Output.WriteLine("        <tr>");
		        Output.WriteLine("          <td style=\"width:120px\"><label for=\"admin_wordmark_code\">Wordmark Code:</label></td>");
		        Output.WriteLine("          <td><input class=\"sbkWav_small_input sbkAdmin_Focusable\" name=\"admin_wordmark_code\" id=\"admin_wordmark_code\" type=\"text\" value=\"\" /></td>");
		        Output.WriteLine("        </tr>");

		        // Add line for filename
		        Output.WriteLine("        <tr>");
		        Output.WriteLine("          <td><label for=\"admin_wordmark_file\">Image File:</label></td>");
		        Output.WriteLine("          <td>");
		        Output.WriteLine("            <select class=\"sbkWav_medium_input\" name=\"admin_wordmark_file\" id=\"admin_wordmark_file\" onchange=\"wordmark_select_changed('" + currentMode.Base_URL + "design/wordmarks/');\">");

		        foreach (string thisFile in loweredFiles)
		        {
			        Output.WriteLine("              <option value=\"" + thisFile + "\">" + thisFile + "</option>");
		        }

		        Output.WriteLine("            </select>");
		        Output.WriteLine("          </td>");
		        Output.WriteLine("        </tr>");


		        // Add line for title
		        Output.WriteLine("        <tr><td><label for=\"admin_wordmark_title\">Title:</label></td><td><input class=\"sbkWav_large_input sbkAdmin_Focusable\" name=\"admin_wordmark_title\" id=\"admin_wordmark_title\" type=\"text\" value=\"\" /></td></tr>");

		        // Add line for banner link
		        Output.WriteLine("        <tr><td><label for=\"admin_wordmark_link\">Link:</label></td><td><input class=\"sbkWav_large_input sbkAdmin_Focusable\" name=\"admin_wordmark_link\" id=\"admin_wordmark_link\" type=\"text\" value=\"\" /></td></tr>");

				// Add the SAVE button
				Output.WriteLine("        <tr style=\"height:30px; text-align: center;\"><td colspan=\"2\"><button title=\"Save new wordmark\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_wordmark();\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td></tr>");
		        Output.WriteLine("      </table>");
		        Output.WriteLine("    </div>");
	        }
	        else
	        {
				Output.WriteLine("  <br />");
	        }

	        Output.WriteLine("  </div>");
			Output.WriteLine("  <br />");
			Output.WriteLine("</form>");

			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");
			Output.WriteLine("  <h2>Upload New Image File</h2>");
			Output.WriteLine("  <blockquote>");
			Output.WriteLine("  Browse to a new wordmark image file below and then select Upload to add the new wordmark image file.  The image should be a jpeg, gif, bmp, or png file.<br /><br />");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
			Output.WriteLine("<!-- Wordmarks_AdminViewer.Write_ItemNavForm_Closing -->");

			Output.WriteLine("  </blockquote>");


			Output.WriteLine("  <br />");
			Output.WriteLine("  <br />");

	        if (loweredFiles.Count > 0)
	        {
		        Output.WriteLine("  <h2>Unused Image Files</h2>");
		        Output.WriteLine("  <p>These are uploaded image files which are not associated with a wordmark/icon.  Use the form above to register the unused image files and allow them to be attached to digital resources.</p>");

		        Output.WriteLine("  <table style=\"border:0; width:100%; border-collapse: collapse; border-spacing: 0;\" class=\"statsTable\">");
		        //			Output.WriteLine("  <tr><td colspan=\"4\" style=\"background-color:#e7e7e7;\" ></td></tr>");
		        Output.WriteLine("    <tr style=\"text-align:center; vertical-align:bottom;\" >");

		        int unused_column = 0;
		        foreach (string thisIcon in loweredFiles)
		        {
			        Output.Write("      <td style=\"width:210px;\">");
			        Output.Write("<img style=\"border: 0;\" class=\"sbkWav_ItemWordmark\" src=\"" + currentMode.Base_URL + "design/wordmarks/" + thisIcon + "\" alt=\"Missing Thumbnail\" title=\"" + thisIcon + "\" />");
			        Output.Write("<br /><span class=\"sbkWav_ItemWordmarkTitle\">" + thisIcon + "</span>");

			        // Build the action links
			        Output.Write("<br /><span class=\"sbkAdm_ActionLink\" >( ");
			        Output.Write("<a title=\"Click to delete this file\" href=\"javascript:delete_wordmark_file('" + thisIcon + "');\">delete</a> )</span>");
			        Output.WriteLine("</td>");

			        unused_column++;

			        if (unused_column >= 4)
			        {
				        Output.WriteLine("    </tr>");
				        Output.WriteLine("    <tr><td colspan=\"4\" class=\"sbkWav_TableRule\" ></td></tr>");
				        Output.WriteLine("    <tr style=\"text-align:center; vertical-align:bottom;\" >");
				        unused_column = 0;
			        }
		        }

		        if (unused_column > 0)
		        {
			        Output.WriteLine("    <tr><td colspan=\"4\" class=\"sbkWav_TableRule\" ></td></tr>");
		        }

		        Output.WriteLine("  </table>");
		        Output.WriteLine("  <br />");
		        Output.WriteLine("  <br />");
	        }

	        if (wordmarks.Count > 0)
	        {
		        Output.WriteLine("  <h2>Existing Wordmarks / Icons</h2>");
		        Output.WriteLine("  <p>Wordmark/icons which exist within the system and are ready to be attached to digital resources.</p>");

		        Output.WriteLine("  <table style=\"border:0; width:100%; border-collapse: collapse; border-spacing: 0;\" class=\"statsTable\">");
		        //			Output.WriteLine("  <tr><td colspan=\"4\" style=\"background-color:#e7e7e7;\" ></td></tr>");
		        Output.WriteLine("    <tr style=\"text-align:center; vertical-align:bottom;\" >");

		        int current_column = 0;
		        SortedList<string, Wordmark_Icon> sortedIcons = new SortedList<string, Wordmark_Icon>();
		        foreach (Wordmark_Icon thisIcon in wordmarks.Values)
		        {
			        sortedIcons.Add(thisIcon.Code, thisIcon);
		        }

		        foreach (Wordmark_Icon thisIcon in sortedIcons.Values)
		        {
			        Output.Write("      <td style=\"width:210px;\">");
			        if (thisIcon.Link.Length > 0)
				        Output.Write("<a href=\"" + thisIcon.Link.Replace("<%BASEURL%>", currentMode.Base_URL).Replace("<%?URLOPTS%>", "") + "\" target=\"_blank\">");
			        Output.Write("<img style=\"border: 0;\" class=\"sbkWav_ItemWordmarkTitle\" src=\"" + currentMode.Base_URL + "design/wordmarks/" + thisIcon.Image_FileName + "\"");
			        if (thisIcon.Title.Length > 0)
				        Output.Write(" title=\"" + thisIcon.Title + "\"");
			        Output.Write(" />");
			        if (thisIcon.Link.Length > 0)
				        Output.Write("</a>");
			        Output.Write("<br /><span class=\"sbkWav_ItemWordmarkTitle\">" + thisIcon.Code + "</span>");

			        // Build the action links
			        Output.Write("<br /><span class=\"sbkAdm_ActionLink\" >( ");
			        Output.Write("<a title=\"Click to edit\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return wordmark_form_popup( '" + thisIcon.Code + "', '" + thisIcon.Title.Replace("'", "") + "','" + thisIcon.Image_FileName + "','" + thisIcon.Link + "');\">edit</a> | ");
			        Output.Write("<a title=\"Click to delete\" href=\"javascript:delete_wordmark('" + thisIcon.Code + "');\">delete</a> )</span>");
			        Output.WriteLine("</td>");

			        current_column++;

			        if (current_column >= 4)
			        {
				        Output.WriteLine("    </tr>");
				        Output.WriteLine("    <tr><td colspan=\"4\" class=\"sbkWav_TableRule\" ></td></tr>");
				        Output.WriteLine("    <tr style=\"text-align:center; vertical-align:bottom;\" >");
				        current_column = 0;
			        }
		        }

				if (current_column > 0)
		        {
			        Output.WriteLine("  <tr><td colspan=\"4\" class=\"sbkWav_TableRule\" ></td></tr>");
		        }

		        Output.WriteLine("  </table>");
		        Output.WriteLine("  <br />");
	        }
	        Output.WriteLine("</div>");
            Output.WriteLine();

			Output.WriteLine("<!-- END Wordmarks_AdminViewer.Write_ItemNavForm_Closing -->");
			Output.WriteLine();
        }

		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
		/// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("File_Managament_MySobekViewer.Add_Controls", String.Empty);

			// Add the upload controls to the file place holder
			add_upload_controls(MainPlaceHolder, Tracer);
		}

		private void add_upload_controls(PlaceHolder UploadFilesPlaceHolder, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("File_Managament_MySobekViewer.add_upload_controls", String.Empty);

			StringBuilder filesBuilder = new StringBuilder(2000);

			LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
			UploadFilesPlaceHolder.Controls.Add(filesLiteral2);
			filesBuilder.Remove(0, filesBuilder.Length);

			djUploadController1 = new DJUploadController
			{
				CSSPath = currentMode.Base_URL + "default/scripts/upload_styles",
				ImagePath = currentMode.Base_URL + "default/scripts/upload_images",
				ScriptPath = currentMode.Base_URL + "default/scripts/upload_scripts",
				AllowedFileExtensions = ".jpg,.png,.gif,.bmp,.jpeg"
			};
			UploadFilesPlaceHolder.Controls.Add(djUploadController1);

			djAccessibleProgrssBar1 = new DJAccessibleProgressBar();
			UploadFilesPlaceHolder.Controls.Add(djAccessibleProgrssBar1);

			djFileUpload1 = new DJFileUpload { ShowAddButton = false, ShowUploadButton = true, MaxFileUploads = 1, AllowedFileExtensions = ".jpg,.png,.gif,.bmp,.jpeg", GoButton_CSS = "sbkAdm_UploadButton" };
			UploadFilesPlaceHolder.Controls.Add(djFileUpload1);

			// Set the default processor
			FileSystemProcessor fs = new FileSystemProcessor { OutputPath = wordmarkDirectory };
			djUploadController1.DefaultFileProcessor = fs;

			// Change the file processor and set it's properties.
			FieldTestProcessor fsd = new FieldTestProcessor { OutputPath = wordmarkDirectory };
			djFileUpload1.FileProcessor = fsd;

			LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
			UploadFilesPlaceHolder.Controls.Add(literal1);
		}
    }
}

