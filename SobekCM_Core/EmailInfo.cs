#region Using directives

using System;
using System.Runtime.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core
{
    /// <summary> Class represents information for an email to be sent by the system </summary>
    [Serializable, DataContract, ProtoContract]
    public class EmailInfo
    {
        /// <summary> List of recipients, seperated by semicolons </summary>
        [DataMember(Name = "recipients")]
        [ProtoMember(1)]
        public string RecipientsList { get; set; }

        /// <summary> Subject line for the email to be sent </summary>
        [DataMember(Name = "subject")]
        [ProtoMember(2)]
        public string Subject { get; set; }

        /// <summary> Body of the email to tbe sent </summary>
        [DataMember(Name = "body")]
        [ProtoMember(3)]
        public string Body { get; set; }

        /// <summary> From email address for this email to be sent </summary>
        /// <remarks> This can include the format 'Name &lt;name@domain.com&gt;'</remarks>
        [DataMember(Name = "from")]
        [ProtoMember(4)]
        public string FromAddress { get; set; }

        /// <summary> Email address that responses should be sent to (or NULL) </summary>
        [DataMember(EmitDefaultValue = false, Name = "replyTo")]
        [ProtoMember(5)]
        public string ReplyTo { get; set;  }

        /// <summary> Flag indicates if this should be sent as HTML or not </summary>
        [DataMember(Name = "isHtml")]
        [ProtoMember(6)]
        public bool isHTML { get; set; }

        /// <summary> Flag indicates if this is from the Contact Us form </summary>
        [DataMember(Name = "isContactUs")]
        [ProtoMember(7)]
        public bool isContactUs { get; set; }

        /// <summary> ID is recorded if this is a response to an existing email in the system </summary>
        [DataMember(EmitDefaultValue = false, Name = "replayId")]
        [ProtoMember(8)]
        public int? ReplayToEmailID { get; set; }

        /// <summary> UserID (or NULL if system) that initiated this email request </summary>
        [DataMember(EmitDefaultValue = false, Name = "userId")]
        [ProtoMember(9)]
        public int? UserID { get; set;  }

    }
}
