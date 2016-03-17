using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the web-specific information from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Web_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            if (New.Web == null)
                New.Web = new BriefItem_Web();

            // Set the primary date
            string date = Original.Bib_Info.Origin_Info.Date_Created;
            if (String.IsNullOrEmpty(Original.Bib_Info.Origin_Info.Date_Created))
                date = Original.Bib_Info.Origin_Info.Date_Issued;
            New.Web.Date = date;

            // Copy over the primary keys
            New.Web.ItemID = Original.Web.ItemID;
            New.Web.GroupID = Original.Web.GroupID;

            // Add other (fairly random) stuff held in the web portion
            New.Web.Siblings = Original.Web.Siblings;
            New.Web.Source_URL = Original.Web.Source_URL;

            // Step through all the files and collection file extensions
            List<string> extensions = new List<string>();
            collect_extensions(Original.Divisions.Download_Tree, extensions);
            collect_extensions(Original.Divisions.Physical_Tree, extensions);
            New.Web.File_Extensions = extensions;

            // Copy over the related titles
            if (Original.Web.Related_Titles_Count > 0)
            {
                foreach (Resource_Object.Behaviors.Related_Titles origTitle in Original.Web.All_Related_Titles)
                {
                    New.Web.Add_Related_Title(origTitle.Relationship, origTitle.Title, origTitle.Link);
                }
            }

            // Set the additional link to show under the title, in the title box
            if ((Original.Bib_Info.hasLocationInformation) && (Original.Bib_Info.Location.Other_URL.Length > 0))
            {
                // Exclude YOUTUBE urls
                if (Original.Bib_Info.Location.Other_URL.ToLower().IndexOf("www.youtube.com") < 0)
                {
                    // Determine the type of link
                    New.Web.Title_Box_Additional_Link_Type = "Related Link";
                    if ( !String.IsNullOrEmpty(Original.Bib_Info.Location.Other_URL_Display_Label))
                    {
                        New.Web.Title_Box_Additional_Link_Type = Original.Bib_Info.Location.Other_URL_Display_Label;
                    }


                    // Determine the display value
                    string note = Original.Bib_Info.Location.Other_URL;
                    if (Original.Bib_Info.Location.Other_URL_Note.Length > 0)
                    {
                        note = Original.Bib_Info.Location.Other_URL_Note;
                    }

                    // Add the link
                    New.Web.Title_Box_Additional_Link = "<a href=\"" + Original.Bib_Info.Location.Other_URL + "\">" + note + "</a>";
                }
            }

            return true;
        }

        private void collect_extensions(Division_Tree Tree, List<string> Extensions)
        {
            // If not roots, do nothing!
            if ((Tree == null) || (Tree.Roots == null) || (Tree.Roots.Count == 0))
                return;

            // Now, step through all the nodes
            foreach (abstract_TreeNode thisNode in Tree.Roots)
            {
                recurse_through_nodes(thisNode, Extensions);
            }
        }

        private void recurse_through_nodes(abstract_TreeNode Node, List<string> Extensions)
        {
            // Was this node a page?
            if (Node.Page)
            {
                // Cast back to the PAGE node
                Page_TreeNode pageNode = (Page_TreeNode)Node;

                // If no files, do not add this back
                if (pageNode.Files.Count == 0)
                    return;

                // Add a filenode for each file
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    string extension = thisFile.File_Extension.ToLower();
                    if (!Extensions.Contains(extension))
                        Extensions.Add(extension);
                }
            }
            else
            {
                // This was a division node
                Division_TreeNode divNode = (Division_TreeNode)Node;

                // Look for children nodes
                foreach (abstract_TreeNode childNode in divNode.Nodes)
                {
                    // Visit each child node 
                    recurse_through_nodes(childNode, Extensions);
                }
            }
        }
    }
}
