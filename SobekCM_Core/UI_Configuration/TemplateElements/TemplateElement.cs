using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.TemplateElements
{
    /// <summary> Class represents a mapping from type/subtype within a template 
    /// configuration file and an assembly/class to use </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("TemplateElement")]
    public class TemplateElement
    {
        /// <summary> Gets the 'type' value used in the template configuration files
        ///  to select this metadata template element </summary>
        /// <remarks> The type and subtype uniquely identify this template element </remarks>
        [DataMember(Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(1)]
        public string Type { get; set; }

        /// <summary> Gets the 'subtype' value used in the template configuration files
        ///  to select this metadata template element </summary>
        /// <remarks> The type and subtype uniquely identify this template element </remarks>
        [DataMember(Name = "subtype", EmitDefaultValue = false)]
        [XmlAttribute("subtype")]
        [ProtoMember(2)]
        public string Subtype { get; set; }

        /// <summary> Fully qualified (including namespace) name of the class used 
        /// for this template element </summary>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(3)]
        public string Class { get; set; }

        /// <summary> Name of the assembly within which this class resides, unless this
        /// is one of the default elements included in the core code </summary>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(4)]
        public string Assembly { get; set; }

        /// <summary> Image for this template, if one exists </summary>
        [DataMember(Name = "image", EmitDefaultValue = false)]
        [XmlAttribute("image")]
        [ProtoMember(5)]
        public string Image { get; set; }

        /// <summary> Url for any relevant help page online, which details any TECHNICAL 
        /// aspects about this template element generally  </summary> 
        [DataMember(Name = "adminUri", EmitDefaultValue = false)]
        [XmlAttribute("adminUri")]
        [ProtoMember(6)]
        public string AdminUri { get; set; }
         

        /// <summary> Url for any relevant help page online which were created for 
        /// the end users who request help  </summary> 
        /// <remarks> This can be overriden within the individual template configuration </remarks>
        [DataMember(Name = "helpUri", EmitDefaultValue = false)]
        [XmlAttribute("helpUri")]
        [ProtoMember(7)]
        public string HelpUri { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="TemplateElement"/> class </summary>
        public TemplateElement()
        {
            // Do nothing - used for serialization purposes
        }


        /// <summary> Constructor for a new instance of the <see cref="TemplateElement"/> class </summary>
        /// <param name="Type"> The 'type' value used in the template configuration files
        ///  to select this metadata template element </param>
        /// <param name="Subtype"> The 'subtype' value used in the template configuration files
        ///  to select this metadata template element </param>
        /// <param name="Class"> Fully qualified (including namespace) name of the class used 
        /// for this template element </param>
        public TemplateElement(string Type, string Subtype, string Class)
        {
            this.Type = Type;
            this.Subtype = Subtype;
            this.Class = Class;
        }

        /// <summary> Constructor for a new instance of the <see cref="TemplateElement"/> class </summary>
        /// <param name="Type"> The 'type' value used in the template configuration files
        ///  to select this metadata template element </param>
        /// <param name="Subtype"> The 'subtype' value used in the template configuration files
        ///  to select this metadata template element </param>
        /// <param name="Class"> Fully qualified (including namespace) name of the class used 
        /// for this template element </param>
        /// <param name="Assembly"> Name of the assembly within which this class resides, unless this
        /// is one of the default elements included in the core code </param>
        public TemplateElement(string Type, string Subtype, string Class, string Assembly)
        {
            this.Type = Type;
            this.Subtype = Subtype;
            this.Class = Class;
            this.Assembly = Assembly;
        }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Subtype property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeSubtype()
        {
            return (!String.IsNullOrEmpty(Subtype));
        }

        /// <summary> Method suppresses XML Serialization of the Assembly property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeAssembly()
        {
            return (!String.IsNullOrEmpty(Assembly));
        }

        #endregion

    }
}
