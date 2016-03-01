// HTML5 - 10/14

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Core.Client;
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
        private int page;

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

            page = 1;
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
                }
            }
        }

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner, HtmlSubwriter_Behaviors_Enum.Use_Jquery_DataTables }; }
        }

        /// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
        /// <value> Returns 'container-inner1215' always. </value>
	    public override string Container_CssClass
	    {
	        get
	        {
                return "container-inner1215"; 
	            
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

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override bool Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
           // Output.WriteLine("   <script type = \"text/javascript\" src=\"" + Static_Resources.Chart_Js + "\"></script>");

            // Add the code for the calendar pop-up if it may be required
            if (page == 2)
            {
                Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"" + Static_Resources.Jsdatepick_Ltr_Css + "\" />");
                Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources.Jsdatepick_Min_1_3_Js + "\"></script>");
            }

            return false;
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

            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "XyzzyXyzzy";
            string url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;

            const string TAB1_TITLE = "STATUS";
            const string TAB2_TITLE = "BUILDER LOGS";
            const string TAB3_TITLE = "SCHEDULED TASKS";

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
                    add_builder_logs(Output, base_url, Tracer);
                    break;

                case 3:
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

        public void add_builder_logs(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {
            // Get the base url
            string baseURL = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("  <p>Below you can view the list of builder logs.</p>");

            // Add the filter boxes
            Output.WriteLine("  <p>Use the boxes below to filter the results to only show a subset.</p>");
            Output.WriteLine("  <div id=\"sbkWcav_FilterPanel\">");

            // Get the browse info mode, and also the redirect url without the mode information
            string currentInfoBrowseMode = RequestSpecificValues.Current_Mode.Info_Browse_Mode;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;
            string redirect_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = currentInfoBrowseMode;


            // Get the two dates
            DateTime? date1 = null;
            DateTime? date2 = null;
            if (!String.IsNullOrEmpty(currentInfoBrowseMode))
            {
                try
                {
                    string[] splitter = currentInfoBrowseMode.Split("-".ToCharArray());
                    if (splitter.Length == 3)
                    {
                        int year1;
                        Int32.TryParse(splitter[2], out year1);
                        if (year1 < 100)
                            year1 = 2000 + year1;
                        date1 = new DateTime(year1, Convert.ToInt32(splitter[0]), Convert.ToInt32(splitter[1]));
                    }
                    else if (splitter.Length == 6)
                    {
                        int year1;
                        Int32.TryParse(splitter[2], out year1);
                        if (year1 < 100)
                            year1 = 2000 + year1;
                        int year2;
                        Int32.TryParse(splitter[5], out year2);
                        if (year2 < 100)
                            year2 = 2000 + year2;
                        date1 = new DateTime(year1, Convert.ToInt32(splitter[0]), Convert.ToInt32(splitter[1]));
                        date2 = new DateTime(year2, Convert.ToInt32(splitter[3]), Convert.ToInt32(splitter[4]));

                        if (date1.Value.CompareTo(date2.Value) > 0)
                        {
                            DateTime? tempDate = date1;
                            date1 = date2;
                            date2 = tempDate;
                        }

                        DateTime modifiedDate = date2.Value.AddDays(1);
                    }

                }
                catch (Exception)
                {
                    // If the parsing of the date from the URL fails, no item count information is pulled from the database
                }
            }

            Output.WriteLine("  <script type=\"text/javascript\">");
            Output.WriteLine("    window.onload = function() { ");
            if (date1.HasValue)
            {
                Output.WriteLine("      new JsDatePick({ useMode:2, target:\"date1input\", target_cssClass:\"sbkShw_smallinput\", launcher:\"calendar1img\", dateFormat: \"%n/%j/%Y\", imgPath: \"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/datepicker/\", selectedDate:{ year:" + date1.Value.Year + ", month:" + date1.Value.Month + ", day:" + date1.Value.Day + "	} 	});");
            }
            else
            {
                Output.WriteLine("      new JsDatePick({ useMode:2, target:\"date1input\", target_cssClass:\"sbkShw_smallinput\", launcher:\"calendar1img\", dateFormat: \"%n/%j/%Y\", imgPath: \"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/datepicker/\", selectedDate:{ year:" + DateTime.Now.Year + ", month:" + DateTime.Now.Month + ", day:" + DateTime.Now.Day + "	} 	});");
            }

            if (date2.HasValue)
            {
                Output.WriteLine("      new JsDatePick({ useMode:2, target:\"date2input\", target_cssClass:\"sbkShw_smallinput\", launcher:\"calendar2img\", dateFormat: \"%n/%j/%Y\", imgPath: \"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/datepicker/\", selectedDate:{ year:" + date2.Value.Year + ", month:" + date2.Value.Month + ", day:" + date2.Value.Day + "	} 	});");
            }
            else
            {
                Output.WriteLine("      new JsDatePick({ useMode:2, target:\"date2input\", target_cssClass:\"sbkShw_smallinput\", launcher:\"calendar2img\", dateFormat: \"%n/%j/%Y\", imgPath: \"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/datepicker/\", selectedDate:{ year:" + DateTime.Now.Year + ", month:" + DateTime.Now.Month + ", day:" + DateTime.Now.Day + "	} 	});");
            }
            Output.WriteLine("    }; ");
            Output.WriteLine("  </script>");

            Output.WriteLine("  <form name=\"log_filter_form\" id=\"addedForm\">");
            Output.WriteLine("    <table id=\"sbkBav_LogFilterTable\">");
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <th>Filter by Date:</th>");
            Output.WriteLine("        <td>From:</td>");
            if (date1.HasValue)
            {
                Output.WriteLine("        <td><input type=\"text\" name=\"date1input\" id=\"date1input\" class=\"sbkShw_smallinput\" value=\"" + date1.Value.ToShortDateString() + "\" onblur=\"textbox_leave_default_value(this, 'sbkShw_smallinput','mm/dd/yyyy');\" /></td>");
            }
            else
            {
                Output.WriteLine("        <td><input type=\"text\" name=\"date1input\" id=\"date1input\" class=\"sbkShw_smallinput_initial\" value=\"mm/dd/yyyy\" onblur=\"textbox_leave_default_value(this, 'sbkShw_smallinput','mm/dd/yyyy');\" /></td>");
            }
            Output.WriteLine("        <td style=\"width:50px;\"><img src=\"" + Static_Resources.Calendar_Button_Img + "\" title=\"Show a calendar to select this date\"  onclick=\"return false;\" name=\"calendar1img\" ID=\"calendar1img\" class=\"calendar_button\" /></td>");
            Output.WriteLine("        <td>To:</td>");
            if (date2.HasValue)
            {
                Output.WriteLine("        <td><input type=\"text\" name=\"date2input\" id=\"date2input\" class=\"sbkShw_smallinput\" value=\"" + date2.Value.ToShortDateString() + "\" onblur=\"textbox_leave_default_value(this, 'sbkShw_smallinput','mm/dd/yyyy');\" /></td>");
            }
            else
            {
                Output.WriteLine("        <td><input type=\"text\" name=\"date2input\" id=\"date2input\" class=\"sbkShw_smallinput_initial\" value=\"mm/dd/yyyy\" onblur=\"textbox_leave_default_value(this, 'sbkShw_smallinput','mm/dd/yyyy');\" /></td>");
            }
            Output.WriteLine("        <td><img src=\"" + Static_Resources.Calendar_Button_Img + "\" title=\"Show a calendar to select this date\" onclick=\"return false;\" name=\"calendar2img\" ID=\"calendar2img\" class=\"calendar_button\" /></td>");
            Output.WriteLine("      </tr>");
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <th>Filter by BibID/VID:</th>");
            Output.WriteLine("        <td colspan=\"6\"><input type=\"text\" id=\"bibVidFilter\" name=\"bibVidFilter\" class=\"sbkBav_LogFilterBox\"></td>");
            Output.WriteLine("      </tr>");
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td colspan=\"6\" id=\"sbkBav_LogFilterInstructions\">To change the dates shown or set a filter, choose your dates above and hit the GO button.</td>");
            Output.WriteLine("        <td>");
            Output.WriteLine("          <button title=\"Select Range\" class=\"roundbutton\" onclick=\"arbitrary_item_count('" + redirect_url + "'); return false;\">GO <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class\"roundbutton_img_right\" alt=\"\" /></button>");
            Output.WriteLine("        </td>");
            Output.WriteLine("      </tr>");
            Output.WriteLine("    </table>");
            Output.WriteLine("  </form>");
            Output.WriteLine();


            Output.WriteLine("  </div>");
            Output.WriteLine();

            Output.WriteLine("  <table id=\"sbkWcav_MainTable\" class=\"sbkWcav_Table display\">");
            Output.WriteLine("    <thead>");
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <th>Log Date</th>");
            Output.WriteLine("        <th>BibID : VID</th>");
            Output.WriteLine("        <th>Log Type</th>");
            Output.WriteLine("        <th>Log Message</th>");
            Output.WriteLine("      </tr>");
            Output.WriteLine("    </thead>");
            Output.WriteLine("    <tbody>");
            Output.WriteLine("      <tr><td colspan=\"4\" class=\"dataTables_empty\">Loading data from server</td></tr>");
            Output.WriteLine("    </tbody>");
            Output.WriteLine("  </table>");

            Output.WriteLine();
            Output.WriteLine("<script type=\"text/javascript\">");
            Output.WriteLine("  $(document).ready(function() {");
            Output.WriteLine("     var shifted=false;");
            Output.WriteLine("     $(document).on('keydown', function(e){shifted = e.shiftKey;} );");
            Output.WriteLine("     $(document).on('keyup', function(e){shifted = false;} );");

            Output.WriteLine();
            Output.WriteLine("      var oTable = $('#sbkWcav_MainTable').dataTable({");
            Output.WriteLine("           \"lengthMenu\": [ [50, 100, 500, 1000, -1], [50, 100, 500, 1000, \"All\"] ],");
            Output.WriteLine("           \"pageLength\": 50,");
            //Output.WriteLine("           \"bFilter\": false,");
            Output.WriteLine("           \"processing\": true,");
            Output.WriteLine("           \"serverSide\": true,");
            Output.WriteLine("           \"sDom\": \"lprtip\",");

            // Determine the URL for the results
            string data_source_url = SobekEngineClient.Builder.Get_Builder_Logs_JDataTable_URL;

            //// Add any query string (should probably use StringBuilder, but this should be fairly seldomly used very deeply)
            //if (!String.IsNullOrEmpty(userFilter))
            //{
            //    data_source_url = data_source_url + "?user=" + userFilter;
            //}
            //else if (!String.IsNullOrEmpty(level1))
            //{
            //    data_source_url = data_source_url + "?l1=" + level1;
            //    if (!String.IsNullOrEmpty(level2))
            //    {
            //        data_source_url = data_source_url + "&l2=" + level2;
            //        if (!String.IsNullOrEmpty(level3))
            //        {
            //            data_source_url = data_source_url + "&l3=" + level3;
            //            if (!String.IsNullOrEmpty(level4))
            //            {
            //                data_source_url = data_source_url + "&l4=" + level4;
            //                if (!String.IsNullOrEmpty(level5))
            //                {
            //                    data_source_url = data_source_url + "&l5=" + level5;
            //                }
            //            }
            //        }
            //    }
            //}

            Output.WriteLine("           \"sAjaxSource\": \"" + data_source_url + "\",");
            Output.WriteLine("           \"aoColumns\": [ null, null, null, null ]  });");
            Output.WriteLine();

            Output.WriteLine("  });");
            Output.WriteLine("</script>");
            Output.WriteLine();

            Output.WriteLine();
        }

        public void add_builder_scheduled_tasks(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {

        }
    }
}



