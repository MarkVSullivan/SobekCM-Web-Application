namespace SobekCM.Management_Tool.Importer
{
    partial class Constant_Assignment_Control
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboMappedField = new System.Windows.Forms.ComboBox();
            this.cboMappedConstant = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cboMappedField
            // 
            this.cboMappedField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMappedField.FormattingEnabled = true;
            this.cboMappedField.Location = new System.Drawing.Point(14, 3);
            this.cboMappedField.Name = "cboMappedField";
            this.cboMappedField.Size = new System.Drawing.Size(159, 21);
            this.cboMappedField.Sorted = true;
            this.cboMappedField.TabIndex = 1;
            this.cboMappedField.SelectedIndexChanged += new System.EventHandler(this.cboMappedField_SelectedIndexChanged);
            // 
            // cboMappedConstant
            // 
            this.cboMappedConstant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cboMappedConstant.FormattingEnabled = true;
            this.cboMappedConstant.Location = new System.Drawing.Point(189, 3);
            this.cboMappedConstant.Name = "cboMappedConstant";
            this.cboMappedConstant.Size = new System.Drawing.Size(240, 21);
            this.cboMappedConstant.Sorted = true;
            this.cboMappedConstant.TabIndex = 2;
            // 
            // Constant_Assignment_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.cboMappedConstant);
            this.Controls.Add(this.cboMappedField);
            this.Name = "Constant_Assignment_Control";
            this.Size = new System.Drawing.Size(464, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboMappedField;
        private System.Windows.Forms.ComboBox cboMappedConstant;
    }
}
