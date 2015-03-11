using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Client;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.Skins;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UploadiFive;
using SobekCM.Tools;
using SobekCM.UI_Library;

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated aggregation admin to edit information related to a single item aggregation. </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to view and edit information related to a single item aggregation</li>
    /// </ul></remarks>
    public class Skin_Single_AdminViewer : abstract_AdminViewer
    {
        private string actionMessage;
        private readonly string skinDirectory;
        private readonly Complete_Web_Skin_Object webSkin;

        private readonly int page;


        /// <summary> Constructor for a new instance of the Skin_Single_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public Skin_Single_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Skin_Single_AdminViewer.Constructor", String.Empty);

            // Set some defaults
            actionMessage = String.Empty;
            string code = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;

            // If the user cannot edit this, go back
            if ((!RequestSpecificValues.Current_User.Is_Portal_Admin ) && ( !RequestSpecificValues.Current_User.Is_System_Admin ))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Load the web skin, either currenlty from the session (if already editing this skin )
            // or by building the complete web skin object
            object possibleEditSkin = HttpContext.Current.Session["Edit_Skin_" + code];
            Complete_Web_Skin_Object cachedInstance = possibleEditSkin as Complete_Web_Skin_Object;
            if (cachedInstance != null)
            {
                webSkin = cachedInstance;
            }
            else
            {
                webSkin = SobekEngineClient.WebSkins.Get_Complete_Web_Skin(code, RequestSpecificValues.Tracer);
            }

            // If unable to retrieve this skin, send to home
            if (webSkin == null)
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Get the skin directory and ensure it exists
            skinDirectory = HttpContext.Current.Server.MapPath("design/skins/" + webSkin.Skin_Code);
            if (!Directory.Exists(skinDirectory))
                Directory.CreateDirectory(skinDirectory);

            // Determine the page
            page = 1;
            if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "b")
                page = 2;
            else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "c")
                page = 3;
            else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "d")
                page = 4;

            // If this is a postback, handle any events first
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    // Get the curret action
                    string action = form["admin_aggr_save"];

                    // If no action, then we should return to the current tab page
                    if (action.Length == 0)
                        action = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;

                    // If this is to cancel, handle that here; no need to handle post-back from the
                    // editing form page first
                    if (action == "z")
                    {
                        // Clear the aggregation from the sessions
                        HttpContext.Current.Session["Edit_Skin_" + webSkin.Skin_Code] = null;

                        // Redirect the RequestSpecificValues.Current_User
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                        RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        return;
                    }

                    // Save the returned values, depending on the page
                    switch (page)
                    {
                        case 1:
                            Save_Page_1_Postback(form);
                            break;

                        case 2:
                            Save_Page_2_Postback(form);
                            break;

                        case 3:
                            Save_Page_3_Postback(form);
                            break;

                        case 4:
                            Save_Page_4_Postback(form);
                            break;
                    }

                    // Should this be saved to the database?
                    //if (action == "save")
                    //{
                    //    // Save to the database
                    //    if (!Item_Aggregation_Utilities.Save_To_Database(itemAggregation, RequestSpecificValues.Current_User.Full_Name, null))
                    //        successful_save = false;

                    //    // Clear the aggregation from the cache
                    //    CachedDataManager.Aggregations.Remove_Item_Aggregation(itemAggregation.Code, null);

                    //    // Forward back to the aggregation home page, if this was successful
                    //    if (successful_save)
                    //    {
                    //        // Clear the aggregation from the sessions
                    //        HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;
                    //        HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] = null;

                    //        // Redirect the RequestSpecificValues.Current_User
                    //        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                    //        RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    //        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    //    }
                    //    else
                    //    {
                    //        actionMessage = "Error saving aggregation information!";
                    //    }
                    //}
                    //else
                    //{
                    //    // Save to the admins session
                    //    HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = itemAggregation;
                    //    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = action;
                    //    HttpContext.Current.Response.Redirect(UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode), false);
                    //    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    //    RequestSpecificValues.Current_Mode.Request_Completed = true;
                    //}
                }
                catch
                {
                    actionMessage = "Unable to correctly parse postback data.";
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Skins' </value>
        public override string Web_Title
        {
            get { return webSkin != null ? "Edit " + webSkin.Skin_Code + " Web Skin" : "Edit Web Skin"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Skin_Single_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form before any controls are placed in the main place holder </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Skin_Single_AdminViewer.Write_ItemNavForm_Opening", "Add the majority of the HTML before the placeholder");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_reset\" name=\"admin_aggr_reset\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_save\" name=\"admin_aggr_save\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_action\" name=\"admin_aggr_action\" value=\"\" />");
            Output.WriteLine();

            Tracer.Add_Trace("Skin_Single_AdminViewer.Write_ItemNavForm_Closing", "Add the rest of the form");

            Output.WriteLine("<!-- Users_AdminViewer.Write_ItemNavForm_Closing -->");

            Output.WriteLine("<script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine();

            Output.WriteLine("<div id=\"sbkSaav_PageContainer\">");

            // Add the buttons (unless this is a sub-page like editing the CSS file)
            if (page < 9)
            {
                string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
                Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('z');\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                Output.WriteLine("    <button title=\"Save changes to this item Aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"return save_aggr_edits();\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
                Output.WriteLine("  </div>");
                Output.WriteLine();
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;
            }
            else if (page == 10)
            {
                Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
                Output.WriteLine("    <button title=\"Close this child page details and return to main admin pages\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('g');\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> BACK </button>");
                Output.WriteLine("  </div>");
            }


            Output.WriteLine("  <div class=\"sbkSaav_HomeText\">");
            Output.WriteLine("    <br />");
            Output.WriteLine("    <h1>Web Skin Administration : " + webSkin.Skin_Code + "</h1>");
            Output.WriteLine("  </div>");
            Output.WriteLine();

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs\">");

            // Add all the possible tabs (unless this is a sub-page like editing the CSS file)
            if (page < 9)
            {
                Output.WriteLine("    <div class=\"tabs\">");
                Output.WriteLine("      <ul>");

                const string GENERAL = "General";
                const string STYLESHEET = "CSS Stylesheet";
                const string HTML = "HTML (Headers/Footers)";
                const string BANNERS = "Banners";

                // Draw all the page tabs for this form
                if (page == 1)
                {
                    Output.WriteLine("    <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + GENERAL + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_1\" onclick=\"return new_aggr_edit_page('a');\">" + GENERAL + "</li>");
                }

                if (page == 2)
                {
                    Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + STYLESHEET + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_aggr_edit_page('b');\">" + STYLESHEET + "</li>");
                }

                if (page == 3)
                {
                    Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + HTML + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_aggr_edit_page('c');\">" + HTML + "</li>");
                }

                if (page == 4)
                {
                    Output.WriteLine("    <li id=\"tabHeader_3\" class=\"tabActiveHeader\">" + BANNERS + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_3\" onclick=\"return new_aggr_edit_page('d');\">" + BANNERS + "</li>");
                }

                Output.WriteLine("      </ul>");
                Output.WriteLine("    </div>");
            }

            // Add the single tab.  When users click on a tab, it goes back to the server (here)
            // to render the correct tab content
            Output.WriteLine("    <div class=\"tabscontent\">");
            Output.WriteLine("    	<div class=\"tabpage\" id=\"tabpage_1\">");


            switch (page)
            {
                case 1:
                    Add_Page_1(Output);
                    break;

                case 2:
                    Add_Page_2(Output);
                    break;

                case 3:
                    Add_Page_3(Output);
                    break;

                case 4:
                    Add_Page_4(Output);
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
            Tracer.Add_Trace("Skin_Single_AdminViewer.Write_ItemNavForm_Closing", "Add any html after the placeholder and close tabs");

            switch (page)
            {
                case 1:
                    Finish_Page_4(Output);
                    break;
            }


            Output.WriteLine("    </div>");
            Output.WriteLine("  </div>");
            Output.WriteLine("</div>");
            Output.WriteLine("<br />");
        }

        #region Methods to render (and parse) page 1 - Basic Information

        private void Save_Page_1_Postback(NameValueCollection Form)
        {


        }

        private void Add_Page_1(TextWriter Output)
        {
            // Help constants (for now)
            const string LONG_NAME_HELP = "Long name help place holder";
            const string SHORT_NAME_HELP = "Short name help place holder";
            const string LINK_HELP = "Link help place holder";
            const string DESCRIPTION_HELP = "Description help place holder";
            const string EMAIL_HELP = "Email help place holder";
            const string ACTIVE_HELP = "Active checkbox help place holder";
            const string HIDDEN_HELP = "Hidden checkbox help place holder";
            const string COLLECTION_BUTTON_HELP = "Collection button help place holder";


            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Basic Information</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The information in this section is the basic information about the web skin, such as the full name, the shortened name used for breadcrumbs, the description, and the email contact.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

            // Add the web skin code
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\">Web Skin Code:</td>");
            Output.WriteLine("    <td> " + HttpUtility.HtmlEncode(webSkin.Skin_Code) + "</td>");
            Output.WriteLine("  </tr>");

            // Add the base skin code
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"webskin_basecode\">Base Skin Code:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
            
            Output.WriteLine("        <select class=\"sbkSav_small_input1 sbkAdmin_Focusable\" name=\"webskin_basecode\" id=\"webskin_basecode\">");
            Output.WriteLine("          <option value=\"\"></option>");
            foreach (string thisSkinCode in UI_ApplicationCache_Gateway.Web_Skin_Collection.Ordered_Skin_Codes)
            {
                Output.WriteLine("          <option value=\"" + thisSkinCode.ToUpper() + "\">" + thisSkinCode + "</option>");
            }
            Output.WriteLine("        </select></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + LONG_NAME_HELP + "');\"  title=\"" + LONG_NAME_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            // Add the banner link
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"webskin_bannerlink\">Banner Link:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_medium_input sbkAdmin_Focusable\" name=\"webskin_bannerlink\" id=\"webskin_bannerlink\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(webSkin.Banner_Link) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + SHORT_NAME_HELP + "');\"  title=\"" + SHORT_NAME_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            // Add the notes
            Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"webskin_notes\">Notes:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSaav_large_textbox sbkAdmin_Focusable\" rows=\"6\" name=\"webskin_notes\" id=\"webskin_notes\">" + HttpUtility.HtmlEncode(webSkin.Notes) + "</textarea></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DESCRIPTION_HELP + "');\"  title=\"" + DESCRIPTION_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            //// Add checkboxes for overriding the header/footer 
            //Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            //Output.WriteLine("    <td>&nbsp;</td>");
            //Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_email\">Contact Email:</label></td>");
            //Output.WriteLine("    <td>");
            //Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_email\" id=\"admin_aggr_email\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Contact_Email) + "\" /></td>");
            //Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + EMAIL_HELP + "');\"  title=\"" + EMAIL_HELP + "\" /></td></tr></table>");
            //Output.WriteLine("     </td>");
            //Output.WriteLine("  </tr>");

            //// Add checkboxes for building the web skin on launch (and retain fairly permanently - rather than discard after X minutes )
            //Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            //Output.WriteLine("    <td>&nbsp;</td>");
            //Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_email\">Contact Email:</label></td>");
            //Output.WriteLine("    <td>");
            //Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_email\" id=\"admin_aggr_email\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Contact_Email) + "\" /></td>");
            //Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + EMAIL_HELP + "');\"  title=\"" + EMAIL_HELP + "\" /></td></tr></table>");
            //Output.WriteLine("     </td>");
            //Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br />");

        }


        #endregion

        #region Methods to render (and parse) page 2 - Stylesheet

        private void Save_Page_2_Postback(NameValueCollection Form)
        {
            //// Check for action flag
            //string action = Form["admin_aggr_action"];
            //if (action == "save_css")
            //{
            //    string css_contents = Form["admin_aggr_css_edit"].Trim();
            //    if (css_contents.Length == 0)
            //        css_contents = "/**  Aggregation-level CSS for " + itemAggregation.Code + " **/";
            //    string file = aggregationDirectory + "\\" + itemAggregation.CSS_File;
            //    StreamWriter writer = new StreamWriter(file, false);
            //    writer.WriteLine(css_contents);
            //    writer.WriteLine();
            //    writer.Flush();
            //    writer.Close();
            //}
        }

        private void Add_Page_2(TextWriter Output)
        {
            //// Get the CSS file's contents
            //string css_contents;
            //string file = aggregationDirectory + "\\" + itemAggregation.CSS_File;
            //if (File.Exists(file))
            //{
            //    StreamReader reader = new StreamReader(file);
            //    css_contents = reader.ReadToEnd();
            //    reader.Close();
            //}
            //else
            //{
            //    css_contents = "/**  Aggregation-level CSS for " + itemAggregation.Code + " **/";
            //}

            //Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            //Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Aggregation-level Custom Stylesheet (CSS)</td></tr>");
            //Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>You can edit the contents of the aggregation-level custom stylesheet (css) file here.  Click SAVE when complete to return to the main aggregation administration screen.</p><p>NOTE: You may need to refresh your browser for your changes to take affect.</p></td></tr>");

            //// Add the css edit textarea code
            //Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" >");
            //Output.WriteLine("    <td style=\"width:40px;\">&nbsp;</td>");
            //Output.WriteLine("    <td>");
            //Output.WriteLine("      <textarea class=\"sbkSaav_EditCssTextarea sbkAdmin_Focusable\" id=\"admin_aggr_css_edit\" name=\"admin_aggr_css_edit\">");
            //Output.WriteLine(css_contents);
            //Output.WriteLine("      </textarea>");
            //Output.WriteLine("     </td>");
            //Output.WriteLine("  </tr>");


            //// Add the button line
            //Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" style=\"height:60px\">");
            //Output.WriteLine("    <td>&nbsp;</td>");
            //Output.WriteLine("    <td style=\"text-align:right; padding-right: 100px\">");
            //Output.WriteLine("      <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('e');\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            //Output.WriteLine("      <button title=\"Save changes to this stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return save_css_edits();\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            //Output.WriteLine("    </td>");
            //Output.WriteLine("  </tr>");


            //Output.WriteLine("</table>");
            //Output.WriteLine("<br />");

        }

        #endregion

        #region Methods to render (and parse) page 3 - HTML (headers and footers)

        private void Save_Page_3_Postback(NameValueCollection Form)
        {
  
        }

        private void Add_Page_3(TextWriter Output)
        {
 
        }

        #endregion

        #region Methods to render (and parse) page 4 - Banners

        private void Save_Page_4_Postback(NameValueCollection Form)
        {
 
        }

        private void Add_Page_4(TextWriter Output)
        {
 
        }

        private void Finish_Page_4(TextWriter Output)
        {

        }

        #endregion

        #region Methods to add file upload controls to the page

        /// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("File_Managament_MySobekViewer.Add_Controls", String.Empty);

            switch (page)
            {
                case 4:
                    add_upload_controls(MainPlaceHolder, ".gif", skinDirectory, "banner.gif", Tracer);
                    break;
            }
        }

        private void add_upload_controls(PlaceHolder UploadFilesPlaceHolder, string FileExtensions, string UploadDirectory, string ServerSideName, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("File_Managament_MySobekViewer.add_upload_controls", String.Empty);

            // Ensure the directory exists
            if (!File.Exists(UploadDirectory))
                Directory.CreateDirectory(UploadDirectory);

            StringBuilder filesBuilder = new StringBuilder(2000);

            LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
            UploadFilesPlaceHolder.Controls.Add(filesLiteral2);
            filesBuilder.Remove(0, filesBuilder.Length);

            UploadiFiveControl uploadControl = new UploadiFiveControl();
            uploadControl.UploadPath = UploadDirectory;
            uploadControl.UploadScript = RequestSpecificValues.Current_Mode.Base_URL + "UploadiFiveFileHandler.ashx";
            uploadControl.AllowedFileExtensions = FileExtensions;
            uploadControl.SubmitWhenQueueCompletes = true;
            uploadControl.RemoveCompleted = true;
            uploadControl.Multi = false;
            uploadControl.ServerSideFileName = ServerSideName;
            UploadFilesPlaceHolder.Controls.Add(uploadControl);

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            UploadFilesPlaceHolder.Controls.Add(literal1);
        }

        #endregion
    }
}
