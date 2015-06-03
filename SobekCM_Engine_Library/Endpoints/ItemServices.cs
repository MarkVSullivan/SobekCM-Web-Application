#region Using references

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Jil;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Items;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Resource_Object;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
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
        /// <param name="Protocol"></param>
        public void GetItemCitation(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
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
                        BriefItemInfo returnValue = getBriefItem(bibid, vid, null, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (QueryString["debug"] == "debug")
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
                        if (QueryString["debug"] == "debug")
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
                        tracer.Add_Trace("ItemServices.GetItemCitation", "Requested VID 0000 - Currently invalid");

                        // If this was debug mode, then just write the tracer
                        if (QueryString["debug"] == "debug")
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
                    if (QueryString["debug"] == "debug")
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


        /// <summary> Gets the information about a single digital resource </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetItemBrief(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
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

                    tracer.Add_Trace("ItemServices.GetItemBrief", "Requested brief item info for " + bibid + ":" + vid);

                    if ((vid.Length > 0) && (vid != "00000"))
                    {
                        tracer.Add_Trace("ItemServices.GetItemBrief", "Build full brief item");
                        BriefItemInfo returnValue = getBriefItem(bibid, vid, null, tracer);

                        // Was the item null?
                        if (returnValue == null)
                        {
                            // If this was debug mode, then just write the tracer
                            if (QueryString["debug"] == "debug")
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
                        if (QueryString["debug"] == "debug")
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
                        tracer.Add_Trace("ItemServices.GetItemBrief", "Requested VID 0000 - Currently invalid");

                        // If this was debug mode, then just write the tracer
                        if (QueryString["debug"] == "debug")
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
                    if (QueryString["debug"] == "debug")
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

        private SobekCM_Item getSobekItem(string BibID, string VID, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("ItemServices.getSobekItem", "Get the Single_Item object from the Item_Lookup_Object from the cache");
            Single_Item selected_item = Engine_ApplicationCache_Gateway.Items.Item_By_Bib_VID(BibID, VID, Tracer);

            // If this is for a single item, return that
            if (selected_item != null)
            {
                // Try to get this from the cache
                SobekCM_Item currentItem = CachedDataManager.Retrieve_Digital_Resource_Object(BibID, VID, Tracer);

                // If not pulled from the cache, then we will have to build the item
                if (currentItem == null)
                {
                    Tracer.Add_Trace("ItemServices.getSobekItem", "Unable to find the digital resource on the cache.. will build");
                    currentItem = SobekCM_Item_Factory.Get_Item(BibID, VID, Engine_ApplicationCache_Gateway.Icon_List, Engine_ApplicationCache_Gateway.Item_Viewer_Priority, Tracer);
                    if (currentItem != null)
                    {
                        Tracer.Add_Trace("ItemServices.getSobekItem", "Store the digital resource object to the cache");
                        CachedDataManager.Store_Digital_Resource_Object(BibID, VID, currentItem, Tracer);
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
            else
            {
                Tracer.Add_Trace("ItemServices.getSobekItem", "Could not locate the object from the Item_Lookup_Object.. may not be a valid bibid/vid combination");
            }

            return null;
        }

        private BriefItemInfo getBriefItem(string BibID, string VID, string MappingSet, Custom_Tracer Tracer)
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
                BriefItemInfo item = Items.BriefItems.BriefItem_Factory.Create(currentItem, Tracer);
                return item;
            }

            Tracer.Add_Trace("ItemServices.getBriefItem", "Map to the brief item, using mapping set '" + MappingSet + "'");
            BriefItemInfo item2 = Items.BriefItems.BriefItem_Factory.Create(currentItem, MappingSet, Tracer);
            return item2;
        }

        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetRandomItem(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON) || ( Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P ))
            {
                Tuple<string, string> result = Engine_Database.Get_Random_Item(null);

                if (result != null)
                {
                    if ( Protocol == Microservice_Endpoint_Protocol_Enum.JSON )
                        JSON.Serialize(new { bibid = result.Item1, vid = result.Item2 }, Response.Output, Options.ISO8601ExcludeNulls);
                    if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P)
                    {
                        // Get the JSON-P callback function
                        string json_callback = "parseRandomItem";
                        if (!String.IsNullOrEmpty(QueryString["callback"]))
                        {
                            json_callback = QueryString["callback"];
                        }

                        Response.Output.Write( json_callback + "(");
                        JSON.Serialize(new {bibid = result.Item1, vid = result.Item2}, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                        Response.Output.Write(");");
                    }
                }
            }
        }

        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetItemRdf(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
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
                    Dictionary<string, object> options_rdf = new Dictionary<string, object>();
                    options_rdf["DC_File_ReaderWriter:RDF_Style"] = true;
                    DC_File_ReaderWriter rdfWriter = new DC_File_ReaderWriter();
                    rdfWriter.Write_Metadata(Response.Output, sobekItem, options_rdf, out errorMessage);
                }
            }
        }

        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetItemXml(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol)
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
                            options["MarcXML_File_ReaderWriter:System Name"] = Engine_ApplicationCache_Gateway.Settings.System_Name;
                            options["MarcXML_File_ReaderWriter:System Abbreviation"] = Engine_ApplicationCache_Gateway.Settings.System_Abbreviation;


                            MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                            marcWriter.Write_Metadata(Response.Output, sobekItem, options, out errorMessage);
                            break;
                    }
                }
            }
        }
    }
}
