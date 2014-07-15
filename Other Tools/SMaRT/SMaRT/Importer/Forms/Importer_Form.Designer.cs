namespace SobekCM.Management_Tool.Importer.Forms
{
    partial class Importer_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Importer_Form));
            this.panel1 = new System.Windows.Forms.Panel();
            this.autoFillLinkLabel = new System.Windows.Forms.LinkLabel();
            this.marcLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.spreadsheetLinkLabel = new System.Windows.Forms.LinkLabel();
            this.exitButton = new SobekCM.Management_Tool.Round_Button();
            this.mainLabel = new System.Windows.Forms.Label();
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
            this.panel1.Controls.Add(this.autoFillLinkLabel);
            this.panel1.Controls.Add(this.marcLinkLabel);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.spreadsheetLinkLabel);
            this.panel1.Location = new System.Drawing.Point(21, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(406, 254);
            this.panel1.TabIndex = 3;
            // 
            // autoFillLinkLabel
            // 
            this.autoFillLinkLabel.AutoSize = true;
            this.autoFillLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoFillLinkLabel.Location = new System.Drawing.Point(107, 178);
            this.autoFillLinkLabel.Name = "autoFillLinkLabel";
            this.autoFillLinkLabel.Size = new System.Drawing.Size(189, 18);
            this.autoFillLinkLabel.TabIndex = 6;
            this.autoFillLinkLabel.TabStop = true;
            this.autoFillLinkLabel.Text = "Volume Import and Auto-Fill";
            this.autoFillLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.autoFillLinkLabel_LinkClicked);
            // 
            // marcLinkLabel
            // 
            this.marcLinkLabel.AutoSize = true;
            this.marcLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.marcLinkLabel.Location = new System.Drawing.Point(107, 86);
            this.marcLinkLabel.Name = "marcLinkLabel";
            this.marcLinkLabel.Size = new System.Drawing.Size(147, 18);
            this.marcLinkLabel.TabIndex = 0;
            this.marcLinkLabel.TabStop = true;
            this.marcLinkLabel.Text = "MARC File of Records";
            this.marcLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.marcLinkLabel_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(52, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(278, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "Select a format to import from below:";
            // 
            // spreadsheetLinkLabel
            // 
            this.spreadsheetLinkLabel.AutoSize = true;
            this.spreadsheetLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spreadsheetLinkLabel.Location = new System.Drawing.Point(107, 132);
            this.spreadsheetLinkLabel.Name = "spreadsheetLinkLabel";
            this.spreadsheetLinkLabel.Size = new System.Drawing.Size(164, 18);
            this.spreadsheetLinkLabel.TabIndex = 3;
            this.spreadsheetLinkLabel.TabStop = true;
            this.spreadsheetLinkLabel.Text = "Spreadsheet of Records";
            this.spreadsheetLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.spreadsheetLinkLabel_LinkClicked);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.BackColor = System.Drawing.Color.Transparent;
            this.exitButton.Button_Enabled = true;
            this.exitButton.Button_Text = "CLOSE";
            this.exitButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Standard;
            this.exitButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(333, 308);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(94, 26);
            this.exitButton.TabIndex = 4;
            this.exitButton.Button_Pressed += new System.EventHandler(this.exitButton_Button_Pressed);
            // 
            // mainLabel
            // 
            this.mainLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mainLabel.BackColor = System.Drawing.Color.Transparent;
            this.mainLabel.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel.ForeColor = System.Drawing.Color.MediumBlue;
            this.mainLabel.Location = new System.Drawing.Point(16, 13);
            this.mainLabel.Name = "mainLabel";
            this.mainLabel.Size = new System.Drawing.Size(411, 32);
            this.mainLabel.TabIndex = 5;
            this.mainLabel.Text = "Import Records";
            this.mainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Importer_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 346);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.mainLabel);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Importer_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importing Module";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel marcLinkLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel spreadsheetLinkLabel;
        private Round_Button exitButton;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.LinkLabel autoFillLinkLabel;
    }
}