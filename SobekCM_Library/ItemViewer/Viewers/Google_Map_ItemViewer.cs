using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{

    /// <summary> Google map item viewer prototyper, which is used to check to see if there is geo-spatial information
    /// associated with a digital resource, to create the link in the main menu, and to create the viewer itself 
    /// if the user selects that option </summary>
    public class Google_Map_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Google_Map_ItemViewer_Prototyper class </summary>
        public Google_Map_ItemViewer_Prototyper()
        {
            ViewerType = "GOOGLE_MAP";
            ViewerCode = "map";
        }

        /// <summary> Name of this viewer, which matches the viewer name from the database and 
        /// in the configuration files as well.  This is actually populate by the configuration information </summary>
        public string ViewerType { get; set; }

        /// <summary> Code for this viewer, which can also be set from the configuration information </summary>
        public string ViewerCode { get; set; }

        /// <summary> If this viewer is tied to certain files existing in the digital resource, this lists all the 
        /// possible file extensions this supports (from the configuration file usually) </summary>
        public string[] FileExtensions { get; set; }

        /// <summary> Indicates if the specified item matches the basic requirements for this viewer, or
        /// if this viewer should be ignored for this item </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer really should be included </param>
        /// <returns> TRUE if this viewer should generally be included with this item, otherwise FALSE </returns>
        public bool Include_Viewer(BriefItemInfo CurrentItem)
        {
            return (( CurrentItem.GeoSpatial != null ) && ( CurrentItem.GeoSpatial.hasData ));
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> TRUE always, since PDFs should never be shown if an item is checked out </returns>
        public bool Override_On_Checkout(BriefItemInfo CurrentItem)
        {
            return true;
        }

        /// <summary> Flag indicates if the current user has access to this viewer for the item </summary>
        /// <param name="CurrentItem"> Digital resource to see if the current user has correct permissions to use this viewer </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        /// <returns> TRUE if the user has access to use this viewer, otherwise FALSE </returns>
        public bool Has_Access(BriefItemInfo CurrentItem, User_Object CurrentUser, bool IpRestricted)
        {
            return !IpRestricted;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems, bool IpRestricted )
        {
            // Determine the label to show on the menu
            string label = "Map It!";
            if (!String.IsNullOrEmpty(CurrentRequest.Coordinates))
            {
                if (CurrentRequest.ViewerCode == "mapsearch")
                {
                    label = "Map Search";
                }
                else
                {
                    if (((CurrentItem.Images != null ) && ( CurrentItem.Images.Count > 1)) || ( String.Compare(CurrentItem.Type, "map", StringComparison.OrdinalIgnoreCase) != 0 ))
                    {
                        label = "Search Results";
                    }
                    else
                    {
                        label = "Map Coverage";
                    }
                }
            }

            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Allow the label to be implemented for this viewer from the database as an override
            BriefItem_BehaviorViewer thisViewerInfo = CurrentItem.Behaviors.Get_Viewer(ViewerCode);

            // If this is found, and has a custom label, use that 
            if ((thisViewerInfo != null) && (!String.IsNullOrWhiteSpace(thisViewerInfo.Label)))
                label = thisViewerInfo.Label;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem(label, null, null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Google_Map_ItemViewer"/> class for showing the 
        /// geographic information associated with a digital  resource within a Google map context during execution 
        /// of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Google_Map_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Google_Map_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }


    /// <summary> Item viewer displays the geographic information associated with a digital 
    /// resource within a Google map context</summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Google_Map_ItemViewer : abstractNoPaginationItemViewer
    {
        private bool googleItemSearch;
        private readonly StringBuilder mapBuilder;
        private readonly List<string> matchingTilesList;
        private readonly double providedMaxLat;
        private readonly double providedMaxLong;
        private readonly double providedMinLat;
        private readonly double providedMinLong;

        private readonly List<BriefItem_Coordinate_Polygon> allPolygons;
        private readonly List<BriefItem_Coordinate_Point> allPoints;
        private readonly List<BriefItem_Coordinate_Line> allLines;

        /// <summary> Constructor for a new instance of the Google_Map_ItemViewer class, used to display the geographic
        /// information associated with a digital resource within a Google map context</summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public Google_Map_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;
            googleItemSearch = false;


            if (CurrentRequest.ViewerCode == "mapsearch")
                googleItemSearch = true;

            // If coordinates were passed in, pull the actual coordinates out of the URL
            bool validCoordinateSearchFound = false;
            if (!String.IsNullOrEmpty(CurrentRequest.Coordinates))
            {
                string[] splitter = CurrentRequest.Coordinates.Split(",".ToCharArray());
                if (((splitter.Length > 1) && (splitter.Length < 4)) || ((splitter.Length == 4) && (splitter[2].Length == 0) && (splitter[3].Length == 0)))
                {
                    if ((Double.TryParse(splitter[0], out providedMaxLat)) && (Double.TryParse(splitter[1], out providedMaxLong)))
                        validCoordinateSearchFound = true;
                    providedMinLat = providedMaxLat;
                    providedMinLong = providedMaxLong;
                }
                else if (splitter.Length >= 4)
                {
                    if ((Double.TryParse(splitter[0], out providedMaxLat)) && (Double.TryParse(splitter[1], out providedMaxLong)) &&
                        (Double.TryParse(splitter[2], out providedMinLat)) && (Double.TryParse(splitter[3], out providedMinLong)))
                        validCoordinateSearchFound = true;
                }
            }

            mapBuilder = new StringBuilder();

            allPolygons = new List<BriefItem_Coordinate_Polygon>();
            allPoints = new List<BriefItem_Coordinate_Point>();
            allLines = new List<BriefItem_Coordinate_Line>();

            // Only continue if there actually IS a map key
            if (!String.IsNullOrWhiteSpace(UI_ApplicationCache_Gateway.Settings.System.Google_Map_API_Key))
            {
                mapBuilder.AppendLine("<script src=\"https://maps.googleapis.com/maps/api/js?key=" + UI_ApplicationCache_Gateway.Settings.System.Google_Map_API_Key + "\" type=\"text/javascript\"></script>");

                //mapBuilder.AppendLine("<script type=\"text/javascript\" src=\"http://maps.google.com/maps/api/js?sensor=false\"></script>");
                mapBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Sobekcm_Map_Search_Js + "\"></script>");
                mapBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Sobekcm_Map_Tool_Js + "\"></script>");

                // Set some values for the map key
                string search_type = "geographic";
                bool areas_shown = false;
                bool points_shown = false;
                string areas_name = "Sheet";
                if (BriefItem.Type.IndexOf("aerial", StringComparison.OrdinalIgnoreCase) >= 0)
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
                if (BriefItem != null)
                {

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
                    BriefItem_GeoSpatial geoInfo = BriefItem.GeoSpatial;
                    if ((geoInfo != null) && (geoInfo.hasData))
                    {
                        if (geoInfo.Polygon_Count > 0)
                        {
                            foreach (BriefItem_Coordinate_Polygon thisPolygon in geoInfo.Polygons)
                                allPolygons.Add(thisPolygon);
                        }
                        if (geoInfo.Line_Count > 0)
                        {
                            foreach (BriefItem_Coordinate_Line thisLine in geoInfo.Lines)
                                allLines.Add(thisLine);
                        }
                        if (geoInfo.Point_Count > 0)
                        {
                            foreach (BriefItem_Coordinate_Point thisPoint in geoInfo.Points)
                                allPoints.Add(thisPoint);
                        }
                    }

                    // Add all the polygons now
                    if ((allPolygons.Count > 0) && (allPolygons[0].Edge_Points_Count > 1))
                    {
                        areas_shown = true;

                        // Determine a base URL for pointing for any polygons that correspond to a page sequence
                        string currentViewerCode = CurrentRequest.ViewerCode;
                        CurrentRequest.ViewerCode = "XXXXXXXX";
                        string redirect_url = UrlWriterHelper.Redirect_URL(CurrentRequest);
                        ;
                        CurrentRequest.ViewerCode = currentViewerCode;

                        // Add each polygon 
                        foreach (BriefItem_Coordinate_Polygon itemPolygon in allPolygons)
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
                                foreach (BriefItem_Coordinate_Point thisPoint in itemPolygon.Edge_Points)
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
                                foreach (BriefItem_Coordinate_Point thisPoint in itemPolygon.Edge_Points)
                                {
                                    mapBuilder.AppendLine("      new google.maps.LatLng(" + thisPoint.Latitude + ", " + thisPoint.Longitude + "),");
                                }

                                mapBuilder.Append("      new google.maps.LatLng(" + itemPolygon.Edge_Points[0].Latitude + ", " + itemPolygon.Edge_Points[0].Longitude + ")],");

                                // If just one polygon, still show the red outline
                                if (allPolygons.Count == 1)
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
                    if (allPoints.Count > 0)
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
            else
            {
                // No Google Map API Key
                mapBuilder.AppendLine("<script type=\"text/javascript\">");
                mapBuilder.AppendLine("  //<![CDATA[ ");
                mapBuilder.AppendLine("  function load() {  }");
                mapBuilder.AppendLine("  //]]>");
                mapBuilder.AppendLine("</script>");
            }
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "load();"));
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkGmiv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkGmiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (CurrentRequest.ViewerCode == "mapsearch")
                googleItemSearch = true;

            if (Tracer != null)
            {
                Tracer.Add_Trace("Google_Map_ItemViewer.Write_Main_Viewer_Section", "");
            }

            Output.WriteLine("        <!-- GOOGLE MAP VIEWER OUTPUT -->" + Environment.NewLine);

            if ((allPolygons.Count > 0) || (allPoints.Count > 0) || (allLines.Count > 0))
            {

                // If there is a coordinate search here
                if ((allPolygons.Count > 1) &&
                    ((!String.IsNullOrEmpty(CurrentRequest.Coordinates)) && (matchingTilesList != null) || (googleItemSearch)))
                {
                    if (googleItemSearch)
                    {
                        // Compute the redirect stem to use
                        string redirect_stem = CurrentRequest.BibID + "/" + CurrentRequest.VID + "/map";
                        if (CurrentRequest.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
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
                            string currentModeViewerCode = CurrentRequest.ViewerCode;
                            CurrentRequest.ViewerCode = "mapsearch";
                            Output.WriteLine("            ( <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) +
                                              "\">Modify item search</a> )");
                            CurrentRequest.ViewerCode = currentModeViewerCode;

                            // If there was an aggregation included, we can assume that was the origin of the coordinate search,  
                            // or at least that map searching is allowed for that collection
                            if (CurrentRequest.Aggregation.Length > 0)
                            {
                                Output.WriteLine("            <br /><br />");
                                CurrentRequest.Mode = Display_Mode_Enum.Results;
                                CurrentRequest.Search_Type = Search_Type_Enum.Map;
                                CurrentRequest.Result_Display_Type = Result_Display_Type_Enum.Map;
                                if ((providedMinLat > 0) && (providedMinLong > 0) && (providedMaxLat != providedMinLat) &&
                                    (providedMaxLong != providedMinLong))
                                {
                                    CurrentRequest.Search_String = providedMaxLat.ToString() + "," + providedMaxLong + "," +
                                                                providedMinLat + "," + providedMinLong;
                                }
                                else
                                {
                                    CurrentRequest.Search_String = providedMaxLat.ToString() + "," + providedMaxLong;
                                }
                                Output.WriteLine("            <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) +
                                                  "\">Click here to search other items in the current collection</a><br />");
                                CurrentRequest.Mode = Display_Mode_Enum.Item_Display;
                            }
                            Output.WriteLine("          </td>" + Environment.NewLine + "        </tr>");
                        }
                        else
                        {
                            string modify_item_search = "Modify item search";
                            const string ZOOM_EXTENT = "Zoom to extent";
                            const string ZOOM_MATCHES = "Zoom to matches";
                            if (BriefItem.Type.IndexOf("aerial", StringComparison.OrdinalIgnoreCase) >= 0)
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
                            string url_options = UrlWriterHelper.URL_Options(CurrentRequest);
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
                            string currentModeViewerCode = CurrentRequest.ViewerCode;
                            CurrentRequest.ViewerCode = "mapsearch";
                            Output.WriteLine("                  <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) + "\">" +
                                              modify_item_search + "</a> &nbsp; &nbsp; &nbsp;  &nbsp; &nbsp; ");
                            CurrentRequest.ViewerCode = currentModeViewerCode;

                            // Add link to search entire collection
                            if (CurrentRequest.Aggregation.Length > 0)
                            {
                                CurrentRequest.Mode = Display_Mode_Enum.Results;
                                CurrentRequest.Search_Type = Search_Type_Enum.Map;
                                CurrentRequest.Result_Display_Type = Result_Display_Type_Enum.Map;
                                if ((providedMinLat > 0) && (providedMinLong > 0) && (providedMaxLat != providedMinLat) && (providedMaxLong != providedMinLong))
                                {
                                    CurrentRequest.Search_String = providedMaxLat.ToString() + "," + providedMaxLong + "," + providedMinLat + "," + providedMinLong;
                                }
                                else
                                {
                                    CurrentRequest.Search_String = providedMaxLat.ToString() + "," + providedMaxLong;
                                }

                                if (CurrentRequest.Aggregation == "aerials")
                                    Output.WriteLine("                  <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) + "\">Search all flights</a><br />");
                                else
                                    Output.WriteLine("                  <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) + "\">Search entire collection</a><br />");

                                CurrentRequest.Mode = Display_Mode_Enum.Item_Display;
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
                    if (allPolygons.Count == 1)
                    {
                        title = allPolygons[0].Label;
                    }
                }
            }

            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td class=\"SobekCitationDisplay\">");

            if (!String.IsNullOrWhiteSpace(UI_ApplicationCache_Gateway.Settings.System.Google_Map_API_Key))
            {
                Output.WriteLine("            <div id=\"sbkGmiv_MapDiv\"></div>");
                Output.WriteLine();
            }
            else
            {
                Output.WriteLine("            <div style=\"padding: 50px\">");
                Output.WriteLine("              <p style=\"font-weight:bold; text-size:1.1em\">ERROR: Google Maps are not enabled on this instance of SobekCM!</p>");
                Output.WriteLine("              <p>To enable them, please create a Google Map API key and enter it in the system-wide settings.</p>");
                Output.WriteLine("              <p>Information on this process can be found here: <a href=\"http://sobekrepository.org/software/config/googlemaps\" style=\"color:white;\">http://sobekrepository.org/software/config/googlemaps</a>.</p>");
                Output.WriteLine("            </div>");
            }


            Tracer.Add_Trace("goole_map_itemviewer.Write_HTML", "Adding google map instructions as script");
            Output.WriteLine(mapBuilder.ToString());
            Output.WriteLine();

            Output.WriteLine("          </td>");
            Output.WriteLine("        <!-- END GOOGLE MAP VIEWER OUTPUT -->");
        }

        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This method does nothing, since nothing is added to the place holder as a control for this item viewer </remarks>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Do nothing
        }
    }
}
