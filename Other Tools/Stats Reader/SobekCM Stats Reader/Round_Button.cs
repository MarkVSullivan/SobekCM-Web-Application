#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace SobekCM_Stats_Reader
{
    /// <summary> Custom user control draws a rounded button on a windows form </summary>
    public partial class Round_Button : UserControl
    {
        #region Static class members 

        /// <summary> Static constructor for the round button control </summary>
        static Round_Button()
        {
            // Set some defaults
            Inactive_Fill_Color = Color.Gray;
            Inactive_Border_Color = Color.DarkGray;
            Inactive_Text_Color = Color.DarkGray;
            Active_Fill_Color = Color.Yellow;
            Active_Border_Color = Color.DarkOrange;
            Active_Text_Color = Color.Black;
            Mouse_Down_Fill_Color = Color.LightSteelBlue;
            Mouse_Down_Border_Color = Color.SteelBlue;
            Mouse_Down_Text_Color = Color.DarkSlateBlue;
        }

        /// <summary> Color used to fill the body of all inactive buttons </summary>
        public static Color Inactive_Fill_Color { get; set; }

        /// <summary> Color used to draw the border of all inactive buttons </summary>
        public static Color Inactive_Border_Color { get; set; }

        /// <summary> Color used to draw the text of all inactive buttons </summary>
        public static Color Inactive_Text_Color { get; set; }

        /// <summary> Color used to fill all active buttons </summary>
        public static Color Active_Fill_Color { get; set; }

        /// <summary> Color used to draw the border of all active buttons </summary>
        public static Color Active_Border_Color { get; set; }

        /// <summary> Color used to draw the text on all active buttons </summary>
        public static Color Active_Text_Color { get; set; }

        /// <summary> Color used to fill all active buttons when an active button is selected </summary>
        public static Color Mouse_Down_Fill_Color { get; set; }

        /// <summary> Color used to draw borders for all active buttons when an active button is selected </summary>
        public static Color Mouse_Down_Border_Color { get; set; }

        /// <summary> Color used to draw the text on all active buttons when an active button is selected </summary>
        public static Color Mouse_Down_Text_Color { get; set; }

        #endregion

        #region Button_Type_Enum enum

        /// <summary> Enumeration determines the type of arrow to display within the rendered button </summary>
        public enum Button_Type_Enum
        {
            /// <summary> Standard button does not have any arrows displayed </summary>
            Standard = 1,

            /// <summary> Button has a forward arrow pointing to the right </summary>
            Forward, 

            /// <summary> Button has a backward arrow pointing to the left </summary>
            Backward,

            /// <summary> Button has a backward arrow pointing to the left with a line indicating 
            /// to go all the way to the start </summary>
            Full_Backward,

            /// <summary> Button has a forward arrow pointing to the right with a line indicating
            /// to go all the way to the end </summary>
            Full_Forward
        }

        #endregion

        private Button_Type_Enum buttonType;
        private bool buttonEnabled;
        private bool mouseDown;
        private bool mouseOver;

        /// <summary> Constructor for a new instance of the Round_Button control </summary>
        public Round_Button()
        {
            InitializeComponent();
            buttonLabel.BackColor = Color.Transparent;
            buttonType = Button_Type_Enum.Standard;

            mouseDown = false;
            mouseOver = false;
            buttonEnabled = true;
        }

        /// <summary> Flag indicates if this button is currently enabled </summary>
        public bool Button_Enabled
        {
            get
            {
                return buttonEnabled;
            }
            set
            {
                if (value != buttonEnabled)
                {
                    buttonEnabled = value;
                    buttonLabel.Cursor = buttonEnabled ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }
            }
        }

        /// <summary> Text to be displayed within this button </summary>
        public string Button_Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                switch( buttonType )
                {
                    case Button_Type_Enum.Standard:
                        buttonLabel.Text = value;
                        break;

                    case Button_Type_Enum.Forward:
                    case Button_Type_Enum.Full_Forward:
                        buttonLabel.Text = value.Trim() + "   ";
                        break;

                    case Button_Type_Enum.Backward:
                    case Button_Type_Enum.Full_Backward:
                        buttonLabel.Text = "   " + value.Trim();
                        break;
                }
                base.Text = value;
            }
        }

        /// <summary> Type of button to be rendered ( what type of arrow to display ) </summary>
        public Button_Type_Enum Button_Type
        {
            get
            {
                return buttonType;
            }
            set
            {
                buttonType = value;
                Button_Text = buttonLabel.Text.Trim();
                Invalidate();
            }
        }

        /// <summary> Event is fired when this button is pressed </summary>
        public event EventHandler Button_Pressed;

        private void Round_Button_MouseEnter(object sender, EventArgs e)
        {
            if (buttonEnabled)
            {
                mouseOver = true;
                Invalidate();
            }
        }

        private void Round_Button_MouseLeave(object sender, EventArgs e)
        {
            mouseOver = false;
            mouseDown = false;
            if (buttonEnabled)
            {
                Invalidate();
            }
        }

        /// <summary> Overrides tbe standard OnKeyDown method and listens for a value which should cause the button to be pressed </summary>
        /// <param name="e"> Key event arguments </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Tab) || (e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Control) || (e.KeyCode == Keys.C))
                base.OnKeyDown(e);
            else
            {
                if (Button_Pressed != null)
                    Button_Pressed(this, new EventArgs());
            }
        }

        private void Round_Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (buttonEnabled)
            {
                mouseOver = true;
                mouseDown = true;
                Invalidate();
            }
        }

        private void Round_Button_MouseUp(object sender, MouseEventArgs e)
        {
            if (buttonEnabled)
            {
                mouseOver = false;
                mouseDown = false;

                if (Button_Pressed != null)
                    Button_Pressed(this, new EventArgs());


                Invalidate();
            }
        }

        /// <summary> Overrides tbe standard OnPaint method and draws this button </summary>
        /// <param name="e"> Paint event arguments </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (buttonEnabled)
            {
                if ((!mouseDown) && (!mouseOver) && ( !Focused ))
                {
                    draw_button(e.Graphics, Active_Border_Color, Active_Fill_Color, Active_Text_Color);
                }
                else
                {
                    if (mouseDown)
                    {
                        draw_button(e.Graphics, Mouse_Down_Border_Color, Mouse_Down_Fill_Color, Mouse_Down_Text_Color);
                    }
                    else
                    {
                        draw_button(e.Graphics, Active_Border_Color, ControlPaint.LightLight( Active_Fill_Color ), Active_Text_Color);
                    }
                }
            }
            else
            {
                draw_button(e.Graphics, Inactive_Border_Color, Inactive_Fill_Color, Inactive_Text_Color);
            }

            base.OnPaint(e);
        }

        /// <summary> Overrides tbe standard OnEnter method and causes this button to be redrawn </summary>
        /// <param name="e"> Event arguments </param>
        protected override void OnEnter(EventArgs e)
        {
            Invalidate();
            base.OnEnter(e);
        }

        /// <summary> Overrides tbe standard OnLeave method and causes this button to be redrawn </summary>
        /// <param name="e"> Event arguments </param>
        protected override void OnLeave(EventArgs e)
        {
            Invalidate();
            base.OnLeave(e);
        }

        private void draw_button(Graphics g, Color borderColor, Color fillColor, Color TextColor)
        {
            int height = Height - 2;
            int width = Width - 1;

            int half_radius = height / 4;
            int radius = half_radius * 2;
            height = half_radius * 4;

            Pen borderPen = new Pen( borderColor, 1);
            Brush fillBrush = new SolidBrush(fillColor);

            g.FillPie(fillBrush, 0, 1, height, height, 90, 180);
            g.FillRectangle(fillBrush, 0 + radius, 1, width - height, height);
            g.FillPie(fillBrush, 0 + width - height, 1, height, height, 270, 180);

            g.DrawArc(borderPen, 0, 1, height, height, 90, 180);
            g.DrawLine(borderPen, 0 + radius, 1, 0 + width - radius + 1, 1);
            g.DrawLine(borderPen, 0 + radius, 1 + height, 0 + width - radius + 1, 1 + height);
            g.DrawArc(borderPen, 0 + width - height, 1, height, height, 270, 180);

            buttonLabel.ForeColor = TextColor;

            switch (buttonType)
            {
                case Button_Type_Enum.Backward:
                    Brush backBrush = new SolidBrush( TextColor );
                    g.FillPolygon(backBrush, new[] { new Point(10, radius), new Point(10 + radius, half_radius), new Point(10 + radius, 3 * half_radius) });
                    break;

                case Button_Type_Enum.Full_Backward:
                    Brush fullBackBrush = new SolidBrush(TextColor);
                    g.FillPolygon(fullBackBrush, new[] { new Point(10, radius), new Point(10 + radius, half_radius), new Point(10 + radius, 3 * half_radius) });
                    g.FillRectangle(fullBackBrush, 8, half_radius, 2, radius);
                    break;


                case Button_Type_Enum.Forward:
                    Brush forwardBrush = new SolidBrush(TextColor);
                    g.FillPolygon(forwardBrush, new[] { new Point(width - 10, radius), new Point(width - (10 + radius), half_radius), new Point(width - (10 + radius), 3 * half_radius) });
                    break;

                case Button_Type_Enum.Full_Forward:
                    Brush fullForwardBrush = new SolidBrush(TextColor);
                    g.FillPolygon(fullForwardBrush, new[] { new Point(width - 10, radius), new Point(width - (10 + radius), half_radius), new Point(width - (10 + radius), 3 * half_radius) });
                    g.FillRectangle(fullForwardBrush, width - 8, half_radius, 2, radius);
                    break;
            }

            //if (addLabel)
            //{
            //    Label thisLabel = new Label();
            //    thisLabel.Font = this.Font;
            //    thisLabel.ForeColor = TextColor;
            //    thisLabel.BackColor = Color.Transparent;
            //    thisLabel.Text = Text;
            //    thisLabel.TextAlign = ContentAlignment.MiddleCenter;
            //    thisLabel.AutoSize = false;
            //    thisLabel.Location = new Point(x + (radius / 2), 6);
            //    thisLabel.Size = new Size(width - radius, height - 10);
            //    thisLabel.Tag = count;
            //    thisLabel.Cursor = Cursors.Hand;
            //    thisLabel.Click += new EventHandler(thisLabel_Click);
            //    this.Controls.Add(thisLabel);
            //    labels.Add(thisLabel);
            //}
            //else
            //{
            //    ((Label)labels[count - 1]).ForeColor = TextColor;
            //    ((Label)labels[count - 1]).Location = new Point(x + (radius / 2), 6);
            //    ((Label)labels[count - 1]).Size = new Size(width - radius, height - 10);
            //}

            // Brush textBrush = new SolidBrush( TextColor );
            // g.DrawString(Text, this.Font, textBrush, new RectangleF(x + (radius / 2), 6, width - radius, height - 10));

        }
    }
}
