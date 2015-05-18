using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Resource_Object.Database;

namespace SobekCM.Engine_Library
{
    public class MicroserviceHandler : IHttpHandler
    {
        private Microservices_Configuration microserviceConfig;

        public void ProcessRequest(HttpContext context)
        {
            Engine_Database.Connection_String = Engine_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String;

            // Get the original query string
            string queryString = context.Request.QueryString["urlrelative"];
            if (!String.IsNullOrEmpty(queryString))
            {
                // Make sure the microservices configuration has been read
                if (microserviceConfig == null)
                {
                    string path = context.Server.MapPath("config/default/sobekcm_engine.config");
                    microserviceConfig = Microservices_Config_Reader.Read_Config(path);
                }

                // Collect the requested paths
                string[] splitter;
                if (( queryString.IndexOf("/") == 0 ) && ( queryString.Length > 1 ))
                    splitter = queryString.Substring(1).Split("/".ToCharArray());
                else
                    splitter = queryString.Split("/".ToCharArray());
                List<string> paths = splitter.ToList();

                // Set the encoding
                context.Response.Charset = Encoding.UTF8.WebName;

                // Set this to allow us to have our own error messages, without IIS jumping into it
                context.Response.TrySkipIisCustomErrors = true;

                // Get any matching endpoint configuration
                Microservice_Endpoint endpoint = microserviceConfig.Get_Endpoint(paths);
                if (endpoint == null)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = 501;
                    context.Response.Write("No endpoint found");
                    
                }
                else
                {
                    // Ensure this is allowed in the range
                    string requestIp = context.Request.UserHostAddress;
                    if (!endpoint.AccessPermitted(requestIp))
                    {
                        context.Response.ContentType = "text/plain";
                        context.Response.StatusCode = 403;
                        context.Response.Write("You are forbidden from accessing this endpoint");
                        return;
                    }

                    // Set the protocoal
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                        context.Response.ContentType = "application/json";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P)
                        context.Response.ContentType = "application/javascript";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.PROTOBUF)
                        context.Response.ContentType = "application/octet-stream";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.XML)
                        context.Response.ContentType = "text/xml";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.SOAP)
                        context.Response.ContentType = "text/xml";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.BINARY)
                        context.Request.ContentType = "application/octet-stream";

                    try
                    {
                        endpoint.Invoke(context.Response, paths, context.Request.QueryString, context.Request.Form);
                    }
                    catch (Exception ee)
                    {
                        context.Response.ContentType = "text/plain";
                        context.Response.Output.WriteLine("Error invoking the exception method: " + ee.Message);
                        context.Response.StatusCode = 500;
                    }
                }
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = 400;
                context.Response.Write("Invalid URI - No endpoint requested");
            }
        }

        public bool IsReusable { get { return true;  } }
    }
}
