#region Using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Bib_Package.EAD;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays the container list associated with an archival EAD/Finding guide item. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class EAD_Container_List_ItemViewer : abstractItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.EAD_Container_List"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.EAD_Container_List; }
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

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This is a single page viewer, so this property always returns FALSE</value>
        public override bool Show_Page_Selector
        {
            get
            {
                return false;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 650 </value>
        public override int Viewer_Width
        {
            get
            {
                return 750;
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
                Tracer.Add_Trace("EAD_Container_List_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
        }

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("EAD_Container_List_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

            // Build any search terms
            List<string> terms = new List<string>();
            if (CurrentMode.Text_Search.Length > 0)
            {
                // Get any search terms
                if (CurrentMode.Text_Search.Trim().Length > 0)
                {
                    string[] splitter = CurrentMode.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                    terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
                }
            }

            // Build the value
            StringBuilder builder = new StringBuilder(15000);
            builder.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\">Container List</span></td>");
            builder.AppendLine("        </tr>");
            builder.AppendLine("        <tr>");
            builder.AppendLine("          <td>");
            builder.AppendLine("            <div class=\"SobekCitation\">");
            builder.AppendLine("              <br />");
            builder.AppendLine("              <blockquote>" );


            // Step through the top level first
            foreach( Component_Info container in CurrentItem.EAD.Container_Hierarchy.C_Tags )
            {
                // Add this container title and date information first
                builder.Append("<h2>" + container.Unit_Title);
                if (container.Unit_Date.Length > 0)
                    builder.Append(", " + container.Unit_Date);
                builder.AppendLine("</h2>");

                // Add physical extent, if it exists
                if (container.Extent.Length > 0)
                    builder.AppendLine("<strong>" + container.Extent + "</strong><br />");

                // Add the scope content next
                if (container.Scope_And_Content.Length > 0)
                    builder.AppendLine(container.Scope_And_Content);

                // Add any bioghist next
                if (container.Biographical_History.Length > 0)
                    builder.AppendLine(container.Biographical_History);

                // Are there children to this top container
                if (container.Children.Count > 0)
                {
                    // Dump the current builder into a literal
                    Literal newLiteral = new Literal
                                             { Text = Text_Search_Term_Highlighter.Hightlight_Term_In_HTML( builder.ToString(), terms) };
                    placeHolder.Controls.Add(newLiteral);

                    // Clear the contents of the builder
                    builder.Remove(0, builder.Length);

                    // Now, add this as a tree
                    TreeView treeView1 = new TreeView
                                             { Width = new Unit(700), NodeWrap = true, EnableClientScript = true, PopulateNodesFromClient = false };

                    // Set some tree view properties
                    treeView1.TreeNodePopulate += treeView1_TreeNodePopulate;

                    // Add each child tree node
                    foreach (Component_Info child in container.Children)
                    {
                        // Add this node
                        TreeNode childNode = new TreeNode(child.Unit_Title) {SelectAction = TreeNodeSelectAction.None};
                        if (child.DAO_Link.Length > 0)
                        {
                            if (child.DAO_Title.Length > 0)
                            {
                                childNode.Text = child.Unit_Title + " &nbsp; <a href=\"" + child.DAO_Link + "\">" + child.DAO_Title + "</a>";
                            }
                            else
                            {
                                childNode.Text = "<a href=\"" + child.DAO_Link + "\">" + child.Unit_Title + "</a>";
                            }
                        }
 
                        treeView1.Nodes.Add(childNode);

                        // Add the description, if there is one
                        if (child.Scope_And_Content.Length > 0)
                        {
                            TreeNode scopeContentNode = new TreeNode(child.Scope_And_Content)
                                                            {SelectAction = TreeNodeSelectAction.None};
                            childNode.ChildNodes.Add(scopeContentNode);
                        }

                        // Add the grand children
                        if (child.Children.Count > 0)
                        {
                            foreach (Component_Info grandChild in child.Children)
                            {
                                // Add this node
                                TreeNode grandChildNode = new TreeNode(grandChild.Unit_Title)
                                                              {SelectAction = TreeNodeSelectAction.None};
                                if (grandChild.DAO_Link.Length > 0)
                                {
                                    if (grandChild.DAO_Title.Length > 0)
                                    {
                                        grandChildNode.Text = grandChild.Unit_Title + " &nbsp; <a href=\"" + grandChild.DAO_Link + "\">" + grandChild.DAO_Title + "</a>";
                                    }
                                    else
                                    {
                                        grandChildNode.Text = "<a href=\"" + grandChild.DAO_Link + "\">" + grandChild.Unit_Title + "</a>";
                                    }
                                }
                                childNode.ChildNodes.Add(grandChildNode);
                            }
                        }
                    }

                    // Configure the tree view collapsed nodes
                    treeView1.CollapseAll();                    

                    // Add this tree view to the place holder
                    placeHolder.Controls.Add(treeView1);
                }

                // Put some spaces for now
                builder.AppendLine("<br /><br />");                
            }

            builder.AppendLine("              </blockquote>" );
            builder.AppendLine("              <br />");
            builder.AppendLine("            </div>");

            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            placeHolder.Controls.Add(mainLiteral);
        }

        /// <summary> Event handler loads the nodes on request to the serial hierarchy trees when the user requests them
        /// by expanding a node </summary>
        /// <param name="sender"> TreeView object that fired this event </param>
        /// <param name="e"> Event arguments includes the tree node which was expanded </param>
        static void treeView1_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {

        }
    }
}
