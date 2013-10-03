#region using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Library.HTML;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Dataset viewer shows the paged data from the dataset and allows simple searching/filtering </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Dataset_ViewData_ItemViewer : abstractItemViewer
	{
		private DataSet itemDataset;
		private string error_message;

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
				string xml_file = (from thisFile in CurrentItem.Divisions.Download_Other_Files where thisFile.System_Name.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) > 0 select thisFile.System_Name).FirstOrDefault();

				// If one was found, read it in!
				if (!String.IsNullOrEmpty(xml_file))
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

		/// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
		/// <value> This returns -1, which allows this to use all the screen </value>
		public override int Viewer_Width
		{
			get
			{
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

			Output.WriteLine("          <td><div id=\"sbkDvd_ViewerTitle\">View Data</div></td>");
			Output.WriteLine("        </tr>");
			Output.WriteLine("        <tr>");


			// Was there an error getting the dataset?
			if ((itemDataset == null) || ( error_message.Length > 0 ))
			{
				Output.WriteLine("          <td id=\"sbkDvd_MainAreaError\">");
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

			// If only one datatable set the subpage
			if (itemDataset.Tables.Count == 1)
				CurrentMode.SubPage = 2;

			// Is the subpage invalid?
			if ((CurrentMode.SubPage - 1 > itemDataset.Tables.Count) || (CurrentMode.SubPage < 2))
			{
				Output.WriteLine("          <td id=\"sbkDvd_MainAreaError\">");
				if (error_message.Length > 0)
				{
					Output.WriteLine("            " + error_message);
				}
				else
				{
					Output.WriteLine("            Invalid table requested from dataset");
				}
				Output.WriteLine("          </td>");
				return;
			}

			// Get the datatable from the set
			DataTable thisTable = itemDataset.Tables[CurrentMode.SubPage - 2];
				 

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
				Output.WriteLine(INDENT + "        <th>" + thisColumn.ColumnName.Replace("_"," ") + "</th>");
				eachColumn.Add(thisColumn);
			}
			Output.WriteLine(INDENT + "      </tr>");
			Output.WriteLine(INDENT + "    </thead>");
			Output.WriteLine(INDENT + "    <tbody>");
			Output.WriteLine(INDENT + "      <tr><td colspan=\"" + ( eachColumn.Count + 1 ) + "\" class=\"dataTables_empty\">Loading data from server</td></tr>");

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
			Output.WriteLine("           \"sAjaxSource\": \"" + CurrentMode.Redirect_URL() + "\",");
			Output.Write    ("           \"aoColumns\": [{ \"bVisible\": false }");
			for (int i = 0; i < eachColumn.Count; i++)
				Output.Write(", null");
			Output.WriteLine("],");
			Output.WriteLine("           \"bAutoWidth\": false });");
			Output.WriteLine("  } );");
			Output.WriteLine("</script>");
			Output.WriteLine();

			Output.WriteLine(INDENT + "</td>");
			Output.WriteLine();

		}

		/// <summary> Write any additional values within the HTML Head of the final served page </summary>
		/// <param name="Output"> Output stream currently within the HTML head tags </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <remarks> Adds the javascript and styles needed for the jQuery datatables </remarks>
		public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
		{
			Output.WriteLine("  <link href=\"" + CurrentMode.Base_URL + "default/SobekCM_DataTables.css\" rel=\"stylesheet\" type=\"text/css\" />");
			Output.WriteLine("  <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/datatables/js/jquery.dataTables.js\" ></script>");

		}

	}
}
