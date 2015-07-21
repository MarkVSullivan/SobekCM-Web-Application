using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Aggregations;
using SobekCM.Tools;

namespace SobekCM.Core.MemoryMgmt
{
    /// <summary> Web content related services for the Cached Data Manager, which allows top-level static
    /// web content objects, and closely related objects, to be cached locally for resuse  </summary>
    public class CachedDataManager_WebContentServices
    {
        private readonly CachedDataManager_Settings settings;

        /// <summary> Constructor for a new instance of the <see cref="CachedDataManager_WebContentServices"/> class. </summary>
        /// <param name="Settings"> Cached data manager settings object </param>
        public CachedDataManager_WebContentServices(CachedDataManager_Settings Settings)
        {
            settings = Settings;
        }

        #region Method related to the global list of recent updates

        /// <summary> Retrieves the list of recent updates to all the top-level web content pages  </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the data set of all recent updates </returns>
        public DataSet Retrieve_Global_Recent_Updates(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|RECENT_UPDATES";

            // See if this is in the local cache first
            DataSet returnValue = HttpContext.Current.Cache.Get(KEY) as DataSet;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates", "Found recent updates on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates", "Recent updates not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of recent updates to all the top-level web content pages  </summary>
        /// <param name="StoreObject"> Data set of all the recent updates </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Global_Recent_Updates(DataSet StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates", "Entering Store_Global_Recent_Updates method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|RECENT_UPDATES";

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the global list of recent updates to top-level static web content pages from the cache </summary>
        public void Clear_Global_Recent_Udpates()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|RECENT_UPDATES";
            HttpContext.Current.Cache.Remove(KEY);
        }

        #endregion

        #region Method related to the list of web content pages (excluding redirects)

        /// <summary> Retrieves the list of web content pages (excluding redirects)  </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the data set of all web content pages </returns>
        public DataSet Retrieve_All_Web_Content_Pages(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Web_Content_Pages", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|PAGES";

            // See if this is in the local cache first
            DataSet returnValue = HttpContext.Current.Cache.Get(KEY) as DataSet;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Web_Content_Pages", "Found recent updates on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Web_Content_Pages", "Recent updates not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of all top-level web content pages (excluding redirects)  </summary>
        /// <param name="StoreObject"> Data set of all the web content pages </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_All_Web_Content_Pages(DataSet StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content_Pages", "Entering Store_All_Web_Content_Pages method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content_Pages", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|PAGES";

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content_Pages", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the global list of all web content pages (excluding redirects) </summary>
        public void Clear_All_Web_Content_Pages()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|PAGES";
            HttpContext.Current.Cache.Remove(KEY);
        }

        #endregion

        #region Method related to the list of redirects

        /// <summary> Retrieves the list of global redirects  </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the data set of all redirects </returns>
        public DataSet Retrieve_Redirects(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Redirects", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|REDIRECTS";

            // See if this is in the local cache first
            DataSet returnValue = HttpContext.Current.Cache.Get(KEY) as DataSet;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Redirects", "Found recent updates on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Redirects", "Recent updates not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of global redirects </summary>
        /// <param name="StoreObject"> Data set of all the global redirects </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Redirects(DataSet StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Redirects", "Entering Store_All_Web_Content_Pages method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Redirects", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|REDIRECTS";

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Redirects", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the list of global redirects </summary>
        public void Clear_Redirects()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|REDIRECTS";
            HttpContext.Current.Cache.Remove(KEY);
        }

        #endregion

        #region Method related to the global usage reports 

        /// <summary> Retrieves the global web content usage report between two dates </summary>
        /// <param name="Year1"> Start year of the year/month range for these usage stats </param>
        /// <param name="Month1"> Start month of the year/month range for these usage stats </param>
        /// <param name="Year2"> End year of the year/month range for these usage stats </param>
        /// <param name="Month2"> End month of the year/month range for these usage stats </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Either NULL or the data set of usage stats between those dates </returns>
        public DataSet Retrieve_Global_Usage_Report(int Year1, int Month1, int Year2, int Month2, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Usage_Report", "");
            }

            // Determine the key
            string key = "WEBCONTENT|GLOBALSTATS|" + Year1 + "|" + Month1 + "|" + Year2 + "|" + Month2;

            // See if this is in the local cache first
            DataSet returnValue = HttpContext.Current.Cache.Get(key) as DataSet;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Usage_Report", "Found requested usage report on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Usage_Report", "Requested usage report not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }


        /// <summary> Stores a pulled usage statistics report across all web content pages </summary>
        /// <param name="StoreObject"> Data set of the usage  </param>
        /// <param name="Year1"> Start year of the year/month range for these usage stats </param>
        /// <param name="Month1"> Start month of the year/month range for these usage stats </param>
        /// <param name="Year2"> End year of the year/month range for these usage stats </param>
        /// <param name="Month2"> End month of the year/month range for these usage stats </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Global_Usage_Report(DataSet StoreObject, int Year1, int Month1, int Year2, int Month2, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report", "Entering Store_All_Web_Content_Pages method");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "WEBCONTENT|GLOBALSTATS|" + Year1 + "|" + Month1 + "|" + Year2 + "|" + Month2;

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report", "Adding object '" + key + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear all global usage reports from the cache </summary>
        public void Clear_Global_Usage_Report()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("WEBCONTENT|GLOBALSTATS|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        #endregion
    }
}
