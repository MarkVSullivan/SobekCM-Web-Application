namespace SobekCM.Management_Tool
{
    partial class Ad_Hoc_Report_Display_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ad_Hoc_Report_Display_Form));
            this.panel1 = new System.Windows.Forms.Panel();
            this.exitButton = new SobekCM.Management_Tool.Round_Button();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelSpreadsheetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabDelimitedTextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commaSeperatedValuesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLDataSetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pageSetupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printAllRowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printSelectedRowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateBornDigitalFlagMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setTrackingBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateMaterialReceivedDateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDispositionAdviceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateItemDispositionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addWorklogHistoryEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.printTrackingSheetsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionOnClickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openItemFromWebMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewItemFormMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.openWebContextMenuItem = new System.Windows.Forms.MenuItem();
            this.detailsContextMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.updateBornDigitalContextMenuItem = new System.Windows.Forms.MenuItem();
            this.setTrackingBoxContextMenuItem = new System.Windows.Forms.MenuItem();
            this.updateMaterialReceivedContextMenuItem = new System.Windows.Forms.MenuItem();
            this.editDispositionAdviceContextMenuItem = new System.Windows.Forms.MenuItem();
            this.updateItemDispositionContextMenuItem = new System.Windows.Forms.MenuItem();
            this.addWorklogHistoryContextMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.printBibIdReportContextMenuItem = new System.Windows.Forms.MenuItem();
            this.printAllRowsContextMenuItem = new System.Windows.Forms.MenuItem();
            this.printedSelectedRowsContextMenuItem = new System.Windows.Forms.MenuItem();
            this.hitCountLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(12, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(666, 363);
            this.panel1.TabIndex = 27;
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.BackColor = System.Drawing.Color.Transparent;
            this.exitButton.Button_Enabled = true;
            this.exitButton.Button_Text = "CLOSE";
            this.exitButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Standard;
            this.exitButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(584, 408);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(94, 26);
            this.exitButton.TabIndex = 26;
            this.exitButton.Button_Pressed += new System.EventHandler(this.exitButton_Button_Pressed);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Document = this.printDocument1;
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // pageSetupDialog1
            // 
            this.pageSetupDialog1.Document = this.printDocument1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionsMenuItem,
            this.preferencesToolStripMenuItem,
            this.helpMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(690, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsMenuItem,
            this.toolStripSeparator1,
            this.pageSetupMenuItem,
            this.printPreviewMenuItem,
            this.printMenuItem,
            this.toolStripSeparator2,
            this.exitMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveAsMenuItem
            // 
            this.saveAsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelSpreadsheetMenuItem,
            this.tabDelimitedTextMenuItem,
            this.commaSeperatedValuesMenuItem,
            this.xMLDataSetMenuItem});
            this.saveAsMenuItem.Name = "saveAsMenuItem";
            this.saveAsMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsMenuItem.Text = "Save As..";
            // 
            // excelSpreadsheetMenuItem
            // 
            this.excelSpreadsheetMenuItem.Name = "excelSpreadsheetMenuItem";
            this.excelSpreadsheetMenuItem.Size = new System.Drawing.Size(274, 22);
            this.excelSpreadsheetMenuItem.Text = "Excel Spreadsheet ( *.xls, *.xlsx )";
            this.excelSpreadsheetMenuItem.Click += new System.EventHandler(this.excelSpreadsheetMenuItem_Click);
            // 
            // tabDelimitedTextMenuItem
            // 
            this.tabDelimitedTextMenuItem.Name = "tabDelimitedTextMenuItem";
            this.tabDelimitedTextMenuItem.Size = new System.Drawing.Size(274, 22);
            this.tabDelimitedTextMenuItem.Text = "Tab-Delimited Text File ( *.txt )";
            this.tabDelimitedTextMenuItem.Click += new System.EventHandler(this.tabDelimitedTextMenuItem_Click);
            // 
            // commaSeperatedValuesMenuItem
            // 
            this.commaSeperatedValuesMenuItem.Name = "commaSeperatedValuesMenuItem";
            this.commaSeperatedValuesMenuItem.Size = new System.Drawing.Size(274, 22);
            this.commaSeperatedValuesMenuItem.Text = "Comma-Seperated Values File ( *.csv )";
            this.commaSeperatedValuesMenuItem.Click += new System.EventHandler(this.commaSeperatedValuesMenuItem_Click);
            // 
            // xMLDataSetMenuItem
            // 
            this.xMLDataSetMenuItem.Name = "xMLDataSetMenuItem";
            this.xMLDataSetMenuItem.Size = new System.Drawing.Size(274, 22);
            this.xMLDataSetMenuItem.Text = "XML DataSet ( *.xml )";
            this.xMLDataSetMenuItem.Click += new System.EventHandler(this.xMLDataSetMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
            // 
            // pageSetupMenuItem
            // 
            this.pageSetupMenuItem.Name = "pageSetupMenuItem";
            this.pageSetupMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pageSetupMenuItem.Text = "Page Setup";
            this.pageSetupMenuItem.Click += new System.EventHandler(this.pageSetupMenuItem_Click);
            // 
            // printPreviewMenuItem
            // 
            this.printPreviewMenuItem.Name = "printPreviewMenuItem";
            this.printPreviewMenuItem.Size = new System.Drawing.Size(143, 22);
            this.printPreviewMenuItem.Text = "Print Preview";
            // 
            // printMenuItem
            // 
            this.printMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printAllRowsMenuItem,
            this.printSelectedRowsMenuItem});
            this.printMenuItem.Name = "printMenuItem";
            this.printMenuItem.Size = new System.Drawing.Size(152, 22);
            this.printMenuItem.Text = "Print";
            // 
            // printAllRowsMenuItem
            // 
            this.printAllRowsMenuItem.Name = "printAllRowsMenuItem";
            this.printAllRowsMenuItem.Size = new System.Drawing.Size(173, 22);
            this.printAllRowsMenuItem.Text = "Print all rows";
            this.printAllRowsMenuItem.Click += new System.EventHandler(this.printAllRowsContextMenuItem_Click);
            // 
            // printSelectedRowsMenuItem
            // 
            this.printSelectedRowsMenuItem.Name = "printSelectedRowsMenuItem";
            this.printSelectedRowsMenuItem.Size = new System.Drawing.Size(173, 22);
            this.printSelectedRowsMenuItem.Text = "Print selected rows";
            this.printSelectedRowsMenuItem.Click += new System.EventHandler(this.printSelectedRowsMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(140, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // actionsMenuItem
            // 
            this.actionsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateBornDigitalFlagMenuItem,
            this.setTrackingBoxToolStripMenuItem,
            this.updateMaterialReceivedDateMenuItem,
            this.editDispositionAdviceMenuItem,
            this.updateItemDispositionMenuItem,
            this.addWorklogHistoryEntryMenuItem,
            this.toolStripSeparator3,
            this.printTrackingSheetsMenuItem});
            this.actionsMenuItem.Name = "actionsMenuItem";
            this.actionsMenuItem.Size = new System.Drawing.Size(59, 20);
            this.actionsMenuItem.Text = "Actions";
            // 
            // updateBornDigitalFlagMenuItem
            // 
            this.updateBornDigitalFlagMenuItem.Name = "updateBornDigitalFlagMenuItem";
            this.updateBornDigitalFlagMenuItem.Size = new System.Drawing.Size(235, 22);
            this.updateBornDigitalFlagMenuItem.Text = "Update Born Digital Flag";
            this.updateBornDigitalFlagMenuItem.Click += new System.EventHandler(this.updateBornDigitalFlagMenuItem_Click);
            // 
            // setTrackingBoxToolStripMenuItem
            // 
            this.setTrackingBoxToolStripMenuItem.Name = "setTrackingBoxToolStripMenuItem";
            this.setTrackingBoxToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.setTrackingBoxToolStripMenuItem.Text = "Set Tracking Box";
            this.setTrackingBoxToolStripMenuItem.Click += new System.EventHandler(this.setTrackingBoxToolStripMenuItem_Click);
            // 
            // updateMaterialReceivedDateMenuItem
            // 
            this.updateMaterialReceivedDateMenuItem.Name = "updateMaterialReceivedDateMenuItem";
            this.updateMaterialReceivedDateMenuItem.Size = new System.Drawing.Size(235, 22);
            this.updateMaterialReceivedDateMenuItem.Text = "Update Material Received Date";
            this.updateMaterialReceivedDateMenuItem.Click += new System.EventHandler(this.updateMaterialReceivedDateMenuItem_Click);
            // 
            // editDispositionAdviceMenuItem
            // 
            this.editDispositionAdviceMenuItem.Name = "editDispositionAdviceMenuItem";
            this.editDispositionAdviceMenuItem.Size = new System.Drawing.Size(235, 22);
            this.editDispositionAdviceMenuItem.Text = "Edit Disposition Advice";
            this.editDispositionAdviceMenuItem.Click += new System.EventHandler(this.editDispositionAdviceMenuItem_Click);
            // 
            // updateItemDispositionMenuItem
            // 
            this.updateItemDispositionMenuItem.Name = "updateItemDispositionMenuItem";
            this.updateItemDispositionMenuItem.Size = new System.Drawing.Size(235, 22);
            this.updateItemDispositionMenuItem.Text = "Update Item Disposition";
            this.updateItemDispositionMenuItem.Click += new System.EventHandler(this.updateItemDispositionMenuItem_Click);
            // 
            // addWorklogHistoryEntryMenuItem
            // 
            this.addWorklogHistoryEntryMenuItem.Name = "addWorklogHistoryEntryMenuItem";
            this.addWorklogHistoryEntryMenuItem.Size = new System.Drawing.Size(235, 22);
            this.addWorklogHistoryEntryMenuItem.Text = "Add Worklog History Entry";
            this.addWorklogHistoryEntryMenuItem.Click += new System.EventHandler(this.addWorklogHistoryEntryMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(232, 6);
            // 
            // printTrackingSheetsMenuItem
            // 
            this.printTrackingSheetsMenuItem.Name = "printTrackingSheetsMenuItem";
            this.printTrackingSheetsMenuItem.Size = new System.Drawing.Size(235, 22);
            this.printTrackingSheetsMenuItem.Text = "Print Volume Report";
            this.printTrackingSheetsMenuItem.Click += new System.EventHandler(this.printTrackingSheetsMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionOnClickToolStripMenuItem});
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // actionOnClickToolStripMenuItem
            // 
            this.actionOnClickToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openItemFromWebMenuItem,
            this.viewItemFormMenuItem});
            this.actionOnClickToolStripMenuItem.Name = "actionOnClickToolStripMenuItem";
            this.actionOnClickToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.actionOnClickToolStripMenuItem.Text = "Action on double-click";
            // 
            // openItemFromWebMenuItem
            // 
            this.openItemFromWebMenuItem.Name = "openItemFromWebMenuItem";
            this.openItemFromWebMenuItem.Size = new System.Drawing.Size(287, 22);
            this.openItemFromWebMenuItem.Text = "Open item from web in browser window";
            this.openItemFromWebMenuItem.Click += new System.EventHandler(this.openItemFromWebInBrowserWindowToolStripMenuItem_Click);
            // 
            // viewItemFormMenuItem
            // 
            this.viewItemFormMenuItem.Checked = true;
            this.viewItemFormMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewItemFormMenuItem.Name = "viewItemFormMenuItem";
            this.viewItemFormMenuItem.Size = new System.Drawing.Size(287, 22);
            this.viewItemFormMenuItem.Text = "View item form";
            this.viewItemFormMenuItem.Click += new System.EventHandler(this.viewItemFormToolStripMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpMenuItem.Text = "Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.Document = this.printDocument1;
            this.printDialog1.UseEXDialog = true;
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.openWebContextMenuItem,
            this.detailsContextMenuItem,
            this.menuItem1,
            this.updateBornDigitalContextMenuItem,
            this.setTrackingBoxContextMenuItem,
            this.updateMaterialReceivedContextMenuItem,
            this.editDispositionAdviceContextMenuItem,
            this.updateItemDispositionContextMenuItem,
            this.addWorklogHistoryContextMenuItem,
            this.menuItem9,
            this.printBibIdReportContextMenuItem,
            this.printAllRowsContextMenuItem,
            this.printedSelectedRowsContextMenuItem});
            // 
            // openWebContextMenuItem
            // 
            this.openWebContextMenuItem.Index = 0;
            this.openWebContextMenuItem.Text = "Open Item in SobekCM";
            this.openWebContextMenuItem.Click += new System.EventHandler(this.openWebContextMenuItem_Click);
            // 
            // detailsContextMenuItem
            // 
            this.detailsContextMenuItem.Index = 1;
            this.detailsContextMenuItem.Text = "View Item Form";
            this.detailsContextMenuItem.Click += new System.EventHandler(this.detailsContextMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.Text = "-";
            // 
            // updateBornDigitalContextMenuItem
            // 
            this.updateBornDigitalContextMenuItem.Index = 3;
            this.updateBornDigitalContextMenuItem.Text = "Update Born Digital Flag";
            this.updateBornDigitalContextMenuItem.Click += new System.EventHandler(this.updateBornDigitalContextMenuItem_Click);
            // 
            // setTrackingBoxContextMenuItem
            // 
            this.setTrackingBoxContextMenuItem.Index = 4;
            this.setTrackingBoxContextMenuItem.Text = "Set Tracking Box";
            this.setTrackingBoxContextMenuItem.Click += new System.EventHandler(this.setTrackingBoxContextMenuItem_Click);
            // 
            // updateMaterialReceivedContextMenuItem
            // 
            this.updateMaterialReceivedContextMenuItem.Index = 5;
            this.updateMaterialReceivedContextMenuItem.Text = "Update Material Received Date";
            this.updateMaterialReceivedContextMenuItem.Click += new System.EventHandler(this.updateMaterialReceivedContextMenuItem_Click);
            // 
            // editDispositionAdviceContextMenuItem
            // 
            this.editDispositionAdviceContextMenuItem.Index = 6;
            this.editDispositionAdviceContextMenuItem.Text = "Edit Disposition Advice";
            this.editDispositionAdviceContextMenuItem.Click += new System.EventHandler(this.editDispositionAdviceContextMenuItem_Click);
            // 
            // updateItemDispositionContextMenuItem
            // 
            this.updateItemDispositionContextMenuItem.Index = 7;
            this.updateItemDispositionContextMenuItem.Text = "Update Item Disposition";
            this.updateItemDispositionContextMenuItem.Click += new System.EventHandler(this.updateDispositionContextMenuItem_Click);
            // 
            // addWorklogHistoryContextMenuItem
            // 
            this.addWorklogHistoryContextMenuItem.Index = 8;
            this.addWorklogHistoryContextMenuItem.Text = "Add Worklog History Entry";
            this.addWorklogHistoryContextMenuItem.Click += new System.EventHandler(this.addWorklogHistoryEntryContextMenuItem_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 9;
            this.menuItem9.Text = "-";
            // 
            // printBibIdReportContextMenuItem
            // 
            this.printBibIdReportContextMenuItem.Index = 10;
            this.printBibIdReportContextMenuItem.Text = "Print Tracking Sheet";
            this.printBibIdReportContextMenuItem.Click += new System.EventHandler(this.printBibIdReportContextMenuItem_Click);
            // 
            // printAllRowsContextMenuItem
            // 
            this.printAllRowsContextMenuItem.Index = 11;
            this.printAllRowsContextMenuItem.Text = "Print All Rows";
            this.printAllRowsContextMenuItem.Click += new System.EventHandler(this.printAllRowsContextMenuItem_Click);
            // 
            // printedSelectedRowsContextMenuItem
            // 
            this.printedSelectedRowsContextMenuItem.Index = 12;
            this.printedSelectedRowsContextMenuItem.Text = "Print Selected Rows";
            this.printedSelectedRowsContextMenuItem.Click += new System.EventHandler(this.printedSelectedRowsContextMenuItem_Click);
            // 
            // hitCountLabel
            // 
            this.hitCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hitCountLabel.AutoSize = true;
            this.hitCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.hitCountLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hitCountLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.hitCountLabel.Location = new System.Drawing.Point(28, 416);
            this.hitCountLabel.Name = "hitCountLabel";
            this.hitCountLabel.Size = new System.Drawing.Size(144, 18);
            this.hitCountLabel.TabIndex = 36;
            this.hitCountLabel.Text = "No Matches Found";
            // 
            // Ad_Hoc_Report_Display_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(690, 446);
            this.Controls.Add(this.hitCountLabel);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.exitButton);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Ad_Hoc_Report_Display_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ad Hoc Reporting Display Form";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Round_Button exitButton;
        private System.Windows.Forms.Panel panel1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excelSpreadsheetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tabDelimitedTextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commaSeperatedValuesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xMLDataSetMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem pageSetupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printPreviewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printAllRowsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printSelectedRowsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateBornDigitalFlagMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setTrackingBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateMaterialReceivedDateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editDispositionAdviceMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateItemDispositionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addWorklogHistoryEntryMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem printTrackingSheetsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionOnClickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openItemFromWebMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewItemFormMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem openWebContextMenuItem;
        private System.Windows.Forms.MenuItem detailsContextMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem updateBornDigitalContextMenuItem;
        private System.Windows.Forms.MenuItem setTrackingBoxContextMenuItem;
        private System.Windows.Forms.MenuItem updateMaterialReceivedContextMenuItem;
        private System.Windows.Forms.MenuItem editDispositionAdviceContextMenuItem;
        private System.Windows.Forms.MenuItem updateItemDispositionContextMenuItem;
        private System.Windows.Forms.MenuItem addWorklogHistoryContextMenuItem;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem printBibIdReportContextMenuItem;
        private System.Windows.Forms.MenuItem printAllRowsContextMenuItem;
        private System.Windows.Forms.MenuItem printedSelectedRowsContextMenuItem;
        private System.Windows.Forms.Label hitCountLabel;
    }
}