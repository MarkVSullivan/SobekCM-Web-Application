using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core
{
    /// <summary> Fully serializable string key/value pair class </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("KeyValuePair")]
    public class StringKeyValuePair
    {
        /// <summary> Key for this key/value pair </summary>
        [DataMember(Name = "key")]
        [XmlAttribute("key")]
        [ProtoMember(1)]
        public string Key { get; set; }

        /// <summary> Value for this key/value pair </summary>
        [DataMember(Name = "value")]
        [XmlAttribute("value")]
        [ProtoMember(2)]
        public string Value { get; set; }

        /// <summary> Constructor for a new instance of the StringKeyValuePair class </summary>
        public StringKeyValuePair()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Constructor for a new instance of the StringKeyValuePair class </summary>
        /// <param name="Key"> Key for this key/value pair </param>
        /// <param name="Value"> Value for this key/value pair </param>
        public StringKeyValuePair(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }
}
