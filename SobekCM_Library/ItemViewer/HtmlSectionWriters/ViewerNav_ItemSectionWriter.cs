using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.BriefItem;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Viewers;

namespace SobekCM.Library.ItemViewer.HtmlSectionWriters
{
    /// <summary> Item HTML section writer adds the nav-bar section 
    /// from the item viewer to the item display </summary>
    public class ViewerNav_ItemSectionWriter : iItemSectionWriter
    {
        /// <summary> Write the nav-bar section 
        /// from the item viewer to the item display html</summary>
        /// <param name="Output"> Stream to which to write </param>
        /// <param name="Prototyper"> Current item viewer prototyper </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        /// <param name="Behaviors"> Behaviors for the current view and situation </param>
        public void Write_HTML(TextWriter Output, iItemViewerPrototyper Prototyper, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues, List<HtmlSubwriter_Behaviors_Enum> Behaviors)
        {
            // Allow the item/page viewer to show anything in the left navigational menu
            if (CurrentViewer != null)
                CurrentViewer.Write_Left_Nav_Menu_Section(Output, RequestSpecificValues.Tracer);
        }
    }
}