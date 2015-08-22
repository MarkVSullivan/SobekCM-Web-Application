using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.WebContent;
using SobekCM.Core.WebContent.Hierarchy;
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


        #region Method used to cache a single non-aggregational web content page details (or redirect)

        /// <summary> Retrieves a fully built non-aggregational static web content page (engine and client side) </summary>
        /// <param name="WebContentID"> Primary key for this web content page (or redirect) </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the fully built HTML based content object </returns>
        public HTML_Based_Content Retrieve_Page_Details( int WebContentID, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Page_Details", "");
            }

            // Determine the key
            string key = "WEBCONTENT|DETAILS|" + WebContentID;

            // See if this is in the local cache first
            HTML_Based_Content returnValue = HttpContext.Current.Cache.Get(key) as HTML_Based_Content;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Page_Details", "Found page details on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Page_Details", "Page details not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores a fully built non-aggregational static web content page (engine and client side) </summary>
        /// <param name="StoreObject"> Fully built web content page (or redirect) object </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Page_Details(HTML_Based_Content StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Page_Details");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Page_Details", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "WEBCONTENT|DETAILS|" + StoreObject.WebContentID;

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Page_Details", "Adding object '" + key + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Retrieves the special missing web content page, used when a requested resource is missing (engine and client side) </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the fully built HTML based content object </returns>
        public HTML_Based_Content Retrieve_Special_Missing_Page(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Special_Missing_Pages", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|DETAILS|!MISSING!";

            // See if this is in the local cache first
            HTML_Based_Content returnValue = HttpContext.Current.Cache.Get(KEY) as HTML_Based_Content;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Special_Missing_Pages", "Found MISSING page details on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Special_Missing_Pages", "MISSING page details not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the special missing web content page, used when a requested resource is missing (engine and client side) </summary>
        /// <param name="StoreObject"> Fully built web content page (or redirect) object </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Special_Missing_Page(HTML_Based_Content StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Special_Missing_Page");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Special_Missing_Page", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|DETAILS|!MISSING!";

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Special_Missing_Page", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clears a single web content page (or redirect) detail objects that is currently cached (engine and client side) </summary>
        /// <param name="WebContentID"> Primary key for the web content page to clear</param>
        public void Clear_Page_Details(int WebContentID)
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            string key = "WEBCONTENT|DETAILS|" + WebContentID;
            HttpContext.Current.Cache.Remove(key);
        }

        /// <summary> Clears all the web content page (or redirect) detail objects that are currently cached (engine and client side) </summary>
        public void Clear_Page_Details()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("WEBCONTENT|DETAILS|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }


        #endregion

        #region Methods used to cache the database outputs for use by the SobekCM engine

        #region Methods related to the raw data global list of recent updates (engine side)

        /// <summary> Retrieves the raw data list of recent updates to all the top-level web content pages (engine side) </summary>
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
            const string KEY = "WEBCONTENT|ENGINE|RECENT_UPDATES";

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

        /// <summary> Stores the raw data list of recent updates to all the top-level web content pages (engine side) </summary>
        /// <param name="StoreObject"> Data set of all the recent updates </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Global_Recent_Updates(DataSet StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|ENGINE|RECENT_UPDATES";

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the raw data global list of recent updates to top-level static web content pages from the cache (engine side) </summary>
        public void Clear_Global_Recent_Updates()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|ENGINE|RECENT_UPDATES";
            HttpContext.Current.Cache.Remove(KEY);
        }

        #endregion

        #region Methods related to the raw data list of web content pages, excluding redirects  (engine side)

        /// <summary> Retrieves the raw data list of web content pages, excluding redirects  (engine side) </summary>
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
            const string KEY = "WEBCONTENT|ENGINE|PAGES";

            // See if this is in the local cache first
            DataSet returnValue = HttpContext.Current.Cache.Get(KEY) as DataSet;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Web_Content_Pages", "Found web content pages lists on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Web_Content_Pages", "Web content pages lists not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the raw data list of all top-level web content pages, excluding redirects (engine side) </summary>
        /// <param name="StoreObject"> Data set of all the web content pages </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_All_Web_Content_Pages(DataSet StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content_Pages");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content_Pages", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|ENGINE|PAGES";

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content_Pages", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the raw data global list of all web content pages, excluding redirects (engine side) </summary>
        public void Clear_All_Web_Content_Pages()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|ENGINE|PAGES";
            HttpContext.Current.Cache.Remove(KEY);
        }

        #endregion

        #region Methods related to raw data the list of redirects  (engine side)

        /// <summary> Retrieves the raw data list of global redirects (engine side) </summary>
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
            const string KEY = "WEBCONTENT|ENGINE|REDIRECTS";

            // See if this is in the local cache first
            DataSet returnValue = HttpContext.Current.Cache.Get(KEY) as DataSet;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Redirects", "Found web content redirects list on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Redirects", "Web content redirects list not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the raw data list of global redirects (engine side) </summary>
        /// <param name="StoreObject"> Data set of all the global redirects </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Redirects(DataSet StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Redirects");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Redirects", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|ENGINE|REDIRECTS";

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Redirects", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the raw data list of global redirects (engine side) </summary>
        public void Clear_Redirects()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|ENGINE|REDIRECTS";
            HttpContext.Current.Cache.Remove(KEY);
        }

        #endregion

        #region Methods related to the raw data list of web content entities, including pages and redirects  (engine side)

        /// <summary> Retrieves the raw data list of web content entities, including pages and redirects (engine side) </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the data set of all web content pages and redirects </returns>
        public DataSet Retrieve_All_Web_Content(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Web_Content", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|ENGINE|ALL";

            // See if this is in the local cache first
            DataSet returnValue = HttpContext.Current.Cache.Get(KEY) as DataSet;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Web_Content", "Found web content entities (pages and redirects) on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Web_Content", "Web content entities (pages and redirects) not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the raw data list of all top-level web content entities, including pages and redirects (engine side) </summary>
        /// <param name="StoreObject"> Data set of all the web content pages and redirects </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_All_Web_Content(DataSet StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|ENGINE|ALL";

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Web_Content", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the raw data global list of all web content entities, including pages and redirects (engine side) </summary>
        public void Clear_All_Web_Content_Lists()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Release all the related info
            const string KEY1 = "WEBCONTENT|ENGINE|ALL";
            HttpContext.Current.Cache.Remove(KEY1);

            const string KEY2 = "WEBCONTENT|ENGINE|PAGES";
            HttpContext.Current.Cache.Remove(KEY2);

            const string KEY3 = "WEBCONTENT|ENGINE|REDIRECTS";
            HttpContext.Current.Cache.Remove(KEY3);
        }

        #endregion

        #region Methods related to the raw data global usage reports  (engine side)

        /// <summary> Retrieves the raw data global web content usage report between two dates (engine side) </summary>
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
            string key = "WEBCONTENT|ENGINE|GLOBALSTATS|" + Year1 + "|" + Month1 + "|" + Year2 + "|" + Month2;

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


        /// <summary> Stores a pulled usage statistics raw data report across all web content pages (engine side) </summary>
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
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report", "Caching is disabled");
                return;
            }

            // Determine the key
            string key = "WEBCONTENT|ENGINE|GLOBALSTATS|" + Year1 + "|" + Month1 + "|" + Year2 + "|" + Month2;

            const int LOCAL_EXPIRATION = 5;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report", "Adding object '" + key + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear all global raw data usage reports from the cache (engine side ) </summary>
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
            foreach (string key in keys.Where(Key => Key.StartsWith("WEBCONTENT|ENGINE|GLOBALSTATS|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        #endregion

        #endregion

        #region Methods used by the client to cache the output received from the SobekCM engine

        #region Methods related to the processed global list of recent updates  (client side)

        /// <summary> Retrieves the flag indicating if there are any global recent updates to the web content entities (pages and redirects) </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> True if there were recent global updates, False if there were none, or NULL if not set </returns>
        public bool? Retrieve_Has_Global_Recent_Updates_Flag(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Global_Recent_Updates_Flag", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|RECENT_UPDATES_FLAG";

            // See if this is in the local cache first
            bool? returnValue = HttpContext.Current.Cache.Get(KEY) as bool?;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Global_Recent_Updates_Flag", "Found flag on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Global_Recent_Updates_Flag", "Flag not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the flag indicating if there are any global recent updates to the web content entities (pages and redirects) </summary>
        /// <param name="StoreObject"> Flag indicating if there are recent updates </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Has_Global_Recent_Updates_Flag(bool StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Global_Recent_Updates_Flag");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Global_Recent_Updates_Flag", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|RECENT_UPDATES_FLAG";

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Global_Recent_Updates_Flag", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the flag indicating if there are any global recent updates to the web content entities (pages and redirects) </summary>
        public void Clear_Has_Global_Recent_Udpates_Flag()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|RECENT_UPDATES_FLAG";
            HttpContext.Current.Cache.Remove(KEY);
        }

        /// <summary> Retrieves the list of possible next level from an existing page in the recent updates </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of next level values, or NULL if not set </returns>
        public List<string> Retrieve_Global_Recent_Updates_NextLevel(Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates_NextLevel", "");
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|RECENT_UPDATES|NEXTLEVEL|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // See if this is in the local cache first
            List<string> returnValue = HttpContext.Current.Cache.Get(keyBuilder.ToString()) as List<string>;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates_NextLevel", "Found next level values on local cache ( " + keyBuilder + " )");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates_NextLevel", "Next level values not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of possible next level from an existing page in the recent updates </summary>
        /// <param name="StoreObject"> List of next level values </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        public void Store_Global_Recent_Updates_NextLevel(List<string> StoreObject, Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates_NextLevel");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates_NextLevel", "Caching is disabled");
                return;
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|RECENT_UPDATES|NEXTLEVEL|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates_NextLevel", "Adding object '" + keyBuilder + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(keyBuilder.ToString(), StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the list of possible next level from an existing page in the recent updates </summary>
        public void Clear_Global_Recent_Updates_NextLevel()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("WEBCONTENT|CLIENT|RECENT_UPDATES|NEXTLEVEL|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        /// <summary> Retrieves the list of all users that have participated in the recent updates to all top-level static web content pages </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> List of users, or NULL if not set </returns>
        public List<string> Retrieve_Global_Recent_Updates_Users(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates_Users", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|RECENT_UPDATES|USERS";

            // See if this is in the local cache first
            List<string> returnValue = HttpContext.Current.Cache.Get(KEY) as List<string>;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates_Users", "Found user values on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Recent_Updates_Users", "User values not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of all users that have participated in the recent updates to all top-level static web content pages </summary>
        /// <param name="StoreObject"> List of users that recently updated a web content page </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Global_Recent_Updates_Users(List<string> StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates_Users");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates_Users", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|RECENT_UPDATES|USERS";
            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Recent_Updates_Users", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the list of all users that have participated in the recent updates to all top-level static web content pages </summary>
        public void Clear_Global_Recent_Updates_Users()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|RECENT_UPDATES|USERS";
            HttpContext.Current.Cache.Remove(KEY);
        }

        #endregion

        #region Methods related to the processed list of web content pages, excluding redirects  (client side)

        /// <summary> Retrieves the flag indicating if there are any web content pages (excluding redirects) </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> True if there are any web content pages, False if there were none, or NULL if not set </returns>
        public bool? Retrieve_Has_Content_Pages_Flag(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Content_Pages_Flag", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|PAGES_FLAG";

            // See if this is in the local cache first
            bool? returnValue = HttpContext.Current.Cache.Get(KEY) as bool?;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Content_Pages_Flag", "Found flag on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Content_Pages_Flag", "Flag not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the flag indicating if there are any web content pages (excluding redirects) </summary>
        /// <param name="StoreObject"> Flag indicating if there are any web content pages </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Has_Content_Pages_Flag(bool StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Content_Pages_Flag");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Content_Pages_Flag", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|PAGES_FLAG";

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Content_Pages_Flag", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the flag indicating if there are any global recent updates to the web content entities (pages and redirects) </summary>
        public void Clear_Has_Content_Pages_Flag()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|PAGES_FLAG";
            HttpContext.Current.Cache.Remove(KEY);
        }

        /// <summary> Retrieves the list of possible next level from an existing point in the page hierarchy </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of next level values, or NULL if not set </returns>
        public List<string> Retrieve_All_Pages_NextLevel(Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Pages_NextLevel", "");
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|PAGES|NEXTLEVEL|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // See if this is in the local cache first
            List<string> returnValue = HttpContext.Current.Cache.Get(keyBuilder.ToString()) as List<string>;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Pages_NextLevel", "Found next level values on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Pages_NextLevel", "Next level values not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of possible next level from an existing point in the page hierarchy </summary>
        /// <param name="StoreObject"> List of next level values </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        public void Store_All_Pages_NextLevel(List<string> StoreObject, Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Pages_NextLevel");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Pages_NextLevel", "Caching is disabled");
                return;
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|PAGES|NEXTLEVEL|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Pages_NextLevel", "Adding object '" + keyBuilder + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(keyBuilder.ToString(), StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the list of possible next level from an existing point in the page hierarchy </summary>
        public void Clear_All_Pages_NextLevel()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("WEBCONTENT|CLIENT|PAGES|NEXTLEVEL|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        #endregion

        #region Methods related to processed the list of redirects  (client side)

        /// <summary> Retrieves the flag indicating if there are any global redirects within the web content system </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> True if there are any global redirects, False if there were none, or NULL if not set </returns>
        public bool? Retrieve_Has_Redirects_Flag(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Redirects_Flag", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|REDIRECTS_FLAG";

            // See if this is in the local cache first
            bool? returnValue = HttpContext.Current.Cache.Get(KEY) as bool?;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Redirects_Flag", "Found flag on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Redirects_Flag", "Flag not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the flag indicating if there are any global redirects within the web content system </summary>
        /// <param name="StoreObject"> Flag indicating if there are any redirects </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Has_Redirects_Flag(bool StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Redirects_Flag");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Redirects_Flag", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|REDIRECTS_FLAG";

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Redirects_Flag", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the flag indicating if there are any global redirects within the web content system </summary>
        public void Clear_Has_Redirects_Flag()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|REDIRECTS_FLAG";
            HttpContext.Current.Cache.Remove(KEY);
        }

        /// <summary> Retrieves the list of possible next level from an existing point in the redirect hierarchy </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of next level values, or NULL if not set </returns>
        public List<string> Retrieve_All_Redirects_NextLevel(Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Redirects_NextLevel", "");
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|REDIRECTS|NEXTLEVEL|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // See if this is in the local cache first
            List<string> returnValue = HttpContext.Current.Cache.Get(keyBuilder.ToString()) as List<string>;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Redirects_NextLevel", "Found next level values on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Redirects_NextLevel", "Next level values not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of possible next level from an existing point in the redirect hierarchy </summary>
        /// <param name="StoreObject"> List of next level values </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        public void Store_All_Redirects_NextLevel(List<string> StoreObject, Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Redirects_NextLevel");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Redirects_NextLevel", "Caching is disabled");
                return;
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|REDIRECTS|NEXTLEVEL|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Redirects_NextLevel", "Adding object '" + keyBuilder + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(keyBuilder.ToString(), StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the list of possible next level from an existing point in the redirect hierarchy </summary>
        public void Clear_All_Redirects_NextLevel()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("WEBCONTENT|CLIENT|REDIRECTS|NEXTLEVEL|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        #endregion

        #region Methods related to the processed list of web content entities, including pages and redirects  (client side)

        /// <summary> Retrieves the flag indicating if there are any web content entities, including pages and redirects </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> True if there are web content entities, including pages and redirects, False if there were none, or NULL if not set </returns>
        public bool? Retrieve_Has_Content_Flag(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Content_Flag", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|CONTENT_FLAG";

            // See if this is in the local cache first
            bool? returnValue = HttpContext.Current.Cache.Get(KEY) as bool?;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Content_Flag", "Found flag on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Content_Flag", "Flag not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the flag indicating if there are any web content entities, including pages and redirects </summary>
        /// <param name="StoreObject"> Flag indicating if there are any web content entities, including pages and redirects </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Has_Content_Flag(bool StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Content_Flag");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Content_Flag", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|CONTENT_FLAG";

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Content_Flag", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the flag indicating if there are any web content entities, including pages and redirects </summary>
        public void Clear_Has_Content_Flag()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|CONTENT_FLAG";
            HttpContext.Current.Cache.Remove(KEY);
        }

        /// <summary> Retrieves the list of possible next level from an existing point in the web content entities, including pages and redirects, hierarchy </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of next level values, or NULL if not set </returns>
        public List<string> Retrieve_All_Content_NextLevel(Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Content_NextLevel", "");
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|CONTENT|NEXTLEVEL|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // See if this is in the local cache first
            List<string> returnValue = HttpContext.Current.Cache.Get(keyBuilder.ToString()) as List<string>;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Content_NextLevel", "Found next level values on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_All_Content_NextLevel", "Next level values not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of possible next level from an existing point in the web content entities, including pages and redirects, hierarchy </summary>
        /// <param name="StoreObject"> List of next level values </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        public void Store_All_Content_NextLevel(List<string> StoreObject, Custom_Tracer Tracer, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Content_NextLevel");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Content_NextLevel", "Caching is disabled");
                return;
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|CONTENT|NEXTLEVEL|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_All_Content_NextLevel", "Adding object '" + keyBuilder + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(keyBuilder.ToString(), StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the list of possible next level from an existing point in the web content entities, including pages and redirects, hierarchy </summary>
        public void Clear_All_Content_NextLevel()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("WEBCONTENT|CLIENT|CONTENT|NEXTLEVEL|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        #endregion

        #region Methods related to the raw data global usage reports  (engine side)

        /// <summary> Retrieves the flag indicating if any usage has been reported for this instance's web content entities (pages and redirects) </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> True if there were usage, False if there were none, or NULL if not set </returns>
        public bool? Retrieve_Has_Global_Usage_Flag(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Global_Usage_Flag", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|USAGE_FLAG";

            // See if this is in the local cache first
            bool? returnValue = HttpContext.Current.Cache.Get(KEY) as bool?;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Global_Usage_Flag", "Found flag on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Has_Global_Usage_Flag", "Flag not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the flag indicating if any usage has been reported for this instance's web content entities (pages and redirects) </summary>
        /// <param name="StoreObject"> Flag indicating if there has been any usage </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Has_Global_Usage_Flag(bool StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Global_Usage_Flag");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Global_Usage_Flag", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|USAGE_FLAG";

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Has_Global_Usage_Flag", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the flag indicating if any usage has been reported for this instance's web content entities (pages and redirects) </summary>
        public void Clear_Has_Global_Usage_Flag()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|USAGE_FLAG";
            HttpContext.Current.Cache.Remove(KEY);
        }

        /// <summary> Retrieves the list of possible next level from an existing used page in a global usage report for a date range </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Year1"> Start year of the year/month range for these usage stats </param>
        /// <param name="Month1"> Start month of the year/month range for these usage stats </param>
        /// <param name="Year2"> End year of the year/month range for these usage stats </param>
        /// <param name="Month2"> End month of the year/month range for these usage stats </param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <returns> List of next level values, or NULL if not set </returns>
        public List<string> Retrieve_Global_Usage_Report_NextLevel(Custom_Tracer Tracer, int Year1, int Month1, int Year2, int Month2, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Usage_Report_NextLevel", "");
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|USAGE_REPORT|NEXTLEVEL|" + Year1 + "|" + Month1 + "|" + Year2 + "|" + Month2 + "|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // See if this is in the local cache first
            List<string> returnValue = HttpContext.Current.Cache.Get(keyBuilder.ToString()) as List<string>;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Usage_Report_NextLevel", "Found next level values on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Global_Usage_Report_NextLevel", "Next level values not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the list of possible next level from an existing used page in a global usage report for a date range </summary>
        /// <param name="StoreObject"> List of next level values </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Year1"> Start year of the year/month range for these usage stats </param>
        /// <param name="Month1"> Start month of the year/month range for these usage stats </param>
        /// <param name="Year2"> End year of the year/month range for these usage stats </param>
        /// <param name="Month2"> End month of the year/month range for these usage stats </param>
        /// <param name="Level1"> (Optional) First level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level2"> (Optional) Second level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level3"> (Optional) Third level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level4"> (Optional) Fourth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level5"> (Optional) Fifth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level6"> (Optional) Sixth level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level7"> (Optional) Seventh level of URL for the updated web content entity, if looking for children of a page </param>
        /// <param name="Level8"> (Optional) Eighth level of URL for the updated web content entity, if looking for children of a page </param>
        public void Store_Global_Usage_Report_NextLevel(List<string> StoreObject, Custom_Tracer Tracer, int Year1, int Month1, int Year2, int Month2, string Level1 = null, string Level2 = null, string Level3 = null, string Level4 = null, string Level5 = null, string Level6 = null, string Level7 = null, string Level8 = null)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report_NextLevel");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report_NextLevel", "Caching is disabled");
                return;
            }

            // Determine the key
            StringBuilder keyBuilder = new StringBuilder("WEBCONTENT|CLIENT|USAGE_REPORT|NEXTLEVEL|" + Year1 + "|" + Month1 + "|" + Year2 + "|" + Month2 + "|");
            if (!String.IsNullOrEmpty(Level1))
            {
                keyBuilder.Append(Level1 + "|");
                if (!String.IsNullOrEmpty(Level2))
                {
                    keyBuilder.Append(Level2 + "|");
                    if (!String.IsNullOrEmpty(Level3))
                    {
                        keyBuilder.Append(Level3 + "|");
                        if (!String.IsNullOrEmpty(Level4))
                        {
                            keyBuilder.Append(Level4 + "|");
                            if (!String.IsNullOrEmpty(Level5))
                            {
                                keyBuilder.Append(Level5 + "|");
                                if (!String.IsNullOrEmpty(Level6))
                                {
                                    keyBuilder.Append(Level6 + "|");
                                    if (!String.IsNullOrEmpty(Level7))
                                    {
                                        keyBuilder.Append(Level7 + "|");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Global_Usage_Report_NextLevel", "Adding object '" + keyBuilder + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(keyBuilder.ToString(), StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the list of possible next level from an existing used page in a global usage report for a date range</summary>
        public void Clear_Global_Usage_Report_NextLevel()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            // Get collection of keys in the Cache
            List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

            // Delete all items from the Cache
            foreach (string key in keys.Where(Key => Key.StartsWith("WEBCONTENT|CLIENT|USAGE_REPORT|NEXTLEVEL|")))
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        #endregion

        #region Methods related to the complete hierarchy of pages (client side - not used by default UI)

        /// <summary> Retrieves the complete hierarchy of non-aggregational static web content pages and redirects, used for navigation </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Complete hierarchy of non-aggregational static web content pages and redirects, used for navigation </returns>
        /// <remarks> By default, this method is not used, but is exposed for others that are utilizing the engine client code </remarks>
        public WebContent_Hierarchy Retrieve_Hierarchy(Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Hierarchy", "");
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|HIERARCHY";

            // See if this is in the local cache first
            WebContent_Hierarchy returnValue = HttpContext.Current.Cache.Get(KEY) as WebContent_Hierarchy;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Hierarchy", "Found complete web content hierarchy on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Retrieve_Hierarchy", "Complete web content hierarchy not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the complete hierarchy of non-aggregational static web content pages and redirects, used for navigation </summary>
        /// <param name="StoreObject"> List of next level values </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Hierarchy(WebContent_Hierarchy StoreObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Hierarchy");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Hierarchy", "Caching is disabled");
                return;
            }

            // Determine the key
            const string KEY = "WEBCONTENT|CLIENT|HIERARCHY";
            const int LOCAL_EXPIRATION = 15;

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_WebContentServices.Store_Hierarchy", "Adding object '" + KEY + "' to the local cache with expiration of " + LOCAL_EXPIRATION + " minute(s)");
            }

            HttpContext.Current.Cache.Insert(KEY, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(LOCAL_EXPIRATION));
        }

        /// <summary> Clear the complete hierarchy of non-aggregational static web content pages and redirects, used for navigation </summary>
        public void Clear_Hierarchy()
        {
            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                return;
            }

            HttpContext.Current.Cache.Remove("WEBCONTENT|CLIENT|HIERARCHY");
        }

        #endregion

        #endregion
    }
}
