#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DLC.Tools.Forms;
using SobekCM.Library.Database;
using SobekCM.Management_Tool.Importer;
using SobekCM.Management_Tool.Importer.Forms;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form is used to import a spreadsheet of new volumes to add to a single title  </summary>
    public partial class Volume_Import_Form : Form
    {
        private Dictionary<string, string> columnname_to_input;
        private string filename = String.Empty;
        private Thread processThread;
        private Volume_Import_Processor processor;
        private DataTable rawDataTbl;
        private readonly DataTable transformedDataTbl;

        /// <summary> Constructor for a new instance of the Volume_Import_Form form  </summary>
        public Volume_Import_Form()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);
            visibilityComboBox.SelectedIndex = 1;

            

            bibidLabel.Hide();
            bibIdTextBox.TextChanged += bibIdTextBox_TextChanged;

            // Create the transformed data table
            transformedDataTbl = new DataTable();
            transformedDataTbl.Columns.Add("BibID");
            transformedDataTbl.Columns.Add("VID");
            transformedDataTbl.Columns.Add("Title");
            transformedDataTbl.Columns.Add("Level1_Text");
            transformedDataTbl.Columns.Add("Level1_Index", Type.GetType("System.Int16"));
            transformedDataTbl.Columns.Add("Level2_Text");
            transformedDataTbl.Columns.Add("Level2_Index", Type.GetType("System.Int16"));
            transformedDataTbl.Columns.Add("Level3_Text");
            transformedDataTbl.Columns.Add("Level3_Index", Type.GetType("System.Int16"));
            transformedDataTbl.Columns.Add("PubDate");
            transformedDataTbl.Columns.Add("Tracking_Box");
            transformedDataTbl.Columns.Add("Received_Date");
            transformedDataTbl.Columns.Add("Disposition_Advice");

            columnname_to_input = new Dictionary<string, string>();

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                fileTextBox.BorderStyle = BorderStyle.FixedSingle;
                bibIdTextBox.BorderStyle = BorderStyle.FixedSingle;
                sheetComboBox.FlatStyle = FlatStyle.Flat;
                vidComboBox.FlatStyle = FlatStyle.Flat;
                browseButton.FlatStyle = FlatStyle.Flat;
                btnShowData.FlatStyle = FlatStyle.Flat;
                viewOnlineButton.FlatStyle = FlatStyle.Flat;
                visibilityComboBox.FlatStyle = FlatStyle.Flat;
            }
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary> Constructor for a new instance of the Volume_Import_Form form </summary>
        /// <param name="bibid">Bibliographic identifier for the title to add volumes to</param>
        /// <param name="grouptitle">Group title for the title to add volumes to</param>
        /// <param name="childTable"></param>
        public Volume_Import_Form( string bibid, string grouptitle, DataTable childTable )
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);
            visibilityComboBox.SelectedIndex = 1;

            bibidLabel.Text = bibid;
            bibIdTextBox.Text = bibid;
            titleLabel.Font = Font;
            titleLabel.Text = grouptitle;
            DataColumn vidColumn = childTable.Columns["VID"];
            foreach (DataRow vidRow in childTable.Rows)
            {
                vidComboBox.Items.Add(vidRow[vidColumn].ToString());
            }
            vidComboBox.SelectedIndex = vidComboBox.Items.Count - 1;
            bibIdTextBox.Hide();

            // Create the transformed data table
            transformedDataTbl = new DataTable();
            transformedDataTbl.Columns.Add("BibID");
            transformedDataTbl.Columns.Add("VID");
            transformedDataTbl.Columns.Add("Title");
            transformedDataTbl.Columns.Add("Level1_Text");
            transformedDataTbl.Columns.Add("Level1_Index", Type.GetType("System.Int16"));
            transformedDataTbl.Columns.Add("Level2_Text");
            transformedDataTbl.Columns.Add("Level2_Index", Type.GetType("System.Int16"));
            transformedDataTbl.Columns.Add("Level3_Text");
            transformedDataTbl.Columns.Add("Level3_Index", Type.GetType("System.Int16"));
            transformedDataTbl.Columns.Add("PubDate");
            transformedDataTbl.Columns.Add("Tracking_Box");
            transformedDataTbl.Columns.Add("Received_Date");
            transformedDataTbl.Columns.Add("Disposition_Advice");

            columnname_to_input = new Dictionary<string, string>();

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                fileTextBox.BorderStyle = BorderStyle.FixedSingle;
                bibIdTextBox.BorderStyle = BorderStyle.FixedSingle;
                sheetComboBox.FlatStyle = FlatStyle.Flat;
                vidComboBox.FlatStyle = FlatStyle.Flat;
                browseButton.FlatStyle = FlatStyle.Flat;
                btnShowData.FlatStyle = FlatStyle.Flat;
                viewOnlineButton.FlatStyle = FlatStyle.Flat;
                visibilityComboBox.FlatStyle = FlatStyle.Flat;
            }
        }

        #region Method to draw the background

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

        private void fileTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            Browse_Source();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            Browse_Source();
        }

        private void Browse_Source()
        {
            fileTextBox.Text = String.Empty;
            filename = String.Empty;
            sheetComboBox.Items.Clear();
            sheetComboBox.Text = String.Empty;
            sheetComboBox.Hide();
            sheetLabel.Hide();
            btnShowData.Hide();
            executeButton.Button_Enabled = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Write the filename to the text box first
                fileTextBox.Text = openFileDialog1.FileName;
                filename = openFileDialog1.FileName;

                // Determine the filetype
                FileInfo fileInfo = new FileInfo(filename);
                string extension = fileInfo.Extension.ToUpper();
                if ((extension == ".XLS") || (extension == ".XLSX"))
                {
                    ExcelBibliographicReader read = new ExcelBibliographicReader();

                    try
                    {
                        // Try getting the worksheet names from the selected workbook
                        // Get the sheet names
                        read = new ExcelBibliographicReader();
                        List<string> tables = read.GetExcelSheetNames(openFileDialog1.FileName);


                        if (tables == null)
                        {
                            MessageBox.Show("Unable to read the source workbook.   ");
                            fileTextBox.Text = String.Empty;
                            filename = String.Empty;
                        }
                        else
                        {

                            // Populate the combo box
                            foreach (string thisSheetName in tables)
                                sheetComboBox.Items.Add(thisSheetName);

                            sheetComboBox.Show();
                            sheetLabel.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMessageBox.Show(ex.Message, "Unexpected Error", ex);
                        fileTextBox.Text = String.Empty;
                        filename = String.Empty;
                    }
                    finally
                    {
                        // Close the reader
                        read.Close();
                    }
                    return;
                }

                // Check with CSV or TXT files
                if ((extension == ".CSV") || (extension == ".TXT"))
                {
                    // Create the splitter to use
                    string splitter = ",";
                    if (extension == ".TXT")
                        splitter = ",\t";
                    char[] splitter_chars = splitter.ToCharArray();

                    StreamReader text_reader = new StreamReader(filename);

                    try
                    {
                        string line = text_reader.ReadLine();
                        if (line != null)
                        {
                            string[] split = line.Split(splitter_chars);
                            int columns = 0;
                            rawDataTbl = new DataTable();
                            columnname_to_input.Clear();
                            foreach (string topRowSplit in split)
                            {
                                if (topRowSplit.Length > 0)
                                {
                                    columns++;
                                    rawDataTbl.Columns.Add(topRowSplit.ToUpper().Trim().Replace(" ", ""));
                                    columnname_to_input[topRowSplit.ToUpper().Trim().Replace(" ", "")] =
                                        topRowSplit.Trim();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            line = text_reader.ReadLine();
                            while (line != null)
                            {
                                split = line.Split(splitter_chars);
                                DataRow newRow = rawDataTbl.NewRow();
                                for (int i = 0; (i < columns) && (i < split.Length); i++)
                                {
                                    newRow[i] = split[i];
                                }
                                rawDataTbl.Rows.Add(newRow);
                                line = text_reader.ReadLine();
                            }
                        }
                        btnShowData.Location = new Point(73,67);
                        btnShowData.Show();
                        if (bibidLabel.Visible)
                            executeButton.Button_Enabled = true;
                    }
                    catch
                    {
                        MessageBox.Show("Input text file is in improper format");
                        fileTextBox.Text = String.Empty;
                        filename = String.Empty;
                    }

                    text_reader.Close();
                    return;
                }

                // Type was invalid so just return
                fileTextBox.Text = String.Empty;
                filename = String.Empty;
            }
        }            

        private void sheetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExcelBibliographicReader read = new ExcelBibliographicReader();

            try
            {
                // Make sure there is a filename and a sheet name
                if ((fileTextBox.Text.Length > 0) && (sheetComboBox.SelectedIndex >= 0))
                {
                    read.Sheet = sheetComboBox.Text;
                    read.Filename = fileTextBox.Text;

                    // Try reading data from the selected Excel Worksheet
                    if (!read.Check_Source())
                    {
                        MessageBox.Show("Unable to read the source sheet   ");
                        fileTextBox.Text = String.Empty;
                        filename = String.Empty;
                        sheetComboBox.Items.Clear();
                        sheetComboBox.Text = String.Empty;
                        sheetComboBox.Hide();
                        sheetLabel.Hide();
                        executeButton.Button_Enabled = false;
                    }
                    else
                    {
                        rawDataTbl = read.excelData;
                        columnname_to_input.Clear();
                        foreach (DataColumn thisColumn in rawDataTbl.Columns)
                        {
                            columnname_to_input[thisColumn.ColumnName.ToUpper().Trim()] = thisColumn.ColumnName.Trim();
                            thisColumn.ColumnName = thisColumn.ColumnName.ToUpper().Trim().Replace(" ","");
                        }

                        if (!Transform_Data())
                        {
                            fileTextBox.Text = String.Empty;
                            filename = String.Empty;
                            sheetComboBox.Items.Clear();
                            sheetComboBox.Text = String.Empty;
                            sheetComboBox.Hide();
                            sheetLabel.Hide();
                            executeButton.Button_Enabled = false;
                        }
                        else
                        {

                            // Check to see if multiple bibids loaded
                            string check_bib = transformedDataTbl.Rows[0][0].ToString().ToUpper();
                            bool multiple_bibs_in_table = transformedDataTbl.Rows.Cast<DataRow>().Any(thisRow => thisRow[0].ToString().ToUpper() != check_bib);

                            btnShowData.Location = new Point(239, 66);
                            btnShowData.Show();
                            if ((bibidLabel.Visible) || ( multiple_bibs_in_table ))
                                executeButton.Button_Enabled = true;
                        }


                    }

                    // Close the reader
                    read.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorMessageBox.Show(ex.Message, "Unexpected Error", ex);
                fileTextBox.Text = String.Empty;
                filename = String.Empty;
                sheetComboBox.Items.Clear();
                sheetComboBox.Text = String.Empty;
                sheetComboBox.Hide();
                sheetLabel.Hide();
                executeButton.Button_Enabled = false;
            }
            finally
            {
                // Close the reader
                read.Close();
            }
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            // Make sure there are rows in the data table
            if (transformedDataTbl.Rows.Count >= 0)
            {
                DataGridForm showData = new DataGridForm(transformedDataTbl);
                showData.ShowDialog();
            }
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void executeButton_Button_Pressed(object sender, EventArgs e)
        {
            // Make sure some rows of data are ready to be imported
            if (transformedDataTbl.Rows.Count == 0)
            {
                MessageBox.Show("No data rows to import!     ", "Import Aborted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check to see if multiple bibids loaded
            string check_bib = transformedDataTbl.Rows[0][0].ToString().ToUpper();
            bool multiple_bibs_in_table = transformedDataTbl.Rows.Cast<DataRow>().Any(thisRow => thisRow[0].ToString().ToUpper() != check_bib);

            // Create a temporary folder
            string temp_folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SMaRT\\Temporary";
            try
            {
                if (!Directory.Exists(temp_folder))
                {
                    Directory.CreateDirectory(temp_folder);
                }
            }
            catch
            {
            }


            string bibid = bibidLabel.Text;
            string vid = vidComboBox.Text;
            int visibility_index = visibilityComboBox.SelectedIndex;
            bool dark = false;
            int visibility = 0;
            switch (visibility_index)
            {
                case 0:
                    dark = true;
                    visibility = -1;
                    break;

                case 1:
                    visibility = -1;
                    break;

                case 2:
                    visibility = 1;
                    break;

                case 3:
                    visibility = 0;
                    break;
            }

            // Look for the online directory
            string mets_file = String.Empty;
            if (!multiple_bibs_in_table)
            {
                string online_folder = Library.SobekCM_Library_Settings.Image_Server_Network + "\\" + bibid.Substring(0, 2) + "\\" + bibid.Substring(2, 2) + "\\" + bibid.Substring(4, 2) + "\\" + bibid.Substring(6, 2) + "\\" + bibid.Substring(8) + "\\" + vid;
                if (!Directory.Exists(online_folder))
                {
                    MessageBox.Show("Select a different VID, since this VID is not yet fully loaded into " + Library.SobekCM_Library_Settings.System_Abbreviation + ".    \n\nDirectory not present online.", "VID not loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Look for the online METS
                mets_file = online_folder + "\\" + bibid + "_" + vid + ".mets.xml";
                if (!File.Exists(mets_file))
                {
                    MessageBox.Show("Select a different VID, since this VID is not yet fully loaded into " + Library.SobekCM_Library_Settings.System_Abbreviation + ".       \n\nMETS file missing online.", "VID not loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Try to copy this to a local temporary folder for speed purposes
                try
                {
                    File.Copy(mets_file, temp_folder + "\\" + bibid + "_" + vid + ".mets.xml", true);
                    mets_file = temp_folder + "\\" + bibid + "_" + vid + ".mets.xml";
                }
                catch
                {
                }
            }

            // Display an hourglass cursor and set max value on the ProgressBar
            Cursor = Cursors.WaitCursor;
            progressBar1.Maximum = transformedDataTbl.Rows.Count;
            
            // enable the Stop button
            executeButton.Button_Enabled = true;

            // Show the progress bar
            progressBar1.Visible = true;
            progressBar1.Value = progressBar1.Minimum;
            
            try
            {
                // Create the Processor and assign the Delegate method for event processing.
                processor = !multiple_bibs_in_table ? new Volume_Import_Processor(transformedDataTbl, bibid, mets_file, temp_folder, visibility, dark) : new Volume_Import_Processor(transformedDataTbl, String.Empty, String.Empty, temp_folder, visibility, dark);

                processor.New_Progress += processor_New_Progress;
                processor.Complete += processor_Complete;

                // Create the thread to do the processing work, and start it.            
                processThread = new Thread(processor.Go);
                processThread.SetApartmentState(ApartmentState.STA);    
                processThread.Start();
            }
            catch (Exception ee)
            {
                // display the error message
                ErrorMessageBox.Show("Error encountered while processing!\n\n" + ee.Message, "Volume Import Error", ee);
                                    
                Cursor = Cursors.Default;
                progressBar1.Value = progressBar1.Minimum;
            } 
        }


        void processor_Complete(int New_Progress)
        {
            //// Check to see if Processor thread should be stopped
            //if (this.processor != null)
            //{
            //    try
            //    {
            //        // terminate the Processor thread
            //        processThread.Abort();
            //        processThread.Join();
            //        processor = null;
            //    }
            //    catch (System.Threading.ThreadAbortException)
            //    {

            //    }
            //    catch (Exception ee)
            //    {
            //        MessageBox.Show("ABORT EXCEPTION\n\n" + ee.ToString());
            //    }
            //}

            Cursor = Cursors.Default;

            //DataGridForm showData = new DataGridForm(transformedDataTbl);
            //showData.ShowDialog();


            Close();
        }

        void processor_New_Progress(int New_Progress)
        {
            // Just increment the progress bar
            progressBar1.Value = New_Progress;
        }

        private void viewOnlineButton_Click(object sender, EventArgs e)
        {

        }

        private void bibIdTextBox_Leave(object sender, EventArgs e)
        {
            if (bibIdTextBox.Text.Trim().Length == 10)
                check_for_bib();
            else
                clear_bib_info();
        }

        private void bibIdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (bibIdTextBox.Text.Trim().Length == 10)
                check_for_bib();
            else
                clear_bib_info();
        }

        private void check_for_bib()
        {
            DataSet bibInfo = SobekCM_Database.Get_Item_Details(bibIdTextBox.Text.Trim(), String.Empty, null);
            if ((bibInfo != null) && ( bibInfo.Tables[0].Rows.Count > 0 ))
            {
                titleLabel.Text = bibInfo.Tables[0].Rows[0]["GroupTitle"].ToString();
                vidComboBox.Items.Clear();
                DataColumn vidColumn = bibInfo.Tables[1].Columns["VID"];
                foreach (DataRow vidRow in bibInfo.Tables[1].Rows)
                {
                    vidComboBox.Items.Add(vidRow[vidColumn].ToString());
                }
                vidComboBox.SelectedIndex = vidComboBox.Items.Count - 1;
                bibidLabel.Text = bibIdTextBox.Text.Trim();
                executeButton.Button_Enabled = true;
            }
            else
            {
                vidComboBox.Items.Clear();
                titleLabel.Text = String.Empty;
                executeButton.Button_Enabled = false;
            }
        }

        private void clear_bib_info()
        {
            vidComboBox.Items.Clear();
            titleLabel.Text = String.Empty;
            executeButton.Button_Enabled = false;
        }

        private string Try_Convert_To_Month( string Possible_Month )
        {
            switch (Possible_Month)
            {
                case "1":
                    return "January";

                case "2":
                    return "February";

                case "3":
                    return "March";

                case "4":
                    return "April";

                case "5":
                    return "May";

                case "6":
                    return "June";

                case "7":
                    return "July";

                case "8":
                    return "August";

                case "9":
                    return "September";

                case "10":
                    return "October";

                case "11":
                    return "November";

                case "12":
                    return "December";

                default:
                    return Possible_Month;
            }
        }

        private string Convert_to_Month(int Month)
        {
            switch (Month)
            {
                case 1:
                    return "January";

                case 2:
                    return "February";

                case 3:
                    return "March";

                case 4:
                    return "April";

                case 5:
                    return "May";

                case 6:
                    return "June";

                case 7:
                    return "July";

                case 8:
                    return "August";

                case 9:
                    return "September";

                case 10:
                    return "October";

                case 11:
                    return "November";

                case 12:
                    return "December";

                default:
                    return "ERROR";
            }
        }

        private bool Transform_Data()
        {

            transformedDataTbl.Clear();

            try
            {
                // Find the title column
                DataColumn titleColumn = null;
                if (rawDataTbl.Columns.Contains("TITLE"))
                    titleColumn = rawDataTbl.Columns["TITLE"];

                // Find the bibid column
                DataColumn bibColumn = null;
                if (rawDataTbl.Columns.Contains("BIBID"))
                    bibColumn = rawDataTbl.Columns["BIBID"];

                // Find the material received column
                string[] materialRecdColumnNames = new[] { "MATERIALSRECEIVED", "MATERIALRECEIVED", "RECEIVEDDATE", "MATERIALRECDDATE", "RECDDATE", "MATERIALRECEIVEDDATE", "MATERIALSRECEIVEDDATE" };
                DataColumn materialRecdColumn = (from possibleName in materialRecdColumnNames where rawDataTbl.Columns.Contains(possibleName) select rawDataTbl.Columns[possibleName]).FirstOrDefault();

                // Find the tracking box column
                DataColumn trackingBoxColumn = null;
                if (rawDataTbl.Columns.Contains("TRACKINGBOX"))
                {
                    trackingBoxColumn = rawDataTbl.Columns["TRACKINGBOX"];
                }

                // Find the disposition advice column
                DataColumn dispositionAdviceColumn = null;
                if (rawDataTbl.Columns.Contains("DISPOSITIONADVICE"))
                {
                    dispositionAdviceColumn = rawDataTbl.Columns["DISPOSITIONADVICE"];
                }

                // Is there a DATE column?  Special code for that
                if (rawDataTbl.Columns.Contains("DATE"))
                {
                    int error = 0;
                    foreach (DataRow thisRow in rawDataTbl.Rows)
                    {
                        DataRow newRow = transformedDataTbl.NewRow();
                        newRow[0] = bibidLabel.Text;
                        if (bibColumn != null)
                            newRow[0] = thisRow[bibColumn];
                        if (titleColumn != null)
                            newRow[2] = thisRow[titleColumn];
                        if (trackingBoxColumn != null)
                            newRow[10] = thisRow[trackingBoxColumn];
                        if (materialRecdColumn != null)
                            newRow[11] = thisRow[materialRecdColumn];
                        if (dispositionAdviceColumn != null)
                            newRow[12] = thisRow[dispositionAdviceColumn];
                        try
                        {
                            DateTime dateTime = Convert.ToDateTime(thisRow["DATE"]);
                            newRow[3] = dateTime.Year;
                            newRow[4] = dateTime.Year;
                            newRow[5] = Convert_to_Month(dateTime.Month);
                            newRow[6] = dateTime.Month;
                            newRow[7] = dateTime.Day;
                            newRow[8] = dateTime.Day;
                            newRow[9] = dateTime.ToLongDateString();
                            transformedDataTbl.Rows.Add(newRow);
                        }
                        catch
                        {
                            error++;
                        }

                        if (error >= 5)
                        {
                            MessageBox.Show("Unable to convert multiple entries in DATE column to date type.         ", "Error in casting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
                else
                {

                    DataColumn firstTextColumn = null;
                    DataColumn firstIndexColumn = null;
                    DataColumn secondTextColumn = null;
                    DataColumn secondIndexColumn = null;
                    DataColumn thirdTextColumn = null;
                    DataColumn thirdIndexColumn = null;
                    DataColumn pubDateColumn = null;
                    string firstTextColumnPrefix = String.Empty;
                    string secondTextColumnPrefix = String.Empty;
                    string thirdTextColumnPrefix = String.Empty;

                    if (rawDataTbl.Columns.Contains("PUBDATE"))
                        pubDateColumn = rawDataTbl.Columns["PUBDATE"];

                    if ((rawDataTbl.Columns.Contains("LEVEL1")) || (rawDataTbl.Columns.Contains("LEVEL1TEXT")) || (rawDataTbl.Columns.Contains("LEVEL1_TEXT")))
                    {
                        // Set the level 1 text column 
                        if (rawDataTbl.Columns.Contains("LEVEL1"))
                        {
                            firstTextColumn = rawDataTbl.Columns["LEVEL1"];
                        }
                        if ((firstTextColumn == null) && (rawDataTbl.Columns.Contains("LEVEL1TEXT")))
                        {
                            firstTextColumn = rawDataTbl.Columns["LEVEL1TEXT"];
                        }
                        if ((firstTextColumn == null) && (rawDataTbl.Columns.Contains("LEVEL1_TEXT")))
                        {
                            firstTextColumn = rawDataTbl.Columns["LEVEL1_TEXT"];
                        }

                        // Set the level 1 index column 
                        if (rawDataTbl.Columns.Contains("LEVEL1INDEX"))
                        {
                            firstIndexColumn = rawDataTbl.Columns["LEVEL1INDEX"];
                        }
                        if ((firstIndexColumn == null) && (rawDataTbl.Columns.Contains("LEVEL1_INDEX")))
                        {
                            firstIndexColumn = rawDataTbl.Columns["LEVEL1_INDEX"];
                        }
                        if (firstIndexColumn != null)
                        {
                            firstIndexColumn = firstTextColumn;
                        }

                        // Set the level 2 text column 
                        if (rawDataTbl.Columns.Contains("LEVEL2"))
                        {
                            secondTextColumn = rawDataTbl.Columns["LEVEL2"];
                        }
                        if ((secondTextColumn == null) && (rawDataTbl.Columns.Contains("LEVEL2TEXT")))
                        {
                            secondTextColumn = rawDataTbl.Columns["LEVEL2TEXT"];
                        }
                        if ((secondTextColumn == null) && (rawDataTbl.Columns.Contains("LEVEL2_TEXT")))
                        {
                            secondTextColumn = rawDataTbl.Columns["LEVEL2_TEXT"];
                        }

                        // Set the level 2 index column 
                        if (rawDataTbl.Columns.Contains("LEVEL2INDEX"))
                        {
                            secondIndexColumn = rawDataTbl.Columns["LEVEL2INDEX"];
                        }
                        if ((secondIndexColumn == null) && (rawDataTbl.Columns.Contains("LEVEL2_INDEX")))
                        {
                            secondIndexColumn = rawDataTbl.Columns["LEVEL2_INDEX"];
                        }
                        if (secondIndexColumn != null)
                        {
                            secondIndexColumn = secondTextColumn;
                        }

                        // Set the level 3 text column 
                        if (rawDataTbl.Columns.Contains("LEVEL3"))
                        {
                            thirdTextColumn = rawDataTbl.Columns["LEVEL3"];
                        }
                        if ((thirdTextColumn == null) && (rawDataTbl.Columns.Contains("LEVEL3TEXT")))
                        {
                            thirdTextColumn = rawDataTbl.Columns["LEVEL3TEXT"];
                        }
                        if ((thirdTextColumn == null) && (rawDataTbl.Columns.Contains("LEVEL3_TEXT")))
                        {
                            thirdTextColumn = rawDataTbl.Columns["LEVEL3_TEXT"];
                        }

                        // Set the level 3 index column 
                        if (rawDataTbl.Columns.Contains("LEVEL3INDEX"))
                        {
                            thirdIndexColumn = rawDataTbl.Columns["LEVEL3INDEX"];
                        }
                        if ((thirdIndexColumn == null) && (rawDataTbl.Columns.Contains("LEVEL3_INDEX")))
                        {
                            thirdIndexColumn = rawDataTbl.Columns["LEVEL3_INDEX"];
                        }
                        if (thirdIndexColumn != null)
                        {
                            thirdIndexColumn = thirdTextColumn;
                        }
                    }
                    else
                    {
                        int adjusted_column_counter = 0;
                        while ((adjusted_column_counter < rawDataTbl.Columns.Count) && ((rawDataTbl.Columns[adjusted_column_counter].ColumnName == "BIBID") || (rawDataTbl.Columns[adjusted_column_counter].ColumnName == "TITLE")))
                            adjusted_column_counter++;

                        firstTextColumn = rawDataTbl.Columns[adjusted_column_counter];
                        firstIndexColumn = rawDataTbl.Columns[adjusted_column_counter];
                        string columnName = rawDataTbl.Columns[adjusted_column_counter].ColumnName.Trim();
                        if ((columnName != "YEAR") && (columnName != "MONTH") && (columnName != "DAY"))
                            firstTextColumnPrefix = columnname_to_input[columnName] + " ";

                        if (rawDataTbl.Columns.Count > adjusted_column_counter + 1)
                        {
                            secondTextColumn = rawDataTbl.Columns[adjusted_column_counter + 1];
                            secondIndexColumn = rawDataTbl.Columns[adjusted_column_counter + 1];
                            columnName = rawDataTbl.Columns[adjusted_column_counter + 1].ColumnName.Trim();
                            if ((columnName != "YEAR") && (columnName != "MONTH") && (columnName != "DAY"))
                                secondTextColumnPrefix = columnname_to_input[columnName] + " ";
                        }
                        if (rawDataTbl.Columns.Count > adjusted_column_counter + 2)
                        {
                            thirdTextColumn = rawDataTbl.Columns[adjusted_column_counter + 2];
                            thirdIndexColumn = rawDataTbl.Columns[adjusted_column_counter + 2];
                            columnName = rawDataTbl.Columns[adjusted_column_counter + 2].ColumnName.Trim();
                            if ((columnName != "YEAR") && (columnName != "MONTH") && (columnName != "DAY"))
                                thirdTextColumnPrefix = columnname_to_input[columnName] + " ";
                        }
                    }

                    // Now, step through and add each transformed row
                    foreach (DataRow thisRow in rawDataTbl.Rows)
                    {
                        string year = String.Empty;
                        string month = String.Empty;
                        string day = String.Empty;
                        DataRow newRow = transformedDataTbl.NewRow();
                        newRow[0] = bibidLabel.Text;
                        if (bibColumn != null)
                            newRow[0] = thisRow[bibColumn];
                        if (titleColumn != null)
                            newRow[2] = thisRow[titleColumn];
                        if (trackingBoxColumn != null)
                            newRow[10] = thisRow[trackingBoxColumn];
                        if (materialRecdColumn != null)
                            newRow[11] = thisRow[materialRecdColumn];
                        if (dispositionAdviceColumn != null)
                            newRow[12] = thisRow[dispositionAdviceColumn];
                        newRow[3] = firstTextColumnPrefix + thisRow[firstTextColumn];
                        newRow[4] = To_Number(thisRow[firstIndexColumn].ToString());
                        if (firstTextColumn.ColumnName == "YEAR")
                            year = thisRow[firstTextColumn].ToString();

                        if (secondTextColumn != null)
                        {
                            if ((secondTextColumn.ColumnName == "MONTH") && (secondTextColumnPrefix.Length == 0))
                            {
                                string month_as_text = Try_Convert_To_Month(thisRow[secondTextColumn].ToString());
                                newRow[5] = month_as_text;
                                month = month_as_text;
                            }
                            else
                            {
                                newRow[5] = secondTextColumnPrefix + thisRow[secondTextColumn];
                                if (secondTextColumn.ColumnName == "YEAR")
                                    year = thisRow[secondTextColumn].ToString();
                            }
                            newRow[6] = To_Number(thisRow[secondIndexColumn].ToString());

                            if (thirdTextColumn != null)
                            {
                                if ((thirdTextColumn.ColumnName == "MONTH") && (thirdTextColumnPrefix.Length == 0))
                                {
                                    string month_as_text = Try_Convert_To_Month(thisRow[thirdTextColumn].ToString());
                                    newRow[7] = month_as_text;
                                    month = month_as_text;
                                }
                                else
                                {
                                    if (thirdTextColumn.ColumnName == "YEAR")
                                        year = thisRow[thirdTextColumn].ToString();
                                    if (thirdTextColumn.ColumnName == "DAY")
                                        day = thisRow[thirdTextColumn].ToString();
                                    newRow[7] = thirdTextColumnPrefix + thisRow[thirdTextColumn];
                                }
                                newRow[8] = To_Number(thisRow[thirdIndexColumn].ToString());
                            }
                            else
                            {
                                newRow[7] = String.Empty;
                                newRow[8] = -1;
                            }
                        }
                        else
                        {
                            newRow[5] = String.Empty;
                            newRow[6] = -1;
                            newRow[7] = String.Empty;
                            newRow[8] = -1;
                        }

                        if (pubDateColumn != null)
                            newRow[9] = thisRow[pubDateColumn];
                        else
                        {
                            if (year.Length > 0)
                            {
                                if (month.Length > 0)
                                {
                                    if (day.Length > 0)
                                    {
                                        newRow[9] = month + " " + day + ", " + year;
                                    }
                                    else
                                    {
                                        newRow[9] = month + " " + year;
                                    }
                                }
                                else
                                {
                                    newRow[9] = year;
                                }
                            }
                            else
                            {
                                newRow[9] = String.Empty;
                            }
                        }

                        transformedDataTbl.Rows.Add(newRow);
                    }
                }

                return true;
            }
            catch 
            {
                MessageBox.Show("Error transforming provided spreadsheet to correct format for import.   ", "Error Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private short To_Number(string Possible_Number)
        {
           
            try
            {
                Possible_Number = Possible_Number.Trim();

                if ( Possible_Number.Length == 0 )
                    return -1;

                // Determine if this is already a number
                bool allNumber = Possible_Number.All(Char.IsNumber);

                if (allNumber)
                {
                    return Convert.ToInt16(Possible_Number);
                }

                short current_number = Possible_Number.Where(Char.IsNumber).Aggregate<char, short>(0, (current, thisChar) => (short) ((current*10) + Convert.ToInt32(thisChar)));

                if (current_number > 0)
                    return current_number;

                // Check for months
                Possible_Number = Possible_Number.ToUpper();
                switch (Possible_Number)
                {
                    case "JANUARY":
                        return 1;

                    case "FEBRUARY":
                        return 2;

                    case "MARCH":
                        return 3;

                    case "APRIL":
                        return 4;

                    case "MAY":
                        return 5;

                    case "JUNE":
                        return 6;

                    case "JULY":
                        return 7;

                    case "AUGUST":
                        return 8;

                    case "SEPTEMBER":
                        return 9;

                    case "OCTOBER":
                        return 10;

                    case "NOVEMEBER":
                        return 11;

                    case "DECEMBER":
                        return 12;
                }

                return -1;
            }
            catch
            {
                return -1;
            }
        }


    }
}
