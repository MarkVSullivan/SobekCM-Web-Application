#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Geographic information in the form of a line comprised of two or more coordinate points </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("coordinateLine")]
    public class BriefItem_Coordinate_Line
    {
        /// <summary> Constructor for a new instance of this BriefItem_Coordinate_Line class </summary>
        public BriefItem_Coordinate_Line()
        {
            Points = new List<BriefItem_Coordinate_Point>();
        }

        /// <summary> Collection of all the points for this line </summary>
        [DataMember(EmitDefaultValue = false, Name = "points")]
        [XmlArray("points")]
        [XmlArrayItem("point", typeof(BriefItem_Coordinate_Point))]
        [ProtoMember(1)]
        public List<BriefItem_Coordinate_Point> Points { get; set; }

        /// <summary> Number of points within this line </summary>
        [XmlIgnore]
        public int Point_Count
        {
            get { return Points.Count; }
        }

        /// <summary> Label for this line within the digital resource</summary>
        [DataMember(EmitDefaultValue = false, Name = "label")]
        [XmlAttribute("label")]
        [ProtoMember(2)]
        public string Label { get; set; }

        /// <summary> FeatureType associated with this point  </summary>
        [DataMember(EmitDefaultValue = false, Name = "feature")]
        [XmlAttribute("feature")]
        [ProtoMember(3)]
        public string FeatureType { get; set; }
    }
}