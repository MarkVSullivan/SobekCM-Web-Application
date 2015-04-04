using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;
using SobekCM.Tools;
using SobekCM.UI_Library;

namespace SobekCM.Library.AggregationViewer.Viewers
{
    public class User_Permissions_AggregationViewer : abstractAggregationViewer
    {
        /// <summary> Constructor for a new instance of the User_Permissions_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public User_Permissions_AggregationViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // User must AT LEAST be logged on, return
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
            {
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If the user is not an admin of some type, also return
            if ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin) && (!RequestSpecificValues.Current_User.Is_Aggregation_Curator(RequestSpecificValues.Hierarchy_Object.Code)))
            {
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.User_Permissions; }
        }

        /// <summary> Flag which indicates whether the selection panel should be displayed </summary>
        /// <value> This defaults to <see cref="Selection_Panel_Display_Enum.Selectable"/> but is overwritten by most collection viewers </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get { return Selection_Panel_Display_Enum.Never; }
        }

        /// <summary> Gets flag which indicates whether this is an internal view, which may have a 
        /// slightly different design feel </summary>
        /// <remarks> This returns FALSE by default, but can be overriden by individual viewer implementations</remarks>
        public override bool Is_Internal_View
        {
            get { return true; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Collection-Specific User Permissions"; }
        }

        /// <summary> Gets the URL for the icon related to this aggregational viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.User_Permission_Img; }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This does nothing - as an internal type view, this will not be called </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Add the main HTML to be added to the page </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes the HTML from the static browse or info page here  </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            DataTable permissionsTbl = SobekCM_Database.Get_Aggregation_User_Permissions(RequestSpecificValues.Hierarchy_Object.Code, RequestSpecificValues.Tracer);

            // Is this a system administrator?
            bool isSysAdmin = ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Host_Admin));
            string userAdminUrl = String.Empty;
            string userGroupAdminUrl = String.Empty;

            // If no permissions received, just show a message
            if ((permissionsTbl == null) || (permissionsTbl.Rows.Count == 0))
            {
                Output.WriteLine("<p>No special user permissions found for this collection.</p>");

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

            Output.WriteLine("<p style=\"text-align: left; padding:0 20px 0 20px;\">Below is the list of all users that have specialized user permissions for this collection.  These permissions may be assigned individually, or through a user group.</p>");





            // Show a message about selecting the user below to edit them
            if (isSysAdmin)
            {
                Output.WriteLine("<p style=\"text-align: left; padding:0 20px 0 20px;\">Select a user from the list below to edit that user's permissions.</p>");

                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "Xyzzy";
                userAdminUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Groups;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "Xyzzy";
                userGroupAdminUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
            }

            // Is this using detailed permissions?
            bool detailedPermissions = UI_ApplicationCache_Gateway.Settings.Detailed_User_Aggregation_Permissions;

            // Dertermine the number of columns
            int columns = 5;
            if (detailedPermissions)
                columns = 10;

            Output.WriteLine("  <table class=\"sbkWhav_Table\">");
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
                    if (isSysAdmin)
                    {
                        Output.WriteLine("    <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userAdminUrl.Replace("Xyzzy", thisUserId.ToString()) + "', '_UserEdit+ " + thisUserId + "');\">");
                    }
                    else
                    {
                        Output.WriteLine("    <tr>");
                    }

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
                    Output.WriteLine("    <tr class=\"sbkWhav_TableRule\"><td colspan=\"" + columns + "\"></td></tr>");


                    // Prepare to collect the information about this user
                    last_userid = thisUserId;
                    username = thisUser["LastName"] + "," + thisUser["FirstName"];
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
            if (isSysAdmin)
            {
                Output.WriteLine("    <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userAdminUrl.Replace("Xyzzy", last_userid.ToString()) + "', '_UserEdit+ " + last_userid + "');\">");
            }
            else
            {
                Output.WriteLine("    <tr>");
            }
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
            Output.WriteLine("    <tr class=\"sbkWhav_TableRule\"><td colspan=\"" + columns + "\"></td></tr>");

            Output.WriteLine("  </table>");
            Output.WriteLine("  <br /><br />");


            // If there were user groups, add them now also.
            if (userGroupRows.Count > 0)
            {
                Output.WriteLine("<p style=\"text-align: left; padding:0 20px 0 20px;\">Some of the permissions above are assigned to the users through user groups.  These user groups and their permissions appear below:</p>");


                Output.WriteLine("  <table class=\"sbkWhav_Table\">");
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

                foreach (KeyValuePair<string, DataRow> userGroupRow in userGroupRows )
                {
                    DataRow thisUser = userGroupRow.Value;
                    int userGroupId = Convert.ToInt32(thisUser["UserGroupID"]);

                    if (isSysAdmin)
                    {
                        Output.WriteLine("    <tr class=\"sbkUpav_SelectableRow\" onclick=\"window.open('" + userGroupAdminUrl.Replace("Xyzzy", userGroupId.ToString()) + "', '_UserGroupEdit+ " + userGroupId + "');\">");
                    }
                    else
                    {
                        Output.WriteLine("    <tr>");
                    }
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
                    Output.WriteLine("    <tr class=\"sbkWhav_TableRule\"><td colspan=\"" + columns + "\"></td></tr>");
                }

                Output.WriteLine("  </table>");
                Output.WriteLine("  <br /><br />");

                if (isSysAdmin)
                {
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;

                    Output.WriteLine("  <p style=\"text-align: left; padding:0 20px 0 20px;\">Use the <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">administrative Users &amp; Groups</a> to assign any new collection-specific user permissions.");
                    Output.WriteLine("  <br /><br />");
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                }

            }


        }

        private string flag_to_display(bool ToDisplay)
        {
            if (ToDisplay)
                return "Y";
            return "";
        }

        private string flag_to_display(object ToDisplay)
        {
            if ((ToDisplay != DBNull.Value ) && ( Convert.ToBoolean(ToDisplay)))
                return "Y";
            return "";
        }
    }
}