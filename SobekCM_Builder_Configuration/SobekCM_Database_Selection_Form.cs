using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Configuration
{
    public partial class SobekCM_Database_Selection_Form : Form
    {
        public string Connection_String;
        private bool during_install;

        public SobekCM_Database_Selection_Form( bool during_install )
        {
            InitializeComponent();
            this.during_install = during_install;
            if (during_install)
                testConnectionButton.Hide();

            Connection_String = String.Empty;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = System.Drawing.Color.Khaki;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = System.Drawing.Color.White;

            if ((serverNameTextBox.Text.Trim().Length > 0) && (serverNameTextBox.Text.IndexOf("/") > 0))
                serverNameTextBox.Text = serverNameTextBox.Text.Trim().Replace("/", "\\");
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if ((serverNameTextBox.Text.Trim().Length > 0) && (databaseNameTextBox.Text.Trim().Length > 0))
            {
                if (!testConnectionButton.Button_Enabled)
                    testConnectionButton.Button_Enabled = true;
                if (!saveButton.Button_Enabled)
                    saveButton.Button_Enabled = true;
            }
            else
            {
                if (testConnectionButton.Button_Enabled)
                    testConnectionButton.Button_Enabled = false;
                if (saveButton.Button_Enabled)
                    saveButton.Button_Enabled = false;
            }

        }

        private void testConnectionButton_Button_Pressed(object sender, EventArgs e)
        {
            Connection_String = "data source=" + serverNameTextBox.Text.Trim() + ";initial catalog=" + databaseNameTextBox.Text.Trim() + ";integrated security=Yes;";

            if (Database.SobekCM_Database.Test_Connection(Connection_String))
                MessageBox.Show("CONNECTION SUCCESSFUL!");
            else
                MessageBox.Show("CONNECTION FAILED!");
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void saveButton_Button_Pressed(object sender, EventArgs e)
        {
            Connection_String = "data source=" + serverNameTextBox.Text.Trim() + ";initial catalog=" + databaseNameTextBox.Text.Trim() + ";integrated security=Yes;";

            if ((during_install) || (Database.SobekCM_Database.Test_Connection(Connection_String)))
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
                MessageBox.Show("CONNECTION FAILED!");
        }
    }
}
