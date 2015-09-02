using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an administrator to view overall usage across all web content pages </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class. </remarks>
    public class WebContent_Usage_AdminViewer : abstract_AdminViewer
    {
        private string actionMessage;
        private readonly string level1;
        private readonly string level2;
        private readonly string level3;
        private readonly string level4;
        private readonly string level5;
        private readonly int month1;
        private readonly int year1;
        private readonly int month2;
        private readonly int year2;


        /// <summary> Constructor for a new instance of the WebContent_Usage_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public WebContent_Usage_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("WebContent_Usage_AdminViewer.Constructor", String.Empty);
            actionMessage = String.Empty;


            // Ensure the user is the system admin or portal admin
            if ((RequestSpecificValues.Current_User == null) || ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin)))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            //// If this is posted back, look for the reset
            //if (RequestSpecificValues.Current_Mode.isPostBack)
            //{
            //    string reset_value = HttpContext.Current.Request.Form[""];
            //    if ((!String.IsNullOrEmpty(reset_value)) && (reset_value == "reset"))
            //    {
            //        // Just ensure everything is emptied out
            //        HttpContext.Current.Cache.Remove("GlobalPermissionsReport");
            //        HttpContext.Current.Cache.Remove("GlobalPermissionsUsersLinked");
            //        HttpContext.Current.Cache.Remove("GlobalPermissionsLinkedAggr");
            //        HttpContext.Current.Cache.Remove("GlobalPermissionsReportSubmit");
            //    }
            //}

            // Set filters initially to empty strings
            level1 = String.Empty;
            level2 = String.Empty;
            level3 = String.Empty;
            level4 = String.Empty;
            level5 = String.Empty;
            month1 = -1;
            year1 = -1;
            month2 = -1;
            year2 = -1;

            // Get any level filter information from the query string
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["l1"]))
            {
                level1 = HttpContext.Current.Request.QueryString["l1"];

                if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["l2"]))
                {
                    level2 = HttpContext.Current.Request.QueryString["l2"];

                    if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["l3"]))
                    {
                        level3 = HttpContext.Current.Request.QueryString["l3"];

                        if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["l4"]))
                        {
                            level4 = HttpContext.Current.Request.QueryString["l4"];

                            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["l5"]))
                            {
                                level5 = HttpContext.Current.Request.QueryString["l5"];
                            }
                        }
                    }
                }
            }

            // Get the year and month filters
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["d1"]))
            {
                string date1 = HttpContext.Current.Request.QueryString["d1"];
                if (date1.Length == 6)
                {
                    int year1possible;
                    int month1possible;
                    if ((Int32.TryParse(date1.Substring(0, 4), out year1possible)) && (Int32.TryParse(date1.Substring(4), out month1possible)))
                    {
                        year1 = year1possible;
                        month1 = month1possible;
                    }
                }
            }
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["d2"]))
            {
                string date2 = HttpContext.Current.Request.QueryString["d2"];
                if (date2.Length == 6)
                {
                    int year2possible;
                    int month2possible;
                    if ((Int32.TryParse(date2.Substring(0, 4), out year2possible)) && (Int32.TryParse(date2.Substring(4), out month2possible)))
                    {
                        year2 = year2possible;
                        month2 = month2possible;
                    }
                }
            }

            // If the end year is filled out, but not the month, set to the end of that year
            if ((year2 > 1900) && ( month2 < 1 ) || ( month2 > 12 ))
            {
                month2 = (year2 == DateTime.Now.Year) ? DateTime.Now.Month : 12;
            }

            // If no final year/month, set it to now as well
            if (year2 < 1900)
            {
                year2 = UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Year;
                month2 = UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Month;
            }

            // If no initial year/month, use the first stats date
            if ((year1 < 1900) || ( year1 < UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Year ))
            {
                year1 = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Year;
                month1 = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Month;
            }

            if ((month1 < 1) || (month1 > 12))
            {
                month1 = 1;
                if (year1 == UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Year)
                    month1 = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Month;
            }
        }

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner, HtmlSubwriter_Behaviors_Enum.Use_Jquery_DataTables }; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Web Content Usage Reports' </value>
        public override string Web_Title
        {
            get { return "Web Content Usage Reports"; }
        }

        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.WebContent_Usage_Img; }
        }


        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("WebContent_Usage_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("WebContent_Usage_AdminViewer.Write_ItemNavForm_Closing", "");

            Output.WriteLine("<!-- WebContent_Usage_AdminViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");

            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            
            

            if (actionMessage.Length > 0)
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

            

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs sbkAdm_HomeTabs\">");
            Output.WriteLine("  <div class=\"tabs\">");
            Output.WriteLine("    <ul>");

            string tab1_title = "USAGE";
            Output.WriteLine("      <li class=\"tabActiveHeader\"> " + tab1_title + " </li>");
           
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

            // Determine the two URLS needed (one for the GO button, and another for the jQuery datatable results)
            StringBuilder script_builder = new StringBuilder(UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode));
            StringBuilder results_builder = new StringBuilder(SobekEngineClient.WebContent.Get_Global_Usage_Report_JDataTable_URL);

            // Get the base URL without any filtering
            string filterUrl = script_builder + "?d1=" + year1 + month1.ToString().PadLeft(2, '0') + "&d2=" + year2 + month2.ToString().PadLeft(2, '0');

            // Add the month/year query stuff to the results
            results_builder.Append("?year1=" + year1 + "&month1=" + month1.ToString().PadLeft(2, '0') + "&year2=" + year2 + "&month2=" + month2.ToString().PadLeft(2, '0'));

            // Add any query string related to the filter levels
            if (!String.IsNullOrEmpty(level1))
            {
                script_builder.Append("?l1=" + level1);
                results_builder.Append("&l1=" + level1);
                if (!String.IsNullOrEmpty(level2))
                {
                    script_builder.Append("&l2=" + level2);
                    results_builder.Append("&l2=" + level2);
                    if (!String.IsNullOrEmpty(level3))
                    {
                        script_builder.Append("&l3=" + level3);
                        results_builder.Append("&l3=" + level3);
                        if (!String.IsNullOrEmpty(level4))
                        {
                            script_builder.Append("&l4=" + level4);
                            results_builder.Append("&l4=" + level4);
                            if (!String.IsNullOrEmpty(level5))
                            {
                                script_builder.Append("&l5=" + level5);
                                results_builder.Append("&l5=" + level5);
                            }
                        }
                    }
                }
            }

            // Get the URLS from the string builders
            string goUrl = script_builder.ToString();
            string dataUrl = results_builder.ToString();






            // If there are none whatsoever, show  a special message and don't bother with the table
            if (!SobekEngineClient.WebContent.Has_Global_Usage(Tracer))
            {
                Output.WriteLine("<div id=\"sbkWchs_NoDataMsg\">No usage statistics collected</div>");
            }
            else
            {
                Output.WriteLine("  <p>Usage statistics for the web content pages over time appears below.");
                Output.WriteLine("     Views are the number of times this page was requested.  Hierarchical views includes the hits on this page, as well as all the child pages.");
                Output.WriteLine("     Usage statistics are collected from the web logs by the SobekCM builder in a regular, monthly automated process.</p>");


                Output.WriteLine("  <p>The usage for the web content pages appears below for the following data range:</p>");

                Output.WriteLine("  <div id=\"sbkWcuav_DatePanel\">");
                Output.WriteLine("    From: <select id=\"date1_selector\" class=\"SobekStatsDateSelector\" >");

                int select_month = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Month;
                int select_year = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Year;
                while ((select_month != UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Month) || (select_year != UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Year))
                {
                    if ((month1 == select_month) && (year1 == select_year))
                    {
                        Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\" selected=\"selected\" >" + Month_From_Int(select_month) + " " + select_year + "</option>");
                    }
                    else
                    {
                        Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\">" + Month_From_Int(select_month) + " " + select_year + "</option>");
                    }

                    select_month++;
                    if (select_month > 12)
                    {
                        select_month = 1;
                        select_year++;
                    }
                }
                if ((month1 == select_month) && (year1 == select_year))
                {
                    Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\" selected=\"selected\" >" + Month_From_Int(select_month) + " " + select_year + "</option>");
                }
                else
                {
                    Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\">" + Month_From_Int(select_month) + " " + select_year + "</option>");
                }
                Output.WriteLine("    </select>");
                Output.WriteLine("    &nbsp; &nbsp;");
                Output.WriteLine("    To: <select id=\"date2_selector\" class=\"SobekStatsDateSelector\" >");

                select_month = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Month;
                select_year = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Year;
                while ((select_month != UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Month) || (select_year != UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Year))
                {
                    if ((month2 == select_month) && (year2 == select_year))
                    {
                        Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\" selected=\"selected\" >" + Month_From_Int(select_month) + " " + select_year + "</option>");
                    }
                    else
                    {
                        Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\">" + Month_From_Int(select_month) + " " + select_year + "</option>");
                    }

                    select_month++;
                    if (select_month > 12)
                    {
                        select_month = 1;
                        select_year++;
                    }
                }
                if ((month2 == select_month) && (year2 == select_year))
                {
                    Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\" selected=\"selected\" >" + Month_From_Int(select_month) + " " + select_year + "</option>");
                }
                else
                {
                    Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\">" + Month_From_Int(select_month) + " " + select_year + "</option>");
                }

                Output.WriteLine("    </select>");
                Output.WriteLine("    &nbsp; &nbsp;");
                Output.WriteLine("    <button title=\"Select Range\" class=\"sbkShw_RoundButton\" onclick=\"date_jump_sobekcm('" + goUrl + "'); return false;\">GO <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class\"roundbutton_img_right\" alt=\"\" /></button>");
                Output.WriteLine("  </div>");


                // Add the filter boxes
                Output.WriteLine("  <p>Use the boxes below to filter the results to only show a subset.</p>");
                Output.WriteLine("  <div id=\"sbkWcuav_FilterPanel\">");

                Output.WriteLine("    Filter by URL: ");
                Output.WriteLine("    <select id=\"lvl1Filter\" name=\"lvl1Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + filterUrl + "',1);\">");
                if (String.IsNullOrEmpty(level1))
                    Output.WriteLine("      <option value=\"\" selected=\"selected\"></option>");
                else
                    Output.WriteLine("      <option value=\"\"></option>");

                List<string> level1Options = SobekEngineClient.WebContent.Get_Global_Recent_Updates_NextLevel(Tracer);
                foreach (string thisOption in level1Options)
                {
                    if (String.Compare(level1, thisOption, StringComparison.OrdinalIgnoreCase) == 0)
                        Output.WriteLine("      <option value=\"" + thisOption + "\" selected=\"selected\">" + thisOption + "</option>");
                    else
                        Output.WriteLine("      <option value=\"" + thisOption + "\">" + thisOption + "</option>");
                }
                Output.WriteLine("    </select>");

                // Should the second level be shown?
                if (!String.IsNullOrEmpty(level1))
                {
                    List<string> level2Options = SobekEngineClient.WebContent.Get_Global_Recent_Updates_NextLevel(Tracer, level1);
                    if (level2Options.Count > 0)
                    {
                        Output.WriteLine("    /");
                        Output.WriteLine("    <select id=\"lvl2Filter\" name=\"lvl2Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + filterUrl + "',2);\">");
                        if (String.IsNullOrEmpty(level2))
                            Output.WriteLine("      <option value=\"\" selected=\"selected\"></option>");
                        else
                            Output.WriteLine("      <option value=\"\"></option>");


                        foreach (string thisOption in level2Options)
                        {
                            if (String.Compare(level2, thisOption, StringComparison.OrdinalIgnoreCase) == 0)
                                Output.WriteLine("      <option value=\"" + thisOption + "\" selected=\"selected\">" + thisOption + "</option>");
                            else
                                Output.WriteLine("      <option value=\"" + thisOption + "\">" + thisOption + "</option>");
                        }
                        Output.WriteLine("    </select>");

                        // Should the third level be shown?
                        if (!String.IsNullOrEmpty(level2))
                        {
                            List<string> level3Options = SobekEngineClient.WebContent.Get_Global_Recent_Updates_NextLevel(Tracer, level1, level2);
                            if (level3Options.Count > 0)
                            {
                                Output.WriteLine("    /");
                                Output.WriteLine("    <select id=\"lvl3Filter\" name=\"lvl3Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + filterUrl + "',3);\">");
                                if (String.IsNullOrEmpty(level3))
                                    Output.WriteLine("      <option value=\"\" selected=\"selected\"></option>");
                                else
                                    Output.WriteLine("      <option value=\"\"></option>");


                                foreach (string thisOption in level3Options)
                                {
                                    if (String.Compare(level3, thisOption, StringComparison.OrdinalIgnoreCase) == 0)
                                        Output.WriteLine("      <option value=\"" + thisOption + "\" selected=\"selected\">" + thisOption + "</option>");
                                    else
                                        Output.WriteLine("      <option value=\"" + thisOption + "\">" + thisOption + "</option>");
                                }
                                Output.WriteLine("    </select>");

                                // Should the fourth level be shown?
                                if (!String.IsNullOrEmpty(level3))
                                {
                                    List<string> level4Options = SobekEngineClient.WebContent.Get_Global_Recent_Updates_NextLevel(Tracer, level1, level2, level3);
                                    if (level4Options.Count > 0)
                                    {
                                        Output.WriteLine("    /");
                                        Output.WriteLine("    <select id=\"lvl4Filter\" name=\"lvl4Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + filterUrl + "',4);\">");
                                        if (String.IsNullOrEmpty(level4))
                                            Output.WriteLine("      <option value=\"\" selected=\"selected\"></option>");
                                        else
                                            Output.WriteLine("      <option value=\"\"></option>");


                                        foreach (string thisOption in level4Options)
                                        {
                                            if (String.Compare(level4, thisOption, StringComparison.OrdinalIgnoreCase) == 0)
                                                Output.WriteLine("      <option value=\"" + thisOption + "\" selected=\"selected\">" + thisOption + "</option>");
                                            else
                                                Output.WriteLine("      <option value=\"" + thisOption + "\">" + thisOption + "</option>");
                                        }
                                        Output.WriteLine("    </select>");


                                        // Should the fifth level be shown?
                                        if (!String.IsNullOrEmpty(level4))
                                        {
                                            List<string> level5Options = SobekEngineClient.WebContent.Get_Global_Recent_Updates_NextLevel(Tracer, level1, level2, level3, level4);
                                            if (level5Options.Count > 0)
                                            {
                                                Output.WriteLine("    /");
                                                Output.WriteLine("    <select id=\"lvl5Filter\" name=\"lvl5Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + filterUrl + "',5);\">");
                                                if (String.IsNullOrEmpty(level5))
                                                    Output.WriteLine("      <option value=\"\" selected=\"selected\"></option>");
                                                else
                                                    Output.WriteLine("      <option value=\"\"></option>");


                                                foreach (string thisOption in level5Options)
                                                {
                                                    if (String.Compare(level5, thisOption, StringComparison.OrdinalIgnoreCase) == 0)
                                                        Output.WriteLine("      <option value=\"" + thisOption + "\" selected=\"selected\">" + thisOption + "</option>");
                                                    else
                                                        Output.WriteLine("      <option value=\"" + thisOption + "\">" + thisOption + "</option>");
                                                }
                                                Output.WriteLine("    </select>");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Output.WriteLine("  </div>");
                Output.WriteLine();

                Output.WriteLine("  <table id=\"sbkWcav_MainTable\" class=\"sbkWcuav_Table display\">");
                Output.WriteLine("    <thead>");
                Output.WriteLine("      <tr>");
                Output.WriteLine("        <th>URL</th>");
                Output.WriteLine("        <th>Title</th>");
                Output.WriteLine("        <th>Hits</th>");
                Output.WriteLine("        <th>HitsHierarchical</th>");
                Output.WriteLine("      </tr>");
                Output.WriteLine("    </thead>");
                Output.WriteLine("    <tbody>");
                Output.WriteLine("      <tr><td colspan=\"5\" class=\"dataTables_empty\">Loading data from server</td></tr>");
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




                Output.WriteLine("           \"sAjaxSource\": \"" + dataUrl + "\",");
                Output.WriteLine("           \"aoColumns\": [ null, null, null, null ]  });");
                Output.WriteLine();

                Output.WriteLine("     $('#sbkWcav_MainTable tbody').on( 'click', 'tr', function () {");
                Output.WriteLine("          var aData = oTable.fnGetData( this );");
                Output.WriteLine("          var iId = aData[1];");
                Output.WriteLine("          if ( shifted == true )");
                Output.WriteLine("          {");
                Output.WriteLine("             window.open('" + RequestSpecificValues.Current_Mode.Base_URL + "' + iId);");
                Output.WriteLine("             shifted=false;");
                Output.WriteLine("          }");
                Output.WriteLine("          else");
                Output.WriteLine("             window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + "' + iId;");
                Output.WriteLine("     });");
                Output.WriteLine("  });");
                Output.WriteLine("</script>");
                Output.WriteLine();
                //// Add the buttons
                //RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                //Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
                //Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                //Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
                //Output.WriteLine("  </div>");

                Output.WriteLine();
            }

            Output.WriteLine("</div>");
            Output.WriteLine("</div>");
            Output.WriteLine("</div>");

            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
        }

        private static string Month_From_Int(int Month_Int)
        {
            string monthString1 = "Invalid";
            switch (Month_Int)
            {
                case 1:
                    monthString1 = "January";
                    break;

                case 2:
                    monthString1 = "February";
                    break;

                case 3:
                    monthString1 = "March";
                    break;

                case 4:
                    monthString1 = "April";
                    break;

                case 5:
                    monthString1 = "May";
                    break;

                case 6:
                    monthString1 = "June";
                    break;

                case 7:
                    monthString1 = "July";
                    break;

                case 8:
                    monthString1 = "August";
                    break;

                case 9:
                    monthString1 = "September";
                    break;

                case 10:
                    monthString1 = "October";
                    break;

                case 11:
                    monthString1 = "November";
                    break;

                case 12:
                    monthString1 = "December";
                    break;
            }
            return monthString1;
        }
    }
}
