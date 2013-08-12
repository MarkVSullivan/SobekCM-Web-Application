#region Using directives

using System;
using System.Collections.Generic;
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
    /// <summary> Gives the current SobekCM status and allows an authenticated system admin to temporarily halt the 
    /// builder remotely via a database flag  </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer allow the system administrator to view and
    /// update the status of the SobekCM builder application.</li>
    /// </ul></remarks>
    public class Builder_AdminViewer : abstract_AdminViewer
    {
        /// <summary> Constructor for a new instance of the Builder_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        public Builder_AdminViewer(User_Object User, SobekCM_Navigation_Object Current_Mode)
            : base(User)
        {
            currentMode = Current_Mode;

            // Ensure the user is the system admin
            if ((User == null) || (!User.Is_System_Admin))
            {
                Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                Current_Mode.Redirect();
                return;
            }

           // If this is a postback, handle any events first
            if (Current_Mode.isPostBack)
            {
                // Pull the hidden value
                string save_value = HttpContext.Current.Request.Form["admin_builder_tosave"].ToUpper().Trim();
                if (save_value.Length > 0)
                {
                    // Set this value
                    SobekCM_Database.Set_Setting("Builder Operation Flag", save_value);
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'SobekCM Builder Status' </value>
        public override string Web_Title
        {
            get { return "SobekCM Builder Status"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the alias list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Builder_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Builder_AdminViewer.Add_HTML_In_Main_Form", "Add the current status and add html controls to change status");

            Output.WriteLine("<!-- Builder_AdminViewer.Add_HTML_In_Main_Form -->");

            // Pull the builder settings
            Dictionary<string, string> builderSettings = SobekCM_Database.Get_Settings(Tracer);

            Output.WriteLine("<!-- Hidden field to keep the newly requested status -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_builder_tosave\" name=\"admin_builder_tosave\" value=\"\" />");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");

            // Start to show the text 
            Output.WriteLine("<div class=\"SobekHomeText\">");

            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    The SobekCM builder is constantly loading new items and updates and building the full text indexes.  This page can be used to view and updated the current status as well as view the most recent log files.<br /><br />");
            Output.WriteLine("    For more information about the builder and possible actions from this screen, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/builder\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.");
            Output.WriteLine("  </blockquote>");

            // If missing values, display an error
            if ((!builderSettings.ContainsKey("Builder Operation Flag")) || (!builderSettings.ContainsKey("Log Files URL")) || (!builderSettings.ContainsKey("Log Files Directory")))
            {
                Output.WriteLine("<br /><br />ERROR PULLING BUILDER SETTINGS... MISSING VALUES<br /><br />");
            }
            else
            {
                // Get the current pertinent values
                string operationFlag = builderSettings["Builder Operation Flag"];
                string logURL = builderSettings["Log Files URL"];


                Output.WriteLine("  <span class=\"SobekAdminTitle\">SobekCM Builder Status</span>");
                Output.WriteLine("  <blockquote>");
                Output.WriteLine("    <table cellspacing=\"5\" cellpadding=\"5\">");
                Output.WriteLine("      <tr><td>Current Status: </td><td><strong>" + operationFlag + "</strong></td><td>&nbsp;</td></tr>");
                Output.Write("      <tr valign=\"center\"><td>Next Status: </td><td>");
                Output.Write("<select class=\"admin_builder_select\" name=\"admin_builder_status\" id=\"admin_builder_status\">");

                if ((operationFlag != "ABORT REQUESTED") && (operationFlag != "NO BUILDING REQUESTED"))
                    Output.Write("<option value=\"STANDARD OPERATION\" selected=\"selected\">STANDARD OPERATION</option>");
                else
                    Output.Write("<option value=\"STANDARD OPERATION\">STANDARD OPERATION</option>");

                Output.Write(operationFlag == "PAUSE REQUESTED"
                 ? "<option value=\"PAUSE REQUESTED\" selected=\"selected\">PAUSE REQUESTED</option>"
                 : "<option value=\"PAUSE REQUESTED\">PAUSE REQUESTED</option>");

                Output.Write(operationFlag == "ABORT REQUESTED"
                                 ? "<option value=\"ABORT REQUESTED\" selected=\"selected\">ABORT REQUESTED</option>"
                                 : "<option value=\"ABORT REQUESTED\">ABORT REQUESTED</option>");

                Output.Write(operationFlag == "NO BUILDING REQUESTED"
                                 ? "<option value=\"NO BUILDING REQUESTED\" selected=\"selected\" >NO BUILDING REQUESTED</option>"
                                 : "<option value=\"NO BUILDING REQUESTED\" >NO BUILDING REQUESTED</option>");


                Output.Write("</select> </td><td> <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\" onclick=\"return save_new_builder_status();\"/></td></tr>");
                Output.WriteLine("    </table>");
                Output.WriteLine("  </blockquote>");
                Output.WriteLine("  <br />");

                Output.WriteLine("  <span class=\"SobekAdminTitle\">Recent Incoming Logs</span>");
                Output.WriteLine("  <blockquote>");
                Output.WriteLine("    Select a date below to view the recent incoming resources log file:");
                Output.WriteLine("    <blockquote>");

                string logDirectory = SobekCM_Library_Settings.Base_Design_Location + "extra\\logs";
                string[] logFiles = Directory.GetFiles(logDirectory, "incoming*.html");
                Output.WriteLine("    <table cellspacing=\"2\" cellpadding=\"2\">");
                foreach (string logFile in logFiles)
                {
                    string logName = (new FileInfo(logFile)).Name;
                    string date_string = logName.ToLower().Replace("incoming_","").Replace(".html","");
                    if ( date_string.Length == 10 )
                    {
                        DateTime date = new DateTime(Convert.ToInt32(date_string.Substring(0, 4)), Convert.ToInt32(date_string.Substring(5, 2)), Convert.ToInt32(date_string.Substring(8)));
                        Output.WriteLine("      <tr><td><a href=\"" + logURL + logName + "\">" + date.ToLongDateString() + "</a></td></tr>");
                    }
                }
                Output.WriteLine("    </table>");
                Output.WriteLine("    </blockquote>");
                Output.WriteLine("  </blockquote>");
                Output.WriteLine("  <br />");
            }

            Output.WriteLine("  <span class=\"SobekAdminTitle\">Related Links</span>");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    <table cellspacing=\"2\" cellpadding=\"2\">");
            Output.WriteLine("      <tr><td><a href=\"" + currentMode.Base_URL + "l/internal/new\">Newly added or updated items</a></td></tr>");
            Output.WriteLine("      <tr><td><a href=\"" + currentMode.Base_URL + "l/internal/failures\">Failed packages or builder errors</a></td></tr>");
            Output.WriteLine("    </table>");
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <br />");

            Output.WriteLine("</div>");

        }
    }
}



