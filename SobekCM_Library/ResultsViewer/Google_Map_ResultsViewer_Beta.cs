using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.Navigation;

namespace SobekCM.Library.ResultsViewer
{
    class Google_Map_ResultsViewer_Beta : abstract_ResultsViewer
    {
        /// <summary> Constructor for a new instance of the Full_ResultsViewer class </summary>
        /// <param name="CurrentMode"> Sobek object that holds useful information </param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Google_Map_ResultsViewer_Beta(SobekCM_Navigation_Object CurrentMode, Item_Lookup_Object All_Items_Lookup)
        {
            //create new tracer?
            Custom_Tracer Tracer = new Custom_Tracer();

            base.All_Items_Lookup = All_Items_Lookup;
            
            //holds actions from page
            string payload = HttpContext.Current.Request.Form["payload"] ?? String.Empty;
            
            // See if there were hidden requests
            if (!String.IsNullOrEmpty(payload))
            {
                //if (action == "action")
                    Perform_Callback_Action(payload, Tracer);
            }
            else
            {
                //do a search for all the items in this agg
                string temp_AggregationId = CurrentMode.Aggregation;
                string[] temp_AggregationList = temp_AggregationId.Split(' ');
                //Perform_Aggregation_Search(temp_AggregationList, Tracer);
            }

        }

        //could not get working
        public static void Refresh_MSRKey()
        {
            //string[] aggregationIds = CurrentMode.Aggregation.Split(' ');
            //int MSRKeyHashSpecial = 0;
            //foreach (string aggregationId in aggregationIds)
            //{
            //    byte[] tempAggChars = Encoding.ASCII.GetBytes(aggregationId);
            //    foreach (byte tempAggChar in tempAggChars)
            //    {
            //        MSRKeyHashSpecial += Convert.ToInt32(tempAggChar);
            //    }
            //}
            ////finish processing msrkeyhash and store
            //MSRKeyHashSpecial = Convert.ToInt32(MSRKeyHashSpecial * aggregationIds.Length);
            //HttpContext.Current.Session["MapSearchResultsKey"] = "MapSearchResults_" + MSRKeyHashSpecial.ToString();
        }
        
        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_HTML(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            //tracer
            if (Tracer != null)
                Tracer.Add_Trace("Map_ResultsWriter.Add_HTML", "Rendering results in map view");
            
            // Start to build the response
            StringBuilder mapSearchBuilder = new StringBuilder();
            
            //HEADER CONTENT
            //hidden input (for callback)
            mapSearchBuilder.AppendLine("     <input type=\"hidden\" id=\"payload\" name=\"payload\" value=\"\" /> ");
            //TEMP HEADER FILES
            mapSearchBuilder.AppendLine("     <script type=\"text/javascript\" src=\"https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&key=AIzaSyCzliz5FjUlEI9D2605b33-etBrENSSBZM&libraries=drawing\"></script> ");
            mapSearchBuilder.AppendLine("     <script src=\"" + CurrentMode.Base_URL + "default/scripts/mapsearch/external_markerclusterer_compiled.js\"></script>  ");
            mapSearchBuilder.AppendLine("     <script src=\"http://www.hlmatt.com/uf/part2/bin/js/external_jquery_1.10.2.js\"></script> ");
            mapSearchBuilder.AppendLine("     <script src=\"http://www.hlmatt.com/uf/part2/bin/js/external_jquery_ui.min.js\"></script> ");
            mapSearchBuilder.AppendLine("     <script src=\"http://www.hlmatt.com/uf/part2/bin/js/external_jquery_ui_labeledslider.js\"></script> ");
            mapSearchBuilder.AppendLine("     <script type=\"text/javascript\" src=\"http://www.hlmatt.com/uf/part2/bin/js/external_gmaps_infobox.js\"></script> ");
            mapSearchBuilder.AppendLine("     <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapsearch/sobekcm_map_search.js\"></script> ");
            mapSearchBuilder.AppendLine("     <link rel=\"stylesheet\" href=\"http://www.hlmatt.com/uf/part2/bin/css/external_jquery_ui_1.10.4.css\"> ");
            mapSearchBuilder.AppendLine("     <link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_MapSearch.css\"> ");
            // ADD SERVER2CLIENT VARS
            mapSearchBuilder.AppendLine("  ");
            mapSearchBuilder.AppendLine("     <script type=\"text/javascript\"> ");
            mapSearchBuilder.AppendLine("       function initServerToClientVars(){ ");
            mapSearchBuilder.AppendLine("         baseURL = \"" + CurrentMode.Base_URL + "\"; ");
            mapSearchBuilder.AppendLine("       } ");
            mapSearchBuilder.AppendLine("  ");
            //ADD EXISTING POINTS WITH JSON
            mapSearchBuilder.AppendLine("       function initJSON(){ ");
            if (HttpContext.Current.Items["DSR"]!=null)
                mapSearchBuilder.AppendLine("       DSR = JSON.stringify(" + HttpContext.Current.Items["DSR"].ToString() + "); ");
            mapSearchBuilder.AppendLine("       } ");
            mapSearchBuilder.AppendLine("     </script> ");
            mapSearchBuilder.AppendLine("  ");

            //BETA BLANKET
            //mapSearchBuilder.AppendLine(" <div id=\"container-betaBlanket\">WARNING: This page is currently in beta testing and so some features may not work.</div> ");
            
            // PAGE LITERAL
            mapSearchBuilder.AppendLine(" <div id=\"container_SearchMap\"></div> ");

            // Add this to the page
            Literal writeData = new Literal { Text = mapSearchBuilder.ToString() };
            MainPlaceHolder.Controls.Add(writeData);

        }
        
        /// <summary> parse and process incoming message  </summary>
        /// <param name="sendData"> message from page </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Perform_Callback_Action(String sendData, Custom_Tracer Tracer)
        {
            //currently in the live class, move to here eventually.
        }
        
        //performs an aggregation search based on a single aggregation (can be used to remove aggregation as well
        public static void Perform_Aggregation_Search(string[] aggregationIds, Custom_Tracer Tracer)
        {
            //hooks (eventual expansion) 
            int HOOK_maxFIDCount = 8; 

            #region FIDs Support

            //get fids
            

            //old
            if (HttpContext.Current.Session["FIDKey"]==null)
                HttpContext.Current.Session["FIDKey"] = ""; //init
            string FIDKey = HttpContext.Current.Session["FIDKey"].ToString();
            List<string> FIDs = (List<string>)HttpContext.Current.Cache[FIDKey];
            List<string> temp_FIDs = new List<string>();
            foreach (string FID in FIDs)
            {
                //add the metadata name (converted to dbColumn name format)
                temp_FIDs.Add(FID.Replace(" ", "_"));
            }

            //make sure we have alll the FIDs, if not create blank holders
            if (HOOK_maxFIDCount > temp_FIDs.Count)
            {
                while (HOOK_maxFIDCount >= temp_FIDs.Count)
                {
                    temp_FIDs.Add("");
                }
            }

            #endregion

            #region MSRKey Support

            int MSRKeyHashSpecial = 0;
            foreach (string aggregationId in aggregationIds)
            {
                byte[] tempAggChars = Encoding.ASCII.GetBytes(aggregationId);
                foreach (byte tempAggChar in tempAggChars)
                {
                    MSRKeyHashSpecial += Convert.ToInt32(tempAggChar);
                }
            }
            //finish processing msrkeyhash and store
            MSRKeyHashSpecial = Convert.ToInt32(MSRKeyHashSpecial * aggregationIds.Length);
            HttpContext.Current.Session["MapSearchResultsKey"] = "MapSearchResults_" + MSRKeyHashSpecial.ToString();
            
            #endregion

            #region search the database for all items in all of the provided aggregations and merge them into one datatable

            //define MSR support objects
            DataTable searchResults = new DataTable();
            DataTable displaySearchResults = new DataTable();
            
            //create MSR columns
            searchResults.Columns.Add("ItemID", typeof(string));
            searchResults.Columns.Add("Point_Latitude", typeof(string));
            searchResults.Columns.Add("Point_Longitude", typeof(string));
            searchResults.Columns.Add("Start_DateTime", typeof(string));
            searchResults.Columns.Add("End_DateTime", typeof(string));
            #region create MSR filter columns
            //get the db column name for each filter
            foreach (string filterName in FIDs)
            {
                searchResults.Columns.Add(filterName.Replace(" ", "_"), typeof(string));
            }

            //determine if we have used all 8 of the real filters, if not, create gap (8 + x where x= non fid count to date)
            for (int i = searchResults.Columns.Count; i < (8 + 5); i++)
            {
                searchResults.Columns.Add("empty" + i.ToString(), typeof(string));
                //FIDs.Add("empty" + i.ToString());
                FIDs.Add("");
            }
            #endregion

            //create DSR columns
            displaySearchResults.Columns.Add("ItemID", typeof(string));
            displaySearchResults.Columns.Add("Point_Latitude", typeof(string));
            displaySearchResults.Columns.Add("Point_Longitude", typeof(string));
            
            //check to see if we already have msr in cache, else get a new msr
            string MSRKey = HttpContext.Current.Session["MapSearchResultsKey"].ToString();
            if (HttpContext.Current.Cache[MSRKey] == null)
            {
                #region Create New MSR
                
                //temp objects
                List<DataTable> temp_Tables = new List<DataTable>(); //change to dataset?
                DataTable temp_searchResults = new DataTable();

                //holds bounds calculator fields
                double swx = -1;
                double swy = -1;
                double nex = -1;
                double ney = -1;

                //search for all items in each collection (handles multiple aggregations)
                foreach (string aggregationId in aggregationIds)
                {
                    temp_Tables.Add(SobekCM_Database.Get_All_Items_By_AggregationID(aggregationId, temp_FIDs, Tracer));
                }

                //merge the tables if there are multiple aggregations
                foreach (DataTable temp_Table in temp_Tables)
                {
                    temp_searchResults.Merge(temp_Table);
                }

                //go through each item from search and it and its metadata to MSR
                foreach (DataRow searchResult in temp_searchResults.Rows)
                {
                    //define holders
                    string a = String.Empty;
                    string b = String.Empty;
                    string c = String.Empty;
                    string d = String.Empty;
                    string e = String.Empty;

                    //get itemID
                    if(searchResult["ItemID"]!=null)
                        a = searchResult["ItemID"].ToString();

                    //get lat/long
                    if (searchResult["MainLatitude"] != null)
                        b = searchResult["MainLatitude"].ToString();
                    if (searchResult["MainLongitude"] != null)
                        c = searchResult["MainLongitude"].ToString();

                    //fallback to spatial kml
                    if (string.IsNullOrEmpty(b))
                    {
                        string[] temp = searchResult["Spatial_KML"].ToString().Split('|');
                        //is this a point?
                        if (temp.Length == 2)
                        {
                            string[] temp2 = temp[1].Split(',');
                            b = temp2[0]; //lat
                            c = temp2[1]; //lng
                        }
                    }

                    //work with bounds calculator
                    if (!string.IsNullOrEmpty(b))
                    {
                        if ((Convert.ToDouble(b) < swx) || (swx == -1))
                            swx = Convert.ToDouble(b);
                        if ((Convert.ToDouble(b) > nex) || (swy == -1))
                            nex = Convert.ToDouble(b);
                        if ((Convert.ToDouble(c) < swy) || (nex == -1))
                            swy = Convert.ToDouble(c);
                        if ((Convert.ToDouble(c) > ney) || (ney == -1))
                            ney = Convert.ToDouble(c);
                    }

                    //handle start datetime
                    if (searchResult["PubDate"] != null)
                        d = searchResult["PubDate"].ToString();
                    else
                        if (searchResult["CreateDate"] != null)
                            d = searchResult["CreateDate"].ToString();

                    //handle end datetime
                    if (searchResult["PubDate"] != null)
                        e = searchResult["PubDate"].ToString();
                    else
                        if (searchResult["CreateDate"] != null)
                            e = searchResult["CreateDate"].ToString();

                    List<string> filterValues = new List<string>();
                    foreach (string filterName in FIDs)
                    {
                        //hande empty filter slots
                        //if (filterName.IndexOf("empty") != -1)
                        if (string.IsNullOrEmpty(filterName))
                            filterValues.Add("");
                        else
                            filterValues.Add(searchResult[filterName.Replace(" ", "_")].ToString());
                    }

                    //add to the DSR (id/lat/lng) only if lat/lng not null
                    if (!string.IsNullOrEmpty(b))
                        displaySearchResults.Rows.Add(a, b, c);

                    //add all to search results (with 8 fids)
                    searchResults.Rows.Add(a, b, c, d, e, filterValues[0], filterValues[1], filterValues[2], filterValues[3], filterValues[4], filterValues[5], filterValues[6], filterValues[7]);

                }

                //add to collection level params to search results
                searchResults.Rows.Add("swBounds", swx.ToString(), swy.ToString(), "", "", "", "", "", "", "", "", "", "");
                searchResults.Rows.Add("neBounds", nex.ToString(), ney.ToString(), "", "", "", "", "", "", "", "", "", "");

                //add to collection level params to display search results
                displaySearchResults.Rows.Add("swBounds", swx.ToString(), swy.ToString());
                displaySearchResults.Rows.Add("neBounds", nex.ToString(), ney.ToString());

                //assign and hold the current search result datatable, from now on we will be using this as the base layer...
                HttpContext.Current.Cache[MSRKey] = searchResults;
                HttpContext.Current.Cache[MSRKey + "_Created"] = DateTime.Now;

                #endregion
            }
            else
            {
                if (HttpContext.Current.Cache[MSRKey + "_Created"] != null)
                {
                    //determine if MSR is more than 30 mins old
                    if ((DateTime.Now.Subtract(Convert.ToDateTime(HttpContext.Current.Cache[MSRKey + "_Created"].ToString())).TotalMinutes) > 30)
                    {
                        #region Create new MSR

                        //temp objects
                        List<DataTable> temp_Tables = new List<DataTable>(); //change to dataset?
                        DataTable temp_searchResults = new DataTable();

                        //holds bounds calculator fields
                        double swx = -1;
                        double swy = -1;
                        double nex = -1;
                        double ney = -1;

                        //search for all items in each collection (handles multiple aggregations)
                        foreach (string aggregationId in aggregationIds)
                        {
                            temp_Tables.Add(SobekCM_Database.Get_All_Items_By_AggregationID(aggregationId, temp_FIDs, Tracer));
                        }

                        //merge the tables if there are multiple aggregations
                        foreach (DataTable temp_Table in temp_Tables)
                        {
                            temp_searchResults.Merge(temp_Table);
                        }

                        //go through each item from search and it and its metadata to MSR
                        foreach (DataRow searchResult in temp_searchResults.Rows)
                        {
                            //get itemID
                            string a = searchResult["ItemID"].ToString();

                            //get lat/long
                            string b = searchResult["MainLatitude"].ToString();
                            string c = searchResult["MainLongitude"].ToString();

                            //fallback to spatial kml
                            if (string.IsNullOrEmpty(b))
                            {
                                string[] temp = searchResult["Spatial_KML"].ToString().Split(',');
                                //is this a point?
                                if (temp.Length == 2)
                                {
                                    b = temp[0].Replace("P|", "").Replace("A|", "");
                                    c = temp[1];
                                }
                            }

                            //work with bounds calculator
                            if (!string.IsNullOrEmpty(b))
                            {
                                if ((Convert.ToDouble(b) < swx) || (swx == -1))
                                    swx = Convert.ToDouble(b);
                                if ((Convert.ToDouble(b) > nex) || (swy == -1))
                                    nex = Convert.ToDouble(b);
                                if ((Convert.ToDouble(c) < swy) || (nex == -1))
                                    swy = Convert.ToDouble(c);
                                if ((Convert.ToDouble(c) > ney) || (ney == -1))
                                    ney = Convert.ToDouble(c);
                            }

                            //handle date time
                            string d = searchResult["PubDate"].ToString();
                            if (string.IsNullOrEmpty(d))
                                d = searchResult["CreateDate"].ToString();
                            //d = Convert.ToDateTime(d);
                            string e = searchResult["PubDate"].ToString();
                            if (string.IsNullOrEmpty(e))
                                e = searchResult["CreateDate"].ToString();

                            List<string> filterValues = new List<string>();
                            foreach (string filterName in FIDs)
                            {
                                //hande empty filter slots
                                //if (filterName.IndexOf("empty") != -1)
                                if (string.IsNullOrEmpty(filterName))
                                    filterValues.Add("");
                                else
                                    filterValues.Add(searchResult[filterName.Replace(" ", "_")].ToString());
                            }

                            //add to the DSR (id/lat/lng) only if lat/lng not null
                            if (!string.IsNullOrEmpty(b))
                                displaySearchResults.Rows.Add(a, b, c);

                            //add all to search results (with 8 fids)
                            searchResults.Rows.Add(a, b, c, d, e, filterValues[0], filterValues[1], filterValues[2], filterValues[3], filterValues[4], filterValues[5], filterValues[6], filterValues[7]);

                        }

                        //add to collection level params to search results
                        searchResults.Rows.Add("swBounds", swx.ToString(), swy.ToString(), "", "", "", "", "", "", "", "", "", "");
                        searchResults.Rows.Add("neBounds", nex.ToString(), ney.ToString(), "", "", "", "", "", "", "", "", "", "");

                        //add to collection level params to display search results
                        displaySearchResults.Rows.Add("swBounds", swx.ToString(), swy.ToString());
                        displaySearchResults.Rows.Add("neBounds", nex.ToString(), ney.ToString());

                        //assign and hold the current search result datatable, from now on we will be using this as the base layer...
                        HttpContext.Current.Cache[MSRKey] = searchResults;
                        HttpContext.Current.Cache[MSRKey + "_Created"] = DateTime.Now;

                        #endregion
                    }
                    else
                    {
                        #region Use Existing MSR

                        //assign cached MSR to active MSR
                        searchResults = HttpContext.Current.Cache[MSRKey] as DataTable;

                        //add to the DSR
                        foreach (DataRow searchResult in searchResults.Rows)
                        {
                            //get itemID
                            string a = searchResult["ItemID"].ToString();

                            //get lat/long
                            string b = searchResult["Point_Latitude"].ToString();
                            string c = searchResult["Point_Longitude"].ToString();

                            //add to the DSR (id/lat/lng) only if lat/lng not null
                            if (!string.IsNullOrEmpty(b))
                                displaySearchResults.Rows.Add(a, b, c);
                        }

                        #endregion
                    }
                }
            }
            
            //call json writer with these results
            object DSR = Create_JSON_Search_Results_Object(displaySearchResults);

            //send json obj to page and tell page to read it (via callback results)
            HttpContext.Current.Items["DSR"] = DSR;

            #endregion

        }

        //performs a bounds search
        public static void Perform_Coordinate_Bounds_Search(double swx, double swy, double nex, double ney)
        {
            //create them display search results object
            DataTable tempDSR = new DataTable();
            tempDSR.Columns.Add("ItemID", typeof (string));
            tempDSR.Columns.Add("Point_Latitude", typeof(string));
            tempDSR.Columns.Add("Point_Longitude", typeof(string));
            Refresh_MSRKey();
            //get original SR
            if (HttpContext.Current.Session["MapSearchResultsKey"] == null)
                Refresh_MSRKey();
            string MSRKey = HttpContext.Current.Session["MapSearchResultsKey"].ToString();
            
            DataTable SR = new DataTable();
            SR = HttpContext.Current.Cache[MSRKey] as DataTable;

            //add only the points within the bounds to the dsr
            foreach (DataRow result in SR.Rows)
            {
                //get itemID
                string a = result["ItemID"].ToString();

                string b = result["Point_Latitude"].ToString();
                if (string.IsNullOrEmpty(b))
                    b = "-360";
                string c = result["Point_Longitude"].ToString();
                if (string.IsNullOrEmpty(c))
                    c = "-360";
                double cLat = Convert.ToDouble(b);
                double cLng = Convert.ToDouble(c);

                //if there is a valid lat/lng, determine if point is within bounds and add it to the tempDSR if so
                if (cLat!=-360)
                    if (((cLat > swx) & (cLat < nex)) & ((cLng > swy) & (cLng < ney)))
                        tempDSR.Rows.Add(a, cLat.ToString(), cLng.ToString());

            }

            //call json writer with these results
            object DSR = Create_JSON_Search_Results_Object(tempDSR);

            //add the dsr to the session state
            HttpContext.Current.Items["DSR"] = DSR;

        }

        //performs a filter search based on a single filter (can be used to remove filter as well
        public static void Perform_Filter_Search(string[] filters)
        {
            //call db to search
            
            //call json writer with results
            
            //send json obj to page and tell page to read it (via callback results
        }

        //performs a temporal search based on time range (usually triggered by sliding timebar)
        public static void Perform_DateTime_Range_Search(DateTime startDateTime, DateTime endDateTime)
        {
            //call db
            //call json writer with results
            //send json obj to page and tell page to read it (via callback results
        }
        
        //performs a complete search (aggregation, all fliters, time range, and coords) generally uses as an initializer
        public static void Perform_Complete_Search(string[] aggregationIds, string[] filters, DateTime startDateTime, DateTime endDateTime, double lat1, double long1, double lat2, double long2, Custom_Tracer Tracer)
        {
            //call db to search for matches

            //call json writer with results

            //send json obj to page and tell page to read it (via callback results
        }

        //creates and returns a json object from search results interacting with the db
        public static object Create_JSON_Search_Results_Object(DataTable searchResults)
        {
            //take the search results from db query (incoming) and parse into JSON
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;
            foreach (DataRow dr in searchResults.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in searchResults.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            //return JSON object
            return serializer.Serialize(rows);
        }

    }
}
