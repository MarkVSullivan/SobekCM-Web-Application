#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Library.HTML;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Item viewer displays a JPEG file with the related full text and possibility to edit
	/// the full text to correct transcribe the image </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class JPEG_Text_Two_Up_ItemViewer : abstractItemViewer
	{
		private int width;
		private int height;

		/// <summary> Constructor for a new instance of the JPEG_Text_Two_Up_ItemViewer class </summary>
		public JPEG_Text_Two_Up_ItemViewer()
		{
			width = -1;
			height = -1;
		}

		/// <summary> Constructor for a new instance of the JPEG_Text_Two_Up_ItemViewer class </summary>
        /// <param name="Attributes"> Attributes for the JPEG file to display, including width </param>
		public JPEG_Text_Two_Up_ItemViewer(string Attributes)
		{
			width = -1;
            height = -1;

            // Parse if there were attributes
            if (Attributes.Length <= 0) return;

            string[] splitter = Attributes.Split(";".ToCharArray());
            foreach (string thisSplitter in splitter.Where(thisSplitter => thisSplitter.ToUpper().IndexOf("WIDTH") >= 0))
            {
                Int32.TryParse(thisSplitter.Substring(thisSplitter.IndexOf("=") + 1), out width);
            }
            foreach (string thisSplitter in splitter.Where(thisSplitter => thisSplitter.ToUpper().IndexOf("HEIGHT") >= 0))
            {
                Int32.TryParse(thisSplitter.Substring(thisSplitter.IndexOf("=") + 1), out height);
            }
		}

		/// <summary> Sets the attributes for the JPEG file to display, including width  </summary>
		public override string Attributes
		{
			set
			{
				// Parse if there were attributes
				if (value.Length > 0)
				{
					string[] splitter = value.Split(";".ToCharArray());
					foreach (string thisSplitter in splitter.Where(thisSplitter => thisSplitter.ToUpper().IndexOf("WIDTH") >= 0))
					{
						Int32.TryParse(thisSplitter.Substring(thisSplitter.IndexOf("=") + 1), out width);
					}
				}
			}
		}

		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.JPEG_Text_Two_Up"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.JPEG_Text_Two_Up; }
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
				Tracer.Add_Trace("JPEG_Text_Two_Up_ItemViewer.Write_Main_Viewer_Section", "");
			}
		}


	}
}
