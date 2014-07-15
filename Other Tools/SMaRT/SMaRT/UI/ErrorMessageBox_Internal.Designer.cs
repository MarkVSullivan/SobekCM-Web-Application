namespace DLC.Tools.Forms
{
    partial class ErrorMessageBox_Internal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorMessageBox_Internal));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mainLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.detailsLabel = new System.Windows.Forms.LinkLabel();
            this.clipboardLabel = new System.Windows.Forms.LinkLabel();
            this.exceptionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(41, 40);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // mainLabel
            // 
            this.mainLabel.AutoSize = true;
            this.mainLabel.Location = new System.Drawing.Point(60, 12);
            this.mainLabel.MaximumSize = new System.Drawing.Size(600, 600);
            this.mainLabel.Name = "mainLabel";
            this.mainLabel.Size = new System.Drawing.Size(55, 13);
            this.mainLabel.TabIndex = 1;
            this.mainLabel.Text = "mainLabel";
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(104, 50);
            this.okButton.Name = "okButton";
            this.okButton.Padding = new System.Windows.Forms.Padding(1);
            this.okButton.Size = new System.Drawing.Size(74, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // detailsLabel
            // 
            this.detailsLabel.AutoSize = true;
            this.detailsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailsLabel.Location = new System.Drawing.Point(200, 64);
            this.detailsLabel.Name = "detailsLabel";
            this.detailsLabel.Size = new System.Drawing.Size(58, 12);
            this.detailsLabel.TabIndex = 3;
            this.detailsLabel.TabStop = true;
            this.detailsLabel.Text = "View Details";
            this.detailsLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.detailsLabel_LinkClicked);
            // 
            // clipboardLabel
            // 
            this.clipboardLabel.AutoSize = true;
            this.clipboardLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clipboardLabel.Location = new System.Drawing.Point(137, 9);
            this.clipboardLabel.Name = "clipboardLabel";
            this.clipboardLabel.Size = new System.Drawing.Size(121, 12);
            this.clipboardLabel.TabIndex = 4;
            this.clipboardLabel.TabStop = true;
            this.clipboardLabel.Text = "Copy Exception to Clipboard";
            this.clipboardLabel.Visible = false;
            this.clipboardLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.clipboardLabel_LinkClicked);
            // 
            // exceptionLabel
            // 
            this.exceptionLabel.AutoSize = true;
            this.exceptionLabel.Location = new System.Drawing.Point(12, 61);
            this.exceptionLabel.MaximumSize = new System.Drawing.Size(700, 600);
            this.exceptionLabel.Name = "exceptionLabel";
            this.exceptionLabel.Size = new System.Drawing.Size(35, 13);
            this.exceptionLabel.TabIndex = 5;
            this.exceptionLabel.Text = "label1";
            this.exceptionLabel.Visible = false;
            // 
            // ErrorMessageBox_Internal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 83);
            this.Controls.Add(this.exceptionLabel);
            this.Controls.Add(this.clipboardLabel);
            this.Controls.Add(this.detailsLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.mainLabel);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorMessageBox_Internal";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ErrorMessageBox";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel detailsLabel;
        private System.Windows.Forms.LinkLabel clipboardLabel;
        private System.Windows.Forms.Label exceptionLabel;
    }
}