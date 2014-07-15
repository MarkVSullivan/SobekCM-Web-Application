using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Collections;

namespace SobekCM.Management_Tool.Importer
{

	// ************************ DataGridPrinter ********************
	// By Michael Gold
	// Copyright 2002.  All Rights Reserved
	// *********************************************************


	/// <summary> Class is used to print a data grid to a printer. <br /> <br /> </summary>
	/// <remarks> Adapted by Mark Sullivan (2005) from original by Michael Gold (2002). </remarks>
	public class DataGridPrinter
	{

		private PrintDocument ThePrintDocument;
		private DataTable TheTable;
		private DataGrid  TheDataGrid;

		public int RowCount = 0;  // current count of rows;
		private const int kVerticalCellLeeway = 10;
		public int PageNumber = 1;
		public ArrayList Lines = new ArrayList();

		int PageWidth;
		int PageHeight;
		int TopMargin;
		int BottomMargin;
		int LeftMargin;
		int TotalWidth;
		int[] columnWidths;


		/// <summary> Constructor for a new instance of this class </summary>
		/// <param name="aGrid"></param>
		/// <param name="aPrintDocument"></param>
		/// <param name="aTable"></param>
		public DataGridPrinter(DataGrid aGrid, PrintDocument aPrintDocument, DataTable aTable)
		{
			//
			// TODO: Add constructor logic here
			//
			TheDataGrid = aGrid;
			ThePrintDocument = aPrintDocument;
			TheTable = aTable;

			PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Width;
			PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Height;
			TopMargin = ThePrintDocument.DefaultPageSettings.Margins.Top;
			BottomMargin = ThePrintDocument.DefaultPageSettings.Margins.Bottom;
		}

		public void DrawHeader(Graphics g)
		{
			SolidBrush ForeBrush = new SolidBrush(TheDataGrid.HeaderForeColor);
			SolidBrush BackBrush = new SolidBrush(TheDataGrid.HeaderBackColor);
			Pen TheLinePen = new Pen(TheDataGrid.GridLineColor, 1);
			StringFormat cellformat = new StringFormat();
			cellformat.Trimming = StringTrimming.EllipsisCharacter;
			cellformat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;



			int columnwidth = PageWidth/TheTable.Columns.Count;

			int initialRowCount = RowCount;

			// draw the table header
			float startxposition = LeftMargin;
			RectangleF nextcellbounds = new RectangleF(0,0, 0, 0);

			RectangleF HeaderBounds  = new RectangleF(0, 0, 0, 0);

			HeaderBounds.X = LeftMargin;
			HeaderBounds.Y = TopMargin + (RowCount - initialRowCount) * (TheDataGrid.Font.SizeInPoints  + kVerticalCellLeeway);
			HeaderBounds.Height = TheDataGrid.Font.SizeInPoints + kVerticalCellLeeway;
			HeaderBounds.Width = PageWidth;

			g.FillRectangle(BackBrush, HeaderBounds);

			for (int k = 0; k < TheTable.Columns.Count; k++)
			{
				// Only show this column if it has a width
				if ( columnWidths[k] > 3 )
				{	
					string nextcolumn = TheDataGrid.TableStyles[0].GridColumnStyles[k].HeaderText;
					RectangleF cellbounds = new RectangleF(startxposition, TopMargin + (RowCount - initialRowCount) * (TheDataGrid.Font.SizeInPoints  + kVerticalCellLeeway),
						columnWidths[k], 
						TheDataGrid.HeaderFont.SizeInPoints + kVerticalCellLeeway);
					nextcellbounds = cellbounds;

					g.DrawString(nextcolumn, TheDataGrid.HeaderFont, ForeBrush, cellbounds, cellformat);
					startxposition = startxposition + columnWidths[k];
				}
			}
	
			if (TheDataGrid.GridLineStyle != DataGridLineStyle.None)
				g.DrawLine(TheLinePen, LeftMargin, nextcellbounds.Bottom, PageWidth + LeftMargin, nextcellbounds.Bottom);
		}

		public bool DrawRows(Graphics g)
		{
			try
			{
				int lastRowBottom = TopMargin; 
				//   Create an array to save the horizontal positions for drawing horizontal gridlines
				ArrayList Lines = new ArrayList();

				// form brushes based on the color properties of the DataGrid
				// These brushes will be used to draw the grid borders and cells 
				SolidBrush ForeBrush = new SolidBrush(TheDataGrid.ForeColor);
				SolidBrush BackBrush = new SolidBrush(TheDataGrid.BackColor);
				SolidBrush AlternatingBackBrush = new SolidBrush(TheDataGrid.AlternatingBackColor);
				Pen TheLinePen = new Pen(TheDataGrid.GridLineColor, 1);

				// Create a format for the cell so that the string in the cell is cut off at the end of the column width
				StringFormat cellformat = new StringFormat();
				cellformat.Trimming = StringTrimming.EllipsisCharacter;
				cellformat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

				// set the initial row count, this will start at 0 for the first page, and be a different value for the 2nd, 3rd, 4th, etc.
				// pages.
				int initialRowCount = RowCount;
				RectangleF RowBounds = new RectangleF(0, 0, 0, 0);

				// draw the rows of the table 
				for (int i = initialRowCount; i < TheTable.Rows.Count; i++)
				{
					// get the next DataRow in the DataTable
					DataRow dr = TheTable.Rows[i];
					int startxposition = LeftMargin;

					//  Calculate the row boundary based on teh RowCount and offsets into the page
					RowBounds.X = LeftMargin;
					RowBounds.Y = TopMargin + ((RowCount - initialRowCount)+1) * (TheDataGrid.Font.SizeInPoints + kVerticalCellLeeway);
					RowBounds.Height = TheDataGrid.Font.SizeInPoints + kVerticalCellLeeway;
					RowBounds.Width = PageWidth;

					// save the vertical row positions for drawing grid lines
					Lines.Add(RowBounds.Bottom);

					// paint rows differently for alternate row colors
					if (i%2 == 0)
						g.FillRectangle(BackBrush, RowBounds);
					else
						g.FillRectangle(AlternatingBackBrush, RowBounds);

					// Go through each column in the row and draw the information from the DataRow
					for (int j = 0; j < TheTable.Columns.Count; j++)
					{
						// Only show this column if it has a width
						if ( columnWidths[j] > 3 )
						{
							RectangleF cellbounds = new RectangleF(startxposition,
								TopMargin + ((RowCount - initialRowCount) + 1) * 
								(TheDataGrid.Font.SizeInPoints + kVerticalCellLeeway),
								columnWidths[j],
								TheDataGrid.Font.SizeInPoints + kVerticalCellLeeway);

							g.DrawString(dr[j].ToString(), TheDataGrid.Font, ForeBrush, cellbounds, cellformat);
							lastRowBottom = (int)cellbounds.Bottom;

							// increment the column position
							startxposition = startxposition + columnWidths[j];
						}
					}
					RowCount++;

					// when we've reached the bottom of the page, draw the horizontal and vertical grid lines and return true
					if (RowCount * (TheDataGrid.Font.SizeInPoints + kVerticalCellLeeway) > (PageHeight * PageNumber))
					{
						DrawHorizontalLines(g, Lines);
						DrawVerticalGridLines(g, TheLinePen, lastRowBottom);
						return true; 
					}
				}

				// when we've reached the end of the table, draw the horizontal and vertical grid lines and return false
				DrawHorizontalLines(g, Lines);
				DrawVerticalGridLines(g, TheLinePen,lastRowBottom);

				return false;
			}
			catch (Exception ex)
			{
                DLC.Tools.Forms.ErrorMessageBox.Show(ex.Message, "Unexpected Error", ex);
				return false;
			}
		}

		void DrawHorizontalLines(Graphics g, ArrayList lines)
		{
			Pen TheLinePen = new Pen(TheDataGrid.GridLineColor, 1);

			if (TheDataGrid.GridLineStyle == DataGridLineStyle.None)
				return;

			for (int i = 0;  i < lines.Count; i++)
			{
				g.DrawLine(TheLinePen, LeftMargin, (float)lines[i], PageWidth + LeftMargin, (float)lines[i]);
			}
		}

		void DrawVerticalGridLines(Graphics g, Pen TheLinePen, int bottom)
		{
			if (TheDataGrid.GridLineStyle == DataGridLineStyle.None)
				return;

			int start = LeftMargin;
			for (int k = 0; k < TheTable.Columns.Count; k++)
			{
				if ( columnWidths[k] > 3 )
				{
					g.DrawLine(TheLinePen, start, TopMargin, start, bottom);
					start = start + columnWidths[k];
				}
			}

			// Draw the last line
			g.DrawLine(TheLinePen, start, TopMargin, start, bottom);
		}


		public bool DrawDataGrid(Graphics g)
		{
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

			TopMargin = ThePrintDocument.DefaultPageSettings.Margins.Top;
			BottomMargin = ThePrintDocument.DefaultPageSettings.Margins.Bottom;
			LeftMargin = ThePrintDocument.DefaultPageSettings.Margins.Left;

			// Calculate the column widths
			columnWidths = new int[ TheTable.Columns.Count ];
			columnWidths[0] = 80;
			GridColumnStylesCollection colStyle = TheDataGrid.TableStyles[0].GridColumnStyles;
			TotalWidth = 0;
			for( int i = 1 ; i < columnWidths.Length ; i++ )
			{
				if ( colStyle[i].Width <= 5 )
                    columnWidths[i] = 0;
				else
					columnWidths[i] = colStyle[i].Width;
				TotalWidth += columnWidths[i];
			}

			// Adjust the last column as well
			if (( columnWidths.Length > 7 ) && ( columnWidths[7] > 0 ))
			{
				TotalWidth -= columnWidths[7];
				columnWidths[7] = 170;

				// Now, factor the widths correctly
				for( int i = 1 ; i < 7; i++ )
				{
					columnWidths[i] = (int) ((((float) columnWidths[i]) / ((float) TotalWidth)) * (PageWidth - 250));
				}
			}
			else
			{
				// Now, factor the widths correctly
				for( int i = 1 ; i < columnWidths.Length; i++ )
				{
					columnWidths[i] = (int) ((((float) columnWidths[i]) / ((float) TotalWidth)) * (PageWidth - 80));
				}
			}


			try
			{
				DrawHeader(g);
				bool bContinue = DrawRows(g);
				return bContinue;
			}
			catch (Exception ex)
			{
                DLC.Tools.Forms.ErrorMessageBox.Show(ex.Message, "Unexpected Error", ex);
				return false;
			}
		}
	}
}

