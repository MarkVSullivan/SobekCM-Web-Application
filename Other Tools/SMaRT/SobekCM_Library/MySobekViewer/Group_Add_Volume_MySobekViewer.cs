#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Library.Application_State;
using SobekCM.Library.Citation.Template;
using SobekCM.Library.HTML;
using SobekCM.Library.Items;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated user to add a new item/volume to an existing title </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to allow the user to add a new item/volume to an existing title </li>
    /// </ul></remarks>
    public class Group_Add_Volume_MySobekViewer : abstract_MySobekViewer
    {
        private readonly bool bornDigital;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly string date;
        private readonly short dispositionAdvice;
        private readonly string dispositionAdviceNotes;
        private bool hierarchyCopiedFromDate;
        private readonly Dictionary<string, Wordmark_Icon> iconList;
        private readonly short ipRestrict;
        private readonly SobekCM_Item item;
        private readonly Item_Lookup_Object itemList;
        private readonly SobekCM_Items_In_Title itemsInTitle;
        private readonly string level1;
        private readonly int level1Order;
        private readonly string level2;
        private readonly int level2Order;
        private readonly string level3;
        private readonly int level3Order;
        private readonly DateTime? materialRecdDate;
        private readonly string materialRecdNotes;
        private string message;
        private readonly Template template;
        private readonly string title;
        private readonly string trackingBox;
        private readonly SobekCM_Skin_Object webSkin;


        /// <summary> Constructor for a new instance of the Group_Add_Volume_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="All_Items_Lookup"> Allows individual items to be retrieved by various methods as <see cref="Single_Item"/> objects.</param>
        /// <param name="Current_Item"> Individual digital resource to be edited by the user </param>
        /// <param name="Code_Manager"> Code manager contains the list of all valid aggregation codes </param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Icon_Table"> Dictionary of all the wordmark/icons which can be tagged to the items </param>
        /// <param name="Items_In_Title"> List of items within this title </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Group_Add_Volume_MySobekViewer(User_Object User,
            SobekCM_Navigation_Object Current_Mode,
            Item_Lookup_Object All_Items_Lookup,
            SobekCM_Item Current_Item, Aggregation_Code_Manager Code_Manager,
            Dictionary<string, Wordmark_Icon> Icon_Table,
            SobekCM_Skin_Object HTML_Skin,
            SobekCM_Items_In_Title Items_In_Title,
            Language_Support_Info Translator,
            Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Group_Add_Volume_MySobekViewer.Constructor", String.Empty);

            currentMode = Current_Mode;
            item = Current_Item;
            itemList = All_Items_Lookup;
            codeManager = Code_Manager;
            iconList = Icon_Table;
            webSkin = HTML_Skin;
            itemsInTitle = Items_In_Title;
            base.Translator = Translator;

            // Set some defaults
            ipRestrict = -1;
            title = String.Empty;
            date = String.Empty;
            level1 = String.Empty;
            level2 = String.Empty;
            level3 = String.Empty;
            level1Order = -1;
            level2Order = -1;
            level3Order = -1;
            hierarchyCopiedFromDate = false;
            message = String.Empty;
            trackingBox = String.Empty;
            bornDigital = false;
            materialRecdDate = null;
            materialRecdNotes = String.Empty;
            dispositionAdvice = -1;
            dispositionAdviceNotes = String.Empty;


            // If the user cannot edit this item, go back
            if (!user.Can_Edit_This_Item(item))
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
            }

            // Determine the default template code 
            string template_code = "addvolume";
            if (!user.Include_Tracking_In_Standard_Forms)
                template_code = "addvolume_notracking";

            // Load this template
            template = Cached_Data_Manager.Retrieve_Template(template_code, Tracer);
            if (template != null)
            {
                Tracer.Add_Trace("Group_Add_Volume_MySobekViewer.Constructor", "Found template in cache");
            }
            else
            {
                Tracer.Add_Trace("Group_Add_Volume_MySobekViewer.Constructor", "Reading template file");

                // Read this template
                Template_XML_Reader reader = new Template_XML_Reader();
                template = new Template();
                reader.Read_XML(SobekCM_Library_Settings.Base_MySobek_Directory + "templates\\defaults\\" + template_code + ".xml", template, true);

                // Add the current codes to this template
                template.Add_Codes(Code_Manager);

                // Save this into the cache
                Cached_Data_Manager.Store_Template(template_code, template, Tracer);
            }

            // See if there was a hidden request
            string hidden_request = HttpContext.Current.Request.Form["action"] ?? String.Empty;

            // If this was a cancel request do that
            if (hidden_request == "cancel")
            {
                currentMode.Mode = Display_Mode_Enum.Item_Display;
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
            }
            else if (hidden_request.IndexOf("save") == 0 )
            {
                // Get the VID that used as a source for this
                string vid = HttpContext.Current.Request.Form["base_volume"];

                if (string.IsNullOrEmpty(vid))
                {
                     message = "<span style=\"color: red\"><strong>No base volume selected!</strong></span>";
                }
                else
                {
                    try
                    {
                        // Get a new instance of this item
                        SobekCM_Item saveItem = SobekCM_Item_Factory.Get_Item(Current_Mode.BibID, vid, Icon_Table, Tracer);
                        
                        // Clear some values for this item
                        saveItem.VID = String.Empty;
                        saveItem.Divisions.Clear();
                        saveItem.Behaviors.Serial_Info.Clear();
                        saveItem.Bib_Info.Series_Part_Info.Clear();
                        saveItem.Behaviors.Clear_Ticklers();
                        saveItem.Tracking.Internal_Comments = String.Empty;

                        // Save the template changes to this item
                        template.Save_To_Bib(saveItem, user, 1);

                        // Save this item and copy over
                        complete_item_submission(saveItem, Tracer);

                        // Clear the volume list
                        Cached_Data_Manager.Remove_Items_In_Title(saveItem.BibID, Tracer);

                        // Forward differently depending on request
                        switch (hidden_request)
                        {
                            case "save_edit":
                                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
                                currentMode.VID = saveItem.VID;
                                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                                break;

                            case "save_again":
                                // No redirect, but save values
                                date = saveItem.Bib_Info.Origin_Info.Date_Issued;
                                ipRestrict = saveItem.Behaviors.IP_Restriction_Membership;
                                trackingBox = saveItem.Tracking.Tracking_Box;
                                bornDigital = saveItem.Tracking.Born_Digital;
                                dispositionAdvice = saveItem.Tracking.Disposition_Advice;
                                dispositionAdviceNotes = saveItem.Tracking.Disposition_Advice_Notes;
                                materialRecdDate = saveItem.Tracking.Material_Received_Date;
                                materialRecdNotes = saveItem.Tracking.Material_Received_Notes;
                                if (!hierarchyCopiedFromDate)
                                {
                                    if (saveItem.Behaviors.Serial_Info.Count > 0)
                                    {
                                        level1 = saveItem.Behaviors.Serial_Info[0].Display;
                                        level1Order = saveItem.Behaviors.Serial_Info[0].Order;
                                    }
                                    if (saveItem.Behaviors.Serial_Info.Count > 1)
                                    {
                                        level2 = saveItem.Behaviors.Serial_Info[1].Display;
                                        level2Order = saveItem.Behaviors.Serial_Info[1].Order;
                                    }
                                    if (saveItem.Behaviors.Serial_Info.Count > 2)
                                    {
                                        level3 = saveItem.Behaviors.Serial_Info[2].Display;
                                        level3Order = saveItem.Behaviors.Serial_Info[2].Order;
                                    }
                                }
                                message = message + "<span style=\"color: blue\"><strong>Saved new volume ( " + saveItem.BibID + " : " + saveItem.VID + ")</strong></span>";
                                break;

                            //case "save_addfiles":
                            //    break;

                            default:
                                currentMode.Mode = Display_Mode_Enum.Item_Display;
                                currentMode.VID = saveItem.VID;
                                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                                break;
                        }

                    }
                    catch ( Exception ee )
                    {
                        message = message + "<br /><span style=\"color: red\"><strong>EXCEPTION CAUGHT!<br /><br />" + ee.Message + "<br /><br />" + ee.StackTrace.Replace("\n","<br />") + "</strong></span>";

                    }
                }
            }
        }

        #region Method commpletes the item submission on the way to the congratulations screen

        private void complete_item_submission(SobekCM_Item Item_To_Complete, Custom_Tracer Tracer)
        {
            // If this is a newspaper type, and the pubdate has a value, try to use that for the serial heirarchy
            if ((Item_To_Complete.Behaviors.Serial_Info.Count == 0) && (Item_To_Complete.Bib_Info.Origin_Info.Date_Issued.Length > 0) && (Item_To_Complete.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Newspaper ))
            {
                    DateTime asDateTime;
                    if (DateTime.TryParse(Item_To_Complete.Bib_Info.Origin_Info.Date_Issued, out asDateTime))
                    {
                        hierarchyCopiedFromDate = true;
                        Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(1, asDateTime.Year, asDateTime.Year.ToString());
                        switch (asDateTime.Month)
                        {
                            case 1:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "January");
                                break;

                            case 2:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "February");
                                break;

                            case 3:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "March");
                                break;

                            case 4:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "April");
                                break;

                            case 5:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "May");
                                break;

                            case 6:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "June");
                                break;

                            case 7:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "July");
                                break;

                            case 8:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "August");
                                break;

                            case 9:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "September");
                                break;

                            case 10:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "October");
                                break;

                            case 11:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "November");
                                break;

                            case 12:
                                Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(2, asDateTime.Month, "December");
                                break;
                        }

                        Item_To_Complete.Behaviors.Serial_Info.Add_Hierarchy(3, asDateTime.Day, asDateTime.Day.ToString());
                    }
            }

            // Determine the in process directory for this
            string user_in_process_directory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.UserName.Replace(".", "").Replace("@", "") + "\\newitem";
            if (user.UFID.Trim().Length > 0)
                user_in_process_directory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.UFID + "\\newitem";

            // Ensure this directory exists
            if (!Directory.Exists(user_in_process_directory))
                Directory.CreateDirectory(user_in_process_directory);

            // Now, delete all the files in the processing directory
            string[] all_files = Directory.GetFiles(user_in_process_directory);
            foreach (string thisFile in all_files)
            {
                File.Delete(thisFile);
            }

            // Save to the database
            Item_To_Complete.Web.File_Root = Item_To_Complete.BibID.Substring(0, 2) + "\\" + Item_To_Complete.BibID.Substring(2, 2) + "\\" + Item_To_Complete.BibID.Substring(4, 2) + "\\" + Item_To_Complete.BibID.Substring(6, 2) + "\\" + Item_To_Complete.BibID.Substring(8, 2);
            SobekCM_Database.Save_New_Digital_Resource(Item_To_Complete, false, false, user.UserName, String.Empty, -1);

            // Assign the file root and assoc file path
            Item_To_Complete.Web.AssocFilePath = Item_To_Complete.Web.File_Root + "\\" + Item_To_Complete.VID + "\\";

            // Create the static html pages
            string base_url = currentMode.Base_URL;
            try
            {
                Static_Pages_Builder staticBuilder = new Static_Pages_Builder(SobekCM_Library_Settings.System_Base_URL, SobekCM_Library_Settings.Base_Data_Directory, Translator, codeManager, itemList, iconList, webSkin);
                string filename = user_in_process_directory + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".html";
                staticBuilder.Create_Item_Citation_HTML(Item_To_Complete, filename, String.Empty);
            }
            catch (Exception ee)
            {
                message = message + "<br /><span style=\"color: red\"><strong>" + ee.Message + "<br />" + ee.StackTrace.Replace("\n", "<br />") + "</strong></span>";
            }

            currentMode.Base_URL = base_url;

            // Save the rest of the metadata
            Item_To_Complete.Source_Directory = user_in_process_directory;
            Item_To_Complete.Save_SobekCM_METS();
            Item_To_Complete.Save_Citation_Only_METS();

            // Add this to the cache
            itemList.Add_SobekCM_Item(Item_To_Complete);

            Database.SobekCM_Database.Add_Item_To_User_Folder(user.UserID, "Submitted Items", Item_To_Complete.BibID, Item_To_Complete.VID, 0, String.Empty, Tracer);

            // Save Bib_Level METS?
            //SobekCM.Resource_Object.Writers.OAI_Writer oaiWriter = new SobekCM.Resource_Object.Writers.OAI_Writer();
            //oaiWriter.Save_OAI_File(bibPackage, resource_folder + "\\oai_dc.xml", bibPackage.Processing_Parameters.Collection_Primary.ToLower(), createDate);

            // If there was no match, try to save to the tracking database
            Database.SobekCM_Database.Tracking_Online_Submit_Complete(Item_To_Complete.Web.ItemID, user.Full_Name, String.Empty);



            List<string> collectionnames = new List<string>();
            MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
            string Error_Message;
            Dictionary<string, object> options = new Dictionary<string, object>();
            options["MarcXML_File_ReaderWriter:Additional_Tags"] = Item_To_Complete.MARC_Sobek_Standard_Tags(collectionnames, true, SobekCM_Library_Settings.System_Name, SobekCM_Library_Settings.System_Abbreviation);
            marcWriter.Write_Metadata(Item_To_Complete.Source_Directory + "\\marc.xml", Item_To_Complete, options, out Error_Message);


            // Copy this to all the image servers
            SobekCM_Library_Settings.Refresh(Database.SobekCM_Database.Get_Settings_Complete(Tracer));
            string[] allFiles = Directory.GetFiles(user_in_process_directory);

            // Copy all the files over to the server 
            string serverNetworkFolder = SobekCM_Library_Settings.Image_Server_Network + Item_To_Complete.Web.AssocFilePath;
            // Create the folder
            if (!Directory.Exists(serverNetworkFolder))
                Directory.CreateDirectory(serverNetworkFolder);

            foreach (string thisFile in allFiles)
            {
                string destination_file = serverNetworkFolder + "\\" + (new FileInfo(thisFile)).Name;
                File.Copy(thisFile, destination_file, true);
            }

            // Add this to the cache
            itemList.Add_SobekCM_Item(Item_To_Complete);

            // Incrememnt the count of number of items submitted by this user
            user.Items_Submitted_Count++;

            // Delete any remaining items
            all_files = Directory.GetFiles(user_in_process_directory);
            foreach (string thisFile in all_files)
            {
                File.Delete(thisFile);
            }
        }

        #endregion

        /// <summary> Property indicates the standard navigation to be included at the top of the page by the
        /// main MySobek html subwriter. </summary>
        /// <value> This returns none since this viewer writes all the necessary navigational elements </value>
        /// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        /// administrative tabs should be included as well.  </remarks>
        public override MySobek_Included_Navigation_Enum Standard_Navigation_Type
        {
            get
            {
                return MySobek_Included_Navigation_Enum.NONE;
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the value 'Add New Volume' </value>
        public override string Web_Title
        {
            get { return "Add New Volume"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the template html is added in the <see cref="Add_HTML_In_Main_Form" /> method </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Group_Add_Volume_MySobekViewer.Write_HTML", "Do nothing");
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Group_Add_Volume_MySobekViewer.Add_HTML_In_Main_Form", "");

            Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
            Output.WriteLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");


            Output.WriteLine("<!-- Group_Add_Volume_MySobekViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <b>Add a new volume to this existing title/item group.</b>");
            Output.WriteLine("    <ul>");
            Output.WriteLine("      <li>Only enter data that you wish to override the data in the existing base volume.</li>");
            //Output.WriteLine("      <li>Clicking on the green plus button ( <img class=\"repeat_button\" src=\"" + currentMode.Base_URL + "default/images/new_element_demo.jpg\" /> ) will add another instance of the element, if the element is repeatable.</li>");
            Output.WriteLine("      <li>Click <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "help/addvolume\" target=\"_EDIT_INSTRUCTIONS\">here for detailed instructions</a> on adding new volumes online.</li>");
            Output.WriteLine("     </ul>");
            Output.WriteLine("</div>");
            Output.WriteLine();

            if (message.Length > 0)
            {
                Output.WriteLine("" + message + "<br />");
            }

            Output.WriteLine("<a name=\"template\"> </a>");
            Output.WriteLine("<div class=\"ViewsBrowsesRow\">");
            Output.WriteLine(Selected_Tab_Start + "NEW VOLUME" + Selected_Tab_End + " ");
            Output.WriteLine("</div>");
            Output.WriteLine("<div class=\"SobekEditPanel\">");
            Output.WriteLine("<!-- Add SAVE and CANCEL buttons to top of form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<table width=\"100%\">");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td width=\"20px\">&nbsp;</td>");

            // Output.WriteLine("    <td width=\"460px\"> Import metadata and behaviors from existing volume: &nbsp; ");
            Output.WriteLine("    <td width=\"300px\"> Import from existing volume: &nbsp; ");
            Output.WriteLine("      <select id=\"base_volume\" name=\"base_volume\" class=\"addvolume_base_volume\">");

            DataColumn vidColumn = itemsInTitle.Item_Table.Columns["VID"];
            bool first = true;
            DataView sortedView = new DataView(itemsInTitle.Item_Table) {Sort = "VID DESC"};
            foreach (DataRowView itemRowView in sortedView)
            {
                if (first)
                {
                    Output.WriteLine("        <option value=\"" + itemRowView.Row[vidColumn] + "\" selected=\"selected\">" + itemRowView.Row[vidColumn] + "</option>");
                    first = false;
                }
                else
                {
                    Output.WriteLine("        <option value=\"" + itemRowView.Row[vidColumn] + "\">" + itemRowView.Row[vidColumn] + "</option>");
                }
            }


            Output.WriteLine("      </select>");
            Output.WriteLine("    </td>");
            Output.WriteLine("    <td align=\"right\">");
            Output.WriteLine("      <a onmousedown=\"addvolume_cancel_form(); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CANCEL\" /></a> &nbsp; &nbsp; ");
            Output.WriteLine("      <a onmousedown=\"addvolume_save_form(''); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" alt=\"SAVE\" /></a> &nbsp; &nbsp; ");
            Output.WriteLine("      <a onmousedown=\"addvolume_save_form('_again'); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_add_another.gif\" alt=\"SAVE AND ADD ANOTHER\" /></a>");
            Output.WriteLine("    </td>");
            Output.WriteLine("    <td width=\"20px\">&nbsp;</td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");

            bool isMozilla = false;
            if (currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                isMozilla = true;

            // Create a new blank item for display purposes
            SobekCM_Item displayItem = new SobekCM_Item {BibID = item.BibID};
            displayItem.Behaviors.IP_Restriction_Membership = ipRestrict;
            displayItem.Behaviors.Serial_Info.Clear();
            displayItem.Tracking.Born_Digital = bornDigital;
            displayItem.Tracking.Tracking_Box = trackingBox;
            displayItem.Tracking.Material_Received_Notes = materialRecdNotes;
            displayItem.Tracking.Material_Received_Date = materialRecdDate;
            displayItem.Tracking.Disposition_Advice = dispositionAdvice;
            displayItem.Tracking.Disposition_Advice_Notes = dispositionAdviceNotes;
            if (title.Length > 0)
            {
                displayItem.Bib_Info.Main_Title.Clear();
                displayItem.Bib_Info.Main_Title.Title = title;
            }
            if (date.Length > 0)
                displayItem.Bib_Info.Origin_Info.Date_Issued = date;
            if ((level1.Length > 0) && (level1Order >= 0))
            {
                displayItem.Behaviors.Serial_Info.Add_Hierarchy(1, level1Order, level1);
                if ((level2.Length > 0) && (level2Order >= 0))
                {
                    displayItem.Behaviors.Serial_Info.Add_Hierarchy(2, level2Order, level2);
                    if ((level3.Length > 0) && (level3Order >= 0))
                    {
                        displayItem.Behaviors.Serial_Info.Add_Hierarchy(3, level3Order, level3);
                    }
                }
            }

            template.Render_Template_HTML(Output, displayItem, currentMode.Skin == currentMode.Default_Skin ? currentMode.Skin.ToUpper() : currentMode.Skin, isMozilla, user, currentMode.Language, Translator, currentMode.Base_URL, 1); 

            // Add the second buttons at the bottom of the form
            Output.WriteLine("<!-- Add SAVE and CANCEL buttons to bottom of form -->");
            Output.WriteLine("<table width=\"100%\">");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td width=\"480px\">&nbsp;</td>");
            Output.WriteLine("    <td align=\"right\">");
            Output.WriteLine("      <a onmousedown=\"addvolume_cancel_form(); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CANCEL\" /></a> &nbsp; &nbsp; ");
            Output.WriteLine("      <a onmousedown=\"addvolume_save_form(''); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" alt=\"SAVE\" /></a>");
            Output.WriteLine("    </td>");
            Output.WriteLine("    <td width=\"20px\">&nbsp;</td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
            Output.WriteLine("<hr />");
            Output.WriteLine("<table width=\"100%\" cellspacing=\"4px\" >");
            Output.WriteLine("  <tr height=\"25px\">");
            Output.WriteLine("    <td width=\"20px\">&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"2\">In addition, the following actions are available:</td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("  <tr height=\"30px\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td width=\"80px\">&nbsp;</td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <a onmousedown=\"addvolume_save_form('_edit'); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_edit.gif\" alt=\"SAVE AND EDIT ITEM\" /></a> &nbsp; &nbsp; ");
            Output.WriteLine("      <a onmousedown=\"addvolume_save_form('_addfiles'); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_add_files.gif\" alt=\"SAVE AND ADD FILES\" /></a> &nbsp; &nbsp; ");
//            Output.WriteLine("      <img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_add_files_disabled.gif\" alt=\"SAVE AND ADD FILES\" /> &nbsp; &nbsp; ");

            Output.WriteLine("      <a onmousedown=\"addvolume_save_form('_again'); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_add_another.gif\" alt=\"SAVE AND ADD ANOTHER\" /></a>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");

            Output.WriteLine("<br />");
            Output.WriteLine("</div>");
            Output.WriteLine("<br />");
        }
    }
}
