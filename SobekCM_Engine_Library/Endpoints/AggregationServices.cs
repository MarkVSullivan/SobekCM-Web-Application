using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Jil;
using ProtoBuf;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.JSON_Client_Helpers;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Tools;

namespace SobekCM.Engine_Library.Endpoints
{
    public class AggregationServices
    {
        public void GetCompleteAggregationByCode(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                string aggrCode = UrlSegments[0];

                Complete_Item_Aggregation returnValue = get_complete_aggregation(aggrCode, false, tracer);


                if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                {
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                }
                else
                {
                    Serializer.Serialize(Response.OutputStream, returnValue);
                }
            }
        }


        public void GetAggregationByCode(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
        {

            if (UrlSegments.Count > 1)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                // Get the code and language from the URL
                string aggrCode = UrlSegments[0];
                string language = UrlSegments[1];
                Web_Language_Enum languageEnum = Web_Language_Enum_Converter.Code_To_Enum(language);

                Item_Aggregation returnValue = get_item_aggregation(aggrCode, languageEnum, Engine_ApplicationCache_Gateway.Settings.Default_UI_Language, false, tracer);


                if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                {
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                }
                else
                {
                    Serializer.Serialize(Response.OutputStream, returnValue);
                }
            }
        }


        public void GetAllAggregations(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            // Get the aggregation code manager
            List<Item_Aggregation_Related_Aggregations> list = Engine_ApplicationCache_Gateway.Codes.All_Aggregations;

            if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
            {
                JSON.Serialize(list, Response.Output, Options.ISO8601ExcludeNulls);
            }
            else
            {
                Serializer.Serialize(Response.OutputStream, list);
            }
        }

        public void GetAggregationUploadedImages(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
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

                        string[] files = Directory.GetFiles(design_folder);
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

        public void GetCollectionHierarchy(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            // Get the aggregation code manager
            Aggregation_Hierarchy returnValue = get_aggregation_hierarchy(null);

            if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
            {
                JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
            }
            else
            {
                Serializer.Serialize(Response.OutputStream, returnValue);
            }
        }

        #region Helper methods (some may currently be public though)

        public static Aggregation_Hierarchy get_aggregation_hierarchy(Custom_Tracer Tracer)
        {
            // Get the aggregation code manager
            Aggregation_Hierarchy returnValue = CachedDataManager.Aggregations.Retrieve_Aggregation_Hierarchy(Tracer);
            if (returnValue == null)
            {
                // Build the collection hierarchy (from the database)
                returnValue = Item_Aggregation_Utilities.Get_Collection_Hierarchy(Tracer);

                // Store in the cache
                if ( returnValue != null )
                    CachedDataManager.Aggregations.Store_Aggregation_Hierarchy(returnValue, Tracer);
            }

            return returnValue;
        }


        public static HTML_Based_Content get_item_aggregation_html_child_page(string AggregationCode, Web_Language_Enum RequestedLanguage, Web_Language_Enum DefaultLanguage, bool isRobot, string ChildPageCode, Custom_Tracer Tracer)
        {
            // Try to pull from the cache
            HTML_Based_Content cacheInst = CachedDataManager.Aggregations.Retrieve_Aggregation_HTML_Based_Content(AggregationCode, RequestedLanguage, ChildPageCode, Tracer);
            if (cacheInst != null)
                return cacheInst;

            Item_Aggregation itemAggr = get_item_aggregation(AggregationCode, RequestedLanguage, DefaultLanguage, false, Tracer);
            if (itemAggr != null)
            {
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

            return null;
        }

        

        public static Item_Aggregation get_item_aggregation(string AggregationCode, Web_Language_Enum RequestedLanguage, Web_Language_Enum DefaultLanguage, bool isRobot, Custom_Tracer Tracer)
        {
            // Try to pull from the cache
            Item_Aggregation cacheInst = CachedDataManager.Aggregations.Retrieve_Item_Aggregation(AggregationCode, RequestedLanguage, Tracer);
            if (cacheInst != null)
                return cacheInst;

            // Get the complete aggregation
            Complete_Item_Aggregation compAggr = get_complete_aggregation(AggregationCode, isRobot, Tracer);

            // Get the language-specific version
            Item_Aggregation returnValue = Item_Aggregation_Utilities.Get_Item_Aggregation(compAggr, RequestedLanguage, Tracer);

            // Store in cache again
            CachedDataManager.Aggregations.Store_Item_Aggregation(AggregationCode, RequestedLanguage, returnValue, Tracer);

            return returnValue;
        }

        public static Complete_Item_Aggregation get_complete_aggregation(string AggregationCode, bool isRobot, Custom_Tracer Tracer)
        {
            // Try to pull this from the cache
            Complete_Item_Aggregation cacheAggr = CachedDataManager.Aggregations.Retrieve_Complete_Item_Aggregation(AggregationCode, Tracer);
            if (cacheAggr != null)
                return cacheAggr;

            // Either use the cache version, or build the complete item aggregation
            Complete_Item_Aggregation itemAggr = Item_Aggregation_Utilities.Get_Complete_Item_Aggregation(AggregationCode, isRobot, Tracer);

            // Now, save this to the cache
            if (itemAggr != null)
            {
                if (!isRobot)
                {
                    CachedDataManager.Aggregations.Store_Complete_Item_Aggregation(AggregationCode, itemAggr, Tracer);
                }
                else
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Skipping storing item aggregation on cache due to robot flag");
                    }
                }
            }

            return itemAggr;
        }

        #endregion


    }
}
