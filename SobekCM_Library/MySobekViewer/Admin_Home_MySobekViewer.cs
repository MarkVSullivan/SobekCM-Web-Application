#region Using directives

using System;
using System.IO;
using System.Web;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows a system admin to view the administrative home page, with all their options in a menu  </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the system admin home page </li>
    /// </ul></remarks>
    public class Admin_Home_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the Admin_Home_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode">Mode / navigation information for the current request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Admin_Home_MySobekViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Admin_Home_MySobekViewer.Constructor", String.Empty);

            if ((User == null) || ((!User.Is_Portal_Admin) && (!User.Is_System_Admin)))
            {
                Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                HttpContext.Current.Response.Redirect(Current_Mode.Redirect_URL(), false);                   
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> The value of this message changes, depending on if this is the user's first time here.  It is always a welcoming message though </value>
        public override string Web_Title
        {
            get
            {
                return "System Administrative Tasks";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_Home_MySobekViewer.Write_HTML", String.Empty);

            Output.WriteLine("<div class=\"SobekHomeText\" >");
            Output.WriteLine("<table width=\"700\"><tr><td width=\"200px\">&nbsp;</td>");
            Output.WriteLine("<td><div style=\"font-size:larger\">");
            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
            Output.WriteLine("What would you like to edit today?");
            Output.WriteLine("<blockquote>");

            Output.WriteLine("<table>");

            // Edit item aggregations
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Aggregations_Mgmt;
            Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "building.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Item Aggregations</a></td></tr>");

            // Edit interfaces
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Interfaces;
            Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "skins.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Web Skins</a></td></tr>");

            // Edit wordmarks
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Wordmarks;
            Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "wordmarks.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Wordmarks / Icons</a></td></tr>");

            // Edit forwarding
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Forwarding;
            Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "forwarding.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Aggregation Aliases</a></td></tr>");

            // Edit Projects
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Projects;
            Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "pmets.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Projects</a></td></tr>"); 

            // Edit Thematic Headings
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Thematic_Headings;
            Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Thematic Headings</a></td></tr>");

            if (user.Is_System_Admin)
            {
                // Edit users
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Users;
                Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "users.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Registered Users and Groups</a></td></tr>");

                // Edit IP Restrictions
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_IP_Restrictions;
                Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "firewall.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">IP Restriction Ranges</a></td></tr>");

                // Edit URL Portals
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_URL_Portals;
                Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "portals.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">URL Portals</a></td></tr>");

                // Edit Settings
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Settings;
                Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "wrench.png\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">System-Wide Settings</a></td></tr>");

                // View and set SobekCM Builder Status
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Builder_Status;
                Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "gears.png\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">SobekCM Builder Status</a></td></tr>");
            }

            // Reset cache
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Reset;
            Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "refresh.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Reset Cache</a></td></tr>");

            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Home;

            Output.WriteLine("</table>");
            Output.WriteLine("</blockquote>");
            Output.WriteLine("<br />");
            Output.WriteLine("</div>");
            Output.WriteLine("</td></tr></table>");
            Output.WriteLine(" &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; For clarification on any of these options, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "admin/tasks\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.<br /><br />");
            Output.WriteLine(" &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; You are currently running version " + SobekCM_Library_Settings.CURRENT_WEB_VERSION + ". ( <a href=\"http://ufdc.ufl.edu/sobekcm/development/history\">see release notes</a> )<br /><br />");
            Output.WriteLine("</div>");
        }
    }
}
