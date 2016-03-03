using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Builder
{
    /// <summary> History of a single execution of a scheduled task </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("scheduledTaskExecution")]
    public class Builder_Scheduled_Task_Execution_History
    {
        /// <summary> The date of this execution history for a builder scheduled task </summary>
        [DataMember(Name = "date", EmitDefaultValue = false)]
        [XmlAttribute("date")]
        [ProtoMember(1)]
        public DateTime Date { get; set; }

        /// <summary> Outcome of this execution for a builder scheduled task </summary>
        [DataMember(Name = "outcome", EmitDefaultValue = false)]
        [XmlAttribute("outcome")]
        [ProtoMember(2)]
        public string Outcome { get; set; }

        /// <summary> Complete message from this execution of a builder scheduled task </summary>
        [DataMember(Name = "message", EmitDefaultValue = false)]
        [XmlAttribute("message")]
        [ProtoMember(3)]
        public string Message { get; set; }
    }
}
