#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using SobekCM.Core.Configuration;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Core.Configuration.Engine;

#endregion

namespace SobekCM.Engine_Library
{
    /// <summary> Handler is used to handle any incoming requests for a microservice exposed by the engine </summary>
    public class MicroserviceHandler : IHttpHandler
    {
        private static Dictionary<string, object> restApiObjectsDictionary;
        private static Dictionary<string, MethodInfo> restApiMethodDictionary;

        /// <summary> Static constructor </summary>
        static MicroserviceHandler()
        {
            restApiObjectsDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            restApiMethodDictionary = new Dictionary<string, MethodInfo>();
        }
       
        /// <summary> Processes the request </summary>
        /// <param name="Context">The context for the current request </param>
        public void ProcessRequest(HttpContext Context)
        {
            Engine_Database.Connection_String = Engine_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String;

            // Get the original query string
            string queryString = Context.Request.QueryString["urlrelative"];
            if (!String.IsNullOrEmpty(queryString))
            {
                // Set the encoding
                Context.Response.Charset = Encoding.UTF8.WebName;

                // Set this to allow us to have our own error messages, without IIS jumping into it
                Context.Response.TrySkipIisCustomErrors = true;

                // Make sure the microservices configuration has been read
                if (!Engine_ApplicationCache_Gateway.Configuration.HasData)
                {
                    // If we got to here, it means it attempted to read it and failed somehow
                    Context.Response.ContentType = "text/plain";
                    Context.Response.StatusCode = 500;
                    Context.Response.Write("Error reading the configuration files!");
                    return;
                }

                // Collect the requested paths
                string[] splitter;
                if (( queryString.IndexOf("/") == 0 ) && ( queryString.Length > 1 ))
                    splitter = queryString.Substring(1).Split("/".ToCharArray());
                else
                    splitter = queryString.Split("/".ToCharArray());
                List<string> paths = splitter.ToList();





                // Get any matching endpoint configuration
                Engine_Path_Endpoint endpoint = Engine_ApplicationCache_Gateway.Configuration.Engine.Get_Endpoint(paths);
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

                    // Get the component information
                    if (verbMapping.Component == null)
                    {
                        Context.Response.ContentType = "text/plain";
                        Context.Response.Output.WriteLine("No component listed or found for this valid endpoint");
                        Context.Response.StatusCode = 500;
                        return;
                    }

                    // Look for this component
                    object restApiObject = null;
                    if (restApiObjectsDictionary.ContainsKey(verbMapping.Component.ID))
                    {
                        restApiObject = restApiObjectsDictionary[verbMapping.Component.ID];
                    }
                    else
                    {
                        try
                        {
                            // Create this component object (assumes same assembly for the moment)
                            Assembly dllAssembly = Assembly.GetExecutingAssembly();
                            // Type restApiClassType = dllAssembly.GetType(Component.Namespace + "." + Component.Class);

                            Type restApiClassType = dllAssembly.GetType("SobekCM.Engine_Library.Endpoints." + verbMapping.Component.Class);
                            restApiObject = Activator.CreateInstance(restApiClassType);

                            // Add this to the dictionary of rest api objects as well
                            restApiObjectsDictionary[verbMapping.Component.ID] = restApiObject;
                        }
                        catch (Exception ee)
                        {
                            Context.Response.ContentType = "text/plain";
                            Context.Response.Output.WriteLine("Error creating the endpoint object " + verbMapping.Component.Class);
                            Context.Response.Output.WriteLine(ee.Message);
                            Context.Response.StatusCode = 500;
                            return;
                        }
                    }

                    // One more check that the value really was not null
                    if (verbMapping.Component.Class == null)
                    {
                        Context.Response.ContentType = "text/plain";
                        Context.Response.Output.WriteLine("Error creating the endpoint object " + verbMapping.Component.Class);
                        Context.Response.StatusCode = 500;
                        return;
                    }

                    // Now, check for the method itself

                    try
                    {
                        // Get the method information for the method to execute
                        MethodInfo methodInfo = null;
                        if (restApiMethodDictionary.ContainsKey(verbMapping.Component.ID + "|" + verbMapping.Method))
                        {
                            methodInfo = restApiMethodDictionary[verbMapping.Component.ID + "|" + verbMapping.Method];
                        }
                        else
                        {
                            Type restApiClassType = restApiObject.GetType();
                            methodInfo = restApiClassType.GetMethod(verbMapping.Method);

                            // Add back to the dictionary
                            restApiMethodDictionary[verbMapping.Component.ID + "|" + verbMapping.Method] = methodInfo;
                        }

                        // If the this somehow didn't throw an error, but is null, show the same message
                        if (methodInfo == null)
                        {
                            Context.Response.ContentType = "text/plain";
                            Context.Response.Output.WriteLine("Error invoking the endpoint method: No Method Found");
                            Context.Response.StatusCode = 500;
                            return;
                        }

                        // Invokation is different, dependingon whether this is a PUT or POST
                        if (verbMapping.RequestType == Microservice_Endpoint_RequestType_Enum.GET)
                            methodInfo.Invoke(restApiObject, new object[] { Context.Response, paths, Context.Request.QueryString, verbMapping.Protocol, debug });
                        else
                            methodInfo.Invoke(restApiObject, new object[] { Context.Response, paths, Context.Request.QueryString, Context.Request.Form, debug });
                    }
                    catch (Exception ee)
                    {
                        Context.Response.ContentType = "text/plain";
                        Context.Response.Output.WriteLine("Error invoking the endpoint method: " + ee.Message);
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
