#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.EAD
{
    /// <summary> Description of subordinate Containers (or container list) for an encoded archival description (EAD) </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("eadContainers")]
    public class EAD_Transfer_Desc_Sub_Components
    {
        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Description_of_Subordinate_Components class </summary>
        public EAD_Transfer_Desc_Sub_Components()
        {
            Containers = new List<EAD_Transfer_Container_Info>();
            Did = new EAD_Transfer_Descriptive_Identification();
        }

        #endregion

        #region Public Properties

        /// <summary> Gets the type value</summary>
        [DataMember(Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(1)]
        public string Type { get; set; }

        /// <summary> Gets the list of container objects </summary>
        [DataMember(Name = "roots")]
        [XmlArray("roots")]
        [XmlArrayItem("containers", typeof(EAD_Transfer_Container_Info))]
        [ProtoMember(2)]
        public List<EAD_Transfer_Container_Info> Containers { get; set; }

        /// <summary> Gets the did information object </summary>
        [DataMember(Name = "did")]
        [XmlElement("did")]
        [ProtoMember(3)]
        public EAD_Transfer_Descriptive_Identification Did { get; set; }

        /// <summary> Holds basic head information about the containers, from the EAD </summary>
        [DataMember(Name = "head")]
        [XmlElement("head")]
        [ProtoMember(4)]
        public string Head { get; set; }

        #endregion

        /// <summary> Clear the contents of child componets and the descriptive identification </summary>
        public void Clear()
        {
            Did.Clear();
            Containers.Clear();
        }

        /// <summary> Returns the recursively built container information for this class as a string for debug purposes </summary>
        /// <returns> Child container information, returned as a string for debug purposes </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (EAD_Transfer_Container_Info component in Containers)
            {
                component.recursively_add_container_information(builder);
            }

            return builder.ToString();
        }
    }
}