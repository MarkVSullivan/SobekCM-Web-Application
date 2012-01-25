using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;

namespace SobekCM.Bib_Package.Divisions
{
    /// <summary> Stores information about the divisions and files associated with this resource </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Division_Info : XML_Writing_Base_Type
    {
        private Division_Tree physicalDivisionTree;
        private Division_Tree downloadDivisionTree;
        private string source_directory;
        private bool suppress_checksum;
        private int directly_set_page_count;


        /// <summary> Constructor creates a new instance of the Divison_Info class </summary>
        public Division_Info()
        {
            physicalDivisionTree = new Division_Tree("PAGE");
            downloadDivisionTree = new Division_Tree("FILES");
            source_directory = String.Empty;
            suppress_checksum = true;
            directly_set_page_count = 0;
        }

        /// <summary> Method removes many of the string values that are loaded while reading the metadata
        /// but are not needed for general use of the item in the SobekCM web application  </summary>
        public void Minimize_File_Size()
        {
            // First clear the extraneous checksum information on the physical division tree
            List<abstract_TreeNode> allNodes = physicalDivisionTree.Divisions_PreOrder;
            foreach (abstract_TreeNode thisNode in allNodes)
            {
                if (thisNode.Page)
                {
                    Page_TreeNode pageNode = (Page_TreeNode)thisNode;
                    foreach (SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        thisFile.Checksum_Type = null;
                        thisFile.Checksum = null;
                    }
                }
            }

            // Next clear the extraneous checksum information on the download tree
            allNodes = downloadDivisionTree.Divisions_PreOrder;
            foreach (abstract_TreeNode thisNode in allNodes)
            {
                if (thisNode.Page)
                {
                    Page_TreeNode pageNode = (Page_TreeNode)thisNode;
                    foreach (SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        thisFile.Checksum_Type = null;
                        thisFile.Checksum = null;
                    }
                }
            }
        }

        #region Public Properties

        /// <summary> Gets and sets the flag to suppress checksums </summary>
        public bool Suppress_Checksum
        {
            set { suppress_checksum = value; }
            get { return suppress_checksum; }
        }

        /// <summary> Gets and sets the source directory, where all the related files should exist </summary>
        public string Source_Directory
        {
            get { return source_directory; }
            set { source_directory = value; }
        }

        /// <summary> Gets the number of pages in the physical structure map for this item,
        /// or allows the number of pages to be set for items which do not have any pages (such as PDF only objects)</summary>
        public int Page_Count
        {
            get
            {
                int pages_count = physicalDivisionTree.Pages_PreOrder.Count;
                if (pages_count > 0)
                    return pages_count;
                if (directly_set_page_count > 0)
                    return directly_set_page_count;
                return 0;
            }
            set
            {
                directly_set_page_count = value;
            }
        } 

        /// <summary> Gets the physical division hierarchy tree (analogous to the table of contents 
        /// for an original phyiscl object) </summary>
        public Division_Tree Physical_Tree
        {
            get { return physicalDivisionTree; }
        }

        /// <summary> Gets the division hierarchy tree which contains all other download files </summary>
        public Division_Tree Download_Tree
        {
            get { return downloadDivisionTree; }
        }

        /// <summary> Gets the collection of all files associated with this resource, either in the physical 
        /// hierarchy tree or the download/other files hierarchy </summary>
        public List<SobekCM_File_Info> Files
        {
            get
            {
                // Return empty collection if here are no nodes at all
                if ((( physicalDivisionTree == null) || (physicalDivisionTree.Roots.Count == 0)) && (( downloadDivisionTree == null ) || ( downloadDivisionTree.Roots.Count == 0 )))
                {
                    return new List<SobekCM_File_Info>();
                }
                
                // Get the list from the physical tree first
                List<SobekCM_File_Info> returnVal = physicalDivisionTree.All_Files;

                // Now, add the files from the download tree
                returnVal.AddRange(downloadDivisionTree.All_Files);

                // Return the collection
                return returnVal;
            }
        }

        /// <summary> Flag indicates if there are any files linked to this item in either  
        /// the physical TOC tree or the download/other files tree </summary>
        public bool Has_Files
        {
            get
            {
                if (physicalDivisionTree.Has_Files)
                    return true;
                if (downloadDivisionTree.Has_Files)
                    return true;
                return false;
            }
        }

        /// <summary> Flag indicates if there are multiple divisions for this physical tree </summary>
        public bool Has_Multiple_Divisions
        {
            get
            {
                if (physicalDivisionTree.Roots.Count > 1)
                    return true;
                if (physicalDivisionTree.Roots.Count == 1)
                {
                    if (!physicalDivisionTree.Roots[0].Page)
                    {
                        int division_count = 0;
                        Division_TreeNode divNode = (Division_TreeNode)physicalDivisionTree.Roots[0];
                        foreach (abstract_TreeNode node in physicalDivisionTree.Roots)
                        {
                            if (!node.Page)
                            {
                                division_count++;
                                if (division_count > 1)
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }
        }


        /// <summary> Gets the collection of all page image files associated with the
        /// physical structure map for this resource </summary>
        public List<SobekCM_File_Info> Page_Files
        {
            get
            {
                return physicalDivisionTree.All_Files;
            }
        }

        /// <summary> Gets the collection of all download/other files associated with this resource </summary>
        public List<SobekCM_File_Info> Download_Other_Files
        {
            get
            {
                return downloadDivisionTree.All_Files;
            }
        }

        // Returns the entire table of contents as XML
        internal string Table_Of_Contents_XML
        {
            get
            {
                StringBuilder tocBuilder = new StringBuilder();
                List<abstract_TreeNode> divisions = physicalDivisionTree.Divisions_PreOrder;
                foreach (abstract_TreeNode node in divisions)
                {
                    if (!node.Page)
                    {
                        tocBuilder.Append(node.Label + " ");
                    }
                }

                return base.Convert_String_To_XML_Safe(tocBuilder.ToString());
            }
        }

        #endregion

        #region Public Methods
 
        /// <summary> Clear all divisions and all files </summary>
        public void Clear()
        {
            physicalDivisionTree.Clear();
            downloadDivisionTree.Clear();
        }  

        #endregion

        #region Methods to clear and create the checksums for all the files in this package

        /// <summary> Clears the checksums associated with all the page file images </summary>
        public void Clear_Checksums()
        {
            // Get the list of all pages from this division 
            List<abstract_TreeNode> pageNodes = this.physicalDivisionTree.Pages_PreOrder;
            List<abstract_TreeNode> fileNodes = this.downloadDivisionTree.Pages_PreOrder;

            foreach (Divisions.Page_TreeNode pageNode in pageNodes)
            {
                // Step through each file
                foreach (Divisions.SobekCM_File_Info thisFile in pageNode.Files)
                {
                    thisFile.Checksum_Type = null;
                    thisFile.Checksum = null;
                }
            }
            foreach (Divisions.Page_TreeNode pageNode in fileNodes)
            {
                // Step through each file
                foreach (Divisions.SobekCM_File_Info thisFile in pageNode.Files)
                {
                    thisFile.Checksum_Type = null;
                    thisFile.Checksum = null;
                }
            }
        }

        /// <summary> Returns flag indicating it needs checksums calculated </summary>
        public bool Needs_Checksums
        {
            get
            {
                // Get the list of all pages from this division 
                List<abstract_TreeNode> pageNodes = this.physicalDivisionTree.Pages_PreOrder;
                foreach (Page_TreeNode pageNode in pageNodes)
                {
                    // Step through each file
                    foreach (Divisions.SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        // Recalculate this file info?
                        if ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0))
                        {
                            return true;
                        }
                    }
                }

                // Get the list of all pages from the downloads/other files tree 
                pageNodes = this.downloadDivisionTree.Pages_PreOrder;
                foreach (Page_TreeNode pageNode in pageNodes)
                {
                    // Step through each file
                    foreach (Divisions.SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        // Recalculate this file info?
                        if ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }


        /// <summary> Recalculates all the checksums for associated files </summary>
        /// <param name="Recalculate_All"> Flag indicates whether to recalculate all files, regardless of need </param>
        /// <param name="Progress_Delegate"> Progress delegate is called to update any progress displays </param>
        public void Calculate_Checksum(bool Recalculate_All, New_SobekCM_Bib_Package_Progress_Task_Group Progress_Delegate )
        {
            // Get the list of all pages from this division trees
            List<abstract_TreeNode> pageNodes = this.physicalDivisionTree.Pages_PreOrder;
            List<abstract_TreeNode> fileNodes = this.downloadDivisionTree.Pages_PreOrder;

            // Update progress 
            if (Progress_Delegate != null)
            {
                Progress_Delegate("Calculating checksums", "Checking all files", 0, 2);
            }

            // Count how many pages to calculate for
            int total_files = 0;
            foreach (Page_TreeNode pageNode in pageNodes)
            {
                // Step through each file
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    // Recalculate this file info?
                    if ((Recalculate_All) || ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0)))
                    {
                        total_files++;
                    }
                }
            }
            foreach (Page_TreeNode pageNode in fileNodes)
            {
                // Step through each file
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    // Recalculate this file info?
                    if ((Recalculate_All) || ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0)))
                    {
                        total_files++;
                    }
                }
            }

            if (total_files == 0)
                return;

            // Now caulcate
            int file_count = 0;
            FileMD5 checksummer = new FileMD5();
            foreach (Page_TreeNode pageNode in pageNodes)
            {
                // Step through each file
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    // Recalculate this file info?
                    if ((Recalculate_All) || ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0)))
                    {
                        // Perform in a try catch
                        try
                        {
                            // Get the size first
                            System.IO.FileInfo thisFileInfo = new System.IO.FileInfo(source_directory + "/" + thisFile.System_Name);
                            thisFile.Size = thisFileInfo.Length;

                            // Get the checksum, if it doesn't exist
                            if ((String.IsNullOrEmpty(thisFile.Checksum)) || ( Recalculate_All ))
                            {
                                thisFile.Checksum = checksummer.Calculate_Checksum(source_directory + "/" + thisFile.System_Name);
                                thisFile.Checksum_Type = "MD5";
                            }
                        }
                        catch ( Exception ee )
                        {
                            string error = ee.Message;
                        }

                        file_count++;

                        // Update progress 
                        if (Progress_Delegate != null)
                        {
                            Progress_Delegate("Calculating checksums", "Calculating '" + thisFile.System_Name + "'", file_count, total_files);
                        }
                    }
                }
            }
            foreach (Page_TreeNode pageNode in fileNodes)
            {
                // Step through each file
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    // Recalculate this file info?
                    if ((Recalculate_All) || ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0)))
                    {
                        // Perform in a try catch
                        try
                        {
                            // Get the size first
                            System.IO.FileInfo thisFileInfo = new System.IO.FileInfo(source_directory + "/" + thisFile.System_Name);
                            thisFile.Size = thisFileInfo.Length;

                            // Get the checksum, if it doesn't exist
                            if ((String.IsNullOrEmpty(thisFile.Checksum)) || (Recalculate_All))
                            {
                                thisFile.Checksum = checksummer.Calculate_Checksum(source_directory + "/" + thisFile.System_Name);
                                thisFile.Checksum_Type = "MD5";
                            }
                        }
                        catch (Exception ee)
                        {
                            string error = ee.Message;
                        }

                        file_count++;

                        // Update progress 
                        if (Progress_Delegate != null)
                        {
                            Progress_Delegate("Calculating checksums", "Calculating '" + thisFile.System_Name + "'", file_count, total_files);
                        }
                    }
                }
            }

            // Update progress 
            if (Progress_Delegate != null)
            {
                Progress_Delegate("Checksum Complete", "Checksum Complete", file_count, total_files);
            }
        }



        #endregion

        #region Code to create the METS File Sections

        /// <summary> Adds the file/division related sections to the METS file ( fileSec, structMap, etc.. ) </summary>
        /// <param name="include_sobekcm_custom_file_tech_specs"> Flag indicates whether to include the custom SobekCM-formatted technical specs on image files</param>
        /// <param name="minimize_file_size">Flag indicates whether to minimize file size by leaving out checksums and file sizes</param>
        /// <param name="stream">Stream to which to write the METS file section and possibly the SobekCM file specs </param>
        /// <param name="title">Title of this resource</param>
        /// <param name="main_dmd_links">IDs for the dmdSec in this METS file, linking the whole structure map to the bibliographic descriptor sections</param>
        /// <param name="mimes_to_exclude"> List of mime types to exclude from the resulting METS file </param>
        internal void Add_METS(System.IO.TextWriter stream, bool minimize_file_size, bool include_sobekcm_custom_file_tech_specs, string title, string main_dmd_links, List<string> mimes_to_exclude )
        {
            string sobekcm_namespace = "sobekcm";

            // Create the collections needed for this work
            Dictionary<SobekCM_File_Info, int> file_to_group_number = new Dictionary<SobekCM_File_Info, int>();
            Dictionary<SobekCM_File_Info, string> file_to_fileid = new Dictionary<SobekCM_File_Info, string>();
            List<SobekCM_File_Info> allFiles = new List<SobekCM_File_Info>();
            Dictionary<string, List<SobekCM_File_Info>> mimeHash = new Dictionary<string, List<SobekCM_File_Info>>();
            Dictionary<abstract_TreeNode, int> pages_to_appearances = new Dictionary<abstract_TreeNode, int>();
            List<SobekCM_File_Info> tiffMimeList = null;

            // Create some private variables for this
            string mime, filename_lower;
            int group_number = 0;
            bool validNewPage = false;
            bool hasPageFiles = false;
            bool hasDownloadFiles = false;

            // Get the list of divisions from the physical tree and the download/other file tre
            List<abstract_TreeNode> physicalDivisions = physicalDivisionTree.Divisions_PreOrder;
            List<abstract_TreeNode> downloadDivisions = downloadDivisionTree.Divisions_PreOrder;

            // First, assign group numbers for all the files on each page (physical or other)
            // Group numbers in the METS file correspond to files on the same page (physical or other)
            // At the same time, we will build the list of all files and files by mime type
            foreach (abstract_TreeNode thisNode in physicalDivisions)
            {
                // If this is a page, look for files
                if (thisNode.Page)
                {
                    validNewPage = false;
                    Page_TreeNode pageNode = (Page_TreeNode)thisNode;

                    // Step through any files under this page
                    foreach (SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        // If this is a new file, this must also be a new page
                        if (!file_to_group_number.ContainsKey(thisFile))
                        {
                            // Increment the group number
                            if (!validNewPage)
                            {
                                validNewPage = true;
                                group_number++;
                            }

                            // Get this MIME type
                            mime = thisFile.MIME_Type(thisFile.File_Extension);
                            if (!mimes_to_exclude.Contains(mime))
                            {
                                // Add this to the lookup hash for later
                                file_to_group_number[thisFile] = group_number;

                                // Also add to the list of files
                                allFiles.Add(thisFile);

                                // Also ensure we know there are page image files
                                hasPageFiles = true;

                                // Special check for thumbnail duplicates of type jpeg or gif
                                if ((mime == "image/jpeg") || (mime == "image/gif"))
                                {
                                    filename_lower = thisFile.System_Name.ToLower();
                                    if ((filename_lower.IndexOf("thm.gif") > 0) || (filename_lower.IndexOf("thm.jp") > 0))
                                    {
                                        mime = mime + "-thumbnails";
                                    }
                                }

                                // If this is a new MIME type, add it
                                if (!mimeHash.ContainsKey(mime))
                                {
                                    List<SobekCM_File_Info> newList = new List<SobekCM_File_Info>();
                                    newList.Add(thisFile);
                                    mimeHash[mime] = newList;

                                    // If this was the special tiff mime, save this
                                    if (mime == "image/tiff")
                                        tiffMimeList = newList;
                                }
                                else
                                {
                                    mimeHash[mime].Add(thisFile);
                                }
                            }
                        }
                    }
                }
            }

            // Now, do the same thing for the download/other files division tree
            // Group numbers in the METS file correspond to files on the same page (physical or other)
            // At the same time, we will build the list of all files and files by mime type
            foreach (abstract_TreeNode thisNode in downloadDivisions)
            {
                // If this is a page, look for files
                if (thisNode.Page)
                {
                    validNewPage = false;
                    Page_TreeNode pageNode = (Page_TreeNode)thisNode;

                    // Step through any files under this page
                    foreach (SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        // If this is a new file, this must also be a new page
                        if (!file_to_group_number.ContainsKey(thisFile))
                        {
                            // Increment the group number
                            if (!validNewPage)
                            {
                                validNewPage = true;
                                group_number++;
                            }

                            // Add this to the lookup hash for later
                            file_to_group_number[thisFile] = group_number;

                            // Also add to the list of files
                            allFiles.Add(thisFile);

                            // Also ensure we know there are download files
                            hasDownloadFiles = true;

                            // Get this MIME type
                            mime = thisFile.MIME_Type(thisFile.File_Extension);

                            // Special check for thumbnail duplicates of type jpeg or gif
                            if ((mime == "image/jpeg") || (mime == "image/gif"))
                            {
                                filename_lower = thisFile.System_Name.ToLower();
                                if ((filename_lower.IndexOf("thm.gif") > 0) || (filename_lower.IndexOf("thm.jp") > 0))
                                {
                                    mime = mime + "-thumbnails";
                                }
                            }

                            // If this is a new MIME type, add it
                            if (!mimeHash.ContainsKey(mime))
                            {
                                List<SobekCM_File_Info> newList = new List<SobekCM_File_Info>();
                                newList.Add(thisFile);
                                mimeHash[mime] = newList;

                                // If this was the special tiff mime, save this
                                if (mime == "image/tiff")
                                    tiffMimeList = newList;
                            }
                            else
                            {
                                mimeHash[mime].Add(thisFile);
                            }
                        }
                    }
                }
            }

            // Start building the result strings
            StringBuilder results = new StringBuilder(5000);
            results.Append("<METS:fileSec> \r\n");

            // Also start building the technical section
            StringBuilder amdSecBuilder = new StringBuilder(5000);

            // Step through each mime type
            foreach (List<SobekCM_File_Info> mimeCollection in mimeHash.Values)
            {
                // Start this file group section
                if (mimeCollection == tiffMimeList)
                {
                    results.Append("<METS:fileGrp USE=\"archive\" > \r\n");
                }
                else
                {
                    results.Append("<METS:fileGrp USE=\"reference\" > \r\n");
                }

                // Step through each file of this mime type and append the METS
                List<string> fileids_used = new List<string>();
                foreach (SobekCM_File_Info thisFile in mimeCollection)
                {
                    // Get the group number for this file now and save the resulting file id
                    group_number = file_to_group_number[thisFile];
                    string fileid = thisFile.Add_METS(source_directory, suppress_checksum, minimize_file_size, group_number, results, fileids_used);
                    fileids_used.Add(fileid);
                    file_to_fileid[thisFile] = fileid;

                    if (include_sobekcm_custom_file_tech_specs)
                    {
                        amdSecBuilder.Append(thisFile.toTechMETS(sobekcm_namespace, fileid));
                    }
                }

                // Close out this file group section
                results.Append("</METS:fileGrp> \r\n");
            }

            // Finish out the file section
            results.Append("</METS:fileSec> \r\n");

            // Add the built amd sec, if it should be included
            if ((sobekcm_namespace.Length > 0) && (amdSecBuilder.Length > 0) && (include_sobekcm_custom_file_tech_specs))
            {
                stream.Write("<METS:amdSec>\r\n<METS:techMD ID=\"TECHMD1\">\r\n<METS:mdWrap MDTYPE=\"OTHER\" OTHERMDTYPE=\"SobekCM\" MIMETYPE=\"text/xml\" LABEL=\"SobekCM File Technical Details\">\r\n<METS:xmlData>\r\n<" + sobekcm_namespace + ":FileInfo>\r\n" + amdSecBuilder.ToString() + "</" + sobekcm_namespace + ":FileInfo>\r\n</METS:xmlData>\r\n</METS:mdWrap>\r\n</METS:techMD>\r\n</METS:amdSec>\r\n");
            }

            // Add the file section
            stream.Write(results.ToString());

            // Clear these string builders
            amdSecBuilder = null;
            results = null;

            // Check that some files do indeed exist
            if (allFiles.Count == 0)
            {
                // Structure map is a required element for METS
                stream.Write("<METS:structMap> <METS:div /> </METS:structMap>\r\n");

            }
            else
            {
                if (hasPageFiles)
                {
                    stream.Write("<METS:structMap ID=\"STRUCT1\" TYPE=\"physical\"> \r\n");

                    // Start the main division information
                    stream.Write("<METS:div DMDID=\"" + main_dmd_links + "\"");

                    // Add the title, if one was provided
                    if (title.Length > 0)
                    {
                        stream.Write(" LABEL=\"" + title + "\"");
                    }

                    // Finish out this first, main division tag
                    stream.Write(" ORDER=\"0\" TYPE=\"main\"> \r\n");

                    // Add all the divisions recursively
                    int order = 1;
                    int division_counter = 1;
                    int page_counter = 1;
                    foreach (abstract_TreeNode thisRoot in physicalDivisionTree.Roots)
                    {
                        recursively_add_div_info(thisRoot, stream, pages_to_appearances, file_to_fileid, "PDIV", "PAGE", order++, ref division_counter, ref page_counter);
                    }

                    // Close out the main division tag
                    stream.Write("</METS:div>\r\n");

                    // Close out this structure map portion
                    stream.Write("</METS:structMap>\r\n");
                }

                if (hasDownloadFiles)
                {
                    stream.Write("<METS:structMap ID=\"STRUCT2\" TYPE=\"other\"> \r\n");

                    // Start the main division information
                    stream.Write("<METS:div DMDID=\"" + main_dmd_links + "\"");

                    // Add the title, if one was provided
                    if (title.Length > 0)
                    {
                        stream.Write(" LABEL=\"" + title + "\"");
                    }

                    // Finish out this first, main division tag
                    stream.Write(" ORDER=\"0\" TYPE=\"main\"> \r\n");

                    // Add all the divisions recursively
                    int order = 1;
                    int division_counter = 1;
                    int page_counter = 1;
                    foreach (abstract_TreeNode thisRoot in downloadDivisionTree.Roots)
                    {
                        recursively_add_div_info(thisRoot, stream, pages_to_appearances, file_to_fileid, "DDIV", "FILES", order++, ref division_counter, ref page_counter);
                    }

                    // Close out the main division tag
                    stream.Write("</METS:div>\r\n");

                    // Close out this structure map portion
                    stream.Write("</METS:structMap>\r\n");
                }
            }
        }

        private void recursively_add_div_info(abstract_TreeNode thisNode, System.IO.TextWriter results, Dictionary<abstract_TreeNode, int> pages_to_appearances, Dictionary<SobekCM_File_Info, string> file_to_fileid, string division_prefix, string page_prefix, int order, ref int division_counter, ref int page_counter  )
        {
            // Add the div information for this node first
            if (thisNode.Page)
            {
                if (pages_to_appearances.ContainsKey(thisNode))
                {
                    pages_to_appearances[thisNode] = pages_to_appearances[thisNode] + 1;
                    results.Write("<METS:div ID=\"" + page_prefix + page_counter.ToString() + "_repeat" + pages_to_appearances[thisNode] + "\"");
                }
                else
                {
                    pages_to_appearances[thisNode] = 1;
                    results.Write("<METS:div ID=\"" + page_prefix + page_counter.ToString() + "\"");
                    page_counter++;
                }
            }
            else
            {
                results.Write("<METS:div ID=\"" + division_prefix + division_counter.ToString() + "\"");
                division_counter++;
            }

            // Add the label, if there is one
            if ((thisNode.Label.Length > 0) && (thisNode.Label != thisNode.Type))
                results.Write(" LABEL=\"" + base.Convert_String_To_XML_Safe(thisNode.Label) + "\"");

            // Finish the start div label for this division
            results.Write(" ORDER=\"" + order + "\" TYPE=\"" + thisNode.Type + "\"> \r\n");

            // If this is a page, add all the files, otherwise call this method recursively
            if (thisNode.Page)
            {
                // Add each file
                Page_TreeNode thisPage = (Page_TreeNode)thisNode;
                foreach (SobekCM_File_Info thisFile in thisPage.Files)
                {
                    if (file_to_fileid.ContainsKey(thisFile))
                    {
                        // Add the file pointer informatino
                        results.Write("<METS:fptr FILEID=\"" + file_to_fileid[thisFile] + "\" /> \r\n");
                    }
                }
            }
            else
            {
                // Call this method for each subdivision
                int inner_order = 1;
                Division_TreeNode thisDivision = (Division_TreeNode)thisNode;
                foreach (abstract_TreeNode thisSubDivision in thisDivision.Nodes)
                {
                    recursively_add_div_info(thisSubDivision, results, pages_to_appearances, file_to_fileid, division_prefix, page_prefix, inner_order++, ref division_counter, ref page_counter );
                }
            }

            // Close out this division
            results.Write("</METS:div> \r\n");
        }

        #endregion

        #region Methods to write the GSA metadata

        /// <summary> Returns the associated file section for the Greenstone Archival format </summary>
        /// <returns>XML of the associated file section for the Greenstone Archival format </returns>
        internal string GSA_Assoc_File_Section()
        {
            // Now, start a string builder for the results
            StringBuilder result = new StringBuilder();

            // Add each page as a section
            List<abstract_TreeNode> pageCollection = physicalDivisionTree.Pages_PreOrder;
            foreach (Page_TreeNode thisPage in pageCollection)
            {
                // Find the JPEG file for this page
                foreach (SobekCM_File_Info thisFile in thisPage.Files)
                {
                    // Is this the JPEG?  Also, exclude any thumbnail JPEGs for now
                    string extension = thisFile.File_Extension;
                    if (((extension == "JPEG") || ( extension == "JPG" )) && (thisFile.System_Name.ToUpper().IndexOf("THM") < 0))
                    {
                        // Add this as an associated file
                        result.Append("    <Metadata name=\"gsdlassocfile\">" + thisFile.System_Name + ":image/jpg:</Metadata>\r\n");
                    }
                }
            }

            return result.ToString();
        }

        /// <summary> Returns the detailed file section for the Greenstone Archival format </summary>
        /// <returns> XML of the detailed file section for the Greenstone Archival format </returns>
        internal string GSA_Detailed_File_Section(string BibID, string VID, bool textDisplayable, string Text_File_Directory)
        {
            // Need to build a collection of all the pages, in sequence
            List<Divisions.abstract_TreeNode> pageCollection = physicalDivisionTree.Pages_PreOrder;

            // Now, start a string builder for the results
            StringBuilder result = new StringBuilder();

            string text_directory = this.source_directory;
            if (Text_File_Directory.Length > 0)
                text_directory = Text_File_Directory;

            // Add each page as a section
            ushort order = 1;
            foreach (Page_TreeNode thisPage in pageCollection)
            {
                result.Append(thisPage.toGSA(BibID, VID, order++, text_directory, textDisplayable));
            }

            return result.ToString();
        }
        #endregion
    }
}
