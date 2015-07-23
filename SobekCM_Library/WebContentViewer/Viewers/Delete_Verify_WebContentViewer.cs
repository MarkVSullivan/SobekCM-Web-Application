using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Tools;

namespace SobekCM.Library.WebContentViewer.Viewers
{
    /// <summary> Web content viewer requests verification to delete a web content page or redirect before it is deleted </summary>
    /// <remarks> This viewer extends the <see cref="abstractWebContentViewer" /> abstract class and implements the <see cref="iWebContentViewer"/> interface. </remarks>
    public class Delete_Verify_WebContentViewer : abstractWebContentViewer
    {
        /// <summary>  Constructor for a new instance of the Delete_Verify_WebContentViewer class  </summary>
        /// <param name="RequestSpecificValues">  All the necessary, non-global data specific to the current request  </param>
        public Delete_Verify_WebContentViewer(RequestCache RequestSpecificValues)
        {
            
        }


        /// <summary> Gets the type of specialized web content viewer </summary>
        public override WebContent_Type_Enum Type { get { return WebContent_Type_Enum.Delete_Verify; }}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Verify Web Content Deletion"; }
        }

        /// <summary> Gets the URL for the icon related to this web content viewer task </summary>
        public override string Viewer_Icon
        {
            get { return null; }
        }

        /// <summary> Add the HTML to be displayed </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Delete_Verify_WebContentViewer.Add_HTML", "No html added");
            }

            // No html to be added here
        }
    }
}
