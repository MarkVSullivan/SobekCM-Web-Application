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

            //Static_Pages_Builder builder = new Static_Pages_Builder(SobekCM_Library_Settings.Application_Server_URL, SobekCM_Library_Settings.Static_Pages_Location, SobekCM_Library_Settings.Application_Server_Network);
            //builder.Rebuild_All_Static_Pages(preloader_logger, false, SobekCM_Library_Settings.Local_Log_Directory, dbInstance.Name, staticRebuildLogId);


        }
    }
}
