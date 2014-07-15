#region Using directives

using System;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Control is a textbox which listens for the up button, down button, and tab button, passing
    /// these events out to an external control </summary>
    /// <remarks> This class extends the TextBox control </remarks>
    public class Arrow_TextBox : TextBox
    {
        #region Delegates

        /// <summary> Empty delegate used for the events fired by the <see cref="Arrow_TextBox"/> class's events </summary>
        public delegate void Arrow_TextBox_Delegate();

        #endregion

        private bool focusRegistered;

        /// <summary> Event is fired when the user hits the UP button from within this text box </summary>
        public event Arrow_TextBox_Delegate Up_Button_Pressed;

        /// <summary> Event is fired when the user hits the DOWN button from within this text box </summary>
        public event Arrow_TextBox_Delegate Down_Button_Pressed;

        /// <summary> Event is fired when the user hits the TAB button from within this text box </summary>
        public event Arrow_TextBox_Delegate Tab_Button_Pressed;

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
        /// key strokes and raises the appropriate events when an UP button, DOWN button, or 
        /// TAB button is detected. </summary>
        /// <param name="msg"> Message to be preprocessed </param>
        /// <returns> TRUE if the keystroke is handled </returns>
        public override bool PreProcessMessage(ref Message msg)
        {
            Keys key = (Keys)(int)msg.WParam & Keys.KeyCode;

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

            // All others are not handled handled
            return false;
        }

    }
}
