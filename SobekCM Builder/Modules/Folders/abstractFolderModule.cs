using System.Collections.Generic;
using SobekCM.Library.Builder;

namespace SobekCM.Builder.Modules.Folders
{
    public abstract class abstractFolderModule : iFolderModule
    {

        public abstract void DoWork(Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes);

        public virtual void ReleaseResources()
        {
            // Do nothing by default
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
