using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using SobekCM.Tools;

namespace SobekCM.Core.MemoryMgmt
{
    /// <summary> Builder-specific services for the Cached Data Manager, which allows items to be 
    /// cached, or portions of items and REST reponses related to items to be cached </summary>
    public class CachedDataManager_BuilderServices
    {
        private readonly CachedDataManager_Settings settings;

        /// <summary> Constructor for a new instance of the <see cref="CachedDataManager_BuilderServices"/> class. </summary>
        /// <param name="Settings"> Cached data manager settings object </param>
        public CachedDataManager_BuilderServices(CachedDataManager_Settings Settings)
        {
            settings = Settings;
        }

        /// <summary> Retrieves the raw data list of recent builder logs (engine side) </summary>
        /// <param name="StartDate"> Possibly the starting date for the log range </param>
        /// <param name="EndDate"> Possibly the ending date for the log range </param>
        /// <param name="BibVidFilter"> Any search filter to see only particular BibID or BibID/VID</param>
        /// <param name="IncludeNoWorkFlag"> Flag indicates if 'No Work' entries should be included</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Either NULL or the data set of matching builder logs </returns>
        public DataSet Retrieve_Builder_Logs(DateTime? StartDate, DateTime? EndDate, string BibVidFilter, bool IncludeNoWorkFlag, Custom_Tracer Tracer)
        {
            // If the cache is disabled, just return before even tracing
            if ((settings.Disabled) || (HttpContext.Current == null))
                return null;

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_BuilderServices.Retrieve_Builder_Logs", "");
            }

            // Determine the key
            StringBuilder build_key = new StringBuilder("BUILDER|LOGS|");
            if (StartDate.HasValue)
                build_key.Append(StartDate + "|");
            else
                build_key.Append("null|");
            if (EndDate.HasValue)
                build_key.Append(EndDate + "|");
            else
                build_key.Append("null|");
            build_key.Append(BibVidFilter + "|" + IncludeNoWorkFlag.ToString());
            string key = build_key.ToString();

            // See if this is in the local cache first
            DataSet returnValue = HttpContext.Current.Cache.Get(key) as DataSet;
            if (returnValue != null)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("CachedDataManager_BuilderServices.Retrieve_Builder_Logs", "Found builder logs on local cache");
                }

                return returnValue;
            }

            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_BuilderServices.Retrieve_Builder_Logs", "Builder logds not found in the local cache ");
            }

            // Since everything failed, just return null
            return null;
        }

        /// <summary> Stores the raw data list of builder logs (engine side) </summary>
        /// <param name="StoreObject"> Data set of all the recent updates </param>
        /// <param name="StartDate"> Possibly the starting date for the log range </param>
        /// <param name="EndDate"> Possibly the ending date for the log range </param>
        /// <param name="BibVidFilter"> Any search filter to see only particular BibID or BibID/VID</param>
        /// <param name="IncludeNoWorkFlag"> Flag indicates if 'No Work' entries should be included</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Store_Builder_Logs(DataSet StoreObject, DateTime? StartDate, DateTime? EndDate, string BibVidFilter, bool IncludeNoWorkFlag, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_BuilderServices.Store_Builder_Logs");
            }

            // If the cache is disabled, just return before even tracing
            if (settings.Disabled)
            {
                if (Tracer != null) Tracer.Add_Trace("CachedDataManager_BuilderServices.Store_Builder_Logs", "Caching is disabled");
                return;
            }

            // Determine the key
            StringBuilder build_key = new StringBuilder("BUILDER|LOGS|");
            if (StartDate.HasValue)
                build_key.Append(StartDate + "|");
            else
                build_key.Append("null|");
            if (EndDate.HasValue)
                build_key.Append(EndDate + "|");
            else
                build_key.Append("null|");
            build_key.Append(BibVidFilter + "|" + IncludeNoWorkFlag.ToString());
            string key = build_key.ToString();

            // Locally cache if this doesn't exceed the limit
            if (Tracer != null)
            {
                Tracer.Add_Trace("CachedDataManager_BuilderServices.Store_Builder_Logs", "Adding object '" + key + "' to the local cache with expiration of 30 seconds");
            }

            HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(30));
        }
    }
}
