#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Results;
using SobekCM.Core.WebContent;
using SobekCM.Engine.MemoryMgmt;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Aggregations
{
	/// <summary> Class is used to build the appropriate instance of the <see cref="Item_Aggregation"/> object.  This class
	/// pulls the data from the database, fills the object, and then performs final preparation for displaying the item 
	/// aggregation via the web.  </summary>
	public class Item_Aggregation_Utilities
	{
		/// <summary> Gets a fully built item aggregation object for a particular aggregation code and language code.  </summary>
		/// <param name="AggregationCode">Code for this aggregation object</param>
		/// <param name="Language_Code">Code for the language for this aggregation object</param>
		/// <param name="CacheInstance">Instance of this item aggregation pulled from cache (or NULL)</param>
		/// <param name="IsRobot">Flag tells if this request is from a robot (which will vary cacheing time)</param>
		/// <param name="StoreInCache"> Flag indicates if this should be stored in the cache once built </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns>Fully built item aggregation object for the particular aggregation code and language code</returns>
		/// <remarks>Item aggregation object is also placed in the cache.<br /><br />
		/// Building of an item aggregation always starts by pulling the item from the database ( either <see cref="SobekCM_Database.Get_Item_Aggregation"/> or <see cref="SobekCM_Database.Get_Main_Aggregation"/> ).<br /><br />
		/// Then, either the Item Aggregation XML file is read (if present) or the entire folder hierarchy is analyzed to find the browses, infos, banners, etc..</remarks>
		public static Item_Aggregation Get_Item_Aggregation(string AggregationCode, string Language_Code, Item_Aggregation CacheInstance, bool IsRobot, bool StoreInCache, Custom_Tracer Tracer)
		{
			// Does this exist in the cache?
			if (CacheInstance == null)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Creating '" + AggregationCode + "' item aggregation");
				}

				// Get the information about this collection and this entry point
				Item_Aggregation hierarchyObject;
				if ((AggregationCode.Length > 0) && (AggregationCode != "all"))
					hierarchyObject = Engine_Database.Get_Item_Aggregation(AggregationCode, false, IsRobot, Tracer);
				else
					hierarchyObject = Engine_Database.Get_Main_Aggregation(Tracer);

				// If no value was returned, don't do anything else here
				if (hierarchyObject != null)
				{
					// Add all the values to this object
					string xmlDataFile = Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + hierarchyObject.ObjDirectory + "\\" + hierarchyObject.Code + ".xml";
					if (File.Exists(xmlDataFile))
					{
						if (Tracer != null)
						{
							Tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Reading XML Configuration File");
						}

						// Add the ALL and NEW browses
						Add_All_New_Browses(hierarchyObject);

						// Add all the other data from the XML file
						Item_Aggregation_XML_Reader reader = new Item_Aggregation_XML_Reader();
						reader.Add_Info_From_XML_File(hierarchyObject, xmlDataFile);
					}
					else
					{
						if (Tracer != null)
						{
							Tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Adding banner, home, and all/new browse information");
						}

						Add_HTML(hierarchyObject);
						Add_All_New_Browses(hierarchyObject);
						if (!IsRobot)
						{
						    if (Tracer != null)
						    {
						        Tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Scanning Design Directory for browse and info files");
						    }
                            Add_Browse_Files(hierarchyObject, Tracer);
						}

                        // Since there was no configuration file, save one
                        hierarchyObject.Write_Configuration_File(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + hierarchyObject.ObjDirectory);
					}

                    // Now, save this to the cache
                    if ((!IsRobot) && (StoreInCache))
                    {
                        Cached_Data_Manager.Store_Item_Aggregation(AggregationCode, Language_Code, hierarchyObject, Tracer);
                    }
                    else
                    {
                        if (Tracer != null)
                        {
                            Tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Skipping storing item aggregation on cache due to robot flag");
                        }
                    }

					// Return this built hierarchy object
					return hierarchyObject;
				}
			    
                if (Tracer != null)
			    {
			        Tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "NULL value returned from database");
			    }
			    return null;
			}
		    
            if (Tracer != null)
		    {
		        Tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Found '" + AggregationCode + "' item aggregation in cache");
		    }

		    // Get the HTML element and search fields and return all this
		    return CacheInstance;
		}


		/// <summary> Adds the ALL ITEMS and NEW ITEMS browses to the item aggregation, if the display options and last added
		/// item date call for it </summary>
		/// <param name="ThisObject"> Item aggregation to which to add the ALL ITEMS and NEW ITEMS browse</param>
		/// <remarks>This method is always called while building an item aggregation, irregardless of whether there is an
		/// item aggregation configuration XML file or not.</remarks>
		protected static void Add_All_New_Browses(Item_Aggregation ThisObject)
		{
			// If this is the main home page for this site, do not show ALL since we cannot browse ALL items
			if (!ThisObject.Can_Browse_Items )
				return;

			// If this is in the display options, and the item browses
			if ((ThisObject.Display_Options.Length == 0) || (ThisObject.Display_Options.IndexOf("I") >= 0))
			{
				// Add the ALL browse, if there should be one
				ThisObject.Add_Child_Page(Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU, "all", String.Empty, "All Items");

				// Add the NEW search, if the ALL search exists
				if ((ThisObject.Get_Browse_Info_Object("all") != null) && (ThisObject.Show_New_Item_Browse))
				{
					ThisObject.Add_Child_Page(Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU, "new", String.Empty, "Recently Added Items");
				}
			}
			else
			{
				// Add the ALL browse as an info
				ThisObject.Add_Child_Page(Item_Aggregation_Child_Page.Visibility_Type.NONE, "all", String.Empty, "All Items");
			}
		}

		/// <summary> Checks the appropriate design folders to add any existing browse or info pages to the item aggregation </summary>
		/// <param name="ThisObject"> Item aggregation object to add the browse and info pages to</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks>This method is only called if the item aggregation does not have an existing XML configuration file.</remarks>
		protected static void Add_Browse_Files( Item_Aggregation ThisObject, Custom_Tracer Tracer  )
		{
			// Collect the list of items in the browse folder
			if (Directory.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/browse"))
			{
				string[] files = Directory.GetFiles(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/browse", "*.htm*");
				foreach (string thisFile in files)
				{
					// Get the new browse info object
					Item_Aggregation_Child_Page newBrowse = Get_Item_Aggregation_Browse_Info(thisFile, Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU, Tracer);
					if (newBrowse != null)
					{
						ThisObject.Add_Child_Page(newBrowse);
					}
				}
			}

			// Collect the list of items in the info folder
			if (Directory.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/info"))
			{
				string[] files = Directory.GetFiles(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/info", "*.htm*");
				foreach (string thisFile in files)
				{
					// Get the title for this file
					// Get the new browse info object
					Item_Aggregation_Child_Page newInfo = Get_Item_Aggregation_Browse_Info(thisFile, Item_Aggregation_Child_Page.Visibility_Type.NONE, Tracer);
					if (newInfo != null)
					{
						ThisObject.Add_Child_Page(newInfo);
					}
				}
			}
		}

		/// <summary>Reads the item aggregation browse or info file and returns a built <see cref="Item_Aggregation_Child_Page"/> object for
		/// inclusion in the item aggregation </summary>
		/// <param name="FileName"> Filename of the browse or info file</param>
		/// <param name="Browse_Type"> Flag indicates if this is a browse or info file</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Built object containing all of the pertinent details about this info or browse </returns>
		private static Item_Aggregation_Child_Page Get_Item_Aggregation_Browse_Info( string FileName, Item_Aggregation_Child_Page.Visibility_Type Browse_Type, Custom_Tracer Tracer )
		{
			HTML_Based_Content fileContent = HTML_Based_Content_Reader.Read_HTML_File(FileName, false, Tracer);
			Item_Aggregation_Child_Page returnObject = new Item_Aggregation_Child_Page(Browse_Type, Item_Aggregation_Child_Page.Source_Type.Static_HTML, fileContent.Code, FileName, fileContent.Title);
			return returnObject;
		}

		/// <summary> Finds the home page source file and banner images or html for this item aggregation </summary>
		/// <param name="ThisObject"> Item aggregation to add the home page link and banner html </param>
		/// <remarks>This method is only called if the item aggregation does not have an existing XML configuration file. </remarks>
		protected static void Add_HTML( Item_Aggregation ThisObject )
		{
			// Just use the standard home text
            if ( File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text.html"))
    			ThisObject.Add_Home_Page_File(  "html/home/text.html", Engine_ApplicationCache_Gateway.Settings.Default_UI_Language );
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text_en.html"))
                ThisObject.Add_Home_Page_File("html/home/text_en.html",  Web_Language_Enum.English );
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text_fr.html"))
                ThisObject.Add_Home_Page_File("html/home/text_fr.html", Web_Language_Enum.French);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text_es.html"))
                ThisObject.Add_Home_Page_File("html/home/text_es.html", Web_Language_Enum.Spanish);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text_sp.html"))
                ThisObject.Add_Home_Page_File("html/home/text_sp.html", Web_Language_Enum.Spanish);

			// Just use the standard banner image
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll.jpg", Engine_ApplicationCache_Gateway.Settings.Default_UI_Language);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll_en.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll_en.jpg", Web_Language_Enum.English);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll_fr.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll_fr.jpg", Web_Language_Enum.French);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll_es.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll_es.jpg", Web_Language_Enum.Spanish);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll_sp.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll_sp.jpg", Web_Language_Enum.Spanish);
		}


        /// <summary> Method returns the table of results for the browse indicated </summary>
        /// <param name = "ChildPageObject">Object with all the information about the browse</param>
        /// <param name = "Page"> Page of results requested for the indicated browse </param>
        /// <param name = "Sort"> Sort applied to the results before being returned </param>
        /// <param name="Potentially_Include_Facets"> Flag indicates if facets could be included in this browse results </param>
        /// <param name = "Need_Browse_Statistics"> Flag indicates if the browse statistics (facets and total counts) are required for this browse as well </param>
        /// <param name = "Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Results_Per_Page"> Number of results to retrieve per page</param>
        /// <returns> Resutls for the browse or info in table form </returns>
        public static Multiple_Paged_Results_Args Get_Browse_Results(Item_Aggregation ItemAggr, Item_Aggregation_Child_Page ChildPageObject,
                                                                      int Page, int Sort, int Results_Per_Page, bool Potentially_Include_Facets, bool Need_Browse_Statistics,
                                                                      Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation.Get_Browse_Table", String.Empty);
            }

            // Get the list of facets first
            List<short> facetsList = ItemAggr.Facets;
            if (!Potentially_Include_Facets)
                facetsList = null;

            // Pull data from the database if necessary
            if ((ChildPageObject.Code == "all") || (ChildPageObject.Code == "new"))
            {
                // Get this browse from the database
                if ((ItemAggr.Aggregation_ID < 0) || (ItemAggr.Code.ToUpper() == "ALL"))
                {
                    if (ChildPageObject.Code == "new")
                        return Engine_Database.Get_All_Browse_Paged(true, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
                    return Engine_Database.Get_All_Browse_Paged(false, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
                }

                if (ChildPageObject.Code == "new")
                {
                    return Engine_Database.Get_Item_Aggregation_Browse_Paged(ItemAggr.Code, true, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
                }
                return Engine_Database.Get_Item_Aggregation_Browse_Paged(ItemAggr.Code, false, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
            }

            // Default return NULL
            return null;
        }

        #region Method to save this item aggregation to the database

        /// <summary> Saves the information about this item aggregation to the database </summary>
        /// <param name="Username"> Name of the user performing this save, for the item aggregation milestones</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public static bool Save_To_Database(Item_Aggregation ItemAggr, string Username, Custom_Tracer Tracer)
        {
            return Engine_Database.Save_Item_Aggregation(ItemAggr.Aggregation_ID, ItemAggr.Code, ItemAggr.Name, ItemAggr.ShortName,
                ItemAggr.Description, ItemAggr.Thematic_Heading_ID, ItemAggr.Aggregation_Type, ItemAggr.Is_Active, ItemAggr.Hidden,
                ItemAggr.Display_Options, ItemAggr.Map_Search, ItemAggr.Map_Search_Beta, ItemAggr.Map_Display, ItemAggr.Map_Display_Beta,
                ItemAggr.OAI_Flag, ItemAggr.OAI_Metadata, ItemAggr.Contact_Email, String.Empty, ItemAggr.External_Link, -1, Username, Tracer);
        }

        #endregion
	}
}