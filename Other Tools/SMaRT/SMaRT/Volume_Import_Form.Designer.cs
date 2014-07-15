namespace SobekCM.Management_Tool
{
    partial class Volume_Import_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Volume_Import_Form));
            this.fileLabel = new System.Windows.Forms.Label();
            this.fileTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.sheetLabel = new System.Windows.Forms.Label();
            this.sheetComboBox = new System.Windows.Forms.ComboBox();
            this.btnShowData = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.titleGroupBox = new System.Windows.Forms.GroupBox();
            this.visibilityComboBox = new System.Windows.Forms.ComboBox();
            this.visibilityStaticLabel = new System.Windows.Forms.Label();
            this.bibidLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.viewOnlineButton = new System.Windows.Forms.Button();
            this.titleStaticLabel = new System.Windows.Forms.Label();
            this.vidComboBox = new System.Windows.Forms.ComboBox();
            this.importFromVidLabel = new System.Windows.Forms.Label();
            this.bibIdTextBox = new System.Windows.Forms.TextBox();
            this.bibidStaticLabel = new System.Windows.Forms.Label();
            this.fileGroupBox = new System.Windows.Forms.GroupBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.executeButton = new SobekCM.Management_Tool.Round_Button();
            this.cancelButton = new SobekCM.Management_Tool.Round_Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1.SuspendLayout();
            this.titleGroupBox.SuspendLayout();
            this.fileGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileLabel
            // 
            this.fileLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileLabel.Location = new System.Drawing.Point(14, 30);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(32, 23);
            this.fileLabel.TabIndex = 0;
            this.fileLabel.Text = "File:";
            this.fileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileTextBox
            // 
            this.fileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTextBox.BackColor = System.Drawing.Color.White;
            this.fileTextBox.Location = new System.Drawing.Point(73, 30);
            this.fileTextBox.Name = "fileTextBox";
            this.fileTextBox.ReadOnly = true;
            this.fileTextBox.Size = new System.Drawing.Size(389, 22);
            this.fileTextBox.TabIndex = 1;
            this.fileTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fileTextBox_MouseDown);
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(468, 28);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 24);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "SELECT";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // sheetLabel
            // 
            this.sheetLabel.BackColor = System.Drawing.Color.Transparent;
            this.sheetLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheetLabel.Location = new System.Drawing.Point(14, 66);
            this.sheetLabel.Name = "sheetLabel";
            this.sheetLabel.Size = new System.Drawing.Size(53, 23);
            this.sheetLabel.TabIndex = 3;
            this.sheetLabel.Text = "Sheet:";
            this.sheetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.sheetLabel.Visible = false;
            // 
            // sheetComboBox
            // 
            this.sheetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sheetComboBox.Location = new System.Drawing.Point(73, 67);
            this.sheetComboBox.Name = "sheetComboBox";
            this.sheetComboBox.Size = new System.Drawing.Size(160, 22);
            this.sheetComboBox.TabIndex = 4;
            this.sheetComboBox.Visible = false;
            this.sheetComboBox.SelectedIndexChanged += new System.EventHandler(this.sheetComboBox_SelectedIndexChanged);
            // 
            // btnShowData
            // 
            this.btnShowData.Location = new System.Drawing.Point(239, 66);
            this.btnShowData.Name = "btnShowData";
            this.btnShowData.Size = new System.Drawing.Size(75, 23);
            this.btnShowData.TabIndex = 5;
            this.btnShowData.Text = "Show Data";
            this.btnShowData.UseVisualStyleBackColor = true;
            this.btnShowData.Visible = false;
            this.btnShowData.Click += new System.EventHandler(this.btnShowData_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.titleGroupBox);
            this.panel1.Controls.Add(this.fileGroupBox);
            this.panel1.Location = new System.Drawing.Point(13, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(588, 316);
            this.panel1.TabIndex = 35;
            // 
            // titleGroupBox
            // 
            this.titleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.titleGroupBox.Controls.Add(this.visibilityComboBox);
            this.titleGroupBox.Controls.Add(this.visibilityStaticLabel);
            this.titleGroupBox.Controls.Add(this.bibidLabel);
            this.titleGroupBox.Controls.Add(this.titleLabel);
            this.titleGroupBox.Controls.Add(this.viewOnlineButton);
            this.titleGroupBox.Controls.Add(this.titleStaticLabel);
            this.titleGroupBox.Controls.Add(this.vidComboBox);
            this.titleGroupBox.Controls.Add(this.importFromVidLabel);
            this.titleGroupBox.Controls.Add(this.bibIdTextBox);
            this.titleGroupBox.Controls.Add(this.bibidStaticLabel);
            this.titleGroupBox.Location = new System.Drawing.Point(13, 123);
            this.titleGroupBox.Name = "titleGroupBox";
            this.titleGroupBox.Size = new System.Drawing.Size(558, 184);
            this.titleGroupBox.TabIndex = 1;
            this.titleGroupBox.TabStop = false;
            this.titleGroupBox.Text = "Digital Resource Information";
            // 
            // visibilityComboBox
            // 
            this.visibilityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.visibilityComboBox.Items.AddRange(new object[] {
            "DARK",
            "PRIVATE",
            "IP RESTRICTED",
            "PUBLIC"});
            this.visibilityComboBox.Location = new System.Drawing.Point(76, 135);
            this.visibilityComboBox.Name = "visibilityComboBox";
            this.visibilityComboBox.Size = new System.Drawing.Size(157, 22);
            this.visibilityComboBox.TabIndex = 9;
            // 
            // visibilityStaticLabel
            // 
            this.visibilityStaticLabel.BackColor = System.Drawing.Color.Transparent;
            this.visibilityStaticLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.visibilityStaticLabel.Location = new System.Drawing.Point(14, 134);
            this.visibilityStaticLabel.Name = "visibilityStaticLabel";
            this.visibilityStaticLabel.Size = new System.Drawing.Size(53, 23);
            this.visibilityStaticLabel.TabIndex = 8;
            this.visibilityStaticLabel.Text = "Visibility:";
            this.visibilityStaticLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bibidLabel
            // 
            this.bibidLabel.BackColor = System.Drawing.Color.Transparent;
            this.bibidLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bibidLabel.Location = new System.Drawing.Point(73, 29);
            this.bibidLabel.Name = "bibidLabel";
            this.bibidLabel.Size = new System.Drawing.Size(98, 23);
            this.bibidLabel.TabIndex = 7;
            this.bibidLabel.Text = "n/a";
            this.bibidLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(70, 65);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(470, 23);
            this.titleLabel.TabIndex = 4;
            this.titleLabel.Text = "( enter a BibID above to view item group title )";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // viewOnlineButton
            // 
            this.viewOnlineButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewOnlineButton.Location = new System.Drawing.Point(217, 27);
            this.viewOnlineButton.Name = "viewOnlineButton";
            this.viewOnlineButton.Size = new System.Drawing.Size(115, 23);
            this.viewOnlineButton.TabIndex = 2;
            this.viewOnlineButton.Text = "View Online";
            this.viewOnlineButton.UseVisualStyleBackColor = true;
            this.viewOnlineButton.Visible = false;
            this.viewOnlineButton.Click += new System.EventHandler(this.viewOnlineButton_Click);
            // 
            // titleStaticLabel
            // 
            this.titleStaticLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleStaticLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleStaticLabel.Location = new System.Drawing.Point(14, 65);
            this.titleStaticLabel.Name = "titleStaticLabel";
            this.titleStaticLabel.Size = new System.Drawing.Size(53, 23);
            this.titleStaticLabel.TabIndex = 3;
            this.titleStaticLabel.Text = "Title:";
            this.titleStaticLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // vidComboBox
            // 
            this.vidComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.vidComboBox.Location = new System.Drawing.Point(322, 99);
            this.vidComboBox.Name = "vidComboBox";
            this.vidComboBox.Size = new System.Drawing.Size(103, 22);
            this.vidComboBox.Sorted = true;
            this.vidComboBox.TabIndex = 6;
            // 
            // importFromVidLabel
            // 
            this.importFromVidLabel.BackColor = System.Drawing.Color.Transparent;
            this.importFromVidLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importFromVidLabel.Location = new System.Drawing.Point(14, 99);
            this.importFromVidLabel.Name = "importFromVidLabel";
            this.importFromVidLabel.Size = new System.Drawing.Size(318, 23);
            this.importFromVidLabel.TabIndex = 5;
            this.importFromVidLabel.Text = "Import metadata and behaviors from existing volume:";
            this.importFromVidLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bibIdTextBox
            // 
            this.bibIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bibIdTextBox.BackColor = System.Drawing.Color.White;
            this.bibIdTextBox.Location = new System.Drawing.Point(73, 29);
            this.bibIdTextBox.Name = "bibIdTextBox";
            this.bibIdTextBox.Size = new System.Drawing.Size(126, 22);
            this.bibIdTextBox.TabIndex = 1;
            this.bibIdTextBox.TextChanged += new System.EventHandler(this.bibIdTextBox_TextChanged);
            this.bibIdTextBox.Leave += new System.EventHandler(this.bibIdTextBox_Leave);
            // 
            // bibidStaticLabel
            // 
            this.bibidStaticLabel.BackColor = System.Drawing.Color.Transparent;
            this.bibidStaticLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bibidStaticLabel.Location = new System.Drawing.Point(14, 29);
            this.bibidStaticLabel.Name = "bibidStaticLabel";
            this.bibidStaticLabel.Size = new System.Drawing.Size(53, 23);
            this.bibidStaticLabel.TabIndex = 0;
            this.bibidStaticLabel.Text = "BibID:";
            this.bibidStaticLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileGroupBox
            // 
            this.fileGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileGroupBox.Controls.Add(this.fileTextBox);
            this.fileGroupBox.Controls.Add(this.btnShowData);
            this.fileGroupBox.Controls.Add(this.browseButton);
            this.fileGroupBox.Controls.Add(this.sheetComboBox);
            this.fileGroupBox.Controls.Add(this.fileLabel);
            this.fileGroupBox.Controls.Add(this.sheetLabel);
            this.fileGroupBox.Location = new System.Drawing.Point(13, 12);
            this.fileGroupBox.Name = "fileGroupBox";
            this.fileGroupBox.Size = new System.Drawing.Size(558, 105);
            this.fileGroupBox.TabIndex = 0;
            this.fileGroupBox.TabStop = false;
            this.fileGroupBox.Text = "Source File Information";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Excel files|*.xls;*.xlsx|CSV files|*.csv|Tab-delimited Text Files|*.txt";
            // 
            // executeButton
            // 
            this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.executeButton.BackColor = System.Drawing.Color.Transparent;
            this.executeButton.Button_Enabled = false;
            this.executeButton.Button_Text = "EXECUTE";
            this.executeButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Forward;
            this.executeButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.executeButton.Location = new System.Drawing.Point(501, 334);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(100, 26);
            this.executeButton.TabIndex = 1;
            this.executeButton.Button_Pressed += new System.EventHandler(this.executeButton_Button_Pressed);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.cancelButton.Button_Enabled = true;
            this.cancelButton.Button_Text = "CANCEL";
            this.cancelButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Backward;
            this.cancelButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(386, 334);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 26);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Button_Pressed += new System.EventHandler(this.cancelButton_Button_Pressed);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(13, 372);
            this.progressBar1.Maximum = 4;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(588, 13);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 36;
            this.progressBar1.Visible = false;
            // 
            // Volume_Import_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 397);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.cancelButton);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Volume_Import_Form";
            this.Text = "Volume Import and Auto-Fill Form";
            this.panel1.ResumeLayout(false);
            this.titleGroupBox.ResumeLayout(false);
            this.titleGroupBox.PerformLayout();
            this.fileGroupBox.ResumeLayout(false);
            this.fileGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.TextBox fileTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label sheetLabel;
        private System.Windows.Forms.ComboBox sheetComboBox;
        private System.Windows.Forms.Button btnShowData;
        private Round_Button executeButton;
        private Round_Button cancelButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox fileGroupBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox titleGroupBox;
        private System.Windows.Forms.TextBox bibIdTextBox;
        private System.Windows.Forms.Label bibidStaticLabel;
        private System.Windows.Forms.Button viewOnlineButton;
        private System.Windows.Forms.Label titleStaticLabel;
        private System.Windows.Forms.ComboBox vidComboBox;
        private System.Windows.Forms.Label importFromVidLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label bibidLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox visibilityComboBox;
        private System.Windows.Forms.Label visibilityStaticLabel;
    }
}