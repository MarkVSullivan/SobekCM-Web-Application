using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.WebContent;
using SobekCM.Tools;

namespace SobekCM.Core.MemoryMgmt
{
    public class CachedDataManager_AggregationServices
    {
        private readonly CachedDataManager_Settings settings;

        public CachedDataManager_AggregationServices(CachedDataManager_Settings Settings)
        {
            settings = Settings;
        }

        #region Methods relating to storing and retrieving COMPLETE ITEM AGGREGATION objects

        /// <summary> Retrieves the item aggregation obejct from the cache or caching server </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation to retrieve </param>
        /// <param name="Language_Code"> Current language code (item aggregation instances are currently language-specific)</param>
        /// <param name="Possibly_Cache_Locally"> Flag indicates whether to potentially copy to the local cache if it was present on the distant cache </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the item aggregation object </returns>
        public Complete_Item_Aggregation Retrieve_Complete_Item_Aggregation(string Aggregation_Code, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "");
            }

            // Determine the key
            string key = "AGGR|" + Aggregation_Code.ToUpper() + "|COMPLETE";

            // See if this is in the local cache first
            Complete_Item_Aggregation returnValue = HttpContext.Current.Cache.Get(key) as Complete_Item_Aggregation;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Found item aggregation on local cache");
                }

                return returnValue;
            }

            // Try to get this from the caching server, if enabled
            if ((settings.CachingServerEnabled) && (AppFabric_Manager.Contains(key)))
            {
                object from_app_fabric = AppFabric_Manager.Get(key, Tracer);
                if (from_app_fabric != null)
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Found item aggregation on caching server");
                    }

                    //// Check the number of item aggregationPermissions currently locally cached
                    //if ((Possibly_Cache_Locally) && (settings.LOCALLY_CACHED_AGGREGATION_LIMIT > 0))
                    //{
                    //    int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => ThisItem.Key.ToString().IndexOf("AGGR_") == 0);

                    //    // Locally cache if this doesn't exceed the limit
                    //    if (items_cached < settings.LOCALLY_CACHED_AGGREGATION_LIMIT)
                    //    {
                    //        if (Tracer != null)
                    //        {
                    //            Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
                    //        }

                    //        HttpContext.Current.Cache.Insert(key, from_app_fabric, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
                    //    }
                    //}

                    return (Complete_Item_Aggregation)from_app_fabric;
                }
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Aggregation not found in either the local cache or caching server");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the item aggregation object to the cache or caching server </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation to store </param>
        /// <param name="Language_Code"> Current language code (item aggregation instances are currently language-specific)</param>
        /// <param name="StoreObject"> Item aggregation object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Complete_Item_Aggregation(string Aggregation_Code, Complete_Item_Aggregation StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Entering Store_Item_Aggregation method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "AGGR|" + Aggregation_Code.ToUpper() + "|COMPLETE";

            // Check the number of item aggregationPermissions currently locally cached
            //int items_cached = 0;
            int local_expiration = 15;
            //if (Aggregation_Code != "all")
            //{
            //    local_expiration = 1;
            //    if ((LOCALLY_CACHED_AGGREGATION_LIMIT > 0) && ( caching_serving_enabled ))
            //    {
            //        items_cached += HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => ThisItem.Key.ToString().IndexOf("AGGR_") == 0);
            //    }
            //}

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Adding object '" + key + "' to the local cache with expiration of " + local_expiration + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(local_expiration));

            // try to store in the caching server, if enabled
            if ((settings.CachingServerEnabled) && (Aggregation_Code != "all"))
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Adding object '" + key + "' to the caching server");
                }

                AppFabric_Manager.Add(key, StoreObject, Tracer);
            }
        }


        #endregion

        #region Methods relating to storing and retrieving LANGUAGE-SPECIFIC ITEM AGGREGATION objects

        /// <summary> Retrieves the item aggregation obejct from the cache or caching server </summary>
        /// <param name="AggregationCode"> Code for the item aggregation to retrieve </param>
        /// <param name="Language"> Current language code (item aggregation instances are currently language-specific)</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the item aggregation object </returns>
        public Item_Aggregation Retrieve_Item_Aggregation(string AggregationCode, Web_Language_Enum Language, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "");
            }

            // Determine the key
            string key = "AGGR|" + AggregationCode.ToUpper() + "|" + Web_Language_Enum_Converter.Enum_To_Code(Language);

            // See if this is in the local cache first
            Item_Aggregation returnValue = HttpContext.Current.Cache.Get(key) as Item_Aggregation;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Found item aggregation on local cache");
                }

                return returnValue;
            }

            // Try to get this from the caching server, if enabled
            if ((settings.CachingServerEnabled) && (AppFabric_Manager.Contains(key)))
            {
                object from_app_fabric = AppFabric_Manager.Get(key, Tracer);
                if (from_app_fabric != null)
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Found item aggregation on caching server");
                    }

                    return (Item_Aggregation)from_app_fabric;
                }
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Aggregation not found in either the local cache or caching server");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the item aggregation object to the cache or caching server </summary>
        /// <param name="AggregationCode"> Code for the item aggregation to store </param>
        /// <param name="Language"> Current language code (item aggregation instances are currently language-specific)</param>
        /// <param name="StoreObject"> Item aggregation object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Item_Aggregation(string AggregationCode, Web_Language_Enum Language, Item_Aggregation StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Entering Store_Item_Aggregation method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "AGGR|" + AggregationCode.ToUpper() + "|" + Web_Language_Enum_Converter.Enum_To_Code(Language);

            int local_expiration = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Adding object '" + key + "' to the local cache with expiration of " + local_expiration + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(local_expiration));

            // try to store in the caching server, if enabled
            if ((settings.CachingServerEnabled) && (AggregationCode != "all"))
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Adding object '" + key + "' to the caching server");
                }

                AppFabric_Manager.Add(key, StoreObject, Tracer);
            }
        }

        #endregion


        /// <summary> Removes all references to a particular item aggregation from the cache or caching server </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation to remove </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Remove_Item_Aggregation(string Aggregation_Code, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            // Determine the key
            string key_nolanguage = "AGGR|" + Aggregation_Code;
            string key_start = "AGGR|" + Aggregation_Code + "|";

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Remove_Item_Aggregation", "Removing item aggregation '" + Aggregation_Code + "' from the cache");
            }

            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache where (thisItem.Key.ToString() == key_nolanguage) || (thisItem.Key.ToString().IndexOf(key_start) == 0) select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys)
            {
                HttpContext.Current.Cache.Remove(key);
            }

            // Do the same thing for the remote cache
            keys.Clear();
            keys.AddRange(from cachedObject in AppFabric_Manager.Cached_Items where (cachedObject.Object_Key == key_nolanguage) || (cachedObject.Object_Key.IndexOf(key_start) == 0) select cachedObject.Object_Key);

            // Delete all items from the Cache
            foreach (string key in keys)
            {
                AppFabric_Manager.Expire_Item(key);
            }
        }



        public HTML_Based_Content Retrieve_Aggregation_HTML_Based_Content(string Aggregation_Code, Web_Language_Enum Language, string ChildPageCode, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_AggregationServices.Retrieve_Aggregation_HTML_Based_Content", "");
            }

            // Determine the key
            string key = "AGGR|" + Aggregation_Code.ToUpper() + "|" + Web_Language_Enum_Converter.Enum_To_Code(Language) + "|" + ChildPageCode;

            // See if this is in the local cache first
            HTML_Based_Content returnValue = HttpContext.Current.Cache.Get(key) as HTML_Based_Content;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_AggregationServices.Retrieve_Aggregation_HTML_Based_Content", "Found aggregation HTML based content on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_AggregationServices.Retrieve_Aggregation_HTML_Based_Content", "Aggregation HTML based content not found in either the local cache");
            }

            // Since everything failed, just return null
            return null;
        }


        public void Store_Aggregation_HTML_Based_Content(string Aggregation_Code, Web_Language_Enum Language, string ChildPageCode, HTML_Based_Content StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_AggregationServices.Store_Aggregation_HTML_Based_Content", "Entering Store_Aggregation_HTML_Based_Content method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                Tracer.Add_Trace("CachedDataManager_AggregationServices.Store_Aggregation_HTML_Based_Content", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "AGGR|" + Aggregation_Code.ToUpper() + "|" + Web_Language_Enum_Converter.Enum_To_Code(Language) + "|" + ChildPageCode;

            // Check the number of item aggregationPermissions currently locally cached
            int local_expiration = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_AggregationServices.Store_Aggregation_HTML_Based_Content", "Adding object '" + key + "' to the local cache with expiration of " + local_expiration + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(local_expiration));
        }

    }
}
