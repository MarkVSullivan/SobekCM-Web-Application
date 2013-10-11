#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.GeoSpatial
{
    /// <summary> Geographic information in the form of a line comprised of two or more coordinate points </summary>
    [Serializable]
    public class Coordinate_Line
    {
        private string label;
        private List<Coordinate_Point> points;
        private string featureType;


        /// <summary> Constructor for a new instance of this Coordinate_Line class </summary>
        public Coordinate_Line()
        {
            points = new List<Coordinate_Point>();
        }

        /// <summary> Read-only collection of all the points for this line </summary>
        public ReadOnlyCollection<Coordinate_Point> Points
        {
            get { return new ReadOnlyCollection<Coordinate_Point>(points); }
        }

        /// <summary> Number of points within this line </summary>
        public int Point_Count
        {
            get { return points.Count; }
        }

        /// <summary> Label for this line within the digital resource</summary>
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

        /// <summary>Add a pre-existing point object to this line </summary>
        /// <param name="newPoint"> New point to add to this line </param>
        public void Add_Point(Coordinate_Point newPoint)
        {
            points.Add(newPoint);
        }

        /// <summary> Adds a new point to this line </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <returns> Fully built and added point object </returns>
        public Coordinate_Point Add_Point(double Latitude, double Longitude, string Label)
        {
            Coordinate_Point newPoint = new Coordinate_Point(Latitude, Longitude, Label);
            points.Add(newPoint);
            return newPoint;
        }

        /// <summary> Adds a new point to this line, including altitude</summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <param name="Altitude"> Altitude for this point on a 3-dimensional plane (in meters)</param>
        /// <returns>Fully built and added point object </returns>
        public Coordinate_Point Add_Point(double Latitude, double Longitude, string Label, long Altitude)
        {
            Coordinate_Point newPoint = new Coordinate_Point(Latitude, Longitude, Label, Altitude);
            points.Add(newPoint);
            return newPoint;
        }
    }
}