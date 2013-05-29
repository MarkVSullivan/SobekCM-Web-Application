using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Class to allow a user to add coordinate information to 
    /// a digital resource ( map coverage, points of interest, etc.. ) </summary>
    public class Google_Coordinate_Entry_ItemViewer : abstractItemViewer
    {
        /// <summary> Constructor for a new instance of the Google_Coordinate_Entry_ItemViewer class </summary>
        public Google_Coordinate_Entry_ItemViewer()
        {
            // Empty for now
        }


        /// <summary> Flag indicates if the item viewer should add the standard item menu, or
        /// if this item viewer overrides that menu and will write its own menu </summary>
        /// <remarks> By default, this returns TRUE.  The QC and the spatial editing itemviewers create their own custom menus
        /// due to the complexity of the work being done in those viewers. </remarks>
        public override bool Include_Standard_Item_Menu
        {
            get
            {
                return false;
            }
        }
        /// <summary> Flag indicates if the header (with the title, group title, etc..) should be displayed </summary>
        /// <value> This always returns the value FALSE, to suppress the standard header information </value>
        public override bool Show_Header
        {
            get
            {
                return false;
            }
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

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 800 </value>
        public override int Viewer_Width
        {
            get { return 800; }
        }

        /// <summary> Property gets the type of item viewer </summary>
        /// <value> This always returns ItemViewer_Type_Enum.Google_Coordinate_Entry </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Google_Coordinate_Entry; }
        }

        /// <summary> Abstract method adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if this viewer added something to the left navigational bar, otherwise FALSE</returns>
        public override bool Add_Nav_Bar_Menu_Section(PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer)
        {
            return false;
        }

        /// <summary> Abstract method adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            // Start to build the response
            StringBuilder responseBuilder = new StringBuilder();
            responseBuilder.AppendLine("<td>");

            responseBuilder.AppendLine("THIS IS WHERE EVERYTHING WILL GO");


            responseBuilder.AppendLine("</td>");

            // Add the literal to the placeholder
            Literal placeHolderText = new Literal();
            placeHolderText.Text = responseBuilder.ToString();
            placeHolder.Controls.Add(placeHolderText);
            
        }
    }
}
