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

            //    DmsDatabaseGateway.DbConnectionString = "Server=DMS;Database=USYellow;Integrated Security=true;Connection Timeout=120";

                // Get any matching endpoint configuration
                Microservice_Endpoint endpoint = microserviceConfig.Get_Endpoint(paths);
                if (endpoint == null)
                {
                    context.Response.StatusCode = 501;
                    context.Response.Write("No endpoint found");
                }
                else
                {
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                        context.Response.ContentType = "application/json";
                    if (endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.PROTOBUF)
                        context.Response.ContentType = "application/octet-stream";

                    try
                    {
                        endpoint.Invoke(context.Response, paths, context.Request.Form);
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
                context.Response.StatusCode = 400;
                context.Response.Write("Invalid URI - No endpoint requested");
            }
        }

        public bool IsReusable { get { return true;  } }
    }
}
