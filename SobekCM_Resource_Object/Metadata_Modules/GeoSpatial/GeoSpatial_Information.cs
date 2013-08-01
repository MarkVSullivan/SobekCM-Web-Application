#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.GeoSpatial
{
    /// <summary> Extension metadata module contains geo-spatial coordinate information, such
    /// as map coverage and points or areas of interest </summary>
    [Serializable]
    public class GeoSpatial_Information : iMetadata_Module
    {
        private string kml_reference;
        private List<Coordinate_Line> lines;
        private List<Coordinate_Point> points;
        private List<Coordinate_Polygon> polygons;
        private double sobekcm_main_spatial_distance;
        private string sobekcm_main_spatial_string;

        /// <summary> Constructor for a new instance of the GeoSpatial_Information class </summary>
        public GeoSpatial_Information()
        {
            points = new List<Coordinate_Point>();
            polygons = new List<Coordinate_Polygon>();
            lines = new List<Coordinate_Line>();

            sobekcm_main_spatial_distance = -1;
            sobekcm_main_spatial_string = String.Empty;
        }
        
        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'GeoSpatial'</value>
        public string Module_Name
        {
            get { return "GeoSpatial"; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get { return null; }
        }

        /// <summary> Chance for this metadata module to perform any additional database work
        /// such as saving digital resource data into custom tables </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString"> Connection string for the current database </param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently saves the coordinate information to the database by calling
        /// the stored procedure SobekCM_Save_Item_Footprint  </remarks>
        public bool Save_Additional_Info_To_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            // Save each and every coordinate point
            foreach (Coordinate_Point thisPoint in Points)
            {
                Save_Item_Footprint(ItemID, thisPoint.Latitude, thisPoint.Longitude, -1.0, -1.0, -1.0, -1.0, DB_ConnectionString, out Error_Message);
            }

            // Add each polygon (bounding box only)
            double rect_latitude_a;
            double rect_longitude_a;
            double rect_latitude_b;
            double rect_longitude_b;
            for (int rect_index = 0; rect_index < Polygon_Count; rect_index++)
            {
                try
                {
                    // Get the polygon
                    Coordinate_Polygon polygon = Get_Polygon(rect_index);

                    // Set initial values
                    rect_latitude_a = -1.0;
                    rect_longitude_a = -1.0;
                    rect_latitude_b = -1.0;
                    rect_longitude_b = -1.0;

                    // Step through each point
                    double this_latitude;
                    double this_longitude;
                    bool first_point = true;
                    foreach ( Coordinate_Point thisPoint in polygon.Edge_Points)
                    {
                        try
                        {
                            this_latitude = thisPoint.Latitude;
                            this_longitude = thisPoint.Longitude;

                            if (first_point)
                            {
                                rect_latitude_a = this_latitude;
                                rect_longitude_a = this_longitude;
                                rect_latitude_b = this_latitude;
                                rect_longitude_b = this_longitude;
                                first_point = false;
                            }
                            else
                            {
                                if (this_latitude < rect_latitude_a)
                                {
                                    rect_latitude_a = this_latitude;
                                }
                                if (this_latitude > rect_latitude_b)
                                {
                                    rect_latitude_b = this_latitude;
                                }
                                if (this_longitude < rect_longitude_a)
                                {
                                    rect_longitude_a = this_longitude;
                                }
                                if (this_longitude > rect_longitude_b)
                                {
                                    rect_longitude_b = this_longitude;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    // Ensure the values aren't all still -1.0
                    if ((rect_latitude_a != -1.0) && (rect_longitude_a != -1.0))
                    {
                        Save_Item_Footprint(ItemID, -1.0, -1.0, rect_latitude_a, rect_longitude_a, rect_latitude_b, rect_longitude_b, DB_ConnectionString, out Error_Message );
                    }
                }
                catch
                {

                }
            }

            return true;
        }

        /// <summary> Tries to convert a coordinate string into a floating point number </summary>
        /// <param name="coordinate_string"> Cooordinate string to convert </param>
        /// <returns> Coordinate as a floating point (double) </returns>
        public static double get_coordinate_float(string coordinate_string)
        {
            coordinate_string = coordinate_string.Replace("°", "");
            return Convert.ToDouble(coordinate_string);
        }

        /// <summary> Saves ta single item footprint to the database by calling the appropriate stored procedure </summary>
        /// <param name="ItemID"> Primary key for this item in the database </param>
        /// <param name="Point_Latitude">  Latitude of a single point </param>
        /// <param name="Point_Longitude"> Longitude of a single point </param>
        /// <param name="Rect_Latitude_A"> Latitude of the first point of a rectangular footprint </param>
        /// <param name="Rect_Longitude_A"> Longitude of the first point of a rectangular footprint </param>
        /// <param name="Rect_Latitude_B"> Latitude of the second point of a rectangular footprint </param>
        /// <param name="Rect_Longitude_B"> Longitude of the second point of a rectangular footprint </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Save_Item_Footprint(int ItemID, double Point_Latitude, double Point_Longitude, double Rect_Latitude_A,
            double Rect_Longitude_A, double Rect_Latitude_B, double Rect_Longitude_B, string DB_ConnectionString, out string Error_Message )
        {
            Error_Message = String.Empty;

             // Open the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DB_ConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    Error_Message = "Error opening connection to the database from GeoSpatial metadata module : " + ex.Message;
                    return false;
                }

                // Create the sql command / stored procedure
                SqlCommand cmd = new SqlCommand("SobekCM_Save_Item_Footprint");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnect;

                // Add the parameters
                cmd.Parameters.AddWithValue("@itemid", ItemID);
                if ( Point_Latitude == -1.0 )
                    cmd.Parameters.AddWithValue("@Point_Latitude", DBNull.Value );
                else
                    cmd.Parameters.AddWithValue("@Point_Latitude", Point_Latitude);
                if (Point_Longitude == -1.0)
                    cmd.Parameters.AddWithValue("@Point_Longitude", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Point_Longitude", Point_Longitude);
                if (Rect_Latitude_A == -1.0)
                    cmd.Parameters.AddWithValue("@Rect_Latitude_A", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Rect_Latitude_A", Rect_Latitude_A);
                if (Rect_Longitude_A == -1.0)
                    cmd.Parameters.AddWithValue("@Rect_Longitude_A", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Rect_Longitude_A", Rect_Longitude_A);
                if (Rect_Latitude_B == -1.0)
                    cmd.Parameters.AddWithValue("@Rect_Latitude_B", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Rect_Latitude_B", Rect_Latitude_B);
                if (Rect_Longitude_B == -1.0)
                    cmd.Parameters.AddWithValue("@Rect_Longitude_B", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Rect_Longitude_B", Rect_Longitude_B);

                // Execute the non-query SQL stored procedure
                try
                {
                    // Run the command
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Error_Message = "Error saving values from GeoSpatial metadata module : " + ex.Message;
                    return false;
                }
                                
                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    Error_Message = "Error closing the connection to the database from GeoSpatial metadata module : " + ex.Message;
                    return false;
                }
            }

            return true;

        }

        /// <summary> Chance for this metadata module to load any additional data from the 
        /// database when building this digital resource  in memory </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString">Connection string for the current database</param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Retrieve_Additional_Info_From_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        #endregion

        /// <summary> Gets flag indicating if this object contains any coordinate information </summary>
        /// <value>TRUE if any data exists, otherwise FALSE </value>
        public bool hasData
        {
            get
            {
                if (((points == null) || (points.Count == 0)) &&
                    ((polygons == null) || (polygons.Count == 0)) &&
                    ((lines == null) || (lines.Count == 0)) &&
                    (String.IsNullOrEmpty(kml_reference)))
                    return false;
                else
                    return true;
            }
        }

        /// <summary> Reference to an external KML file to use for the coordinates </summary>
        public string KML_Reference
        {
            get { return kml_reference ?? String.Empty; }
            set { kml_reference = value; }
        }

        /// <summary> Read-only collection of points associated with this digital resource </summary>
        public ReadOnlyCollection<Coordinate_Point> Points
        {
            get { return new ReadOnlyCollection<Coordinate_Point>(points); }
        }

        /// <summary> Number of coordinate points associated with this digital resource </summary>
        public int Point_Count
        {
            get { return points.Count; }
        }

        /// <summary> Read-only collection of lines associated with this digital resource </summary>
        public ReadOnlyCollection<Coordinate_Line> Lines
        {
            get { return new ReadOnlyCollection<Coordinate_Line>(lines); }
        }

        /// <summary> Number of lines associated with this digital resource </summary>
        public int Line_Count
        {
            get { return lines.Count; }
        }

        /// <summary> Read-only collection of polygons associated with this digital resource </summary>
        public ReadOnlyCollection<Coordinate_Polygon> Polygons
        {
            get { return new ReadOnlyCollection<Coordinate_Polygon>(polygons); }
        }

        /// <summary> Number of polygons associated with this digital resource </summary>
        public int Polygon_Count
        {
            get { return polygons.Count; }
        }

        /// <summary> Remove a polygon from the collection of polygons associated with these coordinates </summary>
        /// <param name="Polygon_To_Remove"> Polygon to remove </param>
        public void Remove_Polygon(Coordinate_Polygon Polygon_To_Remove)
        {
            polygons.Remove(Polygon_To_Remove);
        }

        /// <summary> Gets the distance value, which is the diagonal between the furthest points 
        /// in the bounding box of the area </summary>
        /// <remarks> If the only coordinates are just points or lines, this value will be zero </remarks>
        public double SobekCM_Main_Spatial_Distance
        {
            get
            {
                if (sobekcm_main_spatial_distance < 0)
                {
                    Compute_SobekCM_Main_Spatial();
                }
                return sobekcm_main_spatial_distance;
            }
        }

        /// <summary> Gets the string which represents the main display area or point that represents the
        /// coordinate information for this digital resource </summary>
        /// <remarks> This is stored in the database and is used to display during search results at the item aggregation level </remarks>
        public string SobekCM_Main_Spatial_String
        {
            get
            {
                if (sobekcm_main_spatial_distance < 0)
                {
                    Compute_SobekCM_Main_Spatial();
                }

                return sobekcm_main_spatial_string;
            }
        }

        #region Public property calculates the SobekCM main spatial string for the database

        private void Compute_SobekCM_Main_Spatial()
        {
            // Set the distance to zero initially
            sobekcm_main_spatial_distance = 0;

            // Fields to hold the boundary positions
            double least_latitude = 0;
            double most_latitude = 0;
            double least_longitude = 0;
            double most_longitude = 0;

            // Build the spatial_kml for this
            StringBuilder spatial_kml_builder = new StringBuilder(50);
            string spatial_kml = String.Empty;
            try
            {
                // Check for areas first
                if (Polygon_Count > 0)
                {
                    // If only one polygon, easy to assign
                    if (Polygon_Count == 1)
                    {
                        spatial_kml_builder.Append("A|");

                        Coordinate_Point first_point = Get_Polygon(0).Edge_Points[0];
                        least_latitude = first_point.Latitude;
                        most_latitude = first_point.Latitude;
                        least_longitude = first_point.Longitude;
                        most_longitude = first_point.Longitude;
                        foreach (Coordinate_Point thisPoint in Get_Polygon(0).Edge_Points)
                        {
                            if (thisPoint.Latitude < least_latitude)
                                least_latitude = thisPoint.Latitude;
                            if (thisPoint.Latitude > most_latitude)
                                most_latitude = thisPoint.Latitude;
                            if (thisPoint.Longitude < least_longitude)
                                least_longitude = thisPoint.Longitude;
                            if (thisPoint.Longitude > most_longitude)
                                most_longitude = thisPoint.Longitude;

                            if (spatial_kml_builder.Length > 2)
                                spatial_kml_builder.Append("|");

                            spatial_kml_builder.Append(thisPoint.Latitude + "," + thisPoint.Longitude);
                        }
                    }
                    else
                    {
                        // Try to find the display polygon
                        Coordinate_Polygon polygon = null;
                        for (int i = 0; i < Polygon_Count; i++)
                        {
                            Coordinate_Polygon thisPolygon = Get_Polygon(i);
                            if ((thisPolygon.Label.ToUpper().IndexOf("DISPLAY") >= 0) || (thisPolygon.Label.ToUpper().IndexOf("MAIN") >= 0))
                            {
                                polygon = thisPolygon;
                                break;
                            }
                        }
                        if (polygon != null)
                        {
                            // Either a DISPLAY or MAIN polygon was found
                            spatial_kml_builder.Append("A|");
                            foreach (Coordinate_Point thisPoint in polygon.Edge_Points)
                            {
                                if (spatial_kml_builder.Length > 2)
                                    spatial_kml_builder.Append("|");

                                spatial_kml_builder.Append(thisPoint.Latitude + "," + thisPoint.Longitude);
                            }
                            ReadOnlyCollection<Coordinate_Point> bounding_boxes = polygon.Bounding_Box;
                            if (bounding_boxes.Count == 2)
                            {
                                least_latitude = Math.Min(bounding_boxes[0].Latitude, bounding_boxes[1].Latitude);
                                most_latitude = Math.Max(bounding_boxes[0].Latitude, bounding_boxes[1].Latitude);
                                least_longitude = Math.Min(bounding_boxes[0].Longitude, bounding_boxes[1].Longitude);
                                most_longitude = Math.Max(bounding_boxes[0].Longitude, bounding_boxes[1].Longitude);
                            }
                        }
                        else
                        {
                            // Determine a bounding box then
                            Coordinate_Polygon polygon2 = Get_Polygon(0);
                            Coordinate_Point first_point = polygon2.Bounding_Box[0];
                            least_latitude = first_point.Latitude;
                            most_latitude = first_point.Latitude;
                            least_longitude = first_point.Longitude;
                            most_longitude = first_point.Longitude;
                            foreach (Coordinate_Point thisPoint in polygon2.Bounding_Box)
                            {
                                if (thisPoint.Latitude < least_latitude)
                                    least_latitude = thisPoint.Latitude;
                                if (thisPoint.Latitude > most_latitude)
                                    most_latitude = thisPoint.Latitude;
                                if (thisPoint.Longitude < least_longitude)
                                    least_longitude = thisPoint.Longitude;
                                if (thisPoint.Longitude > most_longitude)
                                    most_longitude = thisPoint.Longitude;
                            }
                            for (int i = 1; i < Polygon_Count; i++)
                            {
                                Coordinate_Polygon thisPolygon = Get_Polygon(i);
                                foreach (Coordinate_Point thisPoint in thisPolygon.Bounding_Box)
                                {
                                    if (thisPoint.Latitude < least_latitude)
                                        least_latitude = thisPoint.Latitude;
                                    if (thisPoint.Latitude > most_latitude)
                                        most_latitude = thisPoint.Latitude;
                                    if (thisPoint.Longitude < least_longitude)
                                        least_longitude = thisPoint.Longitude;
                                    if (thisPoint.Longitude > most_longitude)
                                        most_longitude = thisPoint.Longitude;
                                }
                            }
                            if ((least_latitude != most_latitude) || (least_longitude != most_longitude))
                            {
                                spatial_kml_builder.Append("A|" + least_latitude + "," + least_longitude + "|" + most_latitude + "," + most_longitude);
                            }
                            else
                            {
                                spatial_kml_builder.Append("P|" + least_latitude + "," + least_longitude);
                            }
                        }
                    }

                    // Since this is an area, compute the greatest distance
                    double latitude_distance = Math.Abs(most_latitude - least_latitude);
                    double longitude_distance = Math.Abs(most_longitude - least_longitude);
                    sobekcm_main_spatial_distance = Math.Sqrt(Math.Pow(latitude_distance, 2) + Math.Pow(longitude_distance, 2));
                }
            }
            catch
            {
            }

            try
            {
                // Try to build the spatial kml from points if there was no area possible
                if ((spatial_kml_builder.Length == 0) && (Point_Count > 0))
                {
                    // If only one point, this is easy!
                    if (Point_Count == 1)
                    {
                        Coordinate_Point thisPoint = points[0];
                        spatial_kml_builder.Append("P|" + thisPoint.Latitude + "," + thisPoint.Longitude);
                    }
                    else
                    {
                        Coordinate_Point first_point = points[0];
                        least_latitude = Convert.ToDouble(first_point.Latitude);
                        most_latitude = least_latitude;
                        least_longitude = Convert.ToDouble(first_point.Longitude);
                        most_longitude = least_latitude;
                        foreach (Coordinate_Point thisPoint in points)
                        {
                            if (thisPoint.Latitude < least_latitude)
                                least_latitude = thisPoint.Latitude;
                            if (thisPoint.Latitude > most_latitude)
                                most_latitude = thisPoint.Latitude;
                            if (thisPoint.Longitude < least_longitude)
                                least_longitude = thisPoint.Longitude;
                            if (thisPoint.Longitude > most_longitude)
                                most_longitude = thisPoint.Longitude;
                        }

                        if ((least_latitude != most_latitude) || (least_longitude != most_longitude))
                        {
                            spatial_kml_builder.Append("A|" + least_latitude + "," + least_longitude + "|" + most_latitude + "," + most_longitude);

                            // Since this is really points, we'll not chane the distance value from zero
                        }
                        else
                        {
                            spatial_kml_builder.Append("P|" + least_latitude + "," + least_longitude);
                        }
                    }
                }
            }
            catch
            {
            }

            sobekcm_main_spatial_string = spatial_kml_builder.ToString();
        }

        #endregion

        /// <summary> Adds a special polygon for an aerial tile coverage from the point notation in the upper right corner </summary>
        /// <param name="Latitude_Point"> Decimal degree latitude for the point notation for the aerial </param>
        /// <param name="Longitude_Point"> Decimal degree longitude for the point notation for the aerial</param>
        /// <param name="Scale"> Scale of the aerial tile ( i.e., for a map of scale 1:20000, set S = 20,000 )</param>
        /// <param name="Tile_Width"> Width of the physical aerial tile </param>
        /// <param name="Tile_Height"> Height of the physical aerial tile </param>
        /// <param name="Earth_Radius"> Radius of the earth in the same units as the height and width above </param>
        /// <param name="Label"> Label for this new aerial polygon notation from the point </param>
        /// <returns> Fully built coordinate polygon </returns>
        public Coordinate_Polygon Add_Aerial_Polygon(double Latitude_Point, double Longitude_Point, int Scale, double Tile_Width, double Tile_Height, ulong Earth_Radius, string Label)
        {
            // Calculate the opposite corner
            double latitude_calculated = Latitude_Point - ((Scale * Tile_Height * 180) / (Math.PI * Earth_Radius));
            double latitude_average_radian = Math.PI * (Latitude_Point + latitude_calculated) / 360;
            double longitude_calculated = Longitude_Point - ((Scale * Tile_Width * 180) / (Math.PI * Earth_Radius * Math.Cos(latitude_average_radian)));

            // Add a slight shift to account for the original point placement
            double lower_left_latitude = latitude_calculated;
            double lower_left_longitude = longitude_calculated;
            double upper_right_latitude = Latitude_Point;
            double upper_right_longitude = Longitude_Point;

            // Create the polygon
            Coordinate_Polygon aerialPolygon = new Coordinate_Polygon();
            aerialPolygon.Add_Edge_Point(upper_right_latitude, upper_right_longitude);
            aerialPolygon.Add_Edge_Point(upper_right_latitude, lower_left_longitude);
            aerialPolygon.Add_Edge_Point(lower_left_latitude, lower_left_longitude);
            aerialPolygon.Add_Edge_Point(lower_left_latitude, upper_right_longitude);
            aerialPolygon.Label = Label;

            // Add this to this item
            polygons.Add(aerialPolygon);

            // Return the built polygon
            return aerialPolygon;
        }

        /// <summary> Add a single, completely built point associated with this digital resource </summary>
        /// <param name="Point"> Coordinate line object associated with this digital resource </param>
        public void Add_Point(Coordinate_Point Point)
        {
            // Only add this point if it does not already exists
            if (points == null)
                points = new List<Coordinate_Point>();

            foreach (Coordinate_Point existingPoint in points)
            {
                if ((existingPoint.Latitude == Point.Latitude) && (existingPoint.Longitude == Point.Longitude))
                    return;
            }

            if (Point != null)
                points.Add(Point);
        }

        /// <summary> Adds a new coordinate point associated with this digital resource </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        public void Add_Point(double Latitude, double Longitude)
        {
            // Only add this point if it does not already exists
            if (points == null)
                points = new List<Coordinate_Point>();

            foreach (Coordinate_Point existingPoint in points)
            {
                if ((existingPoint.Latitude == Latitude) && (existingPoint.Longitude == Longitude))
                    return;
            }

            points.Add(new Coordinate_Point(Latitude, Longitude));
        }

        /// <summary> Adds a new coordinate point associated with this digital resource </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        public void Add_Point(double Latitude, double Longitude, string Label)
        {
            // Only add this point if it does not already exists
            if (points == null)
                points = new List<Coordinate_Point>();

            foreach (Coordinate_Point existingPoint in points)
            {
                if ((existingPoint.Latitude == Latitude) && (existingPoint.Longitude == Longitude))
                    return;
            }

            points.Add(new Coordinate_Point(Latitude, Longitude, Label));
        }

        /// <summary> Adds a new coordinate point associated with this digital resource </summary>
        /// <param name="Latitude"> Latitude (expressed in decimal notation) for this point </param>
        /// <param name="Longitude"> Longitude (expressed in decimal notation) for this point </param>
        /// <param name="Label"> Label to associate with this point </param>
        /// <param name="Altitude"> Altitude for this point on a 3-dimensional plane (in meters)</param>
        public void Add_Point(double Latitude, double Longitude, string Label, long Altitude)
        {
            // Only add this point if it does not already exists
            if (points == null)
                points = new List<Coordinate_Point>();

            foreach (Coordinate_Point existingPoint in points)
            {
                if ((existingPoint.Latitude == Latitude) && (existingPoint.Longitude == Longitude))
                    return;
            }

            points.Add(new Coordinate_Point(Latitude, Longitude, Label, Altitude));
        }

        /// <summary> Pulls one coordinate line from the collection of lines associated with this digital resource </summary>
        /// <param name="index"> Index of the line to retrieve </param>
        /// <returns> The indicated line or NULL </returns>
        public Coordinate_Line Get_Line(int index)
        {
            if (index < lines.Count)
            {
                return lines[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary> Add a single, completely built line associated with this digital resource </summary>
        /// <param name="Line"> Coordinate line object associated with this digital resource </param>
        public void Add_Line(Coordinate_Line Line)
        {
            lines.Add(Line);
        }

        /// <summary> Pulls one coordinate polygon from the collection of polygons associated with this digital resource </summary>
        /// <param name="index"> Index of the polygon to retrieve </param>
        /// <returns> The indicated polygon or NULL </returns>
        public Coordinate_Polygon Get_Polygon(int index)
        {
            if (index < polygons.Count)
            {
                return polygons[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary> Add a single, completely built polygon associated with this digital resource </summary>
        /// <param name="Polygon"> Coordinate polygon object associated with this digital resource </param>
        public void Add_Polygon(Coordinate_Polygon Polygon)
        {
            polygons.Add(Polygon);
        }

        /// <summary> Clears all of the coordinate data from this digital resource </summary>
        public void Clear_Data()
        {
            points.Clear();
            polygons.Clear();
            lines.Clear();
        }

        /// <summary> Clears all of the individual point information from this digital resource </summary>
        public void Clear_Points()
        {
            points.Clear();
        }

        /// <summary> Clears all user added polygons and all coordinate lines from this digital resource </summary>
        public void Clear_User_Polygons_And_Lines()
        {
            // Save any user entered polygons
            if (polygons.Count > 0)
            {
                List<Coordinate_Polygon> savePolys = new List<Coordinate_Polygon>();
                foreach (Coordinate_Polygon thisPoly in polygons)
                {
                    if (thisPoly.ID.IndexOf("USER") >= 0)
                    {
                        savePolys.Add(thisPoly);
                    }
                }
                polygons.Clear();
                foreach (Coordinate_Polygon thisPoly in savePolys)
                {
                    polygons.Remove(thisPoly);
                }
            }
            lines.Clear();
        }

        //add poi point
        public void Add_POI_Point(double Latitude, double Longitude, string Label)
        {
            // Only add this point if it does not already exists
            if (points == null)
                points = new List<Coordinate_Point>();

            foreach (Coordinate_Point existingPoint in points)
            {
                if ((existingPoint.Latitude == Latitude) && (existingPoint.Longitude == Longitude))
                    return;
            }

            points.Add(new Coordinate_Point(Latitude, Longitude, Label));
        }

        //add line
        public void Add_POI_Line(Coordinate_Line Line)
        {
            lines.Add(Line);
        }

        //add polygon
        public void Add_POI_Polygon(Coordinate_Polygon Polygon)
        {
            polygons.Add(Polygon);
        }

        //add circle
        public void Add_POI_Circle()
        {
           
        }
    }
}