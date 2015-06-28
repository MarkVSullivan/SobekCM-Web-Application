#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Core.Configuration;
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

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 650 </value>
        public override int Viewer_Width
        {
            get
            {
                return 650;
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
            if (CurrentItem.Divisions.Download_Tree.Has_Files)
            {
                List<abstract_TreeNode> pages = CurrentItem.Divisions.Download_Tree.Pages_PreOrder;

                Output.WriteLine("                <h2>" + explanation_text + "</h2>");
                Output.WriteLine("                  <div id=\"sbkDiv_Downloads\">");
                // Step through each download in this item
                string greenstoneLocation = CurrentItem.Web.Source_URL + "/";
                foreach (Page_TreeNode downloadGroup in pages)
                {
                    // Step through each download in this download group/page
                    foreach (SobekCM_File_Info download in downloadGroup.Files)
                    {
                        // Is this an external link?
                        if (download.System_Name.IndexOf("http") >= 0)
                        {
                            // Is the extension already a part of the label?
                            string label_upper = downloadGroup.Label.ToUpper();
                            if (label_upper.IndexOf(download.File_Extension) > 0)
                            {
                                Output.WriteLine("                  <a href=\"" + download.System_Name + "\" target=\"_blank\">" + downloadGroup.Label + "</a><br /><br />");
                            }
                            else
                            {
                                Output.WriteLine("                  <a href=\"" + download.System_Name + "\" target=\"_blank\">" + downloadGroup.Label + " ( " + download.File_Extension + " )</a><br /><br />");
                            }
                        }
                        else
                        {
                            string file_link = greenstoneLocation + download.System_Name;

                            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
                            if ((CurrentItem.Behaviors.Dark_Flag) || (CurrentItem.Behaviors.IP_Restriction_Membership > 0))
                                file_link = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/" + download.System_Name;


                            // Is the extension already a part of the label?
                            string label = downloadGroup.Label;
                            if (label.Length == 0)
                            {
                                label = download.System_Name;
                            }
                            if (label.IndexOf(download.File_Extension, StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                Output.WriteLine("                  <a href=\"" + file_link + "\" target=\"_blank\">" + label + "</a><br /><br />");
                            }
                            else
                            {
                                Output.WriteLine("                  <a href=\"" + file_link + "\" target=\"_blank\">" + label + " ( " + download.File_Extension + " )</a><br /><br />");
                            }
                        }
                    }
                }

                Output.WriteLine("                  </div>");
            }

            // If this was an aerial, allow each jpeg2000 page to be downloaded
            if (CurrentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Aerial)
            {
                List<string> pageDownloads = new List<string>();
                List<abstract_TreeNode> nodes = CurrentItem.Divisions.Physical_Tree.Divisions_PreOrder;
                foreach (Page_TreeNode pageNode in nodes.Where(node => node.Page).Cast<Page_TreeNode>())
                {
                    foreach (SobekCM_File_Info thisFile in pageNode.Files.Where(thisFile => thisFile.System_Name.ToUpper().IndexOf(".JP2") > 0))
                    {
                        pageDownloads.Add("<a href=\"" + (CurrentItem.Web.Source_URL + "/" + thisFile.System_Name).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://") + "\">" + pageNode.Label + "</a>");
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
