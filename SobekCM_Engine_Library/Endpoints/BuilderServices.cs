using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.Database;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Engine_Library.Settings;
using SobekCM.Tools;

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Class supports all the builder-related services provided by the SobekCM engine </summary>
    public class BuilderServices : EndpointBase
    {
        /// <summary> Gets the builder-specific settings, including all the builder modules and incoming folders </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetBuilderSettings(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Should descriptions be suppressed?
            bool includeDescriptions = !((!String.IsNullOrEmpty(QueryString["IncludeDescs"])) && (QueryString["IncludeDescs"].ToUpper() == "FALSE"));
            try
            {
                tracer.Add_Trace("BuilderServices.GetBuilderSettings", "Pulling dataset from the database");

                // Get the complete aggregation
                DataSet builderSet = Engine_Database.Get_Builder_Settings(true, tracer);

                // If the returned value from the database was NULL, there was an error
                if ((builderSet == null) || ( builderSet.Tables.Count == 0 ))
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");

                    if (IsDebug)
                    {
                        Response.Output.WriteLine("DataSet returned from the database was either NULL or empty");
                        if (Engine_Database.Last_Exception != null)
                        {
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(Engine_Database.Last_Exception.Message);
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(Engine_Database.Last_Exception.StackTrace);
                        }

                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }

                    Response.StatusCode = 500;
                    return;
                }

                tracer.Add_Trace("BuilderServices.GetBuilderSettings", "Build the builder-specific settings object");
                Builder_Settings returnValue = new Builder_Settings();
                if (!Builder_Settings_Builder.Refresh(returnValue, builderSet, includeDescriptions))
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");

                    if (IsDebug)
                    {
                        Response.Output.WriteLine("Error creating the Builder_Settings object from the database tables");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }

                    Response.StatusCode = 500;
                    return;
                }

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parseBuilderSettings";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnValue, Response, Protocol, json_callback);
            }
            catch (Exception ee)
            {
                if (IsDebug)
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
    }
}
