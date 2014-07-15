using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Globalization;

namespace Generic_Importer.Forms
{
	/// <summary>Custom control which allows null date to be selected</summary>
	public delegate void DateValue_Changed_delegate( string thisValue );
	
	/// <summary>
	/// Summary description for Nullable_DateTimePicker.
	/// </summary>
	public class Nullable_DateTimePicker : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.DateTimePicker dateTimePicker1;
		private System.Windows.Forms.TextBox dateTextBox;
		

		public event DateValue_Changed_delegate Date_Changed;

		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>Constructor for a new Nullable_DateTimePicker.</summary>
		public Nullable_DateTimePicker()
		{		
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.dateTimePicker1.Format = DateTimePickerFormat.Custom;
			this.dateTimePicker1.CustomFormat = "MM/dd/yyyy";					
			this.dateTimePicker1.MaxDate = DateTime.Today;
			this.dateTimePicker1.Value = DateTime.Today;	
			this.dateTextBox.Text = DateTime.Today.ToString("MM/dd/yyyy");	
		}

		/// <summary> Consturctor for a new Nullable_DateTimePicker.</summary>
		/// <param name="thisDate"></param>
		public Nullable_DateTimePicker(string thisDate)
		{
			InitializeComponent();
			this.dateTimePicker1.CustomFormat = "MM/dd/yyyy";
			this.dateTimePicker1.Format = DateTimePickerFormat.Custom;			
			this.dateTimePicker1.MaxDate = DateTime.Today;
			
			try
			{
				this.dateTimePicker1.Value = Convert.ToDateTime(thisDate);				
				this.dateTextBox.Text = this.dateTimePicker1.Value.ToString("MM/dd/yyyy");
			}
			catch
			{   
				this.dateTimePicker1.Value = DateTime.Today;
				this.dateTextBox.Text = DateTime.Today.ToString("MM/dd/yyyy");
			}
			
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
			this.dateTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// dateTimePicker1
			// 
			this.dateTimePicker1.CustomFormat = "MM/dd/yyyy";
			this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker1.Location = new System.Drawing.Point(0, 0);
			this.dateTimePicker1.MinDate = new System.DateTime(1990, 1, 1, 0, 0, 0, 0);
			this.dateTimePicker1.Name = "dateTimePicker1";
			this.dateTimePicker1.Size = new System.Drawing.Size(90, 21);
			this.dateTimePicker1.TabIndex = 1;
			this.dateTimePicker1.MouseHover += new System.EventHandler(this.dateTimePicker1_MouseHover);
			this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
			// 
			// dateTextBox
			// 
			this.dateTextBox.Location = new System.Drawing.Point(0, 0);
			this.dateTextBox.Name = "dateTextBox";
			this.dateTextBox.Size = new System.Drawing.Size(72, 21);
			this.dateTextBox.TabIndex = 0;
			this.dateTextBox.Text = "";
			this.dateTextBox.TextChanged += new System.EventHandler(this.dateTextBox_TextChanged);
			// 
			// Nullable_DateTimePicker
			// 
			this.Controls.Add(this.dateTextBox);
			this.Controls.Add(this.dateTimePicker1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "Nullable_DateTimePicker";
			this.Size = new System.Drawing.Size(88, 23);
			this.ResumeLayout(false);

		}
		#endregion



		private void dateTimePicker1_ValueChanged(object sender, System.EventArgs e)
		{
			this.dateTextBox.Text = this.dateTimePicker1.Text;
			if (Date_Changed != null)
				Date_Changed(this.dateTextBox.Text.Trim());
		}


		private void dateTimePicker1_MouseHover(object sender, System.EventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.Default;
		}

		private void dateTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (this.dateTextBox.Text.Trim().Length == 0 )
				if (Date_Changed != null)
					Date_Changed(this.dateTextBox.Text.Trim());
		}
		


		public string DATE
		{
			get
			{
				return this.dateTextBox.Text.Trim();
			}
			set
			{
				try
				{
					this.dateTimePicker1.Value = Convert.ToDateTime(value);
					this.dateTextBox.Text = ((string)(value + " ")).Split(" ".ToCharArray())[0];

				}
				catch
				{
					this.dateTextBox.Text = "";
					this.dateTimePicker1.Value = DateTime.Today;
				}
			}
		}

	}
}
