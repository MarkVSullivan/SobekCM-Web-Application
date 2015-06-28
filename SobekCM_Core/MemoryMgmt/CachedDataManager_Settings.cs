namespace SobekCM.Core.MemoryMgmt
{
    /// <summary> Settings for the cached data manager, relating to retention of objects </summary>
    public class CachedDataManager_Settings
    {
        /// <summary> Flag indicates if the cache is entirely disabled </summary>
        /// <remarks> This flag is utilized, in particular, by the builder which has no access to the web's local or distributed cache </remarks>
        public bool Disabled { get; set; }
    }
}
