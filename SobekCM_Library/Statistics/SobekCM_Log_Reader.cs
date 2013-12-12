#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Windows.Forms;
using SobekCM.Library;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Skins;

#endregion

namespace SobekCM.Library.Statistics
{
    /// <summary> Reads the usage logs and outputs a corresponding <see cref="SobekCM_Stats_DataSet"/> object </summary>
    /// <remarks> This class is used by the SobekCM Stats Reader app </remarks>
    public class SobekCM_Log_Reader
    {

        private static Aggregation_Code_Manager Codes;
        private static Dictionary<string, string> Collection_Aliases;
        private static Dictionary<string, Wordmark_Icon> Icon_List;
        private static IP_Restriction_Ranges IP_Restrictions;
        private static Item_Lookup_Object Item_Lookup_Object;
        private static DateTime Last_Refresh;
        private static Recent_Searches Search_History;
        private static List<string> Search_Stop_Words;
        private static SobekCM_Skin_Collection Skins;
        private static Statistics_Dates Stats_Date_Range;
        private static List<Thematic_Heading> Thematic_Headings;
        private static Language_Support_Info Translation;
        private static Portal_List URL_Portals;
        private static string Version;
        private static Dictionary<string, Mime_Type_Info> Mime_Types; 


        private List<string> dloc_ips;
        private SortedList<SobekCM_Hit, SobekCM_Hit> hits;
        private DataTable itemList;
        private Dictionary<string, SobekCM_Session> sessions;

        /// <summary> Constructor for a new instance of the SobekCM_Log_Reader class </summary>
        /// <param name="Item_List"> List of all items </param>
        /// <param name="SobekCM_Web_App_Directory"></param>
        public SobekCM_Log_Reader(DataTable Item_List, string SobekCM_Web_App_Directory )
        {
            itemList = Item_List;

            // Build the application state
            Custom_Tracer tracer = new Custom_Tracer();

            // Make sure all the needed data is loaded into the Application State
            Application_State_Builder.Build_Application_State(tracer, false, ref Skins, ref Translation,
                                                              ref Codes, ref Item_Lookup_Object, ref Icon_List,
                                                              ref Stats_Date_Range, ref Thematic_Headings,
                                                              ref Collection_Aliases, ref IP_Restrictions,
                                                              ref URL_Portals, ref Mime_Types);

            // The cache needs to be disabled
            Cached_Data_Manager.Disabled = true;

            // Set the constant settings base directory value to the production location
            SobekCM_Library_Settings.Base_Directory = SobekCM_Web_App_Directory;
        }

        /// <summary> Read a IIS web log, analyze completely, and return the corresponding <see cref="SobekCM_Stats_DataSet"/> object </summary>
        /// <param name="Log_File"> Location for the log file to read </param>
        /// <returns> Object with all the analyzed hits and sessions from the web log </returns>
        public SobekCM_Stats_DataSet Read_Log(string Log_File)
        {
            // Get list of ips which include dloc.com
            dloc_ips = new List<string>();

            // Create the list of hits
            hits = new SortedList<SobekCM_Hit, SobekCM_Hit>();

            // Create the list of sessions
            sessions = new Dictionary<string, SobekCM_Session>();

            // Create the return set
            SobekCM_Stats_DataSet returnValue = new SobekCM_Stats_DataSet();

            // Get the date of the log file
            FileInfo fileInfo = new FileInfo(Log_File);
            string name = fileInfo.Name.Replace(fileInfo.Extension, "");
            DateTime logDate = new DateTime(Convert.ToInt32("20" + name.Substring(4, 2)),
                                            Convert.ToInt32(name.Substring(6, 2)), Convert.ToInt32(name.Substring(8, 2)));
            returnValue.Date = logDate;

            // Open a connection to the log file and save each hit
            StreamReader reader = new StreamReader(Log_File);
            string line = reader.ReadLine();
            while (line != null)
            {
                parse_line(line);
                line = reader.ReadLine();
            }

            // Now, step through each hit in the list
            foreach (SobekCM_Hit hit in hits.Values)
            {
				if (hit.SobekCM_URL.ToUpper().IndexOf(".ASPX") < 0)
                {
                    // Always increment the hits
                    returnValue.Increment_Hits();

                    // Add this IP hit
                    returnValue.Add_IP_Hit(hit.IP, hit.UserAgent);

                    // Shouldn't start with '/'
					if (hit.SobekCM_URL[0] == '/')
                    {
						hit.SobekCM_URL = hit.SobekCM_URL.Substring(1);
                    }
					hit.SobekCM_URL = hit.SobekCM_URL.ToLower();
					if (hit.SobekCM_URL.IndexOf("design/webcontent/") == 0)
						hit.SobekCM_URL = hit.SobekCM_URL.Substring(18);

                    // Add this as a webcontent hit
					returnValue.Add_WebContent_Hit(hit.SobekCM_URL);
                }
                else
                {
                    // parse the url
                    string[] splitter = hit.Query_String.ToLower().Split("&".ToCharArray());
                    NameValueCollection queryStringCollection = new NameValueCollection();
                    foreach (string thisSplit in splitter)
                    {
                        int equals_index = thisSplit.IndexOf("=");
                        if ((equals_index > 0) && (equals_index < thisSplit.Length - 1))
                        {
                            string query_name = thisSplit.Substring(0, equals_index);
                            string query_value = thisSplit.Substring(equals_index + 1);
                            queryStringCollection[query_name] = query_value;

                            if (query_name.ToLower() == "portal")
								hit.SobekCM_URL = query_value;
                        }
                    }

                    // Now, get the navigation object using the standard SobekCM method

                    try
                    {
						SobekCM_Navigation_Object currentMode = new SobekCM_Navigation_Object(queryStringCollection, hit.SobekCM_URL,
                                                                                              new string[] {"en"}, Codes, Collection_Aliases,
                                                                                              ref Item_Lookup_Object, URL_Portals, null);
                        if (currentMode != null)
                            currentMode.Set_Robot_Flag(hit.UserAgent, hit.IP);
                        if ((currentMode != null) && (!currentMode.Is_Robot))
                        {
                            // Always increment the hits
                            returnValue.Increment_Hits();

                            // Add this IP hit
                            returnValue.Add_IP_Hit(hit.IP, hit.UserAgent);

                            // Increment the portal hits
                            returnValue.Add_Portal_Hit(currentMode.SobekCM_Instance_Name.ToUpper());

                            // Check for pre-existing session
                            SobekCM_Session thisSession;
                            if (sessions.ContainsKey(hit.IP))
                            {
                                SobekCM_Session possibleSession = sessions[hit.IP];
                                TimeSpan difference = hit.Time.Subtract(possibleSession.Last_Hit);
                                if (difference.TotalMinutes >= 60)
                                {
                                    thisSession = new SobekCM_Session(hit.IP, hit.Time);
                                    sessions[hit.IP] = thisSession;

                                    returnValue.Increment_Sessions();
                                }
                                else
                                {
                                    possibleSession.Last_Hit = hit.Time;
                                    thisSession = possibleSession;
                                }
                            }
                            else
                            {
                                thisSession = new SobekCM_Session(hit.IP, hit.Time);
                                sessions.Add(hit.IP, thisSession);

                                returnValue.Increment_Sessions();
                            }

                            if ((currentMode.Mode == Display_Mode_Enum.Item_Display) ||
                                (currentMode.Mode == Display_Mode_Enum.Item_Print))
                            {
                                if ((currentMode.ItemID_DEPRECATED > 0) ||
                                    ((currentMode.VID.Length > 0) && (currentMode.BibID.Length > 0)))
                                {
                                    if (currentMode.ItemID_DEPRECATED <= 0)
                                    {
                                        DataRow[] sobek2_bib_select =
                                            itemList.Select("bibid = '" + currentMode.BibID + "' and vid='" +
                                                            currentMode.VID + "'");
										if (sobek2_bib_select.Length > 0)
                                        {
                                            currentMode.ItemID_DEPRECATED =
												Convert.ToInt32(sobek2_bib_select[0]["itemid"]);
                                        }
                                    }

                                    returnValue.Add_Item_Hit(currentMode.ItemID_DEPRECATED, currentMode.BibID,
                                                             currentMode.VID, currentMode.ViewerCode.ToUpper(),
                                                             currentMode.Text_Search, thisSession.SessionID);
                                }
                                else if (currentMode.BibID.Length > 0)
                                {
                                    returnValue.Add_Bib_Hit(currentMode.BibID.ToUpper(), thisSession.SessionID);
                                }
                            }
                            else
                            {
                                string code = currentMode.Aggregation;
                                string institution = String.Empty;
                                if ((code.Length > 0) && (code.ToUpper()[0] == 'I'))
                                {
                                    institution = code;
                                    code = String.Empty;
                                }


                                if ((institution.Length > 0) && (institution.ToUpper()[0] != 'I'))
                                    institution = "i" + institution;

                                // For some collections we are counting the institution hit and collection
                                // hit just so the full use of the site is recorded
                                if (code.Length > 0)
                                {
                                    returnValue.Add_Collection_Hit(code.ToLower(), currentMode.Mode, currentMode.Aggregation_Type, thisSession.SessionID);
                                }

                                // Was this an institutional level hit?
                                if (institution.Length > 0)
                                {
                                    returnValue.Add_Institution_Hit(institution.ToLower(), currentMode.Mode, currentMode.Aggregation_Type, thisSession.SessionID);
                                }

                                // Is this a static "webcontent" top-level page?
                                if (currentMode.Mode == Display_Mode_Enum.Simple_HTML_CMS)
                                {
                                    if ((currentMode.Info_Browse_Mode != "unknown") &&
                                        (currentMode.Info_Browse_Mode != "default"))
                                    {
                                        returnValue.Add_WebContent_Hit(currentMode.Info_Browse_Mode.ToLower());
                                    }
                                }

                                // Add the write type, if not normal HTML stuff
                                switch (currentMode.Writer_Type)
                                {
                                    case Writer_Type_Enum.DataSet:
                                    case Writer_Type_Enum.XML:
                                        returnValue.Add_XML_Hit();
                                        break;

                                    case Writer_Type_Enum.OAI:
                                        returnValue.Add_OAI_Hit();
                                        break;

                                    case Writer_Type_Enum.JSON:
                                        returnValue.Add_JSON_Hit();
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if ((currentMode != null) && (currentMode.Is_Robot))
                                returnValue.Add_Robot_Hit();
                        }
                    }
                    catch (Exception ee)
                    {
                        bool error = true;
                    }
                }
            }

            return returnValue;
        }

        private void parse_line(string stats_line)
        {
            // If this was empty or a remark, also skip it
            if ((stats_line.Length == 0) || (stats_line[0] == '#'))
                return;

            // Uppercase this line for further analysis
            string stats_line_upper = stats_line.ToUpper();

            // Leave out any UNKNOWN UNKNOWN
            if (stats_line_upper.IndexOf("UNKNOWN UNKNOWN") > 0)
                return;

            // If this was not just .. and did not include the .ASPX, then return (i.e., static images,etc..)
            if ((stats_line_upper.IndexOf("/SOBEKCM_DATA.ASPX") < 0) && 
                (stats_line_upper.IndexOf("/SOBEKCM_DATA.ASPX") < 0) && (stats_line_upper.IndexOf("/SOBEKCM_OAI.ASPX") < 0) &&
                (stats_line_upper.IndexOf("/SOBEKCM.ASPX") < 0) && (stats_line_upper.IndexOf(".MSI") < 0) &&
                (stats_line_upper.IndexOf(".ZIP") < 0))
                return;

            try
            {
                // Find the first two spaces.. and then the date
                int space_location = stats_line.IndexOf(" ");
                space_location = stats_line.IndexOf(" ", space_location + 1);
                string date_time = stats_line.Substring(0, space_location).Trim();
                DateTime date = Convert.ToDateTime(date_time);

                if (stats_line_upper.IndexOf(" W3SVC3 SL2K3WEB1 ") > 0)
                {
                    space_location = stats_line.IndexOf(" ", space_location + 1);
                    space_location = stats_line.IndexOf(" ", space_location + 1);
                }

                // Find the next space and the server IP address
                int ip_space_location = stats_line.IndexOf(" ", space_location + 1);
                string server_ip = stats_line.Substring(space_location, ip_space_location - space_location).Trim();

                // Check this is a GET, and not a POST, which does not count
                space_location = stats_line.IndexOf(" ", ip_space_location + 1);
                string action = stats_line.Substring(ip_space_location, space_location - ip_space_location);
                if (action.Trim() != "GET")
                    return;

                // Find the url
                int url_space_location = stats_line.IndexOf(" ", space_location + 1);
                string url_result = stats_line.Substring(space_location, url_space_location - space_location).Trim();

                // Find the query string
                int query_string_space_location = stats_line.IndexOf(" ", url_space_location + 1);
                string query_string =
                    stats_line.Substring(url_space_location, query_string_space_location - url_space_location).Trim();
                if (query_string == "-")
                    query_string = String.Empty;

                // Now, find the origination IP address
                space_location = stats_line.IndexOf(" ", query_string_space_location + 1);
                space_location = stats_line.IndexOf(" ", space_location + 1);
                int source_ip_location = stats_line.IndexOf(" ", space_location + 1);
                string ip = stats_line.Substring(space_location, source_ip_location - space_location).Trim();

                int end_agent_location = stats_line.IndexOf(" ", source_ip_location + 1);
                string useragent = stats_line.Substring(source_ip_location + 1, end_agent_location - source_ip_location);

                // Create the hit object
                SobekCM_Hit thisHit = new SobekCM_Hit(date, ip, query_string.ToUpper(), url_result, useragent);

                // Add this URL to the list
                hits.Add(thisHit, thisHit);
            }
            catch (Exception ee)
            {
                MessageBox.Show("ERROR IN parse_line( " + stats_line + " )\n\n" + ee.ToString());
            }
        }
    }
}