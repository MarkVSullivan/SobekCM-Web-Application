#region Using directives

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.WebContent
{
    /// <summary> Class is used to read the source browse and info static html files and create the
    /// appropriate <see cref="HTML_Based_Content"/> object with all the bibliographic information
    /// (author, keywords, description, title, code) loaded from the header in the HTML file </summary>
    public class HTML_Based_Content_Reader
    {
        /// <summary> Read the source browse and info static html files and create the
        /// appropriate <see cref="HTML_Based_Content"/> object with all the bibliographic information
        /// (author, keywords, description, title, code) loaded from the header in the HTML file</summary>
        /// <param name="Source_URL"> URL to the source HTML document, retrievable via the web</param>
        /// <param name="Retain_Entire_Display_Text"> Flag indicates whether the entire display text should be retained (as it is about to be displayed) or just the basic information from the HEAD of the file </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built browse info object with all the bibliographic information</returns>
        public static HTML_Based_Content Read_Web_Document(string Source_URL,  bool Retain_Entire_Display_Text, Custom_Tracer Tracer)
        {
            try
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("HTML_Based_Content_Reader.Read_Web_Document", "Reading source file via web response");
                }

                // the html retrieved from the page
                string displayText;
                WebRequest objRequest = WebRequest.Create(Source_URL);
                WebResponse objResponse = objRequest.GetResponse();

                // the using keyword will automatically dispose the object once complete
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    displayText = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }

                // Convert this to the object
                return Text_To_HTML_Based_Content(displayText, Retain_Entire_Display_Text, String.Empty);
            }
            catch
            {
                return null;
            }
        }

        /// <summary> Read the source browse and info static html files and create the
        /// appropriate <see cref="HTML_Based_Content"/> object with all the bibliographic information
        /// (author, keywords, description, title, code) loaded from the header in the HTML file</summary>
        /// <param name="Source_File"> Source file to read directly from the local network (not via the web)</param>
        /// <param name="Retain_Entire_Display_Text"> Flag indicates whether the entire display text should be retained (as it is about to be displayed) or just the basic information from the HEAD of the file </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built browse info object with all the bibliographic information</returns>
        public static HTML_Based_Content Read_HTML_File(string Source_File,  bool Retain_Entire_Display_Text, Custom_Tracer Tracer)
        {
            try
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("HTML_Based_Content_Reader.Read_HTML_File", "Reading source file");
                }

                // Read this info file 
                StreamReader reader = new StreamReader(Source_File);
                string displayText = reader.ReadToEnd();
                reader.Close();

                // Convert this to the object
                return Text_To_HTML_Based_Content(displayText, Retain_Entire_Display_Text, Source_File);
            }
            catch
            {
                return null;
            }
        }

        private static HTML_Based_Content Text_To_HTML_Based_Content(string Display_Text, bool Retain_Entire_Display_Text, string Source)
        {
            // Create the values to hold the information
            string code = String.Empty;
            string title = String.Empty;
            string author = String.Empty;
            string description = String.Empty;
            string thumbnail = String.Empty;
            string keyword = String.Empty;
            string banner = String.Empty;
            string date = String.Empty;
            string sitemap = String.Empty;
            string webskin = String.Empty;

            // StringBuilder keeps track of any other information in the head that should be retained
            StringBuilder headBuilder = new StringBuilder();

            HTML_Based_Content returnValue = new HTML_Based_Content();

            // Try to read the head using XML
            int head_start = Display_Text.IndexOf("<head>", StringComparison.OrdinalIgnoreCase);
            int head_end = Display_Text.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
            bool read_as_xml = false;
            if ((head_start >= 0) && (head_end > head_start))
            {
                try
                {
                    string head_xml = Display_Text.Substring(head_start, (head_end - head_start) + 7);
                    XmlTextReader xmlReader = new XmlTextReader(new StringReader(head_xml));
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType != XmlNodeType.Element) continue;

                        switch (xmlReader.Name.ToUpper())
                        {
                            case "LINK":
                                headBuilder.Append("<link ");
                                int attributeCount = xmlReader.AttributeCount;
                                for (int i = 0; i < attributeCount; i++)
                                {
                                    xmlReader.MoveToAttribute(i);
                                    headBuilder.Append(xmlReader.Name + "=\"" + xmlReader.Value + "\" ");
                                }
                                headBuilder.AppendLine(" />");
                                break;

                            case "TITLE":
                                xmlReader.Read();
                                title = xmlReader.Value;
                                break;

                            case "CODE":
                                xmlReader.Read();
                                code = xmlReader.Value.ToLower();
                                break;

                            case "META":
                                string name_type = String.Empty;
                                string content_type = String.Empty;
                                if (xmlReader.MoveToAttribute("name"))
                                {
                                    name_type = xmlReader.Value;
                                }
                                if (xmlReader.MoveToAttribute("content"))
                                {
                                    content_type = xmlReader.Value;
                                }
                                if ((name_type.Length > 0) && (content_type.Length > 0))
                                {
                                    switch (name_type.ToUpper())
                                    {
                                        case "BANNER":
                                            banner = content_type;
                                            break;

                                        case "TITLE":
                                            title = content_type;
                                            break;

                                        case "THUMBNAIL":
                                            thumbnail = content_type;
                                            break;

                                        case "AUTHOR":
                                            author = content_type;
                                            break;

                                        case "DATE":
                                            date = content_type;
                                            break;

                                        case "KEYWORDS":
                                            keyword = content_type;
                                            break;

                                        case "DESCRIPTION":
                                            description = content_type;
                                            break;

                                        case "CODE":
                                            code = content_type.ToLower();
                                            break;

                                        case "SITEMAP":
                                            sitemap = content_type;
                                            break;

                                        case "WEBSKIN":
                                            webskin = content_type;
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    read_as_xml = true;
                }
                catch
                {
                    read_as_xml = false;
                }


                // Read this the old way if unable to read via XML for some reason
                if (!read_as_xml)
                {
                    // Get the title and code

                    string header_info = Display_Text.Substring(0, Display_Text.IndexOf("<body>"));
                    if (header_info.IndexOf("<title>") > 0)
                    {
                        string possible_title = header_info.Substring(header_info.IndexOf("<title>"));
                        possible_title = possible_title.Substring(7, possible_title.IndexOf("</title>") - 7);
                        title = possible_title.Trim();
                    }
                    if (header_info.IndexOf("<code>") > 0)
                    {
                        string possible_code = header_info.Substring(header_info.IndexOf("<code>"));
                        possible_code = possible_code.Substring(6, possible_code.IndexOf("</code>") - 6);
                        code = possible_code.Trim();
                    }

                    // See if the banner should be displayed (default is only option righht now)
                    if (Display_Text.IndexOf("<meta name=\"banner\" content=\"default\"") > 0)
                    {
                        banner = "default";
                    }
                }

                // Create return value
                if (code.Length > 0)
                    returnValue.Code = code;
                if (title.Length > 0)
                    returnValue.Title = title;
                if (author.Length > 0)
                    returnValue.Author = author;
                if (banner.Length > 0)
                    returnValue.Banner = banner;
                if (date.Length > 0)
                    returnValue.Date = date;
                if (description.Length > 0)
                    returnValue.Description = description;
                if (keyword.Length > 0)
                    returnValue.Keywords = keyword;
                if (thumbnail.Length > 0)
                    returnValue.Thumbnail = thumbnail;
                if (sitemap.Length > 0)
                    returnValue.SiteMap = sitemap;
                if (webskin.Length > 0)
                    returnValue.Web_Skin = webskin;
                if (headBuilder.Length > 0)
                    returnValue.Extra_Head_Info = headBuilder.ToString();
            }

            // Should the actual display text be retained?
            if (Retain_Entire_Display_Text)
            {
                int start_body = Display_Text.IndexOf("<body>");
                int end_body = Display_Text.IndexOf("</body>");
                if ((start_body > 0) && (end_body > start_body))
                {
                    start_body += 6;
                    returnValue.Content = Display_Text.Substring(start_body, end_body - start_body) + " ";

                    if ((Source.Length > 0) && (returnValue.Content.IndexOf("<%LASTMODIFIED%>") > 0))
                    {
                        FileInfo fileInfo = new FileInfo(Source);
                        DateTime lastWritten = fileInfo.LastWriteTime;
                        returnValue.Content = returnValue.Content.Replace("<%LASTMODIFIED%>", lastWritten.ToLongDateString());
                    }
                }
                else
                {
                    returnValue.Content = Display_Text.Replace("<body>", "").Replace("</body>", "");
                }
            }

            return returnValue;
        }
    }
}
