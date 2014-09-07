#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Resource_Object
{
    /// <summary> Class holds temporary values and web-specific values which assist with
    /// optimizing the display of a resource within the SobekCM web application </summary>
    /// <remarks>  None of this data sohuld appear in the METS file, which differentiates this 
    /// object from the Behaviors_Info object.  This class is utilized by the web server to assist 
    /// with some rendering and to generally improve performance by calculating and saving several values
    /// when building the resource object.  </remarks>
    [Serializable]
    public class Web_Info
    {
        private Behaviors_Info behaviors;

        private List<Related_Titles> related_titles_collection;

        private string assocFilePath;
        private string bibid;      
        private int divCount;
        private List<string> download_eligible_files;

        
        private string fileLocation;
        private int groupID;
        private string guid;
        private string imageRoot;        

        private int itemID;
        private int pageCount;
        private List<Page_TreeNode> pages_by_seq;       
        private string service_url;
        private bool show_validation_errors;
        private Nullable<int> siblings;
        private string vid;
        private Dictionary<string, SobekCM_File_Info> viewer_to_file;

        public bool Additional_Work_Needed { get; set; }
        
        /// <summary> Constructor for a new instance of the Behaviors_Info class </summary>
        public Web_Info( Behaviors_Info Behaviors )
        {
            show_validation_errors = false;
            itemID = -1;
            groupID = -1;
            pageCount = 0;
            divCount = 0;
            behaviors = Behaviors;

            Additional_Work_Needed = false;
        }

        /// <summary> Flag indicates if this item should be displayed Left-to-Right, rather than the
        /// default Right-to-Left. </summary>
        /// <remarks> This adds support for page turning for languages such as Yiddish </remarks>
        public bool Left_To_Right
        {
            get { return false; }
        }

        /// <summary> Gets  the Greenstone source URL </summary>
        public string Source_URL
        {
            get
            {
                if ((assocFilePath == null) || (assocFilePath.Length == 0))
                {
                    assocFilePath = bibid.Substring(0, 2) + "/" + bibid.Substring(2, 2) + "/" + bibid.Substring(4, 2) + "/" + bibid.Substring(6, 2) + "/" + bibid.Substring(8, 2) + "/" + vid;
                }
                string assocFilePath_URLstyle = assocFilePath.Replace("\\", "/");
                if ((assocFilePath_URLstyle.Length > 0) && (assocFilePath_URLstyle[assocFilePath_URLstyle.Length - 1] == '/'))
                {
                    assocFilePath_URLstyle = assocFilePath_URLstyle.Substring(0, assocFilePath_URLstyle.Length - 1);
                }
                if (imageRoot != null)
                {
                    if (imageRoot.IndexOf(assocFilePath_URLstyle) >= 0)
                        return imageRoot;
                    else
                        return imageRoot + assocFilePath_URLstyle;
                }
                else
                {
                    return assocFilePath_URLstyle;
                }
            }
        }

        /// <summary> Gets the collection of pages by sequence </summary>
        public ReadOnlyCollection<Page_TreeNode> Pages_By_Sequence
        {
            get
            {
                if (pages_by_seq == null)
                    return new ReadOnlyCollection<Page_TreeNode>(new List<Page_TreeNode>());
                else
                    return new ReadOnlyCollection<Page_TreeNode>(pages_by_seq);
            }
        }

        /// <summary> Clears the complete list of pages by sequence </summary>
        public void Clear_Pages_By_Sequence()
        {
            if ( pages_by_seq != null )
                pages_by_seq.Clear();
        }

        /// <summary> Sets the bibliographic identifier </summary>
        internal string BibID
        {
            set { bibid = value; }
        }

        /// <summary> Sets the volume identifier </summary>
        internal string VID
        {
            set { vid = value; }
        }

        #region Methods used by the SobekCM web application to get a valid viewer by code 

        /// <summary> Gets the collection of pages by sequence </summary>
        public Dictionary<string, SobekCM_File_Info> Viewer_To_File
        {
            get
            {
                if (viewer_to_file == null)
                    viewer_to_file = new Dictionary<string, SobekCM_File_Info>();


                return viewer_to_file;
            }
        }


        /// <summary> Gets a valid viewer code for this item, based on the requested viewer code and page </summary>
        /// <param name="viewer_code"> Requested viewer code </param>
        /// <param name="page"> Requested page </param>
        /// <returns> Valid viewer code and page </returns>
        /// <remarks> The requested viewer code and page are validated, and if valid, returned.  Otherwise, the closest valid viewer code to the one requested is returned. </remarks>
        public string Get_Valid_Viewer_Code(string viewer_code, int page)
        {
            string lower_code = viewer_code.ToLower();

            // If this is 'RES' for restricted, taht is always valid
            if (viewer_code == "res")
                return "res";

            // Is this in the item level viewer list?
            if (lower_code.Length > 0)
            {
                if (behaviors.Views_Count > 0 )
                {
                    foreach (View_Object thisView in behaviors.Views)
                    {
                        foreach (string thisCode in thisView.Viewer_Codes)
                        {
                            if (thisCode.ToLower() == lower_code)
                            {
                                return lower_code;
                            }
                        }
                    }
                }

                // Check if this is a Related Items page, which also allows paging
                if (lower_code.IndexOf(View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0]) >= 0)
                {
                    int viewer_page = 0;
                    try
                    {
                        string try_page_get = lower_code.Replace(View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0], "").Trim();
                        if (try_page_get.Length > 0)
                        {
                            viewer_page = Convert.ToInt32(try_page_get);
                        }
                        return View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0];
                    }
                    catch
                    {
                    }
                }

                // Check if this is the quality conrtols page, which also allows paging
                if (lower_code.IndexOf(View_Object.Viewer_Code_By_Type(View_Enum.QUALITY_CONTROL)[0]) >= 0)
                {
                    int viewer_page = 0;
                    try
                    {
                        string try_page_get = lower_code.Replace(View_Object.Viewer_Code_By_Type(View_Enum.QUALITY_CONTROL)[0], "").Trim();
                        if (try_page_get.Length > 0)
                        {
                            viewer_page = Convert.ToInt32(try_page_get);
                        }
                        return View_Object.Viewer_Code_By_Type(View_Enum.QUALITY_CONTROL)[0];
                    }
                    catch
                    {
                    }
                }

                // Check each page
                if (viewer_to_file != null)
                {
					if (viewer_to_file.ContainsKey(lower_code))
					{
						return lower_code;
					}
					else
					{
						// TODO: This should not be hardcoded for JPEG, ZOOMABLE, and then TEXT priority. This should come from the item viewer priority table in the database.
						int testInt;
						if (Int32.TryParse(lower_code, out testInt))
						{
							if (viewer_to_file.ContainsKey(lower_code + "j"))
								return lower_code + "j";
							if (viewer_to_file.ContainsKey(lower_code + "x"))
								return lower_code + "x";
							if (viewer_to_file.ContainsKey(lower_code + "t"))
								return lower_code + "t";
						}
					}
                }
            }

            // No match, so just return the default, if there is one
            if ((viewer_code.Length == 0) && ( behaviors.Default_View != null) && (behaviors.Default_View.Viewer_Codes.Length > 0))
            {
	            if (behaviors.Item_Level_Page_Views.Contains(behaviors.Default_View))
	            {
					foreach (SobekCM_File_Info thisFile in pages_by_seq[page - 1].Files)
					{
						if (thisFile.Get_Viewer() != null)
						{
							if (thisFile.Get_Viewer().View_Type == behaviors.Default_View.View_Type)
							{
								return page.ToString() + thisFile.Get_Viewer().Viewer_Codes[0];
							}
						}
					}

					// If no matching item level page view was found matching the view type,
					// default to any of the item level page views
					foreach (View_Object itemTaggedView in behaviors.Item_Level_Page_Views)
					{
						foreach (SobekCM_File_Info thisFile in pages_by_seq[page - 1].Files)
						{
							if (thisFile.Get_Viewer() != null)
							{
								if (thisFile.Get_Viewer().View_Type == itemTaggedView.View_Type)
								{
									return page.ToString() + thisFile.Get_Viewer().Viewer_Codes[0];
								}
							}
						}
					}
	            }
	            else
	            {
					return behaviors.Default_View.Viewer_Codes[0];
	            }
            }

            // Return first page viewer code
            if ((pages_by_seq != null) && (behaviors.Item_Level_Page_Views_Count > 0 ) && (pages_by_seq.Count > 0) && (pages_by_seq[0].Files.Count > 0))
            {
                foreach (View_Object itemTaggedView in behaviors.Item_Level_Page_Views)
                {
                    foreach (SobekCM_File_Info thisFile in pages_by_seq[0].Files)
                    {
                        if (thisFile.Get_Viewer() != null)
                        {
                            if (thisFile.Get_Viewer().View_Type == itemTaggedView.View_Type)
                            {
                                return "1" + thisFile.Get_Viewer().Viewer_Codes[0];
                            }
                        }
                    }
                }
            }

            return View_Object.Viewer_Code_By_Type(View_Enum.CITATION)[0];
        }

        /// <summary> Gets the view object from this item based on the requested viewer code  </summary>
        /// <param name="viewer_code"> Viewer code for the viewer requested </param>
        /// <returns> Valid view object from this item, based on requested viewer code, or NULL </returns>
        public View_Object Get_Viewer(string viewer_code)
        {
            string lower_code = viewer_code.ToLower();

            // If this is for the restricted viewer, that is always valid
            if (lower_code == "res")
            {
                return new View_Object(View_Enum.RESTRICTED);
            }

            // If this was for the full citation, jsut return that
            if (lower_code.IndexOf("citation") == 0)
            {
                return new View_Object(View_Enum.CITATION);
            }

            // If this was for the full citation, jsut return that
            if (lower_code == "test" )
            {
                return new View_Object(View_Enum.TEST);
            }

            // Is this in the item level viewer list?
            if ( behaviors.Views_Count > 0 )
            {
                foreach (View_Object thisView in behaviors.Views)
                {
                    foreach (string thisCode in thisView.Viewer_Codes)
                    {
                        if (thisCode.ToLower() == lower_code)
                        {
                            return thisView;
                        }
                    }
                }
            }

            // Check each page
            if ((viewer_to_file != null) && (viewer_to_file.ContainsKey(lower_code)))
            {
                return viewer_to_file[lower_code].Get_Viewer();
            }

            // No match, so just return the default..
            if ( behaviors.Default_View != null)
            {
                return behaviors.Default_View;
            }

            // Return first page viewer
            if ((pages_by_seq != null) && (pages_by_seq.Count > 0) && (pages_by_seq[0].Files.Count > 0))
            {
                SobekCM_File_Info first_page = pages_by_seq[0].Files[0];
                View_Object firstPageViewer = first_page.Get_Viewer();
            }

            // If there is a single view, return the first code for it
            if ( behaviors.Views_Count > 0 )
            {
                return behaviors.Views[0];
            }

            // Return null
            return null;
        }

        #endregion


        #region Related title properties and methods

        /// <summary> Gets the number of related titles from the SobekCM database </summary>
        public int Related_Titles_Count
        {
            get
            {
                if (related_titles_collection == null)
                    return 0;
                else
                    return related_titles_collection.Count;
            }
        }

        /// <summary> Gets the related titles collection </summary>
        public List<Related_Titles> All_Related_Titles
        {
            get
            {
                if (related_titles_collection == null)
                    related_titles_collection = new List<Related_Titles>();
                return related_titles_collection;
            }
        }

        #endregion

        #region Methods for building and returning information about the files in the (online) directory

        /// <summary> Gets the list of all download eligible files for this package </summary>
        /// <param name="Directory"> Directory to check for the files </param>
        /// <returns> List of all download eligible files </returns>
        /// <remarks> This list excludes all page image type files and the standard SobekCM metadata files </remarks>
        public ReadOnlyCollection<string> Get_Download_Eligible_Files(string Directory)
        {
            if (download_eligible_files == null)
            {
                download_eligible_files = new List<string>();

                try
                {
                    string[] files = System.IO.Directory.GetFiles(Directory);
                    foreach (string thisFile in files)
                    {
                        FileInfo thisFileInfo = new FileInfo(thisFile);
                        string extension = thisFileInfo.Extension.ToUpper();
                        if ((extension.IndexOf("JPG") < 0) && (extension != ".JP2") && (extension != ".TXT") && (extension != ".PRO") && (extension.IndexOf("METS") < 0) && (extension.IndexOf("HTM") < 0))
                        {
                            // Now exclude specific file names.
                            // Part of the reason I am doing this is so users can make their XML files downloadable
                            string filename = thisFileInfo.Name.ToLower();
                            if ((filename != "marc.xml") && (filename != "doc.xml") && (filename != "citation_mets.xml") && (filename != "ufdc_mets.xml") && (filename.IndexOf(".METS_Header.bak") < 0) && (filename != bibid.ToLower() + "_" + vid + ".html") && (filename.IndexOf(".mets") < 0))
                            {
                                string file_group_name = filename.Replace(thisFileInfo.Extension.ToLower(), ".*");
                                if (!download_eligible_files.Contains(file_group_name))
                                    download_eligible_files.Add(file_group_name);
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return new ReadOnlyCollection<string>(download_eligible_files);
        }

        /// <summary> Gets the list of all possible thumbnail jpeg files for this package </summary>
        /// <param name="Directory"> Directory to check for the files </param>
        /// <returns> List of all possible thumbnail jpeg files </returns>
        /// <remarks> This pulls the file list from the directory each time it is called.  In addition, it
        /// sorts by the filename, since this was not always happening correctly and this is primarily used
        /// for the online metadata update.</remarks>
        public ReadOnlyCollection<string> Get_Thumbnail_Files(string Directory)
        {
            SortedList<string, string> thumbnail_files = new SortedList<string, string>();
            try
            {
                string[] files = System.IO.Directory.GetFiles(Directory, "*thm.jpg");
                foreach (string thisFile in files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    thumbnail_files[thisFileInfo.Name] = thisFileInfo.Name;
                }
            }
            catch
            {
            }
            return new ReadOnlyCollection<string>(thumbnail_files.Values);
        }

        #endregion

        #region Simple properties


        /// <summary> Gets or sets the GUID for this item </summary>
        public string GUID
        {
            get { return guid ?? String.Empty; }
            set { guid = value; }
        }

        /// <summary> Gets or sets the URL (non-PURL) for this item in this SobekCM Item </summary>
        public string Service_URL
        {
            get { return service_url ?? String.Empty; }
            set { service_url = value; }
        }

        /// <summary> Flag determines if validation errors should be shown </summary>
        public bool Show_Validation_Errors
        {
            get { return show_validation_errors; }
            set { show_validation_errors = value; }
        }

        /// <summary> Sets the greenstone image root  </summary>
        public string Image_Root
        {
            set { imageRoot = value; }
            get { return imageRoot; }
        }


        /// <summary> Gets and sets the associated file path for this item from the SobekCM Web database </summary>
        public string AssocFilePath
        {
            get { return assocFilePath ?? String.Empty; }
            set { assocFilePath = value; }
        }

        ///// <summary> Gets and sets the greenstone code for this item from the SobekCM Web database </summary>
        //public string Greenstone_Code
        //{
        //    get { return greenstoneCode ?? String.Empty; }
        //    set { greenstoneCode = value; }
        //}

        /// <summary> Gets and sets the file root for this item from the SobekCM Web database </summary>
        public string File_Root
        {
            get { return fileLocation ?? String.Empty; }
            set { fileLocation = value; }
        }

        /// <summary> Gets and sets the group id for this item from the SobekCM Web database </summary>
        public int GroupID
        {
            get { return groupID; }
            set { groupID = value; }
        }

        /// <summary> Gets and sets the item id for this item from the SobekCM Web database </summary>
        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        /// <summary> Gets and sets the item id for this item from the SobekCM Web database </summary>
        public Nullable<int> Siblings
        {
            get { return siblings; }
            set { siblings = value; }
        }

        /// <summary> Gets and sets the static page count for this item from the SobekCM Web database </summary>
        public int Static_PageCount
        {
            get { return pageCount; }
            set { pageCount = value; }
        }

        /// <summary> Gets and sets the static division count for this item from the SobekCM Web database </summary>
        public int Static_Division_Count
        {
            get { return divCount; }
            set { divCount = value; }
        }

        #endregion

        /// <summary> Sets the BibID and VID in this child object, which is used to determine the default folder structure </summary>
        /// <param name="BibID"> BibID </param>
        /// <param name="VID"> VID </param>
        public void Set_BibID_VID(string BibID, string VID)
        {
            bibid = BibID;
            vid = VID;
        }

        /// <summary> Adds to the collection of pages by sequence </summary>
        /// <param name="Next_Page"> Next page in sequence to add </param>
        public void Add_Pages_By_Sequence(Page_TreeNode Next_Page)
        {
            if (pages_by_seq == null)
                pages_by_seq = new List<Page_TreeNode>();
            pages_by_seq.Add(Next_Page);
        }
    }
}