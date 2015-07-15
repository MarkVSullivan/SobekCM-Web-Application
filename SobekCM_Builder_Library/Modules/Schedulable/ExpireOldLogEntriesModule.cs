#region Using directives

using SobekCM.Core.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    /// <summary> Schedulable builder module expires (and deletes) expired log files, per instance-wide settings </summary>
    /// <remarks> This class implements the <see cref="abstractSchedulableModule" /> abstract class and implements the <see cref="iSchedulableModule" /> interface. </remarks>
    public class ExpireOldLogEntriesModule : abstractSchedulableModule
    {
        /// <summary> Clears the old logs that are ready to be expired, per instance-wide settings </summary>
        /// <param name="Settings"> Instance-wide settings which may be required for this process </param>
        public override void DoWork(InstanceWide_Settings Settings)
        {
            // CLear the old logs
            //Console.WriteLine(dbInstance.Name + " - Expiring old log entries");
            //preloader_logger.AddNonError(dbInstance.Name + " - Expiring old log entries");
            //Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Expiring old log entries", String.Empty);
            //Library.Database.SobekCM_Database.Builder_Expire_Log_Entries(InstanceWide_Settings_Singleton.Settings.Builder_Log_Expiration_Days);
        }
    }
}
