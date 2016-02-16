#region Using directives

using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer is substituted for the regular image/download viewers if an item is 
    /// restricted by IP address range and is not accessible to the the current user </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Restricted_ItemViewer : abstractItemViewer
    {
        private readonly string restrictedMessage;

        /// <summary> Constructor for a new instance of the Restricted_ItemViewer class </summary>
        /// <param name="Restricted_Message"> Message to show to the user if they are restricted </param>
        public Restricted_ItemViewer( string Restricted_Message )
        {
            restrictedMessage = Restricted_Message;
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkRiv_Viewer' </value>
        public override string Viewer_CSS
        {
            get { return "sbkRiv_Viewer"; }
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Restricted"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Restricted; }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is a single page viewer, so this property always returns the value 1</value>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
			// Replace item URL in the restricted message
	        CurrentMode.ViewerCode = string.Empty;
	        string msg = restrictedMessage.Replace("<%ITEMURL%>", UrlWriterHelper.Redirect_URL(CurrentMode));

			Output.WriteLine("<td style=\"text-align:left;\" id=\"sbkRes_MainArea\">" + msg + "</td>");
        }
    }
}
