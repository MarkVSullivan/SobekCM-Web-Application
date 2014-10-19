#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    

    /// <summary> Item viewer displays the spatial coordinates or coverage for a digital resource on a Google map. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Google_Map_ItemViewer : abstractItemViewer
    {
        private bool googleItemSearch;
        private StringBuilder mapBuilder;
        private List<string> matchingTilesList;
        private double providedMaxLat;
        private double providedMaxLong;
        private double providedMinLat;
        private double providedMinLong;
        private bool validCoordinateSearchFound;

        List<Coordinate_Polygon> allPolygons;
        List<Coordinate_Point> allPoints;
        List<Coordinate_Line> allLines;

        /// <summary> Constructor for a new instance of the Google_Map_ItemViewer class </summary>
        public Google_Map_ItemViewer()
        {
            googleItemSearch = false;
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Google_Map"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Google_Map; }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is a single page viewer, so this property always returns the value 1</value>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 800 </value>
        public override int Viewer_Width
        {
            get { return 800; }
        }

        /// <summary> Perform necessary pre-display work.  In this case, any coordinate search is applied against 
        /// the polygon/coordinates in the METS file for the current item </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Perform_PreDisplay_Work(Custom_Tracer Tracer)
        {
            Perform_Coordinate_Search();
        }

        /// <summary> Method performs the actual search against the polygon/coordinates 
        /// in the METS file for the current item  </summary>
        protected void Perform_Coordinate_Search()
        {
            mapBuilder = new StringBuilder();
            if (CurrentMode.ViewerCode == "mapsearch")
                googleItemSearch = true;

            // If coordinates were passed in, pull the actual coordinates out of the URL
            validCoordinateSearchFound = false;
            if (CurrentMode.Coordinates.Length > 0)
            {
                string[] splitter = CurrentMode.Coordinates.Split(",".ToCharArray());
                if (((splitter.Length > 1) && (splitter.Length < 4)) || ((splitter.Length == 4) && (splitter[2].Length == 0) && (splitter[3].Length == 0)))
                {
                    if ((Double.TryParse(splitter[0], out providedMaxLat)) && (Double.TryParse(splitter[1], out providedMaxLong)))
                        validCoordinateSearchFound = true;
                    providedMinLat = providedMaxLat;
                    providedMinLong = providedMaxLong;
                }
                else if (splitter.Length >= 4)
                {
                    if (( Double.TryParse(splitter[0], out providedMaxLat)) && (Double.TryParse(splitter[1], out providedMaxLong)) &&
                        (Double.TryParse(splitter[2], out providedMinLat)) && ( Double.TryParse(splitter[3], out providedMinLong)))
                    validCoordinateSearchFound = true;
                }
            }

            // Get the google map API
            mapBuilder.AppendLine("<script type=\"text/javascript\" src=\"http://maps.google.com/maps/api/js?sensor=false\"></script>");
            mapBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_map_search.js\"></script>");
            mapBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_map_tool.js\"></script>");

            // Set some values for the map key
            string search_type = "geographic";
            bool areas_shown = false;
            bool points_shown = false;
            string areas_name = "Sheet";
            if (CurrentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Aerial)
                areas_name = "Tile";

            // Load the map
            mapBuilder.AppendLine("<script type=\"text/javascript\">");
            mapBuilder.AppendLine("  //<![CDATA[");
            mapBuilder.AppendLine("  function load() {");
            mapBuilder.AppendLine(googleItemSearch
									  ? "    load_search_map(6, 19.5, 3, \"sbkGmiv_MapDiv\");"
									  : "    load_map(6, 19.5, 3, \"sbkGmiv_MapDiv\");");

            // Keep track of any matching tiles
            matchingTilesList = new List<string>();

            // Add the points
            if (CurrentItem != null)
            {
                allPolygons = new List<Coordinate_Polygon>();
                allPoints = new List<Coordinate_Point>();
                allLines = new List<Coordinate_Line>();

                // Add the search rectangle first
                if ((validCoordinateSearchFound) && (!googleItemSearch))
                {
                    if ((providedMaxLat != providedMinLat) || (providedMaxLong != providedMinLong))
                    {
                        search_type = "rectangle";
                        mapBuilder.AppendLine("    add_rectangle(" + providedMaxLat + ", " + providedMaxLong + ", " + providedMinLat + ", " + providedMinLong + ");");
                    }
                    else
                    {
                        search_type = "point";
                    }
                }

                // Build the matching polygon HTML to overlay the matches over the non-matches
                StringBuilder matchingPolygonsBuilder = new StringBuilder();

                // Collect all the polygons, points, and lines
                GeoSpatial_Information geoInfo = CurrentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                if ((geoInfo != null) && (geoInfo.hasData))
                {
                    if (geoInfo.Polygon_Count > 0)
                    {
                        foreach (Coordinate_Polygon thisPolygon in geoInfo.Polygons)
                            allPolygons.Add(thisPolygon);
                    }
                    if (geoInfo.Line_Count > 0)
                    {
                        foreach (Coordinate_Line thisLine in geoInfo.Lines)
                            allLines.Add(thisLine);
                    }
                    if (geoInfo.Point_Count > 0)
                    {
                        foreach (Coordinate_Point thisPoint in geoInfo.Points)
                            allPoints.Add(thisPoint);
                    }
                }
                List<abstract_TreeNode> pages = CurrentItem.Divisions.Physical_Tree.Pages_PreOrder;
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
                                allPolygons.Add(thisPolygon);
                            }
                        }
                        if (geoInfo2.Line_Count > 0)
                        {
                            foreach (Coordinate_Line thisLine in geoInfo2.Lines)
                            {
                                allLines.Add(thisLine);
                            }
                        }
                        if (geoInfo2.Point_Count > 0)
                        {
                            foreach (Coordinate_Point thisPoint in geoInfo2.Points)
                            {
                                allPoints.Add(thisPoint);
                            }
                        }
                    }
                }


                // Add all the polygons now
                if ((allPolygons.Count > 0) && (allPolygons[0].Edge_Points_Count > 1))
                {
                    areas_shown = true;

                    // Determine a base URL for pointing for any polygons that correspond to a page sequence
                    string currentViewerCode = CurrentMode.ViewerCode;
                    CurrentMode.ViewerCode = "XXXXXXXX";
                    string redirect_url = UrlWriterHelper.Redirect_URL(CurrentMode);;
                    CurrentMode.ViewerCode = currentViewerCode;

                    // Add each polygon 
                    foreach (Coordinate_Polygon itemPolygon in allPolygons)
                    {
                        // Determine if this polygon is within the search
                        bool in_coordinates_search = false;
                        if ((validCoordinateSearchFound) && (!googleItemSearch))
                        {
                            // Was this a point search or area search?
                            if ((providedMaxLong == providedMinLong) && (providedMaxLat == providedMinLat))
                            {
                                // Check this point
                                if (itemPolygon.is_In_Bounding_Box(providedMaxLat, providedMaxLong))
                                {
                                    in_coordinates_search = true;
                                }
                            }
                            else
                            {
                                // Chieck this area search
                                if (itemPolygon.is_In_Bounding_Box(providedMaxLat, providedMaxLong, providedMinLat, providedMinLong))
                                {
                                    in_coordinates_search = true;
                                }
                            }
                        }

                        // Look for a link for this item
                        string link = String.Empty;
                        if ((itemPolygon.Page_Sequence > 0) && (!googleItemSearch))
                        {
                            link = redirect_url.Replace("XXXXXXXX", itemPolygon.Page_Sequence.ToString());
                        }

                        // If this is an item search, don't show labels (too distracting)
                        string label = itemPolygon.Label;
                        if (googleItemSearch)
                            label = String.Empty;

                        if (in_coordinates_search)
                        {
                            // Start to call the add polygon method
                            matchingPolygonsBuilder.AppendLine("    add_polygon([");
                            foreach (Coordinate_Point thisPoint in itemPolygon.Edge_Points)
                            {
                                matchingPolygonsBuilder.AppendLine("      new google.maps.LatLng(" + thisPoint.Latitude + ", " + thisPoint.Longitude + "),");
                            }
                            matchingPolygonsBuilder.Append("      new google.maps.LatLng(" + itemPolygon.Edge_Points[0].Latitude + ", " + itemPolygon.Edge_Points[0].Longitude + ")],");
                            matchingPolygonsBuilder.AppendLine("true, '" + label + "', '" + link + "' );");

                            // Also add to the list of matching titles
                            matchingTilesList.Add("<a href=\"" + link + "\">" + itemPolygon.Label + "</a>");
                        }
                        else
                        {
                            // Start to call the add polygon method
                            mapBuilder.AppendLine("    add_polygon([");
                            foreach (Coordinate_Point thisPoint in itemPolygon.Edge_Points)
                            {
                                mapBuilder.AppendLine("      new google.maps.LatLng(" + thisPoint.Latitude + ", " + thisPoint.Longitude + "),");
                            }

                            mapBuilder.Append("      new google.maps.LatLng(" + itemPolygon.Edge_Points[0].Latitude + ", " + itemPolygon.Edge_Points[0].Longitude + ")],");

                            // If just one polygon, still show the red outline
                            if ( allPolygons.Count == 1)
                            {
                                mapBuilder.AppendLine("true, '', '" + link + "' );");
                            }
                            else
                            {
                                mapBuilder.AppendLine("false, '" + label + "', '" + link + "' );");
                            }
                        }
                    }

                    // Add any matching polygons last
                    mapBuilder.Append(matchingPolygonsBuilder.ToString());
                }

                // Draw all the single points 
                if ( allPoints.Count > 0)
                {
                    points_shown = true;
                    for (int point = 0; point < allPoints.Count; point++)
                    {
                        mapBuilder.AppendLine("    add_point(" + allPoints[point].Latitude + ", " + allPoints[point].Longitude + ", '" + allPoints[point].Label + "' );");
                    }
                }

                // If this was a point search, also add the point
                if (validCoordinateSearchFound)
                {
                    if ((providedMaxLat == providedMinLat) && (providedMaxLong == providedMinLong))
                    {
                        search_type = "point";
                        mapBuilder.AppendLine("    add_selection_point(" + providedMaxLat + ", " + providedMaxLong + ", 8 );");
                    }
                }

                // Add the searchable, draggable polygon last (if in search mode)
                if ((validCoordinateSearchFound) && (googleItemSearch))
                {
                    if ((providedMaxLat != providedMinLat) || (providedMaxLong != providedMinLong))
                    {
                        mapBuilder.AppendLine("    add_selection_rectangle(" + providedMaxLat + ", " + providedMaxLong + ", " + providedMinLat + ", " + providedMinLong + " );");
                    }
                }

                // Add the map key
                if (!googleItemSearch)
                {
                    mapBuilder.AppendLine("    add_key('" + search_type + "', " + areas_shown.ToString().ToLower() + ", " + points_shown.ToString().ToLower() + ", '" + areas_name + "');");
                }

                // Zoom appropriately
                mapBuilder.AppendLine(matchingPolygonsBuilder.Length > 0 ? "    zoom_to_selected();" : "    zoom_to_bounds();");
            }


            mapBuilder.AppendLine("  }");
            mapBuilder.AppendLine("  //]]>");
            mapBuilder.AppendLine("</script>");
        }

        /// <summary> Writes the google map script to display the spatial coordinates and zoom correctly upon page load </summary>
        /// <param name="Output"> Output stream to write the script to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Google_Map_Scripts(TextWriter Output, Custom_Tracer Tracer )
        {
            if (CurrentMode.ViewerCode == "mapsearch")
                googleItemSearch = true;

            Tracer.Add_Trace("goole_map_itemviewer.Write_HTML", "Adding google map instructions as script");

            Output.WriteLine(mapBuilder.ToString());
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (CurrentMode.ViewerCode == "mapsearch")
                googleItemSearch = true;

            if (Tracer != null)
            {
                Tracer.Add_Trace("Google_Map_ItemViewer.Write_Main_Viewer_Section", "");
            }

            Output.WriteLine("        <!-- GOOGLE MAP VIEWER OUTPUT -->" + Environment.NewLine);

            if (( allPolygons.Count > 0 ) || ( allPoints.Count > 0 ) || ( allLines.Count > 0 ))
            {

                // If there is a coordinate search here
                if (( allPolygons.Count > 1) &&
                    ((CurrentMode.Coordinates.Length > 0) && (matchingTilesList != null) || (googleItemSearch)))
                {
                    if (googleItemSearch)
                    {
                        // Compute the redirect stem to use
                        string redirect_stem = CurrentMode.BibID + "/" + CurrentMode.VID + "/map";
                        if (CurrentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                            redirect_stem = "l/" + redirect_stem;

                        // Set some constants
                        const string SEARCH_BUTTON_TEXT = "Search";
                        const string FIND_BUTTON_TEXT = "Find Address";
                        string script_action_name = "map_item_search_sobekcm('" + redirect_stem + "');";

                        Output.WriteLine("    <td style=\"text-align:left\">");
                        Output.WriteLine("      <ol>");
                        Output.WriteLine(
                            "        <li>Use the <i>Select Area</i> button below to draw a search box on the map or enter an address and press <i>Find Address</i>.</li>");
                        Output.WriteLine("        <li>Press the <i>Search</i> button to see results</li>");
                        Output.WriteLine("      </ol>");
                        Output.WriteLine("        <div class=\"map_address_div\">");
                        Output.WriteLine("          <label for=\"AddressTextBox\">Address:</label> &nbsp; ");
                        Output.WriteLine(
                            "          <input name=\"AddressTextBox\" type=\"text\" id=\"AddressTextBox\" class=\"MapAddressBox_initial\" value=\"Enter address ( i.e., 12 Main Street, Gainesville Florida )\" onfocus=\"enter_address_box(this);\" onblur=\"leave_address_box(this);\" onkeypress=\"address_box_changed(this);\" onchange=\"address_box_changed(this);\" /> &nbsp; ");
                        Output.WriteLine("          <input type=\"button\" name=\"findButton\" value=\"" + FIND_BUTTON_TEXT +
                                          "\" id=\"findButton\" class=\"SobekSearchButton\" onclick=\"map_address_geocode();\" /> &nbsp; ");
                        Output.WriteLine("          <input type=\"button\" name=\"searchButton\" value=\"" +
                                          SEARCH_BUTTON_TEXT +
                                          "\" id=\"searchButton\" class=\"SobekSearchButton\" onclick=\"" +
                                          script_action_name + "\" />");
                        Output.WriteLine("        </div>");
                        Output.WriteLine("         <input name=\"Textbox1\" type=\"hidden\" id=\"Textbox1\" value=\"" +
                                          providedMaxLat.ToString() + "\" />");
                        Output.WriteLine("         <input name=\"Textbox2\" type=\"hidden\" id=\"Textbox2\" value=\"" +
                                          providedMaxLong.ToString() + "\" />");
                        Output.WriteLine("         <input name=\"Textbox3\" type=\"hidden\" id=\"Textbox3\" value=\"" +
                                          providedMinLat.ToString() + "\" />");
                        Output.WriteLine("         <input name=\"Textbox4\" type=\"hidden\" id=\"Textbox4\" value=\"" +
                                          providedMaxLong.ToString() + "\" />");
                        Output.WriteLine("    </td>");
                    }
                    else
                    {
                        if (matchingTilesList == null || matchingTilesList.Count == 0)
                        {
                            Output.WriteLine("          <td align=\"center\">");
                            Output.WriteLine(
                                "            There were no matches within this item for your geographic search. &nbsp; ");
                            string currentModeViewerCode = CurrentMode.ViewerCode;
                            CurrentMode.ViewerCode = "mapsearch";
                            Output.WriteLine("            ( <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) +
                                              "\">Modify item search</a> )");
                            CurrentMode.ViewerCode = currentModeViewerCode;

                            // If there was an aggregation included, we can assume that was the origin of the coordinate search,  
                            // or at least that map searching is allowed for that collection
                            if (CurrentMode.Aggregation.Length > 0)
                            {
                                Output.WriteLine("            <br /><br />");
                                CurrentMode.Mode = Display_Mode_Enum.Results;
                                CurrentMode.Search_Type = Search_Type_Enum.Map;
                                CurrentMode.Result_Display_Type = Result_Display_Type_Enum.Map;
                                if ((providedMinLat > 0) && (providedMinLong > 0) && (providedMaxLat != providedMinLat) &&
                                    (providedMaxLong != providedMinLong))
                                {
                                    CurrentMode.Search_String = providedMaxLat.ToString() + "," + providedMaxLong + "," +
                                                                providedMinLat + "," + providedMinLong;
                                }
                                else
                                {
                                    CurrentMode.Search_String = providedMaxLat.ToString() + "," + providedMaxLong;
                                }
                                Output.WriteLine("            <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) +
                                                  "\">Click here to search other items in the current collection</a><br />");
                                CurrentMode.Mode = Display_Mode_Enum.Item_Display;
                            }
                            Output.WriteLine("          </td>" + Environment.NewLine + "        </tr>");
                        }
                        else
                        {
                            string modify_item_search = "Modify item search";
                            const string ZOOM_EXTENT = "Zoom to extent";
                            const string ZOOM_MATCHES = "Zoom to matches";
                            if (CurrentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Aerial)
                                modify_item_search = "Modify search within flight";

                            Output.WriteLine("          <td style=\"vertical-align: left\">");
							Output.WriteLine("            <table id=\"sbkGmiv_ResultsTable\">");
							
                            Output.WriteLine("              <tr>");
							Output.WriteLine("                <td style=\"width:50px\">&nbsp;</td>");
                            Output.WriteLine(
								"                <td colspan=\"4\">The following results match your geographic search:</td>"); //  and also appear on the navigation bar to the left
                            Output.WriteLine("              </tr>");

                            int column = 0;
                            bool first_row = true;
                            string url_options = UrlWriterHelper.URL_Options(CurrentMode);
                            string urlOptions1 = String.Empty;
                            string urlOptions2 = String.Empty;
                            if (url_options.Length > 0)
                            {
                                urlOptions1 = "?" + url_options;
                                urlOptions2 = "&" + url_options;
                            }
                            foreach (string thisResult in matchingTilesList)
                            {
                                // Start this row, as it is needed
                                if (column == 0)
                                {
                                    Output.WriteLine("              <tr>");
                                    if (first_row)
                                    {
                                        Output.WriteLine("                <td style=\"width:50px\">&nbsp;</td>");
										Output.WriteLine("                <td style=\"width:50px\">&nbsp;</td>");
                                        first_row = false;
                                    }
                                    else
                                    {
                                        Output.WriteLine("                <td colspan=\"2\">&nbsp;</td>");
                                    }
                                }

                                // Add the information for this tile
								Output.WriteLine("                <td style=\"width:80px\">" +
                                                  thisResult.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>",
                                                                                                         urlOptions1).
                                                      Replace("<%&URLOPTS%>", urlOptions2) + "</td>");
                                column++;

                                // If this was the last column, end it
                                if (column >= 3)
                                {
                                    Output.WriteLine("              </tr>");
                                    column = 0;
                                }
                            }

                            // If a row was started, finish it
                            if (column > 0)
                            {
                                while (column < 3)
                                {
									Output.WriteLine("                <td style=\"width:80px\">&nbsp;</td>");
                                    column++;
                                }
                                Output.WriteLine("              </tr>");
                            }

                            // Add a horizontal line here
                            Output.WriteLine("              <tr><td></td><td style=\"background-color:#cccccc\" colspan=\"4\"></td></tr>");

                            // Also, add the navigation links 
                            Output.WriteLine("              <tr>");
                            Output.WriteLine("                <td>&nbsp;</td>");
                            Output.WriteLine("                <td colspan=\"4\">");
                            Output.WriteLine("                  <a href=\"\" onclick=\"return zoom_to_bounds();\">" +
                                              ZOOM_EXTENT + "</a> &nbsp; &nbsp; &nbsp;  &nbsp; &nbsp; ");
                            if (matchingTilesList.Count > 0)
                            {
                                Output.WriteLine("                  <a href=\"\" onclick=\"return zoom_to_selected();\">" +
                                                  ZOOM_MATCHES + "</a> &nbsp; &nbsp; &nbsp;  &nbsp; &nbsp; ");
                            }



                            // Add link to modify this item search
                            string currentModeViewerCode = CurrentMode.ViewerCode;
                            CurrentMode.ViewerCode = "mapsearch";
                            Output.WriteLine("                  <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">" +
                                              modify_item_search + "</a> &nbsp; &nbsp; &nbsp;  &nbsp; &nbsp; ");
                            CurrentMode.ViewerCode = currentModeViewerCode;

                            // Add link to search entire collection
                            if (CurrentMode.Aggregation.Length > 0)
                            {
                                CurrentMode.Mode = Display_Mode_Enum.Results;
                                CurrentMode.Search_Type = Search_Type_Enum.Map;
                                CurrentMode.Result_Display_Type = Result_Display_Type_Enum.Map;
                                if ((providedMinLat > 0) && (providedMinLong > 0) && (providedMaxLat != providedMinLat) && (providedMaxLong != providedMinLong))
                                {
                                    CurrentMode.Search_String = providedMaxLat.ToString() + "," + providedMaxLong + "," + providedMinLat + "," + providedMinLong;
                                }
                                else
                                {
                                    CurrentMode.Search_String = providedMaxLat.ToString() + "," + providedMaxLong;
                                }

                                if (CurrentMode.Aggregation == "aerials")
                                    Output.WriteLine("                  <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Search all flights</a><br />");
                                else
                                    Output.WriteLine("                  <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Search entire collection</a><br />");

                                CurrentMode.Mode = Display_Mode_Enum.Item_Display;
                            }

                            Output.WriteLine("                </td>");
                            Output.WriteLine("              </tr>");
                            Output.WriteLine("            </table>");
                            Output.WriteLine("          </td>");
                            Output.WriteLine("        </tr>");
                        }
                    }
                }
                else
                {
                    // Start the citation table
                    string title = "Map It!";
                    if ( allPolygons.Count == 1)
                    {
                        title = allPolygons[0].Label;
                    }
                }
            }

	        Output.WriteLine("        <tr>");
			Output.WriteLine("          <td class=\"SobekCitationDisplay\">");
            Output.WriteLine("            <div id=\"sbkGmiv_MapDiv\"></div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        <!-- END GOOGLE MAP VIEWER OUTPUT -->");
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "load();"));
        }
    }
}
