#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Library.Navigation;

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

        /// <summary> Flag indicates if the header (with the title, group title, etc..) should be displayed </summary>
        /// <value> This always returns the value FALSE, to suppress the standard header information </value>
        public override bool Show_Header
        {
            get
            {
                return false;
            }
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
        /// <value> This is a single page viewer, so this property always returns FALSE</value>
        public override bool Show_Page_Selector
        {
            get
            {
                return false;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value -1 </value>
        public override int Viewer_Width
        {
            get
            {
                return -1;
            }
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
            string search_type = "SobekCM_Search_Type_Enum.none";
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
                                      ? "    load_search_map(6, 19.5, 3, \"map1\");"
                                      : "    load_map(6, 19.5, 3, \"map1\");");

            // Keep track of any matching tiles
            matchingTilesList = new List<string>();

            // Add the points
            if (CurrentItem != null)
            {
                // Add the search rectangle first
                if ((validCoordinateSearchFound) && (!googleItemSearch))
                {
                    if ((providedMaxLat != providedMinLat) || (providedMaxLong != providedMinLong))
                    {
                        search_type = "SobekCM_Search_Type_Enum.rectangle";
                        mapBuilder.AppendLine("    add_rectangle(" + providedMaxLat + ", " + providedMaxLong + ", " + providedMinLat + ", " + providedMinLong + ");");
                    }
                    else
                    {
                        search_type = "SobekCM_Search_Type_Enum.point";
                    }
                }

                // Build the matching polygon HTML to overlay the matches over the non-matches
                StringBuilder matchingPolygonsBuilder = new StringBuilder();

                // Add all the polygons now
                if ((CurrentItem.Bib_Info.Coordinates.Polygon_Count > 0) && (CurrentItem.Bib_Info.Coordinates.Get_Polygon(0).Edge_Points_Count > 1))
                {
                    areas_shown = true;

                    // Determine a base URL for pointing for any polygons that correspond to a page sequence
                    string currentViewerCode = CurrentMode.ViewerCode;
                    CurrentMode.ViewerCode = "XXXXXXXX";
                    string redirect_url = CurrentMode.Redirect_URL();
                    CurrentMode.ViewerCode = currentViewerCode;

                    // Add each polygon 
                    foreach (Coordinate_Polygon itemPolygon in CurrentItem.Bib_Info.Coordinates.Polygons)
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
                            if (CurrentItem.Bib_Info.Coordinates.Polygon_Count == 1)
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
                if (CurrentItem.Bib_Info.Coordinates.Point_Count > 0)
                {
                    points_shown = true;
                    for (int point = 0; point < CurrentItem.Bib_Info.Coordinates.Point_Count; point++)
                    {
                        mapBuilder.AppendLine("    add_point(" + CurrentItem.Bib_Info.Coordinates.Points[point].Latitude + ", " + CurrentItem.Bib_Info.Coordinates.Points[point].Longitude + ", '" + CurrentItem.Bib_Info.Coordinates.Points[point].Label + "' );");
                    }
                }

                // If this was a point search, also add the point
                if (validCoordinateSearchFound)
                {
                    if ((providedMaxLat == providedMinLat) && (providedMaxLong == providedMinLong))
                    {
                        search_type = "SobekCM_Search_Type_Enum.point";
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
                    mapBuilder.AppendLine("    add_key(" + search_type + ", " + areas_shown.ToString().ToLower() + ", " + points_shown.ToString().ToLower() + ", '" + areas_name + "');");
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

        /// <summary> Adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Returns FALSE since nothing was added to the left navigational bar </returns>
        /// <remarks> For this item viewer, this method does nothing except return FALSE </remarks>
        public override bool Add_Nav_Bar_Menu_Section(PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Google_Map_ItemViewer.Add_Nav_Bar_Menu_Section", "Do nothing");
            }

            return false;
        }

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (CurrentMode.ViewerCode == "mapsearch")
                googleItemSearch = true;

            if (Tracer != null)
            {
                Tracer.Add_Trace("Google_Map_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

            // Add the HTML for the image
            Literal mainLiteral = new Literal();

            StringBuilder result = new StringBuilder("        <!-- GOOGLE MAP VIEWER OUTPUT -->" + Environment.NewLine);

            // If there is a coordinate search here
            if ((CurrentItem.Bib_Info.Coordinates.Polygon_Count > 1) &&
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

                    result.AppendLine("    <td align=\"left\" >");
                    result.AppendLine("      <ol>");
                    result.AppendLine(
                        "        <li>Use the <i>Select Area</i> button below to draw a search box on the map or enter an address and press <i>Find Address</i>.</li>");
                    result.AppendLine("        <li>Press the <i>Search</i> button to see results</li>");
                    result.AppendLine("      </ol>");
                    result.AppendLine("        <div class=\"map_address_div\">");
                    result.AppendLine("          <label for=\"AddressTextBox\">Address:</label> &nbsp; ");
                    result.AppendLine(
                        "          <input name=\"AddressTextBox\" type=\"text\" id=\"AddressTextBox\" class=\"MapAddressBox_initial\" value=\"Enter address ( i.e., 12 Main Street, Gainesville Florida )\" onfocus=\"enter_address_box(this);\" onblur=\"leave_address_box(this);\" onkeypress=\"address_box_changed(this);\" onchange=\"address_box_changed(this);\" /> &nbsp; ");
                    result.AppendLine("          <input type=\"button\" name=\"findButton\" value=\"" + FIND_BUTTON_TEXT +
                                      "\" id=\"findButton\" class=\"SobekSearchButton\" onclick=\"map_address_geocode();\" /> &nbsp; ");
                    result.AppendLine("          <input type=\"button\" name=\"searchButton\" value=\"" +
                                      SEARCH_BUTTON_TEXT +
                                      "\" id=\"searchButton\" class=\"SobekSearchButton\" onclick=\"" +
                                      script_action_name + "\" />");
                    result.AppendLine("        </div>");
                    result.AppendLine("         <input name=\"Textbox1\" type=\"hidden\" id=\"Textbox1\" value=\"" +
                                      providedMaxLat.ToString() + "\" />");
                    result.AppendLine("         <input name=\"Textbox2\" type=\"hidden\" id=\"Textbox2\" value=\"" +
                                      providedMaxLong.ToString() + "\" />");
                    result.AppendLine("         <input name=\"Textbox3\" type=\"hidden\" id=\"Textbox3\" value=\"" +
                                      providedMinLat.ToString() + "\" />");
                    result.AppendLine("         <input name=\"Textbox4\" type=\"hidden\" id=\"Textbox4\" value=\"" +
                                      providedMaxLong.ToString() + "\" />");
                    result.AppendLine("    </td>");
                }
                else
                {
                    if (matchingTilesList == null || matchingTilesList.Count == 0)
                    {
                        result.AppendLine("          <td align=\"center\">");
                        result.AppendLine(
                            "            There were no matches within this item for your geographic search. &nbsp; ");
                        string currentModeViewerCode = CurrentMode.ViewerCode;
                        CurrentMode.ViewerCode = "mapsearch";
                        result.AppendLine("            ( <a href=\"" + CurrentMode.Redirect_URL() +
                                          "\">Modify item search</a> )");
                        CurrentMode.ViewerCode = currentModeViewerCode;

                        // If there was an aggregation included, we can assume that was the origin of the coordinate search,  
                        // or at least that map searching is allowed for that collection
                        if (CurrentMode.Aggregation.Length > 0)
                        {
                            result.AppendLine("            <br /><br />");
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
                            result.AppendLine("            <a href=\"" + CurrentMode.Redirect_URL() +
                                              "\">Click here to search other items in the current collection</a><br />");
                            CurrentMode.Mode = Display_Mode_Enum.Item_Display;
                        }
                        result.AppendLine("          </td>" + Environment.NewLine + "        </tr>");
                    }
                    else
                    {
                        string modify_item_search = "Modify item search";
                        const string ZOOM_EXTENT = "Zoom to extent";
                        const string ZOOM_MATCHES = "Zoom to matches";
                        if (CurrentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Aerial)
                            modify_item_search = "Modify search within flight";

                        result.AppendLine("          <td align=\"left\">");
                        result.AppendLine("            <table width=\"700px\" >");
                        result.AppendLine("              <tr>");
                        result.AppendLine("                <td width=\"50px\">&nbsp;</td>");
                        result.AppendLine(
                            "                <td colspan=\"4\">The following results match your geographic search and also appear on the navigation bar to the left:</td>");
                        result.AppendLine("              </tr>");

                        int column = 0;
                        bool first_row = true;
                        string url_options = CurrentMode.URL_Options();
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
                                result.AppendLine("              <tr>");
                                if (first_row)
                                {
                                    result.AppendLine("                <td width=\"50px\">&nbsp;</td>");
                                    result.AppendLine("                <td width=\"50px\">&nbsp;</td>");
                                    first_row = false;
                                }
                                else
                                {
                                    result.AppendLine("                <td colspan=\"2\">&nbsp;</td>");
                                }
                            }

                            // Add the information for this tile
                            result.AppendLine("                <td  width=\"80px\">" +
                                              thisResult.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>",
                                                                                                     urlOptions1).
                                                  Replace("<%&URLOPTS%>", urlOptions2) + "</td>");
                            column++;

                            // If this was the last column, end it
                            if (column >= 3)
                            {
                                result.AppendLine("              </tr>");
                                column = 0;
                            }
                        }

                        // If a row was started, finish it
                        if (column > 0)
                        {
                            while (column < 3)
                            {
                                result.AppendLine("                <td  width=\"80px\">&nbsp;</td>");
                                column++;
                            }
                            result.AppendLine("              </tr>");
                        }

                        // Add a horizontal line here
                        result.AppendLine("              <tr><td></td><td bgcolor=\"#cccccc\" colspan=\"4\"></td></tr>");

                        // Also, add the navigation links 
                        result.AppendLine("              <tr>");
                        result.AppendLine("                <td>&nbsp;</td>");
                        result.AppendLine("                <td colspan=\"4\">");
                        result.AppendLine("                  <a href=\"\" onclick=\"return zoom_to_bounds();\">" +
                                          ZOOM_EXTENT + "</a> &nbsp; &nbsp; &nbsp;  &nbsp; &nbsp; ");
                        if (matchingTilesList.Count > 0)
                        {
                            result.AppendLine("                  <a href=\"\" onclick=\"return zoom_to_selected();\">" +
                                              ZOOM_MATCHES + "</a> &nbsp; &nbsp; &nbsp;  &nbsp; &nbsp; ");
                        }



                        // Add link to modify this item search
                        string currentModeViewerCode = CurrentMode.ViewerCode;
                        CurrentMode.ViewerCode = "mapsearch";
                        result.AppendLine("                  <a href=\"" + CurrentMode.Redirect_URL() + "\">" +
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
                                result.AppendLine("                  <a href=\"" + CurrentMode.Redirect_URL() +"\">Search all flights</a><br />");
                            else
                                result.AppendLine("                  <a href=\"" + CurrentMode.Redirect_URL() +"\">Search entire collection</a><br />");

                            CurrentMode.Mode = Display_Mode_Enum.Item_Display;
                        }

                        result.AppendLine("                </td>");
                        result.AppendLine("              </tr>");
                        result.AppendLine("            </table>");
                        result.AppendLine("          </td>");
                        result.AppendLine("        </tr>");
                    }
                }
            }
            else
            {
                // Start the citation table
                string title = "Map It!";
                if (CurrentItem.Bib_Info.Coordinates.Polygon_Count == 1)
                {
                    title = CurrentItem.Bib_Info.Coordinates.Get_Polygon(0).Label;
                }

                result.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\"><b>" + title +"</b></span></td>");
                result.AppendLine( "        </tr>");
            }

            result.AppendLine("        <tr>" + Environment.NewLine + "          <td class=\"SobekCitationDisplay\">");
            result.AppendLine("            <div id=\"map1\" style=\"width: 800px; height: 600px\"></div>");
            result.AppendLine("          </td>");
            result.AppendLine("        <!-- END GOOGLE MAP VIEWER OUTPUT -->");

            mainLiteral.Text = result.ToString();
            placeHolder.Controls.Add(mainLiteral);
        }
    }
}
