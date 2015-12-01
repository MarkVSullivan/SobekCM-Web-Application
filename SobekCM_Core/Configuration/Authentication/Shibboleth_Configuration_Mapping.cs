using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Users;

namespace SobekCM.Core.Configuration.Authentication
{
    /// <summary> Stores the link between a mapping to a user object and a string value for 
    /// purposes of Shibboleth authentication </summary>
    /// <remarks> This class is used to hold constants ( a value --> mapping ) and 
    /// general mapping from a shibboleth key to a mapping </remarks>
    [Serializable, DataContract, ProtoContract]
    public class Shibboleth_Configuration_Mapping
    {

        /// <summary> Indicates a mapping to an attribute under the user class </summary>
        [DataMember(Name = "attribute", EmitDefaultValue = false)]
        [XmlAttribute("attribute")]
        [ProtoMember(1)]
        public User_Object_Attribute_Mapping_Enum Mapping { get; set; }

        /// <summary> Value, either a constant to put in the user via the mapping, or the 
        /// Shibboleth authentication key, used to find a user specific value and map
        /// to the user class </summary>
        [DataMember(Name = "value", EmitDefaultValue = false)]
        [XmlAttribute("value")]
        [ProtoMember(2)]
        public string Value { get; set; }

        /// <summary> Constructor for a new instance of the Shibboleth_Configuration_Mapping class </summary>
        public Shibboleth_Configuration_Mapping()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Constructor for a new instance of the Shibboleth_Configuration_Mapping class </summary>
        /// <param name="Mapping"> Indicates a mapping to an attribute under the user class </param>
        /// <param name="Value"> Value, either a constant to put in the user via the mapping, or the 
        /// Shibboleth authentication key, used to find a user specific value and map
        /// to the user class </param>
        public Shibboleth_Configuration_Mapping(User_Object_Attribute_Mapping_Enum Mapping, string Value)
        {
            this.Mapping = Mapping;
            this.Value = Value;
        }
    }
}
