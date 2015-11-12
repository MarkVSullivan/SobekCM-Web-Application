#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Administrative viewer allows all the users with special permissions to be viewed,
    /// allowing for top-level user management  </summary>
    public class Permissions_Reports_AdminViewer : abstract_AdminViewer
    {
        private string actionMessage;

        /// <summary> Constructor for a new version of the <see cref="Permissions_Reports_AdminViewer"/> class. </summary>
        /// <param name="RequestSpecificValues">All the necessary, non-global data specific to the current request</param>
        public Permissions_Reports_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Permissions_Reports_AdminViewer.Constructor", String.Empty);
            actionMessage = String.Empty;

            // Ensure the user is the system admin
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.Is_System_Admin))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If this is posted back, look for the reset
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                string reset_value = HttpContext.Current.Request.Form[""];
                if ((!String.IsNullOrEmpty(reset_value)) && (reset_value == "reset"))
                {
                    // Just ensure everything is emptied out
                    HttpContext.Current.Cache.Remove("GlobalPermissionsReport");
                    HttpContext.Current.Cache.Remove("GlobalPermissionsUsersLinked");
                    HttpContext.Current.Cache.Remove("GlobalPermissionsLinkedAggr");
                    HttpContext.Current.Cache.Remove("GlobalPermissionsReportSubmit");
                }
            }
        }

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner, HtmlSubwriter_Behaviors_Enum.Use_Jquery_DataTables }; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page,
        ///  just below the banner </summary>
        public override string Web_Title
        {
            get { return "User Permissions Reports"; }
        }
        
        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.User_Permission_Img; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of any form) </summary>
        /// <param name="Output">Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This does nothing </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Nothin yet
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<!-- Permissions_Reports_AdminViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");

            int page = 1;
            string submode = "a";

            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.My_Sobek_SubMode))
            {
                switch (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.ToLower())
                {
                    case "b":
                        page = 2;
                        break;

                    case "c":
                        page = 3;
                        break;

                    case "d":
                        page = 4;
                        break;

                    case "e":
                        page = 5;
                        break;
                }

                submode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            }

            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "Xyzzy";
            string userAdminUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Groups;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "Xyzzy";
            string userGroupAdminUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = submode;

            Output.WriteLine("  <div class=\"sbkAdm_HomeText\">");

            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                if (actionMessage.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageError\">" + actionMessage + "</div>");
                }
                else
                {

                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageSuccess\">" + actionMessage + "</div>");
                }
                Output.WriteLine("  <br />");
            }

           
            Output.WriteLine("  <div class=\"sbkPrav_ButtonsDiv\">");
            Output.WriteLine("    <button title=\"Refresh all permissions\" class=\"sbkPrav_RoundButton\" onclick=\"$('#admin_permissions_reset').val('reset');\"> REFRESH </button>");
            Output.WriteLine("  </div>");
            Output.WriteLine("  <input type=\"hidden\" id=\"admin_permissions_reset\" name=\"admin_permissions_reset\" value=\"\" />");
    


            Output.WriteLine("  <p style=\"text-align: left; padding:0 20px 0 70px;width:800px;\">This report allows you to view the permissions that are set for users and groups within this repository both globally and at the individual aggregation and user level.</p>");


            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("  <p style=\"text-align: left; padding:0 20px 0 70px;width:800px;\">Use the <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">administrative Users &amp; Groups</a> to assign any new collection-specific user permissions.</p>");
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;

            //Output.WriteLine("    <ul>");
            //Output.WriteLine("      <li>Enter the permissions for this user below and press the SAVE button when all your edits are complete.</li>");
            //Output.WriteLine("      <li>For clarification of any terms on this form, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/users\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</li>");
            //Output.WriteLine("     </ul>");
            Output.WriteLine("  </div>");

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs sbkAdm_HomeTabs\">");
            Output.WriteLine("  <div class=\"tabs\">");
            Output.WriteLine("    <ul>");
            
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "XyzzyXyzzy";
            string url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;

            if (page == 1)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> GLOBAL LIST </li>");
            }
            else
            {
                Output.WriteLine("      <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy","a") + "';return false;\"> GLOBAL LIST </li>");
            }

            if (page == 2)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> GLOBAL TABLE </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "b") + "';return false;\"> GLOBAL TABLE </li>");
            }

            if (page == 3)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> ALL AGGREGATIONS </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "c") + "';return false;\"> ALL AGGREGATIONS </li>");
            }

            if (page == 4)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> SINGLE AGGREGATION </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "d") + "';return false;\"> SINGLE AGGREGATION </li>");
            }

            if (page == 5)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> CAN SUBMIT ITEMS </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"window.location.href=\'" + url.Replace("XyzzyXyzzy", "e") + "';return false;\"> CAN SUBMIT ITEMS </li>");
            }

            Output.WriteLine("    </ul>");
            Output.WriteLine("  </div>");

            Output.WriteLine("    <div class=\"tabscontent\">");
            Output.WriteLine("    	<div class=\"sbkUgav_TabPage\" id=\"tabpage_1\">");

            //// Add the buttons
            //Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            //Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            //Output.WriteLine("    <button title=\"Save changes to this user group\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            //Output.WriteLine("  </div>");
            //Output.WriteLine();


            Output.WriteLine();

            switch (page)
            {
                case 1:
                    // Try to get the global permissions table
                    DataTable globalPermissions = HttpContext.Current.Cache["GlobalPermissionsReport"] as DataTable;
                    if (globalPermissions == null)
                    {
                        globalPermissions = SobekCM_Database.Get_Global_User_Permissions(RequestSpecificValues.Tracer);
                        if (globalPermissions == null)
                        {
                            actionMessage = "Error pulling global user permission!";
                        }
                        else
                        {
                            HttpContext.Current.Cache.Insert("GlobalPermissionsReport", globalPermissions, null, DateTime.Now.AddSeconds(60d), Cache.NoSlidingExpiration);
                        }
                    }

                    if (globalPermissions != null)
                    {
                        Output.WriteLine("<p style=\"width:800px;\">The lists below indicate the global permissions assigned to different registered users, either individually assigned or assigned thorugh user group membership.</p>");

                        List<string> hostAdmins = new List<string>();
                        List<string> sysAdmins = new List<string>();
                        List<string> portalAdmins = new List<string>();
                        List<string> canDeleteAll = new List<string>();
                        List<string> internalUsers = new List<string>();
                        List<string> canEditAll = new List<string>();

                        foreach (DataRow thisUser in globalPermissions.Rows)
                        {
                            string name = thisUser["LastName"] + ", " + thisUser["FirstName"];
                            if ((thisUser["Nickname"] != DBNull.Value) && (!String.IsNullOrEmpty(thisUser["Nickname"].ToString())))
                            {
                                name = name + " (\"" + thisUser["Nickname"] + "\")";
                            }

                            // Check for host admins
                            if (Convert.ToBoolean(thisUser["IsHostAdmin"]))
                            {
                                if (!hostAdmins.Contains(name)) hostAdmins.Add(name);
                            }
                            else if (Convert.ToBoolean(thisUser["IsSystemAdmin"]))
                            {
                                if (!sysAdmins.Contains(name)) sysAdmins.Add(name);
                                if (!canDeleteAll.Contains(name)) canDeleteAll.Add(name);
                                if (!canEditAll.Contains(name)) canEditAll.Add(name);
                                if (!internalUsers.Contains(name)) internalUsers.Add(name);
                            }
                            else if (Convert.ToBoolean(thisUser["IsPortalAdmin"]))
                            {
                                if (!portalAdmins.Contains(name)) portalAdmins.Add(name);
                                if (!canDeleteAll.Contains(name)) canDeleteAll.Add(name);
                                if (!canEditAll.Contains(name)) canEditAll.Add(name);
                                if (!internalUsers.Contains(name)) internalUsers.Add(name);
                            }
                            else
                            {
                                if ((Convert.ToBoolean(thisUser["Can_Delete_All_Items"])) && (!canDeleteAll.Contains(name))) canDeleteAll.Add(name);
                                if ((Convert.ToBoolean(thisUser["Internal_User"])) && (!internalUsers.Contains(name))) internalUsers.Add(name);
                                if ((Convert.ToBoolean(thisUser["Can_Edit_All_Items"])) && (!canEditAll.Contains(name))) canEditAll.Add(name);
                            }
                        }

                        Output.WriteLine("  <table id=\"sbkUpav_ListTable\">");
                        Output.WriteLine("    <tr>");


                        // A bunch of code here just to write the admins in up to three columns and 
                        // start the table
                        int column = 1;
                        if (hostAdmins.Count > 0)
                        {
                            Output.WriteLine("      <td style=\"width:33%\">");
                            add_user_list(Output, hostAdmins, "Host Administrators", false);
                            Output.WriteLine("      </td>");
                            column++;
                        }

                        if (sysAdmins.Count > 0)
                        {
                            Output.WriteLine("      <td style=\"width:33%\">");
                            add_user_list(Output, sysAdmins, "System Administrators", false);
                            Output.WriteLine("      </td>");
                            column++;
                        }

                        if (portalAdmins.Count > 0)
                        {
                            Output.WriteLine("      <td style=\"width:33%\">");
                            add_user_list(Output, portalAdmins, "Portal Administrators", false);
                            Output.WriteLine("      </td>");
                            column++;
                        }

                        while (column <= 3)
                        {
                            Output.WriteLine("      <td style=\"width:33%\">&nbsp;</td>");
                            column++;
                        }

                        Output.WriteLine("    </tr>");

                        // If all of these are less than 25, than just show the lists
                        if ((canDeleteAll.Count <= 25) && (internalUsers.Count <= 25) && (canEditAll.Count <= 25))
                        {
                            Output.WriteLine("    <tr>");

                            column = 1;
                            if (canDeleteAll.Count > 0)
                            {
                                Output.WriteLine("      <td>");
                                add_user_list(Output, canDeleteAll, "Can Delete All Items", false);
                                Output.WriteLine("      </td>");
                                column++;
                            }

                            if (internalUsers.Count > 0)
                            {
                                Output.WriteLine("      <td>");
                                add_user_list(Output, internalUsers, "Power Users", false);
                                Output.WriteLine("      </td>");
                                column++;
                            }

                            if (canEditAll.Count > 0)
                            {
                                Output.WriteLine("      <td>");
                                add_user_list(Output, canEditAll, "Can Edit All Items", false);
                                Output.WriteLine("      </td>");
                                column++;
                            }

                            while (column <= 3)
                            {
                                Output.WriteLine("      <td>&nbsp;</td>");
                                column++;
                            }

                            Output.WriteLine("    </tr>");
                        }
                        else
                        {
                            // Allow these to all be multi-column then
                            if (canDeleteAll.Count > 0) add_user_list(Output, canDeleteAll, "Can Delete All Items", true);
                            if (internalUsers.Count > 0) add_user_list(Output, internalUsers, "Power Users", true);
                            if (canEditAll.Count > 0) add_user_list(Output, canEditAll, "Can Edit All Items", true);
                        }

                        Output.WriteLine("  </table>");
                    }
                    break;

                case 2:

                    // Try to get the global permissions table
                    DataTable globalPermissions2 = HttpContext.Current.Cache["GlobalPermissionsReport"] as DataTable;
                    if (globalPermissions2 == null)
                    {
                        globalPermissions2 = SobekCM_Database.Get_Global_User_Permissions(RequestSpecificValues.Tracer);
                        if (globalPermissions2 == null)
                        {
                            actionMessage = "Error pulling global user permission!";
                        }
                        else
                        {
                            HttpContext.Current.Cache.Insert("GlobalPermissionsReport", globalPermissions2, null, DateTime.Now.AddSeconds(60d), Cache.NoSlidingExpiration);
                        }
                    }

                    if (globalPermissions2 != null)
                    {
                        Output.WriteLine("<p style=\"width:800px;\">The table below shows the global permissions assigned to different registered users, either individually assigned or assigned thorugh user group membership.</p>");
                        Output.WriteLine("<p style=\"width:800px;\">Only users with some global permissions appears in the list below.</p>");
                        Output.WriteLine("<p style=\"width:800px;\">Select a user from the table below to edit the permissions on that individual user.</p>");


                        bool includeHostAdmin = false;
                        foreach (DataRow thisUser in globalPermissions2.Rows)
                        {
                            if (Convert.ToBoolean(thisUser["IsHostAdmin"].ToString()))
                            {
                                includeHostAdmin = true;
                                break;
                            }
                        }

                        Output.WriteLine("  <table id=\"sbkUpav_Table\">");
                        Output.WriteLine("    <thead>");
                        Output.WriteLine("      <tr>");
                        Output.WriteLine("        <th>Name</th>");
                        Output.WriteLine("        <th>Created</th>");
                        Output.WriteLine("        <th>Last Activity</th>");
                        Output.WriteLine("        <th style=\"text-align:center\">Can Edit<br />All Items</th>");
                        Output.WriteLine("        <th style=\"text-align:center\">Power<br />User</th>");
                        Output.WriteLine("        <th style=\"text-align:center\">Can Delete<br />All Items</th>");
                        Output.WriteLine("        <th style=\"text-align:center\">Portal<br />Admin</th>");
                        Output.WriteLine("        <th style=\"text-align:center\">System<br />Admin</th>");
                        if (includeHostAdmin)
                            Output.WriteLine("      <th style=\"text-align:center\">Host<br />Admin</th>");
                        Output.WriteLine("      </tr>");
                        Output.WriteLine("    </thead>");

                        Output.WriteLine("    <tfoot>");
                        Output.WriteLine("      <tr>");
                        Output.WriteLine("        <th><input id=\"reportNameSearch\"  type=\"text\" placeholder=\"Search Name\" /></th>");
                        Output.WriteLine("        <th>Created</th>");
                        Output.WriteLine("        <th>Last Activity</th>");
                        Output.WriteLine("        <th style=\"text-align:center\"></th>");
                        Output.WriteLine("        <th style=\"text-align:center\"></th>");
                        Output.WriteLine("        <th style=\"text-align:center\"></th>");
                        Output.WriteLine("        <th style=\"text-align:center\"></th>");
                        Output.WriteLine("        <th style=\"text-align:center\"></th>");
                        if (includeHostAdmin)
                            Output.WriteLine("      <th style=\"text-align:center\"></th>");
                        Output.WriteLine("      </tr>");
                        Output.WriteLine("    </tfoot>");


                        Output.WriteLine("    <tbody>");

                        int last_userid = Convert.ToInt32(globalPermissions2.Rows[0]["UserID"].ToString());
                        bool canEdit = false;
                        bool powerUser = false;
                        bool canDelete = false;
                        bool portalAdmin = false;
                        bool sysAdmin = false;
                        bool hostAdmin = false;
                        string firstName = String.Empty;
                        string lastName = String.Empty;
                        string dateCreated = String.Empty;
                        string lastActvity = String.Empty;
                        int userCount = 1;
                        foreach (DataRow thisRow in globalPermissions2.Rows)
                        {
                            int this_userid = Convert.ToInt32(thisRow["UserID"].ToString());
                            if (this_userid != last_userid)
                            {
                                // Add this row
                                Output.WriteLine("      <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userAdminUrl.Replace("Xyzzy", last_userid.ToString()) + "', '_UserEdit+ " + last_userid + "');\">");
                                Output.WriteLine("        <td>" + lastName + ", " + firstName + "</td>");
                                Output.WriteLine("        <td>" + dateCreated + "</td>");
                                Output.WriteLine("        <td>" + lastActvity + "</td>");
                                Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(canEdit) + "</td>");
                                Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(powerUser) + "</td>");
                                Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(canDelete) + "</td>");
                                Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(portalAdmin) + "</td>");
                                Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(sysAdmin) + "</td>");
                                if (includeHostAdmin)
                                    Output.WriteLine("      <td style=\"text-align:center\">" + bool_to_char(hostAdmin) + "</td>");
                                Output.WriteLine("      </tr>");

                                // Clear the flags
                                canEdit = false;
                                powerUser = false;
                                canDelete = false;
                                portalAdmin = false;
                                sysAdmin = false;
                                hostAdmin = false;

                                // Save the user id 
                                last_userid = this_userid;

                                // Increment the number of users
                                userCount++;
                            }

                            firstName = thisRow["FirstName"].ToString();
                            lastName = thisRow["LastName"].ToString();
                            if (thisRow["DateCreated"] != DBNull.Value)
                                dateCreated = Convert.ToDateTime(thisRow["DateCreated"]).ToShortDateString();
                            if (thisRow["LastActivity"] != DBNull.Value)
                                lastActvity = Convert.ToDateTime(thisRow["LastActivity"]).ToShortDateString();
                            if (Convert.ToBoolean(thisRow["Can_Edit_All_Items"].ToString())) canEdit = true;
                            if (Convert.ToBoolean(thisRow["Internal_User"].ToString())) powerUser = true;
                            if (Convert.ToBoolean(thisRow["Can_Delete_All_Items"].ToString())) canDelete = true;
                            if (Convert.ToBoolean(thisRow["IsPortalAdmin"].ToString())) portalAdmin = true;
                            if (Convert.ToBoolean(thisRow["IsSystemAdmin"].ToString())) sysAdmin = true;
                            if (Convert.ToBoolean(thisRow["IsHostAdmin"].ToString())) hostAdmin = true;
                        }

                        // Add the last row
                        Output.WriteLine("      <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userAdminUrl.Replace("Xyzzy", last_userid.ToString()) + "', '_UserEdit+ " + last_userid + "');\">");
                        Output.WriteLine("        <td>" + lastName + ", " + firstName + "</td>");
                        Output.WriteLine("        <td>" + dateCreated + "</td>");
                        Output.WriteLine("        <td>" + lastActvity + "</td>");
                        Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(canEdit) + "</td>");
                        Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(powerUser) + "</td>");
                        Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(canDelete) + "</td>");
                        Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(portalAdmin) + "</td>");
                        Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(sysAdmin) + "</td>");
                        if (includeHostAdmin)
                            Output.WriteLine("      <td style=\"text-align:center\">" + bool_to_char(hostAdmin) + "</td>");
                        Output.WriteLine("      </tr>");


                        Output.WriteLine("    </tbody>");


                        Output.WriteLine("  </table>");

                        Output.WriteLine("<script type=\"text/javascript\">");
                        Output.WriteLine("    $(document).ready(function() { ");
                        Output.WriteLine("        var table = $('#sbkUpav_Table').DataTable({ ");

                        // If 100 or less users, suppress paging
                        if (userCount <= 100)
                        {
                            Output.WriteLine("            \"paging\":   false, ");
                            Output.WriteLine("            \"info\":   false, ");
                        }
                        else
                        {
                            Output.WriteLine("            \"lengthMenu\": [ [50, 100, -1], [50, 100, \"All\"] ], ");
                            Output.WriteLine("            \"pageLength\":  50, ");
                        }

                        Output.WriteLine("            initComplete: function () {");
                        Output.WriteLine("                var api = this.api();");

                        Output.WriteLine("                api.columns().indexes().flatten().each( function ( i ) {");
                        Output.WriteLine("                    if (( i > 2 )) {");
                        Output.WriteLine("                        var column = api.column( i );");
                        Output.WriteLine("                        var select = $('<select><option value=\"\"></option></select>')");
                        Output.WriteLine("                                 .appendTo( $(column.footer()).empty() )");
                        Output.WriteLine("                                 .on( 'change', function () {");
                        Output.WriteLine("                                       var val = $.fn.dataTable.util.escapeRegex($(this).val());");
                        Output.WriteLine("                                       column.search( val ? '^'+val+'$' : '', true, false ).draw();");
                        Output.WriteLine("                                        } );");
                        Output.WriteLine("                                 column.data().unique().sort().each( function ( d, j ) {");
                        Output.WriteLine("                                      select.append( '<option value=\"'+d+'\">'+d+'</option>' )");
                        Output.WriteLine("                                 } );");
                        Output.WriteLine("                        }");
                        Output.WriteLine("                    } );");
                        Output.WriteLine("               }");
                        Output.WriteLine("         });");

                        Output.WriteLine("         $('#reportNameSearch').on( 'keyup change', function () { table.column( 0 ).search( this.value ).draw(); } );");
                        Output.WriteLine("    } );");

                        Output.WriteLine("</script>");
                        Output.WriteLine();
                    }

                    break;

                case 3:
                    // Try to get the list of users linked to aggregations
                    DataTable usersLinked = HttpContext.Current.Cache["GlobalPermissionsUsersLinked"] as DataTable;
                    if (usersLinked == null)
                    {
                        usersLinked = SobekCM_Database.Get_Global_User_Permissions_Aggregations_Links(RequestSpecificValues.Tracer);
                        if (usersLinked == null)
                        {
                            actionMessage = "Error pulling global user permission!";
                        }
                        else
                        {
                            HttpContext.Current.Cache.Insert("GlobalPermissionsUsersLinked", usersLinked, null, DateTime.Now.AddSeconds(60d), Cache.NoSlidingExpiration);
                        }
                    }

                    // Show all the users that are linked (via special permissions) to aggregations
                    if (usersLinked != null)
                    {
                        Output.WriteLine("<p style=\"width:800px;\">The table below shows which users have been given special rights on one or more aggregations, either individually assigned or assigned thorugh user group membership.</p>");
                        Output.WriteLine("<p style=\"width:800px;\">Select a user from the table below to edit the permissions on that individual user.</p>");

                        Output.WriteLine("  <table id=\"sbkUpav_Table\">");
                        Output.WriteLine("    <thead>");
                        Output.WriteLine("      <tr>");
                        Output.WriteLine("        <th>Name</th>");
                        Output.WriteLine("        <th>Created</th>");
                        Output.WriteLine("        <th>Last Activity</th>");
                        Output.WriteLine("        <th style=\"min-width:150px\">Aggregations (User Specified)</th>");
                        Output.WriteLine("        <th style=\"min-width:150px\">Aggregations (Group Specified)</th>");
                        Output.WriteLine("      </tr>");
                        Output.WriteLine("    </thead>");

                        Output.WriteLine("    <tfoot>");
                        Output.WriteLine("      <tr>");
                        Output.WriteLine("        <th><input id=\"reportNameSearch\"  type=\"text\" placeholder=\"Search Name\" /></th>");
                        Output.WriteLine("        <th>Created</th>");
                        Output.WriteLine("        <th>Last Activity</th>");
                        Output.WriteLine("        <th><input id=\"userLinkedSearch\"  type=\"text\" placeholder=\"Search User Links\" /></th>");
                        Output.WriteLine("        <th><input id=\"groupLinkedSearch\"  type=\"text\" placeholder=\"Search Group Links\" /></th>");
                        Output.WriteLine("      </tr>");
                        Output.WriteLine("    </tfoot>");


                        Output.WriteLine("    <tbody>");
                        int userCount = 0;
                        foreach (DataRow thisRow in usersLinked.Rows)
                        {
                            // Initialize some values
                            string dateCreated = String.Empty;
                            string lastActivity = String.Empty;

                            // Get the UserID
                            int this_userid = Convert.ToInt32(thisRow["UserID"].ToString());

                            // Get the two date values
                            if (thisRow["DateCreated"] != DBNull.Value)
                                dateCreated = Convert.ToDateTime(thisRow["DateCreated"]).ToShortDateString();
                            if (thisRow["LastActivity"] != DBNull.Value)
                                lastActivity = Convert.ToDateTime(thisRow["LastActivity"]).ToShortDateString();

                            // Add this row
                            Output.WriteLine("      <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userAdminUrl.Replace("Xyzzy", this_userid.ToString()) + "', '_UserEdit+ " + this_userid + "');\">");

                            Output.WriteLine("        <td>" + thisRow["LastName"] + ", " + thisRow["FirstName"] + "</td>");
                            Output.WriteLine("        <td>" + dateCreated + "</td>");
                            Output.WriteLine("        <td>" + lastActivity + "</td>");
                            Output.WriteLine("        <td>" + thisRow["UserPermissioned"] + "</td>");
                            Output.WriteLine("        <td>" + thisRow["GroupPermissioned"] + "</td>");
                            Output.WriteLine("      </tr>");

                            // Increment the number of users
                            userCount++;
                        }

                        Output.WriteLine("    </tbody>");
                        Output.WriteLine("  </table>");

                        Output.WriteLine("<script type=\"text/javascript\">");
                        Output.WriteLine("    $(document).ready(function() { ");
                        Output.WriteLine("        var table = $('#sbkUpav_Table').DataTable({ ");

                        // If 100 or less users, suppress paging
                        if (userCount <= 100)
                        {
                            Output.WriteLine("            \"paging\":   false, ");
                            Output.WriteLine("            \"info\":   false });");
                        }
                        else
                        {
                            Output.WriteLine("            \"lengthMenu\": [ [50, 100, -1], [50, 100, \"All\"] ], ");
                            Output.WriteLine("            \"pageLength\":  50 });");
                        }


                        Output.WriteLine("         $('#reportNameSearch').on( 'keyup change', function () { table.column( 0 ).search( this.value ).draw(); } );");
                        Output.WriteLine("         $('#userLinkedSearch').on( 'keyup change', function () { table.column( 3 ).search( this.value ).draw(); } );");
                        Output.WriteLine("         $('#groupLinkedSearch').on( 'keyup change', function () { table.column( 4 ).search( this.value ).draw(); } );");

                        Output.WriteLine("    } );");

                        Output.WriteLine("</script>");
                        Output.WriteLine();
                    }
                    break;

                case 4:
                    // First, try to get the list of aggregations that have special permissioned users linked to them
                    DataTable linkedAggrs = HttpContext.Current.Cache["GlobalPermissionsLinkedAggr"] as DataTable;
                    if (linkedAggrs == null)
                    {
                        linkedAggrs = SobekCM_Database.Get_Global_User_Permissions_Linked_Aggregations(RequestSpecificValues.Tracer);
                        if (linkedAggrs == null)
                        {
                            actionMessage = "Error pulling list of aggregations that have specially permissioned users attached!";
                        }
                        else
                        {
                            HttpContext.Current.Cache.Insert("GlobalPermissionsLinkedAggr", linkedAggrs, null, DateTime.Now.AddSeconds(60d), Cache.NoSlidingExpiration);
                        }
                    }

                    // Now, was an aggregation selected
                    string current_aggr_code = String.Empty;
                    if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["aggr"]))
                    {
                        current_aggr_code = HttpContext.Current.Request.QueryString["aggr"];
                    }

                    // Get the current URL
                    string aggr_search_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                    if (aggr_search_url.IndexOf("?") > 0)
                        aggr_search_url = aggr_search_url + "&aggr=";
                    else
                        aggr_search_url = aggr_search_url + "?aggr=";

                    Output.WriteLine("<p style=\"width:800px;\">Select an aggregation below to view the granted aggregation-specific permissions.</p>");
                    Output.WriteLine("<p style=\"width:800px;\">Only aggregations with aggregation-specific permissions will show in the select box below.  So, if you do not see an aggregation you are looking for, it must have no aggregation-specific permissions granted to any users.</p>");

                    // Add the select optoin
                    Output.WriteLine("  <table class=\"sbkPrav_SelectAggr\">");
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <td>Select Aggregation:</td>");
                    Output.WriteLine("      <td>");

                    Output.WriteLine("        <select id=\"sbkPrav_AggrSelect\" name=\"sbkPrav_AggrSelect\" onchange=\"window.location.href='" + aggr_search_url + "' + this.value;\">");

                    if (current_aggr_code.Length == 0)
                        Output.WriteLine("          <option value=\"\" selected=\"selected\"></option>");
                    string collection_name = String.Empty;

                    if (linkedAggrs != null)
                    {
                        foreach (DataRow thisRow in linkedAggrs.Rows)
                        {
                            string display_code = thisRow["Code"].ToString().ToUpper();
                            string thisType = thisRow["Type"].ToString();
                            if ((display_code[0] == 'I') && (thisType.IndexOf("Institution", StringComparison.InvariantCultureIgnoreCase) >= 0))
                            {
                                display_code = "i" + display_code.Substring(1);
                            }

                            if (String.Compare(thisRow["Code"].ToString(), current_aggr_code, StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                Output.WriteLine("          <option value=\"" + thisRow["Code"].ToString().ToLower() + "\" selected=\"selected\">" + display_code + " - " + thisRow["Name"] + "</option>");
                                collection_name = thisRow["Name"].ToString();
                            }
                            else
                                Output.WriteLine("          <option value=\"" + thisRow["Code"].ToString().ToLower() + "\">" + display_code + " - " + thisRow["Name"] + "</option>");

                        }
                    }

                    Output.WriteLine("        </select>");

                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                    Output.WriteLine("  </table>");

                    if (!String.IsNullOrEmpty(current_aggr_code))
                    {
                        DataTable permissionsTbl = SobekCM_Database.Get_Aggregation_User_Permissions(current_aggr_code, RequestSpecificValues.Tracer);

                        if ((permissionsTbl == null) || (permissionsTbl.Rows.Count == 0))
                        {
                            Output.WriteLine("No user permissions are assigned to this collection.");
                        }
                        else
                        {

                            // Is this a system administrator?
                            bool isSysAdmin = ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Host_Admin));

                            // If no permissions received, just show a message
                            if ((permissionsTbl == null) || (permissionsTbl.Rows.Count == 0))
                            {
                                Output.WriteLine("<p style=\"width:800px;\">No special user permissions found for this collection.</p>");

                                if (isSysAdmin)
                                {
                                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
                                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;

                                    Output.WriteLine("<p>Use the <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">administrative Users &amp; Groups</a> to assign collection-specific user permissions.");

                                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                                }

                                Output.WriteLine("  <br /><br />");

                                return;
                            }

                            Output.WriteLine("  <br /><br />");
                            Output.WriteLine("<h3>" + collection_name + "</h3>");
                            Output.WriteLine("<p style=\"width:800px;\">Below is the list of all users that have specialized user permissions for this collection.  These permissions may be assigned individually, or through a user group.</p>");

                            // Show a message about selecting the user below to edit them
                            Output.WriteLine("<p style=\"width:800px;\">elect a user from the list below to edit that user's permissions.</p>");

                            // Is this using detailed permissions?
                            bool detailedPermissions = UI_ApplicationCache_Gateway.Settings.System.Detailed_User_Aggregation_Permissions;

                            Output.WriteLine("  <table id=\"sbkPrav_DetailedUsers\">");
                            Output.WriteLine("  <thead>");
                            Output.WriteLine("    <tr>");
                            Output.WriteLine("      <th style=\"width:180px;\">User</th>");
                            Output.WriteLine("      <th style=\"width:90px;\"><acronym title=\"Can select this aggregation when editing or submitting an item\">Can<br />Select</acronym></th>");

                            if (detailedPermissions)
                            {
                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Edit<br />Metadata</acronym></th>");
                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Edit<br />Behaviors</acronym></th>");
                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Perform<br />QC</acronym></th>");
                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Upload<br />Files</acronym></th>");
                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Change<br />Visibility</acronym></th>");
                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Can<br />Delete</acronym></th>");

                            }
                            else
                            {
                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit any item in this aggregation\">Can<br />Edit</acronym></th>");
                            }

                            Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">Is<br />Curator</acronym></th>");
                            Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">Is<br />Admin</acronym></th>");
                            Output.WriteLine("    </tr>");
                            Output.WriteLine("  </thead>");
                            Output.WriteLine("  <tbody>");

                            // Collect the relevant user group rows, if some permissions were assined by user group
                            SortedDictionary<string, DataRow> userGroupRows = new SortedDictionary<string, DataRow>();

                            // Users that are attached to user groups may have multiple rows with their name, so collect
                            // all the user information from all rows before displaying
                            int last_userid = -1;
                            string username = String.Empty;
                            bool canSelect = false;
                            bool canEditMetadata = false;
                            bool canEditBehaviors = false;
                            bool canPerformQc = false;
                            bool canUploadFiles = false;
                            bool canChangeVisibility = false;
                            bool canDelete = false;
                            bool isCurator = false;
                            bool isAdmin = false;
                            foreach (DataRow thisUser in permissionsTbl.Rows)
                            {
                                // Get the user id for this user row
                                int thisUserId = Convert.ToInt32(thisUser["UserID"].ToString());

                                // See if this is a new user, to be displayed
                                if ((last_userid > 0) && (last_userid != thisUserId))
                                {
                                    // Show the information for this user
                                    Output.WriteLine("    <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userAdminUrl.Replace("Xyzzy", thisUserId.ToString()) + "', '_UserEdit+ " + thisUserId + "');\">");

                                    Output.WriteLine("      <td>" + username + "</td>");
                                    Output.WriteLine("      <td>" + flag_to_display(canSelect) + "</td>");
                                    if (detailedPermissions)
                                    {
                                        Output.WriteLine("      <td>" + flag_to_display(canEditMetadata) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(canEditBehaviors) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(canPerformQc) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(canUploadFiles) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(canChangeVisibility) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(canDelete) + "</td>");
                                    }
                                    else
                                    {
                                        Output.WriteLine("      <td>" + flag_to_display(canEditMetadata) + "</td>");
                                    }


                                    Output.WriteLine("      <td>" + flag_to_display(isCurator) + "</td>");
                                    Output.WriteLine("      <td>" + flag_to_display(isAdmin) + "</td>");

                                    Output.WriteLine("    </tr>");

                                    // Prepare to collect the information about this user
                                    canSelect = false;
                                    canEditMetadata = false;
                                    canEditBehaviors = false;
                                    canPerformQc = false;
                                    canUploadFiles = false;
                                    canChangeVisibility = false;
                                    canDelete = false;
                                    isCurator = false;
                                    isAdmin = false;
                                }
                                last_userid = thisUserId;

                                // Collect the flags from this row
                                username = thisUser["LastName"] + "," + thisUser["FirstName"];
                                if ((thisUser["CanSelect"] != DBNull.Value) && (Convert.ToBoolean(thisUser["CanSelect"]))) canSelect = true;
                                if ((thisUser["CanEditMetadata"] != DBNull.Value) && (Convert.ToBoolean(thisUser["CanEditMetadata"]))) canEditMetadata = true;
                                if ((thisUser["CanEditBehaviors"] != DBNull.Value) && (Convert.ToBoolean(thisUser["CanEditBehaviors"]))) canEditBehaviors = true;
                                if ((thisUser["CanPerformQc"] != DBNull.Value) && (Convert.ToBoolean(thisUser["CanPerformQc"]))) canPerformQc = true;
                                if ((thisUser["CanUploadFiles"] != DBNull.Value) && (Convert.ToBoolean(thisUser["CanUploadFiles"]))) canUploadFiles = true;
                                if ((thisUser["CanChangeVisibility"] != DBNull.Value) && (Convert.ToBoolean(thisUser["CanChangeVisibility"]))) canChangeVisibility = true;
                                if ((thisUser["CanDelete"] != DBNull.Value) && (Convert.ToBoolean(thisUser["CanDelete"]))) canDelete = true;
                                if ((thisUser["IsCollectionManager"] != DBNull.Value) && (Convert.ToBoolean(thisUser["IsCollectionManager"]))) isCurator = true;
                                if ((thisUser["IsAggregationAdmin"] != DBNull.Value) && (Convert.ToBoolean(thisUser["IsAggregationAdmin"]))) isAdmin = true;

                                // If this is from a user group, save that row as well
                                if ((thisUser["GroupName"] != DBNull.Value) && (thisUser["GroupName"].ToString().Length > 0))
                                {
                                    string groupName = thisUser["GroupName"].ToString();
                                    if (!userGroupRows.ContainsKey(groupName))
                                    {
                                        userGroupRows[groupName] = thisUser;
                                    }
                                }
                            }

                            // Show the information for the last user
                            Output.WriteLine("    <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userAdminUrl.Replace("Xyzzy", last_userid.ToString()) + "', '_UserEdit+ " + last_userid + "');\">");
                            Output.WriteLine("      <td>" + username + "</td>");
                            Output.WriteLine("      <td>" + flag_to_display(canSelect) + "</td>");
                            if (detailedPermissions)
                            {
                                Output.WriteLine("      <td>" + flag_to_display(canEditMetadata) + "</td>");
                                Output.WriteLine("      <td>" + flag_to_display(canEditBehaviors) + "</td>");
                                Output.WriteLine("      <td>" + flag_to_display(canPerformQc) + "</td>");
                                Output.WriteLine("      <td>" + flag_to_display(canUploadFiles) + "</td>");
                                Output.WriteLine("      <td>" + flag_to_display(canChangeVisibility) + "</td>");
                                Output.WriteLine("      <td>" + flag_to_display(canDelete) + "</td>");
                            }
                            else
                            {
                                Output.WriteLine("      <td>" + flag_to_display(canEditMetadata) + "</td>");
                            }


                            Output.WriteLine("      <td>" + flag_to_display(isCurator) + "</td>");
                            Output.WriteLine("      <td>" + flag_to_display(isAdmin) + "</td>");

                            Output.WriteLine("    </tr>");
                            Output.WriteLine("  <tbody>");
                            Output.WriteLine("  </table>");
                            Output.WriteLine("  <br /><br />");

                            Output.WriteLine("<script type=\"text/javascript\">");
                            Output.WriteLine("    $(document).ready(function() { ");
                            Output.WriteLine("        var table = $('#sbkPrav_DetailedUsers').DataTable({ ");
                            Output.WriteLine("            \"paging\":   false, ");
                            Output.WriteLine("            \"filter\":   false, ");
                            Output.WriteLine("            \"info\":   false });");
                            Output.WriteLine("    } );");
                            Output.WriteLine("</script>");


                            // If there were user groups, add them now also.
                            if (userGroupRows.Count > 0)
                            {
                                Output.WriteLine("<p style=\"text-align: left; padding:0 20px 0 20px;\">Some of the permissions above are assigned to the users through user groups.  These user groups and their permissions appear below:</p>");


                                Output.WriteLine("  <table id=\"sbkPrav_DetailedUserGroups\">");
                                Output.WriteLine("  <thead>");
                                Output.WriteLine("    <tr>");
                                Output.WriteLine("      <th style=\"width:180px;\">User Group</th>");
                                Output.WriteLine("      <th style=\"width:90px;\"><acronym title=\"Can select this aggregation when editing or submitting an item\">Can<br />Select</acronym></th>");

                                if (detailedPermissions)
                                {
                                    Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Edit<br />Metadata</acronym></th>");
                                    Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Edit<br />Behaviors</acronym></th>");
                                    Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Perform<br />QC</acronym></th>");
                                    Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Upload<br />Files</acronym></th>");
                                    Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Change<br />Visibility</acronym></th>");
                                    Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">Item<br />Can<br />Delete</acronym></th>");

                                }
                                else
                                {
                                    Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can edit any item in this aggregation\">Can<br />Edit</acronym></th>");
                                }

                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">Is<br />Curator</acronym></th>");
                                Output.WriteLine("      <th style=\"width:85px;\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">Is<br />Admin</acronym></th>");
                                Output.WriteLine("    </tr>");
                                Output.WriteLine("  </thead>");
                                Output.WriteLine("  <tbody>");


                                foreach (KeyValuePair<string, DataRow> userGroupRow in userGroupRows)
                                {
                                    DataRow thisUser = userGroupRow.Value;
                                    int userGroupId = Convert.ToInt32(thisUser["UserGroupID"]);
                                    Output.WriteLine("    <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userGroupAdminUrl.Replace("Xyzzy", userGroupId.ToString()) + "', '_UserGroupEdit+ " + userGroupId + "');\">");

                                    Output.WriteLine("      <td>" + userGroupRow.Key + "</td>");
                                    Output.WriteLine("      <td>" + flag_to_display(thisUser["CanSelect"]) + "</td>");
                                    if (detailedPermissions)
                                    {
                                        Output.WriteLine("      <td>" + flag_to_display(thisUser["CanEditMetadata"]) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(thisUser["CanEditBehaviors"]) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(thisUser["CanPerformQc"]) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(thisUser["CanUploadFiles"]) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(thisUser["CanChangeVisibility"]) + "</td>");
                                        Output.WriteLine("      <td>" + flag_to_display(thisUser["CanDelete"]) + "</td>");
                                    }
                                    else
                                    {
                                        Output.WriteLine("      <td>" + flag_to_display(thisUser["CanEditMetadata"]) + "</td>");
                                    }


                                    Output.WriteLine("      <td>" + flag_to_display(thisUser["IsCollectionManager"]) + "</td>");
                                    Output.WriteLine("      <td>" + flag_to_display(thisUser["IsAggregationAdmin"]) + "</td>");

                                    Output.WriteLine("    </tr>");
                                    //Output.WriteLine("    <tr class=\"sbkWhav_TableRule\"><td colspan=\"" + columns + "\"></td></tr>");
                                }

                                Output.WriteLine("  </tbody>");
                                Output.WriteLine("  </table>");
                                Output.WriteLine("  <br /><br />");

                                Output.WriteLine("<script type=\"text/javascript\">");
                                Output.WriteLine("    $(document).ready(function() { ");
                                Output.WriteLine("        var table2 = $('#sbkPrav_DetailedUserGroups').DataTable({ ");
                                Output.WriteLine("            \"paging\":   false, ");
                                Output.WriteLine("            \"filter\":   false, ");
                                Output.WriteLine("            \"info\":   false });");
                                Output.WriteLine("    } );");
                                Output.WriteLine("</script>");

                            }

                        }

                    }


                    break;

                case 5:
                    // Try to get the global can submit permissions table
                    DataTable userSubmitPermit = HttpContext.Current.Cache["GlobalPermissionsReportSubmit"] as DataTable;
                    if (userSubmitPermit == null)
                    {
                        userSubmitPermit = SobekCM_Database.Get_Global_User_Permissions_Submission_Rights(RequestSpecificValues.Tracer);
                        if (userSubmitPermit == null)
                        {
                            actionMessage = "Error pulling global list of user that can submit items!";
                        }
                        else
                        {
                            HttpContext.Current.Cache.Insert("GlobalPermissionsReportSubmit", userSubmitPermit, null, DateTime.Now.AddSeconds(60d), Cache.NoSlidingExpiration);
                        }
                    }

                    if (userSubmitPermit != null)
                    {
                        Output.WriteLine("<p style=\"width:800px;\">Users with permission to submit materials to this instance appear below.</p>");
                        Output.WriteLine("<p style=\"width:800px;\">Select a user from the table below to edit the permissions on that individual user.</p>");



                        Output.WriteLine("  <table id=\"sbkUpav_Table\">");
                        Output.WriteLine("    <thead>");
                        Output.WriteLine("      <tr>");
                        Output.WriteLine("        <th>Name</th>");
                        Output.WriteLine("        <th>Created</th>");
                        Output.WriteLine("        <th>Last Activity</th>");
                        Output.WriteLine("        <th style=\"min-width:150px\">Templates</th>");
                        Output.WriteLine("        <th style=\"min-width:150px\">Default Metadata</th>");
                        Output.WriteLine("      </tr>");
                        Output.WriteLine("    </thead>");

                        Output.WriteLine("    <tfoot>");
                        Output.WriteLine("      <tr>");
                        Output.WriteLine("        <th><input id=\"reportNameSearch\"  type=\"text\" placeholder=\"Search Name\" /></th>");
                        Output.WriteLine("        <th>Created</th>");
                        Output.WriteLine("        <th>Last Activity</th>");
                        Output.WriteLine("        <th><input id=\"templatesSearch\"  type=\"text\" placeholder=\"Search Templates\" /></th>");
                        Output.WriteLine("        <th><input id=\"defaultMetadataSearch\"  type=\"text\" placeholder=\"Search Default Metadata\" /></th>");
                        Output.WriteLine("      </tr>");
                        Output.WriteLine("    </tfoot>");


                        Output.WriteLine("    <tbody>");
                        int userCount = 0;
                        foreach (DataRow thisRow in userSubmitPermit.Rows)
                        {
                            // Initialize some values
                            string dateCreated = String.Empty;
                            string lastActivity = String.Empty;

                            // Get the UserID
                            int this_userid = Convert.ToInt32(thisRow["UserID"].ToString());

                            // Get the two date values
                            if (thisRow["DateCreated"] != DBNull.Value)
                                dateCreated = Convert.ToDateTime(thisRow["DateCreated"]).ToShortDateString();
                            if (thisRow["LastActivity"] != DBNull.Value)
                                lastActivity = Convert.ToDateTime(thisRow["LastActivity"]).ToShortDateString();

                            // Add this row
                            // Add this row
                            Output.WriteLine("      <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userAdminUrl.Replace("Xyzzy", this_userid.ToString()) + "', '_UserEdit+ " + this_userid + "');\">");

                            Output.WriteLine("        <td>" + thisRow["LastName"] + ", " + thisRow["FirstName"] + "</td>");
                            Output.WriteLine("        <td>" + dateCreated + "</td>");
                            Output.WriteLine("        <td>" + lastActivity + "</td>");
                            Output.WriteLine("        <td>" + thisRow["Templates"] + "</td>");
                            Output.WriteLine("        <td>" + thisRow["DefaultMetadatas"] + "</td>");
                            Output.WriteLine("      </tr>");

                            // Increment the number of users
                            userCount++;
                        }

                        Output.WriteLine("    </tbody>");
                        Output.WriteLine("  </table>");

                        Output.WriteLine("<script type=\"text/javascript\">");
                        Output.WriteLine("    $(document).ready(function() { ");
                        Output.WriteLine("        var table = $('#sbkUpav_Table').DataTable({ ");

                        // If 100 or less users, suppress paging
                        if (userCount <= 100)
                        {
                            Output.WriteLine("            \"paging\":   false, ");
                            Output.WriteLine("            \"info\":   false });");
                        }
                        else
                        {
                            Output.WriteLine("            \"lengthMenu\": [ [50, 100, -1], [50, 100, \"All\"] ], ");
                            Output.WriteLine("            \"pageLength\":  50 });");
                        }


                        Output.WriteLine("         $('#reportNameSearch').on( 'keyup change', function () { table.column( 0 ).search( this.value ).draw(); } );");
                        Output.WriteLine("         $('#templatesSearch').on( 'keyup change', function () { table.column( 3 ).search( this.value ).draw(); } );");
                        Output.WriteLine("         $('#defaultMetadataSearch').on( 'keyup change', function () { table.column( 4 ).search( this.value ).draw(); } );");

                        Output.WriteLine("    } );");

                        Output.WriteLine("</script>");
                        Output.WriteLine();
                    }
                    break;
            }


            //// Add the buttons
            //RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            //Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            //Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
            //Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            //Output.WriteLine("  </div>");

            Output.WriteLine();

            Output.WriteLine("</div>");
            Output.WriteLine("</div>");
            Output.WriteLine("</div>");

            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
        }

        private string flag_to_display(bool ToDisplay)
        {
            if (ToDisplay)
                return "Y";
            return "";
        }

        private string flag_to_display(object ToDisplay)
        {
            if ((ToDisplay != DBNull.Value) && (Convert.ToBoolean(ToDisplay)))
                return "Y";
            return "";
        }

        private string bool_to_char(bool Value)
        {
            return Value ? "Y" : "";
        }

        private void add_user_list(TextWriter Output, List<string> UserList, string Title, bool Multicolumn )
        {
            if (!Multicolumn)
            {
                Output.WriteLine("        <h3>" + Title + "</h3>");
                Output.WriteLine("        <blockquote>");

                foreach (string thisUser in UserList)
                    Output.WriteLine("          " + thisUser + "<br />");

                Output.WriteLine("      </blockquote>");
                Output.WriteLine("      <br />");
            }
            else
            {
                Output.WriteLine("    <tr><td colspan=\"3\"><h3>" + Title + "</h3></td></tr>");
                Output.WriteLine("    <tr>");



                // How many rows?
                int rows = ((UserList.Count - 1)/3) + 1;
                Output.WriteLine("      <td>");
                Output.WriteLine("        <blockquote>");
                for (int i = 0; i < rows; i++)
                    Output.WriteLine("          " + UserList[i] + "<br />");
                Output.WriteLine("        </blockquote>");
                Output.WriteLine("        <br />");
                Output.WriteLine("      </td>");

                Output.WriteLine("      <td>");
                Output.WriteLine("        <blockquote>");
                for (int i = rows; i < ( 2 *rows ) && i < UserList.Count ; i++)
                    Output.WriteLine("          " + UserList[i] + "<br />");
                Output.WriteLine("        </blockquote>");
                Output.WriteLine("      </td>");

                Output.WriteLine("      <td>");
                Output.WriteLine("        <blockquote>");
                for (int i = 2 * rows; i < (3 * rows) && i < UserList.Count; i++)
                    Output.WriteLine("          " + UserList[i] + "<br />");
                Output.WriteLine("        </blockquote>");
                Output.WriteLine("      </td>");

                Output.WriteLine("    </tr>");
            }

        }
    }
}