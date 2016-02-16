#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays a JPEG file related to the digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
	public class JPEG_ItemViewer : abstractItemViewer
	{
		private int width;
        private int height;

        /// <summary> Constructor for a new instance of the JPEG_ItemViewer class </summary>
		public JPEG_ItemViewer()
		{
			width = -1;
            height = -1;
		}

        /// <summary> Constructor for a new instance of the JPEG_ItemViewer class </summary>
        /// <param name="Attributes"> Attributes for the JPEG file to display, including width </param>
		public JPEG_ItemViewer( string Attributes )
		{
			width = -1;
            height = -1;

            // Parse if there were attributes
            if (Attributes.Length <= 0) return;

            string[] splitter = Attributes.Split(";".ToCharArray());
            foreach (string thisSplitter in splitter.Where(ThisSplitter => ThisSplitter.ToUpper().IndexOf("WIDTH") >= 0))
            {
                Int32.TryParse(thisSplitter.Substring(thisSplitter.IndexOf("=") + 1), out width);
            }
            foreach (string thisSplitter in splitter.Where(ThisSplitter => ThisSplitter.ToUpper().IndexOf("HEIGHT") >= 0))
            {
                Int32.TryParse(thisSplitter.Substring(thisSplitter.IndexOf("=") + 1), out height);
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
				    foreach (string thisSplitter in splitter.Where(ThisSplitter => ThisSplitter.ToUpper().IndexOf("WIDTH") >= 0))
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

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG_ItemViewer.Write_Main_Viewer_Section", "");
            }

            // Determine if there is a zoomable version of this page
	        bool isZoomable = false;
	        if (( BriefItem.Images != null ) && ( BriefItem.Images.Count > CurrentMode.Page - 1))
	        {
		        int currentPageIndex = CurrentMode.Page.HasValue ? CurrentMode.Page.Value : 1;
	            foreach (BriefItem_File thisFile in BriefItem.Images[currentPageIndex - 1].Files)
	            {
	                if (String.Compare(thisFile.File_Extension, ".jp2", StringComparison.OrdinalIgnoreCase) == 0)
	                {
                        isZoomable = true;
                        break;
	                }
	            }
	        }

            // Now, check to see if the JPEG2000 viewer is included here
            bool zoomableViewerIncluded = false;
            foreach (BriefItem_BehaviorViewer viewer in BriefItem.Behaviors.Viewers)
            {
                if (String.Compare(viewer.ViewerType, "JPEG2000", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    zoomableViewerIncluded = true;
                    break;
                }
            }

	        string displayFileName = CurrentItem.Web.Source_URL + "/" + FileName;

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((BriefItem.Behaviors.Dark_Flag) || (BriefItem.Behaviors.IP_Restriction_Membership > 0))
                displayFileName = CurrentMode.Base_URL + "files/" + BriefItem.BibID + "/" + BriefItem.VID + "/" + FileName;


            string name_for_image = HttpUtility.HtmlEncode(BriefItem.Title);


            if ((BriefItem.Images != null) && (BriefItem.Images.Count > 1) && (Current_Page - 1 < BriefItem.Images.Count))
            {
                string name_of_page = BriefItem.Images[Current_Page - 1].Label;
                name_for_image = name_for_image + " - " + HttpUtility.HtmlEncode(name_of_page);
            }



            // Add the HTML for the image
	        if ((isZoomable) && ( zoomableViewerIncluded ))
	        {
		        string currViewer = CurrentMode.ViewerCode;
		        CurrentMode.ViewerCode = CurrentMode.ViewerCode.ToLower().Replace("j", "") + "x";
		        string toZoomable = UrlWriterHelper.Redirect_URL(CurrentMode);
	            CurrentMode.ViewerCode = currViewer;
		        Output.WriteLine("\t\t<td id=\"sbkJiv_ImageZoomable\">");
		        Output.WriteLine("Click on image below to switch to zoomable version<br />");
		        Output.WriteLine("<a href=\"" + toZoomable + "\" title=\"Click on image to switch to zoomable version\">");

		        Output.Write("\t\t\t<img itemprop=\"primaryImageOfPage\" ");
		        if ((height > 0) && (width > 0))
			        Output.Write("style=\"height:" + height + "px;width:" + width + "px;\" ");
		        Output.WriteLine("src=\"" + displayFileName + "\" alt=\"" + name_for_image + "\" />");

		        Output.WriteLine("</a>");
	        }
	        else
	        {
		        Output.WriteLine("\t\t<td align=\"center\" id=\"sbkJiv_Image\">");

		        Output.Write("\t\t\t<img itemprop=\"primaryImageOfPage\" ");
		        if ((height > 0) && (width > 0))
			        Output.Write("style=\"height:" + height + "px;width:" + width + "px;\" ");
		        Output.WriteLine("src=\"" + displayFileName + "\" alt=\"" + name_for_image + "\" />");
	        }

	        Output.WriteLine("\t\t</td>");

        }
	}
}
