#region Using directives

using System;
using System.Linq;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays a JPEG file related to the digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
	public class JPEG_ItemViewer : abstractItemViewer
	{
		private int width;

        /// <summary> Constructor for a new instance of the JPEG_ItemViewer class </summary>
		public JPEG_ItemViewer()
		{
			width = -1;
		}

        /// <summary> Constructor for a new instance of the JPEG_ItemViewer class </summary>
        /// <param name="Attributes"> Attributes for the JPEG file to display, including width </param>
		public JPEG_ItemViewer( string Attributes )
		{
			width = -1;

            // Parse if there were attributes
            if (Attributes.Length <= 0) return;

            string[] splitter = Attributes.Split(";".ToCharArray());
            foreach (string thisSplitter in splitter.Where(thisSplitter => thisSplitter.ToUpper().IndexOf("WIDTH") >= 0))
            {
                Int32.TryParse(thisSplitter.Substring(thisSplitter.IndexOf("=") + 1), out width);
            }
		}

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.JPEG"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.JPEG; }
        }
        
        /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <remarks> This always returns the value TRUE for this viewer </remarks>
        public override bool Override_On_Checked_Out
        {
            get
            {
                return true;
            }
        }

        /// <summary> Sets the attributes for the JPEG file to display, including width  </summary>
		public override string Attributes
		{
			set
			{
				// Parse if there were attributes
				if ( value.Length > 0 )
				{
				    string[] splitter = value.Split(";".ToCharArray() );
				    foreach (string thisSplitter in splitter.Where(thisSplitter => thisSplitter.ToUpper().IndexOf("WIDTH") >= 0))
				    {
				        Int32.TryParse(thisSplitter.Substring(thisSplitter.IndexOf("=") + 1), out width);
				    }
				}
			}
		}

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This returns the width of the jpeg file to display or 500, whichever is larger </value>
		public override int Viewer_Width
		{
			get {
			    return width < 500 ? 500 : width;
			}
		}

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

            // Add the HTML for the image
            Literal mainLiteral = new Literal
                                      { Text = "\t\t<td align=\"center\" colspan=\"3\" id=\"printedimage\">" + Environment.NewLine + "\t\t\t<img src=\"" + CurrentItem.SobekCM_Web.Source_URL + "/" + FileName + "\" alt=\"MISSING IMAGE\" />" + Environment.NewLine + "\t\t</td>" + Environment.NewLine };
            placeHolder.Controls.Add( mainLiteral );
        }


        /// <summary> Adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Returns FALSE since nothing was added to the left navigational bar </returns>
        /// <remarks> For this item viewer, this method does nothing except return FALSE </remarks>
        public override bool Add_Nav_Bar_Menu_Section(PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
		}
	}
}
