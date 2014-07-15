#region Using directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool
{
	/// <summary> About is a form used to display the simple versioning and copyright information
	/// for an application. <br /> <br /> </summary>
	/// <remarks> This form appears and goes away when the user clicks the form anywhere.  This form
    /// is often used in concert with the <see cref="SobekCM.Management_Tool.Versioning.VersionChecker"/> and <see cref="SobekCM.Management_Tool.Versioning.VersionConfigSettings"/> objects,
	/// as shown in the second example below. 	
	/// <br /> <br />
	/// Object created by Mark V Sullivan (2003) for University of Florida's Digital Library Center.  </remarks>
	/// <example> EXAMPLE 1: Shows how to use this form in a stand-alone application.
	/// <code> <SPAN class="lang">[C#]</SPAN> 
	///	using System;
	///	using UF.Libraries.DLC.CustomTools.Forms;
	///
	///	namespace UF.Libraries.DLC.CustomTools
	///	{
	///		public class AboutForm_Example_1
	///		{
	///			static void Main() 
	///			{
	///				// Create a new About Form
	///				About showAbout = new About( "Baldwin Record Importer", "1.1.0" );
	///				showAbout.ShowDialog();
	///			}
	///		}
	///	}
	///	</code> <br />
	///	Below is what this form will look like: <br /> <br />
	///	<img src="AboutForm.jpg" />
	/// <br /> <br /> <br />
    /// EXAMPLE 2: Using the About form in conjunction with the <see cref="SobekCM.Management_Tool.Versioning.VersionChecker"/> solution and <see cref="SobekCM.Management_Tool.Versioning.VersionConfigSettings"/> object.
	/// <code> <SPAN class="lang">[C#]</SPAN> 
	///	using System;
	///	using UF.Libraries.DLC.CustomTools;
	///	using UF.Libraries.DLC.CustomTools.Forms;
	///
	///	namespace UF.Libraries.DLC.CustomTools
	///	{
	///		public class AboutForm_Example_2
	///		{
	///			static void Main() 
	///			{
	///				// Create a new About Form
	///				About showAbout = new About( VersionConfigSettings.AppName, VersionConfigSettings.AppName );
	///				showAbout.ShowDialog();
	///			}
	///		}
	///	}
	///	</code>
	/// </example>
	public class About : Form
	{
	    /// <summary> Private form-related Label variable holds the textual boxes. </summary>
        private Label appNameLabel;

	    /// <summary> Private form-related PictureBox variable holds the UF seal. </summary>
	    private PictureBox ufSealPictureBox;

	    /// <summary> Private form-related Panel variable holds all the information in the white box </summary>
	    private Panel uFimagePanel;

	    /// <summary> Required designer variable. </summary>
		private Container components;

	    /// <summary> Private form-related Label variable holds the textual boxes. </summary>
	    private Label developedByLabel;

	    private LinkLabel linkLabel1;
        private PictureBox pictureBox1;
        private RichTextBox richTextBox1;

	    /// <summary> Private form-related Label variable holds the textual boxes. </summary>
	    private Label versionLabel;

	    /// <summary> Default constructor accepts the version number of this software. </summary>
		/// <param name="appName"> Name of the application </param>
		/// <param name="version"> Version number for the application </param>
		public About( string appName, string version )
		{
			// Save the parameters

	        // Initialize this form
			InitializeComponent();
            BackColor = Color.FromArgb(240, 240, 240);

			// Now, modify the form correctly
			Text = "About " + appName + " ( Version " + version + " )";
			versionLabel.Text = "( Version " + version + " )";
			appNameLabel.Text = appName;

            // Set the content of the rich text box
            richTextBox1.Text = "This tool is used to query a SobekCM library locally, bulk import records, pull metadata or files from a local instance, and perform tracking functions during digitization/loading of resources.  Features which are supported by the web application are seamlessly integrated with this tool.  This tool currently must be used with network access to the SobekCM database. ";
		}

	    public override sealed string Text
	    {
	        get { return base.Text; }
	        set { base.Text = value; }
	    }

	    public override sealed Color BackColor
	    {
	        get { return base.BackColor; }
	        set { base.BackColor = value; }
	    }

	    #region Windows Form Designer generated code

	    /// <summary> Clean up any resources being used. </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.uFimagePanel = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ufSealPictureBox = new System.Windows.Forms.PictureBox();
            this.appNameLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.developedByLabel = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.uFimagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ufSealPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // uFimagePanel
            // 
            this.uFimagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uFimagePanel.BackColor = System.Drawing.Color.White;
            this.uFimagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uFimagePanel.Controls.Add(this.richTextBox1);
            this.uFimagePanel.Controls.Add(this.pictureBox1);
            this.uFimagePanel.Controls.Add(this.ufSealPictureBox);
            this.uFimagePanel.Controls.Add(this.appNameLabel);
            this.uFimagePanel.Controls.Add(this.versionLabel);
            this.uFimagePanel.Controls.Add(this.developedByLabel);
            this.uFimagePanel.Location = new System.Drawing.Point(12, 12);
            this.uFimagePanel.Name = "uFimagePanel";
            this.uFimagePanel.Size = new System.Drawing.Size(586, 341);
            this.uFimagePanel.TabIndex = 0;
            this.uFimagePanel.Click += new System.EventHandler(this.About_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.Color.White;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(21, 154);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(548, 117);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = "This tool is used to query a SobekCM library locally, bulk import records, pull m" +
                "etadata or files from a local instance, and";
            this.richTextBox1.Click += new System.EventHandler(this.About_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(21, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 123);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.About_Click);
            // 
            // ufSealPictureBox
            // 
            this.ufSealPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ufSealPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ufSealPictureBox.Image")));
            this.ufSealPictureBox.Location = new System.Drawing.Point(348, 276);
            this.ufSealPictureBox.Name = "ufSealPictureBox";
            this.ufSealPictureBox.Size = new System.Drawing.Size(233, 59);
            this.ufSealPictureBox.TabIndex = 0;
            this.ufSealPictureBox.TabStop = false;
            this.ufSealPictureBox.Click += new System.EventHandler(this.About_Click);
            // 
            // appNameLabel
            // 
            this.appNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.appNameLabel.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appNameLabel.Location = new System.Drawing.Point(164, 0);
            this.appNameLabel.Name = "appNameLabel";
            this.appNameLabel.Size = new System.Drawing.Size(405, 98);
            this.appNameLabel.TabIndex = 2;
            this.appNameLabel.Text = "SobekCM Management and Reporting Tool";
            this.appNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.appNameLabel.Click += new System.EventHandler(this.About_Click);
            // 
            // versionLabel
            // 
            this.versionLabel.BackColor = System.Drawing.Color.Transparent;
            this.versionLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.Location = new System.Drawing.Point(166, 98);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(412, 23);
            this.versionLabel.TabIndex = 3;
            this.versionLabel.Text = "Version ...";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // developedByLabel
            // 
            this.developedByLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.developedByLabel.BackColor = System.Drawing.Color.Transparent;
            this.developedByLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.developedByLabel.Location = new System.Drawing.Point(3, 294);
            this.developedByLabel.Name = "developedByLabel";
            this.developedByLabel.Size = new System.Drawing.Size(288, 41);
            this.developedByLabel.TabIndex = 1;
            this.developedByLabel.Text = "Developed by Mark Sullivan for the Digital Library Center at the University of Fl" +
                "orida George A. Smathers Libraries";
            this.developedByLabel.Click += new System.EventHandler(this.About_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Location = new System.Drawing.Point(234, 358);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(161, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Click anywhere to close this form";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // About
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(610, 380);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.uFimagePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About SMaRT";
            this.TopMost = true;
            this.Click += new System.EventHandler(this.About_Click);
            this.uFimagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ufSealPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

	    }
	    #endregion

	    /// <summary> Private Event_Handler is called when the form is clicked.  
		/// This closes the form. </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void About_Click(object sender, EventArgs e)
		{
			Close();
		}

        ///// <summary> Method is called whenever this form is resized. </summary>
        ///// <param name="e"></param>
        ///// <remarks> This redraws the background of this form </remarks>
        //protected override void OnResize(EventArgs e)
        //{
        //    base.OnResize(e);

        //    // Get rid of any current background image
        //    if (this.BackgroundImage != null)
        //    {
        //        this.BackgroundImage.Dispose();
        //        this.BackgroundImage = null;
        //    }

        //    if (this.ClientSize.Width > 0)
        //    {
        //        // Create the items needed to draw the background
        //        Bitmap image = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
        //        Graphics gr = Graphics.FromImage(image);
        //        Rectangle rect = new Rectangle(new Point(0, 0), this.ClientSize);

        //        // Create the brush
        //        LinearGradientBrush brush = new LinearGradientBrush(rect, ControlPaint.Dark(this.BackColor), this.BackColor, LinearGradientMode.Vertical);
        //        brush.SetBlendTriangularShape(0.33F);

        //        // Create the image
        //        gr.FillRectangle(brush, rect);
        //        gr.Dispose();

        //        // Set this as the backgroundf
        //        this.BackgroundImage = image;
        //    }
        //}

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }



	}
}
