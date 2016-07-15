using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Class holds the information about a single user-entered descriptive tag in relation to this digital object </summary>
    /// <remarks>This data is not stored in the item metadata, but is retrieved from the database when the item is displayed in the digital library </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("userTag")]
    public class BriefItem_UserTag
    {
        /// <summary> Primary key for this tag from the database </summary>
        /// <remarks> This is useful for deleting a tag or editing an existing tag in the database </remarks>
        [DataMember(Name = "id")]
        [XmlElement("id")]
        [ProtoMember(1)]
        public int TagID { get; set; }

        /// <summary> Date the tag was added or last modified </summary>
        [DataMember(Name = "date")]
        [XmlElement("date")]
        [ProtoMember(2)]
        public DateTime Date_Added { get; set; }

        /// <summary> Primary key to the user who entered this tag </summary>
        [DataMember(Name = "user_id")]
        [XmlElement("user_id")]
        [ProtoMember(3)]
        public int UserID { get; set; }

        /// <summary> Name of the user who entered this tag in the format 'Last Name, First Name' </summary>
        [DataMember(Name = "user")]
        [XmlElement("user")]
        [ProtoMember(4)]
        public string UserName { get; set; }

        /// <summary> Text of the user-entered descriptive tag </summary>
        [DataMember(Name = "text")]
        [XmlText]
        [ProtoMember(5)]
        public string Description_Tag { get; set; }

        /// <summary> Constructor for a new instance of the BriefItem_UserTag class </summary>
        /// <param name="UserID"> Primary key for the user who entered this tag </param>
        /// <param name="UserName"> Name of the user ( Last Name, Firt Name )</param>
        /// <param name="Description_Tag"> Text of the user-entered descriptive tag </param>
        /// <param name="Date_Added"> Date the tag was added or last modified </param>
        /// <param name="TagID"> Primary key for this tag from the database </param>
        public BriefItem_UserTag(int UserID, string UserName, string Description_Tag, DateTime Date_Added, int TagID)
        {
            this.UserName = UserName;
            this.UserID = UserID;
            this.Description_Tag = Description_Tag;
            this.Date_Added = Date_Added;
            this.TagID = TagID;
        }

        /// <summary> Constructor for a new instance of the BriefItem_UserTag class </summary>
        /// <remarks> This is an empty constructor for serialization/deserialization purposes </remarks>
        public BriefItem_UserTag()
        {
            // Empty constructor for serialization / deserialization purposes
        }

    }
}
