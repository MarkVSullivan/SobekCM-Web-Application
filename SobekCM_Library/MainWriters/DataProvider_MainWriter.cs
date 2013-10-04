#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;

#endregion

namespace SobekCM.Library.MainWriters
{
	/// <summary> Main writer provides datatables as JSON to drive anything which requires
	/// server-side paging or sorting of a large amount of data. </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
	public class DataProvider_MainWriter : abstractMainWriter
	{
		/// <summary> Constructor for a new instance of the DataProvider_MainWriter class </summary>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
		/// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
		/// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
		/// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
		/// <param name="Current_Item"> Current item to display </param>
		/// <param name="Current_Page"> Current page within the item</param>
		public DataProvider_MainWriter(SobekCM_Navigation_Object Current_Mode,
			Item_Aggregation Hierarchy_Object,
			Search_Results_Statistics Results_Statistics,
			List<iSearch_Title_Result> Paged_Results,
			Item_Aggregation_Browse_Info Browse_Object,
			SobekCM_Item Current_Item,
			Page_TreeNode Current_Page)
			: base(Current_Mode, Hierarchy_Object, Results_Statistics, Paged_Results, Browse_Object, Current_Item, Current_Page, null)
		{
			// All work done in base class
		}

		/// <summary> Gets the enumeration of the type of main writer </summary>
		/// <value> This property always returns the enumerational value <see cref="SobekCM.Library.Navigation.Writer_Type_Enum.DataSet"/>. </value>
		public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.DataSet; } }

		/// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
		/// <param name="Output"> Stream to which to write the text for this main writer </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
		{
			switch (currentMode.Mode)
			{
				case Display_Mode_Enum.Item_Display:
					if (currentMode.ViewerCode.IndexOf("dsview", StringComparison.OrdinalIgnoreCase) == 0)
					{
						provide_dataset_items_view_data(Output);
					}
					break;
				default:
					Output.Write("DataSet Writer - Unknown Mode");
					break;
			}
		}

		private void provide_dataset_items_view_data(TextWriter Output)
		{
			string error_message = String.Empty;
			string key = currentItem.BibID + "_" + currentItem.VID + "_Dataset";
			DataSet itemDataset = HttpContext.Current.Cache[key] as DataSet;
			if (itemDataset == null)
			{
				// Find the dataset from the METS strucutre map.  Currently this looks
				// only for XML with attached XSD
				string xml_file = (from thisFile in currentItem.Divisions.Download_Other_Files where thisFile.System_Name.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) > 0 select thisFile.System_Name).FirstOrDefault();

				// If one was found, read it in!
				if (!String.IsNullOrEmpty(xml_file))
				{
					itemDataset = new DataSet();
					try
					{
						// Read the XML file
						itemDataset.ReadXml(currentItem.Source_Directory + "\\" + xml_file);

						// Add this to the cache
						HttpContext.Current.Cache.Insert(key, itemDataset, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
					}
					catch (Exception)
					{
						itemDataset = null;
						error_message = "Error while reading XML file " + xml_file;
					}
				}
			}

			// Return if the result was null
			if (itemDataset == null)
				return;

			// Get ready to pull the informaiton from the query string which the
			// jquery datatables library pass in
			int displayStart;
			int displayLength;
			string sEcho = String.Empty;
			int sortingColumn1;
			string sortDirection1 = "asc";
			int sortingColumn2;
			string sortDirection2 = "asc";


			// Get the display start and length from the DataTables generated data URL
			Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayStart"], out displayStart);
			Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayLength"], out displayLength);

			// Get the echo value
			sEcho = HttpContext.Current.Request.QueryString["sEcho"];

			// Get the sorting column and sorting direction
			Int32.TryParse(HttpContext.Current.Request.QueryString["iSortCol_0"], out sortingColumn1);
			if ((HttpContext.Current.Request.QueryString["sSortDir_0"] != null) && (HttpContext.Current.Request.QueryString["sSortDir_0"] == "desc"))
				sortDirection1 = "desc";
			Int32.TryParse(HttpContext.Current.Request.QueryString["iSortCol_1"], out sortingColumn2);
			if ((HttpContext.Current.Request.QueryString["sSortDir_1"] != null) && (HttpContext.Current.Request.QueryString["sSortDir_1"] == "desc"))
				sortDirection2 = "desc";


			// Look for the search term and such from the current query string
			string term = String.Empty;
			string field = String.Empty;
			string[] possibles = new string[] { "col1", "col2", "col3", "col4", "col5", "col6", "col7", "col8", "col9", "col10", "col11", "col12", "col13", "col14", "col15", "col16", "col17", "col18", "col19", "col20" };
			foreach (string possibility in possibles)
			{
				if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString[possibility]))
				{
					field = possibility;
					term = HttpContext.Current.Request.QueryString[possibility];
					break;
				}
			}

			// Create the results view
			DataTable results = itemDataset.Tables[currentMode.SubPage - 2];
			DataView resultsView = new DataView(results);

			// Should a filter be applied?
			if (term.Length > 0)
			{
				int column = Convert.ToInt32(field.Replace("col", "")) - 1;
				if ((column >= 0) && (column < results.Columns.Count))
				{
					string columnname = results.Columns[column].ColumnName;
					resultsView.RowFilter = columnname + " like '%" + term.Replace("'", "''") + "%'";
				}
			}

			// Get the count of results
			int total_results = resultsView.Count;

			// Start the JSON response
			Output.WriteLine("{");
			Output.WriteLine("\"sEcho\": " + sEcho + ",");
			Output.WriteLine("\"iTotalRecords\": \"" + total_results + "\",");
			Output.WriteLine("\"iTotalDisplayRecords\": \"" + total_results + "\",");
			Output.WriteLine("\"aaData\": [");


			// Get columns to display 
			List<DataColumn> columns_to_display = results.Columns.Cast<DataColumn>().ToList();

			// Sort by the correct column
			DataColumn sortColumn = null;
			if (sortingColumn1 > 0)
			{
				sortColumn = columns_to_display[sortingColumn1 - 1];
				string column_name_for_sort = sortColumn.ColumnName;
				resultsView.Sort = column_name_for_sort + " " + sortDirection1;

				//if (column_name_for_sort == "InstitutionName")
				//	resultsView.Sort = resultsView.Sort + ", Standard1 asc";

				if (sortColumn.DataType == Type.GetType("System.Int32"))
				{
					if ( resultsView.RowFilter.Length > 0 )
						resultsView.RowFilter = resultsView.RowFilter + " and " + column_name_for_sort + " >= 0 and " + column_name_for_sort + " is not null";
					else
						resultsView.RowFilter = column_name_for_sort + " >= 0 and " + column_name_for_sort + " is not null";
				}
				if (sortColumn.DataType == Type.GetType("System.String"))
				{
					if (resultsView.RowFilter.Length > 0)
						resultsView.RowFilter = resultsView.RowFilter + " and " + column_name_for_sort + " <> '' and " + column_name_for_sort + " is not null";
					else
						resultsView.RowFilter = column_name_for_sort + " <> '' and " + column_name_for_sort + " is not null";
				}
			}

			// Get the last column
			DataColumn lastColumn = columns_to_display[columns_to_display.Count - 1];

			// Add the data for the rows to show
			int adjust_for_filter = 0;
			bool filter_modified = false;
			int row_count = 1;
			for (int i = displayStart; (i < displayStart + displayLength) && (i < total_results); i++)
			{

				// Is this now over the resultsView count, possibly due to a sort?
				if (i - adjust_for_filter > resultsView.Count - 1)
				{
					if ((resultsView.RowFilter.Length > 0) && (sortColumn != null) && (!filter_modified))
					{
						adjust_for_filter = resultsView.Count;
						filter_modified = true;

						if (sortColumn.DataType == System.Type.GetType("System.Int32"))
							resultsView.RowFilter = sortColumn.ColumnName + " < 0 or " + sortColumn.ColumnName + " is null";
						if (sortColumn.DataType == System.Type.GetType("System.String"))
							resultsView.RowFilter = sortColumn.ColumnName + " = '' or " + sortColumn.ColumnName + " is null";
					}
					else
					{
						// Should never get here, but just in case, exit cleanly
						break;
					}
				}

				// Start the JSON response for this row
				Output.Write("[ \"" + row_count + "\",");

				DataRow thisRow = resultsView[i - adjust_for_filter].Row;

				// Add the data for each column
				foreach (DataColumn thisCOlumn in columns_to_display)
				{
					string value = String.Empty;
					if (thisRow[thisCOlumn] != DBNull.Value)
					{
						value = thisRow[thisCOlumn].ToString().Replace(" 12:00:00 AM", "").Replace("1/1/1900", "");
						if (value == "-1")
							value = String.Empty;
					}


					if (thisCOlumn == lastColumn)
					{
						Output.Write("\"" + value.Replace(" | ", "<br />").Replace("\"", "'") + "\"");
					}
					else
					{
						Output.Write("\"" + value.Replace(" | ", "<br />").Replace("\"", "'") + "\",");
					}
				}

				// Finish this row
				if ((i < displayStart + displayLength - 1) && (i < total_results - 1))
					Output.WriteLine("],");
				else
					Output.WriteLine("]");

				row_count++;
			}

			Output.WriteLine("]");
			Output.WriteLine("}");


		}
	}
}
