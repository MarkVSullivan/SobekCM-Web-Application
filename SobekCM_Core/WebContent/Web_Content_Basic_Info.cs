using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent
{
    /// <summary> Basic information, from the database, for a web content page </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webContentPageBasic")]
    public class Web_Content_Basic_Info
    {
        /// <summary> Primary key for this web content page, from the database </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int? WebContentID { get; set; }

        /// <summary> Title for this web content page, from the database </summary>
        /// <remarks> Generally, this is used for auto-creating sitemaps and reporting purposes </remarks>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(2)]
        public string Title { get; set; }

        /// <summary> Summary for this web content page, from the database </summary>
        /// <remarks> Generally, this is used for auto-creating sitemaps </remarks>
        [DataMember(EmitDefaultValue = false, Name = "summary")]
        [XmlAttribute("summary")]
        [ProtoMember(3)]
        public string Summary { get; set; }

        /// <summary> Flag indicates if this web content page is currently deleted within the database </summary>
        [DataMember(EmitDefaultValue = false, Name = "deleted")]
        [XmlAttribute("deleted")]
        [ProtoMember(4)]
        public bool? Deleted { get; set; }

        #region Methods to control XML serialization

        /// <summary> Method suppresses XML Serialization of the Footer_Has_Container_Directive property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeWebContentID()
        {
            return WebContentID.HasValue;
        }

        /// <summary> Method suppresses XML Serialization of the Footer_Has_Container_Directive property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeDeleted()
        {
            return Deleted.HasValue;
        }

        #endregion

        /// <summary> Constructor for a new instance of the Web_Content_Basic_Info class </summary>
        public Web_Content_Basic_Info()
        {
            // Empty constructor for deserialization purposes
        }

        /// <summary> Constructor for a new instance of the Web_Content_Basic_Info class </summary>
        /// <param name="WebContentID"> Primary key for this web content page, from the database </param>
        /// <param name="Title"> Title for this web content page, from the database </param>
        /// <param name="Summary"> Summary for this web content page, from the database </param>
        /// <param name="Deleted"> Flag indicates if this web content page is currently deleted within the database </param>
        public Web_Content_Basic_Info(int WebContentID, string Title, string Summary, bool Deleted)
        {
            this.WebContentID = WebContentID;
            this.Title = Title;
            this.Summary = Summary;
            this.Deleted = Deleted;
        }
    }
}
