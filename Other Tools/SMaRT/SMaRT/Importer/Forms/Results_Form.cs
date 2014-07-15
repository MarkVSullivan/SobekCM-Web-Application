using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using DLC.Custom_Grid;
using SobekCM.Resource_Object;


namespace SobekCM.Management_Tool.Importer.Forms
{
	/// <summary> Form displays a simple <see cref="DataGrid"/> and allows the user to print
	/// reports based on that data. <br /> <br /> </summary>
	/// <remarks> Written by Mark Sullivan (2005) </remarks>
	public class Results_Form : System.Windows.Forms.Form
	{
		#region Form-Related Private Class Members

        private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem setupMenuItem;
		private System.Windows.Forms.MenuItem previewMenuItem;
		private System.Windows.Forms.MenuItem printMenuItem;
		private System.Windows.Forms.MenuItem saveMenuItem;
		private System.Windows.Forms.MenuItem exitMenuItem;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
		private System.Windows.Forms.PrintDialog printDialog1;
		private System.Drawing.Printing.PrintDocument printDocument1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Button printButton;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.MenuItem actionMenuItem;
		private System.Windows.Forms.MenuItem helpMenuItem;
		private System.Windows.Forms.MenuItem aboutMenuItem;
		private System.Windows.Forms.MenuItem detailsMenuItem;
        private System.Windows.Forms.MenuItem detailsMenuItem_ContextMenuItem;
        private System.Windows.Forms.MenuItem commentsMenuItem_ContextMenuItem;

        private System.Windows.Forms.MenuItem actionMI_ShowDetails;        

		#endregion

		private CustomGrid_Panel customGrid_Panel1;
        private CustomGrid_Printer gridPrinter;

		private ArrayList resultList;
		private DataTable resultTable;
        private string importerName;

		/// <summary> Constructor for a new instance of this class </summary>
		/// <param name="resultSet"> Results set to display </param>
        //public Results_Form(DataTable resultSet, ArrayList resultList, SobekCM_Item bibPackage)
        public Results_Form(DataTable resultSet, Importer_Type_Enum importerType, bool preview_mode )            
		{
			// Initialize this form
			InitializeComponent();

            // Perform some additional work if this was not XP theme
            if (!Windows_Appearance_Checker.is_XP_Theme)
            {
                saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                printButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;               
            }

			// Save these values
				this.resultTable = resultSet;

			// Show the result set
			customGrid_Panel1.DataTable = resultSet;

			// Set some overall styles
			customGrid_Panel1.Style.Default_Column_Color = System.Drawing.Color.LightBlue;
			customGrid_Panel1.Style.Header_Back_Color = Color.DarkBlue;
			customGrid_Panel1.Style.Header_Fore_Color = Color.White;
			customGrid_Panel1.Style.Default_Column_Color = System.Drawing.Color.LightBlue;
         
            // assign the context menu to the custom grid
            this.customGrid_Panel1.Double_Clicked += new CustomGrid_Panel_Delegate_Single(this.customGrid_Panel1_Double_Clicked);
            this.customGrid_Panel1.ContextMenu = this.contextMenu1;
            
            
            // determine what should be displayed on the results form
            switch (importerType)
            {
                case Importer_Type_Enum.MARC:
                    // set column styles for displaying the results table                                  
                    customGrid_Panel1.Style.Column_Styles[0].Visible = false;
                    customGrid_Panel1.Style.Column_Styles[1].Header_Text = "Bib ID";
                    customGrid_Panel1.Style.Column_Styles[1].Fixed_Print_Width = 100;
                    customGrid_Panel1.Style.Column_Styles[1].Width = 100;
                    customGrid_Panel1.Style.Column_Styles[2].Header_Text = "VID";
                    customGrid_Panel1.Style.Column_Styles[2].Width = 70;
                    customGrid_Panel1.Style.Column_Styles[2].Fixed_Print_Width = 65;
                    customGrid_Panel1.Style.Column_Styles[3].Header_Text = "Comment";
                    customGrid_Panel1.Style.Column_Styles[3].Width = 200;
                    customGrid_Panel1.Style.Column_Styles[4].Visible = false;
                    customGrid_Panel1.Style.Column_Styles[5].Header_Text = "Aleph";
                    customGrid_Panel1.Style.Column_Styles[5].Width = 70;
                    customGrid_Panel1.Style.Column_Styles[6].Header_Text = "OCLC";
                    customGrid_Panel1.Style.Column_Styles[6].Width = 70;
                    customGrid_Panel1.Style.Column_Styles[7].Header_Text = "Bib Title";
                    customGrid_Panel1.Style.Column_Styles[7].Width = 300;
                    customGrid_Panel1.Style.Column_Styles[7].BackColor = Color.White;
                    customGrid_Panel1.Style.Column_Styles[8].Header_Text = "Volume Title";
                    customGrid_Panel1.Style.Column_Styles[8].Width = 300;
                    customGrid_Panel1.Style.Column_Styles[8].BackColor = Color.White;
                    customGrid_Panel1.Style.Column_Styles[9].Header_Text = "Author";
                    customGrid_Panel1.Style.Column_Styles[9].Width = 150;
                    customGrid_Panel1.Style.Column_Styles[9].BackColor = Color.White;
                    customGrid_Panel1.Style.Column_Styles[10].Header_Text = "Type";
                    customGrid_Panel1.Style.Column_Styles[10].Width = 70;
                    customGrid_Panel1.Style.Column_Styles[11].Header_Text = "Project";
                    customGrid_Panel1.Style.Column_Styles[11].Width = 70;

                    // set text on menu item                
                    this.detailsMenuItem.Text = "Show MARC XML";
                    this.detailsMenuItem_ContextMenuItem.Text = "Show MARC XML";
                    this.Text = "MARC Importer - Results Form";
                    break;

                case Importer_Type_Enum.METS:
                case Importer_Type_Enum.Spreadsheet:
                    // set column styles for displaying the results table                                  
                    customGrid_Panel1.Style.Column_Styles[0].Visible = false;
                    customGrid_Panel1.Style.Column_Styles[1].Header_Text = "Bib ID";
                    customGrid_Panel1.Style.Column_Styles[1].Width = 100;
                    customGrid_Panel1.Style.Column_Styles[1].Fixed_Print_Width = 100;
                    customGrid_Panel1.Style.Column_Styles[2].Header_Text = "VID";
                    customGrid_Panel1.Style.Column_Styles[2].Width = 70;
                    customGrid_Panel1.Style.Column_Styles[2].Fixed_Print_Width = 65;
                    customGrid_Panel1.Style.Column_Styles[3].Header_Text = "Comment";
                    customGrid_Panel1.Style.Column_Styles[3].Width = 200;
                    customGrid_Panel1.Style.Column_Styles[4].Visible = false;
                    customGrid_Panel1.Style.Column_Styles[5].Header_Text = "Aleph";
                    customGrid_Panel1.Style.Column_Styles[5].Width = 70;
                    customGrid_Panel1.Style.Column_Styles[6].Header_Text = "OCLC";
                    customGrid_Panel1.Style.Column_Styles[6].Width = 70;
                    customGrid_Panel1.Style.Column_Styles[7].Header_Text = "Bib Title";
                    customGrid_Panel1.Style.Column_Styles[7].Width = 300;
                    customGrid_Panel1.Style.Column_Styles[7].BackColor = Color.White;
                    customGrid_Panel1.Style.Column_Styles[8].Header_Text = "Volume Title";
                    customGrid_Panel1.Style.Column_Styles[8].Width = 300;
                    customGrid_Panel1.Style.Column_Styles[8].BackColor = Color.White;
                    customGrid_Panel1.Style.Column_Styles[9].Header_Text = "Author";
                    customGrid_Panel1.Style.Column_Styles[9].Width = 150;
                    customGrid_Panel1.Style.Column_Styles[9].BackColor = Color.White;
                    customGrid_Panel1.Style.Column_Styles[10].Header_Text = "Type";
                    customGrid_Panel1.Style.Column_Styles[10].Width = 70;
                    customGrid_Panel1.Style.Column_Styles[11].Header_Text = "Project";
                    customGrid_Panel1.Style.Column_Styles[11].Width = 70;

                    // set text on menu item
                    this.detailsMenuItem.Text = "Show METS";
                    this.detailsMenuItem_ContextMenuItem.Text = "Show METS";
                    if ( importerType == Importer_Type_Enum.METS )
                        this.Text = "METS Importer - Results Form";
                    else
                        this.Text = "SpreadSheet Importer - Results Form";
                    break;
            }

            if (preview_mode)
            {
                this.Text = this.Text + " (PREVIEW ONLY!)";
            }
                       
			// Set the default print as landscape
			printDocument1.DefaultPageSettings.Landscape = true;

			// Create the object to print the grid
			gridPrinter = new CustomGrid_Printer( customGrid_Panel1, printDocument1 );
		}

		#region Windows Form Designer generated code

		/// <summary> Clean up any resources being used.  </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Results_Form));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.setupMenuItem = new System.Windows.Forms.MenuItem();
            this.previewMenuItem = new System.Windows.Forms.MenuItem();
            this.printMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.saveMenuItem = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.actionMenuItem = new System.Windows.Forms.MenuItem();
            this.detailsMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.detailsMenuItem_ContextMenuItem = new System.Windows.Forms.MenuItem();
            this.commentsMenuItem_ContextMenuItem = new System.Windows.Forms.MenuItem();
            this.actionMI_ShowDetails = new System.Windows.Forms.MenuItem();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.customGrid_Panel1 = new CustomGrid_Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.printButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.actionMenuItem,
            this.helpMenuItem});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.setupMenuItem,
            this.previewMenuItem,
            this.printMenuItem,
            this.menuItem5,
            this.saveMenuItem,
            this.exitMenuItem});
            this.menuItem1.Text = "File";
            // 
            // setupMenuItem
            // 
            this.setupMenuItem.Index = 0;
            this.setupMenuItem.OwnerDraw = true;
            this.setupMenuItem.Text = "Page Setup";
            this.setupMenuItem.Click += new System.EventHandler(this.setupMenuItem_Click);
            // 
            // previewMenuItem
            // 
            this.previewMenuItem.Index = 1;
            this.previewMenuItem.OwnerDraw = true;
            this.previewMenuItem.Text = "Print Preview";
            this.previewMenuItem.Click += new System.EventHandler(this.previewMenuItem_Click);
            // 
            // printMenuItem
            // 
            this.printMenuItem.Index = 2;
            this.printMenuItem.OwnerDraw = true;
            this.printMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
            this.printMenuItem.Text = "Print";
            this.printMenuItem.Click += new System.EventHandler(this.printMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.OwnerDraw = true;
            this.menuItem5.Text = "-";
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Index = 4;
            this.saveMenuItem.OwnerDraw = true;
            this.saveMenuItem.Text = "Save";
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Index = 5;
            this.exitMenuItem.OwnerDraw = true;
            this.exitMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.exitMenuItem.Text = "Close";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // actionMenuItem
            // 
            this.actionMenuItem.Index = 1;
            this.actionMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.detailsMenuItem});
            this.actionMenuItem.Text = "Action";
            // 
            // detailsMenuItem
            // 
            this.detailsMenuItem.Index = 0;
            this.detailsMenuItem.OwnerDraw = true;
            this.detailsMenuItem.Text = "Show MARC";
            this.detailsMenuItem.Click += new System.EventHandler(this.detailsMenuItem_Click);           
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.Index = 2;
            this.helpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.aboutMenuItem});
            this.helpMenuItem.Text = "Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Index = 0;
            this.aboutMenuItem.OwnerDraw = true;
            this.aboutMenuItem.Text = "About";
            // 
            // detailsMenuItem_ContextMenuItem
            // 
            this.detailsMenuItem_ContextMenuItem.Index = 0;
            this.detailsMenuItem_ContextMenuItem.OwnerDraw = true;
            this.detailsMenuItem_ContextMenuItem.Text = "Show MARC";
            this.detailsMenuItem_ContextMenuItem.Click += new System.EventHandler(this.detailsMenuItem_Click);
            // 
            // commentsMenuItem_ContextMenuItem
            // 
            this.commentsMenuItem_ContextMenuItem.Index = 0;
            this.commentsMenuItem_ContextMenuItem.OwnerDraw = true;
            this.commentsMenuItem_ContextMenuItem.Text = "Show Comments";
            this.commentsMenuItem_ContextMenuItem.Click += new System.EventHandler(this.commentsMenuItem_Click);
            // 
            // actionMI_ShowDetails
            // 
            this.actionMI_ShowDetails.Index = -1;
            this.actionMI_ShowDetails.OwnerDraw = true;
            this.actionMI_ShowDetails.Text = "";
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.detailsMenuItem_ContextMenuItem,
            this.commentsMenuItem_ContextMenuItem});
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
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // printDialog1
            // 
            this.printDialog1.Document = this.printDocument1;
            // 
            // pageSetupDialog1
            // 
            this.pageSetupDialog1.Document = this.printDocument1;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            // 
            // customGrid_Panel1
            // 
            this.customGrid_Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.customGrid_Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.customGrid_Panel1.Current_Sort_String = "";
            this.customGrid_Panel1.DataTable = null;
            this.customGrid_Panel1.Location = new System.Drawing.Point(8, 0);
            this.customGrid_Panel1.Name = "customGrid_Panel1";
            this.customGrid_Panel1.Size = new System.Drawing.Size(776, 600);
            this.customGrid_Panel1.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(704, 608);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "EXIT";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Comma-seperated Values File (*.csv)|*.csv|Tab-delimited Text File (*.txt)|*.txt|E" +
                "xcel Spreadsheet (*.xls)|*.xls";
            this.saveFileDialog1.FilterIndex = 3;
            this.saveFileDialog1.Title = "Save As";
            // 
            // printButton
            // 
            this.printButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.printButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.printButton.ImageIndex = 2;
            this.printButton.ImageList = this.imageList1;
            this.printButton.Location = new System.Drawing.Point(113, 608);
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size(80, 23);
            this.printButton.TabIndex = 3;
            this.printButton.Text = "    Print";
            this.printButton.Click += new System.EventHandler(this.printButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.ImageIndex = 3;
            this.saveButton.ImageList = this.imageList1;
            this.saveButton.Location = new System.Drawing.Point(17, 608);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(80, 23);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "    Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // Results_Form
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(792, 641);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.printButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.customGrid_Panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "Results_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DLC Importer - Results Form";
            this.ResumeLayout(false);

		}
		#endregion

		#region Form-Related and Printing-Related Event Handlers

		#region Menu Item Events

		private void exitMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void setupMenuItem_Click(object sender, System.EventArgs e)
		{
			this.pageSetupDialog1.ShowDialog();
		}

		private void previewMenuItem_Click(object sender, System.EventArgs e)
		{
            gridPrinter.Prepare_To_Print();
			if (this.printPreviewDialog1.ShowDialog() == DialogResult.OK)
			{
			}
		}

		private void printMenuItem_Click(object sender, System.EventArgs e)
		{
            gridPrinter.Prepare_To_Print();
			if (printDialog1.ShowDialog() == DialogResult.OK)
			{
				printDocument1.Print();
			}
		}

		private void detailsMenuItem_Click(object sender, System.EventArgs e)
		{
            if (this.customGrid_Panel1.Selected_Row.Length > 0)
            {
                if (this.customGrid_Panel1.Selected_Row[0]["Related_File"].ToString().ToUpper().IndexOf(".METS") > 0)
                {
                    // Display the METS file                   
                    this.Open_METS(this.customGrid_Panel1.Selected_Row[0]["Related_File"].ToString());
                }
                else
                {
                    Process runFile = new Process();
                    runFile.StartInfo.FileName = this.customGrid_Panel1.Selected_Row[0]["Related_File"].ToString();
                    runFile.Start();
                }
            }
            else
                MessageBox.Show("Please select at least one row!", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

        private void commentsMenuItem_Click(object sender, System.EventArgs e)
        {
            if (this.customGrid_Panel1.Selected_Row.Length > 0)
            {
                if (this.customGrid_Panel1.Selected_Row[0]["Comment"].ToString().Length > 0)
                {

                    // parse the comment strings from the data row
                    string[] comments_array = this.customGrid_Panel1.Selected_Row[0]["Comment"].ToString().Split(new Char[] { ';', '.' });
                                                
                    string message = "The following is a list of comments for the selected row:               \n\n";

                    for (int i = 0; i < comments_array.Length; i++)
                    {
                        if (comments_array[i].Trim().Length > 0)
                            message = message + "* " + comments_array[i].Trim() + "\n\n";
                    }
                  
                    // display the contents of the Comments field
                    MessageBox.Show(message, "Comments", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select at least one row!", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
		}

        private void customGrid_Panel1_Double_Clicked(System.Data.DataRow thisRow)
        {
            if (thisRow != null)
            {
                this.Cursor = Cursors.WaitCursor;
                                             
                // check the importer name to determine what should be displayed
                switch (this.importerName)
                {
                    case "MARC_IMPORTER":
                        //Display the MARC record
                        int row_position = 0;
                        try
                        {
                            // get the index variable to display the correct MARC record
                            row_position = Convert.ToInt32(this.customGrid_Panel1.Selected_Row[0]["RecordIndex"].ToString());
                        }
                        catch { }

                     //   DLC_Importer_Main.Forms.Batch_Importer.MARC_Record_Form showDetails = new DLC_Importer_Main.Forms.Batch_Importer.MARC_Record_Form(resultList, resultTable, row_position);
                     //   showDetails.ShowDialog();
                        break;

                    case "METS_IMPORTER":
                    case "SPREADSHEET_IMPORTER":
                        // Display the METS file                   
                        this.Open_METS(this.customGrid_Panel1.Selected_Row[0]["METS_File_URL"].ToString());
                        break;
                }

                this.Cursor = Cursors.Default;
                this.Show();
            }
            else
                MessageBox.Show("Please select at least one row!", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

		private void saveMenuItem_Click(object sender, System.EventArgs e)
		{
            DialogResult result = this.saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                switch (this.saveFileDialog1.FilterIndex)
                {
                    case 1:
                        // save file as CSV (Comma delimited) and display message box                      
                        if (this.customGrid_Panel1.Export_as_Text(saveFileDialog1.FileName, ','))
                        {
                            MessageBox.Show("Data exported as text.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    case 2:
                        // save file as Text (Tab delimited) (*.txt) and display message box                        
                        if (this.customGrid_Panel1.Export_as_Text(saveFileDialog1.FileName, '\t'))
                        {
                            MessageBox.Show("Data exported as text.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    case 3:
                        // save file as Excel (*.xls) and open worksheet in Excel
                        this.customGrid_Panel1.Export_as_Excel(saveFileDialog1.FileName, "Batch Record Importer", "RESULTS");
                        break;

                    default:
                        break;
                }
            }
		}

		#endregion

		#region Button Event Handlers

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void saveButton_Click(object sender, System.EventArgs e)
		{
			DialogResult result = this.saveFileDialog1.ShowDialog();
			if ( result == DialogResult.OK )
			{
                switch (this.saveFileDialog1.FilterIndex)
                {
                    case 1:
                        // save file as CSV (Comma delimited) and display message box                      
                        if (this.customGrid_Panel1.Export_as_Text(saveFileDialog1.FileName, ','))
                        {
                            MessageBox.Show("Data exported as text.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }                        
                        break;

                    case 2:
                        // save file as Text (Tab delimited) (*.txt) and display message box                        
                        if (this.customGrid_Panel1.Export_as_Text(saveFileDialog1.FileName, '\t'))
                        {
                            MessageBox.Show("Data exported as text.\n\n" + saveFileDialog1.FileName + "       \n\n", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }                        
                        break;

                    case 3:
                        // save file as Excel (*.xls) and open worksheet in Excel
                        this.customGrid_Panel1.Export_as_Excel(saveFileDialog1.FileName, "Batch Record Importer", "RESULTS");
                        break;

                    default:                        
                        break;
                }
			}
		}

		private void printButton_Click(object sender, System.EventArgs e)
		{
            gridPrinter.Prepare_To_Print();
            printDialog1.UseEXDialog = true;
            DialogResult showResults = printDialog1.ShowDialog();
            if (showResults == DialogResult.OK)
			{
				printDocument1.Print();
			}
		}

		#endregion

        /// <summary> Method is called whenever this form is resized. </summary>
        /// <param name="e"></param>
        /// <remarks> This redraws the background of this form </remarks>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Get rid of any current background image
            if (this.BackgroundImage != null)
            {
                this.BackgroundImage.Dispose();
                this.BackgroundImage = null;
            }

            if (this.ClientSize.Width > 0)
            {
                // Create the items needed to draw the background
                Bitmap image = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                Graphics gr = Graphics.FromImage(image);
                Rectangle rect = new Rectangle(new Point(0, 0), this.ClientSize);

                // Create the brush
                LinearGradientBrush brush = new LinearGradientBrush(rect, SystemColors.Control, ControlPaint.Dark(SystemColors.Control), LinearGradientMode.Vertical);
                brush.SetBlendTriangularShape(0.33F);

                // Create the image
                gr.FillRectangle(brush, rect);
                gr.Dispose();

                // Set this as the background image
                this.BackgroundImage = image;
            }
        }     

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;
			DrawTopLabel(g);
			bool more = gridPrinter.DrawDataGrid(g);
			if (more == true)
			{
				e.HasMorePages = true;
                gridPrinter.Increment_Page();
			}
		}

		private void DrawTopLabel(Graphics g)
		{
			int TopMargin = printDocument1.DefaultPageSettings.Margins.Top;
		}

        private void Open_METS(string mets_filepath)
        {
            // Displays a METS file in the MetaTemplate.exe application.
            // This method uses parameter mets_filepath as the complete path to the METS file.

            if (mets_filepath.Length > 0)
            {
                try
                {                 
                    // check if the METS file exists
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(mets_filepath);
                    ProcessStartInfo startInfo;

                    if (!fileInfo.Exists)
                    {
                        // display message the file was not found                          
                        DialogResult result = MessageBox.Show("Unable to locate METS file (" + mets_filepath + ").", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                       
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    string mets_viewer = Application.StartupPath + "\\MetaTemplate.exe";
                    if (System.IO.File.Exists( mets_viewer ))
                    {
                        // METS Viewer application exists; open the METS file in edit mode
                        startInfo = new ProcessStartInfo(mets_viewer);
                        startInfo.Arguments = "false \"" + mets_filepath + "\"";

                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        startInfo.ErrorDialog = true;
                        startInfo.UseShellExecute = false;
                        Process.Start(startInfo);
                    }
                    else
                    {                                                  
                        try
                        {
                            // Try opening the METS file with the an application that has been selected 
                            // to open METS (*.mets) files.
                            startInfo = new ProcessStartInfo(mets_filepath);                                
                            startInfo.Arguments = mets_filepath;
                            Process.Start(startInfo);
                        }
                        catch 
                        {
                            // open the METS file with the default text editor                        
                            startInfo = new ProcessStartInfo("notepad.exe");
                            startInfo.Arguments = mets_filepath;
                            Process.Start(startInfo);
                        }                        
                    }
                }
                catch(Exception ex)
                {
                    DLC.Tools.Forms.ErrorMessageBox.Show("Error encountered while opening METS file!" + ex.Message, "Unexpected Error", ex);
                }

            }

            this.Cursor = Cursors.Default;
        }
      
		#endregion
	}
}
