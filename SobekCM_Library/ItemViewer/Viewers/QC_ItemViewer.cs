#region Using directives

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Configuration;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using System.Xml;



#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	public class QC_ItemViewer : abstractItemViewer
	{
		private readonly string title;
		private int thumbnailsPerPage;
		private int thumbnailSize;
        private int currPageNumber;
		private string hidden_request;
		private string hidden_main_thumbnail;
		private bool autosave_option;

		private Dictionary<Page_TreeNode, Division_TreeNode> childToParent;
		private QualityControl_Profile qc_profile;

		/// <summary> Constructor for a new instance of the QC_ItemViewer class </summary>
		public QC_ItemViewer( SobekCM_Item Current_Object )
		{
			// Get the default QC profile
			qc_profile = QualityControl_Configuration.Default_Profile;

			title = "Quality Control";

            
			 // See if there was a hidden request
			hidden_request = HttpContext.Current.Request.Form["QC_behaviors_request"] ?? String.Empty;
			hidden_main_thumbnail = HttpContext.Current.Request.Form["Main_Thumbnail_Index"] ?? String.Empty;

			try
			{
				bool autosaveCacheValue=true;
				bool autosaveCache = false;
				
				//Conversion result of autosaveCacheValue(conversion successful or not) saved in autosaveCache
				   
				if (HttpContext.Current.Cache.Get("autosave_option")!=null)
				   autosaveCache=bool.TryParse(HttpContext.Current.Cache.Get("autosave_option").ToString(), out autosaveCacheValue);
				bool convert = bool.TryParse(HttpContext.Current.Request.Form["Autosave_Option"], out autosave_option);
				if (!convert && !autosaveCache)
				{
					autosave_option = true;
				}
				else if (!convert && autosaveCache)
				{
					autosave_option = autosaveCacheValue;
				}

				else
				{
					HttpContext.Current.Cache.Insert("autosave_option", (object)autosave_option);
				}
			}
			catch (Exception e)
			{
				bool error = true;
			}

			//If a main thumbnail value was selected (either for assignment or removal)
			if (!String.IsNullOrEmpty(hidden_main_thumbnail) && (hidden_request=="pick_main_thumbnail" || hidden_request=="unpick_main_thumbnail"))
			{
				// Save the selected page index to the cache if request_type is "pick"
				if (hidden_request == "pick_main_thumbnail")
					HttpContext.Current.Cache.Insert("main_thumbnail_index", (object)hidden_main_thumbnail);

				//else clear the cached value if request_type is "unpick"
				else if(hidden_request=="unpick_main_thumbnail")
				{
					HttpContext.Current.Cache.Remove("main_thumbnail_index");
				}

				//Save the request type as well
				HttpContext.Current.Cache.Insert("main_thumbnail_action", (object) hidden_request);

				string url_redirect = HttpContext.Current.Request.Url.ToString();
				HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl.ToString());

			}
			else
			{
				//Get the main thumbnail index value from the cache if present
				if (HttpContext.Current.Cache.Get("main_thumbnail_index")!=null)
				{
					hidden_main_thumbnail = HttpContext.Current.Cache.Get("main_thumbnail_index").ToString();

				}
			}

			// If this was a cancel request do that
			if (hidden_request == "cancel")
			{
				HttpContext.Current.Response.Redirect(CurrentMode.Redirect_URL());
			}
			else if (hidden_request == "save")
			{
				// First, build a dictionary of all the pages
				Dictionary<string, Page_TreeNode> pages_by_name = new Dictionary<string, Page_TreeNode>();
				foreach (Page_TreeNode thisPage in Current_Object.Web.Pages_By_Sequence)
				{
					if ( thisPage.Files.Count > 0 )
					{
						pages_by_name[thisPage.Files[0].File_Name_Sans_Extension] = thisPage;
					}
				}

				// Need to clear the divisions completely here (rebuilding from scratch)
				// NOTE: There can be dmdSecs and amdSecs related to the divisions, which we would have
				//       to do extra work to retain.
				Current_Object.Divisions.Physical_Tree.Clear();
				Current_Object.Web.Clear_Pages_By_Sequence();
				Current_Object.Web.Static_Division_Count = 0;

				// We need to keep track of the current division ( since a division can have multiple pages)
				Division_TreeNode currDivision = null;

				try
				{
					// Now, step through each of the pages in the return
					string[] keysFromForm = HttpContext.Current.Request.Form.AllKeys;
					foreach (string thisKey in keysFromForm)
					{
						// Has this gotten to the next page?
						if ((thisKey.IndexOf("filename") == 0) && (thisKey.Length > 8))
						{
							// Get the current page
							string thisValue = HttpContext.Current.Request.Form[thisKey];
							Page_TreeNode currPage = pages_by_name[thisValue];

							// Ensure we got a page
							if (currPage != null)
							{
								// Get the index to use for all the other keys
								string thisIndex = thisKey.Substring(8);

								// Get the page name 
								string pagename = HttpContext.Current.Request.Form["textbox" + thisIndex];
								currPage.Label = pagename;

								// Is this a new division?
								if (HttpContext.Current.Request.Form["newdiv" + thisIndex] != null )
								{
									// Get the new division type/label
									string divisionType = HttpContext.Current.Request.Form["selectDivType" + thisIndex].Trim().Replace("!","");
									string divisionLabel = String.Empty;
									if (HttpContext.Current.Request.Form["txtDivName" + thisIndex] != null)
										divisionLabel = HttpContext.Current.Request.Form["txtDivName" + thisIndex].Trim();
									if (divisionType.Length == 0)
										divisionType = "Chapter";

									// Get the division config, based on the division type
									QualityControl_Division_Config divInfo = qc_profile[divisionType];
									if (divInfo.BaseTypeName.Length > 0)
									{
										divisionLabel = divisionType;
										divisionType = divInfo.BaseTypeName;
									}


									// Create the new division and add to the package
									currDivision = new Division_TreeNode(divisionType, divisionLabel);
									Current_Object.Divisions.Physical_Tree.Roots.Add(currDivision);
									Current_Object.Web.Static_Division_Count = Current_Object.Web.Static_Division_Count + 1;
								}

								// Add this page to the division
								currDivision.Add_Child(currPage);

								// Also add the page to the page-by-sequence values in the special web shortcut area
								Current_Object.Web.Add_Pages_By_Sequence(currPage);
							}
						}
					}
				}
				catch (Exception ee)
				{
					bool error = true;
				}


				// Save the item to the cache
				SobekCM.Library.MemoryMgmt.Cached_Data_Manager.Store_Digital_Resource_Object(Current_Object.BibID, Current_Object.VID, Current_Object, null);


				string url_redirect = HttpContext.Current.Request.Url.ToString();
				HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl.ToString());

			}

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

        private void add_main_menu(StringBuilder builder)
        {
            //StringBuilder builder = new StringBuilder(4000);
            builder.AppendLine("<div id=\"qcmenubar\">");
            builder.AppendLine("<ul class=\"qc-menu\">");

            builder.AppendLine("<li>Resource<ul>");
            builder.AppendLine("\t<li>View METS</li>");
            builder.AppendLine("\t<li>View Directory</li>");
            builder.AppendLine("\t<li>View QC History</li>");
            builder.AppendLine("\t<li>Volume Error<ul>");
            builder.AppendLine("\t\t<li>No volume level error</li>");
            builder.AppendLine("\t\t<li>Invalid images</li>");
            builder.AppendLine("\t\t<li>Incorrect volume/title</li>");
            builder.AppendLine("\t</ul></li>");
            builder.AppendLine("\t<li>Save</li>");
            builder.AppendLine("\t<li>Complete</li>");
            builder.AppendLine("\t<li>Cancel</li>");
            builder.AppendLine("</ul></li>");

            builder.AppendLine("<li>Edit<ul>");
            builder.AppendLine("\t<li>Clear Pagination</li>");
            builder.AppendLine("\t<li>Clear All &amp; Reorder Pages</li>");
            builder.AppendLine("\t<li>Automatic Numbering<ul>");
            builder.AppendLine("\t\t<li>No automatic numbering</li>");
            builder.AppendLine("\t\t<li>Within same division</li>");
            builder.AppendLine("\t\t<li>Entire document</li>");
            builder.AppendLine("\t</ul></li>");
            builder.AppendLine("</ul></li>");

            builder.AppendLine("<li>View<ul>");
            builder.AppendLine("\t<li>Thumbnail Size<ul>");
            builder.AppendLine("\t\t<li>Small</li>");
            builder.AppendLine("\t\t<li>Medium</li>");
            builder.AppendLine("\t\t<li>Large</li>");
            builder.AppendLine("\t</ul></li>");
            builder.AppendLine("\t<li>Thumbnails per page<ul>");
            builder.AppendLine("\t\t<li>25</li>");
            builder.AppendLine("\t\t<li>50</li>");
            builder.AppendLine("\t\t<li>All thumbnails</li>");
            builder.AppendLine("\t</ul></li>");
            builder.AppendLine("</ul></li>");

            builder.AppendLine("<li>Help</li>");

            builder.AppendLine("</ul>");
            builder.AppendLine("</div>");

            builder.AppendLine("<script>");
            builder.AppendLine("    jQuery(document).ready(function(){");
            builder.AppendLine("       jQuery('ul.qc-menu').superfish();");
            builder.AppendLine("    });");
            builder.AppendLine("</script>");
            builder.AppendLine();

            //  Literal htmlLiteral = new Literal();
            // htmlLiteral.Text = builder.ToString();

            // placeHolder.Controls.Add(htmlLiteral);
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

            // Get the links for the METS
            string greenstoneLocation = CurrentItem.Web.Source_URL + "/";
            string complete_mets = greenstoneLocation + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((CurrentItem.Behaviors.Dark_Flag) || (CurrentItem.Behaviors.IP_Restriction_Membership > 0))
            {
                complete_mets = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";
            }


			// Build the value
			StringBuilder builder = new StringBuilder(5000);

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;
			ushort current_view_page = CurrentMode.Page;


			builder.AppendLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new page, etc..) -->");
			builder.AppendLine("<input type=\"hidden\" id=\"QC_behaviors_request\" name=\"QC_behaviors_request\" value=\"\" />");
			builder.AppendLine("<input type=\"hidden\" id=\"Main_Thumbnail_Index\" name=\"Main_Thumbnail_Index\" value=\"\" />");
			builder.AppendLine("<input type=\"hidden\" id=\"Autosave_Option\" name=\"Autosave_Option\" value=\"\" />");
  
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
					builder.AppendLine("<span id=\"span" + page_index + "\" onclick=\"PickMainThumbnail(this.id);\" align=\"left\" style=\"display:inline-block;\" onmouseover=\"this.className='thumbnailHighlight'; showQcPageIcons(this.id); showErrorIcon(this.id);\" onmouseout=\"this.className='thumbnailNormal'; hideQcPageIcons(this.id); hideErrorIcon(this.id);\" >");
					builder.AppendLine("<div class=\"qcpage\" align=\"center\" id=\"parent" + image_url + "\" >");
					builder.AppendLine("<table>");

					// Add the name of the file
					builder.AppendLine("<tr><td class=\"qcfilename\" align=\"left\"><input type=\"hidden\" id=\"filename" + page_index + "\" name=\"filename" + page_index + "\" value=\"" + thisFile.File_Name_Sans_Extension + "\" />" + thisFile.File_Name_Sans_Extension + "</td>");
					                  
					//Determine the error icon size, main-thumbnail-selected icon size based on the current thumbnail size 
					int error_icon_height = 20;
					int error_icon_width = 20;
					int pick_main_thumbnail_height = 20;
					int pick_main_thumbnail_width = 20;
					switch (size_of_thumbnails)
					{
						case 2:
							error_icon_height = 25;
							error_icon_width = 25;
							pick_main_thumbnail_height = 25;
							pick_main_thumbnail_width = 25;
							break;

						case 3:
							error_icon_height = 30;
							error_icon_width = 30;
							pick_main_thumbnail_height = 30;
							pick_main_thumbnail_width = 30;
							break;

						case 4:
							error_icon_height = 30;
							error_icon_width = 30;
							pick_main_thumbnail_height = 30;
							pick_main_thumbnail_width = 30;
							break;

						default:
							error_icon_height = 20;
							error_icon_height = 20;
							pick_main_thumbnail_height = 20;
							pick_main_thumbnail_width = 20;
							break;
					}

                    //Add the checkbox for moving this thumbnail
                    builder.AppendLine("<td><span class=\"chkMoveThumbnailHidden\"><input type=\"checkbox\" id=\"chkMoveThumbnail" + page_index + "\" name=\"chkMoveThumbnail" + page_index + "\" class=\"chkMoveThumbnailHidden\" onchange=\"chkMoveThumbnailChanged(this.id, "+CurrentItem.Web.Static_PageCount+")\"/></span>");


					//Add the error icon
					builder.AppendLine("<span id=\"error" + page_index + "\" class=\"errorIconSpan\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/Cancel.ico\" height=\"" + error_icon_height + "\" width=\"" + error_icon_width + "\" alt=\"Missing Icon Image\"></img></span>");
					int main_thumbnail_index = -1;
					if (!String.IsNullOrEmpty(hidden_main_thumbnail))
						Int32.TryParse(hidden_main_thumbnail, out main_thumbnail_index);
					//Add the pick_main_thumbnail icon
					if (main_thumbnail_index >= 0 && main_thumbnail_index == page_index && hidden_request != "unpick_main_thumbnail")
						builder.AppendLine("<span id=\"spanImg" + page_index + "\" class=\"pickMainThumbnailIconSelected\"><img id=\"pick_main_thumbnail" + page_index + "\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/thumbnail_large.gif\" height=" + pick_main_thumbnail_height + " width=" + pick_main_thumbnail_width + "/></span></td></tr>");
					else
						builder.AppendLine("<span id=\"spanImg" + page_index + "\" class=\"pickMainThumbnailIcon\"><img id=\"pick_main_thumbnail" + page_index + "\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/thumbnail_large.gif\" height=" + pick_main_thumbnail_height + " width=" + pick_main_thumbnail_width + "/></span></td></tr>");

					// Add the anchor for jumping to the file?
					builder.Append("<tr><td colspan=\"2\"><a id=\"" + page_index+ "\" href=\"" + url + "\" target=\"_blank\">");

					// Write the image and determine some values, based on current thumbnail size
					string division_text = "Division:";
					string pagination_text = "Pagination:";
					string division_name_text = "Name:";
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
							builder.AppendLine("<img  src=\"" + image_url + "\" alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\"/>");
							division_box = "divisionbox_small";
							pagination_box = "pagebox_small";
							division_text = "D:";
							pagination_text = "Page:";
							break;
					}
								   
					builder.AppendLine("</a></td></tr>");

					// Add the text box for entering the name of this page
					builder.AppendLine("<tr><td class=\"paginationtext\" align=\"left\">" + pagination_text + "</td>");
                    builder.AppendLine("<td><input type=\"text\" id=\"textbox" + page_index + "\" name=\"textbox" + page_index + "\" class=\"" + pagination_box + "\" value=\"" + thisPage.Label + "\" onchange=\"PaginationTextChanged(this.id,0," + CurrentItem.Web.Static_PageCount + ");\"></input></td></tr>");

					// Was this a new parent?
					bool newParent = thisParent != lastParent;

					// Add the Division prompting, and the check box for a new division
					builder.Append("<tr><td class=\"divisiontext\" align=\"left\">" + division_text);
                    builder.Append("<input type=\"checkbox\" id=\"newDivType" + page_index + "\" name=\"newdiv" + page_index + "\" value=\"new\" onclick=\"UpdateDivDropdown(this.name, " +  CurrentItem.Web.Static_PageCount + ");\"");
					if ( newParent )
						builder.Append(" checked=\"checked\"");
					builder.AppendLine("/></td>");

					// Determine the text for the parent
					string parentLabel = String.Empty;
					string parentType = "Chapter";
					if (thisParent != null)
					{
						parentLabel = thisParent.Label;
						parentType = thisParent.Type;

					}

					// Add the division box
	       			if(newParent)
                        builder.AppendLine("<td><select id=\"selectDivType" + page_index + "\" name=\"selectDivType" + page_index + "\" class=\"" + division_box + "\" onchange=\"DivisionTypeChanged(this.id," + CurrentItem.Web.Static_PageCount + ");\">");
					else
					{
                        builder.AppendLine("<td><select id=\"selectDivType" + page_index + "\" name=\"selectDivType" + page_index + "\" class=\"" + division_box + "\" disabled=\"disabled\" onchange=\"DivisionTypeChanged(this.id," + CurrentItem.Web.Static_PageCount + ");\">");
					}

					string txtDivNameCssClass = "txtNamedDivHidden";
                    //Add the division types fromt he current QC Config profile to a local dictionary
                    Dictionary<string, bool> qcDivisionList = new Dictionary<string, bool>();
                    foreach (QualityControl_Division_Config qcDivConfig in qc_profile.All_Division_Types)
                    {
                        qcDivisionList.Add(qcDivConfig.TypeName,qcDivConfig.isNameable);
                    }

                    //Get the division types from the Page Tree (from this METS), and the extra ones not in the profile to the 
                    foreach (KeyValuePair<Page_TreeNode, Division_TreeNode> node in childToParent)
                    {
                        string type = node.Value.Type;
                        string label = node.Value.Label;
                        if (!qcDivisionList.ContainsKey(type))
                        {
                            bool isNameable = (String.IsNullOrEmpty(label)) ? false : true;
                            qcDivisionList.Add(type, isNameable);
                        }
                    }


                    //Iterate through all the division types in this profile
                    foreach (KeyValuePair<string, bool> divisionType in qcDivisionList)
                    {
                        if (divisionType.Key == parentType && divisionType.Value == false)
                            builder.AppendLine("<option value=\"" + divisionType.Key + "\" selected=\"selected\">" + divisionType.Key + "</option>");
                        else if (divisionType.Key == parentType && divisionType.Value == true)
                        {
                            builder.AppendLine("<option value=\"!" + divisionType.Key + "\" selected=\"selected\">" + divisionType.Key + "</option>");
                            txtDivNameCssClass = "txtNamedDivVisible";
                        }
                        else if (divisionType.Value == true)
                            builder.AppendLine("<option value=\"!" + divisionType.Key + "\">" + divisionType.Key + "</option>");
                        else
                            builder.AppendLine("<option value=\"" + divisionType.Key + "\">" + divisionType.Key + "</option>");
                    }

					builder.AppendLine("</select></td></tr>");

					//Add the textbox for named divisions
					builder.AppendLine("<tr id=\"divNameTableRow"+page_index+"\" class=\""+txtDivNameCssClass+"\"><td class=\"namedDivisionText\" align=\"left\">"+division_name_text+"</td>");
                    builder.AppendLine("<td><input type=\"text\" id=\"txtDivName" + page_index + "\" name=\"txtDivName" + page_index + "\" class=\"" + pagination_box + "\" value=\"" + HttpUtility.HtmlEncode(parentLabel) + "\" onchange=\"DivNameTextChanged(this.id," + CurrentItem.Web.Static_PageCount + ");\"/></td></tr>");
					
					//Add the span with the on-hover-options for the page thumbnail
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
					builder.AppendLine("</td></tr>");

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
			builder.AppendLine("<script type=\"text/javascript\">window.onload=MakeSpanFlashOnPageLoad();</script>");
			builder.AppendLine("<script type=\"text/javascript\">window.onload=MakeSortable1();</script>");
//		    builder.AppendLine("<script type=\"text/javascript\">window.onload=MoveOnScroll();</script>");
//			builder.AppendLine("<script type=\"text/javascript\"> WindowResizeActions();</script>");
			
			//If the autosave option is not set, or set to true, set the interval (3 minutes) for autosaving
			if(String.IsNullOrEmpty(autosave_option.ToString()) || autosave_option)
			  builder.AppendLine("<script type=\"text/javascript\">setInterval(qc_auto_save, 180* 1000);</script>");

			//Add the Complete and Cancel buttons at the end of the form
			builder.AppendLine("</tr><tr><td colspan=\"100%\" style=\"float:right\">");
			builder.AppendLine("<button type=\"button\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/check.ico\" width=\"25\" height=\"25\"/>Complete</button>");
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

				// Get the links for the METS
				string greenstoneLocation = CurrentItem.Web.Source_URL + "/";
				string complete_mets = greenstoneLocation + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";

				// MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
				if ((CurrentItem.Behaviors.Dark_Flag) || (CurrentItem.Behaviors.IP_Restriction_Membership > 0))
				{
					complete_mets = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";
				}

				// Build the value
				StringBuilder navRowBuilder = new StringBuilder(5000);

				//Start building the top nav bar
				navRowBuilder.AppendLine("\t\t<!-- QUALITY CONTROL VIEWER TOP NAV ROW -->");

				//Include the js files
				navRowBuilder.AppendLine("<script language=\"JavaScript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_related_items.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-1.9.1.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.color-2.1.1.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_qc.js\"></script>");
			   navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.timers.min.js\"></script>");
			   navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.min.js\"></script>");
				
			  navRowBuilder.AppendLine("<link rel=\"stylesheet\" href=\"http://code.jquery.com/ui/1.10.1/themes/base/jquery-ui.css\" />");
			 // navRowBuilder.AppendLine("<script src=\"http://code.jquery.com/jquery-1.9.1.js\"></script>");
			  navRowBuilder.AppendLine("<script src=\"http://code.jquery.com/ui/1.10.1/jquery-ui.js\"></script>");


             // navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-1.6.2.min.js\"></script>");
              //navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.8.16.custom.min.js\"></script>");
              navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

              // Add the popup form
			  navRowBuilder.AppendLine();
              navRowBuilder.AppendLine("<!-- Pop-up form for moving page(s) by selecting the checkbox in image -->");
              navRowBuilder.AppendLine("<div class=\"qcmove_popup_div\" id=\"form_qcmove\" style=\"display:none;\">");
              navRowBuilder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">MOVE SELECTED PAGES</td><td align=\"right\"><a href=\"" + CurrentMode.Base_URL + "logon/help\" target=\"_FORM_QCMOVE_HELP\" >?</a> &nbsp; <a href=\"#template\" onclick=\" popdown( 'form_qcmove' ); \">X</a> &nbsp; </td></tr></table></div>");
              navRowBuilder.AppendLine("  <br />");
              navRowBuilder.AppendLine("  <table class=\"popup_table\">");

              // Add the rows of data
			    navRowBuilder.AppendLine("<tr><td>ALL STUFF HERE</td></tr>");



              // Finish the popup form
              navRowBuilder.AppendLine("  </table>");
              navRowBuilder.AppendLine("  <br />");
              navRowBuilder.AppendLine("</div>");
              navRowBuilder.AppendLine();



              add_main_menu(navRowBuilder);

              navRowBuilder.AppendLine("<div id=\"divMoveOnScroll\" class=\"qcDivMoveOnScrollHidden\"><button type=\"button\" id=\"btnMovePages\" name=\"btnMovePages\" class=\"btnMovePages\">Move to</button></div>");
			  navRowBuilder.AppendLine("<table width=\"100%\"><tr align=\"center\">");
				

				//Add the dropdown for the number of thumbnails per page, only if there are >25 thumbnails
				if (CurrentItem.Web.Static_PageCount > 25)
				{
					//Redirect to the first page of results when the number of thumbnails option is changed by the user
					string current_viewercode = CurrentMode.ViewerCode;
					CurrentMode.ViewerCode = "1qc";
					string current_Page_url = CurrentMode.Redirect_URL("1qc");

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


				// For now, just add a TEST link here
			    navRowBuilder.AppendLine("<td><a id=\"form_qcmove_link\" href=\"http://ufdc.ufl.edu/l/technical/javascriptrequired\" onclick=\"return popup('form_qcmove', 'form_qcmove_link', 280, 400 );\">test</a></td>");

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
				if (String.IsNullOrEmpty(autosave_option.ToString()) || (autosave_option))
					navRowBuilder.AppendLine("<span><a id=\"autosaveLink\" href=\"\" onclick=\"javascript:changeAutoSaveOption(); return false;\">Turn Off Autosave</a>");
				else
					navRowBuilder.AppendLine("<span><a id=\"autosaveLink\" href=\"\" onclick=\"javascript:changeAutoSaveOption(); return false;\">Turn On Autosave</a>");
				navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
				navRowBuilder.AppendLine("<span><a href=\"\" onclick=\"javascript:behaviors_save_form(); return false;\"><img src=\"" + image_location + "ToolboxImages/Save.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></span>");
				navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
				navRowBuilder.AppendLine("<span><a href=\"\" onclick=\"javascript:ResetCursorToDefault("+CurrentItem.Web.Static_PageCount+"); return false;\"><img src=\"" + image_location + "ToolboxImages/Point13.ICO" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></span>");
				navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
				navRowBuilder.AppendLine("<span><a href=\"\" onclick=\"javascript:ChangeMouseCursor("+CurrentItem.Web.Static_PageCount+"); return false;\"><img src=\"" + image_location + "ToolboxImages/thumbnail_large.gif" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></span>");
				navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
                navRowBuilder.AppendLine("<span><a href=\"\" onclick=\"javascript:MovePages(" + CurrentItem.Web.Static_PageCount + "); return false;\"><img src=\"" + image_location + "ToolboxImages/DRAG1PG.ICO" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></span>");
                //navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
                //navRowBuilder.AppendLine("<span><img src=\"" + image_location + "ToolboxImages/next_error.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"></img></span>");
				navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;");
				navRowBuilder.AppendLine("<span><a href=\"" + complete_mets + "\" target=\"_blank\"><img src=\"" + image_location + "ToolboxImages/mets.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"></img></a></span>");
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

        /// <summary> Flag indicates if the item viewer should add the standard item menu, or
        /// if this item viewer overrides that menu and will write its own menu </summary>
        /// <remarks> By default, this returns TRUE.  The QC and the spatial editing itemviewers create their own custom menus
        /// due to the complexity of the work being done in those viewers. </remarks>
        /// <value>This always returns FALSE since it writes its own menu </value>
        public override bool Include_Standard_Item_Menu
        {
            get { return false; }
        }
	}
}
