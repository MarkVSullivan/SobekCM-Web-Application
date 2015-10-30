using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Simple setting object holds the key, the value, and the ID </summary>
    [Serializable, DataContract, ProtoContract]
    public class Simple_Setting
    {
        /// <summary> Constructor for a new instance of the <see cref="Simple_Setting"/> class </summary>
        public Simple_Setting()
        {
            // Do nothing - this is here for serialization purposes
        }

        /// <summary> Constructor for a new instance of the <see cref="Simple_Setting"/> class </summary>
        /// <param name="Key"> Name / key for this database setting</param>
        /// <param name="Value"> Current value for this setting </param>
        /// <param name="SettingID"> Unique key to this setting </param>
        public Simple_Setting(string Key, string Value, short SettingID)
        {
            this.Key = Key;
            this.Value = Value;
            this.SettingID = SettingID;
        }

        /// <summary> Name / key for this database setting </summary>
        [DataMember(Name = "key")]
        [XmlAttribute("key")]
        [ProtoMember(1)]
        public string Key;

        /// <summary> Current value for this setting </summary>
        [DataMember(Name = "value", EmitDefaultValue = false)]
        [XmlElement("value")]
        [ProtoMember(2)]
        public string Value;

        /// <summary> Unique key to this setting </summary>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(3)]
        public short SettingID;
    }
}
