// HTML5 10/15/2013

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Engine_Library.Solr;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Settings;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated system administrator to delete an item from this digital library  </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to delete the item requested </li>
    /// </ul></remarks>
    public class Delete_Item_MySobekViewer : abstract_MySobekViewer
    {
        private int errorCode;

        /// <summary> Constructor for a new instance of the Delete_Item_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Delete_Item_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Delete this item");

            // Save mode and set defaults
            errorCode = -1;

            // Second, ensure this is a logged on user and system administrator before continuing
            RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Validate user permissions");
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
			{
                RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "User does not have delete permissions", Custom_Trace_Type_Enum.Error);
                errorCode = 1;
            }
            else
            {
	            bool canDelete = false;
                if ((RequestSpecificValues.Current_User.Can_Delete_All) || (RequestSpecificValues.Current_User.Is_System_Admin))
				{
					canDelete = true;
				}
				else
				{
					// In this case, we actually need to build this!
					try
					{
	//					SobekCM_Item testItem = SobekCM_Item_Factory.Get_Item(Current_Mode.BibID, Current_Mode.VID, null, Tracer);
                        if (RequestSpecificValues.Current_User.Can_Edit_This_Item(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.Bib_Info.SobekCM_Type_String, RequestSpecificValues.Current_Item.Bib_Info.Source.Code, RequestSpecificValues.Current_Item.Bib_Info.HoldingCode, RequestSpecificValues.Current_Item.Behaviors.Aggregation_Code_List))
							canDelete = true;
					}
					catch
					{
						canDelete = false;
					}
				}

				if (!canDelete)
				{
                    RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "User does not have delete permissions", Custom_Trace_Type_Enum.Error);
					errorCode = 1;
				}
            }

			// Ensure the item is valid
			if (errorCode == -1)
			{
                RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Validate item exists");
                if (!UI_ApplicationCache_Gateway.Items.Contains_BibID_VID(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID))
				{
                    RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Item indicated is not valid", Custom_Trace_Type_Enum.Error);
					errorCode = 2;
				}
			}
    

             // If this is a postback, handle any events first
            if ((RequestSpecificValues.Current_Mode.isPostBack) && (errorCode < 0))
            {
                Debug.Assert(RequestSpecificValues.Current_User != null, "User != null");

                // Pull the standard values
                string save_value = HttpContext.Current.Request.Form["admin_delete_item"];
                string text_value = HttpContext.Current.Request.Form["admin_delete_confirm"];

                // Better say "DELETE", or just send back to the item
                if (( save_value == null ) || ( save_value.ToUpper() != "DELETE" ) || ( text_value.ToUpper() != "DELETE"))
                {
                    HttpContext.Current.Response.Redirect(RequestSpecificValues.Current_Mode.Base_URL + RequestSpecificValues.Current_Mode.BibID + "/" + RequestSpecificValues.Current_Mode.VID, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    RequestSpecificValues.Current_Mode.Request_Completed = true;
                }
                else
                {
                    if (RequestSpecificValues.Current_Mode.BibID.ToUpper() == "TEMP000001")
					{
						for (int deleteVID = 2124; deleteVID <= 2134; deleteVID++)
						{
                            RequestSpecificValues.Current_Mode.VID = deleteVID.ToString().PadLeft(5, '0');
							Delete_Item();
						}
					}
					else
					{
						Delete_Item();
					}

                }
            }
        }

		private void Delete_Item()
		{
			errorCode = 0;

			// Get the current item details
			string vid_location = RequestSpecificValues.Current_Item.Source_Directory;
			string bib_location = (new DirectoryInfo(vid_location)).Parent.FullName;
			//if (errorCode == -1)
			//{
			//	// Get item details
			//	DataSet itemDetails = SobekCM_Database.Get_Item_Details(currentMode.BibID, currentMode.VID, Tracer);

			//	// If the itemdetails was null, this item is somehow invalid item then
			//	if (itemDetails == null)
			//	{
			//		Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Item indicated is not valid", Custom_Trace_Type_Enum.Error);
			//		errorCode = 2;
			//	}
			//	else
			//	{
			//		// Get the location for this METS file from the returned value
			//		DataRow mainItemRow = itemDetails.Tables[2].Rows[0];
			//		bib_location = UI_ApplicationCache_Gateway.Settings.Image_Server_Network + mainItemRow["File_Location"].ToString().Replace("/", "\\");
			//		vid_location = bib_location + "\\" + currentMode.VID;
			//	}
			//}     

			// Perform the database delete
            RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Perform database update");
            bool database_result2 = SobekCM_Database.Delete_SobekCM_Item(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID, RequestSpecificValues.Current_User.Is_System_Admin, String.Empty);

			// Perform the SOLR delete
            RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Perform solr delete");
            Solr_Controller.Delete_Resource_From_Index(UI_ApplicationCache_Gateway.Settings.Document_Solr_Index_URL, UI_ApplicationCache_Gateway.Settings.Page_Solr_Index_URL, RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID);

			if (!database_result2)
			{
                RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Error performing delete in the database", Custom_Trace_Type_Enum.Error);
				errorCode = 3;
			}
			else
			{
				// Move the folder to deletes
				try
				{
                    RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Move resource files to RECYCLE BIN folder");

					// Make sure upper RECYCLE BIN folder exists, or create it
					string delete_folder = UI_ApplicationCache_Gateway.Settings.Image_Server_Network + "RECYCLE BIN";
					if (!Directory.Exists(delete_folder))
						Directory.CreateDirectory(delete_folder);

					// Create the bib level folder next
                    string bib_folder = UI_ApplicationCache_Gateway.Settings.Image_Server_Network + "RECYCLE BIN\\" + RequestSpecificValues.Current_Mode.BibID;
					if (!Directory.Exists(bib_folder))
						Directory.CreateDirectory(bib_folder);

					// Ensure the VID folder does not exist
                    string vid_folder = UI_ApplicationCache_Gateway.Settings.Image_Server_Network + "RECYCLE BIN\\" + RequestSpecificValues.Current_Mode.BibID + "\\" + RequestSpecificValues.Current_Mode.VID;
					if (Directory.Exists(vid_folder))
						Directory.Move(vid_folder, vid_folder + "_OLD");

					// Move the VID folder over now
					Directory.Move(vid_location, vid_folder);

					// Check if this was the last VID under this BIB
					if (Directory.GetDirectories(bib_location).Length == 0)
					{
						// Move all files over to the bib folder then
						string[] bib_files = Directory.GetFiles(bib_location);
						foreach (string thisFile in bib_files)
						{
							string fileName = (new FileInfo(thisFile)).Name;
							string new_file = bib_folder + "\\" + fileName;
							File.Move(thisFile, new_file);
						}
					}
				}
				catch (Exception ee)
				{
                    RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Error moving the folder and files to the RECYCLE BIN folder", Custom_Trace_Type_Enum.Error);
                    RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", ee.Message, Custom_Trace_Type_Enum.Error);
                    RequestSpecificValues.Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", ee.StackTrace, Custom_Trace_Type_Enum.Error);
					errorCode = 4;
				}

				// Remove from the item list
				UI_ApplicationCache_Gateway.Items.Remove_Item(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID);

				// Also remove from the cache
                CachedDataManager.Remove_Digital_Resource_Object(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID, RequestSpecificValues.Tracer);
			}
		}

		/// <summary> Property indicates the standard navigation to be included at the top of the page by the
		/// main MySobek html subwriter. </summary>
		/// <value> This returns none since this viewer writes all the necessary navigational elements </value>
		/// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
		/// administrative tabs should be included as well.  </remarks>
		public override MySobek_Included_Navigation_Enum Standard_Navigation_Type
		{
			get
			{
				return MySobek_Included_Navigation_Enum.NONE;
			}
		}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Delete Item' </value>
        public override string Web_Title
        {
            get
            {
                return "Delete Item";
            }
        }

        /// <summary> Write the text for this delete request directly into the main form </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Write_ItemNavForm_Closing", String.Empty);

            if (errorCode == -1)
            {
                // Add the hidden field
                Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                Output.WriteLine("<input type=\"hidden\" id=\"admin_delete_item\" name=\"admin_delete_item\" value=\"\" />");
				Output.WriteLine();

				// Write the top item mimic html portion
                Write_Item_Type_Top(Output, RequestSpecificValues.Current_Item);

				Output.WriteLine("<div id=\"container-inner\">");
				Output.WriteLine("<div id=\"pagecontainer\">");

				Output.WriteLine("<div class=\"sbkMySobek_HomeText\" >");
                Output.WriteLine("  <br /><br />");
                Output.WriteLine("  <p>Enter DELETE in the textbox below and select GO to complete this deletion.</p>");
				Output.WriteLine("  <div id=\"sbkDimv_VerifyDiv\">");
				Output.WriteLine("    <input class=\"sbkDimv_input sbkMySobek_Focusable\" name=\"admin_delete_confirm\" id=\"admin_delete_confirm\" type=\"text\" value=\"\" /> &nbsp; &nbsp; ");
				Output.WriteLine("    <button title=\"Confirm delete of this item\" class=\"sbkMySobek_RoundButton\" onclick=\"delete_item(); return false;\">CONFIRM <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
                Output.WriteLine("  </div>");
                Output.WriteLine("</div>");
				Output.WriteLine();
				Output.WriteLine("</div>");
				Output.WriteLine("</div>");
				Output.WriteLine();
				Output.WriteLine("<!-- Focus on confirm box -->");
				Output.WriteLine("<script type=\"text/javascript\">focus_element('admin_delete_confirm');</script>");
				Output.WriteLine();
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Write_HTML", String.Empty);

            if (errorCode >= 0)
            {
				// Write the top item mimic html portion
				Write_Item_Type_Top(Output, RequestSpecificValues.Current_Item);

				Output.WriteLine("<div id=\"container-inner\">");
				Output.WriteLine("<div id=\"pagecontainer\">");

				Output.WriteLine("<div class=\"sbkMySobek_HomeText\" >");
                Output.WriteLine("  <br /><br />");
                Output.WriteLine("  <p>");

                switch (errorCode)
                {
                    case 0:
						Output.WriteLine("    <div class=\"sbkDimv_SuccessMsg\">DELETE SUCCESSFUL</div>");
                        break;

                    case 1:
						Output.WriteLine("    <div class=\"sbkDimv_ErrorMsg\">DELETE FAILED<br /><br />Insufficient user permissions to perform delete</div>");
                        break;

                    case 2:
						Output.WriteLine("    <div class=\"sbkDimv_ErrorMsg\">DELETE FAILED<br /><br />Item indicated does not exists</div>");
                        break;

                    case 3:
						Output.WriteLine("    <div class=\"sbkDimv_ErrorMsg\">DELETE FAILED<br /><br />Error while performing delete in database</div>");
                        break;

                    case 4:
						Output.WriteLine("    <div class=\"sbkDimv_ErrorMsg\">DELETE PARTIALLY SUCCESSFUL<br /><br />Unable to move all files to the RECYCLE BIN folder</div>");
                        break;
                }

                Output.WriteLine("  </p>");
                Output.WriteLine("</div>");
                Output.WriteLine("<br /><br />");
				Output.WriteLine("</div>");
				Output.WriteLine("</div>");
            }
        }

		/// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
		/// requests from the main HTML subwriter. </summary>
		/// <value> This tells the HTML and mySobek writers to mimic the item viewer </value>
		public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
		{
			get
			{
				return new List<HtmlSubwriter_Behaviors_Enum>
				{
					HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter,
					HtmlSubwriter_Behaviors_Enum.Suppress_Banner
				};
			}
		}
    }
}

