using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Jil;
using ProtoBuf;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Tools;

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Endpoint supports services related to basic navigation, such 
    /// as resolving the URL to the Navigation_Object </summary>
    public class NavigationServices
    {
        /// <summary> Resolve the URL to a Navigation_Object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void ResolveUrl(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            // Pull out the http request
            HttpRequest request = HttpContext.Current.Request;

            // Get the base url
            string base_url = request.Url.AbsoluteUri.ToLower().Replace("sobekcm.aspx", "").Replace("sobekcm.svc", "");
            if (base_url.IndexOf("?") > 0)
                base_url = base_url.Substring(0, base_url.IndexOf("?"));

            Custom_Tracer tracer = new Custom_Tracer();

            Navigation_Object returnValue = get_navigation_object( QueryString, base_url, request.UserLanguages, tracer);

            returnValue.Base_URL = base_url;
            returnValue.Browser_Type = request.Browser.Type.ToUpper();
            returnValue.Set_Robot_Flag(request.UserAgent, request.UserHostAddress);

            switch (Protocol)
            {
                case Microservice_Endpoint_Protocol_Enum.JSON:
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                    break;

                case Microservice_Endpoint_Protocol_Enum.JSON_P:
                    Response.Output.Write("parseUrlResolver(");
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                    Response.Output.Write(");");
                    break;

                case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                    Serializer.Serialize(Response.OutputStream, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.XML:
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(returnValue.GetType());
                    x.Serialize(Response.Output, returnValue);
                    break;
            }
        }


        public Navigation_Object get_navigation_object(NameValueCollection RequestQueryString, string BaseUrl, string[] RequestUserLanguages, Custom_Tracer tracer)
        {
            NameValueCollection keys = RequestQueryString.Copy();

            string redirect_url = RequestQueryString["urlrelative"];
            redirect_url = redirect_url.Replace("/url-resolver/json", "").Replace("/url-resolver/json-p", "").Replace("/url-resolver/protobuf", "").Replace("/url-resolver/xml", "");
            keys["urlrelative"] = redirect_url;

            Navigation_Object currentMode = new Navigation_Object();
            SobekCM_QueryString_Analyzer.Parse_Query(keys, currentMode, BaseUrl, RequestUserLanguages, Engine_ApplicationCache_Gateway.Codes, Engine_ApplicationCache_Gateway.Collection_Aliases, Engine_ApplicationCache_Gateway.Items, Engine_ApplicationCache_Gateway.URL_Portals, tracer);



            return currentMode;
        }
    }
}
