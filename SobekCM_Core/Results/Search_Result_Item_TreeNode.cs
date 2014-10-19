#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> Tree node object for a sorted tree which sorts each of its main nodes (and then children nodes) 
    /// by a value and allows a single node from the entire tree can be pulled by value</summary>
    /// <remarks> This tree node object is used by the <see cref="Search_Result_Item_Tree"/> object. </remarks>
    [Serializable]
    public class Search_Result_Item_TreeNode : IEquatable<Search_Result_Item_TreeNode>
    {
        private readonly SortedList<int, List<Search_Result_Item_TreeNode>> childNodes;

        /// <summary> Constructor for a new instance of the Sorted_TreeNode class </summary>
        public Search_Result_Item_TreeNode( )
        {
            Link = String.Empty;
            Name = String.Empty;
            Value = String.Empty;
            childNodes = new SortedList<int, List<Search_Result_Item_TreeNode>>();
        }

        /// <summary> Constructor for a new instance of the Sorted_TreeNode class </summary>
        /// <param name="Name"> Display name for this new node (title of the item) </param>
        /// <param name="Link"> Link for this new node of the tree </param>
        public Search_Result_Item_TreeNode(string Name, string Link)
        {
            this.Link = Link;
            this.Name = Name;
            Value = String.Empty;
            childNodes = new SortedList<int, List<Search_Result_Item_TreeNode>>();
        }

        /// <summary> Constructor for a new instance of the Sorted_TreeNode class </summary>
        /// <param name="Name"> Display name for this new node (title of the item) </param>
        /// <param name="Link"> Link for this new node of the tree </param>
        /// <param name="Value"> Value of this new node (actually the BibID + '_' + VID) </param>
        public Search_Result_Item_TreeNode(string Name, string Link, string Value)
        {
            this.Link = Link;
            this.Name = Name;
            this.Value = Value;
            childNodes = new SortedList<int, List<Search_Result_Item_TreeNode>>();
        }

        /// <summary> Gets and sets the URL link for this node </summary>
        /// <value> This is the relative link to this item in this digital library </value>
        public string Link { get; set; }

        /// <summary> Gets and sets the name to display for this node </summary>
        /// <value> This is usually the individual volume title, or part of the serial heirarchy</value>
        public string Name { get; set; }

        /// <summary> Gets and sets the value for this node </summary>
        /// <value> The value of a node is usually the BibID + '_' + VID of the item represented </value>
        public string Value { get; set; }

        /// <summary> Gets the list of child nodes </summary>
        public List<Search_Result_Item_TreeNode> ChildNodes
        {
            get 
            {
                List<Search_Result_Item_TreeNode> returnValue = new List<Search_Result_Item_TreeNode>();
                foreach (List<Search_Result_Item_TreeNode> nodeList in childNodes.Values)
                {
                    returnValue.AddRange(nodeList);
                }
                return returnValue;
            }
        }

        #region IEquatable<Search_Result_Item_TreeNode> Members

        /// <summary> Determine if this tree node is equal to another </summary>
        /// <param name="other"> Sorted tree node root to compare to </param>
        /// <returns> TRUE if equal, otherwise FALSE </returns>
        /// <remarks> Two tree nodes are considered equal if they name and link are the same </remarks>
        bool IEquatable<Search_Result_Item_TreeNode>.Equals(Search_Result_Item_TreeNode other)
        {
            return (other.Name == Name) && (other.Link == Link);
        }

        #endregion

        /// <summary> Add a single child node under this node </summary>
        /// <param name="ChildName"> Display name for this new node (title of the item) </param>
        /// <param name="ChildLink"> Link for this new node of the tree </param>
        /// <param name="Sort_Value"> Value used to sort this item within its peers </param>
        /// <returns> Newly built sorted tree node object </returns>
        public Search_Result_Item_TreeNode Add_Child_Node(string ChildName, string ChildLink, int Sort_Value)
        {
            Search_Result_Item_TreeNode returnNode = new Search_Result_Item_TreeNode(ChildName, ChildLink);

            if (childNodes.ContainsKey(Sort_Value))
            {
                if (childNodes[Sort_Value].Contains(returnNode))
                    return childNodes[Sort_Value].Find(s => s.Equals(returnNode));
                childNodes[Sort_Value].Add(returnNode);
            }
            else
            {
                List<Search_Result_Item_TreeNode> listValue = new List<Search_Result_Item_TreeNode> {returnNode};
                childNodes[Sort_Value] = listValue;
            }

            return returnNode;
        }

        /// <summary> Determine if this tree node is equal to another </summary>
        /// <param name="other"> Sorted tree node root to compare to </param>
        /// <returns> TRUE if equal, otherwise FALSE </returns>
        /// <remarks> Two tree nodes are considered equal if they name and link are the same </remarks>
        public bool Equals(Search_Result_Item_TreeNode other)
        {
            return (other.Name == Name) && ( other.Link == Link );
        }
    }
}
