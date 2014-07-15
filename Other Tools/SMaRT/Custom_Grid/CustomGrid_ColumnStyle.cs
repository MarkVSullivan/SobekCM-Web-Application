using System;
using System.Drawing;
using System.Windows.Forms;

namespace DLC.Custom_Grid
{
	/// <summary> CustomGrid_ColumnStyle stores data about the style to use
	/// while displaying a column </summary>
	public class 
        CustomGrid_ColumnStyle
	{
		#region Private Class Members

		/// <summary> Stores the horizonatal alignment for text in this column. </summary>
		private HorizontalAlignment textAlignment;

		/// <summary> Stores the text to display in the header of this column </summary>
		private string headerText;

		/// <summary> Stores the name in the DataTable this column matches </summary>
		private string mappingName;

		/// <summary> Stores the flag which indicates whether to display this column </summary>
		private bool visible;

		/// <summary> Stores the color which is the background color for this column </summary>
		private Color backColour;

		/// <summary> Stores the width for this column to dispay </summary>
		private int width;

		/// <summary> Stores the sort string to be used when this column should be ascending </summary>
		private string ascending_sort;

		/// <summary> Stores the sort string to be used when this column should be descending </summary>
		private string descending_sort;

        private int fixed_print_width;
        private bool short_date_form;

		#endregion

        /// <summary> Constructor for a new instance of the CustomGrid_ColumnStyle class </summary>
		public CustomGrid_ColumnStyle()
		{
			// Set some defaults
			width = 100;
			mappingName = "";
			headerText = "";
			textAlignment = HorizontalAlignment.Center;
			backColour = System.Drawing.Color.White;
			visible = true;
			ascending_sort = "";
			descending_sort = "";
            fixed_print_width = -1;
            short_date_form = false;
		}

		#region Public Properties

		/// <summary> Gets and sets the horizonatal alignment for text in this column. </summary>
		public HorizontalAlignment Text_Alignment
		{
			get
			{
				return textAlignment;
			}
			set
			{
				textAlignment = value;
			}
		}

		/// <summary> Gets and sets the text to display in the header of this column </summary>
		public string Header_Text
		{
			get
			{
				return headerText;
			}
			set
			{
				headerText = value;
			}
		}

		/// <summary> Gets and sets the name this column maps to in the data table </summary>
		public string Mapping_Name
		{
			get
			{
				return mappingName;
			}
			set
			{
				mappingName = value;
				ascending_sort = value + " ASC";
				descending_sort = value + " DESC";
			}
		}

		/// <summary> Gets and sets the flag which indicates whether to display this column </summary>
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}

		/// <summary> Gets and sets the flag which indicates if this is readonly </summary>
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

		/// <summary> Gets and sets the color which is the background color for this column </summary>
		public Color BackColor
		{
			get
			{
				return backColour;
			}
			set
			{
				backColour = value;
			}
		}

        /// <summary> Gets and sets the fixed print width for this column to dispay </summary>
        public int Fixed_Print_Width
        {
            get
            {
                return fixed_print_width;
            }
            set
            {
                fixed_print_width = value;
            }
        }

		/// <summary> Gets and sets the width for this column to dispay </summary>
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		/// <summary> Gets and sets the string used to sort this column in ascending value 
		/// in the context of the entire DataTable </summary>
		public string Ascending_Sort
		{
			get
			{	
				return ascending_sort;
			}
			set
			{
				ascending_sort = value;
			}
		}

		/// <summary> Gets and sets the string used to sort this column in descending value 
		/// in the context of the entire DataTable </summary>
		public string Descending_Sort
		{
			get
			{	
				return descending_sort;
			}
			set
			{
				descending_sort = value;
			}
		}

        /// <summary> Flag indicates that if this column is a datetime type, display the
        /// date in short date format, excluding the time  </summary>
        public bool Short_Date_Format
        {
            get
            {
                return short_date_form;
            }
            set
            {
                short_date_form = value;
            }
        }

		#endregion
	}
}
