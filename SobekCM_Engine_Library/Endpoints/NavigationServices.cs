#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Endpoint supports services related to basic navigation, such 
    /// as resolving the URL to the Navigation_Object </summary>
    public class NavigationServices : EndpointBase
    {
        /// <summary> Resolve the URL to a Navigation_Object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void ResolveUrl(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("NavigationServices.ResolveUrl", "Parse request and return navigation object");

            try
            {

                // Pull out the http request
                tracer.Add_Trace("NavigationServices.ResolveUrl", "Get the current request HttpRequest object");
                HttpRequest request = HttpContext.Current.Request;

                // Get the base url
                string base_url = request.Url.AbsoluteUri.ToLower().Replace("sobekcm.aspx", "").Replace("sobekcm.svc", "");
                if (base_url.IndexOf("?") > 0)
                    base_url = base_url.Substring(0, base_url.IndexOf("?"));


                tracer.Add_Trace("NavigationServices.ResolveUrl", "Get the navigation object");
                Navigation_Object returnValue = get_navigation_object(QueryString, base_url, request.UserLanguages, tracer);

                tracer.Add_Trace("NavigationServices.ResolveUrl", "Set base url and browser type (may not be useful)");
                returnValue.Base_URL = base_url;
                returnValue.Browser_Type = request.Browser.Type.ToUpper();

                tracer.Add_Trace("NavigationServices.ResolveUrl", "Determine if the user host address was a robot request");
                returnValue.Set_Robot_Flag(request.UserAgent, request.UserHostAddress);

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parseUrlResolver";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnValue, Response, Protocol, json_callback);
            }
            catch (Exception ee)
            {
                if ( IsDebug )
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("EXCEPTION CAUGHT!");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.StackTrace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    return;
                }

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error completing request");
                Response.StatusCode = 500;
            }
        }


        /// <summary> Resolve the requested URL (sepecified via the base url and the query string ) into a SobekCM navigation object </summary>
        /// <param name="RequestQueryString"> Query string, from the request The request query string.</param>
        /// <param name="BaseUrl"> Base URL of the original request </param>
        /// <param name="RequestUserLanguages"> List of user languages requested (via browser settings) </param>
        /// <param name="Tracer">The tracer.</param>
        /// <returns> Fully built navigation controller object </returns>
        public Navigation_Object get_navigation_object(NameValueCollection RequestQueryString, string BaseUrl, string[] RequestUserLanguages, Custom_Tracer Tracer)
        {
            NameValueCollection keys = RequestQueryString.Copy();

            string redirect_url = RequestQueryString["urlrelative"];
            redirect_url = redirect_url.Replace("/url-resolver/json", "").Replace("/url-resolver/json-p", "").Replace("/url-resolver/protobuf", "").Replace("/url-resolver/xml", "");
            keys["urlrelative"] = redirect_url;

            Navigation_Object currentMode = new Navigation_Object();
            QueryString_Analyzer.Parse_Query(keys, currentMode, BaseUrl, RequestUserLanguages, Engine_ApplicationCache_Gateway.Codes, Engine_ApplicationCache_Gateway.Collection_Aliases, Engine_ApplicationCache_Gateway.Items, Engine_ApplicationCache_Gateway.URL_Portals, Engine_ApplicationCache_Gateway.WebContent_Hierarchy, Tracer);

            return currentMode;
        }
    }
}
