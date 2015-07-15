#region Using directives

using System.Collections.Generic;
using SobekCM.Core.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.PreProcess
{
    /// <summary> Interface class that all pre-process builder modules MUST implement </summary>
    public interface iPreProcessModule
    {
        /// <summary> Arguments passed in to this module, used to determine process details </summary>
        List<string> Arguments { get; set; }

        /// <summary> Method performs the work of the pre-process builder module </summary>
        /// <param name="Settings"> Instance-wide settings which may be required for this process </param>
        void DoWork( InstanceWide_Settings Settings );

        /// <summary> Event is fired when an error occurs during processing </summary>
        event ModuleErrorLoggingDelegate Error;

        /// <summary> Event is fired to report progress during processing </summary>
        event ModuleStandardLoggingDelegate Process;
    }
}
