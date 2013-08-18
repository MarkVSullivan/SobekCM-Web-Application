#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
    /// <summary> Class allows an authenticated system admin to view all existing users, and choose a user to edit </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to view all registered users digital library</li>
    /// </ul></remarks>
    public class Users_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly User_Object editUser;
        private readonly Users_Admin_Mode_Enum mode;

        ///// <summary> Property indicates the standard navigation to be included at the top of the page by the
        ///// main MySobek html subwriter. </summary>
        ///// <value> This returns either NONE or ADMIN, depending on whether an individual user is being edited
        ///// or if the list is being viewed. </value>
        ///// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        ///// administrative tabs should be included as well.  </remarks>
        //public override MySobek_Included_Navigation_Enum Standard_Navigation_Type
        //{
        //    get
        //    {
        //        if ((currentMode.My_Sobek_SubMode.Length == 0) || (currentMode.My_Sobek_SubMode.IndexOf("v") > 0))
        //            return MySobek_Included_Navigation_Enum.System_Admin;
        //        else                        
        //            return MySobek_Included_Navigation_Enum.NONE;
        //    }
        //}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns various values depending on the current submode </value>
        public override string Web_Title
        {
            get 
            {
                switch (mode)
                {
                    case Users_Admin_Mode_Enum.List_Users_And_Groups:
                        return "Registered Users and Groups";

                    case Users_Admin_Mode_Enum.Edit_User:
                        return "Edit User";

                    case Users_Admin_Mode_Enum.View_User:
                        return "View User Information";
                }
                return "Registered Users and Groups";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Users_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Users_AdminViewer.Add_HTML_In_Main_Form", "Add hidden field");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_user_reset\" name=\"admin_user_reset\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_user_save\" name=\"admin_user_save\" value=\"\" />");
            Output.WriteLine();

            Tracer.Add_Trace("Users_AdminViewer.Add_HTML_In_Main_Form", "Add the rest of the form");

            Output.WriteLine("<!-- Users_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");


            // Is this for a single user edit more, or to list all the users
            switch (mode)
            {
                case Users_Admin_Mode_Enum.List_Users_And_Groups:
                    Write_User_User_Groups_List(Output, Tracer);
                    break;

                case Users_Admin_Mode_Enum.Edit_User:
                    Write_Edit_User_Form(Output, Tracer);
                    break;

                case Users_Admin_Mode_Enum.View_User:
                    Write_View_User_Form(Output);
                    break;
            }
        }

        private void Write_View_User_Form(TextWriter Output)
        {
            Output.WriteLine("<div class=\"SobekHomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            currentMode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\">Return to user list</a><br /><br />");
            currentMode.My_Sobek_SubMode = editUser.UserID.ToString();
            Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\">Edit this user</a>");
            currentMode.My_Sobek_SubMode = editUser.UserID.ToString() + "v";
            Output.WriteLine("  </blockquote>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">Basic Information</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("  <table cellpadding=\"4px\" >");
            if (editUser.UFID.Trim().Length > 0)
                Output.WriteLine("  <tr valign=\"top\"><td><b>UFID:</b></td><td>" + editUser.UFID + "</td></tr>");
            Output.WriteLine("  <tr valign=\"top\"><td><b>UserName:</b></td><td>" + editUser.UserName + "</td></tr>");
            Output.WriteLine("  <tr valign=\"top\"><td><b>Email:</b></td><td>" + editUser.Email + "</td></tr>");
            Output.WriteLine("  <tr valign=\"top\"><td><b>Full Name:</b></td><td>" + editUser.Full_Name + "</td></tr>");

            // Build the rights statement
            StringBuilder text_builder = new StringBuilder();
            if (editUser.Can_Submit)
                text_builder.Append("Can submit items<br />");
            if (editUser.Is_Internal_User)
                text_builder.Append("Is internal user<br />");            
            if (editUser.Editable_Regular_Expressions.Any(thisRegularExpression => thisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}"))
            {
                text_builder.Append("Can edit all items<br />");
            }
            if (editUser.Is_Portal_Admin)
                text_builder.Append("Is portal administrator<br />");

            if (editUser.Is_System_Admin)
                text_builder.Append("Is system administrator<br />");

            if (editUser.Include_Tracking_In_Standard_Forms)
                text_builder.Append("Tracking data should be included in standard input forms<br />");

            if (text_builder.Length == 0)
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Global Permissions:</b></td><td><i>none</i></td></tr>");
            }
            else
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Global Permissions:</b></td><td>" + text_builder + "</td></tr>");
                text_builder.Remove(0, text_builder.Length);
            }

            Output.WriteLine("  <tr valign=\"top\"><td><b>Edit Templates:</b></td><td>" + editUser.Edit_Template_MARC_Code + "<br />" + editUser.Edit_Template_Code + "</td></tr>");

            // Build the templates list
            foreach (string thisTemplate in editUser.Templates)
            {
                text_builder.Append(thisTemplate + "<br />");
            }
            if (text_builder.Length == 0)
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Templates:</b></td><td><i>none</i></td></tr>");
            }
            else
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Templates:</b></td><td>" + text_builder + "</td></tr>");
                text_builder.Remove(0, text_builder.Length);
            }

            // Build the projects list
            foreach (string thisProject in editUser.Projects)
                text_builder.Append(thisProject + "<br />");
            if (text_builder.Length == 0)
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Projects:</b></td><td><i>none</i></td></tr>");
            }
            else
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Projects:</b></td><td>" + text_builder + "</td></tr>");
                text_builder.Remove(0, text_builder.Length);
            }


            Output.WriteLine("  </table>");
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Group Membership</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            if (editUser.User_Groups.Count == 0)
            {
                Output.WriteLine("<i> &nbsp;This user is not a member of any user groups</i>");
            }
            else
            {
                foreach (string userGroup in editUser.User_Groups)
                {
                    text_builder.Append(userGroup + "<br />");
                }
                Output.WriteLine("  <table cellpadding=\"4px\" >");
                Output.WriteLine("  <tr valign=\"top\"><td><b>User Groups:</b></td><td>" + text_builder + "</td></tr>");
                Output.WriteLine("  </table>");
            }
            
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Aggregations</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            if (editUser.Aggregations.Count == 0)
            {

                Output.WriteLine("<i> &nbsp;No special aggregation rights are assigned to this user</i>");

            }
            else
            {
                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");

                // Get the list of collections lists in the user object
                ReadOnlyCollection<User_Editable_Aggregation> aggregations_in_editable_user = editUser.Aggregations;
                Dictionary<string, User_Editable_Aggregation> lookup_aggs = aggregations_in_editable_user.ToDictionary(thisAggr => thisAggr.Code.ToLower());

                // Step through each aggregation type
                foreach (string aggregationType in codeManager.All_Types)
                {
                    bool type_label_drawn = false;

                    // Show all matching rows
                    foreach (Item_Aggregation_Related_Aggregations thisAggr in codeManager.Aggregations_By_Type(aggregationType))
                    {

                        if (lookup_aggs.ContainsKey(thisAggr.Code.ToLower()))
                        {
                            if (!type_label_drawn)
                            {
                                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                                if ((aggregationType.Length > 0) && (aggregationType[aggregationType.Length - 1] != 'S'))
                                {
                                    Output.WriteLine("    <td colspan=\"7\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "S</b></span></td>");
                                }
                                else
                                {
                                    Output.WriteLine("    <td colspan=\"7\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "</b></span></td>");
                                }
                                Output.WriteLine("  </tr>");

                                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                                Output.WriteLine("    <td width=\"55px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Is on user's custom home page\">ON<br />HOME</acronym></span></td>");
                                Output.WriteLine("    <td width=\"57px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can select this aggregation when editing or submitting an item\">CAN<br />SELECT</acronym></span></td>");
                                Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit any item in this aggregation\">CAN<br />EDIT</acronym></span></td>");
                                Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">IS<br />CURATOR</acronym></span></td>");
                                Output.WriteLine("    <td align=\"left\" colspan=\"2\"><span style=\"color: White\">ITEM AGGREGATION</span></td>");
                                Output.WriteLine("   </tr>");

                                type_label_drawn = true;
                            }

                            Output.WriteLine("  <tr align=\"left\" >");
                            Output.WriteLine(lookup_aggs[thisAggr.Code.ToLower()].OnHomePage
                                                 ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                                 : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                            Output.WriteLine(lookup_aggs[thisAggr.Code.ToLower()].CanSelect
                                                 ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                                 : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                            Output.WriteLine(lookup_aggs[thisAggr.Code.ToLower()].CanEditItems
                                                 ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                                 : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                            Output.WriteLine(lookup_aggs[thisAggr.Code.ToLower()].IsCurator
                                                 ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                                 : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                            Output.WriteLine("    <td>" + thisAggr.Code + "</td>");
                            Output.WriteLine("    <td>" + thisAggr.Name + "</td>");
                            Output.WriteLine("   </tr>");
                            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"7\"></td></tr>");
                        }
                    }
                }

                Output.WriteLine("</table>");
                Output.WriteLine("<br />");

            }
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("</div>");
        }

        private void Write_Edit_User_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            int page = 1;
            if (currentMode.My_Sobek_SubMode.IndexOf("b") > 0)
                page = 2;
            else if (currentMode.My_Sobek_SubMode.IndexOf("c") > 0)
                page = 3;

            Output.WriteLine("  <div class=\"SobekHomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <b>Edit this user's permissions, abilities, and basic information</b>");
            Output.WriteLine("    <ul>");
            Output.WriteLine("      <li>Enter the permissions for this user below and press the SAVE button when all your edits are complete.</li>");
            Output.WriteLine("      <li>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/users\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</li>");
            Output.WriteLine("     </ul>");
            Output.WriteLine("  </div>");

            Output.WriteLine("  <div class=\"ViewsBrowsesRow\">");
            string last_mode = currentMode.My_Sobek_SubMode;
            currentMode.My_Sobek_SubMode = currentMode.My_Sobek_SubMode.Replace("b", "").Replace("c", "");
            if (page == 1)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " BASIC INFORMATION " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_user_edit_page('" + editUser.UserID + "a" + "');\">" + Unselected_Tab_Start + " BASIC INFORMATION " + Unselected_Tab_End + "</a>");
            }

            if (page == 2)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " GROUP MEMBERSHIP " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_user_edit_page('" + editUser.UserID + "b" + "');\">" + Unselected_Tab_Start + " GROUP MEMBERSHIP " + Unselected_Tab_End + "</a>");
            }

            if (page == 3)
            {
                Output.WriteLine("    " + Selected_Tab_Start + " AGGREGATIONS " + Selected_Tab_End);
            }
            else
            {
                Output.WriteLine("    <a href=\"\" onclick=\"return new_user_edit_page('" + editUser.UserID + "c" + "');\">" + Unselected_Tab_Start + " AGGREGATIONS " + Unselected_Tab_End + "</a>");


                //currentMode.My_Sobek_SubMode = edit_user.UserID + "c";
                //Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\">" + base.Unselected_Tab_Start + " AGGREGATIONS " + base.Unselected_Tab_End + "</a>");
            }
            Output.WriteLine("  </div>");

            Output.WriteLine("  <div class=\"SobekEditPanel\">");

            // Add the buttons
            currentMode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("  <table width=\"100%px\"><tr><td width=\"480px\">&nbsp;</td><td align=\"right\"><a href=\"" + currentMode.Redirect_URL() + "\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; <a href=\"\" onclick=\"return save_user_edits();\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Save\" alt=\"Save\" /></a></td><td width=\"20px\">&nbsp;</td></tr></table>");
            currentMode.My_Sobek_SubMode = last_mode;

            switch (page)
            {
                case 1:
                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle_first\"> &nbsp; User Information</span>");
                    Output.WriteLine("  <blockquote>");
                    Output.WriteLine("    <table>");

                    if (editUser.UFID.Trim().Length > 0)
                    {
                        if (editUser.UFID.Length > 4)
                        {
                            Output.Write("      <tr height=\"27px\"><td width=\"80px\">UFID:</td><td width=\"200px\"><span class=\"form_linkline\">" + editUser.UFID.Substring(0, 4) + "-" + editUser.UFID.Substring(4) + " &nbsp; &nbsp; </span></td>");
                        }
                        else
                        {
                            Output.Write("      <tr height=\"27px\"><td width=\"80px\">UFID:</td><td width=\"200px\"><span class=\"form_linkline\">" + editUser.UFID + " &nbsp; &nbsp; </span></td>");
                        }
                    }
                    else
                    {
                        Output.Write("      <tr height=\"27px\"><td width=\"80px\">&nbsp</td><td width=\"200px\">&nbsp;</span></td>");
                    }

                    Output.WriteLine("<td width=\"80\">Email:</td><td><span class=\"form_linkline\">" + editUser.Email + " &nbsp; &nbsp; </span></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td>UserName:</td><td><span class=\"form_linkline\">" + editUser.UserName + " &nbsp; &nbsp; </span></td><td>Full Name:</td><td><span class=\"form_linkline\">" + editUser.Full_Name + " &nbsp; &nbsp; </span></td></tr>");
                    Output.WriteLine("    </table>");
                    Output.WriteLine("  </blockquote>");

                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Current Affiliation Information</span><br />");
                    Output.WriteLine("  <blockquote>");
                    Output.WriteLine("    <table>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_organization\">Organization/University:</label></td><td><input id=\"admin_user_organization\" name=\"admin_user_organization\" class=\"users_large_input\" value=\"" + editUser.Organization + "\" type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_organization', 'users_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_organization', 'users_large_input')\" /></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_college\">College:</label></td><td><input id=\"admin_user_college\" name=\"admin_user_college\" class=\"users_large_input\" value=\"" + editUser.College + "\"type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_college', 'users_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_college', 'users_large_input')\" /></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_department\">Department:</label></td><td><input id=\"admin_user_department\" name=\"admin_user_department\" class=\"users_large_input\" value=\"" + editUser.Department + "\"type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_department', 'users_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_department', 'users_large_input')\" /></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_unit\">Unit:</label></td><td><input id=\"admin_user_unit\" name=\"admin_user_unit\" class=\"users_large_input\" value=\"" + editUser.Unit + "\" type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_unit', 'users_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_unit', 'users_large_input')\" /></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_org_code\">Code:</label></td><td><input id=\"admin_user_org_code\" name=\"admin_user_org_code\" class=\"users_code_input\" value=\"" + editUser.Organization_Code + "\" type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_org_code', 'users_code_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_org_code', 'users_code_input')\" /></td></tr>");
                    Output.WriteLine("    </table>");
                    Output.WriteLine("  </blockquote>");


                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Global Permissions</span><br />");
                    Output.WriteLine(editUser.Can_Submit
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_submit\" id=\"admin_user_submit\" checked=\"checked\" /> <label for=\"admin_user_submit\">Can submit items</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_submit\" id=\"admin_user_submit\" /> <label for=\"admin_user_submit\">Can submit items</label> <br />");

                    Output.WriteLine(editUser.Is_Internal_User
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_internal\" id=\"admin_user_internal\" checked=\"checked\" /> <label for=\"admin_user_internal\">Is internal user</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_internal\" id=\"admin_user_internal\" /> <label for=\"admin_user_internal\">Is internal user</label> <br />");

                    bool canEditAll = editUser.Editable_Regular_Expressions.Any(thisRegularExpression => thisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}");
                    Output.WriteLine(canEditAll
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_editall\" id=\"admin_user_editall\" checked=\"checked\" /> <label for=\"admin_user_editall\">Can edit <u>all</u> items</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_editall\" id=\"admin_user_editall\" /> <label for=\"admin_user_editall\">Can edit <u>all</u> items</label> <br />");

                    Output.WriteLine(editUser.Is_Portal_Admin
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_portaladmin\" id=\"admin_user_portaladmin\" checked=\"checked\" /> <label for=\"admin_user_portaladmin\">Is portal administrator</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_portaladmin\" id=\"admin_user_portaladmin\" /> <label for=\"admin_user_portaladmin\">Is portal administrator</label> <br />");

                    Output.WriteLine(editUser.Is_System_Admin
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_sysadmin\" id=\"admin_user_sysadmin\" checked=\"checked\" /> <label for=\"admin_user_sysadmin\">Is system administrator</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_sysadmin\" id=\"admin_user_sysadmin\" /> <label for=\"admin_user_sysadmin\">Is system administrator</label> <br />");

                    Output.WriteLine(editUser.Include_Tracking_In_Standard_Forms
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_includetracking\" id=\"admin_user_includetracking\" checked=\"checked\" /> <label for=\"admin_user_includetracking\">Tracking data should be included in standard input forms</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_includetracking\" id=\"admin_user_includetracking\" /> <label for=\"admin_user_includetracking\">Tracking data should be included in standard input forms</label> <br />");

                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Templates and Projects</span>");
                    Output.WriteLine("  <blockquote>");
                    Output.WriteLine("    <table>");
                    Output.WriteLine("      <tr height=\"35px\" valign=\"top\" >");
                    Output.WriteLine("        <td width=\"300px\">");
                    Output.WriteLine("          Edit Templates: &nbsp; ");
                    Output.WriteLine("          <select class=\"admin_user_select\" name=\"admin_user_edittemplate\" id=\"admin_user_edittemplate\">");

                    if (editUser.Edit_Template_Code.ToUpper().IndexOf("INTERNAL") >= 0)
                    {
                        Output.WriteLine("            <option value=\"internal\" selected=\"selected\">Internal</option>");
                        Output.WriteLine("            <option value=\"standard\">Standard</option>");
                    }
                    else
                    {
                        Output.WriteLine("            <option value=\"internal\">Internal</option>");
                        Output.WriteLine("            <option value=\"standard\" selected=\"selected\">Standard</option>");
                    }

                    Output.WriteLine("          </select>");
                    Output.WriteLine("        </td>");
                    Output.WriteLine("        <td> &nbsp; </td>");
                    Output.WriteLine("      </tr>");

                    DataSet projectTemplateSet = SobekCM_Database.Get_All_Projects_Templates(Tracer);

                    Output.WriteLine("      <tr valign=\"top\" >");
                    Output.WriteLine("        <td>");
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                    Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\">TEMPLATES</span></th>");
                    Output.WriteLine("   </tr>");
                    Output.WriteLine("  <tr ><td bgcolor=\"#e7e7e7\"></td></tr>");

                    ReadOnlyCollection<string> user_templates = editUser.Templates;
                    foreach (DataRow thisTemplate in projectTemplateSet.Tables[1].Rows)
                    {
                        string template_name = thisTemplate["TemplateName"].ToString();
                        string template_code = thisTemplate["TemplateCode"].ToString();

                        Output.Write("  <tr align=\"left\"><td><input type=\"checkbox\" name=\"admin_user_template_" + template_code + "\" id=\"admin_user_template_" + template_code + "\"");
                        if (user_templates.Contains(template_code))
                        {
                            Output.Write(" checked=\"checked\"");
                        }
                        if (template_name.Length > 0)
                        {
                            Output.WriteLine(" /> &nbsp; <acronym title=\"" + template_name.Replace("\"", "'") + "\"><label for=\"admin_user_template_" + template_code + "\">" + template_code + "</label></acronym></td></tr>");
                        }
                        else
                        {
                            Output.WriteLine(" /> &nbsp; <label for=\"admin_user_template_" + template_code + "\">" + template_code + "</label></td></tr>");
                        }
                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\"></td></tr>");
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("        </td>");

                    Output.WriteLine("        <td>");
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                    Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\">PROJECTS</span></th>");
                    Output.WriteLine("   </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\"></td></tr>");

                    ReadOnlyCollection<string> user_projects = editUser.Projects;
                    foreach (DataRow thisProject in projectTemplateSet.Tables[0].Rows)
                    {
                        string project_name = thisProject["ProjectName"].ToString();
                        string project_code = thisProject["ProjectCode"].ToString();

                        Output.Write("  <tr align=\"left\"><td><input type=\"checkbox\" name=\"admin_user_project_" + project_code + "\" id=\"admin_user_project_" + project_code + "\"");
                        if (user_projects.Contains(project_code))
                        {
                            Output.Write(" checked=\"checked\"");
                        }
                        if (project_name.Length > 0)
                        {
                            Output.WriteLine(" /> &nbsp; <acronym title=\"" + project_name.Replace("\"", "'") + "\"><label for=\"admin_user_project_" + project_code + "\">" + project_code + "</label></acronym></td></tr>");
                        }
                        else
                        {
                            Output.WriteLine(" /> &nbsp; <label for=\"admin_user_project_" + project_code + "\">" + project_code + "</label></td></tr>");
                        }

                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\"></td></tr>");
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("        </td>");

                    Output.WriteLine("      </tr>");
                    Output.WriteLine("   </table>");
                    Output.WriteLine("  </blockquote>");
                    break;

                case 2:
                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle_first\"> &nbsp; User Group Membership</span>");
                    Output.WriteLine("  <blockquote>");

                    DataTable userGroup = SobekCM_Database.Get_All_User_Groups(Tracer);
                    if ((userGroup == null) || (userGroup.Rows.Count == 0))
                    {
                        Output.WriteLine("<br />");
                        Output.WriteLine("<b>No user groups exist within this library instance</b>");
                        Output.WriteLine("<br />");
                    }
                    else
                    {
                        Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
                        Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                        Output.WriteLine("    <td colspan=\"3\"><span style=\"color: White\"><b>USER GROUPS</b></span></td>");
                        Output.WriteLine("   </tr>");
                        //Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                        //Output.WriteLine("    <td width=\"100px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Is this user a member of this group?\">IS MEMBER</acronym></span></td>");
                        //Output.WriteLine("    <td width=\"120px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Name of this user group\">GROUP NAME</acronym></span></td>");
                        //Output.WriteLine("    <td width=\"300px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Description of this user group\">GROUP DESCRIPTION</acronym></span></td>");
                        //Output.WriteLine("   </tr>");

                        foreach (DataRow thisRow in userGroup.Rows)
                        {
                            Output.WriteLine("  <tr align=\"left\" >");                         

                            Output.Write("    <td width=\"50px\" ><input type=\"checkbox\" name=\"group_" + thisRow["UserGroupID"] + "\" id=\"group_" + thisRow["UserGroupID"] + "\" ");
                            if ( editUser.User_Groups.Contains( thisRow["GroupName"] ))
                                Output.Write(" checked=\"checked\"");
                            Output.WriteLine("/></td>");
                            Output.WriteLine("    <td width=\"150px\" >" + thisRow["GroupName"] + "</td>");
                            Output.WriteLine("    <td width=\"400px\">" + thisRow["GroupDescription"] + "</td>");
                            Output.WriteLine("  </tr>");
                            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                        }

                        Output.WriteLine("</table>");
                    }

                    Output.WriteLine("  </blockquote>");
                    break;

                case 3:
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");

                    // Get the list of collections lists in the user object
                    ReadOnlyCollection<User_Editable_Aggregation> aggregations_in_editable_user = editUser.Aggregations;
                    Dictionary<string, User_Editable_Aggregation> lookup_aggs = aggregations_in_editable_user.ToDictionary(thisAggr => thisAggr.Code.ToLower());

                    // Step through each aggregation type
                    foreach (string aggregationType in codeManager.All_Types)
                    {
                        Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                        if ((aggregationType.Length > 0) && (aggregationType[aggregationType.Length - 1] != 'S'))
                        {
                            Output.WriteLine("    <td colspan=\"7\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "S</b></span></td>");
                        }
                        else
                        {
                            Output.WriteLine("    <td colspan=\"7\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "</b></span></td>");
                        }
                        Output.WriteLine("  </tr>");

                        Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                        Output.WriteLine("    <td width=\"55px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Is on user's custom home page\">ON<br />HOME</acronym></span></td>");
                        Output.WriteLine("    <td width=\"57px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can select this aggregation when editing or submitting an item\">CAN<br />SELECT</acronym></span></td>");
                        Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit any item in this aggregation\">CAN<br />EDIT</acronym></span></td>");
                        Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">IS<br />CURATOR</acronym></span></td>");
                        Output.WriteLine("    <td align=\"left\" colspan=\"2\"><span style=\"color: White\">ITEM AGGREGATION</span></td>");
                        Output.WriteLine("   </tr>");

                        // Show all matching rows
                        foreach (Item_Aggregation_Related_Aggregations thisAggr in codeManager.Aggregations_By_Type(aggregationType))
                        {
                            Output.WriteLine("  <tr align=\"left\" >");
                            if (!lookup_aggs.ContainsKey(thisAggr.Code.ToLower()))
                            {
                                Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_onhome_" + thisAggr.Code + "\" id=\"admin_project_onhome_" + thisAggr.Code + "\" /></td>");
                                Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" /></td>");
                                Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_" + thisAggr.Code + "\" id=\"admin_project_edit_" + thisAggr.Code + "\" /></td>");
                                Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" /></td>");
                            }
                            else
                            {
                                if (lookup_aggs[thisAggr.Code.ToLower()].OnHomePage)
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_onhome_" + thisAggr.Code + "\" id=\"admin_project_onhome_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
                                else
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_onhome_" + thisAggr.Code + "\" id=\"admin_project_onhome_" + thisAggr.Code + "\" /></td>");

                                if (lookup_aggs[thisAggr.Code.ToLower()].CanSelect)
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
                                else
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" /></td>");

                                if (lookup_aggs[thisAggr.Code.ToLower()].CanEditItems)
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_" + thisAggr.Code + "\" id=\"admin_project_edit_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
                                else
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_" + thisAggr.Code + "\" id=\"admin_project_edit_" + thisAggr.Code + "\" /></td>");

                                if (lookup_aggs[thisAggr.Code.ToLower()].IsCurator)
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
                                else
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" /></td>");

                            }

                            Output.WriteLine("    <td>" + thisAggr.Code + "</td>");
                            Output.WriteLine("    <td>" + thisAggr.Name + "</td>");
                            Output.WriteLine("   </tr>");
                            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"7\"></td></tr>");
                        }
                    }

                    Output.WriteLine("</table>");
                    Output.WriteLine("<br />");
                    break;
            }


            // Add the buttons
            currentMode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("  <table width=\"100%px\"><tr><td width=\"480px\">&nbsp;</td><td align=\"right\"><a href=\"" + currentMode.Redirect_URL() + "\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; <a href=\"\" onclick=\"return save_user_edits();\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Save\" alt=\"Save\" /></a></td><td width=\"20px\">&nbsp;</td></tr></table>");
            currentMode.My_Sobek_SubMode = last_mode;


            Output.WriteLine("</div>");
        }

        private void Write_User_User_Groups_List(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<div class=\"SobekHomeText\">");

            // Display the action message if there is one
            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            Output.WriteLine("  <br />");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing User Groups</span>");
            Output.WriteLine("  <br /><br />");

            // get the list of all user groups
            DataTable userGroup = SobekCM_Database.Get_All_User_Groups(Tracer);

            // Get the redirect
            currentMode.My_Sobek_SubMode = "XXXXXXX";
            currentMode.Admin_Type = Admin_Type_Enum.User_Groups;
            string redirect = currentMode.Redirect_URL();
            currentMode.My_Sobek_SubMode = String.Empty;
            currentMode.Admin_Type = Admin_Type_Enum.Users;

            // Show the user groups
            if ((userGroup == null) || (userGroup.Rows.Count == 0))
            {
                currentMode.My_Sobek_SubMode = "new";
                currentMode.Admin_Type = Admin_Type_Enum.User_Groups;
                Output.WriteLine("<blockquote>No user groups exist within this library instance. <a href=\"" + currentMode.Redirect_URL() + "\">Click here to add a new user group.</a></blockquote>");
                currentMode.My_Sobek_SubMode = String.Empty;
                currentMode.Admin_Type = Admin_Type_Enum.Users;
            }
            else
            {
                currentMode.My_Sobek_SubMode = "new";
                currentMode.Admin_Type = Admin_Type_Enum.User_Groups;
                Output.WriteLine("  <blockquote>Select a user group to edit or view.  <a href=\"" + currentMode.Redirect_URL() + "\">Click here to add a new user group.</a></blockquote>");
                currentMode.My_Sobek_SubMode = String.Empty;
                currentMode.Admin_Type = Admin_Type_Enum.Users;

                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                Output.WriteLine("    <th width=\"200px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
                Output.WriteLine("    <th width=\"140px\" align=\"left\"><span style=\"color: White\">NAME</span></th>");
                Output.WriteLine("    <th align=\"left\"><span style=\"color: White\">DESCRIPTION</span></th>");
                Output.WriteLine("   </tr>");

                foreach (DataRow thisRow in userGroup.Rows)
                {
                    Output.WriteLine("  <tr align=\"left\" >");
                    Output.Write("    <td class=\"SobekAdminActionLink\" >( ");

                    Output.Write("<a title=\"Click to edit\" href=\"" + redirect.Replace("XXXXXXX", thisRow["UserGroupID"].ToString()) + "\">edit</a> | ");
                    Output.Write("<a title=\"Click to view\" href=\"" + redirect.Replace("XXXXXXX", thisRow["UserGroupID"].ToString()) + "v\">view</a> ) </td>");

                    Output.WriteLine("    <td>" + thisRow["GroupName"] + "</td>");
                    Output.WriteLine("    <td>" + thisRow["GroupDescription"] + "</td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                }

                Output.WriteLine("</table>");
                Output.WriteLine("  <br />");
            }
            Output.WriteLine("  <br />");

            // List of all users
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Registered Users</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>Select a user to edit. Click <i>reset password</i> to email a new temporary password to the user.</blockquote>");
            // Get the list of all users
            DataTable usersTable = SobekCM_Database.Get_All_Users(Tracer);

            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"190px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
            Output.WriteLine("    <th width=\"320px\" align=\"left\"><span style=\"color: White\">NAME</span></th>");
            Output.WriteLine("    <th align=\"left\"><span style=\"color: White\">EMAIL</span></th>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            // Get the redirect
            currentMode.My_Sobek_SubMode = "XXXXXXX";
            redirect = currentMode.Redirect_URL();
            currentMode.My_Sobek_SubMode = String.Empty;

            // Write the data for each interface
            foreach (DataRow thisRow in usersTable.Rows)
            {
                // Pull all these values
                string userid = thisRow["UserID"].ToString();
                string fullname = thisRow["Full_Name"].ToString();
                string username = thisRow["UserName"].ToString();
                string email = thisRow["EmailAddress"].ToString();

                // Build the action links
                Output.WriteLine("  <tr align=\"left\" >");
                Output.Write("    <td class=\"SobekAdminActionLink\" >( ");

                Output.Write("<a title=\"Click to edit\" href=\"" + redirect.Replace("XXXXXXX", userid) + "\">edit</a> | ");
                Output.Write("<a title=\"Click to reset the password\" id=\"RESET_" + userid + "\" href=\"javascript:reset_password('" + userid + "','" + fullname.Replace("'", "") + "');\">reset password</a> | ");
                Output.Write("<a title=\"Click to view\" href=\"" + redirect.Replace("XXXXXXX", userid) + "v\">view</a> ) </td>");

                // Add the rest of the row with data
                Output.WriteLine("    <td>" + fullname + " ( " + username + " )</span></td>");
                Output.WriteLine("    <td>" + email + "</span></td>");
                Output.WriteLine("   </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            }

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }

        #region Nested type: Users_Admin_Mode_Enum

        private enum Users_Admin_Mode_Enum : byte
        {
            List_Users_And_Groups = 1,
            View_User,
            Edit_User
        }

        #endregion

        #region Constructor and code to handle any post backs

        /// <summary> Constructor for a new instance of the Users_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="currentMode"> Mode / navigation information for the current request</param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from a user edit or from reseting a user's password is handled here in the constructor </remarks>
        public Users_AdminViewer(User_Object User, SobekCM_Navigation_Object currentMode, Aggregation_Code_Manager Code_Manager, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Users_AdminViewer.Constructor", String.Empty);

            this.currentMode = currentMode;

            // Ensure the user is the system admin
            if ((User == null) || (!User.Is_System_Admin))
            {
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
                return;
            }

            // Set the action message to clear initially
            actionMessage = String.Empty;
            codeManager = Code_Manager;

            // Get the user to edit, if there was a user id in the submode
            editUser = null;
            if (currentMode.My_Sobek_SubMode.Length > 0)
            {
                try
                {
                    int edit_userid = Convert.ToInt32(currentMode.My_Sobek_SubMode.Replace("a", "").Replace("b", "").Replace("c", "").Replace("v", ""));

                    // Check this admin's session for this user object
                    Object sessionEditUser = HttpContext.Current.Session["Edit_User_" + edit_userid];
                    if (sessionEditUser != null)
                        editUser = (User_Object)sessionEditUser;
                    else
                    {
                        editUser = SobekCM_Database.Get_User(edit_userid, Tracer);
                        editUser.Should_Be_Able_To_Edit_All_Items = false;
                        if (editUser.Editable_Regular_Expressions.Any(thisRegularExpression => thisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}"))
                        {
                            editUser.Should_Be_Able_To_Edit_All_Items = true;
                        }
                    }
                }
                catch ( Exception)
                {
                    actionMessage = "Error while handing your request";
                }
            }

            // Determine the mode
            mode = Users_Admin_Mode_Enum.List_Users_And_Groups;
            if (editUser != null)
            {
                mode = currentMode.My_Sobek_SubMode.IndexOf("v") > 0 ? Users_Admin_Mode_Enum.View_User : Users_Admin_Mode_Enum.Edit_User;
            }
            else
            {
                currentMode.My_Sobek_SubMode = String.Empty;
            }

            // Perform post back work
            if (currentMode.isPostBack)
            {
                if ( mode == Users_Admin_Mode_Enum.List_Users_And_Groups )
                {
                    try
                    {
                        string reset_value = HttpContext.Current.Request.Form["admin_user_reset"];
                        if (reset_value.Length > 0)
                        {
                            int userid = Convert.ToInt32(reset_value);
                            User_Object reset_user = SobekCM_Database.Get_User(userid, Tracer);

                            // Create the random password
                            StringBuilder passwordBuilder = new StringBuilder();
                            Random randomGenerator = new Random(DateTime.Now.Millisecond);
                            while (passwordBuilder.Length < 12)
                            {
                                switch (randomGenerator.Next(0, 3))
                                {
                                    case 0:
                                        int randomNumber = randomGenerator.Next(65, 91);
                                        if ((randomNumber != 79) && (randomNumber != 75)) // Omit the 'O' and the 'K', confusing
                                            passwordBuilder.Append((char)randomNumber);
                                        break;

                                    case 1:
                                        int randomNumber2 = randomGenerator.Next(97, 123);
                                        if ((randomNumber2 != 111) && (randomNumber2 != 108) && (randomNumber2 != 107))  // Omit the 'o' and the 'l' and the 'k', confusing
                                            passwordBuilder.Append((char)randomNumber2);
                                        break;

                                    case 2:
                                        // Zero and one is omitted in this range, confusing
                                        int randomNumber3 = randomGenerator.Next(50, 58);
                                        passwordBuilder.Append((char)randomNumber3);
                                        break;
                                }
                            }
                            string password = passwordBuilder.ToString();

                            // Reset this password
                            if (!SobekCM_Database.Reset_User_Password(userid, password, true, Tracer))
                            {
                                actionMessage = "ERROR reseting password";
                            }
                            else
                            {

                                if (SobekCM_Database.Send_Database_Email(reset_user.Email, "my" + currentMode.SobekCM_Instance_Abbreviation.ToUpper() + " Password Reset", reset_user.Full_Name + ",\n\nYour my" + currentMode.SobekCM_Instance_Abbreviation.ToUpper() + " password has been reset to a temporary password.  The first time you logon, you will be required to change it.\n\n\tUsername: " + reset_user.UserName + "\n\tPassword: " + password + "\n\nYour password is case-sensitive and must be entered exactly as it appears above when logging on.\n\nIf you have any questions or problems logging on, feel free to contact us at " + SobekCM_Library_Settings.System_Email + ", or reply to this email.\n\n" + currentMode.Base_URL + "my/home\n", false, false, -1, -1))
                                {
                                    if ((user.UserID == 1) || (user.UserID == 2))
                                        actionMessage = "Reset of password (" + password + ") for '" + reset_user.Full_Name + "' complete";
                                    else
                                        actionMessage = "Reset of password for '" + reset_user.Full_Name + "' complete";
                                }
                                else
                                {
                                    if ((user.UserID == 1) || (user.UserID == 2))
                                        actionMessage = "ERROR while sending new password (" + password + ") to '" + reset_user.Full_Name + "'!";
                                    else
                                        actionMessage = "ERROR while sending new password to '" + reset_user.Full_Name + "'!";
                                }
                            }
                        }
                    }
                    catch
                    {
                        actionMessage = "ERROR while checking postback";
                    }
                }

                if (( mode == Users_Admin_Mode_Enum.Edit_User ) && ( editUser != null ))
                {
                    // Determine which page you are on
                    int page = 1;
                    if (currentMode.My_Sobek_SubMode.IndexOf("b") > 0)
                        page = 2;
                    else if (currentMode.My_Sobek_SubMode.IndexOf("c") > 0)
                        page = 3;

                    // Get a reference to this form
                    NameValueCollection form = HttpContext.Current.Request.Form;
                    string[] getKeys = form.AllKeys;

                    // Get the curret action
                    string action = form["admin_user_save"];

                    bool successful_save = true;
                    switch (page)
                    {
                        case 1:
                            string editTemplate = "Standard";
                            List<string> projects = new List<string>();
                            List<string> templates = new List<string>();

                            // First, set some flags to FALSE
                            editUser.Can_Submit = false;
                            editUser.Is_Internal_User = false;
                            editUser.Should_Be_Able_To_Edit_All_Items = false;
                            editUser.Is_System_Admin = false;
                            editUser.Is_Portal_Admin = false;
                            editUser.Include_Tracking_In_Standard_Forms = false;

                            // Step through each key
                            foreach (string thisKey in getKeys)
                            {
                                switch (thisKey)
                                {
                                    case "admin_user_submit":
                                        editUser.Can_Submit = true;
                                        break;

                                    case "admin_user_internal":
                                        editUser.Is_Internal_User = true;
                                        break;

                                    case "admin_user_editall":
                                        editUser.Should_Be_Able_To_Edit_All_Items = true;
                                        break;

                                    case "admin_user_sysadmin":
                                        editUser.Is_System_Admin = true;
                                        break;

                                    case "admin_user_portaladmin":
                                        editUser.Is_Portal_Admin = true;
                                        break;

                                    case "admin_user_includetracking":
                                        editUser.Include_Tracking_In_Standard_Forms = true;
                                        break;

                                    case "admin_user_edittemplate":
                                        editTemplate = form["admin_user_edittemplate"];
                                        break;

                                    case "admin_user_organization":
                                        editUser.Organization = form["admin_user_organization"];
                                        break;

                                    case "admin_user_college":
                                        editUser.College = form["admin_user_college"];
                                        break;

                                    case "admin_user_department":
                                        editUser.Department = form["admin_user_department"];
                                        break;

                                    case "admin_user_unit":
                                        editUser.Unit = form["admin_user_unit"];
                                        break;

                                    case "admin_user_org_code":
                                        editUser.Organization_Code = form["admin_user_org_code"];
                                        break;

                                    default:
                                        if (thisKey.IndexOf("admin_user_template_") == 0)
                                        {
                                            templates.Add(thisKey.Replace("admin_user_template_", ""));
                                        }
                                        if (thisKey.IndexOf("admin_user_project_") == 0)
                                        {
                                            projects.Add(thisKey.Replace("admin_user_project_", ""));
                                        }
                                        break;
                                }
                            }

                            // Determine the name for the actual edit templates from the combo box selection
                            editUser.Edit_Template_Code = "edit";
                            editUser.Edit_Template_MARC_Code = "editmarc";
                            if (editTemplate == "internal")
                            {
                                editUser.Edit_Template_Code = "edit_internal";
                                editUser.Edit_Template_MARC_Code = "editmarc_internal";
                            }

                            // Determine if the projects and templates need to be updated
                            bool update_templates_projects = false;
                            if ((templates.Count != editUser.Templates.Count) || (projects.Count != editUser.Projects.Count))
                            {
                                update_templates_projects = true;
                            }
                            else
                            {
                                // Check all of the templates
                                if (templates.Any(template => !editUser.Templates.Contains(template)))
                                {
                                    update_templates_projects = true;
                                }

                                // Check all the projects
                                if (!update_templates_projects)
                                {
                                    if (projects.Any(project => !editUser.Projects.Contains(project)))
                                    {
                                        update_templates_projects = true;
                                    }
                                }
                            }

                            // Update the templates and projects, if requested
                            if (update_templates_projects)
                            {
                                // Get the last defaults
                                string default_project = String.Empty;
                                string default_template = String.Empty;
                                if (editUser.Projects.Count > 0)
                                    default_project = editUser.Projects[0];
                                if (editUser.Templates.Count > 0)
                                    default_template = editUser.Templates[0];

                                // Now, set the user's template and projects
                                editUser.Clear_Projects();
                                editUser.Clear_Templates();
                                foreach (string thisProject in projects)
                                {
                                    editUser.Add_Project(thisProject);
                                }
                                foreach (string thisTemplate in templates)
                                {
                                    editUser.Add_Template(thisTemplate);
                                }

                                // Try to add back the defaults, which won't do anything if 
                                // the old defaults aren't in the new list
                                editUser.Set_Default_Project(default_project);
                                editUser.Set_Default_Template(default_template);
                            }
                            break;

                        case 2:
                            // Check the user groups for update
                            bool update_user_groups = false;
                            DataTable userGroup = SobekCM_Database.Get_All_User_Groups(Tracer);
                            List<string> newGroups = new List<string>();
                            foreach (DataRow thisRow in userGroup.Rows)
                            {
                                if ( form["group_" + thisRow["UserGroupID"]] != null )
                                {
                                    newGroups.Add( thisRow["GroupName"].ToString());
                                }
                            }

                            // Should we add the new user groups?  Did it change?
                            if ( newGroups.Count != editUser.User_Groups.Count )
                            {
                                update_user_groups = true;
                            }
                            if (update_user_groups)
                            {
                                editUser.Clear_UserGroup_Membership();
                                foreach( string thisUserGroup in newGroups )
                                    editUser.Add_User_Group( thisUserGroup );
                            }
                            break;

                        case 3:
                            Dictionary<string, User_Editable_Aggregation> aggregations = new Dictionary<string, User_Editable_Aggregation>();

                            // Step through each key
                            foreach (string thisKey in getKeys)
                            {
                                if (thisKey.IndexOf("admin_project_onhome_") == 0)
                                {
                                    string select_project = thisKey.Replace("admin_project_onhome_", "");
                                    if (aggregations.ContainsKey(select_project))
                                    {
                                        aggregations[select_project].OnHomePage = true;
                                    }
                                    else
                                    {
                                        aggregations.Add(select_project, new User_Editable_Aggregation(select_project, String.Empty, false, false, false, true));
                                    }
                                }
                                if (thisKey.IndexOf("admin_project_select_") == 0)
                                {
                                    string select_project = thisKey.Replace("admin_project_select_", "");
                                    if (aggregations.ContainsKey(select_project))
                                    {
                                        aggregations[select_project].CanSelect = true;
                                    }
                                    else
                                    {
                                        aggregations.Add(select_project, new User_Editable_Aggregation(select_project, String.Empty, true, false, false, false));
                                    }
                                }
                                if (thisKey.IndexOf("admin_project_edit_") == 0)
                                {
                                    string edit_project = thisKey.Replace("admin_project_edit_", "");
                                    if (aggregations.ContainsKey(edit_project))
                                    {
                                        aggregations[edit_project].CanEditItems = true;
                                    }
                                    else
                                    {
                                        aggregations.Add(edit_project, new User_Editable_Aggregation(edit_project, String.Empty, false, true, false, false));
                                    }
                                }
                                if (thisKey.IndexOf("admin_project_admin_") == 0)
                                {
                                    string admin_project = thisKey.Replace("admin_project_admin_", "");
                                    if (aggregations.ContainsKey(admin_project))
                                    {
                                        aggregations[admin_project].IsCurator = true;
                                    }
                                    else
                                    {
                                        aggregations.Add(admin_project, new User_Editable_Aggregation(admin_project, String.Empty, false, false, true, false));
                                    }
                                }
                            }

                            // Determine if the aggregations need to be edited
                            bool update_aggregations = false;
                            if (aggregations.Count != editUser.Aggregations.Count)
                            {
                                update_aggregations = true;
                            }
                            else
                            {
                                // Build a dictionary of the user aggregations as well
                                Dictionary<string, User_Editable_Aggregation> existingAggr = editUser.Aggregations.ToDictionary(thisAggr => thisAggr.Code);

                                // Check all the aggregations
                                foreach (User_Editable_Aggregation adminAggr in aggregations.Values)
                                {
                                    if (existingAggr.ContainsKey(adminAggr.Code))
                                    {
                                        if ((adminAggr.CanSelect != existingAggr[adminAggr.Code].CanSelect) || (adminAggr.CanEditItems != existingAggr[adminAggr.Code].CanEditItems) || (adminAggr.IsCurator != existingAggr[adminAggr.Code].IsCurator) || (adminAggr.OnHomePage != existingAggr[adminAggr.Code].OnHomePage))
                                        {
                                            update_aggregations = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        update_aggregations = true;
                                        break;
                                    }
                                }
                            }

                            // Update the aggregations, if requested
                            if (update_aggregations)
                            {
                                editUser.Clear_Aggregations();
                                if (aggregations.Count > 0)
                                {
                                    foreach (User_Editable_Aggregation dictionaryAggregation in aggregations.Values)
                                    {
                                        editUser.Add_Aggregation(dictionaryAggregation.Code, dictionaryAggregation.Name, dictionaryAggregation.CanSelect, dictionaryAggregation.CanEditItems, dictionaryAggregation.IsCurator, dictionaryAggregation.OnHomePage);
                                    }
                                }
                            }
                            break;
                    }

                    // Should this be saved to the database?
                    if (action == "save")
                    {
                        // Save this user
                        SobekCM_Database.Save_User(editUser, String.Empty, Tracer);

                        // Update the basic user information
                        SobekCM_Database.Update_SobekCM_User(editUser.UserID, editUser.Can_Submit, editUser.Is_Internal_User, editUser.Should_Be_Able_To_Edit_All_Items, editUser.Is_System_Admin, editUser.Is_Portal_Admin, editUser.Include_Tracking_In_Standard_Forms, editUser.Edit_Template_Code, editUser.Edit_Template_MARC_Code, true, true, true, Tracer);

                        // Update projects, if necessary
                        if (editUser.Projects.Count > 0)
                        {
                            if (!SobekCM_Database.Update_SobekCM_User_Projects(editUser.UserID, editUser.Projects, Tracer))
                            {
                                successful_save = false;
                            }
                        }

                        // Update templates, if necessary
                        if (editUser.Templates.Count > 0)
                        {
                            if (!SobekCM_Database.Update_SobekCM_User_Templates(editUser.UserID, editUser.Templates, Tracer))
                            {
                                successful_save = false;
                            }
                        }

                        // Save the aggregations linked to this user
                        if (!SobekCM_Database.Update_SobekCM_User_Aggregations(editUser.UserID, editUser.Aggregations, Tracer))
                        {
                            successful_save = false;
                        }

                        // Save the user group links
                        DataTable userGroup = SobekCM_Database.Get_All_User_Groups(Tracer);
                        Dictionary<string, int> groupnames_to_id = new Dictionary<string,int>();
                        foreach (DataRow thisRow in userGroup.Rows)
                        {
                            groupnames_to_id[ thisRow["GroupName"].ToString()] = Convert.ToInt32( thisRow["UserGroupID"]);
                         }
                        foreach( string userGroupName in editUser.User_Groups )
                        {
                            SobekCM_Database.Link_User_To_User_Group( editUser.UserID, groupnames_to_id[userGroupName]);
                        }

                        // Forward back to the list of users, if this was successful
                        if (successful_save)
                        {                       
                            // Clear the user from the sessions
                            HttpContext.Current.Session["Edit_User_" + editUser.UserID] = null;

                            // Redirect the user
                            currentMode.My_Sobek_SubMode = String.Empty;
                            currentMode.Redirect();
        }
                    }
                    else
                    {
                        // Save to the admins session
                        HttpContext.Current.Session["Edit_User_" + editUser.UserID] = editUser;
                        currentMode.My_Sobek_SubMode = action;
                        currentMode.Redirect();
                    }
                }
            }
        }

        #endregion
    }
}
