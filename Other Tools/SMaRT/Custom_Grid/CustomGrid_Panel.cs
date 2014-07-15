#region Using directives

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GemBox.Spreadsheet;

#endregion

namespace DLC.Custom_Grid
{
	#region Public Delegate Declarations

	/// <summary> Custom delegate is used for events which can return multiple rows </summary>
    /// <param name="rows"> Array of rows returned with the event  </param>
	public delegate void CustomGrid_Panel_Delegate_Multiple( DataRow[] rows );

    /// <summary> Custom delegate is used for events which return a single row </summary>
    /// <param name="thisRow"> Single row to return with the event </param>
	public delegate void CustomGrid_Panel_Delegate_Single( DataRow thisRow );

	#endregion

	/// <summary> Form user control to provide a custom view of a database, with built-in support for selecting, resorting, firing
    /// double-click events for the user, printing, and exporting to common file formats.  </summary>
    /// <remarks> Written by Mark Sullivan (2005) </remarks>
	public class CustomGrid_Panel : UserControl
	{
	    #region Public Event Declarations

	    /// <summary> Event is fired when the current selection changes </summary>
	    public event CustomGrid_Panel_Delegate_Multiple Selection_Changed;

	    /// <summary> Event is fired when a row is double clicked </summary>
	    public event CustomGrid_Panel_Delegate_Single Double_Clicked;

        /// <summary> Event is fired when the user requests to copy selected rows into the clipboard</summary>
        public event CustomGrid_Panel_Delegate_Multiple Clipboard_Copy_Requested;



	    #endregion

	    #region Private Class Variables

	    /// <summary> Stores the style to display for this table </summary>
	    private readonly CustomGrid_Style style;

	    /// <summary> Stores the button which is currently being resized </summary>
	    private int columnBeingResized = -1;
	    private int columnResizeStart;

	    /// <summary> 
	    /// Required designer variable.
	    /// </summary>
	    private Container components;

	    private Custom_Panel_w_Scroll_Events dataPanel;
	    private readonly Timer doubleClickTimer;

	    private readonly Font headerFont = new Font( "Tahoma", 8.25F, FontStyle.Bold );
	    private Panel headerPanel;
	    private Panel innerRowPanel;

	    /// <summary> Stores the index of the last clicked row in this panel </summary>
	    protected int last_clicked_row;

	    private Button selectAllButton;
	    private bool selectable = true;

	    /// <summary> Stores the key of the currently selected row </summary>
	    protected SortedList selected_row_numbers;

	    private bool settingBothMenus;
	    private bool sortAsc;
	    private int sortColumn = -1;

	    /// <summary> Stores the datatable passed in </summary>
	    private DataTable sourceTable;

	    /// <summary> Stores the view on the source </summary>
	    private DataView sourceView;

	    private int totalWidth;
	    private VScrollBar vScrollBar1;
	    private int[] visibleColumnIndexes;

	    #endregion

	    #region Constructors

	    /// <summary> Constructor for a new instance of the Custom Grid class </summary>
	    public CustomGrid_Panel()
	    {
	        selected_row_numbers = new SortedList();

	        // This call is required by the Windows.Forms Form Designer.
	        InitializeComponent();

	        // Create a new CustomGrid_Style object
	        style = new CustomGrid_Style();

	        // Create the visible column list
	        create_visible_column_index_list();

	        // Focus on the data panel
	        dataPanel.Focus();

	        // Create the double click timer
	        doubleClickTimer = new Timer {Interval = style.Double_Click_Delay};
	        doubleClickTimer.Tick+=double_click_timer_Tick;

	        // Set the size of the select all button
	        selectAllButton.Width = style.Row_Select_Button_Width + 1;
	        selectAllButton.Height = style.Header_Height;

            // Add the listener for the Ctrl-C 
	        dataPanel.KeyDown += dataPanel_KeyDown;
	    }

	    // Listen for the Ctrl-C combination for copying selected rows into the clipboard

	    /// <summary> Constructor for a new instance of the Custom Grid class </summary>
	    public CustomGrid_Panel( DataTable thisTable )
	    {
	        selected_row_numbers = new SortedList();

	        // This call is required by the Windows.Forms Form Designer.
	        InitializeComponent();

	        // Create a new CustomGrid_Style object
	        style = new CustomGrid_Style();

	        // Save this datatable
	        DataTable = thisTable;

	        // Create the double click timer
	        doubleClickTimer = new Timer {Interval = style.Double_Click_Delay};
	        doubleClickTimer.Tick+=double_click_timer_Tick;

	        // Set the size of the select all button
	        selectAllButton.Width = style.Row_Select_Button_Width + 1;
	        selectAllButton.Height = style.Header_Height;

            // Add the listener for the Ctrl-C 
            dataPanel.KeyDown += new KeyEventHandler(dataPanel_KeyDown );
            dataPanel.KeyPress += new KeyPressEventHandler(dataPanel_KeyPress);
	    }

	    protected void dataPanel_KeyDown(object sender, KeyEventArgs e)
	    {
	        if ((e.Control) && (e.KeyCode == Keys.C))
	        {
	            e.Handled = true;
	            OnClipboardCopyRequested();
	        }
	        else
	        {
	            base.OnKeyDown(e);
	        }
	    }

        protected void dataPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

        }

	    #endregion

	    #region Public Properties

	    /// <summary> Indicates if this grid panel allows users to select a row </summary>
	    public bool Selectable
	    {
	        get { return selectable; }
	        set { selectable = value; }
	    }

	    /// <summary> Gets the currently selected row's datarow </summary>
	    public DataRow[] Selected_Row
	    {
	        get
	        {
	            // If a row is selected, return that, otherwise NULL
	            if ( selected_row_numbers.Count >= 0 )
	            {
	                DataRow[] returnValues = new DataRow[ selected_row_numbers.Count ];
	                for( int i = 0 ; i < selected_row_numbers.Count ; i++ )
	                {
	                    if ( Convert.ToInt32(selected_row_numbers.GetByIndex(i)) >= sourceView.Count )
	                    {
	                        return null;
	                    }

                        // else...
	                    returnValues[i] = sourceView[ Convert.ToInt32(selected_row_numbers.GetByIndex(i)) ].Row;
	                }
	                return returnValues;
	            }
	            
                // else..
                return null;
	        }
	    }

	    /// <summary> Number of rows currently selected </summary>
	    public int Selected_Row_Count
	    {
	        get
	        {
	            return selected_row_numbers.Count;
	        }
	    }

	    /// <summary> Gets and sets the borderstyle for this panel. </summary>
	    public new BorderStyle BorderStyle
	    {
	        get
	        {
	            return dataPanel.BorderStyle;
	        }
	        set
	        {
	            dataPanel.BorderStyle = value;
	        }
	    }

	    /// <summary> Gets the font currently being used by the header row </summary>
	    public Font Header_Font
	    {
	        get
	        {
	            return headerFont;
	        }
	    }

	    /// <summary> Gets the style parameter for this panel </summary>
	    public CustomGrid_Style Style
	    {
	        get
	        {
	            return style;
	        }	
	    }
        	
	    /// <summary> Set and get the data source for this </summary>
	    public DataTable DataTable
	    {
	        get	{	return sourceTable;		}
	        set
	        {
	            // Save this value
	            sourceTable = value;
				
	            // Reset the sort column info
	            sortColumn = -1;
	            sortAsc = false;

	            // Continue only if the source table is not null
	            if ( value != null )
	            {
	                // Create the column styles from the table
	                style.Data_Source = value;

	                // Declare a new data view on that
	                sourceView = new DataView( sourceTable );

	                // Clear the currently selected index
	                if (selected_row_numbers != null)
	                    selected_row_numbers.Clear();

	                // Get new list of visible column indexes
	                create_visible_column_index_list();

	                // Configure the vertical scroll bar
	                config_vscrollbar();

	                // Compute the total width
	                totalWidth = style.Row_Select_Button_Width;
	                foreach (int t in visibleColumnIndexes)
	                {
                        // Add the width to the running total
	                    totalWidth += style.Column_Styles[ t ].Width;
	                }

	                // Make sure the width matched
	                if ( innerRowPanel.Width != ( totalWidth + 6) )
	                {
	                    innerRowPanel.Width = ( totalWidth + 6);
	                }

	                // Clear the main panel
	                dataPanel.Invalidate();
	                headerPanel.Invalidate();
	                innerRowPanel.Invalidate();
	            }
	        }
	    }

	    /// <summary> Gets the current data view  </summary>
	    internal DataView DataView
	    {
	        get
	        {
	            return sourceView;
	        }
	    }

	    /// <summary> Gets the current number of matches, or rows, being displayed </summary>
	    public int View_Count
	    {
	        get
	        {
	            return sourceView.Count;
	        }
	    }

	    /// <summary>Gets and sets the current sort string of the dataview </summary>
	    public string Current_Sort_String
	    {
	        get {
	            return sourceView != null ? sourceView.Sort : "";
	        }
	        set
	        {
	            if ( sourceView != null )
	                sourceView.Sort = value;
	            sortColumn = -1;
	        }
	    }

	    /// <summary>Gets the filter string of the dataview</summary>
	    public string Current_Filter_String
	    {
	        get {
	            return sourceView != null ? sourceView.RowFilter : "";
	        }
	    }

	    #endregion

	    #region Public Methods

	    /// <summary> Forces the panel to redraw itself and refocus on the inner row panel </summary>
	    public void ReDraw()
	    {
	        config_vscrollbar();
	        dataPanel.Invalidate( true );
	        innerRowPanel.Focus();
	    }

	    /// <summary> Adds a context menu to this custom data grid </summary>
	    /// <param name="e"></param>
	    protected override void OnContextMenuChanged(EventArgs e)
	    {
	        if ( !settingBothMenus )
	        {
	            // Save the new menu
	            ContextMenu menu = ContextMenu;
	            ContextMenu = null;
	            innerRowPanel.ContextMenu = menu;
	        }
	    }

	    /// <summary> Sets the two context menus to be used by this grid panel </summary>
	    /// <param name="Off_Row_Menu"> Context menu to be used when the user is not over a row in this panel </param>
	    /// <param name="On_Row_Menu"> Context menu to be used when the user is over a row in this panel </param>
	    public void Set_Context_Menus( ContextMenu Off_Row_Menu, ContextMenu On_Row_Menu )
	    {
	        settingBothMenus = true;

	        ContextMenu = Off_Row_Menu;
	        innerRowPanel.ContextMenu = On_Row_Menu;

	        settingBothMenus = false;
	    }

	    /// <summary> Add a filter to the displayed rows </summary>
	    /// <param name="rowFilter"> New filter to apply </param>
	    /// <returns> Number of rows that match the filter </returns>
	    public int Add_Filter( string rowFilter )
	    {
	        // Apply the filter
	        sourceView.RowFilter = rowFilter;

	        // Deselect any currently selected row
	        if (selected_row_numbers != null)
	            selected_row_numbers.Clear();
			
	        // Show the results
	        vScrollBar1.Value = 0;
	        config_vscrollbar();
	        innerRowPanel.Invalidate();

	        // Return the number of rows
	        return sourceView.Count;
	    }

	    /// <summary> Refresh to a table with the same schema as the
	    /// old table.  </summary>
	    /// <param name="thisSourceTable"></param>
	    public void Refresh_DataTable(DataTable thisSourceTable)
	    {
	        // Save the filter and sort
	        string sort = sourceView.Sort;
	        string filter = sourceView.RowFilter;

	        sourceTable = thisSourceTable;

	        // Assign the new source table
	        sourceView.Table = sourceTable;

	        // Reassign the old filter and sort
	        sourceView.Sort = sort;
	        sourceView.RowFilter = filter;

	        // Find the new selected row in this new datatable
	        ////			if ( this.Style.Primary_Key_Column >= 0 )
	        ////			{
	        ////				DataRow[] selected = sourceTable.Select( this.sourceTable.Columns[ this.Style.Primary_Key_Column ].ColumnName + " = " + this.current_selected_key );
	        ////				if ( selected.Length > 0 )
	        ////				{
	        ////					current_dataRow = selected[0];
	        ////				}
	        ////				else
	        ////				{
	        ////					current_selected_key = -1;
	        ////					this.current_dataRow = null;
	        ////				}
	        ////			}
	        ////			else
	        ////			{
	        ////				current_selected_key = -1;
	        ////				current_dataRow = null;
	        ////			}
	    }

	    /// <summary> Selects a row and scrolls to it, if there is a match to
	    /// the expression provided. </summary>
	    /// <param name="filterExpression"> The criteria to use to select the row  </param>
	    public void Select_Row( string filterExpression )
	    {
	        // Unselect the last row now
	        selected_row_numbers.Clear();

	        // Select the matching row(s) from the data table
	        DataRow[] selected = sourceView.RowFilter.Length > 0 ? sourceTable.Select( filterExpression + " AND " + sourceView.RowFilter ) : sourceTable.Select( filterExpression );

	        // Move selected into ArrayList
	        ArrayList selectedRowList = new ArrayList( selected );

	        // Select all the matching rows
	        if ( selected.Length > 0 )
	        {
	            // Step through the current dataview to find this row
	            for( int i = 0 ; i < sourceView.Count; i++ )
	            {
	                // Is this a match?
	                if ( selectedRowList.Contains( sourceView[i].Row ))
	                {
	                    selected_row_numbers[ i ] = i;
	                }
	            }
			
	            // Scroll to the first one
	            int natural_row_location = style.Row_Height * Convert.ToInt32(selected_row_numbers.GetByIndex(0));
	            int scroll_bar_max_limitor = ( vScrollBar1.Maximum - dataPanel.Height + style.Header_Height );

	            if (( natural_row_location ) > ( scroll_bar_max_limitor ))
	            {
	                if (( natural_row_location - dataPanel.Height + style.Header_Height ) > 0 )
	                    vScrollBar1.Value = ( natural_row_location ) - dataPanel.Height + style.Header_Height;
	                else
	                    vScrollBar1.Value = 0;
	            }
	            else
	                vScrollBar1.Value = ( natural_row_location );
	        }

	        // Invalidate this panel to redraw it
	        ReDraw();
	    }



			
	    #endregion

	    /// <summary> Build list of visible column indexes </summary>
		/// <remarks> This is used since columns can be hidden from view using
		/// the <see cref="CustomGrid_ColumnStyle"/> object. </remarks>
		private void create_visible_column_index_list()
		{
			// Only continue if the sourcetable has a value
			if ( sourceTable != null )
			{
				// Create new integer array 
				visibleColumnIndexes = new int[ style.Visible_Columns.Count ];

				// Populate this list
			    for( int i = 0 ; i < visibleColumnIndexes.Length ; i++ )
				{
				    DataColumn thisColumn = sourceTable.Columns[ style.Visible_Columns[i].Mapping_Name ];
				    if ( thisColumn != null )
						visibleColumnIndexes[i] = thisColumn.Ordinal;
				}
			}
		}

	    /// <summary> Method fires the <see cref="Selection_Changed" /> event for this panel </summary>
        /// <param name="rows"> Rows currently selected </param>
		protected void OnNewRowSelected( DataRow[] rows )
		{
			// Fire the event
			if ( Selection_Changed != null )
			{
				Selection_Changed( rows );
			}
		}

        /// <summary> Method fires the <see cref="Double_Clicked" /> event for this panel </summary>
        /// <param name="thisRow"> Row double clicked </param>
		protected void OnDoubleClick( DataRow thisRow )
		{
			// Fire the event
			if ( Double_Clicked != null )
			{
				Double_Clicked( thisRow );
			}
		}

        /// <summary> Method fires the <see cref="Clipboard_Copy_Requested" /> event for this panel </summary>
        public void OnClipboardCopyRequested()
        {
            // Fire the event
            if (Clipboard_Copy_Requested != null)
            {
                Clipboard_Copy_Requested(Selected_Row);
            }
        }

		private void CustomGrid_Panel_Enter(object sender, EventArgs e)
		{
			dataPanel.Focus();
		}

		/// <summary> Creates an export copy of the database in Excel </summary>
        /// <param name="output_file"> Output file name </param>
		/// <param name="title"> Title to use for the resulting excel file </param>
        /// <param name="worksheet_name"> Name of the worksheet </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
		public bool Export_as_Excel( string output_file, string title, string worksheet_name )
		{
            // Now, output to MS Excel
            try
            {
                // Create the excel file and worksheet
                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheet excelSheet = excelFile.Worksheets.Add( worksheet_name );
                excelFile.Worksheets.ActiveWorksheet = excelSheet;

                // Create the header cell style
                CellStyle headerStyle = new CellStyle {HorizontalAlignment = HorizontalAlignmentStyle.Left};
                headerStyle.FillPattern.SetSolid(Color.Yellow);
                headerStyle.Font.Weight = ExcelFont.BoldWeight;
                headerStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                // Create the title cell style
                CellStyle titleStyle = new CellStyle
                                           {
                                               HorizontalAlignment = HorizontalAlignmentStyle.Left,
                                               Font = {Weight = ExcelFont.BoldWeight, Size = 14*20}
                                           };

                // Set the default style
                CellStyle defaultStyle = new CellStyle();
                defaultStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                // Add the title
                excelSheet.Cells[0, 0].Value = title;
                excelSheet.Cells[0, 0].Style = titleStyle;

                // Add the header values
                for (int i = 0; i < visibleColumnIndexes.Length; i++)
                {
                    excelSheet.Cells[2, i].Value = style.Column_Styles[visibleColumnIndexes[i]].Header_Text;
                    excelSheet.Cells[2, i].Style = headerStyle;
                }

                // Add each piece of data
                int rowNumber = 3;
                foreach (DataRowView thisRow in sourceView)
                {
                    // Add each cell
                    for (int i = 0; i < visibleColumnIndexes.Length; i++)
                    {
                        if (!thisRow[visibleColumnIndexes[i]].Equals(DBNull.Value))
                            excelSheet.Cells[rowNumber, i].Value = thisRow[visibleColumnIndexes[i]].ToString();
                        //else
                        //    excelSheet.Cells[rowNumber, i].Value = String.Empty;
                        excelSheet.Cells[rowNumber, i].Style = defaultStyle;
                    }

                    // Go to next row
                    rowNumber++;
                }

                // Set the correct widths on all the columns
                for (int i = 0; i < visibleColumnIndexes.Length; i++)
                {
                    int colWidth = style.Column_Styles[visibleColumnIndexes[i]].Width / 6;
                    if (colWidth < 0)
                        colWidth = 0;
                    if (colWidth > 255)
                        colWidth = 255;
                    excelSheet.Columns[i].Width = colWidth * 256;                
                }

                // Get the final end range for the columns
                char endRange = (char)(64 + visibleColumnIndexes.Length);

                // Set the border
                excelSheet.Cells.GetSubrange("A3", endRange + rowNumber.ToString()).SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                excelSheet.Cells.GetSubrange("A3", endRange + "3").SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
   
                // Save the file
                if (output_file.ToUpper().IndexOf(".XLSX") > 0)
                {
                    excelFile.SaveXlsx(output_file);
                }
                else
                {
                    excelFile.SaveXls(output_file);
                }

                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show("Error while outputing Excel Worksheet.\n\n" + ee.Message, "Excel Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }			
		}

        /// <summary> Export the values in the custom grid as a text file using the provided delimiter character (usually a comma or tab) </summary>
        /// <param name="output_file"> Name of the file to output this text into </param>
        /// <param name="delimiter"> Character to use as a delimiter between seperate values in the same row </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Export_as_Text(string output_file, char delimiter)
        {
            // Now, output to TEXT
            try
            {
                // Create the stream
                StreamWriter writer = new StreamWriter(output_file);

                // Add the header values
                for (int i = 0; i < visibleColumnIndexes.Length; i++)
                {
                    // Add the delimiter, as necessary
                    if (i > 0)
                        writer.Write(delimiter);

                    // Add this header information
                    string header_value = style.Column_Styles[visibleColumnIndexes[i]].Header_Text.Replace("\"", "'");
                    if (header_value.IndexOf(delimiter) >= 0)
                        writer.Write("\"" + header_value + "\"");
                    else
                        writer.Write(header_value);
                }
                writer.WriteLine();

                // Now, add all the data
                foreach (DataRowView thisRow in sourceView)
                {
                    // Add each cell
                    for (int i = 0; i < visibleColumnIndexes.Length; i++)
                    {
                        // Add the delimiter, as necessary
                        if (i > 0)
                            writer.Write(delimiter);

                        if (!thisRow[visibleColumnIndexes[i]].Equals(DBNull.Value))
                        {
                            // Add this header information
                            string data_value = thisRow[visibleColumnIndexes[i]].ToString().Replace("\"", "'");
                            if (data_value.IndexOf(delimiter) >= 0)
                                writer.Write("\"" + data_value + "\"");
                            else
                                writer.Write(data_value);
                        }
                    }

                    // Go to next row
                    writer.WriteLine();
                }

                writer.Flush();
                writer.Close();
                return true;

            }
            catch (Exception ee)
            {
                MessageBox.Show("Error while exporting data.\n\n         \n\n" + ee, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            // Is every row already selected?
            if (selected_row_numbers.Count == sourceView.Count)
            {
                // Unselect the last row now
                selected_row_numbers.Clear();
            }
            else
            {
                // Unselect the last row now
                selected_row_numbers.Clear();

                // Step through the current dataview to find this row
                for (int i = 0; i < sourceView.Count; i++)
                {
                    selected_row_numbers[i] = i;
                }
            }

            // Invalidate this panel to redraw it
            ReDraw();
        }

	    #region Component Designer generated code

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
	        this.dataPanel = new DLC.Custom_Grid.Custom_Panel_w_Scroll_Events();
	        this.selectAllButton = new System.Windows.Forms.Button();
	        this.innerRowPanel = new System.Windows.Forms.Panel();
	        this.headerPanel = new System.Windows.Forms.Panel();
	        this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
	        this.dataPanel.SuspendLayout();
	        this.headerPanel.SuspendLayout();
	        this.SuspendLayout();
	        // 
	        // dataPanel
	        // 
	        this.dataPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	                                                                       | System.Windows.Forms.AnchorStyles.Left)
	                                                                      | System.Windows.Forms.AnchorStyles.Right)));
	        this.dataPanel.AutoScroll = true;
	        this.dataPanel.AutoScrollHPos = 0;
	        this.dataPanel.AutoScrollVPos = 0;
	        this.dataPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
	        this.dataPanel.Controls.Add(this.innerRowPanel);
	        this.dataPanel.Location = new System.Drawing.Point(0, 32);
	        this.dataPanel.Name = "dataPanel";
	        this.dataPanel.Size = new System.Drawing.Size(712, 272);
	        this.dataPanel.TabIndex = 1;
	        this.dataPanel.HorizontalScrollValueChanged += new System.Windows.Forms.ScrollEventHandler(this.dataPanel_HorizontallyScrolled);
	        this.dataPanel.Vertical_Scroll_Requested += new DLC.Custom_Grid.Vertical_Scroll_Requested_Delegate(this.dataPanel_Vertical_Scroll_Requested);
	        this.dataPanel.VerticalScrollValueChanged += new System.Windows.Forms.ScrollEventHandler(this.dataPanel_VerticalScrollValueChanged);
	        this.dataPanel.SizeChanged += new System.EventHandler(this.dataPanel_SizeChanged);
	        // 
	        // selectAllButton
	        // 
	        this.selectAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
	        this.selectAllButton.Location = new System.Drawing.Point(0, 0);
	        this.selectAllButton.Name = "selectAllButton";
	        this.selectAllButton.Size = new System.Drawing.Size(75, 32);
	        this.selectAllButton.TabIndex = 1;
	        this.selectAllButton.UseVisualStyleBackColor = true;
	        this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
	        // 
	        // innerRowPanel
	        // 
	        this.innerRowPanel.Location = new System.Drawing.Point(0, 0);
	        this.innerRowPanel.Name = "innerRowPanel";
	        this.innerRowPanel.Size = new System.Drawing.Size(500, 100);
	        this.innerRowPanel.TabIndex = 0;
	        this.innerRowPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.innerRowPanel_Paint);
	        this.innerRowPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.innerRowPanel_MouseDown);
	        // 
	        // headerPanel
	        // 
	        this.headerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
	                                                                        | System.Windows.Forms.AnchorStyles.Right)));
	        this.headerPanel.Controls.Add(this.selectAllButton);
	        this.headerPanel.Location = new System.Drawing.Point(0, 0);
	        this.headerPanel.Name = "headerPanel";
	        this.headerPanel.Size = new System.Drawing.Size(712, 32);
	        this.headerPanel.TabIndex = 2;
	        this.headerPanel.MouseLeave += new System.EventHandler(this.headerPanel_MouseLeave);
	        this.headerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.headerPanel_Paint);
	        this.headerPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.headerPanel_MouseMove);
	        this.headerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.headerPanel_MouseDown);
	        this.headerPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.headerPanel_MouseUp);
	        // 
	        // vScrollBar1
	        // 
	        this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	                                                                        | System.Windows.Forms.AnchorStyles.Right)));
	        this.vScrollBar1.Location = new System.Drawing.Point(695, 32);
	        this.vScrollBar1.Name = "vScrollBar1";
	        this.vScrollBar1.Size = new System.Drawing.Size(17, 272);
	        this.vScrollBar1.TabIndex = 1;
	        this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
	        // 
	        // CustomGrid_Panel
	        // 
	        this.Controls.Add(this.vScrollBar1);
	        this.Controls.Add(this.headerPanel);
	        this.Controls.Add(this.dataPanel);
	        this.Name = "CustomGrid_Panel";
	        this.Size = new System.Drawing.Size(712, 304);
	        this.Enter += new System.EventHandler(this.CustomGrid_Panel_Enter);
	        this.dataPanel.ResumeLayout(false);
	        this.headerPanel.ResumeLayout(false);
	        this.ResumeLayout(false);

	    }
	    #endregion

	    #region InnerRowPanel - Related Methods (panel which actually holds the data rows)

	    #region Paint Method for the rows in this panel

	    private void innerRowPanel_Paint(object sender, PaintEventArgs e)
	    {
	        // Only continue if there is a current sourceView
	        if ( sourceView != null )
	        {
	            // Visible columns may have changed, so redetermine that
	            create_visible_column_index_list();

	            // If the header panel size does not match the style object,
	            // then resize both panels
	            if ( headerPanel.Height != style.Header_Height )
	            {
	                // Resize the header panel
	                headerPanel.Height = style.Header_Height;

	                // Resize the data panel
	                dataPanel.Height = Height - style.Header_Height;
	                dataPanel.Location = new Point( 0, style.Header_Height );

	                // Resize the vertical scroll bar
	                vScrollBar1.Location = new Point( Width - 17, style.Header_Height + 1 );
	                vScrollBar1.Height = Height - style.Header_Height - 1;
	            }
	            else
	            {
	                headerPanel.Invalidate();
	            }

	            try
	            {
	                // If there are no matches, show the text
	                if ( sourceView.Count == 0 )
	                {
	                    // Find the spot to show the NO MATCHES text
	                    int y = (dataPanel.Height / 2) - 20;
	                    int x = (dataPanel.Width / 2) - 130;

	                    // Create the font and brush
	                    Font noMatchFont = new Font("Tahoma", 18, FontStyle.Bold );
	                    Brush noMatchBrush = new SolidBrush( style.No_Matches_Text_Color );

	                    // Resize the inner row panel to allow this to be seen
	                    innerRowPanel.Height = dataPanel.Height;

	                    // Draw the string
	                    e.Graphics.DrawString( style.No_Matches_Text, noMatchFont, noMatchBrush, x, y );
	                }
	                else
	                {
	                    // Get out the graphics object and create the pen and brushes needed
	                    Graphics g = e.Graphics;
	                    Pen gridColorPen = new Pen( style.Grid_Line_Color, 1 );
	                    Brush rowSelectBackBrush = new SolidBrush( style.Row_Select_Button_Back_Color );

	                    // Set the height of the panel, based on the number of rows
	                    if (( (sourceView.Count * style.Row_Height ) < dataPanel.Height ) &&
	                        ( innerRowPanel.Height != ( sourceView.Count * style.Row_Height ) + 1 ))
	                    {
	                        innerRowPanel.Height = ( sourceView.Count * style.Row_Height ) + 1;
	                    }
	                    if ( (sourceView.Count * style.Row_Height ) >= dataPanel.Height )
	                    {
	                        innerRowPanel.Height = dataPanel.Height + style.Row_Height;
	                    }

	                    // Compute start of rows
	                    int start_row = ( vScrollBar1.Value ) / style.Row_Height;
	                    if ( start_row < 0 )
	                        start_row = 0;
	                    if ( start_row >= 1 )
	                        start_row = start_row - 1;

	                    // Compute the last row
	                    int last_row = start_row + ( innerRowPanel.Height / style.Row_Height ) + 1;

	                    // Convert the row numbers to Y, Height
	                    int last_y = last_row * style.Row_Height;
	                    int height = (last_row - start_row) * style.Row_Height;
	                    int start_y = start_row * style.Row_Height;

	                    // Color the columns on the panel
	                    int currentX = style.Row_Select_Button_Width;
	                    foreach (int t in visibleColumnIndexes)
	                    {
                            // Fill the column
	                        g.FillRectangle( new SolidBrush( style.Column_Styles[ t ].BackColor ), currentX, start_y - vScrollBar1.Value, style.Column_Styles[ t ].Width, height );
	                        currentX += style.Column_Styles[ t ].Width;
	                    }

	                    // Save the total width
	                    totalWidth = currentX;

	                    // Draw the selected row, if there is one
	                    if (selectable)
	                    {
	                        foreach (int rowNumber in selected_row_numbers.Keys)
	                        {
	                            g.FillRectangle(new SolidBrush(style.Selection_Color), 0, rowNumber * style.Row_Height - vScrollBar1.Value, currentX, style.Row_Height);
	                        }
	                    }

	                    //Draw the vertical lines on the panel
	                    currentX = style.Row_Select_Button_Width;
	                    g.DrawLine( gridColorPen, currentX, start_y - vScrollBar1.Value, currentX, last_y - vScrollBar1.Value );
	                    foreach (int t in visibleColumnIndexes)
	                    {
                            // Add the width to the running total and draw the line
	                        currentX += style.Column_Styles[ t ].Width;
	                        g.DrawLine( gridColorPen, currentX, start_y - vScrollBar1.Value, currentX, last_y - vScrollBar1.Value );
	                    }

	                    // Draw all the horizontal lines next, starting with the first, and add
	                    // each row select button
	                    g.DrawLine( gridColorPen, 0, 0, currentX, 0 );
	                    int currentY = start_row * style.Row_Height - vScrollBar1.Value;
	                    for( int i = start_row ; i < last_row ; i++ )
	                    {
	                        // Draw the select button
	                        g.FillRectangle( rowSelectBackBrush, 0, currentY, style.Row_Select_Button_Width, style.Row_Height );

	                        // Draw the horizontal line
	                        g.DrawLine( gridColorPen, 0, currentY, totalWidth, currentY );

	                        // Go to the next row
	                        currentY += style.Row_Height;
	                    }

	                    // Draw the arrow for the currently selected row, if there is one
	                    if (( selectable ) && ( selected_row_numbers.Count == 1 ))
	                    {
	                        foreach( int thisSelectedRow in selected_row_numbers.Keys )
	                        {
	                            int selected_y = thisSelectedRow * style.Row_Height - vScrollBar1.Value;
	                            g.FillPolygon( new SolidBrush( style.Row_Select_Button_Fore_Color ), 
	                                           new[] { new Point( 5, selected_y + 7 ), new Point( 18, selected_y + 7 ),
	                                                   new Point( 18, selected_y + 2), new Point( 28, selected_y + 10 ), 
	                                                   new Point( 18, selected_y + 18 ), new Point( 18, selected_y + 14),
	                                                   new Point( 5, selected_y + 14 ) } );
	                        }
	                    }

	                    // Add the text for each row of data, but only the data which is shown
	                    Font textFont = new Font( "Tahoma", 8.25F );
	                    Brush blackBrush =new SolidBrush( Color.Black );
	                    currentY = 5 + ( start_row * style.Row_Height ) - vScrollBar1.Value;
	                    currentX = style.Row_Select_Button_Width;
	                    for( int thisRow = start_row ; thisRow < last_row ; thisRow++ )
	                    {
	                        // If this row is past the end of the dataview, break
	                        if ( thisRow >= sourceView.Count )
	                            break;

	                        // Add each field from this row
	                        foreach (int t in visibleColumnIndexes)
	                        {
	                            if ((sourceView[thisRow][t] is DateTime) && (style.Column_Styles[t].Short_Date_Format ))
	                            {
	                                g.DrawString( ((DateTime)sourceView[thisRow][t]).ToShortDateString(), textFont, blackBrush, new RectangleF(currentX + 1, currentY, style.Column_Styles[t].Width, style.Row_Height - 5));
	                            }
	                            else
	                            {
	                                g.DrawString(sourceView[thisRow][t].ToString(), textFont, blackBrush, new RectangleF(currentX + 1, currentY, style.Column_Styles[t].Width, style.Row_Height - 5));
	                            }
	                            currentX += style.Column_Styles[ t ].Width;
	                        }

	                        // Move down to the next row 
	                        currentY += style.Row_Height;
	                        currentX = style.Row_Select_Button_Width;
	                    }
	                }
	            }
	            catch 
	            {
	                // DLC.Tools.Forms.ErrorMessageBox.Show("Error encountered while painting the inner row panel", "Custom Grid Error", ee);
	            }
	        }
	    }

	    #endregion

	    #region Mouse Event Handlers

	    private void innerRowPanel_MouseDown(object sender, MouseEventArgs e)
	    {
	        dataPanel.Focus();

	        bool shift = Keyboard.Keyboard.IsKeyPressed( Keyboard.Keyboard.VirtualKeyStates.VK_SHIFT );
	        bool control = Keyboard.Keyboard.IsKeyPressed( Keyboard.Keyboard.VirtualKeyStates.VK_CONTROL );

	        // Get the row number
	        int newRow = ( e.Y + vScrollBar1.Value ) / style.Row_Height;
	        if (( newRow < 0 ) || ( newRow >= sourceView.Count ))
	        {
	            return;
	        }

	        // Save the old last clicked, and set this as the new
	        int old_last_click = last_clicked_row;
	        last_clicked_row = newRow;
			
	        // Save the old selected row numbers, if we aren't keeping them
	        ArrayList new_selects = new ArrayList();
	        SortedList deselect_rows = new SortedList();
	        if ( e.Button != MouseButtons.Right )
	        {
	            if (( shift ) || ( control ))
	            {
	                if ( shift )
	                {
	                    // Build the range of selected rows between last clicked and this
	                    deselect_rows = (SortedList) selected_row_numbers.Clone();
	                    if ( last_clicked_row < 0 )
	                    {
	                        new_selects.Add( newRow );
	                    }
	                    else
	                    {
	                        // Build the range of selected rows
	                        if ( newRow > old_last_click )
	                        {
	                            for( int j = newRow ; j >= old_last_click ; j-- )
	                            {
	                                new_selects.Add( j );
	                            }
	                        }
	                        else
	                        {
	                            for( int j = newRow ; j <= old_last_click ; j++ )
	                            {
	                                new_selects.Add( j );
	                            }
	                        }
	                    }
	                }
	                else
	                {
	                    // Did they click on a row already selected?
	                    if ( selected_row_numbers.Contains( newRow ))
	                    {
	                        // Need to DESELET this row
	                        deselect_rows[ newRow ] = newRow;
	                    }
	                    else
	                    {
	                        new_selects.Add( newRow );
	                    }
	                }
	            }
	            else
	            {
	                deselect_rows = (SortedList) selected_row_numbers.Clone();
	                new_selects.Add( newRow );
	            }
	        }

	        // If this row is already selected, see if this could be a double
	        // click, or if it should be unselected
	        if ( selected_row_numbers.Contains( newRow ) )
	        {
	            // Is the timer for double click running?
	            if ( doubleClickTimer.Enabled )
	            {
	                if ( e.Button == MouseButtons.Left )
	                {
	                    // Fire the double click event
	                    OnDoubleClick( sourceView[ newRow ].Row );
	                }
	            }
	            else
	            {
	                // Must be LEFT button to unselect
	                if ( e.Button == MouseButtons.Left )
	                {
	                    deselect_rows[ newRow ] = newRow;
	                    OnNewRowSelected( null );
	                }
	            }
	        }

	        // Remove deselected rows from the selected rows arraylist
	        foreach( int row in deselect_rows.Keys )
	        {
	            if ( selected_row_numbers.Contains( row ))
	                selected_row_numbers.Remove( row );	
	        }

	        // Add all newly selected row to the selected rows 
	        foreach( int row in new_selects )
	        {
	            if ( !selected_row_numbers.Contains( row ))
	                selected_row_numbers[ row ] = row;	
	        }

	        // First, set the new number, then invalidate the old and the new locations
	        // Invalidate all the old rows that are now deselected
	        foreach( int row in deselect_rows.Keys )
	        {
	            innerRowPanel.Invalidate( new Rectangle( 0, (row*style.Row_Height) - vScrollBar1.Value, totalWidth, style.Row_Height) );	
	        }

	        // Invalidate all the newly selected rows
	        foreach( int row in new_selects )
	        {
	            innerRowPanel.Invalidate( new Rectangle( 0, (row*style.Row_Height) - vScrollBar1.Value,  totalWidth, style.Row_Height) );		
	        }

	        // Invalidate the arrow rectangle
	        innerRowPanel.Invalidate( new Rectangle( 0, 0, style.Row_Select_Button_Width, innerRowPanel.Height ));

	        // Start the timer to check for double click
	        doubleClickTimer.Interval = style.Double_Click_Delay;
	        doubleClickTimer.Start();		

	        DataRow[] returnValue = new DataRow[selected_row_numbers.Count];
	        for( int i = 0 ; i < selected_row_numbers.Count ; i++ )
	        {
	            if ( Convert.ToInt32( selected_row_numbers.GetByIndex(i) ) >= sourceView.Count )
	            {
	                returnValue = null;
	                break;
	            }

                // else..
	            returnValue[i] = sourceView[ Convert.ToInt32( selected_row_numbers.GetByIndex(i) )].Row;
	        }

	        OnNewRowSelected( returnValue );


	    }

	    private void double_click_timer_Tick(object sender, EventArgs e)
	    {
	        // Let this tick once, and then stop the timer
	        doubleClickTimer.Stop();
	    }

	    #endregion

	    #endregion

	    #region Header Related Methods

	    #region Mouse Event Handlers

	    private void headerPanel_MouseDown(object sender, MouseEventArgs e)
	    {
	        // Determine if this is for a sort, or for a resize...
	        if ( Cursor == Cursors.VSplit )
	        {
	            // Which column is being resized?
	            columnBeingResized = -1;
	            int current_x = style.Row_Select_Button_Width - dataPanel.AutoScrollHPos;
	            foreach (int t in visibleColumnIndexes)
	            {
                    // Go to next column
	                current_x += style.Column_Styles[ t ].Width;

	                // Is the mouse close here?
	                if ( Math.Abs( current_x - e.X ) <= 10 )
	                {
	                    columnBeingResized = t;
	                    break;
	                }
	            }

	            // Also save the point, so it can be used to determine the new column size
	            columnResizeStart = e.X;
	        }
	        else
	        {
	            // This is for a re-sort, so get the column to resort
	            int new_sort_column = determine_mouse_down_column( e.X );
	            if ( new_sort_column < 0 )
	                return;

	            // If this is a different column, get ascending
	            string sort;
	            if ( sortColumn != new_sort_column )
	            {
	                sort = style.Column_Styles[ new_sort_column ].Ascending_Sort;
	                sortAsc = true;
	            }
	            else
	            {
	                // Already sorted on this column, so do reverse of current sort
	                if ( sortAsc )
	                {
	                    sort = style.Column_Styles[ new_sort_column ].Descending_Sort;
	                    sortAsc = false;
	                }
	                else
	                {
	                    sort = style.Column_Styles[ new_sort_column ].Ascending_Sort;
	                    sortAsc = true;
	                }
	            }

	            // Save this new sort column
	            sortColumn = new_sort_column;

	            // Apply this sort
	            sourceView.Sort = sort;

	            // Deselect the currently selected row
	            selected_row_numbers.Clear();

	            // Redraw the data and the header (for new arrow)
	            innerRowPanel.Invalidate();
	            headerPanel.Invalidate();

	            // Set the focus back on the rows
	            innerRowPanel.Focus();
	        }
	    }

	    private int determine_mouse_down_column( int x )
	    {
	        // Step through each column
	        int current_x = style.Row_Select_Button_Width - dataPanel.AutoScrollHPos;
	        foreach (int t in visibleColumnIndexes)
	        {
                // Is the mouse in this column?
	            if (( x > current_x ) && ( x < ( current_x + style.Column_Styles[ t ].Width )))
	            {
	                return t;
	            }

	            // Go to next column
	            current_x += style.Column_Styles[ t ].Width;
	        }

	        // Not currently on any column
	        return -1;
	    }

	    private void headerPanel_MouseMove(object sender, MouseEventArgs e)
	    {
	        // If you can't resize the columns (or there are no columns), return
	        if (( !style.Columns_Resizable ) || ( style.Column_Styles.Count == 0 ))
	            return;


	        // Is a column currently being resized?
	        if ( columnBeingResized >= 0 )
	        {
	            // Determine the amount to change, and change the column width accordingly
	            int change = e.X - columnResizeStart;
	            style.Column_Styles[ columnBeingResized ].Width = style.Column_Styles[ columnBeingResized ].Width + change;
	            columnResizeStart = e.X;

	            // Minimum width is -5 though
	            if ( style.Column_Styles[ columnBeingResized ].Width < 5 )
	                style.Column_Styles[ columnBeingResized ].Width = 5;

	            // Make the header redraw
	            headerPanel.Invalidate();

	            // Determine the new total width
	            totalWidth = style.Row_Select_Button_Width;
	            foreach (int t in visibleColumnIndexes)
	            {
                    // Add the width for each visible column
	                totalWidth += style.Column_Styles[ t ].Width;
	            }

	            // Resize the panel
	            innerRowPanel.Width = totalWidth + 5;

	            // Redraw the rows as well?
	            innerRowPanel.Invalidate();
	        }
	        else
	        {
	            // Is this near the edge of one of the columns?
	            bool on_grid_line = false;
	            int current_x = style.Row_Select_Button_Width - dataPanel.AutoScrollHPos;
	            foreach (int t in visibleColumnIndexes)
	            {
                     // Go to next column
	                current_x += style.Column_Styles[ t ].Width;

	                // Is the mouse close here?
	                if ( Math.Abs( current_x - e.X ) <= 10 )
	                {
	                    on_grid_line = true;
	                    break;
	                }
	            }
				
				
	            Cursor = on_grid_line ? Cursors.VSplit : Cursors.Default;
	        }
	    }

	    private void headerPanel_MouseLeave(object sender, EventArgs e)
	    {
	        // Just set the mouse back to the default
	        Cursor = Cursors.Default;
	        columnBeingResized = -1;
	    }

	    private void headerPanel_MouseUp(object sender, MouseEventArgs e)
	    {
	        columnBeingResized = -1;
	    }

	    #endregion

	    #region Methods related to painting the header 

	    private void headerPanel_Paint(object sender, PaintEventArgs e)
	    {
	        drawHeader( e.Graphics );
	    }

	    private void drawHeader( Graphics g )
	    {
	        if (( style == null ) || ( visibleColumnIndexes == null ))
	            return;

	        // Check size of the button versus the style
	        if (selectAllButton.Width != style.Row_Select_Button_Width + 1)
	            selectAllButton.Width = style.Row_Select_Button_Width + 1;
	        if (selectAllButton.Height != style.Header_Height)
	            selectAllButton.Height = style.Header_Height;


	        // Create the brush and pen for the header
	        Brush headerBrush = new SolidBrush( style.Header_Back_Color );
	        Brush headerForeBrush = new SolidBrush( style.Header_Fore_Color );
	        Pen headerPen = new Pen( style.Header_Fore_Color, 1 );
	        Pen outerPen = new Pen( style.Grid_Line_Color, 1 );

	        // Determine the spot to start this text
	        int y_text_loc = ((style.Header_Height - headerFont.Height ) / 2);
			
	        // Step through each column which is visible
	        int current_x = dataPanel.DisplayRectangle.X + style.Row_Select_Button_Width + 1;
	        foreach (int t in visibleColumnIndexes)
	        {
                // Fill and then draw the rectangle
	            g.FillRectangle( headerBrush, current_x, 0, style.Column_Styles[ t ].Width, style.Header_Height );
	            g.DrawRectangle( headerPen, current_x, 0, style.Column_Styles[ t ].Width, style.Header_Height );

	            // If this row is current sort, then need to leave room for the sort-triangle, and draw it
	            if ( t == sortColumn )
	            {
	                // Determine the triangle size
	                int triangle_size = (int) ( 0.5F * style.Header_Height );

	                // Add the text to this rectangle
	                g.DrawString( style.Column_Styles[ t].Header_Text, headerFont, headerForeBrush, new Rectangle( current_x + 5, y_text_loc, (int) (style.Column_Styles[ t].Width - (1.8F * triangle_size)), (int) headerFont.SizeInPoints + 5) );

	                // Draw the triangle
	                int triangle_start = (int) (current_x + style.Column_Styles[ t ].Width - (1.5F * triangle_size));
	                if ( sortAsc )
	                {
	                    g.FillPolygon(headerPen.Brush,
	                                  new[] { new Point( triangle_start, (int) (.30F * style.Header_Height )), new Point( (int) (triangle_start + (0.5F * triangle_size)), (int) (.70F * style.Header_Height )),
	                                          new Point( triangle_start + triangle_size, (int) (.30F * style.Header_Height )) } );
	                }
	                else
	                {
	                    g.FillPolygon( headerPen.Brush,
	                                   new[] { new Point( triangle_start, (int) (.70F * style.Header_Height )), new Point( (int) (triangle_start + (0.5F * triangle_size)), (int) (.30F * style.Header_Height )),
	                                           new Point( triangle_start + triangle_size, (int) (.70F * style.Header_Height )) } );
	                }
	            }
	            else
	            {
	                // Add the text to this rectangle
	                g.DrawString( style.Column_Styles[ t].Header_Text, headerFont, headerForeBrush, new Rectangle( current_x + 5,  y_text_loc, style.Column_Styles[ t].Width, (int) headerFont.SizeInPoints + 5 ) );
	            }

	            // Prepare for the next rectangle
	            current_x = current_x + style.Column_Styles[ t ].Width;
	        }

	        // Draw a line above and to the right of the header row
	        g.DrawLine( outerPen, dataPanel.DisplayRectangle.X + style.Row_Select_Button_Width + 1, 0, dataPanel.DisplayRectangle.X + style.Row_Select_Button_Width + 1, style.Header_Height );
	        g.DrawLine( outerPen, dataPanel.DisplayRectangle.X + style.Row_Select_Button_Width + 1, 0, current_x, 0 );
	        g.DrawLine( outerPen, current_x, 0, current_x, style.Header_Height );
	    }

	    #endregion

	    #endregion

	    #region Data Panel Event Handlers (Outer panel which holds the 'innerRowPanel')

	    private void dataPanel_VerticalScrollValueChanged(object sender, ScrollEventArgs e)
	    {
	        dataPanel.Invalidate();
	    }

	    private void dataPanel_HorizontallyScrolled(object sender, ScrollEventArgs e)
	    {
	        // If this horizontal scroll is over, redraw the header
	        if (( e.Type == ScrollEventType.EndScroll ) || ( e.Type == ScrollEventType.LargeDecrement ) || ( e.Type == ScrollEventType.LargeIncrement ))
	        {
	            headerPanel.Invalidate();
	        }
	    }

	    private void dataPanel_SizeChanged(object sender, EventArgs e)
	    {
	        // Redraw the header
	        headerPanel.Invalidate();

	        // Configure the vertical scroll bar
	        config_vscrollbar();

	        // Continue only if there are visible columns
	        if ( visibleColumnIndexes == null )
	            return;

	        // Determine the new total width
	        totalWidth = style.Row_Select_Button_Width;
	        foreach (int t in visibleColumnIndexes)
	        {
                // Add the width for each visible column
	            totalWidth += style.Column_Styles[ t ].Width;
	        }

	        // Resize the panel
	        innerRowPanel.Width = totalWidth + 5;

	        innerRowPanel.Invalidate();
	    }

	    #endregion

	    #region Vertical Scroll Bar Code

	    private void config_vscrollbar()
	    {
	        // Determine if the scroll bar should be visible
	        if (( sourceView != null ) && ( style != null ))
	        {
	            // See if the vertical scroll bar should appear
	            if (( sourceView.Count * style.Row_Height ) > ( dataPanel.Height ))
	            {
	                // Show the scroll bar
	                vScrollBar1.Show();
	                vScrollBar1.Maximum = ( sourceView.Count + 1 ) * style.Row_Height + 3;
	                vScrollBar1.LargeChange = Height;

	                // If the horizontal scroll bar is visible, then one more row must be
	                // added to the max value for the vertical scroll bar
	                if ( innerRowPanel.Width > dataPanel.Width )
	                    vScrollBar1.Maximum = vScrollBar1.Maximum + 16;
	            }
	            else
	            {
	                vScrollBar1.Hide();
	                vScrollBar1.Value = 0;
	            }		
	        }
	    }

	    private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
	    {
	        innerRowPanel.Invalidate();
	    }

	    private void dataPanel_Vertical_Scroll_Requested(int scroll_change)
	    {
	        // This is the same as scroll
	        if ( vScrollBar1.Value + scroll_change < 0 )
	        {
	            // If the value is currently zero, do nothing
	            if ( vScrollBar1.Value != 0 )
	            {
	                vScrollBar1.Value = 0;
	                innerRowPanel.Invalidate();
	            }
	            return;
	        }

	        if ( ( vScrollBar1.Value + scroll_change ) > ( sourceView.Count * style.Row_Height ) - dataPanel.Height )
	        {
	            if ( vScrollBar1.Maximum - dataPanel.Height - style.Row_Height < 0 )
	                vScrollBar1.Value = 0;
	            else
	                vScrollBar1.Value = vScrollBar1.Maximum - dataPanel.Height - style.Row_Height; 
	        }
	        else
	        {
	            vScrollBar1.Value = vScrollBar1.Value + scroll_change;
	        }
				
	        innerRowPanel.Invalidate();
	    }

	    #endregion

    }
}
