using System;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.BriefItem;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Tools;

namespace SobekCM.Library.Citation.SectionWriter
{
    /// <summary> Special citation section writer adds the coordinate information
    /// to the citation, from the main geographic section of the brief item object </summary>
    ///  <remarks> This class implements the <see cref="iCitationSectionWriter"/> interface. </remarks>
    public class Coordinates_SectionWriter : iCitationSectionWriter
    {
        /// <summary> Returns flag that indicates this citation section writer 
        /// will be writing alues to the output stream </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Item"> Digital resource to analyze for data to write </param>
        public bool Has_Data_To_Write(CitationElement ElementInfo, BriefItemInfo Item)
        {
            return ((Item.GeoSpatial != null) && ((Item.GeoSpatial.Point_Count > 0) || (Item.GeoSpatial.Polygon_Count > 0)));
        }

        /// <summary> Wites a section of citation from a provided digital resource </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Item"> Digital resource with all the data to write </param>
        /// <param name="LeftColumnWidth"> Number of pixels of the left column, or the definition terms </param>
        /// <param name="SearchLink"> Beginning of the search link that can be used to allow the web patron to select a term and run a search against this instance </param>
        /// <param name="SearchLinkEnd"> End of the search link that can be used to allow the web patron to select a term and run a search against this instance  </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Citation_Section(CitationElement ElementInfo, StringBuilder Output, BriefItemInfo Item, int LeftColumnWidth, string SearchLink, string SearchLinkEnd, Custom_Tracer Tracer)
        {
            bool first_coordinate = true;

            string displayLabel = ( String.IsNullOrEmpty(ElementInfo.DisplayTerm )) ? "Coordinates" : ElementInfo.DisplayTerm;

            Output.AppendLine("        <dt class=\"sbk_CivCOORDINATES_Element\" style=\"width:" + LeftColumnWidth + "px;\" >" + displayLabel + ": </dt>");
            Output.Append("        <dd class=\"sbk_CivCOORDINATES_Element\" style=\"margin-left:" + LeftColumnWidth + "px;\" >");

            // Add the points first
            if (Item.GeoSpatial.Point_Count > 0)
            {
                for (int i = 0; i < Item.GeoSpatial.Point_Count; i++)
                {
                    // Was this the first?
                    if ( first_coordinate ) first_coordinate = false;
                    else Output.AppendLine("<br />");

                    // Add this coordiante
                    if (Item.GeoSpatial.Points[i].Label.Length > 0)
                    {
                        Output.Append("          <span itemprop=\"geo\" itemscope itemtype=\"http://schema.org/GeoCoordinates\"><span itemprop=\"latitude\">" + Item.GeoSpatial.Points[i].Latitude + "</span> x <span itemprop=\"longitude\">" + Item.GeoSpatial.Points[i].Longitude + "</span> ( <span itemprop=\"name\">" + HttpUtility.HtmlEncode(Item.GeoSpatial.Points[i].Label) + "</span> )</span>");
                    }
                    else
                    {
                        Output.Append("          <span itemprop=\"geo\" itemscope itemtype=\"http://schema.org/GeoCoordinates\"><span itemprop=\"latitude\">" + Item.GeoSpatial.Points[i].Latitude + "</span> x <span itemprop=\"longitude\">" + Item.GeoSpatial.Points[i].Longitude + "</span></span>");
                    }
                }
            }

            // If there is a single polygon ,add it
            if (Item.GeoSpatial.Polygon_Count == 1)
            {
                // If not the first, add 
                for (int i = 0; i < Item.GeoSpatial.Polygon_Count; i++)
                {
                    // Was this the first?
                    if (first_coordinate) first_coordinate = false;
                    else Output.AppendLine("<br />");

                    // Get the polygon and draw it
                    BriefItem_Coordinate_Polygon polygon = Item.GeoSpatial.Polygons[i];
                    StringBuilder polygonBuilder = new StringBuilder();
                    foreach (BriefItem_Coordinate_Point thisPoint in polygon.Edge_Points)
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
                        polygonBuilder.Append(" ( " + HttpUtility.HtmlEncode(polygon.Label) + " )");
                    }
                    if (polygonBuilder.ToString().Trim().Length > 0)
                    {
                        Output.Append("          " + polygonBuilder);
                    }
                }
            }

            Output.AppendLine("</dd>");
            Output.AppendLine();

        }
    }
}
