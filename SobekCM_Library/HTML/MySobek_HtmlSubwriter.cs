#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
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
    /// <summary> My Sobek html subwriter is used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. <br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create this necessary subwriter since this action requires authentication. </li>
    /// <li>This class will create a mySobek subwriter (extending <see cref="MySobekViewer.abstract_MySobekViewer"/> ) for the specified task.The mySobek subwriter creates an instance of this viewer to view and edit existing item aggregations in this digital library</li>
    /// </ul></remarks>
    public class MySobek_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly Aggregation_Code_Manager codeManager;
        private readonly Item_Aggregation currentCollection;
        private readonly SobekCM_Item currentItem;
        private readonly Dictionary<string, Wordmark_Icon> iconTable;
        private readonly IP_Restriction_Ranges ipRestrictions;
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
        /// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations </param>
        /// <param name="Web_Skin_Collection"> Collection of all the web skins </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="Icon_Table"> Dictionary of all the wordmark/icons which can be tagged to the items </param>
        /// <param name="IP_Restrictions"> List of all IP Restriction ranges in use by this digital library </param>
        /// <param name="URL_Portals"> List of all web portals into this system </param>
        /// <param name="Stats_Date_Range"> Object contains the start and end dates for the statistical data in the database </param>
        /// <param name="Thematic_Headings"> Headings under which all the highlighted collections on the home page are organized </param>
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

            Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Saving values and geting user object back from the session");

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
                Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Performing logout");

                HttpContext.Current.Session["user"] = null;
                HttpContext.Current.Response.Redirect("?");
            }

            if ((Current_Mode.My_Sobek_Type != My_Sobek_Type_Enum.Logon) && (user != null) && (user.Is_Temporary_Password))
            {
                Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.New_Password;
            }

            if (Current_Mode.Logon_Required)
                Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;

            // If the user is not an admin, and admin was selected, reroute this
            if ((Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Aggregations_Mgmt) || (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Forwarding) || (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Interfaces) || (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Projects) || (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Users) || (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_User_Groups) || (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Wordmarks) || (Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_IP_Restrictions) || ( Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Builder_Status ) || ( Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Settings ) || ( Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Thematic_Headings))
            {
                if ((!Current_User.Is_System_Admin) && ( !Current_User.Is_Portal_Admin ))
                {
                    Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                    Current_Mode.My_Sobek_SubMode = String.Empty;
                }
            }

            Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Building the my sobek viewer object");
            switch (Current_Mode.My_Sobek_Type)
            {
                case My_Sobek_Type_Enum.Admin_Aggregation_Single:
                    mySobekViewer = new Aggregation_Single_Admin_MySobekViewer(user, Current_Mode, codeManager, Thematic_Headings, Web_Skin_Collection, Tracer);
                    break;

                case My_Sobek_Type_Enum.Home:
                    mySobekViewer = new Home_MySobekViewer(user, Tracer);
                    break;

                case My_Sobek_Type_Enum.New_Item:
                    mySobekViewer = new New_Group_And_Item_MySobekViewer(user, Current_Mode, itemList, codeManager, iconTable, htmlSkin, translator, Tracer);
                    break;

                case My_Sobek_Type_Enum.Folder_Management:
                    mySobekViewer = new Folder_Mgmt_MySobekViewer(user, resultsStatistics, pagedResults, codeManager, itemList, currentCollection, htmlSkin, translator, Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.Saved_Searches:
                    mySobekViewer = new Saved_Searches_MySobekViewer(user, translator, Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.Preferences:
                    mySobekViewer = new Preferences_MySobekViewer(user, Tracer);
                    break;

                case My_Sobek_Type_Enum.Logon:
                    mySobekViewer = new Logon_MySobekViewer(Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.New_Password:
                    mySobekViewer = new NewPassword_MySobekViewer(user, Tracer);
                    break;

                case My_Sobek_Type_Enum.Delete_Item:
                    mySobekViewer = new Delete_Item_MySobekViewer(user, Current_Mode, All_Items_Lookup, Tracer);
                    break;

                case My_Sobek_Type_Enum.Edit_Item_Behaviors:
                    mySobekViewer = new Edit_Item_Behaviors_MySobekViewer(user, Current_Mode, currentItem, codeManager, Tracer);
                    break;

                case My_Sobek_Type_Enum.Edit_Item_Metadata:
                    mySobekViewer = new Edit_Item_Metadata_MySobekViewer(user, Current_Mode, itemList, currentItem, codeManager, iconTable, htmlSkin, Tracer);
                    break;

                case My_Sobek_Type_Enum.File_Management:
                    mySobekViewer = new File_Management_MySobekViewer(user, Current_Mode, Current_Item, itemList, codeManager, iconTable, htmlSkin, translator, Tracer);
                    break;

                case My_Sobek_Type_Enum.Edit_Group_Behaviors:
                    mySobekViewer = new Edit_Group_Behaviors_MySobekViewer(user, Current_Mode, currentItem, codeManager, Tracer);
                    break;

                case My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy:
                    mySobekViewer = new Edit_Serial_Hierarchy_MySobekViewer(user);
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
                    mySobekViewer = new Group_Add_Volume_MySobekViewer(user, Current_Mode, itemList, currentItem, codeManager, iconTable, htmlSkin, itemsInTitle, translator, Tracer);
                    break;

                case My_Sobek_Type_Enum.Group_AutoFill_Volumes:
                    mySobekViewer = new Group_AutoFill_Volume_MySobekViewer(user);
                    break;

                case My_Sobek_Type_Enum.Group_Mass_Update_Items:
                    mySobekViewer = new Mass_Update_Items_MySobekViewer(user, Current_Mode, currentItem, codeManager, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_Home:
                    mySobekViewer = new Admin_Home_MySobekViewer(user, Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_Builder_Status:
                    mySobekViewer = new Builder_Admin_MySobekViewer(user, Current_Mode);
                    break;

                case My_Sobek_Type_Enum.Admin_Interfaces:
                    mySobekViewer = new Skins_Admin_MySobekViewer(user, Current_Mode, Web_Skin_Collection, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_Forwarding:
                    mySobekViewer = new Aliases_Admin_MySobekViewer(user, Current_Mode, Aggregation_Aliases, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_Wordmarks:
                    mySobekViewer = new Wordmarks_Admin_MySobekViewer(user, Current_Mode, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_URL_Portals:
                    mySobekViewer = new Portals_Admin_MySobekViewer(user, Current_Mode, URL_Portals, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_Users:
                    mySobekViewer = new Users_Admin_MySobekViewer(user, Current_Mode, codeManager, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_User_Groups:
                    mySobekViewer = new User_Group_Admin_MySobekViewer(user, Current_Mode, codeManager, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_Aggregations_Mgmt:
                    mySobekViewer = new Aggregations_Mgmt_Admin_MySobekViewer(user, Current_Mode, codeManager, Tracer);
                    break;

                case My_Sobek_Type_Enum.User_Tags:
                    mySobekViewer = new User_Tags_MySobekViewer(user, Tracer);
                    break;

                case My_Sobek_Type_Enum.User_Usage_Stats:
                    mySobekViewer = new User_Usage_Stats_MySobekViewer(user, Current_Mode, statsDates, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_IP_Restrictions:
                    mySobekViewer = new IP_Restrictions_Admin_MySobekViewer(user, Current_Mode, ipRestrictions, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_Thematic_Headings:
                    mySobekViewer = new Thematic_Headings_Admin_MySobekViewer(user, Current_Mode, Thematic_Headings, Tracer);
                    break;

                case My_Sobek_Type_Enum.Admin_Settings:
                    mySobekViewer = new Settings_Admin_MySobekViewer(user, Current_Mode, Tracer);
                    break;


                case My_Sobek_Type_Enum.Admin_Projects:
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
                                mySobekViewer = new Edit_Item_Metadata_MySobekViewer(user, Current_Mode, itemList, projectObject, codeManager, iconTable, htmlSkin, Tracer);
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

                                    mySobekViewer = new Edit_Item_Metadata_MySobekViewer(user, Current_Mode, itemList, pmets_item, codeManager, iconTable,  htmlSkin, Tracer);
                                }
                            }
                        }
                    }
                    
                    if ( mySobekViewer == null )
                        mySobekViewer = new Projects_Admin_MySobekViewer(user, Current_Mode, Tracer);
                    break;
            }

            // Pass in the navigation and translator information
            mySobekViewer.CurrentMode = Current_Mode;
            mySobekViewer.Translator = translator;

        }

        #endregion

        /// <summary> Flag indicates if a banner should be included </summary>
        /// <remarks> This always returns the value FALSE, since the banner is handled seperately in thie html subwriter </remarks>
        public override bool Include_Banner
        {
            get
            {
                // When editing the aggregation details, the banner should be included here
                if (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Admin_Aggregation_Single)
                    return true;
                return false;
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

        /// <summary> Adds additional HTML needed just before the main place holder but after the other place holders.  </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Add_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("MySobek_HtmlSubwriter.Add_Additional_HTML", "Adding any form elements popup divs");
            if (( currentMode.Logon_Required ) || ( mySobekViewer.Contains_Popup_Forms ))
            {
                mySobekViewer.Add_Popup_HTML(Output, Tracer);
            }

            // Also, add any additional stuff here
            mySobekViewer.Add_HTML_In_Main_Form(Output, Tracer);
        }

        /// <summary> Writes the HTML generated by this my sobek html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_HTML", "Rendering HTML");

            if ((HttpContext.Current.Session["agreement_date"] == null) && (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item ) && ((currentMode.My_Sobek_SubMode.Length == 0) || (currentMode.My_Sobek_SubMode[0] != '1')))
            {
                currentMode.My_Sobek_SubMode = "1";
            }


            if ((!mySobekViewer.Contains_Popup_Forms) && (!currentMode.Logon_Required))
            {
                bool template_banner_override = false;
                if ((user != null) && (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item))
                {
                    if (((New_Group_And_Item_MySobekViewer)mySobekViewer).Current_Template_Banner.Length > 0)
                    {
                        string template_banner = ((New_Group_And_Item_MySobekViewer)mySobekViewer).Current_Template_Banner;
                        template_banner_override = true;
                        Output.WriteLine("<img id=\"mainBanner\" src=\"" + template_banner + "\" alt=\"MISSING BANNER\" />");
                    }
                }
                if ((!template_banner_override) && ( currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Group_Add_Volume ) && ( currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Admin_Aggregation_Single ))
                    Add_Banner(Output);

                // A few cases skip the view selectors at the top entirely
                if ( mySobekViewer.Standard_Navigation_Type != MySobek_Included_Navigation_Enum.NONE )
                {
                    // Write the general view type selector stuff
                    Write_General_View_Type_Selectors(Output);

                    // Write the admin tabs at the bottom?
                    if ( mySobekViewer.Standard_Navigation_Type == MySobek_Included_Navigation_Enum.System_Admin )
                    {
                        // Write the bottom tabs
                        Write_Admin_View_Type_Selectors(Output);
                    }
                }
            }

            // Add the text here
            mySobekViewer.Write_HTML(Output, Tracer);
            return false;
        }

        /// <summary> Adds any necessary controls to one of two place holders on the main ASPX page </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="uploadFilesPlaceHolder"> Alternate place holder ( &quot;myUfdcUploadPlaceHolder&quot; )in the fileUploadForm on the main ASPX page</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Controls(PlaceHolder placeHolder, PlaceHolder uploadFilesPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("MySobek_HtmlSubwriter.Add_Controls", "Build my sobek viewer and add controls");

            // Add the banner now
            if ((currentMode.Logon_Required) || ( mySobekViewer.Contains_Popup_Forms ))
            {
                // Start to build the result to write, with the banner
                StringBuilder header_builder = new StringBuilder();
                StringWriter header_writer = new StringWriter(header_builder);
                Add_Banner( header_writer);

                // NEED TO ADD THE REGULAR BANNER HERE FOR My FOLDER STUFF
                if (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Folder_Management)
                {
                    Write_General_View_Type_Selectors(header_writer);
                }

                // Now, add this literal
                LiteralControl header_literal = new LiteralControl(header_builder.ToString());
                placeHolder.Controls.Add(header_literal);
            }

            // Add any controls needed
            mySobekViewer.Add_Controls(placeHolder, uploadFilesPlaceHolder, Tracer);
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

            My_Sobek_Type_Enum mySobekType = currentMode.My_Sobek_Type;
            string submode = currentMode.My_Sobek_SubMode;
            currentMode.My_Sobek_SubMode = String.Empty;

            Output.WriteLine("<div class=\"ShowSelectRow\">");
            Output.WriteLine("");

            // Write the aggregations tab
            if (mySobekType == My_Sobek_Type_Enum.Admin_Aggregations_Mgmt)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + aggregations + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Aggregations_Mgmt;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + aggregations + Down_Tab_End + "</a>");
            }

            // Write the interfaces tab
            if (mySobekType == My_Sobek_Type_Enum.Admin_Interfaces)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + interfaces + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Interfaces;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + interfaces + Down_Tab_End + "</a>");
            }

            // Write the wordmarks / icon tab
            if (mySobekType == My_Sobek_Type_Enum.Admin_Wordmarks)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + wordmarks + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Wordmarks;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + wordmarks + Down_Tab_End + "</a>");
            }

            // Write the forwarding tab
            if (mySobekType == My_Sobek_Type_Enum.Admin_Forwarding)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + forwarding + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Forwarding;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + forwarding + Down_Tab_End + "</a>");
            }

            // Write the projects tab
            if (mySobekType == My_Sobek_Type_Enum.Admin_Projects)
            {
                Output.WriteLine("  " + Down_Selected_Tab_Start + projects + Down_Selected_Tab_End);
            }
            else
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Projects;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + projects + Down_Tab_End + "</a>");
            }

            if (!user.Is_System_Admin)
            {
                // Write the thematic headings tab
                if (mySobekType == My_Sobek_Type_Enum.Admin_Thematic_Headings)
                {
                    Output.WriteLine("  " + Down_Selected_Tab_Start + thematicHeadings + Down_Selected_Tab_End);
                }
                else
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Thematic_Headings;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + thematicHeadings + Down_Tab_End + "</a>");
                }
            }
            else
            {
                // Write the users tab
                if ((mySobekType == My_Sobek_Type_Enum.Admin_Users) || (mySobekType == My_Sobek_Type_Enum.Admin_User_Groups))
                {
                    if (submode.Length > 0)
                    {
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_User_Groups;
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
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Users;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + users + Down_Tab_End + "</a>");
                }

                // Write the restrictions tab
                if (mySobekType == My_Sobek_Type_Enum.Admin_IP_Restrictions)
                {
                    Output.WriteLine("  " + Down_Selected_Tab_Start + restrictions + Down_Selected_Tab_End);
                }
                else
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_IP_Restrictions;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + restrictions + Down_Tab_End + "</a>");
                }

                // Write the url portals tab
                if (mySobekType == My_Sobek_Type_Enum.Admin_URL_Portals)
                {
                    Output.WriteLine("  " + Down_Selected_Tab_Start + portals + Down_Selected_Tab_End);
                }
                else
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_URL_Portals;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + portals + Down_Tab_End + "</a>");
                }

                // Write the builder tab
                if (mySobekType == My_Sobek_Type_Enum.Admin_Builder_Status)
                {
                    Output.WriteLine("  " + Down_Selected_Tab_Start + builder + Down_Selected_Tab_End);
                }
                else
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Builder_Status;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Down_Tab_Start + builder + Down_Tab_End + "</a>");
                }
            }
            
            currentMode.My_Sobek_Type = mySobekType;
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
            if ((user != null ) && (user.Is_Portal_Admin) && (!user.Is_System_Admin))
                sobek_admin = "PORTAL ADMIN";

            My_Sobek_Type_Enum mySobekType = currentMode.My_Sobek_Type;
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
                if (mySobekType == My_Sobek_Type_Enum.Home)
                {
                    Output.WriteLine("  " + Selected_Tab_Start + my_sobek_home + Selected_Tab_End);
                }
                else
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + my_sobek_home + Unselected_Tab_End + "</a>");
                }

                // Write the folders tab
                if (mySobekType == My_Sobek_Type_Enum.Folder_Management)
                {
                    Output.WriteLine("  " + Selected_Tab_Start + myLibrary + Selected_Tab_End);
                }
                else
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + myLibrary + Unselected_Tab_End + "</a>");
                }

                // Write the preferences tab
                if (mySobekType == My_Sobek_Type_Enum.Preferences)
                {
                    Output.WriteLine("  " + Selected_Tab_Start + myPreferences + Selected_Tab_End);
                }
                else
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + myPreferences + Unselected_Tab_End + "</a>");
                }

                // If this user is internal, add that
                if ( user.Is_Internal_User )
                {
                    currentMode.Mode = Display_Mode_Enum.Internal;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + internalTab + Unselected_Tab_End + "</a>");
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                }

                // Write the sobek admin tab
                if ((user.Is_System_Admin) || ( user.Is_Portal_Admin ))
                {
                    if (mySobekType == My_Sobek_Type_Enum.Admin_Home)
                    {
                        Output.WriteLine("  " + Selected_Tab_Start + sobek_admin + Selected_Tab_End);
                    }
                    else
                    {
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Admin_Home;
                        if ((mySobekType == My_Sobek_Type_Enum.Admin_Aggregations_Mgmt) || (mySobekType == My_Sobek_Type_Enum.Admin_Forwarding) || (mySobekType == My_Sobek_Type_Enum.Admin_Interfaces) || (mySobekType == My_Sobek_Type_Enum.Admin_Projects) || (mySobekType == My_Sobek_Type_Enum.Admin_Users) || (mySobekType == My_Sobek_Type_Enum.Admin_Wordmarks) || (mySobekType == My_Sobek_Type_Enum.Admin_IP_Restrictions) || (mySobekType == My_Sobek_Type_Enum.Admin_URL_Portals) || (mySobekType == My_Sobek_Type_Enum.Admin_Settings) || (mySobekType == My_Sobek_Type_Enum.Admin_Thematic_Headings) || (mySobekType == My_Sobek_Type_Enum.Admin_Builder_Status))
                            Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Selected_Tab_Start + sobek_admin + Selected_Tab_End + "</a>");
                        else
                            Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + sobek_admin + Unselected_Tab_End + "</a>");
                    }
                }
            }

            currentMode.My_Sobek_Type = mySobekType;
            currentMode.My_Sobek_SubMode = submode;

            Output.WriteLine("");
            Output.WriteLine("</div>");
            Output.WriteLine();

            if ((currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Folder_Management) || (currentMode.My_Sobek_SubMode != "submitted items"))
            {
                Output.WriteLine("<div class=\"SobekSearchPanel\">");
                if (mySobekViewer != null)
                    Output.WriteLine("  <h1>" + mySobekViewer.Web_Title + "</h1>");
                else if (user != null) Output.WriteLine("  <h1>Welcome back, " + user.Nickname + "</h1>");
                Output.WriteLine("</div>");
                Output.WriteLine();
            }
        }

        #endregion
    }
}
