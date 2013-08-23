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
    /// <summary> Class allows an authenticated system admin to view all existing user groups, and choose a user group to edit </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to view all user groups in this digital library</li>
    /// </ul></remarks>
    public class User_Group_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly User_Group editGroup;
        private readonly Users_Group_Admin_Mode_Enum mode;

        #region Constructor and code to handle any post backs

        /// <summary> Constructor for a new instance of the User_Group_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="currentMode"> Mode / navigation information for the current request</param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from a user group edit is handled here in the constructor </remarks>
        public User_Group_AdminViewer(User_Object User, SobekCM_Navigation_Object currentMode, Aggregation_Code_Manager Code_Manager, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("User_Group_AdminViewer.Constructor", String.Empty);

            // Set the action message to clear initially
            actionMessage = String.Empty;
            codeManager = Code_Manager;

            // Get the user to edit, if there was a user id in the submode
            int edit_usergroupid = -100;
            editGroup = null;
            if (currentMode.My_Sobek_SubMode.Length > 0)
            {
                if (currentMode.My_Sobek_SubMode == "new")
                {
                    edit_usergroupid = -1;
                }
                else
                {
                    if (Int32.TryParse(currentMode.My_Sobek_SubMode.Replace("a", "").Replace("b", "").Replace("c", "").Replace("v", ""), out edit_usergroupid))
                        editGroup = SobekCM_Database.Get_User_Group(edit_usergroupid, Tracer);
                }
            }

            // Determine the mode
            mode = Users_Group_Admin_Mode_Enum.Error;
            if ((editGroup != null) || (edit_usergroupid == -1))
            {
                if ((currentMode.My_Sobek_SubMode.IndexOf("v") > 0) && (edit_usergroupid > 0))
                    mode = Users_Group_Admin_Mode_Enum.View_User_Group;
                else
                    mode = Users_Group_Admin_Mode_Enum.Edit_User_Group;
            }
            else
            {
                currentMode.My_Sobek_SubMode = String.Empty;
                currentMode.Admin_Type = Admin_Type_Enum.Users;
                currentMode.Redirect();
                return;
            }

            // Set an empty user group object for a new item
            if (edit_usergroupid < 0)
            {
                editGroup = new User_Group(String.Empty, String.Empty, -1);
            }

            // Perform post back work
            if (currentMode.isPostBack)
            {
                if ((mode == Users_Group_Admin_Mode_Enum.Edit_User_Group) && (editGroup != null))
                {
                    // Get a reference to this form
                    NameValueCollection form = HttpContext.Current.Request.Form;
                    string[] getKeys = form.AllKeys;

                    bool successful_save = true;
                    bool can_editall = editGroup.Editable_Regular_Expressions.Any(thisRegularExpression => thisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}");

                    bool can_submit = false;
                    bool is_internal = false;
                    bool is_admin = false;
                    bool is_portal = false;
                    string name = editGroup.Name;
                    string description = editGroup.Description;

                    List<string> projects = new List<string>();
                    List<string> templates = new List<string>();

                    Dictionary<string, User_Editable_Aggregation> aggregations = new Dictionary<string, User_Editable_Aggregation>();

                    // Step through each key
                    foreach (string thisKey in getKeys)
                    {
                        switch (thisKey)
                        {
                            case "groupName":
                                name = form[thisKey].Trim();
                                break;

                            case "groupDescription":
                                description = form[thisKey].Trim();
                                break;

                            case "admin_user_submit":
                                can_submit = true;
                                break;

                            case "admin_user_internal":
                                is_internal = true;
                                break;

                            case "admin_user_editall":
                                can_editall = true;
                                break;

                            case "admin_user_admin":
                                is_admin = true;
                                break;

                            case "admin_user_portaladmin":
                                is_portal = true;
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
                                if (thisKey.IndexOf("admin_project_onhome_") == 0)
                                {
                                    string select_project = thisKey.Replace("admin_project_onhome_", "");
                                    if (aggregations.ContainsKey(select_project))
                                    {
                                        aggregations[select_project].OnHomePage = true;
                                    }
                                    else
                                    {
                                        aggregations.Add(select_project, new User_Editable_Aggregation(select_project, String.Empty, false, false, false, true, false));
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
                                        aggregations.Add(select_project, new User_Editable_Aggregation(select_project, String.Empty, true, false, false, false, false));
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
                                        aggregations.Add(edit_project, new User_Editable_Aggregation(edit_project, String.Empty, false, true, false, false, false));
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
                                        aggregations.Add(admin_project, new User_Editable_Aggregation(admin_project, String.Empty, false, false, true, false, false ));
                                    }
                                }
                                break;
                        }
                    }

                    // Determine if the projects and templates need to be updated
                    bool update_templates_projects = false;
                    if ((templates.Count != editGroup.Templates.Count) || (projects.Count != editGroup.Projects.Count))
                    {
                        update_templates_projects = true;
                    }
                    else
                    {
                        // Check all of the templates
                        if (templates.Any(template => !editGroup.Templates.Contains(template)))
                        {
                            update_templates_projects = true;
                        }

                        // Check all the projects
                        if (!update_templates_projects)
                        {
                            if (projects.Any(project => !editGroup.Projects.Contains(project)))
                            {
                                update_templates_projects = true;
                            }
                        }
                    }

                    // Determine if the aggregations need to be edited
                    bool update_aggregations = false;
                    if (aggregations.Count != editGroup.Aggregations.Count)
                    {
                        update_aggregations = true;
                    }
                    else
                    {
                        // Build a dictionary of the user aggregations as well
                        Dictionary<string, User_Editable_Aggregation> existingAggr = editGroup.Aggregations.ToDictionary(thisAggr => thisAggr.Code);

                        // Check all the aggregations
                        foreach (User_Editable_Aggregation adminAggr in aggregations.Values)
                        {
                            if (existingAggr.ContainsKey(adminAggr.Code))
                            {
                                if ((adminAggr.CanSelect != existingAggr[adminAggr.Code].CanSelect) || (adminAggr.CanEditItems != existingAggr[adminAggr.Code].CanEditItems) || (adminAggr.IsCurator != existingAggr[adminAggr.Code].IsCurator))
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

                    // Must have a name to continue
                    if (name.Length > 0)
                    {
                        // Update the basic user information
                        int newid = SobekCM_Database.Save_User_Group(editGroup.UserGroupID, name, description, can_submit, is_internal, can_editall, is_admin, is_portal, false, update_templates_projects, update_aggregations, false, Tracer);
                        if (editGroup.UserGroupID < 0)
                        {
                            editGroup.UserGroupID = newid;
                        }

                        if (editGroup.UserGroupID > 0)
                        {
                            // Update the templates and projects, if requested
                            if (update_templates_projects)
                            {
                                // Update projects, if necessary
                                if (projects.Count > 0)
                                {
                                    if (!SobekCM_Database.Update_SobekCM_User_Group_Projects(editGroup.UserGroupID, projects, Tracer))
                                    {
                                        successful_save = false;
                                    }
                                }

                                // Update templates, if necessary
                                if (templates.Count > 0)
                                {
                                    if (!SobekCM_Database.Update_SobekCM_User_Group_Templates(editGroup.UserGroupID, templates, Tracer))
                                    {
                                        successful_save = false;
                                    }
                                }
                            }

                            // Update the aggregations, if requested
                            if (update_aggregations)
                            {
                                if (aggregations.Count > 0)
                                {
                                    List<User_Editable_Aggregation> aggregationList = aggregations.Values.ToList();
                                    if (!SobekCM_Database.Update_SobekCM_User_Group_Aggregations(editGroup.UserGroupID, aggregationList, Tracer))
                                    {
                                        successful_save = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            successful_save = false;
                        }
                    }
                    else
                    {
                        actionMessage = "User group's name must have a length greater than zero";
                        successful_save = false;
                    }

                    // Forward back to the list of users, if this was successful
                    if (successful_save)
                    {
                        currentMode.My_Sobek_SubMode = String.Empty;
                        currentMode.Redirect();
                    }
                }
            }
        }

        #endregion

        ///// <summary> Property indicates the standard navigation to be included at the top of the page by the
        ///// main MySobek html subwriter. </summary>
        ///// <value> This returns either NONE or ADMIN, depending on whether an individual user group is being edited
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
                    case Users_Group_Admin_Mode_Enum.Edit_User_Group:
                        return "Edit User Group";

                    case Users_Group_Admin_Mode_Enum.View_User_Group:
                        return "View User Group Information";
                }
                return "Error";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("User_Group_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("User_Group_AdminViewer.Add_HTML_In_Main_Form", "Add hidden field");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_user_reset\" name=\"admin_user_reset\" value=\"\" />");
            Output.WriteLine();

            Tracer.Add_Trace("User_Group_AdminViewer.Add_HTML_In_Main_Form", "Add the rest of the form");

            Output.WriteLine("<!-- User_Group_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");


            // Is this for a single user edit more, or to list all the users
            switch (mode)
            {
                case Users_Group_Admin_Mode_Enum.Edit_User_Group:
                    Write_Edit_User_Group_Form(Output, Tracer);
                    break;

                case Users_Group_Admin_Mode_Enum.View_User_Group:
                    Write_View_User_Group_Form(Output);
                    break;
            }
        }

        private void Write_View_User_Group_Form(TextWriter Output)
        {
            Output.WriteLine("<div class=\"SobekHomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            currentMode.My_Sobek_SubMode = String.Empty;
            currentMode.Admin_Type = Admin_Type_Enum.Users;
            Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\">Return to user group list</a><br /><br />");
            currentMode.Admin_Type = Admin_Type_Enum.User_Groups;
            currentMode.My_Sobek_SubMode = editGroup.UserGroupID.ToString();
            Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\">Edit this user group</a>");
            currentMode.My_Sobek_SubMode = editGroup.UserGroupID.ToString() +"v";
            Output.WriteLine("  </blockquote>");

            if ( !String.IsNullOrEmpty(actionMessage) )
                Output.WriteLine("<strong>" + actionMessage + "</strong>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">Basic Information</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("  <table cellpadding=\"4px\" >");
            Output.WriteLine("  <tr valign=\"top\"><td><b>Name:</b></td><td>" + editGroup.Name + "</td></tr>");
            Output.WriteLine("  <tr valign=\"top\"><td><b>Description:</b></td><td>" + editGroup.Description + "</td></tr>");

            // Build the rights statement
            StringBuilder text_builder = new StringBuilder();
            if (editGroup.Can_Submit)
                text_builder.Append("Can submit items<br />");
            if (editGroup.Is_Internal_User)
                text_builder.Append("Is internal user<br />");     
            if (editGroup.Editable_Regular_Expressions.Any(thisRegularExpression => thisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}"))
            {
                text_builder.Append("Can edit all items<br />");
            }
            if (editGroup.Is_System_Admin)
                text_builder.Append("Is system administrator<br />");
            if (text_builder.Length == 0)
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Global Permissions:</b></td><td><i>none</i></td></tr>");
            }
            else
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Global Permissions:</b></td><td>" + text_builder + "</td></tr>");
                text_builder.Remove(0, text_builder.Length);
            }

            // Build the templates list
            foreach (string thisTemplate in editGroup.Templates)
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
            foreach (string thisProject in editGroup.Projects)
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
            Output.WriteLine("  <span class=\"SobekAdminTitle\">User Membership</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            if (editGroup.Users.Count == 0)
            {
                Output.WriteLine("<i> &nbsp;This user group does not currently contain any users</i>");
            }
            else
            {
                foreach (User_Group_Member thisUser in editGroup.Users)
                {
                    text_builder.Append(thisUser.UserName + "<br />");
                }
                Output.WriteLine("  <table cellpadding=\"4px\" >");
                Output.WriteLine("  <tr valign=\"top\"><td><b>Users:</b></td><td>" + text_builder + "</td></tr>");
                Output.WriteLine("  </table>");
            }

            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Aggregations</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            if (editGroup.Aggregations.Count == 0)            
            {
                Output.WriteLine("<i> &nbsp;No special aggregation rights are assigned to this user group</i>");
            }
            else
            {
                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");

                // Get the list of collections lists in the user object
                ReadOnlyCollection<User_Editable_Aggregation> aggregations_in_editable_user = editGroup.Aggregations;
                Dictionary<string, User_Editable_Aggregation> lookup_aggs = aggregations_in_editable_user.ToDictionary(thisAggr => thisAggr.Code.ToLower());

                // Step through each aggregation type
                foreach (string aggregationType in codeManager.All_Types)
                {
                    bool type_label_drawn = false;

                    // Show all matching rows
                    foreach (Item_Aggregation_Related_Aggregations thisAggr in codeManager.Aggregations_By_Type(aggregationType).Where(thisAggr => lookup_aggs.ContainsKey(thisAggr.Code)))
                    {
                        if (!type_label_drawn)
                        {
                            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                            if ((aggregationType.Length > 0) && (aggregationType[aggregationType.Length - 1] != 'S'))
                            {
                                Output.WriteLine("    <td colspan=\"6\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "S</b></span></td>");
                            }
                            else
                            {
                                Output.WriteLine("    <td colspan=\"6\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "</b></span></td>");
                            }
                            Output.WriteLine("  </tr>");

                            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                            Output.WriteLine("    <td width=\"57px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can select this aggregation when editing or submitting an item\">CAN<br />SELECT</acronym></span></td>");
                            Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit any item in this aggregation\">CAN<br />EDIT</acronym></span></td>");
                            Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">IS<br />CURATOR</acronym></span></td>");
                            Output.WriteLine("    <td align=\"left\" colspan=\"2\"><span style=\"color: White\">ITEM AGGREGATION</span></td>");
                            Output.WriteLine("   </tr>");

                            type_label_drawn = true;
                        }

                        Output.WriteLine("  <tr align=\"left\" >");

                        Output.WriteLine(lookup_aggs[thisAggr.Code].CanSelect
                                             ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                             : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                        Output.WriteLine(lookup_aggs[thisAggr.Code].CanEditItems
                                             ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                             : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                        Output.WriteLine(lookup_aggs[thisAggr.Code].IsCurator
                                             ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                             : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                        Output.WriteLine("    <td>" + thisAggr.Code + "</td>");
                        Output.WriteLine("    <td>" + thisAggr.Name + "</td>");
                        Output.WriteLine("   </tr>");
                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"6\"></td></tr>");
                    }
                }

                Output.WriteLine("</table>");
                Output.WriteLine("<br />");

            }
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("</div>");
        }

        private void Write_Edit_User_Group_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <div class=\"SobekHomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <b>Edit this user group's permissions and abilities</b>");
            Output.WriteLine("    <ul>");
            Output.WriteLine("      <li>Enter the permissions for this user group below and press the SAVE button when all your edits are complete.</li>");
            Output.WriteLine("      <li>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/users\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</li>");
            Output.WriteLine("     </ul>");
            Output.WriteLine("  </div>");

            Output.WriteLine("  <div class=\"ViewsBrowsesRow\">");
            Output.WriteLine("    " + Selected_Tab_Start + " GROUP INFORMATION " + Selected_Tab_End);
            Output.WriteLine("  </div>");

            Output.WriteLine("  <div class=\"SobekEditPanel\">");

            // Add the buttons
            string last_mode = currentMode.My_Sobek_SubMode;
            currentMode.My_Sobek_SubMode = String.Empty;
            currentMode.Admin_Type = Admin_Type_Enum.Users;
            Output.WriteLine("  <table width=\"100%px\"><tr><td width=\"480px\">&nbsp;</td><td align=\"right\"><a href=\"" + currentMode.Redirect_URL() + "\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></td><td width=\"20px\">&nbsp;</td></tr></table>");
            currentMode.My_Sobek_SubMode = last_mode;
            currentMode.Admin_Type = Admin_Type_Enum.User_Groups;


            Output.WriteLine("  <span class=\"SobekEditItemSectionTitle_first\"> &nbsp; Basic Information</span>");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    <table>");
            Output.WriteLine("      <tr><td><b><label for=\"groupName\">Name:</label></b></td><td><input id=\"groupName\" name=\"groupName\" class=\"admin_small_input\" value=\"" + editGroup.Name + "\" type=\"text\" onfocus=\"javascript:textbox_enter('groupName', 'admin_small_input_focused')\" onblur=\"javascript:textbox_leave('groupName', 'admin_small_input')\" /></td></tr>");
            Output.WriteLine("      <tr><td><b><label for=\"groupDescription\">Description:</label></b></td><td><input id=\"groupDescription\" name=\"groupDescription\" class=\"admin_large_input\" value=\"" + editGroup.Description + "\" type=\"text\" onfocus=\"javascript:textbox_enter('groupDescription', 'admin_large_input_focused')\" onblur=\"javascript:textbox_leave('groupDescription', 'admin_large_input')\" /></td></tr>");
            Output.WriteLine("    </table>");
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <br />");

            Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Global Permissions</span><br />");
            Output.WriteLine(editGroup.Can_Submit
                                 ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_submit\" id=\"admin_user_submit\" checked=\"checked\" /> <label for=\"admin_user_submit\">Can submit items</label> <br />"
                                 : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_submit\" id=\"admin_user_submit\" /> <label for=\"admin_user_submit\">Can submit items</label> <br />");

            Output.WriteLine(editGroup.Is_Internal_User
                                 ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_internal\" id=\"admin_user_internal\" checked=\"checked\" /> <label for=\"admin_user_internal\">Is internal user</label> <br />"
                                 : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_internal\" id=\"admin_user_internal\" /> <label for=\"admin_user_internal\">Is internal user</label> <br />");

            bool canEditAll = editGroup.Editable_Regular_Expressions.Any(thisRegularExpression => thisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}");
            Output.WriteLine(canEditAll
                                 ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_editall\" id=\"admin_user_editall\" checked=\"checked\" /> <label for=\"admin_user_editall\">Can edit <u>all</u> items</label> <br />"
                                 : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_editall\" id=\"admin_user_editall\" /> <label for=\"admin_user_editall\">Can edit <u>all</u> items</label> <br />");

            Output.WriteLine(editGroup.Is_System_Admin
                                 ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_admin\" id=\"admin_user_admin\" checked=\"checked\" /> <label for=\"admin_user_admin\">Is system administrator</label> <br />"
                                 : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_admin\" id=\"admin_user_admin\" /> <label for=\"admin_user_admin\">Is system administrator</label> <br />");

            Output.WriteLine("  <br />");
            Output.WriteLine("  <br />");


            Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Templates and Projects</span>");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    <table width=\"600px\">");

            DataSet projectTemplateSet = SobekCM_Database.Get_All_Projects_Templates(Tracer);

            Output.WriteLine("      <tr valign=\"top\" >");
            Output.WriteLine("        <td wdith=\"300px\">");
            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\">TEMPLATES</span></th>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr ><td bgcolor=\"#e7e7e7\"></td></tr>");

            ReadOnlyCollection<string> user_templates = editGroup.Templates;
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

            ReadOnlyCollection<string> user_projects = editGroup.Projects;
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

            Output.WriteLine("  <br />");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Aggregations</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <br />");
            Output.WriteLine("<table><tr><td width=\"30px\">&nbsp;</td><td>");
            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");

            // Get the list of collections lists in the user object
            ReadOnlyCollection<User_Editable_Aggregation> aggregations_in_editable_user = editGroup.Aggregations;
            Dictionary<string, User_Editable_Aggregation> lookup_aggs = aggregations_in_editable_user.ToDictionary(thisAggr => thisAggr.Code.ToLower());

            // Step through each aggregation type
            foreach (string aggregationType in codeManager.All_Types)
            {
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                if ((aggregationType.Length > 0) && (aggregationType[aggregationType.Length - 1] != 'S'))
                {
                    Output.WriteLine("    <td colspan=\"6\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "S</b></span></td>");
                }
                else
                {
                    Output.WriteLine("    <td colspan=\"6\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "</b></span></td>");
                }
                Output.WriteLine("  </tr>");

                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                Output.WriteLine("    <td width=\"57px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can select this aggregation when editing or submitting an item\">CAN<br />SELECT</acronym></span></td>");
                Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit any item in this aggregation\">CAN<br />EDIT</acronym></span></td>");
                Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">IS<br />CURATOR</acronym></span></td>");
                Output.WriteLine("    <td align=\"left\" colspan=\"2\"><span style=\"color: White\">ITEM AGGREGATION</span></td>");
                Output.WriteLine("   </tr>");

                // Show all matching rows
                foreach (Item_Aggregation_Related_Aggregations thisAggr in codeManager.Aggregations_By_Type(aggregationType))
                {
                    Output.WriteLine("  <tr align=\"left\" >");
                    if (!lookup_aggs.ContainsKey(thisAggr.Code))
                    {
                        Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" /></td>");
                        Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_" + thisAggr.Code + "\" id=\"admin_project_edit_" + thisAggr.Code + "\" /></td>");
                        Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" /></td>");
                    }
                    else
                    {
                        if (lookup_aggs[thisAggr.Code].CanSelect)
                            Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
                        else
                            Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" /></td>");

                        if (lookup_aggs[thisAggr.Code].CanEditItems)
                            Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_" + thisAggr.Code + "\" id=\"admin_project_edit_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
                        else
                            Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_" + thisAggr.Code + "\" id=\"admin_project_edit_" + thisAggr.Code + "\" /></td>");

                        if (lookup_aggs[thisAggr.Code].IsCurator)
                            Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
                        else
                            Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" /></td>");

                    }

                    Output.WriteLine("    <td>" + thisAggr.Code + "</td>");
                    Output.WriteLine("    <td>" + thisAggr.Name + "</td>");
                    Output.WriteLine("   </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"6\"></td></tr>");
                }
            }

            Output.WriteLine("</table>");
            Output.WriteLine("</td></tr></table>");
            Output.WriteLine("<br />");

            // Add the buttons
            currentMode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("  <table width=\"100%px\"><tr><td width=\"480px\">&nbsp;</td><td align=\"right\"><a href=\"" + currentMode.Redirect_URL() + "\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></td><td width=\"20px\">&nbsp;</td></tr></table>");
            currentMode.My_Sobek_SubMode = last_mode;

            Output.WriteLine("</div>");
        }

        #region Nested type: Users_Group_Admin_Mode_Enum

        private enum Users_Group_Admin_Mode_Enum : byte
        {
            Error = 1,
            View_User_Group,
            Edit_User_Group
        }

        #endregion
    }
}
