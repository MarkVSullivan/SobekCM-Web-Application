using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Web;
using Jil;
using ProtoBuf;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Message;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.JSON_Client_Helpers;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Tools;

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Class supports all the aggregation-level services provided by the SobekCM engine </summary>
    public class AggregationServices
    {
        /// <summary> Gets the complete (language agnostic) item aggregation, by aggregation code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetCompleteAggregationByCode(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                string aggrCode = UrlSegments[0];

                Complete_Item_Aggregation returnValue = get_complete_aggregation(aggrCode, true, tracer);

                switch (Protocol)
                {
                    case Microservice_Endpoint_Protocol_Enum.JSON:
                        JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                        Serializer.Serialize(Response.OutputStream, returnValue);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.JSON_P:
                        Response.Output.Write("parseCompleteAggregation(");
                        JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                        Response.Output.Write(");");
                        break;

                    case Microservice_Endpoint_Protocol_Enum.XML:
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(returnValue.GetType());
                        x.Serialize(Response.Output, returnValue);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.SOAP:
                        DataContractSerializer soap = new DataContractSerializer(returnValue.GetType());
                        soap.WriteObject(Response.OutputStream, returnValue);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.BINARY:
                        IFormatter binary = new BinaryFormatter();
                        binary.Serialize(Response.OutputStream, returnValue);
                        break;
                }
            }
        }

        /// <summary> Gets the language-specific item aggregation, by aggregation code and language code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetAggregationByCode(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {

            if (UrlSegments.Count > 1)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                // Get the code and language from the URL
                string aggrCode = UrlSegments[0];
                string language = UrlSegments[1];
                Web_Language_Enum languageEnum = Web_Language_Enum_Converter.Code_To_Enum(language);

                Item_Aggregation returnValue = get_item_aggregation(aggrCode, languageEnum, Engine_ApplicationCache_Gateway.Settings.Default_UI_Language, tracer);

                switch (Protocol)
                {
                    case Microservice_Endpoint_Protocol_Enum.JSON:
                        JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                        Serializer.Serialize(Response.OutputStream, returnValue);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.JSON_P:
                        Response.Output.Write("parseAggregationByCode(");
                        JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                        Response.Output.Write(");");
                        break;

                    case Microservice_Endpoint_Protocol_Enum.XML:
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(returnValue.GetType());
                        x.Serialize(Response.Output, returnValue);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.SOAP:
                        DataContractSerializer soap = new DataContractSerializer(returnValue.GetType());
                        soap.WriteObject(Response.OutputStream, returnValue);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.BINARY:
                        IFormatter binary = new BinaryFormatter();
                        binary.Serialize(Response.OutputStream, returnValue);
                        break;
                }
            }
        }

        /// <summary> Gets the list of all aggregations - including inactive and hidden </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetAllAggregations(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            // Get the aggregation code manager
            List<Item_Aggregation_Related_Aggregations> returnValue = Engine_ApplicationCache_Gateway.Codes.All_Aggregations;

            switch (Protocol)
            {
                case Microservice_Endpoint_Protocol_Enum.JSON:
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                    break;

                case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                    Serializer.Serialize(Response.OutputStream, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.JSON_P:
                    Response.Output.Write("parseAllAggregations(");
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                    Response.Output.Write(");");
                    break;

                case Microservice_Endpoint_Protocol_Enum.XML:
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(returnValue.GetType());
                    x.Serialize(Response.Output, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.SOAP:
                        DataContractSerializer soap = new DataContractSerializer(returnValue.GetType());
                        soap.WriteObject(Response.OutputStream, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.BINARY:
                    IFormatter binary = new BinaryFormatter();
                    binary.Serialize(Response.OutputStream, returnValue);
                    break;
            }
        }

        /// <summary> [PUBLIC] Get the list of uploaded images for a particular aggregation </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <remarks> This REST API should be publicly available for users that are performing administrative work </remarks>
        public void GetAggregationUploadedImages(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            if (UrlSegments.Count > 0)
            {
                string aggregation = UrlSegments[0];

                // Ensure a valid aggregation
                Item_Aggregation_Related_Aggregations aggrInfo = Engine_ApplicationCache_Gateway.Codes[aggregation];
                if (aggrInfo != null)
                {
                    List<UploadedFileFolderInfo> serverFiles = new List<UploadedFileFolderInfo>();

                    string design_folder = Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + "aggregations\\" + aggregation + "\\uploads";
                    if (Directory.Exists(design_folder))
                    {
                        string foldername = aggrInfo.ShortName;
                        if ( String.IsNullOrEmpty(foldername))
                            foldername = aggregation;

                        string[] files = SobekCM_File_Utilities.GetFiles(design_folder, "*.jpg|*.bmp|*.gif|*.png");
                        foreach (string thisFile in files)
                        {
                            string filename = Path.GetFileName(thisFile);
                            string extension = Path.GetExtension(thisFile);
                            
                            // Exclude some files
                            if ((!String.IsNullOrEmpty(extension)) && (extension.ToLower().IndexOf(".db") < 0) && (extension.ToLower().IndexOf("bridge") < 0) && (extension.ToLower().IndexOf("cache") < 0))
                            {
                                string url = Engine_ApplicationCache_Gateway.Settings.System_Base_URL + "design/aggregations/" + aggregation + "/uploads/" + filename;
                                serverFiles.Add(new UploadedFileFolderInfo(url, foldername));
                            }
                        }
                    }

                    JSON.Serialize(serverFiles, Response.Output, Options.ISO8601ExcludeNulls);
                }
            }
        }

        /// <summary> Gets the entire collection hierarchy (used for hierarchical tree displays) </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetCollectionHierarchy(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            // Get the aggregation code manager
            Aggregation_Hierarchy returnValue = get_aggregation_hierarchy(null);

            switch (Protocol)
            {
                case Microservice_Endpoint_Protocol_Enum.JSON:
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                    break;

                case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                    Serializer.Serialize(Response.OutputStream, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.JSON_P:
                    Response.Output.Write("parseCollectionHierarchy(");
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                    Response.Output.Write(");");
                    break;

                case Microservice_Endpoint_Protocol_Enum.XML:
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(returnValue.GetType());
                    x.Serialize(Response.Output, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.SOAP:
                        DataContractSerializer soap = new DataContractSerializer(returnValue.GetType());
                        soap.WriteObject(Response.OutputStream, returnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.BINARY:
                    IFormatter binary = new BinaryFormatter();
                    binary.Serialize(Response.OutputStream, returnValue);
                    break;
            }
        }

        /// <summary> Gets the all information, including the HTML, for an item aggregation child page </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetCollectionStaticPage(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            if (UrlSegments.Count > 2)
            {
                // Get the code and language from the URL
                string aggrCode = UrlSegments[0];
                string language = UrlSegments[1];
                string childCode = UrlSegments[2];
                Web_Language_Enum langEnum = Web_Language_Enum_Converter.Code_To_Enum(language);

                // Get the aggregation code manager
                HTML_Based_Content returnValue = get_item_aggregation_html_child_page(aggrCode, langEnum, Engine_ApplicationCache_Gateway.Settings.Default_UI_Language, childCode, null);

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

                    case Microservice_Endpoint_Protocol_Enum.SOAP:
                        DataContractSerializer soap = new DataContractSerializer(returnValue.GetType());
                        soap.WriteObject(Response.OutputStream, returnValue);
                        break;

                    case Microservice_Endpoint_Protocol_Enum.BINARY:
                        IFormatter binary = new BinaryFormatter();
                        binary.Serialize(Response.OutputStream, returnValue);
                        break;
                }
            }
        }

        #region Helper methods (some may currently be public though)

        /// <summary> [HELPER] Gets the entire collection hierarchy (used for hierarchical tree displays) </summary>
        /// <param name="Tracer"></param>
        /// <returns> Fully built aggregation hierarchy </returns>
        /// <remarks> This may be public now, but this will be converted into a private helped class with 
        /// the release of SobekCM 5.0 </remarks>
        public static Aggregation_Hierarchy get_aggregation_hierarchy(Custom_Tracer Tracer)
        {
            // Get the aggregation code manager
            Aggregation_Hierarchy returnValue = CachedDataManager.Aggregations.Retrieve_Aggregation_Hierarchy(Tracer);
            if (returnValue != null) return returnValue;

            // Build the collection hierarchy (from the database)
            returnValue = Item_Aggregation_Utilities.Get_Collection_Hierarchy(Tracer);

            // Store in the cache
            if ( returnValue != null )
                CachedDataManager.Aggregations.Store_Aggregation_Hierarchy(returnValue, Tracer);

            return returnValue;
        }

        /// <summary> [HELPER] Gets the all information, including the HTML, for an item aggregation child page </summary>
        /// <param name="AggregationCode"> Code for the aggregation </param>
        /// <param name="RequestedLanguage"> Requested language to retrieve </param>
        /// <param name="DefaultLanguage"> Default interface language, in case the requested language does not exist </param>
        /// <param name="ChildPageCode"> Code the requested child page </param>
        /// <param name="Tracer"></param>
        /// <returns> Fully built object, based on the aggregation configuration and reading the source HTML file </returns>
        /// <remarks> This may be public now, but this will be converted into a private helped class with 
        /// the release of SobekCM 5.0 </remarks>
        public static HTML_Based_Content get_item_aggregation_html_child_page(string AggregationCode, Web_Language_Enum RequestedLanguage, Web_Language_Enum DefaultLanguage, string ChildPageCode, Custom_Tracer Tracer)
        {
            // Try to pull from the cache
            HTML_Based_Content cacheInst = CachedDataManager.Aggregations.Retrieve_Aggregation_HTML_Based_Content(AggregationCode, RequestedLanguage, ChildPageCode, Tracer);
            if (cacheInst != null)
                return cacheInst;

            Item_Aggregation itemAggr = get_item_aggregation(AggregationCode, RequestedLanguage, DefaultLanguage, Tracer);
            if (itemAggr == null) return null;

            Item_Aggregation_Child_Page childPage = itemAggr.Child_Page_By_Code(ChildPageCode);
            if ((childPage == null) || ( childPage.Source_Data_Type != Item_Aggregation_Child_Source_Data_Enum.Static_HTML )) return null;

            string path = Path.Combine("/design/", itemAggr.ObjDirectory, childPage.Source);
            string file = HttpContext.Current.Server.MapPath(path);

            HTML_Based_Content results = HTML_Based_Content_Reader.Read_HTML_File(file, true, Tracer);

            if (results != null)
            {
                CachedDataManager.Aggregations.Store_Aggregation_HTML_Based_Content(AggregationCode, RequestedLanguage, ChildPageCode, results, Tracer);
            }

            return results;
        }


        /// <summary> [HELPER] Gets the language-specific item aggregation, by aggregation code and language code </summary>
        /// <param name="AggregationCode"> Code for the aggregation </param>
        /// <param name="RequestedLanguage"> Requested language to retrieve </param>
        /// <param name="DefaultLanguage"> Default interface language, in case the requested language does not exist </param>
        /// <param name="Tracer"></param>
        /// <returns> Built language-specific item aggregation object </returns>
        /// <remarks> This may be public now, but this will be converted into a private helped class with 
        /// the release of SobekCM 5.0 </remarks>
        public static Item_Aggregation get_item_aggregation(string AggregationCode, Web_Language_Enum RequestedLanguage, Web_Language_Enum DefaultLanguage, Custom_Tracer Tracer)
        {
            // Try to pull from the cache
            Item_Aggregation cacheInst = CachedDataManager.Aggregations.Retrieve_Item_Aggregation(AggregationCode, RequestedLanguage, Tracer);
            if (cacheInst != null)
                return cacheInst;

            // Get the complete aggregation
            Complete_Item_Aggregation compAggr = get_complete_aggregation(AggregationCode, true, Tracer);

            // Get the language-specific version
            Item_Aggregation returnValue = Item_Aggregation_Utilities.Get_Item_Aggregation(compAggr, RequestedLanguage, Tracer);

            // Store in cache again
            CachedDataManager.Aggregations.Store_Item_Aggregation(AggregationCode, RequestedLanguage, returnValue, Tracer);

            return returnValue;
        }

        /// <summary> [HELPER] Gets the complete (language agnostic) item aggregation, by aggregation code </summary>
        /// <param name="AggregationCode"> Code the requested aggregation </param>
        /// <param name="Tracer"></param>
        /// <returns> Fully built language-agnostic aggregation, with all related configurations </returns>
        /// <remarks> This may be public now, but this will be converted into a private helped class with 
        /// the release of SobekCM 5.0 </remarks>
        public static Complete_Item_Aggregation get_complete_aggregation(string AggregationCode, bool UseCache, Custom_Tracer Tracer)
        {
            // Try to pull this from the cache
            if (UseCache)
            {
                Complete_Item_Aggregation cacheAggr = CachedDataManager.Aggregations.Retrieve_Complete_Item_Aggregation(AggregationCode, Tracer);
                if (cacheAggr != null)
                    return cacheAggr;

                // Either use the cache version, or build the complete item aggregation
                Complete_Item_Aggregation itemAggr = Item_Aggregation_Utilities.Get_Complete_Item_Aggregation(AggregationCode, Tracer);

                // Now, save this to the cache
                if (itemAggr != null)
                {
                    CachedDataManager.Aggregations.Store_Complete_Item_Aggregation(AggregationCode, itemAggr, Tracer);
                }

                return itemAggr;
            }
            else
            {
                return Item_Aggregation_Utilities.Get_Complete_Item_Aggregation(AggregationCode, Tracer);
            }
        }

        #endregion

        #region Method to add a new aggrgeation to the system, building all the default folders and files

        /// <summary> Add a new aggregation to the system </summary>
        /// <param name="NewAggregation"> Information for the new aggregation </param>
        /// <returns> Message indicating success or any errors encountered </returns>
        public static ErrorRestMessage add_new_aggregation(New_Aggregation_Arguments NewAggregation)
        {
            // Convert to the integer id for the parent and begin to do checking
            List<string> errors = new List<string>();
            int parentid = -1;
            if (NewAggregation.ParentCode.Length > 0)
            {
                Item_Aggregation_Related_Aggregations parentAggr = Engine_ApplicationCache_Gateway.Codes[NewAggregation.ParentCode];
                if (parentAggr != null)
                    parentid = parentAggr.ID;
                else
                    errors.Add("Parent code is not valid");
            }
            else
            {
                errors.Add("You must select a PARENT for this new aggregation");
            }

            // Validate the code

            if (NewAggregation.Code.Length > 20)
            {
                errors.Add("New aggregation code must be twenty characters long or less");
            }
            else if (NewAggregation.Code.Length == 0)
            {
                errors.Add("You must enter a CODE for this item aggregation");
            }
            else if (Engine_ApplicationCache_Gateway.Codes[NewAggregation.Code.ToUpper()] != null)
            {
                errors.Add("New code must be unique... <i>" + NewAggregation.Code + "</i> already exists");
            }
            else if (Engine_ApplicationCache_Gateway.Settings.Reserved_Keywords.Contains(NewAggregation.Code.ToLower()))
            {
                errors.Add("That code is a system-reserved keyword.  Try a different code.");
            }
            else
            {
                bool alphaNumericTest = NewAggregation.Code.All(C => Char.IsLetterOrDigit(C) || C == '_' || C == '-');
                if (!alphaNumericTest)
                {
                    errors.Add("New aggregation code must be only letters and numbers");
                    NewAggregation.Code = NewAggregation.Code.Replace("\"", "");
                }
            }

            // Was there a type and name
            if (NewAggregation.Type.Length == 0)
            {
                errors.Add("You must select a TYPE for this new aggregation");
            }
            if (NewAggregation.Description.Length == 0)
            {
                errors.Add("You must enter a DESCRIPTION for this new aggregation");
            }
            if (NewAggregation.Name.Length == 0)
            {
                errors.Add("You must enter a NAME for this new aggregation");
            }
            else
            {
                if (NewAggregation.ShortName.Length == 0)
                    NewAggregation.ShortName = NewAggregation.Name;
            }

            // Check for the thematic heading
            int thematicHeadingId = -1;
            if ( !String.IsNullOrEmpty(NewAggregation.Thematic_Heading))
            {
                // Look for the matching thematic heading
                foreach (Thematic_Heading thisHeading in Engine_ApplicationCache_Gateway.Thematic_Headings)
                {
                    if (String.Compare(thisHeading.Text, NewAggregation.Thematic_Heading, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        thematicHeadingId = thisHeading.ID;
                        break;
                    }
                }

                // If there was no match, the thematic heading was invalid, unless it was new
                if (thematicHeadingId < 0)
                {
                    if ((!NewAggregation.NewThematicHeading.HasValue) || ( !NewAggregation.NewThematicHeading.Value ))
                    {
                        errors.Add("Invalid thematic heading indicated");
                    }
                    else if (errors.Count == 0)
                    {
                        // Add the thematic heading first
                        if ((thematicHeadingId < 0) && (NewAggregation.NewThematicHeading.HasValue) && (NewAggregation.NewThematicHeading.Value))
                        {
                            thematicHeadingId = Engine_Database.Edit_Thematic_Heading(-1, 10, NewAggregation.Thematic_Heading, null);
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {

                // Create the error message
                StringBuilder actionMessage = new StringBuilder("ERROR: Invalid entry for new item aggregation.<br />");
                foreach (string error in errors)
                    actionMessage.Append("<br />" + error);

                return new ErrorRestMessage(ErrorRestType.InputError, actionMessage.ToString());
            }



            string language = Web_Language_Enum_Converter.Enum_To_Code(Engine_ApplicationCache_Gateway.Settings.Default_UI_Language);

            // Try to save the new item aggregation
            if (!Engine_Database.Save_Item_Aggregation(NewAggregation.Code, NewAggregation.Name, NewAggregation.ShortName, NewAggregation.Description, thematicHeadingId, NewAggregation.Type, NewAggregation.Active, NewAggregation.Hidden, NewAggregation.External_Link, parentid, NewAggregation.User, language, null))
            {
                return new ErrorRestMessage(ErrorRestType.Exception, "ERROR saving the new item aggregation to the database");
            }
            // Ensure a folder exists for this, otherwise create one
            try
            {
                string folder = Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + "aggregations\\" + NewAggregation.Code.ToLower();
                if (!Directory.Exists(folder))
                {
                    // Create this directory and all the subdirectories
                    Directory.CreateDirectory(folder);
                    Directory.CreateDirectory(folder + "/html");
                    Directory.CreateDirectory(folder + "/images");
                    Directory.CreateDirectory(folder + "/html/home");
                    Directory.CreateDirectory(folder + "/html/custom/home");
                    Directory.CreateDirectory(folder + "/images/buttons");
                    Directory.CreateDirectory(folder + "/images/banners");
                    Directory.CreateDirectory(folder + "/uploads");

                    // Get the parent name
                    string link_to_parent = String.Empty;
                    Item_Aggregation_Related_Aggregations parentAggr = Engine_ApplicationCache_Gateway.Codes.Aggregation_By_ID(parentid);
                    if (parentAggr != null)
                    {
                        if (String.Compare(parentAggr.Code, "all", StringComparison.InvariantCultureIgnoreCase) == 0)
                            link_to_parent = "<br />" + Environment.NewLine + " ← Back to <a href=\"<%BASEURL%>\" alt=\"Return to parent collection\">" + parentAggr.Name + "</a>" + Environment.NewLine;
                        else
                            link_to_parent = "<br />" + Environment.NewLine + " ← Back to <a href=\"<%BASEURL%>" + parentAggr.Code + "\" alt=\"Return to parent collection\">" + parentAggr.Name + "</a>" + Environment.NewLine;
                    }

                    // Create a default home text file
                    StreamWriter writer = new StreamWriter(folder + "/html/home/text.html");
                    writer.WriteLine(link_to_parent + "<br />" + Environment.NewLine + "<h3>About " + NewAggregation.Name + "</h3>" + Environment.NewLine + "<p>" + NewAggregation.Description + "</p>" + Environment.NewLine + "<p>To edit this, log on as the aggregation admin and hover over this text to edit it.</p>" + Environment.NewLine + "<br />");

                    writer.Flush();
                    writer.Close();

                    // Was a button indicated, and does it exist?
                    if ((!String.IsNullOrEmpty(NewAggregation.ButtonFile)) && (File.Exists(NewAggregation.ButtonFile)))
                    {
                        File.Copy(NewAggregation.ButtonFile, folder + "/images/buttons/coll.gif");
                    }
                    else
                    {
                        // Copy the default banner and buttons from images
                        if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design/aggregations/default_button.png"))
                            File.Copy(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design/aggregations/default_button.png", folder + "/images/buttons/coll.png");
                        if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design/aggregations/default_button.gif"))
                            File.Copy(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design/aggregations/default_button.gif", folder + "/images/buttons/coll.gif");

                    }

                    // Was a banner indicated, and does it exist?
                    string banner_file = String.Empty;
                    if ((!String.IsNullOrEmpty(NewAggregation.BannerFile)) && (File.Exists(NewAggregation.BannerFile)))
                    {
                        banner_file = "images/banners/" + Path.GetFileName(NewAggregation.BannerFile);
                        File.Copy(NewAggregation.BannerFile, folder + "//" + banner_file, true);
                    }
                    else
                    {

                        // Try to create a new custom banner
                        bool custom_banner_created = false;

                        // Create the banner with the name of the collection
                        if (Directory.Exists(Engine_ApplicationCache_Gateway.Settings.Application_Server_Network + "\\default\\banner_images"))
                        {
                            try
                            {
                                string[] banners = Directory.GetFiles(Engine_ApplicationCache_Gateway.Settings.Application_Server_Network + "\\default\\banner_images", "*.jpg");
                                if (banners.Length > 0)
                                {
                                    Random randomizer = new Random();
                                    string banner_to_use = banners[randomizer.Next(0, banners.Length - 1)];
                                    Bitmap bitmap = (Bitmap) (Image.FromFile(banner_to_use));

                                    RectangleF rectf = new RectangleF(30, bitmap.Height - 55, bitmap.Width - 40, 40);
                                    Graphics g = Graphics.FromImage(bitmap);
                                    g.SmoothingMode = SmoothingMode.AntiAlias;
                                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                    g.DrawString(NewAggregation.Name, new Font("Tahoma", 25, FontStyle.Bold), Brushes.Black, rectf);
                                    g.Flush();

                                    string new_file = folder + "/images/banners/coll.jpg";
                                    if (!File.Exists(new_file))
                                    {
                                        bitmap.Save(new_file, ImageFormat.Jpeg);
                                        custom_banner_created = true;
                                        banner_file = "images/banners/coll.jpg";
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                // Suppress this Error... 
                            }
                        }

                        if ((!custom_banner_created) && (!File.Exists(folder + "/images/banners/coll.jpg")))
                        {
                            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design/aggregations/default_banner.jpg"))
                            {
                                banner_file = "images/banners/coll.jpg";
                                File.Copy(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "design/aggregations/default_banner.jpg", folder + "/images/banners/coll.jpg", true);
                            }
                        }
                    }

                    // Now, try to create the item aggregation and write the configuration file
                    Complete_Item_Aggregation itemAggregation = SobekEngineClient.Aggregations.Get_Complete_Aggregation(NewAggregation.Code, true, null);
                    if (banner_file.Length > 0)
                    {
                        itemAggregation.Banner_Dictionary.Clear();
                        itemAggregation.Add_Banner_Image(banner_file, Engine_ApplicationCache_Gateway.Settings.Default_UI_Language);
                    }
                    itemAggregation.Write_Configuration_File(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory);

                    // If an email shoudl be sent, do that now
                    if (String.Compare(Engine_ApplicationCache_Gateway.Settings.Send_Email_On_Added_Aggregation, "always", true) == 0)
                    {
                        string user = String.Empty;
                        if (!String.IsNullOrEmpty(NewAggregation.User))
                            user = NewAggregation.User;

                        string body = "New aggregation added to this system:\n\n\tCode:\t" + itemAggregation.Code + "\n\tType:\t" + itemAggregation.Type + "\n\tName:\t" + itemAggregation.Name + "\n\tShort:\t" + itemAggregation.ShortName + "\n\tUser:\t" + user + "\n\n" + Engine_ApplicationCache_Gateway.Settings.Application_Server_URL + "/" + itemAggregation.Code;
                        Email.Email_Helper.SendEmail(Engine_ApplicationCache_Gateway.Settings.System_Email, "New " + itemAggregation.Type + " - " + itemAggregation.ShortName, body, false, Engine_ApplicationCache_Gateway.Settings.System_Name);
                    }
                }
            }
            catch
            {
                // Reload the list of all codes, to include this new one and the new hierarchy
                lock (Engine_ApplicationCache_Gateway.Codes)
                {
                    Engine_Database.Populate_Code_Manager(Engine_ApplicationCache_Gateway.Codes, null);
                }

                return new ErrorRestMessage(ErrorRestType.Exception, "ERROR completing the new aggregation add");
            }

            // Reload the list of all codes, to include this new one and the new hierarchy
            lock (Engine_ApplicationCache_Gateway.Codes)
            {
                Engine_Database.Populate_Code_Manager(Engine_ApplicationCache_Gateway.Codes, null);
            }

            // Clear all aggregation information (and thematic heading info) from the cache as well
            CachedDataManager.Aggregations.Clear();

            return new ErrorRestMessage(ErrorRestType.Successful, null);

        }

        #endregion


    }
}
