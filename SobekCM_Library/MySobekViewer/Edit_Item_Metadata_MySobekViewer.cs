#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Library.Application_State;
using SobekCM.Library.Citation.Template;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated user to edit a digital resource online, using various possible templates </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the item for editing</li>
    /// <li>This viewer uses the <see cref="SobekCM.Library.Citation.Template.Template"/> class to display the correct elements for editing </li>
    /// </ul></remarks>
    public class Edit_Item_Metadata_MySobekViewer : abstract_MySobekViewer
    {
        private readonly Aggregation_Code_Manager codeManager;
        private readonly Dictionary<string, Wordmark_Icon> iconList;
        private readonly bool isProject;
        private readonly SobekCM_Item item;
        private readonly Item_Lookup_Object itemList;
        private readonly double page;
        private string popUpFormsHtml;
        private readonly Template template;
        private readonly SobekCM_Skin_Object webSkin;
	    private readonly string delayed_popup;

        #region Constructor

        /// <summary> Constructor for a new instance of the Edit_Item_Metadata_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="All_Items_Lookup"> Allows individual items to be retrieved by various methods as <see cref="Single_Item"/> objects.</param>
        /// <param name="Current_Item"> Individual digital resource to be edited by the user </param>
        /// <param name="Code_Manager"> Code manager contains the list of all valid aggregation codes </param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Icon_Table"> Dictionary of all the wordmark/icons which can be tagged to the items </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Edit_Item_Metadata_MySobekViewer(User_Object User,
                                                SobekCM_Navigation_Object Current_Mode, 
                                                Item_Lookup_Object All_Items_Lookup,
                                                SobekCM_Item Current_Item, Aggregation_Code_Manager Code_Manager,
                                                Dictionary<string, Wordmark_Icon> Icon_Table,
                                                SobekCM_Skin_Object HTML_Skin,
                                                Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", String.Empty);

            currentMode = Current_Mode;
            item = Current_Item;
            itemList = All_Items_Lookup;
            codeManager = Code_Manager;
            iconList = Icon_Table;
            webSkin = HTML_Skin;
            popUpFormsHtml = String.Empty;
	        delayed_popup = String.Empty;


            // If the user cannot edit this item, go back
            if (!user.Can_Edit_This_Item( item ))
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
            }

            // Is this a project
            isProject = item.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project;

	        string template_code = user.Edit_Template_Code;
            if ((isProject) || (item.Contains_Complex_Content) || (item.Using_Complex_Template))
            {
                template_code = user.Edit_Template_MARC_Code;
            }
            template = Cached_Data_Manager.Retrieve_Template(template_code, Tracer);
            if (template != null)
            {
                Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Found template in cache");
            }
            else
            {
                Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Reading template file");

                // Read this template
                Template_XML_Reader reader = new Template_XML_Reader();
                template = new Template();
                reader.Read_XML( SobekCM_Library_Settings.Base_MySobek_Directory + "templates\\" + template_code + ".xml", template, true);

                // Add the current codes to this template
                template.Add_Codes(Code_Manager);

                // Save this into the cache
                Cached_Data_Manager.Store_Template(template_code, template, Tracer);
            }

            // Get the current page number, or default to 1
            page = 1;
            if (currentMode.My_Sobek_SubMode.Length > 0)
            {
                if ((currentMode.My_Sobek_SubMode == "preview") || (currentMode.My_Sobek_SubMode == "marc") || (currentMode.My_Sobek_SubMode == "mets"))
                {
                    page = 0;
                }
                else
                {
                    page = 1;
                    bool isNumber = currentMode.My_Sobek_SubMode.All(Char.IsNumber);
                    if (isNumber)
                    {
                        if (isProject)
                            Double.TryParse(currentMode.My_Sobek_SubMode[0].ToString(), out page);
                        else
                            Double.TryParse(currentMode.My_Sobek_SubMode, out page);
                    }
                    else if ( isProject )
                    { 
                        if ( Char.IsNumber(currentMode.My_Sobek_SubMode[0]))
                            Double.TryParse(currentMode.My_Sobek_SubMode[0].ToString(), out page);
                    }
                }
            }

			// Handle post backs
	        if (Current_Mode.isPostBack)
	        {
		        // See if there was a hidden request
		        string hidden_request = HttpContext.Current.Request.Form["new_element_requested"] ?? String.Empty;

		        // If this was a cancel request do that
		        if (hidden_request == "cancel")
		        {
			        if (isProject)
			        {
				        Cached_Data_Manager.Remove_Project(user.UserID, item.BibID, null);

				        currentMode.Mode = Display_Mode_Enum.Administrative;
				        currentMode.Admin_Type = Admin_Type_Enum.Default_Metadata;
				        currentMode.My_Sobek_SubMode = String.Empty;
				        currentMode.Redirect();
			        }
			        else
			        {
				        Cached_Data_Manager.Remove_Digital_Resource_Object(user.UserID, item.BibID, item.VID, null);

				        currentMode.Mode = Display_Mode_Enum.Item_Display;
				        currentMode.Redirect();
			        }
			        return;
		        }


		        // Save these changes to bib
		        template.Save_To_Bib(item, user, ((int) page));

		        // See if the user asked for a new element of a complex form type
		        delayed_popup = String.Empty;
		        switch (hidden_request.Trim())
		        {
			        case "name":
				        delayed_popup = "name";
				        item.Bib_Info.Add_Named_Entity(String.Empty).Name_Type = Name_Info_Type_Enum.personal;
				        break;

			        case "title":
				        delayed_popup = "title";
				        item.Bib_Info.Add_Other_Title(String.Empty, Title_Type_Enum.alternative);
				        break;

			        case "subject":
				        delayed_popup = "subject";
				        item.Bib_Info.Add_Subject();
				        break;

			        case "spatial":
				        delayed_popup = "spatial";
				        item.Bib_Info.Add_Hierarchical_Geographic_Subject();
				        break;

			        case "relateditem":
				        delayed_popup = "relateditem";
				        item.Bib_Info.Add_Related_Item(new Related_Item_Info());
				        break;

			        case "save":
				        Complete_Item_Save();
				        break;

					case "complicate":
						item.Using_Complex_Template = true;
				        HttpContext.Current.Response.Redirect( "?" + HttpContext.Current.Request.QueryString, false);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						currentMode.Request_Completed = true;
				        return;

					case "simplify":
						item.Using_Complex_Template = false;
				        HttpContext.Current.Response.Redirect( "?" + HttpContext.Current.Request.QueryString, false);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						currentMode.Request_Completed = true;
				        return;
		        }

				// Was this for a new page?
				if (hidden_request.IndexOf("newpage") == 0)
				{
					string page_requested = hidden_request.Replace("newpage", "");
					if (page_requested != currentMode.My_Sobek_SubMode)
					{
						// forward to requested page
						currentMode.My_Sobek_SubMode = page_requested;
						if (currentMode.My_Sobek_SubMode == "0")
							currentMode.My_Sobek_SubMode = "preview";
						if (isProject)
							currentMode.My_Sobek_SubMode = page_requested + item.BibID;

						HttpContext.Current.Response.Redirect(currentMode.Redirect_URL() + "#template", false);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						currentMode.Request_Completed = true;
					}
				}
	        }
        }

        #endregion

        /// <summary> Property indicates if this mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        ///<value> This mySobek viewer always returns the value TRUE </value>
        public override bool Contains_Popup_Forms
        {
            get
            {
                return true;
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the value 'Edit Item' or 'Edit Project' </value>
        public override string Web_Title
        {
            get {
                return isProject ? "Edit Project" : "Edit Item";
            }
        }

		public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			// DO nothing
		}

		public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			Output.WriteLine("<!-- Edit_Item_Metadata_MySobekViewer.Add_Controls -->");
			Output.WriteLine("<div class=\"SobekHomeText\">");
			Output.WriteLine("  <br />");
			if (!isProject)
			{
				Output.WriteLine("  <b>Edit this item</b>");
				Output.WriteLine("    <ul>");
				Output.WriteLine("      <li>Enter the data for this item below and press the SAVE button when all your edits are complete.</li>");
				Output.WriteLine("      <li>Clicking on the green plus button ( <img class=\"repeat_button\" src=\"" + currentMode.Base_URL + "default/images/new_element_demo.jpg\" /> ) will add another instance of the element, if the element is repeatable.</li>");

				if ((item.Using_Complex_Template) || (item.Contains_Complex_Content))
				{
					if (item.Contains_Complex_Content)
					{
						Output.WriteLine("      <li>You are using the full editing form because this item contains complex elements or was derived from MARC.</li>");
					}
					else
					{
						Output.Write("      <li>You are using the full editing form.  Click");
						Output.Write("<a href=\"#\" onclick=\"editmetadata_simplify();return false;\">here to return to the simplified version</a>.");
						Output.WriteLine("</li>");
					}
				}
				else
				{
					Output.WriteLine("      <li>You are using the simplified editing form.  Click");
					Output.Write("<a href=\"#\" onclick=\"editmetadata_complicate();return false;\">here to use the full form</a>.");
					Output.WriteLine("</li>");
				}

				if (template.Code.ToUpper().IndexOf("MARC") > 0)
				{
					Output.WriteLine("      <li>To open detailed edit forms, click on the linked metadata values.</li>");
				}
				Output.WriteLine("      <li>Click <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "help/editinstructions\" target=\"_EDIT_INSTRUCTIONS\">here for detailed instructions</a> on editing metadata online.</li>");
			}
			else
			{
				Output.WriteLine("  <b>Edit this project</b>");
				Output.WriteLine("    <ul>");
				Output.WriteLine("      <li>Enter the default data for this project below and press the SAVE button when all your edits are complete.</li>");
				Output.WriteLine("      <li>Clicking on the blue plus signs ( <img class=\"repeat_button\" src=\"" + currentMode.Base_URL + "default/images/new_element_demo.jpg\" /> ) will add another instance of the element, if the element is repeatable.</li>");
				Output.WriteLine("      <li>Click on the element names for detailed information inluding definitions, best practices, and technical information.</li>");
				Output.WriteLine("      <li>You are using the full editing form because you are editing a project.</li>");
			}

			Output.WriteLine("     </ul>");
			Output.WriteLine("</div>");
			Output.WriteLine();

			Output.WriteLine("<!-- Buttons to select each template page -->");
			Output.WriteLine("<a name=\"template\"> </a>");
			Output.WriteLine("<div id=\"tabContainer\" class=\"fulltabs\">");
			Output.WriteLine("  <div class=\"tabs\">");
			Output.WriteLine("    <ul>");


			int page_iterator = 1;
			string current_submode = currentMode.My_Sobek_SubMode;
			if (current_submode.Length == 0)
				current_submode = "1";
			while (page_iterator <= template.InputPages.Count)
			{
				if (current_submode[0].ToString() == page_iterator.ToString())
					Output.Write("      <li class=\"tabActiveHeader\">" + template.InputPages[page_iterator - 1].Title + "</li>");
				else
					Output.Write("      <li onclick=\"editmetadata_newpage('" + page_iterator + "');\">" + template.InputPages[page_iterator - 1].Title + "</li>");

				page_iterator++;
			}

			bool preview = false;
			if (!isProject)
			{
				if (page < 1)
				{
					preview = true;
					Output.Write("      <li class=\"tabActiveHeader\">Preview</li>");
				}
				else
					Output.Write("      <li onclick=\"editmetadata_newpage('0');\">Preview</li>");
			}

			currentMode.My_Sobek_SubMode = current_submode;

			Output.WriteLine("    </ul>");
			Output.WriteLine("  </div>");
			Output.WriteLine("  <div class=\"graytabscontent\">");
			Output.WriteLine("    <div class=\"tabpage\" id=\"tabpage_1\">");

			// Add the first buttons
			Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to top of form -->");
			Output.WriteLine("      <script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
			Output.WriteLine();
			Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
			Output.WriteLine("        <button onclick=\"editmetadata_cancel_form();return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button onclick=\"editmetadata_save_form();return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </div>");
			Output.WriteLine("      <br /><br />");
			Output.WriteLine();


			Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Add_Controls", "Render template html");
			if (!preview)
			{
				if (page >= 1)
				{
					bool isMozilla = currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0;

					popUpFormsHtml = template.Render_Template_HTML(Output, item, currentMode.Skin == currentMode.Default_Skin ? currentMode.Skin.ToUpper() : currentMode.Skin, isMozilla, user, currentMode.Language, Translator, currentMode.Base_URL, ((int)page));
				}
			}
			else
			{
				show_preview(Output, currentMode.My_Sobek_SubMode, Tracer);
			}

			// Add the second buttons at the bottom of the form
			Output.WriteLine();
			Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to bottom of form -->");
			Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
			Output.WriteLine("        <button onclick=\"behaviors_cancel_form(); return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button onclick=\"behaviors_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </div>");
			Output.WriteLine("      <br />");
			Output.WriteLine("    </div>");
			Output.WriteLine("  </div>");
			Output.WriteLine("</div>");
			Output.WriteLine("<br />");
			Output.WriteLine();

		}


        /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds any popup divisions for form metadata elements </remarks>
        public override void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Add_Popup_HTML", "Add any popup divisions for form elements");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
            Output.WriteLine("<input type=\"hidden\" id=\"new_element_requested\" name=\"new_element_requested\" value=\"\" />");
            Output.WriteLine();
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");

            if (popUpFormsHtml.Length > 0)
            {
                //Output.WriteLine("<!-- Blankets out the rest of the web form when a pop-up form is envoked -->");
                //Output.WriteLine("<div id=\"blanket_outer\" style=\"display:none;\"></div>");
                //Output.WriteLine();

                Output.WriteLine(popUpFormsHtml);
            }

			// If there was a delayed popup requested, do that now
			if (!String.IsNullOrEmpty(delayed_popup))
			{
				switch (delayed_popup)
				{

					case "relateditem":
						int related_item_count = item.Bib_Info.RelatedItems_Count;
						if (related_item_count > 0)
						{
							Output.WriteLine("<!-- Popup the delayed box -->");
							Output.WriteLine("<script type=\"text/javascript\">popup_focus('form_related_item_" + related_item_count + "', 'form_related_item_term_" + related_item_count + "', 'form_relateditem_title_" + related_item_count + "', 575, 620 );</script>");
							Output.WriteLine();
						}
						break;

					case "subject":
						int subject_index = 0;
						if (item.Bib_Info.Subjects_Count > 0)
						{
							subject_index += item.Bib_Info.Subjects.Count(ThisSubject => ThisSubject.Class_Type == Subject_Info_Type.Standard);
						}
						if (subject_index > 0)
						{
							Output.WriteLine("<!-- Popup the delayed box -->");
							Output.WriteLine("<script type=\"text/javascript\">popup_focus('form_subject_" + subject_index + "', 'form_subject_term_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "', 310, 600 );</script>");
							Output.WriteLine();
						}
						break;

					case "spatial":
						int spatial_index = 0;
						if (item.Bib_Info.Subjects_Count > 0)
						{
							spatial_index += item.Bib_Info.Subjects.Count(ThisSubject => ThisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial);
						}
						if (spatial_index > 0)
						{
							Output.WriteLine("<!-- Popup the delayed box -->");
							Output.WriteLine("<script type=\"text/javascript\">popup_focus('form_spatial_" + spatial_index + "', 'form_spatial_term_" + spatial_index + "', 'formspatialcontinent_" + spatial_index + "', 590, 550 );</script>");
							Output.WriteLine();
						}
						break;

					case "name":
						int name_count = 0;
						if ((item.Bib_Info.hasMainEntityName) && (item.Bib_Info.Main_Entity_Name.hasData))
							name_count++;
						name_count += item.Bib_Info.Names_Count;

						if (name_count > 0)
						{
							Output.WriteLine("<!-- Popup the delayed box -->");
							Output.WriteLine("<script type=\"text/javascript\">popup_focus('form_name_" + name_count + "', 'form_name_line_" + name_count + "', 'form_name_full_" + name_count + "', 475, 700 );</script>");
							Output.WriteLine();
						}
						break;

					case "title":
						int title_count = item.Bib_Info.Other_Titles_Count;
						if ((item.Bib_Info.hasSeriesTitle) && (item.Bib_Info.SeriesTitle.Title.Length > 0))
							title_count++;
						if (title_count > 0)
						{
							Output.WriteLine("<!-- Popup the delayed box -->");
							Output.WriteLine("<script type=\"text/javascript\">popup_focus('form_othertitle_" + title_count + "', 'form_othertitle_line_" + title_count + "', 'formothertitletitle_" + title_count + "', 295, 675 );</script>");
							Output.WriteLine();
						}
						break;
				}
			}
        }

        void complicateButton_Click(object Sender, EventArgs E)
        {

        }

        void simplifyButton_Click(object Sender, EventArgs E)
        {
            if (!item.Contains_Complex_Content)
                item.Using_Complex_Template = false;

            HttpContext.Current.Response.Redirect( "?" + HttpContext.Current.Request.QueryString, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            currentMode.Request_Completed = true;
        }

        #region Method to complete a save to SobekCM

        private void Complete_Item_Save()
        {
            if (isProject)
            {
                // Save the new project METS
                item.Save_METS();

                // Clear the cache of this item
                Cached_Data_Manager.Remove_Project(user.UserID, item.BibID, null);

                // Redirect
                currentMode.Mode = Display_Mode_Enum.Administrative;
                currentMode.Admin_Type = Admin_Type_Enum.Default_Metadata;
                currentMode.My_Sobek_SubMode = String.Empty;
                currentMode.Redirect();
            }
            else
            {
                // Determine the in process directory for this
                string user_bib_vid_process_directory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.UFID + "\\metadata_updates\\" + item.BibID + "_" + item.VID;
                if (user.UFID.Trim().Length == 0)
                    user_bib_vid_process_directory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.UserName.Replace(".", "").Replace("@", "") + "\\metadata_updates\\" + item.BibID + "_" + item.VID;

                // Ensure the folder exists and is empty to start with
                if (!Directory.Exists(user_bib_vid_process_directory))
                    Directory.CreateDirectory(user_bib_vid_process_directory);
                else
                {
                    // Anything older than a day should be deleted
                    string[] files = Directory.GetFiles(user_bib_vid_process_directory);
                    foreach (string thisFile in files)
                    {
                        try
                        {
                            File.Delete(thisFile);
                        }
                        catch (Exception)
                        {
                            // Not much to do here
                        }
                    }
                }

                // Update the METS file with METS note and name
                item.METS_Header.Creator_Individual = user.UserName;
                item.METS_Header.Modify_Date = DateTime.Now;
                item.METS_Header.RecordStatus_Enum = METS_Record_Status.METADATA_UPDATE;

                // Save the METS file and related items
                bool successful_save = true;
                try
                {
                    SobekCM_Database.Save_Digital_Resource(item, DateTime.Now, true);
                }
                catch
                {
                    successful_save = false;
                }

                // Create the static html pages
                string base_url = currentMode.Base_URL;
                try
                {
                    Static_Pages_Builder staticBuilder = new Static_Pages_Builder(SobekCM_Library_Settings.System_Base_URL, SobekCM_Library_Settings.Base_Data_Directory, Translator, codeManager, itemList, iconList, webSkin);
                    string filename = user_bib_vid_process_directory + "\\" + item.BibID + "_" + item.VID + ".html";
                    staticBuilder.Create_Item_Citation_HTML(item, filename, SobekCM_Library_Settings.Image_Server_Network + item.Web.AssocFilePath);
                }
                catch (Exception ee)
                {
                    // Failing to make the static page is not the worst thing in the world...
                }

                currentMode.Base_URL = base_url;

                item.Source_Directory = user_bib_vid_process_directory;
                item.Save_SobekCM_METS();
                item.Save_Citation_Only_METS();


                // If this was not able to be saved in the UFDC database, try it again
                if (!successful_save)
                {
                    SobekCM_Database.Save_Digital_Resource(item, DateTime.Now, false);
                }

                // Make sure the progress has been added to this item's work log
                try
                {
                    Database.SobekCM_Database.Tracking_Online_Edit_Complete(item.Web.ItemID, user.Full_Name, String.Empty);
                }
                catch(Exception)
                {
                    // This is not critical
                }

                List<string> collectionnames = new List<string>();
                MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                string errorMessage;
                Dictionary<string, object> options = new Dictionary<string, object>();
                options["MarcXML_File_ReaderWriter:Additional_Tags"] = item.MARC_Sobek_Standard_Tags(collectionnames, true, SobekCM_Library_Settings.System_Name, SobekCM_Library_Settings.System_Abbreviation);
                marcWriter.Write_Metadata(item.Source_Directory + "\\marc.xml", item, options, out errorMessage);

                // Copy this to all the image servers
                SobekCM_Library_Settings.Refresh(Database.SobekCM_Database.Get_Settings_Complete(null));
                string[] allFiles = Directory.GetFiles(user_bib_vid_process_directory);

                string serverNetworkFolder = SobekCM_Library_Settings.Image_Server_Network + item.Web.AssocFilePath;

                // Create the folder
                if (!Directory.Exists(serverNetworkFolder))
                    Directory.CreateDirectory(serverNetworkFolder);
                else
                {
                    // Rename any existing standard mets to keep a backup
                    if (File.Exists(serverNetworkFolder + "\\" + item.BibID + "_" + item.VID + ".mets.xml"))
                    {
                        FileInfo currentMetsFileInfo = new FileInfo(serverNetworkFolder + "\\" + item.BibID + "_" + item.VID + ".mets.xml");
                        DateTime lastModDate = currentMetsFileInfo.LastWriteTime;
                        File.Copy(serverNetworkFolder + "\\" + item.BibID + "_" + item.VID + ".mets.xml", serverNetworkFolder + "\\" + item.BibID + "_" + item.VID + "_" + lastModDate.Year + "_" + lastModDate.Month + "_" + lastModDate.Day + ".mets.bak", true);
                    }
                }

                foreach (string thisFile in allFiles)
                {
                    string destination_file = serverNetworkFolder + "\\" + (new FileInfo(thisFile)).Name;
                    File.Copy(thisFile, destination_file, true);
                }

                // Copy the static HTML file as well
                try
                {
                    if (!Directory.Exists(SobekCM_Library_Settings.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8)))
                        Directory.CreateDirectory(SobekCM_Library_Settings.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8));
                    if (File.Exists(user_bib_vid_process_directory + "\\" + item.BibID + "_" + item.VID + ".html"))
                        File.Copy(user_bib_vid_process_directory + "\\" + item.BibID + "_" + item.VID + ".html", SobekCM_Library_Settings.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8) + "\\" + item.BibID + "_" + item.VID + ".html", true);
                }
                catch (Exception)
                {
                    // This is not critical
                }

                // Add this to the cache
                itemList.Add_SobekCM_Item(item, false);

                // Now, delete all the files here
                string[] all_files = Directory.GetFiles(user_bib_vid_process_directory);
                foreach (string thisFile in all_files)
                {
                    File.Delete(thisFile);
                }

                // Clear the user-specific and global cache of this item 
                Cached_Data_Manager.Remove_Digital_Resource_Object(user.UserID, item.BibID, item.VID, null);
                Cached_Data_Manager.Remove_Digital_Resource_Object(item.BibID, item.VID, null);
                Cached_Data_Manager.Remove_Items_In_Title(item.BibID, null);


                // Forward to the display item again
                currentMode.Mode = Display_Mode_Enum.Item_Display;
                currentMode.ViewerCode = "citation";
                currentMode.Redirect();
            }
        }

        #endregion

        #region Method to add the preview html

        private void show_preview(TextWriter Output, string Preview_Mode, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.show_preview", String.Empty);

            Output.WriteLine("<script language=\"JavaScript\">");

            // Get the URL to use for forwarding
            string current_submode = currentMode.My_Sobek_SubMode;
            currentMode.My_Sobek_SubMode = "ZZZZZ";
            string redirect_url = currentMode.Redirect_URL();
            currentMode.My_Sobek_SubMode = current_submode;

            Output.WriteLine("  function preview1() {if (document.itemNavForm.pickme) location='" + redirect_url.Replace("ZZZZZ", "preview") + "#template'}");
            Output.WriteLine("  function preview2() {if (document.itemNavForm.pickme) location='" + redirect_url.Replace("ZZZZZ", "marc") + "#template'}");
            Output.WriteLine("  function preview3() {if (document.itemNavForm.pickme) location='" + redirect_url.Replace("ZZZZZ", "mets") + "#template'}");
            Output.WriteLine("</script>");

            Output.WriteLine("<center>");
            Output.WriteLine(Preview_Mode == "preview"
								 ? "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfPreview\" checked=\"checked\" /><label for=\"sbkTypeOfPreview\">Standard View</label> &nbsp; &nbsp; "
								 : "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfPreview\" onchange=\"location='" + redirect_url.Replace("ZZZZZ", "preview") + "#template';\" /><label for=\"sbkTypeOfPreview\">Standard View</label> &nbsp; &nbsp; ");

            Output.WriteLine(Preview_Mode == "marc"
								 ? "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfMarc\" checked=\"checked\" /><label for=\"sbkTypeOfMarc\">MARC View</label> &nbsp; &nbsp; "
								 : "<input TYPE=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfMarc\" onchange=\"location='" + redirect_url.Replace("ZZZZZ", "marc") + "#template';\" /><label for=\"sbkTypeOfMarc\">MARC View</label> &nbsp; &nbsp; ");

            Output.WriteLine(Preview_Mode == "mets"
								 ? "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfMets\" checked=\"checked\" /><label for=\"sbkTypeOfMets\">METS View</label>"
								 : "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfMets\" onchange=\"location='" + redirect_url.Replace("ZZZZZ", "mets") + "#template';\" /><label for=\"sbkTypeOfMets\">METS View</label>");

            Output.WriteLine("</center>");
            Output.WriteLine("<br />");

            switch (Preview_Mode)
            {
                case "marc":
					Output.WriteLine("<div class=\"sbkEimv_Citation\">");
                    Citation_ItemViewer marcViewer = new Citation_ItemViewer(Translator, codeManager, false)
                                                         {CurrentItem = item, CurrentMode = currentMode};
                    Output.WriteLine(marcViewer.MARC_String("735px", Tracer));
                    break;

                case "mets":
					Output.WriteLine("<div class=\"sbkEimv_Citation\" >");
                  //  Output.WriteLine("<table width=\"950px\"><tr><td width=\"950px\">");
                    StringBuilder mets_builder = new StringBuilder(2000);
                    StringWriter mets_output = new StringWriter(mets_builder);

                    METS_File_ReaderWriter metsWriter = new METS_File_ReaderWriter();

                    string errorMessage;
                    metsWriter.Write_Metadata(mets_output, item, null, out errorMessage);
                    string mets_string = mets_builder.ToString();
                    string header = mets_string.Substring(0, mets_string.IndexOf("<METS:mets"));
                    string remainder = mets_string.Substring(header.Length);
                    Output.WriteLine(header.Replace("<?", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt?").Replace("?>", "?&gt;&AAA;/span&ZZZ;").Replace("<!--", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt!--&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Gray;&QQQ;&ZZZ;").Replace("-->", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;--&gt;&AAA;/span&ZZZ;").Replace("\r", "<br />").Replace("&AAA;", "<").Replace("&ZZZ;", ">").Replace("&QQQ;", "\""));
                    Output.WriteLine(remainder.Replace("<?", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt?").Replace("?>", "?&gt;&AAA;/span&ZZZ;").Replace("<!--", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt!--&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Gray;&QQQ;&ZZZ;").Replace("-->", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;--&gt;&AAA;/span&ZZZ;").Replace("</", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt;/&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Maroon;&QQQ;&ZZZ;").Replace("<", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt;&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Maroon;&QQQ;&ZZZ;").Replace("=\"", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;=&quot;&AAA;/span&ZZZ;").Replace("\">", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&quot;&gt;&AAA;/span&ZZZ;").Replace("\"", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&quot;&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Maroon;&QQQ;&ZZZ;").Replace("/>", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;/&gt;&AAA;/span&ZZZ;").Replace(">", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&gt;&AAA;/span&ZZZ;").Replace("\r", "<br />").Replace("&AAA;", "<").Replace("&ZZZ;", ">").Replace("&QQQ;", "\""));
                 //   Output.WriteLine("</td></tr></table>");
                    break;

                default:
					Output.WriteLine("<div class=\"sbkEimv_Citation\">");
                    Citation_ItemViewer citationViewer = new Citation_ItemViewer(Translator, codeManager, false)
                                                             {CurrentItem = item, CurrentMode = currentMode};
                    Output.WriteLine(citationViewer.Standard_Citation_String(false, Tracer));
                    break;

            }
            Output.WriteLine("</div><br />");
        }

        #endregion

		/// <summary> Property indicates the standard navigation to be included at the top of the page by the
		/// main MySobek html subwriter. </summary>
		/// <value> This viewer always returns NONE </value>
		public override MySobek_Included_Navigation_Enum Standard_Navigation_Type
		{
			get
			{
				return MySobek_Included_Navigation_Enum.NONE;
			}
		}
    }
}
  



