using System;
using System.Text;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Core.BriefItem;

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
                // Add each point first
                for (int i = 0; i < geoInfo.Point_Count; i++)
                {
                    if ( !String.IsNullOrEmpty(geoInfo.Points[i].Label))
                    {
                        New.Add_Description("Coordinates", geoInfo.Points[i].Latitude + " x " + geoInfo.Points[i].Longitude + " ( " + geoInfo.Points[i].Label + " )");
                    }
                    else
                    {
                        New.Add_Description("Coordinates", geoInfo.Points[i].Latitude + " x " + geoInfo.Points[i].Longitude );
                    }
                }

                // Add the polygons
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
            }

            return true;
        }
    }
}
