#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.SiteMap
{
    /// <summary> SiteMap contains hierarchical site map pages and is used to render a tree-view
    /// navigation control to the left on non-aggregation related web content pages </summary>
    [DataContract]
    public class SobekCM_SiteMap
    {
        private string defaultBreadcrumb;
        private string restrictedRobotUrl;

        /// <summary> Constructor for a new instance of the SobekCM_SiteMap class </summary>
        public SobekCM_SiteMap()
        {
            Width = -1;
            RootNodes = new List<SobekCM_SiteMap_Node>();
        }

        /// <summary> Gets a flag indicating if this sitemap has a URL restriction for indexing robots </summary>
        public bool Is_URL_Restricted_For_Robots
        {
            get
            {
                return !String.IsNullOrEmpty(restrictedRobotUrl);
            }
        }

        /// <summary> Gets and sets the URL which robots are required to use to index these pages </summary>
        /// <remarks> This is an optional field in the sitemaps and this will be an empty string if no restriction is provided </remarks>
        public string Restricted_Robot_URL
        {
            get { return restrictedRobotUrl ?? String.Empty; }
            set { restrictedRobotUrl = value; }
        }

        /// <summary> Gets and sets the width for the left navigational tree. </summary>
        /// <remarks> This is an optional field in the sitemaps and this value will be -1 if there is no width provided </remarks>
        public short Width { get; set; }

        /// <summary> Gets and sets the default breadcrumb root to be displayed before the calculated breadcrumb </summary>
        public string Default_Breadcrumb
        {
            get { return defaultBreadcrumb ?? String.Empty; }
            set { defaultBreadcrumb = value; }
        }

        /// <summary> Gets and sets the list of root nodes for this site map </summary>
        public List<SobekCM_SiteMap_Node> RootNodes { get; set; }

        /// <summary> Gets the currently selected node value, based on the current URL </summary>
        /// <param name="Current_URL"> Current URL </param>
        /// <returns> Node value of the (first) selected node </returns>
        public int Selected_NodeValue(string Current_URL)
        {
            foreach (SobekCM_SiteMap_Node rootNode in RootNodes)
            {
                if (rootNode.URL == Current_URL)
                    return rootNode.NodeValue;

                if (rootNode.Child_Nodes_Count > 0)
                {
                    foreach (int returnValue in rootNode.Child_Nodes.Select(childNode => recurse_selected_nodevalue_find(childNode, Current_URL)).Where(returnValue => returnValue > 0))
                    {
                        return returnValue;
                    }
                }
            }

            return -1;
        }

        private int recurse_selected_nodevalue_find(SobekCM_SiteMap_Node Node, string Current_URL)
        {
            if (Node.URL == Current_URL)
                return Node.NodeValue;

            if (Node.Child_Nodes_Count > 0)
            {
                foreach (int returnValue in Node.Child_Nodes.Select(childNode => recurse_selected_nodevalue_find(childNode, Current_URL)).Where(returnValue => returnValue > 0))
                {
                    return returnValue;
                }
            }

            return -1;
        }
        
        /// <summary> Gets a node from this tree by its unique node value </summary>
        /// <param name="NodeValue"> Node value to retrieve </param>
        /// <returns> Either NULL or the matching sitemap node</returns>
        public SobekCM_SiteMap_Node Node_By_Value( int NodeValue )
        {
            if ((RootNodes == null) || (RootNodes.Count == 0))
                return null;

            return RootNodes.Select(rootNode => find_node_by_value(rootNode, NodeValue)).FirstOrDefault(returnValue => returnValue != null);
        }

        private SobekCM_SiteMap_Node find_node_by_value(SobekCM_SiteMap_Node Node, int NodeValue)
        {
            // If this is the matching node, return it
            if (Node.NodeValue == NodeValue)
                return Node;

            // If no children or first child is already invalid, return NULL
            if ((Node.Child_Nodes_Count == 0) || ( Node.Child_Nodes[0].NodeValue > NodeValue ))
                return null;

            // Look through the children nodes
            int nodeNumber = 0;
            ReadOnlyCollection<SobekCM_SiteMap_Node> childNodes = Node.Child_Nodes;
            while (nodeNumber < ( Node.Child_Nodes_Count - 1 ))
            {
                // Is the NEXT child node value too great, then it would be in THIS node's child tree
                if (childNodes[nodeNumber + 1].NodeValue > NodeValue)
                {
                    return find_node_by_value(childNodes[nodeNumber], NodeValue);
                }
                nodeNumber++;
            }

            // Must still be in the last child node tree, the VERY LAST node
            return find_node_by_value(childNodes[childNodes.Count - 1], NodeValue);
        }
    }
}
