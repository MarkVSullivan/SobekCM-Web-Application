// HTML5 10/12/2013

#region Using directives

using System;
using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Settings;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.AdminViewer
{
	/// <summary> Class allows a system admin to view the administrative home page, with all their options in a menu  </summary>
	/// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
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
	public class Home_AdminViewer : abstract_AdminViewer
	{
		/// <summary> Constructor for a new instance of the Home_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Home_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
		{
            RequestSpecificValues.Tracer.Add_Trace("Home_AdminViewer.Constructor", String.Empty);

            if ((RequestSpecificValues.Current_User == null) || ((!RequestSpecificValues.Current_User.Is_Portal_Admin) && (!RequestSpecificValues.Current_User.Is_System_Admin)))
			{
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);                 
			}
		}

		/// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
		/// <value> The value of this message changes, depending on if this is the RequestSpecificValues.Current_User's first time here.  It is always a welcoming message though </value>
		public override string Web_Title
		{
			get
			{
				return "System Administrative Tasks";
			}
		}


        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return String.Empty; }
        }

		/// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
		/// <param name="Output"> Textwriter to write the HTML for this viewer</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("Home_AdminViewer.Write_HTML", String.Empty);

			Output.WriteLine("<div class=\"sbkHav_MainText\" >");
			Output.WriteLine("  What would you like to edit today?");
			Output.WriteLine("  <table id=\"sbkHav_OptionsTable\">");

		    Output.WriteLine("    <tr><td colspan=\"5\"><h2 name=\"common\">Common Tasks</h2></td></tr>");
		    Output.WriteLine("    <tr><td style=\"width:60px\">&nbsp;</td>");

            // Add collection wizard
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wizard_Png + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Add Collection Wizard</a></td>");

            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Edit Current Web Skin</a></td>");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            Output.WriteLine("    </tr>");

		    if (RequestSpecificValues.Current_User.Is_System_Admin)
		    {
		        Output.WriteLine("    <tr><td>&nbsp;</td>");

		        // Edit aggregation aliases
		        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Users and Groups</a></td>");

		        Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

		        Output.WriteLine("    </tr>");
		    }

		    Output.WriteLine("    <tr><td colspan=\"5\"><h2 name=\"appearance\">Appearance</h2></td></tr>");

            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Edit Current Web Skin</a></td>");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            // Edit URL Portals
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Portals_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">URL Portals</a></td>");

            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Edit web skins
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Web Skins</a></td>");


            Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><td colspan=\"5\"><h2 name=\"collections\">Collections</h2></td></tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Add collection wizard
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wizard_Png + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Add Collection Wizard</a></td>");

			// Edit aggregation aliases
			RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aliases;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Forwarding_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Aggregation Aliases</a></td>");

            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

			// Edit item aggregationPermissions
			RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Building_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Aggregation Management</a></td>");

            // Edit Thematic Headings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Themaitc_Heading_Png + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Thematic Headings</a></td>");


		    Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td colspan=\"5\"><h2 name=\"items\">Items</h2></td></tr>");

            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Edit Default_Metadata
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Pmets_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Default Metadata</a></td>");

            // Edit wordmarks
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wordmarks_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Wordmarks / Icons</a></td>");


            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // View and set SobekCM Builder Status
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Gears_Png + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Builder Status</a></td>");

            Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

            Output.WriteLine("    </tr>");


		        Output.WriteLine("    <tr><td colspan=\"5\"><h2 name=\"permissions\">Permissions</h2></td></tr>");

                Output.WriteLine("    <tr><td>&nbsp;</td>");

                // View permissions report
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
                Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Icon_Permission_Png + "\" /></a></td><td style=\"width:290px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">User Permissions Reports</a></td>");

		    if (RequestSpecificValues.Current_User.Is_System_Admin)
		    {
		        // Edit users
		        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Users and Groups</a></td>");
		    }
		    else
		    {
                Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");
		    }

		    Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><td colspan=\"5\"><h2 name=\"settings\">Settings</h2></td></tr>");

            Output.WriteLine("    <tr><td>&nbsp;</td>");

		    // Edit IP Restrictions
			RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Firewall_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">IP Restriction Ranges</a></td>");

			// Edit Settings
			RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Settings;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wrench_Png + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">System-Wide Settings</a></td>");

            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Reset cache
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Reset;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Refresh_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Reset Cache</a></td>");

            Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

            Output.WriteLine("    </tr>");


			RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Home;

			Output.WriteLine("  </table>");

			Output.WriteLine("  <p>For clarification on any of these options, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/tasks\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");
			Output.WriteLine("  <p>You are currently running version " + UI_ApplicationCache_Gateway.Settings.Current_Web_Version + ". ( <a href=\"http://ufdc.ufl.edu/sobekcm/development/history\">see release notes</a> )</p>");
			Output.WriteLine("</div>");
		}
	}
}
