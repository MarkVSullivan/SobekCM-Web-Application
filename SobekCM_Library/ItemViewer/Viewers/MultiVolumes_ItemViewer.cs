#region Using directives

using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object.Behaviors;
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
        private string issues_type;
        private View_Type viewType;
        private int thumbnail_count;

        /// <summary> Sets the list of all the items within this title to be displayed </summary>
        public SobekCM_Items_In_Title Item_List { private get; set; }

        /// <summary> This provides an opportunity for the viewer to perform any pre-display work
        /// which is necessary before entering any of the rendering portions </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> A good amount of work is done here.  Since the TREE view is done through controls and the other views are written 
        /// directly to the stream, work is done here to prevent having to duplicate the code.  </remarks>
        public override void Perform_PreDisplay_Work(Custom_Tracer Tracer)
        {
            // Determine the citation type
            viewType = View_Type.Tree;
            switch (CurrentMode.ViewerCode)
            {
                case "allvolumes2":
                    viewType = View_Type.Thumbnail;
                    break;

                case "allvolumes3":
                    viewType = View_Type.List;
                    break;
            }

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

            issues_type = volumes_text;
            if (CurrentItem.Behaviors.GroupType.ToUpper().IndexOf("NEWSPAPER") >= 0)
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

            // If the view type is the thumbnails (where we only show PUBLIC and RESTRICTED items) 
            // need a count of the number of public items
            thumbnail_count = 1;
            if (viewType == View_Type.Thumbnail)
            {
                thumbnail_count = Item_List.Item_Table.Select("(IP_Restriction_Mask >= 0 ) and ( Dark = 'false')").Length;
            }
        }

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

                if (viewType != View_Type.Thumbnail )
                    return 1;

                if (CurrentMode.SubPage <= 0)
                    CurrentMode.SubPage = 1;

                if (thumbnail_count <= 100)
                    return 1;

                return (((thumbnail_count - 1) / 60)) + 1;
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
        /// <value> For tree views, this returns 750 (pixels) otherwise it will take the full screen. </value>
        public override int Viewer_Width
        {
            get
            {
	            return viewType == View_Type.Tree ? 750 : -1;
            }
        }

        /// <summary> Gets the url to go to the first page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public override string First_Page_URL
        {
            get
            {
                if (viewType != View_Type.Thumbnail )
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
                if (viewType != View_Type.Thumbnail)
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
                if (viewType != View_Type.Thumbnail)
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
                if (viewType != View_Type.Thumbnail)
                    return String.Empty;

                ushort subpage = CurrentMode.SubPage;
                CurrentMode.SubPage = (ushort)((thumbnail_count / 60));
                if ((thumbnail_count % 60) != 0)
                    CurrentMode.SubPage = (ushort)(CurrentMode.SubPage + 1);
                string returnVal = CurrentMode.Redirect_URL();
                CurrentMode.SubPage = subpage;
                return returnVal;
            }
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(System.IO.TextWriter Output, Custom_Tracer Tracer)
        {
            if (viewType != View_Type.Tree)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("MultiVolumes_ItemViewer.Write_Main_Viewer_Section", "Write the main viewer section (for tree and list view)");
                }

                // Build the value
                Output.WriteLine("          <td><div id=\"sbkMviv_ViewerTitle\">" + issues_type + "</div></td>");
                Output.WriteLine("        </tr>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                if (viewType == View_Type.Thumbnail)
                    Output.WriteLine("            <div id=\"sbkMviv_ThumbnailsArea\">");
                else
                    Output.WriteLine("            <div id=\"sbkMviv_MainArea\">");

                if (viewType == View_Type.List)
                {
                    Write_List(Output, Item_List.Item_Table);
                }
                else if (viewType == View_Type.Thumbnail)
                {
                    Write_Thumbnails(Output, Item_List.Item_Table);
                }
            }
        }


        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            // Add the tree view as controls
            if (viewType == View_Type.Tree)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("MultiVolumes_ItemViewer.Add_Main_Viewer_Section", "Adds tree view control");
                }

                // Build the value
                StringBuilder builder = new StringBuilder(5000);
                builder.AppendLine("          <td><div id=\"sbkMviv_ViewerTitle\">" + issues_type + "</div></td>");
                builder.AppendLine("        </tr>");
                builder.AppendLine("        <tr>");
                builder.AppendLine("          <td>");
                if (viewType == View_Type.Thumbnail)
                    builder.AppendLine("            <div id=\"sbkMviv_ThumbnailsArea\">");
                else
                    builder.AppendLine("            <div id=\"sbkMviv_MainArea\">");

                // Add the HTML for the image
                Literal mainLiteral = new Literal {Text = builder.ToString()};
                placeHolder.Controls.Add(mainLiteral);

                // Add the tree view
                TreeView treeView1 = new TreeView {EnableViewState = false, CssClass = "sbkMviv_Tree"};
                Build_Tree(treeView1, Item_List.Item_Table);
                placeHolder.Controls.Add(treeView1);
            }

            // Add the final HTML
            Literal secondLiteral = new Literal();
            if (CurrentItem.Web.Related_Titles_Count > 0)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("MultiVolumes_ItemViewer.Add_Main_Viewer_Section", "Add the related titles and close the remaining divs");
                }

                StringBuilder relatedBuilder = new StringBuilder(1000);
                relatedBuilder.AppendLine("<table id=\"sbkMviv_RelatedTitles\">");
                relatedBuilder.AppendLine("  <tr>");
                relatedBuilder.AppendLine("    <td colspan=\"2\"><h2>Related Titles</h2></td>");
                relatedBuilder.AppendLine("  </tr>");
                string url_opts = CurrentMode.URL_Options();
                foreach (Related_Titles thisTitle in CurrentItem.Web.All_Related_Titles)
                {
                    relatedBuilder.AppendLine("  <tr>");
                    relatedBuilder.AppendLine("    <td class=\"sbkMviv_RelatedTitlesRelation\">" + thisTitle.Relationship + ": </td>");
                    relatedBuilder.AppendLine("    <td>" + thisTitle.Title_And_Link.Replace("<%URL_OPTS%>", url_opts) + "</td>");
                    relatedBuilder.AppendLine("  <tr>");
                }
                relatedBuilder.AppendLine("</table>");
                secondLiteral.Text = "" + Environment.NewLine  + relatedBuilder + "</div><!-- FINISHING -->" + Environment.NewLine + "</td>" + Environment.NewLine ;
            }
            else
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("MultiVolumes_ItemViewer.Add_Main_Viewer_Section", "Close the remaining divs");
                }

                secondLiteral.Text = "" + Environment.NewLine + "            </div><!-- FINISHING -->" + Environment.NewLine + "</td>" + Environment.NewLine ;
            }
            placeHolder.Controls.Add(secondLiteral);
        }

        /// <summary> Writes the list of volumes associated with the same title as a digital resource to the output stream </summary>
        /// <param name="Output"> HTML output response stream </param>
        /// <param name="Volumes"> Source datatable with all affiliated volumes </param>
        protected internal void Write_List(System.IO.TextWriter Output, DataTable Volumes)
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
            Output.WriteLine("<table id=\"sbkMviv_Table\">");
            Output.WriteLine("  <tr id=\"sbkMviv_TableHeaderRow\">");
            Output.WriteLine("    <th style=\"width:50px;\">VID</th>");
            Output.WriteLine("    <th>LEVEL 1</th>");
            if (depth > 1)
            {
                Output.WriteLine("    <th>LEVEL 2</th>");
            }
            if (depth > 2)
            {
                Output.WriteLine("    <th>LEVEL 3</th>");
            }
            Output.WriteLine("    <th style=\"width:65px;\">ACCESS</th>");
            Output.WriteLine("  </tr>");

            foreach (DataRowView thisItem in vidSorted)
            {
                int access_int = Convert.ToInt32(thisItem.Row[restriction_column]);
                if (access_int < 0)
                {
                    Output.WriteLine("  <tr class=\"sbkMviv_TablePrivateItem\">");
                }
                else
                {
                    Output.WriteLine(access_int == 0 ? "  <tr class=\"sbkMviv_TablePublicItem\">" : "  <tr class=\"sbkMviv_TableRestrictedItem\">");
                }

                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td><a href=\"" + redirect_url.Replace("<%VID%>", thisItem.Row[vid_column].ToString()) + "\">" + thisItem.Row[vid_column] + "</a></td>");
                if (thisItem.Row[level1_text_column].ToString().Length == 0)
                {
                    Output.WriteLine("    <td>" + thisItem.Row[title_column] + "</td>");
                }
                else
                {
                    Output.WriteLine("    <td>" + thisItem.Row[level1_text_column] + "</td>");
                }
                if (depth > 1)
                {
                    Output.WriteLine("    <td>" + thisItem.Row[level2_text_column] + "</td>");
                }
                if (depth > 2)
                {
                    Output.WriteLine("    <td>" + thisItem.Row[level3_text_column] + "</td>");
                }

                if (access_int < 0)
                {
                    Output.WriteLine("    <td>private</td>");
                }
                else
                {
                    Output.WriteLine(access_int == 0 ? "    <td>public</td>" : "    <td>restricted</td>");
                }

                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td class=\"sbkMviv_TableRowSeperator\" colspan=\"" + (depth + 2) + "\"></td></tr>");
            }
            Output.WriteLine("</table>");

            // Restore the mode
            CurrentMode.ViewerCode = current_view_code;
            CurrentMode.Page = current_view_page;
        }

        /// <summary> Writes the collection of thumbnails for volumes associated with the same title as a digital resource to the output stream </summary>
        /// <param name="Volumes"> Source datatable with all affiliated volumes </param>
        /// <param name="Output"> HTML output response stream </param>
        protected internal void Write_Thumbnails(System.IO.TextWriter Output, DataTable Volumes)
        {
            if (CurrentMode.SubPage <= 0)
                CurrentMode.SubPage = 1;

            // Is this a newspaper?
            bool newspaper = CurrentItem.Behaviors.GroupType.ToUpper() == "NEWSPAPER";

            // Save the current viewer code
            string current_view_code = CurrentMode.ViewerCode;
            ushort current_view_page = CurrentMode.Page;

            // Start this table
            string width_statement = String.Empty;
            if (Volumes.Rows.Count > 2)
            {
                width_statement = " width=\"33%\"";
            }

            //Outer div which contains all the thumbnails
            Output.WriteLine("<div style=\"margin:5px;text-align:center;\">");

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

            // Get the rows which match the requirements (either PUBLIC or RESTRICTED)
            DataRow[] matches = Volumes.Select("(IP_Restriction_Mask >= 0 ) and ( Dark = 'false')");

            // Step through item in the results
            int col = 0;
            int startItemCount = (CurrentMode.SubPage - 1) * 60;
            int endItemCount = (CurrentMode.SubPage) * 60;
            if (Volumes.Rows.Count < 100)
                endItemCount = matches.Length;
            for (int i = startItemCount; (i < endItemCount) && (i < matches.Length); i++)
            {
                DataRow thisItem = matches[i];

                if (Convert.ToInt16(thisItem[visibility_column]) >= 0)
                {
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

                    string vid = thisItem[vid_column].ToString();
                    string url = redirect_url.Replace("<%VID%>", vid).Replace("&", "&amp;");

                    if (Convert.ToInt32(thisItem[itemid_column]) == CurrentItem.Web.ItemID)
                    {
                        Output.WriteLine("  <table class=\"sbkMviv_Thumbnail\" id=\"sbkMviv_ThumbnailCurrent\">");
                    }
                    else
                    {
                        Output.WriteLine("  <table class=\"sbkMviv_Thumbnail\">");
                    }
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <td>");
                    Output.WriteLine("        <a href=\"" + url + "\" title=\"" + thumbnail_text + "\">");
                    Output.WriteLine("          <img src=\"" + jpeg_base + vid + "/" + thisItem[main_thumbnail_column] + "\" alt=\"MISSING THUMBNAIL\" />");
                    Output.WriteLine("        </a>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <td style=\"align:center\">" + thumbnail_text + "</td>");
                    Output.WriteLine("    </tr>");
                    Output.WriteLine("  </table>");
                    Output.WriteLine();
                }
            }

            //Close the outer div
            Output.WriteLine("</div>");

            // Restore the mode
            CurrentMode.ViewerCode = current_view_code;
            CurrentMode.Page = current_view_page;
        }

        /// <summary> Populates a tree view control with the hierarchical collection of volumes associated with the same title as a digital resource </summary>
        /// <param name="treeView1"> Treeview control to populate with the associated volumes </param>
        /// <param name="Volumes"> Source datatable with all affiliated volumes </param>
        protected internal void Build_Tree(TreeView treeView1, DataTable Volumes)
        {
            const int LINE_TO_LONG = 100;

            // Add the root node
            TreeNode rootNode = new TreeNode("<span id=\"sbkMviv_TableGroupTitle\">" + CurrentItem.Behaviors.GroupTitle + "</span>");
            if (CurrentItem.Behaviors.GroupTitle.Length > LINE_TO_LONG)
                rootNode.Text = "<span id=\"sbkMviv_TableGroupTitle\">" + CurrentItem.Behaviors.GroupTitle.Substring(0, LINE_TO_LONG) + "...</span>";
            rootNode.SelectAction = TreeNodeSelectAction.None;
            treeView1.Nodes.Add(rootNode);

            // Is this a newspaper?
            bool newspaper = CurrentItem.Behaviors.GroupType.ToUpper() == "NEWSPAPER";

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
            DataColumn dark_column = Volumes.Columns[16];

            //DataColumn level1_index_column = Volumes.Columns[3];
            //DataColumn level2_index_column = Volumes.Columns[5];
            //DataColumn level3_index_column = Volumes.Columns[7];
            //DataColumn level4_index_column = Volumes.Columns[9];
            //DataColumn level5_index_column = Volumes.Columns[11];

            // Does this user have special rights on the item?
            bool specialRights = ((CurrentUser != null) && ((CurrentUser.Is_System_Admin) || (CurrentUser.Is_Internal_User) || (CurrentUser.Can_Edit_This_Item(CurrentItem))));

            foreach (DataRow thisItem in Volumes.Rows)
            {
                // Do not show PRIVATE items in this tree view
                int access_int = Convert.ToInt32(thisItem[visibility_column]);
                bool dark = Convert.ToBoolean(thisItem[dark_column]);
                if (dark) access_int = -1;
                if (( access_int >= 0) || (specialRights))
                {
                    // Set the access string and span name
                    string access_string = String.Empty;
                    string access_span_start = String.Empty;
                    string access_span_end = String.Empty;
                    if (dark)
                    {
                        access_span_start = "<span class=\"sbkMviv_TreeDarkNode\">";
                        access_string = " ( dark )";
                        access_span_end = "</span>";
                    }
                    else
                    {
                        if (access_int < 0)
                        {
                            access_span_start = "<span class=\"sbkMviv_TreePrivateNode\">";
                            access_string = " ( private )";
                            access_span_end = "</span>";
                        }
                        else if (access_int > 0)
                        {
                            access_span_start = "<span class=\"sbkMviv_TreeRestrictedNode\">";
                            access_string = " ( some restrictions apply )";
                            access_span_end = "</span>";
                        }
                    }

                    // Determine the text for all the levels (and nodes)
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
                            singleNode.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + singleNode.Text + "</span>";

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
                            TreeNode singleNode1 = new TreeNode(access_span_start + level1_text + access_string + access_span_end);
                            if (thisItem[level1_text_column].ToString().Length > LINE_TO_LONG)
                            {
                                singleNode1.ToolTip = level1_text;
                                level1_text = level1_text.Substring(0, LINE_TO_LONG) + "...";
                                singleNode1.Text = access_span_start + level1_text + access_string + access_span_end;
                            }
                            if (itemid == CurrentItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode1;
                                singleNode1.SelectAction = TreeNodeSelectAction.None;
                                singleNode1.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + singleNode1.Text + access_string + "</span>"; ;

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
                                // Since this is the TOP level, let's look down and see if there are any non-private, non-dark items
                                string nontranslated = thisItem[level1_text_column].ToString();
                                int index = Convert.ToInt32(thisItem["Level1_Index"]);
                                bool allPrivate = Volumes.Select("(IP_Restriction_Mask >= 0 ) and ( Dark = 'false') and ( " + level1_text_column.ColumnName + "='" + nontranslated + "') and ( Level1_Index=" + index + ")").Length == 0;

                                DataRow[] test = Volumes.Select("(IP_Restriction_Mask >= 0 ) and ( Dark = 'false') and ( " + level1_text_column.ColumnName + "='" + nontranslated + "') and ( Level1_Index=" + index + ")");
                                allPrivate = test.Length == 0;

                                lastNode1 = new TreeNode(level1_text);
                                if (level1_text.Length > LINE_TO_LONG)
                                {
                                    lastNode1.ToolTip = lastNode1.Text;
                                    level1_text = level1_text.Substring(0, LINE_TO_LONG) + "...";
                                    lastNode1.Text = level1_text;
                                }

                                if (allPrivate)
                                {
                                    lastNode1.Text = "<span class=\"sbkMviv_TreePrivateNode\">" + level1_text + " ( all private or dark )</span>";
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
                            TreeNode singleNode2 = new TreeNode( access_span_start + level2_text + access_string + access_span_end );
                            if (level2_text.Length > LINE_TO_LONG)
                            {
                                singleNode2.ToolTip = level2_text;
                                level2_text = level2_text.Substring(0, LINE_TO_LONG) + "...";
                                singleNode2.Text = access_span_start + level2_text + access_string + access_span_end;
                            }
                            if (itemid == CurrentItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode2;
                                singleNode2.SelectAction = TreeNodeSelectAction.None;
                                singleNode2.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + level2_text + access_string + "</span>"; ;
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
                            TreeNode singleNode3 = new TreeNode(access_span_start + level3_text + access_string + access_span_end);
                            if (level3_text.Length > LINE_TO_LONG)
                            {
                                singleNode3.ToolTip = level3_text;
                                level3_text = level3_text.Substring(0, LINE_TO_LONG) + "...";
                                singleNode3.Text = access_span_start + level3_text + access_string + access_span_end;
                            }
                            if (newspaper)
                            {
                                level3_text = access_span_start + level2_text + " " + level3_text + ", " + level1_text + access_string + access_span_end;
                                singleNode3.Text = level3_text;
                            }

                            if (itemid == CurrentItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode3;
                                singleNode3.SelectAction = TreeNodeSelectAction.None;
                                singleNode3.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + level3_text + access_string + "</span>"; ;
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
                            TreeNode singleNode4 = new TreeNode(access_span_start + level4_text + access_string + access_span_end);
                            if (level4_text.Length > LINE_TO_LONG)
                            {
                                singleNode4.ToolTip = level4_text;
                                level4_text = level4_text.Substring(0, LINE_TO_LONG) + "...";
                                singleNode4.Text = access_span_start + level4_text + access_string + access_span_end;
                            }
                            if (itemid == CurrentItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode4;
                                singleNode4.SelectAction = TreeNodeSelectAction.None;
                                singleNode4.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + level4_text + access_string + "</span>"; ;
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
                        TreeNode lastNode5 = new TreeNode(access_span_start + level5_text + access_string + access_span_end );
                        if (level5_text.Length > LINE_TO_LONG)
                        {
                            lastNode5.ToolTip = level5_text;
                            level5_text = level5_text.Substring(0, LINE_TO_LONG) + "...";
                            lastNode5.Text = access_span_start + level5_text + access_string + access_span_end;
                        }
                        if (itemid == CurrentItem.Web.ItemID)
                        {
                            currentSelectedNode = lastNode5;
                            lastNode5.SelectAction = TreeNodeSelectAction.None;
                            lastNode5.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + level5_text + access_string + "</span>"; ;
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
