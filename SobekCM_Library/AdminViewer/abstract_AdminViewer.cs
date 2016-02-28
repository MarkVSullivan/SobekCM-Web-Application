#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Library.HTML;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Abstract base class extended by all admin viewer objects </summary>
    public abstract class abstract_AdminViewer : iMySobek_Admin_Viewer
    {

        /// <summary> Empty list of behaviors, returned by default </summary>
        /// <remarks> This just prevents an empty set from having to be created over and over </remarks>
        protected static List<HtmlSubwriter_Behaviors_Enum> emptybehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

        /// <summary> Protected field contains the information specific to the current request </summary>
        protected RequestCache RequestSpecificValues;

        /// <summary> Constructor for a new instance of the abstract_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        protected abstract_AdminViewer(RequestCache RequestSpecificValues)
        {
            this.RequestSpecificValues = RequestSpecificValues;
        }
        
        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <remarks> Abstract property must be implemented by all extending classes </remarks>
        public abstract string Web_Title { get; }

        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        /// <remarks> Abstract property must be implemented by all extending classes </remarks>
        public abstract string Viewer_Icon { get; }
   
        /// <summary> Property indicates if this mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        ///<value> This defaults to FALSE but is overwritten by the mySobek viewers which use pop-up forms </value>
        public virtual bool Contains_Popup_Forms
        {
            get { return false; }
        }

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public virtual List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get
            {
                if (Contains_Popup_Forms)
                {
                    return new List<HtmlSubwriter_Behaviors_Enum>
                    {
                        HtmlSubwriter_Behaviors_Enum.Suppress_Header, 
                        HtmlSubwriter_Behaviors_Enum.Suppress_Footer
                    };
                }

                return emptybehaviors;
            }
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

		/// <summary> This is an opportunity to write HTML directly into the main form before any controls are placed in the main place holder </summary>
		/// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public virtual void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("abstract_MySobekViewer.Write_ItemNavForm_Opening", "No HTML Added");
			}
		}

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public virtual void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstract_MySobekViewer.Write_ItemNavForm_Closing", "No HTML Added");
            }
        }

		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No controls are added here, although some children class override this virtual method to add controls </remarks>
        public virtual void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstract_MySobekViewer.Add_Controls", "No controls added");
            }

            // No controls to be added here
        }


        /// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden/omitted. </summary>
        /// <value> By default, this returns FALSE.</value>
        /// <remarks> This can be overriden in base classes that extend this abstract class </remarks>
        public virtual bool Upload_File_Possible { get { return false; } }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if this should completely override the default added by the admin or mySobek viewer </returns>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual html subwriters </remarks>
        public virtual bool Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            return false;
        }

        /// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
        /// <value> By default, this returns NULL, which would use the base, or default container </value>
        /// <remarks> This can be overriden in base classes that extend this abstract class </remarks>
        public virtual string Container_CssClass { get { return null; } }

        /// <summary> Navigation type to be displayed (mostly used by the mySobek viewers) </summary>
        public virtual MySobek_Admin_Included_Navigation_Enum Standard_Navigation_Type { get { return MySobek_Admin_Included_Navigation_Enum.Admin; } }

        /// <summary> Flag indicates if a user must be logged in to access this 
        /// admin or mySobek view.  </summary>
        /// <value> This returns TRUE by default, but can be overriden by classes that extend this abstract class </value>
        public virtual bool Requires_Logged_In_User { get { return true; } }
    }
}
