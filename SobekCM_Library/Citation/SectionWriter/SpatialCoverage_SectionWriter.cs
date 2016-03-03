using System;
using System.IO;
using SobekCM.Core.BriefItem;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Tools;

namespace SobekCM.Library.Citation.SectionWriter
{
    /// <summary> Special citation section writer adds the hierarchical spatial
    /// coverage to the citation allowing each individual portion of the hierarchy
    /// to be selected for searching purposes </summary> 
    /// <remarks> This class implements the <see cref="iCitationSectionWriter"/> interface. </remarks>
    public class SpatialCoverage_SectionWriter : iCitationSectionWriter
    {
        /// <summary> Returns flag that indicates this citation section writer 
        /// will be writing alues to the output stream </summary>
        /// <param name="Item"> Digital resource to analyze for data to write </param>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        public bool Has_Data_To_Write(CitationElement ElementInfo, BriefItemInfo Item)
        {
            throw new NotImplementedException();
        }

        /// <summary> Wites a section of citation from a provided digital resource </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Item"> Digital resource with all the data to write </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Citation_Section(CitationElement ElementInfo, TextWriter Output, BriefItemInfo Item, Custom_Tracer Tracer)
        {
            throw new NotImplementedException();
        }
    }
}
