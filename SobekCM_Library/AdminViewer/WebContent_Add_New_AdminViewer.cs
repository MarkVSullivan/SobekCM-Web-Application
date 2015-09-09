using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    public class WebContent_Add_New_AdminViewer : abstract_AdminViewer
    {
        
        private string actionMessage;

        private readonly string level1;
        private readonly string level2;
        private readonly string level3;
        private readonly string level4;
        private readonly string level5;
        private readonly string level6;
        private readonly string level7;
        private readonly string level8;
        private readonly string title;
        private readonly string description;
        private readonly string redirect_url;
        private readonly string webSkin;

        /// <summary> Constructor for a new instance of the WebContent_Add_New_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public WebContent_Add_New_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("WebContent_Add_New_AdminViewer.Constructor", String.Empty);

            // Set some defaults
            actionMessage = String.Empty;

            // Ensure the user is the system admin or portal admin
            if ((RequestSpecificValues.Current_User == null) || ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin)))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If this is posted back, look for the reset
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                string reset_value = HttpContext.Current.Request.Form[""];
                if ((!String.IsNullOrEmpty(reset_value)) && (reset_value == "reset"))
                {
                    // Just ensure everything is emptied out
                    HttpContext.Current.Cache.Remove("GlobalPermissionsReport");
                    HttpContext.Current.Cache.Remove("GlobalPermissionsUsersLinked");
                    HttpContext.Current.Cache.Remove("GlobalPermissionsLinkedAggr");
                    HttpContext.Current.Cache.Remove("GlobalPermissionsReportSubmit");
                }
            }
            else
            {
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
                                    if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["l6"]))
                                    {
                                        level6 = HttpContext.Current.Request.QueryString["l6"];
                                        if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["l7"]))
                                        {
                                            level7 = HttpContext.Current.Request.QueryString["l7"];
                                            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["l8"]))
                                            {
                                                level5 = HttpContext.Current.Request.QueryString["l8"];
                                            }
                                        }
                                    }
                                }
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
            get { return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner }; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Web Content Usage Reports' </value>
        public override string Web_Title
        {
            get { return "Add New Web Content"; }
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
            Tracer.Add_Trace("WebContent_Add_New_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<!-- WebContent_Add_New_AdminViewer.Write_ItemNavForm_Closing -->");
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

            Output.WriteLine("    <div class=\"tabscontent\">");
            Output.WriteLine("    	<div class=\"sbkUgav_TabPage\" id=\"tabpage_1\">");

            // Add the buttons
            Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            Output.WriteLine("    <button title=\"Save changes to this user group\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            Output.WriteLine("  </div>");
            Output.WriteLine();

            // Help constants (for now)
            const string TITLE_HELP = "Help for title";
            const string DESCRIPTION_HELP = "Help for description";
            const string REDIRECT_HELP = "Help for redirect";
            const string WEBSKIN_HELP = "Help for webskin";

            string baseUrl = RequestSpecificValues.Current_Mode.Base_URL;
            if (baseUrl[baseUrl.Length - 1] == '/')
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Web Content URL</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>Web content pages (and redirects) are defined by the URL.  The URL can be up to eight segments deep.  For example, http://sobekrepository.org/sobekcm/development would have a level1 of 'sobekcm' and level2 of 'development'.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/webcontentadd\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_level1\">Level 1:</label></td>");
            Output.WriteLine("    <td><input class=\"sbkWcav_small_input sbkAdmin_Focusable\" name=\"admin_webcontent_level1\" id=\"admin_webcontent_level1\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(level1) + "\" onkeyup=\"new_webcontent_determine_url('" + baseUrl + "');\" /></td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_level2\">Level 2:</label></td>");
            Output.WriteLine("    <td><input class=\"sbkWcav_small_input sbkAdmin_Focusable\" name=\"admin_webcontent_level2\" id=\"admin_webcontent_level2\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(level2) + "\" onkeyup=\"new_webcontent_determine_url('" + baseUrl + "');\"  /></td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_level3\">Level 3:</label></td>");
            Output.WriteLine("    <td><input class=\"sbkWcav_small_input sbkAdmin_Focusable\" name=\"admin_webcontent_level3\" id=\"admin_webcontent_level3\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(level3) + "\" onkeyup=\"new_webcontent_determine_url('" + baseUrl + "');\"  /></td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_level4\">Level 4:</label></td>");
            Output.WriteLine("    <td><input class=\"sbkWcav_small_input sbkAdmin_Focusable\" name=\"admin_webcontent_level4\" id=\"admin_webcontent_level4\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(level4) + "\" onkeyup=\"new_webcontent_determine_url('" + baseUrl + "');\"  /></td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_level5\">Level 5:</label></td>");
            Output.WriteLine("    <td><input class=\"sbkWcav_small_input sbkAdmin_Focusable\" name=\"admin_webcontent_level5\" id=\"admin_webcontent_level5\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(level5) + "\" onkeyup=\"new_webcontent_determine_url('" + baseUrl + "');\"  /></td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_level6\">Level 6:</label></td>");
            Output.WriteLine("    <td><input class=\"sbkWcav_small_input sbkAdmin_Focusable\" name=\"admin_webcontent_level6\" id=\"admin_webcontent_level6\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(level6) + "\" onkeyup=\"new_webcontent_determine_url('" + baseUrl + "');\"  /></td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_level7\">Level 7:</label></td>");
            Output.WriteLine("    <td><input class=\"sbkWcav_small_input sbkAdmin_Focusable\" name=\"admin_webcontent_level7\" id=\"admin_webcontent_level7\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(level7) + "\" onkeyup=\"new_webcontent_determine_url('" + baseUrl + "');\"  /></td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_level8\">Level 8:</label></td>");
            Output.WriteLine("    <td><input class=\"sbkWcav_small_input sbkAdmin_Focusable\" name=\"admin_webcontent_level8\" id=\"admin_webcontent_level8\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(level8) + "\" onkeyup=\"new_webcontent_determine_url('" + baseUrl + "');\"  /></td>");
            Output.WriteLine("  </tr>");

            // Determine the initial url
            StringBuilder urlbuilder = new StringBuilder(baseUrl);
            if (!String.IsNullOrEmpty(level1))
            {
                urlbuilder.Append("/" + level1);
                if (!String.IsNullOrEmpty(level2))
                {
                    urlbuilder.Append("/" + level2);
                    if (!String.IsNullOrEmpty(level3))
                    {
                        urlbuilder.Append("/" + level3);
                        if (!String.IsNullOrEmpty(level4))
                        {
                            urlbuilder.Append("/" + level4);
                            if (!String.IsNullOrEmpty(level5))
                            {
                                urlbuilder.Append("/" + level5);
                                if (!String.IsNullOrEmpty(level6))
                                {
                                    urlbuilder.Append("/" + level6);
                                    if (!String.IsNullOrEmpty(level7))
                                    {
                                        urlbuilder.Append("/" + level7);
                                        if (!String.IsNullOrEmpty(level8))
                                        {
                                            urlbuilder.Append("/" + level8);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\">Complete URL:</td>");
            Output.WriteLine("    <td><span id=\"urlSpan\">" + urlbuilder + "</span></td>");
            Output.WriteLine("  </tr>");


            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Basic Information</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The information in this section is the basic information about the web content page and includes much of the metadata that is provided to search engines to increase page rank on relevant searches.</p></td></tr>");


            // Add the Title
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_title\">Title:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkWcav_large_input sbkAdmin_Focusable\" name=\"admin_webcontent_title\" id=\"admin_webcontent_title\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(title) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + TITLE_HELP + "');\"  title=\"" + TITLE_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            // Add the Description/Summary box
            Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_webcontent_desc\">Description:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkWcav_large_textbox sbkAdmin_Focusable\" rows=\"6\" name=\"admin_webcontent_desc\" id=\"admin_webcontent_desc\">" + HttpUtility.HtmlEncode(description) + "</textarea></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + DESCRIPTION_HELP + "');\"  title=\"" + DESCRIPTION_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            // Add the redirect URL
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_redirect\">Redirect URL:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkWcav_large_input sbkAdmin_Focusable\" name=\"admin_webcontent_redirect\" id=\"admin_webcontent_redirect\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(redirect_url) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + REDIRECT_HELP + "');\"  title=\"" + REDIRECT_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");



            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Appearance</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The value in this section determines how this web content page appears to users by allowing a web skin to be selected for this web content page to appear under.</p></td></tr>");


            // Add the web skin
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Web Skin:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");

            // Start the select box
            Output.Write("          <select class=\"sbkSaav_SelectSkin\" name=\"admin_webcontent_skin\" id=\"admin_webcontent_skin\">");

            // Add the NONE option first
            Output.Write(String.IsNullOrEmpty(webSkin) ? "<option value=\"\" selected=\"selected\" ></option>" : "<option value=\"\"></option>");

            // Get the ordered list of all skin codes
            List<string> skinCodes = UI_ApplicationCache_Gateway.Web_Skin_Collection.Ordered_Skin_Codes;

            // Add each web skin code to the select box
            foreach (string skinCode in skinCodes)
            {
                if (String.Compare(webSkin, skinCode, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Output.Write("<option value=\"" + skinCode + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(skinCode) + "</option>");
                }
                else
                {
                    Output.Write("<option value=\"" + skinCode + "\">" + HttpUtility.HtmlEncode(skinCode) + "</option>");
                }
            }
            Output.WriteLine("</select>");

            Output.WriteLine("        </td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + WEBSKIN_HELP + "');\"  title=\"" + WEBSKIN_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");


            Output.WriteLine("</table>");
            Output.WriteLine("<br />");

            Output.WriteLine();



  

            Output.WriteLine("</div>");
            Output.WriteLine("</div>");
            Output.WriteLine("</div>");

            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
        }
    }
}
