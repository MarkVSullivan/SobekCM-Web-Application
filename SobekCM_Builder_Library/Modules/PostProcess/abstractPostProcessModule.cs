#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Builder_Library.Modules.PostProcess
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
