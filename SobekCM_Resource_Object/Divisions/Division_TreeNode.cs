#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Resource_Object.Divisions
{
    /// <summary> Division node in a strucutral map tree associated with a digital resource</summary>
    /// <remarks> This class extends the <see cref="abstract_TreeNode"/> class. <br /> <br /> 
    /// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center. </remarks>
    [Serializable]
    [DataContract]
    public class Division_TreeNode : abstract_TreeNode
    {
        /// <summary> Stores the children for this node </summary>
        private readonly List<abstract_TreeNode> children;

        /// <summary> Constructor for a new instance of the Division_TreeNode class </summary>
        /// <param name="Type">Node type</param>
        /// <param name="Label">Node label</param>
        public Division_TreeNode(string Type, string Label) : base(Type, Label)
        {
            // Set the collection of children
            children = new List<abstract_TreeNode>();
        }

        /// <summary> Constructor for an empty instance of the Division_TreeNode class </summary>
        public Division_TreeNode() 
        {
            // Set the collection of children
            children = new List<abstract_TreeNode>();
        }

        /// <summary> Gets the flag indicating if this is a page node or not </summary>
        /// <value>Always returns 'FALSE'</value>
        [DataMember]
        public override bool Page
        {
            get { return false; }
        }

        /// <summary> Gets the collection of child nodes </summary>
        /// <remarks> This returns a generic list which is a collection of <see cref="abstract_TreeNode"/> objects. </remarks>
        [DataMember]
        public List<abstract_TreeNode> Nodes
        {
            get { return children; }
        }

        /// <summary> Add a new child node under this tree node </summary>
        /// <param name="ChildNode"> New node to add </param>
        public void Add_Child(abstract_TreeNode ChildNode)
        {
            children.Add(ChildNode);
        }

        /// <summary> Clears all of the children under this node </summary>
        public void Clear()
        {
            children.Clear();
        }

        /// <summary> Display label for this division, which is either the label, or the type if there is no label </summary>
        [DataMember]
        public string Display_Label
        {
            get
            {
                if (label.Length > 0)
                {
                    return label;
                }
                return type;
            }
        }

        /// <summary> Shorter version of the label for this division, which is either the short label or the type </summary>
        public string Display_Short_Label
        {
            get
            {
                if (label.Length > 0)
                {
                    return shorten(label);
                }
                return type;
            }
        }

        private string shorten(string LongLabel)
        {
            const int SHORT_LENGTH = 30;
            if (LongLabel.Length > SHORT_LENGTH)
            {
                // See if there is a space somewhere convenient
                int spaceLocation = LongLabel.IndexOf(" ", SHORT_LENGTH, StringComparison.Ordinal);
                if (spaceLocation >= 0)
                {
                    return LongLabel.Substring(0, spaceLocation) + "...";
                }
                
                spaceLocation = LongLabel.IndexOf(" ", SHORT_LENGTH - 5, StringComparison.Ordinal);
                if ((spaceLocation >= 0) && (spaceLocation <= SHORT_LENGTH + 5))
                {
                    return LongLabel.Substring(0, spaceLocation) + "...";
                }
                
                return LongLabel.Substring(0, SHORT_LENGTH) + "...";
            }
            
            return LongLabel;
        }
    }
}