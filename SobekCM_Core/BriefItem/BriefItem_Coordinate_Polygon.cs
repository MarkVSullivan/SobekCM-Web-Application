#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Geographic information in the form of a polygon comprised of several coordinate points </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("coordinatePolygon")]
    public class BriefItem_Coordinate_Polygon
    {
        private List<BriefItem_Coordinate_Point> bounding_box;

        /// <summary> Constructor for a new instance of this BriefItem_Coordinate_Polygon class </summary>
        public BriefItem_Coordinate_Polygon()
        {
            Edge_Points = new List<BriefItem_Coordinate_Point>();
            Inner_Points = new List<BriefItem_Coordinate_Point>();
            Page_Sequence = 0;
        }

        /// <summary> Return the rectangular bounding box, with the first point in the upper left corner 
        /// and the second point in the lower right corner </summary>
        [XmlIgnore]
        public ReadOnlyCollection<BriefItem_Coordinate_Point> Bounding_Box
        {
            get
            {
                if (bounding_box != null)
                {
                    return new ReadOnlyCollection<BriefItem_Coordinate_Point>(bounding_box);
                }
                else
                {
                    bounding_box = new List<BriefItem_Coordinate_Point>();

                    if (Edge_Points.Count > 0)
                    {
                        // Look for the bounding box now
                        double least_latitude = Convert.ToDouble(Edge_Points[0].Latitude);
                        double most_latitude = least_latitude;
                        double least_longitude = Convert.ToDouble(Edge_Points[0].Longitude);
                        double most_longitude = least_longitude;
                        double interior_point_latitude = least_latitude;
                        double interior_point_longitude = least_longitude;
                        double this_latitude = least_latitude;
                        double this_longitude = least_longitude;
                        foreach (BriefItem_Coordinate_Point thisPoint in Edge_Points)
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
                        bounding_box.Add(new BriefItem_Coordinate_Point(most_latitude, least_longitude, String.Empty));
                        bounding_box.Add(new BriefItem_Coordinate_Point(least_latitude, most_longitude, String.Empty));
                    }

                    return new ReadOnlyCollection<BriefItem_Coordinate_Point>(bounding_box);
                }
            }
        }

        /// <summary> Number of points which comprise the edge of this polygon </summary>
        [XmlIgnore]
        public int Edge_Points_Count
        {
            get { return Edge_Points.Count; }
        }

        /// <summary> Number of internal points of interest in this polygon </summary>
        [XmlIgnore]
        public int Inner_Points_Count
        {
            get { return Inner_Points.Count; }
        }

        /// <summary> Read-only collection of all the edge points for this polygon </summary>
        [DataMember(EmitDefaultValue = false, Name = "vertices")]
        [XmlArray("vertices")]
        [XmlArrayItem("point", typeof(BriefItem_Coordinate_Point))]
        [ProtoMember(1)]
        public List<BriefItem_Coordinate_Point> Edge_Points { get; set; }

        /// <summary> Read-only collection of all inner points of interest for this polygon </summary>
        [DataMember(EmitDefaultValue = false, Name = "internals")]
        [XmlArray("internals")]
        [XmlArrayItem("point", typeof(BriefItem_Coordinate_Point))]
        [ProtoMember(2)]
        public List<BriefItem_Coordinate_Point> Inner_Points { get; set; }

        /// <summary> Rotation of this polygon </summary>
        [DataMember(EmitDefaultValue = false, Name = "rotation")]
        [XmlAttribute("rotation")]
        [ProtoMember(3)]
        public double Rotation { get; set; }

        /// <summary> FeatureType associated with this polygon </summary>
        [DataMember(EmitDefaultValue = false, Name = "feature")]
        [XmlAttribute("feature")]
        [ProtoMember(4)]
        public string FeatureType { get; set; }

        /// <summary> PolygonType associated with this polygon </summary>
        [DataMember(EmitDefaultValue = false, Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(5)]
        public string PolygonType { get; set; }

        /// <summary> Label for this polygon within the digital resource </summary>
        [DataMember(EmitDefaultValue = false, Name = "label")]
        [XmlAttribute("label")]
        [ProtoMember(6)]
        public string Label { get; set; }

        /// <summary> Page sequence if this polygon represents coordinates for a single page in this resource </summary>
        [DataMember(EmitDefaultValue = false, Name = "sequence")]
        [XmlAttribute("sequence")]
        [ProtoMember(7)]
        public ushort Page_Sequence { get; set; }

        /// <summary> Clears all the edge points in thie polygon </summary>
        public void Clear_Edge_Points()
        {
            Edge_Points.Clear();
        }

        /// <summary> Checks to see if a point is in the bounding box defined by this polygon </summary>
        /// <param name="Latitude"> Latitude of the point to check </param>
        /// <param name="Longitude"> Longitude of the point to check </param>
        /// <returns> TRUE if the point is in this polygon's bounding box, otherwise false</returns>
        public bool is_In_Bounding_Box(double Latitude, double Longitude)
        {
            bool in_lat_search = false;
            bool in_long_search = false;
            ReadOnlyCollection<BriefItem_Coordinate_Point> boundingBox = Bounding_Box;
            if ((Bounding_Box[0].Latitude >= Latitude) && (boundingBox[1].Latitude <= Latitude))
            {
                in_lat_search = true;
            }

            if ((boundingBox[0].Latitude <= Latitude) && (boundingBox[1].Latitude >= Latitude))
            {
                in_lat_search = true;
            }

            if (!in_lat_search)
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
            ReadOnlyCollection<BriefItem_Coordinate_Point> boundingBox = Bounding_Box;

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
            if (test_rect_lat2 > lat1)
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
        public ReadOnlyCollection<BriefItem_Coordinate_Point> Recalculate_Bounding_Box()
        {
            bounding_box = null;
            return Bounding_Box;
        }

        /// <summary> Add a new edge point of interest to this polygon  </summary>
        /// <param name="newPoint"> Built coordinate point object to add </param>
        public void Add_Edge_Point(BriefItem_Coordinate_Point newPoint)
        {
            // Update the current bounding box, if there is one
            if (bounding_box != null)
            {
                bounding_box = null;
            }

            Edge_Points.Add(newPoint);
        }

        /// <summary> Add a new edge point of interest to this polygon  </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <returns> Fully built BriefItem_Coordinate_Point object </returns>
        public BriefItem_Coordinate_Point Add_Edge_Point(double Latitude, double Longitude)
        {
            // Update the current bounding box, if there is one
            if (bounding_box != null)
            {
                bounding_box = null;
            }

            BriefItem_Coordinate_Point newPoint = new BriefItem_Coordinate_Point(Latitude, Longitude, String.Empty);
            Edge_Points.Add(newPoint);
            return newPoint;
        }

        /// <summary> Add a new edge point of interest to this polygon  </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <returns> Fully built BriefItem_Coordinate_Point object </returns>
        public BriefItem_Coordinate_Point Add_Edge_Point(double Latitude, double Longitude, string Label)
        {
            // Update the current bounding box, if there is one
            if (bounding_box != null)
            {
                bounding_box = null;
            }

            BriefItem_Coordinate_Point newPoint = new BriefItem_Coordinate_Point(Latitude, Longitude, Label);
            Edge_Points.Add(newPoint);
            return newPoint;
        }

        /// <summary> Add a new inner point of interest to this polygon  </summary>
        /// <param name="newPoint"> Built coordinate point object to add </param>
        public void Add_Inner_Point(BriefItem_Coordinate_Point newPoint)
        {
            Inner_Points.Add(newPoint);
        }

        /// <summary> Add a new inner point of interest to this polygon  </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <returns> Fully built BriefItem_Coordinate_Point object </returns>
        public BriefItem_Coordinate_Point Add_Inner_Point(double Latitude, double Longitude)
        {
            BriefItem_Coordinate_Point newPoint = new BriefItem_Coordinate_Point(Latitude, Longitude, String.Empty);
            Inner_Points.Add(newPoint);
            return newPoint;
        }

        /// <summary> Add a new inner point of interest to this polygon  </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <returns> Fully built BriefItem_Coordinate_Point object </returns>
        public BriefItem_Coordinate_Point Add_Inner_Point(double Latitude, double Longitude, string Label)
        {
            BriefItem_Coordinate_Point newPoint = new BriefItem_Coordinate_Point(Latitude, Longitude, Label);
            Inner_Points.Add(newPoint);
            return newPoint;
        }
    }
}