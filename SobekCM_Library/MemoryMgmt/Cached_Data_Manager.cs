#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Citation.Template;
using SobekCM.Library.Items;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.SiteMap;
using SobekCM.Library.Skins;
using SobekCM.Library.Solr;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MemoryMgmt
{
	/// <summary> Static class manages the local and distributed caches and handles all requests for object retrieval and object storing </summary>
	public class Cached_Data_Manager
	{
		private static readonly bool caching_serving_enabled;
		private const int LOCALLY_CACHED_AGGREGATION_LIMIT = 10;
		private const int LOCALLY_CACHED_ITEM_LIMIT = 1;
		private const int LOCALLY_CACHED_SKINS_LIMIT = 5;

		/// <summary> Static constructor initializes several variables </summary>
		static Cached_Data_Manager()
		{
			caching_serving_enabled = SobekCM_Library_Settings.Caching_Server.Length > 0;
			Disabled = false;
		}

		/// <summary> Flag indicates if the cache is entirely disabled </summary>
		/// <remarks> This flag is utilized, in particular, by the builder which has no access to the web's local or distributed cache </remarks>
		public static bool Disabled { get; set; }

		/// <summary> Read-only list of basic information about all the objects stored in the local cache </summary>
		public static ReadOnlyCollection<Cached_Object_Info> Locally_Cached_Objects
		{
			get
			{
				// Build the sorted list
				SortedList<string, Cached_Object_Info> sortedList = new SortedList<string, Cached_Object_Info>();
				foreach (DictionaryEntry thisItem in HttpContext.Current.Cache)
				{
					sortedList.Add(thisItem.Key.ToString(), new Cached_Object_Info(thisItem.Key.ToString(), thisItem.Value.GetType()));
				}

				// Return the readonly collection of all the values
				return new ReadOnlyCollection<Cached_Object_Info>(sortedList.Values);
			}
		}

		/// <summary> Read-only list of basic information about all the objects stored in the remote cache </summary>
		public static ReadOnlyCollection<Cached_Object_Info> Remotely_Cached_Objects
		{
			get
			{
				// Return the readonly collection of all the values
				return AppFabric_Manager.Cached_Items;
			}
		}

		/// <summary> Clears all values from the cache and caching server </summary>
		public static void Clear_Cache()
		{
			// Get collection of keys in the Cache
			List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache select thisItem.Key.ToString()).ToList();

			// Delete all items from the Cache
			foreach (string key in keys)
			{
				HttpContext.Current.Cache.Remove(key);
			}

			// Clear the list of keys in the caching server
			AppFabric_Manager.Clear_All_Keys();
		}

		#region Static methods relating to storing and retrieving (assumed private) user's folders

		/// <summary> Retrieves a (assumed private) user's folder browse by user id and folder name </summary>
		/// <param name="User_ID"> Primary key for the user </param>
		/// <param name="Folder_Name"> Name of the folder to retrieve </param>
		/// <param name="ResultsPage"> The page of matching item results to retrieve from the cache </param>
		/// <param name="Results_Per_Page">Number of results per page </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the folder browse from the cache or caching server </returns>
		public static List<iSearch_Title_Result> Retrieve_User_Folder_Browse(int User_ID, string Folder_Name, int ResultsPage, int Results_Per_Page, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_User_Folder_Browse", "");
			}

			object returnValue = HttpContext.Current.Cache.Get("USER_FOLDER_" + User_ID + "_" + Folder_Name.ToLower() + "_" + ResultsPage  + "_" + Results_Per_Page);
			return (returnValue != null) ? (List<iSearch_Title_Result>) returnValue : null;
		}

		/// <summary> Stores a (assumed private) user's folder browse into the cache or caching server </summary>
		/// <param name="User_ID"> Primary key for the user </param>
		/// <param name="Folder_Name"> Name of the folder to store </param>
		/// <param name="ResultsPage"> Page of matching item results to store in the cache </param>
		/// <param name="Results_Per_Page"> Number of result displayed per page </param>
		/// <param name="StoreObject"> Object to store  </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_User_Folder_Browse(int User_ID, string Folder_Name, int ResultsPage, int Results_Per_Page, List<iSearch_Title_Result> StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			string key = "USER_FOLDER_" + User_ID + "_" + Folder_Name.ToLower() + "_" + ResultsPage + "_" + Results_Per_Page;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_User_Folder_Browse", "Adding object '" + key + "' to the cache with expiration of 1 minute");
			}

			if (HttpContext.Current.Cache[key] == null)
			{
				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
			}
		}

		/// <summary> Retrieves a (assumed private) user's folder browse by user id and folder name </summary>
		/// <param name="User_ID"> Primary key for the user </param>
		/// <param name="Folder_Name"> Name of the folder to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the folder browse from the cache or caching server </returns>
		public static Search_Results_Statistics Retrieve_User_Folder_Browse_Statistics(int User_ID, string Folder_Name, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_User_Folder_Browse_Statistics", "");
			}

			object returnValue = HttpContext.Current.Cache.Get("USER_FOLDER_" + User_ID + "_" + Folder_Name.ToLower() + "_STATISTICS");
			return (returnValue != null) ? (Search_Results_Statistics) returnValue : null;
		}

		/// <summary> Stores a (assumed private) user's folder browse into the cache or caching server </summary>
		/// <param name="User_ID"> Primary key for the user </param>
		/// <param name="Folder_Name"> Name of the folder to store </param>
		/// <param name="StoreObject"> Object to store  </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_User_Folder_Browse_Statistics(int User_ID, string Folder_Name, Search_Results_Statistics StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			string key = "USER_FOLDER_" + User_ID + "_" + Folder_Name.ToLower() + "_STATISTICS";

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_User_Folder_Browse_Statistics", "Adding object '" + key + "' to the cache with expiration of 1 minute");
			}

			if (HttpContext.Current.Cache[key] == null)
			{
				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
			}
		}

		/// <summary> Removes a (assumed private) user's folder browse from the cache or caching server </summary>
		/// <param name="User_ID"> Primary key for the user </param>
		/// <param name="Folder_Name"> Name of the folder to remove from the cache </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_User_Folder_Browse(int User_ID, string Folder_Name, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			string key_start = "USER_FOLDER_" + User_ID + "_" + Folder_Name.ToLower()+ "_";

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Remove_User_Folder_Browse", "Removing objects '" + key_start + "_*' from the cache");
			}

			// Get collection of keys in the Cache
			List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache where thisItem.Key.ToString().IndexOf(key_start) == 0 select thisItem.Key.ToString()).ToList();

			// Delete all items from the Cache
			foreach (string key in keys)
			{
				HttpContext.Current.Cache.Remove(key);
			}
		}

		/// <summary> Removes all the user folder browses for a particular user from the cache </summary>
		/// <param name="User_ID"> Primary key for the user </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_All_User_Folder_Browses(int User_ID, Custom_Tracer Tracer )
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Remove_All_User_Folder_Browses");
			}

			string key_start = "USER_FOLDER_" + User_ID + "_";

			// Get collection of keys in the Cache
			List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache where thisItem.Key.ToString().IndexOf(key_start) == 0 select thisItem.Key.ToString()).ToList();

			// Delete all items from the Cache
			foreach (string key in keys)
			{
				HttpContext.Current.Cache.Remove(key);
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving of public folder (info and browse)

		/// <summary> Retrieves a built public folder object from the cache or caching server </summary>
		/// <param name="UserFolderID"> Primary key for the user folder to retrieve </param>
		/// <returns> Either NULL or the public folder information from the cache or caching server </returns>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static Public_User_Folder Retrieve_Public_Folder_Info(int UserFolderID, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Public_Folder_Info", "");
			}

			object returnValue = HttpContext.Current.Cache.Get("FOLDER_INFO_" + UserFolderID);
			return (returnValue != null) ? (Public_User_Folder) returnValue : null;
		}

		/// <summary> Retrieves a public folder browse from the cache or caching server </summary>
		/// <param name="UserFolderID"> Primary key for the user folder browse to retrieve </param>
		/// <param name="ResultsPage"> The page of matching item results to retrieve from the cache </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the public folder browse from the cache or caching server </returns>
		public static List<iSearch_Title_Result> Retrieve_Public_Folder_Browse(int UserFolderID, int ResultsPage, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Public_Folder_Browse", "");
			}

			object returnValue = HttpContext.Current.Cache.Get("FOLDER_BROWSE_" + UserFolderID + "_" + ResultsPage);
			return (returnValue != null) ? (List<iSearch_Title_Result>) returnValue : null;
		}

		/// <summary> Retrieves a public folder browse from the cache or caching server </summary>
		/// <param name="UserFolderID"> Primary key for the user folder browse to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the public folder browse from the cache or caching server </returns>
		public static Search_Results_Statistics Retrieve_Public_Folder_Statistics(int UserFolderID, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Public_Folder_Browse", "");
			}

			object returnValue = HttpContext.Current.Cache.Get("FOLDER_BROWSE_" + UserFolderID + "_STATISTICS");
			return (returnValue != null) ? (Search_Results_Statistics) returnValue : null;
		}

		/// <summary> Stores a public folder object into the cache or caching server </summary>
		/// <param name="StoreObject"> Object to store (which contains the primary key) </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Public_Folder_Info( Public_User_Folder StoreObject, Custom_Tracer Tracer )
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			string key = "FOLDER_INFO_" + StoreObject.UserFolderID;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_Public_Folder_Info", "Adding object '" + key + "' to the cache with expiration of 2 minutes");
			}

			if ( HttpContext.Current.Cache[key] == null)
			{
				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2));
			}
		}

		/// <summary> Stores a public folder browse into the cache or caching server </summary>
		/// <param name="UserFolderID"> Primary key for the user folder browse to store </param>
		/// <param name="ResultsPage"> The page of matching item results to store within the cache </param>
		/// <param name="StoreObject"> Object to store </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Public_Folder_Browse(int UserFolderID, int ResultsPage, List<iSearch_Title_Result> StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			string key = "FOLDER_BROWSE_" + UserFolderID + "_" + ResultsPage;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_Public_Folder_Browse", "Adding object '" + key + "' to the cache with expiration of 2 minutes");
			}

			if (HttpContext.Current.Cache[key] == null)
			{
				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2));
			}
		}

		/// <summary> Stores a public folder browse into the cache or caching server </summary>
		/// <param name="UserFolderID"> Primary key for the user folder browse to store </param>
		/// <param name="StoreObject"> Object to store </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Public_Folder_Statistics(int UserFolderID, Search_Results_Statistics StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			string key = "FOLDER_BROWSE_" + UserFolderID + "_STATISTICS";

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_Public_Folder_Browse", "Adding object '" + key + "' to the cache with expiration of 2 minutes");
			}

			if (HttpContext.Current.Cache[key] == null)
			{
				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2));
			}
		}

		/// <summary> Clears the related public folder information, if it exists </summary>
		/// <param name="UserFolderID"> Primary key for the user folder info and browse to clear </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Clear_Public_Folder_Info(int UserFolderID, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Clear_Public_Folder_Info", "");
			}

			HttpContext.Current.Cache.Remove( "FOLDER_INFO_" + UserFolderID);

			string key_start = "FOLDER_BROWSE_" + UserFolderID + "_";

			// Get collection of keys in the Cache
			List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache where thisItem.Key.ToString().IndexOf(key_start) == 0 select thisItem.Key.ToString()).ToList();

			// Delete all items from the Cache
			foreach (string key in keys)
			{
				HttpContext.Current.Cache.Remove(key);
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving information about the entire set of results (not just paged results)

		/// <summary> Retrieves the table of search results from the cache or caching server </summary>
		/// <param name="Aggregation_Code"> Aggregation code for the browse statistics to retrieve from the cache </param>
		/// <param name="Browse_Name"> Name of the browse to retrieve the browse statistics from the cache</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the search results item/title list </returns>
		public static Search_Results_Statistics Retrieve_Browse_Result_Statistics(string Aggregation_Code, string Browse_Name, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Browse_Result_Statistics", "");
			}

			// Determine the key
			string key = "TOTALBROWSE_" + Aggregation_Code.ToUpper() + "_" + Browse_Name.ToUpper();

			// Try to get this from the caching server if enabled
			if (caching_serving_enabled)
			{
				object from_appfabric = AppFabric_Manager.Get(key, Tracer);
				if (from_appfabric != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Browse_Result_Statistics", "Results pulled from caching server");
					}

					return (Search_Results_Statistics)from_appfabric;
				}
			}

			// Try to get this from the local cache next
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Browse_Result_Statistics", "Results pulled from local cache");
				}

				return (Search_Results_Statistics)returnValue;
			}
			
			return null;
		}

		/// <summary> Stores the table of search results to the cache or caching server </summary>
		/// <param name="Aggregation_Code"> Aggregation code for the browse statistics to store in the cache </param>
		/// <param name="Browse_Name"> Name of the browse to store the browse statistics in the cache</param>
		/// <param name="StoreObject"> Search results item/title list </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Browse_Result_Statistics(string Aggregation_Code, string Browse_Name, Search_Results_Statistics StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key = "TOTALBROWSE_" + Aggregation_Code.ToUpper() + "_" + Browse_Name.ToUpper();

			// try to store in the caching server, if enabled
			if (caching_serving_enabled)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Browse_Result_Statistics", "Adding object '" + key + "' to the caching server");
				}

				if (AppFabric_Manager.Add(key, StoreObject, Tracer))
					return;
			}

			// Store this on the local cache, if not there and storing on the cache server failed
			if (HttpContext.Current.Cache[key] == null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Browse_Result_Statistics", "Adding object '" + key + "' to the local cache with expiration of 3 minutes");
				}

				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(3));
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving a page of a browse and info result dataset

		/// <summary> Retrieves a result set for browsing a set of items underneath an item aggregation </summary>
		/// <param name="Aggregation_Code"> Code for the item aggregation being browsed </param>
		/// <param name="Browse_Name"> Name of this browse (or info) </param>
		/// <param name="Page"> Page of the browse results to retrieve </param>
		/// <param name="Sort"> Sort type for the current browse results to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the item browse from the cache or caching server </returns>
		public static List<iSearch_Title_Result> Retrieve_Browse_Results( string Aggregation_Code, string Browse_Name, int Page, int Sort, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Aggregation_Browse", "");
			}

			// Determine the key
			string key = "PAGEDBROWSE_" + Aggregation_Code.ToUpper() + "_" + Browse_Name.ToUpper() + "_" + Sort + "_" + Page;

			// Try to get this object from the local cache first
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Aggregation_Browse", "Results pulled from local cache");
				}

				return (List<iSearch_Title_Result>)returnValue;
			}

			// If the caching server is enabled get the data from there
			if (caching_serving_enabled)
			{
				object serialize_data = AppFabric_Manager.Get(key, Tracer);
				if (serialize_data != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Aggregation_Browse", "Results pulled from caching server");
					}

					return (List<iSearch_Title_Result>)serialize_data;
				}
			}

			return null;

		}

		/// <summary> Stores a result set for browsing a set of items underneath an item aggregation </summary>
		/// <param name="Aggregation_Code"> Code for the item aggregation being browsed</param>
		/// <param name="Browse_Name"> Name of this browse (or info)</param>
		/// <param name="Page"> Page of the browse results to store </param>
		/// <param name="Sort"> Sort type for the current browse results to store </param>
		/// <param name="StoreObject"> Result set of items and titles for this browse </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Browse_Results( string Aggregation_Code, string Browse_Name, int Page, int Sort, List<List<iSearch_Title_Result>> StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Save the requested page of results and any additionally returned pages
			int currentpage = Page;
			foreach (List<iSearch_Title_Result> pageOfResults in StoreObject)
			{
				// Determine the key 
				string key = "PAGEDBROWSE_" + Aggregation_Code.ToUpper() + "_" + Browse_Name.ToUpper() + "_" + + Sort + "_" +currentpage;

				// try to store in the caching server, if enabled
				bool remotely_cached = false;
				if (caching_serving_enabled)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Store_Info_Browse", "Adding object '" + key + "' to the caching server");
					}

					if (AppFabric_Manager.Add(key, pageOfResults, Tracer))
						remotely_cached = true;
				}

				// Store this on the local cache, if not there and storing on the cache server failed
				if ((HttpContext.Current.Cache[key] == null) && (( !remotely_cached )))
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Store_Info_Browse", "Adding object '" + key + "' to the local cache with expiration of 3 minutes");
					}

					HttpContext.Current.Cache.Insert(key, pageOfResults, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(3));
				}

				currentpage++;
			}
			 

		}

		#endregion

		#region Static methods relating to storing the metadata browse (browse by..)

		/// <summary> Retrieves a metadata browse list for an item aggregation </summary>
		/// <param name="Aggregation_Code"> Code for the item aggregation being metadata browsed </param>
		/// <param name="Browse_Name"> Name of the metadata field being browsed </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the metadata browse from the cache or caching server </returns>
		public static List<string> Retrieve_Aggregation_Metadata_Browse(string Aggregation_Code, string Browse_Name, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Aggregation_Metadata_Browse", "");
			}

			// Determine the key
			string key = "BROWSEBY_" + Aggregation_Code.ToUpper() + "_" + Browse_Name.ToUpper();

			// If the caching server is enabled get the data from there
			if (caching_serving_enabled)
			{
				object serialize_data = AppFabric_Manager.Get(key, Tracer);
				if (serialize_data != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Aggregation_Metadata_Browse", "Metadata browse pulled from caching server");
					}

					return (List<string>)serialize_data;
				}
			}

			// Try to get this object from the local cache if above fails
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Aggregation_Metadata_Browse", "Metadata browse pulled from local cache");
				}

				return (List<string>)returnValue;
			}
			
			return null;
		}

		/// <summary> Stores a list of metadata fields for metadata browsing an item aggregation </summary>
		/// <param name="Aggregation_Code"> Code for the item aggregation being metadata browsed</param>
		/// <param name="Browse_Name">  Name of the metadata field being browsed </param>
		/// <param name="StoreObject"> List of all the metadata for the metadata field being browsed </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Aggregation_Metadata_Browse(string Aggregation_Code, string Browse_Name, List<string> StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key 
			string key = "BROWSEBY_" + Aggregation_Code.ToUpper() + "_" + Browse_Name.ToUpper();

			// try to store in the caching server, if enabled
			if (caching_serving_enabled)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Aggregation_Metadata_Browse", "Adding object '" + key + "' to the caching server");
				}

				if (AppFabric_Manager.Add(key, StoreObject, Tracer))
					return;
			}

			// Store this on the local cache, if not there and storing on the cache server failed
			if (HttpContext.Current.Cache[key] == null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Aggregation_Metadata_Browse", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
				}

				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
			}
		}


		#endregion

		#region Static methods relating to storing and retrieving digital resource objects

		/// <summary> Removes all digital resource objects from the cache or caching server, for a given BibID </summary>
		/// <param name="BibID"> Bibliographic Identifier for the digital resources to remove </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_Digital_Resource_Objects(string BibID, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Remove_Digital_Resource_Objects", "");
			}

			string key_start = "ITEM_" + BibID + "_";

			// Get the list of objects locally cached
			ReadOnlyCollection<Cached_Object_Info> locally_cached_objects = Locally_Cached_Objects;
			List<string> keys_to_expire = (from cachedObject in locally_cached_objects where cachedObject.Object_Key.IndexOf(key_start) == 0 select cachedObject.Object_Key).ToList();

			// Clear these from the local cache
			foreach (string expireKey in keys_to_expire)
			{
				HttpContext.Current.Cache.Remove(expireKey);
			}

			// Remove the keys from the caching server list as well
			AppFabric_Manager.Expire_Items(key_start);

			// Now, remove the actual item group
			string key = "ITEM_GROUP_" + BibID;

			// Clear the item group from the local cache
			HttpContext.Current.Cache.Remove(key);

			// Remove the item group from the caching server list as well
			AppFabric_Manager.Expire_Item(key);
		}

		/// <summary> Removes a global digital resource object from the cache or caching server, it it exists </summary>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource to remove </param>
		/// <param name="VID"> Volume Identifier for the digital resource to remove </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_Digital_Resource_Object( string BibID, string VID, Custom_Tracer Tracer)
		{
			Remove_Digital_Resource_Object( -1, BibID, VID, Tracer);
		}

		/// <summary> Removes a user-specific digital resource object from the cache or caching server, it it exists </summary>
		/// <param name="UserID"> Primary key of the user, if this should be removed from the user-specific cache</param>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource to remove </param>
		/// <param name="VID"> Volume Identifier for the digital resource to remove </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_Digital_Resource_Object(int UserID, string BibID, string VID, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Remove_Digital_Resource_Object", "");
			}

			// Determine the key
			string key = "ITEM_" + BibID + "_" + VID;
			if (UserID > 0)
			{
				key = "USERITEM" + UserID + "_" + key;
			}

			// Clear this from the local cache
			HttpContext.Current.Cache.Remove(key);

			// Remove the key from the caching server list as well
			AppFabric_Manager.Expire_Item(key);
		}


		/// <summary> Retrieves a user-specific digital resource object from the cache or caching server </summary>
		/// <param name="UserID"> Primary key of the user, if this should be pulled from the user-specific cache </param>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
		/// <param name="VID"> Volume Identifier for the digital resource to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the digital resource object from the cache or caching server </returns>
		public static SobekCM_Item Retrieve_Digital_Resource_Object( int UserID, string BibID, string VID, Custom_Tracer Tracer )
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			// Determine the key
			string key = "ITEM_" + BibID + "_" + VID;
			if (UserID > 0)
			{
				key = "USERITEM" + UserID + "_" + key;
			}

			// See if this is in the local cache first
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Digital_Resource_Object", "Found item on local cache");
				}

				return (SobekCM_Item)returnValue;
			}

			// Try to get this from the caching server, if enabled
			if ((caching_serving_enabled) && ( AppFabric_Manager.Contains( key )))
			{
				object from_app_fabric = AppFabric_Manager.Get(key, Tracer);
				if (from_app_fabric != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Digital_Resource_Object", "Found item on caching server");
					}

					// Check the number of items currently locally cached 
					if (LOCALLY_CACHED_ITEM_LIMIT > 0)
					{
						int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(thisItem => (thisItem.Key.ToString().IndexOf("ITEM_") == 0) || (thisItem.Key.ToString().IndexOf("USERITEM") == 0));

						// Locally cache if this doesn't exceed the limit
						if (items_cached < LOCALLY_CACHED_ITEM_LIMIT)
						{
							if (Tracer != null)
							{
								Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Digital_Resource_Object", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
							}

							HttpContext.Current.Cache.Insert(key, from_app_fabric, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
						}
					}

					return (SobekCM_Item)from_app_fabric;
				}
			}

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Digital_Resource_Object", "Item not found in either the local cache or caching server");
			}

			// Since everything failed, just return null
			return null;
		}

		/// <summary> Retrieves a global digital resource object from the cache or caching server </summary>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
		/// <param name="VID"> Volume Identifier for the digital resource to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the digital resource object from the cache or caching server </returns>
		public static SobekCM_Item Retrieve_Digital_Resource_Object(string BibID, string VID, Custom_Tracer Tracer)
		{
			return Retrieve_Digital_Resource_Object(-1, BibID, VID, Tracer);
		}


		/// <summary> Store a user-specific digital resource object on the cache or caching server  </summary>
		/// <param name="UserID"> Primary key of the user, if this should be stored in a user-specific cache </param>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
		/// <param name="VID"> Volume Identifier for the digital resource to store </param>
		/// <param name="StoreObject"> Digital Resource object to store for later retrieval </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Digital_Resource_Object(int UserID, string BibID, string VID, SobekCM_Item StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key = "ITEM_" + BibID + "_" + VID;
			if (UserID > 0)
			{
				key = "USERITEM" + UserID + "_" + key;
			}

			// Check the number of items currently locally cached
			if ((LOCALLY_CACHED_ITEM_LIMIT > 0) || ( !caching_serving_enabled) || (UserID > 0 ))
			{
				int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => (ThisItem.Key.ToString().IndexOf("ITEM_") == 0) || (ThisItem.Key.ToString().IndexOf("USERITEM") == 0));

				// Locally cache if this doesn't exceed the limit
				if ((items_cached < LOCALLY_CACHED_ITEM_LIMIT) || (!caching_serving_enabled) || (UserID > 0))
				{
					int length_of_time = 1;
					if (UserID > 0)
						length_of_time = 15;

					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Store_Digital_Resource_Object", "Adding object '" + key + "' to the local cache with expiration of " + length_of_time + " minute");
					}

					HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(length_of_time));
				}
			}


			// try to store in the caching server, if enabled
			if (caching_serving_enabled)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Digital_Resource_Object", "Adding object '" + key + "' to the caching server");
				}

				AppFabric_Manager.Add(key, StoreObject, Tracer);
			}
		}

		/// <summary> Store a global digital resource object on the cache or caching server  </summary>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
		/// <param name="VID"> Volume Identifier for the digital resource to store </param>
		/// <param name="StoreObject"> Digital Resource object to store for later retrieval </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Digital_Resource_Object(string BibID, string VID, SobekCM_Item StoreObject, Custom_Tracer Tracer)
		{
			Store_Digital_Resource_Object(-1, BibID, VID, StoreObject, Tracer);
		}

		/// <summary> Retrieves the title-level object from the cache or caching server </summary>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the digital resource object from the cache or caching server </returns>
		public static SobekCM_Item Retrieve_Digital_Resource_Object( string BibID, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			// Determine the key
			string key = "ITEM_GROUP_" + BibID;

			// See if this is in the local cache first
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Digital_Resource_Object", "Found item on local cache");
				}

				return (SobekCM_Item)returnValue;
			}

			// Try to get this from the caching server, if enabled
			if ((caching_serving_enabled) && (AppFabric_Manager.Contains(key)))
			{
				object from_app_fabric = AppFabric_Manager.Get(key, Tracer);
				if (from_app_fabric != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Digital_Resource_Object", "Found item on caching server");
					}

					// Check the number of items currently locally cached
					if (LOCALLY_CACHED_ITEM_LIMIT > 0)
					{
						int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => (ThisItem.Key.ToString().IndexOf("ITEM_") == 0) || (ThisItem.Key.ToString().IndexOf("USERITEM") == 0));
						// Locally cache if this doesn't exceed the limit
						if (items_cached < LOCALLY_CACHED_ITEM_LIMIT)
						{
							if (Tracer != null)
							{
								Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Digital_Resource_Object", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
							}

							HttpContext.Current.Cache.Insert(key, from_app_fabric, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
						}
					}

					return (SobekCM_Item)from_app_fabric;
				}
			}

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Digital_Resource_Object", "Item not found in either the local cache or caching server");
			}

			// Since everything failed, just return null
			return null;

		}

		/// <summary> Stores the title-level digital resource object on the cache or caching server  </summary>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource to store </param>
		/// <param name="StoreObject"> Digital Resource object to store for later retrieval </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Digital_Resource_Object( string BibID, SobekCM_Item StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key = "ITEM_GROUP_" + BibID;

			// Check the number of items currently cached
			if ((LOCALLY_CACHED_ITEM_LIMIT > 0) || ( !caching_serving_enabled ))
			{
				int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => (ThisItem.Key.ToString().IndexOf("ITEM_") == 0) || (ThisItem.Key.ToString().IndexOf("USERITEM") == 0));

				// Locally cache if this doesn't exceed the limit
				if ( (items_cached < LOCALLY_CACHED_ITEM_LIMIT) || ( !caching_serving_enabled ))
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Store_Digital_Resource_Object", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
					}

					HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
				}
			}

			// try to store in the caching server, if enabled
			if (caching_serving_enabled)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Digital_Resource_Object", "Adding object '" + key + "' to the caching server");
				}

				AppFabric_Manager.Add(key, StoreObject, Tracer);
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving lists of digital volumes within one title

		/// <summary> Retrieves the list of items for a single bibid from the cache or caching server </summary>
		/// <param name="BibID"> Bibliographic identifier for the list of items </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the list of items within a single title </returns>
		public static SobekCM_Items_In_Title Retrieve_Items_In_Title( string BibID, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Items_In_Title", "");
			}

			// Try to get this from the local cache next
			object returnValue = HttpContext.Current.Cache.Get("ITEMLIST_" + BibID );
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Items_In_Title", "List of items in title pulled from local cache");
				}

				return (SobekCM_Items_In_Title)returnValue;
			}
			
			return null;
		}

		/// <summary> Stores the list of items for a single bibid to the cache or caching server </summary>
		/// <param name="BibID"> Bibliographic identifier for the list of items </param>
		/// <param name="StoreObject"> List of items within the single title </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Items_In_Title( string BibID , SobekCM_Items_In_Title StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_Items_In_Title", "");
			}

			// Store this on the local cache, if not there and storing on the cache server failed
			string key = "ITEMLIST_" + BibID;
			if (HttpContext.Current.Cache[key] == null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Items_In_Title", "Adding object '" + key + "' to the local cache with expiration of 1 minutes");
				}

				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
			}
		}

		/// <summary> Removes the  list of items for a single bibid to the cache or caching server </summary>
		/// <param name="BibID"> Bibliographic identifier for the list of items </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_Items_In_Title(string BibID, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Remove_Items_In_Title", "");
			}

			// Store this on the local cache, if not there and storing on the cache server failed
			string key = "ITEMLIST_" + BibID;

			// Clear this from the local cache
			HttpContext.Current.Cache.Remove(key);

			// Clear from the remote cache
			AppFabric_Manager.Expire_Item(key);
		}

		#endregion

		#region Static methods relating to storing and retrieving information about the entire set of results (not just paged results)

		/// <summary> Retrieves the table of search results from the cache or caching server </summary>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Terms"> List of all search terms for the search result statistics to retrieve </param>
		/// <param name="Fields"> List of all search fields for the search result statistics to retrieve </param>
		/// <param name="DateRange_Start"> Beginning of a date range search, or -1 </param>
		/// <param name="DateRange_End"> End of a date range search, or -1 </param>
		/// <param name="Count"> Number of fields or terms to include in the key for this result </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the search results item/title list </returns>
		public static Search_Results_Statistics Retrieve_Search_Result_Statistics( SobekCM_Navigation_Object Current_Mode, int Count, List<string> Fields, List<string> Terms, long DateRange_Start, long DateRange_End, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Search_Result_Statistics", "");
			}

			// Determine the key
			// If there is no aggregation listed, use 'all'
			string aggregation_code = Current_Mode.Aggregation.ToLower();
			if (aggregation_code.Length == 0)
				aggregation_code = "all";

			// Determine the search precision
			string precision = "results";
			switch (Current_Mode.Search_Precision)
			{
				case Search_Precision_Type_Enum.Contains:
					precision = "contains";
					break;

				case Search_Precision_Type_Enum.Exact_Match:
					precision = "exact";
					break;

				case Search_Precision_Type_Enum.Synonmic_Form:
					precision = "like";
					break;
			}

			// Start to build the key
			StringBuilder keyBuilder = new StringBuilder("TOTALRESULTS_" + precision + "_" + aggregation_code + "_T_");
			for (int i = 0; i < Count; i++)
			{
				keyBuilder.Append(Terms[i].ToLower() + "_");
			}
			keyBuilder.Append("F_");
			for (int i = 0; i < Count; i++)
			{
				keyBuilder.Append(Fields[i] + "_");
			}

			// Add possivle date range search restrction to the key
			if (DateRange_Start >= 0)
			{
				keyBuilder.Append("_DATE" + DateRange_Start);
				if (DateRange_End >= 0)
				{
					keyBuilder.Append("-" + DateRange_End);
				}
			}

			string key = keyBuilder.ToString();

			//if (Current_Mode.SubAggregation.Length > 0)
			//{
			//    key = "a_" + precision + "_" + aggregation_code + "s_" + Current_Mode.SubAggregation + "t_" + Current_Mode.Search_String + "f_" + search_fields;
			//}
			if ((Current_Mode.Search_String.Length == 0) && (Current_Mode.Coordinates.Length > 0))
			{
				key = "TOTALRESULTS_" + precision + "_" + aggregation_code + "coord_" + Current_Mode.Coordinates;
			}

			// Try to get this from the local cache first
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Search_Result_Statistics", "Results pulled from local cache");
				}

				return (Search_Results_Statistics)returnValue;
			}

			// Try to get this from the caching server if enabled
			if (caching_serving_enabled)
			{
				object from_appfabric = AppFabric_Manager.Get( key, Tracer);
				if (from_appfabric != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Search_Result_Statistics", "Results pulled from caching server");
					}

					return (Search_Results_Statistics)from_appfabric;
				}
			}

			return null;
		}

		/// <summary> Stores the table of search results to the cache or caching server </summary>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Terms"> List of all search terms for the search result statistics to store </param>
		/// <param name="Fields"> List of all search fields for the search result statistics to store </param>
		/// <param name="Count"> Number of fields or terms to include in the key for this result </param>
		/// <param name="DateRange_Start"> Beginning of a date range search, or -1 </param>
		/// <param name="DateRange_End"> End of a date range search, or -1 </param>
		/// <param name="StoreObject"> Search results item/title list </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Search_Result_Statistics(SobekCM_Navigation_Object Current_Mode, int Count, List<string> Fields, List<string> Terms, long DateRange_Start, long DateRange_End, Search_Results_Statistics StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;
			// Determine the key
			// If there is no aggregation listed, use 'all'
			string aggregation_code = Current_Mode.Aggregation.ToLower();
			if (aggregation_code.Length == 0)
				aggregation_code = "all";

			// Determine the search precision
			string precision = "results";
			switch (Current_Mode.Search_Precision)
			{
				case Search_Precision_Type_Enum.Contains:
					precision = "contains";
					break;

				case Search_Precision_Type_Enum.Exact_Match:
					precision = "exact";
					break;

				case Search_Precision_Type_Enum.Synonmic_Form:
					precision = "like";
					break;
			}

			// Start to build the key
			StringBuilder keyBuilder = new StringBuilder("TOTALRESULTS_" + precision + "_" + aggregation_code + "_T_");
			for (int i = 0; i < Count; i++)
			{
				keyBuilder.Append(Terms[i].ToLower() + "_");
			}
			keyBuilder.Append("F_");
			for (int i = 0; i < Count; i++)
			{
				keyBuilder.Append(Fields[i] + "_");
			}

			// Add possivle date range search restrction to the key
			if (DateRange_Start >= 0)
			{
				keyBuilder.Append("_DATE" + DateRange_Start);
				if (DateRange_End >= 0)
				{
					keyBuilder.Append("-" + DateRange_End);
				}
			}

			string key = keyBuilder.ToString();

			//if (Current_Mode.SubAggregation.Length > 0)
			//{
			//    key = "a_" + precision + "_" + aggregation_code + "s_" + Current_Mode.SubAggregation + "t_" + Current_Mode.Search_String + "f_" + search_fields;
			//}
			if ((Current_Mode.Search_String.Length == 0) && (Current_Mode.Coordinates.Length > 0))
			{
				key = "TOTALRESULTS_" + precision + "_" + aggregation_code + "coord_" + Current_Mode.Coordinates;
			}

			// try to store in the caching server, if enabled
			if (caching_serving_enabled)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Search_Result_Statistics", "Adding object '" + key + "' to the caching server");
				}

				if (AppFabric_Manager.Add(key, StoreObject, Tracer))
					return;
			}

			// Store this on the local cache, if not there and storing on the cache server failed
			if (HttpContext.Current.Cache[key] == null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Search_Result_Statistics", "Adding object '" + key + "' to the local cache with expiration of 3 minutes");
				}

				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(3));
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving a single page of search results

		/// <summary> Retrieves the table of search results from the cache or caching server </summary>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Terms"> List of all search terms for the search result to retrieve </param>
		/// <param name="Fields"> List of all search fields for the search result to retrieve </param>
		/// <param name="Sort"> Sort for the current search results to retrieve </param>
		/// <param name="Count"> Number of fields or terms to include in the key for this result </param>
		/// <param name="DateRange_Start"> Beginning of a date range search, or -1 </param>
		/// <param name="DateRange_End"> End of a date range search, or -1 </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the search results item/title list </returns>
		public static List<iSearch_Title_Result> Retrieve_Search_Results(SobekCM_Navigation_Object Current_Mode, int Sort, int Count, List<string> Fields, List<string> Terms, long DateRange_Start, long DateRange_End, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Search_Results", "");
			}

			// Determine the key
			// If there is no aggregation listed, use 'all'
			string aggregation_code = Current_Mode.Aggregation;
			if (aggregation_code.Length == 0)
				aggregation_code = "all";

			// Determine the search precision
			string precision = "results";
			switch (Current_Mode.Search_Precision)
			{
				case Search_Precision_Type_Enum.Contains:
					precision = "contains";
					break;

				case Search_Precision_Type_Enum.Exact_Match:
					precision = "exact";
					break;

				case Search_Precision_Type_Enum.Synonmic_Form:
					precision = "like";
					break;
			}

			// Start to build the key
			StringBuilder keyBuilder = new StringBuilder("PAGEDRESULTS_" + precision + "_" + aggregation_code + "_" + Current_Mode.Page + "_T_");
			for (int i = 0; i < Count; i++)
			{
				keyBuilder.Append(Terms[i].ToLower() + "_");
			}
			keyBuilder.Append("F_");
			for (int i = 0; i < Count; i++)
			{
				keyBuilder.Append(Fields[i] + "_");
			}
			keyBuilder.Append(Sort);

			// Add possivle date range search restrction to the key
			if (DateRange_Start >= 0)
			{
				keyBuilder.Append("_DATE" + DateRange_Start);
				if (DateRange_End >= 0)
				{
					keyBuilder.Append("-" + DateRange_End);
				}
			}

			string key = keyBuilder.ToString();

			//if (Current_Mode.SubAggregation.Length > 0)
			//{
			//    key = "a_" + precision + "_" + aggregation_code + "s_" + Current_Mode.SubAggregation + "t_" + Current_Mode.Search_String + "f_" + search_fields;
			//}
			if ((Current_Mode.Search_String.Length == 0) && (Current_Mode.Coordinates.Length > 0))
			{
				key = "TOTALRESULTS_" + precision + "_" + aggregation_code + "coord_" + Current_Mode.Coordinates;
			}

			// Try to get this from the caching server if enabled
			if (caching_serving_enabled)
			{
				object object_from_cache = AppFabric_Manager.Get(key, Tracer);
				if (object_from_cache != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Search_Results", "Results pulled from caching server");
					}

					return (List<iSearch_Title_Result>)object_from_cache;
				}
			}

			// Try to get this from the local cache next
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Search_Results", "Results pulled from local cache");
				}

				return (List<iSearch_Title_Result>)returnValue;
			}
			
			return null;
		}

		/// <summary> Stores a single page of search results to the cache or caching server </summary>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Terms"> List of all search terms for the search result to store </param>
		/// <param name="Fields"> List of all search fields for the search result to store </param>
		/// <param name="Sort"> Sort for the current search results to store </param>
		/// <param name="Count"> Number of fields or terms to include in the key for this result </param>
		/// <param name="DateRange_Start"> Beginning of a date range search, or -1 </param>
		/// <param name="DateRange_End"> End of a date range search, or -1 </param>
		/// <param name="StoreObject"> Search results item/title list </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Search_Results(SobekCM_Navigation_Object Current_Mode, int Sort, int Count, List<string> Fields, List<string> Terms, long DateRange_Start, long DateRange_End, List<iSearch_Title_Result> StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			// If there is no aggregation listed, use 'all'
			string aggregation_code = Current_Mode.Aggregation;
			if (aggregation_code.Length == 0)
				aggregation_code = "all";

			// Determine the search precision
			string precision = "results";
			switch (Current_Mode.Search_Precision)
			{
				case Search_Precision_Type_Enum.Contains:
					precision = "contains";
					break;

				case Search_Precision_Type_Enum.Exact_Match:
					precision = "exact";
					break;

				case Search_Precision_Type_Enum.Synonmic_Form:
					precision = "like";
					break;
			}

			// Start to build the key
			StringBuilder keyBuilder = new StringBuilder("PAGEDRESULTS_" + precision + "_" + aggregation_code + "_" + Current_Mode.Page + "_T_");
			for (int i = 0; i < Count; i++)
			{
				keyBuilder.Append(Terms[i].ToLower() + "_");
			}
			keyBuilder.Append("F_");
			for (int i = 0; i < Count; i++)
			{
				keyBuilder.Append(Fields[i] + "_");
			}
			keyBuilder.Append(Sort);

			// Add possivle date range search restrction to the key
			if (DateRange_Start >= 0)
			{
				keyBuilder.Append("_DATE" + DateRange_Start);
				if (DateRange_End >= 0)
				{
					keyBuilder.Append("-" + DateRange_End);
				}
			}

			string key = keyBuilder.ToString();

			//if (Current_Mode.SubAggregation.Length > 0)
			//{
			//    key = "a_" + precision + "_" + aggregation_code + "s_" + Current_Mode.SubAggregation + "t_" + Current_Mode.Search_String + "f_" + search_fields;
			//}
			if ((Current_Mode.Search_String.Length == 0) && (Current_Mode.Coordinates.Length > 0))
			{
				key = "TOTALRESULTS_" + precision + "_" + aggregation_code + "coord_" + Current_Mode.Coordinates;
			}

			// try to store in the caching server, if enabled
			if (caching_serving_enabled)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Search_Results", "Adding object '" + key + "' to the caching server");
				}

				if (AppFabric_Manager.Add(key, StoreObject, Tracer))
					return;
			}

			// Store this on the local cache, if not there and storing on the cache server failed
			if (HttpContext.Current.Cache[key] == null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Search_Results", "Adding object '" + key + "' to the local cache with expiration of 3 minutes");
				}

				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(3));
			}
		}

		/// <summary> Store several pages of search results to the cache or caching server </summary>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Terms"> List of all search terms for the search result to store </param>
		/// <param name="Fields"> List of all search fields for the search result to store </param>
		/// <param name="Sort"> Sort for the current search results to store </param>
		/// <param name="Count"> Number of fields or terms to include in the key for this result </param>
		/// <param name="DateRange_Start"> Beginning of a date range search, or -1 </param>
		/// <param name="DateRange_End"> End of a date range search, or -1 </param>
		/// <param name="StoreObject"> Search results item/title list </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Search_Results(SobekCM_Navigation_Object Current_Mode, int Sort, int Count, List<string> Fields, List<string> Terms, long DateRange_Start, long DateRange_End, List<List<iSearch_Title_Result>> StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			// If there is no aggregation listed, use 'all'
			string aggregation_code = Current_Mode.Aggregation;
			if (aggregation_code.Length == 0)
				aggregation_code = "all";

			// Determine the search precision
			string precision = "results";
			switch (Current_Mode.Search_Precision)
			{
				case Search_Precision_Type_Enum.Contains:
					precision = "contains";
					break;

				case Search_Precision_Type_Enum.Exact_Match:
					precision = "exact";
					break;

				case Search_Precision_Type_Enum.Synonmic_Form:
					precision = "like";
					break;
			}

			// Save the requested page of results and any additionally returned pages
			int currentpage = Current_Mode.Page;
			foreach (List<iSearch_Title_Result> pageOfResults in StoreObject)
			{
				// Start to build the key
				StringBuilder keyBuilder = new StringBuilder("PAGEDRESULTS_" + precision + "_" + aggregation_code + "_" + currentpage + "_T_");
				for (int i = 0; i < Count; i++)
				{
					keyBuilder.Append(Terms[i].ToLower() + "_");
				}
				keyBuilder.Append("F_");
				for (int i = 0; i < Count; i++)
				{
					keyBuilder.Append(Fields[i] + "_");
				}
				keyBuilder.Append(Sort);


				//if (Current_Mode.SubAggregation.Length > 0)
				//{
				//    key = "a_" + precision + "_" + aggregation_code + "s_" + Current_Mode.SubAggregation + "t_" + Current_Mode.Search_String + "f_" + search_fields;
				//}

				// Add possivle date range search restrction to the key
				if (DateRange_Start >= 0)
				{
					keyBuilder.Append("_DATE" + DateRange_Start);
					if (DateRange_End >= 0)
					{
						keyBuilder.Append("-" + DateRange_End);
					}
				}

				string key = keyBuilder.ToString();
				if ((Current_Mode.Search_String.Length == 0) && (Current_Mode.Coordinates.Length > 0))
				{
					key = "TOTALRESULTS_" + precision + "_" + aggregation_code + "coord_" + Current_Mode.Coordinates;
				}

				// try to store in the caching server, if enabled
				bool remotely_cached = false;
				if (caching_serving_enabled)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Store_Search_Results", "Adding object '" + key + "' to the caching server");
					}

					if (AppFabric_Manager.Add(key, pageOfResults, Tracer))
						remotely_cached = true;
				}

				// Store this on the local cache, if not there and storing on the cache server failed
				if ((HttpContext.Current.Cache[key] == null) && (( !remotely_cached )))
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Store_Search_Results", "Adding object '" + key + "' to the local cache with expiration of 1 minutes");
					}

					HttpContext.Current.Cache.Insert(key, pageOfResults, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
				}

				currentpage++;
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving item-level search results

		/// <summary> Retrieves the item-level search results string </summary>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource against which the search was conducted </param>
		/// <param name="VID"> Volume Identifier for the digital resourceagainst which the search was conducted </param>
		/// <param name="Search_Terms"> Search term used in the search (either full text search or map search)</param>
		/// <param name="Page_Number"> Page number within the results set which are included within this object </param>
		/// <param name="Sort_By_Score"> Flag indicates if this is sorted by score, rather than sorted by page order </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the item-level search result string </returns>
		public static Solr_Page_Results Retrieve_Item_Search_Results( string BibID, string VID, string Search_Terms, int Page_Number, bool Sort_By_Score, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Item_Search_Results", "");
			}

			// Determine the key
			string key = "ITEMSEARCH_" + BibID + "_" + VID + "_" + Search_Terms.ToLower() + "_" + Page_Number;
			if (Sort_By_Score)
				key = key + "_SCORE";

			// Try to get this object
			object returnValue = HttpContext.Current.Cache.Get(key);
			return (returnValue != null) ? (Solr_Page_Results) returnValue : null;
		}

		/// <summary> Stores the item-level search results string </summary>
		/// <param name="BibID"> Bibliographic Identifier for the digital resource against which the search was conducted </param>
		/// <param name="VID"> Volume Identifier for the digital resourceagainst which the search was conducted </param>
		/// <param name="StoreObject"> Item-level search results strings </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Item_Search_Results(string BibID, string VID, Solr_Page_Results StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key = "ITEMSEARCH_" + BibID + "_" + VID + "_" + StoreObject.Query.ToLower() + "_" + StoreObject.Page_Number;
			if (StoreObject.Sort_By_Score)
				key = key + "_SCORE";

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_Item_Search_Results", "Adding object '" + key + "' to the cache with expiration of 5 minutes");
			}

			// Store this on the cache
			if (HttpContext.Current.Cache[key] == null)
			{
				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving item aggregation objects

		/// <summary> Retrieves the item aggregation obejct from the cache or caching server </summary>
		/// <param name="Aggregation_Code"> Code for the item aggregation to retrieve </param>
		/// <param name="Language_Code"> Current language code (item aggregation instances are currently language-specific)</param>
		/// <param name="Possibly_Cache_Locally"> Flag indicates whether to potentially copy to the local cache if it was present on the distant cache </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the item aggregation object </returns>
		public static Item_Aggregation Retrieve_Item_Aggregation(string Aggregation_Code, string Language_Code, bool Possibly_Cache_Locally, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Item_Aggregation", "");
			}

			// Determine the key
			string key = "AGGR_" + Aggregation_Code;
			if ((Language_Code.Length > 0) && (Language_Code != "en"))
				key = key + "_" + Language_Code;

			// See if this is in the local cache first
			Item_Aggregation returnValue = HttpContext.Current.Cache.Get(key) as Item_Aggregation;
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Item_Aggregation", "Found item aggregation on local cache");
				}

				return returnValue;
			}

			// Try to get this from the caching server, if enabled
			if ((caching_serving_enabled) && (AppFabric_Manager.Contains(key)))
			{
				object from_app_fabric = AppFabric_Manager.Get(key, Tracer);
				if (from_app_fabric != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Item_Aggregation", "Found item aggregation on caching server");
					}

					// Check the number of item aggregations currently locally cached
					if (( Possibly_Cache_Locally ) && (LOCALLY_CACHED_AGGREGATION_LIMIT > 0))
					{
						int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => ThisItem.Key.ToString().IndexOf("AGGR_") == 0);

						// Locally cache if this doesn't exceed the limit
						if (items_cached < LOCALLY_CACHED_AGGREGATION_LIMIT) 
						{
							if (Tracer != null)
							{
								Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Item_Aggregation", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
							}

							HttpContext.Current.Cache.Insert(key, from_app_fabric, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
						}
					}

					return (Item_Aggregation)from_app_fabric;
				}
			}

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Item_Aggregation", "Aggregation not found in either the local cache or caching server");
			}

			// Since everything failed, just return null
			return null;
		}

		/// <summary> Stores the item aggregation object to the cache or caching server </summary>
		/// <param name="Aggregation_Code"> Code for the item aggregation to store </param>
		/// <param name="Language_Code"> Current language code (item aggregation instances are currently language-specific)</param>
		/// <param name="StoreObject"> Item aggregation object to store for later retrieval </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Item_Aggregation(string Aggregation_Code, string Language_Code, Item_Aggregation StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key = "AGGR_" + Aggregation_Code;
			if ((Language_Code.Length > 0) && (Language_Code != "en"))
				key = key + "_" + Language_Code;

			// Check the number of item aggregations currently locally cached
			int items_cached = 0;
			int local_expiration = 15;
			if (Aggregation_Code != "all")
			{
				local_expiration = 1;
				if ((LOCALLY_CACHED_AGGREGATION_LIMIT > 0) && ( caching_serving_enabled ))
				{
					items_cached += HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => ThisItem.Key.ToString().IndexOf("AGGR_") == 0);
				}
			}

			// Locally cache if this doesn't exceed the limit
			if ((items_cached < LOCALLY_CACHED_AGGREGATION_LIMIT) || (Aggregation_Code == "all") || (!caching_serving_enabled))
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Item_Aggregation", "Adding object '" + key + "' to the local cache with expiration of " + local_expiration + " minute(s)");
				}

				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(local_expiration));
			}

			// try to store in the caching server, if enabled
			if ((caching_serving_enabled) && ( Aggregation_Code != "all" ))
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Item_Aggregation", "Adding object '" + key + "' to the caching server");
				}

				AppFabric_Manager.Add(key, StoreObject, Tracer);
			}
		}

		/// <summary> Removes all references to a particular item aggregation from the cache or caching server </summary>
		/// <param name="Aggregation_Code"> Code for the item aggregation to remove </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_Item_Aggregation(string Aggregation_Code, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key_nolanguage = "AGGR_" + Aggregation_Code;
			string key_start = "AGGR_" + Aggregation_Code + "_";

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Remove_Item_Aggregation", "Removing item aggregation '" + Aggregation_Code + "' from the cache");
			}

			// Get collection of keys in the Cache
			List<string> keys = (from DictionaryEntry thisItem in HttpContext.Current.Cache where (thisItem.Key.ToString() == key_nolanguage) || (thisItem.Key.ToString().IndexOf(key_start) == 0) select thisItem.Key.ToString()).ToList();

			// Delete all items from the Cache
			foreach (string key in keys)
			{
				HttpContext.Current.Cache.Remove(key);
			}

			// Do the same thing for the remote cache
			keys.Clear();
			keys.AddRange(from cachedObject in AppFabric_Manager.Cached_Items where (cachedObject.Object_Key == key_nolanguage) || (cachedObject.Object_Key.IndexOf(key_start) == 0) select cachedObject.Object_Key);

			// Delete all items from the Cache
			foreach (string key in keys)
			{
				AppFabric_Manager.Expire_Item(key);
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving templates (for online submission and editing)

		/// <summary> Retrieves the template ( for online submission and editing ) from the cache or caching server </summary>
		/// <param name="Template_Code"> Code which specifies the template to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Requested template object for online submissions and editing</returns>
		public static Template Retrieve_Template(string Template_Code, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Template", "");
			}

			// Determine the key
			string key = "TEMPLATE_" + Template_Code;

			// Try to get this object
			object returnValue = HttpContext.Current.Cache.Get(key);
			return (returnValue != null) ? (Template) returnValue : null;
		}

		/// <summary> Stores the template ( for online submission and editing ) to the cache or caching server </summary>
		/// <param name="Template_Code"> Code for the template to store </param>
		/// <param name="StoreObject"> Template object for online submissions and editing to store</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Template(string Template_Code, Template StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key = "TEMPLATE_" + Template_Code;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_Template", "Adding object '" + key + "' to the cache with expiration of thirty minutes");
			}

			// Store this on the cache
			if (HttpContext.Current.Cache[key] == null)
			{
				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving project objects

		/// <summary> Removes a global project object from the cache or caching server, it it exists </summary>
		/// <param name="Project_Code"> Code which identifies the project </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_Project(string Project_Code, Custom_Tracer Tracer)
		{
			Remove_Project(-1, Project_Code, Tracer);
		}

		/// <summary> Removes a user-specific project object from the cache or caching server, it it exists </summary>
		/// <param name="UserID"> Primary key of the user, if this should be removed from the user-specific cache</param>
		/// <param name="Project_Code"> Code which identifies the project </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Remove_Project( int UserID, string Project_Code, Custom_Tracer Tracer )
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Remove_Project", "");
			}


			// Determine the key
			string key = "PROJECT_" + Project_Code.ToLower();
			if (UserID > 0)
			{
				key = "USER" + UserID + "_" + key;
			}

			// Clear this from the local cache
			HttpContext.Current.Cache.Remove(key);
		}

		/// <summary> Retrieves a global project object from the cache or caching server </summary>
		/// <param name="Project_Code"> Code which identifies the project </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the project from the cache or caching server </returns>
		public static SobekCM_Item Retrieve_Project( string Project_Code, Custom_Tracer Tracer)
		{
			return Retrieve_Project(-1, Project_Code, Tracer);
		}

		/// <summary> Retrieves a user-specific project object from the cache or caching server </summary>
		/// <param name="UserID"> Primary key of the user, if this should be pulled from the user-specific cache </param>
		/// <param name="Project_Code"> Code which identifies the project </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the project from the cache or caching server </returns>
		public static SobekCM_Item Retrieve_Project(int UserID, string Project_Code, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Project", "");
			}

			// Determine the key
			string key = "PROJECT_" + Project_Code.ToLower();
			if (UserID > 0)
			{
				key = "USER" + UserID + "_" + key;
			}

			// Try to get this object
			object returnValue = HttpContext.Current.Cache.Get(key);
			return (returnValue != null) ? (SobekCM_Item) returnValue : null;
		}

		/// <summary> Stores a global project object on the cache or caching server  </summary>
		/// <param name="Project_Code"> Code which identifies the project </param>
		/// <param name="StoreObject"> Project object to store for later retrieval </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Project( string Project_Code, SobekCM_Item StoreObject, Custom_Tracer Tracer)
		{
			Store_Project( -1, Project_Code, StoreObject, Tracer );
		}

		/// <summary> Stores a user-specific project object on the cache or caching server  </summary>
		/// <param name="UserID"> Primary key of the user, if this should be stored in the user-specific cache </param>
		/// <param name="Project_Code"> Code which identifies the project </param>
		/// <param name="StoreObject"> Project object to store for later retrieval </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Project(int UserID, string Project_Code, SobekCM_Item StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key = "PROJECT_" + Project_Code.ToLower();
			if (UserID > 0)
			{
				key = "USER" + UserID + "_" + key;
			}

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Store_Project", "Adding object '" + key + "' to the cache with expiration of 15 minutes");
			}

			// Store this on the cache
			if (HttpContext.Current.Cache[key] == null)
			{
				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15));
			}
		}


		#endregion

		#region Static methods relating to storing and retrieving HTML skin objects

		/// <summary> Removes all matching html skin objects from the cache or caching server </summary>
		/// <param name="Skin_Code"> Code identifying this skin </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Number of instances of this skin removed from the cache or caching server </returns>
		/// <remarks> This removes every instance of this skin, regardless of language </remarks>
		public static int Remove_Skin(string Skin_Code, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return 0;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Remove_Skin");
			}

			int values_cleared = 0;
			foreach (DictionaryEntry thisItem in HttpContext.Current.Cache)
			{
				if ((thisItem.Key.ToString() == "SKIN_" + Skin_Code.ToLower()) || (thisItem.Key.ToString().IndexOf("SKIN_" + Skin_Code.ToLower() + "_") == 0))
				{
					HttpContext.Current.Cache.Remove(thisItem.Key.ToString());
					values_cleared++;
				}
			}

			// Do the same thing for the remote cache
			foreach (Cached_Object_Info cachedObject in AppFabric_Manager.Cached_Items)
			{
				if ((cachedObject.Object_Key == "SKIN_" + Skin_Code.ToLower()) || (cachedObject.Object_Key.IndexOf("SKIN_" + Skin_Code.ToLower() + "_") == 0))
				{
					AppFabric_Manager.Expire_Item(cachedObject.Object_Key);
					values_cleared++;
				}
			}

			return values_cleared;
		}

		/// <summary> Retrieves the html skin object from the cache or caching server </summary>
		/// <param name="Skin_Code"> Code for this html display skin </param>
		/// <param name="Language_Code"> Current language code for the user interface ( skins are language-specific)</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the html skin object </returns>
		public static SobekCM_Skin_Object Retrieve_Skin( string Skin_Code, string Language_Code, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			// Determine the key
			string key = "SKIN_" + Skin_Code.ToLower();
			if ((Language_Code.Length > 0) && (Language_Code != "en"))
				key = key + "_" + Language_Code;

			// See if this is in the local cache first
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Skin", "Found html skin on local cache");
				}

				return (SobekCM_Skin_Object)returnValue;
			}

			// Try to get this from the caching server, if enabled
			if ((caching_serving_enabled) && (AppFabric_Manager.Contains(key)))
			{
				object from_app_fabric = AppFabric_Manager.Get(key, Tracer);
				if (from_app_fabric != null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Skin", "Found html skin on caching server");
					}

					// Check the number of item aggregations currently locally cached
					if (LOCALLY_CACHED_SKINS_LIMIT > 0)
					{
						int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => ThisItem.Key.ToString().IndexOf("SKIN_") == 0);

						// Locally cache if this doesn't exceed the limit
						if (items_cached < LOCALLY_CACHED_SKINS_LIMIT)
						{
							if (Tracer != null)
							{
								Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Skin", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
							}

							HttpContext.Current.Cache.Insert(key, from_app_fabric, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
						}
					}

					return (SobekCM_Skin_Object)from_app_fabric;
				}
			}

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Skin", "Skin not found in either the local cache or caching server");
			}

			// Since everything failed, just return null
			return null;
		}

		/// <summary> Stores the html skin object to the cache or caching server </summary>
		/// <param name="Skin_Code"> Code for this html display skin </param>
		/// <param name="Language_Code"> Current language code for the user interface ( skins are language-specific)</param>
		/// <param name="StoreObject"> HTML Skin object </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Skin( string Skin_Code, string Language_Code, SobekCM_Skin_Object StoreObject, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if ((Disabled) || ( StoreObject == null ))
				return;

			// Determine the key
			string key = "SKIN_" + Skin_Code.ToLower();
			if ((Language_Code.Length > 0) && (Language_Code != "en"))
				key = key + "_" + Language_Code;


			// Check the number of item aggregations currently locally cached
			if ((LOCALLY_CACHED_SKINS_LIMIT > 0) || ( !caching_serving_enabled ))
			{
				int items_cached = HttpContext.Current.Cache.Cast<DictionaryEntry>().Count(ThisItem => ThisItem.Key.ToString().IndexOf("SKIN_") == 0);

				// Locally cache if this doesn't exceed the limit
				if ((items_cached < LOCALLY_CACHED_SKINS_LIMIT) || ( !caching_serving_enabled ))
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("Cached_Data_Manager.Store_Skin", "Adding object '" + key + "' to the local cache with expiration of 1 minute");
					}

					HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
				}
			}

			// try to store in the caching server, if enabled
			if (caching_serving_enabled)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Skin", "Adding object '" + key + "' to the caching server");
				}

				AppFabric_Manager.Add(key, StoreObject, Tracer);
			}
		}

		#endregion

		#region Static methods relating to storing and retrieving site map objects

		/// <summary> Retrieves a site map navigational object for static web content pages </summary>
		/// <param name="SiteMap_File"> Name of the site map file which indicates the site map to retrieve from memory </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Either NULL or the site map from the local cache</returns>
		public static SobekCM_SiteMap Retrieve_Site_Map(string SiteMap_File, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Site_Map", "");
			}

			// Determine the key
			string key = "SITEMAP_" + SiteMap_File;

			// Try to get this object from the local cache 
			object returnValue = HttpContext.Current.Cache.Get(key);
			if (returnValue != null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Retrieve_Site_Map", "Site map pulled from local cache");
				}

				return (SobekCM_SiteMap)returnValue;
			}
			

			return null;
		}

		/// <summary> Stores a site map navigational object for static web content pages </summary>
		/// <param name="StoreObject"> Sitemap object to be locally cached </param>
		/// <param name="SiteMap_File"> Name of the site map file which indicates the site map to retrieve from memory </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public static void Store_Site_Map( SobekCM_SiteMap StoreObject, string SiteMap_File, Custom_Tracer Tracer)
		{
			// If the cache is disabled, just return before even tracing
			if (Disabled)
				return;

			// Determine the key
			string key = "SITEMAP_" + SiteMap_File;

			// Store this on the local cache, if not there and storing on the cache server failed
			if (HttpContext.Current.Cache[key] == null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Cached_Data_Manager.Store_Site_Map", "Adding object '" + key + "' to the local cache with expiration of 3 minutes");
				}

				HttpContext.Current.Cache.Insert(key, StoreObject, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(3));
			}
		}

		#endregion
	}
}
