using System.IO;
using SobekCM.Core.Aggregations;


namespace SobekCM.Library.AggregationViewer.HtmlHeadWriters
{
    /// <summary> Writes simple JSON for the current aggregation being displayed into the HTML head </summary>
    public class JSON_AggregationHtmlHeadWriter : iAggregationHtmlHeadWriter
    {
        /// <summary> Write the simple JSON metadata about the aggregation into the HTML head </summary>
        /// <param name="Output"> Stream to which to write within the HTML head tag </param>
        /// <param name="CurrentAggregation"> Current aggregation being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        public void Write_Within_HTML_Head(TextWriter Output, Item_Aggregation CurrentAggregation, RequestCache RequestSpecificValues)
        {
            // If the item is NULL, do nothing
            if (CurrentAggregation == null)
                return;

            // Create the simple aggregation object, and get the JSON
            Simple_Aggregation simpleAggr = new Simple_Aggregation(CurrentAggregation);
            string simpleAggr_asJson = simpleAggr.ToJSON();

            // Write the JSON to the header
            Output.WriteLine("  <script id=\"aggregation_json\" type=\"application/json\">" + simpleAggr_asJson + "</script>");

            // To use in javascript, use: var data = JSON.parse($("#aggregation_json").html());
        }
    }
}
