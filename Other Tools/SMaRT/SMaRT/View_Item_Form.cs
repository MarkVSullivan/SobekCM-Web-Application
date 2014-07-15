#region Using directives

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using DLC.Custom_Grid;
using SobekCM.Resource_Object.Tracking;
using SobekCM.Library;
using SobekCM.Library.Database;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form is used to display all the details about a single item from within a title 
    /// from the SobekCM system </summary>
    public partial class View_Item_Form : Form
    {
        #region Private form members

        private readonly CustomGrid_Panel aggrPanel;
        private readonly CustomGrid_Panel archivePanel;
        private readonly string bibid;
        private readonly int itemid;
        private readonly CustomGrid_Panel mediaPanel;
        private readonly string vid;
        private BibIdReport_Printer bibIdPrinter;
        private CustomGrid_Panel historyPanel;
        private DataSet trackingInfo;
        private Tracking_Info trackingInfoObj;

        #endregion

        #region Constructor

        /// <summary> Constructor for a new instance of the View_Item_Form form </summary>
        /// <param name="ItemID"> Primary key for this item in the SobekCM database </param>
        /// <param name="BibID"> Bibliographic identifier for the title related to this item </param>
        /// <param name="VID"> Volume identifier for this item within the title </param>
        public View_Item_Form( int ItemID, string BibID, string VID )
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            bibid = BibID;
            vid = VID;
            itemid = ItemID;

            mainLinkLabel.Text = BibID + ":" + VID;

            show_milestones_worklog();

            // Show the media table
            if (trackingInfo.Tables[0].Rows.Count > 0)
            {
                mediaPanel = new CustomGrid_Panel
                                 {
                                     Size = new Size(mediaTabPage.Width - 20, mediaTabPage.Height - 20),
                                     Location = new Point(10, 10)
                                 };
                mediaPanel.Style.Default_Column_Width = 80;
                mediaPanel.Style.Default_Column_Color = Color.LightBlue;
                mediaPanel.Style.Header_Back_Color = Color.DarkBlue;
                mediaPanel.Style.Header_Fore_Color = Color.White;
                mediaPanel.BackColor = Color.White;
                mediaPanel.BorderStyle = BorderStyle.FixedSingle;
                mediaPanel.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
                mediaTabPage.Controls.Add(mediaPanel);
                mediaPanel.DataTable = trackingInfo.Tables[0];
                mediaPanel.Style.Column_Styles[1].Width = 400;
                mediaPanel.Style.Column_Styles[1].BackColor = Color.White;
                mediaPanel.Style.Column_Styles[2].Width = 70;
                mediaPanel.Style.Column_Styles[3].Width = 70;
                mediaPanel.Style.Column_Styles[4].Width = 90;
            }
            else
            {
                tabControl1.TabPages.Remove(mediaTabPage);
            }

            // Show the archives table
            if (trackingInfo.Tables[2].Rows.Count > 0)
            {
                archivePanel = new CustomGrid_Panel
                                   {
                                       Size = new Size(archiveTabPage.Width - 20, archiveTabPage.Height - 20),
                                       Location = new Point(10, 10)
                                   };
                archivePanel.Style.Default_Column_Width = 80;
                archivePanel.Style.Default_Column_Color = Color.LightBlue;
                archivePanel.Style.Header_Back_Color = Color.DarkBlue;
                archivePanel.Style.Header_Fore_Color = Color.White;
                archivePanel.BackColor = Color.White;
                archivePanel.BorderStyle = BorderStyle.FixedSingle;
                archivePanel.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
                archiveTabPage.Controls.Add(archivePanel);
                archivePanel.DataTable = trackingInfo.Tables[2];
                archivePanel.Style.Column_Styles[0].Visible = false;
                archivePanel.Style.Column_Styles[1].Visible = false;
                archivePanel.Style.Column_Styles[2].Visible = false;
                archivePanel.Style.Column_Styles[3].Width = 300;
                archivePanel.Style.Column_Styles[3].BackColor = Color.White;
                archivePanel.Style.Column_Styles[5].Width = 140;
                archivePanel.Style.Column_Styles[5].Header_Text = "Last Modified";
                archivePanel.Style.Column_Styles[6].Width = 140;
                archivePanel.Style.Column_Styles[6].Header_Text = "Date Archived";
            }
            else
            {
                tabControl1.TabPages.Remove(archiveTabPage);
                archiveRetrieveButton.Button_Enabled = false;
            }

            // Show the aggregation membership
            if (( trackingInfo.Tables.Count > 4 ) && ( trackingInfo.Tables[4].Rows.Count > 0))
            {
                aggrPanel = new CustomGrid_Panel
                                {
                                    Size = new Size(aggregationsTabPage.Width - 20, aggregationsTabPage.Height - 20),
                                    Location = new Point(10, 10)
                                };
                aggrPanel.Style.Default_Column_Width = 80;
                aggrPanel.Style.Default_Column_Color = Color.LightBlue;
                aggrPanel.Style.Header_Back_Color = Color.DarkBlue;
                aggrPanel.Style.Header_Fore_Color = Color.White;
                aggrPanel.BackColor = Color.White;
                aggrPanel.BorderStyle = BorderStyle.FixedSingle;
                aggrPanel.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
                aggregationsTabPage.Controls.Add(aggrPanel);
                aggrPanel.DataTable = trackingInfo.Tables[4];
                aggrPanel.Style.Column_Styles[0].Width = 100;
                aggrPanel.Style.Column_Styles[1].Width = 200;
                aggrPanel.Style.Column_Styles[1].BackColor = Color.White;
                aggrPanel.Style.Column_Styles[2].Width = 200;
                aggrPanel.Style.Column_Styles[2].BackColor = Color.White;
                aggrPanel.Style.Column_Styles[2].Header_Text = "Short Name";
                aggrPanel.Style.Column_Styles[3].Width = 100;
                aggrPanel.Style.Column_Styles[4].Width = 65;
                aggrPanel.Style.Column_Styles[4].Header_Text = "Implied?";
                aggrPanel.Style.Column_Styles[5].Width = 60;
                aggrPanel.Style.Column_Styles[5].Header_Text = "Hidden?";
                aggrPanel.Style.Column_Styles[6].Width = 60;
                aggrPanel.Style.Column_Styles[6].Header_Text = "Active?";
            }
            else
            {
                tabControl1.TabPages.Remove(aggregationsTabPage);
            }

            // Pull the IP restriction, dark info, etc..
            DataRow volumeInfo = trackingInfo.Tables[3].Rows[0];
            titleLabel.Text = volumeInfo["Title"].ToString();

            authorLabel.Text = volumeInfo["Author"].ToString();
            if (authorLabel.Text.Length == 0)
            {
                authorLabel.Text = "( none )";
                authorLabel.ForeColor = Color.Gray;
            }
            publisherLabel.Text = volumeInfo["Publisher"].ToString();
            if (publisherLabel.Text.Length == 0)
            {
                publisherLabel.Text = "( none )";
                publisherLabel.ForeColor = Color.Gray;
            }

            dateLabel.Text = volumeInfo["PubDate"].ToString();
            if (dateLabel.Text.Length == 0)
            {
                dateLabel.Text = "( none )";
                dateLabel.ForeColor = Color.Gray;
            }

            level1Label.Text = volumeInfo["Level1_Text"].ToString();
            if (level1Label.Text.Length == 0)
            {
                level1Label.Text = "( none )";
                level1Label.ForeColor = Color.Gray;
            }

            level2Label.Text = volumeInfo["Level2_Text"].ToString();
            if (level2Label.Text.Length == 0)
            {
                level2Label.Text = "( none )";
                level2Label.ForeColor = Color.Gray;
            }

            level3Label.Text = volumeInfo["Level3_Text"].ToString();
            if (level3Label.Text.Length == 0)
            {
                level3Label.Text = "( none )";
                level3Label.ForeColor = Color.Gray;
            }

            // Set the restriction info
            int restriction = Convert.ToInt16(volumeInfo["IP_Restriction_Mask"]);
            bool dark = Convert.ToBoolean(volumeInfo["Dark"]);

            // Add the directory
            directoryLinkLabel.Text = SobekCM_Library_Settings.Image_Server_Network + bibid.Substring(0, 2) + "\\" + bibid.Substring(2, 2) + "\\" + bibid.Substring(4, 2) + "\\" + bibid.Substring(6, 2) + "\\" + bibid.Substring(8, 2) + "\\" + vid;

            if (dark)
            {
                visibilityPictureBox.Image = imageList1.Images[3];
            }
            else
            {
                if (restriction < 0)
                {
                    visibilityPictureBox.Image = imageList1.Images[0];
                }
                else
                {
                    visibilityPictureBox.Image = restriction == 0 ? imageList1.Images[2] : imageList1.Images[1];
                }
            }

            // Set the form size correctly
            Size = SMaRT_UserSettings.View_Item_Form_Size;
            int screen_width = Screen.PrimaryScreen.WorkingArea.Width;
            int screen_height = Screen.PrimaryScreen.WorkingArea.Height;
            if ((Width > screen_width) || (Height > screen_height) || ( SMaRT_UserSettings.View_Item_Form_Maximized ))
                WindowState = FormWindowState.Maximized;

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                titleLabel.BackColor = SystemColors.Control;
                authorLabel.BackColor = SystemColors.Control;
                publisherLabel.BackColor = SystemColors.Control;
                dateLabel.BackColor = SystemColors.Control;
                level1Label.BackColor = SystemColors.Control;
                level2Label.BackColor = SystemColors.Control;
                level3Label.BackColor = SystemColors.Control;

            }
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        private void show_milestones_worklog()
        {
            // Pull the tracking information
            trackingInfo = SobekCM_Database.Tracking_Get_History_Archives(itemid, null);

            // Pull out the standard tracking milestones
            trackingInfoObj = new Tracking_Info();
            trackingInfoObj.Set_Tracking_Info(trackingInfo);
            if (trackingInfoObj.Digital_Acquisition_Milestone.HasValue)
                acquisitionLabel.Text = trackingInfoObj.Digital_Acquisition_Milestone.Value.ToShortDateString();
            if (trackingInfoObj.Image_Processing_Milestone.HasValue)
                imageProcessingLabel.Text = trackingInfoObj.Image_Processing_Milestone.Value.ToShortDateString();
            if (trackingInfoObj.Quality_Control_Milestone.HasValue)
                qcLabel.Text = trackingInfoObj.Quality_Control_Milestone.Value.ToShortDateString();
            if (trackingInfoObj.Online_Complete_Milestone.HasValue)
                onlineCompleteLabel.Text = trackingInfoObj.Online_Complete_Milestone.Value.ToShortDateString();

            if (trackingInfoObj.Born_Digital)
            {
                if (trackingInfoObj.Material_Received_Date.HasValue)
                {
                    if (trackingInfoObj.Material_Rec_Date_Estimated)
                    {
                        receivedLabel.Text = trackingInfoObj.Material_Received_Date.Value.ToShortDateString() + " (estimated - Born Digital)";
                    }
                    else
                    {
                        receivedLabel.Text = trackingInfoObj.Material_Received_Date.Value.ToShortDateString() + " (Born Digital)";
                    }
                }
                else
                {
                    receivedLabel.Text = "Born Digital";
                }
            }
            else
            {
                if (trackingInfoObj.Material_Received_Date.HasValue)
                {
                    if (trackingInfoObj.Material_Rec_Date_Estimated)
                    {
                        receivedLabel.Text = trackingInfoObj.Material_Received_Date.Value.ToShortDateString() + " (estimated)";
                    }
                    else
                    {
                        receivedLabel.Text = trackingInfoObj.Material_Received_Date.Value.ToShortDateString();
                    }
                }
            }
            if (trackingInfoObj.Disposition_Date.HasValue)
            {
                adviceLinkLabel.Hide();
                adviceLabel.Show();
                dispositionLabel.Show();
                dispositionLinkLabel.Hide();
                string disposition = SobekCM_Library_Settings.Disposition_Term_Past(trackingInfoObj.Disposition_Type).ToUpper();
                if (trackingInfoObj.Disposition_Notes.Trim().Length > 0)
                {
                    dispositionLabel.Text = disposition + " " + trackingInfoObj.Disposition_Date.Value.ToShortDateString() + " - " + trackingInfoObj.Disposition_Notes;
                }
                else
                {
                    dispositionLabel.Text = disposition + " " + trackingInfoObj.Disposition_Date.Value.ToShortDateString();
                }
            }
            else
            {
                adviceLabel.Hide();
                adviceLinkLabel.Show();
                dispositionLabel.Hide();
                dispositionLinkLabel.Show();
            }


            if (trackingInfoObj.Disposition_Advice > 0)
            {
                string disposition = SobekCM_Library_Settings.Disposition_Term_Future(trackingInfoObj.Disposition_Advice).ToUpper();
                if (trackingInfoObj.Disposition_Advice_Notes.Trim().Length > 0)
                {
                    adviceLinkLabel.Text = disposition + " - " + trackingInfoObj.Disposition_Advice_Notes;
                    adviceLabel.Text = disposition + " - " + trackingInfoObj.Disposition_Advice_Notes;
                }
                else
                {
                    adviceLinkLabel.Text = disposition;
                    adviceLabel.Text = disposition;
                }
            }

            if ((!trackingInfoObj.Locally_Archived) && (!trackingInfoObj.Remotely_Archived))
            {
                archivingLabel1.Text = "NOT ARCHIVED";
                archivingLabel2.Text = String.Empty;
            }
            else
            {
                if (trackingInfoObj.Locally_Archived)
                {
                    archivingLabel1.Text = "Locally Stored on CD or Tape";
                    archivingLabel2.Text = trackingInfoObj.Remotely_Archived ? "Archived Remotely (FDA)" : String.Empty;
                }
                else
                {
                    archivingLabel1.Text = "Archived Remotely (FDA)";
                    archivingLabel2.Text = String.Empty;
                }
            }
            if (trackingInfoObj.Tracking_Box.Length > 0)
            {
                trackingBoxLabel.Text = trackingInfoObj.Tracking_Box;
            }


            // Show the history table
            if (historyPanel == null)
            {
                historyPanel = new CustomGrid_Panel
                                   {
                                       Size = new Size(historyTabPage.Width - 20, historyTabPage.Height - 20),
                                       Location = new Point(10, 10)
                                   };
                historyPanel.Style.Default_Column_Width = 80;
                historyPanel.Style.Default_Column_Color = Color.LightBlue;
                historyPanel.Style.Header_Back_Color = Color.DarkBlue;
                historyPanel.Style.Header_Fore_Color = Color.White;
                historyPanel.BackColor = Color.White;
                historyPanel.BorderStyle = BorderStyle.FixedSingle;
                historyPanel.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
                historyTabPage.Controls.Add(historyPanel);
            }

            historyPanel.DataTable = trackingInfo.Tables[1];
            historyPanel.Style.Column_Styles[0].Visible = false;
            historyPanel.Style.Column_Styles[1].BackColor = Color.White;
            historyPanel.Style.Column_Styles[1].Width = 150;
            historyPanel.Style.Column_Styles[2].Width = 80;
            historyPanel.Style.Column_Styles[2].Header_Text = "Completed";
            historyPanel.Style.Column_Styles[3].Header_Text = "User";
            historyPanel.Style.Column_Styles[3].Width = 100;
            historyPanel.Style.Column_Styles[4].Header_Text = "Location";
            historyPanel.Style.Column_Styles[5].Width = 200;
        }

        #endregion

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
                brush.SetBlendTriangularShape(0.50F);

                // Create the image
                gr.FillRectangle(brush, rect);
                gr.Dispose();

                // Set this as the backgroundf
                BackgroundImage = image;
            }
        }

        #endregion

        #region Methods to print the tracking sheets or the item spreadsheet

        private void print_tracking_report()
        {
            // if ((gridPanel.Selected_Row != null) && (gridPanel.Selected_Row.Length > 0))
            //{
            //    foreach (DataRow thisRow in gridPanel.Selected_Row)
            //    {
            //        PrintDocument printReport = new PrintDocument();
            //        printReport.PrintPage += new PrintPageEventHandler(printReport_PrintPage);
            //        bibIdPrinter = new BibIdReport_Printer(printReport, bibid, thisRow["VID"].ToString(), grouptitle, thisRow["Volume_Title"].ToString(), String.Empty, String.Empty, type, aleph, oclc, thisRow["Level1_Text"].ToString(), thisRow["Level2_Text"].ToString(), thisRow["Level3_Text"].ToString());
            //        printReport.Print();
            //    }
            //}
            //else
            //    MessageBox.Show("Please select at least one row.", "No row selected", MessageBoxButtons.OK,
            //        MessageBoxIcon.Information);
        }

        private void printReport_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                bibIdPrinter.Print_Title_Report(g);
            }
            catch
            {
                MessageBox.Show("Error encountered while printing!       ", "I/O Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Form-related (Non-menu driven) event-handling methods

        protected override void OnClosing(CancelEventArgs e)
        {
            if (WindowState != FormWindowState.Maximized)
            {
                SMaRT_UserSettings.View_Item_Form_Size = Size;
                SMaRT_UserSettings.View_Item_Form_Maximized = false;
            }
            else
                SMaRT_UserSettings.View_Item_Form_Maximized = true;

            SMaRT_UserSettings.Save();
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void mainLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = SobekCM_Library_Settings.System_Base_URL + bibid + "/" + vid;

            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening item group from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void volumeReportButton_Button_Pressed(object sender, EventArgs e)
        {
            print_tracking_report();
        }

        private void directoryLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(directoryLinkLabel.Text + "\\");
            }
            catch
            {
                MessageBox.Show("Error opening item directory.     ", "Explorer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void archiveRetrieveButton_Button_Pressed(object sender, EventArgs e)
        {
            Retrieve_Archives_Form form = new Retrieve_Archives_Form(bibid, vid);
            Hide();
            form.ShowDialog();
            Show();
        }

        #endregion

        #region Event handlers related to the picture buttons at the top of the form

        private void pictureBox_MouseEnter(object sender, EventArgs e)
        {
            behaviorsPictureBox.BorderStyle = BorderStyle.None;
            metadataPictureBox.BorderStyle = BorderStyle.None;
            commentPictureBox.BorderStyle = BorderStyle.None;
            visibilityPictureBox.BorderStyle = BorderStyle.None;
            metsPictureBox.BorderStyle = BorderStyle.None;

            PictureBox senderBox = (PictureBox)sender;
            senderBox.BorderStyle = BorderStyle.FixedSingle;

        }

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            behaviorsPictureBox.BorderStyle = BorderStyle.None;
            metadataPictureBox.BorderStyle = BorderStyle.None;
            commentPictureBox.BorderStyle = BorderStyle.None;
            visibilityPictureBox.BorderStyle = BorderStyle.None;
            metsPictureBox.BorderStyle = BorderStyle.None;
        }

        private void metadataPictureBox_Click(object sender, EventArgs e)
        {
            string url = SobekCM_Library_Settings.System_Base_URL + "my/edit/" + bibid + "/" + vid + "/1";

            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening item metadata edit from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void behaviorsPictureBox_Click(object sender, EventArgs e)
        {
            string url = SobekCM_Library_Settings.System_Base_URL + "my/behaviors/" + bibid + "/" + vid + "/1"; 

            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening item behavior edit from the web.     ", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void metsPictureBox_Click(object sender, EventArgs e)
        {
            string url = SobekCM_Library_Settings.Image_URL + bibid.Substring(0, 2) + "/" + bibid.Substring(2, 2) + "/" + bibid.Substring(4, 2) + "/" + bibid.Substring(6, 2) + "/" + bibid.Substring(8, 2) + "/" + vid + "/" + bibid + "_" + vid + ".mets.xml";
           
            try
            {
                Process openOnWeb = new Process {StartInfo = {FileName = url}};
                openOnWeb.Start();
            }
            catch
            {
                MessageBox.Show("Error opening METS file from the web.     ", "Browser or URL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mainLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            behaviorsPictureBox.BorderStyle = BorderStyle.None;
            metadataPictureBox.BorderStyle = BorderStyle.None;
            commentPictureBox.BorderStyle = BorderStyle.None;
            visibilityPictureBox.BorderStyle = BorderStyle.None;
            metsPictureBox.BorderStyle = BorderStyle.None;
        }

        private void tabControl1_Enter(object sender, EventArgs e)
        {
            behaviorsPictureBox.BorderStyle = BorderStyle.None;
            metadataPictureBox.BorderStyle = BorderStyle.None;
            commentPictureBox.BorderStyle = BorderStyle.None;
            visibilityPictureBox.BorderStyle = BorderStyle.None;
            metsPictureBox.BorderStyle = BorderStyle.None;
        }

        #endregion

        private void adviceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Edit_Disposition_Advice_Form trackingForm = new Edit_Disposition_Advice_Form(trackingInfoObj.Disposition_Advice, trackingInfoObj.Disposition_Advice_Notes);
            if (trackingForm.ShowDialog() == DialogResult.OK)
            {
                int type = trackingForm.Disposition_Type_ID;
                string notes = trackingForm.Disposition_Notes;

                if ((type != trackingInfoObj.Disposition_Advice) || (notes.Trim() != trackingInfoObj.Disposition_Advice_Notes.Trim()))
                {
                    if (Resource_Object.Database.SobekCM_Database.Edit_Disposition_Advice(itemid, type, notes))
                    {
                        trackingInfoObj.Disposition_Advice = (short)type;
                        trackingInfoObj.Disposition_Advice_Notes = notes;

                        string disposition = SobekCM_Library_Settings.Disposition_Term_Future(trackingInfoObj.Disposition_Advice).ToUpper();
                        if (trackingInfoObj.Disposition_Advice_Notes.Trim().Length > 0)
                        {
                            adviceLinkLabel.Text = disposition + " - " + trackingInfoObj.Disposition_Advice_Notes;
                            adviceLabel.Text = disposition + " - " + trackingInfoObj.Disposition_Advice_Notes;
                        }
                        else
                        {
                            adviceLinkLabel.Text = disposition;
                            adviceLabel.Text = disposition;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Error encountered while saving disposition advice!    ", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dispositionLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Update_Disposition_Form trackingForm = new Update_Disposition_Form();
            if (trackingForm.ShowDialog() == DialogResult.OK)
            {
                DateTime date = trackingForm.Disposition_Date;
                int type = trackingForm.Disposition_Type_ID;
                string notes = trackingForm.Disposition_Notes;

                if (Resource_Object.Database.SobekCM_Database.Update_Disposition(itemid, type, notes, date, Environment.UserName))
                {
                    show_milestones_worklog();
                }
                else
                {
                    MessageBox.Show("Error encountered while saving disposition information!    ", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void trackingBoxLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Enter_Tracking_Box_Form trackingForm = new Enter_Tracking_Box_Form(trackingInfoObj.Tracking_Box);
            if (trackingForm.ShowDialog() == DialogResult.OK)
            {
                string newBox = trackingForm.New_Tracking_Box;
                if (newBox.Trim() != trackingInfoObj.Tracking_Box.Trim())
                {
                    if (Resource_Object.Database.SobekCM_Database.Save_New_Tracking_Box(itemid, newBox))
                    {
                        trackingInfoObj.Tracking_Box = newBox;
                        trackingBoxLabel.Text = newBox;
                    }
                    else
                    {
                        MessageBox.Show("Error encountered while setting the tracking box!    ", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void copyPictureBox_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, bibid + ":" + vid );
        }
    }
}
