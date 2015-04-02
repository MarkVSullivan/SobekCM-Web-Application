using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    public class Permissions_Reports_AdminViewer : abstract_AdminViewer
    {
        private DataSet globalPermissions;
        private string actionMessage;

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

            // Try to get the global permissions table
            globalPermissions = HttpContext.Current.Cache["GlobalPermissionsReport"] as DataSet;
            if (globalPermissions == null)
            {
                globalPermissions = SobekCM_Database.Get_Global_User_Permissions(RequestSpecificValues.Tracer);
                if (globalPermissions == null)
                {
                    actionMessage = "Error pulling global user permission!";
                }
                else
                {
                    HttpContext.Current.Cache.Insert("GlobalPermissionsReport", globalPermissions, null, DateTime.Now.AddSeconds(30d), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }
        }

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner, HtmlSubwriter_Behaviors_Enum.Use_Jquery_DataTables }; }
        }

        public override string Web_Title
        {
            get { return "User Permissions Reports"; }
        }
        
        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.User_Permission_Img; }
        }

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
            }

            Output.WriteLine("  <div class=\"SobekHomeText\">");

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

            Output.WriteLine("  <br />");
            Output.WriteLine("  <p>This report allows you to view the permissions that are set for users and groups within this repository.</p>");
            //Output.WriteLine("    <ul>");
            //Output.WriteLine("      <li>Enter the permissions for this user below and press the SAVE button when all your edits are complete.</li>");
            //Output.WriteLine("      <li>For clarification of any terms on this form, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/users\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</li>");
            //Output.WriteLine("     </ul>");
            Output.WriteLine("  </div>");

            // If the data did not load correctly (or I suppose there are NO users)
            if (globalPermissions == null)
            {
                Output.WriteLine("</div>");
                Output.WriteLine("</div>");

                Output.WriteLine("<br />");
                Output.WriteLine("<br />");
                return;
            }

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs\">");
            Output.WriteLine("  <div class=\"tabs\">");
            Output.WriteLine("    <ul>");
            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
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

            Output.WriteLine("  <br /><br />");
            Output.WriteLine();

            switch (page)
            {
                case 1:
                    List<string> hostAdmins = new List<string>();
                    List<string> sysAdmins = new List<string>();
                    List<string> portalAdmins = new List<string>();
                    List<string> canDeleteAll = new List<string>();
                    List<string> internalUsers = new List<string>();
                    List<string> canEditAll = new List<string>();
                    List<string> canSubmitList = new List<string>();

                    foreach (DataRow thisUser in globalPermissions.Tables[0].Rows)
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
                            if (!canDeleteAll.Contains(name)) canDeleteAll.Add(name);
                            if (!canEditAll.Contains(name)) canEditAll.Add(name);
                            if (!canSubmitList.Contains(name)) canSubmitList.Add(name);
                            if (!internalUsers.Contains(name)) internalUsers.Add(name);
                        }
                        else if (Convert.ToBoolean(thisUser["IsSystemAdmin"]))
                        {
                            if (!sysAdmins.Contains(name)) sysAdmins.Add(name);
                            if (!canDeleteAll.Contains(name)) canDeleteAll.Add(name);
                            if (!canEditAll.Contains(name)) canEditAll.Add(name);
                            if (!canSubmitList.Contains(name)) canSubmitList.Add(name);
                            if (!internalUsers.Contains(name)) internalUsers.Add(name);
                        }
                        else if (Convert.ToBoolean(thisUser["IsPortalAdmin"]))
                        {
                            if (!portalAdmins.Contains(name)) portalAdmins.Add(name);
                            if (!canDeleteAll.Contains(name)) canDeleteAll.Add(name);
                            if (!canEditAll.Contains(name)) canEditAll.Add(name);
                            if (!canSubmitList.Contains(name)) canSubmitList.Add(name);
                            if (!internalUsers.Contains(name)) internalUsers.Add(name);
                        }
                        else
                        {
                            if ((Convert.ToBoolean(thisUser["Can_Delete_All_Items"])) && (!canDeleteAll.Contains(name))) canDeleteAll.Add(name);
                            if ((Convert.ToBoolean(thisUser["Internal_User"])) && (!internalUsers.Contains(name))) internalUsers.Add(name);
                            if ((Convert.ToBoolean(thisUser["Can_Edit_All_Items"])) && (!canEditAll.Contains(name))) canEditAll.Add(name);
                            if ((Convert.ToBoolean(thisUser["Can_Submit_Items"])) && (!canSubmitList.Contains(name))) canSubmitList.Add(name);
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
                    bool spanning = false;
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
                        spanning = true;
                        if (canDeleteAll.Count > 0) add_user_list(Output, canDeleteAll, "Can Delete All Items", true);
                        if (internalUsers.Count > 0) add_user_list(Output, internalUsers, "Power Users", true);
                        if (canEditAll.Count > 0) add_user_list(Output, canEditAll, "Can Edit All Items", true);
                    }

                    if (canSubmitList.Count > 0)
                    {
                        if (spanning)
                        {
                            add_user_list(Output, canSubmitList, "Can Submit Items Online", true);
                        }
                        else
                        {
                            Output.WriteLine("    <tr>");
                            Output.WriteLine("      <td>");
                            add_user_list(Output, canSubmitList, "Can Submit Items Online", false);
                            Output.WriteLine("      </td>");
                            Output.WriteLine("      <td></td>");
                            Output.WriteLine("      <td></td>");
                            Output.WriteLine("    </tr>");
                        }
                    }


                    Output.WriteLine("  </table>");
                    break;

                case 2:

                    if (globalPermissions.Tables[0].Rows.Count == 0)
                        return;

                    bool includeHostAdmin = false;
                    foreach (DataRow thisUser in globalPermissions.Tables[0].Rows)
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
                    Output.WriteLine("        <th>First Name</th>");
                    Output.WriteLine("        <th>Last Name</th>");
                    Output.WriteLine("        <th>Created</th>");
                    Output.WriteLine("        <th>Last Activity</th>");
                    Output.WriteLine("        <th style=\"text-align:center\">Can Submit<br />Items</th>");
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
                    Output.WriteLine("        <th><input id=\"reportFirstNameSearch\"  type=\"text\" placeholder=\"Search First Name\" /></th>");
                    Output.WriteLine("        <th><input id=\"reportLastNameSearch\"  type=\"text\" placeholder=\"Search Last Name\" /></th>");
                    Output.WriteLine("        <th>Created</th>");
                    Output.WriteLine("        <th>Last Activity</th>");
                    Output.WriteLine("        <th style=\"text-align:center\"></th>");
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

                    int last_userid = Convert.ToInt32(globalPermissions.Tables[0].Rows[0]["UserID"].ToString());
                    bool canSubmit = false;
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
                    foreach (DataRow thisRow in globalPermissions.Tables[0].Rows)
                    {
                        int this_userid = Convert.ToInt32(thisRow["UserID"].ToString());
                        if (this_userid != last_userid)
                        {
                            // Add this row
                            Output.WriteLine("      <tr>");
                            Output.WriteLine("        <td>" + firstName + "</td>");
                            Output.WriteLine("        <td>" + lastName + "</td>");
                            Output.WriteLine("        <td>" + dateCreated + "</td>");
                            Output.WriteLine("        <td>" + lastActvity + "</td>");
                            Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(canSubmit) + "</td>");
                            Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(canEdit) + "</td>");
                            Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(powerUser) + "</td>");
                            Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(canDelete) + "</td>");
                            Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(portalAdmin) + "</td>");
                            Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(sysAdmin) + "</td>");
                            if (includeHostAdmin)
                                Output.WriteLine("      <td style=\"text-align:center\">" + bool_to_char(hostAdmin) + "</td>");
                            Output.WriteLine("      </tr>");

                            // Clear the flags
                            canSubmit = false;
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
                        if (Convert.ToBoolean(thisRow["Can_Submit_Items"].ToString())) canSubmit = true;
                        if (Convert.ToBoolean(thisRow["Can_Edit_All_Items"].ToString())) canEdit = true;
                        if (Convert.ToBoolean(thisRow["Internal_User"].ToString())) powerUser = true;
                        if (Convert.ToBoolean(thisRow["Can_Delete_All_Items"].ToString())) canDelete = true;
                        if (Convert.ToBoolean(thisRow["IsPortalAdmin"].ToString())) portalAdmin = true;
                        if (Convert.ToBoolean(thisRow["IsSystemAdmin"].ToString())) sysAdmin = true;
                        if (Convert.ToBoolean(thisRow["IsHostAdmin"].ToString())) hostAdmin = true;
                    }

                    // Add the last row
                    Output.WriteLine("      <tr>");
                    Output.WriteLine("        <td>" + firstName + "</td>");
                    Output.WriteLine("        <td>" + lastName + "</td>");
                    Output.WriteLine("        <td>" + dateCreated + "</td>");
                    Output.WriteLine("        <td>" + lastActvity + "</td>");
                    Output.WriteLine("        <td style=\"text-align:center\">" + bool_to_char(canSubmit) + "</td>");
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
                    Output.WriteLine("                    if (( i > 3 )) {");
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

                    Output.WriteLine("         $('#reportFirstNameSearch').on( 'keyup change', function () { table.column( 0 ).search( this.value ).draw(); } );");
                    Output.WriteLine("         $('#reportLastNameSearch').on( 'keyup change', function () { table.column( 1 ).search( this.value ).draw(); } );");
                    Output.WriteLine("    } );");

                    Output.WriteLine("</script>");
                    Output.WriteLine();

                    break;

                case 3:
                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle_first\"> &nbsp; All Aggregations</span>");
                    Output.WriteLine("  <blockquote>");

                    Output.WriteLine("  </blockquote>");
                    break;

                case 4:
                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle_first\"> &nbsp; Single Aggregations</span>");
                    Output.WriteLine("  <blockquote>");

                    Output.WriteLine("  </blockquote>");
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

        private string bool_to_char(bool Value)
        {
            return Value ? "Y" : "";
        }

        private void add_user_list(TextWriter Output, List<string> UserList, string Title, bool multicolumn )
        {
            if (!multicolumn)
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
                int rows = ((UserList.Count - 1)%3) + 1;
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