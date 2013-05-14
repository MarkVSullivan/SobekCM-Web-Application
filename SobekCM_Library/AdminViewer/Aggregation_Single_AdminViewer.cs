#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Search;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated aggregation admin to edit information related to a single item aggregation. </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to view and edit information related to a single item aggregation</li>
    /// </ul></remarks>
    public class Aggregation_Single_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly Item_Aggregation itemAggregation;
        private List<Thematic_Heading> thematicHeadings;
        private SobekCM_Skin_Collection webSkins;

        private int page;


        /// <summary> Constructor for a new instance of the Aggregation_Single_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Thematic_Headings"> Headings under which all the highlighted collections on the home page are organized </param>       
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public Aggregation_Single_AdminViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Aggregation_Code_Manager Code_Manager, List<Thematic_Heading> Thematic_Headings, SobekCM_Skin_Collection Web_Skin_Collection, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Aggregation_Single_AdminViewer.Constructor", String.Empty);

            // Save the parameters
            thematicHeadings = Thematic_Headings;
            webSkins = Web_Skin_Collection;
            codeManager = Code_Manager;
            base.currentMode = Current_Mode;

            // Set some defaults
            actionMessage = String.Empty;

            // Get the code for the aggregation being edited
            string code = currentMode.Aggregation;

            // If the user cannot edit this, go back
            if (!user.Is_Aggregation_Curator(code))
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
            }

            // Load the item aggregation, either currenlty from the session (if already editing this aggregation )
            // or by reading all the appropriate XML and reading data from the database
            object possibleEditAggregation = HttpContext.Current.Session["Edit_Aggregation_" + code];
            Item_Aggregation cachedInstance = null;
            if (possibleEditAggregation != null)
                cachedInstance = (Item_Aggregation)possibleEditAggregation;

            itemAggregation = Item_Aggregation_Builder.Get_Item_Aggregation(code, String.Empty, cachedInstance, false, Tracer);
            
            // If unable to retrieve this aggregation, send to home
            if (itemAggregation == null)
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
            }

            // Determine the page
            page = 1;
            if (currentMode.My_Sobek_SubMode == "b" )
                page = 2;
            else if (currentMode.My_Sobek_SubMode == "c")
                page = 3;
            else if (currentMode.My_Sobek_SubMode == "d")
                page = 4;
            else if (currentMode.My_Sobek_SubMode == "e")
                page = 5;
            else if (currentMode.My_Sobek_SubMode == "f")
                page = 6;
            else if (currentMode.My_Sobek_SubMode == "g")
                page = 7;

            // If this is a postback, handle any events first
            if (currentMode.isPostBack)
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    // Get the curret action
                    string action = form["admin_aggr_save"];

                    // If this is to cancel, handle that here; no need to handle post-back from the
                    // editing form page first
                    if (action == "z")
                    {
                        // Clear the aggregation from the sessions
                        HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;
                        // Redirect the user
                        currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                        HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                        return;
                    }

                    // Save the returned values, depending on the page
                    switch (page)
                    {
                        case 1:
                            Save_Page_1_Postback(form);
                            break;

                        case 2:
                            Save_Page_2_Postback(form);
                            break;

                        case 3:
                            Save_Page_3_Postback(form);
                            break;

                        case 4:
                            Save_Page_4_Postback(form);
                            break;

                        case 5:
                            Save_Page_5_Postback(form);
                            break;

                        case 6:
                            Save_Page_6_Postback(form);
                            break;

                        case 7:
                            Save_Page_7_Postback(form);
                            break;

                        default:
                            break;
                    }

                    // Should this be saved to the database?
                    if (action == "save")
                    {
                        // Save this aggregation information
                        bool successful_save = true;

                        // Save the new configuration file
                        itemAggregation.Write_Configuration_File(  SobekCM_Library_Settings.Base_Design_Location + itemAggregation.objDirectory );

                        // Save to the database
                        itemAggregation.Save_To_Database(null);

                        // Clear the aggregation from the cache
                        MemoryMgmt.Cached_Data_Manager.Remove_Item_Aggregation(itemAggregation.Code, null);

                        // Forward back to the aggregation home page, if this was successful
                        if (successful_save)
                        {
                            // Clear the aggregation from the sessions
                            HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;

                            // Redirect the user
                            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                            HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                        }
                    }
                    else
                    {
                        // Save to the admins session
                        HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = itemAggregation;
                        currentMode.My_Sobek_SubMode = action;
                        HttpContext.Current.Response.Redirect(currentMode.Redirect_URL(), false);
                    }
                }
                catch
                {
                    actionMessage = "Unable to correctly parse postback data.";
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Interfaces' </value>
        public override string Web_Title
        {
            get { return itemAggregation != null ? "Edit " + itemAggregation.ShortName : "Edit Item Aggregation"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aggregation_Single_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
             Tracer.Add_Trace("Aggregation_Single_AdminViewer.Add_HTML_In_Main_Form", "Add hidden field");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_reset\" name=\"admin_aggr_reset\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_save\" name=\"admin_aggr_save\" value=\"\" />");
            Output.WriteLine();

            Tracer.Add_Trace("Aggregation_Single_AdminViewer.Add_HTML_In_Main_Form", "Add the rest of the form");

            Output.WriteLine("<!-- Users_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");

            Output.WriteLine("  <div class=\"SobekHomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <b>Edit all values associated with an item aggregation</b>");
            Output.WriteLine("    <ul>");
            Output.WriteLine("      <li>Enter the new values for this aggreagtion and press the SAVE button when all your edits are complete.</li>");
            Output.WriteLine("      <li>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "admin/singleaggr\" target=\"ADMIN_AGGR_HELP\" >click here to view the help page</a>.</li>");
            Output.WriteLine("     </ul>");
            Output.WriteLine("  </div>");

            Output.WriteLine("  <div class=\"ViewsBrowsesRow\">");

            // Get the redirect URL for the different page tabs on this form 
            string last_mode = currentMode.My_Sobek_SubMode;
            currentMode.My_Sobek_SubMode = String.Empty;                 
            string redirect_url = currentMode.Redirect_URL();
            currentMode.My_Sobek_SubMode = last_mode;

            // Draw all the page tabs for this form
            if (page == 1)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " BASIC INFORMATION " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_aggr_edit_page('a');\">" + Unselected_Tab_Start + " BASIC INFORMATION " + Unselected_Tab_End + "</a>");
            }

            if (page == 2)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " RESULTS " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_aggr_edit_page('b');\">" + Unselected_Tab_Start + " RESULTS " + Unselected_Tab_End + "</a>");
            }

            if (page == 3)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " OAI/PMH " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_aggr_edit_page('c');\">" + Unselected_Tab_Start + " OAI/PMH " + Unselected_Tab_End + "</a>");
            }

            if (page == 4)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " APPEARANCE " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_aggr_edit_page('d');\">" + Unselected_Tab_Start + " APPEARANCE " + Unselected_Tab_End + "</a>");
            }

            if (page == 5)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " STATIC PAGES " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_aggr_edit_page('e');\">" + Unselected_Tab_Start + " STATIC PAGES " + Unselected_Tab_End + "</a>");
            }

            if (page == 6)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " METADATA BROWSE " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_aggr_edit_page('f');\">" + Unselected_Tab_Start + " METADATA BROWSE " + Unselected_Tab_End + "</a>");
            }

            if (page == 7)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " HIGHLIGHTS " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_aggr_edit_page('g');\">" + Unselected_Tab_Start + " HIGHLIGHTS " + Unselected_Tab_End + "</a>");
            }


            Output.WriteLine("  </div>");

            Output.WriteLine("  <div class=\"SobekEditPanel\">");

            // Add the buttons
            currentMode.My_Sobek_SubMode = String.Empty;

            
            if (( page == 5 ) || ( page == 7 ))
            {
                Output.WriteLine("  <table width=\"100%px\"><tr><td width=\"480px\"><span style=\"color: red;\"><strong> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; This beta feature is currently read-only</strong></span></td><td align=\"right\"><a href=\"\" onclick=\"return new_aggr_edit_page('z');\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CANCEL\" /></a> &nbsp; <a href=\"\" onclick=\"return save_aggr_edits();\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Save\" alt=\"Save\" /></a></td><td width=\"20px\">&nbsp;</td></tr></table>");
            }
            else
            {
                Output.WriteLine("  <table width=\"100%px\"><tr><td width=\"480px\">&nbsp;</td><td align=\"right\"><a href=\"\" onclick=\"return new_aggr_edit_page('z');\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CANCEL\" /></a> &nbsp; <a href=\"\" onclick=\"return save_aggr_edits();\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Save\" alt=\"Save\" /></a></td><td width=\"20px\">&nbsp;</td></tr></table>");
            }
            currentMode.My_Sobek_SubMode = last_mode;

            switch (page)
            {
                case 1:
                    Add_Page_1(Output );
                    break;

                case 2:
                    Add_Page_2(Output);
                    break;

                case 3:
                    Add_Page_3(Output);
                    break;

                case 4:
                    Add_Page_4(Output);
                    break;

                case 5:
                    Add_Page_5(Output);
                    break;

                case 6:
                    Add_Page_6(Output);
                    break;

                case 7:
                    Add_Page_7(Output);
                    break;
            }

            // Add the buttons
            currentMode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("  <table width=\"100%px\"><tr><td width=\"480px\">&nbsp;</td><td align=\"right\"><a href=\"\" onclick=\"return new_aggr_edit_page('z');\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CANCEL\" /></a> &nbsp; <a href=\"\" onclick=\"return save_aggr_edits();\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Save\" alt=\"Save\" /></a></td><td width=\"20px\">&nbsp;</td></tr></table>");
            currentMode.My_Sobek_SubMode = last_mode;


            Output.WriteLine("</div>");
            Output.WriteLine("<br />");
        }

        #region Methods to render (and parse) page 1 - Basic Information

        private void Save_Page_1_Postback(NameValueCollection form)
        {
            if (form["admin_aggr_name"] != null) itemAggregation.Name = form["admin_aggr_name"].ToString();
            if (form["admin_aggr_shortname"] != null) itemAggregation.ShortName = form["admin_aggr_shortname"].ToString();
            if (form["admin_aggr_link"] != null) itemAggregation.External_Link = form["admin_aggr_link"].ToString();
            if ( form["admin_aggr_desc"] != null ) itemAggregation.Description = form["admin_aggr_desc"].ToString();
            if (form["admin_aggr_email"] != null) itemAggregation.Contact_Email = form["admin_aggr_email"].ToString();
            if (form["admin_aggr_isactive"] != null) itemAggregation.Is_Active = true; else itemAggregation.Is_Active = false;
            if (form["admin_aggr_ishidden"] != null) itemAggregation.Hidden = true; else itemAggregation.Hidden = false;
            if (form["admin_aggr_heading"] != null)
                itemAggregation.Thematic_Heading_ID = Convert.ToInt32(form["admin_aggr_heading"]);
            if (form["admin_aggr_mapsearch_type"] != null)
                itemAggregation.Map_Search = Convert.ToUInt16(form["admin_aggr_mapsearch_type"]);
            
            // Build the display options string
            StringBuilder displayOptionsBldr = new StringBuilder();
            if (form["admin_aggr_basicsearch"] != null) displayOptionsBldr.Append("B");
            if (form["admin_aggr_advsearch"] != null) displayOptionsBldr.Append("A");
            if (form["admin_aggr_textsearch"] != null) displayOptionsBldr.Append("F");
            if (form["admin_aggr_newspsearch"] != null) displayOptionsBldr.Append("N");
            if (form["admin_aggr_dloctextsearch"] != null) displayOptionsBldr.Append("C");
            if (form["admin_aggr_allitems"] != null) displayOptionsBldr.Append("I");
            if (form["admin_aggr_mapsearch"] != null) displayOptionsBldr.Append("M");
            if (form["admin_aggr_mapbrowse"] != null) displayOptionsBldr.Append("G");
            itemAggregation.Display_Options = displayOptionsBldr.ToString();

            
        }

        private void Add_Page_1( TextWriter Output )
        {
            Output.WriteLine("        <table class=\"popup_table\">");

            // Add line for aggregation code and aggregation type
            Output.WriteLine("          <tr valign=\"middle\" height=\"25px\"><td width=\"120px\">Code:</td>");
            Output.WriteLine("            <td>" + itemAggregation.Code + "</td>");

            // TEMPORARY
            Output.WriteLine("<td></td></tr>");
            Output.WriteLine("          <tr valign=\"middle\" height=\"25px\"><td width=\"120px\">Type:</td>");
            Output.WriteLine("            <td colspan=\"2\">" + itemAggregation.Aggregation_Type + "</td></tr>");

            //Output.WriteLine("            <td width=\"300px\" align=\"right\"><label for=\"admin_aggr_type\">Type:</label> &nbsp; ");
            //Output.Write("<select class=\"admin_aggr_select\" name=\"admin_aggr_type\" id=\"admin_aggr_type\">");
            //if (itemAggregation.Aggregation_Type == String.Empty)
            //    Output.Write("<option value=\"\" selected=\"selected\" ></option>");

            //Output.Write(itemAggregation.Aggregation_Type == "coll"
            //                 ? "<option value=\"coll\" selected=\"selected\" >Collection</option>"
            //                 : "<option value=\"coll\">Collection</option>");

            //Output.Write(itemAggregation.Aggregation_Type == "group"
            //                 ? "<option value=\"group\" selected=\"selected\" >Collection Group</option>"
            //                 : "<option value=\"group\">Collection Group</option>");

            //Output.Write(itemAggregation.Aggregation_Type == "exhibit"
            //                 ? "<option value=\"exhibit\" selected=\"selected\" >Exhibit</option>"
            //                 : "<option value=\"exhibit\">Exhibit</option>");

            //Output.Write(itemAggregation.Aggregation_Type == "inst"
            //                 ? "<option value=\"inst\" selected=\"selected\" >Institution</option>"
            //                 : "<option value=\"inst\">Institution</option>");

            //Output.Write(itemAggregation.Aggregation_Type == "subinst"
            //                 ? "<option value=\"subinst\" selected=\"selected\" >Institutional Division</option>"
            //                 : "<option value=\"subinst\">Institutional Division</option>");

            //Output.Write(itemAggregation.Aggregation_Type == "subcoll"
            //                 ? "<option value=\"subcoll\" selected=\"selected\" >SubCollection</option>"
            //                 : "<option value=\"subcoll\">SubCollection</option>");

            //Output.WriteLine("</select></td></tr>");

            //// Add the parent code(s)
            //Output.WriteLine("          <tr><td>Parent Code(s):</td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_parents\" id=\"admin_aggr_parents\" type=\"text\" readonly=\"readonly\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Parent_Codes) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_parents', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_parents', 'admin_aggr_large_input')\" /></td></tr>");
            Output.WriteLine("          <tr valign=\"middle\" height=\"25px\"><td>Parent Code(s):</td><td colspan=\"2\">" + HttpUtility.HtmlEncode(itemAggregation.Parent_Codes) + "</td></tr>");

            // Add the full name line
            Output.WriteLine("          <tr><td><label for=\"admin_aggr_name\">Name (full):</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_name\" id=\"admin_aggr_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Name) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_name', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_name', 'admin_aggr_large_input')\" /></td></tr>");

            // Add the short name line
            Output.WriteLine("          <tr><td><label for=\"admin_aggr_shortname\">Name (short):</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.ShortName) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_shortname', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_shortname', 'admin_aggr_large_input')\" /></td></tr>");

            // Add the link line
            Output.WriteLine("          <tr><td><label for=\"admin_aggr_link\">External Link:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_link\" id=\"admin_aggr_link\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.External_Link) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_link', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_link', 'admin_aggr_large_input')\" /></td></tr>");

            // Add the description box
            int actual_cols = 75;
            if (currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                actual_cols = 70;
            Output.WriteLine("          <tr valign=\"top\"><td valign=\"top\"><label for=\"admin_aggr_desc\">Description:</label></td><td colspan=\"2\"><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\" class=\"admin_aggr_input\" onfocus=\"javascript:textbox_enter('admin_aggr_desc','admin_aggr_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_desc','admin_aggr_input')\">" + HttpUtility.HtmlEncode(itemAggregation.Description) + "</textarea></td></tr>");

            // Add the aggregation email address
            Output.WriteLine("          <tr><td><label for=\"admin_aggr_email\">Contact Email:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_email\" id=\"admin_aggr_email\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Contact_Email) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_email', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_email', 'admin_aggr_large_input')\" /></td></tr>");


            // Add checkboxes for is active and is hidden
            Output.Write(itemAggregation.Is_Active
                             ? "          <tr height=\"30px\"><td>Behavior:</td><td colspan=\"2\"><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" checked=\"checked\" /> <label for=\"admin_aggr_isactive\">Active?</label> "
                             : "          <tr height=\"30px\"><td>Behavior:</td><td colspan=\"2\"><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" /> <label for=\"admin_aggr_isactive\">Active?</label> ");

            Output.Write("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ");

            Output.Write(itemAggregation.Hidden
                             ? "<input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" checked=\"checked\" /> <label for=\"admin_aggr_ishidden\">Hidden?</label> "
                             : "<input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" /> <label for=\"admin_aggr_ishidden\">Hidden?</label> ");
            Output.WriteLine("</td></tr>");

            // Add the thematic heading 
            Output.Write("          <tr><td><label for=\"admin_aggr_heading\">Thematic Heading:</label></td><td colspan=\"2\">");
            Output.Write("<select class=\"admin_aggr_select_large\" name=\"admin_aggr_heading\" id=\"admin_aggr_heading\">");
            if ( itemAggregation.Thematic_Heading_ID < 0  )
            {
                Output.Write("<option value=\"-1\" selected=\"selected\" ></option>");
            }
            else
            {
                Output.Write("<option value=\"-1\"></option>");
            }
            foreach (Thematic_Heading thisHeading in thematicHeadings)
            {
                if (itemAggregation.Thematic_Heading_ID == thisHeading.ThematicHeadingID)
                {
                    Output.Write("<option value=\"" + thisHeading.ThematicHeadingID + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode( thisHeading.ThemeName ) + "</option>");
                }
                else
                {
                    Output.Write("<option value=\"" + thisHeading.ThematicHeadingID + "\">" + HttpUtility.HtmlEncode(thisHeading.ThemeName) + "</option>");
                }
            }
            Output.WriteLine("</select></td></tr>");

            // Add the top-level display options
            Output.WriteLine("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Display Options:</div></td><td colspan=\"2\"><table>");
            Output.WriteLine("\t<tr>");

            Output.Write("\t\t<td><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_basicsearch\" id=\"admin_aggr_basicsearch\"");
            if (( itemAggregation.Display_Options.IndexOf("B") >= 0 ) || (itemAggregation.Display_Options.IndexOf("D") >= 0 ))
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_basicsearch\">Basic Search</label></td>");

            Output.Write("\t\t<td><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_advsearch\" id=\"admin_aggr_advsearch\"");
            if ( itemAggregation.Display_Options.IndexOf("A") >= 0 )
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_advsearch\">Advanced Search</label></td>");
            
            Output.WriteLine("\t</tr>");
            Output.WriteLine("\t<tr>");

            Output.Write("\t\t<td><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_textsearch\" id=\"admin_aggr_textsearch\"");
            if ( itemAggregation.Display_Options.IndexOf("F") >= 0 ) 
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_textsearch\">Full Text Search</label></td>");

            Output.Write("\t\t<td><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_newspsearch\" id=\"admin_aggr_newspsearch\"");
            if ( itemAggregation.Display_Options.IndexOf("N") >= 0 )
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_newspsearch\">Newspaper Search</label></td>");
            
            Output.WriteLine("\t</tr>");
            Output.WriteLine("\t<tr>");

            Output.Write("\t\t<td><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_dloctextsearch\" id=\"admin_aggr_dloctextsearch\"");
            if ( itemAggregation.Display_Options.IndexOf("C") >= 0 ) 
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_dloctextsearch\">dLOC Full Text Search</label></td>");

            Output.Write("\t\t<td><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_allitems\" id=\"admin_aggr_allitems\"");
            if ( itemAggregation.Display_Options.IndexOf("I") >= 0 )
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_allitems\">All / New Items</label></td>");
            
            Output.WriteLine("\t</tr>");
            Output.WriteLine("\t<tr>");

            Output.Write("\t\t<td><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_mapsearch\" id=\"admin_aggr_mapsearch\"");
            if ( itemAggregation.Display_Options.IndexOf("M") >= 0 ) 
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_mapsearch\">Map Search</label></td>");

            Output.Write("\t\t<td><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_mapbrowse\" id=\"admin_aggr_mapbrowse\"");
            if ( itemAggregation.Display_Options.IndexOf("G") >= 0 )
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_mapbrowse\">Map Browse</label></td>");
            
            Output.WriteLine("\t</tr>");
            Output.WriteLine("</table></td></tr>");


             // Add the map search type
            Output.WriteLine("<tr><td><label for=\"admin_aggr_mapsearch_type\">Map Search Default:</label></div></td><td colspan=\"2\">");
            Output.Write("<select class=\"admin_aggr_select\" name=\"admin_aggr_mapsearch_type\" id=\"admin_aggr_mapsearch_type\">");

            Output.Write(itemAggregation.Map_Search % 100 == 0
                             ? "<option value=\"0\" selected=\"selected\" >World</option>"
                             : "<option value=\"0\">World</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 1
                             ? "<option value=\"1\" selected=\"selected\" >Florida</option>"
                             : "<option value=\"1\">Florida</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 2
                             ? "<option value=\"2\" selected=\"selected\" >United States</option>"
                             : "<option value=\"2\">United States</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 3
                             ? "<option value=\"3\" selected=\"selected\" >North America</option>"
                             : "<option value=\"3\">North America</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 4
                             ? "<option value=\"4\" selected=\"selected\" >Caribbean</option>"
                             : "<option value=\"4\">Caribbean</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 5
                             ? "<option value=\"5\" selected=\"selected\" >South America</option>"
                             : "<option value=\"5\">South America</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 6
                             ? "<option value=\"6\" selected=\"selected\" >Africa</option>"
                             : "<option value=\"6\">Africa</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 7
                             ? "<option value=\"7\" selected=\"selected\" >Europe</option>"
                             : "<option value=\"7\">Europe</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 8
                             ? "<option value=\"8\" selected=\"selected\" >Asia</option>"
                             : "<option value=\"8\">Asia</option>");

            Output.Write(itemAggregation.Map_Search % 100 == 9
                             ? "<option value=\"9\" selected=\"selected\" >Middle East</option>"
                             : "<option value=\"9\">Middle East</option>");

            Output.WriteLine("</select></td></tr>");



            Output.WriteLine("        </table>");
            Output.WriteLine("<br />");

        }

        #endregion

        #region Methods to render (and parse) page 2 - Facets and result views

        private void Save_Page_2_Postback(NameValueCollection form)
        {
            // Reset the facets
            itemAggregation.Clear_Facets();
            if (( form["admin_aggr_facet1"] != null ) && ( Convert.ToInt16( form["admin_aggr_facet1"]) > 0 ))
                itemAggregation.Add_Facet( Convert.ToInt16( form["admin_aggr_facet1"] ));
            if (( form["admin_aggr_facet2"] != null ) && ( Convert.ToInt16( form["admin_aggr_facet2"]) > 0 ))
                itemAggregation.Add_Facet( Convert.ToInt16( form["admin_aggr_facet2"] ));
            if (( form["admin_aggr_facet3"] != null ) && ( Convert.ToInt16( form["admin_aggr_facet3"]) > 0 ))
                itemAggregation.Add_Facet( Convert.ToInt16( form["admin_aggr_facet3"] ));
            if (( form["admin_aggr_facet4"] != null ) && ( Convert.ToInt16( form["admin_aggr_facet4"]) > 0 ))
                itemAggregation.Add_Facet( Convert.ToInt16( form["admin_aggr_facet4"] ));
            if (( form["admin_aggr_facet5"] != null ) && ( Convert.ToInt16( form["admin_aggr_facet5"]) > 0 ))
                itemAggregation.Add_Facet( Convert.ToInt16( form["admin_aggr_facet5"] ));
            if (( form["admin_aggr_facet6"] != null ) && ( Convert.ToInt16( form["admin_aggr_facet6"]) > 0 ))
                itemAggregation.Add_Facet( Convert.ToInt16( form["admin_aggr_facet6"] ));
            if (( form["admin_aggr_facet7"] != null ) && ( Convert.ToInt16( form["admin_aggr_facet7"]) > 0 ))
                itemAggregation.Add_Facet( Convert.ToInt16( form["admin_aggr_facet7"] ));
            if (( form["admin_aggr_facet8"] != null ) && ( Convert.ToInt16( form["admin_aggr_facet8"]) > 0 ))
                itemAggregation.Add_Facet( Convert.ToInt16( form["admin_aggr_facet8"] ));

            // Reset the result views
            itemAggregation.Result_Views.Clear();
            itemAggregation.Default_Result_View = Result_Display_Type_Enum.Default;

            // Add the default result view
            if (form["admin_add_default_view"] != null)
            {
                switch( form["admin_add_default_view"])
                {
                    case "brief":
                        itemAggregation.Default_Result_View = Result_Display_Type_Enum.Brief;
                        itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Brief );
                        break;

                    case "thumbnails":
                        itemAggregation.Default_Result_View = Result_Display_Type_Enum.Thumbnails;
                        itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Thumbnails );
                        break;

                    case "table":
                        itemAggregation.Default_Result_View = Result_Display_Type_Enum.Table;
                        itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Table );
                        break;

                    case "full":
                        itemAggregation.Default_Result_View = Result_Display_Type_Enum.Full_Citation;
                        itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Full_Citation );
                        break;
                }
            }

            // Add the result views
            if ( form["admin_aggr_result_view1"] != null ) add_result_view( form["admin_aggr_result_view1"].ToString() );
            if ( form["admin_aggr_result_view2"] != null ) add_result_view( form["admin_aggr_result_view2"].ToString() );
            if ( form["admin_aggr_result_view3"] != null ) add_result_view( form["admin_aggr_result_view3"].ToString() );
            if ( form["admin_aggr_result_view4"] != null ) add_result_view( form["admin_aggr_result_view4"].ToString() );
            if ( form["admin_aggr_result_view5"] != null ) add_result_view( form["admin_aggr_result_view5"].ToString() );
        }

        private void add_result_view( string Result )
        {
            switch( Result )
                {
                    case "brief":
                        if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Brief ))
                            itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Brief );
                        break;

                    case "thumbnails":
                        if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Thumbnails ))
                            itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Thumbnails );
                        break;

                    case "table":
                        if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Table ))
                            itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Table );
                        break;

                case "full":
                        if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Full_Citation ))
                            itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Full_Citation );
                        break;
                }
        }

        private void Add_Page_2( TextWriter Output )
        {
            Output.WriteLine("<table class=\"popup_table\">");

            // Add the facets
            Output.Write("<tr valign=\"top\"><td width=\"140px\"><div class=\"admin_aggr_label2\">Facets:</div></td><td>");
            for (int i = 0; i < 8; i++)
            {
                short thisFacet = -1;
                if (itemAggregation.Facets.Count > i)
                    thisFacet = itemAggregation.Facets[i];
                Facet_Writer_Helper(Output, thisFacet, i + 1);
                if (i < 7)
                    Output.WriteLine("<br />");
                else
                    Output.WriteLine("</td></tr>");
            }

            // Add some space
            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

            // Add the default result view
            Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Default Result View:</div></td><td>");
            Result_Writer_Helper( Output, "admin_aggr_default_view", "( NO DEFAULT )", itemAggregation.Default_Result_View );
            Output.WriteLine("</td><tr>");

            // Add some space
            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

            // Add all the views
            Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Result Views:</div></td><td>");
            for (int i = 0; i < 5; i++)
            {
                Result_Display_Type_Enum thisResult = Result_Display_Type_Enum.Default;
                if (itemAggregation.Result_Views.Count > i)
                    thisResult = itemAggregation.Result_Views[i];
                Result_Writer_Helper( Output, "admin_aggr_result_view" + ( i + 1 ).ToString(), "", thisResult );

                if (i < 4)
                    Output.WriteLine("<br />");
                else
                    Output.WriteLine("</td></tr>");
            }

            Output.WriteLine("</table>");

        }

        private void Result_Writer_Helper( TextWriter Output, string FieldName, string NoOption, Result_Display_Type_Enum Result_Type )
        {
            // Start the select box
            Output.Write("<select class=\"admin_aggr_select\" name=\"" + FieldName+ "\" id=\"" + FieldName  + "\">");

            // Add the NONE option first
            if (Result_Type ==  Result_Display_Type_Enum.Default )
            {
                Output.Write("<option value=\"\" selected=\"selected\" >" + NoOption + "</option>");
            }
            else
            {
                Output.Write("<option value=\"\">" + NoOption + "</option>");
            }

            // Add each result view to the select boxes ( brief, map, table, thumbnails, full )
            if ( Result_Type == Result_Display_Type_Enum.Brief )
            {
                        Output.Write("<option value=\"brief\" selected=\"selected\" >Brief View</option>");
            }
            else
            {
                        Output.Write("<option value=\"brief\">Brief View</option>");
            }

            if ( Result_Type == Result_Display_Type_Enum.Table )
            {
                        Output.Write("<option value=\"table\" selected=\"selected\" >Table View</option>");
            }
            else
            {
                        Output.Write("<option value=\"table\">Table View</option>");
            }

            if ( Result_Type == Result_Display_Type_Enum.Thumbnails )
            {
                        Output.Write("<option value=\"thumbnails\" selected=\"selected\" >Thumbnail View</option>");
            }
            else
            {
                        Output.Write("<option value=\"thumbnails\">Thumbnail View</option>");
            }

            if ( Result_Type == Result_Display_Type_Enum.Full_Citation )
            {
                        Output.Write("<option value=\"full\" selected=\"selected\" >Full View</option>");
            }
            else
            {
                        Output.Write("<option value=\"full\">Full View</option>");
            }
            Output.Write("</select>");
        }

        private void Facet_Writer_Helper(  TextWriter Output, short FacetID, int FacetCounter )
        {
            // Start the select box
            Output.Write("<select class=\"admin_aggr_select\" name=\"admin_aggr_facet" + FacetCounter + "\" id=\"admin_aggr_facet" + FacetCounter + "\">");

            // Add the NONE option first
            if ( FacetID == - 1)
            {
                Output.Write("<option value=\"-1\" selected=\"selected\" ></option>");
            }
            else
            {
                Output.Write("<option value=\"-1\"></option>");
            }

            // Add each metadata field to the select boxes
            foreach (Metadata_Search_Field metadataField in SobekCM_Library_Settings.Metadata_Search_Fields )
            {
                // Anywhere as -1 is in the list, so leave that out
                if ((metadataField.ID > 0) && (metadataField.Display_Term != "Undefined"))
                {
                    if (metadataField.ID == FacetID)
                    {
                        Output.Write("<option value=\"" + metadataField.ID + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(metadataField.Display_Term) + "</option>");
                    }
                    else
                    {
                        Output.Write("<option value=\"" + metadataField.ID + "\">" + HttpUtility.HtmlEncode(metadataField.Display_Term) + "</option>");
                    }
                }
            }
            Output.Write("</select>");
        }
        #endregion

        #region Methods to render (and parse) page 3 - OAI/PMH

        private void Save_Page_3_Postback(NameValueCollection form)
        {
            if (form["admin_aggr_oai_flag"] != null)
                itemAggregation.OAI_Flag = true;
            else
                itemAggregation.OAI_Flag = false;

            if (form["admin_aggr_oai_metadata"] != null)
                itemAggregation.OAI_Metadata = form["admin_aggr_oai_metadata"].ToString();
        }

        private void Add_Page_3(TextWriter Output)
        {
            Output.WriteLine("<table class=\"popup_table\">");

            //Output.WriteLine("<tr><td width=\"50px\"></td><td></td></tr>");
            
            // Add flag to include this as OAI
            Output.Write("<tr><td colspan=\"2\"><input class=\"admin_aggr_checkbox\" type=\"checkbox\" name=\"admin_aggr_oai_flag\" id=\"admin_aggr_oai_flag\"");
            if ( itemAggregation.OAI_Flag )
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_oai_flag\">Include in OAI-PMH as a set?</label></td></tr>");

            // Add some space
            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

            // Add label for adding metadata to this OAI-SET
            Output.WriteLine("<tr><td colspan=\"2\"><label for=\"admin_aggr_oai_metadata\">Additional dublin core metadata to include in OAI-PMH set list:</label></td></tr>");

            // Add text box for adding metadata to this OAI-SET
            int actual_cols = 95;
            if (currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                actual_cols = 90;
            Output.WriteLine("<tr><td width=\"50px\">&nbsp;</td><td><textarea rows=\"12\" cols=\"" + actual_cols + "\" name=\"admin_aggr_oai_metadata\" id=\"admin_aggr_oai_metadata\" class=\"admin_aggr_input\" onfocus=\"javascript:textbox_enter('admin_aggr_oai_metadata','admin_aggr_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_oai_metadata','admin_aggr_input')\">" + HttpUtility.HtmlEncode(itemAggregation.OAI_Metadata) + "</textarea></td></tr>");

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }

        #endregion

        #region Methods to render (and parse) page 4 -  Appearance

        private void Save_Page_4_Postback(NameValueCollection form)
        {
            // Set the web skin
            itemAggregation.Web_Skins.Clear();
            itemAggregation.Default_Skin = String.Empty;
            if (( form["admin_aggr_skin_1"] != null ) && ( form["admin_aggr_skin_1"].ToString().Length > 0 ))
            {
                itemAggregation.Web_Skins.Add( form["admin_aggr_skin_1"].ToString() );
                itemAggregation.Default_Skin = form["admin_aggr_skin_1"].ToString();
            }

            // Get the front banner specificiations information
            if (form["admin_aggr_frontbanner_height"] != null)
            {
                ushort front_banner_height = 0;
                if (UInt16.TryParse(form["admin_aggr_frontbanner_height"].ToString(), out front_banner_height))
                    itemAggregation.Front_Banner_Height = front_banner_height;
            }
            if (form["admin_aggr_frontbanner_width"] != null)
            {
                ushort front_banner_width = 0;
                if (UInt16.TryParse(form["admin_aggr_frontbanner_width"].ToString(), out front_banner_width))
                    itemAggregation.Front_Banner_Width = front_banner_width;                        
            }
            if ((form["admin_aggr_frontbanner_side"] != null) && (form["admin_aggr_frontbanner_side"].ToString() == "left' "))
                itemAggregation.Front_Banner_Left_Side = true;
            else
                itemAggregation.Front_Banner_Left_Side = false;

            // Clear the front banners and banners
            int front_banner_dictionary_length = itemAggregation.Front_Banner_Dictionary.Count;
            int banner_dictionary_length = itemAggregation.Banner_Dictionary.Count;
           itemAggregation.Clear_Banners();

            // Read the front banners
            for (int i = 0; i < front_banner_dictionary_length + 2; i++)
            {
                read_banner(form, "admin_aggr_frontbanner_" + (i + 1).ToString(), true );
            }

            // Read the standard banners
            for (int i = 0; i < banner_dictionary_length + 2; i++)
            {
                read_banner(form, "admin_aggr_banner_" + (i + 1).ToString(), false);
            }

            // Clear the home dictionary
            int home_page_length = itemAggregation.Home_Page_File_Dictionary.Count;
            itemAggregation.Home_Page_File_Dictionary.Clear();

            // Read the home page source information
            for ( int i = 0 ; i < home_page_length + 2 ; i++ )
            {
                read_home( form, "admin_aggr_home_" + (i+1).ToString());
            }
        }

        private void read_home(NameValueCollection form, string id_prefix)
        {
            Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
            string source = String.Empty;
            if (form[id_prefix + "_lang"] != null)
                language = Web_Language_Enum_Converter.Code_To_Enum(form[id_prefix + "_lang"].ToString());
            if ( form[id_prefix + "_source"] != null )
                source = form[id_prefix + "_source"].ToString();
            if ( source.Length > 0 )
            {
                itemAggregation.Add_Home_Page_File( source, language );
            }
        }

        private void read_banner(NameValueCollection form, string id_prefix, bool front_banner)
        {
            Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
            string source = String.Empty;
            if (form[id_prefix + "_lang"] != null)
                language = Web_Language_Enum_Converter.Code_To_Enum(form[id_prefix + "_lang"].ToString());
            if ( form[id_prefix + "_source"] != null )
                source = form[id_prefix + "_source"].ToString();
            if ( source.Length > 0 )
            {
                if ( !front_banner )
                {
                    itemAggregation.Add_Banner_Image( source, language);
                }
                else
                {
                    itemAggregation.Add_Front_Banner_Image( source, language);
                }
            }
        }

        private void Add_Page_4(TextWriter Output)
        {
            Output.WriteLine("<table class=\"popup_table\">");

            // Get the ordered list of all skin codes
            ReadOnlyCollection<string> skinCodes = webSkins.Ordered_Skin_Codes;
           
            // Add all the web skins
            Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Web Skin:</div></td><td>");
            for (int i = 0; i <1 ; i++ ) // itemAggregation.Web_Skins.Count + 5; i++)
            {
                string skin = String.Empty;
                if (i < itemAggregation.Web_Skins.Count)
                    skin = itemAggregation.Web_Skins[i];
                Skin_Writer_Helper(Output, "admin_aggr_skin_" + (i+1).ToString(), skin, skinCodes );
                if ( (i+1) % 3 == 0 )
                    Output.WriteLine("<br />");
            }
            Output.WriteLine("</td></tr>");

            // Add some space
            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");


             // Add the banner height line
            Output.WriteLine("<tr><td><label for=\"admin_aggr_frontbanner_height\">Front Banner Height:</label></td><td><input class=\"admin_aggr_small_input\" name=\"admin_aggr_frontbanner_height\" id=\"admin_aggr_frontbanner_height\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Front_Banner_Height) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_frontbanner_height', 'admin_aggr_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_frontbanner_height', 'admin_aggr_small_input')\" /></td></tr>");

            // Add the banner width line
            Output.WriteLine("<tr><td><label for=\"admin_aggr_frontbanner_width\">Front Banner Width:</label></td><td><input class=\"admin_aggr_small_input\" name=\"admin_aggr_frontbanner_width\" id=\"admin_aggr_frontbanner_width\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Front_Banner_Width) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_frontbanner_width', 'admin_aggr_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_frontbanner_width', 'admin_aggr_small_input')\" /></td></tr>");

            // Add the radio buttons for the banner side (left or right)
            Output.WriteLine(" <tr><td>Front Banner Side:</td><td><input type=\"radio\" name=\"admin_aggr_frontbanner_side\" id=\"left\" value=\"left\"");
            if ( itemAggregation.Front_Banner_Left_Side )
                Output.Write(" checked=\"checked\"");
            Output.Write("/><label for=\"left\">Left</label> &nbsp; <input type=\"radio\" name=\"admin_aggr_frontbanner_side\" id=\"right\" value=\"right\"");
            if (! itemAggregation.Front_Banner_Left_Side )
                Output.Write(" checked=\"checked\"");
            Output.WriteLine("/><label for=\"right\">Right</label></td></tr>");

            // Add some space
            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

            // Add the front banners
            Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Front Banners:</div></td><td>");
            for (int i = 0; i < itemAggregation.Front_Banner_Dictionary.Count + 2; i++)
            {
                // Find the language and banner source
                Web_Language_Enum language = Web_Language_Enum.DEFAULT; 
                string banner_source = String.Empty;
                if (i < itemAggregation.Front_Banner_Dictionary.Count)
                {
                    language = itemAggregation.Front_Banner_Dictionary.Keys.ElementAt(i);
                    banner_source = itemAggregation.Front_Banner_Dictionary[language];
                }
                else if (i == 0)
                    language = SobekCM_Library_Settings.Default_UI_Language;

                // Add this banner
                Banner_Writer_Helper(Output, "admin_aggr_frontbanner_" + (i + 1).ToString(), language, banner_source);
            }
            Output.WriteLine("</td></tr>");

            // Add some space
            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

            // Add the standard banners
            Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Banners:</div></td><td>");
            for (int i = 0; i < itemAggregation.Banner_Dictionary.Count + 2; i++)
            {
                // Find the language and banner source
                Web_Language_Enum language = Web_Language_Enum.DEFAULT;
                string banner_source = String.Empty;
                if (i < itemAggregation.Banner_Dictionary.Count)
                {
                    language = itemAggregation.Banner_Dictionary.Keys.ElementAt(i);
                    banner_source = itemAggregation.Banner_Dictionary[language];
                }
                else if (i == 0)
                    language = SobekCM_Library_Settings.Default_UI_Language;

                // Add this banner
                Banner_Writer_Helper( Output, "admin_aggr_banner_" + ( i+1).ToString(), language, banner_source );
            }
            Output.WriteLine("</td></tr>");

            // Add some space
            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

            // Add the home source files
            Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Home Page:</div></td><td>");
            for (int i = 0; i < itemAggregation.Home_Page_File_Dictionary.Count + 2; i++)
            {
                // Find the language and banner source
                Web_Language_Enum language = Web_Language_Enum.DEFAULT;
                string source_file = String.Empty;
                if (i < itemAggregation.Home_Page_File_Dictionary.Count)
                {
                    language = itemAggregation.Home_Page_File_Dictionary.Keys.ElementAt(i);
                    source_file = itemAggregation.Home_Page_File_Dictionary[language];
                }
                else if (i == 0)
                    language = SobekCM_Library_Settings.Default_UI_Language;

                // Add this banner
                Home_Writer_Helper(Output, "admin_aggr_home_" + (i + 1).ToString(), language, source_file);
            }
            Output.WriteLine("</td></tr>");

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }


        private void Home_Writer_Helper(TextWriter Output, string HomeID, Web_Language_Enum Language, string Source)
        {
            string directory = (SobekCM_Library_Settings.Base_Design_Location + itemAggregation.objDirectory).ToLower().Replace("/", "\\");

            Output.Write("<select class=\"admin_aggr_select2\" name=\"" + HomeID + "_lang\" id=\"" + HomeID + "_lang\">");

            // Add the blank language, if this is a blank
            if (Language == Web_Language_Enum.DEFAULT)
                Output.Write("<option value=\"\" selected=\"selected\"></option>");

            // Add each language in the combo box
            foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
            {
                if (Language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
                {
                    Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                }
                else
                {
                    Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                }
            }
            Output.WriteLine("</select> <br /> ");

            // Add the text to the text box
            if ((Source.ToLower().IndexOf("http://") < 0) && (Source.IndexOf("<%BASEURL%>") < 0))
            {
                Source = Source.ToLower().Replace("/", "\\").Replace(directory, "");
            }
            Output.Write("<input class=\"admin_aggr_large_input\" name=\"" + HomeID + "_source\" id=\"" + HomeID + "_source\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Source) + "\" onfocus=\"javascript:textbox_enter('" + HomeID + "_source', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('" + HomeID + "_source', 'admin_aggr_large_input')\" /><br /><br />");
        }

        private void Banner_Writer_Helper(TextWriter Output, string BannerID, Web_Language_Enum Language, string Source)
        {
            string directory = (SobekCM_Library_Settings.Base_Design_Location + itemAggregation.objDirectory).ToLower().Replace("/", "\\");

            Output.Write("<select class=\"admin_aggr_select2\" name=\"" + BannerID + "_lang\" id=\"" + BannerID + "_lang\">");

            // Add the blank language, if this is a blank
            if (Language == Web_Language_Enum.DEFAULT)
                Output.Write("<option value=\"\" selected=\"selected\"></option>");

            // Add each language in the combo box
            foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
            {
                if (Language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
                {
                    Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                }
                else
                {
                    Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                }
            }
            Output.WriteLine("</select> <br /> ");

            // Add the text to the text box
            if ((Source.ToLower().IndexOf("http://") < 0) && (Source.IndexOf("<%BASEURL%>") < 0))
            {
                Source = Source.ToLower().Replace("/", "\\").Replace(directory, "");
            }
            Output.Write("<input class=\"admin_aggr_large_input\" name=\"" + BannerID + "_source\" id=\"" + BannerID + "_source\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Source) + "\" onfocus=\"javascript:textbox_enter('" + BannerID + "_source', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('" + BannerID + "_source', 'admin_aggr_large_input')\" /><br /><br />");
        }

        private void Skin_Writer_Helper(TextWriter Output, string SkinID, string Skin, ReadOnlyCollection<string> Skin_Codes )
        {
            // Start the select box
            Output.Write("<select class=\"admin_aggr_select\" name=\"" + SkinID + "\" id=\"" + SkinID + "\">");

            // Add the NONE option first
            if (Skin.Length == 0)
            {
                Output.Write("<option value=\"\" selected=\"selected\" ></option>");
            }
            else
            {
                Output.Write("<option value=\"\"></option>");
            }

            // Add each metadata field to the select boxes
            foreach ( string skinCode in Skin_Codes)
            {
                if (String.Compare(Skin, skinCode, true) == 0)
                {
                    Output.Write("<option value=\"" + skinCode + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(skinCode) + "</option>");
                }
                else
                {
                    Output.Write("<option value=\"" + skinCode + "\">" + HttpUtility.HtmlEncode(skinCode) + "</option>");
                }
            }
            Output.Write("</select>");
        }

        #endregion

        #region Methods to render (and parse) page 5 - Static Pages

        private void Save_Page_5_Postback(NameValueCollection form)
        {

        }

        private void Add_Page_5(TextWriter Output)
        {
            Output.WriteLine("<table class=\"popup_table\">");

            // Determine the maximum number of languages used in tooltips and text
            int max_labels = 0;
            int max_sources = 0;
            List<Item_Aggregation_Browse_Info> browse_infos = new List<Item_Aggregation_Browse_Info>();
            foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Browse_Home_Pages(SobekCM_Library_Settings.Default_UI_Language))
            {
                if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML)
                {
                    max_labels = Math.Max(max_labels, thisBrowse.Label_Dictionary.Count);
                    max_sources = Math.Max(max_sources, thisBrowse.Source_Dictionary.Count);
                    browse_infos.Add(thisBrowse);
                }
            }
            foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Browse_By_Pages(SobekCM_Library_Settings.Default_UI_Language))
            {
                if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML)
                {
                    max_labels = Math.Max(max_labels, thisBrowse.Label_Dictionary.Count);
                    max_sources = Math.Max(max_sources, thisBrowse.Source_Dictionary.Count);
                    browse_infos.Add(thisBrowse);
                }
            }
            foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Info_Pages)
            {
                if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML)
                {
                    max_labels = Math.Max(max_labels, thisBrowse.Label_Dictionary.Count);
                    max_sources = Math.Max(max_sources, thisBrowse.Source_Dictionary.Count);
                    browse_infos.Add(thisBrowse);
                }
            }
            max_labels += 1;
            max_sources += 1;

            // Add each browse and info page
            for (int i = 0; i < browse_infos.Count + 1; i++)
            {
                if (i > 0)
                {
                    // Add some space and a line
                    Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");
                    Output.WriteLine("<tr style=\"background:#333333\"><td colspan=\"2\"></td></tr>");
                    Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");
                }

                // Get the info/browse object to display, or make an empty one
                Item_Aggregation_Browse_Info emptyBrowseInfo = new Item_Aggregation_Browse_Info();
                emptyBrowseInfo.Source = Item_Aggregation_Browse_Info.Source_Type.Static_HTML;
                emptyBrowseInfo.Browse_Type = Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_Home;
                if (i < browse_infos.Count)
                    emptyBrowseInfo = browse_infos[i];

                // Now, add it to the form
                Browse_Writer_Helper(Output, i + 1, emptyBrowseInfo, max_labels, max_sources);
            }

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }


        private void Browse_Writer_Helper(TextWriter Output, int BrowseCounter, Item_Aggregation_Browse_Info thisBrowse, int Max_Labels, int Max_Sources)
        {
            string directory = (SobekCM_Library_Settings.Base_Design_Location + itemAggregation.objDirectory).ToLower().Replace("/", "\\");

            // Add the code line
            Output.WriteLine("<tr><td width=\"120px\"> &nbsp; &nbsp; <label for=\"admin_aggr_browse_code_" + BrowseCounter + "\">Code:</label></td><td><input class=\"admin_aggr_small_input\" name=\"admin_aggr_browse_code_" + BrowseCounter + "\" id=\"admin_aggr_browse_code_" + BrowseCounter + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisBrowse.Code) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_browse_code_" + BrowseCounter + "', 'admin_aggr_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_browse_code_" + BrowseCounter + "', 'admin_aggr_small_input')\" /></td></tr>");

            // Add the type line
            Output.Write("<tr><td> &nbsp; &nbsp; <label for=\"admin_aggr_link_" + BrowseCounter + "\">Type:</label></td><td><select class=\"admin_aggr_select\" name=\"admin_aggr_browse_type_" + BrowseCounter + "\" id=\"admin_aggr_browse_type_" + BrowseCounter + "\">");
            if (thisBrowse.Browse_Type ==  Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_Home)
                Output.Write("<option value=\"browse\" selected=\"selected\">Browse</option>");
            else
                Output.Write("<option value=\"browse\">Browse</option>");

            if (thisBrowse.Browse_Type ==  Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By)
                Output.Write("<option value=\"browseby\" selected=\"selected\">Browse By</option>");
            else
                Output.Write("<option value=\"browseby\">Browse By</option>");

            if (thisBrowse.Browse_Type ==  Item_Aggregation_Browse_Info.Browse_Info_Type.Info)
                Output.Write("<option value=\"info\" selected=\"selected\">Info</option>");
            else
                Output.Write("<option value=\"info\">Info</option>");
            
            Output.WriteLine("</td></tr>");

            // Add lines for the label
            if (Max_Labels == 1)
                Output.Write("<tr><td> &nbsp; &nbsp; Label:</td><td>");
            else
                Output.Write("<tr valign=\"top\"><td><br /> &nbsp; &nbsp; Label:</td><td>");
            for (int j = 0; j < Max_Labels; j++)
            {
                Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
                string text = String.Empty;
                if (j < thisBrowse.Label_Dictionary.Count)
                {
                    language = thisBrowse.Label_Dictionary.Keys.ElementAt(j);
                    text = thisBrowse.Label_Dictionary[language];
                }

                // Start the select box
                string id = "admin_aggr_label_lang_" + BrowseCounter + "_" + (j + 1).ToString();
                string id2 = "admin_aggr_label_" + BrowseCounter + "_" + (j + 1).ToString();
                Output.Write("<select class=\"admin_aggr_select2\" name=\"" + id + "\" id=\"" + id + "\">");

                // Add each language in the combo box
                foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
                {
                    if (language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
                    {
                        Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                    }
                    else
                    {
                        Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                    }
                }
                Output.WriteLine("</select> &nbsp; &nbsp; ");

                // Add the text to the text box
                Output.Write("<input class=\"admin_aggr_medium_input\" name=\"" + id2 + "\" id=\"" + id2 + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text) + "\" onfocus=\"javascript:textbox_enter('" + id2 + "', 'admin_aggr_medium_input_focused')\" onblur=\"javascript:textbox_leave('" + id2 + "', 'admin_aggr_medium_input')\" /><br />");
            }
            Output.WriteLine("</td></tr>");

            // Add sources by language
            if (Max_Sources == 1)
                Output.Write("<tr><td> &nbsp; &nbsp; HTML Source:</td><td>");
            else
                Output.Write("<tr valign=\"top\"><td><br /> &nbsp; &nbsp; HTML Source:</td><td>");
            for (int j = 0; j < Max_Sources; j++)
            {
                Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
                string text = String.Empty;
                if (j < thisBrowse.Source_Dictionary.Count)
                {
                    language = thisBrowse.Source_Dictionary.Keys.ElementAt(j);
                    text = thisBrowse.Source_Dictionary[language];
                }

                // Start the select box
                string id = "admin_aggr_source_lang_" + BrowseCounter + "_" + (j + 1).ToString();
                string id2 = "admin_aggr_source_" + BrowseCounter + "_" + (j + 1).ToString();
                Output.Write("<hr /><select class=\"admin_aggr_select2\" name=\"" + id + "\" id=\"" + id + "\">");

                // Add each language in the combo box
                foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
                {
                    if (language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
                    {
                        Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                    }
                    else
                    {
                        Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                    }
                }
                Output.WriteLine("</select> <br /> ");

                // Add the text to the text box
                if ((text.ToLower().IndexOf("http://") < 0) && (text.IndexOf("<%BASEURL%>") < 0))
                {

                    text = text.ToLower().Replace("/", "\\").Replace(directory, "");
                }
                Output.Write("<input class=\"admin_aggr_large_input\" name=\"" + id2 + "\" id=\"" + id2 + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text) + "\" onfocus=\"javascript:textbox_enter('" + id2 + "', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('" + id2 + "', 'admin_aggr_large_input')\" /><br />");
            }
            Output.WriteLine("</td></tr>");

            // Add a delete option
            Output.WriteLine("<tr><td colspan=\"2\" align=\"right\"><a href=\"\">DELETE THIS PAGE</a></td></tr>");

        }

        #endregion

        #region Methods to render (and parse) page 6 - Metdata Browse

        private void Save_Page_6_Postback(NameValueCollection form)
        {
            // Get the metadata browses
            List<Item_Aggregation_Browse_Info> metadata_browse_bys = new List<Item_Aggregation_Browse_Info>();
            foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Browse_By_Pages(SobekCM_Library_Settings.Default_UI_Language))
            {
                if (thisBrowse.Browse_Type == Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By)
                {
                    if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Database)
                    {
                        metadata_browse_bys.Add(thisBrowse);
                    }
                }
            }

            // Remove all these browse by's
            foreach (Item_Aggregation_Browse_Info browseBy in metadata_browse_bys)
            {
                itemAggregation.Remove_Browse_Info_Page(browseBy);
            }

            // Look for the default browse by
            short default_browseby_id = 0;
            if (form["admin_aggr_default_browseby"] != null)
            {
                string default_browseby = form["admin_aggr_default_browseby"].ToString();
                if (Int16.TryParse(default_browseby, out default_browseby_id))
                {
                    Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(default_browseby_id);
                    if (field != null)
                    {
                        Item_Aggregation_Browse_Info newBrowse = new Item_Aggregation_Browse_Info(Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By, Item_Aggregation_Browse_Info.Source_Type.Database, field.Display_Term, String.Empty, field.Display_Term);
                        itemAggregation.Add_Browse_Info(newBrowse);
                        itemAggregation.Default_BrowseBy = field.Display_Term;
                    }
                }
                else
                {
                    itemAggregation.Default_BrowseBy = default_browseby;
                }
            }

            // Now, get all the new browse bys
            for (int i = 0; i < metadata_browse_bys.Count + 10; i++)
            {
                if (form["admin_aggr_browseby_" + i] != null)
                {
                    short browseby_id = Convert.ToInt16(form["admin_aggr_browseby_" + i]);
                    if ((browseby_id > 0) && (default_browseby_id != browseby_id))
                    {
                        Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(browseby_id);
                        if (field != null)
                        {                               
                            Item_Aggregation_Browse_Info newBrowse = new Item_Aggregation_Browse_Info(Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By, Item_Aggregation_Browse_Info.Source_Type.Database, field.Display_Term, String.Empty, field.Display_Term);
                            itemAggregation.Add_Browse_Info(newBrowse);
                        }
                    }
                }
            }
        }

        private void Add_Page_6(TextWriter Output)
        {
            Output.WriteLine("<table class=\"popup_table\">");

            // Get the metadata browses
            List<string> metadata_browse_bys = new List<string>();
            string default_browse_by = itemAggregation.Default_BrowseBy;
            List<string> otherBrowseBys = new List<string>();
            foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Browse_By_Pages( SobekCM_Library_Settings.Default_UI_Language))
            {
                if (thisBrowse.Browse_Type == Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By)
                {
                    if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Database)
                    {
                        metadata_browse_bys.Add(thisBrowse.Code);
                    }
                    else
                    {
                        otherBrowseBys.Add(thisBrowse.Code);
                    }
                }
            }

            // Get the additional values include
            string[] empty_set = new string[0];

            // Add the default browse by
            Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Default Browse By:</div></td><td>");
            BrowseBy_Writer_Helper( Output, "admin_aggr_default_browseby", "( NO DEFAULT )", default_browse_by, otherBrowseBys.ToArray() );
            Output.WriteLine("</td><tr>");

            // Add some space
            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

            // Add all the browse by's
            Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Metadata Browses:</div></td><td>");
            for (int i = 0; i < metadata_browse_bys.Count + 10 ; i++)
            {
                string browse_by = String.Empty;
                if (i < metadata_browse_bys.Count)
                    browse_by = metadata_browse_bys[i];
                BrowseBy_Writer_Helper(Output, "admin_aggr_browseby_" + i, String.Empty, browse_by, empty_set );
                Output.WriteLine("<br />");
            }

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }

        private void BrowseBy_Writer_Helper( TextWriter Output, string ID, string Default, string Value, string[] otherValues )
        {
            // Start the select box
            Output.Write("<select class=\"admin_aggr_select\" name=\"" + ID + "\" id=\"" + ID + "\">");

            // Add the NONE option first
            if (Value.Length == 0 )
            {
                Output.Write("<option value=\"-1\" selected=\"selected\" >" + Default + "</option>");
            }
            else
            {
                Output.Write("<option value=\"-1\">" + Default + "</option>");
            }

            // Add any other values
            foreach (string thisOtherValue in otherValues)
            {
                if (String.Equals(thisOtherValue, Value, StringComparison.OrdinalIgnoreCase))
                {
                    Output.Write("<option value=\"" + thisOtherValue + "\" selected=\"selected\" >" + thisOtherValue + "</option>");
                }
                else
                {
                    Output.Write("<option value=\"" + thisOtherValue + "\">" + thisOtherValue + "</option>");
                }
            }

            // Add each metadata field to the select boxes
            foreach (Metadata_Search_Field metadataField in SobekCM_Library_Settings.Metadata_Search_Fields)
            {
                // Anywhere as -1 is in the list, so leave that out
                if ((metadataField.ID > 0 ) && (metadataField.Display_Term != "Undefined"))
                {
                    if ( String.Equals(metadataField.Display_Term, Value, StringComparison.OrdinalIgnoreCase ))
                    {
                        Output.Write("<option value=\"" + metadataField.ID + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(metadataField.Display_Term) + "</option>");
                    }
                    else
                    {
                        Output.Write("<option value=\"" + metadataField.ID + "\">" + HttpUtility.HtmlEncode(metadataField.Display_Term) + "</option>");
                    }
                }
            }
            Output.Write("</select>");
        }

        #endregion

        #region Methods to render (and parse) page 7 -  Highlights

        private void Save_Page_7_Postback(NameValueCollection form)
        {

        }

        private void Add_Page_7(TextWriter Output)
        {
            Output.WriteLine("<table class=\"popup_table\">");

            // Add the highlight type
            Output.Write("<tr><td width=\"120px\">Highlights Type:</td><td><input type=\"radio\" name=\"admin_aggr_highlight_type\" id=\"rotating\" value=\"rotating\"");
            if ( itemAggregation.Rotating_Highlights )
                Output.Write(" checked=\"checked\"");
            Output.Write("/><label for=\"rotating\">Rotating</label> &nbsp; <input type=\"radio\" name=\"admin_aggr_highlight_type\" id=\"static\" value=\"static\"");
            if (! itemAggregation.Rotating_Highlights )
                Output.Write(" checked=\"checked\"");
            Output.WriteLine("/><label for=\"static\">Static</label></td></tr>");

            // Determine the maximum number of languages used in tooltips and text
            int max_tooltips = 0;
            int max_text = 0;
            foreach (Item_Aggregation_Highlights thisHighlight in itemAggregation.Highlights)
            {
                max_tooltips = Math.Max(max_tooltips, thisHighlight.Tooltip_Dictionary.Count);
                max_text = Math.Max(max_text, thisHighlight.Text_Dictionary.Count);
            }
            max_tooltips += 1;
            max_text += 1;
            
            // Add each highlight
            for (int i = 0; i < itemAggregation.Highlights.Count + 5; i++)
            {
                // Add some space and a line
                Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");
                Output.WriteLine("<tr style=\"background:#333333\"><td colspan=\"2\"></td></tr>");
                Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

                // Either get the highlight, or just make one
                Item_Aggregation_Highlights emptyHighlight = new Item_Aggregation_Highlights();
                if (i < itemAggregation.Highlights.Count)
                    emptyHighlight = itemAggregation.Highlights[i];

                // Now, add it to the form
                Highlight_Writer_Helper(Output, i + 1, emptyHighlight, max_text, max_tooltips);
            }

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }

        private void Highlight_Writer_Helper( TextWriter Output, int HighlightCounter, Item_Aggregation_Highlights Highlight, int Max_Text, int Max_Tooltips )
        {
            // Add the image line
            Output.WriteLine("<tr><td> &nbsp; &nbsp; <label for=\"admin_aggr_image_" + HighlightCounter + "\">Image:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_image_" + HighlightCounter + "\" id=\"admin_aggr_image_" + HighlightCounter + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Highlight.Image) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input')\" /></td></tr>");

            // Add the link line
            Output.WriteLine("<tr><td> &nbsp; &nbsp; <label for=\"admin_aggr_link_" + HighlightCounter + "\">Link:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_link_" + HighlightCounter + "\" id=\"admin_aggr_image_" + HighlightCounter + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Highlight.Link) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_link_" + HighlightCounter + "', 'admin_aggr_large_input')\" /></td></tr>");

            // Add lines for the text
            if ( Max_Text == 1 )
                Output.Write("<tr><td> &nbsp; &nbsp; Text:</td><td>");
            else
                Output.Write("<tr valign=\"top\"><td><br /> &nbsp; &nbsp; Text:</td><td>");
            for (int j = 0; j < Max_Text; j++)
            {
                Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
                string text = String.Empty;
                if (j < Highlight.Text_Dictionary.Count)
                {
                    language = Highlight.Text_Dictionary.Keys.ElementAt(j);
                    text = Highlight.Text_Dictionary[language];
                }

                // Start the select box
                string id = "admin_aggr_text_lang_" + HighlightCounter + "_" + (j + 1).ToString();
                string id2 = "admin_aggr_text_" + HighlightCounter + "_" + (j + 1).ToString();
                Output.Write("<select class=\"admin_aggr_select2\" name=\"" + id + "\" id=\"" + id + "\">");

                // Add each language in the combo box
                foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array )
                {
                        if ( language == Web_Language_Enum_Converter.Code_To_Enum( possible_language ))
                        {
                            Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                        }
                        else
                        {
                            Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                        }
                }
                Output.WriteLine("</select> &nbsp; &nbsp; ");

                // Add the text to the text box
                Output.Write("<input class=\"admin_aggr_medium_input\" name=\"" + id2 + "\" id=\"" + id2 + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text) + "\" onfocus=\"javascript:textbox_enter('" + id2 + "', 'admin_aggr_medium_input_focused')\" onblur=\"javascript:textbox_leave('" + id2 + "', 'admin_aggr_medium_input')\" /><br />");
            }
            Output.WriteLine("</td></tr>");

            // Add lines for the tooltips
            if (Max_Tooltips == 1)
                Output.Write("<tr><td> &nbsp; &nbsp; Tooltip:</td><td>");
            else
                Output.Write("<tr valign=\"top\"><td><br /> &nbsp; &nbsp; Tooltip:</td><td>");
            for (int j = 0; j < Max_Tooltips; j++)
            {
                Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
                string text = String.Empty;
                if (j < Highlight.Tooltip_Dictionary.Count)
                {
                    language = Highlight.Tooltip_Dictionary.Keys.ElementAt(j);
                    text = Highlight.Tooltip_Dictionary[language];
                }

                // Start the select box
                string id = "admin_aggr_tooltip_lang_" + HighlightCounter + "_" + (j + 1).ToString();
                string id2 = "admin_aggr_tooltip_" + HighlightCounter + "_" + (j + 1).ToString();
                Output.Write("<select class=\"admin_aggr_select2\" name=\"" + id + "\" id=\"" + id + "\">");

                // Add each language in the combo box
                foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
                {
                    if (language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
                    {
                        Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                    }
                    else
                    {
                        Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
                    }
                }
                Output.WriteLine("</select> &nbsp; &nbsp; ");

                // Add the text to the text box
                Output.Write("<input class=\"admin_aggr_medium_input\" name=\"" + id2 + "\" id=\"" + id2 + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text) + "\" onfocus=\"javascript:textbox_enter('" + id2 + "', 'admin_aggr_medium_input_focused')\" onblur=\"javascript:textbox_leave('" + id2 + "', 'admin_aggr_medium_input')\" /><br />");
            }
            Output.WriteLine("</td></tr>");

            // Add a delete option
            Output.WriteLine("<tr><td colspan=\"2\" align=\"right\"><a href=\"\">DELETE THIS HIGHLIGHT</a></td></tr>");

        }

        #endregion
    }
}
