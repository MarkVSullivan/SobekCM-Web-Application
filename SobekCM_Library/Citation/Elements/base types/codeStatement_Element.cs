using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using UFDC_Bib_Package;
using DLC.MetaTemplate.Template;

namespace DLC.MetaTemplate.Elements
{
	/// <summary>
	/// Summary description for codeStatement_Element.
	/// </summary>
	public abstract class codeStatement_Element : abstract_Element
	{
	//	protected DrawFlat.FlatComboBox thisCodeBox;
        protected ComboBox thisCodeBox;
		protected System.Windows.Forms.TextBox thisStatementBox;
		protected System.Windows.Forms.TextBox readonlyCodeBox;
		private bool restrict_values;
        private bool isXP;

		public codeStatement_Element()
		{
			// Configure the code box
		//	thisCodeBox = new FlatComboBox();
            thisCodeBox = new ComboBox();
			thisCodeBox.Location = new Point( 115, 2 );
			thisCodeBox.Width = 100;
			thisCodeBox.TextChanged +=new EventHandler(thisCodeBox_TextChanged);
            thisCodeBox.Sorted = true;
            thisCodeBox.Enter += new EventHandler(thisCodeBox_Enter);
            thisCodeBox.Leave += new EventHandler(thisCodeBox_Leave);
            thisCodeBox.ForeColor = System.Drawing.Color.MediumBlue;
			this.Controls.Add( thisCodeBox );

			// Configure the code box, but leave it hidden
			readonlyCodeBox = new TextBox();
			readonlyCodeBox.Location = new Point( 115, 2 );
			readonlyCodeBox.Width = 100;
			readonlyCodeBox.Hide();
			readonlyCodeBox.BackColor = Color.WhiteSmoke;
			readonlyCodeBox.ReadOnly = true;
            readonlyCodeBox.Enter += new EventHandler(textBox_Enter);
            readonlyCodeBox.Leave += new EventHandler(textBox_Leave);
            readonlyCodeBox.ForeColor = System.Drawing.Color.MediumBlue;
			this.Controls.Add( readonlyCodeBox );

			// Configure the statement box
			thisStatementBox = new TextBox();
            thisStatementBox.TextChanged += new EventHandler(thisStatementBox_TextChanged);
			thisStatementBox.Location = new Point( 240, 2 );
			thisStatementBox.BackColor = Color.White;
            thisStatementBox.Enter += new EventHandler(textBox_Enter);
            thisStatementBox.Leave += new EventHandler(textBox_Leave);
            thisStatementBox.ForeColor = System.Drawing.Color.MediumBlue;
			this.Controls.Add( thisStatementBox );

			// Set default title to blank
			title = "no default";

            if (!DLC.Tools.Windows_Appearance_Checker.is_XP_Theme)
            {
                thisCodeBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                thisStatementBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                readonlyCodeBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                isXP = false;
            }
            else
            {
                isXP = true;
            }
		}

		public codeStatement_Element( string defaultTitle )
		{
			// Configure the code box
	//		thisCodeBox = new FlatComboBox();
            thisCodeBox = new ComboBox();
			thisCodeBox.Location = new Point( 115, 2 );
			thisCodeBox.Width = 100;
			thisCodeBox.TextChanged +=new EventHandler(thisCodeBox_TextChanged);
            thisCodeBox.Sorted = true;
            thisCodeBox.Enter += new EventHandler(thisCodeBox_Enter);
            thisCodeBox.Leave += new EventHandler(thisCodeBox_Leave);
            thisCodeBox.ForeColor = System.Drawing.Color.MediumBlue;
			this.Controls.Add( thisCodeBox );

			// Configure the code box, but leave it hidden
			readonlyCodeBox = new TextBox();
			readonlyCodeBox.Location = new Point( 115, 2 );
			readonlyCodeBox.Width = 100;
			readonlyCodeBox.Hide();
			readonlyCodeBox.BackColor = Color.WhiteSmoke;
			readonlyCodeBox.ReadOnly = true;
            readonlyCodeBox.Enter += new EventHandler(textBox_Enter);
            readonlyCodeBox.Leave += new EventHandler(textBox_Leave);
            readonlyCodeBox.ForeColor = System.Drawing.Color.MediumBlue;
			this.Controls.Add( readonlyCodeBox );

			// Configure the statement box
			thisStatementBox = new TextBox();
			thisStatementBox.Location = new Point( 240, 2 );
			thisStatementBox.BackColor = Color.White;
            thisStatementBox.TextChanged += new EventHandler(thisStatementBox_TextChanged);
            thisStatementBox.Enter += new EventHandler(textBox_Enter);
            thisStatementBox.Leave += new EventHandler(textBox_Leave);
            thisStatementBox.ForeColor = System.Drawing.Color.MediumBlue;
			this.Controls.Add( thisStatementBox );

			// Save the title
			title = defaultTitle;

            if (!DLC.Tools.Windows_Appearance_Checker.is_XP_Theme)
            {
                thisCodeBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                thisStatementBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                readonlyCodeBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                isXP = false;
            }
            else
            {
                isXP = true;
            }
		}

        void thisCodeBox_Leave(object sender, EventArgs e)
        {
            if (!read_only)
            {
                thisCodeBox.BackColor = Color.White;
            }
        }

        void thisCodeBox_Enter(object sender, EventArgs e)
        {
            if (!read_only)
            {
                thisCodeBox.BackColor = Color.Khaki;
            }
        }

        void textBox_Leave(object sender, EventArgs e)
        {
            if (!read_only)
            {
                ((TextBox)sender).BackColor = Color.White;
            }
        }

        void textBox_Enter(object sender, EventArgs e)
        {
            if (!read_only)
            {
                ((TextBox)sender).BackColor = Color.Khaki;
            }
        }

		/// <summary> Gets and sets the flag indicating values are limited to
		/// those in the drop down list. </summary>
		protected bool Restrict_Values
		{
			get	{	return restrict_values;		}
			set	
			{
				restrict_values = value;

				if ( value )
				{
					thisCodeBox.DropDownStyle = ComboBoxStyle.DropDownList;
				}
				else
				{
					thisCodeBox.DropDownStyle = ComboBoxStyle.DropDown;
				}
			}
		}

		/// <summary> Add a code to this combo box </summary>
		/// <param name="newItem"></param>
		public void Add_Code( string newItem )
		{
			if ( !thisCodeBox.Items.Contains( newItem ))
			{
				thisCodeBox.Items.Add( newItem );
			}
		}

		/// <summary> Override the OnPaint method to draw the title before the text box </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			// Draw the title
			Draw_Title( e.Graphics, title );

			// Determine the y-mid-point
			int midpoint = (int) (1.5 * this.Font.SizeInPoints );

			// If this is repeatable, show the '+' to add another after this one
			Draw_Repeatable_Icon( e.Graphics, this.Width - 22, midpoint - 8 );

			// Call this for the base
			OnPaint (e);

            if ((!isXP) && ( !read_only ))
            {
                e.Graphics.DrawRectangle(new Pen(Color.Black), thisCodeBox.Location.X - 1, thisCodeBox.Location.Y - 1, thisCodeBox.Width + 1, thisCodeBox.Height + 1);
            }
		}

		#region Methods Implementing the Abstract Methods from abstract_Element class

		/// <summary> Reads the inner data from the Template XML format </summary>
		protected override void Inner_Read_Data( XmlTextReader xmlReader )
		{
			string default_value = String.Empty;
			while ( xmlReader.Read() )
			{
				if (( xmlReader.NodeType == XmlNodeType.Element ) && (( xmlReader.Name.ToLower() == "code" ) || ( xmlReader.Name.ToLower() == "options" ) || ( xmlReader.Name.ToLower() == "statement"))) 
				{
					if ( xmlReader.Name.ToLower() == "code" )
					{
						xmlReader.Read();
						default_value = xmlReader.Value.Trim();
					}

					if ( xmlReader.Name.ToLower() == "statement" )
					{
						xmlReader.Read();
						thisStatementBox.Text = xmlReader.Value.Trim();
					}

					if ( xmlReader.Name.ToLower() == "options" )
					{
						xmlReader.Read();
						string options = xmlReader.Value.Trim();
						thisCodeBox.Items.Clear();
						if ( options.Length > 0 )
						{
							string[] options_parsed = options.Split(",".ToCharArray());
							foreach( string thisOption in options_parsed )
							{
								if ( !thisCodeBox.Items.Contains( thisOption.Trim() ))
								{
									thisCodeBox.Items.Add( thisOption.Trim() );
								}
							}
						}
					}
				}
			}

			// Set the value if there was one
			if ( default_value.Length > 0 )
			{	
				this.thisCodeBox.Text = default_value;
				this.readonlyCodeBox.Text = default_value;
			}
		}

		/// <summary> Writes the inner data into Template XML format </summary>
		protected override string Inner_Write_Data( )
		{
			return String.Empty;
		}

		/// <summary> Perform any height setting calculations specific to the 
		/// implementation of abstract_Element.  </summary>
		/// <param name="size"> Height of the font </param>
		protected override void Inner_Set_Height( float size )
		{
			// Set total height
			int size_int = (int) size;
			this.Height = size_int + ( size_int + 9 ) + 1;

			// Now, set the height of the text box
			//			thisBox.Height =  ( size_int + 7 ) + 4;
		}

		/// <summary> Perform any width setting calculations specific to the 
		/// implementation of abstract_Element.  </summary>
		/// <param name="size"> Height of the font </param>
		protected override void Inner_Set_Width( int new_width )
		{
			// Set the width of the text box
			thisStatementBox.Width = new_width - title_length - 155;
			thisStatementBox.Location = new Point( title_length + 125, thisStatementBox.Location.Y );

			// Set the location of the code box
			thisCodeBox.Location = new Point( title_length, thisCodeBox.Location.Y );
			readonlyCodeBox.Location = new Point( title_length, readonlyCodeBox.Location.Y );
		}

		/// <summary> Perform any readonly functions specific to the
		/// implementation of abstract_Element. </summary>
		protected override void Inner_Set_Read_Only()
		{
			if ( read_only )
			{
				thisCodeBox.Enabled = false;
				thisCodeBox.Hide();
				readonlyCodeBox.Text = thisCodeBox.Text;
				readonlyCodeBox.Show();
				thisStatementBox.ReadOnly = true;
				thisStatementBox.BackColor = Color.WhiteSmoke;
			}
			else
			{
				thisCodeBox.Enabled = true;
				thisCodeBox.Show();
				readonlyCodeBox.Hide();
				thisStatementBox.ReadOnly = false;
				thisStatementBox.BackColor = Color.White;
			}
		}

		/// <summary> Clones this element, not copying the actual data
		/// in the fields, but all other values. </summary>
		/// <returns>Clone of this element</returns>
		public override abstract_Element Clone()
		{
			// Get the new element
			codeStatement_Element newElement = (codeStatement_Element) Element_Factory.getElement( this.Type, this.Display_SubType );
			newElement.Location = this.Location;
			newElement.Language = this.Language;
			newElement.Title_Length = this.Title_Length;
			newElement.Height = this.Height;
			newElement.Font = this.Font;
			newElement.Set_Width( this.Width );
			newElement.Index = this.Index + 1;

			// Copy the combo box specific values
			newElement.Restrict_Values = this.Restrict_Values;
			foreach( string thisItem in thisCodeBox.Items )
			{
				newElement.Add_Code( thisItem );
			}

			return newElement;
		}

		/// <summary> Gets the flag indicating this element has an entered value </summary>
		public override bool hasValue
		{
			get
			{
				if (( this.thisCodeBox.Text.Trim().Length > 0 ) || ( this.thisStatementBox.Text.Trim().Length > 0 ))
					return true;
				else
					return false;
			}
		}

		/// <summary> Checks the data in this element for validity. </summary>
		/// <returns> TRUE if valid, otherwise FALSE </returns>
		/// <remarks> This sets the <see cref="abstract_Element.Invalid_String" /> value. </remarks>
		public override bool isValid()
		{
			return true;
		}

		#endregion

		private void thisCodeBox_TextChanged(object sender, EventArgs e)
		{
			this.readonlyCodeBox.Text = thisCodeBox.Text;
            OnDataChanged();
		}

        void thisStatementBox_TextChanged(object sender, EventArgs e)
        {
            OnDataChanged();
        }
	}
}
