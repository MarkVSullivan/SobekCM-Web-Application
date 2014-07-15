namespace SobekCM.Management_Tool.Importer.Forms
{
    partial class Matching_Record_Dialog_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Matching_Record_Dialog_Form));
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.defaultRadioButton = new System.Windows.Forms.RadioButton();
            this.alwaysUseOptionCheckBox = new System.Windows.Forms.CheckBox();
            this.skipRecordRadioButton = new System.Windows.Forms.RadioButton();
            this.createNewRecordRadioButton = new System.Windows.Forms.RadioButton();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.messageLabel = new System.Windows.Forms.Label();
            this.overlayRadioButton = new System.Windows.Forms.RadioButton();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsGroupBox.Controls.Add(this.overlayRadioButton);
            this.optionsGroupBox.Controls.Add(this.defaultRadioButton);
            this.optionsGroupBox.Controls.Add(this.skipRecordRadioButton);
            this.optionsGroupBox.Controls.Add(this.createNewRecordRadioButton);
            this.optionsGroupBox.Location = new System.Drawing.Point(14, 98);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(530, 146);
            this.optionsGroupBox.TabIndex = 0;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Matching Record Options";
            // 
            // defaultRadioButton
            // 
            this.defaultRadioButton.AutoSize = true;
            this.defaultRadioButton.Checked = true;
            this.defaultRadioButton.Location = new System.Drawing.Point(342, 122);
            this.defaultRadioButton.Name = "defaultRadioButton";
            this.defaultRadioButton.Size = new System.Drawing.Size(188, 18);
            this.defaultRadioButton.TabIndex = 4;
            this.defaultRadioButton.TabStop = true;
            this.defaultRadioButton.Text = "defaultRadioButton-not visible";
            this.defaultRadioButton.UseVisualStyleBackColor = true;
            this.defaultRadioButton.Visible = false;
            // 
            // alwaysUseOptionCheckBox
            // 
            this.alwaysUseOptionCheckBox.AutoSize = true;
            this.alwaysUseOptionCheckBox.Location = new System.Drawing.Point(32, 257);
            this.alwaysUseOptionCheckBox.Name = "alwaysUseOptionCheckBox";
            this.alwaysUseOptionCheckBox.Size = new System.Drawing.Size(277, 18);
            this.alwaysUseOptionCheckBox.TabIndex = 6;
            this.alwaysUseOptionCheckBox.Text = "Use this option for every occurence in this file";
            this.alwaysUseOptionCheckBox.UseVisualStyleBackColor = true;
            // 
            // skipRecordRadioButton
            // 
            this.skipRecordRadioButton.AutoSize = true;
            this.skipRecordRadioButton.Location = new System.Drawing.Point(42, 70);
            this.skipRecordRadioButton.Name = "skipRecordRadioButton";
            this.skipRecordRadioButton.Size = new System.Drawing.Size(428, 18);
            this.skipRecordRadioButton.TabIndex = 5;
            this.skipRecordRadioButton.Text = "Skip this record and continue processing the next record in the input file.";
            this.skipRecordRadioButton.UseVisualStyleBackColor = true;
            // 
            // createNewRecordRadioButton
            // 
            this.createNewRecordRadioButton.AutoSize = true;
            this.createNewRecordRadioButton.Location = new System.Drawing.Point(42, 36);
            this.createNewRecordRadioButton.Name = "createNewRecordRadioButton";
            this.createNewRecordRadioButton.Size = new System.Drawing.Size(309, 18);
            this.createNewRecordRadioButton.TabIndex = 4;
            this.createNewRecordRadioButton.Text = "Create a new record from the data in the input file.";
            this.createNewRecordRadioButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(351, 250);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(87, 25);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "OK";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(457, 250);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 25);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select destination folder for this METS file.";
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel.Location = new System.Drawing.Point(10, 21);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(0, 14);
            this.messageLabel.TabIndex = 3;
            // 
            // overlayRadioButton
            // 
            this.overlayRadioButton.AutoSize = true;
            this.overlayRadioButton.Location = new System.Drawing.Point(42, 104);
            this.overlayRadioButton.Name = "overlayRadioButton";
            this.overlayRadioButton.Size = new System.Drawing.Size(277, 18);
            this.overlayRadioButton.TabIndex = 7;
            this.overlayRadioButton.Text = "Overlay all the metadata for the existing item.";
            this.overlayRadioButton.UseVisualStyleBackColor = true;
            // 
            // Matching_Record_Dialog_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(558, 287);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.alwaysUseOptionCheckBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.optionsGroupBox);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Matching_Record_Dialog_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Matching Record Confirmation";
            this.TopMost = true;
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.RadioButton createNewRecordRadioButton;
        private System.Windows.Forms.RadioButton skipRecordRadioButton;
        private System.Windows.Forms.CheckBox alwaysUseOptionCheckBox;
        private System.Windows.Forms.RadioButton defaultRadioButton;
        private System.Windows.Forms.RadioButton overlayRadioButton;
    }
}       