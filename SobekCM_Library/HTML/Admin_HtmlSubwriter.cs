#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.Items;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.MySobekViewer;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.HTML
{
    public class Admin_HtmlSubwriter: abstractHtmlSubwriter
    {
        private readonly Aggregation_Code_Manager codeManager;
        private readonly Item_Aggregation currentCollection;
	    private readonly Dictionary<string, Wordmark_Icon> iconTable;
        private readonly IP_Restriction_Ranges ipRestrictions;
        private readonly Item_Lookup_Object itemList;
        private readonly iMySobek_Admin_Viewer adminViewer;
	    private readonly Language_Support_Info translator;
        private readonly User_Object user;
 
        #region Constructor, which also creates the applicable MySobekViewer object

        /// <summary> Constructor for a new instance of the Admin_HtmlSubwriter class </summary>
        /// <param name="Results_Statistics"> Information about the entire set of results for a browse of a user's bookshelf folder </param>
        /// <param name="Paged_Results"> Single page of results for a browse of a user's bookshelf folder, within the entire set </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Item">Current item to edit, if the user is requesting to edit an item</param>
        /// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations </param>
        /// <param name="Web_Skin_Collection"> Collection of all the web skins </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="Icon_Table"> Dictionary of all the wordmark/icons which can be tagged to the items </param>
        /// <param name="IP_Restrictions"> List of all IP Restriction ranges in use by this digital library </param>
        /// <param name="URL_Portals"> List of all web portals into this system </param>
        /// <param name="Stats_Date_Range"> Object contains the start and end dates for the statistical data in the database </param>
        /// <param name="Thematic_Headings"> Headings under which all the highlighted collections on the home page are organized </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Admin_HtmlSubwriter(Search_Results_Statistics Results_Statistics,
                                     List<iSearch_Title_Result> Paged_Results,
                                     Aggregation_Code_Manager Code_Manager,
                                     Item_Lookup_Object All_Items_Lookup,
                                     Item_Aggregation Hierarchy_Object,
                                     SobekCM_Skin_Object HTML_Skin,
                                     Language_Support_Info Translator,
                                     SobekCM_Navigation_Object Current_Mode,
                                     SobekCM_Item Current_Item,
                                     Dictionary<string,string> Aggregation_Aliases,
                                     SobekCM_Skin_Collection Web_Skin_Collection,
                                     User_Object Current_User,
                                     IP_Restriction_Ranges IP_Restrictions,
                                     Dictionary<string, Wordmark_Icon> Icon_Table,
                                     Portal_List URL_Portals,
                                     Statistics_Dates Stats_Date_Range,
                                     List<Thematic_Heading> Thematic_Headings,
                                     Custom_Tracer Tracer )
        {

            Tracer.Add_Trace("Admin_HtmlSubwriter.Constructor", "Saving values and geting user object back from the session");

	        codeManager = Code_Manager;
            itemList = All_Items_Lookup;
            htmlSkin = HTML_Skin;
            translator = Translator;
            currentCollection = Hierarchy_Object;
	        user = Current_User;
            ipRestrictions = IP_Restrictions;
            iconTable = Icon_Table;


	        if (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Log_Out)
            {
                Tracer.Add_Trace("Admin_HtmlSubwriter.Constructor", "Performing logout");

                HttpContext.Current.Session["user"] = null;
                HttpContext.Current.Response.Redirect("?", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                Current_Mode.Request_Completed = true;
                return;
            }

            if ((Current_Mode.My_Sobek_Type != My_Sobek_Type_Enum.Logon) && (user != null) && (user.Is_Temporary_Password))
            {
                Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.New_Password;
            }

            if (Current_Mode.Logon_Required)
                Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;

            // If the user is not an admin, and admin was selected, reroute this
            if ((!Current_User.Is_System_Admin) && (!Current_User.Is_Portal_Admin))
            {
                Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                Current_Mode.My_Sobek_SubMode = String.Empty;
                Current_Mode.Redirect();
                return;
            }

            Tracer.Add_Trace("Admin_HtmlSubwriter.Constructor", "Building the my sobek viewer object");
            switch (Current_Mode.Admin_Type)
            {
                case Admin_Type_Enum.Aggregation_Single:
                    adminViewer = new Aggregation_Single_AdminViewer(user, Current_Mode, codeManager, Thematic_Headings, Web_Skin_Collection, Tracer);
                    break;

                case Admin_Type_Enum.Home:
                    adminViewer = new Home_AdminViewer(user, Current_Mode, Tracer);
                    break;

                case Admin_Type_Enum.Builder_Status:
                    adminViewer = new Builder_AdminViewer(user, Current_Mode);
                    break;

                case Admin_Type_Enum.Interfaces:
                    adminViewer = new Skins_AdminViewer(user, Current_Mode, Web_Skin_Collection, Tracer);
                    break;

                case Admin_Type_Enum.Forwarding:
                    adminViewer = new Aliases_AdminViewer(user, Current_Mode, Aggregation_Aliases, Tracer);
                    break;

                case Admin_Type_Enum.Wordmarks:
                    adminViewer = new Wordmarks_AdminViewer(user, Current_Mode, Tracer);
                    break;

                case Admin_Type_Enum.URL_Portals:
                    adminViewer = new Portals_AdminViewer(user, Current_Mode, URL_Portals, Tracer);
                    break;

                case Admin_Type_Enum.Users:
                    adminViewer = new Users_AdminViewer(user, Current_Mode, codeManager, Tracer);
                    break;

                case Admin_Type_Enum.User_Groups:
                    adminViewer = new User_Group_AdminViewer(user, Current_Mode, codeManager, Tracer);
                    break;

                case Admin_Type_Enum.Aggregations_Mgmt:
                    adminViewer = new Aggregations_Mgmt_AdminViewer(user, Current_Mode, codeManager, Tracer);
                    break;

                case Admin_Type_Enum.IP_Restrictions:
                    adminViewer = new IP_Restrictions_AdminViewer(user, Current_Mode, ipRestrictions, Tracer);
                    break;

                case Admin_Type_Enum.Thematic_Headings:
                    adminViewer = new Thematic_Headings_AdminViewer(user, Current_Mode, Thematic_Headings, codeManager, Tracer);
                    break;

                case Admin_Type_Enum.Settings:
                    adminViewer = new Settings_AdminViewer(user, Current_Mode, Tracer);
                    break;

                case Admin_Type_Enum.Item_Tracking:
                    adminViewer = new Track_Item_MySobekViewer(user, Current_Mode, Current_Item, codeManager, Tracer);
                    break;

                case Admin_Type_Enum.Projects:
                    if (Current_Mode.My_Sobek_SubMode.Length > 1)
                    {
                        string project_code = Current_Mode.My_Sobek_SubMode.Substring(1);
                        Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Checking cache for valid project file");
                        if (user != null)
                        {
                            SobekCM_Item projectObject = Cached_Data_Manager.Retrieve_Project(user.UserID, project_code, Tracer);
                            if (projectObject != null)
                            {
                                Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Valid project file found in cache");
                                adminViewer = new Edit_Item_Metadata_MySobekViewer(user, Current_Mode, itemList, projectObject, codeManager, iconTable, htmlSkin, Tracer);
                            }
                            else
                            {
                                if (SobekCM_Database.Get_All_Projects_Templates(Tracer).Tables[0].Select("ProjectCode='" + project_code + "'").Length > 0)
                                {
                                    Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Building project file from (possible) PMETS");
                                    string pmets_file = SobekCM_Library_Settings.Base_MySobek_Directory + "projects\\" + Current_Mode.My_Sobek_SubMode.Substring(1) + ".pmets";
                                    SobekCM_Item pmets_item = File.Exists(pmets_file) ? SobekCM_Item.Read_METS(pmets_file) : new SobekCM_Item();
                                    pmets_item.Bib_Info.Main_Title.Title = "Project level metadata for '" + project_code + "'";
                                    pmets_item.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Project;
                                    pmets_item.BibID = project_code.ToUpper();
                                    pmets_item.VID = "00001";
                                    pmets_item.Source_Directory = SobekCM_Library_Settings.Base_MySobek_Directory +  "projects\\";

                                    Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Adding project file to cache");

                                    Cached_Data_Manager.Store_Project(user.UserID, project_code, pmets_item, Tracer);

                                    adminViewer = new Edit_Item_Metadata_MySobekViewer(user, Current_Mode, itemList, pmets_item, codeManager, iconTable, htmlSkin, Tracer);
                                }
                            }
                        }
                    }


                    if (adminViewer == null)
                        adminViewer = new Projects_AdminViewer(user, Current_Mode, Tracer);
                    break;
            }

            // Pass in the navigation and translator information
            adminViewer.CurrentMode = Current_Mode;
            adminViewer.Translator = translator;

        }

        #endregion

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML writer. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get
            {
                List<HtmlSubwriter_Behaviors_Enum> returnVal = new List<HtmlSubwriter_Behaviors_Enum>();


                returnVal.Add(HtmlSubwriter_Behaviors_Enum.Suppress_Banner);

                if (Contains_Popup_Forms)
                {
                    returnVal.Add(HtmlSubwriter_Behaviors_Enum.Suppress_Header);
                    returnVal.Add(HtmlSubwriter_Behaviors_Enum.Suppress_Footer);
                }

                return returnVal;
            }
        }

        /// <summary> Property indicates if the current mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        public bool Contains_Popup_Forms
        {
            get
            {
                return adminViewer.Contains_Popup_Forms;
            }
        }

		/// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
		/// for the current request, or if it can be hidden. </summary>
		public override bool Upload_File_Possible
		{
			get
			{
				if (user == null)
					return false;

				if ( currentMode.Admin_Type == Admin_Type_Enum.Wordmarks )
					return true;

				return false;
			}
		}

        /// <summary> Writes additional HTML needed in the main form before the main place holder but after the other place holders.  </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_HtmlSubwriter.Write_Additional_HTML", "Adding any form elements popup divs");
            if ((currentMode.Logon_Required) || (adminViewer.Contains_Popup_Forms))
            {
                adminViewer.Add_Popup_HTML(Output, Tracer);
            }
        }

		/// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("Admin_HtmlSubwriter.Write_HTML", "Rendering HTML");

			//if (currentMode.Admin_Type != Admin_Type_Enum.Wordmarks)
			//	return;

			//// Add the text here
			//adminViewer.Write_HTML(Output, Tracer);
			return;
		}

        /// <summary> Writes the HTML generated by this my sobek html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_HtmlSubwriter.Write_HTML", "Rendering HTML");

	       // if (currentMode.Admin_Type == Admin_Type_Enum.Wordmarks)
		   //     return false;

            if ((!adminViewer.Contains_Popup_Forms) && (!currentMode.Logon_Required))
            {
                if ( currentMode.Admin_Type != Admin_Type_Enum.Aggregation_Single )
                {
                    // Add the banner
                    Add_Banner(Output, "sbkAhs_BannerDiv", currentMode, htmlSkin, currentCollection);

                    // Add the user-specific main menu
                    UserSpecific_MainMenu_Writer.Add_Main_Menu(Output, currentMode, user);

                    // Start the page container
                    Output.WriteLine("<div id=\"pagecontainer\">");
                    Output.WriteLine("<br />");

                    // Add the box with the title
                    if ((currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Folder_Management) || (currentMode.My_Sobek_SubMode != "submitted items"))
                    {
                        Output.WriteLine("<div class=\"SobekSearchPanel\">");
                        if (adminViewer != null)
                            Output.WriteLine("  <h1>" + adminViewer.Web_Title + "</h1>");
                        else if (user != null) Output.WriteLine("  <h1>Welcome back, " + user.Nickname + "</h1>");
                        Output.WriteLine("</div>");
                        Output.WriteLine();
                    }
                }
            }

            // Add the text here
            adminViewer.Write_HTML(Output, Tracer);
            return false;
        }


		/// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
		/// <param name="Output"> Stream to which to write the text for this main writer </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("Admin_HtmlSubwriter.Write_ItemNavForm_Closing", "");

			// Also, add any additional stuff here
			adminViewer.Add_HTML_In_Main_Form(Output, Tracer);
		}

		/// <summary> Add controls directly to the form in the main control area placeholder</summary>
		/// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_HtmlSubwriter.Add_Controls", "Build admin viewer and add controls");

            // Add the banner now
            if ((currentMode.Logon_Required) || (adminViewer.Contains_Popup_Forms))
            {
                // Start to build the result to write, with the banner
                StringBuilder header_builder = new StringBuilder();
                StringWriter header_writer = new StringWriter(header_builder);
                Add_Banner(header_writer, "sbkAhs_BannerDiv", currentMode, htmlSkin, currentCollection);

                // Now, add this literal
                LiteralControl header_literal = new LiteralControl(header_builder.ToString());
				MainPlaceHolder.Controls.Add(header_literal);
            }

            // Add any controls needed
			adminViewer.Add_Controls(MainPlaceHolder, Tracer);
         }

 
        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get { return "{0} System Administration"; }
        }

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual html subwriters </remarks>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <meta name=\"robots\" content=\"index, nofollow\" />");

            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Admin.css\" rel=\"stylesheet\" type=\"text/css\" media=\"screen\" />");
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_UserMenu.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");

            // If editing projects, add the mySobek stylesheet as well
            if ((currentMode.Admin_Type == Admin_Type_Enum.Projects) && (currentMode.My_Sobek_SubMode.Length > 0))
            {
                Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Metadata.css\" rel=\"stylesheet\" type=\"text/css\" media=\"screen\" />");
            }
        }

        /// <summary> Writes final HTML after all the forms </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<!-- Close the pagecontainer div -->");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }

    }
}
