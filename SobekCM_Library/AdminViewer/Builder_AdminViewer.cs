// HTML5 - 10/14

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

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
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Builder_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Ensure the user is the system admin
            if ((RequestSpecificValues.Current_User == null) || ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin )))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

           // If this is a postback, handle any events first
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                if (((RequestSpecificValues.Current_User.Is_System_Admin) && (!UI_ApplicationCache_Gateway.Settings.isHosted)) ||
                    (RequestSpecificValues.Current_User.Is_Host_Admin))
                {
                    // Pull the hidden value
                    string save_value = HttpContext.Current.Request.Form["admin_builder_tosave"].ToUpper().Trim();
                    if (save_value.Length > 0)
                    {
                        // Set this value
                        SobekCM_Database.Set_Setting("Builder Operation Flag", save_value);
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    }
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'SobekCM Builder Status' </value>
        public override string Web_Title
        {
            get { return "SobekCM Builder Status"; }
        }


        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.Gears_Img; }
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
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Builder_AdminViewer.Write_ItemNavForm_Closing", "Add the current status and add html controls to change status");

            Output.WriteLine("<!-- Builder_AdminViewer.Write_ItemNavForm_Closing -->");

            // Pull the builder settings
            Dictionary<string, string> builderSettings = SobekCM_Database.Get_Settings(Tracer);

            Output.WriteLine("<!-- Hidden field to keep the newly requested status -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_builder_tosave\" name=\"admin_builder_tosave\" value=\"\" />");
            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");

            // Start to show the text 
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

            Output.WriteLine("  <p>The SobekCM builder is constantly loading new items and updates and building the full text indexes.  This page can be used to view and update the current status as well as view the most recent log files.</p>");
            Output.WriteLine("  <p>For more information about the builder and possible actions from this screen, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/builder\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");
			Output.WriteLine();

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


                Output.WriteLine("  <h2>SobekCM Builder Status</h2>");
				Output.WriteLine("  <table class=\"sbkBav_table\">");
                Output.WriteLine("    <tr><td>Current Status: </td><td><strong>" + operationFlag + "</strong></td><td>&nbsp;</td></tr>");

                if (((RequestSpecificValues.Current_User.Is_System_Admin) && ( !UI_ApplicationCache_Gateway.Settings.isHosted )) ||
                    (RequestSpecificValues.Current_User.Is_Host_Admin))
	            {
		            Output.WriteLine("    <tr>");
		            Output.WriteLine("      <td>Next Status: </td>");
		            Output.WriteLine("      <td>");
		            Output.WriteLine("        <select class=\"sbkBav_select\" name=\"admin_builder_status\" id=\"admin_builder_status\">");

		            if ((operationFlag != "ABORT REQUESTED") && (operationFlag != "NO BUILDING REQUESTED"))
			            Output.WriteLine("          <option value=\"STANDARD OPERATION\" selected=\"selected\">STANDARD OPERATION</option>");
		            else
			            Output.WriteLine("          <option value=\"STANDARD OPERATION\">STANDARD OPERATION</option>");

		            Output.WriteLine(operationFlag == "PAUSE REQUESTED"
			                             ? "          <option value=\"PAUSE REQUESTED\" selected=\"selected\">PAUSE REQUESTED</option>"
			                             : "          <option value=\"PAUSE REQUESTED\">PAUSE REQUESTED</option>");

		            Output.WriteLine(operationFlag == "ABORT REQUESTED"
			                             ? "          <option value=\"ABORT REQUESTED\" selected=\"selected\">ABORT REQUESTED</option>"
			                             : "          <option value=\"ABORT REQUESTED\">ABORT REQUESTED</option>");

		            Output.WriteLine(operationFlag == "NO BUILDING REQUESTED"
			                             ? "          <option value=\"NO BUILDING REQUESTED\" selected=\"selected\" >NO BUILDING REQUESTED</option>"
			                             : "          <option value=\"NO BUILDING REQUESTED\" >NO BUILDING REQUESTED</option>");

		            Output.WriteLine("        </select>");
		            Output.WriteLine("      </td>");
					Output.WriteLine("      <td><button title=\"Set new builder status\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_builder_status();\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td>");
		            Output.WriteLine("    </tr>");
	            }
	            Output.WriteLine("  </table>");
				Output.WriteLine();


                Output.WriteLine("  <h2>Recent Incoming Logs</h2>");

				string logDirectory = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + "extra\\logs";
                if (Directory.Exists(logDirectory))
                {

                    string[] logFiles = Directory.GetFiles(logDirectory, "incoming*.html");

                    if (logFiles.Length > 0)
                    {
                        Output.WriteLine("  <p>Select a date below to view the recent incoming resources log file:</p>");
                        Output.WriteLine("  <ul class=\"sbkBav_List\">");

                        for (int i = logFiles.Length - 1; i >= 0; i--)
                        {
                            string logFile = logFiles[i];
                            string logName = (new FileInfo(logFile)).Name;
                            string date_string = logName.ToLower().Replace("incoming_", "").Replace(".html", "");
                            if (date_string.Length == 10)
                            {
                                DateTime date = new DateTime(Convert.ToInt32(date_string.Substring(0, 4)), Convert.ToInt32(date_string.Substring(5, 2)), Convert.ToInt32(date_string.Substring(8)));
                                Output.WriteLine("    <li><a href=\"" + logURL + logName + "\">" + date.ToLongDateString() + "</a></li>");
                            }
                        }
                        Output.WriteLine("  </ul>");
                    }
                    else
                    {
                        Output.WriteLine("  <p>No builder logs found.</p>");
                        Output.WriteLine("  <br />");
                    }
                }
                else
                {
                    Output.WriteLine("  <p>No builder logs found.</p>");
                    Output.WriteLine("  <br />");
                }
            }

			Output.WriteLine();
            Output.WriteLine("  <h2>Related Links</h2>");
			Output.WriteLine("  <ul class=\"sbkBav_List\">");
            Output.WriteLine("      <li><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/internal/new\">Newly added or updated items</a></li>");
            Output.WriteLine("      <li><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/internal/failures\">Failed packages or builder errors</a></li>");
            Output.WriteLine("  </ul>");
            Output.WriteLine("</div>");

        }
    }
}



