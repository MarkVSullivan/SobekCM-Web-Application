using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Citation
{
    /// <summary> Information about a special citation section writer which should be 
    /// loaded to dispay a portion in the main citation </summary>
    /// <remarks> These classes should all implement the iCitationSectionWriter class
    /// to be used with the default SobekCM web application </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("CitationSectionWriter")]
    public class SectionWriter
    {
        /// <summary> Fully qualified name (including namespace) of the special citation 
        /// section writer class </summary>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(1)]
        public string Class_Name { get; set; }

        /// <summary> Name of the assembly from which this special citation section
        /// viewer should be loaded </summary>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(2)]
        public string Assembly { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="SectionWriter"/> class </summary>
        /// <remarks> This creates an empty, uninitialized instance of the <see cref="SectionWriter"/> class,
        /// but is required for seralization/deserialization purposes </remarks>
        public SectionWriter()
        {
            // Does nothing - primarily for serialization and deserialization
        }

        /// <summary> Constructor for a new instance of the <see cref="SectionWriter"/> class </summary>
        /// <param name="Assembly"> Name of the assembly from which this special citation section
        /// viewer should be loaded </param>
        /// <param name="Class_Name"> Fully qualified name (including namespace) of the special citation 
        /// section writer class </param>
        public SectionWriter( string Assembly, string Class_Name )
        {
            this.Assembly = Assembly;
            this.Class_Name = Class_Name;
        }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Assembly property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeAssembly()
        {
            return (!String.IsNullOrEmpty(Assembly));
        }

        #endregion

    }
}
