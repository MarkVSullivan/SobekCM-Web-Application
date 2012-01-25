#region Using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.Divisions;
using SobekCM.Library.Application_State;

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
        /// <value> This is a single page viewer, so this property always returns FALSE</value>
        public override bool Show_Page_Selector
        {
            get
            {
                return false;
            }
        }

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Download_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

            // Build the value
            StringBuilder builder = new StringBuilder(1500);

            // Start the citation table
            string explanation_text = "This item has the following downloads:";
            switch( CurrentMode.Language )
            {
                case Language_Enum.French:
                    builder.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\">Téléchargements</span></td>");
                    explanation_text = "Ce document est la suivante téléchargements:";
                    break;

                case Language_Enum.Spanish:
                    builder.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\">Descargas</span></td>");
                    explanation_text = "Este objeto tiene las siguientes descargas:";
                    break;

                default:
                    if (CurrentItem.SobekCM_Web.Static_PageCount == 0)
                        explanation_text = "This item is only available as the following downloads:";
                    builder.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\">Downloads</span></td>");
                    break;
            }

            builder.AppendLine("        </tr>" );
            builder.AppendLine("        <tr>");
            builder.AppendLine("          <td>");
            builder.AppendLine("            <div class=\"SobekCitation\">");

            builder.AppendLine("              <br />" );
            builder.AppendLine("              <blockquote>");
            if (CurrentItem.Divisions.Download_Tree.Has_Files)
            {
                List<abstract_TreeNode> pages = CurrentItem.Divisions.Download_Tree.Pages_PreOrder;

                builder.AppendLine("                <b>" + explanation_text + "</b><br /><br />" );
                builder.AppendLine("                <blockquote>");

                // Step through each download in this item
                string greenstoneLocation = CurrentItem.SobekCM_Web.Source_URL + "/";
                foreach (Page_TreeNode downloadGroup in pages)
                {
                    // Step through each download in this download group/page
                    foreach (SobekCM_File_Info download in downloadGroup.Files)
                    {
                        // Is this an external link?
                        if (download.Service_Copy.IndexOf("http") >= 0)
                        {
                            // Is the extension already a part of the label?
                            string label_upper = downloadGroup.Label.ToUpper();
                            if (label_upper.IndexOf(download.File_Extension) > 0)
                            {
                                builder.AppendLine("                  <a href=\"" + download.Service_Copy + "\" target=\"_blank\">" + downloadGroup.Label + "</a><br /><br />");
                            }
                            else
                            {
                                builder.AppendLine("                  <a href=\"" + download.Service_Copy + "\" target=\"_blank\">" + downloadGroup.Label + " ( " + download.File_Extension + " )</a><br /><br />");
                            }
                        }
                        else
                        {
                            // Is the extension already a part of the label?
                            string label_upper = downloadGroup.Label.ToUpper();
                            if (label_upper.IndexOf(download.File_Extension) > 0)
                            {
                                builder.AppendLine("                  <a href=\"" + greenstoneLocation + download.System_Name + "\" target=\"_blank\">" + downloadGroup.Label + "</a><br /><br />");
                            }
                            else
                            {
                                builder.AppendLine("                  <a href=\"" + greenstoneLocation + download.System_Name + "\" target=\"_blank\">" + downloadGroup.Label + " ( " + download.File_Extension + " )</a><br /><br />");
                            }
                        }
                    }
                }

                builder.AppendLine("                </blockquote>" );
            }

            // If this was an aerial, allow each jpeg2000 page to be downloaded
            if (CurrentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Aerial )
            {
                List<string> pageDownloads = new List<string>();
                List<abstract_TreeNode> nodes = CurrentItem.Divisions.Physical_Tree.Divisions_PreOrder;
                foreach (Page_TreeNode pageNode in nodes.Where(node => node.Page).Cast<Page_TreeNode>())
                {
                    foreach (SobekCM_File_Info thisFile in pageNode.Files.Where(thisFile => thisFile.System_Name.ToUpper().IndexOf(".JP2") > 0))
                    {
                        pageDownloads.Add("<a href=\"" + (CurrentItem.SobekCM_Web.Source_URL + "/" + thisFile.System_Name).Replace("\\", "/").Replace("//","/").Replace("http:/","http://") + "\">" + pageNode.Label + "</a>");
                        break;
                    }
                }

                if (pageDownloads.Count > 0)
                {

                    builder.AppendLine("                <b>The following tiles are available for download:</b><br /><br />");
                    builder.AppendLine(CurrentMode.Browser_Type.IndexOf("FIREFOX") >= 0
                                           ? "                To download, right click on the tile name below, select 'Save Link As...' and save the JPEG2000 to your local computer. <br /><br />"
                                           : "                To download, right click on the tile name below, select 'Save Target As...' and save the JPEG2000 to your local computer. <br /><br />");
                    builder.AppendLine("                <blockquote>" );
                    builder.AppendLine("                  <table width=\"100%\">" );

                    int rows = pageDownloads.Count / 3;
                    if ((pageDownloads.Count % 3) != 0)
                        rows++;

                    for( int i = 0 ; i < rows ; i++)
                    {
                        builder.Append("                    <tr>");
                        if (pageDownloads.Count > i)
                        {
                            builder.Append("<td>" + pageDownloads[i] + "</td>");
                        }
                        if (pageDownloads.Count > (i + rows))
                        {
                            builder.Append("<td>" + pageDownloads[i + rows] + "</td>");
                        }
                        else
                        {
                            builder.Append("<td>&nbsp;</td>");
                        }
                        if (pageDownloads.Count > (i + rows + rows))
                        {
                            builder.Append("<td>" + pageDownloads[i + rows + rows] + "</td>");
                        }
                        else
                        {
                            builder.Append("<td>&nbsp;</td>");
                        }

                        builder.AppendLine("</tr>");
                    }

                    builder.AppendLine("                  </table>");
                    builder.AppendLine("                </blockquote>" );
                }
            }

            builder.AppendLine("              </blockquote>" );
            builder.AppendLine("              <br />" );
            builder.AppendLine("            </div>");

            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            placeHolder.Controls.Add(mainLiteral);
        }

        /// <summary> Adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Returns FALSE since nothing was added to the left navigational bar </returns>
        /// <remarks> For this item viewer, this method does nothing except return FALSE </remarks>
        public override bool Add_Nav_Bar_Menu_Section(PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Download_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
        }
    }
}
