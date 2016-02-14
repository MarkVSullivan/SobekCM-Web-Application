#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> A single point in the coordinate grid, with latitude and longitude and optionally altitude </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("coordinateCircle")]
    public class BriefItem_Coordinate_Circle
    {
        
        #region Constructors

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Circle class </summary>
        /// <remarks> The object created using this constructor is completely undefined </remarks>
        public BriefItem_Coordinate_Circle()
        {
            // do nothing
        }

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Circle class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this circle </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this circle </param>
        /// <param name="Radius"> Radius (expressed in decimal notation) for this circle</param>
        public BriefItem_Coordinate_Circle(double Latitude, double Longitude, double Radius)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Radius = Radius;
        }

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Circle class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this circle </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this circle </param>
        /// <param name="Radius"> Radius (expressed in decimal notation) for this circle</param>
        /// <param name="Label"> Label to associate with this circle </param>
        public BriefItem_Coordinate_Circle(double Latitude, double Longitude, double Radius, string Label)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Radius = Radius;
            this.Label = Label;
        }

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Circle class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this circle </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this circle </param>
        /// <param name="Radius"> Radius (expressed in decimal notation) for this circle</param>
        /// <param name="Label"> Label to associate with this circle </param>
        /// <param name="FeatureType"> Altitude for this circle on a 3-dimensional plane (in meters)</param>
        public BriefItem_Coordinate_Circle(double Latitude, double Longitude, double Radius, string Label, string FeatureType)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Radius = Radius;
            this.FeatureType = FeatureType;
            this.Label = Label;
        }

        #endregion

        /// <summary> Latitude (expressed in decimal notation) for this circle </summary>
        [DataMember(EmitDefaultValue = false, Name = "latitude")]
        [XmlAttribute("latitude")]
        [ProtoMember(1)]
        public double Latitude { get; set; }

        /// <summary> Longitude (expressed in decimal notation) for this circle </summary>
        [DataMember(EmitDefaultValue = false, Name = "longitude")]
        [XmlAttribute("longitude")]
        [ProtoMember(2)]
        public double Longitude { get; set; }

        /// <summary> Altitude for this circle on a 3-dimensional plane (in meters) </summary>
        [DataMember(EmitDefaultValue = false, Name = "radius")]
        [XmlAttribute("radius")]
        [ProtoMember(3)]
        public double Radius { get; set; }

        /// <summary> Label associated with this circle  </summary>
        [DataMember(EmitDefaultValue = false, Name = "label")]
        [XmlAttribute("label")]
        [ProtoMember(4)]
        public string Label { get; set; }

        /// <summary> FeatureType associated with this circle  </summary>
        [DataMember(EmitDefaultValue = false, Name = "feature")]
        [XmlAttribute("feature")]
        [ProtoMember(5)]
        public string FeatureType { get; set; }
    }
}