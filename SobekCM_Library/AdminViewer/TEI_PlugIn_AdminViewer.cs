using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.TEI;
using SobekCM.Library.UI;
using SobekCM.Library.UploadiFive;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated aggregation admin to updated mappings, XSLT, and CSS files as well as 
    /// assign permissions to use each one to users in the administrateive screens  </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.</remarks>
    public class TEI_PlugIn_AdminViewer : abstract_AdminViewer
    {
        private readonly int page;
        private string actionMessage;

        private TEI_Configuration teiConfig;
        private DataTable teiUserSettings;
        private List<Tuple<string, int>> teiUsers;

        /// <summary> Constructor for a new instance of the TEI_PlugIn_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public TEI_PlugIn_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Ensure the user is the system admin
            if ((RequestSpecificValues.Current_User == null) || ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin)))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Ensure the plug-in list exists and contains the TEI plug-in
            if ((UI_ApplicationCache_Gateway.Configuration.Extensions == null) || 
                ( UI_ApplicationCache_Gateway.Configuration.Extensions.Get_Extension("TEI") == null ) || 
                ( !UI_ApplicationCache_Gateway.Configuration.Extensions.Get_Extension("TEI").Enabled ))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Try to pull the configuration from the cache, otherwise create it manually
            teiConfig = HttpContext.Current.Cache.Get("TEI.Configuration") as TEI_Configuration;

            // Did not find it in the cache
            if (teiConfig == null)
            {
                // Build the new object then
                string plugin_directory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins", "tei");
                teiConfig = new TEI_Configuration(plugin_directory);

                // Store on the cache for several minutes
                HttpContext.Current.Cache.Insert("TEI.Configuration", teiConfig, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2));
            }

            // Try to pull the latest tei user settings
            teiUserSettings = HttpContext.Current.Cache.Get("TEI.UserSettings") as DataTable;

            // Did not find it in the cache
            if (teiUserSettings == null)
            {
                // Get all the settings from the database thne
                teiUserSettings = SobekCM_Database.Get_All_User_Settings_Like("TEI.%", "true");

                // Store on the cache for several minutes
                HttpContext.Current.Cache.Insert("TEI.UserSettings", teiUserSettings, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2));
            }

            // Create the list of sorted users approvied for TEI
            SortedList<string, Tuple<string, int>> nameSorter = new SortedList<string, Tuple<string, int>>();
            DataRow[] enabledRows = teiUserSettings.Select("Setting_Key = 'TEI.Enabled'");
            foreach (DataRow thisRow in enabledRows)
            {
                string name = thisRow["LastName"] + ", " + thisRow["FirstName"] + " (" + thisRow["UserName"] + ")";
                int id = Int32.Parse(thisRow["UserID"].ToString());
                nameSorter[name] = new Tuple<string, int>(name, id);
            }
            teiUsers = new List<Tuple<string, int>>();
            teiUsers.AddRange(nameSorter.Values);           
            
            // Determine the page
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

            // Look for a post back
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                // Get a reference to this form
                NameValueCollection form = HttpContext.Current.Request.Form;

                // Get the curret action
                string action = form["tei_admin_action"];
                if (action == "save")
                {
                    string[] getKeys = form.AllKeys;

                    actionMessage = "All changes saved";

                    bool values_changed = false;

                    switch (page)
                    {
                        case 1:
                            // Now, step through each sorted file name
                            foreach (string thisFileName in teiConfig.XSLT_Files)
                            {
                                // Now, step through each sorted username
                                foreach (Tuple<string, int> thisUser in teiUsers)
                                {
                                    string user_file_key = "admin_user_tei_xslt_" + thisFileName.ToLower() + "_" + thisUser.Item2;

                                    // Look for this checkbox
                                    if (form[user_file_key] == null)
                                    {
                                        // If the setting is already the same, no need to update the database
                                        if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.XSLT." + thisFileName.ToUpper() + "'").Length > 0)
                                        {
                                            SobekCM_Database.Set_User_Setting(thisUser.Item2, "TEI.XSLT." + thisFileName.ToUpper(), "false");
                                            values_changed = true;
                                        }
                                    }
                                    else
                                    {
                                        // If the setting is already the same, no need to update the database
                                        if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.XSLT." + thisFileName.ToUpper() + "'").Length == 0)
                                        {
                                            SobekCM_Database.Set_User_Setting(thisUser.Item2, "TEI.XSLT." + thisFileName.ToUpper(), "true");
                                            values_changed = true;
                                        }
                                    }
                                }
                            }
                            break;

                        case 2:
                            // Now, step through each sorted file name
                            foreach (string thisFileName in teiConfig.CSS_Files)
                            {
                                // Now, step through each sorted username
                                foreach (Tuple<string, int> thisUser in teiUsers)
                                {
                                    string user_file_key = "admin_user_tei_css_" + thisFileName.ToLower() + "_" + thisUser.Item2;

                                    // Look for this checkbox
                                    if (form[user_file_key] == null)
                                    {
                                        // If the setting is already the same, no need to update the database
                                        if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.CSS." + thisFileName.ToUpper() + "'").Length > 0)
                                        {
                                            SobekCM_Database.Set_User_Setting(thisUser.Item2, "TEI.CSS." + thisFileName.ToUpper(), "false");
                                            values_changed = true;
                                        }
                                    }
                                    else
                                    {
                                        // If the setting is already the same, no need to update the database
                                        if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.CSS." + thisFileName.ToUpper() + "'").Length == 0)
                                        {
                                            SobekCM_Database.Set_User_Setting(thisUser.Item2, "TEI.CSS." + thisFileName.ToUpper(), "true");
                                            values_changed = true;
                                        }
                                    }
                                }
                            }
                            break;

                        case 3:
                            // Now, step through each sorted file name
                            foreach (string thisFileName in teiConfig.Mapping_Files)
                            {
                                // Now, step through each sorted username
                                foreach (Tuple<string, int> thisUser in teiUsers)
                                {
                                    string user_file_key = "admin_user_tei_mapping_" + thisFileName.ToLower() + "_" + thisUser.Item2;

                                    // Look for this checkbox
                                    if (form[user_file_key] == null)
                                    {
                                        // If the setting is already the same, no need to update the database
                                        if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.MAPPING." + thisFileName.ToUpper() + "'").Length > 0)
                                        {
                                            SobekCM_Database.Set_User_Setting(thisUser.Item2, "TEI.MAPPING." + thisFileName.ToUpper(), "false");
                                            values_changed = true;
                                        }
                                    }
                                    else
                                    {
                                        // If the setting is already the same, no need to update the database
                                        if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.MAPPING." + thisFileName.ToUpper() + "'").Length == 0)
                                        {
                                            SobekCM_Database.Set_User_Setting(thisUser.Item2, "TEI.MAPPING." + thisFileName.ToUpper(), "true");
                                            values_changed = true;
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    // Should we refresh from the database?
                    if (values_changed)
                    {
                        // Get all the settings from the database thne
                        teiUserSettings = SobekCM_Database.Get_All_User_Settings_Like("TEI.%", "true");

                        // Store on the cache for several minutes
                        HttpContext.Current.Cache.Insert("TEI.UserSettings", teiUserSettings, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2));
                    }
                }

                // When a new file is uploaded, the action is empty string
                if (action == "")
                {
                    // Build the new object then
                    string plugin_directory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins", "tei");
                    teiConfig = new TEI_Configuration(plugin_directory);

                    // Store on the cache for several minutes
                    HttpContext.Current.Cache.Insert("TEI.Configuration", teiConfig, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2));
                }
            }
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

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner }; }
        }

        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // do noting
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'TEI Administrative Features' </value>
        public override string Web_Title
        {
            get { return "TEI Administrative Features"; }
        }

        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources_Gateway.Settings_Img; }
        }



        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the wordmarks list is added as controls, not HTML </remarks>
        public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("TEI_PlugIn_AdminViewer.Write_ItemNavForm_Opening", "");

            Output.WriteLine("<!-- TEI_PlugIn_AdminViewer.Write_ItemNavForm_Opening -->");

            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"tei_admin_action\" name=\"tei_admin_action\" value=\"\" />");

            Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");

            Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

            if (!String.IsNullOrWhiteSpace(actionMessage))
            {
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
            }

            Output.WriteLine("  <p style=\"text-align: left; padding:0 20px 0 70px;width:800px;\">These tabs allow you to manage the different portions of the TEI plug-in.  This includes uploading new XSLTs, CSS files, and mapping files.  In addition, you can map the different users to the different files to enable different functionality to different users.</p>");



            //Output.WriteLine("    <ul>");
            //Output.WriteLine("      <li>Enter the permissions for this user below and press the SAVE button when all your edits are complete.</li>");
            //Output.WriteLine("      <li>For clarification of any terms on this form, <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/users\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</li>");
            //Output.WriteLine("     </ul>");
            Output.WriteLine("  </div>");

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs sbkAdm_HomeTabs\">");
            Output.WriteLine("  <div class=\"tabs\">");
            Output.WriteLine("    <ul>");

            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "XyzzyXyzzy";
            string url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;

            if (page == 1)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> XML Transforms (XSLT) </li>");
            }
            else
            {
                Output.WriteLine("      <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "a") + "';return false;\"> XML Transforms (XSLT) </li>");
            }

            if (page == 2)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> Stylesheets (CSS) </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "b") + "';return false;\"> Stylesheets (CSS) </li>");
            }

            if (page == 3)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> Metadata Mappings </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "c") + "';return false;\"> Metadata Mappings </li>");
            }


            Output.WriteLine("    </ul>");
            Output.WriteLine("  </div>");

            Output.WriteLine("    <div class=\"tabscontent\">");
            Output.WriteLine("    	<div class=\"sbkTeiAv_TabPage\" id=\"tabpage_1\">");

            //// Add the buttons
            //Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            //Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            //Output.WriteLine("    <button title=\"Save changes to this user group\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            //Output.WriteLine("  </div>");
            //Output.WriteLine();


            Output.WriteLine();

            switch (page)
            {
                case 1:
                    Output.WriteLine("    	  <p style=\"width:800px;\">When the TEI item is viewed online, the XSLT is used to transform the TEI XML into HTML to display.</p>");
                    Output.WriteLine("    	  <p style=\"width:800px;\">This page allows you to upload new XSLTs to use within the TEI module and allows you to permission users to XSLTs.  This controls if a user can select the XSLT transform to be used for a new TEI file they upload.</p>");

                    Output.WriteLine("    	  <h3>Upload new XSLT</h3>");
                    Output.WriteLine("    	  <p style=\"width:800px;\">To upload a new XSLT, or replace an existing one, select the button below and navigate to your XSLT file(s) to upload.</p>");
                    break;

                case 2:
                    Output.WriteLine("    	  <p style=\"width:800px;\">When the TEI item is viewed online, TEI HTML is displayed within the SobekCM window, but a special CSS file can be included to control the look and feel of the TEI.</p>");
                    Output.WriteLine("    	  <p style=\"width:800px;\">This page allows you to upload new CSSs to use within the TEI module and allows you to permission users to CSS files.  This controls if a user can select the stylesheet to be used for a new TEI file they upload.</p>");

                    Output.WriteLine("    	  <h3>Upload new CSS</h3>");
                    Output.WriteLine("    	  <p style=\"width:800px;\">To upload a new CSS stylesheet, or replace an existing one, select the button below and navigate to your XSLT file(s) to upload.</p>");
                    break;

                case 3:
                    Output.WriteLine("    	  <p style=\"width:800px;\">When the TEI item is ingested into the system, the metadata within the TEI is extracted using a generic XML reader provided a specific mapping gile.</p>");
                    Output.WriteLine("    	  <p style=\"width:800px;\">This page allows you to upload new mappings to use within the TEI module and allows you to permission users to use individual mapping files. </p>");

                    Output.WriteLine("    	  <h3>Upload new mapping file</h3>");
                    Output.WriteLine("    	  <p style=\"width:800px;\">To upload a new mapping XML file, or replace an existing one, select the button below and navigate to your XSLT file(s) to upload.</p>");
                    break;
            }



        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<!-- TEI_PlugIn_AdminViewer.Write_ItemNavForm_Closing -->");


            //// Add the buttons
            //RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            //Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            //Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            //Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            //Output.WriteLine("  </div>");


            // Get the current view type
            string view = HttpContext.Current.Request.QueryString["view"];

            // Get the URl for the other view type
            string url = UrlWriterHelper.Redirect_URL( RequestSpecificValues.Current_Mode );
            if (view != "users")
            {
                url = (url.IndexOf("?") > 0) ? (url + "&view=users") : (url + "?view=users");
            }

            switch (page)
            {
                case 1:
                    // Start this section
                    Output.WriteLine("    	  <h3>Existing XSLT Files</h3>");

                    // Were there files?
                    if (teiConfig.XSLT_Files.Count == 0)
                    {
                        Output.WriteLine("    	  <p style=\"width:800px;\">There are currently no XSLT files for the TEI module.  Please upload one above.</p>");
                    }
                    else
                    {
                        Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\" style=\"visibility:hidden;\">");
                        Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'cancel');return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                        Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'save');return false;\">SAVE <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
                        Output.WriteLine("  </div>");

                        Output.WriteLine("    	  <p style=\"width:800px;\">Below is the list of all the XSLT files and who is currently permissioned to use them.</p>");
                        Output.WriteLine("    	  <p style=\"width:800px;\">Only the users that have been enabled for the TEI work will appear in the list below.  If there are no users listed below, go into the Users &amp; Groups permissions management screen and enable a user to allow them to use the TEI plug-in.</p>");


                        if (view != "users")
                        {
                            Output.WriteLine("    	  <p style=\"width:800px;\">You are currently viewing the table by XSLT filename.  <a href=\"" + url + "\">Click here to view by user.</a></p>");
                            Output.WriteLine();

                            Output.WriteLine("        <table class=\"sbkAdm_Table\" id=\"sbkTeiAv_Table\">");
                            Output.WriteLine("          <tr>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableFileColumn\">XSLT File</th>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableLargerColumn\">Users</th>");
                            Output.WriteLine("          </tr>");



                            // Now, step through each sorted file name
                            foreach (string thisFileName in teiConfig.XSLT_Files)
                            {
                                Output.WriteLine("          <tr>");
                                Output.WriteLine("            <td>" + thisFileName.Replace("_", " ") + "</td>");
                                Output.WriteLine("            <td>");

                                // Now, step through each sorted username
                                foreach (Tuple<string, int> thisUser in teiUsers)
                                {
                                    Output.Write("              <div class=\"sbkTeiAv_CheckboxDiv\">");
                                    string html_name = "admin_user_tei_xslt_" + thisFileName.ToLower() + "_" + thisUser.Item2;
                                    if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.XSLT." + thisFileName.ToUpper() + "'").Length > 0)
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' checked=\"checked\" /> <label for=\"" + html_name + "\">" + thisUser.Item1 + "</label>");

                                    }
                                    else
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' /> <label for=\"" + html_name + "\">" + thisUser.Item1 + "</label>");
                                    }
                                    Output.WriteLine("</div>");
                                }


                                Output.WriteLine("            </td>");
                                Output.WriteLine("          </tr>");
                                Output.WriteLine("          <tr><td class=\"sbkAdm_TableRule\" colspan=\"2\"></td></tr>");
                            }

                            Output.WriteLine("        </table>");
                        }
                        else
                        {

                            Output.WriteLine("    	  <p style=\"width:800px;\">You are currently viewing the table by username.  <a href=\"" + url + "\">Click here to view by XSLT filename.</a></p>");
                            Output.WriteLine();

                            Output.WriteLine("        <table class=\"sbkAdm_Table\" id=\"sbkTeiAv_Table\">");
                            Output.WriteLine("          <tr>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableUserColumn\">User</th>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableLargerColumn\">XSLT Files</th>");
                            Output.WriteLine("          </tr>");

                            // Step through users on the outside of this loop
                            foreach (Tuple<string, int> thisUser in teiUsers)
                            {
                                Output.WriteLine("          <tr>");
                                Output.WriteLine("            <td>" + thisUser.Item1 + "</td>");
                                Output.WriteLine("            <td>");

                                // Now, step through each sorted file name
                                foreach (string thisFileName in teiConfig.XSLT_Files)
                                {

                                    Output.Write("              <div class=\"sbkTeiAv_CheckboxDiv\">");
                                    string html_name = "admin_user_tei_xslt_" + thisFileName.ToLower() + "_" + thisUser.Item2;
                                    if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.XSLT." + thisFileName.ToUpper() + "'").Length > 0)
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' checked=\"checked\" /> <label for=\"" + html_name + "\">" + thisFileName.Replace("_", " ") + "</label>");

                                    }
                                    else
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' /> <label for=\"" + html_name + "\">" + thisFileName.Replace("_", " ") + "</label>");
                                    }
                                    Output.WriteLine("</div>");
                                }


                                Output.WriteLine("            </td>");
                                Output.WriteLine("          </tr>");
                                Output.WriteLine("          <tr><td class=\"sbkAdm_TableRule\" colspan=\"2\"></td></tr>");
                            }

                            Output.WriteLine("        </table>");
                        }

                        Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\" style=\"visibility:hidden;\">");
                        Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'cancel');return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                        Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'save');return false;\">SAVE <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
                        Output.WriteLine("  </div>");
                    }

                    break;

                case 2:
                    // Start this section
                    Output.WriteLine("    	  <h3>Existing CSS Files</h3>");

                    // Were there files?
                    if (teiConfig.XSLT_Files.Count == 0)
                    {
                        Output.WriteLine("    	  <p style=\"width:800px;\">There are currently no CSS files for the TEI module.  Please upload one above.</p>");
                    }
                    else
                    {
                        Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\" style=\"visibility:hidden;\">");
                        Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'cancel');return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                        Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'save');return false;\">SAVE <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
                        Output.WriteLine("  </div>");

                        Output.WriteLine("    	  <p style=\"width:800px;\">Below is the list of all the CSS files and who is currently permissioned to use them.</p>");
                        Output.WriteLine("    	  <p style=\"width:800px;\">Only the users that have been enabled for the TEI work will appear in the list below.  If there are no users listed below, go into the Users &amp; Groups permissions management screen and enable a user to allow them to use the TEI plug-in.</p>");


                        if (view != "users")
                        {
                            Output.WriteLine("    	  <p style=\"width:800px;\">You are currently viewing the table by CSS filename.  <a href=\"" + url + "\">Click here to view by user.</a></p>");
                            Output.WriteLine();

                            Output.WriteLine("        <table class=\"sbkAdm_Table\" id=\"sbkTeiAv_Table\">");
                            Output.WriteLine("          <tr>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableFileColumn\">CSS File</th>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableLargerColumn\">Users</th>");
                            Output.WriteLine("          </tr>");



                            // Now, step through each sorted file name
                            foreach (string thisFileName in teiConfig.CSS_Files)
                            {
                                Output.WriteLine("          <tr>");
                                Output.WriteLine("            <td>" + thisFileName.Replace("_", " ") + "</td>");
                                Output.WriteLine("            <td>");

                                // Now, step through each sorted username
                                foreach (Tuple<string, int> thisUser in teiUsers)
                                {
                                    Output.Write("              <div class=\"sbkTeiAv_CheckboxDiv\">");
                                    string html_name = "admin_user_tei_css_" + thisFileName.ToLower() + "_" + thisUser.Item2;
                                    if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.CSS." + thisFileName.ToUpper() + "'").Length > 0)
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' checked=\"checked\" /> <label for=\"" + html_name + "\">" + thisUser.Item1 + "</label>");

                                    }
                                    else
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' /> <label for=\"" + html_name + "\">" + thisUser.Item1 + "</label>");
                                    }
                                    Output.WriteLine("</div>");
                                }


                                Output.WriteLine("            </td>");
                                Output.WriteLine("          </tr>");
                                Output.WriteLine("          <tr><td class=\"sbkAdm_TableRule\" colspan=\"2\"></td></tr>");
                            }

                            Output.WriteLine("        </table>");
                        }
                        else
                        {

                            Output.WriteLine("    	  <p style=\"width:800px;\">You are currently viewing the table by username.  <a href=\"" + url + "\">Click here to view by CSS filename.</a></p>");
                            Output.WriteLine();

                            Output.WriteLine("        <table class=\"sbkAdm_Table\" id=\"sbkTeiAv_Table\">");
                            Output.WriteLine("          <tr>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableUserColumn\">User</th>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableLargerColumn\">CSS Files</th>");
                            Output.WriteLine("          </tr>");

                            // Step through users on the outside of this loop
                            foreach (Tuple<string, int> thisUser in teiUsers)
                            {
                                Output.WriteLine("          <tr>");
                                Output.WriteLine("            <td>" + thisUser.Item1 + "</td>");
                                Output.WriteLine("            <td>");

                                // Now, step through each sorted file name
                                foreach (string thisFileName in teiConfig.CSS_Files)
                                {

                                    Output.Write("              <div class=\"sbkTeiAv_CheckboxDiv\">");
                                    string html_name = "admin_user_tei_css_" + thisFileName.ToLower() + "_" + thisUser.Item2;
                                    if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.CSS." + thisFileName.ToUpper() + "'").Length > 0)
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' checked=\"checked\" /> <label for=\"" + html_name + "\">" + thisFileName.Replace("_", " ") + "</label>");

                                    }
                                    else
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' /> <label for=\"" + html_name + "\">" + thisFileName.Replace("_", " ") + "</label>");
                                    }
                                    Output.WriteLine("</div>");
                                }


                                Output.WriteLine("            </td>");
                                Output.WriteLine("          </tr>");
                                Output.WriteLine("          <tr><td class=\"sbkAdm_TableRule\" colspan=\"2\"></td></tr>");
                            }

                            Output.WriteLine("        </table>");
                        }

                        Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\" style=\"visibility:hidden;\">");
                        Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'cancel');return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                        Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'save');return false;\">SAVE <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
                        Output.WriteLine("  </div>");
                    }
                    break;

                case 3:
                    // Start this section
                    Output.WriteLine("    	  <h3>Existing XML Mapping Files</h3>");

                    // Were there files?
                    if (teiConfig.XSLT_Files.Count == 0)
                    {
                        Output.WriteLine("    	  <p style=\"width:800px;\">There are currently no mapping files for the TEI module.  Please upload one above.</p>");
                    }
                    else
                    {
                        Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\" style=\"visibility:hidden;\">");
                        Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'cancel');return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                        Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'save');return false;\">SAVE <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
                        Output.WriteLine("  </div>");

                        Output.WriteLine("    	  <p style=\"width:800px;\">Below is the list of all the mapping files and who is currently permissioned to use them.</p>");
                        Output.WriteLine("    	  <p style=\"width:800px;\">Only the users that have been enabled for the TEI work will appear in the list below.  If there are no users listed below, go into the Users &amp; Groups permissions management screen and enable a user to allow them to use the TEI plug-in.</p>");


                        if (view != "users")
                        {
                            Output.WriteLine("    	  <p style=\"width:800px;\">You are currently viewing the table by mapping filename.  <a href=\"" + url + "\">Click here to view by user.</a></p>");
                            Output.WriteLine();

                            Output.WriteLine("        <table class=\"sbkAdm_Table\" id=\"sbkTeiAv_Table\">");
                            Output.WriteLine("          <tr>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableFileColumn\">Mapping File</th>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableLargerColumn\">Users</th>");
                            Output.WriteLine("          </tr>");



                            // Now, step through each sorted file name
                            foreach (string thisFileName in teiConfig.Mapping_Files)
                            {
                                Output.WriteLine("          <tr>");
                                Output.WriteLine("            <td>" + thisFileName.Replace("_", " ") + "</td>");
                                Output.WriteLine("            <td>");

                                // Now, step through each sorted username
                                foreach (Tuple<string, int> thisUser in teiUsers)
                                {
                                    Output.Write("              <div class=\"sbkTeiAv_CheckboxDiv\">");
                                    string html_name = "admin_user_tei_mapping_" + thisFileName.ToLower() + "_" + thisUser.Item2;
                                    if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.MAPPING." + thisFileName.ToUpper() + "'").Length > 0)
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' checked=\"checked\" /> <label for=\"" + html_name + "\">" + thisUser.Item1 + "</label>");

                                    }
                                    else
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' /> <label for=\"" + html_name + "\">" + thisUser.Item1 + "</label>");
                                    }
                                    Output.WriteLine("</div>");
                                }


                                Output.WriteLine("            </td>");
                                Output.WriteLine("          </tr>");
                                Output.WriteLine("          <tr><td class=\"sbkAdm_TableRule\" colspan=\"2\"></td></tr>");
                            }

                            Output.WriteLine("        </table>");
                        }
                        else
                        {

                            Output.WriteLine("    	  <p style=\"width:800px;\">You are currently viewing the table by username.  <a href=\"" + url + "\">Click here to view by mapping filename.</a></p>");
                            Output.WriteLine();

                            Output.WriteLine("        <table class=\"sbkAdm_Table\" id=\"sbkTeiAv_Table\">");
                            Output.WriteLine("          <tr>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableUserColumn\">User</th>");
                            Output.WriteLine("            <th id=\"sbkTeiAv_TableLargerColumn\">Mapping Files</th>");
                            Output.WriteLine("          </tr>");

                            // Step through users on the outside of this loop
                            foreach (Tuple<string, int> thisUser in teiUsers)
                            {
                                Output.WriteLine("          <tr>");
                                Output.WriteLine("            <td>" + thisUser.Item1 + "</td>");
                                Output.WriteLine("            <td>");

                                // Now, step through each sorted file name
                                foreach (string thisFileName in teiConfig.Mapping_Files)
                                {

                                    Output.Write("              <div class=\"sbkTeiAv_CheckboxDiv\">");
                                    string html_name = "admin_user_tei_mapping_" + thisFileName.ToLower() + "_" + thisUser.Item2;
                                    if (teiUserSettings.Select("UserID=" + thisUser.Item2 + " and Setting_Key='TEI.MAPPING." + thisFileName.ToUpper() + "'").Length > 0)
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' checked=\"checked\" /> <label for=\"" + html_name + "\">" + thisFileName.Replace("_", " ") + "</label>");

                                    }
                                    else
                                    {
                                        Output.Write("<input type=\"checkbox\" name=\"" + html_name + "\" id=\"" + html_name + "\" onchange='$(\".sbkSeav_ButtonsDiv\").css(\"visibility\", \"visible\");' /> <label for=\"" + html_name + "\">" + thisFileName.Replace("_", " ") + "</label>");
                                    }
                                    Output.WriteLine("</div>");
                                }


                                Output.WriteLine("            </td>");
                                Output.WriteLine("          </tr>");
                                Output.WriteLine("          <tr><td class=\"sbkAdm_TableRule\" colspan=\"2\"></td></tr>");
                            }

                            Output.WriteLine("        </table>");
                        }

                        Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\" style=\"visibility:hidden;\">");
                        Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'cancel');return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                        Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return set_hidden_value_postback('tei_admin_action', 'save');return false;\">SAVE <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
                        Output.WriteLine("  </div>");
                    }
                    break;
            }

            Output.WriteLine();

            Output.WriteLine("</div>");
            Output.WriteLine("</div>");
            Output.WriteLine("</div>");

            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
        }

        /// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("File_Managament_MySobekViewer.Add_Controls", String.Empty);

            // Determine the directory and the extensions
            string directory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins", "tei");
            string extensions = "";
            string buttonText = "Select Files";
            switch (page)
            {
                case 1:
                    directory = Path.Combine(directory, "xslt");
                    extensions = ".xslt";
                    buttonText = "Select XSLT Files";
                    break;

                case 2:
                    directory = Path.Combine(directory, "css");
                    extensions = ".css";
                    buttonText = "Select CSS Files";
                    break;

                case 3:
                    directory = Path.Combine(directory, "mapping");
                    extensions = ".xml";
                    buttonText = "Select Mapping Files";
                    break;

            }

            // Check to see if the directory exists
            try
            {
                if ( !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }
            catch (Exception)
            {
                actionMessage = "Error creating the necesasry folder under the TEI plug-in folder";
                return;
            }

            // Add the upload controls to the file place holder
            add_upload_controls(directory, extensions, buttonText, MainPlaceHolder, Tracer);
        }


        private void add_upload_controls(string DestinationDirectory, string Extensions, string ButtonText, PlaceHolder UploadFilesPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("File_Managament_MySobekViewer.add_upload_controls", String.Empty);

            StringBuilder filesBuilder = new StringBuilder(2000);

            LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
            UploadFilesPlaceHolder.Controls.Add(filesLiteral2);
            filesBuilder.Remove(0, filesBuilder.Length);

            UploadiFiveControl uploadControl = new UploadiFiveControl();
            uploadControl.UploadPath = DestinationDirectory;
            uploadControl.UploadScript = RequestSpecificValues.Current_Mode.Base_URL + "UploadiFiveFileHandler.ashx";
            uploadControl.AllowedFileExtensions = Extensions;//.jpg,.png,.gif,.bmp,.jpeg";
            uploadControl.RemoveCompleted = true;
            uploadControl.SubmitWhenQueueCompletes = true;
            uploadControl.Multi = false;
            uploadControl.ButtonText = ButtonText;
            uploadControl.CssClass = "sbkTeiAv_UploadButton";
            UploadFilesPlaceHolder.Controls.Add(uploadControl);

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            UploadFilesPlaceHolder.Controls.Add(literal1);
        }

        /// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden/omitted. </summary>
        /// <value> This property returns TRUE, since wordmarks can be uploaded here </value>
        public override bool Upload_File_Possible
        {
            get { return true; }
        }
    }
}
