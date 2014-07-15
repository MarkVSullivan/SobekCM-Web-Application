#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Configuration;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	public class QC_ItemViewer : abstractItemViewer
	{
		private readonly string title;
		private int thumbnailsPerPage;
		private int thumbnailSize;

		private Dictionary<Page_TreeNode, Division_TreeNode> childToParent;


        /// <summary> Constructor for a new instance of the QC_ItemViewer class </summary>
		public QC_ItemViewer()
		{
			title = "Quality Control";

			 // See if there was a hidden request
			string hidden_request = HttpContext.Current.Request.Form["QC_behaviors_request"] ?? String.Empty;

			// If this was a cancel request do that
			if (hidden_request == "cancel")
			{
			   // currentMode.Mode = Display_Mode_Enum.Item_Display;
		  //      HttpContext.Current.Response.Redirect(CurrentMode.Redirect_URL(), true);
				
			//    return;
			}
			else if (hidden_request == "save")
			{
				//Save the QC form
			  //  HttpContext.Current.Response.Redirect(CurrentMode.Redirect_URL(), false);
				//return;
			}

			//Redirect 
		 //   HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
		}

		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Quality_Control"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.Quality_Control; }
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
		/// <remarks> If there are more than 100 related images, then the thumbnails are paged at 60 a page</remarks>
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
					  
				return ((PageCount > 1) && (CurrentMode.Page > 1)) ? CurrentMode.Redirect_URL("1qc")+queryString : String.Empty;
			}
		}

		/// <summary> Gets the url to go to the preivous page of thumbnails </summary>
		public override string Previous_Page_URL
		{
			get
			{
					 
				return ((PageCount > 1) && ( CurrentMode.Page > 1 )) ? CurrentMode.Redirect_URL( (CurrentMode.Page - 1).ToString() + "qc" ) : String.Empty;
			}
		}

		/// <summary> Gets the url to go to the next page of thumbnails </summary>
		public override string Next_Page_URL
		{
			get
			{
				int temp_page_count = PageCount;
				return  ( temp_page_count > 1 ) && (CurrentMode.Page < temp_page_count) ? CurrentMode.Redirect_URL( (CurrentMode.Page + 1).ToString() + "qc" ) :  String.Empty;
			}
		}

		/// <summary> Gets the url to go to the last page of thumbnails </summary>
		public override string Last_Page_URL
		{
			get
			{
				int temp_page_count = PageCount;
				return (temp_page_count > 1) && (CurrentMode.Page < temp_page_count) ? CurrentMode.Redirect_URL(temp_page_count.ToString() + "qc") : String.Empty;
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
					goToUrls.Add(CurrentMode.Redirect_URL(i + "qc"));
				}
				return goToUrls.ToArray();
			}
		}


  
		/// <summary> Adds the main view section to the page turner </summary>
		/// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("QC_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
			}

			int images_per_page = thumbnailsPerPage;
			int size_of_thumbnails = thumbnailSize;


			// Build the value
			StringBuilder builder = new StringBuilder(5000);

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;
			ushort current_view_page = CurrentMode.Page;

			builder.AppendLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new page, etc..) -->");
			builder.AppendLine("<input type=\"hidden\" id=\"QC_behaviors_request\" name=\"QC_behaviors_request\" value=\"\" />");

			// Start the citation table
			builder.AppendLine( "\t\t<!-- QUALITY CONTROL VIEWER OUTPUT -->" );
			if (CurrentItem.Web.Static_PageCount < 100)
			{
				builder.AppendLine("\t\t<td align=\"left\" height=\"40px\" ><span class=\"SobekViewerTitle\"><b>" + translator.Get_Translation(title, CurrentMode.Language) + "</b></span></td></tr>");
				builder.AppendLine("\t<tr>") ;
			}
			builder.AppendLine("\t\t<td>" );

			// Start the main div for the thumbnails
	
			ushort page = (ushort)(CurrentMode.Page - 1);
			if (page > (CurrentItem.Web.Static_PageCount - 1) / images_per_page)
				page = (ushort)((CurrentItem.Web.Static_PageCount - 1) / images_per_page);

			//Outer div which contains all the thumbnails
			builder.AppendLine("<div id=\"allThumbnailsOuterDiv1\" align=\"center\" style=\"margin:5px;\"><span id=\"allThumbnailsOuterDiv\" align=\"left\" style=\"float:left\" class=\"doNotSort\">");


			// Step through each page in the item
			Division_TreeNode lastParent = null;
			for (int page_index = page * images_per_page; (page_index < (page + 1) * images_per_page) && (page_index < CurrentItem.Web.Static_PageCount); page_index++)
			{
				Page_TreeNode thisPage = CurrentItem.Web.Pages_By_Sequence[page_index];
				Division_TreeNode thisParent = childToParent[thisPage];
				
				// Find the jpeg image
				foreach (SobekCM_File_Info thisFile in thisPage.Files.Where(thisFile => thisFile.System_Name.IndexOf(".jpg") > 0))
				{
					// Get the image URL
					CurrentMode.Page = (ushort) (page_index + 1);
					CurrentMode.ViewerCode = (page_index + 1).ToString();

					//set the image url to fetch the small thumbnail .thm image
					string image_url = (CurrentItem.Web.Source_URL + "/" + thisFile.System_Name.Replace(".jpg", "thm.jpg")).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

					//If thumbnail size selected is large, get the full-size jpg image
					if (size_of_thumbnails == 2 || size_of_thumbnails == 3 || size_of_thumbnails == 4)
						image_url = (CurrentItem.Web.Source_URL + "/" + thisFile.System_Name).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
					string url = CurrentMode.Redirect_URL().Replace("&", "&amp;").Replace("\"", "&quot;");

					// Start the box for this thumbnail
					builder.AppendLine("<span id=\"span" + page_index + "\" align=\"left\" style=\"display:inline-block;\" onmouseover=\"this.className='thumbnailHighlight'; showQcPageIcons(this.id); showErrorIcon(this.id);\" onmouseout=\"this.className='thumbnailNormal'; hideQcPageIcons(this.id); hideErrorIcon(this.id);\" >");
					builder.AppendLine("<div class=\"qcpage\" align=\"center\" id=\"parent" + image_url + "\" >");
					builder.AppendLine("<table>");

					// Add the name of the file
					builder.AppendLine("<tr><td class=\"qcfilename\" align=\"left\">" + thisFile.File_Name_Sans_Extension + "</td>");
					
					//Determine the error icon size based on the current thumbnail size 
					int error_icon_height = 20;
					int error_icon_width = 20;
					switch (size_of_thumbnails)
					{
						case 2:
							error_icon_height = 25;
							error_icon_width = 25;
							break;

						case 3:
							error_icon_height = 30;
							error_icon_width = 30;
							break;

						case 4:
							error_icon_height = 30;
							error_icon_width = 30;
							break;

						default:
							error_icon_height = 20;
							error_icon_height = 20;
							break;
					}
					//Add the error icon
					builder.AppendLine("<td><span id=\"error" + page_index + "\" class=\"errorIconSpan\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/Cancel.ico\" height=\"" + error_icon_height + "\" width=\"" + error_icon_width + "\" alt=\"Missing Icon Image\"></img></span></td></tr>");

					// Add the anchor for jumping to the file?
					builder.Append("<tr><td colspan=\"2\"><a id=\"" + page_index+ "\" href=\"" + url + "\" target=\"_blank\">");

					// Write the image and determine some values, based on current thumbnail size
					string division_text = "Division:";
					string pagination_text = "Pagination:";
					string division_box = "divisionbox_small";
					string pagination_box = "pagebox_small";
					int icon_width = 15;
					int icon_height = 15;
					int num_spaces = 1;
					switch (size_of_thumbnails)
					{
						case 2:
							builder.Append("<img id=\"child" + image_url + "\"  src=\"" + image_url + "\" width=\"315px\" height=\"50%\" alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\" />");
							division_box = "divisionbox_medium";
							pagination_box = "pagebox_medium";
							icon_width = 20;
							icon_height = 20;
							num_spaces = 3;
							break;

						case 3:
							builder.AppendLine("<img id=\"child" + image_url + "\" src=\"" + image_url + "\" width=\"472.5px\" height=\"75%\" alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\" />");
							division_box = "divisionbox_large";
							pagination_box = "pagebox_large";
							icon_width = 20;
							icon_height = 20;
							num_spaces = 4;
							break;

						case 4:
							builder.AppendLine("<img id=\"child" + image_url + "\" src=\"" + image_url + "\"  alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\" />");
							division_box = "divisionbox_full";
							pagination_box = "pagebox_full";
							icon_width = 25;
							icon_height = 25;
							num_spaces = 4;
							break;

						default:
							builder.AppendLine("<img src=\"" + image_url + "\" alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\" />");
							division_box = "divisionbox_small";
							pagination_box = "pagebox_small";
							division_text = "D:";
							pagination_text = "Page:";
							break;
					}
					builder.AppendLine("</a></td></tr>");

					// Add the text box for entering the name of this page
					builder.AppendLine("<tr><td class=\"paginationtext\" align=\"left\">" + pagination_text + "</td>");
					builder.AppendLine("<td><input type=\"text\" id=\"textbox"+page_index+"\" class=\"" + pagination_box + "\" value=\"" + thisPage.Label + "\" onchange=\"PaginationTextChanged(this.id,0,"+CurrentItem.Web.Static_PageCount+");\"></input></td></tr>");

					// Was this a new parent?
					bool newParent = thisParent != lastParent;

					// Add the Division prompting, and the check box for a new division
					builder.Append("<tr><td class=\"divisiontext\" align=\"left\">" + division_text);
					builder.Append("<input type=\"checkbox\" id=\"newDivType" + page_index + "\" name=\"newdiv" + page_index + "\" value=\"new\" onclick=\"UpdateDivDropdown(this.name, " + CurrentItem.Web.Static_PageCount + ");\"");
					if ( newParent )
						builder.Append(" checked=\"checked\"");
					builder.AppendLine("/></td>");

					// Determine the text for the parent
					string parentLabel = String.Empty;
					if (thisParent != null)
					{
						parentLabel = thisParent.Display_Label;
						//if (parentLabel.Length == 0)
						//    parentLabel = thisParent.Type;
					}

					// Add the division box
				 //   builder.AppendLine("<td><input type=\"text\" class=\"" + division_box + "\" value=\"" + parentLabel + "\"></input></td></tr>");
					if(newParent)
						builder.AppendLine("<td><select id=\"selectDivType" + page_index + "\" class=\"" + division_box + "\" onchange=\"DivisionTypeChanged(this.id,"+CurrentItem.Web.Static_PageCount+");\">");
					else
					{
						builder.AppendLine("<td><select id=\"selectDivType" + page_index + "\" class=\"" + division_box + "\" disabled=\"disabled\" onchange=\"DivisionTypeChanged(this.id,"+CurrentItem.Web.Static_PageCount+");\">");
					}
					
					//Read the Division_Types_Errors.xml file to get the select options
					string divisionTypesErrorsFile = CurrentMode.Base_URL + "config/Division_Types_Errors.xml";
					List<string> divOptions = new List<string>();
					DataSet divList = new DataSet();
					divList.ReadXml(divisionTypesErrorsFile);
					
					foreach (DataRow row in divList.Tables[1].Rows)
					{
						divOptions.Add(row[1].ToString());
					}


					foreach (string divoption in divOptions)
				  {  
						if(divoption==parentLabel)
							builder.AppendLine("<option value=\"" + divoption + "\" selected=\"selected\">" + divoption + "</option>");
						else
							builder.AppendLine("<option value=\"" + divoption + "\">" + divoption + "</option>");
				  }

				
					builder.AppendLine("</select></td></tr>");

					//Add the on-hover options span for the page thumbnail
					builder.AppendLine("<tr><td colspan=\"100%\">");
					builder.AppendLine("<span id=\"qcPageOptions"+page_index+"\" class=\"qcPageOptionsSpan\" style=\"float:right\"><img src=\""+CurrentMode.Base_URL+"default/images/ToolboxImages/Main_Information.ICO\" height=\""+icon_height+"\" width=\""+icon_width+"\" alt=\"Missing Icon Image\"></img>");
					
					//Add spaces between icons based on the current thumbnail size 
					for (int i = 0; i < num_spaces; i++) { builder.AppendLine("&nbsp;"); }
					builder.AppendLine("<a href=\"" + url + "\" target=\"_blank\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/View.ico\" height=\"" + icon_height + "\" width=\"" + icon_width + "\" alt=\"Missing Icon Image\"></img></a>");
					for (int i = 0; i < num_spaces; i++) { builder.AppendLine("&nbsp;"); }
					builder.AppendLine("<img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/TRASH01.ICO\" height=\"" + icon_height + "\" width=\"" + icon_width + "\" alt=\"Missing Icon Image\"></img>");
					for (int i = 0; i < num_spaces; i++) { builder.AppendLine("&nbsp;"); }
					builder.AppendLine("<img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT02.ICO\" height=\"" + icon_height + "\" width=\"" + icon_width + "\" alt=\"Missing Icon Image\"></img>");
					for (int i = 0; i < num_spaces; i++) { builder.AppendLine("&nbsp;"); }
					builder.AppendLine("<img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT04.ICO\" height=\"" + icon_height + "\" width=\"" + icon_width + "\" alt=\"Missing Icon Image\"></img>");
					builder.AppendLine("</span>");
					builder.AppendLine("</tr></td>");

					// Finish this one division
					builder.AppendLine("</table></div></span>");
					builder.AppendLine();
					break;
				}

				// Save the last parent
				lastParent = thisParent;

			}


			//Close the outer div
			builder.AppendLine("</span></div>");

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;
			CurrentMode.Page = current_view_page;

			// Finish the citation table
			builder.AppendLine("\t\t</td>");
			builder.AppendLine("\t\t<!-- END QUALITY CONTROL VIEWER OUTPUT -->");

			//If the current url has an anchor, call the javascript function to animate the corresponding span background color
			//builder.AppendLine("<script type=\"text/javascript\">window.onload=MakeSpanFlashOnPageLoad();</script>");
			builder.AppendLine("<script type=\"text/javascript\">window.onload=MakeSortable1();</script>");
			builder.AppendLine("<script type=\"text/javascript\"> WindowResizeActions();</script>");

			//Add the Complete and Cancel buttons at the end of the form
			builder.AppendLine("</tr><tr><td colspan=\"100%\" style=\"float:right\">");
			builder.AppendLine("<button type=\"button\" onclick=\"behaviors_save_form();\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/check.ico\" width=\"25\" height=\"25\"/>Complete</button>");
			builder.AppendLine("<button type=\"button\" onclick=\"behaviors_cancel_form();\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/Cancel.ico\" width=\"25\" height=\"25\" />Cancel</button>");
			builder.AppendLine("</td></tr>");

			// Add the build HTML for all the images
			Literal mainLiteral = new Literal {Text = builder.ToString()};
			placeHolder.Controls.Add( mainLiteral );

		}
	   
		/// <summary>
		/// Add the Viewer specific information to the top navigation row
		/// This nav row adds the different thumbnail viewing options(# of thumbnails, size of thumbnails, list of all related item thumbnails)
		/// </summary>
		public override string NavigationRow
		{
			get
			{
				string Num_Of_Thumbnails = "Thumbnails per page";
				string Size_Of_Thumbnail = "Thumbnail size";
				string Go_To_Thumbnail = "Go to thumbnail";

				if (CurrentMode.Language == Web_Language_Enum.French)
				{
					Num_Of_Thumbnails = "Vignettes par page";
					Size_Of_Thumbnail = "la taille des vignettes";
					Go_To_Thumbnail = "Aller à l'Vignette";
				}

				if (CurrentMode.Language == Web_Language_Enum.Spanish)
				{
					Num_Of_Thumbnails = "Miniaturas por página";
					Size_Of_Thumbnail = "Miniatura de tamaño";
					Go_To_Thumbnail = "Ir a la miniatura";
				}


				//Set the number of thumbnails to show per page
				int thumbnails_per_page;
				int size_of_thumbnails;

				if (CurrentItem.Web.Static_PageCount > 100)
					thumbnails_per_page = 50;
				else
				{
					thumbnails_per_page = CurrentItem.Web.Static_PageCount;
				}
				//Set the thumbnails_per_page to the value from the query string, if present
				Uri uri = HttpContext.Current.Request.Url;

				if (uri.Query.IndexOf("ts") > 0)
				{
					size_of_thumbnails = Convert.ToInt32(HttpUtility.ParseQueryString(uri.Query).Get("nt"));
					CurrentMode.Size_Of_Thumbnails = (short) size_of_thumbnails;
				}
				else
				{
					CurrentMode.Size_Of_Thumbnails = -1;
				}

				// Build the value
				StringBuilder navRowBuilder = new StringBuilder(5000);

				//Start building the top nav bar
				navRowBuilder.AppendLine("\t\t<!-- QUALITY CONTROL VIEWER TOP NAV ROW -->");

	//			navRowBuilder.AppendLine("<div id=\"itemviewersbar\" style=\"text-align: center\">");
	  //          navRowBuilder.AppendLine("\t<ul class=\"sf-menu\">");

				//Include the js files
				navRowBuilder.AppendLine("<script language=\"JavaScript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_related_items.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery-1.9.1.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery-ui-1.10.1.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery.color-2.1.1.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_qc.js\"></script>");
				
				 navRowBuilder.AppendLine("<link rel=\"stylesheet\" href=\"http://code.jquery.com/ui/1.10.1/themes/base/jquery-ui.css\" />");
			  navRowBuilder.AppendLine("<script src=\"http://code.jquery.com/jquery-1.9.1.js\"></script>");
				  navRowBuilder.AppendLine("<script src=\"http://code.jquery.com/ui/1.10.1/jquery-ui.js\"></script>");
				
				navRowBuilder.AppendLine("<table width=\"100%\"><tr align=\"center\">");
				

				//Add the dropdown for the number of thumbnails per page, only if there are >25 thumbnails
				if (CurrentItem.Web.Static_PageCount > 25)
				{
					//Redirect to the first page of results when the number of thumbnails option is changed by the user
					string current_viewercode = CurrentMode.ViewerCode;
					CurrentMode.ViewerCode = "1qc";
					string current_Page_url = CurrentMode.Redirect_URL("1qc");

					//   CurrentMode.Thumbnails_Per_Page = -1;
					//  string current_Page_url = CurrentMode.Redirect_URL("1qc");

					// Collect the list of options to display
					List<int> thumbsOptions = new List<int>();
					thumbsOptions.Add(25);
					if (CurrentItem.Web.Static_PageCount > 50) thumbsOptions.Add(50);
					if (CurrentItem.Web.Static_PageCount > 100) thumbsOptions.Add(100);
					if (CurrentItem.Web.Static_PageCount > 250) thumbsOptions.Add(250);
					if (CurrentItem.Web.Static_PageCount > 500) thumbsOptions.Add(500);

					// Start the drop down select list 
					navRowBuilder.AppendLine("<td valign=\"top\">");
					navRowBuilder.AppendLine("<span><select id=\"selectNumOfThumbnails\" onchange=\"location=this.options[this.selectedIndex].value;\">");
					//   navRowBuilder.AppendLine("<br/><br/><span><select id=\"selectNumOfThumbnails\" onchange=current_Page_url>");

					// Step through all the options
					foreach (int thumbOption in thumbsOptions)
					{
						CurrentMode.Thumbnails_Per_Page = (short) thumbOption;
						if (thumbnailsPerPage == thumbOption)
						{
							navRowBuilder.AppendLine("<option value=\"" + CurrentMode.Redirect_URL() + "\" selected=\"selected\">" + thumbOption + " thumbnails per page</option>");
						}
						else
						{

							navRowBuilder.AppendLine("<option value=\"" + CurrentMode.Redirect_URL() + "\">" + thumbOption + " thumbnails per page</option>");
						}
					}

					CurrentMode.Thumbnails_Per_Page = -1;
					if (thumbnails_per_page == int.MaxValue)
					{
						navRowBuilder.AppendLine("<option value=\"" + CurrentMode.Redirect_URL() + "\" selected=\"selected\">All thumbnails</option>");
					}
					else
					{
						navRowBuilder.AppendLine("<option value=\"" + CurrentMode.Redirect_URL() + "\">All thumbnails</option>");
					}

					//Reset the Current Mode Thumbnails_Per_Page

					CurrentMode.ViewerCode = current_viewercode;
					navRowBuilder.AppendLine("</select></span>");
					navRowBuilder.AppendLine("</td>");

				}
				CurrentMode.Thumbnails_Per_Page = -100;


				//Add the control for the thumbnail size

				//Get the icons for the thumbnail sizes
				string image_location = CurrentMode.Default_Images_URL;
				


				navRowBuilder.AppendLine("<td valign=\"top\">");
				if (thumbnailSize == 1)
					navRowBuilder.Append("<a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "thumbs3.gif\"/></a>");
				else
				{
					CurrentMode.Size_Of_Thumbnails = 1;
					navRowBuilder.Append("<a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "thumbs3.gif\"/></a>");
				}
				if (thumbnailSize == 2)
					navRowBuilder.Append("<a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "thumbs2.gif\"/></a>");
				else
				{
					CurrentMode.Size_Of_Thumbnails = 2;
					navRowBuilder.Append("<a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "thumbs2.gif\"/></a>");
				}
				if (thumbnailSize == 3)
					navRowBuilder.Append("<a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "thumbs1.gif\"/></a>");
				else
				{
					CurrentMode.Size_Of_Thumbnails = 3;
					navRowBuilder.Append("<a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "thumbs1.gif\"/></a>");
				}
				//Reset the current mode
				CurrentMode.Size_Of_Thumbnails = -1;
				navRowBuilder.AppendLine("</td>");


				//Add the dropdown for the thumbnail anchor within the page to directly navigate to
				navRowBuilder.AppendLine("<td valign=\"top\">");
				navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
				navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Go_To_Thumbnail + ":</b>");
				navRowBuilder.AppendLine("<span><select id=\"selectGoToThumbnail\" onchange=\"location=this.options[this.selectedIndex].value; AddAnchorDivEffect(this.options[this.selectedIndex].value);\" >");

				//iterate through the page items
				if (CurrentItem.Web.Static_PageCount > 0)
				{
					int thumbnail_count = 0;
					foreach (Page_TreeNode thisFile in CurrentItem.Web.Pages_By_Sequence)
					{
						thumbnail_count++;

						string current_Page_url1 = CurrentMode.Redirect_URL((thumbnail_count/thumbnails_per_page + (thumbnail_count%thumbnails_per_page == 0 ? 0 : 1)).ToString() + "qc");

						navRowBuilder.AppendLine("<option value=\"" + current_Page_url1 + "#" + thisFile.Label + "\">" + thisFile.Label + "</option>");

					}
				}
				navRowBuilder.AppendLine("</select></span>");
			 //   navRowBuilder.AppendLine("<br /><br />");

				//Add the nav row QC image icons
                navRowBuilder.AppendLine("<span id=\"qcIconsTopNavRow\" class=\"spanQCIconsTopNavRow\">");
                navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
                navRowBuilder.AppendLine("<span><img src=\"" + image_location + "ToolboxImages/Save.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing image\"></img></span>");
                navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
                navRowBuilder.AppendLine("<span><img src=\"" + image_location + "ToolboxImages/thumbnail_large.gif" + "\" height=\"20\" width=\"20\" alt=\"Missing image\"></img></span>");
                navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
                navRowBuilder.AppendLine("<span><img src=\"" + image_location + "ToolboxImages/prev_Error.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing image\"></img></span>");
                navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
                navRowBuilder.AppendLine("<span><img src=\"" + image_location + "ToolboxImages/next_error.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing image\"></img></span>");
                navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
                navRowBuilder.AppendLine("<span><img src=\"" + image_location + "ToolboxImages/mets.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing image\"></img></span>");
                navRowBuilder.AppendLine("</span>");

				navRowBuilder.AppendLine("</td></tr></table>");
			
				// Finish the nav row controls
				navRowBuilder.AppendLine("\t\t<!-- END QUALITY CONTROL VIEWER NAV ROW -->");

				// Return the html string
				return navRowBuilder.ToString();

			}
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
				Tracer.Add_Trace("QC_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
			 }

		   return false;
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
				thumbnailsPerPage = CurrentUser.Get_Option("QC_ItemViewer:ThumbnailsPerPage", 50);

				// Or was there a new value in the URL?
				if (CurrentMode.Thumbnails_Per_Page >= -1)
				{
					CurrentUser.Add_Option("QC_ItemViewer:ThumbnailsPerPage", CurrentMode.Thumbnails_Per_Page);
					thumbnailsPerPage = CurrentMode.Thumbnails_Per_Page;
				}
			}
			else
			{
				int tempValue = 50;
				object sessionValue = HttpContext.Current.Session["QC_ItemViewer:ThumbnailsPerPage"];
				if (sessionValue != null)
				{
					int.TryParse(sessionValue.ToString(), out tempValue);
				}
				thumbnailsPerPage = tempValue;

				// Or was there a new value in the URL?
				if (CurrentMode.Thumbnails_Per_Page >= -1)
				{
					HttpContext.Current.Session["QC_ItemViewer:ThumbnailsPerPage"] = CurrentMode.Thumbnails_Per_Page;
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
				thumbnailSize = CurrentUser.Get_Option("QC_ItemViewer:ThumbnailSize", 1);

				// Or was there a new value in the URL?
				if (CurrentMode.Size_Of_Thumbnails > -1)
				{
					CurrentUser.Add_Option("QC_ItemViewer:ThumbnailSize", CurrentMode.Size_Of_Thumbnails);
					thumbnailSize = CurrentMode.Size_Of_Thumbnails;
				}
			}
			else
			{
				int tempValue = 1;
				object sessionValue = HttpContext.Current.Session["QC_ItemViewer:ThumbnailSize"];
				if (sessionValue != null)
				{
					int.TryParse(sessionValue.ToString(), out tempValue);
				}
				thumbnailSize = tempValue;

				// Or was there a new value in the URL?
				if (CurrentMode.Size_Of_Thumbnails > -1)
				{
					HttpContext.Current.Session["QC_ItemViewer:ThumbnailSize"] = CurrentMode.Size_Of_Thumbnails;
					thumbnailSize = CurrentMode.Size_Of_Thumbnails;
				}
			}

			// Now, reset the value in the navigation object, since we won't need to set it again
			CurrentMode.Size_Of_Thumbnails = -1;

			// Now, build a list from child node to parent node
			childToParent = new Dictionary<Page_TreeNode, Division_TreeNode>();
			foreach (abstract_TreeNode rootNode in CurrentItem.Divisions.Physical_Tree.Roots)
			{
				if (!rootNode.Page)
				{
					recurse_through_and_find_child_parent_relationship( (Division_TreeNode) rootNode);
				}
			}

		}

		private void recurse_through_and_find_child_parent_relationship( Division_TreeNode parentNode )
		{
			foreach (abstract_TreeNode childNode in parentNode.Nodes)
			{
				if (childNode.Page)
				{
					childToParent[(Page_TreeNode) childNode] = parentNode;
				}
				else
				{
					recurse_through_and_find_child_parent_relationship((Division_TreeNode) childNode);
				}
			}
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
	}
}
