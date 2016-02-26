using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Viewers
{
    /// <summary> Configuration information for the special web content HTML writer, including
    /// the viewers configuration </summary>
    /// <remarks> Due to the simplicity of this (and the non-core functionality) no viewer configuration
    /// information is included in this configuration. </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("WebContentWriterConfig")]
    public class WebContentWriterConfig
    {
        /// <summary> Fully qualified (including namespace) name of the main class used
        /// as the web content HTML writer </summary>
        /// <remarks> By default, this would be 'SobekCM.Library.HTML.Web_Content_HtmlSubwriter' </remarks>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(1)]
        public string Class { get; set; }

        /// <summary> Name of the assembly within which this class resides, unless this
        /// is one of the default subviewers included in the core code </summary>
        /// <remarks> By default, this would be blank </remarks>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(2)]
        public string Assembly { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="WebContentWriterConfig"/> class </summary>
        public WebContentWriterConfig()
        {
            Class = "SobekCM.Library.HTML.Web_Content_HtmlSubwriter";
        }


        /// <summary> Clears all the previously loaded information, such as the default values </summary>
        /// <remarks> This clears the assembly, and sets the class to the default web content html subwriter class. </remarks>
        public void Clear()
        {
            Assembly = String.Empty;
            Class = "SobekCM.Library.HTML.Web_Content_HtmlSubwriter";
        }
    }
}
