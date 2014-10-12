#region Using directives

using System;
using System.IO;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.PreProcess
{
    public class ProcessPendingFdaReportsModule : abstractPreProcessModule
    {
        /// <summary> Check for any FDA/DAITSS reports which were dropped into a FDA/DAITSS report drop box </summary>
        public override void DoWork()
        {
            // Step through each incoming folder and look for FDA reports
            if ((InstanceWide_Settings_Singleton.Settings.FDA_Report_DropBox.Length > 0) && (Directory.Exists(InstanceWide_Settings_Singleton.Settings.FDA_Report_DropBox)))
            {
                // Create the FDA process
                FDA_Report_Processor fdaProcessor = new FDA_Report_Processor();

                // Process all pending FDA reports
                fdaProcessor.Process(InstanceWide_Settings_Singleton.Settings.FDA_Report_DropBox);

                // Log successes and failures
                if ((fdaProcessor.Error_Count > 0) || (fdaProcessor.Success_Count > 0))
                {
                    // Clear any previous report
                    SobekCM_Database.Builder_Clear_Item_Error_Log("FDA REPORT", "", "SobekCM Builder");

                    if (fdaProcessor.Error_Count > 0)
                    {
                        OnError("Processed " + fdaProcessor.Success_Count + " FDA reports with " + fdaProcessor.Error_Count + " errors", String.Empty, String.Empty, -1);
                    }
                    else
                    {
                        OnProcess("Processed " + fdaProcessor.Success_Count + " FDA reports", "Standard", String.Empty, String.Empty, -1);
                    }
                }
            }
        }
    }
}
