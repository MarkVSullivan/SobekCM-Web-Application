#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.Items;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MySobekViewer;
using SobekCM.Library.Settings;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> My Sobek html subwriter is used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. <br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create this necessary subwriter since this action requires authentication. </li>
    /// <li>This class will create a mySobek subwriter (extending <see cref="MySobekViewer.abstract_MySobekViewer"/> ) for the specified task.The mySobek subwriter creates an instance of this viewer to view and edit existing item aggregationPermissions in this digital library</li>
    /// </ul></remarks>
    public class MySobek_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly iMySobek_Admin_Viewer mySobekViewer;
 
        #region Constructor, which also creates the applicable MySobekViewer object

	    /// <summary> Constructor for a new instance of the MySobek_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public MySobek_HtmlSubwriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues) 
        {

            RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Saving values and geting user object back from the session");



            if (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Log_Out)
            {
                RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Performing logout");

                HttpContext.Current.Session["user"] = null;
                HttpContext.Current.Response.Redirect("?", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                RequestSpecificValues.Current_Mode.Request_Completed = true;
                return;
            }

            if ((RequestSpecificValues.Current_Mode.My_Sobek_Type != My_Sobek_Type_Enum.Logon) && (RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.Is_Temporary_Password))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.New_Password;
            }

            if (RequestSpecificValues.Current_Mode.Logon_Required)
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;

            RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Building the my sobek viewer object");

            // Get the appropriate mysobek viewer from the factory
            mySobekViewer = MySobekViewer_Factory.Get_MySobekViewer(RequestSpecificValues);
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
                // If no user, always return false (should not really get here)
                if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
                    return false;

                // If no admin viewer was found, also return false
                if (mySobekViewer == null)
                    return false;

                // Return the value from the admin viewer
                return mySobekViewer.Upload_File_Possible;
			}
		}


		/// <summary> Write any additional values within the HTML Head of the
		/// final served page </summary>
		/// <param name="Output"> Output stream currently within the HTML head tags </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
		{
            // Admin viewers can override all of this
            StringBuilder builder = new StringBuilder();
            if (mySobekViewer != null)
            {
                using (StringWriter writer = new StringWriter(builder))
                {
                    bool overrideHead = mySobekViewer.Write_Within_HTML_Head(writer, Tracer);
                    if (overrideHead)
                    {
                        Output.WriteLine(builder.ToString());
                        return;
                    }
                }
            }


            Output.WriteLine("  <meta name=\"robots\" content=\"index, nofollow\" />");

            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Admin_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");

            // If there was a viewer, add based on behaviors and flags
            if (mySobekViewer != null)
            {
                // Add the uploader libraries if editing an item
                if (mySobekViewer.Upload_File_Possible)
                {
                    Output.WriteLine("  <script src=\"" + Static_Resources.Jquery_Uploadifive_Js + "\" type=\"text/javascript\"></script>");
                    Output.WriteLine("  <script src=\"" + Static_Resources.Jquery_Uploadify_Js + "\" type=\"text/javascript\"></script>");

                    Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Static_Resources.Uploadifive_Css + "\">");
                    Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Static_Resources.Uploadify_Css + "\">");
                }

                if (mySobekViewer.Viewer_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter))
                {
                    Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Item_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
                }

                if ((mySobekViewer.Viewer_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Use_Jquery_DataTables)))
                {
                    Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Datatables_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
                    Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Datatables_Js + "\" ></script>");
                }

                // Allow the admin viewer to also write into the header
                if (builder.Length > 0)
                {
                    Output.WriteLine(builder.ToString());
                }
            }
		}


        /// <summary> Writes the HTML generated by this my sobek html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_HTML", "Rendering HTML");

            if ((HttpContext.Current.Session["agreement_date"] == null) && (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item ) && ((RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length == 0) || (RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0] != '1')))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
            }
                // A few cases skip the view selectors at the top entirely
	        if (mySobekViewer.Standard_Navigation_Type == MySobek_Admin_Included_Navigation_Enum.Standard)
	        {
		        // Add the user-specific main menu
		        MainMenus_Helper_HtmlSubWriter.Add_UserSpecific_Main_Menu(Output, RequestSpecificValues);

		        // Start the page container
		        Output.WriteLine("<div id=\"pagecontainer\">");
		        Output.WriteLine("<br />");
	        }
            else if (mySobekViewer.Standard_Navigation_Type == MySobek_Admin_Included_Navigation_Enum.LogOn)
			{
				// Add the item views
				Output.WriteLine("<!-- Add the main user-specific menu -->");
				Output.WriteLine("<div id=\"sbkUsm_MenuBar\" class=\"sbkMenu_Bar\">");
				Output.WriteLine("<ul class=\"sf-menu\">");

				// Get ready to draw the tabs
				string sobek_home_text = RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Home";

				// Add the 'SOBEK HOME' first menu option and suboptions
				RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
				RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
				RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;
                Output.WriteLine("\t\t<li id=\"sbkUsm_Home\" class=\"sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a></li>");
				Output.WriteLine("\t</ul></div>");

				Output.WriteLine("<!-- Initialize the main user menu -->");
				Output.WriteLine("<script>");
				Output.WriteLine("  jQuery(document).ready(function () {");
				Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
				Output.WriteLine("  });");
				Output.WriteLine("</script>");
				Output.WriteLine();

				// Restore the current view information type
				RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;

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
			if ((RequestSpecificValues.Current_Mode.Logon_Required) || (mySobekViewer.Contains_Popup_Forms))
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
                if (mySobekViewer != null)
                {
                    string cssStyle = mySobekViewer.Container_CssClass;
                    if (!String.IsNullOrEmpty(cssStyle))
                        return cssStyle;
                }

                return base.Container_CssClass;
            }
		}
    }
}
