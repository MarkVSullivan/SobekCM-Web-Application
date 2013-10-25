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
using AjaxControlToolkit;
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
        private SobekCM_Item item;
        private int stage=1;

        private DataTable tracking_users;


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
            barcodeString = HttpContext.Current.Request.Form["txtScannedString"] ?? String.Empty;
           
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
                    checksum = barcodeString.Substring(len - 1);

                    bool isValidChecksum = Is_Valid_Checksum(encodedItemID, val.Value, checksum);
                    if (!isValidChecksum)
                        error_message = "Barcode error- checksum error";

                    bool IsValidItem= Get_Item_Info_from_Barcode(encodedItemID);
                    if (!IsValidItem)
                        error_message = "Barcode error - invalid ItemID referenced";
                }
            }

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
          }


        /// <summary> Validate the checksum on the barcode value </summary>
        /// <param name="encoded_ItemID">The itemID in Base-26 format</param>
        /// <param name="Stage">Indicates the event boundary</param>
        /// <param name="checksum_string">The checksum value generated for this barcode</param>
        /// <returns>Returns TRUE if the checksum is valid, else FALSE</returns>
        private bool Is_Valid_Checksum(string encoded_ItemID, string Stage, string checksum_string)
        {
            bool valid_checksum = true;
            int event_num=0;
            int thisItemID = 0;
            int thisChecksumValue = 0;
            
            Int32.TryParse(Stage, out event_num);
            thisItemID = Int_from_Base26(encoded_ItemID);
            thisChecksumValue = Int_from_Base26(checksum_string);

            if (thisChecksumValue != (thisItemID + event_num)%26)
                valid_checksum = false;

            return valid_checksum;
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
            //TODO: Get the Bib, VID, Tracking info forthis itemID
//            BibID = Resource_Object.Database.SobekCM_Database.
            BibID = itemID.ToString();

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
            Output.WriteLine("<script type=\"text/javascript\" src=\"http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css\"></script>");
        }


        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Add_Controls", "");
         //   base.Add_Controls(MainPlaceHolder, Tracer);

            StringBuilder builder = new StringBuilder(2000);
            builder.AppendLine("<!-- Track_Item_MySobekViewer.Add_Controls -->");
            builder.AppendLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/SobekCM_MySobek.css\" /> ");
            builder.AppendLine("<div class=\"SobekHomeText\">");

            //Start the Entry Type Table
            builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Entry Type</h2></span>");
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr><td><input type=\"radio\" name=\"rbEntryType\" id=\"rb_barcode\" value=0 checked onclick=\"rbEntryTypeChanged(this.value);\">Barcode Entry</td></tr>");
            builder.AppendLine("<tr><td><input type=\"radio\" name=\"rbEntryType\" id=\"rb_manual\" value=1 onclick=\"rbEntryTypeChanged(this.value);\">Manual Entry</td></tr>");
            builder.AppendLine("</table>");


            builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Item Information</h2></span>");
           

            //Start the Item info table
            //TODO: Get the correct Bib and VID 
            string bibid = (String.IsNullOrEmpty(BibID))? String.Empty:BibID;
            string vid = (String.IsNullOrEmpty(VID)) ? String.Empty : VID;
            
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr id=\"tblrow_Barcode\"><td>Scan barcode here:</td>");
            builder.AppendLine("         <td colspan=\"3\"><input type=\"text\" id=\"txtScannedString\" name=\"txtScannedString\" autofocus /></td></tr>");
            builder.AppendLine("<tr id=\"tblrow_Manual1\" style=\"display:none\"><td>BibID:</td><td><input type=\"text\" id=\"txtBibID\">"+bibid+"</input></td>");
            builder.AppendLine("         <td>VID:</td><td><input type=\"text\" id=\"txtVID\"/>"+vid+"</td>");
            builder.AppendLine("</tr>");
            builder.AppendLine("<tr id=\"tblrow_Manual2\" style=\"display:none\">");
            builder.AppendLine("<td>Event:</td><td><select id=\"ddlManualEvent\" name=\"ddlManualEvent\">");
            builder.AppendLine("                                       <option value=\"1\" selected>Scanning</option>");
            builder.AppendLine("                                        <option value=\"2\">Processing</option></select>");
            builder.AppendLine("</td>");
            builder.AppendLine("<td>Title:</td><td><input type=\"text\" id=\"txtTitle\" disabled/></input></td>");

            builder.AppendLine("</tr>");
            //End this table
            builder.AppendLine("</table>");

            //Start the Tracking Info section
            builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Add Tracking Information</h2></span>");
       
            //Start the table for the current tracking event
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr><td>Event:</td>");
            builder.AppendLine("         <td><select id=\"ddlEvent\" name=\"ddlEvent\"> disabled");
            builder.AppendLine("                  <option value=\"1\" selected>Scanning</option>");
            builder.AppendLine("                  <option value=\"2\">Processing</option></select>");
            builder.AppendLine("         </td>");
            builder.AppendLine("         <td>Date:</td>");

            builder.AppendLine("         <td><input type=\"text\" name=\"txtStartDate\" id=\"txtStartDate\" value=\"" + DateTime.Now.Date.ToShortDateString() + "\" /> </td>");
            builder.AppendLine("</tr>");
            builder.AppendLine("<tr>");
            builder.AppendLine("          <td>Scanned/Processed by:</td>");
            builder.AppendLine("          <td><select id=\"ddlUserStart\" name=\"ddlUserStart\">");
            
            //Add the list of users to the dropdown list
            foreach (KeyValuePair<string, string> thisUser in user_list)
            {
                if(thisUser.Key==user.UserName)
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
            foreach(string thisScanner in scanners_list)
              builder.AppendLine("<option value=\"\">"+thisScanner+"</option>");
            builder.AppendLine("</select></td>");
            builder.AppendLine("</tr>");
            //Add the Start and End Times
            builder.AppendLine("<tr>");
            builder.AppendLine("<td>Start Time:</td>");
            builder.AppendLine("<td><input type=\"text\" name=\"txtStartTime\" id=\"txtStartTime\"/></td>");
            builder.AppendLine("<td>End Time:</td>");
            builder.AppendLine("<td><input type=\"text\" name=\"txtEndTime\" id=\"txtEndTime\"/></td>");

            //End this table
            builder.AppendLine("</table>");

            //Add the Save and Done buttons
            builder.AppendLine("<div id=\"divButtons\" style=\"float:right;\">");
            builder.AppendLine("    <button title=\"Save changes\" class=\"sbkMySobek_RoundButton\" onclick=\"save(); return false;\">SAVE</button>");
            builder.AppendLine("    <button title=\"Save all changes and exit\" class=\"sbkMySobek_RoundButton\" onclick=\"save(); return false;\">DONE</button>");
            builder.AppendLine("</div");
            builder.AppendLine("<br/><br/>");
            //Close the main div
            builder.AppendLine("</div>");

            builder.AppendLine("<script>$(function() {$( \"#txtStartDate\" ).datepicker();});</script>");

            LiteralControl control1 = new LiteralControl(builder.ToString());
          
            MainPlaceHolder.Controls.Add(control1);


     }



    }
}
