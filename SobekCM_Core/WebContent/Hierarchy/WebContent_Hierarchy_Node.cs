using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent.Hierarchy
{
    /// <summary> A single node within the web content page (and redirects) hierarchy </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webHierarchyNode")]
    public class WebContent_Hierarchy_Node
    {
        /// <summary> Primary key for this web content page, from the database </summary>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int WebContentID { get; set; }

        /// <summary> Segment name for this element with the web content hierarchy </summary>
        [DataMember(EmitDefaultValue = false, Name = "segment")]
        [XmlAttribute("segment")]
        [ProtoMember(2)]
        public string Segment { get; set; }

        /// <summary> URL to which a request for this page should be redirected </summary>
        [DataMember(EmitDefaultValue = false, Name = "redirect")]
        [XmlAttribute("redirect")]
        [ProtoMember(3)]
        public string Redirect { get; set; }


        /// <summary> Collection of child nodes, indexed by segment </summary>
        [DataMember(EmitDefaultValue = false, Name = "children")]
        [XmlIgnore]
        [ProtoMember(4)]
        public Dictionary<string, WebContent_Hierarchy_Node> Children { get; set; }

        /// <summary> Collection of child nodes, converted to a list for XML serialization </summary>
        [IgnoreDataMember]
        [XmlArray("children")]
        [XmlArrayItem("webHierarchyNode", typeof(WebContent_Hierarchy_Node))]
        public List<WebContent_Hierarchy_Node> Children_XML
        {
            get { return ((Children != null) && (Children.Count > 0)) ? Children.Values.ToList() : null; }
            set
            {
                if ((value != null) && ( value.Count > 0 ))
                {
                    // Ensure the dictionary is not null
                    if (Children == null)
                        Children = new Dictionary<string, WebContent_Hierarchy_Node>(StringComparer.OrdinalIgnoreCase);

                    // Add each child 
                    foreach (WebContent_Hierarchy_Node thisChild in value)
                    {
                        Children[thisChild.Segment] = new WebContent_Hierarchy_Node(thisChild.Segment, thisChild.WebContentID, thisChild.Redirect);
                    }
                }
            }
        }

        /// <summary> Constructor for a new instance of the WebContent_Hiearchy_Node class </summary>
        public WebContent_Hierarchy_Node()
        {
            // Empty constructor for serialization purposes
            WebContentID = -1;
        }

        /// <summary> Constructor for a new instance of the WebContent_Hiearchy_Node class </summary>
        /// <param name="Segment"> Segment name for this element with the web content hierarchy </param>
        /// <param name="WebContentID"> Primary key for this web content page, from the database </param>
        /// <param name="Redirect"> URL to which a request for this page should be redirected </param>
        public WebContent_Hierarchy_Node(string Segment, int WebContentID, string Redirect )
        {
            this.Segment = Segment;
            this.WebContentID = WebContentID;
            this.Redirect = Redirect;
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
            Children[NewSegment.ToLower()] = returnValue;

            // Return the built child node 
            return returnValue;
        }

        /// <summary> Add a new, fully built child node to this node </summary>
        /// <param name="NewChild"> New child node to add </param>
        public void Add_Child(WebContent_Hierarchy_Node NewChild)
        {
            // Ensure the collection has been built
            if (Children == null)
                Children = new Dictionary<string, WebContent_Hierarchy_Node>(StringComparer.OrdinalIgnoreCase);

            // Add the child node
            Children[NewChild.Segment.ToLower()] = NewChild;
        }

        #region Methods to control XML serialization

        /// <summary> Method suppresses XML Serialization of the WebContentID property if it is -1 </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeWebContentID()
        {
            return WebContentID > 0;
        }

        #endregion
    }
}
