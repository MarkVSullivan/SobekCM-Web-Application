using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.Users;
using SobekCM.Library.Application_State;
using SobekCM.Library.HTML;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Skins;
using SobekCM.Resource_Object;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

namespace SobekCM.Library.MySobekViewer
{
    public class Edit_Item_Permissions_MySobekViewer : abstract_MySobekViewer
    {
        private readonly SobekCM_Item currentItem;
        private User_Object currentUser;
        private List<User_Group> userGroups;
        private IP_Restriction_Ranges ipRestrictions;

        public Edit_Item_Permissions_MySobekViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, SobekCM_Item CurrentCurrentItem, SobekCM_Skin_Object HTML_Skin, Language_Support_Info Translator, List<User_Group> userGroups, IP_Restriction_Ranges ipRestrictions, Custom_Tracer tracer )
            : base(User)
        {
            this.currentUser = User;
            this.userGroups = userGroups;
            this.ipRestrictions = ipRestrictions;
            this.currentItem = CurrentCurrentItem;
            this.currentMode = Current_Mode;


            if ((!currentUser.Is_Internal_User) && (!currentUser.Is_System_Admin))
            {
                currentMode.Mode = Display_Mode_Enum.Item_Display;
                currentMode.Redirect();
            }
        }

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

        public override string Web_Title
        {
            get { return "Edit Item Permissions"; }
        }


        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the template html is added in the <see cref="Write_ItemNavForm_Closing" /> method </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Write_HTML", "Do nothing");
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            const string VISIBILITY = "VISIBILITY";

            currentMode.Mode = Display_Mode_Enum.Item_Display;
            string item_url = currentMode.Redirect_URL();
            currentMode.Mode = Display_Mode_Enum.My_Sobek;

            Tracer.Add_Trace("Edit_Item_Permissions_MySobekViewer.Write_ItemNavForm_Closing", "");

            Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
            Output.WriteLine("<input type=\"hidden\" id=\"behaviors_request\" name=\"behaviors_request\" value=\"\" />");

            // Write the top currentItem mimic html portion
            Write_Item_Type_Top(Output, currentItem);

            Output.WriteLine("<div id=\"container-inner1000\">");
            Output.WriteLine("<div id=\"pagecontainer\">");

            Output.WriteLine("<!-- Edit_Item_Permissions_MySobekViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <h2>Edit currentItem-level permissions for this currentItem</h2>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li>Use this form to change visibility (and related embargo dates) on this currentItem </li>");
            Output.WriteLine("    <li>This form also allows ip restriction and user group permissions to be set </li>");
            Output.WriteLine("    <li>Click <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "help/itempermissions\" target=\"_EDIT_INSTRUCTIONS\">here for detailed instructions</a> on editing permissions online.</li>");
            Output.WriteLine("  </ul>");
            Output.WriteLine("</div>");
            Output.WriteLine();

            Output.WriteLine("<a name=\"template\"> </a>");
            Output.WriteLine("<div id=\"tabContainer\" class=\"fulltabs\">");
            Output.WriteLine("  <div class=\"tabs\">");
            Output.WriteLine("    <ul>");
            Output.WriteLine("      <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + VISIBILITY + "</li>");
            Output.WriteLine("    </ul>");
            Output.WriteLine("  </div>");
            Output.WriteLine("  <div class=\"graytabscontent\">");
            Output.WriteLine("    <div class=\"tabpage\" id=\"tabpage_1\">");

            Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to top of form -->");
            Output.WriteLine("      <script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
            Output.WriteLine();
            Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
            Output.WriteLine("        <button onclick=\"window.location.href='" + item_url + "';return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
            Output.WriteLine("        <button onclick=\"behaviors_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
            Output.WriteLine("      </div>");
            Output.WriteLine("      <br /><br />");
            Output.WriteLine();

            Output.WriteLine("        <table id=\"access_restrictions_div\" >");
            Output.WriteLine("          <tr style=\"text-align:left;\">");
            Output.WriteLine("            <td style=\"vertical-align:top\" class=\"intheader_label\">SET ACCESS RESTRICTIONS: </td>");
            Output.WriteLine("            <td>");
            Output.WriteLine("              <button title=\"Make currentItem public\" class=\"sbkIsw_intheader_button public_resource_button\" onclick=\"set_item_access('public'); return false;\"></button>");
            Output.WriteLine("              <button title=\"Add IP restriction to this currentItem\" class=\"sbkIsw_intheader_button restricted_resource_button\" onclick=\"set_item_access('restricted'); return false;\"></button>");
            Output.WriteLine("              <button title=\"Make currentItem private\" class=\"sbkIsw_intheader_button private_resource_button\" onclick=\"set_item_access('private'); return false;\"></button>");

            // Should we add ability to delete this currentItem?
            if (currentUser.Can_Delete_This_Item(currentItem.BibID, currentItem.Bib_Info.Source.Code, currentItem.Bib_Info.HoldingCode, currentItem.Behaviors.Aggregation_Code_List))
            {
                // Determine the delete URL
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Delete_Item;
                string delete_url = currentMode.Redirect_URL();
                currentMode.Mode = Display_Mode_Enum.Item_Display;
                Output.WriteLine("              <button title=\"Delete this currentItem\" class=\"sbkIsw_intheader_button delete_button\" onclick=\"if(confirm('Delete this currentItem completely?')) window.location.href = '" + delete_url + "'; return false;\"></button>");
            }

            Output.WriteLine("            </td>");
            Output.WriteLine("          </tr>");
            Output.WriteLine("        </table>");

            // Add the second buttons at the bottom of the form
            Output.WriteLine();
            Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to bottom of form -->");
            Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
            Output.WriteLine("        <button onclick=\"window.location.href='" + item_url + "';return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
            Output.WriteLine("        <button onclick=\"behaviors_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
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
        /// <value> This tells the HTML and mySobek writers to mimic the currentItem viewer </value>
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
