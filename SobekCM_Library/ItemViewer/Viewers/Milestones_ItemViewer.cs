using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.Items;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.Database;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Tracking;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Milestones item viewer prototyper, which is used to check to see if a user has access to view the milestone information
    /// for a digital resource, and to create the viewer itself if the user selects that option </summary>
    public class Milestones_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Milestones_ItemViewer_Prototyper class </summary>
        public Milestones_ItemViewer_Prototyper()
        {
            ViewerType = "MILESTONES";
            ViewerCode = "milestones";
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
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            // Do nothing since this is already handed and added to the menu by the MANAGE MENU item viewer and INTERNAL header
        }

        /// <summary> Creates and returns the an instance of the <see cref="Milestones_ItemViewer"/> class for showing the
        /// milestones tracking for a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Milestones_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Milestones_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer);
        }
    }

    /// <summary> Item viewer displays the milestone tracking information for a digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Milestones_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly Item_Tracking_Details trackingDetails;

        /// <summary> Constructor for a new instance of the Milestones_ItemViewer class, used display
        /// the tracking milestones information for a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Milestones_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            // Get the milestones information
            Tracer.Add_Trace("Milestones_ItemViewer.Constructor", "Try to pull the tracking details for this item");
            trackingDetails = SobekEngineClient.Items.Get_Item_Tracking_Work_History(BriefItem.BibID, BriefItem.VID, Tracer);
            if (trackingDetails == null)
            {
                Tracer.Add_Trace("Milestones_ItemViewer.Constructor", "Unable to pull tracking details");
                CurrentRequest.Mode = Display_Mode_Enum.Error;
                CurrentRequest.Error_Message = "Internal Error : Unable to pull tracking information for " + BriefItem.BibID + ":" + BriefItem.VID;
                return;
            }
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
            Tracer.Add_Trace("Milestones_ItemViewer.Write_Main_Viewer_Section", "");

            // Add the HTML for the image
            Output.WriteLine("<!-- MILESTONES ITEM VIEWER OUTPUT -->");

            // Start the citation table
            Output.WriteLine("  <td align=\"left\"><span class=\"sbkTrk_ViewerTitle\">Tracking Information</span></td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("<tr>");
            Output.WriteLine("  <td class=\"sbkTrk_MainArea\">");

            // Add the tabs for related admin viewers (if they exist)
            Tracking_ItemViewer.Write_Tracking_Tabs(Output, CurrentRequest, BriefItem);

            Tracer.Add_Trace("Milestones_ItemViewer.Archives_String", "Displaying milestone information");

            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
            Output.WriteLine("<span style=\"font-size:1.1em\">");
            Output.WriteLine("<blockquote>");
            Output.WriteLine("<table width=\"450px\">");
            Output.WriteLine("<tr height=\"20px\" bgcolor=\"#7d90d5\"><td colspan=\"3\"><span style=\"color: White\"><strong> &nbsp; DIGITIZATION MILESTONES</strong></span></td></tr>");
            if (trackingDetails.Milestone_DigitalAcquisition.HasValue)
            {
                Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Digital Acquisition</td><td>" + trackingDetails.Milestone_DigitalAcquisition.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Digital Acquisition</td><td>n/a</td></tr>");
            }
            Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            if (trackingDetails.Milestone_ImageProcessing.HasValue)
            {
                Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Post-Acquisition Processing</td><td>" + trackingDetails.Milestone_ImageProcessing.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Post-Acquisition Processing</td><td>n/a</td></tr>");
            }
            Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            if (trackingDetails.Milestone_QualityControl.HasValue)
            {
                Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Quality Control Performed</td><td>" + trackingDetails.Milestone_QualityControl.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Quality Control Performed</td><td>n/a</td></tr>");
            }
            Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            if (trackingDetails.Milestone_OnlineComplete.HasValue)
            {
                Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Online Complete</td><td>" + trackingDetails.Milestone_OnlineComplete.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Online Complete</td><td>n/a</td></tr>");
            }
            Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
            Output.WriteLine("<br />");

            //Output.WriteLine("<table width=\"450px\">");
            //Output.WriteLine("<tr height=\"20px\" bgcolor=\"#7d90d5\"><td colspan=\"3\"><span style=\"color: White\"><strong> &nbsp; PHYSICAL MATERIAL MILESTONES</strong></span></td></tr>");
            //if (trackingDetails.Material_ReceivedDate.HasValue)
            //{
            //    if (currentItem.Tracking.Material_Rec_Date_Estimated)
            //        Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Materials Received</td><td>" + currentItem.Tracking.Material_Received_Date.Value.ToShortDateString() + " (estimated) </td></tr>");
            //    else
            //        Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Materials Received</td><td>" + trackingDetails.Material_ReceivedDate.Value.ToShortDateString() + "</td></tr>");
            //}
            //else
            //{
            //    Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Materials Received</td><td>n/a</td></tr>");
            //}
            //Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            //if (trackingDetails.Disposition_Date.HasValue)
            //{
            //    Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Disposition Date</td><td>" + trackingDetails.Disposition_Date.Value.ToShortDateString() + "</td></tr>");
            //}
            //else
            //{
            //    Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Disposition Date</td><td>n/a</td></tr>");
            //}
            //Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            //if (currentItem.Tracking.Tracking_Box.Length > 0)
            //{
            //    Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Tracking Box</td><td>" + currentItem.Tracking.Tracking_Box + "</td></tr>");
            //    Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
            //}

            //Output.WriteLine("</table>");
            //Output.WriteLine("<br />");
            //Output.WriteLine("<br />");

            //if ((currentItem.Tracking.Born_Digital) || (currentItem.Tracking.Disposition_Advice > 0))
            //{
            //    Output.WriteLine("<table width=\"450px\">");
            //    Output.WriteLine("<tr height=\"20px\" bgcolor=\"#7d90d5\"><td colspan=\"3\"><span style=\"color: White\"><strong> &nbsp; PHYSICAL MATERIAL RELATED FIELDS</strong></span></td></tr>");
            //    if (currentItem.Tracking.Born_Digital)
            //    {
            //        Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Born Digital</td><td>&nbsp;</td></tr>");
            //        Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            //    }

            //    if (currentItem.Tracking.Disposition_Advice > 0)
            //    {
            //        Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Disposition Advice</td><td>" + currentItem.Tracking.Disposition_Advice + "</td></tr>");
            //        Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
            //    }

            //    Output.WriteLine("</table>");
            //    Output.WriteLine("<br />");
            //    Output.WriteLine("<br />");
            //}

            //Output.WriteLine("<table width=\"450px\">");
            //Output.WriteLine("<tr height=\"20px\" bgcolor=\"#7d90d5\"><td colspan=\"3\"><span style=\"color: White\"><strong> &nbsp; ARCHIVING MILESTONES</strong></span></td></tr>");
            //if ((!currentItem.Tracking.Locally_Archived) && (!currentItem.Tracking.Remotely_Archived))
            //{
            //    Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>NOT ARCHIVED</td><td>&nbsp;</td></tr>");
            //    Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            //}
            //else
            //{
            //    if (currentItem.Tracking.Locally_Archived)
            //    {
            //        Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Locally Stored on CD or Tape</td><td>&nbsp;</td></tr>");
            //        Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            //    }

            //    if (currentItem.Tracking.Remotely_Archived)
            //    {
            //        Output.WriteLine("<tr><td width=\"25px\">&nbsp;</td><td>Archived Remotely (FDA)</td><td>&nbsp;</td></tr>");
            //        Output.WriteLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
            //    }
            //}

            //Output.WriteLine("</table>");
            //Output.WriteLine("<br />");
            //Output.WriteLine("<br />");

            Output.WriteLine("</blockquote>");
            Output.WriteLine("</span>");
            Output.WriteLine("<br />");

            Output.WriteLine("  </td>");
            Output.WriteLine("  <!-- END MILESTONES VIEWER OUTPUT -->");
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
