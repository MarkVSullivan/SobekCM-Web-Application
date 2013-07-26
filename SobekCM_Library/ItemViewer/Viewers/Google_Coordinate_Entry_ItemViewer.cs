using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Windows.Forms;
using SobekCM.Library.HTML;
using SobekCM.Library.Users;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SobekCM.Resource_Object;

namespace SobekCM.Library.ItemViewer.Viewers
{

    /// <summary> Class to allow a user to add coordinate information to 
    /// a digital resource ( map coverage, points of interest, etc.. ) </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Google_Coordinate_Entry_ItemViewer : abstractItemViewer
    {
        private bool googleItemSearch;
        private StringBuilder mapBuilder;
        private List<string> matchingTilesList;
        private double providedMaxLat;
        private double providedMaxLong;
        private double providedMinLat;
        private double providedMinLong;
        private bool validCoordinateSearchFound;

        //private static GeoSpatial_Information geoInfo;
        private static User_Object CurrentUser;
        private static SobekCM_Item CurrentItem;
        private static GeoSpatial_Information geoInfo2;

        private static GeoSpatial_Information itemGeoInfo;
        private static List<Coordinate_Polygon> itemPolygons;

        private static List<Coordinate_Polygon> allPolygons;
        List<Coordinate_Point> allPoints;
        List<Coordinate_Line> allLines;

        public Google_Coordinate_Entry_ItemViewer(User_Object Current_User, SobekCM_Item Current_Item, SobekCM_Navigation_Object Current_Mode)
        {
            CurrentUser = Current_User;
            CurrentItem = Current_Item;
            this.CurrentMode = Current_Mode;
            //geoInfo = CurrentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;

            // If there is no user, send to the login
            if (CurrentUser == null)
            {
                CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
                CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                CurrentMode.Redirect();
                return;
            }           

        }

        //parse and save incoming message 
        public static void SaveContent(String sendData)
        {
            //ensure we have a geo-spatial module in the digital resource
            GeoSpatial_Information resourceGeo = CurrentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            //if there was no geo-spatial module
            if (resourceGeo == null)
            {
                //create new geo-spatial module, if we do not already have one
                resourceGeo = new GeoSpatial_Information();
                CurrentItem.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, resourceGeo);
            }

            //create a new list of all the polygons for a resource item
            itemPolygons = new List<Coordinate_Polygon>();
            List<abstract_TreeNode> pages = CurrentItem.Divisions.Physical_Tree.Pages_PreOrder;
            for (int i = 0; i < pages.Count; i++)
            {
                abstract_TreeNode pageNode = pages[i];
                itemGeoInfo = pageNode.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                if ((itemGeoInfo != null) && (itemGeoInfo.hasData))
                {
                    if (itemGeoInfo.Polygon_Count > 0)
                    {
                        foreach (Coordinate_Polygon thisPolygon in itemGeoInfo.Polygons)
                        {
                            thisPolygon.Page_Sequence = (ushort)(i + 1);
                            itemPolygons.Add(thisPolygon);
                        }
                    }
                }
            }
            
            //get the length of incoming message
            int index = sendData.LastIndexOf("|");
            //split into array base on vertical pipes
            string[] ar = sendData.Substring(0, index).Split('|');
            //determine the save type (position 0 in array)
            string saveType = ar[0];
            //based on saveType, parse into objects

            switch (saveType)
            {
                case "item":

                    //prep incoming lat/long
                    string[] temp1 = ar[1].Split(',');
                    double temp1Lat = Convert.ToDouble(temp1[0].Replace("(", ""));
                    double temp1Long = Convert.ToDouble(temp1[1].Replace(")", ""));

                    //clear previous point (if any)
                    resourceGeo.Clear_Points();

                    //add the new point 
                    resourceGeo.Add_Point(temp1Lat, temp1Long, CurrentItem.METS_Header.ObjectID);

                    break;
                case "overlay":
                    //search through existing overlays and modify if match found
                    if (itemPolygons.Count > 0)
                    {
                        foreach (Coordinate_Polygon itemPolygon in itemPolygons)
                        {
                            if (itemPolygon.Label == ar[1])
                            {
                                //prep incoming bounds
                                string[] temp2 = ar[2].Split(',');
                                itemPolygon.Clear_Edge_Points();
                                itemPolygon.Add_Edge_Point(Convert.ToDouble(temp2[0].Replace("(", "")), Convert.ToDouble(temp2[1].Replace(")", "")));
                                itemPolygon.Add_Edge_Point(Convert.ToDouble(temp2[2].Replace("(", "")), Convert.ToDouble(temp2[3].Replace(")", "")));
                                itemPolygon.Recalculate_Bounding_Box();

                                //add the rotation
                                //tempPolygon.add_Rotation(Convert.ToDouble(ar[4]));
                            }
                        }
                    }
                    else
                    {
                        //create new polygon
                        Coordinate_Polygon itemPolygon = new Coordinate_Polygon();

                        //add the label
                        itemPolygon.Label = ar[1];

                        //prep incoming bounds
                        string[] temp2 = ar[2].Split(',');
                        itemPolygon.Add_Edge_Point(Convert.ToDouble(temp2[0].Replace("(", "")), Convert.ToDouble(temp2[1].Replace(")", "")));
                        itemPolygon.Add_Edge_Point(Convert.ToDouble(temp2[2].Replace("(", "")), Convert.ToDouble(temp2[3].Replace(")", "")));
                        itemPolygon.Recalculate_Bounding_Box();
                        
                        //add the rotation
                        //tempPolygon.Add_Rotation(Convert.ToDouble(ar[4]));

                        //add the polygon to the geo info
                        itemGeoInfo.Add_Polygon(itemPolygon);
                    }
                    break;
                case "poi":
                    string savedPOIType = ar[1];
                    string savedPOIDesc = ar[2];
                    string savedPOIKML = ar[3];
                    //get specific geometry  KML Standard (not used)
                    //switch (ar[1]) {
                    //    case "marker":
                    //        string savedMarkerDesc = ar[1];
                    //        string savedMarkerCoords = ar[2];
                    //        //save
                    //        break;
                    //    case "circle":
                    //        string savedCircleDesc = ar[1];
                    //        string savedCircleCenter = ar[2];
                    //        string savedCircleRadius = ar[3];
                    //        break;
                    //    case "rectangle":
                    //        string savedRectangleDesc = ar[1];
                    //        string savedRectangleBounds = ar[2];
                    //        break;
                    //    case "polygon":
                    //        string savedPolygonDesc = ar[1];
                    //        string savedPolygonPoints = ar[2];
                    //        break;
                    //    case "polyline":
                    //        string savedPolyLineDesc = ar[1];
                    //        string savedPolyLinePoints = ar[2];
                    //        break;
                    //}
                    break;
            }
            //create inprocessing directory
            string userInProcessDirectory = CurrentUser.User_InProcess_Directory("mapwork");
            //ensure the user's process directory exists
            if (!Directory.Exists(userInProcessDirectory))
            {
                Directory.CreateDirectory(userInProcessDirectory);
            }
            //add current geo
            CurrentItem.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, itemGeoInfo);
            //save the item to the temporary location
            CurrentItem.Save_METS(userInProcessDirectory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".xml");

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
            get { return -1; }
        }

        /// <summary> Property gets the type of item viewer </summary>
        /// <value> This always returns ItemViewer_Type_Enum.Google_Coordinate_Entry </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Google_Coordinate_Entry; }
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Clear();
            Body_Attributes.Add(new Tuple<string, string>("onresize", "resizeView();"));
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
                    {
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Footer,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar
                    };
            }
        }

        /// <summary> Abstract method adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            // Start to build the response
            StringBuilder mapperBuilder = new StringBuilder();

            //page content
            mapperBuilder.AppendLine("<td>");

            //used to force doctype html5 and css3
            //mapperBuilder.AppendLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");

            //standard css
            mapperBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-ui.css\"/>");
            mapperBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-searchbox.css\"/>");

            //custom css
            mapperBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapper_Theme_Default.css\"/>");
            mapperBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapper_Layout_Default.css\"/>");
            mapperBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapper_Other.css\"/>");

            //standard js files
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-ui-1.10.1.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-migrate-1.1.1.min.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-rotate.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-knob.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-json-2.4.min\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/gmaps-infobox.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"http://maps.googleapis.com/maps/api/js?key=AIzaSyCzliz5FjUlEI9D2605b33-etBrENSSBZM&sensor=false\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=drawing\"></script>");
            
            //custom js 
            #region

            mapperBuilder.AppendLine(" ");
            mapperBuilder.AppendLine(" <script type=\"text/javascript\"> ");
            mapperBuilder.AppendLine(" ");
            //set base url var
            mapperBuilder.AppendLine(" <!-- Add Base URL Var -->");
            mapperBuilder.AppendLine(" var baseURL = \"" + CurrentMode.Base_URL + "\"; ");
            mapperBuilder.AppendLine(" ");
            //geo objects writer section 
            mapperBuilder.AppendLine(" <!-- Begin Geo Objects Writer --> ");
            mapperBuilder.AppendLine(" function initGeoObjects(){ ");
            mapperBuilder.AppendLine(" ");
            
            #endregion

            //write geo objects as js vars
            #region
            
            mapBuilder = new StringBuilder();

            // Keep track of any matching tiles
            matchingTilesList = new List<string>();

            // Add the points
            if (CurrentItem != null)
            {
                allPolygons = new List<Coordinate_Polygon>();
                allPoints = new List<Coordinate_Point>();
                allLines = new List<Coordinate_Line>();
                
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
                    geoInfo2 = pageNode.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                    if ((geoInfo2 != null) && (geoInfo2.hasData))
                    {
                        if (geoInfo2.Polygon_Count > 0)
                        {
                            foreach (Coordinate_Polygon thisPolygon in geoInfo2.Polygons)
                            {
                                thisPolygon.Page_Sequence = (ushort)(i + 1);
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
                List<string> polygonBounds = new List<string>();
                List<string> polygonURL = new List<string>();
                List<double> polygonRotation = new List<double>();
                int it = 0;
                if ((allPolygons.Count > 0) && (allPolygons[0].Edge_Points_Count > 1))
                {
                    // Add each polygon 
                    foreach (Coordinate_Polygon itemPolygon in allPolygons)
                    {
                        //get and set the bounds
                        string bounds = "new google.maps.LatLngBounds( ";
                        int localit = 0;
                        string bounds1 = "new google.maps.LatLng";
                        string bounds2 = "new google.maps.LatLng";
                        foreach (Coordinate_Point thisPoint in itemPolygon.Edge_Points)
                        {
                            if (localit == 0)
                            {
                                bounds1 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                            }
                            if (localit == 2)
                            {
                                bounds2 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                            }
                            localit++;

                        }
                        bounds += bounds2 + ", " + bounds1;
                        bounds += ")";
                        polygonBounds.Add(bounds);
                        mapperBuilder.AppendLine("      incomingOverlayBounds[" + it + "] = " + bounds + ";");

                        //add the label of the polygon
                        mapperBuilder.AppendLine("      incomingOverlayLabel[" + it + "] = \"" + itemPolygon.Label + "\";");
                        
                        //get the image url
                        List<SobekCM_File_Info> first_page_files = ((Page_TreeNode)CurrentItem.Divisions.Physical_Tree.Pages_PreOrder[it]).Files;
                        string first_page_jpeg = String.Empty;
                        foreach (SobekCM_File_Info thisFile in first_page_files)
                        {
                            if ((thisFile.System_Name.ToLower().IndexOf(".jpg") > 0) &&
                                (thisFile.System_Name.ToLower().IndexOf("thm.jpg") < 0))
                            {
                                first_page_jpeg = thisFile.System_Name;
                                break;
                            }
                        }
                        string first_page_complete_url = "\"" + CurrentItem.Web.Source_URL + "/" + first_page_jpeg + "\"";
                        //polygonURL[it] = first_page_complete_url;
                        polygonURL.Add(first_page_complete_url);
                        mapperBuilder.AppendLine("      incomingOverlaySourceURL[" + it + "] = " + polygonURL[it] + ";");

                        //get and set the rotation value
                        polygonRotation.Add(0);
                        mapperBuilder.AppendLine("      incomingOverlayRotation[" + it + "] = " + polygonRotation[it] + ";");

                        //iterate
                        it++;

                    }
                    mapperBuilder.AppendLine(" ");
                    mapperBuilder.AppendLine("      displayIncomingOverlays(); ");
                    mapperBuilder.AppendLine(" ");
                }

                // Draw all the single points 
                if (allPoints.Count > 0)
                {
                    //add each point
                    for (int point = 0; point < allPoints.Count; point++)
                    {
                        mapperBuilder.AppendLine("      incomingPointCenter[" + point + "] = new google.maps.LatLng(" + allPoints[point].Latitude + "," + allPoints[point].Longitude + "); ");
                        mapperBuilder.AppendLine("      incomingPointLabel[" + point + "] = \"" + allPoints[point].Label + "\"; ");
                    }
                    mapperBuilder.AppendLine(" ");
                    mapperBuilder.AppendLine("      displayIncomingPoints();");
                    mapperBuilder.AppendLine(" ");
                }

                mapperBuilder.AppendLine(" }");
                mapperBuilder.AppendLine(" ");
                mapperBuilder.AppendLine(" <!-- End Geo Objects Writer --> ");
                mapperBuilder.AppendLine(" ");
                mapperBuilder.AppendLine(" </script> ");
                mapperBuilder.AppendLine(" ");

            }

            #endregion

            //html page literal
            #region html page literat

            mapperBuilder.AppendLine(" <div id=\"mapper_container_message\"> ");
            mapperBuilder.AppendLine("     <div id=\"content_message\"></div> ");
            mapperBuilder.AppendLine(" </div> ");
            mapperBuilder.AppendLine(" <div id=\"mapper_container\"> ");
            mapperBuilder.AppendLine("     <div id=\"mapper_container_pane_1\"> ");
            mapperBuilder.AppendLine("             <div id=\"mapper_container_toolbar\"> ");
            mapperBuilder.AppendLine("                 <div class=\"toolbar_grouping\"> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_reset\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_toggleMapControls\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_toggleToolbox\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div class=\"toolbar_grouping\"> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_layerRoadmap\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_layerTerrain\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_layerSatellite\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_layerHybrid\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_layerCustom\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_layerReset\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div class=\"toolbar_grouping\"> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_panUp\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_panLeft\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_panReset\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_panRight\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_panDown\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div class=\"toolbar_grouping\"> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_zoomIn\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_zoomReset\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_zoomOut\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div class=\"toolbar_grouping\"> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_manageItem\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_manageOverlay\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                     <div id=\"content_toolbar_button_managePOI\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div class=\"toolbar_grouping\"> ");
            mapperBuilder.AppendLine("                     <div class=\"mapper_container_search\"> ");
            mapperBuilder.AppendLine("                         <input id=\"content_toolbar_searchField\" class=\"search\" type=\"text\" placeholder=\"\" onClick=\"this.select();\" /> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbar_searchButton\" class=\"searchActionHandle\"></div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("             </div> ");
            mapperBuilder.AppendLine("         </div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("     <div id=\"mapper_container_toolbarGrabber\"> ");
            mapperBuilder.AppendLine("         <div id=\"content_toolbarGrabber\"></div> ");
            mapperBuilder.AppendLine("     </div>     ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("     <div id=\"mapper_container_pane_2\"> ");
            mapperBuilder.AppendLine("          ");
            mapperBuilder.AppendLine("         <!--<div id=\"mapper_container_message\"> ");
            mapperBuilder.AppendLine("             <div id=\"content_message\"></div> ");
            mapperBuilder.AppendLine("         </div>--> ");
            mapperBuilder.AppendLine("          ");
            mapperBuilder.AppendLine("         <div id=\"mapper_container_toolbox\" class=\"ui-widget-content\"> ");
            mapperBuilder.AppendLine("             <div id=\"mapper_container_toolboxMinibar\"> ");
            mapperBuilder.AppendLine("                 <div id=\"content_minibar_icon\"></div>  ");
            mapperBuilder.AppendLine("                 <div id=\"content_minibar_header\"></div>  ");
            mapperBuilder.AppendLine("                 <div id=\"content_minibar_button_close\"></div>  ");
            mapperBuilder.AppendLine("                 <div id=\"content_minibar_button_maximize\"></div>  ");
            mapperBuilder.AppendLine("                 <div id=\"content_minibar_button_minimize\"></div>  ");
            mapperBuilder.AppendLine("             </div> ");
            mapperBuilder.AppendLine("             <div id=\"mapper_container_toolboxTabs\"> ");
            mapperBuilder.AppendLine("                 <div id=\"content_toolbox_tab1_header\" class=\"tab-title\"></div> ");
            mapperBuilder.AppendLine("                 <div class=\"tab\"> ");
            mapperBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
            mapperBuilder.AppendLine("                         <div id=\"mapper_container_toolbox_tab1\"> ");
            mapperBuilder.AppendLine("                             <div id=\"mapper_container_grid\"> ");
            mapperBuilder.AppendLine("                              ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerRoadmap\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerTerrain\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div class=\"x half\"></div> ");
            mapperBuilder.AppendLine("                                 <div class=\"x\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panUp\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div class=\"x\"></div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerSatellite\" class=\"x y button\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerHybrid\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div class=\"x half\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panLeft\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panReset\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panRight\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                              ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerCustom\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerReset\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div class=\"x half\"></div> ");
            mapperBuilder.AppendLine("                                 <div class=\"x\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panDown\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div class=\"x\"></div> ");
            mapperBuilder.AppendLine(" 							 ");
            mapperBuilder.AppendLine("                                 <div class=\"x y half\"></div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_reset\" class=\"x y button\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_toggleMapControls\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div class=\"x half\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_zoomIn\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_zoomReset\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_button_zoomOut\" class=\"x button\"></div> ");
            mapperBuilder.AppendLine("                              ");
            mapperBuilder.AppendLine("                             </div> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div id=\"content_toolbox_tab2_header\" class=\"tab-title\"></div> ");
            mapperBuilder.AppendLine("                 <div class=\"tab\"> ");
            mapperBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
            mapperBuilder.AppendLine("                         <div id=\"mapper_container_toolbox_tab2\"> ");
            mapperBuilder.AppendLine("                             <div id=\"content_toolbox_button_manageItem\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                             <div id=\"content_toolbox_button_manageOverlay\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                             <div id=\"content_toolbox_button_managePOI\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                             <div class=\"mapper_container_search\"> ");
            mapperBuilder.AppendLine("                                 <input id=\"content_toolbox_searchField\" class=\"search\" type=\"text\" placeholder=\"\" onClick=\"this.select();\" /> ");
            mapperBuilder.AppendLine("                                 <div id=\"content_toolbox_searchButton\" class=\"searchActionHandle\"></div> ");
            mapperBuilder.AppendLine("                             </div> ");
            mapperBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                             <div id=\"searchResults_container\"> ");
            mapperBuilder.AppendLine("                                 <div id=\"searchResults_scoll_container\"> ");
            mapperBuilder.AppendLine("                                     <div id=\"searchResults_list\"></div> ");
            mapperBuilder.AppendLine("                                 </div> ");
            mapperBuilder.AppendLine("                             </div>  ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div id=\"content_toolbox_tab3_header\" class=\"tab-title\"></div> ");
            mapperBuilder.AppendLine("                 <div class=\"tab\"> ");
            mapperBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
            mapperBuilder.AppendLine("                         <div id=\"mapper_container_toolbox_tab3\"> ");
            mapperBuilder.AppendLine("                             <div id=\"content_toolbox_button_placeItem\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                             <div id=\"content_toolbox_button_itemGetUserLocation\" class=\"button\"></div>   ");
            mapperBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                             <textarea id=\"content_toolbox_posItem\" class=\"tab-field\" rows=\"2\" cols=\"24\" placeholder=\"Selected Lat/Long\"></textarea> ");
            mapperBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                             <textarea id=\"content_toolbox_rgItem\" class=\"tab-field\" rows=\"3\" cols=\"24\" placeholder=\"Nearest Address\"></textarea> ");
            mapperBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                             <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_saveItem\" > </div> ");
            mapperBuilder.AppendLine("                             <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_clearItem\" > </div> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div id=\"content_toolbox_tab4_header\" class=\"tab-title\"></div> ");
            mapperBuilder.AppendLine("                 <div class=\"tab\"> ");
            mapperBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_placeOverlay\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_overlayGetUserLocation\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"mapper_container_toolbox_overlayTools\"> ");
            mapperBuilder.AppendLine("                             <div id=\"rotation\"> ");
            mapperBuilder.AppendLine("                                 <div id=\"rotationKnob\"> ");
            mapperBuilder.AppendLine("                                     <input class=\"knob\" data-displayInput=\"false\" data-width=\"68\" data-step=\"1\" data-min=\"0\" data-max=\"360\" data-cursor=true data-bgColor=\"#B2B2B2\" data-fgColor=\"#111111\" data-thickness=\"0.3\" value=\"0\"> ");
            mapperBuilder.AppendLine("                                 </div> ");
            mapperBuilder.AppendLine("                                 <div id=\"mapper_container_toolbox_rotationButtons\"> ");
            mapperBuilder.AppendLine("                                     <div id=\"content_toolbox_rotationCounterClockwise\" class=\"button3\"></div> ");
            mapperBuilder.AppendLine("                                     <div id=\"content_toolbox_rotationReset\" class=\"button3\"></div> ");
            mapperBuilder.AppendLine("                                     <div id=\"content_toolbox_rotationClockwise\" class=\"button3\"></div> ");
            mapperBuilder.AppendLine("                                 </div> ");
            mapperBuilder.AppendLine("                             </div> ");
            mapperBuilder.AppendLine("                             <div id=\"transparency\"> ");
            mapperBuilder.AppendLine("                                 <div id=\"overlayTransparencySlider\"></div> ");
            mapperBuilder.AppendLine("                             </div> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"overlayList_container\"> ");
            mapperBuilder.AppendLine("                             <div id=\"overlayList_scoll_container\"> ");
            mapperBuilder.AppendLine("                                 <div id=\"overlayList\"></div> ");
            mapperBuilder.AppendLine("                             </div> ");
            mapperBuilder.AppendLine("                         </div>   ");
            mapperBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                         <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_saveOverlay\" > </div> ");
            mapperBuilder.AppendLine("                         <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_clearOverlay\" > </div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div id=\"content_toolbox_tab5_header\" class=\"tab-title\"></div> ");
            mapperBuilder.AppendLine("                 <div class=\"tab\"> ");
            mapperBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_placePOI\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiGetUserLocation\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiMarker\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiCircle\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiRectangle\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiPolygon\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiLine\" class=\"button\"></div> ");
            mapperBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                         <div id=\"poiList_container\"> ");
            mapperBuilder.AppendLine("                             <div id=\"poiList_scoll_container\"> ");
            mapperBuilder.AppendLine("                                 <div id=\"poiList\"></div> ");
            mapperBuilder.AppendLine("                             </div> ");
            mapperBuilder.AppendLine("                         </div>   ");
            mapperBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
            mapperBuilder.AppendLine("                         <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_savePOI\" > </div> ");
            mapperBuilder.AppendLine("                         <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_clearPOI\" > </div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("             </div> ");
            mapperBuilder.AppendLine("         </div>     ");
            mapperBuilder.AppendLine("         <div id=\"googleMap\"></div> ");
            mapperBuilder.AppendLine("     </div> ");
            mapperBuilder.AppendLine(" </div> ");
            mapperBuilder.AppendLine(" <div id=\"debugs\"></div> ");


            #endregion

            //custom js files (load order does matter)
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/sobekcm_mapper_declarations.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/sobekcm_mapper_localization.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/sobekcm_mapper_listeners.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/sobekcm_mapper_listener_actions.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/sobekcm_mapper_gmap.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/sobekcm_mapper_utilities.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/sobekcm_mapper_other.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/gmaps-markerwithlabel-1.8.1.min.js\"></script>"); //must load after custom

            //end of custom content
            mapperBuilder.AppendLine("</td>");

            // Add the literal to the placeholder
            Literal placeHolderText = new Literal();
            placeHolderText.Text = mapperBuilder.ToString();
            placeHolder.Controls.Add(placeHolderText);

        }
    }
}
