using System;
using SobekCM.Core.Builder;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Settings;
using SobekCM.Tools;

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to all the builder-related endpoints exposed by the SobekCM engine </summary>
    public class SobekEngineClient_BuilderEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_BuilderEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_BuilderEndpoints(MicroservicesClient_Configuration ConfigObj)
            : base(ConfigObj)
        {
            // All work done in the base constructor
        }

        /// <summary> Get the builder-specific settings, including incoming folders and builder module sets </summary>
        /// <param name="IncludeDescriptions"> Flag indicates if the builder module descriptions should be included in the response </param>
        /// <param name="Tracer"></param>
        /// <returns> Fully built builder-specific settings, including incoming folders and builder module sets </returns>
        public Builder_Settings Get_Builder_Settings(bool IncludeDescriptions, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_BuilderServices.Get_Builder_Settings");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Builder.Get_Builder_Settings", Tracer);

            // If the endpoint was null, show an error
            if (endpoint == null)
                Tracer.Add_Trace("SobekEngineClient_BuilderServices.Get_Builder_Settings", "Unable to find the endpoint configuration for Builder.Get_Builder_Settings");


            // Format the URL
            string url = String.Format(endpoint.URL, IncludeDescriptions);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Builder_Settings>(url, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the latest update on the builder status, including the relevant builder
        /// setting values and updates on the scheduled tasks </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built builder-specific status, including latest settings and builder module sets </returns>
        public Builder_Status Get_Builder_Status(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_BuilderServices.Get_Builder_Status");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Builder.Get_Builder_Status", Tracer);

            // If the endpoint was null, show an error
            if (endpoint == null)
            {
                Tracer.Add_Trace("SobekEngineClient_BuilderServices.Get_Builder_Status", "Unable to find the endpoint configuration for Builder.Get_Builder_Status");
                return null;
            }

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Builder_Status>(endpoint.URL, endpoint.Protocol, Tracer);
        }

        /// <summary> Gets the information about a single builder incoming folder </summary>
        /// <param name="FolderID"> Primary key for the incoming builder folder </param>
        /// <param name="Tracer"></param>
        /// <returns> Fully built incoming folder for the builder</returns>
        public Builder_Source_Folder Get_Builder_Folder(int FolderID, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_BuilderServices.Get_Builder_Folder");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Builder.Get_Builder_Folder", Tracer);

            // If the endpoint was null, show an error
            if (endpoint == null)
            {
                Tracer.Add_Trace("SobekEngineClient_BuilderServices.Get_Builder_Folder", "Unable to find the endpoint configuration for Builder.Get_Builder_Folder");
                return null;
            }

            // Format the URL
            string url = String.Format(endpoint.URL, FolderID);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Builder_Source_Folder>(url, endpoint.Protocol, Tracer);
        }

        /// <summary> Get the URL for the list of all builder log files for consumption by the jQuery DataTable.net plug-in </summary>
        /// <remarks> This URL is not an endpoint used by the user interface library, but rather employed by the 
        /// user's browser in concert with the jQuery DataTable.net plug-in.  </remarks>
        public string Get_Builder_Logs_JDataTable_URL
        {
            get
            {
                return Config["Builder.Get_Builder_Logs_JDataTable"] == null ? null : Config["Builder.Get_Builder_Logs_JDataTable"].URL;
            }
        }
    }
}
