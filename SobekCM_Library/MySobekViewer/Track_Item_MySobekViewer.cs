#region Using directives
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;
using SobekCM.Resource_Object;
using System.Collections.Generic;
#endregion

namespace SobekCM.Library.MySobekViewer
{
    class Track_Item_MySobekViewer : abstract_MySobekViewer
    {
       private Dictionary<string, string> user_list;
        private List<string> scanners_list;
        private  string barcodeString;
        private  int itemID;
        private string encodedItemID;
        private string checksum;
        private string BibID;
        private string VID;
        private string error_message = String.Empty;
        private int stage=1;
        private string hidden_request ;
        private string hidden_value;
        private string title;
       
        private DataTable tracking_users;
        private DataTable workflow_entries_from_DB;
        private Dictionary<string, DataRow> current_entries;



        /// <summary> Constructor for a new instance of the Track_Item_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Item"> Individual digital resource to be edited by the user </param>
        /// <param name="Code_Manager"> Code manager contains the list of all valid aggregation codes </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Track_Item_MySobekViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Custom_Tracer Tracer) 
            :  base(User)
          {
                    Tracer.Add_Trace("Track_Item_MySobekViewer.Constructor", String.Empty);

                    currentMode = Current_Mode;
                    

                     //If there is no user, go back
                    if (user == null)
                    {
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                        HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                    }

     
            //Initialize variables
            tracking_users = new DataTable();
            user_list = new Dictionary<string, string>();
            scanners_list = new List<string>();

            //Get the barcode (scanned by the user) from the form
   //         barcodeString = HttpContext.Current.Request.Form["txtScannedString"] ?? String.Empty;
        
   

            //Get the list of users who are possible Scanning/Processing technicians from the DB
            tracking_users = Database.SobekCM_Database.Tracking_Get_Users_Scanning_Processing();
            
            foreach (DataRow row in tracking_users.Rows)
            {
                string username = row["UserName"].ToString();
                string full_name = row["FirstName"].ToString() + " " + row["LastName"].ToString();
                user_list.Add(username, full_name);
            }
           
            if(!user_list.ContainsKey(user.UserName))
                user_list.Add(user.UserName, user.Full_Name);

            //Get the list of scanning equipment
            DataTable scanners = new DataTable();
            scanners = Database.SobekCM_Database.Tracking_Get_Scanners_List();
            foreach (DataRow row in scanners.Rows)
            {
                scanners_list.Add(row["ScanningEquipment"].ToString());
            }

            //See if there were any hidden requests
            hidden_request = HttpContext.Current.Request.Form["Track_Item_behaviors_request"] ?? String.Empty;
            hidden_value = HttpContext.Current.Request.Form["Track_Item_hidden_value"] ?? String.Empty;
   //         hidden_entry_type = HttpContext.Current.Request.Form[""]
            switch (hidden_request.ToLower())
            {
                case "decode_barcode":
                    barcodeString = hidden_value;
                    //Decode the scanned barcode 
                    if (!String.IsNullOrEmpty(barcodeString))
                    {
                        //Find a match for a number within the string, which corresponds to the event
                        Match val = Regex.Match(barcodeString, @"\d");
                        if (val.Success)
                        {
                            int len = barcodeString.IndexOf(val.Value);
                            Int32.TryParse(val.Value, out stage);

                            //Extract the item ID & checksum from the barcode string
                            encodedItemID = barcodeString.Substring(0, len);
                            checksum = barcodeString.Substring(len + 1, (barcodeString.Length - len - 1));

                            bool isValidChecksum = Is_Valid_Checksum(encodedItemID, val.Value, checksum);
                            if (!isValidChecksum)
                                error_message = "Barcode error- checksum error";

                            bool IsValidItem = Get_Item_Info_from_Barcode(encodedItemID);
                            if (!IsValidItem)
                                error_message = "Barcode error - invalid ItemID referenced";
                            else
                            {
                                Get_Bib_VID_from_ItemID(itemID);
                            }

                        }
                    }
                    break;

                case "read_manual_entry":
                    //Get the related hidden values for the selected manual entry fields
                    string hidden_bibID = HttpContext.Current.Request.Form["hidden_BibID"] ?? String.Empty;
                    string hidden_VID =  HttpContext.Current.Request.Form["hidden_VID"] ?? String.Empty;
                    string hidden_event_num = HttpContext.Current.Request.Form["hidden_event_num"] ?? String.Empty;
                    if (String.IsNullOrEmpty(hidden_bibID) || String.IsNullOrEmpty(hidden_VID) || String.IsNullOrEmpty(hidden_event_num))
                    {
                        error_message = "You must enter a BibID and VID!";
                    }
                    else
                    {
                        Int32.TryParse(hidden_event_num, out stage);
                        BibID = hidden_bibID;
                        VID = hidden_VID;
                        try
                        {
                            itemID = Resource_Object.Database.SobekCM_Database.Get_ItemID(BibID, VID);
                            Get_Bib_VID_from_ItemID(itemID);
                        }
                        catch (Exception ee)
                        {
                            error_message = "Invalid BibID or VID!";
                        }


                    }
                    break;

                default:
                    break;
               }

            //TODO: Complete form validation

            //if there is a BibID, VID available, get the open workflows for this item from the database
 //           if (!String.IsNullOrEmpty(itemID.ToString()))
     //           workflow_entries_from_DB = Database.SobekCM_Database.Tracking_Get_Open_Workflows(itemID);

          }

        /// <summary> Get the item BibID, VID, title from the ItemID </summary>
        /// <param name="item_ID"></param>
        private void Get_Bib_VID_from_ItemID(int item_ID)
        {
        
            DataRow temp = Database.SobekCM_Database.Tracking_Get_Item_Info_from_ItemID(item_ID);
            BibID = temp["BibID"].ToString();
            VID = temp["VID"].ToString();
            title = temp["Title"].ToString();
            if (String.IsNullOrEmpty(BibID) || String.IsNullOrEmpty(VID))
                error_message = "No matching item found for this ItemID!";

        }


        /// <summary> Validate the checksum on the barcode value </summary>
        /// <param name="encoded_ItemID">The itemID in Base-26 format</param>
        /// <param name="Stage">Indicates the event boundary</param>
        /// <param name="checksum_string">The checksum value generated for this barcode</param>
        /// <returns>Returns TRUE if the checksum is valid, else FALSE</returns>
        private bool Is_Valid_Checksum(string encoded_ItemID, string Stage, string checksum_string)
        {
            bool is_valid_checksum = true;
            int event_num=0;
            int thisItemID = 0;
            int thisChecksumValue = 0;
            
            Int32.TryParse(Stage, out event_num);
            thisItemID = Int_from_Base26(encoded_ItemID);
            thisChecksumValue = Int_from_Base26(checksum_string);

            if (thisChecksumValue != (thisItemID + event_num)%26)
                is_valid_checksum = false;
 
            return is_valid_checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encodedItemID"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        private bool Get_Item_Info_from_Barcode(string encoded_ItemID)
        {
            bool result = true;
            itemID = Int_from_Base26(encoded_ItemID);
            if (String.IsNullOrEmpty(itemID.ToString()))
                result = false;
            return result;
        }

       
        /// <summary> Converts a Base-26 value to the Base-10 equivalent </summary>
        /// <param name="number">The number in Base-26</param>
        /// <returns>The converted Base-10 equivalent</returns>
        private  int Int_from_Base26(String number)
        {
            int convertedNumber = 0;
            if (!String.IsNullOrEmpty(number))
            {
                convertedNumber = (number[0] - 'A');
                for (int i = 1; i < number.Length; i++)
                {
                    convertedNumber *= 26;
                    convertedNumber += (number[i] - 'A');
                }
            }
            return convertedNumber;
        }


        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the value 'Track Item'</value>
        public override string Web_Title
        {
            get
            {
                return "Item Tracking";
            }
        }

        
        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the individual metadata elements are added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Write_HTML", "Do nothing");

            //Include the js files
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.color-2.1.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.timers.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_track_item.js\" ></script>");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/jquery-ui.css\" />");
        }


        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Add_Controls", "");
         //   base.Add_Controls(MainPlaceHolder, Tracer);

            string barcode_row_style = String.Empty;
            string manual_row_style = String.Empty;

            StringBuilder builder = new StringBuilder(2000);
            builder.AppendLine("<!-- Track_Item_MySobekViewer.Add_Controls -->");
            builder.AppendLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/SobekCM_MySobek.css\" /> ");
            builder.AppendLine("<div class=\"SobekHomeText\">");

            //Add the hidden variables
            builder.AppendLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new page, etc..) -->");
            builder.AppendLine("<input type=\"hidden\" id=\"Track_Item_behaviors_request\" name=\"Track_Item_behaviors_request\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"Track_Item_hidden_value\" name=\"Track_Item_hidden_value\"value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"TI_entry_type\" name=\"TI_entry_type\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_BibID\" name=\"hidden_BibID\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_VID\" name=\"hidden_VID\" value=\"\" />");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_event_num\" name=\"hidden_event_num\" value=\"\" />");

            //Start the User, Equipment info table
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr>");
            builder.AppendLine("          <td>Scanned/Processed by:</td>");
            builder.AppendLine("          <td><select id=\"ddlUserStart\" name=\"ddlUserStart\">");

            //Add the list of users to the dropdown list
            foreach (KeyValuePair<string, string> thisUser in user_list)
            {
                if (thisUser.Key == user.UserName)
                    builder.AppendLine("<option value=\"" + thisUser.Key + "\" selected>" + thisUser.Value + "</option>");
                else
                {
                    builder.AppendLine("<option value=\"" + thisUser.Key + "\">" + thisUser.Value + "</option>");
                }
            }
            builder.AppendLine("</td>");
            builder.AppendLine("           <td>Equipment used:</td>");
            builder.AppendLine("           <td><select name=\"ddlEquipmentStart\" id=\"ddlEquipmentStart\">");
            //Add the list of scanners to the dropdown list
            foreach (string thisScanner in scanners_list)
                builder.AppendLine("<option value=\"\">" + thisScanner + "</option>");
            builder.AppendLine("</select></td>");
            builder.AppendLine("</tr>");
            builder.AppendLine("</table>");


            //Start the Entry Type Table
            builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Entry Type</h2></span>");
            builder.AppendLine("<table class=\"sbkTi_table\">");
            if (hidden_request == "read_manual_entry")
            {
                builder.AppendLine("<tr><td><input type=\"radio\" name=\"rbEntryType\" id=\"rb_barcode\" value=0 onclick=\"rbEntryTypeChanged(this.value);\">Barcode Entry</td></tr>");
                builder.AppendLine("<tr><td><input type=\"radio\" name=\"rbEntryType\" id=\"rb_manual\" value=1 checked onclick=\"rbEntryTypeChanged(this.value);\">Manual Entry</td></tr>");
                barcode_row_style = "style=\"display:none\";";
            }
            else
            {
                builder.AppendLine("<tr><td><input type=\"radio\" name=\"rbEntryType\" id=\"rb_barcode\" value=0 checked onclick=\"rbEntryTypeChanged(this.value);\">Barcode Entry</td></tr>");
                builder.AppendLine("<tr><td><input type=\"radio\" name=\"rbEntryType\" id=\"rb_manual\" value=1 onclick=\"rbEntryTypeChanged(this.value);\">Manual Entry</td></tr>");
                manual_row_style = "style=\"display:none\";";
            }
            builder.AppendLine("</table>");


            builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Item Information</h2></span>");
           

            //Start the Item info table
            string bibid = (String.IsNullOrEmpty(BibID))? String.Empty:BibID;
            string vid = (String.IsNullOrEmpty(VID)) ? String.Empty : VID;

            
      
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr id=\"tblrow_Barcode\" "+barcode_row_style+"><td>Scan barcode here:</td>");
            builder.AppendLine("         <td colspan=\"3\"><input type=\"text\" id=\"txtScannedString\" name=\"txtScannedString\" autofocus onchange=\"BarcodeStringTextbox_Changed(this.value);\"/></td></tr>");
            builder.AppendLine("<tr id=\"tblrow_Manual1\" "+manual_row_style+"><td>BibID:</td><td><input type=\"text\" id=\"txtBibID\" value=\""+bibid+"\" /></td>");
            builder.AppendLine("         <td>VID:</td><td><input type=\"text\" id=\"txtVID\" value=\""+vid+"\" /></td>");
            builder.AppendLine("</tr>");
            builder.AppendLine("<tr id=\"tblrow_Manual2\" "+manual_row_style+">");
            builder.AppendLine("<td>Event:</td><td><select id=\"ddlManualEvent\" name=\"ddlManualEvent\">");
            builder.AppendLine("                                       <option value=\"1\" selected>Start Scanning</option>");
            builder.AppendLine("                                        <option value=\"2\">End Scanning</option>");
            builder.AppendLine("                                        <option value=\"3\">Start Processing</option>");
            builder.AppendLine("                                        <option value=\"4\">End Processing</option></select>");
            builder.AppendLine("</td>");
     //       builder.AppendLine("<td>Title:</td><td><input type=\"text\" id=\"txtTitle\" disabled value=\""+title+"\" /></td>");
            builder.AppendLine("<td>");
            builder.AppendLine("<div id=\"divAddButton\" style=\"float:right;\">");
            builder.AppendLine("    <button title=\"Add new tracking entry\" class=\"sbkMySobek_RoundButton\" onclick=\"Add_new_entry(); return false;\">ADD</button>");
            builder.AppendLine("</div></td>");


            builder.AppendLine("</tr>");
            //End this table
            builder.AppendLine("</table>");

            //If a new event has been scanned/entered, then display this table
            if (!String.IsNullOrEmpty(bibid) && !String.IsNullOrEmpty(vid))
            {
                string selected_text_scanning = String.Empty;
                string selected_text_processing = String.Empty;
                string currentTime = DateTime.Now.ToString("hh:mm");
                string startTime = String.Empty;
                string endTime = String.Empty;


                //TODO: Display any open workflows for this item

                //TODO: Display any workflows opened/closed in this session

                if (stage == 1 || stage == 2)
                {
                    selected_text_scanning = " selected";
                    if (stage == 1)
                        startTime = currentTime;
                    else
                    {
                        endTime = currentTime;
                    }
                }
                else if (stage == 3 || stage == 4)
                {
                    selected_text_processing = " selected";
                    if (stage == 3)
                        startTime = currentTime;
                    else
                    {
                        endTime = currentTime;
                    }
                 }
                //Start the Tracking Info section
                builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Add Tracking Information</h2></span>");
                builder.AppendLine("<table class=\"sbkTi_tblItemDetails\">");
                builder.AppendLine("<tr >");
                builder.AppendLine("<td><span class=\"sbkTi_tblRow_ItemInfoLabel\">BibID: </span></td><td><span class=\"sbkTi_tblRow_ItemInfoText\">" + bibid + "</span></td>");
                builder.AppendLine("<td><span class=\"sbkTi_tblRow_ItemInfoLabel\">VID: </span></td><td><span class=\"sbkTi_tblRow_ItemInfoText\">" + vid + "</span></td>");
                builder.AppendLine("<td><span class=\"sbkTi_tblRow_ItemInfoLabel\">Title: </span></td><td><span class=\"sbkTi_tblRow_ItemInfoText\">" + title + "</span></td>");
                builder.AppendLine("</tr>");
                builder.AppendLine("</table>");

                builder.AppendLine("<span class=\"sbkTi_eventTableSpan\">");
                //Start the table for the current tracking event
                builder.AppendLine("<table class=\"sbkTi_table\">");
                builder.AppendLine("<tr><td>Workflow:</td>");
                builder.AppendLine("         <td><select id=\"ddlEvent\" name=\"ddlEvent\"> disabled");
                builder.AppendLine("                  <option value=\"1\" "+selected_text_scanning+">Scanning</option>");
                builder.AppendLine("                  <option value=\"2\""+selected_text_processing+">Processing</option></select>");
                builder.AppendLine("         </td>");
                builder.AppendLine("         <td>Date:</td>");
                string currentDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                builder.AppendLine("         <td><input type=\"date\" name=\"txtStartDate\" id=\"txtStartDate\" value=\"" + currentDate + "\" /> </td>");
                builder.AppendLine("</tr>");
                
                //Add the Start and End Times
                builder.AppendLine("<tr>");
                builder.AppendLine("<td>Start Time:</td>");
                builder.AppendLine("<td><input type=\"time\" name=\"txtStartTime\" id=\"txtStartTime\" value = \""+startTime+"\"/></td>");
                builder.AppendLine("<td>End Time:</td>");
                builder.AppendLine("<td><input type=\"time\" name=\"txtEndTime\" id=\"txtEndTime\" value = \""+endTime+"\"/></td>");

                //End this table
                builder.AppendLine("</table>");

                builder.AppendLine("</span>");
            }




            //Add the Save and Done buttons
            builder.AppendLine("<div id=\"divButtons\" style=\"float:right;\">");
            builder.AppendLine("    <button title=\"Save changes\" class=\"sbkMySobek_RoundButton\" onclick=\"save(); return false;\">SAVE</button>");
            builder.AppendLine("    <button title=\"Save all changes and exit\" class=\"sbkMySobek_RoundButton\" onclick=\"save(); return false;\">DONE</button>");
            builder.AppendLine("</div");
            builder.AppendLine("<br/><br/>");
            //Close the main div
            builder.AppendLine("</div>");

            
            LiteralControl control1 = new LiteralControl(builder.ToString());
          
            MainPlaceHolder.Controls.Add(control1);


     }



    }
}
