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
    [XmlRoot("ExtensionAdminInfo")]
    public class ExtensionAdminInfo
    {
        /// <summary> Description of this plug-in functionality </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        [XmlElement("description")]
        [ProtoMember(1)]
        public string Description;

        [DataMember(Name = "authors", EmitDefaultValue = false)]
        [XmlArray("authors")]
        [XmlArrayItem("author", typeof(ExtensionAdminAuthorInfo))]
        [ProtoMember(2)]
        public List<ExtensionAdminAuthorInfo> Authors;

        /// <summary> Text statement on permissions required to use this plug-in </summary>
        [DataMember(Name = "permissions", EmitDefaultValue = false)]
        [XmlElement("permissions")]
        [ProtoMember(3)]
        public string Permissions;


        /// <summary> Method suppresses XML Serialization of the Description property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeDescription()
        {
            return !String.IsNullOrWhiteSpace(Description);
        }

        /// <summary> Method suppresses XML Serialization of the Authors property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeAuthors()
        {
            return ((Authors != null) && (Authors.Count > 0));
        }

        /// <summary> Method suppresses XML Serialization of the Permissions property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializePermissions()
        {
            return !String.IsNullOrWhiteSpace(Permissions);
        }

        /// <summary> Adds information about an author of this plug-in </summary>
        /// <param name="Name"> Name of this author of the plug-in </param>
        /// <param name="Email"> Email address, if provided, for this author </param>
        public void Add_Author(string Name, string Email)
        {
            if (Authors == null) Authors = new List<ExtensionAdminAuthorInfo>();

            ExtensionAdminAuthorInfo thisAuthor = new ExtensionAdminAuthorInfo();
            thisAuthor.Name = Name;

            if (!String.IsNullOrWhiteSpace(Email))
                thisAuthor.Email = Email;

            Authors.Add(thisAuthor);
        }
    }
}
