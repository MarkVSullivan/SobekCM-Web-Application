#region Using directives

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using DLC.Custom_Grid;
using SobekCM.Library;
using SobekCM.Library.Database;
using SobekCM.Library.Items;
using SobekCM.Library.Search;
using SobekCM.Management_Tool.Settings;
using SobekCM.Management_Tool.Versioning;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form displays information about a single item group from within a SobekCM library </summary>
    public partial class View_Item_Group_Form : Form
    {
        #region Private form members

        private readonly string aleph;
        private BibIdReport_Printer bibIdPrinter;
        private readonly string bibid;
        private DataTable childTable;
        private bool collapsed;
        private DataTable displayTable;
        private CustomGrid_Panel gridPanel;
        private CustomGrid_Printer gridPrinter;
        private bool gridPrinterShowAllRows = true;
        private readonly string grouptitle;
        private readonly string grouptitleShortened;
        private readonly string oclc;
        private readonly DataRow titleRow;
        private readonly string type;
        private readonly string username;

        #endregion

        #region Constructor

        /// <summary> Constructor for a new instance of the View_Item_Group_Form class </summary>
        /// <param name="Title_Row"> Row of title information from the display table </param>
        public View_Item_Group_Form( DataRow Title_Row )
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            // Set some personalization and customization for the SobekCM Instance Name
            openWebContextMenuItem.Text = "Open Item in " + SobekCM_Library_Settings.System_Abbreviation;

            titleRow = Title_Row;

            bibid = titleRow["BibID"].ToString();
            grouptitle = titleRow["Group_Title"].ToString();
            type = titleRow["Material_Type"].ToString();
            aleph = titleRow["Aleph"].ToString();
            oclc = titleRow["OCLC"].ToString();

            mainLinkLabel.Text = bibid;

            groupTitleLabel.Text = titleRow["Group_Title"].ToString();

            grouptitleShortened = grouptitle;
            if (grouptitleShortened.Length > 150)
                grouptitleShortened = grouptitleShortened.Substring(0, 150) + "...";

            authorLabel.Text = titleRow["Author"].ToString().Trim();
            if (authorLabel.Text.Length == 0)
            {
                authorLabel.Text = "( none )";
                authorLabel.ForeColor = Color.Gray;
            }
            if (authorLabel.Text == "( varies )")
                authorLabel.ForeColor = Color.Gray;
            publisherLabel.Text = titleRow["Publisher"].ToString().Trim();
            if (publisherLabel.Text.Length == 0)
            {
                publisherLabel.Text = "( none )";
                publisherLabel.ForeColor = Color.Gray;
            }
            if (publisherLabel.Text == "( varies )")
                publisherLabel.ForeColor = Color.Gray;

            typeLabel.Text = type;
            oclcLabel.Text = oclc;
            if (oclcLabel.Text.Length == 0)
            {
                oclcLabel.Hide();
                oclcStaticLabel.Hide();
            }
            alephLabel.Text = aleph;
            if (alephLabel.Text.Length == 0)
            {
                alephLabel.Hide();
                alephStaticLabel.Hide();
            }

            show_all_volumes();

            // Set the default filename to the bibid
            saveFileDialog1.FileName = bibid;

            pageSetupDialog1.PageSettings.Landscape = SMaRT_UserSettings.Item_Grid_Print_Landscape;

            // Set the action on click 
            if (SMaRT_UserSettings.Item_Group_Form_Action_On_Click == View_Items_Form_Action_On_Click_Enum.Open_On_Web)
            {
                openItemFromWebMenuItem.Checked = true;
                viewItemFormMenuItem.Checked = false;
            }
            else
            {
                openItemFromWebMenuItem.Checked = false;
                viewItemFormMenuItem.Checked = true;
            }

            // Set the size correctly
            Size = SMaRT_UserSettings.View_Item_Group_Form_Size;
            int screen_width = Screen.PrimaryScreen.WorkingArea.Width;
            int screen_height = Screen.PrimaryScreen.WorkingArea.Height;
            if ((Width > screen_width) || (Height > screen_height) || ( SMaRT_UserSettings.View_Item_Group_Form_Maximized ))
                WindowState = FormWindowState.Maximized;

            username = Environment.UserName;
        }



        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        void gridPanel_Clipboard_Copy_Requested(DataRow[] rows)
        {
            // Copy the rows (and headers) into the clipboard
            StringBuilder builder = new StringBuilder(10000);

            // Copy the headers for all the visible columns
            foreach (CustomGrid_ColumnStyle thisStyle in gridPanel.Style.Column_Styles)
            {
                if (thisStyle.Visible)
                {
                    if (builder.Length > 0)
                        builder.Append("\t");
                    builder.Append("\"" + thisStyle.Header_Text + "\"");
                }
            }
            builder.Append("\n");

            // Now, step through each row
            bool rowStarted;
            foreach (DataRow thisRow in rows)
            {
                rowStarted = false;
                for (int i = 0; i < gridPanel.Style.Column_Styles.Count; i++)
                {
                    if (gridPanel.Style.Column_Styles[i].Visible)
                    {
                        if (!rowStarted)
                            rowStarted = true;
                        else
                        {
                            builder.Append("\t");
                        }

                        builder.Append("\"" + thisRow[i] + "\"");
                    }
                }
                builder.Append("\n");
            }

            // Now, set to the clipboard
            Clipboard.SetText(builder.ToString());
        }

        private void show_all_volumes()
        {
            SobekCM_Items_In_Title testSet1 = SobekCM_Database.Tracking_Multiple_Volumes(bibid, null);
            if (testSet1 == null) return;

            childTable = testSet1.Item_Table;

            // Create the table to display now
            displayTable = new DataTable("Items");
            DataColumn returnItemIdColumn = displayTable.Columns.Add("ItemID", Type.GetType("System.Int32"));
            DataColumn returnVidColumn = displayTable.Columns.Add("VID");
            DataColumn returnTitleColumn = displayTable.Columns.Add("Volume_Title");
            DataColumn returnAuthorColumn = displayTable.Columns.Add("Author");
            DataColumn returnPublisherColumn = displayTable.Columns.Add("Publisher");
            DataColumn returnDateColumn = displayTable.Columns.Add("PubDate");
            DataColumn returnLevel1TextColumn = displayTable.Columns.Add("Level1_Text");
            DataColumn returnLevel1IndexColumn = displayTable.Columns.Add("Level1_Index", Type.GetType("System.Int32"));
            DataColumn returnLevel2TextColumn = displayTable.Columns.Add("Level2_Text");
            DataColumn returnLevel2IndexColumn = displayTable.Columns.Add("Level2_Index", Type.GetType("System.Int32"));
            DataColumn returnLevel3TextColumn = displayTable.Columns.Add("Level3_Text");
            DataColumn returnLevel3IndexColumn = displayTable.Columns.Add("Level3_Index", Type.GetType("System.Int32"));
            DataColumn returnAggregationsColumn = displayTable.Columns.Add("Aggregations");
            DataColumn returnAccessColumn = displayTable.Columns.Add("Access");
            //   DataColumn returnInHandColumn = displayTable.Columns.Add("In_Hand", Type.GetType("System.Boolean"));
            DataColumn returnTrackingBoxColumn = displayTable.Columns.Add("Tracking_Box");
            DataColumn returnMilestoneBoxColumn = displayTable.Columns.Add("Last_Milestone");
            DataColumn returnArchivedColumn = displayTable.Columns.Add("Archived");
            //   DataColumn returnBornDigitalColumn = displayTable.Columns.Add("Born_Digital", Type.GetType("System.Boolean"));
            DataColumn returnDispositionColumn = displayTable.Columns.Add("Disposition_Date");
            DataColumn returnSortTitleColumn = displayTable.Columns.Add("SortTitle");
            DataColumn returnSortDateColumn = displayTable.Columns.Add("SortDate");



            // Add this to a set to facilitate saving to XML later
            DataSet displaySet = new DataSet("Items_In_Title");
            displaySet.Tables.Add(displayTable);

            // Get column references for the source table
            DataColumn itemIdColumn = childTable.Columns["ItemID"];
            DataColumn vidColumn = childTable.Columns["VID"];
            DataColumn titleColumn = childTable.Columns["Title"];
            DataColumn authorColumn = childTable.Columns["Author"];
            DataColumn publisherColumn = childTable.Columns["Publisher"];
            DataColumn level1TextColumn = childTable.Columns["Level1_Text"];
            DataColumn level1IndexColumn = childTable.Columns["Level1_Index"];
            DataColumn level2TextColumn = childTable.Columns["Level2_Text"];
            DataColumn level2IndexColumn = childTable.Columns["Level2_Index"];
            DataColumn level3TextColumn = childTable.Columns["Level3_Text"];
            DataColumn level3IndexColumn = childTable.Columns["Level3_Index"];
            DataColumn accessColumn = childTable.Columns["IP_Restriction_Mask"];
            DataColumn aggregationsColumn = childTable.Columns["AggregationCodes"];
            DataColumn trackingBoxColumn = childTable.Columns["Tracking_Box"];
            DataColumn dispositionColumn = childTable.Columns["Disposition_Date"];
            DataColumn dateColumn = childTable.Columns["PubDate"];
            DataColumn sortDateColumn = childTable.Columns["SortDate"];
            DataColumn sortTitleColumn = childTable.Columns["SortTitle"];
            DataColumn milestoneColumn = childTable.Columns["Last_Milestone"];
            DataColumn localArchiveColumn = childTable.Columns["Locally_Archived"];
            DataColumn remoteArchiveColumn = childTable.Columns["Remotely_Archived"];

            // Check for values at each hierarchical level
            bool hasLevel1Data = false;
            bool hasLevel2Data = false;
            bool hasLevel3Data = false;
            bool hasPublisherData = false;
            bool hasAuthorData = false;
            bool hasDateData = false;

            // Copy over the applicable rows
            foreach (DataRow thisRow in childTable.Rows)
            {
                DataRow newRow = displayTable.NewRow();
                newRow[returnItemIdColumn] = thisRow[itemIdColumn];
                newRow[returnVidColumn] = thisRow[vidColumn];
                newRow[returnTitleColumn] = thisRow[titleColumn];
                newRow[returnAuthorColumn] = thisRow[authorColumn];
                newRow[returnPublisherColumn] = thisRow[publisherColumn];
                newRow[returnDateColumn] = thisRow[dateColumn];
                newRow[returnLevel1TextColumn] = thisRow[level1TextColumn];
                newRow[returnLevel1IndexColumn] = thisRow[level1IndexColumn];
                newRow[returnLevel2TextColumn] = thisRow[level2TextColumn];
                newRow[returnLevel2IndexColumn] = thisRow[level2IndexColumn];
                newRow[returnLevel3TextColumn] = thisRow[level3TextColumn];
                newRow[returnLevel3IndexColumn] = thisRow[level3IndexColumn];
                newRow[returnAggregationsColumn] = thisRow[aggregationsColumn].ToString().Trim().Replace("  ", "").Replace(" ", ",");
                newRow[returnTrackingBoxColumn] = thisRow[trackingBoxColumn];
                newRow[returnDispositionColumn] = thisRow[dispositionColumn];
                newRow[returnSortDateColumn] = thisRow[sortDateColumn];
                newRow[returnSortTitleColumn] = thisRow[sortTitleColumn];

                int access = Convert.ToInt32(thisRow[accessColumn]);
                if (access < 0)
                    newRow[returnAccessColumn] = "private";
                if (access == 0)
                    newRow[returnAccessColumn] = "public";
                if (access > 0)
                    newRow[returnAccessColumn] = "restricted";
                displayTable.Rows.Add(newRow);

                // Compute the last milestone
                int last_milestone = Convert.ToInt32(thisRow[milestoneColumn]);
                switch( last_milestone )
                {
                    case 1:
                        newRow[returnMilestoneBoxColumn] = "Digital Acquisition";
                        break;

                    case 2:
                        newRow[returnMilestoneBoxColumn] = "Post-Acquisition Processing";
                        break;

                    case 3:
                        newRow[returnMilestoneBoxColumn] = "Quality Control";
                        break;

                    case 4:
                        newRow[returnMilestoneBoxColumn] = "Online Complete";
                        break;

                    default:
                        newRow[returnMilestoneBoxColumn] = String.Empty;
                        break;
                }

                // Determine the archiving
                bool locally_archived = false;
                bool remotely_archived = false;
                if (( thisRow[localArchiveColumn] != DBNull.Value ) && (  thisRow[localArchiveColumn] != null ))
                    locally_archived = true;
                if ((thisRow[remoteArchiveColumn] != DBNull.Value) && (thisRow[remoteArchiveColumn] != null))
                    remotely_archived = true;
                if (( !locally_archived ) && ( !remotely_archived ))
                    newRow[returnArchivedColumn] = String.Empty;
                if ((!locally_archived) && (remotely_archived))
                    newRow[returnArchivedColumn] = "Remote";
                if ((locally_archived) && (!remotely_archived))
                    newRow[returnArchivedColumn] = "Local";
                if ((locally_archived) && (remotely_archived))
                    newRow[returnArchivedColumn] = "Both";

                if (!hasAuthorData)
                {
                    if (newRow[returnAuthorColumn].ToString().Length > 0)
                        hasAuthorData = true;
                }
                if (!hasPublisherData)
                {
                    if (newRow[returnPublisherColumn].ToString().Length > 0)
                        hasPublisherData = true;
                }
                if (!hasLevel1Data)
                {
                    if (newRow[returnLevel1TextColumn].ToString().Length > 0)
                        hasLevel1Data = true;
                }
                if (!hasLevel2Data)
                {
                    if (newRow[returnLevel2TextColumn].ToString().Length > 0)
                        hasLevel2Data = true;
                }
                if (!hasLevel3Data)
                {
                    if (newRow[returnLevel3TextColumn].ToString().Length > 0)
                        hasLevel3Data = true;
                }
                if (!hasDateData)
                {
                    if (newRow[returnDateColumn].ToString().Length > 0)
                        hasDateData = true;
                }
            }

            if (gridPanel == null)
            {
                gridPanel = new CustomGrid_Panel
                                {
                                    Size = new Size(volumesPanel.Width - 2, volumesPanel.Height - 2),
                                    Location = new Point(0, 0)
                                };
                gridPanel.Clipboard_Copy_Requested += new CustomGrid_Panel_Delegate_Multiple(gridPanel_Clipboard_Copy_Requested);

            }
            else
            {
                SMaRT_UserSettings.View_Item_Group_Form_Title_Width = gridPanel.Style.Column_Styles[2].Width;
                if (gridPanel.Style.Column_Styles[3].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Author_Width = gridPanel.Style.Column_Styles[3].Width;
                if (gridPanel.Style.Column_Styles[4].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Publisher_Width = gridPanel.Style.Column_Styles[4].Width;
                if (gridPanel.Style.Column_Styles[5].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Date_Width = gridPanel.Style.Column_Styles[5].Width;
                if (gridPanel.Style.Column_Styles[6].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Level1_Width = gridPanel.Style.Column_Styles[6].Width;
                if (gridPanel.Style.Column_Styles[8].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Level2_Width = gridPanel.Style.Column_Styles[8].Width;
                if (gridPanel.Style.Column_Styles[10].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Level3_Width = gridPanel.Style.Column_Styles[10].Width;
            }

            // Configure some table level style settings
            gridPanel.Style.Default_Column_Width = 80;
            gridPanel.Style.Default_Column_Color = Color.LightBlue;
            gridPanel.Style.Header_Back_Color = Color.DarkBlue;
            gridPanel.Style.Header_Fore_Color = Color.White;

            //// initial the sort option
            //if (this.Current_Sort_String.Length == 0)
            //    this.Current_Sort_String = "BibID ASC";// Use the table from the database as the data source
            //this.DataTable = sourceTable;

            // Set the background and border style
            gridPanel.BackColor = Color.WhiteSmoke;
            gridPanel.BorderStyle = BorderStyle.FixedSingle;
            gridPanel.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            volumesPanel.Controls.Add(gridPanel);

            gridPanel.DataTable = displayTable;

            gridPanel.Current_Sort_String = "VID ASC";

            // Configure for this table
            gridPanel.Style.Column_Styles[0].Visible = false;
            gridPanel.Style.Column_Styles[1].Width = 50;
            gridPanel.Style.Column_Styles[1].Header_Text = "VID #";
            gridPanel.Style.Column_Styles[1].Fixed_Print_Width = 65;
            gridPanel.Style.Column_Styles[2].Width = SMaRT_UserSettings.View_Item_Group_Form_Title_Width;
            gridPanel.Style.Column_Styles[2].Text_Alignment = HorizontalAlignment.Left;
            gridPanel.Style.Column_Styles[2].BackColor = Color.White;

            if (hasAuthorData)
                gridPanel.Style.Column_Styles[3].Width = SMaRT_UserSettings.View_Item_Group_Form_Author_Width;
            else
                gridPanel.Style.Column_Styles[3].Visible = false;

            if (hasPublisherData)
                gridPanel.Style.Column_Styles[4].Width = SMaRT_UserSettings.View_Item_Group_Form_Publisher_Width;
            else
                gridPanel.Style.Column_Styles[4].Visible = false;

            if ( hasDateData)
                gridPanel.Style.Column_Styles[5].Width = SMaRT_UserSettings.View_Item_Group_Form_Date_Width;
            else
                gridPanel.Style.Column_Styles[5].Visible = false;

            if (hasLevel1Data)
                gridPanel.Style.Column_Styles[6].Width = SMaRT_UserSettings.View_Item_Group_Form_Level1_Width;
            else
                gridPanel.Style.Column_Styles[6].Visible = false;

            gridPanel.Style.Column_Styles[6].Text_Alignment = HorizontalAlignment.Left;
            gridPanel.Style.Column_Styles[7].Visible = false;
            if (hasLevel2Data)
                gridPanel.Style.Column_Styles[8].Width = SMaRT_UserSettings.View_Item_Group_Form_Level2_Width;
            else
                gridPanel.Style.Column_Styles[8].Visible = false;
            gridPanel.Style.Column_Styles[8].Text_Alignment = HorizontalAlignment.Left;
            gridPanel.Style.Column_Styles[9].Visible = false;
            if (hasLevel3Data)
                gridPanel.Style.Column_Styles[10].Width = SMaRT_UserSettings.View_Item_Group_Form_Level3_Width;
            else
                gridPanel.Style.Column_Styles[10].Visible = false;
            gridPanel.Style.Column_Styles[10].Text_Alignment = HorizontalAlignment.Left;
            gridPanel.Style.Column_Styles[11].Visible = false;
            gridPanel.Style.Column_Styles[12].Width = 60;
            gridPanel.Style.Column_Styles[12].Text_Alignment = HorizontalAlignment.Left;
            gridPanel.Style.Column_Styles[12].BackColor = Color.White;
            gridPanel.Style.Column_Styles[13].Width = 120;
            gridPanel.Style.Column_Styles[13].BackColor = Color.White;
            gridPanel.Style.Column_Styles[14].Width = 100;
            gridPanel.Style.Column_Styles[14].BackColor = Color.White;
            gridPanel.Style.Column_Styles[15].Width = 125;
            gridPanel.Style.Column_Styles[16].Width = 80;
            gridPanel.Style.Column_Styles[17].Visible = false;
            gridPanel.Style.Column_Styles[18].Visible = false;
            gridPanel.Style.Column_Styles[19].Visible = false;

            // Set some sort values
            gridPanel.Style.Column_Styles[2].Ascending_Sort = "SortTitle ASC, Level1_Index ASC, Level2_Index ASC, Level3_Index ASC, SortDate ASC";
            gridPanel.Style.Column_Styles[2].Descending_Sort = "SortTitle DESC, Level1_Index DESC, Level2_Index DESC, Level3_Index DESC, SortDate DESC";
            gridPanel.Style.Column_Styles[5].Ascending_Sort = "SortDate ASC, PubDate ASC";
            gridPanel.Style.Column_Styles[5].Descending_Sort = "SortDate DESC, PubDate DESC";
            gridPanel.Style.Column_Styles[6].Ascending_Sort = "Level1_Index ASC, Level2_Index ASC, Level3_Index ASC, SortDate ASC";
            gridPanel.Style.Column_Styles[6].Descending_Sort = "Level1_Index DESC, Level2_Index DESC, Level3_Index DESC, SortDate DESC";
            gridPanel.Style.Column_Styles[8].Ascending_Sort = "Level2_Index ASC, Level3_Index ASC";
            gridPanel.Style.Column_Styles[8].Descending_Sort = "Level2_Index DESC, Level3_Index DESC";

            // Add the event to the grid panel
            gridPanel.Double_Clicked += gridPanel_Double_Clicked;

            // Add the context menu
            gridPanel.Set_Context_Menus(null, contextMenu1);

            // Add the object to print
            gridPrinter = new CustomGrid_Printer(gridPanel, printDocument1);

            // Determine if any ALEPH, OCLC, AUTHOR, or PUBLISHER data is present
            bool checkingAuthor = true;
            bool checkingPublisher = true;
            foreach (DataRow thisRow in displayTable.Rows)
            {
                if (checkingAuthor)
                {
                    if (thisRow[returnAuthorColumn].ToString().Trim().Length > 0)
                    {
                        checkingAuthor = false;
                    }
                }

                if (checkingPublisher)
                {
                    if (thisRow[returnPublisherColumn].ToString().Trim().Length > 0)
                    {
                        checkingPublisher = false;
                    }
                }

                if ((!checkingAuthor) && (!checkingPublisher))
                    break;
            }

            // Set the visibility of the columns based on the data
            gridPanel.Style.Column_Styles[3].Visible = !checkingAuthor;
            gridPanel.Style.Column_Styles[4].Visible = !checkingPublisher;

            // Set focus
            gridPanel.Focus();
        }

        #endregion

        #region Method to draw the form background

        /// <summary> Method is called whenever this form is resized. </summary>
        /// <param name="e"></param>
        /// <remarks> This redraws the background of this form </remarks>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Get rid of any current background image
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
                BackgroundImage = null;
            }

            if (ClientSize.Width > 0)
            {
                // Create the items needed to draw the background
                Bitmap image = new Bitmap(ClientSize.Width, ClientSize.Height);
                Graphics gr = Graphics.FromImage(image);
                Rectangle rect = new Rectangle(new Point(0, 0), ClientSize);

                // Create the brush
                LinearGradientBrush brush = new LinearGradientBrush(rect, BackColor, ControlPaint.Dark(BackColor), LinearGradientMode.Vertical);
                brush.SetBlendTriangularShape(0.50F);

                // Create the image
                gr.FillRectangle(brush, rect);
                gr.Dispose();

                // Set this as the backgroundf
                BackgroundImage = image;
            }
        }

        #endregion

        #region Methods to print the tracking sheets or the item spreadsheet

        private void print_grid_style_report(bool All_Rows)
        {
            // Save the flag
            gridPrinterShowAllRows = All_Rows;

            try
            {
                // If there are too many rows, ask for confirmation
                if ((gridPanel.View_Count > 500) && (All_Rows))
                {
                    DialogResult result = MessageBox.Show("You are about to print a report with " + gridPanel.View_Count + " rows.\n\nAre you sure you want to do this?", "Large Print Job Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                if ((gridPanel.Selected_Row_Count > 500) && (!All_Rows))
                {
                    DialogResult result = MessageBox.Show("You are about to print a report with " + gridPanel.Selected_Row_Count + " rows.\n\nAre you sure you want to do this?", "Large Print Job Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                // Print this report
                gridPrinter.Prepare_To_Print();
                pageSetupDialog1.PageSettings.Landscape = SMaRT_UserSettings.Item_Grid_Print_Landscape;
                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                    SMaRT_UserSettings.Item_Grid_Print_Landscape = pageSetupDialog1.PageSettings.Landscape;
                    SMaRT_UserSettings.Save();
                    printDocument1.Print();
                }
            }
            catch
            {
                MessageBox.Show("Error encountered while printing!       ", "I/O Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;

                Font headerFont = new Font("Tahoma", 14, FontStyle.Bold);
                Brush headerBrush = new SolidBrush(Color.Black);
                g.DrawString(bibid + " : " + grouptitleShortened, headerFont, headerBrush, 120, 30);

                bool more = gridPrinterShowAllRows ? gridPrinter.DrawDataGrid(g) : gridPrinter.DrawDataGrid_SelectedRows(g);
                if (more)
                {
                    e.HasMorePages = true;
                    gridPrinter.Increment_Page();
                }
            }
            catch
            {
                MessageBox.Show("Error encountered while printing!       ", "I/O Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void print_tracking_report()
        {
            if (gridPanel == null)
                return;

            if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            {
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    PrintDocument printReport = new PrintDocument();
                    printReport.PrintPage += printReport_PrintPage;
                    bibIdPrinter = new BibIdReport_Printer(printReport, bibid, thisRow["VID"].ToString(), grouptitle, thisRow["Volume_Title"].ToString(), String.Empty, String.Empty, type, aleph, oclc, thisRow["Level1_Text"].ToString(), thisRow["Level2_Text"].ToString(), thisRow["Level3_Text"].ToString());
                    printReport.Print();
                }
            }
            else
                MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
        }

        private void printReport_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                bibIdPrinter.Print_Title_Report(g);
            }
            catch
            {
                MessageBox.Show("Error encountered while printing!       ", "I/O Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Form-related (Non-menu driven) event-handling methods

        protected override void OnClosing(CancelEventArgs e)
        {
            // Save the window state and/or size
            if (WindowState != FormWindowState.Maximized)
            {
                SMaRT_UserSettings.View_Item_Group_Form_Size = Size;
                SMaRT_UserSettings.View_Item_Group_Form_Maximized = false;
            }
            else
                SMaRT_UserSettings.View_Item_Group_Form_Maximized = true;

            // Save the column widths
            if (gridPanel != null)
            {
                SMaRT_UserSettings.View_Item_Group_Form_Title_Width = gridPanel.Style.Column_Styles[2].Width;
                if (gridPanel.Style.Column_Styles[3].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Author_Width = gridPanel.Style.Column_Styles[3].Width;
                if (gridPanel.Style.Column_Styles[4].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Publisher_Width = gridPanel.Style.Column_Styles[4].Width;
                if (gridPanel.Style.Column_Styles[5].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Date_Width = gridPanel.Style.Column_Styles[5].Width;
                if (gridPanel.Style.Column_Styles[6].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Level1_Width = gridPanel.Style.Column_Styles[6].Width;
                if (gridPanel.Style.Column_Styles[8].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Level2_Width = gridPanel.Style.Column_Styles[8].Width;
                if (gridPanel.Style.Column_Styles[10].Visible)
                    SMaRT_UserSettings.View_Item_Group_Form_Level3_Width = gridPanel.Style.Column_Styles[10].Width;
            }

            SMaRT_UserSettings.Save();
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void volumeListButton_Button_Pressed(object sender, EventArgs e)
        {
            if (gridPanel == null)
                return;

            if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            {
                volumeListButton.Button_Enabled = false;
                print_grid_style_report(false);
                volumeListButton.Button_Enabled = true;
            }
            else
                MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void volumeReportButton_Button_Pressed(object sender, EventArgs e)
        {
            if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            {
                print_tracking_report();
            }
            else
                MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mainLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = SobekCM_Library_Settings.System_Base_URL + bibid;

            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening item group from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!collapsed)
            {
                collapsed = true;
                pictureBox1.Image = imageList1.Images[1];
                groupInfoPanel.Height = groupInfoPanel.Height - 113;
                groupTitleLabel.Hide();
                groupTitleStaticLabel.Hide();
                volumesPanel.Location = new Point(volumesPanel.Location.X, volumesPanel.Location.Y - 113);
                volumesPanel.Height = volumesPanel.Height + 113;
            }
            else
            {
                collapsed = false;
                pictureBox1.Image = imageList1.Images[0];
                groupInfoPanel.Height = groupInfoPanel.Height + 113;
                groupTitleLabel.Show();
                groupTitleStaticLabel.Show();
                volumesPanel.Location = new Point(volumesPanel.Location.X, volumesPanel.Location.Y + 113);
                volumesPanel.Height = volumesPanel.Height - 113;
            }
        }

        private void copyPictureBox_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, bibid);
        }

        #endregion

        #region Event handlers related to the picture buttons at the top of the form

        private void pictureBox_MouseEnter(object sender, EventArgs e)
        {
            behaviorsPictureBox.BorderStyle = BorderStyle.None;
            addVolumePictureBox.BorderStyle = BorderStyle.None;
            autoFillPictureBox.BorderStyle = BorderStyle.None;
            serialHierarchyPictureBox.BorderStyle = BorderStyle.None;
            massUpdatePictureBox.BorderStyle = BorderStyle.None;

            PictureBox senderBox = (PictureBox)sender;
            senderBox.BorderStyle = BorderStyle.FixedSingle;

        }

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            behaviorsPictureBox.BorderStyle = BorderStyle.None;
            addVolumePictureBox.BorderStyle = BorderStyle.None;
            autoFillPictureBox.BorderStyle = BorderStyle.None;
            serialHierarchyPictureBox.BorderStyle = BorderStyle.None;
            massUpdatePictureBox.BorderStyle = BorderStyle.None;

            PictureBox senderBox = (PictureBox)sender;
            senderBox.BorderStyle = BorderStyle.None;
        }

        private void mainLabel_MouseEnter(object sender, EventArgs e)
        {
            behaviorsPictureBox.BorderStyle = BorderStyle.None;
            addVolumePictureBox.BorderStyle = BorderStyle.None;
            autoFillPictureBox.BorderStyle = BorderStyle.None;
            serialHierarchyPictureBox.BorderStyle = BorderStyle.None;
            massUpdatePictureBox.BorderStyle = BorderStyle.None;
        }

        private void mainPanel_MouseEnter(object sender, EventArgs e)
        {
            behaviorsPictureBox.BorderStyle = BorderStyle.None;
            addVolumePictureBox.BorderStyle = BorderStyle.None;
            autoFillPictureBox.BorderStyle = BorderStyle.None;
            serialHierarchyPictureBox.BorderStyle = BorderStyle.None;
            massUpdatePictureBox.BorderStyle = BorderStyle.None;
        }

        private void behaviorsPictureBox_Click(object sender, EventArgs e)
        {
            string url = SobekCM_Library_Settings.System_Base_URL + "my/groupbehaviors/" + bibid + "/1";

            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening item group from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addVolumePictureBox_Click(object sender, EventArgs e)
        {
            string url = SobekCM_Library_Settings.System_Base_URL + "my/addvolume/" + bibid + "/1";

            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening item group from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void autoFillPictureBox_Click(object sender, EventArgs e)
        {
            Volume_Import_Form showForm = new Volume_Import_Form( bibid, grouptitle, childTable );
            Hide();
            showForm.ShowDialog();
            show_all_volumes();
            Show();
        }

        private void serialHierarchyPictureBox_Click(object sender, EventArgs e)
        {
            SobekCM_Items_In_Title multiple = SobekCM_Database.Get_Multiple_Volumes(bibid, null);
            if (multiple == null)
            {
                MessageBox.Show("Database error occurred");
            }
            else
            {
                Hide();
                Edit_Serial_Hierarchy_Form edit = new Edit_Serial_Hierarchy_Form(bibid, multiple);
                edit.ShowDialog();
                Close();
            }
        }

        private void massUpdatePictureBox_Click(object sender, EventArgs e)
        {
            string url = SobekCM_Library_Settings.System_Base_URL + "my/massupdate/" + bibid + "/1";

            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening item group from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Main Menu Item Event Handlers

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            // Show the about form
            About showAbout = new About("SobekCM Management and Reporting Tool", VersionConfigSettings.AppVersion);
            showAbout.ShowDialog();	
        }

        private void printTrackingSheetsMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            {
                print_tracking_report();
            }
            else
                MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void printSelectedRowsMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            {
                print_grid_style_report(false);
            }
            else
                MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void printAllRowsMenuItem_Click(object sender, EventArgs e)
        {
            print_grid_style_report(true);
        }

        private void pageSetupMenuItem_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.ShowDialog();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void setTrackingBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Enter_Tracking_Box_Form trackingBox = new Enter_Tracking_Box_Form();
            if (trackingBox.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                string new_tracking_box = trackingBox.New_Tracking_Box;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        updated++;
                        if (Resource_Object.Database.SobekCM_Database.Save_New_Tracking_Box(itemid, new_tracking_box))
                            thisRow["Tracking_Box"] = new_tracking_box;
                    }
                }
                Cursor = Cursors.Default;
                gridPanel.Refresh();
                MessageBox.Show(updated + " records updated.");
            }
        }


        private void updateItemDispositionMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Update_Disposition_Form trackingBox = new Update_Disposition_Form();
            if (trackingBox.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                int typeid = trackingBox.Disposition_Type_ID;
                DateTime date = trackingBox.Disposition_Date;
                string notes = trackingBox.Disposition_Notes;
                if (notes.Trim().Length == 0)
                {
                    notes = SobekCM_Library_Settings.Disposition_Term_Past(typeid);
                }
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        Resource_Object.Database.SobekCM_Database.Update_Disposition(itemid, typeid, notes, date, username);
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.\n\n" + skipped + " multi-volume records skipped");
            }
        }

        private void editDispositionAdviceMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Edit_Disposition_Advice_Form trackingBox = new Edit_Disposition_Advice_Form();
            if (trackingBox.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int typeid = trackingBox.Disposition_Type_ID;
                string notes = trackingBox.Disposition_Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        // Verify this wasn't already disposed off, in which case no point in allowing this edit
                        if (thisRow["Disposition_Date"] == DBNull.Value)
                        {
                            Resource_Object.Database.SobekCM_Database.Edit_Disposition_Advice(itemid, typeid, notes);
                            updated++;
                        }
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.");
            }
        }

        private void addWorklogHistoryEntryMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Add_Workflow_Form workflowForm = new Add_Workflow_Form();
            if (workflowForm.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                string workflowType = workflowForm.Workflow_Type;
                DateTime date = workflowForm.Workflow_Date;
                string notes = workflowForm.Workflow_Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        Resource_Object.Database.SobekCM_Database.Add_Past_Workflow(itemid, workflowType, notes, date, username, String.Empty);
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.\n\n" + skipped + " multi-volume records skipped");
            }
        }

        private void updateMaterialReceivedDateMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Update_Material_Received_Form workflowForm = new Update_Material_Received_Form();
            if (workflowForm.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                DateTime date = workflowForm.Date_Received;
                bool estimated = workflowForm.Estimated;
                string notes = workflowForm.Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        Resource_Object.Database.SobekCM_Database.Update_Material_Received(itemid, date, estimated, username, notes);
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.\n\n" + skipped + " multi-volume records skipped");
            }
        }

        private void updateBornDigitalFlagMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Update_Born_Digital_Form workflowForm = new Update_Born_Digital_Form();
            if (workflowForm.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                bool newflag = workflowForm.Born_Digital_Flag;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        Resource_Object.Database.SobekCM_Database.Update_Born_Digital_Flag(itemid, newflag);
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.\n\n" + skipped + " multi-volume records skipped");
            }
        }

        #region 'Save as' event handlers

        private void excelSpreadsheetMenuItem_Click(object sender, EventArgs e)
        {
            if (gridPanel == null)
                return;

            saveFileDialog1.Filter = "Excel Files|*.xls;*.xlsx";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (gridPanel.Export_as_Excel(saveFileDialog1.FileName, SobekCM_Library_Settings.System_Abbreviation + " Volumes within Title '" + bibid + "'", bibid))
                {
                    MessageBox.Show("Data exported as excel.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tabDelimitedTextMenuItem_Click(object sender, EventArgs e)
        {
            if (gridPanel == null)
                return;

            saveFileDialog1.Filter = "Text Files|*.txt";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (gridPanel.Export_as_Text(saveFileDialog1.FileName, '\t'))
                {
                    MessageBox.Show("Data exported as text.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void commaSeperatedValuesMenuItem_Click(object sender, EventArgs e)
        {
            if (gridPanel == null)
                return;

            saveFileDialog1.Filter = "CSV Files|*.csv";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (gridPanel.Export_as_Text(saveFileDialog1.FileName, ','))
                {
                    MessageBox.Show("Data exported as text.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void xMLDataSetMenuItem_Click(object sender, EventArgs e)
        {
            if (displayTable == null)
                return;

            saveFileDialog1.Filter = "XML DataSet|*.xml";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                displayTable.DataSet.WriteXml(saveFileDialog1.FileName, XmlWriteMode.WriteSchema);
                MessageBox.Show("Data exported as XML.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Preferences for when an item is clicked event handlers

        private void openItemFromWebInBrowserWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openItemFromWebMenuItem.Checked = true;
            viewItemFormMenuItem.Checked = false;

            SMaRT_UserSettings.Item_Group_Form_Action_On_Click = View_Items_Form_Action_On_Click_Enum.Open_On_Web;
            SMaRT_UserSettings.Save();
        }

        private void viewItemFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openItemFromWebMenuItem.Checked = false;
            viewItemFormMenuItem.Checked = true;

            SMaRT_UserSettings.Item_Group_Form_Action_On_Click = View_Items_Form_Action_On_Click_Enum.Show_Form;
            SMaRT_UserSettings.Save();
        }

        #endregion

        #endregion

        #region Context menu event handlers

        private void openWebContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            // Get first row then
            DataRow thisRow = gridPanel.Selected_Row[0];

            // Get the information from this row
            string vid = thisRow["VID"].ToString();

            string url = SobekCM_Library_Settings.System_Base_URL + bibid + "/" + vid;

            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening item from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void detailsContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            // Get first row then
            DataRow thisRow = gridPanel.Selected_Row[0];

            // Get the information from this row
            int itemid = Convert.ToInt32(thisRow["ItemID"]);
            string vid = thisRow["VID"].ToString();

            View_Item_Form showItemForm = new View_Item_Form(itemid, bibid, vid);
            Hide();
            showItemForm.ShowDialog();
            Show();
        }

        private void printBibIdReportContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            {
                print_tracking_report();
            }
            else
                MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void printAllRowsContextMenuItem_Click(object sender, EventArgs e)
        {
            print_grid_style_report(true);
        }

        private void printedSelectedRowsContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            {
                print_grid_style_report(false);
            }
            else
                MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void setTrackingBoxContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Enter_Tracking_Box_Form trackingBox = new Enter_Tracking_Box_Form();
            if (trackingBox.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                string new_tracking_box = trackingBox.New_Tracking_Box;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        updated++;
                        if (Resource_Object.Database.SobekCM_Database.Save_New_Tracking_Box(itemid, new_tracking_box))
                            thisRow["Tracking_Box"] = new_tracking_box;
                    }
                }
                Cursor = Cursors.Default;
                gridPanel.Refresh();
                MessageBox.Show(updated + " records updated.");
            }
        }

        private void editDispositionAdviceContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Edit_Disposition_Advice_Form trackingBox = new Edit_Disposition_Advice_Form();
            if (trackingBox.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int typeid = trackingBox.Disposition_Type_ID;
                string notes = trackingBox.Disposition_Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if (thisRow["Disposition_Date"] == DBNull.Value)
                        {
                            Resource_Object.Database.SobekCM_Database.Edit_Disposition_Advice(itemid, typeid, notes);
                            updated++;
                        }
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.");
            }
        }

        private void addWorklogHistoryEntryContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Add_Workflow_Form workflowForm = new Add_Workflow_Form();
            if (workflowForm.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                string workflowType = workflowForm.Workflow_Type;
                DateTime date = workflowForm.Workflow_Date;
                string notes = workflowForm.Workflow_Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        Resource_Object.Database.SobekCM_Database.Add_Past_Workflow(itemid, workflowType, notes, date, username, String.Empty);
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.\n\n" + skipped + " multi-volume records skipped");
            }
        }

        private void updateDispositionContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Update_Disposition_Form trackingBox = new Update_Disposition_Form();
            if (trackingBox.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                int typeid = trackingBox.Disposition_Type_ID;
                DateTime date = trackingBox.Disposition_Date;
                string notes = trackingBox.Disposition_Notes;
                if (notes.Trim().Length == 0)
                {
                    notes = SobekCM_Library_Settings.Disposition_Term_Past(typeid);
                }
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        Resource_Object.Database.SobekCM_Database.Update_Disposition(itemid, typeid, notes, date, username);
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.\n\n" + skipped + " multi-volume records skipped");
            }
        }

        private void updateBornDigitalContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Update_Born_Digital_Form workflowForm = new Update_Born_Digital_Form();
            if (workflowForm.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                bool newflag = workflowForm.Born_Digital_Flag;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        Resource_Object.Database.SobekCM_Database.Update_Born_Digital_Flag(itemid, newflag);
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.\n\n" + skipped + " multi-volume records skipped");
            }
        }

        private void updateMaterialReceivedContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Update_Material_Received_Form workflowForm = new Update_Material_Received_Form();
            if (workflowForm.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                DateTime date = workflowForm.Date_Received;
                bool estimated = workflowForm.Estimated;
                string notes = workflowForm.Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        Resource_Object.Database.SobekCM_Database.Update_Material_Received(itemid, date, estimated, username, notes);
                        updated++;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.\n\n" + skipped + " multi-volume records skipped");
            }
        }

        #endregion

        #region Custom Grid Event Handler

        void gridPanel_Double_Clicked(DataRow thisRow)
        {
            if (thisRow != null)
            {
                int itemid = Convert.ToInt32(thisRow["ItemID"]);
                string vid = thisRow["VID"].ToString();

                // What is the currently selected action?
                if (openItemFromWebMenuItem.Checked)
                {
                    string url = SobekCM_Library_Settings.System_Base_URL + bibid + "/" + vid;

                    try
                    {
                        Process openOnWeb = new Process {StartInfo = {FileName = url}};
                        openOnWeb.Start();
                    }
                    catch
                    {
                        MessageBox.Show("Error opening item from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    View_Item_Form showItemForm = new View_Item_Form(itemid, bibid, vid);
                    Hide();
                    showItemForm.ShowDialog();
                    Show();
                }
            }
        }

        #endregion

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            show_all_volumes();
        }

        private void viewAdHocReportMenuItem_Click(object sender, EventArgs e)
        {
            SobekCM_Search_Object search =
                new SobekCM_Search_Object(SMaRT_UserSettings.Discovery_Panel_Search_Term1,
                                          SMaRT_UserSettings.Discovery_Panel_Search_Term2,
                                          SMaRT_UserSettings.Discovery_Panel_Search_Term3,
                                          SMaRT_UserSettings.Discovery_Panel_Search_Term4);
            bool found_bib = false;
            if (search.First_Term == SobekCM_Search_Object.SobekCM_Term_Enum.BibID)
            {
                search.First_Value = bibid;
                found_bib = true;
            }
            if ((!found_bib) && (search.Second_Term == SobekCM_Search_Object.SobekCM_Term_Enum.BibID))
            {
                search.Second_Value = bibid;
                found_bib = true;
            }
            if ((!found_bib) && (search.Third_Term == SobekCM_Search_Object.SobekCM_Term_Enum.BibID))
            {
                search.Third_Value = bibid;
                found_bib = true;
            }
            if ((!found_bib) && (search.Fourth_Term == SobekCM_Search_Object.SobekCM_Term_Enum.BibID))
            {
                search.Fourth_Value = bibid;
                found_bib = true;
            }
            if (!found_bib)
            {
                search.Second_Value = bibid;
                search.Second_Term = SobekCM_Search_Object.SobekCM_Term_Enum.BibID;
            }

            // Show the ad hoc reporting form
            Ad_Hoc_Reporting_Query_Form showForm = new Ad_Hoc_Reporting_Query_Form(search, null);
            Hide();
            showForm.ShowDialog();
            Show();
        }



    }
}
