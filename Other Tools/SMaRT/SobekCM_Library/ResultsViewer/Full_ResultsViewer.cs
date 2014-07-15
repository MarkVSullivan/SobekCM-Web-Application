#region Using directives

using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Results viewer reads the METS file for an item and displays either the first image or the full citation while navigating through the results set.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class Full_ResultsViewer :  abstract_ResultsViewer
    {
        /// <summary> Constructor for a new instance of the Full_ResultsViewer class </summary>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        public Full_ResultsViewer(Item_Lookup_Object All_Items_Lookup)
        {
            base.All_Items_Lookup = All_Items_Lookup;
        }

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public override void Add_HTML(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Full_ResultsWriter.Add_HTML", "Loading item information");
            }

            //// If results are null, or no results, return empty string
            //if ((resultTable == null) || (resultTable.Title_Count == 0))
            //{
            //    return null;
            //}

            //// Start the results
            //StringBuilder resultsBldr = new StringBuilder();
            //resultsBldr.Append("<table width=\"100%\">\n");

            //// Determine which rows to display
            //int lastRow = base.LastRow;
            //int startRow = base.StartRow(lastRow);

            //// First, show the items WITH a value in the sort field
            //int withValue = resultTable.Filter_NonEmpties();

            //// Step through all the rows 
            ////	int currRowIndex = -1;
            //bool empties = false;
            //int adjust = 0;
            //SobekCM_Item_Collection.SobekCM_Item_Group_Row titleRow = null;
            //List<SobekCM_Item_Collection.SobekCM_Item_Row> itemRows = new List<SobekCM_Item_Collection.SobekCM_Item_Row>();
            ////int i = currentMode.Page - 1;
            //int i = startRow;

            //// Readjust and filter empties next, if appropriate
            //if ((!empties) && (i >= withValue))
            //{
            //    empties = true;
            //    resultTable.Filter_Empties();
            //    adjust = withValue;
            //}

            //// Get the data rows for this
            //if (resultTable.Current_Sort.IndexOf("SortDate") >= 0)
            //{
            //    itemRows.Add(resultTable.GetItem(i - adjust));
            //    titleRow = itemRows[0].Parent_Row;
            //}
            //else
            //{
            //    titleRow = resultTable.GetRow(i - adjust);
            //    itemRows = titleRow.Child_Rows;
            //}

            //// Get the item from the all list
            //SobekCM_Item_Collection.SobekCM_Item_Row firstItemRow = ((SobekCM_Item_Collection.SobekCM_Item_Row)itemRows[0]);

            //// Pull the value from the table for the first item
            //Application_State.Single_Item dbItem = allItems.Item_By_Bib_VID(titleRow.BibID.ToUpper(), firstItemRow.VID, Tracer);
            //Application_State.Multiple_Volume_Item dbTitle = allItems.Title_By_Bib(titleRow.BibID.ToUpper());

            //// Read this item information
            //Item.SobekCM_METS_Based_ItemBuilder builder = new SobekCM.Library.Items.SobekCM_METS_Based_ItemBuilder();
            //string mets_location = SobekCM_Library_Settings.Image_URL + dbTitle.File_Root + "/" + dbItem.VID_String;
            //SobekCM.Resource_Object.SobekCM_Item thisItem = builder.Build_Brief_Item(mets_location, Tracer);

            //if (thisItem != null)
            //{
            //    // Pull values from the database
            //    thisItem.Behaviors.GroupTitle = dbTitle.GroupTitle;
            //    thisItem.Web.AssocFilePath = dbTitle.File_Root + "/" + dbItem.VID_String;
            //    thisItem.Web.File_Root = String.Empty;
            //    thisItem.Behaviors.Image_Root = SobekCM_Library_Settings.Image_URL;
            //    thisItem.Behaviors.IP_Restriction_Membership = dbItem.IP_Range_Membership;

            //    string preview_citation = "PREVIEW CITATION";
            //    string preview_image = "PREVIEW IMAGE";
            //    string full_item = "FULL ITEM";
            //    string map_it = "MAP IT!";
            //    string full_item_link = base.currentMode.Base_URL + thisItem.BibID + "/" + thisItem.VID + base.Image_Redirect_Stem;

            //    // Is this restricted?
            //    bool restricted_by_ip = false;
            //    string restricted_message = String.Empty;
            //    if (dbItem.IP_Range_Membership > 0)
            //    {
            //        int comparison = dbItem.IP_Range_Membership & base.current_user_mask;
            //        if (comparison == 0)
            //        {
            //            restricted_by_ip = true;
            //            restricted_message = ipRestrictions[0].Item_Restricted_Statement;
            //            preview_image = "RESTRICTED";
            //        }
            //    }

            //    resultsBldr.AppendLine("<br />");
            //    resultsBldr.AppendLine("<table class=\"SobekDocumentDisplay\" cellpadding=\"2px\" cellspacing=\"0px\" width=\"630px\" align=\"center\" >");
            //    resultsBldr.AppendLine("  <tr>");
            //    resultsBldr.AppendLine("    <td>");
            //    resultsBldr.AppendLine("      <div class=\"SobekDocumentHeader\">");
            //    resultsBldr.AppendLine("        <table cellspacing=\"6px\">");

            //    string mainTitleCompare = thisItem.Bib_Info.Main_Title.ToString().ToUpper().Replace(".", "").Replace("&QUOT;", "").Replace("&AMP;", "").Replace("&", "").Replace("\"", "").Replace(">","").Replace("<","").Trim();
            //    string groupTitleCompare = thisItem.Behaviors.GroupTitle.ToUpper().Replace(".", "").Replace("&QUOT;", "").Replace("&AMP;", "").Replace("&", "").Replace("\"", "").Replace(">", "").Replace("<", "").Trim();
            //    if (mainTitleCompare != groupTitleCompare)
            //    {
            //        resultsBldr.AppendLine("          <tr valign=\"top\">");
            //        resultsBldr.AppendLine("            <td width=\"30px\"> </td>");
            //        resultsBldr.AppendLine("            <td><b>Group Title: </b></td>");
            //        resultsBldr.AppendLine("            <td align=\"left\">" + thisItem.Behaviors.GroupTitle.Replace("<","&lt;").Replace(">","&gt;") + "</td>");
            //        resultsBldr.AppendLine("          </tr>");
            //    }

            //    resultsBldr.AppendLine("          <tr valign=\"top\">");
            //    resultsBldr.AppendLine("            <td width=\"30px\"> </td>");
            //    resultsBldr.AppendLine("            <td><b>Title: </b></td>");
            //    resultsBldr.AppendLine("            <td align=\"left\">" + thisItem.Bib_Info.Main_Title.ToString() + "</td>");
            //    resultsBldr.AppendLine("          </tr>");

            //    resultsBldr.AppendLine("          <tr valign=\"top\">");
            //    resultsBldr.AppendLine("            <td width=\"30px\"> </td>");
            //    resultsBldr.AppendLine("            <td><b>Full Item: </b></td>");
            //    resultsBldr.AppendLine("            <td align=\"left\"><a href=\"" + full_item_link + "\">Click for full item</a></td>");
            //    resultsBldr.AppendLine("          </tr>");

            //    resultsBldr.AppendLine("        </table>");
            //    resultsBldr.AppendLine("      </div>");

            //    // If this is for an image, but it doesn't exist, skip it
            //    bool skip_image = false;
            //    if (thisItem.Behaviors.Main_Page.FileName.Length == 0)
            //    {
            //        skip_image = true;
            //    }

            //    resultsBldr.AppendLine("      <div class=\"SobekViewSelectRow\">");

            //    if ((currentMode.Result_Display_Type == SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Citation) || ( skip_image ))
            //    {
            //        resultsBldr.AppendLine("        <img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cL_s.gif\" /><span class=\"tab_s\"> " + preview_citation + " </span><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cR_s.gif\" />");
            //    }
            //    else
            //    {
            //        currentMode.Result_Display_Type = SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Citation;
            //        resultsBldr.AppendLine("        <a href=\"" + currentMode.Redirect_URL() + "#image\"> <img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cL.gif\" /><span class=\"tab\"> " + preview_citation + " </span><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cR.gif\" /> </a>");
            //        currentMode.Result_Display_Type = SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Image;
            //    }

            //    if (!skip_image)
            //    {
            //        if (currentMode.Result_Display_Type == SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Image)
            //        {
            //            resultsBldr.AppendLine("        <img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cL_s.gif\" /><span class=\"tab_s\"> " + preview_image + " </span><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cR_s.gif\" />");
            //        }
            //        else
            //        {
            //            currentMode.Result_Display_Type = SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Image;
            //            resultsBldr.AppendLine("        <a href=\"" + currentMode.Redirect_URL() + "#image\"> <img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cL.gif\" /><span class=\"tab\"> " + preview_image + " </span><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cR.gif\" /> </a>");
            //            currentMode.Result_Display_Type = SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Citation;
            //        }
            //    }
            //    resultsBldr.AppendLine("        <a href=\"" + full_item_link + "\"> <img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cL.gif\" /><span class=\"tab\"> " + full_item + " </span><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cR.gif\" /> </a>");

            //    if ((( thisItem.Bib_Info.hasCoordinateInformation ) && ((thisItem.Bib_Info.Coordinates.Point_Count > 0) || (thisItem.Bib_Info.Coordinates.Polygon_Count > 0)))
            //        || (( dbItem.Spatial_KML != null ) && ( dbItem.Spatial_KML.Length > 0 )))
            //    {
            //        if ( !restricted_by_ip )
            //            resultsBldr.AppendLine("        <a href=\"" + full_item_link + "\"> <img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cL.gif\" /><span class=\"tab\"> " + map_it + " </span><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/tabs/cR.gif\" /> </a>");
            //    }

            //    resultsBldr.AppendLine("      </div>");
            //    resultsBldr.AppendLine("    </td>");
            //    resultsBldr.AppendLine("  </tr>");

            //    resultsBldr.AppendLine("  <tr>");

            //    if ((currentMode.Result_Display_Type == SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Image) && ( !skip_image ))
            //    {
            //        if (restricted_by_ip)
            //        {
            //            resultsBldr.AppendLine("    <td align=\"left\" colspan=\"3\">");
            //            resultsBldr.AppendLine("       " + restricted_message);
            //            resultsBldr.AppendLine("    </td>");
            //        }
            //        else
            //        {
            //            resultsBldr.AppendLine("    <td align=\"center\" colspan=\"3\">");
            //            string image_link = SobekCM_Library_Settings.Image_URL + thisItem.Web.AssocFilePath + "/" + thisItem.Behaviors.Main_Page.FileName;
            //            resultsBldr.AppendLine("      <a href=\"" + full_item_link + "\"><img border=\"0\" src=\"" + image_link + "\" /></a>");
            //            resultsBldr.AppendLine("    </td>");
            //        }
            //    }
            //    else
            //    {
            //        resultsBldr.AppendLine("    <td class=\"SobekCitationDisplay\">");
            //        resultsBldr.AppendLine("      <div class=\"SobekCitation\">");
            //        ItemViewer.Viewers.Citation_ItemViewer citationViewer = new SobekCM.Library.ItemViewer.Viewers.Citation_ItemViewer(languageSupport, null, false);
            //        citationViewer.CurrentItem = thisItem;
            //        citationViewer.CurrentMode = currentMode;
            //        resultsBldr.AppendLine(citationViewer.Standard_Citation_String(false, Tracer));
            //        resultsBldr.AppendLine("      </div>");
            //        resultsBldr.AppendLine("    </td>");

            //    }
            //    resultsBldr.AppendLine("  </tr>");

            //    resultsBldr.AppendLine("</table>");
            //    resultsBldr.AppendLine("<br />");

            //    System.Web.UI.LiteralControl newLiteral = new System.Web.UI.LiteralControl();
            //    newLiteral.Text = resultsBldr.ToString();
            //    placeHolder.Controls.Add(newLiteral);
            //}
            //else
            //{
            //    StringBuilder errorBldr = new StringBuilder();

            //    errorBldr.Append("<br /><br />");
            //    errorBldr.Append("<b>An error was encountered while opening this digital resource.</b>");
            //    errorBldr.Append("<br /><br />");

            //    System.Web.UI.LiteralControl errorLiteral = new System.Web.UI.LiteralControl();
            //    errorLiteral.Text = errorBldr.ToString();
            //    placeHolder.Controls.Add(errorLiteral);
            //}

        }
    }
}
