using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Single element from within a TOC (table of contents) </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_TocElement
    {
        /// <summary> Name of this element, for display purposes </summary>
        [DataMember(Name = "name")]
        [ProtoMember(1)]
        public string Name;

        /// <summary> Sequence within the list of file groupings that this element should link to </summary>
        [DataMember(Name = "sequence")]
        [ProtoMember(2)]
        public int Sequence;

        /// <summary> Level within the TOC, if this is a hierarchically organized TOC </summary>
        [DataMember(EmitDefaultValue = false, Name = "level")]
        [ProtoMember(3)]
        public int? Level;

        /// <summary> Constructor for a new instance of the BriefItem_TocElement class </summary>
        public BriefItem_TocElement()
        {
            // Does nothing - needed for deserialization
        }
    }
}
