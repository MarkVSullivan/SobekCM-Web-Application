// HTML5 - 10/12/2013

#region Using directives

using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit existing html skins, and add new skins </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to manage the HTML skins in this digital library</li>
    /// </ul></remarks>
    public class Skins_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly SobekCM_Skin_Collection skinCollection;

        #region Constructor

        /// <summary> Constructor for a new instance of the Skins_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
		/// <param name="CurrentMode"> Mode / navigation information for the current request</param>
        /// <param name="Web_Skin_Collection"> Contains the collection of all the default skins and the data to create any additional skins on request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling an edit or new html skin is handled here in the constructor </remarks>
        public Skins_AdminViewer(User_Object User, SobekCM_Navigation_Object CurrentMode, SobekCM_Skin_Collection Web_Skin_Collection, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Skins_AdminViewer.Constructor", String.Empty);

            // Save the mode and settings  here
			currentMode = CurrentMode;
            skinCollection = Web_Skin_Collection;

            // Set action message to nothing to start
            actionMessage = String.Empty;

            // If the user cannot edit this, go back
            if ((!user.Is_System_Admin) && ( !user.Is_Portal_Admin ))
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

                    string reset_value = form["admin_interface_reset"].ToLower();
                    string save_value = form["admin_interface_tosave"].ToUpper().Trim();
					string delete_value = form["admin_interface_delete"].ToUpper().Trim();
                    string new_interface_code = form["admin_interface_code"].ToUpper().Trim();

                    // Was this a reset request?
                    if (reset_value.Length > 0)
                    {
                        Tracer.Add_Trace("Skins_AdminViewer.Constructor", "Reset html skin '" + reset_value + "'");

                        if (Web_Skin_Collection.Remove(reset_value))
                        {
                            actionMessage = "Removed skin <i>" + reset_value.ToUpper() + "</i> from the semi-permanent skin collection";
                        }
                        else
                        {
                            int values_cleared = Cached_Data_Manager.Remove_Skin(reset_value, Tracer);

                            if (values_cleared == 0)
                            {
                                actionMessage = "Html skin <i>" + reset_value.ToUpper() + "</i> was not in the application cache";
                            }
                            else
                            {
                                actionMessage = "Removed " + values_cleared + " values from the cache for <i>" + reset_value.ToUpper() + "</i>";
                            }
                        }

                    }
					else if (delete_value.Length > 0)
					{
						if (user.Is_System_Admin)
						{
							Tracer.Add_Trace("Skins_AdminViewer.Constructor", "Delete skin '" + delete_value + "' from the database");

							// Delete from the database
							if (SobekCM_Database.Delete_Web_Skin(delete_value, false, Tracer))
							{
								// Set the deleted wordmark message
								actionMessage = "Deleted web skin <i>" + delete_value + "</i>";

								// Remove this web skin from the collection
								SobekCM_Skin_Collection_Builder.Populate_Default_Skins(Web_Skin_Collection, Tracer);
							}
							else
							{
								// Report the error
								if (SobekCM_Database.Last_Exception == null)
								{
									actionMessage = "Unable to delete web skin <i>" + delete_value + "</i> since it is linked to an item, aggregation, or portal";
								}
								else
								{
									actionMessage = "Unknown error while deleting web skin <i>" + delete_value + "</i>";
								}
							}
						}
					}
					else
                    {
                        // Or.. was this a save request
                        if (save_value.Length > 0)
                        {
                            Tracer.Add_Trace("Skins_AdminViewer.Constructor", "Save html skin '" + save_value + "'");

                            bool override_banner = false;
                            bool override_header = false;
                            bool build_on_launch = false;
                            bool suppress_top_nav = false;
                            bool copycurrent = false;
                            object temp_object;

                            // Was this to save a new interface (from the main page) or edit an existing (from the popup form)?
                            if (save_value == new_interface_code)
                            {
                                string new_base_code = form["admin_interface_basecode"].ToUpper().Trim();
                                string new_banner_link = form["admin_interface_link"].Trim();
                                string new_notes = form["admin_interface_notes"].Trim();

                                temp_object = form["admin_interface_banner_override"];
                                if (temp_object != null)
                                {
                                    override_banner = true;
                                }

                                temp_object = form["admin_interface_header_override"];
                                if (temp_object != null)
                                {
                                    override_header = true;
                                }

                                temp_object = form["admin_interface_buildlaunch"];
                                if (temp_object != null)
                                {
                                    build_on_launch = true;
                                }

                                temp_object = form["admin_interface_top_nav"];
                                if (temp_object != null)
                                {
                                    suppress_top_nav = true;
                                }

                                temp_object = form["admin_interface_copycurrent"];
                                if (temp_object != null)
                                {
                                    copycurrent = true;
                                }

                                // Save this new interface
                                if (SobekCM_Database.Save_Web_Skin(save_value, new_base_code, override_banner, override_header, new_banner_link, new_notes, build_on_launch, suppress_top_nav, Tracer))
                                {
                                    // Ensure a folder exists for this, otherwise create one
                                    try
                                    {
                                        string folder = SobekCM_Library_Settings.Base_Design_Location + "skins/" + save_value.ToLower();
                                        if (!Directory.Exists(folder))
                                        {
                                            // Create this directory and the necessary subdirectories
                                            Directory.CreateDirectory(folder);

                                            // Create a default stylesheet
                                            StreamWriter writer = new StreamWriter(folder + "\\" + save_value.ToLower() + ".css");
                                            writer.WriteLine("/*  Skin-specific stylesheet used to override values from the base stylesheets */");
                                            writer.WriteLine();
                                            writer.WriteLine();
                                            writer.Flush();
                                            writer.Close();

                                            // Create the html subdirectory
                                            Directory.CreateDirectory(folder + "/html");

                                            // Do the rest differently depending on whether we should copy the current files
                                            if (!copycurrent)
                                            {
                                                // Write the default header file
                                                writer = new StreamWriter(folder + "\\html\\header.html");
                                                writer.WriteLine("<div id=\"container-inner\">");
                                                writer.WriteLine();
                                                writer.WriteLine("<!-- Add the standard header buttons -->");
                                                writer.WriteLine("<div style=\"width: 100%; background-color: #eeeeee; color: Black; height:30px;\">");
                                                writer.WriteLine("<%BREADCRUMBS%>");
                                                writer.WriteLine("<div style=\"float: right\"><%MYSOBEK%></div>");
                                                writer.WriteLine("</div>");
                                                writer.WriteLine();
                                                writer.WriteLine("<%BANNER%>");
                                                writer.WriteLine();
                                                writer.WriteLine("<div id=\"pagecontainer\">");
                                                writer.WriteLine();
                                                writer.WriteLine("<!-- Blankets out the rest of the web form when a pop-up form is envoked -->");
                                                writer.WriteLine("<div id=\"blanket_outer\" style=\"display:none;\"></div>");
                                                writer.Flush();
                                                writer.Close();

                                                // Write the default header_item file
                                                writer = new StreamWriter(folder + "/html/header_item.html");
                                                writer.WriteLine("<!-- Blankets out the rest of the web form when a pop-up form is envoked -->");
                                                writer.WriteLine("<div id=\"blanket_outer\" style=\"display:none;\"></div>");
                                                writer.WriteLine();
                                                writer.WriteLine("<!-- Add the standard header buttons -->");
                                                writer.WriteLine("<div style=\"width: 100%; background-color: #eeeeee; color: Black; height:30px;\">");
                                                writer.WriteLine("<%BREADCRUMBS%>");
                                                writer.WriteLine("<div style=\"float: right\"><%MYSOBEK%></div>");
                                                writer.WriteLine("</div>");
                                                writer.WriteLine();
                                                writer.WriteLine("<%BANNER%>");                                                    
                                                writer.Flush();
                                                writer.Close();

                                                // Write the default footer file
                                                writer = new StreamWriter(folder + "/html/footer.html");
                                                writer.WriteLine("</div> <!-- END PAGE CONTAINER DIV -->");
                                                writer.WriteLine();
                                                writer.WriteLine("<!-- Add most the standard footer buttons -->");
                                                writer.WriteLine("<center>");
                                                writer.WriteLine("<a href=\"<%BASEURL%>contact<%?URLOPTS%>\">Contact Us</a> | ");
                                                writer.WriteLine("<a href=\"<%BASEURL%>preferences<%?URLOPTS%>\">Preferences</a> | ");
                                                writer.WriteLine("<a href=\"http://ufdc.ufl.edu/sobekcm\">Technical Aspects</a> | ");
                                                writer.WriteLine("<a href=\"<%BASEURL%>stats<%?URLOPTS%>\">Statistics</a> | ");
                                                writer.WriteLine("<a href=\"<%BASEURL%>internal<%?URLOPTS%>\">Internal</a> | ");
                                                writer.WriteLine("<a href=\"<%BASEURL%>admin<%?URLOPTS%>\">Admin</a>");
                                                writer.WriteLine("</center>");
                                                writer.WriteLine("<br />");
                                                writer.WriteLine("<br />");
                                                writer.WriteLine("<span style=\"color: Gray; font-size: 0.8em;\">");
                                                writer.WriteLine("To edit this footer or header, edit header.html or footer.html at:  " + folder + "\\html\\ <br />");
                                                writer.WriteLine("</span>");
                                                writer.WriteLine();
                                                writer.WriteLine("</div> <!-- END CONTAINER INNER -->");      
                                                writer.Flush();
                                                writer.Close();

                                                // Write the default footer item file
                                                writer = new StreamWriter(folder + "/html/footer_item.html");
                                                writer.WriteLine("<!-- Add most the standard footer buttons -->");
                                                writer.WriteLine("<center>");
                                                writer.WriteLine("<a href=\"<%BASEURL%>contact<%?URLOPTS%>\">Contact Us</a> | ");
                                                writer.WriteLine("<a href=\"<%BASEURL%>preferences<%?URLOPTS%>\">Preferences</a> | ");
                                                writer.WriteLine("<a href=\"http://ufdc.ufl.edu/sobekcm\">Technical Aspects</a> | ");
                                                writer.WriteLine("<a href=\"<%BASEURL%>stats<%?URLOPTS%>\">Statistics</a> | ");
                                                writer.WriteLine("<a href=\"<%BASEURL%>internal<%?URLOPTS%>\">Internal</a> | ");
                                                writer.WriteLine("<a href=\"<%BASEURL%>admin<%?URLOPTS%>\">Admin</a>");
                                                writer.WriteLine("</center>");
                                                writer.WriteLine("<br />");
                                                writer.WriteLine("<br />");
                                                writer.WriteLine("<span style=\"color: Gray; font-size: 0.8em;\">");
                                                writer.WriteLine("To edit this footer or header, edit header.html or footer.html at:  " + folder + "\\html\\ <br />");
                                                writer.WriteLine("</span>");
                                                writer.Flush();
                                                writer.Close();
                                            }
                                            else
                                            {
                                                // Copy the web skin information over?
                                                string current_web_skin = currentMode.Skin;
                                                string current_web_folder = SobekCM_Library_Settings.Base_Design_Location + "skins/" + current_web_skin;
                                                copy_entire_folder(current_web_folder, folder);
                                                //if (File.Exists(current_web_folder + "\\" + current_web_skin + ".css"))
                                                //{
                                                //    File.Copy(current_web_folder + "\\" + current_web_skin + ".css", folder + "\\" + new_interface_code + ".css", true );
                                                //}
                                                //if (File.Exists(current_web_folder + "\\html\\header.html"))
                                                //{
                                                //    File.Copy(current_web_folder + "\\html\\header.html", folder + "\\html\\header.html");
                                                //}
                                                //if (File.Exists(current_web_folder + "\\html\\header_item.html"))
                                                //{
                                                //    File.Copy(current_web_folder + "\\html\\header_item.html", folder + "\\html\\header_item.html");
                                                //}
                                                //if (File.Exists(current_web_folder + "\\html\\footer.html"))
                                                //{
                                                //    File.Copy(current_web_folder + "\\html\\footer.html", folder + "\\html\\footer.html");
                                                //}
                                                //if (File.Exists(current_web_folder + "\\html\\footer_item.html"))
                                                //{
                                                //    File.Copy(current_web_folder + "\\html\\footer_item.html", folder + "\\html\\footer_item.html");
                                                //}
                                                if (File.Exists(folder + "\\" + current_web_skin + ".css"))
                                                {
                                                    if (File.Exists(folder + "\\" + new_interface_code + ".css"))
                                                        File.Delete(folder + "\\" + new_interface_code + ".css");
                                                    File.Move( folder + "\\" + current_web_skin + ".css", folder + "\\" + new_interface_code + ".css" );                                                        
                                                }
                                            }

                                            // Irregardless of the user's choice on whether to copy the current skin, if there is NO base skin
                                            // provided and the folder does not exist, then we'll copy over the base skin type of stuff, such
                                            // as buttons, tabs, etc...
                                            if (new_base_code.Length == 0)
                                            {
                                                // What is the current base skin folder then?
                                                string base_skin_folder = SobekCM_Library_Settings.Base_Design_Location + "skins/" + currentMode.Base_Skin;
                                                copy_entire_folder(base_skin_folder + "/buttons", folder + "/buttons");
                                                copy_entire_folder(base_skin_folder + "/tabs", folder + "/tabs");
                                                copy_entire_folder(base_skin_folder + "/zoom_controls", folder + "/zoom_controls");
                                            }

                                        }
                                    }
                                    catch (Exception )
                                    {
	                                    actionMessage = "Error creating some of the files for the new web skin";
                                    }

                                    // Reload the list of all skins from the database, to include this new skin
                                    lock (skinCollection)
                                    {
                                        SobekCM_Skin_Collection_Builder.Populate_Default_Skins(skinCollection, Tracer);
                                    }
	                                if (String.IsNullOrEmpty(actionMessage))
	                                {
		                                actionMessage = "Saved new html skin <i>" + save_value + "</i>";
	                                }
                                }
                                else
                                {
                                    actionMessage = "Unable to save new html skin <i>" + save_value + "</i>";
                                }

                                // Try to create the directory
                                try
                                {
                                    if (!Directory.Exists(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value))
                                    {
                                        Directory.CreateDirectory(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value);
                                    }
                                    if (!Directory.Exists(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value + "\\html"))
                                    {
                                        Directory.CreateDirectory(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value + "\\html");
                                    }
                                    if (new_base_code.Length == 0)
                                    {
                                        if (!Directory.Exists(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value + "\\buttons"))
                                        {
                                            Directory.CreateDirectory(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value + "\\buttons");
                                        }
                                        if (!Directory.Exists(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value + "\\tabs"))
                                        {
                                            Directory.CreateDirectory(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value + "\\tabs");
                                        }
                                        if (!Directory.Exists(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value + "\\zoom_controls"))
                                        {
                                            Directory.CreateDirectory(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + save_value + "\\zoom_controls");
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    actionMessage = "Error creating all the necessary folders";
                                }
                            }
                            else
                            {
                                string edit_base_code = form["form_interface_basecode"].ToUpper().Trim();
                                string edit_banner_link = form["form_interface_link"].Trim();
                                string edit_notes = form["form_interface_notes"].Trim();

                                temp_object = form["form_interface_banner_override"];
                                if (temp_object != null)
                                {
                                    override_banner = true;
                                }

                                temp_object = form["form_interface_header_override"];
                                if (temp_object != null)
                                {
                                    override_header = true;
                                }

                                temp_object = form["form_interface_buildlaunch"];
                                if (temp_object != null)
                                {
                                    build_on_launch = true;
                                }

                                temp_object = form["form_interface_top_nav"];
                                if (temp_object != null)
                                {
                                    suppress_top_nav = true;
                                }

                                // Save this existing interface
                                if (SobekCM_Database.Save_Web_Skin(save_value, edit_base_code, override_banner, override_header, edit_banner_link, edit_notes, build_on_launch, suppress_top_nav, Tracer))
                                {
                                    lock (skinCollection)
                                    {
                                        SobekCM_Skin_Collection_Builder.Populate_Default_Skins(skinCollection, Tracer);
                                    }
                                    Cached_Data_Manager.Remove_Skin(save_value, Tracer);

                                    actionMessage = "Edited existing html skin <i>" + save_value + "</i>";
                                }
                                else
                                {
                                    actionMessage = "Unable to edit existing html skin <i>" + save_value + "</i>";
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    actionMessage = "Unknown error caught while handing your request.";
                }
            }
        }

        private void  copy_entire_folder( string SourceFolder, string DestinationFolder )
        {
            // Does the destination folder exist?
            if (!Directory.Exists(DestinationFolder))
                Directory.CreateDirectory(DestinationFolder);

            // Copy all the individual files
            string[] files = Directory.GetFiles(SourceFolder);
            foreach (string thisFile in files)
            {
                FileInfo thisFileInfo = new FileInfo(thisFile);
                if (!File.Exists(DestinationFolder + "\\" + thisFileInfo.Name))
                    File.Copy(thisFile, DestinationFolder + "\\" + thisFileInfo.Name);
            }

            // Copy all subdirectories
            string[] subdirs = Directory.GetDirectories(SourceFolder);
            foreach (string thisDirectory in subdirs)
            {
                DirectoryInfo thisDirectoryInfo = new DirectoryInfo(thisDirectory);
                copy_entire_folder(thisDirectory, DestinationFolder + "\\" + thisDirectoryInfo.Name);
            }
        }

        #endregion


        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Skins' </value>
        public override string Web_Title
        {
            get { return "Web Skins"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the skin list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Skins_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Skins_AdminViewer.Add_HTML_In_Main_Form", "Add any popup divisions for form elements");
			
			Output.WriteLine("<!-- Skins_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_interface_tosave\" name=\"admin_interface_tosave\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_interface_reset\" name=\"admin_interface_reset\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_interface_delete\" name=\"admin_interface_delete\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- HTML Skins Edit Form -->");
			Output.WriteLine("<div class=\"sbkSav_PopupDiv\" id=\"form_interface\" style=\"display:none;\">");
			Output.WriteLine("  <div class=\"sbkAdm_PopupTitle\"><table style=\"width:100%;\"><tr><td style=\"text-align:left;\">EDIT WEB SKIN</td><td style=\"text-align:right;\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"interface_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
			Output.WriteLine("  <table class=\"sbkAdm_PopupTable\">");

            // Add line for interface code and base interface code
			Output.WriteLine("    <tr style=\"height:25px;\">");
			Output.WriteLine("      <td style=\"width:112px;\"><label for=\"form_interface_code\">Web Skin Code:</label></td>");
            Output.WriteLine("      <td style=\"width:220px;\"><span class=\"form_linkline admin_existing_code_line\" id=\"form_interface_code\"></span></td>");
			Output.WriteLine("      <td style=\"text-align:right;\"><label for=\"form_interface_basecode\">Base Skin Code:</label> &nbsp; <input class=\"sbkSav_small_input1 sbkAdmin_Focusable\" name=\"form_interface_basecode\" id=\"form_interface_basecode\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("    </tr>");

            // Add line for banner link
			Output.WriteLine("    <tr style=\"height:25px;\"><td><label for=\"form_interface_link\">Banner Link:</label></td><td colspan=\"2\"><input class=\"sbkSav_large_input sbkAdmin_Focusable\" name=\"form_interface_link\" id=\"form_interface_link\" type=\"text\" value=\"\" /></td></tr>");

            // Add line for notes
			Output.WriteLine("    <tr style=\"height:25px;\"><td><label for=\"form_interface_notes\">Notes:</label></td><td colspan=\"2\"><input class=\"sbkSav_large_input sbkAdmin_Focusable\" name=\"form_interface_notes\" id=\"form_interface_notes\" type=\"text\" value=\"\" /></td></tr>");

            // Add checkboxes for overriding the header/footer and overriding banner
			Output.WriteLine("          <tr style=\"height:15px;\"><td>Flags:</td><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"form_interface_header_override\" id=\"form_interface_header_override\" checked=\"checked\" /> <label for=\"form_interface_header_override\">Override header and footer?</label></td><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"form_interface_banner_override\" id=\"form_interface_banner_override\" /> <label for=\"form_interface_banner_override\">Override banner?</label></td></tr>");
			Output.WriteLine("          <tr style=\"height:15px;\"><td>&nbsp;</td><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"form_interface_top_nav\" id=\"form_interface_top_nav\" /> <label for=\"form_interface_top_nav\">Suppress top-level navigation?</label></td><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"form_interface_buildlaunch\" id=\"form_interface_buildlaunch\" /> <label for=\"form_interface_buildlaunch\">Build on launch?</label></td></tr>");


			// Add the buttons
			Output.WriteLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			Output.WriteLine("      <td colspan=\"3\">");
			Output.WriteLine("        <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return interface_form_close();\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button title=\"Save changes to this existing web skin\" class=\"sbkAdm_RoundButton\" type=\"submit\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");
			Output.WriteLine("  </table>");
			Output.WriteLine("</div>");
			Output.WriteLine();

            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (!String.IsNullOrEmpty(actionMessage))
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\">" + actionMessage + "</div>");
			}

            Output.WriteLine("  <p>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/webskins\" target=\"ADMIN_INTERFACE_HELP\" >click here to view the help page</a>.</p>");

            Output.WriteLine("  <h2>New Web Skin</h2>");
			Output.WriteLine("  <div class=\"sbkSav_NewDiv\">");
			Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");

            // Add line for interface code and base interface code
	        Output.WriteLine("      <tr style=\"height:25px;\">");
			Output.WriteLine("        <td style=\"width:112px;\"><label for=\"admin_interface_code\">Web Skin Code:</label></td>");
			Output.WriteLine("        <td style=\"width:220px;\"><input class=\"sbkSav_small_input sbkAdmin_Focusable\" name=\"admin_interface_code\" id=\"admin_interface_code\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("        <td style=\"text-align:right;\"><label for=\"admin_interface_basecode\">Base Skin Code:</label> &nbsp; <input class=\"sbkSav_small_input2 sbkAdmin_Focusable\" name=\"admin_interface_basecode\" id=\"admin_interface_basecode\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("      </tr>");

            // Add line for banner link
			Output.WriteLine("      <tr style=\"height:25px;\"><td><label for=\"admin_interface_link\">Banner Link:</label></td><td colspan=\"2\"><input class=\"sbkSav_large_input sbkAdmin_Focusable\" name=\"admin_interface_link\" id=\"admin_interface_link\" type=\"text\" value=\"\" /></td></tr>");

            // Add line for notes
			Output.WriteLine("      <tr style=\"height:25px;\"><td><label for=\"admin_interface_notes\">Notes:</label></td><td colspan=\"2\"><input class=\"sbkSav_large_input sbkAdmin_Focusable\" name=\"admin_interface_notes\" id=\"admin_interface_notes\" type=\"text\" value=\"\" /></td></tr>");

            // Add checkboxes for overriding the header/footer and overriding banner
			Output.WriteLine("      <tr style=\"height:15px;\"><td>Flags:</td><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"admin_interface_header_override\" id=\"admin_interface_header_override\" checked=\"checked\" /> <label for=\"admin_interface_header_override\">Override header and footer?</label></td><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"admin_interface_banner_override\" id=\"admin_interface_banner_override\" /> <label for=\"admin_interface_banner_override\">Override banner?</label></td></tr>");
			Output.WriteLine("      <tr style=\"height:15px;\"><td>&nbsp;</td><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"admin_interface_top_nav\" id=\"admin_interface_top_nav\" /> <label for=\"admin_interface_top_nav\">Suppress top-level navigation?</label></td><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"admin_interface_buildlaunch\" id=\"admin_interface_buildlaunch\" /> <label for=\"admin_interface_buildlaunch\">Build on launch?</label></td></tr>");
			Output.WriteLine("      <tr style=\"height:15px;\"><td>&nbsp;</td><td colspan=\"2\"><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"admin_interface_copycurrent\" id=\"admin_interface_copycurrent\" /> <label for=\"admin_interface_copycurrent\">Copy current files for this new web skin if folder does not exist?</label></td></tr>");

			// Add the SAVE button
			Output.WriteLine("      <tr style=\"height:30px; text-align: center;\"><td colspan=\"3\"><button title=\"Save new web skin\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_interface();\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td></tr>");
		    Output.WriteLine("    </table>");
		    Output.WriteLine("  </div>");
			Output.WriteLine();
            Output.WriteLine("  <h2>Existing Web Skins</h2>");

            // Get the list of all aggregations
			Output.WriteLine("  <table class=\"sbkSav_Table sbkAdm_Table\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSav_TableHeader1\">ACTIONS</th>");
            Output.WriteLine("      <th class=\"sbkSav_TableHeader2\">CODE</th>");
            Output.WriteLine("      <th class=\"sbkSav_TableHeader3\">BASE</th>");
            Output.WriteLine("      <th class=\"sbkSav_TableHeader4\">NOTES</th>");
            Output.WriteLine("    </tr>");
			Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"4\"></td></tr>");

            // Get the view URL
            string current_skin = currentMode.Skin;
            currentMode.Skin = "TESTSKINCODE";
            string view_url = currentMode.Redirect_URL();
            currentMode.Skin = current_skin;

            // Write the data for each interface
            foreach (DataRow thisRow in skinCollection.Skin_Table.Rows)
            {
                // Pull all these values
                string code = thisRow["WebSkinCode"].ToString();
                string base_code = thisRow["BaseInterface"].ToString();
                string notes = thisRow["Notes"].ToString();
                bool overrideHeader = Convert.ToBoolean(thisRow["OverrideHeaderFooter"]);
                bool overrideBanner = Convert.ToBoolean(thisRow["OverrideBanner"]);
                bool buildOnLaunch = Convert.ToBoolean(thisRow["Build_On_Launch"]);
                bool suppressTopNav = Convert.ToBoolean(thisRow["SuppressTopNavigation"]);
                string bannerLink = thisRow["BannerLink"].ToString();

                // Build the action links
                Output.WriteLine("    <tr>");
                Output.Write("      <td class=\"sbkAdm_ActionLink\" >( ");
                Output.Write("<a title=\"Click to edit\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return interface_form_popup('" + code + "','" + base_code + "','" + bannerLink + "','" + notes + "','" + overrideBanner + "','" + overrideHeader + "','" + suppressTopNav + "','" + buildOnLaunch + "');\">edit</a> | ");
                Output.Write("<a title=\"Click to view\" href=\"" + view_url.Replace("testskincode", code) + "\" >view</a> | ");
				Output.Write("<a title=\"Click to reset\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"reset_interface('" + code + "');\">reset</a> | ");

				if ( user.Is_System_Admin )	
					Output.WriteLine("<a title=\"Click to delete this web skin\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\"  onclick=\"return delete_interface('" + code + "');\">delete</a> )</td>");
				else
					Output.WriteLine("<a title=\"Only SYSTEM administrators can delete web skins\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\"  onclick=\"alert('Only SYSTEM administrators can delete web skins'); return false;\">delete</a> )</td>");


                // Add the rest of the row with data
                Output.WriteLine("      <td>" + code + "</span></td>");
                Output.WriteLine("      <td>" + base_code + "</span></td>");
                Output.WriteLine("      <td>" + notes + "</span></td>");
                Output.WriteLine("    </tr>");
				Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"4\"></td></tr>");
            }

            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}
  


