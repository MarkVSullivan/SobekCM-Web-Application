using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Management_Tool.Importer.Forms
{
    public partial class Select_Projects_Form : Form
    {
        private List<string> projects;

        public Select_Projects_Form( DataTable Project_Table, string OCLC, string ALEPH, string Title, string BibID )
        {
            InitializeComponent();

            richTextBox1.Text = "Current record does not have any project code information.\n\n";
            if (BibID.Length > 0)
                richTextBox1.AppendText("\tBibID:\t" + BibID + "\n");
            if (OCLC.Length > 0)
                richTextBox1.AppendText("\tOCLC:\t" + OCLC + "\n") ;
            if ( ALEPH.Length > 0 )
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
            richTextBox1.AppendText("\nIt is required to have at least one project linked to this item.");

            comboBox1.Items.Add("");
            comboBox2.Items.Add("");
            comboBox3.Items.Add("");

            projects = new List<string>();

            foreach (DataRow thisRow in Project_Table.Rows)
            {
                string thisProjCode = thisRow["itemcode"].ToString();
                comboBox1.Items.Add(thisProjCode);
                comboBox2.Items.Add(thisProjCode);
                comboBox3.Items.Add(thisProjCode);
            }
        }

        public List<string> Project_Codes
        {
            get
            {
                return projects;
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
            projects.Clear();
            if (comboBox1.Text.Length > 0)
                projects.Add(comboBox1.Text);
            if (comboBox2.Text.Length > 0)
                projects.Add(comboBox2.Text);
            if (comboBox3.Text.Length > 0)
                projects.Add(comboBox3.Text);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            projects.Clear();
            alwaysUseOptionCheckBox.Checked = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
