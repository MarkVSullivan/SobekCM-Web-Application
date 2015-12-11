// HTML5 - 10/14

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
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
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer allow the system administrator to view and
    /// update the status of the SobekCM builder application.</li>
    /// </ul></remarks>
    public class Builder_AdminViewer : abstract_AdminViewer
    {
        private string actionMessage;

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
                if (((RequestSpecificValues.Current_User.Is_System_Admin) && (!UI_ApplicationCache_Gateway.Settings.Servers.isHosted)) ||
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

            Output.WriteLine("<!-- WebContent_Mgmt_AdminViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");

            int page = 1;
            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.My_Sobek_SubMode))
            {
                switch (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.ToLower())
                {
                    case "b":
                        page = 2;
                        break;

                    case "c":
                        page = 3;
                        break;

                    case "d":
                        page = 4;
                        break;

                    case "e":
                        page = 5;
                        break;
                }
            }



            // Show any action message
            if (!String.IsNullOrEmpty( actionMessage))
            {
                Output.WriteLine("  <div class=\"sbkAdm_HomeText\">");
                Output.WriteLine("  <br />");
                if (actionMessage.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageError\">" + actionMessage + "</div>");
                }
                else
                {

                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageSuccess\">" + actionMessage + "</div>");
                }
                Output.WriteLine("  <br />");
                Output.WriteLine("  </div>");
            }

            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.WebContent_Add_New;
            string wizard_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Builder_Status;

            Output.WriteLine("  <table style=\"margin-left: 50px;\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:500px; text-align: left;\">");
            Output.WriteLine("        <p>Press the button to the right to add a new web content page or redirect, or just type the URL for the new web content page. </p>");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style=\"padding-left: 30px;\">");
            Output.WriteLine("        <button title=\"Add a new web content page or redirect\" class=\"sbkAdm_RoundButton\" style=\"padding: 6px;width: 190px;\" onclick=\"window.location.href='" + wizard_url + "';return false;\"> &nbsp; ADD NEW &nbsp; <br />PAGE OR REDIRECT</button>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs sbkAdm_HomeTabs\">");
            Output.WriteLine("  <div class=\"tabs\">");
            Output.WriteLine("    <ul>");


            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "XyzzyXyzzy";
            string url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;

            const string TAB1_TITLE = "STATUS";
            const string TAB2_TITLE = "SETTINGS";
            const string TAB3_TITLE = "INCOMING FOLDERS"; 
            const string TAB4_TITLE = "MODULES";
            const string TAB5_TITLE = "SCHEDULED TASKS";

            if (page == 1)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> " + TAB1_TITLE + " </li>");
            }
            else
            {
                Output.WriteLine("      <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "a") + "';return false;\"> " + TAB1_TITLE + " </li>");
            }

            if (page == 2)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> " + TAB2_TITLE + " </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "b") + "';return false;\"> " + TAB2_TITLE + " </li>");
            }

            if (page == 3)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> " + TAB3_TITLE + " </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "c") + "';return false;\"> " + TAB3_TITLE + " </li>");
            }

            if (page == 4)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> " + TAB4_TITLE + " </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "d") + "';return false;\"> " + TAB4_TITLE + " </li>");
            }

            if (page == 5)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> " + TAB5_TITLE + " </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "e") + "';return false;\"> " + TAB5_TITLE + " </li>");
            }


            Output.WriteLine("    </ul>");
            Output.WriteLine("  </div>");

            Output.WriteLine("    <div class=\"tabscontent\">");
            Output.WriteLine("    	<div class=\"sbkUgav_TabPage\" id=\"tabpage_1\">");

            //// Add the buttons
            //Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            //Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            //Output.WriteLine("    <button title=\"Save changes to this user group\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            //Output.WriteLine("  </div>");
            //Output.WriteLine();


            Output.WriteLine();

            // Get the base url
            string base_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            switch (page)
            {
                case 1:
                    add_builder_status(Output, base_url, Tracer);
                    break;

                case 2:
                    add_builder_settings(Output, base_url, Tracer);
                    break;

                case 3:
                    add_builder_folders(Output, base_url, Tracer);
                    break;

                case 4:
                    add_builder_modules(Output, base_url, Tracer);
                    break;

                case 5:
                    add_builder_scheduled_tasks(Output, base_url, Tracer);
                    break;
            }


            //// Add the buttons
            //RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            //Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            //Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            //Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            //Output.WriteLine("  </div>");

            Output.WriteLine();

            Output.WriteLine("</div>");
            Output.WriteLine("</div>");
            Output.WriteLine("</div>");

            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
        }

        public void add_builder_status(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {
            // Pull the builder settings
            Dictionary<string, string> builderSettings = SobekCM_Database.Get_Settings(Tracer);

            Output.WriteLine("<!-- Hidden field to keep the newly requested status -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_builder_tosave\" name=\"admin_builder_tosave\" value=\"\" />");
            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");

            // Start to show the text 
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

            Output.WriteLine("  <p>The SobekCM builder is constantly loading new items and updates and building the full text indexes.  This page can be used to view and update the current status as well as view the most recent log files.</p>");
            Output.WriteLine("  <p>For more information about the builder and possible actions from this screen, <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/builder\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");
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

                if (((RequestSpecificValues.Current_User.Is_System_Admin) && ( !UI_ApplicationCache_Gateway.Settings.Servers.isHosted )) ||
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

            }

			Output.WriteLine();
            Output.WriteLine("  <h2>Related Links</h2>");
			Output.WriteLine("  <ul class=\"sbkBav_List\">");
            Output.WriteLine("      <li><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/internal/new\">Newly added or updated items</a></li>");
            Output.WriteLine("      <li><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/internal/failures\">Failed packages or builder errors</a></li>");
            Output.WriteLine("  </ul>");
            Output.WriteLine("</div>");

        }

        public void add_builder_settings(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {

        }

        public void add_builder_folders(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {

        }

        public void add_builder_modules(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {

        }

        public void add_builder_scheduled_tasks(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {

        }
    }
}



