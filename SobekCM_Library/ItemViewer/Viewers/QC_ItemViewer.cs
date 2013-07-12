#region Using directives

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Configuration;
using SobekCM.Library.HTML;
using SobekCM.Library.Items;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;
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
        private int autonumber_mode_from_form; //Mode 0: autonumber all pages of current div; Mode 1: all pages of document
        private int autonumber_mode;
        private string autonumber_number_system;
        private string autonumber_text_only;
        private string autonumber_number_only;
        private string hidden_autonumber_filename;
        private string hidden_request;
        private string hidden_main_thumbnail;
        private bool autosave_option;
        private string hidden_move_relative_position;
        private string hidden_move_destination_fileName;
        private string userInProcessDirectory;
        private string complete_mets;
        private bool makeSortable = true;
        private bool isLower;
        private string mainThumbnailFileName;
        private string mainJPGFileName;
        private SobekCM_Item qc_item;

        private Dictionary<Page_TreeNode, Division_TreeNode> childToParent;
        private QualityControl_Profile qc_profile;

        /// <summary> Constructor for a new instance of the QC_ItemViewer class </summary>
        public QC_ItemViewer(SobekCM_Item Current_Object, User_Object Current_User, Navigation.SobekCM_Navigation_Object Current_Mode)
        {
            // Save the current user and current mode information (this is usually populated AFTER the constructor completes, 
            // but in this case (QC viewer) we need the information for early processing
            this.CurrentMode = Current_Mode;
            this.CurrentUser = Current_User;

            qc_item = Current_Object;

            // If there is no user, send to the login
            if (CurrentUser == null)
            {
                CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
                CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                HttpContext.Current.Response.Redirect(CurrentMode.Redirect_URL());
                return;
            }

            // If the user cannot edit this item, go back
            if (!CurrentUser.Can_Edit_This_Item(Current_Object))
            {
                CurrentMode.ViewerCode = String.Empty;
                HttpContext.Current.Response.Redirect(CurrentMode.Redirect_URL());
                return;
            }

            // Get the links for the METS
            string greenstoneLocation = qc_item.Web.Source_URL + "/";
            complete_mets = greenstoneLocation + qc_item.BibID + "_" + qc_item.VID + ".mets.xml";

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((qc_item.Behaviors.Dark_Flag) || (qc_item.Behaviors.IP_Restriction_Membership > 0))
            {
                complete_mets = CurrentMode.Base_URL + "files/" + qc_item.BibID + "/" + qc_item.VID + "/" + qc_item.BibID + "_" + qc_item.VID + ".mets.xml";
            }


            // Get the special qc_item, which matches the passed in Current_Object, at least the first time.
            // If the QC work is already in process, we may find a temporary METS file to read.

            // Determine the in process directory for this
            userInProcessDirectory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + Current_User.UserName.Replace(".", "").Replace("@", "") + "\\qcwork";
            if (Current_User.UFID.Trim().Length > 0)
                userInProcessDirectory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + Current_User.UFID + "\\qcwork";

            // Is this work in the user's SESSION state?
            qc_item = HttpContext.Current.Session[Current_Object.BibID + "_" + Current_Object.VID + " QC Work"] as SobekCM_Item;
            if (qc_item == null)
            {
                // Is there a temporary METS for this item, which is not expired?
                string metsInProcessFile = userInProcessDirectory + "\\" + Current_Object.BibID + "_" + Current_Object.VID + ".mets";
                if ((File.Exists(metsInProcessFile)) &&
                    (File.GetLastWriteTime(metsInProcessFile).Subtract(DateTime.Now).Hours < 8))
                {
                    // Read the temporary METS file, and use that to build the qc_item
                    qc_item = SobekCM_Item_Factory.Get_Item(metsInProcessFile, Current_Object.BibID, Current_Object.VID, null, null);
                }
                else
                {
                    // Just read the normal otherwise ( if we had the ability to deep copy a SObekCM_Item, we could skip this )
                    qc_item = SobekCM_Item_Factory.Get_Item(Current_Object.BibID, Current_Object.VID, null, null);
                }

                // Save to the session, so it is easily available for next time
                HttpContext.Current.Session[Current_Object.BibID + "_" + Current_Object.VID + " QC Work"] = qc_item;
            }

            // If no QC item, this is an error
            if (qc_item == null)
            {
                throw new ApplicationException("Unable to retrieve the item for Quality Control in QC_ItemViewer.Constructor");
            }

            // Get the default QC profile
            qc_profile = QualityControl_Configuration.Default_Profile;

            title = "Quality Control";

            //TODO: Get the item main thumbnail info from the db
            //mainThumbnailFileName = qc_item.

            // See if there were hidden requests
            hidden_request = HttpContext.Current.Request.Form["QC_behaviors_request"] ?? String.Empty;
            hidden_main_thumbnail = HttpContext.Current.Request.Form["Main_Thumbnail_Index"] ?? String.Empty;
            hidden_move_relative_position = HttpContext.Current.Request.Form["QC_move_relative_position"] ?? String.Empty;
            hidden_move_destination_fileName = HttpContext.Current.Request.Form["QC_move_destination"] ?? String.Empty;
            autonumber_number_system = HttpContext.Current.Request.Form["Autonumber_number_system"] ?? String.Empty;
            string temp = HttpContext.Current.Request.Form["autonumber_mode_from_form"] ?? "0";
            Int32.TryParse(temp, out autonumber_mode_from_form);
            autonumber_text_only = HttpContext.Current.Request.Form["Autonumber_text_without_number"] ?? String.Empty;
            autonumber_number_only = HttpContext.Current.Request.Form["Autonumber_number_only"] ?? String.Empty;
            autonumber_number_system = HttpContext.Current.Request.Form["Autonumber_number_system"] ?? String.Empty;
            hidden_autonumber_filename = HttpContext.Current.Request.Form["Autonumber_last_filename"] ?? String.Empty;

            if (!(Boolean.TryParse(HttpContext.Current.Request.Form["QC_Sortable"], out makeSortable))) makeSortable = true;
            // If the hidden move relative position is BEFORE, it is before the very first page
            if (hidden_move_relative_position == "Before")
                hidden_move_destination_fileName = "[BEFORE FIRST]";

            try
            {

                //Call the JavaScript autosave function based on the option selected
                bool autosaveCacheValue = true;
                bool autosaveCache = false;

                //Conversion result of autosaveCacheValue(conversion successful or not) saved in autosaveCache

                if (HttpContext.Current.Cache.Get("autosave_option") != null)
                    autosaveCache = bool.TryParse(HttpContext.Current.Cache.Get("autosave_option").ToString(), out autosaveCacheValue);
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
                    HttpContext.Current.Cache.Insert("autosave_option", (object) autosave_option);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error retrieving auto save option. " + e.Message);
            }

            //If a main thumbnail value was selected (either for assignment or removal)
            if (!String.IsNullOrEmpty(hidden_main_thumbnail) && (hidden_request == "pick_main_thumbnail" || hidden_request == "unpick_main_thumbnail"))
            {
                // Save the selected page index to the cache if request_type is "pick"
                if (hidden_request == "pick_main_thumbnail")
                    HttpContext.Current.Cache.Insert("main_thumbnail_index", (object) hidden_main_thumbnail);

                    //else clear the cached value if request_type is "unpick"
                else if (hidden_request == "unpick_main_thumbnail")
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
                if (HttpContext.Current.Cache.Get("main_thumbnail_index") != null)
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
                //Save the current time
                HttpContext.Current.Session["QC_timeUpdated"] = DateTime.Now.ToString("hh:mm tt");

                // Read the data from the http form, perform all requests, and
                // update the qc_item (also updates the session and temporary files)
                Save_From_Form_Request_To_Item(String.Empty, String.Empty);

                // Save this updated information in the temporary folder's METS file for reading
                // later if necessary.
                string url_redirect = HttpContext.Current.Request.Url.ToString();
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl.ToString());

            }
            else if (hidden_request == "move_selected_pages")
            {
                // Read the data from the http form, perform all requests, and
                // update the qc_item (also updates the session and temporary files)
                List<QC_Viewer_Page_Division_Info> selected_pages = Save_From_Form_Request_To_Item(hidden_move_destination_fileName, String.Empty);

                string url_redirect = HttpContext.Current.Request.Url.ToString();
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl.ToString());
            }
            else if (hidden_request == "delete_page")
            {
                // Read the data from the http form, perform all requests, and
                // update the qc_item (also updates the session and temporary files)
                string filename_to_delete = HttpContext.Current.Request.Form["QC_affected_file"] ?? String.Empty;
                Save_From_Form_Request_To_Item(String.Empty, filename_to_delete);

                string url_redirect = HttpContext.Current.Request.Url.ToString();
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl.ToString());

            }
            else if (hidden_request == "delete_selected_page")
            {
                // Read the data from the http form, perform all requests, and
                // update the qc_item (also updates the session and temporary files)
                Save_From_Form_Request_To_Item(String.Empty, String.Empty);

                string url_redirect = HttpContext.Current.Request.Url.ToString();
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl.ToString());

            }
        }

      //TODO: Complete this method
        private void Clear_Pagination_And_Reorder_Pages()
        {
            SortedDictionary<Page_TreeNode,string> nodeToFilename = new SortedDictionary<Page_TreeNode, string>();
            try
            {
                foreach (abstract_TreeNode thisNode in qc_item.Divisions.Physical_Tree.Divisions_PreOrder)
                {
                    
                    //Is this a page node?
                    if (thisNode.Page)
                    {
                        thisNode.Label = String.Empty;

                       //nodeToFilename.Add(thisNode.GetHashCode(), ((Page_TreeNode) thisNode).Files[0].File_Name_Sans_Extension);
                        

                    }
                    else
                    {
                        
                    }
                }
                // Save the updated to the session
                  HttpContext.Current.Session[qc_item.BibID + "_" + qc_item.VID + " QC Work"] = qc_item;

                  // Save to the temporary QC work section
                  try
                  {
                      // Ensure the directory exists under the user's temporary mySobek InProcess folder
                      if (!Directory.Exists(userInProcessDirectory))
                          Directory.CreateDirectory(userInProcessDirectory);

                      // Save the METS
                      qc_item.Save_METS(userInProcessDirectory + "\\" + qc_item.BibID + "_" + qc_item.VID + ".mets");
                  }
                  catch (Exception)
                  {
                      throw;
                  }
            }
            catch (Exception)
            {
                throw;
            }
        }


//TODO:Test if this function is working correctly
    /// <summary> Clears the pagination names of all the pages of the qc item </summary>
          private void ClearPagination()
          {
              try
              {
                  foreach (abstract_TreeNode thisNode in qc_item.Divisions.Physical_Tree.Divisions_PreOrder)
                  {

                      //Is this a page node?
                      if (thisNode.Page)
                      {
                          thisNode.Label = String.Empty;
                      }
                  }
                  // Save the updated to the session
                  HttpContext.Current.Session[qc_item.BibID + "_" + qc_item.VID + " QC Work"] = qc_item;

                  // Save to the temporary QC work section
                  try
                  {
                      // Ensure the directory exists under the user's temporary mySobek InProcess folder
                      if (!Directory.Exists(userInProcessDirectory))
                          Directory.CreateDirectory(userInProcessDirectory);

                      // Save the METS
                      qc_item.Save_METS(userInProcessDirectory + "\\" + qc_item.BibID + "_" + qc_item.VID + ".mets");
                  }
                  catch (Exception)
                  {
                      throw;
                  }

              }
              catch (Exception e)
              {
                  throw new Exception("Error clearing all pagination names"+e.Message);
              }
          }

		/// <summary> Save all the data from form post-back into the item in memory, and 
		/// return all the page information for those pages which are CHECKED (with the checkbox) </summary>
		/// <returns> Returns the list of all selected (or checked on the checkbox) page data</returns>
		private List<QC_Viewer_Page_Division_Info> Save_From_Form_Request_To_Item(string filename_to_move_after, string filename_to_omit )
		{
			// Get the current page number
			int current_qc_viewer_page_num = 1;
			if (CurrentMode.ViewerCode.Replace("qc", "").Length > 0)
				Int32.TryParse(CurrentMode.ViewerCode.Replace("qc", ""), out current_qc_viewer_page_num);


			// First, build a dictionary of all the pages ( filename --> page division object )
			Dictionary<Page_TreeNode, Division_TreeNode> pages_to_division = new Dictionary<Page_TreeNode, Division_TreeNode>();
			Dictionary<string, Page_TreeNode> pages_by_name = new Dictionary<string, Page_TreeNode>();
			List<Page_TreeNode> page_list = new List<Page_TreeNode>();
			List<string> page_filename_list = new List<string>();
			Division_TreeNode lastDivision = null;
		   //Autonumber the remaining pages based on the selected option
			if (autonumber_mode_from_form == 0 || autonumber_mode_from_form == 1)
			{
	//		    autonumber_mode = autonumber_mode_from_form;
				bool reached_last_page = false;
				bool reached_next_div = false;
				int number=0;
				if (autonumber_number_system == "decimal")
					number = Int32.Parse(autonumber_number_only) + 1;
				else if (autonumber_number_system == "roman")
					number = RomanToNumber(autonumber_number_only) +1;

				//Do the autonumbering first
				foreach (abstract_TreeNode thisNode in qc_item.Divisions.Physical_Tree.Divisions_PreOrder)
				{
					//Is this a division or a page node?
					if (thisNode.Page)
					{
						Page_TreeNode thisPage = (Page_TreeNode) thisNode;
					 
						//Verify the page
						if (thisPage.Files.Count > 0)
						{
							string filename = thisPage.Files[0].File_Name_Sans_Extension;

							if (filename == hidden_autonumber_filename)
							{
							  //  bool b = true;
								reached_last_page = true;

							}
							//if the last page displayed on the screen has been reached
							else if (reached_last_page == true)
							{
								//Mode "0": Autonumber all pages of current division
								//Mode "1": Autonumber all pages of the entire document
								if ((autonumber_mode_from_form == 0 && reached_next_div == false) || (autonumber_mode_from_form==1))
								{
									if(autonumber_number_system=="decimal")
									  thisPage.Label = autonumber_text_only + number.ToString();
									else
									{
										thisPage.Label = autonumber_text_only + NumberToRoman(number);
									}
									number++;
								}

							}
	
						}
					}
					else if(reached_last_page==true)
					{
						reached_next_div = true;
					}

				}
			}

		   //Move/Delete Pages as appropriate 
			foreach (abstract_TreeNode thisNode in qc_item.Divisions.Physical_Tree.Divisions_PreOrder)
			{
				// Is this a division, or page node?
				if (thisNode.Page)
				{
					Page_TreeNode thisPage = (Page_TreeNode)thisNode;
					// Verify the page 
					if (thisPage.Files.Count > 0)
					{
						string filename = thisPage.Files[0].File_Name_Sans_Extension;
						pages_by_name[filename] = thisPage;
						page_filename_list.Add(filename);
					}

					// Add to the list of pages
					page_list.Add(thisPage);

					// Save the link from the page, up to the division
					pages_to_division[thisPage] = lastDivision;
				}
				else
				{
					lastDivision = (Division_TreeNode)thisNode;
				}
			}

			// Step through and collect all the form data
			List<QC_Viewer_Page_Division_Info> page_div_from_form = new List<QC_Viewer_Page_Division_Info>();
			List<Page_TreeNode> existing_pages_in_window = new List<Page_TreeNode>();

			//Get the list of pages to be moved
			List<string> pages_to_move_list = new List<string>();
			List<QC_Viewer_Page_Division_Info> selected_page_div_from_form = new List<QC_Viewer_Page_Division_Info>();
		  
			try
			{
				// Now, step through each of the pages in the return
				string[] keysFromForm = HttpContext.Current.Request.Form.AllKeys;
				foreach (string thisKey in keysFromForm)
				{
					// Has this gotten to the next page?
					if ((thisKey.IndexOf("filename") == 0) && (thisKey.Length > 8))
					{
						QC_Viewer_Page_Division_Info thisInfo = new QC_Viewer_Page_Division_Info();

						// Get the filename for this new page
						thisInfo.Filename = HttpContext.Current.Request.Form[thisKey];

						// Get the index to use for all the other keys
						string thisIndex = thisKey.Substring(8);

						// Get the page name 
						thisInfo.Page_Label = HttpContext.Current.Request.Form["textbox" + thisIndex];

						// Was this page selected with the checkbox?  (for bulk delete or move)
						//Get this info only if the move/delete operations are explicitly triggered
						if (hidden_request == "delete_page" || hidden_request == "delete_selected_page" || hidden_request == "move_selected_pages")
						{
							if ((HttpContext.Current.Request.Form["chkMoveThumbnail" + thisIndex] != null) || (thisInfo.Filename == filename_to_omit))
							{
								thisInfo.Checkbox_Selected = true;
								selected_page_div_from_form.Add(thisInfo);
							}
						}
						// Is this a new division?
						if (HttpContext.Current.Request.Form["newdiv" + thisIndex] != null)
						{
							thisInfo.New_Division = true;

							// Get the new division type/label
							thisInfo.Division_Type = HttpContext.Current.Request.Form["selectDivType" + thisIndex].Trim().Replace("!", "");
							thisInfo.Division_Label = String.Empty;
							if (HttpContext.Current.Request.Form["txtDivName" + thisIndex] != null)
								thisInfo.Division_Label = HttpContext.Current.Request.Form["txtDivName" + thisIndex].Trim();
							if (thisInfo.Division_Type.Length == 0)
								thisInfo.Division_Type = "Chapter";

							// Get the division config, based on the division type
							if (qc_profile[thisInfo.Division_Type] != null)
							{
								QualityControl_Division_Config divInfo = qc_profile[thisInfo.Division_Type];

								if (divInfo.BaseTypeName.Length > 0)
								{
									thisInfo.Division_Label = thisInfo.Division_Type;
									thisInfo.Division_Type = divInfo.BaseTypeName;
								}
							}
						}
						else
						{
							thisInfo.New_Division = false;
						}

						// Add this page to the collection
						page_div_from_form.Add(thisInfo);

						// Also, collect the page node from the mets/resource for clearing later
						if (pages_by_name.ContainsKey(thisInfo.Filename))
						{
							Page_TreeNode existing_pagenode = pages_by_name[thisInfo.Filename];
							existing_pages_in_window.Add(existing_pagenode);
						}
					}
				}
			}
			catch (Exception ee)
			{
				bool error = true;
			}

			// Determine the "window" that the user was seeing
			int window_first_page_index = page_filename_list.Count - 1;
			int window_last_page_index = 0;
			foreach (QC_Viewer_Page_Division_Info thisInfo in page_div_from_form)
			{
				// Get the filename and then get the order
				int page_order = page_filename_list.IndexOf(thisInfo.Filename);
				if (page_order < window_first_page_index)
					window_first_page_index = page_order;
				if (page_order > window_last_page_index)
					window_last_page_index = page_order;
			}

			// TODO: Do some sanity checks here to ensure it worked

			// Determine if the first page was part of an existing division (and not the first page in that division)
			Page_TreeNode window_first_page = page_list[window_first_page_index];
			Division_TreeNode window_first_division = pages_to_division[window_first_page];
			Division_TreeNode existing_division_containing_first_page = null;
			if (window_first_division.Nodes.IndexOf(window_first_page) > 0)
				existing_division_containing_first_page = window_first_division;


			// Collect any additional, non-cleared pages from the division which contained the last page originally
			List<Page_TreeNode> remnant_pages = new List<Page_TreeNode>();
			Page_TreeNode window_last_page = page_list[window_last_page_index];
			Division_TreeNode window_last_division = pages_to_division[window_last_page];
			if (window_last_division.Nodes.IndexOf(window_last_page) < window_last_division.Nodes.Count)
			{
				for (int i = window_last_division.Nodes.IndexOf(window_last_page) + 1;
					 i < window_last_division.Nodes.Count;
					 i++)
				{
					remnant_pages.Add((Page_TreeNode)window_last_division.Nodes[i]);
				}
			}



			// Clear the window pages completely, including the remnant pages, which we add back at the end
			int index_within_chapter_roots_to_begin_insert = qc_item.Divisions.Physical_Tree.Roots.Count;
			foreach (Page_TreeNode thisNode in existing_pages_in_window)
			{
				// Get the parent division, to clear this
				Division_TreeNode parentNode = pages_to_division[thisNode];
				parentNode.Nodes.Remove(thisNode);

				// What is the index within the list of chapters
				int this_root_index = qc_item.Divisions.Physical_Tree.Roots.IndexOf(parentNode) + 1;


				// Does this clear out the chapter completely?
				if (parentNode.Nodes.Count == 0)
				{
					qc_item.Divisions.Physical_Tree.Roots.Remove(parentNode);
					this_root_index--;
				}

				// If this insert point is prior to the previously collected insert point, use this one for the first chapter
				if (this_root_index < index_within_chapter_roots_to_begin_insert)
					index_within_chapter_roots_to_begin_insert = this_root_index;
			}
			foreach (Page_TreeNode thisNode in remnant_pages)
			{
				// Get the parent division, to clear this
				window_last_division.Nodes.Remove(thisNode);

				// Does this clear out the chapter completely?
				if (window_last_division.Nodes.Count == 0)
				{
					qc_item.Divisions.Physical_Tree.Roots.Remove(window_last_division);
				}
			}

			int move_into_division_index = -1;
			int move_into_node_index = -1;
		  //  string filename_to_move_after = "00002";

			// Add each page from the original form
			Division_TreeNode last_added_division = existing_division_containing_first_page;
			foreach (QC_Viewer_Page_Division_Info pageInfo in page_div_from_form)
			{
				// Is this a new division?
				if (pageInfo.New_Division)
				{
					// If there was a last division, ensure some pages were added and add to the METS
					if (last_added_division != null)
					{
						// Were any pages added to this last div?
						if (last_added_division.Nodes.Count > 0)
						{
							// Since there were pages, add this to the METS
							qc_item.Divisions.Physical_Tree.Roots.Insert(index_within_chapter_roots_to_begin_insert++, last_added_division);
						}
					}

					// Create the new division
					last_added_division = new Division_TreeNode(pageInfo.Division_Type, pageInfo.Division_Label);
				}

				// Get the page tree node and assign the new page label
				Page_TreeNode thisPage = pages_by_name[pageInfo.Filename];
				thisPage.Label = pageInfo.Page_Label;

				// Add this page to the last division (possibly just created above) assuming it is 
				// not marked for removal (either by mass delete or mass move)
				if (!pageInfo.Checkbox_Selected)
					last_added_division.Add_Child(thisPage);
				else
				{
					// Save the built page node for later, in case they will be MOVED
					pageInfo.METS_STructMap_Page_Node = thisPage;
				}

				// Were we involved in a mass move, in which case we are looking for the insertion point?
				if ((filename_to_move_after.Length > 0) && (move_into_division_index < 0 ) && (pageInfo.Filename == filename_to_move_after))
				{
					move_into_division_index = index_within_chapter_roots_to_begin_insert;
					move_into_node_index = last_added_division.Nodes.Count;
				}
			}

			// Handle all the remnant by adding to the last division 
			foreach (Page_TreeNode thisNode in remnant_pages)
			{
				last_added_division.Add_Child(thisNode);
			}
			
			// Handle any unfinished divisions
			// If there was a last division, ensure some pages were added and add to the METS
			if (last_added_division != null)
			{
				// Were any pages added to this last div?
				if (last_added_division.Nodes.Count > 0)
				{
					// Since there were pages, add this to the METS
					qc_item.Divisions.Physical_Tree.Roots.Insert(index_within_chapter_roots_to_begin_insert++, last_added_division);
				}
			}


			// Insert any pages which were moved
			if ((filename_to_move_after.Length > 0) && ( selected_page_div_from_form.Count > 0 ))
			{
				// TODO: Check for the lack of any divisions what-so-ever within the METS.  If so, add one.

				Division_TreeNode divNodeToInsertWithin = null;

				 // Get the division
				if (move_into_division_index >= 0)
					divNodeToInsertWithin = (Division_TreeNode) qc_item.Divisions.Physical_Tree.Roots[move_into_division_index];
				else if (filename_to_move_after == "[BEFORE FIRST]")
				{
					divNodeToInsertWithin = (Division_TreeNode) qc_item.Divisions.Physical_Tree.Roots[0];
					move_into_node_index = 0;
				}

				if ( divNodeToInsertWithin != null )
				{
					// Insert each page in order
					foreach (QC_Viewer_Page_Division_Info insertPage in selected_page_div_from_form)
					{
						divNodeToInsertWithin.Nodes.Insert(move_into_node_index++, insertPage.METS_STructMap_Page_Node);
					}
				}
			}

			// Save the updated to the session
			HttpContext.Current.Session[qc_item.BibID + "_" + qc_item.VID + " QC Work"] = qc_item;

			// Save to the temporary QC work section
			try
			{
				// Ensure the directory exists under the user's temporary mySobek InProcess folder
				if (!Directory.Exists(userInProcessDirectory))
					Directory.CreateDirectory(userInProcessDirectory);

				// Save the METS
				qc_item.Save_METS(userInProcessDirectory + "\\" + qc_item.BibID + "_" + qc_item.VID + ".mets");

                //TODO: Save changes to the DB
                //// Determine the total size of the package before saving
                //string[] all_files_final = Directory.GetFiles(userInProcessDirectory);
                //double size = all_files_final.Aggregate<string, double>(0, (current, thisFile) => current + (((new FileInfo(thisFile)).Length) / 1024));
                //Item_To_Complete.DiskSize_MB = size;
                //SobekCM.Resource_Object.Database.SobekCM_Database.QC_Update_Item_Info(CurrentItem.VID, CurrentUser.UserName, mainThumbnailFileName, mainJPGFileName, PageCount, FileCount, disksize_mb);
			}
			catch (Exception)
			{
				throw;
			}

			// Return the information about all checked boxes
			return selected_page_div_from_form;
		}

		/// <summary> Override the property to get the current item, since the QC viewer uses a DIFFERENT item, to avoid
		/// changing the publicly available item until the user clicks submit and the new METS is written </summary>
		public override SobekCM_Item CurrentItem
		{
			protected get
			{
				// Allow the special qc_item to be retrieved for writing titles, etc.. 
				return qc_item;
			}
			set
			{
				// Do not allow the current item to be set
			}
		}
  
		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Quality_Control"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.Quality_Control; }
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

		#region Methods and properties to allow the paging to work correctly

		/// <summary> Gets the flag that indicates if the page selector should be shown </summary>
		/// <value> This is a single page viewer, so this property always returns NONE</value>
		public override ItemViewer_PageSelector_Type_Enum Page_Selector
		{
			get
			{
				return ItemViewer_PageSelector_Type_Enum.PageLinks;
			}
		}
 
		/// <summary> Gets the number of pages for this viewer </summary>
		/// <remarks> If there are more than 100 related images, then the thumbnails are paged at 60 a page</remarks>
		///<remarks>Edit: The default number of items per page is 50. If a diferent number is selected by the user, this is fetched from the query string</remarks>
		public override int PageCount
		{
			get
			{
				return ((qc_item.Web.Static_PageCount - 1) / thumbnailsPerPage) + 1;
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

		#endregion

		#region Method to add the main menu 

		private void add_main_menu(StringBuilder builder)
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

			if (qc_item.Web.Static_PageCount > 100)
				thumbnails_per_page = 50;
			else
			{
				thumbnails_per_page = qc_item.Web.Static_PageCount;
			}
			//Set the thumbnails_per_page to the value from the query string, if present
			Uri uri = HttpContext.Current.Request.Url;

			if (uri.Query.IndexOf("ts") > 0)
			{
				size_of_thumbnails = Convert.ToInt32(HttpUtility.ParseQueryString(uri.Query).Get("nt"));
				CurrentMode.Size_Of_Thumbnails = (short)size_of_thumbnails;
			}
			else
			{
				CurrentMode.Size_Of_Thumbnails = -1;
			}

            //// Get the links for the METS
            //string greenstoneLocation = qc_item.Web.Source_URL + "/";
            //string complete_mets = greenstoneLocation + qc_item.BibID + "_" + qc_item.VID + ".mets.xml";

            //// MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            //if ((qc_item.Behaviors.Dark_Flag) || (qc_item.Behaviors.IP_Restriction_Membership > 0))
            //{
            //    complete_mets = CurrentMode.Base_URL + "files/" + qc_item.BibID + "/" + qc_item.VID + "/" + qc_item.BibID + "_" + qc_item.VID + ".mets.xml";
            //}


			//StringBuilder builder = new StringBuilder(4000);
			builder.AppendLine("<div id=\"qcmenubar\">");
			builder.AppendLine("<ul id=\"qc-main-menu\" class=\"sf2-menu\">");

			//builder.AppendLine("<li class=\"qc-menu-item\">Resource<ul>");
			//builder.AppendLine("\t<li>Volume Error<ul>");
			//builder.AppendLine("\t\t<li>No volume level error</li>");
			//builder.AppendLine("\t\t<li>Invalid images</li>");
			//builder.AppendLine("\t\t<li>Incorrect volume/title</li>");
			//builder.AppendLine("\t</ul></li>");
            builder.AppendLine("\t<li><a href=\"\" onclick=\"javascript:behaviors_save_form(); return false;\">Save</a></li>");

            builder.AppendLine("<li><a href=\"#\">Resource</a><ul>");
            builder.AppendLine("\t<li><a href=\"#\">Save</a></li>");
            builder.AppendLine("\t<li><a href=\"#\">Complete</a></li>");
            builder.AppendLine("\t<li><a href=\"#\">Cancel</a></li>");
			builder.AppendLine("</ul></li>");

            builder.AppendLine("<li><a href=\"#\">Edit</a><ul>");
            builder.AppendLine("\t<li><a href=\"#\">Clear Pagination</a></li>");
            builder.AppendLine("\t<li><a href=\"#\">Clear All &amp; Reorder Pages</a></li>");
			builder.AppendLine("</ul></li>");

            builder.AppendLine("<li><a href=\"#\">Settings</a><ul>");
            builder.AppendLine("\t<li>Thumbnail Size<ul>");
			//Add the thumbnail size options
            if (thumbnailSize == 1)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*Small</a></li>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 1;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">Small</a></li>");
            }

            if (thumbnailSize == 2)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*Medium</a></li>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 2;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">Medium</a></li>");
            }

            if (thumbnailSize == 3)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*Large</a></li>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 3;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">Large</a></li>");
            }


            //Reset the current mode
            CurrentMode.Size_Of_Thumbnails = (short)thumbnailSize;
            

			builder.AppendLine("\t</ul></li>");
            builder.AppendLine("\t<li><a href=\"#\">Thumbnails per page</a><ul>");

            //Add the thumbnails per page options
		    if (thumbnailsPerPage == 25)
		        builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*25</a></li>");
		    else
		    {
		        CurrentMode.Thumbnails_Per_Page = 25;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">25</a></li>");
		    }
            if (thumbnailsPerPage == 50)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*50</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 50;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">50</a></li>");
            }
            if (thumbnailsPerPage == 100)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*100</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 100;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">100</a></li>");
            }
            if (thumbnailsPerPage == 500)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*500</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 500;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">500</a></li>");
            }
            if (thumbnailsPerPage == 1000)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*1000</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 1000;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">1000</a></li>");
            }
		    if (thumbnailsPerPage == qc_item.Web.Static_PageCount)
		        builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*All thumbnails</a></li>");
		    else
		    {
		        CurrentMode.Thumbnails_Per_Page = (short)qc_item.Web.Static_PageCount;
		        builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">All thumbnails</a></li>");
		    }
            //Reset the current mode
		    CurrentMode.Thumbnails_Per_Page = (short)thumbnails_per_page;
			builder.AppendLine("\t</ul></li>");

            
            builder.AppendLine("\t<li>Automatic Numbering<ul>");
		    if (autonumber_mode == 2)
		        builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc")+"\">*No automatic numbering</a></li>");
		    else
		    {
		        CurrentMode.Autonumbering_Mode = 2;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">No automatic numbering</a></li>");
		    }
            if (autonumber_mode == 1)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*Within same division</a></li>");
            else
            {
                CurrentMode.Autonumbering_Mode = 1;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">Within same division</a></li>");
            }
            if (autonumber_mode == 0)
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">*Entire document</a></li>");
            else
            {
                CurrentMode.Autonumbering_Mode = 0;
                builder.AppendLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">Entire document</a></li>");
            }
            //Reset the mode
		    CurrentMode.Autonumbering_Mode = autonumber_mode;

	        builder.AppendLine("\t</ul></li>");
			builder.AppendLine("</ul></li>");

			builder.AppendLine("<li>View<ul>");
            builder.AppendLine("\t<li><a href=\""+complete_mets+"\" target=\"_blank\">View METS</a></li>");
			builder.AppendLine("\t<li>View Directory</li>");
			
            My_Sobek_Type_Enum temp_my_sobek_type = CurrentMode.My_Sobek_Type;
		    string temp_my_sobek_submode = CurrentMode.My_Sobek_SubMode;

            CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
            CurrentMode.My_Sobek_SubMode = "1";
            builder.AppendLine("\t<li><a href=\""+CurrentMode.Redirect_URL()+">View QC History</a></li>" );
		    
            //Reset the current mode
            CurrentMode.My_Sobek_Type = temp_my_sobek_type;
		    CurrentMode.My_Sobek_SubMode = temp_my_sobek_submode;
			
            builder.AppendLine("</ul></li>");

			builder.AppendLine("<li>Help</li>");
			
			// Add the option to GO TO a certain thumbnail next
			builder.AppendLine("<li>");
			builder.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Go_To_Thumbnail + ":</b>");
			builder.AppendLine("<span><select id=\"selectGoToThumbnail\" onchange=\"location=this.options[this.selectedIndex].value; AddAnchorDivEffect(this.options[this.selectedIndex].value);\" >");

			//iterate through the page items
			if (qc_item.Web.Static_PageCount > 0)
			{
				int thumbnail_count = 0;
				foreach (Page_TreeNode thisFile in qc_item.Web.Pages_By_Sequence)
				{
					thumbnail_count++;

					string current_Page_url1 = CurrentMode.Redirect_URL((thumbnail_count / thumbnails_per_page + (thumbnail_count % thumbnails_per_page == 0 ? 0 : 1)).ToString() + "qc");

					builder.AppendLine("<option value=\"" + current_Page_url1 + "#" + thisFile.Label + "\">" + thisFile.Label + "</option>");

				}
			}
			builder.AppendLine("</select></span>");

			builder.AppendLine("</li>");

			//Get the icons for the thumbnail sizes
			string image_location = CurrentMode.Default_Images_URL;

			builder.AppendLine("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"" + complete_mets + "\" target=\"_blank\"><img src=\"" + image_location + "ToolboxImages/mets.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"></img></a></li>");
			builder.AppendLine("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"\" onclick=\"javascript:DeletePages(" + qc_item.Web.Static_PageCount + "); return false;\"><img src=\"" + image_location + "ToolboxImages/TRASH01.ICO" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></li>");
			builder.AppendLine("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"\" onclick=\"javascript:MovePages(" + qc_item.Web.Static_PageCount + "); return false;left\"><img src=\"" + image_location + "ToolboxImages/DRAG1PG.ICO" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></li>");
			builder.AppendLine("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"\" onclick=\"javascript:ChangeMouseCursor(" + qc_item.Web.Static_PageCount + "); return false;\"><img src=\"" + image_location + "ToolboxImages/thumbnail_large.gif" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></li>");
			builder.AppendLine("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"\" onclick=\"javascript:ResetCursorToDefault(" + qc_item.Web.Static_PageCount + "); return false;\"><img src=\"" + image_location + "ToolboxImages/Point13.ICO" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></li>");

			if (thumbnailSize == 3)
				builder.Append("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_large.ico\"/></a></li>");
			else
			{
				CurrentMode.Size_Of_Thumbnails = 3;
				builder.Append("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_large.ico\"/></a></li>");
			}

			if (thumbnailSize == 2)
				builder.Append("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_medium.ico\"/></a></li>");
			else
			{
				CurrentMode.Size_Of_Thumbnails = 2;
				builder.Append("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_medium.ico\"/></a></li>");
			}

			if (thumbnailSize == 1)
				builder.Append("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_small.ico\"/></a></li>");
			else
			{
				CurrentMode.Size_Of_Thumbnails = 1;
				builder.Append("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_small.ico\"/></a></li>");
			}


			//Reset the current mode
			CurrentMode.Size_Of_Thumbnails = (short) thumbnailSize;



			builder.AppendLine("<li class=\"action-qc-menu-item\" style=\"float:right;\" ><a href=\"\" onclick=\"javascript:behaviors_save_form(); return false;\"><img src=\"" + image_location + "ToolboxImages/Save.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\"/></a></li>");




			builder.AppendLine("</ul>");




			////Add the nav row QC image icons
			//builder.AppendLine("<span id=\"qcIconsTopNavRow\" class=\"spanQCIconsTopNavRow\">");
			//builder.AppendLine("&nbsp;&nbsp;&nbsp;");
			//if (String.IsNullOrEmpty(autosave_option.ToString()) || (autosave_option))
			//    builder.AppendLine("<span><a id=\"autosaveLink\" href=\"\" onclick=\"javascript:changeAutoSaveOption(); return false;\">Turn Off Autosave</a>");
			//else
			//    builder.AppendLine("<span><a id=\"autosaveLink\" href=\"\" onclick=\"javascript:changeAutoSaveOption(); return false;\">Turn On Autosave</a>");
			//builder.AppendLine("</span>");

			builder.AppendLine("</div>");

            builder.AppendLine("<script>");
            builder.AppendLine("    $(function() {");
            builder.AppendLine("       $('#qc-main-menu').superfish({ autoArrows:  true });");
            builder.AppendLine("    });");
            builder.AppendLine("</script>");
            builder.AppendLine();
		}

		#endregion


		/// <summary> Stream to which to write the HTML for this subwriter  </summary>
		/// <param name="Output"> Response stream for the item viewer to write directly to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("QC_ItemViewer.Write_Main_Viewer_Section", "");
			}

			int images_per_page = thumbnailsPerPage;
			int size_of_thumbnails = thumbnailSize;

			//Get the current QC page number
			int current_qc_viewer_page_num = 1;
			if (CurrentMode.ViewerCode.Replace("qc", "").Length > 0)
				Int32.TryParse(CurrentMode.ViewerCode.Replace("qc", ""), out current_qc_viewer_page_num);

			// Get the links for the METS
			string greenstoneLocation = qc_item.Web.Source_URL + "/";
			string complete_mets = greenstoneLocation + qc_item.BibID + "_" + qc_item.VID + ".mets.xml";

			// MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
			if ((qc_item.Behaviors.Dark_Flag) || (qc_item.Behaviors.IP_Restriction_Membership > 0))
			{
				complete_mets = CurrentMode.Base_URL + "files/" + qc_item.BibID + "/" + qc_item.VID + "/" + qc_item.BibID + "_" + qc_item.VID + ".mets.xml";
			}

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;
			ushort current_view_page = CurrentMode.Page;


			Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new page, etc..) -->");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_behaviors_request\" name=\"QC_behaviors_request\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_affected_file\" name=\"QC_affected_file\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"Main_Thumbnail_Index\" name=\"Main_Thumbnail_Index\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"Autosave_Option\" name=\"Autosave_Option\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_move_relative_position\" name=\"QC_move_relative_position\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_move_destination\" name=\"QC_move_destination\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_Sortable\" name=\"QC_Sortable\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"autonumber_mode_from_form\" name=\"autonumber_mode_from_form\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"Autonumber_number_system\" name=\"Autonumber_number_system\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"Autonumber_text_without_number\" name=\"Autonumber_text_without_number\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"Autonumber_number_only\" name=\"Autonumber_number_only\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"Autonumber_last_filename\" name=\"Autonumber_last_filename\" value=\"\"/>");
			

			// Start the citation table
			Output.WriteLine( "\t\t<!-- QUALITY CONTROL VIEWER OUTPUT -->" );
			if (qc_item.Web.Static_PageCount < 100)
			{
                
				Output.WriteLine("\t\t<td align=\"left\" height=\"40px\" ><span class=\"SobekViewerTitle\"><b>" + translator.Get_Translation(title, CurrentMode.Language) + "</b></span></td></tr>");
				Output.WriteLine("\t<tr>") ;
			}
			Output.WriteLine("\t\t<td>" );

			// Start the main div for the thumbnails
	
			ushort page = (ushort)(CurrentMode.Page - 1);
			if (page > (qc_item.Web.Static_PageCount - 1) / images_per_page)
				page = (ushort)((qc_item.Web.Static_PageCount - 1) / images_per_page);

			//Outer div which contains all the thumbnails
			Output.WriteLine("<div id=\"allThumbnailsOuterDiv1\" align=\"center\" style=\"margin:5px;\" class=\"qcContainerDivClass\"><span id=\"allThumbnailsOuterDiv\" align=\"left\" style=\"float:left\" class=\"doNotSort\">");

			List<abstract_TreeNode> static_pages = qc_item.Divisions.Physical_Tree.Pages_PreOrder;
			// Step through each page in the item
			Division_TreeNode lastParent = null;
			for (int page_index = page * images_per_page; (page_index < (page + 1) * images_per_page) && (page_index < static_pages.Count); page_index++)
			{
				Page_TreeNode thisPage = (Page_TreeNode) static_pages[page_index];
				Division_TreeNode thisParent = childToParent[thisPage];
				
				// Find the jpeg image
				foreach (SobekCM_File_Info thisFile in thisPage.Files.Where(thisFile => thisFile.System_Name.IndexOf(".jpg") > 0))
				{
					// Get the image URL
					CurrentMode.Page = (ushort) (page_index + 1);
					CurrentMode.ViewerCode = (page_index + 1).ToString();

					//set the image url to fetch the small thumbnail .thm image
					string image_url = (qc_item.Web.Source_URL + "/" + thisFile.System_Name.Replace(".jpg", "thm.jpg")).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

					//If thumbnail size selected is large, get the full-size jpg image
					if (size_of_thumbnails == 2 || size_of_thumbnails == 3 || size_of_thumbnails == 4)
						image_url = (qc_item.Web.Source_URL + "/" + thisFile.System_Name).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
					string url = CurrentMode.Redirect_URL().Replace("&", "&amp;").Replace("\"", "&quot;");

					// Start the box for this thumbnail
					Output.WriteLine("<a name=\"" + thisPage.Label + "\" id=\"" + thisPage.Label + "\"></a>");
					Output.WriteLine("<span id=\"span" + page_index + "\" onclick=\"PickMainThumbnail(this.id);\" align=\"left\" style=\"display:inline-block;\" onmouseover=\"this.className='thumbnailHighlight'; showQcPageIcons(this.id); showErrorIcon(this.id);\" onmouseout=\"this.className='thumbnailNormal'; hideQcPageIcons(this.id); hideErrorIcon(this.id);\" >");
					Output.WriteLine("<div class=\"qcpage\" align=\"center\" id=\"parent" + image_url + "\" >");
					Output.WriteLine("<table>");

					// Add the name of the file
					string filename_sans_extension = thisFile.File_Name_Sans_Extension;
					Output.WriteLine("<tr><td class=\"qcfilename\" align=\"left\"><input type=\"hidden\" id=\"filename" + page_index + "\" name=\"filename" + page_index + "\" value=\"" + filename_sans_extension + "\" />" + filename_sans_extension + "</td>");
									  
					//Determine the error icon size, main-thumbnail-selected icon size based on the current thumbnail size 
					int error_icon_height = 20;
					int error_icon_width = 20;
					int pick_main_thumbnail_height = 20;
					int pick_main_thumbnail_width = 20;
					int arrow_height = 12;
					int arrow_width = 15;
					switch (size_of_thumbnails)
					{
						case 2:
							error_icon_height = 25;
							error_icon_width = 25;
							pick_main_thumbnail_height = 25;
							pick_main_thumbnail_width = 25;
							arrow_height = 17;
							arrow_width = 20;
							break;

						case 3:
							error_icon_height = 30;
							error_icon_width = 30;
							pick_main_thumbnail_height = 30;
							pick_main_thumbnail_width = 30;
							arrow_height = 22;
							arrow_width = 25;
							break;

						case 4:
							error_icon_height = 30;
							error_icon_width = 30;
							pick_main_thumbnail_height = 30;
							pick_main_thumbnail_width = 30;
							arrow_height = 22;
							arrow_width = 25;
							break;

						default:
							error_icon_height = 20;
							error_icon_height = 20;
							pick_main_thumbnail_height = 20;
							pick_main_thumbnail_width = 20;
							arrow_height = 12;
							arrow_width = 15;
							break;
					}

					//Add the checkbox for moving this thumbnail
					Output.WriteLine("<td><span ><input type=\"checkbox\" id=\"chkMoveThumbnail" + page_index + "\" name=\"chkMoveThumbnail" + page_index + "\" class=\"chkMoveThumbnailHidden\" onchange=\"chkMoveThumbnailChanged(this.id, "+qc_item.Web.Static_PageCount+")\"/></span>");
					Output.WriteLine("<span id=\"movePageArrows" + page_index + "\" class=\"movePageArrowIconHidden\"><a id=\"form_qcmove_link_left\" href=\"http://ufdc.ufl.edu/l/technical/javascriptrequired\" onclick=\"var b=popup('form_qcmove', 'form_qcmove_link', 280, 400 ); update_popup_form('" + thisFile.File_Name_Sans_Extension + "','Before'); return b\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT02.ICO\" height=\"" + arrow_height + "\" width=\"" + arrow_width + "\" alt=\"Missing Icon Image\"></img></a>");
					Output.WriteLine("<a id=\"form_qcmove_link2\" href=\"http://ufdc.ufl.edu/l/technical/javascriptrequired\" onclick=\"var b=popup('form_qcmove', 'form_qcmove_link', 280, 400 ); update_popup_form('" + thisFile.File_Name_Sans_Extension + "','After'); return b\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT04.ICO\" height=\"" + arrow_height + "\" width=\"" + arrow_width + "\" alt=\"Missing Icon Image\"></img></span>");

					//Add the error icon
		//			Output.WriteLine("<span id=\"error" + page_index + "\" class=\"errorIconSpan\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/Cancel.ico\" height=\"" + error_icon_height + "\" width=\"" + error_icon_width + "\" alt=\"Missing Icon Image\"></img></span>");
					int main_thumbnail_index = -1;
					if (!String.IsNullOrEmpty(hidden_main_thumbnail))
						Int32.TryParse(hidden_main_thumbnail, out main_thumbnail_index);
					//Add the pick_main_thumbnail icon
					if (main_thumbnail_index >= 0 && main_thumbnail_index == page_index && hidden_request != "unpick_main_thumbnail")
						Output.WriteLine("<span id=\"spanImg" + page_index + "\" class=\"pickMainThumbnailIconSelected\"><img id=\"pick_main_thumbnail" + page_index + "\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/thumbnail_large.gif\" height=" + pick_main_thumbnail_height + " width=" + pick_main_thumbnail_width + "/></span></td></tr>");
					else
						Output.WriteLine("<span id=\"spanImg" + page_index + "\" class=\"pickMainThumbnailIcon\"><img id=\"pick_main_thumbnail" + page_index + "\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/thumbnail_large.gif\" height=" + pick_main_thumbnail_height + " width=" + pick_main_thumbnail_width + "/></span></td></tr>");

					// Add the anchor for jumping to the file?
					Output.Write("<tr><td colspan=\"2\"><a id=\"" + page_index+ "\" href=\"" + url + "\" target=\"_blank\">");

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
							Output.Write("<img id=\"child" + image_url + "\"  src=\"" + image_url + "\" width=\"315px\" height=\"50%\" alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\" />");
							division_box = "divisionbox_medium";
							pagination_box = "pagebox_medium";
							icon_width = 20;
							icon_height = 20;
							num_spaces = 3;
							break;

						case 3:
							Output.WriteLine("<img id=\"child" + image_url + "\" src=\"" + image_url + "\" width=\"472.5px\" height=\"75%\" alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\" />");
							division_box = "divisionbox_large";
							pagination_box = "pagebox_large";
							icon_width = 20;
							icon_height = 20;
							num_spaces = 4;
							break;

						case 4:
							Output.WriteLine("<img id=\"child" + image_url + "\" src=\"" + image_url + "\"  alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\" />");
							division_box = "divisionbox_full";
							pagination_box = "pagebox_full";
							icon_width = 25;
							icon_height = 25;
							num_spaces = 4;
							break;

						default:
							Output.WriteLine("<img  src=\"" + image_url + "\" alt=\"MISSING THUMBNAIL\" class=\"qcthumbnails\"/>");
							division_box = "divisionbox_small";
							pagination_box = "pagebox_small";
							division_text = "D:";
							pagination_text = "Page:";
							break;
					}
								   
					Output.WriteLine("</a></td></tr>");

					// Add the text box for entering the name of this page
					Output.WriteLine("<tr><td class=\"paginationtext\" align=\"left\">" + pagination_text + "</td>");
					Output.WriteLine("<td><input type=\"text\" id=\"textbox" + page_index + "\" name=\"textbox" + page_index + "\" class=\"" + pagination_box + "\" value=\"" + thisPage.Label + "\" onchange=\"PaginationTextChanged(this.id,"+autonumber_mode+"," + qc_item.Web.Static_PageCount +");\"></input></td></tr>");

					// Was this a new parent?
					bool newParent = thisParent != lastParent;

					// Add the Division prompting, and the check box for a new division
					Output.Write("<tr><td class=\"divisiontext\" align=\"left\">" + division_text);
					Output.Write("<input type=\"checkbox\" id=\"newDivType" + page_index + "\" name=\"newdiv" + page_index + "\" value=\"new\" onclick=\"UpdateDivDropdown(this.name, " +  qc_item.Web.Static_PageCount + ");\"");
					if ( newParent )
						Output.Write(" checked=\"checked\"");
					Output.WriteLine("/></td>");

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
						Output.WriteLine("<td><select id=\"selectDivType" + page_index + "\" name=\"selectDivType" + page_index + "\" class=\"" + division_box + "\" onchange=\"DivisionTypeChanged(this.id," + qc_item.Web.Static_PageCount + ");\">");
					else
					{
						Output.WriteLine("<td><select id=\"selectDivType" + page_index + "\" name=\"selectDivType" + page_index + "\" class=\"" + division_box + "\" disabled=\"disabled\" onchange=\"DivisionTypeChanged(this.id," + qc_item.Web.Static_PageCount + ");\">");
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
							Output.WriteLine("<option value=\"" + divisionType.Key + "\" selected=\"selected\">" + divisionType.Key + "</option>");
						else if (divisionType.Key == parentType && divisionType.Value == true)
						{
							Output.WriteLine("<option value=\"!" + divisionType.Key + "\" selected=\"selected\">" + divisionType.Key + "</option>");
							txtDivNameCssClass = "txtNamedDivVisible";
						}
						else if (divisionType.Value == true)
							Output.WriteLine("<option value=\"!" + divisionType.Key + "\">" + divisionType.Key + "</option>");
						else
							Output.WriteLine("<option value=\"" + divisionType.Key + "\">" + divisionType.Key + "</option>");
					}

					Output.WriteLine("</select></td></tr>");

					//Add the textbox for named divisions

					if (newParent)
					{
						Output.WriteLine("<tr id=\"divNameTableRow" + page_index + "\" class=\"" + txtDivNameCssClass + "\"><td class=\"namedDivisionText\" align=\"left\">" + division_name_text + "</td>");
						Output.WriteLine("<td><input type=\"text\" id=\"txtDivName" + page_index + "\" name=\"txtDivName" + page_index + "\" class=\"" + pagination_box + "\" value=\"" + HttpUtility.HtmlEncode(parentLabel) + "\" onchange=\"DivNameTextChanged(this.id," + qc_item.Web.Static_PageCount + ");\"/></td></tr>");
					}
					else
					{
						Output.WriteLine("<tr id=\"divNameTableRow" + page_index + "\" class=\"" + txtDivNameCssClass + "\"><td class=\"namedDivisionText\" align=\"left\">" + division_name_text + "</td>");
						Output.WriteLine("<td><input type=\"text\" disabled=\"disabled\" id=\"txtDivName" + page_index + "\" name=\"txtDivName" + page_index + "\" class=\"" + pagination_box + "\" value=\"" + HttpUtility.HtmlEncode(parentLabel) + "\" onchange=\"DivNameTextChanged(this.id," + qc_item.Web.Static_PageCount + ");\"/></td></tr>");
					}
					//Add the span with the on-hover-options for the page thumbnail
					Output.WriteLine("<tr><td colspan=\"100%\">");
					Output.WriteLine("<span id=\"qcPageOptions"+page_index+"\" class=\"qcPageOptionsSpan\" style=\"float:right\"><img src=\""+CurrentMode.Base_URL+"default/images/ToolboxImages/Main_Information.ICO\" height=\""+icon_height+"\" width=\""+icon_width+"\" alt=\"Missing Icon Image\"></img>");
					
					//Add spaces between icons based on the current thumbnail size 
					for (int i = 0; i < num_spaces; i++) { Output.WriteLine("&nbsp;"); }
					Output.WriteLine("<a href=\"" + url + "\" target=\"_blank\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/View.ico\" height=\"" + icon_height + "\" width=\"" + icon_width + "\" alt=\"Missing Icon Image\"></img></a>");
					for (int i = 0; i < num_spaces; i++) { Output.WriteLine("&nbsp;"); }

					Output.WriteLine("<img class=\"qc_toolboximage\" onClick=\"return ImageDeleteClicked('" + filename_sans_extension + "');\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/TRASH01.ICO\" height=\"" + icon_height + "\" width=\"" + icon_width + "\" alt=\"Missing Icon Image\"></img>");


					//for (int i = 0; i < num_spaces; i++) { Output.WriteLine("&nbsp;"); }
					//Output.WriteLine("<img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT02.ICO\" height=\"" + icon_height + "\" width=\"" + icon_width + "\" alt=\"Missing Icon Image\"></img>");
					//for (int i = 0; i < num_spaces; i++) { Output.WriteLine("&nbsp;"); }
					//Output.WriteLine("<img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT04.ICO\" height=\"" + icon_height + "\" width=\"" + icon_width + "\" alt=\"Missing Icon Image\"></img>");

					Output.WriteLine("</span>");
					Output.WriteLine("</td></tr>");

					// Finish this one division
					Output.WriteLine("</table></div></span>");
					Output.WriteLine();
					break;
				}

				// Save the last parent
				lastParent = thisParent;

			}


			//Close the outer div
			Output.WriteLine("</span></div>");

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;
			CurrentMode.Page = current_view_page;

			// Finish the citation table
			Output.WriteLine("\t\t</td>");
			Output.WriteLine("\t\t<!-- END QUALITY CONTROL VIEWER OUTPUT -->");

			//If the current url has an anchor, call the javascript function to animate the corresponding span background color
            Output.WriteLine("<script type=\"text/javascript\">addLoadEvent(MakeSpanFlashOnPageLoad());</script>");
			if(makeSortable)
                Output.WriteLine("<script type=\"text/javascript\">addLoadEvent(MakeSortable1());</script>");


			//If the autosave option is not set, or set to true, set the interval (3 minutes) for autosaving
			if(String.IsNullOrEmpty(autosave_option.ToString()) || autosave_option)
			  Output.WriteLine("<script type=\"text/javascript\">setInterval(qc_auto_save, 180* 1000);</script>");

			//Display the time the form was last saved
			object timeSaved = HttpContext.Current.Session["QC_timeUpdated"];
			string displayTimeText = (timeSaved==null) ? String.Empty : "Saved at " + timeSaved.ToString();

			//Add the Complete and Cancel buttons at the end of the form
			Output.WriteLine("</tr><tr><td colspan=\"100%\">");
            //Output.WriteLine("<span id=\"displayTimeSaved\" class=\"displayTimeSaved\" style=\"float:left\">" + displayTimeText + "</span>");
            //Start inner table
            Output.WriteLine("<span style=\"float:right\"><table style=\"width=\"100%\"><tr>");
            Output.WriteLine("<td>Comments: </td><td><textarea cols=\"50\" id=\"txtComments\" name=\"txtComments\"></textarea></td> ");
            Output.WriteLine("<td><button type=\"button\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/check.ico\" width=\"25\" height=\"25\"/>Complete</button></td>");
			Output.WriteLine("<td><button type=\"button\" onclick=\"behaviors_cancel_form();\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/Cancel.ico\" width=\"25\" height=\"25\" />Cancel</button></td>");
		    //Close inner table
		    Output.WriteLine("</tr></table></span>");
            Output.WriteLine("</td></tr>");
		}
	   
		/// <summary>
		/// Add the Viewer specific information to the top navigation row
		/// This nav row adds the different thumbnail viewing options(# of thumbnails, size of thumbnails, list of all related item thumbnails)
		/// </summary>
		public override string NavigationRow
		{
			get
			{
				// Build the value
				StringBuilder navRowBuilder = new StringBuilder(5000);

				//Start building the top nav bar
				navRowBuilder.AppendLine("\t\t<!-- QUALITY CONTROL VIEWER TOP NAV ROW -->");

				//Include the js files
				navRowBuilder.AppendLine("<script language=\"JavaScript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_related_items.js\"></script>");
	            navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.color-2.1.1.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_qc.js\"></script>");
			   navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.timers.min.js\"></script>");
			//   navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.min.js\"></script>");
				
			  navRowBuilder.AppendLine("<link rel=\"stylesheet\" href=\"http://code.jquery.com/ui/1.10.1/themes/base/jquery-ui.css\" />");
			//  navRowBuilder.AppendLine("<script src=\"http://code.jquery.com/ui/1.10.1/jquery-ui.js\"></script>");

			   //Include the superfish.css file for the menu
				navRowBuilder.AppendLine("<link rel=\"stylesheet\" media=\"screen\"  href=\"" + CurrentMode.Base_URL + "default/superfish.css\">");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\""+CurrentMode.Base_URL+"default/scripts/superfish/superfish.js\"></script>");
				navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/superfish/hoverIntent.js\"></script>");
			  navRowBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

				// shift+click checkboxes
  
				navRowBuilder.AppendLine("<script type=\"text/javascript\">$(document).ready(function() {$(function() {$(\"input[name^='chkMoveThumbnail']\").shiftClick();});");
				navRowBuilder.AppendLine("(function($) {$.fn.shiftClick = function() {var lastSelected;var checkBoxes = $(this);this.each(function() {$(this).click(function(ev) {if (ev.shiftKey) {var MaxPageCount = "+qc_item.Web.Static_PageCount+";var spanArrayObjects = new Array();if(window.spanArrayGlobal!= null){ spanArrayObjects = window.spanArrayGlobal;}else{for(var j=0;j<MaxPageCount;j++){spanArrayObjects[j]='span'+j;}}var spanArray=new Array();for(var k=0;k<spanArrayObjects.length;k++){spanArray[k]=spanArrayObjects[k].split('span')[1];} var last = checkBoxes.index(lastSelected);var first = checkBoxes.index(this);var thisID = (this.id).split('chkMoveThumbnail')[1];var lastID = (lastSelected.id).split('chkMoveThumbnail')[1];var thisIndex = spanArray.indexOf(thisID);");
				navRowBuilder.AppendLine("var lastIndex = spanArray.indexOf(lastID); var start = Math.min(thisIndex, lastIndex);var end = Math.max(thisIndex, lastIndex);var chk = lastSelected.checked;for (var i = start; i < end; i++) {document.getElementById('chkMoveThumbnail'+(spanArray[i])).checked = chk;}var atLeastOneSelected=false;if($('body').css('cursor').indexOf(\"move_pages_cursor\")>-1){for(var i=0;i<MaxPageCount; i++){if(document.getElementById('chkMoveThumbnail'+i).checked)atLeastOneSelected=true;}if(!(atLeastOneSelected)){document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';for(var i=0; i<MaxPageCount; i++){if(document.getElementById('movePageArrows'+i))document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';}");
				navRowBuilder.AppendLine("}else{document.getElementById('divMoveOnScroll').className='qcDivMoveOnScroll';for(var i=0; i<MaxPageCount; i++){if(document.getElementById('movePageArrows'+i))document.getElementById('movePageArrows'+i).className = 'movePageArrowIconVisible';}}}} else {lastSelected = this;}})});};})(jQuery);});");
				navRowBuilder.AppendLine("</script>");

				//end shift+click checkboxes


			  // Add the popup form
			  navRowBuilder.AppendLine();
			  navRowBuilder.AppendLine("<!-- Pop-up form for moving page(s) by selecting the checkbox in image -->");
			  navRowBuilder.AppendLine("<div class=\"qcmove_popup_div\" id=\"form_qcmove\" style=\"display:none;\">");
			  navRowBuilder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">MOVE SELECTED PAGES</td><td align=\"right\"><a href=\"" + CurrentMode.Base_URL + "logon/help\" target=\"_FORM_QCMOVE_HELP\" >?</a> &nbsp; <a href=\"#template\" onclick=\" popdown( 'form_qcmove' ); \">X</a> &nbsp; </td></tr></table></div>");
			  navRowBuilder.AppendLine("  <br />");
			  navRowBuilder.AppendLine("  <table class=\"popup_table\">");

			  // Add the rows of data
				navRowBuilder.AppendLine("<tr><td>Move selected pages:</td>");
				navRowBuilder.AppendLine("<td><input type=\"radio\" name=\"rbMovePages\" id=\"rbMovePages1\" value=\"After\" checked=\"true\" onclick=\"rbMovePagesChanged(this.value);\">After");
				navRowBuilder.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;");
				navRowBuilder.AppendLine("<td><select id=\"selectDestinationPageList1\" name=\"selectDestinationPageList1\">");
				//Add the select options
				
				//iterate through the page items
				if (qc_item.Web.Static_PageCount > 0)
				{
					int page_index = 0;
					foreach (Page_TreeNode thisFile in qc_item.Web.Pages_By_Sequence)
					{
						page_index++;

						navRowBuilder.AppendLine("<option value=\"" + thisFile.Files[0].File_Name_Sans_Extension + "\">" + thisFile.Files[0].File_Name_Sans_Extension + "</option>");

					}
				}

				navRowBuilder.AppendLine("</td></tr>");
				navRowBuilder.AppendLine("<tr><td></td><td><input type=\"radio\" name=\"rbMovePages\" id=\"rbMovePages2\" value=\"Before\" onclick=\"rbMovePagesChanged(this.value);\">Before</td>");
	
				navRowBuilder.AppendLine("<td><select id=\"selectDestinationPageList2\"  disabled=\"true\">");

				//iterate through the page items
				if (qc_item.Web.Static_PageCount > 0)
				{
					int page_index = 0;
					foreach (Page_TreeNode thisFile in qc_item.Web.Pages_By_Sequence)
					{
						page_index++;

						navRowBuilder.AppendLine("<option value=\"" + thisFile.Files[0].File_Name_Sans_Extension + "\">" + thisFile.Files[0].File_Name_Sans_Extension + "</option>");

					}
				}
				navRowBuilder.AppendLine("</select></td></tr>");

			 //Add the Cancel & Move buttons
				navRowBuilder.AppendLine("    <tr><td colspan=\"2\"><center>");
				navRowBuilder.AppendLine("      <br><a href=\"\" onclick=\"move_pages_submit();\"><input type=\"image\" src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/buttons/move_big_button.gif\" value=\"Submit\" alt=\"Submit\" /></a>&nbsp;");
				navRowBuilder.AppendLine("      <a href=\"#template\" onclick=\" popdown( 'form_qcmove' );\"><img border=\"0\" src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/buttons/cancel1_big_button.gif\" alt=\"CANCEL\" /></a><br> ");
				navRowBuilder.AppendLine("    </center></td></tr>");

			  // Finish the popup form
			  navRowBuilder.AppendLine("  </table>");
			  navRowBuilder.AppendLine("  <br />");
			  navRowBuilder.AppendLine("</div>");
			  navRowBuilder.AppendLine();

			  add_main_menu(navRowBuilder);

			  navRowBuilder.AppendLine("<div id=\"divMoveOnScroll\" class=\"qcDivMoveOnScrollHidden\"><button type=\"button\" id=\"btnMovePages\" name=\"btnMovePages\" class=\"btnMovePages\" onclick=\"return popup('form_qcmove', 'btnMovePages', 280, 400 );\">Move to</button></div>");
			  //Add the button to delete pages
				navRowBuilder.AppendLine("<div id=\"divDeleteMoveOnScroll\" class=\"qcDivDeleteButtonHidden\"><button type=\"button\" id=\"btnDeletePages\" name=\"btn DeletePages\" class=\"btnDeletePages\" onclick=\"DeleteSelectedPages();\" >Delete</button></div>" );
		
				// Finish the nav row controls
				navRowBuilder.AppendLine("\t\t<!-- END QUALITY CONTROL VIEWER NAV ROW -->");

				// Return the html string
				return navRowBuilder.ToString();

			}
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
				thumbnailsPerPage = CurrentUser.Get_Option("QC_ItemViewer:ThumbnailsPerPage", qc_item.Web.Static_PageCount);

				// Or was there a new value in the URL?
				if (CurrentMode.Thumbnails_Per_Page >= -1)
				{
					CurrentUser.Add_Option("QC_ItemViewer:ThumbnailsPerPage", CurrentMode.Thumbnails_Per_Page);
					thumbnailsPerPage = CurrentMode.Thumbnails_Per_Page;
				}
			}
			else
			{
				int tempValue = qc_item.Web.Static_PageCount;
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

            // Get the autonumbering mode
            if (CurrentUser != null)
            {
                // First, pull the autonumbering mode from the user options
                autonumber_mode = CurrentUser.Get_Option("QC_ItemViewer:AutonumberingMode", 0);

                // Or was there a new value in the URL?
                if (CurrentMode.Autonumbering_Mode > -1)
                {
                    CurrentUser.Add_Option("QC_ItemViewer:AutonumberingMode", CurrentMode.Autonumbering_Mode);
                    autonumber_mode = CurrentMode.Autonumbering_Mode;
                }
            }
            else
            {
                int tempValue = 0;
                object sessionValue = HttpContext.Current.Session["QC_ItemViewer:AutonumberingMode"];
                if (sessionValue != null)
                {
                    tempValue = Int32.Parse(sessionValue.ToString());

                }
                autonumber_mode = tempValue;

                // Or was there a new value in the URL?
                if (CurrentMode.Autonumbering_Mode > -1)
                {
                    HttpContext.Current.Session["QC_ItemViewer:AutonumberingMode"] = CurrentMode.Autonumbering_Mode;
                    autonumber_mode = CurrentMode.Autonumbering_Mode;
                }
            }
            //Now reset the current mode value since we won't need to set it again
            CurrentMode.Autonumbering_Mode = -1;





			// Now, build a list from child node to parent node
			childToParent = new Dictionary<Page_TreeNode, Division_TreeNode>();
			foreach (abstract_TreeNode rootNode in qc_item.Divisions.Physical_Tree.Roots)
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

        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "qc_set_fullscreen();"));
            Body_Attributes.Add(new Tuple<string, string>("onresize", "qc_set_fullscreen();"));
        }

		/// <summary> Write any additional values within the HTML Head of the final served page </summary>
		/// <param name="Output"> Output stream currently within the HTML head tags </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
		{
			Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_QC.css\" /> ");
		}

		/// <summary> Gets the collection of special behaviors which this item viewer
		/// requests from the main HTML subwriter. </summary>
		public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
		{
			get
			{
				return new List<HtmlSubwriter_Behaviors_Enum>
					{
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode,
						HtmlSubwriter_Behaviors_Enum.Suppress_Footer,
						HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header,
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination,
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu,
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar
					};
			}
		}

		#region Support for Roman Numerals

		/// <summary> Converts an integer to a roman number, in either upper or lower case. Default returned in lowercase. </summary>
		/// <param name="number">Integer number</param>
		/// <returns>Roman numeral after conversion</returns>
		public string NumberToRoman(int number)
		{
			string resultWithCase;
			//Set up the key-values
			int[] values=new int[]{1000,900,500,400,100,90,50,40,10,9,5,4,1};
			string[] numerals = new string[]{"M","CM","D","CD","C","XC","L","XL","X","IX","V","IV","I"};
			

			//Initialize the string builder
			StringBuilder result=new StringBuilder();

			for (int i = 0; i < 13; i++)
			{
				//If 'number' is less than the test value, append the corresponding numeral or numeral pair to the resultant string
				while (number >= values[i])
				{
					number -= values[i];
					result.Append(numerals[i]);
				}
			}
			if (isLower == false)
				resultWithCase = result.ToString().ToUpper();
			else resultWithCase = result.ToString().ToLower();

			return resultWithCase;
		}


		/// <summary> Converts a roman numeral to a decimal number </summary>
		/// <param name="roman">Roman numeral (string)</param>
		/// <returns>Corresponding decimal number(integer)</returns>
		public int RomanToNumber(string roman)
		{
			isLower = false;
			try
			{
				//Check if the roman numeral is in upper or lower case
				for (int i = 0; i < roman.Length; i++)
				{
					if (char.IsLower(roman[i]))
						isLower = true;
				}
				roman = roman.ToUpper().Trim();
				if (roman.Split('V').Length > 2 || roman.Split('L').Length > 2 || roman.Split('D').Length > 2)
					throw new ArgumentException("Rule 4 violated");

				//Rule 1-single letter may be repeated upto 3 times consecutively
				int count = 1;
				char last = 'Z';
				foreach (char numeral in roman)
				{
					//Valid character?
					if ("IVXLCDM".IndexOf(numeral) == -1)
						throw new ArgumentException("Invalid Roman Numeral");

					//Check if a numeral has been repeated more than thrice
					if (numeral == last)
					{
						count++;
						if (count == 4)
							throw new ArgumentException("Rule 1 violated - numeral repeated more than 3 times");
					}
					else
					{
						count = 1;
						last = numeral;
					}
				}

				//Create an arraylist containing the values
				int ptr = 0;
				ArrayList values = new ArrayList();
				int maxDigit = 1000;

				while (ptr < roman.Length)
				{
					//Get the base vaue of the numeral
					char numeral = roman[ptr];
					int digit = (int) Enum.Parse(typeof (RomanDigit), numeral.ToString());

					//Check for Rule 3-Subtractive combination
					if (digit > maxDigit)
					{
						throw new ArgumentException("Rule 3 violated");
					}

					//Get the next digit
					int nextDigit = 0;
					if (ptr < roman.Length - 1)
					{
						char nextNumeral = roman[ptr + 1];
						nextDigit = (int) Enum.Parse(typeof (RomanDigit), nextNumeral.ToString());

						if (nextDigit > digit)
						{
							if ("IXC".IndexOf(numeral) == -1 || nextDigit > (digit*10) || roman.Split(numeral).Length > 3)
								throw new ArgumentException("Rule 3 violated");

							maxDigit = digit - 1;
							digit = nextDigit - digit;
							ptr++;
						}
					}
					values.Add(digit);
					//next digit
					ptr++;
				}
				//Check for rule 5 (Going left to right, the value must increase from one letter to the next)
				for (int i = 0; i < values.Count - 1; i++)
				{
					if ((int) values[i] < (int) values[i + 1])
						throw new ArgumentException("Not a valid roman number: Rule 5 violated");
				}
				//Rule 2
				int total = 0;

				foreach (int digit in values)
				{
					total += digit;
				}

				return total;

			}
			catch (Exception e)
			{
				return -1;
			}

		}

		/// <summary> Map the roman numeral digits with their integer equivalents </summary>
		public enum RomanDigit
		{
			/// <summary> Roman numeral I </summary>
			I = 1,
			/// <summary> Roman numeral V </summary>
			V = 5,
			/// <summary> Roman numeral X </summary>
			X = 10,
			/// <summary> Roman numeral L </summary>
			L = 50,
			/// <summary> Roman numeral C </summary>
			C = 100,
			/// <summary> Roman numeral D </summary>
			D = 500,
			/// <summary>Roman numeral M </summary>
			M = 1000
		};

		#endregion


		protected class QC_Viewer_Page_Division_Info
		{
			public string Page_Label { get; set; }

			public string Filename { get; set; }

			public bool New_Division { get; set; }

			public string Division_Type { get; set;  }
	
			public string Division_Label { get; set; }

			public Page_TreeNode METS_STructMap_Page_Node { get; set; }

			public bool Checkbox_Selected { get; set;  }

			public QC_Viewer_Page_Division_Info()
			{
				Checkbox_Selected = false;
			}
		}
	}
}
