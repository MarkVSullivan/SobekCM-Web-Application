namespace SobekCM.Builder_Library.Modules.Schedulable
{
    public class RebuildAllAggregationBrowsesModule : abstractSchedulableModule
    {
        public override void DoWork()
        {
            //// Rebuild all the static pages
            //Console.WriteLine(dbInstance.Name + " - Rebuilding all static pages");
            //preloader_logger.AddNonError(dbInstance.Name + " - Rebuilding all static pages");
            //long staticRebuildLogId = Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Rebuilding all static pages", String.Empty);

            //Static_Pages_Builder builder = new Static_Pages_Builder(InstanceWide_Settings_Singleton.Settings.Application_Server_URL, InstanceWide_Settings_Singleton.Settings.Static_Pages_Location, InstanceWide_Settings_Singleton.Settings.Application_Server_Network);
            //builder.Rebuild_All_Static_Pages(preloader_logger, false, InstanceWide_Settings_Singleton.Settings.Local_Log_Directory, dbInstance.Name, staticRebuildLogId);


        }
    }
}
