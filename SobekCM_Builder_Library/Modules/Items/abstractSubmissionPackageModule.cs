using System.Collections.Generic;
using SobekCM.Core.Settings;

namespace SobekCM.Builder_Library.Modules.Items
{
    public abstract class abstractSubmissionPackageModule : iSubmissionPackageModule
    {
        public List<string> Arguments { get; set; }

        public InstanceWide_Settings Settings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Resource"></param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public abstract bool DoWork(Incoming_Digital_Resource Resource);

        public virtual void ReleaseResources()
        {
            Settings = null;
        }

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
