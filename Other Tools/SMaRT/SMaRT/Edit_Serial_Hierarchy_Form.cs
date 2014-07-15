#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using SobekCM.Library.Database;
using SobekCM.Library.Items;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form is used to edit the serial hierarchy associated with an item group within a SobekCM library </summary>
    public partial class Edit_Serial_Hierarchy_Form : Form
    {
        private readonly string bibid;
        private int firstRow;
        private readonly SobekCM_Items_In_Title groupInfo;
        private int index;
        private readonly bool isXp;


        // References to the columns needed
        readonly DataColumn itemIdColumn;
        readonly DataColumn level1IndexColumn;
        readonly DataColumn level1TextColumn;
        readonly DataColumn level2IndexColumn;
        readonly DataColumn level2TextColumn;
        readonly DataColumn level3IndexColumn;
        readonly DataColumn level3TextColumn;
        private int locationY;
        private bool oddRow;
        private List<Serial_Hierarchy_Row> rows;
        private const int ROWS_PER_PAGE = 500;
        private readonly DataColumn titleColumn;
        private readonly DataColumn vidColumn;
        private readonly DataView view;


        /// <summary> Constructor for a new instance of the Edit_Item_Group_Form class </summary>
        /// <param name="Group_Info"> List of all the items within this item group </param>
        /// <param name="BibID"> BibID of the current item being displayed </param>
        public Edit_Serial_Hierarchy_Form(string BibID, SobekCM_Items_In_Title Group_Info)
        {
            groupInfo = Group_Info;
            bibid = BibID;

            // Get references to each column
            itemIdColumn = groupInfo.Item_Table.Columns["ItemID"];
            titleColumn = groupInfo.Item_Table.Columns["Title"];
            level1TextColumn = groupInfo.Item_Table.Columns["Level1_Text"];
            level1IndexColumn = groupInfo.Item_Table.Columns["Level1_Index"];
            level2TextColumn = groupInfo.Item_Table.Columns["Level2_Text"];
            level2IndexColumn = groupInfo.Item_Table.Columns["Level2_Index"];
            level3TextColumn = groupInfo.Item_Table.Columns["Level3_Text"];
            level3IndexColumn = groupInfo.Item_Table.Columns["Level3_Index"];
            vidColumn = groupInfo.Item_Table.Columns["VID"];

            if (!groupInfo.Item_Table.Columns.Contains("sort1"))
            {
                groupInfo.Item_Table.Columns.Add("sort1", Type.GetType("System.Int32"));
                groupInfo.Item_Table.Columns.Add("sort2", Type.GetType("System.Int32"));
                groupInfo.Item_Table.Columns.Add("sort3", Type.GetType("System.Int32"));
                groupInfo.Item_Table.Columns.Add("changed", Type.GetType("System.Boolean"));
            }

            foreach ( DataRow thisRow in groupInfo.Item_Table.Rows)
            {
                thisRow["sort1"] = thisRow[ level1IndexColumn ];
                thisRow["sort2"] = thisRow[ level2IndexColumn];
                thisRow["sort3"] = thisRow[ level3IndexColumn];
                thisRow["changed"] = false;
            }

            view = new DataView(groupInfo.Item_Table) {Sort = "sort1 ASC, sort2 ASC, sort3 ASC"};

            InitializeComponent();

            isXp = true;
            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                isXp = false;
                checkBox1.FlatStyle = FlatStyle.Flat;
                checkBox1.BackColor = Color.White;
            }

            BackColor = Color.FromArgb(240, 240, 240);
            headerPanel.BackColor = Color.FromArgb(25, 68, 141);

            Text = "Edit Item Group Information ( " + BibID + " )";

            rows = new List<Serial_Hierarchy_Row>();

            oddRow = true;
            locationY = 0;
            index = 0;
            if (groupInfo.Item_Table.Rows.Count < ROWS_PER_PAGE)
            {
                nextButton.Button_Enabled = false;
                prevButton.Button_Enabled = false;

                foreach (object t in groupInfo.Item_Table.Rows)
                {
                    add_row();
                }
            }
            else
            {
                prevButton.Button_Enabled = false;

                for (int i = 0; i < ROWS_PER_PAGE; i++)
                {
                    add_row();
                }
            }

            show_serial_hierarchy();

            // Set the size correctly
            Size = SMaRT_UserSettings.Edit_Hierarchy_Form_Size;
            int screen_width = Screen.PrimaryScreen.WorkingArea.Width;
            int screen_height = Screen.PrimaryScreen.WorkingArea.Height;
            if ((Width > screen_width) || (Height > screen_height) || ( SMaRT_UserSettings.Edit_Hierarchy_Form_Maximized ))
                WindowState = FormWindowState.Maximized;
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

        private void add_row()
        {
            Serial_Hierarchy_Row controlRow = new Serial_Hierarchy_Row( bibid );
            serialPanel.Controls.Add(controlRow);
            controlRow.Size = new Size(serialPanel.Width, 29);
            controlRow.Location = new Point(0, locationY);
            controlRow.Anchor = ((((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right)));
            controlRow.Odd_Row = oddRow;
            controlRow.Checked = true;
            controlRow.Index = index++;
            controlRow.isXP = isXp;
            controlRow.New_Row_Requested += controlRow_New_Row_Requested;
            locationY += 29;
            oddRow = !oddRow;

            rows.Add(controlRow);
        }

        void controlRow_New_Row_Requested(int rowIndex, int location)
        {
            if ((rowIndex >= 0) && ( rowIndex < rows.Count ))
            {
                rows[rowIndex].Focus();
                rows[rowIndex].Set_Interal_Focus(location);
            }
            if (rowIndex >= rows.Count)
            {
                if (prevButton.Button_Enabled)
                {
                    prevButton.Focus();
                    return;
                }

                if ( nextButton.Button_Enabled )
                {
                    nextButton.Focus();
                    return;
                }

                reorderButton.Focus();
            }
        }

        private void show_serial_hierarchy()
        {
            // Determine the number of columns of interest
            int col_interest = 1;
            foreach( DataRow itemRow in groupInfo.Item_Table.Rows )
            {
                if ((itemRow[level2TextColumn].ToString().Length > 0) && (col_interest < 2))
                    col_interest = 2;

                if ((itemRow[level3TextColumn].ToString().Length > 0) && (col_interest < 3))
                    col_interest = 3;

                if (col_interest == 3)
                    break;
            }

            switch (col_interest)
            {
                case 1:
                    level2RenumberLabel.Hide();
                    level2ReplaceLabel.Hide();
                    level3RenumberLabel.Hide();
                    level3ReplaceLabel.Hide();
                    level2Label.Location = new Point(473, 6);
                    level3Label.Location = new Point(572, 6);
                    level1RenumberLabel.Location = new Point(405, 6);
                    break;

                case 2:
                    level2RenumberLabel.Show();
                    level2ReplaceLabel.Show();
                    level3RenumberLabel.Hide();
                    level3ReplaceLabel.Hide();
                    level2ReplaceLabel.Location = new Point(406, 6);
                    level2RenumberLabel.Location = new Point(504, 6);
                    level2Label.Location = new Point(329, 6);
                    level3Label.Location = new Point(572, 6);
                    level1RenumberLabel.Location = new Point(261, 6);
                    break;

                default:
                    level2RenumberLabel.Show();
                    level2ReplaceLabel.Show();
                    level3RenumberLabel.Show();
                    level2ReplaceLabel.Show();
                    level2Label.Location = new Point(281, 6);
                    level3Label.Location = new Point(476, 6);
                    level2ReplaceLabel.Location = new Point(358, 6);
                    level2RenumberLabel.Location = new Point(408, 6);
                    level1RenumberLabel.Location = new Point(213, 6);
                    break;
            }

            int datarow = firstRow;
            foreach (Serial_Hierarchy_Row t in rows)
            {
                if (datarow < groupInfo.Item_Table.Rows.Count)
                {
                    t.Set_Serial_Hierarchy(view[datarow++].Row, itemIdColumn, titleColumn, level1TextColumn, level1IndexColumn, level2TextColumn, level2IndexColumn, level3TextColumn, level3IndexColumn, vidColumn);
                }
                else
                {
                    t.Set_Serial_Hierarchy( null, null, null, null, null, null, null, null, null, null );
                }
                t.Columns_Of_Interest = col_interest;
                t.Invalidate();
            }
        }

        private bool save_row_data()
        {
            return rows.All(controlRow => controlRow.Save(level1TextColumn, level1IndexColumn, level2TextColumn, level2IndexColumn, level3TextColumn, level3IndexColumn));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (WindowState != FormWindowState.Maximized)
            {
                SMaRT_UserSettings.Edit_Hierarchy_Form_Size = Size;
                SMaRT_UserSettings.Edit_Hierarchy_Form_Maximized = false;
            }
            else
                SMaRT_UserSettings.Edit_Hierarchy_Form_Maximized = true;

            SMaRT_UserSettings.Save();
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void prevButton_Button_Pressed(object sender, EventArgs e)
        {
            firstRow = firstRow - ROWS_PER_PAGE;
            if (firstRow <= 0)
            {
                firstRow = 0;
                prevButton.Button_Enabled = false;
            }
            nextButton.Button_Enabled = true;

            save_row_data();

            show_serial_hierarchy();
        }

        private void nextButton_Button_Pressed(object sender, EventArgs e)
        {
            firstRow = firstRow + ROWS_PER_PAGE;
            if (firstRow + ROWS_PER_PAGE > groupInfo.Item_Table.Rows.Count)
            {
                nextButton.Button_Enabled = false;
            }
            prevButton.Button_Enabled = true;

            save_row_data();

            show_serial_hierarchy();
        }

        private void reorderButton_Button_Pressed(object sender, EventArgs e)
        {
            save_row_data();

            foreach ( DataRow thisRow in groupInfo.Item_Table.Rows)
            {
                thisRow["sort1"] = thisRow[level1IndexColumn ];
                thisRow["sort2"] = thisRow[level2IndexColumn];
                thisRow["sort3"] = thisRow[level3IndexColumn];
            }

            show_serial_hierarchy();
        }

        private void linkLabel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.White, 11, 6, 20, 6);
            e.Graphics.DrawLine(Pens.White, 20, 6, 17, 3);
            e.Graphics.DrawLine(Pens.White, 20, 6, 17, 9);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Serial_Hierarchy_Row controlRow in rows)
            {
                controlRow.Checked = checkBox1.Checked;
            }
        }

        private void level1RenumberLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach ( DataRow itemRow in groupInfo.Item_Table.Rows)
            {
                int level1Index = Convert.ToInt32(itemRow[level1IndexColumn]);
                if (level1Index >= 0)
                {
                    itemRow[level1IndexColumn] = level1Index * 10;
                }
            }

            show_serial_hierarchy();
        }

        private void level2RenumberLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (DataRow itemRow in groupInfo.Item_Table.Rows)
            {
                int level2Index = Convert.ToInt32(itemRow[level2IndexColumn]);
                if (level2Index >= 0)
                {
                    itemRow[level2IndexColumn] = level2Index * 10;
                }
            }

            show_serial_hierarchy();
        }

        private void level3RenumberLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (DataRow itemRow in groupInfo.Item_Table.Rows)
            {
                int level3Index = Convert.ToInt32(itemRow[level3IndexColumn]);
                if (level3Index >= 0)
                {
                    itemRow[level3IndexColumn] = level3Index * 10;
                }
            }

            show_serial_hierarchy();
        }

        private void level1ReplaceLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool restrictToSelected = false;
            bool checked_found = false;
            bool unchecked_found = false;
            foreach (Serial_Hierarchy_Row controlRow in rows)
            {
                if ((controlRow.Checked) && (!checked_found))
                {
                    checked_found = true;
                }

                if ((!controlRow.Checked) && (!unchecked_found))
                {
                    unchecked_found = true;
                }
            }
            if ((checked_found) && (unchecked_found))
                restrictToSelected = true;

            Find_Replace_Form replaceForm = new Find_Replace_Form(restrictToSelected);
            replaceForm.ShowDialog();
            if ((!replaceForm.Cancelled) && ( replaceForm.Find_Value.Length > 0 ))
            {
                if (replaceForm.Checked_Rows_Only)
                {
                    foreach (Serial_Hierarchy_Row controlRow in rows)
                    {
                        if (controlRow.Checked)
                        {
                            controlRow.Serial_Hierarchy[level1TextColumn] = controlRow.Serial_Hierarchy[level1TextColumn].ToString().Replace(replaceForm.Find_Value, replaceForm.Replace_Value);
                        }
                    }
                }
                else
                {
                    foreach (DataRow itemRow in groupInfo.Item_Table.Rows)
                    {
                        itemRow[level1TextColumn] = itemRow[level1TextColumn].ToString().Replace(replaceForm.Find_Value, replaceForm.Replace_Value);
                    }
                }
            }

            show_serial_hierarchy();
        }

        private void level2ReplaceLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool restrictToSelected = false;
            bool checked_found = false;
            bool unchecked_found = false;
            foreach (Serial_Hierarchy_Row controlRow in rows)
            {
                if ((controlRow.Checked) && (!checked_found))
                {
                    checked_found = true;
                }

                if ((!controlRow.Checked) && (!unchecked_found))
                {
                    unchecked_found = true;
                }
            }
            if ((checked_found) && (unchecked_found))
                restrictToSelected = true;

            Find_Replace_Form replaceForm = new Find_Replace_Form(restrictToSelected);
            replaceForm.ShowDialog();
            if ((!replaceForm.Cancelled) && (replaceForm.Find_Value.Length > 0))
            {
                if (replaceForm.Checked_Rows_Only)
                {
                    foreach (Serial_Hierarchy_Row controlRow in rows)
                    {
                        if (controlRow.Checked)
                        {
                            controlRow.Serial_Hierarchy[level2TextColumn] = controlRow.Serial_Hierarchy[level2TextColumn].ToString().Replace(replaceForm.Find_Value, replaceForm.Replace_Value);
                        }
                    }
                }
                else
                {
                    foreach (DataRow itemRow in groupInfo.Item_Table.Rows)
                    {
                        itemRow[level2TextColumn] = itemRow[level2TextColumn].ToString().Replace(replaceForm.Find_Value, replaceForm.Replace_Value);
                    }
                }
            }

            show_serial_hierarchy();
        }

        private void level3ReplaceLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool restrictToSelected = false;
            bool checked_found = false;
            bool unchecked_found = false;
            foreach (Serial_Hierarchy_Row controlRow in rows)
            {
                if ((controlRow.Checked) && (!checked_found))
                {
                    checked_found = true;
                }

                if ((!controlRow.Checked) && (!unchecked_found))
                {
                    unchecked_found = true;
                }
            }
            if ((checked_found) && (unchecked_found))
                restrictToSelected = true;

            Find_Replace_Form replaceForm = new Find_Replace_Form(restrictToSelected);
            replaceForm.ShowDialog();
            if ((!replaceForm.Cancelled) && (replaceForm.Find_Value.Length > 0))
            {
                if (replaceForm.Checked_Rows_Only)
                {
                    foreach (Serial_Hierarchy_Row controlRow in rows)
                    {
                        if (controlRow.Checked)
                        {
                            controlRow.Serial_Hierarchy[level3TextColumn] = controlRow.Serial_Hierarchy[level3TextColumn].ToString().Replace(replaceForm.Find_Value, replaceForm.Replace_Value);
                        }
                    }
                }
                else
                {
                    foreach (DataRow itemRow in groupInfo.Item_Table.Rows)
                    {
                        itemRow[level3TextColumn] = itemRow[level3TextColumn].ToString().Replace(replaceForm.Find_Value, replaceForm.Replace_Value);
                    }
                }
            }

            show_serial_hierarchy();
        }

        private void saveButton_Button_Pressed(object sender, EventArgs e)
        {
            if (save_row_data())
            {
                Hide();

                int groupid = SobekCM_Database.Get_GroupID_From_BibID( bibid );

                foreach ( DataRow thisRow in groupInfo.Item_Table.Rows )
                {
                    SobekCM_Database.Save_Serial_Hierarchy(groupid, Convert.ToInt32(thisRow[itemIdColumn]),
                        thisRow[level1TextColumn].ToString(), Convert.ToInt32(thisRow[level1IndexColumn]),
                        thisRow[level2TextColumn].ToString(), Convert.ToInt32(thisRow[level2IndexColumn]),
                        thisRow[level3TextColumn].ToString(), Convert.ToInt32(thisRow[level3IndexColumn]),
                        String.Empty, -1, String.Empty, -1);
                }

                MessageBox.Show("COMPLETE!");
                Close();
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

        private void autoButton_Button_Pressed(object sender, EventArgs e)
        {
            // Show warning
            DialogResult result = MessageBox.Show(
                "This will attempt to determine the index values for all rows which are currently checked.     \n\nThis may cause good indexes to be over-written.\n\nAre you sure you would like to continue?               ",
                "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            // Did the user want to continue?
            if (result != DialogResult.Yes)
                return;

            // Now, step through each row
            foreach (Serial_Hierarchy_Row controlRow in rows)
            {
                if (controlRow.Checked)
                {
                    // Look at the first serial hierarchy level first
                    int possible_value = determine_index_from_string(controlRow.Serial_Hierarchy[level1TextColumn].ToString());
                    if (possible_value != -1)
                        controlRow.Serial_Hierarchy[level1IndexColumn] = possible_value;

                    // Look at the second serial hierarchy level next
                    possible_value = determine_index_from_string(controlRow.Serial_Hierarchy[level2TextColumn].ToString());
                    if (possible_value != -1)
                        controlRow.Serial_Hierarchy[level2IndexColumn] = possible_value;

                    // Look at the third serial hierarchy level next
                    possible_value = determine_index_from_string(controlRow.Serial_Hierarchy[level3TextColumn].ToString());
                    if (possible_value != -1)
                        controlRow.Serial_Hierarchy[level3IndexColumn] = possible_value;
                }
            }

            // Show the new values
            show_serial_hierarchy();
        }

        private int determine_index_from_string( string text )
        {
            // Is the entire thing numeric?
            bool numeric = text.Replace(" ", "").All(Char.IsNumber);
            if (numeric)
            {
                int possible_numeric = -1;
                Int32.TryParse(text, out possible_numeric);
                return possible_numeric;
            }
            
            // Is this a name of a month?
            switch( text.ToLower() )
            {
                case "jan":
                case "january":
                case "enero":
                case "janvier":
                    return 1;
                    
                case "feb":
                case "februrary":
                case "febrero":
                case "février":
                case "fevrier":
                    return 2;

                case "mar":
                case "march":
                case "marzo":
                case "mars":
                    return 3;

                case "apr":
                case "april":
                case "abril":
                case "avril":
                    return 4;

                case "may":
                case "mayo":
                case "mai":
                    return 5;

                case "jun":
                case "june":
                case "junio":
                case "juin":
                    return 6;

                case "jul":
                case "july":
                case "julio":
                case "juillet":
                    return 7;

                case "aug":
                case "august":
                case "agosto":
                case "aout":
                case "août":
                    return 8;

                case "sep":
                case "sept":
                case "september":
                case "septiembre":
                case "setiembre":
                case "septembre":
                    return 9;

                case "oct":
                case "october":
                case "octubre":
                case "octobre":
                    return 10;

                case "nov":
                case "november":
                case "noviembre":
                case "novembre":
                    return 11;

                case "dec":
                case "decemeber":
                case "diciembre":
                case "décembre":
                case "decembre":
                    return 12;
            }

            // As a final recourse, just try to pluck out any numbers from within the text
            int returnVal = 0;
            foreach (char thisChar in text)
            {
                if (Char.IsNumber(thisChar))
                {
                    if (returnVal > 0)
                        returnVal *= 10;
                    returnVal += Convert.ToInt32(thisChar);
                }
            }
            if (returnVal > 0)
                return returnVal;

            // Return -1, since we were unable to come up with anything
            return -1;
        }

    }
}
