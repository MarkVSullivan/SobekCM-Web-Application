using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using SobekCM.Resource_Object;

namespace SobekCM.Management_Tool.Importer.Forms
{
	/// <summary> Form allows the user to set the source and destination
	/// files for this process.  <br /> <br /> </summary>
	/// <remarks> Written by Mark Sullivan (2005) </remarks>
	/// <example> <img src="Source_Setup_Form2.jpg" /> </example>
	public class MARC_Importer_Form : System.Windows.Forms.Form
	{
		#region Form-Related Private Class Members

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private Panel panel1;
        private Label step2Label;
        private TabControl tabControl1;
        private TabPage tabPage2;
        private Panel pnlConstants;
        private Label step1Label;
        private Button browseButton;
        private TextBox sourceTextBox;
        private Label label3;
        private Label labelStatus;
        private Panel mainPanel;
        private Label step3Label;

		#endregion

        private IContainer components;

		/// <summary> Thread in which the processor runs </summary>
		protected Thread processThread;

		/// <summary> Processor object which actually parses the input file and
		/// gets the data for each item. </summary>
        protected MARC_Importer_Processor processor;

        private List<Column_Assignment_Control> column_map_inputs;
        private List<Constant_Assignment_Control> constant_map_inputs;

        private short user_specified_copyright_permissions;
        private CheckBox justSaveMarcXmlCheckBox;
        private Round_Button executeButton;
        private Round_Button cancelButton;
        private CheckBox previewCheckBox;
        private string workingFolder;

        private string tickler;


		/// <summary> Constructor for a new instance of this class </summary>
		public MARC_Importer_Form()
		{
			// Initialize this form 
			InitializeComponent();

            Constructor_Helper();

            workingFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SMaRT";

            // Perform some additional work if this was not XP theme
            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                browseButton.FlatStyle = FlatStyle.Flat;
                previewCheckBox.FlatStyle = FlatStyle.Flat;
                justSaveMarcXmlCheckBox.FlatStyle = FlatStyle.Flat;
                sourceTextBox.BorderStyle = BorderStyle.FixedSingle;
            }
		}

        private void Constructor_Helper()
        {
            CheckForIllegalCrossThreadCalls = false;
            tickler = String.Empty;

            column_map_inputs = new List<Column_Assignment_Control>();
            constant_map_inputs = new List<Constant_Assignment_Control>();

            ResetFormControls();

            // Perform some additional work if this was not XP theme
            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                browseButton.FlatStyle = FlatStyle.Flat;
                previewCheckBox.FlatStyle = FlatStyle.Flat;
                justSaveMarcXmlCheckBox.FlatStyle = FlatStyle.Flat;
                sourceTextBox.BorderStyle = BorderStyle.FixedSingle;
            }
        }

		#region Windows Form Designer generated code

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MARC_Importer_Form));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnlConstants = new System.Windows.Forms.Panel();
            this.step2Label = new System.Windows.Forms.Label();
            this.step1Label = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.sourceTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.step3Label = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.justSaveMarcXmlCheckBox = new System.Windows.Forms.CheckBox();
            this.previewCheckBox = new System.Windows.Forms.CheckBox();
            this.executeButton = new SobekCM.Management_Tool.Round_Button();
            this.cancelButton = new SobekCM.Management_Tool.Round_Button();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Title = "MARC Source File";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(27, 584);
            this.progressBar1.Maximum = 10;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(541, 13);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 0;
            this.progressBar1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.step2Label);
            this.panel1.Location = new System.Drawing.Point(3, 76);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(574, 387);
            this.panel1.TabIndex = 29;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(27, 32);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(517, 339);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage2.Controls.Add(this.pnlConstants);
            this.tabPage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(509, 313);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Constants";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnlConstants
            // 
            this.pnlConstants.AutoScroll = true;
            this.pnlConstants.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlConstants.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlConstants.Location = new System.Drawing.Point(6, 6);
            this.pnlConstants.Name = "pnlConstants";
            this.pnlConstants.Size = new System.Drawing.Size(499, 301);
            this.pnlConstants.TabIndex = 0;
            // 
            // step2Label
            // 
            this.step2Label.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.step2Label.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.step2Label.Location = new System.Drawing.Point(4, 8);
            this.step2Label.Name = "step2Label";
            this.step2Label.Size = new System.Drawing.Size(544, 21);
            this.step2Label.TabIndex = 28;
            this.step2Label.Text = "Step 2: Select Constants and Copyright Permissions";
            // 
            // step1Label
            // 
            this.step1Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.step1Label.BackColor = System.Drawing.Color.Transparent;
            this.step1Label.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.step1Label.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.step1Label.Location = new System.Drawing.Point(7, 4);
            this.step1Label.Name = "step1Label";
            this.step1Label.Size = new System.Drawing.Size(574, 23);
            this.step1Label.TabIndex = 8;
            this.step1Label.Text = "Step 1: Select the source data file to import MARC files.";
            // 
            // browseButton
            // 
            this.browseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseButton.Location = new System.Drawing.Point(77, 29);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 24);
            this.browseButton.TabIndex = 0;
            this.browseButton.Text = "SELECT";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // sourceTextBox
            // 
            this.sourceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceTextBox.Location = new System.Drawing.Point(158, 30);
            this.sourceTextBox.Name = "sourceTextBox";
            this.sourceTextBox.Size = new System.Drawing.Size(397, 20);
            this.sourceTextBox.TabIndex = 1;
            this.sourceTextBox.TextChanged += new System.EventHandler(this.sourceTextBox_TextChanged);
            this.sourceTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.directoryTextBox_MouseDown);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(32, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 23);
            this.label3.TabIndex = 35;
            this.label3.Text = "File:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStatus
            // 
            this.labelStatus.BackColor = System.Drawing.Color.Transparent;
            this.labelStatus.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(26, 551);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(372, 23);
            this.labelStatus.TabIndex = 37;
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // step3Label
            // 
            this.step3Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.step3Label.BackColor = System.Drawing.Color.Transparent;
            this.step3Label.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.step3Label.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.step3Label.Location = new System.Drawing.Point(7, 476);
            this.step3Label.Name = "step3Label";
            this.step3Label.Size = new System.Drawing.Size(544, 29);
            this.step3Label.TabIndex = 38;
            this.step3Label.Text = "Step 3: Click the Execute button";
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Controls.Add(this.sourceTextBox);
            this.mainPanel.Controls.Add(this.step3Label);
            this.mainPanel.Controls.Add(this.panel1);
            this.mainPanel.Controls.Add(this.step1Label);
            this.mainPanel.Controls.Add(this.browseButton);
            this.mainPanel.Controls.Add(this.label3);
            this.mainPanel.Location = new System.Drawing.Point(12, 5);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(582, 508);
            this.mainPanel.TabIndex = 39;
            // 
            // justSaveMarcXmlCheckBox
            // 
            this.justSaveMarcXmlCheckBox.AutoSize = true;
            this.justSaveMarcXmlCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.justSaveMarcXmlCheckBox.Location = new System.Drawing.Point(43, 527);
            this.justSaveMarcXmlCheckBox.Name = "justSaveMarcXmlCheckBox";
            this.justSaveMarcXmlCheckBox.Size = new System.Drawing.Size(319, 18);
            this.justSaveMarcXmlCheckBox.TabIndex = 40;
            this.justSaveMarcXmlCheckBox.Text = "Just update the local data store; no tracking changes";
            this.justSaveMarcXmlCheckBox.UseVisualStyleBackColor = false;
            this.justSaveMarcXmlCheckBox.CheckedChanged += new System.EventHandler(this.justSaveMarcXmlCheckBox_CheckedChanged);
            // 
            // previewCheckBox
            // 
            this.previewCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.previewCheckBox.AutoSize = true;
            this.previewCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.previewCheckBox.Checked = true;
            this.previewCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.previewCheckBox.Location = new System.Drawing.Point(402, 527);
            this.previewCheckBox.Name = "previewCheckBox";
            this.previewCheckBox.Size = new System.Drawing.Size(166, 18);
            this.previewCheckBox.TabIndex = 41;
            this.previewCheckBox.Text = "Execute in preview mode";
            this.previewCheckBox.UseVisualStyleBackColor = false;
            this.previewCheckBox.CheckedChanged += new System.EventHandler(this.previewCheckBox_CheckedChanged);
            // 
            // executeButton
            // 
            this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.executeButton.BackColor = System.Drawing.Color.Transparent;
            this.executeButton.Button_Enabled = true;
            this.executeButton.Button_Text = "EXECUTE";
            this.executeButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Forward;
            this.executeButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.executeButton.Location = new System.Drawing.Point(494, 548);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(100, 26);
            this.executeButton.TabIndex = 43;
            this.executeButton.Button_Pressed += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.cancelButton.Button_Enabled = true;
            this.cancelButton.Button_Text = "CANCEL";
            this.cancelButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Backward;
            this.cancelButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(379, 548);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 26);
            this.cancelButton.TabIndex = 42;
            this.cancelButton.Button_Pressed += new System.EventHandler(this.cancelButton_Click);
            // 
            // MARC_Importer_Form
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(606, 609);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.previewCheckBox);
            this.Controls.Add(this.justSaveMarcXmlCheckBox);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(614, 641);
            this.Name = "MARC_Importer_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MARC Importer";
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#region Form-Related Event Handlers
      
		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void exitMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void browseButton_Click(object sender, System.EventArgs e)
		{
			Browse_Source();
		}

		private void sourceMenuItem_Click(object sender, System.EventArgs e)
		{
			Browse_Source();
		}

        private void directoryTextBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Browse_Source();
        }

		private void okButton_Click(object sender, System.EventArgs e)
		{        
			Import_Records();
		}

		private void importMenuItem_Click(object sender, System.EventArgs e)
		{
			Import_Records();
		}


		private void instructionMenuItem_Click(object sender, System.EventArgs e)
		{
			Process showInstructions = new Process();
			showInstructions.StartInfo.FileName = Application.StartupPath + "\\Instructions.htm";
			showInstructions.Start();
		}

      
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

        /// <summary> Last tickler assigned through the form, during an import </summary>
        public string Last_Tickler
        {
            get
            {
                return tickler;
            }
        }

		/// <summary> Imports the records from the indicated source file </summary>
		protected void Import_Records()
		{            
            // Step through each constant map control
            Constant_Fields constantCollection = new Constant_Fields();

            foreach (Constant_Assignment_Control thisConstant in constant_map_inputs)
            {                
                constantCollection.Add(thisConstant.Mapped_Field, thisConstant.Mapped_Constant);                
            }

            // METS files are not created from the MARC Importer; set flag to false.
            bool create_mets = false;
            bool deriveCopyrightStatusFromMARC = false;

            // validate the form            
            if (!Validate_Import_MARC_Form())
            {               
                this.Enable_FormControls();
                return;
            }

            // disable some of the form controls
            this.Disable_FormControls();

            // Show the progress bar
            this.progressBar1.Visible = true;
            this.progressBar1.Maximum = 10;
            this.progressBar1.Value = 0;           

            // reset the status label
            labelStatus.Text = "";

            try
            {
                // Create the Processor and assign the Delegate method for event processing.
                processor = new MARC_Importer_Processor(this.sourceTextBox.Text, justSaveMarcXmlCheckBox.Checked, "", constantCollection, previewCheckBox.Checked, workingFolder + "\\ERROR" );
                processor.New_Progress += new New_Importer_Progress_Delegate(processor_New_Progress);
                processor.Complete += new New_Importer_Progress_Delegate(processor_Complete);

                // Create the thread to do the processing work, and start it.                        
                processThread = new Thread(new ThreadStart(processor.Do_Work));
                processThread.SetApartmentState(ApartmentState.STA);    
                processThread.Start();
            }
            catch (Exception e)
            {
                // display the error message
                DLC.Tools.Forms.ErrorMessageBox.Show("Error encountered while processing!\n\n" + e.Message, "DLC Importer Error", e);
                     
                // enable form controls on the Importer form                    
                this.Enable_FormControls();

                this.Cursor = Cursors.Default;
                progressBar1.Value = progressBar1.Minimum;
            }      
		}

        void processor_Complete(int New_Progress)
        {
            // set the Cursor and ProgressBar back to default values.
            this.Cursor = Cursors.Default;
            this.progressBar1.Value = progressBar1.Minimum;

            // enable the form controls
            this.Enable_FormControls();


            // Only continue if there are records already
            if (processor.Report_Data.Rows.Count == 0)
            {
                if (!justSaveMarcXmlCheckBox.Checked)
                {
                    MessageBox.Show("No imported records!    ", "Batch Importer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                // Create a table to display the results
                DataTable displayTbl = processor.Report_Data.Copy();

                // create the Results form
                Results_Form showResults = new Results_Form(processor.Report_Data, processor.Importer_Type, false);

                // hide the Importer form
                this.Hide();

                // show the Results form
                showResults.ShowDialog();

                // show the Importer form
                this.ShowDialog();
            }    
        }

        void processor_New_Progress(int New_Progress)
        {
            // Just increment the progress bar
            if (this.progressBar1.Value + 1 > this.progressBar1.Maximum)
                this.progressBar1.Value = 0;
            else
                this.progressBar1.Value = progressBar1.Value + 1;

            // update status label
            labelStatus.Text = "Processed " + New_Progress.ToString("#,##0;") + " records";                
        }

		/// <summary> Browse to the source file to import </summary>
		protected void Browse_Source()
		{
            // reset the status label
            labelStatus.Text = "";

			// Launch the open file dialog
			if ( this.openFileDialog1.ShowDialog() == DialogResult.OK )
			{
				// Save this file name
				this.sourceTextBox.Text = openFileDialog1.FileName;

				// Determine the output file
				FileInfo inputInfo = new FileInfo( this.sourceTextBox.Text );

				// Enable the ok button
				this.executeButton.Button_Enabled = true;

                // populate the Contants Tab if the constant_map_inputs collection is empty
                if (this.constant_map_inputs.Count == 0)
                {
                    // Create the constants mapping custom control
                    // Add eight constant user controls to panel
                    for (int i = 1; i < 9; i++)
                    {
                        Constant_Assignment_Control thisConstantCtrl = new Constant_Assignment_Control();
                        thisConstantCtrl.Location = new Point(10, 10 + ((i - 1) * 30));
                        this.pnlConstants.Controls.Add(thisConstantCtrl);
                        this.constant_map_inputs.Add(thisConstantCtrl);
                    }

                    // set some of the constant columns to required tracking fields
                    constant_map_inputs[0].Mapped_Name = "Material Type";
                    constant_map_inputs[1].Mapped_Name = "Aggregation Code"; 
                    constant_map_inputs[2].Mapped_Name = "Visibility";
                    constant_map_inputs[3].Mapped_Name = "Tickler";
                    FileInfo fileInfo = new FileInfo(sourceTextBox.Text);
                    string name = fileInfo.Name.Replace(fileInfo.Extension, "");
                    if ( name.ToUpper() == "ODDLOTS" )
                        name = name + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2,'0') + DateTime.Now.Day.ToString().PadLeft(2,'0');
                    constant_map_inputs[3].Mapped_Constant = name;
                }

                // show step 2 instructions
                show_step_2();      
			}
			else
			{
				// Clear the current file
				this.sourceTextBox.Clear();
                this.executeButton.Button_Enabled = false;

                // show step 1 instructions
                show_step_1();  
			}
		}      

        #region Private Methods

        private void ResetFormControls()
        {           
            // set volume copyright permissions to default setting of 'public domain'
            this.user_specified_copyright_permissions = -1;

            // reset the status label
            this.labelStatus.Text = "";

            // show step 1 instructions
            show_step_1();  
        }

        private void Disable_FormControls()
        {
            // Disable the OK button and the related Menu Item
            this.executeButton.Button_Enabled = false;

            // Disable Tab Panel
            this.pnlConstants.Enabled = false;

            // Disable the Exit button and the related Menu Item
            this.cancelButton.Enabled = false;

            // Disable the Browse button and the related Menu Item
            this.browseButton.Enabled = false;
            this.sourceTextBox.Enabled = false;

            this.justSaveMarcXmlCheckBox.Enabled = false;
            this.previewCheckBox.Enabled = false;
        }

        private void Enable_FormControls()
        {
            // Enable the OK button and the related Menu Item
            this.executeButton.Button_Enabled = true;

            // Enable Tab Panel
            this.pnlConstants.Enabled = true;

            // Enable the Exit button and the related Menu Item
            this.cancelButton.Enabled = true;

            // Enable the Browse button and the related Menu Item
            this.browseButton.Enabled = true;
            this.sourceTextBox.Enabled = true;

            this.justSaveMarcXmlCheckBox.Enabled = true;
            this.previewCheckBox.Enabled = true;
        }

        private bool Validate_Import_MARC_Form()
        {
            bool retValue = false;

            // Check constant mappings to determine if the required fields 
            // necessary to create a recored in the Tracking database exist. 

            // collection for holding error messages
            StringCollection errors = new StringCollection();

            string material_type = String.Empty;

            bool hasType = false;
            bool hasEntityType = false;
            bool copyright_data_error = false;


            // step through each constant mapping        
            // check if required field has been selected from the Constants tab control, 
            // and that data has been selected from the adjoining combo box
            foreach (Constant_Assignment_Control thisConstant in constant_map_inputs)
            {
                // check material type
                if ((thisConstant.Mapped_Name.Equals("Material Type"))
                        && (thisConstant.Mapped_Constant.Length > 0))
                {
                    hasType = true;
                    material_type = thisConstant.Mapped_Constant;
                    continue;
                }

                // check entity type
                if ((thisConstant.Mapped_Name.Equals("Entity Type"))
                        && (thisConstant.Mapped_Constant.Length > 0))
                {
                    hasEntityType = true;
                    continue;
                }
               
                if (hasType && hasEntityType)
                    break;
            }

           
            // if there were errors, select a Tab page to be the active Tab
            if (copyright_data_error)
                // select the 'Copyright Permissions' Tab Page
                this.tabControl1.SelectedIndex = 1;
            else if (!hasType)
                // select the 'Constants' Tab Page
                this.tabControl1.SelectedIndex = 0;   
            


            // if there were validation errors, build error message array                
            if (errors.Count > 0)
            {
                retValue = false;
                string[] missing = new string[errors.Count];

                for (int i = 0; i < errors.Count; i++)
                {
                    missing[i] = errors[i];
                }

                if (missing.Length > 0)
                {
                    string message = "The following required fields are either missing or invalid:               \n\n";
                  
                    foreach (string thisMissing in missing)
                        message = message + "* " + thisMissing + "\n\n";
                    MessageBox.Show(message + "\nPlease complete these fields to continue.", "Invalid Entries", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                // passed form validation
                retValue = true;


            return retValue;
        }

        private void show_step_1()
        {
            // show Step 1 instructions
            this.step1Label.ForeColor = ControlPaint.Dark(SystemColors.ActiveCaption);
            this.step2Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);
            this.step3Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);
        }

        private void show_step_2()
        {
            // show Step 2 instructions          
            this.step1Label.ForeColor = ControlPaint.LightLight(SystemColors.ActiveCaption);
            this.step2Label.ForeColor = ControlPaint.Dark(SystemColors.ActiveCaption);
            this.step3Label.ForeColor = ControlPaint.Dark(SystemColors.ActiveCaption);
        }
        #endregion   
        
        public void Show_Form()
        {
            // show the MARC Importer form
            if (this != null)
                this.ShowDialog();
        }

        private void sourceTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void previewCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (previewCheckBox.Checked)
                justSaveMarcXmlCheckBox.Checked = false;
        }

        private void justSaveMarcXmlCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (justSaveMarcXmlCheckBox.Checked)
                previewCheckBox.Checked = false;
        }


    }
}
