#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.ModelBinding;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Microservices;

#endregion

namespace SobekCM.Engine_Library
{
    /// <summary> Handler is used to handle any incoming requests for a microservice exposed by the engine </summary>
    public class MicroserviceHandler : IHttpHandler
    {
        private Microservices_Configuration microserviceConfig;
        
        /// <summary> Processes the request </summary>
        /// <param name="Context">The context for the current request </param>
        public void ProcessRequest(HttpContext Context)
        {
            Engine_Database.Connection_String = Engine_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String;

            // Get the original query string
            string queryString = Context.Request.QueryString["urlrelative"];
            if (!String.IsNullOrEmpty(queryString))
            {
                // Make sure the microservices configuration has been read
                if (microserviceConfig == null)
                {
                    string path = Context.Server.MapPath("config/default/sobekcm_engine.config");
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
                Context.Response.Charset = Encoding.UTF8.WebName;

                // Set this to allow us to have our own error messages, without IIS jumping into it
                Context.Response.TrySkipIisCustomErrors = true;

                // Get any matching endpoint configuration
                Microservice_Endpoint endpoint = microserviceConfig.Get_Endpoint(paths);
                if (endpoint == null)
                {
                    Context.Response.ContentType = "text/plain";
                    Context.Response.StatusCode = 501;
                    Context.Response.Write("No endpoint found");
                    
                }
                else
                {
                    // Ensure this is allowed in the range
                    string requestIp = Context.Request.UserHostAddress;
                    if (!endpoint.AccessPermitted(requestIp))
                    {
                        Context.Response.ContentType = "text/plain";
                        Context.Response.StatusCode = 403;
                        Context.Response.Write("You are forbidden from accessing this endpoint");
                        return;
                    }

                    // Set the protocoal
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                        Context.Response.ContentType = "application/json";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P)
                        Context.Response.ContentType = "application/javascript";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.PROTOBUF)
                        Context.Response.ContentType = "application/octet-stream";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.XML)
                        Context.Response.ContentType = "text/xml";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.SOAP)
                        Context.Response.ContentType = "text/xml";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.BINARY)
                        Context.Request.ContentType = "application/octet-stream";

                    // Determine if this is currently in a valid DEBUG mode
                    bool debug = (Context.Request.QueryString["debug"] == "debug");

                    try
                    {
                        endpoint.Invoke(Context.Response, paths, Context.Request.QueryString, Context.Request.Form, debug);
                    }
                    catch (Exception ee)
                    {
                        Context.Response.ContentType = "text/plain";
                        Context.Response.Output.WriteLine("Error invoking the exception method: " + ee.Message);
                        Context.Response.StatusCode = 500;
                    }
                }
            }
            else
            {
                Context.Response.ContentType = "text/plain";
                Context.Response.StatusCode = 400;
                Context.Response.Write("Invalid URI - No endpoint requested");
            }
        }

        /// <summary> Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance. </summary>
        public bool IsReusable { get { return true;  } }
    }
}
