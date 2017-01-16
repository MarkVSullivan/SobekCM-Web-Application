using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Aggregations
{
    /// <summary> Simple information about any child page to an item aggregation, regardless of whether this is a
    /// static html page, or a browse pulled from the database, used in the Simple_Aggregation object </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("childPage.simple")]
    public class Simple_Aggregation_Child_Page
    {
        /// <summary> Constructor for a new instance of the Simple_Aggregation_Child_Page class </summary>
        /// <param name="Code"> Code for this info or browse page </param>
        /// <param name="Label"> Label for this child page, in the current language </param>
        public Simple_Aggregation_Child_Page(string Code, string Label)
        {
            this.Code = Code;
            this.Label = Label;
        }

        /// <summary> Constructor for a new instance of the Simple_Aggregation_Child_Page class </summary>
        /// <remarks> This empty constructor is primarily used by the deserialization routines </remarks>
        public Simple_Aggregation_Child_Page()
        {
            // Empty constructor (used for deserializing)
        }

        /// <summary> Code for this info or browse page </summary>
        /// <remarks> This is the code that is used in the URL to specify this info or browse page </remarks>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary> Label for this child page, in the current language </summary>
        [DataMember(Name = "label", EmitDefaultValue = false)]
        [XmlAttribute("label")]
        [ProtoMember(2)]
        public string Label { get; set; }
    }
}
