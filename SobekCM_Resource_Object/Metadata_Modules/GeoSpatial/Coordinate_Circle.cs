#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.GeoSpatial
{
    /// <summary> A single point in the coordinate grid, with latitude and longitude and optionally altitude </summary>
    [Serializable]
    public class Coordinate_Circle
    {
        private double latitude;
        private double longitude;
        private string label;
        private double radius;
        private string featureType;
        private int order_temp;

        #region Constructors

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <remarks> The object created using this constructor is completely undefined </remarks>
        public Coordinate_Circle()
        {
            order_temp = -1;
        }

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this circle </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this circle </param>
        /// <param name="Radius"> Radius (expressed in decimal notation) for this circle</param>
        public Coordinate_Circle(double Latitude, double Longitude, double Radius)
        {
            latitude = Latitude;
            longitude = Longitude;
            radius = Radius;
            order_temp = -1;
        }

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this circle </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this circle </param>
        /// <param name="Radius"> Radius (expressed in decimal notation) for this circle</param>
        /// <param name="Label"> Label to associate with this circle </param>
        public Coordinate_Circle(double Latitude, double Longitude, double Radius, string Label)
        {
            latitude = Latitude;
            longitude = Longitude;
            radius = Radius;
            label = Label;
            order_temp = -1;
        }

        /// <summary> Constructor for a new instance of the Coordinate_Point class </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this circle </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this circle </param>
        /// <param name="Radius"> Radius (expressed in decimal notation) for this circle</param>
        /// <param name="Label"> Label to associate with this circle </param>
        /// <param name="FeatureType"> Altitude for this circle on a 3-dimensional plane (in meters)</param>
        public Coordinate_Circle(double Latitude, double Longitude, double Radius, string Label, string FeatureType)
        {
            latitude = Latitude;
            longitude = Longitude;
            radius = Radius;
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

        /// <summary> Latitude (expressed in decimal notation) for this circle </summary>
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        /// <summary> Longitude (expressed in decimal notation) for this circle </summary>
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        /// <summary> Altitude for this circle on a 3-dimensional plane (in meters) </summary>
        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        /// <summary> Label associated with this circle  </summary>
        public string Label
        {
            get { return label ?? String.Empty; }
            set { label = value; }
        }

        /// <summary> FeatureType associated with this circle  </summary>
        public string FeatureType
        {
            get { return featureType ?? String.Empty; }
            set { featureType = value; }
        }

        /// <summary> Writes this single coordinate point as SobekCM-formatted XML </summary>
        /// <param name="order"> Order in which this circle appears within a larger object (such as a line or polygon) </param>
        /// <param name="sobekcm_namespace"> Namespace to use for the SobekCM custom schema ( usually 'sobekcm' )</param>
        /// <param name="results"> Stream to write this single coordinate point as SobekCM-formatted XML</param>
        internal void Add_SobekCM_Metadata(int order, string sobekcm_namespace, TextWriter results)
        {
            results.Write("<" + sobekcm_namespace + ":Circle ");
            results.Write("latitude=\"" + latitude + "\" ");
            results.Write("longitude=\"" + longitude + "\" ");
            if (radius > 0)
            {
                results.Write("radius=\"" + radius + "\" ");
            }
            if (order > 0)
            {
                results.Write("order=\"" + order + "\" ");
            }
            if (!String.IsNullOrEmpty(label))
            {
                results.Write("label=\"" + label + "\"");
            }
            if (!String.IsNullOrEmpty(featureType))
            {
                results.Write("featureType=\"" + featureType + "\"");
            }
            results.Write(" />\r\n");
        }
    }
}