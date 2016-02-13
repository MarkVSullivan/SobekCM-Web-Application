#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.EAD
{
    /// <summary> Contains all of the information about a single component (or container) including
    /// the list of child components (or containers)  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("container")]
    public class EAD_Transfer_Container_Info
    {
        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Container_Info class </summary>
        public EAD_Transfer_Container_Info()
        {
            Did = new EAD_Transfer_Descriptive_Identification();
        }

        #endregion

        /// <summary> Flag indicates if this component has complex data, or any children have complex data </summary>
        /// <remarks> A compex component is defined as one that has scope, biogHist, or extend information included, or has a child which has this type of data</remarks>
        [XmlIgnore]
        public bool Is_Complex_Or_Has_Complex_Children
        {
            get
            {
                // If this has complex children, it is complex
                if (Has_Complex_Children)
                    return true;

                // Is this complex?
                if ((!String.IsNullOrEmpty(Scope_And_Content)) || (!String.IsNullOrEmpty(Biographical_History)) || ((Did != null) && (!String.IsNullOrEmpty(Did.Extent))))
                    return true;

                // Default return false
                return false;
            }
        }

        #region Public Properties

        /// <summary> Gets the level value associated with this container in the container list </summary>
        [DataMember(EmitDefaultValue = false, Name = "level")]
        [XmlAttribute("level")]
        [ProtoMember(1)]
        public string Level { get; set; }

        /// <summary> Flag indicates if any child containers are complex (i.e, have descriptive information, etc.. ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "complexChildren")]
        [XmlAttribute("complexChildren")]
        [ProtoMember(2)]
        public bool Has_Complex_Children { get; set; }

        /// <summary> Gets the biogrpahical history value associated with this container in the container list </summary>
        [DataMember(EmitDefaultValue = false, Name = "biogHist")]
        [XmlElement("biogHist")]
        [ProtoMember(3)]
        public string Biographical_History { get; set; }

        /// <summary> Gets the scope and content value associated with this container in the container list </summary>
        [DataMember(EmitDefaultValue = false, Name = "scope")]
        [XmlElement("scope")]
        [ProtoMember(4)]
        public string Scope_And_Content { get; set; }

        /// <summary> Basic descriptive information included in this container </summary>
        [DataMember(EmitDefaultValue = false, Name = "did")]
        [XmlElement("did")]
        [ProtoMember(5)]
        public EAD_Transfer_Descriptive_Identification Did { get; set; }

        /// <summary> Gets the collection of child components </summary>
        [DataMember(EmitDefaultValue = false, Name = "children")]
        [XmlArray("children")]
        [XmlArrayItem("container", typeof(EAD_Transfer_Container_Info))]
        [ProtoMember(6)]
        public List<EAD_Transfer_Container_Info> Children { get; set; }

        /// <summary> Gets the number of child component tags to this component </summary>
        [XmlIgnore]
        public int Children_Count
        {
            get { return Children == null ? 0 : Children.Count; }
        }
        
        #endregion

        #region Methods used (retained) for convenience, that actually reference DID properties

        /// <summary> Gets the number of container information objects included in the descriptive portion of this component </summary>
        [XmlIgnore]
        public int Container_Count
        {
            get { return Did.Container_Count; }
        }

        /// <summary> Gets the number of container information objects in the descriptive portion of this component </summary>
        [XmlIgnore]
        public List<EAD_Transfer_Parent_Container_Info> Containers
        {
            get { return Did.Containers; }
        }

        /// <summary> Gets the unit title value associated with this  </summary>
        [XmlIgnore]
        public string Unit_Title
        {
            get { return Did.Unit_Title; }
            set { Did.Unit_Title = value; }
        }

        /// <summary> Gets the unit date value associated with this  </summary>
        [XmlIgnore]
        public string Unit_Date
        {
            get { return Did.Unit_Date; }
            set { Did.Unit_Date = value; }
        }

        /// <summary> Gets the link to the digital object  </summary>
        [XmlIgnore]
        public string DAO_Link
        {
            get { return Did.DAO_Link; }
            set { Did.DAO_Link = value; }
        }

        /// <summary> Gets the title of the digital object  </summary>
        [XmlIgnore]
        public string DAO_Title
        {
            get { return Did.DAO_Title; }
            set { Did.DAO_Title = value; }
        }

        /// <summary> Gets the dao information of the digital object  </summary>
        [XmlIgnore]
        public string DAO
        {
            get { return Did.DAO; }
            set { Did.DAO = value; }
        }

        /// <summary> Gets the extent information </summary>
        [XmlIgnore]
        public string Extent
        {
            get { return Did.Extent; }
            set { Did.Extent = value; }
        }

        #endregion

        /// <summary> Recursively adds the child component's information to the StringBuilder, in HTML format </summary>
        /// <param name="Builder"> Builder of all the HTML-formatted componenet information for this component</param>
        public void recursively_add_container_information(StringBuilder Builder)
        {
            // Write the information for this tage
            Builder.AppendLine(Did.Unit_Title + "<br />");
            if (Children_Count > 0 )
            {
                Builder.AppendLine("<blockquote>");
                foreach (EAD_Transfer_Container_Info component in Children)
                {
                    component.recursively_add_container_information(Builder);
                }
                Builder.AppendLine("</blockquote>");
            }
        }
    }
}