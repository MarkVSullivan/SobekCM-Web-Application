#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Library.Statistics
{

    #region Definitions of delegates

    /// <summary> Delegate for the custom event which is fired when the status
    /// string on the main form needs to change </summary>
    public delegate void SobekCM_Stats_Reader_Processor_New_Status_Delegate(string new_message);

    #endregion

    /// <summary> Class processes all web IIS logs to SQL insert commands in a worker thread </summary>
    /// <remarks> This class is used by the SobekCM Stats Reader app </remarks>
    public class SobekCM_Stats_Reader_Processor
    {
        private readonly string dataset_location;
        private readonly string sobekcm_log_location;
        private readonly string sobekcm_web_location;
        private readonly string sql_output_location;
        private readonly string[] year_months;

        /// <summary> Constructor for a new instance of the SobekCM_Stats_Reader_Processor worker class </summary>
        /// <param name="SobekCM_Log_Location">  </param>
        /// <param name="SQL_Output_Location"></param>
        /// <param name="Temporary_Workspace"></param>
        /// <param name="SobekCM_Web_App_Directory"> </param>
        /// <param name="Year_Months"></param>
        public SobekCM_Stats_Reader_Processor(string SobekCM_Log_Location, string SQL_Output_Location, string Temporary_Workspace, string SobekCM_Web_App_Directory, string[] Year_Months)
        {
            sobekcm_log_location = SobekCM_Log_Location;
            dataset_location = Temporary_Workspace;
            sql_output_location = SQL_Output_Location;
            sobekcm_web_location = SobekCM_Web_App_Directory;
            year_months = Year_Months;
        }

        public event SobekCM_Stats_Reader_Processor_New_Status_Delegate New_Status;

        private void On_New_Status(string New_Message)
        {
            if (New_Status != null)
                New_Status(New_Message);
        }

        /// <summary> Process the IIS web logs to SQL insert commands </summary>
        public void Process_IIS_Logs()
        {
            // **** READ THE LOOKUP TABLES FROM THE DATABASE **** //
            DataSet lookupTables = SobekCM_Database.Get_Statistics_Lookup_Tables();

            // ***** CODE BELOW READS ALL THE LOG FILES AND THEN WRITES THEM AS XML DATASETS *****//


            // ***** CODE BELOW READS ALL THE DAILY XML DATASETS AND COMBINES THEM INTO MONTHLY *****//
            // ***** DATASETS WHICH ARE SUBSEQUENTLY WRITTEN AS XML DATASETS AS WELL            *****//    
            On_New_Status("Combining daily datasets into a monthly dataset");
            foreach (string year_month in year_months)
            {
                string[] year_month_files = Directory.GetFiles(dataset_location, year_month + "*.xml");
                if (year_month_files.Length > 0)
                {
                    SobekCM_Stats_DataSet combined = new SobekCM_Stats_DataSet();
                    foreach (string file in year_month_files)
                    {
                        if ((new FileInfo(file)).Name.IndexOf(year_month + ".xml") < 0)
                        {
                            SobekCM_Stats_DataSet daily = new SobekCM_Stats_DataSet();
                            daily.Read_XML(file);

                            combined.Merge(daily);
                        }
                    }

                    // Write the complete data set
                    combined.Write_XML(dataset_location, year_month + ".xml");

                    // Just write the highest users in a seperate, more readable, file
                    combined.Write_Highest_Users(dataset_location, "users_" + year_month + ".xml");
                }
            }

            //// ***** CODE BELOW READS THE MONTHLY DATASETS AND THEN WRITES THE SQL INSERTION SCRIPTS ***** //
            // Read all the data lists first for id lookups
            Dictionary<string, int> aggregationHash = Table_To_Hash(lookupTables.Tables[2]);
            Dictionary<string, int> bibHash = Table_To_Hash(lookupTables.Tables[1]);
            Dictionary<string, int> portalHash = new Dictionary<string, int>();
            foreach (DataRow thisRow in lookupTables.Tables[3].Rows)
            {
                if (!portalHash.ContainsKey(thisRow[2].ToString().ToUpper()))
                {
                    portalHash[thisRow[2].ToString().ToUpper()] = Convert.ToInt32(thisRow[0]);
                }
            }

            On_New_Status("Writing SQL insert commands");
            foreach (string yearmonth in year_months)
            {
                SobekCM_Stats_DataSet monthly;
                string thisFile = dataset_location + "\\" + yearmonth + ".xml";
                if (File.Exists(thisFile))
                {
                    monthly = new SobekCM_Stats_DataSet();
                    monthly.Read_XML(thisFile);
                    int year = Convert.ToInt32(yearmonth.Substring(0, 4));
                    int month = Convert.ToInt32(yearmonth.Substring(4));
                    monthly.Write_SQL_Inserts(sql_output_location + "\\" + yearmonth + ".sql", year, month, aggregationHash, bibHash, portalHash);
                }
            }

            On_New_Status("COMPLETE!");
        }

        private static Dictionary<string, int> Table_To_Hash(DataTable table)
        {
            Dictionary<string, int> returnValue = new Dictionary<string, int>();
            foreach (DataRow dataRow in table.Rows)
            {
                returnValue[dataRow[1].ToString().ToUpper()] = Convert.ToInt32(dataRow[0]);
            }
            return returnValue;
        }
    }
}