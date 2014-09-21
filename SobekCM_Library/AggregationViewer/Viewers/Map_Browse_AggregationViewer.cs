#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the browse which displays all coordinate points present for an aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display a static browse or info page, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Map_Browse_AggregationViewer : abstractAggregationViewer
    {
        /// <summary> Constructor for a new instance of the Metadata_Browse_AggregationViewer class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Aggregation"> Current item aggregation object to display </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Map_Browse_AggregationViewer(SobekCM_Navigation_Object Current_Mode, Item_Aggregation Current_Aggregation, Custom_Tracer Tracer): base(Current_Aggregation, Current_Mode)
        {
            // Get the coordinate information
            DataTable coordinates = SobekCM_Database.Get_All_Coordinate_Points_By_Aggregation(Current_Aggregation.Code, Tracer);

            // Add the google script information
            StringBuilder scriptBuilder = new StringBuilder(10000);

            scriptBuilder.AppendLine("<script type=\"text/javascript\" src=\"http://maps.google.com/maps/api/js?sensor=false\"></script>");
            scriptBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + Current_Mode.Base_URL + "default/scripts/keydragzoom_packed.js\"></script>");
            scriptBuilder.AppendLine("<script type=\"text/javascript\">");
            scriptBuilder.AppendLine("  //<![CDATA[");
            scriptBuilder.AppendLine("  // Global values");
            scriptBuilder.AppendLine("  var map, bounds, custom_icon, info_window, last_center;");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("  // Initialize the map on load ");
            scriptBuilder.AppendLine("  function load() { ");

            string center_latitude = "27.75";
            string center_longitude = "-84";
            if (( coordinates != null ) && ( coordinates.Rows.Count == 1 ))
            {
                center_latitude = coordinates.Rows[0]["Point_Latitude"].ToString();
                center_longitude = coordinates.Rows[0]["Point_Longitude"].ToString();
            }
            scriptBuilder.AppendLine("    // Create the map and set some values");
            scriptBuilder.AppendLine("    var latlng = new google.maps.LatLng(" + center_latitude + ", " + center_longitude + ");"); 
            scriptBuilder.AppendLine("    var myOptions = { zoom: 7, center: latlng, mapTypeId: google.maps.MapTypeId.TERRAIN, streetViewControl: false  };");
			scriptBuilder.AppendLine("    map = new google.maps.Map(document.getElementById('sbkMbav_MapDiv'), myOptions);");
            scriptBuilder.AppendLine("    map.enableKeyDragZoom();");

            if (( coordinates != null ) && ( coordinates.Rows.Count > 0 ))
            {
                scriptBuilder.AppendLine();
                scriptBuilder.AppendLine("    // Create the custom icon / marker image");
                scriptBuilder.AppendLine("    var iconSize = new google.maps.Size(11, 11);");
                scriptBuilder.AppendLine("    var iconAnchor = new google.maps.Point(5, 5);");
                scriptBuilder.AppendLine("    var pointer_image = '" + currentMode.Base_URL + "/default/images/map_point.png';");
                scriptBuilder.AppendLine("    custom_icon = new google.maps.MarkerImage(pointer_image, iconSize, null, iconAnchor);");

                scriptBuilder.AppendLine();
                scriptBuilder.AppendLine("    // Create the bounds");
                scriptBuilder.AppendLine("    bounds = new google.maps.LatLngBounds();");

                scriptBuilder.AppendLine();
                scriptBuilder.AppendLine("    // Create the info window for display");
                scriptBuilder.AppendLine("    info_window = new google.maps.InfoWindow();");
                scriptBuilder.AppendLine("    google.maps.event.addListener(info_window, 'closeclick', function() { if (last_center != null) { map.panTo(last_center); last_center = null; } });");

                scriptBuilder.AppendLine();
                scriptBuilder.AppendLine("    // Add all the points");
                string last_latitude = coordinates.Rows[0]["Point_Latitude"].ToString();
                string last_longitude = coordinates.Rows[0]["Point_Longitude"].ToString();
                List<DataRow> bibids_in_this_point = new List<DataRow>();
                foreach( DataRow thisRow in coordinates.Rows )
                {
                    string latitude = thisRow["Point_Latitude"].ToString();
                    string longitude = thisRow["Point_Longitude"].ToString();

                    // Is this upcoming point new?
                    if ((latitude != last_latitude) || (longitude != last_longitude))
                    {
                        // Write the last point, if there was one
                        if (bibids_in_this_point.Count > 0)
                        {
                            // Add the point
                            add_single_point(last_latitude, last_longitude, Current_Mode,  bibids_in_this_point, scriptBuilder);

                            // Assign this as the last value
                            last_latitude = latitude;
                            last_longitude = longitude;

                            // Clear the list of newspapers linked to this point
                            bibids_in_this_point.Clear();
                        }

                        // Start a new list and include this bib id
                        bibids_in_this_point.Add(thisRow);
                    }
                    else
                    {
                        // Add this bibid to the list
                        bibids_in_this_point.Add(thisRow);
                    }
                }

                // Write the last point, if there was one
                if (bibids_in_this_point.Count > 0)
                {
                    // Add the point
                    add_single_point(last_latitude, last_longitude, Current_Mode, bibids_in_this_point, scriptBuilder);

                    // Clear the list of newspapers linked to this point
                    bibids_in_this_point.Clear();
                }
            }
            
            scriptBuilder.AppendLine("  }");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("  // Add a single point ");
            scriptBuilder.AppendLine("  function add_point(latitude, longitude, window_content) {");
            scriptBuilder.AppendLine("    var point = new google.maps.LatLng(latitude, longitude);");
            scriptBuilder.AppendLine("    bounds.extend(point);");
            scriptBuilder.AppendLine("    var marker = new google.maps.Marker({ position: point, draggable: false, map: map, icon: custom_icon });");
            scriptBuilder.AppendLine("    google.maps.event.addListener(marker, 'click', function() { info_window.setContent(window_content); last_center = map.getCenter(); info_window.open(map, marker); });");

            scriptBuilder.AppendLine("  }");
            scriptBuilder.AppendLine("  //]]>");
            scriptBuilder.AppendLine("</script>");
            scriptIncludeName = scriptBuilder.ToString();
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Browse"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Browse; }
        }

        /// <summary> Gets flag which indicates whether to always use the home text as the secondary text </summary>
        public override bool Always_Display_Home_Text
        {
            get
            {
                return false;
            }
        }

        /// <summary>Flag indicates whether the subaggregation selection panel is displayed for this collection viewer</summary>
        /// <value> This property always returns the <see cref="Selection_Panel_Display_Enum.Never"/> enumerational value </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get
            {
                return Selection_Panel_Display_Enum.Never;
            }
        }

        private void add_single_point(string Latitude, string Longitude, SobekCM_Navigation_Object Current_Mode, List<DataRow> DatarowsInThisPoint, StringBuilder ScriptBuilder)
        {
            // Build the info window
            StringBuilder contentBuilder = new StringBuilder(2000);
			contentBuilder.Append(DatarowsInThisPoint.Count <= 2 ? "<div class=\"sbkMbav_InfoDivSmall\">" : "<div class=\"sbkMbav_InfoDiv\">");
            contentBuilder.Append("<table><tr style=\"vertical-align:top;\">");
            foreach (DataRow thisRow in DatarowsInThisPoint)
            {
                string thisBibId = thisRow[2].ToString();
                string groupTitle = thisRow[3].ToString();
                string groupThumbnail = thisBibId.Substring(0,2) + "/" + thisBibId.Substring(2,2) + "/" + thisBibId.Substring(4,2) + "/" + thisBibId.Substring(6,2) + "/" + thisBibId.Substring(8) + "/" + thisRow[4];
                int itemCount = Convert.ToInt32(thisRow[5]);
                string type = thisRow[6].ToString();

                // Calculate the link
                string link = Current_Mode.Base_URL + thisBibId;

                // Calculate the thumbnail
                string thumb = SobekCM_Library_Settings.Image_URL + groupThumbnail.Replace("\\", "/").Replace("//", "/");
                if ((thumb.ToUpper().IndexOf(".JPG") < 0) && (thumb.ToUpper().IndexOf(".GIF") < 0))
                {
                    thumb = currentMode.Default_Images_URL + "NoThumb.jpg";
                }

				contentBuilder.Append("<td><table class=\"sbkMbav_Thumb\" >");
                contentBuilder.Append("<tr><td><a href=\"" + link + "\" target=\"" + thisBibId + "\"><img src=\"" + thumb + "\" /></a></td></tr>");
				contentBuilder.Append("<tr><td><span class=\"sbkMbav_ThumbText\">" + groupTitle.Replace("'", ""));
                if (itemCount > 1)
                {
                    if (type.ToUpper() == "NEWSPAPER")
                        contentBuilder.Append("<br />( " + itemCount + " issues )");
                    else
                        contentBuilder.Append("<br />( " + itemCount + " volumes )");
                }
                contentBuilder.Append("</span></td></tr></table></td>");

            }
            contentBuilder.Append("</tr></table>");
            contentBuilder.Append("</div>");

            if (DatarowsInThisPoint.Count < 2)
                contentBuilder.Append("<center><a href=\"" + Current_Mode.Base_URL + Current_Mode.Aggregation + "/results?coord=" + Latitude + "," + Longitude + ",,\">Click here for more information about this title</a></center><br/>");
            else
                contentBuilder.Append("<center><a href=\"" + Current_Mode.Base_URL + Current_Mode.Aggregation + "/results?coord=" + Latitude + "," + Longitude + ",,\">Click here for more information about these " + DatarowsInThisPoint.Count + " titles</a></center><br/>");


            ScriptBuilder.AppendLine("    add_point( " + Latitude + ", " + Longitude + ", '" + contentBuilder + "' );");
        }


        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the title of the static browse or info into the box </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Map_Browse_AggregationViewer.Add_Search_Box_HTML", "Adding HTML");
            }

			Output.WriteLine("<div class=\"sbkMbav_MainPanel\">");
			Output.WriteLine("  <table id=\"sbkMbav_MainTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <blockquote>Select a point below to view the items from or about that location.<br />Press the SHIFT button, and then drag a box on the map to zoom in.</blockquote>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>");
			Output.WriteLine("        <div id=\"sbkMbav_MapDiv\"></div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes the HTML from the static browse or info page here  </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Map_Browse_AggregationViewer.Add_Secondary_HTML", "Adding HTML");
            }

            Output.WriteLine("<div id=\"sbk_QuickTips\">");
            Output.WriteLine("  <h1>Frequently Asked Questions</h1>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li>What are the points on the map?");
            Output.WriteLine("      <p class=\"tagline\"> The points on the map correspond to material in this collection.  In general, the dot represents the source of the material.  For newspapers, the dot is the place of publication or the audience served by the newspaper title.  For photographs, this usually corresponds to the location the photograph was taken, which is usually the same as the subject matter.</p>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>How can I tell which titles are linked to each point?");
            Output.WriteLine("      <p class=\"tagline\"> An informational window will appear when you select any of the points on the map.  This window includes thumbnails and links to each item.  For more information about all the titles, you can select the link at the bottom of the window.</p>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>Can I search from this map?");
            Output.WriteLine("      <p class=\"tagline\"> To perform a search, use the MAP SEARCH function found under its own tab.</p>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>Some points do not appear to be over any particular building or location?");
            Output.WriteLine("      <p class=\"tagline\"> Many times the center point of a city is used but many times a specific landmark may be tagged.  If the point does not appear to be over any landmark, it is probably just the town center.</p>");
            Output.WriteLine("    </li>");
            Output.WriteLine("  </ul>");
            Output.WriteLine("</div>");
            Output.WriteLine("<br />");
            Output.WriteLine();
        }
    }
}
