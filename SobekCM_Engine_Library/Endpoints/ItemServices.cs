#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Jil;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.BriefItem;
using SobekCM.Core.EAD;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Items;
using SobekCM.Core.MARC;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Items;
using SobekCM.Engine_Library.Items.BriefItems;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.MARC;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.EAD;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Class supports all the item-level services provided by the SobekCM engine </summary>
    public class ItemServices : EndpointBase
    {
        /// <summary> Gets the citation information for a digital resource </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemCitation(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Must at least have one URL segment for the BibID
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                try
                {
                    // Get the BibID and VID
                    string bibid = UrlSegments[0];
                    string vid = (UrlSegments.Count > 1) ? UrlSegments[1] : "00001";

                    tracer.Add_Trace("ItemServices.GetItemCitation", "Requested citation for " + bibid + ":" + vid);

                    if ((vid.Length > 0) && (vid != "00000"))
                    {
                        // Get the brief item
                        tracer.Add_Trace("ItemServices.GetItemCitation", "Build full brief item");
                        BriefItemInfo returnValue = GetBriefItem(bibid, vid, null, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.GetItemCitation", "NULL value returned from getBriefItem method");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // Create the wrapper to return only basic citation-type information
                        tracer.Add_Trace("ItemServices.GetItemCitation", "Create wrapper class to return only the citation info");
                        BriefItem_CitationResponse responder = new BriefItem_CitationResponse(returnValue);

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);

                            return;
                        }

                        // Get the JSON-P callback function
                        string json_callback = "parseItemCitation";
                        if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                        {
                            json_callback = QueryString["callback"];
                        }

                        // Use the base class to serialize the object according to request protocol
                        Serialize(responder, Response, Protocol, json_callback);
                    }
                    else
                    {
                        tracer.Add_Trace("ItemServices.GetItemCitation", "Requested VID 00000  - will pull the item group / title");

                        // Get the brief item
                        tracer.Add_Trace("ItemServices.GetItemCitation", "Build full brief item");
                        BriefItemInfo returnValue = GetBriefTitle(bibid, null, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.GetItemCitation", "NULL value returned from getBriefItem method");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // Create the wrapper to return only basic citation-type information
                        tracer.Add_Trace("ItemServices.GetItemCitation", "Create wrapper class to return only the citation info");
                        BriefItem_CitationResponse responder = new BriefItem_CitationResponse(returnValue);

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);

                            return;
                        }

                        // Get the JSON-P callback function
                        string json_callback = "parseItemGroupCitation";
                        if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                        {
                            json_callback = QueryString["callback"];
                        }

                        // Use the base class to serialize the object according to request protocol
                        Serialize(responder, Response, Protocol, json_callback);
                    }
                }
                catch (Exception ee)
                {
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("EXCEPTION CAUGHT!");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        return;
                    }

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");
                    Response.StatusCode = 500;
                }
            }
        }


        /// <summary> Gets the information about a single digital resource, using the STANDARD mapping set </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemBriefStandard(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            brief_item_response("standard", Response, UrlSegments, QueryString, Protocol, IsDebug );
        }

        /// <summary> Gets the information about a single digital resource, using the STANDARD mapping set </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemBriefInternal(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            brief_item_response("internal", Response, UrlSegments, QueryString, Protocol, IsDebug);
        }

        /// <summary> Gets the information about a single digital resource, using the indicated mapping set </summary>
        /// <param name="Mapping"> Mapping set to use </param>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        protected void brief_item_response(string Mapping, HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Must at least have one URL segment for the BibID
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                try
                {
                    // Get the BibID and VID
                    string bibid = UrlSegments[0];
                    string vid = ((UrlSegments.Count > 1) && ( UrlSegments[1].Length > 0 )) ? UrlSegments[1] : "00001";

                    tracer.Add_Trace("ItemServices.GetItemBrief", "Requested brief item info for " + bibid + ":" + vid + " using " + Mapping + " mapping");

                    if ((vid.Length > 0) && (vid != "00000"))
                    {
                        tracer.Add_Trace("ItemServices.GetItemBrief", "Build full brief item using " + Mapping + " mapping");
                        BriefItemInfo returnValue = GetBriefItem(bibid, vid, Mapping, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.GetItemBrief", "NULL value returned from getBriefItem method");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);

                            return;
                        }

                        // Get the JSON-P callback function
                        string json_callback = "parseItemBrief";
                        if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                        {
                            json_callback = QueryString["callback"];
                        }

                        // Use the base class to serialize the object according to request protocol
                        Serialize(returnValue, Response, Protocol, json_callback);
                    }
                    else
                    {
                        tracer.Add_Trace("ItemServices.GetItemCitation", "Requested VID 00000  - will pull the item group / title");

                        // Get the brief item
                        tracer.Add_Trace("ItemServices.GetItemCitation", "Build full brief item");
                        BriefItemInfo returnValue = GetBriefTitle(bibid, Mapping, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.GetItemCitation", "NULL value returned from getBriefItem method");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);

                            return;
                        }

                        // Get the JSON-P callback function
                        string json_callback = "parseItemGroupBrief";
                        if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                        {
                            json_callback = QueryString["callback"];
                        }

                        // Use the base class to serialize the object according to request protocol
                        Serialize(returnValue, Response, Protocol, json_callback);
                    }
                }
                catch (Exception ee)
                {
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("EXCEPTION CAUGHT!");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        return;
                    }

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");
                    Response.StatusCode = 500;
                }
            }
        }

        #region Code to support the legacy JSON reports supported prior to v5.0

        /// <summary> Gets the information about a single digital resource </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Get_Item_Info_Legacy(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Must at least have one URL segment for the BibID
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                try
                {
                    // Get the BibID and VID
                    string bibid = UrlSegments[0];
                    string vid = (UrlSegments.Count > 1) ? UrlSegments[1] : "00001";
                    string page_string = (UrlSegments.Count > 1) ? UrlSegments[1] : "1";
                    string viewer = (UrlSegments.Count > 3) ? UrlSegments[3] : String.Empty;

                    // Try to get the page
                    int page = 1;
                    int temp_page;
                    if (Int32.TryParse(page_string, out temp_page))
                        page = temp_page;

                    tracer.Add_Trace("ItemServices.Get_Item_Info_Legacy", "Requested legacy item info for " + bibid + ":" + vid);

                    if ((vid.Length > 0) && (vid != "00000"))
                    {
                        tracer.Add_Trace("ItemServices.Get_Item_Info_Legacy", "Build full brief item");
                        BriefItemInfo returnValue = GetBriefItem(bibid, vid, null, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.Get_Item_Info_Legacy", "NULL value returned from getBriefItem method");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);

                            return;
                        }

                        if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                        {
                            legacy_json_display_item_info(Response.Output, returnValue, page, viewer);
                        }
                    }
                    else
                    {
                        tracer.Add_Trace("ItemServices.Get_Item_Info_Legacy", "Requested VID 0000 - Invalid");

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);
                        }
                    }
                }
                catch (Exception ee)
                {
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("EXCEPTION CAUGHT!");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        return;
                    }

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");
                    Response.StatusCode = 500;
                }
            }
        }

        /// <summary> Writes the item information in JSON format directly to the output stream in legacy format </summary>
        /// <param name="Output"> Stream to which to write the JSON item information </param>
        /// <param name="BriefItem"></param>
        /// <param name="Page"></param>
        /// <param name="Viewer"></param>
        protected internal void legacy_json_display_item_info(TextWriter Output, BriefItemInfo BriefItem, int Page, string Viewer)
        {
            // Get the URL and network roots
            string network = Engine_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network;
            string image_url = Engine_ApplicationCache_Gateway.Settings.Servers.Image_URL;
            if ((image_url.Length > 0) && (image_url[image_url.Length - 1] != '/'))
                image_url = image_url + "/";

            // What if the page requested is greater than pages in the book?
            // What is the ID?
            // What if an item does not have jpeg's for each page?  No jpegs at all?
            Output.Write("[");
            if (BriefItem != null)
            {
                // Determine folder from BibID/VID
                string folder = BriefItem.BibID.Substring(0, 2) + "/" + BriefItem.BibID.Substring(2, 2) + "/" + BriefItem.BibID.Substring(4, 2) + "/" + BriefItem.BibID.Substring(6, 2) + "/" + BriefItem.BibID.Substring(8) + "/" + BriefItem.VID;

                if (Viewer != "text")
                {
                    int first_page_to_show = (Page - 1)*20;
                    int last_page_to_show = (Page*20) - 1;
                    if (first_page_to_show < BriefItem.Images.Count)
                    {
                        int page = first_page_to_show;
                        string jpeg_to_view = String.Empty;
                        while ((page < BriefItem.Images.Count) && (page <= last_page_to_show))
                        {
                            BriefItem_FileGrouping thisPage = BriefItem.Images[page];
                            bool found = false;
                            foreach (BriefItem_File thisFile in thisPage.Files.Where(ThisFile => ThisFile.Name.IndexOf(".JPG", StringComparison.OrdinalIgnoreCase) > 0))
                            {
                                jpeg_to_view = image_url + folder + "/" + thisFile.Name;
                                found = true;
                                break;
                            }
                            if (found)
                            {
                                if (page > first_page_to_show)
                                    Output.Write(",");
                                jpeg_to_view = jpeg_to_view.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                                Output.Write("{\"item_page\":{\"position\":" + (page + 1) + ",\"image_url\":\"" + jpeg_to_view + "\",\"id\":" + (page + 1) + ",\"collection_item_id\":1}}");
                            }
                            page++;
                        }
                    }
                }
                else
                {
                    // Get the list of all TEXT files
                    List<string> existing_text_files = new List<string>();
                    if (Directory.Exists(network + folder))
                    {
                        string[] allFiles = Directory.GetFiles(network + folder, "*.txt");
                        existing_text_files.AddRange(allFiles.Select(ThisFile => (new FileInfo(ThisFile)).Name.ToUpper()));
                    }


                    int page = 0;
                    string jpeg_to_view = String.Empty;
                    while (page < BriefItem.Images.Count)
                    {
                        string text_to_read = String.Empty;
                        BriefItem_FileGrouping thisPage = BriefItem.Images[page];
                        bool found = false;
                        foreach (BriefItem_File thisFile in thisPage.Files)
                        {
                            if (thisFile.Name.ToUpper().IndexOf(".JPG") > 0)
                            {
                                if (existing_text_files.Contains(thisFile.Name.ToUpper().Replace(".JPG", "") + ".TXT"))
                                {
                                    text_to_read = image_url + folder + "/" + thisFile.Name.Replace(".JPG", ".TXT").Replace(".jpg", ".txt");
                                }
                                jpeg_to_view = image_url + folder + "/" + thisFile.Name;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            if (page > 0)
                                Output.Write(",");
                            jpeg_to_view = jpeg_to_view.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                            text_to_read = text_to_read.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

                            Output.Write("{\"item_page\":{\"position\":" + (page + 1) + ",\"image_url\":\"" + jpeg_to_view + "\",\"text_url\":\"" + text_to_read + "\"}}");
                        }
                        page++;
                    }
                }
            }

            Output.Write("]");
        }

        #endregion

        #region Helper methods for getting the items

        public SobekCM_Item getSobekItem(string BibID, string VID, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("ItemServices.getSobekItem", "Get the Single_Item object from the Item_Lookup_Object from the cache");
            Single_Item selected_item = Engine_ApplicationCache_Gateway.Items.Item_By_Bib_VID(BibID, VID, Tracer);

            // If this is for a single item, return that
            if (selected_item != null)
            {
                // Try to get this from the cache
                SobekCM_Item currentItem = CachedDataManager.Items.Retrieve_Digital_Resource_Object(BibID, VID, Tracer);

                // If not pulled from the cache, then we will have to build the item
                if (currentItem == null)
                {
                    Tracer.Add_Trace("ItemServices.getSobekItem", "Unable to find the digital resource on the cache.. will build");
                    currentItem = SobekCM_Item_Factory.Get_Item(BibID, VID, Engine_ApplicationCache_Gateway.Icon_List, Tracer);
                    if (currentItem != null)
                    {
                        Tracer.Add_Trace("ItemServices.getSobekItem", "Store the digital resource object to the cache");
                        CachedDataManager.Items.Store_Digital_Resource_Object(BibID, VID, currentItem, Tracer);
                    }
                    else
                    {
                        Tracer.Add_Trace("ItemServices.getSobekItem", "Call to the SobekCM_Item_Factory returned NULL");
                        return null;
                    }
                }
                else
                {
                    Tracer.Add_Trace("ItemServices.getSobekItem", "Found the digital resource object on the cache");
                }

                return currentItem;
            }

            Tracer.Add_Trace("ItemServices.getSobekItem", "Could not locate the object from the Item_Lookup_Object.. may not be a valid bibid/vid combination");

            return null;
        }

        public SobekCM_Item getSobekItem(string BibID, string VID, int UserID, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("ItemServices.getSobekItem", "Get the Single_Item object from the Item_Lookup_Object from the cache");
            Single_Item selected_item = Engine_ApplicationCache_Gateway.Items.Item_By_Bib_VID(BibID, VID, Tracer);

            // If this is for a single item, return that
            if (selected_item != null)
            {
                // Try to get this from the cache
                SobekCM_Item currentItem = CachedDataManager.Items.Retrieve_Digital_Resource_Object(UserID, BibID, VID, Tracer);

                // If not pulled from the cache, then we will have to build the item
                if (currentItem == null)
                {
                    Tracer.Add_Trace("ItemServices.getSobekItem", "Unable to find the digital resource on the cache.. will build");
                    currentItem = SobekCM_Item_Factory.Get_Item(BibID, VID, Engine_ApplicationCache_Gateway.Icon_List, Tracer);
                    if (currentItem != null)
                    {
                        Tracer.Add_Trace("ItemServices.getSobekItem", "Store the digital resource object to the cache");
                        CachedDataManager.Items.Store_Digital_Resource_Object(UserID, BibID, VID, currentItem, Tracer);
                    }
                    else
                    {
                        Tracer.Add_Trace("ItemServices.getSobekItem", "Call to the SobekCM_Item_Factory returned NULL");
                        return null;
                    }
                }
                else
                {
                    Tracer.Add_Trace("ItemServices.getSobekItem", "Found the digital resource object on the cache");
                }

                return currentItem;
            }

            Tracer.Add_Trace("ItemServices.getSobekItem", "Could not locate the object from the Item_Lookup_Object.. may not be a valid bibid/vid combination");

            return null;
        }

        private BriefItemInfo GetBriefItem(string BibID, string VID, string MappingSet, Custom_Tracer Tracer)
        {
            // Get the full SOobekCM_Item object for the provided BibID / VID
            Tracer.Add_Trace("ItemServices.getBriefItem", "Get the full SobekCM_Item object for this BibID / VID");
            SobekCM_Item currentItem = getSobekItem(BibID, VID, Tracer);
            if (currentItem == null)
            {
                Tracer.Add_Trace("ItemServices.getBriefItem", "Could not retrieve the full SobekCM_Item object");
                return null;
            }

            // Was there a mapping set indicated?
            if (String.IsNullOrEmpty(MappingSet))
            {
                Tracer.Add_Trace("ItemServices.getBriefItem", "Map to the brief item, using the default mapping set");
                BriefItemInfo item = BriefItem_Factory.Create(currentItem, Tracer);
                return item;
            }

            Tracer.Add_Trace("ItemServices.getBriefItem", "Map to the brief item, using mapping set '" + MappingSet + "'");
            BriefItemInfo item2 = BriefItem_Factory.Create(currentItem, MappingSet, Tracer);
            return item2;
        }


        public SobekCM_Item getSobekItemGroup(string BibID, Custom_Tracer Tracer)
        {
            // Try to get this from the cache
            SobekCM_Item currentItem = CachedDataManager.Items.Retrieve_Digital_Resource_Object(BibID, Tracer);
            if (currentItem != null)
            {
                Tracer.Add_Trace("ItemServices.getSobekItemGroup", "Found the digital resource object on the cache");
                return currentItem;
            }

            // If not pulled from the cache, then we will have to build the item

            Tracer.Add_Trace("ItemServices.getSobekItemGroup", "Unable to find the digital resource on the cache.. will build");
            currentItem = SobekCM_Item_Factory.Get_Item_Group(BibID, Engine_ApplicationCache_Gateway.Icon_List, Tracer);
            if (currentItem != null)
            {
                Tracer.Add_Trace("ItemServices.getSobekItemGroup", "Store the digital resource object to the cache");
                CachedDataManager.Items.Store_Digital_Resource_Object(BibID, currentItem, Tracer);
            }
            else
            {
                Tracer.Add_Trace("ItemServices.getSobekItemGroup", "Call to the SobekCM_Item_Factory returned NULL");
                return null;
            }

            return currentItem;
        }
        
        
        #endregion

        #region Methods related to pulling information about a single item group

        private BriefItemInfo GetBriefTitle(string BibID, string MappingSet, Custom_Tracer Tracer)
        {
            // Get the full SOobekCM_Item object for the provided BibID / VID
            Tracer.Add_Trace("ItemServices.getBriefTitle", "Get the full SobekCM_Item object for this BibID-level resource");
            SobekCM_Item currentItem = getSobekTitle(BibID, Tracer);
            if (currentItem == null)
            {
                Tracer.Add_Trace("ItemServices.getBriefTitle", "Could not retrieve the full SobekCM_Item object");
                return null;
            }

            // Was there a mapping set indicated?
            if (String.IsNullOrEmpty(MappingSet))
            {
                Tracer.Add_Trace("ItemServices.getBriefTitle", "Map to the brief item, using the default mapping set");
                BriefItemInfo item = BriefItem_Factory.Create(currentItem, Tracer);
                return item;
            }
            
            
            Tracer.Add_Trace("ItemServices.getBriefTitle", "Map to the brief item, using mapping set '" + MappingSet + "'");
            BriefItemInfo item2 = BriefItem_Factory.Create(currentItem, MappingSet, Tracer);
            return item2;
        }

        public SobekCM_Item getSobekTitle(string BibID, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("ItemServices.getSobekTitle", "Verify this BibID exists in the repository");

            // Is this a valid BibID?
            if (!Engine_ApplicationCache_Gateway.Items.Contains_BibID(BibID))
            {
                Tracer.Add_Trace("ItemServices.getSobekTitle", "ERROR: Invalid BibID indicated!");
                return null;
            }

            // Try to get this from the cache
            SobekCM_Item currentItem = CachedDataManager.Items.Retrieve_Digital_Resource_Object(BibID, "00000", Tracer);

            // If not pulled from the cache, then we will have to build the item
            if (currentItem == null)
            {
                Tracer.Add_Trace("ItemServices.getSobekTitle", "Unable to find the digital resource on the cache.. will build");
                currentItem = SobekCM_Item_Factory.Get_Item_Group(BibID, Engine_ApplicationCache_Gateway.Icon_List, Tracer);
                if (currentItem != null)
                {
                    // Make a few adjustments here.
                    //currentItem.Bib_Info.Main_Title.Clear();
                    //currentItem.Bib_Info.Main_Title.Title = currentItem.Behaviors.GroupTitle;
                    currentItem.Bib_Info.SobekCM_Type_String = currentItem.Behaviors.GroupType;
                    currentItem.VID = "00000";

                    // Store this on th cache
                    Tracer.Add_Trace("ItemServices.getSobekTitle", "Store the digital resource object to the cache");
                    CachedDataManager.Items.Store_Digital_Resource_Object(BibID, "00000", currentItem, Tracer);
                }
                else
                {
                    Tracer.Add_Trace("ItemServices.getSobekTitle", "Call to the SobekCM_Item_Factory returned NULL");
                    return null;
                }
            }
            else
            {
                Tracer.Add_Trace("ItemServices.getSobekTitle", "Found the digital resource object on the cache");
            }

            return currentItem;
        }


        #endregion

        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetRandomItem(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON) || (Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P))
            {
                Tuple<string, string> result = Engine_Database.Get_Random_Item(null);

                if (result != null)
                {
                    if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                        JSON.Serialize(new {bibid = result.Item1, vid = result.Item2}, Response.Output, Options.ISO8601ExcludeNulls);
                    if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P)
                    {
                        // Get the JSON-P callback function
                        string json_callback = "parseRandomItem";
                        if (!String.IsNullOrEmpty(QueryString["callback"]))
                        {
                            json_callback = QueryString["callback"];
                        }

                        Response.Output.Write(json_callback + "(");
                        JSON.Serialize(new {bibid = result.Item1, vid = result.Item2}, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                        Response.Output.Write(");");
                    }
                }
            }
        }

        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemRdf(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            if (Protocol == Microservice_Endpoint_Protocol_Enum.XML)
            {

                if (UrlSegments.Count > 1)
                {
                    Custom_Tracer tracer = new Custom_Tracer();

                    string bibid = UrlSegments[1];
                    string vid = "00001";

                    if (UrlSegments.Count > 2)
                        vid = UrlSegments[2];

                    // Get the current item first
                    SobekCM_Item sobekItem = getSobekItem(bibid, vid, tracer);

                    // If null, return
                    if (sobekItem == null)
                        return;


                    string errorMessage;
                    Dictionary<string, object> options_rdf = new Dictionary<string, object>();
                    options_rdf["DC_File_ReaderWriter:RDF_Style"] = true;
                    DC_File_ReaderWriter rdfWriter = new DC_File_ReaderWriter();
                    rdfWriter.Write_Metadata(Response.Output, sobekItem, options_rdf, out errorMessage);
                }
            }
        }

        /// <summary> Get the item in a standard XML format, such as dublin core, marcxml, etc.. </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemXml(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            if (Protocol == Microservice_Endpoint_Protocol_Enum.XML)
            {

                if (UrlSegments.Count > 1)
                {
                    Custom_Tracer tracer = new Custom_Tracer();

                    string format = UrlSegments[0];
                    string bibid = UrlSegments[1];
                    string vid = "00001";

                    if (UrlSegments.Count > 2)
                        vid = UrlSegments[2];

                    // Get the current item first
                    SobekCM_Item sobekItem = getSobekItem(bibid, vid, tracer);

                    // If null, return
                    if (sobekItem == null)
                        return;


                    string errorMessage;
                    switch (format.ToLower())
                    {
                        case "dc":
                            DC_File_ReaderWriter dcWriter = new DC_File_ReaderWriter();
                            dcWriter.Write_Metadata(Response.Output, sobekItem, null, out errorMessage);
                            break;

                        case "rdf":
                            Dictionary<string, object> options_rdf = new Dictionary<string, object>();
                            options_rdf["DC_File_ReaderWriter:RDF_Style"] = true;
                            DC_File_ReaderWriter rdfWriter = new DC_File_ReaderWriter();
                            rdfWriter.Write_Metadata(Response.Output, sobekItem, options_rdf, out errorMessage);
                            break;

                        case "mods":
                            MODS_File_ReaderWriter modsWriter = new MODS_File_ReaderWriter();
                            modsWriter.Write_Metadata(Response.Output, sobekItem, null, out errorMessage);
                            break;

                        case "marc":

                            // Create the options dictionary used when saving information to the database, or writing MarcXML
                            Dictionary<string, object> options = new Dictionary<string, object>();
                            if (Engine_ApplicationCache_Gateway.Settings.MarcGeneration != null)
                            {
                                options["MarcXML_File_ReaderWriter:MARC Cataloging Source Code"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Cataloging_Source_Code;
                                options["MarcXML_File_ReaderWriter:MARC Location Code"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Location_Code;
                                options["MarcXML_File_ReaderWriter:MARC Reproduction Agency"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Agency;
                                options["MarcXML_File_ReaderWriter:MARC Reproduction Place"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Place;
                                options["MarcXML_File_ReaderWriter:MARC XSLT File"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.XSLT_File;
                            }
                            options["MarcXML_File_ReaderWriter:System Name"] = Engine_ApplicationCache_Gateway.Settings.System.System_Name;
                            options["MarcXML_File_ReaderWriter:System Abbreviation"] = Engine_ApplicationCache_Gateway.Settings.System.System_Abbreviation;


                            MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                            marcWriter.Write_Metadata(Response.Output, sobekItem, options, out errorMessage);
                            break;
                    }
                }
            }
        }

        #region Methods to return the EAD information about an item

        /// <summary> Gets any EAD information related to an item </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemEAD(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Must at least have one URL segment for the BibID
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                try
                {
                    // Get the BibID and VID
                    string bibid = UrlSegments[0];
                    string vid = (UrlSegments.Count > 1) ? UrlSegments[1] : "00001";

                    tracer.Add_Trace("ItemServices.GetItemEAD", "Requested ead info for " + bibid + ":" + vid);

                    // Is it a valid BibID/VID, at least in appearance?
                    if ((vid.Length > 0) && (vid != "00000"))
                    {
                        // Get the JSON-P callback function
                        string json_callback = "parseItemEAD";
                        if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                        {
                            json_callback = QueryString["callback"];
                        }

                        // Just look in the cache for the EAD information
                        EAD_Transfer_Object eadTransferInfo = CachedDataManager.Items.Retrieve_EAD_Info(bibid, vid, tracer);
                        if (eadTransferInfo != null)
                        {
                            tracer.Add_Trace("ItemServices.GetItemEAD", "Found pre-built EAD transfer object in the cache");

                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);

                                return;
                            }

                            // Use the base class to serialize the object according to request protocol
                            Serialize(eadTransferInfo, Response, Protocol, json_callback);
                        }

                        // Get the full SOobekCM_Item object for the provided BibID / VID
                        tracer.Add_Trace("ItemServices.GetItemEAD", "Get the full SobekCM_Item object for this BibID / VID");
                        SobekCM_Item currentItem = getSobekItem(bibid, vid, tracer);
                        if (currentItem == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.GetItemEAD", "Could not retrieve the full SobekCM_Item object");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // Create the wrapper to return only basic citation-type information
                        tracer.Add_Trace("ItemServices.GetItemEAD", "Create wrapper class to return only the ead info");
                        EAD_Transfer_Object responder = new EAD_Transfer_Object();

                        // Transfer all the data over to the EAD transfer object
                        EAD_Info eadInfo = currentItem.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as EAD_Info;
                        if (eadInfo != null)
                        {
                            tracer.Add_Trace("ItemServices.GetItemEAD", "Copy all the source EAD information into the transfer EAD object");

                            // Copy over the full description
                            responder.Full_Description = eadInfo.Full_Description;

                            // Add all the ead TOC sections
                            if (eadInfo.TOC_Included_Sections != null)
                            {
                                foreach (EAD_TOC_Included_Section tocSection in eadInfo.TOC_Included_Sections)
                                {
                                    responder.Add_TOC_Included_Section(tocSection.Internal_Link_Name, tocSection.Section_Title);
                                }
                            }

                            // Copy over all the container portions as well
                            if (eadInfo.Container_Hierarchy != null)
                            {
                                responder.Container_Hierarchy.Type = eadInfo.Container_Hierarchy.Type;
                                responder.Container_Hierarchy.Head = eadInfo.Container_Hierarchy.Head;
                                responder.Container_Hierarchy.Did = ead_copy_did_to_transfer(eadInfo.Container_Hierarchy.Did);

                                if (eadInfo.Container_Hierarchy.Containers != null)
                                {
                                    foreach (Container_Info containerInfo in eadInfo.Container_Hierarchy.Containers)
                                    {
                                        responder.Container_Hierarchy.Containers.Add(ead_copy_container_to_transfer(containerInfo));
                                    }
                                }
                            }
                        }
                        else
                        {
                            tracer.Add_Trace("ItemServices.GetItemEAD", "This existing digital resource has no special EAD information");
                        }

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);

                            return;
                        }

                        // Cache this as well, since it appears to be valid
                        CachedDataManager.Items.Store_EAD_Info(bibid, vid, responder, tracer);

                        // Use the base class to serialize the object according to request protocol
                        Serialize(responder, Response, Protocol, json_callback);
                    }
                    else
                    {
                        tracer.Add_Trace("ItemServices.GetItemEAD", "Requested VID 0000 - Invalid");

                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);
                        }
                    }
                }
                catch (Exception ee)
                {
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("EXCEPTION CAUGHT!");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        return;
                    }

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");
                    Response.StatusCode = 500;
                }
            }
        }

        private EAD_Transfer_Descriptive_Identification ead_copy_did_to_transfer(Descriptive_Identification Source)
        {
            // If null, just return null
            if (Source == null)
                return null;

            // Create the main object, and copy over the simple string values
            EAD_Transfer_Descriptive_Identification returnObj = new EAD_Transfer_Descriptive_Identification
            {
                DAO = Source.DAO,
                DAO_Link = Source.DAO_Link,
                DAO_Title = Source.DAO_Title,
                Extent = Source.Extent,
                Unit_Date = Source.Unit_Date,
                Unit_Title = Source.Unit_Title
            };

            // Copy any information about parent containers
            if (Source.Container_Count > 0)
            {
                foreach (Parent_Container_Info parentInfo in Source.Containers)
                {
                    returnObj.Add_Container(parentInfo.Container_Type, parentInfo.Container_Title);
                }
            }

            return returnObj;
        }

        private EAD_Transfer_Container_Info ead_copy_container_to_transfer(Container_Info Source)
        {
            // If null, just return null
            if (Source == null)
                return null;

            // Start to build the return container and copy over basic information
            EAD_Transfer_Container_Info returnObj = new EAD_Transfer_Container_Info
            {
                Did = ead_copy_did_to_transfer(Source.Did),
                Level = Source.Level,
                Has_Complex_Children = Source.Has_Complex_Children,
                Biographical_History = Source.Biographical_History,
                Scope_And_Content = Source.Scope_And_Content
            };

            // Now, need to recursively copy the child containers
            if (Source.Children_Count > 0)
            {
                foreach (Container_Info thisInfo in Source.Children)
                {
                    returnObj.Children.Add(ead_copy_container_to_transfer(thisInfo));
                }
            }

            return returnObj;
        }

        #endregion

        #region Method to get the MARC record for an item 

        /// <summary> Gets the item's marc record in object format for serialization/deserialization </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemMarcRecord(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();



                string bibid = UrlSegments[0];
                string vid = "00001";

                if (UrlSegments.Count > 1)
                    vid = UrlSegments[1];

                tracer.Add_Trace("ItemServices.GetItemMarcRecord", "Requested MARC record for " + bibid + ":" + vid);

                // Get the current item first
                SobekCM_Item sobekItem = getSobekItem(bibid, vid, tracer);

                // If null, return
                if (sobekItem == null)
                {
                    tracer.Add_Trace("ItemServices.GetItemMarcRecord", "Unable to retrieve the indicated digital resource ( " + bibid + ":" + vid + " )");

                    Response.ContentType = "text/plain";
                    Response.StatusCode = 500;
                    Response.Output.WriteLine("Unable to retrieve the indicated digital resource ( " + bibid + ":" + vid + " )");
                    Response.Output.WriteLine();

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                // Create the options dictionary used when saving information to the database, or writing MarcXML
                string cataloging_source_code = String.Empty;
                string location_code = String.Empty;
                string reproduction_agency = String.Empty;
                string reproduction_place = String.Empty;
                string system_name = String.Empty;
                string system_abbreviation = String.Empty;

                // Pull any settings that exist
                if (Engine_ApplicationCache_Gateway.Settings.MarcGeneration != null)
                {
                    if (!String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Cataloging_Source_Code))
                        cataloging_source_code = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Cataloging_Source_Code;
                    if (!String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Location_Code))
                        location_code = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Location_Code;
                    if (!String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Agency))
                        reproduction_agency = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Agency;
                    if (!String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Place))
                        reproduction_place = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Place;
                }

                if (!String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.System.System_Name))
                    system_name = Engine_ApplicationCache_Gateway.Settings.System.System_Name;
                if (!String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.System.System_Abbreviation))
                    system_abbreviation = Engine_ApplicationCache_Gateway.Settings.System.System_Abbreviation;

                // Get the base URL for the thumbnails
                string thumbnail_base = Engine_ApplicationCache_Gateway.Settings.Servers.Base_URL;

                // Get all the standard tags
                tracer.Add_Trace("ItemServices.GetItemMarcRecord", "Creating marc record from valid sobekcm item");
                MARC_Record tags = sobekItem.To_MARC_Record(cataloging_source_code, location_code, reproduction_agency, reproduction_place, system_name, system_abbreviation, thumbnail_base);

                // Now, convert the MARC record from the resource library over to the transfer objects
                MARC_Transfer_Record transferRecord = new MARC_Transfer_Record();
                if (tags != null)
                {
                    tracer.Add_Trace("ItemServices.GetItemMarcRecord", "Mapping from marc record to transfer marc record");

                    // Copy over the basic stuff
                    transferRecord.Control_Number = tags.Control_Number;
                    transferRecord.Leader = tags.Leader;

                    // Copy over the fields
                    List<MARC_Field> fields = tags.Sorted_MARC_Tag_List;
                    foreach (MARC_Field thisField in fields)
                    {
                        MARC_Transfer_Field transferField = new MARC_Transfer_Field
                        {
                            Tag = thisField.Tag,
                            Indicator1 = thisField.Indicator1,
                            Indicator2 = thisField.Indicator2
                        };

                        if (!String.IsNullOrEmpty(thisField.Control_Field_Value))
                            transferField.Control_Field_Value = thisField.Control_Field_Value;

                        if (thisField.Subfield_Count > 0)
                        {
                            ReadOnlyCollection<MARC_Subfield> subfields = thisField.Subfields;
                            foreach (MARC_Subfield thisSubfield in subfields)
                            {
                                transferField.Add_Subfield(thisSubfield.Subfield_Code, thisSubfield.Data);
                            }
                        }

                        // Add this transfer field to the transfer record
                        transferRecord.Add_Field(transferField);
                    }
                }
                else
                {
                    tracer.Add_Trace("ItemServices.GetItemMarcRecord", "MARC record returned was NULL");
                }

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parseMarcRecord";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(transferRecord, Response, Protocol, json_callback);
            }
        }

        #endregion

        #region Method to get the usage statistics for an item

        /// <summary> Gets the month-by-month usage for an item </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemStatisticsHistory(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Must at least have one URL segment for the BibID
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                try
                {
                    // Get the BibID and VID
                    string bibid = UrlSegments[0];
                    string vid = (UrlSegments.Count > 1) ? UrlSegments[1] : "00001";

                    tracer.Add_Trace("ItemServices.GetItemStatisticsHistory", "Requested usage history for " + bibid + ":" + vid);

                    // Look in the cache
                    tracer.Add_Trace("ItemServices.GetItemStatisticsHistory", "Looking in the cache");
                    List<Item_Monthly_Usage> returnValue = CachedDataManager.Items.Retrieve_Item_Usage(bibid, vid, tracer);
                    if (returnValue != null)
                    {
                        tracer.Add_Trace("ItemServices.GetItemStatisticsHistory", "Found the usage history in the cache");
                    }
                    else
                    {
                        // Return the built list of items
                        tracer.Add_Trace("ItemServices.GetItemStatisticsHistory", "Pull the data from the database");
                        returnValue = Engine_Database.Get_Item_Statistics_History(bibid, vid, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.GetItemStatisticsHistory", "NULL value returned from Get_Item_Statistics_History method");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // Store in the cache
                        tracer.Add_Trace("ItemServices.GetItemStatisticsHistory", "Storing in the cache");
                        CachedDataManager.Items.Store_Item_Usage(bibid, vid, returnValue, tracer);
                    }

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);

                        return;
                    }

                    // Get the JSON-P callback function
                    string json_callback = "parseItemUsage";
                    if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                    {
                        json_callback = QueryString["callback"];
                    }

                    // Use the base class to serialize the object according to request protocol
                    Serialize(returnValue, Response, Protocol, json_callback);
                }
                catch (Exception ee)
                {
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("EXCEPTION CAUGHT!");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        return;
                    }

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");
                    Response.StatusCode = 500;
                }
            }
        }

        #endregion

        #region Method to get the tracking/workflow/miletson information for an item

        /// <summary> Gets the work history related to an item </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemTrackingWorkHistory(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Must at least have one URL segment for the BibID
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                try
                {
                    // Get the BibID and VID
                    string bibid = UrlSegments[0];
                    string vid = (UrlSegments.Count > 1) ? UrlSegments[1] : "00001";

                    tracer.Add_Trace("ItemServices.GetItemTrackingWorkHistory", "Requested work history for " + bibid + ":" + vid);

                    // Look in the cache
                    tracer.Add_Trace("ItemServices.GetItemTrackingWorkHistory", "Looking in the cache");
                    Item_Tracking_Details returnValue = CachedDataManager.Items.Retrieve_Item_Tracking(bibid, vid, tracer);
                    if (returnValue != null)
                    {
                        tracer.Add_Trace("ItemServices.GetItemTrackingWorkHistory", "Found the work history in the cache");
                    }
                    else
                    {
                        // Return the built list of items
                        tracer.Add_Trace("ItemServices.GetItemTrackingWorkHistory", "Pull the data from the database");
                        returnValue = Engine_Database.Get_Item_Tracking_Work_History(bibid, vid, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.GetItemTrackingWorkHistory", "NULL value returned from Get_Item_Tracking_Work_History method");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // Store in the cache
                        tracer.Add_Trace("ItemServices.GetItemTrackingWorkHistory", "Storing in the cache");
                        CachedDataManager.Items.Store_Item_Tracking(bibid, vid, returnValue, tracer);
                    }

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);

                        return;
                    }

                    // Get the JSON-P callback function
                    string json_callback = "parseItemTracking";
                    if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                    {
                        json_callback = QueryString["callback"];
                    }

                    // Use the base class to serialize the object according to request protocol
                    Serialize(returnValue, Response, Protocol, json_callback);
                }
                catch (Exception ee)
                {
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("EXCEPTION CAUGHT!");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        return;
                    }

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");
                    Response.StatusCode = 500;
                }
            }
        }

        #endregion

        #region Method to get resource files for an item

        /// <summary> Gets the list of all files related to an item </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetItemFiles(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Must at least have one URL segment for the BibID
            if (UrlSegments.Count > 1)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                try
                {
                    // Get the BibID and VID
                    string bibid = UrlSegments[0];
                    string vid = UrlSegments[1];

                    tracer.Add_Trace("ItemServices.GetItemFiles", "Requested file list for " + bibid + ":" + vid);

                    // Build the brief item
                    tracer.Add_Trace("ItemServices.GetItemFiles", "Building the brief item");
                    BriefItemInfo briefItem = GetBriefItem(bibid, vid, null, tracer);

                    // Was the item null?
                    if (briefItem == null)
                    {
                        // If this was debug mode, then just write the tracer
                        if (IsDebug)
                        {
                            tracer.Add_Trace("ItemServices.GetItemFiles", "NULL value returned from getBriefItem method");

                            Response.ContentType = "text/plain";
                            Response.Output.WriteLine("DEBUG MODE DETECTED");
                            Response.Output.WriteLine();
                            Response.Output.WriteLine(tracer.Text_Trace);
                        }
                        return;
                    }

                    // Look in the cache
                    tracer.Add_Trace("ItemServices.GetItemFiles", "Requesting files from SobekFileSystem");
                    List<SobekFileSystem_FileInfo> files = SobekFileSystem.GetFiles(briefItem);

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);

                        return;
                    }

                    // Get the JSON-P callback function
                    string json_callback = "parseItemFiles";
                    if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                    {
                        json_callback = QueryString["callback"];
                    }

                    // Use the base class to serialize the object according to request protocol
                    Serialize(files, Response, Protocol, json_callback);
                }
                catch (Exception ee)
                {
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("EXCEPTION CAUGHT!");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        return;
                    }

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");
                    Response.StatusCode = 500;
                }
            }
        }

        #endregion

        #region Method to get the collection of items under a single title 

        /// <summary> Gets the collection of items under a title</summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void GetMultipleVolumes(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            // Must at least have one URL segment for the BibID
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                try
                {
                    // Get the BibID 
                    string bibid = UrlSegments[0];

                    tracer.Add_Trace("ItemServices.GetMultipleVolumes", "Requested list of items for " + bibid);

                    // Look in the cache
                    tracer.Add_Trace("ItemServices.GetMultipleVolumes", "Looking in the cache");
                    List<Item_Hierarchy_Details> itemList = CachedDataManager.Items.Retrieve_Item_List(bibid, tracer);
                    if (itemList != null)
                    {
                        tracer.Add_Trace("ItemServices.GetMultipleVolumes", "Found the list in the cache");
                    }
                    else
                    {
                        // Return the built list of items
                        tracer.Add_Trace("ItemServices.GetMultipleVolumes", "Pull the data from the database");
                        itemList = Engine_Database.Get_Multiple_Volumes(bibid, tracer);

                        // Was the item null?
                        if (itemList == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (IsDebug)
                            {
                                tracer.Add_Trace("ItemServices.GetMultipleVolumes", "NULL value returned from Get_Multiple_Volumes method");

                                Response.ContentType = "text/plain";
                                Response.Output.WriteLine("DEBUG MODE DETECTED");
                                Response.Output.WriteLine();
                                Response.Output.WriteLine(tracer.Text_Trace);
                            }
                            return;
                        }

                        // Store in the cache
                        tracer.Add_Trace("ItemServices.GetMultipleVolumes", "Storing in the cache");
                        CachedDataManager.Items.Store_Item_List(bibid, itemList, tracer);
                    }

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);

                        return;
                    }

                    // Get the JSON-P callback function
                    string json_callback = "parseItemList";
                    if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                    {
                        json_callback = QueryString["callback"];
                    }

                    // Use the base class to serialize the object according to request protocol
                    Serialize(itemList, Response, Protocol, json_callback);
                }
                catch (Exception ee)
                {
                    if (IsDebug)
                    {
                        Response.ContentType = "text/plain";
                        Response.Output.WriteLine("EXCEPTION CAUGHT!");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.Message);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(ee.StackTrace);
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                        return;
                    }

                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("Error completing request");
                    Response.StatusCode = 500;
                }
            }
        }

        #endregion

        #region Methods to serve small snippets of HTML to the users, on demand

        /// <summary> Writes the small snippet of HTML to pop-up when the user selects the SEND EMAIL button </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Send_Email_HTML_Snippet(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("ItemServices.Send_Email_HTML_Snippet", "Serve the small EMAIL html for adding tags to an item");

            // Determine the number of columns for text areas, depending on browser
            int actual_cols = 50;
            //if ((!String.IsNullOrEmpty(CurrentMode.Browser_Type)) && (CurrentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0))
            //    actual_cols = 45;

            // Build the response
            StringBuilder responseBuilder = new StringBuilder();
            responseBuilder.AppendLine("<!-- Email form -->");
            responseBuilder.AppendLine("<div id=\"emailform_content\" class=\"sbk_PopupForm\" style=\"width: 537px;\">");
            responseBuilder.AppendLine("  <div class=\"sbk_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left;\">Send this Item to a Friend</td><td style=\"text-align:right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"email_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            responseBuilder.AppendLine("  <br />");
            responseBuilder.AppendLine("  <fieldset><legend>Enter the email information below &nbsp; </legend>");
            responseBuilder.AppendLine("    <br />");
            responseBuilder.AppendLine("    <table class=\"sbk_PopupTable\">");


            // Add email address line
            responseBuilder.Append("      <tr><td style=\"width:80px\"><label for=\"email_address\">To:</label></td>");
            responseBuilder.AppendLine("<td><input class=\"email_input sbk_Focusable\" name=\"email_address\" id=\"email_address\" type=\"text\" value=\"\" /></td></tr>");

            // Add comments area
            responseBuilder.Append("      <tr style=\"vertical-align:top\"><td><br /><label for=\"email_comments\">Comments:</label></td>");
            responseBuilder.AppendLine("<td><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"email_comments\" id=\"email_comments\" class=\"email_textarea sbk_Focusable\" ></textarea></td></tr>");

            // Add format area
            responseBuilder.Append("      <tr style=\"vertical-align:top\"><td>Format:</td>");
            responseBuilder.Append("<td><input type=\"radio\" name=\"email_format\" id=\"email_format_html\" value=\"html\" checked=\"checked\" /> <label for=\"email_format_html\">HTML</label> &nbsp; &nbsp; ");
            responseBuilder.AppendLine("<input type=\"radio\" name=\"email_format\" id=\"email_format_text\" value=\"text\" /> <label for=\"email_format_text\">Plain Text</label></td></tr>");

            responseBuilder.AppendLine("    </table>");
            responseBuilder.AppendLine("    <br />");
            responseBuilder.AppendLine("  </fieldset><br />");
            responseBuilder.AppendLine("  <div style=\"text-align:center; font-size:1.3em;\">");
            responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" onclick=\"return email_form_close();\"> CANCEL </button> &nbsp; &nbsp; ");
            responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" type=\"submit\"> SEND </button>");
            responseBuilder.AppendLine("  </div><br />");
            responseBuilder.AppendLine("</div>");
            responseBuilder.AppendLine();

            // Get the return string them
            string returnValue = responseBuilder.ToString();

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseEmailHtmlSnippet";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Writes the small snippet of HTML to pop-up when the user selects the PRINT button </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Print_HTML_Snippet(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            if (UrlSegments.Count > 1)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                // Get the BibID and VID
                string bibid = UrlSegments[0];
                string vid = (UrlSegments.Count > 1) ? UrlSegments[1] : "00001";
                string pageviewer = (UrlSegments.Count > 2) ? UrlSegments[2] : "CITATION";
                int current_page = -1;
                if (UrlSegments.Count > 3)
                {
                    if (!Int32.TryParse(UrlSegments[3], out current_page))
                        current_page = -1;
                }

                tracer.Add_Trace("ItemServices.Print_HTML_Snippet", "Get the brief digital resource object for " + bibid + ":" + vid);
                BriefItemInfo sobekItem = GetBriefItem(bibid, vid, null, tracer);

                // If this item was NULL, there was an error
                if (sobekItem == null)
                {
                    tracer.Add_Trace("ItemServices.Print_HTML_Snippet", "Unable to retrieve the indicated digital resource ( " + bibid + ":" + vid + " )");

                    Response.ContentType = "text/plain";
                    Response.StatusCode = 500;
                    Response.Output.WriteLine("Unable to retrieve the indicated digital resource ( " + bibid + ":" + vid + " )");
                    Response.Output.WriteLine();

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                tracer.Add_Trace("ItemServices.Print_HTML_Snippet", "Building the HTML response");

                // Build the response
                StringBuilder responseBuilder = new StringBuilder();

                string print_options = String.Empty;
                string url_redirect = Engine_ApplicationCache_Gateway.Settings.Servers.Base_URL + bibid + "/" + vid + "/print";


                responseBuilder.AppendLine("<!-- Print item form -->");
                responseBuilder.AppendLine("<div id=\"printform_content\" class=\"sbk_PopupForm\">");
                responseBuilder.AppendLine("  <div class=\"sbk_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left;\">Print Options</td><td style=\"text-align:right\"> <a href=\"#template\" title=\"CLOSE\" onclick=\"print_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                responseBuilder.AppendLine("  <br />");
                responseBuilder.AppendLine("  <fieldset><legend>Select the options below to print this item &nbsp; </legend>");
                responseBuilder.AppendLine("    <blockquote>");
                responseBuilder.AppendLine("    <input type=\"checkbox\" id=\"print_citation\" name=\"print_citation\" checked=\"checked\" /> <label for=\"print_citation\">Include brief citation?</label><br /><br />");
                if ((sobekItem.Images == null )  || ( sobekItem.Images.Count == 0 ))
                {
                    responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"citation_only\" id=\"citation_only\" checked=\"checked\" /> <label for=\"current_page\">Full Citation</label><br />");
                }
                else
                {

                    bool something_selected = false;
                    if ( String.Compare(pageviewer, "CITATION", StringComparison.OrdinalIgnoreCase ) == 0 )
                    {
                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"citation_only\" id=\"citation_only\" class=\"print_radiobutton\" checked=\"checked\" /> <label for=\"citation_only\">Full Citation</label><br />");
                        something_selected = true;
                    }
                    else
                    {
                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"citation_only\" id=\"citation_only\" class=\"print_radiobutton\" /> <label for=\"citation_only\">Citation only</label><br />");
                    }

                    if (String.Compare(pageviewer, "THUMBNAILS", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"contact_sheet\" id=\"contact_sheet\" class=\"print_radiobutton\" checked=\"checked\" /> <label for=\"contact_sheet\">Print thumbnails</label><br />");
                        something_selected = true;
                    }
                    else
                    {

                        if (sobekItem.Behaviors.Get_Viewer("THUMBNAILS") != null )
                        {
                            responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"contact_sheet\" id=\"contact_sheet\" class=\"print_radiobutton\" /> <label for=\"contact_sheet\">Print thumbnails</label><br />");
                        }
                    }

                    if (String.Compare(pageviewer, "JPEG", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"current_page\" id=\"current_page\" checked=\"checked\" class=\"print_radiobutton\" /> <label for=\"current_page\">Print current page</label><br />");
                        something_selected = true;
                    }

                    if ((sobekItem.Images != null ) && ( sobekItem.Images.Count > 1 ))
                    {
                        // Add the all pages option
                        responseBuilder.AppendLine(!something_selected
                                             ? "    <input type=\"radio\" name=\"print_pages\" value=\"all_pages\" id=\"all_pages\" class=\"print_radiobutton\" checked=\"checked\" /> <label for=\"all_pages\">Print all pages</label><br />"
                                             : "    <input type=\"radio\" name=\"print_pages\" value=\"all_pages\" id=\"all_pages\" class=\"print_radiobutton\" /> <label for=\"all_pages\">Print all pages</label><br />");

                        // Build the options for selecting a page
                        StringBuilder optionBuilder = new StringBuilder();
                        int sequence = 1;
                        foreach (BriefItem_FileGrouping thisPage in sobekItem.Images)
                        {
                            if (thisPage.Label.Length > 25)
                            {
                                if (current_page == sequence )
                                {
                                    optionBuilder.Append("<option value=\"" + sequence + "\" selected=\"selected\">" + thisPage.Label.Substring(0, 20) + "...</option> ");
                                }
                                else
                                {
                                    optionBuilder.Append("<option value=\"" + sequence + "\">" + thisPage.Label.Substring(0, 20) + "...</option> ");
                                }
                            }
                            else
                            {
                                if (current_page == sequence)
                                {
                                    optionBuilder.Append("<option value=\"" + sequence + "\" selected=\"selected\">" + thisPage.Label + "</option> ");
                                }
                                else
                                {
                                    optionBuilder.Append("<option value=\"" + sequence + "\">" + thisPage.Label + "</option> ");
                                }
                            }

                            sequence++;
                        }

                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"range_page\" id=\"range_page\" class=\"print_radiobutton\" /> <label for=\"range_page\">Print a range of pages</label> <label for=\"print_from\">from</label> <select id=\"print_from\" name=\"print_from\">" + optionBuilder + "</select> <label for=\"print_to\">to</label> <select id=\"print_to\" name=\"print_to\">" + optionBuilder + "</select>");
                    }

                    //if ((currentUser != null) && (currentUser.Is_Internal_User))
                    //{
                    //    responseBuilder.AppendLine("    <br /><br /><input type=\"radio\" name=\"print_pages\" value=\"tracking_sheet\" id=\"tracking_sheet\" class=\"print_radiobutton\"  > <label for=\"tracking_sheet\">Print tracking sheet (internal users)</label><br />");
                    //}
                }
                responseBuilder.AppendLine("    </blockquote>");
                responseBuilder.AppendLine("  </fieldset><br />");
                responseBuilder.AppendLine("  <div style=\"text-align:center; font-size:1.3em;\">");
                responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" onclick=\"return print_form_close();return false;\"> CANCEL </button> &nbsp; &nbsp; ");
                responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" onclick=\"return print_item('" + current_page + "','" + url_redirect + "','" + print_options + "');return false;\"> PRINT </button>");
                responseBuilder.AppendLine("  </div><br />");
                responseBuilder.AppendLine("</div>");
                responseBuilder.AppendLine();


                // Get the return string them
                string returnValue = responseBuilder.ToString();

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parsePrintHtmlSnippet";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnValue, Response, Protocol, json_callback);
                return;
            }

            // Add the error
            Response.ContentType = "text/plain";
            Response.StatusCode = 400;
            Response.Output.WriteLine("BibID and VID are required parameters");
            Response.Output.WriteLine();
        }

        /// <summary> Serve the small DESCRIBE html used for adding tags to an item </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Describe_HTML_Snippet(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            tracer.Add_Trace("ItemServices.Describe_HTML_Snippet", "Serve the small DESCRIBE html for adding tags to an item");

            // Determine the number of columns for text areas, depending on browser
            int actual_cols = 50;
            //if ((!String.IsNullOrEmpty(CurrentMode.Browser_Type)) && (CurrentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0))
            //    actual_cols = 45;

            // Build the response
            StringBuilder responseBuilder = new StringBuilder();
            responseBuilder.AppendLine("<!-- Add descriptive tage form  -->");
            responseBuilder.AppendLine("<div class=\"describe_popup_div\" id=\"describe_item_form\" style=\"display:none;\">");
            responseBuilder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">A<span class=\"smaller\">DD </span> I<span class=\"smaller\">TEM </span> D<span class=\"smaller\">ESCRIPTION</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"describe_item_form_close()\">X</a> &nbsp; </td></tr></table></div>");
            responseBuilder.AppendLine("  <br />");
            responseBuilder.AppendLine("  <fieldset><legend>Enter a description or notes to add to this item &nbsp; </legend>");
            responseBuilder.AppendLine("    <br />");
            responseBuilder.AppendLine("    <table class=\"popup_table\">");

            // Add comments area
            responseBuilder.Append("      <tr align=\"left\" valign=\"top\"><td><br /><label for=\"add_notes\">Notes:</label></td>");
            responseBuilder.AppendLine("<td><textarea rows=\"10\" cols=\"" + actual_cols + "\" name=\"add_tag\" id=\"add_tag\" class=\"add_notes_textarea\" onfocus=\"javascript:textbox_enter('add_tag','add_notes_textarea_focused')\" onblur=\"javascript:textbox_leave('add_tag','add_notes_textarea')\"></textarea></td></tr>");
            responseBuilder.AppendLine("    </table>");
            responseBuilder.AppendLine("    <br />");
            responseBuilder.AppendLine("  </fieldset><br />");
            responseBuilder.AppendLine("  <div style=\"text-align:center; font-size:1.3em;\">");
            responseBuilder.AppendLine("    <button title=\"Cancel\" class=\"roundbutton\" onclick=\"return describe_item_form_close();\"> CANCEL </button> &nbsp; &nbsp; ");
            responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" type=\"submit\"> SAVE </button>");
            responseBuilder.AppendLine("  </div><br />");
            responseBuilder.AppendLine("</div>");
            responseBuilder.AppendLine();

            // Get the return string them
            string returnValue = responseBuilder.ToString();

            // If this was debug mode, then just write the tracer
            if (IsDebug)
            {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("DEBUG MODE DETECTED");
                Response.Output.WriteLine();
                Response.Output.WriteLine(tracer.Text_Trace);

                return;
            }

            // Get the JSON-P callback function
            string json_callback = "parseDescribeHtmlSnippet";
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
            {
                json_callback = QueryString["callback"];
            }

            // Use the base class to serialize the object according to request protocol
            Serialize(returnValue, Response, Protocol, json_callback);
        }

        /// <summary> Writes the small snippet of HTML to pop-up when the user selects the SHARE button </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Share_HTTP_Snippet(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            if (UrlSegments.Count > 1)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                // Get the BibID and VID
                string bibid = UrlSegments[0];
                string vid = (UrlSegments.Count > 1) ? UrlSegments[1] : "00001";

                tracer.Add_Trace("ItemServices.Share_HTTP_Snippet", "Get the full SobekCM_Item object for " + bibid + ":" + vid );
                SobekCM_Item sobekItem = getSobekItem(bibid, vid, tracer);

                // If this item was NULL, there was an error
                if (sobekItem == null)
                {
                    tracer.Add_Trace("ItemServices.Share_HTTP_Snippet", "Unable to retrieve the indicated digital resource ( " + bibid + ":" + vid + " )");

                    Response.ContentType = "text/plain";
                    Response.StatusCode = 500;
                    Response.Output.WriteLine("Unable to retrieve the indicated digital resource ( " + bibid + ":" + vid + " )");
                    Response.Output.WriteLine();

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }
                
                tracer.Add_Trace("ItemServices.Share_HTTP_Snippet", "Building the HTML response");

                // Build the response
                StringBuilder responseBuilder = new StringBuilder();

                // Calculate the title and url
                string title = HttpUtility.HtmlEncode(sobekItem.Bib_Info.Main_Title.Title);
                string share_url = Engine_ApplicationCache_Gateway.Settings.Servers.Base_URL + "/" + bibid + "/" + vid;

                responseBuilder.AppendLine("<!-- Share form -->");
                responseBuilder.AppendLine("<div id=\"shareform_content\">");

                responseBuilder.AppendLine("<a href=\"http://www.facebook.com/share.php?u=" + share_url + "&amp;t=" + title + "\" target=\"FACEBOOK_WINDOW\" onmouseover=\"facebook_share.src='" + Static_Resources_Gateway.Facebook_Share_H_Gif + "'\" onfocus=\"facebook_share.src='" + Static_Resources_Gateway.Facebook_Share_H_Gif + "'\" onmouseout=\"facebook_share.src='" + Static_Resources_Gateway.Facebook_Share_Gif + "'\" onblur=\"facebook_share.src='" + Static_Resources_Gateway.Facebook_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"facebook_share\" name=\"facebook_share\" src=\"" + Static_Resources_Gateway.Facebook_Share_Gif + "\" alt=\"FACEBOOK\" /></a>");
                responseBuilder.AppendLine("<a href=\"http://buzz.yahoo.com/buzz?targetUrl=" + share_url + "&amp;headline=" + title + "\" target=\"YAHOOBUZZ_WINDOW\" onmouseover=\"yahoobuzz_share.src='" + Static_Resources_Gateway.Yahoobuzz_Share_H_Gif + "'\" onfocus=\"yahoobuzz_share.src='" + Static_Resources_Gateway.Yahoobuzz_Share_H_Gif + "'\" onmouseout=\"yahoobuzz_share.src='" + Static_Resources_Gateway.Yahoobuzz_Share_Gif + "'\" onblur=\"yahoobuzz_share.src='" + Static_Resources_Gateway.Yahoobuzz_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoobuzz_share\" name=\"yahoobuzz_share\" src=\"" + Static_Resources_Gateway.Yahoobuzz_Share_Gif + "\" alt=\"YAHOO BUZZ\" /></a>");
                responseBuilder.AppendLine("<br />");

                responseBuilder.AppendLine("<a href=\"http://twitter.com/home?status=Currently reading " + share_url + "\" target=\"TWITTER_WINDOW\" onmouseover=\"twitter_share.src='" + Static_Resources_Gateway.Twitter_Share_H_Gif + "'\" onfocus=\"twitter_share.src='" + Static_Resources_Gateway.Twitter_Share_H_Gif + "'\" onmouseout=\"twitter_share.src='" + Static_Resources_Gateway.Twitter_Share_Gif + "'\" onblur=\"twitter_share.src='" + Static_Resources_Gateway.Twitter_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"twitter_share\" name=\"twitter_share\" src=\"" + Static_Resources_Gateway.Twitter_Share_Gif + "\" alt=\"TWITTER\" /></a>");
                responseBuilder.AppendLine("<a href=\"http://www.google.com/bookmarks/mark?op=add&amp;bkmk=" + share_url + "&amp;title=" + title + "\" target=\"GOOGLE_WINDOW\" onmouseover=\"google_share.src='" + Static_Resources_Gateway.Google_Share_H_Gif + "'\" onfocus=\"google_share.src='" + Static_Resources_Gateway.Google_Share_H_Gif + "'\" onmouseout=\"google_share.src='" + Static_Resources_Gateway.Google_Share_Gif + "'\" onblur=\"google_share.src='" + Static_Resources_Gateway.Google_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"google_share\" name=\"google_share\" src=\"" + Static_Resources_Gateway.Google_Share_Gif + "\" alt=\"GOOGLE SHARE\" /></a>");
                responseBuilder.AppendLine("<br />");

                responseBuilder.AppendLine("<a href=\"http://www.stumbleupon.com/submit?url=" + share_url + "&amp;title=" + title + "\" target=\"STUMBLEUPON_WINDOW\" onmouseover=\"stumbleupon_share.src='" + Static_Resources_Gateway.Stumbleupon_Share_H_Gif + "'\" onfocus=\"stumbleupon_share.src='" + Static_Resources_Gateway.Stumbleupon_Share_H_Gif + "'\" onmouseout=\"stumbleupon_share.src='" + Static_Resources_Gateway.Stumbleupon_Share_Gif + "'\" onblur=\"stumbleupon_share.src='" + Static_Resources_Gateway.Stumbleupon_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"stumbleupon_share\" name=\"stumbleupon_share\" src=\"" + Static_Resources_Gateway.Stumbleupon_Share_Gif + "\" alt=\"STUMBLEUPON\" /></a>");
                responseBuilder.AppendLine("<a href=\"http://myweb.yahoo.com/myresults/bookmarklet?t=" + title + "&amp;u=" + share_url + "\" target=\"YAHOO_WINDOW\" onmouseover=\"yahoo_share.src='" + Static_Resources_Gateway.Yahoo_Share_H_Gif + "'\" onfocus=\"yahoo_share.src='" + Static_Resources_Gateway.Yahoo_Share_H_Gif + "'\" onmouseout=\"yahoo_share.src='" + Static_Resources_Gateway.Yahoo_Share_Gif + "'\" onblur=\"yahoo_share.src='" + Static_Resources_Gateway.Yahoo_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoo_share\" name=\"yahoo_share\" src=\"" + Static_Resources_Gateway.Yahoo_Share_Gif + "\" alt=\"YAHOO SHARE\" /></a>");
                responseBuilder.AppendLine("<br />");

                responseBuilder.AppendLine("<a href=\"http://digg.com/submit?phase=2&amp;url=" + share_url + "&amp;title=" + title + "\" target=\"DIGG_WINDOW\" onmouseover=\"digg_share.src='" + Static_Resources_Gateway.Digg_Share_H_Gif + "'\" onfocus=\"digg_share.src='" + Static_Resources_Gateway.Digg_Share_H_Gif + "'\" onmouseout=\"digg_share.src='" + Static_Resources_Gateway.Digg_Share_Gif + "'\" onblur=\"digg_share.src='" + Static_Resources_Gateway.Digg_Share_Gif + "'\"  nclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"digg_share\" name=\"digg_share\" src=\"" + Static_Resources_Gateway.Digg_Share_Gif + "\" alt=\"DIGG\" /></a>");
                responseBuilder.AppendLine("<a onmouseover=\"favorites_share.src='" + Static_Resources_Gateway.Facebook_Share_H_Gif + "'\" onfocus=\"favorites_share.src='" + Static_Resources_Gateway.Facebook_Share_H_Gif + "'\" onmouseout=\"favorites_share.src='" + Static_Resources_Gateway.Facebook_Share_Gif + "'\" onblur=\"favorites_share.src='" + Static_Resources_Gateway.Facebook_Share_Gif + "'\" onclick=\"javascript:add_to_favorites();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"favorites_share\" name=\"favorites_share\" src=\"" + Static_Resources_Gateway.Facebook_Share_Gif + "\" alt=\"MY FAVORITES\" /></a>");
                responseBuilder.AppendLine("<br />");

                responseBuilder.AppendLine("</div>");
                responseBuilder.AppendLine();

                // Get the return string them
                string returnValue = responseBuilder.ToString();

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parseShareHtmlSnippet";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnValue, Response, Protocol, json_callback);
                return;
            }

            // Add the error
            Response.ContentType = "text/plain";
            Response.StatusCode = 400;
            Response.Output.WriteLine("BibID and VID are required parameters");
            Response.Output.WriteLine();
        }

        /// <summary> Writes the small snippet of HTML to pop-up when the user selects the ADD TO BOOKSHELF button </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void Bookshelf_HTTP_Snippet(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                // Try to get the user id
                int userid;
                if (!Int32.TryParse(UrlSegments[0], out userid))
                {
                    tracer.Add_Trace("ItemServices.Bookshelf_HTTP_Snippet", "UserID is not a valid integer");

                    Response.ContentType = "text/plain";
                    Response.StatusCode = 400;
                    Response.Output.WriteLine("UserID is not a valid integer");
                    Response.Output.WriteLine();

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                // Try to get the user information
                tracer.Add_Trace("ItemServices.Bookshelf_HTTP_Snippet", "Requested bookshelf HTML snippet for userid " + userid );
                User_Object thisUser = Engine_Database.Get_User(userid, tracer);

                // If null, respond
                if (thisUser == null)
                {
                    tracer.Add_Trace("ItemServices.Bookshelf_HTTP_Snippet", "User object returned was NULL.. Invalid UserID");

                    Response.ContentType = "text/plain";
                    Response.StatusCode = 400;
                    Response.Output.WriteLine("User object returned was NULL.. Invalid UserID");
                    Response.Output.WriteLine();

                    // If this was debug mode, then just write the tracer
                    if (IsDebug)
                    {
                        Response.Output.WriteLine("DEBUG MODE DETECTED");
                        Response.Output.WriteLine();
                        Response.Output.WriteLine(tracer.Text_Trace);
                    }
                    return;
                }

                tracer.Add_Trace("ItemServices.Bookshelf_HTTP_Snippet", "Building the HTML response");

                // Build the response
                StringBuilder responseBuilder = new StringBuilder();

                // Determine the number of columns for text areas, depending on browser
                int actual_cols = 50;
                //if ((!String.IsNullOrEmpty(CurrentMode.Browser_Type)) && (CurrentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0))
                //    actual_cols = 45;

                responseBuilder.AppendLine("<!-- Add to bookshelf form -->");
                responseBuilder.AppendLine("<div id=\"addform_content\" class=\"sbk_PopupForm\" style=\"width:530px;\">");
                responseBuilder.AppendLine("  <div class=\"sbk_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left;\">Add this item to your Bookshelf</td><td style=\"text-align:right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"add_item_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                responseBuilder.AppendLine("  <br />");
                responseBuilder.AppendLine("  <fieldset><legend>Enter notes for this item in your bookshelf &nbsp; </legend>");
                responseBuilder.AppendLine("    <br />");
                responseBuilder.AppendLine("    <table class=\"sbk_PopupTable\">");


                // Add bookshelf choices
                responseBuilder.Append("      <tr><td style=\"width:80px\"><label for=\"add_bookshelf\">Bookshelf:</label></td>");
                responseBuilder.Append("<td><select class=\"email_bookshelf_input\" name=\"add_bookshelf\" id=\"add_bookshelf\">");

                foreach (User_Folder folder in thisUser.All_Folders)
                {
                    if (folder.Folder_Name.Length > 80)
                    {
                        responseBuilder.Append("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\">" + HttpUtility.HtmlEncode(folder.Folder_Name.Substring(0, 75)) + "...</option>");
                    }
                    else
                    {
                        if (folder.Folder_Name != "Submitted Items")
                        {
                            if (folder.Folder_Name == "My Bookshelf")
                                responseBuilder.Append("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(folder.Folder_Name) + "</option>");
                            else
                                responseBuilder.Append("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\">" + HttpUtility.HtmlEncode(folder.Folder_Name) + "</option>");
                        }
                    }
                }
                responseBuilder.AppendLine("</select></td></tr>");

                // Add comments area
                responseBuilder.Append("      <tr style=\"vertical-align:top\"><td><br /><label for=\"add_notes\">Notes:</label></td>");
                responseBuilder.AppendLine("<td><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"add_notes\" id=\"add_notes\" class=\"add_notes_textarea\" onfocus=\"javascript:textbox_enter('add_notes','add_notes_textarea_focused')\" onblur=\"javascript:textbox_leave('add_notes','add_notes_textarea')\"></textarea></td></tr>");
                responseBuilder.AppendLine("      <tr style=\"vertical-align:top\"><td>&nbsp;</td><td><input type=\"checkbox\" id=\"open_bookshelf\" name=\"open_bookshelf\" value=\"open\" /> <label for=\"open_bookshelf\">Open bookshelf in new window</label></td></tr>");
                responseBuilder.AppendLine("    </table>");
                responseBuilder.AppendLine("    <br />");
                responseBuilder.AppendLine("  </fieldset><br />");
                responseBuilder.AppendLine("  <div style=\"text-align:center; font-size:1.3em;\">");
                responseBuilder.AppendLine("    <button title=\"Cancel\" class=\"roundbutton\" onclick=\"return add_item_form_close();\"> CANCEL </button> &nbsp; &nbsp; ");
                responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" type=\"submit\"> SAVE </button>");
                responseBuilder.AppendLine("  </div><br />");
                responseBuilder.AppendLine("</div>");
                responseBuilder.AppendLine();

                // Get the return string them
                string returnValue = responseBuilder.ToString();

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);

                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "parseBookshelfHtmlSnippet";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnValue, Response, Protocol, json_callback);
                return;
            }

            // Add the error
            Response.ContentType = "text/plain";
            Response.StatusCode = 400;
            Response.Output.WriteLine("UserID is a required parameter");
            Response.Output.WriteLine();
        }

        #endregion

    }
}
