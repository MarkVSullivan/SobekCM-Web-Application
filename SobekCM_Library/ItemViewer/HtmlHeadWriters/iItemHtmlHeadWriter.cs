using System.IO;
using SobekCM.Core.BriefItem;
using SobekCM.Library.ItemViewer.Viewers;

namespace SobekCM.Library.ItemViewer.HtmlHeadWriters
{
    /// <summary> Defines the interface for the objects that can write into the HTML head during item display </summary>
    public interface iItemHtmlHeadWriter
    {
        /// <summary> Write anything within the HTML head while displaying an item </summary>
        /// <param name="Output"> Stream to which to write within the HTML head tag </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        void Write_Within_HTML_Head(TextWriter Output, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues);
    }
}
