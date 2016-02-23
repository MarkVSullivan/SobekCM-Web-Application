using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Tools;

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Class supports all the administrative-related services provided by the SobekCM engine 
    /// and not already provided via a more specialized service-related endpoint </summary>
    public class AdministrativeServices : EndpointBase
    {

        #region Setting-related endpoints

        /// <summary> Gets the administrative setting values, which includes display information
        /// along with the current value and key </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetAdminSettings(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            try
            {
                tracer.Add_Trace("AdministrativeServices.GetAdminSettings", "Pulling dataset from the database");

                // Get the complete aggregation
                DataSet adminSet = Engine_Database.Get_Settings_Complete(true, tracer);

                // If the returned value from the database was NULL, there was an error
                if ((adminSet == null) || (adminSet.Tables.Count == 0) || ( adminSet.Tables[0].Rows.Count == 0))
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

                tracer.Add_Trace("AdministrativeServices.GetAdminSettings", "Build the list of return objects");
                Admin_Setting_Collection returnValue = new Admin_Setting_Collection();

                try
                {
                    DataColumn keyColumn = adminSet.Tables[0].Columns["Setting_Key"];
                    DataColumn valueColumn = adminSet.Tables[0].Columns["Setting_Value"];
                    DataColumn tabPageColumn = adminSet.Tables[0].Columns["TabPage"];
                    DataColumn headingColumn = adminSet.Tables[0].Columns["Heading"];
                    DataColumn hiddenColumn = adminSet.Tables[0].Columns["Hidden"];
                    DataColumn reservedColumn = adminSet.Tables[0].Columns["Reserved"];
                    DataColumn helpColumn = adminSet.Tables[0].Columns["Help"];
                    DataColumn optionsColumn = adminSet.Tables[0].Columns["Options"];
                    DataColumn idColumn = adminSet.Tables[0].Columns["SettingID"];
                    DataColumn dimensionsColumn = adminSet.Tables[0].Columns["Dimensions"];
                    
                    //Setting_Key, Setting_Value, TabPage, Heading, Hidden, Reserved, Help, Options

                    // Build the return values
                    foreach (DataRow thisRow in adminSet.Tables[0].Rows)
                    {
                        // Build the value object
                        Admin_Setting_Value thisValue = new Admin_Setting_Value
                        {
                            Key = thisRow[keyColumn].ToString(), 
                            Value = thisRow[valueColumn] == DBNull.Value ? null : thisRow[valueColumn].ToString(), 
                            TabPage = thisRow[tabPageColumn] == DBNull.Value ? null : thisRow[tabPageColumn].ToString(), 
                            Heading = thisRow[headingColumn] == DBNull.Value ? null : thisRow[headingColumn].ToString(), 
                            Hidden = bool.Parse(thisRow[hiddenColumn].ToString()), 
                            Reserved = short.Parse(thisRow[reservedColumn].ToString()), 
                            Help = thisRow[helpColumn] == DBNull.Value ? null : thisRow[helpColumn].ToString(),
                            SettingID = short.Parse(thisRow[idColumn].ToString())
                        };

                        // Get dimensions, if some were provided
                        if (thisRow[dimensionsColumn] != DBNull.Value)
                        {
                            string dimensions = thisRow[dimensionsColumn].ToString();
                            if (!String.IsNullOrWhiteSpace(dimensions))
                            {
                                short testWidth;
                                short testHeight;

                                // Does this include width AND height?
                                if (dimensions.IndexOf("|") >= 0)
                                {
                                    string[] splitter = dimensions.Split("|".ToCharArray());
                                    if ((splitter[0].Length > 0) && ( short.TryParse(splitter[0], out testWidth )))
                                    {
                                        thisValue.Width = testWidth;
                                        if ((splitter[1].Length > 0) && (short.TryParse(splitter[1], out testHeight)))
                                        {
                                            thisValue.Height = testHeight;

                                        }
                                    }
                                }
                                else
                                {
                                    if (short.TryParse(dimensions, out testWidth))
                                    {
                                        thisValue.Width = testWidth;
                                    } 
                                }
                            }
                        }

                        // Get the options
                        if (thisRow[optionsColumn] != DBNull.Value)
                        {
                            string[] options = thisRow[optionsColumn].ToString().Split("|".ToCharArray());
                            foreach( string thisOption in options )
                                thisValue.Add_Option(thisOption.Trim());
                        }

                        // Add to the return value
                        returnValue.Settings.Add(thisValue);
                    }

                }
                catch (Exception ex)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");

                    if (IsDebug)
                    {
                        Response.Output.WriteLine("Error creating the Builder_Settings object from the database tables");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ex.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ex.StackTrace);
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
                string json_callback = "parseAdminSettings";
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


        /// <summary> Gets the administrative setting values, which includes display information
        /// along with the current value and key </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetSettings(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseSettings";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Settings, Response, Protocol, json_callback);
        }

        #endregion

        #region Configuration-related endpoints

        /// <summary> Gets the complete configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfiguration(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseConfiguration";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration, Response, Protocol, json_callback);
        }

        /// <summary> Gets the authentication configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationAuthentication(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseAuthenticationConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.Authentication, Response, Protocol, json_callback);
        }

        /// <summary> Gets the brief item mapping configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationBriefItemMapping(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseBriefItemMappingConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.BriefItemMapping, Response, Protocol, json_callback);
        }

        /// <summary> Gets the contact form configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationContactForm(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseContactFormConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.ContactForm, Response, Protocol, json_callback);
        }

        /// <summary> Gets the engine endpoints configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationEngine(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseEngineConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.Engine, Response, Protocol, json_callback);
        }

        /// <summary> Gets the extensions configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationExtensions(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseExtensions";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.Extensions, Response, Protocol, json_callback);
        }

        /// <summary> Gets the map editor (UI) configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationMapEditor(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseMapEditorConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.UI.MapEditor, Response, Protocol, json_callback);
        }

        /// <summary> Gets the metadata configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationMetadata(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseMetadataConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.Metadata, Response, Protocol, json_callback);
        }

        /// <summary> Gets the OAI-PMH configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationOaiPmh(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseOaiPmhConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.OAI_PMH, Response, Protocol, json_callback);
        }

        /// <summary> Gets the quality control tool configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationQualityControl(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseQcToolConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.QualityControlTool, Response, Protocol, json_callback);
        }

        /// <summary> Gets the citation dispay (UI) configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationUserInterface(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseUiConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.UI, Response, Protocol, json_callback);
        }

        /// <summary> Gets the citation dispay (UI) configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationCitation(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseCitationConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.UI.CitationViewer, Response, Protocol, json_callback);
        }

        /// <summary> Gets the template elements (UI) configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationTemplateElements(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseTemplateElementsConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.UI.TemplateElements, Response, Protocol, json_callback);
        }

        /// <summary> Gets the viewers (ui) configuration object </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationViewers(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Get the JSON-P callback function
            string json_callback = "parseViewerConfig";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.UI.WriterViewers, Response, Protocol, json_callback);
        }

        /// <summary> Gets the log from reading all of the configuration files </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetConfigurationLog(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            if (Protocol == Microservice_Endpoint_Protocol_Enum.TEXT)
            {
                Response.ContentType = "text/plain";
                foreach (string thisLine in Engine_ApplicationCache_Gateway.Configuration.ReadingLog)
                    Response.Output.WriteLine(thisLine);
                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseConfigLog";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(Engine_ApplicationCache_Gateway.Configuration.ReadingLog, Response, Protocol, json_callback);
        }

        #endregion
    }
}
