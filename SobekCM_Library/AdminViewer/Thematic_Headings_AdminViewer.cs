// HTML5 = 10/12/2013 MVS

#region Using directives

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit the thematic headings used to organize all the highlighted collectons on the library home page </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to show the thematic headings active in this digital library</li>
    /// </ul></remarks>
    public class Thematic_Headings_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;

        /// <summary> Constructor for a new instance of the Thematic_Headings_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new thematic heading is handled here in the constructor </remarks>
        public Thematic_Headings_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Thematic_Headings_AdminViewer.Constructor", String.Empty);

            // Set action message to nothing to start
            actionMessage = String.Empty;

            // If the RequestSpecificValues.Current_User cannot edit this, go back
            if ((!RequestSpecificValues.Current_User.Is_System_Admin) && ( !RequestSpecificValues.Current_User.Is_Portal_Admin ))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Handle any post backs
            if (RequestSpecificValues.Current_Mode.isPostBack)
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
                                    int order = 1 + UI_ApplicationCache_Gateway.Thematic_Headings.TakeWhile(ThisHeading => ThisHeading.ID != id).Count();
                                    if (Engine_Database.Edit_Thematic_Heading(id, order, new_name, RequestSpecificValues.Tracer) < 1)
                                    {
                                        actionMessage = "Unable to edit existing thematic heading";
                                    }
                                    else
                                    {
                                        // For thread safety, lock the thematic headings list
                                        Engine_ApplicationCache_Gateway.RefreshThematicHeadings();

                                        actionMessage = "Thematic heading edits saved";
                                    }
                                }
                                break;

                            case "delete":
                                int thematicDeleteId = Convert.ToInt32(save_value);
                                if (!Engine_Database.Delete_Thematic_Heading(thematicDeleteId, RequestSpecificValues.Tracer))
                                {
                                    // Set action message
                                    actionMessage = "Unable to delete existing thematic heading";
                                }
                                else
                                {
                                    // For thread safety, lock the thematic headings list
                                    lock (UI_ApplicationCache_Gateway.Thematic_Headings)
                                    {
                                        // Remove this thematic heading from the list
                                        int i = 0;
                                        while (i < UI_ApplicationCache_Gateway.Thematic_Headings.Count)
                                        {
                                            if (UI_ApplicationCache_Gateway.Thematic_Headings[i].ID == thematicDeleteId)
                                                UI_ApplicationCache_Gateway.Thematic_Headings.RemoveAt(i);
                                            else
                                                i++;
                                        }
                                    }

                                    // Set action message
                                    actionMessage = "Thematic heading deleted";
                                }
                                break;


                            case "new":
                                int new_order = UI_ApplicationCache_Gateway.Thematic_Headings.Count + 1;
                                int newid = Engine_Database.Edit_Thematic_Heading(-1, new_order, save_value, RequestSpecificValues.Tracer);
								if ( newid  < 1)
                                    actionMessage = "Unable to save new thematic heading";
                                else
                                {
                                    // For thread safety, lock the thematic headings list
                                    lock (UI_ApplicationCache_Gateway.Thematic_Headings)
                                    {
                                        // Repopulate the thematic headings list
                                        Engine_Database.Populate_Thematic_Headings(UI_ApplicationCache_Gateway.Thematic_Headings, RequestSpecificValues.Tracer);
                                    }

									// Add this blank thematic heading to the code manager
	                                UI_ApplicationCache_Gateway.Aggregations.Add_Blank_Thematic_Heading(newid);

                                    actionMessage = "New thematic heading saved";
                                }
                                break;

                            case "moveup":
                                string[] moveup_split = save_value.Split(",".ToCharArray());
                                int moveup_id = Convert.ToInt32(moveup_split[0]);
                                int moveup_order = Convert.ToInt32(moveup_split[1]);
                                if (moveup_order > 1)
                                {
                                    Thematic_Heading themeHeading = UI_ApplicationCache_Gateway.Thematic_Headings[moveup_order - 1];
                                    if (themeHeading.ID == moveup_id)
                                    {
                                        // For thread safety, lock the thematic headings list
                                        lock (UI_ApplicationCache_Gateway.Thematic_Headings)
                                        {
                                            // Move this thematic heading up
                                            UI_ApplicationCache_Gateway.Thematic_Headings.Remove(themeHeading);
                                            UI_ApplicationCache_Gateway.Thematic_Headings.Insert(moveup_order - 2, themeHeading);
                                            int current_order = 1;
                                            foreach (Thematic_Heading thisTheme in UI_ApplicationCache_Gateway.Thematic_Headings)
                                            {
                                                Engine_Database.Edit_Thematic_Heading(thisTheme.ID, current_order, thisTheme.Text, RequestSpecificValues.Tracer);
                                                current_order++;
                                            }

                                            // Repopulate the thematic headings list
                                            Engine_Database.Populate_Thematic_Headings(UI_ApplicationCache_Gateway.Thematic_Headings, RequestSpecificValues.Tracer);
                                        }
                                    }
                                }
                                break;

                            case "movedown":
                                string[] movedown_split = save_value.Split(",".ToCharArray());
                                int movedown_id = Convert.ToInt32(movedown_split[0]);
                                int movedown_order = Convert.ToInt32(movedown_split[1]);
                                if (movedown_order < UI_ApplicationCache_Gateway.Thematic_Headings.Count)
                                {
                                    Thematic_Heading themeHeading = UI_ApplicationCache_Gateway.Thematic_Headings[movedown_order - 1];
                                    if (themeHeading.ID == movedown_id)
                                    {
                                        // For thread safety, lock the thematic headings list
                                        lock (UI_ApplicationCache_Gateway.Thematic_Headings)
                                        {
                                            // Move this thematic heading down
                                            UI_ApplicationCache_Gateway.Thematic_Headings.Remove(themeHeading);
                                            UI_ApplicationCache_Gateway.Thematic_Headings.Insert(movedown_order, themeHeading);
                                            int current_order = 1;
                                            foreach (Thematic_Heading thisTheme in UI_ApplicationCache_Gateway.Thematic_Headings)
                                            {
                                                Engine_Database.Edit_Thematic_Heading(thisTheme.ID, current_order, thisTheme.Text, RequestSpecificValues.Tracer);
                                                current_order++;
                                            }

                                            // Repopulate the thematic headings list
                                            Engine_Database.Populate_Thematic_Headings(UI_ApplicationCache_Gateway.Thematic_Headings, RequestSpecificValues.Tracer);
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


        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.Thematic_Heading_Img; }
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
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Thematic_Headings_AdminViewer.Write_ItemNavForm_Closing", "Add any popup divisions for form elements");

			Output.WriteLine("<!-- Thematic_Headings_AdminViewer.Write_ItemNavForm_Closing -->");
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Ui_1_10_3_Custom_Js + "\"></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_heading_action\" name=\"admin_heading_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_heading_tosave\" name=\"admin_heading_tosave\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- Thematic Heading Edit Form -->");
			Output.WriteLine("<div class=\"sbkThav_PopupDiv\" id=\"form_heading\" style=\"display:none;\">");
			Output.WriteLine("  <div class=\"sbkAdm_PopupTitle\"><table style=\"width:100%;\"><tr><td style=\"text-align: left;\">EDIT THEMATIC HEADING</td><td style=\"text-align:right;\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"heading_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
			Output.WriteLine("  <table class=\"sbkAdm_PopupTable\">");

            // Add line for heading
			Output.WriteLine("    <tr><td style=\"width: 80px;\"><label for=\"form_heading_name\">Heading:</label></td><td colspan=\"2\"><input class=\"sbkThav_input sbkAdmin_Focusable\" name=\"form_heading_name\" id=\"form_heading_name\" type=\"text\" value=\"\" /></td></tr>");

			// Add the buttons
			Output.WriteLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			Output.WriteLine("      <td colspan=\"2\">");
			Output.WriteLine("        <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return heading_form_close();\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button title=\"Save changes to this existing thematic heading\" class=\"sbkAdm_RoundButton\" type=\"submit\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button> &nbsp; &nbsp; ");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");
			Output.WriteLine("  </table>");
			Output.WriteLine("</div>");
			Output.WriteLine();


            Tracer.Add_Trace("Thematic_Headings_AdminViewer.Write_ItemNavForm_Closing", "Write the rest of the form ");

            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (actionMessage.Length > 0)
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\">" + actionMessage + "</div>");
			}

            Output.WriteLine("  <p>Thematic headings are the headings on the main library home page, under which all the item aggregation icons appear.</p>");
            Output.WriteLine("  <p>For more information about thematic headings, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/headings\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");
            Output.WriteLine();

            Output.WriteLine("  <h2>New Thematic Heading</h2>");
			Output.WriteLine("  <div class=\"sbkThav_NewDiv\">");
			Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");
	        Output.WriteLine("      <tr>");
	        Output.WriteLine("        <td><label for=\"admin_heading_name\">Heading:</label></td>");
			Output.WriteLine("        <td><input class=\"sbkThav_input sbkAdmin_Focusable\" name=\"admin_heading_name\" id=\"admin_heading_name\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("        <td><button title=\"Save new thematic heading\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_heading();\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td>");
			Output.WriteLine("      </tr>");
            Output.WriteLine("    </table>");
            Output.WriteLine("  </div>");
            Output.WriteLine("  <br />");

            Output.WriteLine("  <h2>Existing Thematic Headings</h2>");
			Output.WriteLine("  <table class=\"sbkThav_Table sbkAdm_Table\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkThav_TableHeader1\">ACTIONS</th>");
			Output.WriteLine("      <th class=\"sbkThav_TableHeader2\">REORDER</th>");
			Output.WriteLine("      <th class=\"sbkThav_TableHeader3\">THEMATIC HEADING</th>");
            Output.WriteLine("     </tr>");
			Output.WriteLine("     <tr><td class=\"sbkAdm_TableRule\" colspan=\"3\"></td></tr>");

            // Write the data for each interface
            int current_order = 1;
            foreach (Thematic_Heading thisTheme in UI_ApplicationCache_Gateway.Thematic_Headings)
            {
                // Build the action links
                Output.WriteLine("    <tr style=\"text-align:left;\" >");
				Output.Write("      <td class=\"sbkAdm_ActionLink\" >( ");
                Output.Write("<a title=\"Click to edit\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return heading_form_popup('" + thisTheme.Text + "','" + thisTheme.ID + "');\">edit</a> | ");
                Output.WriteLine("<a title=\"Delete this thematic heading\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_heading('" + thisTheme.ID + "', '" + thisTheme.Text.Replace("\'", "") + "');\">delete</a> )</td>");

				Output.Write("      <td class=\"sbkAdm_ActionLink\">( ");
                Output.Write("<a title=\"Move this heading up in the order\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return move_heading_up('" + thisTheme.ID + "', '" + current_order + "');\">up</a> | ");
                Output.WriteLine("<a title=\"Move this heading down in the order\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return move_heading_down('" + thisTheme.ID + "', '" + current_order + "');\">down</a> )</td>");

                // Add the rest of the row with data
                Output.WriteLine("      <td>" + thisTheme.Text + "</td>");
                Output.WriteLine("    </tr>");
				Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"3\"></td></tr>");

                current_order++;
            }

            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}
