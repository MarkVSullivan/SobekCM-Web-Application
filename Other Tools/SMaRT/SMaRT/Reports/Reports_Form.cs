#region Using directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#endregion

namespace SobekCM.Management_Tool.Reports
{
	/// <summary> Reporting form for the SMaRT tool ( SobekCM Management and Reporting Tool )</summary>
	public class Reports_Form : Form
	{
	    private LinkLabel aggregationSpaceUsedLinkLabel;
	    private IContainer components;
	    private LinkLabel customReportLinkLabel;
	    private Round_Button exitButton;
        private Label label2;
	    private LinkLabel linkLabel4;
	    private Label mainLabel;
	    private Panel panel1;
	    private LinkLabel pendingOnlineActivationLinkLabel;
	    private LinkLabel trackingBoxLinkLabel;

	    #region Constructor

	    /// <summary> Constructor for a new instance of the MainForm class </summary>
	    public Reports_Form( )
	    {
	        // Initialize this form
	        InitializeComponent();
	        BackColor = Color.FromArgb(240, 240, 240);

	        string username = Environment.UserName.ToLower();
	        if ((username.IndexOf("msulliva") < 0) && (username.IndexOf("laurien") < 0))
	        {
	            aggregationSpaceUsedLinkLabel.Enabled = false;
	        }
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
	        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Reports_Form));
	        this.mainLabel = new System.Windows.Forms.Label();
	        this.label2 = new System.Windows.Forms.Label();
	        this.pendingOnlineActivationLinkLabel = new System.Windows.Forms.LinkLabel();
	        this.linkLabel4 = new System.Windows.Forms.LinkLabel();
	        this.panel1 = new System.Windows.Forms.Panel();
	        this.customReportLinkLabel = new System.Windows.Forms.LinkLabel();
	        this.aggregationSpaceUsedLinkLabel = new System.Windows.Forms.LinkLabel();
	        this.exitButton = new SobekCM.Management_Tool.Round_Button();
	        this.trackingBoxLinkLabel = new System.Windows.Forms.LinkLabel();
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
	        this.mainLabel.Text = "Available Reports";
	        this.mainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
	        // 
	        // label2
	        // 
	        this.label2.AutoSize = true;
	        this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.label2.Location = new System.Drawing.Point(52, 37);
	        this.label2.Name = "label2";
	        this.label2.Size = new System.Drawing.Size(239, 19);
	        this.label2.TabIndex = 5;
	        this.label2.Text = "Select an available report below:";
	        // 
	        // pendingOnlineActivationLinkLabel
	        // 
	        this.pendingOnlineActivationLinkLabel.AutoSize = true;
	        this.pendingOnlineActivationLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.pendingOnlineActivationLinkLabel.Location = new System.Drawing.Point(107, 126);
	        this.pendingOnlineActivationLinkLabel.Name = "pendingOnlineActivationLinkLabel";
	        this.pendingOnlineActivationLinkLabel.Size = new System.Drawing.Size(168, 18);
	        this.pendingOnlineActivationLinkLabel.TabIndex = 1;
	        this.pendingOnlineActivationLinkLabel.TabStop = true;
	        this.pendingOnlineActivationLinkLabel.Text = "Pending Online Complete";
	        this.pendingOnlineActivationLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.pendingOnlineActivationLinkLabel_LinkClicked);
	        // 
	        // linkLabel4
	        // 
	        this.linkLabel4.AutoSize = true;
	        this.linkLabel4.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.linkLabel4.Location = new System.Drawing.Point(107, 166);
	        this.linkLabel4.Name = "linkLabel4";
	        this.linkLabel4.Size = new System.Drawing.Size(208, 18);
	        this.linkLabel4.TabIndex = 3;
	        this.linkLabel4.TabStop = true;
	        this.linkLabel4.Text = "Newspapers without Serial Info";
	        this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
	        // 
	        // panel1
	        // 
	        this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	                                                                    | System.Windows.Forms.AnchorStyles.Left)
	                                                                   | System.Windows.Forms.AnchorStyles.Right)));
	        this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
	        this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
	        this.panel1.Controls.Add(this.trackingBoxLinkLabel);
	        this.panel1.Controls.Add(this.customReportLinkLabel);
	        this.panel1.Controls.Add(this.aggregationSpaceUsedLinkLabel);
	        this.panel1.Controls.Add(this.label2);
	        this.panel1.Controls.Add(this.linkLabel4);
	        this.panel1.Controls.Add(this.pendingOnlineActivationLinkLabel);
	        this.panel1.Location = new System.Drawing.Point(21, 44);
	        this.panel1.Name = "panel1";
	        this.panel1.Size = new System.Drawing.Size(430, 308);
	        this.panel1.TabIndex = 0;
	        // 
	        // customReportLinkLabel
	        // 
	        this.customReportLinkLabel.AutoSize = true;
	        this.customReportLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.customReportLinkLabel.Location = new System.Drawing.Point(107, 86);
	        this.customReportLinkLabel.Name = "customReportLinkLabel";
	        this.customReportLinkLabel.Size = new System.Drawing.Size(173, 18);
	        this.customReportLinkLabel.TabIndex = 0;
	        this.customReportLinkLabel.TabStop = true;
	        this.customReportLinkLabel.Text = "Ad Hoc Report Generator";
	        this.customReportLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.customReportLinkLabel_LinkClicked);
	        // 
	        // aggregationSpaceUsedLinkLabel
	        // 
	        this.aggregationSpaceUsedLinkLabel.AutoSize = true;
	        this.aggregationSpaceUsedLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.aggregationSpaceUsedLinkLabel.Location = new System.Drawing.Point(106, 206);
	        this.aggregationSpaceUsedLinkLabel.Name = "aggregationSpaceUsedLinkLabel";
	        this.aggregationSpaceUsedLinkLabel.Size = new System.Drawing.Size(178, 18);
	        this.aggregationSpaceUsedLinkLabel.TabIndex = 4;
	        this.aggregationSpaceUsedLinkLabel.TabStop = true;
	        this.aggregationSpaceUsedLinkLabel.Text = "Aggregation Space Utilized";
	        this.aggregationSpaceUsedLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.aggregationSpaceUsedLinkLabel_LinkClicked);
	        // 
	        // exitButton
	        // 
	        this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
	        this.exitButton.BackColor = System.Drawing.Color.Transparent;
	        this.exitButton.Button_Enabled = true;
	        this.exitButton.Button_Text = "CLOSE";
	        this.exitButton.Button_Type = SobekCM.Management_Tool.Round_Button.Button_Type_Enum.Standard;
	        this.exitButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.exitButton.Location = new System.Drawing.Point(357, 358);
	        this.exitButton.Name = "exitButton";
	        this.exitButton.Size = new System.Drawing.Size(94, 26);
	        this.exitButton.TabIndex = 1;
	        this.exitButton.Button_Pressed += new System.EventHandler(this.exitButton_Button_Pressed);
	        // 
	        // trackingBoxLinkLabel
	        // 
	        this.trackingBoxLinkLabel.AutoSize = true;
	        this.trackingBoxLinkLabel.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.trackingBoxLinkLabel.Location = new System.Drawing.Point(107, 246);
	        this.trackingBoxLinkLabel.Name = "trackingBoxLinkLabel";
	        this.trackingBoxLinkLabel.Size = new System.Drawing.Size(156, 18);
	        this.trackingBoxLinkLabel.TabIndex = 6;
	        this.trackingBoxLinkLabel.TabStop = true;
	        this.trackingBoxLinkLabel.Text = "Tracking Boxes Report";
	        this.trackingBoxLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.trackingBoxLinkLabel_LinkClicked);
	        // 
	        // Reports_Form
	        // 
	        this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
	        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
	        this.ClientSize = new System.Drawing.Size(475, 411);
	        this.Controls.Add(this.panel1);
	        this.Controls.Add(this.exitButton);
	        this.Controls.Add(this.mainLabel);
	        this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
	        this.MaximizeBox = false;
	        this.MinimumSize = new System.Drawing.Size(483, 418);
	        this.Name = "Reports_Form";
	        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
	        this.Text = "Reporting Module";
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

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Groups_Needed_Serial_Hierarchy_Form showForm = new Groups_Needed_Serial_Hierarchy_Form();
                Hide();
                showForm.ShowDialog();
                Show();
            }
            catch
            {
                MessageBox.Show("A direct connection to the database is required for this report.", "No Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                linkLabel4.Enabled = false;
            } 
        }

        private void aggregationSpaceUsedLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Aggregation_Space_Used_Form showForm = new Aggregation_Space_Used_Form();
            Hide();
            showForm.ShowDialog();
            Show();
        }

        private void customReportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Ad_Hoc_Reporting_Query_Form showForm = new Ad_Hoc_Reporting_Query_Form();
            Hide();
            showForm.ShowDialog();
            Show();
        }

        private void pendingOnlineActivationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Items_Pending_Online_Complete_Form showForm = new Items_Pending_Online_Complete_Form();
            Hide();
            showForm.ShowDialog();
            Show();
        }

        private void trackingBoxLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tracking_Boxes_Report_Form showForm = new Tracking_Boxes_Report_Form();
            Hide();
            showForm.ShowDialog();
            Show();
        }
	}
}
