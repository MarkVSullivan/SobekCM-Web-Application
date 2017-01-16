using System.IO;
using SobekCM.Core.BriefItem;
using SobekCM.Library.ItemViewer.Viewers;

namespace SobekCM.Library.ItemViewer.HtmlHeadWriters
{
    /// <summary> Writes Dublin Core for the current item being displayed into the HTML head </summary>
    public class DublinCore_ItemHtmlHeadWriter : iItemHtmlHeadWriter
    {
        /// <summary> Write the dublin core metadata about the digital resource into the HTML head </summary>
        /// <param name="Output"> Stream to which to write within the HTML head tag </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        public void Write_Within_HTML_Head(TextWriter Output, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues)
        {
            // If the item is NULL, do nothing
            if (CurrentItem == null)
                return;

            Output.WriteLine("     <link title=\"Dublin Core Metadata Schema\" rel=\"schema.DC\" href=\"http://purl.org/DC/elements/1.1/\" />");
            Output.WriteLine("     <meta name=\"DC.title\" content=\"" + CurrentItem.Title.Replace("\"", "'") + "\" />");
        }
    }
}
