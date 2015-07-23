using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Tools;

namespace SobekCM.Library.WebContentViewer.Viewers
{
    /// <summary> Interface which all specialized web content viewers must implement </summary>
    public interface iWebContentViewer
    {
        /// <summary> Gets the type of specialized web content viewer </summary>
        WebContent_Type_Enum Type { get; }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        string Viewer_Title { get; }

        /// <summary> Gets the URL for the icon related to this web content viewer task </summary>
        string Viewer_Icon { get; }

        /// <summary> Add the HTML to be displayed </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        void Add_HTML(TextWriter Output, Custom_Tracer Tracer);
    }
}
