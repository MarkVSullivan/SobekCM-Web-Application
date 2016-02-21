using SobekCM.Core.Navigation;
using SobekCM.Core.WebContent;
using SobekCM.Library.HTML;
using SobekCM.Library.WebContentViewer.Viewers;

namespace SobekCM.Library.WebContentViewer
{
    /// <summary> Factory class that generate and returns the requested view for a web content page </summary>
    /// <remarks> This is used by <see cref="Web_Content_HtmlSubwriter"/> class </remarks>
    public class WebContentViewer_Factory
    {
        /// <summary> Returns a built collection viewer matching request </summary>
        /// <param name="ViewType"> Web content view type </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="StaticPage"> Static page info for this request </param>
        /// <returns> Web content viewer that extends the <see cref="abstractWebContentViewer"/> class. </returns>
        public static abstractWebContentViewer Get_Viewer(WebContent_Type_Enum ViewType, RequestCache RequestSpecificValues, HTML_Based_Content StaticPage )
        {
            switch (ViewType)
            {
                case WebContent_Type_Enum.Delete_Verify:
                    return new Delete_Verify_WebContentViewer(RequestSpecificValues, StaticPage);

                case WebContent_Type_Enum.Manage_Menu:
                    return new Manage_Menu_WebContentViewer(RequestSpecificValues, StaticPage);

                case WebContent_Type_Enum.Milestones:
                    return new Work_History_WebContentViewer(RequestSpecificValues, StaticPage);

                case WebContent_Type_Enum.Permissions:
                    return new User_Permissions_WebContentViewer(RequestSpecificValues, StaticPage);

                case WebContent_Type_Enum.Usage:
                    return new Usage_Statistics_WebContentViewer(RequestSpecificValues, StaticPage);

                default:
                    return null;
            }
        }

    }
}
