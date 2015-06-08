using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Caching;
using Jil;
using ProtoBuf;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Microservices;

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Endpoint supports services related to the static web content, CMS functionality </summary>
    public class WebContentServices
    {
        public enum WebContentEndpointErrorEnum : byte
        {
            NONE,

            No_File_Found,
     
            Error_Reading_File
        }

        /// <summary> Get top-level web content, static HTML </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        public void Get_HTML_Based_Content(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            WebContentEndpointErrorEnum errorType;
            HTML_Based_Content returnValue = get_html_content(UrlSegments, out errorType);
            if (returnValue == null)
            {
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

            switch (Protocol)
            {
                case Microservice_Endpoint_Protocol_Enum.JSON:
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                    break;

                case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                    Serializer.Serialize(Response.OutputStream, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.JSON_P:
                    Response.Output.Write("parseCollectionStaticPage(");
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                    Response.Output.Write(");");
                    break;

                case Microservice_Endpoint_Protocol_Enum.XML:
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(returnValue.GetType());
                    x.Serialize(Response.Output, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.BINARY:
                    IFormatter binary = new BinaryFormatter();
                    binary.Serialize(Response.OutputStream, returnValue);
                    break;
            }
        }

        /// <summary> Helper method retrieves HTML web content </summary>
        /// <param name="UrlSegments"> URL segments </param>
        /// <param name="ErrorType"> Any error enocuntered during the process </param>
        /// <returns> Built HTML content object, or NULL </returns>
        public HTML_Based_Content get_html_content(List<string> UrlSegments, out WebContentEndpointErrorEnum ErrorType)
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
                    found_source = source + "\\" + filename + "\\default.html";
                }

                if (String.IsNullOrEmpty(found_source))
                {
                    string[] matching_file = Directory.GetFiles(source, filename + ".htm*");
                    if (matching_file.Length > 0)
                    {
                        found_source = matching_file[0];
                    }
                }

                // If this was found, build it and return it
                if (!String.IsNullOrEmpty(found_source))
                {
                    HTML_Based_Content simpleWebContent = HTML_Based_Content_Reader.Read_HTML_File(source, true, null);

                    if ((simpleWebContent == null) || (simpleWebContent.Content.Length == 0))
                    {
                        ErrorType = WebContentEndpointErrorEnum.Error_Reading_File;
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
            ErrorType = WebContentEndpointErrorEnum.No_File_Found;
            return null;
        }


    }
}
