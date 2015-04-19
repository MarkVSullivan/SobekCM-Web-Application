#region Using directives

using System;
using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes search results and item browses as simplified XML to the response stream. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Xml_MainWriter : abstractMainWriter
    {
        /// <summary> Constructor for a new instance of the Xml_MainWriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Xml_MainWriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // All work done in the base constructor
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="Writer_Type_Enum.XML"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.XML; } }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            switch (RequestSpecificValues.Current_Mode.Mode)
            {
                case Display_Mode_Enum.Results:
                case Display_Mode_Enum.Aggregation:
                    if (RequestSpecificValues.Paged_Results != null)
                        display_search_results(Output);
                    break;
				case Display_Mode_Enum.Reports:
					if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Report_Name))
					{
						Output.WriteLine("REPORT REQUESTED: " + RequestSpecificValues.Current_Mode.Report_Name);
					}
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
            string url = UI_ApplicationCache_Gateway.Settings.Image_URL;
            string network = UI_ApplicationCache_Gateway.Settings.Image_Server_Network;
            string base_url = RequestSpecificValues.Current_Mode.Base_URL.Replace("sobekcm_data.aspx", "");

            // Write the header first
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<ResultSet Page=\"" + RequestSpecificValues.Current_Mode.Page + "\" Total=\"" + RequestSpecificValues.Results_Statistics.Total_Titles + "\">");

            // Now, add XML for each title
            string lastBibID = string.Empty;
            foreach (iSearch_Title_Result thisResult in RequestSpecificValues.Paged_Results)
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

            if ( RequestSpecificValues.Paged_Results.Count > 0 )
                Output.WriteLine("</TitleResult>");          
            Output.WriteLine("</ResultSet>");
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
