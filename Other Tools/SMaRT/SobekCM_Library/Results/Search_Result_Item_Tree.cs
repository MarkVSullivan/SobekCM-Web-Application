#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Library.Results
{
    /// <summary> Sorted tree is a small tree structure which sorts each of its main nodes (and then children nodes) 
    /// by a value and allows a single node from the entire tree can be pulled by value. </summary>
    /// <remarks> This is used for the many sorted volumes under a single title in a result or browse view </remarks>
    [Serializable]
    public class Search_Result_Item_Tree
    {
        private readonly Dictionary<string, Search_Result_Item_TreeNode> nodeHash;
        private readonly List<Search_Result_Item_TreeNode> rootNodes;

        /// <summary> Constructor for a new instance of the Sorted_Tree class </summary>
        public Search_Result_Item_Tree()
        {
            rootNodes = new List<Search_Result_Item_TreeNode>();
            nodeHash = new Dictionary<string, Search_Result_Item_TreeNode>();
        }

        /// <summary> Read-only list of all the main root nodes for this sorted tree </summary>
        public ReadOnlyCollection<Search_Result_Item_TreeNode> Root_Nodes
        {
            get
            {
                return new ReadOnlyCollection<Search_Result_Item_TreeNode>(rootNodes);
            }
        }

        /// <summary> Add new root node to this tree </summary>
        /// <param name="Name"> Display name for this new root node (title of the item group at the root node level) </param>
        /// <param name="Link"> Link for this new root node of the tree, or empty if no link </param>
        /// <param name="Value"> Value of this new root node (actually the BibID + '_' + VID) </param>
        /// <returns> Built sorted tree node object </returns>
        public Search_Result_Item_TreeNode Add_Root_Node(string Name, string Link, string Value)
        {
            Search_Result_Item_TreeNode newRootNode = new Search_Result_Item_TreeNode(Name, Link, Value);
            rootNodes.Add(newRootNode);
            return newRootNode;
        }

        /// <summary> Pulls a single node from the entire tree hierarchy by value </summary>
        /// <param name="Value"> Value for the node to retrieve </param>
        /// <returns> Sorted tree node object which matches value request, or NULL </returns>
        /// <remarks> For this to effectively work, the <see cref="Set_Values()"/> method must be called after all
        /// nodes are added to this tree.</remarks>
        public Search_Result_Item_TreeNode Get_Node_By_Value(string Value)
        {
            return nodeHash.ContainsKey(Value) ? nodeHash[Value] : null;
        }

        /// <summary> Builds the dictionary of values to tree nodes to allow nodes to be pulled 
        /// from anywhere in the tree hierarchy by value </summary>
        public void Set_Values()
        {
            nodeHash.Clear();
            foreach (Search_Result_Item_TreeNode rootNode in rootNodes)
            {
                nodeHash[rootNode.Value] = rootNode;
                recurse_and_set_values(rootNode, rootNode.Value);
            }
        }

        private void recurse_and_set_values(Search_Result_Item_TreeNode parent_node, string parent_value)
        {
            foreach (Search_Result_Item_TreeNode childNode in parent_node.ChildNodes)
            {
                childNode.Value = parent_value + "_" + childNode.Name;
                nodeHash[childNode.Value] = childNode;
                recurse_and_set_values(childNode, childNode.Value);
            }
        }
    }
}
