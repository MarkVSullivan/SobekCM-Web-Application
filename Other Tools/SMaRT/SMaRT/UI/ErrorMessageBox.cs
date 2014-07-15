#region Using directives

using System;
using System.Windows.Forms;

#endregion

namespace DLC.Tools.Forms
{
    /// <summary> Class is used to launch the internal error message box which provides
    /// access to the stack trace of any caught exceptions  </summary>
    public class ErrorMessageBox
    {
        /// <summary> Display an error message box with access to the stack trace for 
        /// any caught exception  </summary>
        /// <param name="Message"> Message to display </param>
        /// <param name="Title"> Title for error message box which pops up </param>
        /// <param name="ee"> Exception to include access to the stack trace and information </param>
        /// <returns> Dialog result from the box </returns>
        public static DialogResult Show( string Message, string Title, Exception ee )
        {
            ErrorMessageBox_Internal showError = new ErrorMessageBox_Internal(Message, Title, ee);
            return showError.ShowDialog();
        }

        /// <summary> Display an error message box with access to the stack trace for 
        /// any caught exception  </summary>
        /// <param name="Message"> Message to display </param>
        /// <param name="Title"> Title for error message box which pops up </param>
        /// <returns> Dialog result from the box </returns>
        public static DialogResult Show(string Message, string Title)
        {
            return Show(Message, Title, null);
        }

        /// <summary> Display an error message box with access to the stack trace for 
        /// any caught exception  </summary>
        /// <param name="Message"> Message to display </param>
        /// <returns> Dialog result from the box </returns>
        public static DialogResult Show(string Message)
        {
            return Show(Message, String.Empty, null);
        }
    }
}
