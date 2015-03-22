using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Client;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Message;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Tools;
using SobekCM.UI_Library;

namespace SobekCM.Library.AdminViewer
{
    public class Add_Collection_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;

        private readonly string enteredCode;
        private readonly string enteredDescription;
        private readonly bool enteredIsActive;
        private readonly bool enteredIsHidden;
        private readonly string enteredLink;
        private readonly string enteredName;
        private readonly string enteredParent;
        private readonly string enteredShortname;
        private readonly string enteredType;
        private readonly string enteredThematicHeading;

        /// <summary> Constructor for a new instance of the Add_Collection_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public Add_Collection_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Add_Collection_AdminViewer.Constructor", String.Empty);

            // Set some defaults
            actionMessage = String.Empty;
            enteredCode = String.Empty;
            enteredParent = String.Empty;
            enteredType = String.Empty;
            enteredShortname = String.Empty;
            enteredName = String.Empty;
            enteredDescription = String.Empty;
            enteredThematicHeading = String.Empty;
            enteredIsActive = true;
            enteredIsHidden = false;

            // If the user cannot edit this, go back
            if ((RequestSpecificValues.Current_User == null) || ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin)))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If this is a postback, handle any events first
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    string save_value = form["admin_aggr_tosave"].ToUpper().Trim();
                    string new_aggregation_code = String.Empty;
                    if (form["admin_aggr_code"] != null)
                        new_aggregation_code = form["admin_aggr_code"].ToUpper().Trim();

                    // If there was a save value continue to pull the rest of the data
                    if (save_value.Length > 0)
                    {

                        bool is_active = false;
                        bool is_hidden = true;


                        // Was this to save a new aggregation (from the main page) or edit an existing (from the popup form)?
                        if (save_value == new_aggregation_code)
                        {

                            // Pull the values from the submitted form
                            string new_type = form["admin_aggr_type"];
                            string new_parent = form["admin_aggr_parent"].Trim();
                            string new_name = form["admin_aggr_name"].Trim();
                            string new_shortname = form["admin_aggr_shortname"].Trim();
                            string new_description = form["admin_aggr_desc"].Trim();
                            string new_link = form["admin_aggr_link"].Trim();
                            string new_thematic_heading = form["admin_aggr_heading"].Trim();

                            object temp_object = form["admin_aggr_isactive"];
                            if (temp_object != null)
                            {
                                is_active = true;
                            }

                            temp_object = form["admin_aggr_ishidden"];
                            if (temp_object != null)
                            {
                                is_hidden = false;
                            }

                            // Convert to the integer id for the parent and begin to do checking
                            List<string> errors = new List<string>();
                            if ( String.IsNullOrEmpty(new_parent))
                            {
                                errors.Add("You must select a PARENT for this new aggregation");
                            }

                            // Validate the code

                            if (new_aggregation_code.Length > 20)
                            {
                                errors.Add("New aggregation code must be twenty characters long or less");
                            }
                            else if (new_aggregation_code.Length == 0)
                            {
                                errors.Add("You must enter a CODE for this item aggregation");
                            }
                            else if (UI_ApplicationCache_Gateway.Aggregations[new_aggregation_code.ToUpper()] != null)
                            {
                                errors.Add("New code must be unique... <i>" + new_aggregation_code + "</i> already exists");
                            }
                            else if (UI_ApplicationCache_Gateway.Settings.Reserved_Keywords.Contains(new_aggregation_code.ToLower()))
                            {
                                errors.Add("That code is a system-reserved keyword.  Try a different code.");
                            }
                            else
                            {
                                bool alphaNumericTest = new_aggregation_code.All(C => Char.IsLetterOrDigit(C) || C == '_' || C == '-');
                                if (!alphaNumericTest)
                                {
                                    errors.Add("New aggregation code must be only letters and numbers");
                                    new_aggregation_code = new_aggregation_code.Replace("\"", "");
                                }
                            }

                            // Was there a type and name
                            if (new_type.Length == 0)
                            {
                                errors.Add("You must select a TYPE for this new aggregation");
                            }
                            if (new_description.Length == 0)
                            {
                                errors.Add("You must enter a DESCRIPTION for this new aggregation");
                            }
                            if (new_name.Length == 0)
                            {
                                errors.Add("You must enter a NAME for this new aggregation");
                            }
                            else
                            {
                                if (new_shortname.Length == 0)
                                    new_shortname = new_name;
                            }

                            if (errors.Count > 0)
                            {
                                // Create the error message
                                actionMessage = "ERROR: Invalid entry for new item aggregation<br />";
                                foreach (string error in errors)
                                    actionMessage = actionMessage + "<br />" + error;

                                // Save all the values that were entered
                                enteredCode = new_aggregation_code;
                                enteredDescription = new_description;
                                enteredIsActive = is_active;
                                enteredIsHidden = is_hidden;
                                enteredName = new_name;
                                enteredParent = new_parent;
                                enteredShortname = new_shortname;
                                enteredType = new_type;
                                enteredLink = new_link;
                                enteredThematicHeading = new_thematic_heading;
                            }
                            else
                            {
                                // Get the correct type
                                string correct_type = "Collection";
                                switch (new_type)
                                {
                                    case "coll":
                                        correct_type = "Collection";
                                        break;

                                    case "group":
                                        correct_type = "Collection Group";
                                        break;

                                    case "subcoll":
                                        correct_type = "SubCollection";
                                        break;

                                    case "inst":
                                        correct_type = "Institution";
                                        break;

                                    case "exhibit":
                                        correct_type = "Exhibit";
                                        break;

                                    case "subinst":
                                        correct_type = "Institutional Division";
                                        break;
                                }
                                // Make sure inst and subinst start with 'i'
                                if (new_type.IndexOf("inst") >= 0)
                                {
                                    if (new_aggregation_code[0] == 'I')
                                        new_aggregation_code = "i" + new_aggregation_code.Substring(1);
                                    if (new_aggregation_code[0] != 'i')
                                        new_aggregation_code = "i" + new_aggregation_code;
                                }

                                // Get the thematic heading id (no checks here)
                                string thematicHeading = null;
                                if (form["admin_aggr_heading"] != null)
                                {
                                    int thematicHeadingId = Convert.ToInt32(form["admin_aggr_heading"]);
                                    foreach (Thematic_Heading thisHeading in UI_ApplicationCache_Gateway.Thematic_Headings)
                                    {
                                        if (thisHeading.ID == thematicHeadingId)
                                        {
                                            thematicHeading = thisHeading.Text;
                                            break;
                                        }
                                    }
                                }

                                // Create the new aggregation argument object
                                New_Aggregation_Arguments args = new New_Aggregation_Arguments
                                {
                                    Active = is_active,
                                    Code = new_aggregation_code,
                                    Description = new_description, 
                                    External_Link = enteredLink,
                                    Hidden = is_hidden,
                                    Name = new_name, 
                                    ParentCode = new_parent,
                                    ShortName = new_shortname,
                                    Thematic_Heading = thematicHeading, 
                                    Type = correct_type, 
                                    User = RequestSpecificValues.Current_User.Full_Name
                                };

                                // Try to add this aggregation
                                ErrorRestMessage msg = SobekEngineClient.Aggregations.Add_New_Aggregation(args);

                                if (msg.ErrorType == ErrorRestType.Successful)
                                {
                                    // Clear all aggregation information (and thematic heading info) from the cache as well
                                    CachedDataManager.Aggregations.Clear();

                                    // Forward to the aggregation
                                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                                    RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                                    RequestSpecificValues.Current_Mode.Aggregation = new_aggregation_code;

                                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                                    return;
                                }
                                else
                                {
                                    actionMessage = msg.Message;
                                }
                            }
                        }
                    }
                }
                catch ( Exception ee )
                {
                    actionMessage = "General error while reading postback information";
                }
            }
        }

        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum> {HtmlSubwriter_Behaviors_Enum.Suppress_Banner, HtmlSubwriter_Behaviors_Enum.Use_Jquery_DataTables}; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Skins' </value>
        public override string Web_Title
        {
            get { return "Add a Collection Wizard"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Add_Collection_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Add_Collection_AdminViewer.Write_ItemNavForm_Closing", "");

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_tosave\" name=\"admin_aggr_tosave\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_reset\" name=\"admin_aggr_reset\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_delete\" name=\"admin_aggr_delete\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- Aggregations_Mgmt_AdminViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

            if (actionMessage.Length > 0)
            {
                if (actionMessage.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageError\">" + actionMessage + "</div>");
                }
                else
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageSuccess\">" + actionMessage + "</div>");
                }

            }

            Output.WriteLine("  <p>For clarification of any terms on this form, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/aggregations\" target=\"ADMIN_INTERFACE_HELP\" >click here to view the help page</a>.</p>");


            Output.WriteLine("  <h2>New Item Aggregation</h2>");

            Output.WriteLine("  <div class=\"sbkAsav_NewDiv\">");
            Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");

            // Add line for aggregation code and aggregation type
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td style=\"width:120px;\"><label for=\"admin_aggr_code\">Code:</label></td>");
            Output.WriteLine("        <td><input class=\"sbkAsav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_code\" id=\"admin_aggr_code\" type=\"text\" value=\"" + enteredCode + "\" /></td>");
            Output.WriteLine("        <td style=\"width:300px;text-align:right;\">");
            Output.WriteLine("          <label for=\"admin_aggr_type\">Type:</label> &nbsp; ");
            Output.WriteLine("          <select class=\"sbkAsav_select \" name=\"admin_aggr_type\" id=\"admin_aggr_type\" onchange=\"return aggr_display_external_link(this);\">");
            if (enteredType == String.Empty)
                Output.WriteLine("            <option value=\"\" selected=\"selected\" ></option>");

            Output.WriteLine(enteredType == "coll"
                ? "            <option value=\"coll\" selected=\"selected\" >Collection</option>"
                : "            <option value=\"coll\">Collection</option>");

            Output.WriteLine(enteredType == "group"
                ? "            <option value=\"group\" selected=\"selected\" >Collection Group</option>"
                : "            <option value=\"group\">Collection Group</option>");

            Output.WriteLine(enteredType == "exhibit"
                ? "            <option value=\"exhibit\" selected=\"selected\" >Exhibit</option>"
                : "            <option value=\"exhibit\">Exhibit</option>");

            Output.WriteLine(enteredType == "inst"
                ? "            <option value=\"inst\" selected=\"selected\" >Institution</option>"
                : "            <option value=\"inst\">Institution</option>");

            Output.WriteLine(enteredType == "subinst"
                ? "            <option value=\"subinst\" selected=\"selected\" >Institutional Division</option>"
                : "            <option value=\"subinst\">Institutional Division</option>");

            Output.WriteLine(enteredType == "subcoll"
                ? "            <option value=\"subcoll\" selected=\"selected\" >SubCollection</option>"
                : "            <option value=\"subcoll\">SubCollection</option>");

            Output.WriteLine("          </select>");
            Output.WriteLine("        </td>");
            Output.WriteLine("      </tr>");

            // Add the parent line
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td>");
            Output.WriteLine("          <label for=\"admin_aggr_parent\">Parent:</label></td><td colspan=\"2\">");
            Output.WriteLine("          <select class=\"sbkAsav_select_large\" name=\"admin_aggr_parent\" id=\"admin_aggr_parent\">");
            if (enteredParent == String.Empty)
                Output.WriteLine("            <option value=\"\" selected=\"selected\" ></option>");
            foreach (Item_Aggregation_Related_Aggregations thisAggr in UI_ApplicationCache_Gateway.Aggregations.All_Aggregations)
            {
                if (enteredParent == thisAggr.Code)
                {
                    Output.WriteLine("            <option value=\"" + thisAggr.Code + "\" selected=\"selected\" >" + thisAggr.Code + " - " + thisAggr.Name + "</option>");
                }
                else
                {
                    Output.WriteLine("            <option value=\"" + thisAggr.Code + "\" >" + thisAggr.Code + " - " + thisAggr.Name + "</option>");
                }
            }
            Output.WriteLine("          </select>");
            Output.WriteLine("        </td>");
            Output.WriteLine("      </tr>");

            // Add the full name line
            Output.WriteLine("      <tr><td><label for=\"admin_aggr_name\">Name (full):</label></td><td colspan=\"2\"><input class=\"sbkAsav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_name\" id=\"admin_aggr_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredName) + "\" /></td></tr>");

            // Add the short name line
            Output.WriteLine("      <tr><td><label for=\"admin_aggr_shortname\">Name (short):</label></td><td colspan=\"2\"><input class=\"sbkAsav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredShortname) + "\" /></td></tr>");

            // Add the link line
            Output.WriteLine("      <tr id=\"external_link_row\" style=\"display:none;\"><td><label for=\"admin_aggr_link\">External Link:</label></td><td colspan=\"2\"><input class=\"sbkAsav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_link\" id=\"admin_aggr_link\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredLink) + "\" /></td></tr>");

            // Add the thematic heading line
            Output.WriteLine("      <tr>");
            Output.WriteLine("        <td><label for=\"admin_aggr_heading\">Thematic Heading:</label></td>");
            Output.WriteLine("        <td colspan=\"2\">");
            Output.WriteLine("          <select class=\"sbkAsav_select_large\" name=\"admin_aggr_heading\" id=\"admin_aggr_heading\">");
            Output.WriteLine("            <option value=\"-1\" selected=\"selected\" ></option>");
            foreach (Thematic_Heading thisHeading in UI_ApplicationCache_Gateway.Thematic_Headings)
            {
                if ( thisHeading.Text == enteredThematicHeading )
                    Output.Write("            <option value=\"" + thisHeading.ID + "\" selected=\"selected\">" + HttpUtility.HtmlEncode(thisHeading.Text) + "</option>");
                else
                    Output.Write("            <option value=\"" + thisHeading.ID + "\">" + HttpUtility.HtmlEncode(thisHeading.Text) + "</option>");
            }
            Output.WriteLine("          </select>");
            Output.WriteLine("        </td>");
            Output.WriteLine("      </tr>");



            // Add the description box
            Output.WriteLine("      <tr style=\"vertical-align:top\"><td><label for=\"admin_aggr_desc\">Description:</label></td><td colspan=\"2\"><textarea rows=\"6\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\" class=\"sbkAsav_input sbkAdmin_Focusable\">" + HttpUtility.HtmlEncode(enteredDescription) + "</textarea></td></tr>");

            // Add checkboxes for is active and is hidden
            Output.Write(enteredIsActive
                ? "          <tr style=\"height:30px\"><td>Behavior:</td><td colspan=\"2\"><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" checked=\"checked\" /> <label for=\"admin_aggr_isactive\">Active?</label></td></tr> "
                : "          <tr style=\"height:30px\"><td>Behavior:</td><td colspan=\"2\"><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" /> <label for=\"admin_aggr_isactive\">Active?</label></td></tr> ");


            Output.Write(!enteredIsHidden
                ? "          <tr><td></td><td colspan=\"2\"><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" checked=\"checked\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label></td></tr> "
                : "          <tr><td></td><td colspan=\"2\"><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label></td></tr> ");


            // Add the SAVE button
            Output.WriteLine("      <tr style=\"height:30px; text-align: center;\"><td colspan=\"3\"><button title=\"Save new item aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_aggr();\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td></tr>");
            Output.WriteLine("    </table>");
            Output.WriteLine("  </div>");
            Output.WriteLine();

            Output.WriteLine("  <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}
