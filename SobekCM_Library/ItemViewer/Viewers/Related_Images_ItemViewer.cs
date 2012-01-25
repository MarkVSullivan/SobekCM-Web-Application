#region Using directives

using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Bib_Package.Divisions;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Item viewer displays thumbnails of all the page images related to this digital resource. </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Related_Images_ItemViewer : abstractItemViewer
	{
		private readonly string title;

		/// <summary> Constructor for a new instance of the Flash_ItemViewer class </summary>
		/// <param name="Title"> Title to display for these related images (varies slightly by material type)</param>
		public Related_Images_ItemViewer(string Title)
		{
		    title = String.IsNullOrEmpty(Title) ? "Related Images" : Title;
		}

	    /// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Related_Images"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.Related_Images; }
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
		/// <value> This always returns the value 650 </value>
		public override int Viewer_Width
		{
			get
			{
				return 650;
			}
		}


		/// <summary> Gets the number of pages for this viewer </summary>
		/// <remarks> If there are more than 100 related images, then the thumbnails are paged at 60 a page</remarks>
		public override int PageCount
		{
			get
			{
			    return (CurrentItem.SobekCM_Web.Static_PageCount <= 100) ? 1 : (((CurrentItem.SobekCM_Web.Static_PageCount - 1) / 60)) + 1;
			}
		}

		/// <summary> Gets the url to go to the first page of thumbnails </summary>
		/// <remarks> If there are more than 100 related images, then the thumbnails are paged at 60 a page</remarks>
		public override string First_Page_URL
		{
			get 
			{
				return ((PageCount > 1) && (CurrentMode.Page > 1)) ? CurrentMode.Redirect_URL("1thumbs") : String.Empty;
			}
		}

		/// <summary> Gets the url to go to the preivous page of thumbnails </summary>
		/// <remarks> If there are more than 100 related images, then the thumbnails are paged at 60 a page</remarks>
		public override string Previous_Page_URL
		{
			get
			{
				return ((PageCount > 1) && ( CurrentMode.Page > 1 )) ? CurrentMode.Redirect_URL( (CurrentMode.Page - 1).ToString() + "thumbs" ) : String.Empty;
			}
		}

		/// <summary> Gets the url to go to the next page of thumbnails </summary>
		/// <remarks> If there are more than 100 related images, then the thumbnails are paged at 60 a page</remarks>
		public override string Next_Page_URL
		{
			get
			{
				int temp_page_count = PageCount;
				return  ( temp_page_count > 1 ) && (CurrentMode.Page < temp_page_count) ? CurrentMode.Redirect_URL( (CurrentMode.Page + 1).ToString() + "thumbs" ) :  String.Empty;
			}
		}

		/// <summary> Gets the url to go to the last page of thumbnails </summary>
		/// <remarks> If there are more than 100 related images, then the thumbnails are paged at 60 a page</remarks>
		public override string Last_Page_URL
		{
			get
			{
				int temp_page_count = PageCount;
			    return (temp_page_count > 1) && (CurrentMode.Page < temp_page_count) ? CurrentMode.Redirect_URL(temp_page_count.ToString() + "thumbs") : String.Empty;
			}
		}


		/// <summary> Gets the names to show in the Go To combo box </summary>
		/// <value> This returns an empty string away; GO TO should not be shown for this item viewer </value>
		public override string[] Go_To_Names
		{
			get
			{
				return new string[] { };
			}
		}

		/// <summary> Adds the main view section to the page turner </summary>
		/// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Related_Images_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
			}

			// Build the value
			StringBuilder builder = new StringBuilder(5000);

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;
			ushort current_view_page = CurrentMode.Page;

			// Start the citation table
			builder.AppendLine( "\t\t<!-- RELATED IMAGES VIEWER OUTPUT -->" );
			if (CurrentItem.SobekCM_Web.Static_PageCount < 100)
			{
                builder.AppendLine("\t\t<td align=\"left\" height=\"40px\" ><span class=\"SobekViewerTitle\"><b>" + translator.Get_Translation(title, CurrentMode.Language) + "</b></span></td></tr>");
                builder.AppendLine("\t<tr>") ;
			}
            builder.AppendLine("\t\t<td>" );

			// Start this table
			string width_statement = String.Empty;
			if (CurrentItem.SobekCM_Web.Static_PageCount > 2)
			{
				width_statement = " width=\"33%\"";
			}

            builder.AppendLine("<table align=\"center\" width=\"100%\" cellspacing=\"15px\">");
            builder.AppendLine("\t<tr valign=\"top\">");

			int images_per_page = 100;
			if (CurrentItem.SobekCM_Web.Static_PageCount > 100)
				images_per_page = 60;
			ushort page = (ushort)(CurrentMode.Page - 1);
			if (page > (CurrentItem.SobekCM_Web.Static_PageCount - 1) / images_per_page)
				page = (ushort)((CurrentItem.SobekCM_Web.Static_PageCount - 1) / images_per_page);

			// Step through each page in the item
			int col = 0;
			for (int page_index = page * images_per_page; (page_index < (page + 1) * images_per_page) && (page_index < CurrentItem.SobekCM_Web.Static_PageCount); page_index++)
			{
				Page_TreeNode thisPage = CurrentItem.SobekCM_Web.Pages_By_Sequence[page_index];

				// Should a new row be started
				if (col == 3)
				{
					col = 0;
                    builder.AppendLine("\t</tr>");
                    builder.AppendLine("\t<tr valign=\"top\">");
				}

				// Find the jpeg image
				foreach (SobekCM_File_Info thisFile in thisPage.Files.Where(thisFile => thisFile.System_Name.IndexOf(".jpg") > 0))
				{
				    // Get the image URL
				    CurrentMode.Page = (ushort)(page_index + 1);
				    CurrentMode.ViewerCode = (page_index + 1).ToString();
				    string image_url = (CurrentItem.SobekCM_Web.Source_URL + "/" + thisFile.System_Name.Replace(".jpg", "thm.jpg")).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
				    string url = CurrentMode.Redirect_URL().Replace("&", "&amp;").Replace("\"", "&quot;");

				    if (CurrentItem.SobekCM_Web.Static_PageCount > 2)
				    {
				        // Start this table section
				        if (col == 0)
				        {
				            builder.Append("\t\t<td align=\"left\"" + width_statement + " onmouseover=\"this.className='thumbnailHighlight'\" onmouseout=\"this.className='thumbnailNormal'\" onmousedown=\"window.location.href='" + url + "';\">");
				        }
				        if (col == 1)
				        {
				            builder.Append("\t\t<td align=\"center\"" + width_statement + " onmouseover=\"this.className='thumbnailHighlight'\" onmouseout=\"this.className='thumbnailNormal'\" onmousedown=\"window.location.href='" + url + "';\">");
				        }
				        if (col == 2)
				        {
				            builder.Append("\t\t<td align=\"right\"" + width_statement + " onmouseover=\"this.className='thumbnailHighlight'\" onmouseout=\"this.className='thumbnailNormal'\" onmousedown=\"window.location.href='" + url + "';\">");
				        }
				    }
				    else
				    {
				        builder.Append("\t\t<td align=\"center\"" + width_statement + "  onmouseover=\"this.className='thumbnailHighlight'\" onmouseout=\"this.className='thumbnailNormal'\" onmousedown=\"window.location.href='" + url + "';\">");
				    }

				    builder.AppendLine("<table width=\"150\"><tr><td><a href=\"" + url + "\"><img src=\"" + image_url + "\" alt=\"MISSING THUMBNAIL\" class=\"itemThumbnails\" /></a></td></tr><tr><td align=\"center\"><span class=\"SobekThumbnailText\">" + thisPage.Label + "</span></td></tr></table></td>" );
				    col++;
				    break;
				}
			}

			// End this table
            builder.AppendLine("\t</tr>");
            builder.AppendLine("</table>");

			// Finish the citation table
            builder.AppendLine("\t\t</td>" );
            builder.AppendLine("\t\t<!-- END RELATED IMAGES VIEWER OUTPUT -->" );

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;
			CurrentMode.Page = current_view_page;

			// Add the HTML for the image
		    Literal mainLiteral = new Literal {Text = builder.ToString()};
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
				Tracer.Add_Trace("Related_Images_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
			}

			return false;
		}
	}
}
