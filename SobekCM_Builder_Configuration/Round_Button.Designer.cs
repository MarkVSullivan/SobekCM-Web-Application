namespace SobekCM.Configuration
{
    partial class Round_Button
    {

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonLabel
            // 
            this.buttonLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLabel.Location = new System.Drawing.Point(0, 0);
            this.buttonLabel.Name = "buttonLabel";
            this.buttonLabel.Size = new System.Drawing.Size(120, 32);
            this.buttonLabel.TabIndex = 0;
            this.buttonLabel.Text = "Button";
            this.buttonLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonLabel.MouseLeave += new System.EventHandler(this.Round_Button_MouseLeave);
            this.buttonLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Round_Button_MouseDown);
            this.buttonLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Round_Button_MouseUp);
            this.buttonLabel.MouseEnter += new System.EventHandler(this.Round_Button_MouseEnter);
            // 
            // Round_Button
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonLabel);
            this.Name = "Round_Button";
            this.Size = new System.Drawing.Size(120, 32);
            this.MouseLeave += new System.EventHandler(this.Round_Button_MouseLeave);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Round_Button_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Round_Button_MouseUp);
            this.MouseEnter += new System.EventHandler(this.Round_Button_MouseEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label buttonLabel;
    }
}
