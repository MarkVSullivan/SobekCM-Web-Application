#region Using directives

using System;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> [DEPRECATED] Item viewer shows table of contents in the main item viewer area </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class TOC_ItemViewer : abstractItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.TOC"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.TOC; }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value -1 </value>
        public override int Viewer_Width
        {
            get
            {
                return -1;
            }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is a single page viewer, so this property always returns the value 1</value>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("TOC_ItemViewer.Add_Main_Viewer_Section", "");
            }

            // Build the value
            StringBuilder builder = new StringBuilder();

            // Save the current viewer code
            string current_view_code = CurrentMode.ViewerCode;

            // Start the citation table
            builder.AppendLine("\t\t<!-- TABLE OF CONTENTS VIEWER OUTPUT -->" );
            builder.AppendLine("\t\t<td align=\"left\"><span class=\"SobekViewerTitle\"><b>Table of Contents</b></span></td></tr>" );
            builder.AppendLine("\t\t<tr><td class=\"SobekDocumentDisplay\">");
            builder.AppendLine("\t\t\t<div class=\"SobekTOC\">");
            builder.AppendLine("\t\t\t\t<table cellpadding=\"5px\">");
            builder.AppendLine("\t\t\t\t\t<tr height=\"5\"> <td> </td> </tr>" );
            builder.AppendLine("\t\t\t\t\t<tr>" );
            builder.AppendLine("\t\t\t\t\t\t<td width=\"15\"> </td>" );
            builder.AppendLine("\t\t\t\t\t\t<td valign=\"top\">");

            builder.AppendLine("\t\t\t\t\t\t\t<b>" + CurrentItem.Bib_Info.Main_Title.Title + "</b><br />" );

            // Create the tree structure
           // ArrayList nodes = Create_Tree_From_DataTable(currentItem.Divisions);

            // Recursively build the result
            builder.AppendLine("\t\t\t\t\t\t\t<table>" + Environment.NewLine);
            //foreach ( TOC_TreeNode thisNode in nodes )
            //{
            //    Add_TOC_TreeNode_HTML( thisNode, builder, "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; " );
            //}
            builder.AppendLine("\t\t\t\t\t\t\t</table>" + Environment.NewLine);


            // Finish the citation table
            builder.AppendLine("\t\t\t\t\t\t</td>" );
            builder.AppendLine("\t\t\t\t\t\t<td width=\"15\"> </td>" );
            builder.AppendLine("\t\t\t\t\t</tr>");
            builder.AppendLine("\t\t\t\t\t<tr height=\"5\"> <td> </td> </tr>");
            builder.AppendLine("\t\t\t\t</table>");
            builder.AppendLine("\t\t\t</div>");
            builder.AppendLine("\t\t</td>");
            builder.AppendLine( "\t\t<!-- END TABLE OF CONTENTS VIEWER OUTPUT -->" );

            // Restore the mode
            CurrentMode.ViewerCode = current_view_code;

            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            MainPlaceHolder.Controls.Add( mainLiteral );
        }

        #region Commented out, obsolete code

        //private ArrayList Create_Tree_From_DataTable( DataTable pageData )
        //{
        //    // If the DataTable is null, just return
        //    if ( pageData == null )
        //        return new ArrayList();

        //// Build the hash table
        //Hashtable treeNodeHashTable = new Hashtable( pageData.Rows.Count * 2 );

        //// Get the general link
        //string currentViewCode = currentMode.ViewerCode;
        //currentMode.ViewerCode = "XXXXXX";
        //string base_link = currentMode.Redirect_URL();
        //currentMode.ViewerCode = currentViewCode;

        //// Go through each row in the table, and add to the hash table
        //TOC_TreeNode thisNode;
        //foreach ( Database.Item_Information.DivisionRow thisRow in pageData.Rows )
        //{
        //    thisNode = new TOC_TreeNode( thisRow.longname.Replace("&apos;","'").Replace("&amp;","&"), base_link.Replace("XXXXXX", thisRow.link_pagesequence.ToString() ));
        //    treeNodeHashTable.Add( thisRow.divisionid, thisNode );
        //}

        //// Create the return value
        //ArrayList returnVal = new ArrayList();

        //// Go through each row in the table, building the tree
        //TOC_TreeNode parentNode;
        //foreach ( Database.Item_Information.DivisionRow thisRow in pageData.Rows )
        //{
        //    // Get the node from the hash table
        //    thisNode = (TOC_TreeNode) treeNodeHashTable[ thisRow.divisionid ];

        //    // Is this a root node?
        //    if (( thisRow.IsparentdivisionidNull() ) || ( thisRow.parentdivisionid < 0 ) )
        //        returnVal.Add( thisNode );
        //    else
        //    {
        //        // Get the parent node and place this under it
        //        if (treeNodeHashTable.ContainsKey( thisRow.parentdivisionid ))
        //        {
        //            parentNode = (TOC_TreeNode) treeNodeHashTable[thisRow.parentdivisionid];
        //            parentNode.Children.Add( thisNode );				
        //        }
        //    }
        //}

        //// Return the built collection
        //return returnVal;
        //}

        //private void Add_TOC_TreeNode_HTML(TOC_TreeNode thisNode, StringBuilder builder, string spacer)
        //{
        //    // Add HTML for this node
        //    builder.Append("\t\t\t\t\t\t\t<tr><td nowrap>" + spacer + "<a href=\"" + thisNode.Link + "\">" + thisNode.Name + "</a></td></tr>" + Environment.NewLine );

        //    // Add info for each child node
        //    foreach (TOC_TreeNode childNode in thisNode.Children)
        //    {
        //        Add_TOC_TreeNode_HTML(childNode, builder, spacer + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ");
        //    }
        //}

        //protected class TOC_TreeNode
        //{
        //    public readonly string Name;
        //    public readonly string Link;
        //    public readonly ArrayList Children;
        //    public TOC_TreeNode( string Name, string Link )
        //    {
        //        this.Name = Name;
        //        this.Link = Link;
        //        Children = new ArrayList();
        //    }
        //    public void Add_Child( TOC_TreeNode ChildNode )
        //    {
        //        Children.Add( ChildNode );
        //    }
        //}

        #endregion
    }
}
