using System;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Settings;
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
    }
}
