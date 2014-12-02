#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SobekCM.Builder_Library.Statistics;
using SobekCM.Core.Configuration;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.ApplicationState;

#endregion

namespace SobekCM_Stats_Reader
{
    public partial class Stats_Setup_Form : Form
    {
        private Thread workerThread;
        private bool inProcess = false;

        public Stats_Setup_Form()
        {
            InitializeComponent();

            // Set the last values
            logTextBox.Text = Stats_Reader_UserSettings.IIS_Web_Log_Directory;
            sqlTextBox.Text = Stats_Reader_UserSettings.SQL_Output_Directory;
            if (Stats_Reader_UserSettings.DataSet_Directory.Length > 0)
                datasetTextBox.Text = Stats_Reader_UserSettings.DataSet_Directory;
            else
                datasetTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SobekCM Stats Reader";
            dbTextBox.Text = Stats_Reader_UserSettings.Database_Connection_String;
            sobekTextBox.Text = Stats_Reader_UserSettings.SobekCM_Web_Application_Directory;


            // Set the flatness
            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                outputScriptRadioButton.FlatStyle = FlatStyle.Flat;
                updateDbRadioButton.FlatStyle = FlatStyle.Flat;
                logTextBox.BorderStyle = BorderStyle.FixedSingle;
                sqlTextBox.BorderStyle = BorderStyle.FixedSingle;
                year1TextBox.BorderStyle = BorderStyle.FixedSingle;
                month1TextBox.BorderStyle = BorderStyle.FixedSingle;
                year2TextBox.BorderStyle = BorderStyle.FixedSingle;
                month2TextBox.BorderStyle = BorderStyle.FixedSingle;
                datasetTextBox.BorderStyle = BorderStyle.FixedSingle;
                dbTextBox.BorderStyle = BorderStyle.FixedSingle;
                sobekTextBox.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void sqlBrowseButton2_Button_Pressed(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = sqlTextBox.Text;
            folderBrowserDialog1.Description = "Select folder where the output SQL scripts should be written.";
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                sqlTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void datasetBrowseButton2_Button_Pressed(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = datasetTextBox.Text;
            folderBrowserDialog1.Description = "Select folder where the temporary datasets should be written.";
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                datasetTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void logBrowseButton2_Button_Pressed(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = logTextBox.Text;
            folderBrowserDialog1.Description = "Select the folder where the IIS web logs reside.";
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                logTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void sobekBrowseButton_Button_Pressed(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = sobekTextBox.Text;
            folderBrowserDialog1.Description = "Select the folder where the SobekCM Web application resides.";
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                sobekTextBox.Text = folderBrowserDialog1.SelectedPath;
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

        #region Method to draw the form background

        /// <summary> Method is called whenever this form is resized. </summary>
        /// <param name="e"></param>
        /// <remarks> This redraws the background of this form </remarks>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Get rid of any current background image
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
                BackgroundImage = null;
            }

            if (ClientSize.Width > 0)
            {
                // Create the items needed to draw the background
                Bitmap image = new Bitmap(ClientSize.Width, ClientSize.Height);
                Graphics gr = Graphics.FromImage(image);
                Rectangle rect = new Rectangle(new Point(0, 0), ClientSize);

                // Create the brush
                LinearGradientBrush brush = new LinearGradientBrush(rect, BackColor, ControlPaint.Dark(BackColor), LinearGradientMode.Vertical);
                brush.SetBlendTriangularShape(0.33F);

                // Create the image
                gr.FillRectangle(brush, rect);
                gr.Dispose();

                // Set this as the backgroundf
                BackgroundImage = image;
            }
        }

        #endregion

        private void okRoundButton_Button_Pressed(object sender, EventArgs e)
        {
            // Validate the database connection string
            string connectString = dbTextBox.Text.Trim();
            if (connectString.Length == 0)
            {
                MessageBox.Show("You must enter a valid connection string for the SobekCM database.");
                return;
            }
            if (!SobekCM.Library.Database.SobekCM_Database.Test_Connection(connectString))
            {
                MessageBox.Show("Unable to connect to the database.  Check connection string and try again.");
                return;
            }

            SobekCM.Library.Database.SobekCM_Database.Connection_String = connectString;
           

            // validate the directory entries
            if ((logTextBox.Text.Trim().Length == 0) || (!Directory.Exists(logTextBox.Text.Trim())))
            {
                MessageBox.Show("You must enter a valid directory for the IIS web logs which should be read.");
                return;
            }
            if ((sqlTextBox.Text.Trim().Length == 0) || (!Directory.Exists(sqlTextBox.Text.Trim())))
            {
                MessageBox.Show("You must enter a valid directory for the IIS web logs which should be read.");
                return;
            }
            if ((sobekTextBox.Text.Trim().Length == 0) || (!Directory.Exists(sobekTextBox.Text.Trim())))
            {
                MessageBox.Show("You must enter a valid directory for the SobekCM Web Application directory.");
                return;
            }

            // Get the directory information
            string log_directory = logTextBox.Text.Trim();
            string sql_directory = sqlTextBox.Text.Trim();
            string sobekcm_directory = sobekTextBox.Text.Trim();

            // Validate the months and years
            int year1 = -1;
            int month1 = -1;
            int year2 = -1;
            int month2 = -1;
            if (!Int32.TryParse(year1TextBox.Text, out year1))
            {
                MessageBox.Show("The first year in the date range must be a valid number.");
                return;
            }
            if (!Int32.TryParse(month1TextBox.Text, out month1))
            {
                MessageBox.Show("The first month in the date range must be a valid number.");
                return;
            }
            if ((year1 < 2000) || (year1 > DateTime.Now.Year))
            {
                MessageBox.Show("The first year value is not a valid year.");
                return;
            }
            if ((month1 < 1) || (month1 > 12))
            {
                MessageBox.Show("The first month value is not a valid month.");
                return;
            }

            // Only deal with the second years and months if they have values
            if ((year2TextBox.Text.Trim().Length > 0) && (month2TextBox.Text.Trim().Length > 0))
            {
                if (!Int32.TryParse(year2TextBox.Text, out year2))
                {
                    MessageBox.Show("The second year in the date range must be a valid number.");
                    return;
                }
                if (!Int32.TryParse(month2TextBox.Text, out month2))
                {
                    MessageBox.Show("The second month in the date range must be a valid number.");
                    return;
                }
                if ((year2 < 2000) || (year2 > DateTime.Now.Year))
                {
                    MessageBox.Show("The second year value is not a valid year.");
                    return;
                }
                if ((month2 < 1) || (month2 > 12))
                {
                    MessageBox.Show("The second month value is not a valid month.");
                    return;
                }
            }

            // Was a range selected?  if so, make sure the earliest month/year combo 
            // is first
            if ((year2 >= 2000) && (month2 > 0))
            {
                if (year2 < year1)
                {
                    int temp1 = year1;
                    int temp2 = month1;
                    year1 = year2;
                    month1 = month2;
                    year2 = temp1;
                    month2 = temp2;
                }
                else if (year2 == year1)
                {
                    if (month2 < month1)
                    {
                        int temp = month1;
                        month1 = month2;
                        month2 = temp;
                    }
                }
            }

            // The web space should end in a '\'
            if (sobekcm_directory[sobekcm_directory.Length - 1] != '\\')
                sobekcm_directory = sobekcm_directory + "\\";

            // Get the temporary workspace 
            string temporary_workspace_default = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SobekCM Stats Reader";
            string temporary_workspace = temporary_workspace_default;
            if ((datasetTextBox.Text.Trim().Length > 0) && (Directory.Exists((datasetTextBox.Text.Trim()))))
            {
                temporary_workspace = datasetTextBox.Text.Trim();
            }
            if (!Directory.Exists(temporary_workspace))
            {
                try
                {
                    Directory.CreateDirectory(temporary_workspace);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to create the temporary workspace directory.\n\n" + temporary_workspace);
                    return;
                }
            }

            // Create the array of year-months
            List<string> year_month_list = new List<string>();
            if ((year2 == -1) || (month2 == -1))
            {
                year_month_list.Add(year1 + month1.ToString().PadLeft(2, '0'));
            }
            else
            {
                int iterator_year = year1;
                int iterator_month = month1;
                while (((iterator_month <= month2) && (iterator_year == year2)) || (iterator_year < year2))
                {
                    year_month_list.Add(iterator_year + iterator_month.ToString().PadLeft(2, '0'));
                    iterator_month++;
                    if (iterator_month > 12)
                    {
                        iterator_month = 1;
                        iterator_year++;
                    }
                }
            }
            string[] year_month = year_month_list.ToArray();

            // Save the values as last picked values
            Stats_Reader_UserSettings.DataSet_Directory = temporary_workspace;
            Stats_Reader_UserSettings.IIS_Web_Log_Directory = log_directory;
            Stats_Reader_UserSettings.SQL_Output_Directory = sql_directory;
            Stats_Reader_UserSettings.Database_Connection_String = connectString;
            Stats_Reader_UserSettings.SobekCM_Web_Application_Directory = sobekcm_directory;
            Stats_Reader_UserSettings.Save();

            // Also, assign the database connection string
            SobekCM.Library.Database.SobekCM_Database.Connection_String = connectString;
            SobekCM.Engine_Library.Database.Engine_Database.Connection_String = connectString;

            // Create the db instance
            Database_Instance_Configuration dbInstance = new Database_Instance_Configuration();
            dbInstance.Connection_String = connectString;
            dbInstance.Database_Type = SobekCM.Core.Database.SobekCM_Database_Type_Enum.MSSQL;
            Engine_ApplicationCache_Gateway.RefreshAll(dbInstance);

            // Turn off the button
            okRoundButton.Button_Enabled = false;

            mainPanel.Hide();
            resultPanel.Show();

            // Create the processor
            SobekCM_Stats_Reader_Processor processor = new SobekCM_Stats_Reader_Processor(log_directory, sql_directory, temporary_workspace, sobekcm_directory, year_month);
            processor.New_Status += new SobekCM_Stats_Reader_Processor_New_Status_Delegate(processor_New_Status);

            // Create the thread
            workerThread = new Thread(new ThreadStart(processor.Process_IIS_Logs));
            workerThread.Start();

            // Set flag
            inProcess = true;

        }

        void processor_New_Status(string new_message)
        {
            richTextBox1.AppendText(new_message + "\n");

            if (new_message == "COMPLETE!")
            {
                cancelRoundButton.Button_Text = "EXIT";
                inProcess = false;
            }

        }

        private void cancelRoundButton_Button_Pressed(object sender, EventArgs e)
        {
            if (inProcess)
            {
                DialogResult result = MessageBox.Show("Are you sure yo uwant to abort the current process?", "Confirm Abort", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if ( result != DialogResult.Yes )
                    return;

                try
                {
                    workerThread.Abort();
                }
                catch (Exception)
                {
                    // Suppress
                }
            }
            this.Close();
        }

        private void helpPictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                Process newProcess = new Process();
                newProcess.StartInfo.FileName = "http://ufdc.ufl.edu/software/statsreader";
                newProcess.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Error showing online help at: \n\nhttp://ufdc.ufl.edu/software/statsreader");

            }

        }

        private void databaseNamePictureBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Database connection string used to connect to the SobekCM database.\n\nSelect the wrench to create the MSSQL connection string.", "Database Connection String", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void createConnectionStringButton_Click(object sender, EventArgs e)
        {
            SobekCM_Database_Selection_Form createConnectionString = new SobekCM_Database_Selection_Form(false);
            if (createConnectionString.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                dbTextBox.Text = createConnectionString.Connection_String;
            }
        }






    }
}