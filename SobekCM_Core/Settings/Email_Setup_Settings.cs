using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Details on how the system can send emails (either through database or direct to SMTP port) </summary>
    [Serializable, DataContract, ProtoContract]
    public class Email_Setup_Settings
    {
        /// <summary> Method that emails are sent from this system </summary>
        [DataMember(Name = "method")]
        [XmlElement("method")]
        [ProtoMember(1)]
        public Email_Method_Enum Method { get; set; }

        /// <summary> SMTP Server name for sending emails </summary>
        [DataMember(Name = "smtpServer", EmitDefaultValue = false)]
        [XmlElement("smtpServer")]
        [ProtoMember(2)]
        public string SmtpServer { get; set; }

        /// <summary> SMTP Server port for sending emails </summary>
        [DataMember(Name = "smtpPort", EmitDefaultValue = false)]
        [XmlElement("smtpPort")]
        [ProtoMember(3)]
        public int SmtpPort { get; set; }

        /// <summary> Email address for the FROM portion, if none is provided in the actual email request </summary>
        [DataMember(Name = "defaultFromAddress", EmitDefaultValue = false)]
        [XmlElement("defaultFromAddress")]
        [ProtoMember(4)]
        public string DefaultFromAddress { get; set; }

        /// <summary> Email address display portion, or NULL, to use if none is provided in the actual email request </summary>
        [DataMember(Name = "defaultFromDisplay", EmitDefaultValue = false)]
        [XmlElement("defaultFromDisplay")]
        [ProtoMember(5)]
        public string DefaultFromDisplay { get; set; }

        /// <summary> Get or set the values for the email method by string  </summary>
        [XmlIgnore]
        public string MethodString
        {
            get
            {
                switch (Method)
                {
                    case Email_Method_Enum.MsSqlDatabaseMail:
                        return "DATABASE MAIL";

                    case Email_Method_Enum.SmtpDirect:
                        return "SMTP DIRECT";

                    default:
                        return "DATABASE MAIL";
                }
            }
            set
            {
                if (String.Compare(value, "DATABASE MAIL", StringComparison.OrdinalIgnoreCase) == 0)
                    Method = Email_Method_Enum.MsSqlDatabaseMail;
                if (String.Compare(value, "SMTP DIRECT", StringComparison.OrdinalIgnoreCase) == 0)
                    Method = Email_Method_Enum.SmtpDirect;
            }
        }


        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the SmtpServer property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeSmtpServer()
        {
            return (!String.IsNullOrEmpty(SmtpServer));
        }


        /// <summary> Method suppresses XML Serialization of the SmtpPort property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeSmtpPort()
        {
            return (!String.IsNullOrEmpty(SmtpServer));
        }

        #endregion
    }
}
