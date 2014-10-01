using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder.Modules.Schedulable
{
    public class UpdatedCachedAggreagtionMetadataModule : abstractSchedulableModule
    {
        public override void DoWork()
        {
            Library.Database.SobekCM_Database.Admin_Update_Cached_Aggregation_Metadata_Links();
        }
    }
}
