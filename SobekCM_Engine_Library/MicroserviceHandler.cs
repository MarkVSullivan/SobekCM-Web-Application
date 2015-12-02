#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Core.Configuration.Engine;

#endregion

namespace SobekCM.Engine_Library
{
    /// <summary> Handler is used to handle any incoming requests for a microservice exposed by the engine </summary>
    public class MicroserviceHandler : IHttpHandler
    {
        private Engine_Server_Configuration microserviceConfig;
        
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
                    string default_path = Context.Server.MapPath("config/default/sobekcm_engine.config");
                    string user_path = Context.Server.MapPath("config/user/sobekcm_engine.config");
                    if (File.Exists(user_path))
                    {
                        string[] config_paths = {default_path, user_path};
                        microserviceConfig = Engine_Server_Config_Reader.Read_Config(config_paths);
                        
                    }
                    else
                    {
                        microserviceConfig = Engine_Server_Config_Reader.Read_Config(default_path);
                    }
                    
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
                Engine_Endpoint endpoint = microserviceConfig.Get_Endpoint(paths);
                if (endpoint == null)
                {
                    Context.Response.ContentType = "text/plain";
                    Context.Response.StatusCode = 501;
                    Context.Response.Write("No endpoint found");
                    
                }
                else
                {
                    string method =  Context.Request.HttpMethod.ToUpper();
                    if (!endpoint.VerbMappingExists(method))
                    {
                        Context.Response.ContentType = "text/plain";
                        Context.Response.StatusCode = 406;
                        Context.Response.Write("HTTP method " + method + " is not supported by this URL");
                        return;
                    }

                    // Get the specific verb mapping
                    Engine_VerbMapping verbMapping = endpoint[method];

                    // Ensure this is allowed in the range
                    string requestIp = Context.Request.UserHostAddress;
                    if (!verbMapping.AccessPermitted(requestIp))
                    {
                        Context.Response.ContentType = "text/plain";
                        Context.Response.StatusCode = 403;
                        Context.Response.Write("You are forbidden from accessing this endpoint");
                        return;
                    }

                    // Set the protocoal
                    switch (verbMapping.Protocol)
                    {
                        case Microservice_Endpoint_Protocol_Enum.JSON:
                            Context.Response.ContentType = "application/json";
                            break;

                        case Microservice_Endpoint_Protocol_Enum.JSON_P:
                            Context.Response.ContentType = "application/javascript";
                            break;

                        case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                            Context.Response.ContentType = "application/octet-stream";
                            break;

                        case Microservice_Endpoint_Protocol_Enum.XML:
                            Context.Response.ContentType = "text/xml";
                            break;

                        case Microservice_Endpoint_Protocol_Enum.SOAP:
                            Context.Response.ContentType = "text/xml";
                            break;

                        case Microservice_Endpoint_Protocol_Enum.BINARY:
                            Context.Response.ContentType = "application/octet-stream";
                            break;
                    }

                    // Determine if this is currently in a valid DEBUG mode
                    bool debug = (Context.Request.QueryString["debug"] == "debug");

                    try
                    {
                        verbMapping.Invoke(Context.Response, paths, Context.Request.QueryString, Context.Request.Form, debug);
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
