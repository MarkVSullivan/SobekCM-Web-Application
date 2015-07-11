using System.Collections.Generic;
using SobekCM.Core.Settings;

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Interface class that all item-level submission package builder modules MUST implement </summary>
    public interface iSubmissionPackageModule
    {
        /// <summary> Arguments passed in to this module, used to determine process details </summary>
        List<string> Arguments { get; set; }

        /// <summary> Link to the instance-wide settings which may be required for this process </summary>
        InstanceWide_Settings Settings { get; set; }

        /// <summary> Method performs the work of the item-level submission package builder module </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        bool DoWork(Incoming_Digital_Resource Resource);

        /// <summary> Method releases all resources </summary>
        void ReleaseResources();

        /// <summary> Event is fired when an error occurs during processing </summary>
        event ModuleErrorLoggingDelegate Error;

        /// <summary> Event is fired to report progress during processing </summary>
        event ModuleStandardLoggingDelegate Process;
    }
}
