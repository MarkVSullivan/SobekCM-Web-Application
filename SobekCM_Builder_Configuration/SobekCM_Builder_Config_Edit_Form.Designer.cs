namespace SobekCM.Configuration
{
    partial class SobekCM_Builder_Config_Edit_Form
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SobekCM_Builder_Config_Edit_Form));
            this.titleLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ghostscriptComboBox = new System.Windows.Forms.ComboBox();
            this.ghostscriptLabel = new System.Windows.Forms.Label();
            this.imageMagickComboBox = new System.Windows.Forms.ComboBox();
            this.imagemagickLabel = new System.Windows.Forms.Label();
            this.ghostscriptHelpPictureBox = new System.Windows.Forms.PictureBox();
            this.ghostscriptBrowsePictureBox = new System.Windows.Forms.PictureBox();
            this.imagemagickHelpPictureBox = new System.Windows.Forms.PictureBox();
            this.imageMagickBrowsePictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.createConnectionStringButton = new System.Windows.Forms.PictureBox();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.connectionTextBox = new System.Windows.Forms.TextBox();
            this.dbTypePictureBox = new System.Windows.Forms.PictureBox();
            this.testConnectionButton = new SobekCM.Configuration.Round_Button();
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.databaseNameLabel = new System.Windows.Forms.Label();
            this.databaseNamePictureBox = new System.Windows.Forms.PictureBox();
            this.mainLabel1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveButton = new SobekCM.Configuration.Round_Button();
            this.cancelButton = new SobekCM.Configuration.Round_Button();
            this.builderFoldersButton = new SobekCM.Configuration.Round_Button();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ghostscriptHelpPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ghostscriptBrowsePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imagemagickHelpPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMagickBrowsePictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.createConnectionStringButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dbTypePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseNamePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.titleLabel.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.MediumBlue;
            this.titleLabel.Location = new System.Drawing.Point(7, 7);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(628, 42);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "SobekCM Builder / Bulk Loader Configuration";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.mainLabel1);
            this.panel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(9, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(624, 347);
            this.panel1.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ghostscriptComboBox);
            this.groupBox2.Controls.Add(this.ghostscriptLabel);
            this.groupBox2.Controls.Add(this.imageMagickComboBox);
            this.groupBox2.Controls.Add(this.imagemagickLabel);
            this.groupBox2.Controls.Add(this.ghostscriptHelpPictureBox);
            this.groupBox2.Controls.Add(this.ghostscriptBrowsePictureBox);
            this.groupBox2.Controls.Add(this.imagemagickHelpPictureBox);
            this.groupBox2.Controls.Add(this.imageMagickBrowsePictureBox);
            this.groupBox2.Location = new System.Drawing.Point(15, 212);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(596, 117);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "External Executables";
            // 
            // ghostscriptComboBox
            // 
            this.ghostscriptComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ghostscriptComboBox.FormattingEnabled = true;
            this.ghostscriptComboBox.Location = new System.Drawing.Point(199, 35);
            this.ghostscriptComboBox.Name = "ghostscriptComboBox";
            this.ghostscriptComboBox.Size = new System.Drawing.Size(319, 22);
            this.ghostscriptComboBox.TabIndex = 6;
            this.ghostscriptComboBox.Enter += new System.EventHandler(this.comboBox_Enter);
            this.ghostscriptComboBox.Leave += new System.EventHandler(this.comboBox_Leave);
            // 
            // ghostscriptLabel
            // 
            this.ghostscriptLabel.AutoSize = true;
            this.ghostscriptLabel.Location = new System.Drawing.Point(20, 38);
            this.ghostscriptLabel.Name = "ghostscriptLabel";
            this.ghostscriptLabel.Size = new System.Drawing.Size(157, 14);
            this.ghostscriptLabel.TabIndex = 5;
            this.ghostscriptLabel.Text = "Ghostscript Executable File:";
            // 
            // imageMagickComboBox
            // 
            this.imageMagickComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageMagickComboBox.FormattingEnabled = true;
            this.imageMagickComboBox.Location = new System.Drawing.Point(199, 65);
            this.imageMagickComboBox.Name = "imageMagickComboBox";
            this.imageMagickComboBox.Size = new System.Drawing.Size(319, 22);
            this.imageMagickComboBox.TabIndex = 8;
            this.imageMagickComboBox.Enter += new System.EventHandler(this.comboBox_Enter);
            this.imageMagickComboBox.Leave += new System.EventHandler(this.comboBox_Leave);
            // 
            // imagemagickLabel
            // 
            this.imagemagickLabel.AutoSize = true;
            this.imagemagickLabel.Location = new System.Drawing.Point(20, 68);
            this.imagemagickLabel.Name = "imagemagickLabel";
            this.imagemagickLabel.Size = new System.Drawing.Size(166, 14);
            this.imagemagickLabel.TabIndex = 7;
            this.imagemagickLabel.Text = "ImageMagick Executable File:";
            // 
            // ghostscriptHelpPictureBox
            // 
            this.ghostscriptHelpPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ghostscriptHelpPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ghostscriptHelpPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ghostscriptHelpPictureBox.Image")));
            this.ghostscriptHelpPictureBox.Location = new System.Drawing.Point(552, 38);
            this.ghostscriptHelpPictureBox.Name = "ghostscriptHelpPictureBox";
            this.ghostscriptHelpPictureBox.Size = new System.Drawing.Size(19, 19);
            this.ghostscriptHelpPictureBox.TabIndex = 18;
            this.ghostscriptHelpPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.ghostscriptHelpPictureBox, "Help");
            this.ghostscriptHelpPictureBox.Click += new System.EventHandler(this.ghostscriptHelpPictureBox_Click);
            // 
            // ghostscriptBrowsePictureBox
            // 
            this.ghostscriptBrowsePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ghostscriptBrowsePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ghostscriptBrowsePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ghostscriptBrowsePictureBox.Image")));
            this.ghostscriptBrowsePictureBox.Location = new System.Drawing.Point(524, 38);
            this.ghostscriptBrowsePictureBox.Name = "ghostscriptBrowsePictureBox";
            this.ghostscriptBrowsePictureBox.Size = new System.Drawing.Size(19, 19);
            this.ghostscriptBrowsePictureBox.TabIndex = 22;
            this.ghostscriptBrowsePictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.ghostscriptBrowsePictureBox, "Browse for Ghostscript Executable");
            this.ghostscriptBrowsePictureBox.Click += new System.EventHandler(this.ghostscriptBrowsePictureBox_Click);
            // 
            // imagemagickHelpPictureBox
            // 
            this.imagemagickHelpPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imagemagickHelpPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imagemagickHelpPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("imagemagickHelpPictureBox.Image")));
            this.imagemagickHelpPictureBox.Location = new System.Drawing.Point(552, 68);
            this.imagemagickHelpPictureBox.Name = "imagemagickHelpPictureBox";
            this.imagemagickHelpPictureBox.Size = new System.Drawing.Size(19, 19);
            this.imagemagickHelpPictureBox.TabIndex = 19;
            this.imagemagickHelpPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.imagemagickHelpPictureBox, "Help");
            this.imagemagickHelpPictureBox.Click += new System.EventHandler(this.imagemagickHelpPictureBox_Click);
            // 
            // imageMagickBrowsePictureBox
            // 
            this.imageMagickBrowsePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imageMagickBrowsePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imageMagickBrowsePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("imageMagickBrowsePictureBox.Image")));
            this.imageMagickBrowsePictureBox.Location = new System.Drawing.Point(524, 68);
            this.imageMagickBrowsePictureBox.Name = "imageMagickBrowsePictureBox";
            this.imageMagickBrowsePictureBox.Size = new System.Drawing.Size(19, 19);
            this.imageMagickBrowsePictureBox.TabIndex = 21;
            this.imageMagickBrowsePictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.imageMagickBrowsePictureBox, "Browse for ImageMagick Executable");
            this.imageMagickBrowsePictureBox.Click += new System.EventHandler(this.imageMagickBrowsePictureBox_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.createConnectionStringButton);
            this.groupBox1.Controls.Add(this.typeComboBox);
            this.groupBox1.Controls.Add(this.connectionTextBox);
            this.groupBox1.Controls.Add(this.dbTypePictureBox);
            this.groupBox1.Controls.Add(this.testConnectionButton);
            this.groupBox1.Controls.Add(this.serverNameLabel);
            this.groupBox1.Controls.Add(this.databaseNameLabel);
            this.groupBox1.Controls.Add(this.databaseNamePictureBox);
            this.groupBox1.Location = new System.Drawing.Point(15, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(596, 148);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SobekCM Database Information";
            // 
            // createConnectionStringButton
            // 
            this.createConnectionStringButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createConnectionStringButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.createConnectionStringButton.Image = ((System.Drawing.Image)(resources.GetObject("createConnectionStringButton.Image")));
            this.createConnectionStringButton.Location = new System.Drawing.Point(524, 68);
            this.createConnectionStringButton.Name = "createConnectionStringButton";
            this.createConnectionStringButton.Size = new System.Drawing.Size(25, 25);
            this.createConnectionStringButton.TabIndex = 27;
            this.createConnectionStringButton.TabStop = false;
            this.toolTip1.SetToolTip(this.createConnectionStringButton, "Create the MSSQL Connection String");
            this.createConnectionStringButton.Click += new System.EventHandler(this.createConnectionStringButton_Click);
            // 
            // typeComboBox
            // 
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Items.AddRange(new object[] {
            "MSSQL",
            "PostrgeSQL"});
            this.typeComboBox.Location = new System.Drawing.Point(142, 33);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(121, 22);
            this.typeComboBox.TabIndex = 26;
            this.typeComboBox.Enter += new System.EventHandler(this.comboBox_Enter);
            this.typeComboBox.Leave += new System.EventHandler(this.typeComboBox_Leave);
            // 
            // connectionTextBox
            // 
            this.connectionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionTextBox.Location = new System.Drawing.Point(142, 68);
            this.connectionTextBox.Name = "connectionTextBox";
            this.connectionTextBox.Size = new System.Drawing.Size(376, 22);
            this.connectionTextBox.TabIndex = 25;
            this.connectionTextBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.connectionTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.connectionTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // dbTypePictureBox
            // 
            this.dbTypePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dbTypePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("dbTypePictureBox.Image")));
            this.dbTypePictureBox.Location = new System.Drawing.Point(269, 33);
            this.dbTypePictureBox.Name = "dbTypePictureBox";
            this.dbTypePictureBox.Size = new System.Drawing.Size(19, 19);
            this.dbTypePictureBox.TabIndex = 16;
            this.dbTypePictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.dbTypePictureBox, "Help");
            this.dbTypePictureBox.Click += new System.EventHandler(this.serverNamePictureBox_Click);
            // 
            // testConnectionButton
            // 
            this.testConnectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testConnectionButton.Button_Enabled = false;
            this.testConnectionButton.Button_Text = "TEST CONNECTION";
            this.testConnectionButton.Button_Type = SobekCM.Configuration.Round_Button.Button_Type_Enum.Standard;
            this.testConnectionButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.testConnectionButton.Location = new System.Drawing.Point(372, 99);
            this.testConnectionButton.Name = "testConnectionButton";
            this.testConnectionButton.Size = new System.Drawing.Size(157, 32);
            this.testConnectionButton.TabIndex = 23;
            this.testConnectionButton.Button_Pressed += new System.EventHandler(this.testConnectionButton_Button_Pressed);
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.Location = new System.Drawing.Point(15, 41);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(93, 14);
            this.serverNameLabel.TabIndex = 1;
            this.serverNameLabel.Text = "Database Type:";
            // 
            // databaseNameLabel
            // 
            this.databaseNameLabel.AutoSize = true;
            this.databaseNameLabel.Location = new System.Drawing.Point(15, 71);
            this.databaseNameLabel.Name = "databaseNameLabel";
            this.databaseNameLabel.Size = new System.Drawing.Size(109, 14);
            this.databaseNameLabel.TabIndex = 3;
            this.databaseNameLabel.Text = "Connection String:";
            // 
            // databaseNamePictureBox
            // 
            this.databaseNamePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseNamePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.databaseNamePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("databaseNamePictureBox.Image")));
            this.databaseNamePictureBox.Location = new System.Drawing.Point(552, 70);
            this.databaseNamePictureBox.Name = "databaseNamePictureBox";
            this.databaseNamePictureBox.Size = new System.Drawing.Size(19, 19);
            this.databaseNamePictureBox.TabIndex = 17;
            this.databaseNamePictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.databaseNamePictureBox, "Help");
            this.databaseNamePictureBox.Click += new System.EventHandler(this.databaseNamePictureBox_Click);
            // 
            // mainLabel1
            // 
            this.mainLabel1.AutoSize = true;
            this.mainLabel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.mainLabel1.Location = new System.Drawing.Point(30, 16);
            this.mainLabel1.Name = "mainLabel1";
            this.mainLabel1.Size = new System.Drawing.Size(540, 14);
            this.mainLabel1.TabIndex = 0;
            this.mainLabel1.Text = "Change any of the settings found in the builder/bulk loader\'s configuration file " +
    "below:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Button_Enabled = true;
            this.saveButton.Button_Text = "SAVE";
            this.saveButton.Button_Type = SobekCM.Configuration.Round_Button.Button_Type_Enum.Forward;
            this.saveButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(520, 409);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(101, 32);
            this.saveButton.TabIndex = 3;
            this.saveButton.Button_Pressed += new System.EventHandler(this.saveButton_Button_Pressed);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Button_Enabled = true;
            this.cancelButton.Button_Text = "CANCEL";
            this.cancelButton.Button_Type = SobekCM.Configuration.Round_Button.Button_Type_Enum.Backward;
            this.cancelButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(395, 409);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(101, 32);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Button_Pressed += new System.EventHandler(this.cancelButton_Button_Pressed);
            // 
            // builderFoldersButton
            // 
            this.builderFoldersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.builderFoldersButton.Button_Enabled = true;
            this.builderFoldersButton.Button_Text = "BUILDER FOLDERS";
            this.builderFoldersButton.Button_Type = SobekCM.Configuration.Round_Button.Button_Type_Enum.Standard;
            this.builderFoldersButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.builderFoldersButton.Location = new System.Drawing.Point(12, 409);
            this.builderFoldersButton.Name = "builderFoldersButton";
            this.builderFoldersButton.Size = new System.Drawing.Size(146, 32);
            this.builderFoldersButton.TabIndex = 24;
            this.toolTip1.SetToolTip(this.builderFoldersButton, "Configure inbound folders for the builder");
            this.builderFoldersButton.Button_Pressed += new System.EventHandler(this.builderFoldersButton_Button_Pressed);
            // 
            // SobekCM_Builder_Config_Edit_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(650, 460);
            this.Controls.Add(this.builderFoldersButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1000, 487);
            this.MinimumSize = new System.Drawing.Size(658, 387);
            this.Name = "SobekCM_Builder_Config_Edit_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SobekCM Builder Config Edit Form";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ghostscriptHelpPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ghostscriptBrowsePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imagemagickHelpPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMagickBrowsePictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.createConnectionStringButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dbTypePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseNamePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Round_Button saveButton;
        private Round_Button cancelButton;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox imagemagickHelpPictureBox;
        private System.Windows.Forms.PictureBox ghostscriptHelpPictureBox;
        private System.Windows.Forms.PictureBox databaseNamePictureBox;
        private System.Windows.Forms.PictureBox dbTypePictureBox;
        private System.Windows.Forms.Label imagemagickLabel;
        private System.Windows.Forms.Label ghostscriptLabel;
        private System.Windows.Forms.Label databaseNameLabel;
        private System.Windows.Forms.Label serverNameLabel;
        private System.Windows.Forms.Label mainLabel1;
        private System.Windows.Forms.PictureBox imageMagickBrowsePictureBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox ghostscriptBrowsePictureBox;
        private System.Windows.Forms.ComboBox imageMagickComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox ghostscriptComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox connectionTextBox;
        private Round_Button testConnectionButton;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.PictureBox createConnectionStringButton;
        private Round_Button builderFoldersButton;
    }
}