using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Results;
using SobekCM.Core.ResultTitle;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Resource_Object;
using SobekCM.Tools;
using SolrNet.DSL;

namespace SobekCM.Engine_Library.Endpoints
{
    public class EbscoServices : ResultsServices
    {

        /// <summary> Get just the search statistics information for a search or browse </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void EBSCO_Results_XML(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("EbscoServices.Get_Search_Results_Set", "Parse request to determine search requested");

            // Get all the searh field necessary from the query string
            Results_Arguments args = new Results_Arguments(QueryString);

            // Additional results arguments
            // limit number of results
            int artificial_result_limitation = -1;
            if (!String.IsNullOrEmpty(QueryString["limit_results"]))
            {
                Int32.TryParse(QueryString["limit_results"], out artificial_result_limitation);
            }

            // limit title length, in words
            int artificial_title_length_limitation = -1;
            if (!String.IsNullOrEmpty(QueryString["title_length"]))
            {
                Int32.TryParse(QueryString["title_length"], out artificial_title_length_limitation);
            }

            // Was a collection indicated?
            if (UrlSegments.Count > 0)
                args.Aggregation = UrlSegments[0];

            // Get the aggregation object (we need to know which facets to use, etc.. )
            tracer.Add_Trace("EbscoServices.Get_Search_Results_Set", "Get the '" + args.Aggregation + "' item aggregation (for facets, etc..)");
            Complete_Item_Aggregation aggr = AggregationServices.get_complete_aggregation(args.Aggregation, true, tracer);

            // If no aggregation was returned, that is an error
            if (aggr == null)
            {
                tracer.Add_Trace("EbscoServices.Get_Search_Results_Set", "Returned aggregation was NULL... aggregation code may not be valid");

                if (IsDebug)
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
            tracer.Add_Trace("EbscoServices.Get_Search_Results_Set", "Perform the search");
            Search_Results_Statistics resultsStats;
            List<iSearch_Title_Result> resultsPage;
            ResultsEndpointErrorEnum error = Get_Search_Results(args, aggr, false, tracer, out resultsStats, out resultsPage);


            // Was this in debug mode?
            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("EbscoServices.Get_Search_Results_Set", "Debug mode detected");

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);
                return;
            }

            tracer.Add_Trace("EbscoServices.Get_Search_Results_Set", "Begin writing the XML result to the response");
            Response.Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
            Response.Output.WriteLine("<results>");

            Response.Output.WriteLine("  <stats total_items=\"" + resultsStats.Total_Items + "\" total_titles=\"" + resultsStats.Total_Titles + "\" />");


            // Map to the results object title / item
            tracer.Add_Trace("ResultsServices.Get_Search_Results_Set", "Map to the results object title / item");
            foreach (iSearch_Title_Result thisResult in resultsPage)
            {
                // Every results should have an item
                if (thisResult.Item_Count == 0)
                    continue;

                // add each descriptive field over
                iSearch_Item_Result itemResult = thisResult.Get_Item(0);

                string bibid = thisResult.BibID;
                string title = thisResult.GroupTitle;

                string vid = itemResult.VID;
                string thumbnail = itemResult.MainThumbnail;

                Response.Output.WriteLine("  <result bibid=\"" + bibid + "\" vid=\"" + vid + "\">");
                Response.Output.WriteLine("    <url>" + Engine_ApplicationCache_Gateway.Settings.Servers.Base_URL + bibid + "/" + vid + "</url>");
                Response.Output.WriteLine("    <title>" + HttpUtility.HtmlEncode(title) + "</title>");
                Response.Output.WriteLine("    <thumbnail>" + Engine_ApplicationCache_Gateway.Settings.Servers.Image_URL +
                                          SobekFileSystem.AssociFilePath(bibid, vid).Replace("\\", "/") + thumbnail + "</thumbnail>");

                int field_index = 0;
                foreach (string metadataTerm in resultsStats.Metadata_Labels)
                {
                    if (!String.IsNullOrWhiteSpace(thisResult.Metadata_Display_Values[field_index]))
                    {
                        // how to display this metadata field?
                        string metadataTermDisplay = metadataTerm.ToLower();

                        string termString = thisResult.Metadata_Display_Values[field_index];

                        if (termString.IndexOf("|") > 0)
                        {
                            string[] splitter = termString.Split("|".ToCharArray());
                            foreach (string thisSplit in splitter)
                            {
                                if (!String.IsNullOrWhiteSpace(thisSplit))
                                {
                                    Response.Output.WriteLine("    <" + metadataTermDisplay + ">" + thisSplit.Trim() + "</" + metadataTermDisplay + ">");
                                }
                            }
                        }
                        else
                        {
                            Response.Output.WriteLine("    <" + metadataTermDisplay + ">" + termString.Trim() + "</" + metadataTermDisplay + ">");
                        }
                    }
                    field_index++;
                }

                Response.Output.WriteLine("  </result>");
            }

            Response.Output.WriteLine("</results>");

            tracer.Add_Trace("EbscoServices.Get_Search_Results_Set", "Done writing the XML result to the response");



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
        }

        /// <summary> Get just the search statistics information for a search or browse </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void EBSCO_Results_JSON(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("EbscoServices.EBSCO_Results_JSON", "Parse request to determine search requested");

            // Get all the searh field necessary from the query string
            Results_Arguments args = new Results_Arguments(QueryString);

            // Additional results arguments
            // limit number of results
            int artificial_result_limitation = -1;
            if (!String.IsNullOrEmpty(QueryString["limit_results"]))
            {
                Int32.TryParse(QueryString["limit_results"], out artificial_result_limitation);
            }

            // limit title length, in words
            int artificial_title_length_limitation = -1;
            if (!String.IsNullOrEmpty(QueryString["title_length"]))
            {
                Int32.TryParse(QueryString["title_length"], out artificial_title_length_limitation);
            }

            // Was a collection indicated?
            if (UrlSegments.Count > 0)
                args.Aggregation = UrlSegments[0];

            // Get the aggregation object (we need to know which facets to use, etc.. )
            tracer.Add_Trace("EbscoServices.EBSCO_Results_JSON", "Get the '" + args.Aggregation + "' item aggregation (for facets, etc..)");
            Complete_Item_Aggregation aggr = AggregationServices.get_complete_aggregation(args.Aggregation, true, tracer);

            // If no aggregation was returned, that is an error
            if (aggr == null)
            {
                tracer.Add_Trace("EbscoServices.EBSCO_Results_JSON", "Returned aggregation was NULL... aggregation code may not be valid");

                if (IsDebug)
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
            tracer.Add_Trace("EbscoServices.EBSCO_Results_JSON", "Perform the search");
            Search_Results_Statistics resultsStats;
            List<iSearch_Title_Result> resultsPage;
            ResultsEndpointErrorEnum error = Get_Search_Results(args, aggr, false, tracer, out resultsStats, out resultsPage);


            // Was this in debug mode?
            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);
                return;
            }

            Response.Output.WriteLine("{\"stats\":{\"total_items\":\"" + resultsStats.Total_Items + "\",\"total_titles\":\"" + resultsStats.Total_Titles + "\"},");
            Response.Output.WriteLine(" \"results\":[");

            // Map to the results object title / item
            tracer.Add_Trace("ResultsServices.EBSCO_Results_JSON", "Map to the results object title / item");
            int items_counter = 0;
            foreach (iSearch_Title_Result thisResult in resultsPage)
            {
                // Every results should have an item
                if (thisResult.Item_Count == 0)
                    continue;

                // Was this NOT the first item?
                if (items_counter > 0)
                {
                    Response.Output.WriteLine(",");
                }
                Response.Output.Write("        ");
                items_counter++;


                // add each descriptive field over
                iSearch_Item_Result itemResult = thisResult.Get_Item(0);

                string bibid = thisResult.BibID;
                string title = thisResult.GroupTitle;

                string vid = itemResult.VID;
                string thumbnail = itemResult.MainThumbnail;

                // {"bibid":"1212", "vid":"00001", "title":"sdsd", "subjects":["subj1", "subj2", "subj3"] },

                Response.Output.Write("{ \"bibid\":\"" + bibid + "\", \"vid\":\"" + vid + "\", ");
                Response.Output.Write("\"url\":\"" + Engine_ApplicationCache_Gateway.Settings.Servers.Base_URL + bibid + "/" + vid + "\",");
                Response.Output.Write("\"title\":\"" + HttpUtility.HtmlEncode(title) + "\",");
                Response.Output.Write("\"thumbnail\":\"" + Engine_ApplicationCache_Gateway.Settings.Servers.Image_URL +
                                          SobekFileSystem.AssociFilePath(bibid, vid).Replace("\\", "/") + thumbnail + "\"");

                int field_index = 0;

                foreach (string metadataTerm in resultsStats.Metadata_Labels)
                {
                    if (!String.IsNullOrWhiteSpace(thisResult.Metadata_Display_Values[field_index]))
                    {
                        // how to display this metadata field?
                        string metadataTermDisplay = metadataTerm.ToLower();

                        string termString = thisResult.Metadata_Display_Values[field_index];
                        Response.Output.Write(",\"" + metadataTermDisplay + "\":[");

                        int individual_term_counter = 0;
                        if (termString.IndexOf("|") > 0)
                        {
                            string[] splitter = termString.Split("|".ToCharArray());
                            foreach (string thisSplit in splitter)
                            {
                                if (!String.IsNullOrWhiteSpace(thisSplit))
                                {
                                    if ( individual_term_counter > 0 )
                                        Response.Output.Write(", \"" + thisSplit.Trim() + "\"");
                                    else
                                        Response.Output.Write("\"" + thisSplit.Trim() + "\"");

                                    individual_term_counter++;
                                }
                            }
                        }
                        else
                        {
                            Response.Output.Write("\"" + termString.Trim() + "\"");
                        }

                        Response.Output.Write("]");
                    }
                    field_index++;
                }

                Response.Output.Write("}");
            }

            Response.Output.WriteLine();
            Response.Output.WriteLine("    ]");
            Response.Output.WriteLine("} ");



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
        }
    }
}
