#region Using directives

using System.IO;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated user to change the serial hierarchy for all the items within a single title </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the serial hierarchy for editing</li>
    /// </ul></remarks>
    public class Edit_Serial_Hierarchy_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the Edit_Serial_Hierarchy_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Edit_Serial_Hierarchy_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Everything done in base class constructor
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the value 'Edit Serial Hierarchy' </value>
        public override string Web_Title
        {
            get { return "Edit Serial Hierarchy"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Serial_Hierarchy_MySobekViewer.Write_HTML", "Do nothing");

            Output.WriteLine("<br /><br />");
            Output.WriteLine("<strong>EDIT SERIAL HIERARCHY</strong><br /><br />");
            Output.WriteLine("Implementation for this feature is currently pending.<br /><br /><br />");
            
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the individual metadata elements are added as controls, not HTML </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_Serial_Hierarchy_MySobekViewer.Write_ItemNavForm_Closing", "Do nothing");
        }
    }
}
