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
using SobekCM.Engine_Library.Items;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Resource_Object;
using SobekCM.Rest_API.BriefItem;
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
            if (UrlSegments.Count > 1)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                string bibid = UrlSegments[0];
                string vid = UrlSegments[1];

                // Try to get the very basic information about this item, to determine if the 
                // bib / vid combination is valid

                if ((vid.Length > 0) && (vid != "00000"))
                {
                    Single_Item selected_item = Engine_ApplicationCache_Gateway.Items.Item_By_Bib_VID(bibid, vid, tracer);

                    // If this is for a single item, return that
                    if (selected_item != null)
                    {

                        // Try to get this from the cache
                        SobekCM_Item Current_Item = CachedDataManager.Retrieve_Digital_Resource_Object(bibid, vid, tracer);

                        // If not pulled from the cache, then we will have to build the item
                        if (Current_Item == null)
                        {
                            Current_Item = SobekCM_Item_Factory.Get_Item(bibid, vid, Engine_ApplicationCache_Gateway.Icon_List, Engine_ApplicationCache_Gateway.Item_Viewer_Priority, tracer);
                            if (Current_Item != null)
                            {
                                CachedDataManager.Store_Digital_Resource_Object(bibid, vid, Current_Item, tracer);
                            }
                            else
                            {
                                return;
                            }
                        }

                        BriefItemInfo returnValue = Items.BriefItems.BriefItem_Factory.Create(Current_Item);

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
        }
    }
}
