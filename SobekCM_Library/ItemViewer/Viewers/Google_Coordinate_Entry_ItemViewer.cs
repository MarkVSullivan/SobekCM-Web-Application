using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using SobekCM.Library.HTML;
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

        List<Coordinate_Polygon> allPolygons;
        List<Coordinate_Point> allPoints;
        List<Coordinate_Line> allLines;

        #region myc#

        ////public class Google_Coordinate_Entry_ItemViewer : System.Web.UI.Page, System.Web.UI.ICallbackEventHandler    

        //protected string stockItemCoord = String.Empty;
        //protected string stockItemDesc = String.Empty;
        //protected string savedItemCoord = String.Empty;
        //protected string savedItemDesc = String.Empty;


        //public string stockOverlayBounds = String.Empty;
        //public string stockOverlaySource = String.Empty;
        //public double stockOverlayRotation = 0;
        //protected string savedOverlayBounds = String.Empty;
        //protected string savedOverlaySource = String.Empty;
        //protected string savedOverlayRotation = String.Empty; //recieving 
        //protected double savedOverlayRotationSending = 0; //sending

        //protected string callbackMessage = "0";


        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    //read the goodies at get go
        //    //ReadSavedItem();
        //    //ReadStockItem();
        //    //ReadSavedOverlay();
        //    //ReadStockOverlay();

        //    //Get the Page's ClientScript and assign it to a ClientScriptManger
        //    ClientScriptManager cm = Page.ClientScript;

        //    //Generate the callback reference
        //    string cbReference = cm.GetCallbackEventReference(this, "arg", "HandleResult", "");

        //    //Build the callback script block
        //    string cbScript = "function CallServer(arg, context){" + cbReference + ";}";

        //    //Register the block
        //    cm.RegisterClientScriptBlock(this.GetType(), "CallServer", cbScript, true);

        //}


        //public void RaiseCallbackEvent(string eventArgument)
        //{

        //    //StreamWriter output3 = new StreamWriter("U:/vs12_projects/m3/output/savedPOISet.txt", true);
        //    //output3.WriteLine(eventArgument); //poi kml
        //    //output3.Flush();
        //    //output3.Close();

        //    //This method will be called by the Client; Do your business logic here
        //    //The parameter "eventArgument" is actually the paramenter "arg" of CallServer(arg, context)

        //    int packageIndex = eventArgument.LastIndexOf("~");
        //    string[] packages = eventArgument.Substring(0, packageIndex).Split('~');

        //    //for (var i = 0; i < ar1.length; i++) {
        //    foreach (var pack in packages)
        //    {
        //        int packIndex = pack.LastIndexOf("|");
        //        string[] packs = pack.Substring(0, packIndex).Split('|');

        //        string saveType = packs[0]; //get what type of save it is

        //        switch (saveType)
        //        {

        //            case "item":
        //                savedItemCoord = packs[1];
        //                StreamWriter output1 = new StreamWriter("U:/vs12_projects/m3/output/savedItem.txt", false);
        //                output1.WriteLine(savedItemCoord);
        //                output1.Flush();
        //                output1.Close();
        //                ReadSavedItem(); //trigger a refresh of cached saved item
        //                callbackMessage = "1";
        //                break;
        //            case "overlay":
        //                savedOverlayBounds = packs[1];
        //                savedOverlaySource = packs[2];
        //                savedOverlayRotation = packs[3];
        //                //savedItem = eventArgument; //not used
        //                StreamWriter output2 = new StreamWriter("U:/vs12_projects/m3/output/savedOverlay.txt", false);
        //                output2.WriteLine(savedOverlayBounds);
        //                output2.WriteLine(savedOverlaySource);
        //                output2.WriteLine(savedOverlayRotation);
        //                output2.Flush();
        //                output2.Close();
        //                ReadSavedOverlay(); //trigger a refresh of cached saved overlay
        //                callbackMessage = "2";
        //                break;
        //            case "poi":
        //                StreamWriter output3 = new StreamWriter("U:/vs12_projects/m3/output/savedPOISet.txt", true);
        //                string stuff = DateTime.Now + "," + packs[1] + "," + packs[2] + ",\"" + packs[3] + "\"";
        //                output3.WriteLine(stuff); //poi kml
        //                output3.Flush();
        //                output3.Close();
        //                //ReadSavedPOI(); //trigger a refresh of cached saved overlay
        //                callbackMessage = "3";
        //                break;
        //        }

        //    }

        //    //GetCallbackResult(); //trigger callback
        //}

        //public string GetCallbackResult()
        //{


        //    //This is called after RaiseCallbackEvent and then sent to the client which is the
        //    //function "HandleResult" in javascript of the page

        //    //reread everything
        //    //ReadStockItem();
        //    //ReadSavedItem();
        //    //ReadStockOverlay();
        //    //ReadSavedOverlay();
        //    //ReadStockPOI();
        //    //ReadSavedPOI();

        //    return callbackMessage;
        //}

        ////unknown if I can delete these????
        //public string stockMarkerCoords = String.Empty;
        //public string stockMarkerDesc = String.Empty;


        ////read saved item from file
        //public void ReadSavedItem()
        //{
        //    string[] lines = File.ReadAllLines(@"U:/vs12_projects/m3/output/savedItem.txt");

        //    savedItemCoord = lines[0];
        //}

        ////read stock Item from file
        //public void ReadStockItem()
        //{
        //    string[] lines = File.ReadAllLines(@"U:/vs12_projects/m3/output/stockItem.txt");

        //    stockItemCoord = lines[0];
        //}

        ////read saved overlay from file
        //public void ReadSavedOverlay()
        //{
        //    string[] lines = File.ReadAllLines(@"U:/vs12_projects/m3/output/savedOverlay.txt");

        //    savedOverlayBounds = lines[0];
        //    savedOverlaySource = lines[1];
        //    savedOverlayRotationSending = Convert.ToDouble(lines[2]);
        //}

        ////read stock overlay from file
        //public void ReadStockOverlay()
        //{
        //    string[] lines = File.ReadAllLines(@"U:/vs12_projects/m3/output/stockOverlay.txt");

        //    stockOverlayBounds = lines[0];
        //    stockOverlaySource = lines[1];
        //    stockOverlayRotation = Convert.ToDouble(lines[2]);
        //}

        #endregion
        
        /// <summary> Constructor for a new instance of the Google_Coordinate_Entry_ItemViewer class </summary>
        public Google_Coordinate_Entry_ItemViewer()
        {
            // Empty for now
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
            Body_Attributes.Add(new Tuple<string, string>("onload", "load();"));
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
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu
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
            
            //css
            mapperBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-ui.css\"/>");
            mapperBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-searchbox.css\"/>");
            mapperBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapper.css\"/>");

            //keep in here for testing
            //string mapperHTMLFile = @""+ CurrentMode.Base_URL + "default/mapper.txt";
            //string[] lines = File.ReadAllLines(@"http://hlmatt.com/uf/mapper.txt");
            //string path = @"U:\vs12_projects\git\SobekCM\default\mapper.txt";
            //string[] lines = File.ReadAllLines(path);
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    mapperBuilder.AppendLine(Convert.ToString(lines[i]));
            //}

            //js
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-1.9.1.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-ui-1.10.1.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-migrate-1.1.1.min.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-rotate.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/jquery-knob.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/gmaps-infobox.js\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"http://maps.googleapis.com/maps/api/js?key=AIzaSyCzliz5FjUlEI9D2605b33-etBrENSSBZM&sensor=false\"></script>");
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=drawing\"></script>");
            
            //add geo objects 
            mapperBuilder.AppendLine("");
            mapperBuilder.AppendLine("<!-- Begin Geo Objects Writer -->");
            mapperBuilder.AppendLine("<script type=\"text/javascript\">");
            mapperBuilder.AppendLine("   var incomingOverlayBounds = [];");    //may not need to declare
            mapperBuilder.AppendLine("   var incomingOverlaySourceURL = [];"); //may not need to declare
            mapperBuilder.AppendLine("   var incomingOverlayRotation = [];");  //may not need to declare
            mapperBuilder.AppendLine("   var incomingOverlayRectangle = [];");  //may not need to declare
            mapperBuilder.AppendLine("   function initOverlays(){");   
            
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
                    GeoSpatial_Information geoInfo2 = pageNode.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
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
                            if(localit == 2)
                            {
                                bounds2 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                            }
                            localit++;
                            
                        }
                        bounds += bounds2 + ", " + bounds1;
                        bounds += ")";
                        polygonBounds.Add(bounds);
                        mapperBuilder.AppendLine("      incomingOverlayBounds[" + it + "] = " + bounds + ";");
                        
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
                        
                        //setup rectangle options and bounds
                        //mapperBuilder.AppendLine("      incomingOverlayRectangle[" + it + "] = new google.maps.Rectangle(); ");
                        //mapperBuilder.AppendLine("      incomingOverlayRectangle[" + it + "].setOptions(overlayRectangleOptions); ");
                        //mapperBuilder.AppendLine("      incomingOverlayRectangle[" + it + "].setBounds(" + bounds + "); ");
                                             
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
                        //not yet completed
                        mapperBuilder.AppendLine("      <!-- point holder: " + allPoints[point].Latitude + ", " + allPoints[point].Longitude + ", " + allPoints[point].Label + " --> ");
                    }
                    mapperBuilder.AppendLine(" ");
                    //mapperBuilder.AppendLine("      displayIncomingPoints();");
                    mapperBuilder.AppendLine(" ");
                }

                mapperBuilder.AppendLine("   }");
                mapperBuilder.AppendLine(" ");
                mapperBuilder.AppendLine(" <!-- End Geo Objects Writer --> ");
                mapperBuilder.AppendLine(" ");

                mapperBuilder.AppendLine(" var baseURL = \"" + CurrentMode.Base_URL+"\"; ");
                mapperBuilder.AppendLine(" </script> ");
                mapperBuilder.AppendLine(" ");
                
            }

            //more js (must load after first
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_mapper.js\"></script>");                                       //custom script
            mapperBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapper/gmaps-markerwithlabel-1.8.1.min.js\"></script>");               //must load after custom
            
            //html goodies
            mapperBuilder.AppendLine(" <div id=\"mapper_container_thebigdeal\">  ");
            mapperBuilder.AppendLine("     <div id=\"container1\"> ");
            mapperBuilder.AppendLine("         <div id=\"container2a\"> ");
            mapperBuilder.AppendLine("             <div> ");
            mapperBuilder.AppendLine("                 <div id=\"toolbar\"> ");
            mapperBuilder.AppendLine("                     <div class=\"grouping\"> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"./mapedit\" id=\"toolbar_reset\" title=\"Reset: Reset Map To Defaults\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-reset.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_toggleControls\" title=\"Controls: Toggle Map Controls\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-controls.png\" /></a> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"toolbar_toggletoolbox\" title=\"Toolbox: Toggle Toolbox\" onclick=\"toggletoolbox(4);\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-toolbox.png\" /></a> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                     <div class=\"grouping\"> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_layerRoadmap\" title=\"Roadmap: Toggle Road Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-roadmap.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_layerTerrain\" title=\"Terrain: Toggle Terrain Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-terrain.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_layerSatellite\" title=\"Satellite: Toggle Satellite Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-satellite.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_layerHybrid\" title=\"Hybrid: Toggle Hybrid Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-hybrid.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_layerCustom\" title=\"Block/Lot: Toggle Block/Lot Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-blockLot.png\" /></a> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"toolbar_layerReset\" title=\"Reset Map Type\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-layerReset.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                </div> ");
            mapperBuilder.AppendLine("                     <div class=\"grouping\"> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_panUp\" title=\"Pan Map Up\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panUp.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_panLeft\" title=\"Pan Map Left\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panLeft.png\" /></a> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"toolbar_panReset\" title=\"Pan To Default\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panReset.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_panRight\" title=\"Pan Map Right\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panRight.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_panDown\" title=\"Pan Map Down\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panDown.png\" /></a> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                     <div class=\"grouping\"> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_zoomIn\" title=\"Zoom Map In\" onclick=\"checkZoomLevel();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-zoomIn.png\" /></a> ");
            mapperBuilder.AppendLine(" 	                    <a href=\"#\" id=\"toolbar_zoomReset\" title=\"Reset Zoom Levels\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-zoomReset2.png\" /></a> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"toolbar_zoomOut\" title=\"Zoom Map Out\" onclick=\"checkZoomLevel();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-zoomOut.png\" /></a> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                     <div class=\"grouping\"> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"toolbar_manageItem\" title=\"Manage Location Details\" onclick=\"action1();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-action1.png\" /></a> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"toolbar_manageOverlay\" title=\"Manage Map Coverage\" onclick=\"action2();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-action2.png\" /></a> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"toolbar_managePOI\" title=\"Manage Point of Interest\" onclick=\"action3();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-action3.png\" /></a> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                     <div class=\"grouping\"> ");
            mapperBuilder.AppendLine("                         <input id=\"toolbar_find\" class=\"search\" title=\"Locate: Find A Location On The Map\" type=\"text\" placeholder=\"Locate\" onBlur=\"finder(this.value);\" onClick=\"this.select();\" /> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("             </div> ");
            mapperBuilder.AppendLine("         </div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("         <div id=\"container_toggle_toolbar1\"><a href=\"#\" id=\"toolbar_toggle1\" title=\"Toolbar: Hide the Toolbar\" onclick=\"Toggle('#container2a');\"><img src=\"/sobekcm/default/images/mapper/toolbar-toggle.png\" /></a></div> ");
            mapperBuilder.AppendLine("         <div id=\"container_toggle_toolbar2\"><a href=\"#\" id=\"toolbar_toggle2\" title=\"Toolbar: Show the Toolbar\" onclick=\"Toggle('#container2a');\"><img src=\"/sobekcm/default/images/mapper/toolbar-toggle.png\" /></a></div> ");
            mapperBuilder.AppendLine("          ");
            mapperBuilder.AppendLine("         <div id=\"container2b\"> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("             <div id=\"messageContainer\" style=\"display:none;\"></div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("             <div id=\"toolbox\" class=\"ui-widget-content\"> ");
            mapperBuilder.AppendLine("                 <div class=\"toolbar\" > ");
            mapperBuilder.AppendLine("                      ");
            mapperBuilder.AppendLine("                     <a href=\"#\" id=\"tmin\" onclick=\"toggletoolbox(1);\"><img src=\"/sobekcm/default/images/mapper/toolbox-minimize2.png\" /></a> ");
            mapperBuilder.AppendLine("                     <a href=\"#\" id=\"tmax\" onclick=\"toggletoolbox(2);\"><img src=\"/sobekcm/default/images/mapper/toolbox-maximize2.png\" /></a> ");
            mapperBuilder.AppendLine("                     <a href=\"#\" id=\"tclose\" onclick=\"toggletoolbox(3);\"><img src=\"/sobekcm/default/images/mapper/toolbox-close2.png\" /></a> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("                 <div id=\"toolboxTabs\"> ");
            mapperBuilder.AppendLine("                     <h3 class=\"tab-title\">Map Controls</h3> ");
            mapperBuilder.AppendLine("                     <div class=\"tab\"> ");
            mapperBuilder.AppendLine("                         <div class=\"tab-content\"> ");
            mapperBuilder.AppendLine("                         <table id=\"mapper-table\"> ");
            mapperBuilder.AppendLine("                             <tr> ");
            mapperBuilder.AppendLine("                                 <td><a id=\"toolbox_layerRoadmap\" title=\"Roadmap: Toggle Road Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-roadmap.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_layerTerrain\" title=\"Terrain: Toggle Terrain Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-terrain.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td>&nbsp;</td> ");
            mapperBuilder.AppendLine("                                 <td></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_panUp\" title=\"Pan Map Up\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panUp.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td></td> ");
            mapperBuilder.AppendLine("                             </tr> ");
            mapperBuilder.AppendLine("                             <tr> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_layerSatellite\" title=\"Satellite: Toggle Satellite Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-satellite.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_layerHybrid\" title=\"Hybrid: Toggle Hybrid Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-hybrid.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td>&nbsp;</td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_panLeft\" title=\"Pan Map Left\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panLeft.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_panReset\" title=\"Pan To Default\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panReset.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_panRight\" title=\"Pan Map Right\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panRight.png\" /></a></td> ");
            mapperBuilder.AppendLine("                             </tr> ");
            mapperBuilder.AppendLine("                             <tr> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_layerCustom\" title=\"Block/Lot: Toggle Block/Lot Map Layer\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-blockLot.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_layerReset\" title=\"Reset Map Type\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-layerReset.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td>&nbsp;</td> ");
            mapperBuilder.AppendLine("                                 <td></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_panDown\" title=\"Pan Map Down\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-panDown.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td></td> ");
            mapperBuilder.AppendLine("                             </tr> ");
            mapperBuilder.AppendLine("                             <tr><td colspan=\"6\"><br /></td></tr> ");
            mapperBuilder.AppendLine("                             <tr> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"./mapedit\" id=\"toolbox_reset\" title=\"Reset: Reset Map To Defaults\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-reset.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_toggleControls\" title=\"Controls: Toggle Map Controls\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-controls.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td>&nbsp;</td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_zoomIn\" title=\"Zoom Map In\" onclick=\"checkZoomLevel();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-zoomIn.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_zoomReset\" title=\"Reset Zoom Levels\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-zoomReset2.png\" /></a></td> ");
            mapperBuilder.AppendLine("                                 <td><a href=\"#\" id=\"toolbox_zoomOut\" title=\"Zoom Map Out\" onclick=\"checkZoomLevel();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-zoomOut.png\" /></a></td> ");
            mapperBuilder.AppendLine("                             </tr> ");
            mapperBuilder.AppendLine("                         </table> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("                     <h3 class=\"tab-title\">Actions</h3> ");
            mapperBuilder.AppendLine("                     <div class=\"tab\" > ");
            mapperBuilder.AppendLine("                         <div class=\"tab-content\"> ");
            mapperBuilder.AppendLine("                             <a href=\"#\" id=\"toolbox_manageItem\" title=\"Manage Location Details\" onclick=\"action1();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-action1.png\" /></a>&nbsp ");
            mapperBuilder.AppendLine("                             <a href=\"#\" id=\"toolbox_manageOverlay\" title=\"Manage Map Coverage\" onclick=\"action2();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-action2.png\" /></a>&nbsp ");
            mapperBuilder.AppendLine("                             <a href=\"#\" id=\"toolbox_managePOI\" title=\"Manage Point of Interest\" onclick=\"action3();\"><img class=\"button\" src=\"/sobekcm/default/images/mapper/button-action3.png\" /></a> ");
            mapperBuilder.AppendLine("                             <br/> ");
            mapperBuilder.AppendLine("                             <br/> ");
            mapperBuilder.AppendLine("                             <input id=\"toolbox_locate\" class=\"search\" title=\"Locate: Find A Location On The Map\" type=\"text\" placeholder=\"Locate\" onBlur=\"finder(this.value);\" onClick=\"this.select();\" /> ");
            mapperBuilder.AppendLine("                             <br/> ");
            mapperBuilder.AppendLine("                             <br/> ");
            mapperBuilder.AppendLine("                             <div style=\"clear:both;\"></div> ");
            mapperBuilder.AppendLine("                             <div id=\"search_results\"></div> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                          ");
            mapperBuilder.AppendLine("                          ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                      ");
            mapperBuilder.AppendLine("                     <h3 class=\"tab-title\">Manage Location Details</h3> ");
            mapperBuilder.AppendLine("                     <div class=\"tab\" style=\"text-align:center;\"> ");
            mapperBuilder.AppendLine(" 						<div class=\"tab-content\"> ");
            mapperBuilder.AppendLine(" 							<a href=\"#\" id=\"toolbox_placeItem\" title=\"Place a Point\"><img src=\"/sobekcm/default/images/mapper/button-action1.png\" /></a> ");
            mapperBuilder.AppendLine(" 							<a href=\"#\" id=\"item_getUserLocation\" title=\"Center On Your Current Position\"><img src=\"/sobekcm/default/images/mapper/getUserLocation.png\" /></a> ");
            mapperBuilder.AppendLine(" 							<div style=\"clear:both;text-align:left;\"> ");
            mapperBuilder.AppendLine(" 								<br /> ");
            mapperBuilder.AppendLine(" 								<textarea id=\"posItem\" class=\"tab-field\" title=\"Coordinates: This is the selected Latitude and Longitude of the point you selected.\" rows=\"2\" cols=\"24\" placeholder=\"Selected Lat/Long\"></textarea> ");
            mapperBuilder.AppendLine(" 								<br/> ");
            mapperBuilder.AppendLine(" 								<br/> ");
            mapperBuilder.AppendLine(" 								<textarea id=\"rgItem\" class=\"tab-field\" title=\"Address: This is the nearest address of the point you selected.\" rows=\"3\" cols=\"24\" placeholder=\"Nearest Address\"></textarea> ");
            mapperBuilder.AppendLine(" 								<br/> ");
            mapperBuilder.AppendLine(" 								<br/> ");
            mapperBuilder.AppendLine(" 							</div> ");
            mapperBuilder.AppendLine(" 							<div style=\"clear:both;\"> ");
            mapperBuilder.AppendLine(" 								<a href=\"#\" id=\"saveItem\" title=\"Save Item\" onclick=\"buttonSaveItem();\" class=\"button2\">Save Item</a> ");
            mapperBuilder.AppendLine(" 								<a href=\"#\" id=\"clearItem\" title=\"Clear Item\" onclick=\"buttonClearItem();\" class=\"button2\">Clear Item</a> ");
            mapperBuilder.AppendLine(" 							</div> ");
            mapperBuilder.AppendLine(" 						</div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("                     <h3 class=\"tab-title\" onclick=\"action2();\" >Manage Map Coverage</h3> ");
            mapperBuilder.AppendLine("                     <div class=\"tab\" style=\"text-align:center;\"> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"toolbox_placeOverlay\" title=\"Place Overlay\"><img src=\"/sobekcm/default/images/mapper/button-action2.png\" /></a> ");
            mapperBuilder.AppendLine("                         <a href=\"#\" id=\"overlay_getUserLocation\" title=\"Center On Your Current Position\"><img src=\"/sobekcm/default/images/mapper/getUserLocation.png\" /></a> ");
            mapperBuilder.AppendLine("                         <div style=\"clear:both;\"> ");
            mapperBuilder.AppendLine("                             <br /><br /> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                         <div id=\"rotation\" style=\"float:left;\"> ");
            mapperBuilder.AppendLine("                                 <div id=\"rotationKnob\" title=\"Rotate: Edit the rotation value\"> ");
            mapperBuilder.AppendLine("                                 <input class=\"knob\" data-displayInput=\"false\" data-width=\"68\" data-step=\"1\" data-min=\"0\" data-max=\"360\" data-cursor=true data-bgColor=\"#B2B2B2\" data-fgColor=\"#111111\" data-thickness=\"0.3\" value=\"0\"> ");
            mapperBuilder.AppendLine("                                 </div> ");
            mapperBuilder.AppendLine("                                 <a id=\"rotationCounterClockwise\" href=\"#\" title=\".1&deg Left: Click to Rotate .1&deg Counter-Clockwise\" onclick=\"rotate(-0.1)\"><img src=\"/sobekcm/default/images/mapper/rotation-counterClockwise.png\" /></a> ");
            mapperBuilder.AppendLine("                                 <a id=\"rotationReset\" href=\"#\" title=\"Reset: Click to Reset Rotation\" onclick=\"rotate(0)\"><img src=\"/sobekcm/default/images/mapper/rotation-reset.png\" /></a> ");
            mapperBuilder.AppendLine("                                 <a id=\"rotationClockwise\" href=\"#\" title=\".1&deg Right: Click to Rotate .1&deg Clockwise\" onclick=\"rotate(0.1)\"><img src=\"/sobekcm/default/images/mapper/rotation-clockwise.png\" /></a> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                         <div id=\"transparency\" title=\"Transparency: Set the transparency of this Overlay\"> ");
            mapperBuilder.AppendLine("                             <div id=\"overlayTransparencySlider\"></div> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                         <div style=\"clear:both;margin-left:-3px;\"> ");
            mapperBuilder.AppendLine("                             <br /> ");
            mapperBuilder.AppendLine("                             <br /> ");
            mapperBuilder.AppendLine("                             <a href=\"#\" id=\"saveOverlay\" title=\"Save Overlay\" onclick=\"buttonSaveOverlay();\" class=\"button2\">Save Overlay</a> ");
            mapperBuilder.AppendLine("                             <a href=\"#\" id=\"clearOverlay\" title=\"Clear Overlay\" onclick=\"buttonClearOverlay();\" class=\"button2\">Clear Overlay</a> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("                     <h3 class=\"tab-title\">Manage POI Details</h3> ");
            mapperBuilder.AppendLine("                     <div class=\"tab\" style=\"text-align:center;height:295px;\"> ");
            mapperBuilder.AppendLine("                         <div class=\"tab-content2\" style=\"margin: 0 -20px 0 -20px;\"> ");
            mapperBuilder.AppendLine("                             <a href=\"#\" id=\"toolbox_placePOI\" title=\"Place Point Of Interest\"><img src=\"/sobekcm/default/images/mapper/button-action3.png\" /></a> ");
            mapperBuilder.AppendLine("                             <a href=\"#\" id=\"poi_getUserLocation\" title=\"Center On Your Current Position\"><img src=\"/sobekcm/default/images/mapper/getUserLocation.png\" /></a> ");
            mapperBuilder.AppendLine("                             <br/> ");
            mapperBuilder.AppendLine("                             <br/> ");
            mapperBuilder.AppendLine("                                 <a id=\"toolbox_poiMarker\" href=\"#\" title=\"Marker: Place a Point\"><img src=\"/sobekcm/default/images/mapper/button-drawMarker.png\"/></a>  ");
            mapperBuilder.AppendLine("                                 <a id=\"toolbox_poiCircle\" href=\"#\" title=\"Circle: Place a Circle\"><img src=\"/sobekcm/default/images/mapper/button-drawCircle.png\"/></a>  ");
            mapperBuilder.AppendLine("                                 <a id=\"toolbox_poiRectangle\" href=\"#\" title=\"Rectangle: Place a Rectangle\"><img src=\"/sobekcm/default/images/mapper/button-drawRectangle.png\"/></a>  ");
            mapperBuilder.AppendLine("                                 <a id=\"toolbox_poiPolygon\" href=\"#\" title=\"Polygon: Place a Polygon\"><img src=\"/sobekcm/default/images/mapper/button-drawPolygon.png\"/></a>  ");
            mapperBuilder.AppendLine("                                 <a id=\"toolbox_poiLine\" href=\"#\" title=\"Line: Place a Line\"><img src=\"/sobekcm/default/images/mapper/button-drawLine.png\"/></a>  ");
            mapperBuilder.AppendLine("                             <br/> ");
            mapperBuilder.AppendLine("                             <div style=\"clear:both;text-align:left;\"> ");
            mapperBuilder.AppendLine("                                 <br /> ");
            mapperBuilder.AppendLine("                                  ");
            mapperBuilder.AppendLine("                                 <div id=\"poiList_Container\"> ");
            mapperBuilder.AppendLine("                                     <div id=\"poiList_ScollContainer\"> ");
            mapperBuilder.AppendLine("                                         <div id=\"poiList\"></div> ");
            mapperBuilder.AppendLine("                                     </div> ");
            mapperBuilder.AppendLine("                                 </div> ");
            mapperBuilder.AppendLine("  ");
            mapperBuilder.AppendLine("                                  ");
            mapperBuilder.AppendLine("                                 <br /> ");
            mapperBuilder.AppendLine("                             </div> ");
            mapperBuilder.AppendLine("                             <div style=\"clear:both;\"> ");
            mapperBuilder.AppendLine("                                 <a href=\"#\" id=\"savePOI\" title=\"Save Point Of Interest Set\" onclick=\"buttonSavePOI();\" class=\"button2\">Save POI(s)</a> ");
            mapperBuilder.AppendLine("                                 <a href=\"#\" id=\"clearPOI\" title=\"Clear Point Of Interest Set\" onclick=\"buttonClearPOI();\" class=\"button2\">Clear POI(s)</a> ");
            mapperBuilder.AppendLine("                             </div> ");
            mapperBuilder.AppendLine("                         </div> ");
            mapperBuilder.AppendLine("                     </div> ");
            mapperBuilder.AppendLine("                 </div> ");
            mapperBuilder.AppendLine("             </div> ");
            mapperBuilder.AppendLine("             <!-- ");
            mapperBuilder.AppendLine("                 <div id=\"debugger\" style=\"width:300px;position:absolute;left:50%;margin-top:940px;margin-left:-150px;min-height:50px;opacity:0.6;z-index:100000;background-color:lightskyblue;\">Debugger:<a style=\"float:right;\" onclick=\"hide('#debugger');\">X</a></div> ");
            mapperBuilder.AppendLine("             --> ");
            mapperBuilder.AppendLine("                 <div id=\"googleMap\"></div> ");
            mapperBuilder.AppendLine("              ");
            mapperBuilder.AppendLine("         </div> ");
            mapperBuilder.AppendLine("     </div> ");
            mapperBuilder.AppendLine(" </div> ");
            

            mapperBuilder.AppendLine("</td>");

            // Add the literal to the placeholder
            Literal placeHolderText = new Literal();
            placeHolderText.Text = mapperBuilder.ToString();
            placeHolder.Controls.Add(placeHolderText);
            
        }
    }
}
