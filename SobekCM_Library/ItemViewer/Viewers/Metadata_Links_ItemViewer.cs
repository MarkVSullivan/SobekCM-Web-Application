using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Metadata links screen item viewer prototyper, which is used to create the link in the main menu, 
    /// and to create the viewer itself if the user selects that option </summary>
    public class Metadata_Links_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Metadata_Links_ItemViewer_Prototyper class </summary>
        public Metadata_Links_ItemViewer_Prototyper()
        {
            ViewerType = "METADATA";
            ViewerCode = "metadata";
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
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem("Description", "Metadata", null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Metadata_Links_ItemViewer"/> class for showing the
        /// links to the main metadata files associated with a digital resource (such METS file, marc.xml file, etc..) during 
        /// execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Metadata_Links_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Metadata_Links_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Item viewer displays the links to the main metadata files associated with a digital resource, such METS file, marc.xml file, etc.. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Metadata_Links_ItemViewer : abstractNoPaginationItemViewer
    {
        /// <summary> Constructor for a new instance of the Metadata_Links_ItemViewer class, used to display the 
        /// links to the main metadata files associated with a digital resource, such METS file, marc.xml file, etc.. </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public Metadata_Links_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkMliv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkMliv_Viewer"; }
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

            // Determine if user can edit
            bool userCanEditItem = false;
            if (CurrentUser != null)
            {
                userCanEditItem = CurrentUser.Can_Edit_This_Item(BriefItem.BibID, BriefItem.Type, BriefItem.Behaviors.Source_Institution_Aggregation, BriefItem.Behaviors.Holding_Location_Aggregation, BriefItem.Behaviors.Aggregation_Code_List);
            }

            // Add the HTML for the citation
            Output.WriteLine("        <!-- CITATION ITEM VIEWER OUTPUT -->");
            Output.WriteLine("        <td>");

            // If this is DARK and the user cannot edit and the flag is not set to show citation, show nothing here
            if ((BriefItem.Behaviors.Dark_Flag) && (!userCanEditItem) && (!UI_ApplicationCache_Gateway.Settings.Resources.Show_Citation_For_Dark_Items))
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
                Citation_Standard_ItemViewer.Add_Citation_View_Tabs(Output, BriefItem, CurrentRequest, "METADATA");

            // Now, add the text
            Output.WriteLine();
            Output.WriteLine(Metadata_String(Tracer) + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");


            CurrentRequest.ViewerCode = viewer_code;
        }

        #region Section returns the metadata tab explanation and links

        /// <summary> Returns the string which contains the metadata links and basic information about the types of metadata</summary>
        /// <returns> Sttring with the metadata links and basic information about the types of metadata</returns>
        protected string Metadata_String(Custom_Tracer Tracer)
        {
            // Get the links for the METS and GSA
            string resourceURL = BriefItem.Web.Source_URL + "/";
            string complete_mets = resourceURL + BriefItem.BibID + "_" + BriefItem.VID + ".mets.xml";
            string marc_xml = resourceURL + "marc.xml";

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((BriefItem.Behaviors.Dark_Flag) || (BriefItem.Behaviors.IP_Restriction_Membership > 0))
            {
                resourceURL = CurrentRequest.Base_URL + "files/" + BriefItem.BibID + "/" + BriefItem.VID + "/";
                complete_mets = resourceURL + BriefItem.BibID + "_" + BriefItem.VID + ".mets.xml";
                marc_xml = resourceURL + "marc.xml";
            }


            StringBuilder builder = new StringBuilder(3000);

            builder.AppendLine("<blockquote>");
            builder.AppendLine("<p>The data (or metadata) about this digital resource is available in a variety of metadata formats. For more information about these formats, see the <a href=\"http://ufdc.ufl.edu/sobekcm/metadata\">Metadata Section</a> of the <a href=\"http://ufdc.ufl.edu/sobekcm/\">Technical Aspects</a> information.</p>");
            builder.AppendLine("<br />");

            if ( BriefItem.Type == "EAD" )
            {
                string ead_file = String.Empty;
                foreach (BriefItem_FileGrouping downloadPage in BriefItem.Downloads)
                {
                    // Was this an EAD page?
                    if ((downloadPage.Label == "EAD") && (downloadPage.Files.Count == 1))
                    {
                        if (downloadPage.Files[0].Name.ToLower().IndexOf(".xml") > 0)
                        {
                            ead_file = downloadPage.Files[0].Name;
                            break;
                        }
                    }
                }

                if (ead_file.Length > 0)
                {
                    builder.AppendLine("<div id=\"sbkCiv_EadDownload\" class=\"sbCiv_DownloadSection\">");
                    builder.AppendLine("  <a href=\"" + resourceURL + ead_file + "\" target=\"_blank\">View Finding Aid (EAD)</a>");
                    builder.AppendLine("  <p>This archival collection is described with an electronic finding aid.   This metadata file contains all of the archival description and container list for this archival material.  This file follows the established <a href=\"http://www.loc.gov/ead/\">Encoded Archival Description</a> (EAD) standard.</p>");
                    builder.AppendLine("</div>");
                }
            }

            builder.AppendLine("<div id=\"sbkCiv_MetsDownload\" class=\"sbCiv_DownloadSection\">");
            builder.AppendLine("  <a href=\"" + complete_mets + "\" target=\"_blank\">View Complete METS/MODS</a>");
            builder.AppendLine("  <p>This metadata file is the source metadata file submitted along with all the digital resource files. This contains all of the citation and processing information used to build this resource. This file follows the established <a href=\"http://www.loc.gov/standards/mets/\">Metadata Encoding and Transmission Standard</a> (METS) and <a href=\"http://www.loc.gov/standards/mods/\">Metadata Object Description Schema</a> (MODS). This METS/MODS file was just read when this item was loaded into memory and used to display all the information in the standard view and marc view within the citation.</p>");
            builder.AppendLine("</div>");

            string baseLocationUrl = String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Servers.Base_SobekCM_Location_Relative) ? String.Empty : UI_ApplicationCache_Gateway.Settings.Servers.Base_SobekCM_Location_Relative;

            builder.AppendLine("<div id=\"sbkCiv_MarcXmlDownload\" class=\"sbCiv_DownloadSection\">");
            builder.AppendLine("  <a href=\"" + marc_xml + "\" target=\"_blank\">View MARC XML File</a>");
            builder.AppendLine("  <p>The entered metadata is also converted to MARC XML format, for interoperability with other library catalog systems.  This represents the same data available in the <a href=\"" + baseLocationUrl + UrlWriterHelper.Redirect_URL(CurrentRequest, "FC2") + "\">MARC VIEW</a> except this is a static XML file.  This file follows the <a href=\"http://www.loc.gov/standards/marcxml/\">MarcXML Schema</a>.</p>");
            builder.AppendLine("</div>");

            // Should the TEI be added here?

            if (BriefItem.Behaviors.Get_Viewer("TEI") != null )
            {
                // Does a TEI file exist?
                //string tei_filename = BriefItem.BibID + "_" + BriefItem.VID + ".tei.xml";

                // This code previously created the TEI at this point... before it was retrieved
                // This should go into engine endpoint code or somethin
                //if (!SobekFileSystem.FileExists(BriefItem, tei_filename))
                //{

                //    if (Tracer != null)
                //    {
                //        Tracer.Add_Trace("Citation_ItemViewer.Metadata_String", "Building default TEI file");
                //    }

                //    TEI_File_ReaderWriter writer = new TEI_File_ReaderWriter();

                //    Dictionary<string, object> options = new Dictionary<string, object>();

                //    string error_message;
                //    writer.Write_Metadata(tei_filename, CurrentItem, options, out error_message);
                //}

                // Add the HTML for this
                builder.AppendLine("<div id=\"sbkCiv_TeiDownload\" class=\"sbCiv_DownloadSection\">");
                builder.AppendLine("  <a href=\"" + resourceURL + BriefItem.BibID + "_" + BriefItem.VID + ".tei.xml\" target=\"_blank\">View TEI/Text File</a>");
                builder.AppendLine("  <p>The full-text of this item is also available in the established standard <a href=\"http://www.tei-c.org/index.xml\">Text Encoding Initiative</a> (TEI) downloadable file.</p>");
                builder.AppendLine("</div>");

            }

            builder.AppendLine("</blockquote><br />");
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
