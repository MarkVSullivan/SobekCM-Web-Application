#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

#endregion

namespace SobekCM.Resource_Object.Divisions
{
    /// <summary> Stores information about the divisions and files associated with this resource </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Division_Info : XML_Writing_Base_Type
    {
        private string source_directory;
        
        private Division_Tree downloadDivisionTree;
        private Division_Tree physicalDivisionTree;

        private List<Outer_Division_Info> outerDivisions;  

        private int directly_set_page_count;
        private bool suppress_checksum;


        /// <summary> Constructor creates a new instance of the Divison_Info class </summary>
        public Division_Info()
        {
            physicalDivisionTree = new Division_Tree();
            downloadDivisionTree = new Division_Tree();
            source_directory = String.Empty;
            suppress_checksum = true;
            directly_set_page_count = 0;
        }

        #region Methods and properties about the outer divisions 

        /// <summary> Get the number of outer divisions </summary>
        public int Outer_Division_Count
        {
            get 
            {
                return outerDivisions == null ? 0 : outerDivisions.Count;
            }
        }

        /// <summary> Returns the readonly collection of outer divisions  </summary>
        public ReadOnlyCollection<Outer_Division_Info> Outer_Divisions
        {
            get
            {
                if (outerDivisions == null)
                    return new ReadOnlyCollection<Outer_Division_Info>(new List<Outer_Division_Info>());
                return new ReadOnlyCollection<Outer_Division_Info>(outerDivisions);
            }
        }

        /// <summary> Clears the outer divisions which wrap around the 
        /// rest of the structure map and explain how this resource relates to the larger series or 
        /// set of resources   </summary>
        public void Clear_Outer_Divisions()
        {
            if (outerDivisions != null)
                outerDivisions.Clear();
        }

        /// <summary> Adds a new outer division  </summary>
        /// <param name="Label">Textual label for this outer division ( i.e., 'Volume 4', 'Issue 2')</param>
        /// <param name="OrderLabel">Numeric order information associated with the label</param>
        /// <param name="Type">Type of division ( i.e., volume, issue, etc.. )</param>
        public void Add_Outer_Division(string Label, int OrderLabel, string Type)
        {
            if (outerDivisions == null)
                outerDivisions = new List<Outer_Division_Info>();
            outerDivisions.Add(new Outer_Division_Info(Label, OrderLabel, Type));
        }

        /// <summary> Flag indicates if an outer division with the label and type already exists  </summary>
        /// <param name="Label"> Label for the outer division to check for </param>
        /// <param name="Type"> Type of the outer division to check for </param>
        /// <returns> TRUE if the outer division exists, otherwise FALSE </returns>
        public bool Contains_Outer_Division(string Label, string Type)
        {
            if (outerDivisions == null)
                return false;

            foreach (Outer_Division_Info thisDiv in outerDivisions)
            {
                if ((String.Compare(thisDiv.Type, Type, StringComparison.OrdinalIgnoreCase) == 0) && (String.Compare(thisDiv.Label, Label, StringComparison.OrdinalIgnoreCase) == 0))
                    return true;
            }
            return false;
        }

        #endregion

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
                    Page_TreeNode pageNode = (Page_TreeNode) thisNode;
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
                    Page_TreeNode pageNode = (Page_TreeNode) thisNode;
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
            set { directly_set_page_count = value; }
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
                if (((physicalDivisionTree == null) || (physicalDivisionTree.Roots.Count == 0)) && ((downloadDivisionTree == null) || (downloadDivisionTree.Roots.Count == 0)))
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
            get { return physicalDivisionTree.All_Files; }
        }

        /// <summary> Gets the collection of all download/other files associated with this resource </summary>
        public List<SobekCM_File_Info> Download_Other_Files
        {
            get { return downloadDivisionTree.All_Files; }
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

                return Convert_String_To_XML_Safe(tocBuilder.ToString());
            }
        }

        #endregion

        #region Public Methods

        /// <summary> Clear all divisions and all files </summary>
        public void Clear()
        {
            physicalDivisionTree.Clear();
            downloadDivisionTree.Clear();
            Clear_Outer_Divisions();
        }

        #endregion

        #region Methods to clear and create the checksums for all the files in this package

        /// <summary> Returns flag indicating it needs checksums calculated </summary>
        public bool Needs_Checksums
        {
            get
            {
                // Get the list of all pages from this division 
                List<abstract_TreeNode> pageNodes = physicalDivisionTree.Pages_PreOrder;
                foreach (Page_TreeNode pageNode in pageNodes)
                {
                    // Step through each file
                    foreach (SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        // Recalculate this file info?
                        if ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0))
                        {
                            return true;
                        }
                    }
                }

                // Get the list of all pages from the downloads/other files tree 
                pageNodes = downloadDivisionTree.Pages_PreOrder;
                foreach (Page_TreeNode pageNode in pageNodes)
                {
                    // Step through each file
                    foreach (SobekCM_File_Info thisFile in pageNode.Files)
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

        /// <summary> Clears the checksums associated with all the page file images </summary>
        public void Clear_Checksums()
        {
            // Get the list of all pages from this division 
            List<abstract_TreeNode> pageNodes = physicalDivisionTree.Pages_PreOrder;
            List<abstract_TreeNode> fileNodes = downloadDivisionTree.Pages_PreOrder;

            foreach (Page_TreeNode pageNode in pageNodes)
            {
                // Step through each file
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    thisFile.Checksum_Type = null;
                    thisFile.Checksum = null;
                }
            }
            foreach (Page_TreeNode pageNode in fileNodes)
            {
                // Step through each file
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    thisFile.Checksum_Type = null;
                    thisFile.Checksum = null;
                }
            }
        }


        /// <summary> Recalculates all the checksums for associated files </summary>
        /// <param name="Recalculate_All"> Flag indicates whether to recalculate all files, regardless of need </param>
        /// <param name="Progress_Delegate"> Progress delegate is called to update any progress displays </param>
        public void Calculate_Checksum(bool Recalculate_All, New_SobekCM_Bib_Package_Progress_Task_Group Progress_Delegate)
        {
            // Get the list of all pages from this division trees
            List<abstract_TreeNode> pageNodes = physicalDivisionTree.Pages_PreOrder;
            List<abstract_TreeNode> fileNodes = downloadDivisionTree.Pages_PreOrder;

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
                    if ((thisFile.METS_LocType == SobekCM_File_Info_Type_Enum.SYSTEM) && ((Recalculate_All) || ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0))))
                    {
                        // Perform in a try catch
                        try
                        {
                            // Get the size first
                            FileInfo thisFileInfo = new FileInfo(source_directory + "/" + thisFile.System_Name);
                            thisFile.Size = thisFileInfo.Length;

                            // Get the checksum, if it doesn't exist
                            if ((String.IsNullOrEmpty(thisFile.Checksum)) || (Recalculate_All))
                            {
                                thisFile.Checksum = checksummer.Calculate_Checksum(source_directory + "/" + thisFile.System_Name);
                                thisFile.Checksum_Type = "MD5";
                            }
                        }
                        catch {}

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
                    if ((thisFile.METS_LocType == SobekCM_File_Info_Type_Enum.SYSTEM) && ((Recalculate_All) || ((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0))))
                    {
                        // Perform in a try catch
                        try
                        {
                            // Get the size first
                            FileInfo thisFileInfo = new FileInfo(source_directory + "/" + thisFile.System_Name);
                            thisFile.Size = thisFileInfo.Length;

                            // Get the checksum, if it doesn't exist
                            if ((String.IsNullOrEmpty(thisFile.Checksum)) || (Recalculate_All))
                            {
                                thisFile.Checksum = checksummer.Calculate_Checksum(source_directory + "/" + thisFile.System_Name);
                                thisFile.Checksum_Type = "MD5";
                            }
                        }
                        catch{}

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

    }
}