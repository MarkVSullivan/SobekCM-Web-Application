#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SobekCM.Library;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form is used to update the disposition of an item or set of items  </summary>
    public partial class Update_Disposition_Form : Form
    {
        private DateTime date;
        private string notes;
        private int typeId;

        /// <summary> Constructor for a new instance of the Update_Disposition_Form form </summary>
        public Update_Disposition_Form()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            notes = String.Empty;
            typeId = -1;

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                textBox1.BorderStyle = BorderStyle.FixedSingle;
                comboBox1.FlatStyle = FlatStyle.Flat;                
            }

            // Add each possible disposition type
            List<string> dispositionTypes = SobekCM_Library_Settings.Disposition_Types_Past;
            foreach (string thisType in dispositionTypes)
                comboBox1.Items.Add(thisType);
            comboBox1.SelectedIndex = 0;

            BackColor = Color.FromArgb(240, 240, 240);
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary> Gets the disposition type id selected by the user </summary>
        public int Disposition_Type_ID
        {
            get
            {
                return typeId;
            }
        }

        /// <summary> Gets any disposition notes entered by the user </summary>
        public string Disposition_Notes
        {
            get
            {
                return notes;
            }
        }

        /// <summary> Gets any disposition date entered by the user </summary>
        public DateTime Disposition_Date
        {
            get
            {
                return date;
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

        private void textBox1_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Khaki;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            ((ComboBox)sender).BackColor = Color.Khaki;
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            ((ComboBox)sender).BackColor = Color.White;
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            notes = textBox1.Text;
            date = dateTimePicker1.Value;
            typeId = SobekCM_Library_Settings.Disposition_ID_Past(comboBox1.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
