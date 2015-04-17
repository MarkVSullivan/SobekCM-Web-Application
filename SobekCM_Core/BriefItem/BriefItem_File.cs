using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Information about a single file within a digital resource </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_File
    {
        /// <summary> Name for this file </summary>
        /// <remarks> If this is not in the resource folder, this may include a URL </remarks>
        [DataMember(Name = "name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Width of the file (in pixels), if relevant and available </summary>
        /// <remarks> This could be the actual width, or the preferred viewing width </remarks>
        [DataMember(EmitDefaultValue = false, Name = "width")]
        [ProtoMember(2)]
        private int? Width { get; set; }

        /// <summary> Height of the file (in pixels), if relevant and available </summary>
        /// <remarks> This could be the actual height, or the preferred viewing height </remarks>
        [DataMember(EmitDefaultValue = false, Name = "height")]
        [ProtoMember(3)]
        private int? Height { get; set; }

        /// <summary> Other attributes associated with this file, that may be needed for display purposes </summary>
        [DataMember(EmitDefaultValue = false, Name = "attributes")]
        [ProtoMember(4)]
        private string Attributes { get; set; }
        
        /// <summary> Constructor for a new instance of the BriefItem_File class </summary>
        public BriefItem_File()
        {
            // Does nothing - needed for deserialization
        }
    }
}
