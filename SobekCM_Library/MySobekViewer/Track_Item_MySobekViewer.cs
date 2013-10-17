#region Using directives
using System;
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
#endregion

namespace SobekCM.Library.MySobekViewer
{
    class Track_Item_MySobekViewer : abstract_MySobekViewer
    {
        private readonly string user_id;
        private readonly string user_name;
        
        private  string barcodeString;
        private  int itemID;
        private string encodedItemID;
        private string checksum;
        private string BibID;
        private string VID;
        private string error_message = String.Empty;
        private SobekCM_Item item;
        private int stage=1;


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

            user_name = user.Full_Name;
            user_id = user.UserName;
            // If the user cannot edit this item, go back
            //if (!user.Can_Edit_This_Item(item))
            //{
            //    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
            //    HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
            //}

            //Get the barcode scanned by the user
            barcodeString = HttpContext.Current.Request.Form["txtScannedString"] ?? String.Empty;
           
            //Decode the scanned bar code 
            if (!String.IsNullOrEmpty(barcodeString))
            {
                Match val = Regex.Match(barcodeString, @"\d");
                if (val.Success)
                {
                    int len = barcodeString.IndexOf(val.Value);
                    Int32.TryParse(val.Value, out stage);
                    encodedItemID = barcodeString.Substring(0, len);
                    checksum = barcodeString.Substring(len - 1);

                    bool IsValidString = Get_Item_Info_from_Barcode(encodedItemID, checksum);
                    if (!IsValidString)
                        error_message = "Invalid barcode!";
                }
            }

          }


        private bool Get_Item_Info_from_Barcode(string encodedItemID, string checksum)
        {
            bool result = true;
            itemID = Int_from_Base26(encodedItemID);
            //TODO: Get the Bib, VID, Tracking info forthis itemID
//            BibID = Resource_Object.Database.SobekCM_Database.
            BibID = itemID.ToString();

            return result;
        }

       
        /// <summary> Converts a Base-26 value to the Base-10 equivalent </summary>
        /// <param name="number"></param>
        /// <returns>The converted Base-10 equivalent</returns>
        private  int Int_from_Base26(String number)
        {
            int convertedNumber = 0;
            if (number != null && number.Length > 0)
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
                return "Track Item";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the individual metadata elements are added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Write_HTML", "Do nothing");
        }


        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Add_Controls", "");
         //   base.Add_Controls(MainPlaceHolder, Tracer);

            StringBuilder builder = new StringBuilder(2000);
            builder.AppendLine("<!-- Track_Item_MySobekViewer.Add_Controls -->");
            builder.AppendLine("<div class=\"SobekHomeText\">");

            builder.AppendLine("<span class=\"sbkAdm_HomeText\"><h2>Item Information</h2></span>");
           //Start the Item info table
            //TODO: Get the correct Bib and VID 
            string bibid = (String.IsNullOrEmpty(BibID))? String.Empty:BibID;
            string vid = (String.IsNullOrEmpty(BibID)) ? String.Empty : BibID;
            
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr><td>Scan barcode here:</td>");
            builder.AppendLine("         <td colspan=\"3\"><input type=\"text\" id=\"txtScannedString\" name=\"txtScannedString\" autofocus /></td></tr>");
            builder.AppendLine("<tr><td>BibID:</td><td><input type=\"text\" id=\"txtBibID\"/>"+bibid+"</td>");
            builder.AppendLine("         <td>VID:</td><td><input type=\"text\" id=\"txtVID\"/>"+vid+"</td>");
            builder.AppendLine("</tr>");
            //End this table
            builder.AppendLine("</table>");

            //Start the next section
            builder.AppendLine("<span class=\"sbkAdm_HomeText\"><h2>Add Tracking Information</h2></span>");
            //Start the Tracking Info 

            //Start the table for the 'Start' Stage
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr><td colspan=\"4\"><span><b>Start</b></span></td></tr>");
            builder.AppendLine("<tr><td>Event:</td>");
            builder.AppendLine("         <td><select id=\"ddlEvent\" name=\"ddlEvent\">");
            builder.AppendLine("                  <option value=\"1\" selected>Scanning</option>");
            builder.AppendLine("                  <option value=\"2\">Processing</option>");
            builder.AppendLine("         </td>");
            builder.AppendLine("         <td>Time:</td>");
            builder.AppendLine("         <td><input type=\"text\" name=\"txtStartTime\" id=\"txtStartTime\" value=\"" + DateTime.Now + "\" /> </td>");
            builder.AppendLine("</tr>");
            builder.AppendLine("<tr>");
            builder.AppendLine("          <td>Scanned/Processed by:</td>");
            builder.AppendLine("          <td><select id=\"ddlUserStart\" name=\"ddlUserStart\"><option value=\"" + user_id + "\" selected>" + user_name + "</select></td>");
            builder.AppendLine("           <td>Equipment used:</td>");
            builder.AppendLine("           <td><select name=\"ddlEquipmentStart\" id=\"ddlEquipmentStart\"><option value=\"\">temp value</option></select></td>");
            builder.AppendLine("</tr>");
            ////End this table
            //builder.AppendLine("</table>");
            
            ////Start the table for the 'End' Stage
            //builder.AppendLine("<table>");
            builder.AppendLine("<tr><td colspan=\"4\"><span><b>End</b></span></td></tr>");
            builder.AppendLine("<tr><td>Event:</td>");
            builder.AppendLine("         <td><select id=\"ddlEvent\" name=\"ddlEvent\">");
            builder.AppendLine("                  <option value=\"3\">Scanning</option>");
            builder.AppendLine("                  <option value=\"4\">Processing</option>");
            builder.AppendLine("         </td>");
            builder.AppendLine("         <td>Time:</td>");
            builder.AppendLine("         <td><input type=\"text\" name=\"txtEndTime\" id=\"txtEndTime\" value=\"" + DateTime.Now + "\"/> </td>");
            builder.AppendLine("</tr>");
            builder.AppendLine("<tr>");
            builder.AppendLine("          <td>Scanned/Processed by:</td>");
            builder.AppendLine("          <td><select id=\"ddlUserEnd\" name=\"ddlUserEnd\"><option value=\"" + user_id + "\" selected>" + user_name + "</select></td>");
            builder.AppendLine("           <td>Equipment used:</td>");
            builder.AppendLine("           <td><select name=\"ddlEquipmentEnd\" id=\"ddlEquipmentEnd\"><option value=\"\">temp value</option></select></td>");
            builder.AppendLine("</tr>");
            //End this table
            builder.AppendLine("</table>");


            builder.AppendLine("<span class=\"sbkAdm_HomeText\"><h2>Current Tracking Information</h2></span>");
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr><td>ADD EXISTING INFO HERE</td></tr>");
            builder.AppendLine("</table>");
            //currentMode.My_Sobek_SubMode = current_submode;


            builder.AppendLine("</div>");
            LiteralControl control1 = new LiteralControl(builder.ToString());

            MainPlaceHolder.Controls.Add(control1);



            //StringBuilder builder = new StringBuilder(2000);
            //StringWriter output = new StringWriter(builder);

       


        }


    }
}
