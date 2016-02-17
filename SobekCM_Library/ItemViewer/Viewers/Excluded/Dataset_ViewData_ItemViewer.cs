#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.BriefItem;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Library.HTML;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Dataset viewer shows the paged data from the dataset and allows simple searching/filtering </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer_OLD"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Dataset_ViewData_ItemViewer : abstractItemViewer_OLD
	{
		private DataSet itemDataset;
		private string error_message;
		private int row;

		/// <summary> Constructor for a new instance of the Dataset_ViewData_ItemViewer class </summary>
		public Dataset_ViewData_ItemViewer()
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
				if (!String.IsNullOrEmpty( xml_file))
				{
					itemDataset = new DataSet();
					try
					{
						// Read the XML file
						itemDataset.ReadXml( new StringReader(SobekFileSystem.ReadToEnd( BriefItem, xml_file)));
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



			// Check the row data
			if ((itemDataset != null) && ( itemDataset.Tables.Count > 0 ))
			{
                int subpage_index = CurrentMode.SubPage.HasValue ? CurrentMode.SubPage.Value : 0;

				DataTable tbl = itemDataset.Tables[0];
                if ((subpage_index < itemDataset.Tables.Count + 2) && (subpage_index >= 2))
				{
                    tbl = itemDataset.Tables[subpage_index - 2];
				}

				row = -1;
				if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["row"]))
				{
					if (Int32.TryParse(HttpContext.Current.Request.QueryString["row"], out row))
					{
						if ((row < 1) || (row > tbl.Rows.Count))
						{
							row = -1;
						}
					}
				}
			}
		}

		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.JPEG_Text_Two_Up"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.Dataset_ViewData; }
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
        /// <value> This always returns the value 'sbkDvdiv_Viewer' or 'sbkDvdiv_ViewerFull' </value>
        public override string Viewer_CSS
        {
            get
            {
                // If this is to display a row, restrict width
                if (row > 0)
                    return "sbkDvdiv_Viewer";

                // We can show the left navigation bar if we aren't showing all the
                // data within a table so center the result
                if ((itemDataset == null) || (error_message.Length > 0) ||
                    ((itemDataset.Tables.Count > 1) && ((CurrentMode.SubPage < 2) || (CurrentMode.SubPage - 1 > itemDataset.Tables.Count))))
                {
                    return "sbkDvdiv_Viewer";
                }

                // Otherwise, suppress the left nav bar and go full screen
                return "sbkDvdiv_ViewerFull";
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This returns -1, which allows this to use all the screen </value>
        public override int Viewer_Width
		{
			get
			{
				// If this is to display a row, restrict width
				if (row > 0)
					return 800;

				// We can show the left navigation bar if we aren't showing all the
				// data within a table so center the result
				if ((itemDataset == null) || (error_message.Length > 0) ||
					((itemDataset.Tables.Count > 1) && ((CurrentMode.SubPage < 2) || (CurrentMode.SubPage - 1 > itemDataset.Tables.Count))))
				{
					return 800;
				}

				// Otherwise, suppress the left nav bar and go full screen
				return -1;
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

		/// <summary> Gets the collection of special behaviors which this item viewer
		/// requests from the main HTML subwriter. </summary>
		public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
		{
			get
			{
				// If this is to display a single row, no special behaviors
				if (row > 0)
					return emptybehaviors;

				// We can show the left navigation bar if we aren't showing all the
				// data within a table
				if ((itemDataset == null) || (error_message.Length > 0) ||
				    ((itemDataset.Tables.Count > 1) && ((CurrentMode.SubPage < 2) || (CurrentMode.SubPage - 1 > itemDataset.Tables.Count))))
				{
					return emptybehaviors;
				}

				// Otherwise, suppress the left nav bar
				return new List<HtmlSubwriter_Behaviors_Enum>
					{
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar
					};

			}
		}

		/// <summary> Stream to which to write the HTML for this subwriter  </summary>
		/// <param name="Output"> Response stream for the item viewer to write directly to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Dataset_ViewData_ItemViewer.Write_Main_Viewer_Section", "");
			}

			// Is this to display a single row?
			if (row > 0)
			{
				DataTable tbl = itemDataset.Tables[0];
                int subpage_index = CurrentMode.SubPage.HasValue ? CurrentMode.SubPage.Value : 0;
                if ((subpage_index < itemDataset.Tables.Count + 2) && (subpage_index >= 2))
				{
                    tbl = itemDataset.Tables[subpage_index - 2];
				}
				DataRow thisRow = tbl.Rows[row];

				Output.WriteLine("          <td>");
				Output.WriteLine("            <div id=\"sbkDvd_ViewerTitle\">View Single Row</div>");
				Output.WriteLine("          </td>");
				Output.WriteLine("        </tr>");
				Output.WriteLine("        <tr>");
				Output.WriteLine("          <td id=\"sbkDvd_MainAreaSingleRow\">");

				// find the back url
				string url = HttpContext.Current.Request.RawUrl.Replace("row=" + row, "");
				Output.WriteLine("<a href=\"" + url + "\" title=\"Back to results\">&larr; Back</a><br /><br />");


				Output.WriteLine("            <table class=\"sbkDvd_SingleRowTable\">");
				for (int i = 0; i < tbl.Columns.Count; i++)
				{
					Output.WriteLine("              <tr>");
					Output.WriteLine("                <td class=\"sbkDvd_SingleRowColumn1\">" + tbl.Columns[i].ColumnName.Replace("_", " ") + ":</td>");
					Output.WriteLine("                <td class=\"sbkDvd_SingleRowColumn2\">" + HttpUtility.HtmlEncode(thisRow[i]) + "</td>");
					Output.WriteLine("              <tr>");
				}
				Output.WriteLine("            </table>");

					Output.WriteLine("          </td>");
				Output.WriteLine();

			}
			else
			{

				Output.WriteLine("          <td>");
				Output.WriteLine("            <div id=\"sbkDvd_ViewerTitle\">View Data</div>");

				// Look for the search term and such from the current query string
				string term = String.Empty;
				string field = String.Empty;
				string[] possibles = {"col1", "col2", "col3", "col4", "col5", "col6", "col7", "col8", "col9", "col10", "col11", "col12", "col13", "col14", "col15", "col16", "col17", "col18", "col19", "col20"};
				foreach (string possibility in possibles)
				{
					if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString[possibility]))
					{
						field = possibility;
						term = HttpContext.Current.Request.QueryString[possibility];
						break;
					}
				}

				if ((itemDataset.Tables.Count == 1) || ((CurrentMode.SubPage < itemDataset.Tables.Count + 2) && (CurrentMode.SubPage >= 2)))
				{
					Output.WriteLine("            <div id=\"sbkDvd_SearchDiv\">Filter Results for ");
					Output.WriteLine("              <input type=\"textbox\" id=\"sbkDvd_SearchBox1\" name=\"bkDvd_SearchBox1\" value=\"" + HttpUtility.HtmlEncode(term) + "\" />");
					Output.WriteLine("              in");
					Output.WriteLine("              <select id=\"sbkDvd_Select1\" name=\"bkDvd_Select1\">");

					DataTable tbl = itemDataset.Tables[0];
                    int subpage_index = CurrentMode.SubPage.HasValue ? CurrentMode.SubPage.Value : 0;
                    if ((subpage_index < itemDataset.Tables.Count + 2) && (subpage_index >= 2))
					{
                        tbl = itemDataset.Tables[subpage_index - 2];
					}
					int column_count = 1;
					foreach (DataColumn thisColumn in tbl.Columns)
					{
						Output.Write("                <option value=\"col" + column_count + "\"");
						if ("col" + column_count == field)
							Output.Write(" selected=\"selected\"");
						Output.WriteLine(">" + thisColumn.ColumnName.Replace("_", " ") + "</option>");
						column_count++;
					}

					Output.WriteLine("              </select> &nbsp; ");
					Output.WriteLine("              <button title=\"Filter results\" id=\"sbkDvd_FilterButton\" class=\"sbkIsw_RoundButton\" onclick=\"data_search('" + UrlWriterHelper.Redirect_URL(CurrentMode) + "'); return false;\">GO<img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>");

					Output.WriteLine("            </div");
				}

				Output.WriteLine("          </td>");
				Output.WriteLine("        </tr>");
				Output.WriteLine("        <tr>");



				// Was there an error getting the dataset?
				if ((itemDataset == null) || (error_message.Length > 0))
				{
					Output.WriteLine("          <td id=\"sbkDvd_MainAreaError\">");
					if (error_message.Length > 0)
					{
						Output.WriteLine("            " + error_message);
					}
					else
					{
						Output.WriteLine("            No XML dataset found in the digital resource");
					}
					Output.WriteLine("          </td>");
					return;
				}

				// If only one datatable set the subpage
				if (itemDataset.Tables.Count == 1)
					CurrentMode.SubPage = 2;

				// Is the subpage invalid?
				if ((CurrentMode.SubPage - 1 > itemDataset.Tables.Count) || (CurrentMode.SubPage < 2))
				{
					Output.WriteLine("          <td id=\"sbkDvd_MainAreaInstructions\">");
					Output.WriteLine("            <p>This dataset has multiple tables.</p>");
					Output.WriteLine("            <p>Select a table below to view that data:</p>");
					Output.WriteLine("            <ul>");
					int table_number = 2;
					ushort? subpage = CurrentMode.SubPage;
					foreach (DataTable thisTableList in itemDataset.Tables)
					{
						CurrentMode.SubPage = (ushort) table_number;
						table_number++;
						Output.WriteLine("              <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\" title=\"View all rows from this table\">" + thisTableList.TableName.Replace("_", " ") + "</a> ( " + thisTableList.Rows.Count + " rows )</li>");
					}
					Output.WriteLine("            </ul>");

					CurrentMode.ViewerCode = "dscodebook";
					CurrentMode.SubPage = 0;
					Output.WriteLine("            <p>For more information about the structure of this dataset <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\" title=\"Vist the codebook\">view the data structure/codebook</a> for this dataset.</p>");
					CurrentMode.SubPage = subpage;
					CurrentMode.ViewerCode = "dsview";

					Output.WriteLine("            <br /><br />");
					Output.WriteLine("          </td>");
					return;
				}

				// Get the datatable from the set
                int subpage_index2 = CurrentMode.SubPage.HasValue ? CurrentMode.SubPage.Value : 0;
                DataTable thisTable = itemDataset.Tables[subpage_index2 - 2];


				// Start the main area
				const string INDENT = "          ";

				Output.WriteLine(INDENT + "<td id=\"sbkDvd_MainArea\">");

				Output.WriteLine(INDENT + "  <table id=\"sbkDvd_MainTable\">");
				Output.WriteLine(INDENT + "    <thead>");
				Output.WriteLine(INDENT + "      <tr>");
				Output.WriteLine(INDENT + "        <th>Row</th>");

				List<DataColumn> eachColumn = new List<DataColumn>();
				foreach (DataColumn thisColumn in thisTable.Columns)
				{
					Output.WriteLine(INDENT + "        <th>" + thisColumn.ColumnName.Replace("_", " ") + "</th>");
					eachColumn.Add(thisColumn);
				}
				Output.WriteLine(INDENT + "      </tr>");
				Output.WriteLine(INDENT + "    </thead>");
				Output.WriteLine(INDENT + "    <tbody>");
				Output.WriteLine(INDENT + "      <tr><td colspan=\"" + (eachColumn.Count + 1) + "\" class=\"dataTables_empty\">Loading data from server</td></tr>");

				//// Add all the row data
				//int row_number = 1;
				//foreach (DataRow thisRow in thisTable.Rows)
				//{
				//	Output.WriteLine(INDENT + "      <tr>");
				//	Output.WriteLine(INDENT + "        <td>" + row_number + "</td>");
				//	foreach (DataColumn thisColumn in eachColumn)
				//	{
				//		Output.WriteLine(INDENT + "        <td>" + HttpUtility.HtmlEncode(thisRow[thisColumn]) + "</td>");
				//	}
				//	Output.WriteLine(INDENT + "      </tr>");
				//	row_number++;
				//}

				Output.WriteLine(INDENT + "    </tbody>");
				Output.WriteLine(INDENT + "  </table>");

				CurrentMode.Writer_Type = Writer_Type_Enum.Data_Provider;

				Output.WriteLine();
				Output.WriteLine("<script type=\"text/javascript\">");
				Output.WriteLine("  jQuery(document).ready(function() {");
				Output.WriteLine("      var oTable = jQuery('#sbkDvd_MainTable').dataTable({");
				Output.WriteLine("           \"iDisplayLength\": 100,");
				//  Output.WriteLine("           \"aaSorting\": [[1, \"asc\"]],");
				Output.WriteLine("           \"bFilter\": false,");
				Output.WriteLine("           \"sDom\": 'ipRr<\"tablebuttonsdiv\"><\"tablescroll\"t>',");
				Output.WriteLine("           \"sPaginationType\": \"full_numbers\",");
				Output.WriteLine("           \"bProcessing\": true,");

				Output.WriteLine("           \"bServerSide\": true,");
				string redirect_url = UrlWriterHelper.Redirect_URL(CurrentMode);
				if ((field.Length > 0) && (term.Length > 0))
				{
					if (redirect_url.IndexOf("?") > 0)
						redirect_url = redirect_url + "&" + field + "=" + term;
					else
						redirect_url = redirect_url + "?" + field + "=" + term;
				}
				Output.WriteLine("           \"sAjaxSource\": \"" + redirect_url + "\",");
				Output.Write("           \"aoColumns\": [{ \"bVisible\": false }");
				for (int i = 0; i < eachColumn.Count; i++)
					Output.Write(", null");
				Output.WriteLine("],");
				Output.WriteLine("           \"bAutoWidth\": false });");
				Output.WriteLine();
				Output.WriteLine("    jQuery('#sbkDvd_MainTable tbody').delegate('tr','click', function(ev) {");

				Output.WriteLine("      dataset_rowselected(this);");
				Output.WriteLine("      ev.preventDefault();");
				Output.WriteLine("      ev.stopPropagation();");
				Output.WriteLine("    });");
				Output.WriteLine("  });");
				Output.WriteLine("</script>");
				Output.WriteLine();

				Output.WriteLine(INDENT + "</td>");
				Output.WriteLine();
			}
		}

		/// <summary> Write any additional values within the HTML Head of the final served page </summary>
		/// <param name="Output"> Output stream currently within the HTML head tags </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <remarks> Adds the javascript and styles needed for the jQuery datatables </remarks>
		public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
		{
			Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Datatables_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
			Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Datatables_Js + "\" ></script>");

		}

	}
}
