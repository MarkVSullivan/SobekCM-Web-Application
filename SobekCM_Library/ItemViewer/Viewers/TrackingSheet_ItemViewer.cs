#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Tools;
using SobekCM.UI_Library;
using Zen.Barcode;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    public class TrackingSheet_ItemViewer : abstractItemViewer
    {
        private readonly SobekCM_Item track_item;
        private readonly int itemID;
        private readonly string image_location;
        private readonly string aggregations;
        private readonly string oclc;
        private readonly string aleph;
        private readonly string username;
        private readonly List<string> authors_list;
        private readonly string[] publishers_list;

    
        /// <summary> Constructor for the Tracking Sheet ItemViewer </summary>
        /// <param name="Current_Object"></param>
        /// <param name="Current_User"></param>
        /// <param name="Current_Mode"></param>
        public TrackingSheet_ItemViewer(SobekCM_Item Current_Object, User_Object Current_User, SobekCM_Navigation_Object Current_Mode)
        {
            CurrentMode = Current_Mode;
            CurrentUser = Current_User;
            
            //Assign the current resource object to track_item
            track_item = Current_Object;
            

            //Get the ItemID for this Item from the database
            itemID = track_item.Web.ItemID;

            //Get aggregation info
            aggregations = track_item.Behaviors.Aggregation_Codes;

            //If no aggregationPermissions present, display "none"
            aggregations = aggregations.Length>0 ? aggregations.ToUpper() : "(none)";
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
                if(track_item.Bib_Info.Names[i].Role_String.ToUpper().Contains("PUBLISHER"))
                    continue;
                authors_list.Add(track_item.Bib_Info.Names[i].Full_Name  + track_item.Bib_Info.Names[i].Role_String);
            }
            
            //Determine the publisher(s) for display
            publishers_list=new string[track_item.Bib_Info.Publishers_Count];
            for (int i = 0; i < track_item.Bib_Info.Publishers_Count; i++)
                publishers_list[i] = track_item.Bib_Info.Publishers[i].Name;
                

            //Create the temporary location for saving the barcode images
           image_location = UI_ApplicationCache_Gateway.Settings.Base_Temporary_Directory + "tsBarcodes\\" + itemID.ToString();


            // Create the folder for the user in the temp directory
            if (!Directory.Exists(image_location))
                Directory.CreateDirectory(image_location);

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


        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
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
                        extra_authors = "<span class=\"sbkTs_extraText\"> "+LABEL_SPACE+"  ...+" + (authors_list.Count - 5) + " more</span>";
                    Output.WriteLine("<tr><td>" + author + extra_authors+ "</td></tr>");
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
                    if(count==5 && publishers_list.Length>5)
                        extra_publishers = "<span class=\"sbkTs_extraText\">  " + LABEL_SPACE + "...+" + (publishers_list.Length - 5) + " more</span>";
                    Output.WriteLine("<tr><td>" + publisher + extra_publishers+"</td></tr>");
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
            Output.WriteLine("<span>" +track_item.Bib_Info.SobekCM_Type + "</span></td></tr>");

            //Add the aggregation information
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Aggregations:</span></td>");
            Output.WriteLine("<td colspan=\"3\"><span>" + aggregations + "</span></td></tr>");

            //End this table
            Output.WriteLine("</table>");
            Output.WriteLine("<br/>");

            //Write the serial hierarchy info, if there is any data available
            if(track_item.Behaviors.hasSerialInformation)
            {
                //Start the Serial Hierarchy Table
                Output.WriteLine("<table class=\"sbkTs_tblSerialHierarchy\">");
                Output.WriteLine("<tr><td><span class=\"sbkTs_tableHeader\">Serial Hierarchy</span></td></tr>");

                for (int i = 0; i < track_item.Behaviors.Serial_Info.Count; i++)
                {
                    Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Level "+(i+1).ToString()+":" + LABEL_SPACE + "</span>");
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
                Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Name:</span></td><td class=\"sbkTs_tblCellUnderline\">" + LABEL_SPACE+LABEL_SPACE+LABEL1_SPACE + LABEL1_SPACE + LABEL1_SPACE + "</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Date:</span></td><td class=\"sbkTs_tblCellUnderline\">" + LABEL_SPACE + LABEL_SPACE + "/" + LABEL_SPACE + LABEL_SPACE + " / " + LABEL_SPACE + "</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Page Range:</span></td><td class=\"sbkTs_tblCellUnderline\">"+LABEL_SPACE+LABEL_SPACE+LABEL_SPACE +"</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Duration:</span></td><td class=\"sbkTs_tblCellUnderline\">" + LABEL_SPACE +LABEL_SPACE+LABEL_SPACE+ "</td></tr>");
            }
            Output.WriteLine("</table>");
            Output.WriteLine("<br/><br/>");


            string checked_html = String.Empty;
          if(track_item.Tracking.Born_Digital)
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
                    materialRecd_text += " (" +track_item.Tracking.Material_Received_Notes + ")";
                }
            }
            
            if (!String.IsNullOrEmpty(track_item.Tracking.Disposition_Advice_Notes))
            {
                disposition_class = "";
                disposition_text = track_item.Tracking.Disposition_Advice_Notes;
                if (!String.IsNullOrEmpty(track_item.Tracking.Disposition_Notes) && !String.IsNullOrEmpty(track_item.Tracking.Disposition_Notes))
                {
                    disposition_text += " (" +track_item.Tracking.Disposition_Notes + ")";
                }
            }

            //Add the Physical Material Info 
            Output.WriteLine("<table class=\"sbkTs_tblPhysicalMaterial\"><col width=\"130\">");
            Output.WriteLine("<tr><td colspan=\"2\"><span class=\"sbkTs_tableHeader\">Physical Material</span>" + LABEL_SPACE + LABEL_SPACE+"<input type=\"checkbox\" disabled=\"true\" "+checked_html+"/><span class=\"sbkTs_greyText\">Item is born digital</span></td></tr>");
            Output.WriteLine("<tr><td width=\"auto\"><span class=\"sbkTs_tableLabel\">Material Recd:</span></td>");
            Output.WriteLine("         <td class=\""+materialRecd_class+"\">"+materialRecd_text+"</td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("<tr><td width=\"auto\"><span class=\"sbkTs_tableLabel\">Disposition Advice:</span></td>");
            Output.WriteLine("         <td class=\""+disposition_class+"\">"+disposition_text+"</td>");
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
        /// <param name="InputString"> This is the itemID, in base 26 number system</param>
		/// <param name="Action"> Indicates the Imaging action represented by this barcode</param>
        /// <param name="FilenameToSave"> Name of the output file to save </param>
        /// <returns>The url of the generated barcode GIF image</returns>
        public string Get_BarcodeImageUrl_from_string(int itemID, string Action, string FilenameToSave)
        {
            string convertedItemID = int_to_base26(itemID);
            string InputString = (convertedItemID + Action).ToUpper();
	        if (InputString == null) throw new ArgumentNullException("InputString");

            //Calculate the checksum
            int actionNum = 0;
            Int32.TryParse(Action, out actionNum);
            int checksumInt = (itemID + actionNum)%26;
            string checksum = int_to_base26(checksumInt).ToUpper();
			
            string barcodeString = InputString + checksum;
          
            string image_save_location = image_location + "\\" + FilenameToSave + ".gif";

            //Generate the image
           Code39BarcodeDraw barcode39 = BarcodeDrawFactory.Code39WithoutChecksum;
            Image barcode_image = barcode39.Draw(barcodeString, 60);
            
            //Save the image
            barcode_image.Save(@image_save_location, ImageFormat.Gif);
	       // string url = CurrentMode.Base_URL + "temp/" + CurrentUser.UserName.Replace(".", "").Replace("@", "") + "/tsBarcodes/" + itemID.ToString();
            string url = CurrentMode.Base_URL + "temp/" + "tsBarcodes/" + itemID.ToString();

			return url + "/" + FilenameToSave + ".gif";

        }



        /// <summary> Gets the number of pages for this viewer </summary>
        /// <remarks> Always returns 1</remarks>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Tracking_Sheet"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Tracking_Sheet; }
        }


        /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <remarks> This always returns the value TRUE for this viewer </remarks>
        public override bool Override_On_Checked_Out
        {
            get
            {
                return true;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value -1</value>
        public override int Viewer_Width
        {
            get
            {
                return -1;
            }
        }

        /// <summary> Height for the main viewer section to adjusted to accomodate this viewer</summary>
        public override int Viewer_Height
        {
            get
            {
                return -1;
            }
        }

    }
}
