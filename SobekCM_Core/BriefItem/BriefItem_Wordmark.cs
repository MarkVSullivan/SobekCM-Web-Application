using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> A single wordmark/icon to display with this item in the viewer </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("workdmark")]
    public class BriefItem_Wordmark
    {
        /// <summary> Constructor for a new instance of the BriefItem_Wordmark class </summary>
        public BriefItem_Wordmark()
        {
            // Do nothing
        }


        /// <summary> Code for this wordmark, which links back to the collection of wordmarks </summary>
        [DataMember(EmitDefaultValue = false, Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary> Title for this wordmark </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(2)]
        public string Title { get; set; }

        /// <summary> Html for this wordmark </summary>
        [DataMember(EmitDefaultValue = false, Name = "html")]
        [XmlAttribute("html")]
        [ProtoMember(3)]
        public string HTML { get; set; }

        /// <summary> Link for this wordmark </summary>
        [DataMember(EmitDefaultValue = false, Name = "uri")]
        [XmlAttribute("uri")]
        [ProtoMember(4)]
        public string Link { get; set; }

        #region IEquatable<Wordmark_Info> Members

        /// <summary> Checks to see if this wordmark/icon is equal to another wordmark/icon </summary>
        /// <param name="Other"> Other wordmark/icon to verify equality with </param>
        /// <returns> TRUE if they equal, otherwise FALSE</returns>
        /// <remarks> Two wordmark/icons are considered equal if their codes are identical </remarks>
        public bool Equals(BriefItem_Wordmark Other)
        {
            return String.Compare(Other.Code, Code, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion
    }
}
