#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit the thematic headings used to organize all the highlighted collectons on the library home page </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to show the thematic headings active in this digital library</li>
    /// </ul></remarks>
    public class Thematic_Headings_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly List<Thematic_Heading> thematicHeadings;

        /// <summary> Constructor for a new instance of the Thematic_Headings_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request </param>
        /// <param name="Thematic_Headings"> Headings under which all the highlighted collections on the home page are organized </param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling an edit or new thematic heading is handled here in the constructor </remarks>
        public Thematic_Headings_AdminViewer(User_Object User,
            SobekCM_Navigation_Object Current_Mode,
            List<Thematic_Heading> Thematic_Headings,
			Aggregation_Code_Manager Code_Manager,
            Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Thematic_Headings_AdminViewer.Constructor", String.Empty);

            // Get the current list of thematic headings
            thematicHeadings = Thematic_Headings;

            // Save the mode 
            currentMode = Current_Mode;

            // Set action message to nothing to start
            actionMessage = String.Empty;

            // If the user cannot edit this, go back
            if ((!user.Is_System_Admin) && ( !user.Is_Portal_Admin ))
            {
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
                return;
            }

            // Handle any post backs
            if (currentMode.isPostBack)
            {
                try
                {
                    // Pull the standard values from the form
                    NameValueCollection form = HttpContext.Current.Request.Form;
                    string save_value = form["admin_heading_tosave"];
                    string action_value = form["admin_heading_action"];

                    // Switch, depending on the request
                    if (action_value != null)
                    {
                        switch (action_value.Trim().ToLower())
                        {
                            case "edit":
                                string new_name = form["form_heading_name"];
                                if (new_name != null)
                                {
                                    int id = Convert.ToInt32(save_value);
                                    int order = 1 + Thematic_Headings.TakeWhile(thisHeading => thisHeading.ThematicHeadingID != id).Count();
                                    if (SobekCM_Database.Edit_Thematic_Heading(id, order, new_name, Tracer) < 1)
                                    {
                                        actionMessage = "Unable to edit existing thematic heading";
                                    }
                                    else
                                    {
                                        // For thread safety, lock the thematic headings list
                                        lock (Thematic_Headings)
                                        {
                                            // Repopulate the thematic headings list
                                            SobekCM_Database.Populate_Thematic_Headings(Thematic_Headings, Tracer);
                                        }

                                        actionMessage = "Thematic heading edits saved";
                                    }
                                }
                                break;

                            case "delete":
                                int thematicDeleteId = Convert.ToInt32(save_value);
                                if (!SobekCM_Database.Delete_Thematic_Heading(thematicDeleteId, Tracer))
                                {
                                    // Set action message
                                    actionMessage = "Unable to delete existing thematic heading";
                                }
                                else
                                {
                                    // For thread safety, lock the thematic headings list
                                    lock (Thematic_Headings)
                                    {
                                        // Remove this thematic heading from the list
                                        int i = 0;
                                        while (i < Thematic_Headings.Count)
                                        {
                                            if (Thematic_Headings[i].ThematicHeadingID == thematicDeleteId)
                                                Thematic_Headings.RemoveAt(i);
                                            else
                                                i++;
                                        }
                                    }

                                    // Set action message
                                    actionMessage = "Thematic heading deleted";
                                }
                                break;


                            case "new":
                                int new_order = Thematic_Headings.Count + 1;
		                        int newid = SobekCM_Database.Edit_Thematic_Heading(-1, new_order, save_value, Tracer);
								if ( newid  < 1)
                                    actionMessage = "Unable to save new thematic heading";
                                else
                                {
                                    // For thread safety, lock the thematic headings list
                                    lock (Thematic_Headings)
                                    {
                                        // Repopulate the thematic headings list
                                        SobekCM_Database.Populate_Thematic_Headings(Thematic_Headings, Tracer);
                                    }

									// Add this blank thematic heading to the code manager
	                                Code_Manager.Add_Blank_Thematic_Heading(newid);

                                    actionMessage = "New thematic heading saved";
                                }
                                break;

                            case "moveup":
                                string[] moveup_split = save_value.Split(",".ToCharArray());
                                int moveup_id = Convert.ToInt32(moveup_split[0]);
                                int moveup_order = Convert.ToInt32(moveup_split[1]);
                                if (moveup_order > 1)
                                {
                                    Thematic_Heading themeHeading = Thematic_Headings[moveup_order - 1];
                                    if (themeHeading.ThematicHeadingID == moveup_id)
                                    {
                                        // For thread safety, lock the thematic headings list
                                        lock (Thematic_Headings)
                                        {
                                            // Move this thematic heading up
                                            Thematic_Headings.Remove(themeHeading);
                                            Thematic_Headings.Insert(moveup_order - 2, themeHeading);
                                            int current_order = 1;
                                            foreach (Thematic_Heading thisTheme in Thematic_Headings)
                                            {
                                                SobekCM_Database.Edit_Thematic_Heading(thisTheme.ThematicHeadingID, current_order, thisTheme.ThemeName, Tracer);
                                                current_order++;
                                            }

                                            // Repopulate the thematic headings list
                                            SobekCM_Database.Populate_Thematic_Headings(Thematic_Headings, Tracer);
                                        }
                                    }
                                }
                                break;

                            case "movedown":
                                string[] movedown_split = save_value.Split(",".ToCharArray());
                                int movedown_id = Convert.ToInt32(movedown_split[0]);
                                int movedown_order = Convert.ToInt32(movedown_split[1]);
                                if (movedown_order < Thematic_Headings.Count)
                                {
                                    Thematic_Heading themeHeading = Thematic_Headings[movedown_order - 1];
                                    if (themeHeading.ThematicHeadingID == movedown_id)
                                    {
                                        // For thread safety, lock the thematic headings list
                                        lock (Thematic_Headings)
                                        {
                                            // Move this thematic heading down
                                            Thematic_Headings.Remove(themeHeading);
                                            Thematic_Headings.Insert(movedown_order, themeHeading);
                                            int current_order = 1;
                                            foreach (Thematic_Heading thisTheme in Thematic_Headings)
                                            {
                                                SobekCM_Database.Edit_Thematic_Heading(thisTheme.ThematicHeadingID, current_order, thisTheme.ThemeName, Tracer);
                                                current_order++;
                                            }

                                            // Repopulate the thematic headings list
                                            SobekCM_Database.Populate_Thematic_Headings(Thematic_Headings, Tracer);
                                        }
                                    }
                                }
                                break;

                        }
                    }
                }
                catch (Exception)
                {
                    actionMessage = "Unknown error caught while handling your reqeust";
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Thematic Headings' </value>
        public override string Web_Title
        {
            get { return "Thematic Headings"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the themes list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Thematic_Headings_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Thematic_Headings_AdminViewer.Add_HTML_In_Main_Form", "Add any popup divisions for form elements");

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_heading_action\" name=\"admin_heading_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_heading_tosave\" name=\"admin_heading_tosave\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- Thematic Heading Edit Form -->");
            Output.WriteLine("<div class=\"admin_heading_popup_div\" id=\"form_heading\" style=\"display:none;\">");
            Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT THEMATIC HEADING</td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"heading_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"popup_table\">");

            // Add line for heading
            Output.WriteLine("    <tr><td width=\"80px\"><label for=\"form_heading_name\">Heading:</label></td><td colspan=\"2\"><input class=\"admin_heading_input\" name=\"form_heading_name\" id=\"form_heading_name\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_heading_name', 'admin_heading_input_focused')\" onblur=\"javascript:textbox_leave('form_heading_name', 'admin_heading_input')\" /></td></tr>");

            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <center><a href=\"\" onclick=\"return heading_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center>");
            Output.WriteLine("</div>");

            Tracer.Add_Trace("Thematic_Headings_AdminViewer.Add_HTML_In_Main_Form", "Write the rest of the form ");

            Output.WriteLine("<!-- Thematic_Headings_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");

            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    Thematic headings are the headings on the main library home page, under which all the item ");
            Output.WriteLine("    aggregation icons appear.<br /><br />");
            Output.WriteLine("    For more information about thematic headings, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/headings\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.");
            Output.WriteLine("  </blockquote>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">New Thematic Heading</span>");
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("      <div class=\"admin_heading_new_div\">");
            Output.WriteLine("        <table class=\"popup_table\">");
            Output.WriteLine("          <tr><td><label for=\"admin_heading_name\">Heading:</label></td><td><input class=\"admin_heading_input\" name=\"admin_heading_name\" id=\"admin_heading_name\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_heading_name', 'admin_heading_input_focused')\" onblur=\"javascript:textbox_leave('admin_heading_name', 'admin_heading_input')\" /></td>");
            Output.WriteLine("          <td><input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\" onclick=\"return save_new_heading();\"/></td></tr>");
            Output.WriteLine("        </table>");
            Output.WriteLine("      </div>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("    <br />");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Thematic Headings</span>");
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"90px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
            Output.WriteLine("    <th width=\"120px\" align=\"center\"><span style=\"color: White\">REORDER</span></th>");
            Output.WriteLine("    <th width=\"350px\" align=\"left\"><span style=\"color: White\">THEMATIC HEADING</span></th>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            // Write the data for each interface
            int current_order = 1;
            foreach (Thematic_Heading thisTheme in thematicHeadings)
            {
                // Build the action links
                Output.WriteLine("  <tr align=\"left\" >");
                Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                Output.Write("<a title=\"Click to edit\" id=\"VIEW_" + thisTheme.ThematicHeadingID + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return heading_form_popup('VIEW_" + thisTheme.ThematicHeadingID + "', '" + thisTheme.ThemeName + "','" + thisTheme.ThematicHeadingID + "');\">edit</a> | ");
                Output.Write("<a title=\"Delete this thematic heading\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_heading('" + thisTheme.ThematicHeadingID + "', '" + thisTheme.ThemeName.Replace("\'", "") + "');\">delete</a> )</td>");

                Output.Write("    <td class=\"SobekAdminActionLink\" align=\"center\" >( ");
                Output.Write("<a title=\"Move this heading up in the order\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return move_heading_up('" + thisTheme.ThematicHeadingID + "', '" + current_order + "');\">up</a> | ");
                Output.Write("<a title=\"Move this heading down in the order\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return move_heading_down('" + thisTheme.ThematicHeadingID + "', '" + current_order + "');\">down</a> )</td>");

                // Add the rest of the row with data
                Output.WriteLine("    <td>" + thisTheme.ThemeName + "</td>");
                Output.WriteLine("   </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

                current_order++;
            }

            Output.WriteLine("</table>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("    <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}
