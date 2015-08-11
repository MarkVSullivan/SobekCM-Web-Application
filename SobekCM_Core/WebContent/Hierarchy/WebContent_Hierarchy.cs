using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent.Hierarchy
{
    /// <summary> Web content page (and redirects) hierarchy object </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webHierarchy")]
    public class WebContent_Hierarchy
    {
        /// <summary> Collection of child nodes, indexed by segment </summary>
        [DataMember(EmitDefaultValue = false, Name = "children")]
        [XmlIgnore]
        [ProtoMember(1)]
        public Dictionary<string, WebContent_Hierarchy_Node> Children { get; set; }

        /// <summary> Collection of child nodes, converted to a list for XML serialization </summary>
        [IgnoreDataMember]
        [XmlArray("children")]
        [XmlArrayItem("webHierarchyNode", typeof(WebContent_Hierarchy_Node))]
        public List<WebContent_Hierarchy_Node> Children_XML
        {
            get { return Children.Values.ToList(); }
            set
            {
                if ((value != null) && (value.Count > 0))
                {
                    // Add each child 
                    foreach (WebContent_Hierarchy_Node thisChild in value)
                    {
                        Children[thisChild.Segment] = new WebContent_Hierarchy_Node(thisChild.Segment, thisChild.WebContentID, thisChild.Redirect);
                    }
                }
            }
        }

        /// <summary> Number of child root nodes in this hierarchy </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public int Root_Count
        {
            get { return Children != null ? Children.Count : 0; }
        }

        /// <summary> Constructor for a new instance of the WebContent_Hierarchy class </summary>
        public WebContent_Hierarchy()
        {
            Children = new Dictionary<string, WebContent_Hierarchy_Node>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary> Add a new child node to this node </summary>
        /// <param name="NewSegment"> Segment name for this element with the web content hierarchy </param>
        /// <param name="NewWebContentID"> [Optional] Primary key for this web content page, from the database </param>
        /// <returns> Built and added new child node </returns>
        public WebContent_Hierarchy_Node Add_Child(string NewSegment, int? NewWebContentID = null)
        {
            // Ensure the collection has been built
            if (Children == null)
                Children = new Dictionary<string, WebContent_Hierarchy_Node>(StringComparer.OrdinalIgnoreCase);

            // Build the child node
            WebContent_Hierarchy_Node returnValue = new WebContent_Hierarchy_Node { Segment = NewSegment };
            if (NewWebContentID.HasValue) returnValue.WebContentID = NewWebContentID.Value;
            Children[NewSegment] = returnValue;

            // Return the built child node 
            return returnValue;
        }

        /// <summary> Add a new child node to this node </summary>
        /// <param name="NewNode"> Fully built new root child node </param>
        public void Add_Child(WebContent_Hierarchy_Node NewNode)
        {
            // Ensure the collection has been built
            if (Children == null)
                Children = new Dictionary<string, WebContent_Hierarchy_Node>(StringComparer.OrdinalIgnoreCase);

            // Build the child node
            Children[NewNode.Segment] = NewNode;
        }

        /// <summary> Clear this hierarchy object, by clearing the collection of root nodes </summary>
        public void Clear()
        {
            if ( Children != null )
                Children.Clear();
        }

        /// <summary> Look within this hierarchy for a match for a collection of incoming URL segments  </summary>
        /// <param name="UrlSegments"> Collection of URL segments to match within this deep hierarchy </param>
        /// <returns> Matched node, or NULL if there is no match </returns>
        public WebContent_Hierarchy_Node Find(List<string> UrlSegments)
        {
            // If no children or no matching root nodes, return NULL
            if ((Children == null) || (!Children.ContainsKey(UrlSegments[0])))
                return null;

            // Get the matching root node
            WebContent_Hierarchy_Node currentNode = Children[UrlSegments[0]];

            // If no match (or match was NULL actually) return null
            // This should not ever happen though
            if (currentNode == null)
                return null;

            // Now, continue to drive down through the hierarchy nodes
            int i = 1;
            while (i < UrlSegments.Count)
            {
                // Is there a match at this level?  If not, return NULL
                if ((currentNode.Children == null) || (!currentNode.Children.ContainsKey(UrlSegments[i])))
                    return null;

                // Get the next level matching node
                currentNode = currentNode.Children[UrlSegments[i]];

                // If no match (or match was NULL actually) return null
                // This should not ever happen though
                if (currentNode == null)
                    return null;
                i++;
            }

            return currentNode;
        }
    }
}
