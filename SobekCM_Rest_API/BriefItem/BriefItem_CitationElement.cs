using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Rest_API.BriefItem
{
    /// <summary> Single value for a citation term/type within a brief item </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_CitationElement
    {
        /// <summary> Normalized term for this metadata element, as employed by the SobekCM system </summary>
        [DataMember(Name = "term")]
        [ProtoMember(1)]
        public string Term { get; set; }

        /// <summary> List of external references, which refers to the same metadata type as the normalized term </summary>
        [DataMember(EmitDefaultValue = false,Name="references")]
        [ProtoMember(2)]
        public List<string> References { get; set; }

        /// <summary> List of the values tied to this item of this metadata type </summary>
        [DataMember(EmitDefaultValue = false, Name = "values")]
        [ProtoMember(3)]
        public List<BriefItem_CitationElementValue> Values { get; private set; }

        /// <summary> Constructor for a new instance of the BriefItem_CitationElement class </summary>
        public BriefItem_CitationElement()
        {
            // Empty constructor does nothing.. used for deserialization
        }

        /// <summary> Constructor for a new instance of the BriefItem_CitationElement class </summary>
        /// <param name="Term"> Normalized term for this metadata element, as employed by the SobekCM system </param>
        public BriefItem_CitationElement(string Term)
        {
            this.Term = Term;
        }

        /// <summary> Adds this new value to this citation element </summary>
        /// <param name="Value"> Value as a simple string </param>
        public BriefItem_CitationElementValue Add_Value(string Value)
        {
            if ( Values == null )
                Values = new List<BriefItem_CitationElementValue>();

            BriefItem_CitationElementValue newValue = new BriefItem_CitationElementValue(Value);
            Values.Add(newValue);
            return newValue;
        }
    }
}
