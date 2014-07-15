using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace SobekCM.Management_Tool.Importer.Forms
{
    public partial class Importer_Form : Form
    {
        public Importer_Form(string default_institution_code, string default_institution_statement)
        {
            InitializeComponent();

            baseImporter_Processor.Default_Institution_Code = default_institution_code;
            baseImporter_Processor.Default_Institution_Statement = default_institution_statement;

            Constant_Assignment_Control.Set_Aggregation_Table(SobekCM.Library.Database.SobekCM_Database.Get_Codes_Item_Aggregations( null));
        }

        #region Method to draw the form background

        /// <summary> Method is called whenever this form is resized. </summary>
        /// <param name="e"></param>
        /// <remarks> This redraws the background of this form </remarks>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Get rid of any current background image
            if (this.BackgroundImage != null)
            {
                this.BackgroundImage.Dispose();
                this.BackgroundImage = null;
            }

            if (this.ClientSize.Width > 0)
            {
                // Create the items needed to draw the background
                Bitmap image = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                Graphics gr = Graphics.FromImage(image);
                Rectangle rect = new Rectangle(new Point(0, 0), this.ClientSize);

                // Create the brush
                LinearGradientBrush brush = new LinearGradientBrush(rect, SystemColors.Control, ControlPaint.Dark(SystemColors.Control), LinearGradientMode.Vertical);
                brush.SetBlendTriangularShape(0.33F);

                // Create the image
                gr.FillRectangle(brush, rect);
                gr.Dispose();

                // Set this as the backgroundf
                this.BackgroundImage = image;
            }
        }

        #endregion

        private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void marcLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // create an instance of the MARC Importer form
            Forms.MARC_Importer_Form marcImporterForm = new Forms.MARC_Importer_Form();
            this.Hide();
            marcImporterForm.ShowDialog();
            this.Close();    
        }

        private void spreadsheetLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // create an instance of the SpreadSheet Importer form
            Forms.SpreadSheet_Importer_Form spreadSheetImporterForm = new Forms.SpreadSheet_Importer_Form();
            this.Hide();
            spreadSheetImporterForm.ShowDialog();
            this.Close();
        }

        private void autoFillLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Volume_Import_Form importForm = new Volume_Import_Form();
            this.Hide();
            importForm.ShowDialog();
            this.Show();
        }


    }
}
