using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Extensions
{
    /// <summary> Enumeration indicates when an extensions CSS file should be included
    /// in the HTML output </summary>
    public enum ExtensionCssInfoConditionEnum : byte
    {
        /// <summary> Include this CSS for administrative pages </summary>
        Admin,

        /// <summary> Include this CSS for aggregation pages </summary>
        Aggregation,

        /// <summary> Always include this CSS </summary>
        Always,

        /// <summary> Include this CSS for item (i.e., digital resources) pages </summary>
        Item,

        /// <summary> Include this CSS for metadata editing and entering pages </summary>
        Metadata,

        /// <summary> Include this CSS for mySobek pages </summary>
        MySobek,

        /// <summary> Include this CSS for results pages </summary>
        Results,

        /// <summary> An error occured when determining when this CSS file should be included </summary>
        ERROR
    }

    /// <summary> Contains information about an included CSS file for an extension, including 
    /// the URL and when to include the CSS URL </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionCssInfo")]
    public class ExtensionCssInfo
    {
        /// <summary> Condition upon which this CSS file should be added </summary>
        [DataMember(Name = "condition")]
        [XmlAttribute("condition")]
        [ProtoMember(1)]
        public ExtensionCssInfoConditionEnum Condition { get; set; }

        /// <summary> URL for this CSS </summary>
        [DataMember(Name = "url")]
        [XmlText]
        [ProtoMember(2)]
        public string URL { get; set; }

        /// <summary> Consructor for a new instance of the <see cref="ExtensionCssInfo"/> class </summary>
        public ExtensionCssInfo()
        {
            Condition = ExtensionCssInfoConditionEnum.ERROR;
        }

        /// <summary> Consructor for a new instance of the <see cref="ExtensionCssInfo"/> class </summary>
        /// <param name="URL"> URL for this CSS </param>
        /// <param name="Condition"> Condition upon which this CSS file should be added </param>
        public ExtensionCssInfo(string URL, ExtensionCssInfoConditionEnum Condition)
        {
            this.URL = URL;
            this.Condition = Condition;
        }
    }
}
