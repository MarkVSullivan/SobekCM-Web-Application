using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Tools;
using Zen.Barcode;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Tracking sheet item viewer prototyper, which is used to check to see if a user has access to view and print
    /// the tracking sheet associated with a digital resource, and to create the viewer itself if the user selects that option </summary>
    public class TrackingSheet_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the TrackingSheet_ItemViewer_Prototyper class </summary>
        public TrackingSheet_ItemViewer_Prototyper()
        {
            ViewerType = "TRACKING_SHEET";
            ViewerCode = "ts";
        }

        /// <summary> Name of this viewer, which matches the viewer name from the database and 
        /// in the configuration files as well.  This is actually populate by the configuration information </summary>
        public string ViewerType { get; set; }

        /// <summary> Code for this viewer, which can also be set from the configuration information </summary>
        public string ViewerCode { get; set; }

        /// <summary> If this viewer is tied to certain files existing in the digital resource, this lists all the 
        /// possible file extensions this supports (from the configuration file usually) </summary>
        public string[] FileExtensions { get; set; }

        /// <summary> Indicates if the specified item matches the basic requirements for this viewer, or
        /// if this viewer should be ignored for this item </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer really should be included </param>
        /// <returns> TRUE if this viewer should generally be included with this item, otherwise FALSE </returns>
        public bool Include_Viewer(BriefItemInfo CurrentItem)
        {
            // This should always be included (although it won't be accessible or shown to everyone)
            return true;
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> TRUE always, since PDFs should never be shown if an item is checked out </returns>
        public bool Override_On_Checkout(BriefItemInfo CurrentItem)
        {
            return false;
        }

        /// <summary> Flag indicates if the current user has access to this viewer for the item </summary>
        /// <param name="CurrentItem"> Digital resource to see if the current user has correct permissions to use this viewer </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        /// <returns> TRUE if the user has access to use this viewer, otherwise FALSE </returns>
        public bool Has_Access(BriefItemInfo CurrentItem, User_Object CurrentUser, bool IpRestricted)
        {
            // If there is no user (or they aren't logged in) then obviously, they can't edit this
            if ((CurrentUser == null) || (!CurrentUser.LoggedOn))
            {
                return false;
            }

            // If INTERNAL, user has access
            if ((CurrentUser.Is_Host_Admin) || (CurrentUser.Is_System_Admin) || (CurrentUser.Is_Portal_Admin) || (CurrentUser.Is_Internal_User))
                return true;

            // See if this user can edit this item
            bool userCanEditItem = CurrentUser.Can_Edit_This_Item(CurrentItem.BibID, CurrentItem.Type, CurrentItem.Behaviors.Source_Institution_Aggregation, CurrentItem.Behaviors.Holding_Location_Aggregation, CurrentItem.Behaviors.Aggregation_Code_List);
            if (!userCanEditItem)
            {
                // Can't edit, so don't show and return FALSE
                return false;
            }

            // Apparently it can be shown
            return true;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            // Do nothing since this is already handed and added to the menu by the MANAGE MENU item viewer
        }

        /// <summary> Creates and returns the an instance of the <see cref="TrackingSheet_ItemViewer"/> class for showing the
        /// internal tracking sheet for a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="TrackingSheet_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new TrackingSheet_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer );
        }
    }

    /// <summary> Tracking sheet item viewer displays the internal tracking sheet with milestons and barcode for progress
    /// logging for a single online digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class TrackingSheet_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly SobekCM_Item track_item;
        private readonly int itemID;
        private readonly string image_location;
        private readonly string aggregations;
        private readonly string oclc;
        private readonly string aleph;
        private readonly List<string> authors_list;
        private readonly string[] publishers_list;

        /// <summary> Constructor for a new instance of the TrackingSheet_ItemViewer class, used display the internal 
        /// tracking sheet with milestons and barcode for progress logging for a single online digital resource  </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public TrackingSheet_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer )
        {
            // Add the trace
            if (Tracer != null)
                Tracer.Add_Trace("TrackingSheet_ItemViewer.Constructor");

            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            //Assign the current resource object to track_item
            track_item = Current_Object;


            //Get the ItemID for this Item from the database
            itemID = track_item.Web.ItemID;

            //Get aggregation info
            aggregations = track_item.Behaviors.Aggregation_Codes;

            //If no aggregationPermissions present, display "none"
            aggregations = aggregations.Length > 0 ? aggregations.ToUpper() : "(none)";
            aggregations = aggregations.Replace(";", "; ");

            //Determine the OCLC & Aleph number for display
            oclc = track_item.Bib_Info.OCLC_Record;
            aleph = track_item.Bib_Info.ALEPH_Record;

            if (String.IsNullOrEmpty(oclc) || oclc.Trim() == "0" || oclc.Trim() == "1")
                oclc = "(none)";
            if (String.IsNullOrEmpty(aleph) || aleph.Trim() == "0" || aleph.Trim() == "1")
                aleph = "(none)";

            //Determine the author(s) for display
            authors_list = new List<string>();
            int author_startCount = 0;

            //Add the main entity first
            if (track_item.Bib_Info.hasMainEntityName)
            {
                authors_list.Add(track_item.Bib_Info.Main_Entity_Name.Full_Name + track_item.Bib_Info.Main_Entity_Name.Role_String);
                author_startCount++;
            }

            //Now add all other associated creators
            for (int i = author_startCount; i < track_item.Bib_Info.Names_Count; i++)
            {
                //Skip any publishers in this list
                if (track_item.Bib_Info.Names[i].Role_String.ToUpper().Contains("PUBLISHER"))
                    continue;
                authors_list.Add(track_item.Bib_Info.Names[i].Full_Name + track_item.Bib_Info.Names[i].Role_String);
            }

            //Determine the publisher(s) for display
            publishers_list = new string[track_item.Bib_Info.Publishers_Count];
            for (int i = 0; i < track_item.Bib_Info.Publishers_Count; i++)
                publishers_list[i] = track_item.Bib_Info.Publishers[i].Name;


            //Create the temporary location for saving the barcode images
            image_location = UI_ApplicationCache_Gateway.Settings.Servers.Base_Temporary_Directory + "tsBarcodes\\" + itemID.ToString();


            // Create the folder for the user in the temp directory
            if (!Directory.Exists(image_location))
                Directory.CreateDirectory(image_location);
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkDiv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkDiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("TrackingSheet_ItemViewer.Write_Main_Viewer_Section", "");
            }
            if (Tracer != null)
            {
                Tracer.Add_Trace("TrackingSheet_ItemViewer.Write_Main_Viewer_Section", "");
            }

            Output.WriteLine("\t\t<!-- TRACKING SHEET VIEWER OUTPUT -->");

            #region Variable definitions

            //Spaces to use as empty fields in the tables
            const string LABEL_SPACE = "&nbsp;&nbsp;&nbsp;";
            const string LABEL1_SPACE = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";


            #endregion

            //Start the outer main table
            Output.WriteLine("<table class=\"sbkTs_MainTable\"><tr><td>");

            //Add the Bib, VID and TrackingBox numbers to the title
            Output.WriteLine("<span class = \"sbkTs_Title\">" + track_item.BibID + " : " + track_item.VID + "</span>");
            Output.WriteLine("<span class=\"sbkTs_Title_right\">" + track_item.Tracking.Tracking_Box + "</span>");

            //Start the Material Information Box
            Output.WriteLine("<table class=\"sbkTs_tblMaterialInfo\"><col width=\"40\">");
            Output.WriteLine("<tr><td colspan=\"4\"><br/></td></tr>");
            Output.WriteLine("<tr><td colspan=\"4\"><span class=\"sbkTs_tableHeader\">Material Information</span></td></tr>");

            //Add the title
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Title:</span></td>");
            Output.WriteLine("<td colspan=\"3\"><span>" + track_item.Bib_Info.Main_Title + "</span></td></tr>");

            //Add the Author information
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Author:</span></td>");
            Output.WriteLine("<td colspan=\"3\" ><span>");
            if (authors_list.Count == 1)
            {
                Output.WriteLine(authors_list[0]);
            }
            else
            {
                Output.WriteLine("<table>");
                int count = 1;
                string extra_authors = String.Empty;
                foreach (string author in authors_list)
                {
                    if (count == 5 && authors_list.Count > 5)
                        extra_authors = "<span class=\"sbkTs_extraText\"> " + LABEL_SPACE + "  ...+" + (authors_list.Count - 5) + " more</span>";
                    Output.WriteLine("<tr><td>" + author + extra_authors + "</td></tr>");
                    count++;
                    //Limit the display to 5
                    if (count > 5)
                        break;
                }
                Output.WriteLine("</table>");
            }
            Output.WriteLine("               </span></td>");
            Output.WriteLine("</td></tr>");

            //Add the publisher information
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Publisher:</span></td>");
            Output.WriteLine("<td colspan=\"3\" ><span>");

            if (publishers_list.Length == 1)
            {
                Output.WriteLine(publishers_list[0]);
            }
            else
            {
                Output.WriteLine("<table>");
                int count = 1;
                string extra_publishers = String.Empty;
                foreach (string publisher in publishers_list)
                {
                    if (count == 5 && publishers_list.Length > 5)
                        extra_publishers = "<span class=\"sbkTs_extraText\">  " + LABEL_SPACE + "...+" + (publishers_list.Length - 5) + " more</span>";
                    Output.WriteLine("<tr><td>" + publisher + extra_publishers + "</td></tr>");
                    count++;
                    //Limit the display to 5
                    if (count > 5)
                        break;
                }
                Output.WriteLine("</table>");
            }

            Output.WriteLine("               </span></td>");
            Output.WriteLine("</td></tr>");

            //Add the OCLC, Aleph, Material Type information
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">OCLC:</span></td>");
            Output.WriteLine("<td><span>" + oclc + "</span></td>");
            Output.WriteLine("<td><span class=\"sbkTs_tableLabel\"> Aleph:</span>");
            Output.WriteLine("<span>" + aleph + "</span></td>");
            Output.WriteLine("<td> <span class=\"sbkTs_tableLabel\">  Material Type:</span>");
            Output.WriteLine("<span>" + track_item.Bib_Info.SobekCM_Type + "</span></td></tr>");

            //Add the aggregation information
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Aggregations:</span></td>");
            Output.WriteLine("<td colspan=\"3\"><span>" + aggregations + "</span></td></tr>");

            //End this table
            Output.WriteLine("</table>");
            Output.WriteLine("<br/>");

            //Write the serial hierarchy info, if there is any data available
            if (track_item.Behaviors.hasSerialInformation)
            {
                //Start the Serial Hierarchy Table
                Output.WriteLine("<table class=\"sbkTs_tblSerialHierarchy\">");
                Output.WriteLine("<tr><td><span class=\"sbkTs_tableHeader\">Serial Hierarchy</span></td></tr>");

                for (int i = 0; i < track_item.Behaviors.Serial_Info.Count; i++)
                {
                    Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Level " + (i + 1).ToString() + ":" + LABEL_SPACE + "</span>");
                    Output.WriteLine(track_item.Behaviors.Serial_Info[i].Display + "</td></tr>");

                }


                //End this table
                Output.WriteLine("</table>");
            }
            Output.WriteLine("<br/>");

            //Start the Imaging Progress Table
            Output.WriteLine("<table class=\"sbkTs_tblImagingProgress\">");
            Output.WriteLine("<tr><td colspan=\"8\"><span class=\"sbkTs_tableHeader\">Imaging Progress</span></td></tr>");


            for (int rowCount = 0; rowCount < 2; rowCount++)
            {
                Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Name:</span></td><td class=\"sbkTs_tblCellUnderline\">" + LABEL_SPACE + LABEL_SPACE + LABEL1_SPACE + LABEL1_SPACE + LABEL1_SPACE + "</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Date:</span></td><td class=\"sbkTs_tblCellUnderline\">" + LABEL_SPACE + LABEL_SPACE + "/" + LABEL_SPACE + LABEL_SPACE + " / " + LABEL_SPACE + "</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Page Range:</span></td><td class=\"sbkTs_tblCellUnderline\">" + LABEL_SPACE + LABEL_SPACE + LABEL_SPACE + "</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Duration:</span></td><td class=\"sbkTs_tblCellUnderline\">" + LABEL_SPACE + LABEL_SPACE + LABEL_SPACE + "</td></tr>");
            }
            Output.WriteLine("</table>");
            Output.WriteLine("<br/><br/>");


            string checked_html = String.Empty;
            if (track_item.Tracking.Born_Digital)
                checked_html = "checked=\"checked\"";

            //Determine the Material & Disposition text for display
            string materialRecd_text = "&nbsp;&nbsp;&nbsp;";
            string materialRecd_class = "sbkTs_tblCellUnderline";
            string disposition_text = "&nbsp;&nbsp;&nbsp;";
            string disposition_class = "sbkTs_tblCellUnderline";

            if (!String.IsNullOrEmpty(track_item.Tracking.Material_Received_Date.ToString()))
            {
                materialRecd_class = "";
                materialRecd_text = track_item.Tracking.Material_Received_Date.ToString();
                if (!String.IsNullOrEmpty(track_item.Tracking.Material_Received_Date.ToString()) && !String.IsNullOrEmpty(track_item.Tracking.Material_Received_Notes))
                {
                    materialRecd_text += " (" + track_item.Tracking.Material_Received_Notes + ")";
                }
            }

            if (!String.IsNullOrEmpty(track_item.Tracking.Disposition_Advice_Notes))
            {
                disposition_class = "";
                disposition_text = track_item.Tracking.Disposition_Advice_Notes;
                if (!String.IsNullOrEmpty(track_item.Tracking.Disposition_Notes) && !String.IsNullOrEmpty(track_item.Tracking.Disposition_Notes))
                {
                    disposition_text += " (" + track_item.Tracking.Disposition_Notes + ")";
                }
            }

            //Add the Physical Material Info 
            Output.WriteLine("<table class=\"sbkTs_tblPhysicalMaterial\"><col width=\"130\">");
            Output.WriteLine("<tr><td colspan=\"2\"><span class=\"sbkTs_tableHeader\">Physical Material</span>" + LABEL_SPACE + LABEL_SPACE + "<input type=\"checkbox\" disabled=\"true\" " + checked_html + "/><span class=\"sbkTs_greyText\">Item is born digital</span></td></tr>");
            Output.WriteLine("<tr><td width=\"auto\"><span class=\"sbkTs_tableLabel\">Material Recd:</span></td>");
            Output.WriteLine("         <td class=\"" + materialRecd_class + "\">" + materialRecd_text + "</td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("<tr><td width=\"auto\"><span class=\"sbkTs_tableLabel\">Disposition Advice:</span></td>");
            Output.WriteLine("         <td class=\"" + disposition_class + "\">" + disposition_text + "</td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br/>");


            //Add the Additional Notes table
            Output.WriteLine("<table class=\"sbkTs_tblAdditionalNotes\">");
            Output.WriteLine("<tr><td class=\"sbkTs_tableHeader\">Additional Notes:</td></tr>");
            Output.WriteLine("<tr><td  class=\"sbkTs_tblCellUnderline\">&nbsp;&nbsp;&nbsp;&nbsp; </td></tr>");
            Output.WriteLine("<tr><td  class=\"sbkTs_tblCellUnderline\">&nbsp;&nbsp;&nbsp;&nbsp; </td></tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br/>");

            //Get the barcode images for the events
            string imageUrl1 = Get_BarcodeImageUrl_from_string(itemID, "1", track_item.BibID + track_item.VID + "1");
            string imageUrl2 = Get_BarcodeImageUrl_from_string(itemID, "2", track_item.BibID + track_item.VID + "2");
            string imageUrl3 = Get_BarcodeImageUrl_from_string(itemID, "3", track_item.BibID + track_item.VID + "3");
            string imageUrl4 = Get_BarcodeImageUrl_from_string(itemID, "4", track_item.BibID + track_item.VID + "4");

            //Start the table for the barcodes
            Output.WriteLine("<table class=\"sbkTs_tblBarcodes\">");
            Output.WriteLine("  <tr>");
            Output.WriteLine("     <td><img id=\"barcode1\" src=\"" + imageUrl1 + "\"/></td>");
            Output.WriteLine("     <td><img id=\"barcode2\" src=\"" + imageUrl2 + "\"/></td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("  <tr>");
            Output.WriteLine("     <td><span class=\"sbkTs_barcode_label1\">Start Scanning</span></td>");
            Output.WriteLine("     <td><span class=\"sbkTs_barcode_label1\">End Scanning</span></td>");
            Output.WriteLine("  </tr>");

            Output.WriteLine(" <tr>");
            Output.WriteLine("     <td><img id=\"barcode3\" src=\"" + imageUrl3 + "\"/></td>");
            Output.WriteLine("     <td><img id=\"barcode4\" src=\"" + imageUrl4 + "\"/></td>");
            Output.WriteLine(" </tr>");

            Output.WriteLine(" <tr>");
            Output.WriteLine("     <td><span class=\"sbkTs_barcode_label\">Start Processing</span></td>");
            Output.WriteLine("     <td><span class=\"sbkTs_barcode_label\">End Processing</span></td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("</table>");

            //Close the outer table
            Output.WriteLine("</td></tr></table>");

        }

        /// <summary> Converts a base-10 integer to the base-26 equivalent </summary>
        /// <param name="InputNumber"> Integer to convert to base-26 equivalent </param>
        /// <returns> Base-26 equivalent, utilizing the alphabet to encode the number </returns>
        public string int_to_base26(int InputNumber)
        {
            string convertedNumber = String.Empty;

            InputNumber = Math.Abs(InputNumber);

            do
            {
                int remainder = InputNumber % 26;
                convertedNumber = (char)(remainder + 'A') + convertedNumber;
                InputNumber = (InputNumber - remainder) / 26;
            } while (InputNumber > 0);

            return convertedNumber;
        }


        /// <summary> Generates a barcode(with checksum) for a given string </summary>
        /// <param name="ItemID"> ItemID for the material </param>
        /// <param name="Action"> Indicates the Imaging action represented by this barcode</param>
        /// <param name="FilenameToSave"> Name of the output file to save </param>
        /// <returns>The url of the generated barcode GIF image</returns>
        public string Get_BarcodeImageUrl_from_string(int ItemID, string Action, string FilenameToSave)
        {
            string convertedItemID = int_to_base26(ItemID);
            string inputString = (convertedItemID + Action).ToUpper();
            if (inputString == null) throw new ArgumentNullException("InputString");

            //Calculate the checksum
            int actionNum;
            Int32.TryParse(Action, out actionNum);
            int checksumInt = (ItemID + actionNum) % 26;
            string checksum = int_to_base26(checksumInt).ToUpper();

            string barcodeString = inputString + checksum;

            string image_save_location = image_location + "\\" + FilenameToSave + ".gif";

            //Generate the image
            Code39BarcodeDraw barcode39 = BarcodeDrawFactory.Code39WithoutChecksum;
            System.Drawing.Image barcode_image = barcode39.Draw(barcodeString, 60);

            //Save the image
            barcode_image.Save(@image_save_location, ImageFormat.Gif);

            string url = CurrentRequest.Base_URL + "temp/" + "tsBarcodes/" + ItemID;

            return url + "/" + FilenameToSave + ".gif";

        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Static_Resources.Sobekcm_Trackingsheet_Css + "\" /> ");
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum> 
                    {
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Footer,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Header,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Banner,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Titlebar
                    };
            }
        }

        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This method does nothing, since nothing is added to the place holder as a control for this item viewer </remarks>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Do nothing
        }
    }
}
