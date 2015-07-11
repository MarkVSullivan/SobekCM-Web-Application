#region Using directives

using System.Collections.Generic;
using SobekCM.Core.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Folders
{
    /// <summary> Abstract class that all (standard) folder-level builder modules extend </summary>
    public abstract class abstractFolderModule : iFolderModule
    {
        /// <summary> Constructor for the abstract abstractFolderModule class </summary>
        protected abstractFolderModule()
        {
            Arguments = new List<string>();
        }

        /// <summary> Arguments passed in to this module, used to determine process details </summary>
        public List<string> Arguments { get; set; }

        /// <summary> Link to the instance-wide settings which may be required for this process </summary>
        public InstanceWide_Settings Settings { get; set; }

        /// <summary> Method performs the work of the folder-level builder module </summary>
        /// <param name="BuilderFolder"> Builder folder upon which to perform all work </param>
        /// <param name="IncomingPackages"> List of valid incoming packages, which may be modified by this process </param>
        /// <param name="Deletes"> List of valid deletes, which may be modifyed by this process </param>
        public abstract void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes);

        /// <summary> Method releases all resources </summary>
        public virtual void ReleaseResources()
        {
            Settings = null;
        }

        /// <summary> Event is fired when an error occurs during processing </summary>
        public event ModuleErrorLoggingDelegate Error;

        /// <summary> Event is fired to report progress during processing </summary>
        public event ModuleStandardLoggingDelegate Process;

        /// <summary> Fire the error event, if a delegate is attached to the event </summary>
        /// <param name="LogStatement"> Statement for the error log entry </param>
        /// <param name="BibID_VID"> BibID and VID, if this error occurred while looking at a single digital resource folder </param>
        /// <param name="MetsType"> Incoming METS type, if identified </param>
        /// <param name="RelatedLogID"> Primary key for a related log entry, if this is a log entry related to another </param>
        /// <returns> Primary key for this related log entry, in case other errors should be attached to this, or -1 if no delegates attached </returns>
        protected long OnError(string LogStatement, string BibID_VID, string MetsType, long RelatedLogID)
        {
            if (Error != null)
                return Error(LogStatement, BibID_VID, MetsType, RelatedLogID);

            return -1;
        }

        /// <summary> Fire the process event, to report progress during processing </summary>
        /// <param name="LogStatement"> Statement for the log entry  </param>
        /// <param name="DbLogType"> Type of log entry </param>
        /// <param name="BibID_VID"> BibID and VID, if this occurred while looking at a single digital resource folder </param>
        /// <param name="MetsType"> Incoming METS type, if identified </param>
        /// <param name="RelatedLogID"> Primary key for a related log entry, if this is a log entry related to another </param>
        /// <returns> Primary key for this related log entry, in case other log entries should be attached to this, or -1 if no delegates attached </returns>
        protected long OnProcess(string LogStatement, string DbLogType, string BibID_VID, string MetsType, long RelatedLogID)
        {
            if (Process != null)
                return Process(LogStatement, DbLogType, BibID_VID, MetsType, RelatedLogID);

            return -1;
        }
    }
}
