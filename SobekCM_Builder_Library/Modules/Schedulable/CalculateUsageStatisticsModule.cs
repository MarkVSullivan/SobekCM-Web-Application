using System;
using System.IO;
using SobekCM.Builder_Library.Statistics;
using SobekCM.Core.Settings;

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    /// <summary> Module reads the IIS usage logs and loads statistics from the previous month
    /// into the SobekCM database for display </summary>
    public class CalculateUsageStatisticsModule : abstractSchedulableModule
    {
        public override void DoWork(InstanceWide_Settings Settings)
        {
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



            string log_directory = String.Empty;
            string sql_directory = String.Empty;
            string sobekcm_directory = String.Empty;
            string[] year_month = new string[] {};

            // Create the processor
            SobekCM_Stats_Reader_Processor processor = new SobekCM_Stats_Reader_Processor(log_directory, sql_directory, temporary_workspace, sobekcm_directory, year_month);
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
