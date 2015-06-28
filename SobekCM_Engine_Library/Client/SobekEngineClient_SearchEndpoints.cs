#region Using directives

using SobekCM.Core.MicroservicesClient;

#endregion

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to all the search-related endpoints exposed by the SobekCM engine </summary>
    public class SobekEngineClient_SearchEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_ResultsEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_SearchEndpoints(MicroservicesClient_Configuration ConfigObj) : base(ConfigObj)
        {
            // All work done in the base constructor
        }
    }
}
