#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Endpoints;
using SobekCM.Tools;

#endregion

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

        /// <summary> Get the information for a single top-level web content page </summary>
        /// <param name="InfoBrowseMode"> Path for the requested web content page ( i.e., software/download/.. ) </param>
        /// <param name="Tracer">The tracer.</param>
        /// <returns> Object with all the information and source text for the top-level web content page </returns>
        public HTML_Based_Content Get_HTML_Based_Content( string InfoBrowseMode, Custom_Tracer Tracer )
        {
            // Get the array of portions of the URL to pass into the web content services helper method for now
            string[] splitter = InfoBrowseMode.Split("\\/".ToCharArray());
            List<string> urlSegments = splitter.ToList();

            // Call the web content services endpoint
            WebContentServices.WebContentEndpointErrorEnum error;
            HTML_Based_Content returnValue = WebContentServices.get_html_content(urlSegments, Tracer, out error);

            // Was this null?
            if (returnValue == null)
                return null;

            return returnValue;

        }

        #region Endpoints related to global recent updates


        public WebContent_Recent_Changes Get_Global_Recent_Updates(int Page, int RowsPerPage, string User)
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Recent_Updates");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Recent_Changes>(url, endpoint.Protocol);
        }

        // Get_Global_Recent_Updates_JDataTable

        public List<string> Get_Global_Recent_Updates_NextLevel(string Level1, string Level2, string Level3, string Level4, string Level5, string Level6, string Level7, string Level8)
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Recent_Updates_NextLevel");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol);
        }

        public List<string> Get_Global_Recent_Updates_Users()
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Recent_Updates_Users");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol);
        }


        #endregion

        #region Endpoint related to the usage statistics reports of all web content pages


        public WebContent_Usage_Report Get_Global_Usage_Report(int Page, int RowsPerPage, int Year1, int Month1, int Year2, int Month2)
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Usage_Report");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Usage_Report>(url, endpoint.Protocol);
        }

        // Get_Global_Usage_Report_JDataTable

        public List<string> Get_Global_Usage_Report_NextLevel( int Year1, int Month1, int Year2, int Month2 )
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Usage_Report_NextLevel");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol);
        }


        #endregion

        #region Endpoints related to the complete list of global redirects


        public WebContent_Basic_Pages Get_All_Redirects( int Page, int rowsPerPage )
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_Redirects");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Basic_Pages>(url, endpoint.Protocol);
        }

        // Get_All_Redirects_JDataTable
 

        public List<string> Get_All_Redirects_NextLevel(string Level1, string Level2, string Level3, string Level4, string Level5, string Level6, string Level7, string Level8)
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_Redirects_NextLevel");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol);
        }



        #endregion

        #region Endpoints related to the complete list of web content pages (excluding redirects)


        public WebContent_Basic_Pages Get_All_Pages( int Page, int rowsPerPage )
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_Pages");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Basic_Pages>(url, endpoint.Protocol);
        }


        //Get_All_Pages_JDataTable


        public List<string> Get_All_Pages_NextLevel(string Level1, string Level2, string Level3, string Level4, string Level5, string Level6, string Level7, string Level8 )
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_Pages_NextLevel");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol);
        }


        #endregion

        #region Endpoints related to the complete list of web content entities, including pages and redirects


        public WebContent_Basic_Pages Get_All(int Page, int rowsPerPage)
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Basic_Pages>(url, endpoint.Protocol);
        }


        //Get_All_JDataTable


        public List<string> Get_All_NextLevel(string Level1, string Level2, string Level3, string Level4, string Level5, string Level6, string Level7, string Level8)
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_NextLevel");

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol);
        }

        #endregion
    }
}
