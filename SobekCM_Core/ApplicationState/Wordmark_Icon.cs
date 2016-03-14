#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.ApplicationState
{
    /// <summary> Lookup information about a single wordmark/icon including image, link, and title </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("wordmark")]
    public class Wordmark_Icon
    {
        /// <summary> Metadata code for this wordmark/icon</summary>
        [DataMember(EmitDefaultValue = false, Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary> Name of the image file for this wordmark/icon</summary>
        [DataMember(EmitDefaultValue = false, Name = "image")]
        [XmlAttribute("image")]
        [ProtoMember(2)]
        public string Image_FileName { get; set; }

        /// <summary> Link related to this wordmark/icon</summary>
        [DataMember(EmitDefaultValue = false, Name = "link")]
        [XmlAttribute("link")]
        [ProtoMember(3)]
        public string Link { get; set; }

        /// <summary> Title for this wordmark/icon which appears when you hover over the image</summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(4)]
        public string Title { get; set; }

        /// <summary> Gets the HTML for this wordmark, based off the other properties </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public string HTML
        {
            get
            {
                if (Link.Length == 0)
                {
                    return "<img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + Image_FileName + "\" title=\"" + Title + "\" alt=\"" + Title + "\" />";
                }
                else
                {
                    if (Link[0] == '?')
                    {
                        return "<a href=\"" + Link + "\"><img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + Image_FileName + "\" alt=\"" + Title + "\" /></a>";
                    }
                    else
                    {
                        return "<a href=\"" + Link + "\" target=\"_blank\"><img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + Image_FileName + "\" alt=\"" + Title + "\" /></a>";
                    }
                }
            }

        }

        /// <summary> Constructor for a new instance of the Wordmark_Icon class </summary>
        /// <param name="Code"> Metadata code for this wordmark/icon</param>
        /// <param name="Image_FileName"> Name of the image file for this wordmark/icon</param>
        /// <param name="Link"> Link related to this wordmark/icon</param>
        /// <param name="Title"> Title for this wordmark/icon which appears when you hover over the image</param>
        public Wordmark_Icon(string Code, string Image_FileName, string Link, string Title)
        {
            this.Code = Code;
            this.Image_FileName = Image_FileName;
            this.Link = Link;
            this.Title = Title;
        }
    }
}
