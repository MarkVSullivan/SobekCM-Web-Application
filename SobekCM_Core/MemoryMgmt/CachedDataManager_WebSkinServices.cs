#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Skins;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.MemoryMgmt
{
    /// <summary> Web skin-related services for the Cached Data Manager, which allows skin
    /// objects to be cached locally for reuse </summary>
    public class CachedDataManager_WebSkinServices
    {
        private readonly CachedDataManager_Settings settings;

        /// <summary> Constructor for a new instance of the <see cref="CachedDataManager_WebSkinServices"/> class.  </summary>
        /// <param name="Settings"> Cached data manager settings object </param>
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

        /// <summary> Removes all matching html skin objects from the cache  </summary>
        /// <param name="Skin_Code"> Code identifying this skin </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Number of instances of this skin removed from the cache  </returns>
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

            return values_cleared;
        }

        /// <summary> Retrieves the language-specific html skin object from the cache  </summary>
        /// <param name="Skin_Code"> Code for this html display skin </param>
        /// <param name="Language_Code"> Current language code for the user interface ( skins are language-specific)</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the language-specific html skin object </returns>
        public Web_Skin_Object Retrieve_Skin(string Skin_Code, string Language_Code, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            // Determine the key
            string key = "SKIN|" + Skin_Code.ToLower() + "|";
            if (Language_Code.Length > 0)
                key = key + Language_Code;

            // See if this is in the local cache first
            object returnValue = HttpContext.Current.Cache.Get(key);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Retrieve_Skin", "Found language-specific html skin on local cache");
                }

                return (Web_Skin_Object)returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Skin", "Language specific skin not found in either the local cache");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the language-specific html skin object to the cache  </summary>
        /// <param name="Skin_Code"> Code for this html display skin </param>
        /// <param name="Language_Code"> Current language code for the user interface ( skins are language-specific)</param>
        /// <param name="StoreObject"> Language-specific HTML Skin object </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Skin(string Skin_Code, string Language_Code, Web_Skin_Object StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (StoreObject == null))
                return;

            // Determine the key
            string key = "SKIN|" + Skin_Code.ToLower() + "|";
            if (Language_Code.Length > 0)
                key = key + Language_Code;

            // Log
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Skin", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
            }

            // Add to the cache with five minute expiration
            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
        }

        /// <summary> Retrieves the complete html skin object from the cache  </summary>
        /// <param name="Skin_Code"> Code for this html display skin </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the complete html skin object </returns>
        public Complete_Web_Skin_Object Retrieve_Complete_Skin(string Skin_Code, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return null;

            // Determine the key
            string key = "SKIN|" + Skin_Code.ToLower() + "|COMPLETE";

            // See if this is in the local cache first
            object returnValue = HttpContext.Current.Cache.Get(key);
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Retrieve_Complete_Skin", "Found complete html skin on local cache");
                }

                return (Complete_Web_Skin_Object)returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Complete_Skin", "Complete skin not found in either the local cache");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the complete html skin object to the cache  </summary>
        /// <param name="Skin_Code"> Code for this html display skin </param>
        /// <param name="StoreObject"> Complete HTML Skin object </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Complete_Skin(string Skin_Code, Complete_Web_Skin_Object StoreObject, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (StoreObject == null))
                return;

            // Determine the key
            string key = "SKIN|" + Skin_Code.ToLower() + "|COMPLETE";

            // Log
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Complete_Skin", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
            }

            // Stote the value with five minute expiration
            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
        }

        #endregion
    }
}
