namespace SobekCM.Builder_Library.Modules.Items
{
    public abstract class abstractSubmissionPackageModule : iSubmissionPackageModule
    {
        public abstract void DoWork(Incoming_Digital_Resource Resource);

        public virtual void ReleaseResources()
        {
            // By default, do nothing
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
