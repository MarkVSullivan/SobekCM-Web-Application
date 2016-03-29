using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;


namespace SobekCM.Core.FileSystems
{
    /// <summary> Basic information about a single file in the file system </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("fileInfo")]
    public class SobekFileSystem_FileInfo
    {
        /// <summary> Name of this file in the system </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Time this file was last written to </summary>
        [DataMember(Name = "lastWriteTime")]
        [XmlIgnore]
        [ProtoMember(2)]
        public DateTime? LastWriteTime { get; set; }

        /// <summary> Time this file was last written to (for XML serialization)</summary>
        /// <remarks> This property is only exposed to allow for XML serialization of the nullable datetime </remarks>
        [IgnoreDataMember]
        [XmlAttribute("lastWriteTime")]
        public DateTime LastWriteTime_XML
        {
            get { return LastWriteTime.HasValue ? LastWriteTime.Value : DateTime.MinValue; }
            set { if (value != DateTime.MinValue) LastWriteTime = value; }
        }

        /// <summary> Property controls if the associated property is serialized during XML serialization </summary>
        public bool ShouldSerializeLastWriteTime_XML
        {
            get { return LastWriteTime.HasValue; }
        }

        /// <summary> Gets the string representing the extension part of the file name </summary>
        [DataMember(Name = "extension")]
        [XmlAttribute("extension")]
        [ProtoMember(3)]
        public string Extension { get; set; }

        /// <summary> Gets the size, in bytes, of this file </summary>
        [DataMember(Name = "length")]
        [XmlAttribute("length")]
        [ProtoMember(4)]
        public long Length { get; set; }
    }
}
