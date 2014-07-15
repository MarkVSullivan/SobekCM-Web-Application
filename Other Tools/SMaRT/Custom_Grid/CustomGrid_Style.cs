using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;

namespace DLC.Custom_Grid
{
	/// <summary> CustomGrid_Style is a class which stores information about the 
	/// style of display to use with the CustomGrid object. </summary>
	public class CustomGrid_Style
	{
		#region Private Class Members

		/// <summary> Stores the collection of column styles </summary>
		private List<CustomGrid_ColumnStyle> columns;

		/// <summary> Stores the point to the visible columns </summary>
		private CustomGrid_VisibleColumns visibleColumns;

		/// <summary> Stores integer value accessible through public properties </summary>
		private int default_columnWidth, primaryKey, headerHeight, rowHeight;

		/// <summary> Stores alignment value accessible through public properties </summary>
		private HorizontalAlignment default_textAlignment;

		/// <summary> Stores Color value accessible through public properties </summary>
		private System.Drawing.Color default_backColor, headerBackColor, headerForeColor, selectedColor, rowSelectBackColor, rowSelectForeColor, noMatchesTextColor, alternating_print_backColor, gridLineColor, default_foreColor;

		/// <summary> Stores boolean value accessible through public properties </summary>
		private bool sortable, column_resizable;

		/// <summary> Stores the data source this style is linked to </summary>
		private DataTable source;

		/// <summary> Number of milliseconds to look for double click </summary>
		private int double_click_delay;

		private string no_matches_text = "No Matching Records!";

		#endregion

		#region Constructors

        /// <summary> Constructor for a new instance of the CustomGrid_Style class </summary>
		public CustomGrid_Style()
		{
			// Declare collection of columns
			columns = new List<CustomGrid_ColumnStyle>();
			visibleColumns = new CustomGrid_VisibleColumns( columns );

			// Set some defaults
			default_columnWidth = 100;
			headerHeight = 23;
			rowHeight = 20;
			default_textAlignment = HorizontalAlignment.Center;
			default_backColor = System.Drawing.Color.White;
			default_foreColor = System.Drawing.Color.Black;
			alternating_print_backColor = System.Drawing.Color.Honeydew;
			gridLineColor = System.Drawing.Color.Black;
			headerForeColor = System.Drawing.SystemColors.WindowText;
			headerBackColor = System.Drawing.SystemColors.Control;
			rowSelectForeColor = System.Drawing.SystemColors.WindowText;
			rowSelectBackColor = System.Drawing.SystemColors.Control;
			noMatchesTextColor = System.Drawing.Color.MediumBlue;
			selectedColor = System.Drawing.Color.Yellow;
			sortable = true;
			column_resizable = true;
			primaryKey = -1;
			double_click_delay = 1000;
		}

        /// <summary> Constructor for a new instance of the CustomGrid_Style class </summary>
        /// <param name="Data_Source"> DataTable for which to atuomatically build the style for </param>
		public CustomGrid_Style( DataTable Data_Source )
		{
			// Declare collection of columns
            columns = new List<CustomGrid_ColumnStyle>();
			visibleColumns = new CustomGrid_VisibleColumns( columns );

			// Set some defaults
			default_columnWidth = 100;
			headerHeight = 23;
			rowHeight = 20;
			default_textAlignment = HorizontalAlignment.Center;
			default_backColor = System.Drawing.Color.White;
			default_foreColor = System.Drawing.Color.Black;
			alternating_print_backColor = System.Drawing.Color.Honeydew;
			gridLineColor = System.Drawing.Color.Black;
			headerForeColor = System.Drawing.SystemColors.WindowText;
			headerBackColor = System.Drawing.SystemColors.Control;
			rowSelectForeColor = System.Drawing.SystemColors.WindowText;
			rowSelectBackColor = System.Drawing.SystemColors.Control;
			noMatchesTextColor = System.Drawing.Color.MediumBlue;
			selectedColor = System.Drawing.Color.Yellow;
			sortable = true;
			column_resizable = true;
			primaryKey = -1;
			double_click_delay = 1000;

			// Set this data source
			this.Data_Source = Data_Source;
		}

		#endregion

		#region Public Method

		/// <summary> Adds a new column style </summary>
		/// <returns> The style object created for this column </returns>
		public CustomGrid_ColumnStyle Add_Column()
		{
			// Create the new column 
			CustomGrid_ColumnStyle newColumn = new CustomGrid_ColumnStyle();

			// Configure some defaults
			newColumn.BackColor = this.Default_Column_Color;
			newColumn.ReadOnly = this.ReadOnly;
			newColumn.Width = this.Default_Column_Width;
			newColumn.Text_Alignment = this.Default_Text_Alignment;

			// Add this to the collection
			columns.Add( newColumn );

			// Set the name for the header
			newColumn.Header_Text = "Col" + columns.Count;

			// Return new  object
			return newColumn;
		}

		#endregion

		#region Public Properties

		/// <summary> Text displayed when there are no rows to display </summary>
		public string No_Matches_Text
		{
			get
			{
				return no_matches_text;
			}
			set
			{
				no_matches_text = value;
			}
		}

		/// <summary> Gets or sets the delay (in milliseconds) between the
		/// two clicks necessary to register as a double click. </summary>
		public int Double_Click_Delay
		{
			get	{	return double_click_delay;		}
			set	{	double_click_delay = value;		}
		}

		/// <summary> Gets and sets the flag which indicates these columns
		/// are resizable. </summary>
		public bool Columns_Resizable
		{
			get
			{
				return column_resizable;
			}
			set
			{
				column_resizable = value;
			}
		}

		/// <summary> Get the width to use for the row_select_button </summary>
		public int Row_Select_Button_Width
		{
			get
			{
				return 32;
			}
		}

		/// <summary> Gets the collecion of visible columns </summary>
		public CustomGrid_VisibleColumns Visible_Columns
		{
			get
			{
				return this.visibleColumns;
			}
		}

		/// <summary> Gets and sets the height of each row </summary>
		public int Row_Height
		{
			get
			{
				return rowHeight;
			}
			set
			{
				rowHeight = value;
			}
		}

		/// <summary> Gets and sets the height of the header </summary>
		public int Header_Height
		{
			get
			{
				return headerHeight;
			}
			set
			{
				headerHeight = value;
			}
		}

		/// <summary> Gets and sets the data table for this style </summary>
		public DataTable Data_Source
		{
			set
			{
				// Save this value
				source = value;

				if ( source != null )
				{
					// Clear the column collection
					columns.Clear();

					// Step through each column in the the data table
					CustomGrid_ColumnStyle thisColumnStyle;
					foreach( DataColumn thisColumn in source.Columns )
					{
						// Declare new column style for this column
						thisColumnStyle = new CustomGrid_ColumnStyle();

						// Assign default values to this style
						thisColumnStyle.Text_Alignment = default_textAlignment;
						thisColumnStyle.BackColor = default_backColor;
						thisColumnStyle.Visible = true;
						thisColumnStyle.ReadOnly = this.ReadOnly;
						thisColumnStyle.Width = default_columnWidth;

						// Assign a mapping name and header text
						if ( thisColumn.ColumnName.Length > 0 )
						{
							// Assign the mapping
							thisColumnStyle.Mapping_Name = thisColumn.ColumnName;
							thisColumnStyle.Header_Text = thisColumn.ColumnName.Replace("_"," ");

							// Make the first letter capitalized if not yet
							int ascii = (int) thisColumn.ColumnName[0];
							if (( ascii >= 97 ) && ( ascii <= 122 ))
							{
								if ( thisColumn.ColumnName.Length > 1 )
								{
									thisColumnStyle.Header_Text = thisColumn.ColumnName.Substring(0,1).ToUpper() + thisColumn.ColumnName.Substring(1).Replace("_"," ");
								}
								else
								{
									thisColumnStyle.Header_Text = thisColumn.ColumnName.ToUpper();
								}
							}
						}

						// See if this is the primary key, if none assigned yer
						primaryKey = -1;
						if ( primaryKey == -1 )
						{
							foreach( DataColumn primCheck in source.PrimaryKey )
							{
								// Is this the primary key
								if ( primCheck == thisColumn )
								{	
									// Set this as invisible
									thisColumnStyle.Visible = false;

									// Save this as the primary key
									primaryKey = thisColumn.Ordinal;

									// Stop this check
									break;									
								}
							}
						}

						// Add this column style to the collection
						columns.Add( thisColumnStyle );
					}
				}
			}
			get
			{
				return source;
			}
		}

		/// <summary> Gets the collection of column style objects </summary>
		public List<CustomGrid_ColumnStyle> Column_Styles
		{
			get
			{
				return columns;
			}
		}

		/// <summary> Gets and sets the backcolor for the header </summary>
		public System.Drawing.Color Header_Back_Color
		{
			get
			{
				return headerBackColor;
			}
			set
			{
				headerBackColor = value;
			}
		}

		/// <summary> Gets and sets the fore color for the header </summary>
		public System.Drawing.Color Header_Fore_Color
		{
			get
			{
				return headerForeColor;
			}
			set
			{
				headerForeColor = value;
			}
		}

		/// <summary> Gets and sets no matches text color </summary>
		public System.Drawing.Color No_Matches_Text_Color
		{
			get
			{
				return noMatchesTextColor;
			}
			set
			{
				noMatchesTextColor = value;
			}
		}

		/// <summary> Gets and sets the backcolor for the row selection button </summary>
		public System.Drawing.Color Row_Select_Button_Back_Color
		{
			get
			{
				return rowSelectBackColor;
			}
			set
			{
				rowSelectBackColor = value;
			}
		}

		/// <summary> Gets and sets the fore color for the row selection button </summary>
		public System.Drawing.Color Row_Select_Button_Fore_Color
		{
			get
			{
				return rowSelectForeColor;
			}
			set
			{
				rowSelectForeColor = value;
			}
		}

		/// <summary> Gets and sets the background color for the cells  </summary>
		public System.Drawing.Color Default_Column_Color
		{
			get
			{
				return default_backColor;
			}
			set
			{
				default_backColor = value;
				
				// Change the values for all the column styles
				foreach( CustomGrid_ColumnStyle thisColStyle in columns )
				{
					thisColStyle.BackColor = value;
				}
			}
		}

		/// <summary> Gets and sets the color to use for the text in the grid </summary>
		public System.Drawing.Color ForeColor
		{
			get
			{
				return default_foreColor;
			}
			set
			{
				default_foreColor = value;
			}
		}

		/// <summary> Gets and sets the color to use for alternating lines when 
		/// this grid is printed. </summary>
		public System.Drawing.Color Alternating_Print_BackColor
		{
			get
			{
				return alternating_print_backColor;
			}
			set
			{
				alternating_print_backColor = value;
			}
		}

		/// <summary> Gets and sets the color to use for for the grid lines
		/// when displaying or printing this grid. </summary>
		public System.Drawing.Color Grid_Line_Color
		{
			get
			{
				return gridLineColor;
			}
			set
			{
				gridLineColor = value;
			}
		}

		/// <summary> Gets and sets the color to use when a row is selected </summary>
		public System.Drawing.Color Selection_Color
		{
			get
			{
				return selectedColor;
			}
			set
			{
				selectedColor = value;
			}
		}

		/// <summary> Gets and sets the flag which indicates if this table is sortable </summary>
		public bool Sortable
		{
			get
			{
				return sortable;
			}
			set
			{
				sortable = value;
			}
		}

		/// <summary> Gets and sets the flag which indicates if this table is ready only </summary>
		/// <value> This value is always TRUE</value>
		public bool ReadOnly
		{
			get
			{
				return true;
			}
			set
			{
				// This is ALWAYS true, so do nothing
			}
		}

		/// <summary> Sets the text alignment for all the data in the rows </summary>
		public HorizontalAlignment Default_Text_Alignment
		{
			get
			{
				return default_textAlignment;
			}
			set
			{
				// Save the alignment
				default_textAlignment = value;

				// Change the values for all the column styles
				foreach( CustomGrid_ColumnStyle thisColStyle in columns )
				{
					thisColStyle.Text_Alignment = value;
				}
			}
		}

		/// <summary> Sets the default column width for all columns </summary>
		public int Default_Column_Width
		{
			get
			{
				return default_columnWidth;
			}
			set
			{
				default_columnWidth = value;
			}
		}

		/// <summary> Gets and sets the primary key value for this table </summary>
		public int Primary_Key_Column
		{
			get
			{
				return this.primaryKey;
			}
			set
			{
				this.primaryKey = value;
			}
		}

		#endregion

	}
}
