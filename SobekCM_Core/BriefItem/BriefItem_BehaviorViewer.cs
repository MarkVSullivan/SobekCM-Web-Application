using System;
using System.Runtime.Serialization;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Viewer tied to this item </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("behaviors")]
    public class BriefItem_BehaviorViewer
    {
        /// <summary> Constructor for a new instance of the BriefItem_BehaviorViewer class </summary>
        public BriefItem_BehaviorViewer()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the BriefItem_BehaviorViewer class </summary>
        /// <param name="ViewerType"> Name of this viewer type, from the database </param>
        /// <param name="MenuOrder"> Order this displays in the item main menu </param>
        /// <param name="Excluded"> Flag indicates if this viewer is explicitly excluded from this digital resource </param>
        /// <param name="Label"> Label associated with this viewer for display in the item menu for the digital resource </param>
        public BriefItem_BehaviorViewer(string ViewerType, float MenuOrder, bool Excluded, string Label )
        {
            this.ViewerType = ViewerType;
            this.MenuOrder = MenuOrder;
            this.Excluded = Excluded;
            this.Label = Label;
        }

        /// <summary> Name of this viewer type, from the database </summary>
        [DataMember(Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(1)]
        public string ViewerType { get; set; }

        /// <summary> Order this displays in the item main menu  </summary>
        [DataMember(Name = "order")]
        [XmlAttribute("order")]
        [ProtoMember(2)]
        public float MenuOrder { get; set; }

        /// <summary> Flag indicates if this viewer is explicitly excluded from this digital resource </summary>
        [DataMember(Name = "excluded")]
        [XmlAttribute("excluded")]
        [ProtoMember(3)]
        public bool Excluded { get; set; }

        /// <summary> Attributes related to this viewer for the digital resource </summary>
        [DataMember(Name = "attributes", EmitDefaultValue = false)]
        [XmlAttribute("attributes")]
        [ProtoMember(4)]
        public string Attributes { get; set; }

        /// <summary> Label associated with this viewer for display in the item menu for the digital resource </summary>
        [DataMember(Name = "label", EmitDefaultValue = false)]
        [XmlAttribute("label")]
        [ProtoMember(5)]
        public string Label { get; set; }
    }
}
