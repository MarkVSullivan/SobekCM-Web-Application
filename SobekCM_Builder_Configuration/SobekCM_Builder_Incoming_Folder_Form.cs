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
    public partial class SobekCM_Builder_Incoming_Folder_Form : Form
    {
        private DataRow thisRow;

        public SobekCM_Builder_Incoming_Folder_Form( DataRow New_Row )
        {
            thisRow = New_Row;

            InitializeComponent();

            this.DialogResult = DialogResult.Cancel;

            nameTextBox.Text = thisRow["FolderName"].ToString();
            networkTextBox.Text = thisRow["NetworkFolder"].ToString();
            errorTextBox.Text = thisRow["ErrorFolder"].ToString();
            processingTextBox.Text = thisRow["ProcessingFolder"].ToString();
            if ( thisRow["Perform_Checksum_Validation"] != DBNull.Value )
                checksumCheckBox.Checked = Convert.ToBoolean(thisRow["Perform_Checksum_Validation"]);
            if (thisRow["Archive_TIFF"] != DBNull.Value)
                archiveTiffCheckBox.Checked = Convert.ToBoolean(thisRow["Archive_TIFF"]);
            if (thisRow["Archive_All_Files"] != DBNull.Value)
                archiveAllCheckBox.Checked = Convert.ToBoolean(thisRow["Archive_All_Files"]);
            if (thisRow["Allow_Deletes"] != DBNull.Value)
                allowDeletesCheckBox.Checked = Convert.ToBoolean(thisRow["Allow_Deletes"]);
            if (thisRow["Allow_Folders_No_Metadata"] != DBNull.Value)
                allowFilesOnlyCheckBox.Checked = Convert.ToBoolean(thisRow["Allow_Folders_No_Metadata"]);
            if (thisRow["Contains_Institutional_Folders"] != DBNull.Value)
                institutionalCheckBox.Checked = Convert.ToBoolean(thisRow["Contains_Institutional_Folders"]);
        }

        private void cancelButton_Button_Pressed(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void saveButton_Button_Pressed(object sender, EventArgs e)
        {
            // Validate the values
            if (nameTextBox.Text.Trim().Length == 0)
            {
                MessageBox.Show("You must enter a folder description for this folder.     ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (networkTextBox.Text.Trim().Length == 0)
            {
                MessageBox.Show("You must enter a network location for the inbound portion of this folder.     ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!System.IO.Directory.Exists(networkTextBox.Text.Trim()))
            {
                MessageBox.Show("Network location for the inbound portion of this folder does not exist.     ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (errorTextBox.Text.Trim().Length == 0)
            {
                MessageBox.Show("You must enter a network location for the error folder.     ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!System.IO.Directory.Exists(errorTextBox.Text.Trim()))
            {
                MessageBox.Show("Network location for the error folder does not exist.     ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (processingTextBox.Text.Trim().Length == 0)
            {
                MessageBox.Show("You must enter a unique network location for the processing portion of this folder.     ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!System.IO.Directory.Exists(processingTextBox.Text.Trim()))
            {
                MessageBox.Show("Network location for the processing portion of this folder does not exist.     ", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Retrieve the values
            string foldername = nameTextBox.Text;
            string network = networkTextBox.Text;
            string error = errorTextBox.Text;
            string processing = processingTextBox.Text;
            bool checksum = checksumCheckBox.Checked;
            bool archive_tiff = archiveTiffCheckBox.Checked;
            bool archive_all = archiveAllCheckBox.Checked;
            bool allow_deletes = allowDeletesCheckBox.Checked;
            bool allow_folders = allowFilesOnlyCheckBox.Checked;
            bool institutional = institutionalCheckBox.Checked;

            // Ensure the folders end in the correct slash
            if (network[network.Length - 1] != '\\')
                network = network + '\\';
            if (error[error.Length - 1] != '\\')
                error = error + '\\';
            if (processing[processing.Length - 1] != '\\')
                processing = processing + '\\';

            // Get the id 
            int folderid = -1;
            if (thisRow["IncomingFolderId"] != DBNull.Value)
                folderid = Convert.ToInt32(thisRow["IncomingFolderId"]);

            // Save this value
            if (!Database.SobekCM_Database.Edit_Builder_Incoming_Folder(folderid, foldername, network, error, processing, checksum, archive_tiff, archive_all, allow_deletes, allow_folders, institutional))
            {
                MessageBox.Show("Error while saving edits.     ", "Error Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Close this form
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void nameHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Enter a descriptive label for this incoming folder.\n\nThis will only be used to describe the source of the material in any logs.    ", "Descriptive Name Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void networkHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Network location where new incoming packages will be written for bulk loading.   \n\nThis should be on the same directory system as the inbound and error folders, and should not be a subdirectory of either.", "Inbound Network Folder Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void processingHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Network location where packages which are currently being processed will be moved from the inbound network location.   \n\nThis should be on the same directory system as the inbound and error folders, and should not be a subdirectory of either.", "Processing Folder Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void errorHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Network location where any packages which fail validation or fail to load will be moved.   \n\nThis should be on the same directory system as the inbound and processing folders, and should not be a subdirectory of either.", "Error Folder Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checksumHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Flag indicates that if an incoming METS file has checksums, the files will be validated against those checksums.", "Checksum Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void archiveTiffHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Flag indicates that incoming digital resource TIFF files will be copied to the Archival Drop Box directory, if such a value has been set in the main system settings.\n\nThis setting has no meaning if the next flag (for archiving ALL files) has been set to TRUE.", "Archive TIFF Files Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void archiveAllHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Flag indicates that ALL incoming digital resource files will be copied to the Archival Drop Box directory, if such a value has been set in the main system settings.", "Archive All Files Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void allowDeleteHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Flag indicates if incoming METS files with a record status of DELETE should be accepted and result in the deletion of the corresponding item from this system.", "Allow Deletes Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void allowFilesOnlyHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Flag indicates that all incoming digital resources MUST be accompanied by a METS file, or they will be rejected.", "Files Without Metadata Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void institutionalHelpPictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Flag indicates that there are individual institutional folders under the main network inbound location.\n\nThis is particularly useful for a FTP location used by a number of partners.", "Institutional Subfolders Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void networkBrowsePictureBox_Click(object sender, EventArgs e)
        {
            if (networkTextBox.Text.Trim().Length > 0)
                folderBrowserDialog1.SelectedPath = networkTextBox.Text;
            folderBrowserDialog1.Description = "Select the directory for the network inbound folder for the SobekCM Builder/Bulk Loader.";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                networkTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void processingBrowsePictureBox_Click(object sender, EventArgs e)
        {
            if (processingTextBox.Text.Trim().Length > 0)
                folderBrowserDialog1.SelectedPath = processingTextBox.Text;
            folderBrowserDialog1.Description = "Select the working directory for the SobekCM Builder/Bulk Loader where packages will be moved while they are processed.";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                processingTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void errorBrowsePictureBox_Click(object sender, EventArgs e)
        {
            if (errorTextBox.Text.Trim().Length > 0)
                folderBrowserDialog1.SelectedPath = errorTextBox.Text;
            folderBrowserDialog1.Description = "Select the directory where any packages or files which fail validation or fail to load are moved.";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                errorTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

    }
}
