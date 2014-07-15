#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Management_Tool.Config
{
    /// <summary> Form assists the user in configuring the SMaRT tool by building the Microsoft SQL
    /// connection string from the database name and server name  </summary>
    public partial class SobekCM_Database_Selection_Form : Form
    {
        /// <summary> Connection string built from the server and database names </summary>
        public string Connection_String;

        /// <summary> Constructor for a new instance of the SobekCM_Database_Selection_Form class </summary>
        public SobekCM_Database_Selection_Form( )
        {
            InitializeComponent();

            Connection_String = String.Empty;
            DialogResult = DialogResult.Cancel;
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Khaki;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;

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

            MessageBox.Show(SobekCM_Database.Test_Connection(Connection_String)
                                ? "CONNECTION SUCCESSFUL!"
                                : "CONNECTION FAILED!");
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void saveButton_Button_Pressed(object sender, EventArgs e)
        {
            Connection_String = "data source=" + serverNameTextBox.Text.Trim() + ";initial catalog=" + databaseNameTextBox.Text.Trim() + ";integrated security=Yes;";

            if (SobekCM_Database.Test_Connection(Connection_String))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                MessageBox.Show("CONNECTION FAILED!");
        }
    }
}
