using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.Items;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Tracking item viewer prototyper, which is used to check to see if a user has access to view the full work history
    ///  for a digital resource, and to create the viewer itself if the user selects that option </summary>
    public class Tracking_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Tracking_ItemViewer_Prototyper class </summary>
        public Tracking_ItemViewer_Prototyper()
        {
            ViewerType = "TRACKING";
            ViewerCode = "history";
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

        /// <summary> Creates and returns the an instance of the <see cref="Tracking_ItemViewer"/> class for showing the
        /// full workflow history for a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Tracking_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Tracking_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer);
        }
    }

    /// <summary> Item viewer displays the full workflow history information for a digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Tracking_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly Item_Tracking_Details trackingDetails;

        /// <summary> Constructor for a new instance of the Tracking_ItemViewer class, used display
        /// the full workflow history information for a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Tracking_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer )
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            // Get the tracking information
            Tracer.Add_Trace("Tracking_ItemViewer.Constructor", "Try to pull the tracking details for this item");
            trackingDetails = SobekEngineClient.Items.Get_Item_Tracking_Work_History(BriefItem.BibID, BriefItem.VID, Tracer);
            if (trackingDetails == null)
            {
                Tracer.Add_Trace("Tracking_ItemViewer.Constructor", "Unable to pull tracking details");
                CurrentRequest.Mode = Display_Mode_Enum.Error;
                CurrentRequest.Error_Message = "Internal Error : Unable to pull tracking information for " + BriefItem.BibID + ":" + BriefItem.VID;
            }
        }

        #region Static method add the tracking tabs for all related viewers

        /// <summary> Writes the tracking tabs for all these related viewers </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="BriefItem"> Digital resource object </param>
        public static void Write_Tracking_Tabs(TextWriter Output, Navigation_Object CurrentRequest, BriefItemInfo BriefItem )
        {
            // Set the text
            const string MILESTONES_VIEW = "MILESTONES";
            const string TRACKING_VIEW = "HISTORY";
            const string MEDIA_VIEW = "MEDIA";
            const string ARCHIVE_VIEW = "ARCHIVES";
            const string DIRECTORY_VIEW = "DIRECTORY";

            // Is this bib level?
            if (String.Compare(BriefItem.Type, "BIB_LEVEL", StringComparison.OrdinalIgnoreCase) == 0)
                return;

            // Add the tabs for the different citation information
            string viewer_code = CurrentRequest.ViewerCode;
            Output.WriteLine("    <div id=\"sbkTrk_ViewSelectRow\">");
            Output.WriteLine("      <ul class=\"sbk_FauxDownwardTabsList\">");

            // Get the current viewer code
            string viewerCode = CurrentRequest.ViewerCode;

            // Include milestones?
            if (BriefItem.UI.Includes_Viewer_Type("MILESTONES"))
            {
                string milestones_code = ItemViewer_Factory.ViewCode_From_ViewType("MILESTONES");
                if ( String.Compare(viewerCode, milestones_code, StringComparison.OrdinalIgnoreCase) == 0 )
                {
                    Output.WriteLine("        <li class=\"current\">" + MILESTONES_VIEW + "</li>");
                }
                else
                {
                    Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, milestones_code) + "\">" + MILESTONES_VIEW + "</a></li>");
                } 
            }

            // Include tracking? (Okay, this is probably redundant since this is IN the tracking itemviewer)
            if (BriefItem.UI.Includes_Viewer_Type("TRACKING"))
            {
                string tracking_code = ItemViewer_Factory.ViewCode_From_ViewType("TRACKING");
                if (String.Compare(viewerCode, tracking_code, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Output.WriteLine("        <li class=\"current\">" + TRACKING_VIEW + "</li>");
                }
                else
                {
                    Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, tracking_code) + "\">" + TRACKING_VIEW + "</a></li>");
                }
            }

            // Include UF-specific media viewer?
            if (BriefItem.UI.Includes_Viewer_Type("MEDIA"))
            {
                string media_code = ItemViewer_Factory.ViewCode_From_ViewType("MEDIA");
                if (String.Compare(viewerCode, media_code, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Output.WriteLine("        <li class=\"current\">" + MEDIA_VIEW + "</li>");
                }
                else
                {
                    Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, media_code) + "\">" + MEDIA_VIEW + "</a></li>");
                }
            }

            // Include UF-specific archives viewer?
            if (BriefItem.UI.Includes_Viewer_Type("TIVOLI"))
            {
                string archives_code = ItemViewer_Factory.ViewCode_From_ViewType("TIVOLI");
                if (String.Compare(viewerCode, archives_code, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Output.WriteLine("        <li class=\"current\">" + ARCHIVE_VIEW + "</li>");
                }
                else
                {
                    Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, archives_code) + "\">" + ARCHIVE_VIEW + "</a></li>");
                }
            }

            // Include resource files viewer?
            if (BriefItem.UI.Includes_Viewer_Type("DIRECTORY"))
            {
                string directory_code = ItemViewer_Factory.ViewCode_From_ViewType("DIRECTORY");
                if (String.Compare(viewerCode, directory_code, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Output.WriteLine("        <li class=\"current\">" + DIRECTORY_VIEW + "</li>");
                }
                else
                {
                    Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, directory_code) + "\">" + DIRECTORY_VIEW + "</a></li>");
                }
            }

            Output.WriteLine("              </div>");

            CurrentRequest.ViewerCode = viewer_code;
        }

        #endregion

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
            Tracer.Add_Trace("Tracking_ItemViewer.Write_Main_Viewer_Section", "");

            // Add the HTML for the image
            Output.WriteLine("<!-- TRACKING ITEM VIEWER OUTPUT -->");

            // Start the citation table
            Output.WriteLine("  <td align=\"left\"><span class=\"sbkTrk_ViewerTitle\">Tracking Information</span></td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("<tr>");
            Output.WriteLine("  <td class=\"sbkTrk_MainArea\">");

            // Add the tabs for related admin viewers (if they exist)
            Write_Tracking_Tabs(Output, CurrentRequest, BriefItem);

            // Now, add the text
            Tracer.Add_Trace("Tracking_ItemViewer.History_String", "Displaying history/worklog tracking information");

            if (( trackingDetails == null ) || ( trackingDetails.WorkEvents == null ) || ( trackingDetails.WorkEvents.Count == 0 ))
            {
                Output.WriteLine("<br /><br /><br /><center><strong>ITEM HAS NO HISTORY</strong></center><br /><br /><br />");
            }
            else
            {
                Output.WriteLine("<br />\n");
                Output.WriteLine("<table border=\"1px\" cellpadding=\"5px\" cellspacing=\"0px\" rules=\"cols\" frame=\"void\" bordercolor=\"#e7e7e7\" width=\"100%\">");
                Output.WriteLine("  <tr align=\"center\" bgcolor=\"#0022a7\"><td colspan=\"4\"><span style=\"color: White\"><b>ITEM HISTORY</b></span></td></tr>");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\">");
                Output.WriteLine("    <th><span style=\"color: White\">WORKFLOW NAME</span></th>");
                Output.WriteLine("    <th><span style=\"color: White\">COMPLETED DATE</span></th>");
                Output.WriteLine("    <th><span style=\"color: White\">USER</span></th>");
                Output.WriteLine("    <th><span style=\"color: White\">LOCATION / NOTES</span></th>");
                Output.WriteLine("  </tr>");

                foreach (Item_Tracking_Event worklog in trackingDetails.WorkEvents)
                {
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td>" + worklog.WorkflowName + "</td>");
                    Output.WriteLine("    <td>" + worklog.CompletedDate + "</td>");

                    Output.WriteLine("    <td>" + worklog.WorkPerformedBy + "</td>");
                    if ( String.IsNullOrEmpty(worklog.Notes))
                    {
                        Output.WriteLine("    <td>&nbsp;</td>");
                    }
                    else
                    {
                        Output.WriteLine("    <td>" + worklog.Notes + "</td>");
                    }
                    Output.WriteLine("  </tr>");
                }

                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
                Output.WriteLine("</table>\n");
            }

            Output.WriteLine("<br /> <br />");


            Output.WriteLine("  </td>");
            Output.WriteLine("  <!-- END TRACKING VIEWER OUTPUT -->");
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
