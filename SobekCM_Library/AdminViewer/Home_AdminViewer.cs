// HTML5 10/12/2013

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

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
	    private Dictionary<string, List<string>> categories_dictionary = new Dictionary<String, List<String>>();
	    private SortedList<string, string> icons = new SortedList<String, String>();

        // Constants for the brief explanations
        private const string ADD_COLLECTION_WIZARD_BRIEF = "Add a new collection (or any other type of aggregation) using the wizard.  This will guide you the process of adding a new collection and uploading the banner and button.";
		private const string EDIT_CURR_SKIN_BRIEF = "Edit the web skin currently in use.  This allows editing of headers and footers, implementing general style changes via CSS, and uploading web-skin related images and documents.";
		private const string USERS_AND_GROUPS_BRIEF = "Edit users and user groups and assign new permissions either directly to users or through the user group membership.";
		private const string URL_PORTALS_BRIEF = "URL portals define the different web skins and default aggregations to be displayed for different incoming base URLs.";
		private const string WEB_SKINS_BRIEF = "View, edit, and create web skins to modify the overall look and feel of the site by editing headers, footers, and the CSS stylesheets.";
		private const string ALIASES_BRIEF = "Manage the various aggregation aliases which allow different URLs to point to the same aggregation.";
		private const string AGGR_MGMT_BRIEF = "Manage all the aggregations ( collections, institutions, exhibits, etc.. ) by adding new aggregations, deleting existing aggregations, and other administrative tasks.";
		private const string THEMATIC_HEADING_BRIEF = "Manage the thematic headings, which allow collections within this instance to be added to and categorized on the main repository home page.";
		private const string DEFAULT_METADATA_BRIEF = "Manage the default metadata sets which can be assigned to users to provide some standard metadata for all items added through the online templates.";
		private const string WORDMARKS_BRIEF = "Manage the wordmarks that can be associated with digital reousrces to appear when viewing the item.  These are often used to attribute contributors and granting agencies.";
		private const string BUILDER_STATUS_BRIEF = "Check the current builder status and view recent logs and errors that the builder may have encountered.";
		private const string RESTRICTIONS_BRIEF = "Edit the IP ranges which may be used to restrict access to digital resources to certain institutions or sets of computers, rather than allowing open, public access.";
		private const string SETTINGS_BRIEF = "These settings control the basic operation and behavior of the entire repository.";
        private const string RESET_CACHE_BRIEF = "This resets the cache and many of the application values and forces the web application to pull all the data fresh from the design folders and from the database.";
		private const string PERMISSIONS_BRIEF = "View reports on the different top-level permissions that have been provided to users, either directly or through user group membership.";


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
	            if ((!String.IsNullOrEmpty(new_preference)) && (new_preference != menu_preference))
	            {
	                // Save the new preference
	                menu_preference = new_preference;
	                RequestSpecificValues.Current_User.Add_Setting("Home_AdminViewer:View Preference", menu_preference);
	                Library.Database.SobekCM_Database.Set_User_Setting(RequestSpecificValues.Current_User.UserID, "Home_AdminViewer:View Preference", menu_preference);
	            }
	        }

	        // Add all the known categories
	        categories_dictionary["common"] = new List<string>();
	        categories_dictionary["appearance"] = new List<string>();
	        categories_dictionary["collections"] = new List<string>();
	        categories_dictionary["items"] = new List<string>();
	        categories_dictionary["settings"] = new List<string>();
	        categories_dictionary["permissions"] = new List<string>();

	        // Build the icons lists

	        // Add collection wizard
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
	        string addColUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string addColIcon = "  <a href=\"" + addColUrl + "\" title=\"" + ADD_COLLECTION_WIZARD_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Wizard_Img + "\" /><span class=\"sbkHav_ButtonText\">Add Collection<br />Wizard</span></div></a>";
	        icons["Add Collection Wizard"] = addColIcon;
	        categories_dictionary["common"].Add(addColIcon);

	        // Edit item current web skin
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
	        RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
	        string editCurrSkinUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
	        string editCurrSkinIcon = "  <a href=\"" + editCurrSkinUrl + "\" title=\"" + EDIT_CURR_SKIN_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Skins_Img + "\" /><span class=\"sbkHav_ButtonText\">Edit Current<br />Web Skin</span></div></a>";
	        icons["Edit Current Web Skin"] = editCurrSkinIcon;
	        categories_dictionary["common"].Add(editCurrSkinIcon);

	        string usersIcon = String.Empty;
	        if (RequestSpecificValues.Current_User.Is_System_Admin)
	        {
	            // Edit users and groups
	            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
	            string usersUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	            usersIcon = "  <a href=\"" + usersUrl + "\" title=\"" + USERS_AND_GROUPS_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Users_Img + "\" /><span class=\"sbkHav_ButtonText\">Users and Groups</span></div></a>";
	            icons["Users and Groups"] = usersIcon;
	            categories_dictionary["common"].Add(usersIcon);
	        }


	        // Edit item current web skin (REPEAT FROM COMMON TASKS CATEGORY)
	        categories_dictionary["appearance"].Add(editCurrSkinIcon);

	        // Edit URL Portals
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
	        string urlPortalsUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string urlPortalsIcon = "  <a href=\"" + urlPortalsUrl + "\" title=\"" + URL_PORTALS_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Portals_Img + "\" /><span class=\"sbkHav_ButtonText\">URL Portals</span></div></a>";
	        icons["URL Portals"] = urlPortalsIcon;
	        categories_dictionary["appearance"].Add(urlPortalsIcon);

	        // Edit web skins
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
	        string skinsUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string skinsIcon = "  <a href=\"" + skinsUrl + "\" title=\"" + WEB_SKINS_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Skins_Img + "\" /><span class=\"sbkHav_ButtonText\">Web Skins</span></div></a>";
	        icons["Web Skins"] = skinsIcon;
	        categories_dictionary["appearance"].Add(skinsIcon);

	        // Add collection wizard (REPEAT FROM COMMON TASKS CATEGORY)
	        categories_dictionary["collections"].Add(addColIcon);

	        // Edit aggregation aliases
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aliases;
	        string aliasesUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string aliasesIcon = "  <a href=\"" + aliasesUrl + "\" title=\"" + ALIASES_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Aliases_Img + "\" /><span class=\"sbkHav_ButtonText\">Aggregation<br />Aliases</span></div></a>";
	        icons["Aggregation Aliases"] = aliasesIcon;
	        categories_dictionary["collections"].Add(aliasesIcon);

	        // Edit item aggregations
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
	        string aggrMgmtUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string aggrMgmtIcon = "  <a href=\"" + aggrMgmtUrl + "\" title=\"" + AGGR_MGMT_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Aggregations_Img + "\" /><span class=\"sbkHav_ButtonText\">Aggregation<br />Management</span></div></a>";
	        icons["Aggregation Management"] = aggrMgmtIcon;
	        categories_dictionary["collections"].Add(aggrMgmtIcon);

	        // Edit Thematic Headings
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
	        string thematicHeadingUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string thematicHeadingIcon = "  <a href=\"" + thematicHeadingUrl + "\" title=\"" + THEMATIC_HEADING_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Thematic_Heading_Img + "\" /><span class=\"sbkHav_ButtonText\">Thematic Headings</span></div></a>";
	        icons["Thematic Headings"] = thematicHeadingIcon;
	        categories_dictionary["collections"].Add(thematicHeadingIcon);


	        // Edit Default_Metadata
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
	        string defaultMetadataUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string defaultMetadataIcon = "  <a href=\"" + defaultMetadataUrl + "\" title=\"" + DEFAULT_METADATA_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Pmets_Img + "\" /><span class=\"sbkHav_ButtonText\">Default Metadata</span></div></a>";
	        icons["Default Metadata"] = defaultMetadataIcon;
	        categories_dictionary["items"].Add(defaultMetadataIcon);

	        // Edit wordmarks
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
	        string wordmarksUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string wordmarksIcon = "  <a href=\"" + wordmarksUrl + "\" title=\"" + WORDMARKS_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Wordmarks_Img + "\" /><span class=\"sbkHav_ButtonText\">Wordmarks / Icons</span></div></a>";
	        icons["Wordmarks"] = wordmarksIcon;
	        categories_dictionary["items"].Add(wordmarksIcon);

	        // View and set SobekCM Builder Status
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
	        string builderUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string builderIcon = "  <a href=\"" + builderUrl + "\" title=\"" + BUILDER_STATUS_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Gears_Img + "\" /><span class=\"sbkHav_ButtonText\">Builder Status</span></div></a>";
	        icons["Builder Status"] = builderIcon;
	        categories_dictionary["items"].Add(builderIcon);

	        // Edit IP Restrictions
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
	        string restrictionsUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string restrictionsIcon = "  <a href=\"" + restrictionsUrl + "\" title=\"" + RESTRICTIONS_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Firewall_Img + "\" /><span class=\"sbkHav_ButtonText\">IP Restriction<br />Ranges</span></div></a>";
	        icons["IP Restriction Ranges"] = restrictionsIcon;
	        categories_dictionary["settings"].Add(restrictionsIcon);

	        // Edit Settings
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Settings;
	        string settingsUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string settingsIcon = "  <a href=\"" + settingsUrl + "\" title=\"" + SETTINGS_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Settings_Img + "\" /><span class=\"sbkHav_ButtonText\">System-Wide<br />Settings</span></div></a>";
	        icons["System-Wide Settings"] = settingsIcon;
	        categories_dictionary["settings"].Add(settingsIcon);

	        // Reset cache
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Reset;
	        string resetUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string resetIcon = "  <a href=\"" + resetUrl + "\" title=\"" + RESET_CACHE_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.Refresh_Img + "\" /><span class=\"sbkHav_ButtonText\">Reset Cache</span></div></a>";
	        icons["Reset Cache"] = resetIcon;
	        categories_dictionary["settings"].Add(resetIcon);

	        // View permissions report
	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
	        string permissionsUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
	        string permissionsIcon = "  <a href=\"" + permissionsUrl + "\" title=\"" + PERMISSIONS_BRIEF + "\"><div class=\"sbkHav_ButtonDiv\"><img src=\"" + Static_Resources.User_Permission_Img + "\" /><span class=\"sbkHav_ButtonText\">User Permissions<br />Reports</span></div></a>";
	        icons["User Permissions Reports"] = permissionsIcon;
	        categories_dictionary["permissions"].Add(permissionsIcon);

	        // Edit users (REPEAT FROM COMMON TASKS CATEGORY)
	        if (RequestSpecificValues.Current_User.Is_System_Admin)
	        {
	            // Edit users
	            categories_dictionary["permissions"].Add(usersIcon);
	        }

	        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Home;
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
			Output.WriteLine("  <h1>What would you like to manage today?</h1>");

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
            // Add the common tasks icon at the top first
            Output.WriteLine("  <br />");
            Output.WriteLine("  <h2 id=\"common\">Common Tasks</h2>");
            Output.WriteLine("  <div id=\"sbkHav_ButtonOuterDiv\" style=\"padding-top:0\">");
	        display_single_category(Output, "common", String.Empty);
            Output.WriteLine("  </div>");


            Output.WriteLine("  <table id=\"sbkHav_OptionsTable3\">");
            Output.WriteLine("    <tr><td colspan=\"3\"><h2 id=\"appearance\">Appearance</h2></td></tr>");


            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
            string edit_curr_skin_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:30px\">&nbsp;</td>");
            Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + edit_curr_skin_url + "\"><img src=\"" + Static_Resources.Skins_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + edit_curr_skin_url + "\">Edit Current Web Skin</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + EDIT_CURR_SKIN_BRIEF + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + URL_PORTALS_BRIEF + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + WEB_SKINS_BRIEF  + "</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");


            Output.WriteLine("    <tr><td colspan=\"3\"><h2 id=\"collections\">Collections</h2></td></tr>");

            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
            string add_collection_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + add_collection_url + "\"><img src=\"" + Static_Resources.Wizard_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + add_collection_url + "\">Add Collection Wizard</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + ADD_COLLECTION_WIZARD_BRIEF  + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + ALIASES_BRIEF + "</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");


            // Edit item aggregationPermissions
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
            string aggr_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + aggr_url + "\"><img src=\"" + Static_Resources.Aggregations_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + aggr_url + "\">Aggregation Management</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + AGGR_MGMT_BRIEF  + "</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            // Edit Thematic Headings
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
            string thematic_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>&nbsp;</td>");
            Output.WriteLine("      <td><a href=\"" + thematic_url + "\"><img src=\"" + Static_Resources.Thematic_Heading_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + thematic_url + "\">Thematic Headings</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + THEMATIC_HEADING_BRIEF  + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + DEFAULT_METADATA_BRIEF  + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + WORDMARKS_BRIEF   + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + BUILDER_STATUS_BRIEF  + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + RESTRICTIONS_BRIEF  + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + SETTINGS_BRIEF  + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + RESET_CACHE_BRIEF + "</div>");
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
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + PERMISSIONS_BRIEF + "</div>");
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
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">" + USERS_AND_GROUPS_BRIEF  + "</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>"); 
            }


            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Home;

            Output.WriteLine("  </table>");
        }


	    private void write_categorized(TextWriter Output)
	    {
            Output.WriteLine("  <div id=\"sbkHav_ButtonOuterDiv\">");

	        display_single_category(Output, "common", "Common Tasks");
            display_single_category(Output, "appearance", "Appearance");
            display_single_category(Output, "collections", "Collections");
            display_single_category(Output, "common", "Common Tasks");
            display_single_category(Output, "items", "Items");
            display_single_category(Output, "settings", "Settings");
            display_single_category(Output, "permissions", "Users and Permissions");

            Output.WriteLine("  </div>");
	    }
        
	    private void display_single_category(TextWriter Output, string category, string Title)
	    {
	        if ((categories_dictionary.ContainsKey(category)) && ( categories_dictionary[category].Count > 0 ))
	        {
	            Output.WriteLine();
                if ( Title.Length > 0 )
    	            Output.WriteLine("  <h2 id=\"" + category + "\">" + Title + "</h2>");

	            foreach (string icon in categories_dictionary[category])
	            {
	                Output.WriteLine(icon);
	            }
	        }
	    }

        private void write_classic(TextWriter Output)
        {
            Output.WriteLine("  <div id=\"sbkHav_ClassicDiv\">");

            // Add collection wizard
            if (icons["Add Collection Wizard"] != null) Output.WriteLine(icons["Add Collection Wizard"].Replace("sbkHav_ButtonDiv","sbkHav_ButtonDiv2").Replace("<br />"," "));

            // Edit item aggregation
            if (icons["Aggregation Management"] != null) Output.WriteLine(icons["Aggregation Management"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Edit web skins
            if (icons["Web Skins"] != null) Output.WriteLine(icons["Web Skins"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Edit wordmarks
            if (icons["Wordmarks"] != null) Output.WriteLine(icons["Wordmarks"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Edit aggregation aliases
            if (icons["Aggregation Aliases"] != null) Output.WriteLine(icons["Aggregation Aliases"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // View permissions report
            if (icons["User Permissions Reports"] != null) Output.WriteLine(icons["User Permissions Reports"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // View user and user groups
            if (icons["Users and Groups"] != null) Output.WriteLine(icons["Users and Groups"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Edit Default_Metadata
            if (icons["Default Metadata"] != null) Output.WriteLine(icons["Default Metadata"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Edit IP Restrictions
            if (icons["IP Restriction Ranges"] != null) Output.WriteLine(icons["IP Restriction Ranges"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Edit URL Portals
            if (icons["URL Portals"] != null) Output.WriteLine(icons["URL Portals"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Edit Thematic Headings
            if (icons["Thematic Headings"] != null) Output.WriteLine(icons["Thematic Headings"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Edit Settings
            if (icons["System-Wide Settings"] != null) Output.WriteLine(icons["System-Wide Settings"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // View and set SobekCM Builder Status
            if (icons["Builder Status"] != null) Output.WriteLine(icons["Builder Status"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            // Reset cache
            if (icons["Reset Cache"] != null) Output.WriteLine(icons["Reset Cache"].Replace("sbkHav_ButtonDiv", "sbkHav_ButtonDiv2").Replace("<br />", " "));

            Output.WriteLine("  </div>");
        }


        private void write_alphabetical(TextWriter Output)
        {
            Output.WriteLine("  <div id=\"sbkHav_ButtonOuterDiv\">");

            foreach (KeyValuePair<string, string> thisIcon in icons)
            {
                Output.WriteLine(thisIcon.Value);
            }

            Output.WriteLine("  </div>");
        }
	}
}
