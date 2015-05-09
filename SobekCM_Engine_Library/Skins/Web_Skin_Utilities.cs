#region Using directives

using System;
using System.Data;
using System.IO;
using System.Linq;
using SobekCM.Core.Configuration;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Skins;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Skins
{
	/// <summary> Builder creates individiual <see cref="SobekCM_Skin_Object"/> objects when application first starts and 
    /// when a new skin is needed for a user request </summary>
	public class Web_Skin_Utilities
	{
        /// <summary> Populates/builds the main default HTML skin during application startup </summary>
        /// <param name="SkinList"> List of skin to populate with the default, commonly used skin</param>
        /// <param name="tracer"> Trace object keeps a list of each method executed and important milestones in rendering  </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> Most HTML skins are built as they are needed and then cached for a period of time.  The main default skins are
        /// permanently stored in this global <see cref="SobekCM_Skin_Collection"/> object.</remarks>
        public static bool Populate_Default_Skins(Web_Skin_Collection SkinList, Custom_Tracer tracer)
        {
            if (tracer != null)
            {
                tracer.Add_Trace("SobekCM_Skin_Collection_Builder.Populate_Default_Skins", "Build the standard interfaces");
            }

            // Get the data from the database
            DataTable skinData = Engine_Database.Get_All_Web_Skins(tracer);

            // Just return if the data appears bad..
            if ((skinData == null) || (skinData.Rows.Count == 0))
                return false;

            // Clear existing interfaces
            SkinList.Initialize(skinData);

            return true;
        }

        /// <summary> Builds the complete web skin object </summary>
        /// <param name="Skin_Row"> Row for this web skin, from the database query </param>
        /// <returns> Complete web skin </returns>
	    public static Complete_Web_Skin_Object Build_Skin_Complete(DataRow Skin_Row)
	    {
            // Pull values out from this row
            string code = Skin_Row["WebSkinCode"].ToString();
            string base_interface = Skin_Row["BaseInterface"].ToString();
            bool override_banner = Convert.ToBoolean(Skin_Row["OverrideBanner"]);
            string banner_link = Skin_Row["BannerLink"].ToString();
            string notes = Skin_Row["Notes"].ToString();
            string this_style = code + ".css";
            if (!File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + "skins\\" + code + "\\" + this_style))
                this_style = String.Empty;

            // Create the web skin object
            Complete_Web_Skin_Object completeSkin = new Complete_Web_Skin_Object(code, this_style)
            {
                Override_Banner = override_banner, 
                Suppress_Top_Navigation = Convert.ToBoolean(Skin_Row["SuppressTopNavigation"]),
                Notes = notes
            };

            // Assign the optional values
            if (!String.IsNullOrEmpty(base_interface))
                completeSkin.Base_Skin_Code = base_interface;
            if (!String.IsNullOrEmpty(banner_link))
                completeSkin.Banner_Link = banner_link;

            // Look for source files
	        string html_soure_directory = Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + "skins/" + code + "/html";
            if (Directory.Exists(html_soure_directory))
            {
                string[] possible_header_files = Directory.GetFiles(html_soure_directory, "*.htm*");
                foreach (string thisHeaderFile in possible_header_files)
                {
                    // Get the filename
                    string fileName = Path.GetFileName(thisHeaderFile);

                    // Was this an item header file?
                    if (fileName.IndexOf("header_item", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        // If this is default (with no language specified) add as DEFAULT
                        if (fileName.ToLower().Contains("header_item.htm"))
                        {
                            if (completeSkin.SourceFiles.ContainsKey(Web_Language_Enum.DEFAULT))
                                completeSkin.SourceFiles[Web_Language_Enum.DEFAULT].Header_Item_Source_File = "html\\" + fileName;
                            else
                            {
                                Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Header_Item_Source_File = "html\\" + fileName};
                                completeSkin.SourceFiles[Web_Language_Enum.DEFAULT] = sourceFiles;
                            }
                        }
                        else
                        {
                            // Look for and parse the language code in the file
                            string[] parsed = fileName.Split("_-.".ToCharArray());
                            if (parsed.Length == 4)
                            {
                                Web_Language_Enum languageEnum = Web_Language_Enum_Converter.Code_To_Enum(parsed[2]);
                                if (languageEnum != Web_Language_Enum.UNDEFINED)
                                {
                                    if (completeSkin.SourceFiles.ContainsKey(languageEnum))
                                        completeSkin.SourceFiles[languageEnum].Header_Item_Source_File = "html\\" + fileName;
                                    else
                                    {
                                        Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Header_Item_Source_File = "html\\" + fileName};
                                        completeSkin.SourceFiles[languageEnum] = sourceFiles;
                                    }
                                }
                            }
                        }
                    }
                        // Was this a non-item header file?
                    else if (fileName.IndexOf("header", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        // If this is default (with no language specified) add as DEFAULT
                        if (fileName.ToLower().Contains("header.htm"))
                        {
                            if (completeSkin.SourceFiles.ContainsKey(Web_Language_Enum.DEFAULT))
                                completeSkin.SourceFiles[Web_Language_Enum.DEFAULT].Header_Source_File = "html\\" + fileName;
                            else
                            {
                                Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Header_Source_File = "html\\" + fileName};
                                completeSkin.SourceFiles[Web_Language_Enum.DEFAULT] = sourceFiles;
                            }
                        }
                        else
                        {
                            // Look for and parse the language code in the file
                            string[] parsed = fileName.Split("_-.".ToCharArray());
                            if (parsed.Length == 3)
                            {
                                Web_Language_Enum languageEnum = Web_Language_Enum_Converter.Code_To_Enum(parsed[1]);
                                if (languageEnum != Web_Language_Enum.UNDEFINED)
                                {
                                    if (completeSkin.SourceFiles.ContainsKey(languageEnum))
                                        completeSkin.SourceFiles[languageEnum].Header_Source_File = "html\\" + fileName;
                                    else
                                    {
                                        Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Header_Source_File = "html\\" + fileName};
                                        completeSkin.SourceFiles[languageEnum] = sourceFiles;
                                    }
                                }
                            }
                        }
                    }
                        // Was this a item footer file?
                    else if (fileName.IndexOf("footer_item", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        // If this is default (with no language specified) add as DEFAULT
                        if (fileName.ToLower().Contains("footer_item.htm"))
                        {
                            if (completeSkin.SourceFiles.ContainsKey(Web_Language_Enum.DEFAULT))
                                completeSkin.SourceFiles[Web_Language_Enum.DEFAULT].Footer_Item_Source_File = "html\\" + fileName;
                            else
                            {
                                Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Footer_Item_Source_File = "html\\" + fileName};
                                completeSkin.SourceFiles[Web_Language_Enum.DEFAULT] = sourceFiles;
                            }
                        }
                        else
                        {
                            // Look for and parse the language code in the file
                            string[] parsed = fileName.Split("_-.".ToCharArray());
                            if (parsed.Length == 4)
                            {
                                Web_Language_Enum languageEnum = Web_Language_Enum_Converter.Code_To_Enum(parsed[2]);
                                if (languageEnum != Web_Language_Enum.UNDEFINED)
                                {
                                    if (completeSkin.SourceFiles.ContainsKey(languageEnum))
                                        completeSkin.SourceFiles[languageEnum].Footer_Item_Source_File = "html\\" + fileName;
                                    else
                                    {
                                        Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Footer_Item_Source_File = "html\\" + fileName};
                                        completeSkin.SourceFiles[languageEnum] = sourceFiles;
                                    }
                                }
                            }
                        }
                    }
                        // Was this a non-item footer file?
                    else if (fileName.IndexOf("footer", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        // If this is default (with no language specified) add as DEFAULT
                        if (fileName.ToLower().Contains("footer.htm"))
                        {
                            if (completeSkin.SourceFiles.ContainsKey(Web_Language_Enum.DEFAULT))
                                completeSkin.SourceFiles[Web_Language_Enum.DEFAULT].Footer_Item_Source_File = "html\\" + fileName;
                            else
                            {
                                Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Footer_Source_File = "html\\" + fileName};
                                completeSkin.SourceFiles[Web_Language_Enum.DEFAULT] = sourceFiles;
                            }
                        }
                        else
                        {
                            // Look for and parse the language code in the file
                            string[] parsed = fileName.Split("_-.".ToCharArray());
                            if (parsed.Length == 3)
                            {
                                Web_Language_Enum languageEnum = Web_Language_Enum_Converter.Code_To_Enum(parsed[1]);
                                if (languageEnum != Web_Language_Enum.UNDEFINED)
                                {
                                    if (completeSkin.SourceFiles.ContainsKey(languageEnum))
                                        completeSkin.SourceFiles[languageEnum].Footer_Source_File = "html\\" + fileName;
                                    else
                                    {
                                        Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Footer_Source_File = "html\\" + fileName};
                                        completeSkin.SourceFiles[languageEnum] = sourceFiles;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            // Look for banners as well
	        if (override_banner)
	        {
	            string banner_source_directory = Engine_ApplicationCache_Gateway.Settings.Base_Design_Location + "skins/" + code;
	            if (Directory.Exists(banner_source_directory))
	            {
	                string[] possible_banner_files = Directory.GetFiles(banner_source_directory, "banner*.*");
	                foreach (string thisBannerFile in possible_banner_files)
	                {
	                    // Get the filename
	                    string fileName = Path.GetFileName(thisBannerFile);

	                    // If this is default (with no language specified) add as DEFAULT
	                    if (fileName.ToLower().Contains("banner."))
	                    {
	                        if (completeSkin.SourceFiles.ContainsKey(Web_Language_Enum.DEFAULT))
	                            completeSkin.SourceFiles[Web_Language_Enum.DEFAULT].Banner = fileName;
	                        else
	                        {
	                            Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Banner = fileName};
	                            completeSkin.SourceFiles[Web_Language_Enum.DEFAULT] = sourceFiles;
	                        }
	                    }
	                    else
	                    {
	                        // Look for and parse the language code in the file
	                        string[] parsed = fileName.Split("_-.".ToCharArray());
	                        if (parsed.Length == 3)
	                        {
	                            Web_Language_Enum languageEnum = Web_Language_Enum_Converter.Code_To_Enum(parsed[1]);
	                            if (languageEnum != Web_Language_Enum.UNDEFINED)
	                            {
	                                if (completeSkin.SourceFiles.ContainsKey(languageEnum))
	                                    completeSkin.SourceFiles[languageEnum].Banner = fileName;
	                                else
	                                {
	                                    Complete_Web_Skin_Source_Files sourceFiles = new Complete_Web_Skin_Source_Files {Banner = fileName};
	                                    completeSkin.SourceFiles[languageEnum] = sourceFiles;
	                                }
	                            }
	                        }
	                    }

	                }
	            }
	        }

	        return completeSkin;
	    }


	    /// <summary> Builds a language-specific <see cref="SobekCM_Skin_Object"/> when needed by a user's request </summary>
	    /// <param name="Skin_Row"> Row from a database query with basic information about the interface to build ( codes, override flags, banner link )</param>
	    /// <param name="Language_Code"> Code for the language, which determines which HTML to use </param>
	    /// <returns> Completely built HTML interface object </returns>
	    /// <remarks> The datarow for this method is retrieved from the database by calling the <see cref="Database.SobekCM_Database.Get_All_Web_Skins"/> method during 
	    /// application startup and is then stored in the <see cref="SobekCM_Skin_Collection"/> class until needed. </remarks>
	    public static Web_Skin_Object Build_Skin(DataRow Skin_Row, string Language_Code)
	    {
	        Complete_Web_Skin_Object completeSkinObject = Build_Skin_Complete(Skin_Row);
	        return Build_Skin(completeSkinObject, Language_Code);
	    }

        /// <summary> Builds a language-specific <see cref="SobekCM_Skin_Object"/> when needed by a user's request </summary>
        /// <param name="CompleteSkin"> Complete web skin object </param>
        /// <param name="Language_Code"> Code for the language, which determines which HTML to use </param>
        /// <returns> Completely built HTML interface object </returns>
        /// <remarks> The datarow for this method is retrieved from the database by calling the <see cref="Database.SobekCM_Database.Get_All_Web_Skins"/> method during 
        /// application startup and is then stored in the <see cref="SobekCM_Skin_Collection"/> class until needed. </remarks>
        public static Web_Skin_Object Build_Skin(Complete_Web_Skin_Object CompleteSkin, string Language_Code)
        {
            // Look for the language
            Web_Language_Enum language = Web_Language_Enum_Converter.Code_To_Enum(Language_Code);
            if (!CompleteSkin.SourceFiles.ContainsKey(language))
            {
                language = Engine_ApplicationCache_Gateway.Settings.Default_UI_Language;
            }
            if (!CompleteSkin.SourceFiles.ContainsKey(language))
            {
                language = Web_Language_Enum.DEFAULT;
            }
            if (!CompleteSkin.SourceFiles.ContainsKey(language))
            {
                language = Web_Language_Enum.English;
            }
            if (CompleteSkin.SourceFiles.Count > 0)
            {
                language = CompleteSkin.SourceFiles.Keys.First();
            }
            else
            {
                return null;
            }

            // Now, look in the cache for this
            Web_Skin_Object cacheObject = CachedDataManager.WebSkins.Retrieve_Skin(CompleteSkin.Skin_Code, Web_Language_Enum_Converter.Enum_To_Code(language), null);
            if (cacheObject != null)
                return cacheObject;

            // Build this then
            Web_Skin_Object returnValue = new Web_Skin_Object(CompleteSkin.Skin_Code, CompleteSkin.Base_Skin_Code, "design/skins/" + CompleteSkin.Skin_Code + "/" + CompleteSkin.CSS_Style)
            {
                Override_Banner = CompleteSkin.Override_Banner, 
                Suppress_Top_Navigation = CompleteSkin.Suppress_Top_Navigation
            };

            // If not default, assign the language
            if (language != Web_Language_Enum.DEFAULT)
            {
                returnValue.Language_Code = Web_Language_Enum_Converter.Enum_To_Code(language);
            }

            // Get the source file
            Complete_Web_Skin_Source_Files sourceFiles = CompleteSkin.SourceFiles[language];

            // Build the banner
            if (returnValue.Override_Banner)
            {
                // Find the LANGUAGE-SPECIFIC high-bandwidth banner image
                 if ( !String.IsNullOrEmpty(sourceFiles.Banner))
                {
                    if ( !String.IsNullOrEmpty(CompleteSkin.Banner_Link))
                    {
                        returnValue.Banner_HTML = "<a href=\"" + CompleteSkin.Banner_Link + "\"><img border=\"0\" src=\"<%BASEURL%>skins/" + CompleteSkin.Skin_Code + "/" + sourceFiles.Banner + "\" alt=\"MISSING BANNER\" /></a>";
                    }
                    else
                    {
                        returnValue.Banner_HTML = "<img border=\"0\" src=\"<%BASEURL%>skins/" + CompleteSkin.Skin_Code + "/" + sourceFiles.Banner + "\" alt=\"MISSING BANNER\" />";
                    }
                }
            }

            // Now, set the header and footer html
            string this_header = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location, "skins", CompleteSkin.Skin_Code, sourceFiles.Header_Source_File);
            string this_footer = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location, "skins", CompleteSkin.Skin_Code, sourceFiles.Footer_Source_File);
            string this_item_header = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location, "skins", CompleteSkin.Skin_Code, sourceFiles.Header_Item_Source_File);
            string this_item_footer = Path.Combine(Engine_ApplicationCache_Gateway.Settings.Base_Design_Location, "skins", CompleteSkin.Skin_Code, sourceFiles.Footer_Item_Source_File); 

            // If the item specific stuff doesn't exist, use the regular 
            if (!File.Exists(this_item_header))
                this_item_header = this_header;
            if (!File.Exists(this_item_footer))
                this_item_footer = this_footer;

            // Now, assign all of these
            returnValue.Set_Header_Footer_Source(this_header, this_footer, this_item_header, this_item_footer);

            return returnValue;
        }
	}
}
