#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Item viewer displays the menu of manage options a user has for a digital resource </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class ManageMenu_ItemViewer : abstractItemViewer
	{
		/// <summary> Constructor for a new instance of the ManageMenu_ItemViewer class </summary>
		/// <param name="Current_Object"> Digital resource to display </param>
		/// <param name="Current_User"> Current user for this session </param>
		/// <param name="Current_Mode"> Navigation object which encapsulates the user's current request </param>
		public ManageMenu_ItemViewer(SobekCM_Item Current_Object, User_Object Current_User, SobekCM_Navigation_Object Current_Mode)
		{
			// Save the current user and current mode information (this is usually populated AFTER the constructor completes, 
			// but in this case (QC viewer) we need the information for early processing
			CurrentMode = Current_Mode;
			CurrentUser = Current_User;
			CurrentItem = Current_Object;

			// Determine if this user can edit this item
			if (CurrentUser == null)
			{
				Current_Mode.ViewerCode = String.Empty;
				Current_Mode.Redirect();
				return;
			}
			else
			{
				bool userCanEditItem = CurrentUser.Can_Edit_This_Item(CurrentItem);
				if (!userCanEditItem)
				{
					Current_Mode.ViewerCode = String.Empty;
					Current_Mode.Redirect();
					return;
				}
			}


		}

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Manage"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
			get { return ItemViewer_Type_Enum.Manage; }
        }
        
        /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <remarks> This always returns the value TRUE for this viewer </remarks>
        public override bool Override_On_Checked_Out
        {
            get
            {
                return false;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This returns the width of the jpeg file to display or 500, whichever is larger </value>
		public override int Viewer_Width
		{
			get {
			    return 700;
			}
		}

		/// <summary> Gets the number of pages for this viewer </summary>
		/// <value> This is a single page viewer, so this property always returns the value 1</value>
		public override int PageCount
		{
			get
			{
				return 1;
			}
		}

		/// <summary> Gets the flag that indicates if the page selector should be shown </summary>
		/// <value> This is a single page viewer, so this property always returns NONE</value>
		public override ItemViewer_PageSelector_Type_Enum Page_Selector
		{
			get
			{
				return ItemViewer_PageSelector_Type_Enum.NONE;
			}
		}

		/// <summary> Stream to which to write the HTML for this subwriter  </summary>
		/// <param name="Output"> Response stream for the item viewer to write directly to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("ManageMenu_ItemViewer.Write_Main_Viewer_Section", "");
			}

			string currentViewerCode = CurrentMode.ViewerCode;

			// Add the HTML for the image
			Output.WriteLine("<!-- MANAGE MENU ITEM VIEWER OUTPUT -->");


			if (CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL)
			{
				// Start the citation table
				Output.WriteLine("  <td align=\"left\"><div class=\"sbkMmiv_ViewerTitle\">Manage this Item</div></td>");
				Output.WriteLine("</tr>");
				Output.WriteLine("<tr>");
				Output.WriteLine("  <td class=\"sbkMmiv_MainArea\">");


				Output.WriteLine("\t\t\t<table id=\"sbkMmiv_MainTable\">");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_HeaderRow\"><td colspan=\"3\">How would you like to manage this item today?</td></tr>");


				// Add ability to edit metadata for this item
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
				CurrentMode.My_Sobek_SubMode = "1";
				string url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "edit_metadata_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Edit Item Metadata</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">Edit the information about this item which appears in the citation/description.  This is basic information about the original item and this digital manifestation.</div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");

				// Add ability to edit behaviors for this item
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
				CurrentMode.My_Sobek_SubMode = "1";
				url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "edit_behaviors_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Edit Item Behaviors</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">Change the way this item behaves in this library, including which aggregations it appears under, the wordmarks to the left, and which viewer types are publicly accessible.</div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");

				// Add ability to perform QC ( manage pages and divisions) for this item
				CurrentMode.Mode = Display_Mode_Enum.Item_Display;
				CurrentMode.ViewerCode = "qc";
				url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "qc_button_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Manage Pages and Divisions (Quality Control)</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">Reorder page images, name pages, assign divisions, and delete and add new page images to this item.</div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");


				// Add ability to view work history for this item
				CurrentMode.Mode = Display_Mode_Enum.Item_Display;
				CurrentMode.ViewerCode = "tracking";
				url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "view_work_log_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">View Work History</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">View the history of all work performed on this item.  From this view, you can also see any digitization milestones and digital resource file information.</div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");


				// Add ability to edit behaviors for this item
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
				url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "file_management_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Manage Download Files</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">Upload new files for download or remove existing files that are attached to this item for download.  This generally includes everything except for the page images.</div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");

				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_HeaderRow\"><td colspan=\"3\"></td></tr>");

				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_HeaderRow\"><td colspan=\"3\">In addition, the following changes can be made at the item group level:</td></tr>");

				// Add ability to edit GROUP behaviors for this group
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Behaviors;
				CurrentMode.My_Sobek_SubMode = "1";
				url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "edit_behaviors_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Edit Item Group Behaviors</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">Set the title under which all of these items appear in search results and set the web skins under which all these items should appear.</div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");

				// Add ability to add new volume for this group
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Add_Volume;
				CurrentMode.My_Sobek_SubMode = "1";
				url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "add_volume_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Add New Volume</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">Add a new, related volume to this item group.<br /><br /></div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");

				if ((CurrentItem.Web.Siblings.HasValue) && (CurrentItem.Web.Siblings > 1))
				{
					// Add ability to mass update all items for this group
					CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
					CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Mass_Update_Items;
					CurrentMode.My_Sobek_SubMode = "1";
					url = CurrentMode.Redirect_URL();
					Output.WriteLine("\t\t\t\t<tr>");
					Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
					Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "mass_update_icon.png\" /></a></td>");
					Output.WriteLine("\t\t\t\t\t<td>");
					Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Mass Update Item Behaviors</a>");
					Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">This allows item-level behaviors to be set for all items within this item group, including which aggregations it appears under, the wordmarks to the left, and which viewer types are publicly accessible.</div>");
					Output.WriteLine("\t\t\t\t\t</td>");
					Output.WriteLine("\t\t\t\t</tr>");
					Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");
				}

				Output.WriteLine("\t\t\t</table>");

				Output.WriteLine("\t\t</td>");
			}
			else
			{
				// Start the citation table
				Output.WriteLine("  <td align=\"left\"><div class=\"sbkMmiv_ViewerTitle\">Manage this Item Group</div></td>");
				Output.WriteLine("</tr>");
				Output.WriteLine("<tr>");
				Output.WriteLine("  <td class=\"sbkMmiv_MainArea\">");


				Output.WriteLine("\t\t\t<table id=\"sbkMmiv_MainTable\">");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_HeaderRow\"><td colspan=\"3\">How would you like to manage this item group today?</td></tr>");


				// Add ability to edit GROUP behaviors for this group
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Behaviors;
				CurrentMode.My_Sobek_SubMode = "1";
				string url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "edit_behaviors_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Edit Item Group Behaviors</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">Set the title under which all of these items appear in search results and set the web skins under which all these items should appear.</div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");

				// Add ability to add new volume for this group
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Add_Volume;
				CurrentMode.My_Sobek_SubMode = "1";
				url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "add_volume_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Add New Volume</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">Add a new, related volume to this item group.<br /><br /></div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");

				// Add ability to mass update all items for this group
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Mass_Update_Items;
				CurrentMode.My_Sobek_SubMode = "1";
				url = CurrentMode.Redirect_URL();
				Output.WriteLine("\t\t\t\t<tr>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("\t\t\t\t\t<td style=\"width:60px\"><a href=\"" + url + "\"><img src=\"" + CurrentMode.Default_Images_URL + "mass_update_icon.png\" /></a></td>");
				Output.WriteLine("\t\t\t\t\t<td>");
				Output.WriteLine("\t\t\t\t\t\t<a href=\"" + url + "\">Mass Update Item Behaviors</a>");
				Output.WriteLine("\t\t\t\t\t\t<div class=\"sbkMmiv_Desc\">This allows item-level behaviors to be set for all items within this item group, including which aggregations it appears under, the wordmarks to the left, and which viewer types are publicly accessible.</div>");
				Output.WriteLine("\t\t\t\t\t</td>");
				Output.WriteLine("\t\t\t\t</tr>");
				Output.WriteLine("\t\t\t\t<tr class=\"sbkMmiv_SpacerRow\"><td colspan=\"3\"></td></tr>");

				Output.WriteLine("\t\t\t</table>");

				Output.WriteLine("\t\t</td>");
			}
		}
	}
}
