#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.WebContent
{
    /// <summary> Basic information, from the database, for a web content page </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webContentPageBasic")]
    public class WebContent_Basic_Info
    {
        /// <summary> Primary key for this web content page, from the database </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [XmlIgnore]
        [ProtoMember(1)]
        public int? WebContentID { get; set; }

        /// <summary> For XML serialization, primary key for this web content page, from the database </summary>
        [IgnoreDataMember]
        [XmlAttribute("id")]
        public int WebContentID_XML
        {
            get { return WebContentID.Value; }
            set { WebContentID = value; }
        }

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
        [XmlIgnore]
        [ProtoMember(4)]
        public bool? Deleted { get; set; }

        /// <summary> For XML serialization, flag indicates if this web content page is currently 
        /// deleted within the database </summary>
        [IgnoreDataMember]
        [XmlAttribute("deleted")]
        public bool Deleted_XML
        {
            get { return Deleted.Value; }
            set { Deleted = value; }
        }

        /// <summary> URL to which a request for this page should be redirected </summary>
        [DataMember(EmitDefaultValue = false, Name = "redirect")]
        [XmlAttribute("redirect")]
        [ProtoMember(5)]
        public string Redirect { get; set; }

        /// <summary> Level 1 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level1")]
        [XmlAttribute("level1")]
        [ProtoMember(6)]
        public string Level1 { get; set; }

        /// <summary> Level 2 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level2")]
        [XmlAttribute("level2")]
        [ProtoMember(7)]
        public string Level2 { get; set; }

        /// <summary> Level 3 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level3")]
        [XmlAttribute("level3")]
        [ProtoMember(8)]
        public string Level3 { get; set; }

        /// <summary> Level 4 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level4")]
        [XmlAttribute("level4")]
        [ProtoMember(9)]
        public string Level4 { get; set; }

        /// <summary> Level 5 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level5")]
        [XmlAttribute("level5")]
        [ProtoMember(10)]
        public string Level5 { get; set; }

        /// <summary> Level 6 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level6")]
        [XmlAttribute("level6")]
        [ProtoMember(11)]
        public string Level6 { get; set; }

        /// <summary> Level 7 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level7")]
        [XmlAttribute("level7")]
        [ProtoMember(12)]
        public string Level7 { get; set; }

        /// <summary> Level 8 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level8")]
        [XmlAttribute("level8")]
        [ProtoMember(13)]
        public string Level8 { get; set; }


        #region Methods to control XML serialization

        /// <summary> Method suppresses XML Serialization of the WebContentID_XML property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeWebContentID_XML()
        {
            return WebContentID.HasValue;
        }

        /// <summary> Method suppresses XML Serialization of the Deleted_XML property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeDeleted_XML()
        {
            return Deleted.HasValue && Deleted.Value;
        }

        #endregion

        /// <summary> Constructor for a new instance of the Web_Content_Basic_Info class </summary>
        public WebContent_Basic_Info()
        {
            // Empty constructor for deserialization purposes
        }

        /// <summary> Constructor for a new instance of the Web_Content_Basic_Info class </summary>
        /// <param name="WebContentID"> Primary key for this web content page, from the database </param>
        /// <param name="Title"> Title for this web content page, from the database </param>
        /// <param name="Summary"> Summary for this web content page, from the database </param>
        /// <param name="Deleted"> Flag indicates if this web content page is currently deleted within the database </param>
        /// <param name="Redirect"> URL to which a request for this page should be redirected </param>
        public WebContent_Basic_Info(int WebContentID, string Title, string Summary, bool Deleted, string Redirect )
        {
            this.WebContentID = WebContentID;
            this.Title = Title;
            this.Summary = Summary;
            this.Deleted = Deleted;
            this.Redirect = Redirect;
        }
    }
}
