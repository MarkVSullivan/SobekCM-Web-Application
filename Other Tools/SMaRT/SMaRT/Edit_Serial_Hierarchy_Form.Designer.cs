namespace SobekCM.Management_Tool
{
    partial class Edit_Serial_Hierarchy_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Edit_Serial_Hierarchy_Form));
            this.headerPanel = new System.Windows.Forms.Panel();
            this.level3RenumberLabel = new System.Windows.Forms.LinkLabel();
            this.level2RenumberLabel = new System.Windows.Forms.LinkLabel();
            this.level3ReplaceLabel = new System.Windows.Forms.LinkLabel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.level2ReplaceLabel = new System.Windows.Forms.LinkLabel();
            this.level1RenumberLabel = new System.Windows.Forms.LinkLabel();
            this.level1ReplaceLabel = new System.Windows.Forms.LinkLabel();
            this.level3Label = new System.Windows.Forms.Label();
            this.level2Label = new System.Windows.Forms.Label();
            this.level1Label = new System.Windows.Forms.Label();
            this.vidLabel = new System.Windows.Forms.Label();
            this.serialPanel = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.saveButton = new SobekCM.Management_Tool.Round_Button();
            this.reorderButton = new SobekCM.Management_Tool.Round_Button();
            this.cancelButton = new SobekCM.Management_Tool.Round_Button();
            this.nextButton = new SobekCM.Management_Tool.Round_Button();
            this.prevButton = new SobekCM.Management_Tool.Round_Button();
            this.autoButton = new SobekCM.Management_Tool.Round_Button();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.MidnightBlue;
            this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headerPanel.Controls.Add(this.level3RenumberLabel);
            this.headerPanel.Controls.Add(this.level2RenumberLabel);
            this.headerPanel.Controls.Add(this.level3ReplaceLabel);
            this.headerPanel.Controls.Add(this.checkBox1);
            this.headerPanel.Controls.Add(this.level2ReplaceLabel);
            this.headerPanel.Controls.Add(this.level1RenumberLabel);
            this.headerPanel.Controls.Add(this.level1ReplaceLabel);
            this.headerPanel.Controls.Add(this.level3Label);
            this.headerPanel.Controls.Add(this.level2Label);
            this.headerPanel.Controls.Add(this.level1Label);
            this.headerPanel.Controls.Add(this.vidLabel);
            this.headerPanel.Location = new System.Drawing.Point(13, 12);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(679, 30);
            this.headerPanel.TabIndex = 9;
            // 
            // level3RenumberLabel
            // 
            this.level3RenumberLabel.AutoSize = true;
            this.level3RenumberLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level3RenumberLabel.ForeColor = System.Drawing.Color.White;
            this.level3RenumberLabel.LinkColor = System.Drawing.Color.White;
            this.level3RenumberLabel.Location = new System.Drawing.Point(603, 6);
            this.level3RenumberLabel.Name = "level3RenumberLabel";
            this.level3RenumberLabel.Size = new System.Drawing.Size(28, 13);
            this.level3RenumberLabel.TabIndex = 10;
            this.level3RenumberLabel.TabStop = true;
            this.level3RenumberLabel.Text = "x10";
            this.toolTip1.SetToolTip(this.level3RenumberLabel, "Multiply all sort indexes by 10");
            this.level3RenumberLabel.VisitedLinkColor = System.Drawing.Color.White;
            this.level3RenumberLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.level3RenumberLabel_LinkClicked);
            // 
            // level2RenumberLabel
            // 
            this.level2RenumberLabel.AutoSize = true;
            this.level2RenumberLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level2RenumberLabel.ForeColor = System.Drawing.Color.White;
            this.level2RenumberLabel.LinkColor = System.Drawing.Color.White;
            this.level2RenumberLabel.Location = new System.Drawing.Point(408, 6);
            this.level2RenumberLabel.Name = "level2RenumberLabel";
            this.level2RenumberLabel.Size = new System.Drawing.Size(28, 13);
            this.level2RenumberLabel.TabIndex = 7;
            this.level2RenumberLabel.TabStop = true;
            this.level2RenumberLabel.Text = "x10";
            this.toolTip1.SetToolTip(this.level2RenumberLabel, "Multiply all sort indexes by 10");
            this.level2RenumberLabel.VisitedLinkColor = System.Drawing.Color.White;
            this.level2RenumberLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.level2RenumberLabel_LinkClicked);
            // 
            // level3ReplaceLabel
            // 
            this.level3ReplaceLabel.AutoSize = true;
            this.level3ReplaceLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level3ReplaceLabel.ForeColor = System.Drawing.Color.White;
            this.level3ReplaceLabel.LinkColor = System.Drawing.Color.White;
            this.level3ReplaceLabel.Location = new System.Drawing.Point(553, 6);
            this.level3ReplaceLabel.Name = "level3ReplaceLabel";
            this.level3ReplaceLabel.Size = new System.Drawing.Size(34, 13);
            this.level3ReplaceLabel.TabIndex = 9;
            this.level3ReplaceLabel.TabStop = true;
            this.level3ReplaceLabel.Text = "A    B";
            this.toolTip1.SetToolTip(this.level3ReplaceLabel, "Find and replace on the level 3 text");
            this.level3ReplaceLabel.VisitedLinkColor = System.Drawing.Color.White;
            this.level3ReplaceLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.level3ReplaceLabel_LinkClicked);
            this.level3ReplaceLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.linkLabel1_Paint);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(3, 7);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // level2ReplaceLabel
            // 
            this.level2ReplaceLabel.AutoSize = true;
            this.level2ReplaceLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level2ReplaceLabel.ForeColor = System.Drawing.Color.White;
            this.level2ReplaceLabel.LinkColor = System.Drawing.Color.White;
            this.level2ReplaceLabel.Location = new System.Drawing.Point(358, 6);
            this.level2ReplaceLabel.Name = "level2ReplaceLabel";
            this.level2ReplaceLabel.Size = new System.Drawing.Size(34, 13);
            this.level2ReplaceLabel.TabIndex = 6;
            this.level2ReplaceLabel.TabStop = true;
            this.level2ReplaceLabel.Text = "A    B";
            this.toolTip1.SetToolTip(this.level2ReplaceLabel, "Find and replace on the level 2 text");
            this.level2ReplaceLabel.VisitedLinkColor = System.Drawing.Color.White;
            this.level2ReplaceLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.level2ReplaceLabel_LinkClicked);
            this.level2ReplaceLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.linkLabel1_Paint);
            // 
            // level1RenumberLabel
            // 
            this.level1RenumberLabel.AutoSize = true;
            this.level1RenumberLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level1RenumberLabel.ForeColor = System.Drawing.Color.White;
            this.level1RenumberLabel.LinkColor = System.Drawing.Color.White;
            this.level1RenumberLabel.Location = new System.Drawing.Point(213, 6);
            this.level1RenumberLabel.Name = "level1RenumberLabel";
            this.level1RenumberLabel.Size = new System.Drawing.Size(28, 13);
            this.level1RenumberLabel.TabIndex = 3;
            this.level1RenumberLabel.TabStop = true;
            this.level1RenumberLabel.Text = "x10";
            this.toolTip1.SetToolTip(this.level1RenumberLabel, "Multiply all sort indexes by 10");
            this.level1RenumberLabel.VisitedLinkColor = System.Drawing.Color.White;
            this.level1RenumberLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.level1RenumberLabel_LinkClicked);
            // 
            // level1ReplaceLabel
            // 
            this.level1ReplaceLabel.AutoSize = true;
            this.level1ReplaceLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level1ReplaceLabel.ForeColor = System.Drawing.Color.White;
            this.level1ReplaceLabel.LinkColor = System.Drawing.Color.White;
            this.level1ReplaceLabel.Location = new System.Drawing.Point(163, 6);
            this.level1ReplaceLabel.Name = "level1ReplaceLabel";
            this.level1ReplaceLabel.Size = new System.Drawing.Size(34, 13);
            this.level1ReplaceLabel.TabIndex = 2;
            this.level1ReplaceLabel.TabStop = true;
            this.level1ReplaceLabel.Text = "A    B";
            this.toolTip1.SetToolTip(this.level1ReplaceLabel, "Find and replace on the level 1 text");
            this.level1ReplaceLabel.VisitedLinkColor = System.Drawing.Color.White;
            this.level1ReplaceLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.level1ReplaceLabel_LinkClicked);
            this.level1ReplaceLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.linkLabel1_Paint);
            // 
            // level3Label
            // 
            this.level3Label.AutoSize = true;
            this.level3Label.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level3Label.ForeColor = System.Drawing.Color.White;
            this.level3Label.Location = new System.Drawing.Point(476, 6);
            this.level3Label.Name = "level3Label";
            this.level3Label.Size = new System.Drawing.Size(55, 14);
            this.level3Label.TabIndex = 8;
            this.level3Label.Text = "LEVEL 3";
            // 
            // level2Label
            // 
            this.level2Label.AutoSize = true;
            this.level2Label.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level2Label.ForeColor = System.Drawing.Color.White;
            this.level2Label.Location = new System.Drawing.Point(281, 6);
            this.level2Label.Name = "level2Label";
            this.level2Label.Size = new System.Drawing.Size(55, 14);
            this.level2Label.TabIndex = 5;
            this.level2Label.Text = "LEVEL 2";
            // 
            // level1Label
            // 
            this.level1Label.AutoSize = true;
            this.level1Label.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.level1Label.ForeColor = System.Drawing.Color.White;
            this.level1Label.Location = new System.Drawing.Point(86, 6);
            this.level1Label.Name = "level1Label";
            this.level1Label.Size = new System.Drawing.Size(55, 14);
            this.level1Label.TabIndex = 1;
            this.level1Label.Text = "LEVEL 1";
            // 
            // vidLabel
            // 
            this.vidLabel.AutoSize = true;
            this.vidLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vidLabel.ForeColor = System.Drawing.Color.White;
            this.vidLabel.Location = new System.Drawing.Point(25, 6);
            this.vidLabel.Name = "vidLabel";
            this.vidLabel.Size = new System.Drawing.Size(29, 14);
            this.vidLabel.TabIndex = 0;
            this.vidLabel.Text = "VID";
            // 
            // serialPanel
            // 
            this.serialPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serialPanel.AutoScroll = true;
            this.serialPanel.BackColor = System.Drawing.Color.White;
            this.serialPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serialPanel.Location = new System.Drawing.Point(13, 40);
            this.serialPanel.Name = "serialPanel";
            this.serialPanel.Size = new System.Drawing.Size(679, 376);
            this.serialPanel.TabIndex = 1;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.BackColor = System.Drawing.Color.Transparent;
            this.saveButton.Button_Enabled = true;
            this.saveButton.Button_Text = "SAVE";
            this.saveButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Forward;
            this.saveButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(595, 422);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(94, 26);
            this.saveButton.TabIndex = 2;
            this.saveButton.Button_Pressed += new System.EventHandler(this.saveButton_Button_Pressed);
            // 
            // reorderButton
            // 
            this.reorderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.reorderButton.BackColor = System.Drawing.Color.Transparent;
            this.reorderButton.Button_Enabled = true;
            this.reorderButton.Button_Text = "REORDER";
            this.reorderButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Standard;
            this.reorderButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reorderButton.Location = new System.Drawing.Point(246, 422);
            this.reorderButton.Name = "reorderButton";
            this.reorderButton.Size = new System.Drawing.Size(94, 26);
            this.reorderButton.TabIndex = 4;
            this.reorderButton.Button_Pressed += new System.EventHandler(this.reorderButton_Button_Pressed);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.cancelButton.Button_Enabled = true;
            this.cancelButton.Button_Text = "CANCEL";
            this.cancelButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Backward;
            this.cancelButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(483, 422);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(94, 26);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Button_Pressed += new System.EventHandler(this.cancelButton_Button_Pressed);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.BackColor = System.Drawing.Color.Transparent;
            this.nextButton.Button_Enabled = true;
            this.nextButton.Button_Text = "NEXT";
            this.nextButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Forward;
            this.nextButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextButton.Location = new System.Drawing.Point(129, 422);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(94, 26);
            this.nextButton.TabIndex = 3;
            this.nextButton.Button_Pressed += new System.EventHandler(this.nextButton_Button_Pressed);
            // 
            // prevButton
            // 
            this.prevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.prevButton.BackColor = System.Drawing.Color.Transparent;
            this.prevButton.Button_Enabled = true;
            this.prevButton.Button_Text = "PREV";
            this.prevButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Backward;
            this.prevButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prevButton.Location = new System.Drawing.Point(17, 422);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(94, 26);
            this.prevButton.TabIndex = 2;
            this.prevButton.Button_Pressed += new System.EventHandler(this.prevButton_Button_Pressed);
            // 
            // autoButton
            // 
            this.autoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.autoButton.BackColor = System.Drawing.Color.Transparent;
            this.autoButton.Button_Enabled = true;
            this.autoButton.Button_Text = "AUTO";
            this.autoButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Standard;
            this.autoButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoButton.Location = new System.Drawing.Point(363, 422);
            this.autoButton.Name = "autoButton";
            this.autoButton.Size = new System.Drawing.Size(94, 26);
            this.autoButton.TabIndex = 10;
            this.autoButton.Button_Pressed += new System.EventHandler(this.autoButton_Button_Pressed);
            // 
            // Edit_Serial_Hierarchy_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 460);
            this.Controls.Add(this.autoButton);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.serialPanel);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.reorderButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.prevButton);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(720, 800);
            this.MinimumSize = new System.Drawing.Size(720, 384);
            this.Name = "Edit_Serial_Hierarchy_Form";
            this.Text = "Edit_Item_Group_Form";
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Round_Button cancelButton;
        private Round_Button saveButton;
        private System.Windows.Forms.Panel serialPanel;
        private Round_Button reorderButton;
        private Round_Button nextButton;
        private Round_Button prevButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label level3Label;
        private System.Windows.Forms.Label level2Label;
        private System.Windows.Forms.Label level1Label;
        private System.Windows.Forms.Label vidLabel;
        private System.Windows.Forms.LinkLabel level1RenumberLabel;
        private System.Windows.Forms.LinkLabel level1ReplaceLabel;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.LinkLabel level3RenumberLabel;
        private System.Windows.Forms.LinkLabel level2RenumberLabel;
        private System.Windows.Forms.LinkLabel level3ReplaceLabel;
        private System.Windows.Forms.LinkLabel level2ReplaceLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private Round_Button autoButton;
    }
}