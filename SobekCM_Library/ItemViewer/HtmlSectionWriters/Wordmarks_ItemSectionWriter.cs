using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;

namespace SobekCM.Library.ItemViewer.HtmlSectionWriters
{
    /// <summary> Item HTML section writer adds the wordmarks to the item display </summary>
    public class Wordmarks_ItemSectionWriter : iItemSectionWriter
    {
        /// <summary> Write the item wordmarks to the item display html</summary>
        /// <param name="Output"> Stream to which to write </param>
        /// <param name="Prototyper"> Current item viewer prototyper </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        /// <param name="Behaviors"> Behaviors for the current view and situation </param>
        public void Write_HTML(TextWriter Output, iItemViewerPrototyper Prototyper, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues, List<HtmlSubwriter_Behaviors_Enum> Behaviors)
        {
            // Add any wordmarks
            if ((CurrentItem.Behaviors.Wordmarks != null) && (CurrentItem.Behaviors.Wordmarks.Count > 0))
            {
                Output.WriteLine("\t<div id=\"sbkIsw_Wordmarks\">");

                // Compute the URL options which may be needed
                string url_options = UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode);
                string urlOptions1 = String.Empty;
                string urlOptions2 = String.Empty;
                if (url_options.Length > 0)
                {
                    urlOptions1 = "?" + url_options;
                    urlOptions2 = "&" + url_options;
                }

                // Step through each wordmark mentioned in the brief item
                foreach (string thisIcon in CurrentItem.Behaviors.Wordmarks)
                {
                    // Look for a match in the dictionary
                    if (UI_ApplicationCache_Gateway.Icon_List.ContainsKey(thisIcon))
                    {
                        Wordmark_Icon wordmarkInfo = UI_ApplicationCache_Gateway.Icon_List[thisIcon];

                        Output.WriteLine("\t\t" + wordmarkInfo.HTML.Replace("<%BASEURL%>", RequestSpecificValues.Current_Mode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2));
                    }
                }

                Output.WriteLine("\t</div>");
            }
            //else
            //{
            //    Output.WriteLine("\t<div id=\"sbkIsw_NoWordmarks\">&nbsp;</div>");
            //}
        }
    }
}