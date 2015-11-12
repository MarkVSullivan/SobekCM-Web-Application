#region Using directives

using System;
using System.IO;
using SobekCM.Builder_Library.Tools;
using SobekCM.Core.Settings;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Builder_Library.Modules.PreProcess
{
    /// <summary> Pre-process module processes any incoming FDA (Florida Dark Archive) reports, saves the data to the database, and archives the report in the resource folder </summary>
    /// <remarks> This class implements the <see cref="abstractPreProcessModule" /> abstract class and implements the <see cref="iPreProcessModule" /> interface. </remarks>
    public class ProcessPendingFdaReportsModule : abstractPreProcessModule
    {
        /// <summary> Processes any incoming FDA (Florida Dark Archive) reports, saves the data to the database, and archives the report in the resource folder </summary>
        /// <param name="Settings"> Instance-wide settings which may be required for this process </param>
        public override void DoWork(InstanceWide_Settings Settings)
        {
            // Step through each incoming folder and look for FDA reports
            if (( !String.IsNullOrEmpty(Settings.Florida.FDA_Report_DropBox)) && (Directory.Exists(Settings.Florida.FDA_Report_DropBox)))
            {
                // Create the FDA process
                FDA_Report_Processor fdaProcessor = new FDA_Report_Processor();

                // Process all pending FDA reports
                fdaProcessor.Process(Settings.Florida.FDA_Report_DropBox);

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
