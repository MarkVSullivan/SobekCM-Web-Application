#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.Search;
using SobekCM.Core.Settings;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.Search;
using SobekCM.Library.Settings;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Results viewer shows the results which have spatial information in Google maps.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class Google_Map_ResultsViewer : abstract_ResultsViewer
    {
        private int currentResultCount;

        // Set some constants for this
        private const string LINE_COLOR = "#cccccc";
        private StringBuilder mapScriptHtml;
        private int polyCount;

        /// <summary> Constructor for a new instance of the Full_ResultsViewer class </summary>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        public Google_Map_ResultsViewer(Item_Lookup_Object All_Items_Lookup)
        {
            base.All_Items_Lookup = All_Items_Lookup;
        }

        ///// <summary> Gets the total number of results to display </summary>
        ///// <value> Since results are displayed by geographic information, this returns the number of distinct coordinates in the result set </value>
        //public override int Total_Results
        //{
        //    get
        //    {
        //        return resultTable.Spatial_Info_Count;
        //    }
        //}

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public override void Add_HTML(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Map_ResultsWriter.Add_HTML", "Rendering results in map view");
            }

            // If results are null, or no results, return empty string
            if ((Paged_Results == null) || (Results_Statistics == null) || (Results_Statistics.Total_Items <= 0))
                return;

            // Get the text search redirect stem and (writer-adjusted) base url 
            string textRedirectStem = Text_Redirect_Stem;
            string base_url = CurrentMode.Base_URL;
            if (CurrentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = CurrentMode.Base_URL + "l/";

            // Start the results
            StringBuilder resultsBldr = new StringBuilder("<br />\n");
            resultsBldr.Append("<table>\n");

            // Start to create the HTML
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<br />");
            builder.AppendLine("<table  width=\"900px\" >");

            // Set some values prior to stepping through all the coordinates to display
            polyCount = 1;
            currentResultCount = 0;
            int map_number = 1;
            string coords = String.Empty;

            // Start to create the first map html
            mapScriptHtml = new StringBuilder();
            mapScriptHtml.AppendLine("<script type=\"text/javascript\" src=\"http://maps.google.com/maps/api/js?sensor=false\"></script> ");
            mapScriptHtml.AppendLine("<script type=\"text/javascript\">");
            mapScriptHtml.AppendLine("  //<![CDATA[");
            mapScriptHtml.AppendLine("  function createMarker(map, point, icon_image) {");
            mapScriptHtml.AppendLine("    var this_icon = new GIcon(G_DEFAULT_ICON);");
            mapScriptHtml.AppendLine("    this_icon.image = icon_image;");
            mapScriptHtml.AppendLine("    markerOptions = { icon:this_icon };");
            mapScriptHtml.AppendLine("    map.addOverlay(new GMarker( point, markerOptions));");
            mapScriptHtml.AppendLine("  }");
            mapScriptHtml.AppendLine();
            mapScriptHtml.AppendLine("  function load() {");

            List<iSearch_Title_Result> titles_for_current_map = new List<iSearch_Title_Result>();


            // Step through and add each item to the result set
            // All of the item rows to be displayed together are collected first, and then 
            // the information and google map are rendered.
            foreach (iSearch_Title_Result titleResult in Paged_Results)
            {
                // If this new spatial does not match the last spatial, need to close out the last coordiante
                // and render all the HTML and map script information
                // This happens for each area, although points are lumped together
                if ((titles_for_current_map.Count > 0 ) && (titleResult.Spatial_Coordinates != coords))
                {
                    if (( titleResult.Spatial_Coordinates.Length == 0 ) || ( coords.Length == 0 ) || (titleResult.Spatial_Coordinates[0] == 'A') || (titleResult.Spatial_Coordinates[0] != coords[0]))
                    {
                        // Write the information
                        Add_Item_Info_And_Map(textRedirectStem, base_url, map_number, titles_for_current_map, MainPlaceHolder, builder);

                        // Get ready for the next item
                        map_number++;
                        titles_for_current_map.Clear();
                    }
                }

                // Add this title for the current (possibly new) map
                titles_for_current_map.Add(titleResult);

                // Just making sure the coordinate and bib id really reflect this last item 
                // before going to the next item
                coords = titleResult.Spatial_Coordinates;
            }

            // Again, check for left over collected item rows
            if (titles_for_current_map.Count > 0)
            {
                // Write the information
                Add_Item_Info_And_Map( textRedirectStem, base_url, map_number, titles_for_current_map, MainPlaceHolder, builder);
            }

            // Close out this map table 
            builder.AppendLine("  <tr><td bgcolor=\"" + LINE_COLOR + "\" colspan=\"3\"></td></tr>");
            builder.AppendLine("</table>");

            // End the map script
            mapScriptHtml.AppendLine("  }");
            mapScriptHtml.AppendLine("  //]]>");
            mapScriptHtml.AppendLine("</script>");

            // Add this to the page
            Literal writeData = new Literal {Text = builder + mapScriptHtml.ToString()};
            MainPlaceHolder.Controls.Add(writeData);
        }

        private void Add_Item_Info_And_Map(string TextRedirectStem, string BaseURL, int MapNumber, List<iSearch_Title_Result> TitlesForCurrentMap, PlaceHolder MainPlaceHolder, StringBuilder Builder)
        {
            // Set some values before iterating through the item rows
			const string VARIES_STRING = "<span style=\"color:Gray\">( varies )</span>";

            // Step through each collection of items by bib id for this coordinate and see if this is a collection of points
            bool point_collection_map = false;
            bool polygon_map = false;
            if (TitlesForCurrentMap[0].Spatial_Coordinates.Length > 0)
            {
                if (TitlesForCurrentMap[0].Spatial_Coordinates[0] == 'P')
                {
                    point_collection_map = true;
                }
                else
                {
                    polygon_map = true;
                }
            }

            // Add the map division here
            Builder.AppendLine("  <tr><td bgcolor=\"" + LINE_COLOR + "\" colspan=\"3\"></td></tr>");
            Builder.AppendLine("  <tr valign=\"top\">");

            if (point_collection_map)
            {
                Builder.AppendLine("    <td colspan=\"2\"><div id=\"map" + MapNumber + "\" style=\"width: 450px; height: 450px\"></div></td>");
                Builder.AppendLine("    <td>");
                Builder.AppendLine("      <table width=\"380px\">");
            }

            if( polygon_map )
            {
                Builder.AppendLine("    <td align=\"center\"><div id=\"map" + MapNumber + "\" style=\"width: 250px; height: 250px\"></div></td>");
                Builder.AppendLine("    <td colspan=\"2\">");
                Builder.AppendLine("      <table width=\"580px\">");

                // Put a note here about the number of matches
                if (TitlesForCurrentMap.Count > 1)
                {
                    int total_items = TitlesForCurrentMap.Sum(TitleInMap => TitleInMap.Item_Count);
                    if (total_items != TitlesForCurrentMap.Count)
                    {
                        Builder.AppendLine("        <tr><td colspan=\"3\"><span style=\"color: gray;\"><center><em>The following " + total_items + " matches in " + TitlesForCurrentMap.Count + " sets share the same coordinate information</em></center></span></td></tr>");
                    }
                    else
                    {
                        Builder.AppendLine("        <tr><td colspan=\"3\"><span style=\"color: gray;\"><center><em>The following " + total_items + " matches share the same coordinate information</em></center></span></td></tr>");
                    }
                }
            }

            if ((!point_collection_map) && (!polygon_map))
            {
                Builder.AppendLine("    <td colspan=\"3\">");
                Builder.AppendLine("      <table width=\"100%\">");

                // Put a note here about the number of matches
                if (TitlesForCurrentMap.Count > 1)
                {
                    int total_items = TitlesForCurrentMap.Sum(TitleInMap => TitleInMap.Item_Count);
                    if (total_items != TitlesForCurrentMap.Count)
                    {
                        Builder.AppendLine("        <tr><td colspan=\"3\"><span style=\"color: gray;\"><center><em>The following " + total_items + " matches in " + TitlesForCurrentMap.Count + " sets have no coordinate information</em></center></span></td></tr>");
                    }
                    else
                    {
                        Builder.AppendLine("        <tr><td colspan=\"3\"><span style=\"color: gray;\"><center><em>The following " + total_items + " matches have no coordinate information</em></center></span></td></tr>");
                    }
                }
            }


            // Now, add all the individual item information for each bib id in this map
            int titles_per_this_map = 0;
            int items_per_this_map = 0;
            string last_link = String.Empty;
            int polygons_added_to_this_map = 0;
            int coordinates_per_this_map = 1;
            string coords = String.Empty;
            foreach (iSearch_Title_Result titleResult in TitlesForCurrentMap)
            {
                // Always get the first item for things like the main link and thumbnail
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);

                // Increment the number of items/titles per this coordiante
                titles_per_this_map++;

                // If this is not the first, add a line
                if (titles_per_this_map > 1)
                {
                    if ((polygon_map) || (titleResult.Spatial_Coordinates != coords))
                    {
                        Builder.AppendLine("        <tr><td bgcolor=\"" + LINE_COLOR + "\" colspan=\"3\"></td></tr>");
                    }
                    else
                    {
                        Builder.AppendLine("        <tr><td></td><td bgcolor=\"" + LINE_COLOR + "\" colspan=\"3\"></td></tr>");
                    }
                }

                // Increment by the number of items in this collection of items 
                items_per_this_map += 1;

                // Are there multiple volumes to be displayed here?
                bool multiple = false;
                string pubdate = firstItemResult.PubDate;
                if (titleResult.Item_Count > 1)
                {
                    multiple = true;

                    // This will not include the item details, just the tree
                    Builder.AppendLine("        <tr>");

                    // If this is a point (and the first point of this coordinate) add the point information here
                    if (point_collection_map)
                    {
                        if (titleResult.Spatial_Coordinates != coords)
                        {
                            // Add the icon for the google marker
                            Builder.AppendLine("          <tr><td width=\"30\"><img src=\"" + icon_by_number(coordinates_per_this_map) + "\" /></td>");

                            // Look ahead to see if multiple items have the same coordinate
                            int index = TitlesForCurrentMap.IndexOf(titleResult);
                            int matching_titles_for_this_point = 1;
                            while ((index >= 0) && ((index + 1) < TitlesForCurrentMap.Count))
                            {
                                if (TitlesForCurrentMap[index + 1].Spatial_Coordinates == titleResult.Spatial_Coordinates)
                                {
                                    matching_titles_for_this_point++;
                                }
                                else
                                {
                                    break;
                                }
                                index++;
                            }
                            if (matching_titles_for_this_point > 1)
                            {
                                Builder.AppendLine("            <td colspan=\"2\"><span style=\"color: gray;\"><center><em>The following " + matching_titles_for_this_point + " titles have the same coordinate point</em></center></span></td>");
                                Builder.AppendLine("          </tr>");
                                Builder.AppendLine("          <tr>");
                                Builder.AppendLine("            <td>&nbsp;</td>");
                            }

                            coords = titleResult.Spatial_Coordinates;
                            coordinates_per_this_map++;
                        }
                        else
                        {
                            Builder.AppendLine("          <td>&nbsp;</td>");
                        }
                    }
                    else
                    {
                        Builder.AppendLine("          <td>&nbsp;</td>");
                    }

                    Builder.AppendLine("          <td colspan=\"2\">");

                    // Write all the collected HTML to a literal, since we will be adding a 
                    // tree view control to the web page next
                    Literal literal = new Literal {Text = Builder.ToString()};
                    MainPlaceHolder.Controls.Add(literal);
                    Builder.Remove(0, Builder.Length);

                    // Draw the tree of all matching issues
                    Add_Issue_Tree(MainPlaceHolder, titleResult, currentResultCount, TextRedirectStem, BaseURL);

                    // Finish this table in the item results view                                                       
                    Builder.AppendLine("          </td>");
                    Builder.AppendLine("        </tr>");

                    // Check if the pub date is the same for all items
                    if ((pubdate.Length > 0) && (pubdate != "-1"))
                    {
                        for (int i = 0; i < titleResult.Item_Count; i++)
                        {
                            if (titleResult.Get_Item(i).PubDate != pubdate)
                            {
                                pubdate = String.Empty;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // If this is a point (and the first point of this coordinate) add the point information here
                    if (point_collection_map)
                    {
                        if (titleResult.Spatial_Coordinates != coords)
                        {
                            // Add the icon for the google marker
                            Builder.AppendLine("          <tr><td width=\"30\"><img src=\"" + icon_by_number(coordinates_per_this_map) + "\" /></td>");

                            // Look ahead to see if multiple items have the same coordinate
                            int index = TitlesForCurrentMap.IndexOf(titleResult);
                            int matching_titles_for_this_point = 1;
                            while ((index >= 0) && ((index + 1) < TitlesForCurrentMap.Count))
                            {
                                if (TitlesForCurrentMap[index + 1].Spatial_Coordinates == titleResult.Spatial_Coordinates)
                                {
                                    matching_titles_for_this_point++;
                                }
                                else
                                {
                                    break;
                                }
                                index++;
                            }
                            if (matching_titles_for_this_point > 1)
                            {
                                Builder.AppendLine("            <td colspan=\"2\"><span style=\"color: gray;\"><center><em>The following " + matching_titles_for_this_point + " titles have the same coordinate point</em></center></span></td>");
                                Builder.AppendLine("          </tr>");
                                Builder.AppendLine("          <tr>");
                                Builder.AppendLine("            <td>&nbsp;</td>");
                            }

                            coords = titleResult.Spatial_Coordinates;
                            coordinates_per_this_map++;
                        }
                        else
                        {
                            Builder.AppendLine("          <td>&nbsp;</td>");
                        }

                        Builder.AppendLine("<td colspan=\"2\"><a href=\"" + BaseURL + titleResult.BibID.ToUpper() + "/" + firstItemResult.VID + TextRedirectStem + "\">" + firstItemResult.Title + "</a>");

                    }
                    else
                    {
                        Builder.AppendLine("            <tr><td></td><td colspan=\"2\"><a href=\"" + BaseURL + titleResult.BibID.ToUpper() + "/" + firstItemResult.VID + TextRedirectStem + "\">" + firstItemResult.Title + "</a>");

                        // Save this link, just in case it is the only area in this map
                        last_link = BaseURL + titleResult.BibID.ToUpper() + "/" + firstItemResult.VID + TextRedirectStem;
                    }
                }

                // Add the bib id and vid
                if (CurrentMode.Internal_User)
                {
                    Builder.AppendLine("            <tr height=\"10px\"><td>&nbsp;</td><td>BibID:</td><td>" + titleResult.BibID.ToUpper() + "</td></tr>");
                    if (!multiple)
                    {
                        Builder.AppendLine("            <tr height=\"10px\"><td>&nbsp;</td><td>VID:</td><td>" + firstItemResult.VID + "</td></tr>");
                    }
                }

				for (int i = 0; i < Results_Statistics.Metadata_Labels.Count; i++)
				{
					string field = Results_Statistics.Metadata_Labels[i];
					string value = titleResult.Metadata_Display_Values[i];
					Metadata_Search_Field thisField = InstanceWide_Settings_Singleton.Settings.Metadata_Search_Field_By_Name(field);
					string display_field = string.Empty;
					if (thisField != null)
						display_field = thisField.Display_Term;
					if (display_field.Length == 0)
						display_field = field.Replace("_", " ");

					if (value == "*")
					{
						Builder.AppendLine("\t\t\t\t<tr height=\"10px\"><td>&nbsp;</td><td>" + Translator.Get_Translation(display_field, CurrentMode.Language) + ":</td><td>" + VARIES_STRING + "</td></tr>");
					}
					else if (value.Trim().Length > 0)
					{
						if (value.IndexOf("|") > 0)
						{
							bool value_found = false;
							string[] value_split = value.Split("|".ToCharArray());

							foreach (string thisValue in value_split)
							{
								if (thisValue.Trim().Trim().Length > 0)
								{
									if (!value_found)
									{
										Builder.AppendLine("\t\t\t\t<tr valign=\"top\"><td>&nbsp;</td><td>" + Translator.Get_Translation(display_field, CurrentMode.Language) + ":</td><td>");
										value_found = true;
									}
									Builder.Append(System.Web.HttpUtility.HtmlEncode(thisValue) + "<br />");
								}
							}

							if (value_found)
							{
								Builder.AppendLine("</td></tr>");
							}
						}
						else
						{
							Builder.AppendLine("\t\t\t\t<tr height=\"10px\"><td>&nbsp;</td><td>" + Translator.Get_Translation(display_field, CurrentMode.Language) + ":</td><td>" + System.Web.HttpUtility.HtmlEncode(value) + "</td></tr>");
						}
					}
				}

                // Increment the row counter
                currentResultCount++;
            }

            // End this map row
            Builder.AppendLine("      </table>");
            Builder.AppendLine("    </td>");
            Builder.AppendLine("  </tr>");

            if ((point_collection_map) || (polygon_map))
            {
                // Clear the last latitude and longitude information
                double max_lat = -90;
                double max_long = -180;
                double min_lat = 90;
                double min_long = 180;

                // Now, start to add the map javascript information to the building javascript
                mapScriptHtml.AppendLine();
                mapScriptHtml.AppendLine("    var map" + MapNumber + "_center = new google.maps.LatLng(<%CENTERINFO" + MapNumber + "%>);");
                mapScriptHtml.AppendLine("    var map" + MapNumber + "_options = { zoom: <%ZOOMINFO" + MapNumber + "%>, center: map" + MapNumber + "_center, mapTypeId: google.maps.MapTypeId.ROADMAP, mapTypeControl: false, streetViewControl: false };");
                mapScriptHtml.AppendLine("    var map" + MapNumber + " = new google.maps.Map(document.getElementById(\"map" + MapNumber + "\"), map" + MapNumber + "_options);");

                // Step through each coordinate/title collection for this map
                int point_index = 1;
                coords = "A";
                foreach (iSearch_Title_Result items_per_bib in TitlesForCurrentMap)
                {
                    // Add this coordinate information to the 
                    if (items_per_bib.Spatial_Coordinates.Length > 0)
                    {
                        string[] coords_splitter = items_per_bib.Spatial_Coordinates.Split("|,".ToCharArray());

                        // If this was a point, add this point
                        if (items_per_bib.Spatial_Coordinates[0] == 'P')
                        {
                            if (items_per_bib.Spatial_Coordinates != coords)
                            {
                                coords = items_per_bib.Spatial_Coordinates;

                                // Add the marker to the map script
                                mapScriptHtml.AppendLine("    var marker" + MapNumber + "_" + point_index + " = new google.maps.Marker({ position: new google.maps.LatLng(" + coords_splitter[1] + ", " + coords_splitter[2] + "), map: map" + MapNumber + ", icon: \"" + icon_by_number(point_index) + "\" });");
                                point_index++;

                                // Check the new boundaries
                                check_boundaries(coords_splitter[1], coords_splitter[2], ref max_lat, ref max_long, ref min_lat, ref min_long);
                            }
                        }
                        else
                        {
                            if (items_per_bib.Spatial_Coordinates != coords)
                            {
                                coords = items_per_bib.Spatial_Coordinates;
                                if (coords_splitter.Length == 5)
                                {
                                    mapScriptHtml.AppendLine("    var polygon" + polyCount + "_outline = [ new google.maps.LatLng(" + coords_splitter[1] + "," + coords_splitter[2] + "), new google.maps.LatLng(" + coords_splitter[1] + "," + coords_splitter[4] + "), new google.maps.LatLng(" + coords_splitter[3] + "," + coords_splitter[4] + "),  new google.maps.LatLng(" + coords_splitter[3] + "," + coords_splitter[2] + "), new google.maps.LatLng(" + coords_splitter[1] + "," + coords_splitter[2] + ")];");
                                    mapScriptHtml.AppendLine("    var polygon" + polyCount + " = new google.maps.Polygon({ paths: polygon" + polyCount + "_outline, strokeColor: \"#f33f00\", strokeOpacity: 1, strokeWeight: 5, fillColor: \"#ff0000\", fillOpacity: 0.2 });");
                                    mapScriptHtml.AppendLine("    polygon" + polyCount + ".setMap(map" + MapNumber + ");");

                                    check_boundaries(coords_splitter[1], coords_splitter[2], ref max_lat, ref max_long, ref min_lat, ref min_long);
                                    check_boundaries(coords_splitter[3], coords_splitter[4], ref max_lat, ref max_long, ref min_lat, ref min_long);
                                }
                                else
                                {
                                    bool first = true;

                                    mapScriptHtml.Append("    var polygon" + polyCount + "_outline = [ ");

                                    int point = 1;
                                    while ((point + 2) <= coords_splitter.Length)
                                    {
                                        if (!first)
                                            mapScriptHtml.Append(",");
                                        else
                                            first = false;

                                        mapScriptHtml.Append("new google.maps.LatLng(" + coords_splitter[point] + ", " + coords_splitter[point + 1] + ")");
                                        check_boundaries(coords_splitter[point], coords_splitter[point + 1], ref max_lat, ref max_long, ref min_lat, ref min_long);

                                        point += 2;
                                    }
                                    mapScriptHtml.AppendLine("];");
                                    mapScriptHtml.AppendLine("    var polygon" + polyCount + " = new google.maps.Polygon({ paths: polygon" + polyCount + "_outline, strokeColor: \"#f33f00\", strokeOpacity: 1, strokeWeight: 5, fillColor: \"#ff0000\", fillOpacity: 0.2 });");
                                    mapScriptHtml.AppendLine("    polygon" + polyCount + ".setMap(map" + MapNumber + ");");
                                }

                                // Finish the last polygon by adding the link, if there should be one
                                if ((items_per_this_map == 1) && (last_link.Length > 0))
                                {
                                    mapScriptHtml.AppendLine("    google.maps.event.addListener(polygon" + polyCount + ", 'click', function redirect" + polyCount + "() { window.location.href = \"" + last_link + "\"; }); ");
                                }
                                polyCount++;
                                polygons_added_to_this_map++;
                            }
                        }
                    }
                }


                try
                {
                    // Compute the center and zoom of the last map
                    double mid_lat = (max_lat + min_lat) / 2;
                    double mid_long = (max_long + min_long) / 2;
                    int zoom = compute_zoom(max_lat, max_long, min_lat, min_long);
                    if (coords[0] == 'A')
                        zoom--;
                    if ((polygons_added_to_this_map == 0) && (point_index <= 1))
                        zoom = 6;

                    mapScriptHtml.Replace("<%CENTERINFO" + MapNumber + "%>", mid_lat + ", " + mid_long);
                    mapScriptHtml.Replace("<%ZOOMINFO" + MapNumber + "%>", zoom.ToString());
                }
                catch
                {
                    mapScriptHtml.Replace("<%CENTERINFO" + MapNumber + "%>", "0, 0");
                    mapScriptHtml.Replace("<%ZOOMINFO" + MapNumber + "%>", "8");
                }
            }
        }

        private static int compute_zoom(double MaxLat, double MaxLong, double MinLat, double MinLong)
        {
            try
            {
                double miles_lat = 69.167 * (MaxLat - MinLat);
                double miles_long = 69.167 * (MaxLong - MinLong);
                double miles = Math.Max(miles_lat, miles_long);
                if (miles < 0.5)
                    return 16;
                if ((miles < 1) && (miles >= 0.5))
                    return 15;
                if ((miles < 2) && (miles >= 1))
                    return 14;
                if ((miles < 3) && (miles >= 2))
                    return 13;
                if ((miles < 7) && (miles >= 3))
                    return 12;
                if ((miles < 15) && (miles >= 7))
                    return 11;
                if ((miles < 30) && (miles >= 15))
                    return 10;
                if ((miles < 60) && (miles >= 30))
                    return 9;
                if ((miles < 120) && (miles >= 60))
                    return 8;
                if ((miles < 240) && (miles >= 120))
                    return 7;
                if ((miles < 480) && (miles >= 240))
                    return 6;
                if ((miles < 960) && (miles >= 480))
                    return 5;
                if ((miles < 2000) && (miles >= 960))
                    return 4;
            }
            catch
            {
                return 2;
            }

            return 2;
        }

        private static void check_boundaries(string Latitude, string Longitude, ref double MaxLat, ref double MaxLong, ref double MinLat, ref double MinLong)
        {
            double point_lat = Convert.ToDouble(Latitude);
            double point_long = Convert.ToDouble(Longitude);
            if (point_long > MaxLong)
                MaxLong = point_long;
            if (point_long < MinLong)
                MinLong = point_long;
            if (point_lat > MaxLat)
                MaxLat = point_lat;
            if (point_lat < MinLat)
                MinLat = point_lat;
        }

        private static string icon_by_number(int IconNumber)
        {
            switch (IconNumber)
            {
                case 1:
                    return "http://www.google.com/mapfiles/markerA.png";

                case 2:
                    return "http://www.google.com/mapfiles/markerB.png";

                case 3:
                    return "http://www.google.com/mapfiles/markerC.png";

                case 4:
                    return "http://www.google.com/mapfiles/markerD.png";

                case 5:
                    return "http://www.google.com/mapfiles/markerE.png";

                case 6:
                    return "http://www.google.com/mapfiles/markerF.png";

                case 7:
                    return "http://www.google.com/mapfiles/markerG.png";

                case 8:
                    return "http://www.google.com/mapfiles/markerH.png";

                case 9:
                    return "http://www.google.com/mapfiles/markerI.png";

                case 10:
                    return "http://www.google.com/mapfiles/markerJ.png";

                default:
                    return "http://www.google.com/mapfiles/markerA.png";
            }
        }

        //for some reason I cannot put this in the beta???
        public static object Process_MapSearch_Callback(string sendData)
        {
            //blank tracer
            Custom_Tracer Tracer = new Custom_Tracer();

            #region Process SendData

            //get rid of excess string 
            sendData = sendData.Replace("{\"sendData\": \"", "").Replace("{\"sendData\":\"", "");

            //validate
            if (sendData.Length == 0)
                return "";

            //get the length of incoming message
            int index1 = sendData.LastIndexOf("~", StringComparison.Ordinal);

            //split into each action message
            string[] allActions = sendData.Substring(0, index1).Split('~');

            //hold action type handle
            string actionTypeHandle = null;

            //go through each item to action and check for ovelrays and item only not pois (ORDER does matter because these will be actiond to db before pois are actiond)
            for (int i = 0; i < allActions.Length; i++)
            {
                //get the length of action message
                int index2 = allActions[i].LastIndexOf("|");
                //split into action elements
                string[] ar = allActions[i].Substring(0, index2).Split('|');
                //determine the action type handle (position 0 in array)
                actionTypeHandle = ar[0];
                //determine the action type (position 1 in array)
                string actionType = ar[1];
                //based on actionType, parse into objects
                if (actionTypeHandle == "search")
                {
                    //split aggregation incoming subset into an array
                    string[] aggregationList = ar[2].Replace("##", "|").Split('|');
                    //handle action based on type
                    switch (actionType)
                    {
                        case "aggregation":
                            Google_Map_ResultsViewer_Beta.Perform_Aggregation_Search(aggregationList, Tracer);
                            break;
                        case "bounds":
                            //HttpContext.Current.Session["MapSearchResultsKey"] = ar[6];
                            Google_Map_ResultsViewer_Beta.Perform_Coordinate_Bounds_Search(Convert.ToDouble(ar[2]), Convert.ToDouble(ar[3]), Convert.ToDouble(ar[4]), Convert.ToDouble(ar[5]));
                            break;
                        case "filter":
                            //split filterlist incoming subset into an array
                            string[] filterList = ar[3].Replace("###", "|").Split('|');
                            Google_Map_ResultsViewer_Beta.Perform_Filter_Search(filterList);
                            break;
                        case "dateTime":
                            Google_Map_ResultsViewer_Beta.Perform_DateTime_Range_Search(Convert.ToDateTime(ar[3]), Convert.ToDateTime(ar[4]));
                            break;
                        case "coordinate":
                            Google_Map_ResultsViewer_Beta.Perform_Coordinate_Bounds_Search(Convert.ToDouble(ar[2]), Convert.ToDouble(ar[3]), Convert.ToDouble(ar[4]), Convert.ToDouble(ar[5]));
                            //Map_ResultsViewer_Beta.Perform_Coordinate_Bounds_Search(aggregationList, Convert.ToDouble(ar[3]), Convert.ToDouble(ar[4]), Convert.ToDouble(ar[5]), Convert.ToDouble(ar[6]), Tracer);
                            break;
                        case "complete":
                            string[] filterList2 = ar[3].Replace("###", "|").Split('|'); //split filterlist incoming subset into an array
                            Google_Map_ResultsViewer_Beta.Perform_Complete_Search(aggregationList, filterList2, Convert.ToDateTime(ar[4]), Convert.ToDateTime(ar[5]), Convert.ToDouble(ar[6]), Convert.ToDouble(ar[7]), Convert.ToDouble(ar[8]), Convert.ToDouble(ar[9]), Tracer);
                            break;
                    }
                }
            }

            #endregion

            //return HttpContext.Current.Session["SearchResultsJSON"].ToString();
            return HttpContext.Current.Items["DSR"];
        }

    }
}
