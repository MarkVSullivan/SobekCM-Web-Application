using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> Geographic information in the form of a polygon comprised of several coordinate points </summary>
    [Serializable]
    public class Coordinate_Polygon
    {
        private List<Coordinate_Point> edge_points;
        private List<Coordinate_Point> internal_points;
        private List<Coordinate_Point> bounding_box;
        private string label;
        private string id;
        private ushort pageSequence;

        /// <summary> Constructor for a new instance of this Coordinate_Polygon class </summary>
        public Coordinate_Polygon()
        {
            edge_points = new List<Coordinate_Point>();
            internal_points = new List<Coordinate_Point>();
            pageSequence = 0;
        }

        /// <summary> Clears all the edge points in thie polygon </summary>
        public void Clear_Edge_Points()
        {
            edge_points.Clear();
        }

        /// <summary> Checks to see if a point is in the bounding box defined by this polygon </summary>
        /// <param name="Latitude"> Latitude of the point to check </param>
        /// <param name="Longitude"> Longitude of the point to check </param>
        /// <returns> TRUE if the point is in this polygon's bounding box, otherwise false</returns>
        public bool is_In_Bounding_Box(double Latitude, double Longitude)
        {
            bool in_lat_search = false;
            bool in_long_search = false;
            ReadOnlyCollection<Coordinate_Point> boundingBox = this.Bounding_Box;
            if ((Bounding_Box[0].Latitude >= Latitude) && (boundingBox[1].Latitude <= Latitude))
            {
                in_lat_search = true;
            }

            if ((boundingBox[0].Latitude <= Latitude) && (boundingBox[1].Latitude >= Latitude))
            {
                in_lat_search = true;
            }

            if ( !in_lat_search )
                return false;

            if ((boundingBox[0].Longitude >= Longitude) && (boundingBox[1].Longitude <= Longitude))
            {
                in_long_search = true;
            }

            if ((boundingBox[0].Longitude <= Longitude) && (boundingBox[1].Longitude >= Longitude))
            {
                in_long_search = true;
            }

            if ((in_lat_search) && (in_long_search))
                return true;
            else
                return false;
        }

        /// <summary> Checks to see if a rectangle overlaps the bounding box defined by this polygon </summary>
        /// <param name="Rect_Latitude_A"> Maximum latitude of the rectangle to check </param>
        /// <param name="Rect_Longitude_A"> Maximum longitude of the rectangle to check </param>
        /// <param name="Rect_Latitude_B"> Minimum latitude of the rectangle to check </param>
        /// <param name="Rect_Longitude_B"> Minimum longitude of the rectangle to check </param>
        /// <returns> TRUE if the point is in this polygon's bounding box, otherwise false</returns>
        public bool is_In_Bounding_Box(double Rect_Latitude_A, double Rect_Longitude_A, double Rect_Latitude_B, double Rect_Longitude_B)
        {
            ReadOnlyCollection<Coordinate_Point> boundingBox = this.Bounding_Box;

            double lat1 = Math.Max(bounding_box[0].Latitude, bounding_box[1].Latitude);
            double lat2 = Math.Min(bounding_box[0].Latitude, bounding_box[1].Latitude);
            double long1 = Math.Max(bounding_box[0].Longitude, bounding_box[1].Longitude);
            double long2 = Math.Min(bounding_box[0].Longitude, bounding_box[1].Longitude);

            double test_rect_lat1 = Math.Max(Rect_Latitude_A, Rect_Latitude_B);
            double test_rect_lat2 = Math.Min(Rect_Latitude_A, Rect_Latitude_B);
            double test_rect_long1 = Math.Max(Rect_Longitude_A, Rect_Longitude_B);
            double test_rect_long2 = Math.Min(Rect_Longitude_A, Rect_Longitude_B);

            // If the left side of the test rectangle is to the right of this polygon... FALSE
            if (test_rect_long2 > long1)
                return false;

            // If the right side of the test rectangle is to the left of this polygon... FALSE
            if (test_rect_long1 < long2)
                return false;

            // If the bottom side of the test rectangle is above this polygon... FALSE
            if (test_rect_lat2 > lat1 )
                return false;

            // If the upper side of the test rectangle is to below of this polygon... FALSE
            if (test_rect_lat1 < lat2)
                return false;

            // Otherwise, there is some overlap
            return true;
        }



        /// <summary> Forces a recalculation of the bounding box for this area and returns the 
        /// new bounding box </summary>
        /// <returns>Rectangular bounding box, with the first point in the upper left corner 
        /// and the second point in the lower right corner</returns>
        /// <remarks> Generally, this routing does not need to be called since just adding new edge points to this
        /// polygon forces a recalculation of the bounding box the next time it is requested</remarks>
        public ReadOnlyCollection<Coordinate_Point> Recalculate_Bounding_Box()
        {
            bounding_box = null;
            return Bounding_Box;
        }

        /// <summary> Return the rectangular bounding box, with the first point in the upper left corner 
        /// and the second point in the lower right corner </summary>
        public ReadOnlyCollection<Coordinate_Point> Bounding_Box
        {
            get
            {
                if (bounding_box != null)
                {
                    return new ReadOnlyCollection<Coordinate_Point>(bounding_box);
                }
                else
                {
                    bounding_box = new List<Coordinate_Point>();

                    if (edge_points.Count > 0)
                    {
                        // Look for the bounding box now
                        double least_latitude = Convert.ToDouble(edge_points[0].Latitude);
                        double most_latitude = least_latitude;
                        double least_longitude = Convert.ToDouble(edge_points[0].Longitude);
                        double most_longitude = least_longitude;
                        double interior_point_latitude = least_latitude;
                        double interior_point_longitude = least_longitude;
                        double this_latitude = least_latitude;
                        double this_longitude = least_longitude;
                        foreach (Coordinate_Point thisPoint in edge_points)
                        {
                            this_latitude = Convert.ToDouble(thisPoint.Latitude);
                            this_longitude = Convert.ToDouble(thisPoint.Longitude);
                            if (this_latitude < least_latitude)
                                least_latitude = this_latitude;
                            if (this_latitude > most_latitude)
                                most_latitude = this_latitude;
                            if (this_longitude < least_longitude)
                                least_longitude = this_longitude;
                            if (this_longitude > most_longitude)
                                most_longitude = this_longitude;
                        }

                        // Return the bounding box here
                        bounding_box.Add(new Coordinate_Point(most_latitude, least_longitude, String.Empty));
                        bounding_box.Add(new Coordinate_Point(least_latitude, most_longitude, String.Empty));
                    }

                    return new ReadOnlyCollection<Coordinate_Point>(bounding_box);
                }
            }
        }

        /// <summary> Add a new edge point of interest to this polygon  </summary>
        /// <param name="newPoint"> Built coordinate point object to add </param>
        public void Add_Edge_Point(Coordinate_Point newPoint)
        {
            // Update the current bounding box, if there is one
            if (bounding_box != null)
            {
                bounding_box = null;
            }

            edge_points.Add(newPoint);
        }

        /// <summary> Add a new edge point of interest to this polygon  </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <returns> Fully built Coordinate_Point object </returns>
        public Coordinate_Point Add_Edge_Point(double Latitude, double Longitude)
        {
            // Update the current bounding box, if there is one
            if (bounding_box != null)
            {
                bounding_box = null;
            }

            Coordinate_Point newPoint = new Coordinate_Point(Latitude, Longitude, String.Empty);
            edge_points.Add(newPoint);
            return newPoint;
        }

        /// <summary> Add a new edge point of interest to this polygon  </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <returns> Fully built Coordinate_Point object </returns>
        public Coordinate_Point Add_Edge_Point(double Latitude, double Longitude, string Label)
        {
            // Update the current bounding box, if there is one
            if (bounding_box != null)
            {
                bounding_box = null;
            }

            Coordinate_Point newPoint = new Coordinate_Point(Latitude, Longitude, Label);
            edge_points.Add(newPoint);
            return newPoint;
        }

        /// <summary> Add a new inner point of interest to this polygon  </summary>
        /// <param name="newPoint"> Built coordinate point object to add </param>
        public void Add_Inner_Point(Coordinate_Point newPoint)
        {
            internal_points.Add(newPoint);
        }

        /// <summary> Add a new inner point of interest to this polygon  </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <returns> Fully built Coordinate_Point object </returns>
        public Coordinate_Point Add_Inner_Point(double Latitude, double Longitude)
        {
            Coordinate_Point newPoint = new Coordinate_Point(Latitude, Longitude, String.Empty);
            internal_points.Add(newPoint);
            return newPoint;
        }

        /// <summary> Add a new inner point of interest to this polygon  </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <returns> Fully built Coordinate_Point object </returns>
        public Coordinate_Point Add_Inner_Point(double Latitude, double Longitude, string Label)
        {
            Coordinate_Point newPoint = new Coordinate_Point(Latitude, Longitude, Label);
            internal_points.Add(newPoint);
            return newPoint;
        }

        /// <summary> Number of points which comprise the edge of this polygon </summary>
        public int Edge_Points_Count
        {
            get { return edge_points.Count; }
        }

        /// <summary> Number of internal points of interest in this polygon </summary>
        public int Inner_Points_Count
        {
            get { return internal_points.Count; }
        }

        /// <summary> Read-only collection of all the edge points for this polygon </summary>
        public ReadOnlyCollection<Coordinate_Point> Edge_Points
        {
            get
            {
                return new ReadOnlyCollection<Coordinate_Point>(edge_points);
            }
        }

        /// <summary> Read-only collection of all inner points of interest for this polygon </summary>
        public ReadOnlyCollection<Coordinate_Point> Inner_Points
        {
            get { return new ReadOnlyCollection<Coordinate_Point>(internal_points); }
        }

        /// <summary> Label for this polygon within the digital resource </summary>
        public string Label
        {
            get { return label ?? String.Empty; }
            set { label = value; }
        }

        /// <summary> Identifier for this polygon is pulled from the XML Node ID </summary>
        public string ID
        {
            get { return id ?? String.Empty; }
            set { id = value; }
        }
        
        /// <summary> Page sequence if this polygon represents coordinates for a single page in this resource </summary>
        public ushort Page_Sequence
        {
            get { return pageSequence; }
            set { pageSequence = value; }
        }

        /// <summary> Writes this polygon of coordinate points as SobekCM-formatted XML </summary>
        /// <param name="sobekcm_namespace"> Namespace to use for the SobekCM custom schema ( usually 'sobekcm' )</param>
        /// <param name="results"> Stream to write this polygon of coordinate points as SobekCM-formatted XML</param>
        internal void Add_SobekCM_Metadata(string sobekcm_namespace, System.IO.TextWriter results)
        {
            if ((edge_points.Count == 0) && (internal_points.Count == 0))
                return;

            results.Write( "<" + sobekcm_namespace + ":Polygon");
            if ( !String.IsNullOrEmpty(label))
                results.Write(" label=\"" + label + "\"");
            if (!String.IsNullOrEmpty(id))
                results.Write(" ID=\"" + id + "\"");
            if (pageSequence > 0)
                results.Write(" pageSeq=\"" + pageSequence + "\"");
            results.Write(">\r\n");
            if (edge_points.Count > 0)
            {
                results.Write( "<" + sobekcm_namespace + ":Edge>\r\n");
                int order = 1;
                foreach (Coordinate_Point thisPoint in edge_points)
                {
                    thisPoint.Add_SobekCM_Metadata(order++, sobekcm_namespace, results);
                }
                results.Write( "</" + sobekcm_namespace + ":Edge>\r\n");
            }
            if (internal_points.Count > 0)
            {
                results.Write("<" + sobekcm_namespace + ":Internal>\r\n");
                foreach (Coordinate_Point thisPoint in internal_points)
                {
                    thisPoint.Add_SobekCM_Metadata(-1, sobekcm_namespace, results);
                }
                results.Write( "</" + sobekcm_namespace + ":Internal>\r\n");
            }
            results.Write( "</" + sobekcm_namespace + ":Polygon>\r\n");
        }
    }
}
