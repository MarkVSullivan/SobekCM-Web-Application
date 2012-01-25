using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using UFDC_Bib_Package;
using DLC.MetaTemplate.Template;

namespace DLC.MetaTemplate.Elements
{
    /// <summary>
    /// Summary description for multiple_TextBox_Element.
    /// </summary>
    public abstract class multiple_textBox_Element : abstract_Element
    {
        protected ArrayList textBoxes;
        private int text_box_length;
        private int max_box_count;
        private int plus_x;
        private int plus_y;
        private bool isXP;

        public multiple_textBox_Element()
        {
            // Set the default text box length
            text_box_length = 80;
            max_box_count = 5;

            // Configure the collection of text boxes
            textBoxes = new ArrayList();

            // Configure the text box
            TextBox thisBox = new TextBox();
            thisBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            thisBox.Location = new Point(115, 2);
            thisBox.BackColor = Color.White;
            thisBox.Width = text_box_length;
            thisBox.Enter += new EventHandler(textBox_Enter);
            thisBox.Leave += new EventHandler(textBox_Leave);
            thisBox.ForeColor = System.Drawing.Color.MediumBlue;
            thisBox.KeyDown += new KeyEventHandler(thisBox_KeyDown);
            textBoxes.Add(thisBox);
            this.Controls.Add(thisBox);

            // Set default title to blank
            title = "no default";

            // Add interest in the text box changing
            thisBox.TextChanged += new EventHandler(thisBox_TextChanged);

            maximum_input_length = 1200;
            always_single = false;
            repeatable = true;

            plus_y = 3;

            if (!DLC.Tools.Windows_Appearance_Checker.is_XP_Theme)
            {
                isXP = false;
                thisBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
            else
            {
                isXP = true;
            }
        }


        public multiple_textBox_Element(string defaultTitle)
        {        
            // Set the default text box length
            text_box_length = 80;
            max_box_count = 5;

            // Configure the collection of text boxes
            textBoxes = new ArrayList();

            // Configure the text box
            TextBox thisBox = new TextBox();
            thisBox.Location = new Point(115, 2);
            thisBox.BackColor = Color.White;
            thisBox.Width = text_box_length;
            thisBox.Enter += new EventHandler(textBox_Enter);
            thisBox.Leave += new EventHandler(textBox_Leave);
            thisBox.ForeColor = System.Drawing.Color.MediumBlue;
            thisBox.KeyDown += new KeyEventHandler(thisBox_KeyDown);
            textBoxes.Add(thisBox);
            this.Controls.Add(thisBox);

            // Save the title
            title = defaultTitle;

            // Add interest in the text box changing
            thisBox.TextChanged += new EventHandler(thisBox_TextChanged);

            maximum_input_length = 1200;
            always_single = false;
            repeatable = true;

            plus_y = 3;

            if (!DLC.Tools.Windows_Appearance_Checker.is_XP_Theme)
            {
                isXP = false;
                thisBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
            else
            {
                isXP = true;
            }
        }

        void thisBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((!read_only) && (textBoxes.Count < max_box_count) && (e.KeyCode == Keys.Down))
            {
                add_new_box();
            }
        }

        void textBox_Leave(object sender, EventArgs e)
        {
            if (!read_only)
            {
                ((TextBox)sender).BackColor = Color.White;
            }
        }

        void textBox_Enter(object sender, EventArgs e)
        {
            if (!read_only)
            {
                ((TextBox)sender).BackColor = Color.Khaki;
            }
        }

        protected int Text_Box_Length
        {
            get
            {
                return text_box_length;
            }
            set
            {
                text_box_length = value;
                position_text_boxes();
            }
        }

        protected int Max_Box_Count
        {
            get
            {
                return max_box_count;
            }
            set
            {
                max_box_count = value;
            }
        }


        protected void add_new_box()
        {
            // Configure the text box
            TextBox thisBox = new TextBox();
            if (!isXP)
            {
                thisBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
            thisBox.Location = new Point( title_length + ( textBoxes.Count * (text_box_length + 20 )), 2);
            if (this.read_only)
            {
                thisBox.BackColor = Color.WhiteSmoke;
                thisBox.ReadOnly = true;
            }
            else
            {
                thisBox.BackColor = Color.White;
            }
            thisBox.Width = text_box_length;
            thisBox.Enter += new EventHandler(textBox_Enter);
            thisBox.Leave += new EventHandler(textBox_Leave);
            thisBox.ForeColor = System.Drawing.Color.MediumBlue;
            thisBox.KeyDown += new KeyEventHandler(thisBox_KeyDown);
            textBoxes.Add(thisBox);
            this.Controls.Add(thisBox);

            // Add interest in the text box changing
            thisBox.TextChanged += new EventHandler(thisBox_TextChanged);
            position_text_boxes();

            this.Invalidate();
        }

        protected void position_text_boxes()
        {
            int last_plus_y = plus_y;

            // Calculate the number of boxes to display per line
            int boxes = (int) (this.Width - Title_Length - 20) / (text_box_length + 20);

            // Now, position each box
            int position = title_length;
            int column = 0;
            int row = 0;
            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (column >= boxes)
                {
                    row++;
                    position = title_length;
                    column = 0;
                }

                TextBox thisBox = (TextBox) textBoxes[i];
                thisBox.Location = new Point(position, 2 + ( row * 30));
                thisBox.Width = text_box_length;
                position = position + text_box_length + 20;

                column++;
            }

            plus_x = position - 10;
            plus_y = (row * 30) + 3;

            if (plus_y != last_plus_y)
            {
                Inner_Set_Height(this.Font.Size);
                OnRedrawRequested();
            }
        }

        void thisBox_TextChanged(object sender, EventArgs e)
        {
            OnDataChanged();
        }

        /// <summary> Override the OnPaint method to draw the title before the text box </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the title
            Draw_Title(e.Graphics, title);

            // Determine the y-mid-point
            int midpoint = (int)(1.5 * this.Font.SizeInPoints);

            // If this is repeatable, show the '+' to add another after this one
            if (textBoxes.Count < max_box_count)
            {
                int plus_location_x = title_length + (textBoxes.Count * (text_box_length + 20)) - 10;
                Draw_Repeatable_Icon(e.Graphics, plus_x, plus_y + 2);
            }

            // Call this for the base
            OnPaint(e);
        }

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the Template XML format </summary>
        protected override void Inner_Read_Data(XmlTextReader xmlReader)
        {

        }

        /// <summary> Writes the inner data into Template XML format </summary>
        protected override string Inner_Write_Data()
        {
            return String.Empty;
        }

        /// <summary> Perform any height setting calculations specific to the 
        /// implementation of abstract_Element.  </summary>
        /// <param name="size"> Height of the font </param>
        protected override void Inner_Set_Height(float size)
        {
            // Set total height
            int size_int = (int)size;
            this.Height = plus_y + ((int)(2.5 * size_int));
        }

        /// <summary> Perform any width setting calculations specific to the 
        /// implementation of abstract_Element.  </summary>
        /// <param name="size"> Height of the font </param>
        protected override void Inner_Set_Width(int new_width)
        {
            position_text_boxes();

        }

        /// <summary> Perform any readonly functions specific to the
        /// implementation of abstract_Element. </summary>
        protected override void Inner_Set_Read_Only()
        {
            if (read_only)
            {
                foreach (TextBox thisBox in textBoxes)
                {
                    thisBox.ReadOnly = true;
                    thisBox.BackColor = Color.WhiteSmoke;
                }
            }
            else
            {
                foreach (TextBox thisBox in textBoxes)
                {
                    thisBox.ReadOnly = false;
                    thisBox.BackColor = Color.White;
                }
            }
        }

        /// <summary> Clones this element, not copying the actual data
        /// in the fields, but all other values. </summary>
        /// <returns>Clone of this element</returns>
        public override abstract_Element Clone()
        {
            // Get the new element
            multiple_textBox_Element newElement = (multiple_textBox_Element)Element_Factory.getElement(this.Type, this.Display_SubType);
            newElement.Location = this.Location;
            newElement.Language = this.Language;
            newElement.Title_Length = this.Title_Length;
            newElement.Text_Box_Length = this.Text_Box_Length;
            newElement.Height = this.Height;
            newElement.Font = this.Font;
            newElement.Set_Width(this.Width);
            newElement.Index = this.Index + 1;
            return (abstract_Element) newElement;
        }

        /// <summary> Gets the flag indicating this element has an entered value </summary>
        public override bool hasValue
        {
            get
            {
                bool returnVal = false;
                foreach (TextBox thisBox in textBoxes)
                {
                    if (thisBox.Text.Trim().Length > 0)
                    {
                        returnVal = true;
                        break;
                    }
                }
                return returnVal;
            }
        }

        /// <summary> Checks the data in this element for validity. </summary>
        /// <returns> TRUE if valid, otherwise FALSE </returns>
        /// <remarks> This sets the <see cref="abstract_Element.Invalid_String" /> value. </remarks>
        public override bool isValid()
        {
            return true;
        }

        #endregion

        #region Mouse Listener Methods

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (standard_mouse_actions)
            {
                // Determine the y-mid-point
                int midpoint = (int)(1.5 * this.Font.SizeInPoints);

                // Was this over the title
                if ((index == 0) && (e.X < (title_length - 10)) && (e.Y > 8) && (e.Y < (13 + this.Font.SizeInPoints)))
                {
                    OnHelpRequested();
                }

                // Was this over the '+'?
                int plus_location_x = title_length + (textBoxes.Count * (text_box_length + 20)) - 10;
                if ((textBoxes.Count < max_box_count) && (this.Repeatable) && (e.X > plus_x) && (e.X < plus_x + 15) && (e.Y > plus_y) && (e.Y < plus_y + 15))
                {
                    add_new_box();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (standard_mouse_actions)
            {
                // Determine the y-mid-point
                int midpoint = (int)(1.5 * this.Font.SizeInPoints);

                // Was this over the title
                bool overTitle = false;
                if ((index == 0) && (e.X < (title_length - 10)) && (e.Y > 8) && (e.Y < (13 + this.Font.SizeInPoints)))
                {
                    overTitle = true;
                }

                // Was this over the '+'?
                int plus_location_x = title_length + (textBoxes.Count * (text_box_length + 20)) - 10;
                bool overPlus = false;
                if ((textBoxes.Count < max_box_count) && (this.Repeatable) && (e.X > plus_x) && (e.X < plus_x + 15) && (e.Y > plus_y) && (e.Y < plus_y + 15))
                {
                    overPlus = true;
                }

                // Set the cursor correctly
                if ((overTitle) || (overPlus))
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Arrow;
                }

                // If over the title, do a bit more
                if (overTitle)
                {
                    if (!link_active)
                    {
                        link_active = true;
                        this.Invalidate();
                    }
                }
                else
                {
                    if (link_active)
                    {
                        link_active = false;
                        this.Invalidate();
                    }
                }
            }
        }


        #endregion

    }
}
