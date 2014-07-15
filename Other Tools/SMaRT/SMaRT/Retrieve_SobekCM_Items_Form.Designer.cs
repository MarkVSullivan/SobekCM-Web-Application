namespace SobekCM.Management_Tool
{
    partial class Retrieve_SobekCM_Items_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Retrieve_SobekCM_Items_Form));
            this.panel1 = new System.Windows.Forms.Panel();
            this.instructionsLabel = new System.Windows.Forms.Label();
            this.marcXmlRadioButton = new System.Windows.Forms.RadioButton();
            this.completeRadioButton = new System.Windows.Forms.RadioButton();
            this.metsOnlyRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.destinationTextBox = new System.Windows.Forms.TextBox();
            this.destinationLabel = new System.Windows.Forms.Label();
            this.sobekcmQueryTextBox = new System.Windows.Forms.TextBox();
            this.queryLabel = new System.Windows.Forms.Label();
            this.mainLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.okButton = new SobekCM.Management_Tool.Round_Button();
            this.exitButton = new SobekCM.Management_Tool.Round_Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.instructionsLabel);
            this.panel1.Controls.Add(this.marcXmlRadioButton);
            this.panel1.Controls.Add(this.completeRadioButton);
            this.panel1.Controls.Add(this.metsOnlyRadioButton);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.browseButton);
            this.panel1.Controls.Add(this.destinationTextBox);
            this.panel1.Controls.Add(this.destinationLabel);
            this.panel1.Controls.Add(this.sobekcmQueryTextBox);
            this.panel1.Controls.Add(this.queryLabel);
            this.panel1.Location = new System.Drawing.Point(13, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(627, 187);
            this.panel1.TabIndex = 17;
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instructionsLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.instructionsLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.instructionsLabel.Location = new System.Drawing.Point(17, 9);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Size = new System.Drawing.Size(592, 42);
            this.instructionsLabel.TabIndex = 10;
            this.instructionsLabel.Text = "Enter a query from a SobekCM library below to retrieve metadata or online resourc" +
                "e files for multiple items at once.";
            // 
            // marcXmlRadioButton
            // 
            this.marcXmlRadioButton.AutoSize = true;
            this.marcXmlRadioButton.Location = new System.Drawing.Point(260, 148);
            this.marcXmlRadioButton.Name = "marcXmlRadioButton";
            this.marcXmlRadioButton.Size = new System.Drawing.Size(82, 18);
            this.marcXmlRadioButton.TabIndex = 9;
            this.marcXmlRadioButton.Text = "MARC XML";
            this.marcXmlRadioButton.UseVisualStyleBackColor = true;
            // 
            // completeRadioButton
            // 
            this.completeRadioButton.AutoSize = true;
            this.completeRadioButton.Location = new System.Drawing.Point(406, 148);
            this.completeRadioButton.Name = "completeRadioButton";
            this.completeRadioButton.Size = new System.Drawing.Size(126, 18);
            this.completeRadioButton.TabIndex = 8;
            this.completeRadioButton.Text = "Complete Package";
            this.completeRadioButton.UseVisualStyleBackColor = true;
            this.completeRadioButton.CheckedChanged += new System.EventHandler(this.completeRadioButton_CheckedChanged);
            // 
            // metsOnlyRadioButton
            // 
            this.metsOnlyRadioButton.AutoSize = true;
            this.metsOnlyRadioButton.Checked = true;
            this.metsOnlyRadioButton.Location = new System.Drawing.Point(126, 148);
            this.metsOnlyRadioButton.Name = "metsOnlyRadioButton";
            this.metsOnlyRadioButton.Size = new System.Drawing.Size(84, 18);
            this.metsOnlyRadioButton.TabIndex = 6;
            this.metsOnlyRadioButton.TabStop = true;
            this.metsOnlyRadioButton.Text = "METS Only";
            this.metsOnlyRadioButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Retrieval Type:";
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(534, 94);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 4;
            this.browseButton.Text = "BROWSE";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // destinationTextBox
            // 
            this.destinationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.destinationTextBox.Location = new System.Drawing.Point(100, 94);
            this.destinationTextBox.Name = "destinationTextBox";
            this.destinationTextBox.Size = new System.Drawing.Size(420, 22);
            this.destinationTextBox.TabIndex = 3;
            this.destinationTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.destinationTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // destinationLabel
            // 
            this.destinationLabel.AutoSize = true;
            this.destinationLabel.Location = new System.Drawing.Point(17, 97);
            this.destinationLabel.Name = "destinationLabel";
            this.destinationLabel.Size = new System.Drawing.Size(72, 14);
            this.destinationLabel.TabIndex = 2;
            this.destinationLabel.Text = "Destination:";
            // 
            // sobekcmQueryTextBox
            // 
            this.sobekcmQueryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sobekcmQueryTextBox.Location = new System.Drawing.Point(121, 54);
            this.sobekcmQueryTextBox.Name = "sobekcmQueryTextBox";
            this.sobekcmQueryTextBox.Size = new System.Drawing.Size(488, 22);
            this.sobekcmQueryTextBox.TabIndex = 1;
            this.sobekcmQueryTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.sobekcmQueryTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // queryLabel
            // 
            this.queryLabel.AutoSize = true;
            this.queryLabel.Location = new System.Drawing.Point(17, 57);
            this.queryLabel.Name = "queryLabel";
            this.queryLabel.Size = new System.Drawing.Size(98, 14);
            this.queryLabel.TabIndex = 0;
            this.queryLabel.Text = "SobekCM Query:";
            // 
            // mainLabel
            // 
            this.mainLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mainLabel.BackColor = System.Drawing.Color.Transparent;
            this.mainLabel.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel.ForeColor = System.Drawing.Color.MediumBlue;
            this.mainLabel.Location = new System.Drawing.Point(65, 11);
            this.mainLabel.Name = "mainLabel";
            this.mainLabel.Size = new System.Drawing.Size(519, 43);
            this.mainLabel.TabIndex = 16;
            this.mainLabel.Text = "Retrieve SobekCM Items Form";
            this.mainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select a folder to use as the destination.";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(13, 258);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(400, 23);
            this.progressBar1.TabIndex = 20;
            this.progressBar1.Visible = false;
            // 
            // progressBar2
            // 
            this.progressBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar2.Location = new System.Drawing.Point(13, 287);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(400, 23);
            this.progressBar2.TabIndex = 21;
            this.progressBar2.Visible = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.BackColor = System.Drawing.Color.Transparent;
            this.okButton.Button_Enabled = true;
            this.okButton.Button_Text = "OK";
            this.okButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Forward;
            this.okButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(546, 258);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(94, 26);
            this.okButton.TabIndex = 19;
            this.okButton.Button_Pressed += new System.EventHandler(this.okButton_Button_Pressed);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.BackColor = System.Drawing.Color.Transparent;
            this.exitButton.Button_Enabled = true;
            this.exitButton.Button_Text = "CANCEL";
            this.exitButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Backward;
            this.exitButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(437, 258);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(94, 26);
            this.exitButton.TabIndex = 18;
            this.exitButton.Button_Pressed += new System.EventHandler(this.exitButton_Button_Pressed);
            // 
            // Retrieve_SobekCM_Items_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(652, 320);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mainLabel);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(864, 419);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(556, 319);
            this.Name = "Retrieve_SobekCM_Items_Form";
            this.Text = "Retrieve SobekCM Items Form";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Round_Button exitButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.Label queryLabel;
        private System.Windows.Forms.TextBox sobekcmQueryTextBox;
        private System.Windows.Forms.TextBox destinationTextBox;
        private System.Windows.Forms.Label destinationLabel;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private Round_Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton completeRadioButton;
        private System.Windows.Forms.RadioButton metsOnlyRadioButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.RadioButton marcXmlRadioButton;
        private System.Windows.Forms.Label instructionsLabel;
    }
}