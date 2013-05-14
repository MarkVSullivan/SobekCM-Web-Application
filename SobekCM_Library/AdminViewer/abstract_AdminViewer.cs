#region Using directives

using System;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    public abstract class abstract_AdminViewer : iMySobek_Admin_Viewer
    {
        private const string SELECTED_TAB_START_ORIG = "<img src=\"{0}design/skins/{1}/tabs/cL_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\"> ";
        private const string SELECTED_TAB_END_ORIG = " </span><img src=\"{0}design/skins/{1}/tabs/cR_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";
        private const string UNSELECTED_TAB_START_ORIG = "<img src=\"{0}design/skins/{1}/tabs/cL.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\"> ";
        private const string UNSELECTED_TAB_END_ORIG = " </span><img src=\"{0}design/skins/{1}/tabs/cR.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";
        private const string DOWN_TAB_START_ORIG = "<img src=\"{0}design/skins/{1}/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\">";
        private const string DOWN_TAB_END_ORIG = "</span><img src=\"{0}design/skins/{1}/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";
        private const string DOWN_SELECTED_TAB_START_ORIG = "<img src=\"{0}design/skins/{1}/tabs/cLD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\">";
        private const string DOWN_SELECTED_TAB_END_ORIG = "</span><img src=\"{0}design/skins/{1}/tabs/cRD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";

        /// <summary> Protected field contains the skin-specific code for the END of a DOWNWARD-facing SELECTED tab </summary>
        protected string Down_Selected_Tab_End;

        /// <summary> Protected field contains the skin-specific code for the BEGINNING of a DOWNWARD-facing SELECTED tab </summary>
        protected string Down_Selected_Tab_Start;

        /// <summary> Protected field contains the skin-specific code for the END of a DOWNWARD-facing UNSELECTED tab </summary>
        protected string Down_Tab_End;

        /// <summary> Protected field contains the skin-specific code for the BEGINNING of a DOWNWARD-facing UNSELECTED tab </summary>
        protected string Down_Tab_Start;

        /// <summary> Protected field contains the skin-specific code for the END of a UPWARD-facing SELECTED tab </summary>
        protected string Selected_Tab_End;

        /// <summary> Protected field contains the skin-specific code for the BEGINNING of a UPWARD-facing SELECTED tab </summary>
        protected string Selected_Tab_Start;

        /// <summary> Protected field contains the skin-specific code for the END of a UPWARD-facing UNSELECTED tab </summary>
        protected string Unselected_Tab_End;

        /// <summary> Protected field contains the skin-specific code for the BEGINNING of a UPWARD-facing UNSELECTED tab </summary>
        protected string Unselected_Tab_Start;

        /// <summary> Protected field contains the mode / navigation information for the current request </summary>
        protected SobekCM_Navigation_Object currentMode;

        /// <summary> Protected field contains the authenticated user information </summary>
        protected User_Object user;

        /// <summary> Constructor for a new instance of the abstract_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        protected abstract_AdminViewer(User_Object User)
        {
            user = User;
        }
        

        /// <summary> Sets the mode / navigation information for the current request </summary>
        /// <remarks> This also sets all of the protected tab HTML fields, from the base interface in the navigation object </remarks>
        public SobekCM_Navigation_Object CurrentMode
        {
            set
            {
                currentMode = value;

                Selected_Tab_Start = String.Format(SELECTED_TAB_START_ORIG, currentMode.Base_URL, currentMode.Base_Skin);
                Selected_Tab_End = String.Format(SELECTED_TAB_END_ORIG, currentMode.Base_URL, currentMode.Base_Skin);
                Unselected_Tab_Start = String.Format(UNSELECTED_TAB_START_ORIG, currentMode.Base_URL, currentMode.Base_Skin);
                Unselected_Tab_End = String.Format(UNSELECTED_TAB_END_ORIG, currentMode.Base_URL, currentMode.Base_Skin);
                Down_Tab_Start = String.Format(DOWN_TAB_START_ORIG, currentMode.Base_URL, currentMode.Base_Skin);
                Down_Tab_End = String.Format(DOWN_TAB_END_ORIG, currentMode.Base_URL, currentMode.Base_Skin);
                Down_Selected_Tab_Start = String.Format(DOWN_SELECTED_TAB_START_ORIG, currentMode.Base_URL, currentMode.Base_Skin);
                Down_Selected_Tab_End = String.Format(DOWN_SELECTED_TAB_END_ORIG, currentMode.Base_URL, currentMode.Base_Skin);

            }
        }

        /// <summary> Sets the translation / language support object for writing the user interface in multiple languages </summary>
        public Language_Support_Info Translator { get; set; }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <remarks> Abstract property must be implemented by all extending classes </remarks>
        public abstract string Web_Title { get; }
   
        /// <summary> Property indicates if this mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        ///<value> This defaults to FALSE but is overwritten by the mySobek viewers which use pop-up forms </value>
        public virtual bool Contains_Popup_Forms
        {
            get { return false; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of any form) </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Abstract method must be implemented by all extending classes </remarks>
        public abstract void Write_HTML(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No html is added here, although some children class override this virtual method to add pop-up form HTML </remarks>
        public virtual void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstract_MySobekViewer.Add_Popup_HTML", "No html added");
            }

            // No html to be added here
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public virtual void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstract_MySobekViewer.Add_HTML_In_Main_Form", "No HTML Added");
            }

            // No controls to be added here
        }

        /// <summary> Add controls directly to the form, either to the main control area or to the file upload placeholder </summary>
        /// <param name="placeHolder"> Main place holder to which all main controls are added </param>
        /// <param name="uploadFilesPlaceHolder"> Place holder is used for uploading file </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No controls are added here, although some children class override this virtual method to add controls </remarks>
        public virtual void Add_Controls(PlaceHolder placeHolder, PlaceHolder uploadFilesPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstract_MySobekViewer.Add_Controls", "No controls added");
            }

            // No controls to be added here
        }
    }
}
