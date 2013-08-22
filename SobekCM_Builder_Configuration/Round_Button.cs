using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Configuration
{
    public partial class Round_Button : UserControl
    {
        #region Static class members 

        private static Color inactive_fill_color, active_fill_color, mouse_down_fill_color;
        private static Color inactive_border_color, active_border_color, mouse_down_border_color;
        private static Color inactive_text_color, active_text_color, mouse_down_text_color;

        public static Color Inactive_Fill_Color
        {
            get { return inactive_fill_color; }
            set { inactive_fill_color = value; }
        }

        public static Color Inactive_Border_Color
        {
            get { return inactive_border_color; }
            set { inactive_border_color = value; }
        }

        public static Color Inactive_Text_Color
        {
            get { return inactive_text_color; }
            set { inactive_text_color = value; }
        }

        public static Color Active_Fill_Color
        {
            get { return active_fill_color; }
            set { active_fill_color = value; }
        }

        public static Color Active_Border_Color
        {
            get { return active_border_color; }
            set { active_border_color = value; }
        }

        public static Color Active_Text_Color
        {
            get { return active_text_color; }
            set { active_text_color = value; }
        }

        public static Color Mouse_Down_Fill_Color
        {
            get { return mouse_down_fill_color; }
            set { mouse_down_fill_color = value; }
        }

        public static Color Mouse_Down_Border_Color
        {
            get { return mouse_down_border_color; }
            set { mouse_down_border_color = value; }
        }

        public static Color Mouse_Down_Text_Color
        {
            get { return mouse_down_text_color; }
            set { mouse_down_text_color = value; }
        }

        static Round_Button()
        {
            // Set some defaults
            inactive_fill_color = Color.Gray;
            inactive_border_color = Color.DarkGray;
            inactive_text_color = Color.DarkGray;
            active_fill_color = Color.Yellow;
            active_border_color = Color.DarkOrange;
            active_text_color = Color.Black;
            mouse_down_fill_color = Color.LightSteelBlue;
            mouse_down_border_color = Color.SteelBlue;
            mouse_down_text_color = Color.DarkSlateBlue;
        }

        #endregion

        public enum Button_Type_Enum
        {
            Standard = 1,
            Forward, 
            Backward,
            Full_Backward,
            Full_Forward
        }

        public event EventHandler Button_Pressed;
        private bool mouse_over;
        private bool mouse_down;
        private bool button_enabled;
        private Button_Type_Enum buttonType;

        public Round_Button()
        {
            InitializeComponent();
            buttonLabel.BackColor = Color.Transparent;
            buttonType = Button_Type_Enum.Standard;

            mouse_down = false;
            mouse_over = false;
            button_enabled = true;
        }

        public bool Button_Enabled
        {
            get
            {
                return button_enabled;
            }
            set
            {
                if (value != button_enabled)
                {
                    button_enabled = value;
                    if (button_enabled)
                    {
                        this.buttonLabel.Cursor = System.Windows.Forms.Cursors.Hand;
                    }
                    else
                    {
                        this.buttonLabel.Cursor = System.Windows.Forms.Cursors.Default;
                    }
                    this.Invalidate();
                }
            }
        }

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
                this.Invalidate();
            }
        }

        private void Round_Button_MouseEnter(object sender, EventArgs e)
        {
            if (button_enabled)
            {
                mouse_over = true;
                this.Invalidate();
            }
        }

        private void Round_Button_MouseLeave(object sender, EventArgs e)
        {
            mouse_over = false;
            mouse_down = false;
            if (button_enabled)
            {
                this.Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Tab) || (e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Space ))
                base.OnKeyDown(e);
            else
            {
                if (Button_Pressed != null)
                    Button_Pressed(this, new EventArgs());
            }
        }

        private void Round_Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (button_enabled)
            {
                mouse_over = true;
                mouse_down = true;
                this.Invalidate();
            }
        }

        private void Round_Button_MouseUp(object sender, MouseEventArgs e)
        {
            if (button_enabled)
            {
                if (Button_Pressed != null)
                    Button_Pressed(this, new EventArgs());

                mouse_down = false;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (button_enabled)
            {
                if ((!mouse_down) && (!mouse_over) && ( !Focused ))
                {
                    draw_button(e.Graphics, Round_Button.Active_Border_Color, Round_Button.Active_Fill_Color, Round_Button.Active_Text_Color);
                }
                else
                {
                    if (mouse_down)
                    {
                        draw_button(e.Graphics, Round_Button.Mouse_Down_Border_Color, Round_Button.Mouse_Down_Fill_Color, Round_Button.Mouse_Down_Text_Color);
                    }
                    else
                    {
                        draw_button(e.Graphics, Round_Button.Active_Border_Color, ControlPaint.LightLight( Round_Button.Active_Fill_Color ), Round_Button.Active_Text_Color);
                    }
                }
            }
            else
            {
                draw_button(e.Graphics, Round_Button.Inactive_Border_Color, Round_Button.Inactive_Fill_Color, Round_Button.Inactive_Text_Color);
            }

            base.OnPaint(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            this.Invalidate();
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            this.Invalidate();
            base.OnLeave(e);
        }

        private void draw_button(Graphics g, Color borderColor, Color fillColor, Color TextColor)
        {
            int height = this.Height - 2;
            int width = this.Width - 1;

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
                    g.FillPolygon(backBrush, new Point[] { new Point(10, radius), new Point(10 + radius, half_radius), new Point(10 + radius, 3 * half_radius) });
                    break;

                case Button_Type_Enum.Full_Backward:
                    Brush fullBackBrush = new SolidBrush(TextColor);
                    g.FillPolygon(fullBackBrush, new Point[] { new Point(10, radius), new Point(10 + radius, half_radius), new Point(10 + radius, 3 * half_radius) });
                    g.FillRectangle(fullBackBrush, 8, half_radius, 2, radius);
                    break;


                case Button_Type_Enum.Forward:
                    Brush forwardBrush = new SolidBrush(TextColor);
                    g.FillPolygon(forwardBrush, new Point[] { new Point(width - 10, radius), new Point(width - (10 + radius), half_radius), new Point(width - (10 + radius), 3 * half_radius) });
                    break;

                case Button_Type_Enum.Full_Forward:
                    Brush fullForwardBrush = new SolidBrush(TextColor);
                    g.FillPolygon(fullForwardBrush, new Point[] { new Point(width - 10, radius), new Point(width - (10 + radius), half_radius), new Point(width - (10 + radius), 3 * half_radius) });
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
