#region Using directives

using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using SobekCM.Library;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form is used to retrieve SobekCM_Items from a library, based on 
    /// a SobekCM search or browse URL  </summary>
    public partial class Retrieve_SobekCM_Items_Form : Form
    {
        private Thread processingThread;

        /// <summary> Constructor for a new instance of the Retrieve_SobekCM_Items_Form class </summary>
        public Retrieve_SobekCM_Items_Form()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                destinationTextBox.BorderStyle = BorderStyle.FixedSingle;
                sobekcmQueryTextBox.BorderStyle = BorderStyle.FixedSingle;
                browseButton.FlatStyle = FlatStyle.Flat;
                completeRadioButton.FlatStyle = FlatStyle.Flat;
                metsOnlyRadioButton.FlatStyle = FlatStyle.Flat;
                marcXmlRadioButton.FlatStyle = FlatStyle.Flat;
            }


            // Personalize several labels and controls now for the SobekCM Instance Name
            Text = "Retrieve " + SobekCM_Library_Settings.System_Abbreviation + " Items Form";
            mainLabel.Text = "Retrieve " + SobekCM_Library_Settings.System_Abbreviation + " Items";
            queryLabel.Text = SobekCM_Library_Settings.System_Abbreviation + " Query:";
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
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

        private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                destinationTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            if (sobekcmQueryTextBox.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please include a  " + SobekCM_Library_Settings.System_Abbreviation + " URL for a browse or search.       \n\nFor example: 'http://ufdc.ufl.edu/l/foto/results/?t=flint hall'    ", "Missing URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (( destinationTextBox.Text.Trim().Length == 0 ) || ( !Directory.Exists( destinationTextBox.Text )))
            {
                MessageBox.Show("Please select a valid destination for the " + SobekCM_Library_Settings.System_Abbreviation + " packages.    ", "Missing or Invalid Destination", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sobekcm_url = sobekcmQueryTextBox.Text.Trim().ToLower();
            string destination = destinationTextBox.Text.Trim();

            sobekcmQueryTextBox.ReadOnly = true;
            destinationTextBox.ReadOnly = true;
            completeRadioButton.Enabled = false;
            metsOnlyRadioButton.Enabled = false;
            marcXmlRadioButton.Enabled = false;
            browseButton.Enabled = false;
            okButton.Button_Enabled = false;
            exitButton.Button_Enabled = false;

            sobekcm_url = sobekcm_url.Replace("/l/", "/xml/").Replace("/dataset/", "/xml/").Replace("/json/", "/xml/");
            if (sobekcm_url.IndexOf("http://") < 0)
                sobekcm_url = sobekcm_url + "http://";
            if ( sobekcm_url.IndexOf("/xml/") < 0 )
            {
                if (sobekcm_url.IndexOf(SobekCM_Library_Settings.System_Base_URL.ToLower()) == 0)
                {
                    sobekcm_url = sobekcm_url.Replace(SobekCM_Library_Settings.System_Base_URL.ToLower(),
                                                      SobekCM_Library_Settings.System_Base_URL.ToLower() + "xml/");
                }
                else
                {
                    // Hopefully this is at the root of the web server.. we can try to inset XML in there
                    int index = sobekcm_url.IndexOf("/", 7);
                    sobekcm_url = sobekcm_url.Substring(0, index) + "/xml/" + sobekcm_url.Substring(index + 1);
                }
            }

            string web_stream = Get_Html_Page(sobekcm_url);

            if (web_stream.Length == 0)
            {
                MessageBox.Show("Invalid " + SobekCM_Library_Settings.System_Abbreviation + " Query URL was supplied.\n\nPerform requested search or browse directly in " + SobekCM_Library_Settings.System_Abbreviation + " and     \nthen copy the URL into the " + SobekCM_Library_Settings.System_Abbreviation + " query box.", "Invalid " + SobekCM_Library_Settings.System_Abbreviation + " Query Supplied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sobekcmQueryTextBox.ReadOnly = false;
                destinationTextBox.ReadOnly = false;
                completeRadioButton.Enabled = true;
                metsOnlyRadioButton.Enabled = true;
                browseButton.Enabled = true;
                okButton.Button_Enabled = true;
                exitButton.Button_Enabled = true;
                marcXmlRadioButton.Enabled = true;
                return;
            }

            // Temporary folder
            string temp_folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            temp_folder = temp_folder + "\\SMaRT Temporary";
            try
            {
                if (!Directory.Exists(temp_folder))
                    Directory.CreateDirectory(temp_folder);
            }
            catch
            {
                MessageBox.Show("Unable to create necessary directory:\n\n\t" + temp_folder, "Unable to create temporary folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sobekcmQueryTextBox.ReadOnly = false;
                destinationTextBox.ReadOnly = false;
                completeRadioButton.Enabled = true;
                metsOnlyRadioButton.Enabled = true;
                marcXmlRadioButton.Enabled = true;
                browseButton.Enabled = true;
                okButton.Button_Enabled = true;
                exitButton.Button_Enabled = true;
                return;
            }

            // Save the data to the temp folder
            try
            {
                StreamWriter writer = new StreamWriter(temp_folder + "\\" + SobekCM_Library_Settings.System_Abbreviation.ToLower() + "_download.xml", false );
                writer.Write( web_stream );
                writer.Flush();
                writer.Close();
            }
            catch
            {
                MessageBox.Show("Unable to save the downloaded data to the temporary folder:\n\n\t" + temp_folder + "\\" + SobekCM_Library_Settings.System_Abbreviation.ToLower() + "_download.xml", "Unable to create temporary file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sobekcmQueryTextBox.ReadOnly = false;
                destinationTextBox.ReadOnly = false;
                completeRadioButton.Enabled = true;
                metsOnlyRadioButton.Enabled = true;
                marcXmlRadioButton.Enabled = true;
                browseButton.Enabled = true;
                okButton.Button_Enabled = true;
                exitButton.Button_Enabled = true;
                return;
            }

            // Load this data into a dataset
            DataTable itemList = null;
            try
            {
                itemList = Read_Item_Xml( temp_folder + "\\" + SobekCM_Library_Settings.System_Abbreviation.ToLower() + "_download.xml");
            }
            catch
            {
                MessageBox.Show("Unable to load the xml data into a dataset for processing.   ", "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sobekcmQueryTextBox.ReadOnly = false;
                destinationTextBox.ReadOnly = false;
                completeRadioButton.Enabled = true;
                metsOnlyRadioButton.Enabled = true;
                marcXmlRadioButton.Enabled = true;
                browseButton.Enabled = true;
                okButton.Button_Enabled = true;
                exitButton.Button_Enabled = true;
                return;
            }

            // If there were no matching items, show a message
            if (( itemList == null ) || ( itemList.Rows.Count == 0))
            {
                MessageBox.Show("No items match your query!    ", "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sobekcmQueryTextBox.ReadOnly = false;
                destinationTextBox.ReadOnly = false;
                completeRadioButton.Enabled = true;
                metsOnlyRadioButton.Enabled = true;
                marcXmlRadioButton.Enabled = true;
                browseButton.Enabled = true;
                okButton.Button_Enabled = true;
                exitButton.Button_Enabled = true;
                return;
            }

            // Ensure the user knows what they are doing here
            DialogResult continue_test = MessageBox.Show("You are about to download " + itemList.Rows.Count + " packages from " + SobekCM_Library_Settings.System_Abbreviation.ToLower() + ".    \n\nAre you sure you would like to continue?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (continue_test != DialogResult.Yes)
            {
                sobekcmQueryTextBox.ReadOnly = false;
                destinationTextBox.ReadOnly = false;
                completeRadioButton.Enabled = true;
                metsOnlyRadioButton.Enabled = true;
                marcXmlRadioButton.Enabled = true;
                browseButton.Enabled = true;
                okButton.Button_Enabled = true;
                exitButton.Button_Enabled = true;
                return;
            }

            // Set the maximum on the progress bar
            progressBar1.Maximum = itemList.Rows.Count;

            // Determine the type of retrieval requested
            Retrieval_Type_Enum retrievalType = Retrieval_Type_Enum.METS_Only;
            if (completeRadioButton.Checked)
                retrievalType = Retrieval_Type_Enum.Complete;
            if (marcXmlRadioButton.Checked)
                retrievalType = Retrieval_Type_Enum.MARC_XML;

            // Show progress bars
            progressBar1.Show();
            if (retrievalType == Retrieval_Type_Enum.Complete) 
                progressBar2.Show();

            // Create the processor
            Retrieve_SobekCM_Items_Processor processor = new Retrieve_SobekCM_Items_Processor(itemList, destination, retrievalType);
            processor.New_Progress += processor_New_Progress;
            processor.File_Progress += processor_File_Progress;
            processor.Progress_Complete += processor_Progress_Complete;

            // Create the thread for this
            processingThread = new Thread(processor.Start);
            processingThread.Start();
        }

        private DataTable Read_Item_Xml( string fileName )
        {
            // Create the datatable to hold this data and define each column
            DataTable importItemsTable = new DataTable("Items");
            DataColumn titleIdColumn = importItemsTable.Columns.Add("TItle_ID");
            DataColumn itemIdColumn = importItemsTable.Columns.Add("Item_ID");
            DataColumn titleColumn = importItemsTable.Columns.Add("Title");
            DataColumn dateColumn = importItemsTable.Columns.Add("Date");
            DataColumn urlColumn = importItemsTable.Columns.Add("URL");
            DataColumn webColumn = importItemsTable.Columns.Add("Web_Folder");
            DataColumn networkColumn = importItemsTable.Columns.Add("Network_Folder");

            // Create the temporary values here
            string titleId = String.Empty;
            string itemId = String.Empty;
            string title = String.Empty;
            string date = String.Empty;
            string url = String.Empty;
            string web = String.Empty;
            string network = String.Empty;

            // Open a connection to the XML file and step through the XML
            XmlTextReader reader = new XmlTextReader(new StreamReader(fileName));
            while (reader.Read())
            {
                // What type of XML node is this?
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "ItemResult":
                            if (reader.MoveToAttribute("ID"))
                                itemId = reader.Value;
                            break;

                        case "Title":
                            reader.Read();
                            title = reader.Value;
                            break;

                        case "Date":
                            reader.Read();
                            date = reader.Value;
                            break;

                        case "URL":
                            reader.Read();
                            url = reader.Value;
                            break;

                        case "Folder":
                            if (reader.MoveToAttribute("type"))
                            {
                                if (reader.Value == "web")
                                {
                                    reader.Read();
                                    web = reader.Value;
                                }
                                else if (reader.Value == "network")
                                {
                                    reader.Read();
                                    network = reader.Value;
                                }
                            }
                            break;

                        case "TitleResult":
                            if (reader.MoveToAttribute("ID"))
                                titleId = reader.Value;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // Is this ending the title or an item within the title?
                    switch (reader.Name)
                    {
                        case "ItemResult":
                            // Create the new row and assign all the values
                            DataRow newRow = importItemsTable.NewRow();
                            newRow[titleIdColumn] = titleId;
                            newRow[itemIdColumn] = itemId;
                            newRow[titleColumn] = title;
                            newRow[dateColumn] = date;
                            newRow[urlColumn] = url;
                            newRow[webColumn] = web;
                            newRow[networkColumn] = network;
                            importItemsTable.Rows.Add(newRow);

                            // Now, clear out all the item-level data
                            itemId = String.Empty;
                            title = String.Empty;
                            date = String.Empty;
                            url = String.Empty;
                            web = String.Empty;
                            network = String.Empty;
                            break;


                        case "TitleResult":
                            // Clear out the last title bit of information
                            titleId = String.Empty;
                            break;

                    }
                }
            }
            reader.Close();

            return importItemsTable;
        }


        void processor_File_Progress(int fileCount, int maxFiles)
        {
            if (progressBar2 != null)
            {
                progressBar2.Maximum = maxFiles;
                progressBar2.Value = fileCount;
            }
        }

        void processor_Progress_Complete( int errorCount )
        {
            // Show message
            if (errorCount > 0)
            {
                MessageBox.Show("Process complete with " + errorCount + " errors.   ");
            }
            else
            {
                MessageBox.Show("Process complete with no errors.   ");
            }

            // Show this folder
            Process process = new Process {StartInfo = {FileName = destinationTextBox.Text.Trim()}};
            process.Start();

            // Close the form
            Close();
        }

        void processor_New_Progress(int currentItem)
        {
            progressBar1.Value = currentItem;
        }

        private string Get_Html_Page(string strURL)
        {
            try
            {
                // the html retrieved from the page
                WebRequest objRequest = WebRequest.Create(strURL);
                WebResponse objResponse = objRequest.GetResponse();
                Stream objStream = objResponse.GetResponseStream();

                if (objStream != null)
                {
                    // the using keyword will automatically dispose the object 
                    // once complete
                    string strResult;
                    using (StreamReader sr = new StreamReader(objStream))
                    {
                        strResult = sr.ReadToEnd();
                        // Close and clean up the StreamReader
                        sr.Close();
                    }
                    return strResult;
                }
                return String.Empty;
            }
            catch 
            {
                return String.Empty;
            }
        }

        private void completeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (completeRadioButton.Checked)
            {
                try
                {
                    string[] dirs = Directory.GetDirectories(@"\\cns-uflib-ufdc\UFDC");
                }
                catch
                {
                    MessageBox.Show("You do not appear to have appropriate read rights on the image share.    \n\nIf you should have read access, please put in a GROVER for read-only access.       ", "Insufficient Priviledges Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    metsOnlyRadioButton.Checked = true;
                }
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
    }
}
