#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.Citation;
using SobekCM.Library.Citation.Template;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UI;
using SobekCM.Resource_Object.Database;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated RequestSpecificValues.Current_User to edit a single digital resource's behaviors within this digital library </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the RequestSpecificValues.Current_Item's behaviors for editing</li>
    /// <li>This viewer uses the <see cref="CompleteTemplate"/> class to display the correct elements for editing </li>
    /// </ul></remarks>
    public class Edit_Item_Behaviors_MySobekViewer : abstract_MySobekViewer
    {
        private readonly CompleteTemplate completeTemplate;

        #region Constructor

        /// <summary> Constructor for a new instance of the Edit_Item_Behaviors_MySobekViewer class </summary>
        ///  <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Edit_Item_Behaviors_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Constructor", String.Empty);

            // If the RequestSpecificValues.Current_User cannot edit this RequestSpecificValues.Current_Item, go back
            if (!RequestSpecificValues.Current_User.Can_Edit_This_Item(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.Bib_Info.SobekCM_Type_String, RequestSpecificValues.Current_Item.Bib_Info.Source.Code, RequestSpecificValues.Current_Item.Bib_Info.HoldingCode, RequestSpecificValues.Current_Item.Behaviors.Aggregation_Code_List))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            const string TEMPLATE_CODE = "itembehaviors";
            completeTemplate = Template_MemoryMgmt_Utility.Retrieve_Template(TEMPLATE_CODE, RequestSpecificValues.Tracer);
            if (completeTemplate != null)
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Constructor", "Found CompleteTemplate in cache");
            }
            else
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Constructor", "Reading CompleteTemplate file");

                // Look in the user-defined templates portion first
                string user_template = UI_ApplicationCache_Gateway.Settings.Servers.Base_MySobek_Directory + "templates\\user\\standard\\" + TEMPLATE_CODE + ".xml";
                if (!File.Exists(user_template))
                    user_template = UI_ApplicationCache_Gateway.Settings.Servers.Base_MySobek_Directory + "templates\\default\\standard\\" + TEMPLATE_CODE + ".xml";


                // Read this CompleteTemplate
                Template_XML_Reader reader = new Template_XML_Reader();
                completeTemplate = new CompleteTemplate();
                reader.Read_XML(user_template, completeTemplate, true);

                // Add the current codes to this CompleteTemplate
                completeTemplate.Add_Codes(UI_ApplicationCache_Gateway.Aggregations);

                // Save this into the cache
                Template_MemoryMgmt_Utility.Store_Template(TEMPLATE_CODE, completeTemplate, RequestSpecificValues.Tracer);
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
                // Changes to the tracking box require the metadata search citation be rebuilt for this RequestSpecificValues.Current_Item
                // so save the old tracking box information first
                string oldTrackingBox = RequestSpecificValues.Current_Item.Tracking.Tracking_Box;

                // Save these changes to bib
                completeTemplate.Save_To_Bib(RequestSpecificValues.Current_Item, RequestSpecificValues.Current_User, 1);

                // Save the behaviors
                SobekCM_Database.Save_Behaviors(RequestSpecificValues.Current_Item, RequestSpecificValues.Current_Item.Behaviors.Text_Searchable, false, false );

                // Save the serial hierarchy as well (sort of a behavior)
                SobekCM_Database.Save_Serial_Hierarchy_Information(RequestSpecificValues.Current_Item, RequestSpecificValues.Current_Item.Web.GroupID, RequestSpecificValues.Current_Item.Web.ItemID);

                // Did the tracking box change?
                if (RequestSpecificValues.Current_Item.Tracking.Tracking_Box != oldTrackingBox)
                {
                    SobekCM_Database.Create_Full_Citation_Value(RequestSpecificValues.Current_Item.Web.ItemID);
                }

                // Remoe from the caches (to replace the other)
                CachedDataManager.Remove_Digital_Resource_Object(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.VID, RequestSpecificValues.Tracer);

                // Also remove the list of volumes, since this may have changed
                CachedDataManager.Remove_Items_In_Title(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Tracer);

                // Forward
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
            }
        }

        #endregion


        /// <summary> Navigation type to be displayed (mostly used by the mySobek viewers) </summary>
        /// <value> This returns none since this viewer writes all the necessary navigational elements </value>
        /// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        /// administrative tabs should be included as well.  </remarks>
        public override MySobek_Admin_Included_Navigation_Enum Standard_Navigation_Type { get { return MySobek_Admin_Included_Navigation_Enum.NONE; } }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the value 'Edit Item Behaviors' </value>
        public override string Web_Title
        {
            get
            {
                return "Edit Item Behaviors";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the CompleteTemplate html is added in the <see cref="Write_ItemNavForm_Closing" /> method </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Write_HTML", "Do nothing");
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void  Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
			const string BEHAVIORS = "BEHAVIORS";

            Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Write_ItemNavForm_Closing", "");

            Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
            Output.WriteLine("<input type=\"hidden\" id=\"behaviors_request\" name=\"behaviors_request\" value=\"\" />");

			// Write the top RequestSpecificValues.Current_Item mimic html portion
			Write_Item_Type_Top(Output, RequestSpecificValues.Current_Item);

			Output.WriteLine("<div id=\"container-inner1000\">");
			Output.WriteLine("<div id=\"pagecontainer\">");

            Output.WriteLine("<!-- Edit_Item_Behaviors_MySobekViewer.Write_ItemNavForm_Closing -->");
			Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <h2>Edit this item's behaviors within this library</h2>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li>Enter the data for this item below and press the SAVE button when all your edits are complete.</li>");
            Output.WriteLine("    <li>Clicking on the green plus button ( <img class=\"repeat_button\" src=\"" + Static_Resources.New_Element_Demo_Jpg + "\" /> ) will add another instance of the element, if the element is repeatable.</li>");
            Output.WriteLine("    <li>Click <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "help/behaviors\" target=\"_EDIT_INSTRUCTIONS\">here for detailed instructions</a> on editing behaviors online.</li>");
            Output.WriteLine("  </ul>");
            Output.WriteLine("</div>");
            Output.WriteLine();

			Output.WriteLine("<a name=\"CompleteTemplate\"> </a>");
			Output.WriteLine("<div id=\"tabContainer\" class=\"fulltabs\">");
			Output.WriteLine("  <div class=\"tabs\">");
			Output.WriteLine("    <ul>");
			Output.WriteLine("      <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + BEHAVIORS + "</li>");
			Output.WriteLine("    </ul>");
			Output.WriteLine("  </div>");
			Output.WriteLine("  <div class=\"graytabscontent\">");
			Output.WriteLine("    <div class=\"tabpage\" id=\"tabpage_1\">");

			Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to top of form -->");
			Output.WriteLine("      <script src=\"" + Static_Resources.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
			Output.WriteLine();

			Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
			Output.WriteLine("        <button onclick=\"behaviors_cancel_form(); return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button onclick=\"behaviors_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </div>");
			Output.WriteLine("      <br /><br />");
			Output.WriteLine();

            bool isMozilla = ((!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Browser_Type)) && (RequestSpecificValues.Current_Mode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0));
	        completeTemplate.Render_Template_HTML(Output, RequestSpecificValues.Current_Item, RequestSpecificValues.Current_Mode.Skin == RequestSpecificValues.Current_Mode.Default_Skin ? RequestSpecificValues.Current_Mode.Skin.ToUpper() : RequestSpecificValues.Current_Mode.Skin, isMozilla, RequestSpecificValues.Current_User, RequestSpecificValues.Current_Mode.Language, UI_ApplicationCache_Gateway.Translation, RequestSpecificValues.Current_Mode.Base_URL, 1);

			// Add the second buttons at the bottom of the form
			Output.WriteLine();
			Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to bottom of form -->");
			Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
			Output.WriteLine("        <button onclick=\"behaviors_cancel_form(); return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button onclick=\"behaviors_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </div>");
			Output.WriteLine("      <br />");
			Output.WriteLine("    </div>");
			Output.WriteLine("  </div>");
			Output.WriteLine("</div>");
			Output.WriteLine("</div>");
			Output.WriteLine("</div>");
			Output.WriteLine("<br />");
        }

        /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds any popup divisions for form metadata elements </remarks>
        public override void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Add_Popup_HTML", "Add any popup divisions for form elements");

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

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if this should completely override the default added by the admin or mySobek viewer </returns>
        public override bool Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Metadata_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
            return false;
        }

        /// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
        public override string Container_CssClass { get { return "container-inner1000"; } }
    }
}




