#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace DLC.Tools.Forms
{
    /// <summary> Form displays an error message with optional access to the stack trace of
    /// any related, caught exceptions  </summary>
    public partial class ErrorMessageBox_Internal : Form
    {
        private readonly Exception innerException;
        private int lineY;

        /// <summary> Constructor for a new instance of the ErrorMessageBox_Internal error dialog form </summary>
        /// <param name="message"> Message to display</param>
        /// <param name="title"> Title for error message box which pops up</param>
        /// <param name="ee"> Exception to include access to the stack trace and information</param>
        public ErrorMessageBox_Internal( string message, string title, Exception ee )
        {
            // Save the exception 
            innerException = ee;

            // Set the default
            lineY = -1;

            // Initialize this form
            InitializeComponent();

            // Set the main label information
            mainLabel.Text = message; 

            // Set the title
            Text = title;

            // Configure the form now
            configure_all();

            // With only one button, the dialog result is obviously OK
            DialogResult = DialogResult.OK;
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void configure_all()
        {
            // Get the size of the label
            int label_width = mainLabel.Width;
            int label_height = mainLabel.Height;

            // Set the form size
            if (lineY > 0)
            {
                Width = Math.Max(Math.Max(mainLabel.Width, exceptionLabel.Width) + 94, 400 );
                Height = lineY + 80 + exceptionLabel.Height;
            }
            else
            {
                Height = label_height + 94;
                Width = Math.Max(label_width + 80, 250);
            }

            // Set the button location
            okButton.Location = new Point((Width - okButton.Width) / 2, Height - 65);

            // Configure the view details button
            if (innerException == null)
            {
                detailsLabel.Hide();
            }
            else
            {
                detailsLabel.Location = new Point(Width - 75, Height - 53);
                clipboardLabel.Location = new Point(Width - 138, Height - 53);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void detailsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Save the location for the line
            lineY = Math.Max(mainLabel.Location.Y + mainLabel.Height + 8, 60);
           // mainLabel.Hide();

            // Hide the label
            detailsLabel.Hide();
            clipboardLabel.Show();

            // Show the exception
            exceptionLabel.Text = innerException.ToString();
            exceptionLabel.Location = new Point(12, Math.Max(70, mainLabel.Location.Y + mainLabel.Height + 15));
            exceptionLabel.Show();

            // Configure the form
            configure_all();

            // Invalidate
            Invalidate();
        }

        private void clipboardLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Clipboard.SetText("TITLE: " + Text + "\n----------------------\n\n" + mainLabel.Text.Trim() + "\n\n----------------------\n\nEXCEPTION DETAILS:\n" + exceptionLabel.Text + "\n\n----------------------");
            }
            catch
            {
                MessageBox.Show("Error copying the stack trace into the clipboard.");
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the line, if necessary
            if (lineY > 0)
            {
                e.Graphics.DrawLine(Pens.DarkGray, 0, lineY, Width, lineY);
                e.Graphics.DrawLine(Pens.White, 0, lineY + 1, Width, lineY + 1);
            }

            // Call base onpaoint
            base.OnPaint(e);
        }
    }
}