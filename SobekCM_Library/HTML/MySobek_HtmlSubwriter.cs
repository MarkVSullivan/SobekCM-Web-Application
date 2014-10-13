#region Using directives

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
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
using SobekCM.Core.Users;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> My Sobek html subwriter is used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. <br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create this necessary subwriter since this action requires authentication. </li>
    /// <li>This class will create a mySobek subwriter (extending <see cref="MySobekViewer.abstract_MySobekViewer"/> ) for the specified task.The mySobek subwriter creates an instance of this viewer to view and edit existing item aggregationPermissions in this digital library</li>
    /// </ul></remarks>
    public class MySobek_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly Aggregation_Code_Manager codeManager;
        private readonly Item_Aggregation currentCollection;
        private readonly SobekCM_Item currentItem;
        private readonly Dictionary<string, Wordmark_Icon> iconTable;
	    private readonly Item_Lookup_Object itemList;
        private readonly abstract_MySobekViewer mySobekViewer;
        private readonly List<iSearch_Title_Result> pagedResults;
        private readonly Search_Results_Statistics resultsStatistics;
        private readonly Statistics_Dates statsDates;
        private readonly Language_Support_Info translator;
        private readonly User_Object user;
 
        #region Constructor, which also creates the applicable MySobekViewer object

	    /// <summary> Constructor for a new instance of the MySobek_HtmlSubwriter class </summary>
	    /// <param name="Results_Statistics"> Information about the entire set of results for a browse of a user's bookshelf folder </param>
	    /// <param name="Paged_Results"> Single page of results for a browse of a user's bookshelf folder, within the entire set </param>
	    /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
	    /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
	    /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
	    /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
	    /// <param name="Translator"> Language support object which handles simple translational duties </param>
	    /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
	    /// <param name="Current_Item">Current item to edit, if the user is requesting to edit an item</param>
	    /// <param name="Current_User"> Currently logged on user </param>
	    /// <param name="Icon_Table"> Dictionary of all the wordmark/icons which can be tagged to the items </param>
	    /// <param name="Stats_Date_Range"> Object contains the start and end dates for the statistical data in the database </param>
		/// <param name="HTML_Skin_Collection"> HTML Web skin collection which controls the overall appearance of this digital library </param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
	    public MySobek_HtmlSubwriter(Search_Results_Statistics Results_Statistics,
                                     List<iSearch_Title_Result> Paged_Results,
                                     Aggregation_Code_Manager Code_Manager,
                                     Item_Lookup_Object All_Items_Lookup,
                                     Item_Aggregation Hierarchy_Object,
                                     SobekCM_Skin_Object HTML_Skin,
                                     Language_Support_Info Translator,
                                     SobekCM_Navigation_Object Current_Mode,
                                     SobekCM_Item Current_Item,
                                     User_Object Current_User,
                                     Dictionary<string, Wordmark_Icon> Icon_Table,
                                     Statistics_Dates Stats_Date_Range,
									 SobekCM_Skin_Collection HTML_Skin_Collection,
                                     List<User_Group> userGroups,
                                     IP_Restriction_Ranges ipRestrictions,
                                     Custom_Tracer Tracer )
        {

            Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Saving values and geting user object back from the session");

            resultsStatistics = Results_Statistics;
            pagedResults = Paged_Results;
            codeManager = Code_Manager;
            itemList = All_Items_Lookup;
            Skin = HTML_Skin;
            translator = Translator;
            currentCollection = Hierarchy_Object;
            currentItem = Current_Item;
            user = Current_User;
	        iconTable = Icon_Table;
            statsDates = Stats_Date_Range;


            if (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Log_Out)
            {
                Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Performing logout");

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

            Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Building the my sobek viewer object");
            switch (Current_Mode.My_Sobek_Type)
            {
                case My_Sobek_Type_Enum.Home:
                    mySobekViewer = new Home_MySobekViewer(user, Tracer);
                    break;

                case My_Sobek_Type_Enum.New_Item:
                    mySobekViewer = new New_Group_And_Item_MySobekViewer(user, Current_Mode, itemList, codeManager, iconTable, Skin, translator, HTML_Skin_Collection, Tracer);
                    break;

                case My_Sobek_Type_Enum.Folder_Management:
                    mySobekViewer = new Folder_Mgmt_MySobekViewer(user, resultsStatistics, pagedResults, codeManager, itemList, currentCollection, Skin, translator, Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.Saved_Searches:
                    mySobekViewer = new Saved_Searches_MySobekViewer(user, translator, Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.Preferences:
                    mySobekViewer = new Preferences_MySobekViewer(user, Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.Logon:
                    mySobekViewer = new Logon_MySobekViewer(Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.New_Password:
                    mySobekViewer = new NewPassword_MySobekViewer(user, Tracer);
                    break;

                case My_Sobek_Type_Enum.Delete_Item:
                    mySobekViewer = new Delete_Item_MySobekViewer(user, Current_Mode, currentItem, All_Items_Lookup, Tracer);
                    break;

                case My_Sobek_Type_Enum.Edit_Item_Behaviors:
                    mySobekViewer = new Edit_Item_Behaviors_MySobekViewer(user, Current_Mode, currentItem, codeManager, Tracer);
                    break;

                case My_Sobek_Type_Enum.Edit_Item_Metadata:
					mySobekViewer = new Edit_Item_Metadata_MySobekViewer(user, Current_Mode, itemList, currentItem, codeManager, iconTable, Skin, translator, HTML_Skin_Collection, Tracer);
                    break;

                case My_Sobek_Type_Enum.Edit_Item_Permissions:
                    mySobekViewer = new Edit_Item_Permissions_MySobekViewer(user, Current_Mode, currentItem, Skin, translator, userGroups, ipRestrictions, Tracer );
                    break;

                case My_Sobek_Type_Enum.File_Management:
					mySobekViewer = new File_Management_MySobekViewer(user, Current_Mode, Current_Item, itemList, codeManager, iconTable, Skin, translator, HTML_Skin_Collection, Tracer);
                    break;

                case My_Sobek_Type_Enum.Edit_Group_Behaviors:
                    mySobekViewer = new Edit_Group_Behaviors_MySobekViewer(user, Current_Mode, currentItem, codeManager, Tracer);
                    break;



                case My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy:
                    mySobekViewer = new Edit_Serial_Hierarchy_MySobekViewer(user);
                    break;

                case My_Sobek_Type_Enum.Item_Tracking:
                    mySobekViewer = new Track_Item_MySobekViewer(user, Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.Group_Add_Volume:
                    // Pull the list of items tied to this group
                    SobekCM_Items_In_Title itemsInTitle = Cached_Data_Manager.Retrieve_Items_In_Title(currentItem.BibID, Tracer);
                    if (itemsInTitle == null)
                    {
                        // Get list of information about this item group and save the item list
                        DataSet itemDetails = SobekCM_Database.Get_Item_Group_Details(currentItem.BibID, Tracer);
                        itemsInTitle = new SobekCM_Items_In_Title(itemDetails.Tables[1]);

                        // Store in cache if retrieved
                        Cached_Data_Manager.Store_Items_In_Title(currentItem.BibID, itemsInTitle, Tracer);
                    }
					mySobekViewer = new Group_Add_Volume_MySobekViewer(user, Current_Mode, itemList, currentItem, codeManager, iconTable, Skin, itemsInTitle, translator, HTML_Skin_Collection, Tracer);
                    break;

                case My_Sobek_Type_Enum.Group_AutoFill_Volumes:
                    mySobekViewer = new Group_AutoFill_Volume_MySobekViewer(user);
                    break;

                case My_Sobek_Type_Enum.Group_Mass_Update_Items:
                    mySobekViewer = new Mass_Update_Items_MySobekViewer(user, Current_Mode, currentItem, codeManager, Tracer);
                    break;

                case My_Sobek_Type_Enum.Page_Images_Management:
                    mySobekViewer = new Page_Image_Upload_MySobekViewer(user, Current_Mode, Current_Item, itemList, codeManager, iconTable, Skin, translator, Tracer );
                    break;

                case My_Sobek_Type_Enum.User_Tags:
                    mySobekViewer = new User_Tags_MySobekViewer(user, Tracer);
                    break;

                case My_Sobek_Type_Enum.User_Usage_Stats:
                    mySobekViewer = new User_Usage_Stats_MySobekViewer(user, Current_Mode, statsDates, Tracer);
                    break;
            }

            // Pass in the navigation and translator information
            mySobekViewer.CurrentMode = Current_Mode;
            mySobekViewer.Translator = translator;

        }

        #endregion

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get {
	            return mySobekViewer != null ? mySobekViewer.Viewer_Behaviors : emptybehaviors;
            }
        }

        /// <summary> Property indicates if the current mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        public bool Contains_Popup_Forms
        {
            get
            {
                return mySobekViewer.Contains_Popup_Forms;
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

				if (Mode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management)
					return false;

				if (((Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item) && (Mode.My_Sobek_SubMode.Length > 0) && (Mode.My_Sobek_SubMode[0] == '8')) ||
				    (Mode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management) || (Mode.My_Sobek_Type == My_Sobek_Type_Enum.Page_Images_Management))
					return true;

				return false;
			}
		}


		/// <summary> Write any additional values within the HTML Head of the
		/// final served page </summary>
		/// <param name="Output"> Output stream currently within the HTML head tags </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
		{
			Output.WriteLine("  <meta name=\"robots\" content=\"index, nofollow\" />");

#if DEBUG
			Output.WriteLine("  <link href=\"" + Mode.Base_URL + "default/SobekCM_Metadata.css\" rel=\"stylesheet\" type=\"text/css\" />");
            Output.WriteLine("  <link href=\"" + Mode.Base_URL + "default/SobekCM_MySobek.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
#else
			Output.WriteLine("  <link href=\"" + Mode.Base_URL + "default/SobekCM_Metadata.min.css\" rel=\"stylesheet\" type=\"text/css\" />");
			Output.WriteLine("  <link href=\"" + Mode.Base_URL + "default/SobekCM_MySobek.min.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
#endif


			// If we are currently uploading files, add those specific upload styles 
			if (((Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item) && (Mode.My_Sobek_SubMode.Length > 0) && (Mode.My_Sobek_SubMode[0] == '8')) || (Mode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management) || (Mode.My_Sobek_Type == My_Sobek_Type_Enum.Page_Images_Management))
			{
#if DEBUG
                Output.WriteLine("  <script src=\"" + Mode.Base_URL + "default/scripts/uploadifive/jquery.uploadifive.js\" type=\"text/javascript\"></script>");
                Output.WriteLine("  <script src=\"" + Mode.Base_URL + "default/scripts/uploadify/jquery.uploadify.js\" type=\"text/javascript\"></script>");
#else
				Output.WriteLine("  <script src=\"" + Mode.Base_URL + "default/scripts/uploadifive/jquery.uploadifive.min.js\" type=\"text/javascript\"></script>");
				Output.WriteLine("  <script src=\"" + Mode.Base_URL + "default/scripts/uploadify/jquery.uploadify.min.js\" type=\"text/javascript\"></script>");
#endif

				Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Mode.Base_URL + "default/scripts/uploadifive/uploadifive.css\">");
				Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Mode.Base_URL + "default/scripts/uploadify/uploadify.css\">");
			}

			if (( mySobekViewer != null ) && ( mySobekViewer.Viewer_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter)))
			{
#if DEBUG
                Output.WriteLine("  <link href=\"" + Mode.Base_URL + "default/SobekCM_Item.css\" rel=\"stylesheet\" type=\"text/css\" />");
#else
			Output.WriteLine("  <link href=\"" + Mode.Base_URL + "default/SobekCM_Item.min.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
#endif
			}
		}


        /// <summary> Writes the HTML generated by this my sobek html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_HTML", "Rendering HTML");

            if ((HttpContext.Current.Session["agreement_date"] == null) && (Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item ) && ((Mode.My_Sobek_SubMode.Length == 0) || (Mode.My_Sobek_SubMode[0] != '1')))
            {
                Mode.My_Sobek_SubMode = "1";
            }
                // A few cases skip the view selectors at the top entirely
	        if (mySobekViewer.Standard_Navigation_Type == MySobek_Included_Navigation_Enum.Standard)
	        {
		        // Add the user-specific main menu
		        MainMenus_Helper_HtmlSubWriter.Add_UserSpecific_Main_Menu(Output, Mode, user);

		        // Start the page container
		        Output.WriteLine("<div id=\"pagecontainer\">");
		        Output.WriteLine("<br />");
	        }
			else if (mySobekViewer.Standard_Navigation_Type == MySobek_Included_Navigation_Enum.LogOn)
			{
				// Add the item views
				Output.WriteLine("<!-- Add the main user-specific menu -->");
				Output.WriteLine("<div id=\"sbkUsm_MenuBar\" class=\"sbkMenu_Bar\">");
				Output.WriteLine("<ul class=\"sf-menu\">");

				// Get ready to draw the tabs
				string sobek_home_text = Mode.SobekCM_Instance_Abbreviation + " Home";

				// Add the 'SOBEK HOME' first menu option and suboptions
				Mode.Mode = Display_Mode_Enum.Aggregation;
				Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
				Mode.Home_Type = Home_Type_Enum.List;
				Output.WriteLine("\t\t<li id=\"sbkUsm_Home\" class=\"sbkMenu_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a></li>");
				Output.WriteLine("\t</ul></div>");

				Output.WriteLine("<!-- Initialize the main user menu -->");
				Output.WriteLine("<script>");
				Output.WriteLine("  jQuery(document).ready(function () {");
				Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
				Output.WriteLine("  });");
				Output.WriteLine("</script>");
				Output.WriteLine();

				// Restore the current view information type
				Mode.Mode = Display_Mode_Enum.My_Sobek;

				// Start the page container
				Output.WriteLine("<div id=\"pagecontainer\">");
				Output.WriteLine("<br />");


			}
			else if ( !Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter))
			{
				// Start the page container
				Output.WriteLine("<div id=\"pagecontainer\">");
			}

            // Add the text here
            mySobekViewer.Write_HTML(Output, Tracer);

            return false;
        }



		/// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
		/// <param name="Output"> Stream to which to write the text for this main writer </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_ItemNavForm_Closing", "");

			// Also, add any additional stuff here
			mySobekViewer.Write_ItemNavForm_Opening(Output, Tracer);
		}

		/// <summary> Writes additional HTML needed in the main form before the main place holder but after the other place holders.  </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_Additional_HTML", "Adding any form elements popup divs");
			if ((Mode.Logon_Required) || (mySobekViewer.Contains_Popup_Forms))
			{
				mySobekViewer.Add_Popup_HTML(Output, Tracer);
			}
		}


	    /// <summary> Adds any necessary controls to one of two place holders on the main ASPX page </summary>
	    /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
	    public void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
	    {
		    Tracer.Add_Trace("MySobek_HtmlSubwriter.Add_Controls", "Build my sobek viewer and add controls");

		    // Add any controls needed
		    if (mySobekViewer != null)
			    mySobekViewer.Add_Controls(MainPlaceHolder, Tracer);
	    }

	    /// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
		/// <param name="Output"> Stream to which to write the text for this main writer </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_ItemNavForm_Closing", "");

			// Also, add any additional stuff here
			mySobekViewer.Write_ItemNavForm_Closing(Output, Tracer);
		}

        /// <summary> Writes final HTML after all the forms </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            
	        if (!Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter))
	        {
				Output.WriteLine("<!-- Close the pagecontainer div -->");
				Output.WriteLine("</div>");
				Output.WriteLine();
	        }

        }

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		public override string Container_CssClass
		{
			get
			{
				if ((Mode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Metadata) && (Mode.My_Sobek_SubMode.IndexOf("0.2") == 0))
					return "container-inner1000";

				if ((Mode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Group_Behaviors) || (Mode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Behaviors) ||
				    (Mode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Metadata) || (Mode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Add_Volume) ||
				    (Mode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Mass_Update_Items) || (Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item))
				{
					return "container-inner1000";
				}

				return base.Container_CssClass;
			}
		}
    }
}
