#region Using directives

using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DLC.Custom_Grid;
using SobekCM.Library.Database;
using SobekCM.Library.Items;

#endregion

namespace SobekCM.Management_Tool.Reports
{
    /// <summary> Form displays the newspaper item groups which have items lacking serial hierarchy information  </summary>
    public partial class Groups_Needed_Serial_Hierarchy_Form : Form
    {
        private readonly CustomGrid_Panel iconPanel;

        /// <summary> Constructor for a new instance of the Groups_Needed_Serial_Hierarchy_Form class </summary>
        public Groups_Needed_Serial_Hierarchy_Form()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            DataTable sourceTable = SobekCM_Database.Newspapers_Without_Serial_Info;

            iconPanel = new CustomGrid_Panel();
            iconPanel.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom)
                                   | AnchorStyles.Left)
                                  | AnchorStyles.Right)));
            iconPanel.BorderStyle = BorderStyle.FixedSingle;
            iconPanel.DataTable = null;
            iconPanel.Location = new Point(16, 16);
            iconPanel.Name = "itemGridPanel";
            iconPanel.Size = new Size(mainPanel.Width - 32, mainPanel.Height - 32);
            iconPanel.TabIndex = 0;
            iconPanel.Double_Clicked += iconPanel_Double_Clicked;

            mainPanel.Controls.Add(iconPanel);


            // Configure some table level style settings
            iconPanel.Style.Default_Column_Width = 85;
            iconPanel.Style.Default_Column_Color = Color.LightBlue;
            iconPanel.Style.Header_Back_Color = Color.SteelBlue;
            iconPanel.Style.Header_Fore_Color = Color.White;

            // Use the table from the database as the data source
            iconPanel.DataTable = sourceTable;

            //// Configure some individual columns
            iconPanel.Style.Column_Styles[1].Header_Text = "Group Title";
            iconPanel.Style.Column_Styles[1].BackColor = Color.White;
            iconPanel.Style.Column_Styles[1].Width = 450;
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        private void refresh(  )
        {
            int width = iconPanel.Style.Column_Styles[1].Width;
            DataTable sourceTable = SobekCM_Database.Newspapers_Without_Serial_Info;
            iconPanel.DataTable = sourceTable;
            //// Configure some individual columns
            iconPanel.Style.Column_Styles[1].Header_Text = "Group Title";
            iconPanel.Style.Column_Styles[1].BackColor = Color.White;
            iconPanel.Style.Column_Styles[1].Width = width + 1;
        }

        void iconPanel_Double_Clicked(DataRow thisRow)
        {
            if (thisRow != null)
            {
                string bibid = thisRow["BibID"].ToString();
                SobekCM_Items_In_Title multiple = SobekCM_Database.Get_Multiple_Volumes( bibid, null);
                if (multiple == null)
                {
                    MessageBox.Show("Either the BibID you entered is invalid, or there was a database error.");
                }
                else
                {
                    Hide();
                    Edit_Serial_Hierarchy_Form edit = new Edit_Serial_Hierarchy_Form( bibid, multiple);
                    edit.ShowDialog();
                    refresh();
                    Show();
                    mainPanel.Width = mainPanel.Width - 1;
                    mainPanel.Width = mainPanel.Width + 1;
                }
            }
        }


        private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
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
