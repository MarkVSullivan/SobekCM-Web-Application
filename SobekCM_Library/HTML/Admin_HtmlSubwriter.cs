#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.Database;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MySobekViewer;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.HTML
{
	/// <summary> Adminl subwriter is used for administrative tasks, either collection admin, portal admin, or system admin </summary>
	/// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. <br /><br />
	/// During a valid html request, the following steps occur:
	/// <ul>
	/// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
	/// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
	/// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
	/// <li>The HTML writer will create this necessary subwriter since this action requires administrative rights. </li>
	/// <li>This class will create a admin subwriter (extending <see cref="AdminViewer.abstract_AdminViewer"/> ) for the specified task.The admin subwriter creates an instance of this viewer to view and edit existing item aggregationPermissions in this digital library</li>
	/// </ul></remarks>
    public class Admin_HtmlSubwriter: abstractHtmlSubwriter
    {
        private readonly iMySobek_Admin_Viewer adminViewer;

 
        #region Constructor, which also creates the applicable MySobekViewer object

        /// <summary> Constructor for a new instance of the Admin_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Admin_HtmlSubwriter( RequestCache RequestSpecificValues ) : base ( RequestSpecificValues )
        {

            RequestSpecificValues.Tracer.Add_Trace("Admin_HtmlSubwriter.Constructor", "Saving values and geting RequestSpecificValues.Current_User object back from the session");

            // All Admin pages require a RequestSpecificValues.Current_User being logged on
            if (RequestSpecificValues.Current_User == null)
			{
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
				return;
			}

            // If the RequestSpecificValues.Current_User is not an admin, and admin was selected, reroute this
            if ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin) && (RequestSpecificValues.Current_Mode.Admin_Type != Admin_Type_Enum.Aggregation_Single))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            RequestSpecificValues.Tracer.Add_Trace("Admin_HtmlSubwriter.Constructor", "Building the my sobek viewer object");
            switch (RequestSpecificValues.Current_Mode.Admin_Type)
            {
                case Admin_Type_Enum.Aggregation_Single:
                    adminViewer = new Aggregation_Single_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Home:
                    adminViewer = new Home_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Builder_Status:
                    adminViewer = new Builder_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Skins:
                    adminViewer = new Skins_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Aliases:
                    adminViewer = new Aliases_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Wordmarks:
                    adminViewer = new Wordmarks_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.URL_Portals:
                    adminViewer = new Portals_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Users:
                    adminViewer = new Users_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.User_Groups:
                    adminViewer = new User_Group_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Aggregations_Mgmt:
                    adminViewer = new Aggregations_Mgmt_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.IP_Restrictions:
                    adminViewer = new IP_Restrictions_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Thematic_Headings:
                    adminViewer = new Thematic_Headings_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Settings:
                    adminViewer = new Settings_AdminViewer(RequestSpecificValues);
                    break;

                case Admin_Type_Enum.Default_Metadata:
                    if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length > 1)
                    {
                        string project_code = RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Substring(1);
                        RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Checking cache for valid project file");
                        if (RequestSpecificValues.Current_User != null)
                        {
                            SobekCM_Item projectObject = CachedDataManager.Retrieve_Project(RequestSpecificValues.Current_User.UserID, project_code, RequestSpecificValues.Tracer);
                            if (projectObject != null)
                            {
                                RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Valid default metadata set found in cache");
                                adminViewer = new Edit_Item_Metadata_MySobekViewer( projectObject, RequestSpecificValues);
                            }
                            else
                            {
                                if (Engine_Database.Get_All_Template_DefaultMetadatas(RequestSpecificValues.Tracer).Tables[0].Select("MetadataCode='" + project_code + "'").Length > 0)
                                {
                                    RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Building default metadata set from (possible) PMETS");
                                    string pmets_file = UI_ApplicationCache_Gateway.Settings.Base_MySobek_Directory + "projects\\" + RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Substring(1) + ".pmets";
                                    SobekCM_Item pmets_item = File.Exists(pmets_file) ? SobekCM_Item.Read_METS(pmets_file) : new SobekCM_Item();
                                    pmets_item.Bib_Info.Main_Title.Title = "Default metadata set for '" + project_code + "'";
                                    pmets_item.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Project;
                                    pmets_item.BibID = project_code.ToUpper();
                                    pmets_item.VID = "00001";
                                    pmets_item.Source_Directory = UI_ApplicationCache_Gateway.Settings.Base_MySobek_Directory +  "projects\\";

                                    RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Adding project file to cache");

                                    CachedDataManager.Store_Project(RequestSpecificValues.Current_User.UserID, project_code, pmets_item, RequestSpecificValues.Tracer);

                                    adminViewer = new Edit_Item_Metadata_MySobekViewer(pmets_item, RequestSpecificValues);
                                }
                            }
                        }
                    }


                    if (adminViewer == null)
                        adminViewer = new Default_Metadata_AdminViewer(RequestSpecificValues);
                    break;
            }

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

                returnVal.AddRange(adminViewer.Viewer_Behaviors);

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
				if (RequestSpecificValues.Current_User == null)
					return false;

				if (( RequestSpecificValues.Current_Mode.Admin_Type == Admin_Type_Enum.Wordmarks ) || ( RequestSpecificValues.Current_Mode.Admin_Type == Admin_Type_Enum.Aggregation_Single ))
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
            if ((RequestSpecificValues.Current_Mode.Logon_Required) || (adminViewer.Contains_Popup_Forms))
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

			// Add any intro html text here
			adminViewer.Write_ItemNavForm_Opening(Output, Tracer);
		}

        /// <summary> Writes the HTML generated by this my sobek html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_HtmlSubwriter.Write_HTML", "Rendering HTML");

	       // if (CurrentMode.Admin_Type == Admin_Type_Enum.Wordmarks)
		   //     return false;

            if ((!adminViewer.Contains_Popup_Forms) && (!RequestSpecificValues.Current_Mode.Logon_Required))
            {
                if ( RequestSpecificValues.Current_Mode.Admin_Type != Admin_Type_Enum.Aggregation_Single )
                {
                    // Add the banner
                    Add_Banner(Output, "sbkAhs_BannerDiv", RequestSpecificValues.Current_Mode, RequestSpecificValues.HTML_Skin, RequestSpecificValues.Hierarchy_Object);

                    // Add the RequestSpecificValues.Current_User-specific main menu
                    MainMenus_Helper_HtmlSubWriter.Add_UserSpecific_Main_Menu(Output, RequestSpecificValues );

                    // Start the page container
                    Output.WriteLine("<div id=\"pagecontainer\">");
                    Output.WriteLine("<br />");

                    // Add the box with the title
                    if ((RequestSpecificValues.Current_Mode.My_Sobek_Type != My_Sobek_Type_Enum.Folder_Management) || (RequestSpecificValues.Current_Mode.My_Sobek_SubMode != "submitted items"))
                    {
                        Output.WriteLine("<div class=\"SobekSearchPanel\">");
                        if (adminViewer != null)
                            Output.WriteLine("  <h1>" + adminViewer.Web_Title + "</h1>");
                        else if (RequestSpecificValues.Current_User != null) Output.WriteLine("  <h1>Welcome back, " + RequestSpecificValues.Current_User.Nickname + "</h1>");
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
			adminViewer.Write_ItemNavForm_Closing(Output, Tracer);
		}

		/// <summary> Add controls directly to the form in the main control area placeholder</summary>
		/// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Admin_HtmlSubwriter.Add_Controls", "Build admin viewer and add controls");

            // Add the banner now
            if (((RequestSpecificValues.Current_Mode.Logon_Required) || (adminViewer.Contains_Popup_Forms)) && ( !(adminViewer is Edit_Item_Metadata_MySobekViewer)))
            {
                // Start to build the result to write, with the banner
                StringBuilder header_builder = new StringBuilder();
                StringWriter header_writer = new StringWriter(header_builder);
                Add_Banner(header_writer, "sbkAhs_BannerDiv", RequestSpecificValues.Current_Mode, RequestSpecificValues.HTML_Skin, RequestSpecificValues.Hierarchy_Object);

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

            if (adminViewer is Edit_Item_Metadata_MySobekViewer)
            {
#if DEBUG
                Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_MySobek.css\" rel=\"stylesheet\" type=\"text/css\" />");
#else
			    Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_MySobek.min.css\" rel=\"stylesheet\" type=\"text/css\" />");
#endif
            }
            else
            {
#if DEBUG
                Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_Admin.css\" rel=\"stylesheet\" type=\"text/css\" />");
#else
			    Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_Admin.min.css\" rel=\"stylesheet\" type=\"text/css\" />");
#endif
            }

            // If editing projects, add the mySobek stylesheet as well
            if ((RequestSpecificValues.Current_Mode.Admin_Type == Admin_Type_Enum.Default_Metadata) && (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length > 0))
            {
#if DEBUG
                Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_Metadata.css\" rel=\"stylesheet\" type=\"text/css\" />");
#else
				Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_Metadata.min.css\" rel=\"stylesheet\" type=\"text/css\" />");
#endif
            }

			// Add the uploader libraries if editing an item
	        if ((RequestSpecificValues.Current_Mode.Admin_Type == Admin_Type_Enum.Aggregation_Single) || ( RequestSpecificValues.Current_Mode.Admin_Type == Admin_Type_Enum.Wordmarks ))
	        {
#if DEBUG
                Output.WriteLine("  <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/uploadifive/jquery.uploadifive.js\" type=\"text/javascript\"></script>");
                Output.WriteLine("  <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/uploadify/jquery.uploadify.js\" type=\"text/javascript\"></script>");
#else
		        Output.WriteLine("  <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/uploadifive/jquery.uploadifive.min.js\" type=\"text/javascript\"></script>");
		        Output.WriteLine("  <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/uploadify/jquery.uploadify.min.js\" type=\"text/javascript\"></script>");
#endif

		        Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/uploadifive/uploadifive.css\">");
		        Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/uploadify/uploadify.css\">");
	        }

            if ((adminViewer != null) && (adminViewer.Viewer_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter)))
            {
#if DEBUG
                Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_Item.css\" rel=\"stylesheet\" type=\"text/css\" />");
#else
			Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_Item.min.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
#endif
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

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		public override string Container_CssClass
		{
			get
			{
				switch (RequestSpecificValues.Current_Mode.Admin_Type)
				{
					case Admin_Type_Enum.Wordmarks:
						return "sbkWav_ContainerInner";

					case Admin_Type_Enum.Settings:
						return"sbkSeav_ContainerInner";

					case Admin_Type_Enum.Aggregation_Single:
						return "sbkSaav_ContainerInner";
				}
				return base.Container_CssClass;
			}
		}
    }
}
