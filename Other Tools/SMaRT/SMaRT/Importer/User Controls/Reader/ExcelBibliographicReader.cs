using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data;
using SobekCM.Resource_Object;
using GemBox.Spreadsheet;

namespace SobekCM.Management_Tool.Importer
{
    /// <summary>
    /// Summary description for ExcelBibliographicReader.
    /// </summary>
    public class ExcelBibliographicReader : iBibliographicReader
    {
        private string fileName;
        private string sheetName;

        private Column_Field_Mapper columnMaps;

        public DataTable excelData;

        private int row;

        private SobekCM.Resource_Object.Bibliographic_Mapping bibMap;

        public ExcelBibliographicReader()
        {
            // Set the Gembox spreadsheet license key
            GemBox.Spreadsheet.SpreadsheetInfo.SetLicense("EDWF-ZKV9-D793-1D2A");


            // Set some defaults
            fileName = String.Empty;
            sheetName = String.Empty;
            excelData = null;
            bibMap = new Bibliographic_Mapping();
            columnMaps = new Column_Field_Mapper();
        }

        public string Filename
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string Sheet
        {
            get { return sheetName; }
            set { sheetName = value; }
        }

        public Column_Field_Mapper Column_Maps
        {
            get { return columnMaps; }
            set { columnMaps = value; }
        }

        /// <summary>
        /// This mehtod retrieves the excel sheet names from 
        /// an excel workbook.
        /// </summary>
        /// <param name="Filename">The excel file name.</param>
        /// <returns>String[]</returns>
        public List<string> GetExcelSheetNames(string Filename)
        {
            try
            {
                // Read the excel file
                ExcelFile ef = new ExcelFile();
                if (Filename.ToLower().IndexOf(".xlsx") > 0)
                    ef.LoadXlsx(Filename, XlsxOptions.PreserveMakeCopy);
                else
                    ef.LoadXls(Filename);

                // Get the list of sheet names
                List<string> sheetNames = new List<string>();

                // Step through each worksheet, getting the name
                foreach (ExcelWorksheet sheet in ef.Worksheets)
                {
                    sheetNames.Add(sheet.Name);
                }

                // Return the list
                return sheetNames;
            }
            catch (Exception ee)
            {
                return null;
            }
        }

        public bool Check_Source()
        {
            try
            {
                // Read the excel file
                ExcelFile ef = new ExcelFile();
                if (fileName.ToLower().IndexOf(".xlsx") > 0)
                    ef.LoadXlsx(fileName, XlsxOptions.PreserveMakeCopy);
                else
                    ef.LoadXls(fileName);

                // Get the worksheet
                ExcelWorksheet sheet = ef.Worksheets[sheetName];

                // Read the first row of the worksheet
                ExcelRow row = sheet.Rows[0];

                // Create the datatable
                excelData = new DataTable(sheetName);
                List<string> columnNames = new List<string>();
                foreach (ExcelCell headerCell in row.Cells)
                {
                    if (headerCell.Value != null)
                    {
                        string cellValue = headerCell.Value.ToString();
                        string columnName = headerCell.Value.ToString();
                        if (columnNames.Contains(columnName))
                        {
                            int index = 2;
                            while (columnNames.Contains(cellValue + index))
                                index++;
                            columnName = cellValue + index;
                        }
                        columnNames.Add(columnName);
                        excelData.Columns.Add(columnName, typeof(string));
                    }
                }

                // Manage ExtractDataError.WrongType error
                sheet.ExtractDataEvent += (sender, e) =>
                {
                    if (e.ErrorID == ExtractDataError.WrongType)
                    {
                        e.DataTableValue = e.ExcelValue == null ? null : e.ExcelValue.ToString();
                        e.Action = ExtractDataEventAction.Continue;
                    }
                };

                // Extract the data from the worksheet to the DataTable
                sheet.ExtractToDataTable(excelData, 20000, ExtractDataOptions.StopAtFirstEmptyRow, sheet.Rows[0], sheet.Columns[0]);

                // Remvoe the first row 
                excelData.Rows.RemoveAt(0);

                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        #region iBibliographicReader Members

        public SobekCM_Item Next()
        {
            // Is this a valid row?
            if (row >= excelData.Rows.Count)
                return null;

            // Create the object based on the column mapping
            SobekCM_Item returnPackage = new SobekCM_Item();

            // reset the static variables in the mappings class
            Bibliographic_Mapping.clear_static_variables();

            // Add data from each column into the bib package

            // Step through each column in the data row
            DataRow thisRow = excelData.Rows[row++];
            for (int i = 0; i < excelData.Columns.Count; i++)
            {
                if ((thisRow[i] != null) && (thisRow[i].ToString().Length > 0) && (columnMaps.isMapped(i)))
                {
                    Bibliographic_Mapping.Add_Data(returnPackage, thisRow[i].ToString(), columnMaps.Get_Field(i).Field);
                }
            }

            return returnPackage;
        }

        public int Count
        {
            get
            {
                return excelData.Rows.Count;
            }
        }

        public void Close()
        {
            // All data already loaded in memory, so do nothing here
            // except clear the datatablt
            excelData = null;
        }

        #endregion

	}
}
