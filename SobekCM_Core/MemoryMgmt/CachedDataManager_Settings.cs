using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Core.MemoryMgmt
{
    public class CachedDataManager_Settings
    {


        /// <summary> Flag indicates if the cache is entirely disabled </summary>
        /// <remarks> This flag is utilized, in particular, by the builder which has no access to the web's local or distributed cache </remarks>
        public bool Disabled { get; internal set; }

        public bool CachingServerEnabled { get; internal set; }

        public int LOCALLY_CACHED_AGGREGATION_LIMIT = 1000;
        public int LOCALLY_CACHED_ITEM_LIMIT = 1;
        public int LOCALLY_CACHED_SKINS_LIMIT = 5;
    }
}
