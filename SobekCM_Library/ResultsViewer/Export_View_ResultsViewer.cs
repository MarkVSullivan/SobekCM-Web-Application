#region Using directives

using System;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using GemBox.Spreadsheet;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Results viewer allows the results to exported into either an Excel file or a CSV file for temporary download.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class Export_File_ResultsViewer : abstract_ResultsViewer
    {
        /// <summary> Constructor for a new instance of the Export_File_ResultsViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Export_File_ResultsViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            Results_Per_Page = 1000;
        }

        /// <summary> Flag indicates if this result view is sortable </summary>
        /// <value> This property always returns the value FALSE </value>
        public override bool Sortable
        {
            get
            {
                return false;
            }
        }

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public override void Add_HTML(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Export_File_ResultsViewer.Add_HTML", "Rendering results in text view");
            }

            //// Sort this by BIbID
            //resultTable.Sort("BibID");

            //// Determine which rows to display
            //int lastRow = base.LastRow;
            //int startRow = base.StartRow(lastRow);

            //// prepare to step through each title to display
            //List<SobekCM_Item_Collection.SobekCM_Item_Row> itemRows = new List<SobekCM_Item_Collection.SobekCM_Item_Row>();

            // Prepare to build an output
            StringBuilder resultsBldr = new StringBuilder(5000);

            int currentPage = RequestSpecificValues.Current_Mode.Page.HasValue ? RequestSpecificValues.Current_Mode.Page.Value : 1;
            if (currentPage < 2)
            {
                resultsBldr.Append("<br />" + Environment.NewLine + "<div class=\"SobekHomeText\">" + Environment.NewLine + "<blockquote>" + Environment.NewLine );
                resultsBldr.Append("This option allows you to export the list of results which match your search or browse to an excel document or CSV file for download.<br /><br />");
                resultsBldr.Append("Select the file type below to create the report:");
                resultsBldr.Append("<blockquote>");

                //currentMode.Page = 2;
                //resultsBldr.Append("<a href=\"" + currentMode.Redirect_URL() + "\">Excel Spreadsheet file (XLSX)</a><br /><br />");

                RequestSpecificValues.Current_Mode.Page = 3;
                resultsBldr.Append("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Excel Spreadsheet file (XLS)</a><br /><br />");

                RequestSpecificValues.Current_Mode.Page = 4;
                resultsBldr.Append("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Comma-seperated value text file (CSV)</a><br /><br />");

                //currentMode.Page = 5;
                //resultsBldr.Append("<a href=\"" + currentMode.Redirect_URL() + "\">HTML Table (HTML)</a><br /><br />");


                resultsBldr.Append("</blockquote>");
                resultsBldr.Append("</blockquote>");
                resultsBldr.Append("<br /><br />");
                resultsBldr.Append("</div>");

                // Add this to the page
                Literal mainLiteral = new Literal {Text = resultsBldr.ToString()};
                placeHolder.Controls.Add(mainLiteral);

            }
            else
            {
                string filename = RequestSpecificValues.Current_Mode.Instance_Name;

                // Set the Gembox spreadsheet license key
	           
	            string from_db = String.Empty;
	            string key = String.Empty;
				if (UI_ApplicationCache_Gateway.Settings.Additional_Settings.ContainsKey("Spreadsheet Library License"))
				{
					try
					{
						key = UI_ApplicationCache_Gateway.Settings.Additional_Settings["Spreadsheet Library License"];

						SecurityInfo thisDecryptor = new SecurityInfo();
						string encryptedPassword = thisDecryptor.DecryptString(from_db, "*h3kj(83", "unsalted");
					}
					catch (Exception )
					{

					}
				}

				
                SpreadsheetInfo.SetLicense(key);

                // Create the excel file and worksheet
                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheet excelSheet = excelFile.Worksheets.Add(RequestSpecificValues.Current_Mode.Instance_Name);
                excelFile.Worksheets.ActiveWorksheet = excelSheet;

                // Create the header cell style
                CellStyle headerStyle = new CellStyle
                                            { HorizontalAlignment = HorizontalAlignmentStyle.Left, Font = {Weight = ExcelFont.BoldWeight} };
                headerStyle.FillPattern.SetSolid(Color.Yellow);
                headerStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                // Create the title cell style
                CellStyle titleStyle = new CellStyle
                                           { HorizontalAlignment = HorizontalAlignmentStyle.Left, Font = {Weight = ExcelFont.BoldWeight, Size = 12*20} };

                // Set the default style
                CellStyle defaultStyle = new CellStyle();
                defaultStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);

                const int columnSelector = 0;
                int row = 0;

                if (RequestSpecificValues.Current_Mode.Page != 4)
                {
                    // Add the title
                    excelSheet.Cells[1, columnSelector].Value = "Search or browse result list";
                    excelSheet.Cells[1, columnSelector].Style = titleStyle;
                    row = 3;
                }

                // Add the column headers
                excelSheet.Cells[row, columnSelector].Value = "System ID";
                excelSheet.Cells[row, columnSelector].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 1].Value = "Link";
                excelSheet.Cells[row, columnSelector + 1].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 2].Value = "Group Title";
                excelSheet.Cells[row, columnSelector + 2].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 3].Value = "Item Title";
                excelSheet.Cells[row, columnSelector + 3].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 4].Value = "Date";
                excelSheet.Cells[row, columnSelector + 4].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 5].Value = "Author";
                excelSheet.Cells[row, columnSelector + 5].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 6].Value = "Publisher";
                excelSheet.Cells[row, columnSelector + 6].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 7].Value = "Format";
                excelSheet.Cells[row, columnSelector + 7].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 8].Value = "Edition";
                excelSheet.Cells[row, columnSelector + 8].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 9].Value = "Subjects";
                excelSheet.Cells[row, columnSelector + 9].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 10].Value = "Measurement";
                excelSheet.Cells[row, columnSelector + 10].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 11].Value = "Style / Period";
                excelSheet.Cells[row, columnSelector + 11].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 12].Value = "Technique";
                excelSheet.Cells[row, columnSelector + 12].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 13].Value = "Institution";
                excelSheet.Cells[row, columnSelector + 13].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 14].Value = "Donor";
                excelSheet.Cells[row, columnSelector + 14].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 15].Value = "OCLC";
                excelSheet.Cells[row, columnSelector + 15].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 16].Value = "ALEPH";
                excelSheet.Cells[row, columnSelector + 16].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 17].Value = "Serial1";
                excelSheet.Cells[row, columnSelector + 17].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 18].Value = "Serial2";
                excelSheet.Cells[row, columnSelector + 18].Style = headerStyle;
                excelSheet.Cells[row, columnSelector + 19].Value = "Serial3";
                excelSheet.Cells[row, columnSelector + 19].Style = headerStyle;

                // Set the correct widths
                excelSheet.Columns[columnSelector].Width = 12 * 256;
                excelSheet.Columns[columnSelector + 1].Width = 8 * 256;
                excelSheet.Columns[columnSelector + 2].Width = 40 * 256;
                excelSheet.Columns[columnSelector + 3].Width = 40 * 256;
                excelSheet.Columns[columnSelector + 4].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 5].Width = 14 * 256;
                excelSheet.Columns[columnSelector + 6].Width = 14 * 256;
                excelSheet.Columns[columnSelector + 7].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 8].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 9].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 10].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 11].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 12].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 13].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 14].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 15].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 16].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 17].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 18].Width = 20 * 256;
                excelSheet.Columns[columnSelector + 19].Width = 20 * 256;
                row++;

                // Add each row
                foreach (iSearch_Title_Result titleResult in RequestSpecificValues.Paged_Results)
                {
                    for (int item_count = 0; item_count < titleResult.Item_Count; item_count++)
                    {
                        iSearch_Item_Result itemResult = titleResult.Get_Item(item_count);

                        excelSheet.Cells[row, columnSelector].Value = titleResult.BibID + "_" + itemResult.VID;
                        excelSheet.Cells[row, columnSelector + 1].Value = RequestSpecificValues.Current_Mode.Base_URL + titleResult.BibID + "/" + itemResult.VID;
                        excelSheet.Cells[row, columnSelector + 2].Value = titleResult.GroupTitle.Replace("<i>","").Replace("</i>","").Replace("&amp;","&");
                        excelSheet.Cells[row, columnSelector + 3].Value = itemResult.Title.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 4].Value = itemResult.PubDate.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 5].Value = titleResult.Author.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 6].Value = titleResult.Publisher.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 7].Value = titleResult.Format.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 8].Value = titleResult.Edition.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 9].Value = titleResult.Subjects.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 10].Value = titleResult.Measurement.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 11].Value = titleResult.Style_Period.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 12].Value = titleResult.Technique.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 13].Value = titleResult.Institution.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 14].Value = titleResult.Donor.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//if (titleResult.OCLC_Number > 1)
						//	excelSheet.Cells[row, columnSelector + 15].Value = titleResult.OCLC_Number.ToString();
						//if (titleResult.ALEPH_Number > 1)
						//	excelSheet.Cells[row, columnSelector + 16].Value = titleResult.ALEPH_Number.ToString();
						//excelSheet.Cells[row, columnSelector + 17].Value = itemResult.Level1_Text.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 18].Value = itemResult.Level2_Text.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
						//excelSheet.Cells[row, columnSelector + 19].Value = itemResult.Level3_Text.Replace("<i>", "").Replace("</i>", "").Replace("&amp;", "&");
 
                        row++;
                    }
                }

                // Set the border
                excelSheet.Cells.GetSubrange("A4", "T" + row).SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                excelSheet.Cells.GetSubrange("A4", "t4").SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                if (Tracer != null)
                {
                    Tracer.Add_Trace("Export_File_ResultsViewer.Add_HTML", "Clearing response");
                }

                // Clear any response until now
                HttpContext.Current.Response.Clear();


                // Output in proper format to the user
                switch (RequestSpecificValues.Current_Mode.Page)
                {
                    //case 2:
                    //    System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats";
                    //    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".xlsx");
                    //    // With XLSX it is a bit more complicated as MS Packaging API can't write
                    //    // directly to Response.OutputStream. Therefore we use temporary MemoryStream.
                    //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    //    excelFile.SaveXlsx(ms);
                    //    ms.WriteTo(System.Web.HttpContext.Current.Response.OutputStream);
                    //    break;


                    case 3:
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".xls");
                        excelFile.SaveXls(HttpContext.Current.Response.OutputStream);
                        break;

                    case 4:
                        HttpContext.Current.Response.ContentType = "text/csv";
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".csv");
                        excelFile.SaveCsv(HttpContext.Current.Response.OutputStream, CsvType.CommaDelimited);
                        break;

                    //case 5:
                    //    System.Web.HttpContext.Current.Response.ContentType = "text/html";
                    //    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + "Report18.html");
                    //    System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(System.Web.HttpContext.Current.Response.OutputStream, new UTF8Encoding(false));
                    //    excelFile.SaveHtml(writer, null, true);
                    //    writer.Close();
                    //    break;
                }

                if (Tracer != null)
                {
                    Tracer.Add_Trace("Export_File_ResultsViewer.Add_HTML", "Ending response");
                }

                HttpContext.Current.Response.End();
            }
        }
    }
}
