#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.HTML;
using SobekCM.Library.UI;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Viewer allows permissions on an item to be modified by logged on administrative users  </summary>
    public class Edit_Item_Permissions_MySobekViewer : abstract_MySobekViewer
    {
        private readonly short ipRestrictionMask;
        private readonly bool isDark;
        private DateTime? embargoDate;
        private readonly bool restrictedSelected;

        /// <summary> Constructor for a new instance of the Edit_Item_Permissions_MySobekViewer class  </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Edit_Item_Permissions_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {


            if ( RequestSpecificValues.Current_User == null ) 
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            bool userCanEditItem = RequestSpecificValues.Current_User.Can_Edit_This_Item(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.Bib_Info.SobekCM_Type_String, RequestSpecificValues.Current_Item.Bib_Info.Source.Code, RequestSpecificValues.Current_Item.Bib_Info.HoldingCode, RequestSpecificValues.Current_Item.Behaviors.Aggregation_Code_List);

            if (!userCanEditItem)
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
            }

            // Start by setting the values by the item (good the first time user comes here)
            ipRestrictionMask = RequestSpecificValues.Current_Item.Behaviors.IP_Restriction_Membership;
            isDark = RequestSpecificValues.Current_Item.Behaviors.Dark_Flag;
            restrictedSelected = (ipRestrictionMask > 0);


            // Is there already a RightsMD module in the item?
            // Ensure this metadata module extension exists
            RightsMD_Info rightsInfo = RequestSpecificValues.Current_Item.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;
            if (( rightsInfo != null) && ( rightsInfo.Has_Embargo_End ))
            {
                embargoDate = rightsInfo.Embargo_End;
            }
            

            // Is this a postback?
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                // Get the restriction mask and isDark flag
                if (HttpContext.Current.Request.Form["restrictionMask"] != null)
                {
                    ipRestrictionMask = short.Parse(HttpContext.Current.Request.Form["restrictionMask"]);
                    isDark = bool.Parse(HttpContext.Current.Request.Form["isDark"]);
                }

                // Look for embargo date
                if (HttpContext.Current.Request.Form["embargoDateBox"] != null)
                {
                    string embargoText = HttpContext.Current.Request.Form["embargoDateBox"];
                    DateTime embargoDateNew;
                    if (DateTime.TryParse(embargoText, out embargoDateNew))
                    {
                        embargoDate = embargoDateNew;
                    }
                }

                // If this was restrcted, there will be some checkboxes to determine ip restriction mask
                short checked_mask = 0;

                // Determine the IP restriction mask
                foreach (IP_Restriction_Range thisRange in UI_ApplicationCache_Gateway.IP_Restrictions.IpRanges)
                {
                    // Is this check box checked?
                    if (HttpContext.Current.Request.Form["range" + thisRange.RangeID] != null)
                    {
                        checked_mask += ((short)Math.Pow(2, (thisRange.RangeID - 1)));
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
                        switch (action)
                        {
                            case "public":
                                ipRestrictionMask = 0;
                                isDark = false;
                                restrictedSelected = false;
                                break;

                            case "private":
                                ipRestrictionMask = -1;
                                isDark = false;
                                restrictedSelected = false;
                                break;

                            case "restricted":
                                ipRestrictionMask = short.Parse(HttpContext.Current.Request.Form["selectRestrictionMask"]);
                                restrictedSelected = true;
                                isDark = false;
                                break;

                            case "dark":
                                isDark = true;
                                restrictedSelected = false;
                                break;
                        }


                    }
                }

                // Was the SAVE button pushed?
                if (HttpContext.Current.Request.Form["behaviors_request"] != null)
                {
                    string behaviorRequest = HttpContext.Current.Request.Form["behaviors_request"];
                    if (behaviorRequest == "save")
                    {
                        RequestSpecificValues.Current_Item.Behaviors.IP_Restriction_Membership = ipRestrictionMask;
                        RequestSpecificValues.Current_Item.Behaviors.Dark_Flag = isDark;

                        if ( checked_mask > 0 )
                            ipRestrictionMask = checked_mask;

                        // Save this to the database
                        if (SobekCM_Database.Set_Item_Visibility(RequestSpecificValues.Current_Item.Web.ItemID, ipRestrictionMask, isDark, embargoDate, RequestSpecificValues.Current_User.UserName))
                        {
                            // Update the web.config
                            Resource_Web_Config_Writer.Update_Web_Config(RequestSpecificValues.Current_Item.Source_Directory, RequestSpecificValues.Current_Item.Behaviors.Dark_Flag, ipRestrictionMask, RequestSpecificValues.Current_Item.Behaviors.Main_Thumbnail);

                            // Remove the cached item
                            CachedDataManager.Remove_Digital_Resource_Object(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.VID, RequestSpecificValues.Tracer);

                            // Also clear any searches or browses ( in the future could refine this to only remove those
                            // that are impacted by this save... but this is good enough for now )
                            CachedDataManager.Clear_Search_Results_Browses();
                        }
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    }
                }
            }
        }


        /// <summary> Navigation type to be displayed (mostly used by the mySobek viewers) </summary>
        /// <value> This returns none since this viewer writes all the necessary navigational elements </value>
        /// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        /// administrative tabs should be included as well.  </remarks>
        public override MySobek_Admin_Included_Navigation_Enum Standard_Navigation_Type { get { return MySobek_Admin_Included_Navigation_Enum.NONE; } }

        /// <summary>  Title for the page that displays this viewer, this is shown in the search box at 
        /// the top of the page, just below the banner </summary>
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

            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
            string item_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;

            short selectIpMask = RequestSpecificValues.Current_Item.Behaviors.IP_Restriction_Membership;
            if (selectIpMask == 0)
                selectIpMask = 1;

            Tracer.Add_Trace("Edit_Item_Permissions_MySobekViewer.Write_ItemNavForm_Closing", "");

            Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
            Output.WriteLine("<input type=\"hidden\" id=\"permissions_action\" name=\"permissions_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"behaviors_request\" name=\"behaviors_request\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"restrictionMask\" name=\"restrictionMask\" value=\"" + ipRestrictionMask.ToString() + "\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"selectRestrictionMask\" name=\"selectRestrictionMask\" value=\"" + selectIpMask + "\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"isDark\" name=\"isDark\" value=\"" + isDark.ToString() + "\" />");

            // Write the top RequestSpecificValues.Current_Item mimic html portion
            Write_Item_Type_Top(Output, RequestSpecificValues.Current_Item);

            Output.WriteLine("<div id=\"container-inner1000\">");
            Output.WriteLine("<div id=\"pagecontainer\">");

            Output.WriteLine("<!-- Edit_Item_Permissions_MySobekViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <h2>Edit item-level permissions for this item</h2>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li>Use this form to change visibility (and related embargo dates) on this item </li>");
            Output.WriteLine("    <li>This form also allows ip restriction and user group permissions to be set </li>");
            Output.WriteLine("    <li>Click <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "help/itempermissions\" target=\"_EDIT_INSTRUCTIONS\">here for detailed instructions</a> on editing permissions online.</li>");
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
            Output.WriteLine("      <script src=\"" + Static_Resources.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
            Output.WriteLine();
            Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
            Output.WriteLine("        <button onclick=\"window.location.href='" + item_url + "';return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
            Output.WriteLine("        <button onclick=\"behaviors_save_form(); return false;\" class=\"sbkMySobek_BigButton\"> SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
            Output.WriteLine("      </div>");
            Output.WriteLine("      <br /><br />");
            Output.WriteLine();

            Output.WriteLine("        <div class=\"sbkMyEip_SetAccessText\">SET ACCESS RESTRICTIONS:</div>");

            if (( ipRestrictionMask == 0) && ( !isDark ) && ( !restrictedSelected ))
                Output.WriteLine("              <button title=\"Make item public\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonPublic sbkMyEip_VisButtonCurrent\" onclick=\"set_item_access('public'); return false;\">PUBLIC ITEM</button>");
            else
                Output.WriteLine("              <button title=\"Make item public\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonPublic\" onclick=\"set_item_access('public'); return false;\">PUBLIC ITEM</button>");

            if ((restrictedSelected) && (!isDark))
                Output.WriteLine("              <button title=\"Add IP restriction to this item\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonRestricted sbkMyEip_VisButtonCurrent\" onclick=\"set_item_access('restricted'); return false;\">RESTRICT ITEM</button>");
            else
            {
                if (UI_ApplicationCache_Gateway.IP_Restrictions.Count > 0)
                    Output.WriteLine("              <button title=\"Add IP restriction to this item\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonRestricted\" onclick=\"set_item_access('restricted'); return false;\">RESTRICT ITEM</button>");
                else
                    Output.WriteLine("              <button title=\"Add IP restriction to this item\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonRestricted\" onclick=\"alert('You must have at least one IP range entered in the system to use this option.\\n\\nAt least create an administrative range before assigning RESTRICTED to items'); return false;\">RESTRICT ITEM</button>");

            }

            if ((ipRestrictionMask < 0) && (!isDark) && (!restrictedSelected))
                Output.WriteLine("              <button title=\"Make item private\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonPrivate sbkMyEip_VisButtonCurrent\" onclick=\"set_item_access('private'); return false;\">PRIVATE ITEM</button>");
            else
                Output.WriteLine("              <button title=\"Make item private\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonPrivate\" onclick=\"set_item_access('private'); return false;\">PRIVATE ITEM</button>");

            if (isDark)
                Output.WriteLine("              <button title=\"Make item dark\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonDark sbkMyEip_VisButtonCurrent\" onclick=\"set_item_access('dark'); return false;\">DARKEN ITEM</button>");
            else
                Output.WriteLine("              <button title=\"Make item dark\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonDark\" onclick=\"set_item_access('dark'); return false;\">DARKEN ITEM</button>");


            // Should we add ability to delete this RequestSpecificValues.Current_Item?
            if (RequestSpecificValues.Current_User.Can_Delete_This_Item(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.Bib_Info.Source.Code, RequestSpecificValues.Current_Item.Bib_Info.HoldingCode, RequestSpecificValues.Current_Item.Behaviors.Aggregation_Code_List))
            {
                // Determine the delete URL
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Delete_Item;
                string delete_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Permissions;
                Output.WriteLine("              <button title=\"Delete this item\" class=\"sbkMyEip_VisButton sbkMyEip_VisButtonDelete\" onclick=\"if(confirm('Delete this item completely?')) window.location.href = '" + delete_url + "'; return false;\">DELETE ITEM</button>");
            }

            


            
            Output.WriteLine("      <br /><br />");
            Output.WriteLine("      <table class=\"sbkMyEip_EntryTable\">");


            if ((isDark) || (ipRestrictionMask != 0) || ( restrictedSelected ))
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

            if ((UI_ApplicationCache_Gateway.IP_Restrictions.Count > 0) && (restrictedSelected) && (!isDark))
            {
                Output.WriteLine("         <tr>");
                Output.WriteLine("           <th>Restriction Ranges:-</th>");
                Output.WriteLine("           <td>");

                // At least always select the FIRST ip range, if restricted is selected
                short forComparison = RequestSpecificValues.Current_Item.Behaviors.IP_Restriction_Membership;
                if (forComparison == 0)
                    forComparison = 1;
                foreach (IP_Restriction_Range thisRange in UI_ApplicationCache_Gateway.IP_Restrictions.IpRanges)
                {
                    int comparison = forComparison & ((short)Math.Pow(2, thisRange.RangeID - 1));
                    if (comparison == 0)
                    {
                        Output.WriteLine("             <input type='checkbox' id='range" + thisRange.RangeID + "' name='range" + thisRange.RangeID + "' value='" + thisRange.RangeID + "' /> <label for=\"range" + thisRange.RangeID + "\"><span title=\"" + HttpUtility.HtmlEncode(thisRange.Notes) + "\">" + thisRange.Title + "</span></label><br />");
                    }
                    else
                    {
                        Output.WriteLine("             <input type='checkbox' checked='checked' id='range" + thisRange.RangeID + "' name='range" + thisRange.RangeID + "' value='" + thisRange.RangeID + "' /> <label for=\"range" + thisRange.RangeID + "\"><span title=\"" + HttpUtility.HtmlEncode(thisRange.Notes) + "\">" + thisRange.Title + "</span></label><br />");
                    }
                }

                Output.WriteLine("           </td>");
                Output.WriteLine("         </tr>");
            }


            Output.WriteLine("      </table>");


            // Add the second buttons at the bottom of the form
            Output.WriteLine();
            Output.WriteLine("      <!-- Add SAVE and CANCEL buttons to bottom of form -->");
            Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
            Output.WriteLine("        <button onclick=\"window.location.href='" + item_url + "';return false;\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
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
    }
}
