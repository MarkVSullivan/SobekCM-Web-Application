using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using SobekCM.Resource_Object;
using GemBox.Spreadsheet;

namespace SobekCM.Management_Tool.Importer.Forms
{
    public partial class SpreadSheet_Importer_Form : Form
    {
        private SpreadSheet_Importer_Processor processor;
        protected Thread processThread;
        private DataTable excelDataTbl;
        private List<Column_Assignment_Control> column_map_inputs;
        private List<Constant_Assignment_Control> constant_map_inputs;
        private string filename = String.Empty;

        private string tickler;


        #region Constructor
        public SpreadSheet_Importer_Form()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
            tickler = String.Empty;

            column_map_inputs = new List<Column_Assignment_Control>();
            constant_map_inputs = new List<Constant_Assignment_Control>();

            ResetFormControls();

            // Perform some additional work if this was not XP theme
            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                browseButton.FlatStyle = FlatStyle.Flat;
                sheetComboBox.FlatStyle = FlatStyle.Flat;
                btnShowData.FlatStyle = FlatStyle.Flat;
                previewCheckBox.FlatStyle = FlatStyle.Flat;
            }
        }
        #endregion

        #region Class Events 
        
        /// <summary> Method is called whenever this form is resized. </summary>
        /// <param name="e"></param>
        /// <remarks> This redraws the background of this form </remarks>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Get rid of any current background image
            if (this.BackgroundImage != null)
            {
                this.BackgroundImage.Dispose();
                this.BackgroundImage = null;
            }

            if (this.ClientSize.Width > 0)
            {
                // Create the items needed to draw the background
                Bitmap image = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                Graphics gr = Graphics.FromImage(image);
                Rectangle rect = new Rectangle(new Point(0, 0), this.ClientSize);

                // Create the brush
                LinearGradientBrush brush = new LinearGradientBrush(rect, SystemColors.Control, ControlPaint.Dark(SystemColors.Control), LinearGradientMode.Vertical);
                brush.SetBlendTriangularShape(0.33F);

                // Create the image
                gr.FillRectangle(brush, rect);
                gr.Dispose();

                // Set this as the backgroundf
                this.BackgroundImage = image;
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
                    // reset the status label
                    labelStatus.Text = "";

                    read.Sheet = sheetComboBox.Text;
                    read.Filename = fileTextBox.Text;

                    // Declare constant fields
                    Constant_Fields constants = new Constant_Fields();

                    columnNamePanel.Controls.Clear();
                    column_map_inputs.Clear();

                    pnlConstants.Controls.Clear();
                    constant_map_inputs.Clear();

                    columnNamePanel.Enabled = true;
                    pnlConstants.Enabled = true;

                    // Display an hourglass cursor:
                    this.Cursor = Cursors.WaitCursor;                         


                    // Try reading data from the selected Excel Worksheet
                    bool readFlag = true;

                    while (readFlag)
                    {

                        try
                        {
                            if (!read.Check_Source())
                            {
                                ResetFormControls();
                                return;
                            }
                            else
                            {
                                readFlag = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            DLC.Tools.Forms.ErrorMessageBox.Show(ex.Message, "Unexpected Error", ex);
                        }
                    }
                   

                    // change cursor back to default
                    this.Cursor = Cursors.Default;

                    this.excelDataTbl = read.excelData;

                    if (read.excelData.Rows.Count > 0 || read.excelData.Columns.Count > 0)
                    {

                        int column_counter = 1;
                        foreach (DataColumn thisColumn in excelDataTbl.Columns)
                        {

                            // Create the column mapping custom control
                            Column_Assignment_Control thisColControl = new Column_Assignment_Control();

                            // Get the column name
                            string thisColumnName = thisColumn.ColumnName;
                            thisColControl.Column_Name = thisColumnName;
                            if (thisColumnName == "F" + column_counter)
                            {
                                thisColControl.Empty = true;
                            }

                            thisColControl.Location = new Point(10, 10 + ((column_counter - 1) * 30));
                            this.columnNamePanel.Controls.Add(thisColControl);
                            this.column_map_inputs.Add(thisColControl);

                            // Select value in list control that matches to a Column Name
                            thisColControl.Select_List_Item(thisColumnName);

                            // Increment for the next column
                            column_counter++;
                        }


                        // Create the constants mapping custom control
                        // Add eight constant user controls to panel
                        for (int i = 1; i < 9; i++)
                        {
                            Constant_Assignment_Control thisConstantCtrl = new Constant_Assignment_Control();
                            thisConstantCtrl.Location = new Point(10, 10 + ((i - 1) * 30));
                            this.pnlConstants.Controls.Add(thisConstantCtrl);
                            this.constant_map_inputs.Add(thisConstantCtrl);
                        }

                        // set some of the constant columns to required tracking fields
                        constant_map_inputs[0].Mapped_Name = "Material Type";
                        constant_map_inputs[1].Mapped_Name = "Aggregation Code";
                        constant_map_inputs[2].Mapped_Name = "Visibility";
                        constant_map_inputs[3].Mapped_Name = "Tickler";
                        FileInfo fileInfo = new FileInfo(fileTextBox.Text);
                        constant_map_inputs[3].Mapped_Constant = fileInfo.Name.Replace(fileInfo.Extension,"");

                        // Move to STEP 3
                        show_step_3();

                        if (column_map_inputs.Count > 0)
                            // Move to STEP 4
                            show_step_4();                       
                    }


                    // Close the reader
                    read.Close();

                }

            }
            catch (Exception ex)
            {
                DLC.Tools.Forms.ErrorMessageBox.Show(ex.Message, "Unexpected Error", ex);
            }
            finally
            {
                // change cursor back to default
                this.Cursor = Cursors.Default;

                // Close the reader
                read.Close();
            }
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            // Make sure there are rows in the data table
            if (this.excelDataTbl.Rows.Count >= 0)
            {
                DataGridForm showData = new DataGridForm(excelDataTbl);
                showData.ShowDialog();
            }
        }

        private void MainForm_FormClosed(Object sender, FormClosedEventArgs e)
        {
            // The closed event for this form has been invoked.
            // Check if the Processor thread is running.  If the
            // thread is running, idle the current thread so that the 
            // Processor delegate function can write data to 
            // an Excel worksheet.

            try
            {
                // check if the Processor thread is running
                if ((processThread != null) && (processThread.ThreadState == ThreadState.Running))
                {
                    // set flag to indicate that the Process thread will be
                    // aborted tbe next time through the delegate method - 
                    // processor_Volume_Processed()

                    this.processor.StopThread = true;
                    Thread.Sleep(100);
                }
            }
            catch { }
        }

        private void browseButton_Click(object sender, System.EventArgs e)
        {
            Browse_Source();
        }

        private void sourceMenuItem_Click(object sender, System.EventArgs e)
        {
            Browse_Source();
        }

        private void directoryTextBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Browse_Source();
        }
       
        private void exitMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
   
        #endregion

        #region Class Methods

        private void Browse_Source()
        {
            // reset the status label
            labelStatus.Text = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                ExcelBibliographicReader read = new ExcelBibliographicReader();
                List<string> tables;

                try
                {
                    // Set form controls to default state   
                    ResetFormControls();

                    // Write the filename to the text box first
                    this.fileTextBox.Text = openFileDialog1.FileName;
                    this.filename = openFileDialog1.FileName;


                    // Try getting the worksheet names from the selected workbook
                    bool readFlag = true;

                    while (readFlag)
                    {

                        try
                        {

                            // Get the sheet names
                            read = new ExcelBibliographicReader();
                            tables = read.GetExcelSheetNames(openFileDialog1.FileName);


                            if (tables == null)
                            {
                                ResetFormControls();
                                return;
                            }
                            else
                            {
                                readFlag = false;
                               
                                // Populate the combo box
                                this.sheetComboBox.Enabled = true;
                                foreach (string thisSheetName in tables)
                                    this.sheetComboBox.Items.Add(thisSheetName);

                                // show step 2 instructions
                                show_step_2();
                            }
                        }
                        catch (Exception ex)
                        {
                            DLC.Tools.Forms.ErrorMessageBox.Show(ex.Message, "Unexpected Error", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DLC.Tools.Forms.ErrorMessageBox.Show(ex.Message, "Unexpected Error", ex);
                }
                finally
                {
                    // Close the reader
                    read.Close();
                }
            }
            else
            {
                // reset the form
                this.ResetFormControls();

                // Move to STEP 1
                this.show_step_1();
            }
        }                 

        /// <summary> Imports the records from the indicated source file </summary>
        protected void Import_Records(System.Data.DataTable inputFile)
        {
            // update class variable
            this.excelDataTbl = inputFile;

            // Display an hourglass cursor and set max value on the ProgressBar
            this.Cursor = Cursors.WaitCursor;
            progressBar1.Maximum = this.Total_Records;
                                   
            // Step through each column map control
            List<SobekCM.Resource_Object.Mapped_Fields> mapping = new List<SobekCM.Resource_Object.Mapped_Fields>();      
            foreach (Column_Assignment_Control thisColumn in column_map_inputs)
            {
                mapping.Add(thisColumn.Mapped_Field);            
            }

            // Step through each constant map control
            Constant_Fields constantCollection = new Constant_Fields();
           
            foreach (Constant_Assignment_Control thisConstant in constant_map_inputs)
            {                
                constantCollection.Add(thisConstant.Mapped_Field, thisConstant.Mapped_Constant);                
            }
                       
            //add columns to the input data table  
            if (!excelDataTbl.Columns.Contains("New BIB ID"))
                excelDataTbl.Columns.Add("New BIB ID");
            else
            {
                excelDataTbl.Columns.Remove("New BIB ID");
                excelDataTbl.Columns.Add("New BIB ID");
            }

            if (!excelDataTbl.Columns.Contains("New VID ID"))
                excelDataTbl.Columns.Add("New VID ID");
            else
            {
                excelDataTbl.Columns.Remove("New VID ID");
                excelDataTbl.Columns.Add("New VID ID");
            }

            if (!excelDataTbl.Columns.Contains("Messages"))
                excelDataTbl.Columns.Add("Messages");
            else
            {
                excelDataTbl.Columns.Remove("Messages");
                excelDataTbl.Columns.Add("Messages");
            }

            // disable some of the form controls
            this.Disable_FormControls();

            // enable the Stop button
            this.executeButton.Button_Enabled = true;

            // Show the progress bar
            this.progressBar1.Visible = true;
            progressBar1.Value = progressBar1.Minimum;

            // reset the status label
            labelStatus.Text = "";

            this.previewCheckBox.Enabled = false;

            // Write the current mappings, etc..
            write_mappings_and_constants( inputFile, mapping, constantCollection);

            try
            {
                // Create the Processor and assign the Delegate method for event processing.
                processor = new SpreadSheet_Importer_Processor(inputFile, mapping, constantCollection, previewCheckBox.Checked );
                processor.New_Progress += new New_Importer_Progress_Delegate(processor_New_Progress);
                processor.Complete += new New_Importer_Progress_Delegate(processor_Complete);

                // Create the thread to do the processing work, and start it.            
                processThread = new Thread(new ThreadStart(processor.Do_Work));
                processThread.SetApartmentState(ApartmentState.STA);    
                processThread.Start();
            }
            catch (Exception e)
            {
                // display the error message
                DLC.Tools.Forms.ErrorMessageBox.Show("Error encountered while processing!\n\n" + e.Message, "DLC Importer Error", e);
                                    
                // enable form controls on the Importer form                    
                this.Enable_FormControls();

                this.Cursor = Cursors.Default;
                progressBar1.Value = progressBar1.Minimum;
            }           
        }

        private void write_mappings_and_constants(DataTable inputFile, List<SobekCM.Resource_Object.Mapped_Fields> mapping, Constant_Fields constantCollection)
        {
            try
            {
                string mapping_name = filename + ".importdata";
                StreamWriter mappingWriter = new StreamWriter(mapping_name, false);
                mappingWriter.WriteLine("MAPPING:");
                int column = 0;
                foreach (SobekCM.Resource_Object.Mapped_Fields mappedField in mapping)
                {
                    mappingWriter.WriteLine("\t\"" + inputFile.Columns[column].ColumnName.Replace("\"", "&quot;") + "\" --> " + SobekCM.Resource_Object.Bibliographic_Mapping.Mapped_Field_To_String(mappedField));
                    column++;
                }
                mappingWriter.WriteLine();
                mappingWriter.WriteLine("CONSTANTS:");
                foreach (Constant_Field_Data constantData in constantCollection.constantCollection )
                {
                    if ((constantData.Data.Length > 0) && (constantData.Field != SobekCM.Resource_Object.Mapped_Fields.None))
                    {
                        mappingWriter.WriteLine("\t" + SobekCM.Resource_Object.Bibliographic_Mapping.Mapped_Field_To_String(constantData.Field) + " <-- \"" + constantData.Data.Replace("\"", "&quot;"));
                    }
                }

                mappingWriter.Flush();
                mappingWriter.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("Unable to save the import data for this job.    \n\n" + ee.ToString(), "Error saving mapping", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void processor_Complete(int New_Progress)
        {
            // Check to see if Processor thread should be stopped
            if ((this.processor != null) && (this.processor.StopThread))
            {
                try
                {
                    // terminate the Processor thread
                    processThread.Abort();
                    processThread.Join();
                    processor = null;
                }

                catch (System.Threading.ThreadAbortException)
                {

                    // A ThreadAbortException has been invoked on the
                    // Processor thread.  Write the import data to an
                    // Excel worksheet and update the form controls only
                    // if the MainForm is not being disposed.

                    // update the status controls on this form
                    if (!this.Disposing)
                    {
                        labelStatus.Text = "Processing stopped at record " + (progressBar1.Value + 1).ToString("#,##0;") + " of " + progressBar1.Maximum.ToString("#,##0;") + " records";
                        progressBar1.Value = progressBar1.Minimum;
                        this.Cursor = Cursors.Default;
                    }

                    try
                    {
                        // Create an Excel Worksheet named 'Output' on the input data file,  
                        // and write the importer results to the spreadsheet.
                        Export_as_Excel();
                    }
                    catch { }
                    finally
                    {
                        // create a table to display the results
                        DataTable displayTbl = this.processor.Report_Data.Copy();

                        // create the Results form             
                        Results_Form showResults = new Results_Form(displayTbl, processor.Importer_Type, previewCheckBox.Checked);

                        // hide the Importer form                  
                        this.Hide();

                        // show the Results form
                        showResults.ShowDialog();

                        // enable form controls on the Importer form                    
                        this.Enable_FormControls();

                        // show the Importer form
                        this.ShowDialog();
                    }
                }
                catch { }

            }
            else
            {
                // The complete flag is true, set the Cursor and ProgressBar back to default values.
                this.Cursor = Cursors.Default;
                progressBar1.Value = progressBar1.Minimum;

                // disable the Stop button
                this.executeButton.Button_Enabled = false;

                try
                {
                    // Create an Excel Worksheet named 'Output' on the input data file,  
                    // and write the importer results to the spreadsheet.
                    Export_as_Excel();
                }
                catch { }
                finally
                {
                    // create a table to display the results
                    DataTable displayTbl = this.processor.Report_Data.Copy();

                    // create the Results form             
                    Results_Form showResults = new Results_Form(displayTbl, processor.Importer_Type, previewCheckBox.Checked);

                    // hide the Importer form                  
                    this.Hide();

                    // show the Results form
                    showResults.ShowDialog();

                    // enable form controls on the Importer form                    
                    this.Enable_FormControls();

                    // show the Importer form
                    this.ShowDialog();
                }
            }    
        }

        void processor_New_Progress(int New_Progress)
        {
            // Just increment the progress bar
            progressBar1.Value = New_Progress % progressBar1.Maximum;


            // update status label
            labelStatus.Text = "Processed " + progressBar1.Value.ToString("#,##0;") + " of " + progressBar1.Maximum.ToString("#,##0;") + " records";
        }

        #region Method to export as excel

        public void Export_as_Excel()
        {
            string SHEET_NAME = "OUTPUT";
            if (previewCheckBox.Checked)
                SHEET_NAME = "PREVIEW";

            // Now, output the data table values to the MS Excel Workbook
            try
            {
                string workbookPath = this.filename;

                // Load the Excel file
                ExcelFile excelFile = new ExcelFile();
                if (this.filename.ToUpper().IndexOf(".XLSX") > 0)
                    excelFile.LoadXlsx(filename, XlsxOptions.None);
                else
                    excelFile.LoadXls(filename);

                // If there is more than one worksheet and there is one with the matching SHEET_NAME
                // then delete that sheet.  This allows for a new OUTPUT or PREVIEW worksheet to 
                // be added, and delete any existing one
                if (excelFile.Worksheets.Count > 1)
                {
                    ExcelWorksheet deleteSheet = null;
                    foreach (ExcelWorksheet thisSheet in excelFile.Worksheets)
                    {
                        if (thisSheet.Name.ToUpper() == SHEET_NAME)
                        {
                            //suppress Excel prompts and delete the 'Output' Worksheet
                            deleteSheet = thisSheet;
                            break;
                        }
                    }
                    if (deleteSheet != null)
                        deleteSheet.Delete();
                }

                // Add a new worksheet
                ExcelWorksheet excelSheet = excelFile.Worksheets.Add(SHEET_NAME);
                excelFile.Worksheets.ActiveWorksheet = excelSheet;

                // Create the header cell style
                CellStyle headerStyle = new CellStyle();
            //    headerStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                headerStyle.FillPattern.SetSolid(Color.Khaki);
                headerStyle.Font.Weight = ExcelFont.BoldWeight;
                headerStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
               

                // Create the new BibID/VID header cell style
                CellStyle headerStyle2 = new CellStyle();
            //    headerStyle2.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                headerStyle2.FillPattern.SetSolid(Color.Gainsboro);
                headerStyle2.Font.Weight = ExcelFont.BoldWeight;
                headerStyle2.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                // Create the new Messages header cell style
                CellStyle headerStyle3 = new CellStyle();
            //    headerStyle3.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                headerStyle3.FillPattern.SetSolid(Color.Tomato);
                headerStyle3.Font.Weight = ExcelFont.BoldWeight;
                headerStyle3.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
                

                // Create the title cell style
                CellStyle titleStyle = new CellStyle();
                titleStyle.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                titleStyle.FillPattern.SetSolid(Color.LightSkyBlue);
                titleStyle.Font.Weight = ExcelFont.BoldWeight;
                titleStyle.Font.Size = 14 * 20;

                // Set the default style
                CellStyle defaultStyle = new CellStyle();
                defaultStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                // Add the title
                excelSheet.Cells[0, 0].Value = SHEET_NAME;
                excelSheet.Cells[0, 0].Style = titleStyle;

                // Add the header values
                for (int i = 0; i < this.excelDataTbl.Columns.Count; i++)
                {
                    excelSheet.Cells[1, i].Value = excelDataTbl.Columns[i].ColumnName.ToUpper();

                    int difference = this.excelDataTbl.Columns.Count - i;
                    if (difference <= 3)
                    {
                        if (difference == 1)
                        {
                            excelSheet.Cells[1, i].Style = headerStyle3;
                        }
                        else
                        {
                            excelSheet.Cells[1, i].Style = headerStyle2;
                        }
                    }
                    else
                    {
                        excelSheet.Cells[1, i].Style = headerStyle;
                    }
                }

                // Add each piece of data
                int rowNumber = 2;
                foreach (System.Data.DataRow thisRow in this.excelDataTbl.Rows)
                {
                    // Add each cell
                    for (int i = 0; i < this.excelDataTbl.Columns.Count; i++)
                    {
                        if (!thisRow[excelDataTbl.Columns[i]].Equals(DBNull.Value))
                            excelSheet.Cells[rowNumber, i].Value = thisRow[excelDataTbl.Columns[i]].ToString();
                        else
                            excelSheet.Cells[rowNumber, i].Value = "";
                        excelSheet.Cells[rowNumber, i].Style = defaultStyle;
                    }

                    // Go to next row
                    rowNumber++;
                }

                // Get the final end range for the columns
                String endRange = String.Empty;
                String bibid_col = String.Empty;
                String vidid_col = String.Empty;
                if (this.excelDataTbl.Columns.Count < 26)
                {
                    int range = (64 + this.excelDataTbl.Columns.Count);
                    endRange = Convert.ToString((char)range);
                    bibid_col = Convert.ToString((char)(range - 2));
                    vidid_col = Convert.ToString((char)(range - 1));
                }
                else if (this.excelDataTbl.Columns.Count == 26)
                {
                    int range = (90);   // ASCII 'Z' character
                    endRange = Convert.ToString((char)range);
                    bibid_col = Convert.ToString((char)(range - 2));
                    vidid_col = Convert.ToString((char)(range - 1));
                }
                else if (this.excelDataTbl.Columns.Count > 26)
                {
                    double column_count = (double)excelDataTbl.Columns.Count;
                    int first_char_ascii = (int)(64 + (Math.Floor(column_count / 26)));
                    int second_char_ascii = (int)(64 + (column_count % 26));

                    // set the end range
                    endRange = Convert.ToString((char)first_char_ascii) + Convert.ToString((char)second_char_ascii);

                    // format the the column header values
                    if (second_char_ascii > Convert.ToInt32('B'))
                    {
                        bibid_col = Convert.ToString((char)first_char_ascii) + Convert.ToString((char)(second_char_ascii - 2));
                        vidid_col = Convert.ToString((char)first_char_ascii) + Convert.ToString((char)(second_char_ascii - 1));
                    }
                    else if (second_char_ascii == Convert.ToInt32('B'))
                    {
                        bibid_col = Convert.ToString((char)(first_char_ascii - 1)) + "Z";
                        vidid_col = Convert.ToString((char)first_char_ascii) + Convert.ToString((char)(second_char_ascii - 1));

                        bibid_col = bibid_col.Replace("@", "");
                    }
                    else
                    {
                        bibid_col = Convert.ToString((char)(first_char_ascii - 1)) + Convert.ToString((char)(Convert.ToInt32('Z') - 1));
                        vidid_col = Convert.ToString((char)(first_char_ascii - 1)) + "Z";

                        bibid_col = bibid_col.Replace("@", "");
                        vidid_col = vidid_col.Replace("@", "");
                    }
                }

                // Set some header properties
                excelSheet.Cells.GetSubrange("A1", endRange + "1").Merged = true;
                excelSheet.Rows[0].Height = 512;
                excelSheet.Rows[1].Height = 512;

                // Set the width on the last columns
                excelSheet.Columns[excelDataTbl.Columns.Count - 3].Width = 18 * 256;
                excelSheet.Columns[excelDataTbl.Columns.Count - 2].Width = 14 * 256;
                excelSheet.Columns[excelDataTbl.Columns.Count - 1].Width = 60 * 256;

                // Set the border
                excelSheet.Cells.GetSubrange("A2", endRange + rowNumber.ToString()).SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                excelSheet.Cells.GetSubrange("A2", endRange + "2").SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                // Save the file
                if (filename.ToUpper().IndexOf(".XLSX") > 0)
                {
                    excelFile.SaveXlsx(filename);
                }
                else
                {
                    excelFile.SaveXls(filename);
                }
            }
            catch (Exception e)
            {
                DLC.Tools.Forms.ErrorMessageBox.Show("Error while saving the Excel Worksheet.\n\n" + e.Message, "Excel Error", e);
            }
        }

        #endregion

        private void ResetFormControls()
        {
            // Set form controls to default state  
            
            // enable the Browse button and the related Menu Item
            this.browseButton.Enabled = true;
            this.fileTextBox.Clear();
            this.fileTextBox.Enabled = true;

            // disable worksheet list and Show Data Button
            this.sheetComboBox.Items.Clear();
            this.sheetComboBox.Enabled = false;
            this.btnShowData.Visible = false;
            this.btnShowData.Enabled = false;

            // enable the Clear/Exit button and the related Menu Item
            this.cancelButton.Button_Text = "EXIT";
            this.cancelButton.Button_Enabled = true;

            // disable the Execute/Stop button and the related Menu Item
            this.executeButton.Button_Enabled = false;
            
            // disable the Column Mappings and Constants Tab Panels
            this.columnNamePanel.Controls.Clear();
            this.column_map_inputs.Clear();
            this.pnlConstants.Controls.Clear();
            this.constant_map_inputs.Clear();
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Enabled = false;
          
            // reset the status label
            this.labelStatus.Text = "";

            this.Cursor = Cursors.Default;
           
            // Move to STEP 1
            show_step_1();
        }

        private void Disable_FormControls()
        {
            // Disable the Browse button and the related Menu Item
            this.browseButton.Enabled = false;
            this.fileTextBox.Enabled = false;
            this.btnShowData.Enabled = false;

            // Disable worksheet list box
            this.sheetComboBox.Enabled = false;

            // Disable Tab Panel
            this.columnNamePanel.Enabled = false;
            this.pnlConstants.Enabled = false;
            
            // Disable the Execute/Stop button and the related Menu Item
            this.executeButton.Button_Enabled = false;

            // Disable the Clear/Exit button and the related Menu Item
            this.cancelButton.Button_Enabled = false;

            this.previewCheckBox.Enabled = false;
        }

        private void Enable_FormControls()
        {

            // Enable the Browse button and the related Menu Item
            this.browseButton.Enabled = true;
            this.fileTextBox.Enabled = true;
            this.btnShowData.Enabled = true;

            // Enable worksheet list box
            this.sheetComboBox.Enabled = true;

            // Enable Tab Panel
            this.columnNamePanel.Enabled = true;
            this.pnlConstants.Enabled = true;

            // Enable the Execute/Stop button and the related Menu Item
            this.executeButton.Button_Text = "EXECUTE";
            this.executeButton.Button_Enabled = true;

            // Enable the Clear/Exit button and the related Menu Item
            this.cancelButton.Button_Text = "EXIT";
            this.cancelButton.Button_Enabled = true;

            this.previewCheckBox.Enabled = true;
        }

        private bool Validate_Columns()
        {            
            bool retValue = false;

            if (this.excelDataTbl.Rows.Count >= 0)
            {
                // Check column and constant mappings to determine if the required fields 
                // necessary to create a recored in the tracking database exist. 

                // collection for holding error messages
                StringCollection errors = new StringCollection();
                string material_type = String.Empty;
                bool hasTitle = false;
                bool hasType = false;
                bool hasEntityType = false;
                bool hasProjectCode = false;
                bool copyright_data_error = false;

                // step through each column mapping
                // check if required field is mapped to a column in the input file
                foreach (Column_Assignment_Control thisColumn in column_map_inputs)
                {
                    // check title
                    //if (thisColumn.Mapped_Name.Equals("Title"))
                    if ((thisColumn.Mapped_Name.Equals("Bib (Series) Title"))
                            || (thisColumn.Mapped_Name.Equals("Bib (Uniform) Title"))
                            || (thisColumn.Mapped_Name.Equals("Title")))                                                                             
                    {
                        hasTitle = true;
                        continue;
                    }

                    // check material type
                    if (thisColumn.Mapped_Name.Equals("Material Type"))
                    {
                        hasType = true;
                        continue;
                    }

                    // check project code
                    if (thisColumn.Mapped_Name.Equals("Aggregation Code"))
                    {
                        hasProjectCode = true;
                        continue;
                    }

                    if (hasTitle && hasType && hasProjectCode)
                        break;
                }

                // step through each constant mapping        
                // check if required field has been selected from the Constants tab control, 
                // and that data has been selected from the adjoining combo box
                foreach (Constant_Assignment_Control thisConstant in constant_map_inputs)
                {

                    // check material type
                    if ((thisConstant.Mapped_Name.Equals("Material Type"))
                            && (thisConstant.Mapped_Constant.Length > 0))
                    {
                        hasType = true;
                        material_type = thisConstant.Mapped_Constant;
                        continue;
                    }

                    // check entity type
                    if ((thisConstant.Mapped_Name.Equals("Material Type"))                        
                            && (thisConstant.Mapped_Constant.Length > 0))                          
                    {
                        hasEntityType = true;
                        continue;
                    }

                    // check project code
                    if ((thisConstant.Mapped_Name.Equals("Aggregation Code"))
                            && (thisConstant.Mapped_Constant.Length > 0))
                    {
                        hasProjectCode = true;
                        continue;
                    }
                    
                    if (hasType && hasEntityType && hasProjectCode)
                        break;
                }                
               
                // check flags and assign any error messages

                // Title exist?
                if (!hasTitle)
                    //errors.Add("Title is missing");
                    errors.Add("At least one Title (Bib and/or Volume) value is required");


                // Material Type exist?
                if (!hasType)
                    errors.Add("Material Type is missing");
                
                // Project Code(s) exist?
                if (!hasProjectCode)
                    errors.Add("At least one project code must be selected");
                               
                       
                             
                // if there were errors, select a Tab page to be the active Tab                                
                if (copyright_data_error)
                    // select the 'Copyright Permissions' Tab Page
                    this.tabControl1.SelectedIndex = 2;
                else if (!hasType || !hasProjectCode)
                    // select the 'Constants' Tab Page
                    this.tabControl1.SelectedIndex = 1;   



                // if there were validation errors, build error message array                
                if (errors.Count > 0)
                {
                    retValue = false;
                    string[] missing = new string[errors.Count];

                    for (int i = 0; i < errors.Count; i++)
                    {
                        missing[i] = errors[i];
                    }

                    if (missing.Length > 0)
                    {
                        string message = "The following required fields are either missing or invalid:               \n\n";

                        foreach (string thisMissing in missing)
                            message = message +  "* " + thisMissing + "\n\n";
                        MessageBox.Show(message + "\nPlease complete these fields to continue.", "Invalid Entries", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // redisplay step 4 instructions
                        show_step_4();
                    }
                }
                else
                    // passed form validation
                    retValue = true;                
            }

            return retValue;
        }
                    
        private void show_step_1()
        {
            // show Step 1 instructions
            this.step1Label.ForeColor = ControlPaint.Dark(SystemColors.ActiveCaption);
            this.step2Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);
            this.step3Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);
            this.step4Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption); 
        }

        private void show_step_2()
        {
                // show Step 2 instructions
				this.step1Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);
                this.step2Label.ForeColor = ControlPaint.Dark(SystemColors.ActiveCaption);
        }

        private void show_step_3()
        {
            // show Step 3 instructions
            this.step2Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);
            this.step3Label.ForeColor = ControlPaint.Dark(SystemColors.ActiveCaption);
            this.panel1.Visible = true;

            // Enable form controls
            this.btnShowData.Enabled = true;
            this.btnShowData.Visible = true;
            this.tabControl1.Enabled = true;
        }

        private void show_step_4()
        {
            // show Step 4 instructions
            this.step3Label.ForeColor = ControlPaint.Dark(SystemColors.ActiveCaption);
            this.step4Label.ForeColor = ControlPaint.Dark(SystemColors.ActiveCaption);

            // Enable form controls
            this.Enable_FormControls();
        }
                   

        public int Total_Records
        {
            get
            {
                // count the number of valid records in the input table
                int counter = 0;

                // check for empty rows
                foreach (DataRow row in this.excelDataTbl.Rows)
                {

                    bool empty_row = true;
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (row[i].ToString().Length > 0)
                        {
                            empty_row = false;
                            counter++;
                            break;
                        }
                    }

                    if (empty_row)
                    {
                        // continue to next row in the input table
                        continue;
                    }
                }

                return counter;
            }
        }
        #endregion      

        /// <summary> Last tickler assigned through the form, during an import </summary>
        public string Last_Tickler
        {
            get
            {
                return tickler;
            }
        }

        public void Show_Form()
        {
            // show the SpreadSheet Importer form
            if (this != null)
                this.ShowDialog();
        }

        private void executeButton_Button_Pressed(object sender, EventArgs e)
        {
            if (this.executeButton.Button_Text.ToUpper().Equals("EXECUTE"))
            {

                // Change color on both the 'step3' and 'step 4 labels
                this.step3Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);
                this.step4Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);

                // Toggle the button text
                this.executeButton.Button_Text = "STOP";
                this.cancelButton.Button_Text = "CLEAR";


                // Make sure there are rows in the data table
                if (this.excelDataTbl.Rows.Count >= 0)
                {
                    if (!Validate_Columns())
                    {
                        this.executeButton.Button_Text = "EXECUTE";
                        this.cancelButton.Button_Text = "EXIT";
                        return;
                    }
                    else
                    {
                        // Import Records                   
                        this.Import_Records(this.excelDataTbl);
                    }
                }
                else
                {
                    this.executeButton.Button_Text = "EXECUTE";
                }
            }
            else if (this.executeButton.Button_Text.ToUpper().Equals("STOP"))
            {

                // Toggle the button text               
                this.Disable_FormControls();
                this.cancelButton.Button_Enabled = true;


                try
                {
                    // check if the Processor thread is running
                    if ((processThread != null) && (processThread.ThreadState == ThreadState.Running))
                    {
                        // Set flag to indicate that the Process thread will be
                        // aborted tbe next time through the delegate method: 
                        // processor_Volume_Processed().

                        this.processor.StopThread = true;
                        Thread.Sleep(100);
                    }
                }
                catch
                {
                }
            }
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            if (this.cancelButton.Button_Text.ToUpper().Equals("CLEAR"))
            {
                // reset the form to its default state
                this.ResetFormControls();

                // Toggle the button text
                this.cancelButton.Button_Text = "EXIT";
                this.executeButton.Button_Text = "EXECUTE";

            }
            else if (this.cancelButton.Button_Text.ToUpper().Equals("EXIT"))
            {
                this.Close();
            }
        }
    }
}
