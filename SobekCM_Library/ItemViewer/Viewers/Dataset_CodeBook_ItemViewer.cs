#region Using directives

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Dataset viewer is used to dispay codebook information present in the XSD accompanying a XML dataset </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Dataset_CodeBook_ItemViewer : abstractItemViewer
	{
		private DataSet itemDataset;
		private string error_message;

		/// <summary> Constructor for a new instance of the Dataset_CodeBook_ItemViewer class </summary>
		public Dataset_CodeBook_ItemViewer( )
		{
			error_message = String.Empty;
		}

		/// <summary> This provides an opportunity for the viewer to perform any pre-display work
		/// which is necessary before entering any of the rendering portions </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <remarks> This ensures the dataset has been read into memory/cache for rendering </remarks>
		public override void Perform_PreDisplay_Work(Custom_Tracer Tracer)
		{
			string key = CurrentItem.BibID + "_" + CurrentItem.VID + "_Dataset";
			itemDataset = HttpContext.Current.Cache[key] as DataSet;
			if (itemDataset == null)
			{
				// Find the dataset from the METS strucutre map.  Currently this looks
				// only for XML with attached XSD
				string xml_file = (from thisFile in CurrentItem.Divisions.Download_Other_Files where thisFile.System_Name.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) > 0 select thisFile.System_Name).FirstOrDefault();

				// If one was found, read it in!
				if (!String.IsNullOrEmpty( xml_file))
				{
					itemDataset = new DataSet();
					try
					{
						// Read the XML file
						itemDataset.ReadXml(CurrentItem.Source_Directory + "\\" + xml_file);

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
		}

		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.JPEG_Text_Two_Up"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.Dataset_Codebook; }
		}

		/// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
		/// <remarks> This always returns the value TRUE for this viewer </remarks>
		public override bool Override_On_Checked_Out
		{
			get
			{
				return true;
			}
		}

		/// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
		/// <value> This returns -1, which allows this to use all the screen </value>
		public override int Viewer_Width
		{
			get
			{
				return 800;
			}
		}

		/// <summary> Gets the number of pages for this viewer </summary>
		/// <value> This is a single page viewer, so this property always returns the value 1</value>
		public override int PageCount
		{
			get
			{
				return 1;
			}
		}

		/// <summary> Gets the flag that indicates if the page selector should be shown </summary>
		/// <value> This is a single page viewer, so this property always returns NONE</value>
		public override ItemViewer_PageSelector_Type_Enum Page_Selector
		{
			get
			{
				return ItemViewer_PageSelector_Type_Enum.NONE;
			}
		}

		///// <summary> Gets the collection of special behaviors which this item viewer
		///// requests from the main HTML subwriter. </summary>
		//public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
		//{
		//	get
		//	{
		//		return new List<HtmlSubwriter_Behaviors_Enum> 
		//			{
		//				HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar
		//			};
		//	}
		//}

		/// <summary> Stream to which to write the HTML for this subwriter  </summary>
		/// <param name="Output"> Response stream for the item viewer to write directly to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Dataset_CodeBook_ItemViewer.Write_Main_Viewer_Section", "");
			}

			Output.WriteLine("          <td><div id=\"sbkDcv_ViewerTitle\">Data Structure / Codebook</div></td>");
			Output.WriteLine("        </tr>");
			Output.WriteLine("        <tr>");


			// Was there an error?
			if ((itemDataset == null) || ( error_message.Length > 0 ))
			{
				Output.WriteLine("          <td id=\"sbkDcv_MainAreaError\">");
				if (error_message.Length > 0)
				{
					Output.WriteLine("            " + error_message );
				}
				else
				{
					Output.WriteLine("            No XML dataset found in the digital resource");
				}
				Output.WriteLine("          </td>");
				return;
			}

			// Start the main area
			const string INDENT = "          ";

			// If only one datatable set the subpage
			if (itemDataset.Tables.Count == 1)
				CurrentMode.SubPage = 2;

			if ((CurrentMode.SubPage < 2) || (CurrentMode.SubPage > (1 + itemDataset.Tables.Count)))
			{
				Output.WriteLine("          <td id=\"sbkDcv_MainArea\">");

				// Add the information about all the datatables
				int table_number = 1;
				foreach (DataTable thisTable in itemDataset.Tables)
				{
					Output.WriteLine(INDENT + "<div id=\"CodeBookTable" + table_number + "\" class=\"sbkDcv_TableDiv\">");
					Output.WriteLine(INDENT + "  <table class=\"sbkDcv_Table\">");

					// Add the header row
					Output.WriteLine(INDENT + "    <tr>");
					Output.WriteLine(INDENT + "      <td>&nbsp;</td>");
					CurrentMode.SubPage = (ushort)(table_number + 1);
					Output.WriteLine(INDENT + "      <td colspan=\"3\" class=\"sbkDcv_TableHeader\"><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\" title=\"View additional details for this table\">" + thisTable.TableName.Replace("_", " ") + "</a></td>");
					Output.WriteLine(INDENT + "    </tr>");

					// Add each table's column information
					foreach (DataColumn thisColumn in thisTable.Columns)
					{
						string column_definition = String.Empty;
						string column_reference = "&nbsp;";
						if (thisColumn.Unique)
						{
							column_definition = thisColumn.AutoIncrement ? "PK" : "U";
						}
						foreach (Constraint constraint in thisTable.Constraints)
						{
							ForeignKeyConstraint fkConstraint = constraint as ForeignKeyConstraint;
							if (fkConstraint != null)
							{
								if (fkConstraint.Columns[0] == thisColumn)
								{
									column_definition = "FK";
									column_reference = fkConstraint.RelatedColumns[0].Table.TableName + "." + fkConstraint.RelatedColumns[0].ColumnName + " <img src=\"" + Static_Resources.Leftarrow_Png + "\" alt=\"<--\" />";
								}
							}
						}



						if (thisColumn.AllowDBNull)
							Output.WriteLine(INDENT + "    <tr>");
						else
							Output.WriteLine(INDENT + "    <tr class=\"sbkDcv_TableRequiredField\">");

						Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableColumn0\">" + column_reference + "</td>");




						Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableColumn1\">" + column_definition + "</td>");
						Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableColumn2\">" + thisColumn.ColumnName.Replace("_", " ") + "</td>");

						string colunmType = thisColumn.DataType.ToString().Replace("System.", "").ToLower().Replace("int32", "integer");
						if ((colunmType == "string") && (thisColumn.MaxLength == 1))
							colunmType = "char";
						Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableColumn3\">" + colunmType + "</td>");
						Output.WriteLine(INDENT + "    </tr>");

						if (column_definition == "PK")
						{
							Output.WriteLine(INDENT + "    <tr><td></td><td colspan=\"3\" class=\"sbkDcv_TablePkRule\"></td></tr>");
						}
					}

					// Close the table out
					Output.WriteLine(INDENT + "    <tr>");
					Output.WriteLine(INDENT + "      <td>&nbsp;</td>");
					Output.WriteLine(INDENT + "      <td colspan=\"3\" class=\"sbkDcv_TableFooter\">");

					CurrentMode.SubPage = (ushort) (table_number + 1);
					Output.WriteLine(INDENT + "        <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\" title=\"View additional details for this table\">view details</a> &nbsp; &nbsp; &nbsp;");

					CurrentMode.ViewerCode = "dsview";
					Output.WriteLine(INDENT + "        <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\" title=\"View the rows of data within this table\">view contents</a>");

					CurrentMode.ViewerCode = "dscodebook";

					Output.WriteLine(INDENT + "      </td>");
					Output.WriteLine(INDENT + "    </tr>");

					Output.WriteLine(INDENT + "  </table>");
					Output.WriteLine(INDENT + "</div>");
					Output.WriteLine(INDENT + "<br /><br />");
					Output.WriteLine();

					table_number++;
				}
			}
			else
			{
				Output.WriteLine("          <td id=\"sbkDcv_DetailsArea\">");
                int subpage_index = CurrentMode.SubPage.HasValue ? CurrentMode.SubPage.Value : 0;
                DataTable thisTable = itemDataset.Tables[subpage_index - 2];

				if (itemDataset.Tables.Count > 1)
				{
					ushort? subpage = CurrentMode.SubPage;
					CurrentMode.SubPage = 1;
					Output.WriteLine("<a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\" title=\"Back to full data structure\">&larr; Back</a><br /><br />");
					CurrentMode.SubPage = subpage;
				}
				else
				{
					Output.WriteLine("<br /><br />");
				}

				// Add the basic table information (if the word table is in the table name, don't include the 
				// word table in the title)
				if ( thisTable.TableName.IndexOf("table",StringComparison.OrdinalIgnoreCase) < 0 )
					Output.WriteLine(INDENT + "<div id=\"sbkDcv_DetailsTableName\"><span style=\"font-style:italic\">" + thisTable.TableName.Replace("_", " ") + "</span> Table Details</div>");
				else
					Output.WriteLine(INDENT + "<div id=\"sbkDcv_DetailsTableName\"><span style=\"font-style:italic\">" + thisTable.TableName.Replace("_", " ") + "</span> Details</div>");

				
				if (thisTable.ExtendedProperties.ContainsKey("Description"))
				{
					Output.WriteLine(INDENT + "<div id=\"sbkDcv_DetailsTableDescription\">" + thisTable.ExtendedProperties["Description"] + "</div>");
				}

				Output.WriteLine(INDENT + "  <table id=\"sbkDcv_TableDetails2\">");

				// Add the header row
				Output.WriteLine(INDENT + "    <tr>");
				Output.WriteLine(INDENT + "      <td>&nbsp;</td>");
				Output.WriteLine(INDENT + "      <td colspan=\"4\" class=\"sbkDcv_TableHeader\">" + thisTable.TableName.Replace("_", " ") + "</td>");
				Output.WriteLine(INDENT + "    </tr>");

				// Add each table's column information
				foreach (DataColumn thisColumn in thisTable.Columns)
				{
					string column_definition = String.Empty;
					string column_reference = "&nbsp;";
					if (thisColumn.Unique)
					{
						column_definition = thisColumn.AutoIncrement ? "PK" : "U";
					}
					foreach (Constraint constraint in thisTable.Constraints)
					{
						ForeignKeyConstraint fkConstraint = constraint as ForeignKeyConstraint;
						if (fkConstraint != null)
						{
							if (fkConstraint.Columns[0] == thisColumn)
							{
								column_definition = "FK";
								CurrentMode.SubPage = (ushort) (itemDataset.Tables.IndexOf(fkConstraint.RelatedColumns[0].Table) + 2);
								column_reference = "<a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\" title=\"View details of linked table\">" + fkConstraint.RelatedColumns[0].Table.TableName + "</a>." + fkConstraint.RelatedColumns[0].ColumnName + " <img src=\"" + Static_Resources.Leftarrow_Png + "\" alt=\"<--\" />";
							}
						}
					}

					if (thisColumn.AllowDBNull)
						Output.WriteLine(INDENT + "    <tr>");
					else
						Output.WriteLine(INDENT + "    <tr class=\"sbkDcv_TableRequiredField\">");


					Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableColumn0\">" + column_reference + "</td>");




					Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableDetailsColumn1\">" + column_definition + "</td>");
					Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableDetailsColumn2\">" + thisColumn.ColumnName.Replace("_", " ") + " &nbsp; </td>");

					string colunmType = thisColumn.DataType.ToString().Replace("System.", "").ToLower().Replace("int32", "integer");
					if ((colunmType == "string") && (thisColumn.MaxLength == 1))
						colunmType = "char";
					Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableDetailsColumn3\">" + colunmType + "</td>");
					Output.WriteLine(INDENT + "      <td class=\"sbkDcv_TableDetailsColumn4\">" + thisColumn.Caption.Replace("_", " ") + "</td>");
					Output.WriteLine(INDENT + "    </tr>");

					if (column_definition == "PK")
					{
						Output.WriteLine(INDENT + "    <tr><td></td><td colspan=\"4\" class=\"sbkDcv_TablePkRule\"></td></tr>");
					}
				}

				// Close the table out
				Output.WriteLine(INDENT + "    <tr>");
				Output.WriteLine(INDENT + "      <td>&nbsp;</td>");
				Output.WriteLine(INDENT + "      <td colspan=\"4\" class=\"sbkDcv_TableFooter\">");

				CurrentMode.ViewerCode = "dsview";
				Output.WriteLine(INDENT + "        <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\" title=\"View the rows of data within this table\">view contents</a>");

				CurrentMode.ViewerCode = "dscodebook";

				Output.WriteLine(INDENT + "      </td>");
				Output.WriteLine(INDENT + "    </tr>");

				Output.WriteLine(INDENT + "  </table>");

			}

			Output.WriteLine("          </td>");
		}
	}
}