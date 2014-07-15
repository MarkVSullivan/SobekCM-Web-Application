#region Using directives

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SobekCM.Library.Database;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Class is used to request retrieving some items from the local TIVOLI backup </summary>
    public partial class Retrieve_Archives_Form : Form
    {
        private readonly string bibid;
        private readonly string vid;

        /// <summary> Constructor for a new instance of the Retrieve_Archives_Form form </summary>
        /// <param name="BibID"> Bibliographic identifier for the title </param>
        /// <param name="VID"> Volume identifier for the item within the title </param>
        public Retrieve_Archives_Form( string BibID, string VID )
        {
            bibid = BibID;
            vid = VID;

            InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

            userNameTextBox.Text = SMaRT_UserSettings.User_Name;
            emailTextBox.Text = SMaRT_UserSettings.Email_Address;

            if (userNameTextBox.Text.Length == 0)
            {
                string new_username = SystemInformation.UserName;
                userNameTextBox.Text = new_username;
            }

            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                userNameTextBox.BorderStyle = BorderStyle.FixedSingle;
                notesTextBox.BorderStyle = BorderStyle.FixedSingle;
                emailTextBox.BorderStyle = BorderStyle.FixedSingle;
            }
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

        private void okButton_Button_Pressed(object sender, EventArgs e)
        {
            string username = userNameTextBox.Text.Replace("\\", "").Replace("/", "").Replace(".", "").Replace(",", "").Replace("&", "").Replace(":", "").Replace("@", "").Trim();
            string email = emailTextBox.Text.Trim();
            string note = notesTextBox.Text.Trim();

            // Ensure the required data is present
            if ((username.Length == 0) || (email.Length == 0))
            {
                MessageBox.Show("Username and email are both required fields.      ", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Save the user's preferences
            SMaRT_UserSettings.Email_Address = email;
            SMaRT_UserSettings.User_Name = username;
            SMaRT_UserSettings.Save();

            // Add this request to the database
            bool result = SobekCM_Database.Tivoli_Request_File(bibid, vid, "*", username.Replace(" ",""), email, note);
            if (result)
            {
                MessageBox.Show("Request submitted.   You should receive an email when the retrieval is complete.    ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Error requesting the files from the archives.   ", "Error Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
