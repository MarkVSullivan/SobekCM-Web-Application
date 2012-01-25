#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Library.SiteMap
{
    /// <summary> Single node within a hierarchical site map used to render a tree-view
    /// navigation control to the left on non-aggregation related web content pages </summary>
    [Serializable]
    public class SobekCM_SiteMap_Node
    {
        /// <summary> Description for this single node within a hierarchical site map, used for 
        /// a tooltip when hovering over this node in the navigation tree </summary>
        public readonly string Description;

        /// <summary> Unique node value used to select this node from the site map during 
        /// callbacks due to using the left navigational tree structure </summary>
        public readonly int NodeValue;

        /// <summary> Title for this single node within a hierarchical site map, displayed in the
        /// navigation tree </summary>
        public readonly string Title;

        /// <summary> Relative URL for this single node within a hierarchical site map </summary>
        public readonly string URL;

        private List<SobekCM_SiteMap_Node> childNodes;

        /// <summary> Constructor for a new instancee of the SobekCM_SiteMap_Node class </summary>
        /// <param name="URL">Relative URL for this single node within a hierarchical site map</param>
        /// <param name="Title">Title for this single node within a hierarchical site map, displayed in the
        /// navigation tree </param>
        /// <param name="Description">Description for this single node within a hierarchical site map, used for 
        /// a tooltip when hovering over this node in the navigation tree</param>
        /// <param name="NodeValue"> Unique node value used to select this node from the site map during callbacks due to using the left navigational tree structure </param>
        public SobekCM_SiteMap_Node(string URL, string Title, string Description, int NodeValue )
        {
            this.URL = URL;
            this.Description = Description;
            this.Title = Title;
            this.NodeValue = NodeValue;
        }

        /// <summary> Gets or sets the link back to the parent node </summary>
        public SobekCM_SiteMap_Node Parent_Node { get; set; }

        /// <summary> Gets the number of children nodes under this node </summary>
        public int Child_Nodes_Count
        {
            get
            {
                if (childNodes == null)
                    return 0;
                return childNodes.Count;
            }
        }

        /// <summary> Gets the read-only collection of child nodes under this node  </summary>
        public ReadOnlyCollection<SobekCM_SiteMap_Node> Child_Nodes
        {
            get
            {
                return new ReadOnlyCollection<SobekCM_SiteMap_Node>(childNodes);
            }
        }

        /// <summary> Add a child node to this node </summary>
        /// <param name="Child_URL">Relative URL for the child node within a hierarchical site map</param>
        /// <param name="Child_Title">Title for the child node within a hierarchical site map, displayed in the
        /// navigation tree </param>
        /// <param name="Child_Description">Description for the child node within a hierarchical site map, used for 
        /// a tooltip when hovering over this node in the navigation tree</param>
        /// <param name="Child_NodeValue"> Unique node value used to select this node from the site map during callbacks due to using the left navigational tree structure </param>
        /// <returns> Fully built SobekCM_SiteMap_Node child object </returns>
        public SobekCM_SiteMap_Node Add_Child_Node(string Child_URL, string Child_Title, string Child_Description, int Child_NodeValue )
        {
            SobekCM_SiteMap_Node newNode = new SobekCM_SiteMap_Node(Child_URL, Child_Title, Child_Description, Child_NodeValue);
            Add_Child_Node(newNode);
            newNode.Parent_Node = this;
            return newNode;
        }

        /// <summary> Add a child node to this node </summary>
        /// <param name="Child_Node"> Child node to add </param>
        public void Add_Child_Node(SobekCM_SiteMap_Node Child_Node)
        {
            if (childNodes == null)
                childNodes = new List<SobekCM_SiteMap_Node>();

            childNodes.Add(Child_Node);
            Child_Node.Parent_Node = this;
        }
    }
}
