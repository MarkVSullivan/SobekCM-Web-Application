using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Settings from the database for built-in archiving functionality </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ArchiveSettings")]
    public class Archive_Settings
    {
        /// <summary> Drop box where packages can be placed to be archived locally </summary>
        [DataMember(Name = "dropbox", EmitDefaultValue = false)]
        [XmlAttribute("dropbox")]
        [ProtoMember(1)]
        public string Archive_DropBox { get; set; }

        /// <summary> Gets the regular expression for matching files names to delete AFTER archiving
        /// incoming digital resource files </summary>
        [DataMember(Name = "dropbox", EmitDefaultValue = false)]
        [XmlAttribute("postArchiveDeletes")]
        [ProtoMember(2)]
        public string PostArchive_Files_To_Delete { get; set; }

        /// <summary> Gets the regular expression for matching files names to delete BEFORE archiving
        /// incoming digital resource files </summary>
        [DataMember(Name = "dropbox", EmitDefaultValue = false)]
        [XmlAttribute("preArchiveDeletes")]
        [ProtoMember(3)]
        public string PreArchive_Files_To_Delete { get; set; }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Override_Seconds_Between_Polls flag property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeArchive_DropBox()
        {
            return (!String.IsNullOrEmpty(Archive_DropBox));
        }

        #endregion

    }
}
