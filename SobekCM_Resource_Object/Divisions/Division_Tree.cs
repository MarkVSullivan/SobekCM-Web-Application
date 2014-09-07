#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Resource_Object.Divisions
{
    /// <summary> Division_Tree is a data object in memory which stores a hierarchy of 
    /// TreeNode objects which represent divisions, pages, and files </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center. </remarks>
    [Serializable]
    [DataContract]
    public class Division_Tree
    {
        /// <summary> Stores the root node for this tree </summary>
        private List<abstract_TreeNode> rootNodes;

        /// <summary> Constructor creates a new instance of the Division_Tree class </summary>
        public Division_Tree()
        {
            rootNodes = new List<abstract_TreeNode>();
        }

        /// <summary> Gets the root nodes for this tree </summary>
        [DataMember]
        public List<abstract_TreeNode> Roots
        {
            get { return rootNodes; }
        }

        /// <summary> Flag indicates if there are any files in this division tree  </summary>
        public bool Has_Files
        {
            get
            {
                foreach (abstract_TreeNode TreeNode in rootNodes)
                {
                    if (recursively_check_for_any_files(TreeNode))
                        return true;
                }
                return false;
            }
        }

        private bool recursively_check_for_any_files(abstract_TreeNode Node)
        {
            if (Node.Page)
            {
                if (((Page_TreeNode) Node).Files.Count > 0)
                    return true;
            }

            if (!Node.Page)
            {
                Division_TreeNode divNode = (Division_TreeNode) Node;
                foreach (abstract_TreeNode TreeNode in divNode.Nodes)
                {
                    if (recursively_check_for_any_files(TreeNode))
                        return true;
                }
            }

            return false;
        }

        /// <summary> Clears this tree completely </summary>
        public void Clear()
        {
            rootNodes.Clear();
        }

        /// <summary> Adds a file (with the appropriate divisions and pages) to this tree by filename  </summary>
        /// <param name="FileName"> Name of the file to add </param>
        /// <returns> Newly built <see cref="SobekCM_File_Info" /> object which has been added to this tree </returns>
        /// <remarks> This is generally used to add just a single file.  To add many files, better logic should be implemented </remarks>
        public SobekCM_File_Info Add_File(string FileName)
        {
            return Add_File(FileName, String.Empty);
        }

        /// <summary> Adds a file (with the appropriate divisions and pages) to this tree by filename  </summary>
        /// <param name="FileName"> Name of the file to add </param>
        /// <param name="Label"> Label for the page containing this file, if it is a new page </param>
        /// <returns> Newly built <see cref="SobekCM_File_Info" /> object which has been added to this tree </returns>
        /// <remarks> This is generally used to add just a single file.  To add many files, better logic should be implemented </remarks>
        public SobekCM_File_Info Add_File(string FileName, string Label)
        {
            SobekCM_File_Info newFile = new SobekCM_File_Info(FileName);
            Add_File(newFile, Label);
            return newFile;
        }

        /// <summary> Adds a file  object (with the appropriate divisions and pages) to this tree </summary>
        /// <param name="New_File"> New file object to add </param>
        /// <remarks> This is generally used to add just a single file.  To add many files, better logic should be implemented </remarks>
        public void Add_File(SobekCM_File_Info New_File)
        {
            Add_File(New_File, String.Empty);
        }

        /// <summary> Adds a file  object (with the appropriate divisions and pages) to this tree </summary>
        /// <param name="New_File"> New file object to add </param>
        /// <param name="Label"> Label for the page containing this file, if it is a new page </param>
        /// <remarks> This is generally used to add just a single file.  To add many files, better logic should be implemented </remarks>
        public void Add_File(SobekCM_File_Info New_File, string Label)
        {
            // Determine the upper case name
            string systemname_upper = New_File.File_Name_Sans_Extension;

            // Look for a page/entity which has the same file name, else it will be added to the last division
            foreach (abstract_TreeNode rootNode in Roots)
            {
                if (recursively_add_file(rootNode, New_File, systemname_upper))
                {
                    return;
                }
            }

            // If not found, find the last division
            if (Roots.Count > 0)
            {
                if (!Roots[Roots.Count - 1].Page)
                {
                    // Get his last division
                    Division_TreeNode lastDivision = (Division_TreeNode) Roots[Roots.Count - 1];

                    // Find the last division then
                    while ((lastDivision.Nodes.Count > 0) && (!lastDivision.Nodes[lastDivision.Nodes.Count - 1].Page))
                    {
                        lastDivision = (Division_TreeNode) lastDivision.Nodes[lastDivision.Nodes.Count - 1];
                    }

                    // Add this as a new page on the last division
                    Page_TreeNode newPage = new Page_TreeNode(Label);
                    lastDivision.Add_Child(newPage);

                    // Now, add this file to the page
                    newPage.Files.Add(New_File);
                }
                else
                {
                    // No divisions at all, but pages exist at the top level, which is okay
                    Page_TreeNode pageNode = (Page_TreeNode) Roots[Roots.Count - 1];

                    // Now, add this file to the page
                    pageNode.Files.Add(New_File);
                }
            }
            else
            {
                // No nodes exist, so add a MAIN division node
                Division_TreeNode newDivNode = new Division_TreeNode("Main", String.Empty);
                Roots.Add(newDivNode);

                // Add this as a new page on the new division
                Page_TreeNode newPage = new Page_TreeNode(Label);
                newDivNode.Add_Child(newPage);

                // Now, add this file to the page
                newPage.Files.Add(New_File);
            }
        }

        private bool recursively_add_file(abstract_TreeNode Node, SobekCM_File_Info New_File, string SystemName_Upper)
        {
            // If this is a page, check for a match first
            if (Node.Page)
            {
                Page_TreeNode pageNode = (Page_TreeNode) Node;
                if (pageNode.Files.Count >= 1)
                {
                    if (pageNode.Files[0].File_Name_Sans_Extension == SystemName_Upper)
                    {
                        // Belongs to this page.  Now, just make sure it doesn't already exist
                        foreach (SobekCM_File_Info thisFile in pageNode.Files)
                        {
                            if (thisFile.System_Name.ToUpper() == New_File.System_Name.ToUpper())
                                return true;
                        }

                        // Not found, so add it to this page
                        pageNode.Files.Add(New_File);
                        return true;
                    }
                }
            }

            // If this was a division, check all pages
            if (!Node.Page)
            {
                Division_TreeNode divNode = (Division_TreeNode) Node;
                foreach (abstract_TreeNode childNodes in divNode.Nodes)
                {
                    if (recursively_add_file(childNodes, New_File, SystemName_Upper))
                    {
                        return true;
                    }
                }
            }

            // If nothing found that matches under this node, return false
            return false;
        }

        /// <summary> Returns the page sequence for the indicated file name </summary>
        /// <param name="FileName"> Name of the file to check for similar existence </param>
        /// <returns> The page sequence (first page = index 1), or -1 if it does not exist </returns>
        public int Page_Sequence_By_FileName(string FileName)
        {
            List<abstract_TreeNode> pages = Pages_PreOrder;
            string filename_upper = FileName.ToUpper();
            int page_sequence = 1;
            foreach (Page_TreeNode thisPage in pages)
            {
                if ((thisPage.Files.Count > 0) && (filename_upper == thisPage.Files[0].File_Name_Sans_Extension))
                    return page_sequence;
                page_sequence++;
            }

            return -1;
        }

        #region Methods to return all divisions or the page divisions in preorder 

        /// <summary> Gets all the nodes on the tree in pre-order traversal </summary>
        public List<abstract_TreeNode> Divisions_PreOrder
        {
            get
            {
                // Build the return collection
                List<abstract_TreeNode> returnVal = new List<abstract_TreeNode>();

                // Do the preorder build on each root node
                foreach (abstract_TreeNode rootNode in Roots)
                {
                    preorder_build(returnVal, rootNode, false);
                }

                // Return the built collection
                return returnVal;
            }
        }

        /// <summary> Gets all the page nodes on the tree in pre-order traversal </summary>
        public List<abstract_TreeNode> Pages_PreOrder
        {
            get
            {
                // Build the return collection
                List<abstract_TreeNode> returnVal = new List<abstract_TreeNode>();

                // Do the preorder build on each root node
                foreach (abstract_TreeNode rootNode in Roots)
                {
                    preorder_build(returnVal, rootNode, true);
                }

                // Return the built collection
                return returnVal;
            }
        }

        private void preorder_build(List<abstract_TreeNode> collection, abstract_TreeNode thisNode, bool only_add_pages)
        {
            // Since this is pre-order, first 'visit' this
            if (!only_add_pages)
            {
                collection.Add(thisNode);
            }
            else
            {
                // If we are just getting pages, only add if it is not already added
                if ((thisNode.Page) && (!collection.Contains(thisNode)))
                    collection.Add(thisNode);
            }

            // is this a division node? .. which can have children ..
            if (!thisNode.Page)
            {
                Division_TreeNode thisDivNode = (Division_TreeNode) thisNode;

                // Do the same for all the children
                foreach (abstract_TreeNode childNode in thisDivNode.Nodes)
                {
                    preorder_build(collection, childNode, only_add_pages);
                }
            }
        }

        #endregion

        #region Method to return the list of all files

        /// <summary> Gets the list of all files which belong to this division tree  </summary>
        public List<SobekCM_File_Info> All_Files
        {
            get
            {
                List<SobekCM_File_Info> returnValue = new List<SobekCM_File_Info>();
                List<Page_TreeNode> handledPages = new List<Page_TreeNode>();
                foreach (abstract_TreeNode thisNode in rootNodes)
                    recursively_build_all_files_list(returnValue, handledPages, thisNode);
                return returnValue;
            }
        }

        private void recursively_build_all_files_list(List<SobekCM_File_Info> returnValue, List<Page_TreeNode> handledPages, abstract_TreeNode thisNode)
        {
            // Since this is pre-order, first 'visit' this
            if (thisNode.Page)
            {
                Page_TreeNode pageNode = (Page_TreeNode) thisNode;
                if (!handledPages.Contains(pageNode))
                {
                    foreach (SobekCM_File_Info file in pageNode.Files)
                    {
                        returnValue.Add(file);
                    }
                    handledPages.Add(pageNode);
                }
            }

            // is this a division node? .. which can have children ..
            if (!thisNode.Page)
            {
                Division_TreeNode thisDivNode = (Division_TreeNode) thisNode;

                // Do the same for all the children
                foreach (abstract_TreeNode childNode in thisDivNode.Nodes)
                {
                    recursively_build_all_files_list(returnValue, handledPages, childNode);
                }
            }
        }

        #endregion
    }
}