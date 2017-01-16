using System.IO;
using SobekCM.Core.BriefItem;
using SobekCM.Library.ItemViewer.Viewers;

namespace SobekCM.Library.ItemViewer.HtmlHeadWriters
{
    /// <summary> Writes open graph metadata for the current item being displayed into the HTML head </summary>
    public class OpenGraph_ItemHtmlHeadWriter : iItemHtmlHeadWriter
    {
        /// <summary> Write some open graph metadata about the digital resource into the HTML head </summary>
        /// <param name="Output"> Stream to which to write within the HTML head tag </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        public void Write_Within_HTML_Head(TextWriter Output, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues)
        {
            // If the item is NULL, do nothing
            if (CurrentItem == null)
                return;

            Output.WriteLine("     <link title=\"MODS Metadata Schema\" rel=\"schema.mods\" href=\"http://www.loc.gov/standards/mods/mods.xsd\" />");
            Output.WriteLine("     <meta name=\"mods.title\" content=\"" + CurrentItem.Title.Replace("\"", "'") + "\" />");
        }
    }
}
