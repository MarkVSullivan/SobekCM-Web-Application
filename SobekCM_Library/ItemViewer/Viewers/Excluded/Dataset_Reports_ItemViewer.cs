#region Using directives

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.BriefItem;
using SobekCM.Core.FileSystems;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Dataset viewer is used to view saved reports or create new custom reports </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer_OLD"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Dataset_Reports_ItemViewer : abstractItemViewer_OLD
	{
		private DataSet itemDataset;
        private string error_message;

        /// <summary> Constructor for a new instance of the Dataset_Reports_ItemViewer class </summary>
        public Dataset_Reports_ItemViewer()
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
                string xml_file = null;
                foreach (BriefItem_FileGrouping briefGroup in BriefItem.Downloads)
                {
                    foreach (BriefItem_File thisFile in briefGroup.Files)
                    {
                        if (thisFile.Name.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            xml_file = thisFile.Name;
                            break;
                        }

                    }
                }

                // If one was found, read it in!
                if (!String.IsNullOrEmpty(xml_file))
                {
                    itemDataset = new DataSet();
                    try
                    {
                        // Read the XML file
                        itemDataset.ReadXml(new StringReader(SobekFileSystem.ReadToEnd(BriefItem, xml_file)));
                    }
                    catch (Exception)
                    {
                        itemDataset = null;
                        error_message = "Error while reading XML file " + xml_file;
                    }


                    // Add this to the cache
                    HttpContext.Current.Cache.Insert(key, itemDataset, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
                }
            }
		}

		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.JPEG_Text_Two_Up"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.Dataset_Reports; }
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

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkDriv_Viewer' </value>
        public override string Viewer_CSS
        {
            get { return "sbkDriv_Viewer"; }
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
				Tracer.Add_Trace("Dataset_Reports_ItemViewer.Write_Main_Viewer_Section", "");
			}

			Output.WriteLine("          <td><div id=\"sbkDrv_ViewerTitle\">Data Reports</div></td>");
			Output.WriteLine("        </tr>");
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td id=\"sbkDrv_MainArea\">");
			
			Output.WriteLine("<h3>Build a Custom Report</h3>");

			Output.WriteLine(itemDataset.Tables.Count > 1 ? "<p>Select columns from the tables below to include in your query/report:</p>" : "<p>Select columns from the table below to include in your query/report:</p>");
			
			Output.WriteLine("<div id=\"sbkDrv_TableSelect\">");

			// Add the information about all the datatables
			const string INDENT = "          ";
			int table_number = 1;
			foreach (DataTable thisTable in itemDataset.Tables)
			{
				Output.WriteLine(INDENT + "<div id=\"ReportTable" + table_number + "\" class=\"sbkDrv_TableDiv\">");
				Output.WriteLine(INDENT + "  <table class=\"sbkDrv_Table\">");

				// Add the header row
				Output.WriteLine(INDENT + "    <tr>");
				Output.WriteLine(INDENT + "      <td colspan=\"2\" class=\"sbkDrv_TableHeader\">" + thisTable.TableName.Replace("_", " ") + "</td>");
				Output.WriteLine(INDENT + "    </tr>");

				// Add each table's column information
				int column_count = 1;
				foreach (DataColumn thisColumn in thisTable.Columns)
				{
					Output.WriteLine(INDENT + "    <tr>");
					Output.WriteLine(INDENT + "      <td class=\"sbkDrv_TableColumn1\"><input type=\"checkbox\" id=\"table" + table_number + "column" + column_count + "select\" name=\"table" + table_number + "column" + column_count + "select\" /></td>");

					Output.WriteLine(INDENT + "      <td class=\"sbkDrv_TableColumn2\"><label for=\"table" + table_number + "column" + column_count + "select\">" + thisColumn.ColumnName.Replace("_", " ") + "</label></td>");
					Output.WriteLine(INDENT + "    </tr>");
					column_count++;
				}

				// Add the footer row
				Output.WriteLine(INDENT + "    <tr>");
				Output.WriteLine(INDENT + "      <td colspan=\"2\" class=\"sbkDrv_TableFooter\"></td>");
				Output.WriteLine(INDENT + "    </tr>");

				// Close the table out
				Output.WriteLine(INDENT + "  </table>");
				Output.WriteLine(INDENT + "</div>");
				Output.WriteLine();

				table_number++;
			}


			Output.WriteLine("</div>");

			Output.WriteLine("<p>Enter custom search parameters below to narrow your results:</p>");
			Output.WriteLine("<textarea id=\"sbkDrv_SearchParams\"> </textarea>");

			Output.WriteLine("<p>Enter custom ordering parameters below to change the default order of the report:</p>");
			Output.WriteLine("<textarea id=\"sbkDrv_OrderParams\"> </textarea>");

			Output.WriteLine("<div id=\"sbkDrv_ButtonDiv\">");
			Output.WriteLine("<button title=\"Run custom query\" class=\"sbkIsw_RoundButton\" onclick=\"alert('This has not yet been implemented'); return false;\">Run Query</button> &nbsp; &nbsp; ");
			Output.WriteLine("<button title=\"Save this query\" class=\"sbkIsw_RoundButton\" onclick=\"alert('This has not yet been implemented'); return false;\">Save Query</button>");
			Output.WriteLine("</div>");

			Output.WriteLine("<h3>Saved Reports</h3>");
			Output.WriteLine("<p>Select a saved report below to view the current results:</p>");
			Output.WriteLine("<ul>");
			Output.WriteLine("<li><a href=\"\">Saved Report 1</a></li>");
			Output.WriteLine("<li><a href=\"\">Saved Report 2</a></li>");
			Output.WriteLine("<li><a href=\"\">Saved Report 3</a></li>");
			Output.WriteLine("</ul>");
			Output.WriteLine("          </td>");
		}
	}
}