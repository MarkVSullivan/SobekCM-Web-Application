using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Message
{
    /// <summary> Message is returned with a flag indicating success or failure, an overall message,
    /// and potentially a list of error that occurred </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("responseMessage")]
    public class EnableExtensionMessage
    {
        /// <summary> Flag indicates if the overall process was a success or failure </summary>
        [DataMember(Name = "success")]
        [XmlAttribute("success")]
        [ProtoMember(1)]
        public bool Success { get; set; }

        /// <summary> Overall message indicating success or failure to the user </summary>
        [DataMember(Name = "message")]
        [XmlElement("message")]
        [ProtoMember(2)]
        public string Message { get; set; }

        /// <summary> Possible list of errors that occurred during the procssing </summary>
        [DataMember(EmitDefaultValue = false, Name = "errors")]
        [XmlArray("errors")]
        [XmlArrayItem("error", typeof(string))]
        [ProtoMember(3)]
        public List<string> Errors { get; set; }

        /// <summary> Add an error to this message </summary>
        /// <param name="Error"> Error text to add</param>
        public void Add_Error(string Error)
        {
            if (Errors == null) Errors = new List<string>();

            Errors.Add(Error);
        }
    }
}
