#region Using directives

using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DLC.Custom_Grid;
using SobekCM.Library.Database;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool.Reports
{
    /// <summary> Form displays the items which are QC complete but are pending online complete,
    /// and are currently set to PRIVATE </summary>
    public partial class Items_Pending_Online_Complete_Form : Form
    {
        private readonly CustomGrid_Panel iconPanel;

        /// <summary> Constructor for a new instance of the Items_Pending_Online_Complete_Form form </summary>
        public Items_Pending_Online_Complete_Form()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            DataTable sourceTable = SobekCM_Database.Items_Pending_Online_Complete;

            // Add two columns
            DataColumn accessColumn = sourceTable.Columns.Add("Access");
            DataColumn aggregationColumn = sourceTable.Columns["AggregationCodes"];
            DataColumn restrictionColumn = sourceTable.Columns["IP_Restriction_Mask"];
            foreach (DataRow thisRow in sourceTable.Rows)
            {
                thisRow[aggregationColumn] = thisRow[aggregationColumn].ToString().Trim().Replace("  ", " ").Replace(" ", ",");

                int access = Convert.ToInt16(thisRow[restrictionColumn]);
                if (access < 0)
                    thisRow[accessColumn] = "private";
                if (access == 0)
                    thisRow[accessColumn] = "public";
                if (access > 0)
                    thisRow[accessColumn] = "restricted";
            }

            iconPanel = new CustomGrid_Panel
                            {
                                Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom)
                                             | AnchorStyles.Left)
                                            | AnchorStyles.Right))),
                                BorderStyle = BorderStyle.FixedSingle,
                                DataTable = null,
                                Location = new Point(16, 16),
                                Name = "itemGridPanel",
                                Size = new Size(mainPanel.Width - 32, mainPanel.Height - 32),
                                TabIndex = 0
                            };
            iconPanel.Double_Clicked += iconPanel_Double_Clicked;

            mainPanel.Controls.Add(iconPanel);


            // Configure some table level style settings
            iconPanel.Style.Default_Column_Width = 85;
            iconPanel.Style.Default_Column_Color = Color.LightBlue;
            iconPanel.Style.Header_Back_Color = Color.SteelBlue;
            iconPanel.Style.Header_Fore_Color = Color.White;

            // Use the table from the database as the data source
            iconPanel.DataTable = sourceTable;

            ////// Configure some individual columns
            iconPanel.Style.Column_Styles[1].Width = 50;
            iconPanel.Style.Column_Styles[2].Width = 150;
            iconPanel.Style.Column_Styles[2].Header_Text = "QC Milestone";
            iconPanel.Style.Column_Styles[3].Visible = false;
            iconPanel.Style.Column_Styles[4].Visible = false;
            iconPanel.Style.Column_Styles[5].Header_Text = "Pages";
            iconPanel.Style.Column_Styles[5].Width = 50;
            iconPanel.Style.Column_Styles[6].Width = 250;
            iconPanel.Style.Column_Styles[6].BackColor = Color.White;
            iconPanel.Style.Column_Styles[7].Width = 100;
            iconPanel.Style.Column_Styles[7].BackColor = Color.White;
            iconPanel.Style.Column_Styles[8].Width = 120;
            iconPanel.Style.Column_Styles[8].Header_Text = "Aggregations";
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        void iconPanel_Double_Clicked(DataRow thisRow)
        {
            if (thisRow != null)
            {
                try
                {
                    string bibid = thisRow["BibID"].ToString();
                    string vid = thisRow["VID"].ToString();
                    string url = Library.SobekCM_Library_Settings.System_Base_URL + bibid + "/" + vid;
                    Process showURL = new Process {StartInfo = {FileName = url}};
                    showURL.Start();
                }
                catch
                {
                    MessageBox.Show("Error showing the item online.     ");
                }
            }
        }

        private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void refreshButton_Button_Pressed(object sender, EventArgs e)
        {
            DataTable sourceTable = SobekCM_Database.Items_Pending_Online_Complete;
            iconPanel.DataTable = sourceTable;

            ////// Configure some individual columns
            iconPanel.Style.Column_Styles[1].Width = 50;
            iconPanel.Style.Column_Styles[2].Width = 150;
            iconPanel.Style.Column_Styles[2].Header_Text = "QC Milestone";
            iconPanel.Style.Column_Styles[3].Visible = false;
            iconPanel.Style.Column_Styles[4].Visible = false;
            iconPanel.Style.Column_Styles[5].Header_Text = "Pages";
            iconPanel.Style.Column_Styles[5].Width = 50;
            iconPanel.Style.Column_Styles[6].Width = 250;
            iconPanel.Style.Column_Styles[6].BackColor = Color.White;
            iconPanel.Style.Column_Styles[7].Width = 100;
            iconPanel.Style.Column_Styles[7].BackColor = Color.White;
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
    }
}
