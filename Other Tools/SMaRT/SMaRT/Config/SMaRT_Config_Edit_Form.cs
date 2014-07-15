#region Using directives

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using SobekCM.Library.Database;
using SobekCM.Management_Tool.Versioning;

#endregion

namespace SobekCM.Management_Tool.Config
{
    /// <summary> Configuration helps to provide the very basic configuration information
    /// the first time this application is run on a computer.  This provides the connection information 
    /// for the SobekCM database </summary>
    public partial class SMaRT_Config_Edit_Form : Form
    {
        private const string HELP_URL = "http://ufdc.ufl.edu/sobekcm";

        private string connectionString = String.Empty;
        private string dbType = "MSSQL";
        private readonly bool errorReading;
        private string errorEmails = String.Empty;
        private string errorPage = String.Empty;
        private string ghostscript = String.Empty;
        private string imagemagick = String.Empty;

        /// <summary> Constructor for a new instance of the SobekCM_Web_Config_Edit_Form class </summary>
        public SMaRT_Config_Edit_Form( )
        {
            InitializeComponent();

            // Try to read a config file
            try
            {
                // Is there a valid config file?
                string dir = Application.StartupPath;
                if (File.Exists(dir + "\\sobekcm.config"))
                    Read_Configuration_File(dir + "\\config\\sobekcm.config");
                else if (File.Exists(dir + "\\sample.config"))
                    Read_Configuration_File(dir + "\\config\\sample.config");
            }
            catch 
            {
                errorReading = true;   
            }

            // Set the current values as well
            connectionTextBox.Text = connectionString;
            typeComboBox.SelectedIndex = dbType.ToLower() == "postgresql" ? 1 : 0;

            // Check the SAVE button
            if (connectionString.Trim().Length == 0)
                saveButton.Button_Enabled = false;
        }

        /// <summary> Returns flag indicating if there was an error reading the configuration
        /// file  </summary>
        public bool Error_Reading
        {
            get { return errorReading; }
        }

        private void Read_Configuration_File(string config_file)
        {
            StreamReader reader = new StreamReader(config_file);
            XmlTextReader xmlReader = new XmlTextReader(reader);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    string node_name = xmlReader.Name.ToLower();
                    switch (node_name)
                    {
                        case "connection_string":
                            if (xmlReader.MoveToAttribute("type"))
                            {
                                dbType = xmlReader.Value;
    
                            }
                            xmlReader.Read();
                            connectionString = xmlReader.Value;
                            break;

                        case "error_emails":
                            xmlReader.Read();
                            errorEmails = xmlReader.Value;
                            break;

                        case "error_page":
                            xmlReader.Read();
                            errorPage = xmlReader.Value;
                            break;

                        case "ghostscript_executable":
                            xmlReader.Read();
                            ghostscript = xmlReader.Value;
                            break;

                        case "imagemagick_executable":
                            xmlReader.Read();
                            imagemagick = xmlReader.Value;
                            break;
                    }
                }
            }
            xmlReader.Close();
            reader.Close();
        }

        private bool Save_Configuration()
        {
            try
            {
                if (!Directory.Exists(Application.StartupPath + "\\config "))
                    Directory.CreateDirectory(Application.StartupPath + "\\config");
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to create the necessary and missing 'config' subfolder.\n\nBe sure you have write access to the startup path of this application:                       \n\n" + Application.StartupPath );
                return false;
            }

            try
            {
                StreamWriter writer = new StreamWriter(Application.StartupPath + "\\config\\sobekcm.config", false);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
                writer.WriteLine("<configuration>");
               writer.WriteLine("  <connection_string type=\"" + dbType + "\">" + connectionTextBox.Text.Trim().Replace("&", "&amp;").Replace("\"", "&quot;").Replace(">","&gt;").Replace("<","&lt;") + "</connection_string>");
               if ( !String.IsNullOrEmpty(errorEmails) )
                   writer.WriteLine("  <error_emails>" + errorEmails.Trim().Replace("&", "&amp;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;") + "</error_emails>");
               if ( !String.IsNullOrEmpty(errorPage ))
                   writer.WriteLine("  <error_page>" + errorPage.Trim().Replace("&", "&amp;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;") + "</error_page>");
                if ( !String.IsNullOrEmpty(ghostscript))
                    writer.WriteLine("  <ghostscript_executable>" + ghostscript.Replace("&","&amp;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;") + "</ghostscript_executable>");
                if ( !String.IsNullOrEmpty(imagemagick))
                    writer.WriteLine("  <imagemagick_executable>" + imagemagick.Trim().Replace("&", "&amp;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;") + "</imagemagick_executable>");
                writer.WriteLine("</configuration>");
                writer.Flush();
                writer.Close();


                MessageBox.Show("Configuration information successfully saved.             ", "Configuration Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch ( Exception ee )
            {
                MessageBox.Show("Error saving the configuration!              \n\n" + ee.Message);
                return false;
            }
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Khaki;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\sobekcm.config"))
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel?\n\nThe SMaRT tool will not run correctly until these values are set.    ", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void saveButton_Button_Pressed(object sender, EventArgs e)
        {
            if (Save_Configuration())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\sobekcm.config"))
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel?\n\nThe web application will not run correctly until these values are set.    ", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("You can always run this configuration again by running the executable file in the config subfolder.               ", "Configuration Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Save_Configuration())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void onlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process onlineHelp = new Process {StartInfo = {FileName = HELP_URL}};
                onlineHelp.Start();
            }
            catch
            {
                MessageBox.Show("Error launching the online help.");
            }
        }

        private void testConnectionButton_Button_Pressed(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            bool connection_succsss = SobekCM_Database.Test_Connection(connectionTextBox.Text.Trim());
            Cursor = Cursors.Default;

            MessageBox.Show(connection_succsss ? "CONNECTION SUCCESSFUL!" : "CONNECTION FAILED!");
        }

        private void databaseNamePictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Database connection string used to connect to the SobekCM database.\n\nSelect the wrench to create the MSSQL connection string.", "Database Connection String", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dbTypePictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Type of database (i.e., MSSQL or PostreSQL ).\n\nNOTE: ONLY MSSQL IS CURRENTLY SUPPORTED.", "Database Type", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void connectionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (connectionTextBox.Text.Trim().Length > 0)
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

        private void typeComboBox_Leave(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex != 0)
            {
                MessageBox.Show("Only Microsoft SQL Server databases are currently supported!   ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                typeComboBox.SelectedIndex = 0;
            }
        }

        private void createConnectionStringButton_Click(object sender, EventArgs e)
        {
            SobekCM_Database_Selection_Form createConnectionString = new SobekCM_Database_Selection_Form();
            if (createConnectionString.ShowDialog() != DialogResult.Cancel)
            {
                connectionTextBox.Text = createConnectionString.Connection_String;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About(VersionConfigSettings.AppName, VersionConfigSettings.AppVersion);
            aboutForm.ShowDialog();
        }
    }
}
