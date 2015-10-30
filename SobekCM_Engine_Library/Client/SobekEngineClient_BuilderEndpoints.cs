using System;
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
        public Builder_Settings Get_Builder_Settings( bool IncludeDescriptions, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_BuilderServices.Get_Builder_Settings");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Builder.Get_Builder_Settings", Tracer);

            // Format the URL
            string url = String.Format(endpoint.URL, IncludeDescriptions);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<Builder_Settings>(url, endpoint.Protocol, Tracer);
        }
    }
}
