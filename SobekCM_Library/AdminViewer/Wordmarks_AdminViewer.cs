#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit existing interfaces, and add new interfaces </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to manage the HTML interfaces in this digital library</li>
    /// </ul></remarks>
    public class Wordmarks_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;

        #region Constructor

        /// <summary> Constructor for a new instance of the Wordmarks_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from editing an existing wordmark, deleting a wordmark, or creating a new wordmark is handled here in the constructor </remarks>
        public Wordmarks_AdminViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Wordmarks_AdminViewer.Constructor", String.Empty);

            // Save the mode and settings  here
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

            // If this is a postback, handle any events first
            if (currentMode.isPostBack)
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    string delete_value = form["admin_wordmark_code_delete"].ToUpper().Trim();
                    string save_value = form["admin_wordmark_code_tosave"].ToUpper().Trim();
                    string new_wordmark_code = form["admin_wordmark_code"].ToUpper().Trim();

                    // Was this a reset request?
                    if (delete_value.Length > 0)
                    {
                        Tracer.Add_Trace("Wordmarks_AdminViewer.Constructor", "Delete wordmark '" + delete_value + "'");
                        if (SobekCM_Database.Delete_Icon(delete_value, Tracer))
                        {
                            actionMessage = "Deleted wordmark <i>" + delete_value + "</i>";
                        }
                        else
                        {
                            if (SobekCM_Database.Last_Exception == null)
                            {
                                actionMessage = "Unable to delete wordmark <i>" + delete_value + "</i> since it is in use";
                            }
                            else
                            {
                                actionMessage = "Unknown error while deleting wordmark <i>" + delete_value + "</i>";
                            }
                        }
                    }
                    else
                    {
                        // Or.. was this a save request
                        if (save_value.Length > 0)
                        {
                            Tracer.Add_Trace("Wordmarks_AdminViewer.Constructor", "Save wordmark '" + save_value + "'");

                            // Was this to save a new interface (from the main page) or edit an existing (from the popup form)?
                            if (save_value == new_wordmark_code)
                            {
                                string new_file = form["admin_wordmark_file"].Trim();
                                string new_link = form["admin_wordmark_link"].Trim();
                                string new_title = form["admin_wordmark_title"].Trim();

                                // Save this new wordmark
                                if (SobekCM_Database.Save_Icon( new_wordmark_code, new_file, new_link, new_title, Tracer ) > 0 )
                                {
                                    actionMessage = "Saved new wordmark <i>" + save_value + "</i>";
                                }
                                else
                                {
                                    actionMessage = "Unable to save new wordmark <i>" + save_value + "</i>";
                                }
                            }
                            else
                            {
                                string edit_file = form["form_wordmark_file"].Trim();
                                string edit_link = form["form_wordmark_link"].Trim();
                                string edit_title = form["form_wordmark_title"].Trim();

                                // Save this existing wordmark
                                if (SobekCM_Database.Save_Icon(save_value, edit_file, edit_link, edit_title, Tracer) > 0 )
                                {
                                    actionMessage = "Edited existing wordmark <i>" + save_value + "</i>";
                                }
                                else
                                {
                                    actionMessage = "Unable to edit existing wordmark <i>" + save_value + "</i>";
                                }
                            }
                        }
                    }
                }
                catch ( Exception )
                {
                    actionMessage = "Unknown error caught while handing request.";
                }
            }
        }

        #endregion


        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Interfaces' </value>
        public override string Web_Title
        {
            get { return "Wordmarks / Icons"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Wordmarks_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Wordmarks_AdminViewer.Add_HTML_In_Main_Form", "Add any popup divisions for form elements");

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_wordmark_code_tosave\" name=\"admin_wordmark_code_tosave\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_wordmark_code_delete\" name=\"admin_wordmark_code_delete\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- Wordmarks Edit Form -->");
            Output.WriteLine("<div class=\"admin_wordmark_popup_div\" id=\"form_wordmark\" style=\"display:none;\">");
            Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT WORDMARK / ICON <td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"wordmark_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"popup_table\">");

            // Add line for interface code and base interface code
            Output.Write("    <tr align=\"left\"><td width=\"120px\"><label for=\"form_wordmark_code\">Wordmark Code:</label></td>");
            Output.WriteLine("<td><span class=\"form_linkline admin_existing_code_line\" id=\"form_wordmark_code\"></span></td>");

            // Add line for filename
            Output.WriteLine("          <tr><td><label for=\"form_wordmark_file\">Image File:</label></td><td><input class=\"admin_wordmark_medium_input\" name=\"form_wordmark_file\" id=\"form_wordmark_file\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_wordmark_file', 'admin_wordmark_medium_input_focused')\" onblur=\"javascript:textbox_leave('form_wordmark_file', 'admin_wordmark_medium_input')\" /></td></tr>");

            // Add line for title
            Output.WriteLine("          <tr><td><label for=\"form_wordmark_title\">Title:</label></td><td><input class=\"admin_wordmark_large_input\" name=\"form_wordmark_title\" id=\"form_wordmark_title\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_wordmark_title', 'admin_wordmark_large_input_focused')\" onblur=\"javascript:textbox_leave('form_wordmark_title', 'admin_wordmark_large_input')\" /></td></tr>");

            // Add line for banner link
            Output.WriteLine("          <tr><td><label for=\"form_wordmark_link\">Link:</label></td><td><input class=\"admin_wordmark_large_input\" name=\"form_wordmark_link\" id=\"form_wordmark_link\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_wordmark_link', 'admin_wordmark_large_input_focused')\" onblur=\"javascript:textbox_leave('form_wordmark_link', 'admin_wordmark_large_input')\" /></td></tr>");

            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <center><a href=\"\" onclick=\"return wordmark_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center>");
            Output.WriteLine("</div>");

            Tracer.Add_Trace("Wordmarks_AdminViewer.Add_HTML_In_Main_Form", "Write the HTML for the rest of the form");

            Output.WriteLine("<!-- Wordmarks_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");

            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            Output.WriteLine("  <blockquote>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "admin/wordmarks\" target=\"ADMIN_WORDMARK_HELP\" >click here to view the help page</a>.</blockquote>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">New Wordmark / Icon</span>");
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("      <div class=\"admin_wordmark_new_div\">");
            Output.WriteLine("        <table class=\"popup_table\">");

            // Add line for wordmark code
            Output.Write("          <tr><td width=\"120px\"><label for=\"admin_wordmark_code\">Wordmark Code:</label></td>");
            Output.Write("<td><input class=\"admin_wordmark_small_input\" name=\"admin_wordmark_code\" id=\"admin_wordmark_code\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('admin_wordmark_code', 'admin_wordmark_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_wordmark_code', 'admin_wordmark_small_input')\" /></td></tr>");

            // Add line for filename
            Output.WriteLine("          <tr><td><label for=\"admin_wordmark_file\">Image File:</label></td><td><input class=\"admin_wordmark_medium_input\" name=\"admin_wordmark_file\" id=\"admin_wordmark_file\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_wordmark_file', 'admin_wordmark_medium_input_focused')\" onblur=\"javascript:textbox_leave('admin_wordmark_file', 'admin_wordmark_medium_input')\" /></td></tr>");

            // Add line for title
            Output.WriteLine("          <tr><td><label for=\"admin_wordmark_title\">Title:</label></td><td><input class=\"admin_wordmark_large_input\" name=\"admin_wordmark_title\" id=\"admin_wordmark_title\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_wordmark_title', 'admin_wordmark_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_wordmark_title', 'admin_wordmark_large_input')\" /></td></tr>");

            // Add line for banner link
            Output.WriteLine("          <tr><td><label for=\"admin_wordmark_link\">Link:</label></td><td><input class=\"admin_wordmark_large_input\" name=\"admin_wordmark_link\" id=\"admin_wordmark_link\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_wordmark_link', 'admin_wordmark_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_wordmark_link', 'admin_wordmark_large_input')\" /></td></tr>");

            Output.WriteLine("        </table>");
            Output.WriteLine("        <br />");
            Output.WriteLine("        <center><input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\" onclick=\"return save_new_wordmark();\"/></center>");
            Output.WriteLine("      </div>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("    <br />");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Wordmarks / Icons</span>");

            // Get the list of all wordmarks
            Dictionary<string, Wordmark_Icon> wordmarks = new Dictionary<string, Wordmark_Icon>();
            SobekCM_Database.Populate_Icon_List(wordmarks, Tracer);

            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" width=\"100%\" class=\"statsTable\">");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
            Output.WriteLine("  <tr align=\"center\" valign=\"bottom\" >");

            int current_column = 0;
            SortedList<string, Wordmark_Icon> sortedIcons = new SortedList<string, Wordmark_Icon>();
            foreach (Wordmark_Icon thisIcon in wordmarks.Values)
            {
                sortedIcons.Add(thisIcon.Code, thisIcon);
            }

            foreach (Wordmark_Icon thisIcon in sortedIcons.Values)
            {
                Output.Write("    <td width=\"200px\">");
                if (thisIcon.Link.Length > 0)
                    Output.Write("<a href=\"" + thisIcon.Link + "\" target=\"_blank\">");
                Output.Write("<img border=\"0px\" class=\"UfdcItemWorkdmark\" src=\"" + currentMode.Base_URL + "design/wordmarks/" + thisIcon.Image_FileName + "\"");
                if (thisIcon.Title.Length > 0)
                    Output.Write(" title=\"" + thisIcon.Title + "\"");
                Output.Write(" />");
                if (thisIcon.Link.Length > 0)
                    Output.Write("</a>");
                Output.Write("<br /><b>" + thisIcon.Code + "</b>");

                // Build the action links
                Output.Write("<br /><span class=\"SobekAdminActionLink\" >( ");
                Output.Write("<a title=\"Click to edit\" id=\"VIEW_" + thisIcon.Code + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return wordmark_form_popup('VIEW_" + thisIcon.Code + "', '" + thisIcon.Code + "', '" + thisIcon.Title.Replace("'", "") + "','" + thisIcon.Image_FileName + "','" + thisIcon.Link + "');\">edit</a> | ");
                Output.Write("<a title=\"Click to delete\" href=\"javascript:delete_wordmark('" + thisIcon.Code + "');\">delete</a> )</span>");
                Output.WriteLine("</td>");

                current_column++;

                if (current_column >= 4)
                {
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
                    Output.WriteLine("  <tr align=\"center\" valign=\"bottom\" >");
                    current_column = 0;
                }
            }

            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");

            Output.WriteLine("</table>");
            Output.WriteLine("    <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}

