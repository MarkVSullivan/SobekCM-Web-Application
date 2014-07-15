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
using SobekCM.Resource_Object.Database;
using SobekCM.Library;
using SobekCM.Management_Tool.Settings;
using SobekCM.Management_Tool.Versioning;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form displays the user-requested ad hoc report </summary>
    public partial class Ad_Hoc_Report_Display_Form : Form
    {
        private BibIdReport_Printer bibIdPrinter;
        private readonly DataSet displaySet;
        private readonly CustomGrid_Panel gridPanel;
        private readonly CustomGrid_Printer gridPrinter;
        private bool gridPrinterShowAllRows = true;
        private readonly string username;

        #region Constructor

        /// <summary> Constructor for a new instance of the Ad_Hoc_Report_Display_Form form </summary>
        /// <param name="Display_Set">Set of titles/items to display within this form</param>
        public Ad_Hoc_Report_Display_Form( DataSet Display_Set )
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            displaySet = Display_Set;

            // Check for values at each hierarchical level and author and publisher
            bool hasLevel1Data = false;
            bool hasLevel2Data = false;
            bool hasLevel3Data = false;
            bool hasAuthorData = false;
            bool hasPublisherData = false;
            bool hasDateData = false;

            // Move the date column to the right spot
            Display_Set.Tables[0].Columns["PubDate"].SetOrdinal(8);


            // Get the columns
            DataColumn level1Column = displaySet.Tables[0].Columns["Level1_Text"];
            DataColumn level2Column = displaySet.Tables[0].Columns["Level2_Text"];
            DataColumn level3Column = displaySet.Tables[0].Columns["Level3_Text"];
            DataColumn publisherColumn = displaySet.Tables[0].Columns["Publisher"];
            DataColumn authorColumn = displaySet.Tables[0].Columns["Author"];
            DataColumn dateColumn = displaySet.Tables[0].Columns["PubDate"];

            // Step through each row and check for data existence
            foreach (DataRow thisRow in Display_Set.Tables[0].Rows )
            {
                if ((hasLevel1Data) && (hasLevel2Data) && (hasLevel3Data) && (hasPublisherData) && (hasAuthorData) && ( hasDateData ))
                    break;
                if ((!hasLevel1Data) && (thisRow[level1Column].ToString().Length > 0))
                    hasLevel1Data = true;
                if ((!hasLevel2Data) && (thisRow[level2Column].ToString().Length > 0))
                    hasLevel2Data = true;
                if ((!hasLevel3Data) && (thisRow[level3Column].ToString().Length > 0))
                    hasLevel3Data = true;
                if ((!hasPublisherData) && (thisRow[publisherColumn].ToString().Length > 0))
                    hasPublisherData = true;
                if ((!hasAuthorData) && (thisRow[authorColumn].ToString().Length > 0))
                    hasAuthorData = true;
                if ((!hasDateData) && (thisRow[dateColumn].ToString().Length > 0))
                    hasDateData = true;
            }

            // Create the custom grid
            gridPanel = new CustomGrid_Panel
                            {Size = new Size(panel1.Width - 2, panel1.Height - 2), Location = new Point(0, 0)};

            // Configure some table level style settings
            gridPanel.Style.Default_Column_Width = 80;
            gridPanel.Style.Default_Column_Color = Color.LightBlue;
            gridPanel.Style.Header_Back_Color = Color.DarkBlue;
            gridPanel.Style.Header_Fore_Color = Color.White;

            // Set the background and border style
            gridPanel.BackColor = Color.WhiteSmoke;
            gridPanel.BorderStyle = BorderStyle.FixedSingle;
            gridPanel.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            panel1.Controls.Add(gridPanel);

            gridPanel.DataTable = displaySet.Tables[0];

            gridPanel.Current_Sort_String = "BibID ASC, VID ASC";

            // Configure for this table
            gridPanel.Style.Column_Styles[1].Width = 50;
            gridPanel.Style.Column_Styles[2].Visible = false;
            gridPanel.Style.Column_Styles[3].Visible = false;
            gridPanel.Style.Column_Styles[4].Visible = false;
            gridPanel.Style.Column_Styles[5].Width = SMaRT_UserSettings.Ad_Hoc_Report_Form_Title_Width;
            gridPanel.Style.Column_Styles[5].BackColor = Color.White;
            gridPanel.Style.Column_Styles[6].BackColor = Color.White;

            if (!hasAuthorData)
                gridPanel.Style.Column_Styles[6].Width = SMaRT_UserSettings.Ad_Hoc_Report_Form_Author_Width;
            else
                gridPanel.Style.Column_Styles[6].Visible = false;

            gridPanel.Style.Column_Styles[7].BackColor = Color.White;
            if (hasPublisherData)
                gridPanel.Style.Column_Styles[7].Width = SMaRT_UserSettings.Ad_Hoc_Report_Form_Publisher_Width;
            else
                gridPanel.Style.Column_Styles[7].Visible = false;

            gridPanel.Style.Column_Styles[8].BackColor = Color.White;
            if (hasDateData)
                gridPanel.Style.Column_Styles[8].Width = SMaRT_UserSettings.Ad_Hoc_Report_Form_Date_Width;
            else
                gridPanel.Style.Column_Styles[8].Visible = false;

            if (hasLevel1Data)
                gridPanel.Style.Column_Styles[9].Width =  SMaRT_UserSettings.Ad_Hoc_Report_Form_Level1_Width;
            else
                gridPanel.Style.Column_Styles[9].Visible = false;

            gridPanel.Style.Column_Styles[9].Text_Alignment = HorizontalAlignment.Left;
            gridPanel.Style.Column_Styles[10].Visible = false;
            if (hasLevel2Data)
                gridPanel.Style.Column_Styles[11].Width = SMaRT_UserSettings.Ad_Hoc_Report_Form_Level2_Width;
            else
                gridPanel.Style.Column_Styles[11].Visible = false;
            gridPanel.Style.Column_Styles[11].Text_Alignment = HorizontalAlignment.Left;
            gridPanel.Style.Column_Styles[12].Visible = false;
            if (hasLevel3Data)
                gridPanel.Style.Column_Styles[13].Width = SMaRT_UserSettings.Ad_Hoc_Report_Form_Level3_Width;
            else
                gridPanel.Style.Column_Styles[13].Visible = false;
            gridPanel.Style.Column_Styles[13].Text_Alignment = HorizontalAlignment.Left;
            gridPanel.Style.Column_Styles[14].Visible = false;
            gridPanel.Style.Column_Styles[15].Visible = false;
            gridPanel.Style.Column_Styles[16].Visible = false;
            gridPanel.Style.Column_Styles[17].Visible = false;
            gridPanel.Style.Column_Styles[18].Visible = false;

            gridPanel.Style.Column_Styles[20].Visible = false;
            gridPanel.Style.Column_Styles[21].Header_Text = "Comments";
            gridPanel.Style.Column_Styles[24].Header_Text = "Digitized";
            gridPanel.Style.Column_Styles[24].Short_Date_Format = true;
            gridPanel.Style.Column_Styles[25].Header_Text = "Processed";
            gridPanel.Style.Column_Styles[25].Short_Date_Format = true;
            gridPanel.Style.Column_Styles[26].Header_Text = "QC'd";
            gridPanel.Style.Column_Styles[26].Short_Date_Format = true;
            gridPanel.Style.Column_Styles[27].Header_Text = "Online";
            gridPanel.Style.Column_Styles[27].Short_Date_Format = true;
            gridPanel.Style.Column_Styles[38].Visible = false;

            // Set some sort values
            gridPanel.Style.Column_Styles[9].Ascending_Sort = "Level1_Index ASC, Level2_Index ASC, Level3_Index ASC";
            gridPanel.Style.Column_Styles[9].Descending_Sort = "Level1_Index DESC, Level2_Index DESC, Level3_Index DESC";
            gridPanel.Style.Column_Styles[11].Ascending_Sort = "Level2_Index ASC, Level3_Index ASC";
            gridPanel.Style.Column_Styles[11].Descending_Sort = "Level2_Index DESC, Level3_Index DESC";
            gridPanel.Style.Column_Styles[5].Ascending_Sort = "SortTitle ASC, Level1_Index ASC, Level2_Index ASC, Level3_Index ASC";
            gridPanel.Style.Column_Styles[5].Descending_Sort = "SortTitle DESC, Level1_Index DESC, Level2_Index DESC, Level3_Index DESC";
            gridPanel.Style.Column_Styles[8].Ascending_Sort = "SortDate ASC, SortTitle ASC, Level1_Index ASC, Level2_Index ASC, Level3_Index ASC";
            gridPanel.Style.Column_Styles[8].Descending_Sort = "SortDate DESC, SortTitle DESC, Level1_Index DESC, Level2_Index DESC, Level3_Index DESC";


            gridPanel.Double_Clicked += gridPanel_Double_Clicked;
            gridPanel.Clipboard_Copy_Requested += new CustomGrid_Panel_Delegate_Multiple(gridPanel_Clipboard_Copy_Requested);

            // Add the context menu
            gridPanel.Set_Context_Menus(null, contextMenu1);

            // Add the object to print
            gridPrinter = new CustomGrid_Printer(gridPanel, printDocument1);

            // Set the hits value
            hitCountLabel.Text = "Your search resulted in " + number_to_string(displaySet.Tables[0].Rows.Count) + " items in " + number_to_string(displaySet.Tables[1].Rows.Count) + " titles";


            // Set the size correctly
            Size = SMaRT_UserSettings.Ad_Hoc_Report_Form_Size;
            int screen_width = Screen.PrimaryScreen.WorkingArea.Width;
            int screen_height = Screen.PrimaryScreen.WorkingArea.Height;
            if ((Width > screen_width) || (Height > screen_height) || (SMaRT_UserSettings.Ad_Hoc_Report_Form_Maximized))
                WindowState = FormWindowState.Maximized;

            // GEt the username
            username = Environment.UserName;

            // Set the action on click 
            if (SMaRT_UserSettings.Ad_Hoc_Form_Action_On_Click == View_Items_Form_Action_On_Click_Enum.Open_On_Web)
            {
                openItemFromWebMenuItem.Checked = true;
                viewItemFormMenuItem.Checked = false;
            }
            else
            {
                openItemFromWebMenuItem.Checked = false;
                viewItemFormMenuItem.Checked = true;
            }
        }

        void gridPanel_Clipboard_Copy_Requested(DataRow[] rows)
        {
            // Copy the rows (and headers) into the clipboard
            StringBuilder builder = new StringBuilder(10000);

            // Copy the headers for all the visible columns
            foreach (CustomGrid_ColumnStyle thisStyle in gridPanel.Style.Column_Styles)
            {
                if ( thisStyle.Visible )
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
                for( int i = 0 ; i < gridPanel.Style.Column_Styles.Count ; i++ )
                {
                    if  ( gridPanel.Style.Column_Styles[i].Visible)
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

 
        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        private string number_to_string(int Number)
        {
            switch (Number)
            {
                case 1: return "one";
                case 2: return "two";
                case 3: return "three";
                case 4: return "four";
                case 5: return "five";
                case 6: return "six";
                case 7: return "seven";
                case 8: return "eight";
                case 9: return "nine";
                case 10: return "ten";
                case 11: return "eleven";
                case 12: return "twelve";
                default: return Number.ToString();

            }
        }

        void gridPanel_Double_Clicked(DataRow thisRow)
        {
            if (thisRow == null)
                return;

            string bibid = thisRow["BibID"].ToString();
            string vid = thisRow["VID"].ToString();

            // What is the currently selected action?
            if (openItemFromWebMenuItem.Checked)
            {
                string url = SobekCM_Library_Settings.System_Base_URL + bibid;
                if (vid.IndexOf("(") < 0)
                {
                    url = url + "/" + vid;
                }

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
            else
            {
                int itemid = Convert.ToInt32(thisRow["ItemID"]);
                View_Item_Form showItemForm = new View_Item_Form(itemid, bibid, vid);
                Hide();
                showItemForm.ShowDialog();
                Show();
            }
        }

        #endregion

        private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

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
                brush.SetBlendTriangularShape(0.33F);

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
                g.DrawString("Ad Hoc " + SobekCM_Library_Settings.System_Abbreviation + " Report", headerFont, headerBrush, 120, 30);

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
                    bibIdPrinter = new BibIdReport_Printer(printReport, thisRow["BibID"].ToString(), thisRow["VID"].ToString(), thisRow["GroupTitle"].ToString(), thisRow["Volume_Title"].ToString(), String.Empty, String.Empty, thisRow["Type"].ToString(), thisRow["Aleph"].ToString(), thisRow["OCLC"].ToString(), thisRow["Level1_Text"].ToString(), thisRow["Level2_Text"].ToString(), thisRow["Level3_Text"].ToString());
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
                SMaRT_UserSettings.Ad_Hoc_Report_Form_Size = Size;
                SMaRT_UserSettings.Ad_Hoc_Report_Form_Maximized = false;
            }
            else
                SMaRT_UserSettings.Ad_Hoc_Report_Form_Maximized = true;

            // Save the column widths
            if (gridPanel != null)
            {
                SMaRT_UserSettings.Ad_Hoc_Report_Form_Title_Width = gridPanel.Style.Column_Styles[5].Width;
                if (gridPanel.Style.Column_Styles[6].Visible)
                    SMaRT_UserSettings.Ad_Hoc_Report_Form_Author_Width = gridPanel.Style.Column_Styles[6].Width;
                if (gridPanel.Style.Column_Styles[7].Visible)
                    SMaRT_UserSettings.Ad_Hoc_Report_Form_Publisher_Width = gridPanel.Style.Column_Styles[7].Width;
                if (gridPanel.Style.Column_Styles[8].Visible)
                    SMaRT_UserSettings.Ad_Hoc_Report_Form_Date_Width = gridPanel.Style.Column_Styles[8].Width;
                if (gridPanel.Style.Column_Styles[9].Visible)
                    SMaRT_UserSettings.Ad_Hoc_Report_Form_Level1_Width = gridPanel.Style.Column_Styles[9].Width;
                if (gridPanel.Style.Column_Styles[12].Visible)
                    SMaRT_UserSettings.Ad_Hoc_Report_Form_Level2_Width = gridPanel.Style.Column_Styles[12].Width;
                if (gridPanel.Style.Column_Styles[14].Visible)
                    SMaRT_UserSettings.Ad_Hoc_Report_Form_Level3_Width = gridPanel.Style.Column_Styles[14].Width;
            }

            SMaRT_UserSettings.Save();
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

            // Get the new tracking box
            Enter_Tracking_Box_Form trackingBox = new Enter_Tracking_Box_Form();
            if (trackingBox.ShowDialog() != DialogResult.OK) return;

            // Do the work
            Cursor = Cursors.WaitCursor;
            int updated = 0;
            string new_tracking_box = trackingBox.New_Tracking_Box;
            foreach (DataRow thisRow in gridPanel.Selected_Row)
            {
                int itemid = Convert.ToInt32(thisRow["ItemID"]);
                if (itemid > 0)
                {
                    updated++;
                    if (SobekCM_Database.Save_New_Tracking_Box(itemid, new_tracking_box))
                        thisRow["Tracking_Box"] = new_tracking_box;
                }
            }
            Cursor = Cursors.Default;
            gridPanel.Refresh();
            MessageBox.Show(updated + " records updated.");
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
                int typeid = trackingBox.Disposition_Type_ID;
                DateTime date = trackingBox.Disposition_Date;
                string notes = trackingBox.Disposition_Notes;
                string typeString = SobekCM_Library_Settings.Disposition_Term_Past(typeid);
                if (notes.Trim().Length == 0)
                {
                    notes = SobekCM_Library_Settings.Disposition_Term_Past(typeid);
                }
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if (SobekCM_Database.Update_Disposition(itemid, typeid, notes, date, username))
                        {
                            thisRow["Disposition_Type"] = typeString;
                            thisRow["Disposition_Date"] = date;
                        }
                        updated++;
                    }
                }

                Cursor = Cursors.Default;
                gridPanel.Refresh();
                MessageBox.Show(updated + " records updated.");
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
                string typeString = SobekCM_Library_Settings.Disposition_Term_Future(typeid);
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if (thisRow["Disposition_Date"] == DBNull.Value)
                        {
                            if (SobekCM_Database.Edit_Disposition_Advice(itemid, typeid, notes))
                            {
                                thisRow["Disposition_Advice"] = typeString;
                                //thisRow["Disposition_Advice_Notes"] = notes;
                            }
                            updated++;
                        }
                    }
                }

                Cursor = Cursors.Default;
                gridPanel.Refresh();
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
                string type = workflowForm.Workflow_Type;
                DateTime date = workflowForm.Workflow_Date;
                string notes = workflowForm.Workflow_Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        SobekCM_Database.Add_Past_Workflow(itemid, type, notes, date, username, String.Empty);
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
                DateTime date = workflowForm.Date_Received;
                bool estimated = workflowForm.Estimated;
                string notes = workflowForm.Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if (SobekCM_Database.Update_Material_Received(itemid, date, estimated, username, notes))
                            thisRow["Material_Received_Date"] = date.ToShortDateString();
                        updated++;
                    }
                }

                Cursor = Cursors.Default;
                gridPanel.Refresh();
                MessageBox.Show(updated + " records updated.");
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
                bool newflag = workflowForm.Born_Digital_Flag;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if ( SobekCM_Database.Update_Born_Digital_Flag(itemid, newflag))
                            thisRow["Born_Digital"] = newflag;
                        updated++;
                    }
                }

                Cursor = Cursors.Default;
                gridPanel.Refresh();
                MessageBox.Show(updated + " records updated.");
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
                if (gridPanel.Export_as_Excel(saveFileDialog1.FileName, "Ad Hoc " + SobekCM_Library_Settings.System_Abbreviation + " Report", "Data"))
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
            if (displaySet == null)
                return;

            saveFileDialog1.Filter = "XML DataSet|*.xml";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                displaySet.WriteXml(saveFileDialog1.FileName, XmlWriteMode.WriteSchema);
                MessageBox.Show("Data exported as XML.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Preferences for when an item is clicked event handlers

        private void openItemFromWebInBrowserWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openItemFromWebMenuItem.Checked = true;
            viewItemFormMenuItem.Checked = false;

            SMaRT_UserSettings.Ad_Hoc_Form_Action_On_Click = View_Items_Form_Action_On_Click_Enum.Open_On_Web;
            SMaRT_UserSettings.Save();
        }

        private void viewItemFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openItemFromWebMenuItem.Checked = false;
            viewItemFormMenuItem.Checked = true;

            SMaRT_UserSettings.Ad_Hoc_Form_Action_On_Click = View_Items_Form_Action_On_Click_Enum.Show_Form;
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
            string bibid = thisRow["BibID"].ToString();

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
            string bibid = thisRow["BibID"].ToString();

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

            // Get the tracking box information
            Enter_Tracking_Box_Form trackingBox = new Enter_Tracking_Box_Form();
            if (trackingBox.ShowDialog() != DialogResult.OK) return;

            // Do all the work
            Cursor = Cursors.WaitCursor;
            int updated = 0;
            string new_tracking_box = trackingBox.New_Tracking_Box;
            foreach (DataRow thisRow in gridPanel.Selected_Row)
            {
                int itemid = Convert.ToInt32(thisRow["ItemID"]);
                if (itemid > 0)
                {
                    updated++;
                    if (SobekCM_Database.Save_New_Tracking_Box(itemid, new_tracking_box))
                        thisRow["Tracking_Box"] = new_tracking_box;
                }
            }
            Cursor = Cursors.Default;
            gridPanel.Refresh();
            MessageBox.Show(updated + " records updated.");
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
                string typeString = SobekCM_Library_Settings.Disposition_Term_Future(typeid);
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if (thisRow["Disposition_Date"] == DBNull.Value)
                        {
                            if (SobekCM_Database.Edit_Disposition_Advice(itemid, typeid, notes))
                            {
                                thisRow["Disposition_Advice"] = typeString;
                                //thisRow["Disposition_Advice_Notes"] = notes;
                            }
                            updated++;
                        }
                    }
                }

                Cursor = Cursors.Default;
                gridPanel.Refresh();
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
                string type = workflowForm.Workflow_Type;
                DateTime date = workflowForm.Workflow_Date;
                string notes = workflowForm.Workflow_Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        SobekCM_Database.Add_Past_Workflow(itemid, type, notes, date, username, String.Empty);
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
                int typeid = trackingBox.Disposition_Type_ID;
                DateTime date = trackingBox.Disposition_Date;
                string notes = trackingBox.Disposition_Notes;
                string typeString = SobekCM_Library_Settings.Disposition_Term_Past(typeid);
                if (notes.Trim().Length == 0)
                {
                    notes = SobekCM_Library_Settings.Disposition_Term_Past(typeid);
                }
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if (SobekCM_Database.Update_Disposition(itemid, typeid, notes, date, username))
                        {
                            thisRow["Disposition_Type"] = typeString;
                            thisRow["Disposition_Date"] = date;
                        }
                        updated++;
                    }
                }

                Cursor = Cursors.Default;
                gridPanel.Refresh();
                MessageBox.Show(updated + " records updated.");
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
                bool newflag = workflowForm.Born_Digital_Flag;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if (SobekCM_Database.Update_Born_Digital_Flag(itemid, newflag))
                            thisRow["Born_Digital"] = newflag;
                        updated++;
                    }
                }

                Cursor = Cursors.Default;
                MessageBox.Show(updated + " records updated.");
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
                DateTime date = workflowForm.Date_Received;
                bool estimated = workflowForm.Estimated;
                string notes = workflowForm.Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        if (SobekCM_Database.Update_Material_Received(itemid, date, estimated, username, notes))
                            thisRow["Material_Received_Date"] = date.ToShortDateString();
                        updated++;
                    }
                }

                Cursor = Cursors.Default;
                gridPanel.Refresh();
                MessageBox.Show(updated + " records updated.");
            }
        }

        #endregion
    }
}
