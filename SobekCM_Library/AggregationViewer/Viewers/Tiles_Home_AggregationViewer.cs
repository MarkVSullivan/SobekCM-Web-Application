using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.Aggregations;
using SobekCM.Library.HTML;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Tools;

namespace SobekCM.Library.AggregationViewer.Viewers
{
    public class Tiles_Home_Single_Tile
    {
        public string JpegUri { get; set; }

        public string LinkUri { get; set; }
    }

    public class Tiles_Home_AggregationViewer : abstractAggregationViewer
    {
        protected List<Tiles_Home_Single_Tile> selectedTiles = new List<Tiles_Home_Single_Tile>();

        public Tiles_Home_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag) : base(RequestSpecificValues, ViewBag)
        {
            // Get the list of tiles
            string aggregation_tile_directory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location, ViewBag.Hierarchy_Object.ObjDirectory, "images", "tiles");
            string[] jpeg_tiles = Directory.GetFiles(aggregation_tile_directory, "*.jpg");

            // Compute the URL for these images
            string aggregation_tile_uri = RequestSpecificValues.Current_Mode.Base_URL + "design/aggregations/" + ViewBag.Hierarchy_Object.Code + "/images/tiles/";

            // Get the list of all potential tiles, by checking name
            List<Tiles_Home_Single_Tile> allTiles = new List<Tiles_Home_Single_Tile>();
            foreach (string thisJpegTile in jpeg_tiles)
            {
                // Get the filename
                string thisFileName = Path.GetFileName(thisJpegTile);
                string thisFileNameSansExtension = Path.GetFileNameWithoutExtension(thisJpegTile);

                // Check for a link to the bibid/vid
                string bib_vid_for_link = String.Empty;
                if ((thisFileNameSansExtension.Length == 10) && ( SobekCM_Item.is_bibid_format(thisFileNameSansExtension)))
                {
                    bib_vid_for_link = thisFileNameSansExtension;
                }
                else if ((thisFileNameSansExtension.Length == 15) && (SobekCM_Item.is_bibid_format(thisFileNameSansExtension.Substring(0, 10))) && (SobekCM_Item.is_vids_format(thisFileNameSansExtension.Substring(10))))
                {
                    bib_vid_for_link = thisFileNameSansExtension.Substring(0, 10) + "/" + thisFileNameSansExtension.Substring(10);                 
                }
                else if ((thisFileNameSansExtension.Length == 16) && (SobekCM_Item.is_bibid_format(thisFileNameSansExtension.Substring(0, 10))) && (SobekCM_Item.is_vids_format(thisFileNameSansExtension.Substring(11))))
                {
                    bib_vid_for_link = thisFileNameSansExtension.Substring(0, 10) + "/" + thisFileNameSansExtension.Substring(11);
                }

                // If there was a link calculated, then use this jpeg
                if (!String.IsNullOrEmpty(bib_vid_for_link))
                {
                    allTiles.Add(new Tiles_Home_Single_Tile {JpegUri = aggregation_tile_uri + thisFileName, LinkUri = RequestSpecificValues.Current_Mode.Base_URL + bib_vid_for_link});
                }
            }

            // For now, just copy over all the tiles
            if ( allTiles.Count <= 15 )
                selectedTiles.AddRange(allTiles);
            else
            {
                Random randomGen = new Random();
                while (selectedTiles.Count < 15)
                {
                    int random_index = randomGen.Next(0, allTiles.Count);
                    selectedTiles.Add(allTiles[random_index]);
                    allTiles.RemoveAt(random_index);
                }
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.Custom_Home_Page"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Custom_Home_Page; }
        }

        /// <summary> Gets the collection of special behaviors which this aggregation viewer  requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> AggregationViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
                        {
                            HtmlSubwriter_Behaviors_Enum.Aggregation_Suppress_Home_Text,
                            HtmlSubwriter_Behaviors_Enum.Suppress_SearchForm
                        };
            }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // do nothing
        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the search tips by calling the base method <see cref="abstractAggregationViewer.Add_Simple_Search_Tips"/> </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Tiles_Home_AggregationViewer.Add_Secondary_HTML", "Add the entire tiled home page");
            }

            Output.Write("<div id=\"sbkThav_TileContainer\">");

            foreach (Tiles_Home_Single_Tile thisTile in selectedTiles)
            {
                Output.Write("  <div class=\"sbkThav_Tile\">");
                Output.Write("    <a href=\"" + thisTile.LinkUri + "\">");
                Output.Write("      <img src=\"" + thisTile.JpegUri + "\" />");
                Output.Write("    </a>");
                Output.Write("  </div>");
            }

            Output.Write("</div>");

            // If there are sub aggregations here, show them
            if (ViewBag.Hierarchy_Object.Children_Count > 0)
            {
                Output.WriteLine("<div class=\"SobekText\">");
                Aggregation_HtmlSubwriter.Add_SubCollection_Buttons(Output, RequestSpecificValues, ViewBag.Hierarchy_Object);
                Output.WriteLine("</div>");
            }
            RequestSpecificValues.Current_Mode.Aggregation = ViewBag.Hierarchy_Object.Code;

        }
    }
}
