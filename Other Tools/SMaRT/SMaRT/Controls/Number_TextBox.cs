#region Using directives

using System;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Text box which only allows the entry of numeric values </summary>
    /// <remarks> This is used during serial hierarchy editing and extends the TreeBox control </remarks>
    public class Number_TextBox : TextBox
    {
        #region Delegates

        /// <summary> Empty delegate used for the events fired by the <see cref="Number_TextBox"/> class's events </summary>
        public delegate void Number_TextBox_Delegate();

        #endregion

        private bool focusRegistered;

        /// <summary> Event is fired when the user hits the UP button from within this text box </summary>
        public event Number_TextBox_Delegate Up_Button_Pressed;

        /// <summary> Event is fired when the user hits the DOWN button from within this text box </summary>
        public event Number_TextBox_Delegate Down_Button_Pressed;

        /// <summary> Event is fired when the user hits the TAB button from within this text box </summary>
        public event Number_TextBox_Delegate Tab_Button_Pressed;

        /// <summary> Overrides the base TextBox's onLostFocus method and stores the fact that 
        /// focus has been unregistered </summary>
        /// <param name="e"> Event arguments </param>
        protected override void OnLostFocus(EventArgs e)
        {
            focusRegistered = false;

            base.OnLostFocus(e);
        }

        /// <summary> Overrides the base TextBox's OnMouseDown method and stores the fact that
        /// focus has been registered </summary>
        /// <param name="e"> Event arguments </param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            focusRegistered = true;

            base.OnMouseDown(e);
        }

        /// <summary> Overrides the base TextBox's PreProcessMessage method which listens for
        /// key strokes,raises the appropriate events when an UP button, DOWN button, or 
        /// TAB button is detected, and only allows numeric values to be entered </summary>
        /// <param name="msg"> Message to be preprocessed </param>
        /// <returns> TRUE if the keystroke is handled </returns>
        public override bool PreProcessMessage(ref Message msg)
        {
            Keys key = (Keys)(int) msg.WParam & Keys.KeyCode;


            if ((key == (Keys.LButton | Keys.Back)))
            {
                if (focusRegistered)
                {
                    if (Tab_Button_Pressed != null)
                    {
                        Tab_Button_Pressed();
                    }
                }
                else
                {
                    focusRegistered = true;
                }
                return true;
            }

            if (key == Keys.Down)
            {
                if (focusRegistered)
                {
                    if (Down_Button_Pressed != null)
                    {
                        Down_Button_Pressed();
                    }
                }
                else
                {
                    focusRegistered = true;
                }
                return true;
            }

            if (key == Keys.Up)
            {
                if (focusRegistered)
                {
                    if (Up_Button_Pressed != null)
                    {
                        Up_Button_Pressed();
                    }
                }
                else
                {
                    focusRegistered = true;
                }
                return true;
            }

            if ((key == Keys.D0) || (key == Keys.D1) || (key == Keys.D2) || (key == Keys.D3) || (key == Keys.D4) ||
                (key == Keys.D5) || (key == Keys.D6) || (key == Keys.D7) || (key == Keys.D8) || (key == Keys.D9) ||
                (key == Keys.NumPad0) || (key == Keys.NumPad1) || (key == Keys.NumPad2) || (key == Keys.NumPad3) || (key == Keys.NumPad4) ||
                (key == Keys.NumPad5) || (key == Keys.NumPad6) || (key == Keys.NumPad7) || (key == Keys.NumPad8) || (key == Keys.NumPad9))
            {
                return false;
            }

            if ((key == Keys.Tab) || (key == Keys.Left) || (key == Keys.Right) || (key == Keys.Back) || (key == Keys.Delete))
                return false;

            if (key == Keys.LButton)
                return false;

            // All others are handled
            return true;
        }

    }
}
