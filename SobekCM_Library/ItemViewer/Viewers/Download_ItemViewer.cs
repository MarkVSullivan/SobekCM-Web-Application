#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays the list of links to the downloads associated with a digital resource. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Download_ItemViewer : abstractItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Download"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Download; }
        }

        /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <remarks> This always returns the value TRUE for this viewer </remarks>
        public override bool Override_On_Checked_Out
        {
            get
            {
                return true;
            }
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkDiv_Viewer' </value>
        public override string Viewer_CSS
        {
            get { return "sbkDiv_Viewer"; }
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
                Tracer.Add_Trace("Download_ItemViewer.Write_Main_Viewer_Section", "");
            }

            // Start the citation table
            string explanation_text = "This item has the following downloads:";
            switch (CurrentMode.Language)
            {
                case Web_Language_Enum.French:
                    explanation_text = "Ce document est la suivante téléchargements:";
                    break;

                case Web_Language_Enum.Spanish:
                    explanation_text = "Este objeto tiene las siguientes descargas:";
                    break;

                default:
                    if (CurrentItem.Web.Static_PageCount == 0)
                        explanation_text = "This item is only available as the following downloads:";
                    break;
            }

            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"sbkDiv_MainArea\">");

            Output.WriteLine("              <br />");

            if ((BriefItem.Downloads != null ) && ( BriefItem.Downloads.Count > 0 ))
            {
                Output.WriteLine("                <h2>" + explanation_text + "</h2>");
                Output.WriteLine("                  <div id=\"sbkDiv_Downloads\">");

                // Step through each download in this item
                string greenstoneLocation = CurrentItem.Web.Source_URL + "/";
                foreach (BriefItem_FileGrouping downloadGroup in BriefItem.Downloads)
                {
                    // Step through each download in this download group/page
                    foreach (BriefItem_File download in downloadGroup.Files)
                    {
                        // Get the file extension
                        string file_extension = Path.GetExtension(download.Name);

                        // Is this an external link?
                        if (download.Name.IndexOf("http") >= 0)
                        {
                            // Is the extension already a part of the label?
                            string label_upper = downloadGroup.Label.ToUpper();
                            if (label_upper.IndexOf(file_extension.ToUpper()) > 0)
                            {
                                Output.WriteLine("                  <a href=\"" + download.Name + "\" target=\"_blank\">" + downloadGroup.Label + "</a><br /><br />");
                            }
                            else
                            {
                                Output.WriteLine("                  <a href=\"" + download.Name + "\" target=\"_blank\">" + downloadGroup.Label + " ( " + file_extension + " )</a><br /><br />");
                            }
                        }
                        else
                        {
                            string file_link = greenstoneLocation + download.Name;

                            //// MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
                            //if ((CurrentItem.Behaviors.Dark_Flag) || (CurrentItem.Behaviors.IP_Restriction_Membership > 0))
                            //    file_link = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/" + download.System_Name;


                            // Is the extension already a part of the label?
                            string label = downloadGroup.Label;
                            if (label.Length == 0)
                            {
                                label = download.Name;
                            }
                            if (label.IndexOf(file_extension, StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                Output.WriteLine("                  <a href=\"" + file_link + "\" target=\"_blank\">" + label + "</a><br /><br />");
                            }
                            else
                            {
                                Output.WriteLine("                  <a href=\"" + file_link + "\" target=\"_blank\">" + label + " ( " + file_extension + " )</a><br /><br />");
                            }
                        }
                    }
                }

                Output.WriteLine("                  </div>");
            }

            // If this was an aerial, allow each jpeg2000 page to be downloaded
            if (( BriefItem.Behaviors.Page_File_Extensions_For_Download != null ) && ( BriefItem.Behaviors.Page_File_Extensions_For_Download.Length > 0 ))
            {
                List<string> pageDownloads = new List<string>();
                foreach ( BriefItem_FileGrouping pageNode in BriefItem.Images )
                {
                    // If no file, continue
                    if ((pageNode.Files == null) || (pageNode.Files.Count == 0))
                        continue;

                    // Stp through each file
                    foreach (BriefItem_File thisFile in pageNode.Files)
                    {
                        string file_extension = Path.GetExtension(thisFile.Name);
                        if (!String.IsNullOrEmpty(file_extension))
                        {
                            bool forDownload = BriefItem.Behaviors.Page_File_Extensions_For_Download.Any(thisDownload => String.Compare(thisDownload, file_extension, StringComparison.OrdinalIgnoreCase) == 0);
                            if (forDownload)
                            {
                                pageDownloads.Add("<a href=\"" + (CurrentItem.Web.Source_URL + "/" + thisFile.Name).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://") + "\">" + pageNode.Label + "</a>");
                            }
                        }
                        break;
                    }
                }

                if (pageDownloads.Count > 0)
                {

                    Output.WriteLine("                <h2>The following tiles are available for download:</h2>");
                    Output.WriteLine(( !String.IsNullOrEmpty(CurrentMode.Browser_Type)) && ( CurrentMode.Browser_Type.IndexOf("FIREFOX") >= 0) 
                                           ? "                <p>To download, right click on the tile name below, select 'Save Link As...' and save the JPEG2000 to your local computer.</p>"
                                           : "                <p>To download, right click on the tile name below, select 'Save Target As...' and save the JPEG2000 to your local computer. </p>");
                    Output.WriteLine("                  <table id=\"sbkDiv_Aerials\">");

                    int rows = pageDownloads.Count / 3;
                    if ((pageDownloads.Count % 3) != 0)
                        rows++;

                    for (int i = 0; i < rows; i++)
                    {
                        Output.Write("                    <tr>");
                        if (pageDownloads.Count > i)
                        {
                            Output.Write("<td>" + pageDownloads[i] + "</td>");
                        }
                        if (pageDownloads.Count > (i + rows))
                        {
                            Output.Write("<td>" + pageDownloads[i + rows] + "</td>");
                        }
                        else
                        {
                            Output.Write("<td>&nbsp;</td>");
                        }
                        if (pageDownloads.Count > (i + rows + rows))
                        {
                            Output.Write("<td>" + pageDownloads[i + rows + rows] + "</td>");
                        }
                        else
                        {
                            Output.Write("<td>&nbsp;</td>");
                        }

                        Output.WriteLine("</tr>");
                    }

                    Output.WriteLine("                  </table>");
                }
            }

            Output.WriteLine("              <br />");
            Output.WriteLine("            </div>");
        }
    }
}
