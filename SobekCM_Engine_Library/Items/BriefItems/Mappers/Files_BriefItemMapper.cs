using System.Collections.Generic;
using System.Linq;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the files from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Files_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Step through each of the nodes within the images first
            List<BriefItem_FileGrouping> images = new List<BriefItem_FileGrouping>();
            List<BriefItem_FileGrouping> downloads = new List<BriefItem_FileGrouping>();
            List<BriefItem_TocElement> images_toc = new List<BriefItem_TocElement>();
            List<BriefItem_TocElement> downloads_toc = new List<BriefItem_TocElement>();

            // Do the images (i.e., physical tree) first
            collect_nodes(Original.Divisions.Physical_Tree, images, images_toc);

            // If there were groupings and TOCs assigned, add them
            if (images.Count > 0)
                New.Images = images;
            if (images_toc.Count > 0)
                New.Images_TOC = images_toc;

            // Collect the downloads next
            collect_nodes(Original.Divisions.Download_Tree, downloads, downloads_toc);

            // If there were groupings and TOCs assigned, add them
            if (downloads.Count > 0)
                New.Downloads = downloads;
            if (downloads_toc.Count > 0)
                New.Downloads_TOC = downloads_toc;

            // No exception
            return true;
        }

        private void collect_nodes(Division_Tree Tree, List<BriefItem_FileGrouping> Groupings, List<BriefItem_TocElement> Toc)
        {
            // If not roots, do nothing!
            if (( Tree == null ) || ( Tree.Roots == null ) || ( Tree.Roots.Count == 0 ))
                return;

            // Create the stack used for determining the TOC
            Stack<BriefItem_TocElement> currDivStack = new Stack<BriefItem_TocElement>();

            // Start at the very top?
            List<abstract_TreeNode> rootNodes = Tree.Roots;
            if ((rootNodes.Count == 1) && (!rootNodes[0].Page))
            {
                // This was a division node
                Division_TreeNode divNode = (Division_TreeNode) Tree.Roots[0];

                if ((divNode.Nodes != null) && (divNode.Nodes.Count > 0))
                {
                    rootNodes = divNode.Nodes;
                }
            }

            // Now, step through all the nodes
            foreach (abstract_TreeNode thisNode in rootNodes)
            {
                recurse_through_nodes(thisNode, Groupings, Toc, currDivStack, 1);
            }
        }

        private bool recurse_through_nodes(abstract_TreeNode Node, List<BriefItem_FileGrouping> Groupings, List<BriefItem_TocElement> Toc, Stack<BriefItem_TocElement> CurrDivStack, int Level)
        {
            // Was this node a page?
            if (Node.Page)
            {
                // Cast back to the PAGE node
                Page_TreeNode pageNode = (Page_TreeNode) Node;

                // If no files, do not add this back
                if (pageNode.Files.Count == 0)
                    return false;

                // Create the file grouping object for this
                BriefItem_FileGrouping newNode = new BriefItem_FileGrouping(pageNode.Label);

                // Add a filenode for each file
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    BriefItem_File newFile = new BriefItem_File(thisFile.System_Name);
                    if (thisFile.Width > 0)
                        newFile.Width = thisFile.Width;
                    if (thisFile.Height > 0)
                        newFile.Height = thisFile.Height;
                    newNode.Files.Add(newFile);
                }

                // Add this to the list of images
                Groupings.Add(newNode);

                // Since this was a page with files, return TRUE
                return true;
            }
            else
            {
                // Get what will be the sequence (if it turned out to have pages under it)
                int sequence = Groupings.Count + 1;

                // This was a division node
                Division_TreeNode divNode = (Division_TreeNode) Node;

                // Create the brief item TOC element
                BriefItem_TocElement divToc = new BriefItem_TocElement
                {
                    Level = Level, 
                    Name = divNode.Label
                };
                if (string.IsNullOrEmpty(divToc.Name))
                    divToc.Name = divNode.Type;

                // Add to the stack
                CurrDivStack.Push(divToc);

                // Look for children nodes
                bool some_files_under = false;
                foreach (abstract_TreeNode childNode in divNode.Nodes)
                {
                    // Visit each child node 
                    if (recurse_through_nodes(childNode, Groupings, Toc, CurrDivStack, Level + 1))
                        some_files_under = true;
                }

                // Were there some files under here?
                if (some_files_under)
                {
                    // Now, pop-up all the nodes in the queue and add them 
                    if (CurrDivStack.Count > 0)
                    {
                        IEnumerable<BriefItem_TocElement> reversed = CurrDivStack.Reverse();
                        foreach (BriefItem_TocElement revDiv in reversed)
                        {
                            revDiv.Sequence = sequence;
                            Toc.Add(revDiv);
                        }
                        CurrDivStack.Clear();
                    }
                }
                else
                {
                    // If this division (with NO pages under it apparently) is on the stack, 
                    // just pop it off
                    if (( CurrDivStack.Count > 0 ) && (CurrDivStack.Peek() == divToc))
                    {
                        CurrDivStack.Pop();
                    }
                }

                // Return whether there were any files under this
                return some_files_under;
            }
        }
    }
}
