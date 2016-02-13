#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.EAD
{
    /// <summary> Class contains all the information for rendering a table of contents for EAD Finding Guides
    /// which allow this to be rendered correctly within a SobekCM digital library </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("section")]
    public class EAD_Transfer_TOC_Included_Section
    {
        /// <summary> Name of the internal link with the EAD Finding Guide which is used to allow a
        /// user to move to a section within the complete EAD Finding Guide HTML </summary>
        [DataMember(EmitDefaultValue = false, Name = "link")]
        [XmlAttribute("link")]
        [ProtoMember(1)]
        public string Internal_Link_Name { get; set; }

        /// <summary> Title of this EAD Finding Guide section to be displayed in the table of contents </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(2)]
        public string Section_Title { get; set; }

        /// <summary> Constructor for a new instance of the EAD_TOC_Included_Section class </summary>
        /// <param name="Internal_Link_Name"> Name of the internal link with the EAD Finding Guide which is used to allow a
        /// user to move to a section within the complete EAD Finding Guide HTML </param>
        /// <param name="Section_Title"> Title of this EAD Finding Guide section to be displayed in the table of contents </param>
        public EAD_Transfer_TOC_Included_Section(string Internal_Link_Name, string Section_Title)
        {
            this.Internal_Link_Name = Internal_Link_Name;
            this.Section_Title = Section_Title;
        }

        /// <summary> Constructor for a new instance of the EAD_TOC_Included_Section class </summary>
        /// <remarks> Empty constructor for serialization and deserialization purposes </remarks>
        public EAD_Transfer_TOC_Included_Section()
        {
            // Empty constructor for serialization and deserialization purposes
        }
    }
}