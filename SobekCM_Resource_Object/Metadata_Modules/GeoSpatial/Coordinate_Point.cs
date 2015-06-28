#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.GeoSpatial
{
    /// <summary> A single point in the coordinate grid, with latitude and longitude and optionally altitude </summary>
    [Serializable]
    public class Coordinate_Point
    {
        private long altitude;
        private string label;
        private double latitude;
        private double longitude;
        private int order_temp;
        private string featureType;

        #region Constructors

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <remarks> The object created using this constructor is completely undefined </remarks>
        public Coordinate_Point()
        {
            order_temp = -1;
        }

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        public Coordinate_Point(double Latitude, double Longitude)
        {
            latitude = Latitude;
            longitude = Longitude;
            order_temp = -1;
        }

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        public Coordinate_Point(double Latitude, double Longitude, string Label)
        {
            latitude = Latitude;
            longitude = Longitude;
            label = Label;
            order_temp = -1;
        }

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <param name="Altitude"> Altitude for this point on a 3-dimensional plane (in meters)</param>
        public Coordinate_Point(double Latitude, double Longitude, string Label, long Altitude)
        {
            latitude = Latitude;
            longitude = Longitude;
            altitude = Altitude;
            label = Label;
            order_temp = -1;
        }

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <param name="FeatureType"> Altitude for this point on a 3-dimensional plane (in meters)</param>
        public Coordinate_Point(double Latitude, double Longitude, string Label, string FeatureType)
        {
            latitude = Latitude;
            longitude = Longitude;
            featureType = FeatureType;
            label = Label;
            order_temp = -1;
        }

        #endregion

        /// <summary> Order from the XML read </summary>
        /// <remarks> This is used to initially order these points.  Once the point is used within another object (such as a polygon or line) 
        /// the order should be inherent in that collection of points. </remarks>
        internal int Order_From_XML_Read
        {
            get { return order_temp; }
            set { order_temp = value; }
        }

        /// <summary> Latitude (expressed in decimal notation) for this point </summary>
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        /// <summary> Longitude (expressed in decimal notation) for this point </summary>
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        /// <summary> Altitude for this point on a 3-dimensional plane (in meters) </summary>
        public long Altitude
        {
            get { return altitude; }
            set { altitude = value; }
        }

        /// <summary> Label associated with this point  </summary>
        public string Label
        {
            get { return label ?? String.Empty; }
            set { label = value; }
        }

        /// <summary> FeatureType associated with this point  </summary>
        public string FeatureType
        {
            get { return featureType ?? String.Empty; }
            set { featureType = value; }
        }
    }
}