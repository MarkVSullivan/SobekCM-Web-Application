#region Using directives

using SobekCM.Core.Settings;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    /// <summary> Schedulable builder module updates the database-based cached aggregation browse information </summary>
    /// <remarks> This class implements the <see cref="abstractSchedulableModule" /> abstract class and implements the <see cref="iSchedulableModule" /> interface. </remarks>
    public class UpdatedCachedAggregationMetadataModule : abstractSchedulableModule
    {
        /// <summary> Updates the database-based cached aggregation browse information </summary>
        /// <param name="Settings"> Instance-wide settings which may be required for this process </param>
        public override void DoWork(InstanceWide_Settings Settings)
        {
            SobekCM_Database.Admin_Update_Cached_Aggregation_Metadata_Links();
        }
    }
}
