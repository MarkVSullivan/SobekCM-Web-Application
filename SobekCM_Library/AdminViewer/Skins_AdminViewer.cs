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
        /// <param name="currentMode"> Mode / navigation information for the current request</param>
        /// <param name="Web_Skin_Collection"> Contains the collection of all the default skins and the data to create any additional skins on request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling an edit or new html skin is handled here in the constructor </remarks>
        public Skins_AdminViewer(User_Object User, SobekCM_Navigation_Object currentMode, SobekCM_Skin_Collection Web_Skin_Collection, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Skins_AdminViewer.Constructor", String.Empty);

            // Save the mode and settings  here
            base.currentMode = currentMode;
            skinCollection = Web_Skin_Collection;

            // Set action message to nothing to start
            actionMessage = String.Empty;

            // If the user cannot edit this, go back
            if ((!user.Is_System_Admin) && ( !user.Is_Portal_Admin ))
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
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
                                    catch (Exception ee )
                                    {
                                        bool error = false;
                                    }

                                    // Reload the list of all skins from the database, to include this new skin
                                    lock (skinCollection)
                                    {
                                        SobekCM_Skin_Collection_Builder.Populate_Default_Skins(skinCollection, Tracer);
                                    }
                                    actionMessage = "Saved new html skin <i>" + save_value + "</i>";
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

        private void  copy_entire_folder( string source_folder, string destination_folder )
        {
            // Does the destination folder exist?
            if (!Directory.Exists(destination_folder))
                Directory.CreateDirectory(destination_folder);

            // Copy all the individual files
            string[] files = Directory.GetFiles(source_folder);
            foreach (string thisFile in files)
            {
                FileInfo thisFileInfo = new FileInfo(thisFile);
                if (!File.Exists(destination_folder + "\\" + thisFileInfo.Name))
                    File.Copy(thisFile, destination_folder + "\\" + thisFileInfo.Name);
            }

            // Copy all subdirectories
            string[] subdirs = Directory.GetDirectories(source_folder);
            foreach (string thisDirectory in subdirs)
            {
                DirectoryInfo thisDirectoryInfo = new DirectoryInfo(thisDirectory);
                copy_entire_folder(thisDirectory, destination_folder + "\\" + thisDirectoryInfo.Name);
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

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.8.16.custom.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_interface_tosave\" name=\"admin_interface_tosave\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_interface_reset\" name=\"admin_interface_reset\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- HTML Skins Edit Form -->");
            Output.WriteLine("<div class=\"admin_interface_popup_div\" id=\"form_interface\" style=\"display:none;\">");
            Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT WEB SKIN</td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"interface_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"popup_table\">");

            // Add line for interface code and base interface code
            Output.Write("    <tr align=\"left\"><td width=\"112px\"><label for=\"form_interface_code\">Web Skin Code:</label></td>");
            Output.Write("<td width=\"220px\"><span class=\"form_linkline admin_existing_code_line\" id=\"form_interface_code\"></span></td>");

            Output.WriteLine("<td><label for=\"form_interface_basecode\">Base Skin Code:</label> &nbsp; <input class=\"admin_interface_small_input\" name=\"form_interface_basecode\" id=\"form_interface_basecode\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_interface_basecode', 'admin_interface_small_input_focused')\" onblur=\"javascript:textbox_leave('form_interface_basecode', 'admin_interface_small_input')\" /></td></tr>");

            // Add line for banner link
            Output.WriteLine("    <tr align=\"left\"><td><label for=\"form_interface_link\">Banner Link:</label></td><td colspan=\"2\"><input class=\"admin_interface_large_input\" name=\"form_interface_link\" id=\"form_interface_link\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_interface_link', 'admin_interface_large_input_focused')\" onblur=\"javascript:textbox_leave('form_interface_link', 'admin_interface_large_input')\" /></td></tr>");

            // Add line for notes
            Output.WriteLine("    <tr align=\"left\"><td><label for=\"form_interface_notes\">Notes:</label></td><td colspan=\"2\"><input class=\"admin_interface_large_input\" name=\"form_interface_notes\" id=\"form_interface_notes\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_interface_notes', 'admin_interface_large_input_focused')\" onblur=\"javascript:textbox_leave('form_interface_notes', 'admin_interface_large_input')\" /></td></tr>");

            // Add checkboxes for overriding the header/footer and overriding banner
            Output.WriteLine("          <tr><td>Flags:</td><td><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"form_interface_header_override\" id=\"form_interface_header_override\" checked=\"checked\" /> <label for=\"form_interface_header_override\">Override header and footer?</label></td><td><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"form_interface_banner_override\" id=\"form_interface_banner_override\" /> <label for=\"form_interface_banner_override\">Override banner?</label></td></tr>");
            Output.WriteLine("          <tr><td>&nbsp;</td><td><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"form_interface_top_nav\" id=\"form_interface_top_nav\" /> <label for=\"form_interface_top_nav\">Suppress top-level navigation?</label></td><td><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"form_interface_buildlaunch\" id=\"form_interface_buildlaunch\" /> <label for=\"form_interface_buildlaunch\">Build on launch?</label></td></tr>");
            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <center><a href=\"\" onclick=\"return interface_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center>");
            Output.WriteLine("</div>");


            Tracer.Add_Trace("Skins_AdminViewer.Add_HTML_In_Main_Form", "");

            Output.WriteLine("<!-- Skins_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");

            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            Output.WriteLine("  <blockquote>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "admin/webskins\" target=\"ADMIN_INTERFACE_HELP\" >click here to view the help page</a>.</blockquote>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">New Web Skin</span>");
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("      <div class=\"admin_interface_new_div\">");
            Output.WriteLine("        <table class=\"popup_table\">");

            // Add line for interface code and base interface code
            Output.Write("          <tr><td width=\"112px\"><label for=\"admin_interface_code\">Web Skin Code:</label></td>");
            Output.Write("<td width=\"220px\"><input class=\"admin_interface_small_input\" name=\"admin_interface_code\" id=\"admin_interface_code\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('admin_interface_code', 'admin_interface_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_interface_code', 'admin_interface_small_input')\" /></td>");
            Output.WriteLine("<td><label for=\"admin_interface_basecode\">Base Skin Code:</label> &nbsp; <input class=\"admin_interface_small_input\" name=\"admin_interface_basecode\" id=\"admin_interface_basecode\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_interface_basecode', 'admin_interface_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_interface_basecode', 'admin_interface_small_input')\" /></td></tr>");

            // Add line for banner link
            Output.WriteLine("          <tr><td><label for=\"admin_interface_link\">Banner Link:</label></td><td colspan=\"2\"><input class=\"admin_interface_large_input\" name=\"admin_interface_link\" id=\"admin_interface_link\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_interface_link', 'admin_interface_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_interface_link', 'admin_interface_large_input')\" /></td></tr>");

            // Add line for notes
            Output.WriteLine("          <tr><td><label for=\"admin_interface_notes\">Notes:</label></td><td colspan=\"2\"><input class=\"admin_interface_large_input\" name=\"admin_interface_notes\" id=\"admin_interface_notes\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_interface_notes', 'admin_interface_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_interface_notes', 'admin_interface_large_input')\" /></td></tr>");

            // Add checkboxes for overriding the header/footer and overriding banner
            Output.WriteLine("          <tr><td>Flags:</td><td><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"admin_interface_header_override\" id=\"admin_interface_header_override\" checked=\"checked\" /> <label for=\"admin_interface_header_override\">Override header and footer?</label></td><td><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"admin_interface_banner_override\" id=\"admin_interface_banner_override\" /> <label for=\"admin_interface_banner_override\">Override banner?</label></td></tr>");
            Output.WriteLine("          <tr><td>&nbsp;</td><td><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"admin_interface_top_nav\" id=\"admin_interface_top_nav\" /> <label for=\"admin_interface_top_nav\">Suppress top-level navigation?</label></td><td><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"admin_interface_buildlaunch\" id=\"admin_interface_buildlaunch\" /> <label for=\"admin_interface_buildlaunch\">Build on launch?</label></td></tr>");
            Output.WriteLine("          <tr><td>&nbsp;</td><td colspan=\"2\"><input class=\"admin_interface_checkbox\" type=\"checkbox\" name=\"admin_interface_copycurrent\" id=\"admin_interface_copycurrent\" /> <label for=\"admin_interface_copycurrent\">Copy current files for this new web skin if folder does not exist?</label></td></tr>");
            Output.WriteLine("        </table>");
            Output.WriteLine("        <br />");
            Output.WriteLine("        <center><input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\" onclick=\"return save_new_interface();\"/></center>");


            Output.WriteLine("      </div>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("    <br />");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Web Skins</span>");

            // Get the list of all aggregations
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
            Output.WriteLine("    <th width=\"90px\" align=\"left\"><span style=\"color: White\">CODE</span></th>");
            Output.WriteLine("    <th width=\"90px\" align=\"left\"><span style=\"color: White\">BASE</span></th>");
            Output.WriteLine("    <th width=\"300px\" align=\"left\"><span style=\"color: White\">NOTES</span></th>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");

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
                Output.WriteLine("  <tr align=\"left\" >");
                Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                Output.Write("<a title=\"Click to edit\" id=\"EDIT_" + code + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return interface_form_popup('EDIT_" + code + "', '" + code + "','" + base_code + "','" + bannerLink + "','" + notes + "','" + overrideBanner + "','" + overrideHeader + "','" + suppressTopNav + "','" + buildOnLaunch + "');\">edit</a> | ");
                Output.Write("<a title=\"Click to view\" href=\"" + view_url.Replace("testskincode", code) + "\" >view</a> | ");
                Output.WriteLine("<a title=\"Click to reset\" href=\"javascript:reset_interface('" + code + "');\">reset</a> )</td>");

                // Add the rest of the row with data
                Output.WriteLine("    <td>" + code + "</span></td>");
                Output.WriteLine("    <td>" + base_code + "</span></td>");
                Output.WriteLine("    <td>" + notes + "</span></td>");
                Output.WriteLine("   </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
            }

            Output.WriteLine("</table>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("    <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}
  


