using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Core.Settings;
using SobekCM.Core.Users;
using SobekCM.Library.Application_State;
using SobekCM.Library.HTML;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Skins;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Resource_Object;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;
using SobekCM.Resource_Object.Metadata_Modules;

namespace SobekCM.Library.MySobekViewer
{
    public class Edit_Item_Permissions_MySobekViewer : abstract_MySobekViewer
    {
        private readonly SobekCM_Item currentItem;
        private User_Object currentUser;
        private List<User_Group> userGroups;
        private IP_Restriction_Ranges ipRestrictions;

        private short ipRestrictionMask;
        private bool isDark;
        private DateTime? embargoDate;


        public Edit_Item_Permissions_MySobekViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, SobekCM_Item CurrentCurrentItem, SobekCM_Skin_Object HTML_Skin, Language_Support_Info Translator, List<User_Group> userGroups, IP_Restriction_Ranges ipRestrictions, Custom_Tracer Tracer )
            : base(User)
        {
            this.currentUser = User;
            this.userGroups = userGroups;
            this.ipRestrictions = ipRestrictions;
            this.currentItem = CurrentCurrentItem;
            this.currentMode = Current_Mode;


            if ( currentUser == null ) 
            {
                currentMode.Mode = Display_Mode_Enum.Item_Display;
                currentMode.Redirect();
            }

            bool userCanEditItem = currentUser.Can_Edit_This_Item(currentItem.BibID, currentItem.Bib_Info.SobekCM_Type_String, currentItem.Bib_Info.Source.Code, currentItem.Bib_Info.HoldingCode, currentItem.Behaviors.Aggregation_Code_List);

            if (!userCanEditItem)
            {
                currentMode.Mode = Display_Mode_Enum.Item_Display;
                currentMode.Redirect();
            }

            // Start by setting the values by the item (good the first time user comes here)
            ipRestrictionMask = currentItem.Behaviors.IP_Restriction_Membership;
            isDark = currentItem.Behaviors.Dark_Flag;

            // Is there already a RightsMD module in the item?
            // Ensure this metadata module extension exists
            RightsMD_Info rightsInfo = currentItem.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;
            if (( rightsInfo != null) && ( rightsInfo.Has_Embargo_End ))
            {
                embargoDate = rightsInfo.Embargo_End;
            }
            

            // Is this a postback?
            if (currentMode.isPostBack)
            {
                // Get the restriction mask and isDark flag
                if (HttpContext.Current.Request.Form["restrictionMask"] != null)
                {
                    ipRestrictionMask = short.Parse(HttpContext.Current.Request.Form["restrictionMask"].ToString());
                    isDark = bool.Parse(HttpContext.Current.Request.Form["isDark"].ToString());
                }

                // Look for embargo date
                if (HttpContext.Current.Request.Form["embargoDateBox"] != null)
                {
                    string embargoText = HttpContext.Current.Request.Form["embargoDateBox"].ToString();
                    DateTime embargoDateNew;
                    if (DateTime.TryParse(embargoText, out embargoDateNew))
                    {
                        embargoDate = embargoDateNew;
                    }
                }
                

                // Handle any request from the internal header for the item
                if (HttpContext.Current.Request.Form["permissions_action"] != null)
                {
                    // Pull the action value
                    string action = HttpContext.Current.Request.Form["permissions_action"].Trim();

                    // Is this to change accessibility?
                    if ((action == "public") || (action == "private") || (action == "restricted") || ( action == "dark" ))
                    {
                        int current_mask = currentItem.Behaviors.IP_Restriction_Membership;
                        switch (action)
                        {
                            case "public":
                                ipRestrictionMask = 0;
                                isDark = false;
                                break;

                            case "private":
                                ipRestrictionMask = -1;
                                isDark = false;
                                break;

                            case "restricted":
                                ipRestrictionMask = 1;
                                isDark = false;
                                break;

                            case "dark":
                                isDark = true;
                                break;
                        }


                    }
                }

                // Was the SAVE button pushed?
                if (HttpContext.Current.Request.Form["behaviors_request"] != null)
                {
                    string behaviorRequest = HttpContext.Current.Request.Form["behaviors_request"].ToString();
                    if (behaviorRequest == "save")
                    {
                        currentItem.Behaviors.IP_Restriction_Membership = ipRestrictionMask;
                        currentItem.Behaviors.Dark_Flag = isDark;

                        // Save this to the database
                        if (Resource_Object.Database.SobekCM_Database.Set_Item_Visibility(currentItem.Web.ItemID, ipRestrictionMask, isDark, embargoDate, currentUser.UserName))
                        {
                            // Update the cached item
                            Cached_Data_Manager.Remove_Digital_Resource_Object(currentItem.BibID, currentItem.VID, Tracer);
                            Cached_Data_Manager.Store_Digital_Resource_Object(currentItem.BibID, currentItem.VID, currentItem, Tracer);

                            // Update the web.config
                            Resource_Web_Config_Writer.Update_Web_Config(currentItem.Source_Directory, currentItem.Behaviors.Dark_Flag, (short)ipRestrictionMask, currentItem.Behaviors.Main_Thumbnail);

                            // Send back to this page?
                        }
                        currentMode.Mode = Display_Mode_Enum.Item_Display;
                        currentMode.Redirect();

                    }
                }
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
            Output.WriteLine("<input type=\"hidden\" id=\"permissions_action\" name=\"permissions_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"behaviors_request\" name=\"behaviors_request\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"restrictionMask\" name=\"restrictionMask\" value=\"" + ipRestrictionMask.ToString() + "\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"isDark\" name=\"isDark\" value=\"" + isDark.ToString() + "\" />");

            // Write the top currentItem mimic html portion
            Write_Item_Type_Top(Output, currentItem);

            Output.WriteLine("<div id=\"container-inner1000\">");
            Output.WriteLine("<div id=\"pagecontainer\">");

            Output.WriteLine("<!-- Edit_Item_Permissions_MySobekViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <h2>Edit item-level permissions for this currentItem</h2>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li>Use this form to change visibility (and related embargo dates) on this currentItem </li>");
            Output.WriteLine("    <li>This form also allows ip restriction and user group permissions to be set </li>");
            Output.WriteLine("    <li>Click <a href=\"" + InstanceWide_Settings_Singleton.Settings.Help_URL(currentMode.Base_URL) + "help/itempermissions\" target=\"_EDIT_INSTRUCTIONS\">here for detailed instructions</a> on editing permissions online.</li>");
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

            Output.WriteLine("        <div class=\"sbkMyEip_SetAccessText\">SET ACCESS RESTRICTIONS:</div>");

            if (( ipRestrictionMask == 0) && ( !isDark ))
                Output.WriteLine("              <button title=\"Make item public\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonPublic sbkMyEip_VisButtonCurrent\" onclick=\"set_item_access('public'); return false;\">PUBLIC ITEM</button>");
            else
                Output.WriteLine("              <button title=\"Make item public\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonPublic\" onclick=\"set_item_access('public'); return false;\">PUBLIC ITEM</button>");

            if ((ipRestrictionMask > 0) && (!isDark))
                Output.WriteLine("              <button title=\"Add IP restriction to this item\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonRestricted sbkMyEip_VisButtonCurrent\" onclick=\"set_item_access('restricted'); return false;\">RESTRICT ITEM</button>");
            else
            {
                if (ipRestrictions.Count > 0 )
                    Output.WriteLine("              <button title=\"Add IP restriction to this item\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonRestricted\" onclick=\"set_item_access('restricted'); return false;\">RESTRICT ITEM</button>");
                else
                    Output.WriteLine("              <button title=\"Add IP restriction to this item\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonRestricted\" onclick=\"alert('You must have at least one IP range entered in the system to use this option.\\n\\nAt least create an administrative range before assigning RESTRICTED to items'); return false;\">RESTRICT ITEM</button>");

            }

            if ((ipRestrictionMask < 0) && (!isDark))
                Output.WriteLine("              <button title=\"Make item private\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonPrivate sbkMyEip_VisButtonCurrent\" onclick=\"set_item_access('private'); return false;\">PRIVATE ITEM</button>");
            else
                Output.WriteLine("              <button title=\"Make item private\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonPrivate\" onclick=\"set_item_access('private'); return false;\">PRIVATE ITEM</button>");

            if (isDark)
                Output.WriteLine("              <button title=\"Make item dark\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonDark sbkMyEip_VisButtonCurrent\" onclick=\"set_item_access('dark'); return false;\">DARKEN ITEM</button>");
            else
                Output.WriteLine("              <button title=\"Make item dark\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonDark\" onclick=\"set_item_access('dark'); return false;\">DARKEN ITEM</button>");


            // Should we add ability to delete this currentItem?
            if (currentUser.Can_Delete_This_Item(currentItem.BibID, currentItem.Bib_Info.Source.Code, currentItem.Bib_Info.HoldingCode, currentItem.Behaviors.Aggregation_Code_List))
            {
                // Determine the delete URL
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Delete_Item;
                string delete_url = currentMode.Redirect_URL();
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Permissions;
                Output.WriteLine("              <button title=\"Delete this item\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonDelete\" onclick=\"if(confirm('Delete this currentItem completely?')) window.location.href = '" + delete_url + "'; return false;\">DELETE ITEM</button>");
            }

            


            
            Output.WriteLine("      <br /><br />");
            Output.WriteLine("      <table class=\"sbkMyEip_EntryTable\">");


            if ((isDark) || (ipRestrictionMask != 0))
            {
                Output.WriteLine("         <tr>");
                Output.WriteLine("           <th>Embargo Date:</th>");

                string embargoDateString = String.Empty;
                if (embargoDate.HasValue)
                    embargoDateString = embargoDate.Value.ToShortDateString();

                Output.WriteLine("           <td><input name=\'embargoDateBox' type='text' id='embargoDateBox' class='sbkMyEip_EmbargoDate sbk_Focusable' value='" + embargoDateString + "' /></td>");
                Output.WriteLine("         </tr>");
            }

            Output.WriteLine("         <tr><td colspan=\"2\">&nbsp;</td></tr>");

            if ((ipRestrictions.Count > 0) && (ipRestrictionMask > 0) && ( !isDark))
            {
                Output.WriteLine("         <tr>");
                Output.WriteLine("           <th>Restriction Ranges:-</th>");
                Output.WriteLine("           <td>");

                foreach (IP_Restriction_Range thisRange in ipRestrictions.IpRanges)
                {
                    Output.WriteLine("             <input type='checkbox' checked='checked' id='range" + thisRange.RangeID + "' name='range" + thisRange.RangeID + "' value='" + thisRange.RangeID + "' /> <label for=\"range" + thisRange.RangeID + "\"><span title=\"" + HttpUtility.HtmlEncode(thisRange.Notes) + "\">" + thisRange.Title + "</span></label><br />");
                }

                Output.WriteLine("           </td>");
                Output.WriteLine("         </tr>");
            }


            Output.WriteLine("      </table>");


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
