using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Management_Tool.Importer.Forms
{
    public partial class Select_Material_Type_Form : Form
    {
        private string material_type;

        public Select_Material_Type_Form( string OCLC, string ALEPH, string Title, string BibID)
        {
            InitializeComponent();

            material_type = String.Empty;

            richTextBox1.Text = "Current record does not have valid material type information.\n\n";
            if (BibID.Length > 0)
                richTextBox1.AppendText("\tBibID:\t" + BibID + "\n");
            if (OCLC.Length > 0)
                richTextBox1.AppendText("\tOCLC:\t" + OCLC + "\n");
            if (ALEPH.Length > 0)
                richTextBox1.AppendText("\tALEPH:\t" + ALEPH + "\n");
            if (Title.Length > 0)
            {
                if (Title.Length < 200)
                {
                    richTextBox1.AppendText("\tTitle:\t" + Title + "\n");
                }
                else
                {
                    richTextBox1.AppendText("\tTitle:\t" + Title.Substring(200) + "...\n");
                }
            }
            richTextBox1.AppendText("\nMaterial type is a required field.");

            comboBox1.Items.Add("");
            comboBox1.Items.Add("Aerial");
            comboBox1.Items.Add("Archival");
            comboBox1.Items.Add("Artifact");
            comboBox1.Items.Add("Audio");
            comboBox1.Items.Add("Book");
            comboBox1.Items.Add("Map");
            comboBox1.Items.Add("Newspaper");
            comboBox1.Items.Add("Photograph");
            comboBox1.Items.Add("Serial");
            comboBox1.Items.Add("Video");
        }

        public string Material_Type
        {
            get
            {
                return material_type;
            }
        }

        public bool Always_Use_This_Answer
        {
            get
            {
                return alwaysUseOptionCheckBox.Checked;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            material_type = comboBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            material_type = String.Empty;
            alwaysUseOptionCheckBox.Checked = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
