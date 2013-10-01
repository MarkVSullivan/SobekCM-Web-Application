#region using directives

using System.Collections.Generic;
using System.IO;
using SobekCM.Library.HTML;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Dataset viewer is used to dispay codebook information present in the XSD accompanying a XML dataset </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Dataset_CodeBook_ItemViewer : abstractItemViewer
	{
		/// <summary> Constructor for a new instance of the Dataset_CodeBook_ItemViewer class </summary>
		public Dataset_CodeBook_ItemViewer()
		{

		}

		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.JPEG_Text_Two_Up"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.Dataset_Codebook; }
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

		/// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
		/// <value> This returns -1, which allows this to use all the screen </value>
		public override int Viewer_Width
		{
			get
			{
				return -1;
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

		/// <summary> Gets the collection of special behaviors which this item viewer
		/// requests from the main HTML subwriter. </summary>
		public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
		{
			get
			{
				return new List<HtmlSubwriter_Behaviors_Enum> 
					{
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar
					};
			}
		}

		/// <summary> Stream to which to write the HTML for this subwriter  </summary>
		/// <param name="Output"> Response stream for the item viewer to write directly to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Dataset_CodeBook_ItemViewer.Write_Main_Viewer_Section", "");
			}

			Output.WriteLine("\t\t<td id=\"sbkDcv_MainArea\">");
			Output.WriteLine("CODEBOOK");
            Output.WriteLine("\t\t</td>" );
		}
	}
}