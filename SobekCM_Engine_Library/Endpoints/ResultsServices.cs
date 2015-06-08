using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using Jil;
using ProtoBuf;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Tools;

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Endpoint supports services related to search results (or item browses) across the 
    /// entire instance or a subset of aggregations </summary>
    public class ResultsServices
    {
        /// <summary> Get just the search statistics information for a search or browse </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        public void Get_Search_Statistics(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("ResultsServices.ResolveUrl", "Parse request and return search result statistcs only");

            // Get all the searh field necessary from the query string

            
                    //case WebContentServices.WebContentEndpointErrorEnum.Error_Reading_File:
                    //    Response.ContentType = "text/plain";
                    //    Response.Output.WriteLine("Unable to read existing source file");
                    //    Response.StatusCode = 500;
                    //    return;

                    //case WebContentServices.WebContentEndpointErrorEnum.No_File_Found:
                    //    Response.ContentType = "text/plain";
                    //    Response.Output.WriteLine("Source file does not exist");
                    //    Response.StatusCode = 404;
                    //    return;

                    //default:
                    //    Response.ContentType = "text/plain";
                    //    Response.Output.WriteLine("Error occurred");
                    //    Response.StatusCode = 500;
                    //    return;
                

            //switch (Protocol)
            //{
            //    case Microservice_Endpoint_Protocol_Enum.JSON:
            //        JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
            //        break;

            //    case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
            //        Serializer.Serialize(Response.OutputStream, returnValue);
            //        break;

            //    case Microservice_Endpoint_Protocol_Enum.JSON_P:
            //        Response.Output.Write("parseCollectionStaticPage(");
            //        JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
            //        Response.Output.Write(");");
            //        break;

            //    case Microservice_Endpoint_Protocol_Enum.XML:
            //        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(returnValue.GetType());
            //        x.Serialize(Response.Output, returnValue);
            //        break;

            //    case Microservice_Endpoint_Protocol_Enum.BINARY:
            //        IFormatter binary = new BinaryFormatter();
            //        binary.Serialize(Response.OutputStream, returnValue);
            //        break;
            //}
        }
    }
}
