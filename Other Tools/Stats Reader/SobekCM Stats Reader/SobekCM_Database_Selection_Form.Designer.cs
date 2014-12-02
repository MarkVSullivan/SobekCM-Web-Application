namespace SobekCM_Stats_Reader
{
    partial class SobekCM_Database_Selection_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SobekCM_Database_Selection_Form));
            this.saveButton = new Round_Button();
            this.cancelButton = new Round_Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.databaseNameTextBox = new System.Windows.Forms.TextBox();
            this.serverNameTextBox = new System.Windows.Forms.TextBox();
            this.testConnectionButton = new Round_Button();
            this.databaseNameLabel = new System.Windows.Forms.Label();
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.mainLabel2 = new System.Windows.Forms.Label();
            this.mainLabel1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Button_Enabled = false;
            this.saveButton.Button_Text = "CONTINUE";
            this.saveButton.Button_Type = Round_Button.Button_Type_Enum.Forward;
            this.saveButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(299, 229);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(120, 32);
            this.saveButton.TabIndex = 7;
            this.saveButton.Button_Pressed += new System.EventHandler(this.saveButton_Button_Pressed);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Button_Enabled = true;
            this.cancelButton.Button_Text = "CANCEL";
            this.cancelButton.Button_Type = Round_Button.Button_Type_Enum.Backward;
            this.cancelButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(158, 229);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(120, 32);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Button_Pressed += new System.EventHandler(this.cancelButton_Button_Pressed);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.mainLabel2);
            this.panel1.Controls.Add(this.mainLabel1);
            this.panel1.Controls.Add(this.databaseNameTextBox);
            this.panel1.Controls.Add(this.serverNameTextBox);
            this.panel1.Controls.Add(this.testConnectionButton);
            this.panel1.Controls.Add(this.databaseNameLabel);
            this.panel1.Controls.Add(this.serverNameLabel);
            this.panel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(9, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(425, 211);
            this.panel1.TabIndex = 5;
            // 
            // databaseNameTextBox
            // 
            this.databaseNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseNameTextBox.Location = new System.Drawing.Point(130, 116);
            this.databaseNameTextBox.Name = "databaseNameTextBox";
            this.databaseNameTextBox.Size = new System.Drawing.Size(249, 22);
            this.databaseNameTextBox.TabIndex = 10;
            this.databaseNameTextBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.databaseNameTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.databaseNameTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverNameTextBox.Location = new System.Drawing.Point(130, 86);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(249, 22);
            this.serverNameTextBox.TabIndex = 9;
            this.serverNameTextBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.serverNameTextBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.serverNameTextBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // testConnectionButton
            // 
            this.testConnectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.testConnectionButton.Button_Enabled = false;
            this.testConnectionButton.Button_Text = "TEST CONNECTION";
            this.testConnectionButton.Button_Type = Round_Button.Button_Type_Enum.Standard;
            this.testConnectionButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.testConnectionButton.Location = new System.Drawing.Point(148, 155);
            this.testConnectionButton.Name = "testConnectionButton";
            this.testConnectionButton.Size = new System.Drawing.Size(157, 32);
            this.testConnectionButton.TabIndex = 8;
            this.testConnectionButton.Button_Pressed += new System.EventHandler(this.testConnectionButton_Button_Pressed);
            // 
            // databaseNameLabel
            // 
            this.databaseNameLabel.AutoSize = true;
            this.databaseNameLabel.Location = new System.Drawing.Point(24, 119);
            this.databaseNameLabel.Name = "databaseNameLabel";
            this.databaseNameLabel.Size = new System.Drawing.Size(96, 14);
            this.databaseNameLabel.TabIndex = 3;
            this.databaseNameLabel.Text = "Database Name:";
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.Location = new System.Drawing.Point(24, 89);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(100, 14);
            this.serverNameLabel.TabIndex = 1;
            this.serverNameLabel.Text = "Database Server:";
            // 
            // mainLabel2
            // 
            this.mainLabel2.AutoSize = true;
            this.mainLabel2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.mainLabel2.Location = new System.Drawing.Point(27, 44);
            this.mainLabel2.Name = "mainLabel2";
            this.mainLabel2.Size = new System.Drawing.Size(223, 14);
            this.mainLabel2.TabIndex = 12;
            this.mainLabel2.Text = "to the MS SQL SobekCM Database.";
            // 
            // mainLabel1
            // 
            this.mainLabel1.AutoSize = true;
            this.mainLabel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.mainLabel1.Location = new System.Drawing.Point(27, 21);
            this.mainLabel1.Name = "mainLabel1";
            this.mainLabel1.Size = new System.Drawing.Size(336, 14);
            this.mainLabel1.TabIndex = 11;
            this.mainLabel1.Text = "Enter database information to build a connect string ";
            // 
            // SobekCM_Database_Selection_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 269);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SobekCM_Database_Selection_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Build MSSQL Database Connection String";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Round_Button saveButton;
        private Round_Button cancelButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label databaseNameLabel;
        private System.Windows.Forms.Label serverNameLabel;
        private Round_Button testConnectionButton;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.TextBox databaseNameTextBox;
        private System.Windows.Forms.Label mainLabel2;
        private System.Windows.Forms.Label mainLabel1;
    }
}