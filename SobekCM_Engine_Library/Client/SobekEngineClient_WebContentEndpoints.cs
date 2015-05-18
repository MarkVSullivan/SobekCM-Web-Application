using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.MicroservicesClient;

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to all the web content-related endpoints exposed by the SobekCM engine </summary>
    public class SobekEngineClient_WebContentEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_WebContentEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_WebContentEndpoints(MicroservicesClient_Configuration ConfigObj)
            : base(ConfigObj)
        {
            // All work done in the base constructor
        }
    }
}
