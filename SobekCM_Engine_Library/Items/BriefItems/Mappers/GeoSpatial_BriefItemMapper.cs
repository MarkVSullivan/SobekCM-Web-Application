#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the geo-spatial specific metadata from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class GeoSpatial_BriefItemMapper: IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Attempt to pull the top-level geo-spatial data from the source object
            GeoSpatial_Information geoInfo = Original.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;

            // If there was geo-spatial data here, add it to the new item
            if ((geoInfo != null) && (geoInfo.hasData) && ((geoInfo.Point_Count > 0) || (geoInfo.Polygon_Count > 0)))
            {
                // Add each point first into the description
                ReadOnlyCollection<Coordinate_Point> origPoints = geoInfo.Points;
                foreach ( Coordinate_Point thisPoint in origPoints )
                {
                    if ( !String.IsNullOrEmpty(thisPoint.Label))
                    {
                        New.Add_Description("Coordinates", thisPoint.Latitude + " x " + thisPoint.Longitude + " ( " + thisPoint.Label + " )");
                    }
                    else
                    {
                        New.Add_Description("Coordinates", thisPoint.Latitude + " x " + thisPoint.Longitude );
                    }

                    // Also add each point into the object itself
                    if (New.GeoSpatial.Points == null)
                        New.GeoSpatial.Points = new List<BriefItem_Coordinate_Point>();

                    // Create the new point
                    BriefItem_Coordinate_Point cPoint = new BriefItem_Coordinate_Point
                    {
                        Latitude = thisPoint.Latitude, 
                        Longitude = thisPoint.Longitude, 
                        Altitude = thisPoint.Altitude, 
                        Label = thisPoint.Label, 
                        FeatureType = thisPoint.FeatureType
                    };

                    // Add it
                    New.GeoSpatial.Points.Add(cPoint);
                }

                // Add the polygons to the description, if there is only one.
                if (geoInfo.Polygon_Count == 1)
                {
                    for (int i = 0; i < geoInfo.Polygon_Count; i++)
                    {
                        Coordinate_Polygon polygon = geoInfo.Get_Polygon(i);
                        StringBuilder polygonBuilder = new StringBuilder();
                        foreach (Coordinate_Point thisPoint in polygon.Edge_Points)
                        {
                            if (polygonBuilder.Length > 0)
                            {
                                polygonBuilder.Append(", " + thisPoint.Latitude + " x " + thisPoint.Longitude);
                            }
                            else
                            {
                                polygonBuilder.Append(thisPoint.Latitude + " x " + thisPoint.Longitude);
                            }
                        }

                        if (polygon.Label.Length > 0)
                        {
                            polygonBuilder.Append(" ( " + polygon.Label + " )");
                        }
                        if (polygonBuilder.ToString().Trim().Length > 0)
                        {
                            New.Add_Description("Polygon", polygonBuilder.ToString());
                        }
                    }
                }

                // Map each polygon over now as well
                if (geoInfo.Polygon_Count > 0)
                {
                    // Ensure the polygon collection is defined
                    if (New.GeoSpatial.Polygons == null)
                        New.GeoSpatial.Polygons = new List<BriefItem_Coordinate_Polygon>();

                    // Get the collection of polygons and step through them
                    ReadOnlyCollection<Coordinate_Polygon> origPolys = geoInfo.Polygons;
                    foreach (Coordinate_Polygon thisPoly in origPolys)
                    {
                        // Start to build the new poly
                        BriefItem_Coordinate_Polygon cPoly = new BriefItem_Coordinate_Polygon
                        {
                            Label = thisPoly.Label,
                            FeatureType = thisPoly.FeatureType,
                            Page_Sequence = thisPoly.Page_Sequence,
                            Rotation = thisPoly.Rotation,
                            PolygonType = thisPoly.PolygonType
                        };

                        // Copy over all the vertices
                        if (thisPoly.Edge_Points_Count > 0)
                        {
                            // Ensure the edge points collection is defined 
                            if (cPoly.Edge_Points == null)
                                cPoly.Edge_Points = new List<BriefItem_Coordinate_Point>();

                            // Copy over all the vertices
                            ReadOnlyCollection<Coordinate_Point> origVertices = thisPoly.Edge_Points;
                            foreach (Coordinate_Point thisPoint in origVertices)
                            {
                                // Create the new point
                                BriefItem_Coordinate_Point cPoint = new BriefItem_Coordinate_Point
                                {
                                    Latitude = thisPoint.Latitude,
                                    Longitude = thisPoint.Longitude,
                                    Altitude = thisPoint.Altitude,
                                    Label = thisPoint.Label,
                                    FeatureType = thisPoint.FeatureType
                                };

                                // Add it
                                cPoly.Edge_Points.Add(cPoint);
                            }
                        }

                        // Add this poly
                        New.GeoSpatial.Polygons.Add(cPoly);
                    }
                }

                // Map each line over now as well
                if (geoInfo.Line_Count > 0)
                {
                    // Ensure the line collection is defined
                    if (New.GeoSpatial.Lines == null)
                        New.GeoSpatial.Lines = new List<BriefItem_Coordinate_Line>();

                    // Get the collection of lines and step through them
                    ReadOnlyCollection<Coordinate_Line> origLines = geoInfo.Lines;
                    foreach (Coordinate_Line thisLine in origLines)
                    {
                        // Start to build the new line
                        BriefItem_Coordinate_Line cLine = new BriefItem_Coordinate_Line
                        {
                            Label = thisLine.Label,
                            FeatureType = thisLine.FeatureType
                        };

                        // Copy over all the vertices
                        if (thisLine.Point_Count > 0)
                        {
                            // Ensure the points collection is defined 
                            if (cLine.Points == null)
                                cLine.Points = new List<BriefItem_Coordinate_Point>();

                            // Copy over all the vertices
                            ReadOnlyCollection<Coordinate_Point> origVertices = thisLine.Points;
                            foreach (Coordinate_Point thisPoint in origVertices)
                            {
                                // Create the new point
                                BriefItem_Coordinate_Point cPoint = new BriefItem_Coordinate_Point
                                {
                                    Latitude = thisPoint.Latitude,
                                    Longitude = thisPoint.Longitude,
                                    Altitude = thisPoint.Altitude,
                                    Label = thisPoint.Label,
                                    FeatureType = thisPoint.FeatureType
                                };

                                // Add it
                                cLine.Points.Add(cPoint);
                            }
                        }

                        // Add this poly
                        New.GeoSpatial.Lines.Add(cLine);
                    }
                }
            }

            // Now, copy over all the geo-spatial information at the page level
            List<abstract_TreeNode> pages = Original.Divisions.Physical_Tree.Pages_PreOrder;
            for (int i = 0; i < pages.Count; i++)
            {
                abstract_TreeNode pageNode = pages[i];
                GeoSpatial_Information geoInfo2 = pageNode.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                if ((geoInfo2 != null) && (geoInfo2.hasData))
                {
                    if (geoInfo2.Polygon_Count > 0)
                    {
                        foreach (Coordinate_Polygon thisPolygon in geoInfo2.Polygons)
                        {
                            thisPolygon.Page_Sequence = (ushort) (i + 1);

                            // Ensure the polygon collection is defined
                            if (New.GeoSpatial.Polygons == null)
                                New.GeoSpatial.Polygons = new List<BriefItem_Coordinate_Polygon>();

                            // Get the collection of polygons and step through them
                            ReadOnlyCollection<Coordinate_Polygon> origPolys = geoInfo.Polygons;
                            foreach (Coordinate_Polygon thisPoly in origPolys)
                            {
                                // Start to build the new poly
                                BriefItem_Coordinate_Polygon cPoly = new BriefItem_Coordinate_Polygon
                                {
                                    Label = thisPoly.Label,
                                    FeatureType = thisPoly.FeatureType,
                                    Page_Sequence = thisPoly.Page_Sequence,
                                    Rotation = thisPoly.Rotation,
                                    PolygonType = thisPoly.PolygonType
                                };

                                // Copy over all the vertices
                                if (thisPoly.Edge_Points_Count > 0)
                                {
                                    // Ensure the edge points collection is defined 
                                    if (cPoly.Edge_Points == null)
                                        cPoly.Edge_Points = new List<BriefItem_Coordinate_Point>();

                                    // Copy over all the vertices
                                    ReadOnlyCollection<Coordinate_Point> origVertices = thisPoly.Edge_Points;
                                    foreach (Coordinate_Point thisPoint in origVertices)
                                    {
                                        // Create the new point
                                        BriefItem_Coordinate_Point cPoint = new BriefItem_Coordinate_Point
                                        {
                                            Latitude = thisPoint.Latitude,
                                            Longitude = thisPoint.Longitude,
                                            Altitude = thisPoint.Altitude,
                                            Label = thisPoint.Label,
                                            FeatureType = thisPoint.FeatureType
                                        };

                                        // Add it
                                        cPoly.Edge_Points.Add(cPoint);
                                    }
                                }

                                // Add this poly
                                New.GeoSpatial.Polygons.Add(cPoly);
                            }
                        }
                    }
                    if (geoInfo2.Line_Count > 0)
                    {
                        // Ensure the line collection is defined
                        if (New.GeoSpatial.Lines == null)
                            New.GeoSpatial.Lines = new List<BriefItem_Coordinate_Line>();

                        // Add each line
                        foreach (Coordinate_Line thisLine in geoInfo2.Lines)
                        {
                            // Start to build the new line
                            BriefItem_Coordinate_Line cLine = new BriefItem_Coordinate_Line
                            {
                                Label = thisLine.Label,
                                FeatureType = thisLine.FeatureType
                            };

                            // Copy over all the vertices
                            if (thisLine.Point_Count > 0)
                            {
                                // Ensure the points collection is defined 
                                if (cLine.Points == null)
                                    cLine.Points = new List<BriefItem_Coordinate_Point>();

                                // Copy over all the vertices
                                ReadOnlyCollection<Coordinate_Point> origVertices = thisLine.Points;
                                foreach (Coordinate_Point thisPoint in origVertices)
                                {
                                    // Create the new point
                                    BriefItem_Coordinate_Point cPoint = new BriefItem_Coordinate_Point
                                    {
                                        Latitude = thisPoint.Latitude,
                                        Longitude = thisPoint.Longitude,
                                        Altitude = thisPoint.Altitude,
                                        Label = thisPoint.Label,
                                        FeatureType = thisPoint.FeatureType
                                    };

                                    // Add it
                                    cLine.Points.Add(cPoint);
                                }
                            }

                            // Add this poly
                            New.GeoSpatial.Lines.Add(cLine);
                        }
                    }
                    if (geoInfo2.Point_Count > 0)
                    {
                        // Ensure the points collection was defined
                        if (New.GeoSpatial.Points == null)
                            New.GeoSpatial.Points = new List<BriefItem_Coordinate_Point>();

                        // Add each point, from the page
                        foreach (Coordinate_Point thisPoint in geoInfo2.Points)
                        {
                            // Create the new point
                            BriefItem_Coordinate_Point cPoint = new BriefItem_Coordinate_Point
                            {
                                Latitude = thisPoint.Latitude,
                                Longitude = thisPoint.Longitude,
                                Altitude = thisPoint.Altitude,
                                Label = thisPoint.Label,
                                FeatureType = thisPoint.FeatureType
                            };

                            // Add it
                            New.GeoSpatial.Points.Add(cPoint);
                        }
                    }
                }
            }

            return true;
        }
    }
}
