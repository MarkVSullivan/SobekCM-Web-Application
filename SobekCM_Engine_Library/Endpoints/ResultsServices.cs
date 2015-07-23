#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Core.ResultTitle;
using SobekCM.Core.Search;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Engine_Library.Solr;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Endpoint supports services related to search results (or item browses) across the 
    /// entire instance or a subset of aggregations </summary>
    public class ResultsServices : EndpointBase
    {
        /// <summary> Enumeration of errors that may be encountered during these processes </summary>
        public enum ResultsEndpointErrorEnum : byte
        {
            /// <summary> No exception or error detected. (Everything is OK) </summary>
            NONE,

            /// <summary> An unknown exception was caught during this process </summary>
            Unknown,

            /// <summary> Unknown database exception was caught during this process </summary>
            Database_Exception,

            /// <summary> Timeout occurred while querying the database  </summary>
            Database_Timeout_Exception,

            /// <summary> Exception was encountered while querying the solr/lucene indexes </summary>
            Solr_Exception
        }

        /// <summary> Get just the search statistics information for a search or browse </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Search_Statistics(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("ResultsServices.Get_Search_Statistics", "Parse request to determine search requested");

            // Get all the searh field necessary from the query string
            Results_Arguments args = new Results_Arguments(QueryString);

            // Was a collection indicated?
            if (UrlSegments.Count > 0)
                args.Aggregation = UrlSegments[0];

            // Get the aggregation object (we need to know which facets to use, etc.. )
            tracer.Add_Trace("ResultsServices.Get_Search_Statistics", "Get the '" + args.Aggregation + "' item aggregation (for facets, etc..)");
            Complete_Item_Aggregation aggr = AggregationServices.get_complete_aggregation(args.Aggregation, true, tracer);

            // If no aggregation was returned, that is an error
            if (aggr == null)
            {
                tracer.Add_Trace("ResultsServices.Get_Search_Statistics", "Returned aggregation was NULL... aggregation code may not be valid");

                if ( IsDebug )
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    return;
                }

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error occurred or aggregation '" + args.Aggregation + "' not valid");
                Response.StatusCode = 500;
                return;
            }

            // Perform the search
            tracer.Add_Trace("ResultsServices.Get_Search_Statistics", "Perform the search");
            Search_Results_Statistics resultsStats;
            List<iSearch_Title_Result> resultsPage;
            ResultsEndpointErrorEnum error = Get_Search_Results(args, aggr, tracer, out resultsStats, out resultsPage);

            // Was this in debug mode?
            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);
                return;
            }

            // If an error occurred, return the error
            switch (error)
            {
                case ResultsEndpointErrorEnum.Database_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Database exception");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Database_Timeout_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Database timeout");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Solr_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Solr exception");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Unknown:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Unknown error");
                    Response.StatusCode = 500;
                    return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseResultsStats";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(resultsStats, Response, Protocol, json_callback);
        }

        /// <summary> Get just the search statistics information for a search or browse </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Search_Results_Page(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("ResultsServices.Get_Search_Results_Set", "Parse request to determine search requested");

            // Get all the searh field necessary from the query string
            Results_Arguments args = new Results_Arguments(QueryString);

            // Was a collection indicated?
            if (UrlSegments.Count > 0)
                args.Aggregation = UrlSegments[0];

            // Get the aggregation object (we need to know which facets to use, etc.. )
            tracer.Add_Trace("ResultsServices.Get_Search_Results_Set", "Get the '" + args.Aggregation + "' item aggregation (for facets, etc..)");
            Complete_Item_Aggregation aggr = AggregationServices.get_complete_aggregation(args.Aggregation, true, tracer);

            // If no aggregation was returned, that is an error
            if (aggr == null)
            {
                tracer.Add_Trace("ResultsServices.Get_Search_Results_Set", "Returned aggregation was NULL... aggregation code may not be valid");

                if ( IsDebug )
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    return;
                }

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error occurred or aggregation '" + args.Aggregation + "' not valid");
                Response.StatusCode = 500;
                return;
            }

            // Perform the search
            tracer.Add_Trace("ResultsServices.Get_Search_Results_Set", "Perform the search");
            Search_Results_Statistics resultsStats;
            List<iSearch_Title_Result> resultsPage;
            ResultsEndpointErrorEnum error = Get_Search_Results(args, aggr, tracer, out resultsStats, out resultsPage);

            // Map to the results object title / item
            tracer.Add_Trace("ResultsServices.Get_Search_Results_Set", "Map to the results object title / item");
            List<ResultTitleInfo> results = new List<ResultTitleInfo>();
            foreach (iSearch_Title_Result thisResult in resultsPage)
            {
                // Create the new rest title object
                ResultTitleInfo restTitle = new ResultTitleInfo
                {
                    BibID = thisResult.BibID, 
                    MainThumbnail = thisResult.GroupThumbnail, 
                    Title = thisResult.GroupTitle
                };

                // add each descriptive field over
                int field_index = 0;
                foreach (string metadataTerm in resultsStats.Metadata_Labels)
                {
                    if ( !String.IsNullOrWhiteSpace(thisResult.Metadata_Display_Values[field_index]))
                    {
                        string termString = thisResult.Metadata_Display_Values[field_index];
                        ResultTitle_DescriptiveTerm termObj = new ResultTitle_DescriptiveTerm(metadataTerm);
                        if (termString.IndexOf("|") > 0)
                        {
                            string[] splitter = termString.Split("|".ToCharArray());
                            foreach (string thisSplit in splitter)
                            {
                                if ( !String.IsNullOrWhiteSpace(thisSplit))
                                    termObj.Add_Value(thisSplit.Trim());
                            }
                        }
                        else
                        {
                            termObj.Add_Value(termString.Trim());
                        }
                        restTitle.Description.Add(termObj);
                    }
                    field_index++;
                }

                // Add each item
                for (int i = 0; i < thisResult.Item_Count; i++)
                {
                    iSearch_Item_Result itemResults = thisResult.Get_Item(i);

                    ResultItemInfo newItem = new ResultItemInfo
                    {
                        VID = itemResults.VID, 
                        Title = itemResults.Title, 
                        Link = itemResults.Link, 
                        MainThumbnail = itemResults.MainThumbnail
                    };

                    restTitle.Items.Add(newItem);
                }

                // Add to the array
                results.Add(restTitle);

            }

            // Was this in debug mode?
            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);
                return;
            }

            // If an error occurred, return the error
            switch (error)
            {
                case ResultsEndpointErrorEnum.Database_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Database exception");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Database_Timeout_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Database timeout");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Solr_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Solr exception");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Unknown:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Unknown error");
                    Response.StatusCode = 500;
                    return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseResultsSet";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Create the return object
            ResultSetPage wrappedObject = new ResultSetPage();
            wrappedObject.Results = results;
            wrappedObject.Page = args.Page;

            // Use the base class to serialize the object according to request protocol
            Serialize(wrappedObject, Response, Protocol, json_callback);
        }

        #region Code to support the legacy XML and JSON reports supported prior to v5.0

        /// <summary> Gets the search results and returns them in the legacy format supported prior to v5.0 </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Search_Results_Legacy(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("ResultsServices.Get_Search_Results_Legacy", "Parse request to determine search requested");

            // Get all the searh field necessary from the query string
            Results_Arguments args = new Results_Arguments(QueryString);

            // Was a collection indicated?
            if (UrlSegments.Count > 0)
                args.Aggregation = UrlSegments[0];

            // Get the aggregation object (we need to know which facets to use, etc.. )
            tracer.Add_Trace("ResultsServices.Get_Search_Results_Legacy", "Get the '" + args.Aggregation + "' item aggregation (for facets, etc..)");
            Complete_Item_Aggregation aggr = AggregationServices.get_complete_aggregation(args.Aggregation, true, tracer);

            // If no aggregation was returned, that is an error
            if (aggr == null)
            {
                tracer.Add_Trace("ResultsServices.Get_Search_Results_Legacy", "Returned aggregation was NULL... aggregation code may not be valid");

                if ( IsDebug )
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    return;
                }

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error occurred or aggregation '" + args.Aggregation + "' not valid");
                Response.StatusCode = 500;
                return;
            }

            // Perform the search
            tracer.Add_Trace("ResultsServices.Get_Search_Results_Legacy", "Perform the search");
            Search_Results_Statistics resultsStats;
            List<iSearch_Title_Result> resultsPage;
            ResultsEndpointErrorEnum error = Get_Search_Results(args, aggr, tracer, out resultsStats, out resultsPage);

            // Was this in debug mode?
            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);
                return;
            }

            // If an error occurred, return the error
            switch (error)
            {
                case ResultsEndpointErrorEnum.Database_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Database exception");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Database_Timeout_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Database timeout");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Solr_Exception:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Solr exception");
                    Response.StatusCode = 500;
                    return;

                case ResultsEndpointErrorEnum.Unknown:
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Unknown error");
                    Response.StatusCode = 500;
                    return;
            }

            // Use the base class to serialize the object according to request protocol
            switch (Protocol)
            {
                case Microservice_Endpoint_Protocol_Enum.XML:
                    legacy_xml_display_search_results(Response.Output, args, resultsStats, resultsPage );
                    break;

                case Microservice_Endpoint_Protocol_Enum.JSON:
                    legacy_json_display_search_results(Response.Output, args, resultsStats, resultsPage);
                    break;
            }
        }

        /// <summary> Writes the search or browse information in JSON format directly to the output stream  </summary>
        /// <param name="Output"> Stream to which to write the JSON search or browse information </param>
        /// <param name="Args"></param>
        /// <param name="ResultsStats"></param>
        /// <param name="ResultsPage"></param>
        protected internal void legacy_json_display_search_results(TextWriter Output, Results_Arguments Args, Search_Results_Statistics ResultsStats, List<iSearch_Title_Result> ResultsPage)
        {
            // If results are null, or no results, return empty string
            if ((ResultsPage == null) || (ResultsStats == null) || (ResultsStats.Total_Items <= 0))
                return;

            // Get the URL and network roots
            string image_url = Engine_ApplicationCache_Gateway.Settings.Image_URL;
            string base_url = Engine_ApplicationCache_Gateway.Settings.Base_URL;
            if (HttpContext.Current != null)
            {
                base_url = HttpContext.Current.Request.Url.AbsoluteUri;
                if (base_url.IndexOf("?") > 0)
                    base_url = base_url.Substring(0, base_url.IndexOf("?")).Replace("sobekcm.svc", "");
            }
            if ((base_url.Length > 0) && (base_url[base_url.Length - 1] != '/'))
                base_url = base_url + "/";
            if ((image_url.Length > 0) && (image_url[image_url.Length - 1] != '/'))
                image_url = image_url + "/";

            Output.Write("[");

            // Step through all the results
            int i = 1;
            foreach (iSearch_Title_Result titleResult in ResultsPage)
            {
                // Always get the first item for things like the main link and thumbnail
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);

                // Determine a thumbnail
                string thumb = image_url + titleResult.BibID.Substring(0, 2) + "/" + titleResult.BibID.Substring(2, 2) + "/" + titleResult.BibID.Substring(4, 2) + "/" + titleResult.BibID.Substring(6, 2) + "/" + titleResult.BibID.Substring(8) + "/" + firstItemResult.VID + "/" + firstItemResult.MainThumbnail;
                if ((thumb.ToUpper().IndexOf(".JPG") < 0) && (thumb.ToUpper().IndexOf(".GIF") < 0))
                {
                    thumb = String.Empty;
                }
                thumb = thumb.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

                // Was a previous item/title included here?
                if (i > 1)
                    Output.Write(",");
                Output.Write("{\"collection_item\":{\"name\":\"" + firstItemResult.Title.Trim().Replace("\"", "'") + "\",\"url\":\"" + base_url + titleResult.BibID + "/" + firstItemResult.VID + "\",\"collection_code\":\"\",\"id\":\"" + titleResult.BibID + "_" + firstItemResult.VID + "\",\"thumb_url\":\"" + thumb + "\"}}");

                i++;
            }

            Output.Write("]");
        }

        /// <summary> Display search results in simple XML format </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Args"></param>
        /// <param name="ResultsStats"></param>
        /// <param name="ResultsPage"></param>
        protected internal void legacy_xml_display_search_results(TextWriter Output, Results_Arguments Args, Search_Results_Statistics ResultsStats, List<iSearch_Title_Result> ResultsPage)
        {
            // Get the URL and network roots
            string image_url = Engine_ApplicationCache_Gateway.Settings.Image_URL;
            string network = Engine_ApplicationCache_Gateway.Settings.Image_Server_Network;
            string base_url = Engine_ApplicationCache_Gateway.Settings.Base_URL;
            if (HttpContext.Current != null)
            {
                base_url = HttpContext.Current.Request.Url.AbsoluteUri;
                if (base_url.IndexOf("?") > 0)
                    base_url = base_url.Substring(0, base_url.IndexOf("?")).Replace("sobekcm.svc", "");
            }
            if ((base_url.Length > 0) && (base_url[base_url.Length - 1] != '/'))
                base_url = base_url + "/";
            if ((image_url.Length > 0) && (image_url[image_url.Length - 1] != '/'))
                image_url = image_url + "/";

            // Write the header first
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<ResultSet Page=\"" + Args.Page + "\" Total=\"" + ResultsStats.Total_Titles + "\">");

            // Now, add XML for each title
            string lastBibID = string.Empty;
            foreach (iSearch_Title_Result thisResult in ResultsPage)
            {
                if (thisResult.BibID != lastBibID)
                {
                    if (lastBibID.Length > 0)
                        Output.WriteLine("</TitleResult>");
                    Output.WriteLine("<TitleResult ID=\"" + thisResult.BibID + "\">");
                    lastBibID = thisResult.BibID;
                }

                // Determine folder from BibID
                string folder = thisResult.BibID.Substring(0, 2) + "/" + thisResult.BibID.Substring(2, 2) + "/" + thisResult.BibID.Substring(4, 2) + "/" + thisResult.BibID.Substring(6, 2) + "/" + thisResult.BibID.Substring(8);

                // Now, add XML for each item
                for (int i = 0; i < thisResult.Item_Count; i++)
                {
                    iSearch_Item_Result itemResult = thisResult.Get_Item(i);
                    Output.WriteLine("\t<ItemResult ID=\"" + thisResult.BibID + "_" + itemResult.VID + "\">");
                    Output.Write("\t\t<Title>");
                    Write_XML(Output, itemResult.Title);
                    Output.WriteLine("</Title>");
                    if ( !String.IsNullOrEmpty(itemResult.PubDate))
                    {
                        Output.Write("\t\t<Date>");
                        Write_XML(Output, itemResult.PubDate);
                        Output.WriteLine("</Date>");
                    }
                    Output.WriteLine("\t\t<Location>");
                    Output.WriteLine("\t\t\t<URL>" + base_url + thisResult.BibID + "/" + itemResult.VID + "</URL>");

                    if (!String.IsNullOrEmpty(itemResult.MainThumbnail))
                    {
                        Output.WriteLine("\t\t\t<MainThumb>" + image_url + folder + "/" + itemResult.VID + "/" + itemResult.MainThumbnail + "</MainThumb>");
                    }

                    Output.WriteLine("\t\t\t<Folder type=\"web\">" + image_url + folder + "/" + itemResult.VID + "</Folder>");
                    Output.WriteLine("\t\t\t<Folder type=\"network\">" + network + folder.Replace("/", "\\") + "\\" + itemResult.VID + "</Folder>");
                    Output.WriteLine("\t\t</Location>");
                    Output.WriteLine("\t</ItemResult>");
                }
            }

            if (ResultsPage.Count > 0)
                Output.WriteLine("</TitleResult>");
            Output.WriteLine("</ResultSet>");
        }

        private static void Write_XML(TextWriter Output, string Value)
        {
            foreach (char thisChar in Value)
            {
                switch (thisChar)
                {
                    case '>':
                        Output.Write("&gt;");
                        break;

                    case '<':
                        Output.Write("&lt;");
                        break;

                    case '"':
                        Output.Write("&quot;");
                        break;

                    case '&':
                        Output.Write("&amp;");
                        break;

                    default:
                        Output.Write(thisChar);
                        break;
                }
            }
        }

        #endregion

        #region Helper methods that perform the actual searching

        /// <summary> Performs a search ( or retrieves the search results from the cache ) and outputs the results and search url used  </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Aggregation_Object"> Object for the current aggregation object, against which this search is performed </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Complete_Result_Set_Info"> [OUT] Information about the entire set of results </param>
        /// <param name="Paged_Results"> [OUT] List of search results for the requested page of results </param>
        public ResultsEndpointErrorEnum Get_Search_Results(Results_Arguments Current_Mode,
                                       Complete_Item_Aggregation Aggregation_Object, 
                                       Custom_Tracer Tracer,
                                       out Search_Results_Statistics Complete_Result_Set_Info,
                                       out List<iSearch_Title_Result> Paged_Results)
        {
            Tracer.Add_Trace("SobekCM_Assistant.Get_Search_Results", String.Empty);

            // Set output initially to null
            Paged_Results = null;
            Complete_Result_Set_Info = null;

            // Get the sort
            int sort = Current_Mode.Sort.HasValue ? Math.Max(Current_Mode.Sort.Value, ((ushort)1)) : 0;
            if ((sort != 0) && (sort != 1) && (sort != 2) && (sort != 10) && (sort != 11))
                sort = 0;

            // If there was no search, it is a browse
            if (String.IsNullOrEmpty(Current_Mode.Search_String))
            {
                // Get the page count in the results
                int current_page_index = Current_Mode.Page;

                // Set the flags for how much data is needed.  (i.e., do we need to pull ANYTHING?  or
                // perhaps just the next page of results ( as opposed to pulling facets again).
                bool need_browse_statistics = true;
                bool need_paged_results = true;
                if (Current_Mode.Use_Cache)
                {
                    // Look to see if the browse statistics are available on any cache for this browse
                    Complete_Result_Set_Info = CachedDataManager.Retrieve_Browse_Result_Statistics(Aggregation_Object.Code, "all", Tracer);
                    if (Complete_Result_Set_Info != null)
                        need_browse_statistics = false;

                    // Look to see if the paged results are available on any cache..
                    Paged_Results = CachedDataManager.Retrieve_Browse_Results(Aggregation_Object.Code, "all", current_page_index, sort, Tracer);
                    if (Paged_Results != null)
                        need_paged_results = false;
                }

                // Was a copy found in the cache?
                if ((!need_browse_statistics) && (!need_paged_results))
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("SobekCM_Assistant.Get_Browse_Info", "Browse statistics and paged results retrieved from cache");
                    }
                }
                else
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("SobekCM_Assistant.Get_Browse_Info", "Building results information");
                    }

                    // Try to pull more than one page, so we can cache the next page or so
                    List<List<iSearch_Title_Result>> pagesOfResults;

                    // Get from the hierarchy object
                        Multiple_Paged_Results_Args returnArgs = Item_Aggregation_Utilities.Gat_All_Browse(Aggregation_Object, current_page_index, sort, (int) Current_Mode.ResultsPerPage, Current_Mode.Use_Cache, need_browse_statistics, Tracer);
                        if (need_browse_statistics)
                        {
                            Complete_Result_Set_Info = returnArgs.Statistics;
                            foreach (Search_Facet_Collection thisFacet in Complete_Result_Set_Info.Facet_Collections)
                            {
                                Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID(thisFacet.MetadataTypeID);
                                thisFacet.MetadataTerm = field.Facet_Term;
                            }
                        }
                        pagesOfResults = returnArgs.Paged_Results;
                        if ((pagesOfResults != null) && (pagesOfResults.Count > 0))
                            Paged_Results = pagesOfResults[0];

                    // Save the overall result set statistics to the cache if something was pulled
                    if (Current_Mode.Use_Cache)
                    {
                        if ((need_browse_statistics) && (Complete_Result_Set_Info != null))
                        {
                            CachedDataManager.Store_Browse_Result_Statistics(Aggregation_Object.Code, "all", Complete_Result_Set_Info, Tracer);
                        }

                        // Save the overall result set statistics to the cache if something was pulled
                        if ((need_paged_results) && (Paged_Results != null))
                        {
                            CachedDataManager.Store_Browse_Results(Aggregation_Object.Code, "all", current_page_index, sort, pagesOfResults, Tracer);
                        }
                    }
                }

                return ResultsEndpointErrorEnum.NONE;
            }


            // Depending on type of search, either go to database or Greenstone
            if (Current_Mode.Search_Type == Search_Type_Enum.Map)
            {
                try
                {
                    double lat1 = 1000;
                    double long1 = 1000;
                    double lat2 = 1000;
                    double long2 = 1000;
                    string[] terms = Current_Mode.Coordinates.Split(",".ToCharArray());
                    if (terms.Length < 4)
                    {
                        lat1 = Convert.ToDouble(terms[0]);
                        lat2 = lat1;
                        long1 = Convert.ToDouble(terms[1]);
                        long2 = long1;
                    }
                    if (terms.Length >= 4)
                    {
                        if (terms[0].Length > 0)
                            lat1 = Convert.ToDouble(terms[0]);
                        if (terms[1].Length > 0)
                            long1 = Convert.ToDouble(terms[1]);
                        if (terms[2].Length > 0)
                            lat2 = Convert.ToDouble(terms[2]);
                        if (terms[3].Length > 0)
                            long2 = Convert.ToDouble(terms[3]);
                    }

                    // If just the first point is valid, use that
                    if ((lat2 == 1000) || (long2 == 1000))
                    {
                        lat2 = lat1;
                        long2 = long1;
                    }

                    // If just the second point is valid, use that
                    if ((lat1 == 1000) || (long1 == 1000))
                    {
                        lat1 = lat2;
                        long1 = long2;
                    }

                    // Perform the search against the database
                    try
                    {
                        // Get the page count in the results
                        int current_page_index = Current_Mode.Page;

                        // Try to pull more than one page, so we can cache the next page or so
                        Multiple_Paged_Results_Args returnArgs = Engine_Database.Get_Items_By_Coordinates(Current_Mode.Aggregation, lat1, long1, lat2, long2, false, 20, current_page_index, sort, false, new List<short>(), true, Tracer);
                        List<List<iSearch_Title_Result>> pagesOfResults = returnArgs.Paged_Results;
                        Complete_Result_Set_Info = returnArgs.Statistics;

                        if ((pagesOfResults != null) && (pagesOfResults.Count > 0))
                            Paged_Results = pagesOfResults[0];
                    }
                    catch (Exception ee)
                    {
                        // Next, show the message to the user
                        Tracer.Add_Trace("ResultServices.Get_Search_Results", "Exception caught while performing database coordinate search");
                        Tracer.Add_Trace("ResultServices.Get_Search_Results", "Exception: " + ee.Message);
                        Tracer.Add_Trace("ResultServices.Get_Search_Results", ee.StackTrace);

                        return ee.Message.ToUpper().IndexOf("TIMEOUT") >= 0 ? ResultsEndpointErrorEnum.Database_Timeout_Exception : ResultsEndpointErrorEnum.Database_Exception;
                    }
                }
                catch (Exception ee )
                {
                    // Next, show the message to the user
                    Tracer.Add_Trace("ResultServices.Get_Search_Results", "Exception: " + ee.Message);
                    Tracer.Add_Trace("ResultServices.Get_Search_Results", ee.StackTrace);
                    return ResultsEndpointErrorEnum.Unknown;
                }
            }
            else
            {
                List<string> terms = new List<string>();
                List<string> web_fields = new List<string>();

                // Split the terms correctly ( only use the database stop words for the split if this will go to the database ultimately)
                if ((Current_Mode.Search_Type == Search_Type_Enum.Full_Text) || (Current_Mode.Search_Fields.IndexOf("TX") >= 0))
                {
                    Split_Clean_Search_Terms_Fields(Current_Mode.Search_String, Current_Mode.Search_Fields, Current_Mode.Search_Type, terms, web_fields, null, Current_Mode.Search_Precision, ',');
                }
                else
                {
                    Split_Clean_Search_Terms_Fields(Current_Mode.Search_String, Current_Mode.Search_Fields, Current_Mode.Search_Type, terms, web_fields, Engine_ApplicationCache_Gateway.StopWords, Current_Mode.Search_Precision, ',');
                }

                // Get the count that will be used
                int actualCount = Math.Min(terms.Count, web_fields.Count);

                // Determine if this is a special search type which returns more rows and is not cached.
                // This is used to return the results as XML and DATASET
                int results_per_page = (int) Current_Mode.ResultsPerPage;

                // Determine if a date range was provided
                long date1 = -1;
                long date2 = -1;
                if (Current_Mode.DateRange_Date1.HasValue)
                {
                    date1 = Current_Mode.DateRange_Date1.Value;
                    if (Current_Mode.DateRange_Date2.HasValue)
                    {
                        if (Current_Mode.DateRange_Date2.Value >= Current_Mode.DateRange_Date1.Value)
                            date2 = Current_Mode.DateRange_Date2.Value;
                        else
                        {
                            date1 = Current_Mode.DateRange_Date2.Value;
                            date2 = Current_Mode.DateRange_Date1.Value;
                        }
                    }
                    else
                    {
                        date2 = date1;
                    }
                }
                if (date1 < 0)
                {
                    if ((Current_Mode.DateRange_Year1.HasValue) && (Current_Mode.DateRange_Year1.Value > 0))
                    {
                        DateTime startDate = new DateTime(Current_Mode.DateRange_Year1.Value, 1, 1);
                        TimeSpan timeElapsed = startDate.Subtract(new DateTime(1, 1, 1));
                        date1 = (long)timeElapsed.TotalDays;
                        if ((Current_Mode.DateRange_Year2.HasValue) && (Current_Mode.DateRange_Year2.Value > 0))
                        {
                            startDate = new DateTime(Current_Mode.DateRange_Year2.Value, 12, 31);
                            timeElapsed = startDate.Subtract(new DateTime(1, 1, 1));
                            date2 = (long)timeElapsed.TotalDays;
                        }
                        else
                        {
                            startDate = new DateTime(Current_Mode.DateRange_Year1.Value, 12, 31);
                            timeElapsed = startDate.Subtract(new DateTime(1, 1, 1));
                            date2 = (long)timeElapsed.TotalDays;
                        }
                    }
                }

                // Set the flags for how much data is needed.  (i.e., do we need to pull ANYTHING?  or
                // perhaps just the next page of results ( as opposed to pulling facets again).
                bool need_search_statistics = true;
                bool need_paged_results = true;
                if (Current_Mode.Use_Cache)
                {
                    // Look to see if the search statistics are available on any cache..
                    Complete_Result_Set_Info = CachedDataManager.Retrieve_Search_Result_Statistics(Current_Mode, actualCount, web_fields, terms, date1, date2, Tracer);
                    if (Complete_Result_Set_Info != null)
                        need_search_statistics = false;

                    // Look to see if the paged results are available on any cache..
                    Paged_Results = CachedDataManager.Retrieve_Search_Results(Current_Mode, sort, actualCount, web_fields, terms, date1, date2, Tracer);
                    if (Paged_Results != null)
                        need_paged_results = false;
                }

                // If both were retrieved, do nothing else
                if ((need_paged_results) || (need_search_statistics))
                {
                    // Should this pull the search from the database, or from greenstone?
                    if ((Current_Mode.Search_Type == Search_Type_Enum.Full_Text) || (Current_Mode.Search_Fields.IndexOf("TX") >= 0))
                    {
                        try
                        {
                            // Get the page count in the results
                            int current_page_index = Current_Mode.Page;

                            // Perform the search against greenstone
                            Search_Results_Statistics recomputed_search_statistics;
                            Perform_Solr_Search(Tracer, terms, web_fields, actualCount, Current_Mode.Aggregation, current_page_index, sort, results_per_page, out recomputed_search_statistics, out Paged_Results);
                            if (need_search_statistics)
                            {
                                Complete_Result_Set_Info = recomputed_search_statistics;

                                foreach (Search_Facet_Collection thisFacet in Complete_Result_Set_Info.Facet_Collections)
                                {
                                    Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID(thisFacet.MetadataTypeID);
                                    thisFacet.MetadataTerm = field.Facet_Term;
                                }
                            }
                        }
                        catch (Exception ee)
                        {
                            // Next, show the message to the user
                            Tracer.Add_Trace("ResultServices.Get_Search_Results", "Exception caught while performing solr/lucene search");
                            Tracer.Add_Trace("ResultServices.Get_Search_Results", "Exception: " + ee.Message);
                            Tracer.Add_Trace("ResultServices.Get_Search_Results", ee.StackTrace);
                            return ResultsEndpointErrorEnum.Unknown;
                        }

                        // If this was a special search, don't cache this
                        if (!Current_Mode.Use_Cache)
                        {
                            // Cache the search statistics, if it was needed
                            if ((need_search_statistics) && (Complete_Result_Set_Info != null))
                            {
                                CachedDataManager.Store_Search_Result_Statistics(Current_Mode, actualCount, web_fields, terms, date1, date2, Complete_Result_Set_Info, Tracer);
                            }

                            // Cache the search results
                            if ((need_paged_results) && (Paged_Results != null))
                            {
                                CachedDataManager.Store_Search_Results(Current_Mode, sort, actualCount, web_fields, terms, date1, date2, Paged_Results, Tracer);
                            }
                        }
                    }
                    else
                    {
                        // Try to pull more than one page, so we can cache the next page or so
                        List<List<iSearch_Title_Result>> pagesOfResults;

                        // Perform the search against the database
                        try
                        {
                            Search_Results_Statistics recomputed_search_statistics;
                            Perform_Database_Search(Tracer, terms, web_fields, date1, date2, actualCount, Current_Mode, sort, Aggregation_Object, results_per_page, Current_Mode.Use_Cache, out recomputed_search_statistics, out pagesOfResults, need_search_statistics);
                            if (need_search_statistics)
                            {
                                Complete_Result_Set_Info = recomputed_search_statistics;

                                foreach (Search_Facet_Collection thisFacet in Complete_Result_Set_Info.Facet_Collections)
                                {
                                    Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID(thisFacet.MetadataTypeID);
                                    thisFacet.MetadataTerm = field.Facet_Term;
                                }
                            }

                            if ((pagesOfResults != null) && (pagesOfResults.Count > 0))
                                Paged_Results = pagesOfResults[0];
                        }
                        catch (Exception ee)
                        {
                            // Next, show the message to the user
                            Tracer.Add_Trace("ResultServices.Get_Search_Results", "Exception caught while performing database coordinate search");
                            Tracer.Add_Trace("ResultServices.Get_Search_Results", "Exception: " + ee.Message);
                            Tracer.Add_Trace("ResultServices.Get_Search_Results", ee.StackTrace);

                            return ee.Message.ToUpper().IndexOf("TIMEOUT") >= 0 ? ResultsEndpointErrorEnum.Database_Timeout_Exception : ResultsEndpointErrorEnum.Database_Exception;
                        }

                        // If this was a special search, don't cache this
                        if (!Current_Mode.Use_Cache)
                        {
                            // Cache the search statistics, if it was needed
                            if ((need_search_statistics) && (Complete_Result_Set_Info != null))
                            {
                                CachedDataManager.Store_Search_Result_Statistics(Current_Mode, actualCount, web_fields, terms, date1, date2, Complete_Result_Set_Info, Tracer);
                            }

                            // Cache the search results
                            if ((need_paged_results) && (pagesOfResults != null))
                            {
                                CachedDataManager.Store_Search_Results(Current_Mode, sort, actualCount, web_fields, terms, date1, date2, pagesOfResults, Tracer);
                            }
                        }
                    }
                }
            }

            return ResultsEndpointErrorEnum.NONE;
        }

        /// <summary> Takes the search string and search fields from the URL and parses them, according to the search type,
        /// into a collection of terms and a collection of fields. Stop words are also suppressed here </summary>
        /// <param name="Search_String">Search string from the SobekCM search results URL </param>
        /// <param name="Search_Fields">Search fields from the SobekCM search results URL </param>
        /// <param name="Search_Type"> Type of search currently being performed (sets how it is parsed and default index)</param>
        /// <param name="Output_Terms"> List takes the results of the parsing of the actual search terms </param>
        /// <param name="Output_Fields"> List takes the results of the parsing of the actual (and implied) search fields </param>
        /// <param name="Search_Stop_Words"> List of all stop words ignored during metadata searching (such as 'The', 'A', etc..) </param>
        /// <param name="Search_Precision"> Search precision for this search ( i.e., exact, contains, stemmed, thesaurus lookup )</param>
        /// <param name="Delimiter_Character"> Character used as delimiter between different components of an advanced search</param>
        public static void Split_Clean_Search_Terms_Fields(string Search_String, string Search_Fields, Search_Type_Enum Search_Type, List<string> Output_Terms, List<string> Output_Fields, List<string> Search_Stop_Words, Search_Precision_Type_Enum Search_Precision, char Delimiter_Character)
        {
            // Find default index
            string default_index = "ZZ";
            if (Search_Type == Search_Type_Enum.Full_Text)
                default_index = "TX";

            // Split the parts
            string[] fieldSplitTemp = Search_Fields.Split(new[] { Delimiter_Character });
            List<string> fieldSplit = new List<string>();
            List<string> searchSplit = new List<string>();
            int first_index = 0;
            int second_index = 0;
            int field_index = 0;
            bool in_quotes = false;
            while (second_index < Search_String.Length)
            {
                if (in_quotes)
                {
                    if (Search_String[second_index] == '"')
                    {
                        in_quotes = false;
                    }
                }
                else
                {
                    if (Search_String[second_index] == '"')
                    {
                        in_quotes = true;
                    }
                    else
                    {
                        if (Search_String[second_index] == Delimiter_Character)
                        {
                            if (first_index < second_index)
                            {
                                string possible_add = Search_String.Substring(first_index, second_index - first_index);
                                if (possible_add.Trim().Length > 0)
                                {
                                    searchSplit.Add(possible_add);
                                    fieldSplit.Add(field_index < fieldSplitTemp.Length ? fieldSplitTemp[field_index] : default_index);
                                }
                            }
                            first_index = second_index + 1;
                            field_index++;
                        }
                        else if (Search_String[second_index] == ' ')
                        {
                            if (first_index < second_index)
                            {
                                string possible_add = Search_String.Substring(first_index, second_index - first_index);
                                if (possible_add.Trim().Length > 0)
                                {
                                    searchSplit.Add(possible_add);
                                    fieldSplit.Add(field_index < fieldSplitTemp.Length ? fieldSplitTemp[field_index] : default_index);
                                }
                            }
                            first_index = second_index + 1;
                        }
                    }
                }
                second_index++;
            }
            if (second_index > first_index)
            {
                searchSplit.Add(Search_String.Substring(first_index));
                fieldSplit.Add(field_index < fieldSplitTemp.Length ? fieldSplitTemp[field_index] : default_index);
            }

            // If this is basic, do some other preparation
            if (Search_Type == Search_Type_Enum.Full_Text)
            {
                Solr_Documents_Searcher.Split_Multi_Terms(Search_String, default_index, Output_Terms, Output_Fields);
            }
            else
            {
                // For advanced, just add all the terms
                Output_Terms.AddRange(searchSplit.Select(ThisTerm => ThisTerm.Trim().Replace("\"", "").Replace("+", " ")));
                Output_Fields.AddRange(fieldSplit.Select(ThisField => ThisField.Trim()));
            }

            // Some special work for basic searches here
            if (Search_Type == Search_Type_Enum.Basic)
            {
                while (Output_Fields.Count < Output_Terms.Count)
                {
                    Output_Fields.Add("ZZ");
                }
            }

            // Now, remove any stop words by themselves
            if (Search_Stop_Words != null)
            {
                int index = 0;
                while ((index < Output_Terms.Count) && (index < Output_Fields.Count))
                {
                    if ((Output_Terms[index].Length == 0) || (Search_Stop_Words.Contains(Output_Terms[index].ToLower())))
                    {
                        Output_Terms.RemoveAt(index);
                        Output_Fields.RemoveAt(index);
                    }
                    else
                    {
                        if (Search_Precision != Search_Precision_Type_Enum.Exact_Match)
                        {
                            Output_Terms[index] = Output_Terms[index].Replace("\"", "").Replace("+", " ").Replace("&amp;", " ").Replace("&", "");
                        }
                        if (Output_Fields[index].Length == 0)
                            Output_Fields[index] = default_index;
                        index++;
                    }
                }
            }
        }

        private void Perform_Database_Search(Custom_Tracer Tracer, List<string> Terms, List<string> Web_Fields, long Date1, long Date2, int ActualCount, Results_Arguments Current_Mode, int Current_Sort, Complete_Item_Aggregation Aggregation_Object, int Results_Per_Page, bool Potentially_Include_Facets, out Search_Results_Statistics Complete_Result_Set_Info, out List<List<iSearch_Title_Result>> Paged_Results, bool Need_Search_Statistics)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Assistant.Perform_Database_Search", "Query the database for search results");
            }

            // Get the list of facets first
            List<short> facetsList = Aggregation_Object.Facets;
            if (!Potentially_Include_Facets)
                facetsList.Clear();

            // Set the return values to NULL initially
            Complete_Result_Set_Info = null;

            const bool INCLUDE_PRIVATE = false;

            List<short> links = new List<short>();
            List<short> db_fields = new List<short>();
            List<string> db_terms = Terms.ToList();

            // Step through all the web fields and convert to db fields
            for (int i = 0; i < ActualCount; i++)
            {
                if (Web_Fields[i].Length > 1)
                {
                    // Find the joiner
                    if ((Web_Fields[i][0] == '+') || (Web_Fields[i][0] == '=') || (Web_Fields[i][0] == '-'))
                    {
                        if (Web_Fields[i][0] == '+')
                            links.Add(0);
                        if (Web_Fields[i][0] == '=')
                            links.Add(1);
                        if (Web_Fields[i][0] == '-')
                            links.Add(2);

                        Web_Fields[i] = Web_Fields[i].Substring(1);
                    }
                    else
                    {
                        links.Add(0);
                    }

                    // Find the db field number
                    db_fields.Add(Metadata_Field_Number(Web_Fields[i]));
                }

                // Also add starting and ending quotes to all the valid searches
                if (db_terms[i].Length > 0)
                {
                    if ((db_terms[i].IndexOf("\"") < 0) && (db_terms[i].IndexOf(" ") < 0))
                    {
                        // Since this is a single word, see what type of special codes to include
                        switch (Current_Mode.Search_Precision)
                        {
                            case Search_Precision_Type_Enum.Contains:
                                db_terms[i] = "\"" + db_terms[i] + "\"";
                                break;

                            case Search_Precision_Type_Enum.Inflectional_Form:
                                // If there are any non-characters, don't use inflectional for this term
                                bool inflectional = db_terms[i].All(Char.IsLetter);
                                if (inflectional)
                                {
                                    db_terms[i] = "FORMSOF(inflectional," + db_terms[i] + ")";
                                }
                                else
                                {
                                    db_terms[i] = "\"" + db_terms[i] + "\"";
                                }
                                break;

                            case Search_Precision_Type_Enum.Synonmic_Form:
                                // If there are any non-characters, don't use thesaurus for this term
                                bool thesaurus = db_terms[i].All(Char.IsLetter);
                                if (thesaurus)
                                {
                                    db_terms[i] = "FORMSOF(thesaurus," + db_terms[i] + ")";
                                }
                                else
                                {
                                    db_terms[i] = "\"" + db_terms[i] + "\"";
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (Current_Mode.Search_Precision != Search_Precision_Type_Enum.Exact_Match)
                        {
                            db_terms[i] = "\"" + db_terms[i] + "\"";
                        }
                    }
                }
            }

            // Get the page count in the results
            int current_page_index =Current_Mode.Page;

            // If this is an exact match, just do the search
            if (Current_Mode.Search_Precision == Search_Precision_Type_Enum.Exact_Match)
            {
                Multiple_Paged_Results_Args returnArgs = Engine_Database.Perform_Metadata_Exact_Search_Paged(db_terms[0], db_fields[0], INCLUDE_PRIVATE, Current_Mode.Aggregation, Date1, Date2, Results_Per_Page, current_page_index, Current_Sort, Need_Search_Statistics, facetsList, Need_Search_Statistics, Tracer);
                if (Need_Search_Statistics)
                    Complete_Result_Set_Info = returnArgs.Statistics;
                Paged_Results = returnArgs.Paged_Results;
            }
            else
            {
                // Finish filling up the fields and links
                while (links.Count < 10)
                    links.Add(0);
                while (db_fields.Count < 10)
                    db_fields.Add(-1);
                while (db_terms.Count < 10)
                    db_terms.Add(String.Empty);

                // See if this is a simple search, which can use a more optimized search routine
                bool simplified_search = db_fields.All(Field => (Field <= 0));

                // Perform either the simpler metadata search, or the more complex
                if (simplified_search)
                {
                    StringBuilder searchBuilder = new StringBuilder();
                    for (int i = 0; i < db_terms.Count; i++)
                    {
                        if (db_terms[i].Length > 0)
                        {
                            if (i > 0)
                            {
                                if (i > links.Count)
                                {
                                    searchBuilder.Append(" AND ");
                                }
                                else
                                {
                                    switch (links[i - 1])
                                    {
                                        case 0:
                                            searchBuilder.Append(" AND ");
                                            break;

                                        case 1:
                                            searchBuilder.Append(" OR ");
                                            break;

                                        case 2:
                                            searchBuilder.Append(" AND NOT ");
                                            break;
                                    }
                                }
                            }

                            searchBuilder.Append(db_terms[i]);
                        }
                    }



                    Multiple_Paged_Results_Args returnArgs = Engine_Database.Perform_Metadata_Search_Paged(searchBuilder.ToString(), INCLUDE_PRIVATE, Current_Mode.Aggregation, Date1, Date2, Results_Per_Page, current_page_index, Current_Sort, Need_Search_Statistics, facetsList, Need_Search_Statistics, Tracer);
                    if (Need_Search_Statistics)
                        Complete_Result_Set_Info = returnArgs.Statistics;
                    Paged_Results = returnArgs.Paged_Results;
                }
                else
                {
                    // Perform search in the database
                    Multiple_Paged_Results_Args returnArgs = Engine_Database.Perform_Metadata_Search_Paged(links[0], db_terms[0], db_fields[0], links[1], db_terms[1], db_fields[1], links[2], db_terms[2], db_fields[2], links[3], db_terms[3],
                                                                                                            db_fields[3], links[4], db_terms[4], db_fields[4], links[5], db_terms[5], db_fields[5], links[6], db_terms[6], db_fields[6], links[7], db_terms[7], db_fields[7], links[8], db_terms[8], db_fields[8],
                                                                                                            links[9], db_terms[9], db_fields[9], INCLUDE_PRIVATE, Current_Mode.Aggregation, Date1, Date2, Results_Per_Page, current_page_index, Current_Sort, Need_Search_Statistics, facetsList, Need_Search_Statistics, Tracer);
                    if (Need_Search_Statistics)
                        Complete_Result_Set_Info = returnArgs.Statistics;
                    Paged_Results = returnArgs.Paged_Results;
                }
            }
        }

        private static short Metadata_Field_Number(string FieldCode)
        {
            Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_Code(FieldCode);
            return (field == null) ? (short)-1 : field.ID;
        }

        private static void Perform_Solr_Search(Custom_Tracer Tracer, List<string> Terms, List<string> Web_Fields, int ActualCount, string Current_Aggregation, int Current_Page, int Current_Sort, int Results_Per_Page, out Search_Results_Statistics Complete_Result_Set_Info, out List<iSearch_Title_Result> Paged_Results)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Assistant.Perform_Solr_Search", "Build the Solr query");
            }

            // Step through all the terms and fields
            StringBuilder queryStringBuilder = new StringBuilder();
            for (int i = 0; i < ActualCount; i++)
            {
                string web_field = Web_Fields[i];
                string searchTerm = Terms[i];
                string solr_field;

                if (i == 0)
                {
                    // Skip any joiner for the very first field indicated
                    if ((web_field[0] == '+') || (web_field[0] == '=') || (web_field[0] == '-'))
                    {
                        web_field = web_field.Substring(1);
                    }

                    // Try to get the solr field
                    if (web_field == "TX")
                    {
                        solr_field = "fulltext:";
                    }
                    else
                    {
                        Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_Code(web_field.ToUpper());
                        if (field != null)
                        {
                            solr_field = field.Solr_Field + ":";
                        }
                        else
                        {
                            solr_field = String.Empty;
                        }
                    }

                    // Add the solr search string
                    if (searchTerm.IndexOf(" ") > 0)
                    {
                        queryStringBuilder.Append("(" + solr_field + "\"" + searchTerm.Replace(":", "") + "\")");
                    }
                    else
                    {
                        queryStringBuilder.Append("(" + solr_field + searchTerm.Replace(":", "") + ")");
                    }
                }
                else
                {
                    // Add the joiner for this subsequent terms
                    if ((web_field[0] == '+') || (web_field[0] == '=') || (web_field[0] == '-'))
                    {
                        switch (web_field[0])
                        {
                            case '=':
                                queryStringBuilder.Append(" OR ");
                                break;

                            case '+':
                                queryStringBuilder.Append(" AND ");
                                break;

                            case '-':
                                queryStringBuilder.Append(" NOT ");
                                break;

                            default:
                                queryStringBuilder.Append(" AND ");
                                break;
                        }
                        web_field = web_field.Substring(1);
                    }
                    else
                    {
                        queryStringBuilder.Append(" AND ");
                    }

                    // Try to get the solr field
                    if (web_field == "TX")
                    {
                        solr_field = "fulltext:";
                    }
                    else
                    {
                        Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_Code(web_field.ToUpper());
                        if (field != null)
                        {
                            solr_field = field.Solr_Field + ":";
                        }
                        else
                        {
                            solr_field = String.Empty;
                        }
                    }

                    // Add the solr search string
                    if (searchTerm.IndexOf(" ") > 0)
                    {
                        queryStringBuilder.Append("(" + solr_field + "\"" + searchTerm.Replace(":", "") + "\")");
                    }
                    else
                    {
                        queryStringBuilder.Append("(" + solr_field + searchTerm.Replace(":", "") + ")");
                    }
                }
            }

            // Use this built query to query against Solr
            Solr_Documents_Searcher.Search(Current_Aggregation, queryStringBuilder.ToString(), Results_Per_Page, Current_Page, (ushort)Current_Sort, Tracer, out Complete_Result_Set_Info, out Paged_Results);
        }

        #endregion
    }
}
