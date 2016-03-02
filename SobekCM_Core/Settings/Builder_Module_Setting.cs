#region Using directives

using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Setting information for a single builder module </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("builderModule")]
    public class Builder_Module_Setting
    {
        /// <summary> [DataMember] Name of the assembly DLL (if not a part of the standard SobekCM assembly </summary>
        [DataMember(Name="assembly",EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(1)]
        public string Assembly { get; set; }

        /// <summary> [DataMember] Fully qualified name of the class for this builder module </summary>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(2)]
        public string Class { get; set; }

        /// <summary> [DataMember] First argument necessary for the execution of this interface methods </summary>
        [DataMember(Name = "argument1", EmitDefaultValue = false)]
        [XmlAttribute("argument1")]
        [ProtoMember(3)]
        public string Argument1 { get; set; }

        /// <summary> [DataMember] Second argument necessary for the execution of this interface methods </summary>
        [DataMember(Name = "argument2", EmitDefaultValue = false)]
        [XmlAttribute("argument2")]
        [ProtoMember(4)]
        public string Argument2 { get; set; }

        /// <summary> [DataMember] Third argument necessary for the execution of this interface methods </summary>
        [DataMember(Name = "argument3", EmitDefaultValue = false)]
        [XmlAttribute("argument3")]
        [ProtoMember(5)]
        public string Argument3 { get; set; }

        /// <summary> [DataMember] Description of the module's primary function, from the dataase </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        [XmlAttribute("description")]
        [ProtoMember(6)]
        public string Description { get; set; }

        /// <summary> Key for this setting, used to lookup for duplicate references to the same module </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public string Key
        {
            get
            {
                // Get the assembly/class/arguments string for lookup
                StringBuilder builder = new StringBuilder(1000);
                if (!String.IsNullOrEmpty(Assembly))
                    builder.Append(Assembly + "|");
                else
                    builder.Append("|");
                builder.Append(Class);
                if (!String.IsNullOrEmpty(Argument1))
                    builder.Append(Argument1 + "|");
                else
                    builder.Append("|");
                if (!String.IsNullOrEmpty(Argument2))
                    builder.Append(Argument2 + "|");
                else
                    builder.Append("|");
                if (!String.IsNullOrEmpty(Argument3))
                    builder.Append(Argument3);
                return builder.ToString();
            }
        }

    }
}
