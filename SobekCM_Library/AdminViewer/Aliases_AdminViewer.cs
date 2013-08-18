#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit existing item aggregation aliases, and add new aliases </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to manage the item aggregation aliases in this digital library</li>
    /// </ul></remarks>
    public class Aliases_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly Dictionary<string, string> aggregationAliases;

        #region Constructor

        /// <summary> Constructor for a new instance of the Aliases_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="currentMode"> Mode / navigation information for the current request</param>
        /// <param name="Aggregation_Aliases"> Dictionary of all current item aggregation aliases </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling an edit or new item aggregation alias is handled here in the constructor </remarks>
        public Aliases_AdminViewer(User_Object User, SobekCM_Navigation_Object currentMode, Dictionary<string,string> Aggregation_Aliases, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Aliases_AdminViewer.Constructor", String.Empty);

            // Save the mode and settings  here
            base.currentMode = currentMode;
            aggregationAliases = Aggregation_Aliases;

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

                    string save_value = form["admin_forwarding_tosave"].ToLower().Trim();
                    string new_alias = form["admin_forwarding_alias"].ToLower().Trim();

                    // Was this a save request
                    if (save_value.Length > 0)
                    {
                        // If this starts with a '-' this is a delete
                        if (save_value[0] == '-')
                        {
                            if ( save_value.Length > 1 )
                            {
                                save_value = save_value.Substring(1);
                                Tracer.Add_Trace("Aliases_AdminViewer.Constructor", "Delete alias '" + save_value + "'");
                                if (SobekCM_Database.Delete_Aggregation_Alias(save_value, Tracer))
                                {
                                    if (aggregationAliases.ContainsKey(save_value))
                                        aggregationAliases.Remove( save_value );

                                    actionMessage = "Deleted existing aggregation alias <i>" + save_value + "</i>";
                                }

                            }


                        }
                        else
                        {
                            Tracer.Add_Trace("Aliases_AdminViewer.Constructor", "Save alias '" + save_value + "'");

                            // Was this to save a new alias (from the main page) or edit an existing (from the popup form)?
                            if (save_value == new_alias)
                            {
                                string new_code = form["admin_forwarding_code"].ToLower().Trim();

                                // Save this new forwarding
                                if (SobekCM_Database.Save_Aggregation_Alias(save_value, new_code, Tracer))
                                {
                                    if (aggregationAliases.ContainsKey(save_value))
                                        aggregationAliases[save_value] = new_code;
                                    else
                                        aggregationAliases.Add(save_value, new_code);

                                    actionMessage = "Saved new aggregation alias <i>" + save_value + "</i>";
                                }
                                else
                                {
                                    actionMessage = "Unable to save new aggregation alias <i>" + save_value + "</i>";
                                }
                            }
                            else
                            {
                                string edit_code = form["form_forwarding_code"].ToLower().Trim();

                                // Save this existing forwarding
                                if (SobekCM_Database.Save_Aggregation_Alias(save_value, edit_code, Tracer))
                                {
                                    if (aggregationAliases.ContainsKey(save_value))
                                        aggregationAliases[save_value] = edit_code;
                                    else
                                        aggregationAliases.Add(save_value, edit_code);

                                    actionMessage = "Edited existing aggregation alias <i>" + save_value + "</i>";
                                }
                                else
                                {
                                    actionMessage = "Unable to save existing aggregation alias <i>" + save_value + "</i>";
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    actionMessage = "Unknown error caught while processing request";
                }
            }
        }

        #endregion

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Item Aggregation Aliases' </value>
        public override string Web_Title
        {
            get { return "Item Aggregation Aliases"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the alias list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aliases_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aliases_AdminViewer.Add_HTML_In_Main_Form", "Add any popup divisions for form elements");

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_forwarding_tosave\" name=\"admin_forwarding_tosave\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- Item Aggregation Aliases Edit Form -->");
            Output.WriteLine("<div class=\"admin_forwarding_popup_div\" id=\"form_forwarding\" style=\"display:none;\">");
            Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT AGGREGATION ALIAS</td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"alias_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"popup_table\">");

            // Add line for alias
            Output.WriteLine("    <tr><td width=\"120px\"><label for=\"form_forwarding_alias\">Alias:</label></td><td colspan=\"2\"><span class=\"form_linkline admin_existing_code_line\" id=\"form_forwarding_alias\"></span></td></tr>");

            // Add line for aggregation
            Output.WriteLine("    <tr><td><label for=\"form_forwarding_code\">Item Aggregation:</label></td><td colspan=\"2\"><input class=\"admin_forwarding_input\" name=\"form_forwarding_code\" id=\"form_forwarding_code\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('form_forwarding_code', 'admin_forwarding_input_focused')\" onblur=\"javascript:textbox_leave('form_forwarding_code', 'admin_forwarding_input')\" /></td></tr>");

            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <center><a href=\"\" onclick=\"return alias_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center>");
            Output.WriteLine("</div>");

            Tracer.Add_Trace("Aliases_AdminViewer.Add_HTML_In_Main_Form", "Write the rest of the form ");

            Output.WriteLine("<!-- Aliases_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");

            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    Use item aggregation aliases to allow a term to forward to an existing item aggregation. ");
            Output.WriteLine("    This creates a simpler URL and can forward from a discontinued item aggregation.<br /><br />");
            Output.WriteLine("    For more information about aggregation aliases and forwarding, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/aggraliases\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.");
            Output.WriteLine("  </blockquote>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">New Item Aggregation Alias</span>");
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("      <div class=\"admin_forwarding_new_div\">");
            Output.WriteLine("        <table class=\"popup_table\">");

            // Add line for alias
            Output.WriteLine("          <tr><td width=\"120px\"><label for=\"admin_forwarding_alias\">Alias:</label></td><td colspan=\"2\"><input class=\"admin_forwarding_input\" name=\"admin_forwarding_alias\" id=\"admin_forwarding_alias\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_forwarding_alias', 'admin_forwarding_input_focused')\" onblur=\"javascript:textbox_leave('admin_forwarding_alias', 'admin_forwarding_input')\" /></td></tr>");

            // Add line for aggregation
            Output.WriteLine("          <tr><td><label for=\"admin_forwarding_code\">Item Aggregation:</label></td><td><input class=\"admin_forwarding_input\" name=\"admin_forwarding_code\" id=\"admin_forwarding_code\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_forwarding_code', 'admin_forwarding_input_focused')\" onblur=\"javascript:textbox_leave('admin_forwarding_code', 'admin_forwarding_input')\" /></td>");

            Output.WriteLine("          <td><input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\" onclick=\"return save_new_alias();\"/></td></tr>");

            //            Output.WriteLine("          <td><a onclick=\"return save_new_alias();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" alt=\"SUBMIT\" /></a></td></tr>");
            Output.WriteLine("        </table>");


            //Output.WriteLine("        <center><input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif\" value=\"Submit\" alt=\"Submit\"></center>");


            Output.WriteLine("      </div>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("    <br />");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Item Aggregation Aliases</span>");
            Output.WriteLine("    <blockquote>");
            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"160px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
            Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\">ALIAS</span></th>");
            Output.WriteLine("    <th width=\"120px\" align=\"left\"><span style=\"color: White\">AGGREGATION</span></th>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            // Write the data for each interface
            foreach (KeyValuePair<string, string> thisForward in aggregationAliases)
            {
                // Build the action links
                Output.WriteLine("  <tr align=\"left\" >");
                Output.Write("    <td class=\"SobekAdminActionLink\" >( ");
                Output.Write("<a title=\"Click to edit\" id=\"VIEW_" + thisForward.Key + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return alias_form_popup('VIEW_" + thisForward.Key + "', '" + thisForward.Key + "','" + thisForward.Value + "');\">edit</a> | ");
                Output.Write("<a title=\"Click to view\" href=\"" + currentMode.Base_URL + thisForward.Key + "\" target=\"_PREVIEW\">view</a> | ");
                Output.Write("<a title=\"Delete this alias\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_alias('" + thisForward.Key + "');\">delete</a> )</td>");

                // Add the rest of the row with data
                Output.WriteLine("    <td>" + thisForward.Key + "</span></td>");
                Output.WriteLine("    <td>" + thisForward.Value + "</span></td>");
                Output.WriteLine("   </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            }

            Output.WriteLine("</table>");
            Output.WriteLine("    </blockquote>");
            Output.WriteLine("    <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}

