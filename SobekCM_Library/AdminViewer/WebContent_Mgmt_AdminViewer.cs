using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Admin viewer lists all of the top-level static web content pages, as well as all the related
    /// top-level redirects within the system </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class. </remarks>
    public class WebContent_Mgmt_AdminViewer : abstract_AdminViewer
    {
        private string actionMessage;
        private readonly string level1;
        private readonly string level2;
        private readonly string level3;
        private readonly string level4;
        private readonly string level5;

        /// <summary> Constructor for a new instance of the WebContent_Mgmt_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public WebContent_Mgmt_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("WebContent_Mgmt_AdminViewer.Constructor", String.Empty);
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
        }

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner, HtmlSubwriter_Behaviors_Enum.Use_Jquery_DataTables }; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Web Content Page Management' </value>
        public override string Web_Title
        {
            get { return "Web Content Page Management"; }
        }

        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.WebContent_Img; }
        }


        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("WebContent_Mgmt_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("WebContent_Mgmt_AdminViewer.Write_ItemNavForm_Closing", "");

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
                }
            }

            

            // Show any action message
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


            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "XyzzyXyzzy";
            string url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;

            const string TAB1_TITLE = "PAGES AND REDIRECTS";
            const string TAB2_TITLE = "WEB PAGES ONLY";
            const string TAB3_TITLE = "REDIRECTS ONLY";

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
                    add_all_table(Output, base_url, Tracer);
                    break;

                case 2:
                    add_pages_table(Output, base_url, Tracer);
                    break;

                case 3:
                    add_redirects_table(Output, base_url, Tracer);
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

        private void add_all_table(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {
            // If there are none whatsoever, show  a special message and don't bother with the table
            if (!SobekEngineClient.WebContent.Has_Pages_Or_Redirects(Tracer))
            {
                Output.WriteLine("  <p>There are currently no web content page or redirects within this system.</p>");
                Output.WriteLine("  <p>To add one, enter the URL you wish to add in your browser and the system will offer to add it if you have sufficient rights.</p>");
                return;
            }

            Output.WriteLine("  <p>Below is this list of all the non-aggregation web content pages and redirects within the system.</p>");

            // Add the filter boxes
            Output.WriteLine("  <p>Use the boxes below to filter the results to only show a subset.</p>");
            Output.WriteLine("  <div id=\"sbkWcav_FilterPanel\">");
            Output.WriteLine("    Filter: ");
            Output.WriteLine("    <select id=\"lvl1Filter\" name=\"lvl1Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',1);\">");
            if (String.IsNullOrEmpty(level1))
                Output.WriteLine("      <option value=\"\" selected=\"selected\"></option>");
            else
                Output.WriteLine("      <option value=\"\"></option>");

            List<string> level1Options = SobekEngineClient.WebContent.Get_All_NextLevel(Tracer);
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
                List<string> level2Options = SobekEngineClient.WebContent.Get_All_NextLevel(Tracer, level1);
                if (level2Options.Count > 0)
                {
                    Output.WriteLine("    /");
                    Output.WriteLine("    <select id=\"lvl2Filter\" name=\"lvl2Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',2);\">");
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
                        List<string> level3Options = SobekEngineClient.WebContent.Get_All_NextLevel(Tracer, level1, level2);
                        if (level3Options.Count > 0)
                        {
                            Output.WriteLine("    /");
                            Output.WriteLine("    <select id=\"lvl3Filter\" name=\"lvl3Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',3);\">");
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
                                List<string> level4Options = SobekEngineClient.WebContent.Get_All_NextLevel(Tracer, level1, level2, level3);
                                if (level4Options.Count > 0)
                                {
                                    Output.WriteLine("    /");
                                    Output.WriteLine("    <select id=\"lvl4Filter\" name=\"lvl4Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',4);\">");
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
                                        List<string> level5Options = SobekEngineClient.WebContent.Get_All_NextLevel(Tracer, level1, level2, level3, level4);
                                        if (level5Options.Count > 0)
                                        {
                                            Output.WriteLine("    /");
                                            Output.WriteLine("    <select id=\"lvl5Filter\" name=\"lvl5Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',5);\">");
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

            Output.WriteLine("  <table id=\"sbkWcav_MainTable\" class=\"sbkWcav_Table display\">");
            Output.WriteLine("    <thead>");
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <th>ID</th>");
            Output.WriteLine("        <th>URL</th>");
            Output.WriteLine("        <th>Title</th>");
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

            // Determine the URL for the results
            string redirect_url = SobekEngineClient.WebContent.Get_All_JDataTable_URL;

            // Add any query string (should probably use StringBuilder, but this should be fairly seldomly used very deeply)
            if (!String.IsNullOrEmpty(level1))
            {
                redirect_url = redirect_url + "?l1=" + level1;
                if (!String.IsNullOrEmpty(level2))
                {
                    redirect_url = redirect_url + "&l2=" + level2;
                    if (!String.IsNullOrEmpty(level3))
                    {
                        redirect_url = redirect_url + "&l3=" + level3;
                        if (!String.IsNullOrEmpty(level4))
                        {
                            redirect_url = redirect_url + "&l4=" + level4;
                            if (!String.IsNullOrEmpty(level5))
                            {
                                redirect_url = redirect_url + "&l5=" + level5;
                            }
                        }
                    }
                }
            }

            Output.WriteLine("           \"sAjaxSource\": \"" + redirect_url + "\",");
            Output.WriteLine("           \"aoColumns\": [ { \"bVisible\": false }, null, null ]  });");
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
        }

        private void add_pages_table(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {
            // If there are none whatsoever, show  a special message and don't bother with the table
            if (!SobekEngineClient.WebContent.Has_Content_Pages(Tracer))
            {
                Output.WriteLine("  <p>There are currently no web content pages within this system.</p>");
                Output.WriteLine("  <p>To add one, enter the URL you wish to add in your browser and the system will offer to add it if you have sufficient rights.</p>");
                return;
            }

            Output.WriteLine("  <p>Below is this list of all the non-aggregation web content pages within the system.  This list does not include any redirects.</p>");

            // Add the filter boxes
            Output.WriteLine("  <p>Use the boxes below to filter the results to only show a subset.</p>");
            Output.WriteLine("  <div id=\"sbkWcav_FilterPanel\">");
            Output.WriteLine("    Filter: ");
            Output.WriteLine("    <select id=\"lvl1Filter\" name=\"lvl1Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',1);\">");
            if (String.IsNullOrEmpty(level1))
                Output.WriteLine("      <option value=\"\" selected=\"selected\"></option>");
            else
                Output.WriteLine("      <option value=\"\"></option>");

            List<string> level1Options = SobekEngineClient.WebContent.Get_All_Pages_NextLevel(Tracer);
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
                List<string> level2Options = SobekEngineClient.WebContent.Get_All_Pages_NextLevel(Tracer, level1);
                if (level2Options.Count > 0)
                {
                    Output.WriteLine("    /");
                    Output.WriteLine("    <select id=\"lvl2Filter\" name=\"lvl2Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',2);\">");
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
                        List<string> level3Options = SobekEngineClient.WebContent.Get_All_Pages_NextLevel(Tracer, level1, level2);
                        if (level3Options.Count > 0)
                        {
                            Output.WriteLine("    /");
                            Output.WriteLine("    <select id=\"lvl3Filter\" name=\"lvl3Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',3);\">");
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
                                List<string> level4Options = SobekEngineClient.WebContent.Get_All_Pages_NextLevel(Tracer, level1, level2, level3);
                                if (level4Options.Count > 0)
                                {
                                    Output.WriteLine("    /");
                                    Output.WriteLine("    <select id=\"lvl4Filter\" name=\"lvl4Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',4);\">");
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
                                        List<string> level5Options = SobekEngineClient.WebContent.Get_All_Pages_NextLevel(Tracer, level1, level2, level3, level4);
                                        if (level5Options.Count > 0)
                                        {
                                            Output.WriteLine("    /");
                                            Output.WriteLine("    <select id=\"lvl5Filter\" name=\"lvl5Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',5);\">");
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

            Output.WriteLine("  <table id=\"sbkWcav_MainTable\" class=\"sbkWcav_Table display\">");
            Output.WriteLine("    <thead>");
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <th>ID</th>");
            Output.WriteLine("        <th>URL</th>");
            Output.WriteLine("        <th>Title</th>");
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

            // Determine the URL for the results
            string redirect_url = SobekEngineClient.WebContent.Get_All_Pages_JDataTable_URL;

            // Add any query string (should probably use StringBuilder, but this should be fairly seldomly used very deeply)
            if (!String.IsNullOrEmpty(level1))
            {
                redirect_url = redirect_url + "?l1=" + level1;
                if (!String.IsNullOrEmpty(level2))
                {
                    redirect_url = redirect_url + "&l2=" + level2;
                    if (!String.IsNullOrEmpty(level3))
                    {
                        redirect_url = redirect_url + "&l3=" + level3;
                        if (!String.IsNullOrEmpty(level4))
                        {
                            redirect_url = redirect_url + "&l4=" + level4;
                            if (!String.IsNullOrEmpty(level5))
                            {
                                redirect_url = redirect_url + "&l5=" + level5;
                            }
                        }
                    }
                }
            }

            Output.WriteLine("           \"sAjaxSource\": \"" + redirect_url + "\",");
            Output.WriteLine("           \"aoColumns\": [ { \"bVisible\": false }, null, null ]  });");
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
        }

        private void add_redirects_table(TextWriter Output, string Base_URL, Custom_Tracer Tracer)
        {            
            // If there are none whatsoever, show  a special message and don't bother with the table
            if (!SobekEngineClient.WebContent.Has_Redirects(Tracer))
            {
                Output.WriteLine("  <p>There are currently no global redirects within this system.</p>");
                Output.WriteLine("  <p>To add one, enter the URL you wish to add in your browser and the system will offer to add it if you have sufficient rights.</p>");
                return;
            }

            Output.WriteLine("  <p>Below is this list of the redirects within the system.</p>");

            // Add the filter boxes
            Output.WriteLine("  <p>Use the boxes below to filter the results to only show a subset.</p>");
            Output.WriteLine("  <div id=\"sbkWcav_FilterPanel\">");
            Output.WriteLine("    Filter: ");
            Output.WriteLine("    <select id=\"lvl1Filter\" name=\"lvl1Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',1);\">");
            if (String.IsNullOrEmpty(level1))
                Output.WriteLine("      <option value=\"\" selected=\"selected\"></option>");
            else
                Output.WriteLine("      <option value=\"\"></option>");

            List<string> level1Options = SobekEngineClient.WebContent.Get_All_Redirects_NextLevel(Tracer);
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
                List<string> level2Options = SobekEngineClient.WebContent.Get_All_Redirects_NextLevel(Tracer, level1);
                if (level2Options.Count > 0)
                {
                    Output.WriteLine("    /");
                    Output.WriteLine("    <select id=\"lvl2Filter\" name=\"lvl2Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',2);\">");
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
                        List<string> level3Options = SobekEngineClient.WebContent.Get_All_Redirects_NextLevel(Tracer, level1, level2);
                        if (level3Options.Count > 0)
                        {
                            Output.WriteLine("    /");
                            Output.WriteLine("    <select id=\"lvl3Filter\" name=\"lvl3Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',3);\">");
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
                                List<string> level4Options = SobekEngineClient.WebContent.Get_All_Redirects_NextLevel(Tracer, level1, level2, level3);
                                if (level4Options.Count > 0)
                                {
                                    Output.WriteLine("    /");
                                    Output.WriteLine("    <select id=\"lvl4Filter\" name=\"lvl4Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',4);\">");
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
                                        List<string> level5Options = SobekEngineClient.WebContent.Get_All_Redirects_NextLevel(Tracer, level1, level2, level3, level4);
                                        if (level5Options.Count > 0)
                                        {
                                            Output.WriteLine("    /");
                                            Output.WriteLine("    <select id=\"lvl5Filter\" name=\"lvl5Filter\" class=\"sbkWcav_FilterBox\" onchange=\"new_webcontent_filter('" + Base_URL + "',5);\">");
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

            Output.WriteLine("  <table id=\"sbkWcav_MainTable\" class=\"sbkWcav_Table display\">");
            Output.WriteLine("    <thead>");
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <th>ID</th>");
            Output.WriteLine("        <th>URL</th>");
            Output.WriteLine("        <th>Title</th>");
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

            // Determine the URL for the results
            string redirect_url = SobekEngineClient.WebContent.Get_All_Redirects_JDataTable_URL;

            // Add any query string (should probably use StringBuilder, but this should be fairly seldomly used very deeply)
            if (!String.IsNullOrEmpty(level1))
            {
                redirect_url = redirect_url + "?l1=" + level1;
                if (!String.IsNullOrEmpty(level2))
                {
                    redirect_url = redirect_url + "&l2=" + level2;
                    if (!String.IsNullOrEmpty(level3))
                    {
                        redirect_url = redirect_url + "&l3=" + level3;
                        if (!String.IsNullOrEmpty(level4))
                        {
                            redirect_url = redirect_url + "&l4=" + level4;
                            if (!String.IsNullOrEmpty(level5))
                            {
                                redirect_url = redirect_url + "&l5=" + level5;
                            }
                        }
                    }
                }
            }

            Output.WriteLine("           \"sAjaxSource\": \"" + redirect_url + "\",");
            Output.WriteLine("           \"aoColumns\": [ { \"bVisible\": false }, null, null ]  });");
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
        }
    }
}
