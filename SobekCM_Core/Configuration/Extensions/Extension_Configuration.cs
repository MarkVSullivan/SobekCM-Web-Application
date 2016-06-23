using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Extensions
{
    /// <summary> Collection of all the extension information </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionConfig")]
    public class Extension_Configuration
    {
        private Dictionary<string, string> assemblyDictionary;

        /// <summary> Collection of information about each extension </summary>
        [DataMember(Name = "extensions", EmitDefaultValue = false)]
        [XmlArray("extensions")]
        [XmlArrayItem("extension", typeof(ExtensionInfo))]
        [ProtoMember(1)]
        public List<ExtensionInfo> Extensions { get; set; }

        /// <summary> Add a new extension to this list of extensions </summary>
        /// <param name="NewExtension"> New extension to add </param>
        public void Add_Extension(ExtensionInfo NewExtension)
        {
            if (Extensions == null)
                Extensions = new List<ExtensionInfo>();

            Extensions.Add(NewExtension);
        }

        /// <summary> Gets an extension, by extension code, otherwise NULL </summary>
        /// <param name="ExtensionCode"> Unique extension code for the extension information to retrieve </param>
        /// <returns> Extension information, or NULL if no matching extension was found </returns>
        public ExtensionInfo Get_Extension(string ExtensionCode)
        {
            if ((Extensions == null) || (Extensions.Count == 0))
                return null;

            foreach (ExtensionInfo thisExtension in Extensions)
            {
                if (String.Compare(thisExtension.Code, ExtensionCode, StringComparison.OrdinalIgnoreCase) == 0)
                    return thisExtension;
            }

            return null;
        }

        /// <summary> Gets the absolute path and filename for an assembly included in one of the 
        /// extensions, by extension ID </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string Get_Assembly(string ID)
        {
           if ((Extensions == null) || (Extensions.Count == 0))
                return null;

            // If the dictionary has not been built, build it
            if (assemblyDictionary == null)
            {
                assemblyDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (ExtensionInfo thisExtension in Extensions)
                {
                    if (thisExtension.Assemblies != null)
                    {
                        foreach (ExtensionAssembly thisAssembly in thisExtension.Assemblies)
                        {
                            assemblyDictionary[thisAssembly.ID] = thisAssembly.FilePath;
                        }
                    }
                }
            }

            // Now look and return the assembly if the ID exists
            if (assemblyDictionary.ContainsKey(ID))
                return assemblyDictionary[ID];

            return null;
        }
    }
}
