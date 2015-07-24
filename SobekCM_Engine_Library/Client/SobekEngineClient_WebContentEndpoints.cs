#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /// <summary> Get the list of all the recent updates to all (non aggregation affiliated) static web content pages </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Page"> Page number of recent updates ( starting with one and counting up ) </param>
        /// <param name="RowsPerPage"> (Optional) Number of rows of updates to include in each page of results </param>
        /// <param name="UserFilter"> (Optional) Filter to only return items updated by one user </param>
        /// <returns> List of requested recent udpates </returns>
        public WebContent_Recent_Changes Get_Global_Recent_Updates(Custom_Tracer Tracer, int Page, int? RowsPerPage = null, string UserFilter = null )
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_Global_Recent_Updates");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Recent_Updates", Tracer );

            // Format the URL
            string url = endpoint.URL + "?page=" + Page;
            if ((RowsPerPage.HasValue) && ( RowsPerPage.Value > 0 ))
                url = url + "&rowsPerPage=" + RowsPerPage.Value;
            if (!String.IsNullOrEmpty(UserFilter))
                url = url + "&user=" + UserFilter;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Recent_Changes>(url, endpoint.Protocol, Tracer);
        }

        /// <summary> Get the URL for the list of all recent updates to (non aggregation affiliated) static web pages 
        /// for consumption by the jQuery DataTable.net plug-in </summary>
        /// <remarks> This URL is not an endpoint used by the user interface library, but rather employed by the 
        /// user's browser in concert with the jQuery DataTable.net plug-in.  </remarks>
        public string Get_Global_Recent_Updates_JDataTable_URL
        {
            get
            {
                return Config["Get_Global_Recent_Updates_JDataTable"] == null ? null : Config["Get_Global_Recent_Updates_JDataTable"].URL;
            }
        }

        /// <summary> Gets the list of possible next level from an existing page in the recent updates, used for filtering </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of the values present in the next level of the recent updates list, used for filtering </returns>
        public List<string> Get_Global_Recent_Updates_NextLevel(Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_Global_Recent_Updates_NextLevel");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Recent_Updates_NextLevel", Tracer);

            // Format the URL
            string url = endpoint.URL;
            if (!String.IsNullOrEmpty(Level1))
            {
                if (String.IsNullOrEmpty(Level2))
                    url = endpoint.URL + "/" + Level1;
                else if ((!String.IsNullOrEmpty(Level2)) && (String.IsNullOrEmpty(Level3)))
                    url = endpoint.URL + "/" + Level1 + "/" + Level2;
                else
                {
                    StringBuilder urlBuilder = new StringBuilder(endpoint.URL + "/" + Level1 + "/" + Level2 + "/" + Level3);
                    if (!String.IsNullOrEmpty(Level4))
                    {
                        urlBuilder.Append("/" + Level4);
                        if (!String.IsNullOrEmpty(Level5))
                        {
                            urlBuilder.Append("/" + Level5);
                            if (!String.IsNullOrEmpty(Level6))
                            {
                                urlBuilder.Append("/" + Level6);
                                if (!String.IsNullOrEmpty(Level7))
                                {
                                    urlBuilder.Append("/" + Level7);
                                    if (!String.IsNullOrEmpty(Level8))
                                    {
                                        urlBuilder.Append("/" + Level8);
                                    }
                                }
                            }
                        }
                    }

                    url = urlBuilder.ToString();
                }
            }

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol, Tracer);
        }

        /// <summary> Get the list of all users that have participated in the recent updates to all top-level static web content pages </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> List of users that made recent updates </returns>
        public List<string> Get_Global_Recent_Updates_Users(Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_Global_Recent_Updates_Users");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Recent_Updates_Users", Tracer);

            // Format the URL
            string url = endpoint.URL;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol, Tracer);
        }


        #endregion

        #region Endpoint related to the usage statistics reports of all web content pages

        /// <summary> Get usage of all web content pages between two dates </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Year1"> Start year of the year/month range for these usage stats </param>
        /// <param name="Month1"> Start month of the year/month range for these usage stats </param>
        /// <param name="Year2"> End year of the year/month range for these usage stats </param>
        /// <param name="Month2"> End month of the year/month range for these usage stats </param>
        /// <param name="Page"> Page number of used pages ( starting with one and counting up ) </param>
        /// <param name="RowsPerPage"> (Optional) Number of rows of used pages to include in each page of results </param>
        /// <returns> Web content usage report </returns>
        public WebContent_Usage_Report Get_Global_Usage_Report(Custom_Tracer Tracer, int Year1, int Month1, int Year2, int Month2, int Page, int? RowsPerPage = null )
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_Global_Usage_Report");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Usage_Report", Tracer);

            // Format the URL
            string url = endpoint.URL + "?year1=" + Year1 + "&month1=" + Month1 + "&year2=" + Year2 + "&month2=" + Month2 + "&page=" + Page;
            if ((RowsPerPage.HasValue) && (RowsPerPage.Value > 0))
            {
                url = url + "&rowsPerPage=" + RowsPerPage.Value;
            }

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Usage_Report>(url, endpoint.Protocol, Tracer);
        }

        /// <summary> Get the URL for the list of usage for a global usage report for consumption by the jQuery DataTable.net plug-in </summary>
        /// <remarks> This URL is not an endpoint used by the user interface library, but rather employed by the 
        /// user's browser in concert with the jQuery DataTable.net plug-in.  </remarks>
        public string Get_Global_Usage_Report_JDataTable_URL
        {
            get
            {
                return Config["Get_Global_Usage_Report_JDataTable"] == null ? null : Config["Get_Global_Usage_Report_JDataTable"].URL;
            }
        }

        /// <summary> Gets the list of possible next level from an existing used page in a global usage report, used for filtering  </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Year1"> Start year of the year/month range for these usage stats </param>
        /// <param name="Month1"> Start month of the year/month range for these usage stats </param>
        /// <param name="Year2"> End year of the year/month range for these usage stats </param>
        /// <param name="Month2"> End month of the year/month range for these usage stats </param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of the values present in the next level of the requested usage report list, used for filtering </returns>
        public List<string> Get_Global_Usage_Report_NextLevel(Custom_Tracer Tracer, int Year1, int Month1, int Year2, int Month2, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_Global_Usage_Report_NextLevel");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_Global_Usage_Report_NextLevel", Tracer);

            // Format the URL
            string url = endpoint.URL;
            if (!String.IsNullOrEmpty(Level1))
            {
                if (String.IsNullOrEmpty(Level2))
                    url = endpoint.URL + "/" + Level1;
                else if ((!String.IsNullOrEmpty(Level2)) && (String.IsNullOrEmpty(Level3)))
                    url = endpoint.URL + "/" + Level1 + "/" + Level2;
                else
                {
                    StringBuilder urlBuilder = new StringBuilder(endpoint.URL + "/" + Level1 + "/" + Level2 + "/" + Level3);
                    if (!String.IsNullOrEmpty(Level4))
                    {
                        urlBuilder.Append("/" + Level4);
                        if (!String.IsNullOrEmpty(Level5))
                        {
                            urlBuilder.Append("/" + Level5);
                            if (!String.IsNullOrEmpty(Level6))
                            {
                                urlBuilder.Append("/" + Level6);
                                if (!String.IsNullOrEmpty(Level7))
                                {
                                    urlBuilder.Append("/" + Level7);
                                    if (!String.IsNullOrEmpty(Level8))
                                    {
                                        urlBuilder.Append("/" + Level8);
                                    }
                                }
                            }
                        }
                    }

                    url = urlBuilder.ToString();
                }
            }
            url = url + "?year1=" + Year1 + "&month1=" + Month1 + "&year2=" + Year2 + "&month2=" + Month2;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol, Tracer);
        }


        #endregion

        #region Endpoints related to the complete list of global redirects

        /// <summary> Get the list of all the global redirects </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Page"> Page number of used pages ( starting with one and counting up ) </param>
        /// <param name="RowsPerPage"> (Optional) Number of rows of used pages to include in each page of results </param>
        /// <returns> Reqeusted list of web content redirect entities </returns>
        public WebContent_Basic_Pages Get_All_Redirects(Custom_Tracer Tracer, int Page, int? RowsPerPage = null)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_All_Redirects");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_Redirects", Tracer);

            // Format the URL
            string url = endpoint.URL + "?page=" + Page;
            if ((RowsPerPage.HasValue) && (RowsPerPage.Value > 0))
                url = url + "&rowsPerPage=" + RowsPerPage.Value;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Basic_Pages>(url, endpoint.Protocol, Tracer);
        }

        /// <summary> Get the URL for the list of all the global redirects for consumption by the jQuery DataTable.net plug-in </summary>
        /// <remarks> This URL is not an endpoint used by the user interface library, but rather employed by the 
        /// user's browser in concert with the jQuery DataTable.net plug-in.  </remarks>
        public string Get_All_Redirects_JDataTable_URL
        {
            get
            {
                return Config["Get_All_Redirects_JDataTable"] == null ? null : Config["Get_All_Redirects_JDataTable"].URL;
            }
        }

        /// <summary> Gets the list of possible next level from an existing point in the redirects hierarchy, used for filtering </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of the values present in the next level of the redirects list, used for filtering </returns>
        public List<string> Get_All_Redirects_NextLevel(Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_All_Redirects_NextLevel");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_Redirects_NextLevel", Tracer);

            // Format the URL
            string url = endpoint.URL;
            if (!String.IsNullOrEmpty(Level1))
            {
                if (String.IsNullOrEmpty(Level2))
                    url = endpoint.URL + "/" + Level1;
                else if ((!String.IsNullOrEmpty(Level2)) && (String.IsNullOrEmpty(Level3)))
                    url = endpoint.URL + "/" + Level1 + "/" + Level2;
                else
                {
                    StringBuilder urlBuilder = new StringBuilder(endpoint.URL + "/" + Level1 + "/" + Level2 + "/" + Level3);
                    if (!String.IsNullOrEmpty(Level4))
                    {
                        urlBuilder.Append("/" + Level4);
                        if (!String.IsNullOrEmpty(Level5))
                        {
                            urlBuilder.Append("/" + Level5);
                            if (!String.IsNullOrEmpty(Level6))
                            {
                                urlBuilder.Append("/" + Level6);
                                if (!String.IsNullOrEmpty(Level7))
                                {
                                    urlBuilder.Append("/" + Level7);
                                    if (!String.IsNullOrEmpty(Level8))
                                    {
                                        urlBuilder.Append("/" + Level8);
                                    }
                                }
                            }
                        }
                    }

                    url = urlBuilder.ToString();
                }
            }

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol, Tracer);
        }


        #endregion

        #region Endpoints related to the complete list of web content pages (excluding redirects)

        /// <summary> Get the list of all the web content pages ( excluding redirects ) </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Page"> Page number of used pages ( starting with one and counting up ) </param>
        /// <param name="RowsPerPage"> (Optional) Number of rows of used pages to include in each page of results </param>
        /// <returns> Requested list of web content pages ( excluding redirects ) </returns>
        public WebContent_Basic_Pages Get_All_Pages(Custom_Tracer Tracer, int Page, int? RowsPerPage = null)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_All_Pages");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_Pages", Tracer);

            // Format the URL
            string url = endpoint.URL + "?page=" + Page;
            if ((RowsPerPage.HasValue) && (RowsPerPage.Value > 0))
                url = url + "&rowsPerPage=" + RowsPerPage.Value;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Basic_Pages>(url, endpoint.Protocol, Tracer);
        }


        /// <summary> Get the URL for the list of all the web content pages ( excluding redirects ) for
        /// consumption by the jQuery DataTable.net plug-in </summary>
        /// <remarks> This URL is not an endpoint used by the user interface library, but rather employed by the 
        /// user's browser in concert with the jQuery DataTable.net plug-in.  </remarks>
        public string Get_All_Pages_JDataTable_URL
        {
            get
            {
                return Config["Get_All_Pages_JDataTable"] == null ? null : Config["Get_All_Pages_JDataTable"].URL;
            }
        }

        /// <summary> Gets the list of possible next level from an existing point in the page hierarchy, used for filtering </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of the values present in the next level of the pages list, used for filtering </returns>
        public List<string> Get_All_Pages_NextLevel(Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_All_Pages_NextLevel");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_Pages_NextLevel", Tracer);

            // Format the URL
            string url = endpoint.URL;
            if (!String.IsNullOrEmpty(Level1))
            {
                if (String.IsNullOrEmpty(Level2))
                    url = endpoint.URL + "/" + Level1;
                else if ((!String.IsNullOrEmpty(Level2)) && (String.IsNullOrEmpty(Level3)))
                    url = endpoint.URL + "/" + Level1 + "/" + Level2;
                else
                {
                    StringBuilder urlBuilder = new StringBuilder(endpoint.URL + "/" + Level1 + "/" + Level2 + "/" + Level3);
                    if (!String.IsNullOrEmpty(Level4))
                    {
                        urlBuilder.Append("/" + Level4);
                        if (!String.IsNullOrEmpty(Level5))
                        {
                            urlBuilder.Append("/" + Level5);
                            if (!String.IsNullOrEmpty(Level6))
                            {
                                urlBuilder.Append("/" + Level6);
                                if (!String.IsNullOrEmpty(Level7))
                                {
                                    urlBuilder.Append("/" + Level7);
                                    if (!String.IsNullOrEmpty(Level8))
                                    {
                                        urlBuilder.Append("/" + Level8);
                                    }
                                }
                            }
                        }
                    }

                    url = urlBuilder.ToString();
                }
            }

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol, Tracer);
        }


        #endregion

        #region Endpoints related to the complete list of web content entities, including pages and redirects

        /// <summary> Get the list of all the web content entities, including pages and redirects </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Page"> Page number of used pages ( starting with one and counting up ) </param>
        /// <param name="RowsPerPage"> (Optional) Number of rows of used pages to include in each page of results </param>
        /// <returns> Requested list of web content entities, including pages and redirects </returns>
        public WebContent_Basic_Pages Get_All(Custom_Tracer Tracer, int Page, int? RowsPerPage = null)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_All");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All", Tracer);

            // Format the URL
            string url = endpoint.URL + "?page=" + Page;
            if ((RowsPerPage.HasValue) && (RowsPerPage.Value > 0))
                url = url + "&rowsPerPage=" + RowsPerPage.Value;

            // Call out to the endpoint and return the deserialized object
            return Deserialize<WebContent_Basic_Pages>(url, endpoint.Protocol, Tracer);
        }


        /// <summary> Get the URL for the list of all the web content entities, including pages and redirects, for
        /// consumption by the jQuery DataTable.net plug-in </summary>
        /// <remarks> This URL is not an endpoint used by the user interface library, but rather employed by the 
        /// user's browser in concert with the jQuery DataTable.net plug-in.  </remarks>
        public string Get_All_JDataTable_URL
        {
            get
            {
                return Config["Get_All_JDataTable"] == null ? null : Config["Get_All_JDataTable"].URL;
            }
        }

        /// <summary> Gets the list of possible next level from an existing point in the page AND redirects hierarchy, used for filtering </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of the values present in the next level of the pages AND redirects list, used for filtering </returns>
        public List<string> Get_All_NextLevel(Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_All_NextLevel");

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("WebContent.Get_All_NextLevel", Tracer);

            // Format the URL
            string url = endpoint.URL;
            if (!String.IsNullOrEmpty(Level1))
            {
                if (String.IsNullOrEmpty(Level2))
                    url = endpoint.URL + "/" + Level1;
                else if ((!String.IsNullOrEmpty(Level2)) && (String.IsNullOrEmpty(Level3)))
                    url = endpoint.URL + "/" + Level1 + "/" + Level2;
                else
                {
                    StringBuilder urlBuilder = new StringBuilder(endpoint.URL + "/" + Level1 + "/" + Level2 + "/" + Level3);
                    if (!String.IsNullOrEmpty(Level4))
                    {
                        urlBuilder.Append("/" + Level4);
                        if (!String.IsNullOrEmpty(Level5))
                        {
                            urlBuilder.Append("/" + Level5);
                            if (!String.IsNullOrEmpty(Level6))
                            {
                                urlBuilder.Append("/" + Level6);
                                if (!String.IsNullOrEmpty(Level7))
                                {
                                    urlBuilder.Append("/" + Level7);
                                    if (!String.IsNullOrEmpty(Level8))
                                    {
                                        urlBuilder.Append("/" + Level8);
                                    }
                                }
                            }
                        }
                    }

                    url = urlBuilder.ToString();
                }
            }

            // Call out to the endpoint and return the deserialized object
            return Deserialize<List<string>>(url, endpoint.Protocol, Tracer);
        }

        #endregion
    }
}
