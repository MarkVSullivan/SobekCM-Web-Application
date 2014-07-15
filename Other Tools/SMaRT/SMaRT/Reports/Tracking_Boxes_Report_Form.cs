#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using SobekCM.Library.Database;
using SobekCM.Library.Search;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool.Reports
{
    /// <summary> Form displays the complete list of tracking boxes currently in use in the system </summary>
    public partial class Tracking_Boxes_Report_Form : Form
    {
        /// <summary> Constructor for a new instance of the Tracking_Boxes_Report_Form form </summary>
        public Tracking_Boxes_Report_Form()
        {
            InitializeComponent();

            // Load the list of all tracking boxes
            listView1.Items.Clear();
            List<string> trackingBoxes = SobekCM_Database.All_Tracking_Boxes;
            foreach (string thisBox in trackingBoxes)
            {
                listView1.Items.Add(thisBox);
            }

            // Set the size correctly
            Size = SMaRT_UserSettings.Tracking_Box_Report_Form_Size;
            int screen_width = Screen.PrimaryScreen.WorkingArea.Width;
            int screen_height = Screen.PrimaryScreen.WorkingArea.Height;
            if ((Width > screen_width) || (Height > screen_height) || (SMaRT_UserSettings.Tracking_Box_Report_Form_Maximized))
                WindowState = FormWindowState.Maximized;
        }



        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if ( listView1.SelectedItems.Count == 1 )
            {
                string box = listView1.SelectedItems[0].Text;

                SobekCM_Search_Object search =
                    new SobekCM_Search_Object(SMaRT_UserSettings.Discovery_Panel_Search_Term1,
                                              SMaRT_UserSettings.Discovery_Panel_Search_Term2,
                                              SMaRT_UserSettings.Discovery_Panel_Search_Term3,
                                              SMaRT_UserSettings.Discovery_Panel_Search_Term4);
                bool found_bib = false;
                if (search.First_Term == SobekCM_Search_Object.SobekCM_Term_Enum.Tracking_Box)
                {
                    search.First_Value = box;
                    found_bib = true;
                }
                if ((!found_bib) && (search.Second_Term == SobekCM_Search_Object.SobekCM_Term_Enum.Tracking_Box))
                {
                    search.Second_Value = box;
                    found_bib = true;
                }
                if ((!found_bib) && (search.Third_Term == SobekCM_Search_Object.SobekCM_Term_Enum.Tracking_Box))
                {
                    search.Third_Value = box;
                    found_bib = true;
                }
                if ((!found_bib) && (search.Fourth_Term == SobekCM_Search_Object.SobekCM_Term_Enum.Tracking_Box))
                {
                    search.Fourth_Value = box;
                    found_bib = true;
                }
                if (!found_bib)
                {
                    search.Fourth_Value = box;
                    search.Fourth_Term = SobekCM_Search_Object.SobekCM_Term_Enum.Tracking_Box;
                }

                // Show the ad hoc reporting form
                Ad_Hoc_Reporting_Query_Form showForm = new Ad_Hoc_Reporting_Query_Form(search, null);
                Hide();
                showForm.ShowDialog();

                // Refresh the tracking box list
                listView1.Items.Clear();
                List<string> trackingBoxes = SobekCM_Database.All_Tracking_Boxes;
                foreach (string thisBox in trackingBoxes)
                {
                    listView1.Items.Add(thisBox);
                }

                Show();
            }
        }

        private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        #region Form-related (Non-menu driven) event-handling methods

        protected override void OnClosing(CancelEventArgs e)
        {
            // Save the window state and/or size
            if (WindowState != FormWindowState.Maximized)
            {
                SMaRT_UserSettings.Tracking_Box_Report_Form_Size = Size;
                SMaRT_UserSettings.Tracking_Box_Report_Form_Maximized = false;
            }
            else
                SMaRT_UserSettings.Tracking_Box_Report_Form_Maximized = true;

            SMaRT_UserSettings.Save();
        }

        #endregion
    }
}
