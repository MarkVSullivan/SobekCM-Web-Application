using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.BriefItem;

namespace SobekCM.Core.ResultTitle
{
    /// <summary> Single value for a descriptive term/type for a title within a result set </summary>
    [Serializable, DataContract, ProtoContract]
    public class ResultTitle_DescriptiveTerm
    {
        /// <summary> Normalized term for this metadata element, as employed by the SobekCM system </summary>
        [DataMember(Name = "term")]
        [XmlAttribute("term")]
        [ProtoMember(1)]
        public string Term { get; set; }

        /// <summary> List of the values tied to this item of this metadata type </summary>
        [DataMember(EmitDefaultValue = false, Name = "properties")]
        [XmlArray("properties")]
        [XmlArrayItem("property", typeof(BriefItem_DescTermValue))]
        [ProtoMember(2)]
        public List<BriefItem_DescTermValue> Values { get; set; }

        /// <summary> Constructor for a new instance of the ResultTitle_DescriptiveTerm class </summary>
        public ResultTitle_DescriptiveTerm()
        {
            // Empty constructor does nothing.. used for deserialization
        }

        /// <summary> Constructor for a new instance of the ResultTitle_DescriptiveTerm class </summary>
        /// <param name="Term"> Normalized term for this metadata element, as employed by the SobekCM system </param>
        public ResultTitle_DescriptiveTerm(string Term)
        {
            this.Term = Term;
        }

        /// <summary> Adds this new value to this citation element </summary>
        /// <param name="Value"> Value as a simple string </param>
        public BriefItem_DescTermValue Add_Value(string Value)
        {
            if ( Values == null )
                Values = new List<BriefItem_DescTermValue>();

            BriefItem_DescTermValue newValue = new BriefItem_DescTermValue(Value);
            Values.Add(newValue);
            return newValue;
        }
    }
}
