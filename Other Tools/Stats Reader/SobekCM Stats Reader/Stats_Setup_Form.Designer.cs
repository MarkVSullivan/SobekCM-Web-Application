namespace SobekCM_Stats_Reader
{
    partial class Stats_Setup_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stats_Setup_Form));
            this.logLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.outputScriptRadioButton = new System.Windows.Forms.RadioButton();
            this.updateDbRadioButton = new System.Windows.Forms.RadioButton();
            this.sqlLabel = new System.Windows.Forms.Label();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.sqlTextBox = new System.Windows.Forms.TextBox();
            this.datasetTextBox = new System.Windows.Forms.TextBox();
            this.datasetLabel = new System.Windows.Forms.Label();
            this.month2TextBox = new System.Windows.Forms.TextBox();
            this.year2TextBox = new System.Windows.Forms.TextBox();
            this.month1TextBox = new System.Windows.Forms.TextBox();
            this.year1TextBox = new System.Windows.Forms.TextBox();
            this.month2Label = new System.Windows.Forms.Label();
            this.year2Label = new System.Windows.Forms.Label();
            this.month1Label = new System.Windows.Forms.Label();
            this.year1Label = new System.Windows.Forms.Label();
            this.dateRangeLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.dbTextBox = new System.Windows.Forms.TextBox();
            this.dbLabel = new System.Windows.Forms.Label();
            this.createConnectionStringButton = new System.Windows.Forms.PictureBox();
            this.databaseNamePictureBox = new System.Windows.Forms.PictureBox();
            this.logBrowseButton2 = new SobekCM_Stats_Reader.Round_Button();
            this.datasetBrowseButton2 = new SobekCM_Stats_Reader.Round_Button();
            this.sqlBrowseButton2 = new SobekCM_Stats_Reader.Round_Button();
            this.instructionsLabel = new System.Windows.Forms.Label();
            this.mainLabel = new System.Windows.Forms.Label();
            this.okRoundButton = new SobekCM_Stats_Reader.Round_Button();
            this.cancelRoundButton = new SobekCM_Stats_Reader.Round_Button();
            this.helpPictureBox = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.resultPanel = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.sobekBrowseButton = new SobekCM_Stats_Reader.Round_Button();
            this.sobekTextBox = new System.Windows.Forms.TextBox();
            this.sobekLabel = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.createConnectionStringButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseNamePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).BeginInit();
            this.resultPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // logLabel
            // 
            this.logLabel.AutoSize = true;
            this.logLabel.Location = new System.Drawing.Point(32, 118);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(133, 14);
            this.logLabel.TabIndex = 4;
            this.logLabel.Text = "IIS Web Log Directory:";
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(32, 207);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(137, 14);
            this.outputLabel.TabIndex = 10;
            this.outputLabel.Text = "Database Update Type:";
            // 
            // outputScriptRadioButton
            // 
            this.outputScriptRadioButton.AutoSize = true;
            this.outputScriptRadioButton.Checked = true;
            this.outputScriptRadioButton.Location = new System.Drawing.Point(201, 207);
            this.outputScriptRadioButton.Name = "outputScriptRadioButton";
            this.outputScriptRadioButton.Size = new System.Drawing.Size(126, 18);
            this.outputScriptRadioButton.TabIndex = 11;
            this.outputScriptRadioButton.TabStop = true;
            this.outputScriptRadioButton.Text = "Output SQL Script";
            this.outputScriptRadioButton.UseVisualStyleBackColor = true;
            // 
            // updateDbRadioButton
            // 
            this.updateDbRadioButton.AutoSize = true;
            this.updateDbRadioButton.Enabled = false;
            this.updateDbRadioButton.Location = new System.Drawing.Point(379, 207);
            this.updateDbRadioButton.Name = "updateDbRadioButton";
            this.updateDbRadioButton.Size = new System.Drawing.Size(163, 18);
            this.updateDbRadioButton.TabIndex = 12;
            this.updateDbRadioButton.Text = "Update Database Directly";
            this.updateDbRadioButton.UseVisualStyleBackColor = true;
            // 
            // sqlLabel
            // 
            this.sqlLabel.AutoSize = true;
            this.sqlLabel.Location = new System.Drawing.Point(32, 250);
            this.sqlLabel.Name = "sqlLabel";
            this.sqlLabel.Size = new System.Drawing.Size(165, 14);
            this.sqlLabel.TabIndex = 13;
            this.sqlLabel.Text = "SQL Script Output Directory:";
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.ForeColor = System.Drawing.Color.Blue;
            this.logTextBox.Location = new System.Drawing.Point(203, 115);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.Size = new System.Drawing.Size(415, 22);
            this.logTextBox.TabIndex = 5;
            this.logTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.logTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // sqlTextBox
            // 
            this.sqlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlTextBox.ForeColor = System.Drawing.Color.Blue;
            this.sqlTextBox.Location = new System.Drawing.Point(203, 247);
            this.sqlTextBox.Name = "sqlTextBox";
            this.sqlTextBox.Size = new System.Drawing.Size(415, 22);
            this.sqlTextBox.TabIndex = 14;
            this.sqlTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.sqlTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // datasetTextBox
            // 
            this.datasetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datasetTextBox.ForeColor = System.Drawing.Color.Blue;
            this.datasetTextBox.Location = new System.Drawing.Point(203, 161);
            this.datasetTextBox.Name = "datasetTextBox";
            this.datasetTextBox.Size = new System.Drawing.Size(415, 22);
            this.datasetTextBox.TabIndex = 8;
            this.datasetTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.datasetTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // datasetLabel
            // 
            this.datasetLabel.AutoSize = true;
            this.datasetLabel.Location = new System.Drawing.Point(32, 164);
            this.datasetLabel.Name = "datasetLabel";
            this.datasetLabel.Size = new System.Drawing.Size(117, 14);
            this.datasetLabel.TabIndex = 7;
            this.datasetLabel.Text = "Dataset Workspace:";
            // 
            // month2TextBox
            // 
            this.month2TextBox.ForeColor = System.Drawing.Color.Blue;
            this.month2TextBox.Location = new System.Drawing.Point(329, 336);
            this.month2TextBox.Name = "month2TextBox";
            this.month2TextBox.Size = new System.Drawing.Size(59, 22);
            this.month2TextBox.TabIndex = 24;
            this.month2TextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.month2TextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // year2TextBox
            // 
            this.year2TextBox.ForeColor = System.Drawing.Color.Blue;
            this.year2TextBox.Location = new System.Drawing.Point(186, 336);
            this.year2TextBox.Name = "year2TextBox";
            this.year2TextBox.Size = new System.Drawing.Size(65, 22);
            this.year2TextBox.TabIndex = 22;
            this.year2TextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.year2TextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // month1TextBox
            // 
            this.month1TextBox.ForeColor = System.Drawing.Color.Blue;
            this.month1TextBox.Location = new System.Drawing.Point(329, 298);
            this.month1TextBox.Name = "month1TextBox";
            this.month1TextBox.Size = new System.Drawing.Size(59, 22);
            this.month1TextBox.TabIndex = 20;
            this.month1TextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.month1TextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // year1TextBox
            // 
            this.year1TextBox.ForeColor = System.Drawing.Color.Blue;
            this.year1TextBox.Location = new System.Drawing.Point(186, 298);
            this.year1TextBox.Name = "year1TextBox";
            this.year1TextBox.Size = new System.Drawing.Size(65, 22);
            this.year1TextBox.TabIndex = 18;
            this.year1TextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.year1TextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // month2Label
            // 
            this.month2Label.AutoSize = true;
            this.month2Label.Location = new System.Drawing.Point(275, 339);
            this.month2Label.Name = "month2Label";
            this.month2Label.Size = new System.Drawing.Size(53, 14);
            this.month2Label.TabIndex = 23;
            this.month2Label.Text = "Month2:";
            // 
            // year2Label
            // 
            this.year2Label.AutoSize = true;
            this.year2Label.Location = new System.Drawing.Point(137, 339);
            this.year2Label.Name = "year2Label";
            this.year2Label.Size = new System.Drawing.Size(43, 14);
            this.year2Label.TabIndex = 21;
            this.year2Label.Text = "Year2:";
            // 
            // month1Label
            // 
            this.month1Label.AutoSize = true;
            this.month1Label.Location = new System.Drawing.Point(270, 301);
            this.month1Label.Name = "month1Label";
            this.month1Label.Size = new System.Drawing.Size(53, 14);
            this.month1Label.TabIndex = 19;
            this.month1Label.Text = "Month1:";
            // 
            // year1Label
            // 
            this.year1Label.AutoSize = true;
            this.year1Label.Location = new System.Drawing.Point(137, 301);
            this.year1Label.Name = "year1Label";
            this.year1Label.Size = new System.Drawing.Size(43, 14);
            this.year1Label.TabIndex = 17;
            this.year1Label.Text = "Year1:";
            // 
            // dateRangeLabel
            // 
            this.dateRangeLabel.AutoSize = true;
            this.dateRangeLabel.Location = new System.Drawing.Point(31, 301);
            this.dateRangeLabel.Name = "dateRangeLabel";
            this.dateRangeLabel.Size = new System.Drawing.Size(75, 14);
            this.dateRangeLabel.TabIndex = 16;
            this.dateRangeLabel.Text = "Date Range:";
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Controls.Add(this.sobekBrowseButton);
            this.mainPanel.Controls.Add(this.sobekTextBox);
            this.mainPanel.Controls.Add(this.sobekLabel);
            this.mainPanel.Controls.Add(this.dbTextBox);
            this.mainPanel.Controls.Add(this.dbLabel);
            this.mainPanel.Controls.Add(this.createConnectionStringButton);
            this.mainPanel.Controls.Add(this.databaseNamePictureBox);
            this.mainPanel.Controls.Add(this.logBrowseButton2);
            this.mainPanel.Controls.Add(this.datasetBrowseButton2);
            this.mainPanel.Controls.Add(this.sqlBrowseButton2);
            this.mainPanel.Controls.Add(this.instructionsLabel);
            this.mainPanel.Controls.Add(this.logTextBox);
            this.mainPanel.Controls.Add(this.datasetTextBox);
            this.mainPanel.Controls.Add(this.sqlLabel);
            this.mainPanel.Controls.Add(this.datasetLabel);
            this.mainPanel.Controls.Add(this.sqlTextBox);
            this.mainPanel.Controls.Add(this.month2TextBox);
            this.mainPanel.Controls.Add(this.updateDbRadioButton);
            this.mainPanel.Controls.Add(this.year2TextBox);
            this.mainPanel.Controls.Add(this.month1TextBox);
            this.mainPanel.Controls.Add(this.outputScriptRadioButton);
            this.mainPanel.Controls.Add(this.year1TextBox);
            this.mainPanel.Controls.Add(this.month2Label);
            this.mainPanel.Controls.Add(this.outputLabel);
            this.mainPanel.Controls.Add(this.year2Label);
            this.mainPanel.Controls.Add(this.logLabel);
            this.mainPanel.Controls.Add(this.month1Label);
            this.mainPanel.Controls.Add(this.dateRangeLabel);
            this.mainPanel.Controls.Add(this.year1Label);
            this.mainPanel.Location = new System.Drawing.Point(12, 45);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(750, 427);
            this.mainPanel.TabIndex = 2;
            // 
            // dbTextBox
            // 
            this.dbTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbTextBox.ForeColor = System.Drawing.Color.Blue;
            this.dbTextBox.Location = new System.Drawing.Point(203, 384);
            this.dbTextBox.Name = "dbTextBox";
            this.dbTextBox.Size = new System.Drawing.Size(469, 22);
            this.dbTextBox.TabIndex = 25;
            this.dbTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.dbTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // dbLabel
            // 
            this.dbLabel.AutoSize = true;
            this.dbLabel.Location = new System.Drawing.Point(32, 387);
            this.dbLabel.Name = "dbLabel";
            this.dbLabel.Size = new System.Drawing.Size(163, 14);
            this.dbLabel.TabIndex = 30;
            this.dbLabel.Text = "Database Connection String:";
            // 
            // createConnectionStringButton
            // 
            this.createConnectionStringButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createConnectionStringButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.createConnectionStringButton.Image = ((System.Drawing.Image)(resources.GetObject("createConnectionStringButton.Image")));
            this.createConnectionStringButton.Location = new System.Drawing.Point(678, 384);
            this.createConnectionStringButton.Name = "createConnectionStringButton";
            this.createConnectionStringButton.Size = new System.Drawing.Size(25, 25);
            this.createConnectionStringButton.TabIndex = 29;
            this.createConnectionStringButton.TabStop = false;
            this.toolTip1.SetToolTip(this.createConnectionStringButton, "Create the MSSQL Connection String");
            this.createConnectionStringButton.Click += new System.EventHandler(this.createConnectionStringButton_Click);
            // 
            // databaseNamePictureBox
            // 
            this.databaseNamePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseNamePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.databaseNamePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("databaseNamePictureBox.Image")));
            this.databaseNamePictureBox.Location = new System.Drawing.Point(706, 386);
            this.databaseNamePictureBox.Name = "databaseNamePictureBox";
            this.databaseNamePictureBox.Size = new System.Drawing.Size(19, 19);
            this.databaseNamePictureBox.TabIndex = 28;
            this.databaseNamePictureBox.TabStop = false;
            this.databaseNamePictureBox.Click += new System.EventHandler(this.databaseNamePictureBox_Click);
            // 
            // logBrowseButton2
            // 
            this.logBrowseButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logBrowseButton2.BackColor = System.Drawing.Color.Transparent;
            this.logBrowseButton2.Button_Enabled = true;
            this.logBrowseButton2.Button_Text = "BROWSE";
            this.logBrowseButton2.Button_Type = SobekCM_Stats_Reader.Round_Button.Button_Type_Enum.Standard;
            this.logBrowseButton2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logBrowseButton2.Location = new System.Drawing.Point(636, 111);
            this.logBrowseButton2.Name = "logBrowseButton2";
            this.logBrowseButton2.Size = new System.Drawing.Size(94, 26);
            this.logBrowseButton2.TabIndex = 6;
            this.logBrowseButton2.Button_Pressed += new System.EventHandler(this.logBrowseButton2_Button_Pressed);
            // 
            // datasetBrowseButton2
            // 
            this.datasetBrowseButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.datasetBrowseButton2.BackColor = System.Drawing.Color.Transparent;
            this.datasetBrowseButton2.Button_Enabled = true;
            this.datasetBrowseButton2.Button_Text = "BROWSE";
            this.datasetBrowseButton2.Button_Type = SobekCM_Stats_Reader.Round_Button.Button_Type_Enum.Standard;
            this.datasetBrowseButton2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.datasetBrowseButton2.Location = new System.Drawing.Point(636, 157);
            this.datasetBrowseButton2.Name = "datasetBrowseButton2";
            this.datasetBrowseButton2.Size = new System.Drawing.Size(94, 26);
            this.datasetBrowseButton2.TabIndex = 9;
            this.datasetBrowseButton2.Button_Pressed += new System.EventHandler(this.datasetBrowseButton2_Button_Pressed);
            // 
            // sqlBrowseButton2
            // 
            this.sqlBrowseButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlBrowseButton2.BackColor = System.Drawing.Color.Transparent;
            this.sqlBrowseButton2.Button_Enabled = true;
            this.sqlBrowseButton2.Button_Text = "BROWSE";
            this.sqlBrowseButton2.Button_Type = SobekCM_Stats_Reader.Round_Button.Button_Type_Enum.Standard;
            this.sqlBrowseButton2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlBrowseButton2.Location = new System.Drawing.Point(636, 243);
            this.sqlBrowseButton2.Name = "sqlBrowseButton2";
            this.sqlBrowseButton2.Size = new System.Drawing.Size(94, 26);
            this.sqlBrowseButton2.TabIndex = 15;
            this.sqlBrowseButton2.Button_Pressed += new System.EventHandler(this.sqlBrowseButton2_Button_Pressed);
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.instructionsLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.instructionsLabel.Location = new System.Drawing.Point(32, 9);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Size = new System.Drawing.Size(682, 47);
            this.instructionsLabel.TabIndex = 0;
            this.instructionsLabel.Text = "Enter localization information below to begin reading the IIS web logs and genera" +
    "ting SQL scripts to save the usage in your SobekCM database.";
            // 
            // mainLabel
            // 
            this.mainLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainLabel.BackColor = System.Drawing.Color.Transparent;
            this.mainLabel.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel.ForeColor = System.Drawing.Color.MediumBlue;
            this.mainLabel.Location = new System.Drawing.Point(7, -1);
            this.mainLabel.Name = "mainLabel";
            this.mainLabel.Size = new System.Drawing.Size(769, 43);
            this.mainLabel.TabIndex = 0;
            this.mainLabel.Text = "SobekCM Usage Statistics Reader";
            this.mainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // okRoundButton
            // 
            this.okRoundButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okRoundButton.BackColor = System.Drawing.Color.Transparent;
            this.okRoundButton.Button_Enabled = true;
            this.okRoundButton.Button_Text = "OK";
            this.okRoundButton.Button_Type = SobekCM_Stats_Reader.Round_Button.Button_Type_Enum.Forward;
            this.okRoundButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okRoundButton.Location = new System.Drawing.Point(668, 478);
            this.okRoundButton.Name = "okRoundButton";
            this.okRoundButton.Size = new System.Drawing.Size(94, 26);
            this.okRoundButton.TabIndex = 4;
            this.okRoundButton.Button_Pressed += new System.EventHandler(this.okRoundButton_Button_Pressed);
            // 
            // cancelRoundButton
            // 
            this.cancelRoundButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelRoundButton.BackColor = System.Drawing.Color.Transparent;
            this.cancelRoundButton.Button_Enabled = true;
            this.cancelRoundButton.Button_Text = "CANCEL";
            this.cancelRoundButton.Button_Type = SobekCM_Stats_Reader.Round_Button.Button_Type_Enum.Backward;
            this.cancelRoundButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelRoundButton.Location = new System.Drawing.Point(553, 478);
            this.cancelRoundButton.Name = "cancelRoundButton";
            this.cancelRoundButton.Size = new System.Drawing.Size(94, 26);
            this.cancelRoundButton.TabIndex = 3;
            this.cancelRoundButton.Button_Pressed += new System.EventHandler(this.cancelRoundButton_Button_Pressed);
            // 
            // helpPictureBox
            // 
            this.helpPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.helpPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.helpPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("helpPictureBox.Image")));
            this.helpPictureBox.Location = new System.Drawing.Point(718, 12);
            this.helpPictureBox.Name = "helpPictureBox";
            this.helpPictureBox.Size = new System.Drawing.Size(25, 26);
            this.helpPictureBox.TabIndex = 24;
            this.helpPictureBox.TabStop = false;
            this.helpPictureBox.Click += new System.EventHandler(this.helpPictureBox_Click);
            // 
            // resultPanel
            // 
            this.resultPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.resultPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultPanel.Controls.Add(this.richTextBox1);
            this.resultPanel.Location = new System.Drawing.Point(6, 41);
            this.resultPanel.Name = "resultPanel";
            this.resultPanel.Size = new System.Drawing.Size(756, 427);
            this.resultPanel.TabIndex = 25;
            this.resultPanel.Visible = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(10, 13);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(726, 397);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // sobekBrowseButton
            // 
            this.sobekBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sobekBrowseButton.BackColor = System.Drawing.Color.Transparent;
            this.sobekBrowseButton.Button_Enabled = true;
            this.sobekBrowseButton.Button_Text = "BROWSE";
            this.sobekBrowseButton.Button_Type = SobekCM_Stats_Reader.Round_Button.Button_Type_Enum.Standard;
            this.sobekBrowseButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sobekBrowseButton.Location = new System.Drawing.Point(636, 64);
            this.sobekBrowseButton.Name = "sobekBrowseButton";
            this.sobekBrowseButton.Size = new System.Drawing.Size(94, 26);
            this.sobekBrowseButton.TabIndex = 3;
            this.sobekBrowseButton.Button_Pressed += new System.EventHandler(this.sobekBrowseButton_Button_Pressed);
            // 
            // sobekTextBox
            // 
            this.sobekTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sobekTextBox.ForeColor = System.Drawing.Color.Blue;
            this.sobekTextBox.Location = new System.Drawing.Point(203, 68);
            this.sobekTextBox.Name = "sobekTextBox";
            this.sobekTextBox.Size = new System.Drawing.Size(415, 22);
            this.sobekTextBox.TabIndex = 2;
            this.sobekTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.sobekTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // sobekLabel
            // 
            this.sobekLabel.AutoSize = true;
            this.sobekLabel.Location = new System.Drawing.Point(32, 71);
            this.sobekLabel.Name = "sobekLabel";
            this.sobekLabel.Size = new System.Drawing.Size(170, 14);
            this.sobekLabel.TabIndex = 1;
            this.sobekLabel.Text = "SobekCM Web App Directory:";
            // 
            // Stats_Setup_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(775, 515);
            this.Controls.Add(this.helpPictureBox);
            this.Controls.Add(this.okRoundButton);
            this.Controls.Add(this.cancelRoundButton);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.mainLabel);
            this.Controls.Add(this.resultPanel);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Stats_Setup_Form";
            this.Text = "SobekCM Usage Statistics Reader";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.createConnectionStringButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseNamePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).EndInit();
            this.resultPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.RadioButton outputScriptRadioButton;
        private System.Windows.Forms.RadioButton updateDbRadioButton;
        private System.Windows.Forms.Label sqlLabel;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.TextBox sqlTextBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox month2TextBox;
        private System.Windows.Forms.TextBox year2TextBox;
        private System.Windows.Forms.TextBox month1TextBox;
        private System.Windows.Forms.TextBox year1TextBox;
        private System.Windows.Forms.Label month2Label;
        private System.Windows.Forms.Label year2Label;
        private System.Windows.Forms.Label month1Label;
        private System.Windows.Forms.Label year1Label;
        private System.Windows.Forms.Label dateRangeLabel;
        private System.Windows.Forms.TextBox datasetTextBox;
        private System.Windows.Forms.Label datasetLabel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.Label instructionsLabel;
        private Round_Button cancelRoundButton;
        private Round_Button okRoundButton;
        private Round_Button sqlBrowseButton2;
        private Round_Button datasetBrowseButton2;
        private Round_Button logBrowseButton2;
        private System.Windows.Forms.PictureBox helpPictureBox;
        private System.Windows.Forms.TextBox dbTextBox;
        private System.Windows.Forms.Label dbLabel;
        private System.Windows.Forms.PictureBox createConnectionStringButton;
        private System.Windows.Forms.PictureBox databaseNamePictureBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel resultPanel;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private Round_Button sobekBrowseButton;
        private System.Windows.Forms.TextBox sobekTextBox;
        private System.Windows.Forms.Label sobekLabel;
    }
}