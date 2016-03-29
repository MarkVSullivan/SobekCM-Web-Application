using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.MARC;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> MARC21 description item viewer prototyper, which is used to create the link in the main menu, 
    /// and to create the viewer itself if the user selects that option </summary>
    public class Citation_MARC_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Citation_MARC_ItemViewer_Prototyper class </summary>
        public Citation_MARC_ItemViewer_Prototyper()
        {
            ViewerType = "MARC";
            ViewerCode = "marc";
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
            // This should always be included 
            return true;
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> FALSE always, since citation type information should always be shown, even if an item is checked out </returns>
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
            // It can always be shown
            return true;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest">  Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem("Description", "MARC View", null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Citation_MARC_ItemViewer"/> class for showing the
        /// description citation information in MARC21 format for a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Citation_MARC_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Citation_MARC_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Item viewer displays the descriptive citation in MARC21 format </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Citation_MARC_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly bool userCanEdit;

        /// <summary> Constructor for a new instance of the Citation_MARC_ItemViewer class, used to display the 
        /// descriptive citation in MARC21 format</summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public Citation_MARC_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            // Check to see if the user can edit this
            userCanEdit = false;
            if ( CurrentUser != null )
                userCanEdit = CurrentUser.Can_Edit_This_Item(BriefItem.BibID, BriefItem.Type, BriefItem.Behaviors.Source_Institution_Aggregation, BriefItem.Behaviors.Holding_Location_Aggregation, BriefItem.Behaviors.Aggregation_Code_List);
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkCmiv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkCmiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Citation_Standard_ItemViewer.Write_Main_Viewer_Section", "Write the citation information directly to the output stream");
            }


            // Add the HTML for the citation
            Output.WriteLine("        <!-- CITATION ITEM VIEWER OUTPUT -->");
            Output.WriteLine("        <td>");

            // If this is DARK and the user cannot edit and the flag is not set to show citation, show nothing here
            if ((BriefItem.Behaviors.Dark_Flag) && (!userCanEdit) && (!UI_ApplicationCache_Gateway.Settings.Resources.Show_Citation_For_Dark_Items))
            {
                Output.WriteLine("          <div id=\"darkItemSuppressCitationMsg\">This item is DARK and cannot be viewed at this time</div>" + Environment.NewLine + "</td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
                return;
            }

            string viewer_code = CurrentRequest.ViewerCode;

            // Get any search terms
            List<string> terms = new List<string>();
            if (!String.IsNullOrWhiteSpace(CurrentRequest.Text_Search))
            {
                string[] splitter = CurrentRequest.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
            }

            // Add the main wrapper division
            Output.WriteLine("<div id=\"sbkCiv_Citation\">");

            if (!CurrentRequest.Is_Robot)
                Citation_Standard_ItemViewer.Add_Citation_View_Tabs(Output, BriefItem, CurrentRequest, "MARC");

            // Now, add the text
            Output.WriteLine();
            if (terms.Count > 0)
            {
                Output.WriteLine(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(MARC_String(Tracer), terms, "<span class=\"sbkCiv_TextHighlight\">", "</span>") + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
            }
            else
            {
                Output.WriteLine(MARC_String(Tracer) + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
            }

            CurrentRequest.ViewerCode = viewer_code;
        }

        #region Code to generate the MARC record in HTML

        /// <summary> Returns the basic information about this digital resource in MARC HTML format </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> HTML string with the MARC information for this digital resource </returns>
        /// <remarks> The width statement for this rendering defaults to 95% of width</remarks>
        public string MARC_String(Custom_Tracer Tracer)
        {
            return MARC_String("95%", Tracer);
        }

        /// <summary> Returns the basic information about this digital resource in MARC HTML format </summary>
        /// <param name="Width"> Width statement to be included in the MARC21 table </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> HTML string with the MARC information for this digital resource </returns>
        public string MARC_String(string Width, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Citation_ItemViewer.MARC_String", "Configuring METS data into MARC format");
            }

            //// Build the value
            StringBuilder builder = new StringBuilder();

            // Add the edit item button, if the user can edit it
            if ((userCanEdit) && (BriefItem.Type != "BIBLEVEL"))
            {
                CurrentRequest.Mode = Display_Mode_Enum.My_Sobek;
                CurrentRequest.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
                CurrentRequest.My_Sobek_SubMode = "1";
                builder.AppendLine("<blockquote><button onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(CurrentRequest) + "';return false;\" id=\"sbkCiv_MarcEditButton\" class=\"roundbutton\"> EDIT THIS ITEM </button></blockquote>");
                CurrentRequest.Mode = Display_Mode_Enum.Item_Display;
            }
            else
            {
                builder.AppendLine("<br />");
            }

            // Get the MARC record
            MARC_Transfer_Record record = SobekEngineClient.Items.Get_Item_MARC_Record(BriefItem.BibID, BriefItem.VID, true, Tracer);

            builder.AppendLine(record.ToHTML( Width));

            builder.AppendLine("<br />");
            builder.AppendLine("<br />");
            builder.AppendLine("<div id=\"sbkCiv_MarcAutoGenerated\">The record above was auto-generated from the METS file.</div>");
            builder.AppendLine();
            builder.AppendLine("<br />");
            builder.AppendLine("<br />");
            builder.AppendLine("</div>");

            return builder.ToString();
        }

        #endregion


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
