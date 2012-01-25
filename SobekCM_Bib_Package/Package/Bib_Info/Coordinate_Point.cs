using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> A single point in the coordinate grid, with latitude and longitude and optionally altitude </summary>
    [Serializable]
    public class Coordinate_Point
    {
        private double latitude;
        private double longitude;
        private long altitude;
        private string label;
        private int order_temp;

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
           
            
        /// <summary> Writes this single coordinate point as SobekCM-formatted XML </summary>
        /// <param name="order"> Order in which this point appears within a larger object (such as a line or polygon) </param>
        /// <param name="sobekcm_namespace"> Namespace to use for the SobekCM custom schema ( usually 'sobekcm' )</param>
        /// <param name="results"> Stream to write this single coordinate point as SobekCM-formatted XML</param>
        internal void Add_SobekCM_Metadata(int order, string sobekcm_namespace, System.IO.TextWriter results)
        {
            results.Write( "<" + sobekcm_namespace + ":Point ");
            results.Write("latitude=\"" + latitude + "\" ");
            results.Write("longitude=\"" + longitude + "\" ");
            if (altitude != 0 )
            {
                results.Write("altitude=\"" + altitude.ToString() + "\" ");
            }
            if (order > 0)
            {
                results.Write("order=\"" + order + "\" ");
            }
            if ( !String.IsNullOrEmpty(label))
            {
                results.Write("label=\"" + label + "\"");
            }
            results.Write(" />\r\n");
        }
    }
}
