#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Extension metadata module contains geo-spatial coordinate information, such
    /// as map coverage and points or areas of interest </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("geoSpatial")]
    public class BriefItem_GeoSpatial 
    {
        /// <summary> Constructor for a new instance of the BriefItem_GeoSpatial class </summary>
        public BriefItem_GeoSpatial()
        {
            // do nothing
        }
        
        /// <summary> Gets flag indicating if this object contains any coordinate information </summary>
        /// <value>TRUE if any data exists, otherwise FALSE </value>
        [XmlIgnore]
        public bool hasData
        {
            get {
                return ((Points != null) && (Points.Count != 0)) || ((Polygons != null) && (Polygons.Count != 0)) || ((Lines != null) && (Lines.Count != 0)) || (!String.IsNullOrEmpty(KML_Reference));
            }
        }

        /// <summary> Reference to an external KML file to use for the coordinates </summary>
        [DataMember(EmitDefaultValue = false, Name = "kml")]
        [XmlElement("kml")]
        [ProtoMember(1)]
        public string KML_Reference { get; set; }

        /// <summary> Collection of points associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false, Name = "points")]
        [XmlArray("points")]
        [XmlArrayItem("point", typeof(BriefItem_Coordinate_Point))]
        [ProtoMember(2)]
        public List<BriefItem_Coordinate_Point> Points { get; set; }

        /// <summary> Number of coordinate points associated with this digital resource </summary>
        [XmlIgnore]
        public int Point_Count
        {
            get { return Points != null ? Points.Count : 0; }
        }

        /// <summary> Collection of lines associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false, Name = "lines")]
        [XmlArray("lines")]
        [XmlArrayItem("line", typeof(BriefItem_Coordinate_Line))]
        [ProtoMember(3)]
        public List<BriefItem_Coordinate_Line> Lines { get; set; }

        /// <summary> Number of lines associated with this digital resource </summary>
        [XmlIgnore]
        public int Line_Count
        {
            get { return Lines != null ? Lines.Count : 0; }
        }

        /// <summary> Collection of polygons associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false, Name = "circles")]
        [XmlArray("circles")]
        [XmlArrayItem("circle", typeof(BriefItem_Coordinate_Circle))]
        [ProtoMember(4)]
        public List<BriefItem_Coordinate_Circle> Circles { get; set; }

        /// <summary> Number of polygons associated with this digital resource </summary>
        [XmlIgnore]
        public int Circle_Count
        {
            get { return Circles != null ? Circles.Count : 0; }
        }

        /// <summary> Collection of polygons associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false, Name = "polygons")]
        [XmlArray("polygons")]
        [XmlArrayItem("polygon", typeof(BriefItem_Coordinate_Polygon))]
        [ProtoMember(5)]
        public List<BriefItem_Coordinate_Polygon> Polygons { get; set; }

        /// <summary> Number of polygons associated with this digital resource </summary>
        [XmlIgnore]
        public int Polygon_Count
        {
            get { return Polygons != null ? Polygons.Count : 0; }
        }
    }
}