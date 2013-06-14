#region Using directives

using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Items;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays other issues related to the current digital resource by title / bib id </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class MultiVolumes_ItemViewer : abstractItemViewer
    {
        /// <summary> Sets the list of all the items within this title to be displayed </summary>
        public SobekCM_Items_In_Title Item_List { private get; set; }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.MultiVolume"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.MultiVolume; }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is generally single page viewer, so this property usually returns the value 1.  If there
        /// are over 100 volumes and it is currently in thumbnail view, then it will return a value so that 
        /// 60 volumes appear on each page.</value>
        public override int PageCount
        {
            get
            {
                if (Item_List == null)
                    return 1;

                if (CurrentMode.ViewerCode != "allvolumes2")
                    return 1;

                if (CurrentMode.SubPage <= 0)
                    CurrentMode.SubPage = 1;

                if ( Item_List.Item_Table.Rows.Count <= 100)
                    return 1;
                
                return (((Item_List.Item_Table.Rows.Count - 1) / 60)) + 1;
            }
        }

        /// <summary> Gets the current page for paging purposes </summary>
        /// <value> This returns either 1, or the <see cref="Navigation.SobekCM_Navigation_Object.SubPage"/> value from the current reqeust mode</value>
        public override int Current_Page
        {
            get
            {
                if (CurrentMode.SubPage <= 0)
                    CurrentMode.SubPage = 1;

                return CurrentMode.SubPage;
            }
        }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This always returns NONE </value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 650 </value>
        public override int Viewer_Width
        {
            get
            {
                return 750;
            }
        }

        /// <summary> Gets the url to go to the first page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public override string First_Page_URL
        {
            get
            {
                if (CurrentMode.ViewerCode != "allvolumes2")
                    return String.Empty;

                ushort subpage = CurrentMode.SubPage;
                CurrentMode.SubPage = 1;
                string returnVal = CurrentMode.Redirect_URL();
                CurrentMode.SubPage = subpage;
                return returnVal;
            }
        }

        /// <summary> Gets the url to go to the previous page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public override string Previous_Page_URL
        {
            get
            {
                if (CurrentMode.ViewerCode != "allvolumes2")
                    return String.Empty;

                ushort subpage = CurrentMode.SubPage;
                CurrentMode.SubPage = (ushort)(CurrentMode.SubPage - 1);
                string returnVal = CurrentMode.Redirect_URL();
                CurrentMode.SubPage = subpage;
                return returnVal;
            }
        }

        /// <summary> Gets the url to go to the next page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public override string Next_Page_URL
        {
            get
            {
                if (CurrentMode.ViewerCode != "allvolumes2")
                    return String.Empty;
                
                ushort subpage = CurrentMode.SubPage;
                CurrentMode.SubPage = (ushort)(CurrentMode.SubPage + 1);
                string returnVal = CurrentMode.Redirect_URL();
                CurrentMode.SubPage = subpage;
                return returnVal;
            }
        }

        /// <summary> Gets the url to go to the last page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public override string Last_Page_URL
        {
            get
            {
                if (CurrentMode.ViewerCode != "allvolumes2")
                    return String.Empty;

                ushort subpage = CurrentMode.SubPage;
                CurrentMode.SubPage = (ushort)((Item_List.Item_Table.Rows.Count / 60));
                if ((Item_List.Item_Table.Rows.Count % 60) != 0)
                    CurrentMode.SubPage = (ushort)(CurrentMode.SubPage + 1);
                string returnVal = CurrentMode.Redirect_URL();
                CurrentMode.SubPage = subpage;
                return returnVal;
            }
        }


        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("MultiVolumes_ItemViewer.Add_Main_Viewer_Section", "Adds tree view control and pulls volumes from database");
            }

            // Determine the citation type
            View_Type viewType = View_Type.Tree;
            switch (CurrentMode.ViewerCode)
            {
                case "allvolumes2":
                    viewType = View_Type.Thumbnail;
                    break;

                case "allvolumes3":
                    viewType = View_Type.List;
                    break;
            }

            // Build the value
            StringBuilder builder = new StringBuilder(5000);

            string volumes_text = "All Volumes";
            string issues_text = "All Issues";
            string map_text = "Related Map Sets";
            const string AERIAL_TEXT = "Related Flights";

            if (CurrentMode.Language == Web_Language_Enum.French)
            {
                volumes_text = "Tous les Volumes";
                issues_text = "Tous les Éditions";
                map_text = "Définit la Carte Connexes";
            }

            if (CurrentMode.Language == Web_Language_Enum.Spanish)
            {
                volumes_text = "Todos los Volumenes";
                issues_text = "Todas las Ediciones";
                map_text = "Relacionado Mapa Conjuntos";
            }

            string issues_type = volumes_text;
            if ( CurrentItem.Behaviors.GroupType.ToUpper().IndexOf("NEWSPAPER") >= 0 )
            {
                issues_type = issues_text;
            }
            else if (CurrentItem.Behaviors.GroupType.ToUpper().IndexOf("MAP") >= 0)
            {
                issues_type = map_text;
            }
            else if (CurrentItem.Behaviors.GroupType.ToUpper().IndexOf("AERIAL") >= 0)
            {
                issues_type = AERIAL_TEXT;
            }

            const string TREE_VIEW = "VOLUME TREE";
            const string THUMBNAIL_VIEW = "VOLUME THUMBNAILS";
            const string LIST_VIEW = "VOLUME LIST";

            builder.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\">" + issues_type + "</span></td>");
            builder.AppendLine("        </tr>");
            builder.AppendLine("        <tr>");
            builder.AppendLine("          <td>");
            builder.AppendLine("            <div class=\"SobekCitation\">" );
            builder.AppendLine("              <div class=\"CitationViewSelectRow\">");
            if (viewType == View_Type.Tree)
            {
                builder.AppendLine("                <img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" alt=\"\" /><span class=\"tab_s\">" + TREE_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" alt=\"\" />");
            }
            else
            {
                builder.AppendLine("                <a href=\"" + CurrentMode.Redirect_URL("allvolumes1") + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + TREE_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" alt=\"\" /></a>");
            }

            if (viewType == View_Type.Thumbnail)
            {
                builder.AppendLine("                <img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" alt=\"\" /><span class=\"tab_s\">" + THUMBNAIL_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" alt=\"\" />");
            }
            else
            {
                builder.AppendLine("                <a href=\"" + CurrentMode.Redirect_URL("allvolumes2") + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + THUMBNAIL_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" alt=\"\" /></a>");
            }

            if ((viewType == View_Type.List) || (CurrentMode.Internal_User))
            {
                if ( viewType == View_Type.List )
                {
                    builder.AppendLine("                <img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" alt=\"\" /><span class=\"tab_s\">" + LIST_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" alt=\"\" />");
                }
                else
                {
                    builder.AppendLine("                <a href=\"" + CurrentMode.Redirect_URL("allvolumes3") + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + LIST_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" alt=\"\" /></a>");
                }
            }
            builder.AppendLine("              </div>");

            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            placeHolder.Controls.Add(mainLiteral);

            switch (viewType)
            {
                case View_Type.Tree:
                    // Add the tree view
                    TreeView treeView1 = new TreeView {EnableViewState = false, CssClass = "SobekGroupViewTree"};
                    Build_Tree(treeView1, Item_List.Item_Table);
                    placeHolder.Controls.Add(treeView1 );
                    break;

                case View_Type.Thumbnail:
                    placeHolder.Controls.Add(Add_Thumbnails(Item_List.Item_Table));
                    CurrentMode.ViewerCode = "allvolumes2";
                    break;

                case View_Type.List:
                    placeHolder.Controls.Add(Add_List(Item_List.Item_Table));
                    CurrentMode.ViewerCode = "allvolumes3";
                    break;
            }

            // Add the final HTML
            Literal secondLiteral = new Literal();
            if (CurrentItem.Web.Related_Titles_Count > 0)
            {
                StringBuilder relatedBuilder = new StringBuilder(1000);
                relatedBuilder.AppendLine("<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection1\">");
                relatedBuilder.AppendLine("  <tr>");
                relatedBuilder.AppendLine("    <td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;Related Titles</b></td>");
                relatedBuilder.AppendLine("  </tr>");
                string url_opts = CurrentMode.URL_Options();
                foreach (Related_Titles thisTitle in CurrentItem.Web.All_Related_Titles)
                {
                    relatedBuilder.AppendLine("  <tr>");
                    relatedBuilder.AppendLine("    <td width=\"15px\">&nbsp;</td>");
                    relatedBuilder.AppendLine("    <td width=\"150px\" valign=\"top\"><b>" + thisTitle.Relationship + ": </b></td>");
                    relatedBuilder.AppendLine("    <td>" + thisTitle.Title_And_Link.Replace("<%URL_OPTS%>", url_opts) + "</td>");
                    relatedBuilder.AppendLine("  <tr>");
                }
                relatedBuilder.AppendLine("</table>");
                secondLiteral.Text = "" + Environment.NewLine  + relatedBuilder + "</div><!-- FINISHING -->" + Environment.NewLine + "</td>" + Environment.NewLine ;
            }
            else
            {
                secondLiteral.Text = "" + Environment.NewLine + "            </div><!-- FINISHING -->" + Environment.NewLine + "</td>" + Environment.NewLine ;
            }
            placeHolder.Controls.Add(secondLiteral);

        }

        /// <summary> Builds a literal with the list of volumes associated with the same title as a digital resource </summary>
        /// <param name="Volumes"> Source datatable with all affiliated volumes </param>
        /// <returns> Literal populated with the list of volumes in HTML </returns>
        protected internal Literal Add_List( DataTable Volumes )
        {
            // Save the current viewer code
            string current_view_code = CurrentMode.ViewerCode;
            ushort current_view_page = CurrentMode.Page;

            // Compute the base redirect URL
            string current_vid = CurrentMode.VID;
            CurrentMode.VID = "<%VID%>";
            string redirect_url = CurrentMode.Redirect_URL(String.Empty);
            CurrentMode.VID = current_vid;

            // Get the column references for speed
            DataColumn level1_text_column = Volumes.Columns[2];
            DataColumn level2_text_column = Volumes.Columns[4];
            DataColumn level3_text_column = Volumes.Columns[6];
            DataColumn vid_column = Volumes.Columns[13];
            DataColumn title_column = Volumes.Columns[1];
            DataColumn restriction_column = Volumes.Columns[14];

            // Make a view
            DataView vidSorted = new DataView(Volumes) {Sort = "VID ASC"};

            // Determine the max depth on this item
            int depth = 1;
            foreach (DataRowView thisItem in vidSorted)
            {
                if ((thisItem.Row[ level2_text_column ].ToString().Length > 0) && ( depth < 2 ))
                {
                    depth = 2;
                }
                if ((thisItem.Row[level3_text_column].ToString().Length > 0) && (depth < 3))
                {
                    depth = 3;
                }
            }

            // Start the table
            StringBuilder builder = new StringBuilder(5000);
            builder.AppendLine("<br />");
            builder.AppendLine("<table border=\"0px\" cellpadding=\"5px\" cellspacing=\"0px\" width=\"750px\">");
            builder.AppendLine("  <tr align=\"left\" bgcolor=\"#0022a7\">");
            builder.AppendLine("    <th width=\"50px\"><span style=\"color: White\">VID</span></th>");
            builder.AppendLine("    <th><span style=\"color: White\">LEVEL 1</span></th>");
            if (depth > 1)
            {
                builder.AppendLine("    <th><span style=\"color: White\">LEVEL 2</span></th>");
            }
            if (depth > 2)
            {
                builder.AppendLine("    <th><span style=\"color: White\">LEVEL 3</span></th>");
            }
            builder.AppendLine("    <th width=\"65px\"><span style=\"color: White\">ACCESS</span></th>");
            builder.AppendLine("  </tr>");

            foreach (DataRowView thisItem in vidSorted)
            {
                builder.AppendLine("  <tr>");
                builder.AppendLine("    <td><a href=\"" + redirect_url.Replace("<%VID%>", thisItem.Row[vid_column].ToString()) + "\">" + thisItem.Row[vid_column] + "</a></td>");
                if (thisItem.Row[level1_text_column].ToString().Length == 0)
                {
                    builder.AppendLine("    <td>" + thisItem.Row[title_column] + "</td>");
                }
                else
                {
                    builder.AppendLine("    <td>" + thisItem.Row[level1_text_column] + "</td>");
                }
                if (depth > 1)
                {
                    builder.AppendLine("    <td>" + thisItem.Row[level2_text_column] + "</td>");
                }
                if (depth > 2)
                {
                    builder.AppendLine("    <td>" + thisItem.Row[level3_text_column] + "</td>");
                }

                int access_int = Convert.ToInt32(thisItem.Row[restriction_column]);
                if (access_int < 0)
                {
                    builder.AppendLine("    <td>private</td>");
                }
                else
                {
                    builder.AppendLine(access_int == 0 ? "    <td>public</td>" : "    <td>restricted</td>");
                }

                builder.AppendLine("  </tr>");
                builder.AppendLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"" + (depth + 2) + "\"></td></tr>");
            }
            builder.AppendLine("</table>");

            // Restore the mode
            CurrentMode.ViewerCode = current_view_code;
            CurrentMode.Page = current_view_page;

            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            return mainLiteral;
        }

        /// <summary> Builds a literal with a collection of thumbnails for volumes associated with the same title as a digital resource </summary>
        /// <param name="Volumes"> Source datatable with all affiliated volumes </param>
        /// <returns> Literal populated with the thumbnails for the volumes in HTML </returns>
        protected internal Literal Add_Thumbnails(DataTable Volumes)
        {
            if (CurrentMode.SubPage <= 0)
                CurrentMode.SubPage = 1;

            // Is this a newspaper?
            bool newspaper = false;
            if ( CurrentItem.Behaviors.GroupType.ToUpper() == "NEWSPAPER")
            {
                newspaper = true;
            }

            // Build the value
            StringBuilder builder = new StringBuilder(5000);

            // Save the current viewer code
            string current_view_code = CurrentMode.ViewerCode;
            ushort current_view_page = CurrentMode.Page;

            // Start this table
            string width_statement = String.Empty;
            if (Volumes.Rows.Count > 2)
            {
                width_statement = " width=\"33%\"";
            }

            builder.AppendLine("<table align=\"center\" width=\"650px\" cellspacing=\"15px\">");
            builder.AppendLine("\t<tr valign=\"top\">");

            // Find the base address for this thumbnail
            string jpeg_base = (SobekCM_Library_Settings.Image_URL + CurrentItem.Web.File_Root + "/").Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

            // Compute the base redirect URL
            string current_vid = CurrentMode.VID;
            string viewercode = CurrentMode.ViewerCode;
            ushort subpage = CurrentMode.SubPage;
            CurrentMode.ViewerCode = String.Empty;
            CurrentMode.SubPage = 0;
            CurrentMode.VID = "<%VID%>";
            string redirect_url = CurrentMode.Redirect_URL(String.Empty);
            CurrentMode.ViewerCode = viewercode;
            CurrentMode.SubPage = subpage;
            CurrentMode.VID = current_vid;

            // Get the data columns for quick access
            DataColumn level1_text_column = Volumes.Columns[2];
            DataColumn level2_text_column = Volumes.Columns[4];
            DataColumn level3_text_column = Volumes.Columns[6];
            DataColumn vid_column = Volumes.Columns[13];
            DataColumn title_column = Volumes.Columns[1];
            DataColumn itemid_column = Volumes.Columns[0];
            DataColumn main_thumbnail_column = Volumes.Columns[12];
            DataColumn visibility_column = Volumes.Columns[14];

            // Step through item in the results
            int col = 0;
            int startItemCount = (CurrentMode.SubPage - 1) * 60;
            int endItemCount = (CurrentMode.SubPage) * 60;
            if (Volumes.Rows.Count < 100)
                endItemCount = Volumes.Rows.Count;
            for ( int i = startItemCount ; ( i < endItemCount ) && ( i < Volumes.Rows.Count ) ; i++ )
            {
                DataRow thisItem = Volumes.Rows[i];

                if (Convert.ToInt16(thisItem[visibility_column]) >= 0)
                {

                    // Should a new row be started
                    if (col == 3)
                    {
                        col = 0;
                        builder.AppendLine("\t</tr>");
                        builder.AppendLine("\t<tr valign=\"top\">");
                    }

                    string thumbnail_text = thisItem[title_column].ToString();
                    if (thisItem[level1_text_column].ToString().Length > 0)
                    {
                        if (thisItem[level2_text_column].ToString().Length > 0)
                        {
                            if (thisItem[level3_text_column].ToString().Length > 0)
                            {
                                if (newspaper)
                                {
                                    thumbnail_text = thisItem[level2_text_column] + " " + thisItem[level3_text_column] + ", " + thisItem[level1_text_column];
                                }
                                else
                                {
                                    thumbnail_text = thisItem[level1_text_column] + ". " + thisItem[level2_text_column] + ". " + thisItem[level3_text_column];
                                }
                            }
                            else
                            {
                                thumbnail_text = thisItem[level1_text_column] + ". " + thisItem[level2_text_column] + ".";
                            }
                        }
                        else
                        {
                            thumbnail_text = thisItem[level1_text_column] + ".";
                        }
                    }

                    if (Volumes.Rows.Count > 2)
                    {
                        // Start this table section
                        if (col == 0)
                        {
                            builder.Append("\t\t<td align=\"left\"" + width_statement + ">");
                        }
                        if (col == 1)
                        {
                            builder.Append("\t\t<td align=\"center\"" + width_statement + ">");
                        }
                        if (col == 2)
                        {
                            builder.Append("\t\t<td align=\"right\"" + width_statement + ">");
                        }
                    }
                    else
                    {
                        builder.Append("\t\t<td align=\"center\"" + width_statement + ">");
                    }

                    string vid = thisItem[vid_column].ToString();
                    string url = redirect_url.Replace("<%VID%>", vid).Replace("&", "&amp;");
                    if (Convert.ToInt32(thisItem[itemid_column]) == CurrentItem.Web.ItemID)
                    {
                        builder.AppendLine("<table width=\"170px\" onmouseover=\"this.className='thumbnailHighlight'\" onmouseout=\"this.className='thumbnailNormal'\" onmousedown=\"window.location.href='" + url + "';\"><tr><td><a href=\"" + url + "\"><img src=\"" + jpeg_base + vid + "/" + thisItem[main_thumbnail_column] + "\" alt=\"MISSING THUMBNAIL\" /></a></td></tr><tr><td align=\"center\"><span class=\"SobekThumbnailText\"><i>" + thumbnail_text + "</i></span></td></tr></table></td>" );
                    }
                    else
                    {
                        builder.AppendLine("<table width=\"170px\" onmouseover=\"this.className='thumbnailHighlight'\" onmouseout=\"this.className='thumbnailNormal'\" onmousedown=\"window.location.href='" + url + "';\"><tr><td><a href=\"" + url + "\"><img src=\"" + jpeg_base + vid + "/" + thisItem[main_thumbnail_column] + "\" alt=\"MISSING THUMBNAIL\" /></a></td></tr><tr><td align=\"center\"><span class=\"SobekThumbnailText\">" + thumbnail_text + "</span></td></tr></table></td>" );
                    }
                    col++;
                }
            }

            // End this table
            builder.AppendLine("\t</tr>");
            builder.AppendLine("</table>");

            // Restore the mode
            CurrentMode.ViewerCode = current_view_code;
            CurrentMode.Page = current_view_page;

            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            return mainLiteral;
        }

        /// <summary> Populates a tree view control with the hierarchical collection of volumes associated with the same title as a digital resource </summary>
        /// <param name="treeView1"> Treeview control to populate with the associated volumes </param>
        /// <param name="Volumes"> Source datatable with all affiliated volumes </param>
        protected internal void Build_Tree(TreeView treeView1, DataTable Volumes)
        {
            const int LINE_TO_LONG = 100;

            // Add the root node
            TreeNode rootNode = new TreeNode("<b>" + CurrentItem.Behaviors.GroupTitle + "</b>");
            if (CurrentItem.Behaviors.GroupTitle.Length > LINE_TO_LONG)
                rootNode.Text = "<b>" + CurrentItem.Behaviors.GroupTitle.Substring(0, LINE_TO_LONG) + "...</b>";
            rootNode.SelectAction = TreeNodeSelectAction.None;
            treeView1.Nodes.Add(rootNode);

            // Is this a newspaper?
            bool newspaper = false;
            if (CurrentItem.Behaviors.GroupType.ToUpper() == "NEWSPAPER")
            {
                newspaper = true;
            }

            // Add the first layer of nodes
          //  Hashtable nodeHash = new Hashtable();
            string lastNodeText1 = String.Empty;
            string lastNodeText2 = String.Empty;
            string lastNodeText3 = String.Empty;
            string lastNodeText4 = String.Empty;
            TreeNode lastNode1 = null;
            TreeNode lastNode2 = null;
            TreeNode lastNode3 = null;
            TreeNode lastNode4 = null;
            TreeNode currentSelectedNode = null;

            // Compute the base redirect URL
            string current_vid = CurrentMode.VID;
            CurrentMode.VID = "<%VID%>";
            string redirect_url = CurrentMode.Redirect_URL(String.Empty);
            CurrentMode.VID = current_vid;

            // Get the data columns for quick access
            DataColumn level1_text_column = Volumes.Columns[2];
            DataColumn level2_text_column = Volumes.Columns[4];
            DataColumn level3_text_column = Volumes.Columns[6];
            DataColumn level4_text_column = Volumes.Columns[8];
            DataColumn level5_text_column = Volumes.Columns[10];
            DataColumn vid_column = Volumes.Columns[13];
            DataColumn title_column = Volumes.Columns[1];
            DataColumn itemid_column = Volumes.Columns[0];
            DataColumn visibility_column = Volumes.Columns[14];

            //DataColumn level1_index_column = Volumes.Columns[3];
            //DataColumn level2_index_column = Volumes.Columns[5];
            //DataColumn level3_index_column = Volumes.Columns[7];
            //DataColumn level4_index_column = Volumes.Columns[9];
            //DataColumn level5_index_column = Volumes.Columns[11];

            foreach (DataRow thisItem in Volumes.Rows)
            {
                // Do not show PRIVATE items in this tree view
                if (Convert.ToInt16(thisItem[visibility_column]) >= 0)
                {
                    string level1_text = translator.Get_Translation(thisItem[level1_text_column].ToString(), CurrentMode.Language);
                    string level2_text = String.Empty;
                    string level3_text = String.Empty;
                    string level4_text = String.Empty;
                    string level5_text = String.Empty;
                    string title = thisItem[title_column].ToString();
                    string vid = thisItem[vid_column].ToString();
                    int itemid = Convert.ToInt32(thisItem[itemid_column]);
                    if (level1_text.Length == 0)
                    {
                        TreeNode singleNode = new TreeNode(title);
                        if (title.Length > LINE_TO_LONG)
                            singleNode.Text = title.Substring(0, LINE_TO_LONG) + "...";
                        if (itemid == CurrentItem.Web.ItemID)
                        {
                            currentSelectedNode = singleNode;
                            singleNode.SelectAction = TreeNodeSelectAction.None;
                            singleNode.Text = "<i>" + singleNode.Text + "</i>";

                        }
                        else
                        {
                            singleNode.NavigateUrl = redirect_url.Replace("<%VID%>", vid);
                        }
                        rootNode.ChildNodes.Add(singleNode);
                    }

                    // Look at the first level
                    if (level1_text.Length > 0)
                    {
                        level2_text = translator.Get_Translation(thisItem[level2_text_column].ToString(), CurrentMode.Language);
                        if (level2_text.Length == 0)
                        {
                            TreeNode singleNode1 = new TreeNode(level1_text);
                            if (thisItem[level1_text_column].ToString().Length > LINE_TO_LONG)
                            {
                                singleNode1.ToolTip = singleNode1.Text;
                                singleNode1.Text = level1_text.Substring(0, LINE_TO_LONG) + "...";
                            }
                            if (itemid == CurrentItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode1;
                                singleNode1.SelectAction = TreeNodeSelectAction.None;
                                singleNode1.Text = "<i>" + singleNode1.Text + "</i>";

                            }
                            else
                            {
                                singleNode1.NavigateUrl = redirect_url.Replace("<%VID%>", vid);
                            }
                            rootNode.ChildNodes.Add(singleNode1);
                        }
                        else
                        {
                            if ((lastNode1 == null) || (lastNodeText1 != level1_text.ToUpper()))
                            {
                                lastNode1 = new TreeNode(level1_text);
                                if (level1_text.Length > LINE_TO_LONG)
                                {
                                    lastNode1.ToolTip = lastNode1.Text;
                                    lastNode1.Text = level1_text.Substring(0, LINE_TO_LONG) + "...";
                                }
                                lastNode1.SelectAction = TreeNodeSelectAction.None;

                                lastNodeText1 = level1_text.ToUpper();
                                rootNode.ChildNodes.Add(lastNode1);

                                lastNode2 = null;
                                lastNodeText2 = String.Empty;
                                lastNode3 = null;
                                lastNodeText3 = String.Empty;
                                lastNode4 = null;
                                lastNodeText4 = String.Empty;
                            }
                        }
                    }

                    // Look at the second level
                    if ((level2_text.Length > 0) && (lastNode1 != null))
                    {
                        level3_text = translator.Get_Translation(thisItem[level3_text_column].ToString(), CurrentMode.Language);
                        if (level3_text.Length == 0)
                        {
                            TreeNode singleNode2 = new TreeNode(level2_text);
                            if (level2_text.Length > LINE_TO_LONG)
                            {
                                singleNode2.ToolTip = singleNode2.Text;
                                singleNode2.Text = level2_text.Substring(0, LINE_TO_LONG) + "...";
                            }
                            if (itemid == CurrentItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode2;
                                singleNode2.SelectAction = TreeNodeSelectAction.None;
                                singleNode2.Text = "<i>" + singleNode2.Text + "</i>";
                            }
                            else
                            {
                                singleNode2.NavigateUrl = redirect_url.Replace("<%VID%>", vid);
                            }
                            lastNode1.ChildNodes.Add(singleNode2);
                        }
                        else
                        {
                            if ((lastNode2 == null) || (lastNodeText2 != level2_text.ToUpper()))
                            {
                                lastNode2 = new TreeNode(level2_text);
                                if (level2_text.Length > LINE_TO_LONG)
                                {
                                    lastNode2.ToolTip = lastNode2.Text;
                                    lastNode2.Text = level2_text.Substring(0, LINE_TO_LONG) + "...";
                                }
                                lastNode2.SelectAction = TreeNodeSelectAction.None;
                                lastNodeText2 = level2_text.ToUpper();
                                lastNode1.ChildNodes.Add(lastNode2);

                                lastNode3 = null;
                                lastNodeText3 = String.Empty;
                                lastNode4 = null;
                                lastNodeText4 = String.Empty;
                            }
                        }
                    }

                    // Look at the third level
                    if ((level3_text.Length > 0) && (lastNode2 != null))
                    {
                        level4_text = translator.Get_Translation(thisItem[level4_text_column].ToString(), CurrentMode.Language);
                        if (level4_text.Length == 0)
                        {
                            TreeNode singleNode3 = new TreeNode(level3_text);
                            if (level3_text.Length > LINE_TO_LONG)
                            {
                                singleNode3.ToolTip = singleNode3.Text;
                                singleNode3.Text = level3_text.Substring(0, LINE_TO_LONG) + "...";
                            }
                            if (newspaper)
                            {
                                singleNode3.Text = level2_text + " " + level3_text + ", " + level1_text;
                            }

                            if (itemid == CurrentItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode3;
                                singleNode3.SelectAction = TreeNodeSelectAction.None;
                                singleNode3.Text = "<i>" + singleNode3.Text + "</i>";
                            }
                            else
                            {
                                singleNode3.NavigateUrl = redirect_url.Replace("<%VID%>", vid);
                            }

                            lastNode2.ChildNodes.Add(singleNode3);
                        }
                        else
                        {
                            if ((lastNode3 == null) || (lastNodeText3 != level3_text.ToUpper()))
                            {
                                lastNode3 = new TreeNode(level3_text);
                                if (level3_text.Length > LINE_TO_LONG)
                                {
                                    lastNode3.ToolTip = lastNode3.Text;
                                    lastNode3.Text = level3_text.Substring(0, LINE_TO_LONG) + "...";
                                }
                                lastNode3.SelectAction = TreeNodeSelectAction.None;
                                lastNodeText3 = level3_text.ToUpper();
                                lastNode2.ChildNodes.Add(lastNode3);

                                lastNode4 = null;
                                lastNodeText4 = String.Empty;
                            }
                        }
                    }

                    // Look at the fourth level
                    if ((level4_text.Length > 0) && (lastNode3 != null))
                    {
                        translator.Get_Translation(thisItem[level5_text_column].ToString(), CurrentMode.Language);
                        if (level5_text.Length == 0)
                        {
                            TreeNode singleNode4 = new TreeNode(level4_text);
                            if (level4_text.Length > LINE_TO_LONG)
                            {
                                singleNode4.ToolTip = singleNode4.Text;
                                singleNode4.Text = level4_text.Substring(0, LINE_TO_LONG) + "...";
                            }
                            if (itemid == CurrentItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode4;
                                singleNode4.SelectAction = TreeNodeSelectAction.None;
                                singleNode4.Text = "<i>" + singleNode4.Text + "</i>";
                            }
                            else
                            {
                                singleNode4.NavigateUrl = redirect_url.Replace("<%VID%>", vid);
                            }
                            lastNode3.ChildNodes.Add(singleNode4);
                        }
                        else
                        {
                            if ((lastNode4 == null) || (lastNodeText4 != level4_text.ToUpper()))
                            {
                                lastNode4 = new TreeNode(level4_text);
                                if (level4_text.Length > LINE_TO_LONG)
                                {
                                    lastNode4.ToolTip = lastNode4.Text;
                                    lastNode4.Text = level4_text.Substring(0, LINE_TO_LONG) + "...";
                                }
                                lastNode4.SelectAction = TreeNodeSelectAction.None;
                                lastNodeText4 = level4_text.ToUpper();
                                lastNode3.ChildNodes.Add(lastNode4);
                            }
                        }
                    }

                    // Look at the fifth level
                    if ((level5_text.Length > 0) && (lastNode4 != null))
                    {
                        TreeNode lastNode5 = new TreeNode(level5_text);
                        if (level5_text.Length > LINE_TO_LONG)
                        {
                            lastNode5.ToolTip = lastNode5.Text;
                            lastNode5.Text = level5_text.Substring(0, LINE_TO_LONG) + "...";
                        }
                        if (itemid == CurrentItem.Web.ItemID)
                        {
                            currentSelectedNode = lastNode5;
                            lastNode5.SelectAction = TreeNodeSelectAction.None;
                            lastNode5.Text = "<i>" + lastNode5.Text + "</i>";
                        }
                        else
                        {
                            lastNode5.NavigateUrl = redirect_url.Replace("<%VID%>", vid);
                        }
                        lastNode4.ChildNodes.Add(lastNode5);
                    }
                }
            }

            rootNode.CollapseAll();
            rootNode.Expand();

            if (currentSelectedNode == null) return;

            while ((currentSelectedNode != rootNode) && ( currentSelectedNode != null ))
            {
                currentSelectedNode.Expand();
                currentSelectedNode = currentSelectedNode.Parent;
            }
        }

        #region Nested type: View_Type

        private enum View_Type : byte { Tree = 1, Thumbnail, List };

        #endregion
    }
}
