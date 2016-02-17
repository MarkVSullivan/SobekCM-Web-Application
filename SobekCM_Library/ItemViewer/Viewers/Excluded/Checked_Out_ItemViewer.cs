#region Using directives

using System.IO;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer is substituted for the regular image/download viewers if an item is 
    /// restricted to single fair use and someone else is currently viewing the item.</summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer_OLD"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Checked_Out_ItemViewer : abstractItemViewer_OLD
    {
        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkCoiv_Viewer' </value>
        public override string Viewer_CSS
        {
            get { return "sbkCoiv_Viewer"; }
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Checked_Out"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Checked_Out; }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is a single page viewer, so this property always returns the value 1</value>
        public override int PageCount
        {
            get { return 1; }
        }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get { return ItemViewer_PageSelector_Type_Enum.NONE; }
        }


        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<td style=\"text-align:center;font-weight:bold;\" colspan=\"3\"><br /><br />The item you have requested contains copyright material and is reserved for single-use.  <br /><br />Someone has currently checked out this digital copy for viewing.  <br /><br />Please try again in several minutes.<br /><br /><br /></td>");
        }
    }
}
