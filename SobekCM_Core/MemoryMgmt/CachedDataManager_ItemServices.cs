using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.BriefItem;
using SobekCM.Core.EAD;
using SobekCM.Core.Items;
using SobekCM.Core.MARC;
using SobekCM.Resource_Object;
using SobekCM.Tools;

namespace SobekCM.Core.MemoryMgmt
{
    /// <summary> Item-specific services for the Cached Data Manager, which allows items to be 
    /// cached, or portions of items and REST reponses related to items to be cached </summary>
    public class CachedDataManager_ItemServices
    {
        private readonly CachedDataManager_Settings settings;

        /// <summary> Constructor for a new instance of the <see cref="CachedDataManager_ItemServices"/> class. </summary>
        /// <param name="Settings"> Cached data manager settings object </param>
        public CachedDataManager_ItemServices(CachedDataManager_Settings Settings)
        {
            settings = Settings;
        }

        #region Static methods relating to storing and retrieving digital resource objects

        /// <summary> Removes all digital resource objects from the cache , for a given BibID </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resources to remove </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Remove_Digital_Resource_Objects(string BibID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Remove_Digital_Resource_Objects", "");
            }

            string key_start = "ITEM_" + BibID + "_";

            // Build the sorted list of locally cached stuff
            List<Cached_Object_Info> locallyCached = (from DictionaryEntry thisItem in HttpContext.Current.Cache select new Cached_Object_Info(thisItem.Key.ToString(), thisItem.Value.GetType())).ToList();

            // Determine which keys to expire
            List<string> keys_to_expire = (from cachedObject in locallyCached where cachedObject.Object_Key.IndexOf(key_start) == 0 select cachedObject.Object_Key).ToList();

            // Clear these from the local cache
            foreach (string expireKey in keys_to_expire)
            {
                HttpContext.Current.Cache.Remove(expireKey);
            }

            // Now, remove the actual item group
            string key = "ITEM_GROUP_" + BibID;

            // Clear the item group from the local cache
            HttpContext.Current.Cache.Remove(key);
        }

        /// <summary> Removes a global digital resource object from the cache , it it exists </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to remove </param>
        /// <param name="VID"> Volume Identifier for the digital resource to remove </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Remove_Digital_Resource_Object(string BibID, string VID, Custom_Tracer Tracer)
        {
            Remove_Digital_Resource_Object(-1, BibID, VID, Tracer);
        }

        /// <summary> Removes a user-specific digital resource object from the cache , it it exists </summary>
        /// <param name="UserID"> Primary key of the user, if this should be removed from the user-specific cache</param>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to remove </param>
        /// <param name="VID"> Volume Identifier for the digital resource to remove </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Remove_Digital_Resource_Object(int UserID, string BibID, string VID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Remove_Digital_Resource_Object", "");
            }

            string key_start = "ITEM_" + BibID + "_" + VID + "_";

            // Build the sorted list of locally cached stuff
            List<Cached_Object_Info> locallyCached = (from DictionaryEntry thisItem in HttpContext.Current.Cache select new Cached_Object_Info(thisItem.Key.ToString(), thisItem.Value.GetType())).ToList();

            // Determine which keys to expire
            List<string> keys_to_expire = (from cachedObject in locallyCached where cachedObject.Object_Key.IndexOf(key_start) == 0 select cachedObject.Object_Key).ToList();

            // Clear these from the local cache
            foreach (string expireKey in keys_to_expire)
            {
                HttpContext.Current.Cache.Remove(expireKey);
            }

            // Now, remove the actual item
            string key = "ITEM_" + BibID + "_" + VID;
            if (UserID > 0)
            {
                key = "USERITEM" + UserID + "_" + key;
            }

            // Clear this from the local cache
            HttpContext.Current.Cache.Remove(key);
        }


        /// <summary> Retrieves a user-specific digital resource object from the cache  </summary>
        /// <param name="UserID"> Primary key of the user, if this should be pulled from the user-specific cache </param>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
        /// <param name="VID"> Volume Identifier for the digital resource to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the digital resource object from the cache  </returns>
        public SobekCM_Item Retrieve_Digital_Resource_Object(int UserID, string BibID, string VID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            // Determine the key
            string key = "ITEM_" + BibID + "_" + VID;
            if (UserID > 0)
            {
                key = "USERITEM" + UserID + "_" + key;
            }

            // See if this is in the local cache first
            object returnValue = HttpContext.Current.Cache.Get(key);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_Digital_Resource_Object", "Found item on local cache");
                }

                return (SobekCM_Item)returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_Digital_Resource_Object", "Item not found in either the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Retrieves a global digital resource object from the cache  </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
        /// <param name="VID"> Volume Identifier for the digital resource to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the digital resource object from the cache  </returns>
        public SobekCM_Item Retrieve_Digital_Resource_Object(string BibID, string VID, Custom_Tracer Tracer)
        {
            return Retrieve_Digital_Resource_Object(-1, BibID, VID, Tracer);
        }


        /// <summary> Store a user-specific digital resource object on the cache   </summary>
        /// <param name="UserID"> Primary key of the user, if this should be stored in a user-specific cache </param>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
        /// <param name="VID"> Volume Identifier for the digital resource to store </param>
        /// <param name="StoreObject"> Digital Resource object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Digital_Resource_Object(int UserID, string BibID, string VID, SobekCM_Item StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            // Determine the key
            string key = "ITEM_" + BibID + "_" + VID;
            if (UserID > 0)
            {
                key = "USERITEM" + UserID + "_" + key;
            }

            int length_of_time = 1;
            if (UserID > 0)
                length_of_time = 15;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Store_Digital_Resource_Object", "Adding object '" + key + "' to the local cache with expiration of " + length_of_time + " minute");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(length_of_time));
        }


        /// <summary> Store a global digital resource object on the cache   </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
        /// <param name="VID"> Volume Identifier for the digital resource to store </param>
        /// <param name="StoreObject"> Digital Resource object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Digital_Resource_Object(string BibID, string VID, SobekCM_Item StoreObject, Custom_Tracer Tracer)
        {
            Store_Digital_Resource_Object(-1, BibID, VID, StoreObject, Tracer);
        }

        /// <summary> Retrieves the title-level object from the cache  </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the digital resource object from the cache  </returns>
        public SobekCM_Item Retrieve_Digital_Resource_Object(string BibID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            // Determine the key
            string key = "ITEM_GROUP_" + BibID;

            // See if this is in the local cache first
            object returnValue = HttpContext.Current.Cache.Get(key);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_Digital_Resource_Object", "Found item on local cache");
                }

                return (SobekCM_Item)returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_Digital_Resource_Object", "Item not found in either the local cache ");
            }

            // Since everything failed, just return null
            return null;

        }

        /// <summary> Stores the title-level digital resource object on the cache   </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
        /// <param name="StoreObject"> Digital Resource object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Digital_Resource_Object(string BibID, SobekCM_Item StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            // Determine the key
            string key = "ITEM_GROUP_" + BibID;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Store_Digital_Resource_Object", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
        }

        #endregion

        #region Static methods relating to storing and retrieving the transfer (BriefItemInfo) digital resource objects

        /// <summary> Retrieves the brief (transfer) digital resource object from the cache  </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
        /// <param name="VID"> Volume Identifier for the digital resource to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the brief (transfer) digital resource object from the cache  </returns>
        public BriefItemInfo Retrieve_Brief_Digital_Resource_Object(string BibID, string VID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            // Determine the key
            string key = "ITEM_" + BibID + "_" + VID + "_Brief";

            // See if this is in the local cache first
            object returnValue = HttpContext.Current.Cache.Get(key);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_Brief_Digital_Resource_Object", "Found brief item on local cache");
                }

                return (BriefItemInfo)returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_Brief_Digital_Resource_Object", "Brief item not found in either the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Store a brief (transfer) digital resource object on the cache   </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
        /// <param name="VID"> Volume Identifier for the digital resource to store </param>
        /// <param name="StoreObject"> Digital Resource object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Brief_Digital_Resource_Object(string BibID, string VID, BriefItemInfo StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            // Determine the key
            string key = "ITEM_" + BibID + "_" + VID + "_Brief";
            const int LENGTH_OF_TIME = 15;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Store_Digital_Resource_Object", "Adding object '" + key + "' to the local cache with expiration of " + LENGTH_OF_TIME + " minute");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LENGTH_OF_TIME));
        }

        #endregion

        #region Static methods relating to storing and retrieving lists of digital volumes within one title

        /// <summary> Retrieves the list of items for a single bibid from the cache  </summary>
        /// <param name="BibID"> Bibliographic identifier for the list of items </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the list of items within a single title </returns>
        public SobekCM_Items_In_Title Retrieve_Items_In_Title(string BibID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_Items_In_Title", "");
            }

            // Try to get this from the local cache next
            object returnValue = HttpContext.Current.Cache.Get("ITEMLIST_" + BibID);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_Items_In_Title", "List of items in title pulled from local cache");
                }

                return (SobekCM_Items_In_Title)returnValue;
            }

            return null;
        }

        /// <summary> Stores the list of items for a single bibid to the cache  </summary>
        /// <param name="BibID"> Bibliographic identifier for the list of items </param>
        /// <param name="StoreObject"> List of items within the single title </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Items_In_Title(string BibID, SobekCM_Items_In_Title StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Store_Items_In_Title", "");
            }

            // Store this on the local cache, if not there and storing on the cache server failed
            string key = "ITEMLIST_" + BibID;
            if (HttpContext.Current.Cache[key] == null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_ItemServices.Store_Items_In_Title", "Adding object '" + key + "' to the local cache with expiration of 1 minutes");
                }

                HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
            }
        }

        /// <summary> Removes the  list of items for a single bibid to the cache  </summary>
        /// <param name="BibID"> Bibliographic identifier for the list of items </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Remove_Items_In_Title(string BibID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Remove_Items_In_Title", "");
            }

            // Store this on the local cache, if not there and storing on the cache server failed
            string key = "ITEMLIST_" + BibID;

            // Clear this from the local cache
            HttpContext.Current.Cache.Remove(key);
        }

        #endregion

        #region Methods related to storing and retrieving EAD information for a single digital resource
        
        /// <summary> Retrieves the EAD information related to a digital resource  </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
        /// <param name="VID"> Volume Identifier for the digital resource to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the EAD information from the cache  </returns>
        public EAD_Transfer_Object Retrieve_EAD_Info(string BibID, string VID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            // Determine the key
            string key = "ITEM_" + BibID + "_" + VID + "_EadInfo";

            // See if this is in the local cache first
            object returnValue = HttpContext.Current.Cache.Get(key);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_EAD_Info", "Found EAD information on local cache");
                }

                return (EAD_Transfer_Object)returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_EAD_Info", "EAD information not found in either the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Store the EAD information related to a digital resource on the cache   </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
        /// <param name="VID"> Volume Identifier for the digital resource to store </param>
        /// <param name="StoreObject"> EAD information for a digital Resource object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_EAD_Info(string BibID, string VID, EAD_Transfer_Object StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            // Determine the key
            string key = "ITEM_" + BibID + "_" + VID + "_EadInfo";
            const int LENGTH_OF_TIME = 3;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Store_EAD_Info", "Adding object '" + key + "' to the local cache with expiration of " + LENGTH_OF_TIME + " minute");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LENGTH_OF_TIME));
        }

        #endregion

        #region Methods related to storing and retrieving the MARC record object for a single digital resource

        /// <summary> Retrieves the MARC record object related to a digital resource  </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
        /// <param name="VID"> Volume Identifier for the digital resource to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the MARC record from the cache  </returns>
        public MARC_Transfer_Record Retrieve_MARC_Record(string BibID, string VID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            // Determine the key
            string key = "ITEM_" + BibID + "_" + VID + "_MarcRecord";

            // See if this is in the local cache first
            object returnValue = HttpContext.Current.Cache.Get(key);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_MARC_Record", "Found MARC record on local cache");
                }

                return (MARC_Transfer_Record)returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Retrieve_MARC_Record", "MARC record not found in either the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Store the MARC record object related to a digital resource on the cache   </summary>
        /// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
        /// <param name="VID"> Volume Identifier for the digital resource to store </param>
        /// <param name="StoreObject"> EAD information for a digital Resource object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_MARC_Record(string BibID, string VID, MARC_Transfer_Record StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            // Determine the key
            string key = "ITEM_" + BibID + "_" + VID + "_MarcRecord";
            const int LENGTH_OF_TIME = 3;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_ItemServices.Store_MARC_Record", "Adding object '" + key + "' to the local cache with expiration of " + LENGTH_OF_TIME + " minute");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LENGTH_OF_TIME));
        }

        #endregion
    }
}
