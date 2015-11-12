using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Settings from the database for built-in archiving functionality </summary>
    [Serializable, DataContract, ProtoContract]
    public class Archive_Settings
    {
        /// <summary> Drop box where packages can be placed to be archived locally </summary>
        [DataMember(Name = "dropbox", EmitDefaultValue = false)]
        [XmlElement("dropbox")]
        [ProtoMember(1)]
        public string Archive_DropBox { get; set; }

        /// <summary> Gets the regular expression for matching files names to delete AFTER archiving
        /// incoming digital resource files </summary>
        [DataMember(Name = "dropbox", EmitDefaultValue = false)]
        [XmlElement("postArchiveDeletes")]
        [ProtoMember(2)]
        public string PostArchive_Files_To_Delete { get; set; }

        /// <summary> Gets the regular expression for matching files names to delete BEFORE archiving
        /// incoming digital resource files </summary>
        [DataMember(Name = "dropbox", EmitDefaultValue = false)]
        [XmlElement("preArchiveDeletes")]
        [ProtoMember(3)]
        public string PreArchive_Files_To_Delete { get; set; }

    }
}
