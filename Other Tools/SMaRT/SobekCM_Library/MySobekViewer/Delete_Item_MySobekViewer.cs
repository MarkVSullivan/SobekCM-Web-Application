#region Using directives

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

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
        private readonly int errorCode;


        /// <summary> Constructor for a new instance of the Delete_Item_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="All_Items_Lookup"> Allows individual items to be retrieved by various methods as <see cref="SobekCM.Library.Application_State.Single_Item"/> objects.</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Delete_Item_MySobekViewer(User_Object User,
            SobekCM_Navigation_Object Current_Mode, 
            Item_Lookup_Object All_Items_Lookup,
            
            Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Delete this item");

            // Save mode and set defaults
            currentMode = Current_Mode;
            errorCode = -1;

            // First, ensure this is a logged on user and system administrator before continuing
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Validate user permissions" );
            if ((User == null) || ((!User.Is_System_Admin) && (User.UserName.ToLower() != "neldamaxs")))
            {
                Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "User does not have delete permissions", Custom_Trace_Type_Enum.Error );
                errorCode = 1;
            }

            // Second, ensure the item is valid
            if( errorCode == -1 )
            {
                Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Validate item exists" );
                if (!All_Items_Lookup.Contains_BibID_VID( Current_Mode.BibID, Current_Mode.VID))
                {
                    Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Item indicated is not valid", Custom_Trace_Type_Enum.Error );
                    errorCode = 2;
                }
            }

            // Get the current item details
            string bib_location = String.Empty;
            string vid_location = String.Empty;
            if ( errorCode == -1 )
            {
                // Get item details
                DataSet itemDetails = SobekCM_Database.Get_Item_Details(currentMode.BibID, currentMode.VID, Tracer);

                // If the itemdetails was null, this item is somehow invalid item then
                if ( itemDetails == null )
                {
                    Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Item indicated is not valid", Custom_Trace_Type_Enum.Error );
                    errorCode = 2;
                }
                else
                {
                    // Get the location for this METS file from the returned value
                    DataRow mainItemRow = itemDetails.Tables[2].Rows[0];
                    bib_location = SobekCM_Library_Settings.Image_Server_Network + mainItemRow["File_Location"].ToString().Replace("/", "\\");
                    vid_location = bib_location + "\\" + currentMode.VID;
                }
            }           

             // If this is a postback, handle any events first
            if ((currentMode.isPostBack) && ( errorCode < 0 ))
            {
                Debug.Assert(User != null, "User != null");

                // Pull the standard values
                string save_value = HttpContext.Current.Request.Form["admin_delete_item"];
                string text_value = HttpContext.Current.Request.Form["admin_delete_confirm"];

                // Better say "DELETE", or just send back to the item
                if (( save_value == null ) || ( save_value.ToUpper() != "DELETE" ) || ( text_value.ToUpper() != "DELETE"))
                {
                    HttpContext.Current.Response.Redirect(Current_Mode.Base_URL + currentMode.BibID + "/" + currentMode.VID);
                }
                else
                {
                    errorCode = 0;

                    // Perform the database delete
                    Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Perform database update");
                    bool database_result = SobekCM_Database.Delete_SobekCM_Item(currentMode.BibID, currentMode.VID, User.Is_System_Admin, String.Empty );

                    if (!database_result)
                    {
                        Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Error performing delete in the database", Custom_Trace_Type_Enum.Error);
                        errorCode = 3;
                    }
                    else
                    {
                        // Move the folder to deletes
                        try
                        {
                            Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Move resource files to DELETED folder");

                            // Make sure upper deleted folder exists, or create it
                            string delete_folder = SobekCM_Library_Settings.Image_Server_Network + "DELETED";
                            if (!Directory.Exists(delete_folder))
                                Directory.CreateDirectory(delete_folder);

                            // Create the bib level folder next
                            string bib_folder = SobekCM_Library_Settings.Image_Server_Network + "DELETED\\" + currentMode.BibID;
                            if (!Directory.Exists(bib_folder))
                                Directory.CreateDirectory(bib_folder);

                            // Ensure the VID folder does not exist
                            string vid_folder = SobekCM_Library_Settings.Image_Server_Network + "DELETED\\" + currentMode.BibID + "\\" + currentMode.VID;
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
                            Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Error moving the folder and files to the DELETED folder", Custom_Trace_Type_Enum.Error);
                            Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", ee.Message, Custom_Trace_Type_Enum.Error);
                            Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                            errorCode = 4;
                        }

                        // Remove from the item list
                        All_Items_Lookup.Remove_Item(currentMode.BibID, currentMode.VID);

                        // Also remove from the cache
                        Cached_Data_Manager.Remove_Digital_Resource_Object(currentMode.BibID, currentMode.VID, Tracer);
                    }
                }
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
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Add_HTML_In_Main_Form", String.Empty);

            if (errorCode == -1)
            {
                // Add the hidden field
                Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                Output.WriteLine("<input type=\"hidden\" id=\"admin_delete_item\" name=\"admin_delete_item\" value=\"\" />");
                Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");

                Output.WriteLine("<div class=\"SobekHomeText\" >");
                Output.WriteLine("<br /><br />");
                Output.WriteLine("<blockquote>");

                Output.WriteLine("Enter DELETE in the textbox below and select GO to complete this deletion.<br /><br />");
                Output.WriteLine("<input class=\"admin_small_input\" name=\"admin_delete_confirm\" id=\"admin_delete_confirm\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('admin_delete_confirm', 'admin_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_delete_confirm', 'admin_small_input')\" /> &nbsp; &nbsp; ");
                Output.WriteLine("<button title=\"Delete Item\" class=\"go_button\" onclick=\"delete_item(); return false;\"></button>");

                Output.WriteLine("</blockquote>");
                Output.WriteLine("</div>");
                Output.WriteLine("<br /><br />");
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

                Output.WriteLine("<div class=\"SobekHomeText\" >");
                Output.WriteLine("<br /><br />");
                Output.WriteLine("<blockquote>");

                switch (errorCode)
                {
                    case 0:
                        Output.WriteLine("<span style=\"color:Blue; font-size:1.3em;\"><center><strong>DELETE SUCCESSFUL</strong></center></span>");
                        break;

                    case 1:
                        Output.WriteLine("<span style=\"color:Red; font-size:1.3em;\"><center><strong>DELETE FAILED</strong><br /><br />Insufficient user permissions to perform delete</center></span>");
                        break;

                    case 2:
                        Output.WriteLine("<span style=\"color:Red; font-size:1.3em;\"><center><strong>DELETE FAILED</strong><br /><br />Item indicated does not exists</center></span>");
                        break;

                    case 3:
                        Output.WriteLine("<span style=\"color:Red; font-size:1.3em;\"><center><strong>DELETE FAILED</strong><br /><br />Error while performing delete in database</center></span>");
                        break;

                    case 4:
                        Output.WriteLine("<span style=\"color:Red; font-size:1.3em;\"><center><strong>DELETE PARTIALLY SUCCESSFUL</strong><br /><br />Unable to move all files to the DELETED folder</center></span>");
                        break;
                }

                Output.WriteLine("</blockquote>");
                Output.WriteLine("</div>");
                Output.WriteLine("<br /><br />");
            }
        }
    }
}

