#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Set of files grouped together, traditionally as a page or a 
    /// set of downloads that are very related </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_FileGrouping
    {
        /// <summary> Label for this file grouping </summary>
        [DataMember(Name = "label")]
        [XmlAttribute("label")]
        [ProtoMember(1)]
        public string Label;

        /// <summary> Collection of one or more files related to this grouping </summary>
        [DataMember(Name="files")]
        [XmlArray("files")]
        [XmlArrayItem("file", typeof(BriefItem_File))]
        [ProtoMember(2)]
        public List<BriefItem_File> Files;

        /// <summary> Constructor for a new instance of the BriefItem_FileGrouping class </summary>
        public BriefItem_FileGrouping()
        {
            Files = new List<BriefItem_File>();
        }

        /// <summary> Constructor for a new instance of the BriefItem_FileGrouping class </summary>
        /// <param name="Label"> Label for this file grouping </param>
        public BriefItem_FileGrouping(string Label)
        {
            this.Label = Label;
            Files = new List<BriefItem_File>();
        }
    }
}
