#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;
using Jil;
using ProtoBuf;
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

        /// <summary> Get top-level web content, static HTML </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        public void Get_HTML_Based_Content(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            WebContentEndpointErrorEnum errorType;
            HTML_Based_Content returnValue = get_html_content(UrlSegments, tracer, out errorType);
            if (returnValue == null)
            {
                // If this was debug mode, then just write the tracer
                if (QueryString["debug"] == "debug")
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
            if (QueryString["debug"] == "debug")
            {
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
            Web_Content_Basic_Info basicInfo = null;
            try
            {
                switch (Protocol)
                {
                    case Microservice_Endpoint_Protocol_Enum.JSON:
                        basicInfo = JSON.Deserialize<Web_Content_Basic_Info>(pageInfoString);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.XML:
                        XmlSerializer x = new XmlSerializer(typeof(Web_Content_Basic_Info));
                        using (TextReader reader = new StringReader(pageInfoString))
                        {
                            basicInfo = (Web_Content_Basic_Info) x.Deserialize(reader);
                        }
                        break;

                    case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                        using (MemoryStream m = new MemoryStream(Encoding.Unicode.GetBytes(pageInfoString ?? "")))
                        {
                            basicInfo = Serializer.Deserialize<Web_Content_Basic_Info>(m);
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
            int newContentId = Engine_Database.WebContent_Add_Page(level1, level2, level3, level4, level5, level6, level7, level8, user, basicInfo.Title, basicInfo.Summary, tracer);

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
