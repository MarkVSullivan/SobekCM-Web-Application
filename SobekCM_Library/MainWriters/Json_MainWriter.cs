#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes search results, item browses, and item information in Java Simple Object
    /// Notation for interfacing with the iPhone mobile applications </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Json_MainWriter : abstractMainWriter
    {
        private readonly Item_Lookup_Object allItems;
        private readonly string currentGreenstoneImageRoot = String.Empty;

        /// <summary> Constructor for a new instance of the Json_MainWriter class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
         /// <param name="Current_Item"> Current item to display </param>
        /// <param name="Current_Page"> Current page within the item</param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Current_Image_Root"> Current root directory to pull images and metadata for digital resources </param>
        public Json_MainWriter(SobekCM_Navigation_Object Current_Mode,
            Item_Aggregation Hierarchy_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Item_Aggregation_Child_Page Browse_Object,
            SobekCM_Item Current_Item,
            Page_TreeNode Current_Page,
            Item_Lookup_Object All_Items_Lookup,
            string Current_Image_Root)
            : base(Current_Mode, Hierarchy_Object, Results_Statistics, Paged_Results, Browse_Object,   Current_Item, Current_Page, null)        
        {
            allItems = All_Items_Lookup;
            currentGreenstoneImageRoot = Current_Image_Root;
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.Library.Navigation.Writer_Type_Enum.JSON"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.JSON; } }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Results:
                case Display_Mode_Enum.Aggregation:
                    if (paged_results != null)
                        display_search_results(Output, allItems);
                    break;
                case Display_Mode_Enum.Item_Display:
                    display_item_info(Output);
                    break;
                default:
                    Output.Write("JSON Writer - Unknown Mode");
                    break;
            }
        }

        /// <summary> Writes the item information in JSON format directly to the output stream  </summary>
        /// <param name="Output"> Stream to which to write the JSON item information </param>
        protected internal void display_item_info(TextWriter Output)
        {
            // What if the page requested is greater than pages in the book?
            // What is the ID?
            // What if an item does not have jpeg's for each page?  No jpegs at all?
            Output.Write("[");
            if (currentItem != null)
            {
                if (currentMode.ViewerCode != "text")
                {
                    int first_page_to_show = (currentMode.Page - 1) * 20;
                    int last_page_to_show = (currentMode.Page * 20) - 1;
                    if (first_page_to_show < currentItem.Web.Static_PageCount)
                    {
                        int page = first_page_to_show;
                        string jpeg_to_view = String.Empty;
                        while ((page < currentItem.Web.Static_PageCount) && (page <= last_page_to_show))
                        {
                            Page_TreeNode thisPage = currentItem.Web.Pages_By_Sequence[page];
                            bool found = false;
                            foreach (SobekCM_File_Info thisFile in thisPage.Files.Where(thisFile => thisFile.System_Name.ToUpper().IndexOf(".JPG") > 0))
                            {
                                jpeg_to_view = currentGreenstoneImageRoot + currentItem.Web.AssocFilePath + "/" + thisFile.System_Name;
                                found = true;
                                break;
                            }
                            if (found)
                            {
                                if (page > first_page_to_show)
                                    Output.Write(",");
                                jpeg_to_view = jpeg_to_view.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                                Output.Write("{\"item_page\":{\"position\":" + (page + 1) + ",\"image_url\":\"" + jpeg_to_view + "\",\"id\":" + (page + 1) + ",\"collection_item_id\":1}}");
                            }
                            page++;
                        }
                    }
                }
                else
                {
                    // Get the list of all TEXT files
                    List<string> existing_text_files = new List<string>();
                    if (Directory.Exists(SobekCM_Library_Settings.Image_Server_Network + currentItem.Web.AssocFilePath))
                    {
                        string[] allFiles = Directory.GetFiles(SobekCM_Library_Settings.Image_Server_Network + currentItem.Web.AssocFilePath, "*.txt");
                        existing_text_files.AddRange(allFiles.Select(thisFile => (new FileInfo(thisFile)).Name.ToUpper()));
                    }


                    int page = 0;
                    string jpeg_to_view = String.Empty;
                    while (page < currentItem.Web.Static_PageCount)
                    {
                        string text_to_read = String.Empty;
                        Page_TreeNode thisPage = currentItem.Web.Pages_By_Sequence[page];
                        bool found = false;
                        foreach (SobekCM_File_Info thisFile in thisPage.Files)
                        {
                            if (thisFile.System_Name.ToUpper().IndexOf(".JPG") > 0)
                            {
                                if (existing_text_files.Contains(thisFile.System_Name.ToUpper().Replace(".JPG", "") + ".TXT"))
                                {
                                    text_to_read = currentGreenstoneImageRoot + currentItem.Web.AssocFilePath + "/" + thisFile.System_Name.Replace(".JPG", ".TXT").Replace(".jpg", ".txt");
                                }
                                jpeg_to_view = currentGreenstoneImageRoot + currentItem.Web.AssocFilePath + "/" + thisFile.System_Name;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            if (page > 0)
                                Output.Write(",");
                            jpeg_to_view = jpeg_to_view.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                            text_to_read = text_to_read.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

                            Output.Write("{\"item_page\":{\"position\":" + (page + 1) + ",\"image_url\":\"" + jpeg_to_view + "\",\"text_url\":\"" + text_to_read + "\"}}");
                        }
                        page++;
                    }
                }
            }

            Output.Write("]");
        }

        /// <summary> Writes the search or browse information in JSON format directly to the output stream  </summary>
        /// <param name="Output"> Stream to which to write the JSON search or browse information </param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        protected internal void display_search_results(TextWriter Output, Item_Lookup_Object All_Items_Lookup)
        {
            // If results are null, or no results, return empty string
            if ((paged_results == null) || (results_statistics == null) || (results_statistics.Total_Items <= 0))
                return;

            Output.Write("[");

            // Step through all the results
            int i = 1;
            foreach (iSearch_Title_Result titleResult in paged_results)
            {
                // Always get the first item for things like the main link and thumbnail
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);

                // Determine a thumbnail
                string thumb = currentGreenstoneImageRoot + titleResult.BibID.Substring(0,2) + "/" + titleResult.BibID.Substring(2,2) + "/" + titleResult.BibID.Substring(4,2) + "/" + titleResult.BibID.Substring(6,2) + "/" + titleResult.BibID.Substring(8) + "/" + firstItemResult.VID + "/" + firstItemResult.MainThumbnail;
                if ((thumb.ToUpper().IndexOf(".JPG") < 0) && (thumb.ToUpper().IndexOf(".GIF") < 0))
                {
                    thumb = currentMode.Default_Images_URL + "NoThumb.jpg";
                }
                thumb = thumb.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

                // Was a previous item/title included here?
                if (i > 1)
                    Output.Write(",");
                Output.Write("{\"collection_item\":{\"name\":\"" + firstItemResult.Title.Trim().Replace("\"", "'") + "\",\"url\":\"" + SobekCM_Library_Settings.System_Base_URL + titleResult.BibID + "/" + firstItemResult.VID + "\",\"collection_code\":\"\",\"id\":\"" + titleResult.BibID + "_" + firstItemResult.VID + "\",\"thumb_url\":\"" + thumb + "\"}}");

                i++;
            }

            Output.Write("]");
        }
    }
}
