using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Shortcuts that are used just for display of this item within the web </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("web")]
    public class BriefItem_Web
    {
        private Dictionary<string, string> fileExtensionLookupDictionary;


        /// <summary> Base source URL for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "sourceUrl")]
        [XmlElement("sourceUrl")]
        [ProtoMember(1)]
        public string Source_URL { get; set; }

        /// <summary> List of all the file extensions included in this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "fileExtensions")]
        [XmlArray("fileExtensions")]
        [XmlArrayItem("extension", typeof(string))]
        [ProtoMember(2)]
        public List<string> File_Extensions { get; set; }

        /// <summary> Number of siblings which this item has ( i.e., is this a multi-volume set? ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "siblings")]
        [XmlElement("siblings")]
        [ProtoMember(3)]
        public int? Siblings { get; set; }

        /// <summary> Primary key (ItemID) for this individual item/volume from the SobekCM Web database </summary>
        [DataMember(Name = "itemid")]
        [XmlAttribute("itemid")]
        [ProtoMember(4)]
        public int ItemID { get; set; }

        /// <summary> Primary key (GroupID) for the BibiD / title from the SobekCM Web database </summary>
        [DataMember(Name = "groupid")]
        [XmlAttribute("groupid")]
        [ProtoMember(5)]
        public int GroupID { get; set; }

        /// <summary> Checks to see if a file extension exists in the file extensions lists </summary>
        /// <param name="Extension"> File extension to check </param>
        /// <returns> TRUE if the file extension exists, otherwise FALSE </returns>
        /// <remarks> The file extension check is not caps sensitive and is tolerant of a period at the beginning, but it is
        /// not necessary either.  </remarks>
        public bool Contains_File_Extension(string Extension)
        {
            // If dictionary is NULL, define it
            if (fileExtensionLookupDictionary == null)
                fileExtensionLookupDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // If the file extensions list is NULL, than probably no files attached
            if ((File_Extensions == null) || (File_Extensions.Count == 0))
                return false;

            // If the count doesn't match the file extensions, than add them all
            if (fileExtensionLookupDictionary.Count != File_Extensions.Count)
            {
                fileExtensionLookupDictionary.Clear();
                foreach (string thisExtention in File_Extensions)
                {
                    if ((thisExtention.Length > 1) && (thisExtention[0] == '.'))
                        fileExtensionLookupDictionary.Add(thisExtention.Substring(1), thisExtention);
                    else
                        fileExtensionLookupDictionary.Add(thisExtention, thisExtention);
                }
            }

            // Now, just look to see if it exists (but check first to see if a period was passed in)
            if ((Extension.Length > 1) && (Extension[0] == '.'))
                return fileExtensionLookupDictionary.ContainsKey(Extension.Substring(1));
            
            // No period, so simple test
            return fileExtensionLookupDictionary.ContainsKey(Extension);
        }


    }
}
