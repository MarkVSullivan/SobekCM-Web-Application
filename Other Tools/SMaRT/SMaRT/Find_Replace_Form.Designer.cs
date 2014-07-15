namespace SobekCM.Management_Tool
{
    partial class Find_Replace_Form
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.findTextBox = new System.Windows.Forms.TextBox();
            this.replaceTextBox = new System.Windows.Forms.TextBox();
            this.allRowsRadioButton = new System.Windows.Forms.RadioButton();
            this.checkedRowsRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new SobekCM.Management_Tool.Round_Button();
            this.round_Button1 = new SobekCM.Management_Tool.Round_Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find what:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Replace with:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Scope:";
            // 
            // findTextBox
            // 
            this.findTextBox.Location = new System.Drawing.Point(102, 11);
            this.findTextBox.Name = "findTextBox";
            this.findTextBox.Size = new System.Drawing.Size(163, 22);
            this.findTextBox.TabIndex = 3;
            // 
            // replaceTextBox
            // 
            this.replaceTextBox.Location = new System.Drawing.Point(102, 41);
            this.replaceTextBox.Name = "replaceTextBox";
            this.replaceTextBox.Size = new System.Drawing.Size(163, 22);
            this.replaceTextBox.TabIndex = 4;
            // 
            // allRowsRadioButton
            // 
            this.allRowsRadioButton.AutoSize = true;
            this.allRowsRadioButton.Checked = true;
            this.allRowsRadioButton.Location = new System.Drawing.Point(90, 76);
            this.allRowsRadioButton.Name = "allRowsRadioButton";
            this.allRowsRadioButton.Size = new System.Drawing.Size(67, 18);
            this.allRowsRadioButton.TabIndex = 5;
            this.allRowsRadioButton.TabStop = true;
            this.allRowsRadioButton.Text = "All rows";
            this.allRowsRadioButton.UseVisualStyleBackColor = true;
            // 
            // checkedRowsRadioButton
            // 
            this.checkedRowsRadioButton.AutoSize = true;
            this.checkedRowsRadioButton.Location = new System.Drawing.Point(163, 76);
            this.checkedRowsRadioButton.Name = "checkedRowsRadioButton";
            this.checkedRowsRadioButton.Size = new System.Drawing.Size(105, 18);
            this.checkedRowsRadioButton.TabIndex = 6;
            this.checkedRowsRadioButton.Text = "Checked Rows";
            this.checkedRowsRadioButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.BackColor = System.Drawing.Color.Transparent;
            this.okButton.Button_Enabled = true;
            this.okButton.Button_Text = "REPLACE ALL";
            this.okButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Forward;
            this.okButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(151, 109);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(137, 26);
            this.okButton.TabIndex = 10;
            this.okButton.Button_Pressed += new System.EventHandler(this.replaceButton_Click);
            // 
            // round_Button1
            // 
            this.round_Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.round_Button1.BackColor = System.Drawing.Color.Transparent;
            this.round_Button1.Button_Enabled = true;
            this.round_Button1.Button_Text = "CANCEL";
            this.round_Button1.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Backward;
            this.round_Button1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.round_Button1.Location = new System.Drawing.Point(40, 109);
            this.round_Button1.Name = "round_Button1";
            this.round_Button1.Size = new System.Drawing.Size(94, 26);
            this.round_Button1.TabIndex = 9;
            this.round_Button1.Button_Pressed += new System.EventHandler(this.cancelButton_Click);
            // 
            // Find_Replace_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 147);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.round_Button1);
            this.Controls.Add(this.checkedRowsRadioButton);
            this.Controls.Add(this.allRowsRadioButton);
            this.Controls.Add(this.replaceTextBox);
            this.Controls.Add(this.findTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Find_Replace_Form";
            this.Text = "Find and Replace";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox findTextBox;
        private System.Windows.Forms.TextBox replaceTextBox;
        private System.Windows.Forms.RadioButton allRowsRadioButton;
        private System.Windows.Forms.RadioButton checkedRowsRadioButton;
        private Round_Button okButton;
        private Round_Button round_Button1;
    }
}