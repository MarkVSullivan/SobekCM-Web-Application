using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Skins
{
    /// <summary> Source file information for a single language, held by the <see cref="Complete_Web_Skin_Object"/> and
    /// used to build the language-specific <see cref="Web_Skin_Object" /> </summary>
    public class Complete_Web_Skin_Source_Files
    {
        /// <summary> Source file for the banner to use, if this is set to override the banner </summary>
        [DataMember(EmitDefaultValue = false, Name = "banner")]
        [ProtoMember(1)]
        public string Banner { get; set; }

        /// <summary> Source file for the standard header, to be included when rendering an HTML page  </summary>
        [DataMember(Name = "header")]
        [ProtoMember(2)]
        public string Header_Source_File { get; set; }

        /// <summary> Source file for the standard footer, to be included when rendering an HTML page  </summary>
        [DataMember(Name = "footer")]
        [ProtoMember(3)]
        public string Footer_Source_File { get; set; }

        /// <summary> Source file for the item-specific header, to be included when rendering an HTML page from the item viewer  </summary>
        [DataMember(EmitDefaultValue = false, Name = "itemHeader")]
        [ProtoMember(4)]
        public string Header_Item_Source_File { get; set; }

        /// <summary> Source file for the item-specific footer, to be included when rendering an HTML page from the item viewer  </summary>
        [DataMember(EmitDefaultValue = false, Name = "itemFooter")]
        [ProtoMember(5)]
        public string Footer_Item_Source_File { get; set; }
    }
}
