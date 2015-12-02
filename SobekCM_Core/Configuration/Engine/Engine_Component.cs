using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Engine
{
    /// <summary> Code component which provides methods to fulfill microservice endpoint requests </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("EngineComponent")]
    public class Engine_Component
    {
        /// <summary> Identifier for this component, which is referenced within the configuration file to specify this component </summary>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public string ID { get; set; }

        /// <summary> Assembly to load for this class, if this is an external assembly </summary>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(2)]
        public string Assembly { get; set; }

        /// <summary> Complete namespace and name of the class which fulfills a microservice endpoint requests </summary>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(3)]
        public string Class { get; set; }

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