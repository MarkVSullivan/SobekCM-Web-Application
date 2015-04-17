using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Set of files grouped together, traditionally as a page or a 
    /// set of downloads that are very related </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_FileGrouping
    {
        /// <summary> Label for this file grouping </summary>
        [DataMember(Name = "label")]
        [ProtoMember(1)]
        public string Label;

        /// <summary> Collection of one or more files related to this grouping </summary>
        [DataMember(Name="files")]
        [ProtoMember(2)]
        public List<BriefItem_File> Files;

        /// <summary> Constructor for a new instance of the BriefItem_FileGrouping class </summary>
        public BriefItem_FileGrouping()
        {
            // Does nothing - needed for deserialization
        }
    }
}
