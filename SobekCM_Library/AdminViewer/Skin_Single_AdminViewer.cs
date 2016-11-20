#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.Skins;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Engine_Library.Skins;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UI;
using SobekCM.Library.UploadiFive;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated aggregation admin to edit information related to a single item aggregation. </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to view and edit information related to a single item aggregation</li>
    /// </ul></remarks>
    public class Skin_Single_AdminViewer : abstract_AdminViewer
    {
        private string actionMessage;
        private readonly string skinDirectory;
        private readonly Complete_Web_Skin_Object webSkin;
        private readonly Dictionary<string, string> updatedSourceFiles;

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

            string page_code = "a";
            if (code.Contains("/"))
            {
                string[] parser = code.Split("/".ToCharArray());
                code = parser[0];
                page_code = parser[1];
            }

            // Determine the page
            page = 1;
            if (page_code == "b")
                page = 2;
            else if (page_code == "c")
                page = 3;
            else if (page_code == "d")
                page = 4;
            else if (page_code == "e")
                page = 5;

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
            Complete_Web_Skin_Object cachedInstance = HttpContext.Current.Session["Edit_Skin_" + code + "|object"] as Complete_Web_Skin_Object;
            webSkin = cachedInstance ?? SobekEngineClient.WebSkins.Get_Complete_Web_Skin(code, RequestSpecificValues.Tracer);

            // If unable to retrieve this skin, send to home
            if (webSkin == null)
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Get the dictionary for updated source files
            updatedSourceFiles = HttpContext.Current.Session["Edit_Skin_" + code + "|files"] as Dictionary<string, string> ?? new Dictionary<string, string>();

            // Get the skin directory and ensure it exists
            skinDirectory = HttpContext.Current.Server.MapPath("design/skins/" + webSkin.Skin_Code);
            if (!Directory.Exists(skinDirectory))
                Directory.CreateDirectory(skinDirectory);

            // If this is a postback, handle any events first
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    // Get the curret action
                    string action = form["admin_skin_save"];

                    // If no action, then we should return to the current tab page
                    if (action.Length == 0)
                        action = page_code;

                    // If this is to cancel, handle that here; no need to handle post-back from the
                    // editing form page first
                    if (action == "z")
                    {
                        // Clear the aggregedit web skin info from the sessions
                        HttpContext.Current.Session["Edit_Skin_" + webSkin.Skin_Code + "|object"] = null;
                        HttpContext.Current.Session["Edit_Skin_" + webSkin.Skin_Code + "|files"] = null;

                        // Redirect the user to the skins mgmt screen
                        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
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

                        case 5:
                            Save_Page_Uploads_Postback(form);
                            break;
                    }

                    // Should this be saved to the database?
                    if ((action == "save") || (action == "save_exit"))
                    {
                        // Save this existing interface
                        bool successful_save = SobekCM_Database.Save_Web_Skin(webSkin.Skin_Code, webSkin.Base_Skin_Code, webSkin.Override_Banner, true, webSkin.Banner_Link, webSkin.Notes, webSkin.Suppress_Top_Navigation, RequestSpecificValues.Tracer);

                        // Prepare the backup folders
                        string backup_folder = skinDirectory + "\\backup\\html";
                        if (!Directory.Exists(backup_folder))
                            Directory.CreateDirectory(backup_folder);
                        string css_backup_folder = skinDirectory + "\\backup\\css";
                        if (!Directory.Exists(css_backup_folder))
                            Directory.CreateDirectory(css_backup_folder);

                        // Also, save all the updated files
                        foreach (KeyValuePair<string, string> pairs in updatedSourceFiles)
                        {
                            try
                            {
                                if (pairs.Key != "CSS")
                                {
                                    string filename = Path.Combine(skinDirectory, pairs.Key);
                                    if (String.IsNullOrEmpty(pairs.Value))
                                    {
                                        if (File.Exists(filename))
                                        {
                                            // Use the last modified date as the name of the backup
                                            DateTime lastModifiedDate = (new FileInfo(filename)).LastWriteTime;
                                            string backup_name = Path.GetFileName(filename).Replace(Path.GetExtension(filename), "") + lastModifiedDate.Year + lastModifiedDate.Month.ToString().PadLeft(2, '0') + lastModifiedDate.Day.ToString().PadLeft(2, '0') + lastModifiedDate.Hour.ToString().PadLeft(2, '0') + lastModifiedDate.Minute.ToString().PadLeft(2, '0') + Path.GetExtension(filename);
                                            if (!File.Exists(backup_folder + "\\" + backup_name))
                                                File.Copy(filename, backup_folder + "\\" + backup_name, false);

                                            // Now, delete the file
                                            File.Delete(filename);
                                        }
                                    }
                                    else
                                    {
                                        string fullName = Path.Combine(skinDirectory, filename);
                                        if (File.Exists(fullName))
                                        {
                                            StreamReader reader = new StreamReader(fullName);
                                            string current_contents = reader.ReadToEnd();
                                            reader.Close();

                                            if (String.CompareOrdinal(pairs.Value.Replace("[%", "<%").Replace("%]", "%>"), current_contents) != 0)
                                            {
                                                // Use the last modified date as the name of the backup
                                                DateTime lastModifiedDate = (new FileInfo(filename)).LastWriteTime;
                                                string backup_name = Path.GetFileName(filename).Replace(Path.GetExtension(filename), "") + lastModifiedDate.Year + lastModifiedDate.Month.ToString().PadLeft(2, '0') + lastModifiedDate.Day.ToString().PadLeft(2, '0') + lastModifiedDate.Hour.ToString().PadLeft(2, '0') + lastModifiedDate.Minute.ToString().PadLeft(2, '0') + Path.GetExtension(filename);
                                                if (!File.Exists(backup_folder + "\\" + backup_name))
                                                    File.Copy(fullName, backup_folder + "\\" + backup_name, false);

                                                StreamWriter writer = new StreamWriter(fullName, false);
                                                writer.Write(pairs.Value.Replace("[%", "<%").Replace("%]", "%>"));
                                                writer.Flush();
                                                writer.Close();
                                            }
                                        }
                                        else
                                        {
                                            StreamWriter writer = new StreamWriter(fullName, false);
                                            writer.Write(pairs.Value.Replace("[%", "<%").Replace("%]", "%>"));
                                            writer.Flush();
                                            writer.Close();
                                        }
                                    }
                                }
                                else
                                {
                                    string new_skin_file = Path.Combine(skinDirectory, webSkin.CSS_Style);
                                    if (File.Exists(new_skin_file))
                                    {
                                        StreamReader reader = new StreamReader(new_skin_file);
                                        string current_contents = reader.ReadToEnd();
                                        reader.Close();

                                        if (String.CompareOrdinal(pairs.Value, current_contents) != 0)
                                        {
                                            // Use the last modified date as the name of the backup
                                            DateTime lastModifiedDate = (new FileInfo(new_skin_file)).LastWriteTime;
                                            string backup_name = Path.GetFileName(new_skin_file).Replace(Path.GetExtension(new_skin_file), "") + lastModifiedDate.Year + lastModifiedDate.Month.ToString().PadLeft(2, '0') + lastModifiedDate.Day.ToString().PadLeft(2, '0') + lastModifiedDate.Hour.ToString().PadLeft(2, '0') + lastModifiedDate.Minute.ToString().PadLeft(2, '0') + Path.GetExtension(new_skin_file);
                                            if (!File.Exists(css_backup_folder + "\\" + backup_name))
                                                File.Copy(new_skin_file, css_backup_folder + "\\" + backup_name, false);

                                            StreamWriter writer = new StreamWriter(new_skin_file, false);
                                            writer.Write(pairs.Value);
                                            writer.Flush();
                                            writer.Close();
                                        }
                                    }
                                    else
                                    {
                                        StreamWriter writer = new StreamWriter(new_skin_file, false);
                                        writer.Write(pairs.Value.Replace("[%", "<%").Replace("%]", "%>"));
                                        writer.Flush();
                                        writer.Close();
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }


                        lock (UI_ApplicationCache_Gateway.Web_Skin_Collection)
                        {
                            Web_Skin_Utilities.Populate_Default_Skins(UI_ApplicationCache_Gateway.Web_Skin_Collection, RequestSpecificValues.Tracer);
                        }
                        CachedDataManager.WebSkins.Remove_Skin(webSkin.Skin_Code, RequestSpecificValues.Tracer);

                        if (successful_save)
                        {
                            if (action == "save_exit")
                            {
                                // Clear the aggregedit web skin info from the sessions
                                HttpContext.Current.Session["Edit_Skin_" + webSkin.Skin_Code + "|object"] = null;
                                HttpContext.Current.Session["Edit_Skin_" + webSkin.Skin_Code + "|files"] = null;

                                // Redirect the user to the skins mgmt screen
                                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
                                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                            }
                        }
                        else
                        {
                            actionMessage = "Unable to edit existing html skin <i>" + webSkin.Skin_Code + "</i>";
                        }
                    }
                    else
                    {
                        string new_lang = String.Empty;
                        if (action == "new_lang")
                        {
                            action = "c";
                            string new_language = form["webskin_new_lang"];
                            string copy_language = form["webskin_new_lang_copy"];

                            Web_Language_Enum new_language_enum = Web_Language_Enum_Converter.Code_To_Enum(new_language);
                            new_lang = new_language;

                            if (!webSkin.SourceFiles.ContainsKey(new_language_enum))
                            {
                                string language_code = "_" + new_language;

                                Complete_Web_Skin_Source_Files sources = new Complete_Web_Skin_Source_Files();
                                sources.Header_Source_File = "html\\header" + language_code + ".html";
                                sources.Footer_Source_File = "html\\footer" + language_code + ".html";
                                sources.Header_Item_Source_File = "html\\header_item" + language_code + ".html";
                                sources.Footer_Item_Source_File = "html\\footer_item" + language_code + ".html";
                                webSkin.SourceFiles[new_language_enum] = sources;

                                updatedSourceFiles[sources.Header_Source_File] = String.Empty;
                                updatedSourceFiles[sources.Footer_Source_File] = String.Empty;
                                updatedSourceFiles[sources.Header_Item_Source_File] = String.Empty;
                                updatedSourceFiles[sources.Footer_Item_Source_File] = String.Empty;

                                if (!String.IsNullOrEmpty(copy_language))
                                {
                                    Web_Language_Enum copy_language_enum = Web_Language_Enum_Converter.Code_To_Enum(copy_language);
                                    if (copy_language == "def")
                                        copy_language_enum = Web_Language_Enum.DEFAULT;

                                    if (webSkin.SourceFiles.ContainsKey(copy_language_enum))
                                    {
                                        Complete_Web_Skin_Source_Files copy_sources = webSkin.SourceFiles[copy_language_enum];
                                        updatedSourceFiles[sources.Header_Source_File] = get_file_source(copy_sources.Header_Source_File);
                                        updatedSourceFiles[sources.Footer_Source_File] = get_file_source(copy_sources.Footer_Source_File);
                                        updatedSourceFiles[sources.Header_Item_Source_File] = get_file_source(copy_sources.Header_Item_Source_File);
                                        updatedSourceFiles[sources.Footer_Item_Source_File] = get_file_source(copy_sources.Footer_Item_Source_File);
                                    }
                                }



                            }
                        }

                        // Determine the base URL to redirect to
                        RequestSpecificValues.Current_Mode.My_Sobek_SubMode = code + "/" + action;

                        // Delete language support for a single language?
                        if (action == "delete_skin")
                        {
                            string lang = form["admin_skin_language"];
                            if (lang.IndexOf("-") == 0)
                            {
                                Web_Language_Enum langEnum = Web_Language_Enum_Converter.Code_To_Enum(lang.Substring(1));
                                if (webSkin.SourceFiles.ContainsKey(langEnum))
                                {
                                    Complete_Web_Skin_Source_Files sources = webSkin.SourceFiles[langEnum];
                                    webSkin.SourceFiles.Remove(langEnum);
                                    updatedSourceFiles[sources.Header_Source_File] = String.Empty;
                                    updatedSourceFiles[sources.Footer_Source_File] = String.Empty;
                                    updatedSourceFiles[sources.Header_Item_Source_File] = String.Empty;
                                    updatedSourceFiles[sources.Footer_Item_Source_File] = String.Empty;
                                }
                            }

                            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = code + "/c";
                        }

                        // Save the updated info
                        HttpContext.Current.Session["Edit_Skin_" + webSkin.Skin_Code + "|object"] = webSkin;
                        HttpContext.Current.Session["Edit_Skin_" + webSkin.Skin_Code + "|files"] = updatedSourceFiles;

                        string url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                        if (action == "c")
                        {
                            string lang = form["admin_skin_language"];
                            if (new_lang.Length > 0)
                                lang = new_lang;

                            if (!String.IsNullOrEmpty(lang))
                            {
                                if (url.Contains("?"))
                                    url = url + "&lang=" + lang;
                                else
                                    url = url + "?lang=" + lang;
                            }
                        }

                        HttpContext.Current.Response.Redirect(url, false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        RequestSpecificValues.Current_Mode.Request_Completed = true;
                    }
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


        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return String.Empty; }
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
            Output.WriteLine("<input type=\"hidden\" id=\"admin_skin_save\" name=\"admin_skin_save\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_skin_action\" name=\"admin_skin_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_skin_language\" name=\"admin_skin_language\" value=\"\" />");
            Output.WriteLine();

            Tracer.Add_Trace("Skin_Single_AdminViewer.Write_ItemNavForm_Closing", "Add the rest of the form");

            Output.WriteLine("<!-- Users_AdminViewer.Write_ItemNavForm_Closing -->");

            Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");
            Output.WriteLine();

            Output.WriteLine("<div id=\"sbkSaav_PageContainer\">");

            // Add the buttons
            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
            Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_skin_edit_page('z');\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");


            // If this page has CKEDITOR fields, ensure they are all committed before saving
            if (page == 3)
            {
                Output.WriteLine("    <button title=\"Save changes to this web skin\" class=\"sbkAdm_RoundButton\" onclick=\"for(var i in CKEDITOR.instances) { CKEDITOR.instances[i].updateElement(); } return save_skin_edits(false);\"> SAVE </button>  &nbsp; &nbsp; ");
                Output.WriteLine("    <button title=\"Save changes to this web skin and exist the admin screen\" class=\"sbkAdm_RoundButton\" onclick=\"for(var i in CKEDITOR.instances) { CKEDITOR.instances[i].updateElement(); } return save_skin_edits(true);\">SAVE & EXIT <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");

            }
            else
            {
                Output.WriteLine("    <button title=\"Save changes to this web skin\" class=\"sbkAdm_RoundButton\" onclick=\"return save_skin_edits(false);\"> SAVE </button>  &nbsp; &nbsp; ");
                Output.WriteLine("    <button title=\"Save changes to this web skin and exist the admin screen\" class=\"sbkAdm_RoundButton\" onclick=\"return save_skin_edits(true);\">SAVE & EXIT <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");

            }
            Output.WriteLine("  </div>");
            Output.WriteLine();
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;


            Output.WriteLine("  <div class=\"sbkAdm_TitleDiv\" style=\"padding-left:20px\">");
            Output.WriteLine("    <img id=\"sbkAdm_TitleDivImg\" src=\"" + Static_Resources_Gateway.Skins_Img + "\" alt=\"\" />");
            Output.WriteLine("    <h1>Web Skin Administration : " + webSkin.Skin_Code + "</h1>");
            Output.WriteLine("  </div>");
            Output.WriteLine();

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs\">");

            // Add all the possible tabs 
            Output.WriteLine("    <div class=\"tabs\">");
            Output.WriteLine("      <ul>");

            const string GENERAL = "General";
            const string STYLESHEET = "CSS Stylesheet";
            const string HTML = "HTML (Headers/Footers)";
            const string UPLOADS = "Uploads";

            // Draw all the page tabs for this form
            if (page == 1)
            {
                Output.WriteLine("    <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + GENERAL + "</li>");
            }
            else
            {
                Output.WriteLine("    <li id=\"tabHeader_1\" onclick=\"return new_skin_edit_page('a');\">" + GENERAL + "</li>");
            }

            if (page == 2)
            {
                Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + STYLESHEET + "</li>");
            }
            else
            {
                Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_skin_edit_page('b');\">" + STYLESHEET + "</li>");
            }

            if (page == 3)
            {
                Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + HTML + "</li>");
            }
            else
            {
                Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_skin_edit_page('c');\">" + HTML + "</li>");
            }

            //if (page == 4)
            //{
            //    Output.WriteLine("    <li id=\"tabHeader_3\" class=\"tabActiveHeader\">" + BANNERS + "</li>");
            //}
            //else
            //{
            //    Output.WriteLine("    <li id=\"tabHeader_3\" onclick=\"return new_skin_edit_page('d');\">" + BANNERS + "</li>");
            //}

            if (page == 5)
            {
                Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + UPLOADS + "</li>");
            }
            else
            {
                Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_skin_edit_page('e');\">" + UPLOADS + "</li>");
            }

            Output.WriteLine("      </ul>");
            Output.WriteLine("    </div>");

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

                case 5:
                    Add_Page_Uploads(Output);
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
                case 4:
                    Finish_Page_4(Output);
                    break;

                case 5:
                    Finish_Page_Uploads(Output);
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
            string edit_base_code = Form["webskin_basecode"].ToUpper().Trim();
            string edit_banner_link = Form["webskin_bannerlink"].Trim();
            string edit_notes = Form["webskin_notes"].Trim();

            bool override_banner = false;
            bool suppress_top_nav = false;

            object temp_object = Form["webskin_banner_override"];
            if (temp_object != null)
            {
                override_banner = true;
            }

            temp_object = Form["webskin_suppress_top_nav"];
            if (temp_object != null)
            {
                suppress_top_nav = true;
            }

            webSkin.Base_Skin_Code = edit_base_code;
            webSkin.Banner_Link = edit_banner_link;
            webSkin.Notes = edit_notes;
            webSkin.Override_Banner = override_banner;
            webSkin.Suppress_Top_Navigation = suppress_top_nav;
        }

        private void Add_Page_1(TextWriter Output)
        {
            // Help constants (for now)
            const string BASE_CODE_HELP = "Advanced option - used to not replicate some of the basic design elements associated with a web skin.  Using the base code will make the child skins reference the default design elements in the base skin.  This reduces duplication and allows one skin to exert control and changes on many other subordinate skins.";
            const string BANNER_LINK_HELP = "Advanced option - if this web skin is set to override the aggregation banner, than this will be the link that is used when users select the banner.";
            const string NOTES_HELP = "Private notes associated with this web skin";
            const string BANNER_OVERRIDE_HELP = "Advanced option - use to override the aggregation banners and always show a web skin banner when viewing aggregations under this web skin.";
            const string SUPPRESS_TOP_NAV_HELP = "Flag suppresses the top main menu on most aggregation views.  The basic navigation will then need to be provided in the header most likely.";


            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Basic Information</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The information in this section is the basic information about the web skin, such as the base skin, banner information, whether this skin overrides the banner over all aggregations, etc..</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleskin\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

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
                if (( !String.IsNullOrEmpty(webSkin.Base_Skin_Code)) && ( String.Compare(webSkin.Base_Skin_Code, thisSkinCode, StringComparison.OrdinalIgnoreCase) == 0 ))
                    Output.WriteLine("          <option value=\"" + thisSkinCode.ToUpper() + "\" selected=\"selected\">" + thisSkinCode + "</option>");
                else
                    Output.WriteLine("          <option value=\"" + thisSkinCode.ToUpper() + "\">" + thisSkinCode + "</option>");
            }
            Output.WriteLine("        </select></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + BASE_CODE_HELP + "');\"  title=\"" + BASE_CODE_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            // Add the banner link
            if ( webSkin.Override_Banner )
                Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" id=\"banner_link_row\" style=\"display:table-row;\">");
            else
                Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" id=\"banner_link_row\" style=\"display:none;\">");

            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"webskin_bannerlink\">Banner Link:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_medium_input sbkAdmin_Focusable\" name=\"webskin_bannerlink\" id=\"webskin_bannerlink\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(webSkin.Banner_Link) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + BANNER_LINK_HELP + "');\"  title=\"" + BANNER_LINK_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            // Add the notes
            Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"webskin_notes\">Notes:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSaav_large_textbox sbkAdmin_Focusable\" rows=\"6\" name=\"webskin_notes\" id=\"webskin_notes\">" + HttpUtility.HtmlEncode(webSkin.Notes) + "</textarea></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + NOTES_HELP + "');\"  title=\"" + NOTES_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\"></tr>");

            // Add checkboxes for overriding the banner   
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Flags:</td>");
            Output.WriteLine("    <td>");
            Output.Write("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"webskin_banner_override\" id=\"webskin_banner_override\" onchange=\"return skins_display_banner_link(this);\" ");
            if (webSkin.Override_Banner)
                Output.Write("checked=\"checked\" ");
            Output.WriteLine("/> <label for=\"webskin_banner_override\">Override banner?</label></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + BANNER_OVERRIDE_HELP + "');\"  title=\"" + BANNER_OVERRIDE_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            // Add checkboxes for suppressing the top navigation 
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td></td>");
            Output.WriteLine("    <td>");
            Output.Write("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSav_checkbox\" type=\"checkbox\" name=\"webskin_suppress_top_nav\" id=\"webskin_suppress_top_nav\" ");
            if (webSkin.Suppress_Top_Navigation)
                Output.Write("checked=\"checked\" ");
            Output.WriteLine("/> <label for=\"webskin_suppress_top_nav\">Suppress main menu?</label></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + SUPPRESS_TOP_NAV_HELP + "');\"  title=\"" + SUPPRESS_TOP_NAV_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");


            Output.WriteLine("</table>");
            Output.WriteLine("<br />");

        }


        #endregion

        #region Methods to render (and parse) page 2 - Stylesheet

        private void Save_Page_2_Postback(NameValueCollection Form)
        {
            // Check for action flag
            string css_contents = Form["admin_skin_css_edit"].Trim();
            if (css_contents.Length == 0)
                css_contents = "/**  CSS for " + webSkin.Skin_Code + " web skin **/";

            updatedSourceFiles["CSS"] = css_contents;
        }

        private void Add_Page_2(TextWriter Output)
        {
            // Get the CSS file's contents
            string css_contents;

            // Is an updated version in the cache?
            if (updatedSourceFiles.ContainsKey("CSS"))
                css_contents = updatedSourceFiles["CSS"];
            else
            {
                string file = skinDirectory + "\\" + webSkin.CSS_Style;
                if (File.Exists(file))
                {
                    StreamReader reader = new StreamReader(file);
                    css_contents = reader.ReadToEnd();
                    reader.Close();
                }
                else
                {
                    css_contents = "/**  CSS for " + webSkin.Skin_Code + " web skin **/";
                }
            }

            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Web Skin Custom Stylesheet (CSS)</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>You can edit the contents of the web skin stylesheet (css) file here.</p><p>Your changes will not take affect until you actually click SAVE when you are done making all your changes.</p><p>NOTE: You may need to refresh your browser when you are all done for your changes to take affect.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleskin\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

            // Add the css edit textarea code
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" >");
            Output.WriteLine("    <td style=\"width:40px;\">&nbsp;</td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <textarea class=\"sbkSaav_EditCssTextarea sbkAdmin_Focusable\" id=\"admin_skin_css_edit\" name=\"admin_skin_css_edit\">");
            Output.WriteLine(css_contents);
            Output.WriteLine("      </textarea>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");

        }

        #endregion

        #region Methods to render (and parse) page 3 - HTML (headers and footers)

        private void Save_Page_3_Postback(NameValueCollection Form)
        {
            Web_Language_Enum current_language = Web_Language_Enum.DEFAULT;

            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["lang"]))
            {
                string current_language_code = HttpContext.Current.Request.QueryString["lang"];
                current_language = current_language_code.ToLower() == "def" ? Web_Language_Enum.DEFAULT : Web_Language_Enum_Converter.Code_To_Enum(current_language_code);
            }

            if (current_language != Web_Language_Enum.UNDEFINED)
            {
                string header_source = Form["webskin_header_source"];
                string footer_source = Form["webskin_footer_source"];
                string header_item_source = Form["webskin_header_item_source"];
                string footer_item_source = Form["webskin_footer_item_source"];

                if (webSkin.SourceFiles.ContainsKey(current_language))
                {
                    Complete_Web_Skin_Source_Files sources = webSkin.SourceFiles[current_language];
                    updatedSourceFiles[sources.Header_Source_File] = header_source;
                    updatedSourceFiles[sources.Footer_Source_File] = footer_source;
                    updatedSourceFiles[sources.Header_Item_Source_File] = header_item_source;
                    updatedSourceFiles[sources.Footer_Item_Source_File] = footer_item_source;
                }
                else
                {
                    string language_code = "_" + Web_Language_Enum_Converter.Enum_To_Code(current_language);
                    if ( current_language == Web_Language_Enum.DEFAULT )
                        language_code = String.Empty;

                    Complete_Web_Skin_Source_Files sources = new Complete_Web_Skin_Source_Files
                    {
                        Header_Source_File = "html\\header" + language_code + ".html", 
                        Footer_Source_File = "html\\footer" + language_code + ".html", 
                        Header_Item_Source_File = "html\\header_item" + language_code + ".html", 
                        Footer_Item_Source_File = "html\\footer_item" + language_code + ".html"
                    };
                    webSkin.SourceFiles[current_language] = sources;

                    updatedSourceFiles[sources.Header_Source_File] = header_source;
                    updatedSourceFiles[sources.Footer_Source_File] = footer_source;
                    updatedSourceFiles[sources.Header_Item_Source_File] = header_item_source;
                    updatedSourceFiles[sources.Footer_Item_Source_File] = footer_item_source;
                }
            }
        }

        private void Add_Page_3(TextWriter Output)
        {
            Web_Language_Enum current_language = Web_Language_Enum.DEFAULT;

            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["lang"]))
            {
                string current_language_code = HttpContext.Current.Request.QueryString["lang"];
                current_language = current_language_code.ToLower() == "def" ? Web_Language_Enum.DEFAULT : Web_Language_Enum_Converter.Code_To_Enum(current_language_code);
            }

            string HEADER_HELP = "Basic header is placed at the top of all non-item web pages throughout the system.";
            string FOOTER_HELP = "Basic footer is placed at the bottom of all non-item web pages throughout the system.";
            string HEADER_ITEM_HELP = "Header is placed at the top of all individual items (i.e., digital resources) views throughout the system.  Generally, this header is either the same as the header above, or a reduced height header to allow the digital resources to take over more of the screen.";
            string FOOTER_ITEM_HELP = "Footer is placed at the bottom of all individual items (i.e., digital resources) views throughout the system.  Generally, this footer is either the same as the footer above, or a reduced height footer to allow the digital resources to take over more of the screen.";
            string EXISTING_LANGUAGE_HELP = "Pick an existing language from this selection box to view and edit the headers and footers used for that language.";
            string NEW_LANGUAGE_HELP = "To add support for translations or versions of the headers and footers for a new language, select the language from the selection box and press ADD.\\n\\nSelect a current language to copy from, otherwise all the headers and footers will start blank.";



            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Language-Specific Headers and Footers</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The web skin defines the headers and footers that display at the top and bottom of every page and these can be customized for each different language you expect your users to request.</p><p>In addition, you can define a header/footer pair for most non-item pages and another pair to use when users are viewing a digital resource within your system.  This allows the item-specific headers and footers to have a smaller height, a lower profile, and work better with the full screen.</p>Your changes will not take affect until you actually click SAVE when you are done making all your changes.</p><p>NOTE: You may need to refresh your browser when you are all done for your changes to take affect.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleskin\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

            //Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\"></tr>");

            // Find all the existing languages
            List<string> existing_languages = new List<string>();
            bool found_language = false;
            if ((webSkin.SourceFiles != null) && (webSkin.SourceFiles.Count > 0))
            {
                foreach (KeyValuePair<Web_Language_Enum, Complete_Web_Skin_Source_Files> languageSupport in webSkin.SourceFiles)
                {
                    if (languageSupport.Key == current_language)
                    {
                        found_language = true;
                    }

                    string thisLangTerm = Web_Language_Enum_Converter.Enum_To_Name(languageSupport.Key);
                    if (languageSupport.Key == Web_Language_Enum.DEFAULT)
                    {
                        thisLangTerm = "DEFAULT";
                    }
                    existing_languages.Add(thisLangTerm);
                }
            }

            if (!found_language)
                current_language = Web_Language_Enum.UNDEFINED;


            // Write the add new web skin information
            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Add New Language</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>Use this option to add a new supported language to this web skin.  This allows you to customize the headers and footers for users that request this language, either explicitly or through their browser settings.</p></td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\" style=\"width:135px\">New Language:</td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td>");

            Output.Write("          <select class=\"sbkSaav_SelectSingle\" id=\"webskin_new_lang\" name=\"webskin_new_lang\">");

            // Add each language in the combo box
            foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
            {
                if (!existing_languages.Contains(possible_language))
                    Output.Write("<option value=\"" + Web_Language_Enum_Converter.Name_To_Code(possible_language) + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
            }
            Output.WriteLine();
            Output.WriteLine("        </td>");
            if ((webSkin.SourceFiles != null) && (webSkin.SourceFiles.Count > 0))
            {
                Output.WriteLine("        <td style=\"padding-left:35px;\">Copy from existing: </td>");
                Output.WriteLine("        <td>");
                Output.Write("          <select id=\"webskin_new_lang_copy\" name=\"webskin_new_lang_copy\">");
                Output.Write("<option value=\"\" selected=\"selected\"></option>");


                foreach (KeyValuePair<Web_Language_Enum, Complete_Web_Skin_Source_Files> languageSupport in webSkin.SourceFiles)
                {
                    string thisLangCode = Web_Language_Enum_Converter.Enum_To_Code(languageSupport.Key);
                    string thisLangTerm = Web_Language_Enum_Converter.Enum_To_Name(languageSupport.Key);

                    if (languageSupport.Key == Web_Language_Enum.DEFAULT)
                    {
                        thisLangCode = "def";
                        thisLangTerm = "DEFAULT";
                    }

                    Output.WriteLine("          <option value=\"" + thisLangCode + "\">" + HttpUtility.HtmlEncode(thisLangTerm) + "</option>");
                }

                Output.WriteLine("</select>");
                Output.WriteLine("        </td>");
            }
            else
            {
                Output.WriteLine("        <td></td>");
                Output.WriteLine("        <td></td>");
            }

            Output.WriteLine("        <td style=\"padding-left:20px\"><button title=\"Add new language\" class=\"sbkAdm_RoundButton\" onclick=\"return add_skin_language();\">ADD</button></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + NEW_LANGUAGE_HELP + "');\"  title=\"" + NEW_LANGUAGE_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td><tr>");

            // Add the existing languages
            if ((webSkin.SourceFiles != null) && (webSkin.SourceFiles.Count > 0))
            {
                Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Existing Language Support</td></tr>");
                Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
                Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" style=\"width:135px\"><label for=\"webskin_existing_language\">Existing Languages:</label></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");

                Output.WriteLine("        <select class=\"sbkSav_small_input1 sbkAdmin_Focusable\" name=\"webskin_existing_language\" id=\"webskin_existing_language\" onchange=\"return new_skin_language(this);\">");
                
                foreach (KeyValuePair<Web_Language_Enum, Complete_Web_Skin_Source_Files> languageSupport in webSkin.SourceFiles)
                {
                    string thisLangCode = Web_Language_Enum_Converter.Enum_To_Code(languageSupport.Key);
                    string thisLangTerm = Web_Language_Enum_Converter.Enum_To_Name(languageSupport.Key);

                    if (languageSupport.Key == Web_Language_Enum.DEFAULT)
                    {
                        thisLangCode = "def";
                        thisLangTerm = "DEFAULT";
                    }

                    if (languageSupport.Key == current_language)
                    {
                        Output.WriteLine("          <option value=\"" + thisLangCode + "\" selected=\"selected\">" + HttpUtility.HtmlEncode(thisLangTerm) + "</option>");
                    }
                    else
                        Output.WriteLine("          <option value=\"" + thisLangCode + "\">" + HttpUtility.HtmlEncode(thisLangTerm) + "</option>");
                }

                Output.WriteLine("        </select></td>");
                if ((current_language != Web_Language_Enum.DEFAULT) && (webSkin.SourceFiles.Count > 1))
                {
                    Output.WriteLine("        <td style=\"padding-left:20px\"><button title=\"Remove support for this language from this web skin\" class=\"sbkAdm_RoundButton\" onclick=\"return delete_skin_language('" + Web_Language_Enum_Converter.Enum_To_Code(current_language) + "');\">REMOVE</button></td>");
                }
                Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + EXISTING_LANGUAGE_HELP + "');\"  title=\"" + EXISTING_LANGUAGE_HELP + "\" /></td></tr></table>");
                Output.WriteLine("     </td>");
                Output.WriteLine("  </tr>");
            }


            if (current_language != Web_Language_Enum.UNDEFINED)
            {

                Complete_Web_Skin_Source_Files sources = webSkin.SourceFiles[current_language];
                string header_source = get_file_source(sources.Header_Source_File);
                string footer_source = get_file_source(sources.Footer_Source_File);
                string header_item_source = get_file_source(sources.Header_Item_Source_File);
                string footer_item_source = get_file_source(sources.Footer_Item_Source_File);


                // Add the standard headers and footers
                Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td><tr>");
                Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
                Output.WriteLine("    <td>&nbsp;</td>");
                Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"webskin_header_source\">Standard Header:</label></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSsav_html_textbox sbkAdmin_Focusable\" rows=\"30\" name=\"webskin_header_source\" id=\"webskin_header_source\">" + HttpUtility.HtmlEncode(header_source) + "</textarea></td>");
                Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + HEADER_HELP + "');\"  title=\"" + HEADER_HELP + "\" /></td></tr></table>");
                Output.WriteLine("     </td>");
                Output.WriteLine("  </tr>");

                Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td><tr>");
                Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
                Output.WriteLine("    <td>&nbsp;</td>");
                Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"webskin_footer_source\">Standard Footer:</label></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSsav_html_textbox sbkAdmin_Focusable\" rows=\"30\" name=\"webskin_footer_source\" id=\"webskin_footer_source\">" + HttpUtility.HtmlEncode(footer_source) + "</textarea></td>");
                Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + FOOTER_HELP + "');\"  title=\"" + FOOTER_HELP + "\" /></td></tr></table>");
                Output.WriteLine("     </td>");
                Output.WriteLine("  </tr>");

                // Add the item headers and footers
                //Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Item-Specific Headers and Footers</td></tr>");
                Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td><tr>");
                Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
                Output.WriteLine("    <td>&nbsp;</td>");
                Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"webskin_header_item_source\">Item Header:</label></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine("      <a title=\"Copy the HTML for the standard header down to this item header\" href=\"\" onclick=\"for(var i in CKEDITOR.instances) { CKEDITOR.instances[i].updateElement(); } return copy_skin_header_html();\">Copy from the standard header</a> <br />");
                Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSsav_html_textbox sbkAdmin_Focusable\" rows=\"30\" name=\"webskin_header_item_source\" id=\"webskin_header_item_source\">" + HttpUtility.HtmlEncode(header_item_source) + "</textarea></td>");
                Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + HEADER_ITEM_HELP + "');\"  title=\"" + HEADER_ITEM_HELP + "\" /></td></tr></table>");
                Output.WriteLine("     </td>");
                Output.WriteLine("  </tr>");

                Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td><tr>");
                Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
                Output.WriteLine("    <td>&nbsp;</td>");
                Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"webskin_footer_item_source\">Item Footer:</label></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine("      <a title=\"Copy the HTML for the standard footer down to this item footer\" href=\"\" onclick=\"for(var i in CKEDITOR.instances) { CKEDITOR.instances[i].updateElement(); } return copy_skin_footer_html();\">Copy from the standard footer</a> <br />");
                Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSsav_html_textbox sbkAdmin_Focusable\" rows=\"30\" name=\"webskin_footer_item_source\" id=\"webskin_footer_item_source\">" + HttpUtility.HtmlEncode(footer_item_source) + "</textarea></td>");
                Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + FOOTER_ITEM_HELP + "');\"  title=\"" + FOOTER_ITEM_HELP + "\" /></td></tr></table>");
                Output.WriteLine("     </td>");
                Output.WriteLine("  </tr>");


                // Determine the aggregation upload directory
                string skin_upload_dir = skinDirectory + "\\uploads";
                string skin_upload_url = UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL + "design/skins/" + webSkin.Skin_Code + "/uploads/";

                // Create the CKEditor objects
                CKEditor.CKEditor editor1 = new CKEditor.CKEditor
                {
                    BaseUrl = RequestSpecificValues.Current_Mode.Base_URL,
                    Language = RequestSpecificValues.Current_Mode.Language,
                    TextAreaID = "webskin_header_source",
                    FileBrowser_ImageUploadUrl = RequestSpecificValues.Current_Mode.Base_URL + "HtmlEditFileHandler.ashx",
                    UploadPath = skin_upload_dir,
                    UploadURL = skin_upload_url,
                    Start_In_Source_Mode = true
                };
                CKEditor.CKEditor editor2 = new CKEditor.CKEditor
                {
                    BaseUrl = RequestSpecificValues.Current_Mode.Base_URL,
                    Language = RequestSpecificValues.Current_Mode.Language,
                    TextAreaID = "webskin_footer_source",
                    FileBrowser_ImageUploadUrl = RequestSpecificValues.Current_Mode.Base_URL + "HtmlEditFileHandler.ashx",
                    UploadPath = skin_upload_dir,
                    UploadURL = skin_upload_url,
                    Start_In_Source_Mode = true
                };
                CKEditor.CKEditor editor3 = new CKEditor.CKEditor
                {
                    BaseUrl = RequestSpecificValues.Current_Mode.Base_URL,
                    Language = RequestSpecificValues.Current_Mode.Language,
                    TextAreaID = "webskin_header_item_source",
                    FileBrowser_ImageUploadUrl = RequestSpecificValues.Current_Mode.Base_URL + "HtmlEditFileHandler.ashx",
                    UploadPath = skin_upload_dir,
                    UploadURL = skin_upload_url,
                    Start_In_Source_Mode = true
                };
                CKEditor.CKEditor editor4 = new CKEditor.CKEditor
                {
                    BaseUrl = RequestSpecificValues.Current_Mode.Base_URL,
                    Language = RequestSpecificValues.Current_Mode.Language,
                    TextAreaID = "webskin_footer_item_source",
                    FileBrowser_ImageUploadUrl = RequestSpecificValues.Current_Mode.Base_URL + "HtmlEditFileHandler.ashx",
                    UploadPath = skin_upload_dir,
                    UploadURL = skin_upload_url,
                    Start_In_Source_Mode = true
                };

                // If there are existing files, add a reference to the URL for the image browser
                if ((Directory.Exists(skin_upload_dir)) && (Directory.GetFiles(skin_upload_dir).Length > 0))
                {
                    // Is there an endpoint defined for looking at uploaded files?
                    string upload_files_json_url = SobekEngineClient.WebSkins.WebSkin_Uploaded_Files_URL;
                    if (!String.IsNullOrEmpty(upload_files_json_url))
                    {
                        editor1.ImageBrowser_ListUrl = String.Format(upload_files_json_url, webSkin.Skin_Code);
                        editor2.ImageBrowser_ListUrl = String.Format(upload_files_json_url, webSkin.Skin_Code);
                        editor3.ImageBrowser_ListUrl = String.Format(upload_files_json_url, webSkin.Skin_Code);
                        editor4.ImageBrowser_ListUrl = String.Format(upload_files_json_url, webSkin.Skin_Code);

                    }
                }

                // Add the HTML from the CKEditor object
                editor1.Add_To_Stream(Output, true);
                editor2.Add_To_Stream(Output, false);
                editor3.Add_To_Stream(Output, false);
                editor4.Add_To_Stream(Output, false);

            }

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }

        private string get_file_source(string FileName)
        {
            if (updatedSourceFiles.ContainsKey(FileName))
                return updatedSourceFiles[FileName];

            string file_in_dir = Path.Combine(skinDirectory, FileName);
            if (!File.Exists(file_in_dir))
                return String.Empty;

            StreamReader reader = new StreamReader(file_in_dir);
            string contents = reader.ReadToEnd();
            reader.Close();

            return contents.Replace("<%", "[%").Replace("%>", "%]");
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

        #region Methods to render (and parse) page 5 -  Uploads

        private void Save_Page_Uploads_Postback(NameValueCollection Form)
        {
            string action = Form["admin_skin_action"];
            if ((action.Length > 0) && (action.IndexOf("delete_") == 0))
            {
                string file = action.Substring(7);
                string path_file = skinDirectory + "\\uploads\\" + file;
                if (File.Exists(path_file))
                    File.Delete(path_file);
            }
        }


        private void Add_Page_Uploads(TextWriter Output)
        {
            // Help constants (for now)
            const string UPLOAD_BANNER_HELP = "Press the SELECT FILES button here to upload new images or documents to associated with this web skin.   You will be able to access the image files when you are editing the headers and footers through the HTML editor.\\n\\nThe following image types can be uploaded: bmp, gif, jpg, png.  The following other documents can also be uploaded: ai, doc, docx, eps, pdf, psd, pub, txt, vsd, vsdx, xls, xlsx, zip.";


            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Upload Images and Documents</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>Manage your uploaded images which can be included in your web skin (either in the CSS or in the headers/footers) or other document types which can be uploaded and associated with this aggregation.</p><p>The following image types can be uploaded: bmp, gif, jpg, png.  The following other documents can also be uploaded: ai, doc, docx, eps, kml, pdf, psd, pub, txt, vsd, vsdx, xls, xlsx, xml, zip.   In addition, the following font formats are now accepted: eot, svg, ttf, woff, woff2.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleskin\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");


            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Upload New Images and Documents</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_UploadRow\">");
            Output.WriteLine("    <td style=\"width:100px\">&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"2\">");
            Output.WriteLine("       <table class=\"sbkSaav_InnerTable\">");
            Output.WriteLine("         <tr>");
            Output.WriteLine("           <td class=\"sbkSaav_UploadInstr\">To upload one or more images or documents press SELECT FILES, browse to the file, and then select UPLOAD</td>");
            Output.WriteLine("           <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + UPLOAD_BANNER_HELP + "');\"  title=\"" + UPLOAD_BANNER_HELP + "\" /></td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("         <tr>");
            Output.WriteLine("           <td colspan=\"2\">");
        }

        private void Finish_Page_Uploads(TextWriter Output)
        {
            Output.WriteLine("           </td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("       </table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");

            string uploads_dir = skinDirectory + "\\uploads";
            if (Directory.Exists(uploads_dir))
            {
                string[] upload_files = SobekCM_File_Utilities.GetFiles(uploads_dir, "*.jpg|*.jpeg|*.bmp|*.gif|*.png");

                if (upload_files.Length > 0)
                {
                    Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Existing Images</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
                    Output.WriteLine("    <td colspan=\"3\">");


                    Output.WriteLine("  <table class=\"sbkSaav_UploadTable\">");
                    Output.WriteLine("    <tr>");

                    int unused_column = 0;
                    foreach (string thisImage in upload_files)
                    {
                        string thisImageFile = Path.GetFileName(thisImage);
                        string thisImageFile_URL = RequestSpecificValues.Current_Mode.Base_URL + "design/skins/" + webSkin.Skin_Code + "/uploads/" + thisImageFile;

                        Output.Write("      <td>");
                        Output.Write("<a href=\"" + thisImageFile_URL + "\" target=\"_" + thisImageFile + "\" title=\"View this uploaded image\">");
                        Output.Write("<img class=\"sbkSaav_UploadThumbnail\" src=\"" + thisImageFile_URL + "\" alt=\"Missing Thumbnail\" title=\"" + thisImageFile + "\" /></a>");


                        string display_name = thisImageFile;
                        if (display_name.Length > 25)
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\"><abbr title=\"" + display_name + "\">" + thisImageFile.Substring(0, 20) + "..." + Path.GetExtension(thisImage) + "</abbr></span>");
                        }
                        else
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\">" + thisImageFile + "</span>");
                        }



                        // Build the action links
                        Output.Write("<br /><span class=\"sbkAdm_ActionLink\" >( ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_skin_upload_file('" + thisImageFile + "');\" title=\"Delete this uploaded file\">delete</a> | ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"window.prompt('Below is the URL, available to copy to your clipboard.  To copy to clipboard, press Ctrl+C (or Cmd+C) and Enter', '" + thisImageFile_URL + "'); return false;\" title=\"View the URL for this file\">view url</a>");
                        Output.WriteLine(" )</span></td>");

                        unused_column++;

                        // Start a new row?
                        if (unused_column >= 4)
                        {
                            Output.WriteLine("    </tr>");
                            Output.WriteLine("    <tr>");
                            unused_column = 0;
                        }
                    }

                    // Finish the table cells and row
                    while (unused_column < 4)
                    {
                        Output.WriteLine("      <td></td>");
                        unused_column++;
                    }
                    Output.WriteLine("    </tr>");

                    Output.WriteLine("  </table>");

                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>");
                }

                // Add exisitng FONTS |*.eot,.svg,.ttf,.woff,.woff2
                string[] font_files = SobekCM_File_Utilities.GetFiles(uploads_dir, "|*.eot|*.svg|*.ttf|*.woff|*.woff2");
                if (font_files.Length > 0)
                {
                    Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Existing Font Files</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
                    Output.WriteLine("    <td colspan=\"3\" style=\"text-align:left\">");


                    Output.WriteLine("  <table class=\"sbkSaav_UploadTable\">");
                    Output.WriteLine("    <tr>");

                    int unused_column = 0;
                    foreach (string thisDocument in font_files)
                    {
                        string thisDocFile = Path.GetFileName(thisDocument);
                        string thisDocFile_URL = RequestSpecificValues.Current_Mode.Base_URL + "design/skins/" + webSkin.Skin_Code + "/uploads/" + thisDocFile;

                        // Determine which image to use for this document
                        string extension = Path.GetExtension(thisDocument);
                        string thisDocFileImage = Static_Resources_Gateway.File_Font_Img;

                        Output.Write("      <td>");
                        Output.Write("<a href=\"" + thisDocFile_URL + "\" target=\"_" + thisDocFile + "\" title=\"Download this uploaded font file\">");
                        Output.Write("<img class=\"sbkSaav_UploadThumbnail2\" src=\"" + thisDocFileImage + "\" alt=\"Document\" title=\"" + thisDocFile + "\" /></a>");


                        string display_name = thisDocFile;

                        if (display_name.Length > 25)
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\"><abbr title=\"" + display_name + "\">" + thisDocFile.Substring(0, 20) + "..." + extension + "</abbr></span>");
                        }
                        else
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\">" + thisDocFile + "</span>");
                        }



                        // Build the action links
                        Output.Write("<br /><span class=\"sbkAdm_ActionLink\" >( ");
                        Output.Write("<a href=\"" + thisDocFile_URL + "\" target=\"_" + thisDocFile + "\" title=\"Download\">download</a> | ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_skin_upload_file('" + thisDocFile + "');\" title=\"Delete this uploaded font\">delete</a> | ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"window.prompt('Below is the URL, available to copy to your clipboard.  To copy to clipboard, press Ctrl+C (or Cmd+C) and Enter', '" + thisDocFile_URL + "'); return false;\" title=\"View the URL for this file\">view url</a>");

                        Output.WriteLine(" )</span></td>");

                        unused_column++;

                        // Start a new row?
                        if (unused_column >= 4)
                        {
                            Output.WriteLine("    </tr>");
                            Output.WriteLine("    <tr>");
                            unused_column = 0;
                        }
                    }

                    // Finish the table cells and row
                    while (unused_column < 4)
                    {
                        Output.WriteLine("      <td></td>");
                        unused_column++;
                    }
                    Output.WriteLine("    </tr>");

                    Output.WriteLine("  </table>");

                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>");
                }

                // Add existing DOCUMENTS
                string[] documents_files = SobekCM_File_Utilities.GetFiles(uploads_dir, "*.ai|*.doc|*.docx|*.eps|*.kml|*.pdf|*.psd|*.pub|*.txt|*.vsd|*.vsdx|*.xls|*.xlsx|*.xml|*.zip");
                if (documents_files.Length > 0)
                {
                    Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Existing Documents</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
                    Output.WriteLine("    <td colspan=\"3\" style=\"text-align:left\">");


                    Output.WriteLine("  <table class=\"sbkSaav_UploadTable\">");
                    Output.WriteLine("    <tr>");

                    int unused_column = 0;
                    foreach (string thisDocument in documents_files)
                    {
                        string thisDocFile = Path.GetFileName(thisDocument);
                        string thisDocFile_URL = RequestSpecificValues.Current_Mode.Base_URL + "design/skins/" + webSkin.Skin_Code + "/uploads/" + thisDocFile;

                        // Determine which image to use for this document
                        string extension = Path.GetExtension(thisDocument);
                        string thisDocFileImage = Static_Resources_Gateway.File_TXT_Img;
                        switch (extension.ToUpper().Replace(".", ""))
                        {
                            case "AI":
                                thisDocFileImage = Static_Resources_Gateway.File_AI_Img;
                                break;

                            case "DOC":
                            case "DOCX":
                                thisDocFileImage = Static_Resources_Gateway.File_Word_Img;
                                break;

                            case "EPS":
                                thisDocFileImage = Static_Resources_Gateway.File_EPS_Img;
                                break;

                            case "KML":
                                thisDocFileImage = Static_Resources_Gateway.File_KML_Img;
                                break;

                            case "PDF":
                                thisDocFileImage = Static_Resources_Gateway.File_PDF_Img;
                                break;

                            case "PSD":
                                thisDocFileImage = Static_Resources_Gateway.File_PSD_Img;
                                break;

                            case "PUB":
                                thisDocFileImage = Static_Resources_Gateway.File_PUB_Img;
                                break;

                            case "TXT":
                                thisDocFileImage = Static_Resources_Gateway.File_TXT_Img;
                                break;

                            case "VSD":
                            case "VSDX":
                                thisDocFileImage = Static_Resources_Gateway.File_VSD_Img;
                                break;

                            case "XLS":
                            case "XLSX":
                                thisDocFileImage = Static_Resources_Gateway.File_Excel_Img;
                                break;

                            case "XML":
                                thisDocFileImage = Static_Resources_Gateway.File_XML_Img;
                                break;

                            case "ZIP":
                                thisDocFileImage = Static_Resources_Gateway.File_ZIP_Img;
                                break;
                        }
                        Output.Write("      <td>");
                        Output.Write("<a href=\"" + thisDocFile_URL + "\" target=\"_" + thisDocFile + "\" title=\"View this uploaded document\">");
                        Output.Write("<img class=\"sbkSaav_UploadThumbnail2\" src=\"" + thisDocFileImage + "\" alt=\"Document\" title=\"" + thisDocFile + "\" /></a>");


                        string display_name = thisDocFile;

                        if (display_name.Length > 25)
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\"><abbr title=\"" + display_name + "\">" + thisDocFile.Substring(0, 20) + "..." + extension + "</abbr></span>");
                        }
                        else
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\">" + thisDocFile + "</span>");
                        }



                        // Build the action links
                        Output.Write("<br /><span class=\"sbkAdm_ActionLink\" >( ");
                        Output.Write("<a href=\"" + thisDocFile_URL + "\" target=\"_" + thisDocFile + "\" title=\"Download and view this uploaded file\">download</a> | ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_skin_upload_file('" + thisDocFile + "');\" title=\"Delete this uploaded file\">delete</a> | ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"window.prompt('Below is the URL, available to copy to your clipboard.  To copy to clipboard, press Ctrl+C (or Cmd+C) and Enter', '" + thisDocFile_URL + "'); return false;\" title=\"View the URL for this file\">view url</a>");

                        Output.WriteLine(" )</span></td>");

                        unused_column++;

                        // Start a new row?
                        if (unused_column >= 4)
                        {
                            Output.WriteLine("    </tr>");
                            Output.WriteLine("    <tr>");
                            unused_column = 0;
                        }
                    }

                    // Finish the table cells and row
                    while (unused_column < 4)
                    {
                        Output.WriteLine("      <td></td>");
                        unused_column++;
                    }
                    Output.WriteLine("    </tr>");

                    Output.WriteLine("  </table>");

                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>");
                }
            }

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }

        #endregion

        #region Methods to add file upload controls to the page

        /// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Skin_Single_AdminViewer.Add_Controls", String.Empty);

            switch (page)
            {
                case 4:
                    add_upload_controls(MainPlaceHolder, ".gif", skinDirectory, "banner.gif", false, Tracer);
                    break;

                case 5:
                    add_upload_controls(MainPlaceHolder, ".gif,.bmp,.jpg,.png,.jpeg,.ai,.doc,.docx,.eps,.kml,.pdf,.psd,.pub,.txt,.vsd,.vsdx,.xls,.xlsx,.xml,.zip,.eot,.svg,.ttf,.woff,.woff2", skinDirectory + "\\uploads", String.Empty, true, Tracer);
                    break;
            }
            
        }

        private void add_upload_controls(PlaceHolder UploadFilesPlaceHolder, string FileExtensions, string UploadDirectory, string ServerSideName, bool UploadMultiple, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Skin_Single_AdminViewer.add_upload_controls", String.Empty);

            // Ensure the directory exists
            if (!File.Exists(UploadDirectory))
                Directory.CreateDirectory(UploadDirectory);

            StringBuilder filesBuilder = new StringBuilder(2000);

            LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
            UploadFilesPlaceHolder.Controls.Add(filesLiteral2);
            filesBuilder.Remove(0, filesBuilder.Length);

            UploadiFiveControl uploadControl = new UploadiFiveControl
            {
                UploadPath = UploadDirectory, 
                UploadScript = RequestSpecificValues.Current_Mode.Base_URL + "UploadiFiveFileHandler.ashx", 
                AllowedFileExtensions = FileExtensions, 
                SubmitWhenQueueCompletes = true, 
                RemoveCompleted = true,
                Multi = UploadMultiple, 
                ServerSideFileName = ServerSideName
            };
            UploadFilesPlaceHolder.Controls.Add(uploadControl);

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            UploadFilesPlaceHolder.Controls.Add(literal1);
        }

        #endregion

        /// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden/omitted. </summary>
        /// <value> This property returns TRUE, since skin design files can be uploaded here </value>
        public override bool Upload_File_Possible
        {
            get { return true; }
        }

        /// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
        /// <value> Returns 'sbkSsav_ContainerInner' </value>
        public override string Container_CssClass { get { return "sbkSsav_ContainerInner"; } }
    }
}
