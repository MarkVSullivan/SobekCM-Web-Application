using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Extensions
{
    /// <summary> Administrative information about an extension/plug-in, such as description,
    /// authors, permissions, etc.. </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionAuthorInfo")]
    public class ExtensionAdminAuthorInfo
    {
        /// <summary> Full name of this author of a plug-in/extension </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Email address for this author, if provided </summary>
        [DataMember(Name = "email", EmitDefaultValue = false)]
        [XmlAttribute("email")]
        [ProtoMember(2)]
        public string Email { get; set; }

        /// <summary> Method suppresses XML Serialization of the Email property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeEmail()
        {
            return !String.IsNullOrWhiteSpace(Email);
        }

    }
}
