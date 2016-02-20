using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Class represents titles which are related to the current title in the
    /// SobekCM database, and utilized as the transfer object for the SobekCM engine </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("relatedTitle")]
    public class BriefItem_Related_Titles
    {
        /// <summary> Relationship between the main title and the related title </summary>
        [DataMember(EmitDefaultValue = false, Name = "relationship")]
        [XmlAttribute("relationship")]
        [ProtoMember(1)]
        public string Relationship { get; set; }

        /// <summary> Title of the related title within this SobekCM library </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(2)]
        public string Title { get; set; }

        /// <summary> Link for the related title within this SobekCM library </summary>
        [DataMember(EmitDefaultValue = false, Name = "uri")]
        [XmlAttribute("uri")]
        [ProtoMember(3)]
        public string Link { get; set; }

        /// <summary> Constructor for a new instance of the BriefItem_Related_Titles class </summary>
        /// <param name="Relationship"> Relationship between the main title and the related title</param>
        /// <param name="Title"> Title of the related title within this SobekCM library</param>
        /// <param name="Link"> Link for the related title within this SobekCM library</param>
        public BriefItem_Related_Titles(string Relationship, string Title, string Link)
        {
            this.Relationship = Relationship;
            this.Title = Title;
            this.Link = Link;
        }

        /// <summary> Constructor for a new instance of the BriefItem_Related_Titles class </summary>
        /// <remarks> The object created using this constructor is completely undefined </remarks>
        public BriefItem_Related_Titles()
        {
            // Do nothing - for serialization purposes
        }
    }
}
