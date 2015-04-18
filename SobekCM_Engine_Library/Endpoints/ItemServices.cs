using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Jil;
using ProtoBuf;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Items;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Resource_Object;
using SobekCM.Core.BriefItem;
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

                    if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                    {
                        JSON.Serialize(new { bibid=returnValue.BibID, vid = returnValue.VID, description=returnValue.Description, title=returnValue.Title }, Response.Output, Options.ISO8601ExcludeNulls);
                    }
                    else
                    {
                        Serializer.Serialize(Response.OutputStream, new { bibid = returnValue.BibID, vid = returnValue.VID, description = returnValue.Description, title = returnValue.Title });
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
        }

        private BriefItemInfo getBriefItem(string BibID, string VID, string MappingSet, Custom_Tracer Tracer)
        {
            Single_Item selected_item = Engine_ApplicationCache_Gateway.Items.Item_By_Bib_VID(BibID, VID, Tracer);

            // If this is for a single item, return that
            if (selected_item != null)
            {

                // Try to get this from the cache
                SobekCM_Item Current_Item = CachedDataManager.Retrieve_Digital_Resource_Object(BibID, VID, Tracer);

                // If not pulled from the cache, then we will have to build the item
                if (Current_Item == null)
                {
                    Current_Item = SobekCM_Item_Factory.Get_Item(BibID, VID, Engine_ApplicationCache_Gateway.Icon_List, Engine_ApplicationCache_Gateway.Item_Viewer_Priority, Tracer);
                    if (Current_Item != null)
                    {
                        CachedDataManager.Store_Digital_Resource_Object(BibID, VID, Current_Item, Tracer);
                    }
                    else
                    {
                        return null;
                    }
                }

                if (String.IsNullOrEmpty(MappingSet))
                {
                    BriefItemInfo item = Items.BriefItems.BriefItem_Factory.Create(Current_Item);
                    return item;
                }
                else
                {
                    BriefItemInfo item2 = Items.BriefItems.BriefItem_Factory.Create(Current_Item, MappingSet);
                    return item2;
                }
            }

            return null;
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
    }
}
