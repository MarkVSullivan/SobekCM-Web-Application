using System.IO;
using SobekCM.Core.Aggregations;

namespace SobekCM.Library.AggregationViewer.HtmlHeadWriters
{
    /// <summary> Writes Dublin Core for the current aggregation being displayed into the HTML head </summary>
    public class DublinCore_AggregationHtmlHeadWriter : iAggregationHtmlHeadWriter
    {
        /// <summary> Write the dublin core metadata about the aggregation into the HTML head </summary>
        /// <param name="Output"> Stream to which to write within the HTML head tag </param>
        /// <param name="CurrentAggregation"> Current aggregation being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        public void Write_Within_HTML_Head(TextWriter Output, Item_Aggregation CurrentAggregation, RequestCache RequestSpecificValues)
        {
            // If the item is NULL, do nothing
            if (CurrentAggregation == null)
                return;

            Output.WriteLine("  <link title=\"Dublin Core Metadata Schema\" rel=\"schema.DC\" href=\"http://purl.org/DC/elements/1.1/\" />");
            Output.WriteLine("  <meta name=\"DC.title\" content=\"" + CurrentAggregation.Name.Replace("\"", "'") + "\" />");
        }
    }
}
