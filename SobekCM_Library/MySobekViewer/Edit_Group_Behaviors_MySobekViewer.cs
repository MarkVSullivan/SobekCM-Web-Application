#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Citation.Template;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Resource_Object.Database;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.MySobekViewer
{
   /// <summary> Class allows an authenticated RequestSpecificValues.Current_User to edit the group-level behaviors for a title within this digital library </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the group's behaviors for editing</li>
    /// <li>This viewer uses the <see cref="SobekCM.Library.Citation.Template.Template"/> class to display the correct elements for editing </li>
    /// </ul></remarks>
    public class Edit_Group_Behaviors_MySobekViewer : abstract_MySobekViewer
    {
       private readonly Template template;

       #region Constructor

       /// <summary> Constructor for a new instance of the Edit_Group_Behaviors_MySobekViewer class </summary>
       /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
       public Edit_Group_Behaviors_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
       {
           RequestSpecificValues.Tracer.Add_Trace("Edit_Group_Behaviors_MySobekViewer.Constructor", String.Empty);

           // If the RequestSpecificValues.Current_User cannot edit this RequestSpecificValues.Current_Item, go back
           if (!RequestSpecificValues.Current_User.Can_Edit_This_Item(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.Bib_Info.SobekCM_Type_String, RequestSpecificValues.Current_Item.Bib_Info.Source.Code, RequestSpecificValues.Current_Item.Bib_Info.HoldingCode, RequestSpecificValues.Current_Item.Behaviors.Aggregation_Code_List))
           {
               RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
               UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
               return;
           }

           const string TEMPLATE_CODE = "groupbehaviors";
           template = Cached_Data_Manager.Retrieve_Template(TEMPLATE_CODE, RequestSpecificValues.Tracer);
           if (template != null)
           {
               RequestSpecificValues.Tracer.Add_Trace("Edit_Group_Behaviors_MySobekViewer.Constructor", "Found template in cache");
           }
           else
           {
               RequestSpecificValues.Tracer.Add_Trace("Edit_Group_Behaviors_MySobekViewer.Constructor", "Reading template file");

               // Read this template
               Template_XML_Reader reader = new Template_XML_Reader();
               template = new Template();
               reader.Read_XML(UI_ApplicationCache_Gateway.Settings.Base_MySobek_Directory + "templates\\defaults\\" + TEMPLATE_CODE + ".xml", template, true);

               // Add the current codes to this template
               template.Add_Codes(UI_ApplicationCache_Gateway.Aggregations);

               // Save this into the cache
               Cached_Data_Manager.Store_Template(TEMPLATE_CODE, template, RequestSpecificValues.Tracer);
           }

           // See if there was a hidden request
           string hidden_request = HttpContext.Current.Request.Form["behaviors_request"] ?? String.Empty;

           // If this was a cancel request do that
           if (hidden_request == "cancel")
           {
               RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
               UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
           }
           else if (hidden_request == "save")
           {
               // Save these changes to bib
               template.Save_To_Bib(RequestSpecificValues.Current_Item, RequestSpecificValues.Current_User, 1);

               // Save the group title
               SobekCM_Database.Update_Item_Group(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.Behaviors.GroupTitle, RequestSpecificValues.Current_Item.Bib_Info.SortSafeTitle(RequestSpecificValues.Current_Item.Behaviors.GroupTitle, true), String.Empty, RequestSpecificValues.Current_Item.Behaviors.Primary_Identifier.Type, RequestSpecificValues.Current_Item.Behaviors.Primary_Identifier.Identifier );

               // Save the interfaces to the group RequestSpecificValues.Current_Item as well
               SobekCM_Database.Save_Item_Group_Web_Skins(RequestSpecificValues.Current_Item.Web.GroupID, RequestSpecificValues.Current_Item );

               // Store on the caches (to replace the other)
               Cached_Data_Manager.Remove_Digital_Resource_Objects(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Tracer);

               // Forward
               RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
               UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
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
        /// <value> This returns the value 'Edit Group Behaviors' </value>
        public override string Web_Title
        {
            get
            {
                return "Edit Group Behaviors";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Group_Behaviors_MySobekViewer.Write_HTML", "Do nothing");
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the individual metadata elements are added as controls, not HTML </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
	        const string BEHAVIORS = "BEHAVIORS";

            Tracer.Add_Trace("Edit_Group_Behaviors_MySobekViewer.Write_ItemNavForm_Closing", "");

            Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
            Output.WriteLine("<input type=\"hidden\" id=\"behaviors_request\" name=\"behaviors_request\" value=\"\" />");

			Output.WriteLine("<div id=\"sbkIsw_Titlebar\">");

			string grouptitle = RequestSpecificValues.Current_Item.Behaviors.GroupTitle;
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

            Output.WriteLine("<!-- Edit_Group_Behaviors_MySobekViewer.Write_ItemNavForm_Closing -->");
			Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
            Output.WriteLine("  <br />");

            Output.WriteLine("  <h2>Edit the behaviors associated with this RequestSpecificValues.Current_Item group within this library</h2>");
            Output.WriteLine("    <ul>");
            Output.WriteLine("      <li>Enter the data for this RequestSpecificValues.Current_Item group below and press the SAVE button when all your edits are complete.</li>");
            Output.WriteLine("      <li>Clicking on the green plus button ( <img class=\"repeat_button\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/new_element_demo.jpg\" /> ) will add another instance of the element, if the element is repeatable.</li>");
            Output.WriteLine("      <li>Click <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "help/groupbehaviors\" target=\"_EDIT_INSTRUCTIONS\">here for detailed instructions</a> on editing behaviors online.</li>");


            Output.WriteLine("     </ul>");
            Output.WriteLine("</div>");
            Output.WriteLine();

            Output.WriteLine("<a name=\"template\"> </a>");
			Output.WriteLine("<div id=\"tabContainer\" class=\"fulltabs\">");
			Output.WriteLine("  <div class=\"tabs\">");
			Output.WriteLine("    <ul>");
			Output.WriteLine("      <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + BEHAVIORS + "</li>");
			Output.WriteLine("    </ul>");
			Output.WriteLine("  </div>");
			Output.WriteLine("  <div class=\"graytabscontent\">");
			Output.WriteLine("    <div class=\"tabpage\" id=\"tabpage_1\">");

            Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to top of form -->");
            Output.WriteLine("      <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
			Output.WriteLine();

			Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
			Output.WriteLine("        <button onclick=\"behaviors_cancel_form(); return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button onclick=\"behaviors_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
            Output.WriteLine("      </div>");
			Output.WriteLine("      <br /><br />");
			Output.WriteLine();

	        bool isMozilla = RequestSpecificValues.Current_Mode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0;

	        template.Render_Template_HTML(Output, RequestSpecificValues.Current_Item, RequestSpecificValues.Current_Mode.Skin == RequestSpecificValues.Current_Mode.Default_Skin ? RequestSpecificValues.Current_Mode.Skin.ToUpper() : RequestSpecificValues.Current_Mode.Skin, isMozilla, RequestSpecificValues.Current_User, RequestSpecificValues.Current_Mode.Language, UI_ApplicationCache_Gateway.Translation, RequestSpecificValues.Current_Mode.Base_URL, 1);

            // Add the second buttons at the bottom of the form
			Output.WriteLine();
            Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to bottom of form -->");
			Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
			Output.WriteLine("        <button onclick=\"behaviors_cancel_form(); return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button onclick=\"behaviors_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </div>");
			Output.WriteLine("      <br />");
			Output.WriteLine("    </div>");
			Output.WriteLine("  </div>");
			Output.WriteLine("</div>");
			Output.WriteLine("</div>");
			Output.WriteLine("</div>");
        }

        /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds any popup divisions for form metadata elements </remarks>
        public override void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Group_Behaviors_MySobekViewer.Add_Popup_HTML", "Add any popup divisions for form elements");

            // Add the hidden field
            Output.WriteLine();
        }

		/// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
		/// requests from the main HTML subwriter. </summary>
		/// <value> This tells the HTML and mySobek writers to mimic the RequestSpecificValues.Current_Item viewer </value>
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




