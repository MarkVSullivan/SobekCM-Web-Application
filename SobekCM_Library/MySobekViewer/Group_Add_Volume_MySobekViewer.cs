#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using SobekCM.Library.Settings;
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
		private readonly SobekCM_Skin_Collection skins;


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
		/// <param name="HTML_Skin_Collection"> HTML Web skin collection which controls the overall appearance of this digital library </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Group_Add_Volume_MySobekViewer(User_Object User,
            SobekCM_Navigation_Object Current_Mode,
            Item_Lookup_Object All_Items_Lookup,
            SobekCM_Item Current_Item, Aggregation_Code_Manager Code_Manager,
            Dictionary<string, Wordmark_Icon> Icon_Table,
            SobekCM_Skin_Object HTML_Skin,
            SobekCM_Items_In_Title Items_In_Title,
            Language_Support_Info Translator,
			SobekCM_Skin_Collection HTML_Skin_Collection,
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
	        skins = HTML_Skin_Collection;

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
                currentMode.Redirect();
                return;
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
                currentMode.Redirect();
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
                        SobekCM_Item saveItem = SobekCM_Item_Factory.Get_Item(Current_Mode.BibID, vid, Icon_Table, null, Tracer);
                        
                        // Clear some values for this item
                        saveItem.VID = String.Empty;
                        saveItem.Divisions.Clear();
                        saveItem.Behaviors.Serial_Info.Clear();
                        saveItem.Bib_Info.Series_Part_Info.Clear();
                        saveItem.Behaviors.Clear_Ticklers();
                        saveItem.Tracking.Internal_Comments = String.Empty;
				        saveItem.Bib_Info.Location.PURL = String.Empty;
						saveItem.Behaviors.Main_Thumbnail = String.Empty;
						saveItem.METS_Header.Create_Date = DateTime.Now;
						saveItem.METS_Header.Modify_Date = saveItem.METS_Header.Create_Date;
	                    saveItem.METS_Header.Creator_Software = "SobekCM Web - Online add a volume (derived from VID " + vid + ")";
						saveItem.METS_Header.Clear_Creator_Individual_Notes();
	                    saveItem.METS_Header.Creator_Individual = user.Full_Name;
	                    saveItem.Bib_Info.Location.Other_URL = String.Empty;
						saveItem.Bib_Info.Location.Other_URL_Display_Label = String.Empty;
						saveItem.Bib_Info.Location.Other_URL_Note = String.Empty;

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
                                currentMode.Redirect();
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

                            case "save_addfiles":
								currentMode.Mode = Display_Mode_Enum.My_Sobek;
								currentMode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
                                currentMode.VID = saveItem.VID;
                                currentMode.Redirect();
                                break;

                            default:
                                currentMode.Mode = Display_Mode_Enum.Item_Display;
                                currentMode.VID = saveItem.VID;
                                currentMode.Redirect();
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
            if (user.ShibbID.Trim().Length > 0)
                user_in_process_directory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.ShibbID + "\\newitem";

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
                Static_Pages_Builder staticBuilder = new Static_Pages_Builder(SobekCM_Library_Settings.System_Base_URL, SobekCM_Library_Settings.Base_Data_Directory, Translator, codeManager, iconList, skins, webSkin.Skin_Code);
                string filename = user_in_process_directory + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".html";
                staticBuilder.Create_Item_Citation_HTML(Item_To_Complete, filename, String.Empty);

				// Copy the static HTML file to the web server
				try
				{
					if (!Directory.Exists(SobekCM_Library_Settings.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8)))
						Directory.CreateDirectory(SobekCM_Library_Settings.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8));
					if (File.Exists(user_in_process_directory + "\\" + item.BibID + "_" + item.VID + ".html"))
						File.Copy(user_in_process_directory + "\\" + item.BibID + "_" + item.VID + ".html", SobekCM_Library_Settings.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8) + "\\" + item.BibID + "_" + item.VID + ".html", true);
				}
				catch (Exception)
				{
					// This is not critical
				}

            }
            catch (Exception ee)
            {
                message = message + "<br /><span style=\"color: red\"><strong>" + ee.Message + "<br />" + ee.StackTrace.Replace("\n", "<br />") + "</strong></span>";
            }

            currentMode.Base_URL = base_url;

            // Save the rest of the metadata
            Item_To_Complete.Source_Directory = user_in_process_directory;
            Item_To_Complete.Save_SobekCM_METS();

            // Add this to the cache
            itemList.Add_SobekCM_Item(Item_To_Complete);

            Database.SobekCM_Database.Add_Item_To_User_Folder(user.UserID, "Submitted Items", Item_To_Complete.BibID, Item_To_Complete.VID, 0, String.Empty, Tracer);

            // Save Bib_Level METS?
            //SobekCM.Resource_Object.Writers.OAI_Writer oaiWriter = new SobekCM.Resource_Object.Writers.OAI_Writer();
            //oaiWriter.Save_OAI_File(bibPackage, resource_folder + "\\oai_dc.xml", bibPackage.Processing_Parameters.Collection_Primary.ToLower(), createDate);

            // If there was no match, try to save to the tracking database
            Database.SobekCM_Database.Tracking_Online_Submit_Complete(Item_To_Complete.Web.ItemID, user.Full_Name, String.Empty);


			// Save the MARC file
            List<string> collectionnames = new List<string>();
            MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
            string errorMessage;
            Dictionary<string, object> options = new Dictionary<string, object>();
            options["MarcXML_File_ReaderWriter:Additional_Tags"] = Item_To_Complete.MARC_Sobek_Standard_Tags(collectionnames, true, SobekCM_Library_Settings.System_Name, SobekCM_Library_Settings.System_Abbreviation);
            marcWriter.Write_Metadata(Item_To_Complete.Source_Directory + "\\marc.xml", Item_To_Complete, options, out errorMessage);


            // Copy this to all the image servers
            SobekCM_Library_Settings.Refresh(Database.SobekCM_Database.Get_Settings_Complete(Tracer));
            

            // Copy all the files over to the server 
            string serverNetworkFolder = SobekCM_Library_Settings.Image_Server_Network + Item_To_Complete.Web.AssocFilePath;

            // Create the folder
            if (!Directory.Exists(serverNetworkFolder))
                Directory.CreateDirectory(serverNetworkFolder);
			if (!Directory.Exists(serverNetworkFolder + "\\" + SobekCM_Library_Settings.BACKUP_FILES_FOLDER_NAME))
				Directory.CreateDirectory(serverNetworkFolder + "\\" + SobekCM_Library_Settings.BACKUP_FILES_FOLDER_NAME);

			// Copy the static HTML page over first
			if (File.Exists(user_in_process_directory + "\\" + item.BibID + "_" + item.VID + ".html"))
			{
				File.Copy(user_in_process_directory + "\\" + item.BibID + "_" + item.VID + ".html", serverNetworkFolder + "\\" + SobekCM_Library_Settings.BACKUP_FILES_FOLDER_NAME + "\\" + item.BibID + "_" + item.VID + ".html", true);
				File.Delete(user_in_process_directory + "\\" + item.BibID + "_" + item.VID + ".html");
			}

			// Copy all the files 
			string[] allFiles = Directory.GetFiles(user_in_process_directory);
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
        /// <remarks> This class does nothing, since the template html is added in the <see cref="Write_ItemNavForm_Closing" /> method </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Group_Add_Volume_MySobekViewer.Write_HTML", "Do nothing");
        }

	    /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
	    /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
	    /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
	    public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
	    {
		    const string NEWVOLUME = "NEW VOLUME";

		    Tracer.Add_Trace("Group_Add_Volume_MySobekViewer.Write_ItemNavForm_Closing", "");

		    Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
		    Output.WriteLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");

		    Output.WriteLine("<div id=\"sbkIsw_Titlebar\">");

		    string grouptitle = item.Behaviors.GroupTitle;
		    if (grouptitle.Length > 125)
		    {
			    Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + grouptitle + "\">" + grouptitle.Substring(0, 120) + "...</abbr></h1>");
		    }
		    else
		    {
			    Output.WriteLine("\t<h1 itemprop=\"name\">" + grouptitle + "</h1>");
		    }

		    Output.WriteLine("</div>");
			Output.WriteLine("<div class=\"sbkMenu_Bar\" id=\"sbkIsw_MenuBar\" style=\"height:20px\">&nbsp;</div>");

		    Output.WriteLine("<div id=\"container-inner1000\">");
		    Output.WriteLine("<div id=\"pagecontainer\">");


		    Output.WriteLine("<!-- Group_Add_Volume_MySobekViewer.Write_ItemNavForm_Closing -->");
		    Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
		    Output.WriteLine("  <br />");
		    Output.WriteLine("  <h2>Add a new volume to this existing title/item group</h2>");
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
		    Output.WriteLine("<div id=\"tabContainer\" class=\"fulltabs\">");
		    Output.WriteLine("  <div class=\"tabs\">");
		    Output.WriteLine("    <ul>");
		    Output.WriteLine("      <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + NEWVOLUME + "</li>");
		    Output.WriteLine("    </ul>");
		    Output.WriteLine("  </div>");
		    Output.WriteLine("  <div class=\"graytabscontent\">");
		    Output.WriteLine("    <div class=\"tabpage\" id=\"tabpage_1\">");

		    Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to top of form -->");
		    Output.WriteLine("      <script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
		    Output.WriteLine();

		    Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
		    Output.WriteLine("        <button onclick=\"addvolume_cancel_form(); return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
		    Output.WriteLine("        <button onclick=\"addvolume_save_form(''); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button> &nbsp; &nbsp; ");
		    Output.WriteLine("        <button onclick=\"addvolume_save_form('_again'); return false;\" class=\"sbkMySobek_BigButton\">SAVE & ADD ANOTHER</button>");
		    Output.WriteLine("      </div>");
		    Output.WriteLine("      <br /><br />");
		    Output.WriteLine();

		    // Output.WriteLine("    <td width=\"460px\"> Import metadata and behaviors from existing volume: &nbsp; ");
		    Output.WriteLine("      <div style=\"text-align:left;padding-left:58px; padding-bottom: 10px;\">Import from existing volume: &nbsp; ");
		    Output.WriteLine("        <select id=\"base_volume\" name=\"base_volume\" class=\"addvolume_base_volume\">");

		    DataColumn vidColumn = itemsInTitle.Item_Table.Columns["VID"];
		    bool first = true;
		    DataView sortedView = new DataView(itemsInTitle.Item_Table) {Sort = "VID DESC"};
		    foreach (DataRowView itemRowView in sortedView)
		    {
			    if (first)
			    {
				    Output.WriteLine("          <option value=\"" + itemRowView.Row[vidColumn] + "\" selected=\"selected\">" + itemRowView.Row[vidColumn] + "</option>");
				    first = false;
			    }
			    else
			    {
				    Output.WriteLine("          <option value=\"" + itemRowView.Row[vidColumn] + "\">" + itemRowView.Row[vidColumn] + "</option>");
			    }
		    }
		    Output.WriteLine("        </select>");
		    Output.WriteLine("      </div>");
		    Output.WriteLine();

		    bool isMozilla = currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0;

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
		    Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to bottom of form -->");
		    Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
		    Output.WriteLine("        <button onclick=\"addvolume_cancel_form(); return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
		    Output.WriteLine("        <button onclick=\"addvolume_save_form(''); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button> &nbsp; &nbsp; ");
		    Output.WriteLine("      </div>");
		    Output.WriteLine();

		    Output.WriteLine("      <br /><br />");
		    Output.WriteLine("      <hr />");
		    Output.WriteLine();

		    Output.WriteLine("      <p>In addition, the following actions are available:</p>");
		    Output.WriteLine("      <button onclick=\"addvolume_save_form('_edit'); return false;\" class=\"sbkMySobek_RoundButton\">SAVE & EDIT ITEM</button> &nbsp; &nbsp; ");
		    Output.WriteLine("      <button onclick=\"addvolume_save_form('_addfiles'); return false;\" class=\"sbkMySobek_RoundButton\">SAVE & ADD FILES</button> &nbsp; &nbsp; ");
		    Output.WriteLine("      <button onclick=\"addvolume_save_form('_again'); return false;\" class=\"sbkMySobek_RoundButton\">SAVE & ADD ANOTHER</button>");

		    Output.WriteLine("    </div>");
		    Output.WriteLine("  </div>");
		    Output.WriteLine("</div>");
		    Output.WriteLine("</div>");
		    Output.WriteLine("</div>");
	    }

	    /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
		/// requests from the main HTML subwriter. </summary>
		/// <value> This tells the HTML and mySobek writers to mimic the item viewer </value>
		public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
		{
			get
			{
				return new List<HtmlSubwriter_Behaviors_Enum>
				{
					HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter,
					HtmlSubwriter_Behaviors_Enum.Suppress_Banner
				};
			}
		}
    }
}
