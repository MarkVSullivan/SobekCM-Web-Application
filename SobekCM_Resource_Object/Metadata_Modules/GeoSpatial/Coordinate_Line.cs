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
        public string featureType;


        /// <summary> Constructor for a new instance of this Coordinate_Line class </summary>
        public Coordinate_Line()
        {
            points = new List<Coordinate_Point>();
            featureType = "main"; //default
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

        /// <summary> Add Feature Type Data</summary>
        public void Add_FeatureType(string incomingFeatureType)
        {
            featureType = incomingFeatureType;
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

        /// <summary> Writes this line of coordinates as SobekCM-formatted XML </summary>
        /// <param name="sobekcm_namespace"> Namespace to use for the SobekCM custom schema ( usually 'sobekcm' )</param>
        /// <param name="results"> Stream to write this line of coordiantes as SobekCM-formatted XML</param>
        internal void Add_SobekCM_Metadata(string sobekcm_namespace, TextWriter results)
        {
            // If no points, return nothing
            if (points.Count == 0)
                return;

            // Step through all the points in this line
            results.Write("<" + sobekcm_namespace + ":Line");
            if (!String.IsNullOrEmpty(label))
                results.Write(" label=\"" + label + "\"");
            results.Write(">\r\n");
            foreach (Coordinate_Point thisPoint in points)
            {
                thisPoint.Add_SobekCM_Metadata(-1, sobekcm_namespace, results);
            }
            results.Write("</" + sobekcm_namespace + ":Line>\r\n");
        }
    }
}