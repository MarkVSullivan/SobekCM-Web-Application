#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form allows the user to update the material received flag for an item
    /// or a set of items  </summary>
    public partial class Update_Material_Received_Form : Form
    {
        private DateTime dateReceived;
        private bool estimated;
        private string notes;

        /// <summary> Constructor for a new instance of the Update_Material_Received_Form form </summary>
        public Update_Material_Received_Form()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            dateReceived = DateTime.Now;
            estimated = false;
            notes = String.Empty;

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                estimatedCheckBox.FlatStyle = FlatStyle.Flat;
                textBox1.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        /// <summary> Gets the date received information the user entered </summary>
        public DateTime Date_Received
        {
            get
            {
                return dateReceived;
            }
        }

        /// <summary> Gets the flag indicated the date received is a wide estimate </summary>
        public bool Estimated
        {
            get
            {
                return estimated;
            }
        }

        /// <summary> Gets any notes associated with the material received event </summary>
        public string Notes
        {
            get
            {
                return notes;
            }
        }

        private void dateTimePicker1_Enter(object sender, EventArgs e)
        {
            ((DateTimePicker)sender).BackColor = Color.Khaki;
        }

        private void dateTimePicker1_Leave(object sender, EventArgs e)
        {
            ((DateTimePicker)sender).BackColor = Color.White;
        }

        private void estimatedCheckBox_Enter(object sender, EventArgs e)
        {
            ((CheckBox)sender).BackColor = Color.Khaki;
        }

        private void estimatedCheckBox_Leave(object sender, EventArgs e)
        {
            ((CheckBox)sender).BackColor = BackColor;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Khaki;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            dateReceived = dateTimePicker1.Value;
            estimated = estimatedCheckBox.Checked;
            notes = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }





    }
}
