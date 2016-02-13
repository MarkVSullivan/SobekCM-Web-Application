#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.EAD
{
    /// <summary> Contains all the information about an Encoded Archival Description (EAD) object, including the container hierarchy </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("eadInfo")]
    public class EAD_Transfer_Object
    {
        /// <summary> Constructor for a new instance of the EAD_Transfer_Object class </summary>
        public EAD_Transfer_Object()
        {
            Container_Hierarchy = new EAD_Transfer_Desc_Sub_Components();
            TOC_Included_Sections = new List<EAD_Transfer_TOC_Included_Section>();
        }

        /// <summary> Gets any container hierarchy for this Encoded Archival Description </summary>
        [DataMember(Name = "containers")]
        [XmlElement("containers")]
        [ProtoMember(1)]
        public EAD_Transfer_Desc_Sub_Components Container_Hierarchy { get; set; }

        /// <summary> Gets the list of included sections in this EAD-type object to be included in 
        /// the table of contents </summary>
        [DataMember(Name = "toc")]
        [XmlArray("toc")]
        [XmlArrayItem("section", typeof(EAD_Transfer_TOC_Included_Section))]
        [ProtoMember(2)]
        public List<EAD_Transfer_TOC_Included_Section> TOC_Included_Sections { get; set; }

        /// <summary> Gets and sets the Archival description chunk of HTML or XML for this EAD-type object </summary>
        /// <summary> Gets any container hierarchy for this Encoded Archival Description </summary>
        [DataMember(Name = "description")]
        [XmlElement("description")]
        [ProtoMember(3)]
        public string Full_Description { get; set; }

        /// <summary> Flag indicates if this EAD metadata module has data </summary>
        [XmlIgnore]
        public bool hasData
        {
            get { return ((Container_Hierarchy.Containers.Count > 0) || (!String.IsNullOrEmpty(Full_Description))); }
        }

        /// <summary> Add a TOC section for this EAD Finding Guide for display within a
        /// SobekCM digital library </summary>
        /// <param name="Internal_Link_Name"> Name of the internal link with the EAD Finding Guide which is used to allow a
        /// user to move to a section within the complete EAD Finding Guide HTML </param>
        /// <param name="Section_Title"> Title of this EAD Finding Guide section to be displayed in the table of contents </param>
        public void Add_TOC_Included_Section(string Internal_Link_Name, string Section_Title)
        {
            TOC_Included_Sections.Add(new EAD_Transfer_TOC_Included_Section(Internal_Link_Name, Section_Title));
        }
    }
}