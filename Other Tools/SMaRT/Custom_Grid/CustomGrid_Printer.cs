using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace DLC.Custom_Grid
{
	/// <summary> Printer object used to print the rows displayed within a <see cref="CustomGrid_Panel" /> control. </summary>
    /// <remarks> Portions adapted from original DataGridPrinter code written by Michael Gold (copyright 2002)</remarks>
	public class CustomGrid_Printer
	{
		private PrintDocument ThePrintDocument;
		private CustomGrid_Panel  TheGrid;

        private const int kVerticalCellLeeway = 10;

        private ArrayList Lines = new ArrayList();

		
		private int PageWidth;
		private int PageHeight;
		private int TopMargin;
		private int BottomMargin;
		private int LeftMargin;
		private int TotalWidth;
		private int[] columnWidths;

        private int rowCount = 0;
        private int pageNumber = 1;


		/// <summary> Constructor for a DataGridPrinter object which will be used to
		/// print a DataGrid to a printer.  </summary>
		/// <param name="aGrid"> DataGrid whose <see cref="DataGridTableStyle"/> will be used to 
		/// determine the appearance of the printed page.  </param>
		/// <param name="aPrintDocument"> Print document to use while formatting the appearance
		/// of the information on the page </param>
		public CustomGrid_Printer( CustomGrid_Panel aGrid, PrintDocument aPrintDocument )
		{
			// Save the parameters
			TheGrid = aGrid;
			ThePrintDocument = aPrintDocument;

			// Save the default page settings from the print document
////			PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Width;
////			PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Height;
////			TopMargin = ThePrintDocument.DefaultPageSettings.Margins.Top;
////			BottomMargin = ThePrintDocument.DefaultPageSettings.Margins.Bottom;
		}

        /// <summary> Prepares this printer object to begin printing a new grid object </summary>
        public void Prepare_To_Print()
        {
            rowCount = 0;
            pageNumber = 1;
        }

        /// <summary> Increment the page count to prepare to print a subsequent page </summary>
        public void Increment_Page()
        {
            pageNumber++;
        }

		/// <summary> Draw the header for the data grid to the graphical document </summary>
		/// <param name="g"></param>
		private void DrawHeader(Graphics g)
		{
			// form brushes based on the color properties of the DataGrid
			// These brushes will be used to draw the grid borders and cells 
			SolidBrush ForeBrush = new SolidBrush( TheGrid.Style.Header_Fore_Color );
			SolidBrush BackBrush = new SolidBrush( TheGrid.Style.Header_Back_Color );
			Pen TheLinePen = new Pen( TheGrid.Style.Grid_Line_Color, 1 );

			// Create a format for the cell so that the string in the cell is cut off at the end of the column width
			StringFormat cellformat = new StringFormat();
			cellformat.Trimming = StringTrimming.EllipsisCharacter;
			cellformat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

			// Set the initial row count equal to the current row count (?)
			int initialRowCount = rowCount;

			// draw the table header
			float startxposition = LeftMargin;
			RectangleF nextcellbounds = new RectangleF(0,0, 0, 0);

			// Defind the boundaries for the header
			RectangleF HeaderBounds  = new RectangleF(0, 0, 0, 0);
			HeaderBounds.X = LeftMargin;
//			HeaderBounds.Y = TopMargin + (RowCount - initialRowCount) * ( TheDataGrid.Font.SizeInPoints  + kVerticalCellLeeway );
			HeaderBounds.Y = TopMargin;
			HeaderBounds.Height = TheGrid.Font.SizeInPoints + kVerticalCellLeeway;
			HeaderBounds.Width = TotalWidth;

			// Fill the header boundaries with the background color
			g.FillRectangle(BackBrush, HeaderBounds);

			// Step through each column in the table, showing the label foe each one
			for (int k = 0; k < columnWidths.Length; k++)
			{
                if (columnWidths[k] > 0)
				{
					// Draw the reactangle boundary around the section of the header for this column
                    RectangleF cellbounds = new RectangleF(startxposition, TopMargin + (rowCount - initialRowCount) * (TheGrid.Font.SizeInPoints + kVerticalCellLeeway),
						columnWidths[k], 
						TheGrid.Header_Font.SizeInPoints + kVerticalCellLeeway);
					nextcellbounds = cellbounds;

					// Get the name of this column, from the style, and display that header information
					string column_name = TheGrid.Style.Column_Styles[k].Header_Text;
					g.DrawString( column_name, TheGrid.Header_Font, ForeBrush, cellbounds, cellformat);

					// Get the position for the next column to start in the header
					startxposition = startxposition + columnWidths[k];
				}
			}
	
			// Draw final line underneath the data grid's header
			g.DrawLine(TheLinePen, LeftMargin, nextcellbounds.Bottom, TotalWidth + LeftMargin, nextcellbounds.Bottom);
		}

		/// <summary> Prints to the print document the rows which belong on this page </summary>
		/// <param name="g"> Graphics object from the print document used to draw the rows and data </param>
        /// <param name="All_In_View"> Flag indicates if every row in the current view should be printed, or 
        /// only those rows which are selected </param>
		/// <returns> TRUE if there remain more rows after this page, otherwise FALSE </returns>
		private bool DrawRows(Graphics g, bool All_In_View )
		{
			try
			{
				int lastRowBottom = TopMargin; 
				//   Create an array to save the horizontal positions for drawing horizontal gridlines
				ArrayList Lines = new ArrayList();

				// form brushes based on the color properties of the DataGrid
				// These brushes will be used to draw the grid borders and cells 
				SolidBrush ForeBrush = new SolidBrush( TheGrid.Style.ForeColor );
				SolidBrush BackBrush = new SolidBrush( Color.White );
				SolidBrush AlternatingBackBrush = new SolidBrush( Color.Linen );
				Pen TheLinePen = new Pen( TheGrid.Style.Grid_Line_Color, 1 );

				// Create a format for the cell so that the string in the cell is cut off at the end of the column width
				StringFormat cellformat = new StringFormat();
				cellformat.Trimming = StringTrimming.EllipsisCharacter;
				cellformat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

				// set the initial row count, this will start at 0 for the first page, and 
				// be a different value for the 2nd, 3rd, 4th, etc. pages.
                int initialRowCount = rowCount;
				RectangleF RowBounds = new RectangleF(0, 0, 0, 0);

				// Calculate the number of rows
				int row_count = TheGrid.View_Count;
				if ( !All_In_View )
				{
					row_count = TheGrid.Selected_Row_Count;
				}

				// draw the rows of the table 
				DataRow dr = null;
				DataRow[] selectedRows = null;
				if ( !All_In_View )
				{
					selectedRows = TheGrid.Selected_Row;
				}

				for (int i = initialRowCount; i < row_count; i++)
				{
					// get the next DataRow in the DataTable
					if ( All_In_View )
					{
						dr = TheGrid.DataView[i].Row;
					}
					else
					{
						dr = selectedRows[i];
					}

					int startxposition = LeftMargin;

					//  Calculate the row boundary based on teh RowCount and offsets into the page
					RowBounds.X = LeftMargin;
                    RowBounds.Y = TopMargin + ((rowCount - initialRowCount) + 1) * (TheGrid.Font.SizeInPoints + kVerticalCellLeeway);
					RowBounds.Height = TheGrid.Font.SizeInPoints + kVerticalCellLeeway;
					RowBounds.Width = TotalWidth;

					// save the vertical row positions for drawing grid lines
					Lines.Add(RowBounds.Bottom);

					// paint rows differently for alternate row colors
					if (i%2 == 0)
						g.FillRectangle(BackBrush, RowBounds);
					else
						g.FillRectangle(AlternatingBackBrush, RowBounds);

					// Go through each column in the row and draw the information from the DataRow
                    for (int j = 0; j < columnWidths.Length; j++)
					{
						// Only show this column if it has a width
                        if (columnWidths[j] > 0 )
						{
							// Draw the cell for this row and column
							RectangleF cellbounds = new RectangleF(startxposition,
                                TopMargin + ((rowCount - initialRowCount) + 1) * 
								(TheGrid.Font.SizeInPoints + kVerticalCellLeeway),
								columnWidths[j],
								TheGrid.Font.SizeInPoints + kVerticalCellLeeway);

							// Add the string for this cell
							g.DrawString(dr[j].ToString(), TheGrid.Font, ForeBrush, cellbounds, cellformat);
							lastRowBottom = (int)cellbounds.Bottom;

							// increment the column position
							startxposition = startxposition + columnWidths[j];
						}
					}
					
					// Increment the row counter
					rowCount++;

					// when we've reached the bottom of the page, draw the horizontal and vertical grid lines and return true
                    if (rowCount * (TheGrid.Font.SizeInPoints + kVerticalCellLeeway) > (PageHeight * pageNumber))
					{
						DrawHorizontalLines(g, Lines);
						DrawVerticalGridLines(g, lastRowBottom);
						return true; 
					}
				}

				// when we've reached the end of the table (last page), draw the horizontal 
				// and vertical grid lines and return false
				DrawHorizontalLines(g, Lines);
				DrawVerticalGridLines(g, lastRowBottom);
				return false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message.ToString());
				return false;
			}
		}

		/// <summary> Method draws the horizontal lines, given the graphics object and
		/// an arraylist with the pixel locations where the lines needs to be drawn. </summary>
		/// <param name="g"></param>
		/// <param name="lines"> Arraylist contains the location to draw the horizontal lines </param>
		private void DrawHorizontalLines(Graphics g, ArrayList lines)
		{
			// Crete the pen in to match the color of the grid lines in the DataGrid
			Pen TheLinePen = new Pen(TheGrid.Style.Grid_Line_Color, 1);

			// Iterate through the Arraylist of the horizontal locations where the lines need to be drawn
			for (int i = 0;  i < lines.Count; i++)
			{
				// Draw the horizontal line for this row
				g.DrawLine(TheLinePen, LeftMargin, (float)lines[i], TotalWidth + LeftMargin, (float)lines[i]);
			}
		}

		/// <summary> Draws the vertical lines from the data grid </summary>
		/// <param name="g"></param>
		/// <param name="page_bottom"> Location of the bottom of the last row on this page </param>
		private void DrawVerticalGridLines(Graphics g, int page_bottom)
		{
			// Crete the pen in to match the color of the grid lines in the DataGrid
			Pen TheLinePen = new Pen(TheGrid.Style.Grid_Line_Color, 1);
			
			// Step through each column, drawing the vertical lines
			int horiz_position = LeftMargin;
			for (int k = 0; k < TheGrid.Style.Column_Styles.Count; k++)
			{
				// Only draw a vertical line for this column if its width is greater than 3 pixels.
				if (( TheGrid.Style.Column_Styles[k].Visible ) && ( TheGrid.Style.Column_Styles[k].Width > 3 ))
				{
					// Draw this vertical line
					g.DrawLine(TheLinePen, horiz_position, TopMargin, horiz_position, page_bottom);
					horiz_position += columnWidths[k];
				}
			}

			// Draw the last line
			g.DrawLine(TheLinePen, horiz_position, TopMargin, horiz_position, page_bottom);
		}

		private bool DrawAppropriateDataGrid( Graphics g, bool All_In_View )
		{
			// Set the page height and width according to whether the page settings dictate the
			// grid being printed in landscape or portrait.
			if ( ThePrintDocument.DefaultPageSettings.Landscape )
			{
				PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Width - (ThePrintDocument.DefaultPageSettings.Margins.Left + ThePrintDocument.DefaultPageSettings.Margins.Right );
				PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Height - (ThePrintDocument.DefaultPageSettings.Margins.Top + ThePrintDocument.DefaultPageSettings.Margins.Bottom );
			}
			else
			{
				PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Width - (ThePrintDocument.DefaultPageSettings.Margins.Left + ThePrintDocument.DefaultPageSettings.Margins.Right );
				PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Height - (ThePrintDocument.DefaultPageSettings.Margins.Top + ThePrintDocument.DefaultPageSettings.Margins.Bottom );
			}

			// Save the margins for this page
			TopMargin = ThePrintDocument.DefaultPageSettings.Margins.Top;
			BottomMargin = ThePrintDocument.DefaultPageSettings.Margins.Bottom;
			LeftMargin = ThePrintDocument.DefaultPageSettings.Margins.Left;

			// Calculate the ideal column widths from the data grid style
			columnWidths = new int[ TheGrid.DataTable.Columns.Count ];
			List<CustomGrid_ColumnStyle> colStyle = TheGrid.Style.Column_Styles;
            int total_width_fixed = 0;
            int total_width_nonfixed = 0;
			TotalWidth = 0;
			
			// Step through each column to get the width from the data grid style
			for( int i = 0 ; i < columnWidths.Length ; i++ )
			{
				// If the width is 5 pixels wide or less, there should be no width
				if (( colStyle[i].Width <= 5 ) || ( !colStyle[i].Visible ))
					columnWidths[i] = 0;
				else
					columnWidths[i] = colStyle[i].Width;

				// Increment the total width preferred by the data grid style
                if (columnWidths[i] != 0)
                {
                    if (colStyle[i].Fixed_Print_Width >= 0)
                    {
                        total_width_fixed = total_width_fixed + colStyle[i].Fixed_Print_Width;
                    }
                    else
                    {
                        total_width_nonfixed = total_width_nonfixed + columnWidths[i]; 
                    }

                }
			}

			// Now, scale the column widths down to fit the width of the page, if the 
			// sum of the widths of each column exceeded the width of the printed page
            TotalWidth = total_width_fixed + total_width_nonfixed;
			if ( (total_width_fixed + total_width_nonfixed ) > PageWidth )
			{
                int non_fixed_total_width = PageWidth - total_width_fixed;

				// Step through each column and multiply by the ratio of total preferred size to page width
				for( int i = 0 ; i < columnWidths.Length; i++ )
				{
                    if (colStyle[i].Fixed_Print_Width < 0)
                    {
                        columnWidths[i] = (int)((((float)columnWidths[i]) / ((float)total_width_nonfixed)) * (non_fixed_total_width));
                    }
				}

				// Computer the actual page width
                TotalWidth = 0;
                for( int i = 0 ; i < columnWidths.Length ; i++ )
                {
                    TotalWidth += columnWidths[i];
                }
			}

			try
			{
				// Draw the header on this page first
				DrawHeader(g);

				// Draw the rows and capture the flag indicating more rows need to be 
				// printed on the next page
				bool bContinue = DrawRows(g, All_In_View);
				return bContinue;
			}
			catch (Exception ex)
			{
				// An error occurred, so show an error message
				MessageBox.Show(ex.Message.ToString());
				return false;
			}
		}

		/// <summary> Draws the datagrid and each row in the view </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		public bool DrawDataGrid(Graphics g)
		{
			return DrawAppropriateDataGrid( g, true );
		}

		/// <summary> Draws the datagrid and the rows that are selected </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		public bool DrawDataGrid_SelectedRows(Graphics g)
		{
			return DrawAppropriateDataGrid( g, false );
		}
	}
}

