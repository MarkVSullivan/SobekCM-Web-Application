#region Using directives

using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Navigation;

#endregion

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to all the navigation-related endpoints exposed by the SobekCM engine </summary>
    public class SobekEngineClient_NavigationEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_NavigationEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_NavigationEndpoints(MicroservicesClient_Configuration ConfigObj) : base(ConfigObj)
        {
            // All work done in the base constructor
        }

        /// <summary> Resolve the URL to a navigation object, which contains the information on how
        /// to process the incoming request </summary>
        /// <param name="AboluteUri"></param>
        /// <param name="UserLanguages"></param>
        /// <param name="BrowserType"></param>
        /// <returns></returns>
        public Navigation_Object URL_Resolver(string AboluteUri, string[] UserLanguages, string BrowserType)
        {
            return new Navigation_Object();
        }
    }
}
