#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.Client;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Engine_Library.Items;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.Citation;
using SobekCM.Library.Citation.Template;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated RequestSpecificValues.Current_User to edit a digital resource online, using various possible templates </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the item for editing</li>
    /// <li>This viewer uses the <see cref="CompleteTemplate"/> class to display the correct elements for editing </li>
    /// </ul></remarks>
    public class Edit_Item_Metadata_MySobekViewer : abstract_MySobekViewer
    {
        private readonly bool isProject;
        private readonly double page;
        private string popUpFormsHtml;
        private readonly CompleteTemplate completeTemplate;
	    private readonly string delayed_popup;

        private readonly SobekCM_Item currentItem;

        #region Constructor

        /// <summary> Constructor for a new instance of the Edit_Item_Metadata_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Edit_Item_Metadata_MySobekViewer( RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", String.Empty);

            popUpFormsHtml = String.Empty;
	        delayed_popup = String.Empty;

            // If no user then that is an error
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation = String.Empty;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Ensure BibID and VID provided
            RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Validate provided bibid / vid");
            if ((String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.BibID)) || (String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.VID)))
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "BibID or VID was not provided!");
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : BibID/VID missing in item metadata edit request";
                return;
            }

            // Ensure the item is valid
            RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Validate bibid/vid exists");
            if (!UI_ApplicationCache_Gateway.Items.Contains_BibID_VID(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID))
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "BibID/VID indicated is not valid", Custom_Trace_Type_Enum.Error);
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : BibID/VID indicated is not valid";
                return;
            }

            RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Try to pull this sobek complete item");
            currentItem = SobekEngineClient.Items.Get_Sobek_Item(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID, RequestSpecificValues.Current_User.UserID, RequestSpecificValues.Tracer);
            if (currentItem == null)
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Unable to build complete item");
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : Unable to build complete item";
                return;
            }

            // If the RequestSpecificValues.Current_User cannot edit this item, go back
            if (!RequestSpecificValues.Current_User.Can_Edit_This_Item(currentItem.BibID, currentItem.Bib_Info.SobekCM_Type_String, currentItem.Bib_Info.Source.Code, currentItem.Bib_Info.HoldingCode, currentItem.Behaviors.Aggregation_Code_List))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                HttpContext.Current.Response.Redirect(UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode));
            }

            // Is this a project
            isProject = currentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project;

	        string template_code = RequestSpecificValues.Current_User.Edit_Template_Code_Simple;

            if ((currentItem.Contains_Complex_Content) || (currentItem.Using_Complex_Template))
            {
                template_code = RequestSpecificValues.Current_User.Edit_Template_Code_Complex;
            }
            if (isProject)
            {
                template_code = "standard_project";
                completeTemplate = Template_MemoryMgmt_Utility.Retrieve_Template(template_code, RequestSpecificValues.Tracer);
                if (completeTemplate != null)
                {
                    RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Found project-specific template in cache");
                }
                else
                {
                    RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Reading project-specific template file");

                    // Look in the user-defined portion
                    string user_template = UI_ApplicationCache_Gateway.Settings.Servers.Base_MySobek_Directory + "templates\\user\\standard\\project.xml";
                    if (!File.Exists(user_template))
                        user_template = UI_ApplicationCache_Gateway.Settings.Servers.Base_MySobek_Directory + "templates\\default\\standard\\project.xml";

                    // Read this CompleteTemplate
                    Template_XML_Reader reader = new Template_XML_Reader();
                    completeTemplate = new CompleteTemplate();
                    reader.Read_XML(user_template, completeTemplate, true);

                    // Add the current codes to this template
                    completeTemplate.Add_Codes(UI_ApplicationCache_Gateway.Aggregations);

                    // Save this into the cache
                    Template_MemoryMgmt_Utility.Store_Template(template_code, completeTemplate, RequestSpecificValues.Tracer);
                }
            }
            else
            {
                completeTemplate = Template_MemoryMgmt_Utility.Retrieve_Template(template_code, RequestSpecificValues.Tracer);
                if (completeTemplate != null)
                {
                    RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Found template in cache");
                }
                else
                {
                    RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Reading template file");

                    // Look in the user-defined portion
                    string user_template = UI_ApplicationCache_Gateway.Settings.Servers.Base_MySobek_Directory + "templates\\user\\edit\\" + template_code + ".xml";
                    if (!File.Exists(user_template))
                        user_template = UI_ApplicationCache_Gateway.Settings.Servers.Base_MySobek_Directory + "templates\\default\\edit\\" + template_code + ".xml";

                    // Read this CompleteTemplate
                    Template_XML_Reader reader = new Template_XML_Reader();
                    completeTemplate = new CompleteTemplate();
                    reader.Read_XML(user_template, completeTemplate, true);

                    // Add the current codes to this template
                    completeTemplate.Add_Codes(UI_ApplicationCache_Gateway.Aggregations);

                    // Save this into the cache
                    Template_MemoryMgmt_Utility.Store_Template(template_code, completeTemplate, RequestSpecificValues.Tracer);
                }
            }

            // Get the current page number, or default to 1
            page = 1;
            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.My_Sobek_SubMode))
            {
                if ((RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "preview") || (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "marc") || (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "mets"))
                {
                    page = 0;
                }
                else
                {
                    page = 1;
                    bool isNumber = RequestSpecificValues.Current_Mode.My_Sobek_SubMode.All(Char.IsNumber);
                    if (isNumber)
                    {
                        if (isProject)
                            Double.TryParse(RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0].ToString(), out page);
                        else
                            Double.TryParse(RequestSpecificValues.Current_Mode.My_Sobek_SubMode, out page);
                    }
                    else if ( isProject )
                    { 
                        if ( Char.IsNumber(RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0]))
                            Double.TryParse(RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0].ToString(), out page);
                    }
                }
            }

			// Handle post backs
	        if (RequestSpecificValues.Current_Mode.isPostBack)
	        {
		        // See if there was a hidden request
		        string hidden_request = HttpContext.Current.Request.Form["new_element_requested"] ?? String.Empty;

		        // If this was a cancel request do that
		        if (hidden_request == "cancel")
		        {
			        if (isProject)
			        {
				        CachedDataManager.Remove_Project(RequestSpecificValues.Current_User.UserID, currentItem.BibID, null);

				        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
				        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
				        RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
				        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
			        }
			        else
			        {
				        CachedDataManager.Items.Remove_Digital_Resource_Object(RequestSpecificValues.Current_User.UserID, currentItem.BibID, currentItem.VID, null);

				        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
				        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
			        }
			        return;
		        }


		        // Save these changes to bib
		        completeTemplate.Save_To_Bib(currentItem, RequestSpecificValues.Current_User, ((int) page));

		        // See if the RequestSpecificValues.Current_User asked for a new element of a complex form type
		        delayed_popup = String.Empty;
		        switch (hidden_request.Trim())
		        {
			        case "name":
				        delayed_popup = "name";
				        currentItem.Bib_Info.Add_Named_Entity(String.Empty).Name_Type = Name_Info_Type_Enum.Personal;
				        break;

			        case "title":
				        delayed_popup = "title";
				        currentItem.Bib_Info.Add_Other_Title(String.Empty, Title_Type_Enum.Alternative);
				        break;

			        case "subject":
				        delayed_popup = "subject";
				        currentItem.Bib_Info.Add_Subject();
				        break;

			        case "spatial":
				        delayed_popup = "spatial";
				        currentItem.Bib_Info.Add_Hierarchical_Geographic_Subject();
				        break;

			        case "relateditem":
				        delayed_popup = "relateditem";
				        currentItem.Bib_Info.Add_Related_Item(new Related_Item_Info());
				        break;

			        case "save":
				        Complete_Item_Save();
				        break;

					case "complicate":
						currentItem.Using_Complex_Template = true;
				        HttpContext.Current.Response.Redirect( "?" + HttpContext.Current.Request.QueryString, false);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						RequestSpecificValues.Current_Mode.Request_Completed = true;
				        return;

					case "simplify":
						currentItem.Using_Complex_Template = false;
				        HttpContext.Current.Response.Redirect( "?" + HttpContext.Current.Request.QueryString, false);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						RequestSpecificValues.Current_Mode.Request_Completed = true;
				        return;
		        }

				// Was this for a new page?
				if (hidden_request.IndexOf("newpage") == 0)
				{
					string page_requested = hidden_request.Replace("newpage", "");
					if (page_requested != RequestSpecificValues.Current_Mode.My_Sobek_SubMode)
					{
						// forward to requested page
						RequestSpecificValues.Current_Mode.My_Sobek_SubMode = page_requested;
						if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "0")
							RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "preview";
						if (isProject)
							RequestSpecificValues.Current_Mode.My_Sobek_SubMode = page_requested + currentItem.BibID;

						HttpContext.Current.Response.Redirect(UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "#CompleteTemplate", false);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						RequestSpecificValues.Current_Mode.Request_Completed = true;
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

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of any form)  </summary>
        /// <param name="Output">Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Does nothing </remarks>
		public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			// DO nothing
		}

        /// <summary> This is an opportunity to write HTML directly into the main form before any controls are 
        /// placed in the main place holder </summary>
        /// <param name="Output">Textwriter to write the pop-up form HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags  </remarks>
	    public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
	    {
		    Output.WriteLine("<!-- Edit_Item_Metadata_MySobekViewer.Add_Controls -->");

			// Write the top item mimic html portion
		    Write_Item_Type_Top(Output, currentItem);

		    Output.WriteLine("<div id=\"container-inner1000\">");
			Output.WriteLine("<div id=\"pagecontainer\">");

			Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
			Output.WriteLine("  <br />");
			if (!isProject)
			{
				Output.WriteLine("  <h2>Edit this item</h2>");
				Output.WriteLine("    <ul>");
				Output.WriteLine("      <li>Enter the data for this item below and press the SAVE button when all your edits are complete.</li>");
                Output.WriteLine("      <li>Clicking on the green plus button ( <img class=\"repeat_button\" src=\"" + Static_Resources.New_Element_Demo_Jpg + "\" /> ) will add another instance of the element, if the element is repeatable.</li>");

                // This whole section only applies if the simple and complex templates are different
			    if (String.Compare(RequestSpecificValues.Current_User.Edit_Template_Code_Complex, RequestSpecificValues.Current_User.Edit_Template_Code_Simple, StringComparison.OrdinalIgnoreCase) != 0)
			    {
			        if ((currentItem.Using_Complex_Template) || (currentItem.Contains_Complex_Content))
			        {
			            if (currentItem.Contains_Complex_Content)
			            {
			                Output.WriteLine("      <li>You are using the full editing form because this item contains complex elements or was derived from MARC.</li>");
			            }
			            else
			            {
			                Output.Write("      <li>You are using the full editing form.  Click");
			                Output.Write("<a href=\"#\" onclick=\"editmetadata_simplify();return false;\"> here to return to the simplified version</a>.");
			                Output.WriteLine("</li>");
			            }
			        }
			        else
			        {
			            Output.WriteLine("      <li>You are using the simplified editing form.  Click");
			            Output.Write("<a href=\"#\" onclick=\"editmetadata_complicate();return false;\"> here to use the full form</a>.");
			            Output.WriteLine("</li>");
			        }

			        if (completeTemplate.Code.ToUpper().IndexOf("MARC") > 0)
			        {
			            Output.WriteLine("      <li>To open detailed edit forms, click on the linked metadata values.</li>");
			        }
			    }


			    Output.WriteLine("      <li>Click <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "help/editinstructions\" target=\"_EDIT_INSTRUCTIONS\">here for detailed instructions</a> on editing metadata online.</li>");
			}
			else
			{
				Output.WriteLine("  <b>Edit this project</b>");
				Output.WriteLine("    <ul>");
				Output.WriteLine("      <li>Enter the default data for this project below and press the SAVE button when all your edits are complete.</li>");
				Output.WriteLine("      <li>Clicking on the blue plus signs ( <img class=\"repeat_button\" src=\"" + Static_Resources.New_Element_Demo_Jpg + "\" /> ) will add another instance of the element, if the element is repeatable.</li>");
				Output.WriteLine("      <li>Click on the element names for detailed information inluding definitions, best practices, and technical information.</li>");
			}

			Output.WriteLine("     </ul>");
			Output.WriteLine("</div>");
			Output.WriteLine();

			Output.WriteLine("<!-- Buttons to select each CompleteTemplate page -->");
			Output.WriteLine("<a name=\"CompleteTemplate\"> </a>");
			Output.WriteLine("<div id=\"tabContainer\" class=\"fulltabs\">");
			Output.WriteLine("  <div class=\"tabs\">");
			Output.WriteLine("    <ul>");


			int page_iterator = 1;
	        string current_submode = String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.My_Sobek_SubMode) ? String.Empty : RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
			if (current_submode.Length == 0)
				current_submode = "1";
			while (page_iterator <= completeTemplate.InputPages.Count)
			{
				if (current_submode[0].ToString() == page_iterator.ToString())
					Output.Write("      <li class=\"tabActiveHeader\">" + completeTemplate.InputPages[page_iterator - 1].Title + "</li>");
				else
					Output.Write("      <li onclick=\"editmetadata_newpage('" + page_iterator + "');\">" + completeTemplate.InputPages[page_iterator - 1].Title + "</li>");

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

			RequestSpecificValues.Current_Mode.My_Sobek_SubMode = current_submode;

			Output.WriteLine("    </ul>");
			Output.WriteLine("  </div>");
			Output.WriteLine("  <div class=\"graytabscontent\">");
			Output.WriteLine("    <div class=\"tabpage\" id=\"tabpage_1\">");

			// Add the first buttons
			Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to top of form -->");
			Output.WriteLine("      <script src=\"" + Static_Resources.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
			Output.WriteLine();
			Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
			Output.WriteLine("        <button onclick=\"editmetadata_cancel_form();return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button onclick=\"editmetadata_save_form();return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </div>");
			Output.WriteLine("      <br /><br />");
			Output.WriteLine();


			Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Add_Controls", "Render CompleteTemplate html");
			if (!preview)
			{
				if (page >= 1)
				{
				    bool isMozilla = ((!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Browser_Type)) && (RequestSpecificValues.Current_Mode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0));

					popUpFormsHtml = completeTemplate.Render_Template_HTML(Output, currentItem, RequestSpecificValues.Current_Mode.Skin == RequestSpecificValues.Current_Mode.Default_Skin ? RequestSpecificValues.Current_Mode.Skin.ToUpper() : RequestSpecificValues.Current_Mode.Skin, isMozilla, RequestSpecificValues.Current_User, RequestSpecificValues.Current_Mode.Language, UI_ApplicationCache_Gateway.Translation, RequestSpecificValues.Current_Mode.Base_URL, ((int)page));
				}
			}
			else
			{
				show_preview(Output, RequestSpecificValues.Current_Mode.My_Sobek_SubMode, Tracer);
			}

			// Add the second buttons at the bottom of the form
			Output.WriteLine();
			Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to bottom of form -->");
			Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
			Output.WriteLine("        <button onclick=\"editmetadata_cancel_form(); return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button onclick=\"editmetadata_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </div>");
			Output.WriteLine("      <br />");
			Output.WriteLine("    </div>");
			Output.WriteLine("  </div>");
			Output.WriteLine("</div>");
			Output.WriteLine("</div>");
			Output.WriteLine("</div>");
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
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Ui_1_10_3_Custom_Js + "\"></script>");

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
						int related_item_count = currentItem.Bib_Info.RelatedItems_Count;
						if (related_item_count > 0)
						{
							Output.WriteLine("<!-- Popup the delayed box -->");
							Output.WriteLine("<script type=\"text/javascript\">popup_focus('form_related_item_" + related_item_count + "', 'form_related_item_term_" + related_item_count + "', 'form_relateditem_title_" + related_item_count + "', 575, 620 );</script>");
							Output.WriteLine();
						}
						break;

					case "subject":
						int subject_index = 0;
						if (currentItem.Bib_Info.Subjects_Count > 0)
						{
							subject_index += currentItem.Bib_Info.Subjects.Count(ThisSubject => ThisSubject.Class_Type == Subject_Info_Type.Standard);
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
						if (currentItem.Bib_Info.Subjects_Count > 0)
						{
							spatial_index += currentItem.Bib_Info.Subjects.Count(ThisSubject => ThisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial);
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
						if ((currentItem.Bib_Info.hasMainEntityName) && (currentItem.Bib_Info.Main_Entity_Name.hasData))
							name_count++;
						name_count += currentItem.Bib_Info.Names_Count;

						if (name_count > 0)
						{
							Output.WriteLine("<!-- Popup the delayed box -->");
							Output.WriteLine("<script type=\"text/javascript\">popup_focus('form_name_" + name_count + "', 'form_name_line_" + name_count + "', 'form_name_full_" + name_count + "', 475, 700 );</script>");
							Output.WriteLine();
						}
						break;

					case "title":
						int title_count = currentItem.Bib_Info.Other_Titles_Count;
						if ((currentItem.Bib_Info.hasSeriesTitle) && (currentItem.Bib_Info.SeriesTitle.Title.Length > 0))
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


        #region Method to complete a save to SobekCM

        private void Complete_Item_Save()
        {
            if (isProject)
            {
                // Save the new project METS
                currentItem.Save_METS();

                // Clear the cache of this item
                CachedDataManager.Remove_Project(RequestSpecificValues.Current_User.UserID, currentItem.BibID, null);

                // Redirect
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
            }
            else
            {
                string error_message;
                SobekCM_Item_Updater.Update_Item(currentItem, RequestSpecificValues.Current_User, out error_message);

                // Forward to the display item again
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                RequestSpecificValues.Current_Mode.ViewerCode = "citation";
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
            }
        }

        #endregion

        #region Method to add the preview html

        private void show_preview(TextWriter Output, string Preview_Mode, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.show_preview", String.Empty);

            Output.WriteLine("<script language=\"JavaScript\">");

            // Get the URL to use for forwarding
            string current_submode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "ZZZZZ";
            string redirect_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = current_submode;

            Output.WriteLine("  function preview1() {if (document.itemNavForm.pickme) location='" + redirect_url.Replace("ZZZZZ", "preview") + "#CompleteTemplate'}");
            Output.WriteLine("  function preview2() {if (document.itemNavForm.pickme) location='" + redirect_url.Replace("ZZZZZ", "marc") + "#CompleteTemplate'}");
            Output.WriteLine("  function preview3() {if (document.itemNavForm.pickme) location='" + redirect_url.Replace("ZZZZZ", "mets") + "#CompleteTemplate'}");
            Output.WriteLine("</script>");

            Output.WriteLine("<center>");
            Output.WriteLine(Preview_Mode == "preview"
								 ? "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfPreview\" checked=\"checked\" /><label for=\"sbkTypeOfPreview\">Standard View</label> &nbsp; &nbsp; "
								 : "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfPreview\" onchange=\"location='" + redirect_url.Replace("ZZZZZ", "preview") + "#CompleteTemplate';\" /><label for=\"sbkTypeOfPreview\">Standard View</label> &nbsp; &nbsp; ");

            Output.WriteLine(Preview_Mode == "marc"
								 ? "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfMarc\" checked=\"checked\" /><label for=\"sbkTypeOfMarc\">MARC View</label> &nbsp; &nbsp; "
								 : "<input TYPE=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfMarc\" onchange=\"location='" + redirect_url.Replace("ZZZZZ", "marc") + "#CompleteTemplate';\" /><label for=\"sbkTypeOfMarc\">MARC View</label> &nbsp; &nbsp; ");

            Output.WriteLine(Preview_Mode == "mets"
								 ? "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfMets\" checked=\"checked\" /><label for=\"sbkTypeOfMets\">METS View</label>"
								 : "<input type=\"radio\" name=\"sbkPreviewType\" id=\"sbkTypeOfMets\" onchange=\"location='" + redirect_url.Replace("ZZZZZ", "mets") + "#CompleteTemplate';\" /><label for=\"sbkTypeOfMets\">METS View</label>");

            Output.WriteLine("</center>");
            Output.WriteLine("<br />");

            switch (Preview_Mode)
            {
                case "marc":
					Output.WriteLine("<div class=\"sbkEimv_Citation\">");
                    //Citation_ItemViewer marcViewer = new Citation_ItemViewer(UI_ApplicationCache_Gateway.Translation, UI_ApplicationCache_Gateway.Aggregations, false)
                    //                                     {CurrentItem = currentItem, CurrentMode = RequestSpecificValues.Current_Mode};
                    //Output.WriteLine(marcViewer.MARC_String("735px", Tracer));
                    break;

                case "mets":
					Output.WriteLine("<div class=\"sbkEimv_Citation\" >");
                  //  Output.WriteLine("<table width=\"950px\"><tr><td width=\"950px\">");
                    StringBuilder mets_builder = new StringBuilder(2000);
                    StringWriter mets_output = new StringWriter(mets_builder);

                    METS_File_ReaderWriter metsWriter = new METS_File_ReaderWriter();

                    string errorMessage;
                    metsWriter.Write_Metadata(mets_output, currentItem, null, out errorMessage);
                    string mets_string = mets_builder.ToString();
                    string header = mets_string.Substring(0, mets_string.IndexOf("<METS:mets"));
                    string remainder = mets_string.Substring(header.Length);
                    Output.WriteLine(header.Replace("<?", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt?").Replace("?>", "?&gt;&AAA;/span&ZZZ;").Replace("<!--", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt!--&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Gray;&QQQ;&ZZZ;").Replace("-->", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;--&gt;&AAA;/span&ZZZ;").Replace("\r", "<br />").Replace("&AAA;", "<").Replace("&ZZZ;", ">").Replace("&QQQ;", "\""));
                    Output.WriteLine(remainder.Replace("<?", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt?").Replace("?>", "?&gt;&AAA;/span&ZZZ;").Replace("<!--", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt!--&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Gray;&QQQ;&ZZZ;").Replace("-->", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;--&gt;&AAA;/span&ZZZ;").Replace("</", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt;/&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Maroon;&QQQ;&ZZZ;").Replace("<", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&lt;&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Maroon;&QQQ;&ZZZ;").Replace("=\"", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;=&quot;&AAA;/span&ZZZ;").Replace("\">", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&quot;&gt;&AAA;/span&ZZZ;").Replace("\"", "&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&quot;&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Maroon;&QQQ;&ZZZ;").Replace("/>", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;/&gt;&AAA;/span&ZZZ;").Replace(">", "&AAA;/span&ZZZ;&AAA;span style=&QQQ;color:Blue;&QQQ;&ZZZ;&gt;&AAA;/span&ZZZ;").Replace("\r", "<br />").Replace("&AAA;", "<").Replace("&ZZZ;", ">").Replace("&QQQ;", "\""));
                 //   Output.WriteLine("</td></tr></table>");
                    break;

                default:
					Output.WriteLine("<div class=\"sbkEimv_Citation\">");
                    //Citation_ItemViewer citationViewer = new Citation_ItemViewer(UI_ApplicationCache_Gateway.Translation, UI_ApplicationCache_Gateway.Aggregations, false)
                    //                                         {CurrentItem = currentItem, CurrentMode = RequestSpecificValues.Current_Mode};
                    //Output.WriteLine(citationViewer.Standard_Citation_String(false, Tracer));
                    break;

            }
            Output.WriteLine("</div><br />");
        }

        #endregion


        /// <summary> Navigation type to be displayed (mostly used by the mySobek viewers) </summary>
        /// <value> This returns none since this viewer writes all the necessary navigational elements </value>
        /// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        /// administrative tabs should be included as well.  </remarks>
        public override MySobek_Admin_Included_Navigation_Enum Standard_Navigation_Type { get { return MySobek_Admin_Included_Navigation_Enum.NONE; } }

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

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if this should completely override the default added by the admin or mySobek viewer </returns>
        public override bool Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Metadata_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Mysobek_Css + "\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Item_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
            return true;
        }

        /// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
        public override string Container_CssClass { get { return "container-inner1000"; } }
    }
}
  



