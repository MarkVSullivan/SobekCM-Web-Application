using SobekCM.Core.Settings;

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    public class ExpireOldLogEntriesModule : abstractSchedulableModule
    {
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
