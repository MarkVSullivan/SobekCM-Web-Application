#region Using directives

using System;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form is used to update the born digital flag on an item or a set of items </summary>
    public partial class Update_Born_Digital_Form : Form
    {
        private bool newFlag;

        /// <summary> Constructor for a new instance of the Update_Born_Digital_Form form </summary>
        public Update_Born_Digital_Form()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            newFlag = false;

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                checkBox1.FlatStyle = FlatStyle.Flat;
            }
        }

        /// <summary> Gets the born digital flag indicated by user </summary>
        public bool Born_Digital_Flag
        {
            get
            {
                return newFlag;
            }
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            newFlag = checkBox1.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
