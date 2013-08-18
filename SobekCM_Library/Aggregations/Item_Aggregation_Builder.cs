#region Using directives

using System;
using System.IO;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Settings;
using SobekCM.Library.WebContent;

#endregion

namespace SobekCM.Library.Aggregations
{
	/// <summary> Class is used to build the appropriate instance of the <see cref="Item_Aggregation"/> object.  This class
	/// pulls the data from the database, fills the object, and then performs final preparation for displaying the item 
	/// aggregation via the web.  </summary>
	public class Item_Aggregation_Builder
	{
		/// <summary> Gets a fully built item aggregation object for a particular aggregation code and language code.  </summary>
		/// <param name="AggregationCode">Code for this aggregation object</param>
		/// <param name="Language_Code">Code for the language for this aggregation object</param>
		/// <param name="cacheInstance">Instance of this item aggregation pulled from cache (or NULL)</param>
		/// <param name="isRobot">Flag tells if this request is from a robot (which will vary cacheing time)</param>
		/// <param name="tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns>Fully built item aggregation object for the particular aggregation code and language code</returns>
		/// <remarks>Item aggregation object is also placed in the cache.<br /><br />
		/// Building of an item aggregation always starts by pulling the item from the database ( either <see cref="SobekCM_Database.Get_Item_Aggregation"/> or <see cref="SobekCM_Database.Get_Main_Aggregation"/> ).<br /><br />
		/// Then, either the Item Aggregation XML file is read (if present) or the entire folder hierarchy is analyzed to find the browses, infos, banners, etc..</remarks>
		public static Item_Aggregation Get_Item_Aggregation(string AggregationCode, string Language_Code, Item_Aggregation cacheInstance, bool isRobot, Custom_Tracer tracer)
		{
			// Does this exist in the cache?
			if (cacheInstance == null)
			{
				if (tracer != null)
				{
					tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Creating '" + AggregationCode + "' item aggregation");
				}

				// Get the information about this collection and this entry point
				Item_Aggregation hierarchyObject;
				if ((AggregationCode.Length > 0) && (AggregationCode != "all"))
					hierarchyObject = SobekCM_Database.Get_Item_Aggregation(AggregationCode, false, isRobot, tracer);
				else
					hierarchyObject = SobekCM_Database.Get_Main_Aggregation(tracer);

				// If no value was returned, don't do anything else here
				if (hierarchyObject != null)
				{
					// Add all the values to this object
					string xmlDataFile = SobekCM_Library_Settings.Base_Design_Location + hierarchyObject.objDirectory + "\\" + hierarchyObject.Code + ".xml";
					if (File.Exists(xmlDataFile))
					{
						if (tracer != null)
						{
							tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Reading XML Configuration File");
						}

						// Add the ALL and NEW browses
						Add_All_New_Browses(hierarchyObject);

						// Add all the other data from the XML file
						Item_Aggregation_XML_Reader reader = new Item_Aggregation_XML_Reader();
						reader.Add_Info_From_XML_File(hierarchyObject, xmlDataFile);
					}
					else
					{
						if (tracer != null)
						{
							tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Adding banner, home, and all/new browse information");
						}

						Add_HTML(hierarchyObject);
						Add_All_New_Browses(hierarchyObject);
						if (!isRobot)
						{
						    if (tracer != null)
						    {
						        tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Scanning Design Directory for browse and info files");
						    }
                            Add_Browse_Files(hierarchyObject, tracer);
						}

                        // Since there was no configuration file, save one
                        hierarchyObject.Write_Configuration_File(SobekCM_Library_Settings.Base_Design_Location + hierarchyObject.objDirectory);

					}

					// Now, save this to the cache
					if (!isRobot)
					{
						Cached_Data_Manager.Store_Item_Aggregation(AggregationCode, Language_Code, hierarchyObject, tracer);
					}
					else
					{
						if (tracer != null)
						{
							tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Skipping storing item aggregation on cache due to robot flag");
						}
					}

					// Return this built hierarchy object
					return hierarchyObject;
				}
			    
                if (tracer != null)
			    {
			        tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "NULL value returned from database");
			    }
			    return null;
			}
		    
            if (tracer != null)
		    {
		        tracer.Add_Trace("Item_Aggregation_Builder.Get_Item_Aggregation", "Found '" + AggregationCode + "' item aggregation in cache");
		    }

		    // Get the HTML element and search fields and return all this
		    return cacheInstance;
		}


		/// <summary> Adds the ALL ITEMS and NEW ITEMS browses to the item aggregation, if the display options and last added
		/// item date call for it </summary>
		/// <param name="thisObject"> Item aggregation to which to add the ALL ITEMS and NEW ITEMS browse</param>
		/// <remarks>This method is always called while building an item aggregation, irregardless of whether there is an
		/// item aggregation configuration XML file or not.</remarks>
		protected static void Add_All_New_Browses(Item_Aggregation thisObject)
		{
			// If this is the main home page for this site, do not show ALL since we cannot browse ALL items
			if (!thisObject.Can_Browse_Items )
				return;

			// If this is in the display options, and the item browses
			if ((thisObject.Display_Options.Length == 0) || (thisObject.Display_Options.IndexOf("I") >= 0))
			{
				// Add the ALL browse, if there should be one
				thisObject.Add_Browse_Info(Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_Home, "all", String.Empty, "All Items");

				// Add the NEW search, if the ALL search exists
				if ((thisObject.Get_Browse_Info_Object("all") != null) && (thisObject.Show_New_Item_Browse))
				{
					thisObject.Add_Browse_Info(Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_Home, "new", String.Empty, "Recently Added Items");
				}
			}
			else
			{
				// Add the ALL browse as an info
				thisObject.Add_Browse_Info(Item_Aggregation_Browse_Info.Browse_Info_Type.Info, "all", String.Empty, "All Items");
			}
		}

		/// <summary> Checks the appropriate design folders to add any existing browse or info pages to the item aggregation </summary>
		/// <param name="thisObject"> Item aggregation object to add the browse and info pages to</param>
		/// <param name="tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks>This method is only called if the item aggregation does not have an existing XML configuration file.</remarks>
		protected static void Add_Browse_Files( Item_Aggregation thisObject, Custom_Tracer tracer  )
		{
			// Collect the list of items in the browse folder
			if (Directory.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/browse"))
			{
				string[] files = Directory.GetFiles(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/browse", "*.htm*");
				foreach (string thisFile in files)
				{
					// Get the new browse info object
					Item_Aggregation_Browse_Info newBrowse = Get_Item_Aggregation_Browse_Info(thisFile, Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_Home, tracer);
					if (newBrowse != null)
					{
						thisObject.Add_Browse_Info(newBrowse);
					}
				}
			}

			// Collect the list of items in the info folder
			if (Directory.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/info"))
			{
				string[] files = Directory.GetFiles(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/info", "*.htm*");
				foreach (string thisFile in files)
				{
					// Get the title for this file
					// Get the new browse info object
					Item_Aggregation_Browse_Info newInfo = Get_Item_Aggregation_Browse_Info(thisFile, Item_Aggregation_Browse_Info.Browse_Info_Type.Info, tracer);
					if (newInfo != null)
					{
						thisObject.Add_Browse_Info(newInfo);
					}
				}
			}
		}

		/// <summary>Reads the item aggregation browse or info file and returns a built <see cref="Item_Aggregation_Browse_Info"/> object for
		/// inclusion in the item aggregation </summary>
		/// <param name="fileName"> Filename of the browse or info file</param>
		/// <param name="Browse_Type"> Flag indicates if this is a browse or info file</param>
		/// <param name="tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Built object containing all of the pertinent details about this info or browse </returns>
		private static Item_Aggregation_Browse_Info Get_Item_Aggregation_Browse_Info( string fileName, Item_Aggregation_Browse_Info.Browse_Info_Type Browse_Type, Custom_Tracer tracer )
		{
			HTML_Based_Content fileContent = HTML_Based_Content_Reader.Read_HTML_File(fileName, false, tracer);
			Item_Aggregation_Browse_Info returnObject = new Item_Aggregation_Browse_Info(Browse_Type, Item_Aggregation_Browse_Info.Source_Type.Static_HTML, fileContent.Code, fileName, fileContent.Title);
			return returnObject;
		}

		/// <summary> Finds the home page source file and banner images or html for this item aggregation </summary>
		/// <param name="thisObject"> Item aggregation to add the home page link and banner html </param>
		/// <remarks>This method is only called if the item aggregation does not have an existing XML configuration file. </remarks>
		protected static void Add_HTML( Item_Aggregation thisObject )
		{
			// Just use the standard home text
            if ( File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/home/text.html"))
    			thisObject.Add_Home_Page_File(  "html/home/text.html", SobekCM_Library_Settings.Default_UI_Language );
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/home/text_en.html"))
                thisObject.Add_Home_Page_File("html/home/text_en.html",  Web_Language_Enum.English );
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/home/text_fr.html"))
                thisObject.Add_Home_Page_File("html/home/text_fr.html", Web_Language_Enum.French);
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/home/text_es.html"))
                thisObject.Add_Home_Page_File("html/home/text_es.html", Web_Language_Enum.Spanish);
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "html/home/text_sp.html"))
                thisObject.Add_Home_Page_File("html/home/text_sp.html", Web_Language_Enum.Spanish);

			// Just use the standard banner image
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "images/banners/coll.jpg"))
                thisObject.Add_Banner_Image("images/banners/coll.jpg", SobekCM_Library_Settings.Default_UI_Language);
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "images/banners/coll_en.jpg"))
                thisObject.Add_Banner_Image("images/banners/coll_en.jpg", Web_Language_Enum.English);
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "images/banners/coll_fr.jpg"))
                thisObject.Add_Banner_Image("images/banners/coll_fr.jpg", Web_Language_Enum.French);
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "images/banners/coll_es.jpg"))
                thisObject.Add_Banner_Image("images/banners/coll_es.jpg", Web_Language_Enum.Spanish);
            if (File.Exists(SobekCM_Library_Settings.Base_Design_Location + thisObject.objDirectory + "images/banners/coll_sp.jpg"))
                thisObject.Add_Banner_Image("images/banners/coll_sp.jpg", Web_Language_Enum.Spanish);
		}
	}
}
