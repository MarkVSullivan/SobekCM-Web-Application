#region Using directives

using System;
using System.Data;
using System.IO;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Library.Skins
{
	/// <summary> Builder creates individiual <see cref="SobekCM_Skin_Object"/> objects when application first starts and 
    /// when a new skin is needed for a user request </summary>
	public class SobekCM_Skin_Collection_Builder
	{
	    /// <summary> Populates/builds the main default HTML skin during application startup </summary>
        /// <param name="Skin_List"> List of skin to populate with the default, commonly used skin</param>
        /// <param name="tracer"> Trace object keeps a list of each method executed and important milestones in rendering  </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> Most HTML skins are built as they are needed and then cached for a period of time.  The main default skins are
        /// permanently stored in this global <see cref="SobekCM_Skin_Collection"/> object.<br /><br />
        /// The default skins currently include: UFDC (english), dLOC (english, french, and spanish) </remarks>
        public static bool Populate_Default_Skins(SobekCM_Skin_Collection Skin_List, Custom_Tracer tracer)
		{
            tracer.Add_Trace("SobekCM_Skin_Collection_Builder.Populate_Default_Skins", "Build the standard interfaces");

            // Get the data from the database
            DataTable skinData = SobekCM_Database.Get_All_Web_Skins(tracer);

            // Just return if the data appears bad..
            if ((skinData == null) || (skinData.Rows.Count == 0))
                return false;

            // Clear existing interfaces
            Skin_List.Clear();

			// Set the data table
            Skin_List.Skin_Table = skinData;

			return true;
		}

	    /// <summary> Builds a specific <see cref="SobekCM_Skin_Object"/> when needed by a user's request </summary>
	    /// <param name="Skin_Row"> Row from a database query with basic information about the interface to build ( codes, override flags, banner link )</param>
	    /// <param name="Language_Code"> Code for the language, which determines which HTML to use </param>
	    /// <returns> Completely built HTML interface object </returns>
	    /// <remarks> The datarow for this method is retrieved from the database by calling the <see cref="Database.SobekCM_Database.Get_All_Web_Skins"/> method during 
	    /// application startup and is then stored in the <see cref="SobekCM_Skin_Collection"/> class until needed. </remarks>
	    public static SobekCM_Skin_Object Build_Skin(DataRow Skin_Row, string Language_Code )
        {
            // Pull values out from this row
            string code = Skin_Row["WebSkinCode"].ToString();
            string base_interface = Skin_Row["BaseInterface"].ToString();
	        bool override_banner = Convert.ToBoolean(Skin_Row["OverrideBanner"]);
            string banner_link = Skin_Row["BannerLink"].ToString();
            string this_style = "design/skins/" + code + "/" + code + ".css";
            if (!File.Exists(SobekCM_Library_Settings.Base_Design_Location + "skins\\" + code + "\\" + code + ".css"))
                this_style = String.Empty;

            // Build the interface, along with any overriding banner
            SobekCM_Skin_Object thisInterface;
            if (override_banner)
            {
                string this_banner = String.Empty;

                // Find the LANGUAGE-SPECIFIC high-bandwidth banner image
                string[] banner_file = Directory.GetFiles(SobekCM_Library_Settings.Base_Design_Location + "skins/" + code, "banner_" + Language_Code + ".*");
                if (banner_file.Length > 0)
                {
                    if (banner_link.Length > 0)
                    {
                        this_banner = "<a href=\"" + banner_link + "\"><img border=\"0\" src=\"<%BASEURL%>skins/" + code + "/" + (new FileInfo(banner_file[0]).Name) + "\" alt=\"MISSING BANNER\" /></a>";
                    }
                    else
                    {
                        this_banner = "<img border=\"0\" src=\"<%BASEURL%>skins/" + code + "/" + (new FileInfo(banner_file[0]).Name) + "\" alt=\"MISSING BANNER\" />";
                    }
                }

                // If nothing was gotten, look for the ENGLISH banner image and use that
                if ((this_banner.Length == 0) && (Language_Code.Length > 0) && (Language_Code != "en"))
                {
                    banner_file = Directory.GetFiles(SobekCM_Library_Settings.Base_Design_Location + "skins/" + code, "banner.*");
                    if (banner_file.Length > 0)
                    {
                        if (banner_link.Length > 0)
                        {
                            this_banner = "<a href=\"" + banner_link + "\"><img border=\"0\" src=\"<%BASEURL%>design/skins/" + code + "/" + (new FileInfo(banner_file[0]).Name) + "\" alt=\"MISSING BANNER\" /></a>";
                        }
                        else
                        {
                            this_banner = "<img border=\"0\" src=\"<%BASEURL%>design/skins/" + code + "/" + (new FileInfo(banner_file[0]).Name) + "\" alt=\"MISSING BANNER\" />";
                        }
                    }
                }

                // Create this interface
                thisInterface = new SobekCM_Skin_Object(code, base_interface, this_style, this_banner);
            }
            else
            {
                // Create an interface without banner override
                thisInterface = new SobekCM_Skin_Object(code, base_interface, this_style);
            }

            // Assign the value to suppress top-level navigation
            thisInterface.Suppress_Top_Navigation = Convert.ToBoolean(Skin_Row["SuppressTopNavigation"]);

            // Assign the header and footer, or pass in the source
            string this_header;
            string this_footer;
            string this_item_header;
            string this_item_footer;

            // If the language isn't english, prepare to default to english
            if ((Language_Code.Length > 0) && (Language_Code != "en"))
            {
                // Assign the default locations for the banners
                this_header = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/header_" + Language_Code + ".html";
                this_footer = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/footer_" + Language_Code + ".html";
                this_item_header = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/header_item_" + Language_Code + ".html";
                this_item_footer = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/footer_item_" + Language_Code + ".html";

                if (!File.Exists(this_header))
                {
                    this_header = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/header.html";
                }

                if (!File.Exists(this_footer))
                {
                    this_footer = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/footer.html";
                }

                if (!File.Exists(this_item_header))
                {
                    this_item_header = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/header_item.html";
                }

                if (!File.Exists(this_item_footer))
                {
                    this_item_footer = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/footer_item.html";
                }
            }
            else
            {
                // Assign the default locations for the banners
                this_header = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/header.html";
                this_footer = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/footer.html";
                this_item_header = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/header_item.html";
                this_item_footer = SobekCM_Library_Settings.Base_Design_Location + "skins/" + code + "/html/footer_item.html";
            }

            // If the item specific stuff doesn't exist, use the regular 
            if (!File.Exists(this_item_header))
                this_item_header = this_header;
            if (!File.Exists(this_item_footer))
                this_item_footer = this_footer;

            // Now, assign all of these
            thisInterface.Set_Header_Footer_Source(this_header, this_footer, this_item_header, this_item_footer);

            // Save the banner override and language code
            thisInterface.Override_Banner = override_banner;
            thisInterface.Language_Code = Language_Code;

            // Return the built interface
            return thisInterface;
        }
	}
}
