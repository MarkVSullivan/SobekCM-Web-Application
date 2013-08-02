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
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.Items;
using SobekCM.Library.MemoryMgmt;
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
		private string mainThumbnailFileName_from_db;
		private string mainJPGFileName_from_db;
		private string notes;
		private bool clear_pagination;
		private bool clear_and_reorder_pagination;
		private SobekCM_Item qc_item;
		private DataRow Item_Detail;

		private Dictionary<Page_TreeNode, Division_TreeNode> childToParent;
		private QualityControl_Profile qc_profile;

		private string metsInProcessFile;

	    private int allThumbnailsOuterDiv1Width;
        private int allThumbnailsOuterDiv1Height;

		/// <summary> Constructor for a new instance of the QC_ItemViewer class </summary>
		public QC_ItemViewer(SobekCM_Item Current_Object, User_Object Current_User, SobekCM_Navigation_Object Current_Mode)
		{
			// Save the current user and current mode information (this is usually populated AFTER the constructor completes, 
			// but in this case (QC viewer) we need the information for early processing
			CurrentMode = Current_Mode;
			CurrentUser = Current_User;

			//Assign the current resource object to qc_item
			qc_item = Current_Object;
 
			// If there is no user, send to the login
			if (CurrentUser == null)
			{
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
				CurrentMode.Redirect();
				return;
			}

			// If the user cannot edit this item, go back
			if (!CurrentUser.Can_Edit_This_Item(Current_Object))
			{
				CurrentMode.ViewerCode = String.Empty;
				CurrentMode.Redirect();
				return;
			}

			// Get the links for the METS
			string greenstoneLocation = Current_Object.Web.Source_URL + "/";
			complete_mets = greenstoneLocation + Current_Object.BibID + "_" + Current_Object.VID + ".mets.xml";

			// MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
			if ((Current_Object.Behaviors.Dark_Flag) || (Current_Object.Behaviors.IP_Restriction_Membership > 0))
			{
				complete_mets = CurrentMode.Base_URL + "files/" + qc_item.BibID + "/" + qc_item.VID + "/" + qc_item.BibID + "_" + qc_item.VID + ".mets.xml";
			}


			// Get the special qc_item, which matches the passed in Current_Object, at least the first time.
			// If the QC work is already in process, we may find a temporary METS file to read.

			// Determine the in process directory for this
            userInProcessDirectory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + Current_User.UserName.Replace(".", "").Replace("@", "") + "\\qcwork\\" + qc_item.METS_Header.ObjectID;
			if (Current_User.UFID.Trim().Length > 0)
                userInProcessDirectory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + Current_User.UFID + "\\qcwork\\" + qc_item.METS_Header.ObjectID;

			// Make the folder for the user in process directory
			if (!Directory.Exists(userInProcessDirectory))
				Directory.CreateDirectory(userInProcessDirectory);

			// Create the name for the tempoary METS file?
			metsInProcessFile = userInProcessDirectory + "\\" + Current_Object.BibID + "_" + Current_Object.VID + ".mets.xml";

			// Is this work in the user's SESSION state?
			qc_item = HttpContext.Current.Session[Current_Object.BibID + "_" + Current_Object.VID + " QC Work"] as SobekCM_Item;
			if (qc_item == null)
			{
				// Is there a temporary METS for this item, which is not expired?
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

            // If this was a post-back keep the required height and width for the qc area
            allThumbnailsOuterDiv1Width = -1;
		    allThumbnailsOuterDiv1Height = -1;
            string temp_width = HttpContext.Current.Request.Form["QC_window_width"] ?? String.Empty;
            string temp_height = HttpContext.Current.Request.Form["QC_window_height"] ?? String.Empty;
            if ((temp_width.Length > 0) && (temp_height.Length > 0))
            {
                // Parse the values and save to the session
                if (Int32.TryParse(temp_width, out allThumbnailsOuterDiv1Width))
                    HttpContext.Current.Session["QC_AllThumbnailsWidth"] = allThumbnailsOuterDiv1Width;
                if (Int32.TryParse(temp_height, out allThumbnailsOuterDiv1Height))
                    HttpContext.Current.Session["QC_AllThumbnailsHeight"] = allThumbnailsOuterDiv1Height;

            }
            else
            {
                object session_width = HttpContext.Current.Session["QC_AllThumbnailsWidth"];
                if (session_width != null)
                    allThumbnailsOuterDiv1Width = (int) session_width;
                object session_height = HttpContext.Current.Session["QC_AllThumbnailsHeight"];
                if (session_height != null)
                    allThumbnailsOuterDiv1Height = (int) session_height;
            }

			// See if there were hidden requests
			hidden_request = HttpContext.Current.Request.Form["QC_behaviors_request"] ?? String.Empty;
		    hidden_main_thumbnail = HttpContext.Current.Request.Form["Main_Thumbnail_File"] ?? String.Empty;
			hidden_move_relative_position = HttpContext.Current.Request.Form["QC_move_relative_position"] ?? String.Empty;
			hidden_move_destination_fileName = HttpContext.Current.Request.Form["QC_move_destination"] ?? String.Empty;
			autonumber_number_system = HttpContext.Current.Request.Form["Autonumber_number_system"] ?? String.Empty;
			string temp = HttpContext.Current.Request.Form["autonumber_mode_from_form"] ?? "0";
			Int32.TryParse(temp, out autonumber_mode_from_form);
			autonumber_text_only = HttpContext.Current.Request.Form["Autonumber_text_without_number"] ?? String.Empty;
			autonumber_number_only = HttpContext.Current.Request.Form["Autonumber_number_only"] ?? String.Empty;
			autonumber_number_system = HttpContext.Current.Request.Form["Autonumber_number_system"] ?? String.Empty;
			hidden_autonumber_filename = HttpContext.Current.Request.Form["Autonumber_last_filename"] ?? String.Empty;
		    string tempSortable = HttpContext.Current.Request.Form["QC_sortable_option"] ?? String.Empty;
		    if (tempSortable == "false") makeSortable = false;


			//Get any notes/comments entered by the user
			notes = HttpContext.Current.Request.Form["txtComments"] ?? String.Empty;

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

				if (HttpContext.Current.Session["autosave_option"] != null)
                    autosaveCache = bool.TryParse(HttpContext.Current.Session["autosave_option"].ToString(), out autosaveCacheValue);
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
					HttpContext.Current.Session["autosave_option"] = autosave_option;
				}
			}
			catch (Exception e)
			{
				throw new ApplicationException("Error retrieving auto save option. " + e.Message);
			}

			// Check for a previously set main thumbnail, or one from the requesting form
			if (!String.IsNullOrEmpty(hidden_main_thumbnail))
			{
    			HttpContext.Current.Session["main_thumbnail_" + qc_item.BibID + "_" + qc_item.VID] = hidden_main_thumbnail;
			}
            else if (HttpContext.Current.Session["main_thumbnail_" + qc_item.BibID + "_" + qc_item.VID] == null )
            {
                hidden_main_thumbnail = qc_item.Behaviors.Main_Thumbnail.Replace("thm.jpg", "");
                HttpContext.Current.Session["main_thumbnail_" + qc_item.BibID + "_" + qc_item.VID] = hidden_main_thumbnail;
            }
            else
            {
                hidden_main_thumbnail = HttpContext.Current.Session["main_thumbnail_" + qc_item.BibID + "_" + qc_item.VID].ToString();
            }

            // Perform any requested actions
            switch (hidden_request)
            {
                case "autosave":
                case "save":
                case "complete":
                    //Save the current time
				    HttpContext.Current.Session["QC_timeUpdated"] = DateTime.Now.ToString("hh:mm tt");

				    // Read the data from the http form, perform all requests, and
				    // update the qc_item (also updates the session and temporary files)
				    // Save this updated information in the temporary folder's METS file for reading later if necessary.
				    if ((Save_From_Form_Request_To_Item(String.Empty, String.Empty)) && (( hidden_request == "save" ) || ( hidden_request == "complete")))
				    {
					    // If the user selected SAVE or COMPLETE, roll out the new version
                        Move_Temp_Changes_To_Production();

					    // Redirect differently depending on SAVE or COMPLETE
					    if (hidden_request == "save")
					    {
						    // Forward back to the QC form 
						    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
						    HttpContext.Current.ApplicationInstance.CompleteRequest();
						    CurrentMode.Request_Completed = true;
					    }
					    else if (hidden_request == "complete")
					    {
						    // Forward to the item
						    CurrentMode.ViewerCode = String.Empty;
						    CurrentMode.Redirect();
					    }
				    }
                    break;

                case "cancel":
                    Cancel_Current_QC();

				    // Forward back to the default item view
				    CurrentMode.ViewerCode = String.Empty;
				    CurrentMode.Redirect();
                    break;

                case "clear_pagination":
                    ClearPagination();
                    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
				    HttpContext.Current.ApplicationInstance.CompleteRequest();
				    CurrentMode.Request_Completed = true;
                    break;

                case "clear_reorder":
                    Clear_Pagination_And_Reorder_Pages();
                    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
				    HttpContext.Current.ApplicationInstance.CompleteRequest();
				    CurrentMode.Request_Completed = true;
                    break;

                case "delete_page":
                    // Read the data from the http form, perform all requests, and
                    // update the qc_item (also updates the session and temporary files)
                    string filename_to_delete = HttpContext.Current.Request.Form["QC_affected_file"] ?? String.Empty;
                    if (Save_From_Form_Request_To_Item(String.Empty, filename_to_delete))
                    {
                        Delete_Resource_File(filename_to_delete);
                    }

                    // Since we deleted a page, we need to roll out our new version
                    Move_Temp_Changes_To_Production();

                    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    CurrentMode.Request_Completed = true;
                    break;

                case "delete_selected_pages":
                    // Read the data from the http form, perform all requests, and
                    // update the qc_item (also updates the session and temporary files)
                    List<QC_Viewer_Page_Division_Info> selected_page_div_from_form;
                    if (Save_From_Form_Request_To_Item(String.Empty, String.Empty, out selected_page_div_from_form))
                    {
                        foreach (QC_Viewer_Page_Division_Info thisPage in selected_page_div_from_form)
                        {
                            Delete_Resource_File(thisPage.METS_StructMap_Page_Node.Files[0].File_Name_Sans_Extension);
                        }
                    }

                    // Since we deleted a page, we need to roll out our new version
                    Move_Temp_Changes_To_Production();

                    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    CurrentMode.Request_Completed = true;
                    break;

                case "move_selected_pages":
				    // Read the data from the http form, perform all requests, and
				    // update the qc_item (also updates the session and temporary files)
				    Save_From_Form_Request_To_Item(hidden_move_destination_fileName, String.Empty);

				    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
				    HttpContext.Current.ApplicationInstance.CompleteRequest();
				    CurrentMode.Request_Completed = true;
                    break;
            }
		}

        #region Perform pre-display work ( retrieving user settings and build child to parent dictionary )

        /// <summary> This provides an opportunity for the viewer to perform any pre-display work
        /// which is necessary before entering any of the rendering portions </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Perform_PreDisplay_Work(Custom_Tracer Tracer)
        {
            //Get the item details from the DB
            Item_Detail = SobekCM_Database.Get_Item_Information(CurrentItem.BibID, CurrentItem.VID, Tracer);

            if (Item_Detail != null)
            {
                //get the main thumbnail & JPEG filenames from the database
                mainThumbnailFileName_from_db = Item_Detail["MainThumbnailFile"].ToString();
                mainJPGFileName_from_db = Item_Detail["MainJPEGFile"].ToString();

                //Get the filenames without the extensions
                int length_thumbnail = mainThumbnailFileName_from_db.IndexOf("thm.jpg");
                int lengthJpeg = mainJPGFileName_from_db.IndexOf(".jpg");

                if (length_thumbnail > 0)
                    mainThumbnailFileName_from_db = mainThumbnailFileName_from_db.Substring(0, length_thumbnail);

                if (lengthJpeg > 0)
                    mainJPGFileName_from_db = mainJPGFileName_from_db.Substring(0, lengthJpeg);

            }

            // Get the proper number of thumbnails per page
            // First, pull the thumbnails per page from the user options
            thumbnailsPerPage = CurrentUser.Get_Option("QC_ItemViewer:ThumbnailsPerPage", qc_item.Web.Static_PageCount);

            // Or was there a new value in the URL?
            if (CurrentMode.Thumbnails_Per_Page >= -1)
            {
                CurrentUser.Add_Option("QC_ItemViewer:ThumbnailsPerPage", CurrentMode.Thumbnails_Per_Page);
                thumbnailsPerPage = CurrentMode.Thumbnails_Per_Page;

                // Now, reset the value in the navigation object, since we won't need to set it again
                CurrentMode.Thumbnails_Per_Page = -100;
            }

            // -1 means to display all thumbnails
            if (thumbnailsPerPage == -1)
                thumbnailsPerPage = int.MaxValue;

            // Get the proper size of thumbnails per page
            // First, pull the thumbnails per page from the user options
            thumbnailSize = CurrentUser.Get_Option("QC_ItemViewer:ThumbnailSize", 1);

            // Or was there a new value in the URL?
            if (CurrentMode.Size_Of_Thumbnails > -1)
            {
                CurrentUser.Add_Option("QC_ItemViewer:ThumbnailSize", CurrentMode.Size_Of_Thumbnails);
                thumbnailSize = CurrentMode.Size_Of_Thumbnails;

                //Now reset the current mode value since we won't need to set it again
                CurrentMode.Size_Of_Thumbnails = -1;
            }

            // Get the autonumbering mode
            // First, pull the autonumbering mode from the user options
            autonumber_mode = CurrentUser.Get_Option("QC_ItemViewer:AutonumberingMode", 0);

            // Or was there a new value in the URL?
            if (CurrentMode.Autonumbering_Mode > -1)
            {
                CurrentUser.Add_Option("QC_ItemViewer:AutonumberingMode", CurrentMode.Autonumbering_Mode);
                autonumber_mode = CurrentMode.Autonumbering_Mode;

                //Now reset the current mode value since we won't need to set it again
                CurrentMode.Autonumbering_Mode = -1;
            }

            // Ensure there are no pages directly under the item
            List<abstract_TreeNode> add_to_new_main = new List<abstract_TreeNode>();
            foreach (abstract_TreeNode rootNode in qc_item.Divisions.Physical_Tree.Roots)
            {
                if (rootNode.Page)
                {
                    add_to_new_main.Add(rootNode);
                }
            }
            if (add_to_new_main.Count > 0)
            {
                Division_TreeNode newMain = new Division_TreeNode("Main", String.Empty);
                qc_item.Divisions.Physical_Tree.Roots.Add(newMain);
                foreach( abstract_TreeNode thisNode in add_to_new_main )
                    newMain.Add_Child(thisNode);
            }


            // Now, build a list from child node to parent node
            childToParent = new Dictionary<Page_TreeNode, Division_TreeNode>();
            foreach (abstract_TreeNode rootNode in qc_item.Divisions.Physical_Tree.Roots)
            {
                if (!rootNode.Page)
                {
                    recurse_through_and_find_child_parent_relationship((Division_TreeNode) rootNode);
                }
                else
                {
                    
                }
            }

            // Save the qc item as the main current item
            CurrentItem = qc_item;
        }

        private void recurse_through_and_find_child_parent_relationship(Division_TreeNode ParentNode)
        {
            foreach (abstract_TreeNode childNode in ParentNode.Nodes)
            {
                if (childNode.Page)
                {
                    childToParent[(Page_TreeNode)childNode] = ParentNode;
                }
                else
                {
                    recurse_through_and_find_child_parent_relationship((Division_TreeNode)childNode);
                }
            }
        }

        #endregion

        #region Event handler for user selecting CANCEL 

        /// <summary> Cancel the current QC, delete temporary files, and remove from cache </summary>
        private void Cancel_Current_QC()
        {
            // Delete all temporary files and cache
            if (Directory.Exists(userInProcessDirectory))
            {
                string[] tempFiles = Directory.GetFiles(userInProcessDirectory);
                foreach (string tempFile in tempFiles)
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch
                    {
                        // Do nothing in this case (not a fatal error)
                    }
                }

                // Delete the folder
                try
                {
                    Directory.Delete(userInProcessDirectory);
                }
                catch
                {
                    // Do nothing in this case (not a fatal error)
                }
            }

            // Clear the updated item from the session
            HttpContext.Current.Session[qc_item.BibID + "_" + qc_item.VID + " QC Work"] = null;
            HttpContext.Current.Session["main_thumbnail_" + qc_item.BibID + "_" + qc_item.VID] = null;

        }

        #endregion

        #region Event handler for user CLEARING ALL PAGINATION AND REORDERING PAGES 

	    /// <summary>
	    /// Clears all the page labels, division types and names, and reorders the pages by filename
	    /// </summary>
	    private void Clear_Pagination_And_Reorder_Pages()
	    {
	        SortedDictionary<string, Page_TreeNode> nodeToFilename = new SortedDictionary<string, Page_TreeNode>();

	        // Add each page node to a sorted list/dictionary and clear the label
	        foreach (Page_TreeNode thisNode in qc_item.Divisions.Physical_Tree.Pages_PreOrder)
	        {
	            thisNode.Label = String.Empty;
	            string file_sans = thisNode.Files[0].File_Name_Sans_Extension;
	            nodeToFilename[file_sans] = thisNode;
	        }

	        // Clear the physical (TOC) tree
	        qc_item.Divisions.Physical_Tree.Clear();

	        // Add the main node to the physical (TOC) division tree 
	        Division_TreeNode mainNode = new Division_TreeNode("Main", String.Empty);
	        qc_item.Divisions.Physical_Tree.Roots.Add(mainNode);

	        // Add back each page, in order by filename (sans extension)
	        for (int i = 0; i < nodeToFilename.Count; i++)
	        {
	            mainNode.Add_Child(nodeToFilename.ElementAt(i).Value);
	        }

	        // Save the updated item to the session
	        HttpContext.Current.Session[qc_item.BibID + "_" + qc_item.VID + " QC Work"] = qc_item;

	        // Save to the temporary QC work section
	        try
	        {
	            // Ensure the directory exists under the user's temporary mySobek InProcess folder
	            if (!Directory.Exists(userInProcessDirectory))
	                Directory.CreateDirectory(userInProcessDirectory);

	            // Save the METS
	            qc_item.Save_METS(metsInProcessFile);
	        }
	        catch (Exception)
	        {
	            throw;
	        }
	    }

	    #endregion

        #region Event handler for user selecing CLEAR PAGINATION

        /// <summary> Clears the pagination names of all the pages of the qc item </summary>
	    private void ClearPagination()
	    {
	        try
	        {
                // Clear all the labels on the pages
	            foreach (abstract_TreeNode thisNode in qc_item.Divisions.Physical_Tree.Pages_PreOrder)
	            {
                    thisNode.Label = String.Empty;
	            }

	            // Save the updated to the session
	            HttpContext.Current.Session[qc_item.BibID + "_" + qc_item.VID + " QC Work"] = qc_item;

	            // Save to the temporary QC work section
	            // Ensure the directory exists under the user's temporary mySobek InProcess folder
	            if (!Directory.Exists(userInProcessDirectory))
	                Directory.CreateDirectory(userInProcessDirectory);

	            // Save the METS
	            qc_item.Save_METS(metsInProcessFile);
	        }
	        catch (Exception e)
	        {
	            throw new Exception("Error clearing all pagination names" + e.Message);
	        }
	    }

        #endregion

        #region Save all the page/division info from the form to the temporary METS

	    /// <summary> Save all the data from form post-back into the item in memory, and 
	    /// return all the page information for those pages which are CHECKED (with the checkbox) </summary>
	    /// <returns> Returns TRUE if successful, otherwise FALSE </returns>
	    private bool Save_From_Form_Request_To_Item(string FilenameToMoveAfter, string FilenameToOmit)
	    {
	        List<QC_Viewer_Page_Division_Info> selected_page_div_from_form;
	        return Save_From_Form_Request_To_Item(FilenameToMoveAfter, FilenameToOmit, out selected_page_div_from_form);
	    }

	    /// <summary> Save all the data from form post-back into the item in memory, and 
		/// return all the page information for those pages which are CHECKED (with the checkbox) </summary>
		/// <returns> Returns TRUE if successful, otherwise FALSE </returns>
        private bool Save_From_Form_Request_To_Item(string FilenameToMoveAfter, string FilenameToOmit, out List<QC_Viewer_Page_Division_Info> Selected_Page_Div_From_Form)
		{
			bool returnValue = true;

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
			    autonumber_mode = autonumber_mode_from_form;
			    CurrentMode.Autonumbering_Mode = autonumber_mode_from_form;
				bool reached_last_page = false;
				bool reached_next_div = false;
				int number = 0;
				if (autonumber_number_system == "decimal")
					number = Int32.Parse(autonumber_number_only) + 1;
				else if (autonumber_number_system == "roman")
					number = RomanToNumber(autonumber_number_only) + 1;

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
								reached_last_page = true;
							}
								//if the last page displayed on the screen has been reached
							else if (reached_last_page == true)
							{
								//Mode "0": Autonumber all pages of current division
								//Mode "1": Autonumber all pages of the entire document
								if ((autonumber_mode_from_form == 0 && reached_next_div == false) || (autonumber_mode_from_form == 1))
								{
									if (autonumber_number_system == "decimal")
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
					else if (reached_last_page)
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
					Page_TreeNode thisPage = (Page_TreeNode) thisNode;
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
					lastDivision = (Division_TreeNode) thisNode;
				}
			}

			// Step through and collect all the form data
			List<QC_Viewer_Page_Division_Info> page_div_from_form = new List<QC_Viewer_Page_Division_Info>();
			List<Page_TreeNode> existing_pages_in_window = new List<Page_TreeNode>();

			//Get the list of pages to be moved
			Selected_Page_Div_From_Form = new List<QC_Viewer_Page_Division_Info>();

			try
			{
				// Now, step through each of the pages in the return
				string[] keysFromForm = HttpContext.Current.Request.Form.AllKeys;
				foreach (string thisKey in keysFromForm)
				{
					// Has this gotten to the next page?
					if ((thisKey.IndexOf("filename") == 0) && (thisKey.Length > 8))
					{
                        // Create the qc viewer page information, and assign the filename
						QC_Viewer_Page_Division_Info thisInfo = new QC_Viewer_Page_Division_Info {Filename = HttpContext.Current.Request.Form[thisKey]};

					    // Get the index to use for all the other keys
						string thisIndex = thisKey.Substring(8);

						// Get the page name 
						thisInfo.Page_Label = HttpContext.Current.Request.Form["textbox" + thisIndex];

						// Was this page selected with the checkbox?  (for bulk delete or move)
						//Get this info only if the move/delete operations are explicitly triggered
						if (hidden_request == "delete_page" || hidden_request == "delete_selected_pages" || hidden_request == "move_selected_pages")
						{
							if ((HttpContext.Current.Request.Form["chkMoveThumbnail" + thisIndex] != null) || (thisInfo.Filename == FilenameToOmit))
							{
								thisInfo.Checkbox_Selected = true;
                                Selected_Page_Div_From_Form.Add(thisInfo);
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
						remnant_pages.Add((Page_TreeNode) window_last_division.Nodes[i]);
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
						pageInfo.METS_StructMap_Page_Node = thisPage;
					}

					// Were we involved in a mass move, in which case we are looking for the insertion point?
					if ((FilenameToMoveAfter.Length > 0) && (move_into_division_index < 0) && (pageInfo.Filename == FilenameToMoveAfter))
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
                if ((FilenameToMoveAfter.Length > 0) && (Selected_Page_Div_From_Form.Count > 0))
				{
					// TODO: Check for the lack of any divisions what-so-ever within the METS.  If so, add one.

					Division_TreeNode divNodeToInsertWithin = null;

					// Get the division
					if (move_into_division_index >= 0)
						divNodeToInsertWithin = (Division_TreeNode) qc_item.Divisions.Physical_Tree.Roots[move_into_division_index];
					else if (FilenameToMoveAfter == "[BEFORE FIRST]")
					{
						divNodeToInsertWithin = (Division_TreeNode) qc_item.Divisions.Physical_Tree.Roots[0];
						move_into_node_index = 0;
					}

					if (divNodeToInsertWithin != null)
					{
						// Insert each page in order
                        foreach (QC_Viewer_Page_Division_Info insertPage in Selected_Page_Div_From_Form)
						{
							divNodeToInsertWithin.Nodes.Insert(move_into_node_index++, insertPage.METS_StructMap_Page_Node);
						}
					}
				}

				// Save the updated to the session
				HttpContext.Current.Session[qc_item.BibID + "_" + qc_item.VID + " QC Work"] = qc_item;

				// Save to the temporary QC work section

				// Ensure the directory exists under the user's temporary mySobek InProcess folder
				if (!Directory.Exists(userInProcessDirectory))
					Directory.CreateDirectory(userInProcessDirectory);

				// Save the METS
				qc_item.Save_METS(metsInProcessFile);

				// Determine the total size of the package before saving
				string[] all_files_final = Directory.GetFiles(userInProcessDirectory);
				double size = all_files_final.Aggregate<string, double>(0, (current, thisFile) => current + (((new FileInfo(thisFile)).Length)/1024));
				qc_item.DiskSize_MB = size;
			}
			catch (Exception ee)
			{
				returnValue = false;
			}

			// Return the flag indicating success
			return returnValue;
		}

        #endregion

        #region Move the temporary changes into production (on SAVE or COMPLETE)

        /// <summary> Move all the changes accumulates in the temporary METS into production
        /// and clear the temporary files/cache so it can be rebuilt from production if needed  </summary>
	    private void Move_Temp_Changes_To_Production()
	    {
	        if (File.Exists(metsInProcessFile))
	        {
	            string resource_directory = SobekCM_Library_Settings.Image_Server_Network + qc_item.Web.AssocFilePath;
                string backup_directory = SobekCM_Library_Settings.Image_Server_Network + qc_item.Web.AssocFilePath + SobekCM_Library_Settings.Backup_Files_Folder_Name;

	            // Ensure the backup directory exists
	            if (!Directory.Exists(backup_directory))
	                Directory.CreateDirectory(backup_directory);

	            // Get the last write date on the current METS
                string current_mets = resource_directory + qc_item.METS_Header.ObjectID + ".mets.xml";
	            if (File.Exists(current_mets)) // SHOULD EXIST!
	            {
	                // Get the last write time 
	                DateTime lastWriteTime = (new FileInfo(current_mets)).LastWriteTime;

	                // Determine the name of the new backup METS
                    string backup_mets_name = backup_directory + "\\" + qc_item.METS_Header.ObjectID + "_" + lastWriteTime.Year + "_" + lastWriteTime.Month.ToString().PadLeft(2, '0') + "_" + lastWriteTime.Day.ToString().PadLeft(2, '0') + ".mets.bak";
	                if (File.Exists(backup_mets_name))
	                {
	                    int version = 1;
	                    do
	                    {
	                        version++;
                            backup_mets_name = backup_directory + "\\" + qc_item.METS_Header.ObjectID + "_" + lastWriteTime.Year + "_" + lastWriteTime.Month.ToString().PadLeft(2, '0') + "_" + lastWriteTime.Day.ToString().PadLeft(2, '0') + "_v" + version.ToString().PadLeft(2, '0') + ".mets.bak";
	                    } while (File.Exists(backup_mets_name));
	                }

	                // Copy the existing mets to the backup spot
	                File.Copy(current_mets, backup_mets_name);
	            }

	            // Copy the inprocess METS into the production digital resource directory
	            File.Copy(metsInProcessFile, current_mets, true);
	        }

	        // Delete all temporary files and cache
	        if (Directory.Exists(userInProcessDirectory))
	        {
	            string[] tempFiles = Directory.GetFiles(userInProcessDirectory);
	            foreach (string tempFile in tempFiles)
	            {
	                try
	                {
	                    File.Delete(tempFile);
	                }
	                catch
	                {
                        // Repress exception, not a fatal error
	                }
	            }

	            // Delete the folder
	            try
	            {
	                Directory.Delete(userInProcessDirectory);
	            }
	            catch
	            {
                    // Repress exception, not a fatal error
	            }
	        }

            //Save changes to the DB
            Resource_Object.Database.SobekCM_Database.QC_Update_Item_Info(qc_item.BibID, qc_item.VID, CurrentUser.UserName, hidden_main_thumbnail + "thm.jpg", hidden_main_thumbnail + ".jpg", 1, 1, 1, notes);

	        // Clear the updated item from the session
	        HttpContext.Current.Session[qc_item.BibID + "_" + qc_item.VID + " QC Work"] = null;
            HttpContext.Current.Session["main_thumbnail_" + qc_item.BibID + "_" + qc_item.VID] = null;

	        // Clear the cache for this item completely, so the system will recreate the object from the new METS
            Cached_Data_Manager.Remove_Digital_Resource_Object(qc_item.BibID, qc_item.VID, null);
	    }

        #endregion

        #region Delete a resource file from the resource folder

	    private void Delete_Resource_File(string FilenameToDelete)
	    {
	        string resource_directory = SobekCM_Library_Settings.Image_Server_Network + qc_item.Web.AssocFilePath;
            string[] files = Directory.GetFiles(resource_directory, FilenameToDelete + ".*");
	        string recycle_bin = SobekCM_Library_Settings.Recycle_Bin + "\\" + qc_item.METS_Header.ObjectID;
	        if (!Directory.Exists(recycle_bin))
	            Directory.CreateDirectory(recycle_bin);

	        foreach (string thisFile in files)
	        {
	            FileInfo thisFileInfo = new FileInfo(thisFile);
	            string extension = thisFileInfo.Extension.ToUpper();
	            if ((extension == ".JPG") || (extension == ".TIF") || (extension == ".JP2") || (extension == ".TXT") || (extension == ".PRO") || (extension == ".TIFF") || (extension == ".JPEG") || (extension == ".GIF") || (extension == ".PNG"))
	            {
                    if ( File.Exists(recycle_bin + "\\" + thisFileInfo.Name))
                        File.Delete(recycle_bin + "\\" + thisFileInfo.Name);
                    File.Move(thisFile, recycle_bin + "\\" + thisFileInfo.Name);
	            }
	        }
            if (File.Exists(resource_directory + "\\" + FilenameToDelete + "thm.jpg"))
	        {
                if (File.Exists(recycle_bin + "\\" + FilenameToDelete + "thm.jpg"))
                    File.Delete(recycle_bin + "\\" + FilenameToDelete + "thm.jpg");
                File.Move(resource_directory + "\\" + FilenameToDelete + "thm.jpg", recycle_bin + "\\" + FilenameToDelete + "thm.jpg");
	        }
	    }

	    #endregion

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

        /// <summary> Height for the main viewer section to adjusted to accomodate this viewer</summary>
        public override int Viewer_Height
        {
            get
            {
                //if (allThumbnailsOuterDiv1Height > 0)
                //{
                //    // Need to accomodate the last row height as well
                //    return allThumbnailsOuterDiv1Height + 38;
                //}
                return -1;
            }
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_QC.css\" /> ");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/scrollbars.css\" />");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/scrollbars-black.css\" />");
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Clear();
            Body_Attributes.Add(new Tuple<string, string>("onload", "qc_set_fullscreen();"));
            Body_Attributes.Add(new Tuple<string, string>("onresize", "qc_set_fullscreen();"));
            
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

        #region Write_Top_Additional_Navigation_Row method (includes main menu)

        /// <summary>Add the Viewer specific information to the top navigation row
	    /// This nav row adds the different thumbnail viewing options(# of thumbnails, size of thumbnails, list of all related item thumbnails)</summary>
	    /// <param name="Output"></param>
	    /// <param name="Tracer"></param>
	    public override void Write_Top_Additional_Navigation_Row(TextWriter Output, Custom_Tracer Tracer)
	    {
	        //Start building the top nav bar
	        Output.WriteLine("\t\t<!-- QUALITY CONTROL VIEWER TOP NAV ROW -->");

	        //Include the js files
	        Output.WriteLine("<script language=\"JavaScript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_related_items.js\"></script>");
	        Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
	        Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.color-2.1.1.js\"></script>");
	        Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_qc.js\"></script>");
	        Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery.timers.min.js\"></script>");
	        Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/superfish/hoverIntent.js\"></script>");
	        Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/superfish/superfish.js\"></script>");
	        Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

	        add_main_menu(Output);

	        // shift+click checkboxes
	        Output.WriteLine("<script type=\"text/javascript\">$(document).ready(function() {$(function() {$(\"input[name^='chkMoveThumbnail']\").shiftClick();});");
	        Output.WriteLine("(function($) {$.fn.shiftClick = function() {var lastSelected;var checkBoxes = $(this);this.each(function() {$(this).click(function(ev) {if (ev.shiftKey) {var MaxPageCount = " + qc_item.Web.Static_PageCount + ";var spanArrayObjects = new Array();if(window.spanArrayGlobal!= null){ spanArrayObjects = window.spanArrayGlobal;}else{for(var j=0;j<MaxPageCount;j++){spanArrayObjects[j]='span'+j;}}var spanArray=new Array();for(var k=0;k<spanArrayObjects.length;k++){spanArray[k]=spanArrayObjects[k].split('span')[1];} var last = checkBoxes.index(lastSelected);var first = checkBoxes.index(this);var thisID = (this.id).split('chkMoveThumbnail')[1];var lastID = (lastSelected.id).split('chkMoveThumbnail')[1];var thisIndex = spanArray.indexOf(thisID);");
	        Output.WriteLine("var lastIndex = spanArray.indexOf(lastID); var start = Math.min(thisIndex, lastIndex);var end = Math.max(thisIndex, lastIndex);var chk = lastSelected.checked;for (var i = start; i < end; i++) {document.getElementById('chkMoveThumbnail'+(spanArray[i])).checked = chk;}var atLeastOneSelected=false;if($('body').css('cursor').indexOf(\"move_pages_cursor\")>-1){for(var i=0;i<MaxPageCount; i++){if(document.getElementById('chkMoveThumbnail'+i).checked)atLeastOneSelected=true;}if(!(atLeastOneSelected)){document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';for(var i=0; i<MaxPageCount; i++){if(document.getElementById('movePageArrows'+i))document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';}");
	        Output.WriteLine("}else{document.getElementById('divMoveOnScroll').className='qcDivMoveOnScroll';for(var i=0; i<MaxPageCount; i++){if(document.getElementById('movePageArrows'+i))document.getElementById('movePageArrows'+i).className = 'movePageArrowIconVisible';}}}} else {lastSelected = this;}})});};})(jQuery);});");
	        Output.WriteLine("</script>");
	        //end shift+click checkboxes

	        Output.WriteLine("<div id=\"divMoveOnScroll\" class=\"qcDivMoveOnScrollHidden\"><button type=\"button\" id=\"btnMovePages\" name=\"btnMovePages\" class=\"btnMovePages\" onclick=\"return popup('form_qcmove');\">Move to</button></div>");

	        //Add the button to delete pages
	        Output.WriteLine("<div id=\"divDeleteMoveOnScroll\" class=\"qcDivDeleteButtonHidden\"><button type=\"button\" id=\"btnDeletePages\" name=\"btn DeletePages\" class=\"btnDeletePages\" onclick=\"DeleteSelectedPages();\" >Delete</button></div>");

	        Output.WriteLine(" <script>");
	        Output.WriteLine("jQuery(document).ready(function () {");
	        Output.WriteLine("jQuery('ul.qc-menu').superfish();");
	        Output.WriteLine("});");
	        Output.WriteLine("</script>");

	        Output.WriteLine("\t\t<!-- END QUALITY CONTROL VIEWER NAV ROW -->");
	    }

        #region Method to add the main menu

        //	private void add_main_menu(StringBuilder builder)
        private void add_main_menu(TextWriter Output)
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
                size_of_thumbnails = Convert.ToInt32(HttpUtility.ParseQueryString(uri.Query).Get("ts"));
                CurrentMode.Size_Of_Thumbnails = (short)size_of_thumbnails;
            }
            else
            {
                CurrentMode.Size_Of_Thumbnails = -1;
            }

            if (uri.Query.IndexOf("an") > 0)
            {
                autonumber_mode = Convert.ToInt32(HttpUtility.ParseQueryString(uri.Query).Get("an"));
                CurrentMode.Autonumbering_Mode = (short)autonumber_mode;
            }
            else
            {
                CurrentMode.Autonumbering_Mode = 0;
            }

            //StringBuilder builder = new StringBuilder(4000);
            Output.WriteLine("<div id=\"qc-menubar\">");

            Output.WriteLine("<div id=\"sbkQc_MenuRightActions\">");

            //Get the icons for the thumbnail sizes
            string image_location = CurrentMode.Default_Images_URL;

            string viewerCode = CurrentMode.ViewerCode;
            CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
            CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;
            string url = CurrentMode.Redirect_URL();
            CurrentMode.Mode = Display_Mode_Enum.Item_Display;
            CurrentMode.ViewerCode = viewerCode;

            Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"\" onclick=\"javascript:UploadNewPageImages('" + url + "');return false;\"><img src=\"" + image_location + "qc_addfiles.png" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\" title=\"Upload new page image files\"/></a></span>");
            Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"\" onclick=\"javascript:behaviors_save_form(); return false;\"><img src=\"" + image_location + "ToolboxImages/Save.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\" title=\"Save the resource and apply your changes\" /></a></span>");

            if (thumbnailSize == 1)
                Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_small.ico\" title=\"Small thumbnails\"/></a></span>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 1;
                Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_small.ico\" title=\"Small thumbnails\"/></a></span>");
            }

            if (thumbnailSize == 2)
                Output.WriteLine("<span class=\"action-qc-menu-item2\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_medium.ico\" title=\"Medium thumbnails\"/></a></span>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 2;
                Output.WriteLine("<span class=\"action-qc-menu-item2\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_medium.ico\" title=\"Medium thumbnails\"/></a></span>");
            }

            if (thumbnailSize == 3)
                Output.WriteLine("<span class=\"action-qc-menu-item2\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_large.ico\" title=\"Large thumbnails\"/></a></span>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 3;
                Output.WriteLine("<span class=\"action-qc-menu-item2\" ><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\"><img src=\"" + image_location + "ToolboxImages/rect_large.ico\" title=\"Large thumbnails\"/></a></span>");
            }

            //Reset the current mode
            CurrentMode.Size_Of_Thumbnails = (short)thumbnailSize;

            Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"\" onclick=\"javascript:ResetCursorToDefault(" + qc_item.Web.Static_PageCount + "); return false;\"><img src=\"" + image_location + "ToolboxImages/Point13.ICO" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\" title=\"Standard cursor\"/></a></span>");
            Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"\" onclick=\"javascript:ChangeMouseCursor(" + qc_item.Web.Static_PageCount + "); return false;\"><img src=\"" + image_location + "ToolboxImages/thumbnail_large.gif" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\" title=\"Choose main Thumbnail\"/></a></span>");
            Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"\" onclick=\"javascript:MovePages(" + qc_item.Web.Static_PageCount + "); return false;left\"><img src=\"" + image_location + "ToolboxImages/DRAG1PG.ICO" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\" title=\"Move multiple pages\"/></a></span>");
            Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"\" onclick=\"javascript:DeletePages(" + qc_item.Web.Static_PageCount + "); return false;\"><img src=\"" + image_location + "ToolboxImages/TRASH01.ICO" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\" title=\"Delete multiple pages\"/></a></span>");
         //   Output.WriteLine("<span class=\"action-qc-menu-item\" ><a href=\"" + complete_mets + "\" target=\"_blank\"><img src=\"" + image_location + "ToolboxImages/mets.ico" + "\" height=\"20\" width=\"20\" alt=\"Missing icon\" title=\"View METS\"></img></a></span>");
            Output.WriteLine("</div>");
            // Add the option to GO TO a certain thumbnail next
            Output.WriteLine("<div id=\"sbkQc_GoToThumbnailDiv\"><span id=\"GoToThumbnailTextSpan\">" + Go_To_Thumbnail + ":</span><select id=\"selectGoToThumbnail\" onchange=\"location=this.options[this.selectedIndex].value; AddAnchorDivEffect_QC(this.options[this.selectedIndex].value);\" /></div>");

            //iterate through the page items
            if (qc_item.Web.Static_PageCount > 0)
            {
                int thumbnail_count = 0;
                foreach (Page_TreeNode thisFile in qc_item.Web.Pages_By_Sequence)
                {
                    thumbnail_count++;
                    string currentPageURL1 = CurrentMode.Redirect_URL((thumbnail_count / thumbnails_per_page + (thumbnail_count % thumbnails_per_page == 0 ? 0 : 1)).ToString() + "qc");
                    string filename = thisFile.Files[0].File_Name_Sans_Extension;
                    string tooltipText = String.Empty;

                    if (filename.Length > 16)
                    {
                        // Are there numbers at the end?
                        if (Char.IsNumber(filename[filename.Length - 1]))
                        {
                            int number_length = 1;
                            while (Char.IsNumber(filename[filename.Length - (1 + number_length)]))
                                number_length++;

                            int characters = 12 - number_length;
                            if (characters < 2)
                                filename = "..." + filename.Substring(filename.Length - Math.Min(number_length, 12));
                            else
                            {
                                tooltipText = filename;
                                filename = filename.Substring(0, characters) + "..." + filename.Substring(filename.Length - number_length);
                            }
                        }
                        else
                        {
                            tooltipText = filename;
                            filename = filename.Substring(0, 12) + "...";
                        }
                    }

                    Output.WriteLine("<option value=\"" + currentPageURL1 + "#" + thisFile.Label + "\" title=\""+tooltipText+"\">" + filename + "</option>");
                }
            }
            Output.WriteLine("</select></div>");
            Output.WriteLine("<ul class=\"qc-menu\">");

            //builder.AppendLine("<li class=\"qc-menu-item\">Resource<ul>");
            //builder.AppendLine("\t<li>Volume Error<ul>");
            //builder.AppendLine("\t\t<li>No volume level error</li>");
            //builder.AppendLine("\t\t<li>Invalid images</li>");
            //builder.AppendLine("\t\t<li>Incorrect volume/title</li>");
            //builder.AppendLine("\t</ul></li>");
            //         Output.WriteLine("\t<li><a href=\"\" onclick=\"javascript:behaviors_save_form(); return false;\">Save</a></li>");


            // Get the checkmarks
            string noCheckmark = "<img src=\"" + image_location + "nocheckmark.png\" alt=\"\" /> ";
            string checkmark = "<img src=\"" + image_location + "checkmark.png\" alt=\"*\" /> ";
            

            Output.WriteLine("<li><a onclick=\"return false;\">Resource</a><ul>");
            Output.WriteLine("\t<li><a href=\"\" onclick=\"return behaviors_save_form();\">Save</a></li>");
            Output.WriteLine("\t<li><a href=\"\" onclick=\"save_submit_form();\">Complete</a></li>");
            Output.WriteLine("\t<li><a href=\"\" onclick=\"behaviors_cancel_form();\">Cancel</a></li>");
            Output.WriteLine("</ul></li>");

            Output.WriteLine("<li><a href=\"#\">Edit</a><ul>");
            Output.WriteLine("\t<li><a href=\"\" onclick=\"javascript:ClearPagination();return false;\">Clear Pagination</a></li>");
            Output.WriteLine("\t<li><a href=\"\" onclick=\"javascript:ClearReorderPagination();return false;\">Clear All &amp; Reorder Pages</a></li>");
            Output.WriteLine("</ul></li>");

            Output.WriteLine("<li><a onclick=\"return false;\">Settings</a><ul>");
            Output.WriteLine("\t<li><a onclick=\"return false;\">Thumbnail Size</a><ul>");
            //Add the thumbnail size options
            if (thumbnailSize == 1)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "Small</a></li>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 1;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "Small</a></li>");
            }

            if (thumbnailSize == 2)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "Medium</a></li>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 2;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "Medium</a></li>");
            }

            if (thumbnailSize == 3)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "Large</a></li>");
            else
            {
                CurrentMode.Size_Of_Thumbnails = 3;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "Large</a></li>");
            }

            //Reset the current mode
            CurrentMode.Size_Of_Thumbnails = (short)thumbnailSize;

            Output.WriteLine("\t</ul></li>");
            Output.WriteLine("\t<li><a onclick=\"return false;\">Thumbnails per page</a><ul>");

            //Add the thumbnails per page options
            if (thumbnailsPerPage == 25)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "25</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 25;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "25</a></li>");
            }
            if (thumbnailsPerPage == 50)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "50</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 50;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "50</a></li>");
            }
            if (thumbnailsPerPage == 100)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "100</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 100;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "100</a></li>");
            }
            if (thumbnailsPerPage == 500)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "500</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 500;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "500</a></li>");
            }
            if (thumbnailsPerPage == 1000)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "1000</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = 1000;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "1000</a></li>");
            }
            if (thumbnailsPerPage == qc_item.Web.Static_PageCount)
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + checkmark + "All thumbnails</a></li>");
            else
            {
                CurrentMode.Thumbnails_Per_Page = (short)qc_item.Web.Static_PageCount;
                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + noCheckmark + "All thumbnails</a></li>");
            }
            //Reset the current mode
            CurrentMode.Thumbnails_Per_Page = (short)thumbnails_per_page;
            Output.WriteLine("\t</ul></li>");


            Output.WriteLine("\t<li><a onclick=\"return false;\">Automatic Numbering</a><ul>");
            if (autonumber_mode == 2)
               // Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + "<img src=\"" + image_location + "checkmark.png\" alt=\"*\" id=\"checkmarkMode2\" /> " + "No automatic numbering</a></li>");
                Output.WriteLine("\t\t<li><a href=\"\" onclick=\"javascript:return Autonumbering_mode_changed(2, '"+image_location+"'); \">" + "<img src=\"" + image_location + "checkmark.png\" alt=\"*\" id=\"checkmarkMode2\" /> " + "No automatic numbering</a></li>");
            else
            {
              //  CurrentMode.Autonumbering_Mode = 2;
                //Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + "<img src=\"" + image_location + "noCheckmark.png\" id=\"checkmarkMode2\" /> " + "No automatic numbering</a></li>");
                Output.WriteLine("\t\t<li><a href=\"\" onclick=\"javascript:Autonumbering_mode_changed(2,'"+image_location+"'); return false;\">" + "<img src=\"" + image_location + "noCheckmark.png\" id=\"checkmarkMode2\" /> " + "No automatic numbering</a></li>");
                
            }
            if (autonumber_mode == 1)
               // Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + "<img src=\"" + image_location + "checkmark.png\" alt=\"*\" id=\"checkmarkMode1\" /> " + "Within same division</a></li>");
                Output.WriteLine("\t\t<li><a href=\"\" onclick=\"javascript:return Autonumbering_mode_changed(1, '" + image_location + "'); \">" + "<img src=\"" + image_location + "checkmark.png\" alt=\"*\" id=\"checkmarkMode1\" /> " + "Within same division</a></li>");
            else
            {
               // CurrentMode.Autonumbering_Mode = 1;
               // Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + "<img src=\"" + image_location + "noCheckmark.png\" id=\"checkmarkMode1\" /> " + "Within same division</a></li>");
                Output.WriteLine("\t\t<li><a href=\"\" onclick=\"javascript:Autonumbering_mode_changed(1,'" + image_location + "'); return false;\">" + "<img src=\"" + image_location + "noCheckmark.png\" id=\"checkmarkMode1\" /> " + "Within same division</a></li>");
            }
            if (autonumber_mode == 0)
               // Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + "<img src=\"" + image_location + "checkmark.png\" alt=\"*\" id=\"checkmarkMode0\" /> " + "Entire document</a></li>");
                Output.WriteLine("\t\t<li><a href=\"\" onclick=\"javascript:return Autonumbering_mode_changed(0, '" + image_location + "'); \">" + "<img src=\"" + image_location + "checkmark.png\" alt=\"*\" id=\"checkmarkMode0\" /> " + "Entire document</a></li>");
            else
            {
              //  CurrentMode.Autonumbering_Mode = 0;
//                Output.WriteLine("\t\t<li><a href=\"" + CurrentMode.Redirect_URL("1qc") + "\">" + "<img src=\"" + image_location + "noCheckmark.png\" id=\"checkmarkMode0\" /> " + "Entire document</a></li>");
                Output.WriteLine("\t\t<li><a href=\"\" onclick=\"javascript:Autonumbering_mode_changed(0,'" + image_location + "'); return false;\">" + "<img src=\"" + image_location + "noCheckmark.png\" id=\"checkmarkMode0\" /> " + "Entire document</a></li>");
            }
            //Reset the mode
            CurrentMode.Autonumbering_Mode = autonumber_mode;
            //Add the options to enable/disable drag & drop of pages
            
            Output.WriteLine("\t</ul></li>");

            Output.WriteLine("\t<li><a onclick=\"return false;\">Drag & Drop Pages</a><ul>");
            if(makeSortable)
            Output.WriteLine("\t<li><a href=\"\" onclick=\"QC_Change_Sortable_Setting(1,'"+image_location+"'); return false;\">"+"<img src=\""+image_location+"checkmark.png\" alt=\"*\" id=\"checkmarkEnableSorting\"/>"+"Enable</a></li>");
            else
              Output.WriteLine("\t<li><a href=\"\" onclick=\"QC_Change_Sortable_Setting(1,'" + image_location + "'); return false;\">" + "<img src=\"" + image_location + "noCheckmark.png\" id=\"checkmarkEnableSorting\"/>" + "Enable</a></li>");
            
            if(!makeSortable)
                Output.WriteLine("\t<li><a href=\"\" onclick=\"QC_Change_Sortable_Setting(0,'"+image_location+"'); return false;\">"+"<img src=\""+image_location+"checkmark.png\" alt=\"*\" id=\"checkmarkDisableSorting\"/>"+"Disable</a></li>");
            else
                Output.WriteLine("\t<li><a href=\"\" onclick=\"QC_Change_Sortable_Setting(0,'" + image_location + "'); return false;\">" + "<img src=\"" + image_location + "noCheckmark.png\" id=\"checkmarkDisableSorting\"/>" + "Disable</a></li>");
                
            Output.WriteLine("</ul></li>");
            
            Output.WriteLine("</ul></li>");
            Output.WriteLine("<li><a onclick=\"return false;\">View</a><ul>");
            Output.WriteLine("\t<li><a href=\"" + complete_mets + "\" target=\"_blank\">View METS</a></li>");
            Output.WriteLine("\t<li><a href=\"" + CurrentMode.Redirect_URL("directory") + "\" target=\"_blank\">View Directory</a></li>");
            Output.WriteLine("\t<li><a href=\"" + CurrentMode.Redirect_URL("tracking") + "\" target=\"_blank\">View QC History</a></li>");
            Output.WriteLine("</ul></li>");
            Output.WriteLine("<li><a onclick=\"return false;\">Help</a></li>");
            Output.WriteLine("</ul>");
            Output.WriteLine("</div>");

            Output.WriteLine();
        }

        #endregion

        #endregion

        #region Write_Main_Viewer_Section method

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

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;
			ushort current_view_page = CurrentMode.Page;

			// Start the citation table
			Output.WriteLine( "\t\t<!-- QUALITY CONTROL VIEWER OUTPUT -->" );

				
			//	Output.WriteLine("\t\t<td align=\"left\" height=\"40px\" ><span class=\"SobekViewerTitle\"><b>" + translator.Get_Translation(title, CurrentMode.Language) + "</b></span></td></tr>");
				Output.WriteLine("\t</tr><tr>") ;
			Output.WriteLine("\t\t<td>" );

			Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new page, etc..) -->");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_behaviors_request\" name=\"QC_behaviors_request\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_affected_file\" name=\"QC_affected_file\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"Main_Thumbnail_File\" name=\"Main_Thumbnail_File\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"Autosave_Option\" name=\"Autosave_Option\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_move_relative_position\" name=\"QC_move_relative_position\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_move_destination\" name=\"QC_move_destination\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"QC_Sortable\" name=\"QC_Sortable\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"autonumber_mode_from_form\" name=\"autonumber_mode_from_form\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"Autonumber_number_system\" name=\"Autonumber_number_system\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"Autonumber_text_without_number\" name=\"Autonumber_text_without_number\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"Autonumber_number_only\" name=\"Autonumber_number_only\" value=\"\"/>");
			Output.WriteLine("<input type=\"hidden\" id=\"Autonumber_last_filename\" name=\"Autonumber_last_filename\" value=\"\"/>");
            Output.WriteLine("<input type=\"hidden\" id=\"QC_window_height\" name=\"QC_window_height\" value=\"\"/>");
            Output.WriteLine("<input type=\"hidden\" id=\"QC_window_width\" name=\"QC_window_width\" value=\"\"/>");
            Output.WriteLine("<input type=\"hidden\" id=\"QC_sortable_option\" name=\"QC_sortable_option\" value=\"\">");

			// Start the main div for the thumbnails
	
			ushort page = (ushort)(CurrentMode.Page - 1);
			if (page > (qc_item.Web.Static_PageCount - 1) / images_per_page)
				page = (ushort)((qc_item.Web.Static_PageCount - 1) / images_per_page);

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

			//Outer div which contains all the thumbnails
            if (( allThumbnailsOuterDiv1Height > 0 ) && ( allThumbnailsOuterDiv1Width > 0 ))
    			Output.WriteLine("<div id=\"allThumbnailsOuterDiv1\" class=\"qcContainerDivClass\" style=\"width:" + allThumbnailsOuterDiv1Width + "px;height:" + allThumbnailsOuterDiv1Height + "px;\"><span id=\"allThumbnailsOuterDiv\" align=\"left\" style=\"float:left\" class=\"doNotSort\">");
            else
            {
                Output.WriteLine("<div id=\"allThumbnailsOuterDiv1\" class=\"qcContainerDivClass\"><span id=\"allThumbnailsOuterDiv\" align=\"left\" style=\"float:left\" class=\"doNotSort\">");
            }

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
				    string filenameToDisplay = filename_sans_extension;
                    //Truncate the filename if too long
                    string tooltipText = String.Empty;

                    if (filename_sans_extension.Length > 16)
                    {
                        // Are there numbers at the end?
                        if (Char.IsNumber(filenameToDisplay[filenameToDisplay.Length - 1]))
                        {
                            int number_length = 1;
                            while (Char.IsNumber(filenameToDisplay[filenameToDisplay.Length - (1 + number_length)]))
                                number_length++;

                            int characters = 8 - number_length;
                            if (characters < 2)
                                filenameToDisplay = "..." + filenameToDisplay.Substring(filenameToDisplay.Length - Math.Min(number_length, 8));
                            else
                            {
                                tooltipText = filenameToDisplay;
                                filenameToDisplay = filenameToDisplay.Substring(0, characters) + "..." + filenameToDisplay.Substring(filenameToDisplay.Length - number_length);
                            }
                        }
                        else
                        {
                            tooltipText = filenameToDisplay;
                            filenameToDisplay = filenameToDisplay.Substring(0, 8) + "...";
                        }
                    }

     //               Output.WriteLine("<option value=\"" + currentPageURL1 + "#" + thisFile.Label + "\" title=\"" + tooltipText + "\">" + filename + "</option>");

                    //End filename truncation



                    Output.WriteLine("<tr><td class=\"qcfilename\" align=\"left\"><input type=\"hidden\" id=\"filename" + page_index + "\" name=\"filename" + page_index + "\" value=\"" + filename_sans_extension + "\" /><span title=\""+tooltipText+"\">" + filenameToDisplay + "</span></td>");

                    //Add the checkbox for moving this thumbnail
                    Output.WriteLine("<td><span ><input type=\"checkbox\" id=\"chkMoveThumbnail" + page_index + "\" name=\"chkMoveThumbnail" + page_index + "\" class=\"chkMoveThumbnailHidden\" onchange=\"chkMoveThumbnailChanged(this.id, " + qc_item.Web.Static_PageCount + ")\"/></span>");
                    Output.WriteLine("<span id=\"movePageArrows" + page_index + "\" class=\"movePageArrowIconHidden\"><a id=\"form_qcmove_link_left\" href=\"http://ufdc.ufl.edu/l/technical/javascriptrequired\" onclick=\"var b=popup('form_qcmove'); update_popup_form('" + thisFile.File_Name_Sans_Extension + "','Before'); return b;\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT02.ICO\" height=\"" + arrow_height + "\" width=\"" + arrow_width + "\" alt=\"Missing Icon Image\"></img></a>");
                    Output.WriteLine("<a id=\"form_qcmove_link2\" href=\"http://ufdc.ufl.edu/l/technical/javascriptrequired\" onclick=\"var b=popup('form_qcmove'); update_popup_form('" + thisFile.File_Name_Sans_Extension + "','After'); return b;\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT04.ICO\" height=\"" + arrow_height + "\" width=\"" + arrow_width + "\" alt=\"Missing Icon Image\"></img></span>");

                    //Add the error icon
                    //			Output.WriteLine("<span id=\"error" + page_index + "\" class=\"errorIconSpan\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/Cancel.ico\" height=\"" + error_icon_height + "\" width=\"" + error_icon_width + "\" alt=\"Missing Icon Image\"></img></span>");
                    int main_thumbnail_index = -1;
                    if (!String.IsNullOrEmpty(hidden_main_thumbnail))
                        Int32.TryParse(hidden_main_thumbnail, out main_thumbnail_index);

                    //Add the main_thumbnail icon
                    if (hidden_main_thumbnail.ToLower() == filename_sans_extension.ToLower())
                        Output.WriteLine("<span id=\"spanImg" + page_index + "\" class=\"pickMainThumbnailIconSelected\"><img id=\"pick_main_thumbnail" + page_index + "\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/thumbnail_large.gif\" height=" + pick_main_thumbnail_height + " width=" + pick_main_thumbnail_width + "/></span></td></tr>");
                    else
                        Output.WriteLine("<span id=\"spanImg" + page_index + "\" class=\"pickMainThumbnailIcon\"><img id=\"pick_main_thumbnail" + page_index + "\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/thumbnail_large.gif\" height=" + pick_main_thumbnail_height + " width=" + pick_main_thumbnail_width + "/></span></td></tr>");


 

                    //// Add the name of the file
                    //string filename_sans_extension = thisFile.File_Name_Sans_Extension;
                    //Output.WriteLine("<span class=\"qcfilename\"><input type=\"hidden\" id=\"filename" + page_index + "\" name=\"filename" + page_index + "\" value=\"" + filename_sans_extension + "\" />" + filename_sans_extension + "</span>");

                    ////Add the checkbox for moving this thumbnail
                    //Output.WriteLine("<span ><input type=\"checkbox\" id=\"chkMoveThumbnail" + page_index + "\" name=\"chkMoveThumbnail" + page_index + "\" class=\"chkMoveThumbnailHidden\" onchange=\"chkMoveThumbnailChanged(this.id, " + qc_item.Web.Static_PageCount + ")\"/></span>");
                    //Output.WriteLine("<span id=\"movePageArrows" + page_index + "\" class=\"movePageArrowIconHidden\"><a id=\"form_qcmove_link_left\" href=\"http://ufdc.ufl.edu/l/technical/javascriptrequired\" onclick=\"var b=popup('form_qcmove'); update_popup_form('" + thisFile.File_Name_Sans_Extension + "','Before'); return b;\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT02.ICO\" height=\"" + arrow_height + "\" width=\"" + arrow_width + "\" alt=\"Missing Icon Image\"></img></a>");
                    //Output.WriteLine("<a id=\"form_qcmove_link2\" href=\"http://ufdc.ufl.edu/l/technical/javascriptrequired\" onclick=\"var b=popup('form_qcmove'); update_popup_form('" + thisFile.File_Name_Sans_Extension + "','After'); return b\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/POINT04.ICO\" height=\"" + arrow_height + "\" width=\"" + arrow_width + "\" alt=\"Missing Icon Image\"></img></span>");

                    ////Add the error icon
                    ////			Output.WriteLine("<span id=\"error" + page_index + "\" class=\"errorIconSpan\"><img src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/Cancel.ico\" height=\"" + error_icon_height + "\" width=\"" + error_icon_width + "\" alt=\"Missing Icon Image\"></img></span>");
                    //int main_thumbnail_index = -1;
                    //if (!String.IsNullOrEmpty(hidden_main_thumbnail))
                    //    Int32.TryParse(hidden_main_thumbnail, out main_thumbnail_index);

                    ////Add the main_thumbnail icon
                    //if (hidden_main_thumbnail.ToLower() == filename_sans_extension.ToLower())
                    //    Output.WriteLine("<span id=\"spanImg" + page_index + "\" class=\"pickMainThumbnailIconSelected\"><img id=\"pick_main_thumbnail" + page_index + "\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/thumbnail_large.gif\" height=" + pick_main_thumbnail_height + " width=" + pick_main_thumbnail_width + "/></span>");
                    //else
                    //    Output.WriteLine("<span id=\"spanImg" + page_index + "\" class=\"pickMainThumbnailIcon\"><img id=\"pick_main_thumbnail" + page_index + "\" src=\"" + CurrentMode.Base_URL + "default/images/ToolboxImages/thumbnail_large.gif\" height=" + pick_main_thumbnail_height + " width=" + pick_main_thumbnail_width + "/></span>");

                    //Output.WriteLine("<table>");

					// Add the anchor for jumping to the file?
					Output.Write("<tr><td colspan=\"2\"><a id=\"" + page_index+ "\" href=\"" + url + "\" target=\"_blank\">");

					// Write the image and determine some values, based on current thumbnail size
					string division_text = "Division:";
					string pagination_text = "Pagination:";
					string division_name_text = "Name:";
				    string division_tooltip_text = "Division";
				    string division_checkbox_tooltip = "Check for the beginning of a new division type";
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
					Output.WriteLine("<td><input type=\"text\" id=\"textbox" + page_index + "\" name=\"textbox" + page_index + "\" class=\"" + pagination_box + "\" value=\"" + Server.HtmlEncode(thisPage.Label) + "\" onchange=\"PaginationTextChanged(this.id,1);\"></input></td></tr>");

					// Was this a new parent?
					bool newParent = thisParent != lastParent;

					// Add the Division prompting, and the check box for a new division
					Output.Write("<tr><td class=\"divisiontext\" align=\"left\"><span title=\"" +division_tooltip_text+"\">"+ division_text+"</span>");
					Output.Write("<span title=\""+division_checkbox_tooltip+"\"><input type=\"checkbox\" id=\"newDivType" + page_index + "\" name=\"newdiv" + page_index + "\" value=\"new\" onclick=\"UpdateDivDropdown(this.id);\"");
					if ( newParent )
						Output.Write(" checked=\"checked\"");
                    Output.WriteLine("/></span></td>");

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
						Output.WriteLine("<td><select id=\"selectDivType" + page_index + "\" name=\"selectDivType" + page_index + "\" class=\"" + division_box + "\" onchange=\"DivisionTypeChanged(this.id);\">");
					else
					{
						Output.WriteLine("<td><select id=\"selectDivType" + page_index + "\" name=\"selectDivType" + page_index + "\" class=\"" + division_box + "\" disabled=\"disabled\" onchange=\"DivisionTypeChanged(this.id);\">");
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
						else if (divisionType.Key == parentType && divisionType.Value )
						{
							Output.WriteLine("<option value=\"!" + divisionType.Key + "\" selected=\"selected\">" + divisionType.Key + "</option>");
							txtDivNameCssClass = "txtNamedDivVisible";
						}
						else if (divisionType.Value )
							Output.WriteLine("<option value=\"!" + divisionType.Key + "\">" + divisionType.Key + "</option>");
						else
							Output.WriteLine("<option value=\"" + divisionType.Key + "\">" + divisionType.Key + "</option>");
					}

					Output.WriteLine("</select></td></tr>");

					//Add the textbox for named divisions
                    if (newParent)
					{
						Output.WriteLine("<tr id=\"divNameTableRow" + page_index + "\" class=\"" + txtDivNameCssClass + "\"><td class=\"namedDivisionText\" align=\"left\">" + division_name_text + "</td>");
						Output.WriteLine("<td><input type=\"text\" id=\"txtDivName" + page_index + "\" name=\"txtDivName" + page_index + "\" class=\"" + pagination_box + "\" value=\"" + HttpUtility.HtmlEncode(parentLabel) + "\" onchange=\"DivNameTextChanged(this.id);\"/></td></tr>");
					}
					else
					{
						Output.WriteLine("<tr id=\"divNameTableRow" + page_index + "\" class=\"" + txtDivNameCssClass + "\"><td class=\"namedDivisionText\" align=\"left\">" + division_name_text + "</td>");
						Output.WriteLine("<td><input type=\"text\" disabled=\"disabled\" id=\"txtDivName" + page_index + "\" name=\"txtDivName" + page_index + "\" class=\"" + pagination_box + "\" value=\"" + HttpUtility.HtmlEncode(parentLabel) + "\" onchange=\"DivNameTextChanged(this.id);\"/></td></tr>");
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

			// Add the popup form

			//      navRowBuilder.AppendLine();
			Output.WriteLine("<!-- Pop-up form for moving page(s) by selecting the checkbox in image -->");
			Output.WriteLine("<div class=\"qcmove_popup_div\" id=\"form_qcmove\" style=\"display:none;\">");
			Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">MOVE SELECTED PAGES</td><td align=\"right\"> &nbsp; <a href=\"#template\" onclick=\" popdown( 'form_qcmove' ); \">X</a> &nbsp; </td></tr></table></div>");
			Output.WriteLine("  <br />");
			Output.WriteLine("  <table class=\"popup_table\">");

			// Add the rows of data
			Output.WriteLine("<tr><td>Move selected pages:</td>");
			Output.WriteLine("<td><input type=\"radio\" name=\"rbMovePages\" id=\"rbMovePages1\" value=\"After\" checked=\"true\" onclick=\"rbMovePagesChanged(this.value);\">After");
			Output.WriteLine("&nbsp;&nbsp;&nbsp;&nbsp;</td>");
			Output.WriteLine("<td><select id=\"selectDestinationPageList1\" name=\"selectDestinationPageList1\">");
			//Add the select options

			//iterate through the page items
			if (qc_item.Web.Static_PageCount > 0)
			{
				foreach (Page_TreeNode thisFile in qc_item.Web.Pages_By_Sequence)
				{
					Output.WriteLine("<option value=\"" + thisFile.Files[0].File_Name_Sans_Extension + "\">" + thisFile.Files[0].File_Name_Sans_Extension + "</option>");
				}
			}

			Output.WriteLine("</td></tr>");
			Output.WriteLine("<tr><td></td><td><input type=\"radio\" name=\"rbMovePages\" id=\"rbMovePages2\" value=\"Before\" onclick=\"rbMovePagesChanged(this.value);\">Before</td>");

			Output.WriteLine("<td><select id=\"selectDestinationPageList2\"  disabled=\"true\">");

			//iterate through the page items
			if (qc_item.Web.Static_PageCount > 0)
			{
				foreach (Page_TreeNode thisFile in qc_item.Web.Pages_By_Sequence)
				{
					Output.WriteLine("<option value=\"" + thisFile.Files[0].File_Name_Sans_Extension + "\">" + thisFile.Files[0].File_Name_Sans_Extension + "</option>");
				}
			}
			Output.WriteLine("</select></td></tr>");

			//Add the Cancel & Move buttons
			Output.WriteLine("    <tr><td colspan=\"3\" style=\"text-align:center\">");
            Output.WriteLine("      <br /><button title=\"Move selected pages\" class=\"qc_movebuttons\" onclick=\"move_pages_submit();return false;\">SUBMIT</button>&nbsp;");
            Output.WriteLine("      <button title=\"Cancel this move\" class=\"qc_movebuttons\" onclick=\"return cancel_move_pages();\">CANCEL</button>&nbsp;<br />");
			Output.WriteLine("    </td></tr>");

			// Finish the popup form
			Output.WriteLine("  </table>");
			Output.WriteLine("  <br />");
			Output.WriteLine("</div>");
			Output.WriteLine();


			// Finish the citation table
			Output.WriteLine("\t\t</td>");
			Output.WriteLine("\t\t<!-- END QUALITY CONTROL VIEWER OUTPUT -->");

			//If the current url has an anchor, call the javascript function to animate the corresponding span background color
			Output.WriteLine("<script type=\"text/javascript\">addLoadEvent(MakeSpanFlashOnPageLoad());</script>");
			Output.WriteLine("<script type=\"text/javascript\">addLoadEvent(Configure_QC(" + qc_item.Web.Static_PageCount + "));</script>");
	//		if(makeSortable)
				Output.WriteLine("<script type=\"text/javascript\">addLoadEvent(MakeSortable1());</script>");


			//If the autosave option is not set, or set to true, set the interval (3 minutes) for autosaving
			if(String.IsNullOrEmpty(autosave_option.ToString()) || autosave_option)
			  Output.WriteLine("<script type=\"text/javascript\">setInterval(qc_auto_save, 180* 1000);</script>");

            //Add the Complete and Cancel buttons at the end of the form
            Output.WriteLine("</tr><tr><td colspan=\"100%\">");
			//Output.WriteLine("<span id=\"displayTimeSaved\" class=\"displayTimeSaved\" style=\"float:left\">" + displayTimeText + "</span>");
			//Start inner table
            Output.WriteLine("<div id=\"sbkQc_BottomRow\">");
			Output.WriteLine("<span id=\"sbkQC_BottomRowTextSpan\">Comments: </span><textarea cols=\"50\" id=\"txtComments\" name=\"txtComments\"></textarea> ");
			Output.WriteLine("<button type=\"button\" class=\"qc_mainbuttons\" onclick=\"save_submit_form();\">Complete</button>");
			Output.WriteLine("<button type=\"button\" class=\"qc_mainbuttons\" onclick=\"behaviors_cancel_form();\">Cancel</button>");
			//Close inner table
			Output.WriteLine("</div>");
			Output.WriteLine("</td></tr>");

			
			////Add button to move multiple pages
			//Output.WriteLine("<div id=\"divMoveOnScroll\" class=\"qcDivMoveOnScrollHidden\"><button type=\"button\" id=\"btnMovePages\" name=\"btnMovePages\" class=\"btnMovePages\" onclick=\"return popup('form_qcmove', 'btnMovePages', 280, 400 );\">Move to</button></div>");
			////Add the button to delete pages
			//Output.WriteLine("<div id=\"divDeleteMoveOnScroll\" class=\"qcDivDeleteButtonHidden\"><button type=\"button\" id=\"btnDeletePages\" name=\"btn DeletePages\" class=\"btnDeletePages\" onclick=\"DeleteSelectedPages();\" >Delete</button></div>");

		}

        #endregion
        
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
		/// <param name="Number">Integer number</param>
		/// <returns>Roman numeral after conversion</returns>
		public string NumberToRoman(int Number)
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
				while (Number >= values[i])
				{
					Number -= values[i];
					result.Append(numerals[i]);
				}
			}
			if (isLower == false)
				resultWithCase = result.ToString().ToUpper();
			else resultWithCase = result.ToString().ToLower();

			return resultWithCase;
		}


		/// <summary> Converts a roman numeral to a decimal number </summary>
		/// <param name="Roman">Roman numeral (string)</param>
		/// <returns>Corresponding decimal number(integer)</returns>
		public int RomanToNumber(string Roman)
		{
			isLower = false;
			try
			{
				//Check if the roman numeral is in upper or lower case
				foreach (char t in Roman)
				{
				    if (char.IsLower(t))
				        isLower = true;
				}
			    Roman = Roman.ToUpper().Trim();
				if (Roman.Split('V').Length > 2 || Roman.Split('L').Length > 2 || Roman.Split('D').Length > 2)
					throw new ArgumentException("Rule 4 violated");

				//Rule 1-single letter may be repeated upto 3 times consecutively
				int count = 1;
				char last = 'Z';
				foreach (char numeral in Roman)
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

				while (ptr < Roman.Length)
				{
					//Get the base vaue of the numeral
					char numeral = Roman[ptr];
					int digit = (int) Enum.Parse(typeof (RomanDigit), numeral.ToString());

					//Check for Rule 3-Subtractive combination
					if (digit > maxDigit)
					{
						throw new ArgumentException("Rule 3 violated");
					}

					//Get the next digit
				    if (ptr < Roman.Length - 1)
					{
						char nextNumeral = Roman[ptr + 1];
						int nextDigit = (int) Enum.Parse(typeof (RomanDigit), nextNumeral.ToString());

						if (nextDigit > digit)
						{
							if ("IXC".IndexOf(numeral) == -1 || nextDigit > (digit*10) || Roman.Split(numeral).Length > 3)
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
			catch
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

			public Page_TreeNode METS_StructMap_Page_Node { get; set; }

			public bool Checkbox_Selected { get; set;  }

			public QC_Viewer_Page_Division_Info()
			{
				Checkbox_Selected = false;
			}
		}
	}
}
