#region Using directives

using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Control represents a single row in the serial hierarchy edit form </summary>
    public partial class Serial_Hierarchy_Row : UserControl
    {
        #region Delegates

        /// <summary> Delegate used for the new row requested event, fired when a user opts to move to the row before or after this row </summary>
        /// <param name="index"> Index for the requested row </param>
        /// <param name="location"> Which text box the focus should be placed </param>
        public delegate void Serial_Hiearchy_Row_Delegate( int index, int location );

        #endregion

        private readonly string bibid;
        private int columnsOfInterest = 3;
        private int index;
        private readonly Color oddColor = Color.FromArgb(238, 238, 238); //System.Drawing.Color.Gainsboro;
        private bool oddRow;
        private DataRow thisItem;
        private string vid;

        /// <summary> Constructor for a new instance of the Serial_Hierarchy_Row class </summary>
        public Serial_Hierarchy_Row( String BibID )
        {
            InitializeComponent();

            index = -1;
            bibid = BibID;

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                checkBox1.FlatStyle = FlatStyle.Flat;
                text1TextBox.BorderStyle = BorderStyle.FixedSingle;
                index1TextBox.BorderStyle = BorderStyle.FixedSingle;
                text2TextBox.BorderStyle = BorderStyle.FixedSingle;
                index2TextBox.BorderStyle = BorderStyle.FixedSingle;
                text3TextBox.BorderStyle = BorderStyle.FixedSingle;
                index3TextBox.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        /// <summary> Sets the number of columns of interest which should be emphasized in this row </summary>
        public int Columns_Of_Interest
        {
            set
            {
                columnsOfInterest = value;

                switch (columnsOfInterest)
                {
                    case 1:
                        text1TextBox.Location = new Point(72, 3);
                        text1TextBox.Size = new Size(328, 22);
                        index1TextBox.Location = new Point(398, 3);
                        text2TextBox.Location = new Point(459, 3);
                        text2TextBox.Size = new Size(40, 22);
                        index2TextBox.Location = new Point(498, 3);
                        text3TextBox.Location = new Point(558, 3);
                        text3TextBox.Size = new Size(40, 22);
                        break;

                    case 2:
                        text1TextBox.Location = new Point(72, 3);
                        text1TextBox.Size = new Size(184, 22);
                        index1TextBox.Location = new Point(255, 3);
                        text2TextBox.Location = new Point(315, 3);
                        text2TextBox.Size = new Size(184, 22);
                        index2TextBox.Location = new Point(498, 3);
                        text3TextBox.Location = new Point(558, 3);
                        text3TextBox.Size = new Size(40, 22);
                        break;

                    default:
                        text1TextBox.Location = new Point(72, 3);
                        text1TextBox.Size = new Size(136, 22);
                        index1TextBox.Location = new Point(207, 3);
                        text2TextBox.Location = new Point(267, 3);
                        text2TextBox.Size = new Size(136, 22);
                        index2TextBox.Location = new Point(402, 3);
                        text3TextBox.Location = new Point(462, 3);
                        text3TextBox.Size = new Size(136, 22);
                        break;

                }
            }
        }

        /// <summary> Flag sets the border style of all the text boxes, depending on whether the
        /// current windows appearance is XP-style or earlier Win95 style </summary>
        public bool isXP
        {
            set
            {
                if (!value)
                {
                    text1TextBox.BorderStyle = BorderStyle.FixedSingle;
                    text2TextBox.BorderStyle = BorderStyle.FixedSingle;
                    text3TextBox.BorderStyle = BorderStyle.FixedSingle;
                    index1TextBox.BorderStyle = BorderStyle.FixedSingle;
                    index2TextBox.BorderStyle = BorderStyle.FixedSingle;
                    index3TextBox.BorderStyle = BorderStyle.FixedSingle;
                    checkBox1.FlatStyle = FlatStyle.Flat;
                }
            }
        }

        /// <summary> Sets the index for this serial hierarcy edit row </summary>
        public int Index
        {
            set { index = value; }
        }

        /// <summary> Gets and sets the flag indicating this row is checked within the serial hierarchy edit form </summary>
        public bool Checked
        {
            set
            {
                checkBox1.Checked = value;
            }
            get
            {
                return checkBox1.Checked;
            }
        }

        /// <summary> Flag indicates if this is an odd row in the collection of rows to display </summary>
        /// <remarks> This just affects the background color used, to make the complete list of rows
        /// more readable. </remarks>
        public bool Odd_Row
        {
            set 
            { 
                oddRow = value;

                if (oddRow)
                {
                    BackColor = oddColor;
                    text1TextBox.BackColor = oddColor;
                    text2TextBox.BackColor = oddColor;
                    text3TextBox.BackColor = oddColor;
                    index1TextBox.BackColor = oddColor;
                    index2TextBox.BackColor = oddColor;
                    index3TextBox.BackColor = oddColor;
                }
            }
        }

        /// <summary> Gets and sets the item row with the serial hierarchy for this individual volume </summary>
        public DataRow Serial_Hierarchy
        {
            get
            {
                return thisItem;
            }
        }

        /// <summary> Event is fired when the user opts to move to the row before or after this one </summary>
        public event Serial_Hiearchy_Row_Delegate New_Row_Requested;

        /// <summary> Sets the internal focus on one of the text boxes within this serial hierarchy </summary>
        /// <param name="box"> Number indicates which text box to focus upon </param>
        public void Set_Interal_Focus(int box)
        {
            switch (box)
            {
                case 1:
                    text1TextBox.Focus();
                    break;

                case 2:
                    index1TextBox.Focus();
                    break;

                case 3: 
                    text2TextBox.Focus();
                    break;

                case 4:
                    index2TextBox.Focus();
                    break;

                case 5:
                    text3TextBox.Focus();
                    break;

                case 6:
                    index3TextBox.Focus();
                    break;

            }
        }

        /// <summary> Save the values from the text boxes within this serial hierarchy row to the
        /// item object held within this row </summary>
        /// <returns> Return TRUE if successful, otherwise FALSE </returns>
        public bool Save( DataColumn level1TextColumn, DataColumn level1IndexColumn, DataColumn level2TextColumn, DataColumn level2IndexColumn, DataColumn level3TextColumn, DataColumn level3IndexColumn )
        {
            if (thisItem != null)
            {
                try
                {
                    if ((index1TextBox.Text.Trim().Length > 0) && (Convert.ToUInt16(index1TextBox.Text.Trim()) >= 0))
                    {
                        thisItem[ level1TextColumn ] = text1TextBox.Text.Trim();
                        thisItem[level1IndexColumn] = Convert.ToUInt16(index1TextBox.Text.Trim());

                        if ((index2TextBox.Text.Trim().Length > 0) && (Convert.ToUInt16(index2TextBox.Text.Trim()) >= 0))
                        {
                            thisItem[level2TextColumn] = text2TextBox.Text.Trim();
                            thisItem[level2IndexColumn] = Convert.ToUInt16(index2TextBox.Text.Trim());

                            if ((index3TextBox.Text.Trim().Length > 0) && (Convert.ToUInt16(index3TextBox.Text.Trim()) >= 0))
                            {
                                thisItem[level3TextColumn] = text3TextBox.Text.Trim();
                                thisItem[level3IndexColumn] = Convert.ToUInt16(index3TextBox.Text.Trim());
                            }
                            else
                            {
                                thisItem[level3IndexColumn] = 0;
                                thisItem[level3TextColumn] = String.Empty;
                            }
                        }
                        else
                        {
                            thisItem[level2IndexColumn] = 0;
                            thisItem[level2TextColumn] = String.Empty;
                            thisItem[level3IndexColumn] = 0;
                            thisItem[level3TextColumn] = String.Empty;
                        }
                    }
                    else
                    {
                        thisItem[level1IndexColumn] = 0;
                        thisItem[level1TextColumn] = String.Empty;
                        thisItem[level2IndexColumn] = 0;
                        thisItem[level2TextColumn] = String.Empty;
                        thisItem[level3IndexColumn] = 0;
                        thisItem[level3TextColumn] = String.Empty;

                    }
                }
                catch
                {
                    MessageBox.Show("Error saving a serial hierarchy row to memory.\n\nIndexes must be between zero and 65536.                    ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        /// <summary> Sets the serial hierarchy displaying within this row </summary>
        /// <param name="Item_Row"></param>
        /// <param name="itemIdColumn"></param>
        /// <param name="titleColumn"></param>
        /// <param name="level1TextColumn"></param>
        /// <param name="level1IndexColumn"></param>
        /// <param name="level2TextColumn"></param>
        /// <param name="level2IndexColumn"></param>
        /// <param name="level3TextColumn"></param>
        /// <param name="level3IndexColumn"></param>
        /// <param name="vidColumn"></param>
        public void Set_Serial_Hierarchy(DataRow Item_Row, DataColumn itemIdColumn, DataColumn titleColumn, DataColumn level1TextColumn, DataColumn level1IndexColumn, DataColumn level2TextColumn, DataColumn level2IndexColumn, DataColumn level3TextColumn, DataColumn level3IndexColumn, DataColumn vidColumn)
        {
            thisItem = Item_Row;

            if (thisItem == null)
            {
                text1TextBox.Clear();
                index1TextBox.Clear();
                text2TextBox.Clear();
                index2TextBox.Clear();
                text3TextBox.Clear();
                index3TextBox.Clear();
                vid = String.Empty;

            }
            else
            {
                vid = Item_Row[vidColumn].ToString();
                string level1Text = Item_Row[level1TextColumn].ToString();
                if (level1Text.Length > 0)
                {
                    text1TextBox.Text = level1Text;
                    index1TextBox.Text = Math.Max(Convert.ToInt32(thisItem[level1IndexColumn]), 0).ToString();
                }
                else
                {
                    text1TextBox.Text = Item_Row[titleColumn].ToString();
                    index1TextBox.Text = "0";
                }

                string level2Text = Item_Row[level2TextColumn].ToString();
                if (level2Text.Length > 0)
                {
                    text2TextBox.Text = level2Text;
                    index2TextBox.Text = Math.Max(Convert.ToInt32(thisItem[level2IndexColumn]), 0).ToString();
                }
                else
                {
                    text2TextBox.Clear();
                    index2TextBox.Clear();
                }

                string level3Text = Item_Row[level3TextColumn].ToString();
                if (level3Text.Length > 0)
                {
                    text3TextBox.Text = level3Text;
                    index3TextBox.Text = Math.Max( Convert.ToInt32( Item_Row[level3IndexColumn] ), 0).ToString();
                }
                else
                {
                    text3TextBox.Clear();
                    index3TextBox.Clear();
                }
            }
        }

        /// <summary>  Override base OnPaint method which writes the VID as a graphics string on this row </summary>
        /// <param name="e"> Paint event arguments </param>
        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);

            if (thisItem != null)
            {
                e.Graphics.DrawString(vid, base.Font, Brushes.MediumBlue, 22, 5);
            }

           // e.Graphics.DrawLine(System.Drawing.Pens.Gray, 0, this.Height - 1, this.Width, this.Height - 1);
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Khaki;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = oddRow ? oddColor : Color.White;
        }

        private void checkBox_Enter(object sender, EventArgs e)
        {
            ((CheckBox)sender).BackColor = Color.Khaki;
        }

        private void checkBox_Leave(object sender, EventArgs e)
        {
            ((CheckBox)sender).BackColor = oddRow ? oddColor : Color.White;
        }

        private void index1TextBox_Down_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index + 1, 2);
        }

        private void index1TextBox_Tab_Button_Pressed()
        {
            text2TextBox.Focus();
        }

        private void index1TextBox_Up_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index - 1, 2);
        }

        private void index2TextBox_Down_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index + 1, 4);
        }

        private void index2TextBox_Tab_Button_Pressed()
        {
            text3TextBox.Focus();
        }

        private void index2TextBox_Up_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index - 1, 4);
        }

        private void index3TextBox_Down_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index + 1, 6);
        }

        private void index3TextBox_Tab_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index + 1, 1);
        }

        private void index3TextBox_Up_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index - 1, 6);
        }

        private void text1TextBox_Down_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index + 1, 1);
        }

        private void text1TextBox_Tab_Button_Pressed()
        {
            index1TextBox.Focus();
        }

        private void text1TextBox_Up_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index - 1, 1);
        }

        private void text2TextBox_Down_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index + 1, 3);
        }

        private void text2TextBox_Tab_Button_Pressed()
        {
            index2TextBox.Focus();
        }

        private void text2TextBox_Up_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index - 1, 3);
        }

        private void text3TextBox_Down_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index + 1, 5);
        }

        private void text3TextBox_Tab_Button_Pressed()
        {
            index3TextBox.Focus();
        }

        private void text3TextBox_Up_Button_Pressed()
        {
            if (New_Row_Requested != null)
                New_Row_Requested(index - 1, 5);
        }

        private void Serial_Hierarchy_Row_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.X > 20) && (e.X < 60))
            {
                Process showItem = new Process { StartInfo = { FileName = Library.SobekCM_Library_Settings.System_Base_URL + bibid + "\\" + vid } };
                showItem.Start();
            }
        }

        private void Serial_Hierarchy_Row_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void Serial_Hierarchy_Row_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.X > 20) && (e.X < 60))
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }



    }
}
