#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
        private readonly SobekCM_Item currentItem;
        private readonly Dictionary<string, Wordmark_Icon> iconTable;
        private readonly IP_Restriction_Ranges ipRestrictions;
        private readonly Item_Lookup_Object itemList;
        private readonly iMySobek_Admin_Viewer adminViewer;
        private readonly List<iSearch_Title_Result> pagedResults;
        private readonly Search_Results_Statistics resultsStatistics;
        private readonly Statistics_Dates statsDates;
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

            resultsStatistics = Results_Statistics;
            pagedResults = Paged_Results;
            codeManager = Code_Manager;
            itemList = All_Items_Lookup;
            htmlSkin = HTML_Skin;
            translator = Translator;
            currentCollection = Hierarchy_Object;
            currentItem = Current_Item;
            user = Current_User;
            ipRestrictions = IP_Restrictions;
            iconTable = Icon_Table;
            statsDates = Stats_Date_Range;


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
                    adminViewer = new Thematic_Headings_AdminViewer(user, Current_Mode, Thematic_Headings, Tracer);
                    break;

                case Admin_Type_Enum.Settings:
                    adminViewer = new Settings_AdminViewer(user, Current_Mode, Tracer);
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

        /// <summary> Writes additional HTML needed in the main form before the main place holder but after the other place holders.  </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_HtmlSubwriter.Add_Additional_HTML", "Adding any form elements popup divs");
            if ((currentMode.Logon_Required) || (adminViewer.Contains_Popup_Forms))
            {
                adminViewer.Add_Popup_HTML(Output, Tracer);
            }

            // Also, add any additional stuff here
            adminViewer.Add_HTML_In_Main_Form(Output, Tracer);
        }

        /// <summary> Writes the HTML generated by this my sobek html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_HtmlSubwriter.Write_HTML", "Rendering HTML");

            if ((!adminViewer.Contains_Popup_Forms) && (!currentMode.Logon_Required))
            {
                if ( currentMode.Admin_Type != Admin_Type_Enum.Aggregation_Single )
                {
                    Add_Banner(Output);

                    // Write the general view type selector stuff
                    Write_General_View_Type_Selectors(Output);

                    // Write the bottom tabs
                    Write_Admin_View_Type_Selectors(Output);
                }
            }

            // Add the text here
            adminViewer.Write_HTML(Output, Tracer);
            return false;
        }

        /// <summary> Adds any necessary controls to one of two place holders on the main ASPX page </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="uploadFilesPlaceHolder"> Alternate place holder ( &quot;myUfdcUploadPlaceHolder&quot; )in the fileUploadForm on the main ASPX page</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Controls(PlaceHolder placeHolder, PlaceHolder uploadFilesPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_HtmlSubwriter.Add_Controls", "Build admin viewer and add controls");

            // Add the banner now
            if ((currentMode.Logon_Required) || (adminViewer.Contains_Popup_Forms))
            {
                // Start to build the result to write, with the banner
                StringBuilder header_builder = new StringBuilder();
                StringWriter header_writer = new StringWriter(header_builder);
                Add_Banner( header_writer);

                // Now, add this literal
                LiteralControl header_literal = new LiteralControl(header_builder.ToString());
                placeHolder.Controls.Add(header_literal);
            }

            // Add any controls needed
            adminViewer.Add_Controls(placeHolder, uploadFilesPlaceHolder, Tracer);
         }

        /// <summary> Adds the banner to the response stream from either the html web skin
        /// or from the current item aggreagtion object, depending on flags in the web skin object </summary>
        /// <param name="Output"> Stream to which to write the HTML for the banner </param>
        private void Add_Banner(TextWriter Output)
        {
            Output.WriteLine("<!-- Write the main collection, interface, or institution banner -->");
            if ((htmlSkin != null) && (htmlSkin.Override_Banner))
            {
                Output.WriteLine(htmlSkin.Banner_HTML);
            }
            else
            {
                string url_options = currentMode.URL_Options();
                if (url_options.Length > 0)
                    url_options = "?" + url_options;

                if ((Hierarchy_Object != null) && (Hierarchy_Object.Code != "all"))
                {
                    Output.WriteLine("<a alt=\"" + Hierarchy_Object.ShortName + "\" href=\"" + currentMode.Base_URL + Hierarchy_Object.Code + url_options + "\"><img id=\"mainBanner\" src=\"" + currentMode.Base_URL + Hierarchy_Object.Banner_Image( currentMode.Language, htmlSkin) + "\" alt=\"\" /></a>");
                }
                else
                {
                    if ((Hierarchy_Object != null) && (Hierarchy_Object.Banner_Image(currentMode.Language, htmlSkin).Length > 0))
                    {
                        Output.WriteLine("<a href=\"" + currentMode.Base_URL + url_options + "\"><img id=\"mainBanner\" src=\"" + currentMode.Base_URL + Hierarchy_Object.Banner_Image(currentMode.Language, htmlSkin) + "\" alt=\"\" /></a>");
                    }
                    else
                    {
                        Output.WriteLine("<a href=\"" + currentMode.Base_URL + url_options + "\"><img id=\"mainBanner\" src=\"" + currentMode.Base_URL + "default/images/sobek.jpg\" alt=\"\" /></a>");
                    }
                }
            }
            Output.WriteLine();
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

            // If editing projects, add the mySobek stylesheet as well
            if ((currentMode.Admin_Type == Admin_Type_Enum.Projects) && (currentMode.My_Sobek_SubMode.Length > 0))
            {
                Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Metadata.css\" rel=\"stylesheet\" type=\"text/css\" media=\"screen\" />");
            }
        }

        #region Writes the HTML for the admin view tabs 

        private void Write_Admin_View_Type_Selectors(TextWriter Output)
        {
            const string aggregations = "AGGREGATIONS";
            const string interfaces = "WEB SKINS";
            const string wordmarks = "WORDMARKS";
            const string forwarding = "ALIASES";
            const string users = "USERS";
            const string projects = "PROJECTS";
            const string restrictions = "RESTRICTIONS";
            const string portals = "PORTALS";
            const string builder = "BUILDER";
            const string thematicHeadings = "THEMATIC HEADINGS";

            Admin_Type_Enum mySobekType = currentMode.Admin_Type;
            string submode = currentMode.My_Sobek_SubMode;
            currentMode.My_Sobek_SubMode = String.Empty;

            Output.WriteLine("<div class=\"ShowSelectRow\">");
            Output.WriteLine("");

            // Write the aggregations tab
            if (mySobekType == Admin_Type_Enum.Aggregations_Mgmt)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + aggregations + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + aggregations + Down_Tab_End + "</a>");
            }

            // Write the interfaces tab
            if (mySobekType == Admin_Type_Enum.Interfaces)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + interfaces + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.Admin_Type = Admin_Type_Enum.Interfaces;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + interfaces + Down_Tab_End + "</a>");
            }

            // Write the wordmarks / icon tab
            if (mySobekType == Admin_Type_Enum.Wordmarks)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + wordmarks + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.Admin_Type = Admin_Type_Enum.Wordmarks;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + wordmarks + Down_Tab_End + "</a>");
            }

            // Write the forwarding tab
            if (mySobekType == Admin_Type_Enum.Forwarding)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + forwarding + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.Admin_Type = Admin_Type_Enum.Forwarding;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + forwarding + Down_Tab_End + "</a>");
            }

            // Write the projects tab
            if (mySobekType == Admin_Type_Enum.Projects)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + projects + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.Admin_Type = Admin_Type_Enum.Projects;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + projects + Down_Tab_End + "</a>");
            }

            if (!user.Is_System_Admin)
            {
                // Write the thematic headings tab
                if (mySobekType == Admin_Type_Enum.Thematic_Headings)
                {
                    Output.WriteLine("  " + Down_Selected_Tab_Start + thematicHeadings + Down_Selected_Tab_End);
                }
                else
                {
                    currentMode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + thematicHeadings + Down_Tab_End + "</a>");
                }
            }
            else
            {
                // Write the users tab
                if ((mySobekType == Admin_Type_Enum.Users) || (mySobekType == Admin_Type_Enum.User_Groups))
                {
                    if (submode.Length > 0)
                    {
                        currentMode.Admin_Type = Admin_Type_Enum.User_Groups;
                        currentMode.My_Sobek_SubMode = String.Empty;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Selected_Tab_Start + users + Down_Selected_Tab_End + "</a>");

                    }
                    else
                    {
                        Output.WriteLine("  " + Down_Selected_Tab_Start + users + Down_Selected_Tab_End);
                    }
                }
                else
                {
                    currentMode.Admin_Type = Admin_Type_Enum.Users;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + users + Down_Tab_End + "</a>");
                }

                // Write the restrictions tab
                if (mySobekType == Admin_Type_Enum.IP_Restrictions)
                {
                    Output.WriteLine("  " + Down_Selected_Tab_Start + restrictions + Down_Selected_Tab_End);
                }
                else
                {
                    currentMode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + restrictions + Down_Tab_End + "</a>");
                }

                // Write the url portals tab
                if (mySobekType == Admin_Type_Enum.URL_Portals)
                {
                    Output.WriteLine("  " + Down_Selected_Tab_Start + portals + Down_Selected_Tab_End);
                }
                else
                {
                    currentMode.Admin_Type = Admin_Type_Enum.URL_Portals;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + portals + Down_Tab_End + "</a>");
                }

                // Write the builder tab
                if (mySobekType == Admin_Type_Enum.Builder_Status)
                {
                    Output.WriteLine("  " + Down_Selected_Tab_Start + builder + Down_Selected_Tab_End);
                }
                else
                {
                    currentMode.Admin_Type = Admin_Type_Enum.Builder_Status;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + builder + Down_Tab_End + "</a>");
                }
            }

            currentMode.Admin_Type = mySobekType;
            currentMode.My_Sobek_SubMode = submode;

            Output.WriteLine("");
            Output.WriteLine("</div>");
            Output.WriteLine();

        }

        #endregion

        #region Writes the HTML for the standard my Sobek view tabs

        private void Write_General_View_Type_Selectors(TextWriter Output)
        {
            // Get ready to draw the tabs
            string sobek_home = currentMode.SobekCM_Instance_Abbreviation.ToUpper() + " HOME";
            string my_sobek_home = "my" + currentMode.SobekCM_Instance_Abbreviation.ToUpper() + " HOME";
            const string myLibrary = "MY LIBRARY";
            const string myPreferences = "MY ACCOUNT";
            const string internalTab = "INTERNAL";
            string sobek_admin = "SYSTEM ADMIN";
            if ((user != null) && (user.Is_Portal_Admin) && (!user.Is_System_Admin))
                sobek_admin = "PORTAL ADMIN";

            Admin_Type_Enum mySobekType = currentMode.Admin_Type;
            string submode = currentMode.My_Sobek_SubMode;
            currentMode.My_Sobek_SubMode = String.Empty;

            Output.WriteLine("<div class=\"ViewsBrowsesRow\">");
            Output.WriteLine("");

            // Write the Sobek home tab
            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
            currentMode.Home_Type = Home_Type_Enum.List;
            Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + sobek_home + Unselected_Tab_End + "</a>");
            currentMode.Mode = Display_Mode_Enum.My_Sobek;

            if (user != null && ((HttpContext.Current.Session["user"] != null) && (currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Log_Out) && (!user.Is_Temporary_Password)))
            {
                // Write the mySobek home tab
                   currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + my_sobek_home + Unselected_Tab_End + "</a>");

                // Write the folders tab
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + myLibrary + Unselected_Tab_End + "</a>");

                // Write the preferences tab
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + myPreferences + Unselected_Tab_End + "</a>");

                // If this user is internal, add that
                if (user.Is_Internal_User)
                {
                    currentMode.Mode = Display_Mode_Enum.Internal;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + internalTab + Unselected_Tab_End + "</a>");
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                }

                // Write the sobek admin tab
                if ((user.Is_System_Admin) || (user.Is_Portal_Admin))
                {
                    currentMode.Mode = Display_Mode_Enum.Administrative;
                    currentMode.Admin_Type = Admin_Type_Enum.Home;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + sobek_admin + Unselected_Tab_End + "</a>");
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                }
            }

            currentMode.Mode = Display_Mode_Enum.Administrative;
            currentMode.Admin_Type = mySobekType;
            currentMode.My_Sobek_SubMode = submode;

            Output.WriteLine("");
            Output.WriteLine("</div>");
            Output.WriteLine();

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

        #endregion

    }
}
