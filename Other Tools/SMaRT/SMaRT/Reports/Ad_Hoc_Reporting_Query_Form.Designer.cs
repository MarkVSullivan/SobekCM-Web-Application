namespace SobekCM.Management_Tool
{
    partial class Ad_Hoc_Reporting_Query_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ad_Hoc_Reporting_Query_Form));
            this.panel1 = new System.Windows.Forms.Panel();
            this.sobekCM_Item_Discovery_Panel1 = new SobekCM.Management_Tool.Controls.SobekCM_Item_Discovery_Panel();
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
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.sobekCM_Item_Discovery_Panel1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(797, 172);
            this.panel1.TabIndex = 26;
            // 
            // sobekCM_Item_Discovery_Panel1
            // 
            this.sobekCM_Item_Discovery_Panel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sobekCM_Item_Discovery_Panel1.Location = new System.Drawing.Point(15, 3);
            this.sobekCM_Item_Discovery_Panel1.Name = "sobekCM_Item_Discovery_Panel1";
            this.sobekCM_Item_Discovery_Panel1.Search_Precision = SobekCM.Library.Navigation.Search_Precision_Type_Enum.Inflectional_Form;
            this.sobekCM_Item_Discovery_Panel1.Size = new System.Drawing.Size(760, 145);
            this.sobekCM_Item_Discovery_Panel1.TabIndex = 0;
            this.sobekCM_Item_Discovery_Panel1.Search_Requested += new SobekCM.Management_Tool.Controls.SobekCM_Item_Discovery_Panel.Search_Requested_Delegate(this.sobekCM_Item_Discovery_Panel1_Search_Requested);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.BackColor = System.Drawing.Color.Transparent;
            this.okButton.Button_Enabled = true;
            this.okButton.Button_Text = "OK";
            this.okButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Forward;
            this.okButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(715, 190);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(94, 26);
            this.okButton.TabIndex = 25;
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
            this.exitButton.Location = new System.Drawing.Point(606, 190);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(94, 26);
            this.exitButton.TabIndex = 24;
            this.exitButton.Button_Pressed += new System.EventHandler(this.exitButton_Button_Pressed);
            // 
            // Ad_Hoc_Reporting_Query_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(821, 228);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.exitButton);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Ad_Hoc_Reporting_Query_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ad Hoc Reporting Query Form";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Round_Button okButton;
        private Round_Button exitButton;
        private System.Windows.Forms.Panel panel1;
        private SobekCM.Management_Tool.Controls.SobekCM_Item_Discovery_Panel sobekCM_Item_Discovery_Panel1;
    }
}