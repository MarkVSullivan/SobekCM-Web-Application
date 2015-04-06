// HTML5 10/12/2013

#region Using directives

using System;
using System.IO;
using System.Web;
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
        private string menu_preference;

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

		    menu_preference = RequestSpecificValues.Current_User.Get_Setting("Home_AdminViewer:View Preference", "brief");

            // Was this a post-back, which would only be due to a preference change
		    if (RequestSpecificValues.Current_Mode.isPostBack)
		    {
                // Get the new preference
		        string new_preference = HttpContext.Current.Request.Form["admin_menu_preference"];
		        if ((!String.IsNullOrEmpty(new_preference)) && ( new_preference != menu_preference ))
		        {
                    // Save the new preference
                    menu_preference = new_preference;
                    RequestSpecificValues.Current_User.Add_Setting("Home_AdminViewer:View Preference", menu_preference);
                    Library.Database.SobekCM_Database.Set_User_Setting(RequestSpecificValues.Current_User.UserID, "Home_AdminViewer:View Preference", menu_preference);
		        }
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
            get { return Static_Resources.Manage_Collection_Img; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Home_AdminViewer.Write_HTML", "Do nothing");
        }

		/// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
		/// <param name="Output"> Textwriter to write the HTML for this viewer</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
            Tracer.Add_Trace("Home_AdminViewer.Write_ItemNavForm_Opening", String.Empty);

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_menu_preference\" name=\"admin_menu_preference\" value=\"\" />");
            Output.WriteLine();

		    Output.WriteLine("<div class=\"sbkHav_ViewPrerence\">");
		    Output.WriteLine("  <span style=\"font-weight:bold\">View Type:</span>");
            Output.WriteLine("  <select id=\"viewTypeSelect\" name=\"viewTypeSelect\" onchange=\"$('#admin_menu_preference').val(this.value);$('form#itemNavForm').submit();\">");

            if ( menu_preference == "alphabetical")
                Output.WriteLine("    <option value=\"alphabetical\" selected=\"selected\">Alphabetical</option>");
            else
                Output.WriteLine("    <option value=\"alphabetical\">Alphabetical</option>");

            if (menu_preference == "brief")
                Output.WriteLine("    <option value=\"brief\" selected=\"selected\">Brief View</option>");
            else
                Output.WriteLine("    <option value=\"brief\">Brief View</option>");

            if (menu_preference == "categories")
                Output.WriteLine("    <option value=\"categories\" selected=\"selected\">Categories</option>");
            else
                Output.WriteLine("    <option value=\"categories\">Categories</option>");

            if (menu_preference == "classic")
                Output.WriteLine("    <option value=\"classic\" selected=\"selected\">Classic</option>");
            else
                Output.WriteLine("    <option value=\"classic\">Classic</option>");

		    Output.WriteLine("  </select>");
		    Output.WriteLine("</div>");
		    Output.WriteLine();


			Output.WriteLine("<div class=\"sbkHav_MainText\" >");
			Output.WriteLine("  <h1>What would you like to edit today?</h1>");

		    switch (menu_preference)
		    {
		        case "categories":
		            write_categorized(Output);
		            break;

                case "alphabetical":
		            write_alphabetical(Output);
		            break;

                case "classic":
                    write_classic(Output);
                    break;

                default:
		            write_brief(Output);
                    break;
		    }


			Output.WriteLine("  <p>For clarification on any of these options, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/tasks\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");
			Output.WriteLine("  <p>You are currently running version " + UI_ApplicationCache_Gateway.Settings.Current_Web_Version + ". ( <a href=\"http://ufdc.ufl.edu/sobekcm/development/history\">see release notes</a> )</p>");
			Output.WriteLine("</div>");
		}

        private void write_brief(TextWriter Output)
        {
            Output.WriteLine("  <table id=\"sbkHav_OptionsTable\">");

            Output.WriteLine("    <tr><td colspan=\"5\"><h2 id=\"common\">Common Tasks</h2></td></tr>");
            Output.WriteLine("    <tr><td style=\"width:60px\">&nbsp;</td>");

            // Add collection wizard
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
            string add_collection_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + add_collection_url + "\"><img src=\"" + Static_Resources.Wizard_Img + "\" /></a></td><td style=\"width:250px\"><a href=\"" + add_collection_url + "\">Add Collection Wizard</a></td>");

            // Edit item current skin
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
            string edit_curr_skin_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + edit_curr_skin_url + "\"><img src=\"" + Static_Resources.Skins_Img + "\" /></a></td><td><a href=\"" + edit_curr_skin_url + "\">Edit Current Web Skin</a></td>");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            Output.WriteLine("    </tr>");

            if (RequestSpecificValues.Current_User.Is_System_Admin)
            {
                Output.WriteLine("    <tr><td>&nbsp;</td>");

                // Edit aggregation aliases
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Users and Groups</a></td>");

                Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

                Output.WriteLine("    </tr>");
            }
            Output.WriteLine("  </table>");

            Output.WriteLine("  <table id=\"sbkHav_OptionsTable3\">");
            Output.WriteLine("    <tr><td colspan=\"3\"><h2 id=\"appearance\">Appearance</h2></td></tr>");

            



            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:60px\">&nbsp;</td>");
            Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + edit_curr_skin_url + "\"><img src=\"" + Static_Resources.Skins_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + edit_curr_skin_url + "\">Edit Current Web Skin</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            // Edit URL Portals
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
            string portal_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + portal_url + "\"><img src=\"" + Static_Resources.Portals_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + portal_url + "\">URL Portals</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");


            // Edit web skins
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
            string web_skin_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + web_skin_url + "\"><img src=\"" + Static_Resources.Skins_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + web_skin_url + "\">Web Skins</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");


            Output.WriteLine("    <tr><td colspan=\"3\"><h2 id=\"collections\">Collections</h2></td></tr>");

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + add_collection_url + "\"><img src=\"" + Static_Resources.Wizard_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + add_collection_url + "\">Add Collection Wizard</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
       

            // Edit aggregation aliases
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aliases;
            string alias_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + alias_url + "\"><img src=\"" + Static_Resources.Aliases_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + alias_url + "\">Aggregation Aliases</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");


            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
            string aggr_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + alias_url + "\"><img src=\"" + Static_Resources.Aggregations_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + alias_url + "\">Aggregation Management</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            // Edit Thematic Headings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
            string thematic_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + alias_url + "\"><img src=\"" + Static_Resources.Thematic_Heading_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + alias_url + "\">Thematic Headings</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            
            Output.WriteLine("    <tr><td colspan=\"3\"><h2 id=\"items\">Items</h2></td></tr>");

            // Edit Default_Metadata
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
            string default_metadata_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + default_metadata_url + "\"><img src=\"" + Static_Resources.Pmets_Img + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + default_metadata_url + "\">Default Metadata</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            // Edit wordmarks
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
            string wordmark_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + wordmark_url + "\"><img src=\"" + Static_Resources.Wordmarks_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + wordmark_url + "\">Wordmarks / Icons</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");


            // View and set SobekCM Builder Status
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
            string builder_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + builder_url + "\"><img src=\"" + Static_Resources.Gears_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + builder_url + "\">Builder Status</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><td colspan=\"3\"><h2 id=\"settings\">Settings</h2></td></tr>");
            
            // Edit IP Restrictions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
            string ip_restrictions_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + ip_restrictions_url + "\"><img src=\"" + Static_Resources.Restricted_Resource_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + ip_restrictions_url + "\">IP Restriction Ranges</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            // Edit Settings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Settings;
            string settings_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + settings_url + "\"><img src=\"" + Static_Resources.Settings_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + settings_url + "\">System-Wide Settings</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            // Reset cache
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Reset;
            string reset_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + reset_url + "\"><img src=\"" + Static_Resources.Refresh_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + reset_url + "\">Reset Cache</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><td colspan=\"3\"><h2 id=\"permissions\">Users and Permissions</h2></td></tr>");

            // View permissions report
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
            string permissions_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + permissions_url + "\"><img src=\"" + Static_Resources.User_Permission_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + permissions_url + "\">User Permissions Reports</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            if (RequestSpecificValues.Current_User.Is_System_Admin)
            {
                // Edit users
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                string users_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>&nbsp;</td>");
                Output.WriteLine("      <td><a href=\"" + users_url + "\"><img src=\"" + Static_Resources.Users_Img_Large + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + users_url + "\">Users and Groups</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>"); 
            }


            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Home;

            Output.WriteLine("  </table>");
        }

	    private void write_categorized(TextWriter Output)
	    {
            Output.WriteLine("  <table id=\"sbkHav_OptionsTable\">");

            Output.WriteLine("    <tr><td colspan=\"5\"><h2 id=\"common\">Common Tasks</h2></td></tr>");
            Output.WriteLine("    <tr><td style=\"width:60px\">&nbsp;</td>");

            // Add collection wizard
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wizard_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Add Collection Wizard</a></td>");

            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Edit Current Web Skin</a></td>");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            Output.WriteLine("    </tr>");

            if (RequestSpecificValues.Current_User.Is_System_Admin)
            {
                Output.WriteLine("    <tr><td>&nbsp;</td>");

                // Edit aggregation aliases
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Users and Groups</a></td>");

                Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

                Output.WriteLine("    </tr>");
            }


            Output.WriteLine("    <tr><td colspan=\"5\"><h2 id=\"appearance\">Appearance</h2></td></tr>");

            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Edit Current Web Skin</a></td>");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            // Edit URL Portals
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Portals_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">URL Portals</a></td>");

            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Edit web skins
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Web Skins</a></td>");


            Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><td colspan=\"5\"><h2 id=\"collections\">Collections</h2></td></tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Add collection wizard
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wizard_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Add Collection Wizard</a></td>");

            // Edit aggregation aliases
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aliases;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Aliases_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Aggregation Aliases</a></td>");

            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Aggregations_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Aggregation Management</a></td>");

            // Edit Thematic Headings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Thematic_Heading_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Thematic Headings</a></td>");


            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td colspan=\"5\"><h2 id=\"items\">Items</h2></td></tr>");

            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Edit Default_Metadata
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Pmets_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Default Metadata</a></td>");

            // Edit wordmarks
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wordmarks_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Wordmarks / Icons</a></td>");


            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // View and set SobekCM Builder Status
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Gears_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Builder Status</a></td>");

            Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><td colspan=\"5\"><h2 id=\"settings\">Settings</h2></td></tr>");

            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Edit IP Restrictions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Firewall_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">IP Restriction Ranges</a></td>");

            // Edit Settings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Settings;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Settings_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">System-Wide Settings</a></td>");

            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // Reset cache
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Reset;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Refresh_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Reset Cache</a></td>");

            Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");

            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><td colspan=\"5\"><h2 id=\"permissions\">Users and Permissions</h2></td></tr>");

            Output.WriteLine("    <tr><td>&nbsp;</td>");

            // View permissions report
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.User_Permission_Img + "\" /></a></td><td style=\"width:290px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">User Permissions Reports</a></td>");

            if (RequestSpecificValues.Current_User.Is_System_Admin)
            {
                // Edit users
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Users and Groups</a></td>");
            }
            else
            {
                Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");
            }

            Output.WriteLine("    </tr>");


            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Home;

            Output.WriteLine("  </table>");
	    }

        private void write_classic(TextWriter Output)
        {
            Output.WriteLine("  <table id=\"sbkHav_OptionsTable2\">");

            // Add collection wizard
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wizard_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Add Collection Wizard</a></td></tr>");

            // Edit item aggregation
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Aggregations_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Aggregation Management</a></td></tr>");

            // Edit web skins
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Web Skins</a></td></tr>");

            // Edit wordmarks
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wordmarks_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Wordmarks / Icons</a></td></tr>");

            // Edit aggregation aliases
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aliases;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Aliases_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Aggregation Aliases</a></td></tr>");

            // View permissions report
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.User_Permission_Img + "\" /></a></td><td style=\"width:290px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">User Permissions Reports</a></td></tr>");

            if (RequestSpecificValues.Current_User.Is_System_Admin)
            {
                // Edit aggregation aliases
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Registered Users and Groups</a></td></tr>");
            }

            // Edit Default_Metadata
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Pmets_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Default Metadata</a></td></tr>");

            // Edit IP Restrictions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
            Output.WriteLine("      <td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Firewall_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">IP Restriction Ranges</a></td>");

            // Edit URL Portals
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Portals_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">URL Portals</a></td></tr>");

            // Edit Thematic Headings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Thematic_Heading_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Thematic Headings</a></td></tr>");

            // Edit Settings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Settings;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Settings_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">System-Wide Settings</a></td></tr>");

            // View and set SobekCM Builder Status
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Gears_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Builder Status</a></td></tr>");

            // Reset cache
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Reset;
            Output.WriteLine("      <tr><td style=\"width:35px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Refresh_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Reset Cache</a></td></tr>");


            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Home;

            Output.WriteLine("  </table>");
        }


        private void write_alphabetical(TextWriter Output)
        {
            Output.WriteLine("  <table id=\"sbkHav_OptionsTable4\">");

            // Add collection wizard
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
            Output.WriteLine("      <tr><td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wizard_Img_Large + "\" /></a></td><td style=\"width:200px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Add Collection<br />Wizard</a></td>");


            // Edit aggregation aliases
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aliases;
            Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Aliases_Img_Large + "\" /></a></td><td style=\"width:200px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Aggregation<br />Aliases</a></td>");

            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
            Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Aggregations_Img_Large + "\" /></a></td style=\"width:200px\"><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Aggregation<br />Management</a></td></tr>");


            // View and set SobekCM Builder Status
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
            Output.WriteLine("      <tr><td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Gears_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Builder Status</a></td>");


            // Edit Default_Metadata
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
            Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Pmets_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Default Metadata</a></td>");

            // Edit current web skin
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
            Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Edit Current<br />Web Skin</a></td></tr>");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            // Edit IP Restrictions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
            Output.WriteLine("      <tr><td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Restricted_Resource_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">IP Restriction<br />Ranges</a></td>");


            // Reset cache
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Reset;
            Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Refresh_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Reset Cache</a></td>");


            // Edit Settings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Settings;
            Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Settings_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">System-Wide<br />Settings</a></td></tr>");


            // Edit Thematic Headings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
            Output.WriteLine("      <tr><td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Thematic_Heading_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Thematic Headings</a></td>");


            // Edit URL Portals
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
            Output.WriteLine("         <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Portals_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">URL Portals</a></td>");


            if (RequestSpecificValues.Current_User.Is_System_Admin)
            {
                // Edit aggregation aliases
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Users and Groups</a></td></tr>");


                // View permissions report
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
                Output.WriteLine("      <tr><td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.User_Permission_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">User Permissions<br />Reports</a></td>");


                // Edit web skins
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
                Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Web Skins</a></td>");


                // Edit wordmarks
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
                Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wordmarks_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Wordmarks / Icons</a></td></tr>");

            }
            else
            {


                // View permissions report
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
                Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.User_Permission_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">User Permissions<br />Reports</a></td>");


                // Edit web skins
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
                Output.WriteLine("      <tr><td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Web Skins</a></td>");


                // Edit wordmarks
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
                Output.WriteLine("          <td style=\"width:45px;\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wordmarks_Img_Large + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Wordmarks / Icons</a></td>");

                Output.WriteLine("          <td></td><td></td></tr>");

            }






            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Home;

            Output.WriteLine("  </table>");
        }
	}
}
