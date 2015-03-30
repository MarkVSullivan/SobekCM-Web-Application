#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Library.HTML;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AdminViewer
{
    public interface iMySobek_Admin_Viewer
    {

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors { get; }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <remarks> Abstract property must be implemented by all extending classes </remarks>
        string Web_Title { get; }

        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        /// <remarks> Abstract property must be implemented by all extending classes </remarks>
        string Viewer_Icon { get; }

        /// <summary> Property indicates if this mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        ///<value> This defaults to FALSE but is overwritten by the mySobek viewers which use pop-up forms </value>
        bool Contains_Popup_Forms { get; }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of any form) </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Abstract method must be implemented by all extending classes </remarks>
        void Write_HTML(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No html is added here, although some children class override this virtual method to add pop-up form HTML </remarks>
        void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer);

		/// <summary> This is an opportunity to write HTML directly into the main form before any controls are placed in the main place holder </summary>
		/// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> This is an opportunity to write HTML directly into the main form after any controls are placed </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No controls are added here, although some children class override this virtual method to add controls </remarks>
        void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer);
    }
}