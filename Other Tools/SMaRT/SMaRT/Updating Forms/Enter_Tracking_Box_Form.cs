#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form is used to allow a user to enter a tracking box into a small
    /// pop-up form, for searching or for updating an item or a set of items </summary>
    public partial class Enter_Tracking_Box_Form : Form
    {
        private string newBoxString;

        /// <summary> Constructor for a new instance of the Enter_Tracking_Box_Form form </summary>
        public Enter_Tracking_Box_Form()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            newBoxString = String.Empty;

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                textBox1.BorderStyle = BorderStyle.FixedSingle;
            }

            BackColor = Color.FromArgb(240, 240, 240);
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary> Constructor for a new instance of the Enter_Tracking_Box_Form class </summary>
        /// <param name="Initial_Tracking_Box"> Initial tracking box to display in this form </param>
        public Enter_Tracking_Box_Form( string Initial_Tracking_Box )
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            newBoxString = String.Empty;
            textBox1.Text = Initial_Tracking_Box;

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                textBox1.BorderStyle = BorderStyle.FixedSingle;
            }

            BackColor = Color.FromArgb(240, 240, 240);
        }

        /// <summary> Gets the tracking box information entered by the user </summary>
        public string New_Tracking_Box
        {
            get { return newBoxString; }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            newBoxString = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }



        private void textBox1_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Khaki;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;
        }
    }
}
