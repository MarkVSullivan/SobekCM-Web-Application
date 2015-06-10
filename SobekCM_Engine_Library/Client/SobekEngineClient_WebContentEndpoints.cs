using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Endpoints;

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to all the web content-related endpoints exposed by the SobekCM engine </summary>
    public class SobekEngineClient_WebContentEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_WebContentEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_WebContentEndpoints(MicroservicesClient_Configuration ConfigObj) : base(ConfigObj)
        {
            // All work done in the base constructor
        }


        public HTML_Based_Content Get_HTML_Based_Content( string InfoBrowseMode )
        {
            // Get the array of portions of the URL to pass into the web content services helper method for now
            string[] splitter = InfoBrowseMode.Split("\\/".ToCharArray());
            List<string> urlSegments = splitter.ToList();

            // Call the web content services endpoint
            WebContentServices.WebContentEndpointErrorEnum error;
            HTML_Based_Content returnValue = WebContentServices.get_html_content(urlSegments, out error);

            // Was this null?
            if (returnValue == null)
                return null;

            return returnValue;

        }

    }
}
