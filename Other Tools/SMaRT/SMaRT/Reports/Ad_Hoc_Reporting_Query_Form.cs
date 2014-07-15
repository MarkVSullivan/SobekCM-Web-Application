#region Using directives

using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SobekCM.Management_Tool.Settings;
using SobekCM.Library.Search;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Form is used to retrieve the user's requested search or browse for an ad hoc
    /// report and only displays if the user does not use a hotkey to take an existing search and 
    /// display it as an ad hoc report  </summary>
    public partial class Ad_Hoc_Reporting_Query_Form : Form
    {
        private DataSet lastResults;
        private SobekCM_Search_Object lastSearch;

        /// <summary> Constructor for a new instance of the Ad_Hoc_Reporting_Query_Form form </summary>
        public Ad_Hoc_Reporting_Query_Form()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            sobekCM_Item_Discovery_Panel1.Finish_Loading_Data();
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary> Constructor for a new instance of the Ad_Hoc_Reporting_Query_Form form </summary>
        /// <param name="Initial_Search"> Initial search to display in this form </param>
        /// <param name="Initial_DataSet"> Initial dataset to display, which matches the intial search </param>
        public Ad_Hoc_Reporting_Query_Form( SobekCM_Search_Object Initial_Search, DataSet Initial_DataSet )
        {
            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            sobekCM_Item_Discovery_Panel1.Finish_Loading_Data();
            sobekCM_Item_Discovery_Panel1.Current_Search = Initial_Search;

            // Save these last values
            lastSearch = Initial_Search;
            lastResults = Initial_DataSet;
        }

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            search_and_create_report();
        }

        private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void sobekCM_Item_Discovery_Panel1_Search_Requested()
        {
            search_and_create_report();
        }

        private void search_and_create_report()
        {
            SobekCM_Search_Object newSearch = sobekCM_Item_Discovery_Panel1.Current_Search;
            if (( lastSearch == null ) || ( lastResults == null ) || (!newSearch.Equals(lastSearch)))
            {
                Cursor = Cursors.WaitCursor;
                lastResults = newSearch.Perform_Tracking_Search();
                Cursor = Cursors.Default;
               
                if (lastResults == null)
                    return;

                if (!newSearch.Equals(lastSearch))
                {
                    lastSearch = newSearch;

                    // Since this is a new search, save the last used parameters
                    SMaRT_UserSettings.Discovery_Panel_Search_Term1 = lastSearch.First_Term;
                    SMaRT_UserSettings.Discovery_Panel_Search_Term2 = lastSearch.Second_Term;
                    SMaRT_UserSettings.Discovery_Panel_Search_Term3 = lastSearch.Third_Term;
                    SMaRT_UserSettings.Discovery_Panel_Search_Term4 = lastSearch.Fourth_Term;
                    SMaRT_UserSettings.Save();
                }
            }


            Ad_Hoc_Report_Display_Form showForm = new Ad_Hoc_Report_Display_Form(lastResults);
            Hide();
            showForm.ShowDialog();
            Show();

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
