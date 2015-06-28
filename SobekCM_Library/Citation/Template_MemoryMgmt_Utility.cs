#region Using directives

using System;
using System.Web;
using System.Web.Caching;
using SobekCM.Library.Citation.Template;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.Citation
{
    /// <summary> Memory management utility for storing and retrieving metadata templates </summary>
    public static class Template_MemoryMgmt_Utility
    {
        #region Static methods relating to storing and retrieving templates (for online submission and editing)

        /// <summary> Retrieves the template ( for online submission and editing ) from the cache or caching server </summary>
        /// <param name="Template_Code"> Code which specifies the template to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Requested template object for online submissions and editing</returns>
        public static CompleteTemplate Retrieve_Template(string Template_Code, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Template_MemoryMgmt_Utility.Retrieve_Template", "");
            }

            // Determine the key
            string key = "TEMPLATE_" + Template_Code;

            // Try to get this object
            object returnValue = HttpContext.Current.Cache.Get(key);
            return (returnValue != null) ? (CompleteTemplate)returnValue : null;
        }

        /// <summary> Stores the template ( for online submission and editing ) to the cache or caching server </summary>
        /// <param name="Template_Code"> Code for the template to store </param>
        /// <param name="StoreObject"> CompleteTemplate object for online submissions and editing to store</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public static void Store_Template(string Template_Code, CompleteTemplate StoreObject, Custom_Tracer Tracer)
        {
            // Determine the key
            string key = "TEMPLATE_" + Template_Code;

            if (Tracer != null)
            {
                Tracer.Add_Trace("Template_MemoryMgmt_Utility.Store_Template", "Adding object '" + key + "' to the cache with expiration of thirty minutes");
            }

            // Store this on the cache
            if (HttpContext.Current.Cache[key] == null)
            {
                HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
            }
        }

        #endregion
    }
}
