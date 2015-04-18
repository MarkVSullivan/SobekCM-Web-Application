using System;
using System.Collections.Generic;
using System.Web;
using Jil;
using ProtoBuf;
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

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Class supports all the item-level services provided by the SobekCM engine </summary>
    public class ItemServices
    {

        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetItemCitation(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                string bibid = UrlSegments[0];
                string vid = "00001";

                if (UrlSegments.Count > 1)
                    vid = UrlSegments[1];

                if ((vid.Length > 0) && (vid != "00000"))
                {
                    BriefItemInfo returnValue = getBriefItem(bibid, vid, null, tracer);

                    if (returnValue == null)
                        return;

                    switch (Protocol)
                    {
                        case Microservice_Endpoint_Protocol_Enum.JSON:
                            JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                            break;

                        case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                            Serializer.Serialize(Response.OutputStream, returnValue);
                            break;

                        case Microservice_Endpoint_Protocol_Enum.JSON_P:
                            Response.Output.Write("parseItemCitation(");
                            JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                            Response.Output.Write(");");
                            break;
                    }
                }
            }
        }


        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetItemBrief(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                string bibid = UrlSegments[0];
                string vid = "00001";

                if ( UrlSegments.Count > 1 )
                    vid = UrlSegments[1];

                if ((vid.Length > 0) && (vid != "00000"))
                {
                    BriefItemInfo returnValue = getBriefItem(bibid, vid, null, tracer);

                    if (returnValue == null)
                        return;

                    switch (Protocol)
                    {
                        case Microservice_Endpoint_Protocol_Enum.JSON:
                            JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                            break;

                        case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                            Serializer.Serialize(Response.OutputStream, returnValue);
                            break;

                        case Microservice_Endpoint_Protocol_Enum.JSON_P:
                            Response.Output.Write("parseItemBrief(");
                            JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                            Response.Output.Write(");");
                            break;
                    }
                }
            }
        }

        private SobekCM_Item getSobekItem(string BibID, string VID, Custom_Tracer Tracer)
        {
            Single_Item selected_item = Engine_ApplicationCache_Gateway.Items.Item_By_Bib_VID(BibID, VID, Tracer);

            // If this is for a single item, return that
            if (selected_item != null)
            {

                // Try to get this from the cache
                SobekCM_Item currentItem = CachedDataManager.Retrieve_Digital_Resource_Object(BibID, VID, Tracer);

                // If not pulled from the cache, then we will have to build the item
                if (currentItem == null)
                {
                    currentItem = SobekCM_Item_Factory.Get_Item(BibID, VID, Engine_ApplicationCache_Gateway.Icon_List, Engine_ApplicationCache_Gateway.Item_Viewer_Priority, Tracer);
                    if (currentItem != null)
                    {
                        CachedDataManager.Store_Digital_Resource_Object(BibID, VID, currentItem, Tracer);
                    }
                    else
                    {
                        return null;
                    }
                }

                return currentItem;
            }

            return null;
        }

        private BriefItemInfo getBriefItem(string BibID, string VID, string MappingSet, Custom_Tracer Tracer)
        {
            SobekCM_Item currentItem = getSobekItem(BibID, VID, Tracer);
            if (currentItem == null)
                return null;

            if (String.IsNullOrEmpty(MappingSet))
            {
                BriefItemInfo item = Items.BriefItems.BriefItem_Factory.Create(currentItem);
                return item;
            }
            
            BriefItemInfo item2 = Items.BriefItems.BriefItem_Factory.Create(currentItem, MappingSet);
            return item2;
        }

        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetRandomItem(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
            {
                Tuple<string, string> result = Engine_Database.Get_Random_Item(null);

                if (result != null)
                {
                    JSON.Serialize(new { bibid = result.Item1, vid = result.Item2 }, Response.Output, Options.ISO8601ExcludeNulls);
                }
            }
        }

        /// <summary> Gets the complete (language agnostic) web skin, by web skin code </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        public void GetItemRdf(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
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
        public void GetItemXml(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
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
