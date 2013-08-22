namespace SobekCM.Configuration
{
    partial class SobekCM_Builder_Folders_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SobekCM_Builder_Folders_Form));
            this.closeButton = new SobekCM.Configuration.Round_Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mainLabel1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.mainLabel2 = new System.Windows.Forms.Label();
            this.newButton = new SobekCM.Configuration.Round_Button();
            this.deleteButton = new SobekCM.Configuration.Round_Button();
            this.editButton = new SobekCM.Configuration.Round_Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Button_Enabled = true;
            this.closeButton.Button_Text = "CLOSE";
            this.closeButton.Button_Type = SobekCM.Configuration.Round_Button.Button_Type_Enum.Backward;
            this.closeButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(457, 372);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(101, 32);
            this.closeButton.TabIndex = 10;
            this.closeButton.Button_Pressed += new System.EventHandler(this.closeButton_Button_Pressed);
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
            this.titleLabel.Size = new System.Drawing.Size(560, 42);
            this.titleLabel.TabIndex = 6;
            this.titleLabel.Text = "Builder/Bulk Loader Incoming Folders";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.editButton);
            this.panel1.Controls.Add(this.deleteButton);
            this.panel1.Controls.Add(this.newButton);
            this.panel1.Controls.Add(this.mainLabel2);
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Controls.Add(this.mainLabel1);
            this.panel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(9, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 311);
            this.panel1.TabIndex = 7;
            // 
            // mainLabel1
            // 
            this.mainLabel1.AutoSize = true;
            this.mainLabel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.mainLabel1.Location = new System.Drawing.Point(27, 25);
            this.mainLabel1.Name = "mainLabel1";
            this.mainLabel1.Size = new System.Drawing.Size(491, 14);
            this.mainLabel1.TabIndex = 0;
            this.mainLabel1.Text = "Select a folder to EDIT or DELETE below, or use the NEW button to create new";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(15, 80);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(527, 177);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 115;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Network";
            this.columnHeader2.Width = 396;
            // 
            // mainLabel2
            // 
            this.mainLabel2.AutoSize = true;
            this.mainLabel2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.mainLabel2.Location = new System.Drawing.Point(27, 50);
            this.mainLabel2.Name = "mainLabel2";
            this.mainLabel2.Size = new System.Drawing.Size(182, 14);
            this.mainLabel2.TabIndex = 2;
            this.mainLabel2.Text = "incoming folder information:";
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newButton.Button_Enabled = true;
            this.newButton.Button_Text = "NEW";
            this.newButton.Button_Type = SobekCM.Configuration.Round_Button.Button_Type_Enum.Standard;
            this.newButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newButton.Location = new System.Drawing.Point(80, 267);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(101, 32);
            this.newButton.TabIndex = 11;
            this.newButton.Button_Pressed += new System.EventHandler(this.newButton_Button_Pressed);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteButton.Button_Enabled = false;
            this.deleteButton.Button_Text = "DELETE";
            this.deleteButton.Button_Type = SobekCM.Configuration.Round_Button.Button_Type_Enum.Standard;
            this.deleteButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteButton.Location = new System.Drawing.Point(230, 267);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(101, 32);
            this.deleteButton.TabIndex = 12;
            this.deleteButton.Button_Pressed += new System.EventHandler(this.deleteButton_Button_Pressed);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.editButton.Button_Enabled = false;
            this.editButton.Button_Text = "EDIT";
            this.editButton.Button_Type = SobekCM.Configuration.Round_Button.Button_Type_Enum.Standard;
            this.editButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editButton.Location = new System.Drawing.Point(380, 267);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(101, 32);
            this.editButton.TabIndex = 13;
            this.editButton.Button_Pressed += new System.EventHandler(this.editButton_Button_Pressed);
            // 
            // SobekCM_Builder_Folders_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 413);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SobekCM_Builder_Folders_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SobekCM Builder/Bulk Loader Folders Form";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Round_Button closeButton;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label mainLabel1;
        private System.Windows.Forms.Label mainLabel2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private Round_Button editButton;
        private Round_Button deleteButton;
        private Round_Button newButton;
    }
}