// HTML5 10/12/2013

#region Using directives

using System;
using System.IO;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Core.Users;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

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
		/// <param name="User"> Authenticated user information </param>
		/// <param name="Current_Mode">Mode / navigation information for the current request</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public Home_AdminViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Custom_Tracer Tracer)
			: base(User)
		{
			Tracer.Add_Trace("Home_AdminViewer.Constructor", String.Empty);

			if ((User == null) || ((!User.Is_Portal_Admin) && (!User.Is_System_Admin)))
			{
				Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
				Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
				currentMode.Redirect();                 
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
			Tracer.Add_Trace("Home_AdminViewer.Write_HTML", String.Empty);

			Output.WriteLine("<div class=\"sbkHav_MainText\" >");
			Output.WriteLine("  What would you like to edit today?");
			Output.WriteLine("  <table id=\"sbkHav_OptionsTable\">");

			// Edit aggregation aliases
			currentMode.Admin_Type = Admin_Type_Enum.Aliases;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "forwarding.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Aggregation Aliases</a></td></tr>");

			// Edit item aggregationPermissions
			currentMode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "building.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Aggregation Management</a></td></tr>");
			

			// View and set SobekCM Builder Status
			currentMode.Admin_Type = Admin_Type_Enum.Builder_Status;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "gears.png\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Builder Status</a></td></tr>");
			

			// Edit Default_Metadata
			currentMode.Admin_Type = Admin_Type_Enum.Default_Metadata;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "pmets.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Default Metadata</a></td></tr>");

			// Edit IP Restrictions
			currentMode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "firewall.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">IP Restriction Ranges</a></td></tr>");

			// Edit Settings
			currentMode.Admin_Type = Admin_Type_Enum.Settings;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "wrench.png\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">System-Wide Settings</a></td></tr>");

			// Edit Thematic Headings
			currentMode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
			Output.WriteLine("    <tr><td style=\"width:35px;\"></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Thematic Headings</a></td></tr>");


			// Edit URL Portals
			currentMode.Admin_Type = Admin_Type_Enum.URL_Portals;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "portals.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">URL Portals</a></td></tr>");

			if (user.Is_System_Admin)
			{
				// Edit users
				currentMode.Admin_Type = Admin_Type_Enum.Users;
				Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "users.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Users and Groups</a></td></tr>");
			}

			// Edit web skins
			currentMode.Admin_Type = Admin_Type_Enum.Skins;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "skins.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Web Skins</a></td></tr>");

			// Edit wordmarks
			currentMode.Admin_Type = Admin_Type_Enum.Wordmarks;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "wordmarks.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Wordmarks / Icons</a></td></tr>");

			// Reset cache
			currentMode.Admin_Type = Admin_Type_Enum.Reset;
			Output.WriteLine("    <tr><td style=\"width:35px;\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "refresh.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Reset Cache</a></td></tr>");

			currentMode.Admin_Type = Admin_Type_Enum.Home;

			Output.WriteLine("  </table>");

			Output.WriteLine("  <p>For clarification on any of these options, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/tasks\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");
			Output.WriteLine("  <p>You are currently running version " + SobekCM_Library_Settings.CURRENT_WEB_VERSION + ". ( <a href=\"http://ufdc.ufl.edu/sobekcm/development/history\">see release notes</a> )</p>");
			Output.WriteLine("</div>");
		}
	}
}
