#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.UI;

#endregion

namespace SobekCM.Builder_Library.Statistics
{
    /// <summary> Reads the usage logs and outputs a corresponding <see cref="SobekCM_Stats_DataSet"/> object </summary>
    /// <remarks> This class is used by the SobekCM Stats Reader app </remarks>
    public class SobekCM_Log_Reader
    {
        private SortedList<SobekCM_Hit, SobekCM_Hit> hits;
        private readonly DataTable itemList;
        private Dictionary<string, SobekCM_Session> sessions;

        /// <summary> Constructor for a new instance of the SobekCM_Log_Reader class </summary>
        /// <param name="Item_List"> List of all items </param>
        /// <param name="SobekCM_Web_App_Directory"></param>
        public SobekCM_Log_Reader(DataTable Item_List, string SobekCM_Web_App_Directory )
        {
            itemList = Item_List;

            // Set the constant settings base directory value to the production location
            UI_ApplicationCache_Gateway.Settings.Base_Directory = SobekCM_Web_App_Directory;
        }

        /// <summary> Read a IIS web log, analyze completely, and return the corresponding <see cref="SobekCM_Stats_DataSet"/> object </summary>
        /// <param name="Log_File"> Location for the log file to read </param>
        /// <returns> Object with all the analyzed hits and sessions from the web log </returns>
        public SobekCM_Stats_DataSet Read_Log(string Log_File)
        {
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
                        Navigation_Object currentMode = new Navigation_Object();
                        QueryString_Analyzer.Parse_Query(queryStringCollection, currentMode, hit.SobekCM_URL,
                            new string[] { "en" }, Engine_ApplicationCache_Gateway.Codes, Engine_ApplicationCache_Gateway.Collection_Aliases,
                            Engine_ApplicationCache_Gateway.Items, Engine_ApplicationCache_Gateway.URL_Portals, Engine_ApplicationCache_Gateway.WebContent_Hierarchy, null);

                        if (currentMode != null)
                            currentMode.Set_Robot_Flag(hit.UserAgent, hit.IP);
                        if ((currentMode != null) && (!currentMode.Is_Robot))
                        {
                            // Always increment the hits
                            returnValue.Increment_Hits();

                            // Add this IP hit
                            returnValue.Add_IP_Hit(hit.IP, hit.UserAgent);

                            // Increment the portal hits
                            returnValue.Add_Portal_Hit(currentMode.Instance_Name.ToUpper());

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

                                    int itemid = -1;
                                    if (currentMode.ItemID_DEPRECATED.HasValue)
                                        itemid = currentMode.ItemID_DEPRECATED.Value;

                                    returnValue.Add_Item_Hit(itemid, currentMode.BibID,
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
                    catch (Exception)
                    {
                        // Do nothing.. not important?
                    }
                }
            }

            return returnValue;
        }

        private void parse_line(string StatsLine)
        {
            // If this was empty or a remark, also skip it
            if ((StatsLine.Length == 0) || (StatsLine[0] == '#'))
                return;

            // Uppercase this line for further analysis
            string stats_line_upper = StatsLine.ToUpper();

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
                int space_location = StatsLine.IndexOf(" ");
                space_location = StatsLine.IndexOf(" ", space_location + 1, StringComparison.Ordinal);
                string date_time = StatsLine.Substring(0, space_location).Trim();
                DateTime date = Convert.ToDateTime(date_time);

                if (stats_line_upper.IndexOf(" W3SVC3 SL2K3WEB1 ") > 0)
                {
                    space_location = StatsLine.IndexOf(" ", space_location + 1, StringComparison.Ordinal);
                }

                // Find the next space and the server IP address
                int ip_space_location = StatsLine.IndexOf(" ", space_location + 1, StringComparison.Ordinal);

                // Check this is a GET, and not a POST, which does not count
                space_location = StatsLine.IndexOf(" ", ip_space_location + 1, StringComparison.Ordinal);
                string action = StatsLine.Substring(ip_space_location, space_location - ip_space_location);
                if (action.Trim() != "GET")
                    return;

                // Find the url
                int url_space_location = StatsLine.IndexOf(" ", space_location + 1, StringComparison.Ordinal);
                string url_result = StatsLine.Substring(space_location, url_space_location - space_location).Trim();

                // Find the query string
                int query_string_space_location = StatsLine.IndexOf(" ", url_space_location + 1, StringComparison.Ordinal);
                string query_string =
                    StatsLine.Substring(url_space_location, query_string_space_location - url_space_location).Trim();
                if (query_string == "-")
                    query_string = String.Empty;

                // Now, find the origination IP address
                space_location = StatsLine.IndexOf(" ", query_string_space_location + 1, StringComparison.Ordinal);
                space_location = StatsLine.IndexOf(" ", space_location + 1, StringComparison.Ordinal);
                int source_ip_location = StatsLine.IndexOf(" ", space_location + 1, StringComparison.Ordinal);
                string ip = StatsLine.Substring(space_location, source_ip_location - space_location).Trim();

                int end_agent_location = StatsLine.IndexOf(" ", source_ip_location + 1, StringComparison.Ordinal);
                string useragent = StatsLine.Substring(source_ip_location + 1, end_agent_location - source_ip_location);

                // Create the hit object
                SobekCM_Hit thisHit = new SobekCM_Hit(date, ip, query_string.ToUpper(), url_result, useragent);

                // Add this URL to the list
                hits.Add(thisHit, thisHit);
            }
            catch (Exception)
            {
                // A single missed hit is not a big deal
            }
        }
    }
}