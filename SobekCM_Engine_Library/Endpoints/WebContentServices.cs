#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Jil;
using ProtoBuf;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Message;
using SobekCM.Core.WebContent;
using SobekCM.Core.WebContent.Admin;
using SobekCM.Core.WebContent.Hierarchy;
using SobekCM.Core.WebContent.Single;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.JSON_Client_Helpers;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Tools;
using SolrNet.DSL;

#endregion

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Endpoint supports services related to the static web content, CMS functionality </summary>
    public class WebContentServices : EndpointBase
    {
        /// <summary> Enumeration of possible web content endpoint errors </summary>
        public enum WebContentEndpointErrorEnum : byte
        {
            /// <summary> No error occurred.  Normal operation. </summary>
            NONE,

            /// <summary> Expected source file was not found </summary>
            No_File_Found,
     
            /// <summary> Unexpected error reading a source file </summary>
            Error_Reading_File
        }


        #region Methods to get the complete web content hierarchy tree

        /// <summary> Get the complete hierarchy of non-aggregational static web content pages and redirects, used for navigation </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Hierarchy(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Hierarchy", "Get the hierarchy object from the application cache gateway");

            // Get the dataset of results
            WebContent_Hierarchy returnValue;
            try
            {
                returnValue = Engine_ApplicationCache_Gateway.WebContent_Hierarchy;
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Exception pulling the web content hierarchy - " + ee.Message );
                Response.StatusCode = 500;

                // If this was debug mode, then write the tracer
                if (IsDebug)
                {
                    tracer.Add_Trace("WebContentServices.Get_Hierarchy", "Exception caught " + ee.Message );
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.StackTrace);
                }
                return;
            }
            

            // If null was returned, an error occurred
            if (returnValue == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull usage report from the application cache / database");
                Response.StatusCode = 500;

                // If this was debug mode, then write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_Single_Usage_Report", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseWebContentHierarchy";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        #endregion

        #region Methods related to a single top-level static HTML content page

        /// <summary> Get top-level web content, static HTML, by primary key from the database or by URL </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_HTML_Based_Content(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content");

            // Must at least have one URL segment for the ID
            if (UrlSegments.Count > 0)
            {
                int webContentId;
                if (Int32.TryParse(UrlSegments[0], out webContentId))
                {
                    Get_HTML_Based_Content_By_ID(Response, UrlSegments, QueryString, Protocol, IsDebug);
                }
                else
                {
                    Get_HTML_Based_Content_By_URL(Response, UrlSegments, QueryString, Protocol, IsDebug);
                }
            }
        }


        /// <summary> Get top-level web content, static HTML, by primary key from the database </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_HTML_Based_Content_By_ID(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content_By_ID", "Get content based on the primary key");

            // Must at least have one URL segment for the ID
            if (UrlSegments.Count > 0)
            {
                int webContentId;
                if (!Int32.TryParse(UrlSegments[0], out webContentId))
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Invalid parameter - expects primary key integer value to the web content object (WebContentID) as part of URL");
                    Response.StatusCode = 500;

                    // If this was debug mode, then write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                // Declare the return object and look this up in the cache by ID
                HTML_Based_Content returnValue = CachedDataManager.WebContent.Retrieve_Page_Details(webContentId, tracer);

                // If nothing was retrieved, build it
                if (returnValue == null)
                {

                    // Try to read and return the html based content 
                    // Get the details from the database
                    WebContent_Basic_Info basicInfo = Engine_Database.WebContent_Get_Page(webContentId, tracer);
                    if (basicInfo == null)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("Unable to pull web content data from the database");
                        Response.StatusCode = 500;
                        return;
                    }

                    // If this has a redirect, return
                    if (!String.IsNullOrEmpty(basicInfo.Redirect))
                    {
                        returnValue = new HTML_Based_Content
                        {
                            Title = basicInfo.Title,
                            Level1 = basicInfo.Level1,
                            Level2 = basicInfo.Level2,
                            Level3 = basicInfo.Level3,
                            Level4 = basicInfo.Level4,
                            Level5 = basicInfo.Level5,
                            Level6 = basicInfo.Level6,
                            Level7 = basicInfo.Level7,
                            Level8 = basicInfo.Level8,
                            Locked = basicInfo.Locked,
                            Description = basicInfo.Summary,
                            Redirect = basicInfo.Redirect,
                            WebContentID = basicInfo.WebContentID
                        };
                    }
                    else
                    {
                        // Build the HTML content
                        WebContentEndpointErrorEnum errorType;
                        returnValue = read_source_file(basicInfo, tracer, out errorType);
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }

                            switch (errorType)
                            {
                                case WebContentEndpointErrorEnum.Error_Reading_File:
                                    Response.ContentType = "text/plain";
                                    Response.Output.WriteLine("Unable to read existing source file");
                                    Response.StatusCode = 500;
                                    return;

                                case WebContentEndpointErrorEnum.No_File_Found:
                                    Response.ContentType = "text/plain";
                                    Response.Output.WriteLine("Source file does not exist");
                                    Response.StatusCode = 404;
                                    return;

                                default:
                                    Response.ContentType = "text/plain";
                                    Response.Output.WriteLine("Error occurred");
                                    Response.StatusCode = 500;
                                    return;
                            }
                        }
                    }

                    // Now, store in the cache
                    CachedDataManager.WebContent.Store_Page_Details(returnValue, tracer);
                }
                else
                {
                    tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content_By_ID", "Object found in the cache");
                }

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content_By_ID", "Debug mode detected");
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parseWebContent";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnValue, Response, Protocol, json_callback);
            }
            else
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Invalid parameter - expects primary key integer value to the web content object (WebContentID) as part of URL");
                Response.StatusCode = 500;
            }
        }

        /// <summary> Get top-level web content, static HTML, by URL </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_HTML_Based_Content_By_URL(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();

            tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content_By_URL","Compute web content id from url segments");

            if (Engine_ApplicationCache_Gateway.WebContent_Hierarchy == null)
            {
                tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content_By_URL", "Unable to pull web content hierarchy from engine application cache");

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull web content hierarchy from engine application cache");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }

                return;
            }

            // Look this up from the web content hierarchy object
            WebContent_Hierarchy_Node matchedNode = Engine_ApplicationCache_Gateway.WebContent_Hierarchy.Find(UrlSegments);
            if (matchedNode == null)
            {
                tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content_By_URL", "Requested page does not exist");

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Requested page does not exist");
                Response.StatusCode = 404;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Declare the return object and look this up in the cache by ID
            HTML_Based_Content returnValue = CachedDataManager.WebContent.Retrieve_Page_Details(matchedNode.WebContentID, tracer);

            // If nothing was retrieved, built it
            if (returnValue == null)
            {
                // If there was a redirect, just return a very lightly built object
                if (!String.IsNullOrEmpty(matchedNode.Redirect))
                {
                    returnValue = new HTML_Based_Content
                    {
                        Redirect = matchedNode.Redirect,
                        WebContentID = matchedNode.WebContentID
                    };
                }
                else
                {
                    // Get the details from the database
                    WebContent_Basic_Info basicInfo = Engine_Database.WebContent_Get_Page(matchedNode.WebContentID, tracer);
                    if (basicInfo == null)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("Unable to pull web content data from the database");
                        Response.StatusCode = 500;
                        return;
                    }

                    // Build the HTML content
                    WebContentEndpointErrorEnum errorType;
                    returnValue = read_source_file(basicInfo, tracer, out errorType);
                    if (returnValue == null)
                    {
                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);
                        }

                        switch (errorType)
                        {
                            case WebContentEndpointErrorEnum.Error_Reading_File:
                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("Unable to read existing source file");
                                Response.StatusCode = 500;
                                return;

                            case WebContentEndpointErrorEnum.No_File_Found:
                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("Source file does not exist");
                                Response.StatusCode = 404;
                                return;

                            default:
                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("Error occurred");
                                Response.StatusCode = 500;
                                return;
                        }
                    }
                }

                // Now, store in the cache
                CachedDataManager.WebContent.Store_Page_Details(returnValue, tracer);
            }
            else
            {
                tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content_By_URL", "Object found in the cache");
            }

            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content_By_URL", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseWebContent";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Delete a non-aggregational top-level web content, static HTML page or redirect </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <param name="RequestForm"></param>
        /// <param name="IsDebug"></param>
        public void Delete_HTML_Based_Content(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, NameValueCollection RequestForm, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Delete_HTML_Based_Content");

            // Validate the web content id exists in the URL
            int webcontentId;
            if ((UrlSegments.Count == 0) || (!Int32.TryParse(UrlSegments[0], out webcontentId)))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Invalid URL.  WebContentID missing from URL"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // Validate the username is present
            if (String.IsNullOrEmpty(RequestForm["User"]))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Required posted object 'User' is missing"), Response, Protocol, null);
                Response.StatusCode = 500;
                return;
            }

            // Get the username
            string user = RequestForm["User"];


            // Validate the milestone is present
            if (String.IsNullOrEmpty(RequestForm["Reason"]))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Required posted object 'Reason' is missing"), Response, Protocol, null);
                Response.StatusCode = 500;
                return;
            }

            // Get the username
            string reason = RequestForm["Reason"];

            // Ensure the web page does not already exist
            if (Engine_Database.WebContent_Delete_Page(webcontentId, reason, user, tracer))
            {
                // Clear cached data
                CachedDataManager.WebContent.Clear_All_Web_Content_Lists();
                CachedDataManager.WebContent.Clear_All_Web_Content_Pages();
                CachedDataManager.WebContent.Clear_Page_Details();

                // Also refresh the list of web content pages
                Engine_ApplicationCache_Gateway.RefreshWebContentHierarchy();

                // Respond with the success message
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Successful, null), Response, Protocol, null);
                Response.StatusCode = 200;
            }
            else
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Error deleting the web content page", tracer.Text_Trace), Response, Protocol, null);
                Response.StatusCode = 500;
            }
        }

        // <summary> Delete a non-aggregational top-level web content, static HTML page or redirect </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <param name="RequestForm"></param>
        /// <param name="IsDebug"></param>
        public void Update_HTML_Based_Content(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, NameValueCollection RequestForm, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.AddUpdate_HTML_Based_Content");

            // Validate the web content id exists in the URL
            int webcontentId;
            if ((UrlSegments.Count == 0) || (!Int32.TryParse(UrlSegments[0], out webcontentId)))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Invalid URL.  WebContentID missing from URL"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // Get and validate the required USER (string) posted request object
            if ((RequestForm["User"] == null) || (String.IsNullOrEmpty(RequestForm["User"])))
            {
                Serialize( new RestResponseMessage(ErrorRestTypeEnum.InputError, "Required posted object 'User' is missing") , Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }
            string user = RequestForm["User"];


            // Get and validate the required CONTENT (HTML_Based_Content) posted request objects
            if ((RequestForm["Content"] == null) || (String.IsNullOrEmpty(RequestForm["Content"])))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Required posted object 'Content' is missing"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }



            string contentString = RequestForm["Content"];
            HTML_Based_Content content = null;
            try
            {
                switch (Protocol)
                {
                    case Microservice_Endpoint_Protocol_Enum.JSON:
                    case Microservice_Endpoint_Protocol_Enum.JSON_P:
                        content = JSON.Deserialize<HTML_Based_Content>(contentString);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                        // Deserialize using the Protocol buffer-net library
                        byte[] byteArray = Encoding.ASCII.GetBytes(contentString);
                        MemoryStream mstream = new MemoryStream(byteArray);
                        content = Serializer.Deserialize<HTML_Based_Content>(mstream);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.XML:
                        byte[] byteArray2 = Encoding.UTF8.GetBytes(contentString);
                        MemoryStream mstream2 = new MemoryStream(byteArray2);
                        XmlSerializer x = new XmlSerializer(typeof(Content));
                        content = (HTML_Based_Content)x.Deserialize(mstream2);
                        break;
                }
            }
            catch (Exception ee)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Unable to deserialize 'Content' parameter to HTML_Based_Content: " + ee.Message), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // If content wasnot successfully deserialized, return error
            if ( content == null )
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Unable to deserialize 'Content' parameter to HTML_Based_Content"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // Level1 must be neither NULL nor empty
            if (String.IsNullOrEmpty(content.Level1))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Level1 of the content cannot be null or empty"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // Valiodate the web content id in the URL matches the object
            if (webcontentId != content.WebContentID)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "WebContentID from URL does not match Content posted object"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // You can't change the URL segments, so pull the current object to ensure that wasn't done
            HTML_Based_Content currentContent = CachedDataManager.WebContent.Retrieve_Page_Details(webcontentId, tracer);

            // If nothing was retrieved, build it
            if (currentContent == null)
            {
                // Try to read and return the html based content 
                // Get the details from the database
                WebContent_Basic_Info basicInfo = Engine_Database.WebContent_Get_Page(webcontentId, tracer);
                if (basicInfo == null)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Unable to pull web content data from the database");
                    Response.StatusCode = 500;
                    return;
                }

                // Set the content levels from the database object
                content.Level1 = basicInfo.Level1;
                content.Level2 = basicInfo.Level2;
                content.Level3 = basicInfo.Level3;
                content.Level4 = basicInfo.Level4;
                content.Level5 = basicInfo.Level5;
                content.Level6 = basicInfo.Level6;
                content.Level7 = basicInfo.Level7;
                content.Level8 = basicInfo.Level8;


                // If this has a redirect, return
                if (!String.IsNullOrEmpty(basicInfo.Redirect))
                {
                    currentContent = new HTML_Based_Content
                    {
                        Title = basicInfo.Title,
                        Level1 = basicInfo.Level1,
                        Level2 = basicInfo.Level2,
                        Level3 = basicInfo.Level3,
                        Level4 = basicInfo.Level4,
                        Level5 = basicInfo.Level5,
                        Level6 = basicInfo.Level6,
                        Level7 = basicInfo.Level7,
                        Level8 = basicInfo.Level8,
                        Locked = basicInfo.Locked,
                        Description = basicInfo.Summary,
                        Redirect = basicInfo.Redirect,
                        WebContentID = basicInfo.WebContentID
                    };
                }
                else
                {
                    // Build the HTML content
                    WebContentEndpointErrorEnum errorType;
                    currentContent = read_source_file(basicInfo, tracer, out errorType);
                }
            }

            // If the current value was pulled, determine what has been changed for the database note
            string changeMessage = "Updated web page";
            if (currentContent != null)
            {
                // If the redirect changed, just make that the message
                string currRedirect = String.Empty;
                string newRedirect = String.Empty;
                if (!String.IsNullOrEmpty(content.Redirect)) newRedirect = content.Redirect;
                if (!String.IsNullOrEmpty(currentContent.Redirect)) currRedirect = currentContent.Redirect;
                if (String.Compare(newRedirect, currRedirect, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if ((newRedirect.Length > 0) && (currRedirect.Length > 0))
                    {
                        changeMessage = "Changed redirect URL";
                    }
                    else if ( newRedirect.Length == 0 )
                    {
                        changeMessage = "Removed redirect URL";
                    }
                    else
                    {
                        changeMessage = "Added redirect URL";
                    }
                }
                else if (!AreEqual(content.ContentSource, currentContent.ContentSource))
                {
                    changeMessage = "Updated the text/html";
                }
                else
                {
                    List<string> updatesBuilderList = new List<string>();
                    if (!AreEqual(content.Author, currentContent.Author)) updatesBuilderList.Add("Author");
                    if (!AreEqual(content.Banner, currentContent.Banner)) updatesBuilderList.Add("Banner");
                    if (!AreEqual(content.CssFile, currentContent.CssFile)) updatesBuilderList.Add("Stylesheet");
                    if (!AreEqual(content.Description, currentContent.Description)) updatesBuilderList.Add("Description");
                    if (!AreEqual(content.Extra_Head_Info, currentContent.Extra_Head_Info)) updatesBuilderList.Add("Header Data");
                    if (!AreEqual(content.JavascriptFile, currentContent.JavascriptFile)) updatesBuilderList.Add("Javascript");
                    if (!AreEqual(content.Keywords, currentContent.Keywords)) updatesBuilderList.Add("Keywords");
                    if (!AreEqual(content.SiteMap, currentContent.SiteMap)) updatesBuilderList.Add("Tree Nav Group");
                    if (!AreEqual(content.Title, currentContent.Title)) updatesBuilderList.Add("Title");
                    if (!AreEqual(content.Web_Skin, currentContent.Web_Skin)) updatesBuilderList.Add("Web Skin");

                    if (updatesBuilderList.Count > 0)
                    {
                        if (updatesBuilderList.Count == 1)
                        {
                            changeMessage = "Updated the " + updatesBuilderList[0];
                        }
                        if (updatesBuilderList.Count == 2)
                        {
                            changeMessage = "Updated the " + updatesBuilderList[0] + " and " + updatesBuilderList[1];
                        }
                        if (updatesBuilderList.Count > 2)
                        {
                            StringBuilder updatesBuilder = new StringBuilder("Updated the ");
                            for (int i = 0; i < updatesBuilderList.Count; i++)
                            {
                                if (i == 0)
                                    updatesBuilder.Append(updatesBuilderList[0]);
                                else if (i < updatesBuilderList.Count - 1)
                                {
                                    updatesBuilder.Append(", " + updatesBuilderList[i]);
                                }
                                else
                                {
                                    updatesBuilder.Append(", and " + updatesBuilderList[i]);
                                }
                            }
                            changeMessage = updatesBuilder.ToString();
                        }
                    }
                }
            }

            // Get the location for this HTML file to be saved
            StringBuilder dirBuilder = new StringBuilder(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory + "design\\webcontent\\" + content.Level1);
            if (!String.IsNullOrEmpty(content.Level2))
            {
                dirBuilder.Append("\\" + content.Level2);
                if (!String.IsNullOrEmpty(content.Level3))
                {
                    dirBuilder.Append("\\" + content.Level3);
                    if (!String.IsNullOrEmpty(content.Level4))
                    {
                        dirBuilder.Append("\\" + content.Level4);
                        if (!String.IsNullOrEmpty(content.Level5))
                        {
                            dirBuilder.Append("\\" + content.Level5);
                            if (!String.IsNullOrEmpty(content.Level6))
                            {
                                dirBuilder.Append("\\" + content.Level6);
                                if (!String.IsNullOrEmpty(content.Level7))
                                {
                                    dirBuilder.Append("\\" + content.Level7);
                                    if (!String.IsNullOrEmpty(content.Level8))
                                    {
                                        dirBuilder.Append("\\" + content.Level8);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Ensure this directory exists
            if (!Directory.Exists(dirBuilder.ToString()))
            {
                try
                {
                    Directory.CreateDirectory(dirBuilder.ToString());
                }
                catch (Exception)
                {
                    Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Unable to create the directory for this web content page"), Response, Protocol, null);
                    Response.StatusCode = 500;
                    return;
                }
            }

            // Save the HTML file to the file system
            try
            {
                string fileName = Path.Combine(dirBuilder.ToString(), "default.html");

                // Make a backup from today, if none made yet
                if (File.Exists(fileName))
                {
                    DateTime lastWrite = (new FileInfo(fileName)).LastWriteTime;
                    string new_file = fileName.ToLower().Replace(".txt", "").Replace(".html", "").Replace(".htm", "") + lastWrite.Year + lastWrite.Month.ToString().PadLeft(2, '0') + lastWrite.Day.ToString().PadLeft(2, '0') + ".bak";
                    if (File.Exists(new_file))
                        File.Delete(new_file);
                    File.Move(fileName, new_file);
                }

                // Save the updated file
                content.Save_To_File(fileName);
            }
            catch (Exception)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Unable to save HTML file for this web content page"), Response, Protocol, null);
                Response.StatusCode = 500;
                return;
            }

            // Save to the database
            bool success;
            try
            {
                success = Engine_Database.WebContent_Edit_Page(webcontentId, content.Title, content.Description, content.Redirect, user, changeMessage, tracer);
                Engine_ApplicationCache_Gateway.WebContent_Hierarchy.Add_Single_Node(webcontentId, content.Redirect, content.Level1, content.Level2, content.Level3, content.Level4, content.Level5, content.Level6, content.Level7, content.Level8);
            }
            catch (Exception)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Unable to save the information for the web content page to the database"), Response, Protocol, null);
                Response.StatusCode = 500;
                return;
            }

            // Clear the cache
            CachedDataManager.WebContent.Clear_Page_Details(webcontentId);

            // If this was a failure, return a message
            if (!success)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Unable to save the updated information to the database"), Response, Protocol, null);
                Response.StatusCode = 500;
                return;
            }

            // Build return value
            RestResponseMessage message = new RestResponseMessage(ErrorRestTypeEnum.Successful, "Updated web page details");

            // Set the URL
            StringBuilder urlBuilder = new StringBuilder(Engine_ApplicationCache_Gateway.Settings.Servers.Base_URL + "/" + content.Level1);
            if (!String.IsNullOrEmpty(content.Level2))
            {
                urlBuilder.Append("/" + content.Level2);
                if (!String.IsNullOrEmpty(content.Level3))
                {
                    urlBuilder.Append("/" + content.Level3);
                    if (!String.IsNullOrEmpty(content.Level4))
                    {
                        urlBuilder.Append("/" + content.Level4);
                        if (!String.IsNullOrEmpty(content.Level5))
                        {
                            urlBuilder.Append("/" + content.Level5);
                            if (!String.IsNullOrEmpty(content.Level6))
                            {
                                urlBuilder.Append("/" + content.Level6);
                                if (!String.IsNullOrEmpty(content.Level7))
                                {
                                    urlBuilder.Append("/" + content.Level7);
                                    if (!String.IsNullOrEmpty(content.Level8))
                                    {
                                        urlBuilder.Append("/" + content.Level8);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            message.URI = urlBuilder.ToString();

            // Use the base class to serialize the object according to request protocol
            Serialize(message, Response, Protocol, null);
        }

        private static bool AreEqual(string a, string b)
        {
            if (String.IsNullOrEmpty(a))
            {
                return String.IsNullOrEmpty(b);
            }
            else if (String.IsNullOrEmpty(b))
            {
                return false;
            }
            else
            {
                return (String.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0);
            }
        }

        // <summary> Add a new HTML based web content page or redirect to the system </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <param name="RequestForm"></param>
        /// <param name="IsDebug"></param>
        public void Add_HTML_Based_Content(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, NameValueCollection RequestForm, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Add_HTML_Based_Content");

            //// Validate the web content id exists in the URL
            //int webcontentId;
            //if ((UrlSegments.Count == 0) || (!Int32.TryParse(UrlSegments[0], out webcontentId)))
            //{
            //    Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Invalid URL.  WebContentID missing from URL"), Response, Protocol, null);
            //    Response.StatusCode = 400;
            //    return;
            //}

            // Get and validate the required USER (string) posted request object
            if (String.IsNullOrEmpty(RequestForm["User"]))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Required posted object 'User' is missing"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }
            string user = RequestForm["User"];

            // Get and validate the required CONTENT (HTML_Based_Content) posted request objects
            if (String.IsNullOrEmpty(RequestForm["Content"]))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Required posted object 'Content' is missing"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            string contentString = RequestForm["Content"];
            HTML_Based_Content content = null;
            try
            {
                switch (Protocol)
                {
                    case Microservice_Endpoint_Protocol_Enum.JSON:
                    case Microservice_Endpoint_Protocol_Enum.JSON_P:
                        content = JSON.Deserialize<HTML_Based_Content>(contentString);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                        // Deserialize using the Protocol buffer-net library
                        byte[] byteArray = Encoding.ASCII.GetBytes(contentString);
                        MemoryStream mstream = new MemoryStream(byteArray);
                        content = Serializer.Deserialize<HTML_Based_Content>(mstream);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.XML:
                        byte[] byteArray2 = Encoding.UTF8.GetBytes(contentString);
                        MemoryStream mstream2 = new MemoryStream(byteArray2);
                        XmlSerializer x = new XmlSerializer(typeof(Content));
                        content = (HTML_Based_Content) x.Deserialize(mstream2);
                        break;
                }
            }
            catch (Exception ee)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Unable to deserialize 'Content' parameter to HTML_Based_Content: " + ee.Message), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // If content wasnot successfully deserialized, return error
            if (content == null)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Unable to deserialize 'Content' parameter to HTML_Based_Content"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // Level1 must be neither NULL nor empty
            if (String.IsNullOrEmpty(content.Level1))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Level1 of the content cannot be null or empty"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // Since this is a PUT verb, the URL should match the uploaded content
            bool invalidUrl = ((UrlSegments.Count < 1) || (UrlSegments[0] != content.Level1));
            if ((!invalidUrl) && (!String.IsNullOrEmpty(content.Level2)) && ((UrlSegments.Count < 2) || (UrlSegments[1] != content.Level2))) invalidUrl = true;
            if ((!invalidUrl) && (!String.IsNullOrEmpty(content.Level3)) && ((UrlSegments.Count < 3) || (UrlSegments[2] != content.Level3))) invalidUrl = true;
            if ((!invalidUrl) && (!String.IsNullOrEmpty(content.Level4)) && ((UrlSegments.Count < 4) || (UrlSegments[3] != content.Level4))) invalidUrl = true;
            if ((!invalidUrl) && (!String.IsNullOrEmpty(content.Level5)) && ((UrlSegments.Count < 5) || (UrlSegments[4] != content.Level5))) invalidUrl = true;
            if ((!invalidUrl) && (!String.IsNullOrEmpty(content.Level6)) && ((UrlSegments.Count < 6) || (UrlSegments[5] != content.Level6))) invalidUrl = true;
            if ((!invalidUrl) && (!String.IsNullOrEmpty(content.Level7)) && ((UrlSegments.Count < 7) || (UrlSegments[6] != content.Level7))) invalidUrl = true;
            if ((!invalidUrl) && (!String.IsNullOrEmpty(content.Level8)) && ((UrlSegments.Count < 8) || (UrlSegments[7] != content.Level8))) invalidUrl = true;

            // Valiodate the web content id in the URL matches the object
            if (invalidUrl)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "URL of PUT request does not match Content posted object"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // Ensure the first segment is not a reserved word
            foreach (string thisReserved in Engine_ApplicationCache_Gateway.Settings.Static.Reserved_Keywords)
            {
                if (String.Compare(thisReserved, content.Level1, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Level1 cannot be a system reserved word."), Response, Protocol, null);
                    Response.StatusCode = 400;
                    return;
                }
            }

            // Look for the inherit flag, if it is possible there is a parent (i.e, at least segment 2)
            if (UrlSegments.Count > 1)
            {
                // Get the inherit from parent flag
                bool inheritFromParent = ((RequestForm["Inherit"] != null) && (RequestForm["Inherit"].ToUpper() == "TRUE"));

                if (inheritFromParent)
                {
                    // Copy the current URL segment list
                    List<string> parentCheck = new List<string>(8);
                    parentCheck.AddRange(UrlSegments);
                    parentCheck.RemoveAt(parentCheck.Count - 1);

                    // Remove the last one
                    while (parentCheck.Count > 0)
                    {
                        // Is this a match?
                        WebContent_Hierarchy_Node possibleParent = Engine_ApplicationCache_Gateway.WebContent_Hierarchy.Find(parentCheck);
                        if (possibleParent != null)
                        {
                            // Declare the return object and look this up in the cache by ID
                            HTML_Based_Content parentValue = CachedDataManager.WebContent.Retrieve_Page_Details(possibleParent.WebContentID, tracer);

                            // If nothing was retrieved, build it
                            if (parentValue == null)
                            {
                                // Try to read and return the html based content 
                                // Get the details from the database
                                WebContent_Basic_Info parentBasicInfo = Engine_Database.WebContent_Get_Page(possibleParent.WebContentID, tracer);
                                if ((parentBasicInfo != null) && (String.IsNullOrEmpty(parentBasicInfo.Redirect)))
                                {
                                    // Try to Build the HTML content
                                    WebContentEndpointErrorEnum errorType;
                                    parentValue = read_source_file(parentBasicInfo, tracer, out errorType);

                                }
                            }

                            // If the parent was found, then copy over values
                            if (parentValue != null)
                            {
                                // Since everything was found, copy over the values and stop looking for a parent
                                if (!String.IsNullOrEmpty(parentValue.Banner)) content.Banner = parentValue.Banner;
                                if (!String.IsNullOrEmpty(parentValue.CssFile)) content.CssFile = parentValue.CssFile;
                                if (!String.IsNullOrEmpty(parentValue.Extra_Head_Info)) content.Extra_Head_Info = parentValue.Extra_Head_Info;
                                if (parentValue.IncludeMenu.HasValue) content.IncludeMenu = parentValue.IncludeMenu;
                                if (!String.IsNullOrEmpty(parentValue.JavascriptFile)) content.JavascriptFile = parentValue.JavascriptFile;
                                if (!String.IsNullOrEmpty(parentValue.SiteMap)) content.SiteMap = parentValue.SiteMap;
                                if (!String.IsNullOrEmpty(parentValue.Web_Skin)) content.Web_Skin = parentValue.Web_Skin;
                                break;
                            }
                        }

                        parentCheck.RemoveAt(parentCheck.Count - 1);
                    }
                }
            }



            // Get the location for this HTML file to be saved
            StringBuilder dirBuilder = new StringBuilder(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory + "design\\webcontent\\" + content.Level1);
            if (!String.IsNullOrEmpty(content.Level2))
            {
                dirBuilder.Append("\\" + content.Level2);
                if (!String.IsNullOrEmpty(content.Level3))
                {
                    dirBuilder.Append("\\" + content.Level3);
                    if (!String.IsNullOrEmpty(content.Level4))
                    {
                        dirBuilder.Append("\\" + content.Level4);
                        if (!String.IsNullOrEmpty(content.Level5))
                        {
                            dirBuilder.Append("\\" + content.Level5);
                            if (!String.IsNullOrEmpty(content.Level6))
                            {
                                dirBuilder.Append("\\" + content.Level6);
                                if (!String.IsNullOrEmpty(content.Level7))
                                {
                                    dirBuilder.Append("\\" + content.Level7);
                                    if (!String.IsNullOrEmpty(content.Level8))
                                    {
                                        dirBuilder.Append("\\" + content.Level8);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Ensure this directory exists
            if (!Directory.Exists(dirBuilder.ToString()))
            {
                try
                {
                    Directory.CreateDirectory(dirBuilder.ToString());
                }
                catch (Exception)
                {
                    Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Unable to create the directory for this web content page"), Response, Protocol, null);
                    Response.StatusCode = 500;
                    return;
                }
            }

            // Save the HTML file to the file system
            try
            {
                string fileName = Path.Combine(dirBuilder.ToString(), "default.html");
                if (!File.Exists(fileName))
                {
                    content.Save_To_File(fileName);
                }
            }
            catch (Exception)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Unable to save HTML file for this web content page"), Response, Protocol, null);
                Response.StatusCode = 500;
                return;
            }

            // Save to the database
            try
            {
                int webcontentid = Engine_Database.WebContent_Add_Page(content.Level1, content.Level2, content.Level3, content.Level4, content.Level5, content.Level6, content.Level7, content.Level8, user, content.Title, content.Description, content.Redirect, tracer);
                Engine_ApplicationCache_Gateway.WebContent_Hierarchy.Add_Single_Node(webcontentid, content.Redirect, content.Level1, content.Level2, content.Level3, content.Level4, content.Level5, content.Level6, content.Level7, content.Level8);
            }
            catch (Exception)
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Unable to save the information for the new web content page to the database"), Response, Protocol, null);
                Response.StatusCode = 500;
                return;
            }


            // Build return value
            RestResponseMessage message = new RestResponseMessage(ErrorRestTypeEnum.Successful, "Added new page");

            // Set the URL
            StringBuilder urlBuilder = new StringBuilder(Engine_ApplicationCache_Gateway.Settings.Servers.Base_URL + "/" + content.Level1);
            if (!String.IsNullOrEmpty(content.Level2))
            {
                urlBuilder.Append("/" + content.Level2);
                if (!String.IsNullOrEmpty(content.Level3))
                {
                    urlBuilder.Append("/" + content.Level3);
                    if (!String.IsNullOrEmpty(content.Level4))
                    {
                        urlBuilder.Append("/" + content.Level4);
                        if (!String.IsNullOrEmpty(content.Level5))
                        {
                            urlBuilder.Append("/" + content.Level5);
                            if (!String.IsNullOrEmpty(content.Level6))
                            {
                                urlBuilder.Append("/" + content.Level6);
                                if (!String.IsNullOrEmpty(content.Level7))
                                {
                                    urlBuilder.Append("/" + content.Level7);
                                    if (!String.IsNullOrEmpty(content.Level8))
                                    {
                                        urlBuilder.Append("/" + content.Level8);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            message.URI = urlBuilder.ToString();

            // Use the base class to serialize the object according to request protocol
            Serialize(message, Response, Protocol, null);
        }

        // <summary> Add a milestone to a static HTML page or redirect </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <param name="RequestForm"></param>
        /// <param name="IsDebug"></param>
        public void Add_Milestone(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, NameValueCollection RequestForm, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Add_Milestone");

            // Validate the web content id exists in the URL
            int webcontentId;
            if ((UrlSegments.Count == 0) || (!Int32.TryParse(UrlSegments[0], out webcontentId)))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Invalid URL.  WebContentID missing from URL"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }

            // Get and validate the required USER (string) posted request object
            if ((RequestForm["User"] == null) || (String.IsNullOrEmpty(RequestForm["User"])))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Required posted object 'User' is missing"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }
            string user = RequestForm["User"];

            // Get and validate the required MILESTONE (string) posted request object
            if ((RequestForm["Milestone"] == null) || (String.IsNullOrEmpty(RequestForm["Milestone"])))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.InputError, "Required posted object 'Milestone' is missing"), Response, Protocol, null);
                Response.StatusCode = 400;
                return;
            }
            string milestone = RequestForm["Milestone"];

            // Ensure the web page does not already exist
            if (Engine_Database.WebContent_Add_Milestone(webcontentId, milestone, user, tracer))
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Successful, null), Response, Protocol, null);
                Response.StatusCode = 200;
            }
            else
            {
                Serialize(new RestResponseMessage(ErrorRestTypeEnum.Exception, "Error adding milestone to database", tracer.Text_Trace), Response, Protocol, null);
                Response.StatusCode = 500;
            }

            // Clear any cached list of global updates
            CachedDataManager.WebContent.Clear_Global_Recent_Updates();
            CachedDataManager.WebContent.Clear_Global_Recent_Updates_NextLevel();
            CachedDataManager.WebContent.Clear_Global_Recent_Updates_Users();
        }

        /// <summary> Gets the special missing web content page, used when a requested resource is missing </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Special_Missing_Page(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            tracer.Add_Trace("WebContentServices.Get_Special_Missing_Page");

            // Declare the return object and look this up in the cache by ID
            HTML_Based_Content simpleWebContent = CachedDataManager.WebContent.Retrieve_Special_Missing_Page(tracer);

            if (simpleWebContent == null)
            {
                string file = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory, "design", "webcontent", "missing.html");
                if (!File.Exists(file))
                {
                    try
                    {
                        // Try to create the directory
                        string directory = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory, "design", "webcontent");
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        // Try to write the file
                        StreamWriter writer = new StreamWriter(file);
                        writer.WriteLine("<html>");
                        writer.WriteLine("<head>");
                        writer.WriteLine("  <title>No Page Found</title>");
                        writer.WriteLine("</head>");
                        writer.WriteLine("<body>");
                        writer.WriteLine("  <div style=\"padding:10px 40px 20px 40px; text-align:left;\">");
                        writer.WriteLine("    <div style=\"width: 100%;padding-bottom: 5px; border-bottom: 2px solid #bbbbbb; margin-bottom: 20px;\">");
                        writer.WriteLine("      <img style=\"float: left;margin-right: 15px;margin-left: 10px;\" src=\"[%BASEURL%]default/images/misc/warning.png\" alt=\"\" />");
                        writer.WriteLine("      <h1 style=\"text-align: left;font-size: 18px;padding-top: 5px;\">Page Not Found</h1>");
                        writer.WriteLine("    </div>");
                        writer.WriteLine();

                        writer.WriteLine("");
                        writer.WriteLine("    <p>The resource you requested does not exist.</p>");
                        writer.WriteLine("");
                        writer.WriteLine("    <p>If you are looking for an individual resource, search from the <a href=\"[%BASEURL%]\">main home page</a>.</p>");
                        writer.WriteLine("");
                        writer.WriteLine("    <p>If you are looking for an individual item aggregation, click <a href=\"[%BASEURL%]tree/expanded\">here to view all existing item aggregations</a>.</p>");
                        writer.WriteLine("");
                        writer.WriteLine("  </div>");
                        writer.WriteLine("</body>");
                        writer.WriteLine("</html>");
                        writer.Flush();
                        writer.Close();
                    }
                    catch (Exception ee)
                    {
                        // This will result in an error anyway, but log it
                        tracer.Add_Trace("SobekCM_Assistant.Get_Special_Missing_Page", "Error trying to create the default.html web content page to use for missing content");
                        tracer.Add_Trace("SobekCM_Assistant.Get_Special_Missing_Page", "Attempted to create " + file);
                        tracer.Add_Trace("SobekCM_Assistant.Get_Special_Missing_Page", "Error was: " + ee.Message);
                    }
                }

                // Now, try to pull it again
                try
                {

                    simpleWebContent = HTML_Based_Content_Reader.Read_HTML_File(file, true, tracer);
                    if (simpleWebContent == null)
                    {
                        tracer.Add_Trace("WebContentServices.Get_Special_Missing_Page", "Error reading the missing.html special file");

                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("Unable to read existing source file");
                        Response.StatusCode = 500;

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);
                        }
                        return;
                    }

                    simpleWebContent.WebContentID = -1;

                    // Store this on the cache
                    CachedDataManager.WebContent.Store_Special_Missing_Page(simpleWebContent, tracer);
                }
                catch (Exception ee)
                {
                    tracer.Add_Trace("WebContentServices.Get_Special_Missing_Page", "Unknown error caught");

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Unable to read existing source file");
                    Response.StatusCode = 500;

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                    }
                    return;
                }
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_Special_Missing_Page", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseWebContent";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(simpleWebContent, Response, Protocol, json_callback);

        }

        /// <summary> Get the list of milestones affecting a single (non aggregation affiliated) static web content page </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Single_Milestones(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Single_Milestones");

            // Must at least have one URL segment for the ID
            if (UrlSegments.Count > 0)
            {
                int webContentId;
                if (!Int32.TryParse(UrlSegments[0], out webContentId))
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Invalid parameter - expects primary key integer value to the web content object (WebContentID) as part of URL");
                    Response.StatusCode = 500;

                    // If this was debug mode, then write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                // Get the report of recent updates
                Single_WebContent_Change_Report returnValue = Engine_Database.WebContent_Get_Milestones(webContentId, tracer);

                // If null was returned, an error occurred
                if (returnValue == null)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Unable to pull milestones from the database");
                    Response.StatusCode = 500;

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    tracer.Add_Trace("WebContentServices.Get_Single_Milestones", "Debug mode detected");
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parseRecentUpdates";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnValue, Response, Protocol, json_callback);
            }
            else
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Invalid parameter - expects primary key integer value to the web content object (WebContentID) as part of URL");
                Response.StatusCode = 500;
            }
        }

        /// <summary> Get the complete monthly usage for a single web content page </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Single_Usage_Report(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Single_Usage_Report");

            // Must at least have one URL segment for the ID
            if (UrlSegments.Count > 0)
            {
                int webContentId;
                if (!Int32.TryParse(UrlSegments[0], out webContentId))
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Invalid parameter - expects primary key integer value to the web content object (WebContentID) as part of URL");
                    Response.StatusCode = 500;

                    // If this was debug mode, then write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                // Get the dataset of results
                Single_WebContent_Usage_Report returnValue = Engine_Database.WebContent_Get_Usage(webContentId, tracer);

                // If null was returned, an error occurred
                if (returnValue == null)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Unable to pull usage report from the database");
                    Response.StatusCode = 500;

                    // If this was debug mode, then write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    tracer.Add_Trace("WebContentServices.Get_Single_Usage_Report", "Debug mode detected");
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parseWebContentUsageReport";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnValue, Response, Protocol, json_callback);
            }
            else
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Invalid parameter - expects primary key integer value to the web content object (WebContentID) as part of URL");
                Response.StatusCode = 500;
            }
        }

        /// <summary> [PUBLIC] Get the list of uploaded images for a particular aggregation </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        /// <remarks> This REST API should be publicly available for users that are performing administrative work </remarks>
        public void GetUploadedImages(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            tracer.Add_Trace("WebContentServices.GetUploadedImages", "Compute web content id from url segments");

            // If no URL segments, then invalid
            if (UrlSegments.Count == 0)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Invalid request : must include URL segments to specify the web content page");
                Response.StatusCode = 400;
            }

            if (Engine_ApplicationCache_Gateway.WebContent_Hierarchy == null)
            {
                tracer.Add_Trace("WebContentServices.GetUploadedImages", "Unable to pull web content hierarchy from engine application cache");

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull web content hierarchy from engine application cache");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }

                return;
            }

            // Look this up from the web content hierarchy object
            WebContent_Hierarchy_Node matchedNode = Engine_ApplicationCache_Gateway.WebContent_Hierarchy.Find(UrlSegments);
            if (matchedNode == null)
            {
                tracer.Add_Trace("WebContentServices.GetUploadedImages", "Requested page does not exist");

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Requested page does not exist");
                Response.StatusCode = 404;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Since we now know this is a valid web content page, let's just return the uploaded images
            List<UploadedFileFolderInfo> serverFiles = new List<UploadedFileFolderInfo>();

            // Build the folder which will include the uploads
            StringBuilder designFolderBldr = new StringBuilder( "webcontent\\" + UrlSegments[0]);
            if (UrlSegments.Count > 1) designFolderBldr.Append("\\" + UrlSegments[1]);
            if (UrlSegments.Count > 2) designFolderBldr.Append("\\" + UrlSegments[2]);
            if (UrlSegments.Count > 3) designFolderBldr.Append("\\" + UrlSegments[3]);
            if (UrlSegments.Count > 4) designFolderBldr.Append("\\" + UrlSegments[4]);
            if (UrlSegments.Count > 5) designFolderBldr.Append("\\" + UrlSegments[5]);
            if (UrlSegments.Count > 6) designFolderBldr.Append("\\" + UrlSegments[6]);
            if (UrlSegments.Count > 7) designFolderBldr.Append("\\" + UrlSegments[7]);

            // Check that folder
            string design_folder = Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + designFolderBldr;
            if (Directory.Exists(design_folder))
            {
                string foldername = "Uploads";

                string[] files = SobekCM_File_Utilities.GetFiles(design_folder, "*.jpg|*.bmp|*.gif|*.png");
                string design_url = Engine_ApplicationCache_Gateway.Settings.Servers.System_Base_URL + "design/" + designFolderBldr.ToString().Replace("\\", "/") + "/";
                foreach (string thisFile in files)
                {
                    string filename = Path.GetFileName(thisFile);
                    string extension = Path.GetExtension(thisFile);

                    // Exclude some files
                    if ((!String.IsNullOrEmpty(extension)) && (extension.ToLower().IndexOf(".db") < 0) && (extension.ToLower().IndexOf("bridge") < 0) && (extension.ToLower().IndexOf("cache") < 0))
                    {
                        string url = design_url + filename;
                        serverFiles.Add(new UploadedFileFolderInfo(url, foldername));
                    }
                }
            }

            JSON.Serialize(serverFiles, Response.Output, Options.ISO8601ExcludeNulls);

        }

        #endregion

        #region Endpoints related to overall management of the web content pages through the admin pages

        #region Endpoints related to global recent updates

        /// <summary> Returns a flag indicating if there are any global recent updates to the web content entities (pages and redirects) </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        /// <remarks> This is not the most efficient way to verify existence of the updates since this actually pulls the entire list of 
        /// recent updates from the database.  However, this will likely be cached and be needed immediately anyway. </remarks>
        public void Has_Global_Recent_Updates(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Create the tracer and add a trace
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Has_Global_Recent_Updates");

            // Get the dataset of recent updates
            DataSet changes = get_global_recent_updates_set(tracer);

            // If null was returned, an error occurred
            if (changes == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull recent updates from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Were there updates?
            bool returnValue = changes.Tables[0].Rows.Count > 0;
            tracer.Add_Trace("WebContentServices.Has_Global_Recent_Updates","Will return value " + returnValue.ToString().ToUpper());


            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Has_Global_Recent_Updates", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseHasGlobalRecentUpdates";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the list of all the recent updates to all (non aggregation affiliated) static web content pages </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Global_Recent_Updates(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Set the default page information
            int page = 1;
            int rows_per_page = 100;

            // Check for rows_per_page from the query string
            if (!String.IsNullOrEmpty(QueryString["rowsPerPage"]))
            {
                if (!Int32.TryParse(QueryString["rowsPerPage"], out rows_per_page))
                    rows_per_page = 100;
                else if (rows_per_page < 1)
                    rows_per_page = 100;
            }

            // Look for the page number
            if (UrlSegments.Count > 0)
            {
                if (!Int32.TryParse(UrlSegments[0], out page))
                    page = 1;
                else if (page < 1)
                    page = 1;
            }

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Recent_Updates","Get recent updates page " + page + " with " + rows_per_page + " rows per page");

            // Check for a user filter
            string userFilter = QueryString["user"];
            if (!String.IsNullOrWhiteSpace(userFilter))
                tracer.Add_Trace("WebContentServices.Get_Recent_Updates", "Filter by user '" + userFilter + "'");
         
            // Get the dataset of recent updates
            DataSet changes = get_global_recent_updates_set(tracer); 

            // If null was returned, an error occurred
            if (changes == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull recent updates from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the list for the results
            WebContent_Recent_Changes returnValue = new WebContent_Recent_Changes
            {
                Page = page, 
                RowsPerPage = rows_per_page
            };

            // Prepare to step through the rows requested and convert them for returning
            int row_counter = (page - 1) * rows_per_page;
            int final_row_counter = page * rows_per_page;

            // If this is to filter by a user, use a DataViewer
            if (!String.IsNullOrWhiteSpace(userFilter))
            {
                returnValue.UserFilter = userFilter;

                try
                {
                    // Filter by the user
                    DataView view = new DataView(changes.Tables[0])
                    {
                        RowFilter = "MilestoneUser = '" + userFilter + "'"
                    };

                    // Set the total number of changes for this user
                    returnValue.Total = view.Count;

                    // Add the changes within the page requested
                    while ((row_counter < view.Count) && (row_counter < final_row_counter))
                    {
                        returnValue.Changes.Add(datarow_to_webcontent_recent_changed_page(view[row_counter].Row));
                        row_counter++;
                    }
                }
                catch (Exception ee)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error converting filtered rows to return objects");
                    Response.StatusCode = 500;

                    // If this was debug mode, then just write the tracer
                    if ( IsDebug )
                    {
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine(ee.StackTrace);
                    }
                    return;
                }
            }
            else
            {
                try
                {
                    // Set the total number of changes 
                    returnValue.Total = changes.Tables[0].Rows.Count;

                    // Add the changes within the page requested
                    while ((row_counter < changes.Tables[0].Rows.Count) && (row_counter < final_row_counter))
                    {
                        returnValue.Changes.Add(datarow_to_webcontent_recent_changed_page(changes.Tables[0].Rows[row_counter]));
                        row_counter++;
                    }
                }
                catch (Exception ee)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error converting rows to return objects");
                    Response.StatusCode = 500;

                    // If this was debug mode, then just write the tracer
                    if ( IsDebug )
                    {
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine(ee.StackTrace);
                    }
                    return;
                }
            }

            
            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                tracer.Add_Trace("WebContentServices.Get_Global_Recent_Updates", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseGlobalRecentUpdates";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the list of all the recent updates for consumption by the jQuery DataTable.net plug-in </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Global_Recent_Updates_JDataTable(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Get ready to pull the informaiton from the query string which the
            // jquery datatables library pass in
            int displayStart;
            int displayLength;
            int sortingColumn1;
            string sortDirection1 = "asc";

            // Get the display start and length from the DataTables generated data URL
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayStart"], out displayStart)) displayStart = 0;
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayLength"], out displayLength)) displayLength = 50;

            // Get the echo value
            string sEcho = HttpContext.Current.Request.QueryString["sEcho"];

            // Get the sorting column and sorting direction
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iSortCol_0"], out sortingColumn1)) sortingColumn1 = 0;
            if ((HttpContext.Current.Request.QueryString["sSortDir_0"] != null) && (HttpContext.Current.Request.QueryString["sSortDir_0"] == "desc"))
                sortDirection1 = "desc";

            // Get the dataset of recent updates
            DataSet changes = get_global_recent_updates_set(tracer); 

            // If null was returned, an error occurred
            if (changes == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull pages list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the view for sorting and filtering
            DataView resultsView = new DataView(changes.Tables[0]);

            // Check for a user filter
            string userFilter = QueryString["user"];
            if (!String.IsNullOrWhiteSpace(userFilter))
            {
                resultsView.RowFilter = "MilestoneUser='" + userFilter + "'";
            }
            else
            {

                // Should a filter be applied?
                if (!String.IsNullOrEmpty(QueryString["l1"]))
                {
                    string level1_filter = QueryString["l1"];
                    if (!String.IsNullOrEmpty(QueryString["l2"]))
                    {
                        string level2_filter = QueryString["l2"];
                        if (!String.IsNullOrEmpty(QueryString["l3"]))
                        {
                            string level3_filter = QueryString["l3"];
                            if (!String.IsNullOrEmpty(QueryString["l4"]))
                            {
                                string level4_filter = QueryString["l4"];
                                if (!String.IsNullOrEmpty(QueryString["l5"]))
                                {
                                    string level5_filter = QueryString["l5"];
                                    resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "' and Level5='" + level5_filter + "'";
                                }
                                else
                                {
                                    resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "'";
                                }
                            }
                            else
                            {
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "'";
                            }
                        }
                        else
                        {
                            resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
                        }
                    }
                    else
                    {
                        resultsView.RowFilter = "Level1='" + level1_filter + "'";
                    }
                }
            }

            // Get the count of results
            int total_results = resultsView.Count;

            // If this was set to show ALL results, set some page/length information
            if (displayLength == -1)
            {
                displayStart = 0;
                displayLength = total_results;
            }

            // Start the JSON response
            Response.Output.WriteLine("{");
            Response.Output.WriteLine("\"sEcho\": " + sEcho + ",");
            Response.Output.WriteLine("\"iTotalRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"iTotalDisplayRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"aaData\": [");

            // Sort by the correct column
            if ((sortingColumn1 > 0) || (sortDirection1 != "asc"))
            {
                if (sortingColumn1 == 0)
                {
                    // Must be descending column zero then
                    resultsView.Sort = "Level1 desc, Level2 desc, Level3 desc, Level4 desc, Level5 desc, Level6 desc, Level7 desc, Level8 desc";
                }
                else if (sortingColumn1 == 1)
                {
                    resultsView.Sort = "Title " + sortDirection1;
                }
                else if (sortingColumn1 == 2)
                {
                    resultsView.Sort = "MilestoneDate " + sortDirection1;
                }
                else if (sortingColumn1 == 3)
                {
                    resultsView.Sort = "MilestoneUser " + sortDirection1;
                }
                else if (sortingColumn1 == 4)
                {
                    resultsView.Sort = "Milestone " + sortDirection1;
                }
            }

            // Add the data for the rows to show
            for (int i = displayStart; (i < displayStart + displayLength) && (i < total_results); i++)
            {
                // Start the JSON response for this row
                DataRow thisRow = resultsView[i].Row;

                Response.Output.Write("[\"" + thisRow["Level1"]);
                if ((thisRow["Level2"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level2"].ToString())))
                {
                    Response.Output.Write("/" + thisRow["Level2"]);
                    if ((thisRow["Level3"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level3"].ToString())))
                    {
                        Response.Output.Write("/" + thisRow["Level3"]);
                        if ((thisRow["Level4"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level4"].ToString())))
                        {
                            Response.Output.Write("/" + thisRow["Level4"]);
                            if ((thisRow["Level5"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level5"].ToString())))
                            {
                                Response.Output.Write("/" + thisRow["Level5"]);
                                if ((thisRow["Level6"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level6"].ToString())))
                                {
                                    Response.Output.Write("/" + thisRow["Level6"]);
                                    if ((thisRow["Level7"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level7"].ToString())))
                                    {
                                        Response.Output.Write("/" + thisRow["Level7"]);
                                        if ((thisRow["Level8"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level8"].ToString())))
                                        {
                                            Response.Output.Write("/" + thisRow["Level8"]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Response.Output.Write("\", ");
                Response.Output.Write("\"" + thisRow["Title"] + "\", ");
                Response.Output.Write("\"" + (DateTime.Parse(thisRow["MilestoneDate"].ToString())).ToString() + "\", ");
                Response.Output.Write("\"" + thisRow["MilestoneUser"] + "\", ");
                Response.Output.Write("\"" + thisRow["Milestone"] + "\" ");

                // Finish this row
                if ((i < displayStart + displayLength - 1) && (i < total_results - 1))
                    Response.Output.WriteLine("],");
                else
                    Response.Output.WriteLine("]");
            }

            Response.Output.WriteLine("]");
            Response.Output.WriteLine("}");
        }

        /// <summary> Gets the list of possible next level from an existing page in the recent updates </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Global_Recent_Updates_NextLevel(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Get_Global_Recent_Updates_NextLevel", "Into endpoint code");

            // Get the dataset of recent updates
            DataSet changes = get_global_recent_updates_set(tracer); 

            // If null was returned, an error occurred
            if (changes == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull recent updates from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Start the return list
            List<string> returnValue = new List<string>();

            try
            {

                // There are two special cases that we are already prepared for from the database.
                //    - The very top level
                //    - The second level
                if (UrlSegments.Count <= 1)
                {
                    // One of the two special cases
                    if (UrlSegments.Count == 0)
                    {
                        tracer.Add_Trace("WebContentServices.Get_Global_Recent_Updates_NextLevel", "Special case, top level");
                        foreach (DataRow thisRow in changes.Tables[2].Rows)
                        {
                            returnValue.Add(thisRow[0].ToString().ToLower());
                        }
                    }
                    else
                    {
                        tracer.Add_Trace("WebContentServices.Get_Global_Recent_Updates_NextLevel", "Special case, second level");
                        string level1_special = UrlSegments[0];
                        DataView specialView = new DataView(changes.Tables[3])
                        {
                            RowFilter = "Level1 = '" + level1_special + "'"
                        };

                        foreach (DataRowView thisRow in specialView)
                        {
                            returnValue.Add(thisRow[1].ToString().ToLower());
                        }
                    }
                }
                else
                {
                    tracer.Add_Trace("WebContentServices.Get_Global_Recent_Updates_NextLevel", "Deeper, non-special case.  Preparing to scan list.");
                    string level1 = UrlSegments[0];
                    string level2 = UrlSegments[1];

                    // Build the filter
                    StringBuilder filterBuilder = new StringBuilder("Level1='" + level1 + "' and Level2='" + level2 + "'");
                    int column_counter = 3;
                    if ((UrlSegments.Count > 2) && (!String.IsNullOrWhiteSpace(UrlSegments[2])))
                    {
                        filterBuilder.Append(" and Level3='" + UrlSegments[2] + "'");
                        column_counter++;

                        if ((UrlSegments.Count > 3) && (!String.IsNullOrWhiteSpace(UrlSegments[3])))
                        {
                            filterBuilder.Append(" and Level4='" + UrlSegments[3] + "'");
                            column_counter++;

                            if ((UrlSegments.Count > 4) && (!String.IsNullOrWhiteSpace(UrlSegments[4])))
                            {
                                filterBuilder.Append(" and Level5='" + UrlSegments[4] + "'");
                                column_counter++;

                                if ((UrlSegments.Count > 5) && (!String.IsNullOrWhiteSpace(UrlSegments[5])))
                                {
                                    filterBuilder.Append(" and Level6='" + UrlSegments[5] + "'");
                                    column_counter++;

                                    if ((UrlSegments.Count > 6) && (!String.IsNullOrWhiteSpace(UrlSegments[6])))
                                    {
                                        filterBuilder.Append(" and Level7='" + UrlSegments[6] + "'");
                                        column_counter++;
                                    }
                                }
                            }
                        }
                    }

                    tracer.Add_Trace("WebContentServices.Get_Global_Recent_Updates_NextLevel", filterBuilder.ToString());

                    // Create the dataview
                    DataView specialView = new DataView(changes.Tables[0])
                    {
                        RowFilter = filterBuilder.ToString()
                    };


                    // Step through and add each NEW term to a sorted dictionary
                    SortedDictionary<string, string> sortList = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!sortList.ContainsKey(thisTerm)))
                                sortList[thisTerm.ToLower()] = thisTerm;
                        }
                    }

                    // Now, copy the keys over to the return value
                    returnValue.AddRange(sortList.Select(Kvp => Kvp.Key));
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error encountered determing next level");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.StackTrace);

                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_Global_Recent_Updates_NextLevel", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseGlobalUpdatesNextLevel";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the list of all users that have participated in the recent updates to all top-level static web content pages </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Global_Recent_Updates_Users(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Recent_Updates_Users", "Get the list of all users that have participated in the recent updates to all top-level static web content pages");

            // Get the dataset of recent updates
            DataSet changes = get_global_recent_updates_set(tracer);

            // If null was returned, an error occurred
            if (changes == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull recent updates from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Get the list of users
            List<string> userList = new List<string>();
            if ((changes.Tables.Count > 1) && (changes.Tables[1].Rows.Count > 0))
            {
                foreach (DataRow userRow in changes.Tables[1].Rows)
                {
                    userList.Add(userRow[0].ToString());
                }
            }

            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                tracer.Add_Trace("WebContentServices.Get_Global_Recent_Updates_Users", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseGlobalRecentUpdatesUsers";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(userList, Response, Protocol, json_callback);
        }

        private WebContent_Recent_Changed_Page datarow_to_webcontent_recent_changed_page(DataRow ChangeRow)
        {
            // Start to buid the object to report this work
            WebContent_Recent_Changed_Page recentChange = new WebContent_Recent_Changed_Page
            {
                WebContentID = Int32.Parse(ChangeRow[0].ToString()), 
                Level1 = ChangeRow[1].ToString()
            };

            // Grab the values out and assign them
            if ((ChangeRow[2] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[2].ToString())))
            {
                recentChange.Level2 = ChangeRow[2].ToString();
                if ((ChangeRow[3] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[3].ToString())))
                {
                    recentChange.Level3 = ChangeRow[3].ToString();
                    if ((ChangeRow[4] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[4].ToString())))
                    {
                        recentChange.Level4 = ChangeRow[4].ToString();
                        if ((ChangeRow[5] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[5].ToString())))
                        {
                            recentChange.Level5 = ChangeRow[5].ToString();
                            if ((ChangeRow[6] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[6].ToString())))
                            {
                                recentChange.Level6 = ChangeRow[6].ToString();
                                if ((ChangeRow[7] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[7].ToString())))
                                {
                                    recentChange.Level7 = ChangeRow[7].ToString();
                                    if ((ChangeRow[8] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[8].ToString())))
                                    {
                                        recentChange.Level8 = ChangeRow[8].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            recentChange.MilestoneDate = DateTime.Parse(ChangeRow[9].ToString());
            recentChange.User = ChangeRow[10].ToString();
            recentChange.Notes = ChangeRow[11].ToString();
            recentChange.Title = ChangeRow[12].ToString();

            return recentChange;
        }

        /// <summary> Get the full data set of all global recent updates to any top-level static pages </summary>
        /// <param name="Tracer"> Custom tracer </param>
        /// <returns></returns>
        private DataSet get_global_recent_updates_set(Custom_Tracer Tracer)
        {
            // Look in the cache first
            DataSet fromCache = CachedDataManager.WebContent.Retrieve_Global_Recent_Updates(Tracer);
            if (fromCache != null)
                return fromCache;

            // Try to pull from the database
            DataSet fromDb = Engine_Database.WebContent_Get_Recent_Changes(Tracer);

            // Store in the cache if not null
            if (fromDb != null)
            {
                CachedDataManager.WebContent.Store_Global_Recent_Updates(fromDb, Tracer);
            }

            return fromDb;
        }

        #endregion

        #region Endpoint related to the usage statistics reports of all web content pages

        /// <summary> Returns a flag indicating if any usage has been reported for this instance's web content entities (pages and redirects) </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Has_Global_Usage(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Create the tracer and add a trace
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Has_Global_Usage");

            // Look in the cache
            bool? cacheFlag = CachedDataManager.WebContent.Retrieve_Has_Global_Usage_Flag(tracer);
            bool flag;
            if (cacheFlag.HasValue)
                flag = cacheFlag.Value;
            else
            {
                // Get the flag
                flag = Engine_Database.WebContent_Has_Usage(tracer);

                CachedDataManager.WebContent.Store_Has_Global_Usage_Flag(flag, tracer);
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Has_Global_Usage", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseHasWebContentUsage";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(flag, Response, Protocol, json_callback);
        }

        /// <summary> Get usage of all web content pages between two dates </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Global_Usage_Report(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Set the default page information
            int page = 1;
            int rows_per_page = 100;

            // Check for rows_per_page from the query string
            if (!String.IsNullOrEmpty(QueryString["rowsPerPage"]))
            {
                if (!Int32.TryParse(QueryString["rowsPerPage"], out rows_per_page))
                    rows_per_page = 100;
                else if (rows_per_page < 1)
                    rows_per_page = 100;
            }

            // Look for the page number
            if (UrlSegments.Count > 0)
            {
                if (!Int32.TryParse(UrlSegments[0], out page))
                    page = 1;
                else if (page < 1)
                    page = 1;
            }

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report", "Pull usage report page " + page + " with " + rows_per_page + " rows per page");

            // Determine the range
            int year1 = 2000;
            int month1 = 1;
            int year2 = DateTime.Now.Year + 1;
            int month2 = 1;
            int temp;
            if ((!String.IsNullOrEmpty(QueryString["year1"])) && (Int32.TryParse(QueryString["year1"].ToUpper(), out temp)))
                year1 = temp;
            if ((!String.IsNullOrEmpty(QueryString["month1"])) && (Int32.TryParse(QueryString["month1"].ToUpper(), out temp)))
                month1 = temp;
            if ((!String.IsNullOrEmpty(QueryString["year2"])) && (Int32.TryParse(QueryString["year2"].ToUpper(), out temp)))
                year2 = temp;
            if ((!String.IsNullOrEmpty(QueryString["month2"])) && (Int32.TryParse(QueryString["month2"].ToUpper(), out temp)))
                month2 = temp;

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report", "Report will be from " + year1 + "/" + month1 + " and " + year2 + "/" + month2 );
            
            // Get the dataset of results
            DataSet pages = get_global_usage_report_dataset(year1, month1, year2, month2, tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull usage report from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the list for the results
            WebContent_Usage_Report returnValue = new WebContent_Usage_Report
            {
                Page = page,
                RowsPerPage = rows_per_page,
                RangeStart = year1 + "-" + month1,
                RangeEnd = year2 + "-" + month2
            };

            // Prepare to step through the rows requested and convert them for returning
            int row_counter = (page - 1)*rows_per_page;
            int final_row_counter = page*rows_per_page;


            try
            {
                // Set the total number of changes 
                returnValue.Total = pages.Tables[0].Rows.Count;

                // Add the changes within the page requested
                while ((row_counter < pages.Tables[0].Rows.Count) && (row_counter < final_row_counter))
                {
                    returnValue.Pages.Add(datarow_to_page_usage(pages.Tables[0].Rows[row_counter]));
                    row_counter++;
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error converting rows to return objects");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine(ee.StackTrace);
                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseWebContentUsageReport";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

 

        private WebContent_Page_Usage datarow_to_page_usage(DataRow ChangeRow)
        {
            // Start to buid the object to report this work
            WebContent_Page_Usage usedPage = new WebContent_Page_Usage
            {
                Level1 = ChangeRow[0].ToString(),
                Hits = Int32.Parse(ChangeRow[10].ToString()),
                HitsHierarchical = Int32.Parse(ChangeRow[11].ToString()),
                Title = ChangeRow[9].ToString()
            };

            // Grab the values out and assign them
            if ((ChangeRow[1] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[1].ToString())))
            {
                usedPage.Level2 = ChangeRow[1].ToString();
                if ((ChangeRow[2] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[2].ToString())))
                {
                    usedPage.Level3 = ChangeRow[2].ToString();
                    if ((ChangeRow[3] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[3].ToString())))
                    {
                        usedPage.Level4 = ChangeRow[3].ToString();
                        if ((ChangeRow[4] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[4].ToString())))
                        {
                            usedPage.Level5 = ChangeRow[4].ToString();
                            if ((ChangeRow[5] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[5].ToString())))
                            {
                                usedPage.Level6 = ChangeRow[5].ToString();
                                if ((ChangeRow[6] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[6].ToString())))
                                {
                                    usedPage.Level7 = ChangeRow[6].ToString();
                                    if ((ChangeRow[7] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[7].ToString())))
                                    {
                                        usedPage.Level8 = ChangeRow[7].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return usedPage;
        }

        /// <summary> Get the list of usage for a global usage report for consumption by the jQuery DataTable.net plug-in </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Global_Usage_Report_JDataTable(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Get ready to pull the informaiton from the query string which the
            // jquery datatables library pass in
            int displayStart;
            int displayLength;
            int sortingColumn1;
            string sortDirection1 = "asc";

            // Get the display start and length from the DataTables generated data URL
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayStart"], out displayStart)) displayStart = 0;
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayLength"], out displayLength)) displayLength = 50;

            // Get the echo value
            string sEcho = HttpContext.Current.Request.QueryString["sEcho"];

            // Get the sorting column and sorting direction
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iSortCol_0"], out sortingColumn1)) sortingColumn1 = 0;
            if ((HttpContext.Current.Request.QueryString["sSortDir_0"] != null) && (HttpContext.Current.Request.QueryString["sSortDir_0"] == "desc"))
                sortDirection1 = "desc";

            // Determine the range
            int year1 = 2000;
            int month1 = 1;
            int year2 = DateTime.Now.Year + 1;
            int month2 = 1;
            int temp;
            if ((!String.IsNullOrEmpty(QueryString["year1"])) && (Int32.TryParse(QueryString["year1"].ToUpper(), out temp)))
                year1 = temp;
            if ((!String.IsNullOrEmpty(QueryString["month1"])) && (Int32.TryParse(QueryString["month1"].ToUpper(), out temp)))
                month1 = temp;
            if ((!String.IsNullOrEmpty(QueryString["year2"])) && (Int32.TryParse(QueryString["year2"].ToUpper(), out temp)))
                year2 = temp;
            if ((!String.IsNullOrEmpty(QueryString["month2"])) && (Int32.TryParse(QueryString["month2"].ToUpper(), out temp)))
                month2 = temp;

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report_JDataTable", "Report will be from " + year1 + "/" + month1 + " and " + year2 + "/" + month2);

            // Get the dataset of results
            DataSet pages = get_global_usage_report_dataset(year1, month1, year2, month2, tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull usage report from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the view for sorting and filtering
            DataView resultsView = new DataView(pages.Tables[0]);

            // Should a filter be applied?
            if (!String.IsNullOrEmpty(QueryString["l1"]))
            {
                string level1_filter = QueryString["l1"];
                if (!String.IsNullOrEmpty(QueryString["l2"]))
                {
                    string level2_filter = QueryString["l2"];
                    if (!String.IsNullOrEmpty(QueryString["l3"]))
                    {
                        string level3_filter = QueryString["l3"];
                        if (!String.IsNullOrEmpty(QueryString["l4"]))
                        {
                            string level4_filter = QueryString["l4"];
                            if (!String.IsNullOrEmpty(QueryString["l5"]))
                            {
                                string level5_filter = QueryString["l5"];
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "' and Level5='" + level5_filter + "'";
                            }
                            else
                            {
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "'";
                            }
                        }
                        else
                        {
                            resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "'";
                        }
                    }
                    else
                    {
                        resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
                    }
                }
                else
                {
                    resultsView.RowFilter = "Level1='" + level1_filter + "'";
                }
            }

            // Get the count of results
            int total_results = resultsView.Count;

            // If this was set to show ALL results, set some page/length information
            if (displayLength == -1)
            {
                displayStart = 0;
                displayLength = total_results;
            }

            // Start the JSON response
            Response.Output.WriteLine("{");
            Response.Output.WriteLine("\"sEcho\": " + sEcho + ",");
            Response.Output.WriteLine("\"iTotalRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"iTotalDisplayRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"aaData\": [");

            // Sort by the correct column
                if (sortingColumn1 == 0)
                {
                    // Must be descending column zero then
                    resultsView.Sort = "Level1 " + sortDirection1 + ", Level2 " + sortDirection1 + ", Level3 " + sortDirection1 + ", Level4 " + sortDirection1 + ", Level5 " + sortDirection1 + ", Level6 " + sortDirection1 + ", Level7 " + sortDirection1 + ", Level8 " + sortDirection1;
                }
                else if (sortingColumn1 == 1)
                {
                    resultsView.Sort = "Title " + sortDirection1;
                }
                else if (sortingColumn1 == 2)
                {
                    resultsView.Sort = "Hits " + sortDirection1;
                }
                else if (sortingColumn1 == 3)
                {
                    resultsView.Sort = "HitsHierarchical " + sortDirection1;
                }

            // Add the data for the rows to show
            for (int i = displayStart; (i < displayStart + displayLength) && (i < total_results); i++)
            {
                // Start the JSON response for this row
                DataRow thisRow = resultsView[i].Row;

                Response.Output.Write("[ \"" + thisRow["Level1"]);
                if ((thisRow["Level2"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level2"].ToString())))
                {
                    Response.Output.Write("/" + thisRow["Level2"]);
                    if ((thisRow["Level3"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level3"].ToString())))
                    {
                        Response.Output.Write("/" + thisRow["Level3"]);
                        if ((thisRow["Level4"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level4"].ToString())))
                        {
                            Response.Output.Write("/" + thisRow["Level4"]);
                            if ((thisRow["Level5"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level5"].ToString())))
                            {
                                Response.Output.Write("/" + thisRow["Level5"]);
                                if ((thisRow["Level6"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level6"].ToString())))
                                {
                                    Response.Output.Write("/" + thisRow["Level6"]);
                                    if ((thisRow["Level7"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level7"].ToString())))
                                    {
                                        Response.Output.Write("/" + thisRow["Level7"]);
                                        if ((thisRow["Level8"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level8"].ToString())))
                                        {
                                            Response.Output.Write("/" + thisRow["Level8"]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Response.Output.Write("\", \"" + thisRow["Title"] + "\", " + thisRow["Hits"] + ", " + thisRow["HitsHierarchical"] );

                // Finish this row
                if ((i < displayStart + displayLength - 1) && (i < total_results - 1))
                    Response.Output.WriteLine(" ],");
                else
                    Response.Output.WriteLine(" ]");
            }

            Response.Output.WriteLine("]");
            Response.Output.WriteLine("}");
        }

        /// <summary> Gets the list of possible next level from an existing used page in a global usage report </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Global_Usage_Report_NextLevel(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report_NextLevel", "Into endpoint code");

            // Determine the range
            int year1 = 2000;
            int month1 = 1;
            int year2 = DateTime.Now.Year + 1;
            int month2 = 1;
            int temp;
            if ((!String.IsNullOrEmpty(QueryString["year1"])) && (Int32.TryParse(QueryString["year1"].ToUpper(), out temp)))
                year1 = temp;
            if ((!String.IsNullOrEmpty(QueryString["month1"])) && (Int32.TryParse(QueryString["month1"].ToUpper(), out temp)))
                month1 = temp;
            if ((!String.IsNullOrEmpty(QueryString["year2"])) && (Int32.TryParse(QueryString["year2"].ToUpper(), out temp)))
                year2 = temp;
            if ((!String.IsNullOrEmpty(QueryString["month2"])) && (Int32.TryParse(QueryString["month2"].ToUpper(), out temp)))
                month2 = temp;

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report_NextLevel", "Report will be from " + year1 + "/" + month1 + " and " + year2 + "/" + month2);

            // Get the dataset of results
            DataSet pages = get_global_usage_report_dataset(year1, month1, year2, month2, tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull usage report from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Start the return list
            List<string> returnValue = new List<string>();

            try
            {

                // There are two special cases that we are already prepared for from the database.
                //    - The very top level
                //    - The second level
                if (UrlSegments.Count <= 1)
                {
                    // One of the two special cases
                    if (UrlSegments.Count == 0)
                    {
                        tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report_NextLevel", "Special case, top level");
                        foreach (DataRow thisRow in pages.Tables[1].Rows)
                        {
                            returnValue.Add(thisRow[0].ToString().ToLower());
                        }
                    }
                    else
                    {
                        tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report_NextLevel", "Special case, second level");
                        string level1_special = UrlSegments[0];
                        DataView specialView = new DataView(pages.Tables[2])
                        {
                            RowFilter = "Level1 = '" + level1_special + "'"
                        };

                        foreach (DataRowView thisRow in specialView)
                        {
                            returnValue.Add(thisRow[1].ToString().ToLower());
                        }
                    }
                }
                else
                {
                    tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report_NextLevel", "Deeper, non-special case.  Preparing to scan list.");
                    string level1 = UrlSegments[0];
                    string level2 = UrlSegments[1];

                    // Build the filter
                    StringBuilder filterBuilder = new StringBuilder("Level1='" + level1 + "' and Level2='" + level2 + "'");
                    int column_counter = 3;
                    if ((UrlSegments.Count > 2) && (!String.IsNullOrWhiteSpace(UrlSegments[2])))
                    {
                        filterBuilder.Append(" and Level3='" + UrlSegments[2] + "'");
                        column_counter++;

                        if ((UrlSegments.Count > 3) && (!String.IsNullOrWhiteSpace(UrlSegments[3])))
                        {
                            filterBuilder.Append(" and Level4='" + UrlSegments[3] + "'");
                            column_counter++;

                            if ((UrlSegments.Count > 4) && (!String.IsNullOrWhiteSpace(UrlSegments[4])))
                            {
                                filterBuilder.Append(" and Level5='" + UrlSegments[4] + "'");
                                column_counter++;

                                if ((UrlSegments.Count > 5) && (!String.IsNullOrWhiteSpace(UrlSegments[5])))
                                {
                                    filterBuilder.Append(" and Level6='" + UrlSegments[5] + "'");
                                    column_counter++;

                                    if ((UrlSegments.Count > 6) && (!String.IsNullOrWhiteSpace(UrlSegments[6])))
                                    {
                                        filterBuilder.Append(" and Level7='" + UrlSegments[6] + "'");
                                        column_counter++;
                                    }
                                }
                            }
                        }
                    }

                    tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report_NextLevel", filterBuilder.ToString());

                    // Create the dataview
                    DataView specialView = new DataView(pages.Tables[0])
                    {
                        RowFilter = filterBuilder.ToString()
                    };

                    // Step through and add each NEW term to a sorted dictionary
                    SortedDictionary<string, string> sortList = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!sortList.ContainsKey(thisTerm)))
                                sortList[thisTerm.ToLower()] = thisTerm;
                        }
                    }

                    // Now, copy the keys over to the return value
                    returnValue.AddRange(sortList.Select(Kvp => Kvp.Key));
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error encountered determing next level");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.StackTrace);

                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_Global_Usage_Report_NextLevel", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseGlobalUsageReportNextLevel";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the full data set of all top-level static pages (excluding redirects) </summary>
        /// <param name="Month2"></param>
        /// <param name="Tracer"> Custom tracer </param>
        /// <param name="Year1"></param>
        /// <param name="Month1"></param>
        /// <param name="Year2"></param>
        /// <returns></returns>
        private DataSet get_global_usage_report_dataset(int Year1, int Month1, int Year2, int Month2, Custom_Tracer Tracer)
        {
            // Look in the cache first
            DataSet fromCache = CachedDataManager.WebContent.Retrieve_Global_Usage_Report(Year1, Month1, Year2, Month2, Tracer);
            if (fromCache != null)
                return fromCache;

            // Try to pull from the database
            DataSet fromDb = Engine_Database.WebContent_Get_Usage_Report(Year1, Month1, Year2, Month2, Tracer);

            // Store in the cache if not null
            if (fromDb != null)
            {
                CachedDataManager.WebContent.Store_Global_Usage_Report(fromDb, Year1, Month1, Year2, Month2, Tracer);
            }

            return fromDb;
        }

        #endregion

        #region Endpoints related to the complete list of global redirects

        /// <summary> Returns a flag indicating if there are any global redirects within the web content system </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        /// <remarks> This is not the most efficient way to verify existence of the rediects since this actually pulls the entire list of 
        /// redirects from the database.  However, this will likely be cached and be needed immediately anyway. </remarks>
        public void Has_Redirects(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Create the tracer and add a trace
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Has_Redirects");

            // Get the dataset of redirects
            DataSet changes = get_all_redirects(tracer);

            // If null was returned, an error occurred
            if (changes == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull global redirects from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Were there redirects?
            bool returnValue = changes.Tables[0].Rows.Count > 0;
            tracer.Add_Trace("WebContentServices.Has_Redirects", "Will return value " + returnValue.ToString().ToUpper());


            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Has_Redirects", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseHasRedirects";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the list of all the global redirects </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Redirects(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Set the default page information
            int page = 1;
            int rows_per_page = 100;

            // Check for rows_per_page from the query string
            if (!String.IsNullOrEmpty(QueryString["rowsPerPage"]))
            {
                if (!Int32.TryParse(QueryString["rowsPerPage"], out rows_per_page))
                    rows_per_page = 100;
                else if (rows_per_page < 1)
                    rows_per_page = 100;
            }

            // Look for the page number
            if (UrlSegments.Count > 0)
            {
                if (!Int32.TryParse(UrlSegments[0], out page))
                    page = 1;
                else if (page < 1)
                    page = 1;
            }

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_All_Redirects", "Get list of global redirects, page " + page + " with " + rows_per_page + " rows per page");

            // Get the dataset of redirects
            DataSet pages = get_all_redirects(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull pages list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the list for the results
            WebContent_Basic_Pages returnValue = new WebContent_Basic_Pages
            {
                Page = page,
                RowsPerPage = rows_per_page
            };

            // Prepare to step through the rows requested and convert them for returning
            int row_counter = (page - 1) * rows_per_page;
            int final_row_counter = page * rows_per_page;


            try
            {
                // Set the total number of changes 
                returnValue.Total = pages.Tables[0].Rows.Count;

                // Add the changes within the page requested
                while ((row_counter < pages.Tables[0].Rows.Count) && (row_counter < final_row_counter))
                {
                    returnValue.ContentCollection.Add(datarow_to_basic_info(pages.Tables[0].Rows[row_counter]));
                    row_counter++;
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error converting rows to return objects");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine(ee.StackTrace);
                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                tracer.Add_Trace("WebContentServices.Get_All_Redirects", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllRedirects";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the list of all the global redirects for consumption by the jQuery DataTable.net plug-in </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Redirects_JDataTable(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Get ready to pull the informaiton from the query string which the
            // jquery datatables library pass in
            int displayStart;
            int displayLength;
            int sortingColumn1;
            string sortDirection1 = "asc";

            // Get the display start and length from the DataTables generated data URL
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayStart"], out displayStart)) displayStart = 0;
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayLength"], out displayLength)) displayLength = 50;

            // Get the echo value
            string sEcho = HttpContext.Current.Request.QueryString["sEcho"];

            // Get the sorting column and sorting direction
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iSortCol_0"], out sortingColumn1)) sortingColumn1 = 0;
            if ((HttpContext.Current.Request.QueryString["sSortDir_0"] != null) && (HttpContext.Current.Request.QueryString["sSortDir_0"] == "desc"))
                sortDirection1 = "desc";

            // Get the dataset of pages
            DataSet pages = get_all_redirects(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull pages list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the view for sorting and filtering
            DataView resultsView = new DataView(pages.Tables[0]);

            // Should a filter be applied?
            if (!String.IsNullOrEmpty(QueryString["l1"]))
            {
                string level1_filter = QueryString["l1"];
                if (!String.IsNullOrEmpty(QueryString["l2"]))
                {
                    string level2_filter = QueryString["l2"];
                    if (!String.IsNullOrEmpty(QueryString["l3"]))
                    {
                        string level3_filter = QueryString["l3"];
                        if (!String.IsNullOrEmpty(QueryString["l4"]))
                        {
                            string level4_filter = QueryString["l4"];
                            if (!String.IsNullOrEmpty(QueryString["l5"]))
                            {
                                string level5_filter = QueryString["l5"];
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "' and Level5='" + level5_filter + "'";
                            }
                            else
                            {
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "'";
                            }
                        }
                        else
                        {
                            resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "'";
                        }
                    }
                    else
                    {
                        resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
                    }
                }
                else
                {
                    resultsView.RowFilter = "Level1='" + level1_filter + "'";
                }
            }

            // Get the count of results
            int total_results = resultsView.Count;

            // If this was set to show ALL results, set some page/length information
            if (displayLength == -1)
            {
                displayStart = 0;
                displayLength = total_results;
            }

            // Start the JSON response
            Response.Output.WriteLine("{");
            Response.Output.WriteLine("\"sEcho\": " + sEcho + ",");
            Response.Output.WriteLine("\"iTotalRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"iTotalDisplayRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"aaData\": [");

            // Sort by the correct column
            if ((sortingColumn1 > 0) || (sortDirection1 != "asc"))
            {
                if (sortingColumn1 == 0)
                {
                    // Must be descending column zero then
                    resultsView.Sort = "Level1 desc, Level2 desc, Level3 desc, Level4 desc, Level5 desc, Level6 desc, Level7 desc, Level8 desc";
                }
                else if (sortingColumn1 == 1)
                {
                    resultsView.Sort = "Redirect " + sortDirection1;
                }
            }

            // Add the data for the rows to show
            for (int i = displayStart; (i < displayStart + displayLength) && (i < total_results); i++)
            {
                // Start the JSON response for this row
                DataRow thisRow = resultsView[i].Row;
                Response.Output.Write("[ " + thisRow["WebContentID"] + ", ");

                Response.Output.Write("\"" + thisRow["Level1"]);
                if ((thisRow["Level2"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level2"].ToString())))
                {
                    Response.Output.Write("/" + thisRow["Level2"]);
                    if ((thisRow["Level3"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level3"].ToString())))
                    {
                        Response.Output.Write("/" + thisRow["Level3"]);
                        if ((thisRow["Level4"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level4"].ToString())))
                        {
                            Response.Output.Write("/" + thisRow["Level4"]);
                            if ((thisRow["Level5"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level5"].ToString())))
                            {
                                Response.Output.Write("/" + thisRow["Level5"]);
                                if ((thisRow["Level6"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level6"].ToString())))
                                {
                                    Response.Output.Write("/" + thisRow["Level6"]);
                                    if ((thisRow["Level7"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level7"].ToString())))
                                    {
                                        Response.Output.Write("/" + thisRow["Level7"]);
                                        if ((thisRow["Level8"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level8"].ToString())))
                                        {
                                            Response.Output.Write("/" + thisRow["Level8"]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Response.Output.Write("\", \"" + thisRow["Redirect"] + "\"");

                // Finish this row
                if ((i < displayStart + displayLength - 1) && (i < total_results - 1))
                    Response.Output.WriteLine("],");
                else
                    Response.Output.WriteLine("]");
            }

            Response.Output.WriteLine("]");
            Response.Output.WriteLine("}");
        }

        /// <summary> Gets the list of possible next level from an existing point in the redirects hierarchy </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Redirects_NextLevel(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Get_All_Redirects_NextLevel", "Into endpoint code");

            // Get the dataset of pages
            DataSet pages = get_all_redirects(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull redirects list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Start the return list
            List<string> returnValue = new List<string>();

            try
            {

                // There are two special cases that we are already prepared for from the database.
                //    - The very top level
                //    - The second level
                if (UrlSegments.Count <= 1)
                {
                    // One of the two special cases
                    if (UrlSegments.Count == 0)
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_Redirects_NextLevel", "Special case, top level");
                        foreach (DataRow thisRow in pages.Tables[1].Rows)
                        {
                            returnValue.Add(thisRow[0].ToString().ToLower());
                        }
                    }
                    else
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_Redirects_NextLevel", "Special case, second level");
                        string level1_special = UrlSegments[0];
                        DataView specialView = new DataView(pages.Tables[2])
                        {
                            RowFilter = "Level1 = '" + level1_special + "'"
                        };

                        foreach (DataRowView thisRow in specialView)
                        {
                            returnValue.Add(thisRow[1].ToString().ToLower());
                        }
                    }
                }
                else
                {
                    tracer.Add_Trace("WebContentServices.Get_All_Redirects_NextLevel", "Deeper, non-special case.  Preparing to scan list.");
                    string level1 = UrlSegments[0];
                    string level2 = UrlSegments[1];

                    // Build the filter
                    StringBuilder filterBuilder = new StringBuilder("Level1='" + level1 + "' and Level2='" + level2 + "'");
                    int column_counter = 3;
                    if ((UrlSegments.Count > 2) && (!String.IsNullOrWhiteSpace(UrlSegments[2])))
                    {
                        filterBuilder.Append(" and Level3='" + UrlSegments[2] + "'");
                        column_counter++;

                        if ((UrlSegments.Count > 3) && (!String.IsNullOrWhiteSpace(UrlSegments[3])))
                        {
                            filterBuilder.Append(" and Level4='" + UrlSegments[3] + "'");
                            column_counter++;

                            if ((UrlSegments.Count > 4) && (!String.IsNullOrWhiteSpace(UrlSegments[4])))
                            {
                                filterBuilder.Append(" and Level5='" + UrlSegments[4] + "'");
                                column_counter++;

                                if ((UrlSegments.Count > 5) && (!String.IsNullOrWhiteSpace(UrlSegments[5])))
                                {
                                    filterBuilder.Append(" and Level6='" + UrlSegments[5] + "'");
                                    column_counter++;

                                    if ((UrlSegments.Count > 6) && (!String.IsNullOrWhiteSpace(UrlSegments[6])))
                                    {
                                        filterBuilder.Append(" and Level7='" + UrlSegments[6] + "'");
                                        column_counter++;
                                    }
                                }
                            }
                        }
                    }

                    tracer.Add_Trace("WebContentServices.Get_All_Redirects_NextLevel", filterBuilder.ToString());

                    // Create the dataview
                    DataView specialView = new DataView(pages.Tables[0])
                    {
                        RowFilter = filterBuilder.ToString()
                    };

                    // Step through and add each NEW term to a sorted dictionary
                    SortedDictionary<string, string> sortList = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!sortList.ContainsKey(thisTerm)))
                                sortList[thisTerm.ToLower()] = thisTerm;
                        }
                    }

                    // Now, copy the keys over to the return value
                    returnValue.AddRange(sortList.Select(Kvp => Kvp.Key));
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error encountered determing next level");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.StackTrace);

                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_All_Redirects_NextLevel", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllRedirectsNextLevel";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the full data set of all global redirects </summary>
        /// <param name="Tracer"> Custom tracer </param>
        /// <returns></returns>
        private DataSet get_all_redirects(Custom_Tracer Tracer)
        {
            // Look in the cache first
            DataSet fromCache = CachedDataManager.WebContent.Retrieve_Redirects(Tracer);
            if (fromCache != null)
                return fromCache;

            // Try to pull from the database
            DataSet fromDb = Engine_Database.WebContent_Get_All_Redirects(Tracer);

            // Store in the cache if not null
            if (fromDb != null)
            {
                CachedDataManager.WebContent.Store_Redirects(fromDb, Tracer);
            }

            return fromDb;
        }

        #endregion

        #region Endpoints related to the complete list of web content pages (excluding redirects)

        /// <summary> Returns a flag indicating if there are any web content pages (excluding redirects) </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        /// <remarks> This is not the most efficient way to verify existence of the pages since this actually pulls the entire list of 
        /// pages from the database.  However, this will likely be cached and be needed immediately anyway. </remarks>
        public void Has_Content_Pages(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Create the tracer and add a trace
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Has_Content_Pages");

            // Get the dataset of web pages
            DataSet changes = get_all_content_pages(tracer);

            // If null was returned, an error occurred
            if (changes == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull web content pages from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Were there content pages?
            bool returnValue = changes.Tables[0].Rows.Count > 0;
            tracer.Add_Trace("WebContentServices.Has_Content_Pages", "Will return value " + returnValue.ToString().ToUpper());


            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Has_Content_Pages", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseHasPages";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the list of all the web content pages ( excluding redirects ) </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Pages(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Set the default page information
            int page = 1;
            int rows_per_page = 100;

            // Check for rows_per_page from the query string
            if (!String.IsNullOrEmpty(QueryString["rowsPerPage"]))
            {
                if (!Int32.TryParse(QueryString["rowsPerPage"], out rows_per_page))
                    rows_per_page = 100;
                else if (rows_per_page < 1)
                    rows_per_page = 100;
            }

            // Look for the page number
            if (UrlSegments.Count > 0)
            {
                if (!Int32.TryParse(UrlSegments[0], out page))
                    page = 1;
                else if (page < 1)
                    page = 1;
            }

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_All_Pages", "Get list of web content pages, page " + page + " with " + rows_per_page + " rows per page");

            // Get the dataset of pages
            DataSet pages = get_all_content_pages(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull pages list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the list for the results
            WebContent_Basic_Pages returnValue = new WebContent_Basic_Pages
            {
                Page = page,
                RowsPerPage = rows_per_page
            };

            // Prepare to step through the rows requested and convert them for returning
            int row_counter = (page - 1) * rows_per_page;
            int final_row_counter = page * rows_per_page;


            try
            {
                // Set the total number of changes 
                returnValue.Total = pages.Tables[0].Rows.Count;

                // Add the changes within the page requested
                while ((row_counter < pages.Tables[0].Rows.Count) && (row_counter < final_row_counter))
                {
                    returnValue.ContentCollection.Add(datarow_to_basic_info(pages.Tables[0].Rows[row_counter]));
                    row_counter++;
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error converting rows to return objects");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine(ee.StackTrace);
                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                tracer.Add_Trace("WebContentServices.Get_All_Pages", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllWebContentPages";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }


        /// <summary> Get the list of all the web content pages ( excluding redirects ) for
        /// consumption by the jQuery DataTable.net plug-in </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Pages_JDataTable(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Get ready to pull the informaiton from the query string which the
            // jquery datatables library pass in
            int displayStart;
            int displayLength;
            int sortingColumn1;
            string sortDirection1 = "asc";

            // Get the display start and length from the DataTables generated data URL
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayStart"], out displayStart)) displayStart = 0;
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayLength"], out displayLength)) displayLength = 50;

            // Get the echo value
            string sEcho = HttpContext.Current.Request.QueryString["sEcho"];

            // Get the sorting column and sorting direction
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iSortCol_0"], out sortingColumn1)) sortingColumn1 = 0;
            if ((HttpContext.Current.Request.QueryString["sSortDir_0"] != null) && (HttpContext.Current.Request.QueryString["sSortDir_0"] == "desc"))
                sortDirection1 = "desc";

            // Get the dataset of pages
            DataSet pages = get_all_content_pages(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull pages list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the view for sorting and filtering
            DataView resultsView = new DataView(pages.Tables[0]);

            // Should a filter be applied?
            if (!String.IsNullOrEmpty(QueryString["l1"]))
            {
                string level1_filter = QueryString["l1"];
                if (!String.IsNullOrEmpty(QueryString["l2"]))
                {
                    string level2_filter = QueryString["l2"];
                    if (!String.IsNullOrEmpty(QueryString["l3"]))
                    {
                        string level3_filter = QueryString["l3"];
                        if (!String.IsNullOrEmpty(QueryString["l4"]))
                        {
                            string level4_filter = QueryString["l4"];
                            if (!String.IsNullOrEmpty(QueryString["l5"]))
                            {
                                string level5_filter = QueryString["l5"];
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "' and Level5='" + level5_filter + "'";
                            }
                            else
                            {
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "'";
                            }
                        }
                        else
                        {
                            resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "'";
                        }
                    }
                    else
                    {
                        resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
                    }
                }
                else
                {
                    resultsView.RowFilter = "Level1='" + level1_filter + "'";
                }
            }

            // Get the count of results
            int total_results = resultsView.Count;

            // If this was set to show ALL results, set some page/length information
            if (displayLength == -1)
            {
                displayStart = 0;
                displayLength = total_results;
            }

            // Start the JSON response
            Response.Output.WriteLine("{");
            Response.Output.WriteLine("\"sEcho\": " + sEcho + ",");
            Response.Output.WriteLine("\"iTotalRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"iTotalDisplayRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"aaData\": [");

            // Sort by the correct column
            if ((sortingColumn1 > 1) || ( sortDirection1 != "asc"))
            {
                if (sortingColumn1 == 1)
                {
                    // Must be descending column zero then
                    resultsView.Sort = "Level1 desc, Level2 desc, Level3 desc, Level4 desc, Level5 desc, Level6 desc, Level7 desc, Level8 desc";
                }
                else if ( sortingColumn1 == 2 )
                {
                    resultsView.Sort = "Title " + sortDirection1;
                }
            }

            // Add the data for the rows to show
            for (int i = displayStart; (i < displayStart + displayLength) && (i < total_results); i++)
            {
                // Start the JSON response for this row
                DataRow thisRow = resultsView[i].Row;
                Response.Output.Write("[ " + thisRow["WebContentID"] + ", ");

                Response.Output.Write("\"" + thisRow["Level1"]);
                if ((thisRow["Level2"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level2"].ToString())))
                {
                    Response.Output.Write("/" + thisRow["Level2"]);
                    if ((thisRow["Level3"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level3"].ToString())))
                    {
                        Response.Output.Write("/" + thisRow["Level3"]);
                        if ((thisRow["Level4"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level4"].ToString())))
                        {
                            Response.Output.Write("/" + thisRow["Level4"]);
                            if ((thisRow["Level5"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level5"].ToString())))
                            {
                                Response.Output.Write("/" + thisRow["Level5"]);
                                if ((thisRow["Level6"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level6"].ToString())))
                                {
                                    Response.Output.Write("/" + thisRow["Level6"]);
                                    if ((thisRow["Level7"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level7"].ToString())))
                                    {
                                        Response.Output.Write("/" + thisRow["Level7"]);
                                        if ((thisRow["Level8"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level8"].ToString())))
                                        {
                                            Response.Output.Write("/" + thisRow["Level8"]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Response.Output.Write("\", \"" + thisRow["Title"] + "\"");

                // Finish this row
                if ((i < displayStart + displayLength - 1) && (i < total_results - 1))
                    Response.Output.WriteLine("],");
                else
                    Response.Output.WriteLine("]");
            }

            Response.Output.WriteLine("]");
            Response.Output.WriteLine("}");
        }

        /// <summary> Gets the list of possible next level from an existing point in the page hierarchy </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Pages_NextLevel(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Get_All_Pages_NextLevel", "Into endpoint code");

            // Get the dataset of pages
            DataSet pages = get_all_content_pages(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull pages list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Start the return list
            List<string> returnValue = new List<string>();

            try
            {

                // There are two special cases that we are already prepared for from the database.
                //    - The very top level
                //    - The second level
                if (UrlSegments.Count <= 1)
                {
                    // One of the two special cases
                    if (UrlSegments.Count == 0)
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_Pages_NextLevel", "Special case, top level");
                        foreach (DataRow thisRow in pages.Tables[1].Rows)
                        {
                            returnValue.Add(thisRow[0].ToString().ToLower());
                        }
                    }
                    else
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_Pages_NextLevel", "Special case, second level");
                        string level1_special = UrlSegments[0];
                        DataView specialView = new DataView(pages.Tables[2])
                        {
                            RowFilter = "Level1 = '" + level1_special + "'"
                        };

                        foreach (DataRowView thisRow in specialView)
                        {
                            returnValue.Add(thisRow[1].ToString().ToLower());
                        }
                    }
                }
                else
                {
                    tracer.Add_Trace("WebContentServices.Get_All_Pages_NextLevel", "Deeper, non-special case.  Preparing to scan list.");
                    string level1 = UrlSegments[0];
                    string level2 = UrlSegments[1];

                    // Build the filter
                    StringBuilder filterBuilder = new StringBuilder("Level1='" + level1 + "' and Level2='" + level2 + "'");
                    int column_counter = 3;
                    if ((UrlSegments.Count > 2) && ( !String.IsNullOrWhiteSpace(UrlSegments[2])))
                    {
                        filterBuilder.Append(" and Level3='" + UrlSegments[2] + "'");
                        column_counter++;

                        if ((UrlSegments.Count > 3) && ( !String.IsNullOrWhiteSpace(UrlSegments[3])))
                        {
                            filterBuilder.Append(" and Level4='" + UrlSegments[3] + "'");
                            column_counter++;

                            if ((UrlSegments.Count > 4) && ( !String.IsNullOrWhiteSpace(UrlSegments[4])))
                            {
                                filterBuilder.Append(" and Level5='" + UrlSegments[4] + "'");
                                column_counter++;

                                if ((UrlSegments.Count > 5) && ( !String.IsNullOrWhiteSpace(UrlSegments[5])))
                                {
                                    filterBuilder.Append(" and Level6='" + UrlSegments[5] + "'");
                                    column_counter++;

                                    if ((UrlSegments.Count > 6) && (!String.IsNullOrWhiteSpace(UrlSegments[6])))
                                    {
                                        filterBuilder.Append(" and Level7='" + UrlSegments[6] + "'");
                                        column_counter++;
                                    }
                                }
                            }
                        }
                    }

                    tracer.Add_Trace("WebContentServices.Get_All_Pages_NextLevel", filterBuilder.ToString());

                    // Create the dataview
                    DataView specialView = new DataView(pages.Tables[0])
                    {
                        RowFilter = filterBuilder.ToString()
                    };

                    // Step through and add each NEW term to a sorted dictionary
                    SortedDictionary<string, string> sortList = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!sortList.ContainsKey(thisTerm)))
                                sortList[thisTerm.ToLower()] = thisTerm;
                        }
                    }

                    // Now, copy the keys over to the return value
                    returnValue.AddRange(sortList.Select(Kvp => Kvp.Key));
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error encountered determing next level");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if ( IsDebug )
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.StackTrace);

                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                tracer.Add_Trace("WebContentServices.Get_All_Pages_NextLevel", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllWebNextLevel";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        private WebContent_Basic_Info datarow_to_basic_info(DataRow ChangeRow)
        {
            // Start to buid the object to report this work
            WebContent_Basic_Info recentChange = new WebContent_Basic_Info
            {
                WebContentID = Int32.Parse(ChangeRow[0].ToString()),
                Level1 = ChangeRow[1].ToString()
            };

            // Grab the values out and assign them
            if ((ChangeRow[2] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[2].ToString())))
            {
                recentChange.Level2 = ChangeRow[2].ToString();
                if ((ChangeRow[3] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[3].ToString())))
                {
                    recentChange.Level3 = ChangeRow[3].ToString();
                    if ((ChangeRow[4] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[4].ToString())))
                    {
                        recentChange.Level4 = ChangeRow[4].ToString();
                        if ((ChangeRow[5] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[5].ToString())))
                        {
                            recentChange.Level5 = ChangeRow[5].ToString();
                            if ((ChangeRow[6] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[6].ToString())))
                            {
                                recentChange.Level6 = ChangeRow[6].ToString();
                                if ((ChangeRow[7] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[7].ToString())))
                                {
                                    recentChange.Level7 = ChangeRow[7].ToString();
                                    if ((ChangeRow[8] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[8].ToString())))
                                    {
                                        recentChange.Level8 = ChangeRow[8].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            recentChange.Title = ChangeRow[9].ToString();
            if (Boolean.Parse(ChangeRow[11].ToString()))
                recentChange.Deleted = true;
            if ((ChangeRow[12] != DBNull.Value) && (!String.IsNullOrEmpty(ChangeRow[12].ToString())))
                recentChange.Redirect = ChangeRow[12].ToString();
            return recentChange;
        }

        /// <summary> Get the full data set of all top-level static pages (excluding redirects) </summary>
        /// <param name="Tracer"> Custom tracer </param>
        /// <returns></returns>
        private DataSet get_all_content_pages(Custom_Tracer Tracer)
        {
            // Look in the cache first
            DataSet fromCache = CachedDataManager.WebContent.Retrieve_All_Web_Content_Pages(Tracer);
            if (fromCache != null)
                return fromCache;

            // Try to pull from the database
            DataSet fromDb = Engine_Database.WebContent_Get_All_Pages(Tracer);

            // Store in the cache if not null
            if (fromDb != null)
            {
                CachedDataManager.WebContent.Store_All_Web_Content_Pages(fromDb, Tracer);
            }

            return fromDb;
        }

        #endregion

        #region Endpoints related to the complete list of web content entities, including pages and redirects

        /// <summary> Returns a flag indicating if there are any web content entities, including pages and redirects </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        /// <remarks> This is not the most efficient way to verify existence of the pages and redirects since this actually 
        /// pulls the entire list of pages and redirects from the database.  However, this will likely be cached and be 
        /// needed immediately anyway. </remarks>
        public void Has_Pages_Or_Redirects(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Create the tracer and add a trace
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Has_Pages_Or_Redirects");

            // Get the dataset of web pages or redirects
            DataSet changes = get_all_content_entities(tracer);

            // If null was returned, an error occurred
            if (changes == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull web content pages from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Were there web pages or redirects?
            bool returnValue = changes.Tables[0].Rows.Count > 0;
            tracer.Add_Trace("WebContentServices.Has_Pages_Or_Redirects", "Will return value " + returnValue.ToString().ToUpper());


            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Has_Pages_Or_Redirects", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseHasPagesRedirects";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the list of all the web content entities, including pages and redirects </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Set the default page information
            int page = 1;
            int rows_per_page = 100;

            // Check for rows_per_page from the query string
            if (!String.IsNullOrEmpty(QueryString["rowsPerPage"]))
            {
                if (!Int32.TryParse(QueryString["rowsPerPage"], out rows_per_page))
                    rows_per_page = 100;
                else if (rows_per_page < 1)
                    rows_per_page = 100;
            }

            // Look for the page number
            if (UrlSegments.Count > 0)
            {
                if (!Int32.TryParse(UrlSegments[0], out page))
                    page = 1;
                else if (page < 1)
                    page = 1;
            }

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_All", "Get list of web content entities, page " + page + " with " + rows_per_page + " rows per page");

            // Get the dataset of pages
            DataSet pages = get_all_content_entities(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull entities list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the list for the results
            WebContent_Basic_Pages returnValue = new WebContent_Basic_Pages
            {
                Page = page,
                RowsPerPage = rows_per_page
            };

            // Prepare to step through the rows requested and convert them for returning
            int row_counter = (page - 1) * rows_per_page;
            int final_row_counter = page * rows_per_page;


            try
            {
                // Set the total number of changes 
                returnValue.Total = pages.Tables[0].Rows.Count;

                // Add the changes within the page requested
                while ((row_counter < pages.Tables[0].Rows.Count) && (row_counter < final_row_counter))
                {
                    returnValue.ContentCollection.Add(datarow_to_basic_info(pages.Tables[0].Rows[row_counter]));
                    row_counter++;
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error converting rows to return objects");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine(ee.StackTrace);
                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_All", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllWebContentPages";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }


        /// <summary> Get the list of all the web content entities, including pages and redirects, for
        /// consumption by the jQuery DataTable.net plug-in </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_JDataTable(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Get ready to pull the informaiton from the query string which the
            // jquery datatables library pass in
            int displayStart;
            int displayLength;
            int sortingColumn1;
            string sortDirection1 = "asc";

            // Get the display start and length from the DataTables generated data URL
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayStart"], out displayStart)) displayStart = 0;
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayLength"], out displayLength)) displayLength = 50;

            // Get the echo value
            string sEcho = HttpContext.Current.Request.QueryString["sEcho"];

            // Get the sorting column and sorting direction
            if (!Int32.TryParse(HttpContext.Current.Request.QueryString["iSortCol_0"], out sortingColumn1)) sortingColumn1 = 0;
            if ((HttpContext.Current.Request.QueryString["sSortDir_0"] != null) && (HttpContext.Current.Request.QueryString["sSortDir_0"] == "desc"))
                sortDirection1 = "desc";

            // Get the dataset of pages
            DataSet pages = get_all_content_entities(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull entities list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Create the view for sorting and filtering
            DataView resultsView = new DataView(pages.Tables[0]);

            // Should a filter be applied?
            if (!String.IsNullOrEmpty(QueryString["l1"]))
            {
                string level1_filter = QueryString["l1"];
                if (!String.IsNullOrEmpty(QueryString["l2"]))
                {
                    string level2_filter = QueryString["l2"];
                    if (!String.IsNullOrEmpty(QueryString["l3"]))
                    {
                        string level3_filter = QueryString["l3"];
                        if (!String.IsNullOrEmpty(QueryString["l4"]))
                        {
                            string level4_filter = QueryString["l4"];
                            if (!String.IsNullOrEmpty(QueryString["l5"]))
                            {
                                string level5_filter = QueryString["l5"];
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "' and Level5='" + level5_filter + "'";
                            }
                            else
                            {
                                resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "' and Level4='" + level4_filter + "'";
                            }
                        }
                        else
                        {
                            resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "' and Level3='" + level3_filter + "'";
                        }
                    }
                    else
                    {
                        resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
                    }
                }
                else
                {
                    resultsView.RowFilter = "Level1='" + level1_filter + "'";
                }
            }

            // Get the count of results
            int total_results = resultsView.Count;

            // If this was set to show ALL results, set some page/length information
            if (displayLength == -1)
            {
                displayStart = 0;
                displayLength = total_results;
            }

            // Start the JSON response
            Response.Output.WriteLine("{");
            Response.Output.WriteLine("\"sEcho\": " + sEcho + ",");
            Response.Output.WriteLine("\"iTotalRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"iTotalDisplayRecords\": \"" + total_results + "\",");
            Response.Output.WriteLine("\"aaData\": [");

            // Sort by the correct column
            if ((sortingColumn1 > 1) || (sortDirection1 != "asc"))
            {
                if (sortingColumn1 == 1)
                {
                    // Must be descending column zero then
                    resultsView.Sort = "Level1 desc, Level2 desc, Level3 desc, Level4 desc, Level5 desc, Level6 desc, Level7 desc, Level8 desc";
                }
                else if (sortingColumn1 == 2)
                {
                    resultsView.Sort = "Title " + sortDirection1;
                }
            }

            // Add the data for the rows to show
            for (int i = displayStart; (i < displayStart + displayLength) && (i < total_results); i++)
            {
                // Start the JSON response for this row
                DataRow thisRow = resultsView[i].Row;
                if (( thisRow["Redirect"] == DBNull.Value ) || ( thisRow["Redirect"].ToString().Length == 0 ))
                    Response.Output.Write("[ \"" + thisRow["WebContentID"] + "\", ");
                else
                    Response.Output.Write("[ \"" + thisRow["WebContentID"] + "R\", ");

                Response.Output.Write("\"" + thisRow["Level1"]);
                if ((thisRow["Level2"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level2"].ToString())))
                {
                    Response.Output.Write("/" + thisRow["Level2"]);
                    if ((thisRow["Level3"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level3"].ToString())))
                    {
                        Response.Output.Write("/" + thisRow["Level3"]);
                        if ((thisRow["Level4"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level4"].ToString())))
                        {
                            Response.Output.Write("/" + thisRow["Level4"]);
                            if ((thisRow["Level5"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level5"].ToString())))
                            {
                                Response.Output.Write("/" + thisRow["Level5"]);
                                if ((thisRow["Level6"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level6"].ToString())))
                                {
                                    Response.Output.Write("/" + thisRow["Level6"]);
                                    if ((thisRow["Level7"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level7"].ToString())))
                                    {
                                        Response.Output.Write("/" + thisRow["Level7"]);
                                        if ((thisRow["Level8"] != DBNull.Value) && (!String.IsNullOrEmpty(thisRow["Level8"].ToString())))
                                        {
                                            Response.Output.Write("/" + thisRow["Level8"]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Response.Output.Write("\", \"" + thisRow["Title"] + "\"");

                // Finish this row
                if ((i < displayStart + displayLength - 1) && (i < total_results - 1))
                    Response.Output.WriteLine("],");
                else
                    Response.Output.WriteLine("]");
            }

            Response.Output.WriteLine("]");
            Response.Output.WriteLine("}");
        }

        /// <summary> Gets the list of possible next level from an existing point in the pages AND redirects hierarchy </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_NextLevel(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("WebContentServices.Get_All_NextLevel", "Into endpoint code");

            // Get the dataset of pages
            DataSet pages = get_all_content_entities(tracer);

            // If null was returned, an error occurred
            if (pages == null)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Unable to pull entities list from the database");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                }
                return;
            }

            // Start the return list
            List<string> returnValue = new List<string>();

            try
            {

                // There are two special cases that we are already prepared for from the database.
                //    - The very top level
                //    - The second level
                if (UrlSegments.Count <= 1)
                {
                    // One of the two special cases
                    if (UrlSegments.Count == 0)
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_NextLevel", "Special case, top level");
                        foreach (DataRow thisRow in pages.Tables[1].Rows)
                        {
                            returnValue.Add(thisRow[0].ToString().ToLower());
                        }
                    }
                    else
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_NextLevel", "Special case, second level");
                        string level1_special = UrlSegments[0];
                        DataView specialView = new DataView(pages.Tables[2])
                        {
                            RowFilter = "Level1 = '" + level1_special + "'"
                        };

                        foreach (DataRowView thisRow in specialView)
                        {
                            returnValue.Add(thisRow[1].ToString().ToLower());
                        }
                    }
                }
                else
                {
                    tracer.Add_Trace("WebContentServices.Get_All_NextLevel", "Deeper, non-special case.  Preparing to scan list.");
                    string level1 = UrlSegments[0];
                    string level2 = UrlSegments[1];

                    // Build the filter
                    StringBuilder filterBuilder = new StringBuilder("Level1='" + level1 + "' and Level2='" + level2 + "'");
                    int column_counter = 3;
                    if ((UrlSegments.Count > 2) && (!String.IsNullOrWhiteSpace(UrlSegments[2])))
                    {
                        filterBuilder.Append(" and Level3='" + UrlSegments[2] + "'");
                        column_counter++;

                        if ((UrlSegments.Count > 3) && (!String.IsNullOrWhiteSpace(UrlSegments[3])))
                        {
                            filterBuilder.Append(" and Level4='" + UrlSegments[3] + "'");
                            column_counter++;

                            if ((UrlSegments.Count > 4) && (!String.IsNullOrWhiteSpace(UrlSegments[4])))
                            {
                                filterBuilder.Append(" and Level5='" + UrlSegments[4] + "'");
                                column_counter++;

                                if ((UrlSegments.Count > 5) && (!String.IsNullOrWhiteSpace(UrlSegments[5])))
                                {
                                    filterBuilder.Append(" and Level6='" + UrlSegments[5] + "'");
                                    column_counter++;

                                    if ((UrlSegments.Count > 6) && (!String.IsNullOrWhiteSpace(UrlSegments[6])))
                                    {
                                        filterBuilder.Append(" and Level7='" + UrlSegments[6] + "'");
                                        column_counter++;
                                    }
                                }
                            }
                        }
                    }

                    tracer.Add_Trace("WebContentServices.Get_All_NextLevel", filterBuilder.ToString());

                    // Create the dataview
                    DataView specialView = new DataView(pages.Tables[0])
                    {
                        RowFilter = filterBuilder.ToString()
                    };

                    // Step through and add each NEW term to a sorted dictionary
                    SortedDictionary<string, string> sortList = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!sortList.ContainsKey(thisTerm)))
                                sortList[thisTerm.ToLower()] = thisTerm;
                        }
                    }

                    // Now, copy the keys over to the return value
                    returnValue.AddRange(sortList.Select(Kvp => Kvp.Key));
                }
            }
            catch (Exception ee)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error encountered determing next level");
                Response.StatusCode = 500;

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.Output.WriteLine();
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.StackTrace);

                }
                return;
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_All_NextLevel", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllNextLevel";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Get the full data set of all top-level web content entities, including pages and redirects </summary>
        /// <param name="Tracer"> Custom tracer </param>
        /// <returns></returns>
        private DataSet get_all_content_entities(Custom_Tracer Tracer)
        {
            // Look in the cache first
            DataSet fromCache = CachedDataManager.WebContent.Retrieve_All_Web_Content(Tracer);
            if (fromCache != null)
                return fromCache;

            // Try to pull from the database
            DataSet fromDb = Engine_Database.WebContent_Get_All(Tracer);

            // Store in the cache if not null
            if (fromDb != null)
            {
                CachedDataManager.WebContent.Store_All_Web_Content(fromDb, Tracer);
            }

            return fromDb;
        }

        #endregion

        #endregion

        #region Methods related to the sitemaps

        /// <summary> Gets the list of all sitemaps in the system </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Sitemaps(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_All_Sitemaps", "Get the list of all sitemaps");

            // Start the return object and look in the cache first
            List<string> returnValue = CachedDataManager.WebContent.Retrieve_All_Sitemaps(tracer);
            if (returnValue != null)
            {
                tracer.Add_Trace("WebContentServices.Get_All_Sitemaps", "Found the list of sitemaps in the cache");
            }
            else
            {
                // Get the sitemaps directory
                string sitemap_directory = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location, "webcontent", "sitemaps");

                if ( IsDebug )
                    tracer.Add_Trace("WebContentServices.Get_All_Sitemaps", "Sitemap directory: " + sitemap_directory);

                // Create the return value
                returnValue = new List<string>();

                try
                {
                    // Get the sitemaps files from the directory
                    if (Directory.Exists(sitemap_directory))
                    {
                        string[] sitemapFiles = Directory.GetFiles(sitemap_directory, "*.sitemap");
                        foreach (string thisSitemap in sitemapFiles)
                        {
                            returnValue.Add(Path.GetFileName(thisSitemap).Replace(".sitemap", ""));
                        }
                    }
                }
                catch (Exception ee)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Exception pulling the list of sitemap files - " + ee.Message);
                    Response.Output.WriteLine("Sitemap Directory: " + sitemap_directory);
                    Response.StatusCode = 500;

                    // If this was debug mode, then write the tracer
                    if (IsDebug)
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_Sitemaps", "Exception caught " + ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                    }
                    return;
                }

                // Store back in the cache
                tracer.Add_Trace("WebContentServices.Get_All_Sitemaps", "Store sitemap list in the caceh");
                CachedDataManager.WebContent.Store_All_Sitemaps(returnValue, tracer);
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_All_Sitemaps", "Debug mode detected");
                
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllSitemaps";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        #endregion

        #region Methods related to the controlled javascript files

        /// <summary> Gets the list of all controlled javascript files in the system </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Controlled_Javascript(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_All_Controlled_Javascript", "Get the list of all controlled javascript files");

            // Start the return object and look in the cache first
            List<string> returnValue = CachedDataManager.WebContent.Retrieve_All_Controlled_Javascript(tracer);
            if (returnValue != null)
            {
                tracer.Add_Trace("WebContentServices.Get_All_Controlled_Javascript", "Found the list of controlled javascript files in the cache");
            }
            else
            {
                // Get the javascript directory
                string javascript_directory = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location, "webcontent", "javascript");

                if (IsDebug)
                    tracer.Add_Trace("WebContentServices.Get_All_Controlled_Javascript", "Javascript directory: " + javascript_directory);

                try
                {
                    // Get the javascript files from the directory
                    string[] sitemapFiles = Directory.GetFiles(javascript_directory, "*.js");
                    returnValue = new List<string>();
                    foreach (string thisSitemap in sitemapFiles)
                    {
                        returnValue.Add(Path.GetFileName(thisSitemap).Replace(".js", ""));
                    }
                }
                catch (Exception ee)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Exception pulling the list of controlled javascript files - " + ee.Message);
                    Response.Output.WriteLine("Javascript Directory: " + javascript_directory);
                    Response.StatusCode = 500;

                    // If this was debug mode, then write the tracer
                    if (IsDebug)
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_Controlled_Javascript", "Exception caught " + ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                    }
                    return;
                }

                // Store back in the cache
                tracer.Add_Trace("WebContentServices.Get_All_Controlled_Javascript", "Store controlled javascript files list in the caceh");
                CachedDataManager.WebContent.Store_All_Controlled_Javascript(returnValue, tracer);
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_All_Controlled_Javascript", "Debug mode detected");

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllJavascripts";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        #endregion

        #region Methods related to the controlled stylesheet files

        /// <summary> Gets the list of all controlled stylesheet files in the system </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_All_Controlled_Stylesheets(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("WebContentServices.Get_All_Controlled_Stylesheets", "Get the list of all controlled stylesheet files");

            // Start the return object and look in the cache first
            List<string> returnValue = CachedDataManager.WebContent.Retrieve_All_Controlled_Stylesheets(tracer);
            if (returnValue != null)
            {
                tracer.Add_Trace("WebContentServices.Get_All_Controlled_Stylesheets", "Found the list of controlled stylesheet files in the cache");
            }
            else
            {
                // Get the stylesheet directory
                string stylesheet_directory = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location, "webcontent", "css");

                if (IsDebug)
                    tracer.Add_Trace("WebContentServices.Get_All_Controlled_Stylesheets", "Stylesheet directory: " + stylesheet_directory);

                try
                {
                    // Get the stylesheet files from the directory
                    string[] sitemapFiles = Directory.GetFiles(stylesheet_directory, "*.js");
                    returnValue = new List<string>();
                    foreach (string thisSitemap in sitemapFiles)
                    {
                        returnValue.Add(Path.GetFileName(thisSitemap).Replace(".js", ""));
                    }
                }
                catch (Exception ee)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Exception pulling the list of controlled stylesheet files - " + ee.Message);
                    Response.Output.WriteLine("Stylesheet Directory: " + stylesheet_directory);
                    Response.StatusCode = 500;

                    // If this was debug mode, then write the tracer
                    if (IsDebug)
                    {
                        tracer.Add_Trace("WebContentServices.Get_All_Controlled_Stylesheets", "Exception caught " + ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine();
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                    }
                    return;
                }

                // Store back in the cache
                tracer.Add_Trace("WebContentServices.Get_All_Controlled_Stylesheets", "Store controlled stylesheet files list in the caceh");
                CachedDataManager.WebContent.Store_All_Controlled_Stylesheets(returnValue, tracer);
            }

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                tracer.Add_Trace("WebContentServices.Get_All_Controlled_Stylesheets", "Debug mode detected");

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseAllStylesheets";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        #endregion

        #region Helper methods (ultimately destined to be private)

        /// <summary> Helper method retrieves HTML web content </summary>
        /// <param name="BasicInfo"> Basic information from the database for this endpoint, including the URL segments </param>
        /// <param name="Tracer"></param>
        /// <param name="ErrorType"> Any error enocuntered during the process </param>
        /// <returns> Built HTML content object, or NULL </returns>
        private static HTML_Based_Content read_source_file(WebContent_Basic_Info BasicInfo, Custom_Tracer Tracer, out WebContentEndpointErrorEnum ErrorType)
        {
            // Set a default error message first
            ErrorType = WebContentEndpointErrorEnum.NONE;

            // Build the directory to look for the static content
            StringBuilder possibleInfoModeBuilder = new StringBuilder(BasicInfo.Level1);
            if (!String.IsNullOrEmpty(BasicInfo.Level2))
            {
                possibleInfoModeBuilder.Append(Path.DirectorySeparatorChar + BasicInfo.Level2);
                if (!String.IsNullOrEmpty(BasicInfo.Level3))
                {
                    possibleInfoModeBuilder.Append(Path.DirectorySeparatorChar + BasicInfo.Level3);
                    if (!String.IsNullOrEmpty(BasicInfo.Level4))
                    {
                        possibleInfoModeBuilder.Append(Path.DirectorySeparatorChar + BasicInfo.Level4);
                        if (!String.IsNullOrEmpty(BasicInfo.Level5))
                        {
                            possibleInfoModeBuilder.Append(Path.DirectorySeparatorChar + BasicInfo.Level5);
                            if (!String.IsNullOrEmpty(BasicInfo.Level6))
                            {
                                possibleInfoModeBuilder.Append(Path.DirectorySeparatorChar + BasicInfo.Level6);
                                if (!String.IsNullOrEmpty(BasicInfo.Level7))
                                {
                                    possibleInfoModeBuilder.Append(Path.DirectorySeparatorChar + BasicInfo.Level7);
                                    if (!String.IsNullOrEmpty(BasicInfo.Level8))
                                    {
                                        possibleInfoModeBuilder.Append(Path.DirectorySeparatorChar + BasicInfo.Level8);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            string possible_info_mode = possibleInfoModeBuilder.ToString().Replace("'", "").Replace("\"", "");
            string filename = possible_info_mode;
            string base_source = Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory + "design\\webcontent";
            string source = base_source;
            string found_source = null;

            if ((possible_info_mode.IndexOf("\\") > 0) || (possible_info_mode.IndexOf("/") > 0))
            {
                source = source + "\\" + possible_info_mode.Replace("/", "\\");
                string[] split = source.Split("\\".ToCharArray());
                filename = split[split.Length - 1];
                source = source.Substring(0, source.Length - filename.Length);
            }

            // Start to build the return value
            HTML_Based_Content simpleWebContent = null;

            // If the designated source exists, look for the files 
            if (Directory.Exists(source))
            {
                // This may point to the default html in the parent directory
                if ((Directory.Exists(source + "\\" + filename)) && (File.Exists(source + "\\" + filename + "\\default.html")))
                {
                    found_source = Path.Combine(source, filename, "default.html");
                    Tracer.Add_Trace("WebContentServices.get_html_content", "Found source file: " + found_source);
                }

                // If this was found, build it and return it
                if (!String.IsNullOrEmpty(found_source))
                {
                    simpleWebContent = HTML_Based_Content_Reader.Read_HTML_File(found_source, true, Tracer);
                }
            }

            // If NULL then this valid web content page has no source!
            if (simpleWebContent == null)
            {
                simpleWebContent = new HTML_Based_Content();
                if (!String.IsNullOrEmpty(found_source))
                {
                    Tracer.Add_Trace("WebContentServices.get_html_content", "Error reading source file");
                    Tracer.Add_Trace("WebContentServices.get_html_content", found_source);
                    simpleWebContent.Title = "EXCEPTION ENCOUNTERED";
                    simpleWebContent.Build_Exception = "Found web content file, but it was apparently corrupted.  Error reading file " + found_source;
                    ErrorType = WebContentEndpointErrorEnum.Error_Reading_File;
                    simpleWebContent.Content = "<strong>Error reading the existing source file.  It may be corrupt.</strong>";
                    simpleWebContent.ContentSource = simpleWebContent.Source;
                }
                else
                {
                    Tracer.Add_Trace("WebContentServices.get_html_content", "No valid source file found!");
                    Tracer.Add_Trace("WebContentServices.get_html_content", found_source);
                    simpleWebContent.Title = "EXCEPTION ENCOUNTERED";
                    simpleWebContent.Build_Exception = "No valid source file found!";
                    simpleWebContent.Content = "<strong>No valid source file found, despite the fact this is a valid web content page in the database!</strong>";
                    simpleWebContent.ContentSource = simpleWebContent.Source;
                    ErrorType = WebContentEndpointErrorEnum.No_File_Found;
                }
            }

            // Copy over the primary key and URL segments for this web content
            simpleWebContent.WebContentID = BasicInfo.WebContentID;
            simpleWebContent.Level1 = BasicInfo.Level1;
            simpleWebContent.Level2 = BasicInfo.Level2;
            simpleWebContent.Level3 = BasicInfo.Level3;
            simpleWebContent.Level4 = BasicInfo.Level4;
            simpleWebContent.Level5 = BasicInfo.Level5;
            simpleWebContent.Level6 = BasicInfo.Level6;
            simpleWebContent.Level7 = BasicInfo.Level7;
            simpleWebContent.Level8 = BasicInfo.Level8;
            if ((BasicInfo.Locked.HasValue) && (BasicInfo.Locked.Value)) simpleWebContent.Locked = true;

            // Now, check for any "server-side include" directorives in the source text
            int include_index = simpleWebContent.Content.IndexOf("<%INCLUDE");
            while ((include_index > 0) && (simpleWebContent.Content.IndexOf("%>", include_index, StringComparison.Ordinal) > 0))
            {
                int include_finish_index = simpleWebContent.Content.IndexOf("%>", include_index, StringComparison.Ordinal) + 2;
                string include_statement = simpleWebContent.Content.Substring(include_index, include_finish_index - include_index);
                string include_statement_upper = include_statement.ToUpper();
                int file_index = include_statement_upper.IndexOf("FILE");
                string filename_to_include = String.Empty;
                if (file_index > 0)
                {
                    // Pull out the possible file name
                    string possible_file_name = include_statement.Substring(file_index + 4);
                    int file_start = -1;
                    int file_end = -1;
                    int char_index = 0;

                    // Find the start of the file information
                    while ((file_start < 0) && (char_index < possible_file_name.Length))
                    {
                        if ((possible_file_name[char_index] != '"') && (possible_file_name[char_index] != '=') && (possible_file_name[char_index] != ' '))
                        {
                            file_start = char_index;
                        }
                        else
                        {
                            char_index++;
                        }
                    }

                    // Find the end of the file information
                    if (file_start >= 0)
                    {
                        char_index++;
                        while ((file_end < 0) && (char_index < possible_file_name.Length))
                        {
                            if ((possible_file_name[char_index] == '"') || (possible_file_name[char_index] == ' ') || (possible_file_name[char_index] == '%'))
                            {
                                file_end = char_index;
                            }
                            else
                            {
                                char_index++;
                            }
                        }
                    }

                    // Get the filename
                    if ((file_start > 0) && (file_end > 0))
                    {
                        filename_to_include = possible_file_name.Substring(file_start, file_end - file_start);
                    }
                }

                // Remove the include and either place in the text from the indicated file, 
                // or just remove
                if ((filename_to_include.Length > 0) && (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory + "design\\webcontent\\" + filename_to_include)))
                {
                    // Define the value for the include text
                    string include_text;

                    // Look in the cache for this
                    object returnValue = HttpContext.Current.Cache.Get("INCLUDE_" + filename_to_include);
                    if (returnValue != null)
                    {
                        include_text = returnValue.ToString();
                    }
                    else
                    {
                        try
                        {
                            // Pull from the file
                            StreamReader reader = new StreamReader(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory + "design\\webcontent\\" + filename_to_include);
                            include_text = reader.ReadToEnd();
                            reader.Close();

                            // Store on the cache for two minutes, if no indication not to
                            if (include_statement_upper.IndexOf("NOCACHE") < 0)
                                HttpContext.Current.Cache.Insert("INCLUDE_" + filename_to_include, include_text, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2));
                        }
                        catch (Exception)
                        {
                            include_text = "Unable to read the soruce file ( " + filename_to_include + " )";
                        }
                    }

                    // Replace the text with the include file
                    simpleWebContent.Content = simpleWebContent.Content.Replace(include_statement, include_text);
                    include_index = simpleWebContent.Content.IndexOf("<%INCLUDE", include_index + include_text.Length - 1, StringComparison.Ordinal);
                }
                else
                {
                    // No suitable name was found, or it doesn't exist so just remove the INCLUDE completely
                    simpleWebContent.Content = simpleWebContent.Content.Replace(include_statement, "");
                    include_index = simpleWebContent.Content.IndexOf("<%INCLUDE", include_index, StringComparison.Ordinal);
                }
            }

            // Now, return the web content object, with the text
            return simpleWebContent;
        }


        #endregion


    }
}
