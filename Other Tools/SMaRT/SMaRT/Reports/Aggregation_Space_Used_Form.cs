#region Using directives

using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GemBox.Spreadsheet;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Management_Tool.Reports
{
    /// <summary> Form collects input to determine the details for the aggregation space used report </summary>
    public partial class Aggregation_Space_Used_Form : Form
    {
        /// <summary> Constructor for a new instance of the Aggregation_Space_Used_Form class </summary>
        public Aggregation_Space_Used_Form()
        {
            InitializeComponent();

            onlineComboBox.SelectedIndex = 0;
            archiveComboBox.SelectedIndex = 0;
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Khaki;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Use the intersect code to determine the space used by items which \nspan two different aggregations.\n\nFor example, to find the utilized space (online and archival) for\nitems in both dLOC and CNDL, enter those two codes in the two boxes.        ", "Intersect Code Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            // Check for valid entry
            if ((textBox1.Text.Trim().Length == 0) && (textBox2.Text.Trim().Length == 0))
            {
                MessageBox.Show("You must enter at least one aggregation code.     ", "Invalid Entry",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the codes
            string code1 = textBox1.Text.Trim();
            string code2 = string.Empty;
            if (code1.Length == 0)
            {
                code1 = textBox2.Text.Trim();
            }
            else
            {
                code2 = textBox2.Text.Trim();
            }

            // Determine the values for the two types
            short onlineSpaceType = 1;
            if (onlineComboBox.SelectedIndex == 1)
                onlineSpaceType = 2;
            short archiveSpaceType = 0;
            if (archiveComboBox.SelectedIndex == 1)
                archiveSpaceType = 1;
            if (archiveComboBox.SelectedIndex == 2)
                archiveSpaceType = 2;
            if (archiveSpaceType == 2)
                onlineSpaceType = 2;

            // Get the file name to save this as
            string filename = String.Empty;
            if ((onlineSpaceType == 2) || (archiveSpaceType == 2))
            {
                DialogResult result = saveFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    filename = saveFileDialog1.FileName;
                }
                else
                {
                    return;
                }
            }



            // If archival will be pulled, show patience message
            if (archiveSpaceType > 1)
                MessageBox.Show(
                    "Please be patient as the report is built... this may take some time.        \n\nPress OK to begin report creation.",
                    "Patience", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Hide();

            // Get the requested report
            DataSet space_utilized = SobekCM_Database.Online_Archived_Space(code1, code2, onlineSpaceType, archiveSpaceType);

            if (space_utilized == null)
            {
                MessageBox.Show("Error pulling the space utilization report from the database.      ", "Database Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if ((onlineSpaceType == 1) && (archiveSpaceType != 2))
                {
                    // Name the report
                    string report_name = code1;
                    if (code2.Length > 0)
                        report_name = code1 + " intersect " + code2;

                    string online_space = space_utilized.Tables[0].Rows[0][0].ToString();
                    if ((archiveSpaceType > 0) && (space_utilized.Tables.Count > 1))
                    {
                        string archival_space = space_utilized.Tables[1].Rows[0][0].ToString();
                        MessageBox.Show(
                            "Total online space utilized by " + report_name + ": " + online_space +
                            "\n\nTotal local archived space utilized by " + report_name + ": " + archival_space,
                            report_name + " Space Utilization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Total online space utilized by " + report_name + ": " + online_space,
                                        report_name + " Space Utilization", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (!write_spreadsheet(filename, space_utilized, code1, code2))
                    {
                        MessageBox.Show("Error writing the space utilization report.      ", "Report Writing Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show("Space utilization report written.\n\n" + filename + "      ",
                                        "Report Writing Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

            }

            Close();
        }

        private bool write_spreadsheet(string filename, DataSet space_utilized, string code1, string code2 )
        {
            try
            {
                string worksheet_name = code1;
                if (code2.Length > 0)
                    worksheet_name = code1 + " intersect " + code2;

                // Create the excel file and worksheet
                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheet excelSheet = excelFile.Worksheets.Add(worksheet_name);
                excelFile.Worksheets.ActiveWorksheet = excelSheet;

                // Create the header cell style
                CellStyle headerStyle = new CellStyle();
                headerStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                headerStyle.FillPattern.SetSolid(Color.Yellow);
                headerStyle.Font.Weight = ExcelFont.BoldWeight;
                headerStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                // Create the title cell style
                CellStyle titleStyle = new CellStyle();
                titleStyle.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                titleStyle.Font.Weight = ExcelFont.BoldWeight;
                titleStyle.Font.Size = 12 * 20;

                // Set the two decimal places number format style
                CellStyle numberStyle = new CellStyle();
                numberStyle.NumberFormat = "#,#0.00";
                numberStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                // Set the no decimal places number format style
                CellStyle numberStyle2 = new CellStyle();
                numberStyle2.NumberFormat = "#,#0";
                numberStyle2.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                // Set the default style
                CellStyle defaultStyle = new CellStyle();
                defaultStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                excelSheet.Columns[0].Width = 4 * 256;

                // Write the online utilized space
                int column_selector = 1;
                if (space_utilized.Tables[0].Rows.Count > 0)
                {
                    excelSheet.Cells[1, column_selector].Value = "Total size of items online for " + worksheet_name + " by Month/Year";
                    excelSheet.Cells[1, column_selector].Style = titleStyle;

                    excelSheet.Cells[3, column_selector].Value = "Year";
                    excelSheet.Cells[3, column_selector].Style = headerStyle;

                    excelSheet.Cells[3, column_selector + 1].Value = "Month";
                    excelSheet.Cells[3, column_selector + 1].Style = headerStyle;

                    excelSheet.Cells[3, column_selector + 2].Value = "Added Online (KB)";
                    excelSheet.Cells[3, column_selector + 2].Style = headerStyle;

                    excelSheet.Cells[3, column_selector + 3].Value = "Total Online (MB)";
                    excelSheet.Cells[3, column_selector + 3].Style = headerStyle;

                    excelSheet.Cells[3, column_selector + 4].Value = "Total Online (GB)";
                    excelSheet.Cells[3, column_selector + 4].Style = headerStyle;


                    int row = 4;
                    double total_size = 0;
                    int year = Convert.ToInt16(space_utilized.Tables[0].Rows[0][0]);
                    int month = Convert.ToInt16(space_utilized.Tables[0].Rows[0][0]);


                    foreach (DataRow thisRow in space_utilized.Tables[0].Rows)
                    {
                        int this_month = Convert.ToInt32(thisRow[1]);
                        int this_year = Convert.ToInt32(thisRow[0]);
                        while (( this_month > month + 1 ) || ( this_year > year ))
                        {
                            month++;
                            if (month > 12)
                            {
                                month = 0;
                                year++;
                            }
                            else
                            {
                                excelSheet.Cells[row, column_selector].Value = year;
                                excelSheet.Cells[row, column_selector].Style = defaultStyle;

                                excelSheet.Cells[row, column_selector + 1].Value = month;
                                excelSheet.Cells[row, column_selector + 1].Style = defaultStyle;

                                excelSheet.Cells[row, column_selector + 2].Value = 0;
                                excelSheet.Cells[row, column_selector + 2].Style = numberStyle2;

                                excelSheet.Cells[row, column_selector + 3].Value = total_size;
                                excelSheet.Cells[row, column_selector + 3].Style = numberStyle;

                                excelSheet.Cells[row, column_selector + 4].Value = total_size / 1024;
                                excelSheet.Cells[row, column_selector + 4].Style = numberStyle;

                                row++;
                            }
                        }

                        excelSheet.Cells[row, column_selector].Value = this_year;
                        excelSheet.Cells[row, column_selector].Style = defaultStyle;

                        excelSheet.Cells[row, column_selector + 1].Value = this_month;
                        excelSheet.Cells[row, column_selector + 1].Style = defaultStyle;

                        excelSheet.Cells[row, column_selector + 2].Value = Convert.ToInt32(thisRow[2]);
                        excelSheet.Cells[row, column_selector + 2].Style = numberStyle2;

                        total_size = total_size + (Convert.ToDouble(thisRow[2]) / 1024);
                        excelSheet.Cells[row, column_selector + 3].Value = total_size;
                        excelSheet.Cells[row, column_selector + 3].Style = numberStyle;

                        excelSheet.Cells[row, column_selector + 4].Value = total_size / 1024;
                        excelSheet.Cells[row, column_selector + 4].Style = numberStyle;

                        month = this_month;
                        year = this_year;

                        row++;
                    }

                    // Set the correct widths
                    excelSheet.Columns[column_selector].Width = 8 * 256;
                    excelSheet.Columns[column_selector + 1].Width = 8 * 256;
                    excelSheet.Columns[column_selector + 2].Width = 20 * 256;
                    excelSheet.Columns[column_selector + 3].Width = 20 * 256;
                    excelSheet.Columns[column_selector + 4].Width = 20 * 256;

                    // Set the border
                    excelSheet.Cells.GetSubrange("B4", "F" + row).SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                    excelSheet.Cells.GetSubrange("B4", "F4").SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                  //  excelSheet.Cells.GetSubrange("B4", "F" + row).SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thick);

                    // Add to the column selector
                    column_selector = 7;
                }

                // Write the archived utilized space
                if (( space_utilized.Tables.Count > 1 ) && ( space_utilized.Tables[1].Rows.Count > 0))
                {
                    excelSheet.Cells[1, column_selector].Value = "Total size of items archived with CNS for " + worksheet_name + " by Month/Year";
                    excelSheet.Cells[1, column_selector].Style = titleStyle;

                    excelSheet.Cells[3, column_selector].Value = "Year";
                    excelSheet.Cells[3, column_selector].Style = headerStyle;

                    excelSheet.Cells[3, column_selector + 1].Value = "Month";
                    excelSheet.Cells[3, column_selector + 1].Style = headerStyle;

                    excelSheet.Cells[3, column_selector + 2].Value = "Added Archive (MB)";
                    excelSheet.Cells[3, column_selector + 2].Style = headerStyle;

                    excelSheet.Cells[3, column_selector + 3].Value = "Total Archive (MB)";
                    excelSheet.Cells[3, column_selector + 3].Style = headerStyle;

                    excelSheet.Cells[3, column_selector + 4].Value = "Total Archive (GB)";
                    excelSheet.Cells[3, column_selector + 4].Style = headerStyle;


                    int row = 4;
                    double total_size = 0;
                    int year = Convert.ToInt16(space_utilized.Tables[1].Rows[0][0]);
                    int month = Convert.ToInt16(space_utilized.Tables[1].Rows[0][0]);

                    foreach (DataRow thisRow in space_utilized.Tables[1].Rows)
                    {
                        int this_month = Convert.ToInt16(thisRow[1]);
                        int this_year = Convert.ToInt16(thisRow[0]);
                        while ((this_month > month + 1) || (this_year > year))
                        {
                            month++;
                            if (month > 12)
                            {
                                month = 0;
                                year++;
                            }
                            else
                            {
                                excelSheet.Cells[row, column_selector].Value = year;
                                excelSheet.Cells[row, column_selector].Style = defaultStyle;
                                excelSheet.Cells[row, column_selector + 1].Value = month;
                                excelSheet.Cells[row, column_selector + 1].Style = defaultStyle;
                                excelSheet.Cells[row, column_selector + 2].Value = 0;
                                excelSheet.Cells[row, column_selector + 2].Style = numberStyle2;
                                excelSheet.Cells[row, column_selector + 3].Value = total_size;
                                excelSheet.Cells[row, column_selector + 3].Style = numberStyle2;
                                excelSheet.Cells[row, column_selector + 4].Value = total_size / 1024;
                                excelSheet.Cells[row, column_selector + 4].Style = numberStyle;
                                row++;
                            }
                        }

                        excelSheet.Cells[row, column_selector].Value = this_year;
                        excelSheet.Cells[row, column_selector].Style = defaultStyle;

                        excelSheet.Cells[row, column_selector + 1].Value = this_month;
                        excelSheet.Cells[row, column_selector + 1].Style = defaultStyle;

                        excelSheet.Cells[row, column_selector + 2].Value = Convert.ToInt32(thisRow[2]);
                        excelSheet.Cells[row, column_selector + 2].Style = numberStyle2;

                        total_size = total_size + (Convert.ToDouble(thisRow[2]));
                        excelSheet.Cells[row, column_selector + 3].Value = total_size;
                        excelSheet.Cells[row, column_selector + 3].Style = numberStyle2;

                        excelSheet.Cells[row, column_selector + 4].Value = total_size / 1024;
                        excelSheet.Cells[row, column_selector + 4].Style = numberStyle;

                        month = this_month;
                        year = this_year;

                        row++;
                    }

                    // Set the correct widths
                    excelSheet.Columns[column_selector].Width = 8 * 256;
                    excelSheet.Columns[column_selector + 1].Width = 8 * 256;
                    excelSheet.Columns[column_selector + 2].Width = 20 * 256;
                    excelSheet.Columns[column_selector + 3].Width = 20 * 256;
                    excelSheet.Columns[column_selector + 4].Width = 20 * 256;

                    // Set the border
                    if (column_selector > 1)
                    {
                        excelSheet.Cells.GetSubrange("H4", "L4").SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                        excelSheet.Cells.GetSubrange("H4", "L" + row).SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                    }
                    else
                    {
                        excelSheet.Cells.GetSubrange("B4", "F4").SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                        excelSheet.Cells.GetSubrange("B4", "F" + row).SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                    }                   

                }


                // Save the file
                if (filename.ToUpper().IndexOf(".XLSX") > 0)
                {
                    excelFile.SaveXlsx(filename);
                }
                else
                {
                    excelFile.SaveXls(filename);
                }

                return true;
            }
            catch ( Exception ee )
            {
                return false;
            }
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
                LinearGradientBrush brush = new LinearGradientBrush(rect, SystemColors.Control, ControlPaint.Dark(SystemColors.Control), LinearGradientMode.Vertical);
                brush.SetBlendTriangularShape(0.33F);

                // Create the image
                gr.FillRectangle(brush, rect);
                gr.Dispose();

                // Set this as the backgroundf
                BackgroundImage = image;
            }
        }

        #endregion
    }
}
