#region Using directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using SobekCM.Library;
using SobekCM.Management_Tool.Importer.Forms;
using SobekCM.Management_Tool.Reports;
using SobekCM.Management_Tool.Versioning;

#endregion

namespace SobekCM.Management_Tool
{
	/// <summary> Main form for the SMaRT tool ( SobekCM Management and Reporting Tool )</summary>
	public class MainForm : Form
	{
	    private MainMenu cMmainMenu;
	    private MenuItem aboutMenuItem;
        private MenuItem actionMenuItem;
	    private IContainer components;
        private Round_Button exitButton;
	    private MenuItem exitMenuItem;
	    private MenuItem helpMenuItem;
	    private LinkLabel importRecordsLinkLabel;
	    private Label label2;
        private LinkLabel linkLabel1;
	    private Label mainLabel;
	    private Panel panel1;
	    private LinkLabel reportingModuleLinkLabel;
	    private MenuItem retrievePackagesMenuItem;
	    private LinkLabel viewItemsLinkLabel;

	    #region Constructor

	    /// <summary> Constructor for a new instance of the MainForm class </summary>
	    public MainForm(  )
	    {
	        // Initialize this form
	        InitializeComponent();
	        BackColor = Color.FromArgb(240, 240, 240);

	        // Personalize several labels and controls now for the SobekCM Instance Name
	        mainLabel.Text = SobekCM_Library_Settings.System_Abbreviation + " Manager";
	        linkLabel1.Text = "Retrieve " + SobekCM_Library_Settings.System_Abbreviation + " Packages";
	        retrievePackagesMenuItem.Text = "Retrieve " + SobekCM_Library_Settings.System_Abbreviation + " Packages";
	        aboutMenuItem.Text = "About '" + SobekCM_Library_Settings.System_Abbreviation + " Manager'";
	        viewItemsLinkLabel.Text = "View " + SobekCM_Library_Settings.System_Abbreviation + " Items";
	        reportingModuleLinkLabel.Text = SobekCM_Library_Settings.System_Abbreviation + " Reporting Module";

	    }

	    public override sealed Color BackColor
	    {
	        get { return base.BackColor; }
	        set { base.BackColor = value; }
	    }

	    #endregion

	    #region Windows Form Designer generated code
	    /// <summary>
	    /// Required method for Designer support - do not modify
	    /// the contents of this method with the code editor.
	    /// </summary>
	    private void InitializeComponent()
	    {
	        this.components = new System.ComponentModel.Container();
	        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
	        this.mainLabel = new System.Windows.Forms.Label();
	        this.cMmainMenu = new System.Windows.Forms.MainMenu(this.components);
	        this.actionMenuItem = new System.Windows.Forms.MenuItem();
	        this.retrievePackagesMenuItem = new System.Windows.Forms.MenuItem();
	        this.exitMenuItem = new System.Windows.Forms.MenuItem();
	        this.helpMenuItem = new System.Windows.Forms.MenuItem();
	        this.aboutMenuItem = new System.Windows.Forms.MenuItem();
	        this.label2 = new System.Windows.Forms.Label();
	        this.linkLabel1 = new System.Windows.Forms.LinkLabel();
	        this.reportingModuleLinkLabel = new System.Windows.Forms.LinkLabel();
	        this.panel1 = new System.Windows.Forms.Panel();
	        this.viewItemsLinkLabel = new System.Windows.Forms.LinkLabel();
	        this.importRecordsLinkLabel = new System.Windows.Forms.LinkLabel();
	        this.exitButton = new SobekCM.Management_Tool.Round_Button();
	        this.panel1.SuspendLayout();
	        this.SuspendLayout();
	        // 
	        // mainLabel
	        // 
	        this.mainLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
	                                                                      | System.Windows.Forms.AnchorStyles.Right)));
	        this.mainLabel.BackColor = System.Drawing.Color.Transparent;
	        this.mainLabel.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.mainLabel.ForeColor = System.Drawing.Color.MediumBlue;
	        this.mainLabel.Location = new System.Drawing.Point(16, 9);
	        this.mainLabel.Name = "mainLabel";
	        this.mainLabel.Size = new System.Drawing.Size(435, 32);
	        this.mainLabel.TabIndex = 2;
	        this.mainLabel.Text = "SobekCM Manager";
	        this.mainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
	        // 
	        // CMmainMenu
	        // 
	        this.cMmainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
	                                                                                   this.actionMenuItem,
	                                                                                   this.helpMenuItem});
	        // 
	        // actionMenuItem
	        // 
	        this.actionMenuItem.Index = 0;
	        this.actionMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
	                                                                                       this.retrievePackagesMenuItem,
	                                                                                       this.exitMenuItem});
	        this.actionMenuItem.Text = "&Actions";
	        // 
	        // retrievePackagesMenuItem
	        // 
	        this.retrievePackagesMenuItem.Index = 0;
	        this.retrievePackagesMenuItem.Text = "Retrieve " + SobekCM_Library_Settings.System_Abbreviation + " Packages";
	        this.retrievePackagesMenuItem.Click += new System.EventHandler(this.retrievePackagesMenuItem_Click);
	        // 
	        // exitMenuItem
	        // 
	        this.exitMenuItem.Index = 1;
	        this.exitMenuItem.Text = "Exit";
	        this.exitMenuItem.Click += new System.EventHandler(this.exitButton_Click);
	        // 
	        // helpMenuItem
	        // 
	        this.helpMenuItem.Index = 1;
	        this.helpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
	                                                                                     this.aboutMenuItem});
	        this.helpMenuItem.Text = "&Help";
	        // 
	        // aboutMenuItem
	        // 
	        this.aboutMenuItem.Index = 0;
	        this.aboutMenuItem.Text = "&About this application";
	        this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
	        // 
	        // label2
	        // 
	        this.label2.AutoSize = true;
	        this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.label2.Location = new System.Drawing.Point(52, 37);
	        this.label2.Name = "label2";
	        this.label2.Size = new System.Drawing.Size(247, 19);
	        this.label2.TabIndex = 5;
	        this.label2.Text = "What would you like to do today?";
	        // 
	        // linkLabel1
	        // 
	        this.linkLabel1.AutoSize = true;
	        this.linkLabel1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.linkLabel1.Location = new System.Drawing.Point(107, 206);
	        this.linkLabel1.Name = "linkLabel1";
	        this.linkLabel1.Size = new System.Drawing.Size(192, 18);
	        this.linkLabel1.TabIndex = 1;
	        this.linkLabel1.TabStop = true;
	        this.linkLabel1.Text = "Retrieve SobekCM Packages";
	        this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
	        // 
	        // reportingModuleLinkLabel
	        // 
	        this.reportingModuleLinkLabel.AutoSize = true;
	        this.reportingModuleLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.reportingModuleLinkLabel.Location = new System.Drawing.Point(107, 166);
	        this.reportingModuleLinkLabel.Name = "reportingModuleLinkLabel";
	        this.reportingModuleLinkLabel.Size = new System.Drawing.Size(185, 18);
	        this.reportingModuleLinkLabel.TabIndex = 3;
	        this.reportingModuleLinkLabel.TabStop = true;
	        this.reportingModuleLinkLabel.Text = "SobekCM Reporting Module";
	        this.reportingModuleLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.reportingModuleLinkLabel_LinkClicked);
	        // 
	        // panel1
	        // 
	        this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	                                                                    | System.Windows.Forms.AnchorStyles.Left)
	                                                                   | System.Windows.Forms.AnchorStyles.Right)));
	        this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
	        this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
	        this.panel1.Controls.Add(this.viewItemsLinkLabel);
	        this.panel1.Controls.Add(this.importRecordsLinkLabel);
	        this.panel1.Controls.Add(this.label2);
	        this.panel1.Controls.Add(this.reportingModuleLinkLabel);
	        this.panel1.Controls.Add(this.linkLabel1);
	        this.panel1.Location = new System.Drawing.Point(21, 44);
	        this.panel1.Name = "panel1";
	        this.panel1.Size = new System.Drawing.Size(430, 288);
	        this.panel1.TabIndex = 0;
	        // 
	        // viewItemsLinkLabel
	        // 
	        this.viewItemsLinkLabel.AutoSize = true;
	        this.viewItemsLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.viewItemsLinkLabel.Location = new System.Drawing.Point(107, 86);
	        this.viewItemsLinkLabel.Name = "viewItemsLinkLabel";
	        this.viewItemsLinkLabel.Size = new System.Drawing.Size(146, 18);
	        this.viewItemsLinkLabel.TabIndex = 0;
	        this.viewItemsLinkLabel.TabStop = true;
	        this.viewItemsLinkLabel.Text = "View SobekCM Items";
	        this.viewItemsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.viewItemsLinkLabel_LinkClicked);
	        // 
	        // importRecordsLinkLabel
	        // 
	        this.importRecordsLinkLabel.AutoSize = true;
	        this.importRecordsLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.importRecordsLinkLabel.Location = new System.Drawing.Point(106, 126);
	        this.importRecordsLinkLabel.Name = "importRecordsLinkLabel";
	        this.importRecordsLinkLabel.Size = new System.Drawing.Size(110, 18);
	        this.importRecordsLinkLabel.TabIndex = 4;
	        this.importRecordsLinkLabel.TabStop = true;
	        this.importRecordsLinkLabel.Text = "Import Records";
	        this.importRecordsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.importRecordsLinkLabel_LinkClicked);
	        // 
	        // exitButton
	        // 
	        this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
	        this.exitButton.BackColor = System.Drawing.Color.Transparent;
	        this.exitButton.Button_Enabled = true;
	        this.exitButton.Button_Text = "EXIT";
	        this.exitButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Standard;
	        this.exitButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.exitButton.Location = new System.Drawing.Point(357, 338);
	        this.exitButton.Name = "exitButton";
	        this.exitButton.Size = new System.Drawing.Size(94, 26);
	        this.exitButton.TabIndex = 1;
	        this.exitButton.Button_Pressed += new System.EventHandler(this.exitButton_Button_Pressed);
	        // 
	        // MainForm
	        // 
	        this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
	        this.BackColor = System.Drawing.SystemColors.Control;
	        this.ClientSize = new System.Drawing.Size(467, 380);
	        this.Controls.Add(this.panel1);
	        this.Controls.Add(this.exitButton);
	        this.Controls.Add(this.mainLabel);
	        this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
	        this.MaximizeBox = false;
	        this.Menu = this.cMmainMenu;
	        this.MinimumSize = new System.Drawing.Size(483, 418);
	        this.Name = "MainForm";
	        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
	        this.Text = "SobekCM Management and Reporting Tool (SMaRT)";
	        this.panel1.ResumeLayout(false);
	        this.panel1.PerformLayout();
	        this.ResumeLayout(false);

	    }


	    /// <summary>Clean up any resources being used.</summary>
	    protected override void Dispose( bool disposing )
	    {
	        if( disposing )
	        {
	            if (components != null) 
	            {
	                components.Dispose();
	            }
	        }
	        base.Dispose( disposing );
	    }

	    #endregion

	    #region Event Handlers

	    private void exitButton_Click(object sender, EventArgs e)
	    {
	        Close();
	    }

	    private void aboutMenuItem_Click(object sender, EventArgs e)
	    {
	        // Show the about form
	        About showAbout = new About( "SobekCM Management and Reporting Tool", VersionConfigSettings.AppVersion);
	        showAbout.ShowDialog();			
	    }

	    #endregion

	    #region Method to draw the form background

	    /// <summary> Method is called whenever this form is resized. </summary>
	    /// <param name="e"></param>
	    /// <remarks> This redraws the background of this form </remarks>
	    protected override void OnResize(EventArgs e)
	    {
	        base.OnResize(e);

	        // Get rid of any current background image
	        if (BackgroundImage != null)
	        {
	            BackgroundImage.Dispose();
	            BackgroundImage = null;
	        }

	        if (ClientSize.Width > 0)
	        {
	            // Create the items needed to draw the background
	            Bitmap image = new Bitmap(ClientSize.Width, ClientSize.Height);
	            Graphics gr = Graphics.FromImage(image);
	            Rectangle rect = new Rectangle(new Point(0, 0), ClientSize);

	            // Create the brush
	            LinearGradientBrush brush = new LinearGradientBrush(rect, BackColor, ControlPaint.Dark(BackColor), LinearGradientMode.Vertical);
	            brush.SetBlendTriangularShape(0.33F);

	            // Create the image
	            gr.FillRectangle(brush, rect);
	            gr.Dispose();

	            // Set this as the backgroundf
	            BackgroundImage = image;
	        }
	    }

	    #endregion

	    private void exitButton_Button_Pressed(object sender, EventArgs e)
        {
            Close();
        }

        private void retrievePackagesMenuItem_Click(object sender, EventArgs e)
        {
            Retrieve_SobekCM_Items_Form showForm = new Retrieve_SobekCM_Items_Form();
            Hide();
            showForm.ShowDialog();
            Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Retrieve_SobekCM_Items_Form showForm = new Retrieve_SobekCM_Items_Form();
            Hide();
            showForm.ShowDialog();
            Show();
        }

        private void viewItemsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Item_Discovery_Form showForm = new Item_Discovery_Form();
            Hide();
            showForm.ShowDialog();
            Show();
        }

        private void importRecordsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Must have a value (and be valid with write access) to the main builder folder
            // to be able to import things
            string dropbox = SobekCM_Library_Settings.Main_Builder_Input_Folder;
            if (( String.IsNullOrEmpty(dropbox)) || ( !Directory.Exists(dropbox)))
            {
                MessageBox.Show(
                    "The system-wide setting for the 'Main Builder Input Folder' is either not set or is not set correctly.  This directory must exist and be accessible from the machine you are running the SMaRT Tool from in order to import records.",
                    "System-wide Setting Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Verify write access
            try
            {
                string testfile = dropbox + "\\" + Tools.SecurityInfo.Current_UserName.Replace("\\", "") + ".test";
                StreamWriter writer = new StreamWriter(testfile);
                writer.Write("TEST");
                writer.Flush();
                writer.Close();

                File.Delete(testfile);
            }
            catch ( Exception ee )
            {
                MessageBox.Show(
                    "You must have write/modify rights on the 'Main Builder Input Folder' in order to import records.\n\n" + dropbox,
                    "System-wide Setting Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Importer_Form importForm = new Importer_Form("UF", "University of Florida");
            Hide();
            importForm.ShowDialog();
            Show();
        }

        private void reportingModuleLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Reports_Form reportingForm = new Reports_Form();
            Hide();
            reportingForm.ShowDialog();
            Show();
        }


	}
}
