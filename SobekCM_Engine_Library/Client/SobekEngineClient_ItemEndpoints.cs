#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Core.BriefItem;
using SobekCM.Core.EAD;
using SobekCM.Core.Items;
using SobekCM.Core.MARC;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Engine_Library.Endpoints;
using SobekCM.Resource_Object;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to all the item-related endpoints exposed by the SobekCM engine </summary>
    public class SobekEngineClient_ItemEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_ItemEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_ItemEndpoints(MicroservicesClient_Configuration ConfigObj) : base(ConfigObj)
        {
            // All work done in the base constructor
        }

        /// <summary> Gets the brief digital resource object, by BibID_VID </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for the digital resource to retrieve </param>
        /// <param name="VID"> Volume identifier (VID) for the digital resource to retrieve </param>
        /// <param name="UseCache"> Flag indicates if the cache should be used to check for a built copy or store the final product </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built brief digital item object </returns>
        public BriefItemInfo Get_Item_Brief(string BibID, string VID, bool UseCache, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_Brief", "Get brief item information by bibid/vid");

            // Look in the cache
            if ((Config.UseCache) && (UseCache))
            {
                BriefItemInfo fromCache = CachedDataManager.Items.Retrieve_Brief_Digital_Resource_Object(BibID, VID, Tracer);
                if (fromCache != null)
                {
                    Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_Item_Brief", "Found brief item in the local cache");
                    return fromCache;
                }
            }
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Items.GetItemBrief", Tracer);

            // Format the URL
            string url = String.Format(endpoint.URL, BibID, VID);

            // Call out to the endpoint and deserialize the object
            BriefItemInfo returnValue = Deserialize<BriefItemInfo>(url, endpoint.Protocol, Tracer);

            // Add to the local cache
            if ((Config.UseCache) && (UseCache) && (returnValue != null))
            {
                Tracer.Add_Trace("SobekEngineClient_WebContentServices.Get_Item_Brief", "Store brief item in the local cache");
                CachedDataManager.Items.Store_Brief_Digital_Resource_Object(BibID, VID, returnValue, Tracer);
            }

            // Return the object
            return returnValue;
        }

        /// <summary> Gets the special EAD information related to a digital resource object, by BibID_VID </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for the digital resource to retrieve </param>
        /// <param name="VID"> Volume identifier (VID) for the digital resource to retrieve </param>
        /// <param name="UseCache"> Flag indicates if the cache should be used to check for a built copy or store the final product </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built EAD information, or NULL </returns>
        public EAD_Transfer_Object Get_Item_EAD(string BibID, string VID, bool UseCache, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_EAD", "Get EAD information by bibid/vid");

            // Look in the cache
            if ((Config.UseCache) && (UseCache))
            {
                EAD_Transfer_Object fromCache = CachedDataManager.Items.Retrieve_EAD_Info(BibID, VID, Tracer);
                if (fromCache != null)
                {
                    Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_EAD", "Found EAD informationin the local cache");
                    return fromCache;
                }
            }
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Items.GetItemEAD", Tracer);

            // Format the URL
            string url = String.Format(endpoint.URL, BibID, VID);

            // Call out to the endpoint and deserialize the object
            EAD_Transfer_Object returnValue = Deserialize<EAD_Transfer_Object>(url, endpoint.Protocol, Tracer);

            // Add to the local cache
            if ((Config.UseCache) && (UseCache) && (returnValue != null))
            {
                Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_EAD", "Store EAD information in the local cache");
                CachedDataManager.Items.Store_EAD_Info(BibID, VID, returnValue, Tracer);
            }

            // Return the object
            return returnValue;
        }


        /// <summary> Gets the digital resource, mapped to a MARC record object, by BibID_VID </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for the digital resource to retrieve </param>
        /// <param name="VID"> Volume identifier (VID) for the digital resource to retrieve </param>
        /// <param name="UseCache"> Flag indicates if the cache should be used to check for a built copy or store the final product </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built MARC record, or NULL </returns>
        public MARC_Transfer_Record Get_Item_MARC_Record(string BibID, string VID, bool UseCache, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_MARC_Record", "Get MARC record object by bibid/vid");

            // Look in the cache
            if ((Config.UseCache) && (UseCache))
            {
                MARC_Transfer_Record fromCache = CachedDataManager.Items.Retrieve_MARC_Record(BibID, VID, Tracer);
                if (fromCache != null)
                {
                    Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_MARC_Record", "Found MARC record the local cache");
                    return fromCache;
                }
            }
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Items.GetItemMarcRecord", Tracer);

            // Format the URL
            string url = String.Format(endpoint.URL, BibID, VID);

            // Call out to the endpoint and deserialize the object
            MARC_Transfer_Record returnValue = Deserialize<MARC_Transfer_Record>(url, endpoint.Protocol, Tracer);

            // Add to the local cache
            if ((Config.UseCache) && (UseCache) && (returnValue != null))
            {
                Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_MARC_Record", "Store MARC record in the local cache");
                CachedDataManager.Items.Store_MARC_Record(BibID, VID, returnValue, Tracer);
            }

            // Return the object
            return returnValue;
        }

        /// <summary> Gets the month-by-month usage for an item </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for the digital resource </param>
        /// <param name="VID"> Volume identifier (VID) for the digital resource </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built month-by-month usage for the item </returns>
        public List<Item_Monthly_Usage> Get_Item_Statistics_History(string BibID, string VID, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_Statistics_History", "Get monthly usage for an item by bibid/vid");

            // Look in the cache
            if (Config.UseCache)
            {
                List<Item_Monthly_Usage> fromCache = CachedDataManager.Items.Retrieve_Item_Usage(BibID, VID, Tracer);
                if (fromCache != null)
                {
                    Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_Statistics_History", "Found monthly usage on the local cache");
                    return fromCache;
                }
            }
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Items.GetItemStatisticsHistory", Tracer);

            // Format the URL
            string url = String.Format(endpoint.URL, BibID, VID);

            // Call out to the endpoint and deserialize the object
            List<Item_Monthly_Usage> returnValue = Deserialize<List<Item_Monthly_Usage>>(url, endpoint.Protocol, Tracer);

            // Add to the local cache
            if ((Config.UseCache) && (returnValue != null))
            {
                Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_Statistics_History", "Store monthly usage on the local cache");
                CachedDataManager.Items.Store_Item_Usage(BibID, VID, returnValue, Tracer);
            }

            // Return the object
            return returnValue;
        }

        /// <summary> Gets the work history for a single digital resource item </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for the digital resource </param>
        /// <param name="VID"> Volume identifier (VID) for the digital resource </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built month-by-month usage for the item </returns>
        public Item_Tracking_Details Get_Item_Tracking_Work_History(string BibID, string VID, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_Tracking_Work_History", "Get work history for an item by bibid/vid");

            // Look in the cache
            if (Config.UseCache)
            {
                Item_Tracking_Details fromCache = CachedDataManager.Items.Retrieve_Item_Tracking(BibID, VID, Tracer);
                if (fromCache != null)
                {
                    Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_Tracking_Work_History", "Found work history on the local cache");
                    return fromCache;
                }
            }
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Items.GetItemTrackingWorkHistory", Tracer);

            // Format the URL
            string url = String.Format(endpoint.URL, BibID, VID);

            // Call out to the endpoint and deserialize the object
            Item_Tracking_Details returnValue = Deserialize<Item_Tracking_Details>(url, endpoint.Protocol, Tracer);

            // Add to the local cache
            if ((Config.UseCache) && (returnValue != null))
            {
                Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Item_Tracking_Work_History", "Store work history on the local cache");
                CachedDataManager.Items.Store_Item_Tracking(BibID, VID, returnValue, Tracer);
            }

            // Return the object
            return returnValue;
        }

        /// <summary> Gets the list of items (VIDs) under a single title (BibID) </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for the digital resources </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built list of all the items under that bibid </returns>
        public List<Item_Hierarchy_Details> Get_Multiple_Volumes(string BibID, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Multiple_Volumes", "Get list of items under a single BibID");

            // Look in the cache
            if (Config.UseCache)
            {
                List<Item_Hierarchy_Details> fromCache = CachedDataManager.Items.Retrieve_Item_List(BibID, Tracer);
                if (fromCache != null)
                {
                    Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Multiple_Volumes", "Found list of items on the local cache");
                    return fromCache;
                }
            }
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Items.GetMultipleVolumes", Tracer);

            // Format the URL
            string url = String.Format(endpoint.URL, BibID);

            // Call out to the endpoint and deserialize the object
            List<Item_Hierarchy_Details> returnValue = Deserialize<List<Item_Hierarchy_Details>>(url, endpoint.Protocol, Tracer);

            // Add to the local cache
            if ((Config.UseCache) && (returnValue != null))
            {
                Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Get_Multiple_Volumes", "Store list of items on the local cache");
                CachedDataManager.Items.Store_Item_List(BibID, returnValue, Tracer);
            }

            // Return the object
            return returnValue;
        }

        /// <summary> Clears the engine cache of all items related to a particular digital resource </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for the digital resources </param>
        /// <param name="VID"> Volume identifier (VID) for the digital resource </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Clear_Item_Cache(string BibID, string VID, Custom_Tracer Tracer)
        {
            // Add a beginning trace
            Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Clear_Item_Cache", "Clear item cache for " + BibID + ":" + VID );

            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("Items.ClearCache", Tracer);

            // Format the URL
            string url = String.Format(endpoint.URL, BibID, VID);

            // Call out to the endpoint and deserialize the object
            string returnValue = Deserialize<string>(url, endpoint.Protocol, Tracer);

            // Add the message to the trace
            Tracer.Add_Trace("SobekEngineClient_ItemEndpoints.Clear_Item_Cache", "Response: " + returnValue );
        }



        public SobekCM_Item Get_Sobek_Item(string BibID, string VID, Custom_Tracer Tracer)
        {
            ItemServices srvcs = new ItemServices();
            return srvcs.getSobekItem(BibID, VID, Tracer);
        }

        public SobekCM_Item Get_Sobek_Item(string BibID, string VID, int UserID, Custom_Tracer Tracer)
        {
            ItemServices srvcs = new ItemServices();
            return srvcs.getSobekItem(BibID, VID, UserID, Tracer);
        }

        public SobekCM_Item Get_Sobek_Item_Group(string BibID, Custom_Tracer Tracer)
        {
            ItemServices srvcs = new ItemServices();
            return srvcs.getSobekItemGroup(BibID, Tracer);
        }
    }
}
