#region Using directives

using SobekCM.Core.Settings;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    public class UpdatedCachedAggreagtionMetadataModule : abstractSchedulableModule
    {
        public override void DoWork(InstanceWide_Settings Settings)
        {
            SobekCM_Database.Admin_Update_Cached_Aggregation_Metadata_Links();
        }
    }
}
