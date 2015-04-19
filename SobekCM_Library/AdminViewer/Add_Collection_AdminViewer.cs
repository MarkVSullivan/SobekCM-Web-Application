using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Client;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Message;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Library.UploadiFive;
using SobekCM.Tools;
using SobekCM.UI_Library;

namespace SobekCM.Library.AdminViewer
{
    public class Add_Collection_AdminViewer : abstract_AdminViewer
    {
        private readonly int page;
        private string actionMessage;
        private string userInProcessDirectory;
        private string userInProcessUrl;

        private readonly New_Aggregation_Arguments newAggr;

        /// <summary> Constructor for a new instance of the Add_Collection_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public Add_Collection_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Add_Collection_AdminViewer.Constructor", String.Empty);

            // Set some defaults
            actionMessage = String.Empty;
            string page_code = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;

            // If the user is not logged in, they shouldn't be here
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn ))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }
            
            // Was there a parent indicated?
            string parent_locked = String.Empty;
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["parent"]))
            {
                parent_locked = HttpContext.Current.Request.QueryString["parent"];

                // Ensure that aggregation exists
                if (UI_ApplicationCache_Gateway.Aggregations[parent_locked.ToUpper()] == null)
                {
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    return;
                }
            }

            // Load the new aggregation, either currenlty from the session (if already into this wizard )
            // or by building the new aggregation arguments
            New_Aggregation_Arguments cachedInstance = HttpContext.Current.Session["Add_Coll_Wizard"] as New_Aggregation_Arguments;
            newAggr = cachedInstance ?? new New_Aggregation_Arguments("ALL");

            // Lock the parent?
            if (parent_locked.Length > 0)
            {
                // If not already locked, use ths as the parent
                if ((!newAggr.ParentLocked.HasValue) || ( !newAggr.ParentLocked.Value ))
                {
                    newAggr.ParentLocked = true;
                    newAggr.ParentCode = parent_locked;

                    // Also, determine the initial type based on this
                    // Get the type abbreviation
                    Item_Aggregation_Related_Aggregations parentAggr = UI_ApplicationCache_Gateway.Aggregations[newAggr.ParentCode.ToUpper()];
                    newAggr.Type = "Collection";
                    if (parentAggr.Type.IndexOf("Institution", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        newAggr.Type = "Institutional Division";
                    }
                    else if (parentAggr.Code.ToLower() == "all")
                    {
                        newAggr.Type = "Collection Group";
                    }
                    else if (parentAggr.Type.ToLower() == "collection")
                    {
                        newAggr.Type = "SubCollection";
                    }
                    else if (parentAggr.Type.ToLower() == "collection")
                    {
                        newAggr.Type = "SubCollection";
                    }
                }
            }

            // Check for permissions (if not sys or portal admin)
            if ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin))
            {
                // If the parent was locked, this could just be a collection curator/admin
                if ((newAggr.ParentLocked.HasValue) && (!newAggr.ParentLocked.Value))
                {
                    if (!RequestSpecificValues.Current_User.Is_Aggregation_Curator(newAggr.ParentCode))
                    {
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        return;
                    }
                }
                else
                {
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    return;
                }
            }

            // Determine the page
            page = 0;
            if (page_code == "a")
                page = 1;
            if (page_code == "b")
                page = 2;
            else if (page_code == "c")
                page = 3;
            else if (page_code == "d")
                page = 4;
            else if (page_code == "e")
                page = 5;
            else if (page_code == "w")
                page = 0;

            // If this was set to page 0, but the user has chosen not to see that again,
            // move straight onto page 1
            if ((page == 0 ) && ( Convert.ToBoolean(RequestSpecificValues.Current_User.Get_Setting("Add_Collection_AdminViewer:Skip Welcome", "false"))))
            {
                page = 1;
            }

            // Determine the in process directory for this
            if (( !String.IsNullOrEmpty(RequestSpecificValues.Current_User.ShibbID)) && ( RequestSpecificValues.Current_User.ShibbID.Trim().Length > 0 ))
            {
                userInProcessDirectory = Path.Combine(UI_ApplicationCache_Gateway.Settings.In_Process_Submission_Location, RequestSpecificValues.Current_User.ShibbID + "\\addcoll");
                userInProcessUrl = Path.Combine(UI_ApplicationCache_Gateway.Settings.Application_Server_URL, "mySobek/InProcess", RequestSpecificValues.Current_User.ShibbID, "addcoll").Replace("\\","/");
            }
            else
            {
                userInProcessDirectory = Path.Combine(UI_ApplicationCache_Gateway.Settings.In_Process_Submission_Location, RequestSpecificValues.Current_User.UserName.Replace(".", "").Replace("@", "") + "\\addcoll");
                userInProcessUrl = Path.Combine(UI_ApplicationCache_Gateway.Settings.Application_Server_URL, "mySobek/InProcess", RequestSpecificValues.Current_User.UserName.Replace(".", "").Replace("@", ""), "addcoll").Replace("\\", "/"); 
            }

            // If this is a postback, handle any events first
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    string save_value = form["admin_wizard_save"].ToUpper().Trim();
                    string new_aggregation_code = String.Empty;
                    if (form["admin_aggr_code"] != null)
                        new_aggregation_code = form["admin_aggr_code"].ToUpper().Trim();

                    // Get the curret action
                    string action = form["admin_wizard_save"];

                    // If no action, then we should return to the current tab page
                    if (action.Length == 0)
                        action = page_code;

                    // If this is to cancel, handle that here; no need to handle post-back from the
                    // editing form page first
                    if (action == "z")
                    {
                        // Clear the add collection wizard info from the sessions
                        HttpContext.Current.Session["Add_Coll_Wizard"] = null;

                        // Delete all the files
                        if (Directory.Exists(userInProcessDirectory + "\\images\\banners"))
                        {
                            string[] banner_files = SobekCM_File_Utilities.GetFiles(userInProcessDirectory + "\\images\\banners", "*.jpg|*.bmp|*.gif|*.png");
                            foreach (string thisFile in banner_files)
                            {
                                try
                                {
                                    File.Delete(thisFile);
                                }
                                catch { }
                            }
                            string[] button_files = SobekCM_File_Utilities.GetFiles(userInProcessDirectory + "\\images\\buttons", "*.gif");
                            foreach (string thisFile in button_files)
                            {
                                try
                                {
                                    File.Delete(thisFile);
                                }
                                catch { }
                            }
                        }

                        // Redirect the user to the aggregation mgmt screen or parent collection
                        if ((newAggr.ParentLocked.HasValue) && (newAggr.ParentLocked.Value) && (!String.IsNullOrEmpty(newAggr.ParentCode)))
                        {
                            // This was from a parent collection, so go back to that
                            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregation_Single;
                            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = newAggr.ParentCode + "/h";
                        }
                        else
                        {
                            // Send to the main aggregation admin screen
                            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
                            
                        }
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        return;
                    }

                    // Save the returned values, depending on the page
                    switch (page)
                    {
                        case 1:
                            Save_Page_Basic_Postback(form);
                            break;

                        case 2:
                            Save_Page_Visibility_Postback(form);
                            break;

                        case 3:
                            Save_Page_Banner_Postback(form);
                            break;

                        case 4:
                            Save_Page_Buttons_Postback(form);
                            break;

                        case 5:
                            Save_Page_Success_Postback(form);
                            break;

                        case 0:
                            Save_Page_Welcome_Postback(form);
                            break;
                    }

                    // Save the changes to the session
                    HttpContext.Current.Session["Add_Coll_Wizard"] = newAggr;

                    // If there was an error message, than do not go on
                    if (actionMessage.Length > 0)
                    {
                        return;
                    }

                    // If there was a save value continue to pull the rest of the data
                    if (action == "save")
                    {
                        // Some final validation
                        // Convert to the integer id for the parent and begin to do checking
                        List<string> errors = new List<string>();
                        if (String.IsNullOrEmpty(newAggr.ParentCode))
                        {
                            errors.Add("You must select a PARENT for this new aggregation");
                        }

                        // Validate the code
                        if (newAggr.Code.Length > 20)
                        {
                            errors.Add("New aggregation code must be twenty characters long or less");
                        }
                        else if (newAggr.Code.Length == 0)
                        {
                            errors.Add("You must enter a CODE for this item aggregation");
                        }
                        else if (UI_ApplicationCache_Gateway.Aggregations[newAggr.Code.ToUpper()] != null)
                        {
                            errors.Add("New code must be unique... <i>" + newAggr.Code + "</i> already exists");
                        }
                        else if (UI_ApplicationCache_Gateway.Settings.Reserved_Keywords.Contains(newAggr.Code.ToLower()))
                        {
                            errors.Add("That code is a system-reserved keyword.  Try a different code.");
                        }
                        else
                        {
                            bool alphaNumericTest = newAggr.Code.All(C => Char.IsLetterOrDigit(C) || C == '_' || C == '-');
                            if (!alphaNumericTest)
                            {
                                errors.Add("New aggregation code must be only letters and numbers");
                                newAggr.Code = newAggr.Code.Replace("\"", "");
                            }
                        }

                        // Was there a type and name
                        if (newAggr.Type.Length == 0)
                        {
                            errors.Add("You must select a TYPE for this new aggregation");
                        }
                        if (newAggr.Description.Length == 0)
                        {
                            errors.Add("You must enter a DESCRIPTION for this new aggregation");
                        }
                        if (newAggr.Name.Length == 0)
                        {
                            errors.Add("You must enter a NAME for this new aggregation");
                        }
                        else
                        {
                            if (newAggr.ShortName.Length == 0)
                                newAggr.ShortName = newAggr.Name;
                        }

                        // If there were errors copy those over to the action message
                        if (errors.Count > 0)
                        {
                            // Create the error message
                            actionMessage = "ERROR: Invalid entry for new item aggregation<br />";
                            foreach (string error in errors)
                                actionMessage = actionMessage + "<br />" + error;
                            return;
                        }

                        // Try to add this aggregation
                        ErrorRestMessage msg = SobekEngineClient.Aggregations.Add_New_Aggregation(newAggr);

                        if (msg.ErrorType == ErrorRestType.Successful)
                        {
                            // Clear all aggregation information (and thematic heading info) from the cache as well
                            CachedDataManager.Aggregations.Clear();

                            // Delete all the files
                            if (Directory.Exists(userInProcessDirectory + "\\images\\banners"))
                            {
                                string[] banner_files = SobekCM_File_Utilities.GetFiles(userInProcessDirectory + "\\images\\banners", "*.jpg|*.bmp|*.gif|*.png");
                                foreach (string thisFile in banner_files)
                                {
                                    try
                                    {
                                        File.Delete(thisFile);
                                    }
                                    catch { }
                                }
                                string[] button_files = SobekCM_File_Utilities.GetFiles(userInProcessDirectory + "\\images\\buttons", "*.gif");
                                foreach (string thisFile in button_files)
                                {
                                    try
                                    {
                                        File.Delete(thisFile);
                                    }
                                    catch { }
                                }
                            }

                            // If this included a new thematic heading, repopulate that
                            if ((newAggr.NewThematicHeading.HasValue) && (newAggr.NewThematicHeading.Value))
                            {
                                // For thread safety, lock the thematic headings list
                                lock (UI_ApplicationCache_Gateway.Thematic_Headings)
                                {
                                    // Repopulate the thematic headings list
                                    Engine_Database.Populate_Thematic_Headings(UI_ApplicationCache_Gateway.Thematic_Headings, RequestSpecificValues.Tracer);
                                }
                            }

                            // Redirect the user to the new aggregation or parent collection admin
                            if ((newAggr.ParentLocked.HasValue) && (newAggr.ParentLocked.Value) && (!String.IsNullOrEmpty(newAggr.ParentCode)))
                            {
                                // This was from a parent collection, so go back to that
                                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregation_Single;
                                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = newAggr.ParentCode + "/h";
                            }
                            else
                            {
                                // Forward to the aggregation
                                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                                RequestSpecificValues.Current_Mode.Aggregation = newAggr.Code;
                            }

                            // Clear the add collection wizard info from the sessions
                            HttpContext.Current.Session["Add_Coll_Wizard"] = null;

                            UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                            return;
                        }
                        else
                        {
                            actionMessage = msg.Message;
                        }
                    }
                    else
                    {
                        RequestSpecificValues.Current_Mode.My_Sobek_SubMode = action;
                        HttpContext.Current.Response.Redirect(UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode), false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        RequestSpecificValues.Current_Mode.Request_Completed = true;
                    }
                }
                catch ( Exception ee )
                {
                    actionMessage = "General error while reading postback information";
                }
            }
        }

        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum> {HtmlSubwriter_Behaviors_Enum.Suppress_Banner, HtmlSubwriter_Behaviors_Enum.Use_Jquery_DataTables}; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Skins' </value>
        public override string Web_Title
        {
            get { return "Add a Collection Wizard"; }
        }

        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.Wizard_Img; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Add_Collection_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form before any controls are placed in the main place holder </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Add_Collection_AdminViewer.Write_ItemNavForm_Opening", "Add the majority of the HTML before the placeholder");

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Ui_1_10_3_Custom_Js + "\"></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_wizard_save\" name=\"admin_wizard_save\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");
           // Output.WriteLine("<script src=\"http://localhost:52468/default/js/sobekcm-admin/4.9.0/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div id=\"sbkAcw_PageContainer\">");

            // Display any action/error message
            if (actionMessage.Length > 0)
            {
                if (actionMessage.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageError\">" + actionMessage + "</div>");
                }
                else
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageSuccess\">" + actionMessage + "</div>");
                }
            }

            Output.WriteLine("  <br />");
            Output.WriteLine("  <div class=\"sbkAdm_TitleDiv\">");
            // Add the buttons
            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("  <div class=\"sbkAdm_TitleDivButtons\">");

            if (page <= 1)
            {
                Output.WriteLine("    <button title=\"Cancel this new collection\" class=\"sbkAdm_RoundButton\" onclick=\"return new_wizard_edit_page('z');\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            }
            else if ( page < 5 )
            {
                Output.WriteLine("    <button title=\"Back to the previous page of the add new collection wizard\" class=\"sbkAdm_RoundButton\" onclick=\"return new_wizard_edit_page('" + page_to_char(page -1 ) + "');\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> BACK</button> &nbsp; &nbsp; ");
            }

            if (page < 4)
            {
                Output.WriteLine("    <button title=\"Next page of the add new collection wizard\" class=\"sbkAdm_RoundButton\" onclick=\"new_wizard_edit_page('" + page_to_char(page + 1) + "');\">NEXT <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            }
            else if ( page == 4 )
            {
                Output.WriteLine("    <button title=\"Save changes to this item Aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"new_wizard_edit_page('save');\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            }
            else if (page == 5)
            {
                Output.WriteLine("    <button title=\"View your new item aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"new_wizard_edit_page(" + page_to_char(page + 1) + ");\">VIEW NEW COLLECTION <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            }

            Output.WriteLine("  </div>");
            Output.WriteLine();
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;

            Output.WriteLine("     <img id=\"sbkAdm_TitleDivImg\" src=\"" + Static_Resources.Wizard_Img + "\" alt=\"\" />");
            Output.WriteLine("    <h1>Add New Collection Wizard</h1>");
            Output.WriteLine("  </div>");
            Output.WriteLine();

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs\">");

 
            // Add the single tab.  When users click on a tab, it goes back to the server (here)
            // to render the correct tab content
            Output.WriteLine("    <div class=\"tabscontent\">");
            Output.WriteLine("    	<div class=\"tabpage\" id=\"tabpage_1\">");


            switch (page)
            {
                case 1:
                    Add_Page_Basic(Output);
                    break;

                case 2:
                    Add_Page_Visibility(Output);
                    break;

                case 3:
                    Add_Page_Banner(Output);
                    break;

                case 4:
                    Add_Page_Buttons(Output);
                    break;

                case 5:
                    Add_Page_Success(Output);
                    break;

                case 0:
                    Add_Page_Welcome(Output);
                    break;
            }
        }

        private char page_to_char(int page_as_int)
        {
            switch (page_as_int)
            {
                case 0:
                    return 'w';
                case 1:
                    return 'a';
                case 2:
                    return 'b';
                case 3:
                    return 'c';
                case 4:
                    return 'd';
                case 5:
                    return 'e';
            }

            return 'x';
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Add_Collection_AdminViewer.Write_ItemNavForm_Closing", "Add any html after the placeholder and close tabs");

            switch (page)
            {
                case 3:
                    Finish_Page_Banner(Output);
                    break;

                case 4:
                    Finish_Page_Buttons(Output);
                    break;


            }


            Output.WriteLine("    </div>");
            Output.WriteLine("  </div>");
            Output.WriteLine("</div>");
            Output.WriteLine("<br />");
        }


        #region Methods to render (and parse) page 0 - Welcome

        private void Save_Page_Welcome_Postback(NameValueCollection Form)
        {
            // Do nothing
            bool do_not_show_flag = (Form["admin_wizard_donotshow"] != null);
            if (do_not_show_flag)
            {
                RequestSpecificValues.Current_User.Add_Setting("Add_Collection_AdminViewer:Skip Welcome", "true");
                Library.Database.SobekCM_Database.Set_User_Setting(RequestSpecificValues.Current_User.UserID, "Add_Collection_AdminViewer:Skip Welcome", "true");
            }
        }

        private void Add_Page_Welcome(TextWriter Output)
        {
            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkAcw_WelcomeRow\"><td colspan=\"2\">Welcome to the Add New Collection Wizard</td></tr>");

            Output.WriteLine("  <tr class=\"sbkAcw_WelcomeTextRow\">");
            Output.WriteLine("    <td colspan=\"2\">");
            Output.WriteLine("      <p>Welcome to the <span style=\"font-style: italic;\">Add New Collection Wizard</span>.  The next four screens will step you through the process of adding a new collection to your digital repository and provide plenty of help along the way.</p>");
            Output.WriteLine("      <p>Before you get started, you will want to gather some information about your new collection.  During this process, you will be asked for the name of the collection, the identifying code, and some basic descriptive information.  You can also determine where this collection appears within your instance.  It can appear on the home page, on the parent collection, or it can be fairly hidden.</p>");
            Output.WriteLine("      <p>You may want to create some of the imagery for your new collection before you get started as well.  You can upload the collection button, which is a 50 pixels by 50 pixels GIF file and appears on the home page or parent collection.  You can also upload an aggregation banner, which will appear above the search box and provide some collection branding to the home page and all other related collection pages.  You may want to look at existing banners to decide the dimensions you would like to use.   Banners can be uploaded as a jpeg, gif, png, or bmp file.</p>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  <tr class=\"sbkAcw_TextRow\">");
            Output.WriteLine("    <td colspan=\"2\"><p><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_wizard_donotshow\" id=\"admin_wizard_donotshow\" /> <label for=\"admin_wizard_donotshow\">Do not show again</label></p></td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
        }

        #endregion

        #region Methods to render (and parse) page 1 - Basic Information

        private void Save_Page_Basic_Postback(NameValueCollection form)
        {
            // Pull the values from the submitted form
            string new_aggregation_code = form["admin_aggr_code"];
            string new_type = form["admin_aggr_type"];
            string new_parent = String.Empty;
            bool parent_locked = true;
            string new_name = form["admin_aggr_name"].Trim();
            string new_shortname = form["admin_aggr_shortname"].Trim();
            string new_description = form["admin_aggr_desc"].Trim();
            string new_link = form["admin_aggr_link"].Trim();


            // Convert to the integer id for the parent and begin to do checking
            List<string> errors = new List<string>();


            if ((!newAggr.ParentLocked.HasValue) || (!newAggr.ParentLocked.Value))
            {
                new_parent = form["admin_aggr_parent"].Trim();
                parent_locked = false;
                if (String.IsNullOrEmpty(new_parent))
                {
                    errors.Add("You must select a PARENT for this new aggregation");
                }
            }

            // Validate the code
            if (new_aggregation_code.Length > 20)
            {
                errors.Add("New aggregation code must be twenty characters long or less");
            }
            else if (new_aggregation_code.Length == 0)
            {
                errors.Add("You must enter a CODE for this item aggregation");
            }
            else if (UI_ApplicationCache_Gateway.Aggregations[new_aggregation_code.ToUpper()] != null)
            {
                errors.Add("New code must be unique... <i>" + new_aggregation_code + "</i> already exists");
            }
            else if (UI_ApplicationCache_Gateway.Settings.Reserved_Keywords.Contains(new_aggregation_code.ToLower()))
            {
                errors.Add("That code is a system-reserved keyword.  Try a different code.");
            }
            else
            {
                bool alphaNumericTest = new_aggregation_code.All(C => Char.IsLetterOrDigit(C) || C == '_' || C == '-');
                if (!alphaNumericTest)
                {
                    errors.Add("New aggregation code must be only letters and numbers");
                    new_aggregation_code = new_aggregation_code.Replace("\"", "");
                }
            }

            // Was there a type and name
            if (new_type.Length == 0)
            {
                errors.Add("You must select a TYPE for this new aggregation");
            }
            if (new_description.Length == 0)
            {
                errors.Add("You must enter a DESCRIPTION for this new aggregation");
            }
            if (new_name.Length == 0)
            {
                errors.Add("You must enter a NAME for this new aggregation");
            }
            else
            {
                if (new_shortname.Length == 0)
                    new_shortname = new_name;
            }


            // Get the correct type
            string correct_type = "Collection";
            switch (new_type)
            {
                case "coll":
                    correct_type = "Collection";
                    break;

                case "group":
                    correct_type = "Collection Group";
                    break;

                case "subcoll":
                    correct_type = "SubCollection";
                    break;

                case "inst":
                    correct_type = "Institution";
                    break;

                case "exhibit":
                    correct_type = "Exhibit";
                    break;

                case "subinst":
                    correct_type = "Institutional Division";
                    break;
            }


            // Make sure inst and subinst start with 'i'
            if ((new_type.IndexOf("inst") >= 0) && (new_aggregation_code.Length > 0))
            {
                if (new_aggregation_code[0] == 'I')
                    new_aggregation_code = "i" + new_aggregation_code.Substring(1);
                if (new_aggregation_code[0] != 'i')
                    new_aggregation_code = "i" + new_aggregation_code;
            }



            // Create the new aggregation argument object
            newAggr.Code = new_aggregation_code;
            newAggr.Description = new_description;
            newAggr.External_Link = new_link;
            newAggr.Name = new_name;
            if (!parent_locked)
            {
                if (new_parent.Length > 1)
                    newAggr.ParentCode = new_parent.Substring(new_parent.IndexOf("|") + 1);
            }
            newAggr.ShortName = new_shortname;
            newAggr.Type = correct_type;
            newAggr.User = RequestSpecificValues.Current_User.Full_Name;

            // If there were errors copy those over to the action message
            if (errors.Count > 0)
            {
                // Create the error message
                actionMessage = "ERROR: Invalid entry for new item aggregation<br />";
                foreach (string error in errors)
                    actionMessage = actionMessage + "<br />" + error;
            }
        }

        private void Add_Page_Basic(TextWriter Output)
        {
            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkAcw_TitleRow\"><td colspan=\"3\">Basic Information &nbsp; &nbsp; ( step 1 of 4 )</td></tr>");

            // Add the new aggregation code
            Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width:140px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_code\">Aggregation Code:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <input class=\"sbkAcw_small_input sbkAdmin_Focusable\" name=\"admin_aggr_code\" id=\"admin_aggr_code\" type=\"text\" value=\"" + newAggr.Code + "\" />");
            Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Enter a unique collection code of twenty digits or less.  You may use characters, numbers, underscores, or dashes.  This code will appear in the URL when the collection is viewed, so try to make it meaningful for both your users and search engines.</div>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the full name line
            Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_name\">Name (full):</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <input class=\"sbkAcw_large_input sbkAdmin_Focusable\" name=\"admin_aggr_name\" id=\"admin_aggr_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newAggr.Name) + "\" onchange=\"if ( $('#admin_aggr_shortname').val().length == 0 ) { $('#admin_aggr_shortname').val(this.value); }\" />");
            Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Enter the full name for this new collection.  This will be used throughout the system to identify this collection.  The only place this will not appear is in the breadcrumbs, where the shorter version below will be used.</div>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the short name line
            Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_shortname\">Name (short):</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <input class=\"sbkAcw_medium_input sbkAdmin_Focusable\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newAggr.ShortName) + "\" onchange=\"if ( $('#admin_aggr_name').val().length == 0 ) { $('#admin_aggr_name').val(this.value); }\" />");
            Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Enter a shorter version of the name to be used in the breadcrumbs.  Generally, try to keep this as short as possible, as items may appear in multiple collections.</div>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the description box
            Output.WriteLine("  <tr class=\"sbkAcw_TallRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Description:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <textarea class=\"sbkAcw_input sbkAdmin_Focusable\" rows=\"6\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\">" + HttpUtility.HtmlEncode(newAggr.Description) + "</textarea>");
            Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Enter a brief description of this new collection.  This description is public and will appear wherever the collection appears, such as under the thematic headings on the home page or as a subcollection under the parent collection(s).</div>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the parent line
            Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_parent\">Parent:</label></td>");
            Output.WriteLine("    <td>");
            if ((newAggr.ParentLocked.HasValue) && (newAggr.ParentLocked.Value) && (!String.IsNullOrEmpty(newAggr.ParentCode)))
            {
                Item_Aggregation_Related_Aggregations parentAggr = UI_ApplicationCache_Gateway.Aggregations[newAggr.ParentCode.ToUpper()];

                // For institutions, this retains the initial i as lower case (easier to recognize)
                string aggrCode = parentAggr.Code;
                if ((aggrCode[0] == 'I') && (parentAggr.Type.IndexOf("Institution", StringComparison.InvariantCultureIgnoreCase) >= 0))
                {
                    aggrCode = "i" + aggrCode.Substring(1);
                }

                Output.WriteLine("      <span class=\"sbkAcw_CheckText\">" + aggrCode + " - " + parentAggr.Name + "</span>");

                Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">This is the parent collection to this new aggregation.</div>");

            }
            else
            {
                Output.WriteLine("      <select class=\"sbkAcw_select_large\" name=\"admin_aggr_parent\" id=\"admin_aggr_parent\" onchange=\"if ( this.value.length > 0 ) { $('#admin_aggr_type').val(this.value.substr(0, this.value.indexOf('|'))); if ( this.value.indexOf('subinst') == 0 ) $('#external_link_row').css('display', 'table-row'); else $('#external_link_row').css('display', 'none'); }\">");
                if (newAggr.ParentCode == String.Empty)
                    Output.WriteLine("        <option value=\"\" selected=\"selected\" ></option>");
                foreach (Item_Aggregation_Related_Aggregations thisAggr in UI_ApplicationCache_Gateway.Aggregations.All_Aggregations)
                {
                    // Get the type abbreviation
                    string typeAbbrev = "coll";
                    if (thisAggr.Type.IndexOf("Institution", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        typeAbbrev = "subinst";
                    }
                    else if (thisAggr.Code.ToLower() == "all")
                    {
                        typeAbbrev = "group";
                    }
                    else if (thisAggr.Type.ToLower() == "collection")
                    {
                        typeAbbrev = "subcoll";
                    }
                    else if (thisAggr.Type.ToLower() == "subcollection")
                    {
                        typeAbbrev = "subcoll";
                    }

                    // For institutions, this retains the initial i as lower case (easier to recognize)
                    string aggrCode = thisAggr.Code;
                    if ((aggrCode[0] == 'I') && (thisAggr.Type.IndexOf("Institution", StringComparison.InvariantCultureIgnoreCase) >= 0))
                    {
                        aggrCode = "i" + aggrCode.Substring(1);
                    }

                    // Add this option
                    if (newAggr.ParentCode == thisAggr.Code)
                    {
                        Output.WriteLine("        <option value=\"" + typeAbbrev + "|" + thisAggr.Code + "\" selected=\"selected\" >" + aggrCode + " - " + thisAggr.Name + "</option>");
                    }
                    else
                    {
                        Output.WriteLine("        <option value=\"" + typeAbbrev + "|" + thisAggr.Code + "\" >" + aggrCode + " - " + thisAggr.Name + "</option>");
                    }
                }
                Output.WriteLine("      </select>");
                Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Select the parent collection.  If this should be a top-level collection, select the ALL collection.</div>");
            }
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");


            // Add the aggregation type line
            Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_type\">Type:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <select class=\"sbkAsav_select \" name=\"admin_aggr_type\" id=\"admin_aggr_type\" onchange=\"return aggr_display_external_link(this);\">");

            Output.WriteLine(((newAggr.Type == "Collection") || ( newAggr.Type == String.Empty ))
                ? "        <option value=\"coll\" selected=\"selected\" >Collection</option>"
                : "        <option value=\"coll\">Collection</option>");

            Output.WriteLine(newAggr.Type == "Collection Group"
                ? "        <option value=\"group\" selected=\"selected\" >Collection Group</option>"
                : "        <option value=\"group\">Collection Group</option>");

            Output.WriteLine(newAggr.Type == "Exhibit"
                ? "        <option value=\"exhibit\" selected=\"selected\" >Exhibit</option>"
                : "        <option value=\"exhibit\">Exhibit</option>");

            Output.WriteLine(newAggr.Type == "Institution"
                ? "        <option value=\"inst\" selected=\"selected\" >Institution</option>"
                : "        <option value=\"inst\">Institution</option>");

            Output.WriteLine(newAggr.Type == "Institutional Division"
                ? "        <option value=\"subinst\" selected=\"selected\" >Institutional Division</option>"
                : "        <option value=\"subinst\">Institutional Division</option>");

            Output.WriteLine(newAggr.Type == "SubCollection"
                ? "        <option value=\"subcoll\" selected=\"selected\" >SubCollection</option>"
                : "        <option value=\"subcoll\">SubCollection</option>");

            Output.WriteLine("      </select>");
            Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Select the aggregation type above.  A suggested type will be offered based on the parent collection above, but you can change it.  All these types of aggregations work very similarly, and generally you can choose whichever type helps you make sense of your aggregation hierarchy.  Only aggregations of institutional types can be linked to digital resources as source and holding institutions however.</div>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");




            // Add the link line
            Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\" id=\"external_link_row\" style=\"display:none;\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_link\">External Link:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <input class=\"sbkAcw_large_input sbkAdmin_Focusable\" name=\"admin_aggr_link\" id=\"admin_aggr_link\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newAggr.External_Link) + "\" />");
            Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Institutional collections can have an external link added.  The link will be displayed in the citation of any digital resources associated with this institution, linked to the source institution or holding location text.</div>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Close this table
            Output.WriteLine("</table>");
            Output.WriteLine();
        }

        #endregion

        #region Methods to render (and parse) page 2 - Visibility

        private void Save_Page_Visibility_Postback(NameValueCollection form)
        {
            string new_thematic_heading = form["admin_aggr_heading"].Trim();

            bool is_active = form["admin_aggr_isactive"] != null;
            bool is_hidden = form["admin_aggr_ishidden"] == null;

            // Get the thematic heading id (no checks here)
            string thematicHeading = null;
            if (!String.IsNullOrEmpty(new_thematic_heading))
            {
                int thematicHeadingId = Convert.ToInt32(new_thematic_heading);
                newAggr.NewThematicHeading = null;
                if (thematicHeadingId > 0)
                {
                    foreach (Thematic_Heading thisHeading in UI_ApplicationCache_Gateway.Thematic_Headings)
                    {
                        if (thisHeading.ID == thematicHeadingId)
                        {
                            thematicHeading = thisHeading.Text;
                            break;
                        }
                    }
                }
                else if (thematicHeadingId == -2)
                {
                    string newHeading = form["admin_aggr_newheading"];
                    if (!String.IsNullOrEmpty(newHeading))
 {
                        newAggr.NewThematicHeading = true;
                        thematicHeading = form["admin_aggr_newheading"];
                    }
                }
            }

            // Update the new aggregation argument object
            newAggr.Active = is_active;
            if (newAggr.ParentCode.ToLower() != "all")
            {
                newAggr.Hidden = is_hidden;
            }
            newAggr.Thematic_Heading = thematicHeading;
        }

        private void Add_Page_Visibility(TextWriter Output)
        {
            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkAcw_TitleRow\"><td colspan=\"3\">Visibility for New " + newAggr.Type + " &nbsp; &nbsp; ( step 2 of 4 )</td></tr>");

            // Add the active flag
            Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td style=\"width:140px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_isactive\">Active:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine(newAggr.Active
                ? "      <input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" checked=\"checked\" /> <label for=\"admin_aggr_isactive\" class=\"sbkAcw_CheckText\" >New collection will be active</label>"
                : "      <input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" /> <label for=\"admin_aggr_isactive\" class=\"sbkAcw_CheckText\" >New collection will be active</label>");
            Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Flag indicates if this collection should be active.  Active collections appear in breadcrumbs when you view digital resources and generally appear in all public lists of collections.  You can add items to inactive collections and build the collection prior to &quot;publishing&quot; it later by making it active.</div>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the appear in parent collection flag, if not in the ALL collection
            if (newAggr.ParentCode.ToLower() != "all")
            {
                Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\">");
                Output.WriteLine("    <td>&nbsp;</td>");
                Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_ishidden\">Hidden:</label></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine(!newAggr.Hidden
                    ? "      <input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" checked=\"checked\" /> <label for=\"admin_aggr_ishidden\" class=\"sbkAcw_CheckText\" >Show in parent collection home page</label>"
                    : "      <input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" /> <label for=\"admin_aggr_ishidden\" class=\"sbkAcw_CheckText\" >Show in parent collection home page</label>");

                Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">Flag indicates if this collection should appear in the home page of the parent collection.  In all other respects, a hidden collection works just like an active collection.</div>");
                Output.WriteLine("    </td>");
                Output.WriteLine("  </tr>");
            }

            // Add the thematic heading line
            Output.WriteLine("  <tr class=\"sbkAcw_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_heading\">Thematic Heading:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <select class=\"sbkAsav_select_large\" name=\"admin_aggr_heading\" id=\"admin_aggr_heading\" onchange=\"if ( this.value == -2 ) $('#admin_aggr_newheading').css('display', 'block'); else $('#admin_aggr_newheading').css('display', 'none');\">");

            if ( String.IsNullOrEmpty(newAggr.Thematic_Heading))
                Output.WriteLine("        <option value=\"-1\" selected=\"selected\" ></option>");
            else
                Output.WriteLine("        <option value=\"-1\"></option>");

            if (( newAggr.NewThematicHeading.HasValue ) && ( newAggr.NewThematicHeading.Value ))
                Output.WriteLine("        <option value=\"-2\" selected=\"selected\" >&lt;NEW THEMATIC HEADING&gt;</option>");
            else
                Output.WriteLine("        <option value=\"-2\" >&lt;NEW THEMATIC HEADING&gt;</option>");

            foreach (Thematic_Heading thisHeading in UI_ApplicationCache_Gateway.Thematic_Headings)
            {
                if (thisHeading.Text == newAggr.Thematic_Heading)
                    Output.Write("        <option value=\"" + thisHeading.ID + "\" selected=\"selected\">" + HttpUtility.HtmlEncode(thisHeading.Text) + "</option>");
                else
                    Output.Write("        <option value=\"" + thisHeading.ID + "\">" + HttpUtility.HtmlEncode(thisHeading.Text) + "</option>");
            }
            Output.WriteLine("      </select>");

            if ((newAggr.NewThematicHeading.HasValue) && (newAggr.NewThematicHeading.Value))
                Output.WriteLine("      <input class=\"sbkAcw_large_input sbkAdmin_Focusable\" name=\"admin_aggr_newheading\" id=\"admin_aggr_newheading\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(newAggr.Thematic_Heading) + "\" style=\"display:block;\" />");
            else
                Output.WriteLine("      <input class=\"sbkAcw_large_input sbkAdmin_Focusable\" name=\"admin_aggr_newheading\" id=\"admin_aggr_newheading\" type=\"text\" value=\"\" style=\"display:none;\" />");


            Output.WriteLine("      <div class=\"sbkAcw_InlineHelp\">To make this collection appear on the home page of this repository, you must add it to an existing thematic heading.  You can also add a new thematic heading directly from this wizard, by selecting that option above.  Thematic headings categorize the collections within your repository.</div>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");



            // Close this table
            Output.WriteLine("</table>");
            Output.WriteLine();
        }

        #endregion

        #region Methods to render (and parse) page 3 - Banner

        private void Save_Page_Banner_Postback(NameValueCollection Form)
        {
            string banner_folder = userInProcessDirectory + "\\images\\banners";
            if (!Directory.Exists(banner_folder))
                Directory.CreateDirectory(banner_folder);
            string[] banner_files = SobekCM_File_Utilities.GetFiles(banner_folder, "*.jpg|*.bmp|*.gif|*.png");
            string last_added_banner = String.Empty;
            DateTime lastModDate = DateTime.MinValue;
            foreach (string thisBannerFile in banner_files)
            {
                DateTime thisDate = (new FileInfo(thisBannerFile)).LastWriteTime;
                if (thisDate.CompareTo(lastModDate) > 0)
                {
                    lastModDate = thisDate;
                    last_added_banner = thisBannerFile;
                }
            }
            if (!String.IsNullOrEmpty(last_added_banner))
                newAggr.BannerFile = last_added_banner;
        }

        private void Add_Page_Banner(TextWriter Output)
        {
            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkAcw_TitleRow2\"><td colspan=\"3\">Collection Banner &nbsp; &nbsp; ( step 3 of 4 )</td></tr>");

            Output.WriteLine("  <tr class=\"sbkAcw_TextRow\"><td colspan=\"3\"><p>Upload a banner for this new collection.  Banners provide a constant design element present on all the collection pages.  If you do not upload a banner, one will automatically be created for this collection.  You can always replace the banner, or provide language-specific versions, later through the aggregation admin features.</p></td></tr>");

            // Get list of existing banners
            string banner_folder = userInProcessDirectory + "\\images\\banners";
            if (!Directory.Exists(banner_folder))
                Directory.CreateDirectory(banner_folder);
            string[] banner_files = SobekCM_File_Utilities.GetFiles(banner_folder, "*.jpg|*.bmp|*.gif|*.png");
            string last_added_banner = String.Empty;
            if (HttpContext.Current.Items["Uploaded File"] != null)
            {
                string newBanner = HttpContext.Current.Items["Uploaded File"].ToString();
                last_added_banner = Path.GetFileName(newBanner);
            }
            else
            {
                DateTime lastModDate = DateTime.MinValue;
                foreach (string thisBannerFile in banner_files)
                {
                    DateTime thisDate = (new FileInfo(thisBannerFile)).LastWriteTime;
                    if (thisDate.CompareTo(lastModDate) > 0)
                    {
                        lastModDate = thisDate;
                        last_added_banner = Path.GetFileName(thisBannerFile);
                    }
                }
            }

            if (!String.IsNullOrEmpty(last_added_banner))
            {
                Output.WriteLine("  <tr class=\"sbkAcw_TallRow\">");
                Output.WriteLine("    <td>&nbsp;</td>");
                Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Uploaded Banner:</label></td>");
                Output.WriteLine("    <td>");

                string url = Path.Combine(userInProcessUrl, "images/banners", last_added_banner).Replace("\\","/");
                Output.WriteLine("      <img src=\"" + url + "\" alt=\"Access Denied\" style=\"max-width:500px; border: 1px #888888 solid;\" />");
                Output.WriteLine("    </td>");
                Output.WriteLine("  </tr>");
            }


            Output.WriteLine("  <tr class=\"sbkAcw_TallRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\" style=\"width:170px\"><label for=\"admin_aggr_desc\">Upload New Banner:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("       <table class=\"sbkAcw_InnerTable\">");
            Output.WriteLine("         <tr>");
            Output.WriteLine("           <td class=\"sbkAcw_UploadInstr\">To upload, browse to a GIF, PNG, JPEG, or BMP file, and then select UPLOAD</td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("         <tr>");
            Output.WriteLine("           <td>");




        }

        private void Finish_Page_Banner(TextWriter Output)
        {
            Output.WriteLine("           </td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("       </table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
        }

        #endregion

        #region Methods to render (and parse) page 4 - Button

        private void Save_Page_Buttons_Postback(NameValueCollection Form)
        {
            string button_folder = userInProcessDirectory + "\\images\\buttons";
            if (!Directory.Exists(button_folder))
                Directory.CreateDirectory(button_folder);
            string[] button_files = Directory.GetFiles(button_folder, "*.gif");
            string last_added_button = String.Empty;
            DateTime lastModDate = DateTime.MinValue;
            foreach (string thisButtonFile in button_files)
            {
                DateTime thisDate = (new FileInfo(thisButtonFile)).LastWriteTime;
                if (thisDate.CompareTo(lastModDate) > 0)
                {
                    lastModDate = thisDate;
                    last_added_button = thisButtonFile;
                }
            }
            if (!String.IsNullOrEmpty(last_added_button))
                newAggr.ButtonFile = last_added_button;
        }

        private void Add_Page_Buttons(TextWriter Output)
        {
            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkAcw_TitleRow2\"><td colspan=\"3\">Collection Button &nbsp; &nbsp; ( step 4 of 4 )</td></tr>");

            Output.WriteLine("  <tr class=\"sbkAcw_TextRow\"><td colspan=\"3\"><p>Upload a button for this new collection.  Buttons appear on the home page or parent collection's home page once a collection is active and not hidden.  If you do not upload a button, the system default will be used.  You can always replace the button later through the aggregation admin features.</p></td></tr>");

            // Get list of existing buttons
            string button_folder = userInProcessDirectory + "\\images\\buttons";
            if (!Directory.Exists(button_folder))
                Directory.CreateDirectory(button_folder);
            string[] button_files = Directory.GetFiles(button_folder, "*.gif");
            string last_added_button = String.Empty;
            if (HttpContext.Current.Items["Uploaded File"] != null)
            {
                string newButton = HttpContext.Current.Items["Uploaded File"].ToString();
                last_added_button = Path.GetFileName(newButton);
            }
            else
            {
                DateTime lastModDate = DateTime.MinValue;
                foreach (string thisButtonFile in button_files)
                {
                    DateTime thisDate = (new FileInfo(thisButtonFile)).LastWriteTime;
                    if (thisDate.CompareTo(lastModDate) > 0)
                    {
                        lastModDate = thisDate;
                        last_added_button = Path.GetFileName(thisButtonFile);
                    }
                }
            }

            if (!String.IsNullOrEmpty(last_added_button))
            {
                Output.WriteLine("  <tr class=\"sbkAcw_TallRow\">");
                Output.WriteLine("    <td>&nbsp;</td>");
                Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Current Button:</label></td>");
                Output.WriteLine("    <td>");

                string url = Path.Combine(userInProcessUrl, "images/buttons", last_added_button).Replace("\\", "/");
                Output.WriteLine("      <img src=\"" + url + "\" alt=\"Access Denied\" style=\"border: 1px #888888 solid;\" />");
                Output.WriteLine("    </td>");
                Output.WriteLine("  </tr>");
            }
            else
            {
                string default_button = UI_ApplicationCache_Gateway.Settings.Application_Server_Network + "design\\aggregations\\default_button.gif";
                if (File.Exists(default_button))
                {
                    Output.WriteLine("  <tr class=\"sbkAcw_TallRow\">");
                    Output.WriteLine("    <td>&nbsp;</td>");
                    Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Current Button:</label></td>");
                    Output.WriteLine("    <td>");

                    string url = UI_ApplicationCache_Gateway.Settings.Application_Server_URL + "design/aggregations/default_button.gif";
                    Output.WriteLine("      <img src=\"" + url + "\" alt=\"Access Denied\" style=\"border: 1px #888888 solid;\" />");
                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>"); 
                }
            }


            Output.WriteLine("  <tr class=\"sbkAcw_TallRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\" style=\"width:170px\"><label for=\"admin_aggr_desc\">Upload New Button:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("       <table class=\"sbkAcw_InnerTable\">");
            Output.WriteLine("         <tr>");
            Output.WriteLine("           <td class=\"sbkAcw_UploadInstr\">To change, browse to a 50x50 pixel GIF file, and then select UPLOAD</td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("         <tr>");
            Output.WriteLine("           <td>");

        }

        private void Finish_Page_Buttons(TextWriter Output)
        {
            Output.WriteLine("           </td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("       </table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
        }

        #endregion



        #region Methods to render (and parse) page 5 - Success

        private void Save_Page_Success_Postback(NameValueCollection Form)
        {
            // Do nothing
        }

        private void Add_Page_Success(TextWriter Output)
        {
            Output.WriteLine("SUCCESS!!");
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
                case 3:
                    string code = newAggr.Code;
                    string new_file = code;
                    int next_decimal = 65;
                    if (!Directory.Exists(userInProcessDirectory + "\\images\\banners"))
                        Directory.CreateDirectory(userInProcessDirectory + "\\images\\banners");
                    while (Directory.GetFiles(userInProcessDirectory + "\\images\\banners\\", new_file + ".*").Length > 0 )
                    {
                        new_file = code + "_" + Convert.ToChar(next_decimal++);
                    }
                    add_upload_controls(MainPlaceHolder, ".gif,.bmp,.jpg,.png,.jpeg", userInProcessDirectory + "\\images\\banners", new_file, false, Tracer);
                    break;

                case 4:
                    string code2 = newAggr.Code;
                    string new_file2 = code2;
                    int next_decimal2 = 65;
                    if (!Directory.Exists(userInProcessDirectory + "\\images\\buttons"))
                        Directory.CreateDirectory(userInProcessDirectory + "\\images\\buttons");
                    while (Directory.GetFiles(userInProcessDirectory + "\\images\\buttons\\", new_file2 + ".*").Length > 0 )
                    {
                        new_file2 = code2 + "_" + Convert.ToChar(next_decimal2++);
                    }
                    add_upload_controls(MainPlaceHolder, ".gif", userInProcessDirectory + "\\images\\buttons", new_file2 + ".gif", false, Tracer);
                    break;
            }
        }

        private void add_upload_controls(PlaceHolder UploadFilesPlaceHolder, string FileExtensions, string UploadDirectory, string ServerSideName, bool UploadMultiple, Custom_Tracer Tracer)
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
            uploadControl.Multi = UploadMultiple;
            uploadControl.ServerSideFileName = ServerSideName;
            UploadFilesPlaceHolder.Controls.Add(uploadControl);

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            UploadFilesPlaceHolder.Controls.Add(literal1);
        }

        #endregion


    }
}
