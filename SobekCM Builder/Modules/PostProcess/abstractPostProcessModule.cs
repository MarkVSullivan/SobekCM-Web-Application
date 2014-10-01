using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder.Modules.PostProcess
{
    public abstract class abstractPostProcessModule : iPostProcessModule
    {
        public abstract void DoWork(List<string> AggregationsAffected, List<BibVidStruct> ProcessedItems, List<BibVidStruct> DeletedItems);

        public event ModuleErrorLoggingDelegate Error;
        public event ModuleStandardLoggingDelegate Process;

        protected long OnError(string LogStatement, string BibID_VID, string MetsType, long RelatedLogID)
        {
            if (Error != null)
                return Error(LogStatement, BibID_VID, MetsType, RelatedLogID);

            return -1;
        }

        protected long OnProcess(string LogStatement, string DbLogType, string BibID_VID, string MetsType, long RelatedLogID)
        {
            if (Process != null)
                return Process(LogStatement, DbLogType, BibID_VID, MetsType, RelatedLogID);

            return -1;
        }
    }
}
