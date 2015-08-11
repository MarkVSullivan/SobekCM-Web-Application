#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.WebContent.Admin
{
    /// <summary> Represents a recently changed web page, the user who made the changes, and the date 
    /// and type of the change  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webContentUsage")]
    public class WebContent_Recent_Changed_Page
    {
        /// <summary> Date this web page was changed (milestone entry was added) </summary>
        [DataMember(Name = "date")]
        [XmlAttribute("date")]
        [ProtoMember(1)]
        public DateTime MilestoneDate { get; set; }

        /// <summary> User associated with this change / milestone </summary>
        [DataMember(EmitDefaultValue = false, Name = "user")]
        [XmlAttribute("user")]
        [ProtoMember(2)]
        public string User { get; set; }

        /// <summary> Notes (and milestone type) for this miletsone entry </summary>
        [DataMember(Name = "notes")]
        [XmlText]
        [ProtoMember(3)]
        public string Notes;

        /// <summary> Level 1 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level1")]
        [XmlAttribute("level1")]
        [ProtoMember(4)]
        public string Level1 { get; set; }

        /// <summary> Level 2 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level2")]
        [XmlAttribute("level2")]
        [ProtoMember(5)]
        public string Level2 { get; set; }

        /// <summary> Level 3 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level3")]
        [XmlAttribute("level3")]
        [ProtoMember(6)]
        public string Level3 { get; set; }

        /// <summary> Level 4 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level4")]
        [XmlAttribute("level4")]
        [ProtoMember(7)]
        public string Level4 { get; set; }

        /// <summary> Level 5 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level5")]
        [XmlAttribute("level5")]
        [ProtoMember(8)]
        public string Level5 { get; set; }

        /// <summary> Level 6 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level6")]
        [XmlAttribute("level6")]
        [ProtoMember(9)]
        public string Level6 { get; set; }

        /// <summary> Level 7 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level7")]
        [XmlAttribute("level7")]
        [ProtoMember(10)]
        public string Level7 { get; set; }

        /// <summary> Level 8 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level8")]
        [XmlAttribute("level8")]
        [ProtoMember(11)]
        public string Level8 { get; set; }

        /// <summary> Title of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(12)]
        public string Title { get; set; }

        /// <summary> Primary key of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(13)]
        public int WebContentID { get; set; }


    }
    
}
