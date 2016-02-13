using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    public class BriefItem_Behaviors
    {
        /// <summary> List of page file extenions that should be listed in the downloads tab </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageFileExtensionsForDownload")]
        [XmlElement("pageFileExtensionsForDownload")]
        [ProtoMember(1)]
        public string[] Page_File_Extensions_For_Download { get; set; } 
    }
}
