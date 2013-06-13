#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Resource_Object.Metadata_Modules.LearningObjects;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Item viewer displays the citation information including the basic metadata in standard and MARC format, as well as
	/// links to the metadata </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class Citation_ItemViewer : abstractItemViewer
	{
	    private User_Object currentUser;
	    private bool userCanEditItem;
		private int width = 150;

		/// <summary> Constructor for a new instance of the Citation_ItemViewer class </summary>
		/// <param name="Translator"> Language support object which handles simple translational duties </param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
		/// <param name="User_Can_Edit_Item"> Flag indicates if the current user can edit the citation information </param>
		public Citation_ItemViewer(Language_Support_Info Translator, Aggregation_Code_Manager Code_Manager, bool User_Can_Edit_Item )
		{
			translator = Translator;
			this.Code_Manager = Code_Manager;
			userCanEditItem = User_Can_Edit_Item;
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

	    /// <summary> Currently logged on user for determining rights over this item </summary>
		public User_Object Current_User
		{
			set 
			{
			    if (value == null) return;

			    userCanEditItem = value.Can_Edit_This_Item(CurrentItem);
			    currentUser = value;
			}
		}

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
						return 650;
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

			// If this is an internal user or can edit this item, ensure the extra information 
			// has been pulled for this item
			if ((userCanEditItem) || (CurrentMode.Internal_User) || ( CurrentMode.ViewerCode == "tracking" ) || ( CurrentMode.ViewerCode == "media" ) || ( CurrentMode.ViewerCode == "archive" ))
			{
				if (!CurrentItem.Tracking.Tracking_Info_Pulled)
				{
					DataSet data = SobekCM_Database.Tracking_Get_History_Archives(CurrentItem.Web.ItemID, Tracer);
					CurrentItem.Tracking.Set_Tracking_Info(data);
				}
			}

			// Determine the citation type
			Citation_Type citationType = Citation_Type.Standard;
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

			// Add the HTML for the citation
            Output.WriteLine("        <!-- CITATION ITEM VIEWER OUTPUT -->" );
			Output.WriteLine("        <td class=\"SobekCitationDisplay\">" );

			// Set the text
			const string STANDARD_VIEW = "STANDARD VIEW";
			const string MARC_VIEW = "MARC VIEW";
			const string METADATA_VIEW = "METADATA";
			const string STATISTICS_VIEW = "USAGE STATISTICS";

			// Get  the robot flag (if this is rendering for robots, the other citation views are not available)
			bool isRobot = CurrentMode.Is_Robot;

			// Add the tabs for the different citation information
			string viewer_code = CurrentMode.ViewerCode;
            Output.WriteLine("            <div class=\"SobekCitation\">");
            Output.WriteLine("              <div class=\"CitationViewSelectRow\">");
			if (citationType == Citation_Type.Standard)
			{
                Output.WriteLine("                <img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" alt=\"\" /><span class=\"tab_s\">" + STANDARD_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" alt=\"\" />");
			}
			else
			{
                Output.WriteLine("                <a href=\"" + CurrentMode.Redirect_URL("citation") + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + STANDARD_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" alt=\"\" /></a>");
			}

			if (citationType == Citation_Type.MARC)
			{
                Output.WriteLine("                <img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" alt=\"\" /><span class=\"tab_s\">" + MARC_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" alt=\"\" />");
			}
			else
			{
				if ( !isRobot )
                    Output.WriteLine("                <a href=\"" + CurrentMode.Redirect_URL("marc") + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + MARC_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" alt=\"\" /></a>");
				else
                    Output.WriteLine("                <img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + MARC_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" alt=\"\" />");
			}

			// If this item is an external link item (i.e. has related URL, but no pages or downloads) skip the next parts
			bool external_link_only = false;
			if ((CurrentItem.Bib_Info.Location.Other_URL.Length > 0) && (!CurrentItem.Divisions.Has_Files))
				external_link_only = true;

			if ((CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL) && (!external_link_only) && ( !isRobot ))
			{
				if (citationType == Citation_Type.Metadata)
				{
					Output.WriteLine("                <img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" alt=\"\" /><span class=\"tab_s\">" + METADATA_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" alt=\"\" />");
				}
				else
				{
					Output.WriteLine("                <a href=\"" + CurrentMode.Redirect_URL("metadata") + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + METADATA_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" alt=\"\" /></a>");
				}

				if (citationType == Citation_Type.Statistics)
				{
					Output.WriteLine("                <img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" alt=\"\" /><span class=\"tab_s\">" + STATISTICS_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" alt=\"\" />");
				}
				else
				{
					Output.WriteLine("                <a href=\"" + CurrentMode.Redirect_URL("usage") + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + STATISTICS_VIEW + "</span><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" alt=\"\" /></a>");
				}
			}

			Output.WriteLine("              </div>");

			// Get any search terms
			List<string> terms = new List<string>();
			if (CurrentMode.Text_Search.Trim().Length > 0)
			{
			    string[] splitter = CurrentMode.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
			    terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
			}

		    // Now, add the text
            Output.WriteLine();
			switch (citationType)
			{
				case Citation_Type.Standard:
					if ( terms.Count > 0 )
					{
                        Output.WriteLine(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(Standard_Citation_String(!isRobot, Tracer), terms) + "</div>" + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->" );
					}
					else
					{
                        Output.WriteLine(Standard_Citation_String(!isRobot, Tracer) + "</div>" + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->" );
					}
					break;

				case Citation_Type.MARC:
					if ( terms.Count > 0 )
					{
                        Output.WriteLine(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(MARC_String(Tracer), terms) + "</div>" + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->" );
					}
					else
					{
                        Output.WriteLine( MARC_String(Tracer) + "</div>" + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					}
					break;

				case Citation_Type.Metadata:
                    Output.WriteLine( Metadata_String( Tracer ) + "</div>" + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					break;

				case Citation_Type.Statistics:
                    Output.WriteLine( Statistics_String(Tracer) + "</div>" + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
					break;
			}

			CurrentMode.ViewerCode = viewer_code;
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

			builder.AppendLine("<blockquote>This item was has been viewed <%HITS%> times within <%SESSIONS%> visits.  Below are the details for overall usage for this item within this library.<br /><br />For definitions of these terms, see the <a href=\"" + CurrentMode.Base_URL + "stats/usage/definitions\" target=\"_BLANK\">definitions on the main statistics page</a>.</blockquote>");

			builder.AppendLine("  <table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
			builder.AppendLine("    <tr align=\"right\" bgcolor=\"#0022a7\" >");
			builder.AppendLine("      <th width=\"120px\" align=\"left\"><span style=\"color: White\">DATE</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">VIEWS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">VISITS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">JPEG<br />VIEWS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">ZOOMABLE<br />VIEWS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">CITATION<br />VIEWS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">THUMBNAIL<br />VIEWS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">TEXT<br />SEARCHES</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">FLASH<br />VIEWS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">MAP<br />VIEWS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">DOWNLOAD<br />VIEWS</span></th>");
			builder.AppendLine("      <th width=\"90px\"><span style=\"color: White\">STATIC<br />VIEWS</span></th>");
			builder.AppendLine("    </tr>");

			const int columns = 12;
			string last_year = String.Empty;
			if (stats != null)
			{
				foreach (DataRow thisRow in stats.Tables[1].Rows)
				{
					if (thisRow["Year"].ToString() != last_year)
					{
						builder.AppendLine("    <tr><td bgcolor=\"#7d90d5\" colspan=\"" + columns + "\"><span style=\"color: White\"><b> " + thisRow["Year"] + " STATISTICS</b></span></td></tr>");
						last_year = thisRow["Year"].ToString();
					}
					else
					{
						builder.AppendLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"" + columns + "\"></td></tr>");
					}
					builder.AppendLine("    <tr align=\"center\" >");
					builder.AppendLine("      <td align=\"left\">" + Month_From_Int(Convert.ToInt32(thisRow["Month"])) + " " + thisRow["Year"] + "</td>");

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

				builder.AppendLine("    <tr><td bgcolor=\"Black\" colspan=\"" + columns + "\"></td></tr>");
				builder.AppendLine("    <tr align=\"center\" style=\"font-weight:bold\" >");
				builder.AppendLine("      <td align=\"left\">TOTAL</td>");
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
			string greenstoneLocation = CurrentItem.Web.Source_URL + "/";
			string complete_mets = greenstoneLocation + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";
		    string marc_xml = greenstoneLocation + "marc.xml";

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((CurrentItem.Behaviors.Dark_Flag) || (CurrentItem.Behaviors.IP_Restriction_Membership > 0))
            {
                complete_mets = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";
                marc_xml = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/marc.xml";
            }


		    StringBuilder builder = new StringBuilder(3000);
			builder.AppendLine("<br />");
            builder.AppendLine("<blockquote>");

            builder.AppendLine("<blockquote>The data (or metadata) about this digital resource is available in a variety of metadata formats. For more information about these formats, see the <a href=\"http://ufdc.ufl.edu/sobekcm/metadata\">Metadata Section</a> of the <a href=\"http://ufdc.ufl.edu/sobekcm/\">Technical Aspects</a> information.</blockquote>");
            builder.AppendLine("<br />");

            builder.AppendLine("<a href=\"" + complete_mets + "\" target=\"_blank\">View Complete METS/MODS</a>");
            builder.AppendLine("<blockquote>This metadata file is the source metadata file submitted along with all the digital resource files. This contains all of the citation and processing information used to build this resource. This file follows the established <a href=\"http://www.loc.gov/standards/mets/\">Metadata Encoding and Transmission Standard</a> (METS) and <a href=\"http://www.loc.gov/standards/mods/\">Metadata Object Description Schema</a> (MODS). This METS/MODS file was just read when this item was loaded into memory and used to display all the information in the standard view and marc view within the citation.</blockquote>");

            builder.AppendLine("<br />");
            builder.AppendLine("<a href=\"" + marc_xml + "\" target=\"_blank\">View MARC XML File</a>");
            builder.AppendLine("<blockquote>The entered metadata is also converted to MARC XML format, for interoperability with other library catalog systems.  This represents the same data available in the <a href=\"" + SobekCM_Library_Settings.Base_SobekCM_Location_Relative + CurrentMode.Redirect_URL("FC2") + "\">MARC VIEW</a> except this is a static XML file.  This file follows the <a href=\"http://www.loc.gov/standards/marcxml/\">MarcXML Schema</a>.</blockquote>");

            // Should the TEI be added here?

            if (CurrentItem.Behaviors.Has_Viewer_Type(View_Enum.TEI))
            {
                // Does a TEI file exist?
                string error_message = String.Empty;
                string tei_filename = CurrentItem.Source_Directory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".tei.xml";


                if (!System.IO.File.Exists(tei_filename))
                {

                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("Citation_ItemViewer.Metadata_String", "Building default TEI file");
                    }

                    SobekCM.Resource_Object.Metadata_File_ReaderWriters.TEI_File_ReaderWriter writer = new TEI_File_ReaderWriter();

                    Dictionary<string, object> Options = new Dictionary<string, object>();
                    
                    writer.Write_Metadata(tei_filename, CurrentItem, Options, out error_message);
                }

                // Add the HTML for this
                builder.AppendLine("<br />");
                builder.AppendLine("<a href=\"" + greenstoneLocation + CurrentItem.BibID + "_" + CurrentItem.VID + ".tei.xml\" target=\"_blank\">View TEI/Text File</a>");
                builder.AppendLine("<blockquote>The full-text of this item is also available in the established standard <a href=\"http://www.tei-c.org/index.xml\">Text Encoding Initiative</a> (TEI) downloadable file.</blockquote>");

            }

            builder.AppendLine("</blockquote>");
            builder.AppendLine("<br />");

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


			List<string> collections = new List<string>();
			if (CurrentItem.Behaviors.Aggregation_Count > 0)
			{
			    collections.AddRange(from aggregation in CurrentItem.Behaviors.Aggregations select Code_Manager[aggregation.Code] into aggr where aggr != null where aggr.Type.ToUpper() == "COLLECTION" select aggr.ShortName);
			}
		  
			//// Build the value
			StringBuilder builder = new StringBuilder();

			// Add the edit item button, if the user can edit it
			if ((userCanEditItem) && (CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL))
			{
				CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
				CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
				CurrentMode.My_Sobek_SubMode = "1";
				builder.AppendLine("<blockquote><a href=\"" + CurrentMode.Redirect_URL() + "\"><img src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin + "/buttons/edit_item_button.gif\" border=\"0px\" alt=\"Edit this item\" /></a></blockquote>");
				CurrentMode.Mode = Display_Mode_Enum.Item_Display;
			}
			else
			{
                builder.AppendLine("<br />");
			}

            builder.AppendLine(CurrentItem.Get_MARC_HTML(collections, CurrentMode.Internal_User, Width, SobekCM_Library_Settings.System_Name, SobekCM_Library_Settings.System_Abbreviation));
            builder.AppendLine("<br />");
            builder.AppendLine("<br />");
            builder.AppendLine("<div style=\"color:green; text-align:center;\">The record above was auto-generated from the METS file.</div>");
		    builder.AppendLine();
            builder.AppendLine("<br />");
            builder.AppendLine("<br />");

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
            // Retrieve all the metadata modules that wil be used here
		    Zoological_Taxonomy_Info taxonInfo = CurrentItem.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
		    VRACore_Info vraInfo = CurrentItem.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
		    GeoSpatial_Information geoInfo = CurrentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
		    LearningObjectMetadata lomInfo = CurrentItem.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;

			// Compute the URL to use for all searches from the citation
			Display_Mode_Enum lastMode = CurrentMode.Mode;
			CurrentMode.Mode = Display_Mode_Enum.Results;
			CurrentMode.Search_Type = Search_Type_Enum.Advanced;
			CurrentMode.Search_String = "<%VALUE%>";
			CurrentMode.Search_Fields = "<%CODE%>";
			string search_link = "<a href=\"" + CurrentMode.Redirect_URL().Replace("&", "&amp;").Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>", "&quot;<%VALUE%>&quot;") + "\" target=\"_BLANK\">";
			string search_link_end = "</a>";
			CurrentMode.Aggregation = String.Empty;
			CurrentMode.Search_String = String.Empty;
			CurrentMode.Search_Fields = String.Empty;
			CurrentMode.Mode = lastMode;
			string url_options = CurrentMode.URL_Options();
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
				width = 180;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Citation_ItemViewer.MARC_String", "Configuring METS data into standard citation format");
			}

			// Build the strings for each section
			string mets_info = translator.Get_Translation("METS Information", CurrentMode.Language);
			string internal_info = translator.Get_Translation(CurrentMode.SobekCM_Instance_Abbreviation + " Membership", CurrentMode.Language);
			string biblio_info = translator.Get_Translation("Material Information", CurrentMode.Language);
			string subject_info = translator.Get_Translation("Subjects", CurrentMode.Language);
			string notes_info = translator.Get_Translation("Notes", CurrentMode.Language);
			string institutional_info = translator.Get_Translation("Record Information", CurrentMode.Language);
			string related_items = translator.Get_Translation("Related Items", CurrentMode.Language);
			string system_info = translator.Get_Translation("System Admin Information", CurrentMode.Language);
			string zoological_taxonomy = translator.Get_Translation("Zoological Taxonomic Information", CurrentMode.Language);
            string learningobject_title = translator.Get_Translation("Learning Resource Information", CurrentMode.Language);
			List<string> tempList = new List<string>();

			// Build the value
			// Use string builder to build this
			const string indent = "    ";

			StringBuilder result = new StringBuilder();

			// Start this table
			result.Append(indent + "<table width=\"100%\" cellpadding=\"5px\" class=\"SobekCitationSection1\">\n");
			result.Append(indent + "  <tr> <td colspan=\"3\"> </td> </tr>\n");

			// If this item is an external link item (i.e. has related URL, but no pages or downloads) the PURL should display as the Other URL
			bool external_link_only = false;
			if ((CurrentItem.Bib_Info.Location.Other_URL.Length > 0) && (!CurrentItem.Divisions.Has_Files))
				external_link_only = true;

			// Add major links first
			if (external_link_only)
			{
				string url_shortened = CurrentItem.Bib_Info.Location.Other_URL;
				if (url_shortened.Length > 200)
					url_shortened = url_shortened.Substring(0, 200) + "...";
				result.Append(Single_Citation_HTML_Row("External Link", "<a href=\"" + CurrentItem.Bib_Info.Location.Other_URL.Replace("&", "&amp;") + "\">" + url_shortened.Replace("&", "&amp;") + "</a>", indent));
			}
			else
			{
				// Does this have a PURL in the METS package?
				if (CurrentItem.Bib_Info.Location.PURL.Length > 0)
				{
					string packagePurl = CurrentItem.Bib_Info.Location.PURL;
					if (packagePurl.IndexOf("http://") < 0)
						packagePurl = "http://" + packagePurl;
					result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + packagePurl + "\">" + packagePurl + "</a>", indent));
				}
				else
				{
					if (CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL)
					{
						// Does thir portal have a special PURL to be used here?
						if (!String.IsNullOrEmpty(CurrentMode.Portal_PURL))
						{
							string link = CurrentMode.Portal_PURL + CurrentItem.BibID + "/" + CurrentItem.VID;
							result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + link + "\">" + link + "</a>", indent));
						}
						else
						{
							string link = CurrentMode.Base_URL + CurrentItem.BibID + "/" + CurrentItem.VID;
							result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + link + "\">" + link + "</a>", indent));
						}
					}
					else
					{
						// Does thir portal have a special PURL to be used here?
						if (!String.IsNullOrEmpty(CurrentMode.Portal_PURL))
						{
							string link = CurrentMode.Portal_PURL + CurrentItem.BibID;
							result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + link + "\">" + link + "</a>", indent));
						}
						else
						{
							string link = CurrentMode.Base_URL + CurrentItem.BibID;
							result.Append(Single_Citation_HTML_Row("Permanent Link", "<a href=\"" + link + "\">" + link + "</a>", indent));
						}
					}
				}
			}

			// If there is an EAD link, link to that
			if ((CurrentItem.Bib_Info.hasLocationInformation) && (CurrentItem.Bib_Info.Location.EAD_Name.Length > 0) || (CurrentItem.Bib_Info.Location.EAD_URL.Length > 0))
			{
				if (CurrentItem.Bib_Info.Location.EAD_Name.Length == 0)
				{
					result.Append(Single_Citation_HTML_Row("Finding Guide", "<a href=\"" + CurrentItem.Bib_Info.Location.EAD_URL + "\">" + CurrentItem.Bib_Info.Location.EAD_URL + "</a>", indent));
				}
				else
				{
				    result.Append(CurrentItem.Bib_Info.Location.EAD_URL.Length > 0
				                      ? Single_Citation_HTML_Row("Finding Guide","<a href=\"" + CurrentItem.Bib_Info.Location.EAD_URL + "\">" +Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.EAD_Name) +"</a>", indent)
				                      : Single_Citation_HTML_Row("Finding Guide",Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.EAD_Name),indent));
				}
			}

            result.AppendLine(indent + "</table>");

			result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection2\">");
		    result.AppendLine(indent + "<tr>");
		    result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle2\"><b>&nbsp;" + biblio_info + "</b></td>");
            result.AppendLine( indent + "</tr>");

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
						case Title_Type_Enum.alternative:
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

						case Title_Type_Enum.uniform:
							uniform_titles.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisTitle.Title).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "TI") + Convert_String_To_XML_Safe(thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim() + search_link_end);
							break;

						case Title_Type_Enum.translated:
							translated_titles.Add((Convert_String_To_XML_Safe(thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle) + " ( <i>" + thisTitle.Language + "</i> )").Trim());
							break;

						case Title_Type_Enum.abbreviated:
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
								spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(hieroSubj.Country).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CO") + hieroSubj.Country + search_link_end);
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
								spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(hieroSubj.State).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "ST") + hieroSubj.State + search_link_end);
							}
							if (hieroSubj.Territory.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(hieroSubj.Territory);
							}
							if (hieroSubj.County.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(hieroSubj.County).Replace(",", "").Replace("&", "".Replace(" ", "+"))).Replace("<%CODE%>", "CT") + hieroSubj.County + search_link_end);
							}
							if (hieroSubj.City.Length > 0)
							{
								if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
								spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(hieroSubj.City).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CI") + hieroSubj.City + search_link_end);
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
										genres.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(Convert_String_To_XML_Safe(thisGenre)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "GE") + Convert_String_To_XML_Safe(thisGenre) + search_link_end + " &nbsp; ( <i>" + baseSubject.Authority.ToLower() + "</i> )");
									}
									else
									{
										genres.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(Convert_String_To_XML_Safe(thisGenre)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "GE") + Convert_String_To_XML_Safe(thisGenre) + search_link_end);
									}
								}
							}
							else
							{
								if ((thisSubject.Authority.Length > 0) && (thisSubject.Authority.ToUpper() != "NONE"))
								{
									subjects.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisSubject.ToString(false)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "SU") + thisSubject.ToString(false) + search_link_end + " &nbsp; ( <i>" + thisSubject.Authority.ToLower() + "</i> )");
								}
								else
								{
									subjects.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisSubject.ToString(false)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "SU") + thisSubject.ToString(false) + search_link_end);
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
					if (thisGenre.Authority.Length > 0)
					{
						genres.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(Convert_String_To_XML_Safe(thisGenre.Genre_Term)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "GE") + Convert_String_To_XML_Safe(thisGenre.Genre_Term) + search_link_end + " &nbsp; ( <i>" + thisGenre.Authority.ToLower() + "</i> )");
					}
					else
					{
						genres.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(Convert_String_To_XML_Safe(thisGenre.Genre_Term)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "GE") + Convert_String_To_XML_Safe(thisGenre.Genre_Term) + search_link_end);
					}
				}
			}

			// Add the titles
			result.Append(Single_Citation_HTML_Row("Title", Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Main_Title.NonSort + " " + CurrentItem.Bib_Info.Main_Title.Title + " " + CurrentItem.Bib_Info.Main_Title.Subtitle).Trim(), indent));
			if (CurrentItem.Bib_Info.hasSeriesTitle)
				result.Append(Single_Citation_HTML_Row("Series Title", search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(CurrentItem.Bib_Info.SeriesTitle.Title).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "TI") + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.SeriesTitle.NonSort + " " + CurrentItem.Bib_Info.SeriesTitle.Title + " " + CurrentItem.Bib_Info.SeriesTitle.Subtitle).Trim() + search_link_end, indent));
			Add_Citation_HTML_Rows("Uniform Title", uniform_titles, indent, result);
			foreach (KeyValuePair<string, List<string>> altTitleType in alternative_titles)
			{
				Add_Citation_HTML_Rows(altTitleType.Key, altTitleType.Value, indent, result);
			}
			Add_Citation_HTML_Rows("Translated Title", translated_titles, indent, result);
			Add_Citation_HTML_Rows("Abbreviated Title", abbreviated_titles, indent, result);

			if ((CurrentItem.Bib_Info.Original_Description.Extent.Length > 0) && (CurrentItem.Bib_Info.Original_Description.Extent.ToUpper().Trim() != CurrentItem.Bib_Info.SobekCM_Type_String.ToUpper().Trim()))
			{
				result.Append(Single_Citation_HTML_Row("Physical Description", Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Original_Description.Extent), indent));
			}
			else
			{
				result.Append(Single_Citation_HTML_Row("Physical Description", CurrentItem.Bib_Info.SobekCM_Type_String, indent));
			}

			result.Append(Single_Citation_HTML_Row("Scale", scale, indent));

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
						languageList.Add(Convert_String_To_XML_Safe(language_text));
					}
					else
					{
						languageList.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(Convert_String_To_XML_Safe(language_text)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "LA") + Convert_String_To_XML_Safe(language_text) + search_link_end);

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
								languageList.Add(Convert_String_To_XML_Safe(language_text));
							}
							else
							{
								languageList.Add(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(Convert_String_To_XML_Safe(language_text)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "LA") + Convert_String_To_XML_Safe(language_text) + search_link_end);

							}
						}
					}
				}
			}
			Add_Citation_HTML_Rows("Language", languageList, indent, result);


			List<string> creators = new List<string>();
			List<string> conferences = new List<string>();
			if ((CurrentItem.Bib_Info.hasMainEntityName) && (CurrentItem.Bib_Info.Main_Entity_Name.Full_Name.Length > 0))
			{
				if (CurrentItem.Bib_Info.Main_Entity_Name.Name_Type == Name_Info_Type_Enum.conference)
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
					string name_linked = search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisName.ToString(false)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "AU") + nameBuilder + search_link_end;
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
					if (thisName.Name_Type == Name_Info_Type_Enum.conference)
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
						string name_linked = search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisName.ToString(false)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "AU") + nameBuilder + search_link_end;
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
			Add_Citation_HTML_Rows("Creator", creators, indent, result);
			Add_Citation_HTML_Rows("Conference", conferences, indent, result);

			tempList.Clear();
			if (CurrentItem.Bib_Info.Affiliations_Count > 0)
			{
			    tempList.AddRange(CurrentItem.Bib_Info.Affiliations.Select(thisAffiliation => thisAffiliation.ToString()));
			    Add_Citation_HTML_Rows("Affiliation", tempList, indent, result);
			}

		    if ((CurrentItem.Bib_Info.hasDonor) && (CurrentItem.Bib_Info.Donor.Full_Name.Length > 0))
			{
				result.Append(Single_Citation_HTML_Row("Donor", search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(CurrentItem.Bib_Info.Donor.ToString()).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "DO") + CurrentItem.Bib_Info.Donor + search_link_end, indent));
			}

			if (CurrentItem.Bib_Info.Publishers_Count > 0)
			{
				List<string> pubs = CurrentItem.Bib_Info.Publishers.Select(thisPublisher => search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(thisPublisher.Name).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "PU") + Convert_String_To_XML_Safe(thisPublisher.Name) + search_link_end).ToList();
			    Add_Citation_HTML_Rows("Publisher", pubs, indent, result);

				List<string> pub_places = new List<string>();
				foreach (Publisher_Info thisPublisher in CurrentItem.Bib_Info.Publishers)
				{
				    pub_places.AddRange(thisPublisher.Places.Select(place => search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(Convert_String_To_XML_Safe(place.Place_Text)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "PP") + Convert_String_To_XML_Safe(place.Place_Text) + search_link_end));
				}

				Add_Citation_HTML_Rows("Place of Publication", pub_places, indent, result);
			}

			if (CurrentItem.Bib_Info.Manufacturers_Count > 0)
			{
				List<string> manufacturers = CurrentItem.Bib_Info.Manufacturers.Select(thisManufacturer => Convert_String_To_XML_Safe(thisManufacturer.Name)).ToList();
			    Add_Citation_HTML_Rows("Manufacturer", manufacturers, indent, result);
			}

			if (CurrentItem.Bib_Info.Origin_Info.Date_Created.Length > 0)
			{
				if (CurrentItem.Bib_Info.Origin_Info.Date_Created.Trim() != "-1")
				{
					result.Append(Single_Citation_HTML_Row("Creation Date", CurrentItem.Bib_Info.Origin_Info.Date_Created, indent));
				}
			}

			if (CurrentItem.Bib_Info.Origin_Info.Date_Issued.Length > 0)
			{
				if (CurrentItem.Bib_Info.Origin_Info.Date_Issued.Trim() != "-1")
				{
					result.Append(Single_Citation_HTML_Row("Publication Date", CurrentItem.Bib_Info.Origin_Info.Date_Issued, indent));
				}
			}
			else
			{
				result.Append(Single_Citation_HTML_Row("Publication Date", CurrentItem.Bib_Info.Origin_Info.MARC_DateIssued, indent));
			}

			if (CurrentItem.Bib_Info.Origin_Info.Date_Copyrighted.Trim() != "-1")
			{
				result.Append(Single_Citation_HTML_Row("Copyright Date", CurrentItem.Bib_Info.Origin_Info.Date_Copyrighted, indent));
			}

			if (CurrentItem.Bib_Info.Origin_Info.Frequencies_Count > 0)
			{
				List<string> frequencies = new List<string>();
				foreach (Origin_Info_Frequency thisFrequency in CurrentItem.Bib_Info.Origin_Info.Frequencies)
				{
					if (!frequencies.Contains(thisFrequency.Term.ToLower()))
						frequencies.Add(thisFrequency.Term.ToLower());
				}
				Add_Citation_HTML_Rows("Frequency", frequencies, indent, result);
			}

			// Collect the state/edition information
			if ((CurrentItem.Bib_Info.Origin_Info.Edition.Length > 0) || (( vraInfo != null ) && ( vraInfo.State_Edition_Count > 0)))
			{
				string edition_title = "Edition";
				List<string> editions = new List<string>();
				if (CurrentItem.Bib_Info.Origin_Info.Edition.Length > 0)
					editions.Add(CurrentItem.Bib_Info.Origin_Info.Edition);

                if ((vraInfo != null) && (vraInfo.State_Edition_Count > 0))
				{
				    edition_title = "State / Edition";
				    editions.AddRange(vraInfo.State_Editions);
				}
			    Add_Citation_HTML_Rows(edition_title, editions, indent, result);
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
						materialsBuilder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(Convert_String_To_XML_Safe(materials.Materials)).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "MA") + Convert_String_To_XML_Safe(materials.Materials) + search_link_end);
					}
				}
				if (materialsBuilder.Length > 0)
				{
					if (materials_type.Length > 0)
						materialsBuilder.Append(" ( " + Convert_String_To_XML_Safe(materials_type) + " )");
				}
				result.Append(Single_Citation_HTML_Row("Materials", materialsBuilder.ToString(), indent));
			}

			// Collect and display all the measurements information
			if (( vraInfo != null ) && ( vraInfo.Measurement_Count > 0))
			{
				List<string> measurements = (from measurement in vraInfo.Measurements where measurement.Measurements.Length > 0 select measurement.Measurements).ToList();
			    Add_Citation_HTML_Rows("Measurements", measurements, indent, result);
			}

			// Display all cultural context information
			if (( vraInfo != null ) && ( vraInfo.Cultural_Context_Count > 0))
			{
				tempList.Clear();
			    tempList.AddRange(vraInfo.Cultural_Contexts.Select(Convert_String_To_XML_Safe));
			    Add_Citation_HTML_Rows("Cultural Context", tempList, indent, result);
			}

			// Display all style/period information
			if (( vraInfo != null ) && ( vraInfo.Style_Period_Count > 0))
			{
				tempList.Clear();
			    tempList.AddRange( vraInfo.Style_Periods.Select(Convert_String_To_XML_Safe));
			    Add_Citation_HTML_Rows("Style/Period", tempList, indent, result);
			}

			// Display all technique information
			if (( vraInfo != null ) && ( vraInfo.Technique_Count > 0))
			{
				tempList.Clear();
			    tempList.AddRange( vraInfo.Techniques.Select(Convert_String_To_XML_Safe));
			    Add_Citation_HTML_Rows("Technique", tempList, indent, result);
			}

			if (CurrentItem.Bib_Info.Containers_Count > 0)
			{
				StringBuilder physicalLocationBuilder = new StringBuilder(1000);
				physicalLocationBuilder.Append("<table>");
				foreach (Finding_Guide_Container thisContainer in CurrentItem.Bib_Info.Containers)
				{
					physicalLocationBuilder.Append("<tr><td>" + thisContainer.Type + ": </td><td>" + thisContainer.Name + "</td></tr>");
				}
				physicalLocationBuilder.Append("</table>");
				result.Append(Single_Citation_HTML_Row("Physical Location", physicalLocationBuilder.ToString(), indent));
			}
            result.AppendLine(indent + "</table>");

			// Add the taxonomic data if it exists
			if (( taxonInfo != null ) && ( taxonInfo.hasData ))
			{
                result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection1\">");
                result.AppendLine(indent + "<tr>");
                result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + zoological_taxonomy + "</b></td>");
                result.AppendLine(indent + "</tr>");

                result.Append(Single_Citation_HTML_Row("Scientific Name", taxonInfo.Scientific_Name, indent));
                result.Append(Single_Citation_HTML_Row("Kingdom", taxonInfo.Kingdom, indent));
                result.Append(Single_Citation_HTML_Row("Phylum", taxonInfo.Phylum, indent));
                result.Append(Single_Citation_HTML_Row("Class", taxonInfo.Class, indent));
                result.Append(Single_Citation_HTML_Row("Order", taxonInfo.Order, indent));
                result.Append(Single_Citation_HTML_Row("Family", taxonInfo.Family, indent));
                result.Append(Single_Citation_HTML_Row("Genus", taxonInfo.Genus, indent));
                result.Append(Single_Citation_HTML_Row("Species", taxonInfo.Specific_Epithet, indent));
                result.Append(Single_Citation_HTML_Row("Taxonomic Rank", taxonInfo.Taxonomic_Rank, indent));
                result.Append(Single_Citation_HTML_Row("Common Name", taxonInfo.Common_Name, indent));

                result.AppendLine(indent + "</table>");
			}

            // Add the learning object metadata if it exists
            if ((lomInfo != null) && (lomInfo.hasData))
            {
                result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection1\">");
                result.AppendLine(indent + "<tr>");
                result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + learningobject_title + "</b></td>");
                result.AppendLine(indent + "</tr>");

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

                    result.Append(Single_Citation_HTML_Row("Aggregation Level", lom_temp, indent));
                }
                

                // Add the LOM Learning resource type
                if (lomInfo.LearningResourceTypes.Count > 0)
                {
                    List<string> types = new List<string>();

                    foreach (LOM_VocabularyState thisType in lomInfo.LearningResourceTypes)
                    {
                        types.Add(thisType.Value);
                    }

                    Add_Citation_HTML_Rows("Learning Resource Type", types, indent, result);
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

                    result.Append(Single_Citation_HTML_Row("Status", lom_temp, indent));
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

                    result.Append(Single_Citation_HTML_Row("Interactivity Type", lom_temp, indent));
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

                    result.Append(Single_Citation_HTML_Row("Interactivity Level", lom_temp, indent));
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

                    result.Append(Single_Citation_HTML_Row("Interactivity Level", lom_temp, indent));
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
                                roles.Add("Teacher");
                                break;

                            case IntendedEndUserRoleEnum.learner:
                                roles.Add("Learner");
                                break;

                            case IntendedEndUserRoleEnum.author:
                                roles.Add("Author");
                                break;

                            case IntendedEndUserRoleEnum.manager:
                                roles.Add("Manager");
                                break;

                        }
                    }

                    Add_Citation_HTML_Rows("Intended User Roles", roles, indent, result);
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

                    Add_Citation_HTML_Rows("Context", contexts, indent, result);
                }


                // Add the LOM Typical Age Range
                if (lomInfo.TypicalAgeRanges.Count > 0)
                {
                    List<string> ranges = new List<string>();

                    foreach (LOM_LanguageString thisUser in lomInfo.TypicalAgeRanges)
                    {
                        ranges.Add(thisUser.Value);
                    }

                    Add_Citation_HTML_Rows("Typical Age Range", ranges, indent, result);
                }

                // Add the typical learning time
                result.Append(Single_Citation_HTML_Row("Typical Learning Time", lomInfo.TypicalLearningTime, indent));

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

                    Add_Citation_HTML_Rows("System Requirements", reqs, indent, result);
                }

                result.AppendLine(indent + "</table>");
            }


			// Add the subjects and coordinate information if that exists
			if ((subjects.Count > 0) || (genres.Count > 0) || (CurrentItem.Bib_Info.TemporalSubjects_Count > 0) || (hierGeo.Count > 0) ||
                ((geoInfo != null) && (geoInfo.hasData) && ((geoInfo.Point_Count > 0) || (geoInfo.Polygon_Count > 0))))
			{
                result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection1\">");
                result.AppendLine(indent + "<tr>");
                result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + subject_info + "</b></td>");
                result.AppendLine(indent + "</tr>");

				Add_Citation_HTML_Rows("Subjects / Keywords", subjects, indent, result);
				Add_Citation_HTML_Rows("Genre", genres, indent, result);
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
					Add_Citation_HTML_Rows("Temporal Coverage", tempList, indent, result);
				}
				Add_Citation_HTML_Rows("Spatial Coverage", hierGeo, indent, result);

                if (( geoInfo != null) && ( geoInfo.hasData))
				{
                    for (int i = 0; i < geoInfo.Point_Count; i++)
					{
                        if (geoInfo.Points[i].Label.Length > 0)
						{
                            tempList.Add(geoInfo.Points[i].Latitude + " x " + geoInfo.Points[i].Longitude + " ( " + Convert_String_To_XML_Safe(geoInfo.Points[i].Label) + " )");
						}
						else
						{
                            tempList.Add(geoInfo.Points[i].Latitude + " x " + geoInfo.Points[i].Longitude);
						}
					}
					Add_Citation_HTML_Rows("Coordinates", tempList, indent, result);

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
						Add_Citation_HTML_Rows("Polygon", tempList, indent, result);
					}
				}

				tempList.Clear();
				if (CurrentItem.Bib_Info.Target_Audiences_Count > 0)
				{
					foreach (TargetAudience_Info thisAudience in CurrentItem.Bib_Info.Target_Audiences)
					{
						if (thisAudience.Authority.Length > 0)
						{
							tempList.Add(Convert_String_To_XML_Safe(thisAudience.Audience) + " &nbsp; ( <i>" + thisAudience.Authority.ToLower() + "</i> )");
						}
						else
						{
							tempList.Add(Convert_String_To_XML_Safe(thisAudience.Audience));
						}
					}
				}
                result.AppendLine(indent + "</table>");
			}

			if ((CurrentItem.Bib_Info.Abstracts_Count > 0) || (CurrentItem.Bib_Info.Notes_Count > 0) || (CurrentItem.Behaviors.User_Tags_Count > 0))
			{
				// Ensure that if this user is non-internal, the only notes field is not internal
				bool valid_notes_exist = CurrentMode.Internal_User;
				if (!valid_notes_exist)
				{
					if (CurrentItem.Bib_Info.Abstracts_Count > 0)
						valid_notes_exist = true;
					else
					{
						if (CurrentItem.Bib_Info.Notes.Any(thisNote => thisNote.Note_Type != Note_Type_Enum.internal_comments))
						{
						    valid_notes_exist = true;
						}
					}
				}

				if ((valid_notes_exist) || (CurrentItem.Bib_Info.Abstracts_Count > 0) || (CurrentItem.Behaviors.User_Tags_Count > 0) || (( vraInfo != null ) && ( vraInfo.Inscription_Count > 0)))
				{
					result.Append(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection1\">\n");
					result.Append(indent + "<tr>\n" + indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + notes_info + "</b></td>\n" + indent + "</tr>\n");

					if (CurrentItem.Bib_Info.Abstracts_Count > 0)
					{
						foreach (Abstract_Info thisAbstract in CurrentItem.Bib_Info.Abstracts)
						{
						    result.Append(thisAbstract.Display_Label.Length > 0
						                      ? Single_Citation_HTML_Row(Convert_String_To_XML_Safe(thisAbstract.Display_Label), Convert_String_To_XML_Safe(thisAbstract.Abstract_Text), indent)
						                      : Single_Citation_HTML_Row("Abstract",Convert_String_To_XML_Safe(thisAbstract.Abstract_Text), indent));
						}
					}

					if (CurrentItem.Bib_Info.Notes_Count > 0)
					{
						foreach (Note_Info thisNote in CurrentItem.Bib_Info.Notes)
						{
							if (thisNote.Note_Type != Note_Type_Enum.NONE)
							{
								if ((thisNote.Note_Type != Note_Type_Enum.internal_comments) || (CurrentMode.Internal_User))
								{
									result.Append(Single_Citation_HTML_Row(thisNote.Note_Type_Display_String, Convert_String_To_XML_Safe(thisNote.Note), indent));
								}
							}
							else
							{
								result.Append(Single_Citation_HTML_Row("General Note", Convert_String_To_XML_Safe(thisNote.Note), indent));
							}
						}
					}

					if (( vraInfo != null ) && ( vraInfo.Inscription_Count > 0))
					{
						Add_Citation_HTML_Rows("Inscription", vraInfo.Inscriptions, indent, result);
					}

					if (CurrentItem.Behaviors.User_Tags_Count > 0)
					{
						int tag_counter = 1;
						foreach (Descriptive_Tag tag in CurrentItem.Behaviors.User_Tags)
						{
							if ((currentUser != null) && (currentUser.UserID == tag.UserID))
							{
								string citation_value = Convert_String_To_XML_Safe(tag.Description_Tag) + " <br /><i> Description added by you on " + tag.Date_Added.ToShortDateString() + "</i><br />( <a href=\"\" name=\"edit_describe_button" + tag_counter + "\" id=\"edit_describe_button" + tag_counter + "\" onclick=\"return edit_tag( 'edit_describe_button" + tag_counter + "', " + tag.TagID + ", '" + HttpUtility.HtmlEncode(tag.Description_Tag) + "');\">edit</a> | <a href=\"\" onclick=\"return delete_tag(" + tag.TagID + ");\">delete</a> )";
								result.Append(indent + "    <tr>\n" + indent + "      <td width=\"15\"> </td>\n" + indent + "      <td width=\"" + width + "\" valign=\"top\"><b>" + Translator.Get_Translation("User Description", CurrentMode.Language) + ": </b></td>\n" + indent + "      <td style=\"background-color:#eeee88\">" + citation_value + "</td>\n" + indent + "    </tr>\n");
							}
							else
							{
								result.Append(Single_Citation_HTML_Row("User Description", Convert_String_To_XML_Safe(tag.Description_Tag) + " <br /><i> Description added by " + tag.UserName + " on " + tag.Date_Added.ToShortDateString() + "</i>", indent));
							}

							tag_counter++;
						}
					}

                    result.AppendLine(indent + "</table>");
				}
			}

            result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection2\">");
            result.AppendLine(indent + "<tr>");
            result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle2\"><b>&nbsp;" + institutional_info + "</b></td>");
            result.AppendLine(indent + "</tr>");

			// Add the SOURCE INSTITUTION information
			if (CurrentItem.Bib_Info.Source.Statement.Length > 0)
			{
				if (CurrentItem.Bib_Info.Source.Code.Length > 0)
				{
					// Include the code for internal users
					string codeString = String.Empty;
					if (CurrentMode.Internal_User)
						codeString = " ( i" + CurrentItem.Bib_Info.Source.Code + " )";

					if ((Code_Manager != null) && (Code_Manager.isValidCode("i" + CurrentItem.Bib_Info.Source.Code)))
					{
						Item_Aggregation_Related_Aggregations sourceAggr = Code_Manager["i" + CurrentItem.Bib_Info.Source.Code];
						if (sourceAggr.Active)
						{
						    result.Append(sourceAggr.External_Link.Length > 0
						                      ? Single_Citation_HTML_Row("Source Institution",Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) +codeString + " ( <a href=\"" + CurrentMode.Base_URL + "i" +CurrentItem.Bib_Info.Source.Code + url_options + "\">" +CurrentMode.SobekCM_Instance_Abbreviation +" page</a> | <a href=\"" + sourceAggr.External_Link +"\">external link</a> )", indent)
                                              : Single_Citation_HTML_Row("Source Institution","<a href=\"" + CurrentMode.Base_URL + "i" +CurrentItem.Bib_Info.Source.Code + url_options + "\">" +Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) +"</a> " + codeString, indent));
						}
						else
						{
						    result.Append(sourceAggr.External_Link.Length > 0
						                      ? Single_Citation_HTML_Row("Source Institution", "<a href=\"" + sourceAggr.External_Link + "\">" + Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + "</a> " + codeString, indent)
						                      : Single_Citation_HTML_Row("Source Institution", Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + codeString, indent));
						}
					}
					else
					{
						result.Append(Single_Citation_HTML_Row("Source Institution", Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement) + codeString, indent));
					}
				}
				else
				{
					result.Append(Single_Citation_HTML_Row("Source Institution", Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Source.Statement), indent));
				}
			}

			// Add the HOLDING LOCATION information
			if ((CurrentItem.Bib_Info.hasLocationInformation) && (CurrentItem.Bib_Info.Location.Holding_Name.Length > 0))
			{
				if (CurrentItem.Bib_Info.Location.Holding_Code.Length > 0)
				{
					// Include the code for internal users
					string codeString = String.Empty;
					if (CurrentMode.Internal_User)
						codeString = " ( i" + CurrentItem.Bib_Info.Location.Holding_Code + " )";

					if ((Code_Manager != null) && (Code_Manager.isValidCode("i" + CurrentItem.Bib_Info.Location.Holding_Code)))
					{
						Item_Aggregation_Related_Aggregations holdingAggr = Code_Manager["i" + CurrentItem.Bib_Info.Location.Holding_Code];
						if (holdingAggr.Active)
						{
						    result.Append(holdingAggr.External_Link.Length > 0
						                      ? Single_Citation_HTML_Row("Holding Location", Convert_String_To_XML_Safe( CurrentItem.Bib_Info.Location.Holding_Name) + codeString + " ( <a href=\"" + CurrentMode.Base_URL + "i" + CurrentItem.Bib_Info.Location.Holding_Code + url_options + "\">" + CurrentMode.SobekCM_Instance_Abbreviation + " page</a> | <a href=\"" + holdingAggr.External_Link +"\">external link</a> )", indent)
						                      : Single_Citation_HTML_Row("Holding Location","<a href=\"" + CurrentMode.Base_URL + "i" + CurrentItem.Bib_Info.Location.Holding_Code.ToLower() + url_options + "\">" + Convert_String_To_XML_Safe( CurrentItem.Bib_Info.Location.Holding_Name) + "</a> " + codeString, indent));
						}
						else
						{
						    result.Append(holdingAggr.External_Link.Length > 0
						                      ? Single_Citation_HTML_Row("Holding Location", "<a href=\"" + holdingAggr.External_Link + "\">" + Convert_String_To_XML_Safe( CurrentItem.Bib_Info.Location.Holding_Name) + "</a> " + codeString, indent)
						                      : Single_Citation_HTML_Row("Holding Location", Convert_String_To_XML_Safe( CurrentItem.Bib_Info.Location.Holding_Name) + codeString, indent));
						}
					}
					else
					{
						result.Append(Single_Citation_HTML_Row("Holding Location", Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.Holding_Name) + codeString, indent));
					}
				}
				else
				{
					result.Append(Single_Citation_HTML_Row("Holding Location", Convert_String_To_XML_Safe(CurrentItem.Bib_Info.Location.Holding_Name), indent));
				}
			}


			// Add the RIGHTS STATEMENT
			string rights_statement = "All rights reserved by the source institution and holding location.";
			if (CurrentItem.Bib_Info.Access_Condition.Text.Length > 0)
			{
				rights_statement = CurrentItem.Bib_Info.Access_Condition.Text;
			}
			result.Append(indent + "    <tr>\n" + indent + "      <td width=\"15\"> </td>\n" + indent + "      <td width=\"" + width + "\" valign=\"top\"><b>" + Translator.Get_Translation("Rights Management", CurrentMode.Language) + ": </b></td>\n" + indent + "      <td>");
			const string SEE_TEXT = "See License Deed";
			if (rights_statement.IndexOf("http://") == 0)
			{
				rights_statement = "<a href=\"" + rights_statement + "\" target=\"RIGHTS\" >" + rights_statement + "</a>";
			}
			else
			{
				if (rights_statement.IndexOf("[cc by-nc-nd]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-nc-nd]", "<br /><a href=\"http://creativecommons.org/licenses/by-nc-nd/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + CurrentMode.Default_Images_URL + "cc_by_nc_nd.png\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by-nc-sa]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-nc-sa]", "<br /><a href=\"http://creativecommons.org/licenses/by-nc-sa/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + CurrentMode.Default_Images_URL + "cc_by_nc_sa.png\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by-nc]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-nc]", "<br /><a href=\"http://creativecommons.org/licenses/by-nc/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + CurrentMode.Default_Images_URL + "cc_by_nc.png\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by-nd]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-nd]", "<br /><a href=\"http://creativecommons.org/licenses/by-nd/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + CurrentMode.Default_Images_URL + "cc_by_nd.png\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by-sa]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by-sa]", "<br /><a href=\"http://creativecommons.org/licenses/by-sa/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + CurrentMode.Default_Images_URL + "cc_by_sa.png\" /></a>");
				}
				if (rights_statement.IndexOf("[cc by]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc by]", "<br /><a href=\"http://creativecommons.org/licenses/by/3.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + CurrentMode.Default_Images_URL + "cc_by.png\" /></a>");
				}
				if (rights_statement.IndexOf("[cc0]") >= 0)
				{
					rights_statement = rights_statement.Replace("[cc0]", "<br /><a href=\"http://creativecommons.org/publicdomain/zero/1.0/\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + CurrentMode.Default_Images_URL + "cc_zero.png\" /></a>");
				}
			}
			result.Append(rights_statement + "</td>\n" + indent + "    </tr>\n");

			// Add the IDENTIFIERS
			tempList.Clear();
			if (CurrentItem.Bib_Info.Identifiers_Count > 0)
			{
				foreach (Identifier_Info thisIdentifier in CurrentItem.Bib_Info.Identifiers)
				{
					if (thisIdentifier.Type.Length > 0)
					{
						tempList.Add("<i>" + Convert_String_To_XML_Safe(thisIdentifier.Type.ToLower()) + "</i> - " + Convert_String_To_XML_Safe(thisIdentifier.Identifier));
					}
					else
					{
						tempList.Add(Convert_String_To_XML_Safe(thisIdentifier.Identifier));
					}
				}
				Add_Citation_HTML_Rows("Resource Identifier", tempList, indent, result);
			}

			// Add the CLASSIFICATIONS
			tempList.Clear();
			if (CurrentItem.Bib_Info.Classifications_Count > 0)
			{
				foreach (Classification_Info thisClassification in CurrentItem.Bib_Info.Classifications)
				{
					if (thisClassification.Authority.Length > 0)
					{
						tempList.Add("<i>" + Convert_String_To_XML_Safe(thisClassification.Authority.ToLower()) + "</i> - " + Convert_String_To_XML_Safe(thisClassification.Classification));
					}
					else
					{
						tempList.Add(Convert_String_To_XML_Safe(thisClassification.Classification));
					}
				}
				Add_Citation_HTML_Rows("Classification", tempList, indent, result);
			}

			// Add the system id

		    result.Append(CurrentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL
		                      ? Single_Citation_HTML_Row("System ID", CurrentItem.BibID + ":" + CurrentItem.VID, indent)
		                      : Single_Citation_HTML_Row("System ID", CurrentItem.BibID, indent));

            result.AppendLine(indent + "</table>");

			// Add the RELATED ITEMS
			if (CurrentItem.Bib_Info.RelatedItems_Count > 0)
			{
                result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection2\">");
                result.AppendLine(indent + "<tr>");
                result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle2\"><b>&nbsp;" + related_items + "</b></td>");
                result.AppendLine(indent + "</tr>");

				foreach (Related_Item_Info relatedItem in CurrentItem.Bib_Info.RelatedItems)
				{

					string label = related_items;
					switch (relatedItem.Relationship)
					{
						case Related_Item_Type_Enum.host:
							label = "Host material";
							break;

						case Related_Item_Type_Enum.otherFormat:
							label = "Other format";
							break;

						case Related_Item_Type_Enum.otherVersion:
							label = "Other version";
							break;

						case Related_Item_Type_Enum.preceding:
							label = "Preceded by";
							break;

						case Related_Item_Type_Enum.succeeding:
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
							result.Append(Single_Citation_HTML_Row(label, title + "<br />" + url, indent));
						}
						else
						{
						    result.Append(title.Length > 0 ? Single_Citation_HTML_Row(label, title, indent) : Single_Citation_HTML_Row(label, url, indent));
						}
					}
				}

				// Finish out the table
				result.Append(indent + "\t\t<tr height=\"5\"> <td> </td> </tr>\n");
                result.AppendLine(indent + "</table>");
			}

			if (CurrentMode.Internal_User)
			{
				List<string> codeList = new List<string>();

                result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection2\">");
                result.AppendLine(indent + "<tr>");
                result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle2\"><b>&nbsp;" + internal_info + "</b></td>");
                result.AppendLine(indent + "</tr>");

				if (Code_Manager != null)
				{
				    codeList.AddRange(CurrentMode.Internal_User
				                          ? CurrentItem.Behaviors.Aggregations.Select(aggregation => aggregation.Code).Select(altCode =>"<a href=\"" + CurrentMode.Base_URL + altCode.ToLower() + url_options + "\">" +Convert_String_To_XML_Safe(Code_Manager.Get_Collection_Short_Name(altCode)) + "</a> ( " +altCode + " )")
				                          : CurrentItem.Behaviors.Aggregations.Select(aggregation => aggregation.Code).Select(altCode =>"<a href=\"" + CurrentMode.Base_URL + altCode.ToLower() + url_options + "\">" +Convert_String_To_XML_Safe(Code_Manager.Get_Collection_Short_Name(altCode)) + "</a>"));
				    Add_Citation_HTML_Rows("Aggregations", codeList, indent, result);
					codeList.Clear();
				}

				if (CurrentMode.Internal_User)
				{
					if (CurrentItem.Behaviors.Ticklers_Count > 0)
					{
						Add_Citation_HTML_Rows("Ticklers", CurrentItem.Behaviors.Ticklers, indent, result);
					}
				}

				//                result.Append(Single_Citation_HTML_Row("Interfaces", currentItem.METS.String_RecordStatus, indent));

				result.AppendLine(indent + "</table>");
			}

			if (CurrentMode.Internal_User)
			{

				result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection1\">");
			    result.AppendLine(indent + "<tr>");
			    result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + mets_info + "</b></td>");
			    result.AppendLine(indent + "</tr>");
				result.Append(Single_Citation_HTML_Row("Format", CurrentItem.Bib_Info.SobekCM_Type_String, indent));
				result.Append(Single_Citation_HTML_Row("Creation Date", CurrentItem.METS_Header.Create_Date.ToShortDateString(), indent));
				result.Append(Single_Citation_HTML_Row("Last Modified", CurrentItem.METS_Header.Modify_Date.ToShortDateString(), indent));
				result.Append(Single_Citation_HTML_Row("Last Type", CurrentItem.METS_Header.RecordStatus, indent));
				result.Append(Single_Citation_HTML_Row("Last User", CurrentItem.METS_Header.Creator_Individual, indent));
				result.Append(Single_Citation_HTML_Row("System Folder", CurrentItem.Web.AssocFilePath.Replace("/", "\\"), indent));

				result.AppendLine(indent + "</table>");
			}

			if ((currentUser != null) && (currentUser.Is_System_Admin))
			{
                result.AppendLine(indent + "<table width=\"650px\" cellpadding=\"5px\" class=\"SobekCitationSection2\">");
                result.AppendLine(indent + "<tr>");
                result.AppendLine(indent + "<td colspan=\"3\" class=\"SobekCitationSectionTitle2\"><b>&nbsp;" + system_info + "</b></td>");
                result.AppendLine(indent + "</tr>");

                result.Append(Single_Citation_HTML_Row("Item Primary Key", CurrentItem.Web.ItemID.ToString(), indent));
                result.Append(Single_Citation_HTML_Row("Group Primary Key", CurrentItem.Web.GroupID.ToString(), indent));
                result.AppendLine(indent + "</table>");

			}

			result.AppendLine("<br />");

			// Return the built string
			return result.ToString();
		}



		private void Add_Citation_HTML_Rows(string Row_Name, ReadOnlyCollection<string> Values, string indent, StringBuilder results)
		{
			// Only add if there is a value
		    if (Values.Count <= 0) return;

		    results.AppendLine(indent + "    <tr>");
		    results.AppendLine(indent + "      <td width=\"15\"> </td>");
		    results.Append(indent + "      <td width=\"" + width + "\" valign=\"top\"><b>");

		    // Add with proper language
		    switch (CurrentMode.Language)
		    {
		        case Web_Language_Enum.French:
		            results.Append(Translator.Get_French(Row_Name));
		            break;

		        case Web_Language_Enum.Spanish:
		            results.Append(Translator.Get_Spanish(Row_Name));
		            break;

		        default:
		            results.Append(Row_Name);
		            break;
		    }

		    results.AppendLine(": </b></td>");
		    results.Append( indent + "      <td>");
		    bool first = true;
		    foreach (string thisValue in Values.Where(thisValue => thisValue.Length > 0))
		    {
		        if (first)
		        {
		            results.Append(thisValue);
		            first = false;
		        }
		        else
		        {
		            results.Append("<br />" + thisValue);
		        }
		    }
		    results.AppendLine("</td>");
		    results.AppendLine( indent + "    </tr>");
		}

	

		private void Add_Citation_HTML_Rows(string Row_Name, List<string> Values, string indent, StringBuilder results )
		{
			// Only add if there is a value
		    if (Values.Count <= 0) return;

		    results.AppendLine(indent + "    <tr>");
		    results.AppendLine(indent + "      <td width=\"15\"> </td>");
		    results.Append(indent + "      <td width=\"" + width + "\" valign=\"top\"><b>");

		    // Add with proper language
		    switch (CurrentMode.Language)
		    {
		        case Web_Language_Enum.French:
		            results.Append(Translator.Get_French(Row_Name));
		            break;

		        case Web_Language_Enum.Spanish:
		            results.Append(Translator.Get_Spanish(Row_Name));
		            break;

		        default:
		            results.Append(Row_Name);
		            break;
		    }

		    results.AppendLine(": </b></td>");
		    results.Append( indent + "      <td>");
		    bool first = true;
		    foreach (string thisValue in Values.Where(thisValue => thisValue.Length > 0))
		    {
		        if (first)
		        {
		            results.Append(thisValue);
		            first = false;
		        }
		        else
		        {
		            results.Append("<br />" + thisValue);
		        }
		    }
		    results.AppendLine("</td>");
		    results.AppendLine( indent + "    </tr>");
		}

		private string Single_Citation_HTML_Row(string Row_Name, string Value, string indent)
		{
		    // Only add if there is a value
			if (Value.Length > 0)
			{
				switch (CurrentMode.Language)
				{
					case Web_Language_Enum.French:
                        return indent + "    <tr>" + Environment.NewLine + indent + "      <td width=\"15\"> </td>" + Environment.NewLine + indent + "      <td width=\"" + width + "\" valign=\"top\"><b>" + Translator.Get_French(Row_Name) + ": </b></td>" + Environment.NewLine + indent + "      <td>" + Value + "</td>" + Environment.NewLine + indent + "    </tr>" + Environment.NewLine;

					case Web_Language_Enum.Spanish:
                        return indent + "    <tr>" + Environment.NewLine + indent + "      <td width=\"15\"> </td>" + Environment.NewLine + indent + "      <td width=\"" + width + "\" valign=\"top\"><b>" + Translator.Get_Spanish(Row_Name) + ": </b></td>" + Environment.NewLine + indent + "      <td>" + Value + "</td>" + Environment.NewLine + indent + "    </tr>" + Environment.NewLine;

					default:
                        return indent + "    <tr>" + Environment.NewLine + indent + "      <td width=\"15\"> </td>" + Environment.NewLine + indent + "      <td width=\"" + width + "\" valign=\"top\"><b>" + Row_Name + ": </b></td>" + Environment.NewLine + indent + "      <td>" + Value + "</td>" + Environment.NewLine + indent + "   </tr>" + Environment.NewLine;
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
