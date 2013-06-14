#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object.Divisions;

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

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG_ItemViewer.Write_Main_Viewer_Section", "");
            }


            string displayFileName = CurrentItem.Web.Source_URL + "/" + FileName;

            List<SobekCM_File_Info> first_page_files = ((Page_TreeNode) CurrentItem.Divisions.Physical_Tree.Pages_PreOrder[0]).Files;
            string first_page_jpeg = String.Empty;
            foreach (SobekCM_File_Info thisFile in first_page_files)
            {
                if ((thisFile.System_Name.ToLower().IndexOf(".jpg") > 0) &&
                    (thisFile.System_Name.ToLower().IndexOf("thm.jpg") < 0))
                {
                    first_page_jpeg = thisFile.System_Name;
                    break;
                }
            }


            string first_page_complete_url = CurrentItem.Web.Source_URL + "/" + first_page_jpeg;


            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if (( CurrentItem.Behaviors.Dark_Flag ) || ( CurrentItem.Behaviors.IP_Restriction_Membership > 0 ))
                displayFileName = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/" + FileName;


            string name_for_image = HttpUtility.HtmlEncode(CurrentItem.Bib_Info.Main_Title.ToString());
            

            if (( CurrentItem.Web.Pages_By_Sequence.Count > 1) && ( Current_Page -1 < CurrentItem.Web.Pages_By_Sequence.Count  ))
            {
                string name_of_page = CurrentItem.Web.Pages_By_Sequence[Current_Page - 1].Label;
                name_for_image = name_for_image + " - " + HttpUtility.HtmlEncode(name_of_page);
            }


            // Add the HTML for the image
            Output.WriteLine("\t\t<td align=\"center\" colspan=\"3\" id=\"printedimage\">" + Environment.NewLine +
                             "\t\t\t<img src=\"" + displayFileName + "\" alt=\"MISSING IMAGE\" title=\"" +
                             name_for_image + "\" />" + Environment.NewLine + "\t\t</td>");

        }
	}
}
