// HTML5 10/14/2013

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view all existing item aggregationPermissions, select an item aggregation to edit, and add new aggregationPermissions </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to view and edit existing item aggregationPermissions in this digital library</li>
    /// </ul></remarks>
    public class Aggregations_Mgmt_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;

        private readonly string enteredCode;
        private readonly string enteredDescription;
        private readonly bool enteredIsActive;
        private readonly bool enteredIsHidden;
        private readonly string enteredLink;
        private readonly string enteredName;
        private readonly string enteredParent;
        private readonly string enteredShortname;
        private readonly string enteredType;

        /// <summary> Constructor for a new instance of the Aggregations_Mgmt_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public Aggregations_Mgmt_AdminViewer(RequestCache RequestSpecificValues)  : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Aggregations_Mgmt_AdminViewer.Constructor", String.Empty);

            // Set some defaults
            actionMessage = String.Empty;
            enteredCode = String.Empty;
            enteredParent = String.Empty;
            enteredType = String.Empty;
            enteredShortname = String.Empty;
            enteredName = String.Empty;
            enteredDescription = String.Empty;
            enteredIsActive = true;
            enteredIsHidden = true;

            // If the user cannot edit this, go back
            if ((RequestSpecificValues.Current_User == null) || ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin)))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If this is a postback, handle any events first
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    string save_value = form["admin_aggr_tosave"].ToUpper().Trim();
                    string new_aggregation_code = String.Empty;
                    if ( form["admin_aggr_code"] != null )
                        new_aggregation_code = form["admin_aggr_code"].ToUpper().Trim();

                    // Check for reset request as well
                    string reset_aggregation_code = String.Empty;
                    if (form["admin_aggr_reset"] != null)
                        reset_aggregation_code = form["admin_aggr_reset"].ToLower().Trim();

	                string delete_aggregation_code = String.Empty;
					if (form["admin_aggr_delete"] != null)
						delete_aggregation_code = form["admin_aggr_delete"].ToLower().Trim();

					// Was this to delete the aggregation?
					if ( delete_aggregation_code.Length > 0)
					{
						string delete_error;
                        int errorCode = SobekCM_Database.Delete_Item_Aggregation(delete_aggregation_code, RequestSpecificValues.Current_User.Is_System_Admin, RequestSpecificValues.Current_User.Full_Name, RequestSpecificValues.Tracer, out delete_error);
						if (errorCode <= 0)
						{
							string delete_folder = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + "aggregations\\" + delete_aggregation_code;
							if (!SobekCM_File_Utilities.Delete_Folders_Recursively(delete_folder))
                                actionMessage = "Deleted '" + delete_aggregation_code.ToUpper() + "' aggregation<br /><br />Unable to remove aggregation directory<br /><br />Some of the files may be in use";
							else
								actionMessage = "Deleted '" + delete_aggregation_code.ToUpper() + "' aggregation";
						}
						else
						{
							actionMessage = delete_error;
						}


						// Reload the list of all codes, to include this new one and the new hierarchy
						lock (UI_ApplicationCache_Gateway.Aggregations)
						{
                            Engine_Database.Populate_Code_Manager(UI_ApplicationCache_Gateway.Aggregations, RequestSpecificValues.Tracer);
						}
					}


                    // If there is a reset request here, purge the aggregation from the cache
                    if (reset_aggregation_code.Length > 0)
                    {
                        CachedDataManager.Aggregations.Remove_Item_Aggregation(reset_aggregation_code, RequestSpecificValues.Tracer);
                    }

                    // If there was a save value continue to pull the rest of the data
                    if (save_value.Length > 0)
                    {

                        bool is_active = false;
                        bool is_hidden = true;


	                    // Was this to save a new aggregation (from the main page) or edit an existing (from the popup form)?
                        if (save_value == new_aggregation_code)
                        {

                            // Pull the values from the submitted form
                            string new_type = form["admin_aggr_type"];
                            string new_parent = form["admin_aggr_parent"].Trim();
                            string new_name = form["admin_aggr_name"].Trim();
                            string new_shortname = form["admin_aggr_shortname"].Trim();
                            string new_description = form["admin_aggr_desc"].Trim();
                            string new_link = form["admin_aggr_link"].Trim();

                            object temp_object = form["admin_aggr_isactive"];
                            if (temp_object != null)
                            {
                                is_active = true;
                            }

                            temp_object = form["admin_aggr_ishidden"];
                            if (temp_object != null)
                            {
                                is_hidden = false;
                            }

                            // Convert to the integer id for the parent and begin to do checking
                            List<string> errors = new List<string>();
                            int parentid = -1;
                            if (new_parent.Length > 0)
                            {
                                try
                                {
                                    parentid = Convert.ToInt32(new_parent);
                                }
                                catch
                                {
                                    errors.Add("Invalid parent id selected!");
                                }
                            }
                            else
                            {
                                errors.Add("You must select a PARENT for this new aggregation");
                            }

							// Validate the code

							if (new_aggregation_code.Length > 20)
							{
								errors.Add("New aggregation code must be twenty characters long or less");
							}
							else if (new_aggregation_code.Length == 0)
							{
								errors.Add("You must enter a CODE for this item aggregation");
							}
							else if (UI_ApplicationCache_Gateway.Aggregations[new_aggregation_code.ToUpper()] != null)
							{
								errors.Add("New code must be unique... <i>" + new_aggregation_code + "</i> already exists");
							}
                            else if (UI_ApplicationCache_Gateway.Settings.Reserved_Keywords.Contains(new_aggregation_code.ToLower()))
                            {
                                errors.Add("That code is a system-reserved keyword.  Try a different code.");
                            }
                            else
                            {
                                bool alphaNumericTest = new_aggregation_code.All(C => Char.IsLetterOrDigit(C) || C == '_' || C == '-');
                                if (!alphaNumericTest)
                                {
                                    errors.Add("New aggregation code must be only letters and numbers");
                                    new_aggregation_code = new_aggregation_code.Replace("\"", "");
                                }
                            }

                            // Was there a type and name
                            if (new_type.Length == 0)
                            {
                                errors.Add("You must select a TYPE for this new aggregation");
                            }
                            if (new_description.Length == 0)
                            {
                                errors.Add("You must enter a DESCRIPTION for this new aggregation");
                            }
                            if (new_name.Length == 0)
                            {
                                errors.Add("You must enter a NAME for this new aggregation");
                            }
                            else
                            {
                                if (new_shortname.Length == 0)
                                    new_shortname = new_name;
                            }

                            if (errors.Count > 0)
                            {
                                // Create the error message
                                actionMessage = "ERROR: Invalid entry for new item aggregation<br />";
                                foreach (string error in errors)
                                    actionMessage = actionMessage + "<br />" + error;

                                // Save all the values that were entered
                                enteredCode = new_aggregation_code;
                                enteredDescription = new_description;
                                enteredIsActive = is_active;
                                enteredIsHidden = is_hidden;
                                enteredName = new_name;
                                enteredParent = new_parent;
                                enteredShortname = new_shortname;
                                enteredType = new_type;
                                enteredLink = new_link;
                            }
                            else
                            {
                                // Get the correct type
                                string correct_type = "Collection";
                                switch (new_type)
                                {
                                    case "coll":
                                        correct_type = "Collection";
                                        break;

                                    case "group":
                                        correct_type = "Collection Group";
                                        break;

                                    case "subcoll":
                                        correct_type = "SubCollection";
                                        break;

                                    case "inst":
                                        correct_type = "Institution";
                                        break;

                                    case "exhibit":
                                        correct_type = "Exhibit";
                                        break;

                                    case "subinst":
                                        correct_type = "Institutional Division";
                                        break;
                                }
                                // Make sure inst and subinst start with 'i'
                                if (new_type.IndexOf("inst") >= 0)
                                {
                                    if (new_aggregation_code[0] == 'I')
                                        new_aggregation_code = "i" + new_aggregation_code.Substring(1);
                                    if (new_aggregation_code[0] != 'i')
                                        new_aggregation_code = "i" + new_aggregation_code;
                                }

								// Get the thematic heading id (no checks here)
	                            int thematicHeadingId = -1;
								if (form["admin_aggr_heading"] != null)
									thematicHeadingId = Convert.ToInt32(form["admin_aggr_heading"]);

                                string language = Web_Language_Enum_Converter.Enum_To_Code(UI_ApplicationCache_Gateway.Settings.Default_UI_Language);

                                // Try to save the new item aggregation
                                if (Engine_Database.Save_Item_Aggregation(new_aggregation_code, new_name, new_shortname, new_description, thematicHeadingId, correct_type, is_active, is_hidden, new_link, parentid, RequestSpecificValues.Current_User.Full_Name, language, RequestSpecificValues.Tracer))
                                {
                                    // Ensure a folder exists for this, otherwise create one
                                    try
                                    {
	                                    string folder = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + "aggregations\\" + new_aggregation_code.ToLower();
	                                    if (!Directory.Exists(folder))
	                                    {
		                                    // Create this directory and all the subdirectories
		                                    Directory.CreateDirectory(folder);
		                                    Directory.CreateDirectory(folder + "/html");
		                                    Directory.CreateDirectory(folder + "/images");
		                                    Directory.CreateDirectory(folder + "/html/home");
                                            Directory.CreateDirectory(folder + "/html/custom/home");
		                                    Directory.CreateDirectory(folder + "/images/buttons");
		                                    Directory.CreateDirectory(folder + "/images/banners");

                                            // Get the parent name
                                            string link_to_parent = String.Empty;
	                                        Item_Aggregation_Related_Aggregations parentAggr = UI_ApplicationCache_Gateway.Aggregations.Aggregation_By_ID(parentid);
	                                        if (parentAggr != null)
	                                            link_to_parent = "<br />" + Environment.NewLine + " ← Back to <a href=<%BASEURL%>" + parentAggr.Code + "\" alt=\"Return to parent collection\">" + parentAggr.Name + "</a>" + Environment.NewLine;

		                                    // Create a default home text file
		                                    StreamWriter writer = new StreamWriter(folder + "/html/home/text.html");
                                            writer.WriteLine(link_to_parent + "<br />" + Environment.NewLine + "<h3>About " + new_name + "</h3>" + Environment.NewLine + "<p>" + new_description + "</p>" + Environment.NewLine + "<p>To edit this, log on as the aggregation admin and hover over this text to edit it.</p>" + Environment.NewLine + "<br />");
                                             
		                                    writer.Flush();
		                                    writer.Close();

		                                    // Copy the default banner and buttons from images
		                                    if (File.Exists(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_button.png"))
			                                    File.Copy(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_button.png", folder + "/images/buttons/coll.png");
		                                    if (File.Exists(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_button.gif"))
			                                    File.Copy(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_button.gif", folder + "/images/buttons/coll.gif");

                                            // Try to create a new custom banner
                                            bool custom_banner_created = false;
                                            // Create the banner with the name of the collection
                                            if (Directory.Exists(UI_ApplicationCache_Gateway.Settings.Application_Server_Network + "\\default\\banner_images"))
                                            {
                                                try
                                                {
                                                    string[] banners = Directory.GetFiles(UI_ApplicationCache_Gateway.Settings.Application_Server_Network + "\\default\\banner_images", "*.jpg");
                                                    if (banners.Length > 0)
                                                    {
                                                        Random randomizer = new Random();
                                                        string banner_to_use = banners[randomizer.Next(0, banners.Length - 1)];
                                                        Bitmap bitmap = (Bitmap) (Bitmap.FromFile(banner_to_use));

                                                        RectangleF rectf = new RectangleF(30, bitmap.Height - 55, bitmap.Width - 40, 40);
                                                        Graphics g = Graphics.FromImage(bitmap);
                                                        g.SmoothingMode = SmoothingMode.AntiAlias;
                                                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                                        g.DrawString(new_name, new Font("Tahoma", 25, FontStyle.Bold), Brushes.Black, rectf);
                                                        g.Flush();

                                                        string new_file = folder + "/images/banners/coll.jpg";
                                                        if (!File.Exists(new_file))
                                                        {
                                                            bitmap.Save(new_file, ImageFormat.Jpeg);
                                                            custom_banner_created = true;
                                                        }
                                                    }
                                                }
                                                catch (Exception ee)
                                                {
                                                    // Suppress this Error... 
                                                }
                                            }

                                            if ((!custom_banner_created) && (!File.Exists(folder + "/images/banners/coll.jpg")))
                                            {
                                                if (File.Exists(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_banner.jpg"))
                                                    File.Copy(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_banner.jpg", folder + "/images/banners/coll.jpg");
                                            }

		                                    // Now, try to create the item aggregation and write the configuration file
                                            Complete_Item_Aggregation itemAggregation = SobekEngineClient.Aggregations.Get_Complete_Aggregation(new_aggregation_code, false, RequestSpecificValues.Tracer);
		                                    itemAggregation.Write_Configuration_File(UI_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory);
	                                    }
                                    }
                                    catch
                                    {
										actionMessage = "ERROR saving the new item aggregation to the database";
                                    }

                                    // Reload the list of all codes, to include this new one and the new hierarchy
                                    lock (UI_ApplicationCache_Gateway.Aggregations)
                                    {
                                        Engine_Database.Populate_Code_Manager(UI_ApplicationCache_Gateway.Aggregations, RequestSpecificValues.Tracer);
                                    }


                                    if (String.IsNullOrEmpty(actionMessage))
                                    {
                                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                                        RequestSpecificValues.Current_Mode.Aggregation = new_aggregation_code;
                                        actionMessage = "New item aggregation (" + new_aggregation_code.ToUpper() + ") saved successfully.<br /><br /><a href=\"" + UrlWriterHelper.Redirect_URL( RequestSpecificValues.Current_Mode, true ) + "\" target=\"" + new_aggregation_code + "_AGGR\">Click here to view the new aggregation</a>";
                                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
                                        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
                                    }
                                }
                                else
                                {
                                    actionMessage = "ERROR saving the new item aggregation to the database";
                                }

                                // Clear all aggregation information (and thematic heading info) from the cache as well
                                CachedDataManager.Aggregations.Clear();
                            }
                        }
                    }
                }
                catch 
                {
                    actionMessage = "General error while reading postback information";
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'HTML Skins' </value>
        public override string Web_Title
        {
            get { return "Aggregation Management"; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aggregations_Mgmt_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Aggregations_Mgmt_AdminViewer.Write_ItemNavForm_Closing", "");

			Output.WriteLine("<script type=\"text/javascript\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_tosave\" name=\"admin_aggr_tosave\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_reset\" name=\"admin_aggr_reset\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_delete\" name=\"admin_aggr_delete\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- Aggregations_Mgmt_AdminViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (actionMessage.Length > 0)
			{
			    if (actionMessage.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) >= 0)
			    {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageError\">" + actionMessage + "</div>");
			    }
			    else
			    {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageSuccess\">" + actionMessage + "</div>");
			    }

			}

            Output.WriteLine("  <p>For clarification of any terms on this form, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/aggregations\" target=\"ADMIN_INTERFACE_HELP\" >click here to view the help page</a>.</p>");


            // Find the matching type to display
            int index = 0;
            if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length > 0)
            {
                Int32.TryParse(RequestSpecificValues.Current_Mode.My_Sobek_SubMode, out index);
            }

            if ((index <= 0) || (index > UI_ApplicationCache_Gateway.Aggregations.Types_Count))
            {

                Output.WriteLine("  <h2>New Item Aggregation</h2>");

                Output.WriteLine("  <div class=\"sbkAsav_NewDiv\">");
                Output.WriteLine("    <table class=\"sbkAdm_PopupTable\">");

                // Add line for aggregation code and aggregation type
	            Output.WriteLine("      <tr>");
				Output.WriteLine("        <td style=\"width:120px;\"><label for=\"admin_aggr_code\">Code:</label></td>");
                Output.WriteLine("        <td><input class=\"sbkAsav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_code\" id=\"admin_aggr_code\" type=\"text\" value=\"" + enteredCode + "\" /></td>");
	            Output.WriteLine("        <td style=\"width:300px;text-align:right;\">");
				Output.WriteLine("          <label for=\"admin_aggr_type\">Type:</label> &nbsp; ");
				Output.WriteLine("          <select class=\"sbkAsav_select \" name=\"admin_aggr_type\" id=\"admin_aggr_type\">");
                if (enteredType == String.Empty)
                    Output.WriteLine("            <option value=\"\" selected=\"selected\" ></option>");

                Output.WriteLine(enteredType == "coll"
                                 ? "            <option value=\"coll\" selected=\"selected\" >Collection</option>"
                                 : "            <option value=\"coll\">Collection</option>");

                Output.WriteLine(enteredType == "group"
                                 ? "            <option value=\"group\" selected=\"selected\" >Collection Group</option>"
                                 : "            <option value=\"group\">Collection Group</option>");

                Output.WriteLine(enteredType == "exhibit"
                                 ? "            <option value=\"exhibit\" selected=\"selected\" >Exhibit</option>"
                                 : "            <option value=\"exhibit\">Exhibit</option>");

                Output.WriteLine(enteredType == "inst"
                                 ? "            <option value=\"inst\" selected=\"selected\" >Institution</option>"
                                 : "            <option value=\"inst\">Institution</option>");

                Output.WriteLine(enteredType == "subinst"
                                 ? "            <option value=\"subinst\" selected=\"selected\" >Institutional Division</option>"
                                 : "            <option value=\"subinst\">Institutional Division</option>");

                Output.WriteLine(enteredType == "subcoll"
                                 ? "            <option value=\"subcoll\" selected=\"selected\" >SubCollection</option>"
                                 : "            <option value=\"subcoll\">SubCollection</option>");

	            Output.WriteLine("          </select>");
	            Output.WriteLine("        </td>");
				Output.WriteLine("      </tr>");

                // Add the parent line
	            Output.WriteLine("      <tr>");
	            Output.WriteLine("        <td>");
				Output.WriteLine("          <label for=\"admin_aggr_parent\">Parent:</label></td><td colspan=\"2\">");
				Output.WriteLine("          <select class=\"sbkAsav_select_large\" name=\"admin_aggr_parent\" id=\"admin_aggr_parent\">");
                if (enteredParent == String.Empty)
                    Output.WriteLine("            <option value=\"\" selected=\"selected\" ></option>");
                foreach (Item_Aggregation_Related_Aggregations thisAggr in UI_ApplicationCache_Gateway.Aggregations.All_Aggregations)
                {
                    if (enteredParent == thisAggr.ID.ToString())
                    {
                        Output.WriteLine("            <option value=\"" + thisAggr.ID + "\" selected=\"selected\" >" + thisAggr.Code + " - " + thisAggr.Name + "</option>");
                    }
                    else
                    {
                        Output.WriteLine("            <option value=\"" + thisAggr.ID + "\" >" + thisAggr.Code + " - " + thisAggr.Name + "</option>");
                    }
                }
	            Output.WriteLine("          </select>");
	            Output.WriteLine("        </td>");
				Output.WriteLine("      </tr>");

                // Add the full name line
                Output.WriteLine("      <tr><td><label for=\"admin_aggr_name\">Name (full):</label></td><td colspan=\"2\"><input class=\"sbkAsav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_name\" id=\"admin_aggr_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredName) + "\" /></td></tr>");

                // Add the short name line
                Output.WriteLine("      <tr><td><label for=\"admin_aggr_shortname\">Name (short):</label></td><td colspan=\"2\"><input class=\"sbkAsav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredShortname) + "\" /></td></tr>");

                // Add the link line
                Output.WriteLine("      <tr><td><label for=\"admin_aggr_link\">External Link:</label></td><td colspan=\"2\"><input class=\"sbkAsav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_link\" id=\"admin_aggr_link\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredLink) + "\" /></td></tr>");

				// Add the thematic heading line
	            Output.WriteLine("      <tr>");
				Output.WriteLine("        <td><label for=\"admin_aggr_heading\">Thematic Heading:</label></td>");
				Output.WriteLine("        <td colspan=\"2\">");
				Output.WriteLine("          <select class=\"sbkAsav_select_large\" name=\"admin_aggr_heading\" id=\"admin_aggr_heading\">");
				Output.WriteLine("            <option value=\"-1\" selected=\"selected\" ></option>");
				foreach (Thematic_Heading thisHeading in UI_ApplicationCache_Gateway.Thematic_Headings)
				{
					Output.Write("            <option value=\"" + thisHeading.ID + "\">" + HttpUtility.HtmlEncode(thisHeading.Text) + "</option>");
				}
	            Output.WriteLine("          </select>");
	            Output.WriteLine("        </td>");
				Output.WriteLine("      </tr>");



                // Add the description box
                Output.WriteLine("      <tr style=\"vertical-align:top\"><td><label for=\"admin_aggr_desc\">Description:</label></td><td colspan=\"2\"><textarea rows=\"6\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\" class=\"sbkAsav_input sbkAdmin_Focusable\">" + HttpUtility.HtmlEncode(enteredDescription) + "</textarea></td></tr>");

                // Add checkboxes for is active and is hidden
                Output.Write(enteredIsActive
								 ? "          <tr style=\"height:30px\"><td>Behavior:</td><td colspan=\"2\"><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" checked=\"checked\" /> <label for=\"admin_aggr_isactive\">Active?</label></td></tr> "
								 : "          <tr style=\"height:30px\"><td>Behavior:</td><td colspan=\"2\"><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" /> <label for=\"admin_aggr_isactive\">Active?</label></td></tr> ");


                Output.Write(enteredIsHidden
								 ? "          <tr><td></td><td colspan=\"2\"><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" checked=\"checked\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label></td></tr> "
								 : "          <tr><td></td><td colspan=\"2\"><input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label></td></tr> ");
 

				// Add the SAVE button
				Output.WriteLine("      <tr style=\"height:30px; text-align: center;\"><td colspan=\"3\"><button title=\"Save new item aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_aggr();\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button></td></tr>");
				Output.WriteLine("    </table>");
				Output.WriteLine("  </div>");
				Output.WriteLine();

                Output.WriteLine("  <h2 id=\"list\">Existing Item Aggregations</h2>");
                Output.WriteLine("  <p>Select a type below to view all matching item aggregations:</p>");
                Output.WriteLine("  <ul class=\"sbkAsav_List\">");
                int i = 1;
                foreach (string thisType in UI_ApplicationCache_Gateway.Aggregations.All_Types)
                {
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = i.ToString();
                    Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" >" + thisType.ToUpper() + "</a></li>");
                    i++;
                }
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("  </ul>");



            }
            else
            {
                string aggregationType = UI_ApplicationCache_Gateway.Aggregations.All_Types[index - 1];

                Output.WriteLine("  <h2>Other Actions</h2>");
                Output.WriteLine("  <ul class=\"sbkAsav_List\">");
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Add new item aggregation</a></li>");
                Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "#list\">View different aggregations</a></li>");
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = index.ToString();
                Output.WriteLine("  </ul>");
				Output.WriteLine();

                Output.WriteLine("  <h2>Existing " + aggregationType + "s</h2>");

                Output.WriteLine("  <table class=\"sbkAsav_Table sbkAdm_Table\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkAsav_TableHeader1\">ACTIONS</th>");
                Output.WriteLine("      <th class=\"sbkAsav_TableHeader2\">CODE</th>");
                Output.WriteLine("      <th class=\"sbkAsav_TableHeader3\">NAME</th>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"3\"></td></tr>");

                Output.WriteLine("    <tr class=\"sbkAsav_TableTitleRow\" style=\"\" >");
                if ((aggregationType.Length > 0) && (aggregationType[aggregationType.Length - 1] != 'S'))
                {
                    Output.WriteLine("      <td colspan=\"3\">" + aggregationType.ToUpper() + "S</td>");
                }
                else
                {
                    Output.WriteLine("      <td colspan=\"3\">" + aggregationType.ToUpper() + "</td>");
                }
                Output.WriteLine("    </tr>");

                // Show all matching rows
                string last_code = String.Empty;
                foreach (Item_Aggregation_Related_Aggregations thisAggr in UI_ApplicationCache_Gateway.Aggregations.Aggregations_By_Type(aggregationType))
                {
                    if (thisAggr.Code != last_code)
                    {
                        last_code = thisAggr.Code;

                        // Build the action links
                        Output.WriteLine("    <tr>");
                        Output.Write("      <td class=\"sbkAdm_ActionLink\" >( ");
                        
                        Output.Write("<a title=\"Click to edit this item aggregation\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/admin/editaggr/" + thisAggr.Code + "\">edit</a> | ");

                        if (thisAggr.Active)
                            Output.Write("<a title=\"Click to view this item aggregation\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/" + thisAggr.Code + "\">view</a> | ");
                        else
                            Output.Write("view | ");

                        if ( String.Compare(thisAggr.Code, "ALL", StringComparison.InvariantCultureIgnoreCase) != 0 )
						    Output.Write("<a title=\"Click to delete this item aggregation\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_aggr('" + thisAggr.Code + "');\">delete</a> | ");

                        Output.WriteLine("<a title=\"Click to reset the instance in the application cache\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return reset_aggr('" + thisAggr.Code + "');\">reset</a> )</td>");

                        // Add the rest of the row with data
                        Output.WriteLine("      <td>" + thisAggr.Code + "</td>");
                        Output.WriteLine("      <td>" + thisAggr.Name + "</td>");
                        Output.WriteLine("    </tr>");
                        Output.WriteLine("    <tr><td class=\"sbkAdm_TableRule\" colspan=\"3\"></td></tr>");
                    }
                }

                Output.WriteLine("  </table>");
            }

            Output.WriteLine("  <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}
