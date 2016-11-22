using System;
using System.IO;
using System.Text;
using System.Web;
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
            // Look for a match in the item description
            BriefItem_DescriptiveTerm firstSpatial = Item.Get_Description("Hierarchical Spatial");
            return ((firstSpatial != null) && (firstSpatial.Values.Count > 0));
        }

        /// <summary> Wites a section of citation from a provided digital resource </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Item"> Digital resource with all the data to write </param>
        /// <param name="LeftColumnWidth"> Number of pixels of the left column, or the definition terms </param>
        /// <param name="SearchLink"> Beginning of the search link that can be used to allow the web patron to select a term and run a search against this instance </param>
        /// <param name="SearchLinkEnd"> End of the search link that can be used to allow the web patron to select a term and run a search against this instance  </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Citation_Section(CitationElement ElementInfo, StringBuilder Output, BriefItemInfo Item, int LeftColumnWidth, string SearchLink, string SearchLinkEnd, Custom_Tracer Tracer)
        {
            string displayLabel = (String.IsNullOrEmpty(ElementInfo.DisplayTerm)) ? "Spatial Coverage" : ElementInfo.DisplayTerm;

            Output.AppendLine("        <dt class=\"sbk_CivSPATIAL_COVERAGE_Element\" style=\"width:" + LeftColumnWidth + "px;\" >" + displayLabel + ": </dt>");
            Output.Append("        <dd class=\"sbk_CivSPATIAL_COVERAGE_Element\" style=\"margin-left:" + LeftColumnWidth + "px;\">");

            int spatial_count = 1;
            while (true)
            {
                // Determine the term to use
                string term = "Hierarchical Spatial";
                if (spatial_count > 1)
                    term = term + " (" + spatial_count + ")";

                // Look for the match
                BriefItem_DescriptiveTerm thisSpatial = Item.Get_Description(term);

                // If no match, break out of this while loop
                if ((thisSpatial == null) || (thisSpatial.Values == null))
                    break;

                // Past the first complex spatial subject?
                if ( spatial_count > 1 )
                    Output.AppendLine("<br />");

                // Step through each subterm
                StringBuilder spatial_builder = new StringBuilder();
                foreach (BriefItem_DescTermValue thisValue in thisSpatial.Values)
                {
                    if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");

                    switch (thisValue.SubTerm)
                    {
                        case "Continent":
                        case "Province":
                        case "Region":
                        case "Territory":
                        case "City Section":
                        case "Island":
                        case "Area":
                            spatial_builder.Append(thisValue.Value);
                            break;

                        case "Country":
                            spatial_builder.Append(SearchLink.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisValue.Value.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CO") + thisValue.Value + SearchLinkEnd);
                            break;

                        case "State":
                            spatial_builder.Append(SearchLink.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisValue.Value.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "ST") + thisValue.Value + SearchLinkEnd);
                            break;

                        case "County":
                            spatial_builder.Append(SearchLink.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisValue.Value.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CT") + thisValue.Value + SearchLinkEnd);
                            break;

                        case "City":
                            spatial_builder.Append(SearchLink.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisValue.Value.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CI") + thisValue.Value + SearchLinkEnd);
                            break;
                    }
                }

                // Display this
                Output.Append("          " + spatial_builder);

                // Move to check the next spatial
                spatial_count++;
            }

            Output.AppendLine("</dd>");
            Output.AppendLine();
        }
    }
}
