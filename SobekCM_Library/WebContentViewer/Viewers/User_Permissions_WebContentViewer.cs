using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;
using SobekCM.Tools;

namespace SobekCM.Library.WebContentViewer.Viewers
{
    /// <summary> Web content viewer shows all of the user permissions, either assigned individually to a user or through a user group,
    /// affecting the ability of users to perform changes against this web content page </summary>
    /// <remarks> This viewer extends the <see cref="abstractWebContentViewer" /> abstract class and implements the <see cref="iWebContentViewer"/> interface. </remarks>
    public class User_Permissions_WebContentViewer : abstractWebContentViewer
    {
        /// <summary>  Constructor for a new instance of the User_Permissions_WebContentViewer class  </summary>
        /// <param name="RequestSpecificValues">  All the necessary, non-global data specific to the current request  </param>
        public User_Permissions_WebContentViewer(RequestCache RequestSpecificValues) : base ( RequestSpecificValues )
        {
            
        }


        /// <summary> Gets the type of specialized web content viewer </summary>
        public override WebContent_Type_Enum Type { get { return WebContent_Type_Enum.Permissions; }}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Web Content User Permissions"; }
        }

        /// <summary> Gets the URL for the icon related to this web content viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.User_Permission_Img; }
        }

        /// <summary> Add the HTML to be displayed </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("User_Permissions_WebContentViewer.Add_HTML", "No html added");
            }

            Output.WriteLine("<div class=\"Wchs_Text\">");
            Output.WriteLine("  <p>Below are the users that have permissions to edit this page. This includes system and portal administrators, as well as users and user groups that are individually permissioned to this web content page.</p>");
            Output.WriteLine("</div>");


            // Try to get the global permissions table
            DataTable globalPermissions = HttpContext.Current.Cache["GlobalPermissionsReport"] as DataTable;
            if (globalPermissions == null)
            {
                globalPermissions = SobekCM_Database.Get_Global_User_Permissions(RequestSpecificValues.Tracer);
                if (globalPermissions == null)
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageError\">Error pulling user permissions!</div>");
                }
                else
                {
                    HttpContext.Current.Cache.Insert("GlobalPermissionsReport", globalPermissions, null, DateTime.Now.AddSeconds(60d), Cache.NoSlidingExpiration);
                }
            }

            if (globalPermissions != null)
            {
                List<string> hostAdmins = new List<string>();
                List<string> sysAdmins = new List<string>();
                List<string> portalAdmins = new List<string>();

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
                    }
                    else if (Convert.ToBoolean(thisUser["IsPortalAdmin"]))
                    {
                        if (!portalAdmins.Contains(name)) portalAdmins.Add(name);
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

                Output.WriteLine("  </table>");
            }
        }

        private void add_user_list(TextWriter Output, List<string> UserList, string Title, bool Multicolumn)
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
                int rows = ((UserList.Count - 1) / 3) + 1;
                Output.WriteLine("      <td>");
                Output.WriteLine("        <blockquote>");
                for (int i = 0; i < rows; i++)
                    Output.WriteLine("          " + UserList[i] + "<br />");
                Output.WriteLine("        </blockquote>");
                Output.WriteLine("        <br />");
                Output.WriteLine("      </td>");

                Output.WriteLine("      <td>");
                Output.WriteLine("        <blockquote>");
                for (int i = rows; i < (2 * rows) && i < UserList.Count; i++)
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
