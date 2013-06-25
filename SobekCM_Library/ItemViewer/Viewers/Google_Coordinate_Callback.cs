using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using SobekCM.Library.HTML;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SobekCM.Library.ItemViewer.Viewers
{

    public class Google_Coordinate_Callback : System.Web.UI.Page, System.Web.UI.ICallbackEventHandler
    {
        protected string stockItemCoord = String.Empty;
        protected string stockItemDesc = String.Empty;
        protected string savedItemCoord = String.Empty;
        protected string savedItemDesc = String.Empty;


        public string stockOverlayBounds = String.Empty;
        public string stockOverlaySource = String.Empty;
        public double stockOverlayRotation = 0;
        protected string savedOverlayBounds = String.Empty;
        protected string savedOverlaySource = String.Empty;
        protected string savedOverlayRotation = String.Empty; //recieving 
        protected double savedOverlayRotationSending = 0; //sending

        protected string callbackMessage = "0";


        protected void Page_Load(object sender, EventArgs e)
        {
            //read the goodies at get go
            //ReadSavedItem();
            //ReadStockItem();
            //ReadSavedOverlay();
            //ReadStockOverlay();

            //Get the Page's ClientScript and assign it to a ClientScriptManger
            ClientScriptManager cm = Page.ClientScript;

            //Generate the callback reference
            string cbReference = cm.GetCallbackEventReference(this, "arg", "HandleResult", "");

            //Build the callback script block
            string cbScript = "function CallServer(arg, context){" + cbReference + ";}";

            //Register the block
            cm.RegisterClientScriptBlock(this.GetType(), "CallServer", cbScript, true);

        }


        public void RaiseCallbackEvent(string eventArgument)
        {

            //StreamWriter output3 = new StreamWriter("U:/vs12_projects/m3/output/savedPOISet.txt", true);
            //output3.WriteLine(eventArgument); //poi kml
            //output3.Flush();
            //output3.Close();

            //This method will be called by the Client; Do your business logic here
            //The parameter "eventArgument" is actually the paramenter "arg" of CallServer(arg, context)

            int packageIndex = eventArgument.LastIndexOf("~");
            string[] packages = eventArgument.Substring(0, packageIndex).Split('~');

            //for (var i = 0; i < ar1.length; i++) {
            foreach (var pack in packages)
            {
                int packIndex = pack.LastIndexOf("|");
                string[] packs = pack.Substring(0, packIndex).Split('|');

                string saveType = packs[0]; //get what type of save it is

                switch (saveType)
                {

                    case "item":
                        savedItemCoord = packs[1];
                        StreamWriter output1 = new StreamWriter("U:/vs12_projects/m3/output/savedItem.txt", false);
                        output1.WriteLine(savedItemCoord);
                        output1.Flush();
                        output1.Close();
                        ReadSavedItem(); //trigger a refresh of cached saved item
                        callbackMessage = "1";
                        break;
                    case "overlay":
                        savedOverlayBounds = packs[1];
                        savedOverlaySource = packs[2];
                        savedOverlayRotation = packs[3];
                        //savedItem = eventArgument; //not used
                        StreamWriter output2 = new StreamWriter("U:/vs12_projects/m3/output/savedOverlay.txt", false);
                        output2.WriteLine(savedOverlayBounds);
                        output2.WriteLine(savedOverlaySource);
                        output2.WriteLine(savedOverlayRotation);
                        output2.Flush();
                        output2.Close();
                        ReadSavedOverlay(); //trigger a refresh of cached saved overlay
                        callbackMessage = "2";
                        break;
                    case "poi":
                        StreamWriter output3 = new StreamWriter("U:/vs12_projects/m3/output/savedPOISet.txt", true);
                        string stuff = DateTime.Now + "," + packs[1] + "," + packs[2] + ",\"" + packs[3] + "\"";
                        output3.WriteLine(stuff); //poi kml
                        output3.Flush();
                        output3.Close();
                        //ReadSavedPOI(); //trigger a refresh of cached saved overlay
                        callbackMessage = "3";
                        break;
                }

            }

            //GetCallbackResult(); //trigger callback
        }

        public string GetCallbackResult()
        {


            //This is called after RaiseCallbackEvent and then sent to the client which is the
            //function "HandleResult" in javascript of the page

            //reread everything
            //ReadStockItem();
            //ReadSavedItem();
            //ReadStockOverlay();
            //ReadSavedOverlay();
            //ReadStockPOI();
            //ReadSavedPOI();

            return callbackMessage;
        }

        //unknown if I can delete these????
        public string stockMarkerCoords = String.Empty;
        public string stockMarkerDesc = String.Empty;


        //read saved item from file
        public void ReadSavedItem()
        {
            string[] lines = File.ReadAllLines(@"U:/vs12_projects/m3/output/savedItem.txt");

            savedItemCoord = lines[0];
        }

        //read stock Item from file
        public void ReadStockItem()
        {
            string[] lines = File.ReadAllLines(@"U:/vs12_projects/m3/output/stockItem.txt");

            stockItemCoord = lines[0];
        }

        //read saved overlay from file
        public void ReadSavedOverlay()
        {
            string[] lines = File.ReadAllLines(@"U:/vs12_projects/m3/output/savedOverlay.txt");

            savedOverlayBounds = lines[0];
            savedOverlaySource = lines[1];
            savedOverlayRotationSending = Convert.ToDouble(lines[2]);
        }

        //read stock overlay from file
        public void ReadStockOverlay()
        {
            string[] lines = File.ReadAllLines(@"U:/vs12_projects/m3/output/stockOverlay.txt");

            stockOverlayBounds = lines[0];
            stockOverlaySource = lines[1];
            stockOverlayRotation = Convert.ToDouble(lines[2]);
        }
    }
    
}