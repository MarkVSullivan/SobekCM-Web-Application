using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Builder_Library.Statistics;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.Database;

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    /// <summary> Module reads the IIS usage logs and loads statistics from the previous month
    /// into the SobekCM database for display </summary>
    public class CalculateUsageStatisticsModule : abstractSchedulableModule
    {
        public override void DoWork(InstanceWide_Settings Settings)
        {
            // If there is no IIS web log set, then do nothing.  Don't even need a log here.
            if (String.IsNullOrEmpty(Settings.Builder_IIS_Logs_Directory))
                return;

            // Just don't run the first day of the month - ensures logs still not being written
            if (DateTime.Now.Day < 2)
                return;

            // Ensure directory exists and is accessible
            string log_directory = Settings.Builder_IIS_Logs_Directory;
            try
            {
                if (!Directory.Exists(log_directory))
                {
                    OnError("CalculateUsageStatisticsModule : IIS web log directory ( " + log_directory + " ) does not exists or is inaccessible", null, null, -1);
                    return;
                }
            }
            catch (Exception ee)
            {
                OnError("CalculateUsageStatisticsModule : IIS web log directory ( " + log_directory + " ) does not exists or is inaccessible : " + ee.Message, null, null, -1);
                return;
            }

            // Get the temporary workspace directory, and ensure it exists
            string temporary_workspace = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Temporary", "CalculateUsageStatisticsModule");
            try
            {
                if (!Directory.Exists(temporary_workspace))
                    Directory.CreateDirectory(temporary_workspace);
            }
            catch ( Exception ee )
            {
                OnError("CalculateUsageStatisticsModule : Unable to create the temporary workspace ( " + temporary_workspace + " ) : " + ee.Message, null, null, -1);
                return;
            }

            // Clear the temporary workspace
            try
            {
                string[] files = Directory.GetFiles(temporary_workspace);
                foreach (string thisFile in files)
                {
                    File.Delete(thisFile);
                }
            }
            catch (Exception ee)
            {
                OnError("CalculateUsageStatisticsModule : Error caught clearing existing files from the temporary workspace ( " + temporary_workspace + " ) : " + ee.Message, null, null, -1);
                return;
            }


            // Get (and double check) the web directory 
            if (String.IsNullOrEmpty(Settings.Application_Server_Network))
            {
                OnError("CalculateUsageStatisticsModule : No application server network setting!  Correct ASAP!", null, null, -1);
                return;
            }

            // Ensure directory exists and is accessible
            string sobekcm_directory = Settings.Application_Server_Network;
            try
            {
                if (!Directory.Exists(sobekcm_directory))
                {
                    OnError("CalculateUsageStatisticsModule : Web application server network directory ( " + sobekcm_directory + " ) does not exists or is inaccessible", null, null, -1);
                    return;
                }
            }
            catch (Exception ee)
            {
                OnError("CalculateUsageStatisticsModule : Web application server network  ( " + sobekcm_directory + " ) does not exists or is inaccessible : " + ee.Message, null, null, -1);
                return;
            }

            // Determine which year/months already have been analyzed for this instance
            Statistics_Dates statsDates = new Statistics_Dates();
            Engine_Database.Populate_Statistics_Dates(statsDates, null);

            // Get the list of all IIS web logs
            string[] log_files = Directory.GetFiles( log_directory, "u_ex*.log");

            // If no log files, just return
            if (log_files.Length == 0)
            {
                OnError("CalculateUsageStatisticsModule : No IIS web logs found in directory  ( " + log_directory + " )", null, null, -1);
                return;
            }

            // Get the earliest and latest log files from the IIS web logs
            string earliest = "ZZZZZZZZ";
            string latest = "AAAAAAAA";
            foreach (string thisFile in log_files)
            {
                string thisFileName = Path.GetFileNameWithoutExtension(thisFile);
                if ( String.Compare(thisFileName, earliest, StringComparison.OrdinalIgnoreCase) < 0 )
                    earliest = thisFileName;
                if ( String.Compare(thisFileName, latest, StringComparison.OrdinalIgnoreCase) > 0 )
                    latest = thisFileName;
            }

            // Parse them to determine the earliest and latest year/months
            int earliest_year = -1;
            int earliest_month = -1;
            int latest_year = -1;
            int latest_month = -1;
            try
            {
                earliest_year = 2000 + Int32.Parse(earliest.Substring(4, 2));
                earliest_month = Int32.Parse(earliest.Substring(6, 2));
                latest_year = 2000 + Int32.Parse(latest.Substring(4, 2));
                latest_month = Int32.Parse(latest.Substring(6, 2));
            }
            catch (Exception ee)
            {
                OnError("CalculateUsageStatisticsModule : Error parsing the earliest or latest log for year/month (" + earliest + " or " + latest + " )", null, null, -1);
                return;
            }
            
            

            // Determine what years/months are missing
            List<string> year_month = new List<string>();
            if (statsDates.Earliest_Year == 2000)
            {
                // No stats every collected, so collect them all
                int curr_year = earliest_year;
                int curr_month = earliest_month;
                while ((curr_year < latest_year) || ((curr_year == latest_year) && (curr_month <= latest_month)))
                {
                    if ((curr_year == DateTime.Now.Year) && (curr_month == DateTime.Now.Month))
                        break;

                    year_month.Add(curr_year + curr_month.ToString().PadLeft(2, '0'));

                    curr_month++;
                    if (curr_month > 12)
                    {
                        curr_year++;
                        curr_month = 1;
                    }
                }
            }
            else
            {
                // No stats every collected, so collect them all
                int curr_year = earliest_year;
                int curr_month = earliest_month;
                while ((curr_year < latest_year) || ((curr_year == latest_year) && (curr_month <= latest_month)))
                {
                    if ((curr_year == DateTime.Now.Year) && (curr_month == DateTime.Now.Month))
                        break;

                    if (( curr_year > statsDates.Latest_Year ) || (( curr_year == statsDates.Latest_Year ) && ( curr_month > statsDates.Latest_Month )))
                        year_month.Add(curr_year + curr_month.ToString().PadLeft(2, '0'));

                    curr_month++;
                    if (curr_month > 12)
                    {
                        curr_year++;
                        curr_month = 1;
                    }
                }
            }

            // If no year/months were added , then no work to do
            if (year_month.Count == 0)
                return;

            // Create the processor
            SobekCM_Stats_Reader_Processor processor = new SobekCM_Stats_Reader_Processor(log_directory, temporary_workspace, sobekcm_directory, year_month);
            processor.New_Status += new SobekCM_Stats_Reader_Processor_New_Status_Delegate(processor_New_Status);

            // Create the thread
            processor.Process_IIS_Logs();
        }

        void processor_New_Status(string NewMessage)
        {
            
            if (NewMessage == "COMPLETE!")
            {

            }

        }
    }
}
