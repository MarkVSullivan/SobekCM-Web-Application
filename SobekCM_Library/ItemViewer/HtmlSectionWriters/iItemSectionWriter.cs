using System.Collections.Generic;
using System.IO;
using SobekCM.Core.BriefItem;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Viewers;

namespace SobekCM.Library.ItemViewer.HtmlSectionWriters
{
    /// <summary> Defines the interface for the objects that write small sections
    /// within the item HTML writer </summary>
    public interface iItemSectionWriter
    {
        /// <summary> Write HTML within the body of the item page </summary>
        /// <param name="Output"> Stream to which to write </param>
        /// <param name="Prototyper"> Current item viewer prototyper </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        /// <param name="Behaviors"> Behaviors for the current view and situation </param>
        void Write_HTML(TextWriter Output, iItemViewerPrototyper Prototyper, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues, List<HtmlSubwriter_Behaviors_Enum> Behaviors);
    }
}
