#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Information about a single workflow type used for tracking all work that occurs against
    /// a single digital resource  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("WorkflowType")]
    public class Workflow_Type
    {
        /// <summary> Workflow type name, such as 'Scanned', 'Online Metadata Edit', etc.. (default language) </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Key to this workflow type </summary>
        [DataMember(Name = "key", EmitDefaultValue = false)]
        [XmlAttribute("key")]
        [ProtoMember(2)]
        public int Key { get; set; }

        /// <summary> Constructor for a new instance of the Workflow_Type class </summary>
        /// <remarks> Empty constructor for serialization purposes </remarks>
        public Workflow_Type()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Constructor for a new instance of the Workflow_Type class </summary>
        /// <param name="Key"> Key to this workflow type</param>
        /// <param name="Name">Workflow type name, such as 'Scanned', 'Online Metadata Edit', etc.. (default language)</param>
        public Workflow_Type(int Key, string Name)
        {
            this.Key = Key;
            this.Name = Name;
        }
    }
}
