#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Configuration;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Item viewer displays thumbnails of all the page images related to this digital resource. </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Related_Images_ItemViewer : abstractItemViewer
	{
		private int thumbnailsPerPage;
		private int thumbnailSize;

	
		/// <summary> Constructor for a new instance of the Flash_ItemViewer class </summary>
		public Related_Images_ItemViewer()
		{
			// Do nothing
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
		/// <value> This always returns the value -1</value>
		public override int Viewer_Width
		{
			get
			{
				return -1;
			 }
		}

		/// <summary> Gets the number of pages for this viewer </summary>
		///<remarks>Edit: The default number of items per page is 50. If a diferent number is selected by the user, this is fetched from the query string</remarks>
		public override int PageCount
		{
			get
			{
				return ((CurrentItem.Web.Static_PageCount - 1) / thumbnailsPerPage) + 1;
			}
		}

		/// <summary> Gets the url to go to the first page of thumbnails </summary>
		public override string First_Page_URL
		{
			get 
			{
				//Get the querystring, if any, from the current url
				string curr_url = HttpContext.Current.Request.RawUrl;
				string queryString = null;

				//Check if query string variables exist in the url
				int index_queryString = curr_url.IndexOf('?');

				if (index_queryString > 0)
				{
					queryString = (index_queryString < curr_url.Length - 1) ? curr_url.Substring(index_queryString) : String.Empty;
				}
					  
				return ((PageCount > 1) && (CurrentMode.Page > 1)) ? CurrentMode.Redirect_URL("1thumbs")+queryString : String.Empty;
			}
		}

		/// <summary> Gets the url to go to the preivous page of thumbnails </summary>
		public override string Previous_Page_URL
		{
			get
			{                
				return ((PageCount > 1) && ( CurrentMode.Page > 1 )) ? CurrentMode.Redirect_URL( (CurrentMode.Page - 1).ToString() + "thumbs" ) : String.Empty;
			}
		}

		/// <summary> Gets the url to go to the next page of thumbnails </summary>
		public override string Next_Page_URL
		{
			get
			{
				int temp_page_count = PageCount;
				return  ( temp_page_count > 1 ) && (CurrentMode.Page < temp_page_count) ? CurrentMode.Redirect_URL( (CurrentMode.Page + 1).ToString() + "thumbs" ) :  String.Empty;
			}
		}

		/// <summary> Gets the url to go to the last page of thumbnails </summary>
		public override string Last_Page_URL
		{
			get
			{
				int temp_page_count = PageCount;
				return (temp_page_count > 1) && (CurrentMode.Page < temp_page_count) ? CurrentMode.Redirect_URL(temp_page_count.ToString() + "thumbs") : String.Empty;
			}
		}


		/// <summary> Gets the names to show in the Go To combo box </summary>
		public override string[] Go_To_Names
		{
			get
			{
				List<string> goToUrls = new List<string>();
				for (int i = 1; i <= PageCount; i++)
				{
					goToUrls.Add(CurrentMode.Redirect_URL(i + "thumbs"));
				}
				return goToUrls.ToArray();
			}
		}

		/// <summary> Adds any viewer_specific information to the item viewer above the standard pagination buttons </summary>
		/// <param name="Output"> Response stream for the item viewer to write directly to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Top_Additional_Navigation_Row(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Related_Images_ItemViewer.Write_Top_Additional_Navigation_Row", "");
			}

			string numOfThumbnails = "thumbnails per page";
			string goToThumbnail = "Go to thumbnail";
			const string Small_Thumbnails = "Switch to small thumbnails";
			const string Medium_Thumbnails = "Switch to medium thumbnails";
			const string Large_Thumbnails = "Switch to large thumbnails";

			if (CurrentMode.Language == Web_Language_Enum.French)
			{
				numOfThumbnails = "vignettes par page";
				//Size_Of_Thumbnail = "la taille des vignettes";
				goToThumbnail = "Aller à l'Vignette";
			}

			if (CurrentMode.Language == Web_Language_Enum.Spanish)
			{
				numOfThumbnails = "miniaturas por página";
				//Size_Of_Thumbnail = "Miniatura de tamaño";
				goToThumbnail = "Ir a la miniatura";
			}

			//Start building the top nav bar
			Output.WriteLine("\t\t<!-- RELATED IMAGES VIEWER TOP NAV ROW -->");
			Output.WriteLine("<tr>");
			Output.WriteLine("<td>");
			 
			//Include the js files
			Output.WriteLine("<script language=\"JavaScript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_related_items.js\"></script>");
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.color-2.1.1.js\"></script>");
		    Output.WriteLine("<table style=\"width: 100%\">");
            Output.WriteLine("\t<tr>");

			//Add the dropdown for the number of thumbnails per page, only if there are >25 thumbnails
			if (CurrentItem.Web.Static_PageCount > 25)
			{
				//Redirect to the first page of results when the number of thumbnails option is changed by the user
				string current_viewercode = CurrentMode.ViewerCode;
				CurrentMode.Redirect_URL("1thumbs");

				//   CurrentMode.Thumbnails_Per_Page = -1;
				//  string current_Page_url = CurrentMode.Redirect_URL("1thumbs");

				// Collect the list of options to display
				List<int> thumbsOptions = new List<int> {25};
				if (CurrentItem.Web.Static_PageCount > 50) thumbsOptions.Add(50);
				if (CurrentItem.Web.Static_PageCount > 100) thumbsOptions.Add(100);
				if (CurrentItem.Web.Static_PageCount > 250) thumbsOptions.Add(250);
				if (CurrentItem.Web.Static_PageCount > 500) thumbsOptions.Add(500);

				// Start the drop down select list 
				Output.WriteLine("\t\t<td style=\"valign:top;text-align:left;padding-left: 20px;\">");
				Output.WriteLine("\t\t\t<select id=\"selectNumOfThumbnails\" onchange=\"location=this.options[this.selectedIndex].value;\">");

				// Step through all the options
				foreach (int thumbOption in thumbsOptions)
				{
					CurrentMode.Thumbnails_Per_Page = (short)thumbOption;
					if (thumbnailsPerPage == thumbOption)
					{
						Output.WriteLine("\t\t\t\t<option value=\"" + CurrentMode.Redirect_URL() + "\" selected=\"selected\">" + thumbOption + " " + numOfThumbnails + "</option>");
					}
					else
					{

                        Output.WriteLine("\t\t\t\t<option value=\"" + CurrentMode.Redirect_URL() + "\">" + thumbOption + " " + numOfThumbnails + "</option>");
					}
				}

				CurrentMode.Thumbnails_Per_Page = -1;
				if (thumbnailsPerPage == int.MaxValue)
				{
                    Output.WriteLine("\t\t\t\t<option value=\"" + CurrentMode.Redirect_URL() + "\" selected=\"selected\">All thumbnails</option>");
				}
				else
				{
                    Output.WriteLine("\t\t\t\t<option value=\"" + CurrentMode.Redirect_URL() + "\">All thumbnails</option>");
				}

				//Reset the Current Mode Thumbnails_Per_Page

				CurrentMode.ViewerCode = current_viewercode;
                Output.WriteLine("\t\t\t</select>");
				Output.WriteLine("\t\t</td>");

			}
			CurrentMode.Thumbnails_Per_Page = -100;


			//Add the control for the thumbnail size

			//Get the icons for the thumbnail sizes
			string image_location = CurrentMode.Default_Images_URL;

			Output.WriteLine("\t\t<td style=\"valign:top;text-align:center;\" id=\"thumbnailsizeselect\">");
			if (thumbnailSize == 1)
                Output.Write("\t\t\t<img src=\"" + image_location + "thumbs3_selected.gif\"/>");
			else
			{
				CurrentMode.Size_Of_Thumbnails = 1;
                Output.Write("\t\t\t<a href=\"" + CurrentMode.Redirect_URL("1thumbs") + "\" title=\"" + Small_Thumbnails + "\"><img src=\"" + image_location + "thumbs3.gif\"/></a>");
			}

			if (thumbnailSize == 2)
                Output.Write("<img src=\"" + image_location + "thumbs2_selected.gif\"/>");
			else
			{
				CurrentMode.Size_Of_Thumbnails = 2;
                Output.Write("<a href=\"" + CurrentMode.Redirect_URL("1thumbs") + "\" title=\"" + Medium_Thumbnails + "\"><img src=\"" + image_location + "thumbs2.gif\"/></a>");
			}
			if (thumbnailSize == 3)
                Output.Write("<img src=\"" + image_location + "thumbs1_selected.gif\"/>");
			else
			{
				CurrentMode.Size_Of_Thumbnails = 3;
                Output.Write("<a href=\"" + CurrentMode.Redirect_URL("1thumbs") + "\" title=\"" + Large_Thumbnails + "\"><img src=\"" + image_location + "thumbs1.gif\"/></a>");
			}
			//Reset the current mode
			CurrentMode.Size_Of_Thumbnails = -1;
			Output.WriteLine("\t\t</td>");


			//Add the dropdown for the thumbnail anchor within the page to directly navigate to
            Output.WriteLine("\t\t<td style=\"valign:top;text-align:right;font-weight:bold;padding-right: 20px;\">");
			Output.WriteLine(goToThumbnail + ":");
			Output.WriteLine("\t\t\t<select id=\"selectGoToThumbnail\" onchange=\"location=this.options[this.selectedIndex].value; AddAnchorDivEffect(this.options[this.selectedIndex].value);\" >");

			//iterate through the page items
			if (CurrentItem.Web.Static_PageCount > 0)
			{
				int thumbnail_count = 0;
				foreach (Page_TreeNode thisFile in CurrentItem.Web.Pages_By_Sequence)
				{
					string currentPageURL1 = CurrentMode.Redirect_URL((thumbnail_count / thumbnailsPerPage + (thumbnail_count % thumbnailsPerPage == 0 ? 0 : 1)).ToString() + "thumbs");

					//  Output.WriteLine("<option value=\"" + current_Page_url1 + "#" + thisFile.Label + "\">" + thisFile.Label + "</option>");
					if (String.IsNullOrEmpty(thisFile.Label))
                        Output.WriteLine("\t\t\t\t<option value=\"" + currentPageURL1 + "#" + thumbnail_count + "\">" + "(page " + thumbnail_count + ")" + "</option>");
					else
					{
                        Output.WriteLine("\t\t\t\t<option value=\"" + currentPageURL1 + "#" + thumbnail_count + "\">" + thisFile.Label + "</option>");
					}

					thumbnail_count++;

				}
			}
			Output.WriteLine("\t\t\t</select>");

		    Output.WriteLine("\t\t</td>");
		    Output.WriteLine("\t</tr>");
            Output.WriteLine("</table>");

			// Finish the nav row controls
			Output.WriteLine("\t\t<!-- END RELATED IMAGES VIEWER NAV ROW -->");
			Output.WriteLine("\t\t</td>");
			Output.WriteLine("\t</tr>");
		}

		/// <summary> Stream to which to write the HTML for this subwriter  </summary>
		/// <param name="Output"> Response stream for the item viewer to write directly to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Related_Images_ItemViewer.Write_Main_Viewer_Section", "");
			}

			int images_per_page = thumbnailsPerPage;
			int size_of_thumbnails = thumbnailSize;

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;
			ushort current_view_page = CurrentMode.Page;

			// Start the citation table
			Output.WriteLine( "\t\t<!-- RELATED IMAGES VIEWER OUTPUT -->" );
			Output.WriteLine("\t<tr>");
			Output.WriteLine("\t\t<td>" );

			// Start the main div for the thumbnails
	
			ushort page = (ushort)(CurrentMode.Page - 1);
			if (page > (CurrentItem.Web.Static_PageCount - 1) / images_per_page)
				page = (ushort)((CurrentItem.Web.Static_PageCount - 1) / images_per_page);

			//Outer div which contains all the thumbnails
			Output.WriteLine("<div style=\"margin:5px;text-align:center;\">");


			// Step through each page in the item
			for (int page_index = page*images_per_page; (page_index < (page + 1)*images_per_page) && (page_index < CurrentItem.Web.Static_PageCount); page_index++)
			{
				// Get this page
				Page_TreeNode thisPage = CurrentItem.Web.Pages_By_Sequence[page_index];

				// Find the jpeg and thumbnail images
				string jpeg = String.Empty;
				string thumbnail = String.Empty;

				foreach (SobekCM_File_Info thisFile in thisPage.Files)
				{
					if (thisFile.System_Name.ToLower().IndexOf(".jpg") > 0)
					{
						if (thisFile.System_Name.ToLower().IndexOf("thm.jpg") > 0)
							thumbnail = thisFile.System_Name;
						else
							jpeg = thisFile.System_Name;
					}
				}

				// If the thumbnail is not in the METS, just guess it's existence
				if (thumbnail.Length == 0)
					thumbnail = jpeg.ToLower().Replace(".jpg", "thm.jpg");

				// Get the image URL
				CurrentMode.Page = (ushort) (page_index + 1);
				CurrentMode.ViewerCode = (page_index + 1).ToString();
				string url = CurrentMode.Redirect_URL();

                // Determine the width information and the URL for the image
                string image_url; // = (CurrentItem.Web.Source_URL + "/" + thumbnail).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
			    int width = -1;
                switch (size_of_thumbnails)
                {
                    case 2:
                        image_url = (CurrentItem.Web.Source_URL + "/" + jpeg).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                        width = 315;
                        break;

                    case 3:
                        image_url = (CurrentItem.Web.Source_URL + "/" + jpeg).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                        width = 472;
                        break;

                    case 4:
                        image_url = (CurrentItem.Web.Source_URL + "/" + jpeg).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                        break;

                    default:
                        image_url = (CurrentItem.Web.Source_URL + "/" + thumbnail).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                        break;

                }

                Output.WriteLine("<span style=\"display:inline-block;text-align:center;padding:4px;\" class=\"thumbnailspan\" id=\"span" + page_index + "\">");
                Output.WriteLine("  <table style=\"display:inline-block;\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a id=\"" + page_index + "\" href=\"" + url + "\" title=\"" + thisPage.Label + "\">");
                if (width > 0)
                    Output.WriteLine("          <img src=\"" + image_url + "\" style=\"width:" + width + "px;\" alt=\"MISSING THUMBNAIL\" />");
                else
                    Output.WriteLine("          <img src=\"" + image_url + "\" alt=\"MISSING THUMBNAIL\" />");
                Output.WriteLine("        </a>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"align:center\">");
                Output.WriteLine("        <span class=\"SobekThumbnailText\" style=\"display:inline-block;\">" + thisPage.Label + "</span>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("  </table>");
                Output.WriteLine("</span>");
                Output.WriteLine();
			}

			//Close the outer div
			Output.WriteLine("</div>");

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;
			CurrentMode.Page = current_view_page;


			// Finish the citation table
			Output.WriteLine("\t\t</td>");
			Output.WriteLine("\t\t<!-- END RELATED IMAGES VIEWER OUTPUT -->");

			Output.WriteLine("<script type=\"text/javascript\"> WindowResizeActions();</script>");
		}

		/// <summary> This provides an opportunity for the viewer to perform any pre-display work
		/// which is necessary before entering any of the rendering portions </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Perform_PreDisplay_Work(Custom_Tracer Tracer)
		{
			// Get the proper number of thumbnails per page
			if (CurrentUser != null)
			{
				// First, pull the thumbnails per page from the user options
				thumbnailsPerPage = CurrentUser.Get_Option("Related_Images_ItemViewer:ThumbnailsPerPage", 50);

				// Or was there a new value in the URL?
				if (CurrentMode.Thumbnails_Per_Page >= -1)
				{
					CurrentUser.Add_Option("Related_Images_ItemViewer:ThumbnailsPerPage", CurrentMode.Thumbnails_Per_Page);
					thumbnailsPerPage = CurrentMode.Thumbnails_Per_Page;
				}
			}
			else
			{
				int tempValue = 50;
				object sessionValue = HttpContext.Current.Session["Related_Images_ItemViewer:ThumbnailsPerPage"];
				if (sessionValue != null)
				{
					int.TryParse(sessionValue.ToString(), out tempValue);
				}
				thumbnailsPerPage = tempValue;

				// Or was there a new value in the URL?
				if (CurrentMode.Thumbnails_Per_Page >= -1)
				{
					HttpContext.Current.Session["Related_Images_ItemViewer:ThumbnailsPerPage"] = CurrentMode.Thumbnails_Per_Page;
					thumbnailsPerPage = CurrentMode.Thumbnails_Per_Page;
				}
			}

			// -1 means to display all thumbnails
			if (thumbnailsPerPage == -1 )
				thumbnailsPerPage = int.MaxValue;

			// Now, reset the value in the navigation object, since we won't need to set it again
			CurrentMode.Thumbnails_Per_Page = -100;

			// Get the proper size of thumbnails per page
			if (CurrentUser != null)
			{
				// First, pull the thumbnails per page from the user options
				thumbnailSize = CurrentUser.Get_Option("Related_Images_ItemViewer:ThumbnailSize", 1);

				// Or was there a new value in the URL?
				if (CurrentMode.Size_Of_Thumbnails > -1)
				{
					CurrentUser.Add_Option("Related_Images_ItemViewer:ThumbnailSize", CurrentMode.Size_Of_Thumbnails);
					thumbnailSize = CurrentMode.Size_Of_Thumbnails;
				}
			}
			else
			{
				int tempValue = 1;
				object sessionValue = HttpContext.Current.Session["Related_Images_ItemViewer:ThumbnailSize"];
				if (sessionValue != null)
				{
					int.TryParse(sessionValue.ToString(), out tempValue);
				}
				thumbnailSize = tempValue;

				// Or was there a new value in the URL?
				if (CurrentMode.Size_Of_Thumbnails > -1)
				{
					HttpContext.Current.Session["Related_Images_ItemViewer:ThumbnailSize"] = CurrentMode.Size_Of_Thumbnails;
					thumbnailSize = CurrentMode.Size_Of_Thumbnails;
				}
			}

			// Now, reset the value in the navigation object, since we won't need to set it again
			CurrentMode.Size_Of_Thumbnails = -1;

		}

		/// <summary> Gets the flag that indicates if the page selector should be shown </summary>
		/// <value> This is a single page viewer, so this property always returns NONE</value>
		public override ItemViewer_PageSelector_Type_Enum Page_Selector
		{
			get
			{
				return ItemViewer_PageSelector_Type_Enum.PageLinks;
			}
		}


        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "MakeSpanFlashOnPageLoad();"));
        }
	}
}
