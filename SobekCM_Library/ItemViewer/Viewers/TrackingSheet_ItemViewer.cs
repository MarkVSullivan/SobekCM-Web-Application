#region Using directives
using System;
using SobekCM.Library.Settings;
using Zen.Barcode;
using System.Collections.Generic;
using System.IO;
using System.Data;
using SobekCM.Library.HTML;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;
using SobekCM.Resource_Object;
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
        private readonly string[] authors_list;
        private readonly string[] publishers_list;

    
        /// <summary> Constructor for the Tracking Sheet ItemViewer </summary>
        /// <param name="Current_Object"></param>
        /// <param name="Current_User"></param>
        /// <param name="Current_Mode"></param>
        public TrackingSheet_ItemViewer(SobekCM_Item Current_Object, User_Object Current_User, SobekCM_Navigation_Object Current_Mode)
        {
            CurrentMode = Current_Mode;
            CurrentUser = Current_User;
            
            // If there is no user, send to the login
            if (CurrentUser == null)
            {
                CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
                CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                CurrentMode.Redirect();
                return;
            }

            // If the user cannot edit this item, go back
            if (!CurrentUser.Can_Edit_This_Item(Current_Object))
            {
                CurrentMode.ViewerCode = String.Empty;
                CurrentMode.Redirect();
                return;
            }

            //Assign the current resource object to track_item
            track_item = Current_Object;
            

            //Get the ItemID for this Item from the database
            itemID = track_item.Web.ItemID;

            //Get aggregation info
            aggregations = track_item.Behaviors.Aggregation_Codes;

            //If no aggregations present, display "none"
            aggregations = aggregations.Length>0 ? aggregations.ToUpper() : "(none)";

            //Determine the OCLC & Aleph number for display
            oclc = track_item.Bib_Info.OCLC_Record;
            aleph = track_item.Bib_Info.ALEPH_Record;
           
            if (String.IsNullOrEmpty(oclc) || oclc.Trim() == "0" || oclc.Trim() == "1")
                oclc = "(none)";
            if (String.IsNullOrEmpty(aleph) || aleph.Trim() == "0" || aleph.Trim() == "1")
                aleph = "(none)";

            //Determine the author(s) for display
            authors_list = new string[track_item.Bib_Info.Names_Count];
            for (int i = 0; i < track_item.Bib_Info.Names_Count; i++)
            {
                authors_list[i] = track_item.Bib_Info.Names[i].Full_Name;
            }
            
            //Determine the publisher(s) for display
            publishers_list=new string[track_item.Bib_Info.Publishers_Count];
            for (int i = 0; i < track_item.Bib_Info.Publishers_Count; i++)
                publishers_list[i] = track_item.Bib_Info.Publishers[i].Name;
           
                

                //Create the temporary location for saving the barcode images
            image_location = SobekCM_Library_Settings.Base_Temporary_Directory + Current_User.UserName.Replace(".", "").Replace("@", "") + "\\tsBarcodes\\" + itemID.ToString();
            username = Current_User.UserName.Replace(".", "").Replace("@", "");
            if (Current_User.UFID.Trim().Length > 0)
            {
                image_location = SobekCM_Library_Settings.Base_Temporary_Directory + "\\" + Current_User.UFID + "\\tsBarcodes\\" + itemID.ToString();
                username = Current_User.UFID;
            }

            // Make the folder for the user in the temp directory
            if (!Directory.Exists(image_location))
                Directory.CreateDirectory(image_location);

        }


        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_TrackingSheet.css\" /> ");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/scrollbars.css\" />");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/scrollbars-black.css\" />");

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

            //Add the Bib, VID, and TrackingBox numbers to the title
            Output.WriteLine("<span class = \"sbkTs_Title\">" + track_item.BibID + " : " + track_item.VID + "</span>");
            Output.WriteLine("<span class=\"sbkTs_Title_right\">" + track_item.Tracking.Tracking_Box + "</span>");
            
            //Start the Material Information Box
            Output.WriteLine("<table class=\"sbkTs_tblMaterialInfo\"><col width=\"40\">");
            Output.WriteLine("<tr><td colspan=\"4\"><br/></td></tr>");
            Output.WriteLine("<tr><td colspan=\"4\"><span class=\"sbkTs_tableHeader\">Material Information</span></td></tr>");

            //Add the title
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Title:</span></td>");
            Output.WriteLine("<td colspan=\"3\"><span>" + track_item.Bib_Info.Main_Title + "</span></td></tr>");

            //Add the Author info
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Author:</span></td>");
            Output.WriteLine("<td colspan=\"3\" ><span>");
            foreach (string author in authors_list)
            {
                if (authors_list[authors_list.Length - 1] == author)
                    continue;
                Output.WriteLine("<span>"+author+" ;&nbsp;</span>");
            }
            if(authors_list.Length>1)
            Output.WriteLine("<span>" + authors_list[authors_list.Length-1] + " &nbsp;</span>");
            Output.WriteLine("               </span></td>");
            Output.WriteLine("</td></tr>");

            //Add the publisher info
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Publisher:</span></td>");
            Output.WriteLine("<td colspan=\"3\" ><span>");
            foreach (string publisher in publishers_list)
            {
                if (publishers_list[publishers_list.Length - 1] == publisher)
                    continue;
                Output.WriteLine("<span>" + publisher + " ;&nbsp;</span>");
            }
            if(publishers_list.Length>0)
            Output.WriteLine("<span>" + publishers_list[publishers_list.Length - 1] + " &nbsp;</span>");
            Output.WriteLine("               </span></td>");
            Output.WriteLine("</td></tr>");

            //Add the OCLC, Aleph, Material Type info
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">OCLC:</span></td>");
            Output.WriteLine("<td><span>" + oclc + "</span></td>");
            Output.WriteLine("<td><span class=\"sbkTs_tableLabel\"> Aleph:</span>");
            Output.WriteLine("<span>" + aleph + "</span></td>");
            Output.WriteLine("<td> <span class=\"sbkTs_tableLabel\">  Material Type:</span>");
            Output.WriteLine("<span>" +track_item.Bib_Info.SobekCM_Type + "</span></td></tr>");
            
            //Add the aggregation info
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Aggregations:</span></td>");
            Output.WriteLine("<td colspan=\"3\"><span>" + aggregations + "</span></td></tr>");

            //End the table
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
                Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Name:</span></td><td class=\"sbkTs_tblCellUnderline\">" + LABEL1_SPACE+LABEL1_SPACE + LABEL1_SPACE + LABEL1_SPACE + "</td>");
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
                if (!String.IsNullOrEmpty(track_item.Tracking.Material_Received_Date.ToString()))
                {
                    materialRecd_text += " (" +track_item.Tracking.Material_Received_Notes + ")";
                }
            }
            
            if (!String.IsNullOrEmpty(track_item.Tracking.Disposition_Advice_Notes))
            {
                disposition_class = "";
                disposition_text = track_item.Tracking.Disposition_Advice_Notes;
                if (!String.IsNullOrEmpty(track_item.Tracking.Disposition_Notes))
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


            //if (!born_digital)
            //{
                //Get the barcode images for the events
                string imageUrl1 = Get_BarcodeImageUrl_from_string(int_to_base26(itemID), "A", track_item.BibID + track_item.VID + "A");
                string imageUrl2 = Get_BarcodeImageUrl_from_string(int_to_base26(itemID), "B", track_item.BibID + track_item.VID + "B");
                string imageUrl3 = Get_BarcodeImageUrl_from_string(int_to_base26(itemID), "C", track_item.BibID + track_item.VID + "C");
                string imageUrl4 = Get_BarcodeImageUrl_from_string(int_to_base26(itemID), "D", track_item.BibID + track_item.VID + "D");

                //Start the table for the barcodes
                Output.WriteLine("<table class=\"sbkTs_tblBarcodes\">");
                Output.WriteLine("<tr><td><img id=\"barcode1\" src=\"" + imageUrl1 + "\"/></td>");
                Output.WriteLine("<td><img id=\"barcode2\" src=\"" + imageUrl2 + "\"/></td></tr>");
                Output.WriteLine("<tr><td><span class=\"sbkTs_barcode_label1\">Start Scan</span></td>");
                Output.WriteLine("<td><span class=\"sbkTs_barcode_label1\">End Scan</span></td></tr>");

                Output.WriteLine("<tr><td><img id=\"barcode3\" src=\"" + imageUrl3 + "\"/></td>");
                Output.WriteLine("<td><img id=\"barcode4\" src=\"" + imageUrl4 + "\"/></td></tr>");
                Output.WriteLine("<tr><td><span class=\"sbkTs_barcode_label\">Start Processing</span></td>");
                Output.WriteLine("<td><span class=\"sbkTs_barcode_label\">End Processing</span></td></tr>");
                Output.WriteLine("</table>");
            //}
            
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
        public string Get_BarcodeImageUrl_from_string(string InputString, string Action, string FilenameToSave)
        {
	        if (InputString == null) throw new ArgumentNullException("InputString");
			string barcodeString = (InputString + Action).ToUpper();
            string image_save_location = image_location + "\\" + FilenameToSave + ".gif";

            //Generate the image
            Code39BarcodeDraw barcode39 = BarcodeDrawFactory.Code39WithChecksum;
            System.Drawing.Image barcode_image = barcode39.Draw(barcodeString, 60);
            
            //Save the image
            barcode_image.Save(@image_save_location, System.Drawing.Imaging.ImageFormat.Gif);
	        string url = CurrentMode.Base_URL + "temp/" + CurrentUser.UserName.Replace(".", "").Replace("@", "") + "/tsBarcodes/" + itemID.ToString();

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
