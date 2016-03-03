using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Settings;

namespace SobekCM.Core.Builder
{
    /// <summary> Set of builder module configuration information </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("builderModuleSet")]
    public class Builder_Module_Set_Info
    {
        /// <summary> Builder module settings (in order of execution) for this builder module set </summary>
        [DataMember(Name = "modules")]
        [XmlArray("modules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(1)]
        public List<Builder_Module_Setting> Builder_Modules { get; set; }

        /// <summary> Primary key for this set of builder modules </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [XmlAttribute("id")]
        [ProtoMember(2)]
        public int SetID { get; set; }

        /// <summary> Name for this set of builder modules </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(3)]
        public string SetName { get; set; }

        /// <summary> Number of times this builder module set is referenced </summary>
        [DataMember(Name = "usedCount", EmitDefaultValue = false)]
        [XmlIgnore]
        [ProtoMember(4)]
        public int? Used_Count;

        /// <summary> Number of times this builder module set is referenced, for XML serialization purposes </summary>
        [IgnoreDataMember]
        [XmlAttribute("usedCount")]
        public int Used_Count_XML
        {
            get
            {
                if (Used_Count.HasValue) return Used_Count.Value;
                return -1;
            }
            set { if ( value >= 0 ) Used_Count = value; }
        }

        /// <summary> Constructor for a new instance of the Builder_Module_Set_Info class </summary>
        public Builder_Module_Set_Info()
        {
            // Actually DON'T set the collection.. we don't always need it
            // Builder_Modules = new List<Builder_Module_Setting>();
            
        }

        /// <summary> Method suppresses XML Serialization of the Used_Count flag property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeUsed_Count_XML()
        {
            return Used_Count.HasValue;
        }
    }
}
