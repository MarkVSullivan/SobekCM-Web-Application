#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Stores display information for a single possible disposition, or how physical material should be
    /// handled after digitization completes </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("DispositionOption")]
    public class Disposition_Option
    {
        /// <summary> This disposition in a future tense (default language) </summary>
        [DataMember(Name = "future", EmitDefaultValue = false)]
        [XmlAttribute("future")]
        [ProtoMember(1)]
        public string Future { get; internal set; }

        /// <summary> This disposition in a past tense (default language) </summary>
        [DataMember(Name = "past", EmitDefaultValue = false)]
        [XmlAttribute("past")]
        [ProtoMember(2)]
        public string Past { get; internal set; }

        /// <summary> Key to this disposition </summary>
        [DataMember(Name = "key", EmitDefaultValue = false)]
        [XmlAttribute("key")]
        [ProtoMember(3)]
        public int Key { get; internal set; }

        /// <summary> Constructor for a new instance of the Disposition_Option class </summary>
        /// <remarks> Empty constructor for serialization purposes </remarks>
        public Disposition_Option()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Constructor for a new instance of the Disposition_Option class </summary>
        /// <param name="Key"> Key to this disposition </param>
        /// <param name="Past"> This disposition in a past tense (default language) </param>
        /// <param name="Future"> This disposition in a future tense (default language)</param>
        public Disposition_Option(int Key, string Past, string Future)
        {
            this.Future = Future;
            this.Past = Past;
            this.Key = Key;
        }
    }
}
