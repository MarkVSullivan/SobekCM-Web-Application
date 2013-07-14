#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes search results and item browses as simplified XML to the response stream. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Xml_MainWriter : abstractMainWriter
    {
        /// <summary> Constructor for a new instance of the Xml_MainWriter class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
        /// <param name="Current_Item"> Current item to display </param>
        /// <param name="Current_Page"> Current page within the item</param>
        public Xml_MainWriter(SobekCM_Navigation_Object Current_Mode, 
            Item_Aggregation Hierarchy_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Item_Aggregation_Browse_Info Browse_Object,
            SobekCM_Item Current_Item,
            Page_TreeNode Current_Page)
            : base(Current_Mode, Hierarchy_Object, Results_Statistics, Paged_Results, Browse_Object, Current_Item, Current_Page, null)
        {
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.Library.Navigation.Writer_Type_Enum.XML"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.XML; } }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Results:
                case Display_Mode_Enum.Aggregation_Browse_Info:
                    if (paged_results != null)
                        display_search_results(Output);
                    break;
                default:
                    Output.Write("XML Writer - Unknown Mode");
                    break;
            }
        }

        /// <summary> Display search results in simple XML format </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        protected internal void display_search_results(TextWriter Output )
        {
            // Get the URL and network roots
            string url = SobekCM_Library_Settings.Image_URL;
            string network = SobekCM_Library_Settings.Image_Server_Network;
            string base_url = currentMode.Base_URL.Replace("sobekcm_data.aspx", "");

            // Write the header first
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<ResultSet Page=\"" + currentMode.Page + "\" Total=\"" + base.results_statistics.Total_Titles + "\">");

            // Now, add XML for each title
            string lastBibID = string.Empty;
            foreach (iSearch_Title_Result thisResult in base.paged_results)
            {
                if (thisResult.BibID != lastBibID)
                {
                    if ( lastBibID.Length > 0 )
                        Output.WriteLine("</TitleResult>");
                    Output.WriteLine("<TitleResult ID=\"" + thisResult.BibID + "\">");
                    lastBibID = thisResult.BibID;
                }

                // Determine folder from BibID
                string folder = thisResult.BibID.Substring(0,2) + "/" + thisResult.BibID.Substring(2,2) + "/" + thisResult.BibID.Substring(4,2) + "/" + thisResult.BibID.Substring(6,2) + "/" + thisResult.BibID.Substring(8);
                                
                // Now, add XML for each item
                for( int i = 0 ; i < thisResult.Item_Count ; i++ )
                {
                    iSearch_Item_Result itemResult = thisResult.Get_Item(i);
                    Output.WriteLine("\t<ItemResult ID=\"" + thisResult.BibID + "_" + itemResult.VID + "\">");
                    Output.Write("\t\t<Title>" );
                    Write_XML(Output, itemResult.Title);
                    Output.WriteLine("</Title>");
                    if (itemResult.PubDate.Length > 0)
                    {
                        Output.Write("\t\t<Date>");
                        Write_XML(Output, itemResult.PubDate);
                        Output.WriteLine("</Date>");
                    }
                    Output.WriteLine("\t\t<Location>");
                    Output.WriteLine("\t\t\t<URL>" + base_url + thisResult.BibID + "/" + itemResult.VID + "</URL>");
                    Output.WriteLine("\t\t\t<Folder type=\"web\">" + url + folder + "/" + itemResult.VID + "</Folder>");
                    Output.WriteLine("\t\t\t<Folder type=\"network\">" + network + folder.Replace("/","\\") + "\\" + itemResult.VID + "</Folder>");
                    Output.WriteLine("\t\t</Location>");
                    Output.WriteLine("\t</ItemResult>");
                }                          
            }

            if ( base.paged_results.Count > 0 )
                Output.WriteLine("</TitleResult>");          
            Output.WriteLine("</ResultSet>");

            //SobekCM.Resource_Object.Database.DataSets.UFDC_All_Items simplified_xml = new SobekCM.Resource_Object.Database.DataSets.UFDC_All_Items();

            //List<Results.SobekCM_Item_Collection.SobekCM_Item_Row> itemRows;
            //for ( int i = 0 ; i < search_results.Title_Count ; i++ )
            //{
            //    Results.SobekCM_Item_Collection.SobekCM_Item_Group_Row titleRow = search_results.GetRow(i);

            //    // Get the related child rows
            //    itemRows = titleRow.Child_Rows;

            //    // Step through each child row and add to the new table
            //    foreach (SobekCM.Library.Results.SobekCM_Item_Collection.SobekCM_Item_Row itemRow in itemRows)
            //    {
            //        Application_State.Single_Item dbItem = allItems.Item_By_Bib_VID(titleRow.BibID, itemRow.VID, null );
            //        Application_State.Multiple_Volume_Item dbTitle = allItems.Title_By_Bib(titleRow.BibID);
            //        if ((dbItem != null) && ( dbTitle != null ))
            //        {
            //            simplified_xml.Add_Row(titleRow.BibID, itemRow.VID, itemRow.Title, current_greenstone_image_root + dbTitle.File_Root + "/" + dbItem.VID_String );
            //        }
            //    }
            //}

            //// Write this information
            //simplified_xml.WriteXml(Output, XmlWriteMode.WriteSchema);
        }

        private static void Write_XML( TextWriter Output, string Value )
        {
            foreach( char thisChar in Value )
            {
                switch ( thisChar )
                {
                    case '>':
                        Output.Write("&gt;");
                        break;

                    case '<':
                        Output.Write("&lt;");
                        break;

                    case '"':
                        Output.Write("&quot;");
                        break;

                    case '&':
                        Output.Write("&amp;");
                        break;

                    default:
                        Output.Write(thisChar);
                        break;
                }
            }
        }
    }
}
