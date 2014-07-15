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
using SobekCM.Library.Items;
using SobekCM.Library.Navigation;
using SobekCM.Library.Search;
using SobekCM.Management_Tool.Settings;
using SobekCM.Management_Tool.Versioning;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form allows searches and browses to be performed to get a list of matching items
    /// from a SobekCM library and to perform certain tasks against those items </summary>
    public partial class Item_Discovery_Form : Form
    {
        #region Private class members

        private BibIdReport_Printer bibIdPrinter;
        private bool collapsed;
        private DataSet completeSet;
        private DataTable displayTable;
        private CustomGrid_Panel gridPanel;
        private CustomGrid_Printer gridPrinter;
        private bool gridPrinterShowAllRows = true;
        private int itemCount;
        private SobekCM_Search_Object lastSearch;
        private int titleCount;
        private DataRelation titleToItemRelation;
        private readonly string username;

        #endregion

        #region Constructor

        /// <summary> Constructor for a new instance of the View_Items_Form class </summary>
        public Item_Discovery_Form()
        {
            InitializeComponent();

            // Load the last settings for the item discovery panel
            SobekCM_Search_Object sobekCmSearchObject1 =
                new SobekCM_Search_Object(SMaRT_UserSettings.Discovery_Panel_Search_Term1,
                                          SMaRT_UserSettings.Discovery_Panel_Search_Term2,
                                          SMaRT_UserSettings.Discovery_Panel_Search_Term3,
                                          SMaRT_UserSettings.Discovery_Panel_Search_Term4);
            sobekCM_Item_Discovery_Panel1.Current_Search = sobekCmSearchObject1;


            BackColor = Color.FromArgb(240, 240, 240);

            // Set the data for the panel
            sobekCM_Item_Discovery_Panel1.Finish_Loading_Data();

            // Set some personalization and customization for the SobekCM Instance Name
            Text = SobekCM_Library_Settings.System_Abbreviation + " Item Discovery Form";
            mainLabel.Text = "View " + SobekCM_Library_Settings.System_Abbreviation + " Items";
            openWebContextMenuItem.Text = "Open Item/Group in " + SobekCM_Library_Settings.System_Abbreviation;

            filterSearchPanel.BackColor = Color.FromArgb(238, 238, 238);

            instructionLabel.Text = "Select search criteria below and press SEARCH.\n\nTo browse by collection or institution, select the code and press SEARCH. ";



            // Set the correct search precision
            sobekCM_Item_Discovery_Panel1.Search_Precision = SMaRT_UserSettings.Search_Precision;
            switch (SMaRT_UserSettings.Search_Precision)
            {
                case Search_Precision_Type_Enum.Contains:
                    precisionExactMenuItem.Checked = true;
                    precisionStandardMenuItem.Checked = false;
                    break;

                case Search_Precision_Type_Enum.Synonmic_Form:
                    precisionThesaurusMenuItem.Checked = true;
                    precisionStandardMenuItem.Checked = false;
                    break;
            }

            // Set the preferences for single group result sets
            switch (SMaRT_UserSettings.Item_Discovery_Form_Single_Result_Action)
            {
                case Single_Result_Action_Enum.Show_In_Grid:
                    displayInThisFormMenuItem.Checked = true;
                    automaticallyOpenInItemGroupFormMenuItem.Checked = false;
                    break;

                case Single_Result_Action_Enum.Show_Details_For_Single_Item:
                    displayInThisFormMenuItem.Checked = false;
                    automaticallyOpenInItemGroupFormMenuItem.Checked = true;
                    break;
            }

            // Set the correct landscape setting
            pageSetupDialog1.PageSettings.Landscape = SMaRT_UserSettings.Title_Grid_Print_Landscape;

            // Set the action on click 
            if (SMaRT_UserSettings.Item_Discovery_Form_Action_On_Click == View_Items_Form_Action_On_Click_Enum.Open_On_Web)
            {
                openItemGroupOnWebMenuItem.Checked = true;
                viewItemGroupFormMenuItem.Checked = false;
            }
            else
            {
                openItemGroupOnWebMenuItem.Checked = false;
                viewItemGroupFormMenuItem.Checked = true;
            }

            // Set the size correctly
            Size = SMaRT_UserSettings.Item_Discovery_Form_Size;
            int screen_width = Screen.PrimaryScreen.WorkingArea.Width;
            int screen_height = Screen.PrimaryScreen.WorkingArea.Height;
            if ((Width > screen_width) || (Height > screen_height) || ( SMaRT_UserSettings.Item_Discovery_Form_Maximized ))
                WindowState = FormWindowState.Maximized;

            // GEt the username
            username = Environment.UserName;
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
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
                brush.SetBlendTriangularShape(0.33F);

                // Create the image
                gr.FillRectangle(brush, rect);
                gr.Dispose();

                // Set this as the backgroundf
                BackgroundImage = image;
            }
        }

        #endregion
 
        #region Methods to search or browse and to display within this form 

        private void Clear_Items( bool clear_search_boxes )
        {
            if ((gridPanel != null) && (mainPanel.Contains(gridPanel)))
            {
                mainPanel.Controls.Remove(gridPanel);
                gridPanel = null;
            }

            displayTable = null;


            // Clear the serach boxes and show instructions
            hitCountLabel.Text = "Select search criteria above and press SEARCH";
            if (clear_search_boxes)
            {
                sobekCM_Item_Discovery_Panel1.Clear_Search_Boxes();
            }
            instructionLabel.Show();

            // Disable the menu items which are no longer applicable
            saveAsMenuItem.Enabled = false;
            printMenuItem.Enabled = false;
            printTrackingSheetsMenuItem.Enabled = false;
            editSerialHierarchyToolStripMenuItem.Enabled = false;
            viewAdHocReportMenuItem.Enabled = false;
            setTrackingBoxToolStripMenuItem.Enabled = false;
            editDispositionAdviceMenuItem.Enabled = false;
            updateItemDispositionMenuItem.Enabled = false;
            addWorklogHistoryEntryMenuItem.Enabled = false;
            updateBornDigitalFlagMenuItem.Enabled = false;
            updateMaterialReceivedDateMenuItem.Enabled = false;
        }

        private void Show_Items(DataSet dataSet, string Code_for_browse)
        {
            database_dataset_to_tracking_bib_table(dataSet);

            if ((displayTable == null) || (displayTable.Rows.Count == 0))
            {
                if (gridPanel != null)
                {
                    mainPanel.Controls.Remove(gridPanel);
                    gridPanel = null;
                }
                Clear_Items(false);
                hitCountLabel.Text = "No Matches Found";
            }
            else
            {
                instructionLabel.Hide();
                if (gridPanel == null)
                {
                    gridPanel = new CustomGrid_Panel
                                    {
                                        Size = new Size(mainPanel.Width - 2, mainPanel.Height - 2),
                                        Location = new Point(0, 0)
                                    };

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
                    mainPanel.Controls.Add(gridPanel);

                    // Add the event to the grid panel
                    gridPanel.Double_Clicked += gridPanel_Double_Clicked;
                    gridPanel.Clipboard_Copy_Requested +=new CustomGrid_Panel_Delegate_Multiple(gridPanel_Clipboard_Copy_Requested);

                    // Add the context menu
                    gridPanel.Set_Context_Menus(null, contextMenu1);

                    // Add the object to print
                    gridPrinter = new CustomGrid_Printer(gridPanel, printDocument1);
                }
                else
                {
                    // Save the column sizes
                    SMaRT_UserSettings.Discovery_Form_Title_Width = gridPanel.Style.Column_Styles[5].Width;
                    SMaRT_UserSettings.Discovery_Form_Author_Width = gridPanel.Style.Column_Styles[6].Width;
                    SMaRT_UserSettings.Discovery_Form_Publisher_Width = gridPanel.Style.Column_Styles[7].Width;
                }

                gridPanel.DataTable = displayTable;

                gridPanel.Current_Sort_String = "BibID ASC";

                // Configure for this table
                gridPanel.Style.Primary_Key_Column = 0;
                gridPanel.Style.Column_Styles[0].Visible = false;
                gridPanel.Style.Column_Styles[1].Fixed_Print_Width = 100;
                gridPanel.Style.Column_Styles[2].Width = 85;
                gridPanel.Style.Column_Styles[2].Fixed_Print_Width = 65;
                gridPanel.Style.Column_Styles[3].Fixed_Print_Width = 100;
                gridPanel.Style.Column_Styles[4].Fixed_Print_Width = 100;
                gridPanel.Style.Column_Styles[5].Width = SMaRT_UserSettings.Discovery_Form_Title_Width; 
                gridPanel.Style.Column_Styles[5].Text_Alignment = HorizontalAlignment.Left;
                gridPanel.Style.Column_Styles[5].BackColor = Color.White;
                gridPanel.Style.Column_Styles[6].Width = SMaRT_UserSettings.Discovery_Form_Author_Width;
                gridPanel.Style.Column_Styles[6].Text_Alignment = HorizontalAlignment.Left;
                gridPanel.Style.Column_Styles[6].BackColor = Color.White;
                gridPanel.Style.Column_Styles[7].Width = SMaRT_UserSettings.Discovery_Form_Publisher_Width;
                gridPanel.Style.Column_Styles[7].Text_Alignment = HorizontalAlignment.Left;
                gridPanel.Style.Column_Styles[7].BackColor = Color.White;
                gridPanel.Style.Column_Styles[8].Width = 100;
                gridPanel.Style.Column_Styles[9].Width = 120;
                gridPanel.Style.Column_Styles[10].Visible = false;
                gridPanel.Style.Column_Styles[11].Visible = false;

                gridPanel.Style.Column_Styles[3].Ascending_Sort = "ALEPH ASC, BibID ASC";
                gridPanel.Style.Column_Styles[3].Descending_Sort = "ALEPH DESC, BibID DESC";

                gridPanel.Style.Column_Styles[4].Ascending_Sort = "OCLC ASC, BibID ASC";
                gridPanel.Style.Column_Styles[4].Descending_Sort = "OCLC DESC, BibID DESC";

                gridPanel.Style.Column_Styles[5].Ascending_Sort = "SortTitle ASC, BibID ASC";
                gridPanel.Style.Column_Styles[5].Descending_Sort = "SortTitle DESC, BibID DESC";

                gridPanel.Style.Column_Styles[8].Ascending_Sort = "Material_Type ASC, BibID ASC";
                gridPanel.Style.Column_Styles[8].Descending_Sort = "Material_Type DESC, BibID DESC";

                gridPanel.Style.Column_Styles[9].Ascending_Sort = "Aggregations ASC, BibID ASC";
                gridPanel.Style.Column_Styles[9].Descending_Sort = "Aggregations DESC, BibID DESC";


                if (Code_for_browse.Length > 0)
                {
                    switch (displayTable.Rows.Count)
                    {
                        case 1: hitCountLabel.Text = "Your browse resulted in one matching title"; break;
                        default: hitCountLabel.Text = "Your browse resulted in " + number_to_string(displayTable.Rows.Count) + " matching titles"; break;
                    }
                }
                else
                {
                    hitCountLabel.Text = "Your search resulted in " + number_to_string(itemCount) + " items in " + number_to_string(titleCount) + " titles";
                }

                // Determine if any ALEPH, OCLC, AUTHOR, or PUBLISHER data is present
                bool checkingAleph = true;
                bool checkingOclc = true;
                bool checkingAuthor = true;
                bool checkingPublisher = true;
                DataColumn alephColumn = displayTable.Columns["Aleph"];
                DataColumn oclcColumn = displayTable.Columns["OCLC"];
                DataColumn authorColumn = displayTable.Columns["Author"];
                DataColumn publisherColumn = displayTable.Columns["Publisher"];
                foreach (DataRow thisRow in displayTable.Rows)
                {
                    if (checkingAleph)
                    {
                        if (thisRow[alephColumn].ToString().Trim().Length > 0)
                        {
                            checkingAleph = false;
                        }
                    }

                    if (checkingOclc)
                    {
                        if (thisRow[oclcColumn].ToString().Trim().Length > 0)
                        {
                            checkingOclc = false;
                        }
                    }

                    if (checkingAuthor)
                    {
                        if (thisRow[authorColumn].ToString().Trim().Length > 0)
                        {
                            checkingAuthor = false;
                        }
                    }

                    if (checkingPublisher)
                    {
                        if (thisRow[publisherColumn].ToString().Trim().Length > 0)
                        {
                            checkingPublisher = false;
                        }
                    }

                    if ((!checkingAleph) && (!checkingAuthor) && (!checkingOclc) && (!checkingPublisher))
                        break;
                }

                // Set the visibility of the columns based on the data
                gridPanel.Style.Column_Styles[3].Visible = !checkingAleph;
                gridPanel.Style.Column_Styles[4].Visible = !checkingOclc;
                gridPanel.Style.Column_Styles[6].Visible = !checkingAuthor;
                gridPanel.Style.Column_Styles[7].Visible = !checkingPublisher;

                // Enable the menu items which are now applicable
                saveAsMenuItem.Enabled = true;
                printMenuItem.Enabled = true;
                printTrackingSheetsMenuItem.Enabled = true;
                editSerialHierarchyToolStripMenuItem.Enabled = true;
                viewAdHocReportMenuItem.Enabled = true;
                setTrackingBoxToolStripMenuItem.Enabled = true;
                editDispositionAdviceMenuItem.Enabled = true;
                updateItemDispositionMenuItem.Enabled = true;
                addWorklogHistoryEntryMenuItem.Enabled = true;
                updateBornDigitalFlagMenuItem.Enabled = true;
                updateMaterialReceivedDateMenuItem.Enabled = true;

                // Now, some final code if there was only one result
                if ((displayTable.Rows.Count == 1) && (SMaRT_UserSettings.Item_Discovery_Form_Single_Result_Action == Single_Result_Action_Enum.Show_Details_For_Single_Item))
                {
                    // What is the currently selected action?
                    if (openItemGroupOnWebMenuItem.Checked)
                    {
                        string bibid = displayTable.Rows[0]["BibID"].ToString();
                        string vid = displayTable.Rows[0]["VID"].ToString();
                        string url = SobekCM_Library_Settings.System_Base_URL+ bibid;
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
                        // Save the current size
                        SMaRT_UserSettings.Item_Discovery_Form_Size = Size;
                        SMaRT_UserSettings.Save();

                        // Now, show the row 
                        View_Item_Group_Form showForm = new View_Item_Group_Form(displayTable.Rows[0]);
                        Hide();
                        showForm.ShowDialog();
                        Show();
                    }
                }

                gridPanel.ReDraw();
                gridPanel.Focus();
            }
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

        private void sobekCM_Item_Discovery_Panel1_Search_Requested()
        {
            lastSearch = sobekCM_Item_Discovery_Panel1.Current_Search;

            // Since this is a new search, save the last used parameters
            SMaRT_UserSettings.Discovery_Panel_Search_Term1 = lastSearch.First_Term;
            SMaRT_UserSettings.Discovery_Panel_Search_Term2 = lastSearch.Second_Term;
            SMaRT_UserSettings.Discovery_Panel_Search_Term3 = lastSearch.Third_Term;
            SMaRT_UserSettings.Discovery_Panel_Search_Term4 = lastSearch.Fourth_Term;
            SMaRT_UserSettings.Save();

            Cursor = Cursors.WaitCursor;
            DataSet resultSet = lastSearch.Perform_Tracking_Search();
            Cursor = Cursors.Default;

            if (resultSet == null)
                Clear_Items(false);
            else
                Show_Items(resultSet, String.Empty);

            sobekCM_Item_Discovery_Panel1.Invalidate(true);
        }

        private void searchButton_Button_Pressed(object sender, EventArgs e)
        {
            lastSearch = sobekCM_Item_Discovery_Panel1.Current_Search;

            // Since this is a new search, save the last used parameters
            SMaRT_UserSettings.Discovery_Panel_Search_Term1 = lastSearch.First_Term;
            SMaRT_UserSettings.Discovery_Panel_Search_Term2 = lastSearch.Second_Term;
            SMaRT_UserSettings.Discovery_Panel_Search_Term3 = lastSearch.Third_Term;
            SMaRT_UserSettings.Discovery_Panel_Search_Term4 = lastSearch.Fourth_Term;
            SMaRT_UserSettings.Save();

            Cursor = Cursors.WaitCursor;
            DataSet resultSet = lastSearch.Perform_Tracking_Search();
            Cursor = Cursors.Default;

            if (resultSet == null)
                Clear_Items(false);
            else
                Show_Items(resultSet, String.Empty);

            sobekCM_Item_Discovery_Panel1.Invalidate(true);
        }


        private void database_dataset_to_tracking_bib_table(DataSet dataSet)
        {
            // Do nothing if the returned value is NULL
            if (dataSet == null)
            {
                displayTable = null;
                return;
            }

            // Just save the entire set for use later if needed
            completeSet = dataSet;

            // Save the different counts
            titleCount = dataSet.Tables[1].Rows.Count;
            itemCount = dataSet.Tables[0].Rows.Count;

            // Build the table for display
            if (displayTable == null)
            {
                DataSet displaySet = new DataSet("SMaRT");
                displayTable = new DataTable("Matches");
                displayTable.Columns.Add("TitleID", Type.GetType("System.Int32"));
                displayTable.Columns.Add("BibID");
                displayTable.Columns.Add("VID");
                displayTable.Columns.Add("Aleph");
                displayTable.Columns.Add("OCLC");
                displayTable.Columns.Add("Group_Title");
                displayTable.Columns.Add("Author");
                displayTable.Columns.Add("Publisher");
                displayTable.Columns.Add("Material_Type");
                displayTable.Columns.Add("Aggregations");
                displayTable.Columns.Add("ItemID", Type.GetType("System.Int32"));
                displayTable.Columns.Add("SortTitle");
                displaySet.Tables.Add(displayTable);
            }
            else
            {
                displayTable.Rows.Clear();
            }

            // Add a relationship to the original dataset
            if (dataSet.Relations.Count == 0)
            {
                dataSet.Relations.Add(dataSet.Tables[1].Columns["TitleID"], dataSet.Tables[0].Columns["fk_TitleID"]);
            }
            titleToItemRelation = dataSet.Relations[0];

            // Get column references for the title table
            DataColumn titleIdColumn = dataSet.Tables[1].Columns["TitleID"];
            DataColumn bibidColumn = dataSet.Tables[1].Columns["BibID"];
            DataColumn groupTitleColumn = dataSet.Tables[1].Columns["GroupTitle"];
            DataColumn typeColumn = dataSet.Tables[1].Columns["Type"];
            DataColumn oclcColumn = dataSet.Tables[1].Columns["OCLC_Number"];
            DataColumn alephColumn = dataSet.Tables[1].Columns["ALEPH_Number"];
            DataColumn groupSortTitleColumn = dataSet.Tables[1].Columns["SortTitle"];

            // Get the column references for the item table
            DataColumn authorColumn = dataSet.Tables[0].Columns["Author"];
            DataColumn publisherColumn = dataSet.Tables[0].Columns["Publisher"];
            DataColumn vidColumn = dataSet.Tables[0].Columns["VID"];
            DataColumn aggregationsColumn = dataSet.Tables[0].Columns["AggregationCodes"];
            DataColumn itemColumn = dataSet.Tables[0].Columns["ItemID"];

            // Get column references for the display / return table
            DataColumn returnTitleIdColumn = displayTable.Columns[0];
            DataColumn returnBibIdColumn = displayTable.Columns[1];
            DataColumn returnVidColumn = displayTable.Columns[2];
            DataColumn returnAlephColumn = displayTable.Columns[3];
            DataColumn returnOclcColumn = displayTable.Columns[4];
            DataColumn returnGroupTitleColumn = displayTable.Columns[5];
            DataColumn returnAuthorColumn = displayTable.Columns[6];
            DataColumn returnPublisherColumn = displayTable.Columns[7];
            DataColumn returnTypeColumn = displayTable.Columns[8];
            DataColumn returnAggregationsColumn = displayTable.Columns[9];
            DataColumn returnItemColumn = displayTable.Columns[10];
            DataColumn returnSortTitleColumn = displayTable.Columns[11];


            // Step through each title
            foreach (DataRow titleRow in dataSet.Tables[1].Rows)
            {
                // Create new row
                DataRow newRow = displayTable.NewRow();

                // Add the fields from the main title table
                newRow[returnTitleIdColumn] = titleRow[titleIdColumn];
                newRow[returnBibIdColumn] = titleRow[bibidColumn];
                newRow[returnGroupTitleColumn] = titleRow[groupTitleColumn];
                newRow[returnSortTitleColumn] = titleRow[groupSortTitleColumn];
                newRow[returnTypeColumn] = titleRow[typeColumn].ToString().ToUpper();
                string oclc = titleRow[oclcColumn].ToString();
                if ((oclc != "0") && (oclc != "1") && (oclc != "-1"))
                    newRow[returnOclcColumn] = oclc;
                else
                    newRow[returnOclcColumn] = String.Empty;
                string aleph = titleRow[alephColumn].ToString();
                if ((aleph != "0") && (aleph != "1") && (aleph != "-1"))
                    newRow[returnAlephColumn] = aleph;
                else
                    newRow[returnAlephColumn] = String.Empty;

                // Look for children columns
                DataRow[] childRows = titleRow.GetChildRows(titleToItemRelation);
                if (childRows.Length == 0)
                {
                    newRow[returnAuthorColumn] = String.Empty;
                    newRow[returnPublisherColumn] = String.Empty;
                    newRow[returnVidColumn] = "(none)";
                }
                else
                {
                    if (childRows.Length == 1)
                    {
                        newRow[returnVidColumn] = childRows[0][vidColumn];
                        newRow[returnAuthorColumn] = childRows[0][authorColumn].ToString().Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
                        newRow[returnPublisherColumn] = childRows[0][publisherColumn].ToString().Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
                        newRow[returnAggregationsColumn] = childRows[0][aggregationsColumn].ToString().Trim().Replace("  ", "").Replace(" ", ",");
                        if (itemColumn == null)
                        {
                            newRow[returnItemColumn] = -1;
                        }
                        else
                        {
                            newRow[returnItemColumn] = childRows[0][itemColumn];
                        }
                    }
                    else
                    {
                        newRow[returnVidColumn] = "(" + childRows.Length.ToString() + " volumes)";
                        newRow[returnItemColumn] = -1;
                        bool still_checking_author = true;
                        bool still_checking_publisher = true;
                        string author = childRows[0][authorColumn].ToString().Trim();
                        string publisher = childRows[0][publisherColumn].ToString().Trim();
                        int item_counter = 0;
                        foreach (DataRow childRow in childRows)
                        {
                            if (still_checking_author)
                            {
                                if (childRow[authorColumn].ToString().Trim() != author)
                                {
                                    still_checking_author = false;
                                    author = "( varies )";
                                }
                            }

                            if (still_checking_publisher)
                            {
                                if (childRow[publisherColumn].ToString().Trim() != publisher)
                                {
                                    still_checking_publisher = false;
                                    publisher = "( varies )";
                                }
                            }

                            item_counter++;
                            if (((!still_checking_author) && (!still_checking_publisher)) || ( item_counter > 20 ))
                                break;
                        }
                        newRow[returnAuthorColumn] = author.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
                        newRow[returnPublisherColumn] = publisher.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
                        newRow[returnAggregationsColumn] = String.Empty;

                    }
                }

                // Add this row to the table
                displayTable.Rows.Add(newRow);
            }

            // Refresh the datagrid
            if (gridPanel != null)
            {
                gridPanel.Refresh_DataTable(displayTable);
            }
        }

        #endregion

        #region Methods to print the tracking sheets or the title spreadsheet

        private void print_tracking_sheets()
        {
            if (gridPanel == null)
                return;

            if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            {
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    PrintDocument printReport = new PrintDocument();
                    printReport.PrintPage += printReport_PrintPage;
                    bibIdPrinter = new BibIdReport_Printer(printReport, thisRow["BibID"].ToString(), "", thisRow["Group_Title"].ToString(), "", thisRow["Author"].ToString(), thisRow["Publisher"].ToString(), thisRow["Material_Type"].ToString(), thisRow["Aleph"].ToString(), thisRow["OCLC"].ToString(), "", "", "");
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

        private void print_grid_style_report(bool All_Rows)
        {
            if (gridPanel == null)
                return;

            if ((!All_Rows) && ((gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0)))
            {
                MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

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
                pageSetupDialog1.PageSettings.Landscape = SMaRT_UserSettings.Title_Grid_Print_Landscape;
                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                    SMaRT_UserSettings.Title_Grid_Print_Landscape = pageSetupDialog1.PageSettings.Landscape;
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

        #endregion

        #region Form-related (Non-menu driven) event-handling methods

        protected override void OnClosing(CancelEventArgs e)
        {
            // Save the window size
            if (WindowState != FormWindowState.Maximized)
            {
                SMaRT_UserSettings.Item_Discovery_Form_Size = Size;
                SMaRT_UserSettings.Item_Discovery_Form_Maximized = false;
            }
            else
                SMaRT_UserSettings.Item_Discovery_Form_Maximized = true;

            // Save the column sizes
            if (gridPanel != null)
            {
                SMaRT_UserSettings.Discovery_Form_Title_Width = gridPanel.Style.Column_Styles[5].Width;
                SMaRT_UserSettings.Discovery_Form_Author_Width = gridPanel.Style.Column_Styles[6].Width;
                SMaRT_UserSettings.Discovery_Form_Publisher_Width = gridPanel.Style.Column_Styles[7].Width;
            }

            SMaRT_UserSettings.Save();
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!collapsed)
            {
                collapsed = true;
                pictureBox1.Image = imageList1.Images[1];
                filterSearchPanel.Height = 25;
                filterSearchPanel.Location = new Point(filterSearchPanel.Location.X, filterSearchPanel.Location.Y + 191);
                filterSearchPanel.Invalidate();
                mainPanel.Height = mainPanel.Height + 191;
                searchButton.Hide();
                clearButton.Hide();

            }
            else
            {
                collapsed = false;
                pictureBox1.Image = imageList1.Images[0];
                filterSearchPanel.Height = 216;
                filterSearchPanel.Location = new Point(filterSearchPanel.Location.X, filterSearchPanel.Location.Y - 191);
                filterSearchPanel.Invalidate();
                mainPanel.Height = mainPanel.Height - 191;
                searchButton.Show();
                clearButton.Show();
            }
        }

        private void filterSearchPanel_Paint(object sender, PaintEventArgs e)
        {
            if (!collapsed)
            {
                e.Graphics.DrawLine(new Pen(Color.LightGray, 1), 387, 15, 387, filterSearchPanel.Height - 45);
                e.Graphics.DrawLine(new Pen(Color.LightGray, 1), 15, filterSearchPanel.Height - 45, filterSearchPanel.Width - 30, filterSearchPanel.Height - 45);
            }
        }

        private void clearButton_Button_Pressed(object sender, EventArgs e)
        {
            Clear_Items(true);
        }

        #endregion

        #region Main Menu Item Event Handlers

        private void pageSetupMenuItem_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.ShowDialog();

            SMaRT_UserSettings.Title_Grid_Print_Landscape = pageSetupDialog1.PageSettings.Landscape;
            SMaRT_UserSettings.Save();
        }

        private void printPreviewMenuItem_Click(object sender, EventArgs e)
        {
            if (gridPanel == null)
                return;

            gridPrinter.Prepare_To_Print();
            printPreviewDialog1.ShowDialog();
        }

        private void printMenuItem_Click(object sender, EventArgs e)
        {
            if (gridPanel == null)
                return;

            gridPrinter.Prepare_To_Print();
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void printAllRowsMenuItem_Click(object sender, EventArgs e)
        {
            print_grid_style_report(true);
        }

        private void printSelectedRowsMenuItem_Click(object sender, EventArgs e)
        {
            print_grid_style_report(false);
        }


        private void printTrackingSheetsMenuItem_Click(object sender, EventArgs e)
        {
            print_tracking_sheets();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            // Show the about form
            About showAbout = new About("SobekCM Management and Reporting Tool", VersionConfigSettings.AppVersion);
            showAbout.ShowDialog();
        }

        private void viewAdHocReportMenuItem_Click(object sender, EventArgs e)
        {
            Ad_Hoc_Reporting_Query_Form showForm = new Ad_Hoc_Reporting_Query_Form(lastSearch, completeSet);
            Hide();
            showForm.ShowDialog();
            Show();

            //Ad_Hoc_Report_Display_Form showForm = new Ad_Hoc_Report_Display_Form(completeSet);
            //this.Hide();
            //showForm.ShowDialog();
            //this.Show();
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
                int skipped = 0;
                string new_tracking_box = trackingBox.New_Tracking_Box;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        updated++;
                        SobekCM_Database.Save_New_Tracking_Box(itemid, new_tracking_box);
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
                        SobekCM_Database.Update_Disposition(itemid, typeid, notes, date, username);
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
                int skipped = 0;
                int typeid = trackingBox.Disposition_Type_ID;
                string notes = trackingBox.Disposition_Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        // Verify this wasn't already disposed off, in which case no point in allowing this edit
                        DataRow[] selectedRow = completeSet.Tables[0].Select("ItemID=" + itemid);
                        if (selectedRow.Length > 0)
                        {
                            if (selectedRow[0]["Disposition_Date"] == DBNull.Value)
                            {
                                SobekCM_Database.Edit_Disposition_Advice(itemid, typeid, notes);
                                updated++;
                            }
                        }
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
                int skipped = 0;
                DateTime date = workflowForm.Date_Received;
                bool estimated = workflowForm.Estimated;
                string notes = workflowForm.Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        SobekCM_Database.Update_Material_Received(itemid, date, estimated, username, notes);
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
                        SobekCM_Database.Update_Born_Digital_Flag(itemid, newflag);
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
                if (gridPanel.Export_as_Excel(saveFileDialog1.FileName, "Selected " + SobekCM_Library_Settings.System_Abbreviation + " Items", SobekCM_Library_Settings.System_Abbreviation + " Items"))
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
                if ( gridPanel.Export_as_Text(saveFileDialog1.FileName, '\t'))
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

        #region Preferences for when a title is clicked event handlers

        private void openItemGroupOnWebMenuItem_Click(object sender, EventArgs e)
        {
            openItemGroupOnWebMenuItem.Checked = true;
            viewItemGroupFormMenuItem.Checked = false;

            SMaRT_UserSettings.Item_Discovery_Form_Action_On_Click = View_Items_Form_Action_On_Click_Enum.Open_On_Web;
            SMaRT_UserSettings.Save();
        }

        private void viewItemGroupFormMenuItem_Click(object sender, EventArgs e)
        {
            openItemGroupOnWebMenuItem.Checked = false;
            viewItemGroupFormMenuItem.Checked = true;

            SMaRT_UserSettings.Item_Discovery_Form_Action_On_Click = View_Items_Form_Action_On_Click_Enum.Show_Form;
            SMaRT_UserSettings.Save();
        }

        #endregion

        #region Preferences for result sets with only one item group event handlers

        private void displayInThisFormMenuItem_Click(object sender, EventArgs e)
        {
            displayInThisFormMenuItem.Checked = true;
            automaticallyOpenInItemGroupFormMenuItem.Checked = false;

            SMaRT_UserSettings.Item_Discovery_Form_Single_Result_Action = Single_Result_Action_Enum.Show_In_Grid;
            SMaRT_UserSettings.Save();
        }

        private void automaticallyOpenInItemGroupFormMenuItem_Click(object sender, EventArgs e)
        {
            displayInThisFormMenuItem.Checked = false;
            automaticallyOpenInItemGroupFormMenuItem.Checked = true;

            SMaRT_UserSettings.Item_Discovery_Form_Single_Result_Action = Single_Result_Action_Enum.Show_Details_For_Single_Item;
            SMaRT_UserSettings.Save();
        }

        #endregion

        #region Search precision event handlers

        private void precisionExactMenuItem_Click(object sender, EventArgs e)
        {
            precisionExactMenuItem.Checked = true;
            precisionStandardMenuItem.Checked = false;
            precisionThesaurusMenuItem.Checked = false;

            sobekCM_Item_Discovery_Panel1.Search_Precision = Search_Precision_Type_Enum.Contains;
            SMaRT_UserSettings.Search_Precision = Search_Precision_Type_Enum.Contains;
            SMaRT_UserSettings.Save();
        }

        private void precisionStandardMenuItem_Click(object sender, EventArgs e)
        {
            precisionExactMenuItem.Checked = false;
            precisionStandardMenuItem.Checked = true;
            precisionThesaurusMenuItem.Checked = false;

            sobekCM_Item_Discovery_Panel1.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            SMaRT_UserSettings.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            SMaRT_UserSettings.Save();
        }

        private void precisionThesaurusMenuItem_Click(object sender, EventArgs e)
        {
            precisionExactMenuItem.Checked = false;
            precisionStandardMenuItem.Checked = false;
            precisionThesaurusMenuItem.Checked = true;

            sobekCM_Item_Discovery_Panel1.Search_Precision = Search_Precision_Type_Enum.Synonmic_Form;
            SMaRT_UserSettings.Search_Precision = Search_Precision_Type_Enum.Synonmic_Form;
            SMaRT_UserSettings.Save();
        }

        #endregion

        #endregion

        #region Context menu event handlers

        private void detailsContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || ( gridPanel.Selected_Row == null ) || ( gridPanel.Selected_Row.Length == 0 ))
                return;

            // Get first row then
            DataRow thisRow = gridPanel.Selected_Row[0];

            // Save the current size
            SMaRT_UserSettings.Item_Discovery_Form_Size = Size;
            SMaRT_UserSettings.Save();

            // Now, show the row 
            View_Item_Group_Form showForm = new View_Item_Group_Form(thisRow);
            Hide();
            showForm.ShowDialog();
            Show();
        }

        private void printedSelectedRowsContextMenuItem_Click(object sender, EventArgs e)
        {
            print_grid_style_report(false);
        }

        private void printAllRowsContextMenuItem_Click(object sender, EventArgs e)
        {
            print_grid_style_report(true);
        }

        private void printBibIdReportContextMenuItem_Click(object sender, EventArgs e)
        {
            print_tracking_sheets();
        }

        private void openWebContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            // Get first row then
            DataRow thisRow = gridPanel.Selected_Row[0];

            string bibid = thisRow["BibID"].ToString();
            string vid = thisRow["VID"].ToString();
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

        private void setTrackingBoxContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Enter_Tracking_Box_Form trackingBox = new Enter_Tracking_Box_Form();
            if (trackingBox.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                string new_tracking_box = trackingBox.New_Tracking_Box;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        updated++;
                        SobekCM_Database.Save_New_Tracking_Box(itemid, new_tracking_box);
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

        private void editDispositionAdviceContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            Edit_Disposition_Advice_Form trackingBox = new Edit_Disposition_Advice_Form();
            if (trackingBox.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                int updated = 0;
                int skipped = 0;
                int typeid = trackingBox.Disposition_Type_ID;
                string notes = trackingBox.Disposition_Notes;
                foreach (DataRow thisRow in gridPanel.Selected_Row)
                {
                    int itemid = Convert.ToInt32(thisRow["ItemID"]);
                    if (itemid > 0)
                    {
                        // Verify this wasn't already disposed off, in which case no point in allowing this edit
                        DataRow[] selectedRow = completeSet.Tables[0].Select("ItemID=" + itemid);
                        if (selectedRow.Length > 0)
                        {
                            if (selectedRow[0]["Disposition_Date"] == DBNull.Value)
                            {
                                SobekCM_Database.Edit_Disposition_Advice(itemid, typeid, notes);
                                updated++;
                            }
                        }
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
                        SobekCM_Database.Update_Disposition(itemid, typeid, notes, date, username);
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
                        SobekCM_Database.Update_Born_Digital_Flag(itemid, newflag);
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
                        SobekCM_Database.Update_Material_Received(itemid, date, estimated, username, notes);
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
                // What is the currently selected action?
                if (openItemGroupOnWebMenuItem.Checked)
                {
                    string bibid = thisRow["BibID"].ToString();
                    string vid = thisRow["VID"].ToString();
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
                    // Save the current size
                    SMaRT_UserSettings.Item_Discovery_Form_Size = Size;
                    SMaRT_UserSettings.Save();

                    // Now, show the row 
                    View_Item_Group_Form showForm = new View_Item_Group_Form(thisRow);
                    Hide();
                    showForm.ShowDialog();
                    Show();
                }
            }
        }

        #endregion

        #region Methods to edit the serial hierarchy

        private void editSerialHierarchyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            // Get first row then
            DataRow thisRow = gridPanel.Selected_Row[0];

            string bibid = thisRow["BibID"].ToString();

            SobekCM_Items_In_Title multiple = Library.Database.SobekCM_Database.Get_Multiple_Volumes(bibid, null);
            if (multiple == null)
            {
                MessageBox.Show("Database error occurred");
            }
            else
            {
                Hide();
                Edit_Serial_Hierarchy_Form edit = new Edit_Serial_Hierarchy_Form(bibid, multiple);
                edit.ShowDialog();
                Show();
            }
        }

        private void serialHierarchyContextMenuItem_Click(object sender, EventArgs e)
        {
            if ((gridPanel == null) || (gridPanel.Selected_Row == null) || (gridPanel.Selected_Row.Length == 0))
                return;

            // Get first row then
            DataRow thisRow = gridPanel.Selected_Row[0];

            string bibid = thisRow["BibID"].ToString();

            SobekCM_Items_In_Title multiple = Library.Database.SobekCM_Database.Get_Multiple_Volumes(bibid, null);
            if (multiple == null)
            {
                MessageBox.Show("Database error occurred");
            }
            else
            {
                Hide();
                Edit_Serial_Hierarchy_Form edit = new Edit_Serial_Hierarchy_Form(bibid, multiple);
                edit.ShowDialog();
                Show();
            }
        }

        #endregion
    }
}
