#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Configuration;
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
	    /// <summary> Gets a fully built item aggregation object for a particular aggregation code   </summary>
	    /// <param name="AggregationCode">Code for this aggregation object</param>
	    /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
	    /// <returns>Fully built item aggregation object for the particular aggregation code and language code</returns>
	    /// <remarks>Item aggregation object is also placed in the cache.<br /><br />
	    /// Building of an item aggregation always starts by pulling the item from the database ( either <see cref="Engine_Database.Get_Item_Aggregation"/> or <see cref="Engine_Database.Get_Main_Aggregation"/> ).<br /><br />
	    /// Then, either the Item Aggregation XML file is read (if present) or the entire folder hierarchy is analyzed to find the browses, infos, banners, etc..</remarks>
	    public static Complete_Item_Aggregation Get_Complete_Item_Aggregation(string AggregationCode, Custom_Tracer Tracer)
	    {
	        if (Tracer != null)
	        {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Creating '" + AggregationCode + "' item aggregation");
	        }

	        // Get the information about this collection and this entry point
	        Complete_Item_Aggregation hierarchyObject;
	        if ((AggregationCode.Length > 0) && (AggregationCode != "all"))
	            hierarchyObject = Engine_Database.Get_Item_Aggregation(AggregationCode, false, Tracer);
	        else
	            hierarchyObject = Engine_Database.Get_Main_Aggregation(Tracer);

	        // If no value was returned, don't do anything else here
	        if (hierarchyObject != null)
	        {
	            // Add all the values to this object
	            string xmlDataFile = Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + hierarchyObject.ObjDirectory + "\\" + hierarchyObject.Code + ".xml";
	            if (File.Exists(xmlDataFile))
	            {
	                if (Tracer != null)
	                {
                        Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Reading aggregation XML configuration file");
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
                        Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Aggregation XML configuration file missing.. will try to build");

                        Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Adding banner, home, and all/new browse information");
	                }

	                Add_HTML(hierarchyObject);
	                Add_All_New_Browses(hierarchyObject);
	                Add_Browse_Files(hierarchyObject, Tracer);

                    // If no HTML found, just add one
	                if ((hierarchyObject.Home_Page_File_Dictionary == null) || (hierarchyObject.Home_Page_File_Dictionary.Count == 0))
	                {
	                    hierarchyObject.Add_Home_Page_File("html\\home\\text.html", Web_Language_Enum.DEFAULT, false);
	                }

                    // If no banner found, just add one
                    if ((hierarchyObject.Banner_Dictionary == null) || (hierarchyObject.Banner_Dictionary.Count == 0))
                    {
                        hierarchyObject.Add_Banner_Image("images/banners/coll.jpg", Web_Language_Enum.DEFAULT);
                    }

                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Write aggregation XML configuration from built object");
                    }

	                // Since there was no configuration file, save one
	                hierarchyObject.Write_Configuration_File(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + hierarchyObject.ObjDirectory);
	            }

	            // Now, look for any satellite configuration files
	            string contactFormFile = Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + hierarchyObject.ObjDirectory + "\\config\\sobekcm_contactform.config";
	            if (File.Exists(contactFormFile))
	            {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Found aggregation-specific contact form configuration file");
                    }

	                hierarchyObject.ContactForm = ContactForm_Configuration_Reader.Read_Config(contactFormFile);
	            }

	            // Return this built hierarchy object
	            return hierarchyObject;
	        }

	        if (Tracer != null)
	        {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "NULL value returned from database");
	        }
	        return null;

	    }


	    /// <summary> Adds the ALL ITEMS and NEW ITEMS browses to the item aggregation, if the display options and last added
		/// item date call for it </summary>
		/// <param name="ThisObject"> Item aggregation to which to add the ALL ITEMS and NEW ITEMS browse</param>
		/// <remarks>This method is always called while building an item aggregation, irregardless of whether there is an
		/// item aggregation configuration XML file or not.</remarks>
        protected static void Add_All_New_Browses(Complete_Item_Aggregation ThisObject)
		{
			// If this is the main home page for this site, do not show ALL since we cannot browse ALL items
			if (!ThisObject.Can_Browse_Items )
				return;

			// If this is in the display options, and the item browses
			if ((ThisObject.Display_Options.Length == 0) || (ThisObject.Display_Options.IndexOf("I") >= 0))
			{
				// Add the ALL browse, if there should be one
                ThisObject.Add_Child_Page(Item_Aggregation_Child_Visibility_Enum.Main_Menu, "all", String.Empty, "All Items");

				// Add the NEW search, if the ALL search exists
				if ((ThisObject.Get_Browse_Info_Object("all") != null) && (ThisObject.Show_New_Item_Browse))
				{
                    ThisObject.Add_Child_Page(Item_Aggregation_Child_Visibility_Enum.Main_Menu, "new", String.Empty, "Recently Added Items");
				}
			}
			else
			{
				// Add the ALL browse as an info
                ThisObject.Add_Child_Page(Item_Aggregation_Child_Visibility_Enum.None, "all", String.Empty, "All Items");
			}
		}

		/// <summary> Checks the appropriate design folders to add any existing browse or info pages to the item aggregation </summary>
		/// <param name="ThisObject"> Item aggregation object to add the browse and info pages to</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks>This method is only called if the item aggregation does not have an existing XML configuration file.</remarks>
        protected static void Add_Browse_Files(Complete_Item_Aggregation ThisObject, Custom_Tracer Tracer)
		{
			// Collect the list of items in the browse folder
			if (Directory.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/browse"))
			{
				string[] files = Directory.GetFiles(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/browse", "*.htm*");
				foreach (string thisFile in files)
				{
					// Get the new browse info object
                    Complete_Item_Aggregation_Child_Page newBrowse = Get_Item_Aggregation_Browse_Info(thisFile, Item_Aggregation_Child_Visibility_Enum.Main_Menu, Tracer);
					if (newBrowse != null)
					{
						ThisObject.Add_Child_Page(newBrowse);
					}
				}
			}

			// Collect the list of items in the info folder
			if (Directory.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/info"))
			{
				string[] files = Directory.GetFiles(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/info", "*.htm*");
				foreach (string thisFile in files)
				{
					// Get the title for this file
					// Get the new browse info object
                    Complete_Item_Aggregation_Child_Page newInfo = Get_Item_Aggregation_Browse_Info(thisFile, Item_Aggregation_Child_Visibility_Enum.None, Tracer);
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
        private static Complete_Item_Aggregation_Child_Page Get_Item_Aggregation_Browse_Info(string FileName, Item_Aggregation_Child_Visibility_Enum Browse_Type, Custom_Tracer Tracer)
		{
			HTML_Based_Content fileContent = HTML_Based_Content_Reader.Read_HTML_File(FileName, false, Tracer);
            Complete_Item_Aggregation_Child_Page returnObject = new Complete_Item_Aggregation_Child_Page(Browse_Type, Item_Aggregation_Child_Source_Data_Enum.Static_HTML, fileContent.Code, FileName, fileContent.Title ?? "Missing Title");
			return returnObject;
		}

		/// <summary> Finds the home page source file and banner images or html for this item aggregation </summary>
		/// <param name="ThisObject"> Item aggregation to add the home page link and banner html </param>
		/// <remarks>This method is only called if the item aggregation does not have an existing XML configuration file. </remarks>
        protected static void Add_HTML(Complete_Item_Aggregation ThisObject)
		{
			// Just use the standard home text
            if ( File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text.html"))
    			ThisObject.Add_Home_Page_File(  "html/home/text.html", Engine_ApplicationCache_Gateway.Settings.System.Default_UI_Language, false );
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text_en.html"))
                ThisObject.Add_Home_Page_File("html/home/text_en.html",  Web_Language_Enum.English, false );
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text_fr.html"))
                ThisObject.Add_Home_Page_File("html/home/text_fr.html", Web_Language_Enum.French, false);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text_es.html"))
                ThisObject.Add_Home_Page_File("html/home/text_es.html", Web_Language_Enum.Spanish, false);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "html/home/text_sp.html"))
                ThisObject.Add_Home_Page_File("html/home/text_sp.html", Web_Language_Enum.Spanish, false);

			// Just use the standard banner image
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll.jpg", Engine_ApplicationCache_Gateway.Settings.System.Default_UI_Language);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll_en.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll_en.jpg", Web_Language_Enum.English);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll_fr.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll_fr.jpg", Web_Language_Enum.French);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll_es.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll_es.jpg", Web_Language_Enum.Spanish);
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + ThisObject.ObjDirectory + "images/banners/coll_sp.jpg"))
                ThisObject.Add_Banner_Image("images/banners/coll_sp.jpg", Web_Language_Enum.Spanish);
		}


	    /// <summary> Method returns the table of results for the browse indicated </summary>
	    /// <param name="ItemAggr"> Item Aggregation from which to return the browse </param>
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
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Browse_Results", String.Empty);
            }

            // Get the list of facets first
            List<short> facetsList = ItemAggr.Facets;
            if (!Potentially_Include_Facets)
                facetsList = null;

            // Pull data from the database if necessary
            if ((ChildPageObject.Code == "all") || (ChildPageObject.Code == "new"))
            {
                // Get this browse from the database
                if ((ItemAggr.ID < 0) || (ItemAggr.Code.ToUpper() == "ALL"))
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

	    /// <summary> Method returns the table of results for the browse indicated </summary>
	    /// <param name = "ItemAggr">Object with all the information about the browse</param>
	    /// <param name = "Page"> Page of results requested for the indicated browse </param>
	    /// <param name = "Sort"> Sort applied to the results before being returned </param>
	    /// <param name="Potentially_Include_Facets"> Flag indicates if facets could be included in this browse results </param>
	    /// <param name = "Need_Browse_Statistics"> Flag indicates if the browse statistics (facets and total counts) are required for this browse as well </param>
	    /// <param name = "Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
	    /// <param name="Results_Per_Page"> Number of results to retrieve per page</param>
	    /// <returns> Resutls for the browse or info in table form </returns>
	    public static Multiple_Paged_Results_Args Gat_All_Browse(Complete_Item_Aggregation ItemAggr,
	        int Page, int Sort, int Results_Per_Page,
	        bool Potentially_Include_Facets, bool Need_Browse_Statistics,
	        Custom_Tracer Tracer)
	    {
	        if (Tracer != null)
	        {
	            Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Browse_Results", String.Empty);
	        }

	        // Get the list of facets first
	        List<short> facetsList = ItemAggr.Facets;
	        if (!Potentially_Include_Facets)
	            facetsList = null;

	        // Pull data from the database if necessary

	        // Get this browse from the database
	        if ((ItemAggr.ID < 0) || (ItemAggr.Code.ToUpper() == "ALL"))
	        {
	            return Engine_Database.Get_All_Browse_Paged(false, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
	        }

	        return Engine_Database.Get_Item_Aggregation_Browse_Paged(ItemAggr.Code, false, false, Results_Per_Page, Page, Sort, Need_Browse_Statistics, facetsList, Need_Browse_Statistics, Tracer);
	    }


	    #region Method to save the complete item aggregation to the database

	    /// <summary> Saves the information about this item aggregation to the database </summary>
	    /// <param name="ItemAggr"> Item aggregation object with all the information to be saved </param>
	    /// <param name="Username"> Name of the user performing this save, for the item aggregation milestones</param>
	    /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
	    /// <returns>TRUE if successful, otherwise FALSE </returns>
	    public static bool Save_To_Database(Complete_Item_Aggregation ItemAggr, string Username, Custom_Tracer Tracer)
        {
            // Build the list of language variants
            List<string> languageVariants = new List<string>
            {
                Web_Language_Enum_Converter.Enum_To_Code(Engine_ApplicationCache_Gateway.Settings.System.Default_UI_Language)
            };
	        if (ItemAggr.Home_Page_File_Dictionary != null)
            {
                foreach (Web_Language_Enum language in ItemAggr.Home_Page_File_Dictionary.Keys)
                {
                    string code = Web_Language_Enum_Converter.Enum_To_Code(language);
                    if (!languageVariants.Contains(code))
                        languageVariants.Add(code);
                }
            }
            if (ItemAggr.Banner_Dictionary != null)
            {
                foreach (Web_Language_Enum language in ItemAggr.Banner_Dictionary.Keys)
                {
                    string code = Web_Language_Enum_Converter.Enum_To_Code(language);
                    if (!languageVariants.Contains(code))
                        languageVariants.Add(code);
                } 
            }
            if (ItemAggr.Child_Pages != null)
            {
                foreach (Complete_Item_Aggregation_Child_Page childPage in ItemAggr.Child_Pages)
                {
                    if (childPage.Label_Dictionary != null)
                    {
                        foreach (Web_Language_Enum language in childPage.Label_Dictionary.Keys)
                        {
                            string code2 = Web_Language_Enum_Converter.Enum_To_Code(language);
                            if (!languageVariants.Contains(code2))
                                languageVariants.Add(code2);
                        }
                    }
                    if (childPage.Source_Dictionary != null)
                    {
                        foreach (Web_Language_Enum language in childPage.Source_Dictionary.Keys)
                        {
                            string code2 = Web_Language_Enum_Converter.Enum_To_Code(language);
                            if (!languageVariants.Contains(code2))
                                languageVariants.Add(code2);
                        }
                    }
                }
            }
            StringBuilder languageVariantsBuilder = new StringBuilder();
            foreach (string language in languageVariants)
            {
                if (language.Length > 0)
                {
                    if (languageVariantsBuilder.Length > 0)
                        languageVariantsBuilder.Append("|" + language);
                    else
                        languageVariantsBuilder.Append(language);
                }
            }


            return Engine_Database.Save_Item_Aggregation(ItemAggr.ID, ItemAggr.Code, ItemAggr.Name, ItemAggr.ShortName,
                ItemAggr.Description, ItemAggr.Thematic_Heading, ItemAggr.Type, ItemAggr.Active, ItemAggr.Hidden,
                ItemAggr.Display_Options, 0, ItemAggr.Map_Search_Beta, 0, ItemAggr.Map_Display_Beta,
                ItemAggr.OAI_Enabled, ItemAggr.OAI_Metadata, ItemAggr.Contact_Email, String.Empty, ItemAggr.External_Link, -1, Username,
                languageVariantsBuilder.ToString(), Tracer);
        }

        #endregion

        #region Methods to get the language-specific item aggregation

	    /// <summary> Get the language specific item aggregation, from the complete item aggregation object </summary>
	    /// <param name="CompAggr"> Copmlete item aggregation object </param>
	    /// <param name="RequestedLanguage"> Language version requested </param>
	    /// <param name="Tracer"></param>
	    /// <returns> The language-specific aggregation, built from the complete aggregation object, or NULL if an error occurred </returns>
	    public static Item_Aggregation Get_Item_Aggregation(Complete_Item_Aggregation CompAggr, Web_Language_Enum RequestedLanguage, Custom_Tracer Tracer)
	    {
            // If the complete aggregation was null, return null
	        if (CompAggr == null)
	        {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Complete item aggregation was NULL.. aborting and returning NULL");
                }

	            return null;
	        }

            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Building language-specific item aggregation from the complete object");
            }

	        // Build the item aggregation
            Item_Aggregation returnValue = new Item_Aggregation(RequestedLanguage, CompAggr.ID, CompAggr.Code)
            {
                Active = CompAggr.Active,
                BannerImage = CompAggr.Banner_Image(RequestedLanguage, null ),
                Child_Types = CompAggr.Child_Types,
                Contact_Email = CompAggr.Contact_Email,
                ContactForm = CompAggr.ContactForm,
                CSS_File = CompAggr.CSS_File,
                Default_BrowseBy = CompAggr.Default_BrowseBy,
                Default_Result_View = CompAggr.Default_Result_View,
                Default_Skin = CompAggr.Default_Skin,
                Description = CompAggr.Description,
                Display_Options = CompAggr.Display_Options,
                FrontBannerObj = CompAggr.Front_Banner_Image(RequestedLanguage),
                Hidden = CompAggr.Hidden,
                Last_Item_Added = CompAggr.Last_Item_Added,
                Name = CompAggr.Name,
                Rotating_Highlights = CompAggr.Rotating_Highlights,
                ShortName = CompAggr.ShortName,
                Statistics = CompAggr.Statistics,
                Type = CompAggr.Type
            };

            // Copy the map search and browse information
	        if (CompAggr.Map_Search_Display != null) returnValue.Map_Search_Display = CompAggr.Map_Search_Display.Copy();
            if (CompAggr.Map_Browse_Display != null) returnValue.Map_Browse_Display = CompAggr.Map_Browse_Display.Copy();

            // Copy any children aggregations over
            if (CompAggr.Active_Children_Count > 0)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying children objects");
                }

                returnValue.Children = new List<Item_Aggregation_Related_Aggregations>();
                foreach (Item_Aggregation_Related_Aggregations thisAggr in CompAggr.Children)
                {
                    returnValue.Children.Add(thisAggr);
                }
            }

            // Copy any parent aggregations over
            if (CompAggr.Parent_Count > 0)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying parent objects");
                }

                returnValue.Parents = new List<Item_Aggregation_Related_Aggregations>();
                foreach (Item_Aggregation_Related_Aggregations thisAggr in CompAggr.Parents)
                {
                    returnValue.Parents.Add(thisAggr);
                }
            }

            // Copy all the facet information over
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying facets");
            }
            foreach (short thisFacet in CompAggr.Facets)
            {
                returnValue.Facets.Add(thisFacet);
            }

            // Copy over all the results views
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying result views");
            }
            foreach (Result_Display_Type_Enum display in CompAggr.Result_Views)
            {
                returnValue.Result_Views.Add(display);
            }

            // Copy all the views and searches over
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying views and searches");
            }
            if (CompAggr.Views_And_Searches != null)
            {
                foreach (Item_Aggregation_Views_Searches_Enum viewsSearches in CompAggr.Views_And_Searches)
                {
                    returnValue.Views_And_Searches.Add(viewsSearches);
                }
            }

            // Copy over any web skin limitations
            if ((CompAggr.Web_Skins != null) && (CompAggr.Web_Skins.Count > 0))
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying web skins");
                }

                returnValue.Web_Skins = new List<string>();
                foreach (string thisSkin in CompAggr.Web_Skins)
                {
                    returnValue.Web_Skins.Add(thisSkin);
                }
            }

            // Language-specific (and simplified) metadata type info
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying search anbd browseable fields");
            }
            foreach (Complete_Item_Aggregation_Metadata_Type thisAdvSearchField in CompAggr.Search_Fields)
            {
                returnValue.Search_Fields.Add(new Item_Aggregation_Metadata_Type(thisAdvSearchField.DisplayTerm, thisAdvSearchField.SobekCode));
            }
            foreach (Complete_Item_Aggregation_Metadata_Type thisAdvSearchField in CompAggr.Browseable_Fields)
            {
                returnValue.Browseable_Fields.Add(new Item_Aggregation_Metadata_Type(thisAdvSearchField.DisplayTerm, thisAdvSearchField.SobekCode));
            }

            // Language-specific (and simplified) child pages information
            if ((CompAggr.Child_Pages != null) && (CompAggr.Child_Pages.Count > 0))
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying child pages");
                }

                returnValue.Child_Pages = new List<Item_Aggregation_Child_Page>();
                foreach (Complete_Item_Aggregation_Child_Page fullPage in CompAggr.Child_Pages)
                {
                    Item_Aggregation_Child_Page newPage = new Item_Aggregation_Child_Page
                    {
                        Browse_Type = fullPage.Browse_Type, 
                        Code = fullPage.Code, 
                        Parent_Code = fullPage.Parent_Code, 
                        Source_Data_Type = fullPage.Source_Data_Type
                    };

                    string label = fullPage.Get_Label(RequestedLanguage);
                    if (!String.IsNullOrEmpty(label))
                        newPage.Label = label;

                    string source = fullPage.Get_Static_HTML_Source(RequestedLanguage);
                    if (!String.IsNullOrEmpty(label))
                        newPage.Source = source;

                    returnValue.Child_Pages.Add(newPage);
                }
            }

            // Language-specific (and simplified) highlight information
            if ((CompAggr.Highlights != null) && (CompAggr.Highlights.Count > 0))
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "...Copying relevant highlights");
                }

                returnValue.Highlights = new List<Item_Aggregation_Highlights>();
                int day_integer = DateTime.Now.DayOfYear + 1;
                int highlight_to_use = day_integer % CompAggr.Highlights.Count;

                // If this is for rotating highlights, show up to eight
                if ((CompAggr.Rotating_Highlights.HasValue ) && ( CompAggr.Rotating_Highlights.Value ))
                {
                    // Copy over just the eight highlights we should use 
                    int number = Math.Min(8, CompAggr.Highlights.Count);
                    for (int i = 0; i < number; i++)
                    {
                        Complete_Item_Aggregation_Highlights thisHighlight = CompAggr.Highlights[highlight_to_use];

                        Item_Aggregation_Highlights newHighlight = new Item_Aggregation_Highlights
                        {
                            Image = thisHighlight.Image, 
                            Link = thisHighlight.Link
                        };

                        string text = thisHighlight.Get_Text(RequestedLanguage);
                        if (!String.IsNullOrEmpty(text))
                            newHighlight.Text = text;

                        string tooltip = thisHighlight.Get_Tooltip(RequestedLanguage);
                        if (!String.IsNullOrEmpty(tooltip))
                            newHighlight.Tooltip = tooltip;

                        returnValue.Highlights.Add(newHighlight);

                        highlight_to_use++;
                        if (highlight_to_use >= CompAggr.Highlights.Count)
                            highlight_to_use = 0;
                    }
                }
                else
                {
                    Complete_Item_Aggregation_Highlights thisHighlight = CompAggr.Highlights[highlight_to_use];

                    Item_Aggregation_Highlights newHighlight = new Item_Aggregation_Highlights
                    {
                        Image = thisHighlight.Image,
                        Link = thisHighlight.Link
                    };

                    string text = thisHighlight.Get_Text(RequestedLanguage);
                    if (!String.IsNullOrEmpty(text))
                        newHighlight.Text = text;

                    string tooltip = thisHighlight.Get_Tooltip(RequestedLanguage);
                    if (!String.IsNullOrEmpty(tooltip))
                        newHighlight.Tooltip = tooltip;

                    returnValue.Highlights.Add(newHighlight);
                }
            }

            // Language-specific source page
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Getting the home page source");
            }
            returnValue.HomePageSource = String.Empty;
            HTML_Based_Content homeHtml = Get_Home_HTML(CompAggr, RequestedLanguage, null);
            returnValue.HomePageHtml = homeHtml;
	        returnValue.Custom_Home_Page = (CompAggr.Home_Page_File(RequestedLanguage) != null) && (CompAggr.Home_Page_File(RequestedLanguage).isCustomHome);

            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Item_Aggregation", "Returning fully built item aggregation object");
            }
            return returnValue;
	    }

	    /// <summary> Method gets the HOME PAGE html for the appropriate UI settings </summary>
	    /// <param name="CompAggr"> Complete item aggregation object </param>
	    /// <param name = "Language"> Current language of the user interface </param>
	    /// <param name = "Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
	    /// <returns>Home page HTML</returns>
	    private static HTML_Based_Content Get_Home_HTML(Complete_Item_Aggregation CompAggr, Web_Language_Enum Language, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Home_HTML", "Reading home text source file");
            }

            string homeFileSource = "";
            // Get the home file source
            if(CompAggr.Home_Page_File(Language) != null)
               homeFileSource = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location, CompAggr.ObjDirectory, CompAggr.Home_Page_File(Language).Source);

            // If no home file source even found, return a message to that affect
            if (homeFileSource.Length == 0)
            {
                return new HTML_Based_Content("<div class=\"error_div\">NO HOME PAGE SOURCE FILE FOUND</div>", null, homeFileSource);
            }

            // Do the rest in a try/catch
            try
            {
                // Does the file exist?
                if (!File.Exists(homeFileSource))
                {
                    return new HTML_Based_Content("<div class=\"error_div\">HOME PAGE SOURCE FILE '" + homeFileSource + "' DOES NOT EXIST.</div>", null, homeFileSource);
                }

                HTML_Based_Content content = HTML_Based_Content_Reader.Read_HTML_File(homeFileSource, true, Tracer);
                content.Source = homeFileSource;

                return content;
            }
            catch (Exception ee)
            {
                return new HTML_Based_Content("<div class=\"error_div\">EXCEPTION CAUGHT WHILE TRYING TO READ THE HOME PAGE SOURCE FILE '" + homeFileSource + "'.<br /><br />ERROR: " + ee.Message + "</div>", null, homeFileSource);
            }
        }

        #endregion

        #region Methods related to the item aggrgegation hierarchy

        /// <summary> Adds the entire collection hierarchy under the ALL aggregation object </summary>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This is postponed until it is needed for the TREE VIEW on the home page, to allow the system to start
        /// faster, even with a great number of item aggregationPermissions in the hierarchy </remarks>
        public static Aggregation_Hierarchy Get_Collection_Hierarchy(Custom_Tracer Tracer)
        {
            if (Tracer != null)
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Collection_Hierarchy", "Preparing to create the aggregation hierarchy object");

            // Get the database table
            DataSet childInfo = Engine_Database.Get_Aggregation_Hierarchies(Tracer);
            if (childInfo == null)
            {
                if (Tracer != null)
                    Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Collection_Hierarchy", "NULL value returned from database lookup");

                return null;
            }

            // Build the return value
            Aggregation_Hierarchy returnValue = new Aggregation_Hierarchy();

            // Add all the collections
            if (Tracer != null)
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Collection_Hierarchy", "Add all the child aggregations to the hierarchy");
            add_hierarchy_children(returnValue.Collections, childInfo.Tables[0]);

            // Add all the institutions
            if (Tracer != null)
                Tracer.Add_Trace("Item_Aggregation_Utilities.Get_Collection_Hierarchy", "Add all the institutions to the hierarchy");
            add_hierarchy_children(returnValue.Institutions, childInfo.Tables[1]);

            return returnValue;
        }

        /// <summary> Adds the child information to the item aggregation object from the datatable extracted from the database </summary>
        /// <param name="AggrList"> List into which to populate the hierarchy </param>
        /// <param name="ChildInfo"> Datatable from database calls with child item aggregation information </param>
        private static void add_hierarchy_children(List<Item_Aggregation_Related_Aggregations> AggrList, DataTable ChildInfo)
        {
            if (ChildInfo.Rows.Count == 0)
                return;

            // Build a dictionary of nodes while building this tree
            Dictionary<string, Item_Aggregation_Related_Aggregations> nodes = new Dictionary<string, Item_Aggregation_Related_Aggregations>(ChildInfo.Rows.Count);

            // Step through each row of children
            foreach (DataRow thisRow in ChildInfo.Rows)
            {
                // pull some of the basic data out
                int hierarchyLevel = Convert.ToInt16(thisRow[5]);
                string code = thisRow[0].ToString().ToLower();
                string parentCode = thisRow[1].ToString().ToLower();

                // If this does not already exist, create it
                if (!nodes.ContainsKey(code))
                {
                    // Create the object
                    Item_Aggregation_Related_Aggregations childObject = new Item_Aggregation_Related_Aggregations(code, thisRow[2].ToString(), thisRow[4].ToString(), Convert.ToBoolean(thisRow[6]), Convert.ToBoolean(thisRow[7]));

                    // Add this object to the node dictionary
                    nodes.Add(code, childObject);

                    // Check for parent in the node list
                    if ((parentCode.Length > 0) && (nodes.ContainsKey(parentCode)))
                    {
                        nodes[parentCode].Add_Child_Aggregation(childObject);
                    }

                    // If this is the first hierarchy, add to the main item aggregation object
                    if (hierarchyLevel == -1)
                    {
                        AggrList.Add(childObject);
                    }
                }
            }
        }


        #endregion
    }
}
