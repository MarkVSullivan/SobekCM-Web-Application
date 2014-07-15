#region Using directives

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SobekCM.Library.Database;
using SobekCM.Library.Navigation;
using SobekCM.Library.Search;

#endregion

namespace SobekCM.Management_Tool.Controls
{
    /// <summary>Control is used to allow the user to narrow the rows which should show in the 
    /// main section of the SMaRT Tool </summary>
    public partial class SobekCM_Item_Discovery_Panel : UserControl
    {
        #region Delegates

        /// <summary> Delegate defines the signature for the event fired when the user selects a new search  </summary>
        public delegate void Search_Requested_Delegate();

        #endregion

        private readonly bool isXp;
        private Search_Precision_Type_Enum searchPrecision;


        /// <summary> Constructor for a new instance of the SobekCM_Item_Discovery_Panel user control  </summary>
        public SobekCM_Item_Discovery_Panel( )
        {
            InitializeComponent();

            // Get the list of all terms
            Array termEnumValues = Enum.GetValues(typeof(SobekCM_Search_Object.SobekCM_Term_Enum));
            object[] terms = new object[termEnumValues.Length];
            for (int i = 0; i < termEnumValues.Length; i++)
            {
                terms[i] = termEnumValues.GetValue(i).ToString().Replace("_"," ");
            }

            // Get the list of all links
            Array linkEnumValues = Enum.GetValues(typeof(SobekCM_Search_Object.SobekCM_Link_Enum));
            object[] links = new object[linkEnumValues.Length];
            for (int i = 0; i < linkEnumValues.Length; i++)
            {
                links[i] = linkEnumValues.GetValue(i).ToString().Replace("_", " ").ToLower();
            }

            // Populate all the terms
            termComboBox1.Items.AddRange(terms);
            termComboBox2.Items.AddRange(terms);
            termComboBox3.Items.AddRange(terms);
            termComboBox4.Items.AddRange(terms);

            // Populate all the links
            linkComboBox2.Items.AddRange(links);
            linkComboBox3.Items.AddRange(links);
            linkComboBox4.Items.AddRange(links);
              

            searchPrecision = Search_Precision_Type_Enum.Inflectional_Form;

            Current_Search = new SobekCM_Search_Object(Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term1,
                                                       Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term2,
                                                       Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term3,
                                                       Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term4);


            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                institutionRadioButton.FlatStyle = FlatStyle.Flat;
                allRadioButton.FlatStyle = FlatStyle.Flat;
                collectionRadioButton.FlatStyle = FlatStyle.Flat;
                institutionComboBox.FlatStyle = FlatStyle.Flat;
                collectionComboBox.FlatStyle = FlatStyle.Flat;
                linkComboBox2.FlatStyle = FlatStyle.Flat;
                linkComboBox3.FlatStyle = FlatStyle.Flat;
                linkComboBox4.FlatStyle = FlatStyle.Flat;
                termComboBox1.FlatStyle = FlatStyle.Flat;
                termComboBox2.FlatStyle = FlatStyle.Flat;
                termComboBox3.FlatStyle = FlatStyle.Flat;
                termComboBox4.FlatStyle = FlatStyle.Flat;
                searchTextBox1.BorderStyle = BorderStyle.FixedSingle;
                searchTextBox2.BorderStyle = BorderStyle.FixedSingle;
                searchTextBox3.BorderStyle = BorderStyle.FixedSingle;
                searchTextBox4.BorderStyle = BorderStyle.FixedSingle;
                isXp = true;

            }
            else
                isXp = false;
        }

        /// <summary> Gets the current search object for the user's requested search or browse  </summary>
        public SobekCM_Search_Object Current_Search
        {
            get
            {
                SobekCM_Search_Object returnValue = new SobekCM_Search_Object();
                if (institutionRadioButton.Checked)
                    returnValue.Institution = institutionComboBox.Text;
                if (collectionRadioButton.Checked)
                    returnValue.Aggregation = collectionComboBox.Text;
                returnValue.First_Value = searchTextBox1.Text.Trim();
                returnValue.Second_Value = searchTextBox2.Text.Trim();
                returnValue.Third_Value = searchTextBox3.Text.Trim();
                returnValue.Fourth_Value = searchTextBox4.Text.Trim();
                returnValue.First_Term = (SobekCM_Search_Object.SobekCM_Term_Enum)termComboBox1.SelectedIndex;
                returnValue.Second_Term = (SobekCM_Search_Object.SobekCM_Term_Enum)termComboBox2.SelectedIndex;
                returnValue.Third_Term = (SobekCM_Search_Object.SobekCM_Term_Enum)termComboBox3.SelectedIndex;
                returnValue.Fourth_Term = (SobekCM_Search_Object.SobekCM_Term_Enum)termComboBox4.SelectedIndex;
                returnValue.First_Link = (SobekCM_Search_Object.SobekCM_Link_Enum)linkComboBox2.SelectedIndex;
                returnValue.Second_Link = (SobekCM_Search_Object.SobekCM_Link_Enum)linkComboBox3.SelectedIndex;
                returnValue.Third_Link = (SobekCM_Search_Object.SobekCM_Link_Enum)linkComboBox4.SelectedIndex;
                returnValue.Search_Precision = searchPrecision;
                return returnValue;
            }
            set
            {
                // Set the initial values on the drop down lists
                termComboBox1.SelectedIndex = Convert.ToInt32(value.First_Term);
                termComboBox2.SelectedIndex = Convert.ToInt32(value.Second_Term);
                termComboBox3.SelectedIndex = Convert.ToInt32(value.Third_Term);
                termComboBox4.SelectedIndex = Convert.ToInt32(value.Fourth_Term);
                linkComboBox2.SelectedIndex = Convert.ToInt32(value.First_Link);
                linkComboBox3.SelectedIndex = Convert.ToInt32(value.Second_Link);
                linkComboBox4.SelectedIndex = Convert.ToInt32(value.Third_Link);

                // Set initial search values
                searchTextBox1.Text = value.First_Value;
                searchTextBox2.Text = value.Second_Value;
                searchTextBox3.Text = value.Third_Value;
                searchTextBox4.Text = value.Fourth_Value;

                // Set the aggregation and institution           
                if (value.Aggregation.Length > 0)
                {
                    collectionComboBox.Text = value.Aggregation;
                    collectionRadioButton.Checked = true;
                }
                else if (value.Institution.Length > 0)
                {
                    institutionComboBox.Text = value.Institution;
                    institutionRadioButton.Checked = true;
                }

                searchPrecision = value.Search_Precision;

            }
        }

        /// <summary> Gets the user requested search precision </summary>
        public Search_Precision_Type_Enum Search_Precision
        {
            get { return searchPrecision; }
            set { searchPrecision = value; }
        }


        /// <summary> Event is fired when the user requests a new search or browse </summary>
        public event Search_Requested_Delegate Search_Requested;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (isXp)
            {
                e.Graphics.DrawRectangle(Pens.Gray, institutionComboBox.Location.X - 1, institutionComboBox.Location.Y - 1, institutionComboBox.Width + 1, institutionComboBox.Height + 1);
                e.Graphics.DrawRectangle(Pens.Gray, collectionComboBox.Location.X - 1, collectionComboBox.Location.Y - 1, collectionComboBox.Width + 1, collectionComboBox.Height + 1);
                e.Graphics.DrawRectangle(Pens.Gray, linkComboBox2.Location.X - 1, linkComboBox2.Location.Y - 1, linkComboBox2.Width + 1, linkComboBox2.Height + 1);
                e.Graphics.DrawRectangle(Pens.Gray, linkComboBox3.Location.X - 1, linkComboBox3.Location.Y - 1, linkComboBox3.Width + 1, linkComboBox3.Height + 1);
                e.Graphics.DrawRectangle(Pens.Gray, linkComboBox4.Location.X - 1, linkComboBox4.Location.Y - 1, linkComboBox4.Width + 1, linkComboBox4.Height + 1);
                e.Graphics.DrawRectangle(Pens.Gray, termComboBox1.Location.X - 1, termComboBox1.Location.Y - 1, termComboBox1.Width + 1, termComboBox1.Height + 1);
                e.Graphics.DrawRectangle(Pens.Gray, termComboBox2.Location.X - 1, termComboBox2.Location.Y - 1, termComboBox2.Width + 1, termComboBox2.Height + 1);
                e.Graphics.DrawRectangle(Pens.Gray, termComboBox3.Location.X - 1, termComboBox3.Location.Y - 1, termComboBox3.Width + 1, termComboBox3.Height + 1);
                e.Graphics.DrawRectangle(Pens.Gray, termComboBox4.Location.X - 1, termComboBox4.Location.Y - 1, termComboBox4.Width + 1, termComboBox4.Height + 1);
            }
        }

        /// <summary> Finish loading all the data needed to populate this selection panel </summary>
        public void Finish_Loading_Data()
        {
            // Get the list of aggregations/collections 
            try
            {
                DataTable aggregationTable = SobekCM_Database.Get_Codes_Item_Aggregations( null);
                DataColumn codeColumn = aggregationTable.Columns["Code"];
                DataColumn typeColumn = aggregationTable.Columns["Type"];
                foreach (DataRow thisRow in aggregationTable.Rows)
                {
                    string code = thisRow[codeColumn].ToString();
                    string type = thisRow[typeColumn].ToString().ToUpper();

                    if (type.IndexOf("INSTITUT") == 0)
                    {
                        if (!institutionComboBox.Items.Contains(code))
                            institutionComboBox.Items.Add(code);
                    }
                    else
                    {
                        collectionComboBox.Items.Add(code);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error retrieving aggregation codes from the database.", "Error Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            institutionComboBox.SelectedIndex = 0;
            collectionComboBox.SelectedIndex = 0;
            institutionComboBox.SelectedIndexChanged += institutionComboBox_SelectedIndexChanged;
            collectionComboBox.SelectedIndexChanged += collectionComboBox_SelectedIndexChanged;

            // Set some personalization and customization for the SobekCM Instance Name
            allRadioButton.Text = "No limit ( All " + Library.SobekCM_Library_Settings.System_Abbreviation + " Items )";
        }

        void collectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            allRadioButton.Checked = false;
            collectionRadioButton.Checked = true;
            institutionRadioButton.Checked = false;
        }

        void institutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            allRadioButton.Checked = false;
            collectionRadioButton.Checked = false;
            institutionRadioButton.Checked = true;
        }

        /// <summary> Clear all the search box information </summary>
        public void Clear_Search_Boxes()
        {
            searchTextBox1.Clear();
            searchTextBox2.Clear();
            searchTextBox3.Clear();
            searchTextBox4.Clear();
        }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                if (Search_Requested != null)
                    Search_Requested();
            }
        }

        private void searchTextBox_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Khaki;
        }

        private void searchTextBox_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;
        }

    }
}
