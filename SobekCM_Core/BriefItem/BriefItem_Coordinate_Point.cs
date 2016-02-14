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
    [XmlRoot("coordinatePoint")]
    public class BriefItem_Coordinate_Point
    {

        #region Constructors

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Point class </summary>
        /// <remarks> The object created using this constructor is completely undefined </remarks>
        public BriefItem_Coordinate_Point()
        {
            // do nothing
        }

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        public BriefItem_Coordinate_Point(double Latitude, double Longitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        public BriefItem_Coordinate_Point(double Latitude, double Longitude, string Label)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Label = Label;
        }

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <param name="Altitude"> Altitude for this point on a 3-dimensional plane (in meters)</param>
        public BriefItem_Coordinate_Point(double Latitude, double Longitude, string Label, long Altitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Altitude = Altitude;
            this.Label = Label;
        }

        /// <summary> Constructor for a new instance of the BriefItem_Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <param name="FeatureType"> Altitude for this point on a 3-dimensional plane (in meters)</param>
        public BriefItem_Coordinate_Point(double Latitude, double Longitude, string Label, string FeatureType)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.FeatureType = FeatureType;
            this.Label = Label;
        }

        #endregion

        /// <summary> Latitude (expressed in decimal notation) for this point </summary>
        [DataMember(EmitDefaultValue = false, Name = "latitude")]
        [XmlAttribute("latitude")]
        [ProtoMember(1)]
        public double Latitude { get; set; }

        /// <summary> Longitude (expressed in decimal notation) for this point </summary>
        [DataMember(EmitDefaultValue = false, Name = "longitude")]
        [XmlAttribute("longitude")]
        [ProtoMember(2)]
        public double Longitude { get; set; }

        /// <summary> Altitude for this point on a 3-dimensional plane (in meters) </summary>
        [DataMember(EmitDefaultValue = false, Name = "altitude")]
        [XmlAttribute("altitude")]
        [ProtoMember(3)]
        public long Altitude { get; set; }

        /// <summary> Label associated with this point  </summary>
        [DataMember(EmitDefaultValue = false, Name = "label")]
        [XmlAttribute("label")]
        [ProtoMember(4)]
        public string Label { get; set; }

        /// <summary> FeatureType associated with this point  </summary>
        [DataMember(EmitDefaultValue = false, Name = "feature")]
        [XmlAttribute("feature")]
        [ProtoMember(5)]
        public string FeatureType { get; set; }
    }
}