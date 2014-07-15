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
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit existing projects, and add new projects </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to manage the projects in this digital library</li>
    /// </ul></remarks>
    public class Projects_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;

        #region Constructor

        /// <summary> Constructor for a new instance of the Projects_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="currentMode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling a new project is handled here in the constructor </remarks>
        public Projects_AdminViewer(User_Object User, SobekCM_Navigation_Object currentMode, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Projects_AdminViewer.Constructor", String.Empty);

            // Save the mode and settings  here
            base.currentMode = currentMode;

            // Set action message to nothing to start
            actionMessage = String.Empty;

            // If the user cannot edit this, go back
            if ((!user.Is_System_Admin ) && ( !user.Is_Portal_Admin ))
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

                    string save_value = form["admin_project_tosave"].ToUpper().Trim();
                    string code_value = form["admin_project_code"].ToUpper().Trim();

                    // Or.. was this a save request
                    if (save_value.Length > 0)
                    {
                        Tracer.Add_Trace("Projects_AdminViewer.Constructor", "Save project '" + save_value + "'");

                        // Was this to save a new project (from the main page) or rename an existing (from the popup form)?
                        if (save_value == code_value)
                        {
                            string new_base_code = form["admin_project_base"].ToUpper().Trim();
                            string new_name = form["admin_project_name"].Trim();

                            // Save this new interface
                            if (SobekCM_Database.Save_Project(save_value.ToUpper(), new_name, Tracer))
                            {
                                actionMessage = "Saved new project <i>" + save_value + "</i>";
                            }
                            else
                            {
                                actionMessage = "Unable to save new project <i>" + save_value + "</i>";
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
                                actionMessage = "Error copying new project METS to the project folder";
                            }
                        }
                        else
                        {
                            string edit_name = form["form_project_name"].Trim();

                            // Save this existing interface
                            if (SobekCM_Database.Save_Project(save_value.ToUpper(), edit_name, Tracer))
                            {
                                actionMessage = "Renamed existing project <i>" + save_value + "</i>";
                            }
                            else
                            {
                                actionMessage = "Unable to rename existing project <i>" + save_value + "</i>";
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
        /// <value> This always returns the value 'Projects' </value>
        public override string Web_Title
        {
            get { return "Projects"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Projects_AdminViewer.Write_HTML", "Do nothing");

        
        }

                /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Projects_AdminViewer.Add_HTML_In_Main_Form", "Add any popup divisions for form elements");

            // Add the scripts needed
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.6.2.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.8.16.custom.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_project_tosave\" name=\"admin_project_tosave\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- Projects Rename Form -->");
            Output.WriteLine("<div class=\"admin_project_popup_div\" id=\"form_project\" style=\"display:none;\">");
            Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">RENAME PROJECT</td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"project_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"popup_table\">");

            // Add line for code
            Output.Write("    <tr align=\"left\"><td width=\"120px\"><label for=\"form_project_code\">Project Code:</label></td>");
            Output.Write("<td><span class=\"form_linkline admin_existing_code_line\" id=\"form_project_code\"></span></td>");

            // Add line for name
            Output.WriteLine("    <tr><td><label for=\"form_project_name\">Project Name:</label></td><td><input class=\"admin_project_large_input\" name=\"form_project_name\" id=\"form_project_name\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_project_name', 'admin_project_large_input_focused')\" onblur=\"javascript:textbox_leave('form_project_name', 'admin_project_large_input')\" /></td></tr>");

            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <center><a href=\"\" onclick=\"return project_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center>");
            Output.WriteLine("</div>");

            Tracer.Add_Trace("Projects_AdminViewer.Add_HTML_In_Main_Form", "Write the rest of the form html");

            // Get the list of all projects
            DataSet projectsSet = SobekCM_Database.Get_All_Projects_Templates(Tracer);

            Output.WriteLine("<!-- Projects_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");

            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            Output.WriteLine("  <blockquote>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "admin/projects\" target=\"PROJECTS_INTERFACE_HELP\" >click here to view the help page</a>.</blockquote>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">New Project</span>");
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("      <div class=\"admin_project_new_div\">");
            Output.WriteLine("        <table class=\"popup_table\">");

            // Add line for code and base code
            Output.Write("          <tr><td width=\"120px\"><label for=\"admin_project_code\">Project Code:</label></td>");
            Output.Write("<td><input class=\"admin_project_small_input\" name=\"admin_project_code\" id=\"admin_project_code\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('admin_project_code', 'admin_project_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_project_code', 'admin_project_small_input')\" /></td>");
            Output.Write("<td width=\"285px\" ><label for=\"admin_project_base\">Base Project Code:</label> &nbsp;  <select class=\"admin_project_select\" name=\"admin_project_base\" id=\"admin_project_base\">");
            Output.Write("<option value=\"(none)\" selected=\"selected\">(none)</option>");
            foreach (DataRow thisRow in projectsSet.Tables[0].Rows)
            {
                Output.Write("<option value=\"" + thisRow["ProjectCode"] + "\" >" + thisRow["ProjectCode"] + "</option>");
            }
            Output.WriteLine("</select></td></tr>");

            // Add line for name
            Output.WriteLine("          <tr><td><label for=\"admin_project_name\">Project Name:</label></td><td colspan=\"2\"><input class=\"admin_project_large_input\" name=\"admin_project_name\" id=\"admin_project_name\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_project_name', 'admin_project_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_project_name', 'admin_project_large_input')\" /></td></tr>");
            Output.WriteLine("        </table>");
            Output.WriteLine("        <br />");
            Output.WriteLine("        <center><input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\" onclick=\"return save_new_project();\"/></center>");
            Output.WriteLine("      </div>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Projects</span>");
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"110px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
            Output.WriteLine("    <th width=\"140px\" align=\"left\"><span style=\"color: White\">CODE</span></th>");
            Output.WriteLine("    <th width=\"450px\" align=\"left\"><span style=\"color: White\">NAME</span></th>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            currentMode.My_Sobek_SubMode = "XXXXXXX";
            string redirect = currentMode.Redirect_URL();

            // Write the data for each interface
            foreach (DataRow thisRow in projectsSet.Tables[0].Rows)
            {
                // Pull all these values
                string code = thisRow["ProjectCode"].ToString();
                string name = thisRow["ProjectName"].ToString();

                // Build the action links
                Output.WriteLine("  <tr align=\"left\" >");
                Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                Output.Write("<a title=\"Click to edit this project\" id=\"EDIT_" + code + "\" href=\"" + redirect.Replace("XXXXXXX", "1" + code) + "\" >edit</a> | ");
                Output.Write("<a title=\"Click to change the name of this project\" id=\"RENAME_" + code + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return project_form_popup('RENAME_" + code + "', '" + code + "','" + name + "');\">rename</a> )</td>");

                // Add the rest of the row with data
                Output.WriteLine("    <td>" + code + "</span></td>");
                Output.WriteLine("    <td>" + name + "</span></td>");
                Output.WriteLine("   </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            }

            Output.WriteLine("</table>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("    <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }


    }
}



