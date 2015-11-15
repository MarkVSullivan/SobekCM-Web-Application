#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Configuration
{
    /// <summary> A single translated value, or a pair of the web language and the string itself </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("WebLanguageTranslationValue")]
    public class Web_Language_Translation_Value
    {
        /// <summary> Language in which this value is represented </summary>
        [DataMember(Name = "language")]
        [XmlAttribute("language")]
        [ProtoMember(1)]
        public Web_Language_Enum Language { get; set; }

        /// <summary> Value in provided language </summary>
        [DataMember(Name = "value")]
        [XmlAttribute("value")]
        [ProtoMember(2)]
        public string Value { get; set; }

        /// <summary> Constructor for a new instance of the Web_Language_Translation_Value class </summary>
        public Web_Language_Translation_Value()
        {
            // Parameterless constructor for serialization
        }

        /// <summary> Constructor for a new instance of the Web_Language_Translation_Value class </summary>
        /// <param name="Language"> Language in which this value is represented </param>
        /// <param name="Value"> Value in provided language </param>
        public Web_Language_Translation_Value(Web_Language_Enum Language, string Value)
        {
            this.Language = Language;
            this.Value = Value;
        }
    }
}
