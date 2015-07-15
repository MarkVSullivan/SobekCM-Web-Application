#region Using directives

using SobekCM.Core.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    /// <summary> Schedulable builder module rebuilds the static browse pages for the instance and aggregations  </summary>
    /// <remarks> This class implements the <see cref="abstractSchedulableModule" /> abstract class and implements the <see cref="iSchedulableModule" /> interface. </remarks>
 public class RebuildAllAggregationBrowsesModule : abstractSchedulableModule
    {
        /// <summary> Rebuilds the static browse pages for the instance and aggregations </summary>
        /// <param name="Settings"> Instance-wide settings which may be required for this process </param>
        public override void DoWork(InstanceWide_Settings Settings)
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
