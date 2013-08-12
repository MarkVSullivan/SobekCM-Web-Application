#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view all existing item aggregations, select an item aggregation to edit, and add new aggregations </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to view and edit existing item aggregations in this digital library</li>
    /// </ul></remarks>
    public class Aggregations_Mgmt_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly string enteredCode;
        private readonly string enteredDescription;
        private readonly bool enteredIsActive;
        private readonly bool enteredIsHidden;
        private readonly string enteredLink;
        private readonly string enteredName;
        private readonly string enteredParent;
        private readonly string enteredShortname;
        private readonly string enteredType;

        /// <summary> Constructor for a new instance of the Aggregations_Mgmt_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="currentMode"> Mode / navigation information for the current request</param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public Aggregations_Mgmt_AdminViewer(User_Object User, SobekCM_Navigation_Object currentMode, Aggregation_Code_Manager Code_Manager, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Aggregations_Mgmt_AdminViewer.Constructor", String.Empty);

            codeManager = Code_Manager;

            // Set some defaults
            actionMessage = String.Empty;
            enteredCode = String.Empty;
            enteredParent = String.Empty;
            enteredType = String.Empty;
            enteredShortname = String.Empty;
            enteredName = String.Empty;
            enteredDescription = String.Empty;
            enteredIsActive = false;
            enteredIsHidden = false;

            // If the user cannot edit this, go back
            if ((!user.Is_System_Admin) && ( !user.Is_Portal_Admin ))
            {
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
                return;
            }

            // If this is a postback, handle any events first
            if (currentMode.isPostBack)
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    string save_value = form["admin_aggr_tosave"].ToUpper().Trim();
                    string new_aggregation_code = String.Empty;
                    if ( form["admin_aggr_code"] != null )
                        new_aggregation_code = form["admin_aggr_code"].ToUpper().Trim();

                    // Check for reset request as well
                    string reset_aggregation_code = String.Empty;
                    if (form["admin_aggr_reset"] != null)
                        reset_aggregation_code = form["admin_aggr_reset"].ToLower().Trim();

                    // If there is a reset request here, purge the aggregation from the cache
                    if (reset_aggregation_code.Length > 0)
                    {
                        Cached_Data_Manager.Remove_Item_Aggregation(reset_aggregation_code, Tracer);
                    }

                    // If there was a save value continue to pull the rest of the data
                    if (save_value.Length > 0)
                    {

                        bool is_active = false;
                        bool is_hidden = false;
                        object temp_object;


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

                            temp_object = form["admin_aggr_isactive"];
                            if (temp_object != null)
                            {
                                is_active = true;
                            }

                            temp_object = form["admin_aggr_ishidden"];
                            if (temp_object != null)
                            {
                                is_hidden = true;
                            }

                            // Convert to the integer id for the parent and begin to do checking
                            List<string> errors = new List<string>();
                            int parentid = -1;
                            if (new_parent.Length > 0)
                            {
                                try
                                {
                                    parentid = Convert.ToInt32(new_parent);
                                }
                                catch
                                {
                                    errors.Add("Invalid parent id selected!");
                                }
                            }
                            else
                            {
                                errors.Add("You must select a PARENT for this new aggregation");
                            }

                            // Get the list of all aggregations
                            if (new_aggregation_code.Length > 20)
                            {
                                errors.Add("New aggregation code must be twenty characters long or less");
                            }
                            else
                            {
                                if (codeManager[new_aggregation_code] != null )
                                {
                                    errors.Add("New code must be unique... <i>" + new_aggregation_code + "</i> already exists");
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

                                // Try to save the new item aggregation
                                if (SobekCM_Database.Save_Item_Aggregation(new_aggregation_code, new_name, new_shortname, new_description, correct_type, is_active, is_hidden, new_link, parentid, Tracer))
                                {
                                    // Ensure a folder exists for this, otherwise create one
                                    try
                                    {
                                        string folder = SobekCM_Library_Settings.Base_Design_Location + "aggregations\\" + new_aggregation_code.ToLower();
                                        if (!Directory.Exists(folder))
                                        {
                                            // Create this directory and all the subdirectories
                                            Directory.CreateDirectory(folder);
                                            Directory.CreateDirectory(folder + "/html");
                                            Directory.CreateDirectory(folder + "/images");
                                            Directory.CreateDirectory(folder + "/html/home");
                                            Directory.CreateDirectory(folder + "/images/buttons");
                                            Directory.CreateDirectory(folder + "/images/banners");

                                            // Create a default home text file
                                            StreamWriter writer = new StreamWriter(folder + "/html/home/text.html");
                                            writer.WriteLine("<br />New collection home page text goes here.<br /><br />To edit this, edit the following file: " + folder + "\\html\\home\\text.html.<br /><br />");
                                            writer.Flush();
                                            writer.Close();

                                            // Copy the default banner and buttons from images
                                            if (File.Exists(SobekCM_Library_Settings.Base_Directory + "default/images/default_button.png"))
                                                File.Copy(SobekCM_Library_Settings.Base_Directory + "default/images/default_button.png", folder + "/images/buttons/coll.png");
                                            if (File.Exists(SobekCM_Library_Settings.Base_Directory + "default/images/default_button.gif"))
                                                File.Copy(SobekCM_Library_Settings.Base_Directory + "default/images/default_button.gif", folder + "/images/buttons/coll.gif");
                                            if (File.Exists(SobekCM_Library_Settings.Base_Directory + "default/images/default_banner.jpg"))
                                                File.Copy(SobekCM_Library_Settings.Base_Directory + "default/images/default_banner.jpg", folder + "/images/banners/coll.jpg");

                                            // Now, try to create the item aggregation and write the configuration file
                                            Item_Aggregation itemAggregation = Item_Aggregation_Builder.Get_Item_Aggregation(new_aggregation_code, String.Empty, null, false, Tracer);
                                            itemAggregation.Write_Configuration_File(SobekCM_Library_Settings.Base_Design_Location + itemAggregation.objDirectory);
                                        }
                                    }
                                    catch { }

                                    // Reload the list of all codes, to include this new one and the new hierarchy
                                    lock (codeManager)
                                    {
                                        SobekCM_Database.Populate_Code_Manager(codeManager, Tracer);
                                    }
                                    actionMessage = "New item aggregation <i>" + new_aggregation_code + "</i> saved successfully";
                                }
                                else
                                {
                                    actionMessage = "ERROR saving the new item aggregation to the database";
                                }
                            }
                        }
                    }
                }
                catch 
                {
                    actionMessage = "General error while reading postback information";
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Interfaces' </value>
        public override string Web_Title
        {
            get { return "Item Aggregations"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aggregations_Mgmt_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aggregations_Mgmt_AdminViewer.Add_HTML_In_Main_Form", "");

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_tosave\" name=\"admin_aggr_tosave\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_reset\" name=\"admin_aggr_reset\" value=\"\" />");
            Output.WriteLine();

               Output.WriteLine("<!-- Aggregations_Mgmt_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");
            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            Output.WriteLine("  <blockquote>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/aggregations\" target=\"ADMIN_INTERFACE_HELP\" >click here to view the help page</a>.</blockquote>");


            // Find the matching type to display
            int index = 0;
            if (currentMode.My_Sobek_SubMode.Length > 0)
            {
                Int32.TryParse(currentMode.My_Sobek_SubMode, out index);
            }

            if ((index <= 0) || (index > codeManager.Types_Count))
            {

                Output.WriteLine("  <span class=\"SobekAdminTitle\">New Item Aggregation</span>");

                Output.WriteLine("    <blockquote>");
                Output.WriteLine("      <div class=\"admin_aggr_new_div\">");
                Output.WriteLine("        <table class=\"popup_table\">");

                // Add line for aggregation code and aggregation type
                Output.WriteLine("          <tr><td width=\"120px\"><label for=\"admin_aggr_code\">Code:</label></td>");
                Output.WriteLine("            <td><input class=\"admin_aggr_small_input\" name=\"admin_aggr_code\" id=\"admin_aggr_code\" type=\"text\" value=\"" + enteredCode + "\"  onfocus=\"javascript:textbox_enter('admin_aggr_code', 'admin_aggr_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_code', 'admin_aggr_small_input')\" /></td>");
                Output.WriteLine("            <td width=\"300px\" align=\"right\"><label for=\"admin_aggr_type\">Type:</label> &nbsp; ");
                Output.Write("<select class=\"admin_aggr_select\" name=\"admin_aggr_type\" id=\"admin_aggr_type\">");
                if (enteredType == String.Empty)
                    Output.Write("<option value=\"\" selected=\"selected\" ></option>");

                Output.Write(enteredType == "coll"
                                 ? "<option value=\"coll\" selected=\"selected\" >Collection</option>"
                                 : "<option value=\"coll\">Collection</option>");

                Output.Write(enteredType == "group"
                                 ? "<option value=\"group\" selected=\"selected\" >Collection Group</option>"
                                 : "<option value=\"group\">Collection Group</option>");

                Output.Write(enteredType == "exhibit"
                                 ? "<option value=\"exhibit\" selected=\"selected\" >Exhibit</option>"
                                 : "<option value=\"exhibit\">Exhibit</option>");

                Output.Write(enteredType == "inst"
                                 ? "<option value=\"inst\" selected=\"selected\" >Institution</option>"
                                 : "<option value=\"inst\">Institution</option>");

                Output.Write(enteredType == "subinst"
                                 ? "<option value=\"subinst\" selected=\"selected\" >Institutional Division</option>"
                                 : "<option value=\"subinst\">Institutional Division</option>");

                Output.Write(enteredType == "subcoll"
                                 ? "<option value=\"subcoll\" selected=\"selected\" >SubCollection</option>"
                                 : "<option value=\"subcoll\">SubCollection</option>");

                Output.WriteLine("</select></td></tr>");

                // Add the parent line
                Output.Write("          <tr><td><label for=\"admin_aggr_parent\">Parent:</label></td><td colspan=\"2\">");
                Output.Write("<select class=\"admin_aggr_select_large\" name=\"admin_aggr_parent\" id=\"admin_aggr_parent\">");
                if (enteredParent == String.Empty)
                    Output.Write("<option value=\"\" selected=\"selected\" ></option>");
                foreach (Item_Aggregation_Related_Aggregations thisAggr in codeManager.All_Aggregations)
                {
                    if (enteredParent == thisAggr.ID.ToString())
                    {
                        Output.Write("<option value=\"" + thisAggr.ID + "\" selected=\"selected\" >" + thisAggr.Code + " - " + thisAggr.Name + "</option>");
                    }
                    else
                    {
                        Output.Write("<option value=\"" + thisAggr.ID + "\" >" + thisAggr.Code + " - " + thisAggr.Name + "</option>");
                    }
                }
                Output.WriteLine("<select></td></tr>");

                // Add the full name line
                Output.WriteLine("          <tr><td><label for=\"admin_aggr_name\">Name (full):</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_name\" id=\"admin_aggr_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredName) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_name', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_name', 'admin_aggr_large_input')\" /></td></tr>");

                // Add the short name line
                Output.WriteLine("          <tr><td><label for=\"admin_aggr_shortname\">Name (short):</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredShortname) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_shortname', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_shortname', 'admin_aggr_large_input')\" /></td></tr>");

                // Add the link line
                Output.WriteLine("          <tr><td><label for=\"admin_aggr_link\">External Link:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_link\" id=\"admin_aggr_link\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredLink) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_link', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_link', 'admin_aggr_large_input')\" /></td></tr>");

                // Add the description box
                int actual_cols = 75;
                if (currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                    actual_cols = 70;
                Output.WriteLine("          <tr valign=\"top\"><td valign=\"top\"><label for=\"admin_aggr_desc\">Description:</label></td><td colspan=\"2\"><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\" class=\"admin_aggr_input\" onfocus=\"javascript:textbox_enter('admin_aggr_desc','admin_aggr_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_desc','admin_aggr_input')\">" + HttpUtility.HtmlEncode(enteredDescription) + "</textarea></td></tr>");

                // Add checkboxes for is active and is hidden
                Output.Write(enteredIsActive
                                 ? "          <tr height=\"30px\"><td>Behavior:</td><td colspan=\"2\"><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" checked=\"checked\" /> <label for=\"admin_aggr_isactive\">Active?</label> "
                                 : "          <tr height=\"30px\"><td>Behavior:</td><td colspan=\"2\"><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" /> <label for=\"admin_aggr_isactive\">Active?</label> ");

                Output.Write("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ");

                Output.Write(enteredIsHidden
                                 ? "<input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" checked=\"checked\" /> <label for=\"admin_aggr_ishidden\">Hidden?</label> "
                                 : "<input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" /> <label for=\"admin_aggr_ishidden\">Hidden?</label> ");
                Output.WriteLine("</td></tr>");

                Output.WriteLine("        </table>");

                Output.WriteLine("        <center><input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\" onclick=\"return save_new_aggr();\"/></center>");

                Output.WriteLine("      </div>");
                Output.WriteLine("    </blockquote>");
                Output.WriteLine("    <br />");


                Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Item Aggregations</span>");
                Output.WriteLine("  <br /><br />");

                Output.WriteLine("  <a name=\"list\">");
                Output.WriteLine("  <blockquote>");
                Output.WriteLine("    Select a type below to view all matching item aggregations:");
                Output.WriteLine("    <blockquote>");
                int i = 1;
                foreach (string thisType in codeManager.All_Types)
                {
                    currentMode.My_Sobek_SubMode = i.ToString();
                    Output.WriteLine("      <a href=\"" + currentMode.Redirect_URL() + "\" >" + thisType.ToUpper() + "</a><br /><br />");
                    i++;
                }
                currentMode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("    </blockquote>");
                Output.WriteLine("  </blockquote>");
            }
            else
            {
                string aggregationType = codeManager.All_Types[index - 1];

                Output.WriteLine("  <span class=\"SobekAdminTitle\">Other Actions</span>");
                Output.WriteLine("    <blockquote>");
                currentMode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("      <a href=\"" + currentMode.Redirect_URL() + "\">Add new item aggregation</a><br /><br />");
                Output.WriteLine("      <a href=\"" + currentMode.Redirect_URL() + "#list\">View different aggregations</a>");
                currentMode.My_Sobek_SubMode = index.ToString();
                Output.WriteLine("    </blockquote>");
                Output.WriteLine("    <br />");

                Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing " + aggregationType + "s</span>");
                Output.WriteLine("  <br /><br />");

                Output.WriteLine("    <blockquote>");

                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
                Output.WriteLine("    <th width=\"120px\" align=\"left\"><span style=\"color: White\">CODE</span></th>");
                Output.WriteLine("    <th align=\"left\"><span style=\"color: White\">NAME</span></th>");
                Output.WriteLine("   </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                if ((aggregationType.Length > 0) && (aggregationType[aggregationType.Length - 1] != 'S'))
                {
                    Output.WriteLine("    <td colspan=\"3\"><span style=\"color: White\"><b> &nbsp; " + aggregationType.ToUpper() + "S</b></span></td>");
                }
                else
                {
                    Output.WriteLine("    <td colspan=\"3\"><span style=\"color: White\"><b> &nbsp; " + aggregationType.ToUpper() + "</b></span></td>");
                }
                Output.WriteLine("  </tr>");

                // Show all matching rows
                string last_code = String.Empty;
                foreach (Item_Aggregation_Related_Aggregations thisAggr in codeManager.Aggregations_By_Type(aggregationType))
                {
                    if (thisAggr.Code != last_code)
                    {
                        last_code = thisAggr.Code;

                        // Build the action links
                        Output.WriteLine("  <tr align=\"left\" >");
                        Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                        Output.Write("<a title=\"Click to edit this item aggregation\" id=\"EDIT_" + thisAggr.Code + "\" href=\"" + currentMode.Base_URL + "my/editaggr/" + thisAggr.Code + "\">edit</a> | ");
                        if (thisAggr.Active)
                            Output.Write("<a title=\"Click to view this item aggregation\" href=\"" + currentMode.Base_URL + "l/" + thisAggr.Code + "\">view</a> | ");
                        else
                            Output.Write("view | ");

                        Output.Write("<a title=\"Click to reset the instance in the application cache\" id=\"RESET_" + thisAggr.Code + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return reset_aggr('" + thisAggr.Code + "');\">reset</a> )</td>");

                        // Add the rest of the row with data
                        Output.WriteLine("    <td>" + thisAggr.Code + "</span></td>");
                        Output.WriteLine("    <td>" + thisAggr.Name + "</span></td>");
                        Output.WriteLine("   </tr>");
                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                    }
                }

                Output.WriteLine("</table>");
                Output.WriteLine("    </blockquote>");
            }

            Output.WriteLine("    <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}
