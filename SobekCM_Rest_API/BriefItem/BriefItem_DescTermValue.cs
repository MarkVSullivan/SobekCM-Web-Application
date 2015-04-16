using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Rest_API.BriefItem
{
    /// <summary> A single value for a metadata term, which may also have a collection of URIs referenced </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_DescTermValue
    {
        /// <summary> String version of this single value for a metadata term/type </summary>
        [DataMember(Name = "value")]
        [ProtoMember(1)]
        public string Value { get; private set; }

        /// <summary> URI references related to this single value </summary>
        [DataMember(EmitDefaultValue = false, Name = "uris")]
        [ProtoMember(2)]
        public List<string> URIs { get; set; }

        /// <summary> Authority which may be linked to this value (optional) </summary>
        [DataMember(EmitDefaultValue = false, Name = "authority")]
        [ProtoMember(3)]
        public string Authority { get; set; }

        /// <summary> Sub-term (or display label), which may be associated with this value (optional) </summary>
        [DataMember(EmitDefaultValue = false, Name = "subterm")]
        [ProtoMember(4)]
        public string SubTerm { get; set; }

        /// <summary> Language of this value (optional) </summary>
        [DataMember(EmitDefaultValue = false, Name = "lang")]
        [ProtoMember(5)]
        public string Language { get; set; }

        /// <summary> Constructor for a new instance of the BriefItem_DescTermValue class </summary>
        public BriefItem_DescTermValue()
        {
            // Does nothing - used for deserialization
        }

        /// <summary> Constructor for a new instance of the BriefItem_DescTermValue class </summary>
        /// <param name="Value"> String version of this single value for a metadata term/typeString version of this single value for a metadata term/type </param>
        public BriefItem_DescTermValue( string Value )
        {
            this.Value = Value;
        }

        /// <summary> Add a single URI to this value </summary>
        /// <param name="URI"> URI to add </param>
        public void Add_URI(string URI)
        {
            if (URIs == null)
                URIs = new List<string>();

            URIs.Add(URI);
        }
    }
}
