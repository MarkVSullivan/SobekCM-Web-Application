#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.WebContent;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.MemoryMgmt
{
    /// <summary> Aggregation-related services for the Cached Data Manager, which allows aggregation
    /// objects, and closely related objects, to be cached locally for reuse </summary>
    public class CachedDataManager_AggregationServices
    {
        private readonly CachedDataManager_Settings settings;

        /// <summary> Constructor for a new instance of the <see cref="CachedDataManager_AggregationServices"/> class. </summary>
        /// <param name="Settings"> Cached data manager settings object </param>
        public CachedDataManager_AggregationServices(CachedDataManager_Settings Settings)
        {
            settings = Settings;
        }

        #region Methods relating to storing and retrieving COMPLETE ITEM AGGREGATION objects

        /// <summary> Retrieves the complete item aggregation obejct from the cache  </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the complete item aggregation object </returns>
        public Complete_Item_Aggregation Retrieve_Complete_Item_Aggregation(string Aggregation_Code, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || ( HttpContext.Current == null ))
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

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Aggregation not found in either the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the copmlete item aggregation object to the cache </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation to store </param>
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
                if ( Tracer != null ) Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "AGGR|" + Aggregation_Code.ToUpper() + "|COMPLETE";

            // Check the number of item aggregationPermissions currently locally cached
            //int items_cached = 0;
            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Adding object '" + key + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }


        #endregion

        #region Methods relating to storing and retrieving LANGUAGE-SPECIFIC ITEM AGGREGATION objects

        /// <summary> Retrieves the item aggregation obejct from the cache  </summary>
        /// <param name="AggregationCode"> Code for the item aggregation to retrieve </param>
        /// <param name="Language"> Current language code (item aggregation instances are currently language-specific)</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the item aggregation object </returns>
        public Item_Aggregation Retrieve_Item_Aggregation(string AggregationCode, Web_Language_Enum Language, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || ( HttpContext.Current == null ))
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

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Item_Aggregation", "Aggregation not found in either the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the item aggregation object to the cache  </summary>
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

            // Don't store nulls
            if (StoreObject == null)
                return;

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "AGGR|" + AggregationCode.ToUpper() + "|" + Web_Language_Enum_Converter.Enum_To_Code(Language);

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Item_Aggregation", "Adding object '" + key + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        #endregion

        #region Method related to the entire collection hierarchy

        /// <summary> Retrieves the item aggregation hierarchy from the cache  </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the item aggregation hierarchy </returns>
        public Aggregation_Hierarchy Retrieve_Aggregation_Hierarchy(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Aggregation_Hierarchy", "");
            }

            // Determine the key
            const string KEY = "AGGR_HIERARCHY";

            // See if this is in the local cache first
            Aggregation_Hierarchy returnValue = HttpContext.Current.Cache.Get(KEY) as Aggregation_Hierarchy;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager.Retrieve_Aggregation_Hierarchy", "Found item aggregation hierarchy on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Retrieve_Aggregation_Hierarchy", "Aggregation hierarchy not found in either the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the item aggregation hierarchy to the cache  </summary>
        /// <param name="StoreObject"> Item aggregation object to store for later retrieval </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Aggregation_Hierarchy(Aggregation_Hierarchy StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Aggregation_Hierarchy", "Entering Store_Item_Aggregation method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager.Store_Aggregation_Hierarchy", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "AGGR_HIERARCHY";

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager.Store_Aggregation_Hierarchy", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the aggregation hierarchy object from the cache </summary>
        public void Clear_Aggregation_Hierarchy()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "AGGR_HIERARCHY";
            HttpContext.Current.Cache.Remove(KEY);
        }

        #endregion


        /// <summary> Clears all aggregation values from the cache </summary>
        public void Clear()
        {
            // Get collection of keys in the Cache
			List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

			// Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("AGGR|"))) {
                HttpContext.Current.Cache.Remove(key);
            }

            // Delete the hierarchy
            Clear_Aggregation_Hierarchy();
        }


        /// <summary> Removes all references to a particular item aggregation from the cache </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation to remove </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Remove_Item_Aggregation(string Aggregation_Code, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
                return;

            // Determine the key
            string key_nolanguage = "AGGR|" + Aggregation_Code.ToUpper();
            string key_start = "AGGR|" + Aggregation_Code.ToUpper() + "|";

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
        }


        /// <summary> Retrieve the aggregation-level static HTML browse object, including the text itself, from the cache </summary>
        /// <param name="Aggregation_Code"> Aggregation code </param>
        /// <param name="Language"> Requested language version </param>
        /// <param name="ChildPageCode"> Code for the child page in question </param>
        /// <param name="Tracer">The tracer.</param>
        /// <returns> Fully built object with the information about the aggregation-level HTML browse object </returns>
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

        /// <summary>
        /// Store_s the content of the aggregation_ HTM l_ based_.
        /// </summary>
        /// <param name="Aggregation_Code">The aggregation_ code.</param>
        /// <param name="Language">The language.</param>
        /// <param name="ChildPageCode">The child page code.</param>
        /// <param name="StoreObject">The store object.</param>
        /// <param name="Tracer">The tracer.</param>
        public void Store_Aggregation_HTML_Based_Content(string Aggregation_Code, Web_Language_Enum Language, string ChildPageCode, HTML_Based_Content StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_AggregationServices.Store_Aggregation_HTML_Based_Content", "Entering Store_Aggregation_HTML_Based_Content method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_AggregationServices.Store_Aggregation_HTML_Based_Content", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "AGGR|" + Aggregation_Code.ToUpper() + "|" + Web_Language_Enum_Converter.Enum_To_Code(Language) + "|" + ChildPageCode;

            // Check the number of item aggregationPermissions currently locally cached
            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_AggregationServices.Store_Aggregation_HTML_Based_Content", "Adding object '" + key + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

    }
}
