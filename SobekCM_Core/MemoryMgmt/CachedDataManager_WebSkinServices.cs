using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Skins;
using SobekCM.Tools;

namespace SobekCM.Core.MemoryMgmt
{
    public class CachedDataManager_WebSkinServices
    {
        private readonly CachedDataManager_Settings settings;

        public CachedDataManager_WebSkinServices(CachedDataManager_Settings Settings)
        {
            settings = Settings;
        }


        /// <summary> Clears all aggregation values from the cache </summary>
        public void Clear()
        {
            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("SKIN|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        #region Static methods relating to storing and retrieving HTML skin objects

        /// <summary> Removes all matching html skin objects from the cache or caching server </summary>
        /// <param name="Skin_Code"> Code identifying this skin </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Number of instances of this skin removed from the cache or caching server </returns>
        /// <remarks> This removes every instance of this skin, regardless of language </remarks>
        public int Remove_Skin(string Skin_Code, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return 0;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Remove_Skin");
            }

            int values_cleared = 0;
            foreach (DictionaryEntry thisItem in HttpContext.Current.Cache)
            {
                if ((thisItem.Key.ToString() == "SKIN|" + Skin_Code.ToLower()) || (thisItem.Key.ToString().IndexOf("SKIN|" + Skin_Code.ToLower() + "|") == 0))
                {
                    HttpContext.Current.Cache.Remove(thisItem.Key.ToString());
                    values_cleared++;
                }
            }

            //// Do the same thing for the remote cache
            //foreach (Cached_Object_Info cachedObject in AppFabric_Manager.Cached_Items)
            //{
            //    if ((cachedObject.Object_Key == "SKIN_" + Skin_Code.ToLower()) || (cachedObject.Object_Key.IndexOf("SKIN_" + Skin_Code.ToLower() + "_") == 0))
            //    {
            //        AppFabric_Manager.Expire_Item(cachedObject.Object_Key);
            //        values_cleared++;
            //    }
            //}

            return values_cleared;
        }

        /// <summary> Retrieves the html skin object from the cache or caching server </summary>
        /// <param name="Skin_Code"> Code for this html display skin </param>
        /// <param name="Language_Code"> Current language code for the user interface ( skins are language-specific)</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the html skin object </returns>
        public Web_Skin_Object Retrieve_Skin(string Skin_Code, string Language_Code, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            // Determine the key
            string key = "SKIN|" + Skin_Code.ToLower();
            if (Language_Code.Length > 0)
                key = key + "|" + Language_Code;

            // See if this is in the local cache first
            object returnValue = HttpContext.Current.Cache.Get(key);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Retrieve_Skin", "Found html skin on local cache");
                }

                return (Web_Skin_Object)returnValue;
            }

            // Try to get this from the caching server, if enabled
            if ((settings.CachingServerEnabled) && (AppFabric_Manager.Contains(key)))
            {
                object from_app_fabric = AppFabric_Manager.Get(key, Tracer);
                if (from_app_fabric != null)
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("CachedDataManager.Retrieve_Skin", "Found html skin on caching server");
                    }

                    // Check the number of item aggregationPermissions currently locally cached
                    if (settings.LOCALLY_CACHED_SKINS_LIMIT > 0)
                    {
                        int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => ThisItem.Key.ToString().IndexOf("SKIN_") == 0);

                        // Locally cache if this doesn't exceed the limit
                        if (items_cached < settings.LOCALLY_CACHED_SKINS_LIMIT)
                        {
                            if (Tracer != null)
                            {
                                Tracer.Add_Trace("CachedDataManager.Retrieve_Skin", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
                            }

                            HttpContext.Current.Cache.Insert(key, from_app_fabric, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
                        }
                    }

                    return (Web_Skin_Object)from_app_fabric;
                }
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Skin", "Skin not found in either the local cache or caching server");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the html skin object to the cache or caching server </summary>
        /// <param name="Skin_Code"> Code for this html display skin </param>
        /// <param name="Language_Code"> Current language code for the user interface ( skins are language-specific)</param>
        /// <param name="StoreObject"> HTML Skin object </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Skin(string Skin_Code, string Language_Code, Web_Skin_Object StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (StoreObject == null))
                return;

            // Determine the key
            string key = "SKIN|" + Skin_Code.ToLower();
            if (Language_Code.Length > 0)
                key = key + "|" + Language_Code;


            // Check the number of item aggregationPermissions currently locally cached
            if ((settings.LOCALLY_CACHED_SKINS_LIMIT > 0) || (!settings.CachingServerEnabled))
            {
                int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => ThisItem.Key.ToString().IndexOf("SKIN_") == 0);

                // Locally cache if this doesn't exceed the limit
                if ((items_cached < settings.LOCALLY_CACHED_SKINS_LIMIT) || (!settings.CachingServerEnabled))
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("CachedDataManager.Store_Skin", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
                    }

                    HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
                }
            }

            // try to store in the caching server, if enabled
            if (settings.CachingServerEnabled)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Store_Skin", "Adding object '" + key + "' to the caching server");
                }

                AppFabric_Manager.Add(key, StoreObject, Tracer);
            }
        }

        #endregion
    }
}
