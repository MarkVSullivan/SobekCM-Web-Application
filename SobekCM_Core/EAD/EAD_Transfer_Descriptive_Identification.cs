#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.EAD
{
    /// <summary> Contains all the information for a single descriptive identification block within an EAD's container list </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("eadDid")]
    public class EAD_Transfer_Descriptive_Identification
    {

        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Descriptive_Identification class </summary>
        public EAD_Transfer_Descriptive_Identification()
        {
            // Do nothing
        }

        #endregion

        /// <summary> Adds a new container information object to this descriptive information </summary>
        /// <param name="Container_Type"> General type of this container ( usually 'box', 'folder', etc.. )</param>
        /// <param name="Container_Title"> Title or label for this container</param>
        public void Add_Container(string Container_Type, string Container_Title)
        {
            if (Containers == null)
                Containers = new List<EAD_Transfer_Parent_Container_Info>();

            Containers.Add(new EAD_Transfer_Parent_Container_Info(Container_Type, Container_Title));
        }

        /// <summary> Clears all the information in this descriptive identification object </summary>
        public void Clear()
        {
            Unit_Title = null;
            Unit_Date = null;
            DAO_Link = null;
            DAO_Title = null;
            DAO = null;
            Extent = null;
            Containers.Clear();
        }

        #region Public Properties

        /// <summary> Gets the number of container information objects associated with this descriptive identification information </summary>
        [XmlIgnore]
        public int Container_Count
        {
            get { return Containers == null ? 0 : Containers.Count; }
        }

        /// <summary> Gets the collection of container information objects associated with this descriptive 
        /// identifiation object </summary>
        [DataMember(EmitDefaultValue = false, Name = "parentInfo")]
        [XmlArray("parentInfo")]
        [XmlArrayItem("parent", typeof(EAD_Transfer_Parent_Container_Info))]
        [ProtoMember(1)]
        public List<EAD_Transfer_Parent_Container_Info> Containers { get; set; }

        /// <summary> Gets the unit title value associated with this  </summary>
        [DataMember(EmitDefaultValue = false, Name = "unitTitle")]
        [XmlAttribute("unitTitle")]
        [ProtoMember(2)]
        public string Unit_Title { get; set; }

        /// <summary> Gets the unit date value associated with this  </summary>
        [DataMember(EmitDefaultValue = false, Name = "unitDate")]
        [XmlAttribute("unitDate")]
        [ProtoMember(3)]
        public string Unit_Date { get; set; }

        /// <summary> Gets the link to the digital object  </summary>
        [DataMember(EmitDefaultValue = false, Name = "daoHref")]
        [XmlAttribute("daoHref")]
        [ProtoMember(4)]
        public string DAO_Link { get; set; }

        /// <summary> Gets the title of the digital object  </summary>
        [DataMember(EmitDefaultValue = false, Name = "daoTitle")]
        [XmlAttribute("daoTitle")]
        [ProtoMember(5)]
        public string DAO_Title { get; set; }

        /// <summary> Gets the dao information of the digital object  </summary>
        [DataMember(EmitDefaultValue = false, Name = "dao")]
        [XmlAttribute("dao")]
        [ProtoMember(6)]
        public string DAO { get; set; }

        /// <summary> Gets the extent information </summary>
        [DataMember(EmitDefaultValue = false, Name = "extent")]
        [XmlAttribute("extent")]
        [ProtoMember(7)]
        public string Extent { get; set; }

        #endregion
    }
}