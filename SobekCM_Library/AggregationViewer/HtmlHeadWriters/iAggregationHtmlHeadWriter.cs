using System.IO;
using SobekCM.Core.Aggregations;

namespace SobekCM.Library.AggregationViewer.HtmlHeadWriters
{
    /// <summary> Defines the interface for the objects that can write into the HTML head during aggregation display </summary>
    public interface iAggregationHtmlHeadWriter
    {
        /// <summary> Write anything within the HTML head while displaying an aggregation </summary>
        /// <param name="Output"> Stream to which to write within the HTML head tag </param>
        /// <param name="CurrentAggregation"> Current aggregation being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        void Write_Within_HTML_Head(TextWriter Output, Item_Aggregation CurrentAggregation, RequestCache RequestSpecificValues);
    }
}
