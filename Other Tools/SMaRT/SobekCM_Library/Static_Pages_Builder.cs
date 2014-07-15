#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.Items;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Skins;
using SobekCM.Tools.Logs;

#endregion

namespace SobekCM.Library
{
    /// <summary> Class builds the static HTML page for a digital resource to allow indexing by search engines </summary>
    public class Static_Pages_Builder
    {
        private readonly SobekCM_Assistant assistant;
        private Aggregation_Code_Manager codeManager;
        private readonly SobekCM_Navigation_Object currentMode;
        private readonly SobekCM_Skin_Object dlocInterface;

        private int errors;
        private readonly Dictionary<string, Wordmark_Icon> iconList;
        private readonly Item_Lookup_Object itemList;

        /// <summary> MarcXML writer object </summary>
        private readonly MarcXML_File_ReaderWriter marcWriter;

        private readonly string primaryWebServerUrl;
        private readonly string staticSobekcmDataLocation;
        private readonly Custom_Tracer tracer;
        private readonly Language_Support_Info translations;
        private readonly SobekCM_Skin_Object ufdcInterface;

        /// <summary> Constructor for a new instance of the Static_Pages_Builder class </summary>
        /// <param name="Primary_Web_Server_URL"> URL for the primary web server </param>
        /// <param name="Static_Data_Location"> Network location for the data directory </param>
        /// <param name="All_Items_Lookup"> Allows individual items to be retrieved by various methods as <see cref="Application_State.Single_Item"/> objects.</param>
        /// <param name="Code_Manager"> Code manager contains the list of all valid aggregation codes </param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Icon_Table"> Dictionary of all the wordmark/icons which can be tagged to the items </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        public Static_Pages_Builder(string Primary_Web_Server_URL, string Static_Data_Location,
            Language_Support_Info Translator,
            Aggregation_Code_Manager Code_Manager,
            Item_Lookup_Object All_Items_Lookup,
            Dictionary<string, Wordmark_Icon> Icon_Table,
            SobekCM_Skin_Object HTML_Skin)
        {
            primaryWebServerUrl = Primary_Web_Server_URL;
            staticSobekcmDataLocation = Static_Data_Location;

            tracer = new Custom_Tracer();
            assistant = new SobekCM_Assistant();

           // marcWriter = new MARC_Writer();


            // Save all the objects needed by the UFDC Library
            iconList = Icon_Table;
            translations = Translator;
            codeManager = Code_Manager;
            itemList = All_Items_Lookup;
            ufdcInterface = HTML_Skin;
            dlocInterface = HTML_Skin;

            // Create the mode object
            currentMode = new SobekCM_Navigation_Object
                              {
                                  ViewerCode = "citation",
                                  Skin = "UFDC",
                                  Mode = Display_Mode_Enum.Item_Display,
                                  Language = Web_Language_Enum.English,
                                  Base_URL = primaryWebServerUrl
                              };
        }

        /// <summary> Constructor for a new instance of the Static_Pages_Builder class </summary>
        /// <param name="Primary_Web_Server_URL"> URL for the primary web server </param>
        /// <param name="Static_Data_Location"> Network location for the data directory </param>
        /// <remarks> This constructor pulls all the needed information from the database</remarks>
        public Static_Pages_Builder(string Primary_Web_Server_URL, string Static_Data_Location )
        {
            primaryWebServerUrl = Primary_Web_Server_URL;
            staticSobekcmDataLocation = Static_Data_Location;

            tracer = new Custom_Tracer();
            assistant = new SobekCM_Assistant();

           // marcWriter = new MARC_Writer();

            // Get the list of all items
            SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connection_String; ;

            // Build all the objects needed by the UFDC Library
            iconList = new Dictionary<string, Wordmark_Icon>();
            SobekCM_Database.Populate_Icon_List(iconList, tracer);

            translations = new Language_Support_Info();
            SobekCM_Database.Populate_Translations(translations, tracer);

            codeManager = new Aggregation_Code_Manager();
            SobekCM_Database.Populate_Code_Manager(codeManager, tracer);

            // Get the item list and build the hashtable 
       //     DataSet tempSet = SobekCM.Library.Database.SobekCM_Database.Get_Item_List(false, tracer);
            itemList = new Item_Lookup_Object();
            SobekCM_Database.Populate_Item_Lookup_Object(false, itemList, tracer);

            // Set some constant settings
            // SobekCM.Library.SobekCM_Library_Settings.Watermarks_URL = primary_web_server_url + "/design/wordmarks/";
            SobekCM_Library_Settings.Base_SobekCM_Location_Relative = primaryWebServerUrl;

            // Create the mode object
            currentMode = new SobekCM_Navigation_Object
                              {
                                  ViewerCode = "FC",
                                  Skin = "UFDC",
                                  Mode = Display_Mode_Enum.Item_Display,
                                  Language = Web_Language_Enum.English,
                                  Base_URL = primaryWebServerUrl
                              };

            // Create the ufdc interface object
            ufdcInterface = new SobekCM_Skin_Object("ufdc", String.Empty, currentMode.Base_Design_URL + "skins/ufdc/ufdc.css")
                                {
                                    Header_Item_HTML =GetHtmlPage(primaryWebServerUrl + "/design/skins/UFDC/html/header_item.html").Replace("<%BREADCRUMBS%>","<a href=\"" + primaryWebServerUrl + "\">UFDC Home</a>").Replace("<%MYSOBEK%>","<a href=\"" + primaryWebServerUrl + "my\">myUFDC Home</a>"),
                                    Footer_Item_HTML = GetHtmlPage(primaryWebServerUrl + "/design/skins/UFDC/html/footer_item.html").Replace("<%VERSION%>", SobekCM_Library_Settings.CURRENT_WEB_VERSION).Replace("src=\"" + currentMode.Base_URL + "design/", "src=\"" + primaryWebServerUrl + "/design/"),
                                    Header_HTML = GetHtmlPage(primaryWebServerUrl + "/design/skins/UFDC/html/header.html").Replace("<%BREADCRUMBS%>", "<a href=\"" + primaryWebServerUrl + "\">UFDC Home</a>").Replace("<%MYSOBEK%>","<a href=\"" + primaryWebServerUrl + "my\">myUFDC Home</a>"),
                                    Footer_HTML = GetHtmlPage(primaryWebServerUrl + "/design/skins/UFDC/html/footer.html").Replace("<%VERSION%>", SobekCM_Library_Settings.CURRENT_WEB_VERSION).Replace("src=\"" + currentMode.Base_URL + "design/", "src=\"" + primaryWebServerUrl + "/design/"),
                                    Language_Code = "",
                                    Override_Banner = false
                                };

            // Create the dLOC_English interface
            dlocInterface = new SobekCM_Skin_Object("dloc", String.Empty, currentMode.Base_Design_URL + "skins/dloc/dloc.css", "<img id=\"mainBanner\" src=\"" + currentMode.Base_URL + "design/skins/dloc/banner.jpg\" alt=\"MISSING BANNER\" />")
                                {
                                    Header_Item_HTML = GetHtmlPage(primaryWebServerUrl + "/design/skins/dloc/html/header_item.html").Replace("<%BREADCRUMBS%>", "<a href=\"" + primaryWebServerUrl + "\">UFDC Home</a>").Replace("<%MYSOBEK%>", "<a href=\"" + primaryWebServerUrl + "my\">myUFDC Home</a>"),
                                    Footer_Item_HTML = GetHtmlPage(primaryWebServerUrl + "/design/skins/dloc/html/footer_item.html").Replace("<%VERSION%>", SobekCM_Library_Settings.CURRENT_WEB_VERSION).Replace("src=\"" + currentMode.Base_URL + "design/", "src=\"" + primaryWebServerUrl + "/design/"),
                                    Header_HTML = GetHtmlPage(primaryWebServerUrl + "/design/skins/dloc/html/header.html").Replace("<%BREADCRUMBS%>", "<a href=\"" + primaryWebServerUrl + "\">UFDC Home</a>").Replace("<%MYSOBEK%>", "<a href=\"" + primaryWebServerUrl + "my\">myUFDC Home</a>"),
                                    Footer_HTML = GetHtmlPage(primaryWebServerUrl + "/design/skins/dloc/html/footer.html").Replace("<%VERSION%>", SobekCM_Library_Settings.CURRENT_WEB_VERSION).Replace("src=\"" + currentMode.Base_URL + "design/", "src=\"" + primaryWebServerUrl + "/design/"),
                                    Language_Code = "",
                                    Override_Banner = false
                                };

            // Disable the cached data manager
            Cached_Data_Manager.Disabled = true;
        }

        /// <summary> Gets the list of all items used during this creation process </summary>
        /// <remarks> Access to this list allows new items to be added to the list </remarks>
        public Item_Lookup_Object Item_List
        {
            get
            {
                return itemList;
            }
        }

        /// <summary> Builds all of the site map files which point to the static HTML pages </summary>
        /// <returns> Number of site maps created ( Only 30,000 links are included in each site map ) </returns>
        public int Build_Site_Maps()
        {
            try
            {
                int site_map_index = 1;
                string site_map_file = "sitemap" + site_map_index + ".xml";
                int record_count = 0;

                StreamWriter writer = new StreamWriter(staticSobekcmDataLocation + site_map_file, false);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                writer.WriteLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
                DataSet item_list_table = SobekCM_Database.Get_Item_List(false, null);
                foreach (DataRow thisRow in item_list_table.Tables[0].Rows)
                {
                    // Ready to start the next site map?
                    if (record_count > 30000)
                    {
                        writer.WriteLine("</urlset>");
                        writer.Flush();
                        writer.Close();

                        site_map_index++;
                        site_map_file = "sitemap" + site_map_index + ".xml";
                        writer = new StreamWriter(staticSobekcmDataLocation + site_map_file, false);
                        record_count = 0;
                        writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        writer.WriteLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
                    }

                    // Determine the folder 
                    string bibid = thisRow["BibID"].ToString();
                    string vid = thisRow["VID"].ToString();
                    DateTime? lastModifiedDate = null;
                    if (thisRow["LastSaved"] != DBNull.Value)
                    {
                        DateTime tryParseDate;
                        if ( DateTime.TryParse(thisRow["LastSaved"].ToString(), out tryParseDate))
                            lastModifiedDate = tryParseDate;
                    }
                
                    writer.WriteLine("\t<url>");
                    writer.WriteLine("\t\t<loc>" + primaryWebServerUrl + bibid + "/" + vid + "</loc>");
                    if (lastModifiedDate.HasValue)
                    {
                        writer.WriteLine("\t\t<lastmod>" + lastModifiedDate.Value.Year + "-" + lastModifiedDate.Value.Month.ToString().PadLeft(2, '0') + "-" + lastModifiedDate.Value.Day.ToString().PadLeft(2, '0') + "</lastmod>");
                    }
                    writer.WriteLine("\t</url>");
                    record_count++;

                }

                writer.WriteLine("</urlset>");
                writer.Flush();
                writer.Close();
                return site_map_index;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary> Rebuilds all MarcXML files </summary>
        /// <param name="resource_directory"> Directory under which all the resources within this SobekCM library exist </param>
        /// <returns> The number of encountered errors </returns>
        public int Rebuild_All_MARC_Files( string resource_directory )
        {
            // Set the item for the current mode
            errors = 0;
            int successes = 0;

            DataSet item_list_table = SobekCM_Database.Get_Item_List(false, null);
            foreach (DataRow thisRow in item_list_table.Tables[0].Rows)
            {
                string bibid = thisRow["BibID"].ToString();
                string vid = thisRow["VID"].ToString();

                if (!Create_MarcXML_File(bibid, vid, resource_directory))
                {
                    errors++;
                }
                else
                {
                    successes++;
                    if (successes % 1000 == 0 )
                        Console.WriteLine(@"{0} complete", successes);
                }
            }

            return errors;
        }

        /// <summary> Rebuilds all static pages, including the RSS feeds and site maps </summary>
        /// <param name="logger"> Log file to record progress </param>
        /// <param name="build_all_citation_pages"> Flag indicates to build the individual static HTML pages for each digital resource </param>
        /// <param name="rss_feed_location"> Location where the RSS feeds should be updated to </param>
        /// <returns> The number of encountered errors </returns>
        public int Rebuild_All_Static_Pages(LogFileXHTML logger, bool build_all_citation_pages, string rss_feed_location)
        {
            logger.AddNonError("Rebuilding all static pages");

            // Set the item for the current mode
            errors = 0;
            if (build_all_citation_pages)
            {
                DataSet item_list_table = SobekCM_Database.Get_Item_List(false, null);
                foreach (DataRow thisRow in item_list_table.Tables[0].Rows)
                {
                    //if (errors > 10000)
                    //{
                    //    logger.AddError("10000 errors encountered!");
                    //    Console.WriteLine("10000 errors encountered");

                    //    return errors;
                    //}
                    string bibid = thisRow["BibID"].ToString();
                    string vid = thisRow["VID"].ToString();

                    if (Create_Item_Citation_HTML(bibid, vid, thisRow["File_Location"] + "\\" + bibid + "_" + vid + ".html", String.Empty))
                    {
                        //successes.WriteLine(bibid + "\t" + vid);

                    }
                    else
                    {
                        errors++;
                        //failures.WriteLine(bibid + "\t" + vid);
                    }
                }
            }


            // Set the mode away from the display item mode
            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;

            // Get the list of all collections
            DataTable allCollections = SobekCM_Database.Get_Codes_Item_Aggregations( null);
            DataView collectionView = new DataView(allCollections) {Sort = "Name ASC"};

            // Build the basic site map first
            StreamWriter sitemap_writer = new StreamWriter(staticSobekcmDataLocation + "sitemap_collections.xml", false);
            sitemap_writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap_writer.WriteLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            sitemap_writer.WriteLine("\t<url>");
            sitemap_writer.WriteLine("\t\t<loc>" + primaryWebServerUrl + "</loc>");
            sitemap_writer.WriteLine("\t</url>");

            // Prepare to build all the links to static pages
            StringBuilder static_browse_links = new StringBuilder();
            StringBuilder recent_rss_link_builder = new StringBuilder();
            StringBuilder all_rss_link_builder = new StringBuilder();
            int col = 2;
            DataSet items;
            List<string> processed_codes = new List<string>();
            foreach (DataRowView thisCollectionView in collectionView)
            {
                // Clear the tracer
                tracer.Clear();

                if (!Convert.ToBoolean(thisCollectionView.Row["Hidden"]))
                {
                    // Build the static links pages
                    string code = thisCollectionView.Row["Code"].ToString().ToLower();
                    if ((!processed_codes.Contains(code)) && ( code != "all" ))
                    {
                        processed_codes.Add(code);

                        // Add this to the sitemap
                        sitemap_writer.WriteLine("\t<url>");
                        sitemap_writer.WriteLine("\t\t<loc>" + primaryWebServerUrl + code + "</loc>");
                        sitemap_writer.WriteLine("\t</url>");

                        logger.AddNonError(".....Building static links page... " + code);
                        Console.WriteLine(@"Building static links page... {0}", code);

                        //Get the list of items for this collection
                        items = SobekCM_Database.Simple_Item_List(code, tracer);

                        // Continue if there were items
                        if ((items != null) && (items.Tables[0].Rows.Count > 0))
                        {
                            // Build the static browse pages
                            if (Build_All_Browse(code, items))
                            {
                                static_browse_links.Append("<td width=\"150px\"><a href=\"" + code + "_list.html\">" + code + "</a></td>" + Environment.NewLine );
                                col++;
                            }

                            if (col > 5)
                            {
                                static_browse_links.Append("</tr>" + Environment.NewLine + "<tr>");
                                col = 1;
                            }

                            // Build the RSS feeds
                            logger.AddNonError(".....Building RSS feed... " + code);
                            Console.WriteLine(@"Building RSS feed... {0}", code);
                            if (Create_RSS_Feed(code, staticSobekcmDataLocation + "rss\\", thisCollectionView.Row["Name"].ToString(), items))
                            {
                                recent_rss_link_builder.Append("<img src=\"" + primaryWebServerUrl + "default/images/16px-Feed-icon.svg.png\" alt=\"RSS\" width=\"16\" height=\"16\">&nbsp;<a href=\"" + primaryWebServerUrl + "rss/" + code + "_short_rss.xml\">" + thisCollectionView.Row["Name"] + "</a><br />" + Environment.NewLine );
                                all_rss_link_builder.Append("<img src=\"" + primaryWebServerUrl + "default/images/16px-Feed-icon.svg.png\" alt=\"RSS\" width=\"16\" height=\"16\">&nbsp;<a href=\"" + primaryWebServerUrl + "rss/" + code + "_rss.xml\">" + thisCollectionView.Row["Name"] + "</a><br />" + Environment.NewLine );
                            }
                        }
                    }
                }
            }

            // Finish out the collection sitemap
            sitemap_writer.WriteLine("</urlset>");
            sitemap_writer.Flush();
            sitemap_writer.Close();

            items = SobekCM_Database.Simple_Item_List(String.Empty, tracer);
            logger.AddNonError(".....Building static links page... ALL ITEMS");
            Console.WriteLine(@"Building static links page... ALL ITEMS");
            Build_All_Browse(String.Empty, items);

            Console.WriteLine(@"Building RSS feed... ALL ITEMS");
            logger.AddNonError(".....Building RSS feed... ALL ITEMS");
            Create_RSS_Feed("all", staticSobekcmDataLocation + "rss\\", "All UFDC Items", items);

            // Build the site maps
            logger.AddNonError(".....Building site maps");
            Console.WriteLine(@"Building site maps");
            int sitemaps = Build_Site_Maps();

            // Output the main browse and rss links pages
            logger.AddNonError("....Building main browse html page");
            Console.WriteLine(@"Building main browse html page");
            StreamWriter allListWriter = new StreamWriter(staticSobekcmDataLocation + "index.html", false);
            allListWriter.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            allListWriter.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            allListWriter.WriteLine("<head>");
            allListWriter.WriteLine("  <title>UFDC Site Map Links</title>");
            allListWriter.WriteLine();
            allListWriter.WriteLine("  <!-- Static HTML generated by application written by Mark Sullivan -->");
            allListWriter.WriteLine("  <meta name=\"robots\" content=\"index, follow\">");
            allListWriter.WriteLine("  <link href=\"" + primaryWebServerUrl + "default/SobekCM.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
            allListWriter.WriteLine("  <style type=\"text/css\" media=\"screen\">");
            allListWriter.WriteLine("    @import url( http://ufdc.ufl.edu/design/skins/ufdc/ufdc.css );");
            allListWriter.WriteLine("  </style>");
            allListWriter.WriteLine("</head>");
            allListWriter.WriteLine("<body>");

            allListWriter.WriteLine("<div id=\"container-inner\">");


            Display_Header(allListWriter, ufdcInterface);

            allListWriter.WriteLine("<div id=\"pagecontainer\">");


            allListWriter.WriteLine("<center><a href=\"" + primaryWebServerUrl + "\"><img id=\"mainBanner\" src=\"" + primaryWebServerUrl + "design/aggregations/all/images/banners/coll.jpg\" alt=\"MISSING BANNER\" /></a></center>");

            allListWriter.WriteLine("<br /><br />This page is to provide static links to all resources in UFDC. <br />");
            allListWriter.WriteLine("Click <a href=\"" + primaryWebServerUrl + "\">HERE</a> to return to main library. <br />");
            allListWriter.WriteLine("<br />");
            allListWriter.WriteLine("<br />");
            allListWriter.WriteLine("SITE MAPS<br />");
            allListWriter.WriteLine("<br />");
            allListWriter.WriteLine("<a href=\"sitemap_collections.xml\">Map to all the collection home pages</a><br />");
            if (sitemaps > 0)
            {           
                for (int i = 1; i <= sitemaps; i++)
                {
                    allListWriter.WriteLine("<a href=\"sitemap" + i + ".xml\">Site Map File " + i + "</a><br />");
                }
                allListWriter.WriteLine("<br />");
                allListWriter.WriteLine("<br />");
            }
            else
            {
                allListWriter.WriteLine("NO SITE MAPS GENERATED!");
            }

            Display_Footer(allListWriter, ufdcInterface);
            allListWriter.WriteLine("</div>");
            allListWriter.WriteLine("</body>");
            allListWriter.WriteLine("</html>");
            allListWriter.Flush();
            allListWriter.Close();

            // Create the list of all the RSS feeds
            try
            {
                logger.AddNonError(".....Building main rss feed page");
                Console.WriteLine(@"Building main rss feed page");
                StreamWriter writer = new StreamWriter(staticSobekcmDataLocation + "rss\\index.htm", false);
                writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
                writer.WriteLine("<head>");
                writer.WriteLine("  <title>RSS Feeds for UFDC</title>");
                writer.WriteLine();
                writer.WriteLine("  <!-- Static HTML generated by application written by Mark Sullivan -->");
                writer.WriteLine("  <meta name=\"robots\" content=\"index, follow\">");
                writer.WriteLine("  <link href=\"" + primaryWebServerUrl + "default/SobekCM.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
                writer.WriteLine("  <style type=\"text/css\" media=\"screen\">");
                writer.WriteLine("    @import url( http://ufdc.ufl.edu/design/skins/ufdc/ufdc.css );");
                writer.WriteLine("  </style>");
                writer.WriteLine("</head>");
                writer.WriteLine("<body>");

                writer.WriteLine("<div id=\"container-inner\">");
                Display_Header(writer, ufdcInterface);
                writer.WriteLine("<div id=\"pagecontainer\">");

                writer.WriteLine("<center><a href=\"" + primaryWebServerUrl + "\"><img id=\"mainBanner\" src=\"" + primaryWebServerUrl + "design/aggregations/all/images/banners/coll.jpg\" alt=\"MISSING BANNER\" /></a></center>");


                writer.WriteLine("<div class=\"ViewsBrowsesRow\">");
                writer.WriteLine("  <a href=\"" + primaryWebServerUrl + "\"><img src=\"" + primaryWebServerUrl + "design/skins/ufdc/tabs/cL.gif\" border=\"0\" alt=\"\" /><span class=\"tab\"> UFDC HOME </span><img src=\"" + primaryWebServerUrl + "design/skins/ufdc/tabs/cR.gif\" border=\"0\" alt=\"\" /></a>");
                writer.WriteLine("  <img src=\"" + primaryWebServerUrl + "design/skins/ufdc/tabs/cL_s.gif\" border=\"0\" alt=\"\" /><span class=\"tab_s\"> RSS FEEDS </span><img src=\"" + primaryWebServerUrl + "design/skins/ufdc/tabs/cR_s.gif\" border=\"0\" alt=\"\" />");
                writer.WriteLine("</div>");
                writer.WriteLine();
                writer.WriteLine("<div class=\"SobekSearchPanel\">");
                writer.WriteLine("  <h1>RSS Feeds for the University of Florida Digital Collections</h1>");
                writer.WriteLine("</div>");
                writer.WriteLine();

                writer.WriteLine("<div class=\"SobekHomeText\">");
                writer.WriteLine("<table width=\"700\" border=\"0\" align=\"center\">");
                writer.WriteLine("  <tr>");
                writer.WriteLine("    <td>");
                writer.WriteLine("      <br />");
                writer.WriteLine("      This page provides links to RSS feeds for items within UFDC.  The first group of RSS feeds below contains the last 20 items added to the collection.  The second group of items contains links to every item in a collection.  These rss feeds can grow quite lengthy and the load time is often non-trivial.<br />");
                writer.WriteLine("      <br />");
                writer.WriteLine("      In addition, the following three RSS feeds are provided:");
                writer.WriteLine("      <blockquote>");
                writer.WriteLine("        <img src=\"" + primaryWebServerUrl + "default/images/16px-Feed-icon.svg.png\" alt=\"RSS\" width=\"16\" height=\"16\">&nbsp;<a href=\"" + primaryWebServerUrl + "rss/all_rss.xml\">All items in UFDC</a><br />");
                writer.WriteLine("        <img src=\"" + primaryWebServerUrl + "default/images/16px-Feed-icon.svg.png\" alt=\"RSS\" width=\"16\" height=\"16\">&nbsp;<a href=\"" + primaryWebServerUrl + "rss/all_short_rss.xml\">Most recently added items in UFDC (last 100)</a><br />");
                writer.WriteLine("        <img src=\"" + primaryWebServerUrl + "default/images/16px-Feed-icon.svg.png\" alt=\"RSS\" width=\"16\" height=\"16\">&nbsp;<a href=\"" + primaryWebServerUrl + "rss/ufdc_news_rss.xml\">Recent UFDC news and documentation changes</a><br />");
                writer.WriteLine("      </blockquote>");
                writer.WriteLine("      RSS feeds	are a way to keep up-to-date on new materials that are added to the Digital Collections. RSS feeds are written in XML    and require a news reader to access.<br />");
                writer.WriteLine("      <br />");
                writer.WriteLine("      You can download and install a <a href=\"http://dmoz.org/Reference/Libraries/Library_and_Information_Science/Technical_Services/Cataloguing/Metadata/RDF/Applications/RSS/News_Readers/\">news reader</a>.  Or, you can use a Web-based reader such as <a href=\"http://www.google.com/reader\">Google Reader </a>or <a href=\"http://my.yahoo.com/\">My Yahoo!</a>.");
                writer.WriteLine("      Follow the instructions in your reader to subscribe to the feed of   your choice. You will usually need to copy and paste the feed URL into the reader. <br />");
                writer.WriteLine("      <br />");
                writer.WriteLine("    </td>");
                writer.WriteLine("  </tr>");
                writer.WriteLine("</table>");

                writer.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"1\" cellspacing=\"0\">");
                writer.WriteLine("	<tr>");
                writer.WriteLine("    <td bgcolor=\"#cccccc\">");
                writer.WriteLine("      <table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"2\" cellspacing=\"0\">");
                writer.WriteLine("		  <tr>");
                writer.WriteLine("          <td bgcolor=\"#f4f4f4\"><span class=\"groupname\"><span class=\"groupnamecaps\"> &nbsp; M</span>OST <span class=\"groupnamecaps\">R</span>ECENT <span class=\"groupnamecaps\">I</span>TEMS (BY COLLECTION)</span></td>");
                writer.WriteLine("        </tr>");
                writer.WriteLine("      </table>");
                writer.WriteLine("    </td>");
                writer.WriteLine("  </tr>");
                writer.WriteLine("<table>");

                writer.WriteLine("<div class=\"SobekHomeText\">");
                writer.WriteLine("<table width=\"700\" border=\"0\" align=\"center\">");
                writer.WriteLine("  <tr>");
                writer.WriteLine("    <td>");
                writer.WriteLine(recent_rss_link_builder.ToString());
                writer.WriteLine("      <br />");
                writer.WriteLine("    </td>");
                writer.WriteLine("  </tr>");
                writer.WriteLine("</table>");

                writer.WriteLine("<table width=\"750\" border=\"0\" align=\"center\" cellpadding=\"1\" cellspacing=\"0\">");
                writer.WriteLine("	<tr>");
                writer.WriteLine("    <td bgcolor=\"#cccccc\">");
                writer.WriteLine("      <table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"2\" cellspacing=\"0\">");
                writer.WriteLine("		  <tr>");
                writer.WriteLine("          <td bgcolor=\"#f4f4f4\"><span class=\"groupname\"><span class=\"groupnamecaps\"> &nbsp; A</span>LL <span class=\"groupnamecaps\">I</span>TEMS (BY COLLECTION) </span></td>");
                writer.WriteLine("        </tr>");
                writer.WriteLine("      </table>");
                writer.WriteLine("    </td>");
                writer.WriteLine("  </tr>");
                writer.WriteLine("<table>");

                writer.WriteLine("<div class=\"SobekHomeText\">");
                writer.WriteLine("<table width=\"700\" border=\"0\" align=\"center\">");
                writer.WriteLine("  <tr>");
                writer.WriteLine("    <td>");
                writer.WriteLine(all_rss_link_builder.ToString());
                writer.WriteLine("      <br />");
                writer.WriteLine("    </td>");
                writer.WriteLine("  </tr>");
                writer.WriteLine("</table>");


                writer.WriteLine("<br />");



                Display_Footer(writer, ufdcInterface);
                writer.WriteLine("</div>");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");

                writer.Flush();
                writer.Close();
            }
            catch
            {
                logger.AddError("ERROR BUILDING RSS INDEX.HTM FILE");
                Console.WriteLine(@"Error building RSS index.htm file");
            }

            return errors;

        }

        /// <summary> Build the browse XML file that includes links to all items in a particular collection </summary>
        /// <param name="Collection_Code"> Aggregation code for the collection to use </param>
        /// <param name="allItems"> List of all items linked to that aggregation/collection </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Build_All_Browse(string Collection_Code, DataSet allItems)
        {
            if (allItems == null)
                return false;

            if (Collection_Code.Length == 0)
                Collection_Code = "all";

            // Pull the item aggregation object
            Item_Aggregation aggregation = Item_Aggregation_Builder.Get_Item_Aggregation( Collection_Code.ToLower(), "en", null, true, null );

            // Get the skin to use
            SobekCM_Skin_Object skin = ufdcInterface;
            if ( aggregation.Default_Skin.ToLower() == "dloc" )
                skin = dlocInterface;

            StreamWriter writer = new StreamWriter(staticSobekcmDataLocation + Collection_Code + "_all.html", false);

            writer.WriteLine("<div id=\"container-inner\">");
            Display_Header(writer, skin);

            writer.WriteLine("<div id=\"pagecontainer\">");
            writer.WriteLine("<center><a href=\"" + primaryWebServerUrl + Collection_Code + "\"><img id=\"mainBanner\" src=\"" + primaryWebServerUrl + "design/aggregations/" + Collection_Code + "/images/banners/coll.jpg\" alt=\"MISSING BANNER\" /></a></center>");


            writer.WriteLine("<div class=\"ViewsBrowsesRow\">");
            writer.WriteLine("  <a href=\"" + primaryWebServerUrl + Collection_Code + "\"><img src=\"" + primaryWebServerUrl + "/design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cL.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\"> HOME </span><img src=\"" + primaryWebServerUrl + "/design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cR.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /></a>");
            writer.WriteLine("  <img src=\"" + primaryWebServerUrl + "/design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cL_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\"> ALL ITEMS </span><img src=\"" + primaryWebServerUrl + "/design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cR_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />");
            writer.WriteLine("</div>");
 
            writer.WriteLine("<div class=\"SobekBrowseDescPanel\">");
            writer.WriteLine("  <h1>All Items</h1>");
            writer.WriteLine("</div>");
 
            writer.WriteLine("<div class=\"SobekResultsNavBarImbed\">");
            writer.WriteLine("  <br />");
            writer.WriteLine("  " + allItems.Tables[0].Rows.Count + " of " + allItems.Tables[0].Rows.Count + " matching titles");
            writer.WriteLine("</div>");
 
            writer.WriteLine("<div class=\"ResultViewSelectRow\">");
            writer.WriteLine("  <img src=\"" + primaryWebServerUrl + "design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cLD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\">BRIEF VIEW</span><img src=\"" + primaryWebServerUrl + "design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cRD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />");
            writer.WriteLine("  <img src=\"" + primaryWebServerUrl + "design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\">TABLE VIEW</span><img src=\"" + primaryWebServerUrl + "design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />");
            writer.WriteLine("  <img src=\"" + primaryWebServerUrl + "design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\">THUMBNAIL VIEW</span><img src=\"" + primaryWebServerUrl + "design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />");
            writer.WriteLine("  <img src=\"" + primaryWebServerUrl + "design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\">FULL VIEW</span><img src=\"" + primaryWebServerUrl + "design/skins/" + ufdcInterface.Base_Skin_Code + "/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />");
            writer.WriteLine("</div>");

            writer.WriteLine("<table width=\"100%\">");
            writer.WriteLine("<tr valign=\"top\">");
            writer.WriteLine("<td align=\"center\">");
            writer.WriteLine("<div class=\"SobekResultsPanel\" align=\"center\">");


            writer.WriteLine("<br />");
            writer.WriteLine("<br />");

            // Add links for each item
            foreach (DataRow thisRow in allItems.Tables[0].Rows)
            {
                // Determine the folder 
                string bibid = thisRow["BibID"].ToString();
                string vid = thisRow["VID"].ToString();

                writer.WriteLine("<a href=\"" + primaryWebServerUrl + bibid + "/" + vid + "\">" + thisRow["Title"] + "</a><br />");
            }

            writer.WriteLine("<br />");
            writer.WriteLine("<br />");

            writer.WriteLine("</div>");
            writer.WriteLine("</td>");
            writer.WriteLine("</tr>");
            writer.WriteLine("</table>");

            Display_Footer(writer, skin);

            writer.WriteLine("</div>");
            writer.Flush();
            writer.Close();

            return true;
        }

        /// <summary> Forces this builder clsas to refresh all of the data values from the database </summary>
        public void Refresh()
        {
            try
            {
                // Get the item list and build the hashtable 
                itemList.Clear();
                SobekCM_Database.Populate_Item_Lookup_Object(false, itemList, tracer);

                // Build the code manager
                codeManager = new Aggregation_Code_Manager();
                SobekCM_Database.Populate_Code_Manager(codeManager, tracer);
            }
            catch(Exception)
            {
                // Do nothing in this case
            }
        }

        #region Code for creating static citation item HTML

        /// <summary> Create the static HTML citation page for a single digital resource </summary>
        /// <param name="Current_Item"> Digital resource to write as static citaion html </param>
        /// <param name="Static_FileName"> Name of the resulting html file </param>
        /// <param name="Text_File_Directory"> Directory where any text files may exist for this resource </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Create_Item_Citation_HTML(SobekCM_Item Current_Item, string Static_FileName, string Text_File_Directory )
        {
            //try
            //{
            // Clear the current tracer
            tracer.Clear();

            // Set the item for the current mode
            currentMode.BibID = Current_Item.BibID;
            currentMode.VID = Current_Item.VID;
            currentMode.ViewerCode = "citation";
            currentMode.Skin = "UFDC";
            currentMode.Mode = Display_Mode_Enum.Item_Display;
            currentMode.Language = Web_Language_Enum.English;
            currentMode.Internal_User = false;
            currentMode.Trace_Flag = Trace_Flag_Type_Enum.No;

            // Get the current page
            Page_TreeNode currentPage = SobekCM_Item_Factory.Get_Current_Page(Current_Item, currentMode.Page, tracer);

            // Finish writing this
            Finish_writing_html(Current_Item, currentPage, Static_FileName, Text_File_Directory);

            SobekCM_Library_Settings.Base_SobekCM_Location_Relative = String.Empty;
            return true;
            //}
            //catch
            //{
            //    SobekCM.Library.SobekCM_Library_Settings.Base_SobekCM_Location_Relative = String.Empty;
            //    return false;
            //}
        }

        /// <summary> Creates the static MarcXML file for a digital resource </summary>
        /// <param name="bibid"> Bibliographic identifier ( BibID )</param>
        /// <param name="vid"> Volume identifier ( VID ) </param>
        /// <param name="directory"> Directory where the resultant MarcXML file should be written </param>
        /// <returns>  This will read the currently live METS file and build the digital object with additional
        /// information from the database before writing the MarcXML file </returns>
        public bool Create_MarcXML_File(string bibid, string vid, string directory )
        {
            // Clear the current tracer
            tracer.Clear();

            try
            {

                // Set the item for the current mode
                currentMode.BibID = bibid;
                currentMode.VID = vid;
                currentMode.ViewerCode = "citation";
                currentMode.Skin = "UFDC";
                currentMode.Mode = Display_Mode_Enum.Item_Display;
                currentMode.Language = Web_Language_Enum.English;
                currentMode.Internal_User = false;
                currentMode.Trace_Flag = Trace_Flag_Type_Enum.No;

                // Get the item
                SobekCM_Item currentItem;
                Page_TreeNode currentPage;
                SobekCM_Items_In_Title itemsInTitle;
                assistant.Get_Item(String.Empty, currentMode, itemList, SobekCM_Library_Settings.Image_URL, iconList, tracer, null, out currentItem, out currentPage, out itemsInTitle);
                currentMode.Aggregation = String.Empty;
                if (currentItem == null)
                    return false;

                if (currentItem.Behaviors.Aggregation_Count > 0)
                    currentMode.Aggregation = currentItem.Behaviors.Aggregations[0].Code;
                if (currentMode.Aggregation == "EPC")
                    currentMode.Aggregation = "FLCITY";

                string marcFile = directory + currentItem.Web.File_Root + "\\" + currentItem.VID + "\\marc.xml";

                List<string> collectionnames = new List<string>();
                MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                string Error_Message;
                Dictionary<string, object> options = new Dictionary<string, object>();
                options["MarcXML_File_ReaderWriter:Additional_Tags"] = currentItem.MARC_Sobek_Standard_Tags(codeManager.Get_Collection_Short_Name(currentMode.Aggregation), true, SobekCM_Library_Settings.System_Name, SobekCM_Library_Settings.System_Abbreviation);
                return marcWriter.Write_Metadata(marcFile, currentItem, options, out Error_Message);
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Create the static HTML citation page for a single digital resource </summary>
        /// <param name="bibid"> Bibliographic identifier for the digital resource </param>
        /// <param name="vid"> Volume idenfitier for the digital resource </param>
        /// <param name="Static_FileName"> Name of the resulting html file </param>
        /// <param name="Text_File_Directory"> Directory where any text files may exist for this resource </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> THis is generally called by the UFDC Builder windows application/scheduled task</remarks>
        public bool Create_Item_Citation_HTML(string bibid, string vid, string Static_FileName, string Text_File_Directory)
        {

            // Clear the current tracer
            tracer.Clear();

            // Set the item for the current mode
            currentMode.BibID = bibid;
            currentMode.VID = vid;
            currentMode.ViewerCode = "citation";
            currentMode.Skin = "UFDC";
            currentMode.Mode = Display_Mode_Enum.Item_Display;
            currentMode.Language = Web_Language_Enum.English;
            currentMode.Internal_User = false;
            currentMode.Trace_Flag = Trace_Flag_Type_Enum.No;

            // Get the item
            SobekCM_Item currentItem;
            Page_TreeNode currentPage;
            SobekCM_Items_In_Title itemsInTitle;
            assistant.Get_Item(String.Empty, currentMode, itemList, SobekCM_Library_Settings.Image_URL, iconList, tracer, null, out currentItem, out currentPage, out itemsInTitle );
            if (currentItem.Behaviors.Aggregation_Count > 0)
                currentMode.Aggregation = currentItem.Behaviors.Aggregations[0].Code;
            if (currentMode.Aggregation == "EPC")
                currentMode.Aggregation = "FLCITY";

            // Get the current page
            currentPage = SobekCM_Item_Factory.Get_Current_Page(currentItem, currentMode.Page, tracer);

            // Finish writing this
            Finish_writing_html(currentItem, currentPage, Static_FileName, Text_File_Directory);

            return true;
        }

        private void Finish_writing_html(SobekCM_Item currentItem, Page_TreeNode currentPage, string filename, string text_file_location )
        {
            string bibid = currentItem.BibID;
            currentItem.Behaviors.Text_Searchable = false;

            // Create the HTML writer
            Item_HtmlSubwriter itemWriter = new Item_HtmlSubwriter(currentItem, currentPage, null, codeManager, translations, true, true, currentMode, null, String.Empty, null, tracer)
                                                 {Mode = currentMode, Skin = ufdcInterface};
            SobekCM_Library_Settings.Base_SobekCM_Location_Relative = currentMode.Base_URL;
            if ((SobekCM_Library_Settings.Base_SobekCM_Location_Relative.Length == 0) || (SobekCM_Library_Settings.Base_SobekCM_Location_Relative.Contains("localhost")))
            {
                SobekCM_Library_Settings.Base_SobekCM_Location_Relative = primaryWebServerUrl;
                if (bibid.IndexOf("CA") == 0)
                {
                    currentMode.Skin = "dloc";
                    itemWriter.Skin = dlocInterface;
                    SobekCM_Library_Settings.Base_SobekCM_Location_Relative = "http://www.dloc.com/";
                }
                currentMode.Base_URL = SobekCM_Library_Settings.Base_SobekCM_Location_Relative;
            }

            // Now that the item viewer is built, set the robot flag to suppress some checks
            currentMode.Is_Robot = true;

            // Create the TextWriter
            StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8);

            // Add the header
            Display_Header(writer, itemWriter.Skin);

            // Begin to write the item view
            itemWriter.Write_HTML(writer, tracer);

            // Write the table of contents as static HTML, rather than the TreeView web control
            if ((currentItem.Web.Static_PageCount > 1) && (currentItem.Web.Static_Division_Count > 1))
            {
                writer.WriteLine("        <ul class=\"SobekNavBarMenu\">" + Environment.NewLine + "          <li class=\"SobekNavBarHeader\"> TABLE OF CONTENTS </li>" + Environment.NewLine + "        </ul>" + Environment.NewLine  +
                                 "        <div class=\"HideTocRow\">" + Environment.NewLine + "          <img src=\"" + SobekCM_Library_Settings.Base_SobekCM_Location_Relative + "design/skins/" + itemWriter.Skin.Skin_Code + "/tabs/cLG.gif\" border=\"0\" alt=\"\" /><img src=\"" + SobekCM_Library_Settings.Base_SobekCM_Location_Relative + "design/skins/" + itemWriter.Skin.Skin_Code + "/tabs/AU.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">HIDE</span><img src=\"" + SobekCM_Library_Settings.Base_SobekCM_Location_Relative + "design/skins/" + itemWriter.Skin.Skin_Code + "/tabs/cRG.gif\" border=\"0\" alt=\"\" />" + Environment.NewLine + "        </div>");
                writer.WriteLine("<div class=\"SobekTocTreeView\">");

                // load the table of contents in the tree
                TreeView treeView1 = new TreeView();
                itemWriter.Create_TreeView_From_Divisions(treeView1);

                // Step through all the parent nodes
                writer.WriteLine("<table cellspacing=\"4px\" >");
                foreach (TreeNode thisNode in treeView1.Nodes)
                {
                    writer.WriteLine("  <tr><td width=\"9px\">&nbsp;</td><td>" + thisNode.Text.Replace("ufdcSelectedTocTreeViewItem", "ufdcTocTreeViewItem") + "</td></tr>");
                }
                writer.WriteLine("</table>");
                writer.WriteLine("</div>");
            }


            itemWriter.Write_Additional_HTML(writer, tracer);
            PlaceHolder placeHolder = new PlaceHolder();
            itemWriter.PageViewer.Add_Main_Viewer_Section(placeHolder, tracer);
            Literal citationLiteral = (Literal)placeHolder.Controls[0];
            writer.WriteLine(citationLiteral.Text);
            placeHolder.Controls.Clear();

            writer.WriteLine("<!-- COMMENT HERE -->");

            // Close out this tables and form
            writer.WriteLine("       </tr>");

            // Add the download list if there are some
            if ( currentItem.Divisions.Download_Tree.Has_Files )
            {
                writer.WriteLine("       <tr>");
                // Create the downloads viewer to ouput the html
                Download_ItemViewer downloadViewer = new Download_ItemViewer
                                                         {CurrentItem = currentItem, CurrentMode = currentMode};

                // Add the HTML for this now
                downloadViewer.Add_Main_Viewer_Section(placeHolder, tracer);
                Literal downloadLiteral = (Literal)placeHolder.Controls[0];
                writer.WriteLine(downloadLiteral.Text);
                writer.WriteLine("       </tr>");
            }

            // If there is a table of contents write it again, this time it will be complete
            // and also show a hierarchy if there is one
            if ((currentItem.Web.Static_PageCount > 1) && (currentItem.Web.Static_Division_Count > 1))
            {
                writer.WriteLine("       <tr>");
                writer.WriteLine("         <td align=\"left\"><span class=\"SobekViewerTitle\">Table of Contents</span></td>");
                writer.WriteLine("       </tr>");

                writer.WriteLine("       <tr>");
                writer.WriteLine("          <td>");
                writer.WriteLine("            <div class=\"SobekCitation\">");

                foreach (abstract_TreeNode treeNode in currentItem.Divisions.Physical_Tree.Roots)
                {
                    recursively_write_toc(writer, treeNode, "&nbsp; &nbsp; ");
                }

                writer.WriteLine("            </div>");
                writer.WriteLine("          </td>");
                writer.WriteLine("       </tr>");
            }

            // Is the text file location included, in which case any full text should be appended to the end?
            if ((text_file_location.Length > 0) && ( Directory.Exists(text_file_location)))
            {
                // Get the list of all TXT files in this division
                string[] text_files = Directory.GetFiles(text_file_location, "*.txt");
                Dictionary<string, string> text_files_existing = new Dictionary<string, string>();
                foreach (string thisTextFile in text_files)
                {
                    string text_filename = (new FileInfo(thisTextFile)).Name.ToUpper();
                    text_files_existing[text_filename] = text_filename;
                }
                
                // Are there ANY text files?
                if (text_files.Length > 0)
                {
                    // If this has page images, check for related text files 
                    List<string> text_files_included = new List<string>();
                    bool started = false;
                    if (currentItem.Divisions.Physical_Tree.Has_Files)
                    {
                        // Go through the first 100 text pages
                        List<abstract_TreeNode> pages = currentItem.Divisions.Physical_Tree.Pages_PreOrder;
                        int page_count = 0;
                        foreach (Page_TreeNode thisPage in pages)
                        {
                            // Keep track of the page count
                            page_count++;

                            // Look for files in this page
                            if (thisPage.Files.Count > 0)
                            {
                                bool found_non_thumb_file = false;
                                foreach (SobekCM_File_Info thisFile in thisPage.Files)
                                {
                                    // Make sure this is not a thumb
                                    if (thisFile.System_Name.ToLower().IndexOf("thm.jpg") < 0)
                                    {
                                        found_non_thumb_file = true;
                                        string root = thisFile.File_Name_Sans_Extension;
                                        if (text_files_existing.ContainsKey(root.ToUpper() + ".TXT"))
                                        {
                                            string text_file = text_file_location + "\\" + thisFile.File_Name_Sans_Extension + ".txt";

                                            // SInce this is marked to be included, save this name
                                            text_files_included.Add(root.ToUpper() + ".TXT");

                                            // For size reasons, we only include the text from the first 100 pages
                                            if (page_count <= 100)
                                            {
                                                if (!started)
                                                {
                                                    writer.WriteLine("       <tr>");
                                                    writer.WriteLine("         <td align=\"left\"><span class=\"SobekViewerTitle\">Full Text</span></td>");
                                                    writer.WriteLine("       </tr>");
                                                    writer.WriteLine("       <tr>");
                                                    writer.WriteLine("          <td>");
                                                    writer.WriteLine("            <div class=\"SobekCitation\">");

                                                    started = true;
                                                }

                                                try
                                                {
                                                    StreamReader reader = new StreamReader(text_file);
                                                    string text_line = reader.ReadLine();
                                                    while (text_line != null)
                                                    {
                                                        writer.WriteLine(text_line + "<br />");
                                                        text_line = reader.ReadLine();
                                                    }
                                                    reader.Close();
                                                }
                                                catch
                                                {
                                                    writer.WriteLine("Unable to read file: " + text_file);
                                                }

                                                writer.WriteLine("<br /><br />");
                                            }
                                        }

                                    }

                                    // If a suitable file was found, break here
                                    if (found_non_thumb_file)
                                        break;
                                }
                            }
                        }

                        // End this if it was ever started
                        if (started)
                        {
                            writer.WriteLine("            </div>");
                            writer.WriteLine("          </td>");
                            writer.WriteLine("       </tr>");
                        }
                    }

                    // Now, check for any other valid text files 
                    List<string> additional_text_files = text_files_existing.Keys.Where(thisTextFile => (!text_files_included.Contains(thisTextFile.ToUpper())) && (thisTextFile.ToUpper() != "AGREEMENT.TXT") && (thisTextFile.ToUpper().IndexOf("REQUEST") != 0)).ToList();

                    // Now, include any additional text files, which would not be page text files, possiblye 
                    // full text for included PDFs, Powerpoint, Word Doc, etc..
                    foreach (string thisTextFile in additional_text_files)
                    {
                        if (!started)
                        {
                            writer.WriteLine("       <tr>");
                            writer.WriteLine("         <td align=\"left\"><span class=\"SobekViewerTitle\">Full Text</span></td>");
                            writer.WriteLine("       </tr>");
                            writer.WriteLine("       <tr>");
                            writer.WriteLine("          <td>");
                            writer.WriteLine("            <div class=\"SobekCitation\">");

                            started = true;
                        }

                        string text_file = text_file_location + "\\" + thisTextFile;

                        try
                        {
                            

                            StreamReader reader = new StreamReader(text_file);
                            string text_line = reader.ReadLine();
                            while (text_line != null)
                            {
                                writer.WriteLine(text_line + "<br />");
                                text_line = reader.ReadLine();
                            }
                            reader.Close();
                        }
                        catch
                        {
                            writer.WriteLine("Unable to read file: " + text_file);
                        }

                        writer.WriteLine("<br /><br />");
                    }
                }
            }

            writer.WriteLine("      </table>");
            writer.WriteLine("    </td>");
            writer.WriteLine("  </tr>");
            writer.WriteLine("</table>");

            // Write the footer
            Display_Footer(writer, itemWriter.Skin);

            writer.Flush();
            writer.Close();

            // Restore the text searchable flag and robot flag
            currentMode.Is_Robot = false;
            currentItem.Behaviors.Text_Searchable = false;
        }

        private void recursively_write_toc(StreamWriter writer, abstract_TreeNode treeNode, string indent)
        {
            // Write this label, regardless of whether it is a page or a division (some page names have useful info)
            if ( treeNode.Label.Length > 0 )
                writer.WriteLine(indent + treeNode.Label + "<br />");
            else
                writer.WriteLine(indent + treeNode.Type + "<br />");

            // If not a page, recurse more
            if (!treeNode.Page)
            {
                Division_TreeNode divNode = (Division_TreeNode)treeNode;
                foreach (abstract_TreeNode childNode in divNode.Nodes)
                {
                    recursively_write_toc(writer, childNode, indent + "&nbsp; &nbsp; ");
                }
            }
        }



        /// <summary> Writes the static header to go at the top of the static digital resource page </summary>
        /// <param name="writer"> Open stream to write the HTML header to </param>
        /// <param name="htmlSkin"> Default html web skin/interface</param>
        public void Display_Header(TextWriter writer, SobekCM_Skin_Object htmlSkin)
        {
            string breadcrumbs = "<a href=\"" + currentMode.Base_URL + "\">UFDC Home</a>";

            writer.WriteLine(htmlSkin.Header_Item_HTML.Replace("<%URLOPTS%>", "").Replace("<%?URLOPTS%>", "").Replace("<%&URLOPTS%>", "").Replace("<%BREADCRUMBS%>", breadcrumbs).Replace("<%MYSOBEK%>", "").Replace("<%ENGLISH%>", "").Replace("<%FRENCH%>", "").Replace("<%SPANISH%>", "").Replace("<%LOWGRAPHICS%>", "").Replace("<%HIGHGRAPHICS%>", "").Replace("<%BASEURL%>", currentMode.Base_URL));
            writer.WriteLine(String.Empty);
        }

        /// <summary> Writes the static footer to go at the bottom of the static digital resource page </summary>
        /// <param name="writer"> Open stream to write the HTML footer to </param>
        /// <param name="htmlInterface"> Default html web skin/interface</param>
        public void Display_Footer(TextWriter writer, SobekCM_Skin_Object htmlInterface)
        {
            // Get the current contact URL
            Display_Mode_Enum thisMode = currentMode.Mode;
            currentMode.Mode = Display_Mode_Enum.Contact;
            string contact = currentMode.Redirect_URL();

            // Restore the old mode
            currentMode.Mode = thisMode;

            writer.WriteLine(currentMode.Mode == Display_Mode_Enum.Item_Display
                                 ? htmlInterface.Footer_Item_HTML.Replace("<%CONTACT%>", contact).Replace("<%URLOPTS%>", "").Replace("<%?URLOPTS%>", "").Replace("<%&URLOPTS%>", "").Replace("<%VERSION%>", "").Replace("<%BASEURL%>", currentMode.Base_URL).Trim()
                                 : htmlInterface.Footer_HTML.Replace("<%CONTACT%>", contact).Replace("<%URLOPTS%>", "").Replace("<%?URLOPTS%>", "").Replace("<%&URLOPTS%>", "").Replace("<%VERSION%>", "").Replace("<%BASEURL%>", currentMode.Base_URL).Trim());
        }

        #endregion

        #region Code for creating the RSS feeds for UFDC

        /// <summary> Create the RSS feed files necessary </summary>
        /// <param name="Collection_Code"> Aggregation Code for this collection </param>
        /// <param name="rss_feed_location"> Location for the updated RSS feed to be updated </param>
        /// <param name="Collection_Title"> Title of this aggregation/collection </param>
        /// <param name="allItems"> DataSet of all items within this aggregation </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Create_RSS_Feed(string Collection_Code, string rss_feed_location, string Collection_Title, DataSet allItems)
        {
            try
            {
                if (allItems == null)
                    return false;

                int recordNumber = 0;
                int final_most_recent = 20;
                if (Collection_Code == "all")
                    final_most_recent = 100;

                DataView viewer = new DataView(allItems.Tables[0]) {Sort = "CreateDate DESC"};

                StreamWriter rss_writer = new StreamWriter(rss_feed_location + Collection_Code + "_rss.xml");
                rss_writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                rss_writer.WriteLine("<rss version=\"2.0\">");
                rss_writer.WriteLine("<channel>");
                rss_writer.WriteLine("<title>" + Collection_Title.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;") + " (All Items) </title>");
                rss_writer.WriteLine("<link>" + primaryWebServerUrl+ "data/rss/" + Collection_Code + "_rss.xml</link>");
                rss_writer.WriteLine("<description></description>");
                rss_writer.WriteLine("<language>en</language>");
                rss_writer.WriteLine("<copyright>Copyright 2008-2011</copyright>");
                rss_writer.WriteLine("<lastBuildDate>" + DateTime.Now.ToUniversalTime().ToLongTimeString() + "</lastBuildDate>");
                rss_writer.WriteLine("");

                StreamWriter short_rss_writer = new StreamWriter(rss_feed_location + Collection_Code + "_short_rss.xml");
                short_rss_writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                short_rss_writer.WriteLine("<rss version=\"2.0\">");
                short_rss_writer.WriteLine("<channel>");
                short_rss_writer.WriteLine("<title>" + Collection_Title.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;") + " (Most Recent Items) </title>");
                short_rss_writer.WriteLine("<link>" + primaryWebServerUrl + "data/rss/" + Collection_Code + "_short_rss.xml</link>");
                short_rss_writer.WriteLine("<description></description>");
                short_rss_writer.WriteLine("<language>en</language>");
                short_rss_writer.WriteLine("<copyright>Copyright 2008</copyright>");
                short_rss_writer.WriteLine("<lastBuildDate>" + DateTime.Now.ToUniversalTime().ToLongTimeString() + "</lastBuildDate>");
                short_rss_writer.WriteLine("");

                foreach (DataRowView thisRowView in viewer)
                {
                    // Determine the folder name
                    string bibid = thisRowView.Row["BibID"].ToString();
                    string vid = thisRowView["VID"].ToString();

                    recordNumber++;
                    if (recordNumber <= final_most_recent)
                    {
                        short_rss_writer.WriteLine("<item>");
                        short_rss_writer.WriteLine("<title>" + thisRowView.Row["Title"].ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;") + "</title>");
                        short_rss_writer.WriteLine("<description></description>");


                        short_rss_writer.WriteLine("<link>" + primaryWebServerUrl + bibid + "/" + vid + "</link>");
                        try
                        {
                            string create_date_string = thisRowView.Row["CreateDate"].ToString();
                            DateTime dateTime = Convert.ToDateTime(create_date_string).ToUniversalTime();
                            if (create_date_string.Length > 0)
                            {
                                string month = "UNK";
                                switch (dateTime.Month)
                                {
                                    case 1:
                                        month = "Jan";
                                        break;
                                    case 2:
                                        month = "Feb";
                                        break;
                                    case 3:
                                        month = "Mar";
                                        break;
                                    case 4:
                                        month = "Apr";
                                        break;
                                    case 5:
                                        month = "May";
                                        break;
                                    case 6:
                                        month = "Jun";
                                        break;
                                    case 7:
                                        month = "Jul";
                                        break;
                                    case 8:
                                        month = "Aug";
                                        break;
                                    case 9:
                                        month = "Sep";
                                        break;
                                    case 10:
                                        month = "Oct";
                                        break;
                                    case 11:
                                        month = "Nov";
                                        break;
                                    case 12:
                                        month = "Dec";
                                        break;

                                }
                                short_rss_writer.WriteLine("<pubDate>" + dateTime.DayOfWeek.ToString() + ", " + dateTime.Day + " " + month + " " + dateTime.Year + " " + dateTime.Hour + ":" + dateTime.Minute.ToString().PadLeft(2, '0') + ":" + dateTime.Second.ToString().PadLeft(2, '0') + " GMT </pubDate>");
                            }
                        }
                        catch ( Exception )
                        {
                            // Do nothing here
                        }
                        short_rss_writer.WriteLine("</item>");
                        short_rss_writer.WriteLine("");
                    }

                    rss_writer.WriteLine("<item>");
                    rss_writer.WriteLine("<title>" + thisRowView.Row["Title"].ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;") + "</title>");
                    rss_writer.WriteLine("<description></description>");
                    rss_writer.WriteLine("<link>" + primaryWebServerUrl + bibid + "/" + vid + "</link>");

                    try
                    {
                        string create_date_string = thisRowView.Row["CreateDate"].ToString();
                        DateTime dateTime = Convert.ToDateTime(create_date_string).ToUniversalTime();
                        if (create_date_string.Length > 0)
                        {
                            string month = "UNK";
                            switch (dateTime.Month)
                            {
                                case 1:
                                    month = "Jan";
                                    break;
                                case 2:
                                    month = "Feb";
                                    break;
                                case 3:
                                    month = "Mar";
                                    break;
                                case 4:
                                    month = "Apr";
                                    break;
                                case 5:
                                    month = "May";
                                    break;
                                case 6:
                                    month = "Jun";
                                    break;
                                case 7:
                                    month = "Jul";
                                    break;
                                case 8:
                                    month = "Aug";
                                    break;
                                case 9:
                                    month = "Sep";
                                    break;
                                case 10:
                                    month = "Oct";
                                    break;
                                case 11:
                                    month = "Nov";
                                    break;
                                case 12:
                                    month = "Dec";
                                    break;

                            }
                            rss_writer.WriteLine("<pubDate>" + dateTime.DayOfWeek.ToString() + ", " + dateTime.Day + " " + month + " " + dateTime.Year + " " + dateTime.Hour + ":" + dateTime.Minute.ToString().PadLeft(2, '0') + ":" + dateTime.Second.ToString().PadLeft(2, '0') + " GMT </pubDate>");
                        }
                    }
                    catch (Exception)
                    {
                        // Do nothing here
                    }
                    rss_writer.WriteLine("</item>");
                    rss_writer.WriteLine("");
                }

                rss_writer.WriteLine("</channel>");
                rss_writer.WriteLine("</rss>");
                rss_writer.Flush();
                rss_writer.Close();

                short_rss_writer.WriteLine("</channel>");
                short_rss_writer.WriteLine("</rss>");
                short_rss_writer.Flush();
                short_rss_writer.Close();

                return true;
            }
            catch
            {
                Console.WriteLine(@"ERROR BUILDING RSS FEED {0}", Collection_Code);
                return false;
            }
        }

        #endregion

        #region Code for pulling a html page over the web

        private string GetHtmlPage(string strURL)
        {
            try
            {
                // the html retrieved from the page
                String strResult;
                WebRequest objRequest = WebRequest.Create(strURL);
                WebResponse objResponse = objRequest.GetResponse();
                // the using keyword will automatically dispose the object 
                // once complete
                using (StreamReader sr =new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
                return strResult;
            }
            catch
            {
                return "<strong>ERROR LOADING HTML FROM SOURCE (" + strURL + ")</strong><br />";
            }
        }

        #endregion
    }
}
