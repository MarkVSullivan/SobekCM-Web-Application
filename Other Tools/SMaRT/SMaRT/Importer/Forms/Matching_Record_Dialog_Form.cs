using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Management_Tool.Importer.Forms
{
    /// <summary> This is a dialog form that appears when a user trys to input a data row without a bib id and the row matches
    /// an existing bib record in the Tracking database.  The user must select either to overlay the existing bib record, create
    /// a new bib record, or skip the row in the input file.</summary>
    /// <remarks> This default radio button is used to force the other radio buttons to be unchecked when the form is displayed. </remarks>
    public partial class Matching_Record_Dialog_Form : Form
    {
        private bool alwaysUseOptionFlag;
        private Matching_Record_Choice_Enum selectedOptionValue;

        public Matching_Record_Dialog_Form(string dialogMessage, bool allow_overlay)
        {
            InitializeComponent();

            if (!allow_overlay)
                overlayRadioButton.Enabled = false;

            // Perform some additional work if this was not XP theme
            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                createNewRecordRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                skipRecordRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                alwaysUseOptionCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                overlayRadioButton.FlatStyle = FlatStyle.Flat;
            }

            this.messageLabel.Text = dialogMessage;
            this.selectedOptionValue = Matching_Record_Choice_Enum.Skip;           
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if ((!createNewRecordRadioButton.Checked) && (!skipRecordRadioButton.Checked) && ( !overlayRadioButton.Checked ))
            {
                MessageBox.Show("Select an option for processing the matching record.       ", "No Option Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Was CreateRecord checked?
                if (createNewRecordRadioButton.Checked)
                {
                    selectedOptionValue = Matching_Record_Choice_Enum.Create_New_Record;
                }

                // Was SkipRecord checked?
                if (skipRecordRadioButton.Checked)
                {
                    selectedOptionValue = Matching_Record_Choice_Enum.Skip;
                }

                // Was overlay checked?
                if (overlayRadioButton.Checked)
                {
                    selectedOptionValue = Matching_Record_Choice_Enum.Overlay_Bib_Record;
                }                

                // Was Always Use checkbox checked?                
                alwaysUseOptionFlag = this.alwaysUseOptionCheckBox.Checked;

                this.Close();
            }
        }

        public bool Always_Use_Option_Flag
        {
            get { return alwaysUseOptionFlag; }
        }

        public Matching_Record_Choice_Enum Selected_Option_Value
        {
            get { return selectedOptionValue; }
        }
    }
}

