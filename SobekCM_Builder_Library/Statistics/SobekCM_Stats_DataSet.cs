#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Navigation;

#endregion

namespace SobekCM.Builder_Library.Statistics
{
    /// <summary> All of the information about a range of hits and sessions against this SobekCM system </summary>
    /// <remarks> This class is used by the SobekCM Stats Reader app </remarks>
    public class SobekCM_Stats_DataSet
    {
        private Dictionary<string, DataRow> bib_rows;
        private Dictionary<string, List<int>> bib_sessions;
        private DataTable bib_stats;
        private Dictionary<string, DataRow> collection_rows;

        // Session dictionary objects is used to determine if the session is already
        // linked to the collection, bib, or item
        private Dictionary<string, List<int>> collection_sessions;
        private DataTable collection_stats;
        private DateTime date;
        private int hit_ip_cutoff;
        private Dictionary<string, DataRow> institution_rows;
        private Dictionary<string, List<int>> institution_sessions;
        private DataTable institution_stats;
        private DataTable ip_addresses;
        private Dictionary<string, int> ip_hits;
        private Dictionary<string, DataRow> ip_rows;
        private Dictionary<string, string> ip_to_agent;

        // Hashtables are used for quickly getting the correct row from the table, without
        // having to do SELECT statements on the rows
        private Dictionary<int, DataRow> item_rows;
        private Dictionary<string, List<int>> item_sessions;
        private DataTable item_stats;
        private Dictionary<string, DataRow> portal_rows;
        private DataTable portal_stats;
        private DataTable sobekcm_stats;
        private DataSet stats;
        private Dictionary<string, DataRow> webcontent_rows;
        private DataTable webcontent_stats;

        #region Constructor

        /// <summary> Constructor for a new instance of the SobekCM Statistics dataset </summary>
        public SobekCM_Stats_DataSet()
        {
            // Create the dictionaries for quick lookups;
            collection_rows = new Dictionary<string, DataRow>();
            institution_rows = new Dictionary<string, DataRow>();
            bib_rows = new Dictionary<string, DataRow>();
            item_rows = new Dictionary<int, DataRow>();
            ip_rows = new Dictionary<string, DataRow>();
            portal_rows = new Dictionary<string, DataRow>();
            webcontent_rows = new Dictionary<string, DataRow>();

            // Create the new session dictionary objects
            collection_sessions = new Dictionary<string, List<int>>();
            institution_sessions = new Dictionary<string, List<int>>();
            bib_sessions = new Dictionary<string, List<int>>();
            item_sessions = new Dictionary<string, List<int>>();
            ip_hits = new Dictionary<string, int>();
            ip_to_agent = new Dictionary<string, string>();

            // Create the dataset
            stats = new DataSet {DataSetName = "SobekCM_Stats"};

            // Create the table to hold the collection hits
            sobekcm_stats = new DataTable("SobekCM_Hits");
            sobekcm_stats.Columns.Add("sessions", typeof(Int32));
            sobekcm_stats.Columns.Add("hits", typeof(Int32));
            sobekcm_stats.Columns.Add("xml", typeof(Int32));
            sobekcm_stats.Columns.Add("oai", typeof(Int32));
            sobekcm_stats.Columns.Add("json", typeof(Int32));
            sobekcm_stats.Columns.Add("robot_hits", typeof(Int32));
            stats.Tables.Add(sobekcm_stats);

            // Add an empty row here
            DataRow newRow = sobekcm_stats.NewRow();
            newRow[0] = 0;
            newRow[1] = 0;
            newRow[2] = 0;
            newRow[3] = 0;
            newRow[4] = 0;
            newRow[5] = 0;
            sobekcm_stats.Rows.Add(newRow);

            // Create the table to hold the collection hits
            collection_stats = new DataTable("Collection_Hits");
            collection_stats.Columns.Add("code");
            collection_stats.Columns.Add("sessions", typeof(Int32));
            collection_stats.Columns.Add("home_page_hits", typeof(Int32));
            collection_stats.Columns.Add("browse_hits", typeof(Int32));
            collection_stats.Columns.Add("advanced_search_hits", typeof(Int32));
            collection_stats.Columns.Add("results_hits", typeof(Int32));
            stats.Tables.Add(collection_stats);

            // Create the table to hold the institutional hits
            institution_stats = new DataTable("Institution_Hits");
            institution_stats.Columns.Add("code");
            institution_stats.Columns.Add("sessions", typeof(Int32));
            institution_stats.Columns.Add("home_page_hits", typeof(Int32));
            institution_stats.Columns.Add("browse_hits", typeof(Int32));
            institution_stats.Columns.Add("advanced_search_hits", typeof(Int32));
            institution_stats.Columns.Add("results_hits", typeof(Int32));
            stats.Tables.Add(institution_stats);

            // Create the table to hold the bib-level hits
            bib_stats = new DataTable("Bib_Hits");
            bib_stats.Columns.Add("bibid");
            bib_stats.Columns.Add("sessions", typeof(Int32));
            bib_stats.Columns.Add("hits", typeof(Int32));
            stats.Tables.Add(bib_stats);

            // Create the table to hold the item hits
            item_stats = new DataTable("Item_Hits");
            item_stats.Columns.Add("bibid");
            item_stats.Columns.Add("vid");
            item_stats.Columns.Add("itemid", typeof(Int32));
            item_stats.Columns.Add("sessions", typeof(Int32));
            item_stats.Columns.Add("default_hits", typeof(Int32));
            item_stats.Columns.Add("jpeg_hits", typeof(Int32));
            item_stats.Columns.Add("zoomable_hits", typeof(Int32));
            item_stats.Columns.Add("citation_hits", typeof(Int32));
            item_stats.Columns.Add("thumbnail_hits", typeof(Int32));
            item_stats.Columns.Add("text_search_hits", typeof(Int32));
            item_stats.Columns.Add("flash_hits", typeof(Int32));
            item_stats.Columns.Add("google_map_hits", typeof(Int32));
            item_stats.Columns.Add("download_hits", typeof(Int32));
            item_stats.Columns.Add("other_hits", typeof(Int32));
            item_stats.Columns.Add("static_hits", typeof(Int32));
            stats.Tables.Add(item_stats);

            // Create the table to hold most common IP addresses
            ip_addresses = new DataTable("IP_Addresses");
            ip_addresses.Columns.Add("IP");
            ip_addresses.Columns.Add("UserAgent");
            ip_addresses.Columns.Add("hits", typeof(Int32));
            stats.Tables.Add(ip_addresses);

            // Create the table to hold the portal hits
            portal_stats = new DataTable("Portal_Hits");
            portal_stats.Columns.Add("Base_URL");
            portal_stats.Columns.Add("hits", typeof(Int32));
            stats.Tables.Add(portal_stats);

            //  Create the table to hold the static webcontent hits
            webcontent_stats = new DataTable("WebContent_Hits");
            webcontent_stats.Columns.Add("Complete_Mode");
            webcontent_stats.Columns.Add("hits", typeof(Int32));
            webcontent_stats.Columns.Add("Level1");
            webcontent_stats.Columns.Add("Level2");
            webcontent_stats.Columns.Add("Level3");
            webcontent_stats.Columns.Add("Level4");
            webcontent_stats.Columns.Add("Level5");
            webcontent_stats.Columns.Add("Level6");
            webcontent_stats.Columns.Add("Level7");
            webcontent_stats.Columns.Add("Level8");
            stats.Tables.Add(webcontent_stats);

            // Set the default cut off
            hit_ip_cutoff = 50;
        }

        #endregion

        /// <summary> When writing the list of the most commonly session IPs and user agent
        /// information (for analysis on new search engine robots) how many should be written in the
        /// special _users .xml file?  </summary>
        public int Hit_IP_Cutoff
        {
            set { hit_ip_cutoff = value; }
        }

        #region Basic properties

        internal DataTable SobekCM_Stats_Table
        {
            get { return sobekcm_stats; }
        }

        internal DataTable Collection_Stats_Table
        {
            get { return collection_stats; }
        }

        internal DataTable Institution_Stats_Table
        {
            get { return institution_stats; }
        }

        internal DataTable Bib_Stats_Table
        {
            get { return bib_stats; }
        }

        internal DataTable Item_Stats_Table
        {
            get { return item_stats; }
        }

        internal DataTable IP_Addresses
        {
            get { return ip_addresses; }
        }

        internal DataTable Portal_Stats_Table
        {
            get { return portal_stats; }
        }

        internal DataTable WebContent_Stats_Table
        {
            get { return webcontent_stats; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        #endregion

        /// <summary> Add the fact that a single hit occurred against the XML writers </summary>
        public void Add_XML_Hit()
        {
            sobekcm_stats.Rows[0][2] = Convert.ToInt32(sobekcm_stats.Rows[0][2]) + 1;
        }

        /// <summary> Add the fact that a single hit occurred against the OAI-PMH writers </summary>
        public void Add_OAI_Hit()
        {
            sobekcm_stats.Rows[0][3] = Convert.ToInt32(sobekcm_stats.Rows[0][3]) + 1;
        }

        /// <summary> Add the fact that a single hit occurred against the JSON writers </summary>
        public void Add_JSON_Hit()
        {
            sobekcm_stats.Rows[0][4] = Convert.ToInt32(sobekcm_stats.Rows[0][4]) + 1;
        }

        /// <summary> Add a single robotic hit (otherwise not analyzed) </summary>
        public void Add_Robot_Hit()
        {
            sobekcm_stats.Rows[0][5] = Convert.ToInt32(sobekcm_stats.Rows[0][5]) + 1;
        }

        /// <summary> Increment the number of (human) hits </summary>
        public void Increment_Hits()
        {
            sobekcm_stats.Rows[0][1] = Convert.ToInt32(sobekcm_stats.Rows[0][1]) + 1;
        }

        /// <summary> Increment the number of (human) sessions or visits </summary>
        public void Increment_Sessions()
        {
            sobekcm_stats.Rows[0][0] = Convert.ToInt32(sobekcm_stats.Rows[0][0]) + 1;
        }

        /// <summary> Add a single mapping from IP address to the user agent, or browser/system information </summary>
        /// <param name="IP"> IP address from which this request came </param>
        /// <param name="UserAgent"> User agent from the HTTP requrest ( browser/system information ) </param>
        /// <remarks> This is used for determining if there are new search engine robots out there sneaking
        /// through my robot detection algorithm </remarks>
        public void Add_IP_Hit(string IP, string UserAgent)
        {
            if (ip_hits.ContainsKey(IP))
                ip_hits[IP] = ip_hits[IP] + 1;
            else
                ip_hits[IP] = 1;

            if (!ip_to_agent.ContainsKey(IP))
                ip_to_agent[IP] = UserAgent;
        }

        /// <summary> Add a single hit against a portal </summary>
        /// <param name="Portal_Name"> Name of the portal </param>
        public void Add_Portal_Hit(string Portal_Name)
        {
            // Create or fetch the data row for this collection
            DataRow newRow;
            if (portal_rows.ContainsKey(Portal_Name.ToUpper()))
            {
                newRow = portal_rows[Portal_Name.ToUpper()];
                newRow[1] = Convert.ToInt32(newRow[1]) + 1;
            }
            else
            {
                newRow = portal_stats.NewRow();
                newRow[0] = Portal_Name.ToUpper();
                newRow[1] = 1;
                portal_stats.Rows.Add(newRow);
                portal_rows.Add(Portal_Name.ToUpper(), newRow);
            }
        }

        /// <summary> Add a single hit against static web content </summary>
        /// <param name="Browse_Mode"> Name of the web content </param>
        public void Add_WebContent_Hit(string Browse_Mode)
        {
            // Create or fetch the data row for this collection
            DataRow newRow;
            if (webcontent_rows.ContainsKey(Browse_Mode))
            {
                newRow = webcontent_rows[Browse_Mode];
                newRow[1] = Convert.ToInt32(newRow[1]) + 1;
            }
            else
            {
                newRow = webcontent_stats.NewRow();
                newRow[0] = Browse_Mode;
                newRow[1] = 1;
                webcontent_stats.Rows.Add(newRow);
                webcontent_rows.Add(Browse_Mode, newRow);
            }
        }

	    /// <summary> Add a single hit against an aggregation (or collection) </summary>
	    /// <param name="Code"> Aggregation Code </param>
	    /// <param name="Mode"> Mode information </param>
	    /// <param name="AggrType"> Aggregation type, if this is Aggregation mode </param>
	    /// <param name="SessionID"> ID for the session from which this hit originated </param>
	    public void Add_Collection_Hit(string Code, Display_Mode_Enum Mode, Aggregation_Type_Enum AggrType, int SessionID)
        {
            // Determine if this session is already linked to this collection
            int increment_session = 0;
            if (collection_sessions.ContainsKey(Code))
            {
                // This code has been found before, but was this session among them previous sessions?
                if (!collection_sessions[Code].Contains(SessionID))
                {
                    // New session!
                    increment_session = 1;
                    collection_sessions[Code].Add(SessionID);
                }
            }
            else
            {
                // This collection code has not been hit yet at all
                increment_session = 1;
                List<int> new_session_id_list = new List<int> {SessionID};
                collection_sessions.Add(Code, new_session_id_list);
            }

            // Create or fetch the data row for this collection
            DataRow newRow;
            if (collection_rows.ContainsKey(Code))
            {
                newRow = collection_rows[Code];
            }
            else
            {
                newRow = collection_stats.NewRow();
                newRow["code"] = Code;
                newRow["sessions"] = 0;
                newRow["home_page_hits"] = 0;
                newRow["browse_hits"] = 0;
                newRow["advanced_search_hits"] = 0;
                newRow["results_hits"] = 0;
                collection_stats.Rows.Add(newRow);
                collection_rows.Add(Code, newRow);
            }

            // If this is a new session, increment that counter
            if (increment_session > 0)
            {
                newRow["sessions"] = Convert.ToInt32(newRow["sessions"]) + 1;
            }

            // If there is no mode, this is home page
			switch (Mode)
			{
				case Display_Mode_Enum.Aggregation:
					switch (AggrType)
					{
						case Aggregation_Type_Enum.Browse_By:
						case Aggregation_Type_Enum.Browse_Info: 
						case Aggregation_Type_Enum.Browse_Map:
							newRow["browse_hits"] = Convert.ToInt32(newRow["browse_hits"]) + 1;
							break;

						default:
							// Just call this home page then.
							newRow["home_page_hits"] = Convert.ToInt32(newRow["home_page_hits"]) + 1;
							break;
					}
					break;


				case Display_Mode_Enum.Search:
					newRow["advanced_search_hits"] = Convert.ToInt32(newRow["advanced_search_hits"]) + 1;
					break;

				case Display_Mode_Enum.Results:
					newRow["results_hits"] = Convert.ToInt32(newRow["results_hits"]) + 1;
					break;

				default:
					// Just call this home page then.
					newRow["home_page_hits"] = Convert.ToInt32(newRow["home_page_hits"]) + 1;
					break;
			}
        }

	    /// <summary> Add a single hit against an institutional aggregation </summary>
	    /// <param name="Code"> Institutional aggregation code </param>
	    /// <param name="Mode"> Mode information </param>
	    /// <param name="AggrType"> Aggregation type, if this is aggregation mode </param>
	    /// <param name="SessionID"> ID for the session from which this hit originated </param>
	    public void Add_Institution_Hit(string Code, Display_Mode_Enum Mode, Aggregation_Type_Enum AggrType, int SessionID)
        {
            // Determine if this session is already linked to this institution
            int increment_session = 0;
            if (institution_sessions.ContainsKey(Code))
            {
                // This code has been found before, but was this session among them previous sessions?
                if (!institution_sessions[Code].Contains(SessionID))
                {
                    // New session!
                    increment_session = 1;
                    institution_sessions[Code].Add(SessionID);
                }
            }
            else
            {
                // This institution has not been hit yet at all
                increment_session = 1;
                List<int> new_institution_id_list = new List<int> {SessionID};
                institution_sessions.Add(Code, new_institution_id_list);
            }


            // Create or fetch the data row for this collection
            DataRow newRow;
            if (institution_rows.ContainsKey(Code))
            {
                newRow = institution_rows[Code];
            }
            else
            {
                newRow = institution_stats.NewRow();
                newRow["code"] = Code;
                newRow["sessions"] = 0;
                newRow["home_page_hits"] = 0;
                newRow["browse_hits"] = 0;
                newRow["advanced_search_hits"] = 0;
                newRow["results_hits"] = 0;
                institution_stats.Rows.Add(newRow);
                institution_rows.Add(Code, newRow);
            }

            // If this is a new session, increment that counter
            if (increment_session > 0)
            {
                newRow["sessions"] = Convert.ToInt32(newRow["sessions"]) + 1;
            }

            // If there is no mode, this is home page
			switch (Mode)
			{
				case Display_Mode_Enum.Aggregation:
					switch (AggrType)
					{
						case Aggregation_Type_Enum.Browse_By: 
						case Aggregation_Type_Enum.Browse_Info: 
						case Aggregation_Type_Enum.Browse_Map:
							newRow["browse_hits"] = Convert.ToInt32(newRow["browse_hits"]) + 1;
							break;

						default:
							// Just call this home page then.
							newRow["home_page_hits"] = Convert.ToInt32(newRow["home_page_hits"]) + 1;
							break;
					}
					break;

				case Display_Mode_Enum.Search:
					newRow["advanced_search_hits"] = Convert.ToInt32(newRow["advanced_search_hits"]) + 1;
					break;

				case Display_Mode_Enum.Results:
					newRow["results_hits"] = Convert.ToInt32(newRow["results_hits"]) + 1;
					break;

				default:
					// Just call this home page then.
					newRow["home_page_hits"] = Convert.ToInt32(newRow["home_page_hits"]) + 1;
					break;
			}
        }

        /// <summary> Add a single hit against a title (or BibID) </summary>
        /// <param name="BibID"> Bibliographic identifier for the title hit </param>
        /// <param name="SessionID"> ID for the session from which this hit originated </param>
        public void Add_Bib_Hit(string BibID, int SessionID)
        {
            // Determine if this session is already linked to this bib id
            int increment_session = 0;
            if (bib_sessions.ContainsKey(BibID))
            {
                // This code has been found before, but was this session among them previous sessions?
                if (!bib_sessions[BibID].Contains(SessionID))
                {
                    // New session!
                    increment_session = 1;
                    bib_sessions[BibID].Add(SessionID);
                }
            }
            else
            {
                // This bibid has not been hit yet at all
                increment_session = 1;
                List<int> new_bib_id_list = new List<int> {SessionID};
                bib_sessions.Add(BibID, new_bib_id_list);
            }

            // Fetch or create the bibid data row
            DataRow newRow;
            if (bib_rows.ContainsKey(BibID))
            {
                newRow = bib_rows[BibID];
            }
            else
            {
                newRow = bib_stats.NewRow();
                newRow["bibid"] = BibID;
                newRow["sessions"] = 0;
                newRow["hits"] = 0;
                bib_stats.Rows.Add(newRow);
                bib_rows.Add(BibID, newRow);
            }

            if (increment_session > 0)
            {
                newRow["sessions"] = Convert.ToInt32(newRow["sessions"]) + 1;
            }

            newRow["hits"] = Convert.ToInt32(newRow["hits"]) + 1;
        }

        /// <summary> Adds a single hit against an item within this library </summary>
        /// <param name="Item_ID"></param>
        /// <param name="BibID"></param>
        /// <param name="VID"></param>
        /// <param name="ViewerCode"></param>
        /// <param name="Text_Search"></param>
        /// <param name="SessionID"> ID for the session from which this hit originated </param>
        public void Add_Item_Hit(int Item_ID, string BibID, string VID, string ViewerCode, string Text_Search,
                                 int SessionID)
        {
            // Determine if this session is already linked to this item
            int increment_session = 0;
            if (item_sessions.ContainsKey(BibID + VID))
            {
                // This code has been found before, but was this session among them previous sessions?
                if (!item_sessions[BibID + VID].Contains(SessionID))
                {
                    // New session!
                    increment_session = 1;
                    item_sessions[BibID + VID].Add(SessionID);
                }
            }
            else
            {
                // This itemd has not been hit yet at all
                increment_session = 1;
                List<int> new_bib_id_list = new List<int> {SessionID};
                item_sessions.Add(BibID + VID, new_bib_id_list);
            }

            // Fetch or create the bibid data row
            DataRow newRow;
            if (item_rows.ContainsKey(Item_ID))
            {
                newRow = item_rows[Item_ID];
            }
            else
            {
                newRow = item_stats.NewRow();
                newRow["itemid"] = Item_ID;
                newRow["bibid"] = BibID;
                newRow["vid"] = VID;
                newRow["sessions"] = 0;
                newRow["default_hits"] = 0;
                newRow["jpeg_hits"] = 0;
                newRow["zoomable_hits"] = 0;
                newRow["citation_hits"] = 0;
                newRow["thumbnail_hits"] = 0;
                newRow["text_search_hits"] = 0;
                newRow["flash_hits"] = 0;
                newRow["google_map_hits"] = 0;
                newRow["download_hits"] = 0;
                newRow["other_hits"] = 0;
                newRow["static_hits"] = 0;
                item_stats.Rows.Add(newRow);
                item_rows.Add(Item_ID, newRow);
            }

            // Increment session if this is a new session
            if (increment_session > 0)
            {
                newRow["sessions"] = Convert.ToInt32(newRow["sessions"]) + 1;
            }

            if (Text_Search.Length > 0)
            {
                newRow["text_search_hits"] = Convert.ToInt32(newRow["text_search_hits"]) + 1;
                return;
            }

            if ((ViewerCode.IndexOf("GM") >= 0) || (ViewerCode.IndexOf("MAP") >= 0))
            {
                newRow["google_map_hits"] = Convert.ToInt32(newRow["google_map_hits"]) + 1;
                return;
            }

            if ((ViewerCode.IndexOf("DO") > 0) || (ViewerCode.IndexOf("DOWNLOAD") >= 0))
            {
                newRow["download_hits"] = Convert.ToInt32(newRow["download_hits"]) + 1;
                return;
            }

            if ((ViewerCode.IndexOf("FC") >= 0) || (ViewerCode.IndexOf("CITATION") >= 0) ||
                (ViewerCode.IndexOf("MARC") >= 0) || (ViewerCode.IndexOf("TRACKING") >= 0) ||
                (ViewerCode.IndexOf("METADATA") >= 0) || (ViewerCode.IndexOf("USAGE") >= 0))
            {
                newRow["citation_hits"] = Convert.ToInt32(newRow["citation_hits"]) + 1;
                return;
            }

            if ((ViewerCode.IndexOf("RI") >= 0) || (ViewerCode.IndexOf("THUMB") >= 0))
            {
                newRow["thumbnail_hits"] = Convert.ToInt32(newRow["thumbnail_hits"]) + 1;
                return;
            }

            if ((ViewerCode.IndexOf("FL") >= 0) || (ViewerCode.IndexOf("FLASH") >= 0))
            {
                newRow["flash_hits"] = Convert.ToInt32(newRow["flash_hits"]) + 1;
                return;
            }

            if (ViewerCode.IndexOf("J") >= 0)
            {
                newRow["jpeg_hits"] = Convert.ToInt32(newRow["jpeg_hits"]) + 1;
                return;
            }

            if (ViewerCode.IndexOf("X") >= 0)
            {
                newRow["zoomable_hits"] = Convert.ToInt32(newRow["zoomable_hits"]) + 1;
                return;
            }

            newRow["default_hits"] = Convert.ToInt32(newRow["default_hits"]) + 1;
        }

        /// <summary> Read an existing XML output from this class into this object  </summary>
        /// <param name="file"></param>
        public void Read_XML(string file)
        {
            stats = new DataSet();
            stats.ReadXml(file);

            // Copy the tables over
            sobekcm_stats = stats.Tables[0];
            collection_stats = stats.Tables[1];
            institution_stats = stats.Tables[2];
            bib_stats = stats.Tables[3];
            item_stats = stats.Tables[4];
            ip_addresses = stats.Tables[5];
            if (stats.Tables.Count > 6)
            {
                portal_stats = stats.Tables[6];
                webcontent_stats = stats.Tables[7];
            }

            // Create the dictionary lists
            foreach (DataRow thisRow in collection_stats.Rows)
            {
                collection_rows.Add(thisRow[0].ToString(), thisRow);
            }
            foreach (DataRow thisRow in institution_stats.Rows)
            {
                institution_rows.Add(thisRow[0].ToString(), thisRow);
            }
            foreach (DataRow thisRow in bib_stats.Rows)
            {
                bib_rows.Add(thisRow[0].ToString(), thisRow);
            }
            foreach (DataRow thisRow in item_stats.Rows)
            {
                item_rows.Add(Convert.ToInt32(thisRow["itemid"]), thisRow);
            }
            foreach (DataRow thisRow in ip_addresses.Rows)
            {
                ip_rows.Add(thisRow[0].ToString(), thisRow);
            }
            if (portal_stats != null)
            {
                foreach (DataRow thisRow in portal_stats.Rows)
                {
                    string portal = thisRow[0].ToString().ToUpper();
                    if (portal_rows.ContainsKey(portal))
                    {
                        DataRow existingRow = portal_rows[portal];
                        existingRow[1] = Convert.ToInt32(existingRow[1]) + Convert.ToInt32(thisRow[1]);
                    }
                    else
                    {
                        portal_rows.Add(thisRow[0].ToString().ToUpper(), thisRow);
                    }
                }
            }
            if (webcontent_stats != null)
            {
                foreach (DataRow thisRow in webcontent_stats.Rows)
                {
                    webcontent_rows.Add(thisRow[0].ToString(), thisRow);
                }
            }

            // Try to get the data as well
            string filename = (new FileInfo(file)).Name.ToUpper().Replace(".XML", "");
            if (filename.Length == 8)
            {
                int year = Convert.ToInt32(filename.Substring(0, 4));
                int month = Convert.ToInt32(filename.Substring(4, 2));
                int day = Convert.ToInt32(filename.Substring(6, 2));

                date = new DateTime(year, month, day);
            }
        }

        /// <summary> Write the list of all SQL insert command to add this new usage statistical
        /// information to the SobekCM database  </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="aggregationHash"></param>
        /// <param name="bibHash"></param>
        /// <param name="portalHash"></param>
        public void Perform_SQL_Inserts(int year, int month, Dictionary<string, int> aggregationHash,
                                      Dictionary<string, int> bibHash, Dictionary<string, int> portalHash)
        {
            // Add the overall statistics
            if (sobekcm_stats.Rows.Count > 0)
            {
                Engine_Database.Save_TopLevel_Statistics(year, month, Convert.ToInt32(sobekcm_stats.Rows[0]["hits"]),
                    Convert.ToInt32(sobekcm_stats.Rows[0]["sessions"]), Convert.ToInt32(sobekcm_stats.Rows[0]["Robot_Hits"]),
                    Convert.ToInt32(sobekcm_stats.Rows[0]["XML"]), Convert.ToInt32(sobekcm_stats.Rows[0]["OAI"]),
                    Convert.ToInt32(sobekcm_stats.Rows[0]["JSON"]), null);
            }

            // Add the web content statistics
            if ((webcontent_stats != null) && (webcontent_stats.Rows.Count > 0))
            {
                foreach (DataRow thisRow in webcontent_stats.Rows)
                {
                    // Calculate the complete hits
                    StringBuilder sql_builder = new StringBuilder("Level1=\"" + thisRow[2] + "\"");
                    if (thisRow[3].ToString().Length > 0)
                    {
                        sql_builder.Append(" and Level2=\"" + thisRow[3] + "\"");
                        if (thisRow[4].ToString().Length > 0)
                        {
                            sql_builder.Append(" and Level3=\"" + thisRow[4] + "\"");
                            if (thisRow[5].ToString().Length > 0)
                            {
                                sql_builder.Append(" and Level4=\"" + thisRow[5] + "\"");
                                if (thisRow[6].ToString().Length > 0)
                                {
                                    sql_builder.Append(" and Level5=\"" + thisRow[6] + "\"");
                                    if (thisRow[7].ToString().Length > 0)
                                    {
                                        sql_builder.Append(" and Level6=\"" + thisRow[7] + "\"");
                                        if (thisRow[8].ToString().Length > 0)
                                        {
                                            sql_builder.Append(" and Level7=\"" + thisRow[8] + "\"");
                                            if (thisRow[9].ToString().Length > 0)
                                            {
                                                sql_builder.Append(" and Level8=\"" + thisRow[9] + "\"");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    DataRow[] matches = webcontent_stats.Select(sql_builder.ToString().Replace("\"", "'"));
                    int hits_complete = matches.Sum(childRow => Convert.ToInt32(childRow[1]));

                    Engine_Database.Save_WebContent_Statistics(year, month, Convert.ToInt32(thisRow[1]), hits_complete, thisRow[2].ToString(),
                        thisRow[3].ToString(), thisRow[4].ToString(), thisRow[5].ToString(), thisRow[6].ToString(), thisRow[7].ToString(), thisRow[8].ToString(),
                        thisRow[9].ToString(), null);
                }
            }

            // Add the portal hits
            if ((portal_stats != null) && (portal_stats.Rows.Count > 0))
            {
                foreach (DataRow portalRow in portal_stats.Rows)
                {
                    string code = portalRow[0].ToString();
                    if (portalHash.ContainsKey(code.ToUpper()))
                    {
                        int portalid = portalHash[code.ToUpper()];

                        Engine_Database.Save_Portal_Statistics(portalid, year, month, Convert.ToInt32(portalRow[1]), null);
                    }
                }
            }


            // Add the item aggregation stats (non-institutional)
            SortedList<int, string> sql = new SortedList<int, string>();
            foreach (DataRow hierarchyRow in collection_stats.Rows)
            {
                string code = hierarchyRow["code"].ToString().ToUpper();
                if (aggregationHash.ContainsKey(code))
                {
                    int hits = Convert.ToInt32(hierarchyRow["home_page_hits"]) +
                               Convert.ToInt32(hierarchyRow["browse_hits"]) +
                               Convert.ToInt32(hierarchyRow["advanced_search_hits"]) +
                               Convert.ToInt32(hierarchyRow["results_hits"]);

                    Engine_Database.Save_Aggregation_Statistics(aggregationHash[code], year, month, hits, Convert.ToInt32(hierarchyRow["sessions"]),
                        Convert.ToInt32(hierarchyRow["home_page_hits"]), Convert.ToInt32(hierarchyRow["browse_hits"]), Convert.ToInt32(hierarchyRow["advanced_search_hits"]),
                        Convert.ToInt32(hierarchyRow["results_hits"]), null);
                }
            }


            foreach (DataRow hierarchyRow in institution_stats.Rows)
            {
                string code = hierarchyRow["code"].ToString().ToUpper();
                if (aggregationHash.ContainsKey(code))
                {
                    int hits = Convert.ToInt32(hierarchyRow["home_page_hits"]) +
                               Convert.ToInt32(hierarchyRow["browse_hits"]) +
                               Convert.ToInt32(hierarchyRow["advanced_search_hits"]) +
                               Convert.ToInt32(hierarchyRow["results_hits"]);

                    Engine_Database.Save_Aggregation_Statistics(aggregationHash[code], year, month, hits, Convert.ToInt32(hierarchyRow["sessions"]),
                        Convert.ToInt32(hierarchyRow["home_page_hits"]), Convert.ToInt32(hierarchyRow["browse_hits"]), Convert.ToInt32(hierarchyRow["advanced_search_hits"]),
                        Convert.ToInt32(hierarchyRow["results_hits"]), null);
                }
            }


            foreach (DataRow hierarchyRow in bib_stats.Rows)
            {
                if (bibHash.ContainsKey(hierarchyRow["bibid"].ToString().ToUpper()))
                {
                    Engine_Database.Save_Item_Group_Statistics(bibHash[hierarchyRow["bibid"].ToString()], year, month, Convert.ToInt32(hierarchyRow["hits"]), Convert.ToInt32(hierarchyRow["sessions"]), null);
                }
            }

            foreach (DataRow hierarchyRow in item_stats.Rows)
            {
                int hits = Convert.ToInt32(hierarchyRow["default_hits"]) + Convert.ToInt32(hierarchyRow["jpeg_hits"]) +
                           Convert.ToInt32(hierarchyRow["zoomable_hits"]) +
                           Convert.ToInt32(hierarchyRow["citation_hits"]) +
                           Convert.ToInt32(hierarchyRow["thumbnail_hits"]) +
                           Convert.ToInt32(hierarchyRow["text_search_hits"]) +
                           Convert.ToInt32(hierarchyRow["flash_hits"]) +
                           Convert.ToInt32(hierarchyRow["google_map_hits"]) +
                           Convert.ToInt32(hierarchyRow["download_hits"]) + Convert.ToInt32(hierarchyRow["other_hits"]) +
                           Convert.ToInt32(hierarchyRow["static_hits"]);

                Engine_Database.Save_Item_Statistics(year, month, hits, Convert.ToInt32(hierarchyRow["sessions"]), Convert.ToInt32(hierarchyRow["itemid"]),
                    Convert.ToInt32(hierarchyRow["jpeg_hits"]), Convert.ToInt32(hierarchyRow["zoomable_hits"]), Convert.ToInt32(hierarchyRow["citation_hits"]),
                    Convert.ToInt32(hierarchyRow["thumbnail_hits"]), Convert.ToInt32(hierarchyRow["text_search_hits"]), Convert.ToInt32(hierarchyRow["flash_hits"]),
                    Convert.ToInt32(hierarchyRow["google_map_hits"]), Convert.ToInt32(hierarchyRow["download_hits"]), Convert.ToInt32(hierarchyRow["static_hits"]), null );
            }

            // Add the call to the aggregator function
            Engine_Database.Aggregate_Statistics(year, month, null);

        }

        /// <summary> Write the highest users as a XML file </summary>
        /// <remarks> This clears all other rows, so this must be called last</remarks>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        public void Write_Highest_Users(string directory, string filename)
        {
            sobekcm_stats.Rows.Clear();
            collection_stats.Rows.Clear();
            institution_stats.Rows.Clear();
            bib_stats.Rows.Clear();
            item_stats.Rows.Clear();
            portal_stats.Rows.Clear();
            webcontent_stats.Rows.Clear();

            stats.WriteXml(directory + "\\" + filename, XmlWriteMode.IgnoreSchema);
        }

        /// <summary> Write this data as XML </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        public void Write_XML(string directory, string filename)
        {
            // If there is webcontent data, split it now into each level component
            if (webcontent_stats != null)
            {
                foreach (DataRow thisRow in webcontent_stats.Rows)
                {
                    thisRow[2] = String.Empty;
                    thisRow[3] = String.Empty;
                    thisRow[4] = String.Empty;
                    thisRow[5] = String.Empty;
                    thisRow[6] = String.Empty;
                    thisRow[7] = String.Empty;
                    thisRow[8] = String.Empty;
                    thisRow[9] = String.Empty;
                    string[] address = thisRow[0].ToString().Split("/".ToCharArray());
                    if (address.Length > 0) thisRow[2] = address[0];
                    if (address.Length > 1) thisRow[3] = address[1];
                    if (address.Length > 2) thisRow[4] = address[2];
                    if (address.Length > 3) thisRow[5] = address[3];
                    if (address.Length > 4) thisRow[6] = address[4];
                    if (address.Length > 5) thisRow[7] = address[5];
                    if (address.Length > 6) thisRow[8] = address[6];
                    if (address.Length > 7) thisRow[9] = address[7];
                }
            }

            // Populate the IP address stuff here
            if (ip_addresses.Rows.Count != 0)
            {
                ip_to_agent.Clear();
                ip_hits.Clear();
                foreach (DataRow thisRow in ip_addresses.Rows)
                {
                    string ip = thisRow[0].ToString();
                    string useragent = thisRow[1].ToString();
                    int hits = Convert.ToInt32(thisRow[2]);

                    ip_to_agent[ip] = useragent;
                    ip_hits[ip] = hits;
                }
                ip_addresses.Clear();
            }

            if (ip_hits.Count > 0)
            {
                SortedList<int, List<string>> sort_ip = new SortedList<int, List<string>>();
                foreach (string thisKey in ip_hits.Keys)
                {
                    int hits = ip_hits[thisKey];
                    if (sort_ip.ContainsKey(hits))
                    {
                        sort_ip[hits].Add(thisKey);
                    }
                    else
                    {
                        List<string> new_ip_list = new List<string> {thisKey};
                        sort_ip[hits] = new_ip_list;
                    }
                }

                foreach (KeyValuePair<int, List<string>> ipPair in sort_ip)
                {
                    if (ipPair.Key >= hit_ip_cutoff)
                    {
                        foreach (string IP in ipPair.Value)
                        {
                            DataRow newRow = ip_addresses.NewRow();
                            newRow[0] = IP;
                            if (ip_to_agent.ContainsKey(IP))
                                newRow[1] = ip_to_agent[IP];

                            newRow[2] = ipPair.Key;
                            ip_addresses.Rows.Add(newRow);
                        }
                    }
                }
            }

            stats.WriteXml(directory + "\\" + filename, XmlWriteMode.WriteSchema);
        }

        /// <summary> Write this data as XML </summary>
        /// <param name="directory"> Directory to write this XML into </param>
        public void Write_XML(string directory)
        {
            Write_XML(directory, date.Year + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0') + ".xml");
        }

        /// <summary> Merge this data with another existing set of data  </summary>
        /// <remarks> Used when merging daily sets into a monthly set </remarks>
        /// <param name="Stats_DataSet"> Set of usage data to merge with this one </param>
        public void Merge(SobekCM_Stats_DataSet Stats_DataSet)
        {
            // Add overall stats
            foreach (DataRow thisRow in Stats_DataSet.SobekCM_Stats_Table.Rows)
            {
                sobekcm_stats.Rows[0][0] = Convert.ToInt32(sobekcm_stats.Rows[0][0]) + Convert.ToInt32(thisRow[0]);
                sobekcm_stats.Rows[0][1] = Convert.ToInt32(sobekcm_stats.Rows[0][1]) + Convert.ToInt32(thisRow[1]);
                sobekcm_stats.Rows[0][2] = Convert.ToInt32(sobekcm_stats.Rows[0][2]) + Convert.ToInt32(thisRow[2]);
                sobekcm_stats.Rows[0][3] = Convert.ToInt32(sobekcm_stats.Rows[0][3]) + Convert.ToInt32(thisRow[3]);
                sobekcm_stats.Rows[0][4] = Convert.ToInt32(sobekcm_stats.Rows[0][4]) + Convert.ToInt32(thisRow[4]);
                sobekcm_stats.Rows[0][5] = Convert.ToInt32(sobekcm_stats.Rows[0][5]) + Convert.ToInt32(thisRow[5]);
            }

            // Add collection stats
            foreach (DataRow thisRow in Stats_DataSet.Collection_Stats_Table.Rows)
            {
                if (collection_rows.ContainsKey(thisRow[0].ToString()))
                {
                    // Sum the two rows together
                    DataRow matchRow = collection_rows[thisRow[0].ToString()];
                    matchRow[1] = Convert.ToInt32(matchRow[1]) + Convert.ToInt32(thisRow[1]);
                    matchRow[2] = Convert.ToInt32(matchRow[2]) + Convert.ToInt32(thisRow[2]);
                    matchRow[3] = Convert.ToInt32(matchRow[3]) + Convert.ToInt32(thisRow[3]);
                    matchRow[4] = Convert.ToInt32(matchRow[4]) + Convert.ToInt32(thisRow[4]);
                    matchRow[5] = Convert.ToInt32(matchRow[5]) + Convert.ToInt32(thisRow[5]);
                }
                else
                {
                    // Add as a new row
                    DataRow newCollectionRow = collection_stats.NewRow();
                    newCollectionRow[0] = thisRow[0];
                    newCollectionRow[1] = thisRow[1];
                    newCollectionRow[2] = thisRow[2];
                    newCollectionRow[3] = thisRow[3];
                    newCollectionRow[4] = thisRow[4];
                    newCollectionRow[5] = thisRow[5];
                    collection_stats.Rows.Add(newCollectionRow);
                    collection_rows.Add(thisRow[0].ToString(), newCollectionRow);
                }
            }

            // Add institutional stats
            foreach (DataRow thisRow in Stats_DataSet.Institution_Stats_Table.Rows)
            {
                if (institution_rows.ContainsKey(thisRow[0].ToString()))
                {
                    // Sum the two rows together
                    DataRow matchRow = institution_rows[thisRow[0].ToString()];
                    matchRow[1] = Convert.ToInt32(matchRow[1]) + Convert.ToInt32(thisRow[1]);
                    matchRow[2] = Convert.ToInt32(matchRow[2]) + Convert.ToInt32(thisRow[2]);
                    matchRow[3] = Convert.ToInt32(matchRow[3]) + Convert.ToInt32(thisRow[3]);
                    matchRow[4] = Convert.ToInt32(matchRow[4]) + Convert.ToInt32(thisRow[4]);
                    matchRow[5] = Convert.ToInt32(matchRow[5]) + Convert.ToInt32(thisRow[5]);
                }
                else
                {
                    // Add as a new row
                    DataRow newInstitutionRow = institution_stats.NewRow();
                    newInstitutionRow[0] = thisRow[0];
                    newInstitutionRow[1] = thisRow[1];
                    newInstitutionRow[2] = thisRow[2];
                    newInstitutionRow[3] = thisRow[3];
                    newInstitutionRow[4] = thisRow[4];
                    newInstitutionRow[5] = thisRow[5];
                    institution_stats.Rows.Add(newInstitutionRow);
                    institution_rows.Add(thisRow[0].ToString(), newInstitutionRow);
                }
            }

            // Add the bib-level stats
            foreach (DataRow thisRow in Stats_DataSet.Bib_Stats_Table.Rows)
            {
                if (bib_rows.ContainsKey(thisRow[0].ToString()))
                {
                    // Sum the two rows together
                    DataRow matchRow = bib_rows[thisRow[0].ToString()];
                    matchRow[1] = Convert.ToInt32(matchRow[1]) + Convert.ToInt32(thisRow[1]);
                    matchRow[2] = Convert.ToInt32(matchRow[2]) + Convert.ToInt32(thisRow[2]);
                }
                else
                {
                    // Add as a new row
                    DataRow newBibRow = bib_stats.NewRow();
                    newBibRow[0] = thisRow[0];
                    newBibRow[1] = thisRow[1];
                    newBibRow[2] = thisRow[2];
                    bib_stats.Rows.Add(newBibRow);
                    bib_rows.Add(thisRow[0].ToString(), newBibRow);
                }
            }

            // Add the item-level stats
            foreach (DataRow thisRow in Stats_DataSet.Item_Stats_Table.Rows)
            {
                if (item_rows.ContainsKey(Convert.ToInt32(thisRow["itemID"])))
                {
                    // Sum the two rows together
                    DataRow matchRow = item_rows[Convert.ToInt32(thisRow["itemID"])];
                    matchRow[3] = Convert.ToInt32(matchRow[3]) + Convert.ToInt32(thisRow[3]);
                    matchRow[4] = Convert.ToInt32(matchRow[4]) + Convert.ToInt32(thisRow[4]);
                    matchRow[5] = Convert.ToInt32(matchRow[5]) + Convert.ToInt32(thisRow[5]);
                    matchRow[6] = Convert.ToInt32(matchRow[6]) + Convert.ToInt32(thisRow[6]);
                    matchRow[7] = Convert.ToInt32(matchRow[7]) + Convert.ToInt32(thisRow[7]);
                    matchRow[8] = Convert.ToInt32(matchRow[8]) + Convert.ToInt32(thisRow[8]);
                    matchRow[9] = Convert.ToInt32(matchRow[9]) + Convert.ToInt32(thisRow[9]);
                    matchRow[10] = Convert.ToInt32(matchRow[10]) + Convert.ToInt32(thisRow[10]);
                    matchRow[11] = Convert.ToInt32(matchRow[11]) + Convert.ToInt32(thisRow[11]);
                    matchRow[12] = Convert.ToInt32(matchRow[12]) + Convert.ToInt32(thisRow[12]);
                    matchRow[13] = Convert.ToInt32(matchRow[13]) + Convert.ToInt32(thisRow[13]);
                    matchRow[14] = Convert.ToInt32(matchRow[14]) + Convert.ToInt32(thisRow[14]);
                }
                else
                {
                    // Add as a new row
                    DataRow newItemRow = item_stats.NewRow();
                    newItemRow[0] = thisRow[0];
                    newItemRow[1] = thisRow[1];
                    newItemRow[2] = thisRow[2];
                    newItemRow[3] = thisRow[3];
                    newItemRow[4] = thisRow[4];
                    newItemRow[5] = thisRow[5];
                    newItemRow[6] = thisRow[6];
                    newItemRow[7] = thisRow[7];
                    newItemRow[8] = thisRow[8];
                    newItemRow[9] = thisRow[9];
                    newItemRow[10] = thisRow[10];
                    newItemRow[11] = thisRow[11];
                    newItemRow[12] = thisRow[12];
                    newItemRow[13] = thisRow[13];
                    newItemRow[14] = thisRow[14];
                    item_stats.Rows.Add(newItemRow);
                    item_rows.Add(Convert.ToInt32(thisRow["itemID"]), newItemRow);
                }
            }

            // Add the IP addresses
            foreach (DataRow thisRow in Stats_DataSet.IP_Addresses.Rows)
            {
                if (ip_rows.ContainsKey(thisRow[0].ToString()))
                {
                    // Sum the two rows together
                    DataRow matchRow = ip_rows[thisRow[0].ToString()];
                    matchRow[2] = Convert.ToInt32(matchRow[2]) + Convert.ToInt32(thisRow[2]);
                }
                else
                {
                    // Add as a new row
                    DataRow newIpRow = ip_addresses.NewRow();
                    newIpRow[0] = thisRow[0];
                    newIpRow[1] = thisRow[1];
                    newIpRow[2] = thisRow[2];
                    ip_addresses.Rows.Add(newIpRow);
                    ip_rows.Add(thisRow[0].ToString(), newIpRow);
                }
            }

            // Add the portal stats
            if (Stats_DataSet.Portal_Stats_Table != null)
            {
                foreach (DataRow thisRow in Stats_DataSet.Portal_Stats_Table.Rows)
                {
                    string portal = thisRow[0].ToString().ToUpper();
                    if (portal_rows.ContainsKey(portal))
                    {
                        // Sum the two rows together
                        DataRow matchRow = portal_rows[portal];
                        matchRow[1] = Convert.ToInt32(matchRow[1]) + Convert.ToInt32(thisRow[1]);
                    }
                    else
                    {
                        // Add as a new row
                        DataRow newPortalRow = portal_stats.NewRow();
                        newPortalRow[0] = portal;
                        newPortalRow[1] = thisRow[1];
                        portal_stats.Rows.Add(newPortalRow);
                        portal_rows.Add(portal, newPortalRow);
                    }
                }
            }

            // Add the webcontent stats
            if (Stats_DataSet.WebContent_Stats_Table != null)
            {
                foreach (DataRow thisRow in Stats_DataSet.WebContent_Stats_Table.Rows)
                {
                    if (webcontent_rows.ContainsKey(thisRow[0].ToString()))
                    {
                        // Sum the two rows together
                        DataRow matchRow = webcontent_rows[thisRow[0].ToString()];
                        matchRow[1] = Convert.ToInt32(matchRow[1]) + Convert.ToInt32(thisRow[1]);
                    }
                    else
                    {
                        // Add as a new row
                        DataRow newWebContentRow = webcontent_stats.NewRow();
                        newWebContentRow[0] = thisRow[0];
                        newWebContentRow[1] = thisRow[1];
                        webcontent_stats.Rows.Add(newWebContentRow);
                        webcontent_rows.Add(thisRow[0].ToString(), newWebContentRow);
                    }
                }
            }
        }
    }
}