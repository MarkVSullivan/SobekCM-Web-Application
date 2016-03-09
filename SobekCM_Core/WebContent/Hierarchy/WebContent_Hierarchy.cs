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
    public class WebContent_Hierarchy : iSerializationEvents
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
        public List<WebContent_Hierarchy_Node> Children_XML { get; set;}

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

        /// <summary> Add a single new node to the web hierarchy </summary>
        /// <param name="WebContentID"> Primary key to the web content page to add </param>
        /// <param name="Redirect"> Redirect URL associated with the final web content page </param>
        /// <param name="Level1"> First level URL segment for the web content page to add </param>
        /// <param name="Level2"> Second level URL segment for the web content page to add </param>
        /// <param name="Level3"> Third level URL segment for the web content page to add </param>
        /// <param name="Level4"> Fourth level URL segment for the web content page to add </param>
        /// <param name="Level5"> Fifth level URL segment for the web content page to add </param>
        /// <param name="Level6"> Seventh level URL segment for the web content page to add </param>
        /// <param name="Level7"> Seventh level URL segment for the web content page to add </param>
        /// <param name="Level8"> Eighth level URL segment for the web content page to add </param>
        public void Add_Single_Node(int WebContentID, string Redirect, string Level1, string Level2, string Level3, string Level4, string Level5, string Level6, string Level7, string Level8)
        {
            if (String.IsNullOrEmpty(Level1))
                return;

            WebContent_Hierarchy_Node node;

            // Handle the FIRST level of the hierarchy
            if (Children.ContainsKey(Level1))
            {
                node = Children[Level1];
            }
            else
            {
                node = new WebContent_Hierarchy_Node {Segment = Level1};
                Children[Level1] = node;
            }

            // If no second level, assign the values and return
            if (( node == null ) || (String.IsNullOrEmpty(Level2)))
            {
                if (node != null)
                {
                    node.WebContentID = WebContentID;
                    node.Redirect = Redirect;
                }
                return;
            }

            // Handle the SECOND level of the hierarchy
            if (( node.Children != null ) && ( node.Children.ContainsKey(Level2)))
            {
                node = node.Children[Level2];
            }
            else
            {
                node = node.Add_Child(Level2);
            }

            // If no third level, assign the values and return
            if ((node == null) || (String.IsNullOrEmpty(Level3)))
            {
                if (node != null)
                {
                    node.WebContentID = WebContentID;
                    node.Redirect = Redirect;
                }
                return;
            }

            // Handle the THIRD level of the hierarchy
            if ((node.Children != null) && (node.Children.ContainsKey(Level3)))
            {
                node = node.Children[Level3];
            }
            else
            {
                node = node.Add_Child(Level3);
            }

            // If no fourth level, assign the values and return
            if ((node == null) || (String.IsNullOrEmpty(Level4)))
            {
                if (node != null)
                {
                    node.WebContentID = WebContentID;
                    node.Redirect = Redirect;
                }
                return;
            }

            // Handle the FOURTH level of the hierarchy
            if ((node.Children != null) && (node.Children.ContainsKey(Level4)))
            {
                node = node.Children[Level4];
            }
            else
            {
                node = node.Add_Child(Level4);
            }

            // If no fifth level, assign the values and return
            if ((node == null) || (String.IsNullOrEmpty(Level5)))
            {
                if (node != null)
                {
                    node.WebContentID = WebContentID;
                    node.Redirect = Redirect;
                }
                return;
            }

            // Handle the FIFTH level of the hierarchy
            if ((node.Children != null) && (node.Children.ContainsKey(Level5)))
            {
                node = node.Children[Level5];
            }
            else
            {
                node = node.Add_Child(Level5);
            }

            // If no sixth level, assign the values and return
            if ((node == null) || (String.IsNullOrEmpty(Level6)))
            {
                if (node != null)
                {
                    node.WebContentID = WebContentID;
                    node.Redirect = Redirect;
                }
                return;
            }

            // Handle the SIXTH level of the hierarchy
            if ((node.Children != null) && (node.Children.ContainsKey(Level6)))
            {
                node = node.Children[Level6];
            }
            else
            {
                node = node.Add_Child(Level6);
            }

            // If no seventh level, assign the values and return
            if ((node == null) || (String.IsNullOrEmpty(Level7)))
            {
                if (node != null)
                {
                    node.WebContentID = WebContentID;
                    node.Redirect = Redirect;
                }
                return;
            }

            // Handle the SEVENTH level of the hierarchy
            if ((node.Children != null) && (node.Children.ContainsKey(Level7)))
            {
                node = node.Children[Level7];
            }
            else
            {
                node = node.Add_Child(Level7);
            }

            // If no eighth level, assign the values and return
            if ((node == null) || (String.IsNullOrEmpty(Level8)))
            {
                if (node != null)
                {
                    node.WebContentID = WebContentID;
                    node.Redirect = Redirect;
                }
                return;
            }

            // Handle the EIGHTH level of the hierarchy
            if ((node.Children != null) && (node.Children.ContainsKey(Level8)))
            {
                node = node.Children[Level8];
            }
            else
            {
                node = node.Add_Child(Level8);
            }
            if (node != null)
            {
                node.WebContentID = WebContentID;
                node.Redirect = Redirect;
            }

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

        /// <summary> Method is called by the serializer after this item is unserialized </summary>
        /// <region> This method needs to be called if this is unserialized from XML</region>
        public void PostUnSerialization()
        {
            // If this came over from XML, the children need to be transferred to the Children collection
            if (((Children == null) || (Children.Count == 0)) && ((Children_XML != null) && (Children_XML.Count > 0)))
            {
                // Make sure the children collection exists
                if (Children == null)
                {
                    Children = new Dictionary<string, WebContent_Hierarchy_Node>(StringComparer.OrdinalIgnoreCase);
                }

                // Copy over all the child nodes
                foreach (WebContent_Hierarchy_Node childNode in Children_XML)
                {
                    Children[childNode.Segment] = childNode;
                }

                // Clear the Children_XML
                Children_XML.Clear();
            }
        }
    }
}
