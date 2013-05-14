#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.ApplicationServer.Caching;
using SobekCM.Resource_Object;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Skins;

#endregion

namespace SobekCM.Library.MemoryMgmt
{
    /// <summary> Class manages all interactions with the AppFabric Caching server and keeps track
    /// of the keys for all the items on the remote cache which have not been expired </summary>
    public class AppFabric_Manager
    {
        private static DataCacheFactory myCacheFactory;
        private static DataCache myDefaultCache;
        private const string regionName = "SobekCM";
        private static Expiring_Item_Typed_List<string> keys;
        private static DateTime? lastErrorTime;
        private static string lastErrorMessage;
        private static readonly bool enabled;

        /// <summary> Static constructor initialized this class for use </summary>
        static AppFabric_Manager()
        {
            lastErrorMessage = String.Empty;

            if (SobekCM_Library_Settings.Caching_Server.Length > 0)
            {
                enabled = true;
            }

            if (enabled)
            {
                if (PrepareClient(SobekCM_Library_Settings.Caching_Server))
                {
                    keys = new Expiring_Item_Typed_List<string>(30);
                }
                else
                {
                    lastErrorTime = DateTime.Now;
                }
            }
        }

        /// <summary> Gets the list of all items which are currently cached and in the list of non-expired keys </summary>
        public static ReadOnlyCollection<Cached_Object_Info> Cached_Items
        {
            get
            {
                List<Cached_Object_Info> returnValue = new List<Cached_Object_Info>();
                if (keys != null)
                {
                    returnValue.AddRange(keys.Select(item => new Cached_Object_Info(item.Key, item.DataType)));
                }
                return new ReadOnlyCollection<Cached_Object_Info>(returnValue);
            }
        }

        /// <summary> Adds an object to the remote cache and provides a key for retrieving later </summary>
        /// <param name="Key"> Key for this item for later retrieval </param>
        /// <param name="Object_For_Caching"> Object to cache </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Add(string Key, object Object_For_Caching, Custom_Tracer Tracer )
        {
            if (Configure(Tracer))
            {
                try
                {
                    myDefaultCache.Put(Key, Object_For_Caching, regionName);
                    keys.Add(Key, Object_For_Caching.GetType());
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary> Checks to see if the provided key is in the list of keys for items remotely cached </summary>
        /// <param name="Key"> Key to check for existence </param>
        /// <returns> TRUE if the key exists, otherwise FALSE </returns>
        public static bool Contains(string Key)
        {
            return keys != null && keys.Contains(Key);
        }

        /// <summary> Gets an object from the remote cache by key </summary>
        /// <param name="Key"> Key for the object to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Retrieved object, or NULL </returns>
        public static object Get(string Key, Custom_Tracer Tracer)
        {
            try
            {
                if (( keys != null ) && ( keys.Contains(Key)))
                {
                    object returnValue = myDefaultCache.Get(Key, regionName);
                    if (returnValue == null)
                        keys.Remove(Key);
                    else
                        return returnValue;
                }
            }
            catch 
            {
                
            }
            return null;
        }

        /// <summary> Clears the list of all keys, which in effect, causes any remaining items on the
        /// cache to be ignored </summary>
        public static void Clear_All_Keys()
        {
            if ( keys != null )
                keys.Clear();
        }

        /// <summary> Expires an item by removing it from the list of remotely cached items </summary>
        /// <param name="Key"> Key to remove from the list of remotely cached items </param>
        public static void Expire_Item(string Key)
        {
            if ( keys != null )
                keys.Remove(Key);
        }

        /// <summary> Expires a number of items whose key begins with the provided start </summary>
        /// <param name="Key_Start"> Beginning of all matching keys to be epxired </param>
        public static void Expire_Items(string Key_Start)
        {
            if (keys != null)
            {
                List<string> keys_to_expire = (from thisKey in keys where thisKey.Key.IndexOf(Key_Start) == 0 select thisKey.Key).ToList();
                foreach (string expireKey in keys_to_expire)
                {
                    keys.Remove(expireKey);
                }
            }
        }

        private static bool Configure( Custom_Tracer Tracer )
        {
            if ( !enabled )
                return false;

            if (( lastErrorTime.HasValue ) && ( DateTime.Now.Subtract( lastErrorTime.Value ).TotalMinutes < 15 ))
            {
                if (lastErrorMessage.Length > 0)
                {
                    Tracer.Add_Trace("AppFabric_Manager.configure", "Network cache temporarily disabled due to previous error ( " + lastErrorTime.Value.ToShortTimeString() + " )<br />" + lastErrorMessage, Custom_Trace_Type_Enum.Error);
                }
                else
                {
                    Tracer.Add_Trace("AppFabric_Manager.configure", "Network cache temporarily disabled due to previous error ( " + lastErrorTime.Value.ToShortTimeString() + " )", Custom_Trace_Type_Enum.Error);
                }
                return false;
            }


            if ( keys == null )
            {
                if (PrepareClient(SobekCM_Library_Settings.Caching_Server))
                {
                    keys = new Expiring_Item_Typed_List<string>(30);
                    return true;
                }
                
                if (lastErrorMessage.Length > 0)
                {
                    Tracer.Add_Trace("AppFabric_Manager.configure", "Unable to prepare the caching client<br />" + lastErrorMessage, Custom_Trace_Type_Enum.Error);
                }
                else
                {
                    Tracer.Add_Trace("AppFabric_Manager.configure", "Unable to prepare the caching client", Custom_Trace_Type_Enum.Error);
                }

                lastErrorTime = DateTime.Now;
                return false;
            }

            bool returnValue = true;
            try
            {
                lastErrorMessage = String.Empty;
                myDefaultCache.CreateRegion(regionName);
            }
            catch (Exception ee )
            {
                lastErrorMessage = ee.Message;
                returnValue = false;
            }

            if (!returnValue)
            {
                lastErrorTime = DateTime.Now;
                keys = null;
                if (lastErrorMessage.Length > 0)
                {
                    Tracer.Add_Trace("AppFabric_Manager.configure", "Unable to create the region on the cache server<br />" + lastErrorMessage, Custom_Trace_Type_Enum.Error);
                }
                else
                {
                    Tracer.Add_Trace("AppFabric_Manager.configure", "Unable to create the region on the cache server", Custom_Trace_Type_Enum.Error);
                }
                return false;
            }
                
            return true;

        }

        private static bool PrepareClient(string server_name)
        {
            lastErrorMessage = String.Empty;

            try
            {

                //-------------------------
                // Configure Cache Client 
                //-------------------------

                //Define Array for 1 Cache Host
                List<DataCacheServerEndpoint> servers = new List<DataCacheServerEndpoint>(1)
                                                            {new DataCacheServerEndpoint(server_name, 22233)};

                //Specify Cache Host Details 
                //  Parameter 1 = host name
                //  Parameter 2 = cache port number

                //Create cache configuration
                DataCacheFactoryConfiguration configuration = new DataCacheFactoryConfiguration
                                                                  {
                                                                      Servers = servers,
                                                                      SecurityProperties =new DataCacheSecurity( DataCacheSecurityMode.None, DataCacheProtectionLevel.None),
                                                                      LocalCacheProperties = new DataCacheLocalCacheProperties()
                                                                  };

                //Disable exception messages since this sample works on a cache aside
                //DataCacheClientLogManager.ChangeLogLevel(System.Diagnostics.TraceLevel.Off);

                //Pass configuration settings to cacheFactory constructor
                myCacheFactory = new DataCacheFactory(configuration);

                //Get reference to named cache called "default"
                myDefaultCache = myCacheFactory.GetCache("default");

                //specify all possible item and region operations
                const DataCacheOperations itemCacheOperations = DataCacheOperations.AddItem |
                                                                DataCacheOperations.ReplaceItem |
                                                                DataCacheOperations.RemoveItem |
                                                                DataCacheOperations.ClearRegion |
                                                                DataCacheOperations.CreateRegion;

                //add cache-level notification callback 
                //all cache operations from a notifications-enabled cache
                DataCacheNotificationDescriptor ndCacheLvlAllOps = myDefaultCache.AddRegionLevelCallback("SobekCM", itemCacheOperations, myCacheLvlDelegate);
                myDefaultCache.CreateRegion(regionName);

                 return true;
            }
            catch ( Exception ee )
            {
                lastErrorMessage = ee.Message;
                return false;
            }
        }

        //method invoked by notification "ndCacheLvlAllOps" 
        private static void myCacheLvlDelegate(string myCacheName,
            string myRegion,
            string myKey,
            DataCacheItemVersion itemVersion,
            DataCacheOperations OperationId,
            DataCacheNotificationDescriptor nd)
        {
            //try
            //{
                // update the key list
                if (keys != null)
                {
                    if (OperationId == DataCacheOperations.RemoveItem)
                        keys.Remove(myKey);
                    if ((OperationId == DataCacheOperations.AddItem) && (!keys.Contains(myKey)))
                    {
                        // This is where we can try to guess the TYPE, based on how our system works
                        Type thisType = null;

                        if (((myKey.IndexOf("ITEM_") == 0) || (myKey.IndexOf("USERITEM") == 0)))
                        {
                            thisType = typeof(SobekCM_Item);
                        }
                        if ((thisType == null) && ((myKey.IndexOf("ITEMSEARCH_") == 0) || ( myKey.IndexOf("BROWSEBY_") == 0 )))
                        {
                            thisType = typeof(List<string>);
                        }
                        if ((thisType == null) && (myKey.IndexOf("SKIN_") == 0))
                        {
                            thisType = typeof(SobekCM_Skin_Object);
                        }
                        if ((thisType == null) && (myKey.IndexOf("AGGR_") == 0))
                        {
                            thisType = typeof(Item_Aggregation);
                        }


                        keys.Add(myKey, thisType);
                    }
                }
            //}
            //catch
            //{

            //}
         }



    }
}
