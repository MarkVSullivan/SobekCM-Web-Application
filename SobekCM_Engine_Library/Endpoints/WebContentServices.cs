#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;
using Jil;
using ProtoBuf;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Tools;

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

        #region Methods related to a single top-level static HTML content page

        /// <summary> Get top-level web content, static HTML </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_HTML_Based_Content(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug )
        {
            Custom_Tracer tracer = new Custom_Tracer();
            WebContentEndpointErrorEnum errorType;
            HTML_Based_Content returnValue = get_html_content(UrlSegments, tracer, out errorType);
            if (returnValue == null)
            {
                // If this was debug mode, then just write the tracer
                if ( IsDebug )
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

            // If this was debug mode, then just write the tracer
            if ( IsDebug )
            {
                tracer.Add_Trace("WebContentServices.Get_HTML_Based_Content", "Debug mode detected");
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseCollectionStaticPage";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }


        /// <summary> Add a new HTML web content page </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <param name="RequestForm"></param>
        public void Add_HTML_Based_Content(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol, NameValueCollection RequestForm)
        {
            // Create the custom tracer
            Custom_Tracer tracer = new Custom_Tracer();

            // Validate the username is present
            if (String.IsNullOrEmpty(RequestForm["User"]))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Required posted 'User' (name of user) is missing.\"");
                Response.End();
                return;
            }

            // Get the username
            string user = RequestForm["User"];

            // Validate the new page information
            if (String.IsNullOrEmpty(RequestForm["PageInfo"]))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Required posted 'PageInfo' is missing.\"");
                Response.End();
                return;
            }

            // Get the page information and deserialize, according to the indicated protocol
            string pageInfoString = RequestForm["PageInfo"];
            WebContent_Basic_Info basicInfo = null;
            try
            {
                switch (Protocol)
                {
                    case Microservice_Endpoint_Protocol_Enum.JSON:
                        basicInfo = JSON.Deserialize<WebContent_Basic_Info>(pageInfoString);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.XML:
                        XmlSerializer x = new XmlSerializer(typeof(WebContent_Basic_Info));
                        using (TextReader reader = new StringReader(pageInfoString))
                        {
                            basicInfo = (WebContent_Basic_Info) x.Deserialize(reader);
                        }
                        break;

                    case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                        using (MemoryStream m = new MemoryStream(Encoding.Unicode.GetBytes(pageInfoString ?? "")))
                        {
                            basicInfo = Serializer.Deserialize<WebContent_Basic_Info>(m);
                        }
                        break;
                }
            }
            catch (Exception)
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Error deserializing 'PageInfo' into the Web_Content_Basic_Info object.\"");
                Response.End();
                return;
            }

            // Should not ever get here
            if (basicInfo == null)
                return;

            // Get the levels from the URL request
            string level1 = UrlSegments.Count > 0 ? UrlSegments[0] : null;
            string level2 = UrlSegments.Count > 1 ? UrlSegments[1] : null;
            string level3 = UrlSegments.Count > 2 ? UrlSegments[2] : null;
            string level4 = UrlSegments.Count > 3 ? UrlSegments[3] : null;
            string level5 = UrlSegments.Count > 4 ? UrlSegments[4] : null;
            string level6 = UrlSegments.Count > 5 ? UrlSegments[5] : null;
            string level7 = UrlSegments.Count > 6 ? UrlSegments[6] : null;
            string level8 = UrlSegments.Count > 7 ? UrlSegments[7] : null;

            // Ensure the web page does not already exist
            int newContentId = Engine_Database.WebContent_Add_Page(level1, level2, level3, level4, level5, level6, level7, level8, user, basicInfo.Title, basicInfo.Summary, basicInfo.Redirect, tracer);

            // If this is -1, then an error occurred
            if (newContentId < 0)
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Error adding the new web page.\"");
                Response.End();
                return;
            }

            // Assign the ID to the page
            basicInfo.WebContentID = newContentId;

            // Send back the result
            Response.StatusCode = 201;
            Serialize(basicInfo, Response, Protocol, "addHtmlBasedContent");
        }

        /// <summary> Add a milestone to an existing web content page </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <param name="RequestForm"></param>
        public void Add_WebContent_Milestone(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol, NameValueCollection RequestForm)
        {
            // Create the custom tracer
            Custom_Tracer tracer = new Custom_Tracer();

            // Validate the username is present
            if (String.IsNullOrEmpty(RequestForm["User"]))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Required posted 'User' (name of user) is missing.\"");
                Response.End();
                return;
            }

            // Get the username
            string user = RequestForm["User"];

            // Validate the new page information
            if (String.IsNullOrEmpty(RequestForm["WebContentID"]))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Required posted 'WebContentID' is missing.\"");
                Response.End();
                return;
            }

            // Get the webcontent id
            string id_as_string = RequestForm["WebContentID"];
            int id;
            if (!Int32.TryParse(id_as_string, out id))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Invalid data type for  'WebContentID'.  Expected integer.\"");
                Response.End();
                return;
            }

            // Validate the milestone is present
            if (String.IsNullOrEmpty(RequestForm["Milestone"]))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Required posted 'Milestone' is missing.\"");
                Response.End();
                return;
            }

            // Get the username
            string milestone = RequestForm["Milestone"];


            // Ensure the web page does not already exist
            if (Engine_Database.WebContent_Add_Milestone(id, milestone, user, tracer))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 200;
                Response.Output.WriteLine("\"OK\"");
                Response.End();
                return;
            }

            // AN error occurred
            Response.ContentType = "text/plain";
            Response.StatusCode = 500;
            Response.Output.WriteLine("\"Unknown error adding the milestone\"");
            Response.End();
        }

        /// <summary> Delete a web content page </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <param name="RequestForm"></param>
        public void Delete_WebContent_Page(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol, NameValueCollection RequestForm)
        {
            // Create the custom tracer
            Custom_Tracer tracer = new Custom_Tracer();

            // Validate the username is present
            if (String.IsNullOrEmpty(RequestForm["User"]))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Required posted 'User' (name of user) is missing.\"");
                Response.End();
                return;
            }

            // Get the username
            string user = RequestForm["User"];

            // Validate the new page information
            if (String.IsNullOrEmpty(RequestForm["WebContentID"]))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Required posted 'WebContentID' is missing.\"");
                Response.End();
                return;
            }

            // Get the webcontent id
            string id_as_string = RequestForm["WebContentID"];
            int id;
            if (!Int32.TryParse(id_as_string, out id))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Invalid data type for  'WebContentID'.  Expected integer.\"");
                Response.End();
                return;
            }

            // Validate the milestone is present
            if (String.IsNullOrEmpty(RequestForm["Reason"]))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 500;
                Response.Output.WriteLine("\"INVALID REQUEST: Required posted 'Reason' is missing.\"");
                Response.End();
                return;
            }

            // Get the username
            string milestone = RequestForm["Reason"];


            // Ensure the web page does not already exist
            if (Engine_Database.WebContent_Delete_Page(id, milestone, user, tracer))
            {
                Response.ContentType = "text/plain";
                Response.StatusCode = 200;
                Response.Output.WriteLine("\"OK\"");
                Response.End();
                return;
            }

            // AN error occurred
            Response.ContentType = "text/plain";
            Response.StatusCode = 500;
            Response.Output.WriteLine("\"Unknown error deleting the web content page\"");
            Response.End();
        }

        #endregion

        #region Endpoints related to overall management of the web content pages through the admin pages

        #region Endpoints related to global recent updates

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
            tracer.Add_Trace("WebContenServices.Get_Recent_Updates","Get recent updates page " + page + " with " + rows_per_page + " rows per page");

            // Check for a user filter
            string userFilter = QueryString["user"];
            if (!String.IsNullOrWhiteSpace(userFilter))
                tracer.Add_Trace("WebContenServices.Get_Recent_Updates", "Filter by user '" + userFilter + "'");
         
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

            // Should a filter be applied?
            if (!String.IsNullOrEmpty(QueryString["l1"]))
            {
                string level1_filter = QueryString["l1"];
                if (!String.IsNullOrEmpty(QueryString["l2"]))
                {
                    string level2_filter = QueryString["l2"];
                    resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
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


                Response.Output.Write("\", \"" + thisRow["Title"] + "\", ");
                Response.Output.Write("\", \"" + (DateTime.Parse(thisRow["MilestoneDate"].ToString())).ToString() + "\", ");
                Response.Output.Write("\", \"" + thisRow["MilestoneUser"] + "\", ");
                Response.Output.Write("\", \"" + thisRow["Milestone"] + "\" ");

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
                            returnValue.Add(thisRow[0].ToString());
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
                            returnValue.Add(thisRow[1].ToString());
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


                    // Step through and add each NEW term
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString().ToLower();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!returnValue.Contains(thisTerm)))
                                returnValue.Add(thisTerm);
                        }
                    }
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
            tracer.Add_Trace("WebContenServices.Get_Recent_Updates_Users", "Get the list of all users that have participated in the recent updates to all top-level static web content pages");

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
            tracer.Add_Trace("WebContenServices.Get_Global_Usage_Report", "Pull usage report page " + page + " with " + rows_per_page + " rows per page");

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
            tracer.Add_Trace("WebContenServices.Get_Global_Usage_Report", "Report will be from " + year1 + "/" + month1 + " and " + year2 + "/" + month2 );
            
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
            tracer.Add_Trace("WebContenServices.Get_Global_Usage_Report_JDataTable", "Report will be from " + year1 + "/" + month1 + " and " + year2 + "/" + month2);

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
                    resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
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
            tracer.Add_Trace("WebContenServices.Get_Global_Usage_Report_NextLevel", "Report will be from " + year1 + "/" + month1 + " and " + year2 + "/" + month2);

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
                            returnValue.Add(thisRow[0].ToString());
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
                            returnValue.Add(thisRow[1].ToString());
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


                    // Step through and add each NEW term
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString().ToLower();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!returnValue.Contains(thisTerm)))
                                returnValue.Add(thisTerm);
                        }
                    }
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
            tracer.Add_Trace("WebContenServices.Get_All_Redirects", "Get list of global redirects, page " + page + " with " + rows_per_page + " rows per page");

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
                    resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
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
                            returnValue.Add(thisRow[0].ToString());
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
                            returnValue.Add(thisRow[1].ToString());
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


                    // Step through and add each NEW term
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString().ToLower();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!returnValue.Contains(thisTerm)))
                                returnValue.Add(thisTerm);
                        }
                    }
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
            tracer.Add_Trace("WebContenServices.Get_All_Pages", "Get list of web content pages, page " + page + " with " + rows_per_page + " rows per page");

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
                    resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
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
                            returnValue.Add(thisRow[0].ToString());
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
                            returnValue.Add(thisRow[1].ToString());
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


                    // Step through and add each NEW term
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString().ToLower();
                            if (( !String.IsNullOrEmpty(thisTerm)) && (!returnValue.Contains(thisTerm)))
                                returnValue.Add(thisTerm);
                        }
                    }
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
            tracer.Add_Trace("WebContenServices.Get_All", "Get list of web content entities, page " + page + " with " + rows_per_page + " rows per page");

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
                    resultsView.RowFilter = "Level1='" + level1_filter + "' and Level2='" + level2_filter + "'";
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
                            returnValue.Add(thisRow[0].ToString());
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
                            returnValue.Add(thisRow[1].ToString());
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


                    // Step through and add each NEW term
                    foreach (DataRowView thisRow in specialView)
                    {
                        if (thisRow[column_counter] != DBNull.Value)
                        {
                            string thisTerm = thisRow[column_counter].ToString().ToLower();
                            if ((!String.IsNullOrEmpty(thisTerm)) && (!returnValue.Contains(thisTerm)))
                                returnValue.Add(thisTerm);
                        }
                    }
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

        #region Helper methods (ultimately destined to be private)

        /// <summary> Helper method retrieves HTML web content </summary>
        /// <param name="UrlSegments"> URL segments </param>
        /// <param name="Tracer"></param>
        /// <param name="ErrorType"> Any error enocuntered during the process </param>
        /// <returns> Built HTML content object, or NULL </returns>
        public static HTML_Based_Content get_html_content(List<string> UrlSegments, Custom_Tracer Tracer, out WebContentEndpointErrorEnum ErrorType)
        {
            // Set a default error message first
            ErrorType = WebContentEndpointErrorEnum.NONE;

            // Build the directory to look for the static content
            StringBuilder possibleInfoModeBuilder = new StringBuilder();
            if (UrlSegments.Count > 0)
            {
                possibleInfoModeBuilder.Append(UrlSegments[0]);
            }
            for (int i = 1; i < UrlSegments.Count; i++)
            {
                possibleInfoModeBuilder.Append("/" + UrlSegments[i]);
            }

            string possible_info_mode = possibleInfoModeBuilder.ToString().Replace("'", "").Replace("\"", "");
            string filename = possible_info_mode;
            string base_source = Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design\\webcontent";
            string source = base_source;
            string found_source = null;

            if ((possible_info_mode.IndexOf("\\") > 0) || (possible_info_mode.IndexOf("/") > 0))
            {
                source = source + "\\" + possible_info_mode.Replace("/", "\\");
                string[] split = source.Split("\\".ToCharArray());
                filename = split[split.Length - 1];
                source = source.Substring(0, source.Length - filename.Length);
            }

            // If the designated source exists, look for the files 
            if (Directory.Exists(source))
            {
                // This may point to the default html in the parent directory
                if ((Directory.Exists(source + "\\" + filename)) && (File.Exists(source + "\\" + filename + "\\default.html")))
                {
                    found_source = Path.Combine(source, filename, "default.html");
                    Tracer.Add_Trace("WebContentServices.get_html_content", "Found source file: " + found_source);
                }

                if (String.IsNullOrEmpty(found_source))
                {
                    string[] matching_file = Directory.GetFiles(source, filename + ".htm*");
                    if (matching_file.Length > 0)
                    {
                        found_source = matching_file[0];
                        Tracer.Add_Trace("WebContentServices.get_html_content", "Found source file: " + found_source);
                    }
                }

                // If this was found, build it and return it
                if (!String.IsNullOrEmpty(found_source))
                {
                    HTML_Based_Content simpleWebContent = HTML_Based_Content_Reader.Read_HTML_File(found_source, true, Tracer);

                    if ((simpleWebContent == null) || (simpleWebContent.Content.Length == 0))
                    {
                        ErrorType = WebContentEndpointErrorEnum.Error_Reading_File;
                        Tracer.Add_Trace("WebContentServices.get_html_content", "Error reading source file");
                        return null;
                    }

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
                        if ((filename_to_include.Length > 0) && (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design\\webcontent\\" + filename_to_include)))
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
                                    StreamReader reader = new StreamReader(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design\\webcontent\\" + filename_to_include);
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
            }

            // Was never found
            Tracer.Add_Trace("WebContentServices.get_html_content", "No source file found");
            ErrorType = WebContentEndpointErrorEnum.No_File_Found;
            return null;
        }

        #endregion


    }
}
