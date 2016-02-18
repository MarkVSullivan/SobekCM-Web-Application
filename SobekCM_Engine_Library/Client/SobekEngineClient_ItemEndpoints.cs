#region Using directives

using System;
using SobekCM.Core.BriefItem;
using SobekCM.Core.EAD;
using SobekCM.Core.MARC;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.MicroservicesClient;
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
    }
}
