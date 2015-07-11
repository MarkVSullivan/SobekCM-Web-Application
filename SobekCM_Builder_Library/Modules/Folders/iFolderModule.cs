#region Using directives

using System.Collections.Generic;
using SobekCM.Core.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Folders
{
    /// <summary> Interface class that all folder-level builder modules MUST implement </summary>
    public interface iFolderModule
    {
        /// <summary> Arguments passed in to this module, used to determine process details </summary>
        List<string> Arguments { get; set; }

        /// <summary> Link to the instance-wide settings which may be required for this process </summary>
        InstanceWide_Settings Settings { get; set;  }

        /// <summary> Method performs the work of the folder-level builder module </summary>
        /// <param name="BuilderFolder"> Builder folder upon which to perform all work </param>
        /// <param name="IncomingPackages"> List of valid incoming packages, which may be modified by this process </param>
        /// <param name="Deletes"> List of valid deletes, which may be modifyed by this process </param>
        void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes);

        /// <summary> Method releases all resources </summary>
        void ReleaseResources();

        /// <summary> Event is fired when an error occurs during processing </summary>
        event ModuleErrorLoggingDelegate Error;

        /// <summary> Event is fired to report progress during processing </summary>
        event ModuleStandardLoggingDelegate Process;
    }
}
