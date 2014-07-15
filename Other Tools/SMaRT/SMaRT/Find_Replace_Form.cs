#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Tool-type form is used to perform find and replace within serial hierarchy which is being edited </summary>
    public partial class Find_Replace_Form : Form
    {
        private bool cancelled;
        private bool checkedRowsOnly;
        private string findValue;
        private string replaceValue;

        /// <summary> Constructor for a new instance of the Find_Replace_Form class </summary>
        /// <param name="defaultScopeRestricted"> Indicates if the default scope is set to restricted to checked rows </param>
        public Find_Replace_Form( bool defaultScopeRestricted )
        {
            InitializeComponent();

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                findTextBox.BorderStyle = BorderStyle.FixedSingle;
                replaceTextBox.BorderStyle = BorderStyle.FixedSingle;
                checkedRowsRadioButton.FlatStyle = FlatStyle.Flat;
                allRowsRadioButton.FlatStyle = FlatStyle.Flat;
            }

            BackColor = Color.FromArgb(240, 240, 240);
            
            cancelled = true;
            checkedRowsOnly = false;
            findValue = String.Empty;
            replaceValue = String.Empty;

            if (defaultScopeRestricted)
            {
                checkedRowsOnly = true;
                checkedRowsRadioButton.Checked = true;
            }
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary> Returns flag which indicates if this find/replace request was cancelled </summary>
        public bool Cancelled
        {
            get { return cancelled; }
        }

        /// <summary> Returns flag which indicates if this request should only be applied to those
        /// rows which were checked on the serial hierarchy edit form </summary>
        public bool Checked_Rows_Only
        {
            get { return checkedRowsOnly; }
        }
        
        /// <summary> Gets the string value to be found and replaced </summary>
        public string Find_Value
        {
            get { return findValue; }
        }

        /// <summary> Returns the value to be used during replacements </summary>
        public string Replace_Value
        {
            get { return replaceValue; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            cancelled = false;
            checkedRowsOnly = checkedRowsRadioButton.Checked;
            findValue = findTextBox.Text;
            replaceValue = replaceTextBox.Text;
            Close();
        }
    }
}
