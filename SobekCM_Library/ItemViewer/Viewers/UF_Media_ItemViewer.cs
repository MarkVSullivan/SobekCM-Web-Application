using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.Database;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> UF-specific media item viewer prototyper, which is used to check to see if a user has access to view the list of 
    /// archived CDs and DVDs for a digital resource, and to create the viewer itself if the user selects that option </summary>
    public class UF_Media_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the UF_Media_ItemViewer_Prototyper class </summary>
        public UF_Media_ItemViewer_Prototyper()
        {
            ViewerType = "MEDIA";
            ViewerCode = "media";
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
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems, bool IpRestricted )
        {
            // Do nothing since this is already handed and added to the menu by the MANAGE MENU item viewer and INTERNAL header
        }

        /// <summary> Creates and returns the an instance of the <see cref="UF_Media_ItemViewer"/> class for showing the
        /// list of archived CDs and DVDs for a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="UF_Media_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new UF_Media_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer);
        }
    }

    /// <summary> Item viewer displays the list of archived CDs and DVDs for a digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class UF_Media_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly DataTable mediaTable;

        /// <summary> Constructor for a new instance of the UF_Media_ItemViewer class, used display
        /// the tracking milestones information for a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public UF_Media_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            // Get the archives information
            Tracer.Add_Trace("UF_Media_ItemViewer.Constructor", "Try to pull the archives details for this item");
            DataSet data = SobekCM_Database.Tracking_Get_History_Archives(BriefItem.Web.ItemID, Tracer);
            if ((data == null) || (data.Tables.Count < 1))
            {
                Tracer.Add_Trace("Constructor.Constructor", "Unable to pull tracking details");
                CurrentRequest.Mode = Display_Mode_Enum.Error;
                CurrentRequest.Error_Message = "Internal Error : Unable to pull tracking information for " + BriefItem.BibID + ":" + BriefItem.VID;
                return;
            }

            // Save this table 
            mediaTable = data.Tables[0];
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
            Tracer.Add_Trace("UF_Media_ItemViewer.Write_Main_Viewer_Section", "");

            // Add the HTML for the image
            Output.WriteLine("<!-- (UF) MEDIA ITEM VIEWER OUTPUT -->");

            // Start the citation table
            Output.WriteLine("  <td align=\"left\"><span class=\"sbkTrk_ViewerTitle\">Tracking Information</span></td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("<tr>");
            Output.WriteLine("  <td class=\"sbkTrk_MainArea\">");

            // Add the tabs for related admin viewers (if they exist)
            Tracking_ItemViewer.Write_Tracking_Tabs(Output, CurrentRequest, BriefItem);

            Tracer.Add_Trace("Tracking_ItemViewer.Media_String", "Displaying media archive information");
            if ((mediaTable == null) || (mediaTable.Rows.Count == 0))
            {
                Output.WriteLine("<br /><br /><br /><center><strong>ITEM IS NOT ARCHIVED TO MEDIA</strong></center><br /><br /><br />");
            }
            else
            {
                Output.WriteLine("<br />");
                Output.WriteLine("<table border=\"1px\" cellpadding=\"5px\" cellspacing=\"0px\" rules=\"cols\" frame=\"void\" bordercolor=\"#e7e7e7\" >");
                Output.WriteLine("  <tr align=\"center\" bgcolor=\"#0022a7\"><td colspan=\"5\"><span style=\"color: White\"><b>CD/DVD ARCHIVE</b></span></td></tr>");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\">");
                Output.WriteLine("    <th><span style=\"color: White\">CD/DVD NUMBER</span></th>");
                Output.WriteLine("    <th width=\"350px\"><span style=\"color: White\">FILE RANGE</span></th>");
                Output.WriteLine("    <th><span style=\"color: White\">IMAGES</span></th>");
                Output.WriteLine("    <th><span style=\"color: White\">SIZE</span></th>");
                Output.WriteLine("    <th><span style=\"color: White\">DATE BURNED</span></th>");
                Output.WriteLine("  </tr>");

                foreach (DataRow thisRow in mediaTable.Rows)
                {
                    Output.WriteLine("  <tr>");

                    // Add the CD/DVD number
                    Output.WriteLine("    <td>" + thisRow[0] + "</td>");

                    // Add file range
                    if ((thisRow[1] != DBNull.Value) && (thisRow[1].ToString().Length > 0))
                        Output.WriteLine("    <td width=\"350px\">" + thisRow[1] + "</td>");
                    else
                        Output.WriteLine("    <td width=\"350px\">&nbsp;</td>");

                    // Add images
                    if ((thisRow[2] != DBNull.Value) && (thisRow[2].ToString().Length > 0))
                        Output.WriteLine("    <td>" + thisRow[2] + "</td>");
                    else
                        Output.WriteLine("    <td>&nbsp;</td>");

                    // Add size
                    if ((thisRow[3] != DBNull.Value) && (thisRow[3].ToString().Length > 0))
                        Output.WriteLine("    <td>" + thisRow[3] + "</td>");
                    else
                        Output.WriteLine("    <td>&nbsp;</td>");

                    // Add date burned
                    if ((thisRow[4] != DBNull.Value) && (thisRow[4].ToString().Length > 0))
                    {
                        DateTime tempDate;
                        if (DateTime.TryParse(thisRow[4].ToString(), out tempDate))
                            Output.WriteLine("    <td>" + tempDate.ToShortDateString() + "</td>");
                        else
                            Output.WriteLine("    <td>&nbsp;</td>");
                    }
                    else
                        Output.WriteLine("    <td>&nbsp;</td>");

                    Output.WriteLine("  </tr>");
                }
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"5\"></td></tr>");
                Output.WriteLine("</table>");
            }

            Output.WriteLine("<br /> <br />");


            Output.WriteLine("  </td>");
            Output.WriteLine("  <!-- END (UF) MEDIA VIEWER OUTPUT -->");
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
