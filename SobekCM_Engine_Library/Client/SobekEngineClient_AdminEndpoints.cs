using System;
using System.Collections.Generic;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Authentication;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.Configuration.Extensions;
using SobekCM.Core.Configuration.OAIPMH;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Settings;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Core.UI_Configuration.TemplateElements;
using SobekCM.Core.UI_Configuration.Viewers;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Tools;

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to administrative-related endpoints exposed by the SobekCM engine 
    /// not already supported by another service/endpoint </summary>
    public class SobekEngineClient_AdminEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_AdminEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_AdminEndpoints(MicroservicesClient_Configuration ConfigObj) : base(ConfigObj)
        {
            // All work done in the base constructor
        }

        /// <summary> Gets the administrative setting values, which includes display information
        /// along with the current value and key </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built builder-specific settings, including incoming folders and builder module sets </returns>
        public Admin_Setting_Collection Get_Admin_Settings( Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Admin_Settings");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Admin.Get_Admin_Settings", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Admin_Setting_Collection>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        #region Configuration related endpoints

        /// <summary> Gets the complete configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built configuration object </returns>
        public InstanceWide_Configuration Get_Complete_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Complete_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Complete", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<InstanceWide_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the user-interface configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built user interface configuration object </returns>
        public InstanceWide_UI_Configuration Get_User_Interface_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_User_Interface_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.User_Interface_Configs", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<InstanceWide_UI_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the authentication configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built authentication configuration object </returns>
        public Authentication_Configuration Get_Authentication_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Authentication_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Authentication", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Authentication_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the brief-item mapping configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built brief-item mapping configuration object </returns>
        public BriefItemMapping_Configuration Get_Brief_Item_Mapping_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Brief_Item_Mapping_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Brief_Item_Mapping", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<BriefItemMapping_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the contact form configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built contact form configuration object </returns>
        public ContactForm_Configuration Get_Contact_Form_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Contact_Form_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Contact_Form", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<ContactForm_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the engine configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built engine configuration object </returns>
        public Engine_Server_Configuration Get_Engine_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Engine_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Engine", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Engine_Server_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the extensions configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built extensions configuration object </returns>
        public Extension_Configuration Get_Extensions_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Extensions_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Extensions", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Extension_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the metadata configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built metadata configuration object </returns>
        public Metadata_Configuration Get_Metadata_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Metadata_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Metadata", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Metadata_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the oai-pmh configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built oai-pmh configuration object </returns>
        public OAI_PMH_Configuration Get_OAI_PMH_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_OAI_PMH_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.OAI_PMH", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<OAI_PMH_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the qc tool configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built qc tool configuration object </returns>
        public QualityControl_Configuration Get_Quality_Control_Tool_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Quality_Control_Tool_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Quality_Control_Tool", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<QualityControl_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the configuration reading log which lists which config files were read, etc.. </summary>
        /// <param name="Tracer"></param>
        /// <returns> Reading log object </returns>
        public List<string> Get_Configuration_Reading_Log(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_Configuration_Reading_Log");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.Reading_Log", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the citation (UI) configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built citation (UI) configuration object </returns>
        public CitationConfig Get_UI_Citation_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_UI_Citation_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.UI.Citation", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<CitationConfig>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the map editor (UI) configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built map editor (UI) configuration object </returns>
        public MapEditor_Configuration Get_UI_Map_Editor_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_UI_Map_Editor_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.UI.Map_Editor", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<MapEditor_Configuration>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the template element mapping (UI) configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built template element mapping (UI) configuration object </returns>
        public TemplateElementsConfig Get_UI_Template_Elements_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_UI_Template_Elements_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.UI.Template_Elements", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<TemplateElementsConfig>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the viewers (UI) configuration information, read from the configuration files </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built viewers (UI) configuration object </returns>
        public WriterViewerConfig Get_UI_Viewers_Configuration(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_AdminServices.Get_UI_Viewers_Configuration");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Configuration.UI.Viewers", Tracer);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WriterViewerConfig>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        #endregion
    }
}
