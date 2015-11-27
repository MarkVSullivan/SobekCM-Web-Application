using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Settings for emails, including email setup and main email addresses used in certain situations </summary>
    [Serializable, DataContract, ProtoContract]
    public class Email_Settings
    {
        /// <summary> Constructor for a new instance of the Email_Settings class </summary>
        public Email_Settings()
        {
            Setup = new Email_Setup_Settings();

            Send_On_Added_Aggregation = "Always";
            Privacy_Email = String.Empty;
        }

        /// <summary> Email address used when a possible privacy issue appears, such as an apaprent social security number </summary>
        [DataMember(Name = "privacyEmail", EmitDefaultValue = false)]
        [XmlElement("privacyEmail")]
        [ProtoMember(1)]
        public string Privacy_Email { get; set; }

        /// <summary> Flag indicates when emails should be sent on aggrgeations being added </summary>
        [DataMember(Name = "sendOnAddedAggregation", EmitDefaultValue = false)]
        [XmlElement("sendOnAddedAggregation")]
        [ProtoMember(2)]
        public string Send_On_Added_Aggregation { get; set; }

        /// <summary> Email address for system errors </summary>
        [DataMember(Name = "systemErrorEmail", EmitDefaultValue = false)]
        [XmlElement("systemErrorEmail")]
        [ProtoMember(3)]
        public string System_Error_Email { get; set; }

        /// <summary> Main email address for this system </summary>
        [DataMember(Name = "systemEmail", EmitDefaultValue = false)]
        [XmlElement("systemEmail")]
        [ProtoMember(4)]
        public string System_Email { get; set; }

        [DataMember(Name = "setup", EmitDefaultValue = false)]
        [XmlElement("setup")]
        [ProtoMember(5)]
        public Email_Setup_Settings Setup { get; set; }



    }
}
