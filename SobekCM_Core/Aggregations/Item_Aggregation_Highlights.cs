#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Aggregations
{
    
    /// <summary> Single image highlight from an aggregation, with the information for only a single language </summary>
    /// <remarks> This is related to the <see cref="Complete_Item_Aggregation_Highlights" /> class, except this does not have dictionaries
    /// for some of the values, since it is for a single language </remarks>
    [Serializable, DataContract, ProtoContract]
    public class Item_Aggregation_Highlights
    {
        /// <summary> Constructor for a new instance of the Item_Aggregation_Highlights class </summary>
        public Item_Aggregation_Highlights() { }

        /// <summary> Constructor for a new instance of the Item_Aggregation_Highlights class </summary>
        /// <param name="Image"> Primary image to display as the highlight </param>
        /// <param name="Link"> Primary link that the user goes to when they click on this image </param>
        /// <param name="Text"> Text to display under the highlight image </param>
        /// <param name="Tooltip"> Tooltip to display when you hover over the image or text </param>
        public Item_Aggregation_Highlights(string Image, string Link, string Text, string Tooltip)
        {
            this.Image = Image;
            this.Link = Link;
            this.Text = Text;
            this.Tooltip = Tooltip;
        }

        /// <summary> Primary image to display as the highlight </summary>
        [DataMember(Name = "image")]
        [XmlText]
        [ProtoMember(1)]
        public string Image { get; set; }

        /// <summary> Primary link that the user goes to when they click on this image </summary>
        [DataMember(Name = "link")]
        [XmlAttribute("link")]
        [ProtoMember(2)]
        public string Link { get; set; }

        /// <summary> Text to display under the highlight image </summary>
        [DataMember(Name = "text", EmitDefaultValue = false)]
        [XmlAttribute("text")]
        [ProtoMember(3)]
        public string Text { get; set; }

        /// <summary> Tooltip to display when you hover over the image or text </summary>
        [DataMember(Name = "tooltip", EmitDefaultValue = false)]
        [XmlAttribute("tooltip")]
        [ProtoMember(4)]
        public string Tooltip { get; set; }


    }
}
