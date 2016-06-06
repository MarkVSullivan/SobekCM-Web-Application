using System.IO;
using System.Text;
using SobekCM.Core.BriefItem;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Tools;

namespace SobekCM.Library.Citation.SectionWriter
{
    /// <summary> Interface that all citation section writers must implement </summary>
    public interface iCitationSectionWriter
    {
        /// <summary> Returns flag that indicates this citation section writer 
        /// will be writing alues to the output stream </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Item"> Digital resource to analyze for data to write </param>
        bool Has_Data_To_Write(CitationElement ElementInfo, BriefItemInfo Item);

        /// <summary> Wites a section of citation from a provided digital resource </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Item"> Digital resource with all the data to write </param>
        /// <param name="LeftColumnWidth"> Number of pixels of the left column, or the definition terms </param>
        /// <param name="SearchLink"> Beginning of the search link that can be used to allow the web patron to select a term and run a search against this instance </param>
        /// <param name="SearchLinkEnd"> End of the search link that can be used to allow the web patron to select a term and run a search against this instance  </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>

        void Write_Citation_Section(CitationElement ElementInfo, StringBuilder Output, BriefItemInfo Item, int LeftColumnWidth, string SearchLink, string SearchLinkEnd, Custom_Tracer Tracer);
    }
}
