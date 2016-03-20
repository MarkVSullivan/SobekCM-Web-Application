using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Items
{
    /// <summary> Information about how a single item relates to the larger title </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("item")]
    public class Item_Hierarchy_Details
    {
        /// <summary> Primary key for this item from the SObekCM database </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int ItemID { get; set; }

        /// <summary> Title for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(2)]
        public string Title { get; set; }

        /// <summary> Text value for the level 1 hierarchy for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level1_text")]
        [XmlAttribute("level1_text")]
        [ProtoMember(3)]
        public string Level1_Text { get; set; }

        /// <summary> Index for the level 1 hierarchy index for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level1_index")]
        [XmlIgnore]
        [ProtoMember(4)]
        public int? Level1_Index { get; set; }

        /// <summary> For XML serialization, index for the level 1 hierarchy index for this item within the larger title</summary>
        [IgnoreDataMember]
        [XmlAttribute("level1_index")]
        public int Level1_Index_XML
        {
            get { return Level1_Index.Value; } 
            set { Level1_Index = value; }
        }

        /// <summary> Text value for the level 2 hierarchy for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level2_text")]
        [XmlAttribute("level2_text")]
        [ProtoMember(5)]
        public string Level2_Text { get; set; }

        /// <summary> Index for the level 2 hierarchy index for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level2_index")]
        [XmlIgnore]
        [ProtoMember(6)]
        public int? Level2_Index { get; set; }

        /// <summary> For XML serialization, index for the level 2 hierarchy index for this item within the larger title</summary>
        [IgnoreDataMember]
        [XmlAttribute("level2_index")]
        public int Level2_Index_XML
        {
            get { return Level2_Index.Value; }
            set { Level2_Index = value; }
        }

        /// <summary> Text value for the level 3 hierarchy for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level3_text")]
        [XmlAttribute("level3_text")]
        [ProtoMember(7)]
        public string Level3_Text { get; set; }

        /// <summary> Index for the level 3 hierarchy index for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level3_index")]
        [XmlIgnore]
        [ProtoMember(8)]
        public int? Level3_Index { get; set; }

        /// <summary> For XML serialization, index for the level 3 hierarchy index for this item within the larger title</summary>
        [IgnoreDataMember]
        [XmlAttribute("level3_index")]
        public int Level3_Index_XML
        {
            get { return Level3_Index.Value; }
            set { Level3_Index = value; }
        }

        /// <summary> Text value for the level 4 hierarchy for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level4_text")]
        [XmlAttribute("level4_text")]
        [ProtoMember(9)]
        public string Level4_Text { get; set; }

        /// <summary> Index for the level 4 hierarchy index for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level4_index")]
        [XmlIgnore]
        [ProtoMember(10)]
        public int? Level4_Index { get; set; }

        /// <summary> For XML serialization, index for the level 4 hierarchy index for this item within the larger title</summary>
        [IgnoreDataMember]
        [XmlAttribute("level4_index")]
        public int Level4_Index_XML
        {
            get { return Level4_Index.Value; }
            set { Level4_Index = value; }
        }

        /// <summary> Text value for the level 5 hierarchy for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level5_text")]
        [XmlAttribute("level5_text")]
        [ProtoMember(11)]
        public string Level5_Text { get; set; }

        /// <summary> Index for the level 5 hierarchy index for this item within the larger title </summary>
        [DataMember(EmitDefaultValue = false, Name = "level5_index")]
        [XmlIgnore]
        [ProtoMember(12)]
        public int? Level5_Index { get; set; }

        /// <summary> For XML serialization, index for the level 5 hierarchy index for this item within the larger title</summary>
        [IgnoreDataMember]
        [XmlAttribute("level5_index")]
        public int Level5_Index_XML
        {
            get { return Level5_Index.Value; }
            set { Level5_Index = value; }
        }

        /// <summary> Main thumbnail for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "thumbnail")]
        [XmlAttribute("thumbnail")]
        [ProtoMember(13)]
        public string MainThumbnail { get; set; }

        /// <summary> Volume identifier for this item </summary>
        [DataMember(Name = "vid")]
        [XmlAttribute("vid")]
        [ProtoMember(14)]
        public string VID { get; set; }

        /// <summary> IP restriction mask </summary>
        /// <remarks> If this item is PUBLIC, this will be 0.  If it is PRIVATE, it will be -1.  Otherwise, a positive value means it is restricted to certain IP addresses </remarks>
        [DataMember(Name = "restriction")]
        [XmlAttribute("restriction")]
        [ProtoMember(15)]
        public short IP_Restriction_Mask { get; set; }

        /// <summary> Flag indicates if this item is DARK </summary>
        [DataMember(EmitDefaultValue = false, Name = "dark")]
        [XmlAttribute("dark")]
        [ProtoMember(16)]
        public bool Dark { get; set; }


        #region Methods to control XML serialization

        /// <summary> Method suppresses XML Serialization of the Level1_Index_XML property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeLevel1_Index_XML()
        {
            return Level1_Index.HasValue && Level1_Index.Value > 0;
        }

        /// <summary> Method suppresses XML Serialization of the Level2_Index_XML property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeLevel2_Index_XML()
        {
            return Level2_Index.HasValue && Level2_Index.Value > 0;
        }

        /// <summary> Method suppresses XML Serialization of the Level3_Index_XML property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeLevel3_Index_XML()
        {
            return Level3_Index.HasValue && Level3_Index.Value > 0;
        }

        /// <summary> Method suppresses XML Serialization of the Level4_Index_XML property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeLevel4_Index_XML()
        {
            return Level4_Index.HasValue && Level4_Index.Value > 0;
        }

        /// <summary> Method suppresses XML Serialization of the Level5_Index_XML property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeLevel5_Index_XML()
        {
            return Level5_Index.HasValue && Level5_Index.Value > 0;
        }

        #endregion
    

    }

}
