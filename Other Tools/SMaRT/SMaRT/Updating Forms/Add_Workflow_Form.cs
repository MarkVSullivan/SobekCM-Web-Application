#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form allows the user to add a new workflow to an existing item, or a group
    /// of items </summary>
    public partial class Add_Workflow_Form : Form
    {
        private DateTime date;
        private string notes;
        private string type;

        /// <summary>Constructor for a new instance of the Add_Workflow_Form form </summary>
        public Add_Workflow_Form()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            notes = String.Empty;
            type = String.Empty;

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                textBox1.BorderStyle = BorderStyle.FixedSingle;
                comboBox1.FlatStyle = FlatStyle.Flat;
            }

            // Add each possible disposition type
            List<string> workflowTypes = Library.SobekCM_Library_Settings.Workflows;
            foreach (string thisType in workflowTypes)
                comboBox1.Items.Add(thisType);
            comboBox1.SelectedIndex = 0;

            BackColor = Color.FromArgb(240, 240, 240);
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary> Gets the workflow type selected by the user </summary>
        public string Workflow_Type
        {
            get
            {
                return type;
            }
        }

        /// <summary> Gets any progress notes entered by the user </summary>
        public string Workflow_Notes
        {
            get
            {
                return notes;
            }
        }

        /// <summary> Gets the workflow date entered by the user </summary>
        public DateTime Workflow_Date
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
            type = comboBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
