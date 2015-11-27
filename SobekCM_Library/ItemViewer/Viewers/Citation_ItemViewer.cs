#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Resource_Object.Metadata_Modules.LearningObjects;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Item viewer displays the citation information including the basic metadata in standard and MARC format, as well as
	/// links to the metadata </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Citation_ItemViewer : abstractItemViewer
	{
		private Citation_Type citationType;
	  //  private User_Object currentUser;
	    private bool userCanEditItem;
		private int width = 180;

		/// <summary> Constructor for a new instance of the Citation_ItemViewer class </summary>
		/// <param name="Translator"> Language support object which handles simple translational duties </param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
		/// <param name="User_Can_Edit_Item"> Flag indicates if the current user can edit the citation information </param>
		public Citation_ItemViewer(Language_Support_Info Translator, Aggregation_Code_Manager Code_Manager, bool User_Can_Edit_Item )
		{
			translator = Translator;
			this.Code_Manager = Code_Manager;
			userCanEditItem = User_Can_Edit_Item;
			citationType = Citation_Type.Standard;
		}

		/// <summary> Constructor for a new instance of the Citation_ItemViewer class </summary>
		public Citation_ItemViewer( )
		{
			userCanEditItem = false;
		}

		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Citation"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.Citation; }
		}

	    /// <summary> Flag indicates the user is somewhat restricted in use so the metadata and
	    /// usage statistics will be suppressed </summary>
	    public bool Item_Restricted { private get; set; }


	    /// <summary>List of valid collection codes, including mapping from the Sobek collections to Greenstone collections </summary>
	    public Aggregation_Code_Manager Code_Manager { get; set; }

        ///// <summary> Currently logged on user for determining rights over this item </summary>
        //public User_Object Current_User
        //{
        //    set 
        //    {
        //        if (value == null) return;

        //        userCanEditItem = value.Can_Edit_This_Item(CurrentItem.BibID, CurrentItem.Bib_Info.SobekCM_Type_String, CurrentItem.Bib_Info.Source.Code, CurrentItem.Bib_Info.HoldingCode, CurrentItem.Behaviors.Aggregation_Code_List); ;
        //        currentUser = value;
        //    }
        //}

		/// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
		/// <value> This value depends on the current submode being displayed (i.e., MARC, metadata links, etc..) </value>
		public override int Viewer_Width
		{
			get
			{
				switch (CurrentMode.ViewerCode)
				{
					case "citation4":
						return -1;

					case "usage":
						return 950;

					default:
						return 780;
				}
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

        

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
                Tracer.Add_Trace("Citation_ItemViewer.Write_Main_Viewer_Section", "Write the citation information directly to the output stream");
			}

            // Determine if user can edit
            userCanEditItem = false;
            if (CurrentUser != null)
            {
                userCanEditItem = CurrentUser.Can_Edit_This_Item(CurrentItem.BibID, CurrentItem.Bib_Info.SobekCM_Type_String, CurrentItem.Bib_Info.Source.Code, CurrentItem.Bib_Info.HoldingCode, CurrentItem.Behaviors.Aggregation_Code_List); ;
            }

            // Add the HTML for the citation
            Output.WriteLine("        <!-- CITATION ITEM VIEWER OUTPUT -->");
            Output.WriteLine("        <td>");

            // If this is DARK and the user cannot edit and the flag is not set to show citation, show nothing here
            if ((CurrentItem.Behaviors.Dark_Flag) && (!userCanEditItem) && (!UI_ApplicationCache_Gateway.Settings.Resources.Show_Citation_For_Dark_Items))
            {
                Output.WriteLine("          <div id=\"darkItemSuppressCitationMsg\">This item is DARK and cannot be viewed at this time</div>" + Environment.NewLine + "</td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");

                return;
            }

			// Determine the citation type
			citationType = Citation_Type.Standard;
			switch (CurrentMode.ViewerCode)
			{
				case "marc":
					citationType = Citation_Type.MARC;
					break;

				case "metadata":
					citationType = Citation_Type.Metadata;
					break;

				case "usage":
					citationType = Citation_Type.Statistics;
					break;
			}

			// Restricted users can only see the MARC or the standard
			if (Item_Restricted)
			{
				if ((citationType != Citation_Type.Standard) && (citationType != Citation_Type.MARC))
					citationType = Citation_Type.Standard;
			}


			// Get  the robot flag (if this is rendering for robots, the other citation views are not available)
			bool isRobot = CurrentMode.Is_Robot;

			string viewer_code = CurrentMode.ViewerCode;

			// Get any search terms
			List<string> terms = new List<string>();
			if ( !String.IsNullOrWhiteSpace(CurrentMode.Text_Search))
			{
			    string[] splitter = CurrentMode.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
			    terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
			}

			// Add the main wrapper division
			if ( citationType != Citation_Type.Standard )
				Output.WriteLine("<div id=\"sbkCiv_Citation\">");
			else
			{
				// Determine the material type
				string microdata_type = "CreativeWork";
				switch (CurrentItem.Bib_Info.SobekCM_Type)
				{
					case TypeOfResource_SobekCM_Enum.Book:
					case TypeOfResource_SobekCM_Enum.Serial:
					case TypeOfResource_SobekCM_Enum.Newspaper:
						microdata_type = "Book";
						break;

					case TypeOfResource_SobekCM_Enum.Map:
						microdata_type = "Map";
						break;

					case TypeOfResource_SobekCM_Enum.Photograph:
					case TypeOfResource_SobekCM_Enum.Aerial:
						microdata_type = "Photograph";
						break;
				}

				// Add the main wrapper division, with microdata information
				Output.WriteLine("<div id=\"sbkCiv_Citation\" itemprop=\"about\" itemscope itemtype=\"http://schema.org/" + microdata_type + "\">");
			}

			if ( !CurrentMode.Is_Robot )
				Add_Citation_View_Tabs(Output);

		    // Now, add the text
            Output.WriteLine();
			switch (citationType)
			{
				case Citation_Type.Standard:
					if ( terms.Count > 0 )
					{
                        Output.WriteLine(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(Standard_Citation_String(!isRobot, Tracer), terms, "<span class=\"sbkCiv_TextHighlight\">", "</span>") + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					}
					else
					{
                        Output.WriteLine(Standard_Citation_String(!isRobot, Tracer) + Environment.NewLine + "  </td>" + Environment.NewLine + "  <div id=\"sbkCiv_EmptyRobotDiv\" />" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					}
					break;

				case Citation_Type.MARC:
					if ( terms.Count > 0 )
					{
                        Output.WriteLine(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(MARC_String(Tracer), terms, "<span class=\"sbkCiv_TextHighlight\">", "</span>") + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					}
					else
					{
                        Output.WriteLine( MARC_String(Tracer) + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					}
					break;

				case Citation_Type.Metadata:
                    Output.WriteLine( Metadata_String( Tracer ) + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					break;

				case Citation_Type.Statistics:
                    Output.WriteLine( Statistics_String(Tracer) + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					break;
			}

			CurrentMode.ViewerCode = viewer_code;
		}

		private void Add_Citation_View_Tabs(TextWriter Output)
		{
			// Set the text
			const string STANDARD_VIEW = "STANDARD VIEW";
			const string MARC_VIEW = "MARC VIEW";
			const string METADATA_VIEW = "METADATA";
			const string STATISTICS_VIEW = "USAGE STATISTICS";

			// Get  the robot flag (if this is rendering for robots, the other citation views are not available)
			bool isRobot = CurrentMode.Is_Robot;

			// Add the tabs for the different citation information
			string viewer_code = CurrentMode.ViewerCode;
			Output.WriteLine("  <div id=\"sbkCiv_ViewSelectRow\">");
			Output.WriteLine("    <ul class=\"sbk_FauxDownwardTabsList\">");


			if (citationType == Citation_Type.Standard)
			{
				Output.WriteLine("      <li class=\"current\">" + STANDARD_VIEW + "</li>");
			}
			else
			{
				Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode, "citation") + "\">" + STANDARD_VIEW + "</a></li>");
			}

			if (citationType == Citation_Type.MARC)
			{
				Output.WriteLine("      <li class=\"current\">" + MARC_VIEW + "</li>");
			}
			else
			{
				if (!isRobot)
					Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode, "marc") + "\">" + MARC_VIEW + "</a></li>");
				else
					Output.WriteLine("      <li>" + MARC_VIEW + "</li>");
			}

			// If this item is an external link item (i.e. has related URL, but no pages or downloads) skip the next parts
			bool external_link_only = (CurrentItem.Bib_Info.Location.Other_URL.Length > 0) && (!CurrentItem.Divisions.Has_Files);

			if ((CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL) && (!external_link_only) && (!isRobot))
			{
				if (citationType == Citation_Type.Metadata)
				{
					Output.WriteLine("      <li class=\"current\">" + METADATA_VIEW + "</li>");
				}
				else
				{
					Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode, "metadata") + "\">" + METADATA_VIEW + "</a></li>");
				}

				if (citationType == Citation_Type.Statistics)
				{
					Output.WriteLine("      <li class=\"current\">" + STATISTICS_VIEW + "</li>");
				}
				else
				{
					Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode, "usage") + "\">" + STATISTICS_VIEW + "</a></li>");
				}
			}

			Output.WriteLine("    </ul>");
			Output.WriteLine("  </div>");
		}

		#region Section returns the item level statistics

		/// <summary> Returns the string which contains the item and title level statistics </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Sttring with the statistical usage information for this item and title</returns>
		protected string Statistics_String( Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Citation_ItemViewer.Statistics_String", "Create the statistics html");
			}

			int hits = 0;
			int sessions = 0;
			int jpeg_views = 0;
			int zoom_views = 0;
			int thumb_views = 0;
			int flash_views = 0;
			int google_map_views = 0;
			int download_views = 0;
			int citation_views = 0;
			int text_search_views = 0;
			int static_views = 0;

			// Pull the item statistics
			DataSet stats = SobekCM_Database.Get_Item_Statistics_History(CurrentItem.BibID, CurrentItem.VID, Tracer);
			
			StringBuilder builder = new StringBuilder(2000);

			builder.AppendLine("  <p>This item was has been viewed <%HITS%> times within <%SESSIONS%> visits.  Below are the details for overall usage for this item within this library.<br /><br />For definitions of these terms, see the <a href=\"" + CurrentMode.Base_URL + "stats/usage/definitions\" target=\"_BLANK\">definitions on the main statistics page</a>.</p>");

            builder.AppendLine("  <table class=\"sbkCiv_StatsTable\">");
			builder.AppendLine("    <tr class=\"sbkCiv_StatsTableHeaderRow\">");
			builder.AppendLine("      <th style=\"width:120px\">Date</th>");
            builder.AppendLine("      <th style=\"width:90px\">Views</th>");
			builder.AppendLine("      <th style=\"width:90px\">Visits</th>");
            builder.AppendLine("      <th style=\"width:90px\">JPEG<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Zoomable<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Citation<br />Views</th>");
			builder.AppendLine("      <th style=\"width:90px\">Thumbnail<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Text<br />Searches</th>");
            builder.AppendLine("      <th style=\"width:90px\">Flash<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Map<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Download<br />Views</th>");
			builder.AppendLine("      <th style=\"width:90px\">Static<br />Views</th>");
			builder.AppendLine("    </tr>");

			const int COLUMNS = 12;
			string last_year = String.Empty;
			if (stats != null)
			{
				foreach (DataRow thisRow in stats.Tables[1].Rows)
				{
					if (thisRow["Year"].ToString() != last_year)
					{
                        builder.AppendLine("    <tr><td class=\"sbkCiv_StatsTableYearRow\" colspan=\"" + COLUMNS + "\">" + thisRow["Year"] + " STATISTICS</td></tr>");
						last_year = thisRow["Year"].ToString();
					}
					else
					{
						builder.AppendLine("    <tr><td class=\"sbkCiv_StatsTableRowSeperator\" colspan=\"" + COLUMNS + "\"></td></tr>");
					}
					builder.AppendLine("    <tr>");
					builder.AppendLine("      <td style=\"text-align: left\">" + Month_From_Int(Convert.ToInt32(thisRow["Month"])) + " " + thisRow["Year"] + "</td>");

					if (thisRow[5] != DBNull.Value)
					{
						hits += Convert.ToInt32(thisRow[5]);
						builder.AppendLine("      <td>" + thisRow[5] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}

					if (thisRow[6] != DBNull.Value)
					{
						sessions += Convert.ToInt32(thisRow[6]);
						builder.AppendLine("      <td>" + thisRow[6] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}

					if (thisRow[7] != DBNull.Value)
					{
						jpeg_views += Convert.ToInt32(thisRow[10]);
						builder.AppendLine("      <td>" + thisRow[7] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					if (thisRow[8] != DBNull.Value)
					{
						zoom_views += Convert.ToInt32(thisRow[8]);
						builder.AppendLine("      <td>" + thisRow[8] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					if (thisRow[9] != DBNull.Value)
					{
						citation_views += Convert.ToInt32(thisRow[9]);
						builder.AppendLine("      <td>" + thisRow[9] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					if (thisRow[10] != DBNull.Value)
					{
						thumb_views += Convert.ToInt32(thisRow[10]);
						builder.AppendLine("      <td>" + thisRow[10] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					if (thisRow[11] != DBNull.Value)
					{
						text_search_views += Convert.ToInt32(thisRow[11]);
						builder.AppendLine("      <td>" + thisRow[11] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					if (thisRow[12] != DBNull.Value)
					{
						flash_views += Convert.ToInt32(thisRow[12]);
						builder.AppendLine("      <td>" + thisRow[12] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					if (thisRow[13] != DBNull.Value)
					{
						google_map_views += Convert.ToInt32(thisRow[13]);
						builder.AppendLine("      <td>" + thisRow[13] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					if (thisRow[14] != DBNull.Value)
					{
						download_views += Convert.ToInt32(thisRow[14]);
						builder.AppendLine("      <td>" + thisRow[14] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					if (thisRow[15] != DBNull.Value)
					{
						static_views += Convert.ToInt32(thisRow[15]);
						builder.AppendLine("      <td>" + thisRow[15] + "</td>");
					}
					else
					{
						builder.AppendLine("      <td>0</td>");
					}
					builder.AppendLine("    </tr>");
				}

                builder.AppendLine("    <tr><td class=\"sbkCiv_StatsTableFinalSeperator\" colspan=\"" + COLUMNS + "\"></td></tr>");
                builder.AppendLine("    <tr id=\"sbkCiv_StatsTableTotalRow\" >");
				builder.AppendLine("      <td style=\"text-align:left\">TOTAL</td>");
				builder.AppendLine("      <td>" + hits + "</td>");
				builder.AppendLine("      <td>" + sessions + "</td>");
				builder.AppendLine("      <td>" + jpeg_views + "</td>");
				builder.AppendLine("      <td>" + zoom_views + "</td>");
				builder.AppendLine("      <td>" + citation_views + "</td>");
				builder.AppendLine("      <td>" + thumb_views + "</td>");
				builder.AppendLine("      <td>" + text_search_views + "</td>");
				builder.AppendLine("      <td>" + flash_views + "</td>");
				builder.AppendLine("      <td>" + google_map_views + "</td>");
				builder.AppendLine("      <td>" + download_views + "</td>");
				builder.AppendLine("      <td>" + static_views + "</td>");
				builder.AppendLine("    </tr>");
				builder.AppendLine("  </table>");
				builder.AppendLine("  <br />");
			}

			builder.AppendLine("</div>");
			return builder.ToString().Replace("<%HITS%>", hits.ToString()).Replace("<%SESSIONS%>", sessions.ToString());

		}

		private static string Month_From_Int(int Month_Int)
		{
			string monthString1 = "Invalid";
			switch (Month_Int)
			{
				case 1:
					monthString1 = "January";
					break;

				case 2:
					monthString1 = "February";
					break;

				case 3:
					monthString1 = "March";
					break;

				case 4:
					monthString1 = "April";
					break;

				case 5:
					monthString1 = "May";
					break;

				case 6:
					monthString1 = "June";
					break;

				case 7:
					monthString1 = "July";
					break;

				case 8:
					monthString1 = "August";
					break;

				case 9:
					monthString1 = "September";
					break;

				case 10:
					monthString1 = "October";
					break;

				case 11:
					monthString1 = "November";
					break;

				case 12:
					monthString1 = "December";
					break;
			}
			return monthString1;
		}

		#endregion

		#region Section returns the metadata tab explanation and links

		/// <summary> Returns the string which contains the metadata links and basic information about the types of metadata</summary>
		/// <returns> Sttring with the metadata links and basic information about the types of metadata</returns>
		protected string Metadata_String( Custom_Tracer Tracer )
		{
			// Get the links for the METS and GSA
			string resourceURL = CurrentItem.Web.Source_URL + "/";
            string complete_mets = resourceURL + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";
            string marc_xml = resourceURL + "marc.xml";

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((CurrentItem.Behaviors.Dark_Flag) || (CurrentItem.Behaviors.IP_Restriction_Membership > 0))
            {
                resourceURL = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/";
                complete_mets = resourceURL + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";
                marc_xml = resourceURL + "marc.xml";
            }


		    StringBuilder builder = new StringBuilder(3000);

			builder.AppendLine("<blockquote>");
            builder.AppendLine("<p>The data (or metadata) about this digital resource is available in a variety of metadata formats. For more information about these formats, see the <a href=\"http://ufdc.ufl.edu/sobekcm/metadata\">Metadata Section</a> of the <a href=\"http://ufdc.ufl.edu/sobekcm/\">Technical Aspects</a> information.</p>");
            builder.AppendLine("<br />");

            if (CurrentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.EAD)
            {
                string ead_file = String.Empty;
                List<abstract_TreeNode> downloadPages = CurrentItem.Divisions.Download_Tree.Pages_PreOrder;
                foreach (Page_TreeNode downloadPage in downloadPages)
                {
                    // Was this an EAD page?
                    if ((downloadPage.Label == "EAD") && (downloadPage.Files.Count == 1))
                    {
                        if (downloadPage.Files[0].System_Name.ToLower().IndexOf(".xml") > 0)
                        {
                            ead_file = downloadPage.Files[0].System_Name;
                            break;
                        }
                    }
                }

                if (ead_file.Length > 0)
                {
                    builder.AppendLine("<div id=\"sbkCiv_EadDownload\" class=\"sbCiv_DownloadSection\">");
                    builder.AppendLine("  <a href=\"" + resourceURL + ead_file + "\" target=\"_blank\">View Finding Aid (EAD)</a>");
                    builder.AppendLine("  <p>This archival collection is described with an electronic finding aid.   This metadata file contains all of the archival description and container list for this archival material.  This file follows the established <a href=\"http://www.loc.gov/ead/\">Encoded Archival Description</a> (EAD) standard.</p>");
                    builder.AppendLine("</div>");
                }
            }

		    builder.AppendLine("<div id=\"sbkCiv_MetsDownload\" class=\"sbCiv_DownloadSection\">");
            builder.AppendLine("  <a href=\"" + complete_mets + "\" target=\"_blank\">View Complete METS/MODS</a>");
            builder.AppendLine("  <p>This metadata file is the source metadata file submitted along with all the digital resource files. This contains all of the citation and processing information used to build this resource. This file follows the established <a href=\"http://www.loc.gov/standards/mets/\">Metadata Encoding and Transmission Standard</a> (METS) and <a href=\"http://www.loc.gov/standards/mods/\">Metadata Object Description Schema</a> (MODS). This METS/MODS file was just read when this item was loaded into memory and used to display all the information in the standard view and marc view within the citation.</p>");
		    builder.AppendLine("</div>");

            builder.AppendLine("<div id=\"sbkCiv_MarcXmlDownload\" class=\"sbCiv_DownloadSection\">");
            builder.AppendLine("  <a href=\"" + marc_xml + "\" target=\"_blank\">View MARC XML File</a>");
            builder.AppendLine("  <p>The entered metadata is also converted to MARC XML format, for interoperability with other library catalog systems.  This represents the same data available in the <a href=\"" + UI_ApplicationCache_Gateway.Settings.Servers.Base_SobekCM_Location_Relative + UrlWriterHelper.Redirect_URL(CurrentMode, "FC2") + "\">MARC VIEW</a> except this is a static XML file.  This file follows the <a href=\"http://www.loc.gov/standards/marcxml/\">MarcXML Schema</a>.</p>");
		    builder.AppendLine("</div>");

            // Should the TEI be added here?

            if (CurrentItem.Behaviors.Has_Viewer_Type(View_Enum.TEI))
            {
                // Does a TEI file exist?
	            string tei_filename = CurrentItem.Source_Directory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".tei.xml";


                if (!File.Exists(tei_filename))
                {

                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("Citation_ItemViewer.Metadata_String", "Building default TEI file");
                    }

                    TEI_File_ReaderWriter writer = new TEI_File_ReaderWriter();

                    Dictionary<string, object> options = new Dictionary<string, object>();

	                string error_message;
	                writer.Write_Metadata(tei_filename, CurrentItem, options, out error_message);
                }

                // Add the HTML for this
                builder.AppendLine("<div id=\"sbkCiv_TeiDownload\" class=\"sbCiv_DownloadSection\">");
                builder.AppendLine("  <a href=\"" + resourceURL + CurrentItem.BibID + "_" + CurrentItem.VID + ".tei.xml\" target=\"_blank\">View TEI/Text File</a>");
                builder.AppendLine("  <p>The full-text of this item is also available in the established standard <a href=\"http://www.tei-c.org/index.xml\">Text Encoding Initiative</a> (TEI) downloadable file.</p>");
                builder.AppendLine("</div>");

            }

			builder.AppendLine("</blockquote><br />");
			builder.AppendLine("</div>");

			return builder.ToString();

		}

		#endregion

		#region Code to generate the MARC record in HTML

		/// <summary> Returns the basic information about this digital resource in MARC HTML format </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> HTML string with the MARC information for this digital resource </returns>
		/// <remarks> The width statement for this rendering defaults to 95% of width</remarks>
		public string MARC_String(Custom_Tracer Tracer)
		{
			return MARC_String( "95%", Tracer);
		}

		/// <summary> Returns the basic information about this digital resource in MARC HTML format </summary>
		/// <param name="Width"> Width statement to be included in the MARC21 table </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> HTML string with the MARC information for this digital resource </returns>
		public string MARC_String( string Width, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Citation_ItemViewer.MARC_String", "Configuring METS data into MARC format");
			}

			//// Build the value
			StringBuilder builder = new StringBuilder();

			// Add the edit item button, if the user can edit it
			if ((userCanEditItem) && (CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL))
			{
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
				CurrentMode.My_Sobek_SubMode = "1";
				builder.AppendLine("<blockquote><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin_Or_Skin + "/buttons/edit_item_button.gif\" border=\"0px\" alt=\"Edit this item\" /></a></blockquote>");
				CurrentMode.Mode = Display_Mode_Enum.Item_Display;
			}
			else
			{
                builder.AppendLine("<br />");
			}

            // Create the options dictionary used when saving information to the database, or writing MarcXML
            Dictionary<string, object> options = new Dictionary<string, object>();
            if (UI_ApplicationCache_Gateway.Settings.MarcGeneration != null)
            {
                options["MarcXML_File_ReaderWriter:MARC Cataloging Source Code"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.Cataloging_Source_Code;
                options["MarcXML_File_ReaderWriter:MARC Location Code"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.Location_Code;
                options["MarcXML_File_ReaderWriter:MARC Reproduction Agency"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Agency;
                options["MarcXML_File_ReaderWriter:MARC Reproduction Place"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Place;
                options["MarcXML_File_ReaderWriter:MARC XSLT File"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.XSLT_File;
            }
            options["MarcXML_File_ReaderWriter:System Name"] = UI_ApplicationCache_Gateway.Settings.System.System_Name;
            options["MarcXML_File_ReaderWriter:System Abbreviation"] = UI_ApplicationCache_Gateway.Settings.System.System_Abbreviation;


            builder.AppendLine(CurrentItem.Get_MARC_HTML( options, Width ));

            builder.AppendLine("<br />");
            builder.AppendLine("<br />");
            builder.AppendLine("<div id=\"sbkCiv_MarcAutoGenerated\">The record above was auto-generated from the METS file.</div>");
		    builder.AppendLine();
            builder.AppendLine("<br />");
            builder.AppendLine("<br />");
			builder.AppendLine("</div>");

			return builder.ToString();
		}

		#endregion

		#region Code to create the regular citation string

		/// <summary> Returns the basic information about this digital resource in standard format </summary>
		/// <param name="Include_Links"> Flag tells whether to include the search links from this citation view </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> HTML string with the basic information about this digital resource for display </returns>
		public string Standard_Citation_String(bool Include_Links, Custom_Tracer Tracer)
		{
		    bool internalUser = ((CurrentUser != null) && (CurrentUser.LoggedOn) && (CurrentUser.Is_Internal_User));

            // Retrieve all the metadata modules that wil be used here
		    Zoological_Taxonomy_Info taxonInfo = CurrentItem.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
		    VRACore_Info vraInfo = CurrentItem.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
		    GeoSpatial_Information geoInfo = CurrentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
		    LearningObjectMetadata lomInfo = CurrentItem.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
			Thesis_Dissertation_Info thesisInfo = CurrentItem.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
            RightsMD_Info rightsInfo = CurrentItem.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;

			// Compute the URL to use for all searches from the citation
			Display_Mode_Enum lastMode = CurrentMode.Mode;
			CurrentMode.Mode = Display_Mode_Enum.Results;
			CurrentMode.Search_Type = Search_Type_Enum.Advanced;
			CurrentMode.Search_String = "<%VALUE%>";
			CurrentMode.Search_Fields = "<%CODE%>";
			string search_link = "<a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode).Replace("&", "&amp;").Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>", "&quot;<%VALUE%>&quot;") + "\" target=\"_BLANK\">";
			string search_link_end = "</a>";
			CurrentMode.Aggregation = String.Empty;
			CurrentMode.Search_String = String.Empty;
			CurrentMode.Search_Fields = String.Empty;
			CurrentMode.Mode = lastMode;
			string url_options = UrlWriterHelper.URL_Options(CurrentMode);
			if (url_options.Length > 0)
			{
				url_options = "?" + url_options;
			}

			// If no search links should should be included, clear the search strings
			if (!Include_Links)
			{
				search_link = String.Empty;
				search_link_end = String.Empty;
			}


		    if ((CurrentMode.Language == Web_Language_Enum.French) || (CurrentMode.Language == Web_Language_Enum.Spanish))
				width = 230;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Citation_ItemViewer.MARC_String", "Configuring METS data into standard citation format");
			}

			// Build the strings for each section
			string mets_info = translator.Get_Translation("METS Information", CurrentMode.Language);
			string internal_info = translator.Get_Translation(CurrentMode.Instance_Abbreviation + " Membership", CurrentMode.Language);
			string biblio_info = translator.Get_Translation("Material Information", CurrentMode.Language);
			string subject_info = translator.Get_Translation("Subjects", CurrentMode.Language);
			string notes_info = translator.Get_Translation("Notes", CurrentMode.Language);
			string institutional_info = translator.Get_Translation("Record Information", CurrentMode.Language);
			string related_items = translator.Get_Translation("Related Items", CurrentMode.Language);
			string system_info = translator.Get_Translation("System Admin Information", CurrentMode.Language);
			string zoological_taxonomy = translator.Get_Translation("Zoological Taxonomic Information", CurrentMode.Language);
            string learningobject_title = translator.Get_Translation("Learning Resource Information", CurrentMode.Language);
			string thesis_title = translator.Get_Translation("Thesis/Dissertation Information", CurrentMode.Language);

			List<string> tempList = new List<string>();

			// Build the value
			// Use string builder to build this
			const string INDENT = "    ";

			StringBuilder result = new StringBuilder();

			// Start this table
			result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_LinkSection\" >");
			result.AppendLine(INDENT + "  <dl>");

			// If this item is an external link item (i.e. has related URL, but no pages or downloads) the PURL should display as the Other URL
			bool external_link_only = (CurrentItem.Bib_Info.Location.Other_URL.Length > 0) && (!CurrentItem.Divisions.Has_Files);

			// Add major links first
			if (external_link_only)
			{
				string url_shortened = CurrentItem.Bib_Info.Location.Other_URL;
				if (url_shortened.Length > 200)
					url_shortened = url_shortened.Substring(0, 200) + "...";
				result.Append(Single_Citation_HTML_Row("External Link", "<a href=\"" + CurrentItem.Bib_Info.Location.Other_URL.Replace("&", "&amp;") + "\">" + url_shortened.Replace("&", "&amp;") + "</a>", INDENT));
			}
			else
			{
				// Does this have a PURL in the METS package?
				if (CurrentItem.Bib_Info.Location.PURL.Length > 0)
				{
					string packagePurl = CurrentItem.Bib_Info.Location.PURL;
					if (packagePurl.IndexOf("http://") < 0)
						packagePurl = "http://" + packagePurl;
					result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + packagePurl + "\"><span itemprop=\"url\">" + packagePurl + "</span></a>", INDENT));
				}
				else
				{
					if (CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL)
					{
						// Does thir portal have a special PURL to be used here?
						if (!String.IsNullOrEmpty(CurrentMode.Portal_PURL))
						{
							string link = CurrentMode.Portal_PURL + CurrentItem.BibID + "/" + CurrentItem.VID;
							result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + link + "\"><span itemprop=\"url\">" + link + "</span></a>", INDENT));
						}
						else
						{
							string link = CurrentMode.Base_URL + CurrentItem.BibID + "/" + CurrentItem.VID;
							result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + link + "\"><span itemprop=\"url\">" + link + "</span></a>", INDENT));
						}
					}
					else
					{
						// Does thir portal have a special PURL to be used here?
						if (!String.IsNullOrEmpty(CurrentMode.Portal_PURL))
						{
							string link = CurrentMode.Portal_PURL + CurrentItem.BibID;
							result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + link + "\"><span itemprop=\"url\">" + link + "</span></a>", INDENT));
						}
						else
						{
							string link = CurrentMode.Base_URL + CurrentItem.BibID;
							result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + link + "\"><span itemprop=\"url\">" + link + "</span></a>", INDENT));
						}
					}
				}
			}

			// If there is an EAD link, link to that
			if ((CurrentItem.Bib_Info.hasLocationInformation) && (CurrentItem.Bib_Info.Location.EAD_Name.Length > 0) || (CurrentItem.Bib_Info.Location.EAD_URL.Length > 0))
			{
				if (CurrentItem.Bib_Info.Location.EAD_Name.Length == 0)
				{
					result.Append(Single_Citation_HTML_Row("Finding Guide", "<a href=\"" + CurrentItem.Bib_Info.Location.EAD_URL + "\">" + CurrentItem.Bib_Info.Location.EAD_URL + "</a>", INDENT));
				}
				else
				{
				    result.Append(CurrentItem.Bib_Info.Location.EAD_URL.Length > 0
				                      ? Single_Citation_HTML_Row("Finding Guide","<a href=\"" + CurrentItem.Bib_Info.Location.EAD_URL + "\">" +Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.EAD_Name) +"</a>", INDENT)
				                      : Single_Citation_HTML_Row("Finding Guide",Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.EAD_Name),INDENT));
				}
			}

			result.AppendLine(INDENT + "  </dl>");
            result.AppendLine(INDENT + "</div>");

			// Now, try to add the thumbnail from any page images here
		    if (CurrentItem.Behaviors.Dark_Flag != true)
		    {
		        if (!String.IsNullOrEmpty(CurrentItem.Behaviors.Main_Thumbnail))
		        {
		            string name_for_image = HttpUtility.HtmlEncode(CurrentItem.Bib_Info.Main_Title.ToString());
		            result.AppendLine();
                    result.AppendLine(INDENT + "<div id=\"Sbk_CivThumbnailDiv\"><a href=\"" + CurrentMode.Base_URL + CurrentItem.BibID + "/" + CurrentItem.VID + "\" ><img src=\"" + CurrentItem.Web.Source_URL + "/" + CurrentItem.Behaviors.Main_Thumbnail + "\" alt=\"" + name_for_image + "\" id=\"Sbk_CivThumbnailImg\" itemprop=\"primaryImageOfPage\" /></a></div>");
		            result.AppendLine();
		        }
		        else if (CurrentItem.Web.Static_PageCount > 0)
		        {
		            if (CurrentItem.Web.Pages_By_Sequence[0].Files.Count > 0)
		            {
		                string jpeg = String.Empty;
		                foreach (SobekCM_File_Info thisFileInfo in CurrentItem.Web.Pages_By_Sequence[0].Files)
		                {
		                    if (thisFileInfo.System_Name.ToLower().IndexOf(".jpg") > 0)
		                    {
		                        if (jpeg.Length == 0)
		                            jpeg = thisFileInfo.System_Name;
		                        else if (thisFileInfo.System_Name.ToLower().IndexOf("thm.jpg") < 0)
		                            jpeg = thisFileInfo.System_Name;
		                    }
		                }

		                string name_for_image = HttpUtility.HtmlEncode(CurrentItem.Bib_Info.Main_Title.ToString());
		                string name_of_page = CurrentItem.Web.Pages_By_Sequence[0].Label;
		                name_for_image = name_for_image + " - " + HttpUtility.HtmlEncode(name_of_page);


		                // If a jpeg was found, show it
		                if (jpeg.Length > 0)
		                {
		                    result.AppendLine();
		                    result.AppendLine(INDENT + "<div id=\"Sbk_CivThumbnailDiv\"><a href=\"" + CurrentMode.Base_URL + CurrentItem.BibID + "/" + CurrentItem.VID + "\" ><img src=\"" + CurrentItem.Web.Source_URL + "/" + jpeg + "\" alt=\"" + name_for_image + "\" id=\"Sbk_CivThumbnailImg\" itemprop=\"primaryImageOfPage\" /></a></div>");
		                    result.AppendLine();
		                }
		            }
		        }
		    }


		    result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_BiblioSection\" >");
		    result.AppendLine(INDENT + "<h2>" + biblio_info + "</h2>");
			result.AppendLine(INDENT + "  <dl>");

			// Collect the titles
			List<string> uniform_titles = new List<string>();
			Dictionary<string, List<string>> alternative_titles = new Dictionary<string, List<string>>();
			List<string> translated_titles = new List<string>();
			List<string> abbreviated_titles = new List<string>();
			if (CurrentItem.Bib_Info.Other_Titles_Count > 0)
			{
				foreach (Title_Info thisTitle in CurrentItem.Bib_Info.Other_Titles)
				{
					switch (thisTitle.Title_Type)
					{
						case Title_Type_Enum.UNSPECIFIED:
						case Title_Type_Enum.Alternative:
							string titleType = thisTitle.Display_Label;
							if ((titleType.Length == 0) || (titleType.ToUpper() == "OTHER TITLE"))
								titleType = "Alternate Title";
							if (alternative_titles.ContainsKey(titleType))
							{
								alternative_titles[titleType].Add(Convert_String_To_XML_Safe(thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim());
							}
							else
							{
								List<string> newTitleTypeList = new List<string>
								                                    { Convert_String_To_XML_Safe(thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim() };
							    alternative_titles[titleType] = newTitleTypeList;
							}

							break;

						case Title_Type_Enum.Uniform:
							uniform_titles.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(thisTitle.Title).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "TI") + Convert_String_To_XML_Safe(thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim() + search_link_end);
							break;

						case Title_Type_Enum.Translated:
							translated_titles.Add((Convert_String_To_XML_Safe(thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle) + " ( <em>" + thisTitle.Language + "</em> )").Trim());
							break;

						case Title_Type_Enum.Abbreviated:
							abbreviated_titles.Add(Convert_String_To_XML_Safe(thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim());
							break;
					}
				}
			}

			List<string> subjects = new List<string>();
			List<string> hierGeo = new List<string>();
			List<string> genres = new List<string>();
			string scale = String.Empty;
			if (CurrentItem.Bib_Info.Subjects_Count > 0)
			{
				foreach (Subject_Info thisSubject in CurrentItem.Bib_Info.Subjects)
				{
					switch (thisSubject.Class_Type)
					{
						case Subject_Info_Type.Hierarchical_Spatial:
							Subject_Info_HierarchicalGeographic hieroSubj = (Subject_Info_HierarchicalGeographic)thisSubject;
							StringBuilder spatial_builder = new StringBuilder();
							if (hieroSubj.Continent.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(hieroSubj.Continent);
							}
							if (hieroSubj.Country.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                                spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(hieroSubj.Country.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CO") + hieroSubj.Country + search_link_end);
							}
							if (hieroSubj.Province.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(hieroSubj.Province);
							}
							if (hieroSubj.Region.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(hieroSubj.Region);
							}
							if (hieroSubj.State.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                                spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(hieroSubj.State.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "ST") + hieroSubj.State + search_link_end);
							}
							if (hieroSubj.Territory.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(hieroSubj.Territory);
							}
							if (hieroSubj.County.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                                spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(hieroSubj.County.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&", "".Replace(" ", "+"))).Replace("<%CODE%>", "CT") + hieroSubj.County + search_link_end);
							}
							if (hieroSubj.City.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                                spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(hieroSubj.City.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CI") + hieroSubj.City + search_link_end);
							}
							if (hieroSubj.CitySection.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(hieroSubj.CitySection);
							}
							if (hieroSubj.Island.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(hieroSubj.Island);
							}
							if (hieroSubj.Area.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(hieroSubj.Area);
							}
							hierGeo.Add(spatial_builder.ToString());
							break;

						case Subject_Info_Type.Cartographics:
							scale = ((Subject_Info_Cartographics)thisSubject).Scale;
							break;

						case Subject_Info_Type.Standard:
						case Subject_Info_Type.Name:
						case Subject_Info_Type.TitleInfo:
							Subject_Standard_Base baseSubject = (Subject_Standard_Base)thisSubject;
							if ((thisSubject.Class_Type == Subject_Info_Type.Standard) && (baseSubject.Genres_Count > 0) && (baseSubject.Topics_Count == 0) && (baseSubject.Temporals_Count == 0) && (baseSubject.Geographics_Count == 0) && (((Subject_Info_Standard)baseSubject).Occupations_Count == 0))
							{
								foreach (string thisGenre in baseSubject.Genres)
								{
									if (baseSubject.Authority.Length > 0)
									{
                                        genres.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thisGenre).Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "GE") + "<span itemprop=\"genre\">" + Convert_String_To_XML_Safe(thisGenre) + "</span>" + search_link_end + " &nbsp; ( <em>" + baseSubject.Authority.ToLower() + "</em> )");
									}
									else
									{
                                        genres.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thisGenre).Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "GE") + "<span itemprop=\"genre\">" + Convert_String_To_XML_Safe(thisGenre) + "</span>" + search_link_end);
									}
								}
							}
							else
							{
								if ((thisSubject.Authority.Length > 0) && (thisSubject.Authority.ToUpper() != "NONE"))
								{
                                    subjects.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(thisSubject.ToString(false).Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "SU") + "<span itemprop=\"keywords\">" + thisSubject.ToString(false) + "</span>" + search_link_end + " &nbsp; ( <em>" + thisSubject.Authority.ToLower() + "</em> )");
								}
								else
								{
                                    subjects.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(thisSubject.ToString(false).Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "SU") + "<span itemprop=\"keywords\">" + thisSubject.ToString(false) + "</span>" + search_link_end);
								}
							}
							break;
					}
				}
			}

			if (CurrentItem.Bib_Info.Genres_Count > 0)
			{
				foreach (Genre_Info thisGenre in CurrentItem.Bib_Info.Genres)
				{
				    string genreXml = Convert_String_To_XML_Safe(thisGenre.Genre_Term);

					if (thisGenre.Authority.Length > 0)
					{
                        genres.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thisGenre.Genre_Term.Replace("&amp;", "&").Replace("&", "").Replace("  "," "))).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "GE") + "<span itemprop=\"genre\">" + Convert_String_To_XML_Safe(thisGenre.Genre_Term) + "</span>" + search_link_end + " &nbsp; ( <em>" + thisGenre.Authority.ToLower() + "</em> )");
					}
					else
					{
                        genres.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thisGenre.Genre_Term.Replace("&amp;", "&").Replace("&", "").Replace("  "," "))).Replace(",", "").Replace(" ", "+")).Replace("<%CODE%>", "GE") + "<span itemprop=\"genre\">" + Convert_String_To_XML_Safe(thisGenre.Genre_Term) + "</span>" + search_link_end);
					}
				}
			}

			// Add the titles
			result.Append(Single_Citation_HTML_Row("Title", "<span itemprop=\"name\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Main_Title.NonSort + " " + CurrentItem.Bib_Info.Main_Title.Title + " " + CurrentItem.Bib_Info.Main_Title.Subtitle).Trim() + "</span>", INDENT));
			if (CurrentItem.Bib_Info.hasSeriesTitle)
                result.Append(Single_Citation_HTML_Row("Series Title", search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(CurrentItem.Bib_Info.SeriesTitle.Title.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ")).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "TI") + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.SeriesTitle.NonSort + " " + CurrentItem.Bib_Info.SeriesTitle.Title + " " + CurrentItem.Bib_Info.SeriesTitle.Subtitle).Trim() + search_link_end, INDENT));
			Add_Citation_HTML_Rows("Uniform Title", uniform_titles, INDENT, result);
			foreach (KeyValuePair<string, List<string>> altTitleType in alternative_titles)
			{
				Add_Citation_HTML_Rows(altTitleType.Key, altTitleType.Value, INDENT, result);
			}
			Add_Citation_HTML_Rows("Translated Title", translated_titles, INDENT, result);
			Add_Citation_HTML_Rows("Abbreviated Title", abbreviated_titles, INDENT, result);

            // Collect and display the the statement of responsibility
		    if (CurrentItem.Bib_Info.Notes_Count > 0)
		    {
		        Note_Info statementOfResponsibility = null;
		        foreach (Note_Info thisNote in CurrentItem.Bib_Info.Notes)
		        {
		            if (thisNote.Note_Type == Note_Type_Enum.StatementOfResponsibility)
		            {
                        statementOfResponsibility = thisNote;
                        break;
		            }
		        }

		        // If there was a statement of responsibility, add it now
		        if (statementOfResponsibility != null)
		        {
		            result.Append(Single_Citation_HTML_Row(statementOfResponsibility.Note_Type_Display_String, "<span itemprop=\"notes\">" + Convert_String_To_XML_Safe(statementOfResponsibility.Note) + "</span>", INDENT));
		        }
		    }




			List<string> creators = new List<string>();
			List<string> conferences = new List<string>();
			if (CurrentItem.Bib_Info.hasMainEntityName) 
			{
				if (CurrentItem.Bib_Info.Main_Entity_Name.Name_Type == Name_Info_Type_Enum.Conference)
				{
					conferences.Add(CurrentItem.Bib_Info.Main_Entity_Name.ToString());
				}
				else
				{
					Name_Info thisName = CurrentItem.Bib_Info.Main_Entity_Name;
					StringBuilder nameBuilder = new StringBuilder();
					if (thisName.Full_Name.Length > 0)
					{
						nameBuilder.Append(Convert_String_To_XML_Safe(thisName.Full_Name.Replace("|", " -- ")));
					}
					else
					{
						if (thisName.Family_Name.Length > 0)
						{
							if (thisName.Given_Name.Length > 0)
							{
								nameBuilder.Append(Convert_String_To_XML_Safe(thisName.Family_Name) + ", " + Convert_String_To_XML_Safe(thisName.Given_Name));
							}
							else
							{
								nameBuilder.Append(Convert_String_To_XML_Safe(thisName.Family_Name));
							}
						}
						else
						{
						    nameBuilder.Append(thisName.Given_Name.Length > 0 ? Convert_String_To_XML_Safe(thisName.Given_Name) : "unknown");
						}
					}
					string name_linked = search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(thisName.ToString(false)).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "AU") + "<span itemprop=\"creator\">" + nameBuilder + "</span>" + search_link_end;
					if (nameBuilder.ToString() == "unknown")
						name_linked = "unknown";

					if (thisName.Display_Form.Length > 0)
						name_linked = name_linked + " ( " + Convert_String_To_XML_Safe(thisName.Display_Form) + " )";
					if (thisName.Dates.Length > 0)
						name_linked = name_linked + ", " + Convert_String_To_XML_Safe(thisName.Dates);
					creators.Add(name_linked + thisName.Role_String);
				}
			}
			if (CurrentItem.Bib_Info.Names_Count > 0)
			{
				foreach (Name_Info thisName in CurrentItem.Bib_Info.Names)
				{
					if (thisName.Name_Type == Name_Info_Type_Enum.Conference)
					{
						conferences.Add(thisName.ToString());
					}
					else
					{
						StringBuilder nameBuilder = new StringBuilder();
						if (thisName.Full_Name.Length > 0)
						{
							nameBuilder.Append(Convert_String_To_XML_Safe(thisName.Full_Name.Replace("|", " -- ")));
						}
						else
						{
							if (thisName.Family_Name.Length > 0)
							{
								if (thisName.Given_Name.Length > 0)
								{
									nameBuilder.Append(Convert_String_To_XML_Safe(thisName.Family_Name) + ", " + Convert_String_To_XML_Safe(thisName.Given_Name));
								}
								else
								{
									nameBuilder.Append(Convert_String_To_XML_Safe(thisName.Family_Name));
								}
							}
							else
							{
							    nameBuilder.Append(thisName.Given_Name.Length > 0 ? Convert_String_To_XML_Safe(thisName.Given_Name) : "unknown");
							}
						}
                        string name_linked = search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(thisName.ToString(false)).Replace(",", "").Replace("&amp;", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "AU") + "<span itemprop=\"creator\">" + nameBuilder + "</span>" + search_link_end;
						if (nameBuilder.ToString() == "unknown")
							name_linked = "unknown";
						if (thisName.Display_Form.Length > 0)
							name_linked = name_linked + " ( " + Convert_String_To_XML_Safe(thisName.Display_Form) + " )";
						if (thisName.Dates.Length > 0)
							name_linked = name_linked + ", " + Convert_String_To_XML_Safe(thisName.Dates);
						creators.Add(name_linked + thisName.Role_String);
					}
				}
			}
			Add_Citation_HTML_Rows("Creator", creators, INDENT, result);
			Add_Citation_HTML_Rows("Conference", conferences, INDENT, result);

			tempList.Clear();
			if (CurrentItem.Bib_Info.Affiliations_Count > 0)
			{
			    tempList.AddRange(CurrentItem.Bib_Info.Affiliations.Select(ThisAffiliation => ThisAffiliation.ToString()));
			    Add_Citation_HTML_Rows("Affiliation", tempList, INDENT, result);
			}

		    if ((CurrentItem.Bib_Info.hasDonor) && (CurrentItem.Bib_Info.Donor.Full_Name.Length > 0))
		    {
		        string donor_name = CurrentItem.Bib_Info.Donor.ToString(false);

                result.Append(Single_Citation_HTML_Row("Donor", search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(donor_name).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "DO") + donor_name + search_link_end, INDENT));
			}

			if (CurrentItem.Bib_Info.Publishers_Count > 0)
			{
				List<string> pubs = CurrentItem.Bib_Info.Publishers.Select(ThisPublisher => search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(ThisPublisher.Name).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "PU") + "<span itemprop=\"publisher\">" + Convert_String_To_XML_Safe(ThisPublisher.Name) + "</span>" + search_link_end).ToList();

				List<string> pub_places = new List<string>();
				foreach (Publisher_Info thisPublisher in CurrentItem.Bib_Info.Publishers)
				{
				    pub_places.AddRange(thisPublisher.Places.Select(Place => search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(Place.Place_Text)).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "PP") + Convert_String_To_XML_Safe(Place.Place_Text) + search_link_end));
				}

				Add_Citation_HTML_Rows("Place of Publication", pub_places, INDENT, result);

                Add_Citation_HTML_Rows("Publisher", pubs, INDENT, result);
			}

			if (CurrentItem.Bib_Info.Manufacturers_Count > 0)
			{
				List<string> manufacturers = CurrentItem.Bib_Info.Manufacturers.Select(ThisManufacturer => Convert_String_To_XML_Safe(ThisManufacturer.Name)).ToList();
			    Add_Citation_HTML_Rows("Manufacturer", manufacturers, INDENT, result);
			}

			if ((CurrentItem.Bib_Info.Origin_Info.Date_Created.Length > 0) && (CurrentItem.Bib_Info.Origin_Info.Date_Created.Trim() != "-1"))
			{
				result.Append(Single_Citation_HTML_Row("Creation Date", "<span itemprop=\"dateCreated\">" + CurrentItem.Bib_Info.Origin_Info.Date_Created + "</span>", INDENT));
			}

			if (CurrentItem.Bib_Info.Origin_Info.Date_Issued.Length > 0)
			{
				if (CurrentItem.Bib_Info.Origin_Info.Date_Issued.Trim() != "-1")
				{
					result.Append(Single_Citation_HTML_Row("Publication Date", "<span itemprop=\"datePublished\">" + CurrentItem.Bib_Info.Origin_Info.Date_Issued + "</span>", INDENT));
				}
			}
			else if ( CurrentItem.Bib_Info.Origin_Info.MARC_DateIssued.Length > 0 )
			{
				result.Append(Single_Citation_HTML_Row("Publication Date", "<span itemprop=\"datePublished\">" + CurrentItem.Bib_Info.Origin_Info.MARC_DateIssued + "</span>", INDENT));
			}

			if ((CurrentItem.Bib_Info.Origin_Info.Date_Copyrighted.Length > 0) && (CurrentItem.Bib_Info.Origin_Info.Date_Copyrighted.Trim() != "-1"))
			{
				result.Append(Single_Citation_HTML_Row("Copyright Date", "<span itemprop=\"copyrightYear\">" + CurrentItem.Bib_Info.Origin_Info.Date_Copyrighted + "</span>", INDENT));
			}

            if (CurrentItem.Bib_Info.Origin_Info.Frequencies_Count > 0)
            {
                List<string> frequencies = new List<string>();
                foreach (Origin_Info_Frequency thisFrequency in CurrentItem.Bib_Info.Origin_Info.Frequencies)
                {
                    if (!frequencies.Contains(thisFrequency.Term.ToLower()))
                        frequencies.Add(thisFrequency.Term.ToLower());
                }
                Add_Citation_HTML_Rows("Frequency", frequencies, INDENT, result);
            }

            List<string> languageList = new List<string>();
            foreach (Language_Info thisLanguage in CurrentItem.Bib_Info.Languages)
            {
                if (thisLanguage.Language_Text.Length > 0)
                {
                    string language_text = thisLanguage.Language_Text;
                    string from_possible_code = thisLanguage.Get_Language_By_Code(language_text);
                    if (from_possible_code.Length > 0)
                        language_text = from_possible_code;
                    if (language_text.ToUpper().IndexOf("ENGLISH") == 0)
                    {
                        languageList.Add("<span itemprop=\"inLanguage\">" + Convert_String_To_XML_Safe(language_text) + "</span>");
                    }
                    else
                    {
                        languageList.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(language_text)).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "LA") + "<span itemprop=\"inLanguage\">" + Convert_String_To_XML_Safe(language_text) + "</span>" + search_link_end);

                    }
                }
                else
                {
                    if (thisLanguage.Language_ISO_Code.Length > 0)
                    {
                        string language_text = thisLanguage.Get_Language_By_Code(thisLanguage.Language_ISO_Code);
                        if (language_text.Length > 0)
                        {
                            if (language_text.ToUpper().IndexOf("ENGLISH") == 0)
                            {
                                languageList.Add("<span itemprop=\"inLanguage\">" + Convert_String_To_XML_Safe(language_text) + "</span>");
                            }
                            else
                            {
                                languageList.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(language_text)).Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "LA") + "<span itemprop=\"inLanguage\">" + Convert_String_To_XML_Safe(language_text) + "</span>" + search_link_end);

                            }
                        }
                    }
                }
            }
            Add_Citation_HTML_Rows("Language", languageList, INDENT, result);

            if ((CurrentItem.Bib_Info.Original_Description.Extent.Length > 0) && (CurrentItem.Bib_Info.Original_Description.Extent.ToUpper().Trim() != CurrentItem.Bib_Info.SobekCM_Type_String.ToUpper().Trim()))
            {
                result.Append(Single_Citation_HTML_Row("Physical Description", "<span itemprop=\"description\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Original_Description.Extent) + "</span>", INDENT));
            }
            else
            {
                result.Append(Single_Citation_HTML_Row("Physical Description", "<span itemprop=\"description\">" + CurrentItem.Bib_Info.SobekCM_Type_String + "</span>", INDENT));
            }

            result.Append(Single_Citation_HTML_Row("Scale", scale, INDENT));

			// Collect the state/edition information
			if ((CurrentItem.Bib_Info.Origin_Info.Edition.Length > 0) || (( vraInfo != null ) && ( vraInfo.State_Edition_Count > 0)))
			{
				string edition_title = "Edition";
				List<string> editions = new List<string>();
				if (CurrentItem.Bib_Info.Origin_Info.Edition.Length > 0)
					editions.Add("<span itemprop=\"edition\">" + CurrentItem.Bib_Info.Origin_Info.Edition);

                if ((vraInfo != null) && (vraInfo.State_Edition_Count > 0))
                {
	                edition_title = "State / Edition";
	                editions.AddRange(vraInfo.State_Editions.Select(ThisEdition => "<span itemprop=\"edition\">" + ThisEdition + "</span>"));
                }
				Add_Citation_HTML_Rows(edition_title, editions, INDENT, result);
			}

			// Collect and display all the material information
			if (( vraInfo != null ) && ( vraInfo.Material_Count > 0))
			{
				StringBuilder materialsBuilder = new StringBuilder();
				string materials_type = String.Empty;
				foreach (VRACore_Materials_Info materials in vraInfo.Materials)
				{
					if ((materials_type.Length == 0) && (materials.Type.Length > 0))
						materials_type = materials.Type;
					if (materials.Materials.Length > 0)
					{
						if (materialsBuilder.Length > 0)
							materialsBuilder.Append(", ");
						materialsBuilder.Append(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(materials.Materials)).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "MA") + "<span itemprop=\"materials\">" + Convert_String_To_XML_Safe(materials.Materials) + "</span>" + search_link_end);
					}
				}
				if (materialsBuilder.Length > 0)
				{
					if (materials_type.Length > 0)
						materialsBuilder.Append(" ( " + Convert_String_To_XML_Safe(materials_type) + " )");
				}
				result.Append(Single_Citation_HTML_Row("Materials", materialsBuilder.ToString(), INDENT));
			}

			// Collect and display all the measurements information
			if (( vraInfo != null ) && ( vraInfo.Measurement_Count > 0))
			{
				List<string> measurements = (from measurement in vraInfo.Measurements where measurement.Measurements.Length > 0 select measurement.Measurements).ToList();
			    Add_Citation_HTML_Rows("Measurements", measurements, INDENT, result);
			}

			// Display all cultural context information
			if (( vraInfo != null ) && ( vraInfo.Cultural_Context_Count > 0))
			{
				tempList.Clear();
			    tempList.AddRange(vraInfo.Cultural_Contexts.Select(Convert_String_To_XML_Safe));
			    Add_Citation_HTML_Rows("Cultural Context", tempList, INDENT, result);
			}

			// Display all style/period information
			if (( vraInfo != null ) && ( vraInfo.Style_Period_Count > 0))
			{
				tempList.Clear();
			    tempList.AddRange( vraInfo.Style_Periods.Select(Convert_String_To_XML_Safe));
			    Add_Citation_HTML_Rows("Style/Period", tempList, INDENT, result);
			}

			// Display all technique information
			if (( vraInfo != null ) && ( vraInfo.Technique_Count > 0))
			{
				tempList.Clear();
			    tempList.AddRange( vraInfo.Techniques.Select(Convert_String_To_XML_Safe));
			    Add_Citation_HTML_Rows("Technique", tempList, INDENT, result);
			}

			if (CurrentItem.Bib_Info.Containers_Count > 0)
			{
				StringBuilder physicalLocationBuilder = new StringBuilder(1000);
				physicalLocationBuilder.Append("<dl id=\"sbkCiv_LocationList\">");
				foreach (Finding_Guide_Container thisContainer in CurrentItem.Bib_Info.Containers)
				{
					physicalLocationBuilder.Append("<dt>" + thisContainer.Type + ": </dt><dd>" + thisContainer.Name + "</dd>");
				}
				physicalLocationBuilder.Append("</dl>");
				result.Append(Single_Citation_HTML_Row("Physical Location", physicalLocationBuilder.ToString(), INDENT));
			}
			result.AppendLine(INDENT + "  </dl>");
            result.AppendLine(INDENT + "</div>");
			result.AppendLine();

			// Add the thesis/dissertation data if it exists
			if ((thesisInfo != null) && (thesisInfo.hasData))
			{
				result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_ThesisSection\">");
				result.AppendLine(INDENT + "<h2>" + thesis_title + "</h2>");
				result.AppendLine(INDENT + "  <dl>");

				if (thesisInfo.Degree.Length > 0)
				{
					if (thesisInfo.Degree_Level != Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Unknown)
					{
						switch (thesisInfo.Degree_Level)
						{
							case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Bachelors:
								result.Append(Single_Citation_HTML_Row("Degree", "Bachelor's ( " + thesisInfo.Degree + ")", INDENT));
								break;

							case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters:
								result.Append(Single_Citation_HTML_Row("Degree", "Master's ( " + thesisInfo.Degree + ")", INDENT));
								break;

							case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate:
								result.Append(Single_Citation_HTML_Row("Degree", "Doctorate ( " + thesisInfo.Degree + ")", INDENT));
								break;

							case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.PostDoctorate:
								result.Append(Single_Citation_HTML_Row("Degree", "Post-Doctorate ( " + thesisInfo.Degree + ")", INDENT));
								break;
						}
					}
					else
					{
						result.Append(Single_Citation_HTML_Row("Degree", thesisInfo.Degree, INDENT));
					}
				}
				else if (thesisInfo.Degree_Level != Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Unknown)
				{
					switch (thesisInfo.Degree_Level)
					{
						case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Bachelors:
							result.Append(Single_Citation_HTML_Row("Degree", "Bachelor's", INDENT));
							break;

						case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters:
							result.Append(Single_Citation_HTML_Row("Degree", "Master's", INDENT));
							break;

						case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate:
							result.Append(Single_Citation_HTML_Row("Degree", "Doctorate", INDENT));
							break;

						case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.PostDoctorate:
							result.Append(Single_Citation_HTML_Row("Degree", "Post-Doctorate", INDENT));
							break;
					}
				}


				result.Append(Single_Citation_HTML_Row("Degree Grantor", thesisInfo.Degree_Grantor, INDENT));

				if (thesisInfo.Degree_Divisions_Count > 0)
				{
					StringBuilder divisionBuilder = new StringBuilder();
					foreach (string thisDivision in thesisInfo.Degree_Divisions)
					{
						if (divisionBuilder.Length > 0)
							divisionBuilder.Append(", ");
						divisionBuilder.Append(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thisDivision)).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "EJ") + Convert_String_To_XML_Safe(thisDivision) + search_link_end);
					}

					result.Append(Single_Citation_HTML_Row("Degree Divisions", divisionBuilder.ToString(), INDENT));
				}
				
				if (thesisInfo.Degree_Disciplines_Count > 0)
				{
					StringBuilder disciplinesBuilder = new StringBuilder();
					foreach (string thisDiscipline in thesisInfo.Degree_Disciplines)
					{
						if (disciplinesBuilder.Length > 0)
							disciplinesBuilder.Append(", ");
						disciplinesBuilder.Append(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thisDiscipline)).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "EI") + Convert_String_To_XML_Safe(thisDiscipline) + search_link_end);
					}
					string text_disciplines = "Degree Disciplines";
					if (CurrentMode.Skin.ToLower() == "ncf")
						text_disciplines = "Area of Concentration";
					result.Append(Single_Citation_HTML_Row(text_disciplines, disciplinesBuilder.ToString(), INDENT));
				}

				if ( thesisInfo.Committee_Chair.Length > 0 )
					result.Append(Single_Citation_HTML_Row("Committee Chair", search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thesisInfo.Committee_Chair)).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "EC") + Convert_String_To_XML_Safe(thesisInfo.Committee_Chair) + search_link_end, INDENT));
				if (thesisInfo.Committee_Co_Chair.Length > 0)
					result.Append(Single_Citation_HTML_Row("Committee Co-Chair", search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thesisInfo.Committee_Co_Chair)).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "EC") + Convert_String_To_XML_Safe(thesisInfo.Committee_Co_Chair) + search_link_end, INDENT));

				if (thesisInfo.Committee_Members_Count > 0)
				{
					tempList.Clear();
					foreach (string thisMember in thesisInfo.Committee_Members)
					{
						tempList.Add(search_link.Replace("<%VALUE%>", HttpUtility.UrlEncode(Convert_String_To_XML_Safe(thisMember)).Replace(",", "").Replace("&amp;","&").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "EC") + Convert_String_To_XML_Safe(thisMember) + search_link_end);
					}
					string text = "Committee Members";
					if (CurrentMode.Skin.ToLower() == "ncf")
						text = "Faculty Sponsor";
					Add_Citation_HTML_Rows(text, tempList, INDENT, result);
				}
				
				result.AppendLine(INDENT + "  </dl>");
				result.AppendLine(INDENT + "</div>");
			}

			// Add the taxonomic data if it exists
			if (( taxonInfo != null ) && ( taxonInfo.hasData ))
			{
				result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_TaxonSection\">");
                result.AppendLine(INDENT + "<h2>" + zoological_taxonomy + "</h2>");
				result.AppendLine(INDENT + "  <dl>");

                result.Append(Single_Citation_HTML_Row("Scientific Name", taxonInfo.Scientific_Name, INDENT));
                result.Append(Single_Citation_HTML_Row("Kingdom", taxonInfo.Kingdom, INDENT));
                result.Append(Single_Citation_HTML_Row("Phylum", taxonInfo.Phylum, INDENT));
                result.Append(Single_Citation_HTML_Row("Class", taxonInfo.Class, INDENT));
                result.Append(Single_Citation_HTML_Row("Order", taxonInfo.Order, INDENT));
                result.Append(Single_Citation_HTML_Row("Family", taxonInfo.Family, INDENT));
                result.Append(Single_Citation_HTML_Row("Genus", taxonInfo.Genus, INDENT));
                result.Append(Single_Citation_HTML_Row("Species", taxonInfo.Specific_Epithet, INDENT));
                result.Append(Single_Citation_HTML_Row("Taxonomic Rank", taxonInfo.Taxonomic_Rank, INDENT));
                result.Append(Single_Citation_HTML_Row("Common Name", taxonInfo.Common_Name, INDENT));

				result.AppendLine(INDENT + "  </dl>");
                result.AppendLine(INDENT + "</div>");
				result.AppendLine();
			}

            // Add the learning object metadata if it exists
            if ((lomInfo != null) && (lomInfo.hasData))
            {
                result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_LomSection\" >");
                result.AppendLine(INDENT + "<h2>" + learningobject_title + "</h2>");
				result.AppendLine(INDENT + "  <dl>");

                // Add the LOM Aggregation level
                if (lomInfo.AggregationLevel != AggregationLevelEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch ( lomInfo.AggregationLevel )
                    {
                        case AggregationLevelEnum.level1:
                            lom_temp = "Level 1 - a single, atomic object";
                            break;

                        case AggregationLevelEnum.level2:
                            lom_temp = "Level 2 - a lesson plan";
                            break;

                        case AggregationLevelEnum.level3:
                            lom_temp = "Level 3 - a course, or set of lesson plans";
                            break;

                        case AggregationLevelEnum.level4:
                            lom_temp = "Level 4 - a set of courses";
                            break;
                    }

                    result.Append(Single_Citation_HTML_Row("Aggregation Level", lom_temp, INDENT));
                }
                

                // Add the LOM Learning resource type
                if (lomInfo.LearningResourceTypes.Count > 0)
                {
					List<string> types = lomInfo.LearningResourceTypes.Select(ThisType => "<span itemprop=\"learningResourceType\">" + ThisType.Value + "</span>").ToList();

	                Add_Citation_HTML_Rows("Learning Resource Type", types, INDENT, result);
                }

                // Add the LOM Status
                if (lomInfo.Status != StatusEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.Status)
                    {
                        case StatusEnum.draft:
                            lom_temp = "Draft";
                            break;

                        case StatusEnum.final:
                            lom_temp = "Final";
                            break;

                        case StatusEnum.revised:
                            lom_temp = "Revised";
                            break;

                        case StatusEnum.unavailable:
                            lom_temp = "Unavailable";
                            break;
                    }

                    result.Append(Single_Citation_HTML_Row("Status", lom_temp, INDENT));
                }

                // Add the LOM Interactivity Type
                if (lomInfo.InteractivityType != InteractivityTypeEnum.UNDEFINED )
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.InteractivityType)
                    {
                        case InteractivityTypeEnum.active:
                            lom_temp = "Active";
                            break;

                        case InteractivityTypeEnum.expositive:
                            lom_temp = "Expositive";
                            break;

                        case InteractivityTypeEnum.mixed:
                            lom_temp = "Mixed";
                            break;
                    }

					result.Append(Single_Citation_HTML_Row("Interactivity Type", "<span itemprop=\"interactivityType\">" + lom_temp + "</span>", INDENT));
                }

                // Add the LOM Interactivity Level
                if (lomInfo.InteractivityLevel != InteractivityLevelEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.InteractivityLevel)
                    {
                        case InteractivityLevelEnum.very_low:
                            lom_temp = "Very low";
                            break;

                        case InteractivityLevelEnum.low:
                            lom_temp = "Low";
                            break;

                        case InteractivityLevelEnum.medium:
                            lom_temp = "Mediuim";
                            break;

                        case InteractivityLevelEnum.high:
                            lom_temp = "High";
                            break;

                        case InteractivityLevelEnum.very_high:
                            lom_temp = "Very high";
                            break;
                    }

                    result.Append(Single_Citation_HTML_Row("Interactivity Level", lom_temp, INDENT));
                }

                // Add the LOM Difficulty Level
                if (lomInfo.DifficultyLevel != DifficultyLevelEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.DifficultyLevel)
                    {
                        case DifficultyLevelEnum.very_easy:
                            lom_temp = "Very easy";
                            break;

                        case DifficultyLevelEnum.easy:
                            lom_temp = "Easy";
                            break;

                        case DifficultyLevelEnum.medium:
                            lom_temp = "Mediuim";
                            break;

                        case DifficultyLevelEnum.difficult:
                            lom_temp = "Difficult";
                            break;

                        case DifficultyLevelEnum.very_difficult:
                            lom_temp = "Very difficult";
                            break;
                    }

                    result.Append(Single_Citation_HTML_Row("Interactivity Level", lom_temp, INDENT));
                }

                
                // Add the LOM Intended End User Role
                if (lomInfo.IntendedEndUserRoles.Count > 0)
                {
                    List<string> roles = new List<string>();

                    foreach (IntendedEndUserRoleEnum thisUser in lomInfo.IntendedEndUserRoles)
                    {
                        switch( thisUser )
                        {
                            case IntendedEndUserRoleEnum.teacher:
								roles.Add("<span itemprop=\"audience\">Teacher</span>");
                                break;

                            case IntendedEndUserRoleEnum.learner:
								roles.Add("<span itemprop=\"audience\">Learner</span>");
                                break;

                            case IntendedEndUserRoleEnum.author:
								roles.Add("<span itemprop=\"audience\">Author</span>");
                                break;

                            case IntendedEndUserRoleEnum.manager:
                                roles.Add("<span itemprop=\"audience\">Manager</span>");
                                break;

                        }
                    }

                    Add_Citation_HTML_Rows("Intended User Roles", roles, INDENT, result);
                }
                
                // Add the LOM Context 
                if (lomInfo.Contexts.Count > 0)
                {
                    List<string> contexts = new List<string>();

                    foreach (LOM_VocabularyState thisContext in lomInfo.Contexts)
                    {
                        if ( thisContext.Source.Length > 0 )
                            contexts.Add( thisContext.Source + " " + thisContext.Value );
                        else
                            contexts.Add(thisContext.Value);
                    }

                    Add_Citation_HTML_Rows("Context", contexts, INDENT, result);
                }


                // Add the LOM Typical Age Range
                if (lomInfo.TypicalAgeRanges.Count > 0)
                {
					List<string> ranges = lomInfo.TypicalAgeRanges.Select(ThisUser => "<span itemprop=\"typicalAgeRange\">" + ThisUser.Value + "</span>").ToList();

	                Add_Citation_HTML_Rows("Typical Age Range", ranges, INDENT, result);
                }

                // Add the typical learning time
                result.Append(Single_Citation_HTML_Row("Typical Learning Time", lomInfo.TypicalLearningTime, INDENT));

                // Add the system requirements
                if (lomInfo.SystemRequirements.Count > 0)
                {
                    List<string> reqs = new List<string>();

                    foreach (LOM_System_Requirements thisContext in lomInfo.SystemRequirements)
                    {
                        string start = String.Empty;
                        switch ( thisContext.RequirementType )
                        {
                            case RequirementTypeEnum.operating_system:
                                start = "Operating System: " + thisContext.Name;
                                break;

                            case RequirementTypeEnum.browser:
                                start = "Browser: " + thisContext.Name;
                                break;

                            case RequirementTypeEnum.hardware:
                                start = "Hardware: " + thisContext.Name;
                                break;

                            case RequirementTypeEnum.software:
                                start = "Software: " + thisContext.Name;
                                break;
                        }

                        // Add with version, if included
                        if (thisContext.MinimumVersion.Length == 0)
                        {
                            if (thisContext.MaximumVersion.Length > 0)
                            {
                                reqs.Add(start + " ( - " + thisContext.MaximumVersion + " )");
                            }
                            else
                            {
                                reqs.Add(start);
                            }
                        }
                        else
                        {
                            if (thisContext.MaximumVersion.Length > 0)
                            {
                                reqs.Add(start + " ( " + thisContext.MinimumVersion + " - " + thisContext.MaximumVersion + " )");
                            }
                            else
                            {
                                reqs.Add(start + " ( " + thisContext.MinimumVersion + " - )");
                            }
                        }
                    }

                    Add_Citation_HTML_Rows("System Requirements", reqs, INDENT, result);
                }

				result.AppendLine(INDENT + "  </dl>");
                result.AppendLine(INDENT + "</div>");
				result.AppendLine();
            }


			// Add the subjects and coordinate information if that exists
			if ((subjects.Count > 0) || (genres.Count > 0) || (CurrentItem.Bib_Info.TemporalSubjects_Count > 0) || (hierGeo.Count > 0) ||
                ((geoInfo != null) && (geoInfo.hasData) && ((geoInfo.Point_Count > 0) || (geoInfo.Polygon_Count > 0))))
			{
				result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_SubjectSection\" >");
                result.AppendLine(INDENT + "<h2>" + subject_info + "</h2>");
				result.AppendLine(INDENT + "  <dl>");

				Add_Citation_HTML_Rows("Subjects / Keywords", subjects, INDENT, result);
				Add_Citation_HTML_Rows("Genre", genres, INDENT, result);
				tempList.Clear();
				if (CurrentItem.Bib_Info.TemporalSubjects_Count > 0)
				{
					foreach (Temporal_Info thisTemporal in CurrentItem.Bib_Info.TemporalSubjects)
					{
						if (thisTemporal.TimePeriod.Length > 0)
						{
							if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year > 0))
							{
								tempList.Add(Convert_String_To_XML_Safe(thisTemporal.TimePeriod) + " ( " + thisTemporal.Start_Year.ToString() + " - " + thisTemporal.End_Year.ToString() + " )");
							}
							if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year <= 0))
							{
								tempList.Add(Convert_String_To_XML_Safe(thisTemporal.TimePeriod) + " ( " + thisTemporal.Start_Year.ToString() + " - )");
							}
							if ((thisTemporal.Start_Year <= 0) && (thisTemporal.End_Year > 0))
							{
								tempList.Add(Convert_String_To_XML_Safe(thisTemporal.TimePeriod) + " (  - " + thisTemporal.End_Year.ToString() + " )");
							}
						}
						else
						{
							if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year > 0))
							{
								tempList.Add(thisTemporal.Start_Year.ToString() + " - " + thisTemporal.End_Year.ToString());
							}
							if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year <= 0))
							{
								tempList.Add(thisTemporal.Start_Year.ToString() + " - ");
							}
							if ((thisTemporal.Start_Year <= 0) && (thisTemporal.End_Year > 0))
							{
								tempList.Add(" - " + thisTemporal.End_Year.ToString());
							}
						}
					}
					Add_Citation_HTML_Rows("Temporal Coverage", tempList, INDENT, result);
				}
				Add_Citation_HTML_Rows("Spatial Coverage", hierGeo, INDENT, result);

                if (( geoInfo != null) && ( geoInfo.hasData))
				{
                    for (int i = 0; i < geoInfo.Point_Count; i++)
					{
                        if (geoInfo.Points[i].Label.Length > 0)
						{
                            tempList.Add("<span itemprop=\"geo\" itemscope itemtype=\"http://schema.org/GeoCoordinates\"><span itemprop=\"latitude\">" + geoInfo.Points[i].Latitude + "</span> x <span itemprop=\"longitude\">" + geoInfo.Points[i].Longitude + "</span> ( <span itemprop=\"name\">" + Convert_String_To_XML_Safe(geoInfo.Points[i].Label) + "</span> )</span>");
						}
						else
						{
							tempList.Add("<span itemprop=\"geo\" itemscope itemtype=\"http://schema.org/GeoCoordinates\"><span itemprop=\"latitude\">" + geoInfo.Points[i].Latitude + "</span> x <span itemprop=\"longitude\">" + geoInfo.Points[i].Longitude + "</span></span>");
						}
					}
					Add_Citation_HTML_Rows("Coordinates", tempList, INDENT, result);

					tempList.Clear();
                    if (geoInfo.Polygon_Count == 1)
					{
                        for (int i = 0; i < geoInfo.Polygon_Count; i++)
						{
                            Coordinate_Polygon polygon = geoInfo.Get_Polygon(i);
							StringBuilder polygonBuilder = new StringBuilder();
							foreach (Coordinate_Point thisPoint in polygon.Edge_Points)
							{
								if (polygonBuilder.Length > 0)
								{
									polygonBuilder.Append(", " + thisPoint.Latitude + " x " + thisPoint.Longitude);
								}
								else
								{
									polygonBuilder.Append(thisPoint.Latitude + " x " + thisPoint.Longitude);
								}
							}

							if (polygon.Label.Length > 0)
							{
								polygonBuilder.Append(" ( " + Convert_String_To_XML_Safe(polygon.Label) + " )");
							}
							if (polygonBuilder.ToString().Trim().Length > 0)
							{
								tempList.Add(polygonBuilder.ToString());
							}
						}
						Add_Citation_HTML_Rows("Polygon", tempList, INDENT, result);
					}
				}

				tempList.Clear();
				if (CurrentItem.Bib_Info.Target_Audiences_Count > 0)
				{
					foreach (TargetAudience_Info thisAudience in CurrentItem.Bib_Info.Target_Audiences)
					{
						if (thisAudience.Authority.Length > 0)
						{
							tempList.Add(Convert_String_To_XML_Safe(thisAudience.Audience) + " &nbsp; ( <em>" + thisAudience.Authority.ToLower() + "</em> )");
						}
						else
						{
							tempList.Add(Convert_String_To_XML_Safe(thisAudience.Audience));
						}
					}
				}
				result.AppendLine(INDENT + "  </dl>");
                result.AppendLine(INDENT + "</div>");
				result.AppendLine();
			}

			if ((CurrentItem.Bib_Info.Abstracts_Count > 0) || (CurrentItem.Bib_Info.Notes_Count > 0) || (CurrentItem.Behaviors.User_Tags_Count > 0))
			{
				// Ensure that if this user is non-internal, the only notes field is not internal
                bool valid_notes_exist = internalUser;
				if (!valid_notes_exist)
				{
					if (CurrentItem.Bib_Info.Abstracts_Count > 0)
						valid_notes_exist = true;
					else
					{
                        if (CurrentItem.Bib_Info.Notes.Any(ThisNote => (ThisNote.Note_Type != Note_Type_Enum.InternalComments) && (ThisNote.Note_Type != Note_Type_Enum.StatementOfResponsibility)))
						{
						    valid_notes_exist = true;
						}
					}
				}

				if ((valid_notes_exist) || (CurrentItem.Bib_Info.Abstracts_Count > 0) || (CurrentItem.Behaviors.User_Tags_Count > 0) || (( vraInfo != null ) && ( vraInfo.Inscription_Count > 0)))
				{
					result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\"  id=\"sbkCiv_NotesSection\" >");
					result.AppendLine(INDENT + "<h2>" + notes_info + "</h2>");
					result.AppendLine(INDENT + "  <dl>");

					if (CurrentItem.Bib_Info.Abstracts_Count > 0)
					{
						foreach (Abstract_Info thisAbstract in CurrentItem.Bib_Info.Abstracts)
						{
						    result.Append(thisAbstract.Display_Label.Length > 0
											  ? Single_Citation_HTML_Row(Convert_String_To_XML_Safe(thisAbstract.Display_Label), "<span itemprop=\"description\">" + Convert_String_To_XML_Safe(thisAbstract.Abstract_Text) + "</span>", INDENT)
											  : Single_Citation_HTML_Row("Abstract", "<span itemprop=\"description\">" + Convert_String_To_XML_Safe(thisAbstract.Abstract_Text) + "</span>", INDENT));
						}
					}


					if (CurrentItem.Bib_Info.Notes_Count > 0)
					{
						foreach (Note_Info thisNote in CurrentItem.Bib_Info.Notes)
						{
							if (thisNote.Note_Type != Note_Type_Enum.NONE)
							{
                                // Statement of responsibilty will be printed at the very end
							    if (thisNote.Note_Type != Note_Type_Enum.StatementOfResponsibility)
							    {
							        if ((thisNote.Note_Type != Note_Type_Enum.InternalComments) || (internalUser))
							        {
							            result.Append(Single_Citation_HTML_Row(thisNote.Note_Type_Display_String, "<span itemprop=\"notes\">" + Convert_String_To_XML_Safe(thisNote.Note) + "</span>", INDENT));
							        }
							    }
							}
							else
							{
								result.Append(Single_Citation_HTML_Row("General Note", "<span itemprop=\"notes\">" + Convert_String_To_XML_Safe(thisNote.Note) + "</span>", INDENT));
							}
						}
					}

					if (( vraInfo != null ) && ( vraInfo.Inscription_Count > 0))
					{
						Add_Citation_HTML_Rows("Inscription", vraInfo.Inscriptions, INDENT, result);
					}

					if (CurrentItem.Behaviors.User_Tags_Count > 0)
					{
						int tag_counter = 1;
						foreach (Descriptive_Tag tag in CurrentItem.Behaviors.User_Tags)
						{
                            if ((CurrentUser != null) && (CurrentUser.UserID == tag.UserID))
							{
								string citation_value = Convert_String_To_XML_Safe(tag.Description_Tag) + " <br /><em> Description added by you on " + tag.Date_Added.ToShortDateString() + "</em><br />( <a href=\"\" name=\"edit_describe_button" + tag_counter + "\" id=\"edit_describe_button" + tag_counter + "\" onclick=\"return edit_tag( 'edit_describe_button" + tag_counter + "', " + tag.TagID + ", '" + HttpUtility.HtmlEncode(tag.Description_Tag) + "');\">edit</a> | <a href=\"\" onclick=\"return delete_tag(" + tag.TagID + ");\">delete</a> )";
								result.Append(INDENT + "    <tr>\n" + INDENT + "      <td width=\"15\"> </td>\n" + INDENT + "      <td width=\"" + width + "\" valign=\"top\"><b>" + Translator.Get_Translation("User Description", CurrentMode.Language) + ": </b></td>\n" + INDENT + "      <td style=\"background-color:#eeee88\">" + citation_value + "</td>\n" + INDENT + "    </tr>\n");
							}
							else
							{
								result.Append(Single_Citation_HTML_Row("User Description", Convert_String_To_XML_Safe(tag.Description_Tag) + " <br /><em> Description added by " + tag.UserName + " on " + tag.Date_Added.ToShortDateString() + "</em>", INDENT));
							}

							tag_counter++;
						}
					}

					result.AppendLine(INDENT + "  <dl>");
                    result.AppendLine(INDENT + "</div>");
					result.AppendLine();
				}
			}

			result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_InstitutionSection\" >");            
			result.AppendLine(INDENT + "<h2>" + institutional_info + "</h2>");
			result.AppendLine(INDENT + "  <dl>");

			// Add the SOURCE INSTITUTION information
			if (CurrentItem.Bib_Info.Source.Statement.Length > 0)
			{
				if (CurrentItem.Bib_Info.Source.Code.Length > 0)
				{
					// Include the code for internal users
					string codeString = String.Empty;
                    if (internalUser)
						codeString = " ( i" + CurrentItem.Bib_Info.Source.Code + " )";

					if ((Code_Manager != null) && (Code_Manager.isValidCode("i" + CurrentItem.Bib_Info.Source.Code)))
					{
						Item_Aggregation_Related_Aggregations sourceAggr = Code_Manager["i" + CurrentItem.Bib_Info.Source.Code];
						if (sourceAggr.Active)
						{
						    result.Append( !String.IsNullOrEmpty(sourceAggr.External_Link)
											  ? Single_Citation_HTML_Row("Source Institution", "<span itemprop=\"sourceOrganization\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + "</span>" + codeString + " ( <a href=\"" + CurrentMode.Base_URL + "i" + CurrentItem.Bib_Info.Source.Code + url_options + "\">" + CurrentMode.Instance_Abbreviation + " page</a> | <a href=\"" + sourceAggr.External_Link + "\">external link</a> )", INDENT)
											  : Single_Citation_HTML_Row("Source Institution", "<a href=\"" + CurrentMode.Base_URL + "i" + CurrentItem.Bib_Info.Source.Code + url_options + "\"><span itemprop=\"sourceOrganization\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + "</span></a> " + codeString, INDENT));
						}
						else
						{
                            result.Append(!String.IsNullOrEmpty(sourceAggr.External_Link)
											  ? Single_Citation_HTML_Row("Source Institution", "<a href=\"" + sourceAggr.External_Link + "\"><span itemprop=\"sourceOrganization\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + "</span></a> " + codeString, INDENT)
											  : Single_Citation_HTML_Row("Source Institution", "<span itemprop=\"sourceOrganization\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + "</span>" + codeString, INDENT));
						}
					}
					else
					{
						result.Append(Single_Citation_HTML_Row("Source Institution", "<span itemprop=\"sourceOrganization\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + "</span>" + codeString, INDENT));
					}
				}
				else
				{
					result.Append(Single_Citation_HTML_Row("Source Institution", "<span itemprop=\"sourceOrganization\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + "</span>", INDENT));
				}
			}

			// Add the HOLDING LOCATION information
			if ((CurrentItem.Bib_Info.hasLocationInformation) && (CurrentItem.Bib_Info.Location.Holding_Name.Length > 0))
			{
				if (CurrentItem.Bib_Info.Location.Holding_Code.Length > 0)
				{
					// Include the code for internal users
					string codeString = String.Empty;
                    if (internalUser)
						codeString = " ( i" + CurrentItem.Bib_Info.Location.Holding_Code + " )";

					if ((Code_Manager != null) && (Code_Manager.isValidCode("i" + CurrentItem.Bib_Info.Location.Holding_Code)))
					{
						Item_Aggregation_Related_Aggregations holdingAggr = Code_Manager["i" + CurrentItem.Bib_Info.Location.Holding_Code];
						if (holdingAggr.Active)
						{
						    result.Append(!String.IsNullOrEmpty(holdingAggr.External_Link)
											  ? Single_Citation_HTML_Row("Holding Location", "<span itemprop=\"contentLocation\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.Holding_Name) + "</span>" + codeString + " ( <a href=\"" + CurrentMode.Base_URL + "i" + CurrentItem.Bib_Info.Location.Holding_Code + url_options + "\">" + CurrentMode.Instance_Abbreviation + " page</a> | <a href=\"" + holdingAggr.External_Link + "\">external link</a> )", INDENT)
											  : Single_Citation_HTML_Row("Holding Location", "<a href=\"" + CurrentMode.Base_URL + "i" + CurrentItem.Bib_Info.Location.Holding_Code.ToLower() + url_options + "\"><span itemprop=\"contentLocation\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.Holding_Name) + "</span></a> " + codeString, INDENT));
						}
						else
						{
                            result.Append(!String.IsNullOrEmpty(holdingAggr.External_Link)
											  ? Single_Citation_HTML_Row("Holding Location", "<a href=\"" + holdingAggr.External_Link + "\"><span itemprop=\"contentLocation\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.Holding_Name) + "</span></a> " + codeString, INDENT)
											  : Single_Citation_HTML_Row("Holding Location", "<span itemprop=\"contentLocation\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.Holding_Name) + "</span>" + codeString, INDENT));
						}
					}
					else
					{
						result.Append(Single_Citation_HTML_Row("Holding Location", "<span itemprop=\"contentLocation\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.Holding_Name) + "</span>" + codeString, INDENT));
					}
				}
				else
				{
					result.Append(Single_Citation_HTML_Row("Holding Location", "<span itemprop=\"contentLocation\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.Holding_Name) + "</span>", INDENT));
				}
			}


			// Add the RIGHTS STATEMENT
			string rights_statement = "<span itemprop=\"rights\">All applicable rights reserved by the source institution and holding location.</span>";
			if (CurrentItem.Bib_Info.Access_Condition.Text.Length > 0)
			{
                if (CurrentItem.Bib_Info.Access_Condition.Text.IndexOf("http://") == 0)
                    rights_statement = "<span itemprop=\"rights\"><a href=\"" + CurrentItem.Bib_Info.Access_Condition.Text + "\" target=\"RIGHTS\" >" + CurrentItem.Bib_Info.Access_Condition.Text + "</a></span>";
                else
                    rights_statement = "<span itemprop=\"rights\">" + CurrentItem.Bib_Info.Access_Condition.Text + "</span>";
			}
			result.AppendLine(INDENT + "    <dt style=\"width:" + width + "px\">" + Translator.Get_Translation("Rights Management", CurrentMode.Language) + ": </dt>");
			result.Append(INDENT + "      <dd style=\"margin-left:" + width + "px\">");
			const string SEE_TEXT = "See License Deed";

				if (rights_statement.IndexOf("[cc by-nc-nd]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-nc-nd]", "<br /><a href=\"http://creativecommons.org/licenses/by-nc-nd/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + Static_Resources.Cc_By_Nc_Nd_Img + "\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by-nc-sa]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-nc-sa]", "<br /><a href=\"http://creativecommons.org/licenses/by-nc-sa/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + Static_Resources.Cc_By_Nc_Sa_Img + "\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by-nc]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-nc]", "<br /><a href=\"http://creativecommons.org/licenses/by-nc/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + Static_Resources.Cc_By_Nc_Img + "\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by-nd]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-nd]", "<br /><a href=\"http://creativecommons.org/licenses/by-nd/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + Static_Resources.Cc_By_Nd_Img + "\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by-sa]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-sa]", "<br /><a href=\"http://creativecommons.org/licenses/by-sa/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + Static_Resources.Cc_By_Sa_Img + "\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by]", "<br /><a href=\"http://creativecommons.org/licenses/by/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + Static_Resources.Cc_By_Img + "\" /></a>");
				}
				if (rights_statement.IndexOf("[cc0]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc0]", "<br /><a href=\"http://creativecommons.org/publicdomain/zero/1.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + Static_Resources.Cc_Zero_Img + "\" /></a>");
				}

			result.AppendLine(rights_statement + "</dd>");

            // Add eembargo date, if there is one
		    if ((rightsInfo != null) && ( rightsInfo.Has_Embargo_End ))
		    {
                result.Append(Single_Citation_HTML_Row("Embargo Date", rightsInfo.Embargo_End.ToShortDateString(), INDENT));
		    }

			// Add the IDENTIFIERS
			tempList.Clear();
			if (CurrentItem.Bib_Info.Identifiers_Count > 0)
			{
				foreach (Identifier_Info thisIdentifier in CurrentItem.Bib_Info.Identifiers)
				{
					if (thisIdentifier.Type.Length > 0)
					{
						tempList.Add("<em>" + Convert_String_To_XML_Safe(thisIdentifier.Type.ToLower()) + "</em> - <span itemprop=\"identifier\">" + Convert_String_To_XML_Safe(thisIdentifier.Identifier) + "</span>");
					}
					else
					{
						tempList.Add("<span itemprop=\"identifier\">" + Convert_String_To_XML_Safe(thisIdentifier.Identifier) + "</span>");
					}
				}
				Add_Citation_HTML_Rows("Resource Identifier", tempList, INDENT, result);
			}

			// Add the CLASSIFICATIONS
			tempList.Clear();
			if (CurrentItem.Bib_Info.Classifications_Count > 0)
			{
				foreach (Classification_Info thisClassification in CurrentItem.Bib_Info.Classifications)
				{
					if (thisClassification.Authority.Length > 0)
					{
						tempList.Add("<em>" + Convert_String_To_XML_Safe(thisClassification.Authority.ToLower()) + "</em> - <span itemprop=\"classification\">" + Convert_String_To_XML_Safe(thisClassification.Classification) + "</span>");
					}
					else
					{
						tempList.Add("<span itemprop=\"classification\">" + Convert_String_To_XML_Safe(thisClassification.Classification) + "</span>");
					}
				}
				Add_Citation_HTML_Rows("Classification", tempList, INDENT, result);
			}

			// Add the system id

		    result.Append(CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL
		                      ? Single_Citation_HTML_Row("System ID", CurrentItem.BibID + ":" + CurrentItem.VID, INDENT)
		                      : Single_Citation_HTML_Row("System ID", CurrentItem.BibID, INDENT));

			result.AppendLine(INDENT + "  </dl>");
            result.AppendLine(INDENT + "</div>");
			result.AppendLine();

			// Add the RELATED ITEMS
			if (CurrentItem.Bib_Info.RelatedItems_Count > 0)
			{
				result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_RelatedSection\" >");
                result.AppendLine(INDENT + "<h2>" + related_items + "</h2>");
				result.AppendLine(INDENT + "  <dl>");

				foreach (Related_Item_Info relatedItem in CurrentItem.Bib_Info.RelatedItems)
				{

					string label = related_items;
					switch (relatedItem.Relationship)
					{
						case Related_Item_Type_Enum.Host:
							label = "Host material";
							break;

						case Related_Item_Type_Enum.OtherFormat:
							label = "Other format";
							break;

						case Related_Item_Type_Enum.OtherVersion:
							label = "Other version";
							break;

						case Related_Item_Type_Enum.Preceding:
							label = "Preceded by";
							break;

						case Related_Item_Type_Enum.Succeeding:
							label = "Succeeded by";
							break;
					}

					string url = String.Empty;
					string title = Convert_String_To_XML_Safe(relatedItem.Main_Title.Title);

					if (relatedItem.URL.Length > 0)
					{
						if (relatedItem.URL_Display_Label.Length > 0)
						{
							url = "<a href=\"" + relatedItem.URL + "\" target=\"RELATEDITEM\" >" + Convert_String_To_XML_Safe(relatedItem.URL_Display_Label) + "</a>";
						}
						else
						{
							url = "<a href=\"" + relatedItem.URL + "\" target=\"RELATEDITEM\" >" + title + "</a>";
							title = String.Empty;
						}
					}
					else
					{
						if (relatedItem.SobekCM_ID.Length > 0)
						{
                            url = "<a href=\"?b=" + relatedItem.SobekCM_ID + "\" target=\"RELATEDITEM\" >" + title + "</a>";
							title = String.Empty;
						}
					}

					if ((relatedItem.Main_Title.Title == CurrentItem.Bib_Info.Main_Title.Title) && (url.Length > 0))
						title = String.Empty;

					if ((title.Length > 0) || (url.Length > 0))
					{
						if ((title.Length > 0) && (url.Length > 0))
						{
							result.Append(Single_Citation_HTML_Row(label, title + "<br />" + url, INDENT));
						}
						else
						{
						    result.Append(title.Length > 0 ? Single_Citation_HTML_Row(label, title, INDENT) : Single_Citation_HTML_Row(label, url, INDENT));
						}
					}
				}

				// Finish out the table
				result.AppendLine(INDENT + "  </dl>");
                result.AppendLine(INDENT + "</div>");
				result.AppendLine();
			}

            if (internalUser)
			{
				List<string> codeList = new List<string>();

				result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_InternalSection\" >");
                result.AppendLine(INDENT + "<h2>" + internal_info + "</h2>");
				result.AppendLine(INDENT + "  <dl>");

				if (Code_Manager != null)
				{
                    codeList.AddRange(internalUser
				                          ? CurrentItem.Behaviors.Aggregations.Select(Aggregation => Aggregation.Code).Select(AltCode =>"<a href=\"" + CurrentMode.Base_URL + AltCode.ToLower() + url_options + "\">" +Convert_String_To_XML_Safe(Code_Manager.Get_Collection_Short_Name(AltCode)) + "</a> ( " +AltCode + " )")
				                          : CurrentItem.Behaviors.Aggregations.Select(Aggregation => Aggregation.Code).Select(AltCode =>"<a href=\"" + CurrentMode.Base_URL + AltCode.ToLower() + url_options + "\">" +Convert_String_To_XML_Safe(Code_Manager.Get_Collection_Short_Name(AltCode)) + "</a>"));
				    Add_Citation_HTML_Rows("Aggregations", codeList, INDENT, result);
					codeList.Clear();
				}

                if (internalUser)
				{
					if (CurrentItem.Behaviors.Ticklers_Count > 0)
					{
						Add_Citation_HTML_Rows("Ticklers", CurrentItem.Behaviors.Ticklers, INDENT, result);
					}
				}

				//                result.Append(Single_Citation_HTML_Row("Skins", currentItem.METS.String_RecordStatus, indent));

				result.AppendLine(INDENT + "  </dl>");
				result.AppendLine(INDENT + "</div>");
				result.AppendLine();
			}

            if (internalUser)
			{

				result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_MetsSection\" >");
			    result.AppendLine(INDENT + "<h2>" + mets_info + "</h2>");
				result.AppendLine(INDENT + "  <dl>");
				result.Append(Single_Citation_HTML_Row("Format", CurrentItem.Bib_Info.SobekCM_Type_String, INDENT));
				result.Append(Single_Citation_HTML_Row("Creation Date", CurrentItem.METS_Header.Create_Date.ToShortDateString(), INDENT));
				result.Append(Single_Citation_HTML_Row("Last Modified", CurrentItem.METS_Header.Modify_Date.ToShortDateString(), INDENT));
				result.Append(Single_Citation_HTML_Row("Last Type", CurrentItem.METS_Header.RecordStatus, INDENT));
				result.Append(Single_Citation_HTML_Row("Last User", CurrentItem.METS_Header.Creator_Individual, INDENT));
				result.Append(Single_Citation_HTML_Row("System Folder", CurrentItem.Web.AssocFilePath.Replace("/", "\\"), INDENT));

				result.AppendLine(INDENT + "  </dl>");
				result.AppendLine(INDENT + "</div>");
				result.AppendLine();
			}

            if ((CurrentUser != null) && (CurrentUser.Is_System_Admin))
			{
				result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_AdminSection\" >");
				result.AppendLine(INDENT + "<h2>" + system_info + "</h2>");
				result.AppendLine(INDENT + "  <dl>");
                result.Append(Single_Citation_HTML_Row("Item Primary Key", CurrentItem.Web.ItemID.ToString(), INDENT));
                result.Append(Single_Citation_HTML_Row("Group Primary Key", CurrentItem.Web.GroupID.ToString(), INDENT));
				result.AppendLine(INDENT + "  </dl>");
				result.AppendLine(INDENT + "</div>");
				result.AppendLine();
			}

			result.AppendLine(INDENT + "<br />");
			result.AppendLine("</div>");

			// Return the built string
			return result.ToString();
		}



		private void Add_Citation_HTML_Rows(string Row_Name, ReadOnlyCollection<string> Values, string Indent, StringBuilder Results)
		{
			// Only add if there is a value
		    if (Values.Count <= 0) return;

			Results.Append(Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:" + width + "px;\" >");

		    // Add with proper language
		    switch (CurrentMode.Language)
		    {
		        case Web_Language_Enum.French:
		            Results.Append(Translator.Get_French(Row_Name));
		            break;

		        case Web_Language_Enum.Spanish:
		            Results.Append(Translator.Get_Spanish(Row_Name));
		            break;

		        default:
		            Results.Append(Row_Name);
		            break;
		    }

		    Results.Append(": </dt>");
			Results.Append(Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:" + width + "px;\">");
		    bool first = true;
		    foreach (string thisValue in Values.Where(ThisValue => ThisValue.Length > 0))
		    {
		        if (first)
		        {
		            Results.Append(thisValue);
		            first = false;
		        }
		        else
		        {
		            Results.Append("<br />" + thisValue);
		        }
		    }
		    Results.AppendLine("</dd>");
			Results.AppendLine();
		}

	

		private void Add_Citation_HTML_Rows(string Row_Name, List<string> Values, string Indent, StringBuilder Results )
		{
			// Only add if there is a value
		    if (Values.Count <= 0) return;

			Results.Append(Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:" + width + "px;\" >");

		    // Add with proper language
		    switch (CurrentMode.Language)
		    {
		        case Web_Language_Enum.French:
		            Results.Append(Translator.Get_French(Row_Name));
		            break;

		        case Web_Language_Enum.Spanish:
		            Results.Append(Translator.Get_Spanish(Row_Name));
		            break;

		        default:
		            Results.Append(Row_Name);
		            break;
		    }

		    Results.AppendLine(": </dt>");
			Results.Append(Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:" + width + "px;\">");
		    bool first = true;
		    foreach (string thisValue in Values.Where(ThisValue => ThisValue.Length > 0))
		    {
		        if (first)
		        {
		            Results.Append(thisValue);
		            first = false;
		        }
		        else
		        {
		            Results.Append("<br />" + thisValue);
		        }
		    }
		    Results.AppendLine("</dd>");
		    Results.AppendLine();
		}

		private string Single_Citation_HTML_Row(string Row_Name, string Value, string Indent)
		{
		    // Only add if there is a value
			if (Value.Length > 0)
			{
				switch (CurrentMode.Language)
				{
					case Web_Language_Enum.French:
						return Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:" + width + "px;\">" + Translator.Get_French(Row_Name) + ": </dt>" + Environment.NewLine + Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:" + width + "px;\">" + Value + "</dd>" + Environment.NewLine + Environment.NewLine;

					case Web_Language_Enum.Spanish:
						return Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:" + width + "px;\">" + Translator.Get_Spanish(Row_Name) + ": </dt>" + Environment.NewLine + Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:" + width + "px;\">" + Value + "</dd>" + Environment.NewLine + Environment.NewLine;

					default:
						return Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:" + width + "px;\">" + Row_Name + ": </dt>" + Environment.NewLine + Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:" + width + "px;\">" + Value + "</dd>" + Environment.NewLine + Environment.NewLine;
				}
			}
		    return String.Empty;
		}

	    #endregion

		#region Nested type: Citation_Type

		private enum Citation_Type : byte { Standard = 1, MARC, Metadata, Statistics };

		#endregion
	}
}
