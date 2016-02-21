using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Core.WebContent;
using SobekCM.Tools;

namespace SobekCM.Library.WebContentViewer.Viewers
{
    /// <summary> Abstract class which all specialized web content viewers must extend </summary>
    /// <remarks> This implements the <see cref="iWebContentViewer" /> interface. </remarks>
    public abstract class abstractWebContentViewer : iWebContentViewer
    {
        /// <summary> Protected field contains the information specific to the current request </summary>
        protected RequestCache RequestSpecificValues;

        /// <summary> Protcted field contains the static web page information for the current request </summary>
        protected HTML_Based_Content StaticPage;

        /// <summary> Gets the type of specialized web content viewer </summary>
        public abstract WebContent_Type_Enum Type { get; }

        /// <summary> Constructor for objects which implement this abstract class  </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="StaticPage"> Static page info for this request </param>
        protected abstractWebContentViewer(RequestCache RequestSpecificValues, HTML_Based_Content StaticPage )
        {
            this.RequestSpecificValues = RequestSpecificValues;
            this.StaticPage = StaticPage;
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <remarks> This returns NULL by default, but can be override by individual viewer implementations </remarks>
        public virtual string Viewer_Title
        {
            get { return null; }
        }

        /// <summary> Gets the URL for the icon related to this web content viewer task </summary>
        /// <remarks> This returns NULL by default, but can be override by individual viewer implementations </remarks>
        public virtual string Viewer_Icon
        {
            get { return null; }
        }

        /// <summary> Add the HTML to be displayed </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> No html is added here, although children classes should override this virtual method to add HTML </remarks>
        public virtual void Add_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstractWebContentViewer.Add_HTML", "No html added");
            }

            // No html to be added here
        }
    }
}
