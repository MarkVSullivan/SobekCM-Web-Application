using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Configuration
{
    public partial class SobekCM_Builder_Config_Edit_Form : Form
    {
        private bool during_install;
        private bool errorReading = false;
        private string db_type = "MSSQL";
        private string connection_string = String.Empty;
        private string error_emails = String.Empty;
        private string error_page = String.Empty;
        private string ghostscript = String.Empty;
        private string imagemagick = String.Empty;

        public SobekCM_Builder_Config_Edit_Form( bool during_install )
        {
            InitializeComponent();

            this.during_install = during_install;
            if (during_install)
            {
                testConnectionButton.Hide();
                builderFoldersButton.Hide();

            }

            // Try to read a config file
            try
            {
                // Is there a valid config file?
                string dir = Application.StartupPath;
                if (File.Exists(dir + "\\sobekcm.config"))
                    Read_Configuration_File(dir + "\\sobekcm.config");
                else if (File.Exists(dir + "\\sample.config"))
                    Read_Configuration_File(dir + "\\sample.config");
            }
            catch { }


            // Set the current values as well
            imageMagickComboBox.Text = imagemagick;
            imageMagickComboBox.Items.Add(imagemagick);
            ghostscriptComboBox.Text = ghostscript;
            ghostscriptComboBox.Items.Add(ghostscript);
            connectionTextBox.Text = connection_string;
            if (db_type.ToLower() == "postgresql")
                typeComboBox.SelectedIndex = 1;
            else
                typeComboBox.SelectedIndex = 0;

            // Check the SAVE button
            if (connection_string.Trim().Length == 0)
                saveButton.Button_Enabled = false;

            try
            {
                // Look for the ghostscript folder(s)
                string mainFolder = "C:\\Program Files\\gs";
                if (Directory.Exists(mainFolder))
                {
                    string[] subfolders = Directory.GetDirectories(mainFolder);
                    foreach (string subfolder in subfolders)
                    {
                        string[] files = Directory.GetFiles(subfolder + "\\bin", "gswin*c.exe");
                        if (files.Length > 0)
                        {
                            foreach (string thisFile in files)
                            {
                                if (String.Compare(ghostscript, thisFile, true) != 0)
                                    ghostscriptComboBox.Items.Add(thisFile);
                            }
                        }
                        else
                        {
                            files = Directory.GetFiles(subfolder + "\\bin", "gswin*.exe");
                            foreach (string thisFile in files)
                            {
                                if (String.Compare(ghostscript, thisFile, true) != 0)
                                    ghostscriptComboBox.Items.Add(thisFile);
                            }
                        }
                    }
                }
                mainFolder = "C:\\Program Files (x86)\\gs";
                if (Directory.Exists(mainFolder))
                {
                    string[] subfolders = Directory.GetDirectories(mainFolder);
                    foreach (string subfolder in subfolders)
                    {
                        string[] files = Directory.GetFiles(subfolder + "\\bin", "gswin*c.exe");
                        if (files.Length > 0)
                        {
                            foreach (string thisFile in files)
                            {
                                if (String.Compare(ghostscript, thisFile, true) != 0)
                                    ghostscriptComboBox.Items.Add(thisFile);
                            }
                        }
                        else
                        {
                            files = Directory.GetFiles(subfolder + "\\bin", "gswin*.exe");
                            foreach (string thisFile in files)
                            {
                                if (String.Compare(ghostscript, thisFile, true) != 0)
                                    ghostscriptComboBox.Items.Add(thisFile);
                            }
                        }
                    }
                }
                // Look for the imagemagick folder(s)
                mainFolder = "C:\\Program Files";
                if (Directory.Exists(mainFolder))
                {
                    string[] subfolders = Directory.GetDirectories(mainFolder, "ImageMagick*");
                    foreach (string subfolder in subfolders)
                    {
                        string[] files = Directory.GetFiles(subfolder, "convert.exe");
                        if (files.Length > 0)
                        {
                            foreach (string thisFile in files)
                            {
                                if (String.Compare(imagemagick, thisFile, true) != 0)
                                    imageMagickComboBox.Items.Add(thisFile);
                            }
                        }
                    }
                }
                mainFolder = "C:\\Program Files (x86)";
                if (Directory.Exists(mainFolder))
                {
                    string[] subfolders = Directory.GetDirectories(mainFolder, "ImageMagick*");
                    foreach (string subfolder in subfolders)
                    {
                        string[] files = Directory.GetFiles(subfolder, "convert.exe");
                        if (files.Length > 0)
                        {
                            foreach (string thisFile in files)
                            {
                                if (String.Compare(imagemagick, thisFile, true) != 0)
                                    imageMagickComboBox.Items.Add(thisFile);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Just lookig for alternative options for executable files, so no 
                // real need to throw a message for this yet
            }

        }

        private void Read_Configuration_File(string config_file)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(config_file);
            System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(reader);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    string node_name = xmlReader.Name.ToLower();
                    switch (node_name)
                    {
                        case "connection_string":
                            if (xmlReader.MoveToAttribute("type"))
                            {
                                db_type = xmlReader.Value.ToString();

                            }
                            xmlReader.Read();
                            connection_string = xmlReader.Value;
                            break;

                        case "error_emails":
                            xmlReader.Read();
                            error_emails = xmlReader.Value;
                            break;

                        case "error_page":
                            xmlReader.Read();
                            error_page = xmlReader.Value;
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

        public bool Error_Reading
        {
            get { return errorReading; }
        }


        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\sobekcm.config"))
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel?\n\nThe builder will not run correctly until these values are set.    ", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("You can always run this configuration again by either:\n\n\t1) Run the builder with a --config argument\n\t2) Run the executable file in the config subfolder               ", "Configuration Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void ghostscriptBrowsePictureBox_Click(object sender, EventArgs e)
        {
            if ((ghostscriptComboBox.Text.Length > 0) && (System.IO.File.Exists(ghostscriptComboBox.Text)))
            {
                FileInfo thisFileInfo = new FileInfo(ghostscriptComboBox.Text);
                openFileDialog1.FileName = thisFileInfo.Name;
                openFileDialog1.InitialDirectory = thisFileInfo.Directory.ToString();
            }
            else
            {
                openFileDialog1.FileName = "";
            }
            openFileDialog1.Title = "Navigate to the Ghostscript installation folder and select the executable file.";
            openFileDialog1.Filter = "Ghostscript File|gswin*.exe";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
                ghostscriptComboBox.Text = openFileDialog1.FileName;
        }

        private void imageMagickBrowsePictureBox_Click(object sender, EventArgs e)
        {
            if ((imageMagickComboBox.Text.Length > 0) && (System.IO.File.Exists(imageMagickComboBox.Text)))
            {
                FileInfo thisFileInfo = new FileInfo(imageMagickComboBox.Text);
                openFileDialog1.FileName = thisFileInfo.Name;
                openFileDialog1.InitialDirectory = thisFileInfo.Directory.ToString();
            }
            else
            {
                openFileDialog1.FileName = "convert.exe";
            }
            openFileDialog1.Title = "Navigate to the ImageMagick installation folder and select the 'convert.exe' file.";
            openFileDialog1.Filter = "ImageMagick File|convert.exe";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
                imageMagickComboBox.Text = openFileDialog1.FileName;
        }

        private void serverNamePictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Type of database (i.e., MSSQL or PostreSQL ).\n\nNOTE: ONLY MSSQL IS CURRENTLY SUPPORTED.", "Database Type", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void databaseNamePictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Database connection string used to connect to the SobekCM database.\n\nSelect the wrench to create the MSSQL connection string.", "Database Connection String", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ghostscriptHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Path and name for the Ghostscript executable file, which is used to convert PDFs into TIFFs.\n\nThe file name is usually either 'gswin32c.exe' or 'gswin64c.exe'.\n\nPress the magnifying glass to the left to browse to find the application.", "Ghostscript Executable File", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void imagemagickHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Path and name for the ImageMagick executable file, which is used for image manipulation and jpeg ccreation.\n\nThe filename is 'convert.exe'.\n\nPress the magnifying glass to the left to browse to find the application.", "ImageMagick Executable File", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void saveButton_Button_Pressed(object sender, EventArgs e)
        {
            try
            {
                StreamWriter writer = new StreamWriter(Application.StartupPath + "\\sobekcm.config", false);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
                writer.WriteLine("<SobekCM_Config xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
				writer.WriteLine("	xmlns=\"http://sobekrepository.org/schemas/sobekcm_config\"");
				writer.WriteLine("  xsi:schemaLocation=\"http://sobekrepository.org/schemas/sobekcm_config ");
				writer.WriteLine("    http://sobekrepository.org/schemas/sobekcm_config.xsd\">");
				writer.WriteLine("  <Connections>");
                writer.WriteLine("    <Connection_String type=\"" + db_type + "\">" + connectionTextBox.Text.Trim().Replace("&", "&amp;").Replace("\"", "&quot;").Replace(">","&gt;").Replace("<","&lt;") + "</Connection_String>");
				writer.WriteLine("  </Connections>");
				//if ( error_emails.Length > 0 )
				//	writer.WriteLine("  <error_emails>" + error_emails + "</error_emails>");
				//if ( error_page.Length > 0 )
				//	writer.WriteLine("  <error_page>" + error_page + "</error_page>");
				writer.WriteLine("  <Builder>");
                writer.WriteLine("    <Ghostscript_Executable>" + ghostscriptComboBox.Text.Trim().Replace("&", "&amp;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;") + "</Ghostscript_Executable>");
                writer.WriteLine("    <Imagemagick_Executable>" + imageMagickComboBox.Text.Trim().Replace("&", "&amp;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;") + "</Imagemagick_Executable>");
				writer.WriteLine("  </Builder>");
				writer.WriteLine("</SobekCM_Config>");
                writer.Flush();
                writer.Close();


                MessageBox.Show("Configuration information successfully saved.\n\nYou can always run this configuration again by either:\n\n\t1) Run the builder with a --config argument\n\t2) Run the executable file in the config subfolder               ", "Configuration Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch ( Exception ee )
            {
                MessageBox.Show("Error saving the configuration!              \n\n" + ee.Message);
            }
        }

        private void comboBox_Enter(object sender, EventArgs e)
        {
            ((ComboBox)sender).BackColor = System.Drawing.Color.Khaki;
        }

        private void comboBox_Leave(object sender, EventArgs e)
        {
            ((ComboBox)sender).BackColor = System.Drawing.Color.White;
        }


        private void textBox_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = System.Drawing.Color.Khaki;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = System.Drawing.Color.White;

        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if ( connectionTextBox.Text.Trim().Length > 0) 
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
            this.Cursor = Cursors.WaitCursor;
            bool connection_succsss = Database.SobekCM_Database.Test_Connection( connectionTextBox.Text.Trim());
            this.Cursor = Cursors.Default;

            if (connection_succsss)
                MessageBox.Show("CONNECTION SUCCESSFUL!");
            else
                MessageBox.Show("CONNECTION FAILED!");

        }

        private void createConnectionStringButton_Click(object sender, EventArgs e)
        {
            SobekCM_Database_Selection_Form createConnectionString = new SobekCM_Database_Selection_Form( during_install );
            if (createConnectionString.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                connectionTextBox.Text = createConnectionString.Connection_String;
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

        private void builderFoldersButton_Button_Pressed(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            bool connection_succsss = Database.SobekCM_Database.Test_Connection(connectionTextBox.Text.Trim());
            this.Cursor = Cursors.Default;

            if (connection_succsss)
            {
                Database.SobekCM_Database.Database_String = connectionTextBox.Text.Trim();
                SobekCM_Builder_Folders_Form folderForm = new SobekCM_Builder_Folders_Form();
                this.Hide();
                folderForm.ShowDialog();

                this.Show();
            }
            else
                MessageBox.Show("CONNECTION FAILED!");
        }

    }
}
